Imports MySql.Data.MySqlClient
Partial Public Class PurchaEntry
    Dim COMMAND As MySqlCommand
    Dim READER As MySqlDataReader
    Dim stockqty As Double
    Dim quty As Double
    Dim selectedSupplierID As Integer = 0
    Dim historyLevel As Integer = 0 ' 0: Normal, 1: Suppliers, 2: Invoices, 3: Items
    Dim lastSelectedSupName As String = ""
    Dim lastSelectedInvNo As String = ""
    Dim editingTempID As String = ""
    Dim _selectedDebitLimit As Double = 0
    Dim _selectedDebitPeriod As Date = Date.MaxValue
    Dim isHistoryFromSave As Boolean = False

    ' Context Menu for History Grid (Manual Debit Entry)
    Private WithEvents cmsHistory As New ContextMenuStrip()
    Private WithEvents tsmiAddDebit As New ToolStripMenuItem("Add Manual Debit for this Invoice")
    Private WithEvents tsmiAddGeneralDebit As New ToolStripMenuItem("Add General Debit for this Supplier")
    Private WithEvents btnPrintRequest As New Button()
    Private WithEvents chkPRMode As New Label()
    Private isPRModeChecked As Boolean = False
    Private originalInvNo As String = ""
    Private WithEvents chkPrintOnly As New CheckBox()
    Private currentActiveInvNo As String = ""


    Private Sub LockHeader(lock As Boolean)
        ' Locking and gray-out disabled as per user request to keep header fields editable and white at all times
        InvNoTxt.ReadOnly = False
        SupplierNameTxt.ReadOnly = False
        TelNoTxt.ReadOnly = False
        lblInvNo.ForeColor = Color.White
        lblSName.ForeColor = Color.White
        telLabe.ForeColor = Color.White
    End Sub

    Private Sub setup_grid_style(dgv As DataGridView)
        dgv.AllowUserToAddRows = False
        dgv.AllowUserToDeleteRows = False
        dgv.ReadOnly = True
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgv.RowHeadersVisible = False
        dgv.BackgroundColor = SystemColors.ButtonFace
        dgv.DefaultCellStyle.Font = New Font("Microsoft Sans Serif", 14)
        dgv.ColumnHeadersDefaultCellStyle.Font = New Font("Microsoft Sans Serif", 16, FontStyle.Bold)
    End Sub

    Private Sub AddProcessLog(action As String, details As String)
        Dim wasOpen As Boolean = (MySqlConn.State = ConnectionState.Open)
        Try
            If Not wasOpen Then MySqlConn.Open()
            ' Trying Inv_No as a standard underscore naming convention
            Dim query As String = "INSERT INTO purchase_process_log (Inv_No, Action, Details, LogTime) VALUES ('" & InvNoTxt.Text & "', '" & action & "', '" & details.Replace("'", "''") & "', NOW())"
            Using cmdLog As New MySqlCommand(query, MySqlConn)
                cmdLog.ExecuteNonQuery()
            End Using
        Catch ex As Exception
            ' Quietly ignore logging errors to prevent workflow disruption
        Finally
            If Not wasOpen AndAlso MySqlConn.State = ConnectionState.Open Then
                MySqlConn.Close()
            End If
        End Try
    End Sub

    Private Sub CalculateAmount()
        Try
            Dim price As Double = Val(TextBoxItemCost.Text)
            Dim qty As Double = Val(QutTextBox.Text)
            Dim discountPercent As Double = Val(DiscountTextBox.Text)

            Dim subtotal As Double = price * qty
            Dim finalAmount As Double = subtotal

            If discountPercent > 0 Then
                ' Assuming discount is a percentage as per existing logic (100 - dis)
                finalAmount = subtotal * ((100 - discountPercent) / 100)
            End If

            AmountTextBox.Text = finalAmount.ToString("F3")
        Catch ex As Exception
            AmountTextBox.Text = "0.000"
        End Try
    End Sub

    Private Sub CalculateSummary()
        Try
            ' 1. Calculate Total Amount (mgsum) from Grid
            Dim netsum As Double = 0
            For j As Integer = 0 To PurchGrideView.Rows.Count - 1
                Dim val As Object = PurchGrideView.Rows(j).Cells(6).Value
                If val IsNot Nothing AndAlso IsNumeric(val) Then
                    netsum += CDbl(val)
                End If
            Next
            mgsumTxt.Text = netsum.ToString("F3")

            ' 2. Calculate Discount and Grand Total
            Dim totalAmt As Double = netsum
            Dim discountPercent As Double = Val(DiscoText.Text) ' Val() handles empty as 0
            Dim discountAmt As Double = (totalAmt * discountPercent) / 100
            Dim grandTotal As Double = totalAmt - discountAmt

            Fullsumtxt.Text = grandTotal.ToString("F3")

            ' Always show the current grand total in creditAmtTxt as requested
            creditAmtTxt.Text = Fullsumtxt.Text

            ' 3. Calculate Balance
            Dim cashPayment As Double = Val(AmountTextBox2.Text)
            Dim balance As Double = grandTotal - cashPayment
            TextBox6.Text = balance.ToString("F3")

        Catch ex As Exception
            ' Silent fail or log
        End Try
    End Sub
    Private Sub getmarktocode()
        'add mark to the item code text fild'
        Dim str As String = IT_CodeTextBox.Text
        If str.Length() = 3 Then
            str = str + "-"
            'IT_CodeTextBox.Text = Replace(str, "(\n\r|\r|\n)", "-")
            IT_CodeTextBox.Text = str
            IT_CodeTextBox.Focus()
            IT_CodeTextBox.Select(IT_CodeTextBox.Text.Length, 4)
        End If
    End Sub
    ' itemdel() removed because DataGridView2 was deleted from UI
    Private Sub creditSeach()
        Try
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Open()
            Dim table As New DataTable()
            ' Query all suppliers and join with their total credit balance only from purchasing invoices
            Dim query As String = "SELECT s.name as 'Supplier Name', s.tel_no as 'Telephone', " &
                                 "IFNULL((SELECT SUM(balance_due) FROM purchasing WHERE supplier_id = s.id), 0) as 'Due Debit amount', " &
                                 "s.debit_limit as 'Debit Limit', s.debit_period as 'Debit Period' " &
                                 "FROM supplier s"
            Dim adapter As New MySqlDataAdapter(query, conn)
            adapter.Fill(table)

            Dim dv As New DataView(table)
            Dim filter As String = ""

            ' Filter by Name (Escaped to handle brackets like in ABNAR COMPANY [PVT] LTD)
            If Not String.IsNullOrEmpty(SupplierNameTxt.Text) Then
                filter = String.Format("[Supplier Name] Like '%{0}%'", EscapeRowFilter(SupplierNameTxt.Text).Replace("'", "''"))
            End If

            ' Filter by Telephone (Escaped to prevent any formatting/wildcard injection issues)
            If Not String.IsNullOrEmpty(TelNoTxt.Text) Then
                Dim telFilter As String = String.Format("Telephone Like '%{0}%'", EscapeRowFilter(TelNoTxt.Text).Replace("'", "''"))
                If filter <> "" Then
                    filter &= " AND " & telFilter
                Else
                    filter = telFilter
                End If
            End If

            dv.RowFilter = filter
            DataGridView1.DataSource = dv
            setup_grid_style(DataGridView1)
            DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            conn.Close()

            ' Only reset to Level 1 (Suppliers list) if history is open AND user is actively searching/typing
            ' This prevents the view from jumping back to Level 1 while the user is looking at Invoices or Items.
            If historyLevel > 1 AndAlso (SupplierNameTxt.Focused OrElse TelNoTxt.Focused) Then
                historyLevel = 1
                DataGridView1.Visible = True
                PurchGrideView.Visible = False
                sdetailsbtn.Text = "Back"
            End If
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Function GeneratePEInvoiceNumber() As String
        Dim nextNo As Integer = 1
        Dim wasOpen As Boolean = (conn.State = ConnectionState.Open)
        Try
            If Not wasOpen Then conn.Open()
            Dim query As String = "SELECT pur_id FROM purchasing WHERE pur_id LIKE 'PE%' ORDER BY CAST(SUBSTRING(pur_id, 3) AS UNSIGNED) DESC LIMIT 1"
            Using cmd As New MySqlCommand(query, conn)
                Dim res As Object = cmd.ExecuteScalar()
                If res IsNot Nothing AndAlso Not IsDBNull(res) Then
                    Dim lastInvNo As String = res.ToString()
                    Dim lastNo As Integer = 0
                    If lastInvNo.Length > 2 AndAlso Integer.TryParse(lastInvNo.Substring(2), lastNo) Then
                        nextNo = lastNo + 1
                    End If
                End If
            End Using
        Catch ex As Exception
            ' Silent fallback
        Finally
            If Not wasOpen AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
        Return "PE" & nextNo.ToString("D4")
    End Function

    Private Function IsInvoiceNumberDuplicate(invNo As String, supplierId As Integer) As Boolean
        If String.IsNullOrWhiteSpace(invNo) OrElse supplierId <= 0 Then Return False
        Dim wasOpen As Boolean = (conn.State = ConnectionState.Open)
        Try
            If Not wasOpen Then conn.Open()
            Dim query As String = "SELECT COUNT(*) FROM purchasing WHERE pur_id = @inv AND supplier_id = @sup"
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@inv", invNo.Trim())
                cmd.Parameters.AddWithValue("@sup", supplierId)
                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
                Return count > 0
            End Using
        Catch ex As Exception
            Return False
        Finally
            If Not wasOpen AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Function

    Private Function IsPRNumberDuplicate(prNo As String) As Boolean
        If String.IsNullOrWhiteSpace(prNo) Then Return False
        Dim wasOpen As Boolean = (conn.State = ConnectionState.Open)
        Try
            If Not wasOpen Then conn.Open()
            Dim query As String = "SELECT COUNT(*) FROM purchase_request WHERE request_id = @pr"
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@pr", prNo.Trim())
                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
                Return count > 0
            End Using
        Catch ex As Exception
            Return False
        Finally
            If Not wasOpen AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Function

    Private Function GeneratePRNumber() As String
        Dim nextNo As Integer = 1
        Dim wasOpen As Boolean = (conn.State = ConnectionState.Open)
        Try
            If Not wasOpen Then conn.Open()
            Dim query As String = "SELECT request_id FROM purchase_request WHERE request_id LIKE 'PR%' ORDER BY CAST(SUBSTRING(request_id, 3) AS UNSIGNED) DESC LIMIT 1"
            Using cmd As New MySqlCommand(query, conn)
                Dim res As Object = cmd.ExecuteScalar()
                If res IsNot Nothing AndAlso Not IsDBNull(res) Then
                    Dim lastPRNo As String = res.ToString()
                    Dim lastNo As Integer = 0
                    If lastPRNo.Length > 2 AndAlso Integer.TryParse(lastPRNo.Substring(2), lastNo) Then
                        nextNo = lastNo + 1
                    End If
                End If
            End Using
        Catch ex As Exception
            ' Silent fallback
        Finally
            If Not wasOpen AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
        Return "PR" & nextNo.ToString("D4")
    End Function

    Private Sub TogglePRMode(checked As Boolean)
        If checked Then
            originalInvNo = InvNoTxt.Text.Trim()
            lblInvNo.Text = "Po No:"
            InvNoTxt.Text = GeneratePRNumber()
            InvNoTxt.BackColor = Color.Orange

            ' Disable payment fields for PR mode
            ComboBox1.Enabled = False
            txPaymentMethod.Enabled = False
            AmountTextBox2.Enabled = False
        Else
            lblInvNo.Text = "Inv No:"
            If Not String.IsNullOrEmpty(originalInvNo) Then
                InvNoTxt.Text = originalInvNo
                originalInvNo = ""
            ElseIf SupplierNameTxt.Text.Trim().ToLower().Contains("banet") Then
                InvNoTxt.Text = GeneratePEInvoiceNumber()
            Else
                InvNoTxt.Text = ""
            End If
            InvNoTxt.BackColor = SupplierNameTxt.BackColor

            ' Re-enable payment fields
            ComboBox1.Enabled = True
            txPaymentMethod.Enabled = If(ComboBox1.Text = "Cash", True, False)
            AmountTextBox2.Enabled = True
        End If
    End Sub

    Private Sub chkPRMode_Click(sender As Object, e As EventArgs) Handles chkPRMode.Click
        isPRModeChecked = Not isPRModeChecked
        chkPRMode.Invalidate()
        TogglePRMode(isPRModeChecked)

        If isPRModeChecked Then
            SupplierNameTxt.Focus()
            SupplierNameTxt.SelectAll()
        End If
    End Sub

    Private Sub chkPRMode_Paint(sender As Object, e As PaintEventArgs) Handles chkPRMode.Paint
        Dim g As Graphics = e.Graphics
        g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias

        ' Box size: 21x21 centered in 28x28 (starting at 3, 3)
        Dim rect As New Rectangle(3, 3, 21, 21)

        ' Fill with White
        Using brushBg As New SolidBrush(Color.White)
            g.FillRectangle(brushBg, rect)
        End Using

        ' Draw a nice bold dark border
        Using penBorder As New Pen(Color.FromArgb(64, 64, 64), 2)
            g.DrawRectangle(penBorder, rect)
        End Using

        ' If checked, draw a beautiful bold checkmark in black
        If isPRModeChecked Then
            Using penCheck As New Pen(Color.Black, 3)
                ' Checkmark lines in 21x21 box (centered at 3,3)
                Dim pt1 As New Point(7, 13)
                Dim pt2 As New Point(12, 18)
                Dim pt3 As New Point(18, 8)
                g.DrawLine(penCheck, pt1, pt2)
                g.DrawLine(penCheck, pt2, pt3)
            End Using
        End If
    End Sub

    Private Sub LoadSupplierInvoices(supplierName As String, Optional highlightInvoiceNo As String = "")
        Try
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Open()

            ' Fetch and update selectedSupplierID by supplierName to ensure correct parameter bindings when double-clicking invoices
            Dim idCmd As New MySqlCommand("SELECT id FROM supplier WHERE name = @name LIMIT 1", conn)
            idCmd.Parameters.AddWithValue("@name", supplierName)
            Dim idObj = idCmd.ExecuteScalar()
            If idObj IsNot Nothing AndAlso Not DBNull.Value.Equals(idObj) Then
                selectedSupplierID = Convert.ToInt32(idObj)
            End If

            Dim table As New DataTable()
            ' Query unique invoices for this supplier from items_stock (most reliable for history)
            Dim query As String = "SELECT ist.inv_no as 'Invoice No', SUM(ist.qty_purchased) as 'Total Qty', MAX(p.paid_amount) as 'Paid Amount', MAX(p.sub_total) as 'Total Amount', MAX(p.cost) as 'Grand Total', MAX(COALESCE(p.pur_date, ist.date)) as 'Pur_Date', MAX(COALESCE(p.date, ist.date)) as 'Date' " &
                           "FROM items_stock ist " &
                           "JOIN supplier s ON ist.supplier_id = s.id " &
                           "LEFT JOIN purchasing p ON ist.inv_no = p.pur_id AND ist.supplier_id = p.supplier_id " &
                           "WHERE s.name = @sup AND (p.status IS NULL OR p.status <> 'Print_Only') " &
                           "GROUP BY ist.inv_no " &
                           "ORDER BY ist.inv_no ASC"
            Dim adapter As New MySqlDataAdapter(query, conn)
            adapter.SelectCommand.Parameters.AddWithValue("@sup", supplierName)
            adapter.Fill(table)

            Dim dv As New DataView(table)
            dv.Sort = "[Invoice No] ASC"
            DataGridView1.DataSource = dv

            setup_grid_style(DataGridView1)
            DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            conn.Close()

            ' Pre-populate InvNoTxt with empty text or clear it for filtering
            InvNoTxt.Clear()

            ' Highlight the saved row if specified
            If Not String.IsNullOrEmpty(highlightInvoiceNo) Then
                DataGridView1.ClearSelection()
                For Each row As DataGridViewRow In DataGridView1.Rows
                    If Convert.ToString(row.Cells("Invoice No").Value) = highlightInvoiceNo Then
                        row.Selected = True
                        DataGridView1.CurrentCell = row.Cells(0) ' Set current cell to ensure highlight and scrolling
                        Exit For
                    End If
                Next
            End If

            lastSelectedSupName = supplierName
            historyLevel = 2
            sdetailsbtn.Text = "Back"
            If InvNoTxt IsNot Nothing Then
                If isHistoryFromSave Then
                    InvNoTxt.BackColor = SupplierNameTxt.BackColor
                Else
                    InvNoTxt.BackColor = Color.FromArgb(200, 230, 255)
                End If
                InvNoTxt.Focus()
            End If
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then conn.Close()
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub LoadInvoiceItems(invoiceNo As String, supplierId As Integer)
        Try
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Open()
            Dim table As New DataTable()
            ' Select all columns to match the main purchase entry grid's style
            Dim query As String = "SELECT i.item_id as 'Item ID', i.description as 'Description', " &
                                 "i.item_cost as 'Cost', i.selling_price as 'Sell Price', " &
                                 "i.whole_selling_price as 'W. Price', i.retail_selling_price as 'R. Price', " &
                                 "i.avg_cost as 'Avg Cost', i.qty_purchased as 'Qty', " &
                                 "l.location_name as 'Location', " &
                                 "i.discount as 'Disc', i.amount as 'Amount' " &
                                 "FROM items_stock i " &
                                 "LEFT JOIN location l ON i.location_id = l.id " &
                                 "WHERE i.inv_no = @inv AND i.supplier_id = @sup"
            Dim adapter As New MySqlDataAdapter(query, conn)
            adapter.SelectCommand.Parameters.AddWithValue("@inv", invoiceNo)
            adapter.SelectCommand.Parameters.AddWithValue("@sup", supplierId)
            adapter.Fill(table)

            ' Display in the history overlay grid instead of the main grid
            DataGridView1.DataSource = table
            setup_grid_style(DataGridView1)
            DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None

            ' Set consistent column widths and order to match the main grid (PurchGrideView)
            If DataGridView1.Columns.Count >= 11 Then
                ' Order: ID, Desc, Cost, Sell, W. Price, R. Price, Avg Cost, Qty, Location, Disc, Amount
                DataGridView1.Columns(0).Width = 120 ' Item ID (Increased width)
                DataGridView1.Columns(1).Width = 400 ' Description
                DataGridView1.Columns(2).Width = 100 ' Cost
                DataGridView1.Columns(3).Width = 140 ' Sell Price
                DataGridView1.Columns(4).Width = 130 ' W. Price
                DataGridView1.Columns(5).Width = 130 ' R. Price
                DataGridView1.Columns(6).Width = 140 ' Avg Cost
                DataGridView1.Columns(7).Width = 75  ' Qty
                DataGridView1.Columns(8).Width = 130 ' Location
                DataGridView1.Columns(9).Width = 75  ' Disc
                DataGridView1.Columns(10).Width = 115 ' Amount
            End If

            conn.Close()

            lastSelectedInvNo = invoiceNo
            historyLevel = 3
            DataGridView1.Visible = True
            PurchGrideView.Visible = False
            sdetailsbtn.Text = "Back"
            AddProcessLog("History Items Viewed", "Inv: " & invoiceNo)
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then conn.Close()
            MessageBox.Show(ex.Message)
        End Try
    End Sub


    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        If e.RowIndex >= 0 Then
            ' Only update credit display when viewing the Suppliers List (Level 1)
            If historyLevel = 1 Then
                ' Column 2 is 'Due Debit amount' as per creditSeach query
                If DataGridView1.Columns.Count > 2 Then
                    Dim val As Object = DataGridView1.Rows(e.RowIndex).Cells(2).Value
                    If val IsNot Nothing AndAlso IsNumeric(val) Then
                        sudebitt.Text = CDbl(val).ToString("F2")
                    End If
                End If

                ' Update Debit Limit/Period from grid (Level 1)
                If DataGridView1.Columns.Count > 4 Then
                    ' Column 3: Debit Limit, Column 4: Debit Period
                    Dim limitVal As Object = DataGridView1.Rows(e.RowIndex).Cells(3).Value
                    Dim periodVal As Object = DataGridView1.Rows(e.RowIndex).Cells(4).Value

                    _selectedDebitLimit = If(limitVal Is DBNull.Value, 0, Convert.ToDouble(limitVal))
                    txtDebitLimitDisplay.Text = _selectedDebitLimit.ToString("F2")

                    If periodVal IsNot DBNull.Value Then
                        _selectedDebitPeriod = Convert.ToDateTime(periodVal)
                        dtpDebitPeriodDisplay.Value = _selectedDebitPeriod
                    Else
                        _selectedDebitPeriod = DateTime.Today
                        dtpDebitPeriodDisplay.Value = DateTime.Today
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub DataGridView1_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellDoubleClick
        If e.RowIndex >= 0 Then
            Try
                If historyLevel = 1 Then
                    ' Level 1: Viewing Supplier Credit list (from supplicer_credit)
                    ' Double-click to see all Invoices for this specific Supplier
                    Dim supName As String = Convert.ToString(DataGridView1.Rows(e.RowIndex).Cells(0).Value)
                    If Not String.IsNullOrEmpty(supName) Then
                        ' Sync the textbox name to make sure backtracking searches stay focused on this supplier
                        SupplierNameTxt.Text = supName
                        LoadSupplierInvoices(supName)
                    End If
                ElseIf historyLevel = 2 Then
                    ' Level 2: Viewing Invoice list (from items_stock summary)
                    ' Double-click to see the specific Items in that Invoice
                    Dim invNo As String = Convert.ToString(DataGridView1.Rows(e.RowIndex).Cells(0).Value)
                    If Not String.IsNullOrEmpty(invNo) Then
                        LoadInvoiceItems(invNo, selectedSupplierID)
                    End If
                End If
            Catch ex As Exception
                MessageBox.Show("Error loading history: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub load_data()
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()
            Dim table As New DataTable()
            Dim query As String = "SELECT i.id, i.description, " &
                "IFNULL((SELECT ist.item_cost FROM items_stock ist WHERE ist.item_id = i.id ORDER BY ist.date DESC, ist.id DESC LIMIT 1), i.item_cost) as item_cost, " &
                "IFNULL((SELECT ist.avg_cost FROM items_stock ist WHERE ist.item_id = i.id ORDER BY ist.date DESC, ist.id DESC LIMIT 1), i.avg_cost) as avg_cost, " &
                "IFNULL((SELECT ist.selling_price FROM items_stock ist WHERE ist.item_id = i.id ORDER BY ist.date DESC, ist.id DESC LIMIT 1), i.selling_price) as selling_price, " &
                "IFNULL((SELECT ist.whole_selling_price FROM items_stock ist WHERE ist.item_id = i.id ORDER BY ist.date DESC, ist.id DESC LIMIT 1), i.whole_selling_price) as wprice, " &
                "IFNULL((SELECT ist.retail_selling_price FROM items_stock ist WHERE ist.item_id = i.id ORDER BY ist.date DESC, ist.id DESC LIMIT 1), i.retail_selling_price) as rprice, " &
                "IFNULL((SELECT SUM(st_qty) FROM items_stock WHERE item_id = i.id), 0) as st_qty " &
                "FROM items i ORDER BY i.id ASC"
            Dim adapter As New MySqlDataAdapter(query, conn)
            adapter.Fill(table)
            ItemsShow.DataSource = table
            SetItemsShowColumnWidths()
            conn.Close()
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub SetItemsShowColumnWidths()
        If ItemsShow.Columns.Count > 1 Then
            setup_grid_style(ItemsShow)
            ItemsShow.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None ' Enable horizontal scrolling

            ' Header texts and precise widths to ensure everything is visible via scrolling
            ItemsShow.Columns(0).HeaderText = "Item ID"
            ItemsShow.Columns(0).Width = 100
            ItemsShow.Columns(1).HeaderText = "Description"
            ItemsShow.Columns(1).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill

            If ItemsShow.Columns.Count > 2 Then
                ItemsShow.Columns(2).HeaderText = "Cost Price"
                ItemsShow.Columns(2).Width = 120
                ItemsShow.Columns(2).Visible = True
                ItemsShow.Columns(2).DefaultCellStyle.Format = "N3"
            End If
            If ItemsShow.Columns.Count > 3 Then
                ItemsShow.Columns(3).HeaderText = "Avg Cost"
                ItemsShow.Columns(3).Width = 120
                ItemsShow.Columns(3).Visible = True
                ItemsShow.Columns(3).DefaultCellStyle.Format = "N3"
            End If
            If ItemsShow.Columns.Count > 4 Then
                ItemsShow.Columns(4).HeaderText = "Sell Price"
                ItemsShow.Columns(4).Width = 120
                ItemsShow.Columns(4).Visible = True
                ItemsShow.Columns(4).DefaultCellStyle.Format = "N3"
            End If
            If ItemsShow.Columns.Count > 5 Then
                ItemsShow.Columns(5).HeaderText = "W. Price"
                ItemsShow.Columns(5).Width = 110
                ItemsShow.Columns(5).Visible = True
                ItemsShow.Columns(5).DefaultCellStyle.Format = "N3"
            End If
            If ItemsShow.Columns.Count > 6 Then
                ItemsShow.Columns(6).HeaderText = "R. Price"
                ItemsShow.Columns(6).Width = 110
                ItemsShow.Columns(6).Visible = True
                ItemsShow.Columns(6).DefaultCellStyle.Format = "N3"
            End If
            If ItemsShow.Columns.Count > 7 Then
                ItemsShow.Columns(7).HeaderText = "Stock Qty"
                ItemsShow.Columns(7).Width = 100
                ItemsShow.Columns(7).Visible = True
            End If
        End If
    End Sub

    Private Sub GenerateInvoiceNo()
        ' Manual Entry Mode: Auto-generation disabled as per user request
        ' InvNoTxt.Text = "" 
        ' InvNoTxt.ReadOnly = False
    End Sub

    Private Sub PurchEntLoad()
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()

            ' Ensure draft_user and pc_name columns exist in items_stock_tempary for multi-machine concurrency
            Try
                Using cmdAlter1 As New MySqlCommand("ALTER TABLE items_stock_tempary ADD COLUMN draft_user VARCHAR(100) NULL", conn)
                    cmdAlter1.ExecuteNonQuery()
                End Using
            Catch : End Try
            Try
                Using cmdAlter2 As New MySqlCommand("ALTER TABLE items_stock_tempary ADD COLUMN pc_name VARCHAR(100) NULL", conn)
                    cmdAlter2.ExecuteNonQuery()
                End Using
            Catch : End Try

            ' Recover from temporary table if form was closed unexpectedly (isolated per user/machine)
            If String.IsNullOrWhiteSpace(InvNoTxt.Text) Then
                Dim checkCmd As New MySqlCommand("SELECT inv_no FROM items_stock_tempary WHERE draft_user = @uname OR pc_name = @pc LIMIT 1", conn)
                checkCmd.Parameters.AddWithValue("@uname", Module1.UserName)
                checkCmd.Parameters.AddWithValue("@pc", Environment.MachineName)
                Dim recoveredInv = checkCmd.ExecuteScalar()
                If recoveredInv IsNot Nothing AndAlso Not DBNull.Value.Equals(recoveredInv) Then
                    InvNoTxt.Text = Convert.ToString(recoveredInv)
                End If
            End If

            Dim bsource As New BindingSource
            Dim table As New DataTable()
            ' Join with supplier to get the name for the selection logic
            Dim query As String = "SELECT t.inv_no, t.item_id, t.description, t.st_qty, t.item_cost, t.discount, t.amount, t.id, t.location_id, t.selling_price, t.whole_selling_price, t.retail_selling_price, t.avg_cost, s.name as supplier_name, t.supplier_id, s.tel_no, l.location_name " &
                                 "FROM items_stock_tempary t " &
                                 "LEFT JOIN supplier s ON t.supplier_id = s.id " &
                                 "LEFT JOIN location l ON t.location_id = l.id " &
                                 "WHERE t.inv_no = @inv AND (t.draft_user = @uname OR t.pc_name = @pc) ORDER BY t.date DESC"
            Dim adapter As New MySqlDataAdapter(query, conn)
            adapter.SelectCommand.Parameters.AddWithValue("@inv", InvNoTxt.Text)
            adapter.SelectCommand.Parameters.AddWithValue("@uname", Module1.UserName)
            adapter.SelectCommand.Parameters.AddWithValue("@pc", Environment.MachineName)

            adapter.Fill(table)
            bsource.DataSource = table
            PurchGrideView.DataSource = table

            ' Set all column headers, widths, visibility and ORDER in one block
            If PurchGrideView.Columns.Count >= 13 Then
                setup_grid_style(PurchGrideView)

                ' 1. Hide unwanted columns first
                PurchGrideView.Columns(0).Visible = False ' Invoice
                PurchGrideView.Columns(7).Visible = False ' ID
                PurchGrideView.Columns(8).Visible = False ' Location ID

                ' 2. Set Headers and Widths (Capitalized)
                PurchGrideView.Columns(1).HeaderText = "Item ID" : PurchGrideView.Columns(1).Width = 120
                PurchGrideView.Columns(2).HeaderText = "Description" : PurchGrideView.Columns(2).Width = 400
                PurchGrideView.Columns(4).HeaderText = "Cost" : PurchGrideView.Columns(4).Width = 100 : PurchGrideView.Columns(4).DefaultCellStyle.Format = "N3"
                PurchGrideView.Columns(9).HeaderText = "Sell Price" : PurchGrideView.Columns(9).Width = 140 : PurchGrideView.Columns(9).DefaultCellStyle.Format = "N3"
                PurchGrideView.Columns(10).HeaderText = "W. Price" : PurchGrideView.Columns(10).Width = 130 : PurchGrideView.Columns(10).DefaultCellStyle.Format = "N3"
                PurchGrideView.Columns(11).HeaderText = "R. Price" : PurchGrideView.Columns(11).Width = 130 : PurchGrideView.Columns(11).DefaultCellStyle.Format = "N3"
                PurchGrideView.Columns(12).HeaderText = "Avg Cost" : PurchGrideView.Columns(12).Width = 140 : PurchGrideView.Columns(12).DefaultCellStyle.Format = "N3"
                PurchGrideView.Columns(3).HeaderText = "Qty" : PurchGrideView.Columns(3).Width = 75
                PurchGrideView.Columns(5).HeaderText = "Disc" : PurchGrideView.Columns(5).Width = 75
                PurchGrideView.Columns(6).HeaderText = "Amount" : PurchGrideView.Columns(6).Width = 115 : PurchGrideView.Columns(6).DefaultCellStyle.Format = "N3"

                If PurchGrideView.Columns.Count > 16 Then
                    PurchGrideView.Columns(16).HeaderText = "Location" : PurchGrideView.Columns(16).Width = 130
                End If

                ' 1.5 Hide Supplier Name, ID & Phone from grid (only needed for selection logic)
                If PurchGrideView.Columns.Count > 13 Then
                    PurchGrideView.Columns(13).Visible = False
                    If PurchGrideView.Columns.Count > 15 Then
                        PurchGrideView.Columns(14).Visible = False
                        PurchGrideView.Columns(15).Visible = False
                    End If
                End If

                ' 3. Set Display Order (The sequence user requested)
                PurchGrideView.Columns(1).DisplayIndex = 0  ' Item ID
                PurchGrideView.Columns(2).DisplayIndex = 1  ' Description
                PurchGrideView.Columns(4).DisplayIndex = 2  ' Cost
                PurchGrideView.Columns(9).DisplayIndex = 3  ' Sell Price
                PurchGrideView.Columns(10).DisplayIndex = 4 ' W.Price
                PurchGrideView.Columns(11).DisplayIndex = 5 ' R.Price
                PurchGrideView.Columns(12).DisplayIndex = 6 ' Avg Cost
                PurchGrideView.Columns(3).DisplayIndex = 7  ' Qty
                If PurchGrideView.Columns.Count > 16 Then
                    PurchGrideView.Columns(16).DisplayIndex = 8 ' Location
                    PurchGrideView.Columns(5).DisplayIndex = 9  ' Disc
                    PurchGrideView.Columns(6).DisplayIndex = 10 ' Amount
                Else
                    PurchGrideView.Columns(5).DisplayIndex = 8  ' Disc
                    PurchGrideView.Columns(6).DisplayIndex = 9  ' Amount
                End If
            End If

            conn.Close()
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub SupplicerShow()
        conn.Open()

        Dim bsource As New BindingSource
        Dim table As New DataTable()
        Dim adapter As New MySqlDataAdapter("SELECT id, name, tel_no, debit_limit, debit_period FROM supplier ORDER BY name ASC", conn)
        adapter.Fill(table)
        bsource.DataSource = table
        SupplicerGrideView.DataSource = table
        SetSupplierGrideViewLayout()
        conn.Close()

    End Sub
    Private Sub SupplierNameTxt_TextChanged(sender As Object, e As EventArgs) Handles SupplierNameTxt.TextChanged
        SearchSuppliers()
    End Sub

    Private Sub TelNoTxt_TextChanged(sender As Object, e As EventArgs) Handles TelNoTxt.TextChanged
        SearchSuppliers()
    End Sub

    Private Sub SearchSuppliers()
        ' Reset limits immediately when search text changes (supplier is effectively "removed" or "changed")
        selectedSupplierID = 0
        _selectedDebitLimit = 0
        _selectedDebitPeriod = DateTime.Today
        txtDebitLimitDisplay.Text = "0.00"
        dtpDebitPeriodDisplay.Value = DateTime.Today
        creditAmtTxt.Text = "0.00"

        Try
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Open()
            Dim table As New DataTable()
            ' Match columns used in SupplicerGrideView selection logic
            Dim adapter As New MySqlDataAdapter("SELECT id, name, tel_no, debit_limit, debit_period FROM supplier ORDER BY name ASC", conn)
            adapter.Fill(table)
            Dim dv As New DataView(table)

            Dim filter As String = ""
            ' Filter by Name (Escaped to handle brackets like in ABNAR COMPANY [PVT] LTD)
            If Not String.IsNullOrEmpty(SupplierNameTxt.Text) Then
                filter = String.Format("name Like '{0}%'", EscapeRowFilter(SupplierNameTxt.Text).Replace("'", "''"))
            End If

            ' Filter by Telephone
            If Not String.IsNullOrEmpty(TelNoTxt.Text) Then
                Dim telFilter As String = String.Format("tel_no Like '{0}%'", EscapeRowFilter(TelNoTxt.Text).Replace("'", "''"))
                If filter <> "" Then
                    filter &= " AND " & telFilter
                Else
                    filter = telFilter
                End If
            End If

            dv.RowFilter = filter
            SupplicerGrideView.DataSource = dv
            SetSupplierGrideViewLayout()
            conn.Close()

            ' Sycn with the Supplier Details history list
            creditSeach()

            ' Visibility logic
            If historyLevel > 0 Then
                SupplicerGrideView.Visible = False
            ElseIf SupplierNameTxt.Text <> "" OrElse TelNoTxt.Text <> "" Then
                SupplicerGrideView.BringToFront()
                SupplicerGrideView.Visible = True
            Else
                SupplicerGrideView.Visible = False
                ' Reset limits if no supplier is selected or being searched
                selectedSupplierID = 0
                _selectedDebitLimit = 0
                _selectedDebitPeriod = Date.MaxValue
                txtDebitLimitDisplay.Text = "0.00"
                dtpDebitPeriodDisplay.Value = Date.Today.AddYears(10)
                sudebitt.Text = "0.00"
            End If
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub TelNoTxt_Leave(sender As Object, e As EventArgs) Handles TelNoTxt.Leave
        If Not SupplicerGrideView.Focused Then
            SupplicerGrideView.Visible = False
        End If
    End Sub

    Private Sub InvNoTxt_Click(sender As Object, e As EventArgs) Handles InvNoTxt.Click, InvNoTxt.Enter
        If historyLevel > 0 AndAlso historyLevel <> 2 Then
            ResetToNormalView()
        End If
    End Sub

    Private Sub InvNoTxt_KeyDown(sender As Object, e As KeyEventArgs) Handles InvNoTxt.KeyDown
        If historyLevel = 2 Then
            If e.KeyCode = Keys.Enter Then
                e.SuppressKeyPress = True
            End If
            Return
        End If

        If e.KeyCode = Keys.Enter Then
            If Not e.Shift Then
                If isPRModeChecked Then
                    If IsPRNumberDuplicate(InvNoTxt.Text) Then
                        MessageBox.Show("This P/O Number already exists! Please enter a unique P/O Number.", "Duplicate P/O Number", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        InvNoTxt.Focus()
                        InvNoTxt.SelectAll()
                    Else
                        SupplierNameTxt.Focus()
                        SupplierNameTxt.SelectAll()
                    End If
                ElseIf SupplierNameTxt.Text.Trim().ToLower().Contains("banet") Then
                    IT_CodeTextBox.Focus()
                    IT_CodeTextBox.SelectAll()
                Else
                    SupplierNameTxt.Focus()
                    SupplierNameTxt.SelectAll()
                End If
            End If
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub InvNoTxt_Leave(sender As Object, e As EventArgs) Handles InvNoTxt.Leave
        If historyLevel = 2 Then
            Return
        End If

        Dim cleanInv As String = InvNoTxt.Text.Trim()
        If Not String.IsNullOrEmpty(cleanInv) Then
            If isPRModeChecked Then
                If IsPRNumberDuplicate(cleanInv) Then
                    MessageBox.Show("This P/O Number already exists! Please enter a unique P/O Number.", "Duplicate P/O Number", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    InvNoTxt.Focus()
                    InvNoTxt.SelectAll()
                End If
            ElseIf selectedSupplierID > 0 Then
                If IsInvoiceNumberDuplicate(cleanInv, selectedSupplierID) Then
                    MessageBox.Show("This invoice number already exists for " & SupplierNameTxt.Text & "! Please enter a unique invoice number.", "Duplicate Invoice", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    InvNoTxt.Focus()
                    InvNoTxt.SelectAll()
                End If
            End If
        End If
    End Sub

    Private Sub InvNoTxt_TextChanged(sender As Object, e As EventArgs) Handles InvNoTxt.TextChanged
        Dim newInv As String = InvNoTxt.Text.Trim()

        If historyLevel = 2 AndAlso isHistoryFromSave Then
            historyLevel = 0
            isHistoryFromSave = False
            currentActiveInvNo = newInv
            DataGridView1.Visible = False
            PurchGrideView.Visible = True
            sdetailsbtn.Text = "Supplier Details"
            If InvNoTxt IsNot Nothing Then InvNoTxt.BackColor = SupplierNameTxt.BackColor
        End If

        If historyLevel = 0 Then
            ' Update temp items when invoice number is edited programmatically or manually by user
            If Not String.IsNullOrEmpty(currentActiveInvNo) AndAlso Not String.IsNullOrEmpty(newInv) AndAlso currentActiveInvNo <> newInv Then
                Try
                    If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()
                    Dim updateSql As String = "UPDATE items_stock_tempary SET inv_no = @newInv, id = CONCAT(@newInv, '_', item_id, '_', @uname) WHERE inv_no = @oldInv AND (draft_user = @uname OR pc_name = @pc)"
                    Using cmdUpdate As New MySqlCommand(updateSql, MySqlConn)
                        cmdUpdate.Parameters.AddWithValue("@newInv", newInv)
                        cmdUpdate.Parameters.AddWithValue("@oldInv", currentActiveInvNo)
                        cmdUpdate.Parameters.AddWithValue("@uname", Module1.UserName)
                        cmdUpdate.Parameters.AddWithValue("@pc", Environment.MachineName)
                        cmdUpdate.ExecuteNonQuery()
                    End Using
                    MySqlConn.Close()

                    currentActiveInvNo = newInv
                    PurchEntLoad()
                Catch ex As Exception
                    If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
                End Try
            Else
                currentActiveInvNo = newInv
            End If
        End If

        If historyLevel = 2 AndAlso DataGridView1.DataSource IsNot Nothing Then
            Try
                Dim dv As DataView = Nothing
                If TypeOf DataGridView1.DataSource Is DataView Then
                    dv = CType(DataGridView1.DataSource, DataView)
                ElseIf TypeOf DataGridView1.DataSource Is DataTable Then
                    Dim dt As DataTable = CType(DataGridView1.DataSource, DataTable)
                    dv = dt.DefaultView
                    DataGridView1.DataSource = dv
                End If

                If dv IsNot Nothing Then
                    Dim filterText As String = InvNoTxt.Text.Trim()
                    If String.IsNullOrEmpty(filterText) Then
                        dv.RowFilter = ""
                    Else
                        filterText = filterText.Replace("'", "''")
                        ' Starts-with constraint: match starting characters only
                        dv.RowFilter = String.Format("[Invoice No] LIKE '{0}%'", filterText)
                    End If
                    dv.Sort = "[Invoice No] ASC"
                End If
            Catch ex As Exception
                ' Silent fail
            End Try
        End If
    End Sub

    Private Sub SetSupplierGrideViewLayout()
        If SupplicerGrideView.Columns.Count >= 3 Then
            setup_grid_style(SupplicerGrideView)

            ' Indices: 0=id, 1=name, 2=tel_no
            SupplicerGrideView.Columns(0).Visible = False

            ' Align Name with SupplierNameTxt (width 280)
            SupplicerGrideView.Columns(1).HeaderText = "Supplier Name"
            SupplicerGrideView.Columns(1).Width = 280

            ' Align Tel No with TelNoTxt (width 250)
            SupplicerGrideView.Columns(2).HeaderText = "Telephone No"
            SupplicerGrideView.Columns(2).Width = 245

            ' Hide Limit/Period from search dropdown as it's too wide, but they are in the data source
            If SupplicerGrideView.Columns.Count > 3 Then SupplicerGrideView.Columns(3).Visible = False
            If SupplicerGrideView.Columns.Count > 4 Then SupplicerGrideView.Columns(4).Visible = False

            SupplicerGrideView.Width = 530
            SupplicerGrideView.Height = 400
        End If
    End Sub

    Private Sub SupplierNameTxt_Enter(sender As Object, e As EventArgs) Handles SupplierNameTxt.Enter
        If historyLevel = 0 AndAlso SupplierNameTxt.Text <> "" Then
            SupplicerGrideView.BringToFront()
            SupplicerGrideView.Visible = True
        End If
    End Sub

    Private Sub SupplierNameTxt_KeyDown(sender As Object, e As KeyEventArgs) Handles SupplierNameTxt.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                InvNoTxt.Focus()
                InvNoTxt.SelectAll()
            Else
                If SupplicerGrideView.Rows.Count > 0 Then
                    SelectSupplier(0)
                    e.SuppressKeyPress = True
                ElseIf SupplierNameTxt.Text <> "" Then
                    ' No supplier found - offer to create new
                    Dim result As DialogResult = MessageBox.Show("Supplier not found. Would you like to create a new supplier?", "New Supplier", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    If result = DialogResult.Yes Then
                        Dim frm As New Suplier()
                        frm.NameTxt.Text = SupplierNameTxt.Text
                        frm.ShowDialog()
                        ' Refresh search after closing
                        SupplierNameTxt_TextChanged(Nothing, Nothing)
                    End If
                    e.SuppressKeyPress = True
                Else
                    TelNoTxt.Focus()
                    TelNoTxt.SelectAll()
                End If
            End If
            e.SuppressKeyPress = True
        ElseIf e.KeyCode = Keys.Up Then
            SupplicerGrideView.Focus()
        ElseIf e.KeyCode = Keys.Down Then
            SupplicerGrideView.Focus()
        End If
    End Sub

    Private Sub SupplierNameTxt_Leave(sender As Object, e As EventArgs) Handles SupplierNameTxt.Leave
        If Not SupplicerGrideView.Focused Then
            SupplicerGrideView.Visible = False
        End If

        If SupplierNameTxt.Text.Trim().ToLower().Contains("banet") Then
            If String.IsNullOrWhiteSpace(InvNoTxt.Text) OrElse Not InvNoTxt.Text.StartsWith("PE") Then
                InvNoTxt.Text = GeneratePEInvoiceNumber()
            Else
                If IsInvoiceNumberDuplicate(InvNoTxt.Text, selectedSupplierID) Then
                    MessageBox.Show("This invoice number already exists for " & SupplierNameTxt.Text & "! Please enter a unique invoice number.", "Duplicate Invoice", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    InvNoTxt.Focus()
                    InvNoTxt.SelectAll()
                End If
            End If
            InvNoTxt.ReadOnly = False
            ComboBox1.Text = "Cash"
        Else
            If InvNoTxt.Text.StartsWith("PE") Then
                InvNoTxt.Text = ""
            End If
            InvNoTxt.ReadOnly = False

            If Not String.IsNullOrWhiteSpace(InvNoTxt.Text) AndAlso selectedSupplierID > 0 Then
                If IsInvoiceNumberDuplicate(InvNoTxt.Text, selectedSupplierID) Then
                    MessageBox.Show("This invoice number already exists for " & SupplierNameTxt.Text & "! Please enter a unique invoice number.", "Duplicate Invoice", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    InvNoTxt.Focus()
                    InvNoTxt.SelectAll()
                End If
            End If
        End If
    End Sub

    Private Sub TelNoTxt_KeyDown(sender As Object, e As KeyEventArgs) Handles TelNoTxt.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                SupplierNameTxt.Focus()
                SupplierNameTxt.SelectAll()
            Else
                If SupplicerGrideView.Rows.Count > 0 Then
                    SelectSupplier(0)
                    e.SuppressKeyPress = True
                Else
                    IT_CodeTextBox.Focus()
                    IT_CodeTextBox.SelectAll()
                End If
            End If
            e.SuppressKeyPress = True
        ElseIf e.KeyCode = Keys.Up OrElse e.KeyCode = Keys.Down Then
            If SupplicerGrideView.Visible Then
                SupplicerGrideView.Focus()
            End If
        End If
    End Sub

    Private Sub PurchaEntry_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.F12 Then
            Button7.PerformClick() ' Save
        ElseIf e.KeyCode = Keys.F2 Then
            btnAddNew.PerformClick() ' Add New
        ElseIf e.KeyCode = Keys.F3 Then
            Button1.PerformClick() ' Edit
        ElseIf e.KeyCode = Keys.Delete Then
            ' If active control is an editable textbox, let it handle the Delete key normally
            If TypeOf Me.ActiveControl Is TextBox AndAlso Not DirectCast(Me.ActiveControl, TextBox).ReadOnly Then
                Return
            End If
            Button3.PerformClick() ' Delete
        End If
    End Sub

    Private Sub PurchaEntry_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.KeyPreview = True
        ' Initialize Manual Debit Context Menu
        cmsHistory.Items.Add(tsmiAddDebit)
        cmsHistory.Items.Add(tsmiAddGeneralDebit)
        DataGridView1.ContextMenuStrip = cmsHistory

        ' Adjust GroupBox5 layout programmatically to fit Print Request button
        Me.GroupBox5.Location = New Point(1012, 10)
        Me.GroupBox5.Size = New Size(223, 160)

        ' Adjust original buttons inside GroupBox5
        Me.Button2.Location = New Point(9, 15)
        Me.Button7.Location = New Point(115, 15)
        Me.Button1.Location = New Point(9, 62)
        Me.Button3.Location = New Point(115, 62)

        ' Initialize and add btnPrintRequest inside GroupBox5 programmatically
        btnPrintRequest.Text = "      Print Request"
        btnPrintRequest.BackColor = Color.Orange
        btnPrintRequest.ForeColor = Color.Black
        btnPrintRequest.FlatStyle = FlatStyle.Flat
        btnPrintRequest.FlatAppearance.BorderSize = 0
        btnPrintRequest.Font = New Font("Segoe UI", 10, FontStyle.Bold)
        btnPrintRequest.Size = New Size(205, 40)
        btnPrintRequest.Location = New Point(9, 110)
        btnPrintRequest.Cursor = Cursors.Hand
        Me.GroupBox5.Controls.Add(btnPrintRequest)

        ' Initialize and add chkPRMode INSIDE btnPrintRequest programmatically
        chkPRMode.Text = ""
        chkPRMode.Size = New Size(28, 28)
        chkPRMode.Location = New Point(10, 6)
        chkPRMode.BackColor = Color.Transparent
        chkPRMode.Cursor = Cursors.Hand
        btnPrintRequest.Controls.Add(chkPRMode)

        ' Initialize and add chkPrintOnly inside GroupBox7 programmatically above Total Amount
        chkPrintOnly.Text = "Print Only (No Stock/Balance)"
        chkPrintOnly.ForeColor = Color.White
        chkPrintOnly.Font = New Font("Segoe UI", 9.0, FontStyle.Bold)
        chkPrintOnly.Size = New Size(250, 25)
        chkPrintOnly.Location = New Point(12, 12)
        chkPrintOnly.Cursor = Cursors.Hand
        Me.GroupBox7.Controls.Add(chkPrintOnly)

        ' Self-install new tables if they do not exist
        Try
            If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()

            ' Header table
            Dim createHeaderSql As String = "CREATE TABLE IF NOT EXISTS purchase_request (" &
                "request_id VARCHAR(50) PRIMARY KEY, " &
                "supplier_id INT, " &
                "supplier_name VARCHAR(255), " &
                "items_qty INT, " &
                "total_amount DECIMAL(15, 2), " &
                "request_date DATETIME, " &
                "pur_date DATETIME, " &
                "status VARCHAR(50) DEFAULT 'Draft', " &
                "location_id INT" &
                ")"
            Using cmd As New MySqlCommand(createHeaderSql, MySqlConn)
                cmd.ExecuteNonQuery()
            End Using

            ' Safely alter table to add columns if they already exist
            Try
                Using cmdAlter As New MySqlCommand("ALTER TABLE purchase_request ADD COLUMN location_id INT", MySqlConn)
                    cmdAlter.ExecuteNonQuery()
                End Using
            Catch exAlter As Exception
            End Try

            Try
                Using cmdAlterRq As New MySqlCommand("ALTER TABLE purchase_request ADD COLUMN request_date DATETIME", MySqlConn)
                    cmdAlterRq.ExecuteNonQuery()
                End Using
            Catch exAlterRq As Exception
            End Try

            Try
                Using cmdAlterPur As New MySqlCommand("ALTER TABLE purchase_request ADD COLUMN pur_date DATETIME", MySqlConn)
                    cmdAlterPur.ExecuteNonQuery()
                End Using
            Catch exAlterPur As Exception
            End Try

            ' Details table
            Dim createItemsSql As String = "CREATE TABLE IF NOT EXISTS purchase_request_items (" &
                "id VARCHAR(50) PRIMARY KEY, " &
                "request_id VARCHAR(50), " &
                "item_id VARCHAR(50), " &
                "description VARCHAR(255), " &
                "qty DOUBLE, " &
                "item_cost DECIMAL(15, 2), " &
                "amount DECIMAL(15, 2), " &
                "location_id INT, " &
                "date DATETIME" &
                ")"
            Using cmd As New MySqlCommand(createItemsSql, MySqlConn)
                cmd.ExecuteNonQuery()
            End Using
            MySqlConn.Close()
        Catch exDb As Exception
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try

        PurchEntLoad()
        SupplicerShow()
        ' Load all items into ItemsShow on form open
        Try
            conn.Open()
            Dim table As New DataTable()
            Dim adapter As New MySqlDataAdapter("SELECT id, description, item_cost, avg_cost, selling_price, whole_selling_price AS wprice, retail_selling_price AS rprice, st_qty FROM items", conn)
            adapter.Fill(table)
            ItemsShow.DataSource = table
            SetItemsShowColumnWidths()
            conn.Close()
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try

        ItemsShow.Visible = False
        SupplicerGrideView.Visible = False
        DataGridView1.Visible = False
        dgvPaymentMethod.Visible = False
        TextBox6.ReadOnly = True ' Balance should be read-only by default
        sudebitt.Enabled = True ' Set to True so it looks normal
        sudebitt.ReadOnly = True ' Still read-only to prevent editing
        sudebitt.TabStop = False ' Prevent focus via Tab key

        ' Initialize supplier limits to default (Today's date)
        LoadSupplierLimits(0)

        ' Load Locations
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()
            Dim dtLoc As New DataTable()
            ' Load Locations directly from database
            Dim query As String = "SELECT id, location_name FROM location ORDER BY location_name"
            Dim adpLoc As New MySqlDataAdapter(query, conn)
            adpLoc.Fill(dtLoc)
            ComboBoxLocation.DataSource = dtLoc
            ComboBoxLocation.DisplayMember = "location_name"
            ComboBoxLocation.ValueMember = "id"

            ' Set 'Main Stock' (ID 1) as default
            ComboBoxLocation.SelectedValue = 1
            conn.Close()
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try

        ' Populate Payment Type from Database ENUM
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()
            ComboBox1.Items.Clear()
            Dim cmdEnum As New MySqlCommand("SHOW COLUMNS FROM purchasing LIKE 'pur_type'", conn)
            Dim drEnum As MySqlDataReader = cmdEnum.ExecuteReader()
            If drEnum.Read() Then
                ' COLUMN_TYPE is e.g. "enum('Credit','Cash','Cheque')"
                Dim enumStr As String = drEnum("Type").ToString()
                enumStr = enumStr.Replace("enum(", "").Replace(")", "").Replace("'", "")
                Dim enumValues() As String = enumStr.Split(","c)
                For i As Integer = 0 To enumValues.Length - 1
                    ComboBox1.Items.Add(enumValues(i).Trim())
                Next
            End If
            drEnum.Close()
            ComboBox1.SelectedIndex = -1

            ' Populate Supply Method from Database ENUM
            ComboBox2metho.Items.Clear()
            Dim cmdEnumMeth As New MySqlCommand("SHOW COLUMNS FROM purchasing LIKE 'pur_su_method'", conn)
            Dim drEnumMeth As MySqlDataReader = cmdEnumMeth.ExecuteReader()
            If drEnumMeth.Read() Then
                ' COLUMN_TYPE is e.g. "enum('Local','Import','selfproduct')"
                Dim enumStr As String = drEnumMeth("Type").ToString()
                enumStr = enumStr.Replace("enum(", "").Replace(")", "").Replace("'", "")
                Dim enumValues() As String = enumStr.Split(","c)
                For i As Integer = 0 To enumValues.Length - 1
                    ComboBox2metho.Items.Add(enumValues(i).Trim())
                Next
            End If
            drEnumMeth.Close()
            If ComboBox2metho.Items.Count > 0 Then ComboBox2metho.SelectedIndex = 0

            conn.Close()
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then conn.Close()
            ' Fallback if DB fetch fails
            ComboBox1.Items.Clear()
            ComboBox1.Items.Add("Credit")
            ComboBox1.Items.Add("Cash")
            ComboBox1.Items.Add("Cheque")
            ComboBox1.SelectedIndex = -1
        End Try

        ' Position the supplier dropdown below SupplierNameTxt
        Dim pos As Point = SupplierNameTxt.PointToScreen(New Point(0, SupplierNameTxt.Height))
        Dim formPos As Point = Me.PointToClient(pos)
        SupplicerGrideView.Location = New Point(formPos.X, formPos.Y)
        SupplicerGrideView.BringToFront()

        ' Position the item dropdown exactly below IT_CodeTextBox for perfect alignment
        ItemsShow.Location = New Point(11, 151) ' Aligns below price textbox row (GroupBox3 ends at Y=183)
        ItemsShow.Width = 1241 ' Aligns with AmountTextBox.Right
        ItemsShow.Height = 525
        ItemsShow.BringToFront()


        Dim check As Integer = PurchGrideView.Rows.Count - 1
        'This is for the check the colom and if it has data then inset into tedxtboxs'
        If check < 0 Then
            ' GenerateInvoiceNo() ' Disabled for manual entry
            InvNoTxt.Clear()
            InvNoTxt.ReadOnly = False
            LockHeader(False)
        Else
            InvNoTxt.Text = Convert.ToString(PurchGrideView.Rows(check).Cells(0).Value)
            If PurchGrideView.Columns.Contains("supplier_name") Then
                RemoveHandler SupplierNameTxt.TextChanged, AddressOf SupplierNameTxt_TextChanged
                SupplierNameTxt.Text = Convert.ToString(PurchGrideView.Rows(check).Cells("supplier_name").Value)

                If PurchGrideView.Columns.Contains("tel_no") Then
                    RemoveHandler TelNoTxt.TextChanged, AddressOf TelNoTxt_TextChanged

                    Dim rawSupId = PurchGrideView.Rows(check).Cells("supplier_id").Value
                    If rawSupId IsNot DBNull.Value AndAlso rawSupId IsNot Nothing Then
                        selectedSupplierID = Convert.ToInt32(rawSupId)
                    End If
                    TelNoTxt.Text = Convert.ToString(PurchGrideView.Rows(check).Cells("tel_no").Value)
                    LoadSupplierLimits(selectedSupplierID)

                    AddHandler TelNoTxt.TextChanged, AddressOf TelNoTxt_TextChanged
                End If

                AddHandler SupplierNameTxt.TextChanged, AddressOf SupplierNameTxt_TextChanged
            End If

            Dim netsum As Double = 0

            For j As Integer = 0 To PurchGrideView.Rows.Count() - 1 Step +1
                Dim val As Object = PurchGrideView.Rows(j).Cells(6).Value
                If val IsNot Nothing AndAlso IsNumeric(val) Then
                    netsum = netsum + CDbl(val)
                End If
            Next
            mgsumTxt.Text = netsum.ToString()

            ' Lock header if items exist
            If PurchGrideView.Rows.Count > 0 Then
                LockHeader(True)
            End If
        End If

        ' Initialize Payment Method states
        LoadBanks()
        UpdateChequeUI()
        If ComboBox1.Text = "Credit" OrElse ComboBox1.Text = "Cheque" Then
            txPaymentMethod.Enabled = False
        Else
            txPaymentMethod.Enabled = True
        End If

        ' Attach global click reset handlers for click-outside-to-close behavior
        AddHandler Me.Click, AddressOf GlobalReset_Click
        AttachGlobalResetHandlers(Me)
    End Sub

    Private Sub AttachGlobalResetHandlers(parent As Control)
        For Each ctrl As Control In parent.Controls
            ' Exclude buttons, grids, search boxes, and specific controls to prevent click reset on key actions
            If TypeOf ctrl Is Button OrElse TypeOf ctrl Is ButtonBase OrElse TypeOf ctrl Is DataGridView Then
                ' Skip these
            ElseIf ctrl IsNot InvNoTxt AndAlso ctrl IsNot DataGridView1 AndAlso ctrl IsNot sdetailsbtn AndAlso ctrl IsNot chkPRMode Then
                AddHandler ctrl.Click, AddressOf GlobalReset_Click
            End If
            ' Recursively attach to child controls
            If ctrl.HasChildren Then
                AttachGlobalResetHandlers(ctrl)
            End If
        Next
    End Sub

    Private Sub ResetToNormalView()
        DataGridView1.Visible = False
        PurchGrideView.Visible = True
        historyLevel = 0
        sdetailsbtn.Text = "Supplier Details"
        CalculateSummary()
        
        ' Clear selected supplier details safely
        RemoveHandler SupplierNameTxt.TextChanged, AddressOf SupplierNameTxt_TextChanged
        RemoveHandler TelNoTxt.TextChanged, AddressOf TelNoTxt_TextChanged
        
        SupplierNameTxt.Clear()
        TelNoTxt.Clear()
        selectedSupplierID = 0
        _selectedDebitLimit = 0
        _selectedDebitPeriod = Date.MaxValue
        txtDebitLimitDisplay.Text = "0.00"
        dtpDebitPeriodDisplay.Value = Date.Today
        sudebitt.Text = "0.00"
        
        AddHandler SupplierNameTxt.TextChanged, AddressOf SupplierNameTxt_TextChanged
        AddHandler TelNoTxt.TextChanged, AddressOf TelNoTxt_TextChanged

        LoadSupplierLimits(selectedSupplierID)
        InvNoTxt.Clear()
        If InvNoTxt IsNot Nothing Then InvNoTxt.BackColor = SupplierNameTxt.BackColor
    End Sub

    Private Sub GlobalReset_Click(sender As Object, e As EventArgs)
        isHistoryFromSave = False
        If historyLevel > 0 Then
            ResetToNormalView()
        End If
    End Sub

    Private Sub SupplicerGrideView_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles SupplicerGrideView.CellClick
        If e.RowIndex >= 0 Then
            SelectSupplier(e.RowIndex)
        End If
    End Sub

    Private Sub SupplicerGrideView_KeyDown(sender As Object, e As KeyEventArgs) Handles SupplicerGrideView.KeyDown
        If e.KeyCode = Keys.Enter Then
            SelectSupplier(SupplicerGrideView.CurrentRow.Index)
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub SelectSupplier(rowIndex As Integer)
        If rowIndex >= 0 AndAlso rowIndex < SupplicerGrideView.Rows.Count Then
            ' Capture all values FIRST before setting TextBoxes (TextChanged reloads the grid)
            Dim supID As String = Convert.ToString(SupplicerGrideView.Rows(rowIndex).Cells(0).Value)
            Dim supName As String = Convert.ToString(SupplicerGrideView.Rows(rowIndex).Cells(1).Value)
            Dim supTel As String = ""
            If SupplicerGrideView.Columns.Count > 2 Then
                supTel = Convert.ToString(SupplicerGrideView.Rows(rowIndex).Cells(2).Value)
            End If

            ' Temporarily remove handlers to prevent recursive trigger or unwanted searches
            RemoveHandler SupplierNameTxt.TextChanged, AddressOf SupplierNameTxt_TextChanged
            RemoveHandler TelNoTxt.TextChanged, AddressOf TelNoTxt_TextChanged

            ' Now safely set TextBox values
            selectedSupplierID = Val(supID)
            SupplierNameTxt.Text = supName
            TelNoTxt.Text = supTel

            ' Re-enable handlers
            AddHandler SupplierNameTxt.TextChanged, AddressOf SupplierNameTxt_TextChanged
            AddHandler TelNoTxt.TextChanged, AddressOf TelNoTxt_TextChanged

            ' If there are items in the grid, update their supplier_id in items_stock_tempary to maintain database consistency
            If PurchGrideView.Rows.Count > 0 Then
                Try
                    If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()
                    Dim updateTempSql As String = "UPDATE items_stock_tempary SET supplier_id = @sid WHERE inv_no = @inv AND (draft_user = @uname OR pc_name = @pc)"
                    Using cmdUpdate As New MySqlCommand(updateTempSql, MySqlConn)
                        cmdUpdate.Parameters.AddWithValue("@sid", selectedSupplierID)
                        cmdUpdate.Parameters.AddWithValue("@inv", InvNoTxt.Text.Trim())
                        cmdUpdate.Parameters.AddWithValue("@uname", Module1.UserName)
                        cmdUpdate.Parameters.AddWithValue("@pc", Environment.MachineName)
                        cmdUpdate.ExecuteNonQuery()
                    End Using
                    MySqlConn.Close()
                Catch exTemp As Exception
                    If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
                End Try
                ' Reload the grid to make sure grid cell data matches new supplier
                PurchEntLoad()
            End If

            ' Temporarily remove handlers again for the rest of the SelectSupplier logic that might change text values
            RemoveHandler SupplierNameTxt.TextChanged, AddressOf SupplierNameTxt_TextChanged
            RemoveHandler TelNoTxt.TextChanged, AddressOf TelNoTxt_TextChanged

            If supName.Trim().ToLower().Contains("banet") Then
                If String.IsNullOrWhiteSpace(InvNoTxt.Text) OrElse Not InvNoTxt.Text.StartsWith("PE") Then
                    InvNoTxt.Text = GeneratePEInvoiceNumber()
                Else
                    If IsInvoiceNumberDuplicate(InvNoTxt.Text, selectedSupplierID) Then
                        MessageBox.Show("This invoice number already exists for " & supName & "! Please enter a unique invoice number.", "Duplicate Invoice", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        InvNoTxt.Focus()
                        InvNoTxt.SelectAll()
                    End If
                End If
                InvNoTxt.ReadOnly = False
                ComboBox1.Text = "Cash"
            Else
                If InvNoTxt.Text.StartsWith("PE") Then
                    InvNoTxt.Text = ""
                End If
                InvNoTxt.ReadOnly = False

                If Not String.IsNullOrWhiteSpace(InvNoTxt.Text) Then
                    If IsInvoiceNumberDuplicate(InvNoTxt.Text, selectedSupplierID) Then
                        MessageBox.Show("This invoice number already exists for " & supName & "! Please enter a unique invoice number.", "Duplicate Invoice", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        InvNoTxt.Focus()
                        InvNoTxt.SelectAll()
                    End If
                End If
            End If

            ' Auto-fill limits and current credit balance
            LoadSupplierLimits(selectedSupplierID)

            ' Restore handlers
            AddHandler SupplierNameTxt.TextChanged, AddressOf SupplierNameTxt_TextChanged
            AddHandler TelNoTxt.TextChanged, AddressOf TelNoTxt_TextChanged

            SupplicerGrideView.Visible = False
            IT_CodeTextBox.Focus()
            IT_CodeTextBox.SelectAll()
        End If
    End Sub

    Private Sub LoadSupplierLimits(supID As Integer)
        If supID <= 0 Then
            _selectedDebitLimit = 0
            _selectedDebitPeriod = DateTime.Today
            selectedSupplierID = 0
            txtDebitLimitDisplay.Text = "0.00"
            dtpDebitPeriodDisplay.Value = DateTime.Today
            sudebitt.Text = "0.00"
            Return
        End If

        Try
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
            MySqlConn.Open()

            ' Single query to get everything: Limit, Period, and Current Outstanding Balance
            Dim query As String = "SELECT s.debit_limit, s.debit_period, " &
                                 "IFNULL((SELECT SUM(balance_due) FROM purchasing WHERE supplier_id = s.id), 0) as current_balance " &
                                 "FROM supplier s WHERE s.id = @id"

            Using cmd As New MySqlCommand(query, MySqlConn)
                cmd.Parameters.AddWithValue("@id", supID)
                Using dr As MySqlDataReader = cmd.ExecuteReader()
                    If dr.Read() Then
                        ' 1. Debit Limit
                        _selectedDebitLimit = If(dr("debit_limit") Is DBNull.Value, 0, Convert.ToDouble(dr("debit_limit")))
                        txtDebitLimitDisplay.Text = _selectedDebitLimit.ToString("F2")

                        ' 2. Debit Period
                        If dr("debit_period") IsNot DBNull.Value Then
                            _selectedDebitPeriod = Convert.ToDateTime(dr("debit_period"))
                            dtpDebitPeriodDisplay.Value = _selectedDebitPeriod
                        Else
                            _selectedDebitPeriod = DateTime.Today
                            dtpDebitPeriodDisplay.Value = DateTime.Today
                        End If

                        ' 3. Current Credit Balance
                        Dim currentBal As Double = If(dr("current_balance") Is DBNull.Value, 0, Convert.ToDouble(dr("current_balance")))
                        sudebitt.Text = currentBal.ToString("F2")
                    End If
                End Using
            End Using
            MySqlConn.Close()

        Catch ex As Exception
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Sub

    Private Sub SupplierNameTxt_Click(sender As Object, e As EventArgs) Handles SupplierNameTxt.Click
        If historyLevel = 0 AndAlso SupplierNameTxt.Text <> "" Then
            SupplicerGrideView.BringToFront()
            SupplicerGrideView.Visible = True
        End If
    End Sub

    Private Sub sudebitt_Enter(sender As Object, e As EventArgs) Handles sudebitt.Enter
        ' Shift focus instantly to prevent cursor from sitting in this display box
        SupplierNameTxt.Focus()
    End Sub

    Private Sub TelNoTxt_Click(sender As Object, e As EventArgs) Handles TelNoTxt.Click
        If historyLevel = 0 AndAlso TelNoTxt.Text <> "" Then
            SupplicerGrideView.BringToFront()
            SupplicerGrideView.Visible = True
        End If
    End Sub



    Private Sub SearchItems()
        Dim codeKey As String = IT_CodeTextBox.Text.Trim()
        Dim descKey As String = DescriptionTextBox.Text.Trim()

        If String.IsNullOrWhiteSpace(codeKey) AndAlso String.IsNullOrWhiteSpace(descKey) Then
            ItemsShow.Visible = False
            Return
        End If

        Try
            If conn.State = ConnectionState.Closed Then conn.Open()
            Dim table As New DataTable()

            Dim conditions As New List(Of String)()
            Dim sqlParams As New List(Of MySqlParameter)()

            ' 1. Filter by Item Code prefix if provided
            If Not String.IsNullOrWhiteSpace(codeKey) Then
                conditions.Add("(i.id LIKE @codeKey OR i.barcode LIKE @codeKey OR REPLACE(i.id, '-', '') LIKE @codeKeyWithoutHyphen OR REPLACE(i.barcode, '-', '') LIKE @codeKeyWithoutHyphen)")
                sqlParams.Add(New MySqlParameter("@codeKey", codeKey & "%"))
                sqlParams.Add(New MySqlParameter("@codeKeyWithoutHyphen", codeKey.Replace("-", "") & "%"))
            End If

            ' 2. Filter by Description keywords anywhere in any order
            If Not String.IsNullOrWhiteSpace(descKey) Then
                Dim words = descKey.Split(New Char() {" "c}, StringSplitOptions.RemoveEmptyEntries)
                For i As Integer = 0 To words.Length - 1
                    Dim wordParamName = "@descWord" & i
                    conditions.Add("i.description LIKE " & wordParamName)
                    sqlParams.Add(New MySqlParameter(wordParamName, "%" & words(i) & "%"))
                Next
            End If

            Dim whereClause As String = String.Join(" AND ", conditions)

            Dim query As String = "SELECT i.id, i.description, " &
                "IFNULL((SELECT ist.item_cost FROM items_stock ist WHERE ist.item_id = i.id ORDER BY ist.date DESC, ist.id DESC LIMIT 1), i.item_cost) as item_cost, " &
                "IFNULL((SELECT ist.avg_cost FROM items_stock ist WHERE ist.item_id = i.id ORDER BY ist.date DESC, ist.id DESC LIMIT 1), i.avg_cost) as avg_cost, " &
                "IFNULL((SELECT ist.selling_price FROM items_stock ist WHERE ist.item_id = i.id ORDER BY ist.date DESC, ist.id DESC LIMIT 1), i.selling_price) as selling_price, " &
                "IFNULL((SELECT ist.whole_selling_price FROM items_stock ist WHERE ist.item_id = i.id ORDER BY ist.date DESC, ist.id DESC LIMIT 1), i.whole_selling_price) as wprice, " &
                "IFNULL((SELECT ist.retail_selling_price FROM items_stock ist WHERE ist.item_id = i.id ORDER BY ist.date DESC, ist.id DESC LIMIT 1), i.retail_selling_price) as rprice, " &
                "IFNULL((SELECT SUM(st_qty) FROM items_stock WHERE item_id = i.id), 0) as st_qty " &
                "FROM items i WHERE " & whereClause & " ORDER BY i.id ASC"

            Dim cmd As New MySqlCommand(query, conn)
            For Each param In sqlParams
                cmd.Parameters.Add(param)
            Next

            Dim adapter As New MySqlDataAdapter(cmd)
            adapter.Fill(table)

            Dim dv As New DataView(table)
            ItemsShow.DataSource = dv
            SetItemsShowColumnWidths()
            conn.Close()
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try

        If IT_CodeTextBox.Text <> "" OrElse DescriptionTextBox.Text <> "" Then
            If Not ItemsShow.Visible Then ItemsShow.Visible = True
        End If
    End Sub

    Private Sub IT_CodeTextBox_TextChanged(sender As Object, e As EventArgs) Handles IT_CodeTextBox.TextChanged
        SearchItems()
    End Sub

    Private Sub DescriptionTextBox_TextChanged(sender As Object, e As EventArgs) Handles DescriptionTextBox.TextChanged
        SearchItems()
    End Sub

    Private Sub IT_CodeTextBox_Leave(sender As Object, e As EventArgs) Handles IT_CodeTextBox.Leave
        Dim activeCtrl As Control = Me.ActiveControl
        If activeCtrl IsNot Nothing AndAlso activeCtrl IsNot DescriptionTextBox AndAlso activeCtrl IsNot ItemsShow Then
            ItemsShow.Visible = False
        End If
    End Sub

    Private Sub DescriptionTextBox_Leave(sender As Object, e As EventArgs) Handles DescriptionTextBox.Leave
        Dim activeCtrl As Control = Me.ActiveControl
        If activeCtrl IsNot Nothing AndAlso activeCtrl IsNot IT_CodeTextBox AndAlso activeCtrl IsNot ItemsShow Then
            ItemsShow.Visible = False
        End If
    End Sub

    Private Sub ItemsShow_Leave(sender As Object, e As EventArgs) Handles ItemsShow.Leave
        Dim activeCtrl As Control = Me.ActiveControl
        If activeCtrl IsNot Nothing AndAlso activeCtrl IsNot IT_CodeTextBox AndAlso activeCtrl IsNot DescriptionTextBox Then
            ItemsShow.Visible = False
        End If
    End Sub

    Private Sub IT_CodeTextBox_KeyDown(sender As Object, e As KeyEventArgs) Handles IT_CodeTextBox.KeyDown
        If e.KeyCode = Keys.Up Or e.KeyCode = Keys.Down Then
            ItemsShow.Focus()
        End If
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                TelNoTxt.Focus()
                TelNoTxt.SelectAll()
            Else
                Dim typedText As String = IT_CodeTextBox.Text.Trim().ToUpper().Replace("-", "")
                Dim foundExact As Boolean = False
                Dim exactIndex As Integer = -1

                ' Check if any row in ItemsShow has an exact match for the typed text
                If ItemsShow.Rows.Count > 0 Then
                    For i As Integer = 0 To ItemsShow.Rows.Count - 1
                        Dim itemCode As String = Convert.ToString(ItemsShow.Item(0, i).Value).Trim().ToUpper().Replace("-", "")
                        If itemCode = typedText Then
                            foundExact = True
                            exactIndex = i
                            Exit For
                        End If
                    Next
                End If

                If foundExact Then
                    SelectItem(exactIndex)
                Else
                    ' If not an exact match, move to Description
                    DescriptionTextBox.Focus()
                    DescriptionTextBox.SelectAll()
                End If
                e.SuppressKeyPress = True
            End If
        End If
    End Sub

    Private Sub SelectItem(rowIndex As Integer)
        If rowIndex >= 0 AndAlso rowIndex < ItemsShow.Rows.Count Then
            ' Capture all values FIRST to avoid ArgumentOutOfRangeException
            ' changing TextBoxes triggers TextChanged events which re-bind the grid
            Dim itmCode As String = Convert.ToString(ItemsShow.Item(0, rowIndex).Value)
            Dim itmDesc As String = Convert.ToString(ItemsShow.Item(1, rowIndex).Value)
            Dim itmCost As String = Convert.ToString(ItemsShow.Item(2, rowIndex).Value)
            Dim itmStock As String = ""
            If ItemsShow.ColumnCount > 7 Then
                itmStock = Convert.ToString(ItemsShow.Item(7, rowIndex).Value)
            End If

            Dim itmAvgCost As String = ""
            If ItemsShow.ColumnCount > 3 Then itmAvgCost = Convert.ToString(ItemsShow.Item(3, rowIndex).Value)

            ' Additional price fields
            Dim itmSell As String = ""
            Dim itmWPrice As String = ""
            Dim itmRPrice As String = ""
            If ItemsShow.ColumnCount > 4 Then itmSell = Convert.ToString(ItemsShow.Item(4, rowIndex).Value)
            If ItemsShow.ColumnCount > 5 Then itmWPrice = Convert.ToString(ItemsShow.Item(5, rowIndex).Value)
            If ItemsShow.ColumnCount > 6 Then itmRPrice = Convert.ToString(ItemsShow.Item(6, rowIndex).Value)

            ' Temporarily remove handlers to prevent recursive trigger and UI flickering
            RemoveHandler IT_CodeTextBox.TextChanged, AddressOf IT_CodeTextBox_TextChanged
            RemoveHandler DescriptionTextBox.TextChanged, AddressOf DescriptionTextBox_TextChanged

            ' Now apply values to textboxes safely
            IT_CodeTextBox.Text = itmCode
            DescriptionTextBox.Text = itmDesc
            TextBoxItemCost.Text = itmCost
            TextBoxAvgCost.Text = itmAvgCost
            If itmStock <> "" Then StockTxt.Text = itmStock

            ' Populate Prices
            TextBox1.Text = itmSell
            TextBox2.Text = itmWPrice
            TextBox3.Text = itmRPrice

            ' Restore handlers
            AddHandler IT_CodeTextBox.TextChanged, AddressOf IT_CodeTextBox_TextChanged
            AddHandler DescriptionTextBox.TextChanged, AddressOf DescriptionTextBox_TextChanged

            ItemsShow.Visible = False
            TextBoxItemCost.Focus()
            TextBoxItemCost.SelectAll()
        End If
    End Sub

    Private Sub ItemsShow_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles ItemsShow.CellClick
        If e.RowIndex >= 0 Then
            SelectItem(e.RowIndex)
        End If
    End Sub


    Private Sub IT_CodeTextBox_Click(sender As Object, e As EventArgs) Handles IT_CodeTextBox.Click
        ItemsShow.Visible = True
        If String.IsNullOrWhiteSpace(IT_CodeTextBox.Text) AndAlso String.IsNullOrWhiteSpace(DescriptionTextBox.Text) Then
            load_data()
        Else
            SearchItems()
        End If
    End Sub


    Private Sub DescriptionTextBox_Click(sender As Object, e As EventArgs) Handles DescriptionTextBox.Click
        ItemsShow.Visible = True
        If String.IsNullOrWhiteSpace(IT_CodeTextBox.Text) AndAlso String.IsNullOrWhiteSpace(DescriptionTextBox.Text) Then
            load_data()
        Else
            SearchItems()
        End If
    End Sub

    Private Sub DescriptionTextBox_KeyDown(sender As Object, e As KeyEventArgs) Handles DescriptionTextBox.KeyDown
        If e.KeyCode = Keys.Up Or e.KeyCode = Keys.Down Then
            ItemsShow.Focus()
        End If
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                IT_CodeTextBox.Focus()
                IT_CodeTextBox.SelectAll()
            Else
                If DescriptionTextBox.Text.Trim() <> "" AndAlso ItemsShow.Rows.Count > 0 Then
                    SelectItem(0)
                    e.SuppressKeyPress = True
                ElseIf DescriptionTextBox.Text.Trim() <> "" Then
                    ' No items found - offer to create new
                    Dim result As DialogResult = MessageBox.Show("Description not found. Would you like to create a new item?", "New Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    If result = DialogResult.Yes Then
                        Dim frm As New Item_manage()
                        frm.TextBoxDes.Text = DescriptionTextBox.Text
                        frm.ShowDialog()
                        ' Refresh search after closing
                        DescriptionTextBox_TextChanged(Nothing, Nothing)
                    End If
                    e.SuppressKeyPress = True
                End If
                ComboBoxLocation.Focus()
                e.SuppressKeyPress = True
            End If
        End If
    End Sub

    Private Sub ComboBoxLocation_KeyDown(sender As Object, e As KeyEventArgs) Handles ComboBoxLocation.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                DescriptionTextBox.Focus()
                DescriptionTextBox.SelectAll()
            Else
                TextBoxItemCost.Focus()
                TextBoxItemCost.SelectAll()
            End If
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub ItemsShow_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles ItemsShow.CellContentClick
        If e.RowIndex >= 0 Then
            SelectItem(e.RowIndex)
        End If
    End Sub

    Private Sub ItemsShow_KeyDown(sender As Object, e As KeyEventArgs) Handles ItemsShow.KeyDown
        If e.KeyCode = Keys.Enter Then
            If ItemsShow.Rows.Count > 0 AndAlso ItemsShow.CurrentRow IsNot Nothing Then
                Dim sel As Integer = ItemsShow.CurrentRow.Index
                SelectItem(sel)
                e.SuppressKeyPress = True ' Prevent default Enter behavior in grid
            End If
        End If
    End Sub

    Private Sub Unit_PriceTextBox_TextChanged(sender As Object, e As EventArgs) Handles TextBoxItemCost.TextChanged
        CalculateAmount()
    End Sub

    Private Sub Unit_PriceTextBox_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBoxItemCost.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                ComboBoxLocation.Focus()
            Else
                TextBox1.Focus()
                TextBox1.SelectAll()
            End If
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub TextBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox1.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                TextBoxItemCost.Focus()
                TextBoxItemCost.SelectAll()
            Else
                TextBox2.Focus()
                TextBox2.SelectAll()
            End If
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub TextBox2_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox2.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                TextBox1.Focus()
                TextBox1.SelectAll()
            Else
                TextBox3.Focus()
                TextBox3.SelectAll()
            End If
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub TextBox3_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox3.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                TextBox2.Focus()
                TextBox2.SelectAll()
            Else
                TextBoxAvgCost.Focus()
                TextBoxAvgCost.SelectAll()
            End If
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub TextBoxAvgCost_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBoxAvgCost.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                TextBox3.Focus()
                TextBox3.SelectAll()
            Else
                QutTextBox.Focus()
                QutTextBox.SelectAll()
            End If
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub QutTextBox_TextChanged(sender As Object, e As EventArgs) Handles QutTextBox.TextChanged
        CalculateAmount()
    End Sub

    Private Sub QutTextBox_KeyDown(sender As Object, e As KeyEventArgs) Handles QutTextBox.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                TextBox3.Focus()
                TextBox3.SelectAll()
                e.SuppressKeyPress = True
                Return
            End If
            If QutTextBox.Text = "" Then
            Else
                If StockTxt.Text = "" Then
                    Try
                        If conn.State = ConnectionState.Closed Then conn.Open()
                        Dim cmdStock As New MySqlCommand("SELECT st_qty FROM items WHERE id = @id", conn)
                        cmdStock.Parameters.AddWithValue("@id", IT_CodeTextBox.Text)
                        Dim result = cmdStock.ExecuteScalar()
                        If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                            stockqty = Val(Convert.ToString(result))
                        Else
                            stockqty = 0
                        End If
                        conn.Close()
                    Catch ex As Exception
                        If conn.State = ConnectionState.Open Then conn.Close()
                        stockqty = 0
                    End Try
                Else
                    stockqty = Val(StockTxt.Text)
                End If
                quty = Val(QutTextBox.Text)
            End If

            DiscountTextBox.Focus()
            DiscountTextBox.SelectAll()
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub DiscountTextBox_TextChanged(sender As Object, e As EventArgs) Handles DiscountTextBox.TextChanged
        CalculateAmount()
    End Sub

    Private Sub DiscountTextBox_KeyDown(sender As Object, e As KeyEventArgs) Handles DiscountTextBox.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                QutTextBox.Focus()
                QutTextBox.SelectAll()
            Else
                CalculateAmount() ' Final check/trigger
                Button2.PerformClick()
            End If
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Function ValidatePrices() As Boolean
        Dim itemCost As Double = Val(TextBoxItemCost.Text)
        Dim sellPrice As Double = Val(TextBox1.Text)
        Dim wPrice As Double = Val(TextBox2.Text)
        Dim rPrice As Double = Val(TextBox3.Text)
        Dim avgCost As Double = Val(TextBoxAvgCost.Text)

        ' Only validate prices that have been entered (> 0); empty/zero means not provided
        If sellPrice > 0 AndAlso sellPrice < itemCost Then
            MessageBox.Show("Selling Price cannot be less than Item Cost (" & itemCost.ToString("F3") & ")", "Price Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TextBox1.Focus()
            Return False
        End If

        If wPrice > 0 AndAlso wPrice < itemCost Then
            MessageBox.Show("Wholesale Price (WPrice) cannot be less than Item Cost (" & itemCost.ToString("F3") & ")", "Price Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TextBox2.Focus()
            Return False
        End If

        If rPrice > 0 AndAlso rPrice < itemCost Then
            MessageBox.Show("Retail Price (RPrice) cannot be less than Item Cost (" & itemCost.ToString("F3") & ")", "Price Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TextBox3.Focus()
            Return False
        End If

        If avgCost > 0 AndAlso avgCost < itemCost Then
            MessageBox.Show("Average Cost (AvgCost) cannot be less than Item Cost (" & itemCost.ToString("F3") & ")", "Price Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TextBoxAvgCost.Focus()
            Return False
        End If

        Return True
    End Function

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ' Validation for Invoice Number
        If String.IsNullOrWhiteSpace(InvNoTxt.Text) Then
            MessageBox.Show("Please Enter Invoice Number before adding items.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            InvNoTxt.Focus()
            Return
        End If

        ' Validation for Supplier
        If String.IsNullOrWhiteSpace(SupplierNameTxt.Text) OrElse selectedSupplierID = 0 Then
            MessageBox.Show("Please Select a valid Supplier from the list first.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            SupplierNameTxt.Focus()
            Return
        End If

        ' Proactive supplier ID lookup if it's 0 but name exists
        If selectedSupplierID = 0 AndAlso SupplierNameTxt.Text <> "" Then
            Try
                If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
                MySqlConn.Open()
                Dim lookupQuery As String = "SELECT id FROM supplier WHERE name = @sname LIMIT 1"
                Dim lookupCmd As New MySqlCommand(lookupQuery, MySqlConn)
                lookupCmd.Parameters.AddWithValue("@sname", SupplierNameTxt.Text)
                Dim res As Object = lookupCmd.ExecuteScalar()
                If res IsNot Nothing AndAlso Not IsDBNull(res) Then
                    selectedSupplierID = CInt(res)
                End If
                MySqlConn.Close()
            Catch ex As Exception
                If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
            End Try
        End If

        If IT_CodeTextBox.Text = "" Then
            MessageBox.Show("Please Select Item (Enter on Item Code or Click from Grid)", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            IT_CodeTextBox.Focus()
            Return
        End If

        ' Prevent duplicate items in the purchasing grid
        For i As Integer = 0 To PurchGrideView.Rows.Count - 1
            Dim existingCode As Object = PurchGrideView.Rows(i).Cells(1).Value
            If existingCode IsNot Nothing AndAlso Convert.ToString(existingCode).Equals(IT_CodeTextBox.Text, StringComparison.OrdinalIgnoreCase) Then
                Dim resp As DialogResult = MessageBox.Show("This item is already added to the list." & vbCrLf & "It will now be selected so you can modify the quantity/price using the Edit button.", "Duplicate Item", MessageBoxButtons.OK, MessageBoxIcon.Information)
                If resp = DialogResult.OK Then
                    ' Visually select the row in the grid (highlight in blue)
                    PurchGrideView.ClearSelection()
                    PurchGrideView.Rows(i).Selected = True
                    ' Scroll to the duplicated row to ensure visibility
                    PurchGrideView.FirstDisplayedScrollingRowIndex = i

                    ' Load the item into the editing fields just as if it were clicked
                    SelectItemFromGrid(i)
                End If
                Return
            End If
        Next
        If DescriptionTextBox.Text = "" Then
            MessageBox.Show("Description cannot be empty", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            DescriptionTextBox.Focus()
            Return
        End If
        If QutTextBox.Text = "" OrElse Val(QutTextBox.Text) <= 0 Then
            MessageBox.Show("Please Enter Valid Qty", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            QutTextBox.Focus()
            Return
        End If
        If TextBoxItemCost.Text = "" Then
            MessageBox.Show("Item Cost cannot be empty", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TextBoxItemCost.Focus()
            Return
        End If

        ' Validate Prices against Item Cost
        If Not ValidatePrices() Then Return

        Dim chk As String = ""
        Dim oldqty As Double
        For q As Integer = 0 To PurchGrideView.Rows.Count - 1 Step +1
            If IT_CodeTextBox.Text = Convert.ToString(PurchGrideView.Rows(q).Cells(1).Value) Then
                chk = "Isinn"
                oldqty = Val(Convert.ToString(PurchGrideView.Rows(q).Cells(3).Value))
            End If
        Next

        If chk = "Isinn" Then
            ' Already in the temp list - update it
            Try
                Dim updateQuery As String = "UPDATE `items_stock_tempary` SET `st_qty` = @qty, `qty_purchased` = @qty, `item_cost` = @cost, `avg_cost` = @avg, `amount` = @amt, " &
                    "`selling_price` = @sell, `whole_selling_price` = @wprice, `retail_selling_price` = @rprice, `location_id` = @loc, `date` = NOW() " &
                    "WHERE `item_id` = @code AND `inv_no` = @inv AND (draft_user = @uname OR pc_name = @pc)"

                MySqlConn.Open()
                COMMAND = New MySqlCommand(updateQuery, MySqlConn)
                COMMAND.Parameters.AddWithValue("@qty", Val(QutTextBox.Text))
                COMMAND.Parameters.AddWithValue("@cost", Val(TextBoxItemCost.Text))
                COMMAND.Parameters.AddWithValue("@avg", Val(TextBoxAvgCost.Text))
                COMMAND.Parameters.AddWithValue("@amt", Val(AmountTextBox.Text))
                COMMAND.Parameters.AddWithValue("@sell", Val(TextBox1.Text))
                COMMAND.Parameters.AddWithValue("@wprice", Val(TextBox2.Text))
                COMMAND.Parameters.AddWithValue("@rprice", Val(TextBox3.Text))
                COMMAND.Parameters.AddWithValue("@loc", ComboBoxLocation.SelectedValue)
                COMMAND.Parameters.AddWithValue("@code", IT_CodeTextBox.Text)
                COMMAND.Parameters.AddWithValue("@inv", InvNoTxt.Text)
                COMMAND.Parameters.AddWithValue("@uname", Module1.UserName)
                COMMAND.Parameters.AddWithValue("@pc", Environment.MachineName)
                COMMAND.ExecuteNonQuery()
                MySqlConn.Close()
            Catch ex As Exception
                If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
                MessageBox.Show("Update Error: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Else
            ' New item to the temp list
            Try
                Dim insertQuery As String = "INSERT INTO items_stock_tempary (id, inv_no, supplier_id, item_id, description, st_qty, qty_purchased, item_cost, avg_cost, discount, amount, selling_price, whole_selling_price, retail_selling_price, location_id, `date`, draft_user, pc_name) " &
                              "VALUES (@id, @inv, @sup, @code, @name, @qty, @qty, @cost, @avg, @dis, @amt, @sell, @wprice, @rprice, @loc, @now_local, @uname, @pc)"

                MySqlConn.Open()
                COMMAND = New MySqlCommand(insertQuery, MySqlConn)
                COMMAND.Parameters.AddWithValue("@id", InvNoTxt.Text & "_" & IT_CodeTextBox.Text & "_" & Module1.UserName)
                COMMAND.Parameters.AddWithValue("@inv", InvNoTxt.Text)
                COMMAND.Parameters.AddWithValue("@sup", selectedSupplierID)
                COMMAND.Parameters.AddWithValue("@code", IT_CodeTextBox.Text)
                COMMAND.Parameters.AddWithValue("@name", DescriptionTextBox.Text)
                COMMAND.Parameters.AddWithValue("@qty", Val(QutTextBox.Text))
                COMMAND.Parameters.AddWithValue("@cost", Val(TextBoxItemCost.Text))
                COMMAND.Parameters.AddWithValue("@avg", Val(TextBoxAvgCost.Text))
                COMMAND.Parameters.AddWithValue("@dis", Val(DiscountTextBox.Text))
                COMMAND.Parameters.AddWithValue("@amt", Val(AmountTextBox.Text))
                COMMAND.Parameters.AddWithValue("@sell", Val(TextBox1.Text))
                COMMAND.Parameters.AddWithValue("@wprice", Val(TextBox2.Text))
                COMMAND.Parameters.AddWithValue("@rprice", Val(TextBox3.Text))
                COMMAND.Parameters.AddWithValue("@loc", ComboBoxLocation.SelectedValue)
                COMMAND.Parameters.AddWithValue("@now_local", DateTime.Now)
                COMMAND.Parameters.AddWithValue("@uname", Module1.UserName)
                COMMAND.Parameters.AddWithValue("@pc", Environment.MachineName)

                COMMAND.ExecuteNonQuery()
                MySqlConn.Close()
            Catch ex As Exception
                If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
                MessageBox.Show("Insert Error: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If

        Dim addedCode As String = IT_CodeTextBox.Text
        PurchEntLoad()
        CalculateSummary()
        TextBoxItemCost.Clear()
        IT_CodeTextBox.Clear()
        DescriptionTextBox.Clear()
        QutTextBox.Clear()
        DiscountTextBox.Clear()
        AmountTextBox.Clear()
        TextBoxAvgCost.Clear()
        TextBox1.Clear()
        TextBox2.Clear()
        TextBox3.Clear()

        chk = "a"
        IT_CodeTextBox.Select()
        LockHeader(True)
        AddProcessLog("Item Added to Temp", "IT Code: " & addedCode & ", Loc: " & ComboBoxLocation.Text)

        ' Switch back to main view if item is added while viewing history
        If historyLevel > 0 Then
            DataGridView1.Visible = False
            PurchGrideView.Visible = True
            historyLevel = 0
            sdetailsbtn.Text = "Supplier Details"
            CalculateSummary()
        End If

    End Sub

    Private Function CheckSupplierLimits() As Boolean
        ' Only validate if it's a Credit purchase
        Dim selBillingType As String = If(String.IsNullOrWhiteSpace(ComboBox1.Text), "Cash", ComboBox1.Text).Trim()
        If Not String.Equals(selBillingType, "Credit", StringComparison.OrdinalIgnoreCase) Then
            Return True ' Allow Cash/Cheque regardless of limits
        End If

        ' 1. Check Debit Period
        If dtpDebitPeriodDisplay.Value.Date < DateTime.Today Then
            MessageBox.Show("Cannot do purchase: exceeded debit period (Expired: " & dtpDebitPeriodDisplay.Value.ToString("yyyy-MM-dd") & ")", "Limit Exceeded", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        ' 2. Correct Check Debit Limit
        ' We calculate total debt by:
        ' (All other unpaid purchasing bills + All manual credits) + (This current bill's remaining balance)

        Dim otherCreditBalance As Double = 0
        Try
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Open()

            ' Query Sum of ALL other debts except this invoice
            Dim sqlSum As String = "SELECT " &
                "IFNULL((SELECT SUM(balance_due) FROM purchasing WHERE supplier_id = @sup AND pur_id <> @inv), 0)"

            Using cmdSum As New MySqlCommand(sqlSum, conn)
                cmdSum.Parameters.AddWithValue("@sup", selectedSupplierID)
                cmdSum.Parameters.AddWithValue("@inv", InvNoTxt.Text)

                Dim resSum As Object = cmdSum.ExecuteScalar()
                If resSum IsNot Nothing AndAlso Not IsDBNull(resSum) Then
                    otherCreditBalance = Convert.ToDouble(resSum)
                End If
            End Using
            conn.Close()
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try

        Dim newGrandTotal As Double = 0
        Double.TryParse(Fullsumtxt.Text.Replace(",", ""), newGrandTotal)

        Dim paidAmount As Double = 0
        Double.TryParse(AmountTextBox2.Text.Replace(",", ""), paidAmount)

        ' Only count the portion that will be added to credit
        Dim pendingCreditOnThisBill As Double = newGrandTotal - paidAmount

        ' Total future credit = Other debts + This bill's portion going to credit
        Dim totalFutureCredit As Double = otherCreditBalance + pendingCreditOnThisBill

        If _selectedDebitLimit > 0 AndAlso totalFutureCredit > _selectedDebitLimit Then
            MessageBox.Show("Cannot do purchase: exceeded the debit limit." & vbCrLf &
                            "Total Credit including this bill: " & totalFutureCredit.ToString("F3") & vbCrLf &
                            "Allowed Limit: " & _selectedDebitLimit.ToString("F3"), "Limit Exceeded", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Return True
    End Function

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Dim savedInvNo As String = InvNoTxt.Text.Trim()
        If isPRModeChecked Then
            MessageBox.Show("Standard invoice saving is disabled in Print Request Mode. Please use the 'Print Request' button.", "Save Blocked", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' 1. Validate Invoice Number
        If String.IsNullOrWhiteSpace(InvNoTxt.Text) Then
            MessageBox.Show("Please enter an Invoice Number.", "Validation Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            InvNoTxt.Focus()
            Return
        End If

        ' Validate Billing Type
        If String.IsNullOrWhiteSpace(ComboBox1.Text) Then
            MessageBox.Show("Please select a Billing Type.", "Validation Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            ComboBox1.Focus()
            ComboBox1.DroppedDown = True
            Return
        End If

        ' Validate Payment Method
        If String.IsNullOrWhiteSpace(txPaymentMethod.Text) Then
            MessageBox.Show("Please select a Payment Method.", "Validation Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txPaymentMethod.Focus()
            Return
        End If

        ' Auto-lookup supplier ID if not already captured (e.g., form reloaded with existing data)
        If selectedSupplierID = 0 AndAlso SupplierNameTxt.Text <> "" Then
            Try
                If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()
                Dim lookupQuery As String = "SELECT id FROM supplier WHERE name = @sname LIMIT 1"
                Dim lookupCmd As New MySqlCommand(lookupQuery, MySqlConn)
                lookupCmd.Parameters.AddWithValue("@sname", SupplierNameTxt.Text)
                Dim result As Object = lookupCmd.ExecuteScalar()
                If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                    selectedSupplierID = CInt(result)
                End If
                ' Removed the forceful close here to not disrupt subsequent code that expects it open,
                ' or rely on the outer save transaction to handle it.
            Catch ex As Exception
                If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
            End Try
        End If

        ' 2. Validate Supplier
        If String.IsNullOrWhiteSpace(SupplierNameTxt.Text) OrElse selectedSupplierID = 0 Then
            MessageBox.Show("Please add a supplier.", "Validation Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            SupplierNameTxt.Focus()
            Return
        End If

        ' Check for duplicate invoice number for the selected supplier
        If IsInvoiceNumberDuplicate(InvNoTxt.Text, selectedSupplierID) Then
            MessageBox.Show("This invoice number already exists for " & SupplierNameTxt.Text & "! Please enter a unique invoice number.", "Duplicate Invoice", MessageBoxButtons.OK, MessageBoxIcon.Error)
            InvNoTxt.Focus()
            InvNoTxt.SelectAll()
            Return
        End If

        ' Check Billing Type for Banet Motors
        If Not chkPrintOnly.Checked AndAlso SupplierNameTxt.Text.Trim().ToLower().Contains("banet") Then
            Dim checkBillingType As String = If(String.IsNullOrWhiteSpace(ComboBox1.Text), "Cash", ComboBox1.Text).Trim()
            If Not String.Equals(checkBillingType, "Cash", StringComparison.OrdinalIgnoreCase) Then
                MessageBox.Show("Only Cash billing is allowed!", "Invalid Billing Type", MessageBoxButtons.OK, MessageBoxIcon.Error)
                ComboBox1.Text = "Cash"
                ComboBox1.Focus()
                Return
            End If
        End If

        ' 4. Validate Pending Item (User selected an item but didn't click Next)
        If Not String.IsNullOrWhiteSpace(IT_CodeTextBox.Text) Then
            If String.IsNullOrWhiteSpace(QutTextBox.Text) OrElse Val(QutTextBox.Text) <= 0 Then
                MessageBox.Show("Please add quantity and click Next.", "Validation Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                QutTextBox.Focus()
                Return
            Else
                MessageBox.Show("You have an item selected. Please click Next to add it before saving.", "Validation Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Button2.Focus()
                Return
            End If
        End If

        ' 3. Validate Items in Grid
        If PurchGrideView.Rows.Count = 0 Then
            MessageBox.Show("Please add items to save.", "Validation Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            IT_CodeTextBox.Focus()
            Return
        End If

        ' Check Supplier Limits (Debit Limit / Period)
        If Not chkPrintOnly.Checked Then
            If Not CheckSupplierLimits() Then
                Return
            End If
        End If
        ' Validate all items in grid to ensure prices are not less than cost
        For i As Integer = 0 To PurchGrideView.Rows.Count - 1
            Dim itmId As String = Convert.ToString(PurchGrideView.Rows(i).Cells(1).Value)
            Dim itmCost As Double = Val(PurchGrideView.Rows(i).Cells(4).Value)
            Dim itmSell As Double = Val(PurchGrideView.Rows(i).Cells(9).Value)
            Dim itmW As Double = Val(PurchGrideView.Rows(i).Cells(10).Value)
            Dim itmR As Double = Val(PurchGrideView.Rows(i).Cells(11).Value)
            Dim itmAvg As Double = Val(PurchGrideView.Rows(i).Cells(12).Value)

            ' Only flag prices that were actually entered (> 0); 0 means not provided
            If (itmSell > 0 AndAlso itmSell < itmCost) OrElse (itmW > 0 AndAlso itmW < itmCost) OrElse (itmR > 0 AndAlso itmR < itmCost) OrElse (itmAvg > 0 AndAlso itmAvg < itmCost) Then
                MessageBox.Show("The item '" & itmId & "' has prices lower than the cost. Please select it from the grid and correct the prices.", "Price Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                PurchGrideView.ClearSelection()
                PurchGrideView.Rows(i).Selected = True
                PurchGrideView.FirstDisplayedScrollingRowIndex = i
                SelectItemFromGrid(i)
                Return
            End If
        Next

        ' PRE-SAVE VALIDATION
        If Not chkPrintOnly.Checked Then
            Dim v_gTotal As Decimal = 0
            Dim v_cashAmt As Decimal = 0
            Decimal.TryParse(Fullsumtxt.Text.Replace(",", ""), v_gTotal)
            Decimal.TryParse(AmountTextBox2.Text.Replace(",", ""), v_cashAmt)

            Dim selBillingType As String = If(String.IsNullOrWhiteSpace(ComboBox1.Text), "Cash", ComboBox1.Text).Trim()

            ' Cash Validation
            If String.Equals(selBillingType, "Cash", StringComparison.OrdinalIgnoreCase) Then
                If String.IsNullOrWhiteSpace(AmountTextBox2.Text) OrElse v_cashAmt = 0 Then
                    MessageBox.Show("Please enter the Paid Amount. A cash payment is required for Cash bills.", "Paid Amount Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    AmountTextBox2.Focus()
                    Return
                End If
                If v_cashAmt < v_gTotal Then
                    MessageBox.Show("For a Cash bill, the Paid Amount must be greater than or equal to the Grand Total.", "Invalid Amount", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    AmountTextBox2.Focus()
                    Return
                End If
            End If

            ' Credit or Cheque Validation
            If String.Equals(selBillingType, "Credit", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(selBillingType, "Cheque", StringComparison.OrdinalIgnoreCase) Then
                If v_cashAmt >= v_gTotal AndAlso v_gTotal > 0 Then
                    MessageBox.Show("For a " & selBillingType & " bill, the Paid Amount must be less than the Grand Total or zero.", "Invalid Amount", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    AmountTextBox2.Focus()
                    Return
                End If
            End If
        End If

        Try
            MySqlConn.Open()

            ' 1. Insert Summary Header into 'purchasing' table
            ' pur_id is varchar in the database
            Dim purId As String = InvNoTxt.Text
            Dim itemsQty As Integer = 0
            For i As Integer = 0 To PurchGrideView.Rows.Count - 1
                itemsQty += Val(PurchGrideView.Rows(i).Cells(3).Value)
            Next

            ' --- USER REFINEMENT: Settlement, Status & Triple Split ---
            Dim gTotal As Decimal = 0
            Dim cashAmt As Decimal = 0
            Decimal.TryParse(Fullsumtxt.Text.Replace(",", ""), gTotal)
            Decimal.TryParse(AmountTextBox2.Text.Replace(",", ""), cashAmt)

            Dim bType As String = If(String.IsNullOrWhiteSpace(ComboBox1.Text), "Cash", ComboBox1.Text).Trim()

            ' Auto-switch to Cash if the cash amount covers the entire bill
            If cashAmt >= gTotal AndAlso gTotal > 0 Then
                bType = "Cash"
            End If

            Dim pMethod As String = If(String.IsNullOrWhiteSpace(txPaymentMethod.Text), "Cash", txPaymentMethod.Text).Trim()
            Dim statusValue As String = "Paid" ' Default for fully paid
            Dim chequeNo As String = ""
            Dim bankIdValue As Object = DBNull.Value
            Dim finalChequeAmount As Decimal = 0

            Dim remainingBalance As Decimal = gTotal - cashAmt

            ' Triple Split Balance Columns
            Dim chequeBalanceDue As Decimal = 0
            Dim creditBalanceDue As Decimal = 0
            Dim partialCash As Decimal = 0

            If String.Equals(bType, "Cash", StringComparison.OrdinalIgnoreCase) Then
                If cashAmt < gTotal Then
                    statusValue = "Credit" ' Fallback if not fully paid but forced Cash
                Else
                    statusValue = "Paid"
                End If
            ElseIf String.Equals(bType, "Credit", StringComparison.OrdinalIgnoreCase) Then
                If remainingBalance > 0 Then
                    Dim settlementDlg As New SettlementDialog()
                    If settlementDlg.ShowDialog() = DialogResult.OK Then
                        If settlementDlg.SelectedSettlement = "Cheque" Then
                            Dim chequeDlg As New ChequeEntryDialog()
                            chequeDlg.DefaultAmount = remainingBalance
                            If chequeDlg.ShowDialog() = DialogResult.OK Then
                                pMethod = "Cheque"
                                chequeNo = chequeDlg.ChequeNo
                                bankIdValue = chequeDlg.BankID
                                finalChequeAmount = chequeDlg.ChequeAmount
                                If finalChequeAmount < remainingBalance Then
                                    statusValue = If(cashAmt > 0, "Mixed_Payment", "Credit_Cheque")
                                Else
                                    statusValue = If(cashAmt > 0, "cash_Cheque", "Cheque")
                                End If
                            Else
                                Return
                            End If
                        Else
                            pMethod = "Credit"
                            statusValue = If(cashAmt > 0, "cash_Credit", "Credit")
                        End If
                    Else
                        Return
                    End If
                Else
                    statusValue = "Paid"
                End If
            ElseIf String.Equals(bType, "Cheque", StringComparison.OrdinalIgnoreCase) Then
                If remainingBalance <= 0 Then
                    MessageBox.Show("Full amount is covered by Cash. Cannot proceed with Cheque billing.", "Invalid Option", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
                Dim chequeDlg As New ChequeEntryDialog()
                chequeDlg.DefaultAmount = remainingBalance
                chequeDlg.LockAmount = True
                If chequeDlg.ShowDialog() = DialogResult.OK Then
                    pMethod = "Cheque"
                    chequeNo = chequeDlg.ChequeNo
                    bankIdValue = chequeDlg.BankID
                    finalChequeAmount = chequeDlg.ChequeAmount
                    statusValue = If(cashAmt > 0, "cash_Cheque", "Cheque")
                Else
                    Return
                End If
            End If

            ' FINAL CALCULATION FOR TRIPLE SPLIT COLUMNS
            If statusValue = "cash_Credit" Then
                partialCash = cashAmt
                creditBalanceDue = remainingBalance
            ElseIf statusValue = "cash_Cheque" Then
                partialCash = cashAmt
                chequeBalanceDue = remainingBalance
            ElseIf statusValue = "Mixed_Payment" Then
                partialCash = cashAmt
                chequeBalanceDue = finalChequeAmount
                creditBalanceDue = remainingBalance - finalChequeAmount
            ElseIf statusValue = "Credit_Cheque" Then
                partialCash = 0
                chequeBalanceDue = finalChequeAmount
                creditBalanceDue = remainingBalance - finalChequeAmount
            ElseIf statusValue = "Paid" OrElse statusValue = "Paid" Then
                partialCash = gTotal
                chequeBalanceDue = 0
                creditBalanceDue = 0
                statusValue = "Paid" ' Standardize for purchases
            ElseIf statusValue = "Credit" Then
                partialCash = 0
                chequeBalanceDue = 0
                creditBalanceDue = gTotal
            ElseIf statusValue = "Cheque" Then
                partialCash = 0
                chequeBalanceDue = gTotal
                creditBalanceDue = 0
            Else
                partialCash = cashAmt
                creditBalanceDue = gTotal - cashAmt
            End If

            ' RE-ASSIGN BILLING TYPE BASED ON STATUS
            If statusValue = "Paid" Then
                bType = "Cash"
            ElseIf statusValue = "Credit" OrElse statusValue = "cash_Credit" OrElse statusValue = "Mixed_Payment" OrElse statusValue = "Credit_Cheque" Then
                bType = "Credit"
            ElseIf statusValue = "Cheque" OrElse statusValue = "cash_Cheque" Then
                bType = "Cheque"
            End If

            Dim finalBal As Double = creditBalanceDue + chequeBalanceDue

            ' Handle Print Only (No Stock / Balance) Mode
            If chkPrintOnly.Checked Then
                ' Keep pur_type as a valid enum value (standard 'Cash', 'Credit', 'Cheque') to avoid database schema limits
                bType = If(String.IsNullOrWhiteSpace(ComboBox1.Text), "Cash", ComboBox1.Text).Trim()
                If bType <> "Cash" AndAlso bType <> "Credit" AndAlso bType <> "Cheque" Then
                    bType = "Cash"
                End If
                pMethod = "Print_Only"
                statusValue = "Print_Only"
                partialCash = 0
                chequeBalanceDue = 0
                creditBalanceDue = 0
                finalBal = 0
            End If

            Dim summaryQuery As String = "INSERT INTO purchasing (pur_type, pur_id, supplier_id, pur_su_method, items_qty, sub_total, cost, description, p_method, paid_amount, balance_due, inv_dis, cqe_no, bank_id, status, pur_date, date, partial_cash, cheque_balance_due, credit_balance_due) " &
                "VALUES (@p_type, @p_id, @s_id, @p_method, @qty, @sub, @cost, @desc, @pmeth, @paid, @bal, @dis, @cqe, @bank, @status, @pur_date, @date, @pcash, @chq_bal, @crd_bal)"
            Dim cmdSummary As New MySqlCommand(summaryQuery, MySqlConn)
            cmdSummary.Parameters.AddWithValue("@p_type", bType)
            cmdSummary.Parameters.AddWithValue("@p_id", purId)
            cmdSummary.Parameters.AddWithValue("@s_id", selectedSupplierID)
            cmdSummary.Parameters.AddWithValue("@p_method", ComboBox2metho.Text)
            cmdSummary.Parameters.AddWithValue("@qty", itemsQty)
            cmdSummary.Parameters.AddWithValue("@sub", Val(mgsumTxt.Text))
            cmdSummary.Parameters.AddWithValue("@cost", gTotal)
            cmdSummary.Parameters.AddWithValue("@desc", "Inv: " & InvNoTxt.Text)
            cmdSummary.Parameters.AddWithValue("@pmeth", pMethod)
            cmdSummary.Parameters.AddWithValue("@paid", partialCash)
            cmdSummary.Parameters.AddWithValue("@bal", finalBal)
            cmdSummary.Parameters.AddWithValue("@dis", Val(DiscoText.Text))
            cmdSummary.Parameters.AddWithValue("@status", statusValue)

            ' Cheque details
            If bType = "Cheque" OrElse statusValue.Contains("Cheque") OrElse statusValue = "Mixed_Payment" Then
                cmdSummary.Parameters.AddWithValue("@cqe", chequeNo)
                If bankIdValue Is Nothing OrElse Not IsNumeric(bankIdValue) Then
                    cmdSummary.Parameters.AddWithValue("@bank", DBNull.Value)
                Else
                    cmdSummary.Parameters.AddWithValue("@bank", bankIdValue)
                End If
            Else
                cmdSummary.Parameters.AddWithValue("@cqe", DBNull.Value)
                cmdSummary.Parameters.AddWithValue("@bank", DBNull.Value)
            End If

            cmdSummary.Parameters.AddWithValue("@pur_date", DateTimePicker1.Value)
            cmdSummary.Parameters.AddWithValue("@date", DateTime.Now)
            cmdSummary.Parameters.AddWithValue("@pcash", partialCash)
            cmdSummary.Parameters.AddWithValue("@chq_bal", chequeBalanceDue)
            cmdSummary.Parameters.AddWithValue("@crd_bal", creditBalanceDue)

            cmdSummary.ExecuteNonQuery()

            Dim headerID As Long = cmdSummary.LastInsertedId

            ' 2. Insert Item Details into 'items_stock' and Update 'location'
            Dim dtItems As New DataTable()
            ' Match new temp table name
            Dim adpItems As New MySqlDataAdapter("SELECT * FROM items_stock_tempary WHERE inv_no = '" & purId & "'", MySqlConn)
            adpItems.Fill(dtItems)

            For Each row As DataRow In dtItems.Rows
                Dim itemId As String = row("item_id").ToString()
                Dim qty As Double = Val(row("st_qty").ToString())
                Dim locId As Integer = Val(row("location_id").ToString())
                Dim rowId As String = row("id").ToString()

                ' A. Insert into items_stock (Batch Record)
                ' The Trigger 'trg_after_items_stock_insert' will automatically update 'items' table
                Dim stockQuery As String = "INSERT INTO items_stock (id, item_id, st_qty, qty_purchased, location_id, inv_no, supplier_id, item_cost, avg_cost, selling_price, whole_selling_price, retail_selling_price, amount, description, date, discount) " &
                    "VALUES (@id, @i_id, @qty, @qty_p, @loc, @inv, @sup, @cost, @avg, @sell, @wprice, @rprice, @amt, @desc, @now_local, @dis)"
                Dim cmdStock As MySqlCommand = New MySqlCommand(stockQuery, MySqlConn)
                cmdStock.Parameters.AddWithValue("@id", rowId)
                cmdStock.Parameters.AddWithValue("@i_id", itemId)

                ' If in Print Only mode, we store st_qty as 0 in items_stock to prevent physical stock presence, but qty_purchased remains the entered quantity for the printout!
                If chkPrintOnly.Checked Then
                    cmdStock.Parameters.AddWithValue("@qty", 0)
                Else
                    cmdStock.Parameters.AddWithValue("@qty", qty)
                End If

                cmdStock.Parameters.AddWithValue("@qty_p", Val(row("qty_purchased").ToString()))
                cmdStock.Parameters.AddWithValue("@loc", locId)
                cmdStock.Parameters.AddWithValue("@inv", purId)
                cmdStock.Parameters.AddWithValue("@sup", selectedSupplierID)
                cmdStock.Parameters.AddWithValue("@cost", Val(row("item_cost").ToString()))
                cmdStock.Parameters.AddWithValue("@avg", Val(row("avg_cost").ToString()))
                cmdStock.Parameters.AddWithValue("@sell", Val(row("selling_price").ToString()))
                cmdStock.Parameters.AddWithValue("@wprice", Val(row("whole_selling_price").ToString()))
                cmdStock.Parameters.AddWithValue("@rprice", Val(row("retail_selling_price").ToString()))
                cmdStock.Parameters.AddWithValue("@amt", Val(row("amount").ToString()))
                cmdStock.Parameters.AddWithValue("@desc", row("description").ToString())
                cmdStock.Parameters.AddWithValue("@dis", Val(row("discount").ToString()))
                cmdStock.Parameters.AddWithValue("@now_local", DateTime.Now)
                cmdStock.ExecuteNonQuery()

                ' Only update master stock if NOT in Print Only mode
                ' Note: The explicit stock update was removed because the database trigger 
                ' 'trg_after_items_stock_insert' already calculates the SUM of all stock 
                ' directly from items_stock and updates the items table.
                ' Any explicit addition/subtraction here causes stock to be miscalculated.

                ' B. Update location table (REMOVED - location is a master table, quantities are tracked in items_stock)
                SyncItemMasterData(itemId)
            Next

            ' 3. Clear temporary table for this invoice
            Dim clearTemp As String = "DELETE FROM items_stock_tempary WHERE inv_no = @inv AND (draft_user = @uname OR pc_name = @pc)"
            Dim cmdClear As New MySqlCommand(clearTemp, MySqlConn)
            cmdClear.Parameters.AddWithValue("@inv", purId)
            cmdClear.Parameters.AddWithValue("@uname", Module1.UserName)
            cmdClear.Parameters.AddWithValue("@pc", Environment.MachineName)
            cmdClear.ExecuteNonQuery()

            ' (D. Legacy purchasing_stock is now replaced by items_stock)

            ' Capture supplier name before clearing for the post-save view
            Dim savedSupplierName As String = SupplierNameTxt.Text

            ' 3. Handle Supplier Credit - Automatically record debt in supplicer_credit
            If creditBalanceDue > 0 Then
                Try
                    ' Use the existing connection
                    Dim creditQuery As String = "INSERT INTO supplicer_credit (supplier_id, sname, inv_no, amount, getdate) VALUES (@sid, @name, @inv, @amt, @dt)"
                    Using cmdCredit As New MySqlCommand(creditQuery, MySqlConn)
                        cmdCredit.Parameters.AddWithValue("@sid", selectedSupplierID)
                        cmdCredit.Parameters.AddWithValue("@name", savedSupplierName)
                        cmdCredit.Parameters.AddWithValue("@inv", purId)
                        cmdCredit.Parameters.AddWithValue("@amt", creditBalanceDue)
                        cmdCredit.Parameters.AddWithValue("@dt", DateTimePicker1.Value)
                        cmdCredit.ExecuteNonQuery()
                    End Using
                Catch exCredit As Exception
                    ' We don't want to crash the whole save if just the credit record fails, but we should notify
                    MessageBox.Show("Purchase saved, but failed to create the Debit record: " & exCredit.Message, "Debit Record Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End Try
            End If

            MySqlConn.Close()



            ' Log to Petty Cash when payment method is Cash or part-cash (Cash_Credit, Cash_Cheque, Mixed_Payment)
            Dim actualPaymentMethod As String = txPaymentMethod.Text.Trim()

            Dim isPhysicalCashPayment As Boolean =
                String.Equals(actualPaymentMethod, "Cash", StringComparison.OrdinalIgnoreCase)
            Dim isPartCashPayment As Boolean =
                String.Equals(statusValue, "cash_Credit", StringComparison.OrdinalIgnoreCase) OrElse
                String.Equals(statusValue, "Cash_Credit", StringComparison.OrdinalIgnoreCase) OrElse
                String.Equals(statusValue, "cash_Cheque", StringComparison.OrdinalIgnoreCase) OrElse
                String.Equals(statusValue, "Cash_Cheque", StringComparison.OrdinalIgnoreCase) OrElse
                String.Equals(statusValue, "Mixed_Payment", StringComparison.OrdinalIgnoreCase) OrElse
                String.Equals(actualPaymentMethod, "credit_cash", StringComparison.OrdinalIgnoreCase) OrElse
                String.Equals(actualPaymentMethod, "Credit_Cash", StringComparison.OrdinalIgnoreCase) OrElse
                String.Equals(actualPaymentMethod, "cash_credit", StringComparison.OrdinalIgnoreCase) OrElse
                String.Equals(actualPaymentMethod, "Cash_Credit", StringComparison.OrdinalIgnoreCase) OrElse
                String.Equals(actualPaymentMethod, "cash_cheque", StringComparison.OrdinalIgnoreCase) OrElse
                String.Equals(actualPaymentMethod, "Cash_Cheque", StringComparison.OrdinalIgnoreCase) OrElse
                String.Equals(actualPaymentMethod, "Mixed_Payment", StringComparison.OrdinalIgnoreCase)

            If (isPhysicalCashPayment OrElse isPartCashPayment) AndAlso partialCash > 0 AndAlso Not chkPrintOnly.Checked Then
                Dim paymentLabel As String = "Cash"
                If isPartCashPayment Then
                    If String.Equals(statusValue, "cash_Cheque", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(actualPaymentMethod, "cash_cheque", StringComparison.OrdinalIgnoreCase) Then
                        paymentLabel = "Cash_Cheque"
                    ElseIf String.Equals(statusValue, "Mixed_Payment", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(actualPaymentMethod, "Mixed_Payment", StringComparison.OrdinalIgnoreCase) Then
                        paymentLabel = "Mixed_Payment"
                    Else
                        paymentLabel = "Cash_Credit"
                    End If
                End If

                Dim transMsg As String = "Purchase Payment (" & paymentLabel & ") - " & savedSupplierName & " (Inv: " & purId & ")"
                Module1.RegisterCashTransaction(
                    partialCash,
                    "OUT",
                    transMsg,
                    purId
                )
            End If

            If chkPrintOnly.Checked Then
                AddProcessLog("Invoice Saved (Print Only)", "Supplier: " & savedSupplierName & ", Grand Total: " & Fullsumtxt.Text)
            Else
                AddProcessLog("Invoice Saved", "Supplier: " & savedSupplierName & ", Grand Total: " & Fullsumtxt.Text)
            End If
            MessageBox.Show("Purchase Saved Paidfully")

            ' Automatically display the purchase invoice report
            Try
                Dim rptForm As New SaleInv()
                rptForm.ShowReport(purId, 2, False, False, "", 1, selectedSupplierID) ' Index 2 is for Purchase Invoice
            Catch exRpt As Exception
                MessageBox.Show("Purchase saved, but failed to open invoice report: " & exRpt.Message, "Report Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Try

            ' Capture print-only state before clearing the fields
            Dim isPrintOnly As Boolean = chkPrintOnly.Checked

            ' Reset form fields
            ClearAllFields()

            ' Automatically show that supplier's invoice history after save
            If Not isPrintOnly AndAlso Not String.IsNullOrEmpty(savedSupplierName) Then
                DataGridView1.Location = PurchGrideView.Location
                DataGridView1.Size = PurchGrideView.Size
                DataGridView1.Visible = True
                PurchGrideView.Visible = False

                ' This sets historyLevel = 2 and shows the invoice list (including the one just saved) and highlights the newly saved invoice row
                isHistoryFromSave = True
                LoadSupplierInvoices(savedSupplierName, savedInvNo)
                sdetailsbtn.Text = "Back"
            ElseIf isPrintOnly Then
                ' Ensure history grid is hidden and main grid is active
                DataGridView1.Visible = False
                PurchGrideView.Visible = True
                historyLevel = 0
                sdetailsbtn.Text = "Supplier Details"
            End If

        Catch ex As Exception
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
            MessageBox.Show("Save failed: " & ex.Message)
        End Try
    End Sub

    Private Sub ClearAllFields()
        ' Clean up any temporary Print Only invoices from the database immediately
        Try
            If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()

            ' Delete from items_stock first
            Dim deleteStockSql As String = "DELETE FROM items_stock WHERE inv_no IN (SELECT pur_id FROM purchasing WHERE status = 'Print_Only')"
            Using cmdD1 As New MySqlCommand(deleteStockSql, MySqlConn)
                cmdD1.ExecuteNonQuery()
            End Using

            ' Delete from purchasing
            Dim deletePurchSql As String = "DELETE FROM purchasing WHERE status = 'Print_Only'"
            Using cmdD2 As New MySqlCommand(deletePurchSql, MySqlConn)
                cmdD2.ExecuteNonQuery()
            End Using

            MySqlConn.Close()
        Catch ex As Exception
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try

        ' 1. Clear Header and Grid first to prevent re-calculations from old data
        If isPRModeChecked Then
            InvNoTxt.Text = GeneratePRNumber()
        Else
            InvNoTxt.Clear()
        End If
        InvNoTxt.ReadOnly = False
        If InvNoTxt IsNot Nothing Then InvNoTxt.BackColor = SupplierNameTxt.BackColor
        PurchEntLoad() ' This clears the grid as InvNo is now empty

        ' Refresh items grid to immediately show updated prices/quantities
        load_data()

        ' 2. Clear Supplier Info
        SupplierNameTxt.Clear()
        TelNoTxt.Clear()
        LoadSupplierLimits(0)

        ' 3. Clear Item Input Fields
        IT_CodeTextBox.Clear()
        DescriptionTextBox.Clear()
        TextBoxItemCost.Clear()
        QutTextBox.Clear()
        DiscountTextBox.Clear()
        AmountTextBox.Clear()
        TextBoxAvgCost.Clear()
        TextBox1.Clear()
        TextBox2.Clear()
        TextBox3.Clear()
        StockTxt.Clear()

        ' 4. Clear Summary Fields
        mgsumTxt.Text = "0.00"
        Fullsumtxt.Text = "0.00"
        DiscoText.Text = "0"
        AmountTextBox2.Text = "0"
        creditAmtTxt.Text = "0.00"
        sudebitt.Text = "0.00"
        TextBox6.Text = "0.00"

        ' 5. Reset Combo Boxes and Payment Info
        ComboBox1.SelectedIndex = -1

        If ComboBox2metho.Items.Count > 0 Then ComboBox2metho.SelectedIndex = 0
        ' Persist current Location selection as requested (No reset to default)

        txPaymentMethod.Clear()
        dgvPaymentMethod.Visible = False

        ' Reset Print Only Checkbox
        chkPrintOnly.Checked = False

        ' 6. Internal variables
        stockqty = 0
        quty = 0
        editingTempID = ""

        ' 7. Reset UI state
        LockHeader(False)
        SupplicerShow()
        InvNoTxt.Focus()
    End Sub

    Private Sub DiscoText_TextChanged(sender As Object, e As EventArgs) Handles DiscoText.TextChanged
        CalculateSummary()
    End Sub

    Private Sub mgsumTxt_TextChanged(sender As Object, e As EventArgs) Handles mgsumTxt.TextChanged
        CalculateSummary()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If PurchGrideView.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select one or more items from the grid to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim result As DialogResult = MessageBox.Show(String.Format("Are you sure you want to delete {0} item(s)?", PurchGrideView.SelectedRows.Count), "Confirm Delete", MessageBoxButtons.YesNo)
        If result = DialogResult.Yes Then
            Try
                If MySqlConn.State <> ConnectionState.Open Then MySqlConn.Open()

                For Each row As DataGridViewRow In PurchGrideView.SelectedRows
                    Dim rowItemCode As String = Convert.ToString(row.Cells(1).Value)
                    Dim rowQty As Double = Convert.ToDouble(row.Cells(3).Value)
                    Dim rowInv As String = Convert.ToString(row.Cells(0).Value) ' Use invoice from grid directly

                    ' 1. Revert Stock (REMOVED: items.st_qty is now ONLY updated upon actually saving the invoice)

                    ' 2. Delete from Temp Table matching explicitly the InvNo AND ItemCode
                    Dim deleteQuery As String = "DELETE FROM items_stock_tempary WHERE item_id = @id AND inv_no = @inv AND (draft_user = @uname OR pc_name = @pc)"
                    Using cmdDelete As New MySqlCommand(deleteQuery, MySqlConn)
                        cmdDelete.Parameters.AddWithValue("@id", rowItemCode)
                        cmdDelete.Parameters.AddWithValue("@inv", rowInv)
                        cmdDelete.Parameters.AddWithValue("@uname", Module1.UserName)
                        cmdDelete.Parameters.AddWithValue("@pc", Environment.MachineName)
                        cmdDelete.ExecuteNonQuery()
                    End Using

                    AddProcessLog("Item Deleted", "IT Code: " & rowItemCode)
                Next
            Catch ex As Exception
                MessageBox.Show("Error during deletion: " & ex.Message)
            Finally
                If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
            End Try

            ' Refresh Grid & Summary
            load_data()
            PurchEntLoad()
            CalculateSummary()

            ' Clear Fields
            IT_CodeTextBox.Clear()
            DescriptionTextBox.Clear()
            QutTextBox.Clear()
            TextBoxItemCost.Clear()
            DiscountTextBox.Clear()
            AmountTextBox.Clear()
            TextBoxAvgCost.Clear()
            TextBox1.Clear()
            TextBox2.Clear()
            TextBox3.Clear()

            If PurchGrideView.Rows.Count = 0 Then
                InvNoTxt.Clear()
                SupplierNameTxt.Clear()
                TelNoTxt.Clear()
                LoadSupplierLimits(0)
                LockHeader(False)
            End If
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ' --- NEW LOGIC: Update Supplier Limits if no item is selected for editing ---
        If editingTempID = "" Then
            If selectedSupplierID <= 0 Then
                MessageBox.Show("Please select a supplier first.", "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            ' Update Supplier Table
            Try
                If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()
                Dim updateSql As String = "UPDATE supplier SET debit_limit = @limit, debit_period = @period WHERE id = @id"
                Using cmdUpdate As New MySqlCommand(updateSql, MySqlConn)
                    Dim newLimit As Double = 0
                    Double.TryParse(txtDebitLimitDisplay.Text.Replace(",", ""), newLimit)

                    cmdUpdate.Parameters.AddWithValue("@limit", newLimit)
                    cmdUpdate.Parameters.AddWithValue("@period", dtpDebitPeriodDisplay.Value)
                    cmdUpdate.Parameters.AddWithValue("@id", selectedSupplierID)

                    cmdUpdate.ExecuteNonQuery()

                    ' Update local tracking variables for real-time validation
                    _selectedDebitLimit = newLimit
                    _selectedDebitPeriod = dtpDebitPeriodDisplay.Value

                    MessageBox.Show("Supplier credit limits updated successfully.", "Update Successful", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    AddProcessLog("Supplier Limits Updated", "Supplier ID: " & selectedSupplierID & ", New Limit: " & newLimit.ToString("F3"))
                End Using
                MySqlConn.Close()
            Catch ex As Exception
                If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
                MessageBox.Show("Failed to update supplier limits: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
            Return
        End If

        If IT_CodeTextBox.Text = "" Then
            MessageBox.Show("Item ID cannot be empty.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Validate Prices against Item Cost
        If Not ValidatePrices() Then Return

        Try
            MySqlConn.Open()
            Dim updateQuery As String = "UPDATE `items_stock_tempary` SET `item_id` = @code, `id` = @newid, `description` = @name, `st_qty` = @qty, `qty_purchased` = @qty, " &
                "`item_cost` = @cost, `avg_cost` = @avg, `amount` = @amt, `discount` = @dis, `selling_price` = @sell, `whole_selling_price` = @wprice, " &
                "`retail_selling_price` = @rprice, `location_id` = @loc, `date` = NOW() WHERE `id` = @oldid"

            COMMAND = New MySqlCommand(updateQuery, MySqlConn)
            Dim newID As String = InvNoTxt.Text & "_" & IT_CodeTextBox.Text & "_" & Module1.UserName
            COMMAND.Parameters.AddWithValue("@code", IT_CodeTextBox.Text)
            COMMAND.Parameters.AddWithValue("@newid", newID)
            COMMAND.Parameters.AddWithValue("@name", DescriptionTextBox.Text)
            COMMAND.Parameters.AddWithValue("@qty", Val(QutTextBox.Text))
            COMMAND.Parameters.AddWithValue("@cost", Val(TextBoxItemCost.Text))
            COMMAND.Parameters.AddWithValue("@avg", Val(TextBoxAvgCost.Text))
            COMMAND.Parameters.AddWithValue("@amt", Val(AmountTextBox.Text))
            COMMAND.Parameters.AddWithValue("@dis", Val(DiscountTextBox.Text))
            COMMAND.Parameters.AddWithValue("@sell", Val(TextBox1.Text))
            COMMAND.Parameters.AddWithValue("@wprice", Val(TextBox2.Text))
            COMMAND.Parameters.AddWithValue("@rprice", Val(TextBox3.Text))
            COMMAND.Parameters.AddWithValue("@loc", ComboBoxLocation.SelectedValue)
            COMMAND.Parameters.AddWithValue("@oldid", editingTempID)

            COMMAND.ExecuteNonQuery()
            MySqlConn.Close()

            MessageBox.Show("Record Updated Paidfully", "Paid", MessageBoxButtons.OK, MessageBoxIcon.Information)

            editingTempID = ""
            PurchEntLoad()
            CalculateSummary()

            ' Clear Item Fields
            IT_CodeTextBox.Clear()
            DescriptionTextBox.Clear()
            QutTextBox.Clear()
            TextBoxItemCost.Clear()
            DiscountTextBox.Clear()
            AmountTextBox.Clear()
            TextBoxAvgCost.Clear()
            TextBox1.Clear()
            TextBox2.Clear()
            TextBox3.Clear()
            IT_CodeTextBox.Focus()

        Catch ex As Exception
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
            MessageBox.Show("Update Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub SelectItemFromGrid(rowIndex As Integer)
        If rowIndex < 0 OrElse rowIndex >= PurchGrideView.Rows.Count Then Return

        Dim k As Integer = rowIndex

        ' Disable TextChanged events while updating textboxes programmatically
        RemoveHandler IT_CodeTextBox.TextChanged, AddressOf IT_CodeTextBox_TextChanged
        RemoveHandler DescriptionTextBox.TextChanged, AddressOf DescriptionTextBox_TextChanged

        ' Set values
        editingTempID = Convert.ToString(PurchGrideView.Rows(k).Cells(7).Value)
        InvNoTxt.Text = Convert.ToString(PurchGrideView.Rows(k).Cells(0).Value)
        ' Fix: SupplierNameTxt should use index 13 (supplier_name) instead of 8 (location_id)
        If PurchGrideView.Columns.Count > 13 Then
            SupplierNameTxt.Text = Convert.ToString(PurchGrideView.Rows(k).Cells(13).Value)
        End If

        IT_CodeTextBox.Text = Convert.ToString(PurchGrideView.Rows(k).Cells(1).Value)
        DescriptionTextBox.Text = Convert.ToString(PurchGrideView.Rows(k).Cells(2).Value)

        TextBoxItemCost.Text = Convert.ToString(PurchGrideView.Rows(k).Cells(4).Value)
        QutTextBox.Text = Convert.ToString(PurchGrideView.Rows(k).Cells(3).Value)

        DiscountTextBox.Text = Convert.ToString(PurchGrideView.Rows(k).Cells(5).Value)
        AmountTextBox.Text = Convert.ToString(PurchGrideView.Rows(k).Cells(6).Value)

        ' Fill additional fields
        TextBox1.Text = Convert.ToString(PurchGrideView.Rows(k).Cells(9).Value)  ' Selling Price
        TextBox2.Text = Convert.ToString(PurchGrideView.Rows(k).Cells(10).Value) ' Whole Selling Price
        TextBox3.Text = Convert.ToString(PurchGrideView.Rows(k).Cells(11).Value) ' Retail Selling Price
        TextBoxAvgCost.Text = Convert.ToString(PurchGrideView.Rows(k).Cells(12).Value) ' Avg Cost
        ComboBoxLocation.SelectedValue = PurchGrideView.Rows(k).Cells(8).Value ' Location ID

        ' Re-enable TextChanged events
        AddHandler IT_CodeTextBox.TextChanged, AddressOf IT_CodeTextBox_TextChanged
        AddHandler DescriptionTextBox.TextChanged, AddressOf DescriptionTextBox_TextChanged

        ' Ensure the search grid stays hidden after selection
        ItemsShow.Visible = False

        ' Lookup Supplier ID
        If SupplierNameTxt.Text <> "" Then
            Try
                conn.Open()
                Dim lookupCmd As New MySqlCommand("SELECT id FROM supplier WHERE name = @sname LIMIT 1", conn)
                lookupCmd.Parameters.AddWithValue("@sname", SupplierNameTxt.Text)
                Dim result As Object = lookupCmd.ExecuteScalar()
                If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                    selectedSupplierID = CInt(result)
                End If
                conn.Close()
            Catch ex As Exception
                If conn.State = ConnectionState.Open Then conn.Close()
            End Try
        End If

        ' Get Stock info silently
        Try
            conn.Open()
            Dim table As New DataTable()
            Dim adapter As New MySqlDataAdapter("SELECT st_qty FROM items WHERE id = @id", conn)
            adapter.SelectCommand.Parameters.AddWithValue("@id", IT_CodeTextBox.Text)
            adapter.Fill(table)
            If table.Rows.Count > 0 Then
                StockTxt.Text = Convert.ToString(table.Rows(0)(0))
            End If
            conn.Close()
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try

        ' Lock header if items exist
        If PurchGrideView.Rows.Count > 0 Then
            LockHeader(True)
        End If
    End Sub

    Private Sub PurchGrideView_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles PurchGrideView.CellContentClick
        SelectItemFromGrid(PurchGrideView.CurrentRow.Index)
    End Sub

    Private Sub AmountTextBox2_TextChanged(sender As Object, e As EventArgs) Handles AmountTextBox2.TextChanged
        CalculateSummary()
    End Sub

    Private Sub PurchGrideView_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles PurchGrideView.CellClick
        SelectItemFromGrid(e.RowIndex)
    End Sub

    Private Sub lblInvNo_Click(sender As Object, e As EventArgs) Handles lblInvNo.Click
    End Sub

    Private Sub lblAmount_Click(sender As Object, e As EventArgs) Handles lblAmount.Click
    End Sub

    Private Sub sdetailsbtn_Click(sender As Object, e As EventArgs) Handles sdetailsbtn.Click
        isHistoryFromSave = False
        If historyLevel = 0 Then
            ' Opening History for the first time
            DataGridView1.Location = PurchGrideView.Location
            DataGridView1.Size = PurchGrideView.Size
            DataGridView1.Visible = True
            PurchGrideView.Visible = False
            creditSeach() ' Sets Level 1 and filters by current name/tel

            ' Now calculate and show the credit balance ONLY because the button was clicked
            Dim mgcredit As Double = 0
            For i As Integer = 0 To DataGridView1.Rows.Count - 1
                ' Column 2 is 'Due Debit amount' as per creditSeach query
                If DataGridView1.Columns.Count > 2 Then
                    Dim val As Object = DataGridView1.Rows(i).Cells(2).Value
                    If val IsNot Nothing AndAlso IsNumeric(val) Then
                        mgcredit += CDbl(val)
                    End If
                End If
            Next
            sudebitt.Text = mgcredit.ToString("F3")

            historyLevel = 1
            sdetailsbtn.Text = "Back"

        ElseIf historyLevel = 1 Then
            ' At Suppliers -> Back to Main
            ResetToNormalView()
        ElseIf historyLevel = 2 Then
            ' At Invoices -> Back to Suppliers list
            InvNoTxt.Clear()
            If InvNoTxt IsNot Nothing Then InvNoTxt.BackColor = SupplierNameTxt.BackColor
            creditSeach()
            historyLevel = 1
            sdetailsbtn.Text = "Back" ' Ensure text remains Back
        ElseIf historyLevel = 3 Then
            ' At Items list -> Back to Invoices list
            InvNoTxt.Clear()
            LoadSupplierInvoices(lastSelectedSupName)
            historyLevel = 2
            sdetailsbtn.Text = "Back" ' Ensure text remains Back
        End If
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick

    End Sub


    Private Sub Label5_Click(sender As Object, e As EventArgs)
    End Sub

    Private Sub Label4_Click(sender As Object, e As EventArgs)
    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs)
    End Sub

    Private Sub Label2_Click(sender As Object, e As EventArgs)
    End Sub

    Private Sub Label8_Click(sender As Object, e As EventArgs)
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        If SupplierNameTxt.Text.Trim().ToLower().Contains("banet") Then
            If ComboBox1.Text = "Credit" OrElse ComboBox1.Text = "Cheque" Then
                MessageBox.Show("Only Cash billing is allowed!", "Invalid Billing Type", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                ComboBox1.Text = "Cash"
                Return
            End If
        End If

        UpdateChequeUI()
        ' Hide payment method dropdown whenever billing type changes
        dgvPaymentMethod.Visible = False

        If ComboBox1.Text = "Cash" Then
            txPaymentMethod.Text = ""
            txPaymentMethod.Enabled = True
            TextBox6.Enabled = True ' Enable balance field for cash
            DiscoText.Enabled = True ' Enable discount for cash

            ' User wants to enter amount manually, so we clear it
            AmountTextBox2.Text = ""
            AmountTextBox2.Enabled = True
        ElseIf ComboBox1.Text = "Credit" Then
            txPaymentMethod.Text = "Credit"
            txPaymentMethod.Enabled = False
            TextBox6.Enabled = True ' Enable balance field for credit (to show partial balance)
            DiscoText.Enabled = True ' Enable discount field for credit
            ' Enable Paid Amount for Credit to allow partial payments (cash_Credit)
            AmountTextBox2.Enabled = True
        ElseIf ComboBox1.Text = "Cheque" Then
            txPaymentMethod.Text = "Cheque"
            txPaymentMethod.Enabled = False
            TextBox6.Enabled = True ' Enable balance field for cheque
            DiscoText.Enabled = True ' Enable discount field for cheque
            ' Enable Paid Amount for Cheque to allow partial payments (cash_Cheque)
            AmountTextBox2.Enabled = True
        End If
        CalculateSummary()
    End Sub

    Private Sub ComboBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles ComboBox1.KeyDown
        If e.KeyCode = Keys.Enter Then
            If String.IsNullOrWhiteSpace(ComboBox1.Text) Then
                ComboBox1.DroppedDown = True
                e.SuppressKeyPress = True
                Return
            End If

            If ComboBox1.Text = "Cash" Then
                txPaymentMethod.Focus()
                ShowPaymentMethodDropdown()
            Else
                AmountTextBox2.Focus()
                AmountTextBox2.SelectAll()
            End If
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub AmountTextBox2_KeyDown(sender As Object, e As KeyEventArgs) Handles AmountTextBox2.KeyDown
        If e.KeyCode = Keys.Enter Then
            Button7.PerformClick() ' Save
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub UpdateChequeUI()
        ' Removed cheque UI controls handling as controls were deleted
    End Sub

    ' --- Payment Method Search Grid Logic ---
    Private ReadOnly paymentMethods As String() = {"Cash", "Debit Card", "Credit Card", "Online Transfer", "Cheque"}

    Private Sub ShowPaymentMethodDropdown()
        Dim dt As New DataTable()
        dt.Columns.Add("Payment Method")

        Dim filter As String = txPaymentMethod.Text.Trim().ToLower()

        ' If the textbox contains a valid payment method, we allow showing all relevant options
        ' so the user can easily switch between them without manual backspacing.
        Dim isAlreadySelected As Boolean = paymentMethods.Any(Function(m) m.ToLower() = filter)

        Dim ignoreFilter As Boolean = String.IsNullOrEmpty(filter) OrElse isAlreadySelected OrElse
                                    (ComboBox1.Text = "Cash" AndAlso filter = "cash") OrElse
                                    (ComboBox1.Text = "Cheque" AndAlso filter = "cheque")

        For Each method As String In paymentMethods
            Dim shouldAdd As Boolean = False

            If ignoreFilter Then
                ' Show appropriate methods based on the Billing Type
                If ComboBox1.Text = "Cash" Then
                    ' User wants: cash, debit card, credit card, online transfer
                    If method <> "Cheque" Then shouldAdd = True
                ElseIf ComboBox1.Text = "Cheque" Then
                    If method = "Cheque" Then shouldAdd = True
                Else
                    shouldAdd = True
                End If
            Else
                ' Manual filtering when typing
                If method.ToLower().Contains(filter) Then
                    shouldAdd = True
                End If
            End If

            If shouldAdd Then
                dt.Rows.Add(method)
            End If
        Next

        If dt.Rows.Count > 0 Then
            dgvPaymentMethod.DataSource = dt
            dgvPaymentMethod.Visible = True
            dgvPaymentMethod.BringToFront()

            ' User wants the grid at a specific position as shown in the second image
            dgvPaymentMethod.Location = New Point(235, 18)
        Else
            dgvPaymentMethod.Visible = False
        End If
    End Sub

    Private Sub txPaymentMethod_TextChanged(sender As Object, e As EventArgs) Handles txPaymentMethod.TextChanged
        If txPaymentMethod.Focused Then
            ShowPaymentMethodDropdown()
        End If
        UpdateChequeUI()
    End Sub

    Private Sub txPaymentMethod_KeyDown(sender As Object, e As KeyEventArgs) Handles txPaymentMethod.KeyDown
        If e.KeyCode = Keys.Enter Then
            If dgvPaymentMethod.Visible AndAlso dgvPaymentMethod.Rows.Count > 0 Then
                dgvPaymentMethod.Focus()
            Else
                ShowPaymentMethodDropdown()
                If dgvPaymentMethod.Visible Then
                    dgvPaymentMethod.Focus()
                Else
                    AmountTextBox2.Focus()
                End If
            End If
            e.Handled = True
        ElseIf e.KeyCode = Keys.Escape Then
            dgvPaymentMethod.Visible = False
            e.Handled = True
        ElseIf e.KeyCode = Keys.Up OrElse e.KeyCode = Keys.Down Then
            If Not dgvPaymentMethod.Visible Then
                ShowPaymentMethodDropdown()
            End If
            If dgvPaymentMethod.Visible AndAlso dgvPaymentMethod.Rows.Count > 0 Then
                dgvPaymentMethod.Focus()
                Try
                    If dgvPaymentMethod.CurrentRow Is Nothing Then
                        dgvPaymentMethod.CurrentCell = dgvPaymentMethod.Rows(0).Cells(0)
                    End If
                Catch ex As Exception
                    ' Safe fallback
                End Try
            End If
            e.Handled = True
        End If
    End Sub

    Private Sub txPaymentMethod_Click(sender As Object, e As EventArgs) Handles txPaymentMethod.Click
        If ComboBox1.Text <> "Credit" Then
            ShowPaymentMethodDropdown()
        End If
    End Sub

    Private Sub txPaymentMethod_Enter(sender As Object, e As EventArgs) Handles txPaymentMethod.Enter
        If ComboBox1.Text <> "Credit" Then
            ShowPaymentMethodDropdown()
        End If
    End Sub

    Private Sub dgvPaymentMethod_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvPaymentMethod.CellClick
        If e.RowIndex >= 0 Then
            Dim selectedMethod As String = dgvPaymentMethod.Rows(e.RowIndex).Cells(0).Value.ToString()
            txPaymentMethod.Text = selectedMethod
            dgvPaymentMethod.Visible = False
            UpdateChequeUI()

            ' Auto-fill amount for Card/Online payments if in Cash billing mode
            If ComboBox1.Text = "Cash" Then
                If selectedMethod = "Debit Card" OrElse selectedMethod = "Credit Card" OrElse selectedMethod = "Online Transfer" Then
                    AmountTextBox2.Text = Fullsumtxt.Text
                End If
            End If

            AmountTextBox2.Focus()
        End If
    End Sub

    Private Sub dgvPaymentMethod_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvPaymentMethod.CellDoubleClick
        If e.RowIndex >= 0 Then
            Dim selectedMethod As String = dgvPaymentMethod.Rows(e.RowIndex).Cells(0).Value.ToString()
            txPaymentMethod.Text = selectedMethod
            dgvPaymentMethod.Visible = False
            UpdateChequeUI()

            ' Auto-fill amount for Card/Online payments if in Cash billing mode
            If ComboBox1.Text = "Cash" Then
                If selectedMethod = "Debit Card" OrElse selectedMethod = "Credit Card" OrElse selectedMethod = "Online Transfer" Then
                    AmountTextBox2.Text = Fullsumtxt.Text
                End If
            End If

            AmountTextBox2.Focus()
        End If
    End Sub

    Private Sub dgvPaymentMethod_KeyDown(sender As Object, e As KeyEventArgs) Handles dgvPaymentMethod.KeyDown
        If e.KeyCode = Keys.Enter Then
            If dgvPaymentMethod.CurrentRow IsNot Nothing Then
                Dim selectedMethod As String = dgvPaymentMethod.CurrentRow.Cells(0).Value.ToString()
                txPaymentMethod.Text = selectedMethod
                dgvPaymentMethod.Visible = False
                UpdateChequeUI()

                ' Auto-fill amount for Card/Online payments if in Cash billing mode
                If ComboBox1.Text = "Cash" Then
                    If selectedMethod = "Debit Card" OrElse selectedMethod = "Credit Card" OrElse selectedMethod = "Online Transfer" Then
                        AmountTextBox2.Text = Fullsumtxt.Text
                    End If
                End If

                AmountTextBox2.Focus()
                e.SuppressKeyPress = True
                e.Handled = True
            End If
        ElseIf e.KeyCode = Keys.Escape Then
            dgvPaymentMethod.Visible = False
            txPaymentMethod.Focus()
            e.Handled = True
        End If
    End Sub

    Private Sub dgvPaymentMethod_Leave(sender As Object, e As EventArgs) Handles dgvPaymentMethod.Leave
        dgvPaymentMethod.Visible = False
    End Sub

    ' Global click handler to hide the dropdown when clicking elsewhere
    Private Sub Global_Click_Hide_Dropdowns(sender As Object, e As EventArgs) Handles Me.Click, GroupBox1.Click, GroupBox2.Click, GroupBox3.Click, GroupBox4.Click, GroupBox5.Click, PurchGrideView.Click, DataGridView1.Click, ItemsShow.Click, SupplicerGrideView.Click
        If dgvPaymentMethod.Visible Then
            dgvPaymentMethod.Visible = False
        End If

        ' Also hide other dropdowns if they are visible
        If ItemsShow.Visible AndAlso sender IsNot IT_CodeTextBox AndAlso sender IsNot DescriptionTextBox AndAlso sender IsNot ItemsShow Then
            ItemsShow.Visible = False
        End If

        If SupplicerGrideView.Visible AndAlso sender IsNot SupplierNameTxt AndAlso sender IsNot TelNoTxt AndAlso sender IsNot SupplicerGrideView Then
            SupplicerGrideView.Visible = False
        End If
    End Sub

    Private Sub LoadBanks()
        ' Removed bank combo box loading as control was deleted
    End Sub

    Private Sub Label13_Click(sender As Object, e As EventArgs)
    End Sub

    Private Sub Labelbank_Click(sender As Object, e As EventArgs)
    End Sub


    Private Sub btnAddNew_Click(sender As Object, e As EventArgs) Handles btnAddNew.Click
        ' Only clear the item input fields to allow adding a new item to the same transaction
        IT_CodeTextBox.Clear()
        DescriptionTextBox.Clear()
        TextBoxItemCost.Clear()
        QutTextBox.Clear()
        DiscountTextBox.Clear()
        AmountTextBox.Clear()
        TextBoxAvgCost.Clear()
        TextBox1.Clear()
        TextBox2.Clear()
        TextBox3.Clear()
        StockTxt.Clear()

        ' Set focus back to Item ID input field
        IT_CodeTextBox.Focus()
    End Sub

    Private Sub AmountLabel2_Click(sender As Object, e As EventArgs)

    End Sub


    Private Sub cmsHistory_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles cmsHistory.Opening
        If DataGridView1.CurrentRow Is Nothing Then
            e.Cancel = True
            Return
        End If

        If historyLevel = 1 Then
            tsmiAddDebit.Visible = False
            tsmiAddGeneralDebit.Visible = True
            tsmiAddGeneralDebit.Text = "Add General Debit for " & DataGridView1.CurrentRow.Cells(0).Value.ToString()
        ElseIf historyLevel = 2 Then
            tsmiAddDebit.Visible = True
            tsmiAddGeneralDebit.Visible = False
            tsmiAddDebit.Text = "Add Debit for Inv: " & DataGridView1.CurrentRow.Cells(0).Value.ToString()
        Else
            e.Cancel = True
        End If
    End Sub

    Private Sub tsmiAddDebit_Click(sender As Object, e As EventArgs) Handles tsmiAddDebit.Click, tsmiAddGeneralDebit.Click
        If DataGridView1.CurrentRow Is Nothing Then Return

        Dim supName As String = ""
        Dim invNo As String = ""
        Dim defaultAmt As Double = 0

        If historyLevel = 1 Then
            supName = DataGridView1.CurrentRow.Cells(0).Value.ToString()
            invNo = "" ' General debit
            defaultAmt = 0
        ElseIf historyLevel = 2 Then
            supName = lastSelectedSupName
            invNo = DataGridView1.CurrentRow.Cells(0).Value.ToString()
            ' Grand Total is retrieved by column name for safety
            If DataGridView1.Columns.Contains("Grand Total") Then
                defaultAmt = Val(DataGridView1.CurrentRow.Cells("Grand Total").Value)
            ElseIf DataGridView1.Columns.Count > 4 Then
                defaultAmt = Val(DataGridView1.CurrentRow.Cells(4).Value)
            End If
        End If

        Dim inputAmt As String = InputBox("Enter Debit Amount for " & supName & (If(invNo <> "", " (Inv: " & invNo & ")", "")), "Manual Debit Entry", defaultAmt.ToString("F3"))

        If Not String.IsNullOrEmpty(inputAmt) AndAlso IsNumeric(inputAmt) Then
            Dim amt As Double = Val(inputAmt)
            If amt > 0 Then
                Try
                    ' 1. Lookup Supplier ID if not already known
                    Dim sId As Integer = 0
                    If historyLevel = 2 AndAlso selectedSupplierID > 0 Then
                        sId = selectedSupplierID
                    Else
                        If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()
                        Dim lookupCmd As New MySqlCommand("SELECT id FROM supplier WHERE name = @name", MySqlConn)
                        lookupCmd.Parameters.AddWithValue("@name", supName)
                        Dim res = lookupCmd.ExecuteScalar()
                        If res IsNot Nothing AndAlso Not IsDBNull(res) Then sId = Convert.ToInt32(res)
                        MySqlConn.Close()
                    End If

                    If sId = 0 Then
                        MessageBox.Show("Supplier ID not found. Please select a valid supplier.")
                        Return
                    End If

                    ' 2. Insert into supplicer_credit
                    If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()
                    Dim insertQuery As String = "INSERT INTO supplicer_credit (supplier_id, sname, inv_no, amount, getdate) VALUES (@sid, @name, @inv, @amt, @dt)"
                    Using cmdInsert As New MySqlCommand(insertQuery, MySqlConn)
                        cmdInsert.Parameters.AddWithValue("@sid", sId)
                        cmdInsert.Parameters.AddWithValue("@name", supName)
                        cmdInsert.Parameters.AddWithValue("@inv", invNo)
                        cmdInsert.Parameters.AddWithValue("@amt", amt)
                        cmdInsert.Parameters.AddWithValue("@dt", DateTime.Now)
                        cmdInsert.ExecuteNonQuery()
                    End Using
                    MySqlConn.Close()

                    MessageBox.Show("Manual Debit Recorded Successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                    ' Refresh history
                    If historyLevel = 1 Then creditSeach() Else LoadSupplierInvoices(supName)

                Catch ex As Exception
                    If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
                    MessageBox.Show("Error recording debit: " & ex.Message)
                End Try
            End If
        End If
    End Sub

    Private Sub btnPrintRequest_Click(sender As Object, e As EventArgs) Handles btnPrintRequest.Click
        PrintPurchaseRequest()
    End Sub

    Public Sub ImportPurchaseRequest(reqId As String, supId As Integer, supName As String, poNo As String, locId As Integer)
        Dim invNo As String = InvNoTxt.Text.Trim()
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()

            ' 1. Set Supplier Info
            selectedSupplierID = supId
            SupplierNameTxt.Text = supName
            ' Fetch phone number of supplier if we have it
            Using cmdTel As New MySqlCommand("SELECT tel_no FROM supplier WHERE id = @id", conn)
                cmdTel.Parameters.AddWithValue("@id", supId)
                Dim telObj = cmdTel.ExecuteScalar()
                If telObj IsNot Nothing AndAlso Not DBNull.Value.Equals(telObj) Then
                    TelNoTxt.Text = Convert.ToString(telObj)
                End If
            End Using

            ' Set location
            If ComboBoxLocation.Items.Count > 0 Then
                ComboBoxLocation.SelectedValue = locId
            End If

            ' 2. Delete existing items in temp table for this current invoice no to avoid mixing them
            Using cmdDel As New MySqlCommand("DELETE FROM items_stock_tempary WHERE inv_no = @inv AND (draft_user = @uname OR pc_name = @pc)", conn)
                cmdDel.Parameters.AddWithValue("@inv", invNo)
                cmdDel.Parameters.AddWithValue("@uname", Module1.UserName)
                cmdDel.Parameters.AddWithValue("@pc", Environment.MachineName)
                cmdDel.ExecuteNonQuery()
            End Using

            ' 3. Read items from purchase_request_items and insert them into items_stock_tempary under current invNo
            Dim selectSql As String = "SELECT item_id, description, qty, item_cost, amount, location_id FROM purchase_request_items WHERE request_id = @req"
            Dim itemsList As New List(Of Dictionary(Of String, Object))()

            Using cmdFetch As New MySqlCommand(selectSql, conn)
                cmdFetch.Parameters.AddWithValue("@req", reqId)
                Using rdr As MySqlDataReader = cmdFetch.ExecuteReader()
                    While rdr.Read()
                        Dim item As New Dictionary(Of String, Object)()
                        item("item_id") = rdr("item_id")
                        item("description") = rdr("description")
                        item("qty") = rdr("qty")
                        item("item_cost") = rdr("item_cost")
                        item("amount") = rdr("amount")
                        item("location_id") = rdr("location_id")
                        itemsList.Add(item)
                    End While
                End Using
            End Using

            ' Now insert them into items_stock_tempary
            For Each item In itemsList
                Dim itemId As String = Convert.ToString(item("item_id"))
                Dim desc As String = Convert.ToString(item("description"))
                Dim qty As Double = Convert.ToDouble(item("qty"))
                Dim cost As Double = Convert.ToDouble(item("item_cost"))
                Dim amt As Double = Convert.ToDouble(item("amount"))
                Dim itemLocId As Integer = Convert.ToInt32(item("location_id"))

                ' Fetch current selling prices & avg cost from database
                Dim sellPrice As Double = 0
                Dim wSellPrice As Double = 0
                Dim rSellPrice As Double = 0
                Dim avgCost As Double = 0

                Dim itemQuery As String = "SELECT price, w_price, r_price, cost FROM item WHERE item_id = @id LIMIT 1"
                Using cmdItem As New MySqlCommand(itemQuery, conn)
                    cmdItem.Parameters.AddWithValue("@id", itemId)
                    Using rdrItem As MySqlDataReader = cmdItem.ExecuteReader()
                        If rdrItem.Read() Then
                            sellPrice = If(IsDBNull(rdrItem("price")), 0, Convert.ToDouble(rdrItem("price")))
                            wSellPrice = If(IsDBNull(rdrItem("w_price")), 0, Convert.ToDouble(rdrItem("w_price")))
                            rSellPrice = If(IsDBNull(rdrItem("r_price")), 0, Convert.ToDouble(rdrItem("r_price")))
                            avgCost = If(IsDBNull(rdrItem("cost")), 0, Convert.ToDouble(rdrItem("cost")))
                        End If
                    End Using
                End Using

                If avgCost = 0 Then avgCost = cost

                ' Insert into items_stock_tempary
                Dim insertSql As String = "INSERT INTO items_stock_tempary " &
                    "(id, inv_no, item_id, description, st_qty, item_cost, discount, amount, location_id, supplier_id, date, selling_price, whole_selling_price, retail_selling_price, avg_cost, draft_user, pc_name) " &
                    "VALUES (@id, @inv, @item_id, @desc, @qty, @cost, 0, @amt, @loc, @sup, NOW(), @sell, @wsell, @rsell, @avg, @uname, @pc)"

                Using cmdIns As New MySqlCommand(insertSql, conn)
                    cmdIns.Parameters.AddWithValue("@id", invNo & "_" & itemId & "_" & Module1.UserName)
                    cmdIns.Parameters.AddWithValue("@inv", invNo)
                    cmdIns.Parameters.AddWithValue("@item_id", itemId)
                    cmdIns.Parameters.AddWithValue("@desc", desc)
                    cmdIns.Parameters.AddWithValue("@qty", qty)
                    cmdIns.Parameters.AddWithValue("@cost", cost)
                    cmdIns.Parameters.AddWithValue("@amt", amt)
                    cmdIns.Parameters.AddWithValue("@loc", itemLocId)
                    cmdIns.Parameters.AddWithValue("@sup", selectedSupplierID)
                    cmdIns.Parameters.AddWithValue("@sell", sellPrice)
                    cmdIns.Parameters.AddWithValue("@wsell", wSellPrice)
                    cmdIns.Parameters.AddWithValue("@rsell", rSellPrice)
                    cmdIns.Parameters.AddWithValue("@avg", avgCost)
                    cmdIns.Parameters.AddWithValue("@uname", Module1.UserName)
                    cmdIns.Parameters.AddWithValue("@pc", Environment.MachineName)
                    cmdIns.ExecuteNonQuery()
                End Using
            Next

            conn.Close()

            ' 4. Refresh grid and calculate summaries
            PurchEntLoad()
            CalculateSummary()

            MessageBox.Show("Purchase Request '" & reqId & "' successfully loaded into the grid! Feel free to review or adjust the items before saving.", "Import Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            If conn.State = ConnectionState.Open Then conn.Close()
            MessageBox.Show("Error importing purchase request: " & ex.Message, "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub PrintPurchaseRequest()
        ' Validation
        If Not isPRModeChecked Then
            MessageBox.Show("Please tick the 'Print Request' checkbox first.", "Print Request Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        If String.IsNullOrWhiteSpace(InvNoTxt.Text) Then
            MessageBox.Show("Please enter an Invoice Number.", "Validation Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        If String.IsNullOrWhiteSpace(SupplierNameTxt.Text) OrElse selectedSupplierID = 0 Then
            MessageBox.Show("Please select a supplier first.", "Validation Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        If PurchGrideView.Rows.Count = 0 Then
            MessageBox.Show("Please add items to print.", "Validation Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Auto-lookup supplier ID if it's 0 but name exists
        If selectedSupplierID = 0 AndAlso SupplierNameTxt.Text <> "" Then
            Try
                If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()
                Dim lookupQuery As String = "SELECT id FROM supplier WHERE name = @sname LIMIT 1"
                Dim lookupCmd As New MySqlCommand(lookupQuery, MySqlConn)
                lookupCmd.Parameters.AddWithValue("@sname", SupplierNameTxt.Text)
                Dim result As Object = lookupCmd.ExecuteScalar()
                If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                    selectedSupplierID = CInt(result)
                End If
            Catch ex As Exception
            End Try
        End If

        ' Set P/O Number based on chkPRMode or use empty
        Dim poNo As String = ""
        If isPRModeChecked Then
            poNo = InvNoTxt.Text.Trim()
            If IsPRNumberDuplicate(poNo) Then
                MessageBox.Show("This P/O Number already exists! Please enter a unique P/O Number.", "Duplicate P/O Number", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                InvNoTxt.Focus()
                InvNoTxt.SelectAll()
                Return
            End If
        End If

        Dim requestId As String = InvNoTxt.Text.Trim()
        Dim itemsQty As Integer = 0
        For i As Integer = 0 To PurchGrideView.Rows.Count - 1
            itemsQty += Val(PurchGrideView.Rows(i).Cells(3).Value)
        Next

        Dim gTotal As Decimal = 0
        Decimal.TryParse(Fullsumtxt.Text.Replace(",", ""), gTotal)

        Try
            If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()

            ' 1. Delete any existing request with this ID to prevent primary key conflicts on overwrite/reprint
            Using cmdDelItems As New MySqlCommand("DELETE FROM purchase_request_items WHERE request_id = @id", MySqlConn)
                cmdDelItems.Parameters.AddWithValue("@id", requestId)
                cmdDelItems.ExecuteNonQuery()
            End Using

            Using cmdDelHeader As New MySqlCommand("DELETE FROM purchase_request WHERE request_id = @id", MySqlConn)
                cmdDelHeader.Parameters.AddWithValue("@id", requestId)
                cmdDelHeader.ExecuteNonQuery()
            End Using

            ' 2. Save Request Summary Header permanently
            Dim mainLocationId As Integer = 1
            If ComboBoxLocation.SelectedValue IsNot Nothing AndAlso IsNumeric(ComboBoxLocation.SelectedValue) Then
                mainLocationId = Convert.ToInt32(ComboBoxLocation.SelectedValue)
            End If

            Dim insertHeaderSql As String = "INSERT INTO purchase_request (request_id, supplier_id, supplier_name, items_qty, total_amount, request_date, pur_date, status, location_id) " &
                "VALUES (@r_id, @s_id, @s_name, @qty, @total, @dt, @pur_dt, 'Draft', @loc)"
            Using cmdHeader As New MySqlCommand(insertHeaderSql, MySqlConn)
                cmdHeader.Parameters.AddWithValue("@r_id", requestId)
                cmdHeader.Parameters.AddWithValue("@s_id", selectedSupplierID)
                cmdHeader.Parameters.AddWithValue("@s_name", SupplierNameTxt.Text.Trim())
                cmdHeader.Parameters.AddWithValue("@qty", itemsQty)
                cmdHeader.Parameters.AddWithValue("@total", gTotal)
                cmdHeader.Parameters.AddWithValue("@dt", DateTime.Now)
                cmdHeader.Parameters.AddWithValue("@pur_dt", DateTimePicker1.Value)
                cmdHeader.Parameters.AddWithValue("@loc", mainLocationId)
                cmdHeader.ExecuteNonQuery()
            End Using

            ' 3. Save Request Item Details permanently
            For i As Integer = 0 To PurchGrideView.Rows.Count - 1
                Dim itemId As String = Convert.ToString(PurchGrideView.Rows(i).Cells(1).Value)
                Dim qty As Double = Val(PurchGrideView.Rows(i).Cells(3).Value)
                Dim locId As Integer = Val(PurchGrideView.Rows(i).Cells(8).Value)
                If locId = 0 Then locId = 1 ' Default fallback
                Dim itmCost As Double = Val(PurchGrideView.Rows(i).Cells(4).Value)
                Dim amt As Double = Val(PurchGrideView.Rows(i).Cells(6).Value)
                Dim desc As String = Convert.ToString(PurchGrideView.Rows(i).Cells(2).Value)
                Dim rowId As String = Guid.NewGuid().ToString().Substring(0, 10) ' 10-char random key

                Dim insertItemSql As String = "INSERT INTO purchase_request_items (id, request_id, item_id, description, qty, item_cost, amount, location_id, date) " &
                    "VALUES (@id, @r_id, @i_id, @desc, @qty, @cost, @amt, @loc, @dt)"
                Using cmdItem As New MySqlCommand(insertItemSql, MySqlConn)
                    cmdItem.Parameters.AddWithValue("@id", rowId)
                    cmdItem.Parameters.AddWithValue("@r_id", requestId)
                    cmdItem.Parameters.AddWithValue("@i_id", itemId)
                    cmdItem.Parameters.AddWithValue("@desc", desc)
                    cmdItem.Parameters.AddWithValue("@qty", qty)
                    cmdItem.Parameters.AddWithValue("@cost", itmCost)
                    cmdItem.Parameters.AddWithValue("@amt", amt)
                    cmdItem.Parameters.AddWithValue("@loc", locId)
                    cmdItem.Parameters.AddWithValue("@dt", DateTime.Now)
                    cmdItem.ExecuteNonQuery()
                End Using
            Next

            ' Delete temporary items from items_stock_tempary for this request to clear them from client-side temp state
            Using cmdClearTemp As New MySqlCommand("DELETE FROM items_stock_tempary WHERE inv_no = @inv AND (draft_user = @uname OR pc_name = @pc)", MySqlConn)
                cmdClearTemp.Parameters.AddWithValue("@inv", requestId)
                cmdClearTemp.Parameters.AddWithValue("@uname", Module1.UserName)
                cmdClearTemp.Parameters.AddWithValue("@pc", Environment.MachineName)
                cmdClearTemp.ExecuteNonQuery()
            End Using

            If Not String.IsNullOrEmpty(originalInvNo) Then
                Using cmdClearTemp2 As New MySqlCommand("DELETE FROM items_stock_tempary WHERE inv_no = @inv AND (draft_user = @uname OR pc_name = @pc)", MySqlConn)
                    cmdClearTemp2.Parameters.AddWithValue("@inv", originalInvNo)
                    cmdClearTemp2.Parameters.AddWithValue("@uname", Module1.UserName)
                    cmdClearTemp2.Parameters.AddWithValue("@pc", Environment.MachineName)
                    cmdClearTemp2.ExecuteNonQuery()
                End Using
            End If

            MySqlConn.Close()

            MessageBox.Show("Purchase Request permanently saved to database.", "Saved Successfully", MessageBoxButtons.OK, MessageBoxIcon.Information)

            ' 4. Open the report viewer using index 13 (Purchase Request)
            Dim rptForm As New SaleInv()
            rptForm.ShowReport(requestId, 13)

            ' 5. Clear all form fields and untick PR Mode after successfully saving and printing the request
            isPRModeChecked = False
            originalInvNo = ""
            chkPRMode.Invalidate()
            TogglePRMode(False)
            ClearAllFields()
        Catch ex As Exception
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
            MessageBox.Show("Error generating purchase request report: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Button4_Click_1(sender As Object, e As EventArgs) Handles Button4.Click
        ' Delete temporary items from items_stock_tempary for the active invoice before clearing it
        Dim currentInv As String = InvNoTxt.Text.Trim()
        If Not String.IsNullOrEmpty(currentInv) Then
            Try
                If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()
                Using cmdClear As New MySqlCommand("DELETE FROM items_stock_tempary WHERE inv_no = @inv AND (draft_user = @uname OR pc_name = @pc)", MySqlConn)
                    cmdClear.Parameters.AddWithValue("@inv", currentInv)
                    cmdClear.Parameters.AddWithValue("@uname", Module1.UserName)
                    cmdClear.Parameters.AddWithValue("@pc", Environment.MachineName)
                    cmdClear.ExecuteNonQuery()
                End Using
                If Not String.IsNullOrEmpty(originalInvNo) Then
                    Using cmdClear2 As New MySqlCommand("DELETE FROM items_stock_tempary WHERE inv_no = @inv AND (draft_user = @uname OR pc_name = @pc)", MySqlConn)
                        cmdClear2.Parameters.AddWithValue("@inv", originalInvNo)
                        cmdClear2.Parameters.AddWithValue("@uname", Module1.UserName)
                        cmdClear2.Parameters.AddWithValue("@pc", Environment.MachineName)
                        cmdClear2.ExecuteNonQuery()
                    End Using
                End If
                MySqlConn.Close()
            Catch ex As Exception
                If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
            End Try
        End If

        ' Reset PR Mode tick box and clear the entire form
        isPRModeChecked = False
        originalInvNo = ""
        chkPRMode.Invalidate()
        TogglePRMode(False)
        ClearAllFields()
    End Sub

    Private Sub chkPrintOnly_CheckedChanged(sender As Object, e As EventArgs) Handles chkPrintOnly.CheckedChanged
        If chkPrintOnly.Checked Then
            ComboBox1.Text = "Cash"
            ComboBox1.Enabled = False
            txPaymentMethod.Text = "Cash"
            txPaymentMethod.Enabled = False
        Else
            ComboBox1.Enabled = True
            txPaymentMethod.Enabled = True
        End If
    End Sub

    Private Function EscapeRowFilter(val As String) As String
        If String.IsNullOrEmpty(val) Then Return ""
        Dim escaped As String = val
        ' Escape only the open bracket '[', which is all ADO.NET RowFilter requires to prevent bracket wildcard parsing errors.
        ' Escaping ']' as '[]]' after replacing '[' can corrupt the bracket syntax.
        escaped = escaped.Replace("[", "[[]")
        escaped = escaped.Replace("*", "[*]").Replace("%", "[%]").Replace("#", "[#]")
        Return escaped
    End Function
End Class
