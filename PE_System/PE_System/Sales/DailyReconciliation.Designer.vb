<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DailyReconciliation
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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.PanelHeader = New System.Windows.Forms.Panel()
        Me.dtpDate = New System.Windows.Forms.DateTimePicker()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.PanelStats = New System.Windows.Forms.FlowLayoutPanel()
        Me.pnlTotalCash = New System.Windows.Forms.Panel()
        Me.lblCashAmt = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.pnlTotalCheque = New System.Windows.Forms.Panel()
        Me.lblChequeAmt = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.pnlTotalCredit = New System.Windows.Forms.Panel()
        Me.lblCreditAmt = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.dgvTransactions = New System.Windows.Forms.DataGridView()
        Me.PanelHeader.SuspendLayout()
        Me.PanelStats.SuspendLayout()
        Me.pnlTotalCash.SuspendLayout()
        Me.pnlTotalCheque.SuspendLayout()
        Me.pnlTotalCredit.SuspendLayout()
        CType(Me.dgvTransactions, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PanelHeader
        '
        Me.PanelHeader.BackColor = System.Drawing.Color.FromArgb(CType(CType(33, Byte), Integer), CType(CType(37, Byte), Integer), CType(CType(41, Byte), Integer))
        Me.PanelHeader.Controls.Add(Me.dtpDate)
        Me.PanelHeader.Controls.Add(Me.lblTitle)
        Me.PanelHeader.Location = New System.Drawing.Point(0, 0)
        Me.PanelHeader.Name = "PanelHeader"
        Me.PanelHeader.Size = New System.Drawing.Size(1283, 86)
        Me.PanelHeader.TabIndex = 0
        '
        'dtpDate
        '
        Me.dtpDate.Font = New System.Drawing.Font("Segoe UI", 12.0!)
        Me.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtpDate.Location = New System.Drawing.Point(1049, 24)
        Me.dtpDate.Name = "dtpDate"
        Me.dtpDate.Size = New System.Drawing.Size(209, 34)
        Me.dtpDate.TabIndex = 1
        '
        'lblTitle
        '
        Me.lblTitle.AutoSize = True
        Me.lblTitle.Font = New System.Drawing.Font("Segoe UI", 20.0!, System.Drawing.FontStyle.Bold)
        Me.lblTitle.ForeColor = System.Drawing.Color.White
        Me.lblTitle.Location = New System.Drawing.Point(23, 18)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(487, 46)
        Me.lblTitle.TabIndex = 0
        Me.lblTitle.Text = "Daily Financial Reconciliation"
        '
        'PanelStats
        '
        Me.PanelStats.Controls.Add(Me.pnlTotalCash)
        Me.PanelStats.Controls.Add(Me.pnlTotalCheque)
        Me.PanelStats.Controls.Add(Me.pnlTotalCredit)
        Me.PanelStats.Location = New System.Drawing.Point(0, 86)
        Me.PanelStats.Name = "PanelStats"
        Me.PanelStats.Padding = New System.Windows.Forms.Padding(11, 12, 11, 12)
        Me.PanelStats.Size = New System.Drawing.Size(1283, 172)
        Me.PanelStats.TabIndex = 1
        '
        'pnlTotalCash
        '
        Me.pnlTotalCash.BackColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(135, Byte), Integer), CType(CType(84, Byte), Integer))
        Me.pnlTotalCash.Controls.Add(Me.lblCashAmt)
        Me.pnlTotalCash.Controls.Add(Me.Label2)
        Me.pnlTotalCash.Location = New System.Drawing.Point(22, 24)
        Me.pnlTotalCash.Margin = New System.Windows.Forms.Padding(11, 12, 11, 12)
        Me.pnlTotalCash.Name = "pnlTotalCash"
        Me.pnlTotalCash.Size = New System.Drawing.Size(349, 123)
        Me.pnlTotalCash.TabIndex = 0
        '
        'lblCashAmt
        '
        Me.lblCashAmt.Font = New System.Drawing.Font("Segoe UI", 24.0!, System.Drawing.FontStyle.Bold)
        Me.lblCashAmt.ForeColor = System.Drawing.Color.White
        Me.lblCashAmt.Location = New System.Drawing.Point(0, 43)
        Me.lblCashAmt.Name = "lblCashAmt"
        Me.lblCashAmt.Size = New System.Drawing.Size(349, 79)
        Me.lblCashAmt.TabIndex = 1
        Me.lblCashAmt.Text = "0.00"
        Me.lblCashAmt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label2
        '
        Me.Label2.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label2.ForeColor = System.Drawing.Color.White
        Me.Label2.Location = New System.Drawing.Point(0, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(349, 43)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "Total Cash Collected (Sale)"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'pnlTotalCheque
        '
        Me.pnlTotalCheque.BackColor = System.Drawing.Color.FromArgb(CType(CType(13, Byte), Integer), CType(CType(110, Byte), Integer), CType(CType(253, Byte), Integer))
        Me.pnlTotalCheque.Controls.Add(Me.lblChequeAmt)
        Me.pnlTotalCheque.Controls.Add(Me.Label4)
        Me.pnlTotalCheque.Location = New System.Drawing.Point(393, 24)
        Me.pnlTotalCheque.Margin = New System.Windows.Forms.Padding(11, 12, 11, 12)
        Me.pnlTotalCheque.Name = "pnlTotalCheque"
        Me.pnlTotalCheque.Size = New System.Drawing.Size(349, 123)
        Me.pnlTotalCheque.TabIndex = 1
        '
        'lblChequeAmt
        '
        Me.lblChequeAmt.Font = New System.Drawing.Font("Segoe UI", 24.0!, System.Drawing.FontStyle.Bold)
        Me.lblChequeAmt.ForeColor = System.Drawing.Color.White
        Me.lblChequeAmt.Location = New System.Drawing.Point(0, 43)
        Me.lblChequeAmt.Name = "lblChequeAmt"
        Me.lblChequeAmt.Size = New System.Drawing.Size(349, 79)
        Me.lblChequeAmt.TabIndex = 1
        Me.lblChequeAmt.Text = "0.00"
        Me.lblChequeAmt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label4
        '
        Me.Label4.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label4.ForeColor = System.Drawing.Color.White
        Me.Label4.Location = New System.Drawing.Point(0, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(349, 43)
        Me.Label4.TabIndex = 0
        Me.Label4.Text = "Total Cheques"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'pnlTotalCredit
        '
        Me.pnlTotalCredit.BackColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(53, Byte), Integer), CType(CType(69, Byte), Integer))
        Me.pnlTotalCredit.Controls.Add(Me.lblCreditAmt)
        Me.pnlTotalCredit.Controls.Add(Me.Label6)
        Me.pnlTotalCredit.Location = New System.Drawing.Point(764, 24)
        Me.pnlTotalCredit.Margin = New System.Windows.Forms.Padding(11, 12, 11, 12)
        Me.pnlTotalCredit.Name = "pnlTotalCredit"
        Me.pnlTotalCredit.Size = New System.Drawing.Size(349, 123)
        Me.pnlTotalCredit.TabIndex = 2
        '
        'lblCreditAmt
        '
        Me.lblCreditAmt.Font = New System.Drawing.Font("Segoe UI", 24.0!, System.Drawing.FontStyle.Bold)
        Me.lblCreditAmt.ForeColor = System.Drawing.Color.White
        Me.lblCreditAmt.Location = New System.Drawing.Point(0, 43)
        Me.lblCreditAmt.Name = "lblCreditAmt"
        Me.lblCreditAmt.Size = New System.Drawing.Size(349, 79)
        Me.lblCreditAmt.TabIndex = 1
        Me.lblCreditAmt.Text = "0.00"
        Me.lblCreditAmt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label6
        '
        Me.Label6.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label6.ForeColor = System.Drawing.Color.White
        Me.Label6.Location = New System.Drawing.Point(0, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(349, 43)
        Me.Label6.TabIndex = 0
        Me.Label6.Text = "Total Credit Sales"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'dgvTransactions
        '
        Me.dgvTransactions.AllowUserToAddRows = False
        Me.dgvTransactions.AllowUserToDeleteRows = False
        DataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(240, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.dgvTransactions.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.dgvTransactions.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.dgvTransactions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvTransactions.BackgroundColor = System.Drawing.Color.White
        Me.dgvTransactions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Segoe UI", 11.0!)
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(CType(CType(33, Byte), Integer), CType(CType(37, Byte), Integer), CType(CType(41, Byte), Integer))
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.dgvTransactions.DefaultCellStyle = DataGridViewCellStyle2
        Me.dgvTransactions.Location = New System.Drawing.Point(0, 258)
        Me.dgvTransactions.Name = "dgvTransactions"
        Me.dgvTransactions.ReadOnly = True
        Me.dgvTransactions.RowHeadersVisible = False
        Me.dgvTransactions.RowHeadersWidth = 51
        Me.dgvTransactions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvTransactions.Size = New System.Drawing.Size(1283, 467)
        Me.dgvTransactions.TabIndex = 2
        '
        'DailyReconciliation
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(1283, 738)
        Me.Controls.Add(Me.dgvTransactions)
        Me.Controls.Add(Me.PanelStats)
        Me.Controls.Add(Me.PanelHeader)
        Me.Name = "DailyReconciliation"
        Me.Text = "Daily Financial Reconciliation"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.PanelHeader.ResumeLayout(False)
        Me.PanelHeader.PerformLayout()
        Me.PanelStats.ResumeLayout(False)
        Me.pnlTotalCash.ResumeLayout(False)
        Me.pnlTotalCheque.ResumeLayout(False)
        Me.pnlTotalCredit.ResumeLayout(False)
        CType(Me.dgvTransactions, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents PanelHeader As Windows.Forms.Panel
    Friend WithEvents dtpDate As Windows.Forms.DateTimePicker
    Friend WithEvents lblTitle As Windows.Forms.Label
    Friend WithEvents PanelStats As Windows.Forms.FlowLayoutPanel
    Friend WithEvents pnlTotalCash As Windows.Forms.Panel
    Friend WithEvents lblCashAmt As Windows.Forms.Label
    Friend WithEvents Label2 As Windows.Forms.Label
    Friend WithEvents pnlTotalCheque As Windows.Forms.Panel
    Friend WithEvents lblChequeAmt As Windows.Forms.Label
    Friend WithEvents Label4 As Windows.Forms.Label
    Friend WithEvents pnlTotalCredit As Windows.Forms.Panel
    Friend WithEvents lblCreditAmt As Windows.Forms.Label
    Friend WithEvents Label6 As Windows.Forms.Label
    Friend WithEvents dgvTransactions As Windows.Forms.DataGridView
End Class