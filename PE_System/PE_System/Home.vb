Imports MySql.Data.MySqlClient
Imports System.Windows.Forms.DataVisualization.Charting

Public Class Home

    Private Sub Home_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.WindowState = FormWindowState.Maximized
        LoadManagementData()
        UpdateDashboardData()
        lblUserDisplay.Text = "Login User: " & Module1.UserName
        
        ' Enable double buffering for all grids
        Module1.EnableDoubleBuffered(dgvCustomerCredits)
        Module1.EnableDoubleBuffered(dgvBlockedCustomers)
        Module1.EnableDoubleBuffered(dgvSupplierCredits)
        Module1.EnableDoubleBuffered(dgvSupplierAlerts)
        Module1.EnableDoubleBuffered(dgvCustomerReturnCheques)
        Module1.EnableDoubleBuffered(dgvSupplierReturnCheques)
    End Sub

    Private Sub LoadManagementData()
        Try
            Using localConn As New MySqlConnection(Module1.ConnStr)
                localConn.Open()

                ' 1. Customer Credits (Older than 2 Months)
                Dim dtCustCredit As New DataTable()
                Dim sqlCustCredit As String = "SELECT cc.inv_no AS 'Bill ID', c.name AS 'Customer', c.tel_no AS 'Contact', cc.amount AS 'Amount', DATE_FORMAT(cc.timestamps, '%Y-%m-%d') AS 'Bill Date' " &
                                             "FROM customer_credit cc " &
                                             "INNER JOIN customer c ON cc.customer_id = c.id " &
                                             "WHERE cc.is_active = 1 AND cc.amount > 0 AND cc.timestamps <= DATE_SUB(NOW(), INTERVAL 2 MONTH) " & (If(Module1.IsRgrVisible, "", " AND cc.is_rgr = 0 AND cc.inv_no NOT LIKE 'GR%' AND cc.inv_no NOT LIKE 'RGR%' ")) & " ORDER BY cc.timestamps ASC"
                Using adapter As New MySqlDataAdapter(sqlCustCredit, localConn)
                    adapter.Fill(dtCustCredit)
                    dgvCustomerCredits.DataSource = dtCustCredit
                End Using

                ' 2. Blocked / Over-limit Customers
                Dim dtBlocked As New DataTable()
                Dim rgrFilterB = If(Module1.IsRgrVisible, "", " AND is_rgr = 0 AND inv_no NOT LIKE 'GR%' AND inv_no NOT LIKE 'RGR%' ")
                Dim sqlBlocked As String = "SELECT c.name AS 'Customer', c.tel_no AS 'Phone', " &
                                         "CASE WHEN c.is_block = 1 THEN 'Manual Block' ELSE 'Limit/Period Alert' END AS 'Status', " &
                                         "c.credit_limit AS 'Limit', " &
                                         "COALESCE((SELECT SUM(balance_due) FROM billing WHERE customer_id = c.id AND balance_due > 0" & rgrFilterB & "), 0) AS 'Outstanding' " &
                                         "FROM customer c " &
                                         "WHERE c.is_block = 1 OR (c.credit_limit > 0 AND COALESCE((SELECT SUM(balance_due) FROM billing WHERE customer_id = c.id AND balance_due > 0" & rgrFilterB & "), 0) > c.credit_limit)"
                Using adapter As New MySqlDataAdapter(sqlBlocked, localConn)
                    adapter.Fill(dtBlocked)
                    dgvBlockedCustomers.DataSource = dtBlocked
                End Using

                ' 3. Supplier Credits (Total Outstanding)
                Dim dtSupCredit As New DataTable()
                Dim sqlSupCredit As String = "SELECT p.pur_id AS 'Pur ID', s.name AS 'Supplier', p.description AS 'Inv Ref', p.balance_due AS 'Balance', DATE_FORMAT(p.pur_date, '%Y-%m-%d') AS 'Date' " &
                                            "FROM purchasing p " &
                                            "JOIN supplier s ON p.supplier_id = s.id " &
                                            "WHERE p.balance_due > 0 ORDER BY p.pur_date ASC"
                Using adapter As New MySqlDataAdapter(sqlSupCredit, localConn)
                    adapter.Fill(dtSupCredit)
                    dgvSupplierCredits.DataSource = dtSupCredit
                End Using

                ' 4. Supplier Alerts (10 Days or Overdue)
                Dim dtSupAlert As New DataTable()
                Dim sqlSupAlert As String = "SELECT p.pur_id AS 'Pur ID', s.name AS 'Supplier', p.balance_due AS 'Amount', s.debit_period AS 'Due Date' " &
                                           "FROM purchasing p " &
                                           "JOIN supplier s ON p.supplier_id = s.id " &
                                           "WHERE p.balance_due > 0 " &
                                           "AND s.debit_period IS NOT NULL " &
                                           "AND (s.debit_period <= DATE_ADD(CURDATE(), INTERVAL 10 DAY)) " &
                                           "ORDER BY s.debit_period ASC"
                Using adapter As New MySqlDataAdapter(sqlSupAlert, localConn)
                    adapter.Fill(dtSupAlert)
                    dgvSupplierAlerts.DataSource = dtSupAlert
                End Using
                
                ' 5. Customer Return Cheques
                Dim dtCustReturn As New DataTable()
                ' Note: user said "customer name hve a s check_name" - using check_name as alias for Customer
                Dim sqlCustReturn As String = "SELECT check_number AS 'Cheque No', check_name AS 'Customer', amount AS 'Amount', return_reason AS 'Reason', inv_no AS 'Invoice' " &
                                             "FROM cheque_returned ORDER BY issue_date DESC"
                Using adapter As New MySqlDataAdapter(sqlCustReturn, localConn)
                    adapter.Fill(dtCustReturn)
                    dgvCustomerReturnCheques.DataSource = dtCustReturn
                End Using

                ' 6. Supplier Return Cheques
                Dim dtSupReturn As New DataTable()
                Dim sqlSupReturn As String = "SELECT check_number AS 'Cheque No', check_name AS 'Supplier', amount AS 'Amount', return_reason AS 'Reason', inv_no AS 'Invoice' " &
                                            "FROM chaque_return_reason ORDER BY return_date DESC"
                Using adapter As New MySqlDataAdapter(sqlSupReturn, localConn)
                    adapter.Fill(dtSupReturn)
                    dgvSupplierReturnCheques.DataSource = dtSupReturn
                End Using

            End Using
        Catch ex As Exception
            Console.WriteLine("LoadManagementData Error: " & ex.Message)
        End Try
    End Sub

    Private Sub dgvSupplierAlerts_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles dgvSupplierAlerts.CellFormatting
        If dgvSupplierAlerts.Columns(e.ColumnIndex).Name = "Due Date" AndAlso e.Value IsNot Nothing Then
            Try
                Dim dueDate As DateTime = Convert.ToDateTime(e.Value)
                If dueDate < DateTime.Today Then
                    dgvSupplierAlerts.Rows(e.RowIndex).DefaultCellStyle.BackColor = Color.FromArgb(255, 230, 230) ' Light Red
                    dgvSupplierAlerts.Rows(e.RowIndex).DefaultCellStyle.ForeColor = Color.DarkRed
                ElseIf dueDate = DateTime.Today Then
                    dgvSupplierAlerts.Rows(e.RowIndex).DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 224) ' Light Yellow
                End If
            Catch
            End Try
        End If
    End Sub

    Private Sub UpdateDashboardData()
        Try
            Using localConn As New MySqlConnection(Module1.ConnStr)
                localConn.Open()

                ' 1. Today's Sales
                Dim totalSalesValue As Double = 0
                Dim sqlSales As String = "SELECT SUM(grand_total) FROM billing WHERE DATE(timestamps) = CURDATE() AND LOWER(status) != 'cancelled' " & (If(Module1.IsRgrVisible, "", " AND is_rgr = 0 AND inv_no NOT LIKE 'GR%' AND inv_no NOT LIKE 'RGR%' "))
                Using cmd As New MySqlCommand(sqlSales, localConn)
                    Dim res = cmd.ExecuteScalar()
                    If res IsNot Nothing AndAlso Not IsDBNull(res) Then
                        totalSalesValue += Convert.ToDouble(res)
                    End If
                End Using

                Try
                    Dim sqlAdj As String = "SELECT SUM(difference_amount) FROM sales_adjustments WHERE DATE(adjustment_date) = CURDATE() " & (If(Module1.IsRgrVisible, "", " AND is_rgr = 0 AND inv_no NOT LIKE 'GR%' AND inv_no NOT LIKE 'RGR%' "))
                    Using cmdAdj As New MySqlCommand(sqlAdj, localConn)
                        Dim resAdj = cmdAdj.ExecuteScalar()
                        If resAdj IsNot Nothing AndAlso Not IsDBNull(resAdj) Then
                            totalSalesValue += Convert.ToDouble(resAdj)
                        End If
                    End Using
                Catch
                    ' Ignore if table does not exist
                End Try

                lblTodaySales.Text = "Rs. " & totalSalesValue.ToString("N2")

                ' 2. Today's Collection
                Dim totalPaidValue As Double = 0
                Dim sqlPaid As String = "SELECT SUM(paid_amount) FROM billing WHERE DATE(timestamps) = CURDATE() AND LOWER(status) != 'cancelled' " & (If(Module1.IsRgrVisible, "", " AND is_rgr = 0 AND inv_no NOT LIKE 'GR%' AND inv_no NOT LIKE 'RGR%' "))
                Using cmd As New MySqlCommand(sqlPaid, localConn)
                    Dim res = cmd.ExecuteScalar()
                    If res IsNot Nothing AndAlso Not IsDBNull(res) Then
                        totalPaidValue = Convert.ToDouble(res)
                    End If
                End Using
                lblTodayPaid.Text = "Rs. " & totalPaidValue.ToString("N2")

                ' 3. Low Stock Count
                Dim sqlStock As String = "SELECT COUNT(*) FROM items WHERE id IN (SELECT item_id FROM items_stock GROUP BY item_id HAVING SUM(st_qty) < 10)"
                Using cmd As New MySqlCommand(sqlStock, localConn)
                    Dim count = cmd.ExecuteScalar()
                    lblLowStockCount.Text = If(count IsNot Nothing, count.ToString(), "0")
                End Using

                ' 3. Chart: Sales by Category
                Dim sqlSalesCat As String = "SELECT c.name as cat, SUM(bi.unit_price * bi.quantity) as total " &
                                           "FROM billing_item bi " &
                                           "JOIN billing b ON bi.billing_id = b.id " &
                                           "JOIN items i ON bi.item_id = i.id " &
                                           "JOIN category c ON i.category_id = c.id " &
                                           "WHERE 1=1 " & (If(Module1.IsRgrVisible, "", " AND b.is_rgr = 0 AND b.inv_no NOT LIKE 'GR%' AND b.inv_no NOT LIKE 'RGR%' ")) &
                                           "GROUP BY c.name"
                chartSales.Series("Data").Points.Clear()
                Using cmd As New MySqlCommand(sqlSalesCat, localConn)
                    Using dr = cmd.ExecuteReader()
                        While dr.Read()
                            chartSales.Series("Data").Points.AddXY(dr("cat").ToString(), dr("total"))
                        End While
                    End Using
                End Using

                ' 4. Chart: Stock Distribution
                Dim sqlStockDist As String = "SELECT c.name as cat, SUM(s.st_qty) as total " &
                                            "FROM items_stock s " &
                                            "JOIN items i ON s.item_id = i.id " &
                                            "JOIN category c ON i.category_id = c.id " &
                                            "GROUP BY c.name"
                chartStock.Series("Data").Points.Clear()
                Using cmd As New MySqlCommand(sqlStockDist, localConn)
                    Using dr = cmd.ExecuteReader()
                        While dr.Read()
                            chartStock.Series("Data").Points.AddXY(dr("cat").ToString(), dr("total"))
                        End While
                    End Using
                End Using

                ' --- Chart Styling ---
                chartSales.ChartAreas(0).AxisX.Interval = 1
                chartSales.ChartAreas(0).AxisX.LabelStyle.Angle = -45
                chartSales.ChartAreas(0).AxisX.MajorGrid.LineColor = Color.LightGray
                chartSales.ChartAreas(0).AxisY.MajorGrid.LineColor = Color.LightGray

                If chartStock.Legends.Count > 0 Then
                    chartStock.Legends(0).Docking = Docking.Bottom
                    chartStock.Legends(0).Alignment = StringAlignment.Center
                    chartStock.Legends(0).BackColor = Color.Transparent
                End If
                chartStock.ChartAreas(0).Position.Auto = True
                chartStock.Series("Data")("PieLabelStyle") = "Outside"
                chartStock.Series("Data")("PieDrawingStyle") = "SoftEdge"
                chartStock.ChartAreas(0).Area3DStyle.Enable3D = True
                chartStock.ChartAreas(0).Area3DStyle.Inclination = 45

            End Using
        Catch ex As Exception
            Console.WriteLine("UpdateDashboardData Error: " & ex.Message)
        End Try
    End Sub

    Private Sub Home_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        Try
            If flowCharts IsNot Nothing AndAlso chartSales IsNot Nothing AndAlso chartStock IsNot Nothing Then
                flowCharts.Width = Me.ClientSize.Width - 40
                Dim availableWidth As Integer = flowCharts.ClientSize.Width - 50
                If availableWidth > 500 Then
                    chartSales.Width = CInt(availableWidth * 0.6)
                    chartStock.Width = availableWidth - chartSales.Width
                    Dim newChartHeight As Integer = Me.ClientSize.Height - flowCharts.Top - pnlCreditDetails.Height - 20
                    If newChartHeight > 200 Then
                        flowCharts.Height = newChartHeight
                        chartSales.Height = newChartHeight - 15
                        chartStock.Height = newChartHeight - 15
                    End If
                End If
                flowCards.Left = Me.ClientSize.Width - flowCards.Width - 20
                lblUserDisplay.Left = Me.ClientSize.Width - lblUserDisplay.Width - 20
            End If
        Catch ex As Exception
            ' Silent resize error
        End Try
    End Sub

End Class
