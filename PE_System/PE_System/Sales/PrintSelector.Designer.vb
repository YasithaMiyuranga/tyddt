<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PrintSelector
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
        Me.lblPrinterReturn = New System.Windows.Forms.Label()
        Me.cmbPrinterReturn = New System.Windows.Forms.ComboBox()
        Me.lblPrinterSale = New System.Windows.Forms.Label()
        Me.cmbPrinterSale = New System.Windows.Forms.ComboBox()
        Me.lblBillType = New System.Windows.Forms.Label()
        Me.cmbBillType = New System.Windows.Forms.ComboBox()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.chkNoSalePrint = New System.Windows.Forms.CheckBox()
        Me.SuspendLayout()
        '
        'lblPrinterReturn
        '
        Me.lblPrinterReturn.AutoSize = True
        Me.lblPrinterReturn.Location = New System.Drawing.Point(20, 10)
        Me.lblPrinterReturn.Name = "lblPrinterReturn"
        Me.lblPrinterReturn.Size = New System.Drawing.Size(107, 13)
        Me.lblPrinterReturn.TabIndex = 0
        Me.lblPrinterReturn.Text = "Return Note Printer:"
        '
        'cmbPrinterReturn
        '
        Me.cmbPrinterReturn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbPrinterReturn.FormattingEnabled = True
        Me.cmbPrinterReturn.Location = New System.Drawing.Point(20, 25)
        Me.cmbPrinterReturn.Name = "cmbPrinterReturn"
        Me.cmbPrinterReturn.Size = New System.Drawing.Size(240, 21)
        Me.cmbPrinterReturn.TabIndex = 1
        '
        'lblPrinterSale
        '
        Me.lblPrinterSale.AutoSize = True
        Me.lblPrinterSale.Location = New System.Drawing.Point(20, 55)
        Me.lblPrinterSale.Name = "lblPrinterSale"
        Me.lblPrinterSale.Size = New System.Drawing.Size(89, 13)
        Me.lblPrinterSale.TabIndex = 10
        Me.lblPrinterSale.Text = "Sale Bill Printer:"
        '
        'cmbPrinterSale
        '
        Me.cmbPrinterSale.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbPrinterSale.FormattingEnabled = True
        Me.cmbPrinterSale.Location = New System.Drawing.Point(20, 70)
        Me.cmbPrinterSale.Name = "cmbPrinterSale"
        Me.cmbPrinterSale.Size = New System.Drawing.Size(240, 21)
        Me.cmbPrinterSale.TabIndex = 11
        '
        'lblBillType
        '
        Me.lblBillType.AutoSize = True
        Me.lblBillType.Location = New System.Drawing.Point(20, 100)
        Me.lblBillType.Name = "lblBillType"
        Me.lblBillType.Size = New System.Drawing.Size(81, 13)
        Me.lblBillType.TabIndex = 2
        Me.lblBillType.Text = "Select Bill Type:"
        '
        'cmbBillType
        '
        Me.cmbBillType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbBillType.FormattingEnabled = True
        Me.cmbBillType.Location = New System.Drawing.Point(20, 115)
        Me.cmbBillType.Name = "cmbBillType"
        Me.cmbBillType.Size = New System.Drawing.Size(240, 21)
        Me.cmbBillType.TabIndex = 3
        '
        'btnPrint
        '
        Me.btnPrint.BackColor = System.Drawing.Color.LightBlue
        Me.btnPrint.Location = New System.Drawing.Point(40, 185)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(100, 35)
        Me.btnPrint.TabIndex = 4
        Me.btnPrint.Text = "Print"
        Me.btnPrint.UseVisualStyleBackColor = False
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(150, 185)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(100, 35)
        Me.btnCancel.TabIndex = 5
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'chkNoSalePrint
        '
        Me.chkNoSalePrint.AutoSize = True
        Me.chkNoSalePrint.Location = New System.Drawing.Point(20, 145)
        Me.chkNoSalePrint.Name = "chkNoSalePrint"
        Me.chkNoSalePrint.Size = New System.Drawing.Size(161, 17)
        Me.chkNoSalePrint.TabIndex = 6
        Me.chkNoSalePrint.Text = "No Sale Bill Print (Not Needed)"
        Me.chkNoSalePrint.UseVisualStyleBackColor = True
        Me.chkNoSalePrint.Visible = False
        '
        'PrintSelector
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(284, 240)
        Me.Controls.Add(Me.cmbPrinterSale)
        Me.Controls.Add(Me.lblPrinterSale)
        Me.Controls.Add(Me.chkNoSalePrint)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnPrint)
        Me.Controls.Add(Me.cmbBillType)
        Me.Controls.Add(Me.lblBillType)
        Me.Controls.Add(Me.cmbPrinterReturn)
        Me.Controls.Add(Me.lblPrinterReturn)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "PrintSelector"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Print Options"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblPrinterReturn As System.Windows.Forms.Label
    Friend WithEvents cmbPrinterReturn As System.Windows.Forms.ComboBox
    Friend WithEvents lblPrinterSale As System.Windows.Forms.Label
    Friend WithEvents cmbPrinterSale As System.Windows.Forms.ComboBox
    Friend WithEvents lblBillType As System.Windows.Forms.Label
    Friend WithEvents cmbBillType As System.Windows.Forms.ComboBox
    Friend WithEvents btnPrint As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents chkNoSalePrint As System.Windows.Forms.CheckBox
End Class
