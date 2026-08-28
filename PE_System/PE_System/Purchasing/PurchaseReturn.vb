Imports MySql.Data.MySqlClient

Public Class frmPurchaseReturn
    Dim COMMAND As MySqlCommand
    Dim READER As MySqlDataReader
    Dim oldqty As Integer
    Dim Invno As Integer
    Dim isSearchMode As Boolean = False
    Dim isFillingFields As Boolean = False ' Flag to prevent event loops while auto-filling
    Dim historyLevel As Integer = 0 ' 0: Normal, 1: Suppliers, 2: Invoices, 3: Items
    Dim lastSelectedSupName As String = ""
    Dim lastSelectedInvNo As String = ""
    Dim selectedSupplierID As Integer = 0
    Dim selectedBankID As Integer = 0
    Dim selectedCqeNo As String = ""
    Dim originalItemsQty As Integer = 0
    Dim originalCost As Double = 0
    Dim isReturnProcessed As Boolean = False ' Flag to ensure Return is clicked before Save
    Dim WithEvents txtHistorySupName As TextBox
    Dim WithEvents txtHistoryTeleNo As TextBox
    Dim lblHistorySupName As Label
    Dim lblHistoryTeleNo As Label
    Dim lastSelectedTeleNo As String = ""
    Dim lblHistorySelSupName As Label
    Dim txtHistorySelSupName As TextBox
    Dim lblHistorySelTeleNo As Label
    Dim txtHistorySelTeleNo As TextBox
    
    Structure StagedReturn
        Dim ItemId As String
        Dim Description As String
        Dim QtyToReturn As Integer
        Dim Reason As String
        Dim UnitCost As Double
        Dim DiscPercent As Double
        Dim NetItemRtnAmt As Double
        Dim GrandRtnAmt As Double
        Dim OldInvQty As Integer
        Dim LocationID As Integer
    End Structure
    
    Dim stagedReturns As New List(Of StagedReturn)

    Private Sub setup_grid_style(dgv As DataGridView)
        dgv.AllowUserToAddRows = False
        dgv.AllowUserToDeleteRows = False
        dgv.ReadOnly = True
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgv.RowHeadersVisible = False
        dgv.BackgroundColor = SystemColors.ButtonFace
        dgv.DefaultCellStyle.Font = New Font("Microsoft Sans Serif", 14)
        dgv.ColumnHeadersDefaultCellStyle.Font = New Font("Microsoft Sans Serif", 16, FontStyle.Bold)
        dgv.EnableHeadersVisualStyles = True
        dgv.ScrollBars = ScrollBars.Both
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None
    End Sub

    ' Fetches current stock from items table using a local connection to avoid conflicts
    Private Function GetCurrentStock(itemId As String) As Integer
        Dim stock As Integer = 0
        Using localConn As New MySqlConnection(ConnStr)
            Try
                localConn.Open()
                Dim query As String = "SELECT st_qty FROM items WHERE id = @id"
                Using cmd As New MySqlCommand(query, localConn)
                    cmd.Parameters.AddWithValue("@id", itemId)
                    Dim result = cmd.ExecuteScalar()
                    If result IsNot Nothing Then stock = Val(result)
                End Using
            Catch ex As Exception
                ' Silent fail
            End Try
        End Using
        Return stock
    End Function

    ' Load all items for a specific invoice into the grid
    Private Sub LoadInvoiceItems(invNo As String, Optional supplierId As Integer = 0)
        Using localConn As New MySqlConnection(ConnStr)
            Try
                localConn.Open()
                Dim table As New DataTable()
                ' Fetching items from items_stock - Order: item_id, description, item_cost, sell_price, whole_price, retail_price, avg_cost, st_qty, discount, amount
                Dim query As String = "SELECT i.item_id, i.description, i.item_cost, i.selling_price, i.whole_selling_price, i.retail_selling_price, i.avg_cost, i.st_qty, l.location_name as 'Location', i.discount, i.amount, i.location_id " &
                                     "FROM items_stock i LEFT JOIN location l ON i.location_id = l.id WHERE i.inv_no = @inv"
                If supplierId > 0 Then
                    query &= " AND i.supplier_id = @supId"
                End If
                query &= " ORDER BY i.item_id ASC"
                Using adapter As New MySqlDataAdapter(query, localConn)
                    adapter.SelectCommand.Parameters.AddWithValue("@inv", invNo)
                    If supplierId > 0 Then
                        adapter.SelectCommand.Parameters.AddWithValue("@supId", supplierId)
                    End If
                    adapter.Fill(table)
                End Using

                DataGridView1.DataSource = table
                setup_grid_style(DataGridView1)

                ' Headers and Widths optimized for grid size
                If DataGridView1.Columns.Count >= 11 Then
                    DataGridView1.Columns(0).HeaderText = "Item ID" : DataGridView1.Columns(0).Width = 120
                    DataGridView1.Columns(1).HeaderText = "Description" : DataGridView1.Columns(1).Width = 400
                    DataGridView1.Columns(2).HeaderText = "Cost" : DataGridView1.Columns(2).Width = 100 : DataGridView1.Columns(2).DefaultCellStyle.Format = "N3"
                    DataGridView1.Columns(3).HeaderText = "Sell Price" : DataGridView1.Columns(3).Width = 140 : DataGridView1.Columns(3).DefaultCellStyle.Format = "N3"
                    DataGridView1.Columns(4).HeaderText = "W. Price" : DataGridView1.Columns(4).Width = 130 : DataGridView1.Columns(4).DefaultCellStyle.Format = "N3"
                    DataGridView1.Columns(5).HeaderText = "R. Price" : DataGridView1.Columns(5).Width = 130 : DataGridView1.Columns(5).DefaultCellStyle.Format = "N3"
                    DataGridView1.Columns(6).HeaderText = "Avg Cost" : DataGridView1.Columns(6).Width = 140 : DataGridView1.Columns(6).DefaultCellStyle.Format = "N3"
                    DataGridView1.Columns(7).HeaderText = "Qty" : DataGridView1.Columns(7).Width = 75
                    DataGridView1.Columns(8).HeaderText = "Location" : DataGridView1.Columns(8).Width = 150
                    DataGridView1.Columns(9).HeaderText = "Disc" : DataGridView1.Columns(9).Width = 75
                    DataGridView1.Columns(10).HeaderText = "Amount" : DataGridView1.Columns(10).Width = 115 : DataGridView1.Columns(10).DefaultCellStyle.Format = "N3"
                    If DataGridView1.Columns.Count >= 12 Then
                        DataGridView1.Columns(11).Visible = False ' Hide location_id
                    End If
                End If

                calNetAmount()
            Catch ex As Exception
                ' Silent fail
            End Try
        End Using
    End Sub

    Private Sub creditSeach()
        Try
            Using localConn As New MySqlConnection(ConnStr)
                localConn.Open()
                Dim table As New DataTable()
                Dim query As String = "SELECT DISTINCT s.name as 'Supplier Name', s.tel_no as 'Telephone', " &
                                     "IFNULL((SELECT SUM(balance_due) FROM purchasing WHERE supplier_id = s.id), 0) as 'Credit Amount' " &
                                     "FROM supplier s " &
                                     "INNER JOIN purchase_return pr ON s.id = pr.supplier_id ORDER BY s.name ASC"
                Dim adapter As New MySqlDataAdapter(query, localConn)
                adapter.Fill(table)

                Dim dv As New DataView(table)
                Dim filter As String = ""
                If txtHistorySupName IsNot Nothing AndAlso Not String.IsNullOrEmpty(txtHistorySupName.Text) Then
                    filter = String.Format("[Supplier Name] Like '{0}%'", txtHistorySupName.Text.Replace("'", "''"))
                End If
                If txtHistoryTeleNo IsNot Nothing AndAlso Not String.IsNullOrEmpty(txtHistoryTeleNo.Text) Then
                    Dim telFilter As String = String.Format("Telephone Like '{0}%'", txtHistoryTeleNo.Text.Replace("'", "''"))
                    filter = If(filter = "", telFilter, filter & " AND " & telFilter)
                End If

                dv.RowFilter = filter
                DataGridView1.DataSource = dv
                setup_grid_style(DataGridView1)
                DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            End Using

            If historyLevel > 1 AndAlso (CNameTxt.Focused OrElse TelNoTxt.Focused) Then
                historyLevel = 1
                DataGridView1.Visible = True
                sdetailsbtn.Text = "Back"
            End If
        Catch ex As Exception
            ' Silent fail
        End Try
    End Sub

    Private Sub LoadSupplierInvoices_History(supplierName As String, Optional telephone As String = "")
        Try
            Dim isTransitioning As Boolean = (historyLevel = 1)
            If Not String.IsNullOrEmpty(telephone) Then
                lastSelectedTeleNo = telephone
            End If
            If txtHistorySelSupName IsNot Nothing Then txtHistorySelSupName.Text = supplierName
            If txtHistorySelTeleNo IsNot Nothing Then txtHistorySelTeleNo.Text = lastSelectedTeleNo

            Using localConn As New MySqlConnection(ConnStr)
                localConn.Open()
                Dim table As New DataTable()
                Dim query As String = "SELECT pr.pur_id as 'Invoice No', " &
                                     "COALESCE(p.items_qty, 0) as 'Total Qty', " &
                                     "SUM(pr.Rtn_Qty) as 'Total Rtn Qty', " &
                                     "MAX(pr.cash_return) as 'Cash Rtn', " &
                                     "COALESCE(p.paid_amount, 0.00) as 'Paid Amount', " &
                                     "COALESCE(p.sub_total, 0.00) as 'Total Amount', " &
                                     "COALESCE(p.cost, 0.00) as 'Grand Total', " &
                                     "MAX(pr.return_date) as 'Rtn Date', " &
                                     "p.pur_date as 'pur_Date' " &
                                     "FROM purchase_return pr " &
                                     "LEFT JOIN purchasing p ON pr.pur_id = p.pur_id " &
                                     "JOIN supplier s ON pr.supplier_id = s.id " &
                                     "WHERE s.name = @sup " &
                                     "GROUP BY pr.pur_id, p.items_qty, p.paid_amount, p.sub_total, p.cost, p.pur_date " &
                                     "ORDER BY pr.pur_id ASC"
                Dim adapter As New MySqlDataAdapter(query, localConn)
                adapter.SelectCommand.Parameters.AddWithValue("@sup", supplierName)
                adapter.Fill(table)

                Dim dv As New DataView(table)
                If txtHistorySupName IsNot Nothing AndAlso Not String.IsNullOrEmpty(txtHistorySupName.Text) AndAlso Not isTransitioning Then
                    dv.RowFilter = String.Format("[Invoice No] Like '{0}%'", txtHistorySupName.Text.Replace("'", "''"))
                End If
                dv.Sort = "[Invoice No] ASC"

                DataGridView1.DataSource = dv
                setup_grid_style(DataGridView1)
                DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None

                ' Headers and Widths strictly matching requested columns
                If DataGridView1.Columns.Count >= 9 Then
                    DataGridView1.Columns(0).HeaderText = "Invoice No" : DataGridView1.Columns(0).Width = 140
                    DataGridView1.Columns(1).HeaderText = "Total Qty" : DataGridView1.Columns(1).Width = 120
                    DataGridView1.Columns(2).HeaderText = "Total Rtn Qty" : DataGridView1.Columns(2).Width = 150
                    DataGridView1.Columns(3).HeaderText = "Cash Rtn" : DataGridView1.Columns(3).Width = 140 : DataGridView1.Columns(3).DefaultCellStyle.Format = "N3"
                    DataGridView1.Columns(4).HeaderText = "Paid Amount" : DataGridView1.Columns(4).Width = 150 : DataGridView1.Columns(4).DefaultCellStyle.Format = "N3"
                    DataGridView1.Columns(5).HeaderText = "Total Amount" : DataGridView1.Columns(5).Width = 150 : DataGridView1.Columns(5).DefaultCellStyle.Format = "N3"
                    DataGridView1.Columns(6).HeaderText = "Grand Total" : DataGridView1.Columns(6).Width = 150 : DataGridView1.Columns(6).DefaultCellStyle.Format = "N3"
                    DataGridView1.Columns(7).HeaderText = "Rtn Date" : DataGridView1.Columns(7).Width = 200
                    DataGridView1.Columns(8).HeaderText = "pur_Date" : DataGridView1.Columns(8).Width = 200
                End If
            End Using
            lastSelectedSupName = supplierName
            historyLevel = 2
            sdetailsbtn.Text = "Back"
            ToggleMiddleFieldsVisibility(False)

            If isTransitioning Then
                If txtHistorySupName IsNot Nothing Then
                    txtHistorySupName.Clear()
                    txtHistorySupName.Focus()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub LoadInvoiceItems_History(invoiceNo As String)
        Try
            Using localConn As New MySqlConnection(ConnStr)
                localConn.Open()
                Dim table As New DataTable()
                Dim query As String = "SELECT " &
                                     "COALESCE((SELECT DISTINCT item_id FROM items_stock WHERE inv_no = pr.pur_id AND description = pr.description LIMIT 1), (SELECT DISTINCT id FROM items WHERE item_name = pr.description LIMIT 1), 'N/A') as 'Item ID', " &
                                     "pr.description as 'Description', " &
                                     "pr.Rtn_Qty as 'Rtn Qty', " &
                                     "COALESCE((SELECT DISTINCT item_cost FROM items_stock WHERE inv_no = pr.pur_id AND description = pr.description LIMIT 1), (SELECT DISTINCT item_cost FROM items WHERE item_name = pr.description LIMIT 1), 0.00) * pr.Rtn_Qty as 'Total Cost', " &
                                     "pr.reason as 'Reason', " &
                                     "pr.return_amt as 'Return Amt', " &
                                     "COALESCE((SELECT its.discount FROM items_stock its WHERE its.inv_no = pr.pur_id AND its.description = pr.description LIMIT 1), 0.00) as 'Disc', " &
                                     "COALESCE((SELECT l.location_name FROM items_stock its LEFT JOIN location l ON its.location_id = l.id WHERE its.inv_no = pr.pur_id AND its.description = pr.description LIMIT 1), 'N/A') as 'Location' " &
                                     "FROM purchase_return pr " &
                                     "WHERE pr.pur_id = @inv"
                Dim adapter As New MySqlDataAdapter(query, localConn)
                adapter.SelectCommand.Parameters.AddWithValue("@inv", invoiceNo)
                adapter.Fill(table)

                DataGridView1.DataSource = table
                setup_grid_style(DataGridView1)
                DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None

                If DataGridView1.Columns.Count >= 8 Then
                    DataGridView1.Columns(0).HeaderText = "Item ID" : DataGridView1.Columns(0).Width = 120
                    DataGridView1.Columns(1).HeaderText = "Description" : DataGridView1.Columns(1).Width = 350
                    DataGridView1.Columns(2).HeaderText = "Rtn Qty" : DataGridView1.Columns(2).Width = 90
                    DataGridView1.Columns(3).HeaderText = "Total Cost" : DataGridView1.Columns(3).Width = 120 : DataGridView1.Columns(3).DefaultCellStyle.Format = "N3"
                    DataGridView1.Columns(4).HeaderText = "Reason" : DataGridView1.Columns(4).Width = 220
                    DataGridView1.Columns(5).HeaderText = "Return Amt" : DataGridView1.Columns(5).Width = 140 : DataGridView1.Columns(5).DefaultCellStyle.Format = "N3"
                    DataGridView1.Columns(6).HeaderText = "Disc" : DataGridView1.Columns(6).Width = 80
                    DataGridView1.Columns(7).HeaderText = "Location" : DataGridView1.Columns(7).Width = 140
                End If
            End Using
            lastSelectedInvNo = invoiceNo
            historyLevel = 3
            DataGridView1.Visible = True
            sdetailsbtn.Text = "Back"
            ToggleMiddleFieldsVisibility(False)
            If txtHistorySupName IsNot Nothing Then
                txtHistorySupName.Text = invoiceNo
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub FetchInvoiceDetails(invNo As String, Optional supplierName As String = "")
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()
            Dim query As String = "SELECT p.*, s.name, s.tel_no FROM purchasing p JOIN supplier s ON p.supplier_id = s.id WHERE p.pur_id = @inv"
            If Not String.IsNullOrEmpty(supplierName) Then
                query &= " AND s.name = @name"
            End If
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@inv", invNo)
                If Not String.IsNullOrEmpty(supplierName) Then
                    cmd.Parameters.AddWithValue("@name", supplierName)
                End If
                Using dr As MySqlDataReader = cmd.ExecuteReader()
                    If dr.Read() Then
                        InvTxt.Text = dr("pur_id").ToString()
                        CNameTxt.Text = dr("name").ToString()
                        TelNoTxt.Text = dr("tel_no").ToString()
 
                        If Not IsDBNull(dr("pur_date")) Then
                            SaleDateTime.Format = DateTimePickerFormat.Short
                            SaleDateTime.Value = Convert.ToDateTime(dr("pur_date"))
                        End If
 
                        NetAmountTxt.Text = dr("sub_total").ToString()
                        GrandTotalTxt.Text = dr("cost").ToString()
                        TextBox3.Text = dr("balance_due").ToString()
                        DiscoText.Text = dr("inv_dis").ToString()
                        DiscountAmountTxt.Text = "0.00"
                        TextBox7.Clear()
                        CashRtnTxt.Text = "0.00"
                        ComboBox1.Text = dr("pur_type").ToString()
                        txPaymentMethod.Text = dr("p_method").ToString()
                        AmountTextBox2.Text = dr("paid_amount").ToString()
                        ComboBox2metho.Text = dr("pur_su_method").ToString()
                        selectedSupplierID = Val(dr("supplier_id").ToString())
                        selectedBankID = If(IsDBNull(dr("bank_id")), 0, Val(dr("bank_id").ToString()))
                        selectedCqeNo = If(IsDBNull(dr("cqe_no")), "", dr("cqe_no").ToString())
                        originalItemsQty = Val(dr("items_qty").ToString())
                        originalCost = Val(dr("cost").ToString())
                        dr.Close()
                        LoadInvoiceItems(invNo, selectedSupplierID)
                        Panel1.Visible = True
                        DataGridView1.Visible = True
                    Else
                        dr.Close()
                        MessageBox.Show("Invoice not found.")
                    End If
                End Using
            End Using
            conn.Close()
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then conn.Close()
            MessageBox.Show("Error: " & ex.Message)
        End Try
    End Sub

    Private Sub calNetAmount()
        Dim summ As Double = 0
        For q As Integer = 0 To DataGridView1.Rows.Count - 1
            If DataGridView1.Columns.Count > 10 Then
                If DataGridView1.Rows(q).Cells(10).Value IsNot Nothing AndAlso IsNumeric(DataGridView1.Rows(q).Cells(10).Value) Then
                    summ += CDbl(DataGridView1.Rows(q).Cells(10).Value)
                End If
            ElseIf DataGridView1.Columns.Count = 6 Then
                If DataGridView1.Rows(q).Cells(5).Value IsNot Nothing AndAlso IsNumeric(DataGridView1.Rows(q).Cells(5).Value) Then
                    summ += CDbl(DataGridView1.Rows(q).Cells(5).Value)
                End If
            End If
        Next
        NetAmountTxt.Text = summ.ToString("F3")
        Dim discountPercent As Double = Val(DiscoText.Text)
        Dim invoiceDiscountAmt As Double = (summ * discountPercent) / 100
        Dim grandTotal As Double = summ - invoiceDiscountAmt
        GrandTotalTxt.Text = grandTotal.ToString("F3")
        
        ' Update Debit Balance UI
        Dim paidAmt As Double = Val(AmountTextBox2.Text)
        Dim balDue As Double = grandTotal - paidAmt
        If balDue < 0 Then balDue = 0
        TextBox3.Text = balDue.ToString("F3")
    End Sub

    Private Sub PurchReturn_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            Me.KeyPreview = True
            SaleDateTime.Format = DateTimePickerFormat.Custom
            SaleDateTime.CustomFormat = " "
            setup_grid_style(DataGridView1)
            Using localConn As New MySqlConnection(ConnStr)
                localConn.Open()
                ' Populate Billing Type
                ComboBox1.Items.Clear()
                Dim cmdEnum As New MySqlCommand("SHOW COLUMNS FROM purchasing LIKE 'pur_type'", localConn)
                Using drEnum As MySqlDataReader = cmdEnum.ExecuteReader()
                    If drEnum.Read() Then
                        Dim enumStr As String = drEnum("Type").ToString()
                        enumStr = enumStr.Replace("enum(", "").Replace(")", "").Replace("'", "")
                        For Each v In enumStr.Split(","c) : ComboBox1.Items.Add(v.Trim()) : Next
                    End If
                End Using
                ' Populate Supply Method
                ComboBox2metho.Items.Clear()
                Dim cmdEnumMeth As New MySqlCommand("SHOW COLUMNS FROM purchasing LIKE 'pur_su_method'", localConn)
                Using drEnumMeth As MySqlDataReader = cmdEnumMeth.ExecuteReader()
                    If drEnumMeth.Read() Then
                        Dim enumStrM As String = drEnumMeth("Type").ToString()
                        enumStrM = enumStrM.Replace("enum(", "").Replace(")", "").Replace("'", "")
                        For Each v In enumStrM.Split(","c) : ComboBox2metho.Items.Add(v.Trim()) : Next
                    End If
                End Using
                ' Locations
                Dim dtLoc As New DataTable()
                Dim adpLoc As New MySqlDataAdapter("SELECT id, location_name FROM location", localConn)
                adpLoc.Fill(dtLoc)
                ComboBoxLocation.DataSource = dtLoc
                ComboBoxLocation.DisplayMember = "location_name"
                ComboBoxLocation.ValueMember = "id"
                ComboBoxLocation.SelectedIndex = -1
            End Using
            NetAmountTxt.Text = "0.00" : GrandTotalTxt.Text = "0.00" : TextBox3.Text = "0.00" : CashRtnTxt.Text = "0.00" : TextBox7.Text = "0.00"
            ' Reason Init
            ComboBoxreson.SelectedIndex = 0
            DescriptionTxt.Visible = False
            
            Panel1.Visible = True : isSearchMode = False : DataGridView1.Visible = True
            sdetailsbtn.Text = "R Supplier"

            ' Initialize History Search Controls
            txtHistorySupName = New TextBox() With {
                .Name = "txtHistorySupName",
                .BackColor = Color.FromArgb(200, 230, 255),
                .Font = New Font("Microsoft Sans Serif", 12.0!),
                .Location = New Point(160, 70),
                .Size = New Size(280, 39),
                .Visible = False
            }
            AddHandler txtHistorySupName.TextChanged, AddressOf txtHistorySupName_TextChanged
            AddHandler txtHistorySupName.KeyDown, AddressOf txtHistorySupName_KeyDown

            txtHistoryTeleNo = New TextBox() With {
                .Name = "txtHistoryTeleNo",
                .BackColor = Color.FromArgb(200, 230, 255),
                .Font = New Font("Microsoft Sans Serif", 12.0!),
                .Location = New Point(560, 70),
                .Size = New Size(250, 39),
                .Visible = False
            }
            AddHandler txtHistoryTeleNo.TextChanged, AddressOf txtHistoryTeleNo_TextChanged
            AddHandler txtHistoryTeleNo.KeyDown, AddressOf txtHistoryTeleNo_KeyDown

            lblHistorySupName = New Label() With {
                .Name = "lblHistorySupName",
                .Text = "Supplier Name:",
                .Font = New Font("Segoe UI", 11.0!, FontStyle.Bold),
                .ForeColor = Color.Yellow,
                .Location = New Point(14, 76),
                .Size = New Size(200, 36),
                .AutoSize = True,
                .Visible = False
            }

            lblHistoryTeleNo = New Label() With {
                .Name = "lblHistoryTeleNo",
                .Text = "Tele No:",
                .Font = New Font("Segoe UI", 11.0!, FontStyle.Bold),
                .ForeColor = Color.Yellow,
                .Location = New Point(480, 76),
                .Size = New Size(150, 36),
                .AutoSize = True,
                .Visible = False
            }

            lblHistorySelSupName = New Label() With {
                .Name = "lblHistorySelSupName",
                .Text = "Supplier Name:",
                .Font = New Font("Segoe UI", 11.0!, FontStyle.Bold),
                .ForeColor = Color.Yellow,
                .Location = New Point(460, 76),
                .Size = New Size(120, 36),
                .AutoSize = True,
                .Visible = False
            }

            txtHistorySelSupName = New TextBox() With {
                .Name = "txtHistorySelSupName",
                .BackColor = Color.FromArgb(200, 230, 255),
                .Font = New Font("Microsoft Sans Serif", 12.0!),
                .Location = New Point(585, 70),
                .Size = New Size(260, 39),
                .ReadOnly = True,
                .Visible = False
            }

            lblHistorySelTeleNo = New Label() With {
                .Name = "lblHistorySelTeleNo",
                .Text = "Tele No:",
                .Font = New Font("Segoe UI", 11.0!, FontStyle.Bold),
                .ForeColor = Color.Yellow,
                .Location = New Point(860, 76),
                .Size = New Size(80, 36),
                .AutoSize = True,
                .Visible = False
            }

            txtHistorySelTeleNo = New TextBox() With {
                .Name = "txtHistorySelTeleNo",
                .BackColor = Color.FromArgb(200, 230, 255),
                .Font = New Font("Microsoft Sans Serif", 12.0!),
                .Location = New Point(940, 70),
                .Size = New Size(180, 39),
                .ReadOnly = True,
                .Visible = False
            }

            pnlItemDetails.Controls.Add(txtHistorySupName)
            pnlItemDetails.Controls.Add(txtHistoryTeleNo)
            pnlItemDetails.Controls.Add(lblHistorySupName)
            pnlItemDetails.Controls.Add(lblHistoryTeleNo)
            pnlItemDetails.Controls.Add(lblHistorySelSupName)
            pnlItemDetails.Controls.Add(txtHistorySelSupName)
            pnlItemDetails.Controls.Add(lblHistorySelTeleNo)
            pnlItemDetails.Controls.Add(txtHistorySelTeleNo)

            If pnlItemDetails IsNot Nothing Then pnlItemDetails.BringToFront()

            ' Use BeginInvoke to ensure focus happens after form is fully loaded/shown
            Me.BeginInvoke(Sub() InvTxt.Focus())
        Catch ex As Exception
        End Try
    End Sub

    Private Sub PurchaseReturn_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.F2 Then
            btnAddNew.PerformClick()
        ElseIf e.KeyCode = Keys.F4 Then
            printInvBtn.PerformClick()
        End If
    End Sub

    Private Sub InvTxt_Click(sender As Object, e As EventArgs) Handles InvTxt.Click, InvTxt.Enter
        If historyLevel > 0 Then
            ' At any history level -> Back to Main
            historyLevel = 0
            sdetailsbtn.Text = "R Supplier"
            If InvTxt.Text <> "" Then
                FetchInvoiceDetails(InvTxt.Text, CNameTxt.Text)
            Else
                DataGridView1.DataSource = Nothing
            End If
            ToggleMiddleFieldsVisibility(True)
        End If
    End Sub

    Private Sub InvTxt_TextChanged(sender As Object, e As EventArgs) Handles InvTxt.TextChanged
        If InvTxt.Focused Then
            Try
                If conn.State = ConnectionState.Closed Then conn.Open()
                Dim table As New DataTable()
                Dim query As String = "SELECT p.pur_id as 'Inv No', s.name as 'Supplier Name', s.tel_no as 'Tel No', p.pur_date as 'Sale Date', p.sub_total as 'Total Amount', p.cost as 'Grand Total' " &
                                     "FROM purchasing p JOIN supplier s ON p.supplier_id = s.id WHERE p.pur_id LIKE @inv ORDER BY p.pur_id ASC"
                Using adapter As New MySqlDataAdapter(query, conn)
                    adapter.SelectCommand.Parameters.AddWithValue("@inv", InvTxt.Text & "%")
                    adapter.Fill(table)
                End Using
                DataGridView1.DataSource = table
                setup_grid_style(DataGridView1)
                ' Column formatting for Invoice Search
                If DataGridView1.Columns.Count >= 6 Then
                    DataGridView1.Columns(0).Width = 140  ' Inv No
                    DataGridView1.Columns(1).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill ' Supplier Name
                    DataGridView1.Columns(1).MinimumWidth = 250
                    DataGridView1.Columns(2).Width = 180 ' Tel No
                    DataGridView1.Columns(3).Width = 200 ' Sale Date
                    DataGridView1.Columns(4).Width = 180 ' Total Amount
                    DataGridView1.Columns(5).Width = 180 ' Grand Total
                End If
                conn.Close()
                isSearchMode = (InvTxt.Text <> "")
                If historyLevel = 1 Then creditSeach()
            Catch ex As Exception
                If conn.State = ConnectionState.Open Then conn.Close()
            End Try
        End If
    End Sub

    Private Sub CNameTxt_TextChanged(sender As Object, e As EventArgs) Handles CNameTxt.TextChanged
        If CNameTxt.Focused Then
            Try
                If conn.State = ConnectionState.Closed Then conn.Open()
                Dim table As New DataTable()
                Dim query As String = "SELECT p.pur_id as 'Inv No', s.name as 'Supplier Name', s.tel_no as 'Tel No', p.pur_date as 'Sale Date', p.sub_total as 'Total Amount', p.cost as 'Grand Total' " &
                                     "FROM purchasing p JOIN supplier s ON p.supplier_id = s.id WHERE s.name LIKE @name ORDER BY s.name ASC"
                Using adapter As New MySqlDataAdapter(query, conn)
                    adapter.SelectCommand.Parameters.AddWithValue("@name", CNameTxt.Text & "%")
                    adapter.Fill(table)
                End Using
                DataGridView1.DataSource = table
                setup_grid_style(DataGridView1)
                ' Column formatting for Supplier Name Search
                If DataGridView1.Columns.Count >= 6 Then
                    DataGridView1.Columns(0).Width = 140  ' Inv No
                    DataGridView1.Columns(1).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill ' Supplier Name
                    DataGridView1.Columns(1).MinimumWidth = 250
                    DataGridView1.Columns(2).Width = 180 ' Tel No
                    DataGridView1.Columns(3).Width = 200 ' Sale Date
                    DataGridView1.Columns(4).Width = 180 ' Total Amount
                    DataGridView1.Columns(5).Width = 180 ' Grand Total
                End If
                conn.Close()
                isSearchMode = (CNameTxt.Text <> "")
                If historyLevel = 1 Then creditSeach()
            Catch ex As Exception
                If conn.State = ConnectionState.Open Then conn.Close()
            End Try
        End If
    End Sub

    Private Sub InvTxt_KeyDown(sender As Object, e As KeyEventArgs) Handles InvTxt.KeyDown
        If e.KeyCode = Keys.Enter Then
            If DataGridView1.Visible AndAlso DataGridView1.CurrentRow IsNot Nothing Then
                ProcessGridSelection(DataGridView1.CurrentRow.Index)
                TextBox10.Focus() ' Move to Item selection after Inv selection
            ElseIf Not String.IsNullOrEmpty(InvTxt.Text) Then
                FetchInvoiceDetails(InvTxt.Text, CNameTxt.Text)
                TextBox10.Focus() ' Move to Item selection after Fetch
            End If
            e.SuppressKeyPress = True
        ElseIf e.KeyCode = Keys.Down Or e.KeyCode = Keys.Up Then
            HandleArrowKeyNavigation(e)
        End If
    End Sub

    Private Sub CNameTxt_KeyDown(sender As Object, e As KeyEventArgs) Handles CNameTxt.KeyDown
        If e.KeyCode = Keys.Enter Then
            If DataGridView1.Visible AndAlso DataGridView1.CurrentRow IsNot Nothing Then
                ProcessGridSelection(DataGridView1.CurrentRow.Index)
            End If
            e.SuppressKeyPress = True
        ElseIf e.KeyCode = Keys.Down Or e.KeyCode = Keys.Up Then
            HandleArrowKeyNavigation(e)
        End If
    End Sub

    Private Sub TextBox10_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox10.KeyDown
        If e.KeyCode = Keys.Down Or e.KeyCode = Keys.Up Then
            HandleArrowKeyNavigation(e)
        ElseIf e.KeyCode = Keys.Enter Then
            ' If no row is selected, selection logic won't run, but we still move focus
            If DataGridView1.CurrentRow IsNot Nothing Then ProcessGridSelection(DataGridView1.CurrentRow.Index)
            ReQty.Focus() ' Move to Return Qty field
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub DescriptionTextBox_KeyDown(sender As Object, e As KeyEventArgs) Handles DescriptionTextBox.KeyDown
        If e.KeyCode = Keys.Down Or e.KeyCode = Keys.Up Then
            HandleArrowKeyNavigation(e)
        ElseIf e.KeyCode = Keys.Enter Then
            If DataGridView1.CurrentRow IsNot Nothing Then ProcessGridSelection(DataGridView1.CurrentRow.Index)
            ReQty.Focus()
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub HandleArrowKeyNavigation(e As KeyEventArgs)
        If DataGridView1.Rows.Count = 0 Then Return
        Dim currentIndex As Integer = If(DataGridView1.CurrentRow IsNot Nothing, DataGridView1.CurrentRow.Index, -1)
        If e.KeyCode = Keys.Down AndAlso currentIndex < DataGridView1.Rows.Count - 1 Then
            DataGridView1.ClearSelection()
            DataGridView1.Rows(currentIndex + 1).Selected = True
            DataGridView1.CurrentCell = DataGridView1.Rows(currentIndex + 1).Cells(0)
        ElseIf e.KeyCode = Keys.Up AndAlso currentIndex > 0 Then
            DataGridView1.ClearSelection()
            DataGridView1.Rows(currentIndex - 1).Selected = True
            DataGridView1.CurrentCell = DataGridView1.Rows(currentIndex - 1).Cells(0)
        End If
        e.Handled = True
    End Sub

    Private Sub ProcessGridSelection(rowIndex As Integer)
        If rowIndex < 0 OrElse rowIndex >= DataGridView1.Rows.Count Then Return
        ' If in any History/View mode (Supplier, Invoices, Items View), skip auto-filling textboxes
        If historyLevel > 0 Then Return

        Dim row As DataGridViewRow = DataGridView1.Rows(rowIndex)
        If row.IsNewRow Then Return
        isFillingFields = True
        Try
            Dim firstHeader As String = DataGridView1.Columns(0).HeaderText.Trim().ToLower()
            If firstHeader.Contains("inv") Then
                Dim invNo As String = If(row.Cells(0).Value?.ToString(), "")
                If Not String.IsNullOrEmpty(invNo) Then
                    InvTxt.Text = invNo
                    Dim supName As String = ""
                    If row.Cells.Count > 1 Then supName = If(row.Cells(1).Value?.ToString(), "")
                    If row.Cells.Count > 1 Then CNameTxt.Text = supName
                    If row.Cells.Count > 2 Then TelNoTxt.Text = If(row.Cells(2).Value?.ToString(), "")
                    FetchInvoiceDetails(invNo, supName)
                    isSearchMode = False
                End If
            ElseIf firstHeader.Contains("item") OrElse firstHeader.Contains("desc") OrElse firstHeader.Contains("id") Then
                Dim drv As DataRowView = TryCast(row.DataBoundItem, DataRowView)
                If drv IsNot Nothing Then
                    TextBox10.Text = drv("item_id").ToString()
                    DescriptionTextBox.Text = drv("description").ToString()
                    TextBoxItemCost.Text = drv("item_cost").ToString()
                    If drv.DataView.Table.Columns.Contains("selling_price") Then
                        TextBox5.Text = drv("selling_price").ToString() : TextBox2.Text = drv("whole_selling_price").ToString()
                        TextBox1.Text = drv("retail_selling_price").ToString() : TextBoxAvgCost.Text = drv("avg_cost").ToString()
                    End If
                    TextBox9.Text = drv("st_qty").ToString() : TextBox8.Text = drv("discount").ToString() : TextBox6.Text = drv("amount").ToString()
                    DiscountAmountTxt.Text = drv("discount").ToString()
                    If drv.DataView.Table.Columns.Contains("location_id") AndAlso Not IsDBNull(drv("location_id")) Then
                        ComboBoxLocation.SelectedValue = drv("location_id")
                    End If
                Else
                    If row.Cells.Count >= 10 Then
                        TextBox10.Text = If(row.Cells(0).Value?.ToString(), "")
                        DescriptionTextBox.Text = If(row.Cells(1).Value?.ToString(), "")
                        TextBoxItemCost.Text = If(row.Cells(2).Value?.ToString(), "")
                        TextBox5.Text = If(row.Cells(3).Value?.ToString(), "")
                        TextBox2.Text = If(row.Cells(4).Value?.ToString(), "")
                        TextBox1.Text = If(row.Cells(5).Value?.ToString(), "")
                        TextBoxAvgCost.Text = If(row.Cells(6).Value?.ToString(), "")
                        TextBox9.Text = If(row.Cells(7).Value?.ToString(), "")

                        If row.Cells.Count >= 12 AndAlso DataGridView1.Columns(8).HeaderText = "Location" Then
                            If row.Cells(11).Value IsNot Nothing AndAlso Not IsDBNull(row.Cells(11).Value) Then
                                ComboBoxLocation.SelectedValue = row.Cells(11).Value
                            End If
                            TextBox8.Text = If(row.Cells(9).Value?.ToString(), "")
                            TextBox6.Text = If(row.Cells(10).Value?.ToString(), "")
                            DiscountAmountTxt.Text = TextBox8.Text
                        Else
                            TextBox8.Text = If(row.Cells(8).Value?.ToString(), "")
                            TextBox6.Text = If(row.Cells(9).Value?.ToString(), "")
                            DiscountAmountTxt.Text = TextBox8.Text
                        End If
                    End If
                End If
                oldqty = Val(TextBox9.Text)
                ReQty.Clear() : ReQty.Focus()
            End If
        Catch ex As Exception : Finally : isFillingFields = False : End Try
    End Sub

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        If e.RowIndex >= 0 Then
            Try
                If historyLevel = 1 Then
                    Dim supName As String = Convert.ToString(DataGridView1.Rows(e.RowIndex).Cells(0).Value)
                    Dim tele As String = Convert.ToString(DataGridView1.Rows(e.RowIndex).Cells(1).Value)
                    If Not String.IsNullOrEmpty(supName) Then LoadSupplierInvoices_History(supName, tele)
                    Return
                ElseIf historyLevel = 2 Then
                    Dim invN As String = Convert.ToString(DataGridView1.Rows(e.RowIndex).Cells(0).Value)
                    If Not String.IsNullOrEmpty(invN) Then LoadInvoiceItems_History(invN)
                    Return
                End If
            Catch ex As Exception
                ' Silent fail
            End Try
        End If
        ProcessGridSelection(e.RowIndex)
    End Sub

    Private Sub FilterInvoiceItems()
        If historyLevel <> 0 OrElse DataGridView1.DataSource Is Nothing Then Return
        Try
            Dim dt As DataTable = Nothing
            If TypeOf DataGridView1.DataSource Is DataTable Then : dt = DirectCast(DataGridView1.DataSource, DataTable)
            ElseIf TypeOf DataGridView1.DataSource Is DataView Then : dt = DirectCast(DataGridView1.DataSource, DataView).Table : End If
            If dt IsNot Nothing Then
                Dim filter As String = ""
                If Not String.IsNullOrEmpty(TextBox10.Text) Then
                    filter = String.Format("item_id Like '{0}%'", TextBox10.Text.Replace("'", "''"))
                    dt.DefaultView.Sort = "item_id ASC"
                End If
                If Not String.IsNullOrEmpty(DescriptionTextBox.Text) Then
                    Dim descPart As String = String.Format("description Like '{0}%'", DescriptionTextBox.Text.Replace("'", "''"))
                    filter = If(filter = "", descPart, filter & " AND " & descPart)
                    If String.IsNullOrEmpty(TextBox10.Text) Then
                        dt.DefaultView.Sort = "description ASC"
                    End If
                End If
                dt.DefaultView.RowFilter = filter
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub TextBox10_TextChanged(sender As Object, e As EventArgs) Handles TextBox10.TextChanged
        If TextBox10.Focused AndAlso historyLevel = 0 AndAlso Not isFillingFields Then FilterInvoiceItems()
    End Sub

    Private Sub DescriptionTextBox_TextChanged(sender As Object, e As EventArgs) Handles DescriptionTextBox.TextChanged
        If DescriptionTextBox.Focused AndAlso historyLevel = 0 AndAlso Not isFillingFields Then FilterInvoiceItems()
    End Sub

    Private Sub DataGridView1_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellDoubleClick
        If e.RowIndex >= 0 Then
            Try
                If historyLevel = 1 Then
                    Dim supName As String = Convert.ToString(DataGridView1.Rows(e.RowIndex).Cells(0).Value)
                    Dim tele As String = Convert.ToString(DataGridView1.Rows(e.RowIndex).Cells(1).Value)
                    If Not String.IsNullOrEmpty(supName) Then LoadSupplierInvoices_History(supName, tele)
                ElseIf historyLevel = 2 Then
                    Dim invN As String = Convert.ToString(DataGridView1.Rows(e.RowIndex).Cells(0).Value)
                    If Not String.IsNullOrEmpty(invN) Then LoadInvoiceItems_History(invN)
                ElseIf historyLevel = 3 Then
                    ' View only mode for history items - do not fill textboxes
                    ' If Not String.IsNullOrEmpty(lastSelectedInvNo) Then
                    '     FetchInvoiceDetails(lastSelectedInvNo) : historyLevel = 0 : sdetailsbtn.Text = "R Supplier"
                    ' End If
                End If
            Catch ex As Exception
                MessageBox.Show("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Private Sub sdetailsbtn_Click(sender As Object, e As EventArgs) Handles sdetailsbtn.Click
        If historyLevel = 0 Then
            creditSeach()
            historyLevel = 1
            sdetailsbtn.Text = "Back"
            ToggleMiddleFieldsVisibility(False)
        ElseIf historyLevel = 1 Then
            historyLevel = 0
            sdetailsbtn.Text = "R Supplier"
            If InvTxt.Text <> "" Then
                FetchInvoiceDetails(InvTxt.Text, CNameTxt.Text)
            Else
                DataGridView1.DataSource = Nothing
            End If
            ToggleMiddleFieldsVisibility(True)
        ElseIf historyLevel = 2 Then
            creditSeach()
            historyLevel = 1
            ToggleMiddleFieldsVisibility(False)
        ElseIf historyLevel = 3 Then
            If txtHistorySupName IsNot Nothing Then txtHistorySupName.Clear()
            LoadSupplierInvoices_History(lastSelectedSupName)
            historyLevel = 2
            ToggleMiddleFieldsVisibility(False)
        End If
    End Sub
    Private Sub ReturnBtn_Click(sender As Object, e As EventArgs) Handles ReturnBtn.Click
        ' Validation
        If TextBox10.Text = "" Then
            MessageBox.Show("Please select an item.")
            Return
        End If

        ' Mandatory Reason Validation (ComboBox Based)
        Dim finalReason As String = ""
        If ComboBoxreson.Visible Then
            If ComboBoxreson.SelectedIndex <= 0 Then
                MessageBox.Show("Please select a reason.")
                ComboBoxreson.Focus() : ComboBoxreson.DroppedDown = True
                Return
            Else
                finalReason = ComboBoxreson.Text
            End If
        Else
            ' DescriptionTxt is visible (Other was selected)
            If String.IsNullOrWhiteSpace(DescriptionTxt.Text) Then
                MessageBox.Show("Please type a custom reason.")
                DescriptionTxt.Focus()
                Return
            Else
                finalReason = DescriptionTxt.Text
            End If
        End If

        If String.IsNullOrWhiteSpace(ReQty.Text) OrElse Not IsNumeric(ReQty.Text) Then
            MessageBox.Show("Enter Return Quantity.")
            ReQty.Focus()
            Return
        End If

        Dim nqty As Integer = CInt(Val(ReQty.Text))
        If nqty <= 0 OrElse nqty > oldqty Then
            MessageBox.Show("Invalid Qty.")
            Return
        End If

        ' Calculate potential return amounts
        Dim unitAmount As Double = Val(TextBoxItemCost.Text) * ((100 - Val(TextBox8.Text)) / 100)
        Dim totalItemReturnAmount As Double = nqty * unitAmount
        Dim grandReturnAmount As Double = totalItemReturnAmount * ((100 - Val(DiscoText.Text)) / 100)

        ' Update UI Totals
        CashRtnTxt.Text = (Val(CashRtnTxt.Text) + grandReturnAmount).ToString("F3")
        TextBox7.Text = grandReturnAmount.ToString("F3")

        ' Stage or Update the return (Accumulate quantity if already staged)
        Dim stagedIndex As Integer = -1
        For i As Integer = 0 To stagedReturns.Count - 1
            If stagedReturns(i).ItemId = TextBox10.Text Then
                stagedIndex = i
                Exit For
            End If
        Next

        If stagedIndex <> -1 Then
            Dim sr As StagedReturn = stagedReturns(stagedIndex)
            sr.QtyToReturn += nqty
            sr.NetItemRtnAmt += totalItemReturnAmount
            sr.GrandRtnAmt += grandReturnAmount
            stagedReturns(stagedIndex) = sr
        Else
            Dim sr As New StagedReturn With {
                .ItemId = TextBox10.Text,
                .Description = DescriptionTextBox.Text,
                .QtyToReturn = nqty,
                .Reason = finalReason,
                .UnitCost = Val(TextBoxItemCost.Text),
                .DiscPercent = Val(TextBox8.Text),
                .NetItemRtnAmt = totalItemReturnAmount,
                .GrandRtnAmt = grandReturnAmount,
                .OldInvQty = oldqty,
                .LocationID = If(ComboBoxLocation.SelectedValue IsNot Nothing, CInt(ComboBoxLocation.SelectedValue), 0)
            }
            stagedReturns.Add(sr)
        End If

        ' Update Grid Locally (Subtract Qty and Amount)
        For Each row As DataGridViewRow In DataGridView1.Rows
            If row.Cells(0).Value?.ToString() = TextBox10.Text Then
                Dim currentQty As Double = Val(row.Cells(7).Value)
                Dim currentAmt As Double = Val(row.Cells(10).Value)
                row.Cells(7).Value = (currentQty - nqty)
                row.Cells(10).Value = (currentAmt - totalItemReturnAmount)
                Exit For
            End If
        Next

        ' Update Invoice Totals in UI
        calNetAmount()

        isReturnProcessed = True

        ' Clear Item Selection Fields so user can add another
        isFillingFields = True ' Prevent event loops
        oldqty = 0
        TextBox10.Clear()
        DescriptionTextBox.Clear()
        DescriptionTxt.Clear()
        TextBox9.Clear()
        ReQty.Clear()
        TextBoxItemCost.Clear()
        TextBox5.Clear()
        TextBox2.Clear()
        TextBox1.Clear()
        TextBoxAvgCost.Clear()
        TextBox6.Clear()
        TextBox8.Clear()
        ComboBoxLocation.SelectedIndex = -1
        ComboBoxreson.SelectedIndex = 0
        ComboBoxreson.Visible = True
        DescriptionTxt.Visible = False
        DescriptionTxt.Clear()
        isFillingFields = False

        ToggleMiddleFieldsVisibility(True)
        
        TextBox10.Focus()
        ' MessageBox.Show("Item added to return list. You can add another or click Save to finalize.", "Staged", MessageBoxButtons.OK, MessageBoxIcon.Information)

        ' Note: We are keeping the fields filled for Save button to use them.
    End Sub

    Private Sub DataGridView1_KeyDown(sender As Object, e As KeyEventArgs) Handles DataGridView1.KeyDown
        If e.KeyCode = Keys.Enter Then
            DataGridView1_CellClick(sender, New DataGridViewCellEventArgs(DataGridView1.CurrentCell.ColumnIndex, DataGridView1.CurrentCell.RowIndex))
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub ReQty_TextChanged(sender As Object, e As EventArgs) Handles ReQty.TextChanged
        If IsNumeric(ReQty.Text) AndAlso oldqty > 0 Then
            Dim rtnQ As Integer = CInt(Val(ReQty.Text))
            Dim remQ As Integer = oldqty - rtnQ
            If remQ >= 0 Then
                TextBox9.Text = remQ.ToString()
                If IsNumeric(TextBoxItemCost.Text) Then
                    Dim unitAmt As Double = Val(TextBoxItemCost.Text) * ((100 - Val(TextBox8.Text)) / 100)
                    TextBox6.Text = (remQ * unitAmt).ToString("F3")
                End If
            Else : TextBox9.Text = "0" : End If
        ElseIf oldqty > 0 Then
            TextBox9.Text = oldqty.ToString()
            If IsNumeric(TextBoxItemCost.Text) Then
                Dim unitAmt As Double = Val(TextBoxItemCost.Text) * ((100 - Val(TextBox8.Text)) / 100)
                TextBox6.Text = (oldqty * unitAmt).ToString("F3")
            End If
        End If
    End Sub

    ' Event when custom reason typing is done
    Private Sub DescriptionTxt_KeyDown(sender As Object, e As KeyEventArgs) Handles DescriptionTxt.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            ReturnBtn.PerformClick()
        End If
    End Sub

    Private Sub printInvBtn_Click(sender As Object, e As EventArgs) Handles printInvBtn.Click

        ' Validation check: Ensure Return button was clicked first
        If Not isReturnProcessed OrElse stagedReturns.Count = 0 Then
            MessageBox.Show("Please click 'Return' to stage the items before saving.", "Action Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Ask for refund distribution before saving
        Dim returnGrandTotal As Double = 0
        For Each sr In stagedReturns
            returnGrandTotal += sr.GrandRtnAmt
        Next

        Dim applyToCredit As Decimal = 0
        Dim applyToSupplierNote As Decimal = 0
        Dim applyToCash As Decimal = 0

        ' Fetch actual unpaid credit from DB (excluding cheques)
        Dim actualUnpaidCredit As Decimal = 0
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()
            Using cmdCred As New MySqlCommand("SELECT credit_balance_due FROM purchasing WHERE pur_id = @inv AND supplier_id = @supId", conn)
                cmdCred.Parameters.AddWithValue("@inv", InvTxt.Text)
                cmdCred.Parameters.AddWithValue("@supId", selectedSupplierID)
                Dim res = cmdCred.ExecuteScalar()
                If res IsNot DBNull.Value AndAlso res IsNot Nothing Then
                    actualUnpaidCredit = Convert.ToDecimal(res)
                End If
            End Using
        Catch ex As Exception
        End Try

        Using dialog As New SupplierRefundSettlementDialog()
            dialog.TotalRefundDue = CDec(returnGrandTotal)
            dialog.UnpaidCredit = actualUnpaidCredit
            
            If dialog.ShowDialog() = DialogResult.OK Then
                applyToCredit = dialog.ApplyToCredit
                applyToSupplierNote = dialog.ApplyToSupplierCreditNote
                applyToCash = dialog.ApplyToCash
            Else
                Return ' cancelled return
            End If
        End Using

        Using transConn As New MySqlConnection(ConnStr)
            transConn.Open() : Dim trans As MySqlTransaction = transConn.BeginTransaction()
            Try
                Dim insertedReturnIds As New List(Of String)
                For Each sr In stagedReturns
                    Dim currentStock As Integer = GetCurrentStock(sr.ItemId)
                    Dim newStock As Integer = currentStock - sr.QtyToReturn
                    
                    ' Stock update (items table)
                    Using cmd As New MySqlCommand("UPDATE items SET st_qty = @qty WHERE id = @id", transConn, trans)
                        cmd.Parameters.AddWithValue("@qty", newStock) : cmd.Parameters.AddWithValue("@id", sr.ItemId) : cmd.ExecuteNonQuery()
                    End Using
                    
                    ' items_stock update (specific invoice row)
                    If sr.QtyToReturn >= sr.OldInvQty Then
                        Using cmd As New MySqlCommand("DELETE FROM items_stock WHERE item_id = @id AND inv_no = @inv AND supplier_id = @supId", transConn, trans)
                            cmd.Parameters.AddWithValue("@id", sr.ItemId)
                            cmd.Parameters.AddWithValue("@inv", InvTxt.Text)
                            cmd.Parameters.AddWithValue("@supId", selectedSupplierID)
                            cmd.ExecuteNonQuery()
                        End Using
                    Else
                        Using cmd As New MySqlCommand("UPDATE items_stock SET st_qty = st_qty - @rtn, qty_purchased = qty_purchased - @rtn, amount = amount - @amt WHERE item_id = @id AND inv_no = @inv AND supplier_id = @supId", transConn, trans)
                            cmd.Parameters.AddWithValue("@rtn", sr.QtyToReturn) : cmd.Parameters.AddWithValue("@amt", sr.NetItemRtnAmt)
                            cmd.Parameters.AddWithValue("@id", sr.ItemId) : cmd.Parameters.AddWithValue("@inv", InvTxt.Text)
                            cmd.Parameters.AddWithValue("@supId", selectedSupplierID)
                            cmd.ExecuteNonQuery()
                        End Using
                    End If
                    
                    ' purchasing update (decrement the header total and sub_total)
                    Using cmd As New MySqlCommand("UPDATE purchasing SET cost = cost - @grandAmt, sub_total = sub_total - @netAmt, balance_due = balance_due - @grandAmt, items_qty = items_qty - @rtn WHERE pur_id = @inv AND supplier_id = @supId", transConn, trans)
                        cmd.Parameters.AddWithValue("@grandAmt", sr.GrandRtnAmt)
                        cmd.Parameters.AddWithValue("@netAmt", sr.NetItemRtnAmt)
                        cmd.Parameters.AddWithValue("@rtn", sr.QtyToReturn)
                        cmd.Parameters.AddWithValue("@inv", InvTxt.Text)
                        cmd.Parameters.AddWithValue("@supId", selectedSupplierID)
                        cmd.ExecuteNonQuery()
                    End Using
                    
                    ' REMOVED supplicer_credit update from here, moving it outside the loop based on Dialog decision
                    
                    ' Log into purchase_return (one entry per item returned)
                    Dim sqlLog As String = "INSERT INTO purchase_return (pur_type, pur_id, supplier_id, pur_su_method, items_qty, sub_total, cost, description, status, p_method, paid_amount, balance_due, inv_dis, cqe_no, bank_id, Rtn_Qty, reason, return_amt, cash_return, return_date) " & _
                                          "VALUES (@ptype, @pid, @sup, @smeth, @iqty, @subt, @cost, @desc, @stat, @pmeth, @paid, @bal, @dis, @cqe, @bank, @rtnq, @reas, @rtnamt, @cashrtn, NOW())"
                    Using cmdLog As New MySqlCommand(sqlLog, transConn, trans)
                        ' Sanitize ENUM values to match purchase_return schema if possible
                        Dim pTypeVal As String = ComboBox1.Text.Trim()
                        If Not {"Credit", "Cash", "Cheque"}.Contains(pTypeVal) Then pTypeVal = "Cash" ' Fallback for enum safety

                        Dim sMethVal As String = ComboBox2metho.Text.Trim()
                        If sMethVal.ToLower().Contains("self") Then sMethVal = "selfproduct"
                        If Not {"Local", "Import", "selfproduct"}.Contains(sMethVal) Then sMethVal = "Local" ' Fallback for enum safety

                        cmdLog.Parameters.AddWithValue("@ptype", pTypeVal) : cmdLog.Parameters.AddWithValue("@pid", InvTxt.Text)
                        cmdLog.Parameters.AddWithValue("@sup", selectedSupplierID) : cmdLog.Parameters.AddWithValue("@smeth", sMethVal)
                        cmdLog.Parameters.AddWithValue("@iqty", originalItemsQty)
                        cmdLog.Parameters.AddWithValue("@subt", Val(NetAmountTxt.Text) + sr.NetItemRtnAmt)
                        cmdLog.Parameters.AddWithValue("@cost", Val(GrandTotalTxt.Text) + sr.GrandRtnAmt) 
                        Dim itemDesc As String = sr.Description : If itemDesc.Length > 255 Then itemDesc = itemDesc.Substring(0, 255)
                        cmdLog.Parameters.AddWithValue("@desc", itemDesc) : cmdLog.Parameters.AddWithValue("@stat", "Success")
                        cmdLog.Parameters.AddWithValue("@pmeth", txPaymentMethod.Text) : cmdLog.Parameters.AddWithValue("@paid", Val(AmountTextBox2.Text))
                        cmdLog.Parameters.AddWithValue("@bal", Val(TextBox3.Text)) : cmdLog.Parameters.AddWithValue("@dis", Val(DiscoText.Text))
                        cmdLog.Parameters.AddWithValue("@cqe", selectedCqeNo) : cmdLog.Parameters.AddWithValue("@bank", If(selectedBankID = 0, DBNull.Value, selectedBankID))
                        cmdLog.Parameters.AddWithValue("@rtnq", sr.QtyToReturn)
                        Dim reason As String = sr.Reason : If reason.Length > 255 Then reason = reason.Substring(0, 255)
                        cmdLog.Parameters.AddWithValue("@reas", reason) : cmdLog.Parameters.AddWithValue("@rtnamt", sr.GrandRtnAmt)
                        cmdLog.Parameters.AddWithValue("@cashrtn", applyToCash) : cmdLog.ExecuteNonQuery()
                        insertedReturnIds.Add(cmdLog.LastInsertedId.ToString())
                    End Using
                Next
                
                ' purchasing update (final UI state) - Syncing with TextBox3 and other totals
                Dim finalInvQty As Double = 0
                For Each row As DataGridViewRow In DataGridView1.Rows
                    finalInvQty += Val(row.Cells(7).Value)
                Next
                
                ' Fetch current balances from DB for exact deduction logic
                Dim currentCredit As Double = 0
                Dim currentCheque As Double = 0
                Dim currentPaid As Double = 0
                Using cmdBal As New MySqlCommand("SELECT credit_balance_due, cheque_balance_due, paid_amount FROM purchasing WHERE pur_id = @inv AND supplier_id = @supId", transConn, trans)
                    cmdBal.Parameters.AddWithValue("@inv", InvTxt.Text)
                    cmdBal.Parameters.AddWithValue("@supId", selectedSupplierID)
                    Using dr As MySqlDataReader = cmdBal.ExecuteReader()
                        If dr.Read() Then
                            currentCredit = Val(dr("credit_balance_due").ToString())
                            currentCheque = Val(dr("cheque_balance_due").ToString())
                            currentPaid = Val(dr("paid_amount").ToString())
                        End If
                    End Using
                End Using

                Dim remainingReturn As Double = returnGrandTotal
                
                ' Priority 1: Deduct from Credit first
                If remainingReturn > 0 AndAlso currentCredit > 0 Then
                    Dim deduct As Double = Math.Min(remainingReturn, currentCredit)
                    currentCredit -= deduct
                    remainingReturn -= deduct
                End If

                ' Priority 2: Deduct from Cheque second
                Dim showChequeDialog As Boolean = False
                If remainingReturn > 0 AndAlso currentCheque > 0 Then
                    Dim deduct As Double = Math.Min(remainingReturn, currentCheque)
                    currentCheque -= deduct
                    remainingReturn -= deduct
                    
                    If MessageBox.Show("Cheque amount will be decrease want to add a new one?", "Cheque Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                        showChequeDialog = True
                    End If
                End If

                ' Priority 3: Deduct from Paid Amount third
                If remainingReturn > 0 AndAlso currentPaid > 0 Then
                    Dim deduct As Double = Math.Min(remainingReturn, currentPaid)
                    currentPaid -= deduct
                    remainingReturn -= deduct
                End If

                ' Adjust physical paid tracker if applyToCash or applyToSupplierNote was used in Dialog
                ' currentPaid = currentPaid - applyToCash - applyToSupplierNote
                If currentPaid < 0 Then currentPaid = 0
                
                ' Calculate new total balance due
                Dim finalBalance As Double = currentCredit + currentCheque

                Dim sqlUpdateHeader As String = "UPDATE purchasing SET balance_due = @bal, credit_balance_due = @cred, cheque_balance_due = @cheq, paid_amount = @paid, items_qty = @qty WHERE pur_id = @inv AND supplier_id = @supId"
 
                Using cmdUpdateHeader As New MySqlCommand(sqlUpdateHeader, transConn, trans)
                    cmdUpdateHeader.Parameters.AddWithValue("@bal", finalBalance)
                    cmdUpdateHeader.Parameters.AddWithValue("@cred", currentCredit)
                    cmdUpdateHeader.Parameters.AddWithValue("@cheq", currentCheque)
                    cmdUpdateHeader.Parameters.AddWithValue("@paid", currentPaid)
                    cmdUpdateHeader.Parameters.AddWithValue("@qty", finalInvQty)
                    cmdUpdateHeader.Parameters.AddWithValue("@inv", InvTxt.Text)
                    cmdUpdateHeader.Parameters.AddWithValue("@supId", selectedSupplierID)
                    cmdUpdateHeader.ExecuteNonQuery()
                End Using
                
                ' ==== NEW LOGIC: Update Cheque Details if user agreed ====
                If showChequeDialog Then
                    Dim chequeDlg As New ChequeEntryDialog()
                    chequeDlg.DefaultAmount = currentCheque
                    chequeDlg.LockAmount = True
                    If Not String.IsNullOrEmpty(selectedCqeNo) Then
                        chequeDlg.InitialChequeNo = selectedCqeNo
                        chequeDlg.InitialBankID = selectedBankID
                        chequeDlg.InitialAmount = currentCheque
                    End If
                    
                    If chequeDlg.ShowDialog() = DialogResult.OK Then
                        selectedCqeNo = chequeDlg.ChequeNo
                        selectedBankID = chequeDlg.BankID
                        
                        Using cmdCq As New MySqlCommand("UPDATE purchasing SET cqe_no = @cqe, bank_id = @bank WHERE pur_id = @inv AND supplier_id = @supId", transConn, trans)
                            cmdCq.Parameters.AddWithValue("@cqe", selectedCqeNo)
                            cmdCq.Parameters.AddWithValue("@bank", selectedBankID)
                            cmdCq.Parameters.AddWithValue("@inv", InvTxt.Text)
                            cmdCq.Parameters.AddWithValue("@supId", selectedSupplierID)
                            cmdCq.ExecuteNonQuery()
                        End Using
                    End If
                End If
                
                ' ==== NEW LOGIC: SUPPLIER REFUND DISTRIBUTIONS ====
                ' 1. Reduce Unpaid Credit (Supplier Credit Table)
                If applyToCredit > 0 Then
                    Using cmdUpdateCredit As New MySqlCommand("UPDATE supplicer_credit SET amount = GREATEST(0, amount - @credAmt) WHERE inv_no = @inv AND supplier_id = @supId", transConn, trans)
                        cmdUpdateCredit.Parameters.AddWithValue("@credAmt", applyToCredit)
                        cmdUpdateCredit.Parameters.AddWithValue("@inv", InvTxt.Text)
                        cmdUpdateCredit.Parameters.AddWithValue("@supId", selectedSupplierID)
                        cmdUpdateCredit.ExecuteNonQuery()
                    End Using
                End If
                
                ' 2. Add Received Cash back to Petty Cash (Real cash movement)
                If applyToCash > 0 Then
                    Module1.RegisterCashTransaction(applyToCash, "IN", "Supplier Return Refund: " & InvTxt.Text, InvTxt.Text)
                End If
                
                ' 3. Supplier Note (Optional handling if needed in future, currently held implicitly as physical paper voucher)
                
                trans.Commit()
                MessageBox.Show("All Staged Returns Saved Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                
                ' Capture supplier name before clearing to highlight in history
                Dim supNameToHighlight As String = CNameTxt.Text
                Dim savedInvNo As String = InvTxt.Text
                
                ' ==== NEW LOGIC: Show Purchase Return Report ====
                Try
                    Dim formula As String = "{purchase_return1.pur_id} = '" & savedInvNo.Replace("'", "''") & "'"
                    
                    ' Filter exactly by the records we just inserted AND their exact cost to isolate them
                    ' This prevents Cartesian product duplication if items_stock has multiple rows for the same item name.
                    If insertedReturnIds.Count > 0 Then
                        Dim idFilters As New List(Of String)


                        For i As Integer = 0 To insertedReturnIds.Count - 1
                            Dim id As String = insertedReturnIds(i)
                            Dim cost As Decimal = stagedReturns(i).UnitCost
                            idFilters.Add("({purchase_return1.id} = " & id & " AND {ITEMS_STOCK_ALIAS.item_cost} = " & cost & ")")
                        Next
                        formula &= " AND (" & String.Join(" OR ", idFilters) & ")"
                    End If
                    
                    Dim frmReport As New SaleInv()
                    frmReport.ShowReport(savedInvNo, 14, False, True, formula)
                Catch exRpt As Exception
                    MessageBox.Show("Error opening report: " & exRpt.Message)
                End Try
                ' ==================================================
                
                ' Reset staging and clear form
                stagedReturns.Clear()
                cleaBtn_Click(Nothing, Nothing)
                
                ' Return to Supplier Credit Summary view
                sdetailsbtn.Text = "Back" ' Reset text as if we came from summary
                creditSeach()
                historyLevel = 1
                ToggleMiddleFieldsVisibility(False)
                
                ' Select and highlight the supplier in the resulting credit grid
                If Not String.IsNullOrEmpty(supNameToHighlight) Then
                    For Each row As DataGridViewRow In DataGridView1.Rows
                        If row.Cells(0).Value?.ToString() = supNameToHighlight Then
                            DataGridView1.ClearSelection()
                            row.Selected = True
                            DataGridView1.FirstDisplayedScrollingRowIndex = row.Index
                            Exit For
                        End If
                    Next
                End If
                
            Catch ex As Exception
                trans.Rollback()
                MessageBox.Show("Error Saving Returns: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Sub



    ' Clear button logic to reset the entire application form state
    Private Sub cleaBtn_Click(sender As Object, e As EventArgs) Handles cleaBtn.Click
        ' Header Fields
        InvTxt.Clear()
        CNameTxt.Clear()
        TelNoTxt.Clear()
        DescriptionTxt.Clear()
        SaleDateTime.Format = DateTimePickerFormat.Custom
        SaleDateTime.CustomFormat = " "

        ' Item Selection Fields
        oldqty = 0
        TextBox10.Clear()
        DescriptionTextBox.Clear()
        TextBoxItemCost.Clear()
        TextBox5.Clear()
        TextBox2.Clear()
        TextBox1.Clear()
        TextBoxAvgCost.Clear()
        TextBox9.Clear()
        ReQty.Clear()
        TextBox8.Clear()
        TextBox6.Clear()
        ComboBoxLocation.SelectedIndex = -1

        ' Footer Fields
        NetAmountTxt.Text = "0.00"
        DiscoText.Text = "0"
        DiscountAmountTxt.Text = "0.00"
        GrandTotalTxt.Text = "0.00"
        TextBox3.Text = "0.00"
        AmountTextBox2.Text = "0.00"
        TextBox7.Text = "0.00"
        CashRtnTxt.Text = "0.00"
        ComboBox1.SelectedIndex = -1
        ComboBox2metho.SelectedIndex = -1
        txPaymentMethod.Clear()

        ' Grid Reset
        isSearchMode = False
        historyLevel = 0
        sdetailsbtn.Text = "R Supplier"
        isReturnProcessed = False
        stagedReturns.Clear()
        DataGridView1.DataSource = Nothing
        DataGridView1.Columns.Clear()
        
        ' Reason Reset
        ComboBoxreson.SelectedIndex = 0
        ComboBoxreson.Visible = True
        DescriptionTxt.Visible = False
        DescriptionTxt.Clear()
        
        ToggleMiddleFieldsVisibility(True)
        InvTxt.Focus()
    End Sub

    Private Sub btnAddNew_Click(sender As Object, e As EventArgs) Handles btnAddNew.Click
        cleaBtn_Click(sender, e)
    End Sub

    Private Sub ComboBoxreson_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxreson.SelectedIndexChanged
        If ComboBoxreson.Text = "Other" Then
            ComboBoxreson.Visible = False
            DescriptionTxt.Visible = True
            DescriptionTxt.Focus()
        Else
            ComboBoxreson.Visible = True
            DescriptionTxt.Visible = False
            DescriptionTxt.Text = ""
        End If
    End Sub

    Private Sub ReQty_KeyDown(sender As Object, e As KeyEventArgs) Handles ReQty.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            
            ' If reason dropdown is hidden because Other was chosen, focus description
            If Not ComboBoxreson.Visible Then
                DescriptionTxt.Focus()
            Else
                ComboBoxreson.Focus()
                ComboBoxreson.DroppedDown = True
            End If
        End If
    End Sub

    ' Edit Button Logic
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If TextBox10.Text = "" Then
            MessageBox.Show("Please select an item to edit.")
            Return
        End If

        ' 1. Find the staged item
        Dim existingIndex As Integer = -1
        For i As Integer = 0 To stagedReturns.Count - 1
            If stagedReturns(i).ItemId = TextBox10.Text Then
                existingIndex = i
                Exit For
            End If
        Next

        If existingIndex = -1 Then
            MessageBox.Show("This item has not been added to return list yet. Please use the 'Return' button.", "Not Staged", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        ' 2. Revert the previous staging impact locally
        Dim sr = stagedReturns(existingIndex)
        
        ' Add back to Grid Row
        For Each row As DataGridViewRow In DataGridView1.Rows
            If row.Cells(0).Value?.ToString() = sr.ItemId Then
                row.Cells(7).Value = Val(row.Cells(7).Value) + sr.QtyToReturn
                row.Cells(10).Value = Val(row.Cells(10).Value) + sr.NetItemRtnAmt
                Exit For
            End If
        Next
        
        ' Revert Global Cash Return UI Total
        CashRtnTxt.Text = (Val(CashRtnTxt.Text) - sr.GrandRtnAmt).ToString("F3")
        
        ' Remove from list
        stagedReturns.RemoveAt(existingIndex)
        
        ' 3. Recalculate everything
        calNetAmount()
        
        ' 4. Now process it like a fresh return using new inputs (Qty, Reason)
        ReturnBtn.PerformClick()
        
        MessageBox.Show("Quantity updated successfully!", "Updated", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    ' Revert from custom reason to ComboBox if user clicks elsewhere and description is empty
    Private Sub Global_Click_Revert_Reason(sender As Object, e As EventArgs) Handles Me.Click, pnlHeader.Click, pnlItemDetails.Click, pnlFooter.Click, pnlTotals.Click, DataGridView1.Click
        If DescriptionTxt.Visible AndAlso String.IsNullOrWhiteSpace(DescriptionTxt.Text) Then
            ComboBoxreson.Visible = True
            ComboBoxreson.SelectedIndex = 0 
            DescriptionTxt.Visible = False
        End If
    End Sub

    Private Sub Label3_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub ToggleMiddleFieldsVisibility(visible As Boolean)
        ' Hide/Show textboxes
        If TextBoxItemCost IsNot Nothing Then TextBoxItemCost.Visible = visible
        If TextBox5 IsNot Nothing Then TextBox5.Visible = visible
        If TextBox2 IsNot Nothing Then TextBox2.Visible = visible
        If TextBox1 IsNot Nothing Then TextBox1.Visible = visible
        If TextBoxAvgCost IsNot Nothing Then TextBoxAvgCost.Visible = visible
        If TextBox9 IsNot Nothing Then TextBox9.Visible = visible
        If TextBox8 IsNot Nothing Then TextBox8.Visible = visible
        If TextBox6 IsNot Nothing Then TextBox6.Visible = visible
        If lblAmount IsNot Nothing Then lblAmount.Visible = visible

        ' Find and hide/show labels dynamically from pnlItemDetails
        If pnlItemDetails IsNot Nothing Then
            For Each ctrl As Control In pnlItemDetails.Controls
                If TypeOf ctrl Is Label Then
                    Dim lblText As String = ctrl.Text.Trim().ToLower()
                    If lblText = "item cost:" OrElse
                       lblText = "selling price:" OrElse
                       lblText = "wprice:" OrElse
                       lblText = "rprice:" OrElse
                       lblText = "avgcost:" OrElse
                       lblText = "qty:" OrElse
                       lblText = "dis:" Then
                        ctrl.Visible = visible
                    End If
                End If
            Next
        End If

        ' Toggle History Search Controls
        Dim showSearch As Boolean = (Not visible) AndAlso (historyLevel = 1)
        Dim showInvoiceSearch As Boolean = (Not visible) AndAlso (historyLevel = 2 OrElse historyLevel = 3)

        If lblHistorySupName IsNot Nothing Then
            lblHistorySupName.Visible = (showSearch OrElse showInvoiceSearch)
            If showSearch OrElse showInvoiceSearch Then
                lblHistorySupName.Text = If(historyLevel = 2 OrElse historyLevel = 3, "Inv No:", "Supplier Name:")
                lblHistorySupName.BringToFront()
            End If
        End If

        If txtHistorySupName IsNot Nothing Then
            txtHistorySupName.Visible = (showSearch OrElse showInvoiceSearch)
            If showSearch OrElse showInvoiceSearch Then
                If historyLevel = 2 OrElse historyLevel = 3 Then
                    ' Align EXACTLY with TextBox10 (Item Id)
                    If TextBox10 IsNot Nothing Then
                        txtHistorySupName.Location = New Point(TextBox10.Location.X, 70)
                        txtHistorySupName.Width = TextBox10.Width
                    Else
                        txtHistorySupName.Location = New Point(90, 70)
                        txtHistorySupName.Width = 280
                    End If

                    ' Dynamically position the selected supplier display controls right next to txtHistorySupName!
                    Dim supLabelX As Integer = txtHistorySupName.Location.X + txtHistorySupName.Width + 20
                    If lblHistorySelSupName IsNot Nothing Then lblHistorySelSupName.Location = New Point(supLabelX, 76)

                    Dim supTextX As Integer = supLabelX + 115
                    If txtHistorySelSupName IsNot Nothing Then txtHistorySelSupName.Location = New Point(supTextX, 70)

                    Dim teleLabelX As Integer = supTextX + If(txtHistorySelSupName IsNot Nothing, txtHistorySelSupName.Width, 260) + 20
                    If lblHistorySelTeleNo IsNot Nothing Then lblHistorySelTeleNo.Location = New Point(teleLabelX, 76)

                    Dim teleTextX As Integer = teleLabelX + 70
                    If txtHistorySelTeleNo IsNot Nothing Then txtHistorySelTeleNo.Location = New Point(teleTextX, 70)
                Else
                    txtHistorySupName.Location = New Point(160, 70)
                    txtHistorySupName.Width = 280
                End If
                txtHistorySupName.BringToFront()
            End If
        End If

        If lblHistoryTeleNo IsNot Nothing Then
            lblHistoryTeleNo.Visible = showSearch
            If showSearch Then lblHistoryTeleNo.BringToFront()
        End If

        If txtHistoryTeleNo IsNot Nothing Then
            txtHistoryTeleNo.Visible = showSearch
            If showSearch Then txtHistoryTeleNo.BringToFront()
        End If

        If lblHistorySelSupName IsNot Nothing Then
            lblHistorySelSupName.Visible = showInvoiceSearch
            If showInvoiceSearch Then lblHistorySelSupName.BringToFront()
        End If

        If txtHistorySelSupName IsNot Nothing Then
            txtHistorySelSupName.Visible = showInvoiceSearch
            If showInvoiceSearch Then txtHistorySelSupName.BringToFront()
        End If

        If lblHistorySelTeleNo IsNot Nothing Then
            lblHistorySelTeleNo.Visible = showInvoiceSearch
            If showInvoiceSearch Then lblHistorySelTeleNo.BringToFront()
        End If

        If txtHistorySelTeleNo IsNot Nothing Then
            txtHistorySelTeleNo.Visible = showInvoiceSearch
            If showInvoiceSearch Then txtHistorySelTeleNo.BringToFront()
        End If

        ' Clear history search fields and focus when entering history mode
        If Not visible Then
            If historyLevel = 1 Then
                If txtHistorySupName IsNot Nothing Then
                    txtHistorySupName.Clear()
                    txtHistorySupName.Focus()
                End If
                If txtHistoryTeleNo IsNot Nothing Then txtHistoryTeleNo.Clear()
            End If
        End If
        
        If pnlItemDetails IsNot Nothing Then pnlItemDetails.BringToFront()
    End Sub

    ' Event Handlers for History Search
    Private Sub txtHistorySupName_TextChanged(sender As Object, e As EventArgs)
        If historyLevel = 1 Then
            creditSeach()
        ElseIf historyLevel = 2 Then
            LoadSupplierInvoices_History(lastSelectedSupName)
        End If
    End Sub

    Private Sub txtHistoryTeleNo_TextChanged(sender As Object, e As EventArgs)
        If historyLevel = 1 Then
            creditSeach()
        End If
    End Sub

    Private Sub txtHistorySupName_KeyDown(sender As Object, e As KeyEventArgs)
        If e.KeyCode = Keys.Enter Then
            If DataGridView1.Visible AndAlso DataGridView1.CurrentRow IsNot Nothing Then
                If historyLevel = 1 Then
                    Dim supName As String = Convert.ToString(DataGridView1.CurrentRow.Cells(0).Value)
                    Dim tele As String = Convert.ToString(DataGridView1.CurrentRow.Cells(1).Value)
                    If Not String.IsNullOrEmpty(supName) Then LoadSupplierInvoices_History(supName, tele)
                ElseIf historyLevel = 2 Then
                    Dim invN As String = Convert.ToString(DataGridView1.CurrentRow.Cells(0).Value)
                    If Not String.IsNullOrEmpty(invN) Then LoadInvoiceItems_History(invN)
                End If
            End If
            e.SuppressKeyPress = True
        ElseIf e.KeyCode = Keys.Down Or e.KeyCode = Keys.Up Then
            HandleArrowKeyNavigation(e)
        End If
    End Sub

    Private Sub txtHistoryTeleNo_KeyDown(sender As Object, e As KeyEventArgs)
        If e.KeyCode = Keys.Enter Then
            If DataGridView1.Visible AndAlso DataGridView1.CurrentRow IsNot Nothing Then
                Dim supName As String = Convert.ToString(DataGridView1.CurrentRow.Cells(0).Value)
                Dim tele As String = Convert.ToString(DataGridView1.CurrentRow.Cells(1).Value)
                If Not String.IsNullOrEmpty(supName) Then LoadSupplierInvoices_History(supName, tele)
            End If
            e.SuppressKeyPress = True
        ElseIf e.KeyCode = Keys.Down Or e.KeyCode = Keys.Up Then
            HandleArrowKeyNavigation(e)
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        cleaBtn_Click(sender, e)
    End Sub
End Class
