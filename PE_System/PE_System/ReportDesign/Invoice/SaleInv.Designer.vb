<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SaleInv
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
        Me.btnShowInvoice = New System.Windows.Forms.Button()
        Me.txtInvoiceNo = New System.Windows.Forms.TextBox()
        Me.CrystalReportViewer1 = New CrystalDecisions.Windows.Forms.CrystalReportViewer()
        Me.cmbReportType = New System.Windows.Forms.ComboBox()
        Me.lblReportType = New System.Windows.Forms.Label()
        Me.lblInvoiceNo = New System.Windows.Forms.Label()
        Me.cmbPrinter = New System.Windows.Forms.ComboBox()
        Me.lblPrinter = New System.Windows.Forms.Label()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'btnShowInvoice
        '
        Me.btnShowInvoice.Location = New System.Drawing.Point(820, 10)
        Me.btnShowInvoice.Margin = New System.Windows.Forms.Padding(1)
        Me.btnShowInvoice.Name = "btnShowInvoice"
        Me.btnShowInvoice.Size = New System.Drawing.Size(150, 28)
        Me.btnShowInvoice.TabIndex = 6
        Me.btnShowInvoice.Text = "Show Invoice"
        Me.btnShowInvoice.UseVisualStyleBackColor = True
        '
        'txtInvoiceNo
        '
        Me.txtInvoiceNo.Location = New System.Drawing.Point(550, 12)
        Me.txtInvoiceNo.Margin = New System.Windows.Forms.Padding(1)
        Me.txtInvoiceNo.Name = "txtInvoiceNo"
        Me.txtInvoiceNo.Size = New System.Drawing.Size(250, 22)
        Me.txtInvoiceNo.TabIndex = 5
        '
        'cmbReportType
        '
        Me.cmbReportType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbReportType.FormattingEnabled = True
        Me.cmbReportType.Location = New System.Drawing.Point(110, 12)
        Me.cmbReportType.Name = "cmbReportType"
        Me.cmbReportType.Size = New System.Drawing.Size(200, 24)
        Me.cmbReportType.TabIndex = 8
        '
        'lblReportType
        '
        Me.lblReportType.AutoSize = True
        Me.lblReportType.Location = New System.Drawing.Point(12, 15)
        Me.lblReportType.Name = "lblReportType"
        Me.lblReportType.Size = New System.Drawing.Size(92, 17)
        Me.lblReportType.TabIndex = 9
        Me.lblReportType.Text = "Report Type:"
        '
        'lblInvoiceNo
        '
        Me.lblInvoiceNo.AutoSize = True
        Me.lblInvoiceNo.Location = New System.Drawing.Point(330, 15)
        Me.lblInvoiceNo.Name = "lblInvoiceNo"
        Me.lblInvoiceNo.Size = New System.Drawing.Size(78, 17)
        Me.lblInvoiceNo.TabIndex = 10
        Me.lblInvoiceNo.Text = "Invoice No:"
        '
        'CrystalReportViewer1
        '
        Me.CrystalReportViewer1.ActiveViewIndex = -1
        Me.CrystalReportViewer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.CrystalReportViewer1.Cursor = System.Windows.Forms.Cursors.Default
        Me.CrystalReportViewer1.Location = New System.Drawing.Point(0, 44)
        Me.CrystalReportViewer1.Margin = New System.Windows.Forms.Padding(4)
        Me.CrystalReportViewer1.Name = "CrystalReportViewer1"
        Me.CrystalReportViewer1.Size = New System.Drawing.Size(1529, 550)
        Me.CrystalReportViewer1.TabIndex = 7
        Me.CrystalReportViewer1.ToolPanelWidth = 267
        Me.CrystalReportViewer1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        '
        'cmbPrinter
        '
        Me.cmbPrinter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbPrinter.FormattingEnabled = True
        Me.cmbPrinter.Location = New System.Drawing.Point(1050, 12)
        Me.cmbPrinter.Name = "cmbPrinter"
        Me.cmbPrinter.Size = New System.Drawing.Size(250, 24)
        Me.cmbPrinter.TabIndex = 11
        '
        'lblPrinter
        '
        Me.lblPrinter.AutoSize = True
        Me.lblPrinter.Location = New System.Drawing.Point(990, 15)
        Me.lblPrinter.Name = "lblPrinter"
        Me.lblPrinter.Size = New System.Drawing.Size(54, 17)
        Me.lblPrinter.TabIndex = 12
        Me.lblPrinter.Text = "Printer:"
        '
        'btnPrint
        '
        Me.btnPrint.Location = New System.Drawing.Point(1310, 10)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(100, 28)
        Me.btnPrint.TabIndex = 13
        Me.btnPrint.Text = "Print"
        Me.btnPrint.UseVisualStyleBackColor = True
        '
        'SaleInv
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1529, 600)
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.AutoScroll = True
        Me.Controls.Add(Me.btnPrint)
        Me.Controls.Add(Me.lblPrinter)
        Me.Controls.Add(Me.cmbPrinter)
        Me.Controls.Add(Me.lblInvoiceNo)
        Me.Controls.Add(Me.lblReportType)
        Me.Controls.Add(Me.cmbReportType)
        Me.Controls.Add(Me.CrystalReportViewer1)
        Me.Controls.Add(Me.btnShowInvoice)
        Me.Controls.Add(Me.txtInvoiceNo)
        Me.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.Name = "SaleInv"
        Me.Text = "Sales Invoice Report"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents btnShowInvoice As Button
    Friend WithEvents txtInvoiceNo As TextBox
    Friend WithEvents CrystalReportViewer1 As CrystalDecisions.Windows.Forms.CrystalReportViewer
    Friend WithEvents cmbReportType As ComboBox
    Friend WithEvents lblReportType As Label
    Friend WithEvents lblInvoiceNo As Label
    Friend WithEvents cmbPrinter As ComboBox
    Friend WithEvents lblPrinter As Label
    Friend WithEvents btnPrint As Button
End Class
