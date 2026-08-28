Imports MySql.Data.MySqlClient
Public Class customer_add
    Private _isInitializing As Boolean = False
    Private Sub customer_add_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.WindowState = FormWindowState.Maximized
        Me.KeyPreview = True
        db_connection()
        ' Initialize ComboBox1 items
        If ComboBox1.Items.Count = 0 Then
            ComboBox1.Items.Add("Wholesale Sale")
            ComboBox1.Items.Add("Retail Sale")
        End If
        RefreshAll()
    End Sub

    Private Sub customer_add_Activated(sender As Object, e As EventArgs) Handles MyBase.Activated
        ' Removed RefreshAll() to prevent typed data from being wiped 
        ' when returning focus to this form after a MessageBox or other popup.
        ' RefreshAll is already called in the Load event.
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
                ComboBox3.SelectedIndex = ComboBox3.FindStringExact(Module1.UserName)
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

    Private Sub LoadVatOptions()
        Try
            If MySqlConn.State <> ConnectionState.Open Then MySqlConn.Open()

            Dim adapter As New MySqlDataAdapter("SELECT id, vat_name FROM vat", MySqlConn)
            Dim dt As New DataTable()
            adapter.Fill(dt)

            ComboBox2.DataSource = dt
            ComboBox2.DisplayMember = "vat_name"
            ComboBox2.ValueMember = "id"
            ComboBox2.SelectedIndex = -1

            MySqlConn.Close()
        Catch ex As Exception
            MessageBox.Show("Error loading VAT options: " & ex.Message)
        Finally
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Sub

    Dim COMMAND As MySqlCommand
    Dim READER As MySqlDataReader

    ' Tracks the active search filter so it can be re-applied after an edit
    Private _filterField As String = ""
    Private _filterValue As String = ""





    Private Sub UpdateTotalCustomerCount()
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()
            Dim totalCmd As New MySqlCommand("SELECT COUNT(*) FROM customer WHERE deleted_at IS NULL", conn)
            Dim realTotal As Integer = Convert.ToInt32(totalCmd.ExecuteScalar())
            cus_tot.Text = realTotal.ToString()
        Catch ex As Exception
            ' Silently fail or log if count fails, don't block main UI
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub load_Customers()
        Try

            If conn.State = ConnectionState.Closed Then conn.Open()

            Dim table As New DataTable()


            Dim query As String = "SELECT c.id, c.name, c.address, c.city, c.tel_no, c.email, c.customer_type, c.timestamps, c.is_block, v.vat_name, c.vat_id, c.credit_limit, c.credit_period " &
                                 "FROM customer c LEFT JOIN vat v ON c.vat_id = v.id WHERE c.deleted_at IS NULL " &
                                 "ORDER BY c.name ASC"

            Dim adapter As New MySqlDataAdapter(query, conn)
            adapter.Fill(table)

            _isInitializing = True
            CustomerDataGridView.DataSource = table
            BeginInvoke(Sub()
                            CustomerDataGridView.CurrentCell = Nothing
                            CustomerDataGridView.ClearSelection()
                            _isInitializing = False
                        End Sub)

        Catch ex As Exception
            MessageBox.Show("Error loading customers: " & ex.Message)
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try

        UpdateTotalCustomerCount()
        FormatCustomerGrid()
    End Sub

    Private Sub FormatCustomerGrid()
        If CustomerDataGridView.Columns.Count > 0 Then
            CustomerDataGridView.RowHeadersVisible = False
            ' Set Font Sizes
            CustomerDataGridView.DefaultCellStyle.Font = New Font("Microsoft Sans Serif", 14)
            CustomerDataGridView.ColumnHeadersDefaultCellStyle.Font = New Font("Microsoft Sans Serif", 16, FontStyle.Bold)

            ' Adjust Row Height for larger font
            CustomerDataGridView.RowTemplate.Height = 35
            For Each row As DataGridViewRow In CustomerDataGridView.Rows
                row.Height = 35
            Next
            CustomerDataGridView.ColumnHeadersHeight = 60
            CustomerDataGridView.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True
            CustomerDataGridView.AllowUserToAddRows = False

            ' Adjust Column Widths to Fit the Form
            CustomerDataGridView.Columns(0).Visible = False ' Hide ID column
            CustomerDataGridView.Columns(1).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            CustomerDataGridView.Columns(1).FillWeight = 250 ' Name
            CustomerDataGridView.Columns(1).HeaderText = "Name"
            CustomerDataGridView.Columns(2).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            CustomerDataGridView.Columns(2).FillWeight = 240 ' Address
            CustomerDataGridView.Columns(2).HeaderText = "Address"
            CustomerDataGridView.Columns(3).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            CustomerDataGridView.Columns(3).FillWeight = 120 ' City
            CustomerDataGridView.Columns(3).HeaderText = "City"
            CustomerDataGridView.Columns(4).Width = 140 ' Tel
            CustomerDataGridView.Columns(4).HeaderText = "Phone No"
            CustomerDataGridView.Columns(5).Visible = False ' Hide Email column

            ' Hide Type and Timestamps from the table
            CustomerDataGridView.Columns(6).Visible = False ' customer_type
            CustomerDataGridView.Columns(7).Visible = False ' timestamps
        End If

        If CustomerDataGridView.Columns.Contains("vat_name") Then
            CustomerDataGridView.Columns("vat_name").Visible = True
            CustomerDataGridView.Columns("vat_name").HeaderText = "VAT"
            CustomerDataGridView.Columns("vat_name").Width = 40
        End If

        If CustomerDataGridView.Columns.Contains("is_block") Then
            CustomerDataGridView.Columns("is_block").Visible = False
        End If
        If CustomerDataGridView.Columns.Contains("vat_id") Then
            CustomerDataGridView.Columns("vat_id").Visible = False
        End If
        If CustomerDataGridView.Columns.Contains("credit_limit") Then
            CustomerDataGridView.Columns("credit_limit").Visible = True
            CustomerDataGridView.Columns("credit_limit").HeaderText = "Credit Limit"
            CustomerDataGridView.Columns("credit_limit").Width = 130
            CustomerDataGridView.Columns("credit_limit").DefaultCellStyle.Format = "N2"
        End If
        If CustomerDataGridView.Columns.Contains("credit_period") Then
            CustomerDataGridView.Columns("credit_period").Visible = True
            CustomerDataGridView.Columns("credit_period").HeaderText = "Credit Period"
            CustomerDataGridView.Columns("credit_period").Width = 130
            ' Ensure the header is fully visible
            CustomerDataGridView.Columns("credit_period").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
        End If
    End Sub

    Private Sub cus_add_Click(sender As Object, e As EventArgs) Handles cus_add.Click
        ClearInputFields()
    End Sub

    Private Sub ser_name_TextChanged(sender As Object, e As EventArgs) Handles ser_name.TextChanged
        _filterField = "name"
        _filterValue = ser_name.Text
        If conn.State = ConnectionState.Closed Then conn.Open()

        Dim table As New DataTable()
        Dim query As String = "SELECT c.id, c.name, c.address, c.city, c.tel_no, c.email, c.customer_type, c.timestamps, c.is_block, v.vat_name, c.vat_id, c.credit_limit, c.credit_period " &
                             "FROM customer c LEFT JOIN vat v ON c.vat_id = v.id WHERE c.deleted_at IS NULL"
        Dim adapter As New MySqlDataAdapter(query, conn)
        adapter.Fill(table)

        Dim dv As New DataView(table)
        dv.RowFilter = String.Format("name Like '{0}%'", ser_name.Text)
        dv.Sort = "name ASC"
        CustomerDataGridView.DataSource = dv
        FormatCustomerGrid()
        conn.Close()
    End Sub

    Private Sub ser_tel_TextChanged(sender As Object, e As EventArgs) Handles ser_tel.TextChanged
        _filterField = "tel_no"
        _filterValue = ser_tel.Text
        If conn.State = ConnectionState.Closed Then conn.Open()

        Dim table As New DataTable()
        Dim query As String = "SELECT c.id, c.name, c.address, c.city, c.tel_no, c.email, c.customer_type, c.timestamps, c.is_block, v.vat_name, c.vat_id, c.credit_limit, c.credit_period " &
                             "FROM customer c LEFT JOIN vat v ON c.vat_id = v.id WHERE c.deleted_at IS NULL"
        Dim adapter As New MySqlDataAdapter(query, conn)
        adapter.Fill(table)

        Dim dv As New DataView(table)
        dv.RowFilter = String.Format("tel_no Like '{0}%'", ser_tel.Text)
        dv.Sort = "tel_no ASC"
        CustomerDataGridView.DataSource = dv
        FormatCustomerGrid()
        conn.Close()
    End Sub

    Private Sub ser_address_TextChanged(sender As Object, e As EventArgs) Handles ser_address.TextChanged
        _filterField = "address"
        _filterValue = ser_address.Text
        If conn.State = ConnectionState.Closed Then conn.Open()

        Dim table As New DataTable()
        Dim query As String = "SELECT c.id, c.name, c.address, c.city, c.tel_no, c.email, c.customer_type, c.timestamps, c.is_block, v.vat_name, c.vat_id, c.credit_limit, c.credit_period " &
                              "FROM customer c LEFT JOIN vat v ON c.vat_id = v.id WHERE c.deleted_at IS NULL"
        Dim adapter As New MySqlDataAdapter(query, conn)
        adapter.Fill(table)

        Dim dv As New DataView(table)
        dv.RowFilter = String.Format("address Like '%{0}%' OR city Like '%{0}%'", ser_address.Text)
        dv.Sort = "name ASC"
        CustomerDataGridView.DataSource = dv
        FormatCustomerGrid()
        conn.Close()
    End Sub

    ''' <summary>Reloads customers and re-applies the active search filter if any.</summary>
    Public Sub load_Customers_Filtered()
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()
            Dim table As New DataTable()
            Dim query As String = "SELECT c.id, c.name, c.address, c.city, c.tel_no, c.email, c.customer_type, c.timestamps, c.is_block, v.vat_name, c.vat_id, c.credit_limit, c.credit_period " &
                                 "FROM customer c LEFT JOIN vat v ON c.vat_id = v.id WHERE c.deleted_at IS NULL"
            Dim adapter As New MySqlDataAdapter(query, conn)
            adapter.Fill(table)

            UpdateTotalCustomerCount()

            _isInitializing = True
            If _filterField <> "" AndAlso _filterValue <> "" Then
                Dim dv As New DataView(table)
                If _filterField = "address" Then
                    dv.RowFilter = String.Format("address Like '%{0}%' OR city Like '%{0}%'", _filterValue)
                Else
                    dv.RowFilter = String.Format("{0} Like '{1}%'", _filterField, _filterValue)
                End If
                dv.Sort = "name ASC"
                CustomerDataGridView.DataSource = dv
            Else
                CustomerDataGridView.DataSource = table
            End If
            BeginInvoke(Sub()
                            CustomerDataGridView.CurrentCell = Nothing
                            CustomerDataGridView.ClearSelection()
                            _isInitializing = False
                        End Sub)
        Catch ex As Exception
            MessageBox.Show("Error loading customers: " & ex.Message)
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
        FormatCustomerGrid()
    End Sub

    Private Sub cus_save_Click(sender As Object, e As EventArgs) Handles cus_save.Click

        If String.IsNullOrWhiteSpace(cus_name.Text) Then
            MessageBox.Show("Customer Name cannot be empty.")
            cus_name.Focus()
            Exit Sub
        End If






        ' Validate Credit Limit
        If String.IsNullOrWhiteSpace(creditlimit.Text) Then
            MessageBox.Show("Credit Limit is a required field.")
            creditlimit.Focus()
            Exit Sub
        End If

        Dim cl As Decimal
        If Not Decimal.TryParse(creditlimit.Text.Trim(), cl) OrElse cl <= 0 Then
            MessageBox.Show("Please enter a valid numeric Credit Limit (greater than 0).")
            creditlimit.Focus()
            Exit Sub
        End If

        ' Validate Credit Period (must be greater than today)
        If DateTimePicker3.Value.Date <= DateTime.Today Then
            MessageBox.Show("Credit Period must be greater than today's date.")
            DateTimePicker3.Focus()
            Exit Sub
        End If

        Try
            MySqlConn.Open()


            Dim Query As String = "INSERT INTO customer (name, address, city, tel_no, email, customer_type, timestamps, is_block, vat_id, created_at, credit_limit, credit_period) " &
                              "VALUES (@name, @address, @city, @tel, @email, @customer_type, @time, @block, @vat, @created_at, @climit, @cperiod)"

            Dim COMMAND As New MySqlCommand(Query, MySqlConn)


            Dim cleanName As String = SanitizeInput(cus_name.Text, True)
            Dim cleanAddress As String = SanitizeInput(cus_address.Text)
            Dim cleanCity As String = SanitizeInput(cus_city.Text, True)
            Dim cleanEmail As String = SanitizeInput(cus_email.Text).ToLower()
            Dim cleanTel As String = SanitizeInput(cus_tel.Text)

            ' Update UI to reflect sanitized values
            cus_name.Text = cleanName
            cus_address.Text = cleanAddress
            cus_city.Text = cleanCity
            cus_email.Text = cleanEmail
            cus_tel.Text = cleanTel

            COMMAND.Parameters.AddWithValue("@name", cleanName)
            COMMAND.Parameters.AddWithValue("@address", cleanAddress)
            COMMAND.Parameters.AddWithValue("@city", cleanCity)
            COMMAND.Parameters.AddWithValue("@tel", cleanTel)
            COMMAND.Parameters.AddWithValue("@email", cleanEmail)
            COMMAND.Parameters.AddWithValue("@customer_type", ComboBox1.Text)
            COMMAND.Parameters.AddWithValue("@time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
            COMMAND.Parameters.AddWithValue("@block", 0)
            COMMAND.Parameters.AddWithValue("@vat", If(ComboBox2.SelectedValue IsNot Nothing, ComboBox2.SelectedValue, DBNull.Value))
            COMMAND.Parameters.AddWithValue("@created_at", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
            COMMAND.Parameters.AddWithValue("@climit", If(String.IsNullOrWhiteSpace(creditlimit.Text), 0, Convert.ToDecimal(creditlimit.Text)))
            COMMAND.Parameters.AddWithValue("@cperiod", DateTimePicker3.Value.ToString("yyyy-MM-dd"))


            COMMAND.ExecuteNonQuery()

            MessageBox.Show("Customer Saved Successfully!")

        Catch ex As Exception
            MessageBox.Show("Database Error: " & ex.Message)
        Finally
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try

        ' Reset UI
        ClearInputFields()
        load_Customers_Filtered()
        BeginInvoke(Sub() cus_name.Focus())
    End Sub

    ' Standard method to clean up input fields
    Private Function SanitizeInput(input As String, Optional capitalizeWords As Boolean = False) As String
        If String.IsNullOrWhiteSpace(input) Then Return ""

        ' Trim excess whitespace and tabs
        Dim cleaned As String = input.Trim()

        ' Remove problematic characters including single/double quotes, slashes, semi-colons
        cleaned = cleaned.Replace("'", "").Replace("""", "").Replace("\", "").Replace(";", "")

        ' Capitalize words if required
        If capitalizeWords AndAlso cleaned.Length > 0 Then
            cleaned = StrConv(cleaned, VbStrConv.ProperCase)
        End If

        ' Optional: consolidate multiple spaces into single space
        cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, "\s+", " ")

        Return cleaned
    End Function

    ' Helper method to clear fields
    Private Sub ClearInputFields()
        cus_name.Clear()
        cus_address.Clear()
        cus_city.Clear()
        cus_tel.Clear()
        cus_email.Clear()
        creditlimit.Clear()
        ComboBox1.SelectedIndex = -1
        ComboBox2.SelectedIndex = -1
        DateTimePicker3.Value = DateTime.Now
        cus_block.Visible = False
        btn_reactivate.Visible = False
        BeginInvoke(Sub() cus_name.Focus())
    End Sub

    Private Sub RefreshAll()
        ' Clear Search fields
        ser_name.Clear()
        ser_tel.Clear()
        ser_address.Clear()
        _filterField = ""
        _filterValue = ""

        ' Clear Input fields
        ClearInputFields()
        secure_key.Clear()

        ' Reload Data
        LoadVatOptions()
        load_Customers()
        LoadUserList()

        ' Clear Credit Limit
        creditlimit.Clear()

        ' Reset Visibility
        cus_block.Visible = False
        btn_reactivate.Visible = False
    End Sub

    Private Sub cus_name_KeyDown(sender As Object, e As KeyEventArgs) Handles cus_name.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                DateTimePicker3.Focus()
            Else
                cus_email.Select()
            End If
            e.SuppressKeyPress = True
        ElseIf e.KeyCode = Keys.Up OrElse e.KeyCode = Keys.Down Then
            If CustomerDataGridView.Rows.Count > 0 Then
                Dim currentIndex As Integer = If(CustomerDataGridView.CurrentRow IsNot Nothing, CustomerDataGridView.CurrentRow.Index, -1)
                Dim newIndex As Integer = currentIndex

                If e.KeyCode = Keys.Down Then
                    newIndex = Math.Min(CustomerDataGridView.Rows.Count - 1, currentIndex + 1)
                ElseIf e.KeyCode = Keys.Up Then
                    newIndex = Math.Max(0, currentIndex - 1)
                End If

                If newIndex <> currentIndex AndAlso newIndex >= 0 Then
                    CustomerDataGridView.CurrentCell = CustomerDataGridView.Rows(newIndex).Cells(1)
                    ' RowEnter will trigger PopulateSelectedCustomer
                    e.Handled = True
                End If
            End If
        End If
    End Sub

    Private Sub cus_email_KeyDown(sender As Object, e As KeyEventArgs) Handles cus_email.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                cus_name.Select()
            Else
                cus_tel.Select()
            End If
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub cus_tel_KeyDown(sender As Object, e As KeyEventArgs) Handles cus_tel.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                cus_email.Select()
            Else
                cus_address.Select()
            End If
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub cus_address_KeyDown(sender As Object, e As KeyEventArgs) Handles cus_address.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                cus_tel.Select()
            Else
                cus_city.Focus()
                cus_city.SelectAll()
            End If
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub cus_city_KeyDown(sender As Object, e As KeyEventArgs) Handles cus_city.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                cus_address.Focus()
                cus_address.SelectAll()
            Else
                ComboBox1.Focus()
                ComboBox1.DroppedDown = True
            End If
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub ComboBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles ComboBox1.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                ComboBox1.DroppedDown = False
                cus_city.Focus()
                cus_city.SelectAll()
            Else
                ComboBox1.DroppedDown = False
                ComboBox2.Focus()
                ComboBox2.DroppedDown = True
            End If
            e.SuppressKeyPress = True
        ElseIf e.KeyCode = Keys.Back Then
            ComboBox1.DroppedDown = False
            ComboBox1.SelectedIndex = -1
            ComboBox1.Text = ""
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub ComboBox2_KeyDown(sender As Object, e As KeyEventArgs) Handles ComboBox2.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                ComboBox2.DroppedDown = False
                ComboBox1.Focus()
                ComboBox1.DroppedDown = True
            Else
                ComboBox2.DroppedDown = False
                creditlimit.Focus()
                creditlimit.SelectAll()
            End If
            e.SuppressKeyPress = True
        ElseIf e.KeyCode = Keys.Back Then
            ComboBox2.DroppedDown = False
            ComboBox2.SelectedIndex = -1
            ComboBox2.Text = ""
            e.SuppressKeyPress = True
        End If
    End Sub


    Private Sub cus_edit_Click(sender As Object, e As EventArgs) Handles cus_edit.Click
        If CustomerDataGridView.CurrentRow Is Nothing Then
            MessageBox.Show("Please select a customer to update.")
            Exit Sub
        End If

        ' Security Check - Bypassed to allow edit access to all users without secure key
        ' If Not IsSecureKeyValid() Then Exit Sub

        If cus_name.Text.Trim() = "" Then
            MessageBox.Show("Please enter customer name.")
            Exit Sub
        End If



        ' Validate Credit Limit
        If String.IsNullOrWhiteSpace(creditlimit.Text) Then
            MessageBox.Show("Credit Limit is a required field.")
            creditlimit.Focus()
            Exit Sub
        End If

        Dim clEdit As Decimal
        If Not Decimal.TryParse(creditlimit.Text.Trim(), clEdit) OrElse clEdit <= 0 Then
            MessageBox.Show("Please enter a valid numeric Credit Limit (greater than 0).")
            creditlimit.Focus()
            Exit Sub
        End If

        ' Validate Credit Period (must be greater than today)
        If DateTimePicker3.Value.Date <= DateTime.Today Then
            MessageBox.Show("Credit Period must be greater than today's date.")
            DateTimePicker3.Focus()
            Exit Sub
        End If

        Dim selectedID As String = CustomerDataGridView.CurrentRow.Cells(0).Value.ToString()
        '  Dim custype As String
        'If stcomptxt.Text = "Out" Then
        'custype = "Retail"
        '  Else
        'custype = "Wholesale"
        ' End If
        Try
            MySqlConn.Open()

            Dim query As String = "UPDATE customer SET " &
                          "name=@name, " &
                          "email=@email, " &
                          "address=@address, " &
                          "city=@city, " &
                          "tel_no=@tel_no, " &
                          "customer_type=@customer_type, " &
                          "vat_id=@vat_id, " &
                          "timestamps=@time, " &
                          "updated_at=@updated_at, " &
                          "credit_limit=@climit, " &
                          "credit_period=@cperiod " &
                          "WHERE id=@id"

            Dim cmd As New MySqlCommand(query, MySqlConn)

            Dim cleanName As String = SanitizeInput(cus_name.Text, True)
            Dim cleanAddress As String = SanitizeInput(cus_address.Text)
            Dim cleanCity As String = SanitizeInput(cus_city.Text, True)
            Dim cleanEmail As String = SanitizeInput(cus_email.Text).ToLower()
            Dim cleanTel As String = SanitizeInput(cus_tel.Text)

            ' Update UI to reflect sanitized values
            cus_name.Text = cleanName
            cus_address.Text = cleanAddress
            cus_city.Text = cleanCity
            cus_email.Text = cleanEmail
            cus_tel.Text = cleanTel

            cmd.Parameters.AddWithValue("@name", cleanName)
            cmd.Parameters.AddWithValue("@email", cleanEmail)
            cmd.Parameters.AddWithValue("@address", cleanAddress)
            cmd.Parameters.AddWithValue("@city", cleanCity)
            cmd.Parameters.AddWithValue("@tel_no", cleanTel)
            cmd.Parameters.AddWithValue("@customer_type", ComboBox1.Text)
            cmd.Parameters.AddWithValue("@vat_id", If(ComboBox2.SelectedValue IsNot Nothing, ComboBox2.SelectedValue, DBNull.Value))
            cmd.Parameters.AddWithValue("@time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@updated_at", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@climit", If(String.IsNullOrWhiteSpace(creditlimit.Text), 0, Convert.ToDecimal(creditlimit.Text)))
            cmd.Parameters.AddWithValue("@cperiod", DateTimePicker3.Value.ToString("yyyy-MM-dd"))
            cmd.Parameters.AddWithValue("@id", selectedID)

            cmd.ExecuteNonQuery()

            MessageBox.Show("Customer Updated Successfully!")

        Catch ex As MySqlException
            MessageBox.Show("Database Error: " & ex.Message)
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            MySqlConn.Close()
        End Try

        load_Customers_Filtered()
        ClearInputFields()
    End Sub
    Private Sub PopulateSelectedCustomer(rowIndex As Integer)
        If rowIndex < 0 OrElse rowIndex >= CustomerDataGridView.Rows.Count Then Exit Sub

        Dim row As DataGridViewRow = CustomerDataGridView.Rows(rowIndex)
        cus_name.Text = row.Cells("name").Value.ToString()
        cus_address.Text = row.Cells("address").Value.ToString()
        If CustomerDataGridView.Columns.Contains("city") AndAlso Not IsDBNull(row.Cells("city").Value) Then
            cus_city.Text = row.Cells("city").Value.ToString()
        Else
            cus_city.Text = ""
        End If
        cus_tel.Text = row.Cells("tel_no").Value.ToString()
        cus_email.Text = row.Cells("email").Value.ToString()
        ' Robustly set ComboBox1 even if the database value is a partial match or missing " Sale"
        Dim typeVal As String = row.Cells("customer_type").Value.ToString()
        If String.IsNullOrWhiteSpace(typeVal) Then
            ComboBox1.SelectedIndex = -1
        Else
            Dim typeIdx As Integer = ComboBox1.FindString(typeVal)
            If typeIdx >= 0 Then
                ComboBox1.SelectedIndex = typeIdx
            Else
                ComboBox1.Text = typeVal
            End If
        End If

        If Not IsDBNull(row.Cells("vat_id").Value) Then
            ComboBox2.SelectedValue = row.Cells("vat_id").Value
        Else
            ComboBox2.SelectedIndex = -1
        End If

        If CustomerDataGridView.Columns.Contains("credit_limit") AndAlso Not IsDBNull(row.Cells("credit_limit").Value) Then
            creditlimit.Text = row.Cells("credit_limit").Value.ToString()
        Else
            creditlimit.Text = ""
        End If

        If CustomerDataGridView.Columns.Contains("credit_period") AndAlso Not IsDBNull(row.Cells("credit_period").Value) Then
            Dim cpVal As Object = row.Cells("credit_period").Value
            Dim cpDate As DateTime
            If DateTime.TryParse(cpVal.ToString(), cpDate) Then
                DateTimePicker3.Value = cpDate
            Else
                DateTimePicker3.Value = DateTime.Now
            End If
        Else
            DateTimePicker3.Value = DateTime.Now
        End If

        ' Toggle ReActive/Block button visibility based on is_block status
        If row.Cells("is_block").Value IsNot Nothing AndAlso
           row.Cells("is_block").Value.ToString() = "1" Then
            btn_reactivate.Visible = True
            cus_block.Visible = False
        Else
            btn_reactivate.Visible = False
            cus_block.Visible = True
        End If
    End Sub

    Private Sub CustomerDataGridView_RowEnter(sender As Object, e As DataGridViewCellEventArgs) Handles CustomerDataGridView.RowEnter
        If _isInitializing Then Return
        If e.RowIndex >= 0 Then
            PopulateSelectedCustomer(e.RowIndex)
        End If
    End Sub

    Private Sub CustomerDataGridView_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles CustomerDataGridView.CellClick
        If e.RowIndex >= 0 Then
            PopulateSelectedCustomer(e.RowIndex)
        End If
    End Sub

    Private Sub CustomerDataGridView_KeyDown(sender As Object, e As KeyEventArgs) Handles CustomerDataGridView.KeyDown
        If e.KeyCode = Keys.Enter Then
            If CustomerDataGridView.CurrentRow IsNot Nothing Then
                PopulateSelectedCustomer(CustomerDataGridView.CurrentRow.Index)
                cus_name.Focus()
                e.Handled = True
                e.SuppressKeyPress = True
            End If
        End If
    End Sub

    Private Sub cus_delete_Click(sender As Object, e As EventArgs) Handles cus_delete.Click
        If CustomerDataGridView.CurrentRow Is Nothing Then
            MessageBox.Show("Please select a customer from the list to delete.", "Selection Required")
            Exit Sub
        End If

        ' Security Check
        If Not IsSecureKeyValid() Then Exit Sub

        Dim selectedID As String = CustomerDataGridView.CurrentRow.Cells(0).Value.ToString()

        ' Check for outstanding or historical credit records
        Try
            If MySqlConn.State <> ConnectionState.Open Then MySqlConn.Open()
            Dim checkQuery As String = "SELECT COUNT(*) FROM customer_credit WHERE customer_id = @id"
            Dim checkCmd As New MySqlCommand(checkQuery, MySqlConn)
            checkCmd.Parameters.AddWithValue("@id", selectedID)
            Dim count As Integer = Convert.ToInt32(checkCmd.ExecuteScalar())

            If count > 0 Then
                MessageBox.Show("This customer cannot be deleted because they have associated records in the credit table. Please resolve or archive those records before attempting to delete the customer.", "Deletion Blocked", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
        Catch ex As Exception
            MessageBox.Show("Error verifying customer credit: " & ex.Message, "Check Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        Finally
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try

        Dim result As DialogResult = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

        If result = DialogResult.Yes Then
            Try
                MySqlConn.Open()
                Dim Query As String = "UPDATE customer SET deleted_at = @deleted_at WHERE id = @id"
                Dim COMMAND As New MySqlCommand(Query, MySqlConn)
                COMMAND.Parameters.AddWithValue("@id", selectedID)
                COMMAND.Parameters.AddWithValue("@deleted_at", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                COMMAND.ExecuteNonQuery()

                ' Centralized System log deletion
                Module1.LogDeletion("Customer", selectedID.ToString(), "Customer Name: " & cus_name.Text & ", Tel: " & cus_tel.Text & ", Email: " & cus_email.Text)

                MessageBox.Show("Data Deleted Successfully!")
            Catch ex As Exception
                MessageBox.Show("Error: " & ex.Message)
            Finally
                If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
            End Try

            load_Customers_Filtered()
            ClearInputFields()
        End If
    End Sub

    Private Sub cus_block_Click(sender As Object, e As EventArgs) Handles cus_block.Click

        If CustomerDataGridView.CurrentRow Is Nothing Then
            MessageBox.Show("Please select a customer from the list to block.", "Selection Required")
            Exit Sub
        End If


        Dim selectedID As String = CustomerDataGridView.CurrentRow.Cells(0).Value.ToString()
        Dim customerName As String = CustomerDataGridView.CurrentRow.Cells(1).Value.ToString()


        Dim result As DialogResult = MessageBox.Show("Are you sure you want to block " & customerName & "?",
                                                 "Confirm Block", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation)

        If result = DialogResult.Yes Then
            ' Security Check
            If Not IsSecureKeyValid() Then Exit Sub

            Try
                MySqlConn.Open()


                Dim query As String = "UPDATE customer SET is_block = 1 WHERE id = @id"

                Dim cmd As New MySqlCommand(query, MySqlConn)
                cmd.Parameters.AddWithValue("@id", selectedID)

                cmd.ExecuteNonQuery()

                MessageBox.Show("Customer has been blocked successfully.")

            Catch ex As Exception
                MessageBox.Show("Error: " & ex.Message)
            Finally
                If MySqlConn.State = ConnectionState.Open Then
                    MySqlConn.Close()
                End If
            End Try


            load_Customers_Filtered()
            ClearInputFields()
        End If
    End Sub

    Private Sub btn_reactivate_Click(sender As Object, e As EventArgs) Handles btn_reactivate.Click
        If CustomerDataGridView.CurrentRow Is Nothing Then Exit Sub

        Dim selectedID As String = CustomerDataGridView.CurrentRow.Cells(0).Value.ToString()
        Dim customerName As String = CustomerDataGridView.CurrentRow.Cells(1).Value.ToString()

        Dim result As DialogResult = MessageBox.Show("Are you sure you want to re-activate " & customerName & "?",
                                                 "Confirm Re-Activation", MessageBoxButtons.YesNo, MessageBoxIcon.Information)

        If result = DialogResult.Yes Then
            ' Security Check
            If Not IsSecureKeyValid() Then Exit Sub

            Try
                MySqlConn.Open()
                Dim query As String = "UPDATE customer SET is_block = 0 WHERE id = @id"
                Dim cmd As New MySqlCommand(query, MySqlConn)
                cmd.Parameters.AddWithValue("@id", selectedID)
                cmd.ExecuteNonQuery()

                MessageBox.Show("Customer has been re-activated successfully.")
                btn_reactivate.Visible = False
                load_Customers_Filtered()
                ClearInputFields()
            Catch ex As Exception
                MessageBox.Show("Error re-activating customer: " & ex.Message)
            Finally
                If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
            End Try
        End If
    End Sub

    Private Sub CustomerDataGridView_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles CustomerDataGridView.CellFormatting
        ' Check if the grid has data and the column index is valid
        If CustomerDataGridView.Columns.Contains("is_block") Then
            Dim row As DataGridViewRow = CustomerDataGridView.Rows(e.RowIndex)
            Dim isBlocked As Object = row.Cells("is_block").Value

            If isBlocked IsNot Nothing AndAlso isBlocked.ToString() = "1" Then
                e.CellStyle.BackColor = Color.Red
                e.CellStyle.ForeColor = Color.White
            End If
        End If
    End Sub




    Private Sub creditlimit_KeyDown(sender As Object, e As KeyEventArgs) Handles creditlimit.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                ComboBox2.Focus()
                ComboBox2.DroppedDown = True
            Else
                DateTimePicker3.Focus()
            End If
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub DateTimePicker3_KeyDown(sender As Object, e As KeyEventArgs) Handles DateTimePicker3.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                creditlimit.Focus()
                creditlimit.SelectAll()
            Else
                cus_save.PerformClick()
            End If
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub customer_add_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.F2 Then
            e.Handled = True
            e.SuppressKeyPress = True
            cus_add.PerformClick()
            BeginInvoke(Sub() cus_name.Focus())
        ElseIf e.KeyCode = Keys.F3 Then
            cus_edit.PerformClick()
        ElseIf e.KeyCode = Keys.F12 Then
            secure_key.Select()

        ElseIf e.KeyCode = Keys.Delete Then
            cus_delete.PerformClick()
        End If
    End Sub

    Private Sub addvat_Click(sender As Object, e As EventArgs) Handles addvat.Click
        vat_add.StartPosition = FormStartPosition.CenterParent
        vat_add.ShowDialog(Me)
        LoadVatOptions()
    End Sub

    Private Sub print_Click(sender As Object, e As EventArgs) Handles print.Click
        Try
            Dim rptDoc As New CrystalDecisions.CrystalReports.Engine.ReportDocument()
            rptDoc = New Customer()

            ' Dynamically determine the table alias used in the report for customer fields
            Dim tableName As String = "customer"
            Dim foundCust As Boolean = False
            For Each tbl As CrystalDecisions.CrystalReports.Engine.Table In rptDoc.Database.Tables
                If tbl.Name.ToLower().Contains("customer") Then
                    tableName = tbl.Name
                    foundCust = True
                    Exit For
                End If
            Next
            If Not foundCust AndAlso rptDoc.Database.Tables.Count > 0 Then
                tableName = rptDoc.Database.Tables(0).Name
            End If

            ' Apply RecordSelectionFormula if filtering textboxes are not empty
            If ser_name.Text.Trim() <> "" OrElse ser_tel.Text.Trim() <> "" OrElse ser_address.Text.Trim() <> "" Then
                Dim formula As String = ""
                Dim filters As New List(Of String)

                If ser_name.Text.Trim() <> "" Then
                    filters.Add("{" & tableName & ".name} LIKE '*" & ser_name.Text.Replace("'", "''") & "*'")
                End If
                If ser_tel.Text.Trim() <> "" Then
                    filters.Add("{" & tableName & ".tel_no} LIKE '*" & ser_tel.Text.Replace("'", "''") & "*'")
                End If
                If ser_address.Text.Trim() <> "" Then
                    filters.Add("({" & tableName & ".address} LIKE '*" & ser_address.Text.Replace("'", "''") & "*' OR {" & tableName & ".city} LIKE '*" & ser_address.Text.Replace("'", "''") & "*')")
                End If

                If filters.Count > 0 Then
                    formula = String.Join(" AND ", filters)
                    Try
                        rptDoc.RecordSelectionFormula = formula
                    Catch ex As Exception
                        MessageBox.Show("Error applying filter formula: " & ex.Message)
                    End Try
                End If
            End If

            Dim rptViewerForm As New Form()
            rptViewerForm.Text = "Customer Report Preview"
            rptViewerForm.WindowState = FormWindowState.Maximized

            Dim crViewer As New CrystalDecisions.Windows.Forms.CrystalReportViewer()
            crViewer.Dock = DockStyle.Fill
            crViewer.ReportSource = rptDoc
            crViewer.ShowRefreshButton = False
            crViewer.ShowCopyButton = False
            crViewer.ShowGroupTreeButton = False

            rptViewerForm.Controls.Add(crViewer)
            rptViewerForm.ShowDialog()

        Catch ex As Exception
            MessageBox.Show("Error loading report: " & ex.Message)
        End Try
    End Sub

    Private Sub Label8_Click(sender As Object, e As EventArgs) Handles Label8.Click

    End Sub

    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles Label3.Click

    End Sub

End Class

