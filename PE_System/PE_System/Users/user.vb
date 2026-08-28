Imports MySql.Data.MySqlClient

Public Class user
    Public Property EditUserId As Integer = 0

    Private Sub user_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        WrapAndCenterControls()
        load_roles()
        load_financial_roles()
        
        If EditUserId > 0 Then
            ' Edit Mode
            lblTitle.Text = "Edit User"
            btnAddUser.Text = "UPDATE USER"
            txtUserId.Text = EditUserId.ToString()
            LoadUserData(EditUserId)
        Else
            ' Add Mode
            lblTitle.Text = "Add New User"
            btnAddUser.Text = "ADD USER"
            generate_id()
            
            ' Default hide financial role until user type chosen
            lblFinancialRole.Visible = False
            cmbFinancialRole.Visible = False
        End If

        ' Set initial visibility
        lblHiddenKey.Visible = False
        txtHiddenKey.Visible = False
    End Sub

    Private Sub WrapAndCenterControls()
        ' Creates a container panel to hold all controls so they center naturally when form is maximized
        Dim pnlWrap As New Panel()
        pnlWrap.Size = New Size(545, 800) ' Increased height for new fields
        pnlWrap.BackColor = Color.Transparent
        
        Dim controlsToMove As New List(Of Control)
        For Each ctrl As Control In Me.Controls
            controlsToMove.Add(ctrl)
        Next
        
        For Each ctrl As Control In controlsToMove
            pnlWrap.Controls.Add(ctrl)
        Next

        Me.Controls.Add(pnlWrap)
        
        ' Center it within the current unmaximized client size
        pnlWrap.Left = (Me.ClientSize.Width - pnlWrap.Width) \ 2
        pnlWrap.Top = (Me.ClientSize.Height - pnlWrap.Height) \ 2
        
        ' Anchor None ensures it stays exactly in the center when the MDI parent maximizes it
        pnlWrap.Anchor = AnchorStyles.None
    End Sub

    Private Sub LoadUserData(ByVal id As Integer)
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()
            Dim cmd As New MySqlCommand("SELECT name, password, role_id, hiddenSecureKey, financial_role_id FROM user WHERE id = @id", conn)
            cmd.Parameters.AddWithValue("@id", id)
            Using dr As MySqlDataReader = cmd.ExecuteReader()
                If dr.Read() Then
                    txtUserName.Text = dr("name").ToString()
                    txtSecureKey.Text = dr("password").ToString()
                    cmbRole.SelectedValue = dr("role_id")
                    cmbFinancialRole.SelectedValue = If(dr("financial_role_id") Is DBNull.Value, 2, dr("financial_role_id"))
                    txtHiddenKey.Text = If(dr("hiddenSecureKey") Is DBNull.Value, "", dr("hiddenSecureKey").ToString())
                End If
            End Using
            conn.Close()
        Catch ex As Exception
            MessageBox.Show("Error loading user data: " & ex.Message)
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub load_roles()
        Try
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Open()

            Dim query As String = "SELECT id, role_name FROM user_role"
            Dim da As New MySqlDataAdapter(query, conn)
            Dim ds As New DataSet()
            da.Fill(ds, "user_role")

            cmbRole.DataSource = ds.Tables("user_role")
            cmbRole.DisplayMember = "role_name"
            cmbRole.ValueMember = "id"
            
            cmbRole.SelectedIndex = -1 ' Clear selection initially

            conn.Close()
        Catch ex As Exception
            MessageBox.Show("Error loading roles: " & ex.Message)
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub load_financial_roles()
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()
            Dim query As String = "SELECT id, f_role_name FROM financial_role"
            Dim da As New MySqlDataAdapter(query, conn)
            Dim dt As New DataTable()
            da.Fill(dt)

            cmbFinancialRole.DataSource = dt
            cmbFinancialRole.DisplayMember = "f_role_name"
            cmbFinancialRole.ValueMember = "id"
            
            cmbFinancialRole.SelectedIndex = 1 ' Default to Standard Cashier
            conn.Close()
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub cmbFinancialRole_Enter(sender As Object, e As EventArgs) Handles cmbFinancialRole.Enter
        cmbFinancialRole.DroppedDown = True
    End Sub

    Private Sub cmbFinancialRole_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbFinancialRole.SelectedIndexChanged
        ' Comprehensive Role Validation
        If cmbRole.SelectedIndex = -1 Or cmbFinancialRole.SelectedIndex = -1 Then Return

        Dim roleName As String = cmbRole.Text.ToLower()
        Dim fRoleName As String = cmbFinancialRole.Text.ToLower()

        ' 1. Cashier Restrictions: Can be Seller, Normal Seller or Cashier
        If roleName = "cashier" Then
            If fRoleName = "admin" OrElse fRoleName = "owner" Then
                MessageBox.Show("Security Alert: Cashiers cannot be assigned Admin or Owner financial roles.", "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                cmbFinancialRole.SelectedIndex = 0 ' Default to first available entry
            End If

        ' 2. Admin Restrictions: Must be Admin
        ElseIf roleName = "admin" Then
            If fRoleName <> "admin" Then
                MessageBox.Show("Security Alert: Administrators must be assigned the 'Admin' financial role.", "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                ' Try to find and select Admin automatically
                For i As Integer = 0 To cmbFinancialRole.Items.Count - 1
                    If cmbFinancialRole.GetItemText(cmbFinancialRole.Items(i)).ToLower() = "admin" Then
                        cmbFinancialRole.SelectedIndex = i
                        Exit For
                    End If
                Next
            End If

        ' 3. Owner Restrictions: Must be Owner
        ElseIf roleName = "owner" Then
            If fRoleName <> "owner" Then
                MessageBox.Show("Security Alert: Owners must be assigned the 'Owner' financial role.", "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                ' Try to find and select Owner automatically
                For i As Integer = 0 To cmbFinancialRole.Items.Count - 1
                    If cmbFinancialRole.GetItemText(cmbFinancialRole.Items(i)).ToLower() = "owner" Then
                        cmbFinancialRole.SelectedIndex = i
                        Exit For
                    End If
                Next
            End If
        End If
    End Sub

    ' Toggle visibility and validation
    Private Sub cmbRole_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbRole.SelectedIndexChanged
        If cmbRole.SelectedIndex <> -1 Then
            Dim roleName As String = cmbRole.Text.ToLower()
            Dim isAdminOrOwner As Boolean = (roleName = "admin" OrElse roleName = "owner")
            Dim isCashier As Boolean = (roleName = "cashier")
            
            ' Admin/Owner Logic
            lblHiddenKey.Visible = isAdminOrOwner
            txtHiddenKey.Visible = isAdminOrOwner
            
            ' Financial Role Logic: Visible for Admin, Owner AND Cashier
            Dim showFinancialRole As Boolean = isAdminOrOwner OrElse isCashier
            lblFinancialRole.Visible = showFinancialRole
            cmbFinancialRole.Visible = showFinancialRole
            cmbFinancialRole.Enabled = True ' Allow change

            ' If switching to Cashier, default it initially
            If isCashier Then
                cmbFinancialRole.SelectedValue = 1
            End If

            ' If switching away from admin/owner, clear the key
            If Not isAdminOrOwner Then
                txtHiddenKey.Clear()
            End If
            
            ' Trigger validation for newly swapped role
            cmbFinancialRole_SelectedIndexChanged(Nothing, Nothing)
            cmbRole.Enabled = True
        End If
    End Sub

    Private Sub cmbRole_Enter(sender As Object, e As EventArgs) Handles cmbRole.Enter
        ' Automatically drop down when user enters the control (e.g. via Enter key tab navigation)
        cmbRole.DroppedDown = True
    End Sub

    ' Lock role selection if hidden key is entered
    Private Sub txtHiddenKey_TextChanged(sender As Object, e As EventArgs) Handles txtHiddenKey.TextChanged
        Dim roleName As String = cmbRole.Text.ToLower()
        If (roleName = "admin" OrElse roleName = "owner") AndAlso Not String.IsNullOrWhiteSpace(txtHiddenKey.Text) Then
            cmbRole.Enabled = False
        Else
            cmbRole.Enabled = True
        End If
    End Sub

    Private Sub generate_id()
        If EditUserId > 0 Then Return ' Don't generate if editing
        Try
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Open()

            ' Updated to query 'user' table
            Dim cmd As New MySqlCommand("SELECT MAX(id) FROM user", conn)
            Dim result As Object = cmd.ExecuteScalar()

            If IsDBNull(result) Then
                txtUserId.Text = "1"
            Else
                txtUserId.Text = (Convert.ToInt32(result) + 1).ToString()
            End If

            conn.Close()

        Catch ex As Exception
            MessageBox.Show("Error generating ID: " & ex.Message)
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub btnAddUser_Click(sender As Object, e As EventArgs) Handles btnAddUser.Click
        If txtUserName.Text = "" Or txtSecureKey.Text = "" Or cmbRole.SelectedIndex = -1 Or cmbFinancialRole.SelectedIndex = -1 Then
            MessageBox.Show("Please fill in all required fields (Username, Password, Role, Billing Role)", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Try
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Open()

            ' Parse hiddenSecureKey safely
            Dim hKey As Object = DBNull.Value
            If Not String.IsNullOrWhiteSpace(txtHiddenKey.Text) Then
                Dim parsedKey As Integer
                If Integer.TryParse(txtHiddenKey.Text, parsedKey) AndAlso parsedKey <> 0 Then
                    hKey = parsedKey
                End If
            End If

            Dim sql As String = ""
            If EditUserId > 0 Then
                ' UPDATE Logic
                sql = "UPDATE user SET name=@name, password=@pass, role_id=@r_id, hiddenSecureKey=@h_key, financial_role_id=@f_role WHERE id=@id"
            Else
                ' INSERT Logic
                ' Check unique constraints for password & hiddenSecureKey
                Dim checkCmd As New MySqlCommand("SELECT COUNT(*) FROM user WHERE password = @pass OR (hiddenSecureKey = @h_key AND @h_key IS NOT NULL)", conn)
                checkCmd.Parameters.AddWithValue("@pass", txtSecureKey.Text.Trim())
                checkCmd.Parameters.AddWithValue("@h_key", hKey)
                Dim count As Integer = Convert.ToInt32(checkCmd.ExecuteScalar())

                If count > 0 Then
                    MessageBox.Show("Security violation: The password or hidden secure key is already in use by another user.", "Duplicate Credentials", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                    conn.Close()
                    Return
                End If
                sql = "INSERT INTO user (id, name, password, role_id, hiddenSecureKey, financial_role_id) VALUES (@id, @name, @pass, @r_id, @h_key, @f_role)"
            End If

            Dim cmd As New MySqlCommand(sql, conn)
            If EditUserId > 0 Then
                cmd.Parameters.AddWithValue("@id", EditUserId)
            Else
                cmd.Parameters.AddWithValue("@id", txtUserId.Text)
            End If
            cmd.Parameters.AddWithValue("@name", txtUserName.Text.Trim())
            cmd.Parameters.AddWithValue("@pass", txtSecureKey.Text.Trim())
            cmd.Parameters.AddWithValue("@r_id", cmbRole.SelectedValue)
            cmd.Parameters.AddWithValue("@h_key", hKey)
            cmd.Parameters.AddWithValue("@f_role", cmbFinancialRole.SelectedValue)

            cmd.ExecuteNonQuery()
            conn.Close()

            If EditUserId > 0 Then
                MessageBox.Show("User updated successfully", "Success")
                Me.Close()
            Else
                MessageBox.Show("User added successfully", "Success")
                ' Clear fields for next entry
                ClearFields()
            End If

        Catch ex As Exception
            MessageBox.Show("Error processing user: " & ex.Message)
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub ClearFields()
        txtUserName.Clear()
        txtSecureKey.Clear()
        txtHiddenKey.Clear()
        cmbRole.SelectedIndex = -1
        cmbFinancialRole.SelectedIndex = -1
        cmbRole.Enabled = True
        generate_id()
        txtUserName.Focus()
    End Sub

    ' KeyDown Navigation
    Private Sub Navigation_KeyDown(sender As Object, e As KeyEventArgs) Handles txtUserName.KeyDown, cmbRole.KeyDown, txtSecureKey.KeyDown, txtHiddenKey.KeyDown
        If e.KeyCode = Keys.Enter Then
            SendKeys.Send("{TAB}")
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Dim parentForm As Start = TryCast(Me.MdiParent, Start)
        Me.Close()
        If parentForm IsNot Nothing Then
            parentForm.OpenMdiForm(New UserManage())
        End If
    End Sub

End Class
