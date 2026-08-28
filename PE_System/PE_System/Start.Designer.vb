<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Start
    Inherits System.Windows.Forms.Form

    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Start))
        Me.tmrClock = New System.Windows.Forms.Timer(Me.components)
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.HomeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ItemToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ItemToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.BrandToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CategoryToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PurchaseToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PurchaseEntryToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PurchaseReturnToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.StockRequestToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.StockTransferToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PreviousPurchaseRequestsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SaleToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TemSaleToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PrettyToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FinancialTerminalToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AccountingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DayClosingToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.StockToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DailySalesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CurrentStockToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.StockToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.BankToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.VatToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BillDetailsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SupplierToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.NewSuppliersToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ChaqueToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DebitsEntryToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CustomerToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.NewToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ChaqueIssueToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CreditsEntryToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ReportsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.UserToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AddUserToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.UserDetailsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.LogOutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.tssUser = New System.Windows.Forms.ToolStripStatusLabel()
        Me.tssSeparator1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.tssTime = New System.Windows.Forms.ToolStripStatusLabel()
        Me.tssServer = New System.Windows.Forms.ToolStripStatusLabel()
        Me.tssCalculator = New System.Windows.Forms.ToolStripStatusLabel()
        Me.tssCompany = New System.Windows.Forms.ToolStripStatusLabel()
        Me.tssVersion = New System.Windows.Forms.ToolStripStatusLabel()
        Me.tssClock = New System.Windows.Forms.ToolStripStatusLabel()
        Me.lblWelcomeNotice = New System.Windows.Forms.Label()
        Me.MenuStrip1.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'tmrClock
        '
        Me.tmrClock.Interval = 1000
        '
        'MenuStrip1
        '
        Me.MenuStrip1.AllowMerge = False
        Me.MenuStrip1.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.HomeToolStripMenuItem, Me.ItemToolStripMenuItem, Me.PurchaseToolStripMenuItem, Me.SaleToolStripMenuItem, Me.StockToolStripMenuItem, Me.SupplierToolStripMenuItem, Me.CustomerToolStripMenuItem, Me.ReportsToolStripMenuItem, Me.UserToolStripMenuItem, Me.LogOutToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Padding = New System.Windows.Forms.Padding(0)
        Me.MenuStrip1.Size = New System.Drawing.Size(1280, 24)
        Me.MenuStrip1.TabIndex = 0
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'HomeToolStripMenuItem
        '
        Me.HomeToolStripMenuItem.Name = "HomeToolStripMenuItem"
        Me.HomeToolStripMenuItem.Size = New System.Drawing.Size(64, 24)
        Me.HomeToolStripMenuItem.Text = "Home"
        '
        'ItemToolStripMenuItem
        '
        Me.ItemToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ItemToolStripMenuItem1, Me.BrandToolStripMenuItem, Me.CategoryToolStripMenuItem})
        Me.ItemToolStripMenuItem.Name = "ItemToolStripMenuItem"
        Me.ItemToolStripMenuItem.Size = New System.Drawing.Size(53, 24)
        Me.ItemToolStripMenuItem.Text = "Item"
        '
        'ItemToolStripMenuItem1
        '
        Me.ItemToolStripMenuItem1.Name = "ItemToolStripMenuItem1"
        Me.ItemToolStripMenuItem1.Size = New System.Drawing.Size(219, 26)
        Me.ItemToolStripMenuItem1.Text = "Item Mange"
        '
        'BrandToolStripMenuItem
        '
        Me.BrandToolStripMenuItem.Name = "BrandToolStripMenuItem"
        Me.BrandToolStripMenuItem.Size = New System.Drawing.Size(219, 26)
        Me.BrandToolStripMenuItem.Text = "Brand"
        '
        'CategoryToolStripMenuItem
        '
        Me.CategoryToolStripMenuItem.Name = "CategoryToolStripMenuItem"
        Me.CategoryToolStripMenuItem.Size = New System.Drawing.Size(219, 26)
        Me.CategoryToolStripMenuItem.Text = "Category/ Location"
        '
        'PurchaseToolStripMenuItem
        '
        Me.PurchaseToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.PurchaseEntryToolStripMenuItem, Me.PurchaseReturnToolStripMenuItem, Me.StockRequestToolStripMenuItem, Me.StockTransferToolStripMenuItem, Me.PreviousPurchaseRequestsToolStripMenuItem})
        Me.PurchaseToolStripMenuItem.Name = "PurchaseToolStripMenuItem"
        Me.PurchaseToolStripMenuItem.Size = New System.Drawing.Size(81, 24)
        Me.PurchaseToolStripMenuItem.Text = "Purchase"
        '
        'PurchaseEntryToolStripMenuItem
        '
        Me.PurchaseEntryToolStripMenuItem.Name = "PurchaseEntryToolStripMenuItem"
        Me.PurchaseEntryToolStripMenuItem.Size = New System.Drawing.Size(272, 26)
        Me.PurchaseEntryToolStripMenuItem.Text = "Purchase Entry"
        '
        'PurchaseReturnToolStripMenuItem
        '
        Me.PurchaseReturnToolStripMenuItem.Name = "PurchaseReturnToolStripMenuItem"
        Me.PurchaseReturnToolStripMenuItem.Size = New System.Drawing.Size(272, 26)
        Me.PurchaseReturnToolStripMenuItem.Text = "Purchase Return"
        '
        'StockRequestToolStripMenuItem
        '
        Me.StockRequestToolStripMenuItem.Name = "StockRequestToolStripMenuItem"
        Me.StockRequestToolStripMenuItem.Size = New System.Drawing.Size(272, 26)
        Me.StockRequestToolStripMenuItem.Text = "Stock Request"
        '
        'StockTransferToolStripMenuItem
        '
        Me.StockTransferToolStripMenuItem.Name = "StockTransferToolStripMenuItem"
        Me.StockTransferToolStripMenuItem.Size = New System.Drawing.Size(272, 26)
        Me.StockTransferToolStripMenuItem.Text = "Stock Transfer"
        '
        'PreviousPurchaseRequestsToolStripMenuItem
        '
        Me.PreviousPurchaseRequestsToolStripMenuItem.Name = "PreviousPurchaseRequestsToolStripMenuItem"
        Me.PreviousPurchaseRequestsToolStripMenuItem.Size = New System.Drawing.Size(272, 26)
        Me.PreviousPurchaseRequestsToolStripMenuItem.Text = "Previous Purchase Requests"
        '
        'SaleToolStripMenuItem
        '
        Me.SaleToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TemSaleToolStripMenuItem, Me.PrettyToolStripMenuItem, Me.FinancialTerminalToolStripMenuItem, Me.AccountingsToolStripMenuItem, Me.DayClosingToolStripMenuItem})
        Me.SaleToolStripMenuItem.Name = "SaleToolStripMenuItem"
        Me.SaleToolStripMenuItem.Size = New System.Drawing.Size(51, 24)
        Me.SaleToolStripMenuItem.Text = "Sale"
        '
        'TemSaleToolStripMenuItem
        '
        Me.TemSaleToolStripMenuItem.Name = "TemSaleToolStripMenuItem"
        Me.TemSaleToolStripMenuItem.Size = New System.Drawing.Size(239, 26)
        Me.TemSaleToolStripMenuItem.Text = "Temp Sale"
        '
        'PrettyToolStripMenuItem
        '
        Me.PrettyToolStripMenuItem.Name = "PrettyToolStripMenuItem"
        Me.PrettyToolStripMenuItem.Size = New System.Drawing.Size(239, 26)
        Me.PrettyToolStripMenuItem.Text = "Petty Cash"
        '
        'FinancialTerminalToolStripMenuItem
        '
        Me.FinancialTerminalToolStripMenuItem.Name = "FinancialTerminalToolStripMenuItem"
        Me.FinancialTerminalToolStripMenuItem.Size = New System.Drawing.Size(239, 26)
        Me.FinancialTerminalToolStripMenuItem.Text = "Financial Terminal"
        '
        'AccountingsToolStripMenuItem
        '
        Me.AccountingsToolStripMenuItem.Name = "AccountingsToolStripMenuItem"
        Me.AccountingsToolStripMenuItem.Size = New System.Drawing.Size(239, 26)
        Me.AccountingsToolStripMenuItem.Text = "Accountings"
        '
        'DayClosingToolStripMenuItem
        '
        Me.DayClosingToolStripMenuItem.Name = "DayClosingToolStripMenuItem"
        Me.DayClosingToolStripMenuItem.Size = New System.Drawing.Size(239, 26)
        Me.DayClosingToolStripMenuItem.Text = "Day Sessions / Drawer"
        '
        'StockToolStripMenuItem
        '
        Me.StockToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DailySalesToolStripMenuItem, Me.CurrentStockToolStripMenuItem, Me.StockToolStripMenuItem1, Me.BankToolStripMenuItem, Me.VatToolStripMenuItem, Me.BillDetailsToolStripMenuItem})
        Me.StockToolStripMenuItem.Name = "StockToolStripMenuItem"
        Me.StockToolStripMenuItem.Size = New System.Drawing.Size(59, 24)
        Me.StockToolStripMenuItem.Text = "Stock"
        '
        'DailySalesToolStripMenuItem
        '
        Me.DailySalesToolStripMenuItem.Name = "DailySalesToolStripMenuItem"
        Me.DailySalesToolStripMenuItem.Size = New System.Drawing.Size(196, 26)
        Me.DailySalesToolStripMenuItem.Text = "Daily Sales"
        '
        'CurrentStockToolStripMenuItem
        '
        Me.CurrentStockToolStripMenuItem.Name = "CurrentStockToolStripMenuItem"
        Me.CurrentStockToolStripMenuItem.Size = New System.Drawing.Size(196, 26)
        Me.CurrentStockToolStripMenuItem.Text = "Current Stock"
        '
        'StockToolStripMenuItem1
        '
        Me.StockToolStripMenuItem1.Name = "StockToolStripMenuItem1"
        Me.StockToolStripMenuItem1.Size = New System.Drawing.Size(196, 26)
        Me.StockToolStripMenuItem1.Text = "Sale Return Log"
        '
        'BankToolStripMenuItem
        '
        Me.BankToolStripMenuItem.Name = "BankToolStripMenuItem"
        Me.BankToolStripMenuItem.Size = New System.Drawing.Size(196, 26)
        Me.BankToolStripMenuItem.Text = "Bank"
        '
        'VatToolStripMenuItem
        '
        Me.VatToolStripMenuItem.Name = "VatToolStripMenuItem"
        Me.VatToolStripMenuItem.Size = New System.Drawing.Size(196, 26)
        Me.VatToolStripMenuItem.Text = "VAT"
        '
        'BillDetailsToolStripMenuItem
        '
        Me.BillDetailsToolStripMenuItem.Name = "BillDetailsToolStripMenuItem"
        Me.BillDetailsToolStripMenuItem.Size = New System.Drawing.Size(196, 26)
        Me.BillDetailsToolStripMenuItem.Text = "Bill Details"
        '
        'SupplierToolStripMenuItem
        '
        Me.SupplierToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NewSuppliersToolStripMenuItem, Me.ChaqueToolStripMenuItem, Me.DebitsEntryToolStripMenuItem})
        Me.SupplierToolStripMenuItem.Name = "SupplierToolStripMenuItem"
        Me.SupplierToolStripMenuItem.Size = New System.Drawing.Size(78, 24)
        Me.SupplierToolStripMenuItem.Text = "Supplier"
        '
        'NewSuppliersToolStripMenuItem
        '
        Me.NewSuppliersToolStripMenuItem.Name = "NewSuppliersToolStripMenuItem"
        Me.NewSuppliersToolStripMenuItem.Size = New System.Drawing.Size(181, 26)
        Me.NewSuppliersToolStripMenuItem.Text = "New Supplier"
        '
        'ChaqueToolStripMenuItem
        '
        Me.ChaqueToolStripMenuItem.Name = "ChaqueToolStripMenuItem"
        Me.ChaqueToolStripMenuItem.Size = New System.Drawing.Size(181, 26)
        Me.ChaqueToolStripMenuItem.Text = "Cheque Out"
        '
        'DebitsEntryToolStripMenuItem
        '
        Me.DebitsEntryToolStripMenuItem.Name = "DebitsEntryToolStripMenuItem"
        Me.DebitsEntryToolStripMenuItem.Size = New System.Drawing.Size(181, 26)
        Me.DebitsEntryToolStripMenuItem.Text = "Debit Entry"
        '
        'CustomerToolStripMenuItem
        '
        Me.CustomerToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NewToolStripMenuItem, Me.ChaqueIssueToolStripMenuItem, Me.CreditsEntryToolStripMenuItem})
        Me.CustomerToolStripMenuItem.Name = "CustomerToolStripMenuItem"
        Me.CustomerToolStripMenuItem.Size = New System.Drawing.Size(86, 24)
        Me.CustomerToolStripMenuItem.Text = "Customer"
        '
        'NewToolStripMenuItem
        '
        Me.NewToolStripMenuItem.Name = "NewToolStripMenuItem"
        Me.NewToolStripMenuItem.Size = New System.Drawing.Size(189, 26)
        Me.NewToolStripMenuItem.Text = "New Customer"
        '
        'ChaqueIssueToolStripMenuItem
        '
        Me.ChaqueIssueToolStripMenuItem.Name = "ChaqueIssueToolStripMenuItem"
        Me.ChaqueIssueToolStripMenuItem.Size = New System.Drawing.Size(189, 26)
        Me.ChaqueIssueToolStripMenuItem.Text = "Cheque Issue"
        '
        'CreditsEntryToolStripMenuItem
        '
        Me.CreditsEntryToolStripMenuItem.Name = "CreditsEntryToolStripMenuItem"
        Me.CreditsEntryToolStripMenuItem.Size = New System.Drawing.Size(189, 26)
        Me.CreditsEntryToolStripMenuItem.Text = "Credit Entry"
        '
        'ReportsToolStripMenuItem
        '
        Me.ReportsToolStripMenuItem.Name = "ReportsToolStripMenuItem"
        Me.ReportsToolStripMenuItem.Size = New System.Drawing.Size(74, 24)
        Me.ReportsToolStripMenuItem.Text = "Reports"
        '
        'UserToolStripMenuItem
        '
        Me.UserToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AddUserToolStripMenuItem, Me.UserDetailsToolStripMenuItem})
        Me.UserToolStripMenuItem.Name = "UserToolStripMenuItem"
        Me.UserToolStripMenuItem.Size = New System.Drawing.Size(52, 24)
        Me.UserToolStripMenuItem.Text = "User"
        '
        'AddUserToolStripMenuItem
        '
        Me.AddUserToolStripMenuItem.Name = "AddUserToolStripMenuItem"
        Me.AddUserToolStripMenuItem.Size = New System.Drawing.Size(171, 26)
        Me.AddUserToolStripMenuItem.Text = "Add User"
        '
        'UserDetailsToolStripMenuItem
        '
        Me.UserDetailsToolStripMenuItem.Name = "UserDetailsToolStripMenuItem"
        Me.UserDetailsToolStripMenuItem.Size = New System.Drawing.Size(171, 26)
        Me.UserDetailsToolStripMenuItem.Text = "User Details"
        '
        'LogOutToolStripMenuItem
        '
        Me.LogOutToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.LogOutToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(57, Byte), Integer), CType(CType(43, Byte), Integer))
        Me.LogOutToolStripMenuItem.ForeColor = System.Drawing.Color.White
        Me.LogOutToolStripMenuItem.Name = "LogOutToolStripMenuItem"
        Me.LogOutToolStripMenuItem.Size = New System.Drawing.Size(76, 24)
        Me.LogOutToolStripMenuItem.Text = "Log Out"
        '
        'StatusStrip1
        '
        Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tssUser, Me.tssSeparator1, Me.tssTime, Me.tssServer, Me.tssCalculator, Me.tssCompany, Me.tssVersion, Me.tssClock})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 935)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Padding = New System.Windows.Forms.Padding(1, 0, 19, 0)
        Me.StatusStrip1.Size = New System.Drawing.Size(1280, 26)
        Me.StatusStrip1.TabIndex = 3
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'tssUser
        '
        Me.tssUser.Name = "tssUser"
        Me.tssUser.Size = New System.Drawing.Size(100, 20)
        Me.tssUser.Text = "Logged User: "
        '
        'tssSeparator1
        '
        Me.tssSeparator1.Name = "tssSeparator1"
        Me.tssSeparator1.Size = New System.Drawing.Size(12, 20)
        Me.tssSeparator1.Text = "."
        '
        'tssTime
        '
        Me.tssTime.Name = "tssTime"
        Me.tssTime.Size = New System.Drawing.Size(171, 20)
        Me.tssTime.Text = "Logged On At: 00:00 AM"
        '
        'tssServer
        '
        Me.tssServer.Name = "tssServer"
        Me.tssServer.Size = New System.Drawing.Size(146, 20)
        Me.tssServer.Text = "Logged on To Server"
        '
        'tssCalculator
        '
        Me.tssCalculator.ActiveLinkColor = System.Drawing.Color.Red
        Me.tssCalculator.IsLink = True
        Me.tssCalculator.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline
        Me.tssCalculator.LinkColor = System.Drawing.Color.MidnightBlue
        Me.tssCalculator.Margin = New System.Windows.Forms.Padding(0, 3, 20, 2)
        Me.tssCalculator.Name = "tssCalculator"
        Me.tssCalculator.Size = New System.Drawing.Size(93, 21)
        Me.tssCalculator.Text = "☷ Calculator"
        '
        'tssCompany
        '
        Me.tssCompany.Name = "tssCompany"
        Me.tssCompany.Size = New System.Drawing.Size(446, 20)
        Me.tssCompany.Spring = True
        Me.tssCompany.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'tssVersion
        '
        Me.tssVersion.Name = "tssVersion"
        Me.tssVersion.Size = New System.Drawing.Size(98, 20)
        Me.tssVersion.Text = "Version : 1.0.0"
        '
        'tssClock
        '
        Me.tssClock.Name = "tssClock"
        Me.tssClock.Size = New System.Drawing.Size(174, 20)
        Me.tssClock.Text = "00/00/0000  00:00:00 AM"
        '
        'lblWelcomeNotice
        '
        Me.lblWelcomeNotice.BackColor = System.Drawing.Color.FromArgb(CType(CType(198, Byte), Integer), CType(CType(239, Byte), Integer), CType(CType(206, Byte), Integer))
        Me.lblWelcomeNotice.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.lblWelcomeNotice.Font = New System.Drawing.Font("Segoe UI", 13.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblWelcomeNotice.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(97, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.lblWelcomeNotice.Location = New System.Drawing.Point(0, 675)
        Me.lblWelcomeNotice.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblWelcomeNotice.Name = "lblWelcomeNotice"
        Me.lblWelcomeNotice.Padding = New System.Windows.Forms.Padding(5, 5, 5, 5)
        Me.lblWelcomeNotice.Size = New System.Drawing.Size(1280, 260)
        Me.lblWelcomeNotice.TabIndex = 16
        Me.lblWelcomeNotice.Text = "WELCOME" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "B M M SPARE PARTS"
        Me.lblWelcomeNotice.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Start
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.BackgroundImage = Global.PE_System.My.Resources.Resources.WhatsApp_Image_2026_03_31_at_15_13_38
        Me.ClientSize = New System.Drawing.Size(1280, 961)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.lblWelcomeNotice)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.IsMdiContainer = True
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Name = "Start"
        Me.Text = "Start"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents tmrClock As Timer
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents HomeToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ItemToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ItemToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents BrandToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CategoryToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents PurchaseToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents PurchaseEntryToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents PurchaseReturnToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents PreviousPurchaseRequestsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SaleToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents TemSaleToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents StockToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents BankToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents DailySalesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CurrentStockToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents StockToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents VatToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SupplierToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents NewSuppliersToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ChaqueToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents DebitsEntryToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CustomerToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents NewToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ChaqueIssueToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CreditsEntryToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents UserToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AddUserToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents LogOutToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents tssUser As ToolStripStatusLabel
    Friend WithEvents tssSeparator1 As ToolStripStatusLabel
    Friend WithEvents tssTime As ToolStripStatusLabel
    Friend WithEvents tssServer As ToolStripStatusLabel
    Friend WithEvents tssCompany As ToolStripStatusLabel
    Friend WithEvents tssVersion As ToolStripStatusLabel
    Friend WithEvents tssClock As ToolStripStatusLabel
    Friend WithEvents tssCalculator As ToolStripStatusLabel
    Friend WithEvents UserDetailsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents BillDetailsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents FinancialTerminalToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AccountingsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents DayClosingToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents StockRequestToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents StockTransferToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents PrettyToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents lblWelcomeNotice As Label
    Friend WithEvents ReportsToolStripMenuItem As ToolStripMenuItem

End Class
