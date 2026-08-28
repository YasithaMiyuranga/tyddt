Imports MySql.Data.MySqlClient
Public Class category
    Private Sub category_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.KeyPreview = True
        load_data()
        load_location_data()
    End Sub

    ''' <summary>
    ''' Load brands into DataGridView2
    ''' </summary>
    Private Sub load_data()
        conn.Open()
        Dim bsource As New BindingSource
        Dim table As New DataTable()
        Dim adapter As New MySqlDataAdapter("SELECT id, name FROM category ORDER BY id ASC", conn)
        adapter.Fill(table)
        bsource.DataSource = table
        DataGridView1.DataSource = table
        conn.Close()
    End Sub

    Private Sub DataGridView1_SelectionChanged(sender As Object, e As EventArgs) Handles DataGridView1.SelectionChanged
        If DataGridView1.CurrentRow IsNot Nothing Then
            Dim k As Integer = DataGridView1.CurrentRow.Index
            TextBoxCat.Tag = DataGridView1.Rows(k).Cells(0).Value.ToString
            TextBoxCat.Text = DataGridView1.Rows(k).Cells(1).Value.ToString
        End If
    End Sub

    ''' <summary>
    ''' Clear input for new brand
    ''' </summary>
    Private Sub btnAddS_Click(sender As Object, e As EventArgs) Handles btnAddS.Click
        TextBoxCat.Clear()
        TextBoxCat.Tag = Nothing
        TextBoxCat.Focus()
    End Sub

    ''' <summary>
    ''' Save new brand
    ''' </summary>
    Private Sub btnSuccessC_Click(sender As Object, e As EventArgs) Handles btnSuccessC.Click
        If String.IsNullOrWhiteSpace(TextBoxCat.Text) Then
            MessageBox.Show("category name cannot be empty.")
            Return
        End If
        conn.Open()
        Dim query As String = "INSERT INTO category (name) VALUES (@name)"
        Dim cmd As New MySqlCommand(query, conn)
        cmd.Parameters.AddWithValue("@name", TextBoxCat.Text.Trim())
        cmd.ExecuteNonQuery()
        conn.Close()
        MessageBox.Show("category added")
        TextBoxCat.Clear()
        load_data()
    End Sub

    ''' <summary>
    ''' Update existing brand
    ''' </summary>
    Private Sub btnUpdateS_Click(sender As Object, e As EventArgs) Handles btnUpdateS.Click
        If TextBoxCat.Tag Is Nothing Then
            MessageBox.Show("Please select a category to update.")
            Return
        End If

        If String.IsNullOrWhiteSpace(TextBoxCat.Text) Then
            MessageBox.Show("category name cannot be empty.")
            Return
        End If
        conn.Open()
        Dim query As String = "UPDATE category SET name = @name WHERE id = @id"
        Dim cmd As New MySqlCommand(query, conn)
        cmd.Parameters.AddWithValue("@name", TextBoxCat.Text.Trim())
        cmd.Parameters.AddWithValue("@id", TextBoxCat.Tag)
        cmd.ExecuteNonQuery()
        conn.Close()
        MessageBox.Show("category updated")
        load_data()
    End Sub

    ''' <summary>
    ''' Delete brand
    ''' </summary>
    Private Sub btnDeleteS_Click(sender As Object, e As EventArgs) Handles btnDeleteS.Click
        Dim result As DialogResult = MessageBox.Show("Are you sure you want to delete This", "OR Not", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
        If result = DialogResult.Yes Then
            Try
                conn.Open()
                Dim query As String = "DELETE FROM category WHERE id = @id"
                Dim cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@id", TextBoxCat.Tag)
                cmd.ExecuteNonQuery()
                conn.Close()
                MessageBox.Show("category delete")
                TextBoxCat.Clear()
                TextBoxCat.Tag = Nothing
                load_data()
            Catch ex As Exception
                If conn.State = ConnectionState.Open Then conn.Close()
                MessageBox.Show("Error delete category: " & ex.Message)
            End Try
        End If
    End Sub

    ' --- Location Backend Logic ---

    ''' <summary>
    ''' Load locations into DataGridView3
    ''' </summary>
    Private Sub load_location_data()
        Try
            conn.Open()
            Dim table As New DataTable()
            Dim adapter As New MySqlDataAdapter("SELECT id, location_name FROM location ORDER BY id ASC", conn)
            adapter.Fill(table)
            DataGridView3.DataSource = table
            conn.Close()
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then conn.Close()
            MessageBox.Show("Error loading locations: " & ex.Message)
        End Try
    End Sub

    Private Sub DataGridView3_SelectionChanged(sender As Object, e As EventArgs) Handles DataGridView3.SelectionChanged
        If DataGridView3.CurrentRow IsNot Nothing Then
            Dim k As Integer = DataGridView3.CurrentRow.Index
            TextBox2.Tag = DataGridView3.Rows(k).Cells(0).Value.ToString
            TextBox2.Text = DataGridView3.Rows(k).Cells(1).Value.ToString
        End If
    End Sub

    ''' <summary>
    ''' Clear input for new location (Add New/Clear)
    ''' </summary>
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        TextBox2.Clear()
        TextBox2.Tag = Nothing
        TextBox2.Focus()
    End Sub

    ''' <summary>
    ''' Save new location (Success/Save)
    ''' </summary>
    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        If String.IsNullOrWhiteSpace(TextBox2.Text) Then
            MessageBox.Show("Location name cannot be empty.")
            Return
        End If
        Try
            conn.Open()
            Dim query As String = "INSERT INTO location (location_name) VALUES (@name)"
            Dim cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@name", TextBox2.Text.Trim())
            cmd.ExecuteNonQuery()
            conn.Close()
            MessageBox.Show("Location added")
            TextBox2.Clear()
            load_location_data()
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then conn.Close()
            MessageBox.Show("Error adding location: " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Update existing location
    ''' </summary>
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        If TextBox2.Tag Is Nothing Then
            MessageBox.Show("Please select a location to update.")
            Return
        End If

        If String.IsNullOrWhiteSpace(TextBox2.Text) Then
            MessageBox.Show("Location name cannot be empty.")
            Return
        End If
        Try
            conn.Open()
            Dim query As String = "UPDATE location SET location_name = @name WHERE id = @id"
            Dim cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@name", TextBox2.Text.Trim())
            cmd.Parameters.AddWithValue("@id", TextBox2.Tag)
            cmd.ExecuteNonQuery()
            conn.Close()
            MessageBox.Show("Location updated")
            load_location_data()
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then conn.Close()
            MessageBox.Show("Error updating location: " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Delete location
    ''' </summary>
    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        If TextBox2.Tag Is Nothing Then
            MessageBox.Show("Please select a location to delete.")
            Return
        End If

        Dim result As DialogResult = MessageBox.Show("Are you sure you want to delete this location?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
        If result = DialogResult.Yes Then
            Try
                conn.Open()
                Dim query As String = "DELETE FROM location WHERE id = @id"
                Dim cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@id", TextBox2.Tag)
                cmd.ExecuteNonQuery()
                conn.Close()
                MessageBox.Show("Location deleted")
                TextBox2.Clear()
                TextBox2.Tag = Nothing
                load_location_data()
            Catch ex As Exception
                If conn.State = ConnectionState.Open Then conn.Close()
                MessageBox.Show("Error deleting location: " & ex.Message)
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
    Private Sub category_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            If Me.ActiveControl Is DataGridView1 Then
                TextBoxCat.Focus()
                TextBoxCat.SelectAll()
            ElseIf Me.ActiveControl Is DataGridView3 Then
                TextBox2.Focus()
                TextBox2.SelectAll()
            ElseIf TypeOf Me.ActiveControl Is Button Then
                DirectCast(Me.ActiveControl, Button).PerformClick()
            Else
                Me.SelectNextControl(Me.ActiveControl, True, True, True, True)
            End If
        ElseIf e.KeyCode = Keys.Up Then
            If Me.ActiveControl Is TextBoxCat AndAlso DataGridView1.CurrentRow IsNot Nothing AndAlso DataGridView1.CurrentRow.Index > 0 Then
                DataGridView1.CurrentCell = DataGridView1.Rows(DataGridView1.CurrentRow.Index - 1).Cells(1)
                e.Handled = True
            ElseIf Me.ActiveControl Is TextBox2 AndAlso DataGridView3.CurrentRow IsNot Nothing AndAlso DataGridView3.CurrentRow.Index > 0 Then
                DataGridView3.CurrentCell = DataGridView3.Rows(DataGridView3.CurrentRow.Index - 1).Cells(1)
                e.Handled = True
            End If
        ElseIf e.KeyCode = Keys.Down Then
            If Me.ActiveControl Is TextBoxCat AndAlso DataGridView1.CurrentRow IsNot Nothing AndAlso DataGridView1.CurrentRow.Index < DataGridView1.Rows.Count - 1 Then
                DataGridView1.CurrentCell = DataGridView1.Rows(DataGridView1.CurrentRow.Index + 1).Cells(1)
                e.Handled = True
            ElseIf Me.ActiveControl Is TextBox2 AndAlso DataGridView3.CurrentRow IsNot Nothing AndAlso DataGridView3.CurrentRow.Index < DataGridView3.Rows.Count - 1 Then
                DataGridView3.CurrentCell = DataGridView3.Rows(DataGridView3.CurrentRow.Index + 1).Cells(1)
                e.Handled = True
            End If
        ElseIf e.KeyCode = Keys.F2 Then
            btnAddS.PerformClick()
        ElseIf e.KeyCode = Keys.F3 Then
            btnUpdateS.PerformClick()
        ElseIf e.KeyCode = Keys.F12 Then
            btnSuccessC.PerformClick()
        ElseIf e.KeyCode = Keys.Delete Then
            btnDeleteS.PerformClick()
        End If
    End Sub

End Class