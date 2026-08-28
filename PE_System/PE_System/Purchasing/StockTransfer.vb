Imports MySql.Data.MySqlClient

Public Class StockTransfer

    Private Sub StockTransfer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.KeyPreview = True
        LoadLocations()
        FormatDataGridView2()
        trann.Select()
    End Sub

    Private Sub StockTransfer_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.F2 Then
            Button1.PerformClick()
            e.Handled = True
        ElseIf e.KeyCode = Keys.F12 Then
            btnSave.PerformClick()
            e.Handled = True
        End If
    End Sub

    Private Sub LoadSenderDetails()
        ' Intentionally left empty to allow manual entry for Sender Name and Telephone
    End Sub



    Private Sub FormatDataGridView2()
        DataGridView2.AllowUserToAddRows = False
        DataGridView2.AllowUserToDeleteRows = False
        DataGridView2.ReadOnly = True
        DataGridView2.RowHeadersVisible = False
        DataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        DataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        DataGridView2.BackgroundColor = SystemColors.ButtonFace
        DataGridView2.DefaultCellStyle.Font = New Font("Microsoft Sans Serif", 14)
        DataGridView2.ColumnHeadersDefaultCellStyle.Font = New Font("Microsoft Sans Serif", 16, FontStyle.Bold)
        DataGridView2.DefaultCellStyle.ForeColor = Color.Black
        DataGridView2.ForeColor = Color.Black
        DataGridView2.DefaultCellStyle.SelectionBackColor = SystemColors.Highlight
        DataGridView2.DefaultCellStyle.SelectionForeColor = SystemColors.HighlightText

        DataGridView2.DataSource = Nothing
        DataGridView2.Columns.Clear()
        
        ' Default columns for item entry when starting fresh
        DataGridView2.Columns.Add("TransferId", "Transfer Id")
        DataGridView2.Columns.Add("ItemID", "Item Id")
        DataGridView2.Columns.Add("Description", "Description")
        DataGridView2.Columns.Add("Stock", "Stock")
        DataGridView2.Columns.Add("RequestQty", "Request Qty")
    End Sub

    Private Sub LoadLocations()
        Try
            ComboBoxLocation.Items.Clear()
            ComboBox1loca.Items.Clear()
            Dim query As String = "SELECT location_name FROM location"
            Using connection As New MySqlConnection(Module1.ConnStr)
                Using cmd As New MySqlCommand(query, connection)
                    connection.Open()
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim locName As String = reader("location_name").ToString()
                            ComboBoxLocation.Items.Add(locName)
                            ComboBox1loca.Items.Add(locName)
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading locations: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub



    Private Sub trann_Click(sender As Object, e As EventArgs) Handles trann.Click, trann.Enter
        If sdetailsbtn.Text = "Back" Then
            ClearFields()
        End If
    End Sub

    Private Sub trann_TextChanged(sender As Object, e As EventArgs) Handles trann.TextChanged
        Dim keyword As String = trann.Text.Trim()

        If keyword = "" Then
            FormatDataGridView2()
            Return
        End If

        Try
            Dim query As String = "SELECT DISTINCT ss.transfer_id AS 'Transfer Id', " &
                                  "ss.requester_name AS 'Request Name', " &
                                  "ss.requester_tel_no AS 'Request Tel', " &
                                  "l.location_name AS 'Location', " &
                                  "ol.location_name AS 'Our Location' " &
                                  "FROM sending_stock ss " &
                                  "LEFT JOIN receive_stock rs ON ss.id = rs.sending_stock_id " &
                                  "LEFT JOIN location l ON ss.receive_location_id = l.id " &
                                  "LEFT JOIN location ol ON ss.our_location = ol.id " &
                                  "WHERE rs.id IS NULL AND ss.transfer_id LIKE @keyword LIMIT 50"
            Using connection As New MySqlConnection(Module1.ConnStr)
                Using cmd As New MySqlCommand(query, connection)
                    cmd.Parameters.AddWithValue("@keyword", "%" & keyword & "%")
                    Using adapter As New MySqlDataAdapter(cmd)
                        Dim table As New DataTable()
                        adapter.Fill(table)

                        DataGridView2.DataSource = Nothing
                        DataGridView2.Columns.Clear()
                        DataGridView2.DataSource = table
                        
                        If table.Rows.Count > 0 Then
                            DataGridView2.Columns("Transfer Id").Width = 180
                            DataGridView2.Columns("Request Name").Width = 250
                            DataGridView2.Columns("Request Tel").Width = 180
                            DataGridView2.Columns("Location").Width = 250
                            DataGridView2.Columns("Our Location").Width = 250
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error searching transfer ID: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub trann_KeyDown(sender As Object, e As KeyEventArgs) Handles trann.KeyDown
        If e.KeyCode = Keys.Enter Then
            If DataGridView2.Rows.Count > 0 Then
                btnNext.PerformClick()
            End If
            e.SuppressKeyPress = True
        ElseIf e.KeyCode = Keys.Down Then
            If DataGridView2.Rows.Count > 0 Then
                DataGridView2.Focus()
                ' Ensure first row is selected/highlighted
                If DataGridView2.CurrentCell Is Nothing Then
                    DataGridView2.CurrentCell = DataGridView2.Rows(0).Cells(0)
                End If
            End If
        End If
    End Sub



    Private Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
        If DataGridView2.SelectedRows.Count = 0 AndAlso DataGridView2.CurrentRow Is Nothing Then
            MessageBox.Show("Please select a Transfer record first.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim row As DataGridViewRow = If(DataGridView2.SelectedRows.Count > 0, DataGridView2.SelectedRows(0), DataGridView2.CurrentRow)
        
        If DataGridView2.Columns.Contains("Request Name") Then
            ' Currently in Master View. Load Items for this Transfer.
            Dim transId As String = row.Cells("Transfer Id").Value.ToString()
            Dim reqName As String = row.Cells("Request Name").Value.ToString()
            Dim reqTel As String = row.Cells("Request Tel").Value.ToString()
            Dim location As String = row.Cells("Location").Value.ToString()
            Dim ourLocation As String = row.Cells("Our Location").Value.ToString()

            trann.Text = transId
            TextBox2.Text = reqName
            recetel.Text = reqTel
            ComboBoxLocation.Text = ourLocation ' sender
            ComboBox1loca.Text = location ' receiver

            Try
                Dim query As String = "SELECT ss.transfer_id AS 'Transfer Id', " &
                                      "i.item_id AS 'Item Id', " &
                                      "i.description AS 'Description', " &
                                      "(SELECT IFNULL(SUM(CAST(st_qty AS DECIMAL(10,2))), 0) FROM items_stock WHERE item_id = i.item_id AND location_id = ss.our_location) AS 'Stock', " &
                                      "ss.request_quantity AS 'Request Qty' " &
                                      "FROM sending_stock ss " &
                                      "INNER JOIN items_stock i ON ss.items_stock_id = i.id " &
                                      "LEFT JOIN receive_stock rs ON ss.id = rs.sending_stock_id " &
                                      "WHERE rs.id IS NULL AND ss.transfer_id = @transId"
                
                Using connection As New MySqlConnection(Module1.ConnStr)
                    Using cmd As New MySqlCommand(query, connection)
                        cmd.Parameters.AddWithValue("@transId", transId)
                        Using adapter As New MySqlDataAdapter(cmd)
                            Dim table As New DataTable()
                            adapter.Fill(table)
                            
                            DataGridView2.DataSource = table
                            If DataGridView2.Columns.Contains("Transfer Id") Then
                                DataGridView2.Columns("Transfer Id").Visible = False
                            End If
                        End Using
                    End Using
                End Using
            Catch ex As Exception
                MessageBox.Show("Error loading items: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        If String.IsNullOrWhiteSpace(trann.Text) Then
            MessageBox.Show("Please load transfer details first.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If



        If DataGridView2.Rows.Count = 0 Then
            MessageBox.Show("Please add items to the list first using 'Next' button.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If TextBox7.Text.Trim() = "" Then
            MessageBox.Show("Please fill out Sender Name before saving.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TextBox7.Focus()
            Return
        End If

        If TextBox4.Text.Trim() = "" Then
            MessageBox.Show("Please fill out Sender Te No before saving.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TextBox4.Focus()
            Return
        End If

        Try
            Using connection As New MySqlConnection(Module1.ConnStr)
                connection.Open()

                Using transaction = connection.BeginTransaction()
                    Try
                        For Each gRow As DataGridViewRow In DataGridView2.Rows
                            If gRow.IsNewRow Then Continue For

                            Dim transId As String = gRow.Cells("Transfer Id").Value.ToString()
                            Dim itemId As String = gRow.Cells("Item Id").Value.ToString()
                            Dim destDesc As String = gRow.Cells("Description").Value.ToString()
                            Dim transferQty As Decimal = Convert.ToDecimal(gRow.Cells("Request Qty").Value)
                            
                            ' We need to know the Source Location and Destination Location. 
                            ' Since they are processed collectively, they should be in the TextBoxes
                            Dim sourceLocationName As String = ComboBoxLocation.Text.Trim() 
                            Dim destLocationName As String = ComboBox1loca.Text.Trim() 

                            If String.IsNullOrEmpty(sourceLocationName) OrElse String.IsNullOrEmpty(destLocationName) Then
                                Throw New Exception("Source or Destination Location is missing.")
                            End If

                            Dim sourceLocationId As Integer = 0
                            Dim destLocationId As Integer = 0

                            ' Get the location IDs directly from the sending_stock record for this transfer/item
                            Dim fetchLocQuery As String = "SELECT our_location, receive_location_id FROM sending_stock WHERE transfer_id = @transId AND items_stock_id IN (SELECT id FROM items_stock WHERE item_id = @itemId) LIMIT 1"
                            Using cmdLocFetch As New MySqlCommand(fetchLocQuery, connection, transaction)
                                cmdLocFetch.Parameters.AddWithValue("@transId", transId)
                                cmdLocFetch.Parameters.AddWithValue("@itemId", itemId)
                                Using rdr = cmdLocFetch.ExecuteReader()
                                    If rdr.Read() Then
                                        Integer.TryParse(rdr("our_location").ToString(), sourceLocationId)
                                        Integer.TryParse(rdr("receive_location_id").ToString(), destLocationId)
                                    End If
                                End Using
                            End Using

                            ' Falling back to textboxes if database fetch failed (unlikely)
                            If sourceLocationId = 0 Then
                                Using cmdLoc As New MySqlCommand("SELECT id FROM location WHERE location_name = @name LIMIT 1", connection, transaction)
                                    cmdLoc.Parameters.AddWithValue("@name", sourceLocationName)
                                    Dim res = cmdLoc.ExecuteScalar()
                                    If res IsNot Nothing AndAlso res IsNot DBNull.Value Then Integer.TryParse(res.ToString(), sourceLocationId)
                                End Using
                            End If
                            If destLocationId = 0 Then
                                Using cmdLoc As New MySqlCommand("SELECT id FROM location WHERE location_name = @name LIMIT 1", connection, transaction)
                                    cmdLoc.Parameters.AddWithValue("@name", destLocationName)
                                    Dim res = cmdLoc.ExecuteScalar()
                                    If res IsNot Nothing AndAlso res IsNot DBNull.Value Then Integer.TryParse(res.ToString(), destLocationId)
                                End Using
                            End If

                            ' Update the names for error messaging
                            Using cmdN As New MySqlCommand("SELECT location_name FROM location WHERE id = @id", connection, transaction)
                                cmdN.Parameters.AddWithValue("@id", sourceLocationId)
                                Dim n = cmdN.ExecuteScalar()
                                If n IsNot Nothing Then sourceLocationName = n.ToString()
                            End Using
                             Using cmdN As New MySqlCommand("SELECT location_name FROM location WHERE id = @id", connection, transaction)
                                cmdN.Parameters.AddWithValue("@id", destLocationId)
                                Dim n = cmdN.ExecuteScalar()
                                If n IsNot Nothing Then destLocationName = n.ToString()
                            End Using

                            If sourceLocationId = 0 Then
                                Throw New Exception("Source Location '" & sourceLocationName & "' not found in database.")
                            End If
                            If destLocationId = 0 Then
                                Throw New Exception("Destination Location '" & destLocationName & "' not found in database.")
                            End If

                            ' Check total stock in source
                            Dim totalSourceStock As Decimal = 0
                            Using cmdCheck As New MySqlCommand("SELECT IFNULL(SUM(CAST(st_qty AS DECIMAL(10,2))), 0) FROM items_stock WHERE item_id = @item_id AND location_id = @loc_id", connection, transaction)
                                cmdCheck.Parameters.AddWithValue("@item_id", itemId)
                                cmdCheck.Parameters.AddWithValue("@loc_id", sourceLocationId)
                                Dim tStock = cmdCheck.ExecuteScalar()
                                If tStock IsNot Nothing AndAlso tStock IsNot DBNull.Value Then
                                    Decimal.TryParse(tStock.ToString(), totalSourceStock)
                                End If
                            End Using

                            If totalSourceStock < transferQty Then
                                Throw New Exception("Not enough stock in '" & sourceLocationName & "' to complete transfer for item " & itemId & ". Found only: " & totalSourceStock)
                            End If

                            ' FIFO implementation
                            Dim fetchRowsQuery As String = "SELECT * FROM items_stock WHERE item_id = @item_id AND location_id = @loc_id AND CAST(st_qty AS DECIMAL(10,2)) > 0 ORDER BY date ASC"
                            Dim batches As New DataTable()
                            Using cmdFetch As New MySqlCommand(fetchRowsQuery, connection, transaction)
                                cmdFetch.Parameters.AddWithValue("@item_id", itemId)
                                cmdFetch.Parameters.AddWithValue("@loc_id", sourceLocationId)
                                Using adapter As New MySqlDataAdapter(cmdFetch)
                                    adapter.Fill(batches)
                                End Using
                            End Using

                            Dim remainingQty As Decimal = transferQty

                            For Each row As DataRow In batches.Rows
                                If remainingQty <= 0 Then Exit For

                                Dim batchId As String = ""
                                If row("id") IsNot DBNull.Value Then batchId = row("id").ToString()

                                Dim batchQty As Decimal = 0
                                If row("st_qty") IsNot DBNull.Value Then Decimal.TryParse(row("st_qty").ToString(), batchQty)

                                Dim batchDate As DateTime = DateTime.Now
                                If row("date") IsNot DBNull.Value Then DateTime.TryParse(row("date").ToString(), batchDate)

                                Dim qtyToTake As Decimal = Math.Min(batchQty, remainingQty)

                                ' Deduct from source
                                Dim updateSourceSql As String = "UPDATE items_stock SET st_qty = CAST(st_qty AS DECIMAL(10,2)) - @qtyToTake WHERE id = @batch_id"
                                Using cmdUpdSrc As New MySqlCommand(updateSourceSql, connection, transaction)
                                    cmdUpdSrc.Parameters.AddWithValue("@qtyToTake", qtyToTake)
                                    cmdUpdSrc.Parameters.AddWithValue("@batch_id", batchId)
                                    cmdUpdSrc.ExecuteNonQuery()
                                End Using

                                ' Insert/Update to destination maintaining FIFO dates
                                Dim destBatchId As String = ""
                                Dim checkDestSql As String = "SELECT id FROM items_stock WHERE item_id = @item_id AND location_id = @loc_id AND DATE(date) = DATE(@batch_date) LIMIT 1"
                                Using cmdCheckDest As New MySqlCommand(checkDestSql, connection, transaction)
                                    cmdCheckDest.Parameters.AddWithValue("@item_id", itemId)
                                    cmdCheckDest.Parameters.AddWithValue("@loc_id", destLocationId)
                                    cmdCheckDest.Parameters.AddWithValue("@batch_date", batchDate)
                                    Dim res = cmdCheckDest.ExecuteScalar()
                                    If res IsNot Nothing AndAlso res IsNot DBNull.Value Then
                                        destBatchId = res.ToString()
                                    End If
                                End Using

                                If destBatchId <> "" Then
                                    ' Update existing destination batch
                                    Dim updateDestSql As String = "UPDATE items_stock SET st_qty = CAST(st_qty AS DECIMAL(10,2)) + @qtyToTake WHERE id = @dest_batch_id"
                                    Using cmdUpdDest As New MySqlCommand(updateDestSql, connection, transaction)
                                        cmdUpdDest.Parameters.AddWithValue("@qtyToTake", qtyToTake)
                                        cmdUpdDest.Parameters.AddWithValue("@dest_batch_id", destBatchId)
                                        cmdUpdDest.ExecuteNonQuery()
                                    End Using
                                Else
                                    ' Insert new destination batch with full tracking history copied
                                    Dim newId As String = "TRF_" & Guid.NewGuid().ToString("N").Substring(0, 8) & "_" & itemId
                                    Dim insertDestSql As String = "INSERT INTO items_stock (id, item_id, st_qty, qty_purchased, location_id, inv_no, supplier_id, item_cost, avg_cost, selling_price, whole_selling_price, retail_selling_price, amount, description, date, discount) " &
                                                                  "VALUES (@id, @item_id, @st_qty, @qty_purchased, @loc_id, @inv_no, @sup_id, @item_cost, @avg_cost, @sell_p, @w_sell_p, @r_sell_p, @amount, @desc, @date, @discount)"
                                    Using cmdInsDest As New MySqlCommand(insertDestSql, connection, transaction)
                                        cmdInsDest.Parameters.AddWithValue("@id", newId)
                                        cmdInsDest.Parameters.AddWithValue("@item_id", itemId)
                                        cmdInsDest.Parameters.AddWithValue("@st_qty", qtyToTake)
                                        cmdInsDest.Parameters.AddWithValue("@qty_purchased", If(row("qty_purchased") IsNot DBNull.Value, row("qty_purchased"), 0))
                                        cmdInsDest.Parameters.AddWithValue("@loc_id", destLocationId)
                                        cmdInsDest.Parameters.AddWithValue("@inv_no", If(row("inv_no") IsNot DBNull.Value, row("inv_no"), ""))
                                        cmdInsDest.Parameters.AddWithValue("@sup_id", If(row("supplier_id") IsNot DBNull.Value, row("supplier_id"), DBNull.Value))
                                        cmdInsDest.Parameters.AddWithValue("@item_cost", If(row("item_cost") IsNot DBNull.Value, row("item_cost"), 0))
                                        cmdInsDest.Parameters.AddWithValue("@avg_cost", If(row("avg_cost") IsNot DBNull.Value, row("avg_cost"), 0))
                                        cmdInsDest.Parameters.AddWithValue("@sell_p", If(row("selling_price") IsNot DBNull.Value, row("selling_price"), 0))
                                        cmdInsDest.Parameters.AddWithValue("@w_sell_p", If(row("whole_selling_price") IsNot DBNull.Value, row("whole_selling_price"), 0))
                                        cmdInsDest.Parameters.AddWithValue("@r_sell_p", If(row("retail_selling_price") IsNot DBNull.Value, row("retail_selling_price"), 0))
                                        cmdInsDest.Parameters.AddWithValue("@amount", If(row("amount") IsNot DBNull.Value, row("amount"), 0))
                                        cmdInsDest.Parameters.AddWithValue("@desc", If(row("description") IsNot DBNull.Value, row("description"), ""))
                                        cmdInsDest.Parameters.AddWithValue("@date", batchDate)
                                        cmdInsDest.Parameters.AddWithValue("@discount", If(row("discount") IsNot DBNull.Value, row("discount"), 0))
                                        cmdInsDest.ExecuteNonQuery()
                                    End Using
                                End If

                                remainingQty -= qtyToTake
                            Next

                            Dim sendingStockId As Integer = 0
                            Dim fetchSendingIdQuery As String = "SELECT id FROM sending_stock WHERE transfer_id = @transfer_id AND items_stock_id IN (SELECT id FROM items_stock WHERE item_id = @item_id) LIMIT 1"
                            Using cmdSending As New MySqlCommand(fetchSendingIdQuery, connection, transaction)
                                cmdSending.Parameters.AddWithValue("@transfer_id", transId)
                                cmdSending.Parameters.AddWithValue("@item_id", itemId)
                                Dim sIdRes = cmdSending.ExecuteScalar()
                                If sIdRes IsNot Nothing AndAlso sIdRes IsNot DBNull.Value Then
                                    Integer.TryParse(sIdRes.ToString(), sendingStockId)
                                End If
                            End Using

                            If sendingStockId > 0 Then
                                Dim insertReceiveSql As String = "INSERT INTO receive_stock (sending_stock_id, transfer_id, send_quantity, sender_name, sender_tel_no, receiver_name, receiver_tel_no, received_at) " &
                                                                 "VALUES (@sending_stock_id, @transfer_id, @send_quantity, @sender_name, @sender_tel_no, @receiver_name, @receiver_tel_no, NOW())"
                                Using cmdRec As New MySqlCommand(insertReceiveSql, connection, transaction)
                                    cmdRec.Parameters.AddWithValue("@sending_stock_id", sendingStockId)
                                    cmdRec.Parameters.AddWithValue("@transfer_id", transId)
                                    cmdRec.Parameters.AddWithValue("@send_quantity", transferQty)
                                    cmdRec.Parameters.AddWithValue("@sender_name", TextBox7.Text.Trim())
                                    cmdRec.Parameters.AddWithValue("@sender_tel_no", TextBox4.Text.Trim())
                                    cmdRec.Parameters.AddWithValue("@receiver_name", TextBox2.Text.Trim())
                                    cmdRec.Parameters.AddWithValue("@receiver_tel_no", recetel.Text.Trim())
                                    cmdRec.ExecuteNonQuery()
                                End Using
                            End If

                            ' Delete from receive_temp_stock
                            Dim delTempSql As String = "DELETE FROM receive_temp_stock WHERE transfer_id = @transfer_id AND item_id = @item_id"
                            Using cmdDel As New MySqlCommand(delTempSql, connection, transaction)
                                cmdDel.Parameters.AddWithValue("@transfer_id", transId)
                                cmdDel.Parameters.AddWithValue("@item_id", itemId)
                                cmdDel.ExecuteNonQuery()
                            End Using
                        Next

                        transaction.Commit()
                        MessageBox.Show("All grouped items from the list transferred successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                        ' Open the receiving stock report
                        Dim savedTransId As String = trann.Text.Trim()
                        If Not String.IsNullOrEmpty(savedTransId) Then
                            SaleInv.ShowReport(savedTransId, 10)
                        End If

                        ' Clear everything
                        ClearFields()

                        ' Load history and highlight
                        LoadHistoryList()
                        For Each row As DataGridViewRow In DataGridView2.Rows
                            If row.Cells("Transfer ID").Value IsNot Nothing AndAlso row.Cells("Transfer ID").Value.ToString() = savedTransId Then
                                row.Selected = True
                                DataGridView2.FirstDisplayedScrollingRowIndex = row.Index
                                Exit For
                            End If
                        Next
                    Catch ex As Exception
                        transaction.Rollback()
                        Throw ex
                    End Try
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error transferring stock: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadHistoryList()
        Try
            Dim query As String = "SELECT r.transfer_id AS 'Transfer ID', MAX(sl.location_name) AS 'Send Location', MAX(dl.location_name) AS 'Receiver Location' " &
                                  "FROM receive_stock r " &
                                  "INNER JOIN sending_stock s ON r.sending_stock_id = s.id " &
                                  "LEFT JOIN location sl ON s.our_location = sl.id " &
                                  "LEFT JOIN location dl ON s.receive_location_id = dl.id " &
                                  "GROUP BY r.transfer_id " &
                                  "ORDER BY MAX(r.received_at) DESC LIMIT 100"
            Using connection As New MySqlConnection(Module1.ConnStr)
                Using cmd As New MySqlCommand(query, connection)
                    Using adapter As New MySqlDataAdapter(cmd)
                        Dim table As New DataTable()
                        adapter.Fill(table)

                        DataGridView2.DataSource = Nothing
                        DataGridView2.Columns.Clear()
                        DataGridView2.DataSource = table

                        DataGridView2.AllowUserToAddRows = False
                        DataGridView2.ReadOnly = True
                        DataGridView2.RowHeadersVisible = False
                        DataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect
                        DataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                        DataGridView2.BackgroundColor = SystemColors.ButtonFace
                        DataGridView2.DefaultCellStyle.Font = New Font("Microsoft Sans Serif", 14)
                        DataGridView2.ColumnHeadersDefaultCellStyle.Font = New Font("Microsoft Sans Serif", 16, FontStyle.Bold)
                        DataGridView2.DefaultCellStyle.ForeColor = Color.Black

                        ' Set column widths as requested
                        If DataGridView2.Columns.Contains("Transfer ID") Then
                            DataGridView2.Columns("Transfer ID").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                            DataGridView2.Columns("Transfer ID").Width = 280
                        End If
                        If DataGridView2.Columns.Contains("Receiver Location") Then
                            DataGridView2.Columns("Receiver Location").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                            DataGridView2.Columns("Receiver Location").Width = 380
                        End If
                        If DataGridView2.Columns.Contains("Send Location") Then
                            DataGridView2.Columns("Send Location").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                        End If

                        sdetailsbtn.Text = "Back"
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading transfer history: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub sdetailsbtn_Click(sender As Object, e As EventArgs) Handles sdetailsbtn.Click
        If sdetailsbtn.Text = "Back" Then
            If DataGridView2.DataSource IsNot Nothing AndAlso DataGridView2.Columns.Contains("Item Id") Then
                ' We are perfectly in history details mode (shows Item Id). Go back to overview list.
                LoadHistoryList()
                Return
            End If

            ' Go completely back to normal mode
            FormatDataGridView2()
            sdetailsbtn.Text = "Transfer History"
            Return
        End If

        LoadHistoryList()
    End Sub



    Private Sub ClearFields()
        ' Clear all textboxes
        trann.Clear()
        TextBox7.Clear()
        TextBox4.Clear()
        TextBox2.Clear()
        recetel.Clear()
        
        ' Clear comboboxes
        ComboBoxLocation.SelectedIndex = -1
        ComboBox1loca.SelectedIndex = -1
        
        ' Reset Grid 2
        FormatDataGridView2()
        

        
        ' Enable trann
        trann.Enabled = True
        
        ' Reset sdetailsbtn
        sdetailsbtn.Text = "Transfer History"
    End Sub

    Private Sub DataGridView2_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView2.CellDoubleClick
        If e.RowIndex >= 0 Then
            If DataGridView2.DataSource IsNot Nothing AndAlso DataGridView2.Columns.Contains("Transfer ID") AndAlso Not DataGridView2.Columns.Contains("Item Id") Then
                Dim transId As String = DataGridView2.Rows(e.RowIndex).Cells("Transfer ID").Value.ToString()
                Try
                    Dim query As String = "SELECT i.item_id AS 'Item Id', i.description AS 'Description', r.send_quantity AS 'Send Qty', " &
                                          "r.sender_name AS 'Sender Name', r.sender_tel_no AS 'Sender Tel No', " &
                                          "r.receiver_name AS 'Receiver Name', r.receiver_tel_no AS 'Receiver Tel No' " &
                                          "FROM receive_stock r " &
                                          "INNER JOIN sending_stock s ON r.sending_stock_id = s.id " &
                                          "LEFT JOIN items_stock i ON s.items_stock_id = i.id " &
                                          "WHERE r.transfer_id = @transId"
                    Using connection As New MySqlConnection(Module1.ConnStr)
                        Using cmd As New MySqlCommand(query, connection)
                            cmd.Parameters.AddWithValue("@transId", transId)
                            Using adapter As New MySqlDataAdapter(cmd)
                                Dim table As New DataTable()
                                adapter.Fill(table)
                                
                                DataGridView2.DataSource = Nothing
                                DataGridView2.Columns.Clear()
                                DataGridView2.DataSource = table
                                DataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None
                                DataGridView2.ScrollBars = ScrollBars.Both
                                
                                If DataGridView2.Columns.Contains("Item Id") Then
                                    DataGridView2.Columns("Item Id").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                                    DataGridView2.Columns("Item Id").Width = 120
                                End If
                                If DataGridView2.Columns.Contains("Description") Then
                                    DataGridView2.Columns("Description").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                                    DataGridView2.Columns("Description").Width = 400
                                End If
                                If DataGridView2.Columns.Contains("Send Qty") Then
                                    DataGridView2.Columns("Send Qty").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                                    DataGridView2.Columns("Send Qty").Width = 140
                                End If
                                If DataGridView2.Columns.Contains("Sender Name") Then
                                    DataGridView2.Columns("Sender Name").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                                    DataGridView2.Columns("Sender Name").Width = 190
                                End If
                                If DataGridView2.Columns.Contains("Sender Tel No") Then
                                    DataGridView2.Columns("Sender Tel No").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                                    DataGridView2.Columns("Sender Tel No").Width = 190
                                End If
                                If DataGridView2.Columns.Contains("Receiver Name") Then
                                    DataGridView2.Columns("Receiver Name").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                                    DataGridView2.Columns("Receiver Name").Width = 210
                                End If
                                If DataGridView2.Columns.Contains("Receiver Tel No") Then
                                    DataGridView2.Columns("Receiver Tel No").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                                    DataGridView2.Columns("Receiver Tel No").Width = 210
                                End If
                            End Using
                        End Using
                    End Using
                Catch ex As Exception
                    MessageBox.Show("Error loading transfer details: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ClearFields()
        trann.Focus()
    End Sub

    Private Sub TextBox7_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox7.KeyDown
        If e.KeyCode = Keys.Enter Then
            TextBox4.Focus()
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub TextBox4_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox4.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                TextBox7.Focus()
            Else
                btnSave.Focus()
            End If
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub DataGridView2_KeyDown(sender As Object, e As KeyEventArgs) Handles DataGridView2.KeyDown
        If e.KeyCode = Keys.Enter Then
            If DataGridView2.Columns.Contains("Request Name") Then
                btnNext_Click(Nothing, Nothing)
                e.SuppressKeyPress = True
                e.Handled = True
            End If
        End If
    End Sub
    Private Sub DataGridView2_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView2.CellClick
        If e.RowIndex >= 0 Then
            Dim row As DataGridViewRow = DataGridView2.Rows(e.RowIndex)
            
            ' Only fill textboxes if we are in search/master mode (cols like "Request Name" exist)
            If DataGridView2.Columns.Contains("Request Name") Then
                RemoveHandler trann.TextChanged, AddressOf trann_TextChanged
                
                If DataGridView2.Columns.Contains("Transfer Id") Then trann.Text = row.Cells("Transfer Id").Value.ToString()
                If DataGridView2.Columns.Contains("Request Name") Then TextBox2.Text = row.Cells("Request Name").Value.ToString()
                If DataGridView2.Columns.Contains("Request Tel") Then recetel.Text = row.Cells("Request Tel").Value.ToString()
                If DataGridView2.Columns.Contains("Location") Then ComboBox1loca.Text = row.Cells("Location").Value.ToString() ' Receiver
                If DataGridView2.Columns.Contains("Our Location") Then ComboBoxLocation.Text = row.Cells("Our Location").Value.ToString() ' Sender
                
                AddHandler trann.TextChanged, AddressOf trann_TextChanged

                ' Automatically load items for this transfer (Trigger Next button logic)
                btnNext_Click(Nothing, Nothing)
            End If
        End If
    End Sub
End Class