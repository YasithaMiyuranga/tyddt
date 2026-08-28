<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class InvoiceUpdateWarning
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.pnlHeader = New System.Windows.Forms.Panel()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.picWarning = New System.Windows.Forms.PictureBox()
        Me.lblDescription = New System.Windows.Forms.Label()
        Me.lblOriginalBilling = New System.Windows.Forms.Label()
        Me.lblOriginalStatus = New System.Windows.Forms.Label()
        Me.lblOriginalCredit = New System.Windows.Forms.Label()
        Me.lblOriginalCheque = New System.Windows.Forms.Label()
        Me.lblNewBilling = New System.Windows.Forms.Label()
        Me.lblNewStatus = New System.Windows.Forms.Label()
        Me.pnlCheques = New System.Windows.Forms.Panel()
        Me.lblChequeTitle = New System.Windows.Forms.Label()
        Me.dgvCheques = New System.Windows.Forms.DataGridView()
        Me.pnlPayments = New System.Windows.Forms.Panel()
        Me.lblPaymentTitle = New System.Windows.Forms.Label()
        Me.dgvPayments = New System.Windows.Forms.DataGridView()
        Me.btnYes = New System.Windows.Forms.Button()
        Me.btnNo = New System.Windows.Forms.Button()
        Me.pnlHeader.SuspendLayout()
        CType(Me.picWarning, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlCheques.SuspendLayout()
        CType(Me.dgvCheques, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlPayments.SuspendLayout()
        CType(Me.dgvPayments, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'pnlHeader
        '
        Me.pnlHeader.BackColor = System.Drawing.Color.FromArgb(CType(CType(44, Byte), Integer), CType(CType(62, Byte), Integer), CType(CType(80, Byte), Integer))
        Me.pnlHeader.Controls.Add(Me.lblTitle)
        Me.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlHeader.Location = New System.Drawing.Point(0, 0)
        Me.pnlHeader.Name = "pnlHeader"
        Me.pnlHeader.Size = New System.Drawing.Size(600, 40)
        Me.pnlHeader.TabIndex = 0
        '
        'lblTitle
        '
        Me.lblTitle.AutoSize = True
        Me.lblTitle.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblTitle.ForeColor = System.Drawing.Color.White
        Me.lblTitle.Location = New System.Drawing.Point(12, 9)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(286, 28)
        Me.lblTitle.TabIndex = 0
        Me.lblTitle.Text = "Billing Type Change Warning"
        '
        'picWarning
        '
        Me.picWarning.Location = New System.Drawing.Point(12, 50)
        Me.picWarning.Name = "picWarning"
        Me.picWarning.Size = New System.Drawing.Size(53, 72)
        Me.picWarning.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picWarning.TabIndex = 1
        Me.picWarning.TabStop = False
        '
        'lblDescription
        '
        Me.lblDescription.AutoSize = True
        Me.lblDescription.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
        Me.lblDescription.ForeColor = System.Drawing.Color.Red
        Me.lblDescription.Location = New System.Drawing.Point(80, 50)
        Me.lblDescription.Name = "lblDescription"
        Me.lblDescription.Size = New System.Drawing.Size(512, 23)
        Me.lblDescription.TabIndex = 2
        Me.lblDescription.Text = "Warning: You are changing the billing structure of this invoice."
        '
        'lblOriginalBilling
        '
        Me.lblOriginalBilling.AutoSize = True
        Me.lblOriginalBilling.Location = New System.Drawing.Point(80, 129)
        Me.lblOriginalBilling.Name = "lblOriginalBilling"
        Me.lblOriginalBilling.Size = New System.Drawing.Size(121, 20)
        Me.lblOriginalBilling.TabIndex = 3
        Me.lblOriginalBilling.Text = "Original Billing: -"
        '
        'lblOriginalStatus
        '
        Me.lblOriginalStatus.AutoSize = True
        Me.lblOriginalStatus.Location = New System.Drawing.Point(80, 149)
        Me.lblOriginalStatus.Name = "lblOriginalStatus"
        Me.lblOriginalStatus.Size = New System.Drawing.Size(119, 20)
        Me.lblOriginalStatus.TabIndex = 4
        Me.lblOriginalStatus.Text = "Original Status: -"
        '
        'lblOriginalCredit
        '
        Me.lblOriginalCredit.AutoSize = True
        Me.lblOriginalCredit.ForeColor = System.Drawing.Color.Blue
        Me.lblOriginalCredit.Location = New System.Drawing.Point(300, 129)
        Me.lblOriginalCredit.Name = "lblOriginalCredit"
        Me.lblOriginalCredit.Size = New System.Drawing.Size(121, 20)
        Me.lblOriginalCredit.TabIndex = 11
        Me.lblOriginalCredit.Text = "Credit Due: 0.00"
        '
        'lblOriginalCheque
        '
        Me.lblOriginalCheque.AutoSize = True
        Me.lblOriginalCheque.ForeColor = System.Drawing.Color.DarkMagenta
        Me.lblOriginalCheque.Location = New System.Drawing.Point(300, 149)
        Me.lblOriginalCheque.Name = "lblOriginalCheque"
        Me.lblOriginalCheque.Size = New System.Drawing.Size(130, 20)
        Me.lblOriginalCheque.TabIndex = 12
        Me.lblOriginalCheque.Text = "Cheque Due: 0.00"
        '
        'lblNewBilling
        '
        Me.lblNewBilling.AutoSize = True
        Me.lblNewBilling.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblNewBilling.Location = New System.Drawing.Point(80, 179)
        Me.lblNewBilling.Name = "lblNewBilling"
        Me.lblNewBilling.Size = New System.Drawing.Size(103, 20)
        Me.lblNewBilling.TabIndex = 5
        Me.lblNewBilling.Text = "New Billing: -"
        '
        'lblNewStatus
        '
        Me.lblNewStatus.AutoSize = True
        Me.lblNewStatus.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblNewStatus.Location = New System.Drawing.Point(80, 199)
        Me.lblNewStatus.Name = "lblNewStatus"
        Me.lblNewStatus.Size = New System.Drawing.Size(103, 20)
        Me.lblNewStatus.TabIndex = 6
        Me.lblNewStatus.Text = "New Status: -"
        '
        'pnlCheques
        '
        Me.pnlCheques.Controls.Add(Me.lblChequeTitle)
        Me.pnlCheques.Controls.Add(Me.dgvCheques)
        Me.pnlCheques.Location = New System.Drawing.Point(17, 229)
        Me.pnlCheques.Name = "pnlCheques"
        Me.pnlCheques.Size = New System.Drawing.Size(566, 120)
        Me.pnlCheques.TabIndex = 7
        Me.pnlCheques.Visible = False
        '
        'lblChequeTitle
        '
        Me.lblChequeTitle.AutoSize = True
        Me.lblChequeTitle.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblChequeTitle.Location = New System.Drawing.Point(3, 0)
        Me.lblChequeTitle.Name = "lblChequeTitle"
        Me.lblChequeTitle.Size = New System.Drawing.Size(113, 20)
        Me.lblChequeTitle.TabIndex = 1
        Me.lblChequeTitle.Text = "Cheque Details"
        '
        'dgvCheques
        '
        Me.dgvCheques.AllowUserToAddRows = False
        Me.dgvCheques.AllowUserToDeleteRows = False
        Me.dgvCheques.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvCheques.BackgroundColor = System.Drawing.Color.White
        Me.dgvCheques.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvCheques.Location = New System.Drawing.Point(0, 25)
        Me.dgvCheques.Name = "dgvCheques"
        Me.dgvCheques.ReadOnly = True
        Me.dgvCheques.RowHeadersVisible = False
        Me.dgvCheques.RowHeadersWidth = 51
        Me.dgvCheques.RowTemplate.Height = 24
        Me.dgvCheques.Size = New System.Drawing.Size(566, 95)
        Me.dgvCheques.TabIndex = 0
        '
        'pnlPayments
        '
        Me.pnlPayments.Controls.Add(Me.lblPaymentTitle)
        Me.pnlPayments.Controls.Add(Me.dgvPayments)
        Me.pnlPayments.Location = New System.Drawing.Point(17, 359)
        Me.pnlPayments.Name = "pnlPayments"
        Me.pnlPayments.Size = New System.Drawing.Size(566, 120)
        Me.pnlPayments.TabIndex = 8
        Me.pnlPayments.Visible = False
        '
        'lblPaymentTitle
        '
        Me.lblPaymentTitle.AutoSize = True
        Me.lblPaymentTitle.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblPaymentTitle.Location = New System.Drawing.Point(3, 0)
        Me.lblPaymentTitle.Name = "lblPaymentTitle"
        Me.lblPaymentTitle.Size = New System.Drawing.Size(127, 20)
        Me.lblPaymentTitle.TabIndex = 1
        Me.lblPaymentTitle.Text = "Payment History"
        '
        'dgvPayments
        '
        Me.dgvPayments.AllowUserToAddRows = False
        Me.dgvPayments.AllowUserToDeleteRows = False
        Me.dgvPayments.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvPayments.BackgroundColor = System.Drawing.Color.White
        Me.dgvPayments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvPayments.Location = New System.Drawing.Point(0, 25)
        Me.dgvPayments.Name = "dgvPayments"
        Me.dgvPayments.ReadOnly = True
        Me.dgvPayments.RowHeadersVisible = False
        Me.dgvPayments.RowHeadersWidth = 51
        Me.dgvPayments.RowTemplate.Height = 24
        Me.dgvPayments.Size = New System.Drawing.Size(566, 95)
        Me.dgvPayments.TabIndex = 0
        '
        'btnYes
        '
        Me.btnYes.BackColor = System.Drawing.Color.SeaGreen
        Me.btnYes.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnYes.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold)
        Me.btnYes.ForeColor = System.Drawing.Color.White
        Me.btnYes.Location = New System.Drawing.Point(380, 499)
        Me.btnYes.Name = "btnYes"
        Me.btnYes.Size = New System.Drawing.Size(100, 35)
        Me.btnYes.TabIndex = 9
        Me.btnYes.Text = "Yes"
        Me.btnYes.UseVisualStyleBackColor = False
        '
        'btnNo
        '
        Me.btnNo.BackColor = System.Drawing.Color.IndianRed
        Me.btnNo.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnNo.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold)
        Me.btnNo.ForeColor = System.Drawing.Color.White
        Me.btnNo.Location = New System.Drawing.Point(486, 499)
        Me.btnNo.Name = "btnNo"
        Me.btnNo.Size = New System.Drawing.Size(100, 35)
        Me.btnNo.TabIndex = 10
        Me.btnNo.Text = "No"
        Me.btnNo.UseVisualStyleBackColor = False
        '
        'InvoiceUpdateWarning
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(600, 548)
        Me.Controls.Add(Me.btnNo)
        Me.Controls.Add(Me.btnYes)
        Me.Controls.Add(Me.pnlPayments)
        Me.Controls.Add(Me.pnlCheques)
        Me.Controls.Add(Me.lblNewStatus)
        Me.Controls.Add(Me.lblNewBilling)
        Me.Controls.Add(Me.lblOriginalCheque)
        Me.Controls.Add(Me.lblOriginalCredit)
        Me.Controls.Add(Me.lblOriginalStatus)
        Me.Controls.Add(Me.lblOriginalBilling)
        Me.Controls.Add(Me.lblDescription)
        Me.Controls.Add(Me.picWarning)
        Me.Controls.Add(Me.pnlHeader)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "InvoiceUpdateWarning"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Update Confirmation"
        Me.pnlHeader.ResumeLayout(False)
        Me.pnlHeader.PerformLayout()
        CType(Me.picWarning, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlCheques.ResumeLayout(False)
        Me.pnlCheques.PerformLayout()
        CType(Me.dgvCheques, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlPayments.ResumeLayout(False)
        Me.pnlPayments.PerformLayout()
        CType(Me.dgvPayments, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub

    Friend WithEvents pnlHeader As Panel
    Friend WithEvents lblTitle As Label
    Friend WithEvents picWarning As PictureBox
    Friend WithEvents lblDescription As Label
    Friend WithEvents lblOriginalBilling As Label
    Friend WithEvents lblOriginalStatus As Label
    Friend WithEvents lblOriginalCredit As Label
    Friend WithEvents lblOriginalCheque As Label
    Friend WithEvents lblNewBilling As Label
    Friend WithEvents lblNewStatus As Label
    Friend WithEvents pnlCheques As Panel
    Friend WithEvents lblChequeTitle As Label
    Friend WithEvents dgvCheques As DataGridView
    Friend WithEvents pnlPayments As Panel
    Friend WithEvents lblPaymentTitle As Label
    Friend WithEvents dgvPayments As DataGridView
    Friend WithEvents btnYes As Button
    Friend WithEvents btnNo As Button
End Class
