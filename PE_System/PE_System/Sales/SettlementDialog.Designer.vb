<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SettlementDialog
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
        Me.btnCredit = New System.Windows.Forms.Button()
        Me.btnCheque = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'btnCredit
        '
        Me.btnCredit.BackColor = System.Drawing.Color.FromArgb(CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer), CType(CType(48, Byte), Integer))
        Me.btnCredit.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCredit.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCredit.ForeColor = System.Drawing.Color.White
        Me.btnCredit.Location = New System.Drawing.Point(35, 90)
        Me.btnCredit.Name = "btnCredit"
        Me.btnCredit.Size = New System.Drawing.Size(150, 60)
        Me.btnCredit.TabIndex = 0
        Me.btnCredit.Text = "CREDIT"
        Me.btnCredit.UseVisualStyleBackColor = False
        '
        'btnCheque
        '
        Me.btnCheque.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(122, Byte), Integer), CType(CType(204, Byte), Integer))
        Me.btnCheque.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCheque.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCheque.ForeColor = System.Drawing.Color.White
        Me.btnCheque.Location = New System.Drawing.Point(215, 90)
        Me.btnCheque.Name = "btnCheque"
        Me.btnCheque.Size = New System.Drawing.Size(150, 60)
        Me.btnCheque.TabIndex = 1
        Me.btnCheque.Text = "CHEQUE"
        Me.btnCheque.UseVisualStyleBackColor = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Segoe UI", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(60, 30)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(280, 25)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Settle remaining balance via:"
        '
        'SettlementDialog
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(400, 200)
        Me.AutoScroll = True
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnCheque)
        Me.Controls.Add(Me.btnCredit)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "SettlementDialog"
        Me.KeyPreview = True
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Settlement Option"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents btnCredit As Button
    Friend WithEvents btnCheque As Button
    Friend WithEvents Label1 As Label
End Class
