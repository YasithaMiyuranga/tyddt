Imports MySql.Data.MySqlClient

Public Class SaleReturnlog
    Dim sql As String
    Dim cmd As MySqlCommand
    Dim adapter As MySqlDataAdapter
    Dim table As DataTable

    Private Sub SaleReturnlog_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.KeyPreview = True
        ' Setup DataGridViews
        SetupGrid(dgvReturns)
        SetupGrid(dgvReturnItems)

        LoadAllReturns()
    End Sub

    Private Sub SetupGrid(dgv As DataGridView)
        dgv.ReadOnly = True
        dgv.AllowUserToAddRows = False
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgv.MultiSelect = False
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke

        ' Font size updates
        ' Header Font: 16, Bold
        dgv.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 16, FontStyle.Bold)
        dgv.ColumnHeadersHeight = 50

        ' Row Font: 14
        dgv.DefaultCellStyle.Font = New Font("Segoe UI", 14)
        dgv.RowTemplate.Height = 40
    End Sub

    Private Sub LoadAllReturns(Optional filter As String = "")
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()

            sql = "SELECT r.id AS 'Return ID', r.inv_no AS 'Invoice No', c.name AS 'Customer', " &
                  "r.return_date AS 'Date', r.refund_amount AS 'Refund', r.total_return_profit AS 'Loss (Profit)', " &
                  "r.cash_type AS 'Type', u.name AS 'Cashier' " &
                  "FROM sales_return r " &
                  "LEFT JOIN customer c ON r.customer_id = c.id " &
                  "LEFT JOIN user u ON r.user_id = u.id "

            If Not String.IsNullOrEmpty(filter) Then
                sql &= "WHERE r.inv_no LIKE @filter "
                sql &= "ORDER BY r.inv_no ASC"
            Else
                sql &= "ORDER BY r.return_date DESC"
            End If

            cmd = New MySqlCommand(sql, conn)
            If Not String.IsNullOrEmpty(filter) Then
                cmd.Parameters.AddWithValue("@filter", filter & "%")
            End If

            table = New DataTable()
            adapter = New MySqlDataAdapter(cmd)
            adapter.Fill(table)
            dgvReturns.DataSource = table

            ' Clear details if main grid is refreshed
            dgvReturnItems.DataSource = Nothing

        Catch ex As Exception
            MessageBox.Show("Error loading returns: " & ex.Message)
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub LoadReturnItems(returnId As Integer)
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()

            sql = "SELECT ri.item_id AS 'Item ID', ri.description AS 'Description', " &
                  "ri.qty AS 'Qty', ri.unit_price AS 'Price', ri.discount AS 'Dis%', " &
                  "ri.return_amount AS 'Amount', ri.return_profit AS 'Profit Loss', ri.reason AS 'Reason' " &
                  "FROM sales_return_items ri " &
                  "WHERE ri.return_id = @rid"

            cmd = New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@rid", returnId)

            Dim dtItems As New DataTable()
            adapter = New MySqlDataAdapter(cmd)
            adapter.Fill(dtItems)
            dgvReturnItems.DataSource = dtItems

        Catch ex As Exception
            MessageBox.Show("Error loading return items: " & ex.Message)
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub txtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
        LoadAllReturns(txtSearch.Text.Trim())
        
        ' Auto-select first row to show internal items immediately after filtering
        If dgvReturns.Rows.Count > 0 Then
            dgvReturns.Rows(0).Selected = True
            Dim rid As Integer = Convert.ToInt32(dgvReturns.Rows(0).Cells("Return ID").Value)
            LoadReturnItems(rid)
        End If
    End Sub

    Private Sub dgvReturns_SelectionChanged(sender As Object, e As EventArgs) Handles dgvReturns.SelectionChanged
        If dgvReturns.SelectedRows.Count > 0 Then
            Dim row As DataGridViewRow = dgvReturns.SelectedRows(0)
            If row.Cells("Return ID").Value IsNot Nothing AndAlso Not IsDBNull(row.Cells("Return ID").Value) Then
                Dim rid As Integer = Convert.ToInt32(row.Cells("Return ID").Value)
                LoadReturnItems(rid)
            End If
        Else
            dgvReturnItems.DataSource = Nothing
        End If
    End Sub

    ' Ensure items load even if already selected row is clicked
    Private Sub dgvReturns_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvReturns.CellClick
        If e.RowIndex >= 0 Then
            Dim rid As Integer = Convert.ToInt32(dgvReturns.Rows(e.RowIndex).Cells("Return ID").Value)
            LoadReturnItems(rid)
        End If
    End Sub

    Private Sub SaleReturnlog_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Escape Then Me.Close()
    End Sub

    Private Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click
        If dgvReturns.SelectedRows.Count > 0 Then
            Dim row As DataGridViewRow = dgvReturns.SelectedRows(0)
            Dim invNo As String = row.Cells("Invoice No").Value.ToString()
            
            ' Open SaleInv with Stock Return report type (index 3)
            SaleInv.ShowReport(invNo, 3)
        Else
            MessageBox.Show("Please select a return record from the list first.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub btnPrintAll_Click(sender As Object, e As EventArgs) Handles btnPrintAll.Click
        ' Open SaleInv with Stock Return report type (index 3) and show "ALL" records
        SaleInv.ShowReport("ALL", 3)
    End Sub
End Class