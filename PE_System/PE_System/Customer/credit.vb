Imports MySql.Data.MySqlClient
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared
Public Class credit
    Dim CreditAmt As Double
    Dim creid As Integer
    Private isSettingNameProgrammatically As Boolean = False
    Private isSettingComboBoxProgrammatically As Boolean = False
    Private selectedCustomerId As Integer = 0 ' Customer ID
    Private isExistingPaymentSelected As Boolean = False

    ' Enhanced Cheque Entry UI
    Private ChequeEntryPanel As Panel
    Private ChqNoTextBox As TextBox
    Private BankSearchTextBox As TextBox
    Private BankDataGridView As DataGridView
    Private ChqOkBtn As Button
    Private ChqCancelBtn As Button
    Private selectedBankId As Integer = 0
    Private selectedBankName As String = ""
    Private currentDistribution As New Dictionary(Of String, Double) ' Tracks invoice-wise allocation for cheques

    ' Pay Type Filter UI
    Private LabelPayTypeFilter As Label
    Private btnPayTypeAll As Button
    Private btnPayTypeCash As Button
    Private btnPayTypeCheque As Button
    Private btnPayTypeOnline As Button
    Private btnPayTypeReturn As Button
    Private selectedPayTypeFilter As String = "All"

    Private Sub RefreshAllGrids()
        ' This method ensures all three tabs stay synchronized
        LoadPayments()           ' Tab 3 main grid
        LoadCustomerCredits()    ' Tab 3 suggestion grid
        loaddebit()              ' Tab 2 main grid
        load_for_date()          ' Tab 1 main grid (respects date filters)
        If selectedCustomerId > 0 Then
            LoadCustomerInvoices(selectedCustomerId) ' Tab 2 invoice lookup
        End If
    End Sub
    Private Sub LoadPayments()
        MySqlConn.Open()
        Dim bsource As New BindingSource
        Dim table As New DataTable
        Dim rgrFilter As String = ""
        If Not Module1.IsRgrVisible Then
            rgrFilter = " AND cp.is_rgr = 0"
        End If

        Dim adapter As New MySqlDataAdapter(
    "SELECT cp.CusID, cp.Customer, c.tel_no AS CusTel, cp.PaymentType, cp.Amount, cp.Date, cp.inv_no, cp.cheque_no, b.bank_name, cr.status AS ChqStatus, cp.bank_id " &
    "FROM customer_payments cp " &
    "LEFT JOIN customer c ON cp.CusID = c.id " &
    "LEFT JOIN bank b ON cp.bank_id = b.id " &
    "LEFT JOIN check_received cr ON TRIM(cp.cheque_no) = TRIM(cr.check_number) AND cp.bank_id = cr.bank_id AND FIND_IN_SET(TRIM(cp.inv_no), REPLACE(cr.inv_no, ', ', ',')) " &
    "WHERE 1=1" & rgrFilter,
    MySqlConn)
        adapter.Fill(table)
        bsource.DataSource = table
        CustomerPaymentsView.DataSource = table
        FormatPaymentsGrid()
        MySqlConn.Close()
        ApplyPaymentFilters()
    End Sub

    Private Sub FormatPaymentsGrid()
        Dim dgv = CustomerPaymentsView
        If dgv IsNot Nothing AndAlso dgv.Columns.Count > 0 Then
            ' Disable autosize during formatting to prevent internal NullReferenceExceptions
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None
            dgv.RowHeadersVisible = False
            dgv.MultiSelect = False

            ' Visibility
            If dgv.Columns.Contains("CusID") Then dgv.Columns("CusID").Visible = False

            ' Layout for identifiable columns
            If dgv.Columns.Contains("Customer") Then
                dgv.Columns("Customer").HeaderText = "Customer Name"
                dgv.Columns("Customer").FillWeight = 150
                dgv.Columns("Customer").MinimumWidth = 200
            End If

            If dgv.Columns.Contains("PaymentType") Then
                dgv.Columns("PaymentType").HeaderText = "Payment Type"
                dgv.Columns("PaymentType").Width = 120
                dgv.Columns("PaymentType").MinimumWidth = 100
            End If

            If dgv.Columns.Contains("Amount") Then
                dgv.Columns("Amount").HeaderText = "Amount"
                dgv.Columns("Amount").Width = 100
                dgv.Columns("Amount").MinimumWidth = 80
            End If

            If dgv.Columns.Contains("Date") Then
                dgv.Columns("Date").HeaderText = "Date"
                dgv.Columns("Date").Width = 180
                dgv.Columns("Date").MinimumWidth = 150
                dgv.Columns("Date").DefaultCellStyle.Format = "yyyy-MM-dd"
            End If

            ' Format inv_no column if it exists in payments
            If dgv.Columns.Contains("inv_no") Then
                dgv.Columns("inv_no").HeaderText = "Inv No"
                dgv.Columns("inv_no").Width = 110
                dgv.Columns("inv_no").MinimumWidth = 100
            End If

            ' Format bank_name column
            If dgv.Columns.Contains("bank_name") Then
                dgv.Columns("bank_name").HeaderText = "Bank Name"
                dgv.Columns("bank_name").Width = 150
                dgv.Columns("bank_name").MinimumWidth = 150
            End If

            ' Format CusTel column if it exists
            If dgv.Columns.Contains("CusTel") Then
                dgv.Columns("CusTel").HeaderText = "Phone Number"
                dgv.Columns("CusTel").Width = 120
                dgv.Columns("CusTel").MinimumWidth = 100
            End If

            ' Explicitly enable scrollbars
            dgv.ScrollBars = ScrollBars.Both

            ' Hide technical columns but use them for logic
            If dgv.Columns.Contains("ChqStatus") Then dgv.Columns("ChqStatus").Visible = False
            If dgv.Columns.Contains("cheque_no") Then dgv.Columns("cheque_no").Visible = False
            If dgv.Columns.Contains("bank_id") Then dgv.Columns("bank_id").Visible = False

            ' Re-enable fill mode
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

            ' Force layout and scrollbar recalculation
            dgv.PerformLayout()
            dgv.Refresh()
        End If
    End Sub

    Private Sub CustomerPaymentsView_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles CustomerPaymentsView.CellFormatting
        ' Handle row coloring dynamically during formatting (supports sorting/filtering/scrolling)
        If e.RowIndex >= 0 Then
            Dim dgv = DirectCast(sender, DataGridView)
            Dim row = dgv.Rows(e.RowIndex)
            Dim payType = If(dgv.Columns.Contains("PaymentType") AndAlso row.Cells("PaymentType").Value IsNot Nothing, row.Cells("PaymentType").Value.ToString().ToLower(), "")

            If payType.Contains("online") Then
                row.DefaultCellStyle.BackColor = Color.FromArgb(195, 230, 255) ' Soft Light Blue for Online Payment
            ElseIf payType.Contains("return") Then
                row.DefaultCellStyle.BackColor = Color.FromArgb(255, 218, 218) ' Soft Pink for Good Return
            ElseIf dgv.Columns.Contains("ChqStatus") Then
                Dim statusValue = row.Cells("ChqStatus").Value
                If statusValue IsNot Nothing AndAlso Not IsDBNull(statusValue) Then
                    Dim statusStr As String = statusValue.ToString().ToUpper()
                    Select Case statusStr
                        Case "CLEARED", "REALISED", "PAID", "SUCCESS"
                            row.DefaultCellStyle.BackColor = Color.YellowGreen
                        Case "RETURNED", "RETURN"
                            row.DefaultCellStyle.BackColor = Color.Red
                        Case "PENDING"
                            row.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 128)
                        Case Else
                            row.DefaultCellStyle.BackColor = Color.White
                    End Select
                Else
                    ' [FIX] If status is NULL but it is a Cheque payment, default to Yellow (Pending)
                    If payType.Contains("cheque") OrElse payType.Contains("chaque") Then
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 128) ' Default Pending Color
                    Else
                        row.DefaultCellStyle.BackColor = Color.White
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub CreditDataGridView_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles CreditDataGridView.CellFormatting, DataGridView1.CellFormatting, Customer_creditDataGridView1.CellFormatting
        If e.RowIndex >= 0 Then
            Dim dgv = DirectCast(sender, DataGridView)
            Dim row = dgv.Rows(e.RowIndex)

            If dgv.Columns.Contains("Credit_Amount") Then
                Dim val = row.Cells("Credit_Amount").Value
                If val IsNot Nothing AndAlso Not IsDBNull(val) Then
                    Dim amt As Double = 0
                    If Double.TryParse(val.ToString(), amt) Then
                        If amt > 0 Then
                            e.CellStyle.BackColor = Color.LightYellow
                        Else
                            e.CellStyle.BackColor = Color.LightGreen
                        End If
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub FormatCreditsGrid(dgv As DataGridView, Optional isReadOnly As Boolean = True)
        If dgv IsNot Nothing AndAlso dgv.Columns.Count > 0 Then
            ' Disable autosize during formatting to prevent internal NullReferenceExceptions
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None
            dgv.RowHeadersVisible = False
            dgv.MultiSelect = False
            dgv.ReadOnly = isReadOnly

            ' Visibility
            If dgv.Columns.Contains("CreId") Then dgv.Columns("CreId").Visible = False
            If dgv.Columns.Contains("id") Then dgv.Columns("id").Visible = False
            If dgv.Columns.Contains("CusId") Then dgv.Columns("CusId").Visible = False
            If dgv.Columns.Contains("CusTel") Then dgv.Columns("CusTel").Visible = False

            ' WhatsApp Dynamic Button Column for DataGridView1 (Customer Details tab)
            If dgv.Name = "DataGridView1" Then
                If Not dgv.Columns.Contains("btnWA") Then
                    Dim waCol As New DataGridViewButtonColumn()
                    waCol.Name = "btnWA"
                    waCol.HeaderText = "WA"
                    waCol.Text = "W"
                    waCol.UseColumnTextForButtonValue = True
                    waCol.Width = 50
                    waCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                    waCol.FlatStyle = FlatStyle.Flat
                    waCol.DefaultCellStyle.BackColor = Color.Gainsboro ' Grey background for unselected rows
                    waCol.DefaultCellStyle.ForeColor = Color.DimGray ' Grey text for unselected rows
                    waCol.DefaultCellStyle.SelectionBackColor = Color.FromArgb(37, 211, 102) ' WhatsApp Green for selected row
                    waCol.DefaultCellStyle.SelectionForeColor = Color.White ' White text for selected row
                    dgv.Columns.Add(waCol)
                End If
                dgv.Columns("btnWA").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                dgv.Columns("btnWA").Width = 45
                dgv.Columns("btnWA").DisplayIndex = dgv.Columns.Count - 1
            End If

            ' Layout for identifiable columns
            If dgv.Columns.Contains("CusName") Then
                dgv.Columns("CusName").HeaderText = "Customer Name"
                dgv.Columns("CusName").FillWeight = 150
            End If

            If dgv.Columns.Contains("Credit_Amount") Then
                dgv.Columns("Credit_Amount").HeaderText = "Amount"
                dgv.Columns("Credit_Amount").Width = 100
            End If

            If dgv.Columns.Contains("inv_no") Then
                ' Only show Inv No if it's not the aggregated customer credit grid
                If dgv.Name = "Customer_creditDataGridView1" Then
                    dgv.Columns("inv_no").Visible = False
                Else
                    dgv.Columns("inv_no").HeaderText = "Inv No"
                    dgv.Columns("inv_no").Width = 110
                End If
            End If

            ' Format Tel No if it exists (for aggregated view)
            If dgv.Columns.Contains("CusTel") Then
                dgv.Columns("CusTel").Visible = True
                dgv.Columns("CusTel").HeaderText = "Tel Number"
                dgv.Columns("CusTel").Width = 120
            End If

            If dgv.Columns.Contains("CreditDate") Then
                If dgv.Name = "Customer_creditDataGridView1" Then
                    dgv.Columns("CreditDate").Visible = False
                Else
                    dgv.Columns("CreditDate").HeaderText = "Date"
                    dgv.Columns("CreditDate").Width = 180
                    dgv.Columns("CreditDate").DefaultCellStyle.Format = "yyyy-MM-dd"
                End If
            End If

            ' Explicitly enable scrollbars
            dgv.ScrollBars = ScrollBars.Both

            ' Re-enable fill mode
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

            ' Force layout and scrollbar recalculation
            dgv.PerformLayout()
            dgv.Refresh()
        End If
    End Sub

    Private Sub FormatSuggestionGrid(dgv As DataGridView)
        If dgv.Columns.Count > 0 Then
            dgv.RowHeadersVisible = False
            dgv.MultiSelect = False
            ' Assuming standardized query: name, address, id
            dgv.Columns(0).Width = 250 ' Name
            If dgv.Columns.Count > 2 Then dgv.Columns(2).Visible = False ' ID

            ' Explicitly enable scrollbars
            dgv.ScrollBars = ScrollBars.Both
        End If
    End Sub

    Private Sub LoadCustomerCredits()
        ' Reload the customer credit grid to show updated amounts
        Try
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Open()

            Dim bsource As New BindingSource
            Dim table As New DataTable()
            Dim rgrFilter As String = ""
            If Not Module1.IsRgrVisible Then
                rgrFilter = " AND cc.is_rgr = 0"
            End If

            ' [MODIFIED] Aggregated query to show total credit per customer
            Dim sql As String = "SELECT c.name AS CusName, c.tel_no AS CusTel, SUM(cc.amount) AS Credit_Amount, c.id AS CusId " &
                       "FROM customer_credit cc " &
                       "INNER JOIN customer c ON cc.customer_id = c.id WHERE cc.is_active = 1 AND cc.amount > 0 " & rgrFilter &
                       " GROUP BY c.id, c.name, c.tel_no"

            Dim adapter As New MySqlDataAdapter(sql, conn)
            adapter.Fill(table)
            bsource.DataSource = table
            Customer_creditDataGridView1.DataSource = table
            FormatCreditsGrid(Customer_creditDataGridView1, True)
            conn.Close()
        Catch ex As Exception
            MessageBox.Show("Error loading customer credits: " & ex.Message)
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub LoadCustomerInvoices(customerId As Integer)
        Try
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
            MySqlConn.Open()

            Dim table As New DataTable()
            Dim statusFilter As String = ""

            ' Determine status filter based on combo selection
            If DInvoiceStatusCombo.Text = "Credit" Then
                statusFilter = " AND status = 'Credit' "
            ElseIf DInvoiceStatusCombo.Text = "Cash_Credit" Then
                statusFilter = " AND status = 'Cash_Credit' "
            ElseIf DInvoiceStatusCombo.Text = "Mixed_Payment" Then
                statusFilter = " AND status = 'Mixed_Payment' "
            ElseIf DInvoiceStatusCombo.Text = "Credit_Cheque" Then
                statusFilter = " AND status = 'Credit_Cheque' "
            Else
                ' Default / "All Pending" - show specific statuses and credit_balance_due > 0
                statusFilter = " AND status IN ('Credit', 'Cash_Credit', 'Mixed_Payment', 'Credit_Cheque') "
            End If

            Dim rgrFilter As String = ""
            If Not Module1.IsRgrVisible Then
                rgrFilter = " AND is_rgr = 0"
            End If

            ' Unified Query from single billing table - Including credit_balance_due
            ' [MODIFIED] Added NOT IN condition to exclude invoices already in customer_credit table
            Dim sql As String = "SELECT inv_no, subtotal, grand_total, balance_due, credit_balance_due, timestamps, status FROM billing " &
                                "WHERE customer_id = @cid " & statusFilter & " " &
                                "AND credit_balance_due > 0 " & rgrFilter & " " &
                                "AND inv_no NOT IN (SELECT IFNULL(inv_no, '') FROM customer_credit WHERE customer_id = @cid AND is_active = 1) " &
                                "AND LOWER(status) NOT IN ('success', 'paid') " &
                                "ORDER BY timestamps DESC"

            Dim adapter As New MySqlDataAdapter(sql, MySqlConn)
            adapter.SelectCommand.Parameters.AddWithValue("@cid", customerId)
            adapter.Fill(table)

            InvoiceDataGridView.DataSource = table

            ' Format the grid
            If InvoiceDataGridView.Columns.Count > 0 Then
                InvoiceDataGridView.RowHeadersVisible = False
                InvoiceDataGridView.MultiSelect = False
                InvoiceDataGridView.Columns("inv_no").HeaderText = "Invoice No"
                InvoiceDataGridView.Columns("inv_no").Width = 100
                InvoiceDataGridView.Columns("subtotal").HeaderText = "Subtotal"
                InvoiceDataGridView.Columns("subtotal").Width = 100
                InvoiceDataGridView.Columns("grand_total").HeaderText = "Grand Total"
                InvoiceDataGridView.Columns("grand_total").Width = 100
                InvoiceDataGridView.Columns("balance_due").HeaderText = "Balance Due"
                InvoiceDataGridView.Columns("balance_due").Width = 110
                InvoiceDataGridView.Columns("credit_balance_due").HeaderText = "Credit Balance"
                InvoiceDataGridView.Columns("credit_balance_due").Width = 120
                InvoiceDataGridView.Columns("credit_balance_due").DefaultCellStyle.Format = "N2"
                InvoiceDataGridView.Columns("timestamps").HeaderText = "Date"
                InvoiceDataGridView.Columns("timestamps").Width = 140
                InvoiceDataGridView.Columns("timestamps").DefaultCellStyle.Format = "yyyy-MM-dd"
                InvoiceDataGridView.Columns("status").HeaderText = "Status"
                InvoiceDataGridView.Columns("status").Width = 100

                ' Explicitly enable scrollbars
                InvoiceDataGridView.ScrollBars = ScrollBars.Both
            End If

            ' Show panel only if invoices exist
            If table.Rows.Count > 0 Then
                Panel3.Visible = True
                Panel3.BringToFront()
            Else
                Panel3.Visible = False
                ' Only hide cancel button if customer suggestion list is also hidden
            End If

            MySqlConn.Close()
        Catch ex As Exception
            MessageBox.Show("Error loading invoices: " & ex.Message)
        Finally
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Sub

    Private Sub load_for_date()
        Try
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Open()

            Dim bsource As New BindingSource
            Dim table As New DataTable()
            Dim rgrFilter As String = ""
            If Not Module1.IsRgrVisible Then
                rgrFilter = " AND cc.is_rgr = 0"
            End If

            ' Standardized aliases to match ApplyCreditFilters
            Dim query As String = "SELECT c.name AS CusName, cc.amount AS Credit_Amount, cc.timestamps AS CreditDate, cc.inv_no " &
                          "FROM customer_credit cc " &
                          "INNER JOIN customer c ON cc.customer_id = c.id " &
                          "WHERE cc.is_active = 1 AND IFNULL(cc.complete_date, cc.timestamps) BETWEEN @start AND @end " & rgrFilter & " " &
                          "ORDER BY c.name ASC"

            Dim adapter As New MySqlDataAdapter(query, conn)
            adapter.SelectCommand.Parameters.AddWithValue("@start", StartDate.Value.ToString("yyyy-MM-dd 00:00:00"))
            adapter.SelectCommand.Parameters.AddWithValue("@end", EndDate.Value.ToString("yyyy-MM-dd 23:59:59"))
            adapter.Fill(table)

            CreditDataGridView.DataSource = table
            FormatCreditsGrid(CreditDataGridView, True)
            conn.Close()

            ' Now apply any current name/amount filters on top of the date results
            ApplyCreditFilters()
        Catch ex As Exception
            MessageBox.Show("Error loading date filter: " & ex.Message)
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub Load_credit()
        Try
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
            MySqlConn.Open()

            Dim table As New DataTable()
            Dim rgrFilter As String = ""
            If Not Module1.IsRgrVisible Then
                rgrFilter = " AND cc.is_rgr = 0"
            End If

            ' Standardized query with aliases used by filters
            Dim sql As String = "SELECT cc.id AS CreId, c.name AS CusName, cc.amount AS Credit_Amount, cc.timestamps AS CreditDate, cc.inv_no " &
                        "FROM customer_credit cc " &
                        "INNER JOIN customer c ON cc.customer_id = c.id " &
                        "WHERE cc.is_active = 1 " & rgrFilter & " " &
                        "ORDER BY c.name ASC"

            Dim adapter As New MySqlDataAdapter(sql, MySqlConn)
            adapter.Fill(table)

            CreditDataGridView.DataSource = table
            FormatCreditsGrid(CreditDataGridView, True)

            ' Apply any existing text filters
            ApplyCreditFilters()

            MySqlConn.Close()
        Catch ex As Exception
            MessageBox.Show("Error loading credits: " & ex.Message)
        Finally
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Sub


    Private Sub Credit_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.KeyPreview = True
        StartDate.Value = DateTime.Now
        EndDate.Value = DateTime.Now
        dtpDetailsStart.Value = DateTime.Now
        dtpDetailsEnd.Value = DateTime.Now.AddDays(1)
        Customer_creditDataGridView1.ReadOnly = True
        
        SyncMissingManualCredits()
        
        ' Ensure manual entry fields are editable regardless of Designer settings
        DAmountTxt.ReadOnly = False
        DCustomerInvTxt.ReadOnly = False
        Customer_creditDataGridView1.AllowUserToAddRows = False
        ' Set high-contrast styling for the total labels first
        TotalCriditLbl.BackColor = Color.White
        TotalCriditLbl.ForeColor = Color.Black
        TotalCriditLbl.Text = "0.00"

        Label16.BackColor = Color.White
        Label16.ForeColor = Color.Black
        Label16.Text = "0.00"

        Label18.BackColor = Color.White
        Label18.ForeColor = Color.Black
        Label18.Text = "0.00"

        ' Initialize Pay Type Filters
        InitPayTypeFilters()
        UpdatePayTypeButtonStyles()

        ' Now load the data which will populate these labels
        Load_credit()
        LoadPayments()
        loaddebit()
        LoadUsers()

        ' Initialize Invoice Status Filter
        DInvoiceStatusCombo.Items.Clear()
        DInvoiceStatusCombo.Items.Add("All Pending")
        DInvoiceStatusCombo.Items.Add("Credit")
        DInvoiceStatusCombo.Items.Add("Cash_Credit")
        DInvoiceStatusCombo.Items.Add("Mixed_Payment")
        DInvoiceStatusCombo.Items.Add("Credit_Cheque")
        DInvoiceStatusCombo.SelectedIndex = 0 ' Default to All Pending

        ' Initialize Credit Filters
        cmbCreditFilter1.Items.Clear()
        cmbCreditFilter1.Items.AddRange(New Object() {"All", "Pending", "Paid"})
        cmbCreditFilter1.SelectedIndex = 1

        cmbCreditFilter2.Items.Clear()
        cmbCreditFilter2.Items.AddRange(New Object() {"All", "Pending", "Paid"})
        cmbCreditFilter2.SelectedIndex = 1

        Panel1.Visible = False
        pay_cancel.Visible = False
        Panel2.Visible = False
        Panel3.Visible = False

        InitializeChequePanel()
        load_for_date()

        ' Hide Update/Delete buttons for non-owner roles (Admin and Cashier)
        Dim role As String = If(Module1.UserRole IsNot Nothing, Module1.UserRole.ToLower(), "")
        If role = "admin" OrElse role = "cashier" Then
            DDeleteBtn.Visible = False
            DUpdateBtn.Visible = False
            pay_del.Visible = False
        End If
    End Sub

    Private Sub InitPayTypeFilters()
        ' Adjust GroupBox6 (Filter Options) Height and CustomerPaymentsView
        GroupBox6.Height = 140
        CustomerPaymentsView.Location = New Point(4, 307)
        CustomerPaymentsView.Height = 374

        ' Create Label
        LabelPayTypeFilter = New Label()
        LabelPayTypeFilter.AutoSize = True
        LabelPayTypeFilter.Font = New Font("Microsoft Sans Serif", 12.75!, FontStyle.Bold, GraphicsUnit.Point, CType(0, Byte))
        LabelPayTypeFilter.ForeColor = Color.White
        LabelPayTypeFilter.Location = New Point(52, 92)
        LabelPayTypeFilter.Name = "LabelPayTypeFilter"
        LabelPayTypeFilter.Size = New Size(170, 26)
        LabelPayTypeFilter.Text = "Filter Pay Type:"
        GroupBox6.Controls.Add(LabelPayTypeFilter)

        ' Instantiate buttons
        btnPayTypeAll = New Button()
        ConfigureFilterButton(btnPayTypeAll, "All", "All", 250, 100)

        btnPayTypeCash = New Button()
        ConfigureFilterButton(btnPayTypeCash, "Cash", "Cash", 360, 120)

        btnPayTypeCheque = New Button()
        ConfigureFilterButton(btnPayTypeCheque, "Cheque", "Cheque", 490, 140)

        btnPayTypeOnline = New Button()
        ConfigureFilterButton(btnPayTypeOnline, "Online Transfer", "Online Payment", 640, 200)

        btnPayTypeReturn = New Button()
        ConfigureFilterButton(btnPayTypeReturn, "Good Return", "Good Return", 850, 180)
    End Sub

    Private Sub ConfigureFilterButton(btn As Button, text As String, tagVal As String, posX As Integer, widthVal As Integer)
        btn.FlatStyle = FlatStyle.Flat
        btn.Text = text
        btn.Tag = tagVal
        btn.Location = New Point(posX, 85)
        btn.Size = New Size(widthVal, 38)
        btn.Font = New Font("Microsoft Sans Serif", 11.0!, FontStyle.Bold, GraphicsUnit.Point, CType(0, Byte))
        btn.FlatAppearance.BorderSize = 1
        btn.Cursor = Cursors.Hand
        AddHandler btn.Click, AddressOf PayTypeFilterBtn_Click
        GroupBox6.Controls.Add(btn)
    End Sub

    Private Sub PayTypeFilterBtn_Click(sender As Object, e As EventArgs)
        Dim btn = DirectCast(sender, Button)
        selectedPayTypeFilter = btn.Tag.ToString()
        UpdatePayTypeButtonStyles()
        ApplyPaymentFilters()
    End Sub

    Private Sub UpdatePayTypeButtonStyles()
        Dim buttons = {btnPayTypeAll, btnPayTypeCash, btnPayTypeCheque, btnPayTypeOnline, btnPayTypeReturn}
        For Each btn In buttons
            If btn IsNot Nothing Then
                ' Determine background color for each button type
                Dim btnColor As Color = Color.LightGray
                Select Case btn.Tag.ToString()
                    Case "All"
                        btnColor = Color.LightGray
                    Case "Cash"
                        btnColor = Color.White
                    Case "Cheque"
                        btnColor = Color.FromArgb(255, 255, 128) ' Yellow
                    Case "Online Payment"
                        btnColor = Color.FromArgb(195, 230, 255) ' Soft Light Blue
                    Case "Good Return"
                        btnColor = Color.FromArgb(255, 218, 218) ' Soft Pink
                End Select

                btn.BackColor = btnColor
                btn.ForeColor = Color.Black

                If btn.Tag.ToString() = selectedPayTypeFilter Then
                    ' Highlight selected button with a thick black border
                    btn.FlatAppearance.BorderSize = 3
                    btn.FlatAppearance.BorderColor = Color.Black
                Else
                    ' Subtle border for unselected buttons
                    btn.FlatAppearance.BorderSize = 1
                    btn.FlatAppearance.BorderColor = Color.DarkSlateGray
                End If
            End If
        Next
    End Sub

    Private Sub credit_Activated(sender As Object, e As EventArgs) Handles MyBase.Activated
        RefreshAllGrids()
    End Sub

    Private Sub InitializeChequePanel()
        ' Create Panel
        ChequeEntryPanel = New Panel()
        ChequeEntryPanel.Size = New Size(450, 500)
        ChequeEntryPanel.BackColor = Color.FromArgb(52, 73, 94) ' Modern dark blue/grey
        ChequeEntryPanel.BorderStyle = BorderStyle.FixedSingle
        ChequeEntryPanel.Visible = False
        TabPage3.Controls.Add(ChequeEntryPanel)
        ChequeEntryPanel.BringToFront()

        ' Center the panel
        AddHandler Me.Resize, Sub()
                                  ChequeEntryPanel.Location = New Point((TabPage3.Width - ChequeEntryPanel.Width) / 2, (TabPage3.Height - ChequeEntryPanel.Height) / 2)
                              End Sub
        ChequeEntryPanel.Location = New Point((TabPage3.Width - ChequeEntryPanel.Width) / 2, (TabPage3.Height - ChequeEntryPanel.Height) / 2)

        ' Header
        Dim headerLabel As New Label()
        headerLabel.Text = "CHEQUE PAYMENT DETAILS"
        headerLabel.Font = New Font("Segoe UI", 14, FontStyle.Bold)
        headerLabel.ForeColor = Color.Yellow
        headerLabel.AutoSize = True
        headerLabel.Location = New Point(100, 20)
        ChequeEntryPanel.Controls.Add(headerLabel)

        ' Cheque Number
        Dim lblChq As New Label()
        lblChq.Text = "Cheque Number:"
        lblChq.Font = New Font("Segoe UI", 11, FontStyle.Bold)
        lblChq.ForeColor = Color.White
        lblChq.Location = New Point(30, 70)
        lblChq.Size = New Size(150, 25)
        ChequeEntryPanel.Controls.Add(lblChq)

        ChqNoTextBox = New TextBox()
        ChqNoTextBox.Font = New Font("Segoe UI", 11)
        ChqNoTextBox.Location = New Point(30, 95)
        ChqNoTextBox.Size = New Size(380, 27)
        ChequeEntryPanel.Controls.Add(ChqNoTextBox)

        ' Bank Search
        Dim lblBank As New Label()
        lblBank.Text = "Search & Select Bank (Required):"
        lblBank.Font = New Font("Segoe UI", 11, FontStyle.Bold)
        lblBank.ForeColor = Color.White
        lblBank.Location = New Point(30, 140)
        lblBank.Size = New Size(250, 25)
        ChequeEntryPanel.Controls.Add(lblBank)

        BankSearchTextBox = New TextBox()
        BankSearchTextBox.Font = New Font("Segoe UI", 11)
        BankSearchTextBox.Location = New Point(30, 165)
        BankSearchTextBox.Size = New Size(380, 27)
        BankSearchTextBox.BackColor = Color.Bisque
        ChequeEntryPanel.Controls.Add(BankSearchTextBox)

        ' Bank Grid
        BankDataGridView = New DataGridView()
        BankDataGridView.Location = New Point(30, 200)
        BankDataGridView.Size = New Size(380, 220)
        BankDataGridView.AllowUserToAddRows = False
        BankDataGridView.AllowUserToDeleteRows = False
        BankDataGridView.ReadOnly = True
        BankDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        BankDataGridView.MultiSelect = False
        BankDataGridView.RowHeadersVisible = False
        BankDataGridView.BackgroundColor = Color.White
        BankDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        BankDataGridView.ScrollBars = ScrollBars.Both
        ChequeEntryPanel.Controls.Add(BankDataGridView)

        ' Buttons
        ChqOkBtn = New Button()
        ChqOkBtn.Text = "OK"
        ChqOkBtn.Size = New Size(120, 40)
        ChqOkBtn.Location = New Point(180, 440)
        ChqOkBtn.BackColor = Color.DodgerBlue
        ChqOkBtn.Font = New Font("Segoe UI", 11, FontStyle.Bold)
        ChqOkBtn.ForeColor = Color.White
        AddHandler ChqOkBtn.Click, AddressOf ChqOkBtn_Click
        ChequeEntryPanel.Controls.Add(ChqOkBtn)

        ChqCancelBtn = New Button()
        ChqCancelBtn.Text = "Cancel"
        ChqCancelBtn.Size = New Size(100, 40)
        ChqCancelBtn.Location = New Point(310, 440)
        ChqCancelBtn.BackColor = Color.Crimson
        ChqCancelBtn.Font = New Font("Segoe UI", 11, FontStyle.Bold)
        ChqCancelBtn.ForeColor = Color.White
        AddHandler ChqCancelBtn.Click, Sub()
                                           ChequeEntryPanel.Visible = False
                                       End Sub
        ChequeEntryPanel.Controls.Add(ChqCancelBtn)

        ' Keyboard navigation
        AddHandler ChqNoTextBox.KeyDown, Sub(sender As Object, e As KeyEventArgs)
                                             If e.KeyCode = Keys.Enter Then
                                                 BankSearchTextBox.Focus()
                                                 e.Handled = True
                                                 e.SuppressKeyPress = True
                                             End If
                                         End Sub

        AddHandler BankSearchTextBox.KeyDown, Sub(sender As Object, e As KeyEventArgs)
                                                  If e.Shift AndAlso e.KeyCode = Keys.Enter Then
                                                      ChqNoTextBox.Focus()
                                                      e.Handled = True
                                                      e.SuppressKeyPress = True
                                                  ElseIf e.KeyCode = Keys.Up OrElse e.KeyCode = Keys.Down Then
                                                      If BankDataGridView.Rows.Count > 0 Then
                                                          BankDataGridView.Focus()
                                                      End If
                                                      e.Handled = True
                                                      e.SuppressKeyPress = True
                                                  ElseIf e.KeyCode = Keys.Enter Then
                                                      If String.IsNullOrWhiteSpace(BankSearchTextBox.Text) OrElse selectedBankId = 0 Then
                                                          MessageBox.Show("Please select a bank from the list.")
                                                          BankSearchTextBox.Focus()
                                                      ElseIf BankDataGridView.Rows.Count > 0 Then
                                                          BankDataGridView.Focus()
                                                      End If
                                                      e.Handled = True
                                                      e.SuppressKeyPress = True
                                                  End If
                                              End Sub

        AddHandler BankDataGridView.KeyDown, Sub(sender As Object, e As KeyEventArgs)
                                                 If e.KeyCode = Keys.Enter Then
                                                     If BankDataGridView.CurrentRow IsNot Nothing Then
                                                         Dim row = BankDataGridView.CurrentRow
                                                         selectedBankId = Convert.ToInt32(row.Cells("id").Value)
                                                         selectedBankName = row.Cells("bank_name").Value.ToString()
                                                         BankSearchTextBox.Text = selectedBankName
                                                         ChqOkBtn.PerformClick()
                                                     End If
                                                     e.Handled = True
                                                     e.SuppressKeyPress = True
                                                 End If
                                             End Sub

        ' Event for filtering banks
        AddHandler BankSearchTextBox.TextChanged, Sub()
                                                      LoadBanks(BankSearchTextBox.Text)
                                                  End Sub

        ' Event for selecting bank
        AddHandler BankDataGridView.CellClick, Sub(sender As Object, e As DataGridViewCellEventArgs)
                                                   If e.RowIndex >= 0 Then
                                                       Dim row = BankDataGridView.Rows(e.RowIndex)
                                                       selectedBankId = Convert.ToInt32(row.Cells("id").Value)
                                                       selectedBankName = row.Cells("bank_name").Value.ToString()
                                                       BankSearchTextBox.Text = selectedBankName
                                                   End If
                                               End Sub
    End Sub

    Private Sub LoadBanks(Optional filter As String = "")
        ' Use local connection to prevent "Connection already open" conflict
        Dim localConn As New MySqlConnection(ConnStr)
        Try
            localConn.Open()
            Dim sql As String = "SELECT id, bank_name FROM bank"
            If Not String.IsNullOrEmpty(filter) Then
                sql &= " WHERE bank_name LIKE @filter"
            End If
            Dim adapter As New MySqlDataAdapter(sql, localConn)
            If Not String.IsNullOrEmpty(filter) Then
                adapter.SelectCommand.Parameters.AddWithValue("@filter", "%" & filter & "%")
            End If
            Dim table As New DataTable()
            adapter.Fill(table)
            BankDataGridView.DataSource = table
            BankDataGridView.ClearSelection()
            If BankDataGridView.Columns.Contains("id") Then BankDataGridView.Columns("id").Visible = False
        Catch ex As Exception
        Finally
            If localConn.State = ConnectionState.Open Then localConn.Close()
            localConn.Dispose()
        End Try
    End Sub

    Private Sub ChqOkBtn_Click(sender As Object, e As EventArgs)
        If selectedBankId = 0 Then
            MessageBox.Show("Please select a bank from the list.")
            Return
        End If

        ' All good, hide panel and continue save
        ChequeEntryPanel.Visible = False
        SaveChequePaymentFinal()
    End Sub

    Private Sub StartDate_ValueChanged(sender As Object, e As EventArgs) Handles StartDate.ValueChanged
        load_for_date()
    End Sub

    Private Sub EndDate_ValueChanged(sender As Object, e As EventArgs) Handles EndDate.ValueChanged
        load_for_date()
    End Sub

    Private Sub ApplyCreditFilters()
        Try
            Dim table As DataTable = Nothing

            ' Robustly get the underlying DataTable
            If TypeOf CreditDataGridView.DataSource Is DataView Then
                table = DirectCast(CreditDataGridView.DataSource, DataView).Table
            ElseIf TypeOf CreditDataGridView.DataSource Is DataTable Then
                table = DirectCast(CreditDataGridView.DataSource, DataTable)
            ElseIf TypeOf CreditDataGridView.DataSource Is BindingSource Then
                Dim bsource = DirectCast(CreditDataGridView.DataSource, BindingSource)
                If TypeOf bsource.DataSource Is DataTable Then
                    table = DirectCast(bsource.DataSource, DataTable)
                ElseIf TypeOf bsource.DataSource Is DataView Then
                    table = DirectCast(bsource.DataSource, DataView).Table
                End If
            End If

            If table Is Nothing Then Return

            Dim dv As New DataView(table)
            Dim filterStrings As New List(Of String)

            If Not String.IsNullOrEmpty(NameSeachTextBox.Text) Then
                filterStrings.Add(String.Format("CusName Like '{0}%'", NameSeachTextBox.Text.Replace("'", "''")))
            End If

            If Not String.IsNullOrEmpty(credit_amount.Text) Then
                filterStrings.Add(String.Format("Convert(Credit_Amount, 'System.String') Like '%{0}%'", credit_amount.Text.Replace("'", "''")))
            End If

            ' Status filter via cmbCreditFilter2
            If cmbCreditFilter2.Text = "Pending" Then
                filterStrings.Add("Credit_Amount > 0")
            ElseIf cmbCreditFilter2.Text = "Paid" Then
                filterStrings.Add("Credit_Amount = 0")
            End If

            dv.RowFilter = String.Join(" AND ", filterStrings)
            dv.Sort = "CusName ASC, CreditDate DESC"
            CreditDataGridView.DataSource = dv
            FormatCreditsGrid(CreditDataGridView, True)

            ' Calculate total for filtered results
            Dim tot As Double = 0
            For Each row As DataGridViewRow In CreditDataGridView.Rows
                If Not row.IsNewRow Then
                    ' Robustly get the amount value
                    Dim cellValue = Nothing
                    If CreditDataGridView.Columns.Contains("Credit_Amount") Then
                        cellValue = row.Cells("Credit_Amount").Value
                    End If

                    If cellValue IsNot Nothing AndAlso Not IsDBNull(cellValue) Then
                        Dim tempVal As Double = 0
                        ' Use TryParse to avoid numeric conversion errors
                        If Double.TryParse(cellValue.ToString(), tempVal) Then
                            tot += tempVal
                        End If
                    End If
                End If
            Next
            TotalCriditLbl.Text = tot.ToString("N2")
        Catch ex As Exception
            ' Silent fail during typing to avoid intrusive popups
        End Try
    End Sub

    Private Sub pay_search_TextChanged(sender As Object, e As EventArgs) Handles TextBox6.TextChanged
        ApplyPaymentFilters()
    End Sub

    Private Sub TextBox5_TextChanged(sender As Object, e As EventArgs) Handles TextBox5.TextChanged
        ApplyPaymentFilters()
    End Sub

    Private Sub TextBox4_TextChanged(sender As Object, e As EventArgs) Handles TextBox4.TextChanged
        ApplyPaymentFilters()
    End Sub


    Private Sub ApplyPaymentFilters()
        Try
            If CustomerPaymentsView.DataSource Is Nothing Then Return

            Dim table As DataTable = Nothing
            If TypeOf CustomerPaymentsView.DataSource Is DataView Then
                table = DirectCast(CustomerPaymentsView.DataSource, DataView).Table
            ElseIf TypeOf CustomerPaymentsView.DataSource Is DataTable Then
                table = DirectCast(CustomerPaymentsView.DataSource, DataTable)
            ElseIf TypeOf CustomerPaymentsView.DataSource Is BindingSource Then
                Dim bsource = DirectCast(CustomerPaymentsView.DataSource, BindingSource)
                If TypeOf bsource.DataSource Is DataTable Then
                    table = DirectCast(bsource.DataSource, DataTable)
                ElseIf TypeOf bsource.DataSource Is DataView Then
                    table = DirectCast(bsource.DataSource, DataView).Table
                End If
            End If

            If table Is Nothing Then Return

            Dim dv As New DataView(table)
            Dim filters As New List(Of String)

            ' Filter by Name (TextBox6)
            If Not String.IsNullOrEmpty(TextBox6.Text) Then
                filters.Add(String.Format("Customer Like '{0}%'", TextBox6.Text.Replace("'", "''")))
            End If

            ' Filter by Phone (TextBox5)
            If Not String.IsNullOrEmpty(TextBox5.Text) Then
                filters.Add(String.Format("CusTel Like '{0}%'", TextBox5.Text.Replace("'", "''")))
            End If

            ' Filter by Invoice (TextBox4)
            If Not String.IsNullOrEmpty(TextBox4.Text) Then
                filters.Add(String.Format("inv_no Like '{0}%'", TextBox4.Text.Replace("'", "''")))
            End If

            ' Filter by Pay Type
            If Not String.IsNullOrEmpty(selectedPayTypeFilter) AndAlso selectedPayTypeFilter <> "All" Then
                filters.Add(String.Format("PaymentType = '{0}'", selectedPayTypeFilter.Replace("'", "''")))
            End If

            dv.RowFilter = String.Join(" AND ", filters)
            dv.Sort = "Customer ASC, Date DESC"
            CustomerPaymentsView.DataSource = dv
            FormatPaymentsGrid()

            ' Calculate total for filtered results
            Dim tot As Double = 0
            For Each row As DataGridViewRow In CustomerPaymentsView.Rows
                If Not row.IsNewRow Then
                    Dim val = row.Cells("Amount").Value
                    If val IsNot Nothing AndAlso Not IsDBNull(val) Then
                        tot += Convert.ToDouble(val)
                    End If
                End If
            Next
            Label18.Text = tot.ToString("N2")
        Catch ex As Exception
            ' Silent fail during typing
        End Try
    End Sub

    Private Sub CustomerPaymentsView_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles CustomerPaymentsView.CellClick
        If e.RowIndex >= 0 Then
            Dim row As DataGridViewRow = CustomerPaymentsView.Rows(e.RowIndex)

            ' Set the customer ID
            If CustomerPaymentsView.Columns.Contains("CusID") Then
                selectedCustomerId = Convert.ToInt32(row.Cells("CusID").Value)
            End If

            ' Set the customer name
            isExistingPaymentSelected = True ' Capture that we are viewing an existing payment
            isSettingNameProgrammatically = True
            NameTextBox1.Text = If(row.Cells("Customer").Value?.ToString(), "")
            isSettingNameProgrammatically = False

            ' Set the amount
            AmountTextBox1.Text = If(row.Cells("Amount").Value?.ToString(), "")

            ' Set the payment type
            If CustomerPaymentsView.Columns.Contains("PaymentType") Then
                Dim pType As String = If(row.Cells("PaymentType").Value?.ToString(), "").Trim()
                ' Map possible variations
                If pType.Equals("Chaque", StringComparison.OrdinalIgnoreCase) Then pType = "Cheque"

                Dim idx As Integer = -1
                For i As Integer = 0 To ComboBox1.Items.Count - 1
                    If ComboBox1.Items(i).ToString().Equals(pType, StringComparison.OrdinalIgnoreCase) Then
                        idx = i
                        Exit For
                    End If
                Next
                ComboBox1.SelectedIndex = idx
            End If

            ' Set the date
            If CustomerPaymentsView.Columns.Contains("Date") Then
                DateTimePicker4.Value = Convert.ToDateTime(row.Cells("Date").Value)
            End If

            ' Fetch Tel No from customer table
            Try
                If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
                MySqlConn.Open()
                Using cmd As New MySqlCommand("SELECT tel_no FROM customer WHERE id = @id", MySqlConn)
                    cmd.Parameters.AddWithValue("@id", selectedCustomerId)
                    Dim res = cmd.ExecuteScalar()
                    If res IsNot Nothing AndAlso Not IsDBNull(res) Then
                        TelNoTextBox.Text = res.ToString()
                    Else
                        TelNoTextBox.Clear()
                    End If
                End Using
            Catch ex As Exception
                TelNoTextBox.Clear()
            Finally
                If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
            End Try

            ' Load invoices for this customer
            If selectedCustomerId > 0 Then
                LoadCustomerInvoices(selectedCustomerId)
            End If
        End If
    End Sub

    Private Sub CustomerPaymentsView_KeyDown(sender As Object, e As KeyEventArgs) Handles CustomerPaymentsView.KeyDown
        If e.KeyCode = Keys.Enter Then
            If CustomerPaymentsView.CurrentRow IsNot Nothing Then
                Dim row As DataGridViewRow = CustomerPaymentsView.CurrentRow

                ' Set the customer ID
                If CustomerPaymentsView.Columns.Contains("CusID") Then
                    selectedCustomerId = Convert.ToInt32(row.Cells("CusID").Value)
                End If

                ' Set the customer name
                isExistingPaymentSelected = True ' Capture existing payment view
                isSettingNameProgrammatically = True
                NameTextBox1.Text = If(row.Cells("Customer").Value?.ToString(), "")
                isSettingNameProgrammatically = False

                ' Set the amount
                AmountTextBox1.Text = If(row.Cells("Amount").Value?.ToString(), "")

                ' Set the payment type
                If CustomerPaymentsView.Columns.Contains("PaymentType") Then
                    Dim pType As String = If(row.Cells("PaymentType").Value?.ToString(), "").Trim()
                    ' Map possible variations
                    If pType.Equals("Chaque", StringComparison.OrdinalIgnoreCase) Then pType = "Cheque"

                    Dim idx As Integer = -1
                    For i As Integer = 0 To ComboBox1.Items.Count - 1
                        If ComboBox1.Items(i).ToString().Equals(pType, StringComparison.OrdinalIgnoreCase) Then
                            idx = i
                            Exit For
                        End If
                    Next
                    ComboBox1.SelectedIndex = idx
                End If

                ' Set the date
                If CustomerPaymentsView.Columns.Contains("Date") Then
                    DateTimePicker4.Value = Convert.ToDateTime(row.Cells("Date").Value)
                End If

                ' Fetch Tel No from customer table
                Try
                    If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
                    MySqlConn.Open()
                    Using cmd As New MySqlCommand("SELECT tel_no FROM customer WHERE id = @id", MySqlConn)
                        cmd.Parameters.AddWithValue("@id", selectedCustomerId)
                        Dim res = cmd.ExecuteScalar()
                        If res IsNot Nothing AndAlso Not IsDBNull(res) Then
                            TelNoTextBox.Text = res.ToString()
                        Else
                            TelNoTextBox.Clear()
                        End If
                    End Using
                Catch ex As Exception
                    TelNoTextBox.Clear()
                Finally
                    If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
                End Try

                ' Load invoices for this customer
                If selectedCustomerId > 0 Then
                    LoadCustomerInvoices(selectedCustomerId)
                End If

                e.Handled = True
                e.SuppressKeyPress = True
            End If
        End If
    End Sub

    Private Sub NameSeachTextBox_TextChanged(sender As Object, e As EventArgs) Handles NameSeachTextBox.TextChanged
        ApplyCreditFilters()
    End Sub

    Private Sub NameSeachTextBox_KeyDown(sender As Object, e As KeyEventArgs) Handles NameSeachTextBox.KeyDown
        If e.KeyCode = Keys.Down OrElse e.KeyCode = Keys.Enter Then
            credit_amount.Focus()
            If e.KeyCode = Keys.Enter Then
                e.Handled = True
                e.SuppressKeyPress = True
            End If
        End If
    End Sub
    Private Sub credit_amount_KeyDown(sender As Object, e As KeyEventArgs) Handles credit_amount.KeyDown
        If e.KeyCode = Keys.Down OrElse e.KeyCode = Keys.Enter Then
            StartDate.Focus()
            If e.KeyCode = Keys.Enter Then
                e.Handled = True
                e.SuppressKeyPress = True
            End If
        End If
    End Sub

    Private Sub StartDate_Enter(sender As Object, e As EventArgs) Handles StartDate.Enter
        Me.BeginInvoke(Sub() SendKeys.Send("{F4}"))
    End Sub

    Private Sub StartDate_KeyDown(sender As Object, e As KeyEventArgs) Handles StartDate.KeyDown
        If e.KeyCode = Keys.Enter Then
            EndDate.Focus()
            e.Handled = True
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub EndDate_Enter(sender As Object, e As EventArgs) Handles EndDate.Enter
        Me.BeginInvoke(Sub() SendKeys.Send("{F4}"))
    End Sub

    Private Sub EndDate_KeyDown(sender As Object, e As KeyEventArgs) Handles EndDate.KeyDown
        If e.KeyCode = Keys.Enter Then
            cmbCreditFilter2.Focus()
            e.Handled = True
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub credit_amount_TextChanged(sender As Object, e As EventArgs) Handles credit_amount.TextChanged
        ApplyCreditFilters()
    End Sub

    Private Sub NameTextBox1_TextChanged(sender As Object, e As EventArgs) Handles NameTextBox1.TextChanged
        ' Don't reload grid if we're setting the name programmatically from a row click
        If isSettingNameProgrammatically Then
            Return
        End If

        isExistingPaymentSelected = False ' Any manual typing resets existing payment selection state

        '--------------------'
        If conn.State = ConnectionState.Open Then conn.Close()
        conn.Open()

        Dim bsource As New BindingSource
        Dim table As New DataTable()
        Dim rgrFilter As String = ""
        If Not Module1.IsRgrVisible Then
            rgrFilter = " AND cc.is_rgr = 0"
        End If

        ' [MODIFIED] Use aggregated query to match main view
        Dim sql As String = "SELECT c.name AS CusName, c.tel_no AS CusTel, SUM(cc.amount) AS Credit_Amount, c.id AS CusId " &
                   "FROM customer_credit cc " &
                   "INNER JOIN customer c ON cc.customer_id = c.id WHERE cc.is_active = 1 AND cc.amount > 0 " & rgrFilter &
                   " GROUP BY c.id, c.name, c.tel_no"

        Dim adapter As New MySqlDataAdapter(sql, conn)
        adapter.Fill(table)
        bsource.DataSource = table
        Dim dv As New DataView(table)

        dv.RowFilter = String.Format("CusName Like '{0}%'", NameTextBox1.Text.Replace("'", "''"))
        dv.Sort = "CusName ASC"
        Customer_creditDataGridView1.DataSource = dv
        FormatCreditsGrid(Customer_creditDataGridView1, True)

        ' Show panel only if we have data and user has started typing
        Panel1.Visible = (dv.Count > 0 AndAlso NameTextBox1.Text.Trim() <> "")
        pay_cancel.Visible = Panel1.Visible
        conn.Close()
        '----------------'
    End Sub

    Private Sub NameTextBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles NameTextBox1.KeyDown
        If e.KeyCode = Keys.Up Or e.KeyCode = Keys.Down Then
            If Panel1.Visible AndAlso Customer_creditDataGridView1.Rows.Count > 0 Then
                Customer_creditDataGridView1.Focus()
                ' Ensure the first row is selected if nothing is selected
                If Customer_creditDataGridView1.CurrentRow Is Nothing Then
                    Customer_creditDataGridView1.Rows(0).Selected = True
                End If
                e.Handled = True
                e.SuppressKeyPress = True
            End If
        End If

        If e.KeyCode = Keys.Enter Then
            If Panel1.Visible AndAlso Customer_creditDataGridView1.Rows.Count > 0 Then
                ' Select the first row
                Dim row As DataGridViewRow = Customer_creditDataGridView1.Rows(0)

                ' Set the credit ID safely via name
                If Customer_creditDataGridView1.Columns.Contains("CreId") AndAlso row.Cells("CreId").Value IsNot Nothing AndAlso Not IsDBNull(row.Cells("CreId").Value) Then
                    Integer.TryParse(row.Cells("CreId").Value.ToString(), creid)
                Else
                    creid = 0
                End If

                isExistingPaymentSelected = False ' Selecting from credit suggestions is for a new payment
                isSettingNameProgrammatically = True

                ' Set Name safely via name
                If Customer_creditDataGridView1.Columns.Contains("CusName") Then
                    NameTextBox1.Text = If(row.Cells("CusName").Value?.ToString(), "")
                End If

                isSettingNameProgrammatically = False

                ' Set the Tel No safely via name
                If Customer_creditDataGridView1.Columns.Contains("CusTel") Then
                    TelNoTextBox.Text = If(row.Cells("CusTel").Value?.ToString(), "")
                Else
                    TelNoTextBox.Clear()
                End If

                ' Set the current Credit Amount safely via name
                CreditAmt = 0
                If Customer_creditDataGridView1.Columns.Contains("Credit_Amount") AndAlso row.Cells("Credit_Amount").Value IsNot Nothing AndAlso Not IsDBNull(row.Cells("Credit_Amount").Value) Then
                    Dim cellValue = row.Cells("Credit_Amount").Value
                    If TypeOf cellValue Is Double OrElse TypeOf cellValue Is Decimal OrElse TypeOf cellValue Is Integer Then
                        CreditAmt = Convert.ToDouble(cellValue)
                    Else
                        Double.TryParse(cellValue.ToString(), CreditAmt)
                    End If
                End If
                Label18.Text = CreditAmt.ToString("N2")

                ' IMPORTANT: Also need to get the customer ID (selectedCustomerId) strictly
                If Customer_creditDataGridView1.Columns.Contains("CusId") AndAlso row.Cells("CusId").Value IsNot Nothing AndAlso Not IsDBNull(row.Cells("CusId").Value) Then
                    Integer.TryParse(row.Cells("CusId").Value.ToString(), selectedCustomerId)
                ElseIf creid > 0 Then
                    ' Fallback query if for some reason the column is missing
                    Try
                        If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
                        MySqlConn.Open()
                        Dim findUidSql As String = "SELECT customer_id FROM customer_credit WHERE id = @creid"
                        Using cmdFindUid As New MySqlCommand(findUidSql, MySqlConn)
                            cmdFindUid.Parameters.AddWithValue("@creid", creid)
                            Dim res = cmdFindUid.ExecuteScalar()
                            If res IsNot Nothing Then selectedCustomerId = Convert.ToInt32(res)
                        End Using
                    Catch ex As Exception
                    Finally
                        If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
                    End Try
                End If

                ' Move focus and hide panel
                ComboBox1.Select()
                Panel1.Visible = False

                ' Prevent default enter key behavior
                e.Handled = True
                e.SuppressKeyPress = True
            End If
        End If
    End Sub

    Private Sub NameTextBox1_KeyPress(sender As Object, e As KeyPressEventArgs) Handles NameTextBox1.KeyPress
        If e.KeyChar = ChrW(Keys.Escape) Then
            Panel1.Visible = False
        End If
    End Sub


    Private Sub pay_add_Click(sender As Object, e As EventArgs) Handles pay_add.Click
        NameTextBox1.Clear()
        AmountTextBox1.Clear()
        isExistingPaymentSelected = False ' Reset state for new entry
        selectedCustomerId = 0
        creid = 0
        CreditAmt = 0
        TelNoTextBox.Clear()
        ComboBox1.SelectedIndex = -1 ' Clear selection to force user selection
        DateTimePicker4.Value = DateTime.Now
        NameTextBox1.Select()
        ApplyPaymentFilters()
    End Sub
    Private Sub Label12_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub pay_save_Click(sender As Object, e As EventArgs) Handles pay_save.Click
        If isExistingPaymentSelected Then
            MessageBox.Show("This is an existing payment record. You cannot 'Create' it again. Please click 'Add New' to start a new payment record.", "Operation Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Validate that a payment type is explicitly selected
        If ComboBox1.SelectedIndex = -1 OrElse String.IsNullOrWhiteSpace(ComboBox1.Text) Then
            MessageBox.Show("Please select a payment type.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim inputAmount As Double = 0
        If Not Double.TryParse(AmountTextBox1.Text, inputAmount) OrElse inputAmount <= 0 Then
            MessageBox.Show("Please enter a valid numeric amount greater than zero.")
            Return
        End If

        If selectedCustomerId = 0 AndAlso Not String.IsNullOrEmpty(NameTextBox1.Text) Then
            Try
                If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
                MySqlConn.Open()
                Dim findUidSql As String = "SELECT id FROM customer WHERE name = @name AND is_block = 0 LIMIT 1"
                Using cmdFind As New MySqlCommand(findUidSql, MySqlConn)
                    cmdFind.Parameters.AddWithValue("@name", NameTextBox1.Text.Trim())
                    Dim customerIdResult = cmdFind.ExecuteScalar()
                    If customerIdResult IsNot Nothing Then selectedCustomerId = Convert.ToInt32(customerIdResult)
                End Using
                MySqlConn.Close()
            Catch ex As Exception
            End Try
        End If

        If selectedCustomerId = 0 Then
            MessageBox.Show("Invalid Customer Selection. Please select a customer.")
            Return
        End If

        ' [NEW] Check if input amount exceeds total outstanding credit
        Dim totalOutstandingCredit As Double = 0
        Try
            If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()
            Dim sumSql As String = "SELECT SUM(amount) FROM customer_credit WHERE customer_id = @cid AND is_active = 1"
            Using cmdSum As New MySqlCommand(sumSql, MySqlConn)
                cmdSum.Parameters.AddWithValue("@cid", selectedCustomerId)
                Dim res = cmdSum.ExecuteScalar()
                If res IsNot Nothing AndAlso Not IsDBNull(res) Then
                    totalOutstandingCredit = Convert.ToDouble(res)
                End If
            End Using
        Catch ex As Exception
        Finally
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try

        If inputAmount > Math.Round(totalOutstandingCredit, 2) Then
            MessageBox.Show("enter amount greater than total credit")
            Return
        End If

        ' Cheque handling no longer returns early to allow invoice distribution
        ' (Previously returned early, now allowed to proceed to FIFO/Manual prompts)

        ' Ask for distribution method
        Dim result As DialogResult = MessageBox.Show("Do you want to clear credit in invoice order (FIFO)?", "Select Distribution Method", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)

        If result = DialogResult.Cancel Then Return

        Dim pType As String = ComboBox1.Text.Trim().ToLower()
        Dim isCheque As Boolean = (pType = "cheque" OrElse pType = "chaque")

        If result = DialogResult.Yes Then
            If isCheque Then
                PrepareFIFOForCheque(inputAmount)
            Else
                ApplyPaymentFIFO(inputAmount)
            End If
        Else
            ApplyPaymentManual(inputAmount)
        End If
    End Sub

    Private Sub ApplyPaymentFIFO(amount As Double)
        Dim transaction As MySqlTransaction = Nothing
        Dim paymentDate As String = DateTimePicker4.Value.ToString("yyyy-MM-dd HH:mm:ss")
        Try
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
            MySqlConn.Open()
            transaction = MySqlConn.BeginTransaction()

            ' 1. Get FIFO credit items
            Dim creditItems As New DataTable()
            Dim creditItemsSql As String = "SELECT id, amount, inv_no FROM customer_credit " &
                                           "WHERE customer_id = @cid AND amount > 0 AND is_active = 1 " &
                                           "ORDER BY id ASC"
            Using cmd As New MySqlCommand(creditItemsSql, MySqlConn, transaction)
                cmd.Parameters.AddWithValue("@cid", selectedCustomerId)
                Using adapter As New MySqlDataAdapter(cmd)
                    adapter.Fill(creditItems)
                End Using
            End Using

            If creditItems.Rows.Count = 0 Then
                Throw New Exception("No pending credits found for this customer.")
            End If

            Dim remainingAmount As Double = amount
            For Each row As DataRow In creditItems.Rows
                If remainingAmount <= 0 Then Exit For

                Dim invNo As String = row("inv_no").ToString()
                Dim currentCredit As Double = Convert.ToDouble(row("amount"))
                Dim applyAmt As Double = Math.Min(remainingAmount, currentCredit)

                Dim cId As Integer = Convert.ToInt32(row("id"))
                ApplyInvoicePayment(transaction, selectedCustomerId, NameTextBox1.Text, invNo, applyAmt, paymentDate, , , cId)
                remainingAmount -= applyAmt
            Next

            ' If there's still a balance leftover, it doesn't match an invoice (pure credit)
            If remainingAmount > 0 Then
                ApplyInvoicePayment(transaction, selectedCustomerId, NameTextBox1.Text, "CREDIT_ONLY", remainingAmount, paymentDate)
            End If

            transaction.Commit()
            MySqlConn.Close()
            MessageBox.Show("FIFO Payment Distribution Applied Successfully")
            RefreshAllGrids()

            ' No report or printing needed for Cash/Online payments here

            ClearPaymentInputs()

            ' For Cash, we're done. For Cheque, we never reach here because we branch earlier.
        Catch ex As Exception
            If transaction IsNot Nothing Then transaction.Rollback()
            MessageBox.Show("Error in FIFO payment: " & ex.Message)
        Finally
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Sub

    ' New helper for FIFO calculation without immediate database effect (for Cheques)
    Private Sub PrepareFIFOForCheque(amount As Double)
        currentDistribution.Clear()
        Dim remainingToApply As Double = amount
        Dim creditItems As New DataTable()

        Try
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
            MySqlConn.Open()
            Dim creditItemsSql As String = "SELECT id, amount, inv_no FROM customer_credit " &
                                           "WHERE customer_id = @cid AND amount > 0 AND is_active = 1 " &
                                           "ORDER BY id ASC"
            Using cmd As New MySqlCommand(creditItemsSql, MySqlConn)
                cmd.Parameters.AddWithValue("@cid", selectedCustomerId)
                Using adapter As New MySqlDataAdapter(cmd)
                    adapter.Fill(creditItems)
                End Using
            End Using

            For Each row As DataRow In creditItems.Rows
                If remainingToApply <= 0 Then Exit For
                Dim creditId As Integer = Convert.ToInt32(row("id"))
                Dim creditAmount As Double = Convert.ToDouble(row("amount"))
                Dim invNo As String = row("inv_no").ToString()
                Dim applyAmount As Double = Math.Min(remainingToApply, creditAmount)

                currentDistribution.Add(invNo, applyAmount)
                remainingToApply -= applyAmount
            Next

            ' If there's still a balance leftover, it doesn't match an invoice (pure credit)
            If remainingToApply > 0 Then
                currentDistribution.Add("CREDIT_ONLY", remainingToApply)
            End If

            ShowChequeEntry(amount)
        Catch ex As Exception
            MessageBox.Show("Error preparing FIFO for cheque: " & ex.Message)
        Finally
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Sub

    Private Sub ApplyPaymentManual(amount As Double)
        ' Open the Manual Distribution Popup, passing the current payment method
        Dim distributor As New ManualPaymentDistributor(selectedCustomerId, NameTextBox1.Text, amount, ComboBox1.Text)
        distributor.ShowDialog()

        If distributor.IsSuccess Then
            Dim pType As String = ComboBox1.Text.Trim().ToLower()
            If pType = "cheque" OrElse pType = "chaque" Then
                ' For Cheques: Store distribution and show cheque details (defer DB)
                currentDistribution.Clear()
                For Each entry In distributor.Distribution
                    currentDistribution.Add(entry.Key, entry.Value)
                Next
                ShowChequeEntry(amount)
            Else
                ' For Cash: Database transaction immediately
                Dim transaction As MySqlTransaction = Nothing
                Dim paymentDate As String = DateTimePicker4.Value.ToString("yyyy-MM-dd HH:mm:ss")
                Try
                    If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
                    MySqlConn.Open()
                    transaction = MySqlConn.BeginTransaction()

                    ' Apply each distributed amount
                    For Each entry In distributor.Distribution
                        ApplyInvoicePayment(transaction, selectedCustomerId, NameTextBox1.Text, entry.Key, entry.Value, paymentDate)
                    Next

                    transaction.Commit()
                    MySqlConn.Close()
                    MessageBox.Show("Manual Payment Distribution Applied Successfully")
                    RefreshAllGrids()

                    ' No report or printing needed for Cash/Online payments here

                    ClearPaymentInputs()
                Catch ex As Exception
                    If transaction IsNot Nothing Then transaction.Rollback()
                    MessageBox.Show("Error in Manual payment: " & ex.Message)
                Finally
                    If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
                End Try
            End If
        End If
    End Sub

    Private Sub ShowChequeEntry(amount As Double)
        ChqNoTextBox.Clear()
        BankSearchTextBox.Clear()
        selectedBankId = 0
        selectedBankName = ""
        LoadBanks()
        ChequeEntryPanel.Visible = True
        ChequeEntryPanel.BringToFront()
        ChqNoTextBox.Focus()
        ' Note: We don't return here as it's a sub, but the caller should stop processing if needed
    End Sub

    Private Sub ApplyInvoicePayment(transaction As MySqlTransaction, custId As Integer, custName As String, invNo As String, amount As Double, payDate As String, Optional chqNo As String = "", Optional bankId As Integer = 0, Optional creditId As Integer = 0)
        ' 1. Update customer_credit (Deducted immediately on payment receipt)
        If invNo = "CREDIT_ONLY" Then
            ' If it's a pure credit, we create a new credit record instead of reducing an existing one
            Dim insertSql As String = "INSERT INTO customer_credit (customer_id, customer_name, amount, is_active, timestamps) VALUES (@cid, @cus, @pay, 1, @dt)"
            Using cmd As New MySqlCommand(insertSql, MySqlConn, transaction)
                cmd.Parameters.AddWithValue("@cid", custId)
                cmd.Parameters.AddWithValue("@cus", custName)
                cmd.Parameters.AddWithValue("@pay", amount)
                cmd.Parameters.AddWithValue("@dt", payDate)
                cmd.ExecuteNonQuery()
            End Using
        ElseIf creditId > 0 Then
            Using cmd As New MySqlCommand("UPDATE customer_credit SET amount = amount - @pay WHERE id = @creid", MySqlConn, transaction)
                cmd.Parameters.AddWithValue("@pay", amount)
                cmd.Parameters.AddWithValue("@creid", creditId)
                cmd.ExecuteNonQuery()
            End Using
        Else
            Using cmd As New MySqlCommand("UPDATE customer_credit SET amount = amount - @pay WHERE inv_no = @inv AND customer_id = @cid", MySqlConn, transaction)
                cmd.Parameters.AddWithValue("@pay", amount)
                cmd.Parameters.AddWithValue("@inv", invNo)
                cmd.Parameters.AddWithValue("@cid", custId)
                cmd.ExecuteNonQuery()
            End Using
        End If

        ' 2. Insert into customer_payments (Always done to record the payment attempt)
        Dim paySql As String = "INSERT INTO customer_payments (CusID, Customer, PaymentType, Amount, Date, inv_no, cheque_no, bank_id) " &
                               "VALUES (@id, @cus, @type, @amt, @dt, @inv, @chq, @bank)"
        Using cmdPay As New MySqlCommand(paySql, MySqlConn, transaction)
            cmdPay.Parameters.AddWithValue("@id", custId)
            cmdPay.Parameters.AddWithValue("@cus", custName)
            cmdPay.Parameters.AddWithValue("@type", ComboBox1.Text)
            cmdPay.Parameters.AddWithValue("@amt", amount)
            cmdPay.Parameters.AddWithValue("@dt", payDate)
            cmdPay.Parameters.AddWithValue("@inv", If(invNo = "CREDIT_ONLY", "CREDIT", invNo))
            cmdPay.Parameters.AddWithValue("@chq", chqNo)
            cmdPay.Parameters.AddWithValue("@bank", bankId)
            cmdPay.ExecuteNonQuery()
        End Using

        ' 3. Update billing table (Deducted immediately on payment receipt)
        ' Log to Cash Transactions (Petty Cash Table)
        Dim isCash As Boolean = ComboBox1.Text.Trim().Equals("Cash", StringComparison.OrdinalIgnoreCase)
        If isCash Then
            Module1.RegisterCashTransaction(amount, "IN", "Customer Credit Pay: " & custName & " (Inv: " & invNo & ")", refNo:=invNo, customDate:=payDate)
        End If

        Dim billingSql As String = "SELECT id, balance_due, credit_balance_due, cheque_balance_due, paid_amount, status, partial_cash FROM billing " &
                                   "WHERE inv_no = @inv AND customer_id = @cid"
        Dim billingTable As New DataTable()
        Using cmdSelect As New MySqlCommand(billingSql, MySqlConn, transaction)
            cmdSelect.Parameters.AddWithValue("@inv", invNo)
            cmdSelect.Parameters.AddWithValue("@cid", custId)
            Using adapter As New MySqlDataAdapter(cmdSelect)
                adapter.Fill(billingTable)
            End Using
        End Using

        If billingTable.Rows.Count > 0 Then
            Dim row = billingTable.Rows(0)
            Dim billId As Integer = Convert.ToInt32(row("id"))
            Dim balanceDue As Double = If(IsDBNull(row("balance_due")), 0, Convert.ToDouble(row("balance_due")))
            Dim creditBalanceDue As Double = If(IsDBNull(row("credit_balance_due")), 0, Convert.ToDouble(row("credit_balance_due")))
            Dim chequeBalanceDue As Double = If(IsDBNull(row("cheque_balance_due")), 0, Convert.ToDouble(row("cheque_balance_due")))
            Dim paidAmount As Double = If(IsDBNull(row("paid_amount")), 0, Convert.ToDouble(row("paid_amount")))
            Dim partialCash As Double = If(IsDBNull(row("partial_cash")), 0, Convert.ToDouble(row("partial_cash")))

            Dim newCreditBalanceDue As Double = Math.Round(Math.Max(0, creditBalanceDue - amount), 2)
            Dim newBalanceDue As Double = Math.Round(Math.Max(chequeBalanceDue, balanceDue - amount), 2)
            Dim newPaidAmount As Double = Math.Round(paidAmount + amount, 2)

            Dim updateBillingSql As String = "UPDATE billing SET " &
                                           "balance_due = @balance, " &
                                           "credit_balance_due = @crd_bal, " &
                                           "paid_amount = @paid, " &
                                           "collector_user_id = @collector, " &
                                           "status = IF(@balance <= 0.00 AND cheque_balance_due <= 0.00, 'success', " &
                                           "  CASE " &
                                           "    WHEN partial_cash > 0 AND cheque_balance_due > 0 AND @crd_bal > 0 THEN 'Mixed_Payment' " &
                                           "    WHEN partial_cash > 0 AND cheque_balance_due = 0 AND @crd_bal > 0 THEN 'Cash_Credit' " &
                                           "    WHEN partial_cash > 0 AND cheque_balance_due > 0 AND @crd_bal = 0 THEN 'Cash_Cheque' " &
                                           "    WHEN partial_cash = 0 AND cheque_balance_due > 0 AND @crd_bal > 0 THEN 'Credit_Cheque' " &
                                           "    WHEN partial_cash = 0 AND cheque_balance_due = 0 AND @crd_bal > 0 THEN 'Credit' " &
                                           "    WHEN partial_cash = 0 AND cheque_balance_due > 0 AND @crd_bal = 0 THEN 'Cheque' " &
                                           "    ELSE status " &
                                           "  END) " &
                                           "WHERE id = @id"
            Using cmdUpdate As New MySqlCommand(updateBillingSql, MySqlConn, transaction)
                cmdUpdate.Parameters.AddWithValue("@balance", newBalanceDue)
                cmdUpdate.Parameters.AddWithValue("@crd_bal", newCreditBalanceDue)
                cmdUpdate.Parameters.AddWithValue("@paid", newPaidAmount)
                cmdUpdate.Parameters.AddWithValue("@collector", If(Module1.CurrentUserID > 0, Module1.CurrentUserID, DBNull.Value))
                cmdUpdate.Parameters.AddWithValue("@id", billId)
                cmdUpdate.ExecuteNonQuery()
            End Using
        End If
    End Sub


    Private Sub ClearPaymentInputs()
        AmountTextBox1.Clear()
        NameTextBox1.Clear()
        selectedCustomerId = 0
        creid = 0
        CreditAmt = 0
        TelNoTextBox.Clear()
        DateTimePicker4.Value = DateTime.Now
    End Sub
    Private Sub Customer_creditDataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles Customer_creditDataGridView1.CellClick
        If e.RowIndex >= 0 Then
            Dim row As DataGridViewRow = Customer_creditDataGridView1.Rows(e.RowIndex)

            ' Set the credit ID
            If Customer_creditDataGridView1.Columns.Contains("CreId") Then
                creid = Convert.ToInt32(row.Cells("CreId").Value)
            Else
                creid = 0
            End If

            ' Set the customer name
            isExistingPaymentSelected = False ' Selecting from credit list is for a new payment
            isSettingNameProgrammatically = True
            NameTextBox1.Text = If(row.Cells("CusName").Value?.ToString(), "")
            isSettingNameProgrammatically = False

            ' Set the Tel No
            If Customer_creditDataGridView1.Columns.Contains("CusTel") Then
                TelNoTextBox.Text = If(row.Cells("CusTel").Value?.ToString(), "")
            Else
                TelNoTextBox.Clear()
            End If

            ' Set credit amount
            CreditAmt = 0
            If Customer_creditDataGridView1.Columns.Contains("Credit_Amount") Then
                Dim val = row.Cells("Credit_Amount").Value
                If val IsNot Nothing AndAlso Not IsDBNull(val) Then
                    CreditAmt = Convert.ToDouble(val)
                End If
            End If
            Label18.Text = CreditAmt.ToString("N2")

            ' Set the customer ID (selectedCustomerId) strictly
            If Customer_creditDataGridView1.Columns.Contains("CusId") Then
                selectedCustomerId = Convert.ToInt32(row.Cells("CusId").Value)
            End If

            ' Reset creid as we are now selecting a customer, not a specific credit row
            creid = 0

            ' Load invoices for this customer to ensure billing sync
            If selectedCustomerId > 0 Then
                LoadCustomerInvoices(selectedCustomerId)
            End If

            ' No focus hijack here - allow editing interaction if needed
        End If
    End Sub

    Private Sub Customer_creditDataGridView1_KeyDown(sender As Object, e As KeyEventArgs) Handles Customer_creditDataGridView1.KeyDown
        If e.KeyCode = Keys.Enter Then
            Dim itm As Integer = Customer_creditDataGridView1.CurrentRow.Index
            Dim row As DataGridViewRow = Customer_creditDataGridView1.Rows(itm)

            ' Reset and set ID from specific column names
            creid = 0
            selectedCustomerId = 0

            ' Find the Customer ID (CusId)
            If Customer_creditDataGridView1.Columns.Contains("CusId") Then
                Dim val = row.Cells("CusId").Value
                If val IsNot Nothing AndAlso Not IsDBNull(val) Then
                    selectedCustomerId = Convert.ToInt32(val)
                End If
            End If

            ' Find the Customer Name (CusName)
            isExistingPaymentSelected = False
            isSettingNameProgrammatically = True
            If Customer_creditDataGridView1.Columns.Contains("CusName") Then
                NameTextBox1.Text = If(row.Cells("CusName").Value?.ToString(), "")
            End If
            isSettingNameProgrammatically = False

            ' Find the Tel No
            If Customer_creditDataGridView1.Columns.Contains("CusTel") Then
                TelNoTextBox.Text = If(row.Cells("CusTel").Value?.ToString(), "")
            Else
                TelNoTextBox.Clear()
            End If

            ' Set the Credit Amount
            CreditAmt = 0
            If Customer_creditDataGridView1.Columns.Contains("Credit_Amount") Then
                Dim val = row.Cells("Credit_Amount").Value
                If val IsNot Nothing AndAlso Not IsDBNull(val) Then
                    CreditAmt = Convert.ToDouble(val)
                End If
            End If
            Label18.Text = CreditAmt.ToString("N2")

            ' Final checks and resets
            ComboBox1.Select()
            Panel1.Visible = False

            e.Handled = True
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        If isSettingComboBoxProgrammatically Then Return
        ' Confirmation prompt removed based on user feedback to streamline the selection process
    End Sub

    Private Sub ComboBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles ComboBox1.KeyDown
        If e.KeyCode = Keys.Enter Then
            AmountTextBox1.Select()

        End If
    End Sub
    Private Sub AmountTextBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles AmountTextBox1.KeyDown
        If e.Shift AndAlso e.KeyCode = Keys.Enter Then
            ComboBox1.Select()
            e.Handled = True
            e.SuppressKeyPress = True
        ElseIf e.KeyCode = Keys.Enter Then
            pay_save.PerformClick()
        End If
    End Sub
    ' Removed redundant validation - validation happens in pay_save_Click

    Private Sub pay_cancel_Click(sender As Object, e As EventArgs) Handles pay_cancel.Click
        NameTextBox1.Clear()
        AmountTextBox1.Clear()
        creid = 0
        selectedCustomerId = 0
        CreditAmt = 0
        TelNoTextBox.Clear()
        isExistingPaymentSelected = False
        DateTimePicker4.Value = DateTime.Now
        Panel1.Visible = False
        pay_cancel.Visible = False
        ApplyPaymentFilters()
    End Sub



    Private Sub DataGridView1_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellDoubleClick
        If e.RowIndex >= 0 Then
            Dim row As DataGridViewRow = DataGridView1.Rows(e.RowIndex)
            Dim invNo As String = If(row.Cells("inv_no").Value IsNot Nothing, row.Cells("inv_no").Value.ToString(), "")
            
            If Not String.IsNullOrEmpty(invNo) Then
                Dim billingId As String = ""
                Try
                    If conn.State = ConnectionState.Open Then conn.Close()
                    conn.Open()
                    Using cmd As New MySqlCommand("SELECT id FROM billing WHERE inv_no = @inv LIMIT 1", conn)
                        cmd.Parameters.AddWithValue("@inv", invNo)
                        Dim res = cmd.ExecuteScalar()
                        If res IsNot Nothing AndAlso res IsNot DBNull.Value Then
                            billingId = res.ToString()
                        End If
                    End Using
                    
                    If String.IsNullOrEmpty(billingId) Then
                        Using cmd As New MySqlCommand("SELECT id FROM quotation_billing WHERE inv_no = @inv LIMIT 1", conn)
                            cmd.Parameters.AddWithValue("@inv", invNo)
                            Dim res = cmd.ExecuteScalar()
                            If res IsNot Nothing AndAlso res IsNot DBNull.Value Then
                                billingId = res.ToString()
                            End If
                        End Using
                    End If
                    conn.Close()
                Catch ex As Exception
                    If conn.State = ConnectionState.Open Then conn.Close()
                End Try
                
                If Not String.IsNullOrEmpty(billingId) Then
                    Dim mainStart As Start = TryCast(Me.MdiParent, Start)
                    If mainStart IsNot Nothing Then
                        Dim tempSalesForm As TempSales = Nothing
                        For Each child As Form In mainStart.MdiChildren
                            If TypeOf child Is TempSales Then
                                tempSalesForm = DirectCast(child, TempSales)
                                Exit For
                            End If
                        Next
                        
                        If tempSalesForm Is Nothing Then
                            tempSalesForm = New TempSales()
                            mainStart.OpenMdiForm(tempSalesForm)
                        Else
                            tempSalesForm.BringToFront()
                        End If
                        
                        tempSalesForm.LoadInvoiceForEditing(billingId, invNo)
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        If e.RowIndex >= 0 AndAlso DataGridView1.Columns(e.ColumnIndex).Name = "btnWA" Then
            Dim row As DataGridViewRow = DataGridView1.Rows(e.RowIndex)
            Dim phoneNo As String = If(row.Cells("CusTel").Value?.ToString(), "")
            OpenWhatsAppForPhone(phoneNo)
        End If
    End Sub

    Private Sub OpenWhatsAppForPhone(phoneNo As String)
        If String.IsNullOrEmpty(phoneNo) Then
            MessageBox.Show("No phone number found for this customer.", "Empty Phone Number", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        
        ' Clean phone number
        phoneNo = phoneNo.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "")
        If phoneNo.Contains("/") Then
            phoneNo = phoneNo.Split("/"c)(0).Trim()
        End If
        
        If String.IsNullOrEmpty(phoneNo) Then
            MessageBox.Show("Please enter a valid phone number first.", "Empty Phone Number", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        
        If phoneNo.StartsWith("0") Then
            phoneNo = "94" & phoneNo.Substring(1)
        ElseIf Not phoneNo.StartsWith("94") AndAlso phoneNo.Length = 9 Then
            phoneNo = "94" & phoneNo
        End If
        
        Try
            Dim waUrl As String = "whatsapp://send?phone=" & phoneNo
            System.Diagnostics.Process.Start(New System.Diagnostics.ProcessStartInfo(waUrl) With {.UseShellExecute = True})
        Catch ex As Exception
            MessageBox.Show("Error opening WhatsApp: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub pay_del_Click(sender As Object, e As EventArgs) Handles pay_del.Click
        If Not IsSecureKeyValid1() Then Exit Sub

        Dim result As DialogResult = MessageBox.Show("Are you Sure to Delete This", "OR Not", MessageBoxButtons.YesNo)

        If result = DialogResult.Yes Then
            Dim sel As Integer = CustomerPaymentsView.CurrentRow.Index
            ' Get the values to identify the record (using composite key)
            Dim cusId As Integer
            Dim customer As String
            Dim paymentType As String
            Dim amount As Double
            Dim payDate As DateTime
            Dim invNo As String = ""

            Dim chqNo As String = ""
            Dim chqStatus As String = ""

            Try
                ' Robustly get values by column name (WinForms usually names them same as field names from DataSource)
                ' If names fail, we could fallback to indexes, but LoadPayments sets names.
                cusId = Convert.ToInt32(CustomerPaymentsView.Rows(sel).Cells("CusID").Value)
                customer = If(CustomerPaymentsView.Rows(sel).Cells("Customer").Value, "").ToString()
                paymentType = If(CustomerPaymentsView.Rows(sel).Cells("PaymentType").Value, "").ToString()
                amount = Convert.ToDouble(If(CustomerPaymentsView.Rows(sel).Cells("Amount").Value, 0))
                payDate = Convert.ToDateTime(CustomerPaymentsView.Rows(sel).Cells("Date").Value)
                invNo = If(CustomerPaymentsView.Rows(sel).Cells("inv_no").Value IsNot Nothing AndAlso Not IsDBNull(CustomerPaymentsView.Rows(sel).Cells("inv_no").Value), CustomerPaymentsView.Rows(sel).Cells("inv_no").Value.ToString().Trim(), "")
                chqNo = If(CustomerPaymentsView.Rows(sel).Cells("cheque_no").Value IsNot Nothing AndAlso Not IsDBNull(CustomerPaymentsView.Rows(sel).Cells("cheque_no").Value), CustomerPaymentsView.Rows(sel).Cells("cheque_no").Value.ToString().Trim(), "")
                chqStatus = If(CustomerPaymentsView.Rows(sel).Cells("ChqStatus").Value IsNot Nothing AndAlso Not IsDBNull(CustomerPaymentsView.Rows(sel).Cells("ChqStatus").Value), CustomerPaymentsView.Rows(sel).Cells("ChqStatus").Value.ToString().Trim(), "")

                Dim bankId As Integer = 0
                If CustomerPaymentsView.Columns.Contains("bank_id") Then
                    bankId = Convert.ToInt32(If(CustomerPaymentsView.Rows(sel).Cells("bank_id").Value, 0))
                End If

                If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
                MySqlConn.Open()
                Dim transaction = MySqlConn.BeginTransaction()

                Try
                    ' Reverse billing and customer_credit (only if not cheque OR if cleared cheque)
                    Dim isCheque As Boolean = (paymentType.Trim().ToLower() = "chaque" OrElse paymentType.Trim().ToLower() = "cheque")
                    Dim isCleared As Boolean = chqStatus.Trim().Equals("Cleared", StringComparison.OrdinalIgnoreCase)

                    If Not isCheque OrElse isCleared Then
                        ' If it's a cleared cheque, we also need to set its status back to Pending in check_received
                        If isCheque AndAlso Not String.IsNullOrEmpty(chqNo) Then
                            Dim resetChqSql As String = "UPDATE check_received SET status = 'Pending' WHERE check_number = @chq AND bank_id = @bid AND IFNULL(inv_no, '') = IFNULL(@inv, '')"
                            Using cmd As New MySqlCommand(resetChqSql, MySqlConn, transaction)
                                cmd.Parameters.AddWithValue("@chq", chqNo)
                                cmd.Parameters.AddWithValue("@bid", bankId)
                                cmd.Parameters.AddWithValue("@inv", invNo)
                                cmd.ExecuteNonQuery()
                            End Using
                        End If

                        If Not String.IsNullOrEmpty(invNo) Then
                            ' Reverse strictly for the specific invoice
                            Dim revCreditSql As String = "UPDATE customer_credit SET amount = amount + @amt WHERE inv_no = @inv AND customer_id = @cid"
                            Using cmd As New MySqlCommand(revCreditSql, MySqlConn, transaction)
                                cmd.Parameters.AddWithValue("@amt", amount)
                                cmd.Parameters.AddWithValue("@inv", invNo)
                                cmd.Parameters.AddWithValue("@cid", cusId)
                                cmd.ExecuteNonQuery()
                            End Using

                            ' Revert billing balances and status
                            Dim revBillingSql As String = "UPDATE billing SET " &
                                                         "balance_due = balance_due + @amt, " &
                                                         "credit_balance_due = credit_balance_due + @amt, " &
                                                         "paid_amount = paid_amount - @amt, " &
                                                         "status = IF((balance_due + @amt) <= 0 AND cheque_balance_due <= 0, 'success', " &
                                                         "  CASE " &
                                                         "    WHEN partial_cash > 0 AND cheque_balance_due > 0 AND (credit_balance_due + @amt) > 0 THEN 'Mixed_Payment' " &
                                                         "    WHEN partial_cash > 0 AND cheque_balance_due = 0 AND (credit_balance_due + @amt) > 0 THEN 'Cash_Credit' " &
                                                         "    WHEN partial_cash > 0 AND cheque_balance_due > 0 AND (credit_balance_due + @amt) = 0 THEN 'Cheque' " &
                                                         "    WHEN partial_cash = 0 AND cheque_balance_due > 0 AND (credit_balance_due + @amt) > 0 THEN 'Credit_Cheque' " &
                                                         "    WHEN partial_cash = 0 AND cheque_balance_due = 0 AND (credit_balance_due + @amt) > 0 THEN 'Credit' " &
                                                         "    WHEN partial_cash = 0 AND cheque_balance_due > 0 AND (credit_balance_due + @amt) = 0 THEN 'Cheque' " &
                                                         "    ELSE status " &
                                                         "  END) " &
                                                         "WHERE inv_no = @inv AND customer_id = @cid"
                            Using cmd As New MySqlCommand(revBillingSql, MySqlConn, transaction)
                                cmd.Parameters.AddWithValue("@amt", amount)
                                cmd.Parameters.AddWithValue("@inv", invNo)
                                cmd.Parameters.AddWithValue("@cid", cusId)
                                cmd.ExecuteNonQuery()
                            End Using
                        Else
                            ' Reverse FIFO application (LIFO logic)

                            ' Reverse Credit (LIFO)
                            Dim credSql As String = "SELECT id FROM customer_credit WHERE customer_id = @cid AND is_active = 1 ORDER BY timestamps DESC"
                            Dim dtCreds As New DataTable()
                            Using cmd As New MySqlCommand(credSql, MySqlConn, transaction)
                                cmd.Parameters.AddWithValue("@cid", cusId)
                                Using adapter As New MySqlDataAdapter(cmd)
                                    adapter.Fill(dtCreds)
                                End Using
                            End Using

                            If dtCreds.Rows.Count > 0 Then
                                ' Add back to the most recent credit record
                                Dim cId As Integer = Convert.ToInt32(dtCreds.Rows(0)("id"))
                                Dim updCredSql As String = "UPDATE customer_credit SET amount = amount + @amt WHERE id = @id"
                                Using cmd As New MySqlCommand(updCredSql, MySqlConn, transaction)
                                    cmd.Parameters.AddWithValue("@amt", amount)
                                    cmd.Parameters.AddWithValue("@id", cId)
                                    cmd.ExecuteNonQuery()
                                End Using
                            End If

                            ' Reverse Billing (LIFO)
                            Dim remBillingAmt As Double = amount
                            Dim billSql As String = "SELECT id, balance_due, credit_balance_due, paid_amount FROM billing WHERE customer_id = @cid AND paid_amount > 0 ORDER BY timestamps DESC"
                            Dim dtBills As New DataTable()
                            Using cmd As New MySqlCommand(billSql, MySqlConn, transaction)
                                cmd.Parameters.AddWithValue("@cid", cusId)
                                Using adapter As New MySqlDataAdapter(cmd)
                                    adapter.Fill(dtBills)
                                End Using
                            End Using

                            For Each row As DataRow In dtBills.Rows
                                If remBillingAmt <= 0 Then Exit For
                                Dim bId As Integer = Convert.ToInt32(row("id"))
                                Dim bDue As Double = Convert.ToDouble(row("balance_due"))
                                Dim cDue As Double = If(IsDBNull(row("credit_balance_due")), 0, Convert.ToDouble(row("credit_balance_due")))
                                Dim pAmt As Double = Convert.ToDouble(row("paid_amount"))

                                Dim amtToRest As Double = Math.Min(remBillingAmt, pAmt)
                                Dim newBal As Double = bDue + amtToRest
                                Dim newCrdBal As Double = cDue + amtToRest
                                Dim newPaid As Double = pAmt - amtToRest

                                ' Recalculate status using the same logic as payment
                                Dim updBillSql As String = "UPDATE billing SET " &
                                                             "balance_due = @bal, " &
                                                             "credit_balance_due = @crd_bal, " &
                                                             "paid_amount = @paid, " &
                                                             "status = IF(@bal <= 0 AND cheque_balance_due <= 0, 'success', " &
                                                             "  CASE " &
                                                             "    WHEN partial_cash > 0 AND cheque_balance_due > 0 AND @crd_bal > 0 THEN 'Mixed_Payment' " &
                                                             "    WHEN partial_cash > 0 AND cheque_balance_due = 0 AND @crd_bal > 0 THEN 'Cash_Credit' " &
                                                             "    WHEN partial_cash > 0 AND cheque_balance_due > 0 AND @crd_bal = 0 THEN 'Cheque' " &
                                                             "    WHEN partial_cash = 0 AND cheque_balance_due > 0 AND @crd_bal > 0 THEN 'Credit_Cheque' " &
                                                             "    WHEN partial_cash = 0 AND cheque_balance_due = 0 AND @crd_bal > 0 THEN 'Credit' " &
                                                             "    WHEN partial_cash = 0 AND cheque_balance_due > 0 AND @crd_bal = 0 THEN 'Cheque' " &
                                                             "    ELSE status " &
                                                             "  END) " &
                                                             "WHERE id = @id"
                                Using cmd As New MySqlCommand(updBillSql, MySqlConn, transaction)
                                    cmd.Parameters.AddWithValue("@bal", newBal)
                                    cmd.Parameters.AddWithValue("@crd_bal", newCrdBal)
                                    cmd.Parameters.AddWithValue("@paid", newPaid)
                                    cmd.Parameters.AddWithValue("@id", bId)
                                    cmd.ExecuteNonQuery()
                                End Using
                                remBillingAmt -= amtToRest
                            Next
                        End If
                    End If

                    ' Delete using a more robust query to avoid date precision issues
                    Dim Query As String
                    If isCheque AndAlso Not String.IsNullOrEmpty(chqNo) Then
                        ' For cheques, use cheque_no
                        Query = "DELETE FROM customer_payments WHERE CusID=@cusid AND cheque_no=@chq AND Amount=@amt"
                    Else
                        ' For cash, use composite key with a slightly more flexible date match (within 1 second)
                        Query = "DELETE FROM customer_payments WHERE CusID=@cusid AND Customer=@cus AND Amount=@amt AND ABS(TIMESTAMPDIFF(SECOND, Date, @dt)) <= 1"
                    End If

                    Dim affected As Integer = 0
                    Using cmd As New MySqlCommand(Query, MySqlConn, transaction)
                        cmd.Parameters.AddWithValue("@cusid", cusId)
                        cmd.Parameters.AddWithValue("@cus", customer)
                        cmd.Parameters.AddWithValue("@amt", amount)
                        cmd.Parameters.AddWithValue("@dt", payDate) ' Pass DateTime object directly
                        cmd.Parameters.AddWithValue("@chq", chqNo)
                        affected = cmd.ExecuteNonQuery()
                    End Using

                    If affected = 0 Then
                        ' If still not found, try one last time with exact date as string (legacy fallback)
                        Dim fallbackQuery As String = "DELETE FROM customer_payments WHERE CusID=@cusid AND Customer=@cus AND Amount=@amt AND Date=@dt"
                        Using cmd As New MySqlCommand(fallbackQuery, MySqlConn, transaction)
                            cmd.Parameters.AddWithValue("@cusid", cusId)
                            cmd.Parameters.AddWithValue("@cus", customer)
                            cmd.Parameters.AddWithValue("@amt", amount)
                            cmd.Parameters.AddWithValue("@dt", payDate.ToString("yyyy-MM-dd HH:mm:ss"))
                            affected = cmd.ExecuteNonQuery()
                        End Using
                    End If
 
                    Dim chqDeleted As Boolean = False
                    If isCheque AndAlso affected > 0 AndAlso Not String.IsNullOrEmpty(chqNo) Then
                        Dim deleteChqSql As String = "DELETE FROM check_received WHERE check_number = @chq AND bank_id = @bid AND status = 'Pending'"
                        Using cmdDelChq As New MySqlCommand(deleteChqSql, MySqlConn, transaction)
                            cmdDelChq.Parameters.AddWithValue("@chq", chqNo)
                            cmdDelChq.Parameters.AddWithValue("@bid", bankId)
                            If cmdDelChq.ExecuteNonQuery() > 0 Then
                                chqDeleted = True
                            End If
                        End Using
                    End If

                    If affected < 1 Then
                        ' Log but don't critical fail if the row is effectively already gone or mismatched
                        ' throw New Exception("...") is too aggressive for production UI if it rolls back reversal
                    End If

                    transaction.Commit()
                    MySqlConn.Close()

                    ' Centralized System log deletion
                    Module1.LogDeletion("Customer Payment", cusId.ToString(), "Customer Name: " & customer & ", Amount: " & amount & ", Type: " & paymentType & ", Inv No: " & invNo & ", Date: " & payDate.ToString("yyyy-MM-dd HH:mm:ss"))
                    If chqDeleted Then
                        Module1.LogDeletion("Customer Cheque", chqNo, "Deleted during Payment Reversal. Bank ID: " & bankId & ", Amount: " & amount & ", Customer: " & customer & ", Inv No: " & invNo)
                    End If

                    ' Force UI to update properties
                    selectedCustomerId = cusId

                    ' Refresh UI and grids
                    RefreshAllGrids()

                    Application.DoEvents()
                    MessageBox.Show("Payment deleted successfully")

                Catch exTrans As Exception
                    If transaction IsNot Nothing Then transaction.Rollback()
                    Throw exTrans
                End Try

            Catch ex As Exception
                MessageBox.Show("Delete Error: " & ex.Message)
                If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
            End Try
        End If
    End Sub
    'this part for the debit Add'
    Dim CreIds As Integer
    Private Sub loaddebit()
        Try
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Open()

            Dim table As New DataTable()
            Dim sql As String = "SELECT cc.id AS CreId, c.name AS CusName, cc.amount AS Credit_Amount, cc.timestamps AS CreditDate, c.id AS CusId, c.tel_no AS CusTel, cc.inv_no " &
                            "FROM customer_credit cc " &
                            "INNER JOIN customer c ON cc.customer_id = c.id WHERE cc.is_active = 1"

            Dim adapter As New MySqlDataAdapter(sql, conn)
            adapter.Fill(table)

            DataGridView1.DataSource = table
            DataGridView1.RowHeadersVisible = False

            ' Apply filtering and formatting
            ApplyDebitFilters()

        Catch ex As Exception
            MessageBox.Show("Error in loaddebit: " & ex.Message)
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub ApplyDebitFilters()
        Try
            If DataGridView1.DataSource Is Nothing Then Return

            Dim table As DataTable = Nothing

            ' Robustly get the underlying DataTable
            If TypeOf DataGridView1.DataSource Is DataView Then
                table = DirectCast(DataGridView1.DataSource, DataView).Table
            ElseIf TypeOf DataGridView1.DataSource Is DataTable Then
                table = DirectCast(DataGridView1.DataSource, DataTable)
            ElseIf TypeOf DataGridView1.DataSource Is BindingSource Then
                Dim bsource = DirectCast(DataGridView1.DataSource, BindingSource)
                If TypeOf bsource.DataSource Is DataTable Then
                    table = DirectCast(bsource.DataSource, DataTable)
                ElseIf TypeOf bsource.DataSource Is DataView Then
                    table = DirectCast(bsource.DataSource, DataView).Table
                End If
            End If

            If table Is Nothing Then Return

            Dim dv As New DataView(table)
            Dim filterStrings As New List(Of String)

            ' Filter by name search box
            If Not String.IsNullOrEmpty(filter_name.Text) Then
                filterStrings.Add(String.Format("CusName Like '{0}%'", filter_name.Text.Replace("'", "''")))
            End If

            ' Filter by phone number search box
            If Not String.IsNullOrEmpty(filter_phone.Text) Then
                filterStrings.Add(String.Format("CusTel Like '{0}%'", filter_phone.Text.Replace("'", "''")))
            End If

            ' Filter by invoice number search box
            If Not String.IsNullOrEmpty(filter_invoice.Text) Then
                filterStrings.Add(String.Format("inv_no Like '{0}%'", filter_invoice.Text.Replace("'", "''")))
            End If

            ' Status filter via cmbCreditFilter1
            If cmbCreditFilter1.Text = "Pending" Then
                filterStrings.Add("Credit_Amount > 0")
            ElseIf cmbCreditFilter1.Text = "Paid" Then
                filterStrings.Add("Credit_Amount = 0")
            End If

            ' Filter by details date range picker
            Dim startStr As String = dtpDetailsStart.Value.ToString("yyyy-MM-dd 00:00:00")
            Dim endStr As String = dtpDetailsEnd.Value.ToString("yyyy-MM-dd 23:59:59")
            filterStrings.Add(String.Format("CreditDate >= #{0}# AND CreditDate <= #{1}#", startStr, endStr))

            dv.RowFilter = String.Join(" AND ", filterStrings)
            dv.Sort = "CusName ASC, CreditDate DESC"
            DataGridView1.DataSource = dv
            FormatCreditsGrid(DataGridView1, True)

            ' Calculate total for filtered results
            Dim tot As Double = 0
            For Each row As DataGridViewRow In DataGridView1.Rows
                If Not row.IsNewRow Then
                    Dim cellValue = row.Cells("Credit_Amount").Value
                    If cellValue IsNot Nothing AndAlso Not IsDBNull(cellValue) Then
                        Dim tempVal As Double = 0
                        If Double.TryParse(cellValue.ToString(), tempVal) Then
                            tot += tempVal
                        End If
                    End If
                End If
            Next
            Label16.Text = tot.ToString("N2")
        Catch ex As Exception
            ' Silent fail during typing
        End Try
    End Sub
    Private Sub DAddNewBtn_Click(sender As Object, e As EventArgs) Handles DAddNewBtn.Click
        DAmountTxt.Clear()
        DCustomerNameTxt.Clear()
        DCustomerTelTxt.Clear()
        DCustomerInvTxt.Clear()
        creid = 0 ' Reset ID so we know it's a new entry
        selectedCustomerId = 0
        DateTimePicker3.Value = DateTime.Now
        Panel3.Visible = False
        DCustomerNameTxt.Select()
        
        ' Unlock for new entry
        DAmountTxt.ReadOnly = False
        DCustomerInvTxt.ReadOnly = False
        DSaveBtn.Enabled = True
    End Sub
    Private Sub DSaveBtn_Click(sender As Object, e As EventArgs) Handles DSaveBtn.Click
        If DCustomerNameTxt.Text = "" Or DAmountTxt.Text = "" Or DCustomerInvTxt.Text = "" Then
            MsgBox("Please Enter Customer Name, Amount, and Invoice Number")
            Return
        End If

        Dim creditDate As String = Format(Me.DateTimePicker3.Value, "yyyy-MM-dd HH:mm:ss")
        Dim transaction As MySqlTransaction = Nothing

        Try
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
            MySqlConn.Open()
            transaction = MySqlConn.BeginTransaction()

            Dim sql As String = ""
            If creid > 0 Then
                MsgBox("To update an existing record, please use the Update button and provide the secure key.")
                transaction.Rollback()
                Return
            Else
                ' INSERT new record - Use selectedCustomerId directly to avoid duplicates for same name
                If selectedCustomerId <= 0 Then
                    ' Try to find customer by exact name match if they typed it manually
                    Dim findIdCmd As New MySqlCommand("SELECT id FROM customer WHERE name = @name", MySqlConn, transaction)
                    findIdCmd.Parameters.AddWithValue("@name", DCustomerNameTxt.Text.Trim())
                    Dim result = findIdCmd.ExecuteScalar()
                    If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                        selectedCustomerId = Convert.ToInt32(result)
                    Else
                        Throw New Exception("Customer name not found in database. Please select a valid customer from the suggestion list or type the name exactly as it appears in the database.")
                    End If
                End If

                ' Duplicate check: check if same customer, invoice and amount already exists
                Dim checkSql As String = "SELECT COUNT(*) FROM customer_credit WHERE customer_id = @uid AND inv_no = @inv AND amount = @amount AND is_active = 1"
                Using cmdCheck As New MySqlCommand(checkSql, MySqlConn, transaction)
                    cmdCheck.Parameters.AddWithValue("@uid", selectedCustomerId)
                    cmdCheck.Parameters.AddWithValue("@inv", DCustomerInvTxt.Text.Trim())
                    cmdCheck.Parameters.AddWithValue("@amount", DAmountTxt.Text.Trim())
                    Dim count As Integer = Convert.ToInt32(cmdCheck.ExecuteScalar())
                    If count > 0 Then
                        MsgBox("already have that")
                        transaction.Rollback()
                        Return
                    End If
                End Using

                sql = "INSERT INTO customer_credit (amount, customer_id, timestamps, is_active, inv_no) " &
                      "VALUES (@amount, @uid, @date, 1, @inv)"
            End If

            Using cmd As New MySqlCommand(sql, MySqlConn, transaction)
                If creid > 0 Then
                    cmd.Parameters.AddWithValue("@creid", creid)
                Else
                    cmd.Parameters.AddWithValue("@uid", selectedCustomerId)
                End If
                cmd.Parameters.AddWithValue("@amount", DAmountTxt.Text)
                cmd.Parameters.AddWithValue("@date", creditDate)
                cmd.Parameters.AddWithValue("@inv", DCustomerInvTxt.Text.Trim())

                cmd.ExecuteNonQuery()
            End Using

            ' Synchronization with billing table
            If Not String.IsNullOrEmpty(DCustomerInvTxt.Text) Then
                Dim grandTotal As Double = 0
                Dim fetchSql As String = "SELECT grand_total FROM billing WHERE inv_no = @inv AND customer_id = @cid"
                Using cmdFetch As New MySqlCommand(fetchSql, MySqlConn, transaction)
                    cmdFetch.Parameters.AddWithValue("@inv", DCustomerInvTxt.Text.Trim())
                    cmdFetch.Parameters.AddWithValue("@cid", selectedCustomerId)
                    Dim result = cmdFetch.ExecuteScalar()
                    If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                        grandTotal = Convert.ToDouble(result)

                        Dim creditEntryAmt As Double = 0
                        Double.TryParse(DAmountTxt.Text, creditEntryAmt)
                        Dim newTotalBalance As Double = 0 ' (Removed incorrect formula) ' This formula was incorrect.

                        ' Correct way: 
                        ' grandTotal (Original Total)
                        ' paid (Total paid so far, excluding the change we are making now?)
                        ' No, let's use the columns already in the table.

                        ' Update billing including timestamps. Status recalculates based on new total.
                        Dim billingUpdateSql As String = "UPDATE billing SET " &
                                                       "credit_balance_due = @crd_bal, " &
                                                       "balance_due = cheque_balance_due + @crd_bal, " &
                                                       "paid_amount = grand_total - (cheque_balance_due + @crd_bal), " &
                                                       "status = IF((cheque_balance_due + @crd_bal) <= 0 AND cheque_balance_due <= 0, 'success', " &
                                                        "  CASE " &
                                                        "    WHEN partial_cash > 0 AND cheque_balance_due > 0 AND @crd_bal > 0 THEN 'Mixed_Payment' " &
                                                        "    WHEN partial_cash > 0 AND cheque_balance_due = 0 AND @crd_bal > 0 THEN 'Cash_Credit' " &
                                                        "    WHEN partial_cash > 0 AND cheque_balance_due > 0 AND @crd_bal = 0 THEN 'Cheque' " &
                                                        "    WHEN partial_cash = 0 AND cheque_balance_due > 0 AND @crd_bal > 0 THEN 'Credit_Cheque' " &
                                                        "    WHEN partial_cash = 0 AND cheque_balance_due = 0 AND @crd_bal > 0 THEN 'Credit' " &
                                                        "    WHEN partial_cash = 0 AND cheque_balance_due > 0 AND @crd_bal = 0 THEN 'Cheque' " &
                                                        "    ELSE status " &
                                                        "  END), " &
                                                       "timestamps = @date WHERE inv_no = @inv AND customer_id = @cid"

                        Using cmdBilling As New MySqlCommand(billingUpdateSql, MySqlConn, transaction)
                            cmdBilling.Parameters.AddWithValue("@crd_bal", Math.Round(creditEntryAmt, 2))
                            cmdBilling.Parameters.AddWithValue("@date", creditDate)
                            cmdBilling.Parameters.AddWithValue("@inv", DCustomerInvTxt.Text.Trim())
                            cmdBilling.Parameters.AddWithValue("@cid", selectedCustomerId)
                            cmdBilling.ExecuteNonQuery()
                        End Using
                    Else
                        ' Insert a dummy billing record so that Crystal Reports INNER JOIN works for manual credits
                        Dim creditEntryAmt As Double = 0
                        Double.TryParse(DAmountTxt.Text, creditEntryAmt)
                        
                        Dim insertDummySql As String = "INSERT INTO billing (inv_no, printed_inv_no, customer_id, subtotal, grand_total, credit_balance_due, balance_due, status, timestamps, inv_type, billing_type, payment_type, user_id, order_user_id, collector_user_id, po_number, vat_id, bank_id) " &
                                                       "VALUES (@inv, @inv, @cid, @amount, @amount, @amount, @amount, 'Credit', @date, 'Manual Credit', 'Credit', 'Credit', @uid, @uid, @uid, '', 1, 1)"
                        Using cmdDummy As New MySqlCommand(insertDummySql, MySqlConn, transaction)
                            cmdDummy.Parameters.AddWithValue("@inv", DCustomerInvTxt.Text.Trim())
                            cmdDummy.Parameters.AddWithValue("@cid", selectedCustomerId)
                            cmdDummy.Parameters.AddWithValue("@amount", Math.Round(creditEntryAmt, 2))
                            cmdDummy.Parameters.AddWithValue("@date", creditDate)
                            
                            Dim uId As Integer = 101
                            If ComboBox3.SelectedValue IsNot Nothing AndAlso Integer.TryParse(ComboBox3.SelectedValue.ToString(), uId) Then
                                ' Used selected user
                            ElseIf Module1.CurrentUserID > 0 Then
                                uId = Module1.CurrentUserID
                            End If
                            cmdDummy.Parameters.AddWithValue("@uid", uId)
                            
                            cmdDummy.ExecuteNonQuery()
                        End Using
                    End If
                End Using
            End If

            transaction.Commit()
            MySqlConn.Close()

            MsgBox(If(creid > 0, "Updated Successfully", "Saved Successfully"))

            ' Refresh UI
            RefreshAllGrids()

            DCustomerNameTxt.Clear()
            DAmountTxt.Clear()
            DCustomerTelTxt.Clear()
            DCustomerInvTxt.Clear()
            Panel2.Visible = False
            Panel3.Visible = False
            creid = 0 ' Important: Reset after successful save
            
            ' Unlock fields for next entry
            DAmountTxt.ReadOnly = False
            DCustomerInvTxt.ReadOnly = False
            DCustomerNameTxt.Select()

        Catch ex As Exception
            If transaction IsNot Nothing AndAlso MySqlConn.State = ConnectionState.Open Then
                Try
                    transaction.Rollback()
                Catch exRoll As Exception
                End Try
            End If
            MessageBox.Show("Error: " & ex.Message)
        Finally
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Sub
    Private Sub DDeleteBtn_Click(sender As Object, e As EventArgs) Handles DDeleteBtn.Click

        If creid = 0 Then
            MsgBox("Please select a record from the list first.")
            Exit Sub
        End If

        If Not IsSecureKeyValid() Then Exit Sub

        Dim result As DialogResult = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

        If result = DialogResult.Yes Then
            Try

                If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
                MySqlConn.Open()


                Dim Query As String = "DELETE FROM `customer_credit` WHERE `id` = @id;"

                Using cmd As New MySqlCommand(Query, MySqlConn)
                    cmd.Parameters.AddWithValue("@id", creid)


                    Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                    If rowsAffected > 0 Then
                        ' Centralized System log deletion
                        Module1.LogDeletion("Customer Credit", creid.ToString(), "Customer: " & DCustomerNameTxt.Text & ", Inv No: " & DCustomerInvTxt.Text & ", Amount: " & DAmountTxt.Text)

                        ' Delete dummy billing record if it exists
                        Dim delBillingSql As String = "DELETE FROM billing WHERE inv_no = @inv AND inv_type = 'Manual Credit'"
                        Using cmdDelBilling As New MySqlCommand(delBillingSql, MySqlConn)
                            cmdDelBilling.Parameters.AddWithValue("@inv", DCustomerInvTxt.Text.Trim())
                            cmdDelBilling.ExecuteNonQuery()
                        End Using

                        MsgBox("Record Deleted Successfully")
                    Else
                        MsgBox("Delete failed. Record might not exist.")
                    End If
                End Using

                MySqlConn.Close()


                RefreshAllGrids()


                DCustomerNameTxt.Clear()
                DAmountTxt.Clear()
                DCustomerTelTxt.Clear()
                DCustomerInvTxt.Clear()
                creid = 0
                Panel3.Visible = False
                DCustomerNameTxt.Select()

            Catch ex As Exception
                MessageBox.Show("Error: " & ex.Message)
            Finally
                If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
            End Try
        End If
    End Sub
    Private Sub print_Click(sender As Object, e As EventArgs) Handles print.Click
        Try
            SyncMissingManualCredits()
            Dim rptDoc As New CUSTOMER_CREDIT()

            ' Target table and field identification logic (enhanced for ID support)
            Dim tableName As String = "customer"
            Dim nameFieldName As String = "name"
            Dim idFieldName As String = "id"
            Dim foundTable As Boolean = False
            Dim selectedIdVal As Integer = 0
            Dim filterName As String = ""

            ' 1. Identify selected values from UI
            If DataGridView1.CurrentRow IsNot Nothing Then
                Try
                    Dim idCellVal As Object = Nothing
                    If DataGridView1.Columns.Contains("CusId") Then
                        idCellVal = DataGridView1.CurrentRow.Cells("CusId").Value
                    ElseIf DataGridView1.Columns.Contains("id") Then
                        idCellVal = DataGridView1.CurrentRow.Cells("id").Value
                    ElseIf DataGridView1.Columns.Count > 0 Then
                        idCellVal = DataGridView1.CurrentRow.Cells(0).Value
                    End If

                    If idCellVal IsNot Nothing AndAlso Not IsDBNull(idCellVal) Then
                        Integer.TryParse(idCellVal.ToString(), selectedIdVal)
                    End If
                Catch
                End Try
            End If

            If selectedIdVal = 0 Then
                If Not String.IsNullOrEmpty(DCustomerNameTxt.Text.Trim()) Then
                    filterName = DCustomerNameTxt.Text.Trim()
                ElseIf Not String.IsNullOrEmpty(filter_name.Text.Trim()) Then
                    filterName = filter_name.Text.Trim()
                End If
            End If

            ' 1st Priority: Look for exact 'customer' table
            For Each tbl As Table In rptDoc.Database.Tables
                If tbl.Name.Equals("customer", StringComparison.OrdinalIgnoreCase) Then
                    tableName = tbl.Name
                    nameFieldName = "name"
                    idFieldName = "id"
                    foundTable = True
                    Exit For
                End If
            Next

            ' 2nd Priority: Look for 'Command'
            If Not foundTable Then
                For Each tbl As Table In rptDoc.Database.Tables
                    If tbl.Name.Equals("Command", StringComparison.OrdinalIgnoreCase) Then
                        tableName = tbl.Name
                        nameFieldName = "name"
                        idFieldName = "customer_id" ' Common for command-based aliases
                        foundTable = True
                        Exit For
                    End If
                Next
            End If

            ' 3rd Priority: Case-insensitive 'customer' but NOT 'credit'
            If Not foundTable Then
                For Each tbl As Table In rptDoc.Database.Tables
                    If tbl.Name.ToLower().Contains("customer") AndAlso Not tbl.Name.ToLower().Contains("credit") Then
                        tableName = tbl.Name
                        nameFieldName = "name"
                        idFieldName = "id"
                        foundTable = True
                        Exit For
                    End If
                Next
            End If

            ' Fallback: First table containing 'customer'
            If Not foundTable Then
                For Each tbl As Table In rptDoc.Database.Tables
                    If tbl.Name.ToLower().Contains("customer") Then
                        tableName = tbl.Name
                        If tableName.ToLower().Contains("credit") Then
                            nameFieldName = "customer_name"
                            idFieldName = "customer_id"
                        Else
                            nameFieldName = "name"
                            idFieldName = "id"
                        End If
                        foundTable = True
                        Exit For
                    End If
                Next
            End If

            ' Final Fallback: First table
            If Not foundTable AndAlso rptDoc.Database.Tables.Count > 0 Then
                tableName = rptDoc.Database.Tables(0).Name
            End If

            ' Apply filter: Prioritize ID, then Name
            Dim formula As String = ""
            If selectedIdVal > 0 Then
                formula = "{" & tableName & "." & idFieldName & "} = " & selectedIdVal
            ElseIf Not String.IsNullOrEmpty(filterName) Then
                formula = "Trim(UpperCase({" & tableName & "." & nameFieldName & "})) = '" & filterName.ToUpper().Replace("'", "''") & "'"
            End If

            ' --- [NEW] Credit Status Filter ---
            ' Identify active status filter and credit table
            Dim currentStatusFilter As String = "All"
            If TabControl1.SelectedTab Is TabPage1 Then
                currentStatusFilter = cmbCreditFilter2.Text
            Else
                currentStatusFilter = cmbCreditFilter1.Text
            End If

            Dim creditTableForStatus As String = tableName
            If Not creditTableForStatus.ToLower().Contains("credit") Then
                For Each tbl As Table In rptDoc.Database.Tables
                    If tbl.Name.ToLower().Contains("credit") Then
                        creditTableForStatus = tbl.Name
                        Exit For
                    End If
                Next
            End If

            Dim statusFormula As String = ""
            If currentStatusFilter = "Pending" Then
                statusFormula = "{" & creditTableForStatus & ".amount} > 0"
            ElseIf currentStatusFilter = "Paid" Then
                statusFormula = "{" & creditTableForStatus & ".amount} = 0"
            End If

            If Not String.IsNullOrEmpty(statusFormula) Then
                If Not String.IsNullOrEmpty(formula) Then
                    formula &= " AND " & statusFormula
                Else
                    formula = statusFormula
                End If
            End If

            ' --- [NEW] Invoice Filter ---
            Dim currentInvFilter As String = ""
            If TabControl1.SelectedTab Is TabPage2 Then
                currentInvFilter = filter_invoice.Text.Trim()
            End If

            If Not String.IsNullOrEmpty(currentInvFilter) Then
                Dim invFormula As String = "{" & creditTableForStatus & ".inv_no} LIKE '*" & currentInvFilter.Replace("'", "''") & "*'"
                If Not String.IsNullOrEmpty(formula) Then
                    formula &= " AND " & invFormula
                Else
                    formula = invFormula
                End If
            End If
            ' ----------------------------

            rptDoc.RecordSelectionFormula = formula
            ' -----------------------------------

            ' Hand over to SaleInv for centralized display/printing
            SaleInv.ShowReport(rptDoc, 6)

        Catch ex As Exception
            MessageBox.Show("Error generating report: " & ex.Message, "Report Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            If CustomerPaymentsView.CurrentRow Is Nothing Then
                MessageBox.Show("Please select a payment record to print.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim row = CustomerPaymentsView.CurrentRow

            Dim rptDoc As New customerpaymentnote()

            ' Extract values for filtering
            Dim cusId As Integer = 0
            If CustomerPaymentsView.Columns.Contains("CusID") AndAlso row.Cells("CusID").Value IsNot Nothing AndAlso Not IsDBNull(row.Cells("CusID").Value) Then
                Integer.TryParse(row.Cells("CusID").Value.ToString(), cusId)
            End If

            Dim amount As Double = 0
            If CustomerPaymentsView.Columns.Contains("Amount") AndAlso row.Cells("Amount").Value IsNot Nothing AndAlso Not IsDBNull(row.Cells("Amount").Value) Then
                Double.TryParse(row.Cells("Amount").Value.ToString(), amount)
            End If

            Dim payDate As DateTime
            Dim hasDate As Boolean = False
            If CustomerPaymentsView.Columns.Contains("Date") AndAlso row.Cells("Date").Value IsNot Nothing AndAlso Not IsDBNull(row.Cells("Date").Value) Then
                If DateTime.TryParse(row.Cells("Date").Value.ToString(), payDate) Then
                    hasDate = True
                End If
            End If

            Dim targetTable As Table = Nothing
            Dim tableName As String = "customer_payments"
            
            If rptDoc.Database.Tables.Count > 0 Then
                For Each tbl As Table In rptDoc.Database.Tables
                    If tbl.Name.ToLower().Contains("payment") OrElse tbl.Name.ToLower().Contains("command") Then
                        targetTable = tbl
                        Exit For
                    End If
                Next
                
                If targetTable Is Nothing Then
                    targetTable = rptDoc.Database.Tables(0)
                End If
            End If

            Dim hasCusId As Boolean = False
            Dim hasCustomerId As Boolean = False
            Dim hasAmount As Boolean = False
            Dim hasDateFld As Boolean = False

            If targetTable IsNot Nothing Then
                tableName = targetTable.Name
                For Each fld As DatabaseFieldDefinition In targetTable.Fields
                    Dim fName As String = fld.Name.ToLower()
                    If fName = "cusid" Then hasCusId = True
                    If fName = "customer_id" Then hasCustomerId = True
                    If fName = "amount" Then hasAmount = True
                    If fName = "date" Then hasDateFld = True
                Next
            End If

            Dim conditions As New List(Of String)
            If cusId > 0 Then
                If hasCusId AndAlso hasCustomerId Then
                    conditions.Add("({" & tableName & ".CusID} = " & cusId & " OR {" & tableName & ".customer_id} = " & cusId & ")")
                ElseIf hasCusId Then
                    conditions.Add("{" & tableName & ".CusID} = " & cusId)
                ElseIf hasCustomerId Then
                    conditions.Add("{" & tableName & ".customer_id} = " & cusId)
                End If
            End If

            If conditions.Count > 0 Then
                rptDoc.RecordSelectionFormula = String.Join(" AND ", conditions)
            End If

            SaleInv.ShowReport(rptDoc, 6)

        Catch ex As Exception
            MessageBox.Show("Error generating report: " & ex.Message, "Report Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub SyncMissingManualCredits()
        Try
            Using localConn As New MySqlConnection(Module1.ConnStr)
                localConn.Open()
                
                Dim sql As String = "SELECT cc.customer_id, cc.inv_no, cc.amount, cc.timestamps " &
                                    "FROM customer_credit cc " &
                                    "LEFT JOIN billing b ON cc.inv_no = b.inv_no AND cc.customer_id = b.customer_id " &
                                    "WHERE b.inv_no IS NULL AND cc.is_active = 1 AND cc.inv_no IS NOT NULL AND cc.inv_no <> ''"
                
                Dim dt As New DataTable()
                Using cmd As New MySqlCommand(sql, localConn)
                    Using da As New MySqlDataAdapter(cmd)
                        da.Fill(dt)
                    End Using
                End Using

                If dt.Rows.Count > 0 Then
                    Dim transaction = localConn.BeginTransaction()
                    Try
                        For Each row As DataRow In dt.Rows
                            Dim inv As String = row("inv_no").ToString()
                            Dim cid As Integer = Convert.ToInt32(row("customer_id"))
                            Dim amt As Double = Convert.ToDouble(row("amount"))
                            Dim ts As DateTime = Convert.ToDateTime(row("timestamps"))
                            
                            Dim insertDummySql As String = "INSERT INTO billing (inv_no, printed_inv_no, customer_id, subtotal, grand_total, credit_balance_due, balance_due, status, timestamps, inv_type, billing_type, payment_type, user_id, order_user_id, collector_user_id, po_number, vat_id, bank_id) " &
                                                           "VALUES (@inv, @inv, @cid, @amount, @amount, @amount, @amount, 'Credit', @date, 'Manual Credit', 'Credit', 'Credit', @uid, @uid, @uid, '', 1, 1)"
                            
                            Using cmdIns As New MySqlCommand(insertDummySql, localConn, transaction)
                                cmdIns.Parameters.AddWithValue("@inv", inv)
                                cmdIns.Parameters.AddWithValue("@cid", cid)
                                cmdIns.Parameters.AddWithValue("@amount", amt)
                                cmdIns.Parameters.AddWithValue("@date", ts)
                                
                                Dim uId As Integer = If(Module1.CurrentUserID > 0, Module1.CurrentUserID, 101)
                                cmdIns.Parameters.AddWithValue("@uid", uId)
                                
                                cmdIns.ExecuteNonQuery()
                            End Using
                        Next
                        transaction.Commit()
                    Catch ex As Exception
                        transaction.Rollback()
                    End Try
                End If
            End Using
        Catch ex As Exception
        End Try
    End Sub

    Private Sub DCustomerNameTxt_TextChanged(sender As Object, e As EventArgs) Handles DCustomerNameTxt.TextChanged
        If isSettingNameProgrammatically Then Return

        Try
            MySqlConn.Open()
            Dim bsource As New BindingSource
            Dim table As New DataTable
            Dim adapter As New MySqlDataAdapter("SELECT name, tel_no, id 
    FROM customer 
    WHERE is_block = 0", MySqlConn)
            adapter.Fill(table)
            bsource.DataSource = table
            CustomerDataGridView.DataSource = table
            Dim dv As New DataView(table)
            dv.RowFilter = String.Format("name Like '{0}%'", DCustomerNameTxt.Text.Replace("'", "''"))
            dv.Sort = "name ASC"
            CustomerDataGridView.DataSource = dv
            FormatSuggestionGrid(CustomerDataGridView)

            ' Show panel only if we have data and user has started typing
            If dv.Count > 0 AndAlso DCustomerNameTxt.Text.Trim() <> "" Then
                Panel2.Visible = True
                Panel2.BringToFront()
            Else
                Panel2.Visible = False
            End If

            MySqlConn.Close()
        Catch ex As Exception
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Sub
    Private Sub CustomerDataGridView_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles CustomerDataGridView.CellClick
        If e.RowIndex < 0 Then Return
        Dim b As Integer = e.RowIndex
        Dim cellValue = CustomerDataGridView.Rows(b).Cells(2).Value
        If cellValue IsNot Nothing AndAlso Not IsDBNull(cellValue) Then
            selectedCustomerId = Convert.ToInt32(cellValue)
        Else
            selectedCustomerId = 0
        End If
        isSettingNameProgrammatically = True
        DCustomerNameTxt.Text = CustomerDataGridView.Rows(b).Cells(0).Value.ToString
        isSettingNameProgrammatically = False
        DCustomerTelTxt.Text = If(CustomerDataGridView.Rows(b).Cells(1).Value?.ToString(), "")
        LoadCustomerInvoices(selectedCustomerId)
        If Panel3.Visible Then
            InvoiceDataGridView.Select()
        Else
            DAmountTxt.Select()
        End If
        Panel2.Visible = False
    End Sub
    Private Sub CustomerDataGridView_KeyDown(sender As Object, e As KeyEventArgs) Handles CustomerDataGridView.KeyDown
        If e.KeyCode = Keys.Enter Then
            Dim b As Integer = CustomerDataGridView.CurrentRow.Index
            Dim cellValue = CustomerDataGridView.Rows(b).Cells(2).Value
            If cellValue IsNot Nothing AndAlso Not IsDBNull(cellValue) Then
                selectedCustomerId = Convert.ToInt32(cellValue)
            Else
                selectedCustomerId = 0
            End If
            isSettingNameProgrammatically = True
            DCustomerNameTxt.Text = CustomerDataGridView.Rows(b).Cells(0).Value.ToString
            isSettingNameProgrammatically = False
            DCustomerTelTxt.Text = If(CustomerDataGridView.Rows(b).Cells(1).Value?.ToString(), "")
            LoadCustomerInvoices(selectedCustomerId)
            If Panel3.Visible Then
                InvoiceDataGridView.Select()
            Else
                DAmountTxt.Select()
            End If

            Panel2.Visible = False
        End If
    End Sub
    Private Sub DCustomerNameTxt_KeyDown(sender As Object, e As KeyEventArgs) Handles DCustomerNameTxt.KeyDown
        If e.KeyCode = Keys.Escape Then
            Panel2.Visible = False
        End If
        If e.KeyCode = Keys.Up Or e.KeyCode = Keys.Down Then
            If Panel2.Visible Then
                CustomerDataGridView.Select()
            End If
        End If
        If e.KeyCode = Keys.Enter Then
            If Panel2.Visible AndAlso CustomerDataGridView.Rows.Count > 0 Then
                ' Select the first row from visible panel
                Dim b As Integer = 0
                Dim cellValue = CustomerDataGridView.Rows(b).Cells(2).Value
                If cellValue IsNot Nothing AndAlso Not IsDBNull(cellValue) Then
                    selectedCustomerId = Convert.ToInt32(cellValue)
                Else
                    selectedCustomerId = 0
                End If
                isSettingNameProgrammatically = True
                DCustomerNameTxt.Text = CustomerDataGridView.Rows(b).Cells(0).Value.ToString
                isSettingNameProgrammatically = False
                DCustomerTelTxt.Text = If(CustomerDataGridView.Rows(b).Cells(1).Value?.ToString(), "")
                LoadCustomerInvoices(selectedCustomerId)
                If Panel3.Visible Then
                    InvoiceDataGridView.Select()
                Else
                    DAmountTxt.Select()
                End If
                Panel2.Visible = False

                ' Prevent ding sound
                e.Handled = True
                e.SuppressKeyPress = True
            ElseIf Not Panel2.Visible AndAlso DCustomerNameTxt.Text.Trim() <> "" Then
                ' Fallback: Try to find a match even if panel is hidden
                Try
                    If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
                    MySqlConn.Open()
                    Dim cmd As New MySqlCommand("SELECT name, tel_no, id FROM customer WHERE name = @name AND is_block = 0 LIMIT 1", MySqlConn)
                    cmd.Parameters.AddWithValue("@name", DCustomerNameTxt.Text.Trim())
                    Dim reader As MySqlDataReader = cmd.ExecuteReader()
                    If reader.Read() Then
                        selectedCustomerId = reader.GetInt32("id")
                        isSettingNameProgrammatically = True
                        DCustomerNameTxt.Text = reader.GetString("name")
                        isSettingNameProgrammatically = False
                        DCustomerTelTxt.Text = If(reader.IsDBNull(reader.GetOrdinal("tel_no")), "", reader.GetString("tel_no"))
                        reader.Close()
                        LoadCustomerInvoices(selectedCustomerId)
                        If Panel3.Visible Then
                            InvoiceDataGridView.Select()
                        Else
                            DAmountTxt.Select()
                        End If
                        e.Handled = True
                        e.SuppressKeyPress = True
                    End If
                    reader.Close()
                Catch ex As Exception
                Finally
                    If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
                End Try
            End If
        End If
    End Sub
    Private Sub DCustomerNameTxt_KeyPress(sender As Object, e As KeyPressEventArgs) Handles DCustomerNameTxt.KeyPress
        ' Visibility handled in TextChanged
    End Sub

    ' Invoice selection handlers
    Private Sub InvoiceDataGridView_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles InvoiceDataGridView.CellClick
        If e.RowIndex >= 0 Then
            Dim row As DataGridViewRow = InvoiceDataGridView.Rows(e.RowIndex)
            DCustomerInvTxt.Text = If(row.Cells("inv_no").Value?.ToString(), "")
            DAmountTxt.Text = If(row.Cells("credit_balance_due").Value?.ToString(), "")
            Panel3.Visible = False
            ' Hide cancel button if suggestion panel also hidden
            DAmountTxt.Select()
        End If
    End Sub

    Private Sub InvoiceDataGridView_KeyDown(sender As Object, e As KeyEventArgs) Handles InvoiceDataGridView.KeyDown
        If e.KeyCode = Keys.Enter Then
            If InvoiceDataGridView.CurrentRow IsNot Nothing Then
                Dim row As DataGridViewRow = InvoiceDataGridView.CurrentRow
                DCustomerInvTxt.Text = If(row.Cells("inv_no").Value?.ToString(), "")
                DAmountTxt.Text = If(row.Cells("credit_balance_due").Value?.ToString(), "")
                Panel3.Visible = False
                ' Hide cancel button if suggestion panel also hidden
                DAmountTxt.Select()
                e.Handled = True
                e.SuppressKeyPress = True
            End If
        End If
    End Sub

    Private Sub DInvoiceStatusCombo_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DInvoiceStatusCombo.SelectedIndexChanged
        If selectedCustomerId > 0 Then
            LoadCustomerInvoices(selectedCustomerId)
        End If
    End Sub

    Private Sub cmbCreditFilter1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbCreditFilter1.SelectedIndexChanged
        ApplyDebitFilters()
    End Sub

    Private Sub cmbCreditFilter2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbCreditFilter2.SelectedIndexChanged
        ApplyCreditFilters()
    End Sub

    Private Sub DAmountTxt_KeyPress(sender As Object, e As KeyPressEventArgs) Handles DAmountTxt.KeyPress
        ' Restrict input to numbers and a single decimal point
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) AndAlso (e.KeyChar <> "."c) Then
            e.Handled = True
        End If

        ' Only allow one decimal point
        If (e.KeyChar = "."c) AndAlso (DirectCast(sender, TextBox).Text.IndexOf("."c) > -1) Then
            e.Handled = True
        End If
    End Sub

    Private Sub DAmountTxt_KeyDown(sender As Object, e As KeyEventArgs) Handles DAmountTxt.KeyDown
        If e.KeyCode = Keys.Enter Then
            DCustomerInvTxt.Select()
            e.Handled = True
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub DCustomerInvTxt_KeyDown(sender As Object, e As KeyEventArgs) Handles DCustomerInvTxt.KeyDown
        If e.KeyCode = Keys.Enter Then
            DSaveBtn.PerformClick()
            e.Handled = True
            e.SuppressKeyPress = True
        ElseIf e.KeyCode = Keys.Down Then
            If selectedCustomerId > 0 Then
                LoadCustomerInvoices(selectedCustomerId)
                Panel3.Visible = True
                Panel3.BringToFront()
                InvoiceDataGridView.Focus()
            End If
            e.Handled = True
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub DCustomerInvTxt_KeyPress(sender As Object, e As KeyPressEventArgs) Handles DCustomerInvTxt.KeyPress
        If e.KeyChar = Microsoft.VisualBasic.ChrW(Keys.Enter) Then
            e.Handled = True ' Suppress "ding"
        End If
    End Sub


    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        ' Safety check to ensure a valid row was clicked
        If e.RowIndex >= 0 Then
            Dim b As Integer = e.RowIndex

            If DataGridView1.Rows(b).Cells("CreId").Value IsNot Nothing AndAlso Not IsDBNull(DataGridView1.Rows(b).Cells("CreId").Value) Then
                creid = Convert.ToInt32(DataGridView1.Rows(b).Cells("CreId").Value)
            End If
            
            isSettingNameProgrammatically = True
            DCustomerNameTxt.Text = If(DataGridView1.Rows(b).Cells("CusName").Value?.ToString(), "")
            isSettingNameProgrammatically = False

            DAmountTxt.Text = If(DataGridView1.Rows(b).Cells("Credit_Amount").Value?.ToString(), "")
            If DataGridView1.Rows(b).Cells("CusId").Value IsNot Nothing AndAlso Not IsDBNull(DataGridView1.Rows(b).Cells("CusId").Value) Then
                selectedCustomerId = Convert.ToInt32(DataGridView1.Rows(b).Cells("CusId").Value)
            End If
            DCustomerTelTxt.Text = If(DataGridView1.Rows(b).Cells("CusTel").Value?.ToString(), "")

            ' Capture inv_no and set it in the text box (to prioritize it)
            If DataGridView1.Columns.Contains("inv_no") Then
                DCustomerInvTxt.Text = If(DataGridView1.Rows(b).Cells("inv_no").Value?.ToString(), "")
            End If

            LoadCustomerInvoices(selectedCustomerId)

            ' Hide suggestion panels on single click to keep UI clean
            Panel2.Visible = False
            Panel3.Visible = False

            ' If you want to show the date in the picker:
            If DataGridView1.Rows(b).Cells("CreditDate").Value IsNot Nothing AndAlso Not IsDBNull(DataGridView1.Rows(b).Cells("CreditDate").Value) Then
                DateTimePicker3.Value = Convert.ToDateTime(DataGridView1.Rows(b).Cells("CreditDate").Value)
            End If
        End If
    End Sub

    Private Sub DataGridView1_KeyDown(sender As Object, e As KeyEventArgs) Handles DataGridView1.KeyDown
        If e.KeyCode = Keys.Enter Then
            If DataGridView1.CurrentRow IsNot Nothing Then
                Dim row As DataGridViewRow = DataGridView1.CurrentRow

                ' Index 0 = id (CreId), Index 1 = name (DCustomerNameTxt), Index 2 = amount (DAmountTxt), Index 3 = timestamps (CreditDate)
                If row.Cells("CreId").Value IsNot Nothing AndAlso Not IsDBNull(row.Cells("CreId").Value) Then
                    creid = Convert.ToInt32(row.Cells("CreId").Value)
                End If

                isSettingNameProgrammatically = True
                DCustomerNameTxt.Text = If(row.Cells("CusName").Value?.ToString(), "")
                isSettingNameProgrammatically = False

                DAmountTxt.Text = If(row.Cells("Credit_Amount").Value?.ToString(), "")
                selectedCustomerId = Convert.ToInt32(row.Cells("CusId").Value)
                DCustomerTelTxt.Text = If(row.Cells("CusTel").Value?.ToString(), "")

                ' Capture inv_no and set it in the text box
                If DataGridView1.Columns.Contains("inv_no") Then
                    DCustomerInvTxt.Text = If(row.Cells("inv_no").Value?.ToString(), "")
                End If

                LoadCustomerInvoices(selectedCustomerId)

                If row.Cells("CreditDate").Value IsNot Nothing AndAlso Not IsDBNull(row.Cells("CreditDate").Value) Then
                    DateTimePicker3.Value = Convert.ToDateTime(row.Cells("CreditDate").Value)
                End If

                ' Prevent editing existing records (removed to allow typing new values before Update)

                e.Handled = True
                e.SuppressKeyPress = True
            End If
        End If
    End Sub

    Private Sub filter_name_TextChanged(sender As Object, e As EventArgs) Handles filter_name.TextChanged
        ApplyDebitFilters()
    End Sub
    Private Sub filter_phone_TextChanged(sender As Object, e As EventArgs) Handles filter_phone.TextChanged
        ApplyDebitFilters()
    End Sub
    Private Sub filter_invoice_TextChanged(sender As Object, e As EventArgs) Handles filter_invoice.TextChanged
        ApplyDebitFilters()
    End Sub
    Private Sub filter_name_KeyDown(sender As Object, e As KeyEventArgs) Handles filter_name.KeyDown
        If e.KeyCode = Keys.Enter Then
            If DataGridView1.Rows.Count > 0 Then
                ' Get the first visible row (after filter)
                Dim b As Integer = 0

                ' Standardized indices: 0=CreId, 1=CusName, 2=Credit_Amount
                If DataGridView1.Rows(b).Cells(0).Value IsNot Nothing AndAlso Not IsDBNull(DataGridView1.Rows(b).Cells(0).Value) Then
                    creid = Convert.ToInt32(DataGridView1.Rows(b).Cells(0).Value)
                End If

                ' Note: CellClick uses 1 for Amount and 2 for Name, let's fix that consistency
                ' Based on SQL: cc.id AS CreId, c.name AS CusName, cc.amount AS Credit_Amount
                DCustomerNameTxt.Text = If(DataGridView1.Rows(b).Cells(1).Value?.ToString(), "")
                DAmountTxt.Text = If(DataGridView1.Rows(b).Cells(2).Value?.ToString(), "")

                ' Load invoices for the selected customer
                If DataGridView1.Columns.Contains("CusId") AndAlso DataGridView1.Rows(b).Cells("CusId").Value IsNot Nothing AndAlso Not IsDBNull(DataGridView1.Rows(b).Cells("CusId").Value) Then
                    selectedCustomerId = Convert.ToInt32(DataGridView1.Rows(b).Cells("CusId").Value)
                    LoadCustomerInvoices(selectedCustomerId)
                    If Panel3.Visible Then
                        InvoiceDataGridView.Select()
                    Else
                        DAmountTxt.Select()
                    End If
                Else
                    DAmountTxt.Select()
                End If
            End If
        ElseIf e.KeyCode = Keys.Up OrElse e.KeyCode = Keys.Down Then
            If DataGridView1.Rows.Count > 0 Then
                DataGridView1.Focus()
                If DataGridView1.CurrentCell Is Nothing Then
                    For Each col As DataGridViewColumn In DataGridView1.Columns
                        If col.Visible Then
                            DataGridView1.CurrentCell = DataGridView1.Rows(0).Cells(col.Index)
                            Exit For
                        End If
                    Next
                End If
                e.Handled = True
                e.SuppressKeyPress = True
            End If
        End If
    End Sub

    Private Sub filter_phone_KeyDown(sender As Object, e As KeyEventArgs) Handles filter_phone.KeyDown
        If e.KeyCode = Keys.Enter Then
            If DataGridView1.Rows.Count > 0 Then
                Dim b As Integer = 0
                If DataGridView1.Rows(b).Cells("CreId").Value IsNot Nothing AndAlso Not IsDBNull(DataGridView1.Rows(b).Cells("CreId").Value) Then
                    creid = Convert.ToInt32(DataGridView1.Rows(b).Cells("CreId").Value)
                End If
                DCustomerNameTxt.Text = If(DataGridView1.Rows(b).Cells("CusName").Value?.ToString(), "")
                DAmountTxt.Text = If(DataGridView1.Rows(b).Cells("Credit_Amount").Value?.ToString(), "")
                
                ' Load invoices for the selected customer
                If DataGridView1.Columns.Contains("CusId") AndAlso DataGridView1.Rows(b).Cells("CusId").Value IsNot Nothing AndAlso Not IsDBNull(DataGridView1.Rows(b).Cells("CusId").Value) Then
                    selectedCustomerId = Convert.ToInt32(DataGridView1.Rows(b).Cells("CusId").Value)
                    LoadCustomerInvoices(selectedCustomerId)
                    If Panel3.Visible Then
                        InvoiceDataGridView.Select()
                    Else
                        DAmountTxt.Select()
                    End If
                Else
                    DAmountTxt.Select()
                End If
            End If
        ElseIf e.KeyCode = Keys.Up OrElse e.KeyCode = Keys.Down Then
            If DataGridView1.Rows.Count > 0 Then
                DataGridView1.Focus()
                If DataGridView1.CurrentCell Is Nothing Then
                    For Each col As DataGridViewColumn In DataGridView1.Columns
                        If col.Visible Then
                            DataGridView1.CurrentCell = DataGridView1.Rows(0).Cells(col.Index)
                            Exit For
                        End If
                    Next
                End If
                e.Handled = True
                e.SuppressKeyPress = True
            End If
        End If
    End Sub

    Private Sub filter_invoice_KeyDown(sender As Object, e As KeyEventArgs) Handles filter_invoice.KeyDown
        If e.KeyCode = Keys.Enter Then
            If DataGridView1.Rows.Count > 0 Then
                Dim b As Integer = 0
                If DataGridView1.Rows(b).Cells("CreId").Value IsNot Nothing AndAlso Not IsDBNull(DataGridView1.Rows(b).Cells("CreId").Value) Then
                    creid = Convert.ToInt32(DataGridView1.Rows(b).Cells("CreId").Value)
                End If
                DCustomerNameTxt.Text = If(DataGridView1.Rows(b).Cells("CusName").Value?.ToString(), "")
                DAmountTxt.Text = If(DataGridView1.Rows(b).Cells("Credit_Amount").Value?.ToString(), "")
                
                ' Load invoices for the selected customer
                If DataGridView1.Columns.Contains("CusId") AndAlso DataGridView1.Rows(b).Cells("CusId").Value IsNot Nothing AndAlso Not IsDBNull(DataGridView1.Rows(b).Cells("CusId").Value) Then
                    selectedCustomerId = Convert.ToInt32(DataGridView1.Rows(b).Cells("CusId").Value)
                    LoadCustomerInvoices(selectedCustomerId)
                    If Panel3.Visible Then
                        InvoiceDataGridView.Select()
                    Else
                        DAmountTxt.Select()
                    End If
                Else
                    DAmountTxt.Select()
                End If
            End If
        ElseIf e.KeyCode = Keys.Up OrElse e.KeyCode = Keys.Down Then
            If DataGridView1.Rows.Count > 0 Then
                DataGridView1.Focus()
                If DataGridView1.CurrentCell Is Nothing Then
                    For Each col As DataGridViewColumn In DataGridView1.Columns
                        If col.Visible Then
                            DataGridView1.CurrentCell = DataGridView1.Rows(0).Cells(col.Index)
                            Exit For
                        End If
                    Next
                End If
                e.Handled = True
                e.SuppressKeyPress = True
            End If
        End If
    End Sub

    Private Sub TextBox6_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox6.KeyDown
        If e.KeyCode = Keys.Up Or e.KeyCode = Keys.Down Then
            If CustomerPaymentsView.Rows.Count > 0 Then
                CustomerPaymentsView.Focus()
                e.Handled = True
                e.SuppressKeyPress = True
            End If
        ElseIf e.KeyCode = Keys.Enter Then
            If CustomerPaymentsView.Rows.Count > 0 Then
                Dim row As DataGridViewRow = CustomerPaymentsView.Rows(0)
                ' Similar logic to CustomerPaymentsView_CellClick - triggers UI update for the record
                ' We'll simulate a cell click or just call the selection logic
                If Not row.IsNewRow Then
                    ' This will populate the fields
                    isExistingPaymentSelected = True
                    isSettingNameProgrammatically = True
                    NameTextBox1.Text = If(row.Cells("Customer").Value?.ToString(), "")
                    isSettingNameProgrammatically = False
                    AmountTextBox1.Text = If(row.Cells("Amount").Value?.ToString(), "")
                    ' Select payment type
                    If CustomerPaymentsView.Columns.Contains("PaymentType") Then
                        Dim pType As String = If(row.Cells("PaymentType").Value?.ToString(), "").Trim()
                        If pType.Equals("Chaque", StringComparison.OrdinalIgnoreCase) Then pType = "Cheque"
                        ComboBox1.Text = pType
                    End If
                    AmountTextBox1.Select()
                End If
                e.Handled = True
                e.SuppressKeyPress = True
            End If
        End If
    End Sub

    Private Sub TextBox5_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox5.KeyDown
        If e.KeyCode = Keys.Up Or e.KeyCode = Keys.Down Then
            If CustomerPaymentsView.Rows.Count > 0 Then
                CustomerPaymentsView.Focus()
                e.Handled = True
                e.SuppressKeyPress = True
            End If
        ElseIf e.KeyCode = Keys.Enter Then
            ' Same logic as TextBox6
            TextBox6_KeyDown(sender, e)
        End If
    End Sub

    Private Sub TextBox4_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox4.KeyDown
        If e.KeyCode = Keys.Up Or e.KeyCode = Keys.Down Then
            If CustomerPaymentsView.Rows.Count > 0 Then
                CustomerPaymentsView.Focus()
                e.Handled = True
                e.SuppressKeyPress = True
            End If
        ElseIf e.KeyCode = Keys.Enter Then
            ' Same logic as TextBox6
            TextBox6_KeyDown(sender, e)
        End If
    End Sub

    Private Sub AmountTextBox1_TextChanged(sender As Object, e As EventArgs) Handles AmountTextBox1.TextChanged
        ' Removed annoying MessageBox - validation happens in pay_save_Click
        ' Just provide visual feedback if needed
        Dim payment As Double
        If Double.TryParse(AmountTextBox1.Text, payment) AndAlso CreditAmt > 0 Then
            If payment > CreditAmt Then
                AmountTextBox1.BackColor = Color.FromArgb(255, 200, 200) ' Light red
            Else
                AmountTextBox1.BackColor = Color.FromArgb(192, 255, 192) ' Light green
            End If
        End If
    End Sub
    Private Function IsSecureKeyValid() As Boolean
        If ComboBox3.SelectedIndex = -1 Then
            MessageBox.Show("Please select a user")
            ComboBox3.Focus()
            Return False
        End If


        Try
            If MySqlConn.State <> ConnectionState.Open Then MySqlConn.Open()

            Dim userId As Integer = Convert.ToInt32(ComboBox3.SelectedValue)
            ' Join with user_role to get the role name and check both secure key and role
            Dim query As String = "SELECT u.hiddenSecurekey, r.role_name " &
                                 "FROM user u " &
                                 "INNER JOIN user_role r ON u.role_id = r.id " &
                                 "WHERE u.id = @id"
            Dim cmd As New MySqlCommand(query, MySqlConn)
            cmd.Parameters.AddWithValue("@id", userId)

            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.Read() Then
                    Dim dbSecureKey As String = reader("hiddenSecurekey").ToString()
                    Dim roleName As String = reader("role_name").ToString().ToLower()

                    If dbSecureKey = secure_key.Text.Trim() AndAlso (roleName = "admin" OrElse roleName = "owner") Then
                        Return True
                    Else
                        MessageBox.Show("You Are Not Authorized To Do That  ")
                        secure_key.Clear()
                        secure_key.Focus()
                        Return False
                    End If
                Else
                    MessageBox.Show("User not found or role missing.")
                    Return False
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Security check failed: " & ex.Message)
            Return False
        Finally
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Function
    Private Function IsSecureKeyValid1() As Boolean
        If ComboBox2.SelectedIndex = -1 Then
            MessageBox.Show("Please select a user")
            ComboBox2.Focus()
            Return False
        End If


        Try
            If MySqlConn.State <> ConnectionState.Open Then MySqlConn.Open()

            Dim userId As Integer = Convert.ToInt32(ComboBox2.SelectedValue)
            ' Join with user_role to get the role name and check both secure key and role
            Dim query As String = "SELECT u.hiddenSecurekey, r.role_name " &
                                 "FROM user u " &
                                 "INNER JOIN user_role r ON u.role_id = r.id " &
                                 "WHERE u.id = @id"
            Dim cmd As New MySqlCommand(query, MySqlConn)
            cmd.Parameters.AddWithValue("@id", userId)

            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.Read() Then
                    Dim dbSecureKey As String = reader("hiddenSecurekey").ToString()
                    Dim roleName As String = reader("role_name").ToString().ToLower()

                    If dbSecureKey = secure_key1.Text.Trim() AndAlso (roleName = "admin" OrElse roleName = "owner") Then
                        Return True
                    Else
                        MessageBox.Show("You Are Not Authorized To Do That  ")
                        secure_key1.Clear()
                        secure_key1.Focus()
                        Return False
                    End If
                Else
                    MessageBox.Show("User not found or role missing.")
                    Return False
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Security check failed: " & ex.Message)
            Return False
        Finally
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Function

    Private Sub LoadUsers()
        Try
            If MySqlConn.State <> ConnectionState.Open Then MySqlConn.Open()

            Dim adapter As New MySqlDataAdapter("SELECT id, name FROM user", MySqlConn)
            Dim dt As New DataTable()
            adapter.Fill(dt)

            ' Populate ComboBox3 (Customer Details)
            ComboBox3.DataSource = dt
            ComboBox3.DisplayMember = "name"
            ComboBox3.ValueMember = "id"
            If Not String.IsNullOrEmpty(Module1.UserName) Then
                ComboBox3.Text = Module1.UserName
            Else
                ComboBox3.SelectedIndex = -1
            End If

            ' Create a copy of the DataTable for ComboBox2 (Customer Payments)
            ' ComboBoxes can't share the same CurrencyManager (DataSource) effectively without linking selections
            Dim dt2 As DataTable = dt.Copy()
            ComboBox2.DataSource = dt2
            ComboBox2.DisplayMember = "name"
            ComboBox2.ValueMember = "id"
            If Not String.IsNullOrEmpty(Module1.UserName) Then
                ComboBox2.Text = Module1.UserName
            Else
                ComboBox2.SelectedIndex = -1
            End If

            MySqlConn.Close()
        Catch ex As Exception
            MessageBox.Show("Error loading users: " & ex.Message)
        Finally
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Sub

    Private Sub credit_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.F2 Then
            If TabControl1.SelectedTab Is TabPage1 Then
                NameSeachTextBox.Focus()
            ElseIf TabControl1.SelectedTab Is TabPage2 Then
                DAddNewBtn.PerformClick()
            ElseIf TabControl1.SelectedTab Is TabPage3 Then
                pay_add.PerformClick()
            End If
            e.Handled = True
        ElseIf e.KeyCode = Keys.F3 Then
            If TabControl1.SelectedTab Is TabPage2 Then
                DUpdateBtn.PerformClick()
                e.Handled = True
            End If
        ElseIf e.KeyCode = Keys.Delete Then
            If TabControl1.SelectedTab Is TabPage2 Then
                ' Only trigger delete button if we are on the Customer Details tab
                DDeleteBtn.PerformClick()
                e.Handled = True
            ElseIf TabControl1.SelectedTab Is TabPage3 Then
                ' Trigger delete button if we are on the Customer Payments tab
                pay_del.PerformClick()
                e.Handled = True
            End If
        ElseIf e.KeyCode = Keys.Escape Then
            If TabControl1.SelectedTab Is TabPage2 Then
                DAddNewBtn.PerformClick()
                Panel2.Visible = False
            ElseIf TabControl1.SelectedTab Is TabPage3 Then
                pay_cancel.PerformClick()
            End If
            e.Handled = True
            e.SuppressKeyPress = True
        End If
    End Sub
    ' Continuation logic for Cheque Payment after user clicks OK on the panel
    Private Sub SaveChequePaymentFinal()
        If currentDistribution Is Nothing OrElse currentDistribution.Count = 0 Then
            MessageBox.Show("No payment distribution found. Please try again.")
            Return
        End If

        Dim inputAmount As Double = 0
        Double.TryParse(AmountTextBox1.Text, inputAmount)
        Dim paymentDate As String = DateTimePicker4.Value.ToString("yyyy-MM-dd HH:mm:ss")
        Dim chqNo As String = ChqNoTextBox.Text.Trim().Replace("'", "").Replace("""", "").Replace("\", "").Replace(";", "")

        ' Use global connection for the transaction
        Dim transaction As MySqlTransaction = Nothing
        Try
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
            MySqlConn.Open()
            transaction = MySqlConn.BeginTransaction()

            ' Apply each invoice payment from the stored distribution
            For Each entry In currentDistribution
                ApplyInvoicePayment(transaction, selectedCustomerId, NameTextBox1.Text, entry.Key, entry.Value, paymentDate, chqNo, selectedBankId)
            Next

            ' --- AUTO SAVE CHEQUE TO CUSTOMER CHEQUES TABLE ---
            If inputAmount > 0 AndAlso Not String.IsNullOrWhiteSpace(chqNo) AndAlso selectedBankId > 0 Then
                Dim invoiceList As New List(Of String)
                For Each entry In currentDistribution
                    invoiceList.Add(If(entry.Key = "CREDIT_ONLY", "CREDIT", entry.Key))
                Next
                Dim invNoStr As String = String.Join(", ", invoiceList)
 
                Dim checkChqExistSql As String = "SELECT status FROM check_received WHERE check_number = @chq AND bank_id = @bank AND inv_no = @inv LIMIT 1"
                Dim existsCount As Integer = 0
                Dim existingStatus As String = ""
                Using cmdCheckChq As New MySqlCommand(checkChqExistSql, MySqlConn, transaction)
                    cmdCheckChq.Parameters.AddWithValue("@chq", chqNo)
                    cmdCheckChq.Parameters.AddWithValue("@bank", selectedBankId)
                    cmdCheckChq.Parameters.AddWithValue("@inv", invNoStr)
                    Using drChq As MySqlDataReader = cmdCheckChq.ExecuteReader()
                        If drChq.Read() Then
                            existsCount = 1
                            existingStatus = If(drChq("status") Is DBNull.Value, "", drChq("status").ToString())
                        End If
                    End Using
                End Using
 
                If existsCount > 0 Then
                    ' If it exists and status is Pending, update it
                    If String.Equals(existingStatus, "Pending", StringComparison.OrdinalIgnoreCase) Then
                        Dim updateChqSql As String = "UPDATE check_received SET check_name = @name, amount = @amt, check_release_date = @release, issue_date = @issue WHERE check_number = @chq AND bank_id = @bank AND inv_no = @inv"
                        Using cmdUpdateChq As New MySqlCommand(updateChqSql, MySqlConn, transaction)
                            cmdUpdateChq.Parameters.AddWithValue("@chq", chqNo)
                            cmdUpdateChq.Parameters.AddWithValue("@bank", selectedBankId)
                            cmdUpdateChq.Parameters.AddWithValue("@inv", invNoStr)
                            cmdUpdateChq.Parameters.AddWithValue("@name", NameTextBox1.Text.Trim())
                            cmdUpdateChq.Parameters.AddWithValue("@amt", inputAmount)
                            cmdUpdateChq.Parameters.AddWithValue("@release", DateTimePicker4.Value.ToString("yyyy-MM-dd"))
                            cmdUpdateChq.Parameters.AddWithValue("@issue", DateTimePicker4.Value.ToString("yyyy-MM-dd"))
                            cmdUpdateChq.ExecuteNonQuery()
                        End Using
                    End If
                Else
                    ' Insert new cheque
                    Dim insertChqSql As String = "INSERT INTO check_received (check_number, check_name, bank_id, amount, status, issue_date, check_release_date, inv_no) " &
                                                 "VALUES (@chq, @name, @bank, @amt, 'Pending', @issue, @release, @inv)"
                    Using cmdInsertChq As New MySqlCommand(insertChqSql, MySqlConn, transaction)
                        cmdInsertChq.Parameters.AddWithValue("@chq", chqNo)
                        cmdInsertChq.Parameters.AddWithValue("@name", NameTextBox1.Text.Trim())
                        cmdInsertChq.Parameters.AddWithValue("@bank", selectedBankId)
                        cmdInsertChq.Parameters.AddWithValue("@amt", inputAmount)
                        cmdInsertChq.Parameters.AddWithValue("@issue", DateTimePicker4.Value.ToString("yyyy-MM-dd"))
                        cmdInsertChq.Parameters.AddWithValue("@release", DateTimePicker4.Value.ToString("yyyy-MM-dd"))
                        cmdInsertChq.Parameters.AddWithValue("@inv", invNoStr)
                        cmdInsertChq.ExecuteNonQuery()
                    End Using
                End If
            End If
 
            transaction.Commit()
            MySqlConn.Close()
            MessageBox.Show("Cheque payment recorded successfully. Invoice-wise allocation completed.")

            ' Cleanup UI
            AmountTextBox1.Clear()
            NameTextBox1.Clear()
            selectedCustomerId = 0
            creid = 0
            TelNoTextBox.Clear()
            currentDistribution.Clear()
            ChequeEntryPanel.Visible = False
            RefreshAllGrids()

        Catch ex As Exception
            If transaction IsNot Nothing Then transaction.Rollback()
            MessageBox.Show("Error saving cheque distribution: " & ex.Message)
        Finally
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Sub

    Private Sub DUpdateBtn_Click(sender As Object, e As EventArgs) Handles DUpdateBtn.Click
        If creid = 0 Then
            MsgBox("Please select an existing record from the grid to update.")
            Return
        End If

        If DCustomerNameTxt.Text = "" Or DAmountTxt.Text = "" Or DCustomerInvTxt.Text = "" Then
            MsgBox("Please Enter Customer Name, Amount, and Invoice Number")
            Return
        End If

        If Not IsSecureKeyValid() Then Exit Sub

        Dim creditDate As String = Format(Me.DateTimePicker3.Value, "yyyy-MM-dd HH:mm:ss")
        
        Try
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
            MySqlConn.Open()

            Dim sql As String = "UPDATE customer_credit SET amount = @amount, timestamps = @date, inv_no = @inv WHERE id = @creid"
            Using cmd As New MySqlCommand(sql, MySqlConn)
                cmd.Parameters.AddWithValue("@creid", creid)
                cmd.Parameters.AddWithValue("@amount", DAmountTxt.Text)
                cmd.Parameters.AddWithValue("@date", creditDate)
                cmd.Parameters.AddWithValue("@inv", DCustomerInvTxt.Text.Trim())
                cmd.ExecuteNonQuery()
            End Using

            ' Synchronize update to billing table if it is a dummy record
            Dim billingUpdateSql As String = "UPDATE billing SET subtotal = @amount, grand_total = @amount, credit_balance_due = @amount, balance_due = @amount WHERE inv_no = @inv AND inv_type = 'Manual Credit'"
            Using cmdBilling As New MySqlCommand(billingUpdateSql, MySqlConn)
                cmdBilling.Parameters.AddWithValue("@amount", DAmountTxt.Text)
                cmdBilling.Parameters.AddWithValue("@inv", DCustomerInvTxt.Text.Trim())
                cmdBilling.ExecuteNonQuery()
            End Using
            
            MySqlConn.Close()
            MsgBox("Updated Successfully")
            RefreshAllGrids()
            
            ' Clear fields after update
            DAddNewBtn.PerformClick()
            Panel2.Visible = False

        Catch ex As Exception
            MsgBox("Error updating record: " & ex.Message)
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Sub

    Private Sub dtpDetailsStart_ValueChanged(sender As Object, e As EventArgs) Handles dtpDetailsStart.ValueChanged
        ApplyDebitFilters()
    End Sub

    Private Sub dtpDetailsEnd_ValueChanged(sender As Object, e As EventArgs) Handles dtpDetailsEnd.ValueChanged
        ApplyDebitFilters()
    End Sub


End Class
