Imports System.ComponentModel
Imports System.Windows.Forms.DataVisualization.Charting
Imports MySql.Data.MySqlClient

Partial Class Start
    ' Constructor to initialize components
    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()
    End Sub

    Private isLoggingOut As Boolean = False
    Private isClosing As Boolean = False
    Private sessionReleased As Boolean = False
    Public txtRgrPass As ToolStripTextBox
    Private tssOverride As ToolStripButton ' Supervisor Override button for Admin Restricted role
    Private statusCheckCounter As Integer = 0 ' Counter to run database user status check every 10 ticks (seconds)

    ' --- Navigation Control ---
    Private Sub HideCreditPanel()
        ' No longer needed - Home is now a separate MDI child form
    End Sub

    ''' <summary>
    ''' Helper to manage opening MDI child forms consistently.
    ''' Closes existing children to prevent focus/Z-order issues in maximized MDI mode.
    ''' </summary>
    Public Sub OpenMdiForm(ByVal childForm As Form)
        ' [NEW] Enforcement: Prevent accessing specialized modules if Day is not Opened
        ' Note: Always allow Home (Welcome) and DayClosing (where Opening happens)
        Dim formName As String = childForm.GetType().Name
        If Not {"Home", "DayClosing", "Form1"}.Contains(formName) Then
            If Not Module1.CheckDayOpening() Then
                MessageBox.Show("Please Open the Day (Cash Drawer) from the Day Closing menu before performing any transactions.", "Day Not Opened", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                ' Automatically route to DayClosing to help the user
                OpenMdiForm(New DayClosing())
                Return
            End If
        End If

        ' Force the welcome notice to hide immediately
        lblWelcomeNotice.Visible = False
        lblWelcomeNotice.SendToBack()

        ' Close existing children to maintain clean transition
        For Each child As Form In Me.MdiChildren
            If Not child.Equals(childForm) Then
                child.Close()
            End If
        Next

        childForm.MdiParent = Me
        childForm.ControlBox = True  ' Restores minimize and close buttons
        childForm.ShowIcon = False    ' Keeps the redundant icon hidden for a cleaner look
        childForm.AutoScroll = True

        ' Robust check to show background branding only when ALL windows are closed
        AddHandler childForm.FormClosed, Sub(s, ev)
                                             Me.BeginInvoke(Sub()
                                                                If Me.MdiChildren.Length = 0 Then
                                                                    lblWelcomeNotice.Visible = True
                                                                    lblWelcomeNotice.BringToFront()
                                                                End If
                                                            End Sub)
                                         End Sub

        childForm.WindowState = FormWindowState.Normal
        childForm.Show()
        childForm.WindowState = FormWindowState.Maximized
        childForm.BringToFront()
        childForm.Focus()

        ' Refresh layout to reclaim space from hidden label
        Me.Refresh()
    End Sub


    ' --- Event Handlers: Menu ---
    Private Function IsRestrictedAdmin() As Boolean
        Dim currentUser As String = If(Module1.UserName IsNot Nothing, Module1.UserName.Trim().ToLower(), "")
        Return currentUser = "admin1" OrElse currentUser = "admin2" OrElse currentUser = "admin3" OrElse currentUser = "admin4"
    End Function

    Private Sub HomeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles HomeToolStripMenuItem.Click
        OpenMdiForm(New Home())
    End Sub

    Private Sub ItemToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ItemToolStripMenuItem1.Click
        If IsRestrictedAdmin() Then
            MessageBox.Show("Please enter a valid Cashier ID", "Invalid Cashier", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        OpenMdiForm(Item_manage)
    End Sub

    Private Sub CategoryToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CategoryToolStripMenuItem.Click
        OpenMdiForm(category)
    End Sub

    Private Sub BrandToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BrandToolStripMenuItem.Click
        OpenMdiForm(Brand)
    End Sub

    Private Sub PurchaseEntryToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PurchaseEntryToolStripMenuItem.Click
        If IsRestrictedAdmin() Then
            MessageBox.Show("Please enter a valid Cashier ID", "Invalid Cashier", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        OpenMdiForm(PurchaEntry)
    End Sub

    Private Sub PurchaseReturnToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PurchaseReturnToolStripMenuItem.Click
        If IsRestrictedAdmin() Then
            MessageBox.Show("Please enter a valid Cashier ID", "Invalid Cashier", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        OpenMdiForm(frmPurchaseReturn)
    End Sub

    Private Sub PreviousPurchaseRequestsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PreviousPurchaseRequestsToolStripMenuItem.Click
        OpenMdiForm(PreviousPurchaseRequestsForm)
    End Sub

    Private Sub StockRequestToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StockRequestToolStripMenuItem.Click
        OpenMdiForm(StockRequest)
    End Sub

    Private Sub StockTransferToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StockTransferToolStripMenuItem.Click
        OpenMdiForm(StockTransfer)
    End Sub

    Private Sub TemSaleToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TemSaleToolStripMenuItem.Click
        OpenMdiForm(TempSales)
    End Sub

    Private Sub FinancialTerminalToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FinancialTerminalToolStripMenuItem.Click
        OpenMdiForm(PaymentCollector)
    End Sub

    Private Sub AccountingsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AccountingsToolStripMenuItem.Click
        OpenMdiForm(DailyReconciliation)
    End Sub

    Private Sub DayClosingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DayClosingToolStripMenuItem.Click
        OpenMdiForm(DayClosing)
    End Sub

    Private Sub NewSuppliersToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NewSuppliersToolStripMenuItem.Click
        If IsRestrictedAdmin() Then
            MessageBox.Show("Please enter a valid Cashier ID", "Invalid Cashier", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        OpenMdiForm(Suplier)
    End Sub

    Private Sub ChaqueToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ChaqueToolStripMenuItem.Click
        If IsRestrictedAdmin() Then
            MessageBox.Show("Please enter a valid Cashier ID", "Invalid Cashier", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        OpenMdiForm(ChaqueOut)
    End Sub

    Private Sub DebitsEntryToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DebitsEntryToolStripMenuItem.Click
        If IsRestrictedAdmin() Then
            MessageBox.Show("Please enter a valid Cashier ID", "Invalid Cashier", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        OpenMdiForm(DebitEntry)
    End Sub

    Private Sub NewToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NewToolStripMenuItem.Click
        If IsRestrictedAdmin() Then
            MessageBox.Show("Please enter a valid Cashier ID", "Invalid Cashier", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        OpenMdiForm(customer_add)
    End Sub

    Private Sub ChaqueIssueToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ChaqueIssueToolStripMenuItem.Click
        If IsRestrictedAdmin() Then
            MessageBox.Show("Please enter a valid Cashier ID", "Invalid Cashier", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        OpenMdiForm(CusCuaque)
    End Sub

    Private Sub CreditsEntryToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CreditsEntryToolStripMenuItem.Click
        If IsRestrictedAdmin() Then
            MessageBox.Show("Please enter a valid Cashier ID", "Invalid Cashier", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        OpenMdiForm(credit)
    End Sub

    Private Sub CurrentStockToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CurrentStockToolStripMenuItem.Click
        OpenMdiForm(frmStock)
    End Sub

    Private Sub BankToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BankToolStripMenuItem.Click
        OpenMdiForm(bank_add)
    End Sub

    Private Sub DailySalesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DailySalesToolStripMenuItem.Click
        OpenMdiForm(frmSales)
    End Sub

    Private Sub VatToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles VatToolStripMenuItem.Click
        OpenMdiForm(vat_add)
    End Sub

    Private Sub StockToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles StockToolStripMenuItem1.Click
        OpenMdiForm(SaleReturnlog)
    End Sub

    Private Sub BillDetailsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BillDetailsToolStripMenuItem.Click
        OpenMdiForm(BillFilter)
    End Sub

    Private Sub ReportsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReportsToolStripMenuItem.Click
        OpenMdiForm(New ReportHub())
    End Sub

    ' UserToolStripMenuItem should just drop down, no need to open a form directly

    Private Sub AddUserToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddUserToolStripMenuItem.Click
        OpenMdiForm(user)
    End Sub

    Private Sub UserDetailsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UserDetailsToolStripMenuItem.Click
        OpenMdiForm(UserManage)
    End Sub

    Private Sub PrettyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PrettyToolStripMenuItem.Click
        ' Hide branding when dialog opens
        lblWelcomeNotice.Visible = False

        ' Showing as a dialog since it's an entry form with FixedDialog border style
        PettyCashAdd.ShowDialog()

        ' Restore branding if no other MDI child remains
        If Me.MdiChildren.Length = 0 Then
            lblWelcomeNotice.Visible = True
        End If
    End Sub

    Private Sub ReleaseSession()
        ' Guard: run only once per session to prevent double-calls
        If sessionReleased Then Return
        If Not String.IsNullOrEmpty(Module1.UserName) Then
            Try
                Using localConn As New MySqlConnection(Module1.ConnStr)
                    localConn.Open()
                    Dim updateCmd As New MySqlCommand("UPDATE user SET login = 0 WHERE name = @name", localConn)
                    updateCmd.Parameters.AddWithValue("@name", Module1.UserName)
                    updateCmd.ExecuteNonQuery()
                    sessionReleased = True
                End Using
            Catch ex As Exception
                ' Silent fail on logout
            End Try
        End If
    End Sub

    Private Sub LogOutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LogOutToolStripMenuItem.Click
        If MessageBox.Show("Are you sure you want to log out?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            ReleaseSession()        ' Set login = 0 in DB
            isLoggingOut = True     ' Prevent FormClosing from calling ReleaseSession again
            
            ' Close main form first so MDI children (like TempSales) close while Module1.UserName is STILL valid
            Me.Close()

            Module1.UserName = ""
            Module1.UserRole = ""
            Form1.Show()            ' Show login screen
        End If
    End Sub



    ' --- RBAC Permissions ---
    Public Sub ApplyPermissions()
        Dim role As String = If(Module1.UserRole IsNot Nothing, Module1.UserRole.ToLower(), "")
        Dim fRole As String = If(Module1.FinancialRole IsNot Nothing, Module1.FinancialRole.ToLower(), "")

        ' Default all to visible and ensure correct labels
        MenuStrip1.Visible = True
        HomeToolStripMenuItem.Visible = True
        HomeToolStripMenuItem.Text = "Home"

        ItemToolStripMenuItem.Visible = True
        ItemToolStripMenuItem.Text = "Item"

        PurchaseToolStripMenuItem.Visible = True
        PurchaseToolStripMenuItem.Text = "Purchase"

        SaleToolStripMenuItem.Visible = True
        SaleToolStripMenuItem.Text = "Sale"

        StockToolStripMenuItem.Visible = True
        StockToolStripMenuItem.Text = "Stock"

        SupplierToolStripMenuItem.Visible = True
        SupplierToolStripMenuItem.Text = "Supplier"

        CustomerToolStripMenuItem.Visible = True
        CustomerToolStripMenuItem.Text = "Customer"

        UserToolStripMenuItem.Visible = True
        UserToolStripMenuItem.Text = "User"

        ReportsToolStripMenuItem.Visible = True
        ReportsToolStripMenuItem.Text = "Reports"

        ' Sub-menus default
        TemSaleToolStripMenuItem.Visible = True
        PrettyToolStripMenuItem.Visible = True
        FinancialTerminalToolStripMenuItem.Visible = True
        AccountingsToolStripMenuItem.Visible = True
        DayClosingToolStripMenuItem.Visible = True
        DailySalesToolStripMenuItem.Visible = True
        If role = "owner" Then
            DailySalesToolStripMenuItem.Text = "Daily Sales"
        Else
            DailySalesToolStripMenuItem.Text = "Monthly Item Sales"
        End If
        CurrentStockToolStripMenuItem.Visible = True
        StockToolStripMenuItem1.Visible = True
        BillDetailsToolStripMenuItem.Visible = True
        ChaqueToolStripMenuItem.Visible = True
        DebitsEntryToolStripMenuItem.Visible = True

        ' --- DAY OPENING RESTRICTIONS ---
        If Not Module1.IsDayOpened Then
            ' Hide everything by default if day not opened
            HomeToolStripMenuItem.Visible = False
            ItemToolStripMenuItem.Visible = False
            PurchaseToolStripMenuItem.Visible = False
            SaleToolStripMenuItem.Visible = False
            StockToolStripMenuItem.Visible = False
            SupplierToolStripMenuItem.Visible = False
            CustomerToolStripMenuItem.Visible = False
            UserToolStripMenuItem.Visible = False
            ReportsToolStripMenuItem.Visible = False

            ' If user is Admin, Owner, or Cashier, they can access Day Closing to open the day
            If role = "admin" OrElse role = "owner" OrElse role = "cashier" Then
                SaleToolStripMenuItem.Visible = True
                ' Hide all children of Sale EXCEPT Day Closing
                For Each item As ToolStripItem In SaleToolStripMenuItem.DropDownItems
                    If item.Name <> "DayClosingToolStripMenuItem" Then
                        item.Visible = False
                    Else
                        item.Visible = True
                    End If
                Next
            End If

            ' Always allow Logout
            UserToolStripMenuItem.Visible = True
            AddUserToolStripMenuItem.Visible = False
            UserDetailsToolStripMenuItem.Visible = False
            LogOutToolStripMenuItem.Visible = True

            Return ' Skip further role-based overrides if day is closed
        End If

        ' 1. Logic for CASHIER
        If role = "cashier" Then
            UserToolStripMenuItem.Visible = False

            ' NEW: Cashier + Seller (Take Order + Stock)
            If fRole = "seller" Then
                HomeToolStripMenuItem.Visible = False
                PurchaseToolStripMenuItem.Visible = True ' Enabled per user request
                SupplierToolStripMenuItem.Visible = True ' Enabled per request
                CustomerToolStripMenuItem.Visible = True ' Enabled as per user request
                StockToolStripMenuItem1.Visible = True ' Sale Return Log enabled per user request
                FinancialTerminalToolStripMenuItem.Visible = False
                AccountingsToolStripMenuItem.Visible = False
                DayClosingToolStripMenuItem.Visible = False
                PrettyToolStripMenuItem.Visible = True ' Petty Cash enabled per user request
                CurrentStockToolStripMenuItem.Visible = True ' Current Stock enabled per user request
                BillDetailsToolStripMenuItem.Visible = Module1.IsRgrVisible
                ChaqueToolStripMenuItem.Visible = True ' Enabled per request
                DebitsEntryToolStripMenuItem.Visible = True ' Enabled per request

                ' NEW: Cashier + Normal Seller (Sales + Purchase + Supplier)
            ElseIf fRole = "normal seller" Then
                CustomerToolStripMenuItem.Visible = True ' Enabled as part of additive permissions
                FinancialTerminalToolStripMenuItem.Visible = False
                AccountingsToolStripMenuItem.Visible = False
                DayClosingToolStripMenuItem.Visible = False
                PrettyToolStripMenuItem.Visible = False

                ' NEW: Cashier + Cashier (Main Cashier)
            ElseIf fRole = "cashier" Then
                ' Everything visible except User Manage and Daily Sales
            End If

            ' 2. Logic for ADMIN
        ElseIf role = "admin" Then
            UserToolStripMenuItem.Visible = False
            AccountingsToolStripMenuItem.Visible = False ' Removed per user request
            ' Admin + Admin (Full Finance) - No further restrictions needed here as defaults are visible

            ' 3. Logic for OWNER
        ElseIf role = "owner" Then
            ' Owner + Owner (Full Power) - No restrictions
            ReportsToolStripMenuItem.Visible = True
        End If

        ' --- HIDDEN MENU OVERRIDE ---
        ' If the global financial menu toggle is OFF, force hide these sensitive items
        If Not Module1.IsFinanceMenuVisible Then
            DayClosingToolStripMenuItem.Visible = False
            ' Accountings toggled per new user request
            AccountingsToolStripMenuItem.Visible = False
            ' Hide Customer and Supplier Finance sections
            CustomerToolStripMenuItem.Visible = False
            ChaqueToolStripMenuItem.Visible = False
            DebitsEntryToolStripMenuItem.Visible = False
            StockToolStripMenuItem1.Visible = False
            FinancialTerminalToolStripMenuItem.Visible = False
        End If

        ' Force hide specific tabs requested by the user
        HomeToolStripMenuItem.Visible = False
        StockRequestToolStripMenuItem.Visible = False
        StockTransferToolStripMenuItem.Visible = False
        PreviousPurchaseRequestsToolStripMenuItem.Visible = False
        PrettyToolStripMenuItem.Visible = False
        FinancialTerminalToolStripMenuItem.Visible = False
        AccountingsToolStripMenuItem.Visible = False
        CurrentStockToolStripMenuItem.Visible = False
        BillDetailsToolStripMenuItem.Visible = False
    End Sub

    ' --- Form Life-cycle ---
    Private Sub Start_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Enable KeyPreview to handle global shortcuts like Ctrl + M
        Me.KeyPreview = True

        ' Cover Task Manager kills and abnormal exits — fires even when FormClosing does not
        AddHandler AppDomain.CurrentDomain.ProcessExit, Sub(s, ev) ReleaseSession()
        AddHandler Application.ApplicationExit, Sub(s, ev) ReleaseSession()

        ' Start real-time clock immediately
        tmrClock.Interval = 1000
        tmrClock.Start()
        tssClock.Text = DateTime.Now.ToString("dd/MM/yyyy  hh:mm:ss tt")

        Me.WindowState = FormWindowState.Normal
        Me.Size = New System.Drawing.Size(1280, 960)
        Me.MinimumSize = New System.Drawing.Size(1280, 960)
        Me.StartPosition = FormStartPosition.CenterScreen
        tssUser.Text = "Logged User: " & Module1.UserName
        tssTime.Text = "Logged On At: " & DateTime.Now.ToString("hh:mm tt")

        Try
            ' --- Set UI Icon (GitHub Friendly) ---
            Dim iconName As String = "WhatsApp Image 2026-03-31 at 15.13.39 (1).ico"
            Dim iconPath As String = System.IO.Path.Combine(Application.StartupPath, iconName)
            If Not System.IO.File.Exists(iconPath) Then
                iconPath = System.IO.Path.Combine(Application.StartupPath, "..", "..", iconName)
            End If
            If System.IO.File.Exists(iconPath) Then
                Me.Icon = New Icon(iconPath)
            End If

            ' Check if day session is active
            Module1.CheckDayOpening()

            ApplyPermissions()

        ' If day not opened and user is authorized, force open DayClosing
        If Not Module1.IsDayOpened Then
            Dim role As String = If(Module1.UserRole IsNot Nothing, Module1.UserRole.ToLower(), "")
            If role = "admin" OrElse role = "owner" OrElse role = "cashier" Then
                ' Delay slightly to ensure form is ready
                Me.BeginInvoke(Sub() OpenMdiForm(New DayClosing()))
            End If
        Else
            Dim fRole As String = If(Module1.FinancialRole IsNot Nothing, Module1.FinancialRole.ToLower(), "")
            If fRole = "seller" Then
                Me.BeginInvoke(Sub() OpenMdiForm(New TempSales()))
            End If
        End If

        ' --- Center lblWelcomeNotice on form ---
            lblWelcomeNotice.Left = (Me.ClientSize.Width - lblWelcomeNotice.Width) \ 2
            lblWelcomeNotice.Top = (Me.ClientSize.Height - lblWelcomeNotice.Height) \ 2
            lblWelcomeNotice.BringToFront()

            ' --- Welcome Notice ---
            lblWelcomeNotice.BringToFront()

            ' --- MDI Background Image Support ---
            For Each ctl As Control In Me.Controls
                If TypeOf ctl Is MdiClient Then
                    ' Apply the form's BackgroundImage to the MdiClient area
                    ctl.BackColor = Me.BackColor
                    ctl.BackgroundImage = Me.BackgroundImage
                    ' Use Zoom to maintain aspect ratio and prevent the "distorted" look
                    ctl.BackgroundImageLayout = ImageLayout.Zoom
                    Exit For
                End If
            Next


            ' Status Strip Initial Text
            tssUser.Text = "Logged User: " & Module1.UserName
            tssTime.Text = "Logged On At: " & DateTime.Now.ToString("hh:mm tt")

            ' --- HIDDEN RGR PASSWORD BOX ---
            txtRgrPass = New ToolStripTextBox("txtRgrPass")
            txtRgrPass.Alignment = ToolStripItemAlignment.Right
            txtRgrPass.TextBox.PasswordChar = "*"c
            txtRgrPass.Size = New Size(30, 20)
            txtRgrPass.BackColor = MenuStrip1.BackColor
            txtRgrPass.BorderStyle = BorderStyle.None
            txtRgrPass.ForeColor = MenuStrip1.BackColor ' Stars won't be visible either
            AddHandler txtRgrPass.TextChanged, AddressOf txtRgrPass_TextChanged
            MenuStrip1.Items.Add(txtRgrPass)
        Catch ex As Exception
            Console.WriteLine("Start_Load Error: " & ex.Message)
        End Try
    End Sub

    Private Sub Start_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        ' Show management alerts on login for Admin/Owner (exclude admin1, admin2, admin3, admin4)
        Dim currentUser As String = If(Module1.UserName IsNot Nothing, Module1.UserName.Trim().ToLower(), "")
        Dim isSpecialAdmin As Boolean = (currentUser = "admin1" OrElse currentUser = "admin2" OrElse currentUser = "admin3" OrElse currentUser = "admin4")

        If Not isSpecialAdmin AndAlso (Module1.UserRole.ToLower() = "admin" OrElse Module1.UserRole.ToLower() = "owner") Then
            ManagementAlerts.ShowDialog()
        End If
    End Sub

    Private Sub tmrClock_Tick(sender As Object, e As EventArgs) Handles tmrClock.Tick
        tssClock.Text = DateTime.Now.ToString("dd/MM/yyyy  hh:mm:ss tt")
        
        ' Check user status in database every 10 seconds (ticks)
        statusCheckCounter += 1
        If statusCheckCounter >= 10 Then
            statusCheckCounter = 0
            CheckUserStatusActive()
        End If
    End Sub

    Private Sub CheckUserStatusActive()
        If Module1.CurrentUserID = 0 Then Exit Sub
        Try
            Dim status As String = "active"
            Using localConn As New MySqlConnection(Module1.ConnStr)
                localConn.Open()
                Dim sql As String = "SELECT status FROM user WHERE id = @id"
                Using cmd As New MySqlCommand(sql, localConn)
                    cmd.Parameters.AddWithValue("@id", Module1.CurrentUserID)
                    Dim res = cmd.ExecuteScalar()
                    If res IsNot Nothing AndAlso Not IsDBNull(res) Then
                        status = res.ToString().ToLower()
                    Else
                        ' If user was deleted completely from database
                        status = "deleted"
                    End If
                End Using
            End Using

            If status = "blocked" OrElse status = "deleted" Then
                tmrClock.Stop()
                MessageBox.Show("Your account has been " & status & ". You will be logged out immediately.", "Session Terminated", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                
                ' Perform logout actions
                isLoggingOut = True
                ReleaseSession()
                
                ' Close all child forms
                For Each child As Form In Me.MdiChildren
                    child.Close()
                Next
                
                ' Reset global user info
                Module1.UserRole = ""
                Module1.FinancialRole = ""
                Module1.UserName = ""
                Module1.CurrentUserID = 0
            
                Me.Hide()
                Form1.Show()
            End If
        Catch ex As Exception
            ' Silent fail to prevent application crash during temporary database disconnect
        End Try
    End Sub

    Private Sub Start_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        ' Case 1: Logout menu handled everything already
        If isLoggingOut Then Exit Sub

        ' Case 2: Prevent re-entry — Application.Exit() below re-fires FormClosing
        If isClosing Then Exit Sub
        isClosing = True

        ' Case 3: X button / taskbar close / any other close
        ' Release session (login=0) then fully shut down the app
        ReleaseSession()
        Application.Exit()  ' Needed because Form1 may still be alive in background
    End Sub

    Private Sub Start_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.Control AndAlso e.KeyCode = Keys.M Then
            ' Only allow for Admin or Owner
            Dim role As String = If(Module1.UserRole IsNot Nothing, Module1.UserRole.ToLower(), "")
            If role = "admin" OrElse role = "owner" Then
                Dim migrationForm As New DataMigrationForm()
                migrationForm.Show()
            Else
                MessageBox.Show("Access Denied: Data Migration is restricted to Administrators and Owners.", "Authorization Error", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            End If
        ElseIf e.Control AndAlso e.KeyCode = Keys.D Then
            ' Ctrl + D Toggle for visibility
            If txtRgrPass IsNot Nothing Then
                If txtRgrPass.Text = "1234" Then
                    ' Toggle BOTH RGR and Financial visibility
                    Module1.IsRgrVisible = Not Module1.IsRgrVisible
                    Module1.IsFinanceMenuVisible = Module1.IsRgrVisible ' Keep them synced for "All Hidden"

                    txtRgrPass.Text = ""
                    ApplyPermissions() ' Refresh menu visibility immediately

                    Dim statusMsg = If(Module1.IsRgrVisible, "Security Unlocked: Reports & RGR Data Visible", "Security Locked: Reports & RGR Data Hidden ")
                    MessageBox.Show(statusMsg, "Security Update", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If

                ' Refresh current child forms if any to reflect changes
                For Each child In Me.MdiChildren
                    If TypeOf child Is TempSales Then
                        CType(child, TempSales).ApplySecurityLock()
                    ElseIf TypeOf child Is frmSales Then
                        CType(child, frmSales).ApplySecurityLock()
                    ElseIf TypeOf child Is BillFilter Then
                        CType(child, BillFilter).ApplySecurityLock()
                    ElseIf TypeOf child Is ReportHub Then
                        CType(child, ReportHub).ApplySecurityLock()
                    End If
                Next

                ' Refresh all open modeless SalesHistoryForm forms
                For i As Integer = Application.OpenForms.Count - 1 To 0 Step -1
                    Dim f As Form = Application.OpenForms(i)
                    If TypeOf f Is SalesHistoryForm Then
                        CType(f, SalesHistoryForm).ApplySecurityLock()
                    End If
                Next
            Else
                ' Silent fail or small beep
                Console.Beep()
            End If
        End If
    End Sub

    Private Sub txtRgrPass_TextChanged(sender As Object, e As EventArgs)
        For Each child In Me.MdiChildren
            If TypeOf child Is TempSales Then
                CType(child, TempSales).UpdateCostColumnVisibility()
            End If
        Next
    End Sub

    Private Sub tssCalculator_Click(sender As Object, e As EventArgs) Handles tssCalculator.Click
        Try
            System.Diagnostics.Process.Start("calc.exe")
        Catch ex As Exception
            MessageBox.Show("Could not open calculator: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class
