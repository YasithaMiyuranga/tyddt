
Imports MySql.Data.MySqlClient
Public Class bank_add
    Dim COMMAND As MySqlCommand
    Dim READER As MySqlDataReader
    Public selectedBankID As Integer = 0
    Public selectedBankName As String = ""

    Private Sub load_Banks()
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()

            Dim table As New DataTable()
            Dim adapter As New MySqlDataAdapter("select id, bank_name from bank", conn)
            adapter.Fill(table)

            Dim dv As New DataView(table)
            If Not String.IsNullOrEmpty(filter_name.Text) Then
                dv.RowFilter = String.Format("bank_name Like '%{0}%'", filter_name.Text.Replace("'", "''"))
            End If

            BankDataGridView.DataSource = dv
            bankCount.Text = table.Rows.Count
            BankDataGridView.AllowUserToAddRows = False
            BankDataGridView.ReadOnly = True
            BankDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            BankDataGridView.DefaultCellStyle.Font = New Font("Segoe UI", 12)
            BankDataGridView.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 12, FontStyle.Bold)

            If BankDataGridView.Columns.Count >= 2 Then
                BankDataGridView.Columns(0).Width = 70
                BankDataGridView.Columns(1).Width = 480
            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub

    Private Sub bank_add_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.KeyPreview = True
        load_Banks()
    End Sub

    Private Sub bank_add_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            If Me.ActiveControl Is BankDataGridView Then
                If Me.Modal Then
                    Me.DialogResult = DialogResult.OK
                    Me.Close()
                Else
                    bank_name.Focus()
                    bank_name.SelectAll()
                End If
            ElseIf TypeOf Me.ActiveControl Is Button Then
                DirectCast(Me.ActiveControl, Button).PerformClick()
            Else
                Me.SelectNextControl(Me.ActiveControl, True, True, True, True)
            End If
        ElseIf e.KeyCode = Keys.Up Then
            If Me.ActiveControl Is bank_name AndAlso BankDataGridView.CurrentRow IsNot Nothing AndAlso BankDataGridView.CurrentRow.Index > 0 Then
                BankDataGridView.CurrentCell = BankDataGridView.Rows(BankDataGridView.CurrentRow.Index - 1).Cells(1)
                e.Handled = True
            End If
        ElseIf e.KeyCode = Keys.Down Then
            If Me.ActiveControl Is bank_name AndAlso BankDataGridView.CurrentRow IsNot Nothing AndAlso BankDataGridView.CurrentRow.Index < BankDataGridView.Rows.Count - 1 Then
                BankDataGridView.CurrentCell = BankDataGridView.Rows(BankDataGridView.CurrentRow.Index + 1).Cells(1)
                e.Handled = True
            End If
        ElseIf e.KeyCode = Keys.F2 Then
            ButAdd.PerformClick()
        ElseIf e.KeyCode = Keys.F3 Then
            ButUpdate.PerformClick()
        ElseIf e.KeyCode = Keys.Delete Then
            ButDelete.PerformClick()
        End If
    End Sub

    Private Sub filter_name_TextChanged(sender As Object, e As EventArgs) Handles filter_name.TextChanged
        load_Banks()
    End Sub

    Private Sub bank_name_KeyDown(sender As Object, e As KeyEventArgs) Handles bank_name.KeyDown
        If e.KeyCode = Keys.Enter Then
            BtnSave.PerformClick()
        End If
    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles BtnSave.Click
        If bank_name.Text = "" Then
            MessageBox.Show("Bank Name Is Empty")
        Else
            Try
                conn.Open()
                COMMAND = New MySqlCommand("insert into bank(bank_name) values(@bank_name)", conn)
                COMMAND.Parameters.AddWithValue("@bank_name", bank_name.Text)
                COMMAND.ExecuteNonQuery()
                conn.Close()

                MessageBox.Show("Bank Saved Successfully")
                load_Banks()
                ClearInputs()
            Catch ex As Exception
                MessageBox.Show(ex.Message)
                If conn.State = ConnectionState.Open Then conn.Close()
            End Try
        End If
    End Sub

    Private Sub BankDataGridView_SelectionChanged(sender As Object, e As EventArgs) Handles BankDataGridView.SelectionChanged
        If BankDataGridView.CurrentRow IsNot Nothing Then
            Dim a As Integer = BankDataGridView.CurrentRow.Index
            selectedBankID = Convert.ToInt32(BankDataGridView.Rows(a).Cells(0).Value)
            selectedBankName = BankDataGridView.Rows(a).Cells(1).Value.ToString
            bank_name.Text = selectedBankName
        End If
    End Sub

    Private Sub BankDataGridView_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles BankDataGridView.CellDoubleClick
        If e.RowIndex >= 0 Then
            Dim a As Integer = e.RowIndex
            selectedBankID = Convert.ToInt32(BankDataGridView.Rows(a).Cells(0).Value)
            selectedBankName = BankDataGridView.Rows(a).Cells(1).Value.ToString
            bank_name.Text = selectedBankName
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End If
    End Sub

    Private Sub BankDataGridView_KeyDown(sender As Object, e As KeyEventArgs) Handles BankDataGridView.KeyDown
        If e.KeyCode = Keys.Enter Then
            If BankDataGridView.CurrentRow IsNot Nothing Then
                Dim a As Integer = BankDataGridView.CurrentRow.Index
                selectedBankID = Convert.ToInt32(BankDataGridView.Rows(a).Cells(0).Value)
                selectedBankName = BankDataGridView.Rows(a).Cells(1).Value.ToString
                bank_name.Text = selectedBankName
                ' Only set DialogResult and close if it's being used as a selector
                If Me.Modal Then
                    Me.DialogResult = DialogResult.OK
                    Me.Close()
                End If
                e.Handled = True
                e.SuppressKeyPress = True
            End If
        End If
    End Sub

    Private Sub ButUpdate_Click(sender As Object, e As EventArgs) Handles ButUpdate.Click
        If selectedBankID = 0 Then
            MessageBox.Show("Please select a bank from the list first.")
            Exit Sub
        End If

        If bank_name.Text = "" Then
            MessageBox.Show("Bank Name Is Empty")
            Exit Sub
        End If

        Try
            conn.Open()
            COMMAND = New MySqlCommand("update bank set bank_name=@bank_name where id=@id", conn)
            COMMAND.Parameters.AddWithValue("@bank_name", bank_name.Text)
            COMMAND.Parameters.AddWithValue("@id", selectedBankID)
            COMMAND.ExecuteNonQuery()
            conn.Close()

            MessageBox.Show("Bank Updated Successfully")
            load_Banks()
            ClearInputs()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub ClearInputs()
        selectedBankID = 0
        bank_name.Clear()
    End Sub

    Private Sub ButDelete_Click(sender As Object, e As EventArgs) Handles ButDelete.Click
        If selectedBankID = 0 Then
            MessageBox.Show("Please select a bank from the list first.")
            Exit Sub
        End If

        Dim result As DialogResult = MessageBox.Show("Are you sure you want to delete this bank?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

        If result = DialogResult.Yes Then
            Try
                conn.Open()
                COMMAND = New MySqlCommand("delete from bank where id=@id", conn)
                COMMAND.Parameters.AddWithValue("@id", selectedBankID)
                COMMAND.ExecuteNonQuery()
                conn.Close()

                MessageBox.Show("Bank Deleted Successfully")
                load_Banks()
                ClearInputs()
            Catch ex As Exception
                MessageBox.Show(ex.Message)
                If conn.State = ConnectionState.Open Then conn.Close()
            End Try
        End If
    End Sub
    Private Sub ButAdd_Click(sender As Object, e As EventArgs) Handles ButAdd.Click
        ClearInputs()
        bank_name.Focus()
    End Sub

End Class