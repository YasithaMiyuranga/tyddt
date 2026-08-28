Imports MySql.Data.MySqlClient

Public Class UserManage
    Private Sub UserManage_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadUsers()
        SetupGrid()
    End Sub

    Private Sub SetupGrid()
        dgvUsers.ReadOnly = True
        dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvUsers.AllowUserToAddRows = False
        
        ' Enable double buffering
        Try
            Dim dgvType As Type = dgvUsers.GetType()
            Dim pi As System.Reflection.PropertyInfo = dgvType.GetProperty("DoubleBuffered", 
                System.Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.NonPublic)
            pi.SetValue(dgvUsers, True, Nothing)
        Catch ex As Exception
        End try
    End Sub

    Private Sub LoadUsers()
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()
            
            Dim searchVal As String = txtSearch.Text.Trim()
            Dim query As String = "SELECT u.id as 'User ID', u.name as 'User Name', r.role_name as 'Role', " &
                                 "COALESCE(u.status, 'active') as 'Status' " &
                                 "FROM user u " &
                                 "INNER JOIN user_role r ON u.role_id = r.id " &
                                 "WHERE (u.status IS NULL OR u.status != 'deleted') "
            
            If Not String.IsNullOrEmpty(searchVal) Then
                query &= " AND (u.name LIKE @search OR r.role_name LIKE @search) "
            End If
            
            query &= " ORDER BY u.id ASC"
            
            Using cmd As New MySqlCommand(query, conn)
                If Not String.IsNullOrEmpty(searchVal) Then
                    cmd.Parameters.AddWithValue("@search", "%" & searchVal & "%")
                End If
                
                Dim da As New MySqlDataAdapter(cmd)
                Dim dt As New DataTable()
                da.Fill(dt)
                dgvUsers.DataSource = dt
            End Using
            
            conn.Close()
        Catch ex As Exception
            MessageBox.Show("Error loading users: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub txtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
        LoadUsers()
    End Sub

    Private Sub btnBlock_Click(sender As Object, e As EventArgs) Handles btnBlock.Click
        If dgvUsers.CurrentRow IsNot Nothing Then
            Dim userId As String = dgvUsers.CurrentRow.Cells("User ID").Value.ToString()
            Dim userName As String = dgvUsers.CurrentRow.Cells("User Name").Value.ToString()
            Dim currentStatus As String = dgvUsers.CurrentRow.Cells("Status").Value.ToString().ToLower()
            
            ' Prevent blocking yourself
            If userName.ToLower() = Module1.UserName.ToLower() Then
                MessageBox.Show("You cannot block your own account.", "Action Forbidden", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            
            Dim newStatus As String = If(currentStatus = "active", "blocked", "active")
            Dim actionName As String = If(newStatus = "blocked", "BLOCK", "UNBLOCK")
            
            If MessageBox.Show("Are you sure you want to " & actionName & " user '" & userName & "'?", 
                               "Confirm " & actionName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                Try
                    If conn.State = ConnectionState.Closed Then conn.Open()
                    Dim cmd As New MySqlCommand("UPDATE user SET status = @status WHERE id = @id", conn)
                    cmd.Parameters.AddWithValue("@status", newStatus)
                    cmd.Parameters.AddWithValue("@id", userId)
                    cmd.ExecuteNonQuery()
                    conn.Close()
                    
                    MessageBox.Show("User '" & userName & "' has been " & newStatus & " successfully.", "Success")
                    LoadUsers()
                Catch ex As Exception
                    MessageBox.Show("Error updating status: " & ex.Message)
                    If conn.State = ConnectionState.Open Then conn.Close()
                End Try
            End If
        End If
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If dgvUsers.CurrentRow IsNot Nothing Then
            Dim userId As String = dgvUsers.CurrentRow.Cells("User ID").Value.ToString()
            Dim userName As String = dgvUsers.CurrentRow.Cells("User Name").Value.ToString()
            
            ' Prevent deleting yourself
            If userName.ToLower() = Module1.UserName.ToLower() Then
                MessageBox.Show("You cannot delete your own account.", "Action Forbidden", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            
            If MessageBox.Show("Are you sure you want to PERMANENTLY DELETE user '" & userName & "'?" & vbCrLf & "This action cannot be undone.", 
                               "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.Yes Then
                Try
                    If conn.State = ConnectionState.Closed Then conn.Open()
                    Dim cmd As New MySqlCommand("UPDATE user SET status = 'deleted' WHERE id = @id", conn)
                    cmd.Parameters.AddWithValue("@id", userId)
                    cmd.ExecuteNonQuery()
                    conn.Close()
                    
                    MessageBox.Show("User '" & userName & "' has been permanently deleted.", "Deleted")
                    LoadUsers()
                Catch ex As Exception
                    MessageBox.Show("Error deleting user: " & ex.Message)
                    If conn.State = ConnectionState.Open Then conn.Close()
                End Try
            End If
        End If
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        If dgvUsers.CurrentRow IsNot Nothing Then
            Dim userId As Integer = Convert.ToInt32(dgvUsers.CurrentRow.Cells("User ID").Value)
            Dim editForm As New user()
            editForm.EditUserId = userId
            editForm.MdiParent = Me.MdiParent

            ' Add handler to refresh grid when edit form closes
            AddHandler editForm.FormClosed, Sub(s, args) LoadUsers()

            editForm.Show()
        Else
            MessageBox.Show("Please select a user to edit.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub dgvUsers_SelectionChanged(sender As Object, e As EventArgs) Handles dgvUsers.SelectionChanged
        If dgvUsers.CurrentRow IsNot Nothing AndAlso dgvUsers.CurrentRow.Cells("Status").Value IsNot Nothing Then
            Dim currentStatus As String = dgvUsers.CurrentRow.Cells("Status").Value.ToString().ToLower()
            If currentStatus = "blocked" Then
                btnBlock.Text = "UNBLOCK"
            Else
                btnBlock.Text = "BLOCK"
            End If
        End If
    End Sub

End Class
