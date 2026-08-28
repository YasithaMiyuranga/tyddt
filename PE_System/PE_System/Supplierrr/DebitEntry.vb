Imports MySql.Data.MySqlClient
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared
Public Class DebitEntry
    Dim COMMAND As MySqlCommand
    Dim READER As MySqlDataReader
    Dim CreditAmt As Double
    Dim creid As Integer
    Private isSettingNameProgrammatically As Boolean = False
    Dim uid As Integer
    Dim CreIds As Integer
    Dim selectedInvNo As String = ""
    Private isProcessingSave As Boolean = False
    Private selectedPaymentId As Integer = 0
    Private currentDistribution As New Dictionary(Of String, Double) ' Tracks manual allocation


    ' Enhanced Cheque Entry UI (Popup Panel)

    Private ChequeEntryPanel As Panel
    Private ChqNoTextBox As TextBox
    Private BankSearchTextBox As TextBox
    Private BankDataGridView As DataGridView
    Private ChqOkBtn As Button
    Private ChqCancelBtn As Button
    Private selectedBankId As Integer = 0
    Private selectedBankName As String = ""

    ' Pay Type Filter UI
    Private LabelPayTypeFilter As Label
    Private btnPayTypeAll As Button
    Private btnPayTypeCash As Button
    Private btnPayTypeCheque As Button
    Private btnPayTypeOnline As Button
    Private btnPayTypeReturn As Button
    Private selectedPayTypeFilter As String = "All"

    ' Payment Date Range Filter (Tab 3)
    Private PayStartDate As DateTimePicker
    Private PayEndDate As DateTimePicker
    Private btnApplyPayDateFilter As Button

    Private Sub ConfigurePaymentGrid()
        ' Extreme Safety: Disable auto-generation before anything else
        CustomerPaymentsView.AutoGenerateColumns = False
        CustomerPaymentsView.Columns.Clear()

        AddGridColumn("name", "name", "Name", 350)
        AddGridColumn("type", "type", "Type", 100)

        Dim colAmt = AddGridColumn("amount", "amount", "Amount", 150)
        colAmt.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

        AddGridColumn("pdate", "pdate", "Date", 150, "yyyy-MM-dd")
        AddGridColumn("inv_no", "inv_no", "Inv No", 150)
        AddGridColumn("chq_no", "chq_no", "Chq No", 120)
        AddGridColumn("bank_name", "bank_name", "Bank", 150)

        ' Hidden Columns
        AddGridHiddenColumn("id", "id")
        AddGridHiddenColumn("supplier_id", "supplier_id")
        AddGridHiddenColumn("creid", "creid")
        ' Visible Status Column
        AddGridColumn("status", "status", "Status", 100)

        CustomerPaymentsView.AllowUserToAddRows = False
        CustomerPaymentsView.MultiSelect = False
        CustomerPaymentsView.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        CustomerPaymentsView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
    End Sub

    Private Function AddGridColumn(dataName As String, colName As String, header As String, width As Integer, Optional format As String = "") As DataGridViewTextBoxColumn
        Dim col As New DataGridViewTextBoxColumn()
        col.DataPropertyName = dataName
        col.Name = colName
        col.HeaderText = header
        col.Width = width
        If Not String.IsNullOrEmpty(format) Then col.DefaultCellStyle.Format = format
        CustomerPaymentsView.Columns.Add(col)
        Return col
    End Function

    Private Sub ClearDebitFields()
        DCustomerNameTxt.Clear()
        DCustomerTelNoTxt.Clear()
        DAmountTxt.Clear()
        DCustomerInvNoTxt.Clear()
        DateTimePicker1.Value = Date.Now
        uid = 0
        creid = 0
        Panel8.Visible = False
    End Sub

    Private Sub DataGridView1_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellDoubleClick
        If e.RowIndex >= 0 Then
            Dim row As DataGridViewRow = DataGridView1.Rows(e.RowIndex)
            Dim supplierName As String = row.Cells("sname").Value.ToString()

            ' Set the filter text which triggers loaddebit_filtered
            TextBox6.Text = supplierName

            ' Explicitly hide the suggestion panels that may have been opened by the single-click event
            Panel8.Visible = False
            Panel7.Visible = False
        End If
    End Sub

    Private Sub ClearPaymentFields()
        NameTextBox1.Clear()
        TelNoTextBox1.Clear()
        AmountTextBox1.Clear()
        DateTimePicker4.Value = Date.Now
        uid = 0
        creid = 0
        CreditAmt = 0
        selectedInvNo = ""
        Panel7.Visible = False
        selectedPaymentId = 0
        ComboBox1.SelectedIndex = -1 ' Clear selection to force user selection
    End Sub

    Private Sub ResetAllFilters()
        ' Tab 1 (Supplier Debit / Credit Search)
        NameSeachTextBox.Clear()
        Amountbox.Clear()
        If cmbCreditFilter2.Items.Count > 0 Then cmbCreditFilter2.SelectedIndex = 0

        ' Tab 2 (Debit Details / Search)
        TextBox6.Clear()
        If cmbCreditFilter1.Items.Count > 0 Then cmbCreditFilter1.SelectedIndex = 0

        ' Tab 3 (Supplier Payments Search)
        TextBox9.Clear()

        ' Invoice Status Filter (Global/Tab2)
        If DInvoiceStatusCombo.Items.Count > 0 Then DInvoiceStatusCombo.SelectedIndex = 0
    End Sub

    Private Sub AddGridHiddenColumn(dataName As String, colName As String)
        Dim col As New DataGridViewTextBoxColumn()
        col.DataPropertyName = dataName
        col.Name = colName
        col.Visible = False
        CustomerPaymentsView.Columns.Add(col)
    End Sub

    Private Sub LoadPayments()
        ' Comprehensive Self-healing Migration for supplier_payments
        Try
            If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()

            Dim schemaTable As DataTable = MySqlConn.GetSchema("Columns", New String() {Nothing, Nothing, "supplier_payments", Nothing})
            Dim existingCols As New List(Of String)
            For Each row As DataRow In schemaTable.Rows
                existingCols.Add(row("COLUMN_NAME").ToString().ToLower())
            Next

            ' Add missing columns one by one
            If Not existingCols.Contains("supplier_id") Then
                Using cmd As New MySqlCommand("ALTER TABLE supplier_payments ADD COLUMN supplier_id INT", MySqlConn)
                    cmd.ExecuteNonQuery()
                End Using
            End If
            If Not existingCols.Contains("creid") Then
                Using cmd As New MySqlCommand("ALTER TABLE supplier_payments ADD COLUMN creid INT", MySqlConn)
                    cmd.ExecuteNonQuery()
                End Using
            End If
            If Not existingCols.Contains("inv_no") Then
                Using cmd As New MySqlCommand("ALTER TABLE supplier_payments ADD COLUMN inv_no VARCHAR(50)", MySqlConn)
                    cmd.ExecuteNonQuery()
                End Using
            End If
            If Not existingCols.Contains("chq_no") Then
                Using cmd As New MySqlCommand("ALTER TABLE supplier_payments ADD COLUMN chq_no VARCHAR(100)", MySqlConn)
                    cmd.ExecuteNonQuery()
                End Using
            End If
            If Not existingCols.Contains("bank_id") Then
                Using cmd As New MySqlCommand("ALTER TABLE supplier_payments ADD COLUMN bank_id INT", MySqlConn)
                    cmd.ExecuteNonQuery()
                End Using
            End If

            ' [NEW] Migration: Upgrade numeric column precision for large amounts (up to 1,000 million)
            Dim migrateSqls As String() = {
                "ALTER TABLE supplier_payments MODIFY COLUMN amount DECIMAL(18,2)",
                "ALTER TABLE supplicer_credit MODIFY COLUMN amount DECIMAL(18,2)",
                "ALTER TABLE purchasing MODIFY COLUMN balance_due DECIMAL(18,2)",
                "ALTER TABLE purchasing MODIFY COLUMN credit_balance_due DECIMAL(18,2)",
                "ALTER TABLE purchasing MODIFY COLUMN paid_amount DECIMAL(18,2)",
                "ALTER TABLE purchasing MODIFY COLUMN total_amount DECIMAL(18,2)"
            }

            For Each alterSql In migrateSqls
                Try
                    Using alterCmd As New MySqlCommand(alterSql, MySqlConn)
                        alterCmd.ExecuteNonQuery()
                    End Using
                Catch ex As Exception
                    ' Ignore if specific upgrade fails
                End Try
            Next

            MySqlConn.Close()
        Catch ex As Exception
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try

        If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()
        Dim table As New DataTable

        ' [MODIFIED] Join with supplier to allow phone filtering
        Dim sql As String = "SELECT p.name, p.type, p.amount, p.pdate, p.inv_no, p.id, p.supplier_id, p.creid, p.chq_no, b.bank_name, s.tel_no, " &
                           "MAX(CASE WHEN ci.status IN ('REALISED', 'REALIZED', 'RELEASED') THEN 'RELEASED' ELSE ci.status END) as status " &
                           "FROM supplier_payments p " &
                           "LEFT JOIN bank b ON p.bank_id = b.id " &
                           "LEFT JOIN supplier s ON p.supplier_id = s.id " &
                           "LEFT JOIN chaque_issue ci ON (TRIM(p.chq_no) = TRIM(ci.chq_no) AND p.bank_id = ci.bank_id AND FIND_IN_SET(TRIM(p.inv_no), REPLACE(ci.inv_no, ', ', ',')) AND TRIM(p.chq_no) <> '') " &
                           "WHERE 1=1 "

        ' 0. Date Range Filter (pdate)
        If PayStartDate IsNot Nothing AndAlso PayEndDate IsNot Nothing Then
            sql &= " AND p.pdate BETWEEN @payStart AND @payEnd "
        End If

        ' 1. Name Filter
        If TextBox9.Text.Trim() <> "" Then
            sql &= " AND p.name LIKE @name "
        End If

        ' 2. Invoice Filter
        If filter_invoice.Text.Trim() <> "" Then
            sql &= " AND p.inv_no LIKE @inv "
        End If

        ' 3. Phone Filter
        If filter_phone.Text.Trim() <> "" Then
            sql &= " AND s.tel_no LIKE @phone "
        End If

        ' 4. Pay Type Filter
        If Not String.IsNullOrEmpty(selectedPayTypeFilter) AndAlso selectedPayTypeFilter <> "All" Then
            If selectedPayTypeFilter = "Online Payment" Then
                sql &= " AND (p.type = 'Online Payment' OR p.type = 'Online Transfer') "
            Else
                sql &= " AND p.type = @payType "
            End If
        End If

        sql &= "GROUP BY p.id, p.name, p.type, p.amount, p.pdate, p.inv_no, p.supplier_id, p.creid, p.chq_no, b.bank_name, s.tel_no " &
               "ORDER BY p.name ASC, p.pdate DESC"

        Dim adapter As New MySqlDataAdapter(sql, MySqlConn)
        If PayStartDate IsNot Nothing AndAlso PayEndDate IsNot Nothing Then
            adapter.SelectCommand.Parameters.AddWithValue("@payStart", Format(PayStartDate.Value, "yyyy-MM-dd 00:00:00"))
            adapter.SelectCommand.Parameters.AddWithValue("@payEnd", Format(PayEndDate.Value, "yyyy-MM-dd 23:59:59"))
        End If
        If TextBox9.Text.Trim() <> "" Then adapter.SelectCommand.Parameters.AddWithValue("@name", TextBox9.Text.Trim() & "%")
        If filter_invoice.Text.Trim() <> "" Then adapter.SelectCommand.Parameters.AddWithValue("@inv", filter_invoice.Text.Trim() & "%")
        If filter_phone.Text.Trim() <> "" Then adapter.SelectCommand.Parameters.AddWithValue("@phone", filter_phone.Text.Trim() & "%")
        If Not String.IsNullOrEmpty(selectedPayTypeFilter) AndAlso selectedPayTypeFilter <> "All" AndAlso selectedPayTypeFilter <> "Online Payment" Then
            adapter.SelectCommand.Parameters.AddWithValue("@payType", selectedPayTypeFilter)
        End If

        adapter.Fill(table)

        CustomerPaymentsView.DataSource = Nothing
        ConfigurePaymentGrid()

        Dim bsource As New BindingSource
        bsource.DataSource = table
        CustomerPaymentsView.DataSource = bsource
        MySqlConn.Close()

        CalculatePaymentTotal()
    End Sub

    Private Sub CustomerPaymentsView_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles CustomerPaymentsView.CellFormatting
        Try
            If e.RowIndex >= 0 Then
                Dim row As DataGridViewRow = CustomerPaymentsView.Rows(e.RowIndex)
                If row.Cells("status").Value IsNot Nothing AndAlso row.Cells("type").Value IsNot Nothing Then
                    Dim status As String = row.Cells("status").Value.ToString().ToUpper()
                    Dim payType As String = row.Cells("type").Value.ToString().ToLower()

                    If payType.Contains("online") Then
                        row.DefaultCellStyle.BackColor = Color.FromArgb(195, 230, 255) ' Soft Light Blue
                    ElseIf payType.Contains("return") Then
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 218, 218) ' Soft Pink
                    ElseIf payType.Contains("cheque") OrElse payType.Contains("chaque") Then
                        Select Case status
                            Case "RELEASED", "REALIZED", "REALISED", "CLEARED", "SUCCESS", "PAID"
                                row.DefaultCellStyle.BackColor = Color.YellowGreen
                            Case "RETURNED", "RETURN"
                                row.DefaultCellStyle.BackColor = Color.Red
                            Case "PENDING"
                                row.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 128)
                            Case Else
                                row.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 128) ' Default to Yellow for unlinked cheques
                        End Select
                    Else
                        ' Non-cheque, non-online payments (e.g. Cash) are always white
                        row.DefaultCellStyle.BackColor = Color.White
                    End If
                Else
                    ' [FIX] Handling case where status column itself is NULL/Missing
                    Dim payTypeVal = If(row.Cells("type").Value IsNot Nothing, row.Cells("type").Value.ToString().ToLower(), "")
                    If payTypeVal.Contains("online") Then
                        row.DefaultCellStyle.BackColor = Color.FromArgb(195, 230, 255) ' Soft Light Blue
                    ElseIf payTypeVal.Contains("return") Then
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 218, 218) ' Soft Pink
                    ElseIf payTypeVal.Contains("cheque") OrElse payTypeVal.Contains("chaque") Then
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 128) ' Default Pending Color
                    Else
                        row.DefaultCellStyle.BackColor = Color.White
                    End If
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub CreditDataGridView_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles CreditDataGridView.CellFormatting, DataGridView1.CellFormatting
        If e.RowIndex >= 0 Then
            Dim dgv = DirectCast(sender, DataGridView)
            
            If dgv.Columns.Contains("amount") Then
                Dim val = dgv.Rows(e.RowIndex).Cells("amount").Value
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

    Private Sub CalculatePaymentTotal()
        Dim tot As Double = 0
        For Each row As DataGridViewRow In CustomerPaymentsView.Rows
            If Not row.IsNewRow AndAlso row.Cells("amount").Value IsNot Nothing Then
                Dim val As Double = 0
                Double.TryParse(row.Cells("amount").Value.ToString(), val)
                tot += val
            End If
        Next
        Label16.Text = tot.ToString
    End Sub

    Private Sub load_for_date()
        ' Redirect to centralized filtered load
        Load_credit_filtered()
    End Sub


    Private Sub DebitEntry_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Populate Status Filters
        cmbCreditFilter1.Items.Clear()
        cmbCreditFilter1.Items.AddRange(New Object() {"All", "Pending", "Paid"})
        cmbCreditFilter1.SelectedIndex = 1

        cmbCreditFilter2.Items.Clear()
        cmbCreditFilter2.Items.AddRange(New Object() {"All", "Pending", "Paid"})
        cmbCreditFilter2.SelectedIndex = 1

        ' Initialize Invoice Status Filter
        DInvoiceStatusCombo.Items.Clear()
        DInvoiceStatusCombo.Items.Add("All")
        DInvoiceStatusCombo.Items.Add("Credit")
        DInvoiceStatusCombo.Items.Add("cash_Credit")
        DInvoiceStatusCombo.Items.Add("Mixed_Payment")
        DInvoiceStatusCombo.Items.Add("Credit_Cheque")
        DInvoiceStatusCombo.SelectedIndex = 0 ' Default to All

        ' Self-healing Migration: Ensure inv_no column exists in supplicer_credit
        Try
            If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()
            Dim checkInvColCmd As New MySqlCommand("SHOW COLUMNS FROM supplicer_credit LIKE 'inv_no'", MySqlConn)
            Dim invExists = checkInvColCmd.ExecuteScalar()
            If invExists Is Nothing Then
                Dim addInvColCmd As New MySqlCommand("ALTER TABLE supplicer_credit ADD COLUMN inv_no VARCHAR(50) AFTER sname", MySqlConn)
                addInvColCmd.ExecuteNonQuery()
            End If
            MySqlConn.Close()
        Catch ex As Exception
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try

        ' Initialize Pay Type Filters
        InitPayTypeFilters()
        UpdatePayTypeButtonStyles()

        ' Ensure Good Return is present in the Pay Type dropdown list
        If Not ComboBox1.Items.Contains("Good Return") Then
            ComboBox1.Items.Add("Good Return")
        End If

        Load_credit_filtered() ' Use filtered load instead of Load_credit
        LoadPayments()
        loaddebit_filtered() ' Use filtered load instead of loaddebit
        LoadUserList()
        InitializeChequePanel()

        ' Hide Update/Delete buttons for non-owner roles (Admin and Cashier)
        Dim role As String = If(Module1.UserRole IsNot Nothing, Module1.UserRole.ToLower(), "")
        If role = "admin" OrElse role = "cashier" Then
            DDeleteBtn.Visible = False
            DUpdateBtn.Visible = False
            Button12.Visible = False
        End If
    End Sub

    Private Sub DebitEntry_Activated(sender As Object, e As EventArgs) Handles MyBase.Activated
        Load_credit_filtered()
        LoadPayments()
        loaddebit_filtered()
    End Sub

    Private Sub InitializeChequePanel()
        ' Create Panel
        ChequeEntryPanel = New Panel()
        ChequeEntryPanel.Size = New Size(450, 500)
        ChequeEntryPanel.BackColor = Color.FromArgb(44, 62, 80) ' Consistent with TabPage background
        ChequeEntryPanel.BorderStyle = BorderStyle.FixedSingle
        ChequeEntryPanel.Visible = False
        Me.Controls.Add(ChequeEntryPanel)
        ChequeEntryPanel.BringToFront()

        ' Position the panel in the center of the Form
        AddHandler Me.Resize, Sub()
                                  ChequeEntryPanel.Location = New Point((Me.ClientSize.Width - ChequeEntryPanel.Width) / 2, (Me.ClientSize.Height - ChequeEntryPanel.Height) / 2)
                              End Sub
        ' Immediate positioning call
        ChequeEntryPanel.Location = New Point((Me.ClientSize.Width - ChequeEntryPanel.Width) / 2, (Me.ClientSize.Height - ChequeEntryPanel.Height) / 2)

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

    Private Sub InitPayTypeFilters()
        ' Resize GroupBox3 to fit one extra row of pay type buttons below Name/Phone/Inv filters
        GroupBox3.Height = 120
        CustomerPaymentsView.Location = New Point(1, GroupBox3.Bottom + 5)
        CustomerPaymentsView.Height = Me.ClientSize.Height - CustomerPaymentsView.Top - 10

        ' No date range row needed (cleaner layout matching screenshot)

        ' ── Pay Type Row ───────────────────────────────────────────────
        ' Create Label
        LabelPayTypeFilter = New Label()
        LabelPayTypeFilter.AutoSize = True
        LabelPayTypeFilter.Font = New Font("Microsoft Sans Serif", 12.75!, FontStyle.Bold, GraphicsUnit.Point, CType(0, Byte))
        LabelPayTypeFilter.ForeColor = Color.White
        LabelPayTypeFilter.Location = New Point(52, 72)
        LabelPayTypeFilter.Name = "LabelPayTypeFilter"
        LabelPayTypeFilter.Size = New Size(170, 26)
        LabelPayTypeFilter.Text = "Filter Pay Type:"
        GroupBox3.Controls.Add(LabelPayTypeFilter)

        ' Instantiate buttons - sized and spaced to match screenshot
        btnPayTypeAll = New Button()
        ConfigureFilterButton(btnPayTypeAll, "All", "All", 240, 110)

        btnPayTypeCash = New Button()
        ConfigureFilterButton(btnPayTypeCash, "Cash", "Cash", 358, 110)

        btnPayTypeCheque = New Button()
        ConfigureFilterButton(btnPayTypeCheque, "Cheque", "Cheque", 476, 120)

        btnPayTypeOnline = New Button()
        ConfigureFilterButton(btnPayTypeOnline, "Online Transfer", "Online Payment", 604, 170)

        btnPayTypeReturn = New Button()
        ConfigureFilterButton(btnPayTypeReturn, "Good Return", "Good Return", 782, 155)
    End Sub

    Private Sub ConfigureFilterButton(btn As Button, text As String, tagVal As String, posX As Integer, widthVal As Integer)
        btn.FlatStyle = FlatStyle.Flat
        btn.Text = text
        btn.Tag = tagVal
        btn.Location = New Point(posX, 66)
        btn.Size = New Size(widthVal, 38)
        btn.Font = New Font("Microsoft Sans Serif", 11.0!, FontStyle.Bold, GraphicsUnit.Point, CType(0, Byte))
        btn.FlatAppearance.BorderSize = 1
        btn.Cursor = Cursors.Hand
        AddHandler btn.Click, AddressOf PayTypeFilterBtn_Click
        GroupBox3.Controls.Add(btn)
    End Sub

    Private Sub PayTypeFilterBtn_Click(sender As Object, e As EventArgs)
        Dim btn = DirectCast(sender, Button)
        selectedPayTypeFilter = btn.Tag.ToString()
        UpdatePayTypeButtonStyles()
        LoadPayments()
    End Sub

    Private Sub UpdatePayTypeButtonStyles()
        Dim buttons = {btnPayTypeAll, btnPayTypeCash, btnPayTypeCheque, btnPayTypeOnline, btnPayTypeReturn}
        For Each btn In buttons
            If btn IsNot Nothing Then
                Dim btnColor As Color = Color.LightGray
                Select Case btn.Tag.ToString()
                    Case "All"
                        btnColor = Color.LightGray
                    Case "Cash"
                        btnColor = Color.White
                    Case "Cheque"
                        btnColor = Color.FromArgb(255, 255, 128) ' Yellow
                    Case "Online Payment"
                        btnColor = Color.FromArgb(195, 230, 255) ' Soft Blue
                    Case "Good Return"
                        btnColor = Color.FromArgb(255, 218, 218) ' Soft Pink
                End Select

                btn.BackColor = btnColor
                btn.ForeColor = Color.Black

                If btn.Tag.ToString() = selectedPayTypeFilter Then
                    btn.FlatAppearance.BorderSize = 3
                    btn.FlatAppearance.BorderColor = Color.Black
                Else
                    btn.FlatAppearance.BorderSize = 1
                    btn.FlatAppearance.BorderColor = Color.DarkSlateGray
                End If
            End If
        Next
    End Sub

    Private Sub LoadBanks(Optional filter As String = "")
        ' Use local connection to prevent "Connection already open" conflict during real-time search
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

            ' Hide the ID column
            If BankDataGridView.Columns.Contains("id") Then
                BankDataGridView.Columns("id").Visible = False
            End If
        Catch ex As Exception
            ' Log or show error silently as this is called on TextChanged
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

        If currentDistribution IsNot Nothing AndAlso currentDistribution.Count > 0 Then
            SaveManualDistributionFinal()
        Else
            SaveChequePaymentFinal()
        End If
    End Sub

    Private Sub LoadUserList()
        Try
            If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()
            Dim cmd As New MySqlCommand("SELECT name FROM user WHERE (status IS NULL OR status = 'active')", MySqlConn)
            Dim reader As MySqlDataReader = cmd.ExecuteReader()

            ComboBox2.Items.Clear()
            ComboBox3.Items.Clear()

            While reader.Read()
                Dim userName As String = reader.GetString(0)
                ComboBox2.Items.Add(userName)
                ComboBox3.Items.Add(userName)
            End While
            reader.Close()
            If Not String.IsNullOrEmpty(Module1.UserName) Then
                ComboBox2.Text = Module1.UserName
                ComboBox3.Text = Module1.UserName
            End If
            MySqlConn.Close()
        Catch ex As Exception
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Sub

    ' CENTRALIZED FILTERED LOAD FOR TABPAGE1
    Private Sub Load_credit()
        Load_credit_filtered()
    End Sub

    Private Sub Load_credit_filtered()
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()

            Dim query As String = "SELECT sname, inv_no, amount, getdate, creid, supplier_id from supplicer_credit WHERE 1=1"

            ' 1. Name Filter
            If NameSeachTextBox.Text.Trim() <> "" Then
                query &= " AND sname LIKE @name"
            End If

            ' 2. Amount Filter (if specified)
            If Amountbox.Text.Trim() <> "" Then
                query &= " AND CAST(amount AS CHAR) LIKE @amt"
            End If

            ' 3. Status Filter
            If cmbCreditFilter2.Text = "Pending" Then
                query &= " AND amount > 0"
            ElseIf cmbCreditFilter2.Text = "Paid" Then
                query &= " AND amount = 0"
            End If

            ' 4. Date Filter (Always respect the date range)
            Dim Stadate As String = Format(Me.StartDate.Value, "yyyy-MM-dd 00:00:00")
            Dim enddat As String = Format(Me.EndDate.Value, "yyyy-MM-dd 23:59:59")
            query &= " AND getdate BETWEEN @start AND @end"

            Dim adapter As New MySqlDataAdapter(query & " ORDER BY sname ASC, getdate DESC", conn)
            adapter.SelectCommand.Parameters.AddWithValue("@start", Stadate)
            adapter.SelectCommand.Parameters.AddWithValue("@end", enddat)

            If NameSeachTextBox.Text.Trim() <> "" Then
                adapter.SelectCommand.Parameters.AddWithValue("@name", NameSeachTextBox.Text.Trim() & "%")
            End If
            If Amountbox.Text.Trim() <> "" Then
                adapter.SelectCommand.Parameters.AddWithValue("@amt", Amountbox.Text.Trim() & "%")
            End If

            Dim table As New DataTable()
            adapter.Fill(table)

            CreditDataGridView.DataSource = table
            CreditDataGridView.Columns(0).HeaderText = "Supplier Name"
            CreditDataGridView.Columns(0).Width = 300
            CreditDataGridView.Columns(1).HeaderText = "Inv No"
            CreditDataGridView.Columns(1).Width = 150
            CreditDataGridView.Columns(2).HeaderText = "Amount"
            CreditDataGridView.Columns(2).Width = 120
            CreditDataGridView.Columns(3).HeaderText = "Date"
            CreditDataGridView.Columns(3).Width = 150
            CreditDataGridView.Columns(3).DefaultCellStyle.Format = "yyyy-MM-dd"

            ' Hide technical columns properly
            If CreditDataGridView.Columns.Count > 4 Then CreditDataGridView.Columns(4).Visible = False
            If CreditDataGridView.Columns.Count > 5 Then CreditDataGridView.Columns(5).Visible = False

            CreditDataGridView.AllowUserToAddRows = False
            CreditDataGridView.MultiSelect = False
            CreditDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            CreditDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

            ' Update Total
            Dim tot As Double = 0
            For d As Integer = 0 To table.Rows.Count - 1
                tot += Convert.ToDouble(table.Rows(d)("amount"))
            Next
            TotalCriditLbl.Text = tot.ToString("N2")

            conn.Close()
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    ' CENTRALIZED FILTERED LOAD FOR TABPAGE2
    Private Sub loaddebit()
        loaddebit_filtered()
    End Sub

    Private Sub loaddebit_filtered()
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()

            ' [MODIFIED] Join with supplier for phone filtering
            Dim query As String = "SELECT c.getdate, c.sname, c.inv_no, c.amount, c.creid, c.supplier_id, s.tel_no " &
                                 "from supplicer_credit c " &
                                 "LEFT JOIN supplier s ON c.supplier_id = s.id " &
                                 "WHERE 1=1"

            ' 1. Name Filter
            If TextBox6.Text.Trim() <> "" Then
                query &= " AND c.sname LIKE @name"
            End If

            ' 2. Invoice Filter
            If TextBox4.Text.Trim() <> "" Then
                query &= " AND c.inv_no LIKE @inv"
            End If

            ' 3. Phone Filter
            If TextBox5.Text.Trim() <> "" Then
                query &= " AND s.tel_no LIKE @phone"
            End If

            ' 4. Status Filter
            If cmbCreditFilter1.Text = "Pending" Then
                query &= " AND c.amount > 0"
            ElseIf cmbCreditFilter1.Text = "Paid" Then
                query &= " AND c.amount = 0"
            End If

            Dim adapter As New MySqlDataAdapter(query & " ORDER BY c.sname ASC, c.getdate DESC", conn)

            If TextBox6.Text.Trim() <> "" Then adapter.SelectCommand.Parameters.AddWithValue("@name", TextBox6.Text.Trim() & "%")
            If TextBox4.Text.Trim() <> "" Then adapter.SelectCommand.Parameters.AddWithValue("@inv", TextBox4.Text.Trim() & "%")
            If TextBox5.Text.Trim() <> "" Then adapter.SelectCommand.Parameters.AddWithValue("@phone", TextBox5.Text.Trim() & "%")

            Dim table As New DataTable()
            adapter.Fill(table)

            DataGridView1.DataSource = table
            DataGridView1.Columns(0).HeaderText = "Date"
            DataGridView1.Columns(0).DefaultCellStyle.Format = "yyyy-MM-dd"
            DataGridView1.Columns(0).Width = 150
            DataGridView1.Columns(1).HeaderText = "Supplier Name"
            DataGridView1.Columns(1).Width = 300
            DataGridView1.Columns(2).HeaderText = "Inv No"
            DataGridView1.Columns(2).Width = 150
            DataGridView1.Columns(3).HeaderText = "Amount"
            DataGridView1.Columns(3).Width = 120
            DataGridView1.Columns(4).Visible = False ' creid
            DataGridView1.Columns(5).Visible = False ' supplier_id
            If DataGridView1.Columns.Count > 6 Then
                DataGridView1.Columns(6).HeaderText = "Tel No"
                DataGridView1.Columns(6).Width = 110
            End If
            DataGridView1.AllowUserToAddRows = False
            DataGridView1.MultiSelect = False
            DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

            ' Update Total
            Dim tot As Double = 0
            For d As Integer = 0 To table.Rows.Count - 1
                tot += Convert.ToDouble(table.Rows(d)("amount"))
            Next
            Label18.Text = tot.ToString("N2")

            conn.Close()
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub



    Private Sub StartDate_ValueChanged(sender As Object, e As EventArgs) Handles StartDate.ValueChanged
        load_for_date()
    End Sub

    Private Sub EndDate_ValueChanged(sender As Object, e As EventArgs) Handles EndDate.ValueChanged
        load_for_date()
    End Sub

    Private Sub DebitEntry_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        ' F2: Add New shortcut
        If e.KeyCode = Keys.F2 Then
            If TabControl1.SelectedTab Is TabPage2 Then
                DAddNewBtn.PerformClick()
            ElseIf TabControl1.SelectedTab Is TabPage3 Then
                Button10.PerformClick()
            End If
        ElseIf e.KeyCode = Keys.F3 Then
            If TabControl1.SelectedTab Is TabPage2 Then
                DUpdateBtn.PerformClick()
            End If
            ' Delete: Delete shortcut
            ' Logic: Only trigger form-level delete if focus is NOT in a textbox (to allow editing)
        ElseIf e.KeyCode = Keys.Delete Then
            If Not (TypeOf Me.ActiveControl Is TextBox) AndAlso Not (TypeOf Me.ActiveControl Is ComboBox) Then
                If TabControl1.SelectedTab Is TabPage2 Then
                    DDeleteBtn.PerformClick()
                ElseIf TabControl1.SelectedTab Is TabPage3 Then
                    Button12.PerformClick()
                End If
            End If
        ElseIf e.KeyCode = Keys.Escape Then
            If TabControl1.SelectedTab Is TabPage2 Then
                PanelInvoices.Visible = False
                DAmountTxt.Select()
            ElseIf TabControl1.SelectedTab Is TabPage3 Then
                Button13.PerformClick()
            End If
            e.Handled = True
            e.SuppressKeyPress = True
        End If
    End Sub



    Private Sub NameSeachTextBox_TextChanged(sender As Object, e As EventArgs) Handles NameSeachTextBox.TextChanged
        Load_credit_filtered()
    End Sub

    Private Sub Amountbox_TextChanged(sender As Object, e As EventArgs) Handles Amountbox.TextChanged
        Load_credit_filtered()
    End Sub

    Private Sub cmbCreditFilter2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbCreditFilter2.SelectedIndexChanged
        Load_credit_filtered()
    End Sub

    Private Sub cmbCreditFilter1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbCreditFilter1.SelectedIndexChanged
        loaddebit_filtered()
    End Sub

    Private Sub NameTextBox1_TextChanged(sender As Object, e As EventArgs) Handles NameTextBox1.TextChanged
        If isSettingNameProgrammatically Then Return

        If NameTextBox1.Focused Then
            creid = 0
            CreditAmt = 0
            AmountTextBox1.Clear()
            selectedInvNo = ""
            selectedPaymentId = 0 ' Manual name change resets selection state
        End If

        Try
            If conn.State = ConnectionState.Closed Then conn.Open()

            Dim table As New DataTable()
            ' [MODIFIED] Group by supplier_id to show total balance per supplier
            Dim sql As String = "SELECT ANY_VALUE(c.creid) as creid, c.sname, ANY_VALUE(c.inv_no) as inv_no, SUM(c.amount) as amount, MAX(c.getdate) as getdate, s.tel_no, c.supplier_id " &
                               "FROM supplicer_credit c " &
                               "LEFT JOIN supplier s ON c.supplier_id = s.id " &
                               "WHERE c.amount > 0 " &
                               "GROUP BY c.supplier_id, c.sname, s.tel_no"

            Dim adapter As New MySqlDataAdapter(sql, conn)
            adapter.Fill(table)

            Dim dv As New DataView(table)
            dv.RowFilter = String.Format("sname Like '{0}%'", NameTextBox1.Text.Replace("'", "''"))
            dv.Sort = "sname ASC"

            Customer_creditDataGridView1.DataSource = Nothing
            Customer_creditDataGridView1.AutoGenerateColumns = True
            Customer_creditDataGridView1.AllowUserToAddRows = False
            Customer_creditDataGridView1.MultiSelect = False
            Customer_creditDataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect

            Dim bsource As New BindingSource
            bsource.DataSource = dv
            Customer_creditDataGridView1.DataSource = bsource

            If Customer_creditDataGridView1.Columns.Count > 0 Then
                For Each col As DataGridViewColumn In Customer_creditDataGridView1.Columns
                    Select Case col.Name.ToLower()
                        Case "creid", "supplier_id", "inv_no", "getdate" : col.Visible = False ' Hide invoice-specific columns in aggregate view
                        Case "sname" : col.HeaderText = "Supplier Name" : col.Width = 350
                        Case "amount" : col.HeaderText = "Total Balance" : col.Width = 120 : col.DefaultCellStyle.Format = "N2"
                        Case "tel_no" : col.HeaderText = "Tel No" : col.Width = 110
                    End Select
                Next
            End If

            conn.Close()

            ' Logic to show/hide the suggestion panel
            If dv.Count > 0 AndAlso NameTextBox1.Text.Trim() <> "" Then
                Panel7.Visible = True
                Panel7.BringToFront()
            Else
                Panel7.Visible = False
            End If
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub
    ' --------------------------------------------------------------------------------------------------------------------------------------------custem eken gnne gnne-------------------------------------------

    Private Sub NameTextBox1_KeyPress(sender As Object, e As KeyPressEventArgs) Handles NameTextBox1.KeyPress
        ' No longer hiding Panel7 here
    End Sub

    Private Sub NameTextBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles NameTextBox1.KeyDown
        If e.KeyCode = Keys.Escape Then
            Panel7.Visible = False
            e.Handled = True
            e.SuppressKeyPress = True
        End If
        If e.KeyCode = Keys.Up OrElse e.KeyCode = Keys.Down Then
            If Panel7.Visible Then
                Customer_creditDataGridView1.Select()
            End If
        End If
        If e.KeyCode = Keys.Enter Then
            ComboBox1.Select()
            e.Handled = True
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        ClearPaymentFields()
        NameTextBox1.Select()
        CalculatePaymentTotal()
    End Sub

    Private Sub TelNoTextBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles TelNoTextBox1.KeyDown
        If e.KeyCode = Keys.Enter Then
            ComboBox1.Select()
            e.Handled = True
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click
        ProcessPaymentSave()
    End Sub

    Private Sub ProcessPaymentSave()
        If String.IsNullOrWhiteSpace(NameTextBox1.Text) OrElse Val(AmountTextBox1.Text) <= 0 Then
            MessageBox.Show("Please select a supplier and enter a valid amount.")
            Return
        End If

        ' Validate that a payment type is explicitly selected
        If ComboBox1.SelectedIndex = -1 OrElse String.IsNullOrWhiteSpace(ComboBox1.Text) Then
            MessageBox.Show("Please select a payment type.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If isProcessingSave Then Return
        isProcessingSave = True
        Button11.Enabled = False

        ' Variables to track selection and drawer
        Dim actualCreid As Integer = creid
        Dim actualInvNo As String = selectedInvNo
        Dim updatePettyCash As Boolean = False
        Dim balance As Double = Val(AmountTextBox1.Text)

        Try
            ' Extract current supplier ID from Credit list if not already set
            If uid = 0 AndAlso actualCreid > 0 Then
                Try
                    If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()
                    Dim checkIdCmd As New MySqlCommand("SELECT supplier_id FROM supplicer_credit WHERE creid=@cid", MySqlConn)
                    checkIdCmd.Parameters.AddWithValue("@cid", actualCreid)
                    Dim result = checkIdCmd.ExecuteScalar()
                    If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                        uid = Convert.ToInt32(result)
                    End If
                    MySqlConn.Close()
                Catch ex As Exception
                    If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
                End Try
            End If

            ' 1. Distribution Prompt (FIFO or Manual)
            Dim resultPrompt As DialogResult = MessageBox.Show("Do you want to do this against order or invoice or not?", "Payment Distribution", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

            If resultPrompt = DialogResult.No Then
                ' --- MANUAL DISTRIBUTION PATH ---
                Dim distributor As New SupplierManualPaymentDistributor(uid, NameTextBox1.Text, balance, ComboBox1.Text)
                distributor.ShowDialog()

                If distributor.IsSuccess Then
                    currentDistribution = distributor.Distribution

                    ' [NEW] Cash Drawer Prompt
                    If ComboBox1.Text.Trim().ToLower() = "cash" Then
                        If MessageBox.Show("Did you get money from Cash Drawer?", "Cash Drawer", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                            updatePettyCash = True
                        End If
                    End If

                    Dim payTypeLower As String = ComboBox1.Text.Trim().ToLower()
                    If payTypeLower = "cheque" OrElse payTypeLower = "chaque" Then
                        ChqNoTextBox.Clear()
                        BankSearchTextBox.Clear()
                        selectedBankId = 0
                        LoadBanks()
                        ChequeEntryPanel.Location = New Point((Me.ClientSize.Width - ChequeEntryPanel.Width) / 2, (Me.ClientSize.Height - ChequeEntryPanel.Height) / 2)
                        ChequeEntryPanel.Visible = True
                        ChequeEntryPanel.BringToFront()
                        ChqNoTextBox.Focus()
                    Else
                        SaveManualDistributionFinal(updatePettyCash)
                    End If
                End If
                isProcessingSave = False
                Button11.Enabled = True
                Exit Sub
            End If

            ' --- WATERFALL FIFO PATH (Yes) ---
            currentDistribution.Clear()

            ' Fetch all outstanding invoices for this supplier to support cascading and validation
            Dim outstandingInvoices As New List(Of InvoiceItem)
            Try
                If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()
                Using fifoCmd As New MySqlCommand("SELECT creid, inv_no, amount FROM supplicer_credit WHERE supplier_id = @sid AND amount > 0 ORDER BY getdate ASC, creid ASC", MySqlConn)
                    fifoCmd.Parameters.AddWithValue("@sid", uid)
                    Using reader As MySqlDataReader = fifoCmd.ExecuteReader()
                        While reader.Read()
                            outstandingInvoices.Add(New InvoiceItem With {
                                .Creid = Convert.ToInt32(reader("creid")),
                                .InvNo = reader("inv_no").ToString(),
                                .Amount = Convert.ToDouble(reader("amount"))
                            })
                        End While
                    End Using
                End Using

                If outstandingInvoices.Count = 0 Then
                    MessageBox.Show("No outstanding invoices found for this supplier.")
                    MySqlConn.Close()
                    isProcessingSave = False
                    Button11.Enabled = True
                    Exit Sub
                End If

                ' VALIDATION: Check if total entered amount exceeds total outstanding balance
                Dim totalOutstanding As Double = outstandingInvoices.Sum(Function(i) i.Amount)
                If balance > totalOutstanding Then
                    MessageBox.Show("enter debit amount greater than debit remainig")
                    MySqlConn.Close()
                    isProcessingSave = False
                    Button11.Enabled = True
                    Exit Sub
                End If

                MySqlConn.Close()
            Catch ex As Exception
                MessageBox.Show("Error fetching invoices: " & ex.Message)
                If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
                isProcessingSave = False
                Button11.Enabled = True
                Exit Sub
            End Try

            ' [NEW] Cash Drawer Prompt for Single/Multiple Invoice Cascade
            If ComboBox1.Text.Trim().ToLower() = "cash" Then
                If MessageBox.Show("Did you get money from Cash Drawer?", "Cash Drawer", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                    updatePettyCash = True
                End If
            End If

            Dim dates As String = Format(Me.DateTimePicker4.Value, "yyyy-MM-dd HH:mm:ss")
            Try
                If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()
                Dim trans As MySqlTransaction = MySqlConn.BeginTransaction()

                Try
                    Dim payType As String = ComboBox1.Text.Trim()
                    Dim payTypeLower As String = payType.ToLower()
                    Dim remainingPayment As Double = balance

                    ' Handle Cheque: For simplicity and consistency with current logic, 
                    ' if it's a cheque, we still open the panel, but we now know which invoices it will cover.
                    ' However, the user's request for "reduce from next one" implies immediate reduction.
                    ' Cheque reduction is deferred until realization in this app. 
                    ' To keep it simple, I'll store the cascading info in currentDistribution if it's a cheque 
                    ' so SaveChequePaymentFinal can handle it, OR I'll split it here.

                    If payTypeLower = "cheque" OrElse payTypeLower = "chaque" Then
                        trans.Rollback()
                        MySqlConn.Close()

                        ' Build distribution for the cheque
                        currentDistribution.Clear()
                        For Each inv In outstandingInvoices
                            If remainingPayment <= 0 Then Exit For
                            If inv.Amount <= 0 Then Continue For ' Skip zero balances
                            Dim applyAmt As Double = Math.Min(remainingPayment, inv.Amount)
                            currentDistribution.Add(inv.InvNo, applyAmt)
                            remainingPayment -= applyAmt
                        Next

                        ChqNoTextBox.Clear()
                        BankSearchTextBox.Clear()
                        selectedBankId = 0
                        LoadBanks()
                        ChequeEntryPanel.Location = New Point((Me.ClientSize.Width - ChequeEntryPanel.Width) / 2, (Me.ClientSize.Height - ChequeEntryPanel.Height) / 2)
                        ChequeEntryPanel.Visible = True
                        ChequeEntryPanel.BringToFront()
                        ChqNoTextBox.Focus()
                        isProcessingSave = False
                        Button11.Enabled = True
                        Exit Sub
                    End If

                    ' --- CASCADE LOOP FOR CASH/BANK ---
                    For Each inv In outstandingInvoices
                        If remainingPayment <= 0 Then Exit For
                        If inv.Amount <= 0 Then Continue For ' Skip zero balances

                        Dim applyAmt As Double = Math.Min(remainingPayment, inv.Amount)
                        Dim currentCreid As Integer = inv.Creid
                        Dim currentInvNo As String = inv.InvNo

                        ' 1. Save Payment Record
                        Dim Query As String = "INSERT INTO supplier_payments (name, type, amount, pdate, supplier_id, creid, inv_no) VALUES (@name, @type, @amt, @dt, @sid, @cid, @inv)"
                        Using cmdPay As New MySqlCommand(Query, MySqlConn, trans)
                            cmdPay.Parameters.AddWithValue("@name", NameTextBox1.Text)
                            cmdPay.Parameters.AddWithValue("@type", payType)
                            cmdPay.Parameters.AddWithValue("@amt", applyAmt)
                            cmdPay.Parameters.AddWithValue("@dt", dates)
                            cmdPay.Parameters.AddWithValue("@sid", If(uid > 0, uid, DBNull.Value))
                            cmdPay.Parameters.AddWithValue("@cid", If(currentCreid > 0, currentCreid, DBNull.Value))
                            cmdPay.Parameters.AddWithValue("@inv", currentInvNo)
                            cmdPay.ExecuteNonQuery()
                        End Using

                        ' 2. Deduction from supplicer_credit (Safe from negative UNSIGNED)
                        Using cmdDeduct As New MySqlCommand("UPDATE supplicer_credit SET amount = GREATEST(0, amount - @amt) WHERE creid = @cid", MySqlConn, trans)
                            cmdDeduct.Parameters.AddWithValue("@amt", applyAmt)
                            cmdDeduct.Parameters.AddWithValue("@cid", currentCreid)
                            cmdDeduct.ExecuteNonQuery()
                        End Using

                        ' 3. Update Purchasing
                        If Not String.IsNullOrEmpty(currentInvNo) Then
                            Using cmdPur As New MySqlCommand("UPDATE purchasing SET credit_balance_due = GREATEST(0, credit_balance_due - @amt), balance_due = GREATEST(0, balance_due - @amt), paid_amount = paid_amount + @amt WHERE TRIM(pur_id) = TRIM(@pid) AND supplier_id = @sid", MySqlConn, trans)
                                cmdPur.Parameters.AddWithValue("@amt", applyAmt)
                                cmdPur.Parameters.AddWithValue("@pid", currentInvNo)
                                cmdPur.Parameters.AddWithValue("@sid", uid)
                                cmdPur.ExecuteNonQuery()
                            End Using

                            Using cmdStatus As New MySqlCommand("UPDATE purchasing SET status = CASE " &
                                                         "WHEN balance_due <= 0 THEN 'success' " &
                                                         "WHEN credit_balance_due > 0 AND cheque_balance_due > 0 AND paid_amount > 0 THEN 'Mixed_Payment' " &
                                                         "WHEN credit_balance_due > 0 AND cheque_balance_due > 0 THEN 'Credit_Cheque' " &
                                                         "WHEN credit_balance_due > 0 AND paid_amount > 0 THEN 'cash_Credit' " &
                                                         "WHEN credit_balance_due > 0 THEN 'Credit' " &
                                                         "WHEN cheque_balance_due > 0 AND paid_amount > 0 THEN 'Cash_Cheque' " &
                                                         "WHEN cheque_balance_due > 0 THEN 'Cheque' " &
                                                         "ELSE status END " &
                                                         "WHERE TRIM(pur_id) = TRIM(@pid) AND supplier_id = @sid", MySqlConn, trans)
                                cmdStatus.Parameters.AddWithValue("@pid", currentInvNo)
                                cmdStatus.Parameters.AddWithValue("@sid", uid)
                                cmdStatus.ExecuteNonQuery()
                            End Using
                        End If

                        remainingPayment -= applyAmt
                    Next

                    ' [NEW] Update Petty Cash (Once for the total amount)
                    Dim isCashFinal As Boolean = payType.Trim().Equals("Cash", StringComparison.OrdinalIgnoreCase)
                    If isCashFinal AndAlso updatePettyCash Then
                        Module1.RegisterCashTransaction(balance, "OUT", "Supplier Payment (FIFO) - " & NameTextBox1.Text, "FIFO CASCADE", customDate:=dates)
                    End If

                    trans.Commit()
                    MessageBox.Show("Payment Saved and Distributed Successfully.")
                    ClearPaymentFields()
                    LoadPayments()
                    Load_credit_filtered()
                    loaddebit_filtered()

                Catch ex As Exception
                    trans.Rollback()
                    MessageBox.Show("Payment Error: " & ex.Message)
                End Try
                MySqlConn.Close()
            Catch ex As Exception
                MessageBox.Show("DB Error: " & ex.Message)
            End Try
        Finally
            isProcessingSave = False
            Button11.Enabled = True
        End Try
    End Sub

    ' Helper class for invoice cascading
    Private Class InvoiceItem
        Public Property Creid As Integer
        Public Property InvNo As String
        Public Property Amount As Double
    End Class

    Private Sub Print_Click(sender As Object, e As EventArgs) Handles Print.Click
        Try
            Dim rptDoc As New supplierdebit()

            ' Dynamic field identification logic
            Dim tableName As String = "supplicer_credit"
            For Each tbl As Table In rptDoc.Database.Tables
                If tbl.Name.ToLower().Contains("supplicer_credit") OrElse tbl.Name.ToLower().Contains("supplier_credit") OrElse tbl.Name.Equals("Command", StringComparison.OrdinalIgnoreCase) Then
                    tableName = tbl.Name
                    Exit For
                End If
            Next

            ' Build RecordSelectionFormula matching grid filters
            Dim filters As New List(Of String)

            ' 1. Name Filter (TextBox6)
            If TextBox6.Text.Trim() <> "" Then
                filters.Add("UpperCase({" & tableName & ".sname}) startswith '" & TextBox6.Text.Trim().ToUpper().Replace("'", "''") & "'")
            End If

            ' 2. Invoice Filter (TextBox4)
            If TextBox4.Text.Trim() <> "" Then
                filters.Add("UpperCase({" & tableName & ".inv_no}) startswith '" & TextBox4.Text.Trim().ToUpper().Replace("'", "''") & "'")
            End If

            ' 3. Status Filter (cmbCreditFilter1)
            If cmbCreditFilter1.Text = "Pending" Then
                filters.Add("{" & tableName & ".amount} > 0")
            ElseIf cmbCreditFilter1.Text = "Paid" Then
                filters.Add("{" & tableName & ".amount} = 0")
            End If

            If filters.Count > 0 Then
                rptDoc.RecordSelectionFormula = String.Join(" AND ", filters)
            End If

            ' Hand over to SaleInv for centralized display/printing
            SaleInv.ShowReport(rptDoc, 9)

        Catch ex As Exception
            MessageBox.Show("Error generating report: " & ex.Message, "Report Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnprint_Click(sender As Object, e As EventArgs) Handles btnprint.Click
        Try
            Dim rptDoc As New supplierpayment()

            ' Dynamic table identification logic and Cartesian Product Prevention
            Dim hasPayments As Boolean = False
            Dim hasCredit As Boolean = False
            Dim hasSupplier As Boolean = False
            Dim paymentTable As String = "supplier_payments"
            Dim creditTable As String = "supplicer_credit"
            Dim supplierTable As String = "supplier"

            For Each tbl As Table In rptDoc.Database.Tables
                Dim tName As String = tbl.Name.ToLower()
                If tName.Contains("supplier_payments") OrElse tName.Contains("payment") Then
                    paymentTable = tbl.Name
                    hasPayments = True
                ElseIf tName.Contains("supplicer_credit") OrElse tName.Contains("supplier_credit") Then
                    creditTable = tbl.Name
                    hasCredit = True
                ElseIf tName = "supplier" OrElse tName = "supplier1" Then
                    supplierTable = tbl.Name
                    hasSupplier = True
                End If
            Next

            ' Build RecordSelectionFormula matching grid filters
            Dim filters As New List(Of String)

            ' Prevent Cartesian Product by injecting Missing Joins dynamically
            If hasPayments AndAlso hasCredit Then
                filters.Add("{" & paymentTable & ".creid} = {" & creditTable & ".creid}")
            End If
            
            If hasPayments AndAlso hasSupplier Then
                filters.Add("{" & paymentTable & ".supplier_id} = {" & supplierTable & ".id}")
            End If

            ' 1. Name Filter (TextBox9)
            If TextBox9.Text.Trim() <> "" Then
                filters.Add("UpperCase({" & paymentTable & ".name}) startswith '" & TextBox9.Text.Trim().ToUpper().Replace("'", "''") & "'")
            End If

            ' 2. Invoice Filter (filter_invoice)
            If filter_invoice.Text.Trim() <> "" Then
                filters.Add("UpperCase({" & paymentTable & ".inv_no}) startswith '" & filter_invoice.Text.Trim().ToUpper().Replace("'", "''") & "'")
            End If

            ' 3. Phone Filter (filter_phone)
            If filter_phone.Text.Trim() <> "" AndAlso hasSupplier Then
                filters.Add("{" & supplierTable & ".tel_no} startswith '" & filter_phone.Text.Trim().Replace("'", "''") & "'")
            End If

            ' 4. Pay Type Filter
            If Not String.IsNullOrEmpty(selectedPayTypeFilter) AndAlso selectedPayTypeFilter <> "All" Then
                If selectedPayTypeFilter = "Online Payment" Then
                    filters.Add("({" & paymentTable & ".type} = 'Online Payment' OR {" & paymentTable & ".type} = 'Online Transfer')")
                Else
                    filters.Add("{" & paymentTable & ".type} = '" & selectedPayTypeFilter.Replace("'", "''") & "'")
                End If
            End If

            ' 5. Payment Date Range Filter (matches the grid date filter)
            If PayStartDate IsNot Nothing AndAlso PayEndDate IsNot Nothing Then
                Dim rptStart As String = Format(PayStartDate.Value, "yyyy-MM-dd")
                Dim rptEnd As String = Format(PayEndDate.Value, "yyyy-MM-dd")
                filters.Add("{" & paymentTable & ".pdate} >= Date(" &
                             PayStartDate.Value.Year & "," & PayStartDate.Value.Month & "," & PayStartDate.Value.Day & ") AND " &
                             "{" & paymentTable & ".pdate} <= DateTime(" &
                             PayEndDate.Value.Year & "," & PayEndDate.Value.Month & "," & PayEndDate.Value.Day & ",23,59,59)")
            End If

            If filters.Count > 0 Then
                rptDoc.RecordSelectionFormula = String.Join(" AND ", filters)
            End If

            ' Hand over to SaleInv for centralized display/printing
            SaleInv.ShowReport(rptDoc, 10)

        Catch ex As Exception
            MessageBox.Show("Error generating report: " & ex.Message, "Report Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub SaveManualDistributionFinal(Optional updateDrawer As Boolean = False)
        If currentDistribution Is Nothing OrElse currentDistribution.Count = 0 Then Return

        Dim dates As String = DateTimePicker4.Value.ToString("yyyy-MM-dd HH:mm:ss")
        Dim payType As String = ComboBox1.Text.Trim()
        Dim payTypeLower As String = payType.ToLower()
        Dim isCheque As Boolean = (payTypeLower = "cheque" OrElse payTypeLower = "chaque")

        Dim chqNo As String = If(isCheque, ChqNoTextBox.Text.Trim(), "")
        If isCheque Then
            chqNo = chqNo.Replace("'", "").Replace("""", "").Replace("\", "").Replace(";", "")
        End If

        Try
            If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()
            Dim trans As MySqlTransaction = MySqlConn.BeginTransaction()

            Try
                ' [NEW] Update Petty Cash for Manual Distribution
                Dim isCashManual As Boolean = payType.Trim().Equals("Cash", StringComparison.OrdinalIgnoreCase)
                If isCashManual AndAlso updateDrawer Then
                    Dim totalDistAmt As Double = currentDistribution.Values.Sum()
                    Module1.RegisterCashTransaction(totalDistAmt, "OUT", "Supplier Payment (Distributed) - " & NameTextBox1.Text, "DISTRIBUTED", customDate:=dates)
                End If

                For Each kvp In currentDistribution
                    Dim invNo As String = kvp.Key
                    Dim appliedAmt As Double = kvp.Value

                    ' Find creid for this invoice
                    Dim targetCreid As Integer = 0
                    Using findCmd As New MySqlCommand("SELECT creid FROM supplicer_credit WHERE supplier_id = @sid AND inv_no = @inv AND amount >= @amt LIMIT 1", MySqlConn, trans)
                        findCmd.Parameters.AddWithValue("@sid", uid)
                        findCmd.Parameters.AddWithValue("@inv", invNo)
                        findCmd.Parameters.AddWithValue("@amt", appliedAmt)
                        Dim result = findCmd.ExecuteScalar()
                        If result IsNot Nothing Then targetCreid = Convert.ToInt32(result)
                    End Using

                    ' 1. Insert into supplier_payments
                    Dim Query As String = ""
                    If isCheque Then
                        Query = "INSERT INTO supplier_payments (name, type, amount, pdate, supplier_id, creid, inv_no, chq_no, bank_id) VALUES (@name, @type, @amt, @dt, @sid, @cid, @inv, @chq, @bid)"
                    Else
                        Query = "INSERT INTO supplier_payments (name, type, amount, pdate, supplier_id, creid, inv_no) VALUES (@name, @type, @amt, @dt, @sid, @cid, @inv)"
                    End If

                    Using cmdPay As New MySqlCommand(Query, MySqlConn, trans)
                        cmdPay.Parameters.AddWithValue("@name", NameTextBox1.Text)
                        cmdPay.Parameters.AddWithValue("@type", payType)
                        cmdPay.Parameters.AddWithValue("@amt", appliedAmt)
                        cmdPay.Parameters.AddWithValue("@dt", dates)
                        cmdPay.Parameters.AddWithValue("@sid", If(uid > 0, uid, DBNull.Value))
                        cmdPay.Parameters.AddWithValue("@cid", If(targetCreid > 0, targetCreid, DBNull.Value))
                        cmdPay.Parameters.AddWithValue("@inv", invNo)
                        If isCheque Then
                            cmdPay.Parameters.AddWithValue("@chq", chqNo)
                            cmdPay.Parameters.AddWithValue("@bid", selectedBankId)
                        End If
                        cmdPay.ExecuteNonQuery()
                    End Using

                    ' 2. Update supplicer_credit & purchasing (Deducted immediately on payment receipt)
                    If targetCreid > 0 Then
                        Using cmdDeduct As New MySqlCommand("UPDATE supplicer_credit SET amount = GREATEST(0, amount - @amt) WHERE creid = @cid", MySqlConn, trans)
                            cmdDeduct.Parameters.AddWithValue("@amt", appliedAmt)
                            cmdDeduct.Parameters.AddWithValue("@cid", targetCreid)
                            cmdDeduct.ExecuteNonQuery()
                        End Using
                    End If

                    ' Update purchasing (Safe from negative UNSIGNED)
                    Using cmdPur As New MySqlCommand("UPDATE purchasing SET credit_balance_due = GREATEST(0, credit_balance_due - @amt), balance_due = GREATEST(0, balance_due - @amt), paid_amount = paid_amount + @amt WHERE TRIM(pur_id) = TRIM(@pid) AND supplier_id = @sid", MySqlConn, trans)
                        cmdPur.Parameters.AddWithValue("@amt", appliedAmt)
                        cmdPur.Parameters.AddWithValue("@pid", invNo)
                        cmdPur.Parameters.AddWithValue("@sid", uid)
                        cmdPur.ExecuteNonQuery()
                    End Using

                    ' Always Update status
                    Using cmdStatus As New MySqlCommand("UPDATE purchasing SET status = CASE " &
                                                 "WHEN balance_due <= 0 THEN 'success' " &
                                                 "WHEN credit_balance_due > 0 AND cheque_balance_due > 0 AND paid_amount > 0 THEN 'Mixed_Payment' " &
                                                 "WHEN credit_balance_due > 0 AND cheque_balance_due > 0 THEN 'Credit_Cheque' " &
                                                 "WHEN credit_balance_due > 0 AND paid_amount > 0 THEN 'cash_Credit' " &
                                                 "WHEN credit_balance_due > 0 THEN 'Credit' " &
                                                 "WHEN cheque_balance_due > 0 AND paid_amount > 0 THEN 'Cash_Cheque' " &
                                                 "WHEN cheque_balance_due > 0 THEN 'Cheque' " &
                                                 "ELSE status END " &
                                                 "WHERE TRIM(pur_id) = TRIM(@pid) AND supplier_id = @sid", MySqlConn, trans)
                        cmdStatus.Parameters.AddWithValue("@pid", invNo)
                        cmdStatus.Parameters.AddWithValue("@sid", uid)
                        cmdStatus.ExecuteNonQuery()
                    End Using
                Next

                trans.Commit()
                MessageBox.Show("Distributed Payment Saved Successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                ClearPaymentFields()
                currentDistribution.Clear()
                LoadPayments()
                Load_credit_filtered()
                loaddebit_filtered()

            Catch ex As Exception
                trans.Rollback()
                MessageBox.Show("Error during manual distribution: " & ex.Message)
            End Try
            MySqlConn.Close()
        Catch ex As Exception
            MessageBox.Show("Database Connection Error: " & ex.Message)
        End Try
    End Sub

    Private Sub Customer_creditDataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles Customer_creditDataGridView1.CellClick
        If e.RowIndex >= 0 Then
            Dim itm As Integer = e.RowIndex
            selectedPaymentId = 0

            ' Aggregated row: Clear specific invoice selection
            creid = 0
            selectedInvNo = ""
            uid = Convert.ToInt32(Customer_creditDataGridView1.Rows(itm).Cells("supplier_id").Value)
            CreditAmt = Convert.ToDouble(Customer_creditDataGridView1.Rows(itm).Cells("amount").Value)

            isSettingNameProgrammatically = True
            NameTextBox1.Text = Customer_creditDataGridView1.Rows(itm).Cells("sname").Value.ToString
            If Not IsDBNull(Customer_creditDataGridView1.Rows(itm).Cells("tel_no").Value) Then
                TelNoTextBox1.Text = Customer_creditDataGridView1.Rows(itm).Cells("tel_no").Value.ToString
            Else
                TelNoTextBox1.Clear()
            End If
            isSettingNameProgrammatically = False

            Label16.Text = CreditAmt.ToString("N2")
            ComboBox1.Select()
            Panel7.Visible = False
        End If
    End Sub

    Private Sub Customer_creditDataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles Customer_creditDataGridView1.CellContentClick
        Customer_creditDataGridView1_CellClick(sender, e)
    End Sub

    Private Sub Customer_creditDataGridView1_KeyDown(sender As Object, e As KeyEventArgs) Handles Customer_creditDataGridView1.KeyDown
        If e.KeyCode = Keys.Enter Then
            If Customer_creditDataGridView1.CurrentRow IsNot Nothing Then
                Dim itm As Integer = Customer_creditDataGridView1.CurrentRow.Index

                selectedPaymentId = 0
                creid = 0
                selectedInvNo = ""
                uid = Convert.ToInt32(Customer_creditDataGridView1.Rows(itm).Cells("supplier_id").Value)
                CreditAmt = Convert.ToDouble(Customer_creditDataGridView1.Rows(itm).Cells("amount").Value)

                isSettingNameProgrammatically = True
                NameTextBox1.Text = Customer_creditDataGridView1.Rows(itm).Cells("sname").Value.ToString
                If Not IsDBNull(Customer_creditDataGridView1.Rows(itm).Cells("tel_no").Value) Then
                    TelNoTextBox1.Text = Customer_creditDataGridView1.Rows(itm).Cells("tel_no").Value.ToString
                Else
                    TelNoTextBox1.Clear()
                End If
                isSettingNameProgrammatically = False

                Label16.Text = CreditAmt.ToString("N2")
                ComboBox1.Select()
                Panel7.Visible = False

                e.Handled = True
                e.SuppressKeyPress = True
            End If
        ElseIf e.KeyCode = Keys.Down OrElse e.KeyCode = Keys.Up Then
            ' Just swallow arrows to stop focus movement if grid is open
            If Panel7.Visible Then
                ' Focus is already moved by MyBase logic likely, but we can be explicit
                Customer_creditDataGridView1.Select()
            End If
        End If
    End Sub

    Private Sub ComboBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles ComboBox1.KeyDown
        If e.KeyCode = Keys.Enter Then
            AmountTextBox1.Select()
            e.Handled = True
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub SaveChequePaymentFinal()
        Dim dates As String = DateTimePicker4.Value.ToString("yyyy-MM-dd HH:mm:ss")
        Dim balance As Double = 0
        Double.TryParse(AmountTextBox1.Text, balance)

        ' Sanitize Cheque Number: Trim, UpperCase, and remove ', ", \, ;
        Dim chqNo As String = ChqNoTextBox.Text.Trim()
        chqNo = chqNo.Replace("'", "").Replace("""", "").Replace("\", "").Replace(";", "")
        ' Update UI with sanitized value
        ChqNoTextBox.Text = chqNo

        Try
            If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()

            Dim Query As String = "INSERT INTO supplier_payments (name, type, amount, pdate, supplier_id, creid, inv_no, chq_no, bank_id) VALUES (@name, @type, @amt, @dt, @sid, @cid, @inv, @chq, @bid)"
            COMMAND = New MySqlCommand(Query, MySqlConn)
            COMMAND.Parameters.AddWithValue("@name", NameTextBox1.Text)
            COMMAND.Parameters.AddWithValue("@type", ComboBox1.Text.Trim())
            COMMAND.Parameters.AddWithValue("@amt", balance)
            COMMAND.Parameters.AddWithValue("@dt", dates)
            COMMAND.Parameters.AddWithValue("@sid", If(uid > 0, uid, DBNull.Value))
            COMMAND.Parameters.AddWithValue("@cid", If(creid > 0, creid, DBNull.Value))
            COMMAND.Parameters.AddWithValue("@inv", selectedInvNo)
            COMMAND.Parameters.AddWithValue("@chq", chqNo)
            COMMAND.Parameters.AddWithValue("@bid", selectedBankId)
            COMMAND.ExecuteNonQuery()

            ' SYNC WITH CREDIT AND PURCHASING: Subtract from credit balance and potentially update status
            ' DEFERRED FOR CHEQUES: Payment cheques against credit now update balances only upon REalisation in ChaqueOut.vb
            ' This avoids double-reduction and ensures status transitions correctly when cleared.

            ' (Optional: If you want to still shift to cheque_balance_due, keep this, but the user requested NOT to reduce from it)
            ' Based on user request "not reduce from cheque_balance_due", we stop the shift here.

            If Not String.IsNullOrEmpty(selectedInvNo) Then
                ' Deduct from supplicer_credit and purchasing immediately
                If creid > 0 Then
                    Using cmdDeduct As New MySqlCommand("UPDATE supplicer_credit SET amount = GREATEST(0, amount - @amt) WHERE creid = @cid", MySqlConn)
                        cmdDeduct.Parameters.AddWithValue("@amt", balance)
                        cmdDeduct.Parameters.AddWithValue("@cid", creid)
                        cmdDeduct.ExecuteNonQuery()
                    End Using
                End If

                Using cmdPur As New MySqlCommand("UPDATE purchasing SET credit_balance_due = GREATEST(0, credit_balance_due - @amt), balance_due = GREATEST(0, balance_due - @amt), paid_amount = paid_amount + @amt WHERE TRIM(pur_id) = TRIM(@pid) AND supplier_id = @sid", MySqlConn)
                    cmdPur.Parameters.AddWithValue("@amt", balance)
                    cmdPur.Parameters.AddWithValue("@pid", selectedInvNo)
                    cmdPur.Parameters.AddWithValue("@sid", uid)
                    cmdPur.ExecuteNonQuery()
                End Using

                Dim syncStatusQuery As String = "UPDATE purchasing SET status = CASE " &
                                                "WHEN balance_due <= 0 THEN 'success' " &
                                                "WHEN credit_balance_due > 0 AND cheque_balance_due > 0 AND paid_amount > 0 THEN 'Mixed_Payment' " &
                                                "WHEN credit_balance_due > 0 AND cheque_balance_due > 0 THEN 'Credit_Cheque' " &
                                                "WHEN credit_balance_due > 0 AND paid_amount > 0 THEN 'cash_Credit' " &
                                                "WHEN credit_balance_due > 0 THEN 'Credit' " &
                                                "WHEN cheque_balance_due > 0 AND paid_amount > 0 THEN 'Cash_Cheque' " &
                                                "WHEN cheque_balance_due > 0 THEN 'Cheque' " &
                                                "ELSE status END " &
                                                "WHERE TRIM(pur_id) = TRIM(@pid) AND supplier_id = @sid"
                Dim syncStatusCmd As New MySqlCommand(syncStatusQuery, MySqlConn)
                syncStatusCmd.Parameters.AddWithValue("@pid", selectedInvNo)
                syncStatusCmd.Parameters.AddWithValue("@sid", If(uid > 0, uid, DBNull.Value))
                syncStatusCmd.ExecuteNonQuery()
            End If

            MessageBox.Show("Cheque payment recorded successfully. It will appear in the Clearing list.")

            ClearPaymentFields()
            LoadPayments()
            loaddebit_filtered()
            Load_credit_filtered()

            MySqlConn.Close()
        Catch ex As Exception
            MessageBox.Show("Error processing cheque payment: " & ex.Message)
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Sub

    Private Sub AmountTextBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles AmountTextBox1.KeyDown
        If e.KeyCode = Keys.Enter Then
            ProcessPaymentSave()
            e.Handled = True
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub AmountTextBox1_KeyPress(sender As Object, e As KeyPressEventArgs) Handles AmountTextBox1.KeyPress
        ' Validation moved to Save Button
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) AndAlso (e.KeyChar <> "."c) Then
            e.Handled = True
        End If
    End Sub

    Private Sub Button13_Click(sender As Object, e As EventArgs) Handles Button13.Click
        Panel7.Visible = False
        AmountTextBox1.Select()
        CalculatePaymentTotal()
    End Sub

    Private Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click
        If CustomerPaymentsView.CurrentRow Is Nothing Then Exit Sub

        ' 1. Check User Selection
        If ComboBox2.Text = "" Then
            MessageBox.Show("Please Select User First", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            ComboBox2.Focus()
            Exit Sub
        End If

        ' 2. Secure Key Validation
        Try
            If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()
            Dim keyCmd As New MySqlCommand("SELECT hiddenSecureKey FROM user WHERE name=@uname AND (status IS NULL OR status = 'active')", MySqlConn)
            keyCmd.Parameters.AddWithValue("@uname", ComboBox2.Text)
            Dim storedKey = keyCmd.ExecuteScalar()
            MySqlConn.Close()

            If storedKey Is Nothing OrElse IsDBNull(storedKey) OrElse storedKey.ToString() <> secure_key1.Text Then
                MessageBox.Show("You Are Not Authorized To Delete This Item.", "Security Alert", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                secure_key1.Clear()
                secure_key1.Focus()
                Exit Sub
            End If
        Catch ex As Exception
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Open()
            MessageBox.Show("Security Verification Error: " & ex.Message)
            Exit Sub
        End Try

        Dim result As DialogResult = MessageBox.Show("Are you Sure to Delete This Payment?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If result = DialogResult.Yes Then
            Dim selRow As Integer = CustomerPaymentsView.CurrentRow.Index
            Dim paymentId As Integer = CustomerPaymentsView.Rows(selRow).Cells("id").Value
            Dim paidAmt As Double = Convert.ToDouble(CustomerPaymentsView.Rows(selRow).Cells("amount").Value)
            Dim invNo As String = If(CustomerPaymentsView.Rows(selRow).Cells("inv_no").Value?.ToString(), "")
            Dim payType As String = If(CustomerPaymentsView.Rows(selRow).Cells("type").Value?.ToString(), "")
            Dim originalCreid As Integer = 0

            If Not IsDBNull(CustomerPaymentsView.Rows(selRow).Cells("creid").Value) Then
                originalCreid = Convert.ToInt32(CustomerPaymentsView.Rows(selRow).Cells("creid").Value)
            End If
            Dim paymentStatus As String = If(CustomerPaymentsView.Rows(selRow).Cells("status").Value?.ToString(), "")
            Dim chqNoToDelete As String = If(CustomerPaymentsView.Rows(selRow).Cells("chq_no").Value?.ToString(), "")
            Dim paymentSupplierId As Integer = 0
            If Not IsDBNull(CustomerPaymentsView.Rows(selRow).Cells("supplier_id").Value) Then
                paymentSupplierId = Convert.ToInt32(CustomerPaymentsView.Rows(selRow).Cells("supplier_id").Value)
            End If

            Try
                If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()

                ' 1. DELETE FROM chaque_issue if it's a cheque
                Dim payTypeLower As String = payType.Trim().ToLower()
                Dim chqDeleted As Boolean = False
                If payTypeLower = "cheque" OrElse payTypeLower = "chaque" Then
                    Dim deleteChqQuery = "DELETE FROM chaque_issue WHERE TRIM(chq_no) = TRIM(@chq) AND TRIM(inv_no) = TRIM(@inv)"
                    Dim deleteChqCmd As New MySqlCommand(deleteChqQuery, MySqlConn)
                    deleteChqCmd.Parameters.AddWithValue("@chq", chqNoToDelete)
                    deleteChqCmd.Parameters.AddWithValue("@inv", invNo)
                    If deleteChqCmd.ExecuteNonQuery() > 0 Then
                        chqDeleted = True
                    End If
                End If

                ' 2. REVERSE BALANCE: Revert requested fields only in purchasing (Step 3 below)

                ' 3. REVERSE PURCHASING: Subtract paid amount and reset status if needed
                If Not String.IsNullOrEmpty(invNo) Then
                    Dim revPurQuery As String = ""

                    If (payTypeLower = "cheque" OrElse payTypeLower = "chaque") AndAlso Not (paymentStatus = "RELEASED" OrElse paymentStatus = "REALISED" OrElse paymentStatus = "REALIZED") Then
                        ' DEFERRED REVERSAL: For payment cheques, balances are only updated in ChaqueOut REalisation.
                        ' If deleting an un-realised cheque payment, there are no balances to revert in purchasing yet.
                        revPurQuery = "SELECT 1" ' No-op but keeps structure
                    Else
                        ' Reverse Cash/Bank OR REALISED/RELEASED Cheque: Add back to credit and balance_due, reduce paid_amount
                        revPurQuery = "UPDATE purchasing SET credit_balance_due = credit_balance_due + @amt, balance_due = balance_due + @amt, paid_amount = paid_amount - @amt WHERE TRIM(pur_id) = TRIM(@pid) AND supplier_id = @sid"
                    End If

                    If revPurQuery <> "SELECT 1" Then
                        Dim revPurCmd As New MySqlCommand(revPurQuery, MySqlConn)
                        revPurCmd.Parameters.AddWithValue("@amt", paidAmt)
                        revPurCmd.Parameters.AddWithValue("@pid", invNo)
                        revPurCmd.Parameters.AddWithValue("@sid", paymentSupplierId)
                        revPurCmd.ExecuteNonQuery()

                        ' [RE-ADDED] Reverse balance in supplicer_credit for paid/cleared items
                        If originalCreid > 0 Then
                            Using revertCmd As New MySqlCommand("UPDATE supplicer_credit SET amount = amount + @amt WHERE creid = @cid", MySqlConn)
                                revertCmd.Parameters.AddWithValue("@amt", paidAmt)
                                revertCmd.Parameters.AddWithValue("@cid", originalCreid)
                                revertCmd.ExecuteNonQuery()
                            End Using
                        End If
                    End If

                    ' 4. REVERT STATUS: Re-calculate the correct status based on all remaining balances
                    Dim revStatusQuery As String = "UPDATE purchasing SET status = CASE " &
                                                 "WHEN balance_due <= 0 THEN 'success' " &
                                                 "WHEN credit_balance_due > 0 AND cheque_balance_due > 0 AND paid_amount > 0 THEN 'Mixed_Payment' " &
                                                 "WHEN credit_balance_due > 0 AND cheque_balance_due > 0 THEN 'Credit_Cheque' " &
                                                 "WHEN credit_balance_due > 0 AND paid_amount > 0 THEN 'cash_Credit' " &
                                                 "WHEN credit_balance_due > 0 THEN 'Credit' " &
                                                 "WHEN cheque_balance_due > 0 AND paid_amount > 0 THEN 'Cash_Cheque' " &
                                                 "WHEN cheque_balance_due > 0 THEN 'Cheque' " &
                                                 "ELSE status END " &
                                                 "WHERE TRIM(pur_id) = TRIM(@pid) AND supplier_id = @sid"
                    Dim revStatusCmd As New MySqlCommand(revStatusQuery, MySqlConn)
                    revStatusCmd.Parameters.AddWithValue("@pid", invNo)
                    revStatusCmd.Parameters.AddWithValue("@sid", paymentSupplierId)
                    revStatusCmd.ExecuteNonQuery()
                End If

                ' 3. DELETE PAYMENT:
                Dim deleteQuery As String = "DELETE FROM supplier_payments WHERE id = @pid"
                Dim deleteCmd As New MySqlCommand(deleteQuery, MySqlConn)
                deleteCmd.Parameters.AddWithValue("@pid", paymentId)
                deleteCmd.ExecuteNonQuery()

                ' Centralized System log deletion
                Module1.LogDeletion("Supplier Payment", paymentId.ToString(), "Supplier ID: " & paymentSupplierId.ToString() & ", Inv No: " & invNo & ", Amount: " & paidAmt.ToString() & ", Type: " & payType)
                If chqDeleted Then
                    Module1.LogDeletion("Supplier Cheque", chqNoToDelete, "Deleted during Payment Reversal. Supplier ID: " & paymentSupplierId.ToString() & ", Inv No: " & invNo & ", Amount: " & paidAmt.ToString())
                End If

                MySqlConn.Close()

                ' 3. REFRESH EVERYTHING
                LoadPayments()
                Load_credit()
                loaddebit()

                MessageBox.Show("Payment Deleted and Balance Reverted Successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Catch ex As Exception
                MessageBox.Show("Delete Error: " & ex.Message)
                If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
            End Try
        End If
    End Sub

    Private Sub ComboBox2_KeyDown(sender As Object, e As KeyEventArgs) Handles ComboBox2.KeyDown
        If e.KeyCode = Keys.Enter Then
            secure_key1.Select()
            e.Handled = True
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub secure_key1_KeyDown(sender As Object, e As KeyEventArgs) Handles secure_key1.KeyDown
        If e.KeyCode = Keys.Enter Then
            Button12.PerformClick()
            e.Handled = True
            e.SuppressKeyPress = True
        End If
    End Sub

    'this part for the debit Add'


    Private Sub DAddNewBtn_Click(sender As Object, e As EventArgs) Handles DAddNewBtn.Click
        ClearDebitFields()
        DCustomerNameTxt.Select()
    End Sub

    Private Sub DSaveBtn_Click(sender As Object, e As EventArgs) Handles DSaveBtn.Click
        ProcessDebitSave()
    End Sub

    Private Sub ProcessDebitSave()
        If isProcessingSave Then Exit Sub

        If DCustomerNameTxt.Text = "" Then
            MsgBox("Please Enter Customere Name")
            DCustomerNameTxt.Focus()
            Exit Sub
        End If

        If DAmountTxt.Text = "" Then
            MsgBox("Please Enter Amount")
            DAmountTxt.Focus()
            Exit Sub
        End If
        If CreIds > 0 Then
            MsgBox("To update an existing record, please use the Update button and provide the secure key.")
            Exit Sub
        End If

        isProcessingSave = True
        DSaveBtn.Enabled = False
        Try
            ' 1. VALIDATION: Check for pending invoices if Inv No is empty
            If String.IsNullOrEmpty(DCustomerInvNoTxt.Text.Trim()) Then
                Try
                    If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()
                    Dim checkInvCmd As New MySqlCommand("SELECT COUNT(*) FROM purchasing WHERE supplier_id = @sid AND LOWER(status) NOT IN ('success', 'paid')", MySqlConn)
                    checkInvCmd.Parameters.AddWithValue("@sid", uid)
                    Dim pendingCount As Integer = Convert.ToInt32(checkInvCmd.ExecuteScalar())
                    MySqlConn.Close()

                    If pendingCount > 0 Then
                        MessageBox.Show("This supplier has " & pendingCount & " pending invoice(s). Please select an invoice from the list (or press Down arrow in the Inv No box) before saving.", "Invoice Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        ' Force the invoice list to show
                        LoadSupplierInvoices(uid)
                        PanelInvoices.Visible = True
                        PanelInvoices.BringToFront()
                        InvoiceDataGridView.Focus()
                        Exit Sub ' Stop the save process
                    End If
                Catch ex As Exception
                    If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
                End Try
            End If

            ' 2. VALIDATION: Ensure supplier exists in database
            ' If uid is 0 (typed manually), try to find an exact match by name
            If uid <= 0 Then
                Try
                    If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()
                    Dim findIdCmd As New MySqlCommand("SELECT id FROM supplier WHERE name = @name", MySqlConn)
                    findIdCmd.Parameters.AddWithValue("@name", DCustomerNameTxt.Text.Trim())
                    Dim result = findIdCmd.ExecuteScalar()
                    If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                        uid = Convert.ToInt32(result)
                    End If
                    MySqlConn.Close()
                Catch ex As Exception
                    If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
                End Try
            End If

            If uid <= 0 Then
                MessageBox.Show("Supplier name not found in database. Please select a valid supplier from the suggestion list or type the name exactly as it appears in the database.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                DCustomerNameTxt.Focus()
                Exit Sub
            End If

            Dim creditDate As String
            creditDate = Format(Me.DateTimePicker1.Value, "yyyy-MM-dd HH:mm:ss")
            Try
                If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()

                ' Duplicate check: check if same supplier, invoice and amount already exists
                Dim checkSql As String = "SELECT COUNT(*) FROM supplicer_credit WHERE supplier_id = @sid AND inv_no = @inv AND amount = @amt"
                Using cmdCheck As New MySqlCommand(checkSql, MySqlConn)
                    cmdCheck.Parameters.AddWithValue("@sid", uid)
                    cmdCheck.Parameters.AddWithValue("@inv", DCustomerInvNoTxt.Text.Trim())
                    cmdCheck.Parameters.AddWithValue("@amt", DAmountTxt.Text.Trim())
                    Dim count As Integer = Convert.ToInt32(cmdCheck.ExecuteScalar())
                    If count > 0 Then
                        MsgBox("already have that")
                        MySqlConn.Close()
                        Return
                    End If
                End Using

                ' VALIDATION & INSERT: Only insert if the supplier_id (uid) exists in the supplier table
                ' Pull the name (sname) directly from the supplier table for ultimate accuracy
                Dim Query As String = "INSERT INTO supplicer_credit (supplier_id, sname, inv_no, amount, getdate) " &
                                    "SELECT id, name, @inv, @amt, @dt FROM supplier WHERE id = @sid"

                COMMAND = New MySqlCommand(Query, MySqlConn)
                COMMAND.Parameters.AddWithValue("@sid", uid)
                COMMAND.Parameters.AddWithValue("@inv", DCustomerInvNoTxt.Text.Trim())
                COMMAND.Parameters.AddWithValue("@amt", DAmountTxt.Text)
                COMMAND.Parameters.AddWithValue("@dt", creditDate)

                Dim rowsAffected As Integer = COMMAND.ExecuteNonQuery()

                If rowsAffected > 0 Then
                    MySqlConn.Close()
                    loaddebit()
                    Load_credit()
                    ClearDebitFields()
                    DCustomerNameTxt.Select()

                    ' MessageBox BEFORE resetting flags
                    MessageBox.Show("Saved Successfully")
                Else
                    MySqlConn.Close()
                    MessageBox.Show("Supplier not found or not selected correctly. Please pick a supplier from the suggestion list.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            Catch ex As Exception
                MessageBox.Show("Credit Insert Error: " & ex.Message)
                If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
            End Try
        Finally
            isProcessingSave = False
            DSaveBtn.Enabled = True
        End Try
    End Sub

    Private Sub DUpdateBtn_Click(sender As Object, e As EventArgs) Handles DUpdateBtn.Click
        If CreIds > 0 Then
            Dim amount As Decimal = 0
            Decimal.TryParse(DAmountTxt.Text, amount)

            If amount <= 0 Then
                MsgBox("Please enter a valid amount.")
                Return
            End If

            ' Inline Secure Key Check since IsSecureKeyValid is not in Module1 for this branch
            If ComboBox3.Text = "" Then
                MessageBox.Show("Please Select User First", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                ComboBox3.Focus()
                Return
            End If

            ' 2. Secure Key Validation
            Try
                If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()
                Dim keyCmd As New MySqlCommand("SELECT hiddenSecureKey FROM user WHERE name=@uname AND (status IS NULL OR status = 'active')", MySqlConn)
                keyCmd.Parameters.AddWithValue("@uname", ComboBox3.Text)
                Dim storedKey = keyCmd.ExecuteScalar()
                MySqlConn.Close()

                If storedKey Is Nothing OrElse IsDBNull(storedKey) OrElse storedKey.ToString() <> secure_key.Text Then
                    MessageBox.Show("You Are Not Authorized To Update This Item", "Security Alert", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                    secure_key.Clear()
                    secure_key.Focus()
                    Return
                End If
            Catch ex As Exception
                If MySqlConn.State = ConnectionState.Open Then MySqlConn.Open()
                MessageBox.Show("Security Verification Error: " & ex.Message)
                Return
            End Try

            Try
                If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
                MySqlConn.Open()

                Dim query As String = "UPDATE supplicer_credit SET amount = @amount, getdate = @date, inv_no = @inv WHERE creid = @creid"
                Using cmd As New MySqlCommand(query, MySqlConn)
                    cmd.Parameters.AddWithValue("@amount", amount)
                    cmd.Parameters.AddWithValue("@date", Format(Me.DateTimePicker1.Value, "yyyy-MM-dd HH:mm:ss"))
                    cmd.Parameters.AddWithValue("@inv", DCustomerInvNoTxt.Text)
                    cmd.Parameters.AddWithValue("@creid", CreIds)

                    Dim rowsAffected As Integer = cmd.ExecuteNonQuery()
                    If rowsAffected > 0 Then
                        MsgBox("Updated Successfully")
                        DAddNewBtn.PerformClick()
                        loaddebit()
                        Load_credit()
                    Else
                        MsgBox("Update failed. Record not found.")
                    End If
                End Using
            Catch ex As Exception
                MsgBox("Error updating record: " & ex.Message)
            Finally
                If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
                secure_key.Text = ""
            End Try
        Else
            MsgBox("Please select a record to update.")
        End If
    End Sub

    Private Sub DDeleteBtn_Click(sender As Object, e As EventArgs) Handles DDeleteBtn.Click
        ' 1. Check User Selection
        If ComboBox3.Text = "" Then
            MessageBox.Show("Please Select User First", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            ComboBox3.Focus()
            Exit Sub
        End If

        ' 2. Secure Key Validation
        Try
            If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()
            Dim keyCmd As New MySqlCommand("SELECT hiddenSecureKey FROM user WHERE name=@uname AND (status IS NULL OR status = 'active')", MySqlConn)
            keyCmd.Parameters.AddWithValue("@uname", ComboBox3.Text)
            Dim storedKey = keyCmd.ExecuteScalar()
            MySqlConn.Close()

            If storedKey Is Nothing OrElse IsDBNull(storedKey) OrElse storedKey.ToString() <> secure_key.Text Then
                MessageBox.Show("You Are Not Authorized To Delete This Item", "Security Alert", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                secure_key.Clear()
                secure_key.Focus()
                Exit Sub
            End If
        Catch ex As Exception
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Open()
            MessageBox.Show("Security Verification Error: " & ex.Message)
            Exit Sub
        End Try

        Dim result As DialogResult = MessageBox.Show("Are you Sure to Delete This", "OR Not", MessageBoxButtons.YesNo)
        If result = DialogResult.Yes Then
            Try
                If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()
                Dim Query As String = "DELETE FROM supplicer_credit WHERE creid = @cid"
                COMMAND = New MySqlCommand(Query, MySqlConn)
                COMMAND.Parameters.AddWithValue("@cid", CreIds)
                COMMAND.ExecuteNonQuery()

                ' Centralized System log deletion
                Module1.LogDeletion("Supplier Credit", CreIds.ToString(), "Supplier: " & DCustomerNameTxt.Text & ", Inv No: " & DCustomerInvNoTxt.Text & ", Amount: " & DAmountTxt.Text)

                MySqlConn.Close()
                loaddebit()
                Load_credit()
                DCustomerNameTxt.Clear()
                DAmountTxt.Clear()
                DCustomerTelNoTxt.Clear()

                DCustomerNameTxt.Select()

            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try
        End If
    End Sub

    Private Sub ComboBox3_KeyDown(sender As Object, e As KeyEventArgs) Handles ComboBox3.KeyDown
        If e.KeyCode = Keys.Enter Then
            secure_key.Select()
            e.Handled = True
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub secure_key_KeyDown(sender As Object, e As KeyEventArgs) Handles secure_key.KeyDown
        If e.KeyCode = Keys.Enter Then
            DDeleteBtn.PerformClick()
            e.Handled = True
            e.SuppressKeyPress = True
        End If
    End Sub



    Private Sub DCustomerNameTxt_TextChanged(sender As Object, e As EventArgs) Handles DCustomerNameTxt.TextChanged
        If isSettingNameProgrammatically Then Return

        If DCustomerNameTxt.Focused Then
            uid = 0
            creid = 0
            DAmountTxt.Clear()
            DCustomerInvNoTxt.Clear()
        End If

        uid = 0 ' Reset ID whenever text is changed manually by user
        If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()
        Dim bsource As New BindingSource
        Dim table As New DataTable()
        Dim adapter As New MySqlDataAdapter("SELECT id, name, address, tel_no, email, register_date, debit_limit, debit_period from supplier ORDER BY name ASC", MySqlConn)
        adapter.Fill(table)
        bsource.DataSource = table
        CustomerDataGridView.DataSource = table
        Dim dv As New DataView(table)
        dv.RowFilter = String.Format("name Like '{0}%'", DCustomerNameTxt.Text.Replace("'", "''"))
        dv.Sort = "name ASC"
        CustomerDataGridView.DataSource = dv

        CustomerDataGridView.Columns(0).HeaderText = "ID"
        CustomerDataGridView.Columns(0).Width = 60
        CustomerDataGridView.Columns(1).HeaderText = "Supplier Name"
        CustomerDataGridView.Columns(1).Width = 300
        CustomerDataGridView.Columns(2).HeaderText = "Address"
        CustomerDataGridView.Columns(2).Width = 150
        CustomerDataGridView.Columns(2).Visible = True
        CustomerDataGridView.Columns(3).HeaderText = "Tel No"
        CustomerDataGridView.Columns(3).Width = 120
        CustomerDataGridView.Columns(3).Visible = True
        CustomerDataGridView.AllowUserToAddRows = False
        CustomerDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        CustomerDataGridView.MultiSelect = False
        CustomerDataGridView.RowHeadersVisible = False

        ' SHOW Suggestion Panel if text matches
        If dv.Count > 0 AndAlso DCustomerNameTxt.Text.Trim() <> "" Then
            Panel8.Visible = True
            Panel8.BringToFront()
        Else
            Panel8.Visible = False
        End If

        MySqlConn.Close()
    End Sub

    Private Sub CustomerDataGridView_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles CustomerDataGridView.CellClick
        If e.RowIndex >= 0 Then
            Dim b As Integer = e.RowIndex
            uid = CustomerDataGridView.Rows(b).Cells(0).Value
            isSettingNameProgrammatically = True
            DCustomerNameTxt.Text = CustomerDataGridView.Rows(b).Cells(1).Value.ToString
            DCustomerTelNoTxt.Text = CustomerDataGridView.Rows(b).Cells(3).Value.ToString
            isSettingNameProgrammatically = False
            DAmountTxt.Select()
            Panel8.Visible = False
            LoadSupplierInvoices(uid)
        End If
    End Sub



    Private Sub LoadSupplierInvoices(supplierId As Integer)
        Try
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
            MySqlConn.Open()

            Dim table As New DataTable()
            Dim statusFilter As String = ""

            ' Determine status filter based on combo selection
            If DInvoiceStatusCombo.Text = "Credit" Then
                statusFilter = " AND status = 'Credit' "
            ElseIf DInvoiceStatusCombo.Text = "cash_Credit" Then
                statusFilter = " AND status = 'cash_Credit' "
            ElseIf DInvoiceStatusCombo.Text = "Mixed_Payment" Then
                statusFilter = " AND status = 'Mixed_Payment' "
            ElseIf DInvoiceStatusCombo.Text = "Credit_Cheque" Then
                statusFilter = " AND status = 'Credit_Cheque' "
            Else
                ' Default / "All" - show these statuses, excluding completed and only where credit balance exists
                statusFilter = " AND status IN ('Credit', 'cash_Credit', 'Mixed_Payment', 'Credit_Cheque') "
            End If

            ' Refined Query - Exclude completed invoices, those with no credit balance, 
            ' AND those already added to the debit list (supplicer_credit)
            Dim query As String = "SELECT pur_id, status, balance_due, credit_balance_due FROM purchasing " &
                                 "WHERE supplier_id = @sid " & statusFilter & " " &
                                 "AND credit_balance_due > 0 " &
                                 "AND pur_id NOT IN (SELECT IFNULL(inv_no,'') FROM supplicer_credit WHERE supplier_id = @sid) " &
                                 "ORDER BY pur_id DESC"

            Dim adapter As New MySqlDataAdapter(query, MySqlConn)
            adapter.SelectCommand.Parameters.AddWithValue("@sid", supplierId)
            adapter.Fill(table)
            InvoiceDataGridView.DataSource = table

            ' Format the grid
            If InvoiceDataGridView.Columns.Count > 0 Then
                InvoiceDataGridView.Columns(0).Width = 150
                InvoiceDataGridView.Columns(0).HeaderText = "Invoice No"
                InvoiceDataGridView.Columns(1).Width = 150
                InvoiceDataGridView.Columns(2).Width = 150
                InvoiceDataGridView.Columns(2).HeaderText = "Balance Due"
                InvoiceDataGridView.Columns(2).DefaultCellStyle.Format = "N2"
                InvoiceDataGridView.Columns(3).Width = 150
                InvoiceDataGridView.Columns(3).HeaderText = "Credit Balance"
                InvoiceDataGridView.Columns(3).DefaultCellStyle.Format = "N2"
            End If

            MySqlConn.Close()

            If table.Rows.Count > 0 Then
                PanelInvoices.Visible = True
                PanelInvoices.BringToFront()
                InvoiceDataGridView.Focus()
            Else
                PanelInvoices.Visible = False
                DAmountTxt.Select()
            End If
        Catch ex As Exception
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
            MessageBox.Show("Error loading invoices: " & ex.Message)
        End Try
    End Sub

    Private Sub DInvoiceStatusCombo_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DInvoiceStatusCombo.SelectedIndexChanged
        If uid > 0 Then
            LoadSupplierInvoices(uid)
        End If
    End Sub

    Private Sub InvoiceDataGridView_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles InvoiceDataGridView.CellClick
        If e.RowIndex >= 0 Then
            Dim row As DataGridViewRow = InvoiceDataGridView.Rows(e.RowIndex)
            DCustomerInvNoTxt.Text = If(row.Cells(0).Value?.ToString(), "")
            DAmountTxt.Text = If(row.Cells(3).Value?.ToString(), "") ' Use credit_balance_due instead of balance_due

            PanelInvoices.Visible = False
            DAmountTxt.Select()
        End If
    End Sub

    Private Sub InvoiceDataGridView_KeyDown(sender As Object, e As KeyEventArgs) Handles InvoiceDataGridView.KeyDown
        If e.KeyCode = Keys.Enter Then
            If InvoiceDataGridView.CurrentRow IsNot Nothing Then
                InvoiceDataGridView_CellClick(Nothing, New DataGridViewCellEventArgs(0, InvoiceDataGridView.CurrentRow.Index))
                e.Handled = True
                e.SuppressKeyPress = True
            End If
        End If
        If e.KeyCode = Keys.Escape Then
            PanelInvoices.Visible = False
            DAmountTxt.Select()
        End If
    End Sub

    Private Sub CustomerDataGridView_KeyDown(sender As Object, e As KeyEventArgs) Handles CustomerDataGridView.KeyDown
        If e.KeyCode = Keys.Enter Then
            ' Process selection and move focus safely
            If CustomerDataGridView.CurrentRow IsNot Nothing Then
                Dim rowIdx As Integer = CustomerDataGridView.CurrentRow.Index
                uid = Convert.ToInt32(CustomerDataGridView.Rows(rowIdx).Cells(0).Value)
                isSettingNameProgrammatically = True
                DCustomerNameTxt.Text = CustomerDataGridView.Rows(rowIdx).Cells(1).Value.ToString()
                DCustomerTelNoTxt.Text = CustomerDataGridView.Rows(rowIdx).Cells(3).Value.ToString()
                isSettingNameProgrammatically = False
                LoadSupplierInvoices(uid)

                e.Handled = True
                e.SuppressKeyPress = True
            End If
        End If
    End Sub

    Private Sub DCustomerNameTxt_KeyDown(sender As Object, e As KeyEventArgs) Handles DCustomerNameTxt.KeyDown
        If e.KeyCode = Keys.Escape Then
            If Panel8.Visible Then
                Panel8.Visible = False
            Else
                Panel8.Visible = False
            End If
        End If
        If e.KeyCode = Keys.Up Then
            CustomerDataGridView.Select()
        End If
        If e.KeyCode = Keys.Down Then
            CustomerDataGridView.Select()
        End If
        If e.KeyCode = Keys.Enter Then
            If Panel8.Visible Then
                CustomerDataGridView.Select()
            Else
                If String.IsNullOrEmpty(DCustomerTelNoTxt.Text) Then
                    DAmountTxt.Select()
                Else
                    DCustomerTelNoTxt.Select()
                End If
            End If
            e.Handled = True
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub DCustomerNameTxt_KeyPress(sender As Object, e As KeyPressEventArgs) Handles DCustomerNameTxt.KeyPress
        If Panel8.Visible Then
            Panel8.Visible = True
        Else
            Panel8.Visible = True
        End If
    End Sub

    Private Sub DCustomerTelNoTxt_KeyDown(sender As Object, e As KeyEventArgs) Handles DCustomerTelNoTxt.KeyDown
        If e.KeyCode = Keys.Enter Then
            DAmountTxt.Select()
            e.Handled = True
            e.SuppressKeyPress = True
        End If
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
        If e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Down Then
            DCustomerInvNoTxt.Select()
            e.Handled = True
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub DCustomerInvNoTxt_KeyDown(sender As Object, e As KeyEventArgs) Handles DCustomerInvNoTxt.KeyDown
        If e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Down Then
            ProcessDebitSave()
            e.Handled = True
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub DCustomerInvNoTxt_KeyPress(sender As Object, e As KeyPressEventArgs) Handles DCustomerInvNoTxt.KeyPress
        If e.KeyChar = Microsoft.VisualBasic.ChrW(Keys.Enter) Then
            e.Handled = True ' Suppress "ding"
        End If
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick

    End Sub

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        If e.RowIndex >= 0 Then
            Dim b As Integer = e.RowIndex
            isSettingNameProgrammatically = True ' Stop TextChanged from resetting uid when we click a row
            DCustomerNameTxt.Text = DataGridView1.Rows(b).Cells(1).Value.ToString
            DCustomerInvNoTxt.Text = If(DataGridView1.Rows(b).Cells(2).Value?.ToString(), "")
            DAmountTxt.Text = DataGridView1.Rows(b).Cells(3).Value.ToString
            CreIds = DataGridView1.Rows(b).Cells(4).Value ' credit record ID

            ' Load the date into DateTimePicker1
            Dim dateVal = DataGridView1.Rows(b).Cells(0).Value
            If dateVal IsNot Nothing AndAlso Not IsDBNull(dateVal) Then
                DateTimePicker1.Value = Convert.ToDateTime(dateVal)
            End If

            ' Capture the supplier_id (uid) so we can update/save correctly
            If DataGridView1.Rows(b).Cells(5).Value IsNot Nothing AndAlso Not IsDBNull(DataGridView1.Rows(b).Cells(5).Value) Then
                uid = Convert.ToInt32(DataGridView1.Rows(b).Cells(5).Value)

                ' Fetch and display telephone number
                Try
                    If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()
                    Dim telCmd As New MySqlCommand("SELECT tel_no FROM supplier WHERE id = @sid", MySqlConn)
                    telCmd.Parameters.AddWithValue("@sid", uid)
                    Dim telResult = telCmd.ExecuteScalar()
                    If telResult IsNot Nothing AndAlso Not IsDBNull(telResult) Then
                        DCustomerTelNoTxt.Text = telResult.ToString()
                    Else
                        DCustomerTelNoTxt.Clear()
                    End If
                    MySqlConn.Close()
                Catch ex As Exception
                    If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
                End Try
            Else
                uid = 0
                DCustomerTelNoTxt.Clear()
            End If

            isSettingNameProgrammatically = False
        End If
    End Sub

    Private Sub DataGridView1_KeyDown(sender As Object, e As KeyEventArgs) Handles DataGridView1.KeyDown
        If e.KeyCode = Keys.Enter Then
            If DataGridView1.CurrentRow IsNot Nothing Then
                DataGridView1_CellClick(Nothing, New DataGridViewCellEventArgs(0, DataGridView1.CurrentRow.Index))
                e.Handled = True
                e.SuppressKeyPress = True
            End If
        End If
    End Sub


    Private Sub TextBox6_TextChanged(sender As Object, e As EventArgs) Handles TextBox6.TextChanged, TextBox4.TextChanged, TextBox5.TextChanged
        loaddebit_filtered()
    End Sub

    Private Sub TextBox6_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox6.KeyDown, TextBox4.KeyDown, TextBox5.KeyDown
        If e.KeyCode = Keys.Up OrElse e.KeyCode = Keys.Down Then
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

    Private Sub TextBox9_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox9.KeyDown, filter_invoice.KeyDown, filter_phone.KeyDown
        If e.KeyCode = Keys.Up OrElse e.KeyCode = Keys.Down Then
            If CustomerPaymentsView.Rows.Count > 0 Then
                CustomerPaymentsView.Focus()
                If CustomerPaymentsView.CurrentCell Is Nothing Then
                    For Each col As DataGridViewColumn In CustomerPaymentsView.Columns
                        If col.Visible Then
                            CustomerPaymentsView.CurrentCell = CustomerPaymentsView.Rows(0).Cells(col.Index)
                            Exit For
                        End If
                    Next
                End If
                e.Handled = True
                e.SuppressKeyPress = True
            End If
        End If
    End Sub

    Private Sub Label2_Click(sender As Object, e As EventArgs)


    End Sub

    Private Sub TabPage1_Click(sender As Object, e As EventArgs) Handles TabPage1.Click

    End Sub
    Private Sub CustomerPaymentsView_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles CustomerPaymentsView.CellClick
        If e.RowIndex >= 0 Then
            Dim b As Integer = e.RowIndex
            isSettingNameProgrammatically = True

            Dim row = CustomerPaymentsView.Rows(b)
            selectedPaymentId = Convert.ToInt32(row.Cells("id").Value) ' Capture existing payment ID
            NameTextBox1.Text = If(row.Cells("name").Value?.ToString(), "")
            ComboBox1.Text = If(row.Cells("type").Value?.ToString(), "")
            AmountTextBox1.Text = If(row.Cells("amount").Value?.ToString(), "")

            Dim dateVal = row.Cells("pdate").Value
            If dateVal IsNot Nothing AndAlso Not IsDBNull(dateVal) Then
                DateTimePicker4.Value = Convert.ToDateTime(dateVal)
            End If

            ' technical IDs
            Dim supplierIdVal = row.Cells("supplier_id").Value
            If supplierIdVal IsNot Nothing AndAlso Not IsDBNull(supplierIdVal) Then
                uid = Convert.ToInt32(supplierIdVal)
            Else
                uid = 0
            End If

            Dim creidVal = row.Cells("creid").Value
            If creidVal IsNot Nothing AndAlso Not IsDBNull(creidVal) Then
                creid = Convert.ToInt32(creidVal)
            Else
                creid = 0
            End If

            ' Fetch telephone
            If uid > 0 Then
                Try
                    If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()
                    Dim telCmd As New MySqlCommand("SELECT tel_no FROM supplier WHERE id = @sid", MySqlConn)
                    telCmd.Parameters.AddWithValue("@sid", uid)
                    Dim telResult = telCmd.ExecuteScalar()
                    If telResult IsNot Nothing AndAlso Not IsDBNull(telResult) Then
                        TelNoTextBox1.Text = telResult.ToString()
                    Else
                        TelNoTextBox1.Clear()
                    End If
                    MySqlConn.Close()
                Catch ex As Exception
                    If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
                End Try
            Else
                TelNoTextBox1.Clear()
            End If

            isSettingNameProgrammatically = False
        End If
    End Sub

    Private Sub CustomerPaymentsView_KeyDown(sender As Object, e As KeyEventArgs) Handles CustomerPaymentsView.KeyDown
        If e.KeyCode = Keys.Enter Then
            If CustomerPaymentsView.CurrentRow IsNot Nothing Then
                CustomerPaymentsView_CellClick(Nothing, New DataGridViewCellEventArgs(0, CustomerPaymentsView.CurrentRow.Index))
                e.Handled = True
                e.SuppressKeyPress = True
            End If
        End If
    End Sub

    Private Sub TextBox9_TextChanged(sender As Object, e As EventArgs) Handles TextBox9.TextChanged, filter_invoice.TextChanged, filter_phone.TextChanged
        LoadPayments()
    End Sub

    Private Sub GroupBox2_Enter(sender As Object, e As EventArgs) Handles GroupBox2.Enter

    End Sub
End Class