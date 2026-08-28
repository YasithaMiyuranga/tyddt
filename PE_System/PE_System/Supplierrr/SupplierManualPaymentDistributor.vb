Imports MySql.Data.MySqlClient
Imports System.Drawing

Public Class SupplierManualPaymentDistributor
    Private _supplierId As Integer
    Private _supplierName As String
    Private _totalPaidAmount As Double
    Private _remainingAmount As Double
    
    ' Property to hold the distribution: Invoice Number -> Amount applied
    Public Property Distribution As New Dictionary(Of String, Double)
    Public Property IsSuccess As Boolean = False

    Public Sub New(supplierId As Integer, name As String, amount As Double, paymentMethod As String)
        InitializeComponent()
        _supplierId = supplierId
        _supplierName = name
        _totalPaidAmount = amount
        _remainingAmount = amount
        Me.SelectedPaymentMethod = paymentMethod
        Me.KeyPreview = True ' Enable form-level key events
    End Sub

    Private Sub SupplierManualPaymentDistributor_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LabelSupplier.Text = "Supplier: " & _supplierName
        LabelTotal.Text = _totalPaidAmount.ToString("N2")
        LabelRemaining.Text = _remainingAmount.ToString("N2")
        
        LoadInvoices()
    End Sub

    Private Sub LoadInvoices()
        Try
            Dim dt As New DataTable()
            ' Include getdate to show the invoice date
            Dim sql As String = "SELECT getdate, inv_no, amount FROM supplicer_credit WHERE supplier_id = @sid AND amount > 0 ORDER BY getdate ASC"
            
            Using conn As New MySqlConnection(ConnStr)
                conn.Open()
                Using cmd As New MySqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@sid", _supplierId)
                    Using adapter As New MySqlDataAdapter(cmd)
                        adapter.Fill(dt)
                    End Using
                End Using
            End Using

            ' Calculate total outstanding for the header
            Dim totalOutstanding As Double = 0
            If dt.Rows.Count > 0 Then
                totalOutstanding = Convert.ToDouble(dt.Compute("SUM(amount)", String.Empty))
            End If
            LabelTotalDebit.Text = "Total Outstanding: " & totalOutstanding.ToString("N2")

            ' Add a column for manual entry
            dt.Columns.Add("PayNow", GetType(Double))
            For Each row As DataRow In dt.Rows
                row("PayNow") = 0.0
            Next

            DataGridView1.DataSource = dt
            
            ' Format Grid
            DataGridView1.Columns("getdate").HeaderText = "Date"
            DataGridView1.Columns("getdate").ReadOnly = True
            DataGridView1.Columns("getdate").DefaultCellStyle.Format = "yyyy-MM-dd"
            DataGridView1.Columns("getdate").Width = 120

            DataGridView1.Columns("inv_no").HeaderText = "Invoice No"
            DataGridView1.Columns("inv_no").ReadOnly = True
            DataGridView1.Columns("amount").HeaderText = "Debit Balance"
            DataGridView1.Columns("amount").ReadOnly = True
            DataGridView1.Columns("amount").DefaultCellStyle.Format = "N2"
            
            DataGridView1.Columns("PayNow").HeaderText = "Amount to Pay"
            DataGridView1.Columns("PayNow").ReadOnly = False
            DataGridView1.Columns("PayNow").DefaultCellStyle.Format = "N2"
            DataGridView1.Columns("PayNow").DefaultCellStyle.BackColor = Color.FromArgb(64, 64, 64)
            DataGridView1.Columns("PayNow").DefaultCellStyle.ForeColor = Color.White

            ' Larger Font and Styling
            DataGridView1.Font = New Font("Segoe UI", 11)
            DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(20, 25, 72)
            DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            DataGridView1.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 12, FontStyle.Bold)
            DataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(50, 50, 50)
            DataGridView1.RowsDefaultCellStyle.BackColor = Color.FromArgb(40, 40, 40)
            DataGridView1.RowsDefaultCellStyle.ForeColor = Color.White
            DataGridView1.DefaultCellStyle.SelectionBackColor = Color.SeaGreen
            DataGridView1.DefaultCellStyle.SelectionForeColor = Color.White

        Catch ex As Exception
            MessageBox.Show("Error loading invoices: " & ex.Message)
        End Try
    End Sub

    Private Sub DataGridView1_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellValueChanged
        If e.RowIndex >= 0 AndAlso DataGridView1.Columns(e.ColumnIndex).Name = "PayNow" Then
            RecalculateRemaining()
        End If
    End Sub

    Private Sub RecalculateRemaining()
        Dim totalApplied As Double = 0
        For Each row As DataGridViewRow In DataGridView1.Rows
            If Not row.IsNewRow Then
                Dim val = row.Cells("PayNow").Value
                If val IsNot Nothing AndAlso Not IsDBNull(val) Then
                    Dim applied As Double = 0
                    If Double.TryParse(val.ToString(), applied) Then
                        ' Validation: Cannot apply more than the debit balance for that invoice
                        Dim balance As Double = Convert.ToDouble(row.Cells("amount").Value)
                        If applied > balance Then
                            applied = balance
                            row.Cells("PayNow").Value = applied
                        End If
                        totalApplied += applied
                    End If
                End If
            End If
        Next

        _remainingAmount = _totalPaidAmount - totalApplied
        LabelRemaining.Text = _remainingAmount.ToString("N2")

        If _remainingAmount < 0 Then
            LabelRemaining.ForeColor = Color.DarkRed
        ElseIf _remainingAmount = 0 Then
            LabelRemaining.ForeColor = Color.DarkGreen
        Else
            LabelRemaining.ForeColor = Color.Tomato
        End If
    End Sub

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        If e.RowIndex >= 0 Then
            If DataGridView1.Columns.Contains("PayNow") Then
                DataGridView1.CurrentCell = DataGridView1.Rows(e.RowIndex).Cells("PayNow")
                DataGridView1.BeginEdit(True)
            End If
        End If
    End Sub

    Private Sub DataGridView1_KeyDown(sender As Object, e As KeyEventArgs) Handles DataGridView1.KeyDown
        If e.KeyCode = Keys.Enter Then
            ' If balance is already zero, Enter completes the action
            If Math.Round(_remainingAmount, 2) = 0 Then
                CompleteDistribution()
                e.Handled = True
                Return
            End If

            If DataGridView1.CurrentRow IsNot Nothing Then
                Dim idx As Integer = DataGridView1.CurrentRow.Index
                If DataGridView1.Columns.Contains("PayNow") Then
                    DataGridView1.CurrentCell = DataGridView1.Rows(idx).Cells("PayNow")
                    DataGridView1.BeginEdit(True)
                    e.Handled = True
                End If
            End If
        End If
    End Sub

    Private Sub SupplierManualPaymentDistributor_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        ' F5 to Complete
        If e.KeyCode = Keys.F5 Then
            CompleteDistribution()
            e.Handled = True
        End If
    End Sub

    Public Property SelectedPaymentMethod As String = "Cash"

    Private Sub BtnOk_Click(sender As Object, e As EventArgs) Handles BtnOk.Click
        CompleteDistribution()
    End Sub

    Private Sub CompleteDistribution()
        Dim remaining = Math.Round(_remainingAmount, 2)

        If remaining > 0 Then
            MessageBox.Show("Please allocate the full payment amount. Remaining: " & remaining.ToString("N2"))
            Return
        End If

        If remaining < 0 Then
            MessageBox.Show("Total allocated amount exceeds the paid amount. Please adjust.")
            Return
        End If

        ' Confirm if it's a cheque to prompt for details later
        Dim pType = Me.SelectedPaymentMethod.Trim().ToLower()
        Dim isCheque As Boolean = (pType = "chaque" OrElse pType = "cheque")

        If isCheque Then
            MessageBox.Show("Please enter the Cheque number and Bank details in the next screen.", "Cheque Details Required", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

        Distribution.Clear()
        For Each row As DataGridViewRow In DataGridView1.Rows
            If Not row.IsNewRow Then
                Dim applied As Double = 0
                If Double.TryParse(row.Cells("PayNow").Value?.ToString(), applied) AndAlso applied > 0 Then
                    Distribution.Add(row.Cells("inv_no").Value.ToString(), applied)
                End If
            End If
        Next

        If Distribution.Count > 0 Then
            Me.IsSuccess = True
            Me.Close()
        Else
            MessageBox.Show("No payments allocated.")
        End If
    End Sub

    Private Sub LabelTotal_Click(sender As Object, e As EventArgs) Handles LabelTotal.Click

    End Sub
End Class
