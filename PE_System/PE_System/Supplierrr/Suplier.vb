Imports MySql.Data.MySqlClient
Public Class Suplier
    Private _isInitializing As Boolean = False
    Dim COMMAND As MySqlCommand
    Dim READER As MySqlDataReader

    Private Sub setup_supplier_grid()
        If Supplier_DataGridView.Columns.Count = 0 Then Return

        ' Configure general grid properties
        Supplier_DataGridView.AllowUserToAddRows = False
        Supplier_DataGridView.AllowUserToDeleteRows = False
        Supplier_DataGridView.ReadOnly = True
        Supplier_DataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        Supplier_DataGridView.RowHeadersVisible = False
        Supplier_DataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        Supplier_DataGridView.BackgroundColor = Color.White
        Supplier_DataGridView.GridColor = Color.Black
        Supplier_DataGridView.BorderStyle = BorderStyle.None
        Supplier_DataGridView.EditMode = DataGridViewEditMode.EditProgrammatically

        ' Configure Fonts
        Supplier_DataGridView.DefaultCellStyle.Font = New Font("Microsoft Sans Serif", 14)
        Supplier_DataGridView.ColumnHeadersDefaultCellStyle.Font = New Font("Microsoft Sans Serif", 16, FontStyle.Bold)

        ' Adjust Row Height for larger font
        Supplier_DataGridView.RowTemplate.Height = 35
        For Each row As DataGridViewRow In Supplier_DataGridView.Rows
            row.Height = 35
        Next
        Supplier_DataGridView.ColumnHeadersHeight = 45

        ' Set Header Text (Capitalized first letters as requested)
        If Supplier_DataGridView.Columns.Count >= 8 Then
            Supplier_DataGridView.Columns(0).Visible = False
            Supplier_DataGridView.Columns(1).HeaderText = "Supplier Name"
            Supplier_DataGridView.Columns(1).Width = 250
            Supplier_DataGridView.Columns(2).HeaderText = "Address"
            Supplier_DataGridView.Columns(2).Width = 300
            Supplier_DataGridView.Columns(3).HeaderText = "Telephone No"
            Supplier_DataGridView.Columns(3).Width = 150
            Supplier_DataGridView.Columns(4).HeaderText = "Email Address"
            Supplier_DataGridView.Columns(4).Width = 200
            Supplier_DataGridView.Columns(5).HeaderText = "Registered Date"
            Supplier_DataGridView.Columns(5).Width = 150
            Supplier_DataGridView.Columns(6).HeaderText = "Debit Limit"
            Supplier_DataGridView.Columns(6).Width = 150
            Supplier_DataGridView.Columns(7).HeaderText = "Debit Period"
            Supplier_DataGridView.Columns(7).Width = 150
        End If
    End Sub

    Private Sub load_supplier()






        conn.Open()
        Dim table As New DataTable()
        Dim adapter As New MySqlDataAdapter("SELECT id, name, address, tel_no, email, register_date, debit_limit, debit_period from supplier ORDER BY name ASC", conn)
        adapter.Fill(table)

        _isInitializing = True
        Supplier_DataGridView.DataSource = table
        BeginInvoke(Sub()
                        Supplier_DataGridView.CurrentCell = Nothing
                        Supplier_DataGridView.ClearSelection()
                        _isInitializing = False
                    End Sub)
        setup_supplier_grid()
        conn.Close()
    End Sub

    Private Sub Addbtn_Click(sender As Object, e As EventArgs) Handles Addbtn.Click
        NameTxt.Clear()
        AddressTxt.Clear()
        TelNoTxt.Clear()
        EmailTxt.Clear()
        DebitLimitTxt.Clear()
        DateTimePicker3.Value = DateTime.Now.AddMonths(2)
        NameTxt.Select()
    End Sub

    Private Sub Suplier_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.KeyPreview = True
        secure_key.UseSystemPasswordChar = True
        LoadUserList()

        ' Auto-fill logged-in user
        If Not String.IsNullOrEmpty(Module1.UserName) Then
            ComboBox3.Text = Module1.UserName
        End If

        load_supplier()
    End Sub

    Private Sub Suplier_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.F2 Then
            Addbtn.PerformClick()
        ElseIf e.KeyCode = Keys.F3 Then
            Editbtn.PerformClick()
        ElseIf e.KeyCode = Keys.Delete Then
            deletebtn.PerformClick()
        End If
    End Sub

    Private Sub savebtn_Click(sender As Object, e As EventArgs) Handles savebtn.Click
        If String.IsNullOrWhiteSpace(NameTxt.Text) Then
            MessageBox.Show("Supplier Name cannot be empty.")
            NameTxt.Focus()
            Exit Sub
        End If

        Dim dLimit As Double = 0
        Double.TryParse(DebitLimitTxt.Text.Trim(), dLimit)

        Dim Nname As String = NameTxt.Text.Trim()

        Try
            If MySqlConn.State <> ConnectionState.Open Then MySqlConn.Open()
            Dim Query As String = "insert into supplier (name,address,tel_no,email,register_date,debit_limit,debit_period) values (@name, @address, @tel_no, @email, now(), @limit, @debit_period)"
            COMMAND = New MySqlCommand(Query, MySqlConn)
            COMMAND.Parameters.AddWithValue("@name", Nname)
            COMMAND.Parameters.AddWithValue("@address", AddressTxt.Text)
            COMMAND.Parameters.AddWithValue("@tel_no", TelNoTxt.Text)
            COMMAND.Parameters.AddWithValue("@email", EmailTxt.Text)
            COMMAND.Parameters.AddWithValue("@limit", dLimit)
            COMMAND.Parameters.AddWithValue("@debit_period", DateTimePicker3.Value.ToString("yyyy-MM-dd"))
            COMMAND.ExecuteNonQuery()
            MySqlConn.Close()
            MessageBox.Show("Supplier Saved Successfully!")
            ClearFields()
            RefreshGrid()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Sub
    Private Sub NameTxt_KeyDown(sender As Object, e As KeyEventArgs) Handles NameTxt.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            AddressTxt.Select()
        ElseIf e.KeyCode = Keys.Up OrElse e.KeyCode = Keys.Down Then
            If Supplier_DataGridView.Rows.Count > 0 Then
                Dim currentIndex As Integer = If(Supplier_DataGridView.CurrentRow IsNot Nothing, Supplier_DataGridView.CurrentRow.Index, -1)
                Dim newIndex As Integer = currentIndex

                If e.KeyCode = Keys.Down Then
                    newIndex = Math.Min(Supplier_DataGridView.Rows.Count - 1, currentIndex + 1)
                ElseIf e.KeyCode = Keys.Up Then
                    newIndex = Math.Max(0, currentIndex - 1)
                End If

                If newIndex <> currentIndex AndAlso newIndex >= 0 Then
                    Supplier_DataGridView.CurrentCell = Supplier_DataGridView.Rows(newIndex).Cells(1)
                    ' RowEnter will trigger PopulateSelectedSupplier
                    e.Handled = True
                End If
            End If
        End If
    End Sub

    Private Sub AddressTxt_KeyDown(sender As Object, e As KeyEventArgs) Handles AddressTxt.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            If e.Shift Then
                NameTxt.Select()
            Else
                TelNoTxt.Select()
            End If
        End If
    End Sub

    Private Sub TelNoTxt_KeyDown(sender As Object, e As KeyEventArgs) Handles TelNoTxt.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            If e.Shift Then
                AddressTxt.Select()
            Else
                EmailTxt.Select()
            End If
        End If
    End Sub

    Private Sub EmailTxt_KeyDown(sender As Object, e As KeyEventArgs) Handles EmailTxt.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            If e.Shift Then
                TelNoTxt.Select()
            Else
                DebitLimitTxt.Focus()
            End If
        End If
    End Sub

    Private Sub DebitLimitTxt_KeyDown(sender As Object, e As KeyEventArgs) Handles DebitLimitTxt.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            If e.Shift Then
                EmailTxt.Select()
            Else
                DateTimePicker3.Focus()
            End If
        End If
    End Sub

    Private Sub DateTimePicker3_KeyDown(sender As Object, e As KeyEventArgs) Handles DateTimePicker3.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            If e.Shift Then
                DebitLimitTxt.Focus()
            Else
                savebtn.PerformClick()
            End If
        End If
    End Sub

    Private Sub Editbtn_Click(sender As Object, e As EventArgs) Handles Editbtn.Click
        If Supplier_DataGridView.CurrentRow Is Nothing Then
            MessageBox.Show("Please select a supplier from the list to update.")
            Exit Sub
        End If

        ' Security Check
        If Not IsSecureKeyValid() Then Exit Sub

        If String.IsNullOrWhiteSpace(NameTxt.Text) Then
            MessageBox.Show("Supplier Name cannot be empty.")
            NameTxt.Focus()
            Exit Sub
        End If

        Dim dLimit As Double = 0
        Double.TryParse(DebitLimitTxt.Text.Trim(), dLimit)

        Dim selectedID As String = Supplier_DataGridView.CurrentRow.Cells(0).Value.ToString()
        Try
            If MySqlConn.State <> ConnectionState.Open Then MySqlConn.Open()
            Dim Query As String = "update supplier set name=@name, address=@address, tel_no=@tel_no, email=@email, debit_limit=@limit, debit_period=@debit_period where id=@id"
            COMMAND = New MySqlCommand(Query, MySqlConn)
            COMMAND.Parameters.AddWithValue("@name", NameTxt.Text.Trim())
            COMMAND.Parameters.AddWithValue("@address", AddressTxt.Text.Trim())
            COMMAND.Parameters.AddWithValue("@tel_no", TelNoTxt.Text.Trim())
            COMMAND.Parameters.AddWithValue("@email", EmailTxt.Text.Trim())
            COMMAND.Parameters.AddWithValue("@limit", dLimit)
            COMMAND.Parameters.AddWithValue("@debit_period", DateTimePicker3.Value.ToString("yyyy-MM-dd"))
            COMMAND.Parameters.AddWithValue("@id", selectedID)

            COMMAND.ExecuteNonQuery()
            MySqlConn.Close()

            MessageBox.Show("Supplier Updated Successfully!")

            RefreshGrid()

            ClearFields()
        Catch ex As Exception
            MessageBox.Show("Update Error: " & ex.Message)
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Sub

    Private Sub PopulateSelectedSupplier(rowIndex As Integer)
        If rowIndex < 0 OrElse rowIndex >= Supplier_DataGridView.Rows.Count Then Exit Sub

        Dim row As DataGridViewRow = Supplier_DataGridView.Rows(rowIndex)
        NameTxt.Text = row.Cells(1).Value.ToString()
        AddressTxt.Text = row.Cells(2).Value.ToString()
        TelNoTxt.Text = row.Cells(3).Value.ToString()
        EmailTxt.Text = row.Cells(4).Value.ToString()
        DebitLimitTxt.Text = row.Cells(6).Value.ToString()

        ' Populate Debit Period
        Dim dPeriodValue As Object = row.Cells(7).Value
        If dPeriodValue IsNot Nothing AndAlso Not IsDBNull(dPeriodValue) Then
            Dim dPeriod As DateTime
            If DateTime.TryParse(dPeriodValue.ToString(), dPeriod) Then
                DateTimePicker3.Value = dPeriod
            End If
        End If
    End Sub

    Private Sub Supplier_DataGridView_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles Supplier_DataGridView.CellContentClick


        If e.RowIndex >= 0 Then
            PopulateSelectedSupplier(e.RowIndex)
        End If
    End Sub

    Private Sub deletebtn_Click(sender As Object, e As EventArgs) Handles deletebtn.Click
        If Supplier_DataGridView.CurrentRow Is Nothing Then
            MessageBox.Show("Please select a supplier from the list to delete.", "Selection Required")
            Exit Sub
        End If

        ' Security Check
        If Not IsSecureKeyValid() Then Exit Sub
        Dim result As DialogResult = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

        If result = DialogResult.Yes Then
            Dim selectedID As String = Supplier_DataGridView.CurrentRow.Cells(0).Value.ToString()
            Try
                If MySqlConn.State <> ConnectionState.Open Then MySqlConn.Open()
                Dim Query As String = "delete from supplier where id=@id"
                COMMAND = New MySqlCommand(Query, MySqlConn)
                COMMAND.Parameters.AddWithValue("@id", selectedID)
                COMMAND.ExecuteNonQuery()

                ' Centralized System log deletion
                Module1.LogDeletion("Supplier", selectedID, "Supplier Name: " & NameTxt.Text & ", Tel: " & TelNoTxt.Text & ", Email: " & EmailTxt.Text)

                MySqlConn.Close()

                MessageBox.Show("Data Deleted Successfully!")
                RefreshGrid()
                ClearFields()
            Catch ex As Exception
                MessageBox.Show("Delete Error: " & ex.Message)
                If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
            End Try
        Else
            RefreshGrid() ' Ensure UI is in sync if local deletion attempted
        End If
    End Sub

    Private Sub RefreshGrid()
        If Not String.IsNullOrWhiteSpace(Nameserch.Text) Then
            Nameserch_TextChanged(Nothing, Nothing)
        ElseIf Not String.IsNullOrWhiteSpace(TelSearch.Text) Then
            TelSearch_TextChanged(Nothing, Nothing)
        Else
            load_supplier()
        End If
    End Sub

    Private Sub Nameserch_TextChanged(sender As Object, e As EventArgs) Handles Nameserch.TextChanged
        conn.Open()
        Dim table As New DataTable()
        Dim adapter As New MySqlDataAdapter("select id, name, address, tel_no, email, register_date, debit_limit, debit_period from supplier", conn)
        adapter.Fill(table)

        Dim dv As New DataView(table)
        dv.RowFilter = String.Format("name Like '{0}%'", Nameserch.Text.Replace("'", "''"))
        dv.Sort = "name ASC"
        _isInitializing = True
        Supplier_DataGridView.DataSource = dv
        BeginInvoke(Sub()
                        Supplier_DataGridView.CurrentCell = Nothing
                        Supplier_DataGridView.ClearSelection()
                        _isInitializing = False
                    End Sub)
        setup_supplier_grid()
        conn.Close()
    End Sub

    Private Sub TelSearch_TextChanged(sender As Object, e As EventArgs) Handles TelSearch.TextChanged
        conn.Open()
        Dim table As New DataTable()
        Dim adapter As New MySqlDataAdapter("select id, name, address, tel_no, email, register_date, debit_limit, debit_period from supplier", conn)
        adapter.Fill(table)

        Dim dv As New DataView(table)
        dv.RowFilter = String.Format("tel_no Like '{0}%'", TelSearch.Text.Replace("'", "''"))
        dv.Sort = "name ASC"
        _isInitializing = True
        Supplier_DataGridView.DataSource = dv
        BeginInvoke(Sub()
                        Supplier_DataGridView.CurrentCell = Nothing
                        Supplier_DataGridView.ClearSelection()
                        _isInitializing = False
                    End Sub)
        setup_supplier_grid()
        conn.Close()
    End Sub

    Private Sub Supplier_DataGridView_RowEnter(sender As Object, e As DataGridViewCellEventArgs) Handles Supplier_DataGridView.RowEnter
        If _isInitializing Then Return
        If e.RowIndex >= 0 Then
            PopulateSelectedSupplier(e.RowIndex)
        End If
    End Sub

    Private Sub Supplier_DataGridView_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles Supplier_DataGridView.CellClick
        If e.RowIndex >= 0 Then
            PopulateSelectedSupplier(e.RowIndex)
        End If
    End Sub

    Private Sub Supplier_DataGridView_KeyDown(sender As Object, e As KeyEventArgs) Handles Supplier_DataGridView.KeyDown
        If e.KeyCode = Keys.Enter Then
            If Supplier_DataGridView.CurrentRow IsNot Nothing Then
                PopulateSelectedSupplier(Supplier_DataGridView.CurrentRow.Index)
                NameTxt.Focus()
                e.Handled = True
                e.SuppressKeyPress = True
            End If
        End If
    End Sub

    ' Helper method to clear fields (mirrors customer_add.vb logic)
    Private Sub ClearFields()
        NameTxt.Clear()
        AddressTxt.Clear()
        TelNoTxt.Clear()
        EmailTxt.Clear()
        DebitLimitTxt.Clear()
        DateTimePicker3.Value = DateTime.Now.AddMonths(2)
        secure_key.Clear()
        NameTxt.Select()
    End Sub



    Private Sub LoadUserList()
        Try
            If MySqlConn.State <> ConnectionState.Open Then MySqlConn.Open()

            Dim adapter As New MySqlDataAdapter("SELECT id, name FROM user WHERE (status IS NULL OR status = 'active')", MySqlConn)
            Dim dt As New DataTable()
            adapter.Fill(dt)

            ComboBox3.DataSource = dt
            ComboBox3.DisplayMember = "name"
            ComboBox3.ValueMember = "id"
            ComboBox3.SelectedIndex = -1

            MySqlConn.Close()
        Catch ex As Exception
            MessageBox.Show("Error loading users: " & ex.Message)
        Finally
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Sub

    Private Function IsSecureKeyValid() As Boolean
        If ComboBox3.SelectedIndex = -1 Then
            MessageBox.Show("Please select a user from 'Login User' list.")
            ComboBox3.Focus()
            Return False
        End If

        If String.IsNullOrWhiteSpace(secure_key.Text) Then
            MessageBox.Show("You Are Not Authorized To Do That")
            secure_key.Focus()
            Return False
        End If


        Try
            If MySqlConn.State <> ConnectionState.Open Then MySqlConn.Open()

            Dim userId As Integer = Convert.ToInt32(ComboBox3.SelectedValue)
            Dim query As String = "SELECT hiddenSecurekey FROM user WHERE id = @id AND (status IS NULL OR status = 'active')"
            Dim cmd As New MySqlCommand(query, MySqlConn)
            cmd.Parameters.AddWithValue("@id", userId)

            Dim result = cmd.ExecuteScalar()
            If result IsNot Nothing AndAlso result.ToString() = secure_key.Text.Trim() Then
                Return True
            Else
                MessageBox.Show("Invalid Secure Key!")
                secure_key.Clear()
                secure_key.Focus()
                Return False
            End If
        Catch ex As Exception
            MessageBox.Show("Security check failed: " & ex.Message)
            Return False
        Finally
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Function

    Private Sub Print_Click(sender As Object, e As EventArgs) Handles Print.Click
        Try
            Dim rptDoc As New CrystalDecisions.CrystalReports.Engine.ReportDocument()
            rptDoc = New Supplier()

            ' Dynamically determine the table alias used in the report for supplier fields
            Dim tableName As String = "supplier"
            Dim foundSupp As Boolean = False
            For Each tbl As CrystalDecisions.CrystalReports.Engine.Table In rptDoc.Database.Tables
                If tbl.Name.ToLower().Contains("supplier") Then
                    tableName = tbl.Name
                    foundSupp = True
                    Exit For
                End If
            Next
            If Not foundSupp AndAlso rptDoc.Database.Tables.Count > 0 Then
                tableName = rptDoc.Database.Tables(0).Name
            End If

            ' Apply RecordSelectionFormula if filtering textboxes are not empty
            If Nameserch.Text.Trim() <> "" OrElse TelSearch.Text.Trim() <> "" Then
                Dim filters As New List(Of String)

                If Nameserch.Text.Trim() <> "" Then
                    filters.Add("{" & tableName & ".name} LIKE '" & Nameserch.Text.Replace("'", "''") & "*'")
                End If
                If TelSearch.Text.Trim() <> "" Then
                    filters.Add("{" & tableName & ".tel_no} LIKE '" & TelSearch.Text.Replace("'", "''") & "*'")
                End If

                If filters.Count > 0 Then
                    rptDoc.RecordSelectionFormula = String.Join(" AND ", filters)
                End If
            End If

            Dim rptViewerForm As New Form()
            rptViewerForm.Text = "Supplier Report Preview"
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

    Private Sub btnDelete_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub btnSuccess_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub btnAddNew_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Label8_Click(sender As Object, e As EventArgs) Handles Label8.Click

    End Sub

    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles Label3.Click

    End Sub

End Class
