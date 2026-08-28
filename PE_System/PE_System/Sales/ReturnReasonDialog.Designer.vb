<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ReturnReasonDialog
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
        Me.lblReason = New System.Windows.Forms.Label()
        Me.txtReason = New System.Windows.Forms.TextBox()
        Me.chkStock = New System.Windows.Forms.CheckBox()
        Me.btnActionReturn = New System.Windows.Forms.Button()
        Me.btnActionCancel = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'lblReason
        '
        Me.lblReason.AutoSize = True
        Me.lblReason.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblReason.Location = New System.Drawing.Point(20, 18)
        Me.lblReason.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblReason.Name = "lblReason"
        Me.lblReason.Size = New System.Drawing.Size(317, 23)
        Me.lblReason.TabIndex = 0
        Me.lblReason.Text = "Please enter the custom return reason:"
        '
        'txtReason
        '
        Me.txtReason.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtReason.Location = New System.Drawing.Point(20, 55)
        Me.txtReason.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.txtReason.Multiline = True
        Me.txtReason.Name = "txtReason"
        Me.txtReason.Size = New System.Drawing.Size(465, 131)
        Me.txtReason.TabIndex = 1
        '
        'chkStock
        '
        Me.chkStock.AutoSize = True
        Me.chkStock.Checked = True
        Me.chkStock.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkStock.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkStock.Location = New System.Drawing.Point(20, 194)
        Me.chkStock.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.chkStock.Name = "chkStock"
        Me.chkStock.Size = New System.Drawing.Size(366, 27)
        Me.chkStock.TabIndex = 2
        Me.chkStock.Text = "Add returned item back to Stock (Inventory)"
        Me.chkStock.UseVisualStyleBackColor = True
        '
        'btnActionReturn
        '
        Me.btnActionReturn.BackColor = System.Drawing.Color.LightGreen
        Me.btnActionReturn.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.btnActionReturn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnActionReturn.Location = New System.Drawing.Point(263, 242)
        Me.btnActionReturn.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.btnActionReturn.Name = "btnActionReturn"
        Me.btnActionReturn.Size = New System.Drawing.Size(107, 37)
        Me.btnActionReturn.TabIndex = 3
        Me.btnActionReturn.Text = "Return"
        Me.btnActionReturn.UseVisualStyleBackColor = False
        '
        'btnActionCancel
        '
        Me.btnActionCancel.BackColor = System.Drawing.Color.White
        Me.btnActionCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnActionCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnActionCancel.Location = New System.Drawing.Point(378, 242)
        Me.btnActionCancel.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.btnActionCancel.Name = "btnActionCancel"
        Me.btnActionCancel.Size = New System.Drawing.Size(107, 37)
        Me.btnActionCancel.TabIndex = 4
        Me.btnActionCancel.Text = "Cancel"
        Me.btnActionCancel.UseVisualStyleBackColor = False
        '
        'ReturnReasonDialog
        '
        Me.AcceptButton = Me.btnActionReturn
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnActionCancel
        Me.ClientSize = New System.Drawing.Size(533, 308)
        Me.Controls.Add(Me.btnActionCancel)
        Me.Controls.Add(Me.btnActionReturn)
        Me.Controls.Add(Me.chkStock)
        Me.Controls.Add(Me.txtReason)
        Me.Controls.Add(Me.lblReason)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ReturnReasonDialog"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Custom Return Reason"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblReason As System.Windows.Forms.Label
    Friend WithEvents txtReason As System.Windows.Forms.TextBox
    Friend WithEvents chkStock As System.Windows.Forms.CheckBox
    Friend WithEvents btnActionReturn As System.Windows.Forms.Button
    Friend WithEvents btnActionCancel As System.Windows.Forms.Button

End Class
