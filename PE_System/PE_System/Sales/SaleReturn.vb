Imports MySql.Data.MySqlClient

Public Class SaleReturn
    Dim COMMAND As MySqlCommand
    Dim READER As MySqlDataReader
    Dim oldqty As Integer
    Dim Invno As Integer
    Dim TypeCom As String

    Dim IT_Code As String
    Dim Stockt As Integer

    Dim Cost As Double
    Dim RefName As String
    Dim Inv_No As Integer
    Dim selling_date As String
    Dim sql As String
    Dim cmd As MySqlCommand
    Dim SelectedCustomerID As Integer = 0

    Private Sub SaleReturn_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.KeyPreview = True
        DateTimePicker1.Value = DateTime.Now
        LiveTimer.Start()
        LoadCashiers()
        ' Formatting DataGridView to match TempSales look
        DataGridView2.DefaultCellStyle.Font = New Font("Microsoft Sans Serif", 10)
        DataGridView2.ColumnHeadersDefaultCellStyle.Font = New Font("Microsoft Sans Serif", 12, FontStyle.Bold)
        DataGridView2.EnableHeadersVisualStyles = False
        DataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black
        DataGridView2.AllowUserToAddRows = False

        ComboBox5.Items.Add("Cash")
        ComboBox5.Items.Add("Card")
        ComboBox5.Items.Add("Cheque")
        ComboBox5.SelectedIndex = 0 ' Default to Cash

        ApplyRoleBasedUI()
    End Sub

    Private Sub ApplyRoleBasedUI()
        If Module1.FinancialRole.ToLower() = "order taker" Then
            ' Order Takers cannot give cash refunds
            return_amt.Visible = False
            return_cash.Visible = False
            ComboBox5.Enabled = False ' Lock Payment Method
            ' Maybe set a pending state indicator
        End If
    End Sub

    Private Sub LiveTimer_Tick(sender As Object, e As EventArgs) Handles LiveTimer.Tick
        lblLiveTimeDisplay.Text = DateTime.Now.ToString("hh:mm:ss tt")
    End Sub
    Private Sub sale_return(invoiceNum As String)
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()

            Dim table As New DataTable()
            ' JOIN query to get invoice data, item data, and customer name
            ' Including b.id (BID) and bi.id (BIID) for unique identification
            sql = "SELECT bi.item_id AS 'Item ID', bi.id AS 'BIID', b.id AS 'BID', bi.description AS 'Description', bi.quantity AS 'Qty', " &
              "bi.unit_price AS 'Price', bi.discount AS 'Dis', (bi.unit_price * bi.quantity - (bi.unit_price * bi.quantity * bi.discount / 100)) AS 'Item_Amount', " &
              "i.avg_cost AS 'Cost', " &
              "b.subtotal AS 'Inv_Sub', b.grand_total AS 'Inv_Grand', b.timestamps AS 'Selling_Date', b.inv_no AS 'Inv_No', " &
              "b.customer_id AS 'Customer_ID', c.name AS 'Customer_Name', b.billing_type AS 'SType' " &
              "FROM billing b " &
              "JOIN billing_item bi ON b.id = bi.billing_id " &
              "JOIN items i ON bi.item_id = i.id " &
              "LEFT JOIN customer c ON b.customer_id = c.id " &
              "WHERE b.inv_no = @inv"

            cmd = New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@inv", invoiceNum)

            Dim adapter As New MySqlDataAdapter(cmd)
            adapter.Fill(table)
            DataGridView2.DataSource = table

            ' Formatting the Grid
            If DataGridView2.Columns.Count > 0 Then
                DataGridView2.Columns("Item ID").Width = 100
                DataGridView2.Columns("Description").Width = 350
                DataGridView2.Columns("Qty").Width = 80

                ' Hide database-only columns from the user
                Dim hiddenCols() As String = {"Inv_Sub", "Inv_Grand", "Selling_Date", "Inv_No", "Customer_ID", "Customer_Name", "SType", "BID", "BIID", "Cost"}
                For Each colName In hiddenCols
                    If DataGridView2.Columns.Contains(colName) Then DataGridView2.Columns(colName).Visible = False
                Next
                If DataGridView2.Columns.Contains("Item_Amount") Then DataGridView2.Columns("Item_Amount").HeaderText = "Amount"
            End If

            ' Add Return tracking columns to the DataTable
            If Not table.Columns.Contains("Return Qty") Then
                table.Columns.Add("Return Qty", GetType(Decimal)).DefaultValue = 0
                table.Columns.Add("Return Amount", GetType(Decimal)).DefaultValue = 0
            End If

            DataGridView2.Columns("Return Qty").ReadOnly = False
            DataGridView2.Columns("Return Amount").ReadOnly = True

            ' Fill UI labels with invoice header data
            If table.Rows.Count > 0 Then
                Dim firstRow = table.Rows(0)
                tran_cus.Text = If(firstRow("Customer_Name") Is DBNull.Value, firstRow("Customer_ID").ToString(), firstRow("Customer_Name").ToString())

                ' Robustly extract currency values from the billing table
                Dim subT As Decimal = 0
                Dim grandT As Decimal = 0
                Decimal.TryParse(If(firstRow("Inv_Sub"), "0").ToString(), subT)
                Decimal.TryParse(If(firstRow("Inv_Grand"), "0").ToString(), grandT)

                Dim discAmt = subT - grandT
                Dim discPerc As Decimal = 0
                If subT > 0 Then
                    discPerc = (discAmt / subT) * 100
                End If

                totalamount.Text = subT.ToString("F2")
                invdis.Text = discAmt.ToString("F2")
                grandtotal.Text = grandT.ToString("F2")
                discount.Text = discPerc.ToString("F1") ' Display as percentage with 1 decimal

                ' Fix visibility issues (Yellow on Yellow inheritance)
                totalamount.ForeColor = Color.Black
                invdis.ForeColor = Color.Black
                grandtotal.ForeColor = Color.Black
                discount.ForeColor = Color.Black

                ' Store values for processing
                TypeCom = firstRow("SType").ToString()
                selling_date = Convert.ToDateTime(firstRow("Selling_Date")).ToString("yyyy-MM-dd HH:mm:ss")

                ' Fetch and display Customer Debit Balance
                SelectedCustomerID = Convert.ToInt32(firstRow("Customer_ID"))
                FetchCustomerBalance(SelectedCustomerID.ToString())
            End If

            conn.Close()
            calNetAmount()
        Catch ex As Exception
            MessageBox.Show("Error loading return details: " & ex.Message)
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub calNetAmount()
        Dim summ As Double = 0
        For i As Integer = 0 To DataGridView2.Rows.Count - 1
            Dim val As Double = 0
            If DataGridView2.Rows(i).Cells("Item_Amount").Value IsNot Nothing Then
                Double.TryParse(DataGridView2.Rows(i).Cells("Item_Amount").Value.ToString(), val)
                summ += val
            End If
        Next
        totalamount.Text = summ.ToString("F2")
    End Sub

    Private Sub trans_invno_KeyDown(sender As Object, e As KeyEventArgs) Handles trans_invno.KeyDown
        If e.KeyCode = Keys.Enter Then
            If Not String.IsNullOrEmpty(trans_invno.Text.Trim()) Then
                sale_return(trans_invno.Text.Trim())
            End If
        End If
    End Sub

    Private Sub DataGridView2_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView2.CellClick
        If e.RowIndex >= 0 Then
            Dim row As DataGridViewRow = DataGridView2.Rows(e.RowIndex)
            If row.Cells("Item ID").Value IsNot Nothing Then
                trans_itcode.Text = row.Cells("Item ID").Value.ToString()
                trans_des.Text = row.Cells("Description").Value.ToString()
                trans_qyt.Text = row.Cells("Qty").Value.ToString()
                trans_unit.Text = row.Cells("Price").Value.ToString()
                trans_dis.Text = If(row.Cells("Dis").Value IsNot Nothing, row.Cells("Dis").Value.ToString(), "0")
                trans_amount.Text = If(row.Cells("Item_Amount").Value IsNot Nothing, row.Cells("Item_Amount").Value.ToString(), "0")
                Dim tempQty As Integer = 0
                Integer.TryParse(If(row.Cells("Qty").Value, "0").ToString(), tempQty)
                oldqty = tempQty

                ' Fetch and display remaining stock from items table
                Try
                    If conn.State = ConnectionState.Closed Then conn.Open()
                    sql = "SELECT st_qty FROM items WHERE id = @id"
                    Using cmdStock = New MySqlCommand(sql, conn)
                        cmdStock.Parameters.AddWithValue("@id", trans_itcode.Text)
                        Dim stockRes = cmdStock.ExecuteScalar()
                        If stockRes IsNot DBNull.Value AndAlso stockRes IsNot Nothing Then
                            remain_stock.Text = stockRes.ToString()
                        Else
                            remain_stock.Text = "0"
                        End If
                    End Using
                Catch ex As Exception
                    remain_stock.Text = "0"
                Finally
                    If conn.State = ConnectionState.Open Then conn.Close()
                End Try

                ' For editing return qty
                If DataGridView2.Columns.Contains("Return Qty") Then
                    DataGridView2.CurrentCell = row.Cells("Return Qty")
                    DataGridView2.BeginEdit(True)
                End If
            End If
        End If
    End Sub

    Private Sub DataGridView2_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView2.CellValueChanged
        If e.RowIndex >= 0 AndAlso DataGridView2.Columns(e.ColumnIndex).Name = "Return Qty" Then
            Dim row = DataGridView2.Rows(e.RowIndex)
            Dim retQty As Decimal = 0
            Dim soldQty As Decimal = 0
            Decimal.TryParse(If(row.Cells("Qty").Value, "0").ToString(), soldQty)

            If row.Cells("Return Qty").Value IsNot Nothing AndAlso Decimal.TryParse(row.Cells("Return Qty").Value.ToString(), retQty) Then
                If retQty > soldQty Then
                    MessageBox.Show("Return quantity cannot exceed sold quantity.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    row.Cells("Return Qty").Value = 0
                Else
                    Dim price As Decimal = 0
                    Dim discPercent As Decimal = 0
                    Decimal.TryParse(If(row.Cells("Price").Value, "0").ToString(), price)
                    Decimal.TryParse(If(row.Cells("Dis").Value, "0").ToString(), discPercent)

                    Dim netPrice = price - (price * discPercent / 100)
                    row.Cells("Return Amount").Value = netPrice * retQty
                End If
            End If

            ' Update return_amt label
            Dim totalRtn As Decimal = 0
            For Each r As DataGridViewRow In DataGridView2.Rows
                Dim rtnVal As Decimal = 0
                If r.Cells("Return Amount").Value IsNot Nothing Then
                    Decimal.TryParse(r.Cells("Return Amount").Value.ToString(), rtnVal)
                    totalRtn += rtnVal
                End If
            Next
            return_amt.Text = totalRtn.ToString("F2")
        End If
    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        ' Check if Enter is pressed while in the "Return Qty" column
        If keyData = Keys.Enter Then
            If DataGridView2.CurrentCell IsNot Nothing AndAlso DataGridView2.CurrentCell.OwningColumn.Name = "Return Qty" Then
                If DataGridView2.IsCurrentCellInEditMode Then
                    DataGridView2.EndEdit()
                End If
                btnSaveReturn.PerformClick()
                Return True ' Consume the key to prevent default behavior
            End If
        End If
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Private Sub btnSaveReturn_Click(sender As Object, e As EventArgs) Handles btnSaveReturn.Click
        If String.IsNullOrEmpty(trans_invno.Text) Then Return

        ' Cashier Validation
        If cmbCashier.SelectedIndex = -1 Then
            MessageBox.Show("Please select a cashier.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            cmbCashier.Focus()
            Return
        End If

        If String.IsNullOrEmpty(txtCashierID.Text.Trim()) Then
            MessageBox.Show("Please enter cashier ID.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtCashierID.Focus()
            Return
        End If

        ' Verify password/ID
        Dim isLoginSuccess As Boolean = False
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()
            sql = "SELECT password FROM user WHERE id = @id AND (status IS NULL OR status = 'active')"
            Using cmdCheck = New MySqlCommand(sql, conn)
                cmdCheck.Parameters.AddWithValue("@id", cmbCashier.SelectedValue)
                Dim dbPass = cmdCheck.ExecuteScalar()
                If dbPass IsNot DBNull.Value AndAlso dbPass.ToString() = txtCashierID.Text.Trim() Then
                    isLoginSuccess = True
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error validating cashier: " & ex.Message)
            If conn.State = ConnectionState.Open Then conn.Close()
            Return
        End Try

        If Not isLoginSuccess Then
            MessageBox.Show("Invalid cashier ID.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Error)
            txtCashierID.Focus()
            If conn.State = ConnectionState.Open Then conn.Close()
            Return
        End If

        Try
            If conn.State = ConnectionState.Closed Then conn.Open()

            ' 1. Role-based Status Logic
            Dim currentCashStatus As String = "COLLECTED"
            Dim orderUID As Object = Module1.CurrentUserID
            Dim collectorUID As Object = Module1.CurrentUserID
            
            If Module1.FinancialRole.ToLower() = "order taker" Then
                currentCashStatus = "PENDING"
                collectorUID = DBNull.Value
            End If

            ' 2. Insert into sales_return (The Header)
            Dim returnID As Integer
            sql = "INSERT INTO sales_return (inv_no, customer_id, return_date, subtotal, discount, grand_total, refund_amount, cash_type, user_id, total_return_profit, cash_status, order_user_id, collector_user_id) " &
              "VALUES (@inv, @cus, @now_local, @sub, @dis, @grand, @refund, @ctype, @uid, 0, @c_status, @o_uid, @coll_uid); SELECT LAST_INSERT_ID();"

            Using cmdHead = New MySqlCommand(sql, conn)
                cmdHead.Parameters.AddWithValue("@now_local", DateTime.Now)
                cmdHead.Parameters.AddWithValue("@inv", trans_invno.Text)
                cmdHead.Parameters.AddWithValue("@cus", SelectedCustomerID)
                cmdHead.Parameters.AddWithValue("@uid", cmbCashier.SelectedValue)
                cmdHead.Parameters.AddWithValue("@c_status", currentCashStatus)
                cmdHead.Parameters.AddWithValue("@o_uid", orderUID)
                cmdHead.Parameters.AddWithValue("@coll_uid", collectorUID)

                ' SAFE conversion for currency fields
                Dim subtotalVal, disVal, grandVal, refundVal As Decimal
                ' Using Val() for robust extraction as labels might have extra formatting
                subtotalVal = Val(totalamount.Text.Replace(",", ""))
                disVal = Val(invdis.Text.Replace(",", ""))
                grandVal = Val(grandtotal.Text.Replace(",", ""))
                refundVal = Val(return_amt.Text.Replace(",", ""))

                cmdHead.Parameters.AddWithValue("@sub", subtotalVal)
                cmdHead.Parameters.AddWithValue("@dis", disVal)
                cmdHead.Parameters.AddWithValue("@grand", grandVal)
                cmdHead.Parameters.AddWithValue("@refund", refundVal)
                cmdHead.Parameters.AddWithValue("@ctype", ComboBox5.Text)

                ' NEW: Role-based tracking for Returns
                Dim cStatus As String = "COLLECTED"
                Dim collectorId As Object = Module1.CurrentUserID
                If Module1.UserRole.ToLower() = "order taker" Then
                    cStatus = "PENDING"
                    collectorId = DBNull.Value
                End If
                cmdHead.Parameters.AddWithValue("@c_status", cStatus)
                cmdHead.Parameters.AddWithValue("@o_uid", Module1.CurrentUserID)
                cmdHead.Parameters.AddWithValue("@coll_uid", collectorId)

                returnID = Convert.ToInt32(cmdHead.ExecuteScalar())
            End Using

            For i As Integer = 0 To DataGridView2.Rows.Count - 1
                Dim row = DataGridView2.Rows(i)

                ' Check if Return Qty is valid
                Dim nqty As Decimal = 0
                If row.Cells("Return Qty").Value IsNot Nothing AndAlso Decimal.TryParse(row.Cells("Return Qty").Value.ToString(), nqty) AndAlso nqty > 0 Then

                    Dim itCode = row.Cells("Item ID").Value.ToString()
                    Dim retAmt = If(row.Cells("Return Amount").Value Is Nothing, 0, Convert.ToDecimal(row.Cells("Return Amount").Value))

                    ' 2. Insert into sales_return_items (The Details)

                    sql = "INSERT INTO sales_return_items (return_id, item_id, description, qty, unit_price, discount, return_amount, cost_price, return_profit, reason) " &
                      "VALUES (@rid, @it, @des, @qty, @price, @dis, @ramt, @cost, @prof, 'Manual Return')"
                    ' Safe conversion for item details
                    Dim priceVal, discVal, costVal As Decimal
                    Decimal.TryParse(If(row.Cells("Price").Value, "0").ToString(), priceVal)
                    Decimal.TryParse(If(row.Cells("Dis").Value, "0").ToString(), discVal)
                    Decimal.TryParse(If(row.Cells("Cost").Value, "0").ToString(), costVal)

                    Dim retProf = (retAmt / nqty - costVal) * nqty

                    Using cmdItem = New MySqlCommand(sql, conn)
                        cmdItem.Parameters.AddWithValue("@rid", returnID)
                        cmdItem.Parameters.AddWithValue("@it", itCode)
                        cmdItem.Parameters.AddWithValue("@des", If(row.Cells("Description").Value, "").ToString())
                        cmdItem.Parameters.AddWithValue("@qty", nqty)
                        cmdItem.Parameters.AddWithValue("@price", priceVal)
                        cmdItem.Parameters.AddWithValue("@dis", discVal)
                        cmdItem.Parameters.AddWithValue("@ramt", retAmt)
                        cmdItem.Parameters.AddWithValue("@cost", costVal)
                        cmdItem.Parameters.AddWithValue("@prof", retProf)
                        cmdItem.ExecuteNonQuery()
                    End Using

                    ' Update header total profit loss
                    Dim updProfSql = "UPDATE sales_return SET total_return_profit = total_return_profit + @prof WHERE id = @rid"
                    Using cmdUpdProf = New MySqlCommand(updProfSql, conn)
                        cmdUpdProf.Parameters.AddWithValue("@prof", retProf)
                        cmdUpdProf.Parameters.AddWithValue("@rid", returnID)
                        cmdUpdProf.ExecuteNonQuery()
                    End Using

                    ' 3. Update original billing_item (REDUCE sold quantity)
                    Dim biid = row.Cells("BIID").Value.ToString()
                    sql = "UPDATE billing_item SET quantity = quantity - @rqty WHERE id = @biid"
                    Using cmdBillItem = New MySqlCommand(sql, conn)
                        cmdBillItem.Parameters.AddWithValue("@rqty", nqty)
                        cmdBillItem.Parameters.AddWithValue("@biid", biid)
                        cmdBillItem.ExecuteNonQuery()
                    End Using

                    ' 4. Revert Stock in items table (Main Stock)
                    sql = "UPDATE items SET st_qty = st_qty + @rqty WHERE id = @ic"
                    Using cmdItems = New MySqlCommand(sql, conn)
                        cmdItems.Parameters.AddWithValue("@rqty", nqty)
                        cmdItems.Parameters.AddWithValue("@ic", itCode)
                        cmdItems.ExecuteNonQuery()
                    End Using

                    ' 4.1 Update items_stock (MAIN STOCK)
                    sql = "UPDATE items_stock SET st_qty = st_qty + @rqty WHERE item_id = @ic AND location_id = (SELECT id FROM location WHERE location_name = 'MAIN STOCK' LIMIT 1) ORDER BY date DESC LIMIT 1"
                    Using cmdBatch = New MySqlCommand(sql, conn)
                        cmdBatch.Parameters.AddWithValue("@rqty", nqty)
                        cmdBatch.Parameters.AddWithValue("@ic", itCode)
                        Dim affected As Integer = cmdBatch.ExecuteNonQuery()
                        If affected = 0 Then
                            ' Insert a fallback batch if no main stock batch exists
                            Dim costFallback As Decimal = 0
                            Try
                                Using cmdC = New MySqlCommand("SELECT avg_cost FROM items WHERE id=@id", conn)
                                    cmdC.Parameters.AddWithValue("@id", itCode)
                                    Dim costObj = cmdC.ExecuteScalar()
                                    If costObj IsNot Nothing AndAlso Not DBNull.Value.Equals(costObj) Then
                                        costFallback = Convert.ToDecimal(costObj)
                                    End If
                                End Using
                            Catch
                            End Try
                            Dim sqlIns = "INSERT INTO items_stock (item_id, item_cost, st_qty, date, location_id, supplier_id) VALUES (@ic, @cost, @rqty, @now_local, (SELECT id FROM location WHERE location_name = 'MAIN STOCK' LIMIT 1), 1)"
                            Using cmdIns = New MySqlCommand(sqlIns, conn)
                                cmdIns.Parameters.AddWithValue("@ic", itCode)
                                cmdIns.Parameters.AddWithValue("@cost", costFallback)
                                cmdIns.Parameters.AddWithValue("@rqty", nqty)
                                cmdIns.Parameters.AddWithValue("@now_local", DateTime.Now)
                                cmdIns.ExecuteNonQuery()
                            End Using
                        End If
                    End Using

                    ' 5. Revert Stock in all_item (History/Audit)
                    ' Simplified query to avoid "Truncated incorrect DECIMAL value" error
                    sql = "UPDATE all_item SET quantity = quantity - @nqty WHERE item_id = @ic"
                    Using cmdUpd = New MySqlCommand(sql, conn)
                        cmdUpd.Parameters.AddWithValue("@nqty", nqty)
                        cmdUpd.Parameters.AddWithValue("@ic", itCode)
                        cmdUpd.ExecuteNonQuery()
                    End Using
                End If
            Next

            MessageBox.Show("Return history saved and stock updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            txtCashierID.Text = ""
            conn.Close()
            sale_return(trans_invno.Text)
        Catch ex As Exception
            MessageBox.Show("Error processing return: " & ex.Message)
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub btnchange_Click(sender As Object, e As EventArgs) Handles btnchange.Click
        ClearForm()
    End Sub

    Private Sub ClearForm()
        trans_invno.Text = ""
        tran_cus.Text = ""
        trans_itcode.Text = ""
        trans_des.Text = ""
        trans_qyt.Text = "0"
        trans_unit.Text = "0.00"
        trans_dis.Text = "0"
        trans_amount.Text = "0.00"
        totalamount.Text = "0.00"
        invdis.Text = "0.00"
        grandtotal.Text = "0.00"
        discount.Text = "0.0"
        return_amt.Text = "0.00"
        return_debit.Text = "0.00"
        return_cash.Text = ""
        txtCashierID.Text = ""
        remain_stock.Text = "0"
        Label9.Text = "0.00"
        SelectedCustomerID = 0
        DataGridView2.DataSource = Nothing
        trans_invno.Focus()
    End Sub

    Private Sub FetchCustomerBalance(customerId As String)
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()
            ' Simplified SUM query
            sql = "SELECT SUM(balance_due) FROM billing WHERE customer_id = @cus"
            Using cmdBal = New MySqlCommand(sql, conn)
                cmdBal.Parameters.AddWithValue("@cus", customerId)
                Dim res = cmdBal.ExecuteScalar()
                If res IsNot DBNull.Value AndAlso res IsNot Nothing Then
                    return_debit.Text = Convert.ToDecimal(res).ToString("F2")
                Else
                    return_debit.Text = "0.00"
                End If
            End Using
        Catch ex As Exception
            ' If it fails, just set to 0 to avoid blocking the whole process
            return_debit.Text = "0.00"
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub return_cash_TextChanged(sender As Object, e As EventArgs) Handles return_cash.TextChanged
        CalculateCashReturn()
    End Sub

    Private Sub return_amt_TextChanged(sender As Object, e As EventArgs) Handles return_amt.TextChanged
        CalculateCashReturn()
    End Sub

    Private Sub CalculateCashReturn()
        Dim cashPaid, rtnAmt As Decimal
        Decimal.TryParse(return_cash.Text, cashPaid)
        Decimal.TryParse(return_amt.Text, rtnAmt)

        ' Cash Return = Cash Payment - Return Amt
        Label9.Text = (cashPaid - rtnAmt).ToString("F2")
    End Sub

    Private Sub SaleReturn_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Escape Then Me.Close()
    End Sub


    Private Sub LoadCashiers()
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()
            ' Correct query joining user and user_role. Including 'owner' role as well.
            sql = "SELECT u.id, u.name, u.password FROM user u JOIN user_role r ON u.role_id = r.id WHERE (r.role_name = 'admin' OR r.role_name = 'cashier' OR r.role_name = 'owner') AND (u.status IS NULL OR u.status = 'active')"

            cmd = New MySqlCommand(sql, conn)
            Dim da As New MySqlDataAdapter(cmd)
            Dim dt As New DataTable()
            da.Fill(dt)

            cmbCashier.DataSource = dt
            cmbCashier.DisplayMember = "name"
            cmbCashier.ValueMember = "id"
            cmbCashier.SelectedIndex = -1

            ' Enable AutoComplete so typing suggests cashier names
            Dim autoList As New AutoCompleteStringCollection()
            For Each row As DataRow In dt.Rows
                autoList.Add(row("name").ToString())
            Next
            cmbCashier.AutoCompleteSource = AutoCompleteSource.CustomSource
            cmbCashier.AutoCompleteCustomSource = autoList
            cmbCashier.AutoCompleteMode = AutoCompleteMode.SuggestAppend

            conn.Close()
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub cmbCashier_KeyDown(sender As Object, e As KeyEventArgs) Handles cmbCashier.KeyDown
        If e.KeyCode = Keys.Enter Then
            txtCashierID.Focus()
            e.SuppressKeyPress = True ' Prevent ding sound
        End If
    End Sub

    Private Sub txtCashierID_KeyDown(sender As Object, e As KeyEventArgs) Handles txtCashierID.KeyDown
        If e.KeyCode = Keys.Enter Then
            btnSaveReturn.PerformClick()
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub btnReturnLog_Click(sender As Object, e As EventArgs) Handles btnReturnLog.Click
        SaleReturnlog.Show()
    End Sub
End Class
