<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PaymentCollector
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
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.btnShowPending = New System.Windows.Forms.Button()
        Me.btnShowCollected = New System.Windows.Forms.Button()
        Me.btnShowReturns = New System.Windows.Forms.Button()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.dgvPending = New System.Windows.Forms.DataGridView()
        Me.lblPendingCount = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.lblDrawerStatus = New System.Windows.Forms.Label()
        Me.chkManualChange = New System.Windows.Forms.CheckBox()
        Me.PanelTotals = New System.Windows.Forms.Panel()
        Me.lblGrandTotal = New System.Windows.Forms.Label()
        Me.txtInvDiscount = New System.Windows.Forms.TextBox()
        Me.txtOurDiscount = New System.Windows.Forms.TextBox()
        Me.lblTotalAmount = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.PanelMethods = New System.Windows.Forms.Panel()
        Me.txtCash = New System.Windows.Forms.TextBox()
        Me.cmbType = New System.Windows.Forms.ComboBox()
        Me.cmbPayMethod = New System.Windows.Forms.ComboBox()
        Me.cmbBillingType = New System.Windows.Forms.ComboBox()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.PanelBalances = New System.Windows.Forms.Panel()
        Me.chkChangeGiven = New System.Windows.Forms.CheckBox()
        Me.lblTotalBalance = New System.Windows.Forms.Label()
        Me.lblCreditBalance = New System.Windows.Forms.Label()
        Me.lblChangeAmount = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.lblBreakdown = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.btnCollect = New System.Windows.Forms.Button()
        Me.lblInvNo = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblInvoiceType = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.p_num = New System.Windows.Forms.TextBox()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.dgvPending, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        Me.PanelTotals.SuspendLayout()
        Me.PanelMethods.SuspendLayout()
        Me.PanelBalances.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnShowPending
        '
        Me.btnShowPending.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(122, Byte), Integer), CType(CType(204, Byte), Integer))
        Me.btnShowPending.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnShowPending.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnShowPending.Font = New System.Drawing.Font("Segoe UI Semibold", 10.0!, System.Drawing.FontStyle.Bold)
        Me.btnShowPending.ForeColor = System.Drawing.Color.White
        Me.btnShowPending.Location = New System.Drawing.Point(11, 10)
        Me.btnShowPending.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.btnShowPending.Name = "btnShowPending"
        Me.btnShowPending.Size = New System.Drawing.Size(180, 46)
        Me.btnShowPending.TabIndex = 2
        Me.btnShowPending.Text = "PENDING LIST"
        Me.btnShowPending.UseVisualStyleBackColor = False
        '
        'btnShowCollected
        '
        Me.btnShowCollected.BackColor = System.Drawing.Color.Gray
        Me.btnShowCollected.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnShowCollected.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnShowCollected.Font = New System.Drawing.Font("Segoe UI Semibold", 10.0!, System.Drawing.FontStyle.Bold)
        Me.btnShowCollected.ForeColor = System.Drawing.Color.White
        Me.btnShowCollected.Location = New System.Drawing.Point(200, 10)
        Me.btnShowCollected.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.btnShowCollected.Name = "btnShowCollected"
        Me.btnShowCollected.Size = New System.Drawing.Size(180, 46)
        Me.btnShowCollected.TabIndex = 3
        Me.btnShowCollected.Text = "COLLECTED LIST"
        Me.btnShowCollected.UseVisualStyleBackColor = False
        '
        'btnShowReturns
        '
        Me.btnShowReturns.BackColor = System.Drawing.Color.FromArgb(CType(CType(180, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.btnShowReturns.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnShowReturns.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnShowReturns.Font = New System.Drawing.Font("Segoe UI Semibold", 10.0!, System.Drawing.FontStyle.Bold)
        Me.btnShowReturns.ForeColor = System.Drawing.Color.White
        Me.btnShowReturns.Location = New System.Drawing.Point(389, 10)
        Me.btnShowReturns.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.btnShowReturns.Name = "btnShowReturns"
        Me.btnShowReturns.Size = New System.Drawing.Size(180, 46)
        Me.btnShowReturns.TabIndex = 4
        Me.btnShowReturns.Text = "RETURN LIST"
        Me.btnShowReturns.UseVisualStyleBackColor = False
        '
        'btnDelete
        '
        Me.btnDelete.BackColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(53, Byte), Integer), CType(CType(69, Byte), Integer))
        Me.btnDelete.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnDelete.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.btnDelete.ForeColor = System.Drawing.Color.White
        Me.btnDelete.Location = New System.Drawing.Point(1132, 320)
        Me.btnDelete.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(133, 94)
        Me.btnDelete.TabIndex = 45
        Me.btnDelete.Text = "DELETE"
        Me.btnDelete.UseVisualStyleBackColor = False
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.btnShowPending)
        Me.SplitContainer1.Panel1.Controls.Add(Me.btnShowCollected)
        Me.SplitContainer1.Panel1.Controls.Add(Me.btnShowReturns)
        Me.SplitContainer1.Panel1.Controls.Add(Me.dgvPending)
        Me.SplitContainer1.Panel1.Controls.Add(Me.lblPendingCount)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.GroupBox1)
        Me.SplitContainer1.Size = New System.Drawing.Size(1665, 862)
        Me.SplitContainer1.SplitterDistance = 461
        Me.SplitContainer1.SplitterWidth = 5
        Me.SplitContainer1.TabIndex = 0
        '
        'dgvPending
        '
        Me.dgvPending.AllowUserToAddRows = False
        Me.dgvPending.AllowUserToDeleteRows = False
        Me.dgvPending.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.dgvPending.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvPending.BackgroundColor = System.Drawing.Color.White
        Me.dgvPending.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Segoe UI", 11.0!)
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(120, Byte), Integer), CType(CType(215, Byte), Integer))
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.dgvPending.DefaultCellStyle = DataGridViewCellStyle2
        Me.dgvPending.Location = New System.Drawing.Point(0, 65)
        Me.dgvPending.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.dgvPending.MultiSelect = False
        Me.dgvPending.Name = "dgvPending"
        Me.dgvPending.ReadOnly = True
        Me.dgvPending.RowHeadersVisible = False
        Me.dgvPending.RowHeadersWidth = 51
        Me.dgvPending.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvPending.Size = New System.Drawing.Size(1665, 396)
        Me.dgvPending.TabIndex = 0
        '
        'lblPendingCount
        '
        Me.lblPendingCount.BackColor = System.Drawing.Color.FromArgb(CType(CType(33, Byte), Integer), CType(CType(37, Byte), Integer), CType(CType(41, Byte), Integer))
        Me.lblPendingCount.Dock = System.Windows.Forms.DockStyle.Top
        Me.lblPendingCount.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblPendingCount.ForeColor = System.Drawing.Color.White
        Me.lblPendingCount.Location = New System.Drawing.Point(0, 0)
        Me.lblPendingCount.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblPendingCount.Name = "lblPendingCount"
        Me.lblPendingCount.Size = New System.Drawing.Size(1665, 65)
        Me.lblPendingCount.TabIndex = 1
        Me.lblPendingCount.Text = "PENDING LIST"
        Me.lblPendingCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'GroupBox1
        '
        Me.GroupBox1.BackColor = System.Drawing.Color.FromArgb(CType(CType(43, Byte), Integer), CType(CType(81, Byte), Integer), CType(CType(94, Byte), Integer))
        Me.GroupBox1.Controls.Add(Me.lblDrawerStatus)
        Me.GroupBox1.Controls.Add(Me.chkManualChange)
        Me.GroupBox1.Controls.Add(Me.btnDelete)
        Me.GroupBox1.Controls.Add(Me.PanelTotals)
        Me.GroupBox1.Controls.Add(Me.PanelMethods)
        Me.GroupBox1.Controls.Add(Me.PanelBalances)
        Me.GroupBox1.Controls.Add(Me.lblBreakdown)
        Me.GroupBox1.Controls.Add(Me.Label6)
        Me.GroupBox1.Controls.Add(Me.btnCollect)
        Me.GroupBox1.Controls.Add(Me.lblInvNo)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Font = New System.Drawing.Font("Segoe UI", 11.0!)
        Me.GroupBox1.ForeColor = System.Drawing.Color.White
        Me.GroupBox1.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.GroupBox1.Size = New System.Drawing.Size(1665, 486)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Payment Processing Terminal"
        '
        'lblDrawerStatus
        '
        Me.lblDrawerStatus.AutoSize = True
        Me.lblDrawerStatus.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Italic)
        Me.lblDrawerStatus.ForeColor = System.Drawing.Color.LightGray
        Me.lblDrawerStatus.Location = New System.Drawing.Point(9, 418)
        Me.lblDrawerStatus.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblDrawerStatus.Name = "lblDrawerStatus"
        Me.lblDrawerStatus.Size = New System.Drawing.Size(208, 19)
        Me.lblDrawerStatus.TabIndex = 11
        Me.lblDrawerStatus.Text = "Current Drawer Stock: Loading..."
        '
        'chkManualChange
        '
        Me.chkManualChange.AutoSize = True
        Me.chkManualChange.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold)
        Me.chkManualChange.ForeColor = System.Drawing.Color.Orange
        Me.chkManualChange.Location = New System.Drawing.Point(389, 286)
        Me.chkManualChange.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.chkManualChange.Name = "chkManualChange"
        Me.chkManualChange.Size = New System.Drawing.Size(160, 24)
        Me.chkManualChange.TabIndex = 9
        Me.chkManualChange.Text = "Use Other Method"
        Me.chkManualChange.UseVisualStyleBackColor = True
        '
        'PanelTotals
        '
        Me.PanelTotals.BackColor = System.Drawing.Color.FromArgb(CType(CType(33, Byte), Integer), CType(CType(37, Byte), Integer), CType(CType(41, Byte), Integer))
        Me.PanelTotals.Controls.Add(Me.lblGrandTotal)
        Me.PanelTotals.Controls.Add(Me.txtInvDiscount)
        Me.PanelTotals.Controls.Add(Me.txtOurDiscount)
        Me.PanelTotals.Controls.Add(Me.lblTotalAmount)
        Me.PanelTotals.Controls.Add(Me.Label13)
        Me.PanelTotals.Controls.Add(Me.Label15)
        Me.PanelTotals.Controls.Add(Me.Label14)
        Me.PanelTotals.Controls.Add(Me.Label12)
        Me.PanelTotals.Location = New System.Drawing.Point(1273, 49)
        Me.PanelTotals.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.PanelTotals.Name = "PanelTotals"
        Me.PanelTotals.Size = New System.Drawing.Size(365, 382)
        Me.PanelTotals.TabIndex = 44
        '
        'lblGrandTotal
        '
        Me.lblGrandTotal.BackColor = System.Drawing.Color.Yellow
        Me.lblGrandTotal.Font = New System.Drawing.Font("Segoe UI", 24.0!, System.Drawing.FontStyle.Bold)
        Me.lblGrandTotal.ForeColor = System.Drawing.Color.Black
        Me.lblGrandTotal.Location = New System.Drawing.Point(19, 298)
        Me.lblGrandTotal.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblGrandTotal.Name = "lblGrandTotal"
        Me.lblGrandTotal.Size = New System.Drawing.Size(333, 65)
        Me.lblGrandTotal.TabIndex = 21
        Me.lblGrandTotal.Text = "0.00"
        Me.lblGrandTotal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'txtInvDiscount
        '
        Me.txtInvDiscount.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.txtInvDiscount.Location = New System.Drawing.Point(20, 222)
        Me.txtInvDiscount.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.txtInvDiscount.Name = "txtInvDiscount"
        Me.txtInvDiscount.Size = New System.Drawing.Size(105, 34)
        Me.txtInvDiscount.TabIndex = 26
        Me.txtInvDiscount.Text = "0"
        Me.txtInvDiscount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'txtOurDiscount
        '
        Me.txtOurDiscount.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.txtOurDiscount.Location = New System.Drawing.Point(20, 144)
        Me.txtOurDiscount.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.txtOurDiscount.Name = "txtOurDiscount"
        Me.txtOurDiscount.Size = New System.Drawing.Size(105, 34)
        Me.txtOurDiscount.TabIndex = 24
        Me.txtOurDiscount.Text = "0"
        Me.txtOurDiscount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'lblTotalAmount
        '
        Me.lblTotalAmount.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.lblTotalAmount.Font = New System.Drawing.Font("Segoe UI", 16.0!, System.Drawing.FontStyle.Bold)
        Me.lblTotalAmount.ForeColor = System.Drawing.Color.Black
        Me.lblTotalAmount.Location = New System.Drawing.Point(20, 37)
        Me.lblTotalAmount.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblTotalAmount.Name = "lblTotalAmount"
        Me.lblTotalAmount.Size = New System.Drawing.Size(331, 65)
        Me.lblTotalAmount.TabIndex = 20
        Me.lblTotalAmount.Text = "0.00"
        Me.lblTotalAmount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label13.ForeColor = System.Drawing.Color.Yellow
        Me.Label13.Location = New System.Drawing.Point(19, 267)
        Me.Label13.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(155, 28)
        Me.Label13.TabIndex = 22
        Me.Label13.Text = "GRAND TOTAL:"
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Location = New System.Drawing.Point(20, 190)
        Me.Label15.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(64, 25)
        Me.Label15.TabIndex = 25
        Me.Label15.Text = "INV %"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(20, 113)
        Me.Label14.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(71, 25)
        Me.Label14.TabIndex = 23
        Me.Label14.Text = "OUR %"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.Label12.Location = New System.Drawing.Point(20, 6)
        Me.Label12.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(138, 25)
        Me.Label12.TabIndex = 19
        Me.Label12.Text = "Total Amount:"
        '
        'PanelMethods
        '
        Me.PanelMethods.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PanelMethods.Controls.Add(Me.txtCash)
        Me.PanelMethods.Controls.Add(Me.cmbType)
        Me.PanelMethods.Controls.Add(Me.cmbPayMethod)
        Me.PanelMethods.Controls.Add(Me.cmbBillingType)
        Me.PanelMethods.Controls.Add(Me.Label18)
        Me.PanelMethods.Controls.Add(Me.Label7)
        Me.PanelMethods.Controls.Add(Me.Label8)
        Me.PanelMethods.Controls.Add(Me.Label3)
        Me.PanelMethods.Location = New System.Drawing.Point(13, 28)
        Me.PanelMethods.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.PanelMethods.Name = "PanelMethods"
        Me.PanelMethods.Size = New System.Drawing.Size(411, 246)
        Me.PanelMethods.TabIndex = 43
        '
        'txtCash
        '
        Me.txtCash.Font = New System.Drawing.Font("Segoe UI", 14.0!, System.Drawing.FontStyle.Bold)
        Me.txtCash.Location = New System.Drawing.Point(160, 185)
        Me.txtCash.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.txtCash.Name = "txtCash"
        Me.txtCash.Size = New System.Drawing.Size(239, 39)
        Me.txtCash.TabIndex = 37
        Me.txtCash.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'cmbType
        '
        Me.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbType.FormattingEnabled = True
        Me.cmbType.Items.AddRange(New Object() {"Sale", "Quote"})
        Me.cmbType.Location = New System.Drawing.Point(160, 129)
        Me.cmbType.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.cmbType.Name = "cmbType"
        Me.cmbType.Size = New System.Drawing.Size(239, 33)
        Me.cmbType.TabIndex = 36
        '
        'cmbPayMethod
        '
        Me.cmbPayMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbPayMethod.FormattingEnabled = True
        Me.cmbPayMethod.Items.AddRange(New Object() {"Cash", "Debit Card", "Credit Card", "Online Transfer", "Cheque", "Credit"})
        Me.cmbPayMethod.Location = New System.Drawing.Point(160, 74)
        Me.cmbPayMethod.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.cmbPayMethod.Name = "cmbPayMethod"
        Me.cmbPayMethod.Size = New System.Drawing.Size(239, 33)
        Me.cmbPayMethod.TabIndex = 35
        '
        'cmbBillingType
        '
        Me.cmbBillingType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbBillingType.FormattingEnabled = True
        Me.cmbBillingType.Items.AddRange(New Object() {"Cash", "Credit", "Cheque"})
        Me.cmbBillingType.Location = New System.Drawing.Point(160, 18)
        Me.cmbBillingType.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.cmbBillingType.Name = "cmbBillingType"
        Me.cmbBillingType.Size = New System.Drawing.Size(239, 33)
        Me.cmbBillingType.TabIndex = 34
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.Label18.Location = New System.Drawing.Point(20, 135)
        Me.Label18.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(91, 25)
        Me.Label18.TabIndex = 33
        Me.Label18.Text = "Bill Type:"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.Label7.Location = New System.Drawing.Point(20, 80)
        Me.Label7.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(106, 25)
        Me.Label7.TabIndex = 32
        Me.Label7.Text = "P. Method:"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.Label8.Location = New System.Drawing.Point(20, 25)
        Me.Label8.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(120, 25)
        Me.Label8.TabIndex = 31
        Me.Label8.Text = "Billing Type:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label3.ForeColor = System.Drawing.Color.Aqua
        Me.Label3.Location = New System.Drawing.Point(20, 191)
        Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(61, 28)
        Me.Label3.TabIndex = 30
        Me.Label3.Text = "Cash:"
        '
        'PanelBalances
        '
        Me.PanelBalances.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PanelBalances.Controls.Add(Me.p_num)
        Me.PanelBalances.Controls.Add(Me.Label2)
        Me.PanelBalances.Controls.Add(Me.lblTotalBalance)
        Me.PanelBalances.Controls.Add(Me.lblCreditBalance)
        Me.PanelBalances.Controls.Add(Me.lblChangeAmount)
        Me.PanelBalances.Controls.Add(Me.Label16)
        Me.PanelBalances.Controls.Add(Me.Label17)
        Me.PanelBalances.Controls.Add(Me.Label11)
        Me.PanelBalances.Controls.Add(Me.chkChangeGiven)
        Me.PanelBalances.Location = New System.Drawing.Point(432, 28)
        Me.PanelBalances.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.PanelBalances.Name = "PanelBalances"
        Me.PanelBalances.Size = New System.Drawing.Size(657, 246)
        Me.PanelBalances.TabIndex = 42
        '
        'chkChangeGiven
        '
        Me.chkChangeGiven.AutoSize = True
        Me.chkChangeGiven.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.chkChangeGiven.ForeColor = System.Drawing.Color.Yellow
        Me.chkChangeGiven.Location = New System.Drawing.Point(265, 57)
        Me.chkChangeGiven.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.chkChangeGiven.Name = "chkChangeGiven"
        Me.chkChangeGiven.Size = New System.Drawing.Size(272, 29)
        Me.chkChangeGiven.TabIndex = 34
        Me.chkChangeGiven.Text = "Change Given to Customer"
        Me.chkChangeGiven.UseVisualStyleBackColor = True
        '
        'lblTotalBalance
        '
        Me.lblTotalBalance.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.lblTotalBalance.Font = New System.Drawing.Font("Segoe UI", 16.0!, System.Drawing.FontStyle.Bold)
        Me.lblTotalBalance.ForeColor = System.Drawing.Color.Black
        Me.lblTotalBalance.Location = New System.Drawing.Point(211, 177)
        Me.lblTotalBalance.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblTotalBalance.Name = "lblTotalBalance"
        Me.lblTotalBalance.Size = New System.Drawing.Size(435, 49)
        Me.lblTotalBalance.TabIndex = 33
        Me.lblTotalBalance.Text = "0.00"
        Me.lblTotalBalance.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblCreditBalance
        '
        Me.lblCreditBalance.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.lblCreditBalance.Font = New System.Drawing.Font("Segoe UI", 14.0!, System.Drawing.FontStyle.Bold)
        Me.lblCreditBalance.ForeColor = System.Drawing.Color.Black
        Me.lblCreditBalance.Location = New System.Drawing.Point(213, 66)
        Me.lblCreditBalance.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblCreditBalance.Name = "lblCreditBalance"
        Me.lblCreditBalance.Size = New System.Drawing.Size(427, 43)
        Me.lblCreditBalance.TabIndex = 32
        Me.lblCreditBalance.Text = "0.00"
        Me.lblCreditBalance.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblChangeAmount
        '
        Me.lblChangeAmount.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.lblChangeAmount.Font = New System.Drawing.Font("Segoe UI", 14.0!, System.Drawing.FontStyle.Bold)
        Me.lblChangeAmount.ForeColor = System.Drawing.Color.Black
        Me.lblChangeAmount.Location = New System.Drawing.Point(212, 10)
        Me.lblChangeAmount.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblChangeAmount.Name = "lblChangeAmount"
        Me.lblChangeAmount.Size = New System.Drawing.Size(427, 43)
        Me.lblChangeAmount.TabIndex = 31
        Me.lblChangeAmount.Text = "0.00"
        Me.lblChangeAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label16.Location = New System.Drawing.Point(11, 190)
        Me.Label16.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(173, 28)
        Me.Label16.TabIndex = 30
        Me.Label16.Text = "TOTAL BALANCE:"
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Location = New System.Drawing.Point(11, 66)
        Me.Label17.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(138, 25)
        Me.Label17.TabIndex = 29
        Me.Label17.Text = "Credit Balance:"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(4, 9)
        Me.Label11.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(153, 25)
        Me.Label11.TabIndex = 18
        Me.Label11.Text = "Change Amount:"
        '
        'lblBreakdown
        '
        Me.lblBreakdown.BackColor = System.Drawing.Color.FromArgb(CType(CType(33, Byte), Integer), CType(CType(37, Byte), Integer), CType(CType(41, Byte), Integer))
        Me.lblBreakdown.Font = New System.Drawing.Font("Segoe UI", 18.0!, System.Drawing.FontStyle.Bold)
        Me.lblBreakdown.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.lblBreakdown.Location = New System.Drawing.Point(9, 320)
        Me.lblBreakdown.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblBreakdown.Name = "lblBreakdown"
        Me.lblBreakdown.Size = New System.Drawing.Size(740, 94)
        Me.lblBreakdown.TabIndex = 10
        Me.lblBreakdown.Text = "-"
        Me.lblBreakdown.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.Label6.Location = New System.Drawing.Point(122, 283)
        Me.Label6.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(246, 25)
        Me.Label6.TabIndex = 9
        Me.Label6.Text = "Denomination Suggestion"
        '
        'btnCollect
        '
        Me.btnCollect.BackColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(135, Byte), Integer), CType(CType(84, Byte), Integer))
        Me.btnCollect.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnCollect.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCollect.Font = New System.Drawing.Font("Segoe UI", 24.0!, System.Drawing.FontStyle.Bold)
        Me.btnCollect.ForeColor = System.Drawing.Color.White
        Me.btnCollect.Location = New System.Drawing.Point(757, 320)
        Me.btnCollect.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.btnCollect.Name = "btnCollect"
        Me.btnCollect.Size = New System.Drawing.Size(367, 94)
        Me.btnCollect.TabIndex = 2
        Me.btnCollect.Text = "FINAL COLLECT"
        Me.btnCollect.UseVisualStyleBackColor = False
        '
        'lblInvNo
        '
        Me.lblInvNo.AutoSize = True
        Me.lblInvNo.Font = New System.Drawing.Font("Segoe UI", 14.0!, System.Drawing.FontStyle.Bold)
        Me.lblInvNo.ForeColor = System.Drawing.Color.Yellow
        Me.lblInvNo.Location = New System.Drawing.Point(869, 283)
        Me.lblInvNo.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblInvNo.Name = "lblInvNo"
        Me.lblInvNo.Size = New System.Drawing.Size(25, 32)
        Me.lblInvNo.TabIndex = 1
        Me.lblInvNo.Text = "-"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(693, 283)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(106, 25)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Invoice No:"
        '
        'lblInvoiceType
        '
        Me.lblInvoiceType.AutoSize = True
        Me.lblInvoiceType.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblInvoiceType.ForeColor = System.Drawing.Color.Lime
        Me.lblInvoiceType.Location = New System.Drawing.Point(1021, 363)
        Me.lblInvoiceType.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblInvoiceType.Name = "lblInvoiceType"
        Me.lblInvoiceType.Size = New System.Drawing.Size(145, 28)
        Me.lblInvoiceType.TabIndex = 35
        Me.lblInvoiceType.Text = "[Invoice Type]"
        Me.lblInvoiceType.Visible = False
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.Label2.Location = New System.Drawing.Point(11, 119)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(164, 31)
        Me.Label2.TabIndex = 35
        Me.Label2.Text = "P/O Number:"
        '
        'p_num
        '
        Me.p_num.Font = New System.Drawing.Font("Segoe UI", 14.0!, System.Drawing.FontStyle.Bold)
        Me.p_num.Location = New System.Drawing.Point(211, 119)
        Me.p_num.Margin = New System.Windows.Forms.Padding(4)
        Me.p_num.Name = "p_num"
        Me.p_num.Size = New System.Drawing.Size(429, 39)
        Me.p_num.TabIndex = 38
        Me.p_num.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'PaymentCollector
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1665, 862)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.Name = "PaymentCollector"
        Me.Text = "Financial Terminal - Pending Payments"
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        CType(Me.dgvPending, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.PanelTotals.ResumeLayout(False)
        Me.PanelTotals.PerformLayout()
        Me.PanelMethods.ResumeLayout(False)
        Me.PanelMethods.PerformLayout()
        Me.PanelBalances.ResumeLayout(False)
        Me.PanelBalances.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents SplitContainer1 As Windows.Forms.SplitContainer
    Friend WithEvents dgvPending As Windows.Forms.DataGridView
    Friend WithEvents lblPendingCount As Windows.Forms.Label
    Friend WithEvents GroupBox1 As Windows.Forms.GroupBox
    Friend WithEvents lblInvNo As Windows.Forms.Label
    Friend WithEvents Label1 As Windows.Forms.Label
    Friend WithEvents cmbPayMethod As Windows.Forms.ComboBox
    Friend WithEvents Label7 As Windows.Forms.Label
    Friend WithEvents Label8 As Windows.Forms.Label
    Friend WithEvents cmbBillingType As Windows.Forms.ComboBox
    Friend WithEvents txtCash As Windows.Forms.TextBox
    Friend WithEvents lblChangeAmount As Windows.Forms.Label
    Friend WithEvents Label11 As Windows.Forms.Label
    Friend WithEvents lblTotalAmount As Windows.Forms.Label
    Friend WithEvents Label12 As Windows.Forms.Label
    Friend WithEvents lblGrandTotal As Windows.Forms.Label
    Friend WithEvents Label13 As Windows.Forms.Label
    Friend WithEvents txtOurDiscount As Windows.Forms.TextBox
    Friend WithEvents txtInvDiscount As Windows.Forms.TextBox
    Friend WithEvents Label14 As Windows.Forms.Label
    Friend WithEvents Label15 As Windows.Forms.Label
    Friend WithEvents cmbType As Windows.Forms.ComboBox
    Friend WithEvents Label16 As Windows.Forms.Label
    Friend WithEvents lblCreditBalance As Windows.Forms.Label
    Friend WithEvents lblTotalBalance As Windows.Forms.Label
    Friend WithEvents Label17 As Windows.Forms.Label
    Friend WithEvents Label18 As Windows.Forms.Label
    Friend WithEvents btnCollect As Windows.Forms.Button
    Friend WithEvents btnShowPending As Windows.Forms.Button
    Friend WithEvents btnShowCollected As Windows.Forms.Button
    Friend WithEvents btnShowReturns As Windows.Forms.Button
    Friend WithEvents btnDelete As Windows.Forms.Button
    Friend WithEvents lblBreakdown As Windows.Forms.Label
    Friend WithEvents Label6 As Windows.Forms.Label
    Friend WithEvents Label3 As Windows.Forms.Label
    Friend WithEvents PanelTotals As Windows.Forms.Panel
    Friend WithEvents lblInvoiceType As Windows.Forms.Label
    Friend WithEvents PanelMethods As Windows.Forms.Panel
    Friend WithEvents PanelBalances As Windows.Forms.Panel
    Friend WithEvents chkChangeGiven As Windows.Forms.CheckBox
    Friend WithEvents chkManualChange As Windows.Forms.CheckBox
    Friend WithEvents lblDrawerStatus As Windows.Forms.Label
    Friend WithEvents p_num As TextBox
    Friend WithEvents Label2 As Label
End Class