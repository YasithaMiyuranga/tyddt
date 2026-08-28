<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DayClosing
    Inherits System.Windows.Forms.Form

    <System.Diagnostics.DebuggerNonUserCode()> _
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

    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.chkDirectOpening = New System.Windows.Forms.CheckBox()
        Me.chkDirectClosing = New System.Windows.Forms.CheckBox()
        Me.txtDirectOpening = New System.Windows.Forms.TextBox()
        Me.txtDirectClosing = New System.Windows.Forms.TextBox()
        Me.lblDirectOpeningPrompt = New System.Windows.Forms.Label()
        Me.lblDirectClosingPrompt = New System.Windows.Forms.Label()
        Me.PanelHeader = New System.Windows.Forms.Panel()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPageOpening = New System.Windows.Forms.TabPage()
        Me.btnClearOpening = New System.Windows.Forms.Button()
        Me.btnStartDay = New System.Windows.Forms.Button()
        Me.lblOpeningTotal = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.pnlOpeningDenom = New System.Windows.Forms.FlowLayoutPanel()
        Me.TabPagePettyCash = New System.Windows.Forms.TabPage()
        Me.dgvPetty = New System.Windows.Forms.DataGridView()
        Me.btnRefreshPetty = New System.Windows.Forms.Button()
        Me.btnAddPetty = New System.Windows.Forms.Button()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TabPageClosing = New System.Windows.Forms.TabPage()
        Me.TabPageHistory = New System.Windows.Forms.TabPage()
        Me.dgvHistory = New System.Windows.Forms.DataGridView()
        Me.btnRefreshHistory = New System.Windows.Forms.Button()
        Me.lblHistoryHeader = New System.Windows.Forms.Label()
        Me.btnClearClosing = New System.Windows.Forms.Button()
        Me.btnFinalClose = New System.Windows.Forms.Button()
        Me.lblVariance = New System.Windows.Forms.Label()
        Me.lblVarianceHeader = New System.Windows.Forms.Label()
        Me.lblActualPhysical = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.lblExpectedDrawer = New System.Windows.Forms.Label()
        Me.lblExpectedHeader = New System.Windows.Forms.Label()
        Me.pnlClosingDenom = New System.Windows.Forms.FlowLayoutPanel()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.lblPettyIn = New System.Windows.Forms.Label()
        Me.LabelPettyInHeader = New System.Windows.Forms.Label()
        Me.lblTotalPetty = New System.Windows.Forms.Label()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.lblNetSales = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.PanelHeader.SuspendLayout()
        Me.TabControl1.SuspendLayout()
        Me.TabPageOpening.SuspendLayout()
        Me.TabPagePettyCash.SuspendLayout()
        CType(Me.dgvPetty, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPageClosing.SuspendLayout()
        Me.TabPageHistory.SuspendLayout()
        CType(Me.dgvHistory, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelHeader
        '
        Me.PanelHeader.BackColor = System.Drawing.Color.FromArgb(CType(CType(20, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(60, Byte), Integer))
        Me.PanelHeader.Controls.Add(Me.lblTitle)
        Me.PanelHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelHeader.Location = New System.Drawing.Point(0, 0)
        Me.PanelHeader.Name = "PanelHeader"
        Me.PanelHeader.Size = New System.Drawing.Size(1264, 69)
        Me.PanelHeader.TabIndex = 0
        '
        'lblTitle
        '
        Me.lblTitle.AutoSize = True
        Me.lblTitle.Font = New System.Drawing.Font("Segoe UI", 24.0!, System.Drawing.FontStyle.Bold)
        Me.lblTitle.ForeColor = System.Drawing.Color.White
        Me.lblTitle.Location = New System.Drawing.Point(20, 10)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(484, 54)
        Me.lblTitle.TabIndex = 0
        Me.lblTitle.Text = "DRAWER && PETTY CASH"
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPageOpening)
        Me.TabControl1.Controls.Add(Me.TabPagePettyCash)
        Me.TabControl1.Controls.Add(Me.TabPageClosing)
        Me.TabControl1.Controls.Add(Me.TabPageHistory)
        Me.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl1.Location = New System.Drawing.Point(0, 69)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(1264, 531)
        Me.TabControl1.TabIndex = 1
        '
        'TabPageOpening
        '
        Me.TabPageOpening.BackColor = System.Drawing.Color.FromArgb(CType(CType(245, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(245, Byte), Integer))
        Me.TabPageOpening.Controls.Add(Me.lblDirectOpeningPrompt)
        Me.TabPageOpening.Controls.Add(Me.chkDirectOpening)
        Me.TabPageOpening.Controls.Add(Me.txtDirectOpening)
        Me.TabPageOpening.Controls.Add(Me.btnClearOpening)
        Me.TabPageOpening.Controls.Add(Me.btnStartDay)
        Me.TabPageOpening.Controls.Add(Me.lblOpeningTotal)
        Me.TabPageOpening.Controls.Add(Me.Label2)
        Me.TabPageOpening.Controls.Add(Me.pnlOpeningDenom)
        Me.TabPageOpening.Location = New System.Drawing.Point(4, 25)
        Me.TabPageOpening.Name = "TabPageOpening"
        Me.TabPageOpening.Padding = New System.Windows.Forms.Padding(20)
        Me.TabPageOpening.Size = New System.Drawing.Size(1256, 502)
        Me.TabPageOpening.TabIndex = 0
        Me.TabPageOpening.Text = "DAY OPENING"
        '
        'btnClearOpening
        '
        Me.btnClearOpening.BackColor = System.Drawing.Color.Yellow
        Me.btnClearOpening.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnClearOpening.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
        Me.btnClearOpening.ForeColor = System.Drawing.Color.Black
        Me.btnClearOpening.Location = New System.Drawing.Point(840, 345)
        Me.btnClearOpening.Name = "btnClearOpening"
        Me.btnClearOpening.Size = New System.Drawing.Size(100, 35)
        Me.btnClearOpening.TabIndex = 4
        Me.btnClearOpening.Text = "Clear All"
        Me.btnClearOpening.UseVisualStyleBackColor = False
        '
        'btnStartDay
        '
        Me.btnStartDay.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(120, Byte), Integer), CType(CType(215, Byte), Integer))
        Me.btnStartDay.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnStartDay.Font = New System.Drawing.Font("Segoe UI", 18.0!, System.Drawing.FontStyle.Bold)
        Me.btnStartDay.ForeColor = System.Drawing.Color.White
        Me.btnStartDay.Location = New System.Drawing.Point(600, 400)
        Me.btnStartDay.Name = "btnStartDay"
        Me.btnStartDay.Size = New System.Drawing.Size(350, 70)
        Me.btnStartDay.TabIndex = 3
        Me.btnStartDay.Text = "START DAY SESSION"
        Me.btnStartDay.UseVisualStyleBackColor = False
        '
        'lblOpeningTotal
        '
        Me.lblOpeningTotal.BackColor = System.Drawing.Color.FromArgb(CType(CType(33, Byte), Integer), CType(CType(37, Byte), Integer), CType(CType(41, Byte), Integer))
        Me.lblOpeningTotal.Font = New System.Drawing.Font("Segoe UI", 24.0!, System.Drawing.FontStyle.Bold)
        Me.lblOpeningTotal.ForeColor = System.Drawing.Color.Yellow
        Me.lblOpeningTotal.Location = New System.Drawing.Point(20, 400)
        Me.lblOpeningTotal.Name = "lblOpeningTotal"
        Me.lblOpeningTotal.Size = New System.Drawing.Size(550, 70)
        Me.lblOpeningTotal.TabIndex = 2
        Me.lblOpeningTotal.Text = "0.00"
        Me.lblOpeningTotal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Segoe UI", 14.0!, System.Drawing.FontStyle.Bold)
        Me.Label2.Location = New System.Drawing.Point(20, 20)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(481, 32)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Enter Opening Denominations in Drawer"
        '
        'pnlOpeningDenom
        '
        Me.pnlOpeningDenom.AutoScroll = True
        Me.pnlOpeningDenom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnlOpeningDenom.Location = New System.Drawing.Point(20, 60)
        Me.pnlOpeningDenom.Name = "pnlOpeningDenom"
        Me.pnlOpeningDenom.Size = New System.Drawing.Size(950, 330)
        Me.pnlOpeningDenom.TabIndex = 0
        '
        'chkDirectOpening
        '
        Me.chkDirectOpening.AutoSize = True
        Me.chkDirectOpening.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.chkDirectOpening.Location = New System.Drawing.Point(550, 20)
        Me.chkDirectOpening.Name = "chkDirectOpening"
        Me.chkDirectOpening.Size = New System.Drawing.Size(193, 29)
        Me.chkDirectOpening.TabIndex = 5
        Me.chkDirectOpening.Text = "Direct Total Entry"
        Me.chkDirectOpening.UseVisualStyleBackColor = True
        '
        'lblDirectOpeningPrompt
        '
        Me.lblDirectOpeningPrompt.AutoSize = True
        Me.lblDirectOpeningPrompt.Font = New System.Drawing.Font("Segoe UI", 14.0!, System.Drawing.FontStyle.Bold)
        Me.lblDirectOpeningPrompt.Location = New System.Drawing.Point(20, 80)
        Me.lblDirectOpeningPrompt.Name = "lblDirectOpeningPrompt"
        Me.lblDirectOpeningPrompt.Size = New System.Drawing.Size(350, 32)
        Me.lblDirectOpeningPrompt.TabIndex = 7
        Me.lblDirectOpeningPrompt.Text = "Enter Total Opening Amount:"
        Me.lblDirectOpeningPrompt.Visible = False
        '
        'txtDirectOpening
        '
        Me.txtDirectOpening.Font = New System.Drawing.Font("Segoe UI", 14.0!)
        Me.txtDirectOpening.Location = New System.Drawing.Point(20, 120)
        Me.txtDirectOpening.Name = "txtDirectOpening"
        Me.txtDirectOpening.Size = New System.Drawing.Size(300, 39)
        Me.txtDirectOpening.TabIndex = 6
        Me.txtDirectOpening.Text = ""
        Me.txtDirectOpening.Visible = False
        '
        'TabPagePettyCash
        '
        Me.TabPagePettyCash.Controls.Add(Me.dgvPetty)
        Me.TabPagePettyCash.Controls.Add(Me.btnRefreshPetty)
        Me.TabPagePettyCash.Controls.Add(Me.btnAddPetty)
        Me.TabPagePettyCash.Controls.Add(Me.Label4)
        Me.TabPagePettyCash.Location = New System.Drawing.Point(4, 25)
        Me.TabPagePettyCash.Name = "TabPagePettyCash"
        Me.TabPagePettyCash.Padding = New System.Windows.Forms.Padding(20)
        Me.TabPagePettyCash.Size = New System.Drawing.Size(1256, 521)
        Me.TabPagePettyCash.TabIndex = 1
        Me.TabPagePettyCash.Text = "DAILY EXPENSES (PETTY CASH)"
        Me.TabPagePettyCash.UseVisualStyleBackColor = True
        '
        'dgvPetty
        '
        Dim dataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim dataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.dgvPetty.AllowUserToAddRows = False
        Me.dgvPetty.AllowUserToDeleteRows = False
        Me.dgvPetty.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvPetty.BackgroundColor = System.Drawing.Color.White
        dataGridViewCellStyle1.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.dgvPetty.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1
        Me.dgvPetty.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dataGridViewCellStyle2.Font = New System.Drawing.Font("Segoe UI", 11.0!)
        Me.dgvPetty.DefaultCellStyle = dataGridViewCellStyle2
        Me.dgvPetty.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvPetty.Location = New System.Drawing.Point(20, 100)
        Me.dgvPetty.Name = "dgvPetty"
        Me.dgvPetty.ReadOnly = True
        Me.dgvPetty.RowHeadersWidth = 51
        Me.dgvPetty.Size = New System.Drawing.Size(1216, 400)
        Me.dgvPetty.TabIndex = 3
        '
        'btnRefreshPetty
        '
        Me.btnRefreshPetty.Location = New System.Drawing.Point(820, 40)
        Me.btnRefreshPetty.Name = "btnRefreshPetty"
        Me.btnRefreshPetty.Size = New System.Drawing.Size(150, 40)
        Me.btnRefreshPetty.TabIndex = 2
        Me.btnRefreshPetty.Text = "Refresh List"
        Me.btnRefreshPetty.UseVisualStyleBackColor = True
        '
        'btnAddPetty
        '
        Me.btnAddPetty.BackColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(135, Byte), Integer), CType(CType(84, Byte), Integer))
        Me.btnAddPetty.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnAddPetty.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.btnAddPetty.ForeColor = System.Drawing.Color.White
        Me.btnAddPetty.Location = New System.Drawing.Point(620, 40)
        Me.btnAddPetty.Name = "btnAddPetty"
        Me.btnAddPetty.Size = New System.Drawing.Size(180, 40)
        Me.btnAddPetty.TabIndex = 1
        Me.btnAddPetty.Text = "+ Add Expense"
        Me.btnAddPetty.UseVisualStyleBackColor = False
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Segoe UI", 16.0!, System.Drawing.FontStyle.Bold)
        Me.Label4.Location = New System.Drawing.Point(20, 40)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(312, 37)
        Me.Label4.TabIndex = 0
        Me.Label4.Text = "Today's Petty Cash Log"
        '
        'TabPageClosing
        '
        Me.TabPageClosing.Controls.Add(Me.lblDirectClosingPrompt)
        Me.TabPageClosing.Controls.Add(Me.chkDirectClosing)
        Me.TabPageClosing.Controls.Add(Me.txtDirectClosing)
        Me.TabPageClosing.Controls.Add(Me.btnClearClosing)
        Me.TabPageClosing.Controls.Add(Me.btnFinalClose)
        Me.TabPageClosing.Controls.Add(Me.lblVariance)
        Me.TabPageClosing.Controls.Add(Me.lblVarianceHeader)
        Me.TabPageClosing.Controls.Add(Me.lblActualPhysical)
        Me.TabPageClosing.Controls.Add(Me.Label12)
        Me.TabPageClosing.Controls.Add(Me.lblExpectedDrawer)
        Me.TabPageClosing.Controls.Add(Me.lblExpectedHeader)
        Me.TabPageClosing.Controls.Add(Me.pnlClosingDenom)
        Me.TabPageClosing.Controls.Add(Me.Label8)
        Me.TabPageClosing.Controls.Add(Me.GroupBox1)
        Me.TabPageClosing.Location = New System.Drawing.Point(4, 25)
        Me.TabPageClosing.Name = "TabPageClosing"
        Me.TabPageClosing.Padding = New System.Windows.Forms.Padding(20)
        Me.TabPageClosing.Size = New System.Drawing.Size(1256, 521)
        Me.TabPageClosing.TabIndex = 2
        Me.TabPageClosing.Text = "DAY CLOSING && RECONCILIATION"
        Me.TabPageClosing.UseVisualStyleBackColor = True
        '
        'btnClearClosing
        '
        Me.btnClearClosing.BackColor = System.Drawing.Color.Yellow
        Me.btnClearClosing.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnClearClosing.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
        Me.btnClearClosing.ForeColor = System.Drawing.Color.Black
        Me.btnClearClosing.Location = New System.Drawing.Point(470, 465)
        Me.btnClearClosing.Name = "btnClearClosing"
        Me.btnClearClosing.Size = New System.Drawing.Size(100, 35)
        Me.btnClearClosing.TabIndex = 13
        Me.btnClearClosing.Text = "Clear All"
        Me.btnClearClosing.UseVisualStyleBackColor = False
        '
        'btnFinalClose
        '
        Me.btnFinalClose.BackColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(53, Byte), Integer), CType(CType(69, Byte), Integer))
        Me.btnFinalClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnFinalClose.Font = New System.Drawing.Font("Segoe UI", 18.0!, System.Drawing.FontStyle.Bold)
        Me.btnFinalClose.ForeColor = System.Drawing.Color.White
        Me.btnFinalClose.Location = New System.Drawing.Point(620, 440)
        Me.btnFinalClose.Name = "btnFinalClose"
        Me.btnFinalClose.Size = New System.Drawing.Size(350, 60)
        Me.btnFinalClose.TabIndex = 11
        Me.btnFinalClose.Text = "CLOSE DAY SESSION"
        Me.btnFinalClose.UseVisualStyleBackColor = False
        '
        'lblVariance
        '
        Me.lblVariance.BackColor = System.Drawing.Color.FromArgb(CType(CType(33, Byte), Integer), CType(CType(37, Byte), Integer), CType(CType(41, Byte), Integer))
        Me.lblVariance.Font = New System.Drawing.Font("Segoe UI", 18.0!, System.Drawing.FontStyle.Bold)
        Me.lblVariance.ForeColor = System.Drawing.Color.White
        Me.lblVariance.Location = New System.Drawing.Point(620, 390)
        Me.lblVariance.Name = "lblVariance"
        Me.lblVariance.Size = New System.Drawing.Size(350, 30)
        Me.lblVariance.TabIndex = 10
        Me.lblVariance.Text = "0.00"
        Me.lblVariance.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblVarianceHeader
        '
        Me.lblVarianceHeader.AutoSize = True
        Me.lblVarianceHeader.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.lblVarianceHeader.Location = New System.Drawing.Point(620, 370)
        Me.lblVarianceHeader.Name = "lblVarianceHeader"
        Me.lblVarianceHeader.Size = New System.Drawing.Size(236, 25)
        Me.lblVarianceHeader.TabIndex = 9
        Me.lblVarianceHeader.Text = "Variance (Short / Excess):"
        '
        'lblActualPhysical
        '
        Me.lblActualPhysical.BackColor = System.Drawing.Color.FromArgb(CType(CType(33, Byte), Integer), CType(CType(37, Byte), Integer), CType(CType(41, Byte), Integer))
        Me.lblActualPhysical.Font = New System.Drawing.Font("Segoe UI", 18.0!, System.Drawing.FontStyle.Bold)
        Me.lblActualPhysical.ForeColor = System.Drawing.Color.Cyan
        Me.lblActualPhysical.Location = New System.Drawing.Point(620, 320)
        Me.lblActualPhysical.Name = "lblActualPhysical"
        Me.lblActualPhysical.Size = New System.Drawing.Size(350, 30)
        Me.lblActualPhysical.TabIndex = 8
        Me.lblActualPhysical.Text = "0.00"
        Me.lblActualPhysical.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.Label12.Location = New System.Drawing.Point(620, 300)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(194, 25)
        Me.Label12.TabIndex = 7
        Me.Label12.Text = "Actual Physical Cash:"
        '
        'lblExpectedDrawer
        '
        Me.lblExpectedDrawer.BackColor = System.Drawing.Color.FromArgb(CType(CType(33, Byte), Integer), CType(CType(37, Byte), Integer), CType(CType(41, Byte), Integer))
        Me.lblExpectedDrawer.Font = New System.Drawing.Font("Segoe UI", 18.0!, System.Drawing.FontStyle.Bold)
        Me.lblExpectedDrawer.ForeColor = System.Drawing.Color.Yellow
        Me.lblExpectedDrawer.Location = New System.Drawing.Point(620, 250)
        Me.lblExpectedDrawer.Name = "lblExpectedDrawer"
        Me.lblExpectedDrawer.Size = New System.Drawing.Size(350, 30)
        Me.lblExpectedDrawer.TabIndex = 6
        Me.lblExpectedDrawer.Text = "0.00"
        Me.lblExpectedDrawer.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblExpectedHeader
        '
        Me.lblExpectedHeader.AutoSize = True
        Me.lblExpectedHeader.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.lblExpectedHeader.Location = New System.Drawing.Point(620, 230)
        Me.lblExpectedHeader.Name = "lblExpectedHeader"
        Me.lblExpectedHeader.Size = New System.Drawing.Size(214, 25)
        Me.lblExpectedHeader.TabIndex = 5
        Me.lblExpectedHeader.Text = "Expected Drawer Cash:"
        '
        'pnlClosingDenom
        '
        Me.pnlClosingDenom.AutoScroll = True
        Me.pnlClosingDenom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnlClosingDenom.Location = New System.Drawing.Point(20, 60)
        Me.pnlClosingDenom.Name = "pnlClosingDenom"
        Me.pnlClosingDenom.Size = New System.Drawing.Size(580, 450)
        Me.pnlClosingDenom.TabIndex = 1
        '
        'lblDirectClosingPrompt
        '
        Me.lblDirectClosingPrompt.AutoSize = True
        Me.lblDirectClosingPrompt.Font = New System.Drawing.Font("Segoe UI", 14.0!, System.Drawing.FontStyle.Bold)
        Me.lblDirectClosingPrompt.Location = New System.Drawing.Point(20, 80)
        Me.lblDirectClosingPrompt.Name = "lblDirectClosingPrompt"
        Me.lblDirectClosingPrompt.Size = New System.Drawing.Size(350, 32)
        Me.lblDirectClosingPrompt.TabIndex = 16
        Me.lblDirectClosingPrompt.Text = "Enter Total Closing Amount:"
        Me.lblDirectClosingPrompt.Visible = False
        '
        'chkDirectClosing
        '
        Me.chkDirectClosing.AutoSize = True
        Me.chkDirectClosing.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.chkDirectClosing.Location = New System.Drawing.Point(470, 20)
        Me.chkDirectClosing.Name = "chkDirectClosing"
        Me.chkDirectClosing.Size = New System.Drawing.Size(193, 29)
        Me.chkDirectClosing.TabIndex = 14
        Me.chkDirectClosing.Text = "Direct Total Entry"
        Me.chkDirectClosing.UseVisualStyleBackColor = True
        '
        'txtDirectClosing
        '
        Me.txtDirectClosing.Font = New System.Drawing.Font("Segoe UI", 14.0!)
        Me.txtDirectClosing.Location = New System.Drawing.Point(20, 120)
        Me.txtDirectClosing.Name = "txtDirectClosing"
        Me.txtDirectClosing.Size = New System.Drawing.Size(300, 39)
        Me.txtDirectClosing.TabIndex = 15
        Me.txtDirectClosing.Text = ""
        Me.txtDirectClosing.Visible = False
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label8.Location = New System.Drawing.Point(20, 20)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(432, 28)
        Me.Label8.TabIndex = 2
        Me.Label8.Text = "Step 1: Enter Physical Denominations at End"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.lblPettyIn)
        Me.GroupBox1.Controls.Add(Me.LabelPettyInHeader)
        Me.GroupBox1.Controls.Add(Me.lblTotalPetty)
        Me.GroupBox1.Controls.Add(Me.Label18)
        Me.GroupBox1.Controls.Add(Me.lblNetSales)
        Me.GroupBox1.Controls.Add(Me.Label16)
        Me.GroupBox1.Location = New System.Drawing.Point(620, 20)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(350, 180)
        Me.GroupBox1.TabIndex = 12
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Summary Reconciliation"
        '
        'lblPettyIn
        '
        Me.lblPettyIn.AutoSize = True
        Me.lblPettyIn.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblPettyIn.Location = New System.Drawing.Point(220, 115)
        Me.lblPettyIn.Name = "lblPettyIn"
        Me.lblPettyIn.Size = New System.Drawing.Size(53, 28)
        Me.lblPettyIn.TabIndex = 18
        Me.lblPettyIn.Text = "0.00"
        Me.lblPettyIn.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'LabelPettyInHeader
        '
        Me.LabelPettyInHeader.AutoSize = True
        Me.LabelPettyInHeader.Location = New System.Drawing.Point(15, 115)
        Me.LabelPettyInHeader.Name = "LabelPettyInHeader"
        Me.LabelPettyInHeader.Size = New System.Drawing.Size(119, 17)
        Me.LabelPettyInHeader.TabIndex = 17
        Me.LabelPettyInHeader.Text = "Petty Cash IN (+):"
        '
        'lblTotalPetty
        '
        Me.lblTotalPetty.AutoSize = True
        Me.lblTotalPetty.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblTotalPetty.Location = New System.Drawing.Point(220, 75)
        Me.lblTotalPetty.Name = "lblTotalPetty"
        Me.lblTotalPetty.Size = New System.Drawing.Size(53, 28)
        Me.lblTotalPetty.TabIndex = 16
        Me.lblTotalPetty.Text = "0.00"
        Me.lblTotalPetty.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Location = New System.Drawing.Point(15, 75)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(133, 17)
        Me.Label18.TabIndex = 15
        Me.Label18.Text = "Petty Cash OUT (-):"
        '
        'lblNetSales
        '
        Me.lblNetSales.AutoSize = True
        Me.lblNetSales.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblNetSales.Location = New System.Drawing.Point(220, 35)
        Me.lblNetSales.Name = "lblNetSales"
        Me.lblNetSales.Size = New System.Drawing.Size(53, 28)
        Me.lblNetSales.TabIndex = 14
        Me.lblNetSales.Text = "0.00"
        Me.lblNetSales.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Location = New System.Drawing.Point(15, 35)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(131, 17)
        Me.Label16.TabIndex = 13
        Me.Label16.Text = "Net Cash Sales (+):"
        '
        'TabPageHistory
        '
        Me.TabPageHistory.Controls.Add(Me.dgvHistory)
        Me.TabPageHistory.Controls.Add(Me.btnRefreshHistory)
        Me.TabPageHistory.Controls.Add(Me.lblHistoryHeader)
        Me.TabPageHistory.Location = New System.Drawing.Point(4, 25)
        Me.TabPageHistory.Name = "TabPageHistory"
        Me.TabPageHistory.Padding = New System.Windows.Forms.Padding(20)
        Me.TabPageHistory.Size = New System.Drawing.Size(1256, 521)
        Me.TabPageHistory.TabIndex = 3
        Me.TabPageHistory.Text = "SESSION HISTORY"
        Me.TabPageHistory.UseVisualStyleBackColor = True
        '
        'lblHistoryHeader
        '
        Me.lblHistoryHeader.AutoSize = True
        Me.lblHistoryHeader.Font = New System.Drawing.Font("Segoe UI", 16.0!, System.Drawing.FontStyle.Bold)
        Me.lblHistoryHeader.Location = New System.Drawing.Point(20, 40)
        Me.lblHistoryHeader.Name = "lblHistoryHeader"
        Me.lblHistoryHeader.Size = New System.Drawing.Size(400, 37)
        Me.lblHistoryHeader.TabIndex = 0
        Me.lblHistoryHeader.Text = "Past Day Sessions & Cashflows"
        '
        'btnRefreshHistory
        '
        Me.btnRefreshHistory.Location = New System.Drawing.Point(820, 40)
        Me.btnRefreshHistory.Name = "btnRefreshHistory"
        Me.btnRefreshHistory.Size = New System.Drawing.Size(150, 40)
        Me.btnRefreshHistory.TabIndex = 2
        Me.btnRefreshHistory.Text = "Refresh History"
        Me.btnRefreshHistory.UseVisualStyleBackColor = True
        '
        'dgvHistory
        '
        Dim dataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim dataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.dgvHistory.AllowUserToAddRows = False
        Me.dgvHistory.AllowUserToDeleteRows = False
        Me.dgvHistory.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvHistory.BackgroundColor = System.Drawing.Color.White
        dataGridViewCellStyle3.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.dgvHistory.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3
        Me.dgvHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dataGridViewCellStyle4.Font = New System.Drawing.Font("Segoe UI", 11.0!)
        Me.dgvHistory.DefaultCellStyle = dataGridViewCellStyle4
        Me.dgvHistory.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvHistory.Location = New System.Drawing.Point(20, 100)
        Me.dgvHistory.Name = "dgvHistory"
        Me.dgvHistory.ReadOnly = True
        Me.dgvHistory.RowHeadersWidth = 51
        Me.dgvHistory.Size = New System.Drawing.Size(1216, 400)
        Me.dgvHistory.TabIndex = 3
        '
        'DayClosing
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1264, 600)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.PanelHeader)
        Me.Name = "DayClosing"
        Me.Text = "Day Sessions && Reconciliation"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.PanelHeader.ResumeLayout(False)
        Me.PanelHeader.PerformLayout()
        Me.TabControl1.ResumeLayout(False)
        Me.TabPageOpening.ResumeLayout(False)
        Me.TabPageOpening.PerformLayout()
        Me.TabPagePettyCash.ResumeLayout(False)
        Me.TabPagePettyCash.PerformLayout()
        CType(Me.dgvPetty, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPageClosing.ResumeLayout(False)
        Me.TabPageClosing.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.TabPageHistory.ResumeLayout(False)
        Me.TabPageHistory.PerformLayout()
        CType(Me.dgvHistory, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents chkDirectOpening As Windows.Forms.CheckBox
    Friend WithEvents chkDirectClosing As Windows.Forms.CheckBox
    Friend WithEvents txtDirectOpening As Windows.Forms.TextBox
    Friend WithEvents txtDirectClosing As Windows.Forms.TextBox
    Friend WithEvents lblDirectOpeningPrompt As Windows.Forms.Label
    Friend WithEvents lblDirectClosingPrompt As Windows.Forms.Label
    Friend WithEvents PanelHeader As Windows.Forms.Panel
    Friend WithEvents lblTitle As Windows.Forms.Label
    Friend WithEvents TabControl1 As Windows.Forms.TabControl
    Friend WithEvents TabPageOpening As Windows.Forms.TabPage
    Friend WithEvents TabPagePettyCash As Windows.Forms.TabPage
    Friend WithEvents TabPageClosing As Windows.Forms.TabPage
    Friend WithEvents pnlOpeningDenom As Windows.Forms.FlowLayoutPanel
    Friend WithEvents Label2 As Windows.Forms.Label
    Friend WithEvents btnClearOpening As Windows.Forms.Button
    Friend WithEvents lblOpeningTotal As Windows.Forms.Label
    Friend WithEvents btnStartDay As Windows.Forms.Button
    Friend WithEvents dgvPetty As Windows.Forms.DataGridView
    Friend WithEvents btnRefreshPetty As Windows.Forms.Button
    Friend WithEvents btnAddPetty As Windows.Forms.Button
    Friend WithEvents Label4 As Windows.Forms.Label
    Friend WithEvents Label8 As Windows.Forms.Label
    Friend WithEvents pnlClosingDenom As Windows.Forms.FlowLayoutPanel
    Friend WithEvents lblExpectedDrawer As Windows.Forms.Label
    Friend WithEvents lblExpectedHeader As Windows.Forms.Label
    Friend WithEvents lblActualPhysical As Windows.Forms.Label
    Friend WithEvents Label12 As Windows.Forms.Label
    Friend WithEvents lblVariance As Windows.Forms.Label
    Friend WithEvents lblVarianceHeader As Windows.Forms.Label
    Friend WithEvents btnClearClosing As Windows.Forms.Button
    Friend WithEvents btnFinalClose As Windows.Forms.Button
    Friend WithEvents GroupBox1 As Windows.Forms.GroupBox
    Friend WithEvents lblNetSales As Windows.Forms.Label
    Friend WithEvents Label16 As Windows.Forms.Label
    Friend WithEvents lblTotalPetty As Windows.Forms.Label
    Friend WithEvents Label18 As Windows.Forms.Label
    Friend WithEvents lblPettyIn As Windows.Forms.Label
    Friend WithEvents LabelPettyInHeader As Windows.Forms.Label
    Friend WithEvents TabPageHistory As Windows.Forms.TabPage
    Friend WithEvents dgvHistory As Windows.Forms.DataGridView
    Friend WithEvents btnRefreshHistory As Windows.Forms.Button
    Friend WithEvents lblHistoryHeader As Windows.Forms.Label
End Class
