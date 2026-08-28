<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ManagementAlerts
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.pnlHeader = New System.Windows.Forms.Panel()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.tabsMain = New System.Windows.Forms.TabControl()
        Me.tabCustCredits = New System.Windows.Forms.TabPage()
        Me.dgvCustCredits = New System.Windows.Forms.DataGridView()
        Me.tabBlockedCust = New System.Windows.Forms.TabPage()
        Me.dgvBlockedCust = New System.Windows.Forms.DataGridView()
        Me.tabSupCredits = New System.Windows.Forms.TabPage()
        Me.dgvSupCredits = New System.Windows.Forms.DataGridView()
        Me.tabSupAlerts = New System.Windows.Forms.TabPage()
        Me.dgvSupAlerts = New System.Windows.Forms.DataGridView()
        Me.tabCustChequeReturns = New System.Windows.Forms.TabPage()
        Me.dgvCustChequeReturns = New System.Windows.Forms.DataGridView()
        Me.tabSupChequeReturns = New System.Windows.Forms.TabPage()
        Me.dgvSupChequeReturns = New System.Windows.Forms.DataGridView()
        Me.pnlHeader.SuspendLayout()
        Me.tabsMain.SuspendLayout()
        Me.tabCustCredits.SuspendLayout()
        CType(Me.dgvCustCredits, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabBlockedCust.SuspendLayout()
        CType(Me.dgvBlockedCust, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabSupCredits.SuspendLayout()
        CType(Me.dgvSupCredits, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabSupAlerts.SuspendLayout()
        CType(Me.dgvSupAlerts, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabCustChequeReturns.SuspendLayout()
        CType(Me.dgvCustChequeReturns, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabSupChequeReturns.SuspendLayout()
        CType(Me.dgvSupChequeReturns, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'pnlHeader
        '
        Me.pnlHeader.BackColor = System.Drawing.Color.FromArgb(44, 62, 80)
        Me.pnlHeader.Controls.Add(Me.btnClose)
        Me.pnlHeader.Controls.Add(Me.lblTitle)
        Me.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlHeader.Location = New System.Drawing.Point(0, 0)
        Me.pnlHeader.Name = "pnlHeader"
        Me.pnlHeader.Size = New System.Drawing.Size(950, 60)
        Me.pnlHeader.TabIndex = 0
        '
        'btnClose
        '
        Me.btnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClose.BackColor = System.Drawing.Color.FromArgb(231, 76, 60)
        Me.btnClose.FlatAppearance.BorderSize = 0
        Me.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnClose.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
        Me.btnClose.ForeColor = System.Drawing.Color.White
        Me.btnClose.Location = New System.Drawing.Point(830, 12)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(108, 35)
        Me.btnClose.TabIndex = 1
        Me.btnClose.Text = "ACKNOWLEDGE"
        Me.btnClose.UseVisualStyleBackColor = False
        '
        'lblTitle
        '
        Me.lblTitle.AutoSize = True
        Me.lblTitle.Font = New System.Drawing.Font("Segoe UI", 16.0!, System.Drawing.FontStyle.Bold)
        Me.lblTitle.ForeColor = System.Drawing.Color.White
        Me.lblTitle.Location = New System.Drawing.Point(12, 12)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(437, 30)
        Me.lblTitle.TabIndex = 0
        Me.lblTitle.Text = "MANAGEMENT & FINANCIAL NOTIFICATIONS"
        '
        'tabsMain
        '
        Me.tabsMain.Controls.Add(Me.tabCustCredits)
        Me.tabsMain.Controls.Add(Me.tabBlockedCust)
        Me.tabsMain.Controls.Add(Me.tabSupCredits)
        Me.tabsMain.Controls.Add(Me.tabSupAlerts)
        Me.tabsMain.Controls.Add(Me.tabCustChequeReturns)
        Me.tabsMain.Controls.Add(Me.tabSupChequeReturns)
        Me.tabsMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tabsMain.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
        Me.tabsMain.Location = New System.Drawing.Point(0, 60)
        Me.tabsMain.Name = "tabsMain"
        Me.tabsMain.SelectedIndex = 0
        Me.tabsMain.Size = New System.Drawing.Size(950, 540)
        Me.tabsMain.TabIndex = 1
        '
        'tabCustCredits
        '
        Me.tabCustCredits.Controls.Add(Me.dgvCustCredits)
        Me.tabCustCredits.Location = New System.Drawing.Point(4, 26)
        Me.tabCustCredits.Name = "tabCustCredits"
        Me.tabCustCredits.Padding = New System.Windows.Forms.Padding(3)
        Me.tabCustCredits.Size = New System.Drawing.Size(942, 510)
        Me.tabCustCredits.TabIndex = 0
        Me.tabCustCredits.Text = "Customer Credits (2M+)"
        Me.tabCustCredits.UseVisualStyleBackColor = True
        '
        'dgvCustCredits
        '
        Me.dgvCustCredits.AllowUserToAddRows = False
        Me.dgvCustCredits.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvCustCredits.BackgroundColor = System.Drawing.Color.White
        Me.dgvCustCredits.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvCustCredits.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvCustCredits.Location = New System.Drawing.Point(3, 3)
        Me.dgvCustCredits.Name = "dgvCustCredits"
        Me.dgvCustCredits.ReadOnly = True
        Me.dgvCustCredits.RowHeadersVisible = False
        Me.dgvCustCredits.Size = New System.Drawing.Size(936, 504)
        Me.dgvCustCredits.TabIndex = 0
        '
        'tabBlockedCust
        '
        Me.tabBlockedCust.Controls.Add(Me.dgvBlockedCust)
        Me.tabBlockedCust.Location = New System.Drawing.Point(4, 26)
        Me.tabBlockedCust.Name = "tabBlockedCust"
        Me.tabBlockedCust.Padding = New System.Windows.Forms.Padding(3)
        Me.tabBlockedCust.Size = New System.Drawing.Size(942, 510)
        Me.tabBlockedCust.TabIndex = 1
        Me.tabBlockedCust.Text = "Blocked Customers"
        Me.tabBlockedCust.UseVisualStyleBackColor = True
        '
        'dgvBlockedCust
        '
        Me.dgvBlockedCust.AllowUserToAddRows = False
        Me.dgvBlockedCust.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvBlockedCust.BackgroundColor = System.Drawing.Color.White
        Me.dgvBlockedCust.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvBlockedCust.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvBlockedCust.Location = New System.Drawing.Point(3, 3)
        Me.dgvBlockedCust.Name = "dgvBlockedCust"
        Me.dgvBlockedCust.ReadOnly = True
        Me.dgvBlockedCust.RowHeadersVisible = False
        Me.dgvBlockedCust.Size = New System.Drawing.Size(936, 504)
        Me.dgvBlockedCust.TabIndex = 0
        '
        'tabSupCredits
        '
        Me.tabSupCredits.Controls.Add(Me.dgvSupCredits)
        Me.tabSupCredits.Location = New System.Drawing.Point(4, 26)
        Me.tabSupCredits.Name = "tabSupCredits"
        Me.tabSupCredits.Padding = New System.Windows.Forms.Padding(3)
        Me.tabSupCredits.Size = New System.Drawing.Size(942, 510)
        Me.tabSupCredits.TabIndex = 2
        Me.tabSupCredits.Text = "Supplier Credits"
        Me.tabSupCredits.UseVisualStyleBackColor = True
        '
        'dgvSupCredits
        '
        Me.dgvSupCredits.AllowUserToAddRows = False
        Me.dgvSupCredits.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvSupCredits.BackgroundColor = System.Drawing.Color.White
        Me.dgvSupCredits.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvSupCredits.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvSupCredits.Location = New System.Drawing.Point(3, 3)
        Me.dgvSupCredits.Name = "dgvSupCredits"
        Me.dgvSupCredits.ReadOnly = True
        Me.dgvSupCredits.RowHeadersVisible = False
        Me.dgvSupCredits.Size = New System.Drawing.Size(936, 504)
        Me.dgvSupCredits.TabIndex = 0
        '
        'tabSupAlerts
        '
        Me.tabSupAlerts.Controls.Add(Me.dgvSupAlerts)
        Me.tabSupAlerts.Location = New System.Drawing.Point(4, 26)
        Me.tabSupAlerts.Name = "tabSupAlerts"
        Me.tabSupAlerts.Padding = New System.Windows.Forms.Padding(3)
        Me.tabSupAlerts.Size = New System.Drawing.Size(942, 510)
        Me.tabSupAlerts.TabIndex = 3
        Me.tabSupAlerts.Text = "Supplier Alerts (10D)"
        Me.tabSupAlerts.UseVisualStyleBackColor = True
        '
        'dgvSupAlerts
        '
        Me.dgvSupAlerts.AllowUserToAddRows = False
        Me.dgvSupAlerts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvSupAlerts.BackgroundColor = System.Drawing.Color.White
        Me.dgvSupAlerts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvSupAlerts.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvSupAlerts.Location = New System.Drawing.Point(3, 3)
        Me.dgvSupAlerts.Name = "dgvSupAlerts"
        Me.dgvSupAlerts.ReadOnly = True
        Me.dgvSupAlerts.RowHeadersVisible = False
        Me.dgvSupAlerts.Size = New System.Drawing.Size(936, 504)
        Me.dgvSupAlerts.TabIndex = 0
        '
        'tabCustChequeReturns
        '
        Me.tabCustChequeReturns.Controls.Add(Me.dgvCustChequeReturns)
        Me.tabCustChequeReturns.Location = New System.Drawing.Point(4, 26)
        Me.tabCustChequeReturns.Name = "tabCustChequeReturns"
        Me.tabCustChequeReturns.Padding = New System.Windows.Forms.Padding(3)
        Me.tabCustChequeReturns.Size = New System.Drawing.Size(942, 510)
        Me.tabCustChequeReturns.TabIndex = 4
        Me.tabCustChequeReturns.Text = "Customer Cheque Return"
        Me.tabCustChequeReturns.UseVisualStyleBackColor = True
        '
        'dgvCustChequeReturns
        '
        Me.dgvCustChequeReturns.AllowUserToAddRows = False
        Me.dgvCustChequeReturns.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvCustChequeReturns.BackgroundColor = System.Drawing.Color.White
        Me.dgvCustChequeReturns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvCustChequeReturns.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvCustChequeReturns.Location = New System.Drawing.Point(3, 3)
        Me.dgvCustChequeReturns.Name = "dgvCustChequeReturns"
        Me.dgvCustChequeReturns.ReadOnly = True
        Me.dgvCustChequeReturns.RowHeadersVisible = False
        Me.dgvCustChequeReturns.Size = New System.Drawing.Size(936, 504)
        Me.dgvCustChequeReturns.TabIndex = 0
        '
        'tabSupChequeReturns
        '
        Me.tabSupChequeReturns.Controls.Add(Me.dgvSupChequeReturns)
        Me.tabSupChequeReturns.Location = New System.Drawing.Point(4, 26)
        Me.tabSupChequeReturns.Name = "tabSupChequeReturns"
        Me.tabSupChequeReturns.Padding = New System.Windows.Forms.Padding(3)
        Me.tabSupChequeReturns.Size = New System.Drawing.Size(942, 510)
        Me.tabSupChequeReturns.TabIndex = 5
        Me.tabSupChequeReturns.Text = "Supplier cheque return"
        Me.tabSupChequeReturns.UseVisualStyleBackColor = True
        '
        'dgvSupChequeReturns
        '
        Me.dgvSupChequeReturns.AllowUserToAddRows = False
        Me.dgvSupChequeReturns.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvSupChequeReturns.BackgroundColor = System.Drawing.Color.White
        Me.dgvSupChequeReturns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvSupChequeReturns.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvSupChequeReturns.Location = New System.Drawing.Point(3, 3)
        Me.dgvSupChequeReturns.Name = "dgvSupChequeReturns"
        Me.dgvSupChequeReturns.ReadOnly = True
        Me.dgvSupChequeReturns.RowHeadersVisible = False
        Me.dgvSupChequeReturns.Size = New System.Drawing.Size(936, 504)
        Me.dgvSupChequeReturns.TabIndex = 0
        '
        'ManagementAlerts
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(236, 240, 241)
        Me.ClientSize = New System.Drawing.Size(950, 600)
        Me.Controls.Add(Me.tabsMain)
        Me.Controls.Add(Me.pnlHeader)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "ManagementAlerts"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Management Alerts"
        Me.pnlHeader.ResumeLayout(False)
        Me.pnlHeader.PerformLayout()
        Me.tabsMain.ResumeLayout(False)
        Me.tabCustCredits.ResumeLayout(False)
        CType(Me.dgvCustCredits, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabBlockedCust.ResumeLayout(False)
        CType(Me.dgvBlockedCust, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabSupCredits.ResumeLayout(False)
        CType(Me.dgvSupCredits, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabSupAlerts.ResumeLayout(False)
        CType(Me.dgvSupAlerts, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabCustChequeReturns.ResumeLayout(False)
        CType(Me.dgvCustChequeReturns, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabSupChequeReturns.ResumeLayout(False)
        CType(Me.dgvSupChequeReturns, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents pnlHeader As Panel
    Friend WithEvents btnClose As Button
    Friend WithEvents lblTitle As Label
    Friend WithEvents tabsMain As TabControl
    Friend WithEvents tabCustCredits As TabPage
    Friend WithEvents dgvCustCredits As DataGridView
    Friend WithEvents tabBlockedCust As TabPage
    Friend WithEvents dgvBlockedCust As DataGridView
    Friend WithEvents tabSupCredits As TabPage
    Friend WithEvents dgvSupCredits As DataGridView
    Friend WithEvents tabSupAlerts As TabPage
    Friend WithEvents dgvSupAlerts As DataGridView
    Friend WithEvents tabCustChequeReturns As TabPage
    Friend WithEvents dgvCustChequeReturns As DataGridView
    Friend WithEvents tabSupChequeReturns As TabPage
    Friend WithEvents dgvSupChequeReturns As DataGridView
End Class
