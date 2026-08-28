Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class RefundSettlementDialog
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
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblRefundDue = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.txtCashRefund = New System.Windows.Forms.TextBox()
        Me.txtStoreCredit = New System.Windows.Forms.TextBox()
        Me.txtCreditDeduction = New System.Windows.Forms.TextBox()
        Me.lblCashRefund = New System.Windows.Forms.Label()
        Me.lblStoreCredit = New System.Windows.Forms.Label()
        Me.lblCreditDeduction = New System.Windows.Forms.Label()
        Me.btnConfirm = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.LabelExplanation = New System.Windows.Forms.Label()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.Location = New System.Drawing.Point(20, 20)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(155, 21)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Total Refund Due :"
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
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.txtCashRefund)
        Me.GroupBox1.Controls.Add(Me.txtStoreCredit)
        Me.GroupBox1.Controls.Add(Me.txtCreditDeduction)
        Me.GroupBox1.Controls.Add(Me.lblCashRefund)
        Me.GroupBox1.Controls.Add(Me.lblStoreCredit)
        Me.GroupBox1.Controls.Add(Me.lblCreditDeduction)
        Me.GroupBox1.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.GroupBox1.Location = New System.Drawing.Point(24, 70)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(430, 160)
        Me.GroupBox1.TabIndex = 2
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Calculated Settlement (Waterfall Method)"
        '
        'txtCashRefund
        '
        Me.txtCashRefund.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.txtCashRefund.Location = New System.Drawing.Point(250, 110)
        Me.txtCashRefund.Name = "txtCashRefund"
        Me.txtCashRefund.ReadOnly = True
        Me.txtCashRefund.Size = New System.Drawing.Size(150, 27)
        Me.txtCashRefund.TabIndex = 5
        Me.txtCashRefund.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'txtStoreCredit
        '
        Me.txtStoreCredit.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.txtStoreCredit.Location = New System.Drawing.Point(250, 75)
        Me.txtStoreCredit.Name = "txtStoreCredit"
        Me.txtStoreCredit.ReadOnly = True
        Me.txtStoreCredit.Size = New System.Drawing.Size(150, 27)
        Me.txtStoreCredit.TabIndex = 4
        Me.txtStoreCredit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'txtCreditDeduction
        '
        Me.txtCreditDeduction.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.txtCreditDeduction.Location = New System.Drawing.Point(250, 40)
        Me.txtCreditDeduction.Name = "txtCreditDeduction"
        Me.txtCreditDeduction.ReadOnly = True
        Me.txtCreditDeduction.Size = New System.Drawing.Size(150, 27)
        Me.txtCreditDeduction.TabIndex = 3
        Me.txtCreditDeduction.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lblCashRefund
        '
        Me.lblCashRefund.AutoSize = True
        Me.lblCashRefund.Location = New System.Drawing.Point(20, 113)
        Me.lblCashRefund.Name = "lblCashRefund"
        Me.lblCashRefund.Size = New System.Drawing.Size(199, 19)
        Me.lblCashRefund.TabIndex = 2
        Me.lblCashRefund.Text = "To Cash Refund (Cleared Funds)"
        '
        'lblStoreCredit
        '
        Me.lblStoreCredit.AutoSize = True
        Me.lblStoreCredit.Location = New System.Drawing.Point(20, 78)
        Me.lblStoreCredit.Name = "lblStoreCredit"
        Me.lblStoreCredit.Size = New System.Drawing.Size(215, 19)
        Me.lblStoreCredit.TabIndex = 1
        Me.lblStoreCredit.Text = "To Store Credit (Uncleared Cheque)"
        '
        'lblCreditDeduction
        '
        Me.lblCreditDeduction.AutoSize = True
        Me.lblCreditDeduction.Location = New System.Drawing.Point(20, 43)
        Me.lblCreditDeduction.Name = "lblCreditDeduction"
        Me.lblCreditDeduction.Size = New System.Drawing.Size(189, 19)
        Me.lblCreditDeduction.TabIndex = 0
        Me.lblCreditDeduction.Text = "To Deduct from Unpaid Credit"
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
        'LabelExplanation
        '
        Me.LabelExplanation.ForeColor = System.Drawing.Color.DimGray
        Me.LabelExplanation.Location = New System.Drawing.Point(24, 240)
        Me.LabelExplanation.Name = "LabelExplanation"
        Me.LabelExplanation.Size = New System.Drawing.Size(430, 40)
        Me.LabelExplanation.TabIndex = 5
        Me.LabelExplanation.Text = "This safely restricts cash from being given against unpaid credit or uncleared cheques."
        '
        'RefundSettlementDialog
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(484, 351)
        Me.Controls.Add(Me.LabelExplanation)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnConfirm)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.lblRefundDue)
        Me.Controls.Add(Me.Label1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "RefundSettlementDialog"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Refund Settlement Priority"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents lblRefundDue As Label
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents lblCreditDeduction As Label
    Friend WithEvents lblStoreCredit As Label
    Friend WithEvents lblCashRefund As Label
    Friend WithEvents txtCreditDeduction As TextBox
    Friend WithEvents txtStoreCredit As TextBox
    Friend WithEvents txtCashRefund As TextBox
    Friend WithEvents btnConfirm As Button
    Friend WithEvents btnCancel As Button
    Friend WithEvents LabelExplanation As Label
End Class
