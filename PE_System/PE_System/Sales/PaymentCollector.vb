Imports MySql.Data.MySqlClient

Partial Public Class PaymentCollector
    Inherits System.Windows.Forms.Form



    Private currentView As String = "PENDING"
    Private selectedOrigPaidAmt As Decimal = 0  ' Tracks original paid amount for return detection
    Private printerInv As SaleInv = Nothing     ' Shared printer form to avoid slow re-initialization

    'Printer configuration - Mirror TempSales
    'Printer configuration - Change these names to match your printer names in Windows
    'Private POSPrinterName As String = "\\pc2\BIXOLON SRP-E302"
    'Private StandardPrinterName As String = "\\pc2\EPSON L3210 Series"
    'Private POSPrinterName As String = "BIXOLON SRP-E302"
    'Private StandardPrinterName As String = "EPSON L3210 Series"

    'Main
    Private POSPrinterName As String = "\\192.168.100.3\POS-80"
    Private StandardPrinterName As String = "EPSON LQ-310 ESC/P2 (Copy 1)"

    'second Main
    'Private POSPrinterName As String = "\\192.168.100.3\POS-80"
    'Private StandardPrinterName As String = "\\192.168.100.1\epson lq-310 escp2"

    'PC1 
    'Private POSPrinterName As String = "POS-80"
    'Private StandardPrinterName As String = "\\192.168.100.1\epson lq-310 escp2"


    Private Sub PaymentCollector_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.KeyPreview = True ' Enable form to capture key strokes
        LoadBills("PENDING")
        cmbPayMethod.SelectedIndex = 0
        cmbType.SelectedIndex = 0
        cmbBillingType.SelectedIndex = 0
        chkChangeGiven.Visible = False
        UpdateTabButtons()
    End Sub

    Private Sub UpdateTabButtons()
        btnShowPending.BackColor = Color.Gray
        btnShowCollected.BackColor = Color.Gray
        btnShowReturns.BackColor = Color.Gray

        If currentView = "PENDING" Then
            btnShowPending.BackColor = Color.FromArgb(0, 122, 204)
            lblPendingCount.Text = "PENDING BILLS LIST"
        ElseIf currentView = "RETURNS_PENDING" Then
            btnShowReturns.BackColor = Color.FromArgb(180, 60, 0)
            lblPendingCount.Text = "PENDING RETURNS LIST"
        Else
            btnShowCollected.BackColor = Color.FromArgb(25, 135, 84)
            lblPendingCount.Text = "COLLECTED BILLS LIST"
        End If
    End Sub

    Private Sub btnShowPending_Click(sender As Object, e As EventArgs) Handles btnShowPending.Click
        currentView = "PENDING"
        LoadBills("PENDING")
        UpdateTabButtons()
        ClearUI()
    End Sub

    Private Sub btnShowCollected_Click(sender As Object, e As EventArgs) Handles btnShowCollected.Click
        currentView = "COLLECTED"
        LoadBills("COLLECTED")
        UpdateTabButtons()
        ClearUI()
    End Sub

    Private Sub btnShowReturns_Click(sender As Object, e As EventArgs) Handles btnShowReturns.Click
        currentView = "RETURNS_PENDING"
        LoadBills("RETURNS_PENDING")
        UpdateTabButtons()
        ClearUI()
    End Sub

    Private Sub LoadPendingBills()
        LoadBills(currentView)
    End Sub

    Private Sub LoadBills(status As String)
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()

            Dim sql As String
            If status = "RETURNS_PENDING" Then
                ' Identify Return Invoices: Bills that are PENDING in main table but ALREADY EXIST in history
                sql = "SELECT b.id, b.inv_no as 'Invoice Number', CONCAT(b.inv_type, ' Return') as 'Doc Type', b.grand_total as 'Total', " &
                      "b.billing_type as 'Billing Type', u.name as 'Order User', b.timestamps as 'Pending Time', " &
                      "IFNULL(c.name, 'No Customer') as 'Customer Name', b.cash_status, b.payment_type as 'payment_type', " &
                      "b.inv_type, b.paid_amount, b.our_discount, b.inv_discount, b.po_number, b.customer_id " &
                      "FROM billing b " &
                      "LEFT JOIN user u ON b.user_id = u.id " &
                      "LEFT JOIN customer c ON b.customer_id = c.id " &
                      "WHERE b.cash_status = 'PENDING' AND b.billing_type LIKE '%PENDING%' AND " &
                      "EXISTS (SELECT 1 FROM billing_history bh WHERE bh.billing_id = b.id) " &
                      "ORDER BY b.timestamps DESC"
            ElseIf status = "PENDING" Then
                ' Identify Normal Bills: Bills that are PENDING in main table and DO NOT exist in history
                sql = "SELECT b.id, b.inv_no as 'Invoice Number', b.inv_type as 'Doc Type', b.grand_total as 'Total', " &
                      "b.billing_type as 'Billing Type', " &
                      "u.name as 'Order User', b.timestamps as 'Pending Time', IFNULL(c.name, 'No Customer') as 'Customer Name', " &
                      "b.cash_status, b.payment_type as 'payment_type', b.inv_type, b.paid_amount, b.our_discount, b.inv_discount, b.po_number, b.customer_id " &
                      "FROM billing b " &
                      "LEFT JOIN user u ON b.user_id = u.id " &
                      "LEFT JOIN customer c ON b.customer_id = c.id " &
                      "WHERE b.cash_status = 'PENDING' AND b.billing_type LIKE '%PENDING%' AND " &
                      "NOT EXISTS (SELECT 1 FROM billing_history bh WHERE bh.billing_id = b.id) " &
                      "ORDER BY b.timestamps DESC"
            Else
                ' COLLECTED LIST: Normal optimization with history check for labelling
                sql = "SELECT b.id, b.inv_no as 'Invoice Number', " &
                      "CASE WHEN EXISTS (SELECT 1 FROM billing_history bh WHERE bh.billing_id = b.id) THEN CONCAT(b.inv_type, ' Return') ELSE b.inv_type END as 'Doc Type', " &
                      "b.grand_total as 'Total', " &
                      "CASE WHEN UPPER(TRIM(b.billing_type)) = 'PENDING' THEN 'Cash' ELSE b.billing_type END as 'Billing Type', " &
                      "CASE WHEN UPPER(TRIM(b.payment_type)) IN ('PENDING','') OR b.payment_type IS NULL THEN b.billing_type ELSE b.payment_type END as 'Payment Method', " &
                      "b.updated_at as 'Collected Time', IFNULL(c.name, 'No Customer') as 'Customer Name', " &
                      "b.cash_status, b.payment_type as 'payment_type', b.inv_type, b.paid_amount, b.our_discount, b.inv_discount, b.po_number, b.customer_id " &
                      "FROM billing b " &
                      "LEFT JOIN customer c ON b.customer_id = c.id " &
                      "WHERE TRIM(UPPER(b.cash_status)) = 'COLLECTED' " &
                      "ORDER BY b.updated_at DESC"
            End If

            Dim da As New MySqlDataAdapter(sql, conn)
            Dim dt As New DataTable()
            da.Fill(dt)
            dgvPending.DataSource = dt
            ' lblPendingCount.Text updated in UpdateTabButtons + records below

            ' Hide internal support columns used by code logic only
            If dgvPending.Columns.Contains("id") Then dgvPending.Columns("id").Visible = False
            If dgvPending.Columns.Contains("cash_status") Then dgvPending.Columns("cash_status").Visible = False
            If dgvPending.Columns.Contains("payment_type") Then dgvPending.Columns("payment_type").Visible = False
            If dgvPending.Columns.Contains("inv_type") Then dgvPending.Columns("inv_type").Visible = False
            If dgvPending.Columns.Contains("paid_amount") Then dgvPending.Columns("paid_amount").Visible = False
            If dgvPending.Columns.Contains("our_discount") Then dgvPending.Columns("our_discount").Visible = False
            If dgvPending.Columns.Contains("inv_discount") Then dgvPending.Columns("inv_discount").Visible = False
            If dgvPending.Columns.Contains("po_number") Then dgvPending.Columns("po_number").Visible = False
            If dgvPending.Columns.Contains("customer_id") Then dgvPending.Columns("customer_id").Visible = False

            ' Column widths for readability
            If dgvPending.Columns.Contains("Invoice Number") Then dgvPending.Columns("Invoice Number").Width = 120
            If dgvPending.Columns.Contains("Doc Type") Then dgvPending.Columns("Doc Type").Width = 120
            If dgvPending.Columns.Contains("Total") Then
                dgvPending.Columns("Total").DefaultCellStyle.Format = "N2"
                dgvPending.Columns("Total").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            End If

            conn.Close()
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then conn.Close()
            MessageBox.Show("Error loading bills: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub dgvPending_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles dgvPending.CellFormatting
        If e.RowIndex >= 0 Then
            Dim row = dgvPending.Rows(e.RowIndex)
            Dim status = If(row.Cells("cash_status").Value, "").ToString().ToUpper()

            If status = "PENDING" OrElse status = "INV_PENDING" Then
                e.CellStyle.BackColor = Color.FromArgb(255, 230, 80) ' Richer Dark Yellow
            ElseIf status = "COLLECTED" Then
                e.CellStyle.BackColor = Color.FromArgb(220, 255, 220) ' Light Green
            End If
        End If
    End Sub

    Private Sub dgvPending_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvPending.CellClick
        If e.RowIndex >= 0 Then
            Dim row = dgvPending.Rows(e.RowIndex)
            lblInvNo.Text = row.Cells("Invoice Number").Value.ToString()
            lblTotalAmount.Text = Convert.ToDecimal(row.Cells("Total").Value).ToString("N2")

            ' Set invoice type display
            Dim invType As String = If(row.Cells("inv_type").Value IsNot Nothing, row.Cells("inv_type").Value.ToString(), "Sale")
            lblInvoiceType.Text = invType.ToUpper() & " INVOICE"
            lblInvoiceType.Visible = True

            ' Color-code for easier identification
            If invType.ToLower() = "return" Then
                lblInvoiceType.ForeColor = Color.OrangeRed
            Else
                lblInvoiceType.ForeColor = Color.Lime
            End If

            Dim currentBillingId As String = row.Cells("id").Value.ToString()
            Dim origBillingType As String = row.Cells("Billing Type").Value.ToString()
            Dim origPaymentType As String = row.Cells("payment_type").Value.ToString()
            Dim origPaidAmt As Decimal = If(row.Cells("paid_amount").Value Is DBNull.Value, 0D, Convert.ToDecimal(row.Cells("paid_amount").Value))

            ' Query the pending_returns_data to see if there's a preserved original state
            Try
                Dim sqlOrig As String = "SELECT original_billing_type, original_payment_type, original_paid_amount FROM pending_returns_data WHERE billing_id = @id"
                Using connLocal As New MySqlConnection(Module1.ConnStr)
                    Using cmdOrig As New MySqlCommand(sqlOrig, connLocal)
                        cmdOrig.Parameters.AddWithValue("@id", currentBillingId)
                        connLocal.Open()
                        Using drOrig = cmdOrig.ExecuteReader()
                            If drOrig.Read() Then
                                origBillingType = If(drOrig("original_billing_type") Is DBNull.Value, origBillingType, drOrig("original_billing_type").ToString())
                                origPaymentType = If(drOrig("original_payment_type") Is DBNull.Value, origPaymentType, drOrig("original_payment_type").ToString())
                                origPaidAmt = If(drOrig("original_paid_amount") Is DBNull.Value, origPaidAmt, Convert.ToDecimal(drOrig("original_paid_amount")))
                            End If
                        End Using
                    End Using
                End Using
            Catch ex As Exception
                ' Silent fail, fallback to grid defaults
            End Try

            ' Store for use in btnCollect_Click to detect return invoices
            selectedOrigPaidAmt = origPaidAmt

            ' Set initial defaults from the pending bill (or its found original state)
            cmbBillingType.Text = origBillingType
            cmbPayMethod.Text = origPaymentType
            cmbType.Text = invType ' Sync the dropdown too

            Dim status = If(row.Cells("cash_status").Value, "").ToString().ToUpper()
            If status = "COLLECTED" Then
                btnCollect.Text = "RE-PRINT"
                btnCollect.BackColor = Color.Teal
            Else
                btnCollect.Text = "FINAL COLLECT"
                btnCollect.BackColor = Color.FromArgb(25, 135, 84)
            End If

            ' Load original values into processing panel
            txtCash.Text = origPaidAmt.ToString("N2")
            txtOurDiscount.Text = If(row.Cells("our_discount").Value Is DBNull.Value, "0", row.Cells("our_discount").Value.ToString())
            txtInvDiscount.Text = If(row.Cells("inv_discount").Value Is DBNull.Value, "0", row.Cells("inv_discount").Value.ToString())

            ' [NEW] Restrict processing if already COLLECTED
            Dim isCollected As Boolean = (status = "COLLECTED")
            txtCash.Enabled = Not isCollected
            txtOurDiscount.Enabled = Not isCollected
            txtInvDiscount.Enabled = Not isCollected
            cmbBillingType.Enabled = Not isCollected
            cmbPayMethod.Enabled = Not isCollected
            btnDelete.Enabled = Not isCollected
            ' chkChangeGiven.Enabled = Not isCollected ' Removed reference

            ' Load P/O if exists
            If dgvPending.Columns.Contains("po_number") Then
                p_num.Text = If(row.Cells("po_number").Value Is DBNull.Value, "", row.Cells("po_number").Value.ToString())
            End If

            CalculateTotals()
        End If
    End Sub

    Private Sub CalculateTotals()
        Try
            Dim total As Decimal = 0
            Dim ourDiscPerc As Decimal = 0
            Dim invDiscPerc As Decimal = 0
            Dim cash As Decimal = 0

            Decimal.TryParse(lblTotalAmount.Text, total)
            Decimal.TryParse(txtOurDiscount.Text, ourDiscPerc)
            Decimal.TryParse(txtInvDiscount.Text, invDiscPerc)
            Decimal.TryParse(txtCash.Text, cash)

            ' Deduction Logic: Total - (OurDisc%) - (InvDisc%)
            Dim ourDiscVal As Decimal = total * (ourDiscPerc / 100D)
            Dim invDiscVal As Decimal = total * (invDiscPerc / 100D)
            Dim grandTotal As Decimal = total - ourDiscVal - invDiscVal
            lblGrandTotal.Text = grandTotal.ToString("N2")

            Dim billingType As String = If(cmbBillingType.Text, "").Trim()
            Dim payMethod As String = If(cmbPayMethod.Text, "").Trim()

            ' --- Mirror TempSales: Card payments auto-fill cash amount ---
            Dim isCardPayment As Boolean = String.Equals(payMethod, "Debit Card", StringComparison.OrdinalIgnoreCase) OrElse
                                          String.Equals(payMethod, "Credit Card", StringComparison.OrdinalIgnoreCase)
            If isCardPayment Then
                txtCash.Text = grandTotal.ToString("N2")
                cash = grandTotal
            End If

            Dim changeAmt As Decimal = cash - grandTotal

            If String.Equals(billingType, "Credit", StringComparison.OrdinalIgnoreCase) Then
                ' Credit Billing: Show credit balance = Grand Total - Cash (TempSales exact)
                lblChangeAmount.Text = "0.00"
                Dim creditBal As Decimal = grandTotal - cash
                lblCreditBalance.Text = If(creditBal > 0, creditBal.ToString("N2"), "0.00")
                lblTotalBalance.Text = If(creditBal > 0, creditBal.ToString("N2"), "0.00")
            ElseIf String.Equals(billingType, "Cheque", StringComparison.OrdinalIgnoreCase) Then
                ' Cheque Billing: Same as credit - balance display (TempSales exact)
                lblChangeAmount.Text = "0.00"
                Dim chqBal As Decimal = grandTotal - cash
                lblCreditBalance.Text = If(chqBal > 0, chqBal.ToString("N2"), "0.00")
                lblTotalBalance.Text = If(chqBal > 0, chqBal.ToString("N2"), "0.00")
            Else
                ' Cash Billing: Change Amount display
                If changeAmt >= 0 Then
                    lblChangeAmount.Text = changeAmt.ToString("N2")
                    lblTotalBalance.Text = "0.00"
                    lblCreditBalance.Text = "0.00"
                Else
                    lblChangeAmount.Text = "0.00"
                    lblTotalBalance.Text = Math.Abs(changeAmt).ToString("N2")
                    lblCreditBalance.Text = Math.Abs(changeAmt).ToString("N2")
                End If
            End If

            ' Denomination breakdown always shows change
            Dim displayChangeAmt As Decimal = 0
            Decimal.TryParse(lblChangeAmount.Text, displayChangeAmt)
            ShowDenominationBreakdown(displayChangeAmt)

        Catch ex As Exception
        End Try
    End Sub

    Private Sub ShowDenominationBreakdown(amount As Decimal)
        Try
            Dim notes() As Integer = {5000, 2000, 1000, 500, 100, 50, 20, 10, 5, 2, 1}
            Dim suggestions As New List(Of String)()
            Dim usedDenoms As New Dictionary(Of Integer, Integer)()
            For Each d In notes : usedDenoms(d) = 0 : Next

            Dim remaining As Decimal = Math.Floor(amount)
            If remaining <= 0 Then
                lblBreakdown.Text = "-"
                lblDrawerStatus.Text = ""
                Return
            End If

            ' 1. Get current drawer stock
            Dim stock = Module1.GetCurrentDrawerStock()
            Dim stockInfo As New List(Of String)()
            ' Reverse sort by value for display
            Dim sortedKeys = stock.Keys.OrderByDescending(Function(k) k).ToList()
            For Each k In sortedKeys
                If stock(k) > 0 Then stockInfo.Add(k & ":" & stock(k))
            Next
            lblDrawerStatus.Text = "Opening Drawer Cash: " & String.Join(" | ", stockInfo)

            ' 2. Try to satisfy amount using available stock
            Dim tempRemaining = remaining
            For Each note In notes
                If tempRemaining >= note Then
                    Dim needed = tempRemaining \ note
                    Dim available = If(stock.ContainsKey(note), stock(note), 0)
                    
                    Dim take = Math.Min(needed, available)
                    If take > 0 Then
                        suggestions.Add(note & "x" & take)
                        usedDenoms(note) = take
                        tempRemaining -= (note * take)
                    End If
                End If
            Next

            ' 3. Display Suggestion and Check for insufficiency
            If tempRemaining > 0 Then
                lblBreakdown.ForeColor = Color.Red
                lblBreakdown.Text = "INSUFFICIENT! Need Rs." & tempRemaining & " more."
                If suggestions.Count > 0 Then
                    lblBreakdown.Text &= " (Drawer provides: " & String.Join(", ", suggestions) & ")"
                End If
            Else
                lblBreakdown.ForeColor = Color.Lime
                lblBreakdown.Text = "Suggest from Drawer:   " & String.Join("  |  ", suggestions)
            End If

            ' Store used denoms in tag for btnCollect if needed
            lblBreakdown.Tag = usedDenoms

        Catch ex As Exception
            lblBreakdown.Text = "Error calculating denominations"
        End Try
    End Sub

    Private Sub txtPayment_TextChanged(sender As Object, e As EventArgs) Handles txtOurDiscount.TextChanged, txtInvDiscount.TextChanged, txtCash.TextChanged
        CalculateTotals()
    End Sub

    Private Sub cmbPayMethod_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbPayMethod.SelectedIndexChanged
        CalculateTotals()
    End Sub

    Private Sub cmbBillingType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbBillingType.SelectedIndexChanged
        Dim billingType As String = If(cmbBillingType.Text, "").Trim()

        ' Show P/O controls for all billing types so it's optionally accessible
        Label2.Visible = True
        p_num.Visible = True

        If String.Equals(billingType, "Credit", StringComparison.OrdinalIgnoreCase) Then
            ' Credit billing → P.Method MUST be Credit only - lock it
            cmbPayMethod.Text = "Credit"
            cmbPayMethod.Enabled = False

        ElseIf String.Equals(billingType, "Cheque", StringComparison.OrdinalIgnoreCase) Then
            ' Cheque billing → P.Method MUST be Cheque only - lock it
            cmbPayMethod.Text = "Cheque"
            cmbPayMethod.Enabled = False

        ElseIf String.Equals(billingType, "Cash", StringComparison.OrdinalIgnoreCase) Then
            ' Cash billing → Cheque and Credit are FORBIDDEN
            cmbPayMethod.Enabled = True
            Dim curr As String = If(cmbPayMethod.Text, "").Trim()
            If String.Equals(curr, "Cheque", StringComparison.OrdinalIgnoreCase) OrElse
               String.Equals(curr, "Credit", StringComparison.OrdinalIgnoreCase) Then
                cmbPayMethod.Text = "Cash"
            End If

        Else
            ' PENDING or other → re-enable, leave as-is
            cmbPayMethod.Enabled = True
        End If

        CalculateTotals()
    End Sub

    Private Sub btnCollect_Click(sender As Object, e As EventArgs) Handles btnCollect.Click
        If dgvPending.CurrentRow Is Nothing Then Return

        ' Handle Re-print if already collected
        If btnCollect.Text = "RE-PRINT" Then
            PrintBill(lblInvNo.Text.Trim(), "", True)
            Return
        End If

        Dim billingId = dgvPending.CurrentRow.Cells("id").Value.ToString()
        Dim grandTotal As Decimal = 0
        Dim cash As Decimal = 0
        Dim balance As Decimal = 0

        Decimal.TryParse(lblGrandTotal.Text.Replace(",", ""), grandTotal)
        Decimal.TryParse(txtCash.Text.Replace(",", ""), cash)
        Decimal.TryParse(lblTotalBalance.Text, balance)

        Dim pType As String = cmbPayMethod.Text
        Dim bType As String = cmbBillingType.Text

        ' Self-healing for historical corrupted PENDING states 
        If String.Equals(pType, "PENDING", StringComparison.OrdinalIgnoreCase) Then
            If String.Equals(bType, "Cash", StringComparison.OrdinalIgnoreCase) Then
                pType = "Cash"
            ElseIf String.Equals(bType, "Credit", StringComparison.OrdinalIgnoreCase) Then
                pType = "Credit"
            ElseIf String.Equals(bType, "Cheque", StringComparison.OrdinalIgnoreCase) Then
                pType = "Cheque"
            End If
        End If

        ' -----------------------------------------------------
        ' RETURN INVOICE: Show RefundSettlementDialog before collection
        ' If original paid amount > current grand total, customer has items to return
        ' and cashier must confirm how the refund will be handled.
        ' -----------------------------------------------------
        Dim currentGrandTotal As Decimal = 0
        Decimal.TryParse(lblGrandTotal.Text, currentGrandTotal)
        If selectedOrigPaidAmt > currentGrandTotal AndAlso selectedOrigPaidAmt > 0 Then
            Dim refundDlg As New RefundSettlementDialog()
            refundDlg.TotalRefundDue = selectedOrigPaidAmt - currentGrandTotal
            refundDlg.UnpaidCredit = 0
            refundDlg.UnclearedChequeAmount = 0
            If refundDlg.ShowDialog() <> DialogResult.OK Then
                Return  ' Cashier cancelled the refund confirmation
            End If
        End If

        ' -----------------------------------------------------
        ' CHANGE ACTION (Overpayment / Advance / Settlement)
        ' -----------------------------------------------------
        Dim changeAmt As Decimal = cash - grandTotal
        Dim selectedChangeAction As ChangeActionDialog.ChangeAction = ChangeActionDialog.ChangeAction.CashReturn
        Dim customerIdStr As String = ""
        If dgvPending.Columns.Contains("customer_id") AndAlso dgvPending.CurrentRow.Cells("customer_id").Value IsNot DBNull.Value Then
            customerIdStr = dgvPending.CurrentRow.Cells("customer_id").Value.ToString()
        End If

        If changeAmt > 0 AndAlso (String.Equals(bType, "Cash", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(pType, "Cash", StringComparison.OrdinalIgnoreCase)) Then
            Dim hasCredit As Boolean = False
            If Not String.IsNullOrEmpty(customerIdStr) Then
                Try
                    Using connCheck As New MySqlConnection(Module1.ConnStr)
                        connCheck.Open()
                        Using cmdCheck1 As New MySqlCommand("SELECT SUM(credit_balance_due) FROM billing WHERE customer_id=@cid AND status IN ('Credit','Cash_Credit','Mixed_Payment','Credit_Cheque') AND credit_balance_due > 0", connCheck)
                            cmdCheck1.Parameters.AddWithValue("@cid", customerIdStr)
                            Dim credRes1 = cmdCheck1.ExecuteScalar()
                            If credRes1 IsNot DBNull.Value AndAlso Convert.ToDecimal(credRes1) > 0 Then
                                hasCredit = True
                            End If
                        End Using
                        Using cmdCheck2 As New MySqlCommand("SELECT SUM(amount) FROM customer_credit WHERE customer_id=@cid AND amount > 0 AND is_active=1", connCheck)
                            cmdCheck2.Parameters.AddWithValue("@cid", customerIdStr)
                            Dim credRes2 = cmdCheck2.ExecuteScalar()
                            If credRes2 IsNot DBNull.Value AndAlso Convert.ToDecimal(credRes2) > 0 Then
                                hasCredit = True
                            End If
                        End Using
                    End Using
                Catch
                End Try
            End If

            Dim dlg As New ChangeActionDialog(changeAmt, hasCredit)
            If Not String.IsNullOrEmpty(customerIdStr) Then
                If dlg.ShowDialog() = DialogResult.OK Then
                    selectedChangeAction = dlg.SelectedAction
                Else
                    Return ' Cancel collection
                End If
            Else
                selectedChangeAction = ChangeActionDialog.ChangeAction.CashReturn
            End If
        End If

        ' -----------------------------------------------------
        ' EXACT TEMPSALES LOGIC FOR BILLING TYPES
        ' -----------------------------------------------------
        ' Cash Logic Validation
        If String.Equals(bType, "Cash", StringComparison.OrdinalIgnoreCase) Then
            If cash < grandTotal Then
                MessageBox.Show("Cannot proceed. Cash collection is less than the Grand Total. Please select Credit or Partial billing type if payment is incomplete.", "Payment Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtCash.Focus()
                Return
            End If
        End If

        ' Cheque / Credit Dialog Flow — mirrors TempSales exactly
        Dim finalChequeNo As String = ""
        Dim finalBankId As Object = DBNull.Value
        Dim finalChequeAmount As Decimal = 0
        Dim remainingBalance As Decimal = grandTotal - cash

        If String.Equals(bType, "Credit", StringComparison.OrdinalIgnoreCase) Then
            ' P/O Validation for Credit Bills (Using p_num) - Disabled as per user request
            ' If String.IsNullOrWhiteSpace(p_num.Text) OrElse p_num.Text.Trim().ToLower() = "not" Then
            '     MessageBox.Show("P/O Number is mandatory for Credit bills. Please enter a valid P/O number before final collect.", "PO Number Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '     p_num.Focus()
            '     Return
            ' End If

            ' --- TempSales Credit Logic ---
            If remainingBalance > 0 Then
                ' Show SettlementDialog: "Cheque" or "Credit"?
                Dim settleDlg As New SettlementDialog()
                If settleDlg.ShowDialog() <> DialogResult.OK Then
                    Return  ' User cancelled
                End If

                If settleDlg.SelectedSettlement = "Cheque" Then
                    ' Settle with Cheque
                    Dim chqDlg As New ChequeEntryDialog()
                    chqDlg.DefaultAmount = remainingBalance
                    If chqDlg.ShowDialog() <> DialogResult.OK Then Return
                    finalChequeNo = chqDlg.ChequeNo
                    finalBankId = chqDlg.BankID
                    finalChequeAmount = chqDlg.ChequeAmount
                    pType = "Cheque"  ' Override payment type to Cheque
                Else
                    ' Pure credit — pType stays "Credit"
                    pType = "Credit"
                End If
            End If
            ' If remainingBalance <= 0, full cash payment → pType stays as-is

        ElseIf String.Equals(bType, "Cheque", StringComparison.OrdinalIgnoreCase) OrElse
               String.Equals(pType, "Cheque", StringComparison.OrdinalIgnoreCase) Then
            ' --- Cheque Billing Direct Entry ---
            Dim chqDlg As New ChequeEntryDialog()
            chqDlg.DefaultAmount = remainingBalance
            chqDlg.LockAmount = True
            If chqDlg.ShowDialog() <> DialogResult.OK Then
                MessageBox.Show("Cheque collection was cancelled. Payment halted.", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            finalChequeNo = chqDlg.ChequeNo
            finalBankId = chqDlg.BankID
            finalChequeAmount = chqDlg.ChequeAmount
        End If



        ' Change Confirmation Check removed per user request

        Try
            If conn.State = ConnectionState.Closed Then conn.Open()

            Dim finalStatus As String = "Paid"
            Dim collectorUID As Object = Module1.CurrentUserID
            Dim currentCashStatus As String = "COLLECTED"

            ' Triple Split Balance Columns
            Dim chequeBalanceDue As Decimal = 0
            Dim creditBalanceDue As Decimal = 0
            Dim partialCash As Decimal = 0

            ' Determine Status mirroring TempSales waterfall
            If remainingBalance <= 0 Then
                finalStatus = "Paid"
                partialCash = grandTotal
            Else
                If String.Equals(pType, "Cheque", StringComparison.OrdinalIgnoreCase) Then
                    If finalChequeAmount < remainingBalance Then
                        finalStatus = If(cash > 0, "Mixed_Payment", "Credit_Cheque")
                    Else
                        finalStatus = If(cash > 0, "cash_Cheque", "Cheque")
                    End If
                ElseIf String.Equals(pType, "Credit", StringComparison.OrdinalIgnoreCase) Then
                    finalStatus = If(cash > 0, "cash_Credit", "Credit")
                ElseIf String.Equals(pType, "Partial", StringComparison.OrdinalIgnoreCase) Then
                    finalStatus = "Partial" ' Legacy support
                End If

                ' Calculate Split Values (Triple Split Mapping)
                If finalStatus = "Cheque" OrElse finalStatus = "cash_Cheque" Then
                    partialCash = cash
                    chequeBalanceDue = remainingBalance
                    creditBalanceDue = 0
                ElseIf finalStatus = "Credit" OrElse finalStatus = "cash_Credit" Then
                    partialCash = cash
                    chequeBalanceDue = 0
                    creditBalanceDue = remainingBalance
                Else
                    ' Mixed_Payment, Credit_Cheque, or Partial
                    partialCash = cash
                    chequeBalanceDue = finalChequeAmount
                    creditBalanceDue = Math.Max(0, grandTotal - cash - finalChequeAmount)
                End If
            End If

            ' Self-healing Billing Type based on Status (User requested rule)
            If finalStatus = "Paid" Then
                bType = "Cash"
            ElseIf finalStatus = "Credit" OrElse finalStatus = "cash_Credit" OrElse finalStatus = "Mixed_Payment" OrElse finalStatus = "Credit_Cheque" Then
                bType = "Credit"
            ElseIf finalStatus = "Cheque" OrElse finalStatus = "cash_Cheque" Then
                bType = "Cheque"
            End If

            ' Database update - billing table columns only
            Dim sql = "UPDATE billing SET " &
                      "cash_status = @c_status, " &
                      "status = @status, " &
                      "payment_type = @p_type, " &
                      "billing_type = @b_type, " &
                      "collector_user_id = @coll_uid, " &
                      "grand_total = @gt, " &
                      "paid_amount = @paid, " &
                      "cash_received = @received, " &
                      "change_amount = @change, " &
                      "balance_due = @bal, " &
                      "credit_balance_due = @cbd, " &
                      "cheque_balance_due = @chq_bal, " &
                      "partial_cash = @p_cash, " &
                      "our_discount = @od, " &
                      "inv_discount = @id, " &
                      "cheque_no = @chq, " &
                      "bank_id = @bank, " &
                      "po_number = @po, " &
                      "change_action = @ca, " &
                      "updated_at = @now " &
                      "WHERE id = @bid"

            Using cmd = New MySqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@c_status", currentCashStatus)
                cmd.Parameters.AddWithValue("@status", finalStatus)
                cmd.Parameters.AddWithValue("@p_type", pType)
                cmd.Parameters.AddWithValue("@b_type", bType)
                cmd.Parameters.AddWithValue("@coll_uid", collectorUID)
                cmd.Parameters.AddWithValue("@gt", grandTotal)
                cmd.Parameters.AddWithValue("@paid", If(finalStatus = "Paid", grandTotal, cash))
                cmd.Parameters.AddWithValue("@received", cash)
                cmd.Parameters.AddWithValue("@change", If(cash > grandTotal, cash - grandTotal, 0))
                cmd.Parameters.AddWithValue("@bal", Math.Max(0, remainingBalance))
                cmd.Parameters.AddWithValue("@cbd", creditBalanceDue)
                cmd.Parameters.AddWithValue("@chq_bal", chequeBalanceDue)
                cmd.Parameters.AddWithValue("@p_cash", partialCash)
                cmd.Parameters.AddWithValue("@od", Val(txtOurDiscount.Text))
                cmd.Parameters.AddWithValue("@id", Val(txtInvDiscount.Text))
                cmd.Parameters.AddWithValue("@chq", If(chequeBalanceDue > 0 AndAlso Not String.IsNullOrEmpty(finalChequeNo), finalChequeNo, DBNull.Value))
                cmd.Parameters.AddWithValue("@bank", If(chequeBalanceDue > 0 AndAlso finalBankId IsNot DBNull.Value, finalBankId, DBNull.Value))
                cmd.Parameters.AddWithValue("@po", If(String.IsNullOrWhiteSpace(p_num.Text), "not", p_num.Text.Trim()))
                cmd.Parameters.AddWithValue("@ca", selectedChangeAction.ToString())
                cmd.Parameters.AddWithValue("@now", DateTime.Now)
                cmd.Parameters.AddWithValue("@bid", billingId)
                cmd.ExecuteNonQuery()
            End Using

            ' Clean up the pending tracking record now that it's finalized
            Try
                Dim sqlDel As String = "DELETE FROM pending_returns_data WHERE billing_id = @bid"
                Using cmdDel As New MySqlCommand(sqlDel, conn)
                    cmdDel.Parameters.AddWithValue("@bid", billingId)
                    cmdDel.ExecuteNonQuery()
                End Using
            Catch ex As Exception
                ' Silent execution
            End Try

            ' Database updates and deletes completed
            conn.Close()

            ' --- AUTO SYNC TO CUSTOMER CREDIT TABLE ---
            If Not String.IsNullOrEmpty(customerIdStr) AndAlso customerIdStr <> "1" Then
                Try
                    Using connSync As New MySqlConnection(Module1.ConnStr)
                        connSync.Open()
                        Dim syncSql As String = ""
                        Dim existingCreditId As Integer = 0
                        Dim invNo As String = lblInvNo.Text.Trim()

                        ' Check if a credit record already exists for this invoice
                        Dim checkCreditSql As String = "SELECT id FROM customer_credit WHERE inv_no = @inv AND customer_id = @cid LIMIT 1"
                        Using cmdCheck As New MySqlCommand(checkCreditSql, connSync)
                            cmdCheck.Parameters.AddWithValue("@inv", invNo)
                            cmdCheck.Parameters.AddWithValue("@cid", customerIdStr)
                            Dim res = cmdCheck.ExecuteScalar()
                            If res IsNot Nothing AndAlso res IsNot DBNull.Value Then
                                existingCreditId = Convert.ToInt32(res)
                            End If
                        End Using

                        If creditBalanceDue > 0 Then
                            If existingCreditId > 0 Then
                                ' Update existing record
                                syncSql = "UPDATE customer_credit SET amount = @amt, timestamps = @now, is_active = 1 WHERE id = @id"
                            Else
                                ' Insert new record
                                syncSql = "INSERT INTO customer_credit (amount, customer_id, inv_no, timestamps, is_active) VALUES (@amt, @cid, @inv, @now, 1)"
                            End If
                        Else
                            ' If balance is now 0 but a record exists, mark it as inactive/settled
                            If existingCreditId > 0 Then
                                syncSql = "UPDATE customer_credit SET amount = 0, is_active = 0 WHERE id = @id"
                            End If
                        End If

                        If Not String.IsNullOrEmpty(syncSql) Then
                            Using cmdSync As New MySqlCommand(syncSql, connSync)
                                cmdSync.Parameters.AddWithValue("@amt", creditBalanceDue)
                                cmdSync.Parameters.AddWithValue("@cid", customerIdStr)
                                cmdSync.Parameters.AddWithValue("@inv", invNo)
                                cmdSync.Parameters.AddWithValue("@now", DateTime.Now)
                                cmdSync.Parameters.AddWithValue("@id", existingCreditId)
                                cmdSync.ExecuteNonQuery()
                            End Using
                        End If
                    End Using
                Catch ex As Exception
                    ' Silent fail to ensure main transaction completion isn't blocked
                End Try
            End If

            ' -----------------------------------------------------
            ' APPLY CHANGE ACTION (Wallet or Settle Credit)
            ' -----------------------------------------------------
            If selectedChangeAction = ChangeActionDialog.ChangeAction.AddToAdvance Then
                Try
                    Using connAction As New MySqlConnection(Module1.ConnStr)
                        connAction.Open()
                        Dim sqlNote As String = "INSERT INTO customer_credit_notes (customer_id, inv_no, credit_amount, issue_date, status) VALUES (@cid, @inv, @amt, @now, 'active')"
                        Using cmdNote As New MySqlCommand(sqlNote, connAction)
                            cmdNote.Parameters.AddWithValue("@cid", customerIdStr)
                            cmdNote.Parameters.AddWithValue("@inv", lblInvNo.Text)
                            cmdNote.Parameters.AddWithValue("@amt", changeAmt)
                            cmdNote.Parameters.AddWithValue("@now", DateTime.Now)
                            cmdNote.ExecuteNonQuery()
                        End Using
                    End Using
                Catch
                End Try
            ElseIf selectedChangeAction = ChangeActionDialog.ChangeAction.SettlePreviousCredit Then
                Try
                    Using connAction As New MySqlConnection(Module1.ConnStr)
                        connAction.Open()
                        Dim sqlPay As String = "INSERT INTO customer_payments (CusID, Customer, PaymentType, Amount, Date, inv_no) VALUES (@cid, @cname, 'Cash', @amt, @now, @inv)"
                        Dim cname As String = If(dgvPending.CurrentRow.Cells("Customer Name").Value, "").ToString()
                        Using cmdPay As New MySqlCommand(sqlPay, connAction)
                            cmdPay.Parameters.AddWithValue("@cid", customerIdStr)
                            cmdPay.Parameters.AddWithValue("@cname", cname)
                            cmdPay.Parameters.AddWithValue("@amt", changeAmt)
                            cmdPay.Parameters.AddWithValue("@now", DateTime.Now)
                            cmdPay.Parameters.AddWithValue("@inv", "Settled by " & lblInvNo.Text)
                            cmdPay.ExecuteNonQuery()
                        End Using
                        
                        Dim remainingSettle As Decimal = changeAmt
                        
                        Dim sqlOldCred As String = "SELECT id, amount FROM customer_credit WHERE customer_id=@cid AND is_active=1 AND amount > 0 ORDER BY timestamps ASC"
                        Dim dtOldCred As New DataTable()
                        Using cmdOldCred As New MySqlCommand(sqlOldCred, connAction)
                            cmdOldCred.Parameters.AddWithValue("@cid", customerIdStr)
                            Using da As New MySqlDataAdapter(cmdOldCred)
                                da.Fill(dtOldCred)
                            End Using
                        End Using
                        For Each row As DataRow In dtOldCred.Rows
                            If remainingSettle <= 0 Then Exit For
                            Dim oldId As Integer = Convert.ToInt32(row("id"))
                            Dim oldBal As Decimal = Convert.ToDecimal(row("amount"))
                            Dim amountToDeduct As Decimal = Math.Min(oldBal, remainingSettle)
                            Dim sqlUpd As String = "UPDATE customer_credit SET amount = amount - @deduct WHERE id=@id"
                            Using cmdUpd As New MySqlCommand(sqlUpd, connAction)
                                cmdUpd.Parameters.AddWithValue("@deduct", amountToDeduct)
                                cmdUpd.Parameters.AddWithValue("@id", oldId)
                                cmdUpd.ExecuteNonQuery()
                            End Using
                            remainingSettle -= amountToDeduct
                        Next
                        
                        If remainingSettle > 0 Then
                            Dim sqlOldBill As String = "SELECT id, credit_balance_due FROM billing WHERE customer_id=@cid AND status IN ('Credit','Cash_Credit','Mixed_Payment','Credit_Cheque') AND credit_balance_due > 0 ORDER BY timestamps ASC"
                            Dim dtOldBill As New DataTable()
                            Using cmdOldBill As New MySqlCommand(sqlOldBill, connAction)
                                cmdOldBill.Parameters.AddWithValue("@cid", customerIdStr)
                                Using da As New MySqlDataAdapter(cmdOldBill)
                                    da.Fill(dtOldBill)
                                End Using
                            End Using
                            For Each row As DataRow In dtOldBill.Rows
                                If remainingSettle <= 0 Then Exit For
                                Dim oldId As Integer = Convert.ToInt32(row("id"))
                                Dim oldBal As Decimal = Convert.ToDecimal(row("credit_balance_due"))
                                Dim amountToDeduct As Decimal = Math.Min(oldBal, remainingSettle)
                                Dim sqlUpd As String = "UPDATE billing SET credit_balance_due = credit_balance_due - @deduct WHERE id=@id"
                                Using cmdUpd As New MySqlCommand(sqlUpd, connAction)
                                    cmdUpd.Parameters.AddWithValue("@deduct", amountToDeduct)
                                    cmdUpd.Parameters.AddWithValue("@id", oldId)
                                    cmdUpd.ExecuteNonQuery()
                                End Using
                                remainingSettle -= amountToDeduct
                            Next
                        End If
                    End Using
                Catch
                End Try
            End If


            ' -----------------------------------------------------
            ' LOG CASH TRANSACTION (Unified Cash Log)
            ' -----------------------------------------------------
            ' Condition loosened: Log if cash was received OR if it's a refund situation
            If cash > 0 OrElse selectedOrigPaidAmt > grandTotal Then
                ' 1. Log the Primary Transaction (Sale or Refund)
                Dim cName As String = ""
                If dgvPending.CurrentRow IsNot Nothing AndAlso dgvPending.CurrentRow.Cells("Customer Name").Value IsNot Nothing Then
                    cName = dgvPending.CurrentRow.Cells("Customer Name").Value.ToString().Trim()
                End If

                Dim isRealCustomer As Boolean = Not String.IsNullOrEmpty(cName) AndAlso
                                                Not String.Equals(cName, "No Customer", StringComparison.OrdinalIgnoreCase) AndAlso
                                                Not String.Equals(cName, "Cash", StringComparison.OrdinalIgnoreCase) AndAlso
                                                Not String.Equals(cName, "CASH", StringComparison.OrdinalIgnoreCase) AndAlso
                                                Not String.Equals(cName, "Cash Customer", StringComparison.OrdinalIgnoreCase)

                Dim transType As String = "IN"
                Dim transAmt As Decimal = 0
                Dim transMsg As String = If(isRealCustomer, "Cash Sale: " & cName & " (Inv: " & lblInvNo.Text & ")", "Cash Sale: " & lblInvNo.Text)

                If selectedOrigPaidAmt > grandTotal Then
                    ' Refund given to customer (This is an OUT)
                    transType = "OUT"
                    transAmt = selectedOrigPaidAmt - grandTotal
                    transMsg = If(isRealCustomer, "Cash Refund: " & cName & " (Inv: " & lblInvNo.Text & ")", "Cash Refund: " & lblInvNo.Text)
                    Dim refundDenoms = TryCast(lblBreakdown.Tag, Dictionary(Of Integer, Integer))
                    Module1.RegisterCashTransaction(transAmt, transType, transMsg, lblInvNo.Text, refundDenoms)
                Else
                    ' Cash collected from customer (This is an IN)
                    ' transAmt = new cash received today (Total cash - what was already paid)
                    transType = "IN"
                    transAmt = cash - If(selectedOrigPaidAmt > 0, selectedOrigPaidAmt, 0)
                    transMsg = If(isRealCustomer, "Cash Sale: " & cName & " (Inv: " & lblInvNo.Text & ")", "Cash Sale: " & lblInvNo.Text)
                    If transAmt > 0 Then
                        Module1.RegisterCashTransaction(transAmt, transType, transMsg, lblInvNo.Text, Nothing)
                    End If
                    
                    ' 2. Log Change Given back to customer (Separate OUT transaction to sync denoms)
                    Dim changeValue As Decimal = 0
                    Decimal.TryParse(lblChangeAmount.Text, changeValue)
                    If changeValue > 0 AndAlso selectedChangeAction = ChangeActionDialog.ChangeAction.CashReturn Then
                        Dim changeDenoms = TryCast(lblBreakdown.Tag, Dictionary(Of Integer, Integer))
                        Dim changeMsg As String = If(isRealCustomer, "Change Given: " & cName & " (Inv: " & lblInvNo.Text & ")", "Change Given: " & lblInvNo.Text)
                        Module1.RegisterCashTransaction(changeValue, "OUT", changeMsg, lblInvNo.Text, changeDenoms)
                    ElseIf changeValue > 0 Then
                        ' They kept the change as Advance or Settlement, money stays in drawer. No OUT transaction.
                    End If
                End If
            End If

            ' -----------------------------------------------------
            ' PRINTING: Show final invoice using SaleInv
            ' -----------------------------------------------------
            PrintBill(lblInvNo.Text.Trim(), "Payment Finalized Successfully!")

            ClearUI()
            LoadPendingBills()
        Catch ex As Exception
            MessageBox.Show("Process Error: " & ex.Message)
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If dgvPending.CurrentRow Is Nothing OrElse lblInvNo.Text = "-" Then
            MessageBox.Show("Please select an invoice to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim invoiceNo As String = lblInvNo.Text
        Dim billingId As String = dgvPending.CurrentRow.Cells("id").Value.ToString()
        Dim invType As String = dgvPending.CurrentRow.Cells("inv_type").Value.ToString()

        Dim result = MessageBox.Show("Are you sure you want to DELETE this invoice (" & invoiceNo & ")? This will revert any deducted stock.", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
        If result <> DialogResult.Yes Then Return

        Try
            If conn.State = ConnectionState.Closed Then conn.Open()

            ' 1. Revert Stock (only if it's NOT a Quote)
            If invType.ToLower() <> "quote" Then
                Dim itemsDt As New DataTable()
                Using cmdFetch = New MySqlCommand("SELECT item_id, quantity FROM billing_item WHERE billing_id = @bid", conn)
                    cmdFetch.Parameters.AddWithValue("@bid", billingId)
                    Using da = New MySqlDataAdapter(cmdFetch)
                        da.Fill(itemsDt)
                    End Using
                End Using

                For Each row As DataRow In itemsDt.Rows
                    Dim itemId = row("item_id").ToString()
                    Dim qty = Convert.ToDecimal(row("quantity"))

                    ' Update main items table
                    Using cmdStock = New MySqlCommand("UPDATE items SET st_qty = st_qty + @qty WHERE id = @id", conn)
                        cmdStock.Parameters.AddWithValue("@qty", qty)
                        cmdStock.Parameters.AddWithValue("@id", itemId)
                        cmdStock.ExecuteNonQuery()
                    End Using

                    ' Update items_stock (Batch stock)
                    ' We revert to the latest batch or a designated main stock batch
                    Dim batchSql = "UPDATE items_stock SET st_qty = st_qty + @qty WHERE item_id = @id AND location_id = (SELECT id FROM location WHERE location_name='MAIN STOCK' LIMIT 1) ORDER BY date DESC LIMIT 1"
                    Using cmdBatch = New MySqlCommand(batchSql, conn)
                        cmdBatch.Parameters.AddWithValue("@qty", qty)
                        cmdBatch.Parameters.AddWithValue("@id", itemId)
                        If cmdBatch.ExecuteNonQuery() = 0 Then
                            ' Fallback: If no batch found, just create one
                            Dim insSql = "INSERT INTO items_stock (item_id, st_qty, date, location_id, supplier_id) VALUES (@id, @qty, @now, (SELECT id FROM location WHERE location_name='MAIN STOCK' LIMIT 1), 1)"
                            Using cmdIns = New MySqlCommand(insSql, conn)
                                cmdIns.Parameters.AddWithValue("@id", itemId)
                                cmdIns.Parameters.AddWithValue("@qty", qty)
                                cmdIns.Parameters.AddWithValue("@now", DateTime.Now)
                                cmdIns.ExecuteNonQuery()
                            End Using
                        End If
                    End Using
                Next
            End If

            ' 2. Delete Details
            Using cmdDelItems = New MySqlCommand("DELETE FROM billing_item WHERE billing_id = @bid", conn)
                cmdDelItems.Parameters.AddWithValue("@bid", billingId)
                cmdDelItems.ExecuteNonQuery()
            End Using

            ' 3. Delete Header
            Using cmdDelHead = New MySqlCommand("DELETE FROM billing WHERE id = @bid", conn)
                cmdDelHead.Parameters.AddWithValue("@bid", billingId)
                cmdDelHead.ExecuteNonQuery()
            End Using

            ' Centralized System log deletion
            Module1.LogDeletion("Invoice", billingId, "Invoice No: " & invoiceNo & ", Type: " & invType)

            MessageBox.Show("Invoice " & invoiceNo & " has been deleted and stock reverted.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information)
            ClearUI()
            LoadPendingBills()
            conn.Close()
        Catch ex As Exception
            MessageBox.Show("Error deleting invoice: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub ClearUI()
        lblInvNo.Text = "-"
        lblTotalAmount.Text = "0.00"
        lblGrandTotal.Text = "0.00"
        txtOurDiscount.Text = "0"
        txtInvDiscount.Text = "0"
        txtCash.Text = ""
        lblChangeAmount.Text = "0.00"
        lblCreditBalance.Text = "0.00"
        lblTotalBalance.Text = "0.00"
        p_num.Text = ""
        Label2.Visible = False
        p_num.Visible = False
        lblBreakdown.Text = "-"
        lblInvoiceType.Visible = False
        ' chkChangeGiven.Checked = False ' Removed reference
        cmbPayMethod.SelectedIndex = 0
    End Sub

    Private Sub PrintBill(invNo As String, Optional successMsg As String = "", Optional ByVal isDuplicatePrint As Boolean = False)
        If String.IsNullOrEmpty(invNo) OrElse invNo = "-" Then Return

        ' Detection: Check if lblInvoiceType contains "RETURN"
        Dim isTrueReturn As Boolean = lblInvoiceType.Text.ToUpper().Contains("RETURN")

        Try
            ' 1. Show Preview form first (Standard A4 layout by default)
            Using rptForm As New SaleInv()
                ' Use Standard A4 (1) or Quotation A4 (4) for preview
                Dim previewIndex As Integer = If(invNo.StartsWith("QT"), 4, 1)
                rptForm.ShowReport(invNo, previewIndex, False, isTrueReturn, "", 1, 0, isDuplicatePrint)

                ' Ensure the form paints before the modal MessageBox blocks the thread
                rptForm.Refresh()
                Application.DoEvents()

                ' 2. Ask user for printer type while preview is visible
                Dim displayMsg As String = If(String.IsNullOrEmpty(successMsg), "", successMsg & vbCrLf & vbCrLf)
                Dim printChoice As DialogResult
                
                If invNo.StartsWith("QT") Then
                    printChoice = MessageBox.Show(displayMsg & "Do you want to Save as PDF?" & vbCrLf & "(Yes = Microsoft PDF, No = Print Standard, Cancel = Close without Print)", "Quotation Print Selection", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)
                Else
                    printChoice = MessageBox.Show(displayMsg & "Do you want to print a POS Bill?" & vbCrLf & "(Yes = POS, No = Standard A4, Cancel = Close without Print)", "Print Selection", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)
                End If

                If printChoice <> DialogResult.Cancel Then
                    Dim rptIndex As Integer = 0
                    Dim printerToUse As String = ""

                    If invNo.StartsWith("QT") Then
                        rptIndex = 4
                        printerToUse = If(printChoice = DialogResult.Yes, "Microsoft Print to PDF", StandardPrinterName)
                    Else
                        If printChoice = DialogResult.Yes Then
                            rptIndex = 0
                            printerToUse = POSPrinterName
                        Else
                            rptIndex = 1
                            printerToUse = StandardPrinterName
                        End If
                    End If

                    ' 3. Direct Print
                    rptForm.ShowReport(invNo, rptIndex, True, isTrueReturn, "", 1, 0, isDuplicatePrint)
                    rptForm.DirectPrint(printerToUse)
                End If
            End Using

        Catch ex As Exception
            MessageBox.Show("Printing Failed: " & ex.Message, "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Removed btnOpenInSales per user request to simplify workflow
    ' Implementation moved to TempSales history mode entirely.

    Private Sub PaymentCollector_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.F12 Then
            ' Ensure the button is visible and enabled before clicking
            If btnCollect.Enabled AndAlso btnCollect.Visible Then
                btnCollect.PerformClick()
                e.Handled = True
            End If
        End If
    End Sub
End Class
