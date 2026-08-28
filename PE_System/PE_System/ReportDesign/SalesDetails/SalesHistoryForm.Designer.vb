<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SalesHistoryForm
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
        Me.dtpFrom = New System.Windows.Forms.DateTimePicker()
        Me.dtpTo = New System.Windows.Forms.DateTimePicker()
        Me.btnShow = New System.Windows.Forms.Button()
        Me.CrystalReportViewer1 = New CrystalDecisions.Windows.Forms.CrystalReportViewer()
        Me.lblFrom = New System.Windows.Forms.Label()
        Me.lblTo = New System.Windows.Forms.Label()
        Me.lblReportType = New System.Windows.Forms.Label()
        Me.cmbReportType = New System.Windows.Forms.ComboBox()
        Me.cmbPrinter = New System.Windows.Forms.ComboBox()
        Me.lblPrinter = New System.Windows.Forms.Label()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.lblSearchName = New System.Windows.Forms.Label()
        Me.txtSearchName = New System.Windows.Forms.TextBox()
        Me.dgvSearchName = New System.Windows.Forms.DataGridView()
        Me.lblSearchInv = New System.Windows.Forms.Label()
        Me.txtSearchInv = New System.Windows.Forms.TextBox()
        Me.dgvSearchInv = New System.Windows.Forms.DataGridView()
        Me.chkAllDates = New System.Windows.Forms.CheckBox()
        CType(Me.dgvSearchName, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgvSearchInv, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        '
        'dtpFrom
        '
        Me.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtpFrom.Location = New System.Drawing.Point(50, 12)
        Me.dtpFrom.Name = "dtpFrom"
        Me.dtpFrom.Size = New System.Drawing.Size(80, 22)
        Me.dtpFrom.TabIndex = 0
        '
        'dtpTo
        '
        Me.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtpTo.Location = New System.Drawing.Point(165, 12)
        Me.dtpTo.Name = "dtpTo"
        Me.dtpTo.Size = New System.Drawing.Size(80, 22)
        Me.dtpTo.TabIndex = 1
        '
        'chkAllDates
        '
        Me.chkAllDates.AutoSize = False
        Me.chkAllDates.Location = New System.Drawing.Point(250, 14)
        Me.chkAllDates.Name = "chkAllDates"
        Me.chkAllDates.Size = New System.Drawing.Size(80, 21)
        Me.chkAllDates.TabIndex = 15
        Me.chkAllDates.Text = "All Dates"
        Me.chkAllDates.UseVisualStyleBackColor = True
        '
        'btnShow
        '
        Me.btnShow.Location = New System.Drawing.Point(910, 9)
        Me.btnShow.Name = "btnShow"
        Me.btnShow.Size = New System.Drawing.Size(70, 28)
        Me.btnShow.TabIndex = 4
        Me.btnShow.Text = "Show"
        Me.btnShow.UseVisualStyleBackColor = True
        '
        'lblReportType
        '
        Me.lblReportType.AutoSize = True
        Me.lblReportType.Location = New System.Drawing.Point(250, 15)
        Me.lblReportType.Name = "lblReportType"
        Me.lblReportType.Size = New System.Drawing.Size(44, 17)
        Me.lblReportType.TabIndex = 6
        Me.lblReportType.Text = "Type:"
        '
        'lblSearchName
        '
        Me.lblSearchName.AutoSize = True
        Me.lblSearchName.Location = New System.Drawing.Point(440, 15)
        Me.lblSearchName.Name = "lblSearchName"
        Me.lblSearchName.Size = New System.Drawing.Size(107, 17)
        Me.lblSearchName.TabIndex = 10
        Me.lblSearchName.Text = "Search Customer:"
        '
        'txtSearchName
        '
        Me.txtSearchName.Location = New System.Drawing.Point(550, 12)
        Me.txtSearchName.Name = "txtSearchName"
        Me.txtSearchName.Size = New System.Drawing.Size(120, 22)
        Me.txtSearchName.TabIndex = 3
        '
        'dgvSearchName
        '
        Me.dgvSearchName.AllowUserToAddRows = False
        Me.dgvSearchName.AllowUserToDeleteRows = False
        Me.dgvSearchName.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvSearchName.BackgroundColor = System.Drawing.Color.White
        Me.dgvSearchName.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvSearchName.ColumnHeadersVisible = False
        Me.dgvSearchName.Location = New System.Drawing.Point(550, 36)
        Me.dgvSearchName.MultiSelect = False
        Me.dgvSearchName.Name = "dgvSearchName"
        Me.dgvSearchName.ReadOnly = True
        Me.dgvSearchName.RowHeadersVisible = False
        Me.dgvSearchName.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvSearchName.Size = New System.Drawing.Size(250, 200)
        Me.dgvSearchName.TabIndex = 13
        Me.dgvSearchName.Visible = False
        '
        'lblSearchInv
        '
        Me.lblSearchInv.AutoSize = True
        Me.lblSearchInv.Location = New System.Drawing.Point(680, 15)
        Me.lblSearchInv.Name = "lblSearchInv"
        Me.lblSearchInv.Size = New System.Drawing.Size(95, 17)
        Me.lblSearchInv.TabIndex = 12
        Me.lblSearchInv.Text = "Search Invoice:"
        '
        'txtSearchInv
        '
        Me.txtSearchInv.Location = New System.Drawing.Point(780, 12)
        Me.txtSearchInv.Name = "txtSearchInv"
        Me.txtSearchInv.Size = New System.Drawing.Size(120, 22)
        Me.txtSearchInv.TabIndex = 2
        '
        'dgvSearchInv
        '
        Me.dgvSearchInv.AllowUserToAddRows = False
        Me.dgvSearchInv.AllowUserToDeleteRows = False
        Me.dgvSearchInv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvSearchInv.BackgroundColor = System.Drawing.Color.White
        Me.dgvSearchInv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvSearchInv.ColumnHeadersVisible = False
        Me.dgvSearchInv.Location = New System.Drawing.Point(780, 36)
        Me.dgvSearchInv.MultiSelect = False
        Me.dgvSearchInv.Name = "dgvSearchInv"
        Me.dgvSearchInv.ReadOnly = True
        Me.dgvSearchInv.RowHeadersVisible = False
        Me.dgvSearchInv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvSearchInv.Size = New System.Drawing.Size(150, 200)
        Me.dgvSearchInv.TabIndex = 14
        Me.dgvSearchInv.Visible = False
        '
        'cmbReportType
        '
        Me.cmbReportType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbReportType.FormattingEnabled = True
        Me.cmbReportType.Location = New System.Drawing.Point(310, 12)
        Me.cmbReportType.Name = "cmbReportType"
        Me.cmbReportType.Size = New System.Drawing.Size(120, 24)
        Me.cmbReportType.TabIndex = 5
        '
        'CrystalReportViewer1
        '
        Me.CrystalReportViewer1.ActiveViewIndex = -1
        Me.CrystalReportViewer1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CrystalReportViewer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.CrystalReportViewer1.Cursor = System.Windows.Forms.Cursors.Default
        Me.CrystalReportViewer1.Location = New System.Drawing.Point(0, 48)
        Me.CrystalReportViewer1.Name = "CrystalReportViewer1"
        Me.CrystalReportViewer1.Size = New System.Drawing.Size(1200, 700)
        Me.CrystalReportViewer1.TabIndex = 9
        Me.CrystalReportViewer1.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None
        '
        'lblFrom
        '
        Me.lblFrom.AutoSize = True
        Me.lblFrom.Location = New System.Drawing.Point(10, 15)
        Me.lblFrom.Name = "lblFrom"
        Me.lblFrom.Size = New System.Drawing.Size(44, 17)
        Me.lblFrom.TabIndex = 6
        Me.lblFrom.Text = "From:"
        '
        'lblTo
        '
        Me.lblTo.AutoSize = True
        Me.lblTo.Location = New System.Drawing.Point(135, 15)
        Me.lblTo.Name = "lblTo"
        Me.lblTo.Size = New System.Drawing.Size(29, 17)
        Me.lblTo.TabIndex = 7
        Me.lblTo.Text = "To:"
        '
        'cmbPrinter
        '
        Me.cmbPrinter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbPrinter.FormattingEnabled = True
        Me.cmbPrinter.Location = New System.Drawing.Point(1040, 12)
        Me.cmbPrinter.Name = "cmbPrinter"
        Me.cmbPrinter.Size = New System.Drawing.Size(90, 24)
        Me.cmbPrinter.TabIndex = 7
        '
        'lblPrinter
        '
        Me.lblPrinter.AutoSize = True
        Me.lblPrinter.Location = New System.Drawing.Point(990, 15)
        Me.lblPrinter.Name = "lblPrinter"
        Me.lblPrinter.Size = New System.Drawing.Size(54, 17)
        Me.lblPrinter.TabIndex = 8
        Me.lblPrinter.Text = "Printer:"
        '
        'btnPrint
        '
        Me.btnPrint.Location = New System.Drawing.Point(1140, 9)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(60, 28)
        Me.btnPrint.TabIndex = 8
        Me.btnPrint.Text = "Print"
        Me.btnPrint.UseVisualStyleBackColor = True
        '
        'SalesHistoryForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1200, 750)
        Me.AutoScroll = True
        Me.Controls.Add(Me.btnPrint)
        Me.Controls.Add(Me.lblPrinter)
        Me.Controls.Add(Me.cmbPrinter)
        Me.Controls.Add(Me.lblTo)
        Me.Controls.Add(Me.dgvSearchName)
        Me.Controls.Add(Me.dgvSearchInv)
        Me.Controls.Add(Me.chkAllDates)
        Me.Controls.Add(Me.lblSearchName)
        Me.Controls.Add(Me.txtSearchName)
        Me.Controls.Add(Me.lblSearchInv)
        Me.Controls.Add(Me.txtSearchInv)
        Me.Controls.Add(Me.lblFrom)
        Me.Controls.Add(Me.lblReportType)
        Me.Controls.Add(Me.cmbReportType)
        Me.Controls.Add(Me.CrystalReportViewer1)
        Me.Controls.Add(Me.btnShow)
        Me.Controls.Add(Me.dtpTo)
        Me.Controls.Add(Me.dtpFrom)
        Me.Name = "SalesHistoryForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Sales History Report"
        CType(Me.dgvSearchName, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgvSearchInv, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents dtpFrom As DateTimePicker
    Friend WithEvents dtpTo As DateTimePicker
    Friend WithEvents btnShow As Button
    Friend WithEvents CrystalReportViewer1 As CrystalDecisions.Windows.Forms.CrystalReportViewer
    Friend WithEvents lblFrom As Label
    Friend WithEvents lblTo As Label
    Friend WithEvents lblReportType As Label
    Friend WithEvents cmbReportType As ComboBox
    Friend WithEvents cmbPrinter As ComboBox
    Friend WithEvents lblPrinter As Label
    Friend WithEvents btnPrint As Button
    Friend WithEvents lblSearchName As Label
    Friend WithEvents txtSearchName As TextBox
    Friend WithEvents dgvSearchName As DataGridView
    Friend WithEvents lblSearchInv As Label
    Friend WithEvents txtSearchInv As TextBox
    Friend WithEvents dgvSearchInv As DataGridView
    Friend WithEvents chkAllDates As System.Windows.Forms.CheckBox
End Class
