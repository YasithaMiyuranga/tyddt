Imports System.Windows.Forms
Imports System.Drawing
Imports MySql.Data.MySqlClient

Public Class ItemInvoicesForm
    Inherits Form

    Private _itemId As String
    Private _itemName As String
    Private _startDate As Date
    Private _endDate As Date
    Private _filterInvType As String
    Private _filterBillingType As String
    Private _isAdvance As Boolean
    Private _allItem As Boolean

    ' UI Controls
    Private pnlHeader As Panel
    Private lblTitle As Label
    Private dtpStart As DateTimePicker
    Private dtpEnd As DateTimePicker
    Private cmbBillingType As ComboBox
    Private cmbInvType As ComboBox
    Private btnFilter As Button
    Private dgvInvoices As DataGridView
    Private pnlFooter As Panel
    Private lblInstruction As Label
    Private btnClose As Button

    Public Sub New(itemId As String, itemName As String, startDate As Date, endDate As Date, filterInvType As String, filterBillingType As String, isAdvance As Boolean, allItem As Boolean)
        MyBase.New()
        _itemId = itemId
        _itemName = itemName
        _startDate = startDate
        _endDate = endDate
        _filterInvType = filterInvType
        _filterBillingType = filterBillingType
        _isAdvance = isAdvance
        _allItem = allItem

        InitializeControls()
    End Sub

    Private Sub InitializeControls()
        ' Form properties
        Me.Text = "Invoices Purchasing: " & _itemName
        Me.Size = New Size(1000, 600)
        Me.StartPosition = FormStartPosition.CenterParent
        Me.MinimizeBox = False
        Me.ShowInTaskbar = False

        ' Font definitions
        Dim fontHeader As New Font("Segoe UI", 11, FontStyle.Bold)
        Dim fontLabel As New Font("Segoe UI", 10)
        Dim fontItalic As New Font("Segoe UI", 9.5, FontStyle.Italic)

        ' Header panel
        pnlHeader = New Panel()
        pnlHeader.Height = 80
        pnlHeader.Dock = DockStyle.Top
        pnlHeader.BackColor = Color.FromArgb(235, 243, 250)
        pnlHeader.Padding = New Padding(15, 10, 15, 10)

        lblTitle = New Label()
        lblTitle.Dock = DockStyle.Left
        lblTitle.Width = 280
        lblTitle.Font = fontHeader
        lblTitle.ForeColor = Color.FromArgb(41, 128, 185)
        lblTitle.TextAlign = ContentAlignment.MiddleLeft
        lblTitle.Text = String.Format("Item Code: {0}" & vbCrLf & "Description: {1}", _itemId, _itemName)
        pnlHeader.Controls.Add(lblTitle)

        ' Filter panel for date pickers and filter button
        Dim flpFilter As New FlowLayoutPanel()
        flpFilter.Width = 690
        flpFilter.Dock = DockStyle.Right
        flpFilter.FlowDirection = FlowDirection.LeftToRight
        flpFilter.WrapContents = False
        flpFilter.BackColor = Color.Transparent
        flpFilter.Padding = New Padding(0, 15, 0, 0)

        Dim lblStart As New Label()
        lblStart.Text = "From:"
        lblStart.Font = fontLabel
        lblStart.ForeColor = Color.FromArgb(44, 62, 80)
        lblStart.AutoSize = True
        lblStart.Margin = New Padding(0, 5, 0, 0)

        dtpStart = New DateTimePicker()
        dtpStart.Format = DateTimePickerFormat.Short
        dtpStart.Value = _startDate
        dtpStart.Width = 100
        dtpStart.Font = fontLabel
        dtpStart.Margin = New Padding(5, 0, 10, 0)

        Dim lblEnd As New Label()
        lblEnd.Text = "To:"
        lblEnd.Font = fontLabel
        lblEnd.ForeColor = Color.FromArgb(44, 62, 80)
        lblEnd.AutoSize = True
        lblEnd.Margin = New Padding(0, 5, 0, 0)

        dtpEnd = New DateTimePicker()
        dtpEnd.Format = DateTimePickerFormat.Short
        dtpEnd.Value = _endDate
        dtpEnd.Width = 100
        dtpEnd.Font = fontLabel
        dtpEnd.Margin = New Padding(5, 0, 10, 0)

        Dim lblBilling As New Label()
        lblBilling.Text = "Billing:"
        lblBilling.Font = fontLabel
        lblBilling.ForeColor = Color.FromArgb(44, 62, 80)
        lblBilling.AutoSize = True
        lblBilling.Margin = New Padding(0, 5, 0, 0)

        cmbBillingType = New ComboBox()
        cmbBillingType.DropDownStyle = ComboBoxStyle.DropDownList
        cmbBillingType.Width = 120
        cmbBillingType.Font = fontLabel
        cmbBillingType.Margin = New Padding(5, 0, 10, 0)

        ' Populate cmbBillingType
        Dim billingTypes As New List(Of String) From {"All", "Advance"}
        If Not Module1.IsRgrVisible Then
            billingTypes.AddRange({"Cash", "Cash (Cash)", "Cash (Cards/Online)"})
        Else
            billingTypes.AddRange({"Cash", "Cash (Cash)", "Cash (Cards/Online)", "Credit", "Cheque", "Cash+Credit", "Cash+Cheque", "Mixed Payment", "Credit+Cheque"})
        End If
        cmbBillingType.Items.AddRange(billingTypes.ToArray())

        ' Set selected item based on parameters passed to constructor
        If _allItem Then
            cmbBillingType.SelectedItem = "All"
        ElseIf _isAdvance Then
            cmbBillingType.SelectedItem = "Advance"
        Else
            If cmbBillingType.Items.Contains(_filterBillingType) Then
                cmbBillingType.SelectedItem = _filterBillingType
            Else
                cmbBillingType.SelectedIndex = 0
            End If
        End If

        Dim lblInvType As New Label()
        lblInvType.Text = "Type:"
        lblInvType.Font = fontLabel
        lblInvType.ForeColor = Color.FromArgb(44, 62, 80)
        lblInvType.AutoSize = True
        lblInvType.Margin = New Padding(0, 5, 0, 0)

        cmbInvType = New ComboBox()
        cmbInvType.DropDownStyle = ComboBoxStyle.DropDownList
        cmbInvType.Width = 85
        cmbInvType.Font = fontLabel
        cmbInvType.Margin = New Padding(5, 0, 10, 0)

        ' Populate cmbInvType
        Dim invTypes As String() = {"All", "Normal", "Wholesale", "Retail"}
        cmbInvType.Items.AddRange(invTypes)

        ' Set selected item based on parameters passed to constructor
        If _allItem Then
            cmbInvType.SelectedItem = "All"
        Else
            If cmbInvType.Items.Contains(_filterInvType) Then
                cmbInvType.SelectedItem = _filterInvType
            Else
                cmbInvType.SelectedIndex = 0
            End If
        End If

        btnFilter = New Button()
        btnFilter.Text = "Filter"
        btnFilter.Width = 70
        btnFilter.Height = 28
        btnFilter.Font = New Font("Segoe UI", 9.5, FontStyle.Bold)
        btnFilter.BackColor = Color.FromArgb(41, 128, 185)
        btnFilter.ForeColor = Color.White
        btnFilter.FlatStyle = FlatStyle.Flat
        btnFilter.FlatAppearance.BorderSize = 0
        btnFilter.Margin = New Padding(0, -2, 0, 0)
        AddHandler btnFilter.Click, AddressOf BtnFilter_Click

        flpFilter.Controls.Add(lblStart)
        flpFilter.Controls.Add(dtpStart)
        flpFilter.Controls.Add(lblEnd)
        flpFilter.Controls.Add(dtpEnd)
        flpFilter.Controls.Add(lblBilling)
        flpFilter.Controls.Add(cmbBillingType)
        flpFilter.Controls.Add(lblInvType)
        flpFilter.Controls.Add(cmbInvType)
        flpFilter.Controls.Add(btnFilter)

        pnlHeader.Controls.Add(flpFilter)

        ' Footer panel
        pnlFooter = New Panel()
        pnlFooter.Height = 50
        pnlFooter.Dock = DockStyle.Bottom
        pnlFooter.BackColor = SystemColors.Control
        pnlFooter.Padding = New Padding(15, 10, 15, 10)

        lblInstruction = New Label()
        lblInstruction.Dock = DockStyle.Left
        lblInstruction.AutoSize = True
        lblInstruction.Font = fontItalic
        lblInstruction.ForeColor = Color.DimGray
        lblInstruction.TextAlign = ContentAlignment.MiddleLeft
        lblInstruction.Text = "💡 Double-click any row to view or print the corresponding invoice details."

        btnClose = New Button()
        btnClose.Dock = DockStyle.Right
        btnClose.Width = 100
        btnClose.Text = "Close"
        btnClose.Font = fontLabel
        AddHandler btnClose.Click, AddressOf BtnClose_Click

        pnlFooter.Controls.Add(lblInstruction)
        pnlFooter.Controls.Add(btnClose)

        ' DataGridView
        dgvInvoices = New DataGridView()
        dgvInvoices.Dock = DockStyle.Fill
        dgvInvoices.ReadOnly = True
        dgvInvoices.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvInvoices.AllowUserToAddRows = False
        dgvInvoices.RowHeadersVisible = False
        dgvInvoices.BackgroundColor = SystemColors.Window
        dgvInvoices.DefaultCellStyle.Font = New Font("Microsoft Sans Serif", 12)
        dgvInvoices.ColumnHeadersDefaultCellStyle.Font = New Font("Microsoft Sans Serif", 12, FontStyle.Bold)
        dgvInvoices.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250)
        AddHandler dgvInvoices.CellDoubleClick, AddressOf DgvInvoices_CellDoubleClick

        ' Add controls to Form
        Me.Controls.Add(dgvInvoices)
        Me.Controls.Add(pnlHeader)
        Me.Controls.Add(pnlFooter)
    End Sub

    Private Sub ItemInvoicesForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadData()
    End Sub

    Private Sub LoadData()
        Try
            If Module1.conn.State = ConnectionState.Closed Then Module1.conn.Open()

            ' Base Query joining billing, billing_item and customer
            Dim sql As String = "SELECT b.inv_no as 'Invoice No', DATE(b.timestamps) as 'Date', " &
                                "b.billing_type as 'Billing Type', bi.quantity as 'Qty', " &
                                "bi.unit_price as 'Unit Price', bi.discount as 'Disc %', " &
                                "((bi.unit_price - (bi.unit_price * bi.discount / 100)) * bi.quantity) as 'Total Price', " &
                                "COALESCE(c.name, 'Walk-in') as 'Customer Name' " &
                                "FROM billing_item bi " &
                                "JOIN billing b ON bi.billing_id = b.id " &
                                "LEFT JOIN customer c ON b.customer_id = c.id " &
                                "WHERE bi.item_id = @itemId AND LOWER(TRIM(b.status)) IN ('paid', 'success', 'completed', 'advance', 'credit', 'partial_credit', 'cheque', 'partial_cheque', 'cash_credit', 'cash_cheque', 'mixed_payment', 'credit_cheque') " &
                                (If(Module1.IsRgrVisible, "", " AND b.is_rgr = 0 AND b.inv_no NOT LIKE 'GR%' AND b.inv_no NOT LIKE 'RGR%' "))

            ' Apply Date filters
            Dim startStr As String = _startDate.ToString("yyyy-MM-dd") & " 00:00:00"
            Dim endStr As String = _endDate.ToString("yyyy-MM-dd") & " 23:59:59"
            sql &= " AND b.timestamps >= @start AND b.timestamps <= @end "

            ' Apply Invoice Type filter from ComboBox
            Dim selectedInvType As String = "All"
            If cmbInvType IsNot Nothing AndAlso cmbInvType.SelectedItem IsNot Nothing Then
                selectedInvType = cmbInvType.SelectedItem.ToString()
            End If

            If selectedInvType <> "All" Then
                sql &= " AND b.inv_type = @invType "
            End If

            ' Apply Billing Type filter from ComboBox
            Dim selectedBilling As String = "All"
            If cmbBillingType IsNot Nothing AndAlso cmbBillingType.SelectedItem IsNot Nothing Then
                selectedBilling = cmbBillingType.SelectedItem.ToString()
            End If

            If selectedBilling <> "All" Then
                If selectedBilling = "Advance" Then
                    sql &= " AND b.advance_payment != 0 "
                Else
                    Select Case selectedBilling
                        Case "Cash"
                            sql &= " AND LOWER(TRIM(b.status)) = 'paid' "
                        Case "Cash (Cash)"
                            sql &= " AND LOWER(TRIM(b.status)) = 'paid' AND LOWER(TRIM(b.payment_type)) = 'cash' "
                        Case "Cash (Cards/Online)"
                            sql &= " AND LOWER(TRIM(b.status)) = 'paid' AND LOWER(TRIM(b.payment_type)) IN ('credit card', 'debit card', 'online transfer') "
                        Case "Credit"
                            sql &= " AND LOWER(TRIM(b.status)) IN ('credit', 'partial_credit') "
                        Case "Cheque"
                            sql &= " AND LOWER(TRIM(b.status)) IN ('cheque', 'partial_cheque' ) "
                        Case "Cash+Credit"
                            sql &= " AND LOWER(TRIM(b.status)) = 'cash_credit' "
                        Case "Cash+Cheque"
                            sql &= " AND LOWER(TRIM(b.status)) = 'cash_cheque' "
                        Case "Mixed Payment"
                            sql &= " AND LOWER(TRIM(b.status)) = 'mixed_payment' "
                        Case "Credit+Cheque"
                            sql &= " AND LOWER(TRIM(b.status)) = 'credit_cheque' "
                        Case Else
                            sql &= " AND b.billing_type = @bType "
                    End Select
                End If
            End If

            sql &= " ORDER BY b.timestamps DESC"

            Dim cmd As New MySqlCommand(sql, Module1.conn)
            cmd.Parameters.AddWithValue("@itemId", _itemId)
            cmd.Parameters.AddWithValue("@start", startStr)
            cmd.Parameters.AddWithValue("@end", endStr)
            
            If selectedInvType <> "All" Then
                cmd.Parameters.AddWithValue("@invType", selectedInvType)
            End If
            
            If selectedBilling <> "All" AndAlso selectedBilling <> "Advance" Then
                Dim knownTypes As String() = {"Cash", "Cash (Cash)", "Cash (Cards/Online)", "Credit", "Cheque", "Cash+Credit", "Cash+Cheque", "Mixed Payment", "Credit+Cheque"}
                If Not knownTypes.Contains(selectedBilling) Then
                    cmd.Parameters.AddWithValue("@bType", selectedBilling)
                End If
            End If

            Dim da As New MySqlDataAdapter(cmd)
            Dim dt As New DataTable()
            da.Fill(dt)

            dgvInvoices.DataSource = dt

            ' Format Columns
            If dgvInvoices.Columns.Count > 0 Then
                If dgvInvoices.Columns.Contains("Invoice No") Then dgvInvoices.Columns("Invoice No").Width = 140
                If dgvInvoices.Columns.Contains("Date") Then dgvInvoices.Columns("Date").Width = 130
                If dgvInvoices.Columns.Contains("Billing Type") Then dgvInvoices.Columns("Billing Type").Width = 120
                If dgvInvoices.Columns.Contains("Qty") Then
                    dgvInvoices.Columns("Qty").Width = 80
                    dgvInvoices.Columns("Qty").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                End If
                If dgvInvoices.Columns.Contains("Unit Price") Then
                    dgvInvoices.Columns("Unit Price").Width = 120
                    dgvInvoices.Columns("Unit Price").DefaultCellStyle.Format = "N2"
                    dgvInvoices.Columns("Unit Price").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                End If
                If dgvInvoices.Columns.Contains("Disc %") Then
                    dgvInvoices.Columns("Disc %").Width = 90
                    dgvInvoices.Columns("Disc %").DefaultCellStyle.Format = "N2"
                    dgvInvoices.Columns("Disc %").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                End If
                If dgvInvoices.Columns.Contains("Total Price") Then
                    dgvInvoices.Columns("Total Price").Width = 140
                    dgvInvoices.Columns("Total Price").DefaultCellStyle.Format = "N2"
                    dgvInvoices.Columns("Total Price").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                End If
                If dgvInvoices.Columns.Contains("Customer Name") Then dgvInvoices.Columns("Customer Name").Width = 200
            End If

            Module1.conn.Close()
        Catch ex As Exception
            MessageBox.Show("Error loading invoices: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            If Module1.conn.State = ConnectionState.Open Then Module1.conn.Close()
        End Try
    End Sub

    Private Sub DgvInvoices_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex >= 0 Then
            Dim invNo As String = dgvInvoices.Rows(e.RowIndex).Cells("Invoice No").Value.ToString()
            Try
                Dim rptForm As New SaleInv()
                rptForm.MdiParent = Me.MdiParent
                rptForm.Show()
                rptForm.ShowReport(invNo, 0, False, False, "", 1, 0, True)
            Catch ex As Exception
                MessageBox.Show("Error opening invoice details: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub BtnFilter_Click(sender As Object, e As EventArgs)
        _startDate = dtpStart.Value.Date
        _endDate = dtpEnd.Value.Date
        LoadData()
    End Sub

    Private Sub BtnClose_Click(sender As Object, e As EventArgs)
        Me.Close()
    End Sub
End Class
