Imports MySql.Data.MySqlClient
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Collections.Generic

Public Class PreviousPurchaseRequestsForm
    Public Property SelectedRequestId As String = ""
    Public Property SelectedSupplierId As Integer = 0
    Public Property SelectedSupplierName As String = ""
    Public Property SelectedPoNumber As String = ""
    Public Property SelectedLocationId As Integer = 1

    Private conn As New MySqlConnection(Module1.ConnStr)

    Private Sub PreviousPurchaseRequestsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Apply modern styling
        Me.Text = "Previous Purchase Requests"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.BackColor = Color.FromArgb(245, 246, 248)
        
        setup_grid_style(dgvRequests)
        setup_grid_style(dgvItems)
        
        dtpFrom.Value = DateTime.Now.AddDays(-30)
        dtpTo.Value = DateTime.Now
        
        LoadRequests()
    End Sub

    Private Sub setup_grid_style(dgv As DataGridView)
        dgv.AllowUserToAddRows = False
        dgv.AllowUserToDeleteRows = False
        dgv.ReadOnly = True
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgv.RowHeadersVisible = False
        dgv.BackgroundColor = Color.White
        dgv.BorderStyle = BorderStyle.None
        dgv.DefaultCellStyle.Font = New Font("Segoe UI", 10)
        dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219)
        dgv.DefaultCellStyle.SelectionForeColor = Color.White
        dgv.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI Semibold", 11, FontStyle.Bold)
        dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(44, 62, 80)
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        dgv.EnableHeadersVisualStyles = False
        dgv.RowTemplate.Height = 32
    End Sub

    Public Sub LoadRequests()
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()
            Dim query As String = "SELECT r.request_id As 'P/O No', r.request_date As 'Date', " &
                                 "r.supplier_name As 'Supplier', " &
                                 "l.location_name As 'Location', r.items_qty As 'Items Qty', " &
                                 "r.total_amount As 'Total Amount', r.status As 'Status', " &
                                 "r.supplier_id, r.location_id " &
                                 "FROM purchase_request r " &
                                 "LEFT JOIN location l ON r.location_id = l.id " &
                                 "WHERE r.request_date >= @start AND r.request_date <= @end " &
                                 "ORDER BY r.request_date DESC"
            
            Dim cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@start", dtpFrom.Value.Date)
            cmd.Parameters.AddWithValue("@end", dtpTo.Value.Date.AddDays(1).AddSeconds(-1))
            
            Dim da As New MySqlDataAdapter(cmd)
            Dim dt As New DataTable()
            da.Fill(dt)
            
            Dim dv As New DataView(dt)
            Dim filters As New List(Of String)()
            
            ' Apply search textbox filter
            If Not String.IsNullOrEmpty(txtSearch.Text.Trim()) Then
                Dim term As String = txtSearch.Text.Trim().Replace("'", "''")
                filters.Add(String.Format("(Convert([P/O No], 'System.String') Like '{0}%' OR Convert(Supplier, 'System.String') Like '{0}%')", term))
            End If
            
            If filters.Count > 0 Then
                dv.RowFilter = String.Join(" AND ", filters)
            End If
            
            dgvRequests.DataSource = dv
            
            ' Format Grid Columns
            If dgvRequests.Columns.Count > 0 Then
                dgvRequests.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                
                dgvRequests.Columns("P/O No").FillWeight = 110
                dgvRequests.Columns("Date").FillWeight = 130
                dgvRequests.Columns("Supplier").FillWeight = 200
                dgvRequests.Columns("Location").FillWeight = 90
                dgvRequests.Columns("Items Qty").FillWeight = 70
                dgvRequests.Columns("Total Amount").FillWeight = 90
                dgvRequests.Columns("Total Amount").DefaultCellStyle.Format = "N2"
                dgvRequests.Columns("Status").FillWeight = 70
                
                ' Hide raw ids
                If dgvRequests.Columns.Contains("supplier_id") Then dgvRequests.Columns("supplier_id").Visible = False
                If dgvRequests.Columns.Contains("location_id") Then dgvRequests.Columns("location_id").Visible = False
            End If
            
            conn.Close()
            
            ' Force selection changed to refresh child items grid
            LoadRequestItems()
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then conn.Close()
            MessageBox.Show("Error loading previous purchase requests: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadRequestItems()
        If dgvRequests.SelectedRows.Count > 0 Then
            Dim reqId As String = dgvRequests.SelectedRows(0).Cells("P/O No").Value.ToString()
            Try
                If conn.State = ConnectionState.Closed Then conn.Open()
                Dim query As String = "SELECT item_id As 'Item ID', description As 'Description', qty As 'Qty', " &
                                     "item_cost As 'Cost', amount As 'Amount' " &
                                     "FROM purchase_request_items WHERE request_id = @req"
                Dim cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@req", reqId)
                
                Dim da As New MySqlDataAdapter(cmd)
                Dim dt As New DataTable()
                da.Fill(dt)
                
                dgvItems.DataSource = dt
                
                ' Style columns
                If dgvItems.Columns.Count > 0 Then
                    dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                    dgvItems.Columns("Item ID").FillWeight = 80
                    dgvItems.Columns("Description").FillWeight = 200
                    dgvItems.Columns("Qty").FillWeight = 60
                    dgvItems.Columns("Cost").FillWeight = 80
                    dgvItems.Columns("Cost").DefaultCellStyle.Format = "N2"
                    dgvItems.Columns("Amount").FillWeight = 80
                    dgvItems.Columns("Amount").DefaultCellStyle.Format = "N2"
                End If
                
                conn.Close()
            Catch ex As Exception
                If conn.State = ConnectionState.Open Then conn.Close()
            End Try
        Else
            dgvItems.DataSource = Nothing
        End If
    End Sub

    Private Sub dgvRequests_SelectionChanged(sender As Object, e As EventArgs) Handles dgvRequests.SelectionChanged
        LoadRequestItems()
    End Sub

    Private Sub txtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
        LoadRequests()
    End Sub

    Private Sub dtpFrom_ValueChanged(sender As Object, e As EventArgs) Handles dtpFrom.ValueChanged, dtpTo.ValueChanged
        LoadRequests()
    End Sub

    Private Sub btnImport_Click(sender As Object, e As EventArgs) Handles btnImport.Click
        SelectRequestAndClose()
    End Sub

    Private Sub dgvRequests_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvRequests.CellDoubleClick
        If e.RowIndex >= 0 Then
            SelectRequestAndClose()
        End If
    End Sub

    Private Sub SelectRequestAndClose()
        If dgvRequests.SelectedRows.Count > 0 Then
            Dim row As DataGridViewRow = dgvRequests.SelectedRows(0)
            SelectedRequestId = row.Cells("P/O No").Value.ToString()
            SelectedSupplierName = row.Cells("Supplier").Value.ToString()
            SelectedPoNumber = ""
            
            If IsNumeric(row.Cells("supplier_id").Value) Then
                SelectedSupplierId = Convert.ToInt32(row.Cells("supplier_id").Value)
            End If
            
            If IsNumeric(row.Cells("location_id").Value) Then
                SelectedLocationId = Convert.ToInt32(row.Cells("location_id").Value)
            End If
            
            ' Launch Crystal Report viewer - keep this form open so user stays on the same page
            Try
                Dim rptForm As New SaleInv()
                rptForm.ShowReport(SelectedRequestId, 13)
            Catch ex As Exception
                MessageBox.Show("Error displaying purchase request report: " & ex.Message, "Report Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Else
            MessageBox.Show("Please select a purchase request from the list first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub
End Class
