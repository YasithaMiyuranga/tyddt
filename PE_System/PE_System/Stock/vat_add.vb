Imports MySql.Data.MySqlClient
Public Class vat_add

    Dim COMMAND As MySqlCommand
    Dim READER As MySqlDataReader
    Dim selectedVatID As Integer = 0
    Private Sub load_Vats()
        If vat_type.Items.Count = 0 Then
            vat_type.Items.Add("Sales")
            vat_type.Items.Add("Service")
            vat_type.Items.Add("Other")
        End If

        If ser_vattype.Items.Count = 0 Then
            ser_vattype.Items.Add("All")
            ser_vattype.Items.Add("Sales")
            ser_vattype.Items.Add("Service")
            ser_vattype.Items.Add("Other")
            ser_vattype.SelectedIndex = 0
        End If
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()

            Dim table As New DataTable()

            Dim adapter As New MySqlDataAdapter("SELECT id, vat_value, vat_name, type, IF(is_active = 1, 'Active', 'Inactive') AS is_active FROM vat;", conn)
            adapter.Fill(table)

            VatDataGridView.DataSource = table
            FormatGridColumns()
            tot_vat.Text = table.Rows.Count

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub

    Private Sub add_vat_Click(sender As Object, e As EventArgs) Handles add_vat.Click
        vat_name.Clear()
        vat_percentage.Clear()
        btn_active.Visible = False
        btn_inactive.Visible = False
        vat_name.Select()
    End Sub
    Private Sub vat_add_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.KeyPreview = True
        Me.WindowState = FormWindowState.Maximized
        load_Vats()
        load_Users()
        Dim rowcount As Integer = VatDataGridView.Rows.Count
        tot_vat.Text = rowcount
        ClearInputs()

        ' Ensure the form starts at the top and focus is on the first field
        vat_name.Select()
        Me.AutoScrollPosition = New Point(0, 0)
    End Sub

    Private Sub ser_vatname_TextChanged(sender As Object, e As EventArgs) Handles ser_vatname.TextChanged
        load_vat_Filtered()
    End Sub
    Private Sub FormatGridColumns()
        VatDataGridView.ReadOnly = True
        VatDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        VatDataGridView.MultiSelect = False

        If VatDataGridView.Columns.Count >= 5 Then
            VatDataGridView.Columns(0).Visible = False ' Hide id

            VatDataGridView.Columns(1).HeaderText = "Value (%)"

            VatDataGridView.Columns(2).HeaderText = "VAT Name"

            VatDataGridView.Columns(3).HeaderText = "Type"

            VatDataGridView.Columns(4).HeaderText = "Status"
        End If

        VatDataGridView.RowHeadersVisible = False

        ' Increase internal text size of the GridView
        VatDataGridView.DefaultCellStyle.Font = New Font("Microsoft Sans Serif", 14)
        VatDataGridView.ColumnHeadersDefaultCellStyle.Font = New Font("Microsoft Sans Serif", 16, FontStyle.Bold)
    End Sub
    Private Sub load_vat_Filtered(Optional updateTotal As Boolean = False)
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()

            Dim table As New DataTable()
            Dim adapter As New MySqlDataAdapter("SELECT id, vat_value, vat_name, type, IF(is_active = 1, 'Active', 'Inactive') AS is_active FROM vat", conn)
            adapter.Fill(table)

            Dim dv As New DataView(table)
            Dim filterString As String = ""

            If Not String.IsNullOrEmpty(ser_vatname.Text) Then
                filterString &= String.Format("vat_name Like '%{0}%'", ser_vatname.Text)
            End If

            If ser_vattype.SelectedIndex > 0 AndAlso ser_vattype.Text <> "All" Then
                If filterString.Length > 0 Then filterString &= " AND "
                filterString &= String.Format("type = '{0}'", ser_vattype.Text)
            End If

            dv.RowFilter = filterString
            VatDataGridView.DataSource = dv

            FormatGridColumns()

            If updateTotal Then
                tot_vat.Text = table.Rows.Count
            End If

        Catch ex As Exception
            MessageBox.Show("Filter Error: " & ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub

    Private Sub ser_vattype_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ser_vattype.SelectedIndexChanged
        load_vat_Filtered()
    End Sub

    Private Sub vat_save_Click(sender As Object, e As EventArgs) Handles vat_save.Click

        If vat_name.Text.Trim() = "" Or vat_percentage.Text.Trim() = "" Or vat_type.Text = "" Then
            MessageBox.Show("Please fill in all fields (Name, Percentage, and Type).")
            Exit Sub
        End If

        Try
            If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()


            Dim Query As String = "INSERT INTO vat (vat_value, vat_name, type, is_active) VALUES (@value, @name, @type, @active)"

            Using COMMAND As New MySqlCommand(Query, MySqlConn)

                COMMAND.Parameters.AddWithValue("@value", vat_percentage.Text.Trim())
                COMMAND.Parameters.AddWithValue("@name", vat_name.Text.Trim())
                COMMAND.Parameters.AddWithValue("@type", vat_type.Text)
                COMMAND.Parameters.AddWithValue("@active", 1)

                COMMAND.ExecuteNonQuery()
            End Using

            MessageBox.Show("VAT Record Saved Successfully!")


            load_vat_Filtered(True)
            ClearInputs()

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            MySqlConn.Close()
        End Try
    End Sub
    Private Sub vat_name_KeyDown(sender As Object, e As KeyEventArgs) Handles vat_name.KeyDown
        If e.KeyCode = Keys.Enter Then
            vat_percentage.Select()
        End If
    End Sub
    Private Sub vat_percentage_KeyDown(sender As Object, e As KeyEventArgs) Handles vat_percentage.KeyDown
        If e.KeyCode = Keys.Enter OrElse e.KeyCode = Keys.Down Then
            vat_type.Select()
        End If
    End Sub

    Private Sub vat_percentage_KeyPress(sender As Object, e As KeyPressEventArgs) Handles vat_percentage.KeyPress
        ' Allow only numbers, backspace, and one decimal point
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) AndAlso (e.KeyChar <> "."c) Then
            e.Handled = True
        End If

        ' Only allow one decimal point
        If (e.KeyChar = "."c) AndAlso (DirectCast(sender, TextBox).Text.IndexOf("."c) > -1) Then
            e.Handled = True
        End If
    End Sub

    Private Sub vat_percentage_TextChanged(sender As Object, e As EventArgs) Handles vat_percentage.TextChanged
        If String.IsNullOrWhiteSpace(vat_percentage.Text) Then Exit Sub

        Dim val As Decimal
        If Decimal.TryParse(vat_percentage.Text, val) Then
            If val > 100 Then
                vat_percentage.Text = "100"
                vat_percentage.SelectionStart = vat_percentage.Text.Length
            ElseIf val < 0 Then
                vat_percentage.Text = "0"
                vat_percentage.SelectionStart = vat_percentage.Text.Length
            End If
        End If
    End Sub

    Private Sub vat_type_KeyDown(sender As Object, e As KeyEventArgs) Handles vat_type.KeyDown
        If e.KeyCode = Keys.Enter Then
            vat_save.PerformClick()
        End If
    End Sub

    Private Sub PopulateSelectedVat(rowIndex As Integer)
        If rowIndex >= 0 Then
            Dim row As DataGridViewRow = VatDataGridView.Rows(rowIndex)

            ' Now "id" will be found because it's in the SELECT statement
            selectedVatID = Convert.ToInt32(row.Cells("id").Value)

            vat_name.Text = row.Cells("vat_name").Value.ToString()
            vat_percentage.Text = row.Cells("vat_value").Value.ToString()

            ' Ensure your ComboBox name matches your UI (vat_type or vat_type_combo)
            vat_type.Text = row.Cells("type").Value.ToString()

            Dim status As String = row.Cells("is_active").Value.ToString()
            If status = "Active" Then
                btn_active.Visible = False
                btn_inactive.Visible = True
            Else
                btn_active.Visible = True
                btn_inactive.Visible = False
            End If
        End If
    End Sub

    Private Sub VatDataGridView_SelectionChanged(sender As Object, e As EventArgs) Handles VatDataGridView.SelectionChanged
        If VatDataGridView.CurrentRow IsNot Nothing AndAlso VatDataGridView.CurrentRow.Index >= 0 Then
            PopulateSelectedVat(VatDataGridView.CurrentRow.Index)
        End If
    End Sub

    Private Sub VatDataGridView_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles VatDataGridView.CellFormatting
        If e.RowIndex >= 0 Then
            Dim row As DataGridViewRow = VatDataGridView.Rows(e.RowIndex)
            Dim statusValue = row.Cells("is_active").Value
            If statusValue IsNot Nothing AndAlso statusValue.ToString() = "Inactive" Then
                e.CellStyle.BackColor = Color.MistyRose
                e.CellStyle.ForeColor = Color.Red
            End If
        End If
    End Sub

    Private Sub VatDataGridView_KeyDown(sender As Object, e As KeyEventArgs) Handles VatDataGridView.KeyDown
        If e.KeyCode = Keys.Enter Then
            If VatDataGridView.CurrentRow IsNot Nothing Then
                PopulateSelectedVat(VatDataGridView.CurrentRow.Index)
                ' Removed vat_name.Focus() to allow continued arrow key navigation
                e.Handled = True
                e.SuppressKeyPress = True
            End If
        End If
    End Sub
    Private Function IsSecureKeyValid() As Boolean
        If ComboBox3.SelectedIndex = -1 Then
            MessageBox.Show("Please select user")
            ComboBox3.Focus()
            Return False
        End If

        If String.IsNullOrWhiteSpace(secure_key.Text) Then
            MessageBox.Show("You Are Not Authorized To Do That.")
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
                MessageBox.Show("You Are Not Authorized To Do That")
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
    Private Sub vat_edit_Click(sender As Object, e As EventArgs) Handles vat_edit.Click

        If selectedVatID = 0 Then
            MessageBox.Show("Please select a VAT record from the list first.")
            Exit Sub
        End If

        ' Security Check
        If Not IsSecureKeyValid() Then Exit Sub

        If vat_name.Text.Trim() = "" Or vat_percentage.Text.Trim() = "" Then
            MessageBox.Show("Fields cannot be empty.")
            Exit Sub
        End If

        Try
            If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()

            Dim Query As String = "UPDATE vat SET vat_value=@value, vat_name=@name, type=@type WHERE id=@id"

            Using COMMAND As New MySqlCommand(Query, MySqlConn)
                COMMAND.Parameters.AddWithValue("@value", vat_percentage.Text.Trim())
                COMMAND.Parameters.AddWithValue("@name", vat_name.Text.Trim())
                COMMAND.Parameters.AddWithValue("@type", vat_type.Text)
                COMMAND.Parameters.AddWithValue("@id", selectedVatID)

                COMMAND.ExecuteNonQuery()
            End Using

            MessageBox.Show("VAT Updated Successfully!")


            load_vat_Filtered(True)
            ClearInputs()

        Catch ex As Exception
            MessageBox.Show("Update Error: " & ex.Message)
        Finally
            MySqlConn.Close()
        End Try
    End Sub

    Private Sub vat_delete_Click(sender As Object, e As EventArgs) Handles vat_delete.Click
        If VatDataGridView.CurrentRow Is Nothing Then
            MessageBox.Show("Please select a customer from the list to delete.", "Selection Required")
            Exit Sub
        End If

        ' Security Check
        If Not IsSecureKeyValid() Then Exit Sub
        Dim result As DialogResult = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

        If result = DialogResult.Yes Then
            Dim selectedID As String = VatDataGridView.CurrentRow.Cells(0).Value.ToString()
            Try
                MySqlConn.Open()
                Dim Query As String = "DELETE FROM vat WHERE id = @id"
                Dim COMMAND As New MySqlCommand(Query, MySqlConn)
                COMMAND.Parameters.AddWithValue("@id", selectedID)
                COMMAND.ExecuteNonQuery()
                MessageBox.Show("Data Deleted Successfully!")
                secure_key.Clear()
            Catch ex As Exception
                MessageBox.Show("Error: " & ex.Message)
            Finally

                If MySqlConn.State = ConnectionState.Open Then
                    MySqlConn.Close()
                End If
            End Try

            load_vat_Filtered(True)
        End If
    End Sub

    Private Sub load_Users()
        Try
            If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()

            Dim table As New DataTable()
            Dim adapter As New MySqlDataAdapter("SELECT id, name FROM user WHERE (status IS NULL OR status = 'active')", MySqlConn)
            adapter.Fill(table)

            ComboBox3.DataSource = table
            ComboBox3.DisplayMember = "name"
            ComboBox3.ValueMember = "id"
            If Not String.IsNullOrEmpty(UserName) Then
                ComboBox3.Text = UserName
            Else
                ComboBox3.SelectedIndex = -1
            End If

        Catch ex As Exception
            MessageBox.Show("Error loading users: " & ex.Message)
        Finally
            MySqlConn.Close()
        End Try
    End Sub


    Private Sub VAT_add_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.F2 Then
            e.Handled = True
            e.SuppressKeyPress = True
            add_vat.PerformClick()
            vat_name.Focus()
        ElseIf e.KeyCode = Keys.F3 Then
            vat_edit.PerformClick()
        ElseIf e.KeyCode = Keys.F12 Then
            secure_key.Focus()
        ElseIf e.KeyCode = Keys.Delete Then
            vat_delete.PerformClick()
        End If
    End Sub

    Private Sub btn_active_Click(sender As Object, e As EventArgs) Handles btn_active.Click
        UpdateVatStatus(1)
    End Sub

    Private Sub btn_inactive_Click(sender As Object, e As EventArgs) Handles btn_inactive.Click
        UpdateVatStatus(0)
    End Sub

    Private Sub UpdateVatStatus(status As Integer)
        If selectedVatID = 0 Then Exit Sub

        ' Security Check
        If Not IsSecureKeyValid() Then Exit Sub

        Try
            If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()
            Dim Query As String = "UPDATE vat SET is_active = @status WHERE id = @id"
            Using COMMAND As New MySqlCommand(Query, MySqlConn)
                COMMAND.Parameters.AddWithValue("@status", status)
                COMMAND.Parameters.AddWithValue("@id", selectedVatID)
                COMMAND.ExecuteNonQuery()
            End Using

            MessageBox.Show("Status Updated Successfully!")
            load_vat_Filtered(True)
            ClearInputs()

        Catch ex As Exception
            MessageBox.Show("Status Update Error: " & ex.Message)
        Finally
            MySqlConn.Close()
        End Try
    End Sub




    Private Sub ClearInputs()
        selectedVatID = 0
        vat_name.Clear()
        vat_percentage.Clear()
        vat_type.SelectedIndex = -1
        secure_key.Clear()
        btn_active.Visible = False
        btn_inactive.Visible = False
    End Sub
End Class