Imports MySql.Data.MySqlClient
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared
Public Class ChaqueOut
    Dim COMMAND As MySqlCommand
    Dim READER As MySqlDataReader
    Private selectedBankId As Integer = 0
    Private isExistingRecord As Boolean = False
    Private originalChqNo As String = ""
    Private originalBankId As Integer = 0
    Private originalInvNo As String = ""
    Private originalAmount As Decimal = 0
    Private Enum SearchMode
        Supplier
        Bank
    End Enum
    Private currentMode As SearchMode = SearchMode.Supplier
    Private activeSearchControl As Control = Nothing
    Private isSelecting As Boolean = False
    Private Sub load_chaque()
        Try
            If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()
            Dim table As New DataTable
            ' Fixed Join to get bank name from bank table
            ' Updated Query to group by cheque number and bank
            Dim query As String = "SELECT ci.chq_no, ci.c_name, b.bank_name as bank, SUM(ci.amount) as amount, ci.status, ci.issue_date, MAX(ci.close_date) as close_date, " &
                                 "GROUP_CONCAT(ci.inv_no SEPARATOR ', ') as inv_no, " &
                                 "(SELECT MAX(return_reason) FROM chaque_return_reason crr WHERE crr.check_number = ci.chq_no AND crr.bank_id = ci.bank_id) as return_reason, ci.bank_id " &
                                 "FROM chaque_issue ci " &
                                 "LEFT JOIN bank b ON ci.bank_id = b.id "

            If Not CheckBox1.Checked Then
                Dim Stadate As String = Format(Me.DateTimePicker1.Value, "yyyy-MM-dd")
                Dim enddat As String = Format(Me.DateTimePicker2.Value, "yyyy-MM-dd")
                query &= " WHERE ci.close_date BETWEEN '" & Stadate & "' AND '" & enddat & "' "
            End If

            query &= " GROUP BY ci.chq_no, ci.bank_id, ci.c_name, ci.status, ci.issue_date"

            Dim adapter As New MySqlDataAdapter(query, MySqlConn)

            adapter.Fill(table)

            Dim dv As New DataView(table)
            Dim filters As New List(Of String)

            If Not String.IsNullOrEmpty(TextBox1.Text) Then filters.Add(String.Format("c_name Like '{0}%'", TextBox1.Text.Replace("'", "''")))
            If Not String.IsNullOrEmpty(TextBox2.Text) Then filters.Add(String.Format("status Like '{0}%'", TextBox2.Text.Replace("'", "''")))
            If Not String.IsNullOrEmpty(TextBox5.Text) AndAlso TextBox5.Text <> "All" Then filters.Add(String.Format("bank Like '{0}%'", TextBox5.Text.Replace("'", "''")))
            If Not String.IsNullOrEmpty(TextBox6.Text) Then filters.Add(String.Format("chq_no Like '{0}%'", TextBox6.Text.Replace("'", "''")))

            If filters.Count > 0 Then
                dv.RowFilter = String.Join(" AND ", filters)
            End If
            dv.Sort = "c_name ASC, close_date DESC"

            ChaquereceivedDataGridView.DataSource = dv

            ChaquereceivedDataGridView.Columns(0).HeaderText = "Chq No"
            ChaquereceivedDataGridView.Columns(0).AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            ChaquereceivedDataGridView.Columns(0).Width = 80   ' Chq No
            ChaquereceivedDataGridView.Columns(1).HeaderText = "Supplier Name"
            ChaquereceivedDataGridView.Columns(1).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            ChaquereceivedDataGridView.Columns(2).HeaderText = "Bank"
            ChaquereceivedDataGridView.Columns(2).AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            ChaquereceivedDataGridView.Columns(2).Width = 80   ' Bank
            ChaquereceivedDataGridView.Columns(3).HeaderText = "Amount"
            ChaquereceivedDataGridView.Columns(3).AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            ChaquereceivedDataGridView.Columns(3).Width = 100  ' Amount
            ChaquereceivedDataGridView.Columns(4).HeaderText = "Status"
            ChaquereceivedDataGridView.Columns(4).AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            ChaquereceivedDataGridView.Columns(4).Width = 85   ' Status
            ChaquereceivedDataGridView.Columns(5).HeaderText = "Issue Date"
            ChaquereceivedDataGridView.Columns(5).AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            ChaquereceivedDataGridView.Columns(5).Width = 105  ' Issue Date
            ChaquereceivedDataGridView.Columns(6).HeaderText = "Release Date"
            ChaquereceivedDataGridView.Columns(6).AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            ChaquereceivedDataGridView.Columns(6).Width = 105  ' Release Date
            ChaquereceivedDataGridView.Columns(7).HeaderText = "Inv No"
            ChaquereceivedDataGridView.Columns(7).AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            ChaquereceivedDataGridView.Columns(7).Width = 100  ' Inv No
            If ChaquereceivedDataGridView.Columns.Count > 8 Then
                ChaquereceivedDataGridView.Columns(8).HeaderText = "Return Reason"
                ChaquereceivedDataGridView.Columns(8).AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                ChaquereceivedDataGridView.Columns(8).Width = 120  ' Return Reason
            End If
            ' Hide bank_id column
            If ChaquereceivedDataGridView.Columns.Count > 9 Then
                ChaquereceivedDataGridView.Columns(9).Visible = False
            End If
            ChaquereceivedDataGridView.AllowUserToAddRows = False
            ChaquereceivedDataGridView.AllowUserToDeleteRows = False
            ChaquereceivedDataGridView.ReadOnly = True
            ChaquereceivedDataGridView.MultiSelect = False
            ChaquereceivedDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            ChaquereceivedDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None

            ' Expand font size and row height
            ChaquereceivedDataGridView.DefaultCellStyle.Font = New Font("Segoe UI", 11)
            ChaquereceivedDataGridView.DefaultCellStyle.ForeColor = Color.Black
            ChaquereceivedDataGridView.GridColor = Color.Black
            ChaquereceivedDataGridView.RowTemplate.Height = 32
            ChaquereceivedDataGridView.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 11, FontStyle.Bold)
            ChaquereceivedDataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black

            ' Force refresh existing rows height
            For Each row As DataGridViewRow In ChaquereceivedDataGridView.Rows
                row.Height = 32
            Next

            MySqlConn.Close()
            gettotandcolor()
            UpdateActionButtonsVisibility()
        Catch ex As Exception
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Sub

    Private Sub load_users()
        Try
            MySqlConn.Open()
            Dim table As New DataTable
            Dim adapter As New MySqlDataAdapter("select name from user where (status is null or status = 'active')", MySqlConn)
            adapter.Fill(table)
            ComboBox3.DataSource = table
            ComboBox3.DisplayMember = "name"
            ComboBox3.ValueMember = "name"
            If Not String.IsNullOrEmpty(Module1.UserName) Then
                ComboBox3.Text = Module1.UserName
            Else
                ComboBox3.SelectedIndex = -1
            End If
            MySqlConn.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
            MySqlConn.Close()
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

            TextBox5.DataSource = dt
            TextBox5.DisplayMember = "bank_name"
            TextBox5.ValueMember = "bank_name"
            TextBox5.SelectedIndex = 0

            MySqlConn.Close()
        Catch ex As Exception
            MessageBox.Show("Error loading banks for search: " & ex.Message)
        Finally
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Sub

    Private Sub TextBox5_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TextBox5.SelectedIndexChanged
        load_chaque()
    End Sub



    Private Sub Chq_NoTextBox_TextChanged(sender As Object, e As EventArgs) Handles Chq_NoTextBox.TextChanged

    End Sub

    Private Sub C_NameTextBox_TextChanged(sender As Object, e As EventArgs) Handles C_NameTextBox.TextChanged
        If isSelecting Then Return
        activeSearchControl = C_NameTextBox
        currentMode = SearchMode.Supplier
        MySqlConn.Open()
        Dim bsource As New BindingSource
        Dim table As New DataTable
        Dim adapter As New MySqlDataAdapter("select name,tel_no,id  from supplier", MySqlConn)
        adapter.Fill(table)
        bsource.DataSource = table
        CustomerDataGridView.DataSource = table
        Dim dv As New DataView(table)
        dv.RowFilter = String.Format("name Like '{0}%'", C_NameTextBox.Text.Replace("'", "''"))
        dv.Sort = "name ASC"
        CustomerDataGridView.DataSource = dv
        CustomerDataGridView.Columns(0).HeaderText = "Supplier Name"
        CustomerDataGridView.Columns(0).Width = 450
        CustomerDataGridView.Columns(1).HeaderText = "Telephone No"
        CustomerDataGridView.Columns(1).Width = 300
        CustomerDataGridView.AllowUserToAddRows = False
        CustomerDataGridView.AllowUserToDeleteRows = False
        CustomerDataGridView.ReadOnly = True
        CustomerDataGridView.MultiSelect = False
        CustomerDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        CustomerDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        
        ' Expand font size and row height
        CustomerDataGridView.DefaultCellStyle.Font = New Font("Segoe UI", 11)
        CustomerDataGridView.DefaultCellStyle.ForeColor = Color.Black
        CustomerDataGridView.GridColor = Color.Black
        CustomerDataGridView.RowTemplate.Height = 32
        CustomerDataGridView.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 11, FontStyle.Bold)
        CustomerDataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black
        
        For Each row As DataGridViewRow In CustomerDataGridView.Rows
            row.Height = 32
        Next
        
        MySqlConn.Close()

        ' Hide ID column
        If CustomerDataGridView.Columns.Count > 0 Then
            For Each col As DataGridViewColumn In CustomerDataGridView.Columns
                If col.HeaderText.ToLower() = "id" Then
                    col.Visible = False
                End If
            Next
        End If

        ' SHOW Suggestion Panel if text matches
        If dv.Count > 0 AndAlso C_NameTextBox.Text.Trim() <> "" Then
            PositionSuggestionPanel()
            Panel1.Visible = True
            Panel1.BringToFront()
        Else
            Panel1.Visible = False
        End If
    End Sub

    Private Sub ChaqueOut_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Self-healing Migration: Ensure bank_id column exists in chaque_issue
        Try
            If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()
            Dim checkBankIdColCmd As New MySqlCommand("SHOW COLUMNS FROM chaque_issue LIKE 'bank_id'", MySqlConn)
            Dim bankIdExists = checkBankIdColCmd.ExecuteScalar()
            If bankIdExists Is Nothing Then
                ' Add bank_id column. We don't specify AFTER bank because bank column might be gone.
                Dim addBankIdColCmd As New MySqlCommand("ALTER TABLE chaque_issue ADD COLUMN bank_id INT", MySqlConn)
                addBankIdColCmd.ExecuteNonQuery()
            End If
            MySqlConn.Close()
        Catch ex As Exception
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try

        Me.KeyPreview = True
        If Panel1.Visible Then
            Panel1.Visible = False
        Else
            Panel1.Visible = False

        End If
        Me.load_chaque()
        Me.load_users()
        Me.LoadBanksToSearch()
        CheckBox1.Checked = True

        ' Set specific textboxes to read-only as requested
        Chq_NoTextBox.ReadOnly = False
        BankTextBox.ReadOnly = False
        AmountTextBox.ReadOnly = False
    End Sub

    Private Sub ChaqueOut_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.F2 Then
            Addbtn.PerformClick()
        ElseIf e.KeyCode = Keys.Delete Then
            deletebtn.PerformClick()
        End If
    End Sub

    Private Sub C_NameTextBox_KeyPress(sender As Object, e As KeyPressEventArgs) Handles C_NameTextBox.KeyPress
        If e.KeyChar = ChrW(Keys.Escape) Then
            Panel1.Visible = False
            PurchaseDetailPanel.Visible = False
        End If
    End Sub

    Private Sub C_NameTextBox_KeyDown(sender As Object, e As KeyEventArgs) Handles C_NameTextBox.KeyDown
        If e.KeyCode = Keys.Escape Then
            Panel1.Visible = False
            PurchaseDetailPanel.Visible = False
        End If
        If e.KeyCode = Keys.Down Or e.KeyCode = Keys.Up Then
            If Panel1.Visible AndAlso CustomerDataGridView.Rows.Count > 0 Then
                CustomerDataGridView.Focus()
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub CustomerDataGridView_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles CustomerDataGridView.CellClick
        If e.RowIndex >= 0 AndAlso e.RowIndex < CustomerDataGridView.Rows.Count Then
            Dim selectedRow As DataGridViewRow = CustomerDataGridView.Rows(e.RowIndex)
            If currentMode = SearchMode.Supplier Then
                Dim supplierName As String = selectedRow.Cells(0).Value.ToString()
                Dim supplierId As Integer = 0
                Try
                    If selectedRow.Cells("id").Value IsNot Nothing Then
                        supplierId = Convert.ToInt32(selectedRow.Cells("id").Value)
                    End If
                Catch ex As Exception
                    If selectedRow.Cells.Count > 2 AndAlso selectedRow.Cells(2).Value IsNot Nothing Then
                        supplierId = Convert.ToInt32(selectedRow.Cells(2).Value)
                    End If
                End Try

                isSelecting = True
                C_NameTextBox.Text = supplierName
                isSelecting = False
                Panel1.Visible = False

                If supplierId > 0 Then
                    LoadSupplierPurchases(supplierId)
                End If

                If PurchaseDetailPanel.Visible Then
                    PurchaseDataGridView.Focus()
                Else
                    Chq_NoTextBox.Select()
                End If
            ElseIf currentMode = SearchMode.Bank Then
                ' Bank selection logic
                If CustomerDataGridView.Columns.Contains("id") Then
                    selectedBankId = CInt(selectedRow.Cells("id").Value)
                    isSelecting = True
                    BankTextBox.Text = selectedRow.Cells("bank_name").Value.ToString()
                    isSelecting = False
                    Panel1.Visible = False
                    AmountTextBox.Select()
                ElseIf selectedRow.Cells.Count >= 2 Then
                    isSelecting = True
                    BankTextBox.Text = selectedRow.Cells(1).Value.ToString()
                    isSelecting = False
                    Panel1.Visible = False
                    AmountTextBox.Select()
                End If
            End If
        End If
    End Sub

    Private Sub CustomerDataGridView_KeyDown(sender As Object, e As KeyEventArgs) Handles CustomerDataGridView.KeyDown
        If e.KeyCode = Keys.Enter Then
            If CustomerDataGridView.CurrentRow IsNot Nothing Then
                Dim selectedRow As DataGridViewRow = CustomerDataGridView.CurrentRow
                If currentMode = SearchMode.Supplier Then
                    Dim supplierName As String = selectedRow.Cells(0).Value.ToString()
                    Dim supplierId As Integer = 0
                    Try
                        If selectedRow.Cells("id").Value IsNot Nothing Then
                            supplierId = Convert.ToInt32(selectedRow.Cells("id").Value)
                        End If
                    Catch ex As Exception
                        If selectedRow.Cells.Count > 2 AndAlso selectedRow.Cells(2).Value IsNot Nothing Then
                            supplierId = Convert.ToInt32(selectedRow.Cells(2).Value)
                        End If
                    End Try

                    isSelecting = True
                    C_NameTextBox.Text = supplierName
                    isSelecting = False
                    Panel1.Visible = False

                    If supplierId > 0 Then
                        LoadSupplierPurchases(supplierId)
                    End If

                    If PurchaseDetailPanel.Visible Then
                        PurchaseDataGridView.Focus()
                    Else
                        Chq_NoTextBox.Select()
                    End If
                ElseIf currentMode = SearchMode.Bank Then
                    ' Bank selection logic
                    If CustomerDataGridView.Columns.Contains("id") Then
                        selectedBankId = CInt(selectedRow.Cells("id").Value)
                        isSelecting = True
                        BankTextBox.Text = selectedRow.Cells("bank_name").Value.ToString()
                        isSelecting = False
                        Panel1.Visible = False
                        AmountTextBox.Select()
                    ElseIf selectedRow.Cells.Count >= 2 Then
                        isSelecting = True
                        BankTextBox.Text = selectedRow.Cells(1).Value.ToString()
                        isSelecting = False
                        Panel1.Visible = False
                        AmountTextBox.Select()
                    End If
                End If
                e.Handled = True
            End If
        End If
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
            If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()
            Dim table As New DataTable
            Dim adapter As New MySqlDataAdapter("SELECT id, bank_name, amount FROM bank", MySqlConn)
            adapter.Fill(table)

            Dim dv As New DataView(table)
            dv.RowFilter = String.Format("bank_name Like '%{0}%'", BankTextBox.Text.Replace("'", "''"))

            CustomerDataGridView.DataSource = dv

            CustomerDataGridView.Columns(0).Visible = False ' ID
            CustomerDataGridView.Columns(1).HeaderText = "Bank Name"
            CustomerDataGridView.Columns(1).Width = 450
            CustomerDataGridView.Columns(2).HeaderText = "Balance"
            CustomerDataGridView.Columns(2).Width = 300
            CustomerDataGridView.AllowUserToAddRows = False
            CustomerDataGridView.AllowUserToDeleteRows = False
            CustomerDataGridView.ReadOnly = True
            CustomerDataGridView.MultiSelect = False
            CustomerDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            CustomerDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

            PositionSuggestionPanel()
            Panel1.Visible = True
            MySqlConn.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Sub

    Private Sub Addbtn_Click(sender As Object, e As EventArgs) Handles Addbtn.Click
        Chq_NoTextBox.Clear()
        C_NameTextBox.Clear()
        BankTextBox.Clear()
        AmountTextBox.Clear()
        Inv_NoTextBox.Clear()
        selectedBankId = 0

        ' Reset record state and enable save
        isExistingRecord = False
        savebtn.Enabled = True
        savebtn.Text = "Save"

        Issue_dateDateTimePicker.Select()
    End Sub

    Private Sub Issue_dateDateTimePicker_KeyDown(sender As Object, e As KeyEventArgs) Handles Issue_dateDateTimePicker.KeyDown
        If e.KeyCode = Keys.Enter Then
            C_NameTextBox.Select()
        End If
    End Sub

    Private Sub BankTextBox_KeyDown(sender As Object, e As KeyEventArgs) Handles BankTextBox.KeyDown
        If e.KeyCode = Keys.Enter Then
            ' If the suggestion panel is visible, pick the first item
            If Panel1.Visible AndAlso CustomerDataGridView.Rows.Count > 0 Then
                e.SuppressKeyPress = True
                isSelecting = True
                If CustomerDataGridView.Columns.Contains("bank_name") Then
                    BankTextBox.Text = CustomerDataGridView.Rows(0).Cells("bank_name").Value.ToString()
                    If CustomerDataGridView.Columns.Contains("id") Then
                        selectedBankId = CInt(CustomerDataGridView.Rows(0).Cells("id").Value)
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

    Private Sub savebtn_Click(sender As Object, e As EventArgs) Handles savebtn.Click
        ' 0. Handle update for existing records
        If isExistingRecord Then
            PerformUpdate()
            Return
        End If

        If Chq_NoTextBox.Text = "" Then
            MessageBox.Show("Please Enter ChqNo")
        Else
            If C_NameTextBox.Text = "" Then
                MessageBox.Show("Please Enter Customer Name")
            Else
                If BankTextBox.Text = "" Then
                    MessageBox.Show("Enter Bank")
                Else
                    If AmountTextBox.Text = "" Then
                        MessageBox.Show("Enter Chaque Amount")
                    Else
                        Dim State As String = "PENDING"
                        Dim startDate As String
                        Dim CloseDate As String
                        Dim chqno As String = Chq_NoTextBox.Text
                        Dim nchqn As String = chqno.Trim()

                        startDate = Format(Me.Issue_dateDateTimePicker.Value, "yyyy-MM-dd")
                        CloseDate = Format(Me.Close_dateDateTimePicker.Value, "yyyy-MM-dd")
                        Try
                            If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()

                            If selectedBankId = 0 Then
                                Dim getBankIdQuery As String = "SELECT id FROM bank WHERE bank_name = @bname LIMIT 1"
                                Using getCmd As New MySqlCommand(getBankIdQuery, MySqlConn)
                                    getCmd.Parameters.AddWithValue("@bname", BankTextBox.Text.Trim())
                                    Dim res = getCmd.ExecuteScalar()
                                    If res IsNot Nothing Then
                                        selectedBankId = Convert.ToInt32(res)
                                    Else
                                        MessageBox.Show("The Bank name entered does not exist in the system. Please select a valid bank.")
                                        MySqlConn.Close()
                                        Return
                                    End If
                                End Using
                            End If

                            ' Check for duplicate cheque - Sanitize amount to prevent parameter issues
                            Dim cleanAmount As String = AmountTextBox.Text.Replace(",", "")
                            Dim checkQuery As String = "SELECT COUNT(*) FROM chaque_issue WHERE chq_no = @chq AND bank_id = @bid AND amount = @amt"
                            Using checkCmd As New MySqlCommand(checkQuery, MySqlConn)
                                checkCmd.Parameters.AddWithValue("@chq", nchqn)
                                checkCmd.Parameters.AddWithValue("@bid", selectedBankId)
                                checkCmd.Parameters.AddWithValue("@amt", cleanAmount)
                                Dim count As Integer = Convert.ToInt32(checkCmd.ExecuteScalar())
                                If count > 0 Then
                                    MessageBox.Show("already have that cheque")
                                    MySqlConn.Close()
                                    Return
                                End If
                            End Using

                            Dim invList As String = Inv_NoTextBox.Text
                            Dim individualInvs() As String = invList.Split(New String() {", "}, StringSplitOptions.RemoveEmptyEntries)
                            
                            If individualInvs.Length = 0 Then
                                ' B. Insert a single record with empty invoice number (manual cheque)
                                Dim insertQuery As String = "insert into chaque_issue (chq_no,c_name,bank_id,amount,status,issue_date,close_date,inv_no) values (@chq, @name, @bid, @amt, @status, @idate, @cdate, @inv)"
                                Using insCmd As New MySqlCommand(insertQuery, MySqlConn)
                                    insCmd.Parameters.AddWithValue("@chq", nchqn)
                                    insCmd.Parameters.AddWithValue("@name", C_NameTextBox.Text.Replace("'", "''"))
                                    insCmd.Parameters.AddWithValue("@bid", selectedBankId)
                                    insCmd.Parameters.AddWithValue("@amt", Convert.ToDouble(cleanAmount))
                                    insCmd.Parameters.AddWithValue("@status", State)
                                    insCmd.Parameters.AddWithValue("@idate", startDate)
                                    insCmd.Parameters.AddWithValue("@cdate", CloseDate)
                                    insCmd.Parameters.AddWithValue("@inv", "")
                                    insCmd.ExecuteNonQuery()
                                End Using
                            Else
                                Dim processedInvs As New List(Of String)
                                For Each inv As String In individualInvs
                                    inv = inv.Trim()
                                    If String.IsNullOrEmpty(inv) OrElse processedInvs.Contains(inv) Then Continue For
                                    processedInvs.Add(inv)

                                    ' A. Fetch the specific amount for this invoice from purchasing or supplier_payments
                                    Dim individualAmount As Double = 0
                                    
                                    ' Try purchasing first (Added status filter and SUM to capture all split records for this cheque link)
                                    Dim purQuery As String = "SELECT SUM(cheque_balance_due) FROM purchasing p JOIN supplier s ON p.supplier_id = s.id " &
                                                           "WHERE TRIM(p.pur_id) = @pid AND s.name = @name " &
                                                           "AND (p.status IN ('Cheque' ,'%Cheque%','Chaque', 'cash_Cheque', 'Cash_Cheque', 'Mixed_Payment', 'Credit_Cheque')) " &
                                                           "AND TRIM(IF(TRIM(p.cqe_no) = '' OR p.cqe_no IS NULL, TRIM(p.pur_id), p.cqe_no)) = @chq"
                                    Using purCmd As New MySqlCommand(purQuery, MySqlConn)
                                        purCmd.Parameters.AddWithValue("@pid", inv)
                                        purCmd.Parameters.AddWithValue("@name", C_NameTextBox.Text)
                                        purCmd.Parameters.AddWithValue("@chq", nchqn)
                                        Dim res = purCmd.ExecuteScalar()
                                        If res IsNot Nothing AndAlso Not IsDBNull(res) Then
                                            individualAmount = Convert.ToDouble(res)
                                        End If
                                    End Using

                                    ' If not found or zero, try supplier_payments (Added SUM to capture all split records for this cheque)
                                    If individualAmount <= 0 Then
                                        Dim payQuery As String = "SELECT SUM(amount) FROM supplier_payments WHERE TRIM(inv_no) = @inv AND TRIM(chq_no) = @chq AND bank_id = @bid AND (type LIKE '%Cheque%' OR type LIKE 'Chaque')"
                                        Using payCmd As New MySqlCommand(payQuery, MySqlConn)
                                            payCmd.Parameters.AddWithValue("@inv", inv)
                                            payCmd.Parameters.AddWithValue("@chq", nchqn)
                                            payCmd.Parameters.AddWithValue("@bid", selectedBankId)
                                            Dim res = payCmd.ExecuteScalar()
                                            If res IsNot Nothing AndAlso Not IsDBNull(res) Then
                                                individualAmount = Convert.ToDouble(res)
                                            End If
                                        End Using
                                    End If

                                    ' Catch-all: if still 0 but we have a single invoice, use the textbox amount
                                    If individualAmount <= 0 AndAlso individualInvs.Length = 1 Then
                                        Double.TryParse(AmountTextBox.Text.Replace(",", ""), individualAmount)
                                    End If

                                    ' B. Insert the individual record
                                    Dim insertQuery As String = "insert into chaque_issue (chq_no,c_name,bank_id,amount,status,issue_date,close_date,inv_no) values (@chq, @name, @bid, @amt, @status, @idate, @cdate, @inv)"
                                    Using insCmd As New MySqlCommand(insertQuery, MySqlConn)
                                        insCmd.Parameters.AddWithValue("@chq", nchqn)
                                        insCmd.Parameters.AddWithValue("@name", C_NameTextBox.Text.Replace("'", "''"))
                                        insCmd.Parameters.AddWithValue("@bid", selectedBankId)
                                        insCmd.Parameters.AddWithValue("@amt", individualAmount)
                                        insCmd.Parameters.AddWithValue("@status", State)
                                        insCmd.Parameters.AddWithValue("@idate", startDate)
                                        insCmd.Parameters.AddWithValue("@cdate", CloseDate)
                                        insCmd.Parameters.AddWithValue("@inv", inv)
                                        insCmd.ExecuteNonQuery()
                                    End Using
                                Next
                            End If

                            MySqlConn.Close()
                            MessageBox.Show("Saved Successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                            load_chaque()
                            Chq_NoTextBox.Clear()
                            C_NameTextBox.Clear()
                            BankTextBox.Clear()
                            AmountTextBox.Clear()
                            Inv_NoTextBox.Clear()
                            Chq_NoTextBox.Select()
                        Catch ex As Exception
                            MessageBox.Show(ex.Message)
                            MySqlConn.Close()
                        End Try
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub deletebtn_Click(sender As Object, e As EventArgs) Handles deletebtn.Click
        ' Check if user is selected
        If ComboBox3.Text = "" Then
            MessageBox.Show("Please Select User First", "Restriction", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            ComboBox3.Select()
            ComboBox3.DroppedDown = True
            Return
        End If

        ' Check if secure key is entered
        If secure_key.Text = "" Then
            MessageBox.Show("You Are Not Authorized To Delete This Item", "Restriction", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            secure_key.Select()
            Return
        End If

        ' Verify secure key from database
        Dim isKeyValid As Boolean = False
        Try
            MySqlConn.Open()
            Dim Query As String = "SELECT hiddenSecureKey FROM user WHERE name = @uname AND (status IS NULL OR status = 'active')"
            COMMAND = New MySqlCommand(Query, MySqlConn)
            COMMAND.Parameters.AddWithValue("@uname", ComboBox3.Text)
            READER = COMMAND.ExecuteReader
            If READER.Read() Then
                If READER("hiddenSecureKey").ToString() = secure_key.Text.Trim() Then
                    isKeyValid = True
                End If
            End If
            MySqlConn.Close()
        Catch ex As Exception
            MessageBox.Show("Error verifying key: " & ex.Message)
            MySqlConn.Close()
            Return
        End Try

        If Not isKeyValid Then
            MessageBox.Show("You Are Not Authorized To Delete This Item", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Error)
            secure_key.Clear()
            Return
        End If

        'This button for the delete'
        Dim result As DialogResult = MessageBox.Show("Are you Sure to Delete This?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If result = DialogResult.Yes Then
            Try
                If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()

                ' SPECIAL CASE: If the status is RETURNED, just revert to PENDING
                If StatusTextBox.Text = "RETURNED" Then
                    ' 1. Update status in chaque_issue
                    Dim updateChqQuery As String = "UPDATE chaque_issue SET status='PENDING' WHERE chq_no=@chq AND bank_id=@bid"
                    COMMAND = New MySqlCommand(updateChqQuery, MySqlConn)
                    COMMAND.Parameters.AddWithValue("@chq", Chq_NoTextBox.Text)
                    COMMAND.Parameters.AddWithValue("@bid", selectedBankId)
                    COMMAND.ExecuteNonQuery()

                    ' 2. Delete from chaque_return_reason
                    Dim deleteReturnQuery As String = "DELETE FROM chaque_return_reason WHERE check_number=@chq AND bank_id=@bid"
                    COMMAND = New MySqlCommand(deleteReturnQuery, MySqlConn)
                    COMMAND.Parameters.AddWithValue("@chq", Chq_NoTextBox.Text)
                    COMMAND.Parameters.AddWithValue("@bid", selectedBankId)
                    COMMAND.ExecuteNonQuery()

                    MySqlConn.Close()
                    load_chaque()
                    MessageBox.Show("Cheque status reverted back to PENDING and return reason removed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Return
                End If

                ' FETCH ALL INDIVIDUAL RECORDS for this cheque to process balance reversal if RELEASED
                Dim chqRecords As New DataTable()
                Dim fetchQuery As String = "SELECT inv_no, amount FROM chaque_issue WHERE chq_no = @chq AND bank_id = @bid"
                Using fetchCmd As New MySqlCommand(fetchQuery, MySqlConn)
                    fetchCmd.Parameters.AddWithValue("@chq", Chq_NoTextBox.Text)
                    fetchCmd.Parameters.AddWithValue("@bid", selectedBankId)
                    Using adapter As New MySqlDataAdapter(fetchCmd)
                        adapter.Fill(chqRecords)
                    End Using
                End Using

                ' Reverse balance updates for EACH invoice (Reversed upon deletion since payment cheques are deducted immediately)
                For Each row As DataRow In chqRecords.Rows
                    Dim rawInv As String = row("inv_no").ToString()
                    Dim rawAmount As Double = 0
                    If Not Double.TryParse(row("amount").ToString(), rawAmount) Then rawAmount = 0

                    ' Handle potential combined records (backward compatibility)
                    Dim individualInvs() As String = rawInv.Split(New String() {", "}, StringSplitOptions.RemoveEmptyEntries)

                    For Each targetInv As String In individualInvs
                        targetInv = targetInv.Trim()
                        If String.IsNullOrEmpty(targetInv) Then Continue For

                        ' Lookup the individual amount if it was a combined record
                        Dim rowAmount As Double = rawAmount
                        If individualInvs.Length > 1 Then
                            rowAmount = 0
                            ' Look up amount from source
                            Dim purQuery As String = "SELECT cheque_balance_due FROM purchasing p JOIN supplier s ON p.supplier_id = s.id WHERE TRIM(p.pur_id) = @pid AND s.name = @name"
                            Using purCmd As New MySqlCommand(purQuery, MySqlConn)
                                purCmd.Parameters.AddWithValue("@pid", targetInv)
                                purCmd.Parameters.AddWithValue("@name", C_NameTextBox.Text)
                                Dim res = purCmd.ExecuteScalar()
                                If res IsNot Nothing AndAlso Not IsDBNull(res) Then
                                    rowAmount = Convert.ToDouble(res)
                                End If
                            End Using

                            If rowAmount <= 0 Then
                                Dim payQuery As String = "SELECT amount FROM supplier_payments WHERE TRIM(inv_no) = @inv AND TRIM(chq_no) = @chq AND bank_id = @bid"
                                Using payCmd As New MySqlCommand(payQuery, MySqlConn)
                                    payCmd.Parameters.AddWithValue("@inv", targetInv)
                                    payCmd.Parameters.AddWithValue("@chq", Chq_NoTextBox.Text)
                                    payCmd.Parameters.AddWithValue("@bid", selectedBankId)
                                    Dim res = payCmd.ExecuteScalar()
                                    If res IsNot Nothing AndAlso Not IsDBNull(res) Then
                                        rowAmount = Convert.ToDouble(res)
                                    End If
                                End Using
                            End If
                            
                            If rowAmount <= 0 Then rowAmount = rawAmount / individualInvs.Length
                        End If

                        ' A. Determine source of cheque
                        Dim isPaymentCheque As Boolean = False
                        Dim checkSourceQuery As String = "SELECT COUNT(*) FROM supplier_payments WHERE TRIM(chq_no) = TRIM(@chq) AND TRIM(inv_no) = TRIM(@inv)"
                        COMMAND = New MySqlCommand(checkSourceQuery, MySqlConn)
                        COMMAND.Parameters.AddWithValue("@chq", Chq_NoTextBox.Text)
                        COMMAND.Parameters.AddWithValue("@inv", targetInv)
                        isPaymentCheque = (Convert.ToInt32(COMMAND.ExecuteScalar()) > 0)

                        ' B. Reverse if it is a payment cheque (always reversed) OR if it is a billing cheque and status was RELEASED
                        If isPaymentCheque OrElse (StatusTextBox.Text = "RELEASED") Then
                            ' 1. Reverse updates in purchasing table
                            Dim purReverseQuery As String = ""
                            If isPaymentCheque Then
                                purReverseQuery = "UPDATE purchasing p JOIN supplier s ON p.supplier_id = s.id SET p.balance_due = p.balance_due + @amt, p.credit_balance_due = p.credit_balance_due + @amt, p.paid_amount = p.paid_amount - @amt WHERE TRIM(p.pur_id) = TRIM(@pid) AND TRIM(s.name) = TRIM(@name)"
                            Else
                                purReverseQuery = "UPDATE purchasing p JOIN supplier s ON p.supplier_id = s.id SET p.balance_due = p.balance_due + @amt, p.cheque_balance_due = p.cheque_balance_due + @amt, p.paid_amount = p.paid_amount - @amt WHERE TRIM(p.pur_id) = TRIM(@pid) AND TRIM(s.name) = TRIM(@name)"
                            End If
                            
                            COMMAND = New MySqlCommand(purReverseQuery, MySqlConn)
                            COMMAND.Parameters.AddWithValue("@amt", rowAmount)
                            COMMAND.Parameters.AddWithValue("@pid", targetInv)
                            COMMAND.Parameters.AddWithValue("@name", C_NameTextBox.Text)
                            COMMAND.ExecuteNonQuery()

                            ' 2. Recalculate status
                            Dim statusRevQuery As String = "UPDATE purchasing p JOIN supplier s ON p.supplier_id = s.id SET p.status = CASE WHEN p.balance_due <= 0 THEN 'success' WHEN p.credit_balance_due > 0 AND p.cheque_balance_due > 0 AND p.paid_amount > 0 THEN 'Mixed_Payment' WHEN p.credit_balance_due > 0 AND p.cheque_balance_due > 0 THEN 'Credit_Cheque' WHEN p.credit_balance_due > 0 AND p.paid_amount > 0 THEN 'cash_Credit' WHEN p.credit_balance_due > 0 THEN 'Credit' WHEN p.cheque_balance_due > 0 AND p.paid_amount > 0 THEN 'Cash_Cheque' WHEN p.cheque_balance_due > 0 THEN 'Cheque' ELSE p.status END WHERE TRIM(p.pur_id) = TRIM(@pid) AND TRIM(s.name) = TRIM(@name)"
                            COMMAND = New MySqlCommand(statusRevQuery, MySqlConn)
                            COMMAND.Parameters.AddWithValue("@pid", targetInv)
                            COMMAND.Parameters.AddWithValue("@name", C_NameTextBox.Text)
                            COMMAND.ExecuteNonQuery()

                            ' 3. Reverse updates in supplicer_credit (ONLY for Payment Cheques)
                            If isPaymentCheque Then
                                Dim creditReverseQuery As String = "UPDATE supplicer_credit SET amount = amount + @amt WHERE TRIM(inv_no) = TRIM(@inv) AND TRIM(sname) = TRIM(@name)"
                                COMMAND = New MySqlCommand(creditReverseQuery, MySqlConn)
                                COMMAND.Parameters.AddWithValue("@amt", rowAmount)
                                COMMAND.Parameters.AddWithValue("@inv", targetInv)
                                COMMAND.Parameters.AddWithValue("@name", C_NameTextBox.Text)
                                Dim affectedRows = COMMAND.ExecuteNonQuery()

                                ' Fallback
                                If affectedRows = 0 Then
                                    Dim fallbackQuery As String = "UPDATE supplicer_credit SET amount = amount + @amt WHERE TRIM(sname) = TRIM(@name) ORDER BY getdate DESC LIMIT 1"
                                    COMMAND = New MySqlCommand(fallbackQuery, MySqlConn)
                                    COMMAND.Parameters.AddWithValue("@amt", rowAmount)
                                    COMMAND.Parameters.AddWithValue("@name", C_NameTextBox.Text)
                                    COMMAND.ExecuteNonQuery()
                                End If
                            End If
                        End If
                    Next
                Next

                ' Delete from chaque_issue (Deletes ALL records for this physical cheque)
                Dim deleteQuery As String = "DELETE FROM chaque_issue WHERE chq_no=@chq AND bank_id=@bid"
                COMMAND = New MySqlCommand(deleteQuery, MySqlConn)
                COMMAND.Parameters.AddWithValue("@chq", Chq_NoTextBox.Text)
                COMMAND.Parameters.AddWithValue("@bid", selectedBankId)
                COMMAND.ExecuteNonQuery()

                Module1.LogDeletion("Supplier Cheque", Chq_NoTextBox.Text, "Bank ID: " & selectedBankId.ToString() & ", Amount: " & AmountTextBox.Text & ", Supplier: " & C_NameTextBox.Text & ", Inv No: " & Inv_NoTextBox.Text & ", Status: " & StatusTextBox.Text)

                MySqlConn.Close()
                load_chaque()
                MessageBox.Show("Record deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                MessageBox.Show(ex.Message)
                If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
            End Try
        End If

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        'This is the paid button (Mark as RELEASED)'
        If StatusTextBox.Text = "RELEASED" Then
            MsgBox("This cheque is already marked as RELEASED.")
        ElseIf StatusTextBox.Text = "RETURNED" Then
            MsgBox("This is a Return Chaque ")
        ElseIf Chq_NoTextBox.Text = "" Then
            MsgBox("Please select a cheque first.")
        Else
            Dim result As DialogResult = MessageBox.Show("Are you Sure you want to make this chaque as RELEASED and update balances?", "Confirm Release", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If result = DialogResult.Yes Then
                Try
                    Dim amount As Double = 0
                    If Not Double.TryParse(AmountTextBox.Text, amount) Then
                        MessageBox.Show("Invalid amount format.")
                        Return
                    End If

                    StatusTextBox.Text = "RELEASED"
                    MySqlConn.Open()

                    ' 1. Update status in chaque_issue (ALL records for this physical cheque)
                    Dim updateChqQuery As String = "UPDATE chaque_issue SET status='RELEASED' WHERE chq_no=@chq AND bank_id=@bid"
                    COMMAND = New MySqlCommand(updateChqQuery, MySqlConn)
                    COMMAND.Parameters.AddWithValue("@chq", Chq_NoTextBox.Text)
                    COMMAND.Parameters.AddWithValue("@bid", selectedBankId)
                    COMMAND.ExecuteNonQuery()

                    ' 2. Fetch ALL individual records for this physical cheque to update balances
                    Dim chqRecords As New DataTable()
                    Dim fetchQuery As String = "SELECT inv_no, amount FROM chaque_issue WHERE chq_no = @chq AND bank_id = @bid"
                    Using fetchCmd As New MySqlCommand(fetchQuery, MySqlConn)
                        fetchCmd.Parameters.AddWithValue("@chq", Chq_NoTextBox.Text)
                        fetchCmd.Parameters.AddWithValue("@bid", selectedBankId)
                        Using adapter As New MySqlDataAdapter(fetchCmd)
                            adapter.Fill(chqRecords)
                        End Using
                    End Using

                    ' 3. Process each individual invoice associated with this physical cheque
                    For Each row As DataRow In chqRecords.Rows
                        Dim rawInv As String = row("inv_no").ToString()
                        Dim rawAmount As Double = 0
                        If Not Double.TryParse(row("amount").ToString(), rawAmount) Then rawAmount = 0

                        ' Handle potential combined records (backward compatibility)
                        Dim individualInvs() As String = rawInv.Split(New String() {", "}, StringSplitOptions.RemoveEmptyEntries)
                        
                        For Each targetInv As String In individualInvs
                            targetInv = targetInv.Trim()
                            If String.IsNullOrEmpty(targetInv) Then Continue For

                            ' If it was a combined record, we must look up the EXACT amount for this invoice from source
                            Dim rowAmount As Double = rawAmount
                            If individualInvs.Length > 1 Then
                                rowAmount = 0
                                ' Look up amount from source
                                Dim purQuery As String = "SELECT cheque_balance_due FROM purchasing p JOIN supplier s ON p.supplier_id = s.id WHERE TRIM(p.pur_id) = @pid AND s.name = @name"
                                Using purCmd As New MySqlCommand(purQuery, MySqlConn)
                                    purCmd.Parameters.AddWithValue("@pid", targetInv)
                                    purCmd.Parameters.AddWithValue("@name", C_NameTextBox.Text)
                                    Dim res = purCmd.ExecuteScalar()
                                    If res IsNot Nothing AndAlso Not IsDBNull(res) Then
                                        rowAmount = Convert.ToDouble(res)
                                    End If
                                End Using

                                If rowAmount <= 0 Then
                                    Dim payQuery As String = "SELECT amount FROM supplier_payments WHERE TRIM(inv_no) = @inv AND TRIM(chq_no) = @chq AND bank_id = @bid"
                                    Using payCmd As New MySqlCommand(payQuery, MySqlConn)
                                        payCmd.Parameters.AddWithValue("@inv", targetInv)
                                        payCmd.Parameters.AddWithValue("@chq", Chq_NoTextBox.Text)
                                        payCmd.Parameters.AddWithValue("@bid", selectedBankId)
                                        Dim res = payCmd.ExecuteScalar()
                                        If res IsNot Nothing AndAlso Not IsDBNull(res) Then
                                            rowAmount = Convert.ToDouble(res)
                                        End If
                                    End Using
                                End If
                                
                                ' If still 0, we have an issue, but let's try to proceed with rawAmount/count as absolute fallback (not ideal)
                                If rowAmount <= 0 Then rowAmount = rawAmount / individualInvs.Length
                            End If

                            ' A. Identify source of cheque (Payment vs Purchase)
                            Dim isPaymentCheque As Boolean = False
                            Dim checkSourceQuery As String = "SELECT COUNT(*) FROM supplier_payments WHERE TRIM(chq_no) = TRIM(@chq) AND TRIM(inv_no) = TRIM(@inv)"
                            COMMAND = New MySqlCommand(checkSourceQuery, MySqlConn)
                            COMMAND.Parameters.AddWithValue("@chq", Chq_NoTextBox.Text)
                            COMMAND.Parameters.AddWithValue("@inv", targetInv)
                            isPaymentCheque = (Convert.ToInt32(COMMAND.ExecuteScalar()) > 0)

                            ' Only update purchasing balance if NOT a payment cheque (payment cheques are updated immediately on receipt)
                            If Not isPaymentCheque Then
                                Dim purUpdateQuery As String = "UPDATE purchasing p JOIN supplier s ON p.supplier_id = s.id SET p.balance_due = p.balance_due - @amt, p.cheque_balance_due = p.cheque_balance_due - @amt, p.paid_amount = p.paid_amount + @amt WHERE TRIM(p.pur_id) = TRIM(@pid) AND TRIM(s.name) = TRIM(@name)"
                                COMMAND = New MySqlCommand(purUpdateQuery, MySqlConn)
                                COMMAND.Parameters.AddWithValue("@amt", rowAmount)
                                COMMAND.Parameters.AddWithValue("@pid", targetInv)
                                COMMAND.Parameters.AddWithValue("@name", C_NameTextBox.Text)
                                COMMAND.ExecuteNonQuery()
                            End If

                            ' B. Unified Status Update Query
                            Dim statusUpdateQuery As String = "UPDATE purchasing p JOIN supplier s ON p.supplier_id = s.id SET p.status = CASE WHEN p.balance_due <= 0 THEN 'success' WHEN p.credit_balance_due > 0 AND p.cheque_balance_due > 0 AND p.paid_amount > 0 THEN 'Mixed_Payment' WHEN p.credit_balance_due > 0 AND p.cheque_balance_due > 0 THEN 'Credit_Cheque' WHEN p.credit_balance_due > 0 AND p.paid_amount > 0 THEN 'cash_Credit' WHEN p.credit_balance_due > 0 THEN 'Credit' WHEN p.cheque_balance_due > 0 AND p.paid_amount > 0 THEN 'Cash_Cheque' WHEN p.cheque_balance_due > 0 THEN 'Cheque' ELSE p.status END WHERE TRIM(p.pur_id) = TRIM(@pid) AND TRIM(s.name) = TRIM(@name)"
                            COMMAND = New MySqlCommand(statusUpdateQuery, MySqlConn)
                            COMMAND.Parameters.AddWithValue("@pid", targetInv)
                            COMMAND.Parameters.AddWithValue("@name", C_NameTextBox.Text)
                            COMMAND.ExecuteNonQuery()

                            ' C. Update supplicer_credit table (Bypassed since payment cheques are immediately updated on receipt)
                        Next
                    Next


                    MySqlConn.Close()
                    load_chaque()
                    MessageBox.Show("Cheque marked as RELEASED and balances updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Catch ex As Exception
                    MessageBox.Show("Update Error: " & ex.Message)
                    If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
                End Try
            End If
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        'This is the Return button'
        If StatusTextBox.Text = "RETURNED" Then
            MsgBox("This cheque is already marked as RETURNED.")
        ElseIf StatusTextBox.Text = "RELEASED" Then
            MsgBox("This is a Released Chaque ")
        ElseIf Chq_NoTextBox.Text = "" Then
            MsgBox("Please select a cheque first.")
        Else
            ReturnReasonPanel.Visible = True
            ReturnReasonPanel.BringToFront()
            ReturnReasonTextBox.Clear()
            ReturnReasonTextBox.Focus()
        End If
    End Sub

    Private Sub SubmitReturnButton_Click(sender As Object, e As EventArgs) Handles SubmitReturnButton.Click
        If ReturnReasonTextBox.Text.Trim() = "" Then
            MsgBox("Please enter a return reason.")
            ReturnReasonTextBox.Focus()
            Return
        End If

        Dim result As DialogResult = MessageBox.Show("Are you Sure you want make this chaque as Return", "OR Not", MessageBoxButtons.YesNo)
        If result = DialogResult.Yes Then
            StatusTextBox.Text = "RETURNED"

            Try
                If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()

                ' 1. Insert into chaque_return_reason
                Dim returnDate As String = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                Dim issueDate As String = CDate(Issue_dateDateTimePicker.Value).ToString("yyyy-MM-dd")
                Dim closeDate As String = CDate(Close_dateDateTimePicker.Value).ToString("yyyy-MM-dd")

                Dim insertQuery As String = "INSERT INTO chaque_return_reason (check_number, check_name, bank_id, amount, return_reason, return_date, issue_date, check_release_date, inv_no) VALUES (@chq, @name, @bid, @amt, @reason, @rdate, @idate, @cdate, @inv)"

                COMMAND = New MySqlCommand(insertQuery, MySqlConn)
                COMMAND.Parameters.AddWithValue("@chq", Chq_NoTextBox.Text)
                COMMAND.Parameters.AddWithValue("@name", C_NameTextBox.Text)
                COMMAND.Parameters.AddWithValue("@bid", selectedBankId)
                COMMAND.Parameters.AddWithValue("@amt", AmountTextBox.Text)
                COMMAND.Parameters.AddWithValue("@reason", ReturnReasonTextBox.Text)
                COMMAND.Parameters.AddWithValue("@rdate", returnDate)
                COMMAND.Parameters.AddWithValue("@idate", issueDate)
                COMMAND.Parameters.AddWithValue("@cdate", closeDate)
                COMMAND.Parameters.AddWithValue("@inv", Inv_NoTextBox.Text)

                COMMAND.ExecuteNonQuery()

                ' 2. Update status in chaque_issue (ALL individual records for this physical cheque)
                Dim updateQuery As String = "UPDATE chaque_issue SET status='RETURNED' WHERE chq_no=@chq AND bank_id=@bid"
                COMMAND = New MySqlCommand(updateQuery, MySqlConn)
                COMMAND.Parameters.AddWithValue("@chq", Chq_NoTextBox.Text)
                COMMAND.Parameters.AddWithValue("@bid", selectedBankId)
                COMMAND.ExecuteNonQuery()

                ' 2.1 Reversal logic for payment cheques (Deductions are restored because the cheque returned/bounced)
                Dim chqRecords As New DataTable()
                Dim fetchQuery As String = "SELECT inv_no, amount FROM chaque_issue WHERE chq_no = @chq AND bank_id = @bid"
                Using fetchCmd As New MySqlCommand(fetchQuery, MySqlConn)
                    fetchCmd.Parameters.AddWithValue("@chq", Chq_NoTextBox.Text)
                    fetchCmd.Parameters.AddWithValue("@bid", selectedBankId)
                    Using adapter As New MySqlDataAdapter(fetchCmd)
                        adapter.Fill(chqRecords)
                    End Using
                End Using

                For Each row As DataRow In chqRecords.Rows
                    Dim rawInv As String = row("inv_no").ToString()
                    Dim rawAmount As Double = 0
                    If Not Double.TryParse(row("amount").ToString(), rawAmount) Then rawAmount = 0

                    Dim individualInvs() As String = rawInv.Split(New String() {", "}, StringSplitOptions.RemoveEmptyEntries)
                    For Each targetInv As String In individualInvs
                        targetInv = targetInv.Trim()
                        If String.IsNullOrEmpty(targetInv) Then Continue For

                        Dim rowAmount As Double = rawAmount
                        If individualInvs.Length > 1 Then
                            rowAmount = 0
                            Dim purQuery As String = "SELECT cheque_balance_due FROM purchasing p JOIN supplier s ON p.supplier_id = s.id WHERE TRIM(p.pur_id) = @pid AND s.name = @name"
                            Using purCmd As New MySqlCommand(purQuery, MySqlConn)
                                purCmd.Parameters.AddWithValue("@pid", targetInv)
                                purCmd.Parameters.AddWithValue("@name", C_NameTextBox.Text)
                                Dim res = purCmd.ExecuteScalar()
                                If res IsNot Nothing AndAlso Not IsDBNull(res) Then
                                    rowAmount = Convert.ToDouble(res)
                                End If
                            End Using

                            If rowAmount <= 0 Then
                                Dim payQuery As String = "SELECT amount FROM supplier_payments WHERE TRIM(inv_no) = @inv AND TRIM(chq_no) = @chq AND bank_id = @bid"
                                Using payCmd As New MySqlCommand(payQuery, MySqlConn)
                                    payCmd.Parameters.AddWithValue("@inv", targetInv)
                                    payCmd.Parameters.AddWithValue("@chq", Chq_NoTextBox.Text)
                                    payCmd.Parameters.AddWithValue("@bid", selectedBankId)
                                    Dim res = payCmd.ExecuteScalar()
                                    If res IsNot Nothing AndAlso Not IsDBNull(res) Then
                                        rowAmount = Convert.ToDouble(res)
                                    End If
                                End Using
                            End If
                            
                            If rowAmount <= 0 Then rowAmount = rawAmount / individualInvs.Length
                        End If

                        Dim isPaymentCheque As Boolean = False
                        Dim checkSourceQuery As String = "SELECT COUNT(*) FROM supplier_payments WHERE TRIM(chq_no) = TRIM(@chq) AND TRIM(inv_no) = TRIM(@inv)"
                        COMMAND = New MySqlCommand(checkSourceQuery, MySqlConn)
                        COMMAND.Parameters.AddWithValue("@chq", Chq_NoTextBox.Text)
                        COMMAND.Parameters.AddWithValue("@inv", targetInv)
                        isPaymentCheque = (Convert.ToInt32(COMMAND.ExecuteScalar()) > 0)

                        If isPaymentCheque Then
                            ' 1. Reverse updates in purchasing table
                            Dim purReverseQuery As String = "UPDATE purchasing p JOIN supplier s ON p.supplier_id = s.id SET p.balance_due = p.balance_due + @amt, p.credit_balance_due = p.credit_balance_due + @amt, p.paid_amount = p.paid_amount - @amt WHERE TRIM(p.pur_id) = TRIM(@pid) AND TRIM(s.name) = TRIM(@name)"
                            COMMAND = New MySqlCommand(purReverseQuery, MySqlConn)
                            COMMAND.Parameters.AddWithValue("@amt", rowAmount)
                            COMMAND.Parameters.AddWithValue("@pid", targetInv)
                            COMMAND.Parameters.AddWithValue("@name", C_NameTextBox.Text)
                            COMMAND.ExecuteNonQuery()

                            ' 2. Recalculate status
                            Dim statusRevQuery As String = "UPDATE purchasing p JOIN supplier s ON p.supplier_id = s.id SET p.status = CASE WHEN p.balance_due <= 0 THEN 'success' WHEN p.credit_balance_due > 0 AND p.cheque_balance_due > 0 AND p.paid_amount > 0 THEN 'Mixed_Payment' WHEN p.credit_balance_due > 0 AND p.cheque_balance_due > 0 THEN 'Credit_Cheque' WHEN p.credit_balance_due > 0 AND p.paid_amount > 0 THEN 'cash_Credit' WHEN p.credit_balance_due > 0 THEN 'Credit' WHEN p.cheque_balance_due > 0 AND p.paid_amount > 0 THEN 'Cash_Cheque' WHEN p.cheque_balance_due > 0 THEN 'Cheque' ELSE p.status END WHERE TRIM(p.pur_id) = TRIM(@pid) AND TRIM(s.name) = TRIM(@name)"
                            COMMAND = New MySqlCommand(statusRevQuery, MySqlConn)
                            COMMAND.Parameters.AddWithValue("@pid", targetInv)
                            COMMAND.Parameters.AddWithValue("@name", C_NameTextBox.Text)
                            COMMAND.ExecuteNonQuery()

                            ' 3. Reverse updates in supplicer_credit
                            Dim creditReverseQuery As String = "UPDATE supplicer_credit SET amount = amount + @amt WHERE TRIM(inv_no) = TRIM(@inv) AND TRIM(sname) = TRIM(@name)"
                            COMMAND = New MySqlCommand(creditReverseQuery, MySqlConn)
                            COMMAND.Parameters.AddWithValue("@amt", rowAmount)
                            COMMAND.Parameters.AddWithValue("@inv", targetInv)
                            COMMAND.Parameters.AddWithValue("@name", C_NameTextBox.Text)
                            Dim affectedRows = COMMAND.ExecuteNonQuery()

                            ' Fallback
                            If affectedRows = 0 Then
                                Dim fallbackQuery As String = "UPDATE supplicer_credit SET amount = amount + @amt WHERE TRIM(sname) = TRIM(@name) ORDER BY getdate DESC LIMIT 1"
                                COMMAND = New MySqlCommand(fallbackQuery, MySqlConn)
                                COMMAND.Parameters.AddWithValue("@amt", rowAmount)
                                COMMAND.Parameters.AddWithValue("@name", C_NameTextBox.Text)
                                COMMAND.ExecuteNonQuery()
                            End If
                        End If
                    Next
                Next

                MySqlConn.Close()

                ReturnReasonPanel.Visible = False
                load_chaque()
                MsgBox("Cheque marked as RETURNED successfully.")
            Catch ex As Exception
                MessageBox.Show(ex.Message)
                If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
            End Try
        End If
    End Sub

    Private Sub ReturnReasonTextBox_KeyDown(sender As Object, e As KeyEventArgs) Handles ReturnReasonTextBox.KeyDown
        If e.KeyCode = Keys.Enter Then
            If Not e.Shift Then
                e.SuppressKeyPress = True
                SubmitReturnButton.PerformClick()
            End If
        End If
    End Sub

    Private Sub CancelReturnButton_Click(sender As Object, e As EventArgs) Handles CancelReturnButton.Click
        ReturnReasonPanel.Visible = False
    End Sub

    Private Sub Chq_NoTextBox_KeyDown(sender As Object, e As KeyEventArgs) Handles Chq_NoTextBox.KeyDown
        If e.KeyCode = Keys.Enter Then
            BankTextBox.Select()
        End If
    End Sub

    Private Sub TextBox2_TextChanged(sender As Object, e As EventArgs) Handles TextBox2.TextChanged
        load_chaque()
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        load_chaque()
    End Sub

    Private Sub UpdateActionButtonsVisibility()
        If StatusTextBox.Text = "PENDING" Then
            Button1.Visible = True
            Button2.Visible = True
        Else
            Button1.Visible = False
            Button2.Visible = False
        End If
    End Sub


    Private Sub ChaquereceivedDataGridView_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles ChaquereceivedDataGridView.CellClick
        UpdateFieldsFromGrid()
    End Sub

    Private Sub ChaquereceivedDataGridView_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles ChaquereceivedDataGridView.CellContentClick
        UpdateFieldsFromGrid()
    End Sub

    Private Sub ChaquereceivedDataGridView_KeyDown(sender As Object, e As KeyEventArgs) Handles ChaquereceivedDataGridView.KeyDown
        If e.KeyCode = Keys.Enter Then
            If ChaquereceivedDataGridView.CurrentRow IsNot Nothing Then
                UpdateFieldsFromGrid()
                e.Handled = True
                e.SuppressKeyPress = True
            End If
        End If
    End Sub

    Private Sub UpdateFieldsFromGrid()
        Try
            If ChaquereceivedDataGridView.CurrentRow IsNot Nothing Then
                isSelecting = True
                Dim k As Integer = ChaquereceivedDataGridView.CurrentRow.Index
                Chq_NoTextBox.Text = ChaquereceivedDataGridView.Rows(k).Cells(0).Value.ToString()
                C_NameTextBox.Text = ChaquereceivedDataGridView.Rows(k).Cells(1).Value.ToString()
                BankTextBox.Text = ChaquereceivedDataGridView.Rows(k).Cells(2).Value.ToString()
                AmountTextBox.Text = ChaquereceivedDataGridView.Rows(k).Cells(3).Value.ToString()
                StatusTextBox.Text = ChaquereceivedDataGridView.Rows(k).Cells(4).Value.ToString()
                Issue_dateDateTimePicker.Value = ChaquereceivedDataGridView.Rows(k).Cells(5).Value.ToString()
                Close_dateDateTimePicker.Value = ChaquereceivedDataGridView.Rows(k).Cells(6).Value.ToString()
                If ChaquereceivedDataGridView.Columns.Count > 7 Then
                    Inv_NoTextBox.Text = ChaquereceivedDataGridView.Rows(k).Cells(7).Value.ToString()
                End If
                If ChaquereceivedDataGridView.Columns.Count > 9 Then
                    Dim bidVal As Object = ChaquereceivedDataGridView.Rows(k).Cells(9).Value
                    If bidVal IsNot Nothing AndAlso Not IsDBNull(bidVal) Then
                        Integer.TryParse(bidVal.ToString(), selectedBankId)
                    Else
                        selectedBankId = 0
                    End If
                End If

                ' Store original details for update operations
                originalChqNo = ChaquereceivedDataGridView.Rows(k).Cells(0).Value.ToString().Trim()
                If ChaquereceivedDataGridView.Columns.Count > 9 Then
                    Dim bidVal As Object = ChaquereceivedDataGridView.Rows(k).Cells(9).Value
                    If bidVal IsNot Nothing AndAlso Not IsDBNull(bidVal) Then
                        Integer.TryParse(bidVal.ToString(), originalBankId)
                    Else
                        originalBankId = 0
                    End If
                Else
                    originalBankId = 0
                End If
                originalInvNo = If(ChaquereceivedDataGridView.Columns.Count > 7 AndAlso ChaquereceivedDataGridView.Rows(k).Cells(7).Value IsNot Nothing, ChaquereceivedDataGridView.Rows(k).Cells(7).Value.ToString().Trim(), "")
                originalAmount = Convert.ToDecimal(ChaquereceivedDataGridView.Rows(k).Cells(3).Value)

                ' Mark as existing record and enable save as "Update"
                isExistingRecord = True
                savebtn.Text = "Update"
                savebtn.Enabled = True
                Panel1.Visible = False

                UpdateActionButtonsVisibility()
                isSelecting = False
            End If
        Catch ex As Exception
            isSelecting = False
        End Try
    End Sub

    Private Sub TextBox5_TextChanged(sender As Object, e As EventArgs) Handles TextBox5.TextChanged
        load_chaque()
    End Sub

    Private Sub TextBox6_TextChanged(sender As Object, e As EventArgs) Handles TextBox6.TextChanged
        load_chaque()
    End Sub

    Private Sub gettotandcolor()
        Dim sss As Double = 0
        For s As Integer = 0 To ChaquereceivedDataGridView.Rows.Count - 1 Step +1

            sss = sss + ChaquereceivedDataGridView.Rows(s).Cells(3).Value

            If ChaquereceivedDataGridView.Rows(s).Cells(4).Value = "PENDING" Then
                ChaquereceivedDataGridView.Rows(s).DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 128)
            End If
            If ChaquereceivedDataGridView.Rows(s).Cells(4).Value = "RETURNED" Then
                ChaquereceivedDataGridView.Rows(s).DefaultCellStyle.BackColor = Color.Red
            End If
            If ChaquereceivedDataGridView.Rows(s).Cells(4).Value = "RELEASED" Then
                ChaquereceivedDataGridView.Rows(s).DefaultCellStyle.BackColor = Color.YellowGreen
            End If
        Next
        Label18.Text = "Total Amount: " & sss.ToString("N2")
    End Sub


    Private Sub DateTimePicker1_ValueChanged(sender As Object, e As EventArgs) Handles DateTimePicker1.ValueChanged
        load_chaque()
    End Sub

    Private Sub DateTimePicker2_ValueChanged(sender As Object, e As EventArgs) Handles DateTimePicker2.ValueChanged
        load_chaque()
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        TextBox2.Text = "RETURNED"
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        TextBox2.Text = "RELEASED"
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        TextBox2.Text = "PENDING"
    End Sub

    Private Sub btndebit_Click(sender As Object, e As EventArgs) Handles btndebit.Click
        ' Open the Debit Entry form in the main container if hosted by Start
        If Me.MdiParent IsNot Nothing AndAlso TypeOf Me.MdiParent Is Start Then
            Dim startForm As Start = DirectCast(Me.MdiParent, Start)
            
            ' Pass the selected supplier name to the Debit Entry search field
            DebitEntry.NameTextBox1.Text = C_NameTextBox.Text
            
            ' Open the form using the standard MDI helper
            startForm.OpenMdiForm(DebitEntry)
            
            ' Ensure TabPage3 (Supplier Payments) is selected
            DebitEntry.TabControl1.SelectedTab = DebitEntry.TabPage3
        Else
            ' Fallback for standalone or non-MDI mode
            DebitEntry.NameTextBox1.Text = C_NameTextBox.Text
            DebitEntry.Show()
            DebitEntry.BringToFront()
            DebitEntry.Focus()
        End If
    End Sub

    Private Sub ChaquereceivedDataGridView_ColumnHeaderMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles ChaquereceivedDataGridView.ColumnHeaderMouseClick
        For s As Integer = 0 To ChaquereceivedDataGridView.Rows.Count - 1 Step +1
            If ChaquereceivedDataGridView.Rows(s).Cells(4).Value = "PENDING" Then
                ChaquereceivedDataGridView.Rows(s).DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 128)
            End If
            If ChaquereceivedDataGridView.Rows(s).Cells(4).Value = "RETURNED" Then
                ChaquereceivedDataGridView.Rows(s).DefaultCellStyle.BackColor = Color.Red
            End If
            If ChaquereceivedDataGridView.Rows(s).Cells(4).Value = "RELEASED" Then
                ChaquereceivedDataGridView.Rows(s).DefaultCellStyle.BackColor = Color.YellowGreen
            End If
        Next
        UpdateActionButtonsVisibility()
    End Sub


    Private Sub Close_dateDateTimePicker_KeyDown(sender As Object, e As KeyEventArgs) Handles Close_dateDateTimePicker.KeyDown
        If e.KeyCode = Keys.Enter Then
            savebtn.PerformClick()
        End If
    End Sub

    Private Sub AmountTextBox_KeyDown(sender As Object, e As KeyEventArgs) Handles AmountTextBox.KeyDown
        If e.KeyCode = Keys.Enter Then
            Close_dateDateTimePicker.Select()
        End If
    End Sub

    Private Sub Panel1_Paint(sender As Object, e As PaintEventArgs) Handles Panel1.Paint

    End Sub

    Private Sub StatusTextBox_TextChanged(sender As Object, e As EventArgs) Handles StatusTextBox.TextChanged

    End Sub

    Private Sub ComboBox3_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox3.SelectedIndexChanged

    End Sub

    Private Sub ComboBox3_KeyDown(sender As Object, e As KeyEventArgs) Handles ComboBox3.KeyDown
        If e.KeyCode = Keys.Enter Then
            secure_key.Select()
        End If
    End Sub

    Private Sub secure_key_KeyDown(sender As Object, e As KeyEventArgs) Handles secure_key.KeyDown
        If e.KeyCode = Keys.Enter Then
            deletebtn.PerformClick()
        End If
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        DateTimePicker1.Enabled = Not CheckBox1.Checked
        DateTimePicker2.Enabled = Not CheckBox1.Checked
        load_chaque()
    End Sub

    Private Sub LoadSupplierPurchases(supplierId As Integer)
        Try
            If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()
            Dim table As New DataTable()
            ' [MODIFIED] Grouping logic integrated into the suggestion grid load to match user image
            Dim query As String = "SELECT " &
                                 "GROUP_CONCAT(t.pur_id SEPARATOR ', ') as inv_no, " &
                                 "SUM(t.amount) as amount, " &
                                 "t.chq_no, " &
                                 "MAX(t.date) as date, " &
                                 "MAX(t.source) as source, " &
                                 "b.bank_name as bank, " &
                                 "t.bank_id " &
                                 "FROM ( " &
                                 "  SELECT p.pur_id, p.cheque_balance_due as amount, 'Purchase' as source, IFNULL(p.cqe_no, '') as chq_no, p.bank_id, p.pur_date as date " &
                                 "  FROM purchasing p " &
                                 "  LEFT JOIN chaque_issue ci ON (ci.amount = p.cheque_balance_due AND TRIM(ci.chq_no) = IF(TRIM(p.cqe_no) = '' OR p.cqe_no IS NULL, TRIM(p.pur_id), TRIM(p.cqe_no))) " &
                                 "  WHERE p.supplier_id = @sid AND p.cheque_balance_due > 0 " &
                                 "  AND (p.status IN ('Cheque' ,'%Cheque%','Chaque', 'cash_Cheque', 'Cash_Cheque', 'Mixed_Payment', 'Credit_Cheque')) " &
                                 "  AND (ci.id IS NULL OR ci.status = 'PENDING') " &
                                 "  UNION ALL " &
                                 "  SELECT IFNULL(p.inv_no, '') as pur_id, p.amount, 'Payment' as source, p.chq_no, p.bank_id, p.pdate as date " &
                                 "  FROM supplier_payments p " &
                                 "  LEFT JOIN chaque_issue ci ON (TRIM(p.chq_no) = TRIM(ci.chq_no) AND p.amount = ci.amount) " &
                                 "  WHERE p.supplier_id = @sid AND (p.type LIKE '%Cheque%' OR p.type LIKE 'Chaque') " &
                                 "  AND (ci.id IS NULL OR ci.status = 'PENDING') " &
                                 ") t " &
                                 "LEFT JOIN bank b ON t.bank_id = b.id " &
                                 "GROUP BY t.chq_no, t.bank_id " &
                                 "ORDER BY date DESC"

            Dim adapter As New MySqlDataAdapter(query, MySqlConn)
            adapter.SelectCommand.Parameters.AddWithValue("@sid", supplierId)
            adapter.Fill(table)
            MySqlConn.Close()

            If table.Rows.Count > 0 Then
                PurchaseDataGridView.DataSource = table
                
                ' Configure headers to match user provided image
                PurchaseDataGridView.Columns(0).HeaderText = "Inv No"
                PurchaseDataGridView.Columns(1).HeaderText = "Amount"
                PurchaseDataGridView.Columns(2).HeaderText = "Cheque No"
                PurchaseDataGridView.Columns(3).HeaderText = "Date"
                PurchaseDataGridView.Columns(4).HeaderText = "Source"
                PurchaseDataGridView.Columns(5).HeaderText = "Bank"

                PurchaseDataGridView.Columns(1).DefaultCellStyle.Format = "N2"
                PurchaseDataGridView.Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                PurchaseDataGridView.Columns(3).DefaultCellStyle.Format = "yyyy-MM-dd HH:mm"

                ' Hide bank_id column
                If PurchaseDataGridView.Columns.Count > 6 Then
                    PurchaseDataGridView.Columns(6).Visible = False
                End If

                PurchaseDataGridView.AllowUserToAddRows = False
                PurchaseDataGridView.ReadOnly = True
                PurchaseDataGridView.MultiSelect = False
                PurchaseDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect
                PurchaseDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                
                ' Expand font size and row height
                PurchaseDataGridView.DefaultCellStyle.Font = New Font("Segoe UI", 11)
                PurchaseDataGridView.DefaultCellStyle.ForeColor = Color.Black
                PurchaseDataGridView.GridColor = Color.Black
                PurchaseDataGridView.RowTemplate.Height = 32
                PurchaseDataGridView.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 11, FontStyle.Bold)
                PurchaseDataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black
                
                For Each row As DataGridViewRow In PurchaseDataGridView.Rows
                    row.Height = 32
                Next

                PurchaseDetailPanel.Visible = True
                PurchaseDetailPanel.BringToFront()
            Else
                PurchaseDetailPanel.Visible = False
            End If
        Catch ex As Exception
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Sub

    Private Sub ClosePurchasePanelBtn_Click(sender As Object, e As EventArgs) Handles ClosePurchasePanelBtn.Click
        PurchaseDetailPanel.Visible = False
    End Sub

    Private Sub PurchaseDataGridView_KeyDown(sender As Object, e As KeyEventArgs) Handles PurchaseDataGridView.KeyDown
        If e.KeyCode = Keys.Enter Then
            If PurchaseDataGridView.CurrentRow IsNot Nothing Then
                e.SuppressKeyPress = True
                ' Extract values from selected purchase record - Use raw Value where possible
                Dim selectedRef As String = PurchaseDataGridView.CurrentRow.Cells(0).Value.ToString()
                Dim selectedAmountRaw As Object = PurchaseDataGridView.CurrentRow.Cells(1).Value
                Dim selectedAmountStr As String = If(selectedAmountRaw IsNot Nothing, selectedAmountRaw.ToString(), "0")
                Dim selectedChq As String = PurchaseDataGridView.CurrentRow.Cells(2).Value.ToString()
                Dim selectedBankName As String = PurchaseDataGridView.CurrentRow.Cells(5).Value.ToString()
                Dim selectedBankIdVal As Object = PurchaseDataGridView.CurrentRow.Cells(6).Value
                
                Inv_NoTextBox.Text = selectedRef ' Store the pur_id/inv_no here
                Chq_NoTextBox.Text = If(String.IsNullOrEmpty(selectedChq), selectedRef, selectedChq) ' Prefer chq_no if available, fallback to ref
                AmountTextBox.Text = selectedAmountStr
                BankTextBox.Text = selectedBankName

                If selectedBankIdVal IsNot Nothing AndAlso Not IsDBNull(selectedBankIdVal) Then
                    Integer.TryParse(selectedBankIdVal.ToString(), selectedBankId)
                Else
                    selectedBankId = 0
                End If

                ' Fetch bank name for auto-fill based on selected bank_id
                Try
                    If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()
                    Dim bankNameSql As String = ""
                    If selectedBankId > 0 Then
                        bankNameSql = "SELECT bank_name FROM bank WHERE id = @bid"
                    Else
                        bankNameSql = "SELECT bank_name FROM bank LIMIT 1"
                    End If
                    
                    Dim bankCmd As New MySqlCommand(bankNameSql, MySqlConn)
                    If selectedBankId > 0 Then
                        bankCmd.Parameters.AddWithValue("@bid", selectedBankId)
                    End If
                    
                    Dim bankName = bankCmd.ExecuteScalar()
                    If bankName IsNot Nothing Then
                        BankTextBox.Text = bankName.ToString()
                    End If
                    MySqlConn.Close()
                Catch ex As Exception
                    If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
                End Try

                PurchaseDetailPanel.Visible = False

                ' Focus management
                If Chq_NoTextBox.Text = "" Then
                    Chq_NoTextBox.Select() ' Need to enter cheque number manually
                Else
                    AmountTextBox.Select()
                End If
            End If
        End If
        If e.KeyCode = Keys.Escape Then
            PurchaseDetailPanel.Visible = False
        End If
    End Sub

    Private Sub PurchaseDataGridView_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles PurchaseDataGridView.CellClick
        If e.RowIndex >= 0 Then
            Dim currentRow As DataGridViewRow = PurchaseDataGridView.Rows(e.RowIndex)
            If currentRow IsNot Nothing Then
                ' Extract values from selected purchase record - Use raw Value where possible
                Dim selectedRef As String = currentRow.Cells(0).Value.ToString()
                Dim selectedAmountRaw As Object = currentRow.Cells(1).Value
                Dim selectedAmountStr As String = If(selectedAmountRaw IsNot Nothing, selectedAmountRaw.ToString(), "0")
                Dim selectedChq As String = currentRow.Cells(2).Value.ToString()
                Dim selectedBankName As String = currentRow.Cells(5).Value.ToString()
                Dim selectedBankIdVal As Object = currentRow.Cells(6).Value
                
                Inv_NoTextBox.Text = selectedRef ' Store the pur_id/inv_no here
                Chq_NoTextBox.Text = If(String.IsNullOrEmpty(selectedChq), selectedRef, selectedChq) ' Prefer chq_no if available, fallback to ref
                AmountTextBox.Text = selectedAmountStr
                BankTextBox.Text = selectedBankName

                If selectedBankIdVal IsNot Nothing AndAlso Not IsDBNull(selectedBankIdVal) Then
                    Integer.TryParse(selectedBankIdVal.ToString(), selectedBankId)
                Else
                    selectedBankId = 0
                End If

                ' Fetch bank name for auto-fill based on selected bank_id
                Try
                    If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()
                    Dim bankNameSql As String = ""
                    If selectedBankId > 0 Then
                        bankNameSql = "SELECT bank_name FROM bank WHERE id = @bid"
                    Else
                        bankNameSql = "SELECT bank_name FROM bank LIMIT 1"
                    End If
                    
                    Dim bankCmd As New MySqlCommand(bankNameSql, MySqlConn)
                    If selectedBankId > 0 Then
                        bankCmd.Parameters.AddWithValue("@bid", selectedBankId)
                    End If
                    
                    Dim bankName = bankCmd.ExecuteScalar()
                    If bankName IsNot Nothing Then
                        BankTextBox.Text = bankName.ToString()
                    End If
                    MySqlConn.Close()
                Catch ex As Exception
                    If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
                End Try

                PurchaseDetailPanel.Visible = False

                ' Focus management
                If Chq_NoTextBox.Text = "" Then
                    Chq_NoTextBox.Select() ' Need to enter cheque number manually
                Else
                    AmountTextBox.Select()
                End If
            End If
        End If
    End Sub


    Private Sub C_NameTextBox_Enter(sender As Object, e As EventArgs) Handles C_NameTextBox.Enter
        activeSearchControl = C_NameTextBox
        currentMode = SearchMode.Supplier
    End Sub

    Private Sub BankTextBox_Enter(sender As Object, e As EventArgs) Handles BankTextBox.Enter
        activeSearchControl = BankTextBox
        currentMode = SearchMode.Bank
    End Sub

    Private Sub C_NameTextBox_Leave(sender As Object, e As EventArgs) Handles C_NameTextBox.Leave
        ' Optional cleanup logic
    End Sub
    Private Sub ChaqueOut_Activated(sender As Object, e As EventArgs) Handles MyBase.Activated
        load_chaque()
    End Sub
    Private Sub Print_Click(sender As Object, e As EventArgs) Handles Print.Click
        Try
            Dim rptDoc As New suppliercheque()

            ' Dynamic field identification logic
            Dim mainTable As String = "chaque_issue"
            Dim sNameField As String = "c_name"
            Dim statusField As String = "status"
            Dim chqNoField As String = "chq_no"
            Dim closeDateField As String = "close_date"
            Dim bankTable As String = "bank"
            Dim bankNameField As String = "bank_name"

            ' Try to find exact aliases in the report
            For Each tbl As Table In rptDoc.Database.Tables
                If tbl.Name.ToLower().Contains("chaque_issue") OrElse tbl.Name.Equals("Command", StringComparison.OrdinalIgnoreCase) Then
                    mainTable = tbl.Name
                End If
                If tbl.Name.ToLower().Contains("bank") Then
                    bankTable = tbl.Name
                End If
            Next

            ' Build RecordSelectionFormula based on active filters
            Dim formula As New List(Of String)

            ' 1. Supplier Name (TextBox1)
            If Not String.IsNullOrWhiteSpace(TextBox1.Text) Then
                formula.Add("UpperCase({" & mainTable & "." & sNameField & "}) LIKE '" & TextBox1.Text.Trim().ToUpper().Replace("'", "''") & "*'")
            End If

            ' 2. Status (TextBox2)
            If Not String.IsNullOrWhiteSpace(TextBox2.Text) Then
                formula.Add("UpperCase({" & mainTable & "." & statusField & "}) LIKE '" & TextBox2.Text.Trim().ToUpper().Replace("'", "''") & "*'")
            End If

            ' 3. Bank (TextBox5)
            If Not String.IsNullOrWhiteSpace(TextBox5.Text) AndAlso TextBox5.Text <> "All" Then
                ' If bank table exists and linked, use it, otherwise use mainTable fallback
                Dim bTbl As String = If(rptDoc.Database.Tables.Cast(Of Table)().Any(Function(t) t.Name = bankTable), bankTable, mainTable)
                formula.Add("UpperCase({" & bTbl & "." & bankNameField & "}) LIKE '" & TextBox5.Text.Trim().ToUpper().Replace("'", "''") & "*'")
            End If

            ' 4. Cheque Number (TextBox6)
            If Not String.IsNullOrWhiteSpace(TextBox6.Text) Then
                formula.Add("UpperCase({" & mainTable & "." & chqNoField & "}) LIKE '" & TextBox6.Text.Trim().ToUpper().Replace("'", "''") & "*'")
            End If

            ' 5. Date Filter (DateTimePicker1/2)
            If Not CheckBox1.Checked Then
                Dim startDt As String = DateTimePicker1.Value.ToString("yyyy, MM, dd")
                Dim endDt As String = DateTimePicker2.Value.ToString("yyyy, MM, dd")
                formula.Add("{" & mainTable & "." & closeDateField & "} >= Date(" & startDt & ") AND {" & mainTable & "." & closeDateField & "} <= Date(" & endDt & ")")
            End If

            If formula.Count > 0 Then
                rptDoc.RecordSelectionFormula = String.Join(" AND ", formula)
            End If

            ' Hand over to SaleInv for centralized display/printing
            SaleInv.ShowReport(rptDoc, 8)

        Catch ex As Exception
            MessageBox.Show("Error generating report: " & ex.Message, "Report Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub PositionSuggestionPanel()
        If activeSearchControl IsNot Nothing Then
            Dim ctrlScreenPos As Point = activeSearchControl.PointToScreen(Point.Empty)
            Dim formClientPos As Point = Me.PointToClient(ctrlScreenPos)
            Panel1.Location = New Point(formClientPos.X, formClientPos.Y + activeSearchControl.Height)
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

            Dim query As String = "SELECT u.hiddenSecurekey, r.role_name " &
                                 "FROM user u " &
                                 "INNER JOIN user_role r ON u.role_id = r.id " &
                                 "WHERE u.name = @uname"
            Dim cmd As New MySqlCommand(query, MySqlConn)
            cmd.Parameters.AddWithValue("@uname", ComboBox3.Text)

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

            ' A. Update chaque_issue
            Dim updateCheckSql As String = "UPDATE chaque_issue SET " &
                                          "chq_no = @new_chq, " &
                                          "c_name = @name, " &
                                          "bank_id = @bank, " &
                                          "amount = @amt, " &
                                          "issue_date = @issue, " &
                                          "close_date = @release, " &
                                          "inv_no = @inv " &
                                          "WHERE chq_no = @orig_chq AND bank_id = @orig_bid AND IFNULL(inv_no, '') = IFNULL(@orig_inv, '')"
            Using cmd As New MySqlCommand(updateCheckSql, MySqlConn, transaction)
                cmd.Parameters.AddWithValue("@new_chq", Chq_NoTextBox.Text.Trim())
                cmd.Parameters.AddWithValue("@name", C_NameTextBox.Text.Trim())
                cmd.Parameters.AddWithValue("@bank", bankId)
                cmd.Parameters.AddWithValue("@amt", newAmount)
                cmd.Parameters.AddWithValue("@issue", Issue_dateDateTimePicker.Value.ToString("yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@release", Close_dateDateTimePicker.Value.ToString("yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@inv", Inv_NoTextBox.Text.Trim())
                cmd.Parameters.AddWithValue("@orig_chq", originalChqNo)
                cmd.Parameters.AddWithValue("@orig_bid", originalBankId)
                cmd.Parameters.AddWithValue("@orig_inv", originalInvNo)
                cmd.ExecuteNonQuery()
            End Using

            ' B. Update chaque_return_reason if it exists
            Dim updateRetSql As String = "UPDATE chaque_return_reason SET check_number = @new_chq, bank_id = @bank WHERE check_number = @orig_chq AND bank_id = @orig_bid"
            Using cmd As New MySqlCommand(updateRetSql, MySqlConn, transaction)
                cmd.Parameters.AddWithValue("@new_chq", Chq_NoTextBox.Text.Trim())
                cmd.Parameters.AddWithValue("@bank", bankId)
                cmd.Parameters.AddWithValue("@orig_chq", originalChqNo)
                cmd.Parameters.AddWithValue("@orig_bid", originalBankId)
                cmd.ExecuteNonQuery()
            End Using

            ' C. Check if this is a payment cheque (exists in supplier_payments)
            Dim isPaymentCheque As Boolean = False
            Dim checkPaySql As String = "SELECT COUNT(*) FROM supplier_payments WHERE LOWER(TRIM(chq_no)) = LOWER(TRIM(@orig_chq)) AND bank_id = @orig_bid AND FIND_IN_SET(inv_no, REPLACE(@orig_inv, ', ', ','))"
            Using checkPayCmd As New MySqlCommand(checkPaySql, MySqlConn, transaction)
                checkPayCmd.Parameters.AddWithValue("@orig_chq", originalChqNo)
                checkPayCmd.Parameters.AddWithValue("@orig_bid", originalBankId)
                checkPayCmd.Parameters.AddWithValue("@orig_inv", originalInvNo)
                isPaymentCheque = (Convert.ToInt32(checkPayCmd.ExecuteScalar()) > 0)
            End Using

            Dim individualInvs() As String = originalInvNo.Split(New String() {", "}, StringSplitOptions.RemoveEmptyEntries)

            If isPaymentCheque Then
                ' Update supplier_payments
                Dim updatePaySql As String = "UPDATE supplier_payments SET " &
                                             "chq_no = @new_chq, " &
                                             "bank_id = @bank " &
                                             "WHERE LOWER(TRIM(chq_no)) = LOWER(TRIM(@orig_chq)) AND bank_id = @orig_bid AND FIND_IN_SET(inv_no, REPLACE(@orig_inv, ', ', ','))"
                Using cmd As New MySqlCommand(updatePaySql, MySqlConn, transaction)
                    cmd.Parameters.AddWithValue("@new_chq", Chq_NoTextBox.Text.Trim())
                    cmd.Parameters.AddWithValue("@bank", bankId)
                    cmd.Parameters.AddWithValue("@orig_chq", originalChqNo)
                    cmd.Parameters.AddWithValue("@orig_bid", originalBankId)
                    cmd.Parameters.AddWithValue("@orig_inv", originalInvNo)
                    cmd.ExecuteNonQuery()
                End Using

                ' If amount changed, update financial ledger for payments
                If diff <> 0 Then
                    ' Adjust supplier_payments amount
                    Dim updatePayAmtSql As String = "UPDATE supplier_payments SET amount = amount + @diff " &
                                                    "WHERE LOWER(TRIM(chq_no)) = LOWER(TRIM(@new_chq)) AND bank_id = @bank AND FIND_IN_SET(inv_no, REPLACE(@orig_inv, ', ', ',')) LIMIT 1"
                    Using cmd As New MySqlCommand(updatePayAmtSql, MySqlConn, transaction)
                        cmd.Parameters.AddWithValue("@diff", diff)
                        cmd.Parameters.AddWithValue("@new_chq", Chq_NoTextBox.Text.Trim())
                        cmd.Parameters.AddWithValue("@bank", bankId)
                        cmd.Parameters.AddWithValue("@orig_inv", originalInvNo)
                        cmd.ExecuteNonQuery()
                    End Using

                    ' Update purchasing and supplier_credit
                    For Each targetInv As String In individualInvs
                        targetInv = targetInv.Trim()
                        If String.IsNullOrEmpty(targetInv) Then Continue For

                        ' Calculate diff fraction if split, but simple case: divide diff equally or apply to first
                        Dim divisor As Integer = If(individualInvs.Length > 0, individualInvs.Length, 1)
                        Dim termDiff As Decimal = diff / divisor

                        Dim updatePurSql As String = "UPDATE purchasing p JOIN supplier s ON p.supplier_id = s.id SET " &
                                                     "p.balance_due = p.balance_due - @diff, " &
                                                     "p.credit_balance_due = p.credit_balance_due - @diff, " &
                                                     "p.paid_amount = p.paid_amount + @diff " &
                                                     "WHERE TRIM(p.pur_id) = TRIM(@pid) AND TRIM(s.name) = TRIM(@name)"
                        Using cmd As New MySqlCommand(updatePurSql, MySqlConn, transaction)
                            cmd.Parameters.AddWithValue("@diff", termDiff)
                            cmd.Parameters.AddWithValue("@pid", targetInv)
                            cmd.Parameters.AddWithValue("@name", C_NameTextBox.Text.Trim())
                            cmd.ExecuteNonQuery()
                        End Using

                        Dim updateCreditSql As String = "UPDATE supplicer_credit SET amount = amount - @diff WHERE TRIM(inv_no) = TRIM(@inv) AND TRIM(sname) = TRIM(@name)"
                        Using cmd As New MySqlCommand(updateCreditSql, MySqlConn, transaction)
                            cmd.Parameters.AddWithValue("@diff", termDiff)
                            cmd.Parameters.AddWithValue("@inv", targetInv)
                            cmd.Parameters.AddWithValue("@name", C_NameTextBox.Text.Trim())
                            cmd.ExecuteNonQuery()
                        End Using
                    Next
                End If
            Else
                ' Update purchasing table cqe_no and bank_id
                Dim updatePurSql As String = "UPDATE purchasing p JOIN supplier s ON p.supplier_id = s.id SET " &
                                             "p.cqe_no = @new_chq, " &
                                             "p.bank_id = @bank " &
                                             "WHERE LOWER(TRIM(p.cqe_no)) = LOWER(TRIM(@orig_chq)) AND p.bank_id = @orig_bid AND FIND_IN_SET(p.pur_id, REPLACE(@orig_inv, ', ', ','))"
                Using cmd As New MySqlCommand(updatePurSql, MySqlConn, transaction)
                    cmd.Parameters.AddWithValue("@new_chq", Chq_NoTextBox.Text.Trim())
                    cmd.Parameters.AddWithValue("@bank", bankId)
                    cmd.Parameters.AddWithValue("@orig_chq", originalChqNo)
                    cmd.Parameters.AddWithValue("@orig_bid", originalBankId)
                    cmd.Parameters.AddWithValue("@orig_inv", originalInvNo)
                    cmd.ExecuteNonQuery()
                End Using

                ' If amount changed, update purchasing balances for Billing Cheques
                If diff <> 0 Then
                    For Each targetInv As String In individualInvs
                        targetInv = targetInv.Trim()
                        If String.IsNullOrEmpty(targetInv) Then Continue For

                        Dim divisor As Integer = If(individualInvs.Length > 0, individualInvs.Length, 1)
                        Dim termDiff As Decimal = diff / divisor

                        Dim updatePurBalSql As String = "UPDATE purchasing p JOIN supplier s ON p.supplier_id = s.id SET " &
                                                        "p.balance_due = p.balance_due - @diff, " &
                                                        "p.cheque_balance_due = p.cheque_balance_due - @diff, " &
                                                        "p.paid_amount = p.paid_amount + @diff " &
                                                        "WHERE TRIM(p.pur_id) = TRIM(@pid) AND TRIM(s.name) = TRIM(@name)"
                        Using cmd As New MySqlCommand(updatePurBalSql, MySqlConn, transaction)
                            cmd.Parameters.AddWithValue("@diff", termDiff)
                            cmd.Parameters.AddWithValue("@pid", targetInv)
                            cmd.Parameters.AddWithValue("@name", C_NameTextBox.Text.Trim())
                            cmd.ExecuteNonQuery()
                        End Using
                    Next
                End If
            End If

            transaction.Commit()
            If MySqlConn.State <> ConnectionState.Closed Then MySqlConn.Close()
            MessageBox.Show("Supplier cheque details updated successfully.", "Success")

            ' Reset UI state
            Chq_NoTextBox.Clear()
            C_NameTextBox.Clear()
            BankTextBox.Clear()
            AmountTextBox.Clear()
            Inv_NoTextBox.Clear()
            selectedBankId = 0
            savebtn.Text = "Save"
            isExistingRecord = False
            load_chaque()

        Catch ex As Exception
            Try
                If transaction IsNot Nothing AndAlso transaction.Connection IsNot Nothing Then
                    transaction.Rollback()
                End If
            Catch rollEx As Exception
                ' Ignore rollback exception to ensure the original error is displayed
            End Try
            MessageBox.Show("Error updating supplier cheque: " & ex.Message)
        Finally
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Sub
End Class
