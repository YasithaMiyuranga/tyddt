Imports MySql.Data.MySqlClient

Public Class StockRequest

    Private dtLeft As New DataTable()
    Private dtTemp As New DataTable() ' Repurposed for DataGridView4 (the "to be saved" list)
    Private lastActiveGrid As Integer = 3
    Private historyLevel As Integer = 0
    Private isFormLoaded As Boolean = False
    Private lastSavedTransferId As String = ""

    Private Sub StockRequest_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.KeyPreview = True
        TextBox9.HideSelection = False

        ' Initialize dtLeft (Source Grid)
        If dtLeft.Columns.Count = 0 Then
            dtLeft.Columns.Add("Items Stock ID", GetType(String))
            dtLeft.Columns.Add("Item Id", GetType(String))
            dtLeft.Columns.Add("Description", GetType(String))
            dtLeft.Columns.Add("Stock", GetType(Decimal))
            dtLeft.Columns.Add("Request Qty", GetType(Decimal))
        End If

        ' Initialize dtTemp (Destination Grid)
        If dtTemp.Columns.Count = 0 Then
            dtTemp.Columns.Add("ID", GetType(String))
            dtTemp.Columns.Add("Items Stock ID", GetType(String))
            dtTemp.Columns.Add("Item Id", GetType(String))
            dtTemp.Columns.Add("Description", GetType(String))
            dtTemp.Columns.Add("Stock", GetType(Decimal))
            dtTemp.Columns.Add("Request Qty", GetType(Decimal))
            dtTemp.Columns.Add("Location ID", GetType(Integer)) ' Receive Location
            dtTemp.Columns.Add("Our Location", GetType(Integer)) ' Sender Location
        End If

        LoadLocations()
        DataGridView1.Visible = False
        DataGridView2.Visible = False
        FormatDataGridView()
        
        DataGridView3.DataSource = dtLeft
        DataGridView4.DataSource = dtTemp

        ' Hide specific columns and set widths immediately
        ConfigureDataGridView3()
        RefreshRightGrid()
    End Sub

    Private Sub StockRequest_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.F2 Then
            cleaBtn.PerformClick()
            e.Handled = True
        ElseIf e.KeyCode = Keys.F12 Then
            btnSave.PerformClick()
            e.Handled = True
        ElseIf e.KeyCode = Keys.F3 Then
            Button1.PerformClick()
            e.Handled = True
        ElseIf e.KeyCode = Keys.Delete Then
            deletebtn.PerformClick()
            e.Handled = True
        End If
    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        If keyData = (Keys.Shift Or Keys.Enter) Then
            If Me.ActiveControl Is rename Then
                trann.Focus()
                Return True
            ElseIf Me.ActiveControl Is ComboBoxLocation Then
                ComboBox1.Focus()
                Return True
            End If

            Me.SelectNextControl(Me.ActiveControl, False, True, True, True)
            Return True
        End If
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Private Sub StockRequest_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        trann.Focus()
        trann.Select()
        isFormLoaded = True
    End Sub

    Private Sub setup_grid_style(dgv As DataGridView, Optional isReadOnly As Boolean = True)
        dgv.AllowUserToAddRows = False
        dgv.AllowUserToDeleteRows = False
        dgv.ReadOnly = isReadOnly
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgv.RowHeadersVisible = False
        dgv.BackgroundColor = SystemColors.ButtonFace
        dgv.DefaultCellStyle.Font = New Font("Microsoft Sans Serif", 14)
        dgv.ColumnHeadersDefaultCellStyle.Font = New Font("Microsoft Sans Serif", 16, FontStyle.Bold)
        dgv.EnableHeadersVisualStyles = True
        dgv.ScrollBars = ScrollBars.Both
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None
    End Sub

    Private Sub FormatDataGridView()
        DataGridView1.Visible = False

        ' Formatting DataGridView2 (History View) - Matches PurchaseReturn style
        setup_grid_style(DataGridView2, True)
        DataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

        ' Formatting DataGridView3 (Source Items Grid) - Original style (12pt)
        setup_grid_style(DataGridView3, False)
        DataGridView3.BackgroundColor = Color.White
        DataGridView3.DefaultCellStyle.Font = New Font("Microsoft Sans Serif", 12)
        DataGridView3.ColumnHeadersDefaultCellStyle.Font = New Font("Microsoft Sans Serif", 12, FontStyle.Bold)
        DataGridView3.DefaultCellStyle.ForeColor = Color.Black ' Explicitly set black text
        DataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        DataGridView3.EditMode = DataGridViewEditMode.EditOnEnter

        ' Formatting DataGridView4 (Selected Items Grid) - Original style (12pt)
        setup_grid_style(DataGridView4, True)
        DataGridView4.BackgroundColor = Color.White
        DataGridView4.DefaultCellStyle.Font = New Font("Microsoft Sans Serif", 12)
        DataGridView4.ColumnHeadersDefaultCellStyle.Font = New Font("Microsoft Sans Serif", 12, FontStyle.Bold)
        DataGridView4.DefaultCellStyle.ForeColor = Color.Black
        DataGridView4.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        DataGridView4.MultiSelect = True
    End Sub

    Private Sub LoadLocations()
        Try
            ComboBoxLocation.DataSource = Nothing
            ComboBoxLocation.Items.Clear()
            ComboBox1.DataSource = Nothing
            ComboBox1.Items.Clear()

            Dim query As String = "SELECT id, location_name FROM location"
            Using connection As New MySqlConnection(Module1.ConnStr)
                Using cmd As New MySqlCommand(query, connection)
                    connection.Open()
                    Dim dt As New DataTable()
                    Dim dt2 As New DataTable()
                    Using adapter As New MySqlDataAdapter(cmd)
                        adapter.Fill(dt)
                        adapter.Fill(dt2)
                    End Using
                    ComboBoxLocation.DataSource = dt
                    ComboBoxLocation.DisplayMember = "location_name"
                    ComboBoxLocation.ValueMember = "id"
                    ComboBoxLocation.SelectedIndex = -1

                    ComboBox1.DataSource = dt2
                    ComboBox1.DisplayMember = "location_name"
                    ComboBox1.ValueMember = "id"
                    ComboBox1.SelectedIndex = -1
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading locations: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ConfigureDataGridView3()
        If DataGridView3.Columns.Count > 0 Then
            ' Ensure "Items Stock ID" is hidden
            If DataGridView3.Columns.Contains("Items Stock ID") Then
                DataGridView3.Columns("Items Stock ID").Visible = False
            End If
            
            ' Apply styling to columns
            For Each col As DataGridViewColumn In DataGridView3.Columns
                If col.Name = "Request Qty" Then
                    col.ReadOnly = False
                    col.DefaultCellStyle.BackColor = Color.LightYellow
                    ' Ensure it stays yellowish even when the row is selected (like in the photo)
                    col.DefaultCellStyle.SelectionBackColor = Color.LightYellow
                    col.DefaultCellStyle.SelectionForeColor = Color.Black
                Else
                    col.ReadOnly = True
                    ' These columns will use the standard blue selection from setup_grid_style
                End If
            Next

            ' Fixed width for Item Id to prevent it being too small in Fill mode
            If DataGridView3.Columns.Contains("Item Id") Then
                DataGridView3.Columns("Item Id").Width = 140
                DataGridView3.Columns("Item Id").AutoSizeMode = DataGridViewAutoSizeColumnsMode.None
            End If
        End If
    End Sub

    Private Sub RefreshRightGrid()
        If DataGridView4.Columns.Count > 0 Then
            If DataGridView4.Columns.Contains("ID") Then DataGridView4.Columns("ID").Visible = False
            If DataGridView4.Columns.Contains("Items Stock ID") Then DataGridView4.Columns("Items Stock ID").Visible = False
            If DataGridView4.Columns.Contains("Location ID") Then DataGridView4.Columns("Location ID").Visible = False
            If DataGridView4.Columns.Contains("Our Location") Then DataGridView4.Columns("Our Location").Visible = False
        End If
    End Sub

    Private Sub LoadSavedRequests()
        Try
            Dim query As String = "SELECT ss.transfer_id AS 'Transfer ID', " &
                                  "ss.requester_name AS 'Request Name', " &
                                  "ss.requester_tel_no AS 'Request Te No', " &
                                  "l2.location_name AS 'Our Location', " &
                                  "l.location_name AS 'Location' " &
                                  "FROM sending_stock ss " &
                                  "INNER JOIN location l ON ss.receive_location_id = l.id " &
                                  "LEFT JOIN location l2 ON ss.our_location = l2.id " &
                                  "GROUP BY ss.transfer_id, ss.requester_name, ss.requester_tel_no, l2.location_name, l.location_name " &
                                  "ORDER BY MAX(ss.created_at) DESC"

            Using connection As New MySqlConnection(Module1.ConnStr)
                Using cmd As New MySqlCommand(query, connection)
                    Using adapter As New MySqlDataAdapter(cmd)
                        Dim table As New DataTable()
                        adapter.Fill(table)
                        DataGridView2.DataSource = table
                        DataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                        
                        If DataGridView2.Columns.Contains("Transfer ID") Then
                            DataGridView2.Columns("Transfer ID").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                            DataGridView2.Columns("Transfer ID").Width = 160
                        End If

                        ' Highlight last saved Transfer ID if exists
                        If Not String.IsNullOrEmpty(lastSavedTransferId) Then
                            DataGridView2.ClearSelection()
                            For Each row As DataGridViewRow In DataGridView2.Rows
                                Dim cellValue As String = Convert.ToString(row.Cells("Transfer ID").Value).Trim()
                                If String.Equals(cellValue, lastSavedTransferId, StringComparison.OrdinalIgnoreCase) Then
                                    row.Selected = True
                                    ' Force a visual update of the selection
                                    DataGridView2.CurrentCell = row.Cells(0)
                                    row.DefaultCellStyle.BackColor = Color.DodgerBlue
                                    row.DefaultCellStyle.ForeColor = Color.White
                                    DataGridView2.FirstDisplayedScrollingRowIndex = row.Index
                                    Exit For
                                End If
                            Next
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading requests history: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadDetailsForTransfer(transferId As String)
        Try
            ' Group by item_id and our_location to comply with only_full_group_by SQL mode
            Dim query As String = "SELECT i.item_id AS 'Item Id', " &
                                  "MAX(i.description) AS 'Description', " &
                                  "(SELECT IFNULL(SUM(st_qty), 0) FROM items_stock WHERE item_id = i.item_id AND location_id = ss.our_location) AS 'Stock', " &
                                  "SUM(ss.request_quantity) AS 'Request Qty' " &
                                  "FROM sending_stock ss " &
                                  "INNER JOIN items_stock i ON ss.items_stock_id = i.id " &
                                  "WHERE ss.transfer_id = @tid " &
                                  "GROUP BY i.item_id, ss.our_location"

            Using connection As New MySqlConnection(Module1.ConnStr)
                Using cmd As New MySqlCommand(query, connection)
                    cmd.Parameters.AddWithValue("@tid", transferId)
                    Using adapter As New MySqlDataAdapter(cmd)
                        Dim table As New DataTable()
                        adapter.Fill(table)
                        DataGridView2.DataSource = table
                        DataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                        
                        If DataGridView2.Columns.Count >= 4 Then
                            DataGridView2.Columns(0).AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                            DataGridView2.Columns(0).Width = 160 ' Item Id
                            DataGridView2.Columns(1).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill ' Description
                            DataGridView2.Columns(2).AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                            DataGridView2.Columns(2).Width = 180 ' Stock
                            DataGridView2.Columns(3).AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                            DataGridView2.Columns(3).Width = 180 ' Request Qty
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading item details: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        If Not isFormLoaded Then Return
        If ComboBox1.SelectedIndex = -1 Then
            dtLeft.Rows.Clear()
            Return
        End If
        LoadItemsForLocation(Convert.ToInt32(ComboBox1.SelectedValue))
    End Sub

    Private Sub LoadItemsForLocation(locationId As Integer)
        Try
            ' Show all items for the location, even those with 0 stock, to avoid missing items
            Dim query As String = "SELECT MAX(id) as 'Items Stock ID', item_id as 'Item Id', MAX(description) as 'Description', SUM(IFNULL(st_qty, 0)) as 'Stock' " &
                                  "FROM items_stock " &
                                  "WHERE location_id = @loc_id " &
                                  "GROUP BY item_id " &
                                  "ORDER BY item_id ASC"
            Using connection As New MySqlConnection(Module1.ConnStr)
                Using cmd As New MySqlCommand(query, connection)
                    cmd.Parameters.AddWithValue("@loc_id", locationId)
                    Using adapter As New MySqlDataAdapter(cmd)
                        dtLeft.Rows.Clear()
                        Dim tempTable As New DataTable()
                        adapter.Fill(tempTable)
                        
                        For Each row As DataRow In tempTable.Rows
                            Dim dr As DataRow = dtLeft.NewRow()
                            dr("Items Stock ID") = row("Items Stock ID").ToString()
                            dr("Item Id") = row("Item Id").ToString()
                            dr("Description") = row("Description").ToString()
                            dr("Stock") = Val(row("Stock"))
                            dr("Request Qty") = 0
                            dtLeft.Rows.Add(dr)
                        Next
                    End Using
                End Using
            End Using
            ConfigureDataGridView3()
        Catch ex As Exception
            MessageBox.Show("Error loading items for location: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub DataGridView3_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView3.CellClick
        If e.RowIndex >= 0 Then
            Dim row As DataGridViewRow = DataGridView3.Rows(e.RowIndex)
            
            ' Fill bottom textboxes
            IT_CodeTextBox.Text = row.Cells("Item Id").Value.ToString()
            IT_CodeTextBox.Tag = row.Cells("Items Stock ID").Value.ToString()
            DescriptionTextBox.Text = row.Cells("Description").Value.ToString()
            QutTextBox.Text = row.Cells("Stock").Value.ToString()
            TextBox9.Text = If(row.Cells("Request Qty").Value IsNot Nothing, row.Cells("Request Qty").Value.ToString(), "0")
            
            ' Synchronize selection highlight to TextBox9
            TextBox9.SelectAll()
            
            ' Always move to and start editing the "Request Qty" cell in the GRID
            ' This ensures the "0" is selected in blue as requested in your screenshots
            DataGridView3.CurrentCell = row.Cells("Request Qty")
            DataGridView3.BeginEdit(True)
        End If
    End Sub

    Private Sub DataGridView3_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView3.CellValueChanged
        ' Sync grid value back to TextBox9 if the current row matches
        If e.RowIndex >= 0 AndAlso DataGridView3.Columns(e.ColumnIndex).Name = "Request Qty" Then
            If DataGridView3.CurrentRow IsNot Nothing AndAlso DataGridView3.CurrentRow.Index = e.RowIndex Then
                TextBox9.Text = DataGridView3.Rows(e.RowIndex).Cells("Request Qty").Value.ToString()
            End If
        End If
    End Sub

    Private Sub DataGridView3_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles DataGridView3.CurrentCellDirtyStateChanged
        ' Commit changes immediately so CellValueChanged fires as user types or moves
        If DataGridView3.IsCurrentCellDirty Then
            DataGridView3.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub

    Private Sub DataGridView3_EditingControlShowing(sender As Object, e As DataGridViewEditingControlShowingEventArgs) Handles DataGridView3.EditingControlShowing
        ' Remove old handlers
        Dim tb As TextBox = CType(e.Control, TextBox)
        RemoveHandler tb.KeyDown, AddressOf DataGridView3_QtyEntry_KeyDown
        RemoveHandler tb.TextChanged, AddressOf DataGridView3_GridQty_TextChanged
        
        ' Add new handlers
        AddHandler tb.KeyDown, AddressOf DataGridView3_QtyEntry_KeyDown
        ' This ensures that as you type in the grid, TextBox9 updates instantly
        AddHandler tb.TextChanged, AddressOf DataGridView3_GridQty_TextChanged

        ' Pre-select the text so user can immediately overwrite the "0" (as requested)
        tb.SelectAll()
        TextBox9.SelectAll()
    End Sub

    Private Sub DataGridView3_GridQty_TextChanged(sender As Object, e As EventArgs)
        Dim tb As TextBox = CType(sender, TextBox)
        TextBox9.Text = tb.Text
        TextBox9.SelectAll()
    End Sub

    Private Sub DataGridView3_QtyEntry_KeyDown(sender As Object, e As KeyEventArgs)
        If e.KeyCode = Keys.Enter Then
            ' Move focus to the Request Qty cell of the next row
            Dim curRowIdx As Integer = DataGridView3.CurrentCell.RowIndex
            If curRowIdx < DataGridView3.Rows.Count - 1 Then
                ' Delay to allow the grid to finalize the current cell edit
                Me.BeginInvoke(Sub()
                                   DataGridView3.CurrentCell = DataGridView3.Rows(curRowIdx + 1).Cells("Request Qty")
                                   DataGridView3.BeginEdit(True)
                               End Sub)
            End If
            e.Handled = True
            e.SuppressKeyPress = True ' Prevent default Enter behavior (moving to next column)
        End If
    End Sub

    Private Sub TextBox9_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox9.KeyDown
        If e.KeyCode = Keys.Enter Then
            ' Update the quantity in the left grid (DataGridView3) for the currently selected row
            If DataGridView3.CurrentRow IsNot Nothing AndAlso IsNumeric(TextBox9.Text) Then
                Dim qty As Decimal = Val(TextBox9.Text.Trim())
                Dim rowIndex As Integer = DataGridView3.CurrentRow.Index
                
                ' Set the value in the "Request Qty" cell
                DataGridView3.Rows(rowIndex).Cells("Request Qty").Value = qty
                
                ' Select the next item in the grid for speed
                If rowIndex < DataGridView3.Rows.Count - 1 Then
                    DataGridView3.CurrentCell = DataGridView3.Rows(rowIndex + 1).Cells(0)
                    ' Trigger CellClick to fill the textboxes for the next item
                    DataGridView3_CellClick(Nothing, New DataGridViewCellEventArgs(0, rowIndex + 1))
                End If
                
                e.Handled = True
                e.SuppressKeyPress = True
            End If
        End If
    End Sub

    Private Sub TextBox9_TextChanged(sender As Object, e As EventArgs) Handles TextBox9.TextChanged
        ' Sync TextBox9 value to the grid cell in real-time as user types
        If isFormLoaded AndAlso TextBox9.Focused AndAlso DataGridView3.CurrentRow IsNot Nothing Then
            If IsNumeric(TextBox9.Text) Then
                DataGridView3.CurrentRow.Cells("Request Qty").Value = Val(TextBox9.Text)
            ElseIf String.IsNullOrWhiteSpace(TextBox9.Text) Then
                DataGridView3.CurrentRow.Cells("Request Qty").Value = 0
            End If
        End If
    End Sub

    Private Sub trann_KeyDown(sender As Object, e As KeyEventArgs) Handles trann.KeyDown
        If e.KeyCode = Keys.Enter Then
            rename.Focus()
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub rename_KeyDown(sender As Object, e As KeyEventArgs) Handles rename.KeyDown
        If e.KeyCode = Keys.Enter Then
            reteln.Focus()
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub reteln_KeyDown(sender As Object, e As KeyEventArgs) Handles reteln.KeyDown
        If e.KeyCode = Keys.Enter Then
            ComboBox1.Focus()
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub ComboBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles ComboBox1.KeyDown
        If e.KeyCode = Keys.Enter Then
            ComboBoxLocation.Focus()
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub ComboBoxLocation_KeyDown(sender As Object, e As KeyEventArgs) Handles ComboBoxLocation.KeyDown
        If e.KeyCode = Keys.Enter Then
            DataGridView3.Focus()
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Function ValidateForm() As Boolean
        If String.IsNullOrWhiteSpace(trann.Text) Then
            MessageBox.Show("Please enter Transfer ID.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            trann.Focus()
            Return False
        End If
        If String.IsNullOrWhiteSpace(rename.Text) Then
            MessageBox.Show("Please enter Request Name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            rename.Focus()
            Return False
        End If
        If String.IsNullOrWhiteSpace(reteln.Text) OrElse Not System.Text.RegularExpressions.Regex.IsMatch(reteln.Text.Trim(), "^\d{10}$") Then
            MessageBox.Show("Please enter a valid 10-digit Request Tel No.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            reteln.Focus()
            Return False
        End If
        If ComboBox1.SelectedIndex = -1 Then
            MessageBox.Show("Please select Location.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            ComboBox1.Focus()
            Return False
        End If
        If ComboBoxLocation.SelectedIndex = -1 Then
            MessageBox.Show("Please select our Location.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            ComboBoxLocation.Focus()
            Return False
        End If

        ' Prevent same location selection
        If ComboBox1.SelectedValue IsNot Nothing AndAlso ComboBoxLocation.SelectedValue IsNot Nothing Then
            If ComboBox1.SelectedValue.ToString() = ComboBoxLocation.SelectedValue.ToString() Then
                MessageBox.Show("Please select a different location. Sender and Receiver cannot be the same.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                ComboBoxLocation.Focus()
                Return False
            End If
        End If
        Return True
    End Function

    Private Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
        If Not ValidateForm() Then Return

        Dim addedCount As Integer = 0
        For Each row As DataGridViewRow In DataGridView3.Rows
            If row.Cells("Request Qty").Value IsNot Nothing AndAlso IsNumeric(row.Cells("Request Qty").Value) Then
                Dim qty As Decimal = Convert.ToDecimal(row.Cells("Request Qty").Value)
                If qty > 0 Then
                    Dim stock As Decimal = Convert.ToDecimal(row.Cells("Stock").Value)
                    If qty > stock Then
                        MessageBox.Show("Request quantity for " & row.Cells("Item Id").Value.ToString() & " exceeds available stock.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Continue For
                    End If

                    Dim itemsStockId As String = row.Cells("Items Stock ID").Value.ToString()
                    Dim existingRow As DataRow() = dtTemp.Select("[Items Stock ID] = '" & itemsStockId & "'")
                    
                    If existingRow.Length > 0 Then
                        existingRow(0)("Request Qty") = qty
                    Else
                        Dim dr As DataRow = dtTemp.NewRow()
                        dr("ID") = Guid.NewGuid().ToString()
                        dr("Items Stock ID") = itemsStockId
                        dr("Item Id") = row.Cells("Item Id").Value
                        dr("Description") = row.Cells("Description").Value
                        dr("Stock") = row.Cells("Stock").Value
                        dr("Request Qty") = qty
                        dr("Location ID") = Convert.ToInt32(ComboBoxLocation.SelectedValue) ' Receive Location (Our Location)
                        dr("Our Location") = Convert.ToInt32(ComboBox1.SelectedValue) ' Sender Location (Source Location)
                        dtTemp.Rows.Add(dr)
                    End If
                    addedCount += 1
                End If
            End If
        Next

        If addedCount > 0 Then
            MessageBox.Show(addedCount & " item(s) processed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            RefreshRightGrid()
        Else
            MessageBox.Show("No items with Request Qty > 0 found in the list.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub sentbtn_Click(sender As Object, e As EventArgs) Handles sentbtn.Click
        btnNext_Click(Nothing, Nothing)
    End Sub

    Private Sub cleaBtn_Click(sender As Object, e As EventArgs) Handles cleaBtn.Click
        trann.Clear()
        trann.ReadOnly = False
        IT_CodeTextBox.Clear()
        IT_CodeTextBox.Tag = Nothing
        DescriptionTextBox.Clear()
        QutTextBox.Clear()
        rename.Clear()
        reteln.Clear()
        If TextBox9 IsNot Nothing Then TextBox9.Clear()
        ComboBoxLocation.SelectedIndex = -1
        ComboBox1.SelectedIndex = -1
        Button1.Tag = Nothing 
        dtLeft.Rows.Clear()
        dtTemp.Rows.Clear()
        RefreshRightGrid()
        trann.Focus()
    End Sub

    Private Sub DataGridView3_Enter(sender As Object, e As EventArgs) Handles DataGridView3.Enter
        lastActiveGrid = 3
    End Sub

    Private Sub DataGridView4_Enter(sender As Object, e As EventArgs) Handles DataGridView4.Enter
        lastActiveGrid = 4
    End Sub

    Private Sub deletebtn_Click(sender As Object, e As EventArgs) Handles deletebtn.Click
        If DataGridView4.SelectedRows.Count > 0 Then
            Dim count As Integer = 0
            For Each row As DataGridViewRow In DataGridView4.SelectedRows
                Dim itemsStockId As String = row.Cells("Items Stock ID").Value.ToString()

                ' Reset quantity in Left Grid (dtLeft)
                For Each rLeft As DataRow In dtLeft.Rows
                    If rLeft("Items Stock ID").ToString() = itemsStockId Then
                        rLeft("Request Qty") = 0
                        Exit For
                    End If
                Next

                ' Remove from Right Grid (dtTemp)
                For i As Integer = dtTemp.Rows.Count - 1 To 0 Step -1
                    If dtTemp.Rows(i)("Items Stock ID").ToString() = itemsStockId Then
                        dtTemp.Rows.RemoveAt(i)
                        count += 1
                        Exit For
                    End If
                Next
            Next
            If count > 0 Then
                MessageBox.Show(count & " item(s) removed.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information)
                IT_CodeTextBox.Clear()
                IT_CodeTextBox.Tag = Nothing
                DescriptionTextBox.Clear()
                QutTextBox.Clear()
                TextBox9.Clear()
            End If
        Else
            MessageBox.Show("Please select row(s) in the right grid to delete.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If DataGridView4.SelectedRows.Count > 0 Then
            Dim count As Integer = 0
            For Each row As DataGridViewRow In DataGridView4.SelectedRows
                Dim itemsStockId As String = row.Cells("Items Stock ID").Value.ToString()
                
                ' Reset quantity in Left Grid (dtLeft)
                For Each rLeft As DataRow In dtLeft.Rows
                    If rLeft("Items Stock ID").ToString() = itemsStockId Then
                        rLeft("Request Qty") = 0
                        Exit For
                    End If
                Next

                ' Remove from Right Grid (dtTemp)
                For i As Integer = dtTemp.Rows.Count - 1 To 0 Step -1
                    If dtTemp.Rows(i)("Items Stock ID").ToString() = itemsStockId Then
                        dtTemp.Rows.RemoveAt(i)
                        count += 1
                        Exit For
                    End If
                Next
            Next
            
            If count > 0 Then
                RefreshRightGrid()
                ' Clear textboxes
                IT_CodeTextBox.Clear()
                IT_CodeTextBox.Tag = Nothing
                DescriptionTextBox.Clear()
                QutTextBox.Clear()
                TextBox9.Clear()
            End If
        Else
            MessageBox.Show("Please select item(s) from the right grid to remove.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub DataGridView4_SelectionChanged(sender As Object, e As EventArgs) Handles DataGridView4.SelectionChanged
        If DataGridView4.CurrentRow IsNot Nothing Then
            Dim row As DataGridViewRow = DataGridView4.CurrentRow
            IT_CodeTextBox.Text = row.Cells("Item Id").Value.ToString()
            IT_CodeTextBox.Tag = row.Cells("Items Stock ID").Value.ToString()
            DescriptionTextBox.Text = row.Cells("Description").Value.ToString()
            QutTextBox.Text = row.Cells("Stock").Value.ToString()
            TextBox9.Text = row.Cells("Request Qty").Value.ToString()
            TextBox9.SelectAll()
        End If
    End Sub

    Private Sub DataGridView4_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView4.CellDoubleClick
        ' Double-click deletion removed as per user request
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If DataGridView4.CurrentRow Is Nothing Then
            MessageBox.Show("Please select an item from the right grid to edit.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If String.IsNullOrWhiteSpace(TextBox9.Text) OrElse Not IsNumeric(TextBox9.Text) Then
            MessageBox.Show("Please enter a valid Request Qty.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TextBox9.Focus()
            Return
        End If

        Dim qty As Decimal = Convert.ToDecimal(TextBox9.Text.Trim())
        Dim stock As Decimal = Convert.ToDecimal(QutTextBox.Text.Trim())

        If qty > stock Then
            MessageBox.Show("Request quantity cannot exceed available stock.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim itemsStockId As String = IT_CodeTextBox.Tag.ToString()
        
        ' Update Right Grid (dtTemp)
        For Each r As DataRow In dtTemp.Rows
            If r("Items Stock ID").ToString() = itemsStockId Then
                r("Request Qty") = qty
                Exit For
            End If
        Next

        ' Synchronize with Left Grid (dtLeft)
        For Each rLeft As DataRow In dtLeft.Rows
            If rLeft("Items Stock ID").ToString() = itemsStockId Then
                rLeft("Request Qty") = qty
                Exit For
            End If
        Next

        MessageBox.Show("Quantity updated in both lists.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        If Not ValidateForm() Then Return

        If dtTemp.Rows.Count = 0 Then
            MessageBox.Show("No items selected to save.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            Using connection As New MySqlConnection(Module1.ConnStr)
                connection.Open()
                Using transaction = connection.BeginTransaction()
                    Try
                        Dim checkQuery As String = "SELECT COUNT(*) FROM sending_stock WHERE transfer_id = @transfer_id"
                        Using cmdCheck As New MySqlCommand(checkQuery, connection, transaction)
                            cmdCheck.Parameters.AddWithValue("@transfer_id", trann.Text.Trim())
                            Dim count As Integer = Convert.ToInt32(cmdCheck.ExecuteScalar())
                            If count > 0 Then
                                MessageBox.Show("This Transfer ID already exists in the system.", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                Return
                            End If
                        End Using

                        For Each row As DataRow In dtTemp.Rows
                            Dim insertQuery As String = "INSERT INTO sending_stock (items_stock_id, request_quantity, receive_location_id, requester_name, requester_tel_no, status, transfer_id, our_location) " &
                                                       "VALUES (@items_stock_id, @request_quantity, @receive_location_id, @requester_name, @requester_tel_no, 'Pending', @transfer_id, @our_location)"
                            Using cmdIns As New MySqlCommand(insertQuery, connection, transaction)
                                cmdIns.Parameters.AddWithValue("@items_stock_id", row("Items Stock ID"))
                                cmdIns.Parameters.AddWithValue("@request_quantity", row("Request Qty"))
                                cmdIns.Parameters.AddWithValue("@receive_location_id", row("Location ID"))
                                cmdIns.Parameters.AddWithValue("@requester_name", rename.Text.Trim())
                                cmdIns.Parameters.AddWithValue("@requester_tel_no", reteln.Text.Trim())
                                cmdIns.Parameters.AddWithValue("@transfer_id", trann.Text.Trim())
                                cmdIns.Parameters.AddWithValue("@our_location", row("Our Location"))
                                cmdIns.ExecuteNonQuery()
                            End Using
                        Next
                        transaction.Commit()
                        lastSavedTransferId = trann.Text.Trim()
                        MessageBox.Show("Stock request saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        PrintReport(trann.Text.Trim())
                        cleaBtn_Click(Nothing, Nothing)

                        ' Automatically show history and highlight the saved record
                        sdetailsbtn_Click(Nothing, Nothing)
                    Catch ex As Exception
                        transaction.Rollback()
                        Throw ex
                    End Try
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error saving data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub PrintReport(transferId As String)
        Try
            SaleInv.ShowReport(transferId, 11)
        Catch ex As Exception
            MessageBox.Show("Error displaying report: " & ex.Message, "Report Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub sdetailsbtn_Click(sender As Object, e As EventArgs) Handles sdetailsbtn.Click
        If historyLevel = 0 Then
            If DataGridView2.Parent IsNot Me Then DataGridView2.Parent = Me
            Dim gb1 As Control = DataGridView3.Parent
            If gb1 IsNot Nothing Then
                Dim screenZero As Point = gb1.PointToScreen(New Point(0, 0))
                DataGridView2.Location = Me.PointToClient(screenZero)
                DataGridView2.Height = gb1.Height
            End If
            Dim gb4 As Control = DataGridView4.Parent
            If gb4 IsNot Nothing Then
                Dim screenGrid4Right As Point = gb4.PointToScreen(New Point(gb4.Width, 0))
                Dim formGrid4Right As Point = Me.PointToClient(screenGrid4Right)
                DataGridView2.Width = formGrid4Right.X - DataGridView2.Left
            Else
                DataGridView2.Width = 1000
            End If
            DataGridView2.BringToFront()
            DataGridView2.Visible = True
            LoadSavedRequests()
            historyLevel = 1
            sdetailsbtn.Text = "Back"
            sdetailsbtn.BackColor = Color.DodgerBlue
        ElseIf historyLevel = 1 Then
            DataGridView2.Visible = False
            historyLevel = 0
            sdetailsbtn.Text = "Request" & vbCrLf & "History"
            sdetailsbtn.BackColor = Color.DodgerBlue
        ElseIf historyLevel = 2 Then
            LoadSavedRequests()
            historyLevel = 1
            sdetailsbtn.Text = "Back"
            sdetailsbtn.BackColor = Color.DodgerBlue
        End If
    End Sub

    Private Sub DataGridView2_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView2.CellDoubleClick
        If historyLevel = 1 AndAlso e.RowIndex >= 0 Then
            Dim row As DataGridViewRow = DataGridView2.Rows(e.RowIndex)
            If DataGridView2.Columns.Contains("Transfer ID") Then
                Dim transferId As String = row.Cells("Transfer ID").Value.ToString()
                LoadDetailsForTransfer(transferId)
                historyLevel = 2
            End If
        End If
    End Sub

    Private Sub ComboBoxLocation_Enter(sender As Object, e As EventArgs) Handles ComboBoxLocation.Enter
        If isFormLoaded Then ComboBoxLocation.DroppedDown = True
    End Sub

    Private Sub ComboBox1_Enter(sender As Object, e As EventArgs) Handles ComboBox1.Enter
        If isFormLoaded Then ComboBox1.DroppedDown = True
    End Sub

    Private Sub trann_Enter(sender As Object, e As EventArgs) Handles trann.Enter, trann.Click
        If historyLevel > 0 Then
            historyLevel = 0
            DataGridView2.Visible = False
            sdetailsbtn.Text = "Request" & vbCrLf & "History"
            sdetailsbtn.BackColor = Color.DodgerBlue
        End If
    End Sub

End Class