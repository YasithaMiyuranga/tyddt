Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SupplierRefundSettlementDialog
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

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.lblTotalLabel = New System.Windows.Forms.Label()
        Me.lblRefundDue = New System.Windows.Forms.Label()
        Me.groupBox1 = New System.Windows.Forms.GroupBox()
        Me.txtCash = New System.Windows.Forms.TextBox()
        Me.txtStoreCredit = New System.Windows.Forms.TextBox()
        Me.txtCredit = New System.Windows.Forms.TextBox()
        Me.lblCash = New System.Windows.Forms.Label()
        Me.lblStoreCredit = New System.Windows.Forms.Label()
        Me.lblCredit = New System.Windows.Forms.Label()
        Me.btnConfirm = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.lblExplanation = New System.Windows.Forms.Label()
        Me.groupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblTotalLabel
        '
        Me.lblTotalLabel.AutoSize = True
        Me.lblTotalLabel.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblTotalLabel.Location = New System.Drawing.Point(20, 20)
        Me.lblTotalLabel.Name = "lblTotalLabel"
        Me.lblTotalLabel.Size = New System.Drawing.Size(155, 21)
        Me.lblTotalLabel.TabIndex = 0
        Me.lblTotalLabel.Text = "Total Refund Due :"
        '
        'lblRefundDue
        '
        Me.lblRefundDue.AutoSize = True
        Me.lblRefundDue.Font = New System.Drawing.Font("Segoe UI", 14.0!, System.Drawing.FontStyle.Bold)
        Me.lblRefundDue.ForeColor = System.Drawing.Color.MediumBlue
        Me.lblRefundDue.Location = New System.Drawing.Point(180, 18)
        Me.lblRefundDue.Name = "lblRefundDue"
        Me.lblRefundDue.Size = New System.Drawing.Size(50, 25)
        Me.lblRefundDue.TabIndex = 1
        Me.lblRefundDue.Text = "0.00"
        '
        'groupBox1
        '
        Me.groupBox1.Controls.Add(Me.txtCash)
        Me.groupBox1.Controls.Add(Me.txtStoreCredit)
        Me.groupBox1.Controls.Add(Me.txtCredit)
        Me.groupBox1.Controls.Add(Me.lblCash)
        Me.groupBox1.Controls.Add(Me.lblStoreCredit)
        Me.groupBox1.Controls.Add(Me.lblCredit)
        Me.groupBox1.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.groupBox1.Location = New System.Drawing.Point(24, 70)
        Me.groupBox1.Name = "groupBox1"
        Me.groupBox1.Size = New System.Drawing.Size(430, 160)
        Me.groupBox1.TabIndex = 2
        Me.groupBox1.TabStop = False
        Me.groupBox1.Text = "Calculated Settlement (Waterfall Method)"
        '
        'txtCash
        '
        Me.txtCash.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.txtCash.Location = New System.Drawing.Point(300, 110)
        Me.txtCash.Name = "txtCash"
        Me.txtCash.Size = New System.Drawing.Size(100, 27)
        Me.txtCash.TabIndex = 5
        Me.txtCash.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'txtStoreCredit
        '
        Me.txtStoreCredit.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.txtStoreCredit.Location = New System.Drawing.Point(300, 75)
        Me.txtStoreCredit.Name = "txtStoreCredit"
        Me.txtStoreCredit.Size = New System.Drawing.Size(100, 27)
        Me.txtStoreCredit.TabIndex = 4
        Me.txtStoreCredit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'txtCredit
        '
        Me.txtCredit.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.txtCredit.Location = New System.Drawing.Point(300, 40)
        Me.txtCredit.Name = "txtCredit"
        Me.txtCredit.ReadOnly = True
        Me.txtCredit.Size = New System.Drawing.Size(100, 27)
        Me.txtCredit.TabIndex = 3
        Me.txtCredit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lblCash
        '
        Me.lblCash.AutoSize = True
        Me.lblCash.Location = New System.Drawing.Point(20, 113)
        Me.lblCash.Name = "lblCash"
        Me.lblCash.Size = New System.Drawing.Size(248, 19)
        Me.lblCash.TabIndex = 2
        Me.lblCash.Text = "To Cash Refund (Received Cash Back)"
        '
        'lblStoreCredit
        '
        Me.lblStoreCredit.AutoSize = True
        Me.lblStoreCredit.Location = New System.Drawing.Point(20, 78)
        Me.lblStoreCredit.Name = "lblStoreCredit"
        Me.lblStoreCredit.Size = New System.Drawing.Size(262, 19)
        Me.lblStoreCredit.TabIndex = 1
        Me.lblStoreCredit.Text = "To Supplier Credit Note (Kept by Supp)"
        '
        'lblCredit
        '
        Me.lblCredit.AutoSize = True
        Me.lblCredit.Location = New System.Drawing.Point(20, 43)
        Me.lblCredit.Name = "lblCredit"
        Me.lblCredit.Size = New System.Drawing.Size(189, 19)
        Me.lblCredit.TabIndex = 0
        Me.lblCredit.Text = "To Deduct from Unpaid Credit"
        '
        'btnConfirm
        '
        Me.btnConfirm.BackColor = System.Drawing.Color.DeepSkyBlue
        Me.btnConfirm.FlatAppearance.BorderSize = 0
        Me.btnConfirm.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnConfirm.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
        Me.btnConfirm.Location = New System.Drawing.Point(234, 290)
        Me.btnConfirm.Name = "btnConfirm"
        Me.btnConfirm.Size = New System.Drawing.Size(100, 40)
        Me.btnConfirm.TabIndex = 3
        Me.btnConfirm.Text = "Confirm"
        Me.btnConfirm.UseVisualStyleBackColor = False
        '
        'btnCancel
        '
        Me.btnCancel.BackColor = System.Drawing.Color.Tomato
        Me.btnCancel.FlatAppearance.BorderSize = 0
        Me.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCancel.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
        Me.btnCancel.ForeColor = System.Drawing.Color.White
        Me.btnCancel.Location = New System.Drawing.Point(354, 290)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(100, 40)
        Me.btnCancel.TabIndex = 4
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = False
        '
        'lblExplanation
        '
        Me.lblExplanation.ForeColor = System.Drawing.Color.DimGray
        Me.lblExplanation.Location = New System.Drawing.Point(24, 240)
        Me.lblExplanation.Name = "lblExplanation"
        Me.lblExplanation.Size = New System.Drawing.Size(430, 40)
        Me.lblExplanation.TabIndex = 5
        Me.lblExplanation.Text = "This ensures you do not log false cash receipts if the supplier did not actually give you cash back."
        '
        'SupplierRefundSettlementDialog
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(484, 351)
        Me.Controls.Add(Me.lblExplanation)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnConfirm)
        Me.Controls.Add(Me.groupBox1)
        Me.Controls.Add(Me.lblRefundDue)
        Me.Controls.Add(Me.lblTotalLabel)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "SupplierRefundSettlementDialog"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Purchase Refund Settlement Priority"
        Me.groupBox1.ResumeLayout(False)
        Me.groupBox1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblTotalLabel As Label
    Friend WithEvents lblRefundDue As Label
    Friend WithEvents groupBox1 As GroupBox
    Friend WithEvents lblCredit As Label
    Friend WithEvents lblStoreCredit As Label
    Friend WithEvents lblCash As Label
    Friend WithEvents txtCredit As TextBox
    Friend WithEvents txtStoreCredit As TextBox
    Friend WithEvents txtCash As TextBox
    Friend WithEvents btnConfirm As Button
    Friend WithEvents btnCancel As Button
    Friend WithEvents lblExplanation As Label
End Class
