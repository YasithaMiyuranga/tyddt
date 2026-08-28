Imports MySql.Data.MySqlClient

Partial Public Class DailyReconciliation
    Inherits System.Windows.Forms.Form

    Private Sub DailyReconciliation_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadData()
    End Sub

    Private Sub dtpDate_ValueChanged(sender As Object, e As EventArgs) Handles dtpDate.ValueChanged
        LoadData()
    End Sub

    Private Sub LoadData()
        Dim selectedDate As String = dtpDate.Value.ToString("yyyy-MM-dd")
        LoadSummary(selectedDate)
        LoadTransactions(selectedDate)
    End Sub

    Private Sub LoadSummary(selectedDate As String)
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()

            ' 1. Total Cash In
            ' (Sales + Customer Payments + Purchase Refunds + Petty Cash In)
            ' Use partial_cash from all collected bills (accounts for initial cash portions of mixed payments)
            Dim sqlSales = "SELECT SUM(partial_cash) FROM billing WHERE DATE(timestamps) = @date AND cash_status = 'COLLECTED'" & (If(Module1.IsRgrVisible, "", " AND (is_rgr = 0 OR inv_no LIKE 'EL%' OR inv_no LIKE 'VT%') AND inv_no NOT LIKE 'GR%' AND inv_no NOT LIKE 'RGR%' "))
            Dim sqlCusPay = "SELECT SUM(Amount) FROM customer_payments WHERE DATE(Date) = @date"
            Dim sqlPurRefund = "SELECT SUM(amount) FROM petty_cash WHERE DATE(date) = @date AND item_type = 'SYSTEM' AND item_name LIKE 'Supplier Return Refund%'"
            Dim sqlPettyIn = "SELECT SUM(amount) FROM petty_cash WHERE DATE(date) = @date AND transaction_type = 'IN' AND item_type != 'SYSTEM'"

            Dim salesAmt As Decimal = 0
            Dim cusPayAmt As Decimal = 0
            Dim purRefAmt As Decimal = 0
            Dim pettyInAmt As Decimal = 0

            Using cmd = New MySqlCommand(sqlSales, conn)
                cmd.Parameters.AddWithValue("@date", selectedDate)
                Dim res = cmd.ExecuteScalar()
                If res IsNot DBNull.Value AndAlso res IsNot Nothing Then salesAmt = Convert.ToDecimal(res)
            End Using
            Using cmd = New MySqlCommand(sqlCusPay, conn)
                cmd.Parameters.AddWithValue("@date", selectedDate)
                Dim res = cmd.ExecuteScalar()
                If res IsNot DBNull.Value AndAlso res IsNot Nothing Then cusPayAmt = Convert.ToDecimal(res)
            End Using
            Using cmd = New MySqlCommand(sqlPurRefund, conn)
                cmd.Parameters.AddWithValue("@date", selectedDate)
                Dim res = cmd.ExecuteScalar()
                If res IsNot DBNull.Value AndAlso res IsNot Nothing Then purRefAmt = Convert.ToDecimal(res)
            End Using
            Using cmd = New MySqlCommand(sqlPettyIn, conn)
                cmd.Parameters.AddWithValue("@date", selectedDate)
                Dim res = cmd.ExecuteScalar()
                If res IsNot DBNull.Value AndAlso res IsNot Nothing Then pettyInAmt = Convert.ToDecimal(res)
            End Using

            ' 2. Total Cash Out
            ' (Sales Returns + Cash Purchases + Supplier Payments + Petty Cash Out)
            Dim sqlRtn = "SELECT SUM(refund_amount) FROM sales_return WHERE DATE(return_date) = @date AND cash_type = 'Cash'"
            Dim sqlPur = "SELECT SUM(paid_amount) FROM purchasing WHERE DATE(pur_date) = @date AND p_method = 'Cash'"
            Dim sqlSupPay = "SELECT SUM(amount) FROM supplier_payments WHERE DATE(pdate) = @date AND type = 'Cash'"
            Dim sqlPetty = "SELECT SUM(amount) FROM petty_cash WHERE DATE(date) = @date AND transaction_type = 'OUT' AND item_type != 'SYSTEM'"

            Dim rtnAmt As Decimal = 0
            Dim purAmt As Decimal = 0
            Dim supPayAmt As Decimal = 0
            Dim pettyAmt As Decimal = 0

            Using cmd = New MySqlCommand(sqlRtn, conn)
                cmd.Parameters.AddWithValue("@date", selectedDate)
                Dim res = cmd.ExecuteScalar()
                If res IsNot DBNull.Value AndAlso res IsNot Nothing Then rtnAmt = Convert.ToDecimal(res)
            End Using
            Using cmd = New MySqlCommand(sqlPur, conn)
                cmd.Parameters.AddWithValue("@date", selectedDate)
                Dim res = cmd.ExecuteScalar()
                If res IsNot DBNull.Value AndAlso res IsNot Nothing Then purAmt = Convert.ToDecimal(res)
            End Using
            Using cmd = New MySqlCommand(sqlSupPay, conn)
                cmd.Parameters.AddWithValue("@date", selectedDate)
                Dim res = cmd.ExecuteScalar()
                If res IsNot DBNull.Value AndAlso res IsNot Nothing Then supPayAmt = Convert.ToDecimal(res)
            End Using
            Using cmd = New MySqlCommand(sqlPetty, conn)
                cmd.Parameters.AddWithValue("@date", selectedDate)
                Dim res = cmd.ExecuteScalar()
                If res IsNot DBNull.Value AndAlso res IsNot Nothing Then pettyAmt = Convert.ToDecimal(res)
            End Using

            Dim totalCashIn = salesAmt + cusPayAmt
            Dim totalCashOut = rtnAmt + purAmt + supPayAmt + pettyAmt

            lblCashAmt.Text = totalCashIn.ToString("N2")

            ' 3. Total Cheques (Unrealized)
            ' Sum the cheque-specific balance from all bills
            Dim sqlCheque = "SELECT SUM(cheque_balance_due) FROM billing WHERE DATE(timestamps) = @date" & (If(Module1.IsRgrVisible, "", " AND (is_rgr = 0 OR inv_no LIKE 'EL%' OR inv_no LIKE 'VT%') AND inv_no NOT LIKE 'GR%' AND inv_no NOT LIKE 'RGR%' "))
            Using cmd = New MySqlCommand(sqlCheque, conn)
                cmd.Parameters.AddWithValue("@date", selectedDate)
                Dim val = cmd.ExecuteScalar()
                lblChequeAmt.Text = If(val Is DBNull.Value OrElse val Is Nothing, "0.00", Convert.ToDecimal(val).ToString("N2"))
            End Using

            ' 4. Total Credit Sales
            ' Sum the credit-specific balance from all bills
            Dim sqlCredit = "SELECT SUM(credit_balance_due) FROM billing WHERE DATE(timestamps) = @date" & (If(Module1.IsRgrVisible, "", " AND (is_rgr = 0 OR inv_no LIKE 'EL%' OR inv_no LIKE 'VT%') AND inv_no NOT LIKE 'GR%' AND inv_no NOT LIKE 'RGR%' "))
            Using cmd = New MySqlCommand(sqlCredit, conn)
                cmd.Parameters.AddWithValue("@date", selectedDate)
                Dim val = cmd.ExecuteScalar()
                lblCreditAmt.Text = If(val Is DBNull.Value OrElse val Is Nothing, "0.00", Convert.ToDecimal(val).ToString("N2"))
            End Using

            conn.Close()
        Catch ex As Exception
            MessageBox.Show("Error loading summary: " & ex.Message)
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub LoadTransactions(selectedDate As String)
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()

            ' Unified view of Transactions
            Dim sql = "SELECT 'SALE' as Type, inv_no as 'Doc No', billing_type as 'Mode', payment_type as 'Method', partial_cash as 'Amount' FROM billing " &
                      "WHERE DATE(timestamps) = @date AND partial_cash > 0 " & (If(Module1.IsRgrVisible, "", " AND (is_rgr = 0 OR inv_no LIKE 'EL%' OR inv_no LIKE 'VT%') AND inv_no NOT LIKE 'GR%' AND inv_no NOT LIKE 'RGR%' ")) &
                      "UNION ALL " &
                      "SELECT 'S-RTN' as Type, inv_no as 'Doc No', '-' as 'Mode', cash_type as 'Method', (refund_amount * -1) as 'Amount' FROM sales_return " &
                      "WHERE DATE(return_date) = @date AND cash_type = 'Cash' " &
                      "UNION ALL " &
                      "SELECT 'CUS-PAY' as Type, inv_no as 'Doc No', 'Credit' as 'Mode', PaymentType as 'Method', Amount FROM customer_payments " &
                      "WHERE DATE(Date) = @date " &
                      "UNION ALL " &
                      "SELECT 'PURCH' as Type, pur_id as 'Doc No', pur_type as 'Mode', p_method as 'Method', (paid_amount * -1) as 'Amount' FROM purchasing " &
                      "WHERE DATE(pur_date) = @date AND paid_amount > 0 AND p_method = 'Cash' " &
                      "UNION ALL " &
                      "SELECT 'P-RTN' as Type, item_name as 'Doc No', item_type as 'Mode', 'Cash' as 'Method', amount as 'Amount' FROM petty_cash " &
                      "WHERE DATE(date) = @date AND item_type = 'SYSTEM' AND item_name LIKE 'Supplier Return Refund%'" &
                      "UNION ALL " &
                      "SELECT 'SUP-PAY' as Type, inv_no as 'Doc No', 'Debit' as 'Mode', type as 'Method', (amount * -1) as 'Amount' FROM supplier_payments " &
                      "WHERE DATE(pdate) = @date AND type = 'Cash' " &
                      "UNION ALL " &
                      "SELECT 'PETTY' as Type, item_name as 'Doc No', item_type as 'Mode', 'Cash' as 'Method', (CASE WHEN transaction_type = 'IN' THEN amount ELSE amount * -1 END) FROM petty_cash " &
                      "WHERE DATE(date) = @date AND item_type != 'SYSTEM' " &
                      "ORDER BY Type"

            Dim da As New MySqlDataAdapter(sql, conn)
            da.SelectCommand.Parameters.AddWithValue("@date", selectedDate)
            Dim dt As New DataTable()
            da.Fill(dt)
            dgvTransactions.DataSource = dt
            
            conn.Close()
        Catch ex As Exception
            MessageBox.Show("Error loading transactions: " & ex.Message)
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub
End Class
