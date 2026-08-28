Imports MySql.Data.MySqlClient
Public Class frmStock
    Public Enum StockViewMode
        Total
        Batch
        Location
    End Enum

    Private currentViewMode As StockViewMode = StockViewMode.Total
    Private isInitializing As Boolean = True

    Private Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click
        Try
            If ComboBoxPrinter.SelectedIndex = -1 Then
                MessageBox.Show("Please select a Printer first.")
                Return
            End If

            Dim itmId As String = ComboBoxFItemId.Text.Trim()
            Dim itmName As String = TextBoxItemName.Text.Trim()
            Dim brandName As String = If(ComboBoxFBrand.SelectedIndex <> -1, ComboBoxFBrand.Text, "")
            Dim des As String = TextBoxFDescription.Text.Trim()

            Dim viewer As New SalesHistoryForm()
            ' Map Stock Form ComboBoxPrinter to SalesHistoryForm report types (index 3 is Stock Report)
            ' For now, we only have index 3 for stock.
            viewer.SetReportContext(3, DateTime.Now, DateTime.Now, itmId, True, "Normal", "Cash", False, itmName, brandName, des)
            viewer.Show()
        Catch ex As Exception
            MessageBox.Show("Stock Print Error: " & ex.Message)
        End Try
    End Sub

    Private Sub rowcount()
        If DataGridView1.Rows.Count > 0 Then
            Try
                Dim uniqueIds As New HashSet(Of String)
                Dim offset As Integer = If(currentViewMode = StockViewMode.Location, 1, 0)
                
                ' Check if we have at least one column to check
                If DataGridView1.Columns.Count > offset Then
                    For Each row As DataGridViewRow In DataGridView1.Rows
                        If Not row.IsNewRow Then
                            Dim val As Object = row.Cells(0 + offset).Value
                            If val IsNot Nothing AndAlso Not IsDBNull(val) Then
                                uniqueIds.Add(val.ToString())
                            End If
                        End If
                    Next
                End If
                
                LabelTotalItem.Text = uniqueIds.Count.ToString()
            Catch ex As Exception
                ' Fallback to simple row count if any error occurs
                Dim k As Integer = DataGridView1.Rows.Count
                If DataGridView1.AllowUserToAddRows Then k -= 1
                LabelTotalItem.Text = k.ToString()
            End Try
        Else
            LabelTotalItem.Text = "0"
        End If
    End Sub

    Private Sub populate_filter_combos()
        Try
            ' Populate Item ID filter
            Dim dtId As New DataTable()
            Dim adapterId As New MySqlDataAdapter("SELECT DISTINCT id FROM items ORDER BY id", conn)
            adapterId.Fill(dtId)
            ComboBoxFItemId.DataSource = dtId
            ComboBoxFItemId.DisplayMember = "id"
            ComboBoxFItemId.ValueMember = "id"
            ComboBoxFItemId.SelectedIndex = -1

            ' Populate Brand filter
            Dim dtBrand As New DataTable()
            Dim adapterBrand As New MySqlDataAdapter("SELECT id, name FROM brand ORDER BY name", conn)
            adapterBrand.Fill(dtBrand)
            ComboBoxFBrand.DataSource = dtBrand
            ComboBoxFBrand.DisplayMember = "name"
            ComboBoxFBrand.ValueMember = "id"
            ComboBoxFBrand.SelectedIndex = -1

            ' Populate Location filter
            Dim dtLoc As New DataTable()
            Dim adapterLoc As New MySqlDataAdapter("SELECT id, location_name FROM location ORDER BY location_name", conn)
            adapterLoc.Fill(dtLoc)
            ComboBoxFLocation.DataSource = dtLoc
            ComboBoxFLocation.DisplayMember = "location_name"
            ComboBoxFLocation.ValueMember = "id"
            ComboBoxFLocation.SelectedIndex = -1

            ' Populate Print Types (Report Types)
            ComboBoxPrinter.Items.Clear()
            Try
                For Each printer As String In Printing.PrinterSettings.InstalledPrinters
                    ComboBoxPrinter.Items.Add(printer)
                Next

                ' Default to default printer if available
                Dim defaultPrinter As String = New Printing.PrinterSettings().PrinterName
                If ComboBoxPrinter.Items.Contains(defaultPrinter) Then
                    ComboBoxPrinter.SelectedItem = defaultPrinter
                ElseIf ComboBoxPrinter.Items.Count > 0 Then
                    ComboBoxPrinter.SelectedIndex = 0
                End If
            Catch ex As Exception
                ' Silent fail for printer listing
            End Try

        Catch ex As Exception
            MessageBox.Show("Error populating combos: " & ex.Message)
        End Try
    End Sub

    Private Sub apply_filters()
        Try
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Open()
            Dim table As New DataTable()

            Dim sql As String = ""
            Select Case currentViewMode
                Case StockViewMode.Total
                    sql = "SELECT i.id as id, i.item_name, i.description, MAX(i.item_cost) as item_cost, i.avg_cost, SUM(ist.st_qty * ist.item_cost) as exact_total_cost, " &
                          "0 as batch_sp, i.discount, i.category_id, i.brand_id, br.name as brand, i.profit_margin, i.supply_method, i.measure, " &
                          "IFNULL(SUM(ist.st_qty), 0) as st_qty, i.stock_alert, LP.max_w as wholesale, LP.max_r as retail, LP.max_n as billing_price, MAX(ist.date) as purchase_date " &
                          "FROM items i " &
                          "LEFT JOIN items_stock ist ON i.id = ist.item_id " &
                          "LEFT JOIN brand br ON i.brand_id = br.id " &
                          "LEFT JOIN (" &
                          "  SELECT item_id, MAX(whole_selling_price) as max_w, MAX(retail_selling_price) as max_r, MAX(selling_price) as max_n " &
                          "  FROM items_stock GROUP BY item_id" &
                          ") LP ON i.id = LP.item_id " &
                          "WHERE i.deleted_at IS NULL " &
                          "GROUP BY i.id, i.item_name, i.description, i.avg_cost, i.discount, i.category_id, i.brand_id, br.name, i.profit_margin, i.supply_method, i.measure, i.stock_alert, LP.max_w, LP.max_r, LP.max_n " &
                          "ORDER BY id ASC"

                Case StockViewMode.Batch
                    sql = "SELECT ist.item_id as id, i.item_name, i.description, ist.item_cost, i.avg_cost, (ist.st_qty * ist.item_cost) as exact_total_cost, ist.selling_price as batch_sp, i.discount, i.category_id, i.brand_id, br.name as brand, i.profit_margin, i.supply_method, i.measure, ist.st_qty, i.stock_alert, " &
                          "LP.max_w as wholesale, LP.max_r as retail, LP.max_n as billing_price, ist.date as purchase_date " &
                          "FROM items_stock ist " &
                          "INNER JOIN items i ON ist.item_id = i.id " &
                          "LEFT JOIN brand br ON i.brand_id = br.id " &
                          "LEFT JOIN (" &
                          "  SELECT item_id, MAX(whole_selling_price) as max_w, MAX(retail_selling_price) as max_r, MAX(selling_price) as max_n " &
                          "  FROM items_stock GROUP BY item_id" &
                          ") LP ON ist.item_id = LP.item_id " &
                          "WHERE ist.st_qty <> 0 AND i.deleted_at IS NULL " &
                          "ORDER BY id ASC"

                Case StockViewMode.Location
                    sql = "SELECT l.location_name as location, i.id as id, i.item_name, i.description, MAX(i.item_cost) as item_cost, i.avg_cost, SUM(ist.st_qty * ist.item_cost) as exact_total_cost, " &
                          "0 as batch_sp, i.discount, i.category_id, i.brand_id, br.name as brand, i.profit_margin, i.supply_method, i.measure, " &
                          "IFNULL(SUM(ist.st_qty), 0) as st_qty, i.stock_alert, LP.max_w as wholesale, LP.max_r as retail, LP.max_n as billing_price, MAX(ist.date) as purchase_date " &
                          "FROM items i " &
                          "LEFT JOIN items_stock ist ON i.id = ist.item_id " &
                          "LEFT JOIN location l ON ist.location_id = l.id " &
                          "LEFT JOIN brand br ON i.brand_id = br.id " &
                          "LEFT JOIN (" &
                          "  SELECT item_id, MAX(whole_selling_price) as max_w, MAX(retail_selling_price) as max_r, MAX(selling_price) as max_n " &
                          "  FROM items_stock GROUP BY item_id" &
                          ") LP ON i.id = LP.item_id " &
                          "WHERE i.deleted_at IS NULL AND ist.st_qty <> 0 " &
                          "GROUP BY l.location_name, i.id, i.item_name, i.description, i.avg_cost, i.discount, i.category_id, i.brand_id, br.name, i.profit_margin, i.supply_method, i.measure, i.stock_alert, LP.max_w, LP.max_r, LP.max_n " &
                          "ORDER BY id ASC"
            End Select

            Dim adapter As New MySqlDataAdapter(sql, conn)
            adapter.Fill(table)

            Dim dv As New DataView(table)
            Dim filterList As New List(Of String)

            If Not String.IsNullOrEmpty(ComboBoxFItemId.Text) Then
                filterList.Add(String.Format("id LIKE '{0}%'", ComboBoxFItemId.Text.Replace("'", "''")))
            End If

            If Not String.IsNullOrEmpty(TextBoxItemName.Text) Then
                filterList.Add(String.Format("item_name LIKE '{0}%'", TextBoxItemName.Text.Replace("'", "''")))
            End If

            ' Location Filter (Only in Location Mode)
            If currentViewMode = StockViewMode.Location AndAlso ComboBoxFLocation.SelectedIndex <> -1 Then
                filterList.Add(String.Format("location = '{0}'", ComboBoxFLocation.Text.Replace("'", "''")))
            End If

            If Not String.IsNullOrEmpty(TextBoxFDescription.Text) Then
                filterList.Add(String.Format("description Like '{0}%'", TextBoxFDescription.Text.Replace("'", "''")))
            End If

            If ComboBoxFBrand.SelectedIndex <> -1 AndAlso ComboBoxFBrand.SelectedValue IsNot Nothing Then
                Dim val As Object = ComboBoxFBrand.SelectedValue
                If TypeOf val Is DataRowView Then val = DirectCast(val, DataRowView).Item("id")
                filterList.Add(String.Format("brand_id = {0}", val))
            End If

            If filterList.Count > 0 Then
                dv.RowFilter = String.Join(" AND ", filterList)
            End If

            ' Apply dynamic alphabetical sorting based on the active search textbox
            If Not String.IsNullOrEmpty(TextBoxFDescription.Text) Then
                dv.Sort = "description ASC"
            ElseIf Not String.IsNullOrEmpty(TextBoxItemName.Text) Then
                dv.Sort = "item_name ASC"
            ElseIf Not String.IsNullOrEmpty(ComboBoxFItemId.Text) Then
                dv.Sort = "id ASC"
            End If

            DataGridView1.DataSource = dv

            ' Adjust Column Indices for Location mode (Location becomes Column 0)
            Dim offset As Integer = If(currentViewMode = StockViewMode.Location, 1, 0)

            ' Safety: Clear columns or specifically set visibility for extra columns
            If currentViewMode = StockViewMode.Location Then
                DataGridView1.Columns("location").HeaderText = "Location"
                DataGridView1.Columns("location").Visible = True
                DataGridView1.Columns("location").DisplayIndex = 0
                DataGridView1.Columns("location").Width = 120
            End If

            ' Hide unnecessary columns
            DataGridView1.Columns(1 + offset).Visible = False ' item_name
            DataGridView1.Columns(4 + offset).Visible = False ' avg_cost
            DataGridView1.Columns(5 + offset).Visible = False ' exact_total_cost
            DataGridView1.Columns(6 + offset).Visible = False ' batch_sp
            DataGridView1.Columns(7 + offset).Visible = False ' discount
            DataGridView1.Columns(8 + offset).Visible = False ' category_id
            DataGridView1.Columns(9 + offset).Visible = False ' brand_id
            DataGridView1.Columns(11 + offset).Visible = False ' profit_margin
            DataGridView1.Columns(12 + offset).Visible = False ' supply_method
            DataGridView1.Columns(13 + offset).Visible = False ' measure

            ' Re-ordering / Width Setting for visible ones
            DataGridView1.Columns(0 + offset).HeaderText = "ItmCode"
            DataGridView1.Columns(0 + offset).Width = 100
            DataGridView1.Columns(0 + offset).DisplayIndex = 0 + offset

            DataGridView1.Columns(2 + offset).HeaderText = "Description"
            DataGridView1.Columns(2 + offset).Width = 400
            DataGridView1.Columns(2 + offset).DisplayIndex = 1 + offset

            DataGridView1.Columns(10 + offset).HeaderText = "Brand"
            DataGridView1.Columns(10 + offset).Width = 110
            DataGridView1.Columns(10 + offset).DisplayIndex = 2 + offset

            DataGridView1.Columns(3 + offset).HeaderText = "Cost"
            DataGridView1.Columns(3 + offset).Width = 100
            DataGridView1.Columns(3 + offset).DisplayIndex = 3 + offset

            DataGridView1.Columns(14 + offset).HeaderText = "Stock Qty"
            DataGridView1.Columns(14 + offset).Width = 100
            DataGridView1.Columns(14 + offset).DisplayIndex = 4 + offset

            DataGridView1.Columns(15 + offset).HeaderText = "Alert"
            DataGridView1.Columns(15 + offset).Width = 80
            DataGridView1.Columns(15 + offset).DisplayIndex = 5 + offset

            DataGridView1.Columns(18 + offset).HeaderText = "Normal"
            DataGridView1.Columns(18 + offset).Width = 100
            DataGridView1.Columns(18 + offset).DisplayIndex = 6 + offset

            DataGridView1.Columns(16 + offset).HeaderText = "Wholesale"
            DataGridView1.Columns(16 + offset).Width = 100
            DataGridView1.Columns(16 + offset).DisplayIndex = 7 + offset

            DataGridView1.Columns(17 + offset).HeaderText = "Retail"
            DataGridView1.Columns(17 + offset).Width = 100
            DataGridView1.Columns(17 + offset).DisplayIndex = 8 + offset

            DataGridView1.Columns(19 + offset).HeaderText = "Date"
            DataGridView1.Columns(19 + offset).Width = 100
            DataGridView1.Columns(19 + offset).DisplayIndex = 9 + offset

            DataGridView1.ReadOnly = True
            DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            DataGridView1.AllowUserToAddRows = False
            DataGridView1.DefaultCellStyle.Font = New Font("Microsoft Sans Serif", 14)
            DataGridView1.ColumnHeadersDefaultCellStyle.Font = New Font("Microsoft Sans Serif", 16, FontStyle.Bold)

            conn.Close()
            profit_cal()
            rowcount()

        Catch ex As Exception
            If conn.State = ConnectionState.Open Then conn.Close()
            MessageBox.Show("Error applying filters: " & ex.Message)
        End Try
    End Sub


    Private Sub load_data()
        apply_filters()
    End Sub

    Private Sub DataGridView1_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles DataGridView1.CellFormatting
        Try
            ' Safety Check: Ensure we have the required columns and a valid row
            If e.RowIndex >= 0 AndAlso Not DataGridView1.Rows(e.RowIndex).IsNewRow Then
                If DataGridView1.Columns.Contains("st_qty") AndAlso DataGridView1.Columns.Contains("stock_alert") Then
                    Dim colStock As Integer = DataGridView1.Columns("st_qty").Index
                    Dim colAlert As Integer = DataGridView1.Columns("stock_alert").Index

                    Dim valStock As Object = DataGridView1.Rows(e.RowIndex).Cells(colStock).Value
                    Dim valAlert As Object = DataGridView1.Rows(e.RowIndex).Cells(colAlert).Value

                    Dim stock As Double = 0
                    Dim Avstock As Double = 0

                    If Not IsDBNull(valStock) AndAlso valStock IsNot Nothing Then
                        Double.TryParse(valStock.ToString(), stock)
                    End If
                    If Not IsDBNull(valAlert) AndAlso valAlert IsNot Nothing Then
                        Double.TryParse(valAlert.ToString(), Avstock)
                    End If

                    If stock >= Avstock Then
                        If stock = Avstock Then
                            e.CellStyle.BackColor = Color.Yellow
                            e.CellStyle.ForeColor = Color.Black
                        Else
                            ' Default to white if above alert Level
                            ' e.CellStyle.BackColor = Color.White
                        End If
                    Else
                        e.CellStyle.BackColor = Color.Red
                        e.CellStyle.ForeColor = Color.White
                    End If
                End If
            End If
        Catch ex As Exception
            ' Silent fail during rapid UI changes to prevent crash
        End Try
    End Sub



    ' color_check is deprecated, but we'll remove it to keep clear



    Private Sub profit_cal()
        Dim costTotal As Double = 0
        Dim wpriceTotal As Double = 0
        Dim rpriceTotal As Double = 0
        Dim normalPriceTotal As Double = 0

        Dim wprofit As Double = 0
        Dim rprofit As Double = 0
        Dim nprofitTotal As Double = 0

        Try
            If Not DataGridView1.Columns.Contains("st_qty") Then Return

            Dim colStQty As Integer = DataGridView1.Columns("st_qty").Index
            Dim colCost As Integer = DataGridView1.Columns("item_cost").Index
            Dim colWholesale As Integer = DataGridView1.Columns("wholesale").Index
            Dim colRetail As Integer = DataGridView1.Columns("retail").Index
            Dim colNormal As Integer = DataGridView1.Columns("billing_price").Index

            Dim colExactTotalCost As Integer = DataGridView1.Columns("exact_total_cost").Index

            For s As Integer = 0 To DataGridView1.Rows.Count - 1 Step +1
                If DataGridView1.Rows(s).IsNewRow Then Continue For

                Dim nstk As Double = 0
                Dim itemCost As Double = 0
                Dim exactCost As Double = 0
                Dim wPrice As Double = 0
                Dim rPrice As Double = 0
                Dim nPrice As Double = 0

                Dim valStk As Object = DataGridView1.Rows(s).Cells(colStQty).Value
                Dim valCost As Object = DataGridView1.Rows(s).Cells(colCost).Value
                Dim valExactCost As Object = DataGridView1.Rows(s).Cells(colExactTotalCost).Value
                Dim valW As Object = DataGridView1.Rows(s).Cells(colWholesale).Value
                Dim valR As Object = DataGridView1.Rows(s).Cells(colRetail).Value
                Dim valN As Object = DataGridView1.Rows(s).Cells(colNormal).Value

                If Not IsDBNull(valStk) Then Double.TryParse(valStk.ToString(), nstk)
                If Not IsDBNull(valCost) Then Double.TryParse(valCost.ToString(), itemCost)
                If Not IsDBNull(valExactCost) Then Double.TryParse(valExactCost.ToString(), exactCost)
                If Not IsDBNull(valW) Then Double.TryParse(valW.ToString(), wPrice)
                If Not IsDBNull(valR) Then Double.TryParse(valR.ToString(), rPrice)
                If Not IsDBNull(valN) Then Double.TryParse(valN.ToString(), nPrice)

                costTotal += exactCost
                wpriceTotal += (wPrice * nstk)
                rpriceTotal += (rPrice * nstk)
                normalPriceTotal += (nPrice * nstk)

                ' Calculate profit only if a sale has occurred (price > 0)
                If wPrice > 0 Then wprofit += (wPrice * nstk) - exactCost
                If rPrice > 0 Then rprofit += (rPrice * nstk) - exactCost
                If nPrice > 0 Then nprofitTotal += (nPrice * nstk) - exactCost
            Next
        Catch ex As Exception
            ' Silent log or ignore
        End Try



        ' Totals are already summed up in the loop

        CostLbl.Text = costTotal.ToString("N2")
        WpriceLbl.Text = wpriceTotal.ToString("N2")
        RpriceLbl.Text = rpriceTotal.ToString("N2")
        LTotalPrice.Text = normalPriceTotal.ToString("N2")

        Wprofitlb.Text = wprofit.ToString("N2")
        Rprofitlb.Text = rprofit.ToString("N2")
        Nprofit.Text = nprofitTotal.ToString("N2")

        profitLbl.Text = (nprofitTotal).ToString("N2") ' Default to Normal profit for grand total
    End Sub

    Private Sub Stock_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        isInitializing = True
        populate_filter_combos()
        UpdateButtonColors()
        ' apply_filters() ' Removed as per user request to start empty
        
        ' Ensure initial visibility
        UpdateFilterVisibility()
        
        isInitializing = False
    End Sub

    Private Sub UpdateFilterVisibility()
        Dim isLocationMode As Boolean = (currentViewMode = StockViewMode.Location)

        ' Show/hide item name filter based on mode
        TextBoxItemName.Visible = Not isLocationMode

        ' Show/hide location filter based on mode
        LabelFLocation.Visible = isLocationMode
        ComboBoxFLocation.Visible = isLocationMode

        ' When leaving Location mode, clear the location combo so it doesn't
        ' carry a stale filter into Total/Batch views.
        ' Item Name and Description filters are intentionally preserved across all modes.
        If Not isInitializing AndAlso Not isLocationMode Then
            ComboBoxFLocation.SelectedIndex = -1
        End If
    End Sub



    Private Sub ComboBoxFItemId_TextChanged(sender As Object, e As EventArgs) Handles ComboBoxFItemId.TextChanged, ComboBoxFItemId.SelectedIndexChanged
        If Not isInitializing Then apply_filters()
    End Sub

    Private Sub TextBoxItemName_TextChanged(sender As Object, e As EventArgs) Handles TextBoxItemName.TextChanged
        If Not isInitializing Then apply_filters()
    End Sub

    Private Sub ComboBoxFBrand_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxFBrand.SelectedIndexChanged
        If Not isInitializing Then apply_filters()
    End Sub

    Private Sub TextBoxFDescription_TextChanged(sender As Object, e As EventArgs) Handles TextBoxFDescription.TextChanged
        If Not isInitializing Then apply_filters()
    End Sub

    Private Sub ComboBoxFLocation_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxFLocation.SelectedIndexChanged
        If Not isInitializing Then apply_filters()
    End Sub


    ''' <summary>
    ''' Single click: filter the table to show only the clicked item.
    ''' </summary>
    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        If e.RowIndex < 0 Then Return  ' Ignore header clicks

        Try
            ' Determine column offset (Location mode adds a location column at index 0)
            Dim offset As Integer = If(currentViewMode = StockViewMode.Location, 1, 0)
            Dim cellVal As Object = DataGridView1.Rows(e.RowIndex).Cells(0 + offset).Value

            If cellVal IsNot Nothing AndAlso Not IsDBNull(cellVal) Then
                Dim itemId As String = cellVal.ToString().Trim()
                ' Set the Item ID filter — this triggers apply_filters() automatically
                ' and shows only the selected item in the grid
                ComboBoxFItemId.Text = itemId
            End If
        Catch
        End Try
    End Sub

    Private Sub btnViewTotal_Click(sender As Object, e As EventArgs) Handles btnViewTotal.Click
        currentViewMode = StockViewMode.Total
        UpdateFilterVisibility()
        UpdateButtonColors()
        apply_filters()
    End Sub

    Private Sub btnViewBatch_Click(sender As Object, e As EventArgs) Handles btnViewBatch.Click
        currentViewMode = StockViewMode.Batch
        UpdateFilterVisibility()
        UpdateButtonColors()
        apply_filters()
    End Sub

    Private Sub btnViewLocation_Click(sender As Object, e As EventArgs) Handles btnViewLocation.Click
        currentViewMode = StockViewMode.Location
        UpdateFilterVisibility()
        UpdateButtonColors()
        apply_filters()
    End Sub

    Private Sub UpdateButtonColors()
        btnViewTotal.BackColor = If(currentViewMode = StockViewMode.Total, Color.FromArgb(41, 128, 185), Color.FromArgb(52, 152, 219))
        btnViewBatch.BackColor = If(currentViewMode = StockViewMode.Batch, Color.FromArgb(41, 128, 185), Color.FromArgb(52, 152, 219))
        btnViewLocation.BackColor = If(currentViewMode = StockViewMode.Location, Color.FromArgb(41, 128, 185), Color.FromArgb(52, 152, 219))
    End Sub


    Private Sub StockTextBox_TextChanged(sender As Object, e As EventArgs)

    End Sub
End Class
