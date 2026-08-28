Imports MySql.Data.MySqlClient

Public Class ManagementAlerts

    Private Sub ManagementAlerts_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Apply styling to grids
        ApplyGridStyles(dgvCustCredits)
        ApplyGridStyles(dgvBlockedCust)
        ApplyGridStyles(dgvSupCredits)
        ApplyGridStyles(dgvSupAlerts)
        ApplyGridStyles(dgvCustChequeReturns)
        ApplyGridStyles(dgvSupChequeReturns)
        
        LoadAllData()
    End Sub

    Private Sub ApplyGridStyles(dgv As DataGridView)
        dgv.BackgroundColor = Color.White
        dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245)
        dgv.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 10, FontStyle.Bold)
        dgv.DefaultCellStyle.Font = New Font("Segoe UI", 10)
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgv.EnableHeadersVisualStyles = False
        dgv.RowHeadersVisible = False
        Module1.EnableDoubleBuffered(dgv)
    End Sub

    Private Sub LoadAllData()
        Try
            Using conn As New MySqlConnection(Module1.ConnStr)
                conn.Open()

                ' 1. Customer Credits (Older than 2 Months)
                Dim sql1 = "SELECT cc.inv_no AS 'Bill ID', c.name AS 'Customer', cc.amount AS 'Amount', DATE_FORMAT(cc.timestamps, '%Y-%m-%d') AS 'Date' " &
                         "FROM customer_credit cc " &
                         "JOIN customer c ON cc.customer_id = c.id " &
                         "WHERE cc.is_active = 1 AND cc.amount > 0 AND cc.timestamps <= DATE_SUB(NOW(), INTERVAL 2 MONTH) ORDER BY cc.timestamps ASC"
                FillGrid(dgvCustCredits, sql1, conn)

                ' 2. Blocked Customers
                Dim sql2 = "SELECT c.name AS 'Customer', c.tel_no AS 'Phone', " &
                         "CASE WHEN c.is_block = 1 THEN 'Manual' ELSE 'Auto Alert' END AS 'Type', " &
                         "c.credit_limit AS 'Limit', " &
                         "COALESCE((SELECT SUM(balance_due) FROM billing WHERE customer_id = c.id AND balance_due > 0), 0) AS 'Outstanding' " &
                         "FROM customer c " &
                         "WHERE c.is_block = 1 OR (c.credit_limit > 0 AND COALESCE((SELECT SUM(balance_due) FROM billing WHERE customer_id = c.id AND balance_due > 0), 0) > c.credit_limit)"
                FillGrid(dgvBlockedCust, sql2, conn)

                ' 3. Supplier Credits
                Dim sql3 = "SELECT p.pur_id AS 'Pur ID', s.name AS 'Supplier', p.balance_due AS 'Balance', DATE_FORMAT(p.pur_date, '%Y-%m-%d') AS 'Date' " &
                         "FROM purchasing p " &
                         "JOIN supplier s ON p.supplier_id = s.id " &
                         "WHERE p.balance_due > 0 ORDER BY p.pur_date ASC"
                FillGrid(dgvSupCredits, sql3, conn)

                ' 4. Supplier Alerts
                Dim sql4 = "SELECT p.pur_id AS 'Pur ID', s.name AS 'Supplier', p.balance_due AS 'Amount', DATE_FORMAT(s.debit_period, '%Y-%m-%d') AS 'Due Date' " &
                         "FROM purchasing p " &
                         "JOIN supplier s ON p.supplier_id = s.id " &
                         "WHERE p.balance_due > 0 " &
                         "AND s.debit_period IS NOT NULL " &
                         "AND (s.debit_period <= DATE_ADD(CURDATE(), INTERVAL 10 DAY)) " &
                         "ORDER BY s.debit_period ASC"
                FillGrid(dgvSupAlerts, sql4, conn)
                
                ' 5. Customer Return Cheques
                Dim sql5 = "SELECT check_number AS 'Cheque No', check_name AS 'Customer', amount AS 'Amount', return_reason AS 'Reason', inv_no AS 'Invoice' " &
                          "FROM cheque_returned ORDER BY issue_date DESC"
                FillGrid(dgvCustChequeReturns, sql5, conn)

                ' 6. Supplier Return Cheques
                Dim sql6 = "SELECT check_number AS 'Cheque No', check_name AS 'Supplier', amount AS 'Amount', return_reason AS 'Reason', inv_no AS 'Invoice' " &
                          "FROM chaque_return_reason ORDER BY return_date DESC"
                FillGrid(dgvSupChequeReturns, sql6, conn)

            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading alerts: " & ex.Message)
        End Try
    End Sub

    Private Sub FillGrid(dgv As DataGridView, sql As String, conn As MySqlConnection)
        Dim dt As New DataTable()
        Using adapter As New MySqlDataAdapter(sql, conn)
            adapter.Fill(dt)
            dgv.DataSource = dt
        End Using
    End Sub

    Private Sub dgvSupAlerts_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles dgvSupAlerts.CellFormatting
        If dgvSupAlerts.Columns(e.ColumnIndex).Name = "Due Date" AndAlso e.Value IsNot Nothing Then
            Try
                Dim dueDate As DateTime = Convert.ToDateTime(e.Value)
                If dueDate < DateTime.Today Then
                    dgvSupAlerts.Rows(e.RowIndex).DefaultCellStyle.ForeColor = Color.Red
                    dgvSupAlerts.Rows(e.RowIndex).DefaultCellStyle.SelectionForeColor = Color.Red
                End If
            Catch
            End Try
        End If
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    ' Dragging logic for borderless form
    Private dragging As Boolean = False
    Private dragCursorPoint As Point
    Private dragFormPoint As Point

    Private Sub pnlHeader_MouseDown(sender As Object, e As MouseEventArgs) Handles pnlHeader.MouseDown
        dragging = True
        dragCursorPoint = Cursor.Position
        dragFormPoint = Me.Location
    End Sub

    Private Sub pnlHeader_MouseMove(sender As Object, e As MouseEventArgs) Handles pnlHeader.MouseMove
        If dragging Then
            Dim dif As Point = Point.Subtract(Cursor.Position, New Size(dragCursorPoint))
            Me.Location = Point.Add(dragFormPoint, New Size(dif))
        End If
    End Sub

    Private Sub pnlHeader_MouseUp(sender As Object, e As MouseEventArgs) Handles pnlHeader.MouseUp
        dragging = False
    End Sub

End Class
