Imports MySql.Data.MySqlClient
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared
Public Class CusCuaque
    Dim selectedBankID As Integer = 0
    Dim selectedCustomerID As Integer = 0
    Dim selectedInvoiceNo As String = ""
    Dim isSelecting As Boolean = False
    Dim activeSearchControl As Control = Nothing
    Dim isExistingRecord As Boolean = False
    Dim originalChqNo As String = ""
    Dim originalBankID As Integer = 0
    Dim originalInvNo As String = ""
    Dim originalAmount As Decimal = 0
    Private Enum SearchMode
        Customer
        Bank
        Invoice
    End Enum
    Private currentMode As SearchMode = SearchMode.Customer

    Private Sub load_chaque()

        Try
            ' Only open if not already open
            If MySqlConn.State <> ConnectionState.Open Then
                MySqlConn.Open()
            End If

            Dim table As New DataTable
            ' JOIN with bank table to get bank_name and cheque_returned to get return_reason
            Dim sql As String = "SELECT cr.check_number, cr.check_name, b.bank_name, cr.bank_id, cr.amount, cr.status, " &
                               "rt.return_reason, cr.issue_date, cr.check_release_date, cr.inv_no " &
                               "FROM check_received cr " &
                               "LEFT JOIN bank b ON cr.bank_id = b.id " &
                               "LEFT JOIN cheque_returned rt ON cr.check_number = rt.check_number AND IFNULL(cr.inv_no, '') = IFNULL(rt.inv_no, '') AND cr.bank_id = rt.bank_id"
            Dim adapter As New MySqlDataAdapter(sql, MySqlConn)
            adapter.Fill(table)

            ChaquereceivedDataGridView.DataSource = table

            ' Set column widths and headers
            ChaquereceivedDataGridView.Columns(0).HeaderText = "Chq No"
            ChaquereceivedDataGridView.Columns(0).AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            ChaquereceivedDataGridView.Columns(0).Width = 80   ' check_number
            ChaquereceivedDataGridView.Columns(1).HeaderText = "C Name"
            ChaquereceivedDataGridView.Columns(1).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            ChaquereceivedDataGridView.Columns(2).HeaderText = "Bank Name"
            ChaquereceivedDataGridView.Columns(2).AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            ChaquereceivedDataGridView.Columns(2).Width = 80   ' bank_name
            ChaquereceivedDataGridView.Columns(3).Visible = False  ' bank_id (hidden but available for updates)
            ChaquereceivedDataGridView.Columns(4).HeaderText = "Amount"
            ChaquereceivedDataGridView.Columns(4).AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            ChaquereceivedDataGridView.Columns(4).Width = 100  ' amount
            ChaquereceivedDataGridView.Columns(5).HeaderText = "Status"
            ChaquereceivedDataGridView.Columns(5).AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            ChaquereceivedDataGridView.Columns(5).Width = 85   ' status
            ChaquereceivedDataGridView.Columns(6).HeaderText = "Return Reason"
            ChaquereceivedDataGridView.Columns(6).AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            ChaquereceivedDataGridView.Columns(6).Width = 120  ' return_reason
            ChaquereceivedDataGridView.Columns(7).HeaderText = "Issue Date"
            ChaquereceivedDataGridView.Columns(7).AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            ChaquereceivedDataGridView.Columns(7).Width = 105  ' issue_date
            ChaquereceivedDataGridView.Columns(8).HeaderText = "Release Date"
            ChaquereceivedDataGridView.Columns(8).AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            ChaquereceivedDataGridView.Columns(8).Width = 105  ' check_release_date
            ChaquereceivedDataGridView.Columns(9).HeaderText = "Inv No"
            ChaquereceivedDataGridView.Columns(9).AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            ChaquereceivedDataGridView.Columns(9).Width = 100  ' inv_no
            ChaquereceivedDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None

            MySqlConn.Close()

            ' Calculate total
            Dim sss As Double = 0
            For s As Integer = 0 To ChaquereceivedDataGridView.Rows.Count - 1
                ' Skip the new row placeholder
                If ChaquereceivedDataGridView.Rows(s).IsNewRow Then
                    Continue For
                End If

                ' Column 4 is amount
                If ChaquereceivedDataGridView.Rows(s).Cells(4).Value IsNot Nothing AndAlso Not IsDBNull(ChaquereceivedDataGridView.Rows(s).Cells(4).Value) Then
                    sss = sss + Convert.ToDouble(ChaquereceivedDataGridView.Rows(s).Cells(4).Value)
                End If
            Next

            ' Update the turquoise total label instead of Label7
            Label13.Text = "Total Amount: LKR " & sss.ToString("N2")

            ' Force the DataGridView to refresh
            ChaquereceivedDataGridView.Refresh()

        Catch ex As Exception
            MessageBox.Show("Error loading cheques: " & ex.Message)
        Finally
            If MySqlConn.State = ConnectionState.Open Then
                MySqlConn.Close()
            End If
        End Try
    End Sub

    Private Sub C_NameTextBox_TextChanged(sender As Object, e As EventArgs) Handles C_NameTextBox.TextChanged
        If isSelecting Then Return
        activeSearchControl = C_NameTextBox
        currentMode = SearchMode.Customer

        If String.IsNullOrEmpty(C_NameTextBox.Text) Then
            Panel1.Visible = False
            Return
        End If

        If MySqlConn.State <> ConnectionState.Open Then MySqlConn.Open()
        Try
            Dim table As New DataTable
            ' Fetch id, name and tel_no
            Dim adapter As New MySqlDataAdapter("SELECT id, name, tel_no FROM customer WHERE is_block = 0", MySqlConn)
            adapter.Fill(table)

            Dim dv As New DataView(table)
            dv.RowFilter = String.Format("name Like '{0}%'", C_NameTextBox.Text.Replace("'", "''"))
            dv.Sort = "name ASC"
            CustomerDataGridView.DataSource = dv

            CustomerDataGridView.Columns(0).Visible = False ' ID
            CustomerDataGridView.Columns(1).HeaderText = "Customer Name"
            CustomerDataGridView.Columns(1).Width = 450
            CustomerDataGridView.Columns(2).HeaderText = "Phone"
            CustomerDataGridView.Columns(2).Width = 300
        Finally
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try

        PositionSuggestionPanel()
        Panel1.Visible = True
    End Sub

    Private Sub LoadInvoices(customerID As Integer, Optional customerName As String = "")
        Try
            If MySqlConn.State <> ConnectionState.Open Then MySqlConn.Open()

            ' Use the provided customerID if available, otherwise try to find it from the name
            If customerID = 0 AndAlso Not String.IsNullOrEmpty(customerName) Then
                Dim getIDCmd As New MySqlCommand("SELECT id FROM customer WHERE LOWER(TRIM(name)) = @nameExact OR LOWER(name) LIKE @nameLike LIMIT 1", MySqlConn)
                getIDCmd.Parameters.AddWithValue("@nameExact", customerName.Trim().ToLower())
                getIDCmd.Parameters.AddWithValue("@nameLike", "%" & customerName.Trim().ToLower() & "%")
                Dim idResult = getIDCmd.ExecuteScalar()
                If idResult IsNot Nothing Then customerID = Convert.ToInt32(idResult)
            End If

            ' If still not found by ID or Name, try checking customer_payments table for the name to get ID
            If customerID = 0 AndAlso Not String.IsNullOrEmpty(customerName) Then
                Dim getPayIdCmd As New MySqlCommand("SELECT CusID FROM customer_payments WHERE LOWER(Customer) = @nameExact LIMIT 1", MySqlConn)
                getPayIdCmd.Parameters.AddWithValue("@nameExact", customerName.Trim().ToLower())
                Dim idResult = getPayIdCmd.ExecuteScalar()
                If idResult IsNot Nothing Then customerID = Convert.ToInt32(idResult)
            End If

            If customerID > 0 Then
                selectedCustomerID = customerID
            End If

            ' Unified Query across billing table + customer_payments
            ' Specifically filter by cheque-related statuses and cheque_balance_due > 0
            Dim sql As String = "SELECT GROUP_CONCAT(cc.inv_no SEPARATOR ', ') as inv_no, SUM(cc.balance_due) as balance_due, cc.cheque_no, MAX(cc.timestamps) as timestamps, " &
                                "IF(COUNT(DISTINCT cc.Type) > 1, 'Multiple', MAX(cc.Type)) as Type, cc.bank_id, b.bank_name as Bank FROM (" &
                                "  SELECT b.inv_no, CAST(b.cheque_balance_due AS DECIMAL(10,2)) as balance_due, b.cheque_no, b.timestamps, 'Billing' as Type, b.bank_id FROM billing b " &
                                "  WHERE b.status IN ('Cheque', 'Cash_Cheque', 'Credit_Cheque', 'Mixed_Payment') " &
                                "  AND b.customer_id = @cid AND b.cheque_balance_due > 0 " &
                                "  AND NOT EXISTS (SELECT 1 FROM check_received cr WHERE FIND_IN_SET(b.inv_no, REPLACE(cr.inv_no, ', ', ',')) AND (cr.check_number = b.cheque_no OR b.cheque_no = '') AND LOWER(cr.status) IN ('cleared', 'realised', 'paid', 'success', 'cleared cheque', 'returned', 'returned cheque')) " &
                                "  UNION ALL " &
                                "  SELECT cp.inv_no, CAST(cp.Amount AS DECIMAL(10,2)) as balance_due, cp.cheque_no, cp.Date as timestamps, 'Payment' as Type, cp.bank_id FROM customer_payments cp " &
                                "  WHERE (LOWER(cp.PaymentType) LIKE '%chaque%' OR LOWER(cp.PaymentType) LIKE '%cheque%') AND cp.CusID = @cid " &
                                "  AND NOT EXISTS (SELECT 1 FROM check_received cr WHERE FIND_IN_SET(cp.inv_no, REPLACE(cr.inv_no, ', ', ',')) AND (cr.check_number = cp.cheque_no OR cp.cheque_no = '') AND LOWER(cr.status) IN ('cleared', 'realised', 'paid', 'success', 'cleared cheque', 'returned', 'returned cheque')) " &
                                ") AS cc " &
                                "LEFT JOIN bank b ON cc.bank_id = b.id " &
                                "GROUP BY cc.cheque_no, cc.bank_id " &
                                "ORDER BY timestamps DESC"

            Dim adapter As New MySqlDataAdapter(sql, MySqlConn)
            adapter.SelectCommand.Parameters.AddWithValue("@cid", customerID)
            Dim table As New DataTable()
            adapter.Fill(table)

            CustomerDataGridView.DataSource = table
            currentMode = SearchMode.Invoice

            If table.Rows.Count > 0 Then
                CustomerDataGridView.Columns("inv_no").HeaderText = "Inv No"
                CustomerDataGridView.Columns("inv_no").Width = 120
                CustomerDataGridView.Columns("balance_due").HeaderText = "Amount"
                CustomerDataGridView.Columns("balance_due").Width = 100
                CustomerDataGridView.Columns("cheque_no").HeaderText = "Cheque No"
                CustomerDataGridView.Columns("cheque_no").Width = 150
                CustomerDataGridView.Columns("timestamps").HeaderText = "Date"
                CustomerDataGridView.Columns("timestamps").Width = 150
                If CustomerDataGridView.Columns.Contains("Type") Then
                    CustomerDataGridView.Columns("Type").HeaderText = "Source"
                    CustomerDataGridView.Columns("Type").Width = 100
                End If
                If CustomerDataGridView.Columns.Contains("Bank") Then
                    CustomerDataGridView.Columns("Bank").HeaderText = "Bank"
                    CustomerDataGridView.Columns("Bank").Width = 150
                    CustomerDataGridView.Columns("Bank").Visible = True
                End If
                If CustomerDataGridView.Columns.Contains("bank_id") Then
                    CustomerDataGridView.Columns("bank_id").Visible = False
                End If

                ' Ensure panel is visible and on top
                PositionSuggestionPanel()
                Panel1.BringToFront()
                Panel1.Visible = True
                CustomerDataGridView.Focus()
            Else
                Panel1.Visible = False
                Chq_NoTextBox.Select()
            End If

        Catch ex As Exception
            MessageBox.Show("Error loading invoices: " & ex.Message)
        Finally
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Sub


    Private Sub CusCuaque_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.KeyPreview = True
        Panel1.Visible = False

        ' Configure grids: Full row select, Read-Only, and no extra new row
        With ChaquereceivedDataGridView
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .MultiSelect = False
            .ReadOnly = True
            .AllowUserToAddRows = False
            .RowHeadersVisible = False

            ' Expand font size and row height
            .DefaultCellStyle.Font = New Font("Segoe UI", 11)
            .DefaultCellStyle.ForeColor = Color.Black
            .GridColor = Color.Black
            .RowTemplate.Height = 32
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 11, FontStyle.Bold)
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.Black
        End With

        With CustomerDataGridView
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .MultiSelect = False
            .ReadOnly = True
            .AllowUserToAddRows = False

            ' Expand font size and row height
            .DefaultCellStyle.Font = New Font("Segoe UI", 11)
            .DefaultCellStyle.ForeColor = Color.Black
            .GridColor = Color.Black
            .RowTemplate.Height = 32
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 11, FontStyle.Bold)
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.Black
        End With

        ' Start with "With Out Date" checked so the user sees all data by default
        CheckBox1.Checked = True
        DateTimePicker4.Enabled = False
        DateTimePicker3.Enabled = False

        ' Set specific textboxes to read-only as requested
        Chq_NoTextBox.ReadOnly = False
        BankTextBox.ReadOnly = False
        AmountTextBox.ReadOnly = False

        AddHandler ChaquereceivedDataGridView.CellClick, AddressOf ChaquereceivedDataGridView_CellClick
        ' Initially hide action buttons until a row is selected
        btn_paid.Visible = False
        btn_return.Visible = False

        PerformSearch()
        LoadUserList()
        LoadBanksToSearch()
    End Sub

    Private Sub CustomerDataGridView_KeyDown(sender As Object, e As KeyEventArgs) Handles CustomerDataGridView.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True

            Dim currentRow As DataGridViewRow = CustomerDataGridView.CurrentRow
            If currentRow Is Nothing Then Exit Sub

            If currentMode = SearchMode.Customer Then
                isSelecting = True
                selectedCustomerID = CInt(currentRow.Cells(0).Value) ' Catch ID
                C_NameTextBox.Text = currentRow.Cells(1).Value.ToString()
                isSelecting = False
                Panel1.Visible = False
                Chq_NoTextBox.Select()
            ElseIf currentMode = SearchMode.Invoice Then
                isSelecting = True
                ' Populate fields from selected invoice
                If CustomerDataGridView.Columns.Contains("inv_no") Then
                    selectedInvoiceNo = currentRow.Cells("inv_no").Value.ToString()
                End If
                If CustomerDataGridView.Columns.Contains("cheque_no") Then
                    Chq_NoTextBox.Text = currentRow.Cells("cheque_no").Value.ToString()
                End If
                If CustomerDataGridView.Columns.Contains("balance_due") Then
                    AmountTextBox.Text = currentRow.Cells("balance_due").Value.ToString()
                End If
                isSelecting = False
                Panel1.Visible = False
                ' Try get bank_id
                Dim bankIdToFetch As Integer = 0
                If CustomerDataGridView.Columns.Contains("bank_id") AndAlso Not IsDBNull(currentRow.Cells("bank_id").Value) Then
                    bankIdToFetch = Convert.ToInt32(currentRow.Cells("bank_id").Value)
                End If

                If bankIdToFetch > 0 Then
                    FetchAndPopulateBank(bankIdToFetch)
                Else
                    AutoPopulateBank(selectedInvoiceNo)
                End If
                AmountTextBox.Focus()
            ElseIf currentMode = SearchMode.Bank Then
                ' Bank selection logic
                If CustomerDataGridView.Columns.Contains("id") Then
                    selectedBankID = CInt(currentRow.Cells("id").Value)
                    isSelecting = True
                    BankTextBox.Text = currentRow.Cells("bank_name").Value.ToString()
                    isSelecting = False
                    Panel1.Visible = False
                    AmountTextBox.Select()
                Else
                    ' Fallback
                    If currentRow.Cells.Count >= 2 Then
                        isSelecting = True
                        BankTextBox.Text = currentRow.Cells(1).Value.ToString()
                        isSelecting = False
                        Panel1.Visible = False
                        AmountTextBox.Select()
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub CustomerDataGridView_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles CustomerDataGridView.CellClick
        If e.RowIndex >= 0 Then
            Dim currentRow As DataGridViewRow = CustomerDataGridView.Rows(e.RowIndex)

            If currentMode = SearchMode.Customer Then
                isSelecting = True
                selectedCustomerID = CInt(currentRow.Cells(0).Value) ' Catch ID
                C_NameTextBox.Text = currentRow.Cells(1).Value.ToString()
                isSelecting = False
                Panel1.Visible = False
                Chq_NoTextBox.Select()
            ElseIf currentMode = SearchMode.Invoice Then
                isSelecting = True
                ' Populate fields from selected invoice
                If CustomerDataGridView.Columns.Contains("inv_no") Then
                    selectedInvoiceNo = currentRow.Cells("inv_no").Value.ToString()
                End If
                If CustomerDataGridView.Columns.Contains("cheque_no") Then
                    Chq_NoTextBox.Text = currentRow.Cells("cheque_no").Value.ToString()
                End If
                If CustomerDataGridView.Columns.Contains("balance_due") Then
                    AmountTextBox.Text = currentRow.Cells("balance_due").Value.ToString()
                End If
                isSelecting = False
                Panel1.Visible = False
                ' Try get bank_id
                Dim bankIdToFetch As Integer = 0
                If CustomerDataGridView.Columns.Contains("bank_id") AndAlso Not IsDBNull(currentRow.Cells("bank_id").Value) Then
                    bankIdToFetch = Convert.ToInt32(currentRow.Cells("bank_id").Value)
                End If

                If bankIdToFetch > 0 Then
                    FetchAndPopulateBank(bankIdToFetch)
                Else
                    AutoPopulateBank(selectedInvoiceNo)
                End If
                AmountTextBox.Focus()
            ElseIf currentMode = SearchMode.Bank Then
                ' Bank name is usually index 1 (id is 0)
                If CustomerDataGridView.Columns.Contains("bank_name") Then
                    selectedBankID = CInt(currentRow.Cells("id").Value)
                    isSelecting = True
                    BankTextBox.Text = currentRow.Cells("bank_name").Value.ToString()
                    isSelecting = False
                    Panel1.Visible = False
                    AmountTextBox.Select()
                ElseIf currentRow.Cells.Count >= 2 Then
                    isSelecting = True
                    BankTextBox.Text = currentRow.Cells(1).Value.ToString()
                    isSelecting = False
                    Panel1.Visible = False
                    AmountTextBox.Select()
                End If
            End If
        End If
    End Sub

    Private Sub C_NameTextBox_KeyDown(sender As Object, e As KeyEventArgs) Handles C_NameTextBox.KeyDown
        If e.KeyCode = Keys.Enter Then
            ' If the suggestion panel is visible, pick the first item
            If Panel1.Visible AndAlso CustomerDataGridView.Rows.Count > 0 AndAlso currentMode = SearchMode.Customer Then
                e.SuppressKeyPress = True
                isSelecting = True

                selectedCustomerID = CInt(CustomerDataGridView.Rows(0).Cells(0).Value) ' Catch ID
                C_NameTextBox.Text = CustomerDataGridView.Rows(0).Cells(1).Value.ToString() ' Index 1 is name
                isSelecting = False
                Panel1.Visible = False
                Chq_NoTextBox.Select()
            Else
                ' Normal navigation to next field
                Chq_NoTextBox.Select()
            End If
        ElseIf e.KeyCode = Keys.Down Then
            ' Allow navigating to the grid with Down arrow
            If Panel1.Visible Then
                CustomerDataGridView.Focus()
            End If
        End If
    End Sub
    Private Sub BankTextBox_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles BankTextBox.MouseDoubleClick
        ShowBankAddForm()
    End Sub

    Private Sub ShowBankAddForm()
        Using frm As New bank_add()
            If frm.ShowDialog() = DialogResult.OK Then
                isSelecting = True
                BankTextBox.Text = frm.selectedBankName
                selectedBankID = frm.selectedBankID
                isSelecting = False
                AmountTextBox.Select()
            End If
        End Using
    End Sub
    Private Sub BankTextBox_KeyDown(sender As Object, e As KeyEventArgs) Handles BankTextBox.KeyDown
        If e.KeyCode = Keys.Enter Then
            ' If the suggestion panel is visible, pick the first item
            If Panel1.Visible AndAlso CustomerDataGridView.Rows.Count > 0 Then
                e.SuppressKeyPress = True
                isSelecting = True
                ' Bank name is usually in index 1 for bank search (id is 0)
                If CustomerDataGridView.Columns.Contains("bank_name") Then
                    BankTextBox.Text = CustomerDataGridView.Rows(0).Cells("bank_name").Value.ToString()
                    If CustomerDataGridView.Columns.Contains("id") Then
                        selectedBankID = CInt(CustomerDataGridView.Rows(0).Cells("id").Value)
                    End If
                Else
                    BankTextBox.Text = CustomerDataGridView.Rows(0).Cells(1).Value.ToString()
                End If
                isSelecting = False
                Panel1.Visible = False
                AmountTextBox.Select()
            Else
                AmountTextBox.Select()
            End If
        ElseIf e.KeyCode = Keys.Down Then
            ' Allow navigating to the grid with Down arrow
            If Panel1.Visible Then
                CustomerDataGridView.Focus()
            End If
        End If
    End Sub

    Private Sub C_NameTextBox_Enter(sender As Object, e As EventArgs) Handles C_NameTextBox.Enter
        activeSearchControl = C_NameTextBox
    End Sub

    Private Sub BankTextBox_Enter(sender As Object, e As EventArgs) Handles BankTextBox.Enter
        activeSearchControl = BankTextBox
    End Sub

    Private Sub btn_add_Click(sender As Object, e As EventArgs) Handles btn_add.Click
        ' Clear fields and reset internal selection variables
        ClearFields()

        ' Reset record state and enable save
        isExistingRecord = False
        btn_save.Enabled = True

        ' As requested: go to issue date (Chaq Date)
        Issue_dateDateTimePicker.Select()
    End Sub

    Private Sub AddBank(bankName As String)
        Try
            If MySqlConn.State <> ConnectionState.Open Then MySqlConn.Open()
            Dim cmd As New MySqlCommand("INSERT INTO bank (bank_name, amount) VALUES (@name, 0)", MySqlConn)
            cmd.Parameters.AddWithValue("@name", bankName)
            cmd.ExecuteNonQuery()

            MessageBox.Show("Bank added successfully!")

            ' Refresh visuals
            isSelecting = True
            BankTextBox.Text = bankName
            isSelecting = False
            Panel1.Visible = False
            AmountTextBox.Select()

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        Finally
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Sub

    Private Sub AddCustomer(customerName As String)
        Try
            If MySqlConn.State <> ConnectionState.Open Then MySqlConn.Open()
            ' Insert new customer with default address empty
            Dim cmd As New MySqlCommand("INSERT INTO customer (name, address) VALUES (@name, '')", MySqlConn)
            cmd.Parameters.AddWithValue("@name", customerName)
            cmd.ExecuteNonQuery()

            MessageBox.Show("Customer added successfully!")

            ' Refresh visuals
            isSelecting = True
            C_NameTextBox.Text = customerName
            isSelecting = False
            Panel1.Visible = False
            Chq_NoTextBox.Select()

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        Finally
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Sub
    Private Sub FetchAndPopulateBank(bankId As Integer)
        If bankId <= 0 Then Return
        Try
            If MySqlConn.State <> ConnectionState.Open Then MySqlConn.Open()
            Dim getBankNameQuery As String = "SELECT bank_name FROM bank WHERE id = @id LIMIT 1"
            Using cmd As New MySqlCommand(getBankNameQuery, MySqlConn)
                cmd.Parameters.AddWithValue("@id", bankId)
                Dim bankName = cmd.ExecuteScalar()
                If bankName IsNot Nothing Then
                    isSelecting = True
                    BankTextBox.Text = bankName.ToString()
                    selectedBankID = bankId
                    isSelecting = False
                End If
            End Using
        Catch ex As Exception
            Console.WriteLine("Error fetching bank name: " & ex.Message)
        End Try
    End Sub

    Private Sub AutoPopulateBank(invoiceNo As String)
        If String.IsNullOrEmpty(invoiceNo) Then Return

        Try
            If MySqlConn.State <> ConnectionState.Open Then MySqlConn.Open()

            ' 1. Try get bank_id from billing table
            Dim bankId As Integer = 0
            Dim getBankIdQuery As String = "SELECT bank_id FROM billing WHERE inv_no = @inv LIMIT 1"

            Using cmd As New MySqlCommand(getBankIdQuery, MySqlConn)
                cmd.Parameters.AddWithValue("@inv", invoiceNo)
                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                    bankId = Convert.ToInt32(result)
                End If
            End Using

            ' 2. If not found, try customer_payments
            If bankId = 0 Then
                Dim getBankIdPayQuery As String = "SELECT bank_id FROM customer_payments WHERE inv_no = @inv LIMIT 1"
                Using cmdPay As New MySqlCommand(getBankIdPayQuery, MySqlConn)
                    cmdPay.Parameters.AddWithValue("@inv", invoiceNo)
                    Dim result = cmdPay.ExecuteScalar()
                    If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                        bankId = Convert.ToInt32(result)
                    End If
                End Using
            End If

            ' 3. If bankId > 0, populated
            If bankId > 0 Then
                FetchAndPopulateBank(bankId)
            End If

        Catch ex As Exception
            ' Silent error or log it - depends on preference, but here we'll just not fail the UI
            Console.WriteLine("Error auto-populating bank: " & ex.Message)
        Finally
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Sub

    Private Sub BankTextBox_KeyDown_OLD(sender As Object, e As KeyEventArgs)
        ' This is now handled by the new BankTextBox_KeyDown above
    End Sub

    Private Sub btn_save_Click(sender As Object, e As EventArgs) Handles btn_save.Click
        ' 0. Handle update for existing records
        If isExistingRecord Then
            PerformUpdate()
            Return
        End If

        ' 1. Basic UI Validations
        If Chq_NoTextBox.Text = "" Or C_NameTextBox.Text = "" Or BankTextBox.Text = "" Or AmountTextBox.Text = "" Then
            MessageBox.Show("Please fill in all fields before saving.")
            Return
        End If

        ' 2. Database Operation
        Try
            If MySqlConn.State <> ConnectionState.Open Then MySqlConn.Open()

            ' --- STEP A: GET THE BANK ID FROM THE BANK NAME ---
            Dim bankId As Integer = 0
            Dim getBankIdQuery As String = "SELECT id FROM bank WHERE bank_name = @bname LIMIT 1"

            Using getCmd As New MySqlCommand(getBankIdQuery, MySqlConn)
                getCmd.Parameters.AddWithValue("@bname", BankTextBox.Text.Trim())
                Dim result = getCmd.ExecuteScalar()

                If result IsNot Nothing Then
                    bankId = Convert.ToInt32(result)
                Else
                    MessageBox.Show("The Bank name entered does not exist in the system. Please select a valid bank.")
                    Return
                End If
            End Using

            ' --- STEP B: CHECK FOR DUPLICATE CHEQUE ---
            ' Check using composite key: Chq No + Bank + Inv No
            Dim checkQuery As String = "SELECT COUNT(*) FROM check_received WHERE check_number = @chq AND bank_id = @bid AND IFNULL(inv_no, '') = IFNULL(@inv, '')"
            Using checkCmd As New MySqlCommand(checkQuery, MySqlConn)
                checkCmd.Parameters.AddWithValue("@chq", Chq_NoTextBox.Text.Trim())
                checkCmd.Parameters.AddWithValue("@bid", bankId)
                checkCmd.Parameters.AddWithValue("@inv", selectedInvoiceNo)
                Dim count As Integer = Convert.ToInt32(checkCmd.ExecuteScalar())
                If count > 0 Then
                    MessageBox.Show("This cheque record already exists for this invoice and bank.")
                    MySqlConn.Close()
                    Return
                End If
            End Using

            ' --- STEP C: INSERT THE CHEQUE DATA ---
            Dim insertQuery As String = "INSERT INTO check_received (check_number, check_name, bank_id, amount, status, issue_date, check_release_date, inv_no) " &
                                    "VALUES (@chq, @name, @bank, @amt, @status, @issue, @release, @inv)"

            Using insertCmd As New MySqlCommand(insertQuery, MySqlConn)
                ' Use the formatted dates and trimmed values
                insertCmd.Parameters.AddWithValue("@chq", Chq_NoTextBox.Text.Trim())
                insertCmd.Parameters.AddWithValue("@name", C_NameTextBox.Text.Trim())
                insertCmd.Parameters.AddWithValue("@bank", bankId) ' Now passes the actual Integer ID
                insertCmd.Parameters.AddWithValue("@amt", Convert.ToDecimal(AmountTextBox.Text))
                insertCmd.Parameters.AddWithValue("@status", "Pending")
                insertCmd.Parameters.AddWithValue("@issue", Issue_dateDateTimePicker.Value.ToString("yyyy-MM-dd"))
                insertCmd.Parameters.AddWithValue("@release", Close_dateDateTimePicker.Value.ToString("yyyy-MM-dd"))
                insertCmd.Parameters.AddWithValue("@inv", selectedInvoiceNo)

                insertCmd.ExecuteNonQuery()
            End Using

            MessageBox.Show("Saved Successfully!")

            ' 3. UI Reset
            ClearFields() ' Call your clear method
            PerformSearch() ' Refresh your grid (maintains filters)

        Catch ex As Exception
            MessageBox.Show("Database Error: " & ex.Message)
        Finally
            ' Always close the connection even if it fails
            If MySqlConn.State = ConnectionState.Open Then
                MySqlConn.Close()
            End If
        End Try
    End Sub

    Private Sub Print_Click(sender As Object, e As EventArgs) Handles Print.Click
        Try
            Dim rptDoc As New Customercheque()

            ' Dynamic field identification logic (Refined for Persistent Error)
            Dim mainTable As String = ""
            Dim checkNameField As String = ""
            Dim statusField As String = ""
            Dim chqNoField As String = ""
            Dim releaseDateField As String = ""
            Dim bankTable As String = ""
            Dim bankNameField As String = ""

            ' 1. Identify "Main" table (Priority: Command > check_received > Table with "check")
            For Each tbl As Table In rptDoc.Database.Tables
                If tbl.Name.Equals("Command", StringComparison.OrdinalIgnoreCase) Then
                    mainTable = tbl.Name
                    Exit For
                End If
                If tbl.Name.ToLower().Contains("check") Or tbl.Name.ToLower().Contains("cheque") Then
                    If String.IsNullOrEmpty(mainTable) Then mainTable = tbl.Name
                End If
            Next
            If String.IsNullOrEmpty(mainTable) AndAlso rptDoc.Database.Tables.Count > 0 Then
                mainTable = rptDoc.Database.Tables(0).Name
            End If

            ' 2. Locate fields in the identified main table or fallback
            For Each tbl As Table In rptDoc.Database.Tables
                For Each field As DatabaseFieldDefinition In tbl.Fields
                    Dim fName As String = field.Name.ToLower()

                    ' If field is in mainTable, it takes priority
                    Dim isMain As Boolean = tbl.Name.Equals(mainTable, StringComparison.OrdinalIgnoreCase)

                    ' Identify Customer Name field
                    If (isMain Or String.IsNullOrEmpty(checkNameField)) AndAlso (fName = "check_name" OrElse fName = "name" OrElse fName = "customer_name") Then
                        checkNameField = field.Name
                        If isMain Then mainTable = tbl.Name
                    End If

                    ' Identify Status field
                    If (isMain Or String.IsNullOrEmpty(statusField)) AndAlso fName = "status" Then
                        statusField = field.Name
                        If isMain Then mainTable = tbl.Name
                    End If

                    ' Identify Cheque Number field
                    If (isMain Or String.IsNullOrEmpty(chqNoField)) AndAlso (fName = "check_number" OrElse fName = "chq_no" OrElse fName = "number") Then
                        chqNoField = field.Name
                    End If

                    ' Identify Release Date field
                    If (isMain Or String.IsNullOrEmpty(releaseDateField)) AndAlso (fName = "check_release_date" OrElse fName = "release_date" OrElse fName = "date") Then
                        releaseDateField = field.Name
                    End If

                    ' Identify Bank Name field (Commonly in 'bank' table)
                    If (tbl.Name.ToLower().Contains("bank") Or (isMain And String.IsNullOrEmpty(bankNameField))) AndAlso (fName = "bank_name" OrElse fName = "bank") Then
                        bankTable = tbl.Name
                        bankNameField = field.Name
                    End If
                Next
            Next

            ' Build RecordSelectionFormula matching PerformSearch logic
            Dim formula As New List(Of String)

            If Not String.IsNullOrWhiteSpace(ser_customer.Text) AndAlso Not String.IsNullOrEmpty(checkNameField) Then
                ' Formula: UpperCase({Table.Field}) LIKE '*UPPERVALUE*'
                formula.Add("UpperCase({" & mainTable & "." & checkNameField & "}) LIKE '" & ser_customer.Text.Trim().ToUpper().Replace("'", "''") & "*'")
            End If

            If Not String.IsNullOrWhiteSpace(ser_status.Text) AndAlso Not String.IsNullOrEmpty(statusField) Then
                formula.Add("UpperCase({" & mainTable & "." & statusField & "}) LIKE '" & ser_status.Text.Trim().ToUpper().Replace("'", "''") & "*'")
            End If

            If Not String.IsNullOrWhiteSpace(ser_chq.Text) AndAlso Not String.IsNullOrEmpty(chqNoField) Then
                formula.Add("UpperCase({" & mainTable & "." & chqNoField & "}) LIKE '" & ser_chq.Text.Trim().ToUpper().Replace("'", "''") & "*'")
            End If

            If Not String.IsNullOrWhiteSpace(ser_bank.Text) AndAlso ser_bank.Text <> "All" AndAlso Not String.IsNullOrEmpty(bankNameField) Then
                ' Use the identified bank table, or fallback to mainTable
                Dim bTbl As String = If(Not String.IsNullOrEmpty(bankTable), bankTable, mainTable)
                formula.Add("UpperCase({" & bTbl & "." & bankNameField & "}) LIKE '" & ser_bank.Text.Trim().ToUpper().Replace("'", "''") & "*'")
            End If

            If CheckBox1.Checked = False AndAlso Not String.IsNullOrEmpty(releaseDateField) Then
                Dim startDt As String = DateTimePicker4.Value.ToString("yyyy, MM, dd")
                Dim endDt As String = DateTimePicker3.Value.ToString("yyyy, MM, dd")
                formula.Add("{" & mainTable & "." & releaseDateField & "} >= Date(" & startDt & ") AND {" & mainTable & "." & releaseDateField & "} <= Date(" & endDt & ")")
            End If

            ' Attempt to apply formula with graceful error handling
            If formula.Count > 0 Then
                Try
                    Dim newSelection As String = String.Join(" AND ", formula)
                    ' Preserve existing report formula if any
                    If String.IsNullOrEmpty(rptDoc.RecordSelectionFormula) Then
                        rptDoc.RecordSelectionFormula = newSelection
                    Else
                        rptDoc.RecordSelectionFormula = "(" & rptDoc.RecordSelectionFormula & ") AND (" & newSelection & ")"
                    End If
                Catch exFormula As Exception
                    ' If formula application fails, log it
                    Console.WriteLine("Crystal Formula Error: " & exFormula.Message)
                End Try
            End If

            ' Hand over to SaleInv for centralized display/printing
            SaleInv.ShowReport(rptDoc, 7)

        Catch ex As Exception
            MessageBox.Show("Report Error: " & ex.Message, "Print Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Helper method to clear the form
    Private Sub ClearFields()
        Chq_NoTextBox.Clear()
        C_NameTextBox.Clear()
        BankTextBox.Clear()
        AmountTextBox.Clear()
        selectedBankID = 0
        selectedInvoiceNo = ""
        ' ComboBox3.SelectedIndex = -1  ' User requested to keep user selection
        ' secure_key.Clear()           ' User requested to keep secure key
        ' Initially hide action buttons when clearing the form
        btn_paid.Visible = False
        btn_return.Visible = False

        ' Reset record state and enable save
        isExistingRecord = False
        btn_save.Enabled = True
        btn_save.Text = "Save"

        Chq_NoTextBox.Select()
    End Sub
    Private Sub BankTextBox_TextChanged(sender As Object, e As EventArgs) Handles BankTextBox.TextChanged
        If isSelecting Then Return
        activeSearchControl = BankTextBox
        currentMode = SearchMode.Bank

        If String.IsNullOrEmpty(BankTextBox.Text) Then
            Panel1.Visible = False
            Return
        End If

        Try
            If MySqlConn.State <> ConnectionState.Open Then MySqlConn.Open()
            Dim table As New DataTable

            ' Include id column for selection
            Dim adapter As New MySqlDataAdapter("SELECT id, bank_name, amount FROM bank", MySqlConn)
            adapter.Fill(table)

            Dim dv As New DataView(table)
            dv.RowFilter = String.Format("bank_name Like '%{0}%'", BankTextBox.Text)

            CustomerDataGridView.DataSource = dv

            ' Hide the ID column but keep it in the data
            CustomerDataGridView.Columns(0).Visible = False
            CustomerDataGridView.Columns(1).HeaderCell.Value = "Bank Name"
            CustomerDataGridView.Columns(1).Width = 450
            CustomerDataGridView.Columns(2).HeaderCell.Value = "Balance"
            CustomerDataGridView.Columns(2).Width = 300

            PositionSuggestionPanel()
            Panel1.Visible = True

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        Finally
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Sub

    Private Sub btn_delete_Click(sender As Object, e As EventArgs) Handles btn_delete.Click
        ' Check if a row is selected
        If ChaquereceivedDataGridView.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a cheque record to delete from the grid.")
            Return
        End If
        If Not IsSecureKeyValid() Then Exit Sub

        ' Get the selected row and check number
        Dim selectedRow As DataGridViewRow = ChaquereceivedDataGridView.SelectedRows(0)
        Dim checkNumber As String = If(selectedRow.Cells(0).Value IsNot Nothing, selectedRow.Cells(0).Value.ToString().Trim(), "")

        If String.IsNullOrEmpty(checkNumber) Then
            MessageBox.Show("Selected row has no cheque number.")
            Return
        End If

        ' Confirmation check
        Dim result As DialogResult = MessageBox.Show("Are you sure you want to delete cheque #" & checkNumber & "? All financial updates will be reversed.", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

        If result = DialogResult.Yes Then
            Dim transaction As MySqlTransaction = Nothing
            Try
                If MySqlConn.State <> ConnectionState.Open Then MySqlConn.Open()

                ' 1. Fetch current details from check_received for accuracy
                Dim status As String = ""
                Dim globalAmount As Double = 0
                Dim globalInvNo As String = ""
                Dim customerName As String = ""
                Dim globalBankID As Integer = Convert.ToInt32(selectedRow.Cells(3).Value)
                Dim selInvNo As String = If(selectedRow.Cells(9).Value IsNot Nothing, selectedRow.Cells(9).Value.ToString().Trim(), "")

                Dim fetchQuery As String = "SELECT status, amount, inv_no, check_name FROM check_received " &
                                         "WHERE LOWER(check_number) = LOWER(@chq) AND bank_id = @bid AND IFNULL(inv_no, '') = IFNULL(@inv, '') LIMIT 1"
                Using cmdFetch As New MySqlCommand(fetchQuery, MySqlConn)
                    cmdFetch.Parameters.AddWithValue("@chq", checkNumber)
                    cmdFetch.Parameters.AddWithValue("@bid", globalBankID)
                    cmdFetch.Parameters.AddWithValue("@inv", selInvNo)
                    Using reader = cmdFetch.ExecuteReader()
                        If reader.Read() Then
                            status = If(IsDBNull(reader("status")), "PENDING", reader("status").ToString().Trim().ToUpper())
                            globalAmount = If(IsDBNull(reader("amount")), 0, Convert.ToDouble(reader("amount")))
                            globalInvNo = If(IsDBNull(reader("inv_no")), "", reader("inv_no").ToString().Trim())
                            customerName = If(IsDBNull(reader("check_name")), "", reader("check_name").ToString().Trim())
                        Else
                            Throw New Exception("Cheque record not found in database.")
                        End If
                    End Using
                End Using

                transaction = MySqlConn.BeginTransaction()

                ' 2. Reversal logic (Deductions are reversed upon deletion)
                ' A. Process through customer_payments ledger (always reversed regardless of status since they are deducted immediately on receipt)
                Dim paymentsTable As New DataTable()
                Dim findPaymentsRSql As String = "SELECT inv_no, Amount, CusID FROM customer_payments " &
                                               "WHERE LOWER(TRIM(cheque_no)) = LOWER(TRIM(@chq)) AND bank_id = @bid AND FIND_IN_SET(inv_no, REPLACE(@inv, ', ', ','))"
                Using cmdFindPay As New MySqlCommand(findPaymentsRSql, MySqlConn, transaction)
                    cmdFindPay.Parameters.AddWithValue("@chq", checkNumber)
                    cmdFindPay.Parameters.AddWithValue("@bid", globalBankID)
                    cmdFindPay.Parameters.AddWithValue("@inv", selInvNo)
                    Using adapter As New MySqlDataAdapter(cmdFindPay)
                        adapter.Fill(paymentsTable)
                    End Using
                End Using

                Dim reversedInBilling As Boolean = False
                If paymentsTable.Rows.Count > 0 Then
                    For Each row As DataRow In paymentsTable.Rows
                        Dim pInvNo As String = If(IsDBNull(row("inv_no")), "", row("inv_no").ToString().Trim())
                        Dim pAmount As Double = Convert.ToDouble(row("Amount"))
                        Dim pCusID As Integer = Convert.ToInt32(row("CusID"))

                        If Not String.IsNullOrEmpty(pInvNo) Then
                            ' i. Restore Billing balances and status
                            ' Since it's in customer_payments, it was a "Payment" type cheque targeting credit_balance_due
                            Dim upBillSql As String = "UPDATE billing SET " &
                                                     "balance_due = balance_due + @amt, " &
                                                     "credit_balance_due = credit_balance_due + @amt, " &
                                                     "paid_amount = paid_amount - @amt " &
                                                     "WHERE LOWER(TRIM(inv_no)) = LOWER(TRIM(@inv)) AND customer_id = @cid"
                            Using cmdUpBill As New MySqlCommand(upBillSql, MySqlConn, transaction)
                                cmdUpBill.Parameters.AddWithValue("@amt", pAmount)
                                cmdUpBill.Parameters.AddWithValue("@inv", pInvNo)
                                cmdUpBill.Parameters.AddWithValue("@cid", pCusID)
                                cmdUpBill.ExecuteNonQuery()
                            End Using

                            ' ii. Restore Customer Credit
                            Using cmdUpCre As New MySqlCommand("UPDATE customer_credit SET amount = amount + @amt WHERE customer_id = @cid AND LOWER(TRIM(inv_no)) = LOWER(TRIM(@inv)) AND is_active = 1", MySqlConn, transaction)
                                cmdUpCre.Parameters.AddWithValue("@amt", pAmount)
                                cmdUpCre.Parameters.AddWithValue("@cid", pCusID)
                                cmdUpCre.Parameters.AddWithValue("@inv", pInvNo)
                                cmdUpCre.ExecuteNonQuery()
                            End Using
                            reversedInBilling = True
                        End If
                    Next
                End If

                ' B. Fallback: If no payments record found (Likely a "Billing" cheque from Sale Invoice) - only reverse if it was cleared
                If Not reversedInBilling AndAlso (status = "CLEARED" Or status = "PAID" Or status = "REALISED" Or status = "SUCCESS") Then
                    Dim billTable As New DataTable()
                    Using cmdFindBill As New MySqlCommand("SELECT id, inv_no, balance_due, paid_amount, customer_id FROM billing " &
                                                        "WHERE LOWER(TRIM(cheque_no)) = LOWER(TRIM(@chq)) AND bank_id = @bid AND FIND_IN_SET(inv_no, REPLACE(@inv, ', ', ','))", MySqlConn, transaction)
                        cmdFindBill.Parameters.AddWithValue("@chq", checkNumber)
                        cmdFindBill.Parameters.AddWithValue("@bid", globalBankID)
                        cmdFindBill.Parameters.AddWithValue("@inv", selInvNo)
                        Using adapter As New MySqlDataAdapter(cmdFindBill)
                            adapter.Fill(billTable)
                        End Using
                    End Using

                    For Each row As DataRow In billTable.Rows
                        Dim bId As Integer = Convert.ToInt32(row("id"))
                        Dim bPaid As Double = Convert.ToDouble(row("paid_amount"))

                        ' Restore cheque_balance_due for "Billing" cheques
                        Dim upBillSql As String = "UPDATE billing SET " &
                                                 "balance_due = balance_due + @amt, " &
                                                 "cheque_balance_due = cheque_balance_due + @amt, " &
                                                 "paid_amount = paid_amount - @amt " &
                                                 "WHERE id = @id"
                        Using cmdUpBill As New MySqlCommand(upBillSql, MySqlConn, transaction)
                            cmdUpBill.Parameters.AddWithValue("@amt", Math.Min(globalAmount, bPaid))
                            cmdUpBill.Parameters.AddWithValue("@id", bId)
                            cmdUpBill.ExecuteNonQuery()
                        End Using
                    Next
                End If

                ' 2.1 Final status recalculation for ALL affected invoices (ensures Mixed/Cash/etc are correct)
                ' We'll check all unique invoices that were associated with this cheque
                Dim affectedItems As New List(Of (inv As String, cid As Integer))
                
                ' Get from customer_payments
                Using cmdGetInv As New MySqlCommand("SELECT inv_no, CusID FROM customer_payments WHERE LOWER(TRIM(cheque_no)) = LOWER(TRIM(@chq)) AND bank_id = @bid AND FIND_IN_SET(inv_no, REPLACE(@inv, ', ', ','))", MySqlConn, transaction)
                    cmdGetInv.Parameters.AddWithValue("@chq", checkNumber)
                    cmdGetInv.Parameters.AddWithValue("@bid", globalBankID)
                    cmdGetInv.Parameters.AddWithValue("@inv", selInvNo)
                    Using reader = cmdGetInv.ExecuteReader()
                        While reader.Read()
                            Dim i = If(IsDBNull(reader(0)), "", reader(0).ToString())
                            Dim c = Convert.ToInt32(reader(1))
                            If Not String.IsNullOrEmpty(i) Then affectedItems.Add((i, c))
                        End While
                    End Using
                End Using
                ' Get from billing
                Using cmdGetInvB As New MySqlCommand("SELECT inv_no, customer_id FROM billing WHERE LOWER(TRIM(cheque_no)) = LOWER(TRIM(@chq)) AND bank_id = @bid AND FIND_IN_SET(inv_no, REPLACE(@inv, ', ', ','))", MySqlConn, transaction)
                    cmdGetInvB.Parameters.AddWithValue("@chq", checkNumber)
                    cmdGetInvB.Parameters.AddWithValue("@bid", globalBankID)
                    cmdGetInvB.Parameters.AddWithValue("@inv", selInvNo)
                    Using reader = cmdGetInvB.ExecuteReader()
                        While reader.Read()
                            Dim i = If(IsDBNull(reader(0)), "", reader(0).ToString())
                            Dim c = Convert.ToInt32(reader(1))
                            If Not String.IsNullOrEmpty(i) Then affectedItems.Add((i, c))
                        End While
                    End Using
                End Using

                For Each item In affectedItems.Distinct()
                    Dim recalculateStatSql As String = "UPDATE billing SET " &
                                                     "status = IF(balance_due <= 0.00, 'success', " &
                                                     "  CASE " &
                                                     "    WHEN partial_cash > 0 AND cheque_balance_due > 0 AND credit_balance_due > 0 THEN 'Mixed_Payment' " &
                                                     "    WHEN partial_cash > 0 AND cheque_balance_due = 0 AND credit_balance_due > 0 THEN 'Cash_Credit' " &
                                                     "    WHEN partial_cash > 0 AND cheque_balance_due > 0 AND credit_balance_due = 0 THEN 'Cash_Cheque' " &
                                                     "    WHEN partial_cash > 0 AND cheque_balance_due = 0 AND credit_balance_due = 0 THEN 'Cash' " &
                                                     "    WHEN partial_cash = 0 AND cheque_balance_due > 0 AND credit_balance_due > 0 THEN 'Credit_Cheque' " &
                                                     "    WHEN partial_cash = 0 AND cheque_balance_due = 0 AND credit_balance_due > 0 THEN 'Credit' " &
                                                     "    WHEN partial_cash = 0 AND cheque_balance_due > 0 AND credit_balance_due = 0 THEN 'Cheque' " &
                                                     "    ELSE status " &
                                                     "  END) " &
                                                     "WHERE LOWER(TRIM(inv_no)) = LOWER(TRIM(@inv)) AND customer_id = @cid"
                    Using cmdRecalculate As New MySqlCommand(recalculateStatSql, MySqlConn, transaction)
                        cmdRecalculate.Parameters.AddWithValue("@inv", item.inv)
                        cmdRecalculate.Parameters.AddWithValue("@cid", item.cid)
                        cmdRecalculate.ExecuteNonQuery()
                    End Using
                Next

                ' 2.2 Delete from customer_payments
                ' ONLY done if we are actually deleting the cheque (Status is not Cleared/Paid/etc)
                ' If it was Cleared, we keep the payments so it can be re-cleared later
                If Not (status = "CLEARED" Or status = "PAID" Or status = "REALISED" Or status = "SUCCESS") Then
                    Dim delPaySql As String = "DELETE FROM customer_payments WHERE LOWER(TRIM(cheque_no)) = LOWER(TRIM(@chq)) AND bank_id = @bid AND FIND_IN_SET(inv_no, REPLACE(@inv, ', ', ','))"
                    Using cmdDelPay As New MySqlCommand(delPaySql, MySqlConn, transaction)
                        cmdDelPay.Parameters.AddWithValue("@chq", checkNumber)
                        cmdDelPay.Parameters.AddWithValue("@bid", globalBankID)
                        cmdDelPay.Parameters.AddWithValue("@inv", selInvNo)
                        cmdDelPay.ExecuteNonQuery()
                    End Using
                End If

                ' 3. Perform actual action on check_received record
                Dim isDeleted As Boolean = False
                If status = "CLEARED" Or status = "PAID" Or status = "REALISED" Or status = "SUCCESS" Then
                    ' --- UNDO CLEAR: Restore to Pending instead of Deleting ---
                    Dim updateCheckSql As String = "UPDATE check_received SET status = 'Pending' WHERE LOWER(check_number) = LOWER(@chq) AND bank_id = @bid AND IFNULL(inv_no, '') = IFNULL(@inv, '')"
                    Using cmdUpCheck As New MySqlCommand(updateCheckSql, MySqlConn, transaction)
                        cmdUpCheck.Parameters.AddWithValue("@chq", checkNumber)
                        cmdUpCheck.Parameters.AddWithValue("@bid", globalBankID)
                        cmdUpCheck.Parameters.AddWithValue("@inv", selInvNo)
                        cmdUpCheck.ExecuteNonQuery()
                    End Using
                Else
                    ' --- FULL DELETE: For Pending or Returned cheques ---
                    Dim delCheckSql As String = "DELETE FROM check_received WHERE LOWER(check_number) = LOWER(@chq) AND bank_id = @bid AND IFNULL(inv_no, '') = IFNULL(@inv, '')"
                    Using cmdDelCheck As New MySqlCommand(delCheckSql, MySqlConn, transaction)
                        cmdDelCheck.Parameters.AddWithValue("@chq", checkNumber)
                        cmdDelCheck.Parameters.AddWithValue("@bid", globalBankID)
                        cmdDelCheck.Parameters.AddWithValue("@inv", selInvNo)
                        cmdDelCheck.ExecuteNonQuery()
                    End Using
                    isDeleted = True
                End If


                ' 4. Clean up from cheque_returned
                Dim delRetSql As String = "DELETE FROM cheque_returned WHERE LOWER(check_number) = LOWER(@chq) AND bank_id = @bid AND IFNULL(inv_no, '') = IFNULL(@inv, '')"
                Using cmdDelRet As New MySqlCommand(delRetSql, MySqlConn, transaction)
                    cmdDelRet.Parameters.AddWithValue("@chq", checkNumber)
                    cmdDelRet.Parameters.AddWithValue("@bid", globalBankID)
                    cmdDelRet.Parameters.AddWithValue("@inv", selInvNo)
                    cmdDelRet.ExecuteNonQuery()
                End Using

                transaction.Commit()
                If isDeleted Then
                    Module1.LogDeletion("Customer Cheque", checkNumber, "Bank ID: " & globalBankID & ", Amount: " & globalAmount & ", Inv No: " & selInvNo & ", Customer: " & customerName & ", Status: " & status)
                End If
                MessageBox.Show("Cheque and associated financial updates reversed successfully.", "Success")

                PerformSearch() ' Refresh grid
                ClearFields() ' Clear textboxes

            Catch ex As Exception
                If transaction IsNot Nothing Then transaction.Rollback()
                MessageBox.Show("Error during deletion: " & ex.Message)
            Finally
                If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
            End Try
        End If
    End Sub

    Private Sub btn_paid_Click(sender As Object, e As EventArgs) Handles btn_paid.Click
        ' Check if a row is selected
        If ChaquereceivedDataGridView.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a cheque record from the grid to mark as paid.")
            Return
        End If

        ' Get the selected row
        Dim selectedRow As DataGridViewRow = ChaquereceivedDataGridView.SelectedRows(0)
        Dim checkNumber As String = selectedRow.Cells(0).Value.ToString()
        Dim currentStatus As String = selectedRow.Cells(5).Value.ToString().ToUpper()

        ' If cheque is already Returned, do not allow update
        If currentStatus = "RETURN" Or currentStatus = "RETURNED" Then
            MessageBox.Show("This cheque is Returned. You cannot mark it as Paid.")
            Return
        End If

        ' If already Paid/Realised, inform user
        If currentStatus = "PAID" Or currentStatus = "REALISED" Or currentStatus = "CLEARED" Then
            MessageBox.Show("This cheque is already marked as Paid/Realised.")
            Return
        End If

        ' Ask confirmation
        Dim result As DialogResult = MessageBox.Show(
            "Are you sure you want to mark cheque #" & checkNumber & " as Paid? This will reduce the customer balance.",
            "Confirm Payment",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question)

        If result = DialogResult.Yes Then
            Dim transaction As MySqlTransaction = Nothing
            Try
                If MySqlConn.State <> ConnectionState.Open Then MySqlConn.Open()
                transaction = MySqlConn.BeginTransaction()

                ' 1. Get payment details (including inv_no) directly from check_received first
                ' This is the most reliable way since we started storing inv_no there
                Dim cusID As Integer = 0
                Dim invNo As String = ""
                Dim amount As Double = 0
                Dim paymentFound As Boolean = False
                Dim bid As Integer = Convert.ToInt32(selectedRow.Cells(3).Value)
                Dim selInvNo As String = If(selectedRow.Cells(9).Value IsNot Nothing, selectedRow.Cells(9).Value.ToString().Trim(), "")

                Dim findInCheckSql As String = "SELECT inv_no, amount FROM check_received WHERE check_number = @chq AND bank_id = @bid AND IFNULL(inv_no, '') = IFNULL(@inv, '') LIMIT 1"
                Using cmdCheck As New MySqlCommand(findInCheckSql, MySqlConn, transaction)
                    cmdCheck.Parameters.AddWithValue("@chq", checkNumber)
                    cmdCheck.Parameters.AddWithValue("@bid", bid)
                    cmdCheck.Parameters.AddWithValue("@inv", selInvNo)
                    Using reader As MySqlDataReader = cmdCheck.ExecuteReader()
                        If reader.Read() Then
                            invNo = If(IsDBNull(reader("inv_no")), "", reader("inv_no").ToString())
                            amount = Convert.ToDouble(reader("amount"))
                        End If
                    End Using
                End Using

                Dim isPaymentCheque As Boolean = False
                Dim findPaySql As String = "SELECT CusID, inv_no, Amount FROM customer_payments WHERE LOWER(TRIM(cheque_no)) = LOWER(TRIM(@chq)) AND bank_id = @bid AND FIND_IN_SET(inv_no, REPLACE(@inv, ', ', ',')) LIMIT 1"
                Using cmdFind As New MySqlCommand(findPaySql, MySqlConn, transaction)
                    cmdFind.Parameters.AddWithValue("@chq", checkNumber)
                    cmdFind.Parameters.AddWithValue("@bid", bid)
                    cmdFind.Parameters.AddWithValue("@inv", selInvNo)
                    Using reader As MySqlDataReader = cmdFind.ExecuteReader()
                        If reader.Read() Then
                            cusID = Convert.ToInt32(reader("CusID"))
                            If String.IsNullOrEmpty(invNo) Then
                                invNo = If(IsDBNull(reader("inv_no")), "", reader("inv_no").ToString().Trim())
                            End If
                            ' Use payment amount as fallback if check_received amount is 0
                            If amount = 0 Then amount = Convert.ToDouble(reader("Amount"))
                            isPaymentCheque = True
                        End If
                    End Using
                End Using

                ' If not found in customer_payments, try to find customer ID from billing
                If cusID = 0 Then
                    Dim findBillSql As String = "SELECT customer_id, inv_no FROM billing WHERE LOWER(TRIM(cheque_no)) = LOWER(TRIM(@chq)) AND bank_id = @bid AND FIND_IN_SET(inv_no, REPLACE(@inv, ', ', ',')) LIMIT 1"
                    Using cmdBillFind As New MySqlCommand(findBillSql, MySqlConn, transaction)
                        cmdBillFind.Parameters.AddWithValue("@chq", checkNumber)
                        cmdBillFind.Parameters.AddWithValue("@bid", bid)
                        cmdBillFind.Parameters.AddWithValue("@inv", selInvNo)
                        Using reader As MySqlDataReader = cmdBillFind.ExecuteReader()
                            If reader.Read() Then
                                cusID = Convert.ToInt32(reader("customer_id"))
                                If String.IsNullOrEmpty(invNo) Then
                                    invNo = If(IsDBNull(reader("inv_no")), "", reader("inv_no").ToString().Trim())
                                End If
                                If amount = 0 Then amount = Convert.ToDouble(selectedRow.Cells(4).Value)
                            End If
                        End Using
                    End Using
                End If

                ' If still not found, try to find customer ID by name from the cheque
                If cusID = 0 Then
                    Dim customerName As String = selectedRow.Cells(1).Value.ToString().Trim()
                    Dim findCusSql As String = "SELECT id FROM customer WHERE LOWER(TRIM(name)) = @name LIMIT 1"
                    Using cmdCus As New MySqlCommand(findCusSql, MySqlConn, transaction)
                        cmdCus.Parameters.AddWithValue("@name", customerName.ToLower())
                        Dim res = cmdCus.ExecuteScalar()
                        If res IsNot Nothing AndAlso Not IsDBNull(res) Then
                            cusID = Convert.ToInt32(res)
                            If amount = 0 Then amount = Convert.ToDouble(selectedRow.Cells(4).Value)
                        End If
                    End Using
                End If

                If cusID = 0 Then
                    Throw New Exception("Could not find the customer associated with this cheque. Please ensure the customer exists.")
                End If

                Dim updateDate As String = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")

                ' --- SYNCHRONIZED BILLING AND CREDIT UPDATES ---
                Dim checkAmount As Double = amount
                Dim processedInPayments As Boolean = False

                ' A. Process through customer_payments ledger (Primary source for "Payment" cheques)
                Dim paymentsTable As New DataTable()
                Dim findPaymentsSql As String = "SELECT inv_no, Amount, CusID FROM customer_payments " &
                                              "WHERE LOWER(TRIM(cheque_no)) = LOWER(TRIM(@chq)) AND bank_id = @bid AND FIND_IN_SET(inv_no, REPLACE(@inv, ', ', ','))"
                Using cmdFindPay As New MySqlCommand(findPaymentsSql, MySqlConn, transaction)
                    cmdFindPay.Parameters.AddWithValue("@chq", checkNumber)
                    cmdFindPay.Parameters.AddWithValue("@bid", bid)
                    cmdFindPay.Parameters.AddWithValue("@inv", selInvNo)
                    Using adapter As New MySqlDataAdapter(cmdFindPay)
                        adapter.Fill(paymentsTable)
                    End Using
                End Using

                If paymentsTable.Rows.Count > 0 Then
                    ' Bypassed balance deduction because payment cheques are now immediately deducted on receipt
                    processedInPayments = True
                End If

                ' B. Fallback: If not in payments ledger (Meaning it's a "Billing" cheque from Sale Invoice)
                If Not processedInPayments Then
                    Dim billingItems As New DataTable()
                    If Not String.IsNullOrEmpty(invNo) Then
                        Using cmdBill As New MySqlCommand("SELECT id, inv_no, balance_due, cheque_balance_due, credit_balance_due, paid_amount, status, partial_cash FROM billing " &
                                                       "WHERE FIND_IN_SET(inv_no, REPLACE(@inv, ', ', ',')) AND customer_id = @id AND balance_due > 0", MySqlConn, transaction)
                            cmdBill.Parameters.AddWithValue("@inv", invNo.ToLower().Trim())
                            cmdBill.Parameters.AddWithValue("@id", cusID)
                            Using adapter As New MySqlDataAdapter(cmdBill)
                                adapter.Fill(billingItems)
                            End Using
                        End Using
                    Else
                        Using cmdBill As New MySqlCommand("SELECT id, inv_no, balance_due, cheque_balance_due, credit_balance_due, paid_amount, status, partial_cash FROM billing " &
                                                       "WHERE customer_id = @id AND balance_due > 0 ORDER BY timestamps ASC", MySqlConn, transaction)
                            cmdBill.Parameters.AddWithValue("@id", cusID)
                            Using adapter As New MySqlDataAdapter(cmdBill)
                                adapter.Fill(billingItems)
                            End Using
                        End Using
                    End If

                    Dim remainingPayment As Double = checkAmount
                    For Each row As DataRow In billingItems.Rows
                        If remainingPayment <= 0 Then Exit For
                        Dim billId As Integer = Convert.ToInt32(row("id"))
                        Dim bInvNo As String = row("inv_no").ToString()
                        Dim balDue As Double = Math.Round(Convert.ToDouble(row("balance_due")), 2)
                        Dim chqBalDue As Double = Math.Round(If(IsDBNull(row("cheque_balance_due")), 0, Convert.ToDouble(row("cheque_balance_due"))), 2)
                        Dim crdBalDue As Double = Math.Round(If(IsDBNull(row("credit_balance_due")), 0, Convert.ToDouble(row("credit_balance_due"))), 2)
                        Dim paidAmt As Double = Math.Round(Convert.ToDouble(row("paid_amount")), 2)

                        ' Since this is NOT a "Payment" cheque (it's a Billing cheque), prioritize Cheque balance reduction
                        Dim appliedToChq As Double = Math.Round(Math.Min(remainingPayment, chqBalDue), 2)
                        Dim appliedToCrd As Double = Math.Round(Math.Min(remainingPayment - appliedToChq, crdBalDue), 2)
                        Dim totalApplied As Double = appliedToChq + appliedToCrd

                        ' 1. Clear Credit if any
                        If appliedToCrd > 0 Then
                            Dim remToCrd As Double = appliedToCrd
                            Using cmdCre As New MySqlCommand("SELECT id, amount FROM customer_credit WHERE customer_id = @cid AND inv_no = @inv AND is_active = 1 AND amount > 0 ORDER BY timestamps ASC", MySqlConn, transaction)
                                cmdCre.Parameters.AddWithValue("@cid", cusID)
                                cmdCre.Parameters.AddWithValue("@inv", bInvNo)
                                Using adapter As New MySqlDataAdapter(cmdCre)
                                    Dim crs As New DataTable()
                                    adapter.Fill(crs)
                                    For Each cr In crs.Rows
                                        If remToCrd <= 0 Then Exit For
                                        Dim subA As Double = Math.Round(Math.Min(remToCrd, Convert.ToDouble(cr("amount"))), 2)
                                        Using upC As New MySqlCommand("UPDATE customer_credit SET amount = amount - @a WHERE id = @id", MySqlConn, transaction)
                                            upC.Parameters.AddWithValue("@a", subA)
                                            upC.Parameters.AddWithValue("@id", Convert.ToInt32(cr("id")))
                                            upC.ExecuteNonQuery()
                                        End Using
                                        remToCrd -= subA
                                    Next
                                End Using
                            End Using
                        End If

                        ' 2. Update Billing
                        Dim nBal As Double = Math.Max(0, Math.Round(balDue - totalApplied, 2))
                        Dim nChq As Double = Math.Max(0, Math.Round(chqBalDue - appliedToChq, 2))
                        Dim nCrd As Double = Math.Max(0, Math.Round(crdBalDue - appliedToCrd, 2))
                        Dim nPaid As Double = Math.Round(paidAmt + totalApplied, 2)

                        Dim upB As String = "UPDATE billing SET balance_due=@b, cheque_balance_due=@cq, credit_balance_due=@cr, paid_amount=@p, " &
                                          "status = IF(@b <= 0.00, 'success', CASE WHEN partial_cash>0 AND @cq>0 AND @cr>0 THEN 'Mixed_Payment' WHEN partial_cash>0 AND @cq=0 AND @cr>0 THEN 'Cash_Credit' WHEN partial_cash>0 AND @cq>0 AND @cr=0 THEN 'Cash_Cheque' WHEN partial_cash=0 AND @cq>0 AND @cr>0 THEN 'Credit_Cheque' WHEN partial_cash=0 AND @cq=0 AND @cr>0 THEN 'Credit' WHEN partial_cash=0 AND @cq>0 AND @cr=0 THEN 'Cheque' ELSE status END) " &
                                          "WHERE id=@id"
                        Using cmdUpB As New MySqlCommand(upB, MySqlConn, transaction)
                            cmdUpB.Parameters.AddWithValue("@b", nBal)
                            cmdUpB.Parameters.AddWithValue("@cq", nChq)
                            cmdUpB.Parameters.AddWithValue("@cr", nCrd)
                            cmdUpB.Parameters.AddWithValue("@p", nPaid)
                            cmdUpB.Parameters.AddWithValue("@id", billId)
                            cmdUpB.ExecuteNonQuery()
                        End Using
                        remainingPayment -= totalApplied
                    Next
                End If


                ' 4. Update status to 'Cleared' in check_received
                Dim updateStatusSql As String = "UPDATE check_received SET status = @status WHERE check_number = @chq AND bank_id = @bid AND IFNULL(inv_no, '') = IFNULL(@inv, '')"
                Using cmdStat As New MySqlCommand(updateStatusSql, MySqlConn, transaction)
                    cmdStat.Parameters.AddWithValue("@status", "Cleared")
                    cmdStat.Parameters.AddWithValue("@chq", checkNumber)
                    cmdStat.Parameters.AddWithValue("@bid", bid)
                    cmdStat.Parameters.AddWithValue("@inv", selInvNo)
                    cmdStat.ExecuteNonQuery()
                End Using

                transaction.Commit()
                MessageBox.Show("Cheque cleared and balances updated successfully.", "Success")
                PerformSearch() ' Refresh grid (maintains filters)
                ClearFields()
            Catch ex As Exception
                If transaction IsNot Nothing Then transaction.Rollback()
                MessageBox.Show("Error clearing cheque: " & ex.Message)
            Finally
                If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
            End Try
        End If
    End Sub


    Private Sub ChaquereceivedDataGridView_KeyDown(sender As Object, e As KeyEventArgs) Handles ChaquereceivedDataGridView.KeyDown
        If e.KeyCode = Keys.Enter Then
            If ChaquereceivedDataGridView.CurrentRow IsNot Nothing Then
                ProcessGridSelection(ChaquereceivedDataGridView.CurrentRow.Index)
                e.Handled = True
                e.SuppressKeyPress = True
            End If
        End If
    End Sub


    ' Mark as Returned button
    Private Sub btn_return_Click(sender As Object, e As EventArgs) Handles btn_return.Click
        ' Check if a row is selected
        If ChaquereceivedDataGridView.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a cheque record from the grid to mark as returned.")
            Return
        End If

        ' Get the selected row
        Dim selectedRow As DataGridViewRow = ChaquereceivedDataGridView.SelectedRows(0)
        Dim currentStatus As String = selectedRow.Cells(5).Value.ToString().ToUpper()

        ' If already Returned, inform user
        If currentStatus = "RETURN" Or currentStatus = "RETURNED" Then
            MessageBox.Show("This cheque is already marked as Returned.")
            Return
        End If

        ' Show return reason panel
        ReturnReasonTextBox.Clear()
        ReturnReasonPanel.Location = New Point(
            (Me.Width - ReturnReasonPanel.Width) / 2,
            (Me.Height - ReturnReasonPanel.Height) / 2
        )
        ReturnReasonPanel.BringToFront()
        ReturnReasonPanel.Visible = True
        ReturnReasonTextBox.Focus()
    End Sub

    Private Sub CancelReturnButton_Click(sender As Object, e As EventArgs) Handles CancelReturnButton.Click
        ReturnReasonPanel.Visible = False
    End Sub

    Private Sub SubmitReturnButton_Click(sender As Object, e As EventArgs) Handles SubmitReturnButton.Click
        If String.IsNullOrWhiteSpace(ReturnReasonTextBox.Text) Then
            MessageBox.Show("Please enter a reason for the return.")
            ReturnReasonTextBox.Focus()
            Return
        End If

        If ChaquereceivedDataGridView.SelectedRows.Count = 0 Then Return

        Dim selectedRow As DataGridViewRow = ChaquereceivedDataGridView.SelectedRows(0)

        ' Robustly get values with null/DBNull checks
        Dim checkNumber As String = If(selectedRow.Cells(0).Value IsNot Nothing, selectedRow.Cells(0).Value.ToString(), "")
        Dim checkName As String = If(selectedRow.Cells(1).Value IsNot Nothing, selectedRow.Cells(1).Value.ToString(), "")
        Dim bankId As Integer = If(IsDBNull(selectedRow.Cells(3).Value) OrElse selectedRow.Cells(3).Value Is Nothing, 0, Convert.ToInt32(selectedRow.Cells(3).Value))
        Dim amount As Decimal = If(IsDBNull(selectedRow.Cells(4).Value) OrElse selectedRow.Cells(4).Value Is Nothing, 0, Convert.ToDecimal(selectedRow.Cells(4).Value))

        ' Corrected indexes: 7 for issue_date, 8 for check_release_date (index 6 is return_reason which is often DBNull)
        Dim issueDate As String = "2000-01-01"
        If Not IsDBNull(selectedRow.Cells(7).Value) AndAlso selectedRow.Cells(7).Value IsNot Nothing Then
            issueDate = Convert.ToDateTime(selectedRow.Cells(7).Value).ToString("yyyy-MM-dd")
        End If

        Dim releaseDate As String = "2000-01-01"
        If Not IsDBNull(selectedRow.Cells(8).Value) AndAlso selectedRow.Cells(8).Value IsNot Nothing Then
            releaseDate = Convert.ToDateTime(selectedRow.Cells(8).Value).ToString("yyyy-MM-dd")
        End If

        ' We need inv_no which is in the database but might not be in the grid if not explicitly loaded
        ' However, btn_paid_Click logic shows how to find it. 
        ' Let's fetch it from check_received to be sure.

        Try
            If MySqlConn.State <> ConnectionState.Open Then MySqlConn.Open()

            ' invNo is obtained above from selectedRow.Cells(9)
            Dim invNo As String = If(selectedRow.Cells(9).Value IsNot Nothing, selectedRow.Cells(9).Value.ToString(), "")
            
            ' If still empty, fetch it as fallback (though it shouldn't be empty now)
            If String.IsNullOrEmpty(invNo) Then
                Dim getInvSql As String = "SELECT inv_no FROM check_received WHERE check_number = @chq AND bank_id = @bid LIMIT 1"
                Using cmdInv As New MySqlCommand(getInvSql, MySqlConn)
                    cmdInv.Parameters.AddWithValue("@chq", checkNumber)
                    cmdInv.Parameters.AddWithValue("@bid", bankId)
                    Dim res = cmdInv.ExecuteScalar()
                    If res IsNot Nothing Then invNo = res.ToString()
                End Using
            End If

            ' Start Transaction
            Dim transaction As MySqlTransaction = MySqlConn.BeginTransaction()
            Try
                ' 1. Insert into cheque_returned
                Dim insertReturnedSql As String = "INSERT INTO cheque_returned (check_number, check_name, bank_id, amount, return_reason, issue_date, check_release_date, inv_no) " &
                                                 "VALUES (@chq, @name, @bank, @amt, @reason, @issue, @release, @inv)"
                Using cmdIns As New MySqlCommand(insertReturnedSql, MySqlConn, transaction)
                    cmdIns.Parameters.AddWithValue("@chq", checkNumber)
                    cmdIns.Parameters.AddWithValue("@name", checkName)
                    cmdIns.Parameters.AddWithValue("@bank", bankId)
                    cmdIns.Parameters.AddWithValue("@amt", amount)
                    cmdIns.Parameters.AddWithValue("@reason", ReturnReasonTextBox.Text.Trim())
                    cmdIns.Parameters.AddWithValue("@issue", issueDate)
                    cmdIns.Parameters.AddWithValue("@release", releaseDate)
                    cmdIns.Parameters.AddWithValue("@inv", invNo)
                    cmdIns.ExecuteNonQuery()
                End Using

                ' 2. Update status in check_received
                Dim updateCheckSql As String = "UPDATE check_received SET status = 'Returned' WHERE check_number = @chq AND bank_id = @bid AND IFNULL(inv_no, '') = IFNULL(@inv, '')"
                Using cmdUpCheck As New MySqlCommand(updateCheckSql, MySqlConn, transaction)
                    cmdUpCheck.Parameters.AddWithValue("@chq", checkNumber)
                    cmdUpCheck.Parameters.AddWithValue("@bid", bankId)
                    cmdUpCheck.Parameters.AddWithValue("@inv", invNo)
                    cmdUpCheck.ExecuteNonQuery()
                End Using

                ' 3. Reversal logic for customer_payments cheques (Deduction is restored because the cheque returned/bounced)
                Dim paymentsTable As New DataTable()
                Dim findPaymentsRSql As String = "SELECT inv_no, Amount, CusID FROM customer_payments " &
                                               "WHERE LOWER(TRIM(cheque_no)) = LOWER(TRIM(@chq)) AND bank_id = @bid AND FIND_IN_SET(inv_no, REPLACE(@inv, ', ', ','))"
                Using cmdFindPay As New MySqlCommand(findPaymentsRSql, MySqlConn, transaction)
                    cmdFindPay.Parameters.AddWithValue("@chq", checkNumber)
                    cmdFindPay.Parameters.AddWithValue("@bid", bankId)
                    cmdFindPay.Parameters.AddWithValue("@inv", invNo)
                    Using adapter As New MySqlDataAdapter(cmdFindPay)
                        adapter.Fill(paymentsTable)
                    End Using
                End Using

                If paymentsTable.Rows.Count > 0 Then
                    For Each row As DataRow In paymentsTable.Rows
                        Dim pInvNo As String = If(IsDBNull(row("inv_no")), "", row("inv_no").ToString().Trim())
                        Dim pAmount As Double = Convert.ToDouble(row("Amount"))
                        Dim pCusID As Integer = Convert.ToInt32(row("CusID"))

                        If Not String.IsNullOrEmpty(pInvNo) Then
                            ' i. Restore Billing balances and status
                            Dim upBillSql As String = "UPDATE billing SET " &
                                                     "balance_due = balance_due + @amt, " &
                                                     "credit_balance_due = credit_balance_due + @amt, " &
                                                     "paid_amount = paid_amount - @amt " &
                                                     "WHERE LOWER(TRIM(inv_no)) = LOWER(TRIM(@inv)) AND customer_id = @cid"
                            Using cmdUpBill As New MySqlCommand(upBillSql, MySqlConn, transaction)
                                cmdUpBill.Parameters.AddWithValue("@amt", pAmount)
                                cmdUpBill.Parameters.AddWithValue("@inv", pInvNo)
                                cmdUpBill.Parameters.AddWithValue("@cid", pCusID)
                                cmdUpBill.ExecuteNonQuery()
                            End Using

                            ' ii. Restore Customer Credit
                            Using cmdUpCre As New MySqlCommand("UPDATE customer_credit SET amount = amount + @amt WHERE customer_id = @cid AND LOWER(TRIM(inv_no)) = LOWER(TRIM(@inv)) AND is_active = 1", MySqlConn, transaction)
                                cmdUpCre.Parameters.AddWithValue("@amt", pAmount)
                                cmdUpCre.Parameters.AddWithValue("@cid", pCusID)
                                cmdUpCre.Parameters.AddWithValue("@inv", pInvNo)
                                cmdUpCre.ExecuteNonQuery()
                            End Using
                        End If
                    Next
                End If

                transaction.Commit()
                ReturnReasonPanel.Visible = False
                MessageBox.Show("Cheque marked as Returned and logged successfully.", "Success")
                PerformSearch() ' Refresh grid (maintains filters)
                ClearFields()

            Catch ex As Exception
                transaction.Rollback()
                Throw ex
            End Try

        Catch ex As Exception
            MessageBox.Show("Error processing return: " & ex.Message)
        Finally
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Sub

    Private Sub ReturnReasonTextBox_KeyDown(sender As Object, e As KeyEventArgs) Handles ReturnReasonTextBox.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True ' Prevent new line in multiline textbox
            SubmitReturnButton.PerformClick()
        End If
    End Sub

    Private Sub Chq_NoTextBox_KeyDown(sender As Object, e As KeyEventArgs) Handles Chq_NoTextBox.KeyDown
        If e.KeyCode = Keys.Enter Then
            BankTextBox.Select()
        End If
    End Sub

    Private Sub ser_status_TextChanged(sender As Object, e As EventArgs) Handles ser_status.TextChanged
        PerformSearch()
    End Sub

    Private Sub ser_customer_TextChanged(sender As Object, e As EventArgs) Handles ser_customer.TextChanged
        ' Hide the bank suggestion panel during search
        Panel1.Visible = False
        PerformSearch()
    End Sub

    Private Sub ser_customer_KeyDown(sender As Object, e As KeyEventArgs) Handles ser_customer.KeyDown
        If e.KeyCode = Keys.Enter Then
            ser_status.Select()
        End If
    End Sub

    Private Sub ser_status_KeyDown(sender As Object, e As KeyEventArgs) Handles ser_status.KeyDown
        If e.KeyCode = Keys.Enter Then
            DateTimePicker4.Select()
        End If
    End Sub

    Private Sub DateTimePicker4_KeyDown(sender As Object, e As KeyEventArgs) Handles DateTimePicker4.KeyDown
        If e.KeyCode = Keys.Enter Then
            DateTimePicker3.Select()
        End If
    End Sub

    Private Sub DateTimePicker3_KeyDown(sender As Object, e As KeyEventArgs) Handles DateTimePicker3.KeyDown
        If e.KeyCode = Keys.Enter Then
            ser_chq.Select()
        End If
    End Sub

    Private Sub ser_chq_KeyDown(sender As Object, e As KeyEventArgs) Handles ser_chq.KeyDown
        If e.KeyCode = Keys.Enter Then
            ser_bank.Select()
        End If
    End Sub
    Private Sub ChaquereceivedDataGridView_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles ChaquereceivedDataGridView.CellClick
        If e.RowIndex >= 0 Then
            ProcessGridSelection(e.RowIndex)
        End If
    End Sub

    Private Sub ChaquereceivedDataGridView_RowHeaderMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles ChaquereceivedDataGridView.RowHeaderMouseClick
        If e.RowIndex >= 0 Then
            ProcessGridSelection(e.RowIndex)
        End If
    End Sub

    Private Sub ChaquereceivedDataGridView_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles ChaquereceivedDataGridView.CellDoubleClick
        If e.RowIndex >= 0 Then
            ProcessGridSelection(e.RowIndex)
        End If
    End Sub

    Private Sub ProcessGridSelection(rowIndex As Integer)
        Try
            Dim row As DataGridViewRow = ChaquereceivedDataGridView.Rows(rowIndex)

            isSelecting = True
            Chq_NoTextBox.Text = row.Cells(0).Value.ToString()
            C_NameTextBox.Text = row.Cells(1).Value.ToString()
            AmountTextBox.Text = row.Cells(4).Value.ToString()
            StatusTextBox.Text = row.Cells(5).Value.ToString()
            Issue_dateDateTimePicker.Value = Convert.ToDateTime(row.Cells(7).Value)
            Close_dateDateTimePicker.Value = Convert.ToDateTime(row.Cells(8).Value)
            selectedInvoiceNo = If(row.Cells(9).Value IsNot Nothing, row.Cells(9).Value.ToString(), "")

            Panel1.Visible = False
            selectedBankID = Convert.ToInt32(row.Cells(3).Value)

            If row.Cells(2).Value IsNot Nothing Then
                BankTextBox.Text = row.Cells(2).Value.ToString()
            Else
                If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()
                Dim bankQuery As String = "SELECT bank_name FROM bank WHERE id = @id"
                Using cmd As New MySqlCommand(bankQuery, MySqlConn)
                    cmd.Parameters.AddWithValue("@id", selectedBankID)
                    Dim result = cmd.ExecuteScalar()
                    If result IsNot Nothing Then BankTextBox.Text = result.ToString()
                End Using
            End If

            ' Update button visibility based on status
            Dim currentStatus As String = row.Cells(5).Value.ToString().ToUpper()
            If currentStatus = "PAID" Or currentStatus = "REALISED" Or currentStatus = "CLEARED" Or currentStatus = "RETURN" Or currentStatus = "RETURNED" Then
                btn_paid.Visible = False
                btn_return.Visible = False
            Else
                btn_paid.Visible = True
                btn_return.Visible = True
            End If

            ' Store original details for update operations
            originalChqNo = row.Cells(0).Value.ToString().Trim()
            originalBankID = Convert.ToInt32(row.Cells(3).Value)
            originalInvNo = If(row.Cells(9).Value IsNot Nothing, row.Cells(9).Value.ToString().Trim(), "")
            originalAmount = Convert.ToDecimal(row.Cells(4).Value)

            ' Mark as existing record and configure btn_save as "Update"
            isExistingRecord = True
            btn_save.Text = "Update"
            btn_save.Enabled = True

            isSelecting = False

        Catch ex As Exception
            MessageBox.Show("Error loading details: " & ex.Message)
        Finally
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Sub



    Private Sub ser_bank_TextChanged(sender As Object, e As EventArgs) Handles ser_bank.TextChanged
        ' Hide the bank suggestion panel during search
        Panel1.Visible = False
        PerformSearch()
    End Sub

    Private Sub ser_chq_TextChanged(sender As Object, e As EventArgs) Handles ser_chq.TextChanged
        ' Hide the bank suggestion panel during search
        Panel1.Visible = False
        PerformSearch()
    End Sub

    Private Sub FormatGridAndCalculateTotal()
        ' Set column headers, widths and visibility (matching load_chaque standard)
        If ChaquereceivedDataGridView.Columns.Count >= 10 Then
            With ChaquereceivedDataGridView
                .Columns(0).HeaderText = "Chq No"
                .Columns(0).AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                .Columns(0).Width = 80   ' check_number
                .Columns(1).HeaderText = "C Name"
                .Columns(1).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                .Columns(2).HeaderText = "Bank Name"
                .Columns(2).AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                .Columns(2).Width = 80   ' bank_name
                .Columns(3).Visible = False  ' bank_id
                .Columns(4).HeaderText = "Amount"
                .Columns(4).AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                .Columns(4).Width = 100  ' amount
                .Columns(5).HeaderText = "Status"
                .Columns(5).AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                .Columns(5).Width = 85   ' status
                .Columns(6).HeaderText = "Return Reason"
                .Columns(6).AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                .Columns(6).Width = 120  ' return_reason
                .Columns(7).HeaderText = "Issue Date"
                .Columns(7).AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                .Columns(7).Width = 105  ' issue_date
                .Columns(8).HeaderText = "Release Date"
                .Columns(8).AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                .Columns(8).Width = 105  ' check_release_date
                .Columns(9).HeaderText = "Inv No"
                .Columns(9).AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                .Columns(9).Width = 100  ' inv_no
                .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None
            End With
        End If

        Dim sss As Double = 0
        For s As Integer = 0 To ChaquereceivedDataGridView.Rows.Count - 1
            If ChaquereceivedDataGridView.Rows(s).IsNewRow Then Continue For

            ' Indices match standard: 4 = Amount
            If ChaquereceivedDataGridView.Rows(s).Cells(4).Value IsNot Nothing AndAlso Not IsDBNull(ChaquereceivedDataGridView.Rows(s).Cells(4).Value) Then
                sss += Convert.ToDouble(ChaquereceivedDataGridView.Rows(s).Cells(4).Value)
            End If
        Next

        ' Standardize the total label display
        Label7.Text = "Chq No:"
        Label13.Text = "Total Amount: LKR " & sss.ToString("N2")
    End Sub

    Private Sub PerformSearch()
        Try
            If MySqlConn.State <> ConnectionState.Open Then
                MySqlConn.Open()
            End If

            Dim sql As String = "SELECT cr.check_number, cr.check_name, b.bank_name, cr.bank_id, cr.amount, cr.status, " &
                               "rt.return_reason, cr.issue_date, cr.check_release_date, cr.inv_no " &
                               "FROM check_received cr " &
                               "LEFT JOIN bank b ON cr.bank_id = b.id " &
                               "LEFT JOIN cheque_returned rt ON cr.check_number = rt.check_number AND IFNULL(cr.inv_no, '') = IFNULL(rt.inv_no, '') AND cr.bank_id = rt.bank_id " &
                               "WHERE 1=1"

            ' Apply Filters
            If Not String.IsNullOrWhiteSpace(ser_customer.Text) Then
                sql &= " AND cr.check_name LIKE @customer"
            End If
            If Not String.IsNullOrWhiteSpace(ser_bank.Text) AndAlso ser_bank.Text <> "All" Then
                sql &= " AND b.bank_name LIKE @bank"
            End If
            If Not String.IsNullOrWhiteSpace(ser_status.Text) Then
                sql &= " AND cr.status LIKE @status"
            End If
            If Not String.IsNullOrWhiteSpace(ser_chq.Text) Then
                sql &= " AND cr.check_number LIKE @chq"
            End If

            ' Apply Date Range if not bypassed
            If CheckBox1.Checked = False Then
                sql &= " AND cr.check_release_date BETWEEN @start AND @end"
            End If

            Dim table As New DataTable()
            Dim adapter As New MySqlDataAdapter(sql, MySqlConn)

            ' Parameter values
            If Not String.IsNullOrWhiteSpace(ser_customer.Text) Then
                adapter.SelectCommand.Parameters.AddWithValue("@customer", ser_customer.Text.Trim() & "%")
            End If
            If Not String.IsNullOrWhiteSpace(ser_bank.Text) AndAlso ser_bank.Text <> "All" Then
                adapter.SelectCommand.Parameters.AddWithValue("@bank", ser_bank.Text.Trim() & "%")
            End If
            If Not String.IsNullOrWhiteSpace(ser_status.Text) Then
                adapter.SelectCommand.Parameters.AddWithValue("@status", ser_status.Text.Trim() & "%")
            End If
            If Not String.IsNullOrWhiteSpace(ser_chq.Text) Then
                adapter.SelectCommand.Parameters.AddWithValue("@chq", ser_chq.Text.Trim() & "%")
            End If

            If CheckBox1.Checked = False Then
                adapter.SelectCommand.Parameters.AddWithValue("@start", DateTimePicker4.Value.ToString("yyyy-MM-dd"))
                adapter.SelectCommand.Parameters.AddWithValue("@end", DateTimePicker3.Value.ToString("yyyy-MM-dd"))
            End If

            adapter.Fill(table)
            Dim dv As New DataView(table)
            dv.Sort = "check_name ASC"
            ChaquereceivedDataGridView.DataSource = dv

            ' Standard formatting and coloring
            FormatGridAndCalculateTotal()

        Catch ex As Exception
            MessageBox.Show("Error on search: " & ex.Message)
        Finally
            If MySqlConn.State = ConnectionState.Open Then
                MySqlConn.Close()
            End If
        End Try
    End Sub

    Private Sub DateTimePicker3_ValueChanged(sender As Object, e As EventArgs) Handles DateTimePicker3.ValueChanged
        PerformSearch()
    End Sub

    Private Sub DateTimePicker4_ValueChanged(sender As Object, e As EventArgs) Handles DateTimePicker4.ValueChanged
        PerformSearch()
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        ' Disable date pickers if "Without Date" is checked
        DateTimePicker4.Enabled = Not CheckBox1.Checked
        DateTimePicker3.Enabled = Not CheckBox1.Checked
        PerformSearch()
    End Sub

    ' Status Quick Filters
    Private Sub ser_green_Click(sender As Object, e As EventArgs) Handles ser_green.Click
        ser_status.Text = "Cleared"
        PerformSearch()
    End Sub

    Private Sub ser_yellow_Click(sender As Object, e As EventArgs) Handles ser_yellow.Click
        ser_status.Text = "Pending"
        PerformSearch()
    End Sub

    Private Sub ser_red_Click(sender As Object, e As EventArgs) Handles ser_red.Click
        ser_status.Text = "Returned"
        PerformSearch()
    End Sub


    Private Sub ChaquereceivedDataGridView_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles ChaquereceivedDataGridView.CellFormatting
        ' Apply row coloring based on the status column (Index 5)
        If e.RowIndex >= 0 Then
            Dim row As DataGridViewRow = ChaquereceivedDataGridView.Rows(e.RowIndex)
            If row.Cells(5).Value IsNot Nothing AndAlso Not IsDBNull(row.Cells(5).Value) Then
                Dim status As String = row.Cells(5).Value.ToString().ToUpper()

                If status = "PENDING" Then
                    row.DefaultCellStyle.BackColor = Color.Cornsilk
                    row.DefaultCellStyle.ForeColor = Color.Black
                ElseIf status = "RETURN" Or status = "RETURNED" Then
                    row.DefaultCellStyle.BackColor = Color.Red
                    row.DefaultCellStyle.ForeColor = Color.White
                ElseIf status = "PAID" Or status = "REALISED" Or status = "CLEARED" Then
                    row.DefaultCellStyle.BackColor = Color.YellowGreen
                    row.DefaultCellStyle.ForeColor = Color.Black
                Else
                    ' Reset to default if status doesn't match
                    row.DefaultCellStyle.BackColor = Color.White
                    row.DefaultCellStyle.ForeColor = Color.Black
                End If
            End If
        End If
    End Sub

    Private Sub ChaquereceivedDataGridView_ColumnHeaderMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles ChaquereceivedDataGridView.ColumnHeaderMouseClick
        ' Manual coloring loop removed; handled by CellFormatting event.
    End Sub
    Private Sub Issue_dateDateTimePicker_KeyDown(sender As Object, e As KeyEventArgs) Handles Issue_dateDateTimePicker.KeyDown
        If e.KeyCode = Keys.Enter Then
            C_NameTextBox.Select()
        End If
    End Sub
    Private Sub Close_dateDateTimePicker_KeyDown(sender As Object, e As KeyEventArgs) Handles Close_dateDateTimePicker.KeyDown
        If e.KeyCode = Keys.Enter Then
            btn_save.PerformClick()
        End If
    End Sub
    Private Sub AmountTextBox_KeyDown(sender As Object, e As KeyEventArgs) Handles AmountTextBox.KeyDown
        If e.KeyCode = Keys.Enter Then
            Close_dateDateTimePicker.Select()
        End If
    End Sub
    Private Sub LoadUserList()
        Try
            If MySqlConn.State <> ConnectionState.Open Then MySqlConn.Open()

            Dim adapter As New MySqlDataAdapter("SELECT id, name FROM user", MySqlConn)
            Dim dt As New DataTable()
            adapter.Fill(dt)

            ComboBox3.DataSource = dt
            ComboBox3.DisplayMember = "name"
            ComboBox3.ValueMember = "id"
            If Not String.IsNullOrEmpty(Module1.UserName) Then
                ComboBox3.Text = Module1.UserName
            Else
                ComboBox3.SelectedIndex = -1
            End If

            MySqlConn.Close()
        Catch ex As Exception
            MessageBox.Show("Error loading users: " & ex.Message)
        Finally
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Sub
    Private Sub LoadBanksToSearch()
        Try
            If MySqlConn.State <> ConnectionState.Open Then MySqlConn.Open()

            Dim adapter As New MySqlDataAdapter("SELECT DISTINCT bank_name FROM bank ORDER BY bank_name ASC", MySqlConn)
            Dim dt As New DataTable()
            adapter.Fill(dt)

            ' Add an 'All' row for "No Filter"
            Dim dr As DataRow = dt.NewRow()
            dr("bank_name") = "All"
            dt.Rows.InsertAt(dr, 0)

            ser_bank.DataSource = dt
            ser_bank.DisplayMember = "bank_name"
            ser_bank.ValueMember = "bank_name"
            ser_bank.SelectedIndex = 0

            MySqlConn.Close()
        Catch ex As Exception
            MessageBox.Show("Error loading banks for search: " & ex.Message)
        Finally
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Sub

    Private Sub ser_bank_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ser_bank.SelectedIndexChanged
        PerformSearch()
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
    Private Sub CusCuaque_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.F2 Then
            e.Handled = True
            e.SuppressKeyPress = True
            btn_add.PerformClick()
        ElseIf e.KeyCode = Keys.Delete Then
            e.Handled = True
            e.SuppressKeyPress = True
            btn_delete.PerformClick()
        End If
    End Sub

    Private Sub btnViewCustomerCredit_Click(sender As Object, e As EventArgs) Handles btnViewCustomerCredit.Click
        ' Use the MDI parent to open the form in the main container if hosted by Start
        If Me.MdiParent IsNot Nothing AndAlso TypeOf Me.MdiParent Is Start Then
            Dim startForm As Start = DirectCast(Me.MdiParent, Start)
            
            ' Pass the selected customer name to the Credit search box
            credit.NameTextBox1.Text = C_NameTextBox.Text
            
            ' Open the form using the standard MDI helper
            startForm.OpenMdiForm(credit)
            
            ' Ensure TabPage3 (Customer Payments) is selected since NameTextBox1 is there
            credit.TabControl1.SelectedTab = credit.TabPage3
        Else
            ' Fallback for standalone or non-MDI mode
            credit.NameTextBox1.Text = C_NameTextBox.Text
            credit.Show()
            credit.BringToFront()
            credit.Focus()
        End If
    End Sub

    Private Sub PositionSuggestionPanel()
        If activeSearchControl IsNot Nothing Then
            Dim ctrlScreenPos As Point = activeSearchControl.PointToScreen(Point.Empty)
            Dim parentPos As Point = Panel1.Parent.PointToClient(ctrlScreenPos)
            Panel1.Location = New Point(parentPos.X, parentPos.Y + activeSearchControl.Height)
            Panel1.BringToFront()
        End If
    End Sub

    Private Function IsOwnerSecureKeyValid() As Boolean
        If ComboBox3.SelectedIndex = -1 Then
            MessageBox.Show("Please select a user")
            ComboBox3.Focus()
            Return False
        End If

        Try
            If MySqlConn.State <> ConnectionState.Closed Then MySqlConn.Close()
            MySqlConn.Open()

            Dim userId As Integer = Convert.ToInt32(ComboBox3.SelectedValue)
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

                    If dbSecureKey = secure_key.Text.Trim() AndAlso roleName = "owner" Then
                        Return True
                    Else
                        MessageBox.Show("You are not authorized to edit cheque details.")
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
            If MySqlConn.State <> ConnectionState.Closed Then MySqlConn.Close()
        End Try
    End Function

    Private Sub PerformUpdate()
        ' 1. Validate secure key for owner
        If Not IsOwnerSecureKeyValid() Then Return

        ' 2. Basic UI Validations
        If Chq_NoTextBox.Text = "" Or C_NameTextBox.Text = "" Or BankTextBox.Text = "" Or AmountTextBox.Text = "" Then
            MessageBox.Show("Please fill in all fields before updating.")
            Return
        End If

        Dim transaction As MySqlTransaction = Nothing
        Try
            If MySqlConn.State <> ConnectionState.Closed Then MySqlConn.Close()
            MySqlConn.Open()

            ' Get Bank ID
            Dim bankId As Integer = 0
            Dim getBankIdQuery As String = "SELECT id FROM bank WHERE bank_name = @bname LIMIT 1"
            Using getCmd As New MySqlCommand(getBankIdQuery, MySqlConn)
                getCmd.Parameters.AddWithValue("@bname", BankTextBox.Text.Trim())
                Dim result = getCmd.ExecuteScalar()
                If result IsNot Nothing Then
                    bankId = Convert.ToInt32(result)
                Else
                    MessageBox.Show("The Bank name entered does not exist in the system. Please select a valid bank.")
                    Return
                End If
            End Using

            transaction = MySqlConn.BeginTransaction()

            Dim newAmount As Decimal = Convert.ToDecimal(AmountTextBox.Text)
            Dim diff As Decimal = newAmount - originalAmount

            ' A. Update check_received
            Dim updateCheckSql As String = "UPDATE check_received SET " &
                                          "check_number = @new_chq, " &
                                          "check_name = @name, " &
                                          "bank_id = @bank, " &
                                          "amount = @amt, " &
                                          "issue_date = @issue, " &
                                          "check_release_date = @release, " &
                                          "inv_no = @inv " &
                                          "WHERE check_number = @orig_chq AND bank_id = @orig_bid AND IFNULL(inv_no, '') = IFNULL(@orig_inv, '')"
            Using cmd As New MySqlCommand(updateCheckSql, MySqlConn, transaction)
                cmd.Parameters.AddWithValue("@new_chq", Chq_NoTextBox.Text.Trim())
                cmd.Parameters.AddWithValue("@name", C_NameTextBox.Text.Trim())
                cmd.Parameters.AddWithValue("@bank", bankId)
                cmd.Parameters.AddWithValue("@amt", newAmount)
                cmd.Parameters.AddWithValue("@issue", Issue_dateDateTimePicker.Value.ToString("yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@release", Close_dateDateTimePicker.Value.ToString("yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@inv", selectedInvoiceNo)
                cmd.Parameters.AddWithValue("@orig_chq", originalChqNo)
                cmd.Parameters.AddWithValue("@orig_bid", originalBankID)
                cmd.Parameters.AddWithValue("@orig_inv", originalInvNo)
                cmd.ExecuteNonQuery()
            End Using

            ' B. Update cheque_returned if it exists
            Dim updateRetSql As String = "UPDATE cheque_returned SET " &
                                         "check_number = @new_chq, " &
                                         "check_name = @name, " &
                                         "bank_id = @bank, " &
                                         "amount = @amt, " &
                                         "issue_date = @issue, " &
                                         "check_release_date = @release, " &
                                         "inv_no = @inv " &
                                         "WHERE check_number = @orig_chq AND bank_id = @orig_bid AND IFNULL(inv_no, '') = IFNULL(@orig_inv, '')"
            Using cmd As New MySqlCommand(updateRetSql, MySqlConn, transaction)
                cmd.Parameters.AddWithValue("@new_chq", Chq_NoTextBox.Text.Trim())
                cmd.Parameters.AddWithValue("@name", C_NameTextBox.Text.Trim())
                cmd.Parameters.AddWithValue("@bank", bankId)
                cmd.Parameters.AddWithValue("@amt", newAmount)
                cmd.Parameters.AddWithValue("@issue", Issue_dateDateTimePicker.Value.ToString("yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@release", Close_dateDateTimePicker.Value.ToString("yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@inv", selectedInvoiceNo)
                cmd.Parameters.AddWithValue("@orig_chq", originalChqNo)
                cmd.Parameters.AddWithValue("@orig_bid", originalBankID)
                cmd.Parameters.AddWithValue("@orig_inv", originalInvNo)
                cmd.ExecuteNonQuery()
            End Using

            ' C. Check if this is a payment cheque (exists in customer_payments)
            Dim isPaymentCheque As Boolean = False
            Dim checkPaySql As String = "SELECT COUNT(*) FROM customer_payments WHERE LOWER(TRIM(cheque_no)) = LOWER(TRIM(@orig_chq)) AND bank_id = @orig_bid AND (FIND_IN_SET(inv_no, REPLACE(@orig_inv, ', ', ',')) OR (inv_no = 'CREDIT' AND (@orig_inv = '' OR @orig_inv = 'CREDIT')))"
            Using checkPayCmd As New MySqlCommand(checkPaySql, MySqlConn, transaction)
                checkPayCmd.Parameters.AddWithValue("@orig_chq", originalChqNo)
                checkPayCmd.Parameters.AddWithValue("@orig_bid", originalBankID)
                checkPayCmd.Parameters.AddWithValue("@orig_inv", originalInvNo)
                isPaymentCheque = (Convert.ToInt32(checkPayCmd.ExecuteScalar()) > 0)
            End Using

            If isPaymentCheque Then
                ' Update customer_payments
                Dim updatePaySql As String = "UPDATE customer_payments SET " &
                                             "cheque_no = @new_chq, " &
                                             "bank_id = @bank " &
                                             "WHERE LOWER(TRIM(cheque_no)) = LOWER(TRIM(@orig_chq)) AND bank_id = @orig_bid AND (FIND_IN_SET(inv_no, REPLACE(@orig_inv, ', ', ',')) OR (inv_no = 'CREDIT' AND (@orig_inv = '' OR @orig_inv = 'CREDIT')))"
                Using cmd As New MySqlCommand(updatePaySql, MySqlConn, transaction)
                    cmd.Parameters.AddWithValue("@new_chq", Chq_NoTextBox.Text.Trim())
                    cmd.Parameters.AddWithValue("@bank", bankId)
                    cmd.Parameters.AddWithValue("@orig_chq", originalChqNo)
                    cmd.Parameters.AddWithValue("@orig_bid", originalBankID)
                    cmd.Parameters.AddWithValue("@orig_inv", originalInvNo)
                    cmd.ExecuteNonQuery()
                End Using

                ' If amount changed, update financial ledger for payments
                If diff <> 0 Then
                    ' 1. Update customer_payments amount
                    Dim updatePayAmtSql As String = "UPDATE customer_payments SET Amount = Amount + @diff " &
                                                    "WHERE LOWER(TRIM(cheque_no)) = LOWER(TRIM(@new_chq)) AND bank_id = @bank AND (FIND_IN_SET(inv_no, REPLACE(@orig_inv, ', ', ',')) OR (inv_no = 'CREDIT' AND (@orig_inv = '' OR @orig_inv = 'CREDIT'))) LIMIT 1"
                    Using cmd As New MySqlCommand(updatePayAmtSql, MySqlConn, transaction)
                        cmd.Parameters.AddWithValue("@diff", diff)
                        cmd.Parameters.AddWithValue("@new_chq", Chq_NoTextBox.Text.Trim())
                        cmd.Parameters.AddWithValue("@bank", bankId)
                        cmd.Parameters.AddWithValue("@orig_inv", originalInvNo)
                        cmd.ExecuteNonQuery()
                    End Using

                    ' 2. Update customer ID
                    Dim cusID As Integer = 0
                    Dim getCusSql As String = "SELECT CusID FROM customer_payments WHERE LOWER(TRIM(cheque_no)) = LOWER(TRIM(@chq)) AND bank_id = @bid LIMIT 1"
                    Using cmdCus As New MySqlCommand(getCusSql, MySqlConn, transaction)
                        cmdCus.Parameters.AddWithValue("@chq", Chq_NoTextBox.Text.Trim())
                        cmdCus.Parameters.AddWithValue("@bid", bankId)
                        Dim res = cmdCus.ExecuteScalar()
                        If res IsNot Nothing Then cusID = Convert.ToInt32(res)
                    End Using

                    If cusID > 0 Then
                        ' 3. Update customer_credit (subtract the diff from active credit)
                        Dim updateCreditSql As String = "UPDATE customer_credit SET amount = amount - @diff WHERE customer_id = @cid AND (FIND_IN_SET(inv_no, REPLACE(@inv, ', ', ',')) OR (inv_no = 'CREDIT' AND (@inv = '' OR @inv = 'CREDIT'))) AND is_active = 1"
                        Using cmd As New MySqlCommand(updateCreditSql, MySqlConn, transaction)
                            cmd.Parameters.AddWithValue("@diff", diff)
                            cmd.Parameters.AddWithValue("@cid", cusID)
                            cmd.Parameters.AddWithValue("@inv", originalInvNo)
                            cmd.ExecuteNonQuery()
                        End Using

                        ' 4. Update billing table if there's any invoice number
                        If Not String.IsNullOrEmpty(originalInvNo) Then
                            Dim updateBillSql As String = "UPDATE billing SET " &
                                                         "balance_due = balance_due - @diff, " &
                                                         "credit_balance_due = credit_balance_due - @diff, " &
                                                         "paid_amount = paid_amount + @diff " &
                                                         "WHERE customer_id = @cid AND FIND_IN_SET(inv_no, REPLACE(@inv, ', ', ','))"
                            Using cmd As New MySqlCommand(updateBillSql, MySqlConn, transaction)
                                cmd.Parameters.AddWithValue("@diff", diff)
                                cmd.Parameters.AddWithValue("@cid", cusID)
                                cmd.Parameters.AddWithValue("@inv", originalInvNo)
                                cmd.ExecuteNonQuery()
                            End Using
                        End If
                    End If
                End If
            Else
                ' Update billing table cheque_no and bank_id
                Dim updateBillSql As String = "UPDATE billing SET " &
                                              "cheque_no = @new_chq, " &
                                              "bank_id = @bank " &
                                              "WHERE LOWER(TRIM(cheque_no)) = LOWER(TRIM(@orig_chq)) AND bank_id = @orig_bid AND FIND_IN_SET(inv_no, REPLACE(@orig_inv, ', ', ','))"
                Using cmd As New MySqlCommand(updateBillSql, MySqlConn, transaction)
                    cmd.Parameters.AddWithValue("@new_chq", Chq_NoTextBox.Text.Trim())
                    cmd.Parameters.AddWithValue("@bank", bankId)
                    cmd.Parameters.AddWithValue("@orig_chq", originalChqNo)
                    cmd.Parameters.AddWithValue("@orig_bid", originalBankID)
                    cmd.Parameters.AddWithValue("@orig_inv", originalInvNo)
                    cmd.ExecuteNonQuery()
                End Using

                ' If amount changed, update billing balances for Billing Cheques
                If diff <> 0 Then
                    Dim cusID As Integer = 0
                    Dim getCusSql As String = "SELECT customer_id FROM billing WHERE LOWER(TRIM(cheque_no)) = LOWER(TRIM(@chq)) AND bank_id = @bid LIMIT 1"
                    Using cmdCus As New MySqlCommand(getCusSql, MySqlConn, transaction)
                        cmdCus.Parameters.AddWithValue("@chq", Chq_NoTextBox.Text.Trim())
                        cmdCus.Parameters.AddWithValue("@bid", bankId)
                        Dim res = cmdCus.ExecuteScalar()
                        If res IsNot Nothing Then cusID = Convert.ToInt32(res)
                    End Using

                    If cusID > 0 AndAlso Not String.IsNullOrEmpty(originalInvNo) Then
                        Dim updateBillBalSql As String = "UPDATE billing SET " &
                                                         "balance_due = balance_due - @diff, " &
                                                         "cheque_balance_due = cheque_balance_due - @diff, " &
                                                         "paid_amount = paid_amount + @diff " &
                                                         "WHERE customer_id = @cid AND FIND_IN_SET(inv_no, REPLACE(@inv, ', ', ','))"
                        Using cmd As New MySqlCommand(updateBillBalSql, MySqlConn, transaction)
                            cmd.Parameters.AddWithValue("@diff", diff)
                            cmd.Parameters.AddWithValue("@cid", cusID)
                            cmd.Parameters.AddWithValue("@inv", originalInvNo)
                            cmd.ExecuteNonQuery()
                        End Using
                    End If
                End If
            End If

            transaction.Commit()
            If MySqlConn.State <> ConnectionState.Closed Then MySqlConn.Close()
            MessageBox.Show("Cheque details updated successfully.", "Success")
            
            ' Reset UI state
            ClearFields()
            btn_save.Text = "Save"
            isExistingRecord = False
            PerformSearch()

        Catch ex As Exception
            Try
                If transaction IsNot Nothing AndAlso transaction.Connection IsNot Nothing Then
                    transaction.Rollback()
                End If
            Catch rollEx As Exception
                ' Ignore rollback exception to ensure the original error is displayed
            End Try
            MessageBox.Show("Error updating cheque: " & ex.Message)
        Finally
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Sub
End Class
