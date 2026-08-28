Imports MySql.Data.MySqlClient
Public Class Form1

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'Password hide
        TextBox4.UseSystemPasswordChar = True

        'Logo auto fit
        Panel2.AutoSize = True
        Panel2.Dock = DockStyle.Fill
        'Panel cannot take PictureBoxSizeMode. Use BackgroundImageLayout if you meant background image.
        Panel2.BackgroundImageLayout = ImageLayout.Zoom

        'Login box center
        GroupBox1.Left = (Me.ClientSize.Width - GroupBox1.Width) \ 2
        GroupBox1.Top = 10 ' Keep at top



        'Set focus to User Name
        TextBox3.Focus()

        ' Ensure required columns exist in user table
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()
            Dim columnsToCheck As String() = {"last_machine", "login", "status"}
            For Each col In columnsToCheck
                Dim checkColSql = "SELECT COUNT(*) FROM information_schema.columns WHERE table_schema = DATABASE() AND table_name = 'user' AND column_name = @col"
                Using cmdCheck As New MySqlCommand(checkColSql, conn)
                    cmdCheck.Parameters.AddWithValue("@col", col)
                    If Convert.ToInt32(cmdCheck.ExecuteScalar()) = 0 Then
                        Dim alterSql = ""
                        If col = "last_machine" Then
                            alterSql = "ALTER TABLE user ADD COLUMN last_machine VARCHAR(255) DEFAULT ''"
                        ElseIf col = "login" Then
                            alterSql = "ALTER TABLE user ADD COLUMN login INT DEFAULT 0"
                        ElseIf col = "status" Then
                            alterSql = "ALTER TABLE user ADD COLUMN status VARCHAR(50) DEFAULT 'active'"
                        End If

                        If alterSql <> "" Then
                            Using cmdAdd As New MySqlCommand(alterSql, conn)
                                cmdAdd.ExecuteNonQuery()
                            End Using
                        End If
                    End If
                End Using
            Next
        Catch ex As Exception
            ' Silent fail, if DB is down it will be caught by the login attempt anyway
        End Try

    End Sub

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        TextBox3.Focus()
    End Sub

    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles Me.Resize

        'Keep login center when resize
        GroupBox1.Left = (Me.ClientSize.Width - GroupBox1.Width) \ 2

    End Sub


    Private Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click

        If TextBox3.Text = "" Or TextBox4.Text = "" Then
            MessageBox.Show("Enter Username & Password", "Warning")
            Exit Sub
        End If

        Try
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Open()

            ' Query to fetch user role labels from both system and financial role tables (filtering out deleted users)
            Dim query As String = "SELECT r.role_name, fr.f_role_name, u.login, u.id, u.status, u.last_machine FROM user u " &
                                 "JOIN user_role r ON u.role_id = r.id " &
                                 "LEFT JOIN financial_role fr ON u.financial_role_id = fr.id " &
                                 "WHERE u.name = @name AND u.password = @pass AND (u.status IS NULL OR u.status <> 'deleted')"
            Dim cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@name", TextBox3.Text.Trim())
            cmd.Parameters.AddWithValue("@pass", TextBox4.Text.Trim())

            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.Read() Then
                    Dim roleName As String = reader("role_name").ToString()
                    Dim isLoggedIn As Integer = Convert.ToInt32(reader("login"))
                    Dim userId As Integer = Convert.ToInt32(reader("id"))
                    Dim status As String = If(reader("status") Is DBNull.Value, "active", reader("status").ToString())
                    Dim lastMachine As String = If(reader("last_machine") Is DBNull.Value, "", reader("last_machine").ToString())
                    Dim currentMachine As String = Environment.MachineName

                    If status.ToLower() = "blocked" Then
                        MessageBox.Show("Your account has been blocked. Please contact the administrator.", "Login Denied", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                        TextBox4.Clear()
                        TextBox4.Focus()
                        Return
                    End If

                    ' IF already logged in, ONLY block if it's from a DIFFERENT machine
                    If isLoggedIn = 1 AndAlso lastMachine <> currentMachine Then
                        MessageBox.Show("This user is already logged in from another PC (" & lastMachine & ").", "Login Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        TextBox4.Clear()
                        TextBox4.Focus()
                        Return
                    End If

                    Module1.UserRole = roleName
                    Module1.FinancialRole = If(reader("f_role_name") Is DBNull.Value, "Standard Cashier", reader("f_role_name").ToString())
                    Module1.UserName = TextBox3.Text.Trim()
                    Module1.CurrentUserID = userId
                    
                    ' Close reader before executing update
                    reader.Close()

                    ' Mark as logged in and update last_machine
                    Dim updateCmd As New MySqlCommand("UPDATE user SET login = 1, last_machine = @mch WHERE id = @uid", conn)
                    updateCmd.Parameters.AddWithValue("@uid", userId)
                    updateCmd.Parameters.AddWithValue("@mch", currentMachine)
                    updateCmd.ExecuteNonQuery()

                    MessageBox.Show("Login Success. Role: " & Module1.UserRole, "Welcome")

                    Me.Hide()
                    Start.Show()
                Else
                    MessageBox.Show("Invalid Username or Password", "Error")
                    TextBox4.Clear()
                    TextBox4.Focus()
                End If
            End Using

            conn.Close()
        Catch ex As Exception
            MessageBox.Show("Database Error: " & ex.Message, "Error")
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try

    End Sub


    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        TextBox3.Clear()
        TextBox4.Clear()
        TextBox3.Focus()
    End Sub


    'Press ENTER to go to password
    Private Sub TextBox3_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox3.KeyDown
        If e.KeyCode = Keys.Enter Then
            TextBox4.Focus()
            e.SuppressKeyPress = True
        End If
    End Sub

    'Press ENTER to login
    Private Sub txtPass_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox4.KeyDown
        If e.KeyCode = Keys.Enter Then
            btnLogin.PerformClick()
        End If
    End Sub

    Private Sub TextBox4_TextChanged(sender As Object, e As EventArgs) Handles TextBox4.TextChanged

    End Sub

    Private Sub lblUsername_Click(sender As Object, e As EventArgs) Handles lblUsername.Click

    End Sub
End Class
