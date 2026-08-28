Imports MySql.Data.MySqlClient

Public Class Item_manage
    ' Module-level DataTables to store full lists for manual filtering
    Private dtCategories As DataTable
    Private dtBrands As DataTable
    Private dtMeasures As DataTable
    Private dtSuMethods As DataTable
    Private dtUsers As DataTable
    Private dtSuppliers As DataTable
    Private dtItems As DataTable = Nothing
    Private isUpdateMode As Boolean = False


    Private Sub load_categories()
        LoadTableData("SELECT id, name FROM category ORDER BY name", ComboBoxCategory, dtCategories)
    End Sub

    Private Sub load_brands()
        LoadTableData("SELECT id, name FROM brand ORDER BY name", ComboBoxBrand, dtBrands)
    End Sub

    Private Sub load_suppliers()
        LoadTableData("SELECT id, name FROM supplier ORDER BY name", ComboBoxSupplier, dtSuppliers)
    End Sub

    Private Sub LoadTableData(sql As String, cmb As ComboBox, ByRef dt As DataTable)
        Try
            dt = New DataTable()
            Dim adapter As New MySqlDataAdapter(sql, conn)
            adapter.Fill(dt)
            cmb.DataSource = dt
            cmb.DisplayMember = "name"
            cmb.ValueMember = "id"
            cmb.SelectedIndex = -1
        Catch ex As Exception
            MessageBox.Show("Error loading data: " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Load enum types in items table
    ''' </summary>

    Private Sub load_enums(columnName As String, comboBox As ComboBox)
        Try
            Dim dt As New DataTable()
            Dim adapter As New MySqlDataAdapter("SELECT COLUMN_TYPE FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = 'stock_management' AND TABLE_NAME = 'items' AND COLUMN_NAME = '" & columnName & "'", conn)
            adapter.Fill(dt)
            If dt.Rows.Count > 0 Then
                Dim colType As String = dt.Rows(0)("COLUMN_TYPE").ToString()
                ' Parse ENUM('val1','val2'...)
                Dim enumStr As String = colType.Replace("enum(", "").Replace(")", "").Replace("'", "")
                Dim values As String() = enumStr.Split(","c)

                ' Create DataTable for better filtering than raw Items collection
                Dim dtEnum As New DataTable()
                dtEnum.Columns.Add("val")
                For Each item As String In values
                    dtEnum.Rows.Add(item)
                Next

                If columnName = "measure" Then dtMeasures = dtEnum
                If columnName = "supply_method" Then dtSuMethods = dtEnum

                comboBox.DataSource = dtEnum
                comboBox.DisplayMember = "val"
                comboBox.ValueMember = "val"
                comboBox.SelectedIndex = -1
            End If
        Catch ex As Exception
            MessageBox.Show("Error loading enums for " & columnName & ": " & ex.Message)
        End Try
    End Sub
    ''' <summary>
    ''' Load clients when form loads
    ''' </summary>
    Private Sub Item_manage_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim currentUser As String = Module1.UserName.Trim().ToLower()
        If currentUser = "admin1" OrElse currentUser = "admin2" OrElse currentUser = "admin3" OrElse currentUser = "admin4" Then
            MessageBox.Show("Please enter a valid Cashier ID", "Invalid Cashier", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Me.BeginInvoke(New MethodInvoker(AddressOf Me.Close))
            Return
        End If

        load_categories()
        load_brands()
        load_suppliers() ' Load suppliers
        load_enums("measure", ComboBoxMeasure)
        load_enums("supply_method", ComboBoxSuMethod)
        populate_filter_combos()
        LoadAuthUsers()
        load_data()


        ' Make Profit Margin Read-Only
        TextBoxProMargin.ReadOnly = True

        ' Configure DataGridView
        TextBoxStockQyt.ReadOnly = True
        TextBoxStockAlert.ReadOnly = False
        DataGridView1.AllowUserToAddRows = False
        DataGridView1.ReadOnly = True
        DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        DataGridView1.MultiSelect = True


        ' Increase internal text size of the GridView
        DataGridView1.DefaultCellStyle.Font = New Font("Microsoft Sans Serif", 14)
        DataGridView1.ColumnHeadersDefaultCellStyle.Font = New Font("Microsoft Sans Serif", 16, FontStyle.Bold)

        ' Configure AutoComplete and DropDown on Focus for specific ComboBoxes
        ConfigureComboBox(ComboBoxBrand)
        ConfigureComboBox(ComboBoxCategory)
        ConfigureComboBox(ComboBoxMeasure)
        ConfigureComboBox(ComboBoxSuMethod)
        ConfigureComboBox(ComboBoxSupplier)
        ConfigureComboBox(cmbAuthUser)

        ' Numeric Validation Wire-up
        Dim numericFields As TextBox() = {TextBoxItemCost, TextBoxAvgCost, TextBoxSellPrice, TextBoxWPrice, TextBoxRPrice, TextBoxDis, TextBoxStockQyt, TextBoxStockAlert, TextBoxProMargin}
        For Each txt In numericFields
            AddHandler txt.KeyPress, AddressOf NumericOnly_KeyPress
        Next

        ' Initialize Filter Type
        cmbFilterType.SelectedIndex = 0 ' Default to Item ID

        ' Hide restricted buttons by default
        btnLogs.Visible = False
        btnBarcode.Visible = False
        btnAddToList.Visible = False
        LabelProMargine.Visible = False
        TextBoxProMargin.Visible = False

        ' Setup button hover/focus feedback
        SetupButtonFeedback()

        ' Enable Double Buffering to fix flickering/shaking
        EnableDoubleBuffered(DataGridView1)

        ' Set initial focus to the Grid
        DataGridView1.Select()

        ' Set next available Item ID
        GetNextItemId()
    End Sub

    Private Sub SetupButtonFeedback()
        Dim buttons As Button() = {btnSuccess, btnUpdate, btnAddNew, btnDelete, btnBarcode, btnLogs}
        For Each btn In buttons
            AddHandler btn.GotFocus, AddressOf Button_Focused
            AddHandler btn.LostFocus, AddressOf Button_Unfocused
            AddHandler btn.MouseEnter, AddressOf Button_Focused
            AddHandler btn.MouseLeave, AddressOf Button_Unfocused
        Next
    End Sub

    Private Sub Button_Focused(sender As Object, e As EventArgs)
        Dim btn = DirectCast(sender, Button)
        btn.Tag = btn.BackColor ' Store original color
        ' Darken the color slightly
        Select Case btn.Name
            Case "btnSuccess" : btn.BackColor = Color.DeepSkyBlue
            Case "btnUpdate" : btn.BackColor = Color.LightGray
            Case "btnAddNew" : btn.BackColor = Color.LightGray
            Case "btnDelete" : btn.BackColor = Color.DarkRed
            Case "btnBarcode" : btn.BackColor = Color.DimGray
            Case "btnLogs" : btn.BackColor = Color.Gray
        End Select
    End Sub

    Private Sub Button_Unfocused(sender As Object, e As EventArgs)
        Dim btn = DirectCast(sender, Button)
        ' Restore original color
        Select Case btn.Name
            Case "btnSuccess" : btn.BackColor = Color.DeepSkyBlue ' Actually it stays the same but typically we'd restore from Tag
            Case "btnUpdate" : btn.BackColor = Color.White
            Case "btnAddNew" : btn.BackColor = Color.White
            Case "btnDelete" : btn.BackColor = Color.Crimson
            Case "btnBarcode" : btn.BackColor = Color.Gray
            Case "btnLogs" : btn.BackColor = Color.DimGray
        End Select
        ' Configure DataGridView1 for easy multi-selection
        DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        DataGridView1.MultiSelect = True
    End Sub

    Private Sub ConfigureComboBox(cmb As ComboBox)
        ' Disable native suggestions to avoid the "double list" issue
        cmb.AutoCompleteMode = AutoCompleteMode.None
        cmb.AutoCompleteSource = AutoCompleteSource.None
    End Sub

    ' Helper to safely get ID from ComboBox
    Private Function GetSelectedId(cmb As ComboBox) As String
        If cmb.SelectedIndex = -1 OrElse cmb.SelectedValue Is Nothing Then Return "NULL"
        Dim val As Object = cmb.SelectedValue
        If TypeOf val Is DataRowView Then
            Return DirectCast(val, DataRowView).Item(cmb.ValueMember).ToString()
        End If
        Return val.ToString()
    End Function

    Private Sub ResetForm()
        txtReadOnly.Clear()
        TextBoxItemName.Clear()
        GetNextItemId()
        TextBoxItemName.Select()
        TextBoxDes.Clear()
        TextBoxAvgCost.Clear()
        TextBoxDis.Clear()
        TextBoxItemCost.Clear()
        TextBoxSellPrice.Clear()
        TextBoxWPrice.Clear()
        TextBoxRPrice.Clear()
        TextBoxStockAlert.Clear()
        TextBoxStockQyt.Clear()
        TextBoxProMargin.Clear()
        ComboBoxSupplier.SelectedIndex = -1
        ComboBoxBrand.SelectedIndex = -1
        ComboBoxCategory.SelectedIndex = -1
        ComboBoxMeasure.SelectedIndex = -1
        ComboBoxSuMethod.SelectedIndex = -1
        
        ' Default user to current logged-in user
        If Not String.IsNullOrEmpty(Module1.UserName) Then
            cmbAuthUser.Text = Module1.UserName
        Else
            cmbAuthUser.SelectedIndex = -1
        End If

        CheckBoxIsActive.Checked = False
        isUpdateMode = False
        TextBoxItemId.Select()
    End Sub

    Private Function ValidateForm() As Boolean
        If cmbAuthUser.SelectedIndex = -1 Then
            MessageBox.Show("Please select a User before performing this action.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            cmbAuthUser.Focus()
            Return False
        End If

        If String.IsNullOrWhiteSpace(TextBoxItemId.Text) Then
            MessageBox.Show("Item ID is Empty")
            TextBoxItemId.Focus()
            TextBoxItemId.SelectAll()
            Return False
        End If

        ' Item Name validation removed as requested

        If String.IsNullOrWhiteSpace(TextBoxDes.Text) Then
            MessageBox.Show("Description is Empty")
            TextBoxDes.Focus()
            TextBoxDes.SelectAll()
            Return False
        End If

        If String.IsNullOrWhiteSpace(TextBoxItemCost.Text) Then
            MessageBox.Show("Cost is Empty")
            TextBoxItemCost.Focus()
            TextBoxItemCost.SelectAll()
            Return False
        End If

        If String.IsNullOrWhiteSpace(TextBoxAvgCost.Text) Then
            MessageBox.Show("Avg Cost is Empty")
            TextBoxAvgCost.Focus()
            TextBoxAvgCost.SelectAll()
            Return False
        End If

        ' Validate that Item Cost and Avg Cost are similar
        Dim itemCostVal As Double = 0
        Dim avgCostVal As Double = 0
        Double.TryParse(TextBoxItemCost.Text, itemCostVal)
        Double.TryParse(TextBoxAvgCost.Text, avgCostVal)

        If itemCostVal <> avgCostVal Then
            MessageBox.Show("Item Cost and Avg Cost should be similar. Please correct the values.", "Validation Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TextBoxAvgCost.Focus()
            TextBoxAvgCost.SelectAll()
            Return False
        End If

        If String.IsNullOrWhiteSpace(TextBoxSellPrice.Text) Then
            MessageBox.Show("Sell Price is Empty")
            TextBoxSellPrice.Focus()
            TextBoxSellPrice.SelectAll()
            Return False
        End If

        ' Validate that selling price is greater than the average cost
        Dim avgCost As Double = 0
        Dim sellPrice As Double = 0
        Double.TryParse(TextBoxAvgCost.Text, avgCost)
        Double.TryParse(TextBoxSellPrice.Text, sellPrice)

        If sellPrice <= avgCost Then
            MessageBox.Show("Selling price must be greater than the average cost.", "Validation Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TextBoxSellPrice.Focus()
            TextBoxSellPrice.SelectAll()
            Return False
        End If

        If String.IsNullOrWhiteSpace(ComboBoxMeasure.Text) Then
            MessageBox.Show("measure is wrong")
            ComboBoxMeasure.Focus()
            Return False
        End If

        If String.IsNullOrWhiteSpace(ComboBoxSuMethod.Text) Then
            MessageBox.Show("Supply method is wrong")
            ComboBoxSuMethod.Focus()
            Return False
        End If

        Return True
    End Function

    ' Manual Filtering Logic
    Private Sub FilterCombo(cmb As ComboBox, dt As DataTable, displayMember As String)
        If dt Is Nothing Then Return

        Dim filterText As String = cmb.Text
        ' Only filter if there's text, otherwise show all
        If String.IsNullOrEmpty(filterText) Then
            dt.DefaultView.RowFilter = ""
        Else
            ' Use RowFilter on the DataTable's DefaultView
            dt.DefaultView.RowFilter = String.Format("{0} LIKE '%{1}%'", displayMember, filterText.Replace("'", "''"))
        End If

        ' Restore the user's typed text to prevent auto-selecting the first item
        cmb.Text = filterText

        ' Ensure dropdown is visible but don't steal focus
        If Not cmb.DroppedDown Then
            cmb.DroppedDown = True
        End If

        ' Keep cursor at the end
        cmb.SelectionStart = filterText.Length
        cmb.SelectionLength = 0
        Cursor.Current = Cursors.Default
    End Sub

    Private Sub ComboBoxBrand_KeyUp(sender As Object, e As KeyEventArgs) Handles ComboBoxBrand.KeyUp
        ' Skip navigation keys
        If e.KeyCode = Keys.Up Or e.KeyCode = Keys.Down Or e.KeyCode = Keys.Left Or e.KeyCode = Keys.Right Or e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Escape Then Return
        FilterCombo(ComboBoxBrand, dtBrands, "name")
    End Sub

    Private Sub ComboBoxCategory_KeyUp(sender As Object, e As KeyEventArgs) Handles ComboBoxCategory.KeyUp
        If e.KeyCode = Keys.Up Or e.KeyCode = Keys.Down Or e.KeyCode = Keys.Left Or e.KeyCode = Keys.Right Or e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Escape Then Return
        FilterCombo(ComboBoxCategory, dtCategories, "name")
    End Sub

    Private Sub ComboBoxMeasure_KeyUp(sender As Object, e As KeyEventArgs) Handles ComboBoxMeasure.KeyUp
        If e.KeyCode = Keys.Up Or e.KeyCode = Keys.Down Or e.KeyCode = Keys.Left Or e.KeyCode = Keys.Right Or e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Escape Then Return
        FilterCombo(ComboBoxMeasure, dtMeasures, "val")
    End Sub

    Private Sub ComboBoxSuMethod_KeyUp(sender As Object, e As KeyEventArgs) Handles ComboBoxSuMethod.KeyUp
        If e.KeyCode = Keys.Up Or e.KeyCode = Keys.Down Or e.KeyCode = Keys.Left Or e.KeyCode = Keys.Right Or e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Escape Then Return
        FilterCombo(ComboBoxSuMethod, dtSuMethods, "val")
    End Sub

    Private Sub cmbAuthUser_KeyUp(sender As Object, e As KeyEventArgs) Handles cmbAuthUser.KeyUp
        If e.KeyCode = Keys.Up Or e.KeyCode = Keys.Down Or e.KeyCode = Keys.Left Or e.KeyCode = Keys.Right Or e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Escape Then Return
        FilterCombo(cmbAuthUser, dtUsers, "name")
    End Sub

    Private Sub cmbAuthUser_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbAuthUser.SelectedIndexChanged
        txtReadOnly.Clear()
    End Sub

    Private Sub ComboBoxSupplier_KeyUp(sender As Object, e As KeyEventArgs) Handles ComboBoxSupplier.KeyUp
        If e.KeyCode = Keys.Up Or e.KeyCode = Keys.Down Or e.KeyCode = Keys.Left Or e.KeyCode = Keys.Right Or e.KeyCode = Keys.Enter Or e.KeyCode = Keys.Escape Then Return
        FilterCombo(ComboBoxSupplier, dtSuppliers, "name")
    End Sub

    ' Maintain dropdown on focus
    Private Sub ComboBox_GotFocus(sender As Object, e As EventArgs) Handles ComboBoxBrand.GotFocus, ComboBoxCategory.GotFocus, ComboBoxMeasure.GotFocus, ComboBoxSuMethod.GotFocus, cmbAuthUser.GotFocus, ComboBoxSupplier.GotFocus
        Dim cmb As ComboBox = DirectCast(sender, ComboBox)
        cmb.DroppedDown = True
    End Sub

    Private Sub LoadAuthUsers()
        Try
            dtUsers = New DataTable()
            Dim adapter As New MySqlDataAdapter("SELECT id, name, hiddenSecureKey, password, role_id FROM user WHERE (status IS NULL OR status = 'active') AND LOWER(name) NOT IN ('admin1', 'admin2', 'admin3', 'admin4') ORDER BY name", conn)
            adapter.Fill(dtUsers)
            cmbAuthUser.DataSource = dtUsers
            cmbAuthUser.DisplayMember = "name"
            cmbAuthUser.ValueMember = "id"
            
            ' Default to logged-in user
            If Not String.IsNullOrEmpty(Module1.UserName) Then
                cmbAuthUser.Text = Module1.UserName
            Else
                cmbAuthUser.SelectedIndex = -1
            End If
        Catch ex As Exception
            MessageBox.Show("Error loading users: " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Fetch next available Item ID
    ''' </summary>
    Private Sub GetNextItemId(Optional prefix As String = "ITM")
        Try
            ' Clean prefix: letters only, uppercase
            prefix = System.Text.RegularExpressions.Regex.Replace(prefix.ToUpper().Trim(), "[^A-Z]", "")
            If String.IsNullOrEmpty(prefix) Then prefix = "ITM"

            Dim nextId As Integer = 1
            ' Fetch all numeric suffixes for this prefix
            Dim sql As String = "SELECT CAST(SUBSTRING(id, " & (prefix.Length + 2) & ") AS UNSIGNED) FROM items WHERE id LIKE '" & prefix & "-%'"
            Dim cmd As New MySqlCommand(sql, conn)

            If conn.State = ConnectionState.Closed Then conn.Open()
            
            Dim existingIds As New List(Of Integer)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                While reader.Read()
                    If Not reader.IsDBNull(0) Then
                        existingIds.Add(Convert.ToInt32(reader(0)))
                    End If
                End While
            End Using

            ' Sort IDs to find the first missing number (gap)
            existingIds.Sort()
            For Each id In existingIds
                If id = nextId Then
                    nextId += 1
                ElseIf id > nextId Then
                    ' Found a gap
                    Exit For
                End If
            Next

            ' Format as PREFIX-001 (3-digit zero-padded)
            TextBoxItemId.Text = prefix & "-" & nextId.ToString("D3")
        Catch ex As Exception
            TextBoxItemId.Clear()
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub TextBoxItemId_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBoxItemId.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True ' Prevent ding sound
            Dim currentText As String = TextBoxItemId.Text.Trim()
            
            ' Extract prefix if a hyphen already exists, otherwise use typed text as prefix
            Dim userPrefix As String = currentText
            If currentText.Contains("-") Then
                userPrefix = currentText.Split("-"c)(0)
            End If
            
            If String.IsNullOrEmpty(userPrefix) Then
                GetNextItemId()
            Else
                GetNextItemId(userPrefix)
            End If
            
            TextBoxItemName.Select() ' Move to next field
        ElseIf e.KeyCode = Keys.Down Then
            If DataGridView1.Rows.Count > 0 Then
                DataGridView1.Focus()
                e.SuppressKeyPress = True
            End If
        End If
    End Sub

    ''' <summary>
    ''' Sanitize input: Trim, remove special chars, convert to Uppercase
    ''' </summary>
    Private Function SanitizeInput(ByVal input As String) As String
        If String.IsNullOrWhiteSpace(input) Then Return ""
        
        ' 1. Remove special characters: ' " \ ;
        Dim cleaned As String = input.Replace("'", "").Replace("""", "").Replace("\", "").Replace(";", "")
        
        ' 2. Normalize whitespace (remove multiple spaces)
        cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, "\s+", " ")
        
        ' 3. Trim and Uppercase
        Return cleaned.Trim().ToUpper()
    End Function

    ''' <summary>
    ''' Restrict input to numbers and one decimal point
    ''' </summary>
    Private Sub NumericOnly_KeyPress(sender As Object, e As KeyPressEventArgs)
        Dim txt As TextBox = DirectCast(sender, TextBox)
        
        ' Allow digits, backspace, and one decimal point
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) AndAlso (e.KeyChar <> "."c) Then
            e.Handled = True
        End If

        ' Allow only one decimal point
        If (e.KeyChar = "."c) AndAlso (txt.Text.IndexOf("."c) > -1) Then
            e.Handled = True
        End If
    End Sub

    ' Variable to track authentication state
    Dim isAuthorized As Boolean = False
    Dim isStockAuthorized As Boolean = False

    Private Sub txtReadOnly_TextChanged(sender As Object, e As EventArgs) Handles txtReadOnly.TextChanged
        Dim inputText As String = txtReadOnly.Text.Trim()
        isAuthorized = False
        isStockAuthorized = False

        ' If a user is selected, validate password or hidden key
        If cmbAuthUser.SelectedIndex <> -1 AndAlso cmbAuthUser.SelectedItem IsNot Nothing Then
            Dim row As DataRowView = DirectCast(cmbAuthUser.SelectedItem, DataRowView)
            Dim hiddenKey As String = row("hiddenSecureKey").ToString()
            Dim password As String = ""
            
            If row.Row.Table.Columns.Contains("password") AndAlso Not IsDBNull(row("password")) Then
                password = row("password").ToString()
            End If

            ' Authenticate if input matches the actual password, or a non-zero/non-empty hidden key
            Dim isPasswordMatch As Boolean = False
            If Not String.IsNullOrEmpty(password) AndAlso password = inputText Then
                isPasswordMatch = True
            End If
            
            If Not isPasswordMatch AndAlso Not String.IsNullOrEmpty(hiddenKey) AndAlso hiddenKey <> "0" AndAlso hiddenKey = inputText Then
                isPasswordMatch = True
            End If

            If isPasswordMatch Then
                isAuthorized = True
                
                ' Check if user is Admin or Owner (role_id 2 or 3)
                If row.Row.Table.Columns.Contains("role_id") AndAlso Not IsDBNull(row("role_id")) Then
                    Dim roleId As String = row("role_id").ToString()
                    If roleId = "2" OrElse roleId = "3" Then
                        isStockAuthorized = True
                    End If
                End If
            End If
        End If

        If isAuthorized Then
            ' Authorized - Enable sensitive fields based on role
            If isStockAuthorized Then
                TextBoxStockQyt.ReadOnly = False
            Else
                TextBoxStockQyt.ReadOnly = True
            End If
            
            ' Show Logs button ONLY for Owner role (role_id = 3)
            Dim isOwnerRole As Boolean = False
            If cmbAuthUser.SelectedIndex <> -1 AndAlso cmbAuthUser.SelectedItem IsNot Nothing Then
                Dim row As DataRowView = DirectCast(cmbAuthUser.SelectedItem, DataRowView)
                If row.Row.Table.Columns.Contains("role_id") AndAlso Not IsDBNull(row("role_id")) Then
                    If row("role_id").ToString() = "3" Then
                        isOwnerRole = True
                    End If
                End If
            End If

            btnLogs.Visible = isOwnerRole

            btnBarcode.Visible = True
            btnAddToList.Visible = True
        Else
            ' Unauthorized
            TextBoxStockQyt.ReadOnly = True
            btnLogs.Visible = False
            btnBarcode.Visible = False
            btnAddToList.Visible = False
        End If
        ' Stock alert is always editable
        TextBoxStockAlert.ReadOnly = False
    End Sub

    Private Sub CalculateProfitMargin()
        Dim cost As Double = 0
        Dim sellPrice As Double = 0
        Double.TryParse(TextBoxItemCost.Text, cost)
        Double.TryParse(TextBoxSellPrice.Text, sellPrice)

        If cost > 0 Then
            Dim margin As Double = ((sellPrice - cost) / cost) * 100
            TextBoxProMargin.Text = margin.ToString("F2")
        Else
            TextBoxProMargin.Text = "0.00"
        End If
    End Sub

    Private Sub TextBoxItemCost_TextChanged(sender As Object, e As EventArgs) Handles TextBoxItemCost.TextChanged
        TextBoxAvgCost.Text = TextBoxItemCost.Text
        CalculateProfitMargin()
    End Sub

    Private Sub TextBoxSellPrice_TextChanged(sender As Object, e As EventArgs) Handles TextBoxSellPrice.TextChanged
        TextBoxRPrice.Text = TextBoxSellPrice.Text
        CalculateProfitMargin()
    End Sub

    Private Sub TextBoxItemName_TextChanged(sender As Object, e As EventArgs) Handles TextBoxItemName.TextChanged
        TextBoxDes.Text = TextBoxItemName.Text
    End Sub



    Private Sub populate_filter_combos()
        Try

            ' Populate Brand filter
            Dim dtBrand As New DataTable()
            Dim adapterBrand As New MySqlDataAdapter("SELECT id, name FROM brand ORDER BY name", conn)
            adapterBrand.Fill(dtBrand)
            ComboBoxFBrand.DataSource = dtBrand
            ComboBoxFBrand.DisplayMember = "name"
            ComboBoxFBrand.ValueMember = "id"
            ComboBoxFBrand.SelectedIndex = -1

            ' Populate Category filter
            Dim dtCat As New DataTable()
            Dim adapterCat As New MySqlDataAdapter("SELECT id, name FROM category ORDER BY name", conn)
            adapterCat.Fill(dtCat)
            ComboBoxFCategory.DataSource = dtCat
            ComboBoxFCategory.DisplayMember = "name"
            ComboBoxFCategory.ValueMember = "id"
            ComboBoxFCategory.SelectedIndex = -1

        Catch ex As Exception
            MessageBox.Show("Error populating filter combos: " & ex.Message)
        End Try
    End Sub

    Public Sub apply_filters(Optional forceReload As Boolean = False)
        Try
            If dtItems Is Nothing OrElse forceReload Then
                If conn.State = ConnectionState.Closed Then conn.Open()
                dtItems = New DataTable()
                Dim adapter As New MySqlDataAdapter("select id, REPLACE(id, '-', '') as id_no_dash, item_name, description, item_cost, avg_cost, selling_price, whole_selling_price, retail_selling_price, category_id, brand_id, supplier_id, profit_margin, supply_method, measure, IFNULL((SELECT SUM(st_qty) FROM items_stock WHERE item_id = items.id), 0) as st_qty, stock_alert, is_active, IFNULL(updated_at, created_at) as Last_Modified from items WHERE deleted_at IS NULL order by item_name", conn)
                adapter.Fill(dtItems)
                conn.Close()
            End If

            Dim dv As New DataView(dtItems)
            Dim filterList As New List(Of String)



            If Not String.IsNullOrEmpty(TextBoxFItemName.Text) Then
                Dim searchText As String = TextBoxFItemName.Text.Replace("'", "''").Replace("-", "")
                Dim rawSearch As String = TextBoxFItemName.Text.Replace("'", "''")
                If cmbFilterType.Text = "All" Then
                    filterList.Add(String.Format("(id LIKE '%{0}%' OR id_no_dash LIKE '%{1}%' OR item_name LIKE '%{0}%' OR description LIKE '%{0}%')", rawSearch, searchText))
                ElseIf cmbFilterType.Text = "Item ID" Then
                    filterList.Add(String.Format("(id LIKE '%{0}%' OR id_no_dash LIKE '%{1}%')", rawSearch, searchText))
                Else
                    filterList.Add(String.Format("item_name LIKE '%{0}%'", rawSearch))
                End If
            End If

            If Not String.IsNullOrEmpty(TextBoxFDescription.Text) Then
                Dim descKey As String = TextBoxFDescription.Text.Trim().Replace("'", "''")
                Dim words = descKey.Split(New Char() {" "c}, StringSplitOptions.RemoveEmptyEntries)
                For Each word In words
                    filterList.Add(String.Format("description LIKE '%{0}%'", word))
                Next
            End If

            If ComboBoxFBrand.SelectedIndex <> -1 AndAlso ComboBoxFBrand.SelectedValue IsNot Nothing Then
                Dim val As Object = ComboBoxFBrand.SelectedValue
                If TypeOf val Is DataRowView Then val = DirectCast(val, DataRowView).Item("id")
                filterList.Add(String.Format("brand_id = {0}", val))
            End If

            If ComboBoxFCategory.SelectedIndex <> -1 AndAlso ComboBoxFCategory.SelectedValue IsNot Nothing Then
                Dim val As Object = ComboBoxFCategory.SelectedValue
                If TypeOf val Is DataRowView Then val = DirectCast(val, DataRowView).Item("id")
                filterList.Add(String.Format("category_id = {0}", val))
            End If

            If filterList.Count > 0 Then
                dv.RowFilter = String.Join(" AND ", filterList)
            End If

            If Not String.IsNullOrEmpty(TextBoxFDescription.Text) Then
                dv.Sort = "description ASC"
            ElseIf cmbFilterType.Text = "Item ID" Then
                dv.Sort = "id ASC"
            Else
                dv.Sort = "description ASC"
            End If
            DataGridView1.DataSource = dv
            setup_grid_columns()
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then conn.Close()
            MessageBox.Show("Error applying filters: " & ex.Message)
        End Try
    End Sub

    Private Sub TextBoxFItemName_TextChanged(sender As Object, e As EventArgs) Handles TextBoxFItemName.TextChanged
        apply_filters()
    End Sub

    Private Sub ComboBoxFBrand_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxFBrand.SelectedIndexChanged
        apply_filters()
    End Sub

    Private Sub cmbFilterType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbFilterType.SelectedIndexChanged
        apply_filters()
    End Sub

    Private Sub ComboBoxFCategory_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxFCategory.SelectedIndexChanged
        apply_filters()
    End Sub

    Private Sub TextBoxFDescription_TextChanged(sender As Object, e As EventArgs) Handles TextBoxFDescription.TextChanged
        apply_filters()
    End Sub


    Public Sub load_data()
        apply_filters()
    End Sub

    Private Sub setup_grid_columns()
        If DataGridView1.Columns.Count = 0 Then Return

        ' Grid restrictions for select-only operation
        DataGridView1.ReadOnly = True
        DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        DataGridView1.AllowUserToAddRows = False
        DataGridView1.AllowUserToDeleteRows = False
        DataGridView1.EditMode = DataGridViewEditMode.EditProgrammatically

        DataGridView1.Columns(0).HeaderText = "Item ID"
        DataGridView1.Columns(0).Width = 100
        DataGridView1.Columns(0).DisplayIndex = 0

        DataGridView1.Columns(1).Visible = False ' Hide id_no_dash (used for search only)

        DataGridView1.Columns(2).Visible = False ' Hide Item Name

        DataGridView1.Columns(3).HeaderText = "Description"
        DataGridView1.Columns(3).Width = 600
        DataGridView1.Columns(3).DisplayIndex = 1

        ' Price Columns (DisplayIndex 2-6)
        DataGridView1.Columns(4).HeaderText = "Cost Price"
        DataGridView1.Columns(4).Width = 120
        DataGridView1.Columns(4).DisplayIndex = 2

        DataGridView1.Columns(5).HeaderText = "Avg Cost"
        DataGridView1.Columns(5).Width = 120
        DataGridView1.Columns(5).DisplayIndex = 3

        DataGridView1.Columns(6).HeaderText = "Sell Price"
        DataGridView1.Columns(6).Width = 120
        DataGridView1.Columns(6).DisplayIndex = 4

        DataGridView1.Columns(7).HeaderText = "Whole Price"
        DataGridView1.Columns(7).Width = 120
        DataGridView1.Columns(7).DisplayIndex = 5

        DataGridView1.Columns(8).HeaderText = "Retail Price"
        DataGridView1.Columns(8).Width = 120
        DataGridView1.Columns(8).DisplayIndex = 6

        ' Stock Qty moved to DisplayIndex 7 (After prices)
        DataGridView1.Columns(15).HeaderText = "Stock Qyt"
        DataGridView1.Columns(15).Width = 100
        DataGridView1.Columns(15).DefaultCellStyle.ForeColor = Color.Crimson
        DataGridView1.Columns(15).DefaultCellStyle.Font = New Font("Segoe UI", 14, FontStyle.Bold)
        DataGridView1.Columns(15).DisplayIndex = 7

        ' Remaining Columns
        DataGridView1.Columns(9).HeaderText = "Category"
        DataGridView1.Columns(9).Width = 90
        DataGridView1.Columns(9).DisplayIndex = 8

        DataGridView1.Columns(10).HeaderText = "Brand"
        DataGridView1.Columns(10).Width = 90
        DataGridView1.Columns(10).DisplayIndex = 9

        DataGridView1.Columns(11).HeaderText = "Supplier"
        DataGridView1.Columns(11).DisplayIndex = 10

        DataGridView1.Columns(12).HeaderText = "Profit %"
        DataGridView1.Columns(12).Width = 100
        DataGridView1.Columns(12).DisplayIndex = 11

        DataGridView1.Columns(13).HeaderText = "Supply"
        DataGridView1.Columns(13).Width = 100
        DataGridView1.Columns(13).DisplayIndex = 12

        DataGridView1.Columns(14).HeaderText = "Measurement"
        DataGridView1.Columns(14).Width = 120
        DataGridView1.Columns(14).DisplayIndex = 13

        DataGridView1.Columns(16).HeaderText = "Stock Alert"
        DataGridView1.Columns(16).Width = 100
        DataGridView1.Columns(16).DisplayIndex = 14

        DataGridView1.Columns(17).HeaderText = "Status"
        DataGridView1.Columns(17).DisplayIndex = 15

        DataGridView1.Columns(18).HeaderText = "Last Modify"
        DataGridView1.Columns(18).Width = 250
        DataGridView1.Columns(18).DisplayIndex = 16
    End Sub


    Private Sub btnAddNew_Click(sender As Object, e As EventArgs) Handles btnAddNew.Click
        ResetForm()
    End Sub



    ''' <summary>
    ''' Load data in grid data table
    ''' </summary>

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        If e.RowIndex < 0 Then Return
        isUpdateMode = True

        Dim k As Integer = e.RowIndex
        TextBoxItemId.Text = DataGridView1.Rows(k).Cells(0).Value.ToString

        TextBoxItemName.Text = DataGridView1.Rows(k).Cells(2).Value.ToString   ' col 2 = item_name
        TextBoxDes.Text = DataGridView1.Rows(k).Cells(3).Value.ToString
        TextBoxItemCost.Text = DataGridView1.Rows(k).Cells(4).Value.ToString
        TextBoxAvgCost.Text = DataGridView1.Rows(k).Cells(5).Value.ToString
        TextBoxSellPrice.Text = DataGridView1.Rows(k).Cells(6).Value.ToString
        TextBoxWPrice.Text = DataGridView1.Rows(k).Cells(7).Value.ToString
        TextBoxRPrice.Text = DataGridView1.Rows(k).Cells(8).Value.ToString

        If Not IsDBNull(DataGridView1.Rows(k).Cells(9).Value) Then
            ComboBoxCategory.SelectedValue = DataGridView1.Rows(k).Cells(9).Value
        Else
            ComboBoxCategory.SelectedIndex = -1
        End If

        If Not IsDBNull(DataGridView1.Rows(k).Cells(10).Value) Then
            ComboBoxBrand.SelectedValue = DataGridView1.Rows(k).Cells(10).Value
        Else
            ComboBoxBrand.SelectedIndex = -1
        End If

        ' New fields: Supplier and Barcode
        If Not IsDBNull(DataGridView1.Rows(k).Cells(11).Value) Then
            ComboBoxSupplier.SelectedValue = DataGridView1.Rows(k).Cells(11).Value
        Else
            ComboBoxSupplier.SelectedIndex = -1
        End If

        TextBoxProMargin.Text = DataGridView1.Rows(k).Cells(12).Value.ToString
        ComboBoxSuMethod.Text = DataGridView1.Rows(k).Cells(13).Value.ToString
        ComboBoxMeasure.Text = DataGridView1.Rows(k).Cells(14).Value.ToString
        ' Show existing stock on selection
        TextBoxStockQyt.Text = DataGridView1.Rows(k).Cells(15).Value.ToString()
        TextBoxStockAlert.Text = DataGridView1.Rows(k).Cells(16).Value.ToString

        If Not IsDBNull(DataGridView1.Rows(k).Cells(17).Value) Then
            CheckBoxIsActive.Checked = Convert.ToBoolean(DataGridView1.Rows(k).Cells(17).Value)
        Else
            CheckBoxIsActive.Checked = True
        End If


    End Sub


    ''' <summary>
    ''' button sucess action
    ''' </summary>

    Private Sub btnSuccess_Click(sender As Object, e As EventArgs) Handles btnSuccess.Click
        If Not ValidateForm() Then Return

        Dim nitId As String = TextBoxItemId.Text.Trim()
        Dim newDes As String = SanitizeInput(TextBoxDes.Text)
        Dim ItName As String = SanitizeInput(TextBoxItemName.Text)

        ' Update UI to show sanitized values
        TextBoxItemName.Text = ItName
        TextBoxDes.Text = newDes

        Try
            MySqlConn.Open()
            Dim catId As String = GetSelectedId(ComboBoxCategory)
            Dim brandId As String = GetSelectedId(ComboBoxBrand)
            Dim supplierId As String = GetSelectedId(ComboBoxSupplier)
            Dim isActive As Integer = If(CheckBoxIsActive.Checked, 1, 0)

            ' --- Duplicate Entry Protection ---
            ' Check ID
            Dim checkIdCmd As New MySqlCommand("SELECT COUNT(*) FROM items WHERE id='" & nitId & "' AND deleted_at IS NULL", MySqlConn)
            If Convert.ToInt32(checkIdCmd.ExecuteScalar()) > 0 Then
                MySqlConn.Close()
                MessageBox.Show("This Item ID '" & nitId & "' already exists.", "Duplicate ID", MessageBoxButtons.OK, MessageBoxIcon.Error)
                TextBoxItemId.Focus()
                TextBoxItemId.SelectAll()
                Return
            End If

            ' Check Name
            If Not String.IsNullOrWhiteSpace(ItName) Then
                Dim checkNameCmd As New MySqlCommand("SELECT COUNT(*) FROM items WHERE item_name='" & ItName.Replace("'", "''") & "' AND deleted_at IS NULL", MySqlConn)
                If Convert.ToInt32(checkNameCmd.ExecuteScalar()) > 0 Then
                    MySqlConn.Close()
                    MessageBox.Show("This Item Name '" & ItName & "' already exists. Please use a unique name.", "Duplicate Name", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    TextBoxItemName.Focus()
                    TextBoxItemName.SelectAll()
                    Return
                End If
            End If

            ' Check Description
            Dim checkDesCmd As New MySqlCommand("SELECT COUNT(*) FROM items WHERE description='" & newDes.Replace("'", "''") & "' AND deleted_at IS NULL", MySqlConn)
            If Convert.ToInt32(checkDesCmd.ExecuteScalar()) > 0 Then
                MySqlConn.Close()
                MessageBox.Show("This Description '" & newDes & "' already exists for another item.", "Duplicate Description", MessageBoxButtons.OK, MessageBoxIcon.Error)
                TextBoxDes.Focus()
                TextBoxDes.SelectAll()
                Return
            End If

            Dim stockVal As String = If(String.IsNullOrWhiteSpace(TextBoxStockQyt.Text), "0", TextBoxStockQyt.Text)
            Dim alertVal As String = If(String.IsNullOrWhiteSpace(TextBoxStockAlert.Text), "0", TextBoxStockAlert.Text)
            Dim wPriceVal As String = If(String.IsNullOrWhiteSpace(TextBoxWPrice.Text), "0", TextBoxWPrice.Text)
            Dim rPriceVal As String = If(String.IsNullOrWhiteSpace(TextBoxRPrice.Text), "0", TextBoxRPrice.Text)

            Dim Query As String
            Dim localNow As String = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            If isAuthorized Then
                ' Insert 0 for st_qty initially, let the items_stock trigger add the actual quantity
                Query = "insert into items (id, item_name, description, item_cost, avg_cost, selling_price, whole_selling_price, retail_selling_price, category_id, brand_id, supplier_id, profit_margin, supply_method, measure, st_qty, stock_alert, is_active, created_at) values ('" & nitId & "','" & ItName.Replace("'", "''") & "','" & newDes.Replace("'", "''") & "','" & TextBoxItemCost.Text & "','" & TextBoxAvgCost.Text & "','" & TextBoxSellPrice.Text & "','" & wPriceVal & "','" & rPriceVal & "'," & catId & "," & brandId & "," & supplierId & ",'" & TextBoxProMargin.Text & "','" & ComboBoxSuMethod.Text & "','" & ComboBoxMeasure.Text & "','0','" & alertVal & "'," & isActive & ", '" & localNow & "')"
            Else
                Query = "insert into items (id, item_name, description, item_cost, avg_cost, selling_price, whole_selling_price, retail_selling_price, category_id, brand_id, supplier_id, profit_margin, supply_method, measure, st_qty, stock_alert, is_active, created_at) values ('" & nitId & "','" & ItName.Replace("'", "''") & "','" & newDes.Replace("'", "''") & "','" & TextBoxItemCost.Text & "','" & TextBoxAvgCost.Text & "','" & TextBoxSellPrice.Text & "','" & wPriceVal & "','" & rPriceVal & "'," & catId & "," & brandId & "," & supplierId & ",'" & TextBoxProMargin.Text & "','" & ComboBoxSuMethod.Text & "','" & ComboBoxMeasure.Text & "','0','" & alertVal & "'," & isActive & ", '" & localNow & "')"
            End If

            Dim COMMAND As New MySqlCommand(Query, MySqlConn)
            COMMAND.ExecuteNonQuery()

            ' Insert into items_stock to show in Current Stock
            If isStockAuthorized AndAlso Val(stockVal) > 0 Then
                Try
                    Dim newStockId As String = Guid.NewGuid().ToString()
                    Dim stockInsQuery As String = "INSERT INTO items_stock (id, item_id, st_qty, qty_purchased, item_cost, avg_cost, selling_price, whole_selling_price, retail_selling_price, location_id, supplier_id, date) " &
                                                  "VALUES ('" & newStockId & "', '" & nitId & "', " & stockVal & ", " & stockVal & ", '" & TextBoxItemCost.Text & "', '" & TextBoxAvgCost.Text & "', '" & TextBoxSellPrice.Text & "', '" & wPriceVal & "', '" & rPriceVal & "', COALESCE((SELECT id FROM location WHERE location_name='MAIN STOCK' LIMIT 1), 1), " & supplierId & ", '" & localNow & "')"
                    Dim cmdStock As New MySqlCommand(stockInsQuery, MySqlConn)
                    cmdStock.ExecuteNonQuery()
                Catch exStock As Exception
                    MessageBox.Show("Stock Error: " & exStock.Message)
                End Try
            End If

            ' Log this new item so Owner can review it in the Logs form
            Try
                Dim addedByUserId As String = If(cmbAuthUser.SelectedValue IsNot Nothing, cmbAuthUser.SelectedValue.ToString(), "NULL")
                Dim addLogQuery As String =
                    "INSERT INTO item_add_log " &
                    "  (item_id, item_name, description, item_cost, avg_cost, selling_price, " &
                    "   whole_selling_price, retail_selling_price, category_id, brand_id, " &
                    "   supplier_id, measure, supply_method, st_qty, stock_alert, added_by, added_at) " &
                    "VALUES ('" & nitId & "', '" & ItName.Replace("'", "''") & "', '" & newDes.Replace("'", "''") & "', " &
                    "'" & TextBoxItemCost.Text & "', '" & TextBoxAvgCost.Text & "', '" & TextBoxSellPrice.Text & "', " &
                    "'" & wPriceVal & "', '" & rPriceVal & "', " & catId & ", " & brandId & ", " & supplierId & ", " &
                    "'" & ComboBoxMeasure.Text & "', '" & ComboBoxSuMethod.Text & "', " &
                    "'" & stockVal & "', '" & alertVal & "', " & addedByUserId & ", '" & localNow & "')"
                Dim cmdAddLog As New MySqlCommand(addLogQuery, MySqlConn)
                cmdAddLog.ExecuteNonQuery()
            Catch exLog As Exception
                ' Logging failure should not stop the main save
                Console.WriteLine("item_add_log insert failed: " & exLog.Message)
            End Try

            ResetForm()
            MessageBox.Show("Item Add To the System")
            AddCurrentItemToBarcode()
            apply_filters(True)
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        Finally
            MySqlConn.Close()
        End Try
    End Sub


    Private Sub TextBoxDes_TextChanged(sender As Object, e As EventArgs) Handles TextBoxDes.TextChanged
        ' TextBoxDes is an edit field, removing the filter call to prevent selection jump
    End Sub



    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        If Not ValidateForm() Then Return

        Dim nitId As String = TextBoxItemId.Text.Trim()
        Dim newDes As String = SanitizeInput(TextBoxDes.Text)
        Dim ItName As String = SanitizeInput(TextBoxItemName.Text)

        ' Update UI to show sanitized values
        TextBoxItemName.Text = ItName
        TextBoxDes.Text = newDes

        Try
            Dim catId As String = GetSelectedId(ComboBoxCategory)
            Dim brandId As String = GetSelectedId(ComboBoxBrand)
            Dim supplierId As String = GetSelectedId(ComboBoxSupplier)
            Dim isActive As Integer = If(CheckBoxIsActive.Checked, 1, 0)
            Dim alertValUpd As String = If(String.IsNullOrWhiteSpace(TextBoxStockAlert.Text), "0", TextBoxStockAlert.Text)
            Dim wPriceValUpd As String = If(String.IsNullOrWhiteSpace(TextBoxWPrice.Text), "0", TextBoxWPrice.Text)
            Dim rPriceValUpd As String = If(String.IsNullOrWhiteSpace(TextBoxRPrice.Text), "0", TextBoxRPrice.Text)

            MySqlConn.Open()

            ' --- Duplicate Entry Protection (Exclude Current ID) ---
            ' Check Name
            If Not String.IsNullOrWhiteSpace(ItName) Then
                Dim checkNameCmd As New MySqlCommand("SELECT COUNT(*) FROM items WHERE item_name='" & ItName.Replace("'", "''") & "' AND id <> '" & nitId & "' AND deleted_at IS NULL", MySqlConn)
                If Convert.ToInt32(checkNameCmd.ExecuteScalar()) > 0 Then
                    MySqlConn.Close()
                    MessageBox.Show("Another item with this Name '" & ItName & "' already exists.", "Duplicate Name", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    TextBoxItemName.Focus()
                    TextBoxItemName.SelectAll()
                    Return
                End If
            End If

            ' Check Description
            Dim checkDesCmd As New MySqlCommand("SELECT COUNT(*) FROM items WHERE description='" & newDes.Replace("'", "''") & "' AND id <> '" & nitId & "' AND deleted_at IS NULL", MySqlConn)
            If Convert.ToInt32(checkDesCmd.ExecuteScalar()) > 0 Then
                MySqlConn.Close()
                MessageBox.Show("Another item with this Description '" & newDes & "' already exists.", "Duplicate Description", MessageBoxButtons.OK, MessageBoxIcon.Error)
                TextBoxDes.Focus()
                TextBoxDes.SelectAll()
                Return
            End If

            Dim oldStock As Double = 0
            Dim diffStock As Double = 0
            Dim localNow As String = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            Dim userId As String = If(cmbAuthUser.SelectedValue IsNot Nothing, cmbAuthUser.SelectedValue.ToString(), "NULL")

            Try
                ' --- Change Logging Logic and Old Stock Calculation ---
                Dim oldData As New DataTable()
                Dim oldAdapter As New MySqlDataAdapter("SELECT *, IFNULL((SELECT SUM(st_qty) FROM items_stock WHERE item_id = items.id), 0) as real_st_qty FROM items WHERE id='" & nitId & "'", MySqlConn)
                oldAdapter.Fill(oldData)

                If oldData.Rows.Count > 0 Then
                    Dim oldRow As DataRow = oldData.Rows(0)
                    oldStock = Val(oldRow("real_st_qty").ToString())
                    diffStock = Val(TextBoxStockQyt.Text) - oldStock

                    Dim logQuery As String = "INSERT INTO item_change_log (item_id, item_name, description, item_cost, avg_cost, selling_price, whole_selling_price, retail_selling_price, category_id, brand_id, supplier_id, profit_margin, supply_method, measure, st_qty, stock_alert, changed_by, changed_at) VALUES (" &
                                "'" & nitId & "', " &
                                If(oldRow("item_name").ToString() <> ItName, "'" & ItName.Replace("'", "''") & "'", "NULL") & ", " &
                                If(oldRow("description").ToString() <> newDes, "'" & newDes.Replace("'", "''") & "'", "NULL") & ", " &
                                If(oldRow("item_cost").ToString() <> TextBoxItemCost.Text, "'" & TextBoxItemCost.Text & "'", "NULL") & ", " &
                                If(oldRow("avg_cost").ToString() <> TextBoxAvgCost.Text, "'" & TextBoxAvgCost.Text & "'", "NULL") & ", " &
                                If(oldRow("selling_price").ToString() <> TextBoxSellPrice.Text, "'" & TextBoxSellPrice.Text & "'", "NULL") & ", " &
                                If(oldRow("whole_selling_price").ToString() <> TextBoxWPrice.Text, "'" & TextBoxWPrice.Text & "'", "NULL") & ", " &
                                If(oldRow("retail_selling_price").ToString() <> TextBoxRPrice.Text, "'" & TextBoxRPrice.Text & "'", "NULL") & ", " &
                                If(oldRow("category_id").ToString() <> catId, catId, "NULL") & ", " &
                                If(oldRow("brand_id").ToString() <> brandId, brandId, "NULL") & ", " &
                                If(oldRow("supplier_id").ToString() <> supplierId, supplierId, "NULL") & ", " &
                                If(oldRow("profit_margin").ToString() <> TextBoxProMargin.Text, "'" & TextBoxProMargin.Text & "'", "NULL") & ", " &
                                If(oldRow("supply_method").ToString() <> ComboBoxSuMethod.Text, "'" & ComboBoxSuMethod.Text & "'", "NULL") & ", " &
                                If(oldRow("measure").ToString() <> ComboBoxMeasure.Text, "'" & ComboBoxMeasure.Text & "'", "NULL") & ", " &
                                If(oldStock <> Val(TextBoxStockQyt.Text), "'" & TextBoxStockQyt.Text & "'", "NULL") & ", " &
                                If(oldRow("stock_alert").ToString() <> TextBoxStockAlert.Text, "'" & TextBoxStockAlert.Text & "'", "NULL") & ", " & userId & ", '" & localNow & "')"

                    Dim logCmd As New MySqlCommand(logQuery, MySqlConn)
                    logCmd.ExecuteNonQuery()
                Else
                    diffStock = Val(TextBoxStockQyt.Text)
                End If
            Catch exLog As Exception
                ' Logging failure shouldn't stop the main update
                If diffStock = 0 AndAlso Val(TextBoxStockQyt.Text) > 0 Then
                     ' Fallback if getting old stock failed
                     diffStock = Val(TextBoxStockQyt.Text)
                End If
            End Try

            Dim Query As String
            If isStockAuthorized Then
                ' If diffStock > 0, the trigger will add diffStock to oldStock. Otherwise, set it to the explicit value.
                Dim targetQty As Double = If(diffStock > 0, oldStock, Val(TextBoxStockQyt.Text))
                Query = "UPDATE items SET id='" & nitId & "', item_name='" & ItName.Replace("'", "''") & "', description='" & newDes.Replace("'", "''") & "', item_cost='" & TextBoxItemCost.Text & "', avg_cost='" & TextBoxAvgCost.Text & "', selling_price='" & TextBoxSellPrice.Text & "', whole_selling_price='" & wPriceValUpd & "', retail_selling_price='" & rPriceValUpd & "', category_id=" & catId & ", brand_id=" & brandId & ", supplier_id=" & supplierId & ", profit_margin='" & TextBoxProMargin.Text & "', supply_method='" & ComboBoxSuMethod.Text & "', measure='" & ComboBoxMeasure.Text & "', st_qty=" & targetQty & ", stock_alert='" & alertValUpd & "', is_active=" & isActive & ", updated_at='" & localNow & "' WHERE id='" & nitId & "'"
            Else
                Query = "UPDATE items SET id='" & nitId & "', item_name='" & ItName.Replace("'", "''") & "', description='" & newDes.Replace("'", "''") & "', item_cost='" & TextBoxItemCost.Text & "', avg_cost='" & TextBoxAvgCost.Text & "', selling_price='" & TextBoxSellPrice.Text & "', whole_selling_price='" & wPriceValUpd & "', retail_selling_price='" & rPriceValUpd & "', category_id=" & catId & ", brand_id=" & brandId & ", supplier_id=" & supplierId & ", profit_margin='" & TextBoxProMargin.Text & "', supply_method='" & ComboBoxSuMethod.Text & "', measure='" & ComboBoxMeasure.Text & "', stock_alert='" & alertValUpd & "', is_active=" & isActive & ", updated_at='" & localNow & "' WHERE id='" & nitId & "'"
            End If

            Dim COMMAND As New MySqlCommand(Query, MySqlConn)
            COMMAND.ExecuteNonQuery()

            ' Update prices in existing current stock
            Try
                Dim updateStockPricesQuery As String = "UPDATE items_stock SET avg_cost='" & TextBoxAvgCost.Text & "', selling_price='" & TextBoxSellPrice.Text & "', whole_selling_price='" & wPriceValUpd & "', retail_selling_price='" & rPriceValUpd & "' WHERE item_id='" & nitId & "' AND st_qty > 0"
                Dim cmdUpdateStock As New MySqlCommand(updateStockPricesQuery, MySqlConn)
                cmdUpdateStock.ExecuteNonQuery()
            Catch exUpdateStock As Exception
                MessageBox.Show("Error updating current stock prices: " & exUpdateStock.Message)
            End Try

            ' Insert into items_stock to show in Current Stock
            If isStockAuthorized AndAlso diffStock > 0 Then
                Try
                    Dim newStockId As String = Guid.NewGuid().ToString()
                    Dim stockInsQuery As String = "INSERT INTO items_stock (id, item_id, st_qty, qty_purchased, item_cost, avg_cost, selling_price, whole_selling_price, retail_selling_price, location_id, supplier_id, date) " &
                                                  "VALUES ('" & newStockId & "', '" & nitId & "', " & diffStock & ", " & diffStock & ", '" & TextBoxItemCost.Text & "', '" & TextBoxAvgCost.Text & "', '" & TextBoxSellPrice.Text & "', '" & wPriceValUpd & "', '" & rPriceValUpd & "', COALESCE((SELECT id FROM location WHERE location_name='MAIN STOCK' LIMIT 1), 1), " & supplierId & ", '" & localNow & "')"
                    Dim cmdStock As New MySqlCommand(stockInsQuery, MySqlConn)
                    cmdStock.ExecuteNonQuery()
                Catch exStock As Exception
                    MessageBox.Show("Stock Error: " & exStock.Message)
                End Try
            ElseIf isStockAuthorized AndAlso diffStock < 0 Then
                Try
                    Dim deductQty As Double = Math.Abs(diffStock)
                    Dim getBatchesQuery As String = "SELECT id, st_qty FROM items_stock WHERE item_id = '" & nitId & "' AND st_qty > 0 ORDER BY date DESC, id DESC"
                    Dim dtBatches As New DataTable()
                    Dim adpBatches As New MySqlDataAdapter(getBatchesQuery, MySqlConn)
                    adpBatches.Fill(dtBatches)

                    For Each batchRow As DataRow In dtBatches.Rows
                        Dim batchId As String = batchRow("id").ToString()
                        Dim batchQty As Double = Val(batchRow("st_qty").ToString())

                        If batchQty >= deductQty Then
                            Dim updQuery As String = "UPDATE items_stock SET st_qty = st_qty - " & deductQty & " WHERE id = '" & batchId & "'"
                            Dim cmdUpd As New MySqlCommand(updQuery, MySqlConn)
                            cmdUpd.ExecuteNonQuery()
                            deductQty = 0
                            Exit For
                        Else
                            Dim updQuery As String = "UPDATE items_stock SET st_qty = 0 WHERE id = '" & batchId & "'"
                            Dim cmdUpd As New MySqlCommand(updQuery, MySqlConn)
                            cmdUpd.ExecuteNonQuery()
                            deductQty -= batchQty
                        End If
                    Next
                Catch exStock As Exception
                    MessageBox.Show("Stock Deduction Error: " & exStock.Message)
                End Try
            End If

            ResetForm()
            MessageBox.Show("DATA UPDATE")
            AddCurrentItemToBarcode()
            apply_filters(True)
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        Finally
            MySqlConn.Close()
        End Try
    End Sub


    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If cmbAuthUser.SelectedIndex = -1 Then
            MessageBox.Show("Please select a User before performing this action.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If isAuthorized Then
            Dim result As DialogResult = MessageBox.Show("Are you Sure to Delete This", "OR Not", MessageBoxButtons.YesNo)
            If result = DialogResult.Yes Then
                Dim COMMAND As MySqlCommand

                Try
                    Dim localNow As String = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    Dim Query As String
                    MySqlConn.Open()
                    Query = "UPDATE items SET deleted_at='" & localNow & "', is_active=0 WHERE id='" & TextBoxItemId.Text & "'"
                    COMMAND = New MySqlCommand(Query, MySqlConn)
                    COMMAND.ExecuteNonQuery()

                    ' Log Deletion
                    Dim userId As String = If(cmbAuthUser.SelectedValue IsNot Nothing, cmbAuthUser.SelectedValue.ToString(), "NULL")
                    Dim logDelQuery As String = "INSERT INTO item_delete_log (item_id, deleted_by, deleted_at) VALUES ('" & TextBoxItemId.Text & "', " & userId & ", '" & localNow & "')"
                    Dim logDelCmd As New MySqlCommand(logDelQuery, MySqlConn)
                    logDelCmd.ExecuteNonQuery()

                    ' Centralized System log deletion
                    Module1.LogDeletion("Item", TextBoxItemId.Text, "Item Name: " & TextBoxItemName.Text & ", Cost: " & TextBoxItemCost.Text & ", Sell Price: " & TextBoxSellPrice.Text)

                    MessageBox.Show("DATA Delete")
                    ResetForm()
                    apply_filters(True)
                Catch ex As Exception
                    MessageBox.Show(ex.Message)
                    MySqlConn.Close()
                End Try
                MySqlConn.Close()
            End If
        Else
            MsgBox("You Are Not Authorized To Delete This Item")
        End If
    End Sub



    Private Sub TextBoxItemName_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBoxItemName.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                TextBoxItemId.Select()
            Else
                TextBoxStockQyt.Select()
            End If
            e.SuppressKeyPress = True
        ElseIf e.KeyCode = Keys.Down Then
            If DataGridView1.Rows.Count > 0 Then
                DataGridView1.Focus()
                e.SuppressKeyPress = True
            End If
        End If
    End Sub

    Private Sub txtReadOnly_KeyDown(sender As Object, e As KeyEventArgs) Handles txtReadOnly.KeyDown
        If e.KeyCode = Keys.Enter Then
            If String.IsNullOrWhiteSpace(txtReadOnly.Text) Then
                TextBoxDes.Select()
            Else
                TextBoxStockQyt.Select()
            End If
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub TextBoxStockQyt_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBoxStockQyt.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                TextBoxItemName.Select()
            Else
                TextBoxStockAlert.Select()
            End If
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub TextBoxStockAlert_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBoxStockAlert.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                TextBoxStockQyt.Select()
            Else
                TextBoxDes.Select()
            End If
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub TextBoxDes_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBoxDes.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                TextBoxStockAlert.Select()
            Else
                TextBoxItemCost.Select()
            End If
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub TextBoxItemCost_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBoxItemCost.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                TextBoxDes.Select()
            Else
                TextBoxSellPrice.Select()
            End If
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub TextBoxSellPrice_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBoxSellPrice.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                TextBoxItemCost.Select()
            Else
                TextBoxWPrice.Select()
            End If
            e.SuppressKeyPress = True
        End If
    End Sub
    Private Sub TextBoxWPrice_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBoxWPrice.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                TextBoxSellPrice.Select()
            Else
                TextBoxRPrice.Select()
            End If
            e.SuppressKeyPress = True
        End If
    End Sub
    Private Sub TextBoxRPrice_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBoxRPrice.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                TextBoxWPrice.Select()
            Else
                TextBoxAvgCost.Select()
            End If
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub TextBoxProMargin_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBoxProMargin.KeyDown
        If e.KeyCode = Keys.Enter Then
            TextBoxAvgCost.Select()
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub TextBoxAvgCost_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBoxAvgCost.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                TextBoxRPrice.Select()
            Else
                TextBoxDis.Select()
            End If
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub TextBoxDis_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBoxDis.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                TextBoxAvgCost.Select()
            Else
                ComboBoxBrand.Select()
            End If
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub ComboBoxBrand_KeyDown(sender As Object, e As KeyEventArgs) Handles ComboBoxBrand.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                TextBoxDis.Select()
            Else
                If ComboBoxBrand.SelectedIndex = -1 AndAlso Not String.IsNullOrWhiteSpace(ComboBoxBrand.Text) Then
                    Brand.Show()
                End If
                ComboBoxCategory.Select()
            End If
            e.SuppressKeyPress = True
        ElseIf e.KeyCode = Keys.Down Then
            If Not ComboBoxBrand.DroppedDown Then
                If DataGridView1.Rows.Count > 0 Then
                    DataGridView1.Focus()
                    e.SuppressKeyPress = True
                End If
            End If
        End If
    End Sub

    Private Sub ComboBoxCategory_KeyDown(sender As Object, e As KeyEventArgs) Handles ComboBoxCategory.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                ComboBoxBrand.Select()
            Else
                If ComboBoxCategory.SelectedIndex = -1 AndAlso Not String.IsNullOrWhiteSpace(ComboBoxCategory.Text) Then
                    category.Show()
                End If
                ComboBoxMeasure.Select()
            End If
            e.SuppressKeyPress = True
        ElseIf e.KeyCode = Keys.Down Then
            If Not ComboBoxCategory.DroppedDown Then
                If DataGridView1.Rows.Count > 0 Then
                    DataGridView1.Focus()
                    e.SuppressKeyPress = True
                End If
            End If
        End If
    End Sub

    Private Sub ComboBoxMeasure_KeyDown(sender As Object, e As KeyEventArgs) Handles ComboBoxMeasure.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                ComboBoxCategory.Select()
            Else
                CheckBoxIsActive.Select()
            End If
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub CheckBoxIsActive_KeyDown(sender As Object, e As KeyEventArgs) Handles CheckBoxIsActive.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                ComboBoxMeasure.Select()
            Else
                ComboBoxSupplier.Select()
            End If
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub ComboBoxSupplier_KeyDown(sender As Object, e As KeyEventArgs) Handles ComboBoxSupplier.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                CheckBoxIsActive.Select()
            Else
                ComboBoxSuMethod.Select()
            End If
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub ComboBoxSuMethod_KeyDown(sender As Object, e As KeyEventArgs) Handles ComboBoxSuMethod.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                ComboBoxSupplier.Select()
            Else
                btnSuccess.PerformClick()
            End If
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub cmbAuthUser_KeyDown(sender As Object, e As KeyEventArgs) Handles cmbAuthUser.KeyDown
        If e.KeyCode = Keys.ShiftKey Then
            txtReadOnly.Select()
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub Item_manage_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.F2 Then
            btnAddNew.PerformClick()
        ElseIf e.KeyCode = Keys.Enter Then
            ' If a button is focused, trigger its click
            If TypeOf Me.ActiveControl Is Button Then
                DirectCast(Me.ActiveControl, Button).PerformClick()
                e.SuppressKeyPress = True
                Exit Sub
            End If

            If Me.ActiveControl Is cmbAuthUser Then
                ' Intelligent button trigger from User selection
                If isUpdateMode Then
                    btnUpdate.PerformClick()
                Else
                    btnSuccess.PerformClick()
                End If
                e.SuppressKeyPress = True
            ElseIf Me.ActiveControl Is CheckBoxIsActive OrElse CheckBoxIsActive.Focused Then
                CheckBoxIsActive.Checked = Not CheckBoxIsActive.Checked
                ComboBoxSupplier.Select()
                e.SuppressKeyPress = True
            End If
        ElseIf e.KeyCode = Keys.F5 Then
            ' Focus first row in Grid
            If DataGridView1.Rows.Count > 0 Then
                DataGridView1.Focus()
                DataGridView1.CurrentCell = DataGridView1.Rows(0).Cells(0)
                e.SuppressKeyPress = True
            End If
        ElseIf e.KeyCode = Keys.F6 Then
            ' Toggle and focus Is Active checkbox
            CheckBoxIsActive.Checked = Not CheckBoxIsActive.Checked
            CheckBoxIsActive.Focus()
            e.SuppressKeyPress = True
        ElseIf e.KeyCode = Keys.F3 Then
            btnUpdate.PerformClick()
        ElseIf e.KeyCode = Keys.Delete Then
            btnDelete.PerformClick()
        ElseIf e.KeyCode = Keys.F1 Then
            If isAuthorized AndAlso btnLogs.Visible Then
                btnLogs.PerformClick()
            End If
        ElseIf e.KeyCode = Keys.F10 Then
            txtReadOnly.Select()
        ElseIf e.KeyCode = Keys.F12 Then
            btnSuccess.PerformClick()
        End If
    End Sub

    Private Sub DataGridView1_KeyDown(sender As Object, e As KeyEventArgs) Handles DataGridView1.KeyDown
        If e.KeyCode = Keys.Enter Then
            If DataGridView1.CurrentRow IsNot Nothing Then
                Dim rowIdx As Integer = DataGridView1.CurrentRow.Index
                ' Programmatically trigger the cell click behavior
                DataGridView1_CellClick(DataGridView1, New DataGridViewCellEventArgs(0, rowIdx))
                TextBoxStockQyt.Focus() ' Move to a standard primary field
                e.SuppressKeyPress = True
            End If
        End If
    End Sub

    Private Sub DataGridView1_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles DataGridView1.CellFormatting
        If e.RowIndex >= 0 AndAlso Not DataGridView1.Rows(e.RowIndex).IsNewRow Then
            ' If the row is selected, let the selection highlight show
            If DataGridView1.Rows(e.RowIndex).Selected Then
                Return
            End If

            ' Data mapping from apply_filters SELECT:
            ' 15=st_qty, 16=stock_alert, 17=is_active, 18=Last_Modified (DateTime)
            Dim st_qty As Double = 0
            Dim stock_alert As Double = 0
            Dim isActive As Boolean = True

            Dim row As DataGridViewRow = DataGridView1.Rows(e.RowIndex)
            If row.Cells.Count > 15 AndAlso Not IsDBNull(row.Cells(15).Value) Then st_qty = Convert.ToDouble(row.Cells(15).Value)
            If row.Cells.Count > 16 AndAlso Not IsDBNull(row.Cells(16).Value) Then stock_alert = Convert.ToDouble(row.Cells(16).Value)
            If row.Cells.Count > 17 AndAlso Not IsDBNull(row.Cells(17).Value) Then isActive = Convert.ToBoolean(row.Cells(17).Value)

            ' Apply status colors only if NOT selected (checked above)
            If Not isActive Then
                e.CellStyle.BackColor = Color.Red
                e.CellStyle.ForeColor = Color.White
            Else
                If st_qty < stock_alert Then
                    e.CellStyle.BackColor = Color.FromArgb(192, 255, 192)
                    e.CellStyle.ForeColor = Color.Black
                ElseIf st_qty = stock_alert Then
                    e.CellStyle.BackColor = Color.Yellow
                    e.CellStyle.ForeColor = Color.Black
                End If
            End If
        End If
    End Sub


    Private Sub NumericInputOnly(sender As Object, e As KeyPressEventArgs) Handles TextBoxItemCost.KeyPress, TextBoxSellPrice.KeyPress, TextBoxAvgCost.KeyPress, TextBoxDis.KeyPress, TextBoxWPrice.KeyPress, TextBoxRPrice.KeyPress
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) AndAlso (e.KeyChar <> "."c) Then
            e.Handled = True
        End If

        ' Only allow one decimal point
        If (e.KeyChar = "."c) AndAlso (DirectCast(sender, TextBox).Text.IndexOf("."c) > -1) Then
            e.Handled = True
        End If
    End Sub

    Private Sub IntegerInputOnly(sender As Object, e As KeyPressEventArgs) Handles TextBoxStockQyt.KeyPress, TextBoxStockAlert.KeyPress
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub


    Private Sub LiveTimer_Tick(sender As Object, e As EventArgs) Handles LiveTimer.Tick
        ' lblLiveTimeDisplay was removed from UI
    End Sub

    Private Sub btnLogs_Click(sender As Object, e As EventArgs) Handles btnLogs.Click
        logs.Show()
    End Sub



    Private Sub btnBarcode_Click(sender As Object, e As EventArgs) Handles btnBarcode.Click
        ' If there are selected rows in the grid, add them all
        If DataGridView1.SelectedRows.Count > 0 Then
            ' Create or show the BarcodePrint form
            Dim barcodeForm As BarcodePrint = GetBarcodeForm()

            For i As Integer = DataGridView1.SelectedRows.Count - 1 To 0 Step -1
                Dim row As DataGridViewRow = DataGridView1.SelectedRows(i)
                If Not row.IsNewRow Then
                    Dim id As String = row.Cells(0).Value.ToString()
                    Dim name As String = If(row.Cells(2).Value IsNot Nothing, row.Cells(2).Value.ToString(), "")
                    Dim des As String = row.Cells(3).Value.ToString()
                    Dim price As String = row.Cells(6).Value.ToString()
                    Dim cost As String = row.Cells(4).Value.ToString()
                    ' Stock Qty is at index 15 in the actual grid
                    Dim qty As String = row.Cells(15).Value.ToString()
                    
                    barcodeForm.AddToGrid(id, name, des, price, qty, cost)
                End If
            Next
            barcodeForm.Show()
            barcodeForm.BringToFront()
        ElseIf Not String.IsNullOrWhiteSpace(TextBoxItemId.Text) Then
            ' Otherwise add the item currently in textboxes
            AddCurrentItemToBarcode()
            Dim barcodeForm As BarcodePrint = GetBarcodeForm()
            barcodeForm.Show()
            barcodeForm.BringToFront()
        Else
            MessageBox.Show("Please select or enter an item first.", "Barcode", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Function GetBarcodeForm() As BarcodePrint
        Static _barcodeForm As BarcodePrint = Nothing
        If _barcodeForm Is Nothing OrElse _barcodeForm.IsDisposed Then
            _barcodeForm = New BarcodePrint()
        End If
        Return _barcodeForm
    End Function

    ''' <summary>
    ''' Adds the item currently loaded in the textboxes to the barcode printing grid.
    ''' </summary>
    Private Sub AddCurrentItemToBarcode()
        If String.IsNullOrWhiteSpace(TextBoxItemId.Text) Then Return

        Dim barcodeForm As BarcodePrint = GetBarcodeForm()

        ' Use TextBoxStockQyt for quantity if PROVIDED and > 0, otherwise default to 1
        ' This satisfies "item qty eka item eken item ekata update wenna" context
        Dim qty As String = "1"
        Dim stockQty As Double = 0
        If Double.TryParse(TextBoxStockQyt.Text, stockQty) AndAlso stockQty > 0 Then
            qty = stockQty.ToString()
        End If

        barcodeForm.AddToGrid(TextBoxItemId.Text, TextBoxItemName.Text, TextBoxDes.Text, TextBoxSellPrice.Text, qty, TextBoxItemCost.Text)
    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        ' Custom Ctrl + Arrow Key Navigation
        Dim isCtrlPressed As Boolean = (keyData And Keys.Control) = Keys.Control
        Dim keyCode As Keys = (keyData And Not Keys.Control)

        If isCtrlPressed AndAlso (keyCode = Keys.Up OrElse keyCode = Keys.Down OrElse keyCode = Keys.Left OrElse keyCode = Keys.Right) Then
            ' Don't navigate if in DataGridView
            If Not (TypeOf Me.ActiveControl Is DataGridView) Then
                Dim forward As Boolean = (keyCode = Keys.Down OrElse keyCode = Keys.Right)

                ' Check for circular wrap
                If forward AndAlso Me.ActiveControl Is ComboBoxFCategory Then
                    cmbAuthUser.Select()
                    Return True
                ElseIf Not forward AndAlso Me.ActiveControl Is cmbAuthUser Then
                    ComboBoxFCategory.Select()
                    Return True
                End If

                Me.SelectNextControl(Me.ActiveControl, forward, True, True, True)
                Return True
            End If
        End If
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

End Class
