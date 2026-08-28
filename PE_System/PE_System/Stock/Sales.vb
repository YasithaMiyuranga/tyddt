Imports MySql.Data.MySqlClient
Imports System.Drawing.Printing

Public Class frmSales
    Private isSearching As Boolean = False

    Private Sub dailySale_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Setup Inv Type ComboBoxes
        Dim invTypes As String() = {"Normal", "Wholesale", "Retail"}
        Dim billingTypes As String()
        If Not Module1.IsRgrVisible Then
            billingTypes = {"Cash", "Cash (Cash)", "Cash (Cards/Online)"}
        Else
            billingTypes = {"Cash", "Cash (Cash)", "Cash (Cards/Online)", "Credit", "Cheque", "Cash+Credit", "Cash+Cheque", "Mixed Payment", "Credit+Cheque"}
        End If

        ' Clear all ComboBoxes as a precaution
        ComboBox1.Items.Clear() ' Select Printer (Daily)
        ComboBox2.Items.Clear() ' Select Printer (Monthly)
        ComboBox5.Items.Clear() ' Select Printer (Monthly Item)

        ' --- POPULATE PRINTER LIST ---
        Try
            For Each printer As String In PrinterSettings.InstalledPrinters
                ComboBox1.Items.Add(printer)
                ComboBox2.Items.Add(printer)
                ComboBox5.Items.Add(printer)
            Next

            ' Default to default printer if available
            Dim defaultPrinter As String = New PrinterSettings().PrinterName
            If ComboBox1.Items.Contains(defaultPrinter) Then
                ComboBox1.SelectedItem = defaultPrinter
                ComboBox2.SelectedItem = defaultPrinter
                ComboBox5.SelectedItem = defaultPrinter
            ElseIf ComboBox1.Items.Count > 0 Then
                ComboBox1.SelectedIndex = 0
                ComboBox2.SelectedIndex = 0
                ComboBox5.SelectedIndex = 0
            End If
        Catch ex As Exception
            ' Silent fail for printer listing
        End Try

        ' Populate Inv Type ComboBoxes
        ComboBox4.Items.Clear()
        ComboBox4.Items.AddRange(invTypes)
        ComboBox3.Items.Clear()
        ComboBox3.Items.AddRange(invTypes)
        ComboBoxItemInvType.Items.Clear()
        ComboBoxItemInvType.Items.AddRange(invTypes)

        ' Populate Billing Type ComboBoxes
        ComboBoxBilling.Items.Clear()
        ComboBoxBilling.Items.AddRange(billingTypes)
        ComboBoxBillingTypeM.Items.Clear()
        ComboBoxBillingTypeM.Items.AddRange(billingTypes)
        ComboBoxBillingMI.Items.Clear()
        ComboBoxBillingMI.Items.AddRange(billingTypes)

        ' Default values
        ComboBox4.SelectedIndex = 0 ' Normal
        ComboBox3.SelectedIndex = 0 ' Normal
        ComboBoxItemInvType.SelectedIndex = 0 ' Normal

        ComboBoxBilling.SelectedIndex = 0 ' Cash
        ComboBoxBillingTypeM.SelectedIndex = 0 ' Cash
        ComboBoxBillingMI.SelectedIndex = 0 ' Cash

        ' Default dates to today
        DateTimePicker1.Value = DateTime.Now
        dtpCashFlow.Value = DateTime.Now

        DateTimePicker4.Value = DateTime.Now.AddMonths(-1) ' Default monthly range start
        DateTimePicker5.Value = DateTime.Now
        DateTimePickerItemStart.Value = DateTime.Now.AddMonths(-1)
        DateTimePickerItemEnd.Value = DateTime.Now

        ' Setup Grids
        SetupGridStyles(DataGridView1)
        SetupGridStyles(DataGridView2)
        SetupGridStyles(DataGridView3)
        SetupGridStyles(dgvCashFlow)

        ' Initialize Context Menu for Editing
        InitializeEditContextMenu()

        ' Apply security lock on load to set correct tabs/permissions and load active data
        Try
            ApplySecurityLock()
        Catch ex As Exception
            MessageBox.Show("Error during Load: " & ex.Message)
        End Try
    End Sub

    Private Sub TabControl1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControl1.SelectedIndexChanged
        RefreshCurrentTab()
    End Sub

    Private Sub frmSales_Activated(sender As Object, e As EventArgs) Handles MyBase.Activated
        RefreshCurrentTab()
    End Sub

    Private Sub RefreshCurrentTab()
        Try
            If TabControl1.SelectedTab Is TabPage1 Then
                LoadDailySales()
            ElseIf TabControl1.SelectedTab Is TabPage2 Then
                LoadMonthlySales()
            ElseIf TabControl1.SelectedTab Is TabPage3 Then
                LoadMonthlyItemSales()
            ElseIf TabControl1.SelectedTab Is TabPage4 Then
                LoadCashFlow()
            End If
        Catch ex As Exception
            ' Silent fail during background refresh
        End Try
    End Sub

    ' --- EDIT CONTEXT MENU ---
    Private editMenu As ContextMenuStrip

    Private Sub InitializeEditContextMenu()
        editMenu = New ContextMenuStrip()
        Dim editItem As New ToolStripMenuItem("Edit Invoice")
        AddHandler editItem.Click, AddressOf EditInvoice_Click
        editMenu.Items.Add(editItem)

        ' Attach to all sales grids
        DataGridView1.ContextMenuStrip = editMenu
        DataGridView2.ContextMenuStrip = editMenu

        ' Custom Context Menu for DataGridView3 (Monthly Item Sales)
        Dim itemMenu As New ContextMenuStrip()
        Dim viewInvoicesItem As New ToolStripMenuItem("View Invoices for Item")
        AddHandler viewInvoicesItem.Click, AddressOf ViewInvoicesItem_Click
        itemMenu.Items.Add(viewInvoicesItem)
        DataGridView3.ContextMenuStrip = itemMenu
    End Sub

    Private Sub EditInvoice_Click(sender As Object, e As EventArgs)
        ' Get the appropriate grid based on which one was clicked
        Dim dgv As DataGridView = Nothing
        If TabControl1.SelectedTab Is TabPage1 Then dgv = DataGridView1
        If TabControl1.SelectedTab Is TabPage2 Then dgv = DataGridView2
        If TabControl1.SelectedTab Is TabPage3 Then dgv = DataGridView3

        If dgv IsNot Nothing AndAlso dgv.CurrentRow IsNot Nothing Then
            Try
                Dim billingId As String = ""
                Dim invNo As String = ""

                ' Attempt to get 'id' and 'inv_no' / 'Inv No' / 'inv_no'
                If dgv.Columns.Contains("id") Then billingId = dgv.CurrentRow.Cells("id").Value.ToString()

                If dgv.Columns.Contains("inv_no") Then
                    invNo = dgv.CurrentRow.Cells("inv_no").Value.ToString()
                ElseIf dgv.Columns.Contains("Inv No") Then
                    invNo = dgv.CurrentRow.Cells("Inv No").Value.ToString()
                ElseIf dgv.Columns.Contains("inv_no") Then
                    invNo = dgv.CurrentRow.Cells("inv_no").Value.ToString()
                End If

                If String.IsNullOrEmpty(invNo) OrElse String.IsNullOrEmpty(billingId) Then
                    MessageBox.Show("Cannot edit this selection. Invoice Number or ID not found.", "Edit Unavailable", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Return
                End If

                ' Confirm with user
                Dim ans = MessageBox.Show("Do you want to edit Invoice " & invNo & "?" & vbCrLf &
                                        "This will load the invoice into the Sales window for modification.",
                                        "Confirm Edit", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

                If ans = DialogResult.Yes Then
                    ' Call TempSales.LoadInvoiceForEditing
                    ' In VB.NET default instances, we can use the form name
                    TempSales.LoadInvoiceForEditing(billingId, invNo)

                    ' Optionally switch to TempSales form via MDI Parent (Start)
                    If Me.MdiParent IsNot Nothing Then
                        TempSales.MdiParent = Me.MdiParent
                        TempSales.Show()
                        TempSales.WindowState = FormWindowState.Maximized
                        TempSales.Focus()
                    End If
                End If

            Catch ex As Exception
                MessageBox.Show("Error initiating edit: " & ex.Message)
            End Try
        End If
    End Sub

    Private Sub SetupGridStyles(dgv As DataGridView)
        dgv.ReadOnly = True
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgv.AllowUserToAddRows = False
        dgv.DefaultCellStyle.Font = New Font("Microsoft Sans Serif", 14)
        dgv.ColumnHeadersDefaultCellStyle.Font = New Font("Microsoft Sans Serif", 16, FontStyle.Bold)
    End Sub

    ' --- EVENT HANDLERS FOR AUTO-UPDATE ---

    Private Sub ComboBox4_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox4.SelectedIndexChanged, ComboBoxBilling.SelectedIndexChanged
        If Not isSearching Then LoadDailySales()
    End Sub

    Private Sub DateTimePicker1_ValueChanged(sender As Object, e As EventArgs) Handles DateTimePicker1.ValueChanged
        LoadDailySales()
    End Sub

    Private Sub TextBoxItemFilters_TextChanged(sender As Object, e As EventArgs) Handles TextBox3.TextChanged, TextBox4.TextChanged
        If Not isSearching Then LoadMonthlyItemSales()
    End Sub

    Private Sub dtpCashFlow_ValueChanged(sender As Object, e As EventArgs) Handles dtpCashFlow.ValueChanged
        LoadCashFlow()
    End Sub

    Private Sub ComboBox3_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox3.SelectedIndexChanged, ComboBoxBillingTypeM.SelectedIndexChanged
        If Not isSearching Then LoadMonthlySales()
    End Sub

    Private Sub DateTimePicker4_ValueChanged(sender As Object, e As EventArgs) Handles DateTimePicker4.ValueChanged, DateTimePicker5.ValueChanged
        LoadMonthlySales()
    End Sub

    Private Sub ComboBoxItemInvType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxItemInvType.SelectedIndexChanged, ComboBoxBillingMI.SelectedIndexChanged
        If Not isSearching Then LoadMonthlyItemSales()
    End Sub

    Private Sub DateTimePickerItemStart_ValueChanged(sender As Object, e As EventArgs) Handles DateTimePickerItemStart.ValueChanged, DateTimePickerItemEnd.ValueChanged
        LoadMonthlyItemSales()
    End Sub

    ' --- ADVANCE PAYMENT CHECKBOX LOGIC ---

    Private Sub CheckBoxAdvDaily_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxAdvDaily.CheckedChanged
        If CheckBoxAdvDaily.Checked Then
            ComboBoxBilling.SelectedIndex = 0 ' Default to Cash
            ComboBoxBilling.Enabled = False
        Else
            ComboBoxBilling.Enabled = True
        End If
        If Not isSearching Then LoadDailySales()
    End Sub

    Private Sub CheckBoxAdvMonthly_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxAdvMonthly.CheckedChanged
        If CheckBoxAdvMonthly.Checked Then
            ComboBoxBillingTypeM.SelectedIndex = 0 ' Default to Cash
            ComboBoxBillingTypeM.Enabled = False
        Else
            ComboBoxBillingTypeM.Enabled = True
        End If
        If Not isSearching Then LoadMonthlySales()
    End Sub

    Private Sub CheckBoxAdvItem_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxAdvItem.CheckedChanged
        If CheckBoxAdvItem.Checked Then
            ComboBoxBillingMI.SelectedIndex = 0 ' Default to Cash
            ComboBoxBillingMI.Enabled = False
        Else
            ComboBoxBillingMI.Enabled = True
        End If
        If Not isSearching Then LoadMonthlyItemSales()
    End Sub

    ' --- ALL CHECKBOX LOGIC ---

    Private Sub CheckBoxAllDaily_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxAllDaily.CheckedChanged
        If CheckBoxAllDaily.Checked Then
            ComboBox4.Enabled = False
            ComboBoxBilling.Enabled = False
            CheckBoxAdvDaily.Enabled = False
        Else
            ComboBox4.Enabled = True
            ComboBoxBilling.Enabled = Not CheckBoxAdvDaily.Checked
            CheckBoxAdvDaily.Enabled = True
        End If
        If Not isSearching Then LoadDailySales()
    End Sub

    Private Sub CheckBoxAllMonthly_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxAllMonthly.CheckedChanged
        If CheckBoxAllMonthly.Checked Then
            ComboBox3.Enabled = False
            ComboBoxBillingTypeM.Enabled = False
            CheckBoxAdvMonthly.Enabled = False
        Else
            ComboBox3.Enabled = True
            ComboBoxBillingTypeM.Enabled = Not CheckBoxAdvMonthly.Checked
            CheckBoxAdvMonthly.Enabled = True
        End If
        If Not isSearching Then LoadMonthlySales()
    End Sub

    Private Sub CheckBoxAllItem_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxAllItem.CheckedChanged
        If CheckBoxAllItem.Checked Then
            ComboBoxItemInvType.Enabled = False
            ComboBoxBillingMI.Enabled = False
            CheckBoxAdvItem.Enabled = False
        Else
            ComboBoxItemInvType.Enabled = True
            ComboBoxBillingMI.Enabled = Not CheckBoxAdvItem.Checked
            CheckBoxAdvItem.Enabled = True
        End If
        If Not isSearching Then LoadMonthlyItemSales()
    End Sub

    ' --- INVOICE SEARCH HANDLERS ---
    Private Sub TextBoxInvNo_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBoxInvNo.KeyDown
        If e.KeyCode = Keys.Enter Then
            SearchInvoice(TextBoxInvNo.Text.Trim(), "Daily")
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub TextBoxInvNoM_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBoxInvNoM.KeyDown
        If e.KeyCode = Keys.Enter Then
            SearchInvoice(TextBoxInvNoM.Text.Trim(), "Monthly")
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub TextBoxInvNoMI_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBoxInvNoMI.KeyDown
        If e.KeyCode = Keys.Enter Then
            SearchInvoice(TextBoxInvNoMI.Text.Trim(), "Item")
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub SearchInvoice(invNo As String, mode As String)
        If String.IsNullOrEmpty(invNo) Then Return
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()
            Dim found As Boolean = False
            Dim foundTable As String = ""
            Dim billingTables As String() = {"billing", "quotation_billing"}

            For Each table In billingTables
                Dim sqlSearch As String = String.Format("SELECT inv_type, billing_type, advance_payment FROM {0} WHERE inv_no = @no", table)
                Dim cmd As New MySqlCommand(sqlSearch, conn)
                cmd.Parameters.AddWithValue("@no", invNo)
                Using dr As MySqlDataReader = cmd.ExecuteReader()
                    If dr.Read() Then
                        Dim iType As String = dr("inv_type").ToString()
                        Dim bType As String = dr("billing_type").ToString()
                        Dim adv As Decimal = 0
                        If Not IsDBNull(dr("advance_payment")) Then adv = Convert.ToDecimal(dr("advance_payment"))
                        Dim isAdv As Boolean = (adv > 0)

                        isSearching = True
                        Try
                            If mode = "Daily" Then
                                ComboBox4.SelectedItem = iType
                                CheckBoxAdvDaily.Checked = isAdv
                                ComboBoxBilling.SelectedItem = bType
                            ElseIf mode = "Monthly" Then
                                ComboBox3.SelectedItem = iType
                                CheckBoxAdvMonthly.Checked = isAdv
                                ComboBoxBillingTypeM.SelectedItem = bType
                            Else
                                ComboBoxItemInvType.SelectedItem = iType
                                CheckBoxAdvItem.Checked = isAdv
                                ComboBoxBillingMI.SelectedItem = bType
                            End If
                        Finally
                            isSearching = False
                        End Try
                        found = True
                        foundTable = table
                    End If
                End Using
                If found Then Exit For
            Next

            If found Then
                If mode = "Daily" Then LoadDailySales(invNo) Else If mode = "Monthly" Then LoadMonthlySales(invNo) Else LoadMonthlyItemSales(invNo)
            Else
                MessageBox.Show("Invoice Not Found")
            End If
            conn.Close()
        Catch ex As Exception
            MessageBox.Show("Error searching invoice: " & ex.Message)
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    ' --- DAILY SALES LOGIC ---

    Private Sub btnPrimary_Click(sender As Object, e As EventArgs) Handles btnPrimary.Click
        Dim searchInv As String = TextBoxInvNo.Text.Trim()
        Dim selectedDate As Date = DateTimePicker1.Value.Date
        
        Dim filterInvType As String = If(ComboBox4.SelectedItem IsNot Nothing, ComboBox4.SelectedItem.ToString(), "Normal")
        Dim filterBilling As String = If(ComboBoxBilling.SelectedItem IsNot Nothing, ComboBoxBilling.SelectedItem.ToString(), "Cash")
        Dim isAdvance As Boolean = CheckBoxAdvDaily.Checked

        Dim viewer As New SalesHistoryForm()
        viewer.SetReportContext(0, selectedDate, selectedDate, searchInv, CheckBoxAllDaily.Checked, filterInvType, filterBilling, isAdvance)
        viewer.Show()
    End Sub

    ' --- MONTHLY SALES SUMMARY REPORT ---
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim searchInv As String = TextBoxInvNoM.Text.Trim()
        Dim startDate As Date = DateTimePicker4.Value.Date
        Dim endDate As Date = DateTimePicker5.Value.Date

        Dim filterInvType As String = If(ComboBox3.SelectedItem IsNot Nothing, ComboBox3.SelectedItem.ToString(), "Normal")
        Dim filterBilling As String = If(ComboBoxBillingTypeM.SelectedItem IsNot Nothing, ComboBoxBillingTypeM.SelectedItem.ToString(), "Cash")
        Dim isAdvance As Boolean = CheckBoxAdvMonthly.Checked

        Dim viewer As New SalesHistoryForm()
        viewer.SetReportContext(1, startDate, endDate, searchInv, CheckBoxAllMonthly.Checked, filterInvType, filterBilling, isAdvance)
        viewer.Show()
    End Sub

    ' --- MONTHLY ITEM SALES REPORT ---
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim searchInv As String = TextBoxInvNoMI.Text.Trim()
        Dim startDate As Date = DateTimePickerItemStart.Value.Date
        Dim endDate As Date = DateTimePickerItemEnd.Value.Date

        Dim filterInvType As String = If(ComboBoxItemInvType.SelectedItem IsNot Nothing, ComboBoxItemInvType.SelectedItem.ToString(), "Normal")
        Dim filterBilling As String = If(ComboBoxBillingMI.SelectedItem IsNot Nothing, ComboBoxBillingMI.SelectedItem.ToString(), "Cash")
        Dim isAdvance As Boolean = CheckBoxAdvItem.Checked

        Dim viewer As New SalesHistoryForm()
        viewer.SetReportContext(2, startDate, endDate, searchInv, CheckBoxAllItem.Checked, filterInvType, filterBilling, isAdvance, TextBox3.Text.Trim(), "", TextBox4.Text.Trim())
        viewer.Show()
    End Sub

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        Try
            If e.RowIndex >= 0 Then
                Dim row = DataGridView1.Rows(e.RowIndex)
                ' Correctly check if columns exist in the grid
                If DataGridView1.Columns.Contains("Item Code") AndAlso DataGridView1.Columns.Contains("Description") Then
                    Dim itemCode As String = row.Cells("Item Code").Value.ToString()
                    Dim itemName As String = row.Cells("Description").Value.ToString()

                    NameTextBox.Text = itemName
                    StockTextBox.Text = GetCurrentStock(itemCode).ToString("N2")
                End If
            End If
        Catch ex As Exception
            ' Silently fail
        End Try
    End Sub

    Private Sub DataGridView2_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView2.CellClick
        Try
            If e.RowIndex >= 0 Then
                Dim row = DataGridView2.Rows(e.RowIndex)
                ' Correctly check if columns exist in the grid
                If DataGridView2.Columns.Contains("Item Code") AndAlso DataGridView2.Columns.Contains("Description") Then
                    Dim itemCode As String = row.Cells("Item Code").Value.ToString()
                    Dim itemName As String = row.Cells("Description").Value.ToString()

                    TextBox2.Text = itemName
                    TextBox1.Text = GetCurrentStock(itemCode).ToString("N2")
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub DataGridView3_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView3.CellClick
        Try
            If e.RowIndex >= 0 Then
                Dim row = DataGridView3.Rows(e.RowIndex)
                ' Correctly check if columns exist in the grid
                If DataGridView3.Columns.Contains("Item Code") AndAlso DataGridView3.Columns.Contains("Description") Then
                    Dim itemCode As String = row.Cells("Item Code").Value.ToString()
                    Dim itemName As String = row.Cells("Description").Value.ToString()

                    ' For Itemized view, update the Daily tab's boxes as well
                    NameTextBox.Text = itemName
                    StockTextBox.Text = GetCurrentStock(itemCode).ToString("N2")
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Function GetCurrentStock(itemId As String) As Decimal
        Dim stock As Decimal = 0
        Try
            Using localConn As New MySqlConnection(Module1.ConnStr)
                localConn.Open()
                Dim sql = "SELECT SUM(st_qty) FROM items_stock WHERE item_id = @id"
                Using cmd As New MySqlCommand(sql, localConn)
                    cmd.Parameters.AddWithValue("@id", itemId)
                    Dim res = cmd.ExecuteScalar()
                    stock = If(res Is DBNull.Value Or res Is Nothing, 0, Convert.ToDecimal(res))
                End Using
            End Using
        Catch
        End Try
        Return stock
    End Function

    Private Sub LoadDailySales(Optional searchInvNo As String = "")
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()

            Dim startDate As String = DateTimePicker1.Value.ToString("yyyy-MM-dd") & " 00:00:00"
            Dim endDate As String = DateTimePicker1.Value.ToString("yyyy-MM-dd") & " 23:59:59"
            Dim filterInvType As String = If(ComboBox4.SelectedItem IsNot Nothing, ComboBox4.SelectedItem.ToString(), "Normal")
            Dim filterBillingType As String = If(ComboBoxBilling.SelectedItem IsNot Nothing, ComboBoxBilling.SelectedItem.ToString(), "Cash")
            Dim isAdvance As Boolean = CheckBoxAdvDaily.Checked

            ' Dynamic SQL with JOIN and optimized date filtering
            Dim sqlDaily As String = "SELECT b.id as 'id', b.inv_no, ds.item_id as 'Item Code', i.description as 'Description', bi.quantity as 'Qty', " &
                               "bi.unit_price as 'Unit Price', bi.discount as 'Disc %', bi.item_cost as 'Cost', " &
                               "ds.amount as 'Total Amount', " &
                               "ds.amount * (1 + COALESCE(v.vat_value, 0)/100) as 'Price (+VAT)', " &
                               "ds.amount * (COALESCE(v.vat_value, 0)/100) as 'VAT Amt', " &
                               "ds.profit as 'Profit', " &
                               "b.subtotal, b.paid_amount, b.balance_due " &
                               "FROM daily_sale ds " &
                               "JOIN billing_item bi ON (ds.billing_item_id = bi.id OR (ds.billing_item_id = bi.billing_id AND ds.item_id = bi.item_id)) " &
                               "JOIN billing b ON bi.billing_id = b.id " &
                               "JOIN items i ON ds.item_id = i.id " &
                               "LEFT JOIN vat v ON b.vat_id = v.id " &
                               "WHERE LOWER(TRIM(b.status)) IN ('paid', 'success', 'completed', 'advance', 'credit', 'partial_credit', 'cheque', 'partial_cheque', 'cash_credit', 'cash_cheque', 'mixed_payment', 'credit_cheque') " &
                               (If(Module1.IsRgrVisible, "", " AND b.is_rgr = 0 AND b.inv_no NOT LIKE 'GR%' AND b.inv_no NOT LIKE 'RGR%' "))

            If Not String.IsNullOrEmpty(searchInvNo) Then
                sqlDaily &= "AND b.inv_no = @invNo "
            Else
                sqlDaily &= "AND b.timestamps >= @start AND b.timestamps <= @end "
                ' Apply advanced filters only if "All" is NOT checked
                If Not CheckBoxAllDaily.Checked Then
                    sqlDaily &= "AND b.inv_type = @invType "
                    If isAdvance Then
                        sqlDaily &= "AND b.advance_payment != 0 "
                    Else
                        Select Case filterBillingType
                            Case "Cash"
                                sqlDaily &= "AND LOWER(TRIM(b.status)) = 'paid' "
                            Case "Cash (Cash)"
                                sqlDaily &= "AND LOWER(TRIM(b.status)) = 'paid' AND LOWER(TRIM(b.payment_type)) = 'cash' "
                            Case "Cash (Cards/Online)"
                                sqlDaily &= "AND LOWER(TRIM(b.status)) = 'paid' AND LOWER(TRIM(b.payment_type)) IN ('credit card', 'debit card', 'online transfer') "
                            Case "Credit"
                                sqlDaily &= "AND LOWER(TRIM(b.status)) IN ('credit', 'partial_credit') "
                            Case "Cheque"
                                sqlDaily &= "AND LOWER(TRIM(b.status)) IN ('cheque', 'partial_cheque') "
                            Case "Cash+Credit"
                                sqlDaily &= "AND LOWER(TRIM(b.status)) = 'cash_credit' "
                            Case "Cash+Cheque"
                                sqlDaily &= "AND LOWER(TRIM(b.status)) = 'cash_cheque' "
                            Case "Mixed Payment"
                                sqlDaily &= "AND LOWER(TRIM(b.status)) = 'mixed_payment' "
                            Case "Credit+Cheque"
                                sqlDaily &= "AND LOWER(TRIM(b.status)) = 'credit_cheque' "
                            Case Else
                                sqlDaily &= "AND b.billing_type = @bType "
                        End Select
                    End If
                End If
            End If

            Dim cmd As New MySqlCommand(sqlDaily, conn)
            If Not String.IsNullOrEmpty(searchInvNo) Then
                cmd.Parameters.AddWithValue("@invNo", searchInvNo)
            Else
                cmd.Parameters.AddWithValue("@start", startDate)
                cmd.Parameters.AddWithValue("@end", endDate)
                ' Only add params if "All" is NOT checked
                If Not CheckBoxAllDaily.Checked Then
                    cmd.Parameters.AddWithValue("@invType", filterInvType)
                    If Not isAdvance Then
                        cmd.Parameters.AddWithValue("@bType", filterBillingType)
                    End If
                End If
            End If

            Dim da As New MySqlDataAdapter(cmd)
            Dim dt As New DataTable()
            da.Fill(dt)

            DataGridView1.DataSource = dt

            ' Format Grid
            If DataGridView1.Columns.Count > 0 Then
                DataGridView1.Columns("Item Code").Width = 280
                DataGridView1.Columns("Description").Width = 650
                DataGridView1.Columns("Qty").Width = 120
                DataGridView1.Columns("Unit Price").Width = 200
                DataGridView1.Columns("Unit Price").DefaultCellStyle.Format = "N2"
                DataGridView1.Columns("Total Amount").Width = 200
                DataGridView1.Columns("Total Amount").DefaultCellStyle.Format = "N2"
                DataGridView1.Columns("Price (+VAT)").Width = 200
                DataGridView1.Columns("Price (+VAT)").DefaultCellStyle.Format = "N2"
                DataGridView1.Columns("VAT Amt").Width = 180
                DataGridView1.Columns("VAT Amt").DefaultCellStyle.Format = "N2"
                DataGridView1.Columns("Cost").Width = 180
                DataGridView1.Columns("Cost").DefaultCellStyle.Format = "N2"

                ' Hide summary columns from grid but keep for calculations
                If DataGridView1.Columns.Contains("subtotal") Then DataGridView1.Columns("subtotal").Visible = False
                If DataGridView1.Columns.Contains("paid_amount") Then DataGridView1.Columns("paid_amount").Visible = False
                If DataGridView1.Columns.Contains("balance_due") Then DataGridView1.Columns("balance_due").Visible = False
                If DataGridView1.Columns.Contains("inv_no") Then DataGridView1.Columns("inv_no").Visible = False
                If DataGridView1.Columns.Contains("id") Then DataGridView1.Columns("id").Visible = False
                If DataGridView1.Columns.Contains("Profit") Then DataGridView1.Columns("Profit").Visible = False
            End If

            CalculateDailyTotals(dt, Not String.IsNullOrEmpty(searchInvNo))

            conn.Close()
        Catch ex As Exception
            MessageBox.Show("Error loading daily sales: " & ex.Message)
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub CalculateDailyTotals(dt As DataTable, isSearch As Boolean)
        Dim totCost As Decimal = 0
        Dim totPricePlusVat As Decimal = 0
        Dim totProfit As Decimal = 0
        Dim totSubtotal As Decimal = 0
        Dim totPaid As Decimal = 0
        Dim totBalance As Decimal = 0

        ' Item level sums
        For Each row As DataRow In dt.Rows
            totPricePlusVat += Convert.ToDecimal(row("Price (+VAT)"))
            ' Calculate profit correctly from Price - Cost
            Dim rowAmount As Decimal = Convert.ToDecimal(row("Total Amount"))
            Dim rowCost As Decimal = Convert.ToDecimal(row("Cost")) * Convert.ToDecimal(row("Qty"))
            totProfit += (rowAmount - rowCost)
            totCost += rowCost
            totSubtotal += rowAmount
        Next

        ' Bill level sums (to avoid double counting paid/balance for multi-item bills)
        If dt.Columns.Contains("inv_no") Then
            Dim uniqueBills = dt.DefaultView.ToTable(True, "inv_no", "paid_amount", "balance_due")
            For Each row As DataRow In uniqueBills.Rows
                totPaid += Convert.ToDecimal(row("paid_amount"))
                totBalance += Convert.ToDecimal(row("balance_due"))
            Next
        End If

        ' --- ADD ADJUSTMENTS TO TOTALS ---
        If Not isSearch Then
            Try
                Dim startD As String = DateTimePicker1.Value.ToString("yyyy-MM-dd") & " 00:00:00"
                Dim endD As String = DateTimePicker1.Value.ToString("yyyy-MM-dd") & " 23:59:59"
                Dim sqlAdj As String = "SELECT SUM(a.difference_amount) FROM sales_adjustments a JOIN billing b ON a.inv_no = b.inv_no WHERE a.adjustment_date >= @s AND a.adjustment_date <= @e AND (b.timestamps < @s OR b.timestamps > @e) " & (If(Module1.IsRgrVisible, "", " AND a.is_rgr = 0 AND a.inv_no NOT LIKE 'GR%' AND a.inv_no NOT LIKE 'RGR%' "))
                Using cmdAdj As New MySqlCommand(sqlAdj, conn)
                    If conn.State = ConnectionState.Closed Then conn.Open()
                    cmdAdj.Parameters.AddWithValue("@s", startD)
                    cmdAdj.Parameters.AddWithValue("@e", endD)
                    Dim adjRes = cmdAdj.ExecuteScalar()
                    If adjRes IsNot Nothing AndAlso Not IsDBNull(adjRes) Then
                        Dim adjVal = Convert.ToDecimal(adjRes)
                        If Math.Abs(adjVal) < 10000000 Then
                            totPricePlusVat += adjVal
                            totSubtotal += adjVal
                            totPaid += adjVal
                        End If
                    End If
                End Using
            Catch
            End Try
        End If

        CostLbl.Text = totCost.ToString("N2")
        Label10.Text = totPricePlusVat.ToString("N2")
        WpriceLbl.Text = totProfit.ToString("N2")

        Label32.Text = totSubtotal.ToString("N2")
        ' Always show Paid and Balance totals for the filtered set
        Label28.Text = totPaid.ToString("N2")
        Label30.Text = totBalance.ToString("N2")
    End Sub

    ' --- MONTHLY SALES LOGIC ---

    Private Sub LoadMonthlySales(Optional searchInvNo As String = "")
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()

            Dim startDate As String = DateTimePicker4.Value.ToString("yyyy-MM-dd")
            Dim endDate As String = DateTimePicker5.Value.ToString("yyyy-MM-dd")
            Dim filterInvType As String = If(ComboBox3.SelectedItem IsNot Nothing, ComboBox3.SelectedItem.ToString(), "Normal")
            Dim filterBillingType As String = If(ComboBoxBillingTypeM.SelectedItem IsNot Nothing, ComboBoxBillingTypeM.SelectedItem.ToString(), "Cash")
            Dim isAdvance As Boolean = CheckBoxAdvMonthly.Checked

            ' Hide Quick View boxes for Monthly (they are for itemized views)
            TextBox1.Visible = False
            TextBox2.Visible = False
            Label13.Visible = False
            Label14.Visible = False

            Dim billingTable As String = "billing"

            ' Monthly summary grouping by Date OR showing single invoice
            Dim sqlMonthly As String = ""
            If Not String.IsNullOrEmpty(searchInvNo) Then
                ' Adjust item table for search
                Dim itemTable As String = "billing_item"

                sqlMonthly = "SELECT b.id as 'id', DATE(b.timestamps) as 'Date', b.inv_no as 'Inv No', " &
                            "SUM(i.item_cost * i.quantity) as 'Total Cost', " &
                            "SUM((i.unit_price - (i.unit_price * i.discount / 100)) * i.quantity) as 'Total Price', " &
                            "SUM(((i.unit_price - (i.unit_price * i.discount / 100)) * i.quantity) * (1 + COALESCE(v.vat_value, 0)/100)) as 'Price (+VAT)', " &
                            "SUM(((i.unit_price - (i.unit_price * i.discount / 100)) * i.quantity) * (COALESCE(v.vat_value, 0)/100)) as 'VAT Amt', " &
                            "SUM(((i.unit_price - (i.unit_price * i.discount / 100)) - i.item_cost) * i.quantity) as 'Profit', " &
                            "ANY_VALUE(b.subtotal) as subtotal, ANY_VALUE(b.paid_amount) as paid_amount, ANY_VALUE(b.balance_due) as balance_due " &
                            "FROM " & billingTable & " b " &
                            "JOIN " & itemTable & " i ON b.id = i.billing_id " &
                            "LEFT JOIN vat v ON b.vat_id = v.id " &
                            "WHERE b.inv_no = @invNo AND LOWER(TRIM(b.status)) IN ('paid', 'success', 'completed', 'advance', 'credit', 'partial_credit', 'cheque', 'partial_cheque', 'cash_credit', 'cash_cheque', 'mixed_payment', 'credit_cheque') " & (If(Module1.IsRgrVisible, "", " AND b.is_rgr = 0 AND b.inv_no NOT LIKE 'GR%' AND b.inv_no NOT LIKE 'RGR%' ")) &
                            "GROUP BY DATE(b.timestamps), b.inv_no, b.id"
            Else
                ' Use a more reliable aggregation from billing and billing_item
                sqlMonthly = "SELECT DATE(b.timestamps) as 'Date', SUM(b.subtotal) as 'Total Price', " &
                            "SUM(b.subtotal - IFNULL(bi_sum.cost, 0)) as 'Profit', " &
                            "SUM(b.paid_amount) as 'Paid_Sum', SUM(b.balance_due) as 'Balance_Sum' " &
                            "FROM billing b " &
                            "LEFT JOIN (SELECT billing_id, SUM(item_cost * quantity) as cost FROM billing_item GROUP BY billing_id) bi_sum ON b.id = bi_sum.billing_id " &
                            "WHERE LOWER(TRIM(b.status)) IN ('paid', 'success', 'completed', 'advance', 'credit', 'partial_credit', 'cheque', 'partial_cheque', 'cash_credit', 'cash_cheque', 'mixed_payment', 'credit_cheque') AND b.timestamps >= @start AND b.timestamps <= @end " & (If(Module1.IsRgrVisible, "", " AND b.is_rgr = 0 AND b.inv_no NOT LIKE 'GR%' AND b.inv_no NOT LIKE 'RGR%' "))

                ' Apply advanced filters only if "All" is NOT checked
                If Not CheckBoxAllMonthly.Checked Then
                    sqlMonthly &= " AND b.inv_type = @invType "
                    If isAdvance Then
                        sqlMonthly &= " AND b.advance_payment != 0 "
                    Else
                        Select Case filterBillingType
                            Case "Cash"
                                sqlMonthly &= " AND LOWER(TRIM(b.status)) = 'paid' "
                            Case "Cash (Cash)"
                                sqlMonthly &= " AND LOWER(TRIM(b.status)) = 'paid' AND LOWER(TRIM(b.payment_type)) = 'cash' "
                            Case "Cash (Cards/Online)"
                                sqlMonthly &= " AND LOWER(TRIM(b.status)) = 'paid' AND LOWER(TRIM(b.payment_type)) IN ('credit card', 'debit card', 'online transfer') "
                            Case "Credit"
                                sqlMonthly &= " AND LOWER(TRIM(b.status)) IN ('credit', 'partial_credit') "
                            Case "Cheque"
                                sqlMonthly &= " AND LOWER(TRIM(b.status)) IN ('cheque', 'partial_cheque') "
                            Case "Cash+Credit"
                                sqlMonthly &= " AND LOWER(TRIM(b.status)) = 'cash_credit' "
                            Case "Cash+Cheque"
                                sqlMonthly &= " AND LOWER(TRIM(b.status)) = 'cash_cheque' "
                            Case "Mixed Payment"
                                sqlMonthly &= " AND LOWER(TRIM(b.status)) = 'mixed_payment' "
                            Case "Credit+Cheque"
                                sqlMonthly &= " AND LOWER(TRIM(b.status)) = 'credit_cheque' "
                            Case Else
                                sqlMonthly &= " AND b.billing_type = @bType "
                        End Select
                    End If
                End If

                sqlMonthly &= " GROUP BY DATE(b.timestamps) ORDER BY DATE(b.timestamps) DESC"
            End If

            Dim cmd As New MySqlCommand(sqlMonthly, conn)
            If Not String.IsNullOrEmpty(searchInvNo) Then
                cmd.Parameters.AddWithValue("@invNo", searchInvNo)
            Else
                cmd.Parameters.AddWithValue("@start", startDate & " 00:00:00")
                cmd.Parameters.AddWithValue("@end", endDate & " 23:59:59")
                ' Only add params if "All" is NOT checked
                If Not CheckBoxAllMonthly.Checked Then
                    cmd.Parameters.AddWithValue("@invType", filterInvType)
                    If Not isAdvance Then
                        cmd.Parameters.AddWithValue("@bType", filterBillingType)
                    End If
                End If
            End If

            Dim da As New MySqlDataAdapter(cmd)
            Dim dt As New DataTable()
            da.Fill(dt)

            DataGridView2.DataSource = dt

            ' Format Grid - guard every column with Contains() because summary and search queries have different columns
            If DataGridView2.Columns.Count > 0 Then
                If DataGridView2.Columns.Contains("id") Then DataGridView2.Columns("id").Visible = False
                If DataGridView2.Columns.Contains("Date") Then DataGridView2.Columns("Date").Width = 350
                If DataGridView2.Columns.Contains("Total Cost") Then
                    DataGridView2.Columns("Total Cost").Width = 250
                    DataGridView2.Columns("Total Cost").DefaultCellStyle.Format = "N2"
                End If
                If DataGridView2.Columns.Contains("Total Price") Then
                    DataGridView2.Columns("Total Price").Width = 250
                    DataGridView2.Columns("Total Price").DefaultCellStyle.Format = "N2"
                End If
                If DataGridView2.Columns.Contains("Price (+VAT)") Then
                    DataGridView2.Columns("Price (+VAT)").Width = 250
                    DataGridView2.Columns("Price (+VAT)").DefaultCellStyle.Format = "N2"
                End If
                If DataGridView2.Columns.Contains("VAT Amt") Then
                    DataGridView2.Columns("VAT Amt").Width = 220
                    DataGridView2.Columns("VAT Amt").DefaultCellStyle.Format = "N2"
                End If
                If DataGridView2.Columns.Contains("Profit") Then
                    DataGridView2.Columns("Profit").Width = 220
                    DataGridView2.Columns("Profit").DefaultCellStyle.Format = "N2"
                End If
                If DataGridView2.Columns.Contains("Inv No") Then DataGridView2.Columns("Inv No").Width = 220
                If DataGridView2.Columns.Contains("Paid_Sum") Then
                    DataGridView2.Columns("Paid_Sum").Width = 220
                    DataGridView2.Columns("Paid_Sum").DefaultCellStyle.Format = "N2"
                End If
                If DataGridView2.Columns.Contains("Balance_Sum") Then
                    DataGridView2.Columns("Balance_Sum").Width = 220
                    DataGridView2.Columns("Balance_Sum").DefaultCellStyle.Format = "N2"
                End If
            End If

            CalculateMonthlyTotals(dt, Not String.IsNullOrEmpty(searchInvNo))

            conn.Close()
        Catch ex As Exception
            MessageBox.Show("Error loading monthly sales: " & ex.Message)
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub CalculateMonthlyTotals(dt As DataTable, isSearch As Boolean)
        Dim totCost As Decimal = 0
        Dim totPricePlusVat As Decimal = 0
        Dim totProfit As Decimal = 0
        Dim totSubtotal As Decimal = 0
        Dim totPaid As Decimal = 0
        Dim totBalance As Decimal = 0

        ' Detect which query ran by checking available columns
        Dim hasCostCol As Boolean = dt.Columns.Contains("Total Cost")
        Dim hasVatCol As Boolean = dt.Columns.Contains("Price (+VAT)")
        Dim hasSubtotalSum As Boolean = dt.Columns.Contains("Subtotal_Sum")
        Dim hasPaidSum As Boolean = dt.Columns.Contains("Paid_Sum")
        Dim hasIndivSubtotal As Boolean = dt.Columns.Contains("subtotal")

        For Each row As DataRow In dt.Rows
            Dim rowPrice As Decimal = 0
            If hasVatCol Then
                rowPrice = Convert.ToDecimal(row("Price (+VAT)"))
            ElseIf dt.Columns.Contains("Total Price") Then
                rowPrice = Convert.ToDecimal(row("Total Price"))
            End If

            totPricePlusVat += rowPrice

            Dim rowProfit As Decimal = 0
            If dt.Columns.Contains("Profit") Then
                rowProfit = Convert.ToDecimal(row("Profit"))
            End If
            totProfit += rowProfit

            Dim rowCost As Decimal = rowPrice - rowProfit
            totCost += rowCost

            Dim rowPaid As Decimal = If(hasPaidSum, Convert.ToDecimal(row("Paid_Sum")), 0)
            Dim rowBalance As Decimal = If(hasPaidSum, Convert.ToDecimal(row("Balance_Sum")), 0)

            If hasSubtotalSum Then
                Dim st = Convert.ToDecimal(row("Subtotal_Sum"))
                ' Safety: Ignore daily subtotals > 10M in the dashboard totals
                If Math.Abs(st) < 10000000 Then totSubtotal += st
            Else
                If Math.Abs(rowPrice) < 10000000 Then totSubtotal += rowPrice
            End If

            If hasPaidSum Then
                ' Safety: Ignore daily paid/balance > 10M in the dashboard totals
                If Math.Abs(rowPaid) < 10000000 Then
                    totPaid += rowPaid
                    totBalance += rowBalance
                End If
            ElseIf hasIndivSubtotal Then
                ' Single invoice search
                Dim ip = Convert.ToDecimal(row("paid_amount"))
                Dim ib = Convert.ToDecimal(row("balance_due"))
                If Math.Abs(ip) < 10000000 Then
                    totPaid += ip
                    totBalance += ib
                End If
            End If
        Next

        ' --- ADD ADJUSTMENTS TO TOTALS ---
        If Not isSearch Then
            Try
                Dim sDate As String = DateTimePicker4.Value.ToString("yyyy-MM-dd") & " 00:00:00"
                Dim eDate As String = DateTimePicker5.Value.ToString("yyyy-MM-dd") & " 23:59:59"
                Dim sqlAdj As String = "SELECT SUM(a.difference_amount) FROM sales_adjustments a JOIN billing b ON a.inv_no = b.inv_no WHERE a.adjustment_date >= @s AND a.adjustment_date <= @e AND (b.timestamps < @s OR b.timestamps > @e) " & (If(Module1.IsRgrVisible, "", " AND a.is_rgr = 0 AND a.inv_no NOT LIKE 'GR%' AND a.inv_no NOT LIKE 'RGR%' "))
                Using cmdAdj As New MySqlCommand(sqlAdj, conn)
                    If conn.State = ConnectionState.Closed Then conn.Open()
                    cmdAdj.Parameters.AddWithValue("@s", sDate)
                    cmdAdj.Parameters.AddWithValue("@e", eDate)
                    Dim adjRes = cmdAdj.ExecuteScalar()
                    If adjRes IsNot Nothing AndAlso Not IsDBNull(adjRes) Then
                        Dim adjVal = Convert.ToDecimal(adjRes)
                        If Math.Abs(adjVal) < 10000000 Then
                            totPricePlusVat += adjVal
                            totSubtotal += adjVal
                            totPaid += adjVal
                        End If
                    End If
                End Using
            Catch
            End Try
        End If

        Label8.Text = totCost.ToString("N2")
        Label3.Text = totPricePlusVat.ToString("N2")
        Label5.Text = totProfit.ToString("N2")

        LabelTotalPriceM.Text = totSubtotal.ToString("N2")
        ' Always show Paid and Balance totals for the filtered set
        LabelPaidAmountM.Text = totPaid.ToString("N2")
        LabelBalanceM.Text = totBalance.ToString("N2")
    End Sub

    ' --- MONTHLY ITEM SALES LOGIC ---

    Private Sub LoadMonthlyItemSales(Optional searchInvNo As String = "")
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()

            Dim startDate As String = DateTimePickerItemStart.Value.ToString("yyyy-MM-dd")
            Dim endDate As String = DateTimePickerItemEnd.Value.ToString("yyyy-MM-dd")
            Dim filterInvType As String = If(ComboBoxItemInvType.SelectedItem IsNot Nothing, ComboBoxItemInvType.SelectedItem.ToString(), "Normal")
            Dim filterBillingType As String = If(ComboBoxBillingMI.SelectedItem IsNot Nothing, ComboBoxBillingMI.SelectedItem.ToString(), "Cash")
            Dim isAdvance As Boolean = CheckBoxAdvItem.Checked

            Dim billingTable As String = "billing"
            Dim itemTable As String = "billing_item"

            ' Monthly Itemized grouping by Item ID
            Dim sqlItemSales As String = "SELECT i.item_id as 'Item Code', i.description as 'Description', SUM(i.quantity) as 'Qty', " &
                                       "AVG(i.item_cost) as 'Avg Cost', " &
                                       "SUM(i.item_cost * i.quantity) as 'Total Cost', " &
                                       "SUM((i.unit_price - (i.unit_price * i.discount / 100)) * i.quantity) as 'Total Price', " &
                                       "SUM(((i.unit_price - (i.unit_price * i.discount / 100)) * i.quantity) * (1 + COALESCE(v.vat_value, 0)/100)) as 'Price (+VAT)', " &
                                       "SUM(((i.unit_price - (i.unit_price * i.discount / 100)) * i.quantity) * (COALESCE(v.vat_value, 0)/100)) as 'VAT Amt', " &
                                       "SUM(((i.unit_price - (i.unit_price * i.discount / 100)) - i.item_cost) * i.quantity) as 'Profit', " &
                                       "ANY_VALUE(b_sum.st) as Subtotal_Sum, ANY_VALUE(b_sum.pa) as Paid_Sum, ANY_VALUE(b_sum.bd) as Balance_Sum " &
                                       "FROM " & itemTable & " i " &
                                       "JOIN " & billingTable & " b ON i.billing_id = b.id " &
                                       "LEFT JOIN vat v ON b.vat_id = v.id " &
                                       "CROSS JOIN ( " &
                                       "  SELECT SUM(subtotal) as st, SUM(paid_amount) as pa, SUM(balance_due) as bd " &
                                       "  FROM " & billingTable & " WHERE LOWER(TRIM(status)) IN ('paid', 'success', 'completed', 'advance', 'credit', 'partial_credit', 'cheque', 'partial_cheque', 'cash_credit', 'cash_cheque', 'mixed_payment', 'credit_cheque') AND timestamps >= @start AND timestamps <= @end " & (If(Module1.IsRgrVisible, "", " AND is_rgr = 0 AND inv_no NOT LIKE 'GR%' AND inv_no NOT LIKE 'RGR%' "))

            ' Apply advanced filters to b_sum subquery only if "All" is NOT checked
            If Not CheckBoxAllItem.Checked Then
                sqlItemSales &= " AND inv_type = @invType "
                If isAdvance Then
                    sqlItemSales &= " AND advance_payment != 0 "
                Else
                    Select Case filterBillingType
                        Case "Cash"
                            sqlItemSales &= " AND LOWER(TRIM(status)) = 'paid' "
                        Case "Cash (Cash)"
                            sqlItemSales &= " AND LOWER(TRIM(status)) = 'paid' AND LOWER(TRIM(payment_type)) = 'cash' "
                        Case "Cash (Cards/Online)"
                            sqlItemSales &= " AND LOWER(TRIM(status)) = 'paid' AND LOWER(TRIM(payment_type)) IN ('credit card', 'debit card', 'online transfer') "
                        Case "Credit"
                            sqlItemSales &= " AND LOWER(status) IN ('credit', 'partial_credit') "
                        Case "Cheque"
                            sqlItemSales &= " AND LOWER(status) IN ('cheque', 'partial_cheque') "
                        Case "Cash+Credit"
                            sqlItemSales &= " AND LOWER(status) = 'cash_credit' "
                        Case "Cash+Cheque"
                            sqlItemSales &= " AND LOWER(status) = 'cash_cheque' "
                        Case "Mixed Payment"
                            sqlItemSales &= " AND LOWER(status) = 'mixed_payment' "
                        Case "Credit+Cheque"
                            sqlItemSales &= " AND LOWER(status) = 'credit_cheque' "
                        Case Else
                            sqlItemSales &= " AND billing_type = @bType "
                    End Select
                End If
            End If

            sqlItemSales &= " ) b_sum " &
                       "WHERE LOWER(TRIM(b.status)) IN ('paid', 'success', 'completed', 'advance', 'credit', 'partial_credit', 'cheque', 'partial_cheque', 'cash_credit', 'cash_cheque', 'mixed_payment', 'credit_cheque') " & (If(Module1.IsRgrVisible, "", " AND b.is_rgr = 0 AND b.inv_no NOT LIKE 'GR%' AND b.inv_no NOT LIKE 'RGR%' "))

            If Not String.IsNullOrEmpty(searchInvNo) Then
                sqlItemSales &= "AND b.inv_no = @invNo "
            Else
                sqlItemSales &= "AND b.timestamps >= @start AND b.timestamps <= @end "
                ' Apply advanced filters to main query only if "All" is NOT checked
                If Not CheckBoxAllItem.Checked Then
                    sqlItemSales &= "AND b.inv_type = @invType "
                    If isAdvance Then
                        sqlItemSales &= "AND b.advance_payment != 0 "
                    Else
                        Select Case filterBillingType
                            Case "Cash"
                                sqlItemSales &= "AND LOWER(TRIM(b.status)) = 'paid' "
                            Case "Cash (Cash)"
                                sqlItemSales &= "AND LOWER(TRIM(b.status)) = 'paid' AND LOWER(TRIM(b.payment_type)) = 'cash' "
                            Case "Cash (Cards/Online)"
                                sqlItemSales &= "AND LOWER(TRIM(b.status)) = 'paid' AND LOWER(TRIM(b.payment_type)) IN ('credit card', 'debit card', 'online transfer') "
                            Case "Credit"
                                sqlItemSales &= "AND LOWER(TRIM(b.status)) IN ('credit', 'partial_credit') "
                            Case "Cheque"
                                sqlItemSales &= "AND LOWER(TRIM(b.status)) IN ('cheque', 'partial_cheque') "
                            Case "Cash+Credit"
                                sqlItemSales &= "AND LOWER(TRIM(b.status)) = 'cash_credit' "
                            Case "Cash+Cheque"
                                sqlItemSales &= "AND LOWER(TRIM(b.status)) = 'cash_cheque' "
                            Case "Mixed Payment"
                                sqlItemSales &= "AND LOWER(TRIM(b.status)) = 'mixed_payment' "
                            Case "Credit+Cheque"
                                sqlItemSales &= "AND LOWER(TRIM(b.status)) = 'credit_cheque' "
                            Case Else
                                sqlItemSales &= "AND b.billing_type = @bType "
                        End Select
                    End If
                End If
                If Not String.IsNullOrEmpty(TextBox3.Text.Trim()) Then
                    sqlItemSales &= "AND i.item_id LIKE @itemCode "
                End If
                If Not String.IsNullOrEmpty(TextBox4.Text.Trim()) Then
                    sqlItemSales &= "AND i.description LIKE @itemDesc "
                End If
            End If

            sqlItemSales &= "GROUP BY i.item_id, i.description "
            If String.IsNullOrEmpty(searchInvNo) AndAlso (Not String.IsNullOrEmpty(TextBox3.Text.Trim()) OrElse Not String.IsNullOrEmpty(TextBox4.Text.Trim())) Then
                sqlItemSales &= "ORDER BY TRIM(i.description) ASC"
            Else
                sqlItemSales &= "ORDER BY SUM(i.quantity) DESC"
            End If

            Dim cmd As New MySqlCommand(sqlItemSales, conn)
            If Not String.IsNullOrEmpty(searchInvNo) Then
                cmd.Parameters.AddWithValue("@invNo", searchInvNo)
            Else
                cmd.Parameters.AddWithValue("@start", startDate & " 00:00:00")
                cmd.Parameters.AddWithValue("@end", endDate & " 23:59:59")
                ' Only add params if "All" is NOT checked
                If Not CheckBoxAllItem.Checked Then
                    cmd.Parameters.AddWithValue("@invType", filterInvType)
                    If Not isAdvance Then
                        cmd.Parameters.AddWithValue("@bType", filterBillingType)
                    End If
                End If
                If Not String.IsNullOrEmpty(TextBox3.Text.Trim()) Then
                    cmd.Parameters.AddWithValue("@itemCode", "%" & TextBox3.Text.Trim() & "%")
                End If
                If Not String.IsNullOrEmpty(TextBox4.Text.Trim()) Then
                    cmd.Parameters.AddWithValue("@itemDesc", TextBox4.Text.Trim() & "%")
                End If
            End If

            Dim da As New MySqlDataAdapter(cmd)
            Dim dt As New DataTable()
            da.Fill(dt)

            DataGridView3.DataSource = dt

            ' Format Grid
            If DataGridView3.Columns.Count > 0 Then
                DataGridView3.Columns("Item Code").Width = 280
                DataGridView3.Columns("Description").Width = 650
                DataGridView3.Columns("Qty").Width = 120
                DataGridView3.Columns("Avg Cost").Width = 180
                DataGridView3.Columns("Avg Cost").DefaultCellStyle.Format = "N2"
                DataGridView3.Columns("Total Cost").Width = 200
                DataGridView3.Columns("Total Cost").DefaultCellStyle.Format = "N2"
                DataGridView3.Columns("Total Price").Width = 200
                DataGridView3.Columns("Total Price").DefaultCellStyle.Format = "N2"
                DataGridView3.Columns("Price (+VAT)").Width = 200
                DataGridView3.Columns("Price (+VAT)").DefaultCellStyle.Format = "N2"
                DataGridView3.Columns("VAT Amt").Width = 180
                DataGridView3.Columns("VAT Amt").DefaultCellStyle.Format = "N2"
                DataGridView3.Columns("Profit").Width = 180
                DataGridView3.Columns("Profit").DefaultCellStyle.Format = "N2"

                ' Role-based column locking
                Dim isOwner As Boolean = String.Equals(If(Module1.UserRole, ""), "owner", StringComparison.OrdinalIgnoreCase)
                If Not isOwner Then
                    If DataGridView3.Columns.Contains("Avg Cost") Then DataGridView3.Columns("Avg Cost").Visible = False
                    If DataGridView3.Columns.Contains("Total Cost") Then DataGridView3.Columns("Total Cost").Visible = False
                    If DataGridView3.Columns.Contains("Profit") Then DataGridView3.Columns("Profit").Visible = False
                Else
                    If DataGridView3.Columns.Contains("Avg Cost") Then DataGridView3.Columns("Avg Cost").Visible = True
                    If DataGridView3.Columns.Contains("Total Cost") Then DataGridView3.Columns("Total Cost").Visible = True
                    If DataGridView3.Columns.Contains("Profit") Then DataGridView3.Columns("Profit").Visible = True
                End If
            End If

            CalculateMonthlyItemTotals(dt, Not String.IsNullOrEmpty(searchInvNo))

            conn.Close()
        Catch ex As Exception
            MessageBox.Show("Error loading monthly item sales: " & ex.Message)
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub CalculateMonthlyItemTotals(dt As DataTable, isSearch As Boolean)
        Dim totCost As Decimal = 0
        Dim totPricePlusVat As Decimal = 0
        Dim totProfit As Decimal = 0
        Dim totSubtotal As Decimal = 0
        Dim totPaid As Decimal = 0
        Dim totBalance As Decimal = 0

        For Each row As DataRow In dt.Rows
            totCost += Convert.ToDecimal(row("Total Cost"))
            totPricePlusVat += Convert.ToDecimal(row("Price (+VAT)"))
            totProfit += Convert.ToDecimal(row("Profit"))
        Next

        If dt.Rows.Count > 0 Then
            ' Period totals are constant for all rows in this item-grouped view
            totSubtotal = Convert.ToDecimal(dt.Rows(0)("Subtotal_Sum"))
            totPaid = Convert.ToDecimal(dt.Rows(0)("Paid_Sum"))
            totBalance = Convert.ToDecimal(dt.Rows(0)("Balance_Sum"))
        End If

        ' --- ADD ADJUSTMENTS TO TOTALS ---
        If Not isSearch Then
            Try
                Dim sDate As String = DateTimePickerItemStart.Value.ToString("yyyy-MM-dd") & " 00:00:00"
                Dim eDate As String = DateTimePickerItemEnd.Value.ToString("yyyy-MM-dd") & " 23:59:59"
                Dim sqlAdj As String = "SELECT SUM(a.difference_amount) FROM sales_adjustments a JOIN billing b ON a.inv_no = b.inv_no WHERE a.adjustment_date >= @s AND a.adjustment_date <= @e AND (b.timestamps < @s OR b.timestamps > @e) " & (If(Module1.IsRgrVisible, "", " AND a.is_rgr = 0 AND a.inv_no NOT LIKE 'GR%' AND a.inv_no NOT LIKE 'RGR%' "))
                Using cmdAdj As New MySqlCommand(sqlAdj, conn)
                    If conn.State = ConnectionState.Closed Then conn.Open()
                    cmdAdj.Parameters.AddWithValue("@s", sDate)
                    cmdAdj.Parameters.AddWithValue("@e", eDate)
                    Dim adjRes = cmdAdj.ExecuteScalar()
                    If adjRes IsNot Nothing AndAlso Not IsDBNull(adjRes) Then
                        totPricePlusVat += Convert.ToDecimal(adjRes)
                        totSubtotal += Convert.ToDecimal(adjRes)
                        totPaid += Convert.ToDecimal(adjRes)
                    End If
                End Using
            Catch
            End Try
        End If

        LabelItemCost.Text = totCost.ToString("N2")
        LabelItemPrice.Text = totPricePlusVat.ToString("N2")
        LabelItemProfit.Text = totProfit.ToString("N2")

        LabelTotalPriceMI.Text = totSubtotal.ToString("N2")
        ' Show Paid Amount if Advance mode OR All mode is checked
        If CheckBoxAdvItem.Checked Or CheckBoxAllItem.Checked Then
            LabelPaidAmountMI.Text = totPaid.ToString("N2")
            LableBalanceDueMI.Text = totBalance.ToString("N2")
        Else
            LabelPaidAmountMI.Text = "0.00"
            LableBalanceDueMI.Text = "0.00"
        End If
    End Sub

    ' --- CASH FLOW LOGIC ---

    Private Sub LoadCashFlow()
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()

            ' 1. Fetch Cash In (Billing Paid + Customer Payments)
            Dim selDate As String = dtpCashFlow.Value.ToString("yyyy-MM-dd")

            Dim sqlCashIn = "SELECT id as RecordID, 'Sales Adjustment' as Category, inv_no as Reference, difference_amount as Amount, adjustment_date as Time, (CASE WHEN difference_amount > 0 THEN 'IN' ELSE 'OUT' END) as credit FROM sales_adjustments " &
                            "WHERE DATE(adjustment_date) = @dt " & (If(Module1.IsRgrVisible, "", " AND is_rgr = 0 AND inv_no NOT LIKE 'GR%' AND inv_no NOT LIKE 'RGR%' "))

            Dim dtIn As New DataTable()
            Dim cmdIn As New MySqlCommand(sqlCashIn, conn)
            cmdIn.Parameters.AddWithValue("@dt", selDate)
            Dim daIn As New MySqlDataAdapter(cmdIn)
            daIn.Fill(dtIn)

            ' 2. Fetch Petty Cash separately - auto-detect the text column name via INFORMATION_SCHEMA
            Try
                Dim sqlCol As String = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS " &
                                       "WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'petty_cash' " &
                                       "AND DATA_TYPE IN ('varchar','text','char','tinytext','mediumtext','longtext') " &
                                       "ORDER BY ORDINAL_POSITION LIMIT 1"
                Dim cmdCol As New MySqlCommand(sqlCol, conn)
                Dim textColName As String = ""
                Dim colResult = cmdCol.ExecuteScalar()
                If colResult IsNot Nothing AndAlso colResult IsNot DBNull.Value Then
                    textColName = colResult.ToString()
                    Dim pettyFilter As String = ""
                    If Not Module1.IsRgrVisible Then
                        pettyFilter = " AND `{0}` NOT LIKE '% GR%' AND `{0}` NOT LIKE '% RGR%' AND `{0}` NOT LIKE 'Customer Credit Pay%'"
                    End If

                    ' Aliasing transaction_type as credit and dynamically categorizing system logs
                    Dim sqlPetty As String = String.Format(
                        "SELECT id as RecordID, " &
                        "CASE " &
                        "  WHEN item_name LIKE 'Cash Sale%' THEN 'Sale' " &
                        "  WHEN item_name LIKE 'Purchase Payment%' THEN 'Purchase' " &
                        "  WHEN item_name LIKE 'Supplier Payment%' THEN 'Supplier Payment' " &
                        "  WHEN item_name LIKE 'Customer Credit Pay%' THEN 'Credit Payment' " &
                        "  WHEN item_name LIKE 'Cash Refund%' THEN 'Refund' " &
                        "  WHEN item_name LIKE 'Change Given%' THEN 'Change' " &
                        "  ELSE 'Petty Cash' " &
                        "END as Category, " &
                        "`{0}` as Reference, amount as Amount, date as Time, transaction_type as credit FROM petty_cash WHERE DATE(date) = @dt" & pettyFilter,
                        textColName)
                    Dim cmdPetty As New MySqlCommand(sqlPetty, conn)
                    cmdPetty.Parameters.AddWithValue("@dt", selDate)
                    Dim daPetty As New MySqlDataAdapter(cmdPetty)
                    daPetty.Fill(dtIn) ' Merge rows into same DataTable
                End If
            Catch exPetty As Exception
                ' Petty cash failed silently - main cash flow still displays
            End Try

            dgvCashFlow.DataSource = dtIn

            ' Format Grid
            If dgvCashFlow.Columns.Count > 0 Then
                If dgvCashFlow.Columns.Contains("RecordID") Then dgvCashFlow.Columns("RecordID").Visible = False
                dgvCashFlow.Columns("Category").Width = 250
                dgvCashFlow.Columns("Reference").Width = 250
                dgvCashFlow.Columns("Amount").Width = 180
                dgvCashFlow.Columns("Amount").DefaultCellStyle.Format = "N2"
                dgvCashFlow.Columns("Time").Width = 280
                dgvCashFlow.Columns("Time").DefaultCellStyle.Format = "HH:mm:ss"
            End If

            ' 3. Calculate Totals
            Dim totalIn As Decimal = 0
            Dim totalOut As Decimal = 0
            For Each r As DataRow In dtIn.Rows
                Dim amt As Decimal = If(IsDBNull(r("Amount")), 0, Math.Abs(Convert.ToDecimal(r("Amount"))))
                Dim type As String = If(r.Table.Columns.Contains("credit") AndAlso Not IsDBNull(r("credit")), r("credit").ToString().ToUpper(), "OUT")

                If type = "IN" Then
                    totalIn += amt
                Else
                    totalOut += amt
                End If
            Next

            lblCashIn.Text = totalIn.ToString("N2")
            lblCashOut.Text = totalOut.ToString("N2")
            lblNetBalance.Text = (totalIn - totalOut).ToString("N2")

            conn.Close()
        Catch ex As Exception
            MessageBox.Show("Cash Flow Error: " & ex.Message)
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub btnAddPettyCash_Click(sender As Object, e As EventArgs) Handles btnAddPettyCash.Click
        Dim pcForm As New PettyCashAdd()
        pcForm.ShowDialog()
        LoadCashFlow()
    End Sub

    Private Sub dgvCashFlow_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvCashFlow.CellDoubleClick
        If e.RowIndex >= 0 Then
            Dim category As String = dgvCashFlow.Rows(e.RowIndex).Cells("Category").Value.ToString()
            If category = "Petty Cash" Then
                Dim recordId As Integer = Convert.ToInt32(dgvCashFlow.Rows(e.RowIndex).Cells("RecordID").Value)
                Dim pcForm As New PettyCashAdd()
                pcForm.LoadForUpdate(recordId)
                If pcForm.ShowDialog() = DialogResult.OK Then
                    LoadCashFlow()
                End If
            End If
        End If
    End Sub

    Public Sub ApplySecurityLock()
        Dim billingTypes As String()
        If Not Module1.IsRgrVisible Then
            billingTypes = {"Cash", "Cash (Cash)", "Cash (Cards/Online)"}
        Else
            billingTypes = {"Cash", "Cash (Cash)", "Cash (Cards/Online)", "Credit", "Cheque", "Cash+Credit", "Cash+Cheque", "Mixed Payment", "Credit+Cheque"}
        End If

        ComboBoxBilling.Items.Clear()
        ComboBoxBilling.Items.AddRange(billingTypes)
        ComboBoxBilling.SelectedIndex = 0

        ComboBoxBillingTypeM.Items.Clear()
        ComboBoxBillingTypeM.Items.AddRange(billingTypes)
        ComboBoxBillingTypeM.SelectedIndex = 0

        ComboBoxBillingMI.Items.Clear()
        ComboBoxBillingMI.Items.AddRange(billingTypes)
        ComboBoxBillingMI.SelectedIndex = 0

        ' Role-based tab and column locking
        Dim isOwner As Boolean = String.Equals(If(Module1.UserRole, ""), "owner", StringComparison.OrdinalIgnoreCase)
        If Not isOwner Then
            ' Hide Daily Sales, Monthly Sales, and Cash Flow tabs by removing them
            If TabControl1.TabPages.Contains(TabPage1) Then TabControl1.TabPages.Remove(TabPage1)
            If TabControl1.TabPages.Contains(TabPage2) Then TabControl1.TabPages.Remove(TabPage2)
            If TabControl1.TabPages.Contains(TabPage4) Then TabControl1.TabPages.Remove(TabPage4)

            ' Hide the entire summary panel (prices, profits, cost, balance, paid amounts)
            GroupBoxItemSummary.Visible = False

            ' Hide Print button to prevent generating reports containing cost/profit details
            Button2.Visible = False
        Else
            ' Owner has full access: restore all tabs if they are missing
            If Not TabControl1.TabPages.Contains(TabPage1) Then TabControl1.TabPages.Insert(0, TabPage1)
            If Not TabControl1.TabPages.Contains(TabPage2) Then TabControl1.TabPages.Insert(1, TabPage2)
            If Not TabControl1.TabPages.Contains(TabPage4) Then TabControl1.TabPages.Add(TabPage4)

            ' Show the summary panel
            GroupBoxItemSummary.Visible = True

            ' Show Print button
            Button2.Visible = True
        End If

        RefreshCurrentTab()
    End Sub

    Private Sub ShowItemInvoices()
        If DataGridView3.CurrentRow IsNot Nothing Then
            Try
                If DataGridView3.Columns.Contains("Item Code") AndAlso DataGridView3.Columns.Contains("Description") Then
                    Dim itemCode As String = DataGridView3.CurrentRow.Cells("Item Code").Value.ToString()
                    Dim itemName As String = DataGridView3.CurrentRow.Cells("Description").Value.ToString()

                    Dim startDate As Date = DateTimePickerItemStart.Value.Date
                    Dim endDate As Date = DateTimePickerItemEnd.Value.Date
                    Dim filterInvType As String = If(ComboBoxItemInvType.SelectedItem IsNot Nothing, ComboBoxItemInvType.SelectedItem.ToString(), "Normal")
                    Dim filterBilling As String = If(ComboBoxBillingMI.SelectedItem IsNot Nothing, ComboBoxBillingMI.SelectedItem.ToString(), "Cash")
                    Dim isAdvance As Boolean = CheckBoxAdvItem.Checked
                    Dim allItem As Boolean = CheckBoxAllItem.Checked

                    Dim f As New ItemInvoicesForm(itemCode, itemName, startDate, endDate, filterInvType, filterBilling, isAdvance, allItem)
                    f.MdiParent = Me.MdiParent
                    f.Show()
                End If
            Catch ex As Exception
                MessageBox.Show("Error displaying invoices: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub DataGridView3_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView3.CellDoubleClick
        If e.RowIndex >= 0 Then
            ShowItemInvoices()
        End If
    End Sub

    Private Sub ViewInvoicesItem_Click(sender As Object, e As EventArgs)
        ShowItemInvoices()
    End Sub

    Private Sub Label34_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Label34_Click_1(sender As Object, e As EventArgs)

    End Sub

    Private Sub Label35_Click(sender As Object, e As EventArgs)

    End Sub
End Class
