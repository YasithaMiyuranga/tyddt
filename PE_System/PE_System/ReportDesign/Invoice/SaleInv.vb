Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared
Imports MySql.Data.MySqlClient

Public Class SaleInv
    ' Cache for loaded ReportDocument objects to prevent slow Re-initialization
    Private Shared reportCache As New Dictionary(Of String, ReportDocument)()

    Public Property PrintAsRetail As Boolean = False
    
    ' Structures to hold original wholesale data in memory
    Private Structure BillingTotalBackup
        Public SubTotal As Decimal
        Public GrandTotal As Decimal
        Public CashReceived As Decimal
        Public BalanceDue As Decimal
        Public PaidAmount As Decimal
    End Structure

    Private Structure BillingItemBackup
        Public Id As Integer
        Public UnitPrice As Decimal
        Public ItemAmount As Decimal
        Public Discount As Decimal
    End Structure

    Private _hasBackup As Boolean = False
    Private _backupTotals As BillingTotalBackup
    Private _backupItems As New List(Of BillingItemBackup)()
    Private _backupInvoiceNo As String = ""
    Private _backupIsQuotation As Boolean = False

    Public Sub ApplyTemporaryRetailPrices(ByVal invoiceNo As String)
        If String.IsNullOrEmpty(invoiceNo) Then Return
        
        If _hasBackup Then
            If _backupInvoiceNo = invoiceNo Then
                Return
            Else
                RestoreOriginalWholesalePrices()
            End If
        End If
        
        _backupInvoiceNo = invoiceNo
        _backupIsQuotation = invoiceNo.StartsWith("QT")
        Dim mainTable As String = If(_backupIsQuotation, "quotation_billing", "billing")
        Dim itemTable As String = If(_backupIsQuotation, "quotation_billing_item", "billing_item")
        
        _backupItems.Clear()
        _hasBackup = False
        
        Dim connStr As String = Module1.ConnStr
        Using conn As New MySqlConnection(connStr)
            Try
                conn.Open()
                
                ' 1. Fetch and backup main billing record
                Dim billingId As Integer = 0
                Dim selectMainSql As String = $"SELECT id, subtotal, grand_total, cash_received, balance_due, paid_amount FROM {mainTable} WHERE inv_no = @inv"
                Using cmd As New MySqlCommand(selectMainSql, conn)
                    cmd.Parameters.AddWithValue("@inv", invoiceNo)
                    Using dr As MySqlDataReader = cmd.ExecuteReader()
                        If dr.Read() Then
                            billingId = Convert.ToInt32(dr("id"))
                            _backupTotals = New BillingTotalBackup() With {
                                .SubTotal = Convert.ToDecimal(dr("subtotal")),
                                .GrandTotal = Convert.ToDecimal(dr("grand_total")),
                                .CashReceived = Convert.ToDecimal(dr("cash_received")),
                                .BalanceDue = Convert.ToDecimal(dr("balance_due")),
                                .PaidAmount = Convert.ToDecimal(dr("paid_amount"))
                            }
                            _hasBackup = True
                        End If
                    End Using
                End Using
                
                If Not _hasBackup OrElse billingId = 0 Then Return
                
                ' 2. Fetch and backup items
                Dim itemsToUpdate As New List(Of (Id As Integer, Qty As Decimal, RetailPrice As Decimal))()
                
                Dim selectItemsSql As String = $"SELECT bi.id, bi.item_id, bi.quantity, bi.unit_price, bi.item_amount, bi.discount, " &
                                               $"IFNULL(NULLIF(bi.print_retail_price, 0), IFNULL(i.retail_selling_price, bi.unit_price)) as retail_price " &
                                               $"FROM {itemTable} bi " &
                                               $"LEFT JOIN items i ON bi.item_id = i.id " &
                                               $"WHERE bi.billing_id = @bid"
                                               
                Using cmd As New MySqlCommand(selectItemsSql, conn)
                    cmd.Parameters.AddWithValue("@bid", billingId)
                    Using dr As MySqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            Dim itemBackup As New BillingItemBackup() With {
                                .Id = Convert.ToInt32(dr("id")),
                                .UnitPrice = Convert.ToDecimal(dr("unit_price")),
                                .ItemAmount = Convert.ToDecimal(dr("item_amount")),
                                .Discount = Convert.ToDecimal(dr("discount"))
                            }
                            _backupItems.Add(itemBackup)
                            
                            itemsToUpdate.Add((
                                Convert.ToInt32(dr("id")),
                                Convert.ToDecimal(dr("quantity")),
                                Convert.ToDecimal(dr("retail_price"))
                            ))
                        End While
                    End Using
                End Using
                
                ' 3. Update items in the database to use retail prices
                Dim newSubtotal As Decimal = 0
                For Each item In itemsToUpdate
                    Dim newItemAmount As Decimal = item.Qty * item.RetailPrice
                    newSubtotal += newItemAmount
                    
                    Dim updateItemSql As String = $"UPDATE {itemTable} SET unit_price = @uprice, item_amount = @amt, discount = 0 WHERE id = @id"
                    Using cmdUpdate As New MySqlCommand(updateItemSql, conn)
                        cmdUpdate.Parameters.AddWithValue("@uprice", item.RetailPrice)
                        cmdUpdate.Parameters.AddWithValue("@amt", newItemAmount)
                        cmdUpdate.Parameters.AddWithValue("@id", item.Id)
                        cmdUpdate.ExecuteNonQuery()
                    End Using
                Next
                
                ' 4. Calculate new grand total (including VAT if applicable)
                Dim vatRate As Decimal = 0
                Dim selectVatSql As String = $"SELECT v.vat_value FROM {mainTable} b JOIN vat v ON b.vat_id = v.id WHERE b.id = @bid"
                Using cmdVat As New MySqlCommand(selectVatSql, conn)
                    cmdVat.Parameters.AddWithValue("@bid", billingId)
                    Dim res = cmdVat.ExecuteScalar()
                    If res IsNot Nothing AndAlso res IsNot DBNull.Value Then
                        vatRate = Convert.ToDecimal(res)
                    End If
                End Using
                
                Dim newGrandTotal As Decimal = newSubtotal
                If vatRate > 0 Then
                    newGrandTotal = newSubtotal * (1 + (vatRate / 100))
                End If
                
                ' 5. Update main billing record in database
                Dim updateMainSql As String = $"UPDATE {mainTable} SET subtotal = @sub, grand_total = @grand, paid_amount = @grand, cash_received = @grand, balance_due = 0 WHERE id = @bid"
                Using cmdUpdateMain As New MySqlCommand(updateMainSql, conn)
                    cmdUpdateMain.Parameters.AddWithValue("@sub", newSubtotal)
                    cmdUpdateMain.Parameters.AddWithValue("@grand", newGrandTotal)
                    cmdUpdateMain.Parameters.AddWithValue("@bid", billingId)
                    cmdUpdateMain.ExecuteNonQuery()
                End Using
                
            Catch ex As Exception
                MessageBox.Show("Error applying temporary retail prices: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Try
        End Using
    End Sub

    Public Sub RestoreOriginalWholesalePrices()
        If Not _hasBackup OrElse String.IsNullOrEmpty(_backupInvoiceNo) Then Return
        
        Dim mainTable As String = If(_backupIsQuotation, "quotation_billing", "billing")
        Dim itemTable As String = If(_backupIsQuotation, "quotation_billing_item", "billing_item")
        
        Dim connStr As String = Module1.ConnStr
        Using conn As New MySqlConnection(connStr)
            Try
                conn.Open()
                
                ' 1. Restore main billing totals
                Dim updateMainSql As String = $"UPDATE {mainTable} SET subtotal = @sub, grand_total = @grand, cash_received = @cash, balance_due = @bal, paid_amount = @paid WHERE inv_no = @inv"
                Using cmdUpdateMain As New MySqlCommand(updateMainSql, conn)
                    cmdUpdateMain.Parameters.AddWithValue("@sub", _backupTotals.SubTotal)
                    cmdUpdateMain.Parameters.AddWithValue("@grand", _backupTotals.GrandTotal)
                    cmdUpdateMain.Parameters.AddWithValue("@cash", _backupTotals.CashReceived)
                    cmdUpdateMain.Parameters.AddWithValue("@bal", _backupTotals.BalanceDue)
                    cmdUpdateMain.Parameters.AddWithValue("@paid", _backupTotals.PaidAmount)
                    cmdUpdateMain.Parameters.AddWithValue("@inv", _backupInvoiceNo)
                    cmdUpdateMain.ExecuteNonQuery()
                End Using
                
                ' 2. Restore item prices
                For Each item In _backupItems
                    Dim updateItemSql As String = $"UPDATE {itemTable} SET unit_price = @uprice, item_amount = @amt, discount = @disc WHERE id = @id"
                    Using cmdUpdateItem As New MySqlCommand(updateItemSql, conn)
                        cmdUpdateItem.Parameters.AddWithValue("@uprice", item.UnitPrice)
                        cmdUpdateItem.Parameters.AddWithValue("@amt", item.ItemAmount)
                        cmdUpdateItem.Parameters.AddWithValue("@disc", item.Discount)
                        cmdUpdateItem.Parameters.AddWithValue("@id", item.Id)
                        cmdUpdateItem.ExecuteNonQuery()
                    End Using
                Next
                
                _hasBackup = False
                _backupInvoiceNo = ""
                
            Catch ex As Exception
                MessageBox.Show("Error restoring original wholesale prices: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Try
        End Using
    End Sub

    Private Sub SuppressPaymentAndChangeFields(ByRef rpt As ReportDocument)
        Try
            For Each sec As Section In rpt.ReportDefinition.Sections
                For Each obj As ReportObject In sec.ReportObjects
                    ' Handle Text Objects (Labels)
                    If TypeOf obj Is TextObject Then
                        Dim txtObj As TextObject = DirectCast(obj, TextObject)
                        Dim textLower As String = txtObj.Text.ToLower()
                        If textLower.Contains("payment") OrElse 
                           textLower.Contains("change") OrElse 
                           textLower.Contains("cash return") OrElse 
                           textLower.Contains("cashreturn") Then
                            obj.ObjectFormat.EnableSuppress = True
                        End If
                    End If
                    
                    ' Handle Field Objects (Data fields)
                    If TypeOf obj Is FieldObject Then
                        Dim fldObj As FieldObject = DirectCast(obj, FieldObject)
                        Dim nameLower As String = fldObj.Name.ToLower()
                        If nameLower.Contains("cash_received") OrElse 
                           nameLower.Contains("cashreceived") OrElse 
                           nameLower.Contains("change_amount") OrElse 
                           nameLower.Contains("changeamount") OrElse 
                           nameLower.Contains("paid_amount") OrElse 
                           nameLower.Contains("paidamount") Then
                            obj.ObjectFormat.EnableSuppress = True
                        End If
                    End If
                Next
            Next
        Catch ex As Exception
            ' Silent error
        End Try
    End Sub

    Private Sub SaleInv_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.KeyPreview = True
        InitializeReportTypes()
        ' Removed InitializePrinters from Load to prevent UI freeze on startup
        UpdateUILabels() ' Set initial labels
    End Sub

    Private Sub cmbReportType_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles cmbReportType.SelectionChangeCommitted
        UpdateUILabels()
    End Sub

    Private Sub UpdateUILabels()
        If cmbReportType.SelectedIndex = 12 Then
            lblInvoiceNo.Text = "Item ID:"
            btnShowInvoice.Text = "Show Barcode"
            Me.Text = "Barcode Label Viewer"
        Else
            lblInvoiceNo.Text = "Invoice No:"
            btnShowInvoice.Text = "Show Invoice"
            Me.Text = "Sales Invoice Report"
        End If
    End Sub

    Private Sub cmbPrinter_DropDown(sender As Object, e As EventArgs) Handles cmbPrinter.DropDown
        ' Lazy load printers only when the user actually clicks the dropdown
        If cmbPrinter.Items.Count = 0 Then
            InitializePrinters()
        End If
    End Sub

    Private Sub InitializePrinters()
        Try
            Dim currentVal As String = cmbPrinter.Text
            cmbPrinter.Items.Clear()
            For Each printer As String In System.Drawing.Printing.PrinterSettings.InstalledPrinters
                cmbPrinter.Items.Add(printer)
            Next

            ' Select default printer
            If Not String.IsNullOrEmpty(currentVal) AndAlso cmbPrinter.Items.Contains(currentVal) Then
                cmbPrinter.SelectedItem = currentVal
            Else
                Dim printDoc As New System.Drawing.Printing.PrintDocument()
                Dim defaultPrinter As String = printDoc.PrinterSettings.PrinterName
                if cmbPrinter.Items.Contains(defaultPrinter) Then
                    cmbPrinter.SelectedItem = defaultPrinter
                ElseIf cmbPrinter.Items.Count > 0 Then
                    cmbPrinter.SelectedIndex = 0
                End If
            End If
        Catch ex As Exception
            ' Silent fail for printer initialization
        End Try
    End Sub

    Public Sub ShowReport(ByVal rptDoc As ReportDocument, ByVal reportTypeIndex As Integer, Optional ByVal silentMode As Boolean = False)
        Try
            ' Fix known subreport bugs in Crystal Reports design files dynamically
            Try
                For Each subRpt As ReportDocument In rptDoc.Subreports
                    If subRpt.Name.Equals("fullcreditre", StringComparison.OrdinalIgnoreCase) Then
                        ' The .rpt file mistakenly links CusID to the amount field instead of customer_id
                        subRpt.RecordSelectionFormula = "{customer_credit1.customer_id} = {?Pm-customer_payments1.CusID}"
                    End If
                Next
            Catch ex As Exception
            End Try

            ' Ensure ComboBox items are loaded before setting SelectedIndex
            InitializeReportTypes()

            If reportTypeIndex >= 0 AndAlso reportTypeIndex < cmbReportType.Items.Count Then
                cmbReportType.SelectedIndex = reportTypeIndex
            End If

            ' Ensure the report uses the current database connection
            SetReportConnection(rptDoc)

            ' Display the report
            CrystalReportViewer1.ReuseParameterValuesOnRefresh = True
            CrystalReportViewer1.ReportSource = rptDoc
            CrystalReportViewer1.RefreshReport()
            
            ' Ensure the form is visible only if not in silent mode
            If Not silentMode Then
                Me.Show()
                Me.BringToFront()
            End If

        Catch ex As Exception
            If Not silentMode Then MessageBox.Show("Error displaying report: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub InitializeReportTypes()
        If cmbReportType.Items.Count = 0 Then
            cmbReportType.Items.Clear()
            cmbReportType.Items.Add("POS Invoice")
            cmbReportType.Items.Add("Standard Invoice")
            cmbReportType.Items.Add("Purchase Invoice")
            cmbReportType.Items.Add("Sale Return Invoice")
            cmbReportType.Items.Add("Quotation")
            cmbReportType.Items.Add("Quotation POS")
            cmbReportType.Items.Add("Customer Credit")
            cmbReportType.Items.Add("Customer Cheque")
            cmbReportType.Items.Add("Supplier Cheque")
            cmbReportType.Items.Add("Supplier Debit")
            cmbReportType.Items.Add("Receiving Stock")
            cmbReportType.Items.Add("Sending Stock")
            cmbReportType.Items.Add("Barcode Labels")
            cmbReportType.Items.Add("Purchase Request")
            cmbReportType.Items.Add("Purchase Return Invoice")
            cmbReportType.SelectedIndex = 0
        End If
    End Sub

    Private Sub btnShowInvoice_Click(sender As Object, e As EventArgs) Handles btnShowInvoice.Click
        ShowReport(txtInvoiceNo.Text.Trim(), cmbReportType.SelectedIndex)
    End Sub

    Private Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click
        Try
            If CrystalReportViewer1.ReportSource IsNot Nothing Then
                Dim rpt As ReportDocument = DirectCast(CrystalReportViewer1.ReportSource, ReportDocument)

                If cmbPrinter.SelectedIndex <> -1 Then
                    rpt.PrintOptions.PrinterName = cmbPrinter.SelectedItem.ToString()
                End If

                ' Always print 1 copy at report level.
                ' For Barcode labels, the Crystal Report itself already contains the correct number
                ' of rows/records. Passing copies > 1 here would multiply (e.g. 3 items x 3 = 9 labels).
                rpt.PrintToPrinter(1, False, 0, 0)
                MessageBox.Show("Printing started.", "Print", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MessageBox.Show("Printing error: " & ex.Message)
        End Try
    End Sub

    Public Sub ShowReport(ByVal invoiceNo As String, ByVal reportTypeIndex As Integer, Optional ByVal silentMode As Boolean = False, Optional ByVal isReturn As Boolean = False, Optional ByVal selectionFormula As String = "", Optional ByVal numCopies As Integer = 1, Optional ByVal supplierId As Integer = 0, Optional ByVal isDuplicatePrint As Boolean = False)
        Try
            ' Ensure ComboBox items are loaded before setting SelectedIndex
            InitializeReportTypes()

            If String.IsNullOrEmpty(invoiceNo) AndAlso String.IsNullOrEmpty(selectionFormula) Then
                If Not silentMode Then MessageBox.Show("Please enter an ID or Invoice Number.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtInvoiceNo.Focus()
                Return
            End If

            ' Automatically set the invoice number in the text box
            txtInvoiceNo.Text = invoiceNo
            If PrintAsRetail Then
                ApplyTemporaryRetailPrices(invoiceNo)
            End If
            If reportTypeIndex >= 0 AndAlso reportTypeIndex < cmbReportType.Items.Count Then
                cmbReportType.SelectedIndex = reportTypeIndex
            End If

            ' Escape single quotes to avoid Crystal syntax errors
            Dim escapedInvoiceNo As String = invoiceNo.Replace("'", "''")

            ' Determine report file name
            Dim rpt As ReportDocument = Nothing
            Select Case reportTypeIndex
                Case 0
                    rpt = New SaleInvoicePOS()
                Case 1
                    rpt = New SeleInvoice()
                Case 2
                    rpt = New PuchaInvoice()
                Case 3
                    rpt = New SaleReturnInv()
                Case 4
                    rpt = New Quate()
                Case 5
                    rpt = New QuatePOS()
                Case 6
                    rpt = New CUSTOMER_CREDIT()
                Case 7
                    rpt = New Customercheque()
                Case 8
                    rpt = New suppliercheque()
                Case 9
                    rpt = New supplierdebit()
                Case 10
                    rpt = New receiving_stock_report()
                Case 11
                    rpt = New sending_stock_report()
                Case 12
                    rpt = New BarCode()
                Case 13
                    rpt = New puchaseInvoiceRequest()
                Case 14
                    rpt = New purchasereturn()
                Case Else
                    rpt = New SaleInvoicePOS()
            End Select

            SetReportConnection(rpt) ' Force current database connection to ensure it matches Module1.ConnStr
            
            ' Dynamic P/O number printing override for non-Credit billing types
            Try
                If reportTypeIndex = 0 OrElse reportTypeIndex = 1 OrElse reportTypeIndex = 4 OrElse reportTypeIndex = 5 Then
                    Dim poNumber As String = ""
                    Dim queryTable As String = "billing"
                    If invoiceNo.StartsWith("QT") Then
                        queryTable = "quotation_billing"
                    End If
                    Using conn As New MySql.Data.MySqlClient.MySqlConnection(Module1.ConnStr)
                        conn.Open()
                        Dim q As String = "SELECT po_number FROM " & queryTable & " WHERE inv_no = @inv"
                        Using cmd As New MySql.Data.MySqlClient.MySqlCommand(q, conn)
                            cmd.Parameters.AddWithValue("@inv", invoiceNo)
                            Dim res = cmd.ExecuteScalar()
                            If res IsNot Nothing AndAlso res IsNot DBNull.Value Then
                                poNumber = res.ToString().Trim()
                            End If
                        End Using
                    End Using

                    Dim shouldSuppress As Boolean = String.IsNullOrEmpty(poNumber) OrElse String.Equals(poNumber, "not", StringComparison.OrdinalIgnoreCase)
                    
                    Dim rcdProp = rpt.GetType().GetProperty("ReportClientDocument")
                    If rcdProp IsNot Nothing Then
                        Dim rcd As Object = rcdProp.GetValue(rpt, Nothing)
                        If rcd IsNot Nothing Then
                            Dim objs As Object = rcd.ReportDefController.ReportObjectController.GetAllReportObjects()
                            For Each oldObj As Object In objs
                                Dim objName As String = oldObj.Name.ToString()
                                If String.Equals(objName, "Text14", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(objName, "ponumber1", StringComparison.OrdinalIgnoreCase) Then
                                    Dim newObj As Object = oldObj.Clone(True)
                                    newObj.Format.ConditionFormulas.RemoveAll()
                                    newObj.Format.EnableSuppress = shouldSuppress
                                    rcd.ReportDefController.ReportObjectController.Modify(oldObj, newObj)
                                End If
                            Next
                        End If
                    End If
                End If
            Catch exPo As Exception
                ' Silent execution to avoid blocking the main report load
            End Try

            ' Dynamic Original/Duplicate label printing override
            Try
                If reportTypeIndex = 0 OrElse reportTypeIndex = 1 Then
                    Dim rcdProp = rpt.GetType().GetProperty("ReportClientDocument")
                    If rcdProp IsNot Nothing Then
                        Dim rcd As Object = rcdProp.GetValue(rpt, Nothing)
                        If rcd IsNot Nothing Then
                            Dim objs As Object = rcd.ReportDefController.ReportObjectController.GetAllReportObjects()
                            For Each oldObj As Object In objs
                                Dim objName As String = oldObj.Name.ToString()
                                Dim isOriginal As Boolean = False
                                Dim isDuplicate As Boolean = False
                                
                                If reportTypeIndex = 0 Then ' POS Invoice
                                    If String.Equals(objName, "Text28", StringComparison.OrdinalIgnoreCase) Then isOriginal = True
                                    If String.Equals(objName, "Text15", StringComparison.OrdinalIgnoreCase) Then isDuplicate = True
                                ElseIf reportTypeIndex = 1 Then ' Standard Invoice
                                    If String.Equals(objName, "Text29", StringComparison.OrdinalIgnoreCase) Then isOriginal = True
                                    If String.Equals(objName, "Text28", StringComparison.OrdinalIgnoreCase) Then isDuplicate = True
                                End If

                                If isOriginal OrElse isDuplicate Then
                                    Dim newObj As Object = oldObj.Clone(True)
                                    newObj.Format.ConditionFormulas.RemoveAll()
                                    
                                    If isDuplicatePrint Then
                                        ' It's a reprint: show duplicate, hide original
                                        newObj.Format.EnableSuppress = isOriginal
                                    Else
                                        ' First time: show original, hide duplicate
                                        newObj.Format.EnableSuppress = isDuplicate
                                    End If
                                    
                                    rcd.ReportDefController.ReportObjectController.Modify(oldObj, newObj)
                                End If
                            Next
                        End If
                    End If
                End If
            Catch exOrigDup As Exception
                ' Silent execution to avoid blocking the main report load
            End Try

            ' Dynamic Advance Payment printing override (suppress if 0 or empty)
            Try
                If reportTypeIndex = 0 OrElse reportTypeIndex = 1 Then
                    Dim advPayAmt As Decimal = 0
                    Dim queryTable As String = "billing"
                    If invoiceNo.StartsWith("QT") Then
                        queryTable = "quotation_billing"
                    End If
                    Using conn As New MySql.Data.MySqlClient.MySqlConnection(Module1.ConnStr)
                        conn.Open()
                        ' Try adv_pay_amount column first
                        Dim q As String = "SELECT adv_pay_amount FROM " & queryTable & " WHERE inv_no = @inv"
                        If queryTable = "billing" Then
                            q = "SELECT adv_pay_amount FROM billing WHERE inv_no = @inv OR printed_inv_no = @inv"
                        End If
                        Try
                            Using cmd As New MySql.Data.MySqlClient.MySqlCommand(q, conn)
                                cmd.Parameters.AddWithValue("@inv", invoiceNo)
                                Dim res = cmd.ExecuteScalar()
                                If res IsNot Nothing AndAlso res IsNot DBNull.Value Then
                                    Decimal.TryParse(res.ToString(), advPayAmt)
                                End If
                            End Using
                        Catch ex As Exception
                            ' Fallback to advance_payment column
                            Try
                                Dim qFallback As String = "SELECT advance_payment FROM " & queryTable & " WHERE inv_no = @inv"
                                If queryTable = "billing" Then
                                    qFallback = "SELECT advance_payment FROM billing WHERE inv_no = @inv OR printed_inv_no = @inv"
                                End If
                                Using cmd As New MySql.Data.MySqlClient.MySqlCommand(qFallback, conn)
                                    cmd.Parameters.AddWithValue("@inv", invoiceNo)
                                    Dim res = cmd.ExecuteScalar()
                                    If res IsNot Nothing AndAlso res IsNot DBNull.Value Then
                                        Decimal.TryParse(res.ToString(), advPayAmt)
                                    End If
                                End Using
                            Catch
                            End Try
                        End Try
                    End Using

                    Dim suppressAdv As Boolean = (advPayAmt <= 0)
                    Dim targetSectionName As String = If(reportTypeIndex = 0, "ReportFooterSection8", "ReportFooterSection9")
                    Dim targetHeight As Integer = If(suppressAdv, 720, 960)
                    Dim targetObjTop As Integer = If(suppressAdv, 0, 720)
                    Dim targetObjHeight As Integer = If(suppressAdv, 0, 240)
                    
                    Dim rcdProp = rpt.GetType().GetProperty("ReportClientDocument")
                    If rcdProp IsNot Nothing Then
                        Dim rcd As Object = rcdProp.GetValue(rpt, Nothing)
                        If rcd IsNot Nothing Then
                            Dim objs As Object = rcd.ReportDefController.ReportObjectController.GetAllReportObjects()
                            For Each oldObj As Object In objs
                                Dim objName As String = oldObj.Name.ToString()
                                If String.Equals(objName, "Text23", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(objName, "advpayamount1", StringComparison.OrdinalIgnoreCase) Then
                                    Dim newObj As Object = oldObj.Clone(True)
                                    newObj.Top = targetObjTop
                                    newObj.Height = targetObjHeight
                                    newObj.Format.ConditionFormulas.RemoveAll()
                                    newObj.Format.EnableSuppress = suppressAdv
                                    rcd.ReportDefController.ReportObjectController.Modify(oldObj, newObj)
                                End If
                            Next

                            ' Modify the section height to collapse the gap
                            Dim areas As Object = rcd.ReportDefController.ReportDefinition.Areas
                            Dim targetSection As Object = Nothing
                            For i As Integer = 0 To areas.Count - 1
                                Dim area As Object = areas(i)
                                For j As Integer = 0 To area.Sections.Count - 1
                                    Dim section As Object = area.Sections(j)
                                    If String.Equals(section.Name.ToString(), targetSectionName, StringComparison.OrdinalIgnoreCase) Then
                                        targetSection = section
                                        Exit For
                                    End If
                                Next
                                If targetSection IsNot Nothing Then Exit For
                            Next

                            If targetSection IsNot Nothing Then
                                rcd.ReportDefController.ReportSectionController.SetProperty(targetSection, 2, targetHeight)
                            End If
                        End If
                    End If
                End If
            Catch exAdv As Exception
                ' Silent execution to avoid blocking the main report load
            End Try

            ' Dynamic Customer VAT ID printing override (suppress if empty)
            Try
                If reportTypeIndex = 0 OrElse reportTypeIndex = 1 Then ' POS Invoice or Standard Invoice
                    Dim cusVatId As String = ""
                    Dim queryTable As String = "billing"
                    If invoiceNo.StartsWith("QT") Then
                        queryTable = "quotation_billing"
                    End If
                    Using conn As New MySql.Data.MySqlClient.MySqlConnection(Module1.ConnStr)
                        conn.Open()
                        Dim q As String = "SELECT cus_vat_id FROM " & queryTable & " WHERE inv_no = @inv"
                        If queryTable = "billing" Then
                            q = "SELECT cus_vat_id FROM billing WHERE inv_no = @inv OR printed_inv_no = @inv"
                        End If
                        Try
                            Using cmd As New MySql.Data.MySqlClient.MySqlCommand(q, conn)
                                cmd.Parameters.AddWithValue("@inv", invoiceNo)
                                Dim res = cmd.ExecuteScalar()
                                If res IsNot Nothing AndAlso res IsNot DBNull.Value Then
                                    cusVatId = res.ToString().Trim()
                                End If
                            End Using
                        Catch
                        End Try
                    End Using

                    Dim suppressVatId As Boolean = String.IsNullOrEmpty(cusVatId)

                    ' Find the names of the objects to hide/show using the Engine
                    Dim vatObjectsToHide As New List(Of String)()
                    Dim taxInvoiceObjs As New List(Of String)()
                    Dim normalInvoiceObjs As New List(Of String)()

                    For Each sec As CrystalDecisions.CrystalReports.Engine.Section In rpt.ReportDefinition.Sections
                        For Each obj As CrystalDecisions.CrystalReports.Engine.ReportObject In sec.ReportObjects
                            If obj.Name.IndexOf("cus_vat_id", StringComparison.OrdinalIgnoreCase) >= 0 Then
                                vatObjectsToHide.Add(obj.Name)
                            ElseIf TypeOf obj Is CrystalDecisions.CrystalReports.Engine.TextObject Then
                                Dim txtObj = DirectCast(obj, CrystalDecisions.CrystalReports.Engine.TextObject)
                                Dim tStr As String = txtObj.Text
                                If Not String.IsNullOrEmpty(tStr) Then
                                    Dim upperStr As String = tStr.ToUpper().Trim()
                                    ' Explicitly avoid the company's "Regd VAT No" label
                                    If upperStr.Contains("REGD VAT") OrElse upperStr.Contains("REGD. VAT") Then
                                        ' skip
                                    ElseIf upperStr.Contains("VAT ID") OrElse upperStr.Contains("CUS VAT") OrElse upperStr.Contains("CUSTOMER VAT") Then
                                        vatObjectsToHide.Add(obj.Name)
                                    ElseIf upperStr = "TAX INVOICE" OrElse upperStr = "TAX  INVOICE" Then
                                        taxInvoiceObjs.Add(obj.Name)
                                    ElseIf upperStr = "SALE INVOICE" OrElse upperStr = "SALES INVOICE" OrElse upperStr = "INVOICE" Then
                                        normalInvoiceObjs.Add(obj.Name)
                                    End If
                                End If
                            End If
                        Next
                    Next

                    ' Apply suppression and layout adjustments using the ReportClientDocument
                    Dim rcdProp = rpt.GetType().GetProperty("ReportClientDocument")
                    If rcdProp IsNot Nothing Then
                        Dim rcd As Object = rcdProp.GetValue(rpt, Nothing)
                        If rcd IsNot Nothing Then
                            Dim objs As Object = rcd.ReportDefController.ReportObjectController.GetAllReportObjects()
                            For Each oldObj As Object In objs
                                Dim objName As String = oldObj.Name.ToString()
                                
                                ' Toggle VAT ID fields
                                If vatObjectsToHide.Contains(objName) Then
                                    Dim newObj As Object = oldObj.Clone(True)
                                    newObj.Format.ConditionFormulas.RemoveAll()
                                    newObj.Format.EnableSuppress = suppressVatId
                                    
                                    ' Prevent overlap on A4 by shifting the field and label to the far right side of the invoice
                                    If Not suppressVatId AndAlso reportTypeIndex = 1 Then
                                        If objName.IndexOf("cus_vat_id", StringComparison.OrdinalIgnoreCase) >= 0 Then
                                            ' Database Field Value -> move far right
                                            newObj.Left = 7800
                                        Else
                                            ' Text Label -> move right, just before the value
                                            newObj.Left = 6000
                                        End If
                                    End If

                                    rcd.ReportDefController.ReportObjectController.Modify(oldObj, newObj)
                                    
                                ' Toggle TAX INVOICE title
                                ElseIf taxInvoiceObjs.Contains(objName) Then
                                    Dim newObj As Object = oldObj.Clone(True)
                                    newObj.Format.ConditionFormulas.RemoveAll()
                                    newObj.Format.EnableSuppress = suppressVatId ' Suppress TAX INVOICE if no VAT
                                    rcd.ReportDefController.ReportObjectController.Modify(oldObj, newObj)
                                    
                                ' Toggle SALE INVOICE / INVOICE title
                                ElseIf normalInvoiceObjs.Contains(objName) Then
                                    Dim newObj As Object = oldObj.Clone(True)
                                    newObj.Format.ConditionFormulas.RemoveAll()
                                    newObj.Format.EnableSuppress = Not suppressVatId ' Suppress normal invoice if HAS VAT
                                    rcd.ReportDefController.ReportObjectController.Modify(oldObj, newObj)
                                End If
                            Next
                        End If
                    End If
                End If
            Catch exVat As Exception
                ' Silent execution to avoid blocking the main report load
            End Try

            ' Dynamic Cheque & Change/Credit Payment printing override (suppress if 0 or empty)
            Try
                If reportTypeIndex = 0 OrElse reportTypeIndex = 1 Then
                    Dim chequeAmt As Decimal = 0
                    Dim changeAmt As Decimal = 0
                    Dim creditAmt As Decimal = 0
                    Dim queryTable As String = "billing"
                    If invoiceNo.StartsWith("QT") Then
                        queryTable = "quotation_billing"
                    End If
                    Using conn As New MySql.Data.MySqlClient.MySqlConnection(Module1.ConnStr)
                        conn.Open()
                        Dim q As String = "SELECT cheque_balance_due, change_amount, credit_balance_due FROM " & queryTable & " WHERE inv_no = @inv"
                        If queryTable = "billing" Then
                            q = "SELECT cheque_balance_due, change_amount, credit_balance_due FROM billing WHERE inv_no = @inv OR printed_inv_no = @inv"
                        End If
                        Using cmd As New MySql.Data.MySqlClient.MySqlCommand(q, conn)
                            cmd.Parameters.AddWithValue("@inv", invoiceNo)
                            Using reader As MySql.Data.MySqlClient.MySqlDataReader = cmd.ExecuteReader()
                                If reader.Read() Then
                                    If Not reader.IsDBNull(0) Then Decimal.TryParse(reader(0).ToString(), chequeAmt)
                                    If reader.FieldCount > 1 AndAlso Not reader.IsDBNull(1) Then Decimal.TryParse(reader(1).ToString(), changeAmt)
                                    If reader.FieldCount > 2 AndAlso Not reader.IsDBNull(2) Then Decimal.TryParse(reader(2).ToString(), creditAmt)
                                End If
                            End Using
                        End Using
                    End Using

                    Dim suppressChq As Boolean = (chequeAmt <= 0)
                    Dim suppressChangeCredit As Boolean = (changeAmt = 0 AndAlso creditAmt <= 0)
                    
                    ' Standard Invoice formatting
                    If reportTypeIndex = 1 Then
                        Try
                            ' We no longer suppress ReportFooterSection11 entirely to avoid hiding terms & conditions
                            ' rpt.ReportDefinition.Sections("ReportFooterSection11").SectionFormat.EnableSuppress = suppressChq
                        Catch ex As Exception
                        End Try
                        Try
                            rpt.ReportDefinition.Sections("ReportFooterSection10").SectionFormat.EnableSuppress = If(PrintAsRetail, True, suppressChangeCredit)
                        Catch ex As Exception
                        End Try
                    End If

                    ' POS Invoice formatting
                    If reportTypeIndex = 0 Then
                        Try
                            ' We no longer suppress ReportFooterSection4 entirely to avoid hiding other fields
                            ' rpt.ReportDefinition.Sections("ReportFooterSection4").SectionFormat.EnableSuppress = suppressChq
                        Catch ex As Exception
                        End Try
                        Try
                            rpt.ReportDefinition.Sections("ReportFooterSection3").SectionFormat.EnableSuppress = If(PrintAsRetail, True, suppressChangeCredit)
                        Catch ex As Exception
                        End Try
                    End If

                    ' Dynamic object-level suppression for Cheque Balance Due
                    Try
                        Dim rcdProp = rpt.GetType().GetProperty("ReportClientDocument")
                        If rcdProp IsNot Nothing Then
                            Dim rcd As Object = rcdProp.GetValue(rpt, Nothing)
                            If rcd IsNot Nothing Then
                                Dim objs As Object = rcd.ReportDefController.ReportObjectController.GetAllReportObjects()
                                For Each oldObj As Object In objs
                                    Dim objName As String = oldObj.Name.ToString()
                                    Dim isChequeLabel As Boolean = False
                                    If reportTypeIndex = 0 Then ' POS Invoice
                                        If String.Equals(objName, "Text8", StringComparison.OrdinalIgnoreCase) Then isChequeLabel = True
                                    ElseIf reportTypeIndex = 1 Then ' Standard Invoice
                                        If String.Equals(objName, "Text15", StringComparison.OrdinalIgnoreCase) Then isChequeLabel = True
                                    End If
                                    If String.Equals(objName, "chequebalancedue1", StringComparison.OrdinalIgnoreCase) Then isChequeLabel = True

                                    If isChequeLabel Then
                                        Dim newObj As Object = oldObj.Clone(True)
                                        newObj.Format.ConditionFormulas.RemoveAll()
                                        newObj.Format.EnableSuppress = suppressChq
                                        If suppressChq Then
                                            newObj.Height = 0
                                        Else
                                            newObj.Height = If(String.Equals(objName, "chequebalancedue1", StringComparison.OrdinalIgnoreCase), 211, 240)
                                        End If
                                        rcd.ReportDefController.ReportObjectController.Modify(oldObj, newObj)
                                    End If
                                Next
                            End If
                        End If
                    Catch ex As Exception
                    End Try
                End If
            Catch exChq As Exception
                ' Silent execution to avoid blocking the main report load
            End Try

            ' Dynamic Cheque & Credit Payment override for Purchase Invoice (reportTypeIndex 2) and Purchase Return (reportTypeIndex 14)
            Try
                If reportTypeIndex = 2 OrElse reportTypeIndex = 14 Then
                    Dim chequeAmt As Decimal = 0
                    Dim creditAmt As Decimal = 0
                    
                    Using conn As New MySql.Data.MySqlClient.MySqlConnection(Module1.ConnStr)
                        conn.Open()
                        Dim q As String = "SELECT cheque_balance_due, credit_balance_due FROM purchasing WHERE pur_id = @inv"
                        Using cmd As New MySql.Data.MySqlClient.MySqlCommand(q, conn)
                            cmd.Parameters.AddWithValue("@inv", invoiceNo)
                            Using reader As MySql.Data.MySqlClient.MySqlDataReader = cmd.ExecuteReader()
                                If reader.Read() Then
                                    If Not reader.IsDBNull(0) Then Decimal.TryParse(reader(0).ToString(), chequeAmt)
                                    If Not reader.IsDBNull(1) Then Decimal.TryParse(reader(1).ToString(), creditAmt)
                                End If
                            End Using
                        End Using
                    End Using

                    Dim suppressCredit As Boolean = (creditAmt <= 0)
                    Dim suppressCheque As Boolean = (chequeAmt <= 0)

                    Dim rcdProp = rpt.GetType().GetProperty("ReportClientDocument")
                    If rcdProp IsNot Nothing Then
                        Dim rcd As Object = rcdProp.GetValue(rpt, Nothing)
                        If rcd IsNot Nothing Then
                            Dim objs As Object = rcd.ReportDefController.ReportObjectController.GetAllReportObjects()
                            
                            Dim creditLabelName As String = If(reportTypeIndex = 2, "Text5", "Text21")
                            Dim chequeLabelName As String = If(reportTypeIndex = 2, "Text11", "Text9")
                            Dim footer1Name As String = If(reportTypeIndex = 2, "Text17", "Text18")
                            Dim footer2Name As String = If(reportTypeIndex = 2, "Text18", "Text19")

                            Dim targetSecHeight As Integer = If(reportTypeIndex = 2, 2021, 1661)
                            Dim shiftFooter1Top As Integer = If(reportTypeIndex = 2, 1800, 1440)
                            Dim shiftFooter2Top As Integer = If(reportTypeIndex = 2, 1560, 1200)
                            Dim shiftChequeTop As Integer = If(reportTypeIndex = 2, 1200, 960)

                            If suppressCredit Then
                                targetSecHeight -= 240
                                shiftFooter1Top -= 240
                                shiftFooter2Top -= 240
                                shiftChequeTop -= 240
                            End If

                            If suppressCheque Then
                                targetSecHeight -= 240
                                shiftFooter1Top -= 240
                                shiftFooter2Top -= 240
                            End If

                            For Each oldObj As Object In objs
                                Dim objName As String = oldObj.Name.ToString()
                                
                                If String.Equals(objName, creditLabelName, StringComparison.OrdinalIgnoreCase) OrElse _
                                   String.Equals(objName, "creditbalancedue1", StringComparison.OrdinalIgnoreCase) Then
                                    Dim newObj As Object = oldObj.Clone(True)
                                    newObj.Format.ConditionFormulas.RemoveAll()
                                    newObj.Format.EnableSuppress = suppressCredit
                                    If suppressCredit Then
                                        newObj.Height = 0
                                    Else
                                        newObj.Height = If(String.Equals(objName, creditLabelName, StringComparison.OrdinalIgnoreCase), 240, 221)
                                    End If
                                    rcd.ReportDefController.ReportObjectController.Modify(oldObj, newObj)
                                    
                                ElseIf String.Equals(objName, chequeLabelName, StringComparison.OrdinalIgnoreCase) OrElse _
                                       String.Equals(objName, "chequebalancedue1", StringComparison.OrdinalIgnoreCase) Then
                                    Dim newObj As Object = oldObj.Clone(True)
                                    newObj.Top = shiftChequeTop
                                    newObj.Format.ConditionFormulas.RemoveAll()
                                    newObj.Format.EnableSuppress = suppressCheque
                                    If suppressCheque Then
                                        newObj.Height = 0
                                    Else
                                        newObj.Height = If(String.Equals(objName, chequeLabelName, StringComparison.OrdinalIgnoreCase), 240, 221)
                                    End If
                                    rcd.ReportDefController.ReportObjectController.Modify(oldObj, newObj)
                                
                                ElseIf String.Equals(objName, footer1Name, StringComparison.OrdinalIgnoreCase) Then
                                    Dim newObj As Object = oldObj.Clone(True)
                                    newObj.Top = shiftFooter1Top
                                    rcd.ReportDefController.ReportObjectController.Modify(oldObj, newObj)
                                    
                                ElseIf String.Equals(objName, footer2Name, StringComparison.OrdinalIgnoreCase) Then
                                    Dim newObj As Object = oldObj.Clone(True)
                                    newObj.Top = shiftFooter2Top
                                    rcd.ReportDefController.ReportObjectController.Modify(oldObj, newObj)
                                End If
                            Next

                            Dim areas As Object = rcd.ReportDefController.ReportDefinition.Areas
                            Dim targetSection As Object = Nothing
                            For i As Integer = 0 To areas.Count - 1
                                Dim area As Object = areas(i)
                                For j As Integer = 0 To area.Sections.Count - 1
                                    Dim section As Object = area.Sections(j)
                                    If String.Equals(section.Name.ToString(), "Section4", StringComparison.OrdinalIgnoreCase) Then
                                        targetSection = section
                                        Exit For
                                    End If
                                Next
                                If targetSection IsNot Nothing Then Exit For
                            Next
                            If targetSection IsNot Nothing Then
                                rcd.ReportDefController.ReportSectionController.SetProperty(targetSection, 2, targetSecHeight)
                            End If
                        End If
                    End If
                End If
            Catch exPur As Exception
                ' Silent execution to avoid blocking the main report load
            End Try

            ' Clear selection formula to prevent cumulative filters from cached report documents
            rpt.RecordSelectionFormula = ""

            ' Set Selection Formula
            If Not String.IsNullOrEmpty(selectionFormula) AndAlso reportTypeIndex <> 14 Then
                rpt.RecordSelectionFormula = selectionFormula
            ElseIf invoiceNo <> "ALL" AndAlso Not String.IsNullOrEmpty(invoiceNo) Then
                Select Case reportTypeIndex
                    Case 0, 1 ' Sale Invoices (POS & Standard)
                        ' Check both internal and printed invoice numbers
                        ApplyRobustFilter(rpt, {"billing", "billing1", "Command"}, {"inv_no", "printed_inv_no"}, escapedInvoiceNo, False)
                    Case 2 ' Purchase
                        Dim activeSupId As Integer = supplierId
                        If activeSupId = 0 Then
                            ' Fallback: fetch the supplier ID for this invoice
                            Try
                                Using conn As New MySql.Data.MySqlClient.MySqlConnection(Module1.ConnStr)
                                    conn.Open()
                                    Dim q As String = "SELECT supplier_id FROM purchasing WHERE pur_id = @inv ORDER BY date DESC LIMIT 1"
                                    Using cmd As New MySql.Data.MySqlClient.MySqlCommand(q, conn)
                                        cmd.Parameters.AddWithValue("@inv", invoiceNo)
                                        Dim res = cmd.ExecuteScalar()
                                        If res IsNot Nothing AndAlso res IsNot DBNull.Value Then
                                            activeSupId = Convert.ToInt32(res)
                                        End If
                                    End Using
                                End Using
                            Catch
                            End Try
                        End If

                        Dim formula As String = "{purchasing1.pur_id} = '" & escapedInvoiceNo & "'"
                        If activeSupId > 0 Then
                            formula &= " AND {purchasing1.supplier_id} = " & activeSupId
                            
                            ' Dynamically discover items_stock table in the report and filter it
                            For Each tbl As Table In rpt.Database.Tables
                                If tbl.Name.ToLower().Contains("items_stock") Then
                                    formula &= " AND {" & tbl.Name & ".supplier_id} = " & activeSupId
                                End If
                            Next
                        End If
                        rpt.RecordSelectionFormula = formula
                    Case 3 ' Sale Return
                        rpt.RecordSelectionFormula = "{sales_return1.inv_no} = '" & escapedInvoiceNo & "'"
                    Case 14 ' Purchase Return
                        Dim finalFormula As String = selectionFormula
                        
                        If String.IsNullOrEmpty(finalFormula) Then
                            finalFormula = "{purchase_return1.pur_id} = '" & escapedInvoiceNo & "'"
                        End If

                        ' ALWAYS append the missing join conditions to prevent Cartesian product
                        finalFormula &= " AND {purchase_return1.pur_id} = {purchasing1.pur_id} AND {purchase_return1.description} = {ITEMS_STOCK_ALIAS.description}"

                        If finalFormula.Contains("ITEMS_STOCK_ALIAS") Then
                            Dim stockAlias As String = "items_stock1"
                            For Each tbl As Table In rpt.Database.Tables
                                If tbl.Name.ToLower().Contains("items_stock") Then
                                    stockAlias = tbl.Name
                                    Exit For
                                End If
                            Next
                            finalFormula = finalFormula.Replace("ITEMS_STOCK_ALIAS", stockAlias)
                        End If

                        rpt.RecordSelectionFormula = finalFormula
                    Case 4, 5 ' Quotation
                        ' Use a more direct filter for Quotations to ensure reliability
                        rpt.RecordSelectionFormula = "" ' Clear any existing formula first
                        ApplyRobustFilter(rpt, {"quotation_billing", "quotation_billing1", "Command", "quotation_billing_items"}, {"inv_no"}, escapedInvoiceNo, False)
                    Case 6 ' Customer Credit
                        If IsNumeric(invoiceNo) Then
                            ApplyRobustFilter(rpt, {"customer", "customer1", "Command"}, {"id", "customer_id"}, escapedInvoiceNo, True)
                        Else
                            ApplyRobustFilter(rpt, {"customer", "customer1", "Command"}, {"name", "customer_name"}, escapedInvoiceNo, False)
                        End If
                    Case 7 ' Customer Cheque
                        ApplyRobustFilter(rpt, {"check_received", "check_received1", "Command"}, {"check_name", "name", "customer_name"}, escapedInvoiceNo, False)
                        ApplyRobustFilter(rpt, {"check_received", "check_received1", "Command"}, {"check_number", "chq_no", "number"}, escapedInvoiceNo, False)
                    Case 8 ' Supplier Cheque
                        ApplyRobustFilter(rpt, {"chaque_issue", "chaque_issue1", "Command"}, {"c_name", "name", "supplier_name"}, escapedInvoiceNo, False)
                        ApplyRobustFilter(rpt, {"chaque_issue", "chaque_issue1", "Command"}, {"chq_no", "number"}, escapedInvoiceNo, False)
                    Case 9 ' Supplier Debit
                        ApplyRobustFilter(rpt, {"supplicer_credit", "supplicer_credit1", "Command"}, {"sname", "name", "supplier_name"}, escapedInvoiceNo, False)
                        ApplyRobustFilter(rpt, {"supplicer_credit", "supplicer_credit1", "Command"}, {"inv_no"}, escapedInvoiceNo, False)
                    Case 10 ' Receiving Stock
                        rpt.RecordSelectionFormula = "{receive_stock1.transfer_id} = '" & escapedInvoiceNo & "'"
                    Case 11 ' Sending Stock
                        rpt.RecordSelectionFormula = "{sending_stock1.transfer_id} = '" & escapedInvoiceNo & "'"
                    Case 12 ' Barcode Labels
                        If Not String.IsNullOrEmpty(selectionFormula) Then
                            rpt.RecordSelectionFormula = selectionFormula
                        Else
                            ' Default filter
                            rpt.RecordSelectionFormula = "{items1.id} = '" & escapedInvoiceNo & "'"
                        End If
                        
                        ' (Note: Encoding is handled via Parameter p_Barcode below)
                    Case 13 ' Purchase Request
                        Dim formula As String = ""
                        Dim hasRequestTable As Boolean = False
                        Dim hasRequestItemsTable As Boolean = False
                        
                        For Each tbl As Table In rpt.Database.Tables
                            Dim tName As String = tbl.Name.ToLower()
                            If tName = "purchase_request" OrElse tName = "purchase_request1" Then
                                hasRequestTable = True
                            End If
                            If tName = "purchase_request_items" OrElse tName = "purchase_request_items1" Then
                                hasRequestItemsTable = True
                            End If
                        Next
                        
                        If hasRequestTable Then
                            Dim tblName As String = "purchase_request"
                            For Each tbl As Table In rpt.Database.Tables
                                If tbl.Name.Equals("purchase_request1", StringComparison.OrdinalIgnoreCase) Then
                                    tblName = "purchase_request1"
                                    Exit For
                                End If
                            Next
                            formula = "{" & tblName & ".request_id} = '" & escapedInvoiceNo & "'"
                        End If
                        
                        If hasRequestItemsTable Then
                            Dim tblItemsName As String = "purchase_request_items"
                            For Each tbl As Table In rpt.Database.Tables
                                If tbl.Name.Equals("purchase_request_items1", StringComparison.OrdinalIgnoreCase) Then
                                    tblItemsName = "purchase_request_items1"
                                    Exit For
                                End If
                            Next
                            If formula <> "" Then formula &= " AND "
                            formula &= "{" & tblItemsName & ".request_id} = '" & escapedInvoiceNo & "'"
                        End If
                        
                        If formula <> "" Then
                            rpt.RecordSelectionFormula = formula
                        Else
                            ApplyRobustFilter(rpt, {"purchase_request", "purchase_request1", "Command"}, {"request_id", "pur_id", "inv_no"}, escapedInvoiceNo, False)
                        End If
                    Case Else
                        ' Generic fallback
                        If invoiceNo.StartsWith("QT") Then
                            rpt.RecordSelectionFormula = "{quotation_billing1.inv_no} = '" & escapedInvoiceNo & "'"
                        Else
                            ' Try billing1.inv_no as a default
                            Try
                                rpt.RecordSelectionFormula = "{billing1.inv_no} = '" & escapedInvoiceNo & "'"
                            Catch
                            End Try
                        End If
                End Select
            Else
                rpt.RecordSelectionFormula = ""
            End If

            ' --- Fetch customer full credit (outstanding balance) for POS/Standard invoices ---
            Dim fullCreditVal As Decimal = 0
            Dim isSpecialCus As Boolean = False
            If (reportTypeIndex = 0 OrElse reportTypeIndex = 1) AndAlso Not String.IsNullOrEmpty(invoiceNo) Then
                isSpecialCus = IsSpecialCustomer(invoiceNo)
                If Not isSpecialCus Then
                    fullCreditVal = GetCustomerFullCredit(invoiceNo)
                End If
            End If

            ' --- Handle Parameters ---
            ' We use a robust method to satisfy ALL parameters in the report to prevent prompts
            SetAllReportParameters(rpt, invoiceNo, isReturn, "", 0, fullCreditVal)

            Try
                If (reportTypeIndex = 10 OrElse reportTypeIndex = 11) AndAlso Not String.IsNullOrEmpty(invoiceNo) Then
                    SetValuesOnDocument(rpt, invoiceNo, isReturn)
                End If
                
                ' Pass User Name to Barcode Label Report
                If reportTypeIndex = 12 Then
                    SetValuesOnDocument(rpt, invoiceNo, isReturn)
                End If
            Catch : End Try

            ' --- Dynamic Full Credit Layout Adjustment (Section1) ---
            ' Suppress label+field, shift objects below upward, and collapse section when credit is zero.
            Try
                If reportTypeIndex = 0 OrElse reportTypeIndex = 1 Then
                    Dim suppressCredit As Boolean = (fullCreditVal <= 0) OrElse isSpecialCus

                    ' For Standard Invoice (reportTypeIndex = 1), we ONLY suppress the credit label/value.
                    ' We do NOT shift other objects or resize Section1 to avoid layout misalignment and overlaps.
                    If reportTypeIndex = 1 Then
                        Dim rcdPropSec1 = rpt.GetType().GetProperty("ReportClientDocument")
                        If rcdPropSec1 IsNot Nothing Then
                            Dim rcd As Object = rcdPropSec1.GetValue(rpt, Nothing)
                            If rcd IsNot Nothing Then
                                Dim objs As Object = rcd.ReportDefController.ReportObjectController.GetAllReportObjects()
                                For Each oldObj As Object In objs
                                    Dim objName As String = oldObj.Name.ToString()
                                    If String.Equals(objName, "Text26", StringComparison.OrdinalIgnoreCase) OrElse _
                                       String.Equals(objName, "fullcredit1", StringComparison.OrdinalIgnoreCase) Then
                                        Dim newObj As Object = oldObj.Clone(True)
                                        newObj.Format.ConditionFormulas.RemoveAll()
                                        newObj.Format.EnableSuppress = suppressCredit
                                        If suppressCredit Then
                                            newObj.Height = 0
                                        Else
                                            newObj.Height = 221
                                        End If
                                        rcd.ReportDefController.ReportObjectController.Modify(oldObj, newObj)
                                    End If
                                Next
                            End If
                        End If
                        Exit Try
                    End If

                    ' Section1 heights: POS=3071/2831, Standard=2040/1800
                    Dim originalSec1Height As Integer = If(reportTypeIndex = 0, 3071, 2040)
                    Dim suppressedSec1Height As Integer = If(reportTypeIndex = 0, 2831, 1800)
                    Dim targetSec1Height As Integer = If(suppressCredit, suppressedSec1Height, originalSec1Height)

                    ' Build shift map: objectName -> {originalTop, suppressedTop}
                    ' POS Section1: only Text2 is below the credit row (2640)
                    ' Standard Section1: many objects below the credit row (1080)
                    Dim shiftMap As New Dictionary(Of String, Integer())(StringComparer.OrdinalIgnoreCase)
                    If reportTypeIndex = 0 Then
                        ' POS
                        shiftMap.Add("Text2", New Integer() {2880, 2640})
                    Else
                        ' Standard
                        shiftMap.Add("billingtype1", New Integer() {1200, 960})
                        shiftMap.Add("Text24", New Integer() {1320, 1080})
                        shiftMap.Add("Text25", New Integer() {1320, 1080})
                        shiftMap.Add("printedinvno1", New Integer() {1320, 1080})
                        shiftMap.Add("timestamps1", New Integer() {1320, 1080})
                        shiftMap.Add("Text22", New Integer() {1560, 1320})
                        shiftMap.Add("Text14", New Integer() {1560, 1320})
                        shiftMap.Add("ponumber1", New Integer() {1560, 1320})
                        shiftMap.Add("invtype1", New Integer() {1560, 1320})
                        shiftMap.Add("Text13", New Integer() {1800, 1560})
                        shiftMap.Add("Text9", New Integer() {1800, 1560})
                        shiftMap.Add("name2", New Integer() {1800, 1560})
                        shiftMap.Add("Text16", New Integer() {1800, 1560})
                        shiftMap.Add("Text18", New Integer() {1800, 1560})
                        shiftMap.Add("chequeno1", New Integer() {1800, 1560})
                    End If

                    Dim rcdProp = rpt.GetType().GetProperty("ReportClientDocument")
                    If rcdProp IsNot Nothing Then
                        Dim rcd As Object = rcdProp.GetValue(rpt, Nothing)
                        If rcd IsNot Nothing Then
                            Dim objs As Object = rcd.ReportDefController.ReportObjectController.GetAllReportObjects()
                            For Each oldObj As Object In objs
                                Dim objName As String = oldObj.Name.ToString()

                                ' 1. Suppress/restore credit label and value
                                If String.Equals(objName, "Text26", StringComparison.OrdinalIgnoreCase) OrElse _
                                   String.Equals(objName, "fullcredit1", StringComparison.OrdinalIgnoreCase) Then
                                    Dim newObj As Object = oldObj.Clone(True)
                                    newObj.Format.ConditionFormulas.RemoveAll()
                                    newObj.Format.EnableSuppress = suppressCredit
                                    If suppressCredit Then
                                        newObj.Height = 0
                                    Else
                                        If String.Equals(objName, "Text26", StringComparison.OrdinalIgnoreCase) Then
                                            newObj.Height = 240
                                        Else
                                            newObj.Height = If(reportTypeIndex = 0, 227, 221)
                                        End If
                                    End If
                                    rcd.ReportDefController.ReportObjectController.Modify(oldObj, newObj)

                                ' 2. Shift objects below credit row up/down
                                ElseIf shiftMap.ContainsKey(objName) Then
                                    Dim positions As Integer() = shiftMap(objName)
                                    Dim targetTop As Integer = If(suppressCredit, positions(1), positions(0))
                                    Dim newObj As Object = oldObj.Clone(True)
                                    newObj.Top = targetTop
                                    rcd.ReportDefController.ReportObjectController.Modify(oldObj, newObj)
                                End If
                            Next

                            ' 3. Resize Section1 to collapse/restore the gap
                            Dim areas As Object = rcd.ReportDefController.ReportDefinition.Areas
                            Dim targetSection As Object = Nothing
                            For i As Integer = 0 To areas.Count - 1
                                Dim area As Object = areas(i)
                                For j As Integer = 0 To area.Sections.Count - 1
                                    Dim section As Object = area.Sections(j)
                                    If String.Equals(section.Name.ToString(), "Section1", StringComparison.OrdinalIgnoreCase) Then
                                        targetSection = section
                                        Exit For
                                    End If
                                Next
                                If targetSection IsNot Nothing Then Exit For
                            Next
                            If targetSection IsNot Nothing Then
                                rcd.ReportDefController.ReportSectionController.SetProperty(targetSection, 2, targetSec1Height)
                            End If
                        End If
                    End If
                End If
            Catch exCredit As Exception
                ' Silent — layout adjustment is best-effort
            End Try

            ' Display the report
            CrystalReportViewer1.ReuseParameterValuesOnRefresh = True
            
            ' Tag is no longer used for copy count.
            ' Copy count is always 1 at report level to avoid duplication.
            CrystalReportViewer1.Tag = 1
            
            ' Prepare Parameter Fields for the Viewer (Bypass Prompts)
            Dim pFields As New ParameterFields()
            
            ' Only add isReturn / full_credit parameters to Viewer for POS Invoice or Standard Invoice
            If reportTypeIndex = 0 OrElse reportTypeIndex = 1 Then
                Dim pField As New ParameterField()
                pField.Name = "isReturn"
                Dim pDiscreteValue As New ParameterDiscreteValue()
                pDiscreteValue.Value = isReturn
                pField.CurrentValues.Add(pDiscreteValue)
                pFields.Add(pField)

                ' full_credit viewer param to prevent prompt
                Dim pFieldCredit As New ParameterField()
                pFieldCredit.Name = "full_credit"
                Dim pDvCredit As New ParameterDiscreteValue()
                pDvCredit.Value = fullCreditVal.ToString("N2")
                pFieldCredit.CurrentValues.Add(pDvCredit)
                pFields.Add(pFieldCredit)
            End If
            
            ' Assign to Viewer BEFORE setting ReportSource
            CrystalReportViewer1.ParameterFieldInfo = pFields
            If PrintAsRetail Then
                SuppressPaymentAndChangeFields(rpt)
                Try
                    For Each subRpt As ReportDocument In rpt.Subreports
                        SuppressPaymentAndChangeFields(subRpt)
                    Next
                Catch
                End Try
            End If
            CrystalReportViewer1.ReportSource = rpt
            
            ' Force parameters on the document level too
            SetAllReportParameters(rpt, invoiceNo, isReturn, "", 0, fullCreditVal)

            CrystalReportViewer1.RefreshReport()
            
            ' Restore database to wholesale immediately after report has loaded/queried the data
            If PrintAsRetail Then
                RestoreOriginalWholesalePrices()
            End If
            
            If Not silentMode Then
                Me.Show()
                Me.BringToFront()
            End If

        Catch ex As Exception
            MessageBox.Show("Error loading report: " & ex.Message & vbCrLf & vbCrLf & "Check if Crystal Reports Runtime is installed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Attempts to find and apply a record selection formula by discovering table and field names dynamically.
    ''' This handles cases where the report might use 'Command' or other table aliases.
    ''' </summary>
    Private Sub ApplyRobustFilter(ByRef rpt As ReportDocument, ByVal possibleTables As String(), ByVal possibleFields As String(), ByVal value As String, ByVal p_isNumeric As Boolean)
        Try
            Dim tableName As String = ""
            Dim fieldName As String = ""
            Dim foundTable As Boolean = False

            ' 1. Identify Table (Priority: Exact Match)
            For Each tbl As Table In rpt.Database.Tables
                For Each targetTable As String In possibleTables
                    If tbl.Name.Equals(targetTable, StringComparison.OrdinalIgnoreCase) Then
                        tableName = tbl.Name
                        foundTable = True
                        Exit For
                    End If
                Next
                If foundTable Then Exit For
            Next

            ' 2. Fallback table search (Partial Match)
            If Not foundTable Then
                For Each tbl As Table In rpt.Database.Tables
                    For Each targetTable As String In possibleTables
                        If tbl.Name.ToLower().Contains(targetTable.ToLower()) Then
                            tableName = tbl.Name
                            foundTable = True
                            Exit For
                        End If
                    Next
                    If foundTable Then Exit For
                Next
            End If

            ' If still not found, use first table if it exists
            If Not foundTable AndAlso rpt.Database.Tables.Count > 0 Then
                tableName = rpt.Database.Tables(0).Name
            End If

            ' 3. Identify Field in the found table
            If Not String.IsNullOrEmpty(tableName) Then
                Dim tblObj As Table = rpt.Database.Tables(tableName)
                Dim foundField As Boolean = False
                
                ' Priority: Exact Match
                For Each field As DatabaseFieldDefinition In tblObj.Fields
                    For Each targetField As String In possibleFields
                        If field.Name.Equals(targetField, StringComparison.OrdinalIgnoreCase) Then
                            fieldName = field.Name
                            foundField = True
                            Exit For
                        End If
                    Next
                    If foundField Then Exit For
                Next

                ' Fallback Field search (Partial Match)
                If Not foundField Then
                    For Each field As DatabaseFieldDefinition In tblObj.Fields
                        For Each targetField As String In possibleFields
                            If field.Name.ToLower().Contains(targetField.ToLower()) Then
                                fieldName = field.Name
                                foundField = True
                                Exit For
                            End If
                        Next
                        If foundField Then Exit For
                    Next
                End If

                ' 4. Build and Apply Formula across ALL matching fields
                Dim combinedFormula As String = ""
                For Each targetField As String In possibleFields
                    fieldName = ""
                    ' Priority: Exact Match
                    For Each field As DatabaseFieldDefinition In tblObj.Fields
                        If field.Name.Equals(targetField, StringComparison.OrdinalIgnoreCase) Then
                            fieldName = field.Name
                            Exit For
                        End If
                    Next

                    ' Fallback: Partial Match
                    If String.IsNullOrEmpty(fieldName) Then
                        For Each field As DatabaseFieldDefinition In tblObj.Fields
                            If field.Name.ToLower().Contains(targetField.ToLower()) Then
                                fieldName = field.Name
                                Exit For
                            End If
                        Next
                    End If

                    If Not String.IsNullOrEmpty(fieldName) Then
                        Dim currentFormula As String = ""
                        If p_isNumeric AndAlso IsNumeric(value) Then
                            currentFormula = "{" & tableName & "." & fieldName & "} = " & value
                        Else
                            currentFormula = "UpperCase({" & tableName & "." & fieldName & "}) LIKE '*" & value.ToUpper().Replace("'", "''") & "*'"
                        End If

                        If String.IsNullOrEmpty(combinedFormula) Then
                            combinedFormula = currentFormula
                        Else
                            combinedFormula &= " OR " & currentFormula
                        End If
                    End If
                Next

                ' Apply to report
                If Not String.IsNullOrEmpty(combinedFormula) Then
                    If String.IsNullOrEmpty(rpt.RecordSelectionFormula) Then
                        rpt.RecordSelectionFormula = combinedFormula
                    Else
                        ' Use AND to combine with existing filters (like is_rgr=0), but wrap the OR block in parentheses
                        rpt.RecordSelectionFormula &= " AND (" & combinedFormula & ")"
                    End If
                End If
            End If
        Catch ex As Exception
            ' Silent error
        End Try
    End Sub

    Public Sub DirectPrint(Optional ByVal printerName As String = "")
        Try
            Dim rpt As ReportDocument = DirectCast(CrystalReportViewer1.ReportSource, ReportDocument)
            If rpt IsNot Nothing Then
                If Not String.IsNullOrEmpty(printerName) Then
                    rpt.PrintOptions.PrinterName = printerName
                End If
                ' Print directly without standard dialog
                rpt.PrintToPrinter(1, False, 0, 0)
            Else
                MessageBox.Show("No report loaded to print.", "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
        Catch ex As Exception
            MessageBox.Show("Printing Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            ' If form is not visible (silent/automated print), restore database immediately after printing
            If Not Me.Visible Then
                RestoreOriginalWholesalePrices()
            End If
        End Try
    End Sub

    Private Sub SaleInv_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        RestoreOriginalWholesalePrices()
    End Sub

    Public Sub SetParameterValue(ByVal name As String, ByVal value As Object)
        Try
            Dim rpt As ReportDocument = DirectCast(CrystalReportViewer1.ReportSource, ReportDocument)
            If rpt IsNot Nothing Then
                SetValuesOnDocument(rpt, "", False) ' This will now update based on name in loop
            End If
        Catch ex As Exception
            ' Silent fail or log
        End Try
    End Sub

    ''' <summary>
    ''' Exhaustively sets all parameters in the report and its subreports.
    ''' This tries to satisfy isReturn, Invoice Number, and any other common parameters.
    ''' </summary>
    Private Sub SetAllReportParameters(ByRef rpt As ReportDocument, ByVal invoiceNo As String, ByVal isReturn As Boolean, Optional ByVal balanceAction As String = "", Optional ByVal balanceAmt As Decimal = 0, Optional ByVal fullCredit As Decimal = 0)
        Try
            ' 1. Set on Main Report
            SetValuesOnDocument(rpt, invoiceNo, isReturn, balanceAction, balanceAmt, fullCredit)

            ' 2. Set on Subreports
            For Each subRpt As ReportDocument In rpt.Subreports
                SetValuesOnDocument(subRpt, invoiceNo, isReturn, balanceAction, balanceAmt, fullCredit)
            Next
        Catch ex As Exception
            ' Silent fail
        End Try
    End Sub

    Private Sub SetValuesOnDocument(ByRef doc As ReportDocument, ByVal invNo As String, ByVal isRet As Boolean, Optional ByVal balAction As String = "", Optional ByVal balAmt As Decimal = 0, Optional ByVal fullCredit As Decimal = 0)
        Try
            ' 1. Exhaustive attempts to set "isReturn"
            Dim hasIsReturn As Boolean = False
            Try
                For Each pd As ParameterFieldDefinition In doc.DataDefinition.ParameterFields
                    If pd.Name.Equals("isReturn", StringComparison.OrdinalIgnoreCase) Then
                        hasIsReturn = True
                        Exit For
                    End If
                Next
            Catch : End Try

            If hasIsReturn Then
                Try
                    ' Method A: Direct set as Boolean
                    doc.SetParameterValue("isReturn", isRet)
                    ' Method B: Direct set as String (backup)
                    doc.SetParameterValue("isReturn", If(isRet, "True", "False"))
                    ' Method C: Direct set as Integer (backup)
                    doc.SetParameterValue("isReturn", If(isRet, 1, 0))
                Catch : End Try
            End If

            ' 2. Set Balance Action Parameters if they exist
            Try
                If Not String.IsNullOrEmpty(balAction) Then
                    doc.SetParameterValue("p_BalanceAction", balAction)
                    doc.SetParameterValue("p_BalanceAmt", balAmt)
                End If
            Catch : End Try

            ' 3. Set full_credit parameter (outstanding balance for the invoice customer)
            Try
                Dim hasFullCredit As Boolean = False
                For Each pd As ParameterFieldDefinition In doc.DataDefinition.ParameterFields
                    If pd.Name.Equals("full_credit", StringComparison.OrdinalIgnoreCase) Then
                        hasFullCredit = True
                        Exit For
                    End If
                Next
                If hasFullCredit Then
                    Dim creditStr As String = fullCredit.ToString("N2")
                    doc.SetParameterValue("full_credit", creditStr)
                    ' Also apply via ParameterDiscreteValue for robustness
                    For Each pd As ParameterFieldDefinition In doc.DataDefinition.ParameterFields
                        If pd.Name.Equals("full_credit", StringComparison.OrdinalIgnoreCase) Then
                            Dim vals As New ParameterValues()
                            Dim dv As New ParameterDiscreteValue()
                            dv.Value = creditStr
                            vals.Add(dv)
                            pd.ApplyCurrentValues(vals)
                            Exit For
                        End If
                    Next
                End If
            Catch : End Try

            ' Method D: Using ParameterDiscreteValue for isReturn (The most robust low-level method)
            Try
                For Each pd As ParameterFieldDefinition In doc.DataDefinition.ParameterFields
                    If pd.Name.Equals("isReturn", StringComparison.OrdinalIgnoreCase) Then
                        Dim vals As New ParameterValues()
                        Dim dv As New ParameterDiscreteValue()
                        dv.Value = isRet
                        vals.Add(dv)
                        pd.ApplyCurrentValues(vals)
                        Exit For
                    End If
                Next
            Catch : End Try

            ' 4. Handle other common parameters (Invoice No, etc.)
            For Each pd As ParameterFieldDefinition In doc.DataDefinition.ParameterFields
                Dim pName As String = pd.Name.ToLower()
                If pName.Contains("inv") OrElse pName.Contains("bill") OrElse pName.Contains("id") Then
                    Try
                        If Not String.IsNullOrEmpty(invNo) Then
                            doc.SetParameterValue(pd.Name, invNo)
                        End If
                    Catch : End Try
                End If
            Next
        Catch : End Try
    End Sub

    ''' <summary>
    ''' Applies current parameter values from the ReportDocument to the CrystalReportViewer.
    ''' This is a backup measure for reports that are stubborn about parameters.
    ''' </summary>
    Private Sub ApplyParametersToViewer(ByVal rpt As ReportDocument)
        Try
            ' Clear any existing parameter values in the viewer to prevent "Missing ParameterField"
            ' CrystalReportViewer1.ParameterFieldInfo.Clear() ' This can sometimes cause issues, so we just update

            For Each pd As ParameterFieldDefinition In rpt.DataDefinition.ParameterFields
                Try
                    ' Get current value from report doc and sync to viewer
                    Dim currentVal = rpt.ParameterFields(pd.Name).CurrentValues
                    If currentVal.Count > 0 Then
                        ' Ensure the field exists in viewer before accessing
                        Dim viewerField = CrystalReportViewer1.ParameterFieldInfo(pd.Name)
                        If viewerField IsNot Nothing Then
                            viewerField.CurrentValues.Clear()
                            viewerField.CurrentValues.Add(currentVal(0))
                        End If
                    End If
                Catch : End Try
            Next
        Catch : End Try
    End Sub

    Private Sub SetReportConnection(ByRef rpt As ReportDocument)
        Try
            ' Parse connection string (Format: server=...;userid=...;password=...;database=...)
            Dim connStr = Module1.ConnStr
            Dim parts = connStr.Split(";"c)
            Dim server = "", db = "", user = "", pass = ""

            For Each part In parts
                Dim kv = part.Split("="c)
                If kv.Length >= 2 Then
                    Dim key = kv(0).Trim().ToLower()
                    ' Rejoin value in case password contains '='
                    Dim val = String.Join("=", kv, 1, kv.Length - 1).Trim().Replace(vbCr, "").Replace(vbLf, "")
                    
                    If key = "server" OrElse key = "data source" OrElse key = "host" Then server = val
                    If key = "database" OrElse key = "initial catalog" Then db = val
                    If key = "userid" OrElse key = "user id" OrElse key = "uid" Then user = val
                    If key = "password" OrElse key = "pwd" Then pass = val
                End If
            Next

            ' Apply to main report tables
            For Each tbl As Table In rpt.Database.Tables
                Dim tblLogOn = tbl.LogOnInfo
                ' We DO NOT overwrite ServerName or DatabaseName to preserve ODBC DSNs
                tblLogOn.ConnectionInfo.UserID = user
                tblLogOn.ConnectionInfo.Password = pass
                tblLogOn.ConnectionInfo.IntegratedSecurity = False
                tbl.ApplyLogOnInfo(tblLogOn)
            Next

            ' Apply to subreports
            For Each subRpt As ReportDocument In rpt.Subreports
                For Each tbl As Table In subRpt.Database.Tables
                    Dim tblLogOn = tbl.LogOnInfo
                    tblLogOn.ConnectionInfo.UserID = user
                    tblLogOn.ConnectionInfo.Password = pass
                    tblLogOn.ConnectionInfo.IntegratedSecurity = False
                    tbl.ApplyLogOnInfo(tblLogOn)
                Next
            Next

            ' Force top-level logon as additional measure
            ' We only apply the logon using the report's existing ServerName and DatabaseName
            rpt.SetDatabaseLogon(user, pass, rpt.DataSourceConnections(0).ServerName, rpt.DataSourceConnections(0).DatabaseName)

            ' Format report to 3 decimal places
            Module1.FormatReportDecimals(rpt)

        Catch ex As Exception
            ' Silent fail, fallback to stored connection
        End Try
    End Sub

    Private Function IsSpecialCustomer(ByVal invoiceNo As String) As Boolean
        Try
            Dim queryTable As String = "billing"
            If invoiceNo.StartsWith("QT") Then
                queryTable = "quotation_billing"
            End If

            Using conn As New MySql.Data.MySqlClient.MySqlConnection(Module1.ConnStr)
                conn.Open()
                Dim customerId As String = ""
                Dim qCus As String = "SELECT customer_id FROM " & queryTable & " WHERE inv_no = @inv"
                If queryTable = "billing" Then
                    qCus = "SELECT customer_id FROM billing WHERE inv_no = @inv OR printed_inv_no = @inv"
                End If
                Using cmdCus As New MySql.Data.MySqlClient.MySqlCommand(qCus, conn)
                    cmdCus.Parameters.AddWithValue("@inv", invoiceNo)
                    Dim resCus = cmdCus.ExecuteScalar()
                    If resCus IsNot Nothing AndAlso resCus IsNot DBNull.Value Then
                        customerId = resCus.ToString()
                    End If
                End Using

                If Not String.IsNullOrEmpty(customerId) Then
                    Dim qName As String = "SELECT name FROM customer WHERE id = @c_id"
                    Using cmdName As New MySql.Data.MySqlClient.MySqlCommand(qName, conn)
                        cmdName.Parameters.AddWithValue("@c_id", customerId)
                        Dim resName = cmdName.ExecuteScalar()
                        If resName IsNot Nothing AndAlso resName IsNot DBNull.Value Then
                            Dim cusName As String = resName.ToString().Trim()
                            If cusName.ToLower().Contains("thushara thudawa") Then
                                Return True
                            End If
                        End If
                    End Using
                End If
            End Using
        Catch ex As Exception
            ' Silent fail
        End Try
        Return False
    End Function

    Private Function GetCustomerFullCredit(ByVal invoiceNo As String) As Decimal
        Dim fullCredit As Decimal = 0
        Try
            Dim customerId As String = ""
            Dim queryTable As String = "billing"
            If invoiceNo.StartsWith("QT") Then
                queryTable = "quotation_billing"
            End If

            Using conn As New MySql.Data.MySqlClient.MySqlConnection(Module1.ConnStr)
                conn.Open()
                
                ' 1. Get customer ID for the given invoice
                Dim qCus As String = "SELECT customer_id FROM " & queryTable & " WHERE inv_no = @inv"
                If queryTable = "billing" Then
                    qCus = "SELECT customer_id FROM billing WHERE inv_no = @inv OR printed_inv_no = @inv"
                End If
                Using cmdCus As New MySql.Data.MySqlClient.MySqlCommand(qCus, conn)
                    cmdCus.Parameters.AddWithValue("@inv", invoiceNo)
                    Dim resCus = cmdCus.ExecuteScalar()
                    If resCus IsNot Nothing AndAlso resCus IsNot DBNull.Value Then
                        customerId = resCus.ToString()
                    End If
                End Using

                ' 2. If we have a customer ID, sum their balance_due
                If Not String.IsNullOrEmpty(customerId) AndAlso customerId <> "1" Then
                    Dim qCredit As String = "SELECT SUM(balance_due) FROM billing WHERE customer_id = @c_id"
                    If Not Module1.IsRgrVisible Then
                        qCredit &= " AND is_rgr = 0"
                    End If
                    Using cmdCredit As New MySql.Data.MySqlClient.MySqlCommand(qCredit, conn)
                        cmdCredit.Parameters.AddWithValue("@c_id", customerId)
                        Dim resCredit = cmdCredit.ExecuteScalar()
                        If resCredit IsNot Nothing AndAlso resCredit IsNot DBNull.Value Then
                            Decimal.TryParse(resCredit.ToString(), fullCredit)
                        End If
                    End Using
                End If
            End Using
        Catch ex As Exception
            ' Silent fail
        End Try
        Return fullCredit
    End Function

    Private Sub SaleInv_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            btnShowInvoice.PerformClick()
        End If
    End Sub
End Class