<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class PurchaseCashViewer
    Inherits System.Windows.Forms.Form

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

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()

        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.lblSummary = New System.Windows.Forms.Label()
        Me.btnExportCsv = New System.Windows.Forms.Button()
        Me.dtpTo = New System.Windows.Forms.DateTimePicker()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.dtpFrom = New System.Windows.Forms.DateTimePicker()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnShow = New System.Windows.Forms.Button()
        Me.lblReportType = New System.Windows.Forms.Label()

        Me.CrystalReportViewer1 = New CrystalDecisions.Windows.Forms.CrystalReportViewer()

        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()

        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.lblSummary)
        Me.Panel1.Controls.Add(Me.btnExportCsv)
        Me.Panel1.Controls.Add(Me.dtpTo)
        Me.Panel1.Controls.Add(Me.Label2)
        Me.Panel1.Controls.Add(Me.dtpFrom)
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Controls.Add(Me.btnShow)
        Me.Panel1.Controls.Add(Me.lblReportType)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1200, 50)
        Me.Panel1.TabIndex = 0

        '
        'lblReportType
        '
        Me.lblReportType.AutoSize = True
        Me.lblReportType.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
        Me.lblReportType.Location = New System.Drawing.Point(12, 15)
        Me.lblReportType.Name = "lblReportType"
        Me.lblReportType.Size = New System.Drawing.Size(160, 23)
        Me.lblReportType.TabIndex = 0
        Me.lblReportType.Text = "Purchase Cash Bills"

        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(290, 17)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(42, 16)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "From:"

        '
        'dtpFrom
        '
        Me.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtpFrom.Location = New System.Drawing.Point(340, 14)
        Me.dtpFrom.Name = "dtpFrom"
        Me.dtpFrom.Size = New System.Drawing.Size(120, 22)
        Me.dtpFrom.TabIndex = 2

        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(470, 17)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(23, 16)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "To:"

        '
        'dtpTo
        '
        Me.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtpTo.Location = New System.Drawing.Point(500, 14)
        Me.dtpTo.Name = "dtpTo"
        Me.dtpTo.Size = New System.Drawing.Size(120, 22)
        Me.dtpTo.TabIndex = 4

        '
        'btnShow
        '
        Me.btnShow.Location = New System.Drawing.Point(635, 10)
        Me.btnShow.Name = "btnShow"
        Me.btnShow.Size = New System.Drawing.Size(120, 30)
        Me.btnShow.TabIndex = 5
        Me.btnShow.Text = "Show Report"
        Me.btnShow.UseVisualStyleBackColor = True

        '
        'btnExportCsv
        '
        Me.btnExportCsv.BackColor = System.Drawing.Color.FromArgb(46, 204, 113)
        Me.btnExportCsv.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnExportCsv.ForeColor = System.Drawing.Color.White
        Me.btnExportCsv.Location = New System.Drawing.Point(765, 10)
        Me.btnExportCsv.Name = "btnExportCsv"
        Me.btnExportCsv.Size = New System.Drawing.Size(120, 30)
        Me.btnExportCsv.TabIndex = 6
        Me.btnExportCsv.Text = "Export CSV"
        Me.btnExportCsv.UseVisualStyleBackColor = False

        '
        'lblSummary
        '
        Me.lblSummary.AutoSize = True
        Me.lblSummary.Location = New System.Drawing.Point(900, 17)
        Me.lblSummary.Name = "lblSummary"
        Me.lblSummary.Size = New System.Drawing.Size(0, 16)
        Me.lblSummary.TabIndex = 7

        '
        'CrystalReportViewer1
        '
        Me.CrystalReportViewer1.ActiveViewIndex = -1
        Me.CrystalReportViewer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.CrystalReportViewer1.Cursor = System.Windows.Forms.Cursors.Default
        Me.CrystalReportViewer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CrystalReportViewer1.Location = New System.Drawing.Point(0, 50)
        Me.CrystalReportViewer1.Name = "CrystalReportViewer1"
        Me.CrystalReportViewer1.Size = New System.Drawing.Size(1200, 650)
        Me.CrystalReportViewer1.TabIndex = 1

        '
        'PurchaseCashViewer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1200, 700)
        Me.Controls.Add(Me.CrystalReportViewer1)
        Me.Controls.Add(Me.Panel1)
        Me.Name = "PurchaseCashViewer"
        Me.Text = "Purchase Cash Bills Report"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized

        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Panel1 As Panel
    Friend WithEvents lblReportType As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents dtpFrom As DateTimePicker
    Friend WithEvents Label2 As Label
    Friend WithEvents dtpTo As DateTimePicker
    Friend WithEvents btnShow As Button
    Friend WithEvents btnExportCsv As Button
    Friend WithEvents lblSummary As Label
    Friend WithEvents CrystalReportViewer1 As CrystalDecisions.Windows.Forms.CrystalReportViewer

End Class