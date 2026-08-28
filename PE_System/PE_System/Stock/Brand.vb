Imports MySql.Data.MySqlClient

Public Class Brand
    Private Sub Brand_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.KeyPreview = True
        load_data()
    End Sub

    ''' <summary>
    ''' Load brands into DataGridView2
    ''' </summary>
    Private Sub load_data()
        conn.Open()
        Dim bsource As New BindingSource
        Dim table As New DataTable()
        Dim adapter As New MySqlDataAdapter("SELECT id, name FROM brand ORDER BY id ASC", conn)
        adapter.Fill(table)
        bsource.DataSource = table
        DataGridView2.DataSource = table
        conn.Close()
    End Sub

    Private Sub DataGridView2_SelectionChanged(sender As Object, e As EventArgs) Handles DataGridView2.SelectionChanged
        If DataGridView2.CurrentRow IsNot Nothing Then
            Dim k As Integer = DataGridView2.CurrentRow.Index
            TextBoxBrand.Tag = DataGridView2.Rows(k).Cells(0).Value.ToString
            TextBoxBrand.Text = DataGridView2.Rows(k).Cells(1).Value.ToString
        End If
    End Sub

    ''' <summary>
    ''' Clear input for new brand
    ''' </summary>
    Private Sub ButAdd_Click(sender As Object, e As EventArgs) Handles ButAdd.Click
        TextBoxBrand.Clear()
        TextBoxBrand.Tag = Nothing
        TextBoxBrand.Focus()
    End Sub

    ''' <summary>
    ''' Save new brand
    ''' </summary>
    Private Sub ButSuccess_Click(sender As Object, e As EventArgs) Handles ButSuccess.Click
        If String.IsNullOrWhiteSpace(TextBoxBrand.Text) Then
            MessageBox.Show("Brand name cannot be empty.")
            Return
        End If
        conn.Open()
            Dim query As String = "INSERT INTO brand (name) VALUES (@name)"
            Dim cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@name", TextBoxBrand.Text.Trim())
            cmd.ExecuteNonQuery()
            conn.Close()
        MessageBox.Show("Brand added")
        TextBoxBrand.Clear()
            load_data()
    End Sub

    ''' <summary>
    ''' Update existing brand
    ''' </summary>
    Private Sub ButUpdate_Click(sender As Object, e As EventArgs) Handles ButUpdate.Click
        If TextBoxBrand.Tag Is Nothing Then
            MessageBox.Show("Please select a brand to update.")
            Return
        End If

        If String.IsNullOrWhiteSpace(TextBoxBrand.Text) Then
            MessageBox.Show("Brand name cannot be empty.")
            Return
        End If
        conn.Open()
            Dim query As String = "UPDATE brand SET name = @name WHERE id = @id"
            Dim cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@name", TextBoxBrand.Text.Trim())
            cmd.Parameters.AddWithValue("@id", TextBoxBrand.Tag)
            cmd.ExecuteNonQuery()
            conn.Close()
            MessageBox.Show("Brand updated")
            load_data()
    End Sub

    ''' <summary>
    ''' Delete brand
    ''' </summary>
    Private Sub ButDelete_Click(sender As Object, e As EventArgs) Handles ButDelete.Click
        Dim result As DialogResult = MessageBox.Show("Are you sure you want to delete This", "OR Not", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
        If result = DialogResult.Yes Then
            Try
                conn.Open()
                Dim query As String = "DELETE FROM brand WHERE id = @id"
                Dim cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@id", TextBoxBrand.Tag)
                cmd.ExecuteNonQuery()
                conn.Close()
                MessageBox.Show("Brand delete")
                TextBoxBrand.Clear()
                TextBoxBrand.Tag = Nothing
                load_data()
            Catch ex As Exception
                If conn.State = ConnectionState.Open Then conn.Close()
                MessageBox.Show("Error delete brand: " & ex.Message)
            End Try
        End If
    End Sub

    ''' <summary>
    ''' Keyboard Shortcuts:
    ''' F2  = Add New
    ''' F3  = Update
    ''' F12 = Save
    ''' Delete = Delete
    ''' </summary>
    Private Sub Brand_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            If Me.ActiveControl Is DataGridView2 Then
                TextBoxBrand.Focus()
                TextBoxBrand.SelectAll()
            ElseIf TypeOf Me.ActiveControl Is Button Then
                DirectCast(Me.ActiveControl, Button).PerformClick()
            Else
                Me.SelectNextControl(Me.ActiveControl, True, True, True, True)
            End If
        ElseIf e.KeyCode = Keys.Up Then
            If Not (TypeOf Me.ActiveControl Is DataGridView) Then
                If DataGridView2.CurrentRow IsNot Nothing AndAlso DataGridView2.CurrentRow.Index > 0 Then
                    DataGridView2.CurrentCell = DataGridView2.Rows(DataGridView2.CurrentRow.Index - 1).Cells(1)
                    e.Handled = True
                End If
            End If
        ElseIf e.KeyCode = Keys.Down Then
            If Not (TypeOf Me.ActiveControl Is DataGridView) Then
                If DataGridView2.CurrentRow IsNot Nothing AndAlso DataGridView2.CurrentRow.Index < DataGridView2.Rows.Count - 1 Then
                    DataGridView2.CurrentCell = DataGridView2.Rows(DataGridView2.CurrentRow.Index + 1).Cells(1)
                    e.Handled = True
                End If
            End If
        ElseIf e.KeyCode = Keys.F2 Then
            ButAdd.PerformClick()
        ElseIf e.KeyCode = Keys.F3 Then
            ButUpdate.PerformClick()
        ElseIf e.KeyCode = Keys.F12 Then
            ButSuccess.PerformClick()
        ElseIf e.KeyCode = Keys.Delete Then
            ButDelete.PerformClick()
        End If
    End Sub
End Class