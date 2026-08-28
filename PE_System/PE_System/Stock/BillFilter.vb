Imports MySql.Data.MySqlClient

Public Class BillFilter
    Private Sub BillFilter_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PopulateInvoiceTypes()
        
        cmbInvType.SelectedIndex = 0 ' Default to "All"
        dtpStart.Value = DateTime.Now.AddDays(-30) ' Last 30 days
        dtpEnd.Value = DateTime.Now
        
        ' Style the grid
        SetupGrid()

        ' Enable key preview for shortcuts
        Me.KeyPreview = True

        LoadData()
    End Sub

    Private Sub PopulateInvoiceTypes()
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()
            Dim cmd As New MySqlCommand("SELECT IF(printed_inv_no IS NULL OR printed_inv_no = '', inv_no, printed_inv_no) AS inv_no FROM billing UNION SELECT IF(printed_inv_no IS NULL OR printed_inv_no = '', inv_no, printed_inv_no) AS inv_no FROM quotation_billing", conn)
            Dim da As New MySqlDataAdapter(cmd)
            Dim dt As New DataTable()
            da.Fill(dt)
            
            Dim prefixes As New HashSet(Of String)()
            For Each row As DataRow In dt.Rows
                Dim inv As String = row("inv_no").ToString()
                Dim prefix As String = New String(inv.TakeWhile(Function(c) Char.IsLetter(c)).ToArray())
                If Not String.IsNullOrEmpty(prefix) Then
                    prefixes.Add(prefix.ToUpper())
                End If
            Next
            
            cmbInvType.Items.Clear()
            cmbInvType.Items.Add("All")
            
            Dim sortedPrefixes = prefixes.ToList()
            sortedPrefixes.Sort()
            
            For Each p As String In sortedPrefixes
                If p <> "RGR" AndAlso p <> "RE" Then
                    If Not Module1.IsRgrVisible AndAlso p.ToUpper() = "GR" Then
                        Continue For
                    End If
                    cmbInvType.Items.Add(p)
                End If
            Next
            
            ' Add missing defaults if not found in DB
            If Module1.IsRgrVisible Then
                If Not cmbInvType.Items.Contains("GR") Then cmbInvType.Items.Add("GR")
            End If
            If Not cmbInvType.Items.Contains("EL") Then cmbInvType.Items.Add("EL")
            If Not cmbInvType.Items.Contains("QT") Then cmbInvType.Items.Add("QT")
            If Not cmbInvType.Items.Contains("VT") Then cmbInvType.Items.Add("VT")
            
            conn.Close()
        Catch ex As Exception
            If cmbInvType.Items.Count = 0 Then
                If Module1.IsRgrVisible Then
                    cmbInvType.Items.AddRange(New Object() {"All", "GR", "EL", "QT", "VT"})
                Else
                    cmbInvType.Items.AddRange(New Object() {"All", "EL", "QT", "VT"})
                End If
            End If
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub SetupGrid()
        dgvBills.ReadOnly = True
        dgvBills.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvBills.AllowUserToAddRows = False
        dgvBills.RowHeadersVisible = False
        
        ' Enable double buffering to reduce flicker
        Try
            Dim dgvType As Type = dgvBills.GetType()
            Dim pi As System.Reflection.PropertyInfo = dgvType.GetProperty("DoubleBuffered", 
                System.Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.NonPublic)
            pi.SetValue(dgvBills, True, Nothing)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub LoadData()
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()
            
            Dim query As String = ""
            Dim invTypeFilter As String = cmbInvType.Text
            Dim searchVal As String = txtSearch.Text.Trim()
            
            Dim sqlParts As New List(Of String)()
            
            ' Part 1: Standard Billing table
            Dim subQuery1 As String = "SELECT IF(printed_inv_no IS NULL OR printed_inv_no = '', inv_no, printed_inv_no) as inv_no, timestamps as 'Date', grand_total as 'Amount', billing_type as 'Method', 'Sale' as 'Type', status " &
                                   "FROM billing WHERE timestamps >= @start AND timestamps <= @end"

            If invTypeFilter = "All" Then
                subQuery1 &= " AND IF(printed_inv_no IS NULL OR printed_inv_no = '', inv_no, printed_inv_no) NOT LIKE 'RGR%'"
                If Not Module1.IsRgrVisible Then
                    subQuery1 &= " AND IF(printed_inv_no IS NULL OR printed_inv_no = '', inv_no, printed_inv_no) NOT LIKE 'GR%'"
                    subQuery1 &= " AND IF(printed_inv_no IS NULL OR printed_inv_no = '', inv_no, printed_inv_no) NOT LIKE 'gr%'"
                End If
            Else
                subQuery1 &= " AND IF(printed_inv_no IS NULL OR printed_inv_no = '', inv_no, printed_inv_no) LIKE '" & invTypeFilter & "%'"
            End If

            If Not String.IsNullOrEmpty(searchVal) Then
                subQuery1 &= " AND IF(printed_inv_no IS NULL OR printed_inv_no = '', inv_no, printed_inv_no) LIKE @search"
            End If

            sqlParts.Add(subQuery1)
            
            ' Part 2: Quotation Billing table
            Dim subQuery2 As String = "SELECT IF(printed_inv_no IS NULL OR printed_inv_no = '', inv_no, printed_inv_no) as inv_no, timestamps as 'Date', grand_total as 'Amount', billing_type as 'Method', 'Quotation' as 'Type', 'quote' as status " &
                                   "FROM quotation_billing WHERE timestamps >= @start AND timestamps <= @end"
            
            If invTypeFilter <> "All" Then
                subQuery2 &= " AND IF(printed_inv_no IS NULL OR printed_inv_no = '', inv_no, printed_inv_no) LIKE '" & invTypeFilter & "%'"
            End If
            
            If Not String.IsNullOrEmpty(searchVal) Then
                subQuery2 &= " AND IF(printed_inv_no IS NULL OR printed_inv_no = '', inv_no, printed_inv_no) LIKE @search"
            End If
            
            sqlParts.Add(subQuery2)
            
            query = String.Join(" UNION ALL ", sqlParts)
            
            If Not String.IsNullOrEmpty(searchVal) Then
                query &= " ORDER BY inv_no ASC"
            Else
                query &= " ORDER BY Date DESC"
            End If
            
            Dim cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@start", dtpStart.Value.ToString("yyyy-MM-dd") & " 00:00:00")
            cmd.Parameters.AddWithValue("@end", dtpEnd.Value.ToString("yyyy-MM-dd") & " 23:59:59")
            If Not String.IsNullOrEmpty(searchVal) Then
                cmd.Parameters.AddWithValue("@search", searchVal & "%")
            End If
            
            Dim da As New MySqlDataAdapter(cmd)
            Dim dt As New DataTable()
            da.Fill(dt)
            
            dgvBills.DataSource = dt
            
            ' Format columns
            If dgvBills.Columns.Contains("Amount") Then
                dgvBills.Columns("Amount").DefaultCellStyle.Format = "N2"
            End If
            
            conn.Close()
        Catch ex As Exception
            MessageBox.Show("Error loading bills: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub txtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
        LoadData()
    End Sub

    Private Sub FilterCriteria_Changed(sender As Object, e As EventArgs) Handles dtpStart.ValueChanged, dtpEnd.ValueChanged, cmbInvType.SelectedIndexChanged
        LoadData()
    End Sub

    Private Sub BillFilter_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        ' Ctrl + A shortcut to toggle RGR bills
        If e.Control AndAlso e.KeyCode = Keys.A Then
            If cmbInvType.Text = "RGR" Then
                ' If already on RGR, switch back to All
                cmbInvType.SelectedIndex = 0 ' Index 0 is "All"
            Else
                ' If not on RGR, switch to RGR
                If Not cmbInvType.Items.Contains("RGR") Then
                    cmbInvType.Items.Add("RGR")
                End If
                cmbInvType.Text = "RGR"
            End If
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub dgvBills_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvBills.CellDoubleClick
        If e.RowIndex >= 0 Then
            Dim invNo As String = dgvBills.Rows(e.RowIndex).Cells("inv_no").Value.ToString()
            
            ' Open in SaleInv report viewer
            Try
                Dim rptForm As New SaleInv()
                rptForm.MdiParent = Me.MdiParent
                rptForm.Show()
                ' Pass the invoice number and report type (0 = Standard Invoice)
                rptForm.ShowReport(invNo, 0, False, False, "", 1, 0, True)
            Catch ex As Exception
                MessageBox.Show("Error opening report: " & ex.Message)
            End Try
        End If
    End Sub

    Private Sub Btnprint_Click(sender As Object, e As EventArgs) Handles Btnprint.Click
        Try
            Dim invTypeFilter As String = cmbInvType.Text
            Dim searchVal As String = txtSearch.Text.Trim()
            
            Dim isQT As Boolean = (invTypeFilter = "QT" OrElse searchVal.ToUpper().StartsWith("QT"))
            
            ' Create report instance
            Dim rpt As CrystalDecisions.CrystalReports.Engine.ReportDocument
            If isQT Then
                rpt = New Quate()
            Else
                rpt = New billdetails()
            End If
            
            ' Apply database logon info
            Dim connStr As String = Module1.ConnStr
            Dim parts As String() = connStr.Split(";"c)
            Dim server As String = "", db As String = "", user As String = "", pass As String = ""

            For Each part In parts
                Dim kv As String() = part.Split("="c)
                If kv.Length >= 2 Then
                    Dim key As String = kv(0).Trim().ToLower()
                    Dim val As String = String.Join("=", kv, 1, kv.Length - 1).Trim().Replace(vbCr, "").Replace(vbLf, "")
                    If key = "server" OrElse key = "data source" OrElse key = "host" Then server = val
                    If key = "database" OrElse key = "initial catalog" Then db = val
                    If key = "userid" OrElse key = "user id" OrElse key = "uid" Then user = val
                    If key = "password" OrElse key = "pwd" Then pass = val
                End If
            Next

            For Each tbl As CrystalDecisions.CrystalReports.Engine.Table In rpt.Database.Tables
                Dim tblLogOn = tbl.LogOnInfo
                tblLogOn.ConnectionInfo.UserID = user
                tblLogOn.ConnectionInfo.Password = pass
                tblLogOn.ConnectionInfo.IntegratedSecurity = False
                tbl.ApplyLogOnInfo(tblLogOn)
            Next

            ' Build Record Selection Formula based on current UI filters
            Dim tPrefix As String = If(isQT, "quotation_billing1", "billing1")
            
            Dim formula As String = "DATE({" & tPrefix & ".timestamps}) >= Date(" & dtpStart.Value.ToString("yyyy,MM,dd") & ") AND DATE({" & tPrefix & ".timestamps}) <= Date(" & dtpEnd.Value.ToString("yyyy,MM,dd") & ")"

            If invTypeFilter <> "All" Then
                formula &= " AND (If IsNull({" & tPrefix & ".printed_inv_no}) Or {" & tPrefix & ".printed_inv_no} = '' Then {" & tPrefix & ".inv_no} Else {" & tPrefix & ".printed_inv_no}) startswith '" & invTypeFilter & "'"
            Else
                formula &= " AND NOT((If IsNull({" & tPrefix & ".printed_inv_no}) Or {" & tPrefix & ".printed_inv_no} = '' Then {" & tPrefix & ".inv_no} Else {" & tPrefix & ".printed_inv_no}) startswith 'RGR')"
                If Not Module1.IsRgrVisible Then
                    formula &= " AND NOT((If IsNull({" & tPrefix & ".printed_inv_no}) Or {" & tPrefix & ".printed_inv_no} = '' Then {" & tPrefix & ".inv_no} Else {" & tPrefix & ".printed_inv_no}) startswith 'GR')"
                    formula &= " AND NOT((If IsNull({" & tPrefix & ".printed_inv_no}) Or {" & tPrefix & ".printed_inv_no} = '' Then {" & tPrefix & ".inv_no} Else {" & tPrefix & ".printed_inv_no}) startswith 'gr')"
                End If
            End If

            If Not String.IsNullOrEmpty(searchVal) Then
                Dim safeSearch As String = searchVal.Replace("'", "''")
                formula &= " AND (If IsNull({" & tPrefix & ".printed_inv_no}) Or {" & tPrefix & ".printed_inv_no} = '' Then {" & tPrefix & ".inv_no} Else {" & tPrefix & ".printed_inv_no}) like '*" & safeSearch & "*'"
            End If
            
            rpt.RecordSelectionFormula = formula

            ' Apply report decimal formatting
            Module1.FormatReportDecimals(rpt)

            ' Show Report Viewer Form
            Dim viewerForm As New Form()
            viewerForm.Text = "Bill Details Report"
            viewerForm.WindowState = FormWindowState.Maximized
            viewerForm.Icon = Me.Icon ' Inherit icon if possible
            
            Dim viewer As New CrystalDecisions.Windows.Forms.CrystalReportViewer()
            viewer.Dock = DockStyle.Fill
            viewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None
            viewer.ReportSource = rpt
            viewer.RefreshReport()
            
            viewerForm.Controls.Add(viewer)
            viewerForm.ShowDialog()

        Catch ex As Exception
            MessageBox.Show("Error printing report: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public Sub ApplySecurityLock()
        PopulateInvoiceTypes()
        If cmbInvType.Items.Count > 0 AndAlso Not cmbInvType.Items.Contains(cmbInvType.Text) Then
            cmbInvType.SelectedIndex = 0
        End If
        LoadData()
    End Sub
End Class

