Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared
Imports MySql.Data.MySqlClient

Public Class DailyTransactionViewer
    Private Sub DailyTransactionViewer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        InitializeReportTypes()
        dtpFrom.Value = DateTime.Now
        dtpTo.Value = DateTime.Now
    End Sub

    Private Sub InitializeReportTypes()
        If cmbReportType.Items.Count = 0 Then
            cmbReportType.Items.Clear()
            cmbReportType.Items.Add("Daily Cash Bills (Cash Only)")
            cmbReportType.Items.Add("Daily Cash Bills (Cards/Online)")
            cmbReportType.Items.Add("Daily Credit Bills")
            cmbReportType.Items.Add("Daily GR Bills")
            cmbReportType.Items.Add("Daily EL Bills")
            cmbReportType.Items.Add("Daily VAT Bills")
            cmbReportType.Items.Add("Daily Quotations")
            cmbReportType.Items.Add("Full Day Transaction Summary (Reconciliation)")
            cmbReportType.SelectedIndex = 0
        End If
    End Sub

    Public Sub ShowReport(reportIndex As Integer)
        InitializeReportTypes()
        If reportIndex >= 0 AndAlso reportIndex < cmbReportType.Items.Count Then
            cmbReportType.SelectedIndex = reportIndex
            btnShow.PerformClick()
        End If
    End Sub

    Private Sub btnShow_Click(sender As Object, e As EventArgs) Handles btnShow.Click
        LoadReport(cmbReportType.SelectedIndex)
    End Sub

    Private Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click
        If CrystalReportViewer1.ReportSource IsNot Nothing Then
            CrystalReportViewer1.PrintReport()
        End If
    End Sub

    Private Sub LoadReport(index As Integer)
        Try
            Dim rpt As ReportDocument = GetReport(index)
            If rpt Is Nothing Then Return

            Dim selectionFormula As String = ""
            Dim fromDate As String = dtpFrom.Value.ToString("yyyy-MM-dd")
            Dim toDate As String = dtpTo.Value.ToString("yyyy-MM-dd")

            Select Case index
                Case 0, 1 ' Daily Cash Bills (Cash Only) / (Cards/Online)
                    Dim dt As New DataTable("Command")
                    Dim pTypes As String = ""
                    If index = 0 Then
                        pTypes = "'cash'"
                    Else
                        pTypes = "'credit card', 'debit card', 'online transfer'"
                    End If
                    
                    Dim query As String = "SELECT b.id, b.inv_no, b.payment_type, b.inv_type, b.advance_payment, b.billing_type, " &
                                          "b.subtotal, b.grand_total, b.timestamps, b.customer_id, b.vat_id, b.our_discount, " &
                                          "b.inv_discount, b.balance_due, b.cheque_no, b.paid_amount, b.cash_received, b.change_amount, " &
                                          "b.status, b.cash_status, b.user_id, b.order_user_id, b.collector_user_id, b.updated_at, " &
                                          "b.bank_id, b.po_number, b.cheque_balance_due, b.credit_balance_due, b.partial_cash, " &
                                          "b.change_action, b.balance_action, b.balance_amount, b.wallet_used, b.wallet_balance_after, " &
                                          "b.is_rgr, b.printed_inv_no, b.adv_pay_amount, b.cus_vat_id, b.print_as_retail, " &
                                          "c.name AS customer_name, v.vat_name, u.name AS user_name " &
                                          "FROM billing b " &
                                          "LEFT JOIN customer c ON b.customer_id = c.id " &
                                          "LEFT JOIN vat v ON b.vat_id = v.id " &
                                          "LEFT JOIN `user` u ON COALESCE(b.collector_user_id, b.user_id) = u.id " &
                                          "WHERE b.status = 'Paid' " &
                                          "  AND DATE(b.timestamps) >= @start AND DATE(b.timestamps) <= @end " &
                                          "  AND LOWER(TRIM(b.payment_type)) IN (" & pTypes & ") "

                    ' Only UNION customer payments for Daily Cards/Online report (index 1) to exclude Cash credit payments from Cash report
                    If index = 1 Then
                        query &= "UNION ALL " &
                                 "SELECT 0 AS id, cp.inv_no, " &
                                 "CASE " &
                                 "    WHEN LOWER(TRIM(cp.PaymentType)) = 'online payment' THEN 'Online Transfer' " &
                                 "    WHEN LOWER(TRIM(cp.PaymentType)) = 'cash' THEN 'Cash' " &
                                 "    WHEN LOWER(TRIM(cp.PaymentType)) = 'cheque' THEN 'Cheque' " &
                                 "    ELSE cp.PaymentType " &
                                 "END AS payment_type, " &
                                 "COALESCE(b.inv_type, 'Normal') AS inv_type, 0.00 AS advance_payment, 'Payment' AS billing_type, " &
                                 "cp.Amount AS subtotal, cp.Amount AS grand_total, cp.Date AS timestamps, cp.CusID AS customer_id, " &
                                 "COALESCE(b.vat_id, 1) AS vat_id, 0.00 AS our_discount, 0.00 AS inv_discount, 0.00 AS balance_due, " &
                                 "cp.cheque_no, cp.Amount AS paid_amount, cp.Amount AS cash_received, 0.00 AS change_amount, " &
                                 "'Paid' AS status, '' AS cash_status, COALESCE(b.user_id, 101) AS user_id, 0 AS order_user_id, " &
                                 "0 AS collector_user_id, cp.Date AS updated_at, cp.bank_id, '' AS po_number, " &
                                 "0.00 AS cheque_balance_due, 0.00 AS credit_balance_due, 0.00 AS partial_cash, " &
                                 "'' AS change_action, '' AS balance_action, 0.00 AS balance_amount, 0.00 AS wallet_used, " &
                                 "0.00 AS wallet_balance_after, cp.is_rgr, cp.inv_no AS printed_inv_no, 0.00 AS adv_pay_amount, " &
                                 "COALESCE(b.cus_vat_id, '') AS cus_vat_id, 0 AS print_as_retail, cp.Customer AS customer_name, " &
                                 "COALESCE(v.vat_name, 'NO VAT') AS vat_name, COALESCE(u.name, 'Admin') AS user_name " &
                                 "FROM customer_payments cp " &
                                 "LEFT JOIN billing b ON cp.inv_no = b.inv_no " &
                                 "LEFT JOIN vat v ON b.vat_id = v.id " &
                                 "LEFT JOIN `user` u ON COALESCE(b.collector_user_id, b.user_id) = u.id " &
                                 "WHERE DATE(cp.Date) >= @start AND DATE(cp.Date) <= @end " &
                                 "  AND LOWER(TRIM(" &
                                 "      CASE " &
                                 "          WHEN LOWER(TRIM(cp.PaymentType)) = 'online payment' THEN 'Online Transfer' " &
                                 "          WHEN LOWER(TRIM(cp.PaymentType)) = 'cash' THEN 'Cash' " &
                                 "          WHEN LOWER(TRIM(cp.PaymentType)) = 'cheque' THEN 'Cheque' " &
                                 "          ELSE cp.PaymentType " &
                                 "      END" &
                                 "  )) IN (" & pTypes & ")"
                    End If

                    Using conn As New MySqlConnection(Module1.ConnStr)
                        Using cmd As New MySqlCommand(query, conn)
                            cmd.Parameters.AddWithValue("@start", fromDate)
                            cmd.Parameters.AddWithValue("@end", toDate)
                            Using da As New MySqlDataAdapter(cmd)
                                da.Fill(dt)
                            End Using
                        End Using
                    End Using
                    
                    rpt.SetDataSource(dt)
                    selectionFormula = ""

                Case 2 ' Credit Bills
                    selectionFormula = "{Command.status} IN ['Credit','cash_Credit','Mixed_Payment','Credit_Cheque'] AND DATE({Command.timestamps}) >= Date(" & dtpFrom.Value.Year & "," & dtpFrom.Value.Month & "," & dtpFrom.Value.Day & ") AND DATE({Command.timestamps}) <= Date(" & dtpTo.Value.Year & "," & dtpTo.Value.Month & "," & dtpTo.Value.Day & ")"
                Case 3 ' GR Bills
                    selectionFormula = "{Command.inv_no} LIKE 'GR*' AND DATE({Command.timestamps}) >= Date(" & dtpFrom.Value.Year & "," & dtpFrom.Value.Month & "," & dtpFrom.Value.Day & ") AND DATE({Command.timestamps}) <= Date(" & dtpTo.Value.Year & "," & dtpTo.Value.Month & "," & dtpTo.Value.Day & ")"
                Case 4 ' EL Bills
                    selectionFormula = "{Command.inv_no} LIKE 'EL*' AND DATE({Command.timestamps}) >= Date(" & dtpFrom.Value.Year & "," & dtpFrom.Value.Month & "," & dtpFrom.Value.Day & ") AND DATE({Command.timestamps}) <= Date(" & dtpTo.Value.Year & "," & dtpTo.Value.Month & "," & dtpTo.Value.Day & ")"
                Case 5 ' VAT Bills
                    selectionFormula = "{Command.inv_no} LIKE 'VT*' AND DATE({Command.timestamps}) >= Date(" & dtpFrom.Value.Year & "," & dtpFrom.Value.Month & "," & dtpFrom.Value.Day & ") AND DATE({Command.timestamps}) <= Date(" & dtpTo.Value.Year & "," & dtpTo.Value.Month & "," & dtpTo.Value.Day & ")"
                Case 6 ' Quotations
                    selectionFormula = "DATE({Command.timestamps}) >= Date(" & dtpFrom.Value.Year & "," & dtpFrom.Value.Month & "," & dtpFrom.Value.Day & ") AND DATE({Command.timestamps}) <= Date(" & dtpTo.Value.Year & "," & dtpTo.Value.Month & "," & dtpTo.Value.Day & ")"
                Case 7 ' Full Day Summary
                    UpdateCashSalesCustomerNames(fromDate, toDate)
                    selectionFormula = "DATE({Command.date}) >= Date(" & dtpFrom.Value.Year & "," & dtpFrom.Value.Month & "," & dtpFrom.Value.Day & ") AND DATE({Command.date}) <= Date(" & dtpTo.Value.Year & "," & dtpTo.Value.Month & "," & dtpTo.Value.Day & ")"
                Case Else
                    Return
            End Select

            SetReportConnection(rpt)
            rpt.RecordSelectionFormula = selectionFormula
            CrystalReportViewer1.ReportSource = rpt
            CrystalReportViewer1.RefreshReport()
        Catch ex As Exception
            MessageBox.Show("Error loading report: " & ex.Message)
        End Try
    End Sub

    Private Function GetReport(index As Integer) As ReportDocument
        Select Case index
            Case 0, 1
                Return New CashRpeort()
            Case 2
                Return New CreditReport()
            Case 3
                Return New GRReport()
            Case 4
                Return New ELReport()
            Case 5
                Return New VATReport()
            Case 6
                Return New QTReport()
            Case 7
                Return New FullDayAll()
            Case Else
                Return Nothing
        End Select
    End Function

    Private Sub SetReportConnection(ByRef rpt As ReportDocument)
        Try
            ' Parse connection string (Format: server=...;userid=...;password=...;database=...)
            Dim connStr = Module1.ConnStr
            Dim parts = connStr.Split(";"c)
            Dim server = "", db = "", user = "", pass = ""

            For Each part In parts
                Dim kv = part.Split("="c)
                If kv.Length >= 2 Then
                    Dim key = kv(0).Trim().ToLower()
                    ' Rejoin value in case password contains '='
                    Dim val = String.Join("=", kv, 1, kv.Length - 1).Trim().Replace(vbCr, "").Replace(vbLf, "")
                    
                    If key = "server" OrElse key = "data source" OrElse key = "host" Then server = val
                    If key = "database" OrElse key = "initial catalog" Then db = val
                    If key = "userid" OrElse key = "user id" OrElse key = "uid" Then user = val
                    If key = "password" OrElse key = "pwd" Then pass = val
                End If
            Next

            ' Apply to main report tables
            For Each tbl As Table In rpt.Database.Tables
                Dim tblLogOn = tbl.LogOnInfo
                ' We DO NOT overwrite ServerName or DatabaseName to preserve ODBC DSNs
                tblLogOn.ConnectionInfo.UserID = user
                tblLogOn.ConnectionInfo.Password = pass
                tblLogOn.ConnectionInfo.IntegratedSecurity = False
                tbl.ApplyLogOnInfo(tblLogOn)
            Next

            ' Apply to subreports
            For Each subRpt As ReportDocument In rpt.Subreports
                For Each tbl As Table In subRpt.Database.Tables
                    Dim tblLogOn = tbl.LogOnInfo
                    tblLogOn.ConnectionInfo.UserID = user
                    tblLogOn.ConnectionInfo.Password = pass
                    tblLogOn.ConnectionInfo.IntegratedSecurity = False
                    tbl.ApplyLogOnInfo(tblLogOn)
                Next
            Next

            ' Force top-level logon as additional measure
            If rpt.DataSourceConnections.Count > 0 Then
                rpt.SetDatabaseLogon(user, pass, rpt.DataSourceConnections(0).ServerName, rpt.DataSourceConnections(0).DatabaseName)
            End If

            ' Format report to 3 decimal places
            Module1.FormatReportDecimals(rpt)

        Catch ex As Exception
            ' Silent fail, fallback to stored connection
        End Try
    End Sub

    Private Sub UpdateCashSalesCustomerNames(fromDate As String, toDate As String)
        Try
            Using conn As New MySqlConnection(Module1.ConnStr)
                conn.Open()
                
                Dim sqlUpdate As String = 
                    "UPDATE petty_cash pc " &
                    "JOIN billing b ON pc.receipt_no = b.inv_no " &
                    "JOIN customer c ON b.customer_id = c.id " &
                    "SET pc.item_name = CASE " &
                    "  WHEN pc.item_name = CONCAT('Cash Sale: ', b.inv_no) THEN CONCAT('Cash Sale: ', c.name, ' (Inv: ', b.inv_no, ')') " &
                    "  WHEN pc.item_name = CONCAT('Cash Refund: ', b.inv_no) THEN CONCAT('Cash Refund: ', c.name, ' (Inv: ', b.inv_no, ')') " &
                    "  WHEN pc.item_name = CONCAT('Cash Sale Update (Inv: ', b.inv_no, ')') THEN CONCAT('Cash Sale Update: ', c.name, ' (Inv: ', b.inv_no, ')') " &
                    "  WHEN pc.item_name = CONCAT('Cash Refund (Inv: ', b.inv_no, ')') THEN CONCAT('Cash Refund: ', c.name, ' (Inv: ', b.inv_no, ')') " &
                    "  WHEN pc.item_name = CONCAT('Change Given: ', b.inv_no) THEN CONCAT('Change Given: ', c.name, ' (Inv: ', b.inv_no, ')') " &
                    "  WHEN pc.item_name = CONCAT('Cash_Credit Sale: ', b.inv_no) THEN CONCAT('Cash_Credit Sale: ', c.name, ' (Inv: ', b.inv_no, ')') " &
                    "  WHEN pc.item_name = CONCAT('Cash_Credit Update (Inv: ', b.inv_no, ')') THEN CONCAT('Cash_Credit Update: ', c.name, ' (Inv: ', b.inv_no, ')') " &
                    "  WHEN pc.item_name = CONCAT('Cash_Credit Refund (Inv: ', b.inv_no, ')') THEN CONCAT('Cash_Credit Refund: ', c.name, ' (Inv: ', b.inv_no, ')') " &
                    "  WHEN pc.item_name = CONCAT('Cash_Cheque Sale: ', b.inv_no) THEN CONCAT('Cash_Cheque Sale: ', c.name, ' (Inv: ', b.inv_no, ')') " &
                    "  WHEN pc.item_name = CONCAT('Cash_Cheque Update (Inv: ', b.inv_no, ')') THEN CONCAT('Cash_Cheque Update: ', c.name, ' (Inv: ', b.inv_no, ')') " &
                    "  WHEN pc.item_name = CONCAT('Cash_Cheque Refund (Inv: ', b.inv_no, ')') THEN CONCAT('Cash_Cheque Refund: ', c.name, ' (Inv: ', b.inv_no, ')') " &
                    "  WHEN pc.item_name = CONCAT('Mixed_Payment Sale: ', b.inv_no) THEN CONCAT('Mixed_Payment Sale: ', c.name, ' (Inv: ', b.inv_no, ')') " &
                    "  WHEN pc.item_name = CONCAT('Mixed_Payment Update (Inv: ', b.inv_no, ')') THEN CONCAT('Mixed_Payment Update: ', c.name, ' (Inv: ', b.inv_no, ')') " &
                    "  WHEN pc.item_name = CONCAT('Mixed_Payment Refund (Inv: ', b.inv_no, ')') THEN CONCAT('Mixed_Payment Refund: ', c.name, ' (Inv: ', b.inv_no, ')') " &
                    "  ELSE pc.item_name " &
                    "END " &
                    "WHERE DATE(pc.date) >= @from AND DATE(pc.date) <= @to " &
                    "  AND LOWER(TRIM(c.name)) NOT IN ('cash', 'no customer', 'cash customer', 'default') " &
                    "  AND c.name IS NOT NULL AND TRIM(c.name) != ''"
                    
                Using cmd As New MySqlCommand(sqlUpdate, conn)
                    cmd.Parameters.AddWithValue("@from", fromDate)
                    cmd.Parameters.AddWithValue("@to", toDate)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("UpdateCashSalesCustomerNames error: " & ex.Message)
        End Try
    End Sub
End Class
