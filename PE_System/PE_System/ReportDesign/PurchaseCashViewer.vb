Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared

Public Class PurchaseCashViewer
    ' 0 = Cash Only (pur_type=Cash, p_method=Cash)
    ' 1 = Cards/Online (pur_type=Cash, p_method=Credit Card/Debit Card/Online Transfer)
    Private _filterMode As Integer = 0

    Public Sub ShowReport(filterMode As Integer)
        _filterMode = filterMode
    End Sub

    Private Sub PurchaseCashViewer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        dtpFrom.Value = DateTime.Now
        dtpTo.Value = DateTime.Now

        Select Case _filterMode
            Case 0
                lblReportType.Text = "Purchase Cash Bills - Cash Only"
                Me.Text = "Purchase: Cash Only"
            Case 1
                lblReportType.Text = "Purchase Cash Bills - Cards / Online"
                Me.Text = "Purchase: Cards / Online"
        End Select

        LoadData()
    End Sub

    Private Sub btnShow_Click(sender As Object, e As EventArgs) Handles btnShow.Click
        LoadData()
    End Sub

    Private Sub LoadData()
        Try
            Dim rpt As New PurchaseCashReport()

            ' Build payment filter
            Dim payFilter As String = ""
            Select Case _filterMode
                Case 0  ' Cash Only
                    payFilter = "LCase(Trim({Command.Payment Method})) = 'cash'"
                Case 1  ' Cards/Online
                    payFilter = "LCase(Trim({Command.Payment Method})) IN ['credit card','debit card','online transfer']"
                Case Else
                    payFilter = "1=1"
            End Select

            ' Build date filter
            Dim dateFilter As String =
                "DATE({Command.Purchase Date}) >= Date(" &
                dtpFrom.Value.Year & "," & dtpFrom.Value.Month & "," & dtpFrom.Value.Day & ") AND " &
                "DATE({Command.Purchase Date}) <= Date(" &
                dtpTo.Value.Year & "," & dtpTo.Value.Month & "," & dtpTo.Value.Day & ")"

            ' Apply DB connection credentials
            SetReportConnection(rpt)

            ' Apply selection formula
            rpt.RecordSelectionFormula = payFilter & " AND " & dateFilter

            CrystalReportViewer1.ReportSource = rpt
            CrystalReportViewer1.RefreshReport()

        Catch ex As Exception
            MessageBox.Show("Error loading report: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub SetReportConnection(ByRef rpt As ReportDocument)
        Try
            Dim connStr = Module1.ConnStr
            Dim parts = connStr.Split(";"c)
            Dim server = "", db = "", user = "", pass = ""

            For Each part In parts
                Dim kv = part.Split("="c)
                If kv.Length >= 2 Then
                    Dim key = kv(0).Trim().ToLower()
                    Dim val = String.Join("=", kv, 1, kv.Length - 1).Trim().Replace(vbCr, "").Replace(vbLf, "")
                    If key = "server" OrElse key = "data source" OrElse key = "host" Then server = val
                    If key = "database" OrElse key = "initial catalog" Then db = val
                    If key = "userid" OrElse key = "user id" OrElse key = "uid" Then user = val
                    If key = "password" OrElse key = "pwd" Then pass = val
                End If
            Next

            For Each tbl As Table In rpt.Database.Tables
                Dim tblLogOn = tbl.LogOnInfo
                tblLogOn.ConnectionInfo.UserID = user
                tblLogOn.ConnectionInfo.Password = pass
                tblLogOn.ConnectionInfo.IntegratedSecurity = False
                tbl.ApplyLogOnInfo(tblLogOn)
            Next

            If rpt.DataSourceConnections.Count > 0 Then
                rpt.SetDatabaseLogon(user, pass, rpt.DataSourceConnections(0).ServerName, rpt.DataSourceConnections(0).DatabaseName)
            End If

            ' Format report to 3 decimal places
            Module1.FormatReportDecimals(rpt)

        Catch ex As Exception
            ' Silent fail – fallback to stored connection
        End Try
    End Sub

    Private Sub btnExportCsv_Click(sender As Object, e As EventArgs) Handles btnExportCsv.Click
        Try
            Dim sfd As New SaveFileDialog()
            sfd.Filter = "PDF Files (*.pdf)|*.pdf|Excel Files (*.xls)|*.xls"
            sfd.FileName = "Purchase_Cash_" & If(_filterMode = 0, "CashOnly", "CardsOnline") & "_" & DateTime.Now.ToString("yyyyMMdd")
            If sfd.ShowDialog() = DialogResult.OK Then
                If sfd.FilterIndex = 1 Then
                    CrystalReportViewer1.ReportSource.ExportToDisk(ExportFormatType.PortableDocFormat, sfd.FileName)
                Else
                    CrystalReportViewer1.ReportSource.ExportToDisk(ExportFormatType.Excel, sfd.FileName)
                End If
                MessageBox.Show("Exported to: " & sfd.FileName, "Export", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MessageBox.Show("Export error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class
