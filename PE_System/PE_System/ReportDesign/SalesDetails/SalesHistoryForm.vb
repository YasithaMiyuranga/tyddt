Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared
Imports MySql.Data.MySqlClient

Public Class SalesHistoryForm
    Private _isInitializedExternally As Boolean = False
    Private _isInitializing As Boolean = False
    Private _searchInv As String = ""
    Private _allChecked As Boolean = True
    Private _filterInvType As String = "Normal"
    Private _filterBilling As String = "Cash"
    Private _isAdvance As Boolean = False
    Private _fItemName As String = ""
    Private _fBrand As String = ""
    Private _fDes As String = ""
    Private _currentRpt As CrystalDecisions.CrystalReports.Engine.ReportDocument = Nothing
    Private _lastLoadedPath As String = ""
    Private _isRefreshing As Boolean = False
    Private _isUpdatingSearch As Boolean = False

    Public Sub SetReportContext(reportIndex As Integer, fromDate As DateTime, toDate As DateTime, Optional searchInv As String = "", Optional allChecked As Boolean = True, Optional invType As String = "Normal", Optional bType As String = "Cash", Optional isAdv As Boolean = False, Optional itemName As String = "", Optional brand As String = "", Optional description As String = "")
        _isInitializedExternally = True
        _searchInv = searchInv
        _allChecked = allChecked
        _filterInvType = invType
        _filterBilling = bType
        _isAdvance = isAdv
        _fItemName = itemName
        _fBrand = brand
        _fDes = description

        ' Ensure items are loaded before selection
        PopulateReportTypes()

        ' Prevent combo box change event from triggering LoadReportData during setup
        _isInitializing = True

        If reportIndex >= 0 AndAlso reportIndex < cmbReportType.Items.Count Then
            cmbReportType.SelectedIndex = reportIndex
        End If

        dtpFrom.Value = fromDate.Date
        dtpTo.Value = toDate.Date

        ' Automatically set search text for invoice number or name search
        If Not String.IsNullOrEmpty(searchInv) Then
            If IsCustomerName(searchInv) OrElse IsSupplierName(searchInv) Then
                txtSearchName.Text = searchInv
            Else
                txtSearchInv.Text = searchInv
            End If
        End If

        _isInitializing = False
        LoadReportData()
    End Sub

    Private Sub cmbReportType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbReportType.SelectedIndexChanged
        UpdateSearchUI()
        LoadSearchData() ' Re-populate names for the new report type
        If Not _isInitializing Then
            LoadReportData()
            
            ' Auto-open the customer dropdown if it's a customer/supplier report and the search is empty
            If txtSearchName.Visible AndAlso String.IsNullOrWhiteSpace(txtSearchName.Text) Then
                txtSearchName.Focus()
            End If
        End If
    End Sub

    Private Sub txtSearchName_Click(sender As Object, e As EventArgs) Handles txtSearchName.Click, txtSearchName.Enter
        LoadCustomerGrid(txtSearchName.Text.Trim())
    End Sub

    Private Sub txtSearchName_TextChanged(sender As Object, e As EventArgs) Handles txtSearchName.TextChanged
        If _isUpdatingSearch Then Return
        If Not txtSearchName.Focused Then Return
        LoadCustomerGrid(txtSearchName.Text.Trim())
        
        ' Reset invoices on new search
        _isUpdatingSearch = True
        txtSearchInv.Text = ""
        _isUpdatingSearch = False
        dgvSearchInv.DataSource = Nothing
        dgvSearchInv.Visible = False
    End Sub

    Private Sub LoadCustomerGrid(searchTerm As String)
        Dim idx As Integer = cmbReportType.SelectedIndex
        Dim queryName As String = ""
        Dim safeSearch As String = searchTerm.Replace("'", "''")

        If idx = 26 Then
            queryName = "SELECT DISTINCT city AS name FROM customer WHERE city LIKE '" & safeSearch & "%' AND city IS NOT NULL AND city != '' ORDER BY TRIM(city) ASC"
        ElseIf (idx >= 4 AndAlso idx <= 6) OrElse idx = 20 OrElse idx = 21 OrElse idx = 25 OrElse (idx >= 11 AndAlso idx <= 13) OrElse idx = 15 OrElse idx = 16 Then
            If Not Module1.IsRgrVisible Then
                queryName = "SELECT name FROM customer WHERE name LIKE 'cash%' AND name LIKE '" & safeSearch & "%' ORDER BY TRIM(name) ASC"
            Else
                queryName = "SELECT name FROM customer WHERE name LIKE '" & safeSearch & "%' ORDER BY TRIM(name) ASC"
            End If
        ElseIf (idx >= 7 AndAlso idx <= 9) OrElse idx = 17 OrElse idx = 18 OrElse idx = 14 OrElse idx = 19 OrElse idx = 23 OrElse idx = 24 Then
            queryName = "SELECT name FROM supplier WHERE name LIKE '" & safeSearch & "%' ORDER BY TRIM(name) ASC"
        ElseIf idx = 3 Then
            queryName = "SELECT DISTINCT description AS name FROM items WHERE description LIKE '%" & safeSearch & "%' ORDER BY TRIM(description) ASC LIMIT 50"
        End If

        If queryName <> "" Then
            Try
                Using localConn As New MySqlConnection(Module1.ConnStr)
                    localConn.Open()
                    Using cmd As New MySqlCommand(queryName, localConn)
                        Using da As New MySqlDataAdapter(cmd)
                            Dim dt As New DataTable()
                            da.Fill(dt)
                            
                            ' Add "ALL" option for Master Lists
                            If (idx = 4 OrElse idx = 7) AndAlso String.IsNullOrWhiteSpace(searchTerm) Then
                                Dim newRow As DataRow = dt.NewRow()
                                newRow("name") = "--- ALL ---"
                                dt.Rows.InsertAt(newRow, 0)
                            End If
                            
                            dgvSearchName.DataSource = dt
                            dgvSearchName.Visible = (dt.Rows.Count > 0)
                            dgvSearchName.BringToFront()
                        End Using
                    End Using
                End Using
            Catch ex As Exception
            End Try
        Else
            dgvSearchName.Visible = False
        End If
    End Sub

    Private Sub dgvSearchName_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvSearchName.CellClick
        If e.RowIndex >= 0 Then
            SelectCustomerFromGrid(e.RowIndex)
        End If
    End Sub

    Private Sub SelectCustomerFromGrid(rowIndex As Integer)
        If rowIndex < 0 OrElse rowIndex >= dgvSearchName.Rows.Count Then Return
        Dim selectedName As String = dgvSearchName.Rows(rowIndex).Cells(0).Value.ToString()
        
        If selectedName = "ALL" Then
            selectedName = ""
        End If
        
        _isUpdatingSearch = True
        txtSearchName.Text = selectedName
        txtSearchInv.Text = ""
        _isUpdatingSearch = False
        
        dgvSearchName.Visible = False
        
        ' Fetch relevant invoices for this customer/supplier
        Dim idx As Integer = cmbReportType.SelectedIndex
        Dim queryInv As String = ""
        Dim paramName As String = selectedName.Replace("'", "''")
        
        If (idx >= 11 AndAlso idx <= 12) OrElse idx = 22 Then
            Dim custFilter As String = ""
            Dim finRole As String = If(Module1.FinancialRole IsNot Nothing, Module1.FinancialRole.ToLower(), "")
            Dim canAccessGR As Boolean = (Module1.UserRole.ToLower() = "cashier" AndAlso finRole = "seller") AndAlso Module1.IsRgrVisible
            If Not Module1.IsRgrVisible OrElse finRole = "seller" Then
                If canAccessGR Then
                    custFilter &= " AND IF(printed_inv_no IS NULL OR printed_inv_no = '', inv_no, printed_inv_no) NOT LIKE 'RGR%'"
                Else
                    custFilter &= " AND IF(printed_inv_no IS NULL OR printed_inv_no = '', inv_no, printed_inv_no) NOT LIKE 'GR%' AND IF(printed_inv_no IS NULL OR printed_inv_no = '', inv_no, printed_inv_no) NOT LIKE 'gr%' AND IF(printed_inv_no IS NULL OR printed_inv_no = '', inv_no, printed_inv_no) NOT LIKE 'RGR%'"
                End If
            End If
            If idx = 22 AndAlso finRole = "seller" AndAlso Not canAccessGR Then
                custFilter &= " AND (IF(printed_inv_no IS NULL OR printed_inv_no = '', inv_no, printed_inv_no) LIKE 'EL%' OR IF(printed_inv_no IS NULL OR printed_inv_no = '', inv_no, printed_inv_no) LIKE 'VT%')"
            End If
            queryInv = "SELECT DISTINCT IF(printed_inv_no IS NULL OR printed_inv_no = '', inv_no, printed_inv_no) AS name FROM billing WHERE customer_id IN (SELECT id FROM customer WHERE name = '" & paramName & "')" & custFilter & " ORDER BY name DESC"
        ElseIf idx = 13 Then
            Dim roleFilter As String = ""
            Dim finRole As String = If(Module1.FinancialRole IsNot Nothing, Module1.FinancialRole.ToLower(), "")
            Dim canAccessGR As Boolean = (Module1.UserRole.ToLower() = "cashier" AndAlso finRole = "seller") AndAlso Module1.IsRgrVisible
            If Not Module1.IsRgrVisible OrElse finRole = "seller" Then
                If canAccessGR Then
                    roleFilter = " AND inv_no NOT LIKE 'RGR%'"
                Else
                    roleFilter = " AND inv_no NOT LIKE 'GR%' AND inv_no NOT LIKE 'gr%' AND inv_no NOT LIKE 'RGR%'"
                End If
            End If
            queryInv = "SELECT DISTINCT inv_no AS name FROM sales_return WHERE customer_id IN (SELECT id FROM customer WHERE name = '" & paramName & "')" & roleFilter & " ORDER BY inv_no DESC"
        ElseIf idx = 14 Then
            queryInv = "SELECT DISTINCT pur_id AS name FROM purchasing WHERE supplier_id IN (SELECT id FROM supplier WHERE name = '" & paramName & "') ORDER BY pur_id DESC"
        ElseIf idx = 18 Then
            queryInv = "SELECT DISTINCT pur_id AS name FROM purchase_return WHERE supplier_id IN (SELECT id FROM supplier WHERE name = '" & paramName & "') ORDER BY pur_id DESC"
        ElseIf idx = 15 OrElse idx = 16 Then
            queryInv = "SELECT DISTINCT inv_no AS name FROM quotation_billing WHERE customer_id IN (SELECT id FROM customer WHERE name = '" & paramName & "') ORDER BY inv_no DESC"
        ElseIf idx = 19 Then
            queryInv = "SELECT DISTINCT request_id AS name FROM purchase_request WHERE supplier_id IN (SELECT id FROM supplier WHERE name = '" & paramName & "') ORDER BY request_id DESC"
        End If
        
        If queryInv <> "" Then
            Try
                Using localConn As New MySqlConnection(Module1.ConnStr)
                    localConn.Open()
                    Using cmd As New MySqlCommand(queryInv, localConn)
                        Using da As New MySqlDataAdapter(cmd)
                            Dim dt As New DataTable()
                            da.Fill(dt)
                            dgvSearchInv.DataSource = dt
                            If dt.Rows.Count > 0 Then
                                ' Auto-select first invoice so the report immediately loads!
                                _isUpdatingSearch = True
                                txtSearchInv.Text = dt.Rows(0)("name").ToString()
                                _isUpdatingSearch = False
                                
                                dgvSearchInv.Visible = False
                                LoadReportData()
                            Else
                                LoadReportData()
                            End If
                        End Using
                    End Using
                End Using
            Catch ex As Exception
            End Try
        Else
            LoadReportData()
        End If
    End Sub

    Private Sub txtSearchName_KeyDown(sender As Object, e As KeyEventArgs) Handles txtSearchName.KeyDown
        If dgvSearchName.Visible AndAlso dgvSearchName.Rows.Count > 0 Then
            If e.KeyCode = Keys.Down Then
                e.Handled = True
                dgvSearchName.Focus()
                If dgvSearchName.CurrentRow Is Nothing Then
                    dgvSearchName.CurrentCell = dgvSearchName.Rows(0).Cells(0)
                End If
            ElseIf e.KeyCode = Keys.Enter Then
                e.Handled = True
                e.SuppressKeyPress = True
                If dgvSearchName.CurrentRow IsNot Nothing Then
                    SelectCustomerFromGrid(dgvSearchName.CurrentRow.Index)
                End If
            End If
        End If
    End Sub

    Private Sub dgvSearchName_KeyDown(sender As Object, e As KeyEventArgs) Handles dgvSearchName.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.Handled = True
            e.SuppressKeyPress = True
            If dgvSearchName.CurrentRow IsNot Nothing Then
                SelectCustomerFromGrid(dgvSearchName.CurrentRow.Index)
            End If
        End If
    End Sub

    Private Sub txtSearchName_Leave(sender As Object, e As EventArgs) Handles txtSearchName.Leave
        Me.BeginInvoke(New Action(Sub()
                                      If Not dgvSearchName.Focused Then
                                          dgvSearchName.Visible = False
                                      End If
                                  End Sub))
    End Sub

    Private Sub dgvSearchName_Leave(sender As Object, e As EventArgs) Handles dgvSearchName.Leave
        Me.BeginInvoke(New Action(Sub()
                                      If Not txtSearchName.Focused Then
                                          dgvSearchName.Visible = False
                                      End If
                                  End Sub))
    End Sub

    Private Sub txtSearchInv_Click(sender As Object, e As EventArgs) Handles txtSearchInv.Click, txtSearchInv.Enter
        If dgvSearchInv.DataSource IsNot Nothing AndAlso DirectCast(dgvSearchInv.DataSource, DataTable).Rows.Count > 0 Then
            dgvSearchInv.Visible = True
            dgvSearchInv.BringToFront()
        Else
            LoadInvoiceGrid(txtSearchInv.Text.Trim())
        End If
    End Sub

    Private Sub txtSearchInv_TextChanged(sender As Object, e As EventArgs) Handles txtSearchInv.TextChanged
        If _isUpdatingSearch Then Return
        If Not txtSearchInv.Focused Then Return
        LoadInvoiceGrid(txtSearchInv.Text.Trim())
    End Sub

    Private Sub LoadInvoiceGrid(searchTerm As String)
        Dim idx As Integer = cmbReportType.SelectedIndex
        Dim queryInv As String = ""
        Dim safeSearch As String = searchTerm.Replace("'", "''")

        ' If a customer is selected, restrict the invoice search to that customer
        Dim custFilter As String = ""
        Dim custName As String = txtSearchName.Text.Trim().Replace("'", "''")
        
        If (idx >= 11 AndAlso idx <= 12) OrElse idx = 22 Then
            If custName <> "" Then custFilter = " AND customer_id IN (SELECT id FROM customer WHERE name = '" & custName & "')"
            Dim finRole As String = If(Module1.FinancialRole IsNot Nothing, Module1.FinancialRole.ToLower(), "")
            Dim canAccessGR As Boolean = (Module1.UserRole.ToLower() = "cashier" AndAlso finRole = "seller") AndAlso Module1.IsRgrVisible
            If Not Module1.IsRgrVisible OrElse finRole = "seller" Then
                If canAccessGR Then
                    custFilter &= " AND IF(printed_inv_no IS NULL OR printed_inv_no = '', inv_no, printed_inv_no) NOT LIKE 'RGR%'"
                Else
                    custFilter &= " AND IF(printed_inv_no IS NULL OR printed_inv_no = '', inv_no, printed_inv_no) NOT LIKE 'GR%' AND IF(printed_inv_no IS NULL OR printed_inv_no = '', inv_no, printed_inv_no) NOT LIKE 'gr%' AND IF(printed_inv_no IS NULL OR printed_inv_no = '', inv_no, printed_inv_no) NOT LIKE 'RGR%'"
                End If
            End If
            If idx = 22 AndAlso finRole = "seller" AndAlso Not canAccessGR Then
                custFilter &= " AND (IF(printed_inv_no IS NULL OR printed_inv_no = '', inv_no, printed_inv_no) LIKE 'EL%' OR IF(printed_inv_no IS NULL OR printed_inv_no = '', inv_no, printed_inv_no) LIKE 'VT%')"
            End If
            queryInv = "SELECT DISTINCT IF(printed_inv_no IS NULL OR printed_inv_no = '', inv_no, printed_inv_no) AS name FROM billing WHERE IF(printed_inv_no IS NULL OR printed_inv_no = '', inv_no, printed_inv_no) LIKE '" & safeSearch & "%'" & custFilter & " ORDER BY name DESC"
        ElseIf idx = 13 Then
            If custName <> "" Then custFilter = " AND customer_id IN (SELECT id FROM customer WHERE name = '" & custName & "')"
            Dim finRole As String = If(Module1.FinancialRole IsNot Nothing, Module1.FinancialRole.ToLower(), "")
            Dim canAccessGR As Boolean = (Module1.UserRole.ToLower() = "cashier" AndAlso finRole = "seller") AndAlso Module1.IsRgrVisible
            If Not Module1.IsRgrVisible OrElse finRole = "seller" Then
                If canAccessGR Then
                    custFilter &= " AND inv_no NOT LIKE 'RGR%'"
                Else
                    custFilter &= " AND inv_no NOT LIKE 'GR%' AND inv_no NOT LIKE 'gr%' AND inv_no NOT LIKE 'RGR%'"
                End If
            End If
            queryInv = "SELECT DISTINCT inv_no AS name FROM sales_return WHERE inv_no LIKE '" & safeSearch & "%'" & custFilter & " ORDER BY inv_no DESC"
        ElseIf idx = 14 Then
            If custName <> "" Then custFilter = " AND supplier_id IN (SELECT id FROM supplier WHERE name = '" & custName & "')"
            queryInv = "SELECT DISTINCT pur_id AS name FROM purchasing WHERE pur_id LIKE '" & safeSearch & "%'" & custFilter & " ORDER BY pur_id DESC"
        ElseIf idx = 18 Then
            If custName <> "" Then custFilter = " AND supplier_id IN (SELECT id FROM supplier WHERE name = '" & custName & "')"
            queryInv = "SELECT DISTINCT pur_id AS name FROM purchase_return WHERE pur_id LIKE '" & safeSearch & "%'" & custFilter & " ORDER BY pur_id DESC"
        ElseIf idx = 15 OrElse idx = 16 Then
            If custName <> "" Then custFilter = " AND customer_id IN (SELECT id FROM customer WHERE name = '" & custName & "')"
            queryInv = "SELECT DISTINCT inv_no AS name FROM quotation_billing WHERE inv_no LIKE '" & safeSearch & "%'" & custFilter & " ORDER BY inv_no DESC"
        ElseIf idx = 19 Then
            If custName <> "" Then custFilter = " AND supplier_id IN (SELECT id FROM supplier WHERE name = '" & custName & "')"
            queryInv = "SELECT DISTINCT request_id AS name FROM purchase_request WHERE request_id LIKE '" & safeSearch & "%'" & custFilter & " ORDER BY request_id DESC"
        End If

        If queryInv <> "" Then
            Try
                Using localConn As New MySqlConnection(Module1.ConnStr)
                    localConn.Open()
                    Using cmd As New MySqlCommand(queryInv, localConn)
                        Using da As New MySqlDataAdapter(cmd)
                            Dim dt As New DataTable()
                            da.Fill(dt)
                            If idx <> 19 AndAlso idx <> 14 Then
                                Dim elRow As DataRow = dt.NewRow()
                                elRow("name") = "EL Bills"
                                dt.Rows.InsertAt(elRow, 0)
                            End If
                            
                            Dim allRow As DataRow = dt.NewRow()
                            allRow("name") = "All"
                            dt.Rows.InsertAt(allRow, 0)
                            
                            dgvSearchInv.DataSource = dt
                            dgvSearchInv.Visible = (dt.Rows.Count > 0)
                            dgvSearchInv.BringToFront()
                        End Using
                    End Using
                End Using
            Catch ex As Exception
            End Try
        Else
            dgvSearchInv.Visible = False
        End If
    End Sub

    Private Sub dgvSearchInv_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvSearchInv.CellClick
        If e.RowIndex >= 0 Then
            SelectInvoiceFromGrid(e.RowIndex)
        End If
    End Sub

    Private Sub SelectInvoiceFromGrid(rowIndex As Integer)
        If rowIndex < 0 OrElse rowIndex >= dgvSearchInv.Rows.Count Then Return
        
        Dim selectedInv As String = dgvSearchInv.Rows(rowIndex).Cells(0).Value.ToString()
        _isUpdatingSearch = True
        txtSearchInv.Text = selectedInv
        _isUpdatingSearch = False
        
        If selectedInv.ToLower() <> "all" AndAlso selectedInv.ToLower() <> "el bills" Then
            chkAllDates.Checked = True
        End If
        
        dgvSearchInv.Visible = False
        LoadReportData()
    End Sub

    Private Sub txtSearchInv_KeyDown(sender As Object, e As KeyEventArgs) Handles txtSearchInv.KeyDown
        If dgvSearchInv.Visible AndAlso dgvSearchInv.Rows.Count > 0 Then
            If e.KeyCode = Keys.Down Then
                e.Handled = True
                dgvSearchInv.Focus()
                If dgvSearchInv.CurrentRow Is Nothing Then
                    dgvSearchInv.CurrentCell = dgvSearchInv.Rows(0).Cells(0)
                End If
            ElseIf e.KeyCode = Keys.Enter Then
                e.Handled = True
                e.SuppressKeyPress = True
                If dgvSearchInv.CurrentRow IsNot Nothing Then
                    SelectInvoiceFromGrid(dgvSearchInv.CurrentRow.Index)
                End If
            End If
        End If
    End Sub

    Private Sub dgvSearchInv_KeyDown(sender As Object, e As KeyEventArgs) Handles dgvSearchInv.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.Handled = True
            e.SuppressKeyPress = True
            If dgvSearchInv.CurrentRow IsNot Nothing Then
                SelectInvoiceFromGrid(dgvSearchInv.CurrentRow.Index)
            End If
        End If
    End Sub

    Private Sub txtSearchInv_Leave(sender As Object, e As EventArgs) Handles txtSearchInv.Leave
        Me.BeginInvoke(New Action(Sub()
                                      If Not dgvSearchInv.Focused Then
                                          dgvSearchInv.Visible = False
                                      End If
                                  End Sub))
    End Sub

    Private Sub dgvSearchInv_Leave(sender As Object, e As EventArgs) Handles dgvSearchInv.Leave
        Me.BeginInvoke(New Action(Sub()
                                      If Not txtSearchInv.Focused Then
                                          dgvSearchInv.Visible = False
                                      End If
                                  End Sub))
    End Sub


    Public Sub ApplySecurityLock()
        UpdateSearchUI()
        LoadReportData()
    End Sub

    Private Sub UpdateSearchUI()
        Dim idx As Integer = cmbReportType.SelectedIndex
        
        ' Hide date pickers if Invoice or Customer search is active
        ' Exception: specific reports that use dates heavily.
        Dim showDateIdx As Integer() = {0, 1, 2, 3, 5, 6, 8, 9, 10, 20, 21, 22, 23, 24, 25, 26}
        Dim hideDates As Boolean = Not showDateIdx.Contains(idx)
        
        If Not Module1.IsRgrVisible Then
            ' Force hide dates and checkbox
            lblFrom.Visible = False
            dtpFrom.Visible = False
            lblTo.Visible = False
            dtpTo.Visible = False
            chkAllDates.Visible = False
            lblReportType.Left = 10
            
            ' Ensure all dates is checked internally so no date filter is applied to queries
            If Not chkAllDates.Checked Then
                _isInitializing = True
                chkAllDates.Checked = True
                _isInitializing = False
            End If
        Else
            lblFrom.Visible = Not hideDates AndAlso Not chkAllDates.Checked
            dtpFrom.Visible = Not hideDates AndAlso Not chkAllDates.Checked
            lblTo.Visible = Not hideDates AndAlso Not chkAllDates.Checked
            dtpTo.Visible = Not hideDates AndAlso Not chkAllDates.Checked
            
            ' Base X position depends on whether dates are hidden
            If Not hideDates Then
                ' Dates are visible, position Type dropdown after chkAllDates
                chkAllDates.Visible = True
                chkAllDates.BringToFront()
                chkAllDates.Left = dtpTo.Right + 10
                lblReportType.Left = chkAllDates.Right + 10
            Else
                ' Dates are hidden, move Type dropdown to the left
                chkAllDates.Visible = False
                lblReportType.Left = 10
            End If
        End If
        cmbReportType.Left = lblReportType.Right + 5
        
        ' 1. Customer/Supplier Name search control visibility & label
        If idx <= 2 OrElse idx = 10 OrElse idx = 22 Then
            lblSearchName.Visible = False
            txtSearchName.Visible = False
            dgvSearchName.Visible = False
        Else
            lblSearchName.Visible = True
            txtSearchName.Visible = True

            If idx = 3 Then
                lblSearchName.Text = "Search Desc:"
            ElseIf idx = 26 Then
                lblSearchName.Text = "Search City:"
            ElseIf (idx >= 4 AndAlso idx <= 6) OrElse idx = 20 OrElse idx = 21 OrElse idx = 25 OrElse (idx >= 11 AndAlso idx <= 13) OrElse idx = 15 OrElse idx = 16 Then
                lblSearchName.Text = "Search Customer:"
            ElseIf (idx >= 7 AndAlso idx <= 9) OrElse idx = 17 OrElse idx = 18 OrElse idx = 14 OrElse idx = 19 OrElse idx = 23 OrElse idx = 24 Then
                lblSearchName.Text = "Search Supplier:"
            Else
                lblSearchName.Text = "Search Name:"
            End If

            lblSearchName.BringToFront()
            txtSearchName.BringToFront()
            lblSearchName.Left = cmbReportType.Left + cmbReportType.Width + 20
            
            If idx = 3 Then
                txtSearchName.Left = lblSearchName.Left + 80
            Else
                txtSearchName.Left = lblSearchName.Left + 115
            End If
            dgvSearchName.Left = txtSearchName.Left
        End If

        Dim showNameSearch As Boolean = Not (idx <= 2 OrElse idx = 10 OrElse idx = 22)
        
        ' 2. Invoice/Purchase ID search control visibility & label
        If (idx >= 11 AndAlso idx <= 13) OrElse idx = 15 OrElse idx = 16 OrElse idx = 22 Then
            lblSearchInv.Visible = True
            txtSearchInv.Visible = True
            lblSearchInv.Text = "Search Invoice:"
            lblSearchInv.BringToFront()
            txtSearchInv.BringToFront()
            
            ' Ensure it is positioned correctly without relying on Right property which might be 0 during hidden state
            If showNameSearch Then
                lblSearchInv.Left = txtSearchName.Left + txtSearchName.Width + 20
            Else
                lblSearchInv.Left = cmbReportType.Left + cmbReportType.Width + 20
            End If
            txtSearchInv.Left = lblSearchInv.Left + 100 ' Fixed offset for label width
            dgvSearchInv.Left = txtSearchInv.Left

        ElseIf idx = 14 OrElse idx = 18 Then
            lblSearchInv.Visible = True
            txtSearchInv.Visible = True
            lblSearchInv.Text = If(idx = 14, "Search Purchase No:", "Search Return No:")
            lblSearchInv.BringToFront()
            txtSearchInv.BringToFront()
            
            If showNameSearch Then
                lblSearchInv.Left = txtSearchName.Left + txtSearchName.Width + 20
            Else
                lblSearchInv.Left = cmbReportType.Left + cmbReportType.Width + 20
            End If
            txtSearchInv.Left = lblSearchInv.Left + 130 ' Wider label
            dgvSearchInv.Left = txtSearchInv.Left
        ElseIf idx = 19 Then
            lblSearchInv.Visible = True
            txtSearchInv.Visible = True
            lblSearchInv.Text = "Search Request ID:"
            lblSearchInv.BringToFront()
            txtSearchInv.BringToFront()
            
            If showNameSearch Then
                lblSearchInv.Left = txtSearchName.Left + txtSearchName.Width + 20
            Else
                lblSearchInv.Left = cmbReportType.Left + cmbReportType.Width + 20
            End If
            txtSearchInv.Left = lblSearchInv.Left + 115 ' Wider label
            dgvSearchInv.Left = txtSearchInv.Left
        Else
            lblSearchInv.Visible = False
            txtSearchInv.Visible = False
            dgvSearchInv.Visible = False
        End If
    End Sub

    Private Sub LoadSearchData()
        ' Grids are now loaded dynamically on TextChanged, so this is no longer needed.
        ' We just clear the text boxes and hide the grids when switching reports.
        txtSearchName.Text = ""
        txtSearchInv.Text = ""
        dgvSearchName.Visible = False
        dgvSearchInv.Visible = False
    End Sub

    Private Sub PopulateReportTypes()
        If cmbReportType.Items.Count = 0 Then
            cmbReportType.Items.Add("Daily Sales Detailed")
            cmbReportType.Items.Add("Monthly Sales Summary")
            cmbReportType.Items.Add("Monthly Item Sales")
            cmbReportType.Items.Add("Stock Report")
            cmbReportType.Items.Add("Customer List")
            cmbReportType.Items.Add("Customer Credit")
            cmbReportType.Items.Add("Customer Cheque")
            cmbReportType.Items.Add("Supplier List")
            cmbReportType.Items.Add("Supplier Debit")
            cmbReportType.Items.Add("Supplier Cheque")
            cmbReportType.Items.Add("Stock Return")
            cmbReportType.Items.Add("Sale Invoice (A4)")
            cmbReportType.Items.Add("Sale Invoice (POS)")
            cmbReportType.Items.Add("Sale Return Invoice")
            cmbReportType.Items.Add("Purchase Invoice")
            cmbReportType.Items.Add("Quotation")
            cmbReportType.Items.Add("Quotation (POS)")
            cmbReportType.Items.Add("Purchase History")
            cmbReportType.Items.Add("Purchase Return History")
            cmbReportType.Items.Add("Purchase Request")
            cmbReportType.Items.Add("Customer Payment Note")
            cmbReportType.Items.Add("Full Credit Report")
            cmbReportType.Items.Add("Bill Details Report")
            cmbReportType.Items.Add("Supplier Payment Report")
            cmbReportType.Items.Add("Full Debit Report")
            cmbReportType.Items.Add("Full Cheque Report")
            cmbReportType.Items.Add("Customer Credit by City")
        End If
    End Sub

    Private Sub SalesHistoryForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If _currentRpt IsNot Nothing Then
            _currentRpt.Close()
            _currentRpt.Dispose()
        End If
    End Sub

    Private Sub SalesHistoryForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.KeyPreview = True

        If Not _isInitializedExternally Then
            dtpFrom.Value = DateTime.Now.Date
            dtpTo.Value = DateTime.Now.Date
            PopulateReportTypes()
            cmbReportType.SelectedIndex = 0
            _allChecked = True ' Default to All for manual loads
        End If
        InitializePrinters()

        ' Disable elements that often trigger 'No error' popups
        CrystalReportViewer1.ShowLogo = False
        CrystalReportViewer1.DisplayStatusBar = False
        CrystalReportViewer1.ShowGroupTreeButton = False
        CrystalReportViewer1.ShowParameterPanelButton = False
        CrystalReportViewer1.EnableDrillDown = False
        CrystalReportViewer1.DisplayToolbar = True
        CrystalReportViewer1.ReuseParameterValuesOnRefresh = True

        ' Auto refresh on load (skip if already loaded by SetReportContext)
        If Not _isInitializedExternally Then
            LoadReportData()
        End If
    End Sub

    Private Sub InitializePrinters()
        Try
            cmbPrinter.Items.Clear()
            For Each printer As String In System.Drawing.Printing.PrinterSettings.InstalledPrinters
                cmbPrinter.Items.Add(printer)
            Next

            ' Select default printer
            Dim printDoc As New System.Drawing.Printing.PrintDocument()
            Dim defaultPrinter As String = printDoc.PrinterSettings.PrinterName
            If cmbPrinter.Items.Contains(defaultPrinter) Then
                cmbPrinter.SelectedItem = defaultPrinter
            ElseIf cmbPrinter.Items.Count > 0 Then
                cmbPrinter.SelectedIndex = 0
            End If
        Catch ex As Exception
            ' Silent fail for printer initialization
        End Try
    End Sub

    Private Sub btnShow_Click(sender As Object, e As EventArgs) Handles btnShow.Click
        _searchInv = ""
        LoadReportData()
    End Sub

    Private Sub SyncMissingManualCredits()
        Try
            Using localConn As New MySqlConnection(Module1.ConnStr)
                localConn.Open()
                
                Dim sql As String = "SELECT cc.customer_id, cc.inv_no, cc.amount, cc.timestamps " &
                                    "FROM customer_credit cc " &
                                    "LEFT JOIN billing b ON cc.inv_no = b.inv_no AND cc.customer_id = b.customer_id " &
                                    "WHERE b.inv_no IS NULL AND cc.is_active = 1 AND cc.inv_no IS NOT NULL AND cc.inv_no <> ''"
                
                Dim dt As New DataTable()
                Using cmd As New MySqlCommand(sql, localConn)
                    Using da As New MySqlDataAdapter(cmd)
                        da.Fill(dt)
                    End Using
                End Using

                If dt.Rows.Count > 0 Then
                    Dim transaction = localConn.BeginTransaction()
                    Try
                        For Each row As DataRow In dt.Rows
                            Dim inv As String = row("inv_no").ToString()
                            Dim cid As Integer = Convert.ToInt32(row("customer_id"))
                            Dim amt As Double = Convert.ToDouble(row("amount"))
                            Dim ts As DateTime = Convert.ToDateTime(row("timestamps"))
                            
                            Dim insertDummySql As String = "INSERT INTO billing (inv_no, printed_inv_no, customer_id, subtotal, grand_total, credit_balance_due, balance_due, status, timestamps, inv_type, billing_type, payment_type, user_id, order_user_id, collector_user_id, po_number, vat_id, bank_id) " &
                                                           "VALUES (@inv, @inv, @cid, @amount, @amount, @amount, @amount, 'Credit', @date, 'Manual Credit', 'Credit', 'Credit', @uid, @uid, @uid, '', 1, 1)"
                            
                            Using cmdIns As New MySqlCommand(insertDummySql, localConn, transaction)
                                cmdIns.Parameters.AddWithValue("@inv", inv)
                                cmdIns.Parameters.AddWithValue("@cid", cid)
                                cmdIns.Parameters.AddWithValue("@amount", amt)
                                cmdIns.Parameters.AddWithValue("@date", ts)
                                
                                Dim uId As Integer = If(Module1.CurrentUserID > 0, Module1.CurrentUserID, 101)
                                cmdIns.Parameters.AddWithValue("@uid", uId)
                                
                                cmdIns.ExecuteNonQuery()
                            End Using
                        Next
                        transaction.Commit()
                    Catch ex As Exception
                        transaction.Rollback()
                    End Try
                End If
            End Using
        Catch ex As Exception
        End Try
    End Sub

    Private Sub LoadReportData()
        If _isRefreshing Then Return
        _isRefreshing = True
        Try
            ' Ensure legacy manual credit records have dummy billing entries to satisfy report INNER JOINs
            If cmbReportType.SelectedIndex = 5 OrElse cmbReportType.SelectedIndex = 21 OrElse cmbReportType.SelectedIndex = 26 Then
                SyncMissingManualCredits()
            End If

            Dim startDate As DateTime = dtpFrom.Value.Date
            Dim endDate As DateTime = dtpTo.Value.Date

            ' If the system is in hidden mode, daily sales (index 0) and bill details (index 22) must show only today's data.
            If Not Module1.IsRgrVisible Then
                If cmbReportType.SelectedIndex = 0 OrElse cmbReportType.SelectedIndex = 22 Then
                    startDate = DateTime.Today
                    endDate = DateTime.Today
                End If
            End If

            ' Determine Report File based on selection
            Dim reportFile As String = ""
            Select Case cmbReportType.SelectedIndex
                Case 0 : reportFile = "DailySales.rpt"
                Case 1 : reportFile = "MonthlySalesSummary.rpt"
                Case 2 : reportFile = "MonthlyItemSales.rpt"
                Case 3 : reportFile = "Stock.rpt"
                Case 4 : reportFile = "Customer.rpt"
                Case 5 : reportFile = "CUSTOMER_CREDIT.rpt"
                Case 6 : reportFile = "Customercheque.rpt"
                Case 7 : reportFile = "Supplier.rpt"
                Case 8 : reportFile = "supplierdebit.rpt"
                Case 9 : reportFile = "suppliercheque.rpt"
                Case 10 : reportFile = "StockReturn.rpt"
                Case 11 : reportFile = "SeleInvoice.rpt"
                Case 12 : reportFile = "SaleInvoicePOS.rpt"
                Case 13 : reportFile = "SaleReturnInv.rpt"
                Case 14 : reportFile = "PuchaInvoice.rpt"
                Case 15 : reportFile = "Quate.rpt"
                Case 16 : reportFile = "QuatePOS.rpt"
                Case 17 : reportFile = "PuchaInvoice.rpt"
                Case 18 : reportFile = "purchasereturn.rpt"
                Case 19 : reportFile = "puchaseInvoiceRequest.rpt"
                Case 20 : reportFile = "customerpaymentnote.rpt"
                Case 21 : reportFile = "fullcreditreport.rpt"
                Case 22 : reportFile = "billdetails.rpt"
                Case 23 : reportFile = "supplierpayment.rpt"
                Case 24 : reportFile = "fulldebitreport.rpt"
                Case 25 : reportFile = "fullcheque.rpt"
                Case 26 : reportFile = "CUSTOMER_CREDIT_BY_CITY.rpt"
            End Select

            Dim _tmpSearchVal As String = If(Not String.IsNullOrEmpty(txtSearchInv.Text), txtSearchInv.Text.Trim(), _searchInv)
            If (cmbReportType.SelectedIndex = 11 OrElse cmbReportType.SelectedIndex = 12 OrElse cmbReportType.SelectedIndex = 22) AndAlso Not String.IsNullOrEmpty(_tmpSearchVal) AndAlso _tmpSearchVal.StartsWith("QT") Then
                reportFile = "Quate.rpt"
            End If

            Dim rptObj As ReportDocument = Nothing
            
            ' Try strongly-typed classes for known embedded reports to bypass disk-loading issues in ClickOnce
            If reportFile = "customerpaymentnote.rpt" Then rptObj = New customerpaymentnote()
            If reportFile = "fullcreditreport.rpt" Then rptObj = New fullcreditreport()
            If reportFile = "billdetails.rpt" Then rptObj = New billdetails()
            If reportFile = "supplierpayment.rpt" Then rptObj = New supplierpayment()
            If reportFile = "fulldebitreport.rpt" Then rptObj = New fulldebitreport()
            If reportFile = "fullcheque.rpt" Then rptObj = New fullcheque()
            If reportFile = "CashRpeort.rpt" Then rptObj = New CashRpeort()
            If reportFile = "CreditReport.rpt" Then rptObj = New CreditReport()
            If reportFile = "ELReport.rpt" Then rptObj = New ELReport()
            If reportFile = "GRReport.rpt" Then rptObj = New GRReport()
            If reportFile = "VATReport.rpt" Then rptObj = New VATReport()
            If reportFile = "QTReport.rpt" Then rptObj = New QTReport()
            If reportFile = "FullDayAll.rpt" Then rptObj = New FullDayAll()
            If reportFile = "RGRReport.rpt" Then rptObj = New RGRReport()
            If reportFile = "DailySales.rpt" Then rptObj = New DailySales()
            If reportFile = "MonthlySalesSummary.rpt" Then rptObj = New MonthlySalesSummary()
            If reportFile = "MonthlyItemSales.rpt" Then rptObj = New MonthlyItemSales()
            If reportFile = "Quate.rpt" Then rptObj = New Quate()
            If reportFile = "QuatePOS.rpt" Then rptObj = New QuatePOS()
            If reportFile = "SaleInvoicePOS.rpt" Then rptObj = New SaleInvoicePOS()
            If reportFile = "SeleInvoice.rpt" Then rptObj = New SeleInvoice()
            If reportFile = "PuchaInvoice.rpt" Then rptObj = New PuchaInvoice()
            If reportFile = "puchaseInvoiceRequest.rpt" Then rptObj = New puchaseInvoiceRequest()
            If reportFile = "SaleReturnInv.rpt" Then rptObj = New SaleReturnInv()
            If reportFile = "purchasereturn.rpt" Then rptObj = New purchasereturn()
            If reportFile = "Stock.rpt" Then rptObj = New Stock()
            If reportFile = "StockReturn.rpt" Then rptObj = New StockReturn()
            If reportFile = "Customer.rpt" Then rptObj = New Customer()
            If reportFile = "CUSTOMER_CREDIT.rpt" Then rptObj = New CUSTOMER_CREDIT()
            If reportFile = "CUSTOMER_CREDIT_BY_CITY.rpt" Then rptObj = New CUSTOMER_CREDIT_BY_CITY()
            If reportFile = "Customercheque.rpt" Then rptObj = New Customercheque()
            If reportFile = "Supplier.rpt" Then rptObj = New Supplier()
            If reportFile = "supplierdebit.rpt" Then rptObj = New supplierdebit()
            If reportFile = "suppliercheque.rpt" Then rptObj = New suppliercheque()
            If reportFile = "QuatePOS.rpt" Then rptObj = New QuatePOS()
            If reportFile = "puchaseInvoiceRequest.rpt" Then rptObj = New puchaseInvoiceRequest()

            If rptObj IsNot Nothing Then
                If _currentRpt IsNot Nothing Then
                    _currentRpt.Close()
                    _currentRpt.Dispose()
                End If
                _currentRpt = rptObj
                _lastLoadedPath = "EMBEDDED:" & reportFile
                SetReportConnection(_currentRpt)
                If reportFile = "CUSTOMER_CREDIT_BY_CITY.rpt" Then
                    Try
                        _currentRpt.VerifyDatabase()
                    Catch : End Try
                    
                    Try
                        ' Suppress name/label in Section 1 (Report Header)
                        _currentRpt.ReportDefinition.ReportObjects("name1").ObjectFormat.EnableSuppress = True
                        _currentRpt.ReportDefinition.ReportObjects("Text1").ObjectFormat.EnableSuppress = True

                        ' No column (Left=120, Width=400)
                        _currentRpt.ReportDefinition.ReportObjects("RecordNumber1").Left = 120
                        _currentRpt.ReportDefinition.ReportObjects("RecordNumber1").Width = 400
                        _currentRpt.ReportDefinition.ReportObjects("Text3").Left = 120
                        _currentRpt.ReportDefinition.ReportObjects("Text3").Width = 400

                        ' Customer Name column (Left=600, Width=2800)
                        _currentRpt.ReportDefinition.ReportObjects("ponumber1").Left = 600
                        _currentRpt.ReportDefinition.ReportObjects("ponumber1").Width = 2800
                        Dim t4Obj As TextObject = DirectCast(_currentRpt.ReportDefinition.ReportObjects("Text4"), TextObject)
                        t4Obj.Text = "Customer Name"
                        t4Obj.Left = 600
                        t4Obj.Width = 2800

                        ' Invoice No column (Left=3500, Width=1200)
                        _currentRpt.ReportDefinition.ReportObjects("invno1").Left = 3500
                        _currentRpt.ReportDefinition.ReportObjects("invno1").Width = 1200
                        _currentRpt.ReportDefinition.ReportObjects("Text2").Left = 3500
                        _currentRpt.ReportDefinition.ReportObjects("Text2").Width = 1200

                        ' Credit Amount column (Left=4800, Width=1600)
                        _currentRpt.ReportDefinition.ReportObjects("amount1").Left = 4800
                        _currentRpt.ReportDefinition.ReportObjects("amount1").Width = 1600
                        _currentRpt.ReportDefinition.ReportObjects("Text7").Left = 4800
                        _currentRpt.ReportDefinition.ReportObjects("Text7").Width = 1600

                        ' Bill Date column (Left=6500, Width=2300)
                        _currentRpt.ReportDefinition.ReportObjects("timestamps1").Left = 6500
                        _currentRpt.ReportDefinition.ReportObjects("timestamps1").Width = 2300
                        _currentRpt.ReportDefinition.ReportObjects("Text6").Left = 6500
                        _currentRpt.ReportDefinition.ReportObjects("Text6").Width = 2300

                        ' Days Pending column (Left=8900, Width=1300)
                        _currentRpt.ReportDefinition.ReportObjects("DaysPending1").Left = 8900
                        _currentRpt.ReportDefinition.ReportObjects("DaysPending1").Width = 1300
                        _currentRpt.ReportDefinition.ReportObjects("Text9").Left = 8900
                        _currentRpt.ReportDefinition.ReportObjects("Text9").Width = 1300

                        ' Footer total field alignment (Left=4800, Width=1600)
                        _currentRpt.ReportDefinition.ReportObjects("Sumofamount1").Left = 4800
                        _currentRpt.ReportDefinition.ReportObjects("Sumofamount1").Width = 1600
                        _currentRpt.ReportDefinition.ReportObjects("Text8").Left = 4000
                        _currentRpt.ReportDefinition.ReportObjects("Text8").Width = 750
                    Catch : End Try
                End If
            Else
                MessageBox.Show("Report '" & reportFile & "' not recognized.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Dim rpt As ReportDocument = _currentRpt

            ' Fix known subreport bugs in Crystal Reports design files dynamically
            Try
                For Each subRpt As ReportDocument In rpt.Subreports
                    If subRpt.Name.Equals("fullcreditre", StringComparison.OrdinalIgnoreCase) Then
                        ' The .rpt file mistakenly links CusID to the amount field instead of customer_id
                        subRpt.RecordSelectionFormula = "{customer_credit1.customer_id} = {?Pm-customer_payments1.CusID}"
                    End If
                Next
            Catch ex As Exception
            End Try

            ' Generate Formula
            Dim formula As String = ""
            Dim _searchInvVal As String = If(Not String.IsNullOrEmpty(txtSearchInv.Text), txtSearchInv.Text.Trim(), _searchInv)

            ' List of success/paid statuses (excluding pending, unsuccess, and returned)
            Dim statusList As String = "[""paid"", ""success"", ""completed"", ""advance"", ""credit"", ""partial_credit"", ""cheque"", ""partial_cheque"", ""cash_credit"", ""cash_cheque"", ""mixed_payment"", ""credit_cheque""]"

            If cmbReportType.SelectedIndex = 0 Then ' Daily Sales (Detailed)
                ' Set Parameter if it exists (Daily Sales often has pDate for the header)
                Try
                    rpt.SetParameterValue("pDate", startDate.ToString("yyyy-MM-dd"))
                Catch : End Try

                Dim tsField As String = GetReportField(rpt, "timestamps")
                If String.IsNullOrEmpty(tsField) Then tsField = GetReportField(rpt, "sale_time")
                If String.IsNullOrEmpty(tsField) Then tsField = GetReportField(rpt, "date")
                If String.IsNullOrEmpty(tsField) Then tsField = "{billing1.timestamps}"

                Dim stField As String = GetReportField(rpt, "status")
                If String.IsNullOrEmpty(stField) Then stField = "{billing1.status}"

                If Not String.IsNullOrEmpty(_searchInvVal) Then
                    Dim invField As String = GetReportField(rpt, "inv_no")
                    If String.IsNullOrEmpty(invField) Then invField = "{billing1.inv_no}"
                    formula = invField & " = """ & _searchInvVal.Replace("""", """""") & """ AND LCase(Trim(" & stField & ")) <> ""pending"""
                Else
                    If chkAllDates.Checked AndAlso Module1.IsRgrVisible Then
                        formula = "LCase(Trim(" & stField & ")) <> ""pending"" AND LCase(Trim(" & stField & ")) IN " & statusList
                    Else
                        formula = tsField & " >= DateTime(" & startDate.Year & ", " & startDate.Month & ", " & startDate.Day & ", 0, 0, 0) AND " &
                                  tsField & " <= DateTime(" & endDate.Year & ", " & endDate.Month & ", " & endDate.Day & ", 23, 59, 59) AND " &
                                  "LCase(Trim(" & stField & ")) <> ""pending"" AND LCase(Trim(" & stField & ")) IN " & statusList
                    End If

                    If Not _allChecked Then
                        Dim itField As String = GetReportField(rpt, "inv_type")
                        If Not String.IsNullOrEmpty(itField) Then formula &= " AND " & itField & " = """ & _filterInvType & """"

                        If _isAdvance Then
                            Dim advField As String = GetReportField(rpt, "advance_payment")
                            If Not String.IsNullOrEmpty(advField) Then formula &= " AND " & advField & " > 0"
                        Else
                            Select Case _filterBilling
                                Case "Cash"
                                    formula &= " AND LCase(Trim(" & stField & ")) = ""paid"""
                                Case "Cash (Cash)"
                                    Dim pmField As String = GetReportField(rpt, "payment_type")
                                    If String.IsNullOrEmpty(pmField) Then pmField = "{billing1.payment_type}"
                                    formula &= " AND LCase(Trim(" & stField & ")) = ""paid"" AND LCase(Trim(" & pmField & ")) = ""cash"""
                                Case "Cash (Cards/Online)"
                                    Dim pmField As String = GetReportField(rpt, "payment_type")
                                    If String.IsNullOrEmpty(pmField) Then pmField = "{billing1.payment_type}"
                                    formula &= " AND LCase(Trim(" & stField & ")) = ""paid"" AND LCase(Trim(" & pmField & ")) IN [""credit card"", ""debit card"", ""online transfer""]"
                                Case "Credit"
                                    formula &= " AND LCase(Trim(" & stField & ")) IN [""credit"", ""partial_credit""]"
                                Case "Cheque"
                                    formula &= " AND LCase(Trim(" & stField & ")) IN [""cheque"", ""partial_cheque""]"
                                Case "Cash+Credit"
                                    formula &= " AND LCase(Trim(" & stField & ")) = ""cash_credit"""
                                Case "Cash+Cheque"
                                    formula &= " AND LCase(Trim(" & stField & ")) = ""cash_cheque"""
                                Case "Mixed Payment"
                                    formula &= " AND LCase(Trim(" & stField & ")) = ""mixed_payment"""
                                Case "Credit+Cheque"
                                    formula &= " AND LCase(Trim(" & stField & ")) = ""credit_cheque"""
                                Case Else
                                    Dim btField As String = GetReportField(rpt, "billing_type")
                                    If Not String.IsNullOrEmpty(btField) Then formula &= " AND " & btField & " = """ & _filterBilling & """"
                            End Select
                        End If
                    End If
                End If
             ElseIf cmbReportType.SelectedIndex = 1 Then ' Monthly Summary
                Try
                    Dim ds As New DataSet()
                    Using conn As New MySqlConnection(Module1.ConnStr)
                        conn.Open()
                        
                        Dim baseWhere As String = "WHERE LOWER(TRIM(b.status)) IN ('paid', 'success', 'completed', 'advance', 'credit', 'partial_credit', 'cheque', 'partial_cheque', 'cash_credit', 'cash_cheque', 'mixed_payment', 'credit_cheque') " & 
                                                  (If(Module1.IsRgrVisible, "", " AND b.is_rgr = 0 AND b.inv_no NOT LIKE 'GR%' AND b.inv_no NOT LIKE 'RGR%' "))
                        
                        If Not String.IsNullOrEmpty(_searchInvVal) Then
                            baseWhere &= " AND b.inv_no = @invNo "
                        Else
                            If Not chkAllDates.Checked Then
                                baseWhere &= " AND b.timestamps >= @start AND b.timestamps <= @end "
                            End If
                            If Not _allChecked Then
                                baseWhere &= " AND b.inv_type = @invType "
                                If _isAdvance Then
                                    baseWhere &= " AND b.advance_payment != 0 "
                                Else
                                    Select Case _filterBilling
                                        Case "Cash" : baseWhere &= " AND LOWER(TRIM(b.status)) = 'paid' "
                                        Case "Cash (Cash)" : baseWhere &= " AND LOWER(TRIM(b.status)) = 'paid' AND LOWER(TRIM(b.payment_type)) = 'cash' "
                                        Case "Cash (Cards/Online)" : baseWhere &= " AND LOWER(TRIM(b.status)) = 'paid' AND LOWER(TRIM(b.payment_type)) IN ('credit card', 'debit card', 'online transfer') "
                                        Case "Credit" : baseWhere &= " AND LOWER(TRIM(b.status)) IN ('credit', 'partial_credit') "
                                        Case "Cheque" : baseWhere &= " AND LOWER(TRIM(b.status)) IN ('cheque', 'partial_cheque') "
                                        Case "Cash+Credit" : baseWhere &= " AND LOWER(TRIM(b.status)) = 'cash_credit' "
                                        Case "Cash+Cheque" : baseWhere &= " AND LOWER(TRIM(b.status)) = 'cash_cheque' "
                                        Case "Mixed Payment" : baseWhere &= " AND LOWER(TRIM(b.status)) = 'mixed_payment' "
                                        Case "Credit+Cheque" : baseWhere &= " AND LOWER(TRIM(b.status)) = 'credit_cheque' "
                                        Case Else : baseWhere &= " AND b.billing_type = @bType "
                                    End Select
                                End If
                            End If
                        End If
                        
                        ' Command (Detailed data)
                        Dim sqlCmd1 As String = "SELECT DATE(b.timestamps) as 'ReportDate', COUNT(DISTINCT b.id) as 'InvoiceCount', " &
                                                "SUM(b.subtotal) as 'TotalSales', SUM(IFNULL(bi_sum.cost, 0)) as 'TotalCost', " &
                                                "SUM(b.subtotal - IFNULL(bi_sum.cost, 0)) as 'TotalProfit', " &
                                                "SUM(b.paid_amount) as 'TotalPaid', SUM(b.balance_due) as 'TotalBalance' " &
                                                "FROM billing b " &
                                                "LEFT JOIN (SELECT billing_id, SUM(item_cost * quantity) as cost FROM billing_item GROUP BY billing_id) bi_sum ON b.id = bi_sum.billing_id " &
                                                baseWhere & " GROUP BY DATE(b.timestamps)"
                                                
                        Using cmd As New MySqlCommand(sqlCmd1, conn)
                            If Not String.IsNullOrEmpty(_searchInvVal) Then
                                cmd.Parameters.AddWithValue("@invNo", _searchInvVal)
                            Else
                                cmd.Parameters.AddWithValue("@start", startDate.ToString("yyyy-MM-dd") & " 00:00:00")
                                cmd.Parameters.AddWithValue("@end", endDate.ToString("yyyy-MM-dd") & " 23:59:59")
                                cmd.Parameters.AddWithValue("@invType", _filterInvType)
                                cmd.Parameters.AddWithValue("@bType", _filterBilling)
                            End If
                            Using da As New MySqlDataAdapter(cmd)
                                Dim dt As New DataTable("Command")
                                da.Fill(dt)
                                ds.Tables.Add(dt)
                            End Using
                        End Using
                        
                        ' Command_1 (Chart data)
                        Dim sqlCmd2 As String = "SELECT DATE(b.timestamps) as 'ChartDate', SUM(b.subtotal) as 'DailySales' " &
                                                "FROM billing b " & baseWhere & " GROUP BY DATE(b.timestamps)"
                        
                        Using cmd As New MySqlCommand(sqlCmd2, conn)
                            If Not String.IsNullOrEmpty(_searchInvVal) Then
                                cmd.Parameters.AddWithValue("@invNo", _searchInvVal)
                            Else
                                cmd.Parameters.AddWithValue("@start", startDate.ToString("yyyy-MM-dd") & " 00:00:00")
                                cmd.Parameters.AddWithValue("@end", endDate.ToString("yyyy-MM-dd") & " 23:59:59")
                                cmd.Parameters.AddWithValue("@invType", _filterInvType)
                                cmd.Parameters.AddWithValue("@bType", _filterBilling)
                            End If
                            Using da As New MySqlDataAdapter(cmd)
                                Dim dt As New DataTable("Command_1")
                                da.Fill(dt)
                                ds.Tables.Add(dt)
                            End Using
                        End Using
                    End Using
                    
                    rpt.SetDataSource(ds)
                Catch ex As Exception
                End Try
                formula = "{Command.ReportDate} = {Command_1.ChartDate}"
            ElseIf cmbReportType.SelectedIndex = 2 Then ' Monthly Item Sales
                Try
                    Dim ds As New DataSet()
                    Using conn As New MySqlConnection(Module1.ConnStr)
                        conn.Open()
                        
                        Dim baseWhere As String = "WHERE LOWER(TRIM(b.status)) IN ('paid', 'success', 'completed', 'advance', 'credit', 'partial_credit', 'cheque', 'partial_cheque', 'cash_credit', 'cash_cheque', 'mixed_payment', 'credit_cheque') " & 
                                                  (If(Module1.IsRgrVisible, "", " AND b.is_rgr = 0 AND b.inv_no NOT LIKE 'GR%' AND b.inv_no NOT LIKE 'RGR%' "))
                        
                        If Not String.IsNullOrEmpty(_searchInvVal) Then
                            baseWhere &= " AND b.inv_no = @invNo "
                        Else
                            If Not chkAllDates.Checked Then
                                baseWhere &= " AND b.timestamps >= @start AND b.timestamps <= @end "
                            End If
                            If Not _allChecked Then
                                baseWhere &= " AND b.inv_type = @invType "
                                If _isAdvance Then
                                    baseWhere &= " AND b.advance_payment != 0 "
                                Else
                                    Select Case _filterBilling
                                        Case "Cash" : baseWhere &= " AND LOWER(TRIM(b.status)) = 'paid' "
                                        Case "Cash (Cash)" : baseWhere &= " AND LOWER(TRIM(b.status)) = 'paid' AND LOWER(TRIM(b.payment_type)) = 'cash' "
                                        Case "Cash (Cards/Online)" : baseWhere &= " AND LOWER(TRIM(b.status)) = 'paid' AND LOWER(TRIM(b.payment_type)) IN ('credit card', 'debit card', 'online transfer') "
                                        Case "Credit" : baseWhere &= " AND LOWER(TRIM(b.status)) IN ('credit', 'partial_credit') "
                                        Case "Cheque" : baseWhere &= " AND LOWER(TRIM(b.status)) IN ('cheque', 'partial_cheque') "
                                        Case "Cash+Credit" : baseWhere &= " AND LOWER(TRIM(b.status)) = 'cash_credit' "
                                        Case "Cash+Cheque" : baseWhere &= " AND LOWER(TRIM(b.status)) = 'cash_cheque' "
                                        Case "Mixed Payment" : baseWhere &= " AND LOWER(TRIM(b.status)) = 'mixed_payment' "
                                        Case "Credit+Cheque" : baseWhere &= " AND LOWER(TRIM(b.status)) = 'credit_cheque' "
                                        Case Else : baseWhere &= " AND b.billing_type = @bType "
                                    End Select
                                End If
                            End If
                        End If
                        If Not String.IsNullOrEmpty(_fItemName) Then
                            baseWhere &= " AND i.item_id LIKE @itemCode "
                        End If
                        If Not String.IsNullOrEmpty(_fDes) Then
                            baseWhere &= " AND i.description LIKE @itemDesc "
                        End If
                        
                        Dim sqlCmd As String = "SELECT i.item_id as 'item_id', i.description as 'description', SUM(i.quantity) as 'total_qty', " &
                                               "AVG(i.item_cost) as 'avg_price', " &
                                               "SUM((i.unit_price - (i.unit_price * i.discount / 100)) * i.quantity) as 'total_sales', " &
                                               "SUM(i.item_cost * i.quantity) as 'total_cost', " &
                                               "SUM(((i.unit_price - (i.unit_price * i.discount / 100)) - i.item_cost) * i.quantity) as 'total_profit', " &
                                               "MAX(b.timestamps) as 'timestamps' " &
                                               "FROM billing_item i " &
                                               "JOIN billing b ON i.billing_id = b.id " &
                                               baseWhere & " GROUP BY i.item_id, i.description "
                        If String.IsNullOrEmpty(_searchInvVal) AndAlso (Not String.IsNullOrEmpty(_fItemName) OrElse Not String.IsNullOrEmpty(_fDes)) Then
                            sqlCmd &= "ORDER BY TRIM(i.description) ASC"
                        Else
                            sqlCmd &= "ORDER BY SUM(i.quantity) DESC"
                        End If
                                                
                        Using cmd As New MySqlCommand(sqlCmd, conn)
                            If Not String.IsNullOrEmpty(_fItemName) Then
                                cmd.Parameters.AddWithValue("@itemCode", "%" & _fItemName & "%")
                            End If
                            If Not String.IsNullOrEmpty(_fDes) Then
                                cmd.Parameters.AddWithValue("@itemDesc", _fDes & "%")
                            End If
                            If Not String.IsNullOrEmpty(_searchInvVal) Then
                                cmd.Parameters.AddWithValue("@invNo", _searchInvVal)
                            Else
                                cmd.Parameters.AddWithValue("@start", startDate.ToString("yyyy-MM-dd") & " 00:00:00")
                                cmd.Parameters.AddWithValue("@end", endDate.ToString("yyyy-MM-dd") & " 23:59:59")
                                cmd.Parameters.AddWithValue("@invType", _filterInvType)
                                cmd.Parameters.AddWithValue("@bType", _filterBilling)
                            End If
                            Using da As New MySqlDataAdapter(cmd)
                                Dim dt As New DataTable("Command")
                                da.Fill(dt)
                                ds.Tables.Add(dt)
                            End Using
                        End Using
                    End Using
                    
                    rpt.SetDataSource(ds)
                Catch ex As Exception
                End Try
                formula = ""
            ElseIf cmbReportType.SelectedIndex = 3 Then ' Stock Report
                Dim stockFilters As New List(Of String)
                ' Use * for wildcards in Crystal Reports Selection Formula
                ' Updated to use aliases like items1, brand1, items_stock1 as seen in the report designer
                
                Dim fItem As String = If(Not String.IsNullOrEmpty(txtSearchName.Text), txtSearchName.Text.Trim(), _fItemName)
                
                If Not String.IsNullOrEmpty(_searchInv) Then stockFilters.Add("{items1.id} LIKE '" & _searchInv.Replace("'", "''") & "*'")
                If Not String.IsNullOrEmpty(fItem) Then stockFilters.Add("({items1.description} LIKE '*" & fItem.Replace("'", "''") & "*' OR {items_stock1.description} LIKE '*" & fItem.Replace("'", "''") & "*')")
                If Not String.IsNullOrEmpty(_fBrand) Then stockFilters.Add("{brand1.name} LIKE '*" & _fBrand.Replace("'", "''") & "*'")
                If Not String.IsNullOrEmpty(_fDes) Then stockFilters.Add("{items_stock1.description} LIKE '*" & _fDes.Replace("'", "''") & "*'")

                Dim dateField As String = "{items_stock1.date}"
                
                If Not chkAllDates.Checked Then
                    stockFilters.Add(dateField & " >= DateTime(" & startDate.Year & ", " & startDate.Month & ", " & startDate.Day & ", 0, 0, 0)")
                    stockFilters.Add(dateField & " <= DateTime(" & endDate.Year & ", " & endDate.Month & ", " & endDate.Day & ", 23, 59, 59)")
                End If

                If stockFilters.Count > 0 Then
                    formula = String.Join(" AND ", stockFilters)
                End If
            ElseIf cmbReportType.SelectedIndex = 10 Then ' Stock Return
                Dim dateField As String = GetReportField(rpt, "timestamps")
                If String.IsNullOrEmpty(dateField) Then dateField = GetReportField(rpt, "date")
                If Not String.IsNullOrEmpty(dateField) Then
                    If chkAllDates.Checked Then
                        formula = ""
                    Else
                        formula = dateField & " >= DateTime(" & startDate.Year & ", " & startDate.Month & ", " & startDate.Day & ", 0, 0, 0) AND " &
                                  dateField & " <= DateTime(" & endDate.Year & ", " & endDate.Month & ", " & endDate.Day & ", 23, 59, 59)"
                    End If
                End If
            ElseIf cmbReportType.SelectedIndex = 17 Then ' Purchase History
                Dim dateField As String = GetReportField(rpt, "pur_date")
                If String.IsNullOrEmpty(dateField) Then dateField = "{purchasing1.pur_date}"
                If chkAllDates.Checked Then
                    formula = ""
                Else
                    formula = dateField & " >= DateTime(" & startDate.Year & ", " & startDate.Month & ", " & startDate.Day & ", 0, 0, 0) AND " &
                              dateField & " <= DateTime(" & endDate.Year & ", " & endDate.Month & ", " & endDate.Day & ", 23, 59, 59)"
                End If
            ElseIf cmbReportType.SelectedIndex = 18 Then ' Purchase Return History
                ' Fallback to SaleReturn if dedicated doesn't exist, or just use it as a placeholder
                Dim dateField As String = GetReportField(rpt, "return_date")
                If String.IsNullOrEmpty(dateField) Then dateField = "{purchase_return1.return_date}"
                
                Dim dateFilter As String = ""
                If Not chkAllDates.Checked Then
                    dateFilter = dateField & " >= DateTime(" & startDate.Year & ", " & startDate.Month & ", " & startDate.Day & ", 0, 0, 0) AND " &
                                 dateField & " <= DateTime(" & endDate.Year & ", " & endDate.Month & ", " & endDate.Day & ", 23, 59, 59)"
                End If

                If Not String.IsNullOrEmpty(_searchInvVal) AndAlso _searchInvVal.ToLower() <> "all" Then
                    formula = "{purchase_return1.pur_id} = """ & _searchInvVal.Replace("""", """""") & """"
                Else
                    formula = dateFilter
                End If

                ' Fix for duplicate items in Purchase Return Report due to missing joins in the .rpt file
                Dim prJoinFix As String = "{purchase_return1.pur_id} = {purchasing1.pur_id} AND {purchase_return1.description} = {items_stock1.description}"
                If String.IsNullOrEmpty(formula) Then
                    formula = prJoinFix
                Else
                    formula = "(" & formula & ") AND " & prJoinFix
                End If
            ElseIf cmbReportType.SelectedIndex = 19 Then ' Purchase Request
                Dim dateField As String = GetReportField(rpt, "request_date")
                If String.IsNullOrEmpty(dateField) Then dateField = "{purchase_request1.request_date}"

                Dim dateFilter As String = ""
                If chkAllDates.Checked Then
                    dateFilter = "{purchase_request1.request_id} = {purchase_request_items1.request_id}"
                Else
                    dateFilter = "(" & dateField & " >= DateTime(" & startDate.Year & ", " & startDate.Month & ", " & startDate.Day & ", 0, 0, 0) AND " &
                                  dateField & " <= DateTime(" & endDate.Year & ", " & endDate.Month & ", " & endDate.Day & ", 23, 59, 59) AND " &
                                  "{purchase_request1.request_id} = {purchase_request_items1.request_id})"
                End If

                If Not String.IsNullOrEmpty(_searchInvVal) Then
                    formula = "({purchase_request1.request_id} = """ & _searchInvVal.Replace("""", """""") & """ AND {purchase_request_items1.request_id} = """ & _searchInvVal.Replace("""", """""") & """)"
                Else
                    formula = dateFilter
                End If
            ElseIf cmbReportType.SelectedIndex = 11 OrElse cmbReportType.SelectedIndex = 12 OrElse cmbReportType.SelectedIndex = 22 Then ' Sale Invoice (A4 / POS / Bill Details)
                Dim isQT As Boolean = Not String.IsNullOrEmpty(_searchInvVal) AndAlso _searchInvVal.StartsWith("QT")
                Dim tPrefix As String = If(isQT, "quotation_billing1", "billing1")

                Dim tsField As String = GetReportField(rpt, "timestamps")
                If String.IsNullOrEmpty(tsField) Then tsField = GetReportField(rpt, "sale_time")
                If String.IsNullOrEmpty(tsField) Then tsField = GetReportField(rpt, "date")
                If String.IsNullOrEmpty(tsField) Then tsField = "{" & tPrefix & ".timestamps}"

                Dim dateFilter As String = ""
                If Not chkAllDates.Checked OrElse Not Module1.IsRgrVisible Then
                    dateFilter = tsField & " >= DateTime(" & startDate.Year & ", " & startDate.Month & ", " & startDate.Day & ", 0, 0, 0) AND " &
                                 tsField & " <= DateTime(" & endDate.Year & ", " & endDate.Month & ", " & endDate.Day & ", 23, 59, 59)"
                End If

                Dim invField As String = GetReportField(rpt, "inv_no")
                Dim printedInvField As String = GetReportField(rpt, "printed_inv_no")

                If Not String.IsNullOrEmpty(_searchInvVal) AndAlso _searchInvVal.ToLower() <> "all" AndAlso _searchInvVal.ToLower() <> "el bills" Then
                    Dim filterParts As New List(Of String)()
                    If Not String.IsNullOrEmpty(invField) Then filterParts.Add(invField & " = """ & _searchInvVal.Replace("""", """""") & """")
                    If Not String.IsNullOrEmpty(printedInvField) Then filterParts.Add(printedInvField & " = """ & _searchInvVal.Replace("""", """""") & """")
                    If filterParts.Count = 0 Then filterParts.Add("{" & tPrefix & ".inv_no} = """ & _searchInvVal.Replace("""", """""") & """")
                    
                    formula = "(" & String.Join(" OR ", filterParts) & ")"
                ElseIf Not String.IsNullOrEmpty(_searchInvVal) AndAlso _searchInvVal.ToLower() = "el bills" Then
                    formula = "{" & tPrefix & ".printed_inv_no} LIKE ""EL*"""
                    If Not String.IsNullOrEmpty(dateFilter) Then
                        formula &= " AND (" & dateFilter & ")"
                    End If
                Else
                    formula = dateFilter
                End If

                ' Apply security lock for Bill Details and other invoices
                Dim finRole As String = If(Module1.FinancialRole IsNot Nothing, Module1.FinancialRole.ToLower(), "")
                Dim canAccessGR As Boolean = (Module1.UserRole.ToLower() = "cashier" AndAlso finRole = "seller") AndAlso Module1.IsRgrVisible
                If Not Module1.IsRgrVisible OrElse finRole = "seller" Then
                    Dim invFilter As String = ""
                    If canAccessGR Then
                        invFilter = "(NOT ({" & tPrefix & ".printed_inv_no} LIKE ""RGR*""))"
                    Else
                        invFilter = "(NOT ({" & tPrefix & ".printed_inv_no} LIKE ""GR*"") AND NOT ({" & tPrefix & ".printed_inv_no} LIKE ""gr*"") AND NOT ({" & tPrefix & ".printed_inv_no} LIKE ""RGR*""))"
                    End If
                    If cmbReportType.SelectedIndex = 22 AndAlso finRole = "seller" AndAlso Not canAccessGR Then
                        invFilter &= " AND ({" & tPrefix & ".printed_inv_no} LIKE ""EL*"" OR {" & tPrefix & ".printed_inv_no} LIKE ""VT*"")"
                    End If

                    If String.IsNullOrEmpty(formula) Then
                        formula = invFilter
                    Else
                        formula = "(" & formula & ") AND " & invFilter
                    End If
                End If
            ElseIf cmbReportType.SelectedIndex = 13 Then ' Sale Return Invoice
                Dim tsField As String = GetReportField(rpt, "return_date")
                If String.IsNullOrEmpty(tsField) Then tsField = GetReportField(rpt, "timestamps")
                If String.IsNullOrEmpty(tsField) Then tsField = GetReportField(rpt, "date")
                If String.IsNullOrEmpty(tsField) Then tsField = "{sales_return1.return_date}"

                Dim dateFilter As String = ""
                If Not chkAllDates.Checked Then
                    dateFilter = tsField & " >= DateTime(" & startDate.Year & ", " & startDate.Month & ", " & startDate.Day & ", 0, 0, 0) AND " &
                                 tsField & " <= DateTime(" & endDate.Year & ", " & endDate.Month & ", " & endDate.Day & ", 23, 59, 59)"
                End If

                If Not String.IsNullOrEmpty(_searchInvVal) AndAlso _searchInvVal.ToLower() <> "all" Then
                    formula = "{sales_return1.inv_no} = """ & _searchInvVal.Replace("""", """""") & """"
                Else
                    formula = dateFilter
                End If

                Dim finRole As String = If(Module1.FinancialRole IsNot Nothing, Module1.FinancialRole.ToLower(), "")
                Dim canAccessGR As Boolean = (Module1.UserRole.ToLower() = "cashier" AndAlso finRole = "seller") AndAlso Module1.IsRgrVisible
                If Not Module1.IsRgrVisible OrElse finRole = "seller" Then
                    Dim rgrFilter As String = ""
                    If canAccessGR Then
                        rgrFilter = "(NOT ({sales_return1.inv_no} LIKE ""RGR*""))"
                    Else
                        rgrFilter = "(NOT ({sales_return1.inv_no} LIKE ""GR*"") AND NOT ({sales_return1.inv_no} LIKE ""gr*"") AND NOT ({sales_return1.inv_no} LIKE ""RGR*""))"
                    End If
                    If String.IsNullOrEmpty(formula) Then
                        formula = rgrFilter
                    Else
                        formula = "(" & formula & ") AND " & rgrFilter
                    End If
                End If
            ElseIf cmbReportType.SelectedIndex = 14 Then ' Purchase Invoice
                Dim tsField As String = GetReportField(rpt, "pur_date")
                If String.IsNullOrEmpty(tsField) Then tsField = GetReportField(rpt, "date")
                If String.IsNullOrEmpty(tsField) Then tsField = "{purchasing1.pur_date}"

                Dim dateFilter As String = ""
                If Not chkAllDates.Checked Then
                    dateFilter = tsField & " >= DateTime(" & startDate.Year & ", " & startDate.Month & ", " & startDate.Day & ", 0, 0, 0) AND " &
                                 tsField & " <= DateTime(" & endDate.Year & ", " & endDate.Month & ", " & endDate.Day & ", 23, 59, 59)"
                End If

                If Not String.IsNullOrEmpty(_searchInvVal) AndAlso _searchInvVal.ToLower() <> "all" Then
                    formula = "{purchasing1.pur_id} = """ & _searchInvVal.Replace("""", """""") & """"
                Else
                    formula = dateFilter
                End If
            ElseIf cmbReportType.SelectedIndex = 15 OrElse cmbReportType.SelectedIndex = 16 Then ' Quotation / Quotation POS
                Dim tsField As String = GetReportField(rpt, "timestamps")
                If String.IsNullOrEmpty(tsField) Then tsField = GetReportField(rpt, "date")
                If String.IsNullOrEmpty(tsField) Then tsField = "{quotation_billing1.timestamps}"

                Dim invField As String = GetReportField(rpt, "inv_no")
                Dim printedInvField As String = GetReportField(rpt, "printed_inv_no")

                Dim dateFilter As String = ""
                If Not chkAllDates.Checked Then
                    dateFilter = tsField & " >= DateTime(" & startDate.Year & ", " & startDate.Month & ", " & startDate.Day & ", 0, 0, 0) AND " &
                                 tsField & " <= DateTime(" & endDate.Year & ", " & endDate.Month & ", " & endDate.Day & ", 23, 59, 59)"
                End If

                If Not String.IsNullOrEmpty(_searchInvVal) AndAlso _searchInvVal.ToLower() <> "all" Then
                    Dim filterParts As New List(Of String)()
                    If Not String.IsNullOrEmpty(invField) Then filterParts.Add(invField & " = """ & _searchInvVal.Replace("""", """""") & """")
                    If Not String.IsNullOrEmpty(printedInvField) Then filterParts.Add(printedInvField & " = """ & _searchInvVal.Replace("""", """""") & """")
                    If filterParts.Count = 0 Then filterParts.Add("{quotation_billing1.inv_no} = """ & _searchInvVal.Replace("""", """""") & """")
                    
                    formula = "(" & String.Join(" OR ", filterParts) & ")"
                Else
                    formula = dateFilter
                End If
            ElseIf cmbReportType.SelectedIndex = 21 Then ' Full Credit Report
                formula = ""

            ElseIf cmbReportType.SelectedIndex = 24 Then ' Full Debit Report
                formula = ""

            ElseIf cmbReportType.SelectedIndex = 25 Then ' Full Cheque Report
                formula = ""

            ElseIf cmbReportType.SelectedIndex = 23 Then ' Supplier Payment
                formula = "{supplier_payments1.inv_no} = {supplicer_credit1.inv_no} AND {supplier_payments1.supplier_id} = {supplicer_credit1.supplier_id}"
            End If

            ' Add dynamic date filters for specific reports that need it
            Dim filterIdx As Integer = cmbReportType.SelectedIndex
            If (New Integer() {5, 6, 8, 9, 20, 21, 23, 24, 25, 26}).Contains(filterIdx) Then
                Dim dateField As String = ""
                
                Select Case filterIdx
                    Case 5, 21, 26 ' Customer Credit, Full Credit, Credit by City
                        dateField = GetReportField(rpt, "timestamps")
                        If String.IsNullOrEmpty(dateField) Then dateField = GetReportField(rpt, "date")
                    Case 6, 25 ' Customer Cheque, Full Cheque
                        dateField = GetReportField(rpt, "issue_date")
                        If String.IsNullOrEmpty(dateField) Then dateField = GetReportField(rpt, "check_release_date")
                        If String.IsNullOrEmpty(dateField) Then dateField = GetReportField(rpt, "recive_date")
                    Case 20 ' Customer Payment Note
                        dateField = GetReportField(rpt, "Date")
                        If String.IsNullOrEmpty(dateField) Then dateField = GetReportField(rpt, "payment_date")
                    Case 8, 24 ' Supplier Debit, Full Debit
                        dateField = GetReportField(rpt, "getdate")
                        If String.IsNullOrEmpty(dateField) Then dateField = GetReportField(rpt, "date")
                    Case 9 ' Supplier Cheque
                        dateField = GetReportField(rpt, "issue_date")
                        If String.IsNullOrEmpty(dateField) Then dateField = GetReportField(rpt, "date")
                    Case 23 ' Supplier Payment
                        dateField = GetReportField(rpt, "pdate")
                        If String.IsNullOrEmpty(dateField) Then dateField = GetReportField(rpt, "payment_date")
                End Select
                
                If Not String.IsNullOrEmpty(dateField) AndAlso Not chkAllDates.Checked Then
                    Dim dateFilter As String = ""
                    If filterIdx = 6 OrElse filterIdx = 25 Then
                        ' Customer Cheque dates are stored as VARCHAR. We must use String comparison.
                        Dim startStr As String = startDate.ToString("yyyy-MM-dd")
                        Dim endStr As String = endDate.ToString("yyyy-MM-dd")
                        If startStr = endStr Then
                            dateFilter = "Trim(" & dateField & ") LIKE '" & startStr & "*'"
                        Else
                            dateFilter = "Trim(" & dateField & ") >= '" & startStr & "' AND Trim(" & dateField & ") <= '" & endStr & " 23:59:59'"
                        End If
                    Else
                        dateFilter = dateField & " >= DateTime(" & startDate.Year & ", " & startDate.Month & ", " & startDate.Day & ", 0, 0, 0) AND " &
                                     dateField & " <= DateTime(" & endDate.Year & ", " & endDate.Month & ", " & endDate.Day & ", 23, 59, 59)"
                    End If

                    If String.IsNullOrEmpty(formula) Then
                        formula = dateFilter
                    Else
                        formula &= " AND (" & dateFilter & ")"
                    End If
                End If
            End If

            ' Apply Name Search if provided
            If Not String.IsNullOrEmpty(txtSearchName.Text) AndAlso txtSearchName.Text.Trim() <> "--- ALL ---" Then
                Dim idx As Integer = cmbReportType.SelectedIndex
                Dim searchTerm As String = txtSearchName.Text.Trim().Replace("""", """""")
                Dim nameFilter As String = ""

                If idx = 26 Then
                    nameFilter = "Trim(UpperCase({customer1.city})) = """ & searchTerm.ToUpper() & """"
                ElseIf (idx >= 4 AndAlso idx <= 6) OrElse idx = 20 OrElse idx = 21 OrElse idx = 25 Then
                    ' --- Customer Reports: scan dynamically ---
                    Dim nameField As String = GetReportField(rpt, "customer_name")
                    If String.IsNullOrEmpty(nameField) Then nameField = GetReportField(rpt, "check_name")
                    If String.IsNullOrEmpty(nameField) Then nameField = GetReportField(rpt, "c_name")
                    If String.IsNullOrEmpty(nameField) Then nameField = GetReportField(rpt, "name")
                    If String.IsNullOrEmpty(nameField) Then nameField = GetReportField(rpt, "customer")
                    If Not String.IsNullOrEmpty(nameField) Then
                        nameFilter = "Trim(UpperCase(" & nameField & ")) = """ & searchTerm.ToUpper() & """"
                    End If

                ElseIf (idx >= 7 AndAlso idx <= 8) OrElse idx = 23 OrElse idx = 24 Then
                    ' --- Supplier List, Supplier Debit, Supplier Payment, Full Debit: scan dynamically ---
                    Dim nameField As String = GetReportField(rpt, "supplier_name")
                    If String.IsNullOrEmpty(nameField) Then nameField = GetReportField(rpt, "sname")
                    If String.IsNullOrEmpty(nameField) Then nameField = GetReportField(rpt, "name")
                    If String.IsNullOrEmpty(nameField) Then nameField = GetReportField(rpt, "supplier")
                    If Not String.IsNullOrEmpty(nameField) Then
                        nameFilter = "Trim(UpperCase(" & nameField & ")) = """ & searchTerm.ToUpper() & """"
                    End If

                ElseIf idx = 9 Then
                    ' --- Supplier Cheque (chaque_issue table joins supplier) ---
                    ' Try dynamic scan first: c_name is stored directly in chaque_issue
                    Dim nameField As String = GetReportField(rpt, "c_name")
                    If String.IsNullOrEmpty(nameField) Then nameField = GetReportField(rpt, "name")
                    If String.IsNullOrEmpty(nameField) Then nameField = GetReportField(rpt, "sname")
                    If Not String.IsNullOrEmpty(nameField) Then
                        nameFilter = "Trim(UpperCase(" & nameField & ")) = """ & searchTerm.ToUpper() & """"
                    Else
                        ' Hardcoded fallback using known table aliases
                        nameFilter = "Trim(UpperCase({chaque_issue1.c_name})) = """ & searchTerm.ToUpper() & """"
                    End If

                ElseIf idx = 17 Then
                    ' --- Purchase History (purchasing table joins supplier via supplier_id) ---
                    ' The Crystal Report joins supplier table; use supplier.name
                    Dim nameField As String = GetReportField(rpt, "name")
                    If String.IsNullOrEmpty(nameField) Then nameField = GetReportField(rpt, "supplier_name")
                    If String.IsNullOrEmpty(nameField) Then nameField = GetReportField(rpt, "sname")
                    If Not String.IsNullOrEmpty(nameField) Then
                        nameFilter = "Trim(UpperCase(" & nameField & ")) = """ & searchTerm.ToUpper() & """"
                    Else
                        nameFilter = "Trim(UpperCase({supplier1.name})) = """ & searchTerm.ToUpper() & """"
                    End If

                ElseIf idx = 18 Then
                    ' --- Purchase Return History (purchase_return joins supplier via supplier_id) ---
                    Dim nameField As String = GetReportField(rpt, "name")
                    If String.IsNullOrEmpty(nameField) Then nameField = GetReportField(rpt, "supplier_name")
                    If Not String.IsNullOrEmpty(nameField) Then
                        nameFilter = "Trim(UpperCase(" & nameField & ")) = """ & searchTerm.ToUpper() & """"
                    Else
                        nameFilter = "Trim(UpperCase({supplier1.name})) = """ & searchTerm.ToUpper() & """"
                    End If

                ElseIf idx = 11 OrElse idx = 12 OrElse idx = 13 OrElse idx = 15 OrElse idx = 16 OrElse idx = 22 Then
                    ' --- Sale Invoice, Return, Quotation ---
                    Dim nameField As String = GetReportField(rpt, "name")
                    If String.IsNullOrEmpty(nameField) Then nameField = GetReportField(rpt, "customer_name")
                    If String.IsNullOrEmpty(nameField) Then nameField = GetReportField(rpt, "c_name")
                    If String.IsNullOrEmpty(nameField) Then nameField = GetReportField(rpt, "customer")
                    If Not String.IsNullOrEmpty(nameField) Then
                        nameFilter = "Trim(UpperCase(" & nameField & ")) = """ & searchTerm.ToUpper() & """"
                    End If

                ElseIf idx = 14 OrElse idx = 19 Then
                    ' --- Purchase Invoice, Purchase Request ---
                    Dim nameField As String = GetReportField(rpt, "name")
                    If String.IsNullOrEmpty(nameField) Then nameField = GetReportField(rpt, "supplier_name")
                    If String.IsNullOrEmpty(nameField) Then nameField = GetReportField(rpt, "sname")
                    If String.IsNullOrEmpty(nameField) Then nameField = GetReportField(rpt, "supplier")
                    If Not String.IsNullOrEmpty(nameField) Then
                        nameFilter = "Trim(UpperCase(" & nameField & ")) = """ & searchTerm.ToUpper() & """"
                    End If
                End If

                ' Append name filter to main formula
                If Not String.IsNullOrEmpty(nameFilter) Then
                    If String.IsNullOrEmpty(formula) Then
                        formula = nameFilter
                    Else
                        formula &= " AND " & nameFilter
                    End If
                End If
            End If

            ' Apply RGR visibility filter
            Dim finRole2 As String = If(Module1.FinancialRole IsNot Nothing, Module1.FinancialRole.ToLower(), "")
            Dim canAccessGR2 As Boolean = (Module1.UserRole.ToLower() = "cashier" AndAlso finRole2 = "seller") AndAlso Module1.IsRgrVisible
            If Not Module1.IsRgrVisible OrElse finRole2 = "seller" Then
                Dim rgrField As String = GetReportField(rpt, "is_rgr")
                If Not String.IsNullOrEmpty(rgrField) Then
                    If String.IsNullOrEmpty(formula) Then
                        formula = rgrField & " = 0"
                    Else
                        formula &= " AND " & rgrField & " = 0"
                    End If
                End If
                Dim invNoField As String = GetReportField(rpt, "inv_no")
                If Not String.IsNullOrEmpty(invNoField) Then
                    Dim invNoFilter As String = ""
                    If canAccessGR2 Then
                        invNoFilter = "not (" & invNoField & " startswith 'RGR')"
                    Else
                        invNoFilter = "not (" & invNoField & " startswith 'GR') AND not (" & invNoField & " startswith 'RGR')"
                    End If
                    If String.IsNullOrEmpty(formula) Then
                        formula = invNoFilter
                    Else
                        formula &= " AND " & invNoFilter
                    End If
                End If
            End If

            ' Always set formula (clear it if empty, so previous filter doesn't stick)
            rpt.RecordSelectionFormula = If(Not String.IsNullOrEmpty(formula), formula, "")

            ' Dynamic Original/Duplicate label printing override
            Try
                If cmbReportType.SelectedIndex = 11 OrElse cmbReportType.SelectedIndex = 12 Then
                    Dim rcdProp = rpt.GetType().GetProperty("ReportClientDocument")
                    If rcdProp IsNot Nothing Then
                        Dim rcd As Object = rcdProp.GetValue(rpt, Nothing)
                        If rcd IsNot Nothing Then
                            Dim objs As Object = rcd.ReportDefController.ReportObjectController.GetAllReportObjects()
                            For Each oldObj As Object In objs
                                Dim objName As String = oldObj.Name.ToString()
                                Dim isOriginal As Boolean = False
                                Dim isDuplicate As Boolean = False
                                
                                If cmbReportType.SelectedIndex = 12 Then ' POS Invoice
                                    If String.Equals(objName, "Text28", StringComparison.OrdinalIgnoreCase) Then isOriginal = True
                                    If String.Equals(objName, "Text15", StringComparison.OrdinalIgnoreCase) Then isDuplicate = True
                                ElseIf cmbReportType.SelectedIndex = 11 Then ' Standard Invoice
                                    If String.Equals(objName, "Text29", StringComparison.OrdinalIgnoreCase) Then isOriginal = True
                                    If String.Equals(objName, "Text28", StringComparison.OrdinalIgnoreCase) Then isDuplicate = True
                                End If

                                If isOriginal OrElse isDuplicate Then
                                    Dim newObj As Object = oldObj.Clone(True)
                                    newObj.Format.ConditionFormulas.RemoveAll()
                                    ' In SalesHistoryForm (Reprint), show duplicate, hide original
                                    newObj.Format.EnableSuppress = isOriginal
                                    rcd.ReportDefController.ReportObjectController.Modify(oldObj, newObj)
                                End If
                            Next
                        End If
                    End If
                End If
            Catch exOrigDup As Exception
                ' Silent execution to avoid blocking the main report load
            End Try

            ' Dynamic Advance Payment printing override (suppress if 0 or empty)
            Try
                If cmbReportType.SelectedIndex = 11 OrElse cmbReportType.SelectedIndex = 12 Then
                    Dim advPayAmt As Decimal = 0
                    Dim queryTable As String = "billing"
                    Dim invoiceNo As String = If(Not String.IsNullOrEmpty(txtSearchInv.Text), txtSearchInv.Text.Trim(), _searchInv)
                    If Not String.IsNullOrEmpty(invoiceNo) Then
                        If invoiceNo.StartsWith("QT") Then
                            queryTable = "quotation_billing"
                        End If
                        Using connCheck As New MySql.Data.MySqlClient.MySqlConnection(Module1.ConnStr)
                            connCheck.Open()
                            Dim q As String = "SELECT adv_pay_amount FROM " & queryTable & " WHERE inv_no = @inv"
                            If queryTable = "billing" Then
                                q = "SELECT adv_pay_amount FROM billing WHERE inv_no = @inv OR printed_inv_no = @inv"
                            End If
                            Try
                                Using cmd As New MySql.Data.MySqlClient.MySqlCommand(q, connCheck)
                                    cmd.Parameters.AddWithValue("@inv", invoiceNo)
                                    Dim res = cmd.ExecuteScalar()
                                    If res IsNot Nothing AndAlso res IsNot DBNull.Value Then
                                        Decimal.TryParse(res.ToString(), advPayAmt)
                                    End If
                                End Using
                            Catch ex As Exception
                                Try
                                    Dim qFallback As String = "SELECT advance_payment FROM " & queryTable & " WHERE inv_no = @inv"
                                    If queryTable = "billing" Then
                                        qFallback = "SELECT advance_payment FROM billing WHERE inv_no = @inv OR printed_inv_no = @inv"
                                    End If
                                    Using cmd As New MySql.Data.MySqlClient.MySqlCommand(qFallback, connCheck)
                                        cmd.Parameters.AddWithValue("@inv", invoiceNo)
                                        Dim res = cmd.ExecuteScalar()
                                        If res IsNot Nothing AndAlso res IsNot DBNull.Value Then
                                            Decimal.TryParse(res.ToString(), advPayAmt)
                                        End If
                                    End Using
                                Catch
                                End Try
                            End Try
                        End Using
                    End If

                    Dim suppressAdv As Boolean = (advPayAmt <= 0)
                    Dim targetSectionName As String = If(cmbReportType.SelectedIndex = 12, "ReportFooterSection8", "ReportFooterSection9")
                    Dim targetHeight As Integer = If(suppressAdv, 720, 960)
                    Dim targetObjTop As Integer = If(suppressAdv, 0, 720)
                    Dim targetObjHeight As Integer = If(suppressAdv, 0, 240)

                    Dim rcdProp = rpt.GetType().GetProperty("ReportClientDocument")
                    If rcdProp IsNot Nothing Then
                        Dim rcd As Object = rcdProp.GetValue(rpt, Nothing)
                        If rcd IsNot Nothing Then
                            Dim objs As Object = rcd.ReportDefController.ReportObjectController.GetAllReportObjects()
                            For Each oldObj As Object In objs
                                Dim objName As String = oldObj.Name.ToString()
                                If String.Equals(objName, "Text23", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(objName, "advpayamount1", StringComparison.OrdinalIgnoreCase) Then
                                    Dim newObj As Object = oldObj.Clone(True)
                                    newObj.Top = targetObjTop
                                    newObj.Height = targetObjHeight
                                    newObj.Format.ConditionFormulas.RemoveAll()
                                    newObj.Format.EnableSuppress = suppressAdv
                                    rcd.ReportDefController.ReportObjectController.Modify(oldObj, newObj)
                                End If
                            Next

                            ' Modify the section height to collapse the gap
                            Dim areas As Object = rcd.ReportDefController.ReportDefinition.Areas
                            Dim targetSection As Object = Nothing
                            For i As Integer = 0 To areas.Count - 1
                                Dim area As Object = areas(i)
                                For j As Integer = 0 To area.Sections.Count - 1
                                    Dim section As Object = area.Sections(j)
                                    If String.Equals(section.Name.ToString(), targetSectionName, StringComparison.OrdinalIgnoreCase) Then
                                        targetSection = section
                                        Exit For
                                    End If
                                Next
                                If targetSection IsNot Nothing Then Exit For
                            Next

                            If targetSection IsNot Nothing Then
                                rcd.ReportDefController.ReportSectionController.SetProperty(targetSection, 2, targetHeight)
                            End If
                        End If
                    End If
                End If
            Catch exAdv As Exception
                ' Silent execution to avoid blocking the main report load
            End Try

            ' Dynamic Customer VAT ID printing override (suppress if empty)
            Try
                If cmbReportType.SelectedIndex = 11 OrElse cmbReportType.SelectedIndex = 12 Then ' Sale Invoice (A4) or Sale Invoice (POS)
                    Dim invoiceNoVat As String = If(Not String.IsNullOrEmpty(txtSearchInv.Text), txtSearchInv.Text.Trim(), _searchInv)
                    If Not String.IsNullOrEmpty(invoiceNoVat) Then
                        Dim cusVatId As String = ""
                        Dim queryTable As String = "billing"
                        If invoiceNoVat.StartsWith("QT") Then
                            queryTable = "quotation_billing"
                        End If
                        Using connVat As New MySql.Data.MySqlClient.MySqlConnection(Module1.ConnStr)
                            connVat.Open()
                            Dim q As String = "SELECT cus_vat_id FROM " & queryTable & " WHERE inv_no = @inv"
                            If queryTable = "billing" Then
                                q = "SELECT cus_vat_id FROM billing WHERE inv_no = @inv OR printed_inv_no = @inv"
                            End If
                            Try
                                Using cmd As New MySql.Data.MySqlClient.MySqlCommand(q, connVat)
                                    cmd.Parameters.AddWithValue("@inv", invoiceNoVat)
                                    Dim res = cmd.ExecuteScalar()
                                    If res IsNot Nothing AndAlso res IsNot DBNull.Value Then
                                        cusVatId = res.ToString().Trim()
                                    End If
                                End Using
                            Catch
                            End Try
                        End Using

                        Dim suppressVatId As Boolean = String.IsNullOrEmpty(cusVatId)

                        ' Find the names of the objects to hide/show using the Engine
                        Dim vatObjectsToHide As New List(Of String)()
                        Dim taxInvoiceObjs As New List(Of String)()
                        Dim normalInvoiceObjs As New List(Of String)()

                        For Each sec As CrystalDecisions.CrystalReports.Engine.Section In rpt.ReportDefinition.Sections
                            For Each obj As CrystalDecisions.CrystalReports.Engine.ReportObject In sec.ReportObjects
                                If obj.Name.IndexOf("cus_vat_id", StringComparison.OrdinalIgnoreCase) >= 0 Then
                                    vatObjectsToHide.Add(obj.Name)
                                ElseIf TypeOf obj Is CrystalDecisions.CrystalReports.Engine.TextObject Then
                                    Dim txtObj = DirectCast(obj, CrystalDecisions.CrystalReports.Engine.TextObject)
                                    Dim tStr As String = txtObj.Text
                                    If Not String.IsNullOrEmpty(tStr) Then
                                        Dim upperStr As String = tStr.ToUpper().Trim()
                                        ' Explicitly avoid the company's "Regd VAT No" label
                                        If upperStr.Contains("REGD VAT") OrElse upperStr.Contains("REGD. VAT") Then
                                            ' skip
                                        ElseIf upperStr.Contains("VAT ID") OrElse upperStr.Contains("CUS VAT") OrElse upperStr.Contains("CUSTOMER VAT") Then
                                            vatObjectsToHide.Add(obj.Name)
                                        ElseIf upperStr = "TAX INVOICE" OrElse upperStr = "TAX  INVOICE" Then
                                            taxInvoiceObjs.Add(obj.Name)
                                        ElseIf upperStr = "SALE INVOICE" OrElse upperStr = "SALES INVOICE" OrElse upperStr = "INVOICE" Then
                                            normalInvoiceObjs.Add(obj.Name)
                                        End If
                                    End If
                                End If
                            Next
                        Next

                        ' Apply suppression and layout adjustments using the ReportClientDocument
                        Dim rcdProp = rpt.GetType().GetProperty("ReportClientDocument")
                        If rcdProp IsNot Nothing Then
                            Dim rcd As Object = rcdProp.GetValue(rpt, Nothing)
                            If rcd IsNot Nothing Then
                                Dim objs As Object = rcd.ReportDefController.ReportObjectController.GetAllReportObjects()
                                For Each oldObj As Object In objs
                                    Dim objName As String = oldObj.Name.ToString()
                                    
                                    ' Toggle VAT ID fields
                                    If vatObjectsToHide.Contains(objName) Then
                                        Dim newObj As Object = oldObj.Clone(True)
                                        newObj.Format.ConditionFormulas.RemoveAll()
                                        newObj.Format.EnableSuppress = suppressVatId
                                        
                                        ' Prevent overlap on A4 by shifting the field and label to the far right side of the invoice
                                        If Not suppressVatId AndAlso cmbReportType.SelectedIndex = 11 Then
                                            If objName.IndexOf("cus_vat_id", StringComparison.OrdinalIgnoreCase) >= 0 Then
                                                ' Database Field Value -> move far right
                                                newObj.Left = 7800
                                            Else
                                                ' Text Label -> move right, just before the value
                                                newObj.Left = 6000
                                            End If
                                        End If

                                        rcd.ReportDefController.ReportObjectController.Modify(oldObj, newObj)
                                        
                                    ' Toggle TAX INVOICE title
                                    ElseIf taxInvoiceObjs.Contains(objName) Then
                                        Dim newObj As Object = oldObj.Clone(True)
                                        newObj.Format.ConditionFormulas.RemoveAll()
                                        newObj.Format.EnableSuppress = suppressVatId ' Suppress TAX INVOICE if no VAT
                                        rcd.ReportDefController.ReportObjectController.Modify(oldObj, newObj)
                                        
                                    ' Toggle SALE INVOICE / INVOICE title
                                    ElseIf normalInvoiceObjs.Contains(objName) Then
                                        Dim newObj As Object = oldObj.Clone(True)
                                        newObj.Format.ConditionFormulas.RemoveAll()
                                        newObj.Format.EnableSuppress = Not suppressVatId ' Suppress normal invoice if HAS VAT
                                        rcd.ReportDefController.ReportObjectController.Modify(oldObj, newObj)
                                    End If
                                Next
                            End If
                        End If
                    End If
                End If
            Catch exVat As Exception
                ' Silent execution to avoid blocking the main report load
            End Try

            ' Dynamic Cheque & Change/Credit Payment printing override (suppress if 0 or empty)
            Try
                If cmbReportType.SelectedIndex = 11 OrElse cmbReportType.SelectedIndex = 12 Then
                    Dim chequeAmt As Decimal = 0
                    Dim changeAmt As Decimal = 0
                    Dim creditAmt As Decimal = 0
                    Dim queryTable As String = "billing"
                    Dim invoiceNo As String = If(Not String.IsNullOrEmpty(txtSearchInv.Text), txtSearchInv.Text.Trim(), _searchInv)
                    If Not String.IsNullOrEmpty(invoiceNo) Then
                        If invoiceNo.StartsWith("QT") Then
                            queryTable = "quotation_billing"
                        End If
                        Using connCheck As New MySql.Data.MySqlClient.MySqlConnection(Module1.ConnStr)
                            connCheck.Open()
                            Dim q As String = "SELECT cheque_balance_due, change_amount, credit_balance_due FROM " & queryTable & " WHERE inv_no = @inv"
                            If queryTable = "billing" Then
                                q = "SELECT cheque_balance_due, change_amount, credit_balance_due FROM billing WHERE inv_no = @inv OR printed_inv_no = @inv"
                            End If
                            Using cmd As New MySql.Data.MySqlClient.MySqlCommand(q, connCheck)
                                cmd.Parameters.AddWithValue("@inv", invoiceNo)
                                Using reader As MySql.Data.MySqlClient.MySqlDataReader = cmd.ExecuteReader()
                                    If reader.Read() Then
                                        If Not reader.IsDBNull(0) Then Decimal.TryParse(reader(0).ToString(), chequeAmt)
                                        If reader.FieldCount > 1 AndAlso Not reader.IsDBNull(1) Then Decimal.TryParse(reader(1).ToString(), changeAmt)
                                        If reader.FieldCount > 2 AndAlso Not reader.IsDBNull(2) Then Decimal.TryParse(reader(2).ToString(), creditAmt)
                                    End If
                                End Using
                            End Using
                        End Using
                    End If

                    Dim suppressChq As Boolean = (chequeAmt <= 0)
                    Dim suppressChangeCredit As Boolean = (changeAmt = 0 AndAlso creditAmt <= 0)

                    ' Standard Invoice formatting
                    If cmbReportType.SelectedIndex = 11 Then
                        Try
                            ' We no longer suppress ReportFooterSection11 entirely to avoid hiding terms & conditions
                            ' rpt.ReportDefinition.Sections("ReportFooterSection11").SectionFormat.EnableSuppress = suppressChq
                        Catch ex As Exception
                        End Try
                        Try
                            rpt.ReportDefinition.Sections("ReportFooterSection10").SectionFormat.EnableSuppress = suppressChangeCredit
                        Catch ex As Exception
                        End Try
                    End If

                    ' POS Invoice formatting
                    If cmbReportType.SelectedIndex = 12 Then
                        Try
                            ' We no longer suppress ReportFooterSection4 entirely to avoid hiding other fields
                            ' rpt.ReportDefinition.Sections("ReportFooterSection4").SectionFormat.EnableSuppress = suppressChq
                        Catch ex As Exception
                        End Try
                        Try
                            rpt.ReportDefinition.Sections("ReportFooterSection3").SectionFormat.EnableSuppress = suppressChangeCredit
                        Catch ex As Exception
                        End Try
                    End If

                    ' Dynamic object-level suppression for Cheque Balance Due
                    Try
                        Dim rcdProp = rpt.GetType().GetProperty("ReportClientDocument")
                        If rcdProp IsNot Nothing Then
                            Dim rcd As Object = rcdProp.GetValue(rpt, Nothing)
                            If rcd IsNot Nothing Then
                                Dim objs As Object = rcd.ReportDefController.ReportObjectController.GetAllReportObjects()
                                For Each oldObj As Object In objs
                                    Dim objName As String = oldObj.Name.ToString()
                                    Dim isChequeLabel As Boolean = False
                                    If cmbReportType.SelectedIndex = 12 Then ' POS Invoice
                                        If String.Equals(objName, "Text8", StringComparison.OrdinalIgnoreCase) Then isChequeLabel = True
                                    ElseIf cmbReportType.SelectedIndex = 11 Then ' Standard Invoice
                                        If String.Equals(objName, "Text15", StringComparison.OrdinalIgnoreCase) Then isChequeLabel = True
                                    End If
                                    If String.Equals(objName, "chequebalancedue1", StringComparison.OrdinalIgnoreCase) Then isChequeLabel = True

                                    If isChequeLabel Then
                                        Dim newObj As Object = oldObj.Clone(True)
                                        newObj.Format.ConditionFormulas.RemoveAll()
                                        newObj.Format.EnableSuppress = suppressChq
                                        If suppressChq Then
                                            newObj.Height = 0
                                        Else
                                            newObj.Height = If(String.Equals(objName, "chequebalancedue1", StringComparison.OrdinalIgnoreCase), 211, 240)
                                        End If
                                        rcd.ReportDefController.ReportObjectController.Modify(oldObj, newObj)
                                    End If
                                Next
                            End If
                        End If
                    Catch ex As Exception
                    End Try
                End If
            Catch exChq As Exception
                ' Silent execution to avoid blocking the main report load
            End Try

            ' Inject required parameters (like isReturn) into the main report and subreports
            Dim activeInvoiceNo As String = If(Not String.IsNullOrEmpty(txtSearchInv.Text), txtSearchInv.Text.Trim(), _searchInv)
            Dim isRetParam As Boolean = (cmbReportType.SelectedIndex = 13)
            Dim fullCreditVal As Decimal = GetCustomerFullCredit(activeInvoiceNo)

            Try
                SetAllReportParameters(rpt, activeInvoiceNo, isRetParam, fullCreditVal)
            Catch exParams As Exception
                ' Silent execution to avoid blocking
            End Try

            Try
                CrystalReportViewer1.ReuseParameterValuesOnRefresh = True

                Dim pFields As New ParameterFields()
                ' Copy all parameters from the report document to the viewer to prevent any prompts
                For Each pd As ParameterFieldDefinition In rpt.DataDefinition.ParameterFields
                    ' Skip subreport linked parameters as they are supplied by the main report automatically
                    If pd.IsLinked() Then Continue For

                    Dim pField As New ParameterField()
                    pField.Name = pd.Name

                    ' If the report document already has the value set, copy it
                    If pd.CurrentValues.Count > 0 Then
                        For Each val As ParameterValue In pd.CurrentValues
                            pField.CurrentValues.Add(val)
                        Next
                    Else
                        ' Fallback for known parameters
                        Dim pDv As New ParameterDiscreteValue()
                        If pd.Name.Equals("isReturn", StringComparison.OrdinalIgnoreCase) Then
                            pDv.Value = isRetParam
                            pField.CurrentValues.Add(pDv)
                        ElseIf pd.Name.Equals("full_credit", StringComparison.OrdinalIgnoreCase) Then
                            pDv.Value = CDbl(fullCreditVal)
                            pField.CurrentValues.Add(pDv)
                        Else
                            ' Provide a safe dummy value matching the expected data type to prevent prompts
                            If pd.ValueType = FieldValueType.NumberField OrElse pd.ValueType = FieldValueType.CurrencyField Then
                                pDv.Value = 0
                            ElseIf pd.ValueType = FieldValueType.BooleanField Then
                                pDv.Value = False
                            ElseIf pd.ValueType = FieldValueType.DateField OrElse pd.ValueType = FieldValueType.DateTimeField OrElse pd.ValueType = FieldValueType.TimeField Then
                                pDv.Value = DateTime.Now
                            Else
                                pDv.Value = ""
                            End If
                            pField.CurrentValues.Add(pDv)
                        End If
                    End If
                    pFields.Add(pField)
                Next
                CrystalReportViewer1.ParameterFieldInfo = pFields
            Catch ex As Exception
            End Try

            ' Setting ReportSource triggers an initial render.
            CrystalReportViewer1.ReportSource = rpt

            ' Force a full database re-query so the new supplier filter is applied.
            ' Suppress the benign Crystal Reports "No error." popup.
            Try
                CrystalReportViewer1.RefreshReport()
            Catch exRefresh As Exception
                ' Ignore Crystal Reports' benign "No error." message
            End Try

            _isRefreshing = False
        Catch ex As Exception
            _isRefreshing = False
            ' Only show errors if there's actually a message and it's not the redundant 'No error.' popup
            If Not String.IsNullOrEmpty(ex.Message) AndAlso ex.Message <> "No error." Then
                ' MessageBox.Show("Error: " & ex.Message)
            End If
        End Try
    End Sub

    Private Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click
        Dim selectedPrinter As String = If(cmbPrinter.SelectedItem IsNot Nothing, cmbPrinter.SelectedItem.ToString(), "")
        DirectPrint(selectedPrinter)
    End Sub

    Public Sub DirectPrint(Optional ByVal printerName As String = "")
        Try
            Dim rpt As ReportDocument = DirectCast(CrystalReportViewer1.ReportSource, ReportDocument)
            If rpt IsNot Nothing Then
                If Not String.IsNullOrEmpty(printerName) Then
                    rpt.PrintOptions.PrinterName = printerName
                End If
                ' Print directly without standard dialog
                rpt.PrintToPrinter(1, False, 0, 0)
            Else
                MessageBox.Show("No report loaded to print.", "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
        Catch ex As Exception
            MessageBox.Show("Printing Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub SetReportConnection(ByRef rpt As ReportDocument)
        Try
            ' Parse connection string (Format: server=...;userid=...;password=...;database=...)
            Dim connStr = Module1.ConnStr
            Dim parts = connStr.Split(";"c)
            Dim user = "", pass = ""

            For Each part In parts
                Dim kv = part.Split("="c)
                If kv.Length >= 2 Then
                    Dim key = kv(0).Trim().ToLower()
                    Dim val = String.Join("=", kv, 1, kv.Length - 1).Trim().Replace(vbCr, "").Replace(vbLf, "")
                    If key = "userid" OrElse key = "user id" OrElse key = "uid" Then user = val
                    If key = "password" OrElse key = "pwd" Then pass = val
                End If
            Next

            ' Apply to main report tables
            For Each tbl As Table In rpt.Database.Tables
                Dim tblLogOn = tbl.LogOnInfo
                ' ONLY override credentials, preserve ServerName/DatabaseName as they may be ODBC DSNs
                tblLogOn.ConnectionInfo.UserID = user
                tblLogOn.ConnectionInfo.Password = pass
                tblLogOn.ConnectionInfo.IntegratedSecurity = False
                tbl.ApplyLogOnInfo(tblLogOn)
                If TypeOf rpt Is CUSTOMER_CREDIT_BY_CITY AndAlso (tbl.Name = "customer" OrElse tbl.Name = "customer1") Then
                    tbl.Location = "customer_credit_city_view"
                End If
                If TypeOf rpt Is CUSTOMER_CREDIT_BY_CITY AndAlso (tbl.Name = "billing" OrElse tbl.Name = "billing1") Then
                    tbl.Location = "billing_credit_city_view"
                End If
            Next

            ' Apply to subreports
            For Each subRpt As ReportDocument In rpt.Subreports
                For Each tbl As Table In subRpt.Database.Tables
                    Dim tblLogOn = tbl.LogOnInfo
                    tblLogOn.ConnectionInfo.UserID = user
                    tblLogOn.ConnectionInfo.Password = pass
                    tblLogOn.ConnectionInfo.IntegratedSecurity = False
                    tbl.ApplyLogOnInfo(tblLogOn)
                    If TypeOf rpt Is CUSTOMER_CREDIT_BY_CITY AndAlso (tbl.Name = "customer" OrElse tbl.Name = "customer1") Then
                        tbl.Location = "customer_credit_city_view"
                    End If
                    If TypeOf rpt Is CUSTOMER_CREDIT_BY_CITY AndAlso (tbl.Name = "billing" OrElse tbl.Name = "billing1") Then
                        tbl.Location = "billing_credit_city_view"
                    End If
                Next
            Next

            ' Force logon using the report's existing DataSource connection info to maintain driver compatibility
            If rpt.DataSourceConnections.Count > 0 Then
                rpt.SetDatabaseLogon(user, pass, rpt.DataSourceConnections(0).ServerName, rpt.DataSourceConnections(0).DatabaseName)
            End If

            ' Format report to 3 decimal places
            Module1.FormatReportDecimals(rpt)

        Catch ex As Exception
        End Try
    End Sub

    Private Sub SetAllReportParameters(ByRef rpt As ReportDocument, ByVal invoiceNo As String, ByVal isReturn As Boolean, Optional ByVal fullCredit As Decimal = 0)
        Try
            ' 1. Set on Main Report
            SetValuesOnDocument(rpt, invoiceNo, isReturn, fullCredit)

            ' 2. Set on Subreports
            For Each subRpt As ReportDocument In rpt.Subreports
                SetValuesOnDocument(subRpt, invoiceNo, isReturn, fullCredit)
            Next
        Catch ex As Exception
            ' Silent fail
        End Try
    End Sub

    Private Sub SetValuesOnDocument(ByRef doc As ReportDocument, ByVal invNo As String, ByVal isRet As Boolean, Optional ByVal fullCredit As Decimal = 0)
        Try
            ' 1. Exhaustive attempts to set "isReturn"
            Dim hasIsReturn As Boolean = False
            Try
                For Each pd As ParameterFieldDefinition In doc.DataDefinition.ParameterFields
                    If pd.Name.Equals("isReturn", StringComparison.OrdinalIgnoreCase) Then
                        hasIsReturn = True
                        Exit For
                    End If
                Next
            Catch : End Try

            If hasIsReturn Then
                Try
                    ' Method A: Direct set as Boolean
                    doc.SetParameterValue("isReturn", isRet)
                Catch : End Try
            End If

            ' 1.5 Set full_credit parameter (outstanding balance for the invoice customer)
            Try
                Dim hasFullCredit As Boolean = False
                For Each pd As ParameterFieldDefinition In doc.DataDefinition.ParameterFields
                    If pd.Name.Equals("full_credit", StringComparison.OrdinalIgnoreCase) Then
                        hasFullCredit = True
                        Exit For
                    End If
                Next
                If hasFullCredit Then
                    ' Set only as Double
                    Try
                        doc.SetParameterValue("full_credit", CDbl(fullCredit))
                    Catch : End Try

                    ' Also apply via ParameterDiscreteValue for robustness
                    For Each pd As ParameterFieldDefinition In doc.DataDefinition.ParameterFields
                        If pd.Name.Equals("full_credit", StringComparison.OrdinalIgnoreCase) Then
                            Dim vals As New ParameterValues()
                            Dim dv As New ParameterDiscreteValue()
                            dv.Value = CDbl(fullCredit)
                            vals.Add(dv)
                            pd.ApplyCurrentValues(vals)
                            Exit For
                        End If
                    Next
                End If
            Catch : End Try

            ' Method D: Using ParameterDiscreteValue (The most robust low-level method)
            Try
                For Each pd As ParameterFieldDefinition In doc.DataDefinition.ParameterFields
                    If pd.Name.Equals("isReturn", StringComparison.OrdinalIgnoreCase) Then
                        Dim vals As New ParameterValues()
                        Dim dv As New ParameterDiscreteValue()
                        dv.Value = isRet
                        vals.Add(dv)
                        pd.ApplyCurrentValues(vals)
                        Exit For
                    End If
                Next
            Catch : End Try

            ' 2. Handle other common parameters (Invoice No, etc.)
            For Each pd As ParameterFieldDefinition In doc.DataDefinition.ParameterFields
                If pd.IsLinked() Then Continue For
                
                Dim pName As String = pd.Name.ToLower()
                If pName.Contains("inv") OrElse pName.Contains("bill") OrElse pName.Contains("id") Then
                    Dim safeInvNo As String = If(String.IsNullOrEmpty(invNo), "", invNo)
                    Try
                        doc.SetParameterValue(pd.Name, safeInvNo)
                    Catch : End Try

                    ' Also try Method D for invoice/id parameters
                    Try
                        Dim vals As New ParameterValues()
                        Dim dv As New ParameterDiscreteValue()
                        dv.Value = safeInvNo
                        vals.Add(dv)
                        pd.ApplyCurrentValues(vals)
                    Catch : End Try
                End If
            Next
        Catch : End Try
    End Sub

    Private Function IsCustomerName(ByVal name As String) As Boolean
        If String.IsNullOrEmpty(name) Then Return False
        Try
            Using localConn As New MySqlConnection(Module1.ConnStr)
                localConn.Open()
                Using cmd As New MySqlCommand("SELECT COUNT(*) FROM customer WHERE name = @name", localConn)
                    cmd.Parameters.AddWithValue("@name", name)
                    Return Convert.ToInt32(cmd.ExecuteScalar()) > 0
                End Using
            End Using
        Catch
            Return False
        End Try
    End Function

    Private Function IsSupplierName(ByVal name As String) As Boolean
        If String.IsNullOrEmpty(name) Then Return False
        Try
            Using localConn As New MySqlConnection(Module1.ConnStr)
                localConn.Open()
                Using cmd As New MySqlCommand("SELECT COUNT(*) FROM supplier WHERE name = @name", localConn)
                    cmd.Parameters.AddWithValue("@name", name)
                    Return Convert.ToInt32(cmd.ExecuteScalar()) > 0
                End Using
            End Using
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Searches the report database tables for a field by name and returns its fully qualified name {Table.Field}.
    ''' </summary>
    Private Function GetReportField(rpt As ReportDocument, fieldName As String) As String
        Try
            For Each tbl As Table In rpt.Database.Tables
                For Each fld As DatabaseFieldDefinition In tbl.Fields
                    If fld.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase) Then
                        Return "{" & tbl.Name & "." & fld.Name & "}"
                    End If
                Next
            Next
        Catch : End Try
        Return ""
    End Function

    Private Function GetCustomerFullCredit(ByVal invoiceNo As String) As Decimal
        Dim fullCredit As Decimal = 0
        Try
            If String.IsNullOrEmpty(invoiceNo) Then Return 0

            Dim customerId As String = ""
            Dim queryTable As String = "billing"
            If invoiceNo.StartsWith("QT") Then
                queryTable = "quotation_billing"
            End If

            Using conn As New MySql.Data.MySqlClient.MySqlConnection(Module1.ConnStr)
                conn.Open()

                Dim qCus As String = "SELECT customer_id FROM " & queryTable & " WHERE inv_no = @inv"
                If queryTable = "billing" Then
                    qCus = "SELECT customer_id FROM billing WHERE inv_no = @inv OR printed_inv_no = @inv"
                End If
                Using cmdCus As New MySql.Data.MySqlClient.MySqlCommand(qCus, conn)
                    cmdCus.Parameters.AddWithValue("@inv", invoiceNo)
                    Dim resCus = cmdCus.ExecuteScalar()
                    If resCus IsNot Nothing AndAlso resCus IsNot DBNull.Value Then
                        customerId = resCus.ToString()
                    End If
                End Using

                If Not String.IsNullOrEmpty(customerId) AndAlso customerId <> "1" Then
                    Dim qCredit As String = "SELECT SUM(balance_due) FROM billing WHERE customer_id = @c_id"
                    Dim finRole As String = If(Module1.FinancialRole IsNot Nothing, Module1.FinancialRole.ToLower(), "")
                    Dim canAccessGR As Boolean = (Module1.UserRole.ToLower() = "cashier" AndAlso finRole = "seller") AndAlso Module1.IsRgrVisible
                    If Not Module1.IsRgrVisible OrElse finRole = "seller" Then
                        If canAccessGR Then
                            qCredit &= " AND is_rgr = 0 AND inv_no NOT LIKE 'RGR%'"
                        Else
                            qCredit &= " AND is_rgr = 0 AND inv_no NOT LIKE 'GR%' AND inv_no NOT LIKE 'RGR%' AND inv_no NOT LIKE 'gr%'"
                        End If
                    End If
                    Using cmdCredit As New MySql.Data.MySqlClient.MySqlCommand(qCredit, conn)
                        cmdCredit.Parameters.AddWithValue("@c_id", customerId)
                        Dim resCredit = cmdCredit.ExecuteScalar()
                        If resCredit IsNot Nothing AndAlso resCredit IsNot DBNull.Value Then
                            Decimal.TryParse(resCredit.ToString(), fullCredit)
                        End If
                    End Using
                End If
            End Using
        Catch ex As Exception
            ' Silent fail
        End Try
        Return fullCredit
    End Function

    Private Sub SalesHistoryForm_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            btnShow.PerformClick()
        End If
    End Sub

    Private Sub chkAllDates_CheckedChanged(sender As Object, e As EventArgs) Handles chkAllDates.CheckedChanged
        UpdateSearchUI()
        If Not _isInitializing Then LoadReportData()
    End Sub

End Class
