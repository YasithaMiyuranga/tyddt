Imports System.Windows.Forms
Imports System.Drawing

Public Class SupplierRefundSettlementDialog
    Public Property TotalRefundDue As Decimal = 0
    Public Property UnpaidCredit As Decimal = 0

    Public Property ApplyToCredit As Decimal = 0
    Public Property ApplyToSupplierCreditNote As Decimal = 0
    Public Property ApplyToCash As Decimal = 0

    Private isCalculating As Boolean = False

    Private Sub SupplierRefundSettlementDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        lblRefundDue.Text = TotalRefundDue.ToString("N2")

        ' Waterfall Calculation
        Dim remaining As Decimal = TotalRefundDue

        ' 1. Credit Deduction (What we haven't paid them yet)
        ApplyToCredit = Math.Min(remaining, UnpaidCredit)
        remaining -= ApplyToCredit

        ' 2. Assuming no cheques specific tracking yet for purchase returns, 
        ' the remainder was physically paid, so they either gave cash back or kept it as a voucher.
        ' We default to standard behaviour: if we paid cash, assume they gave it back unless told otherwise.
        ApplyToCash = remaining
        ApplyToSupplierCreditNote = 0 ' Default is Cash, user can type into Store Credit if needed.
        
        isCalculating = True
        txtCredit.Text = ApplyToCredit.ToString("N2")
        txtStoreCredit.Text = ApplyToSupplierCreditNote.ToString("N2")
        txtCash.Text = ApplyToCash.ToString("N2")
        isCalculating = False
    End Sub

    Private Sub CalculateRemaining(sender As Object, e As EventArgs) Handles txtStoreCredit.TextChanged, txtCash.TextChanged
        If isCalculating Then Return
        isCalculating = True

        Dim store_cr As Decimal = 0
        Dim cash_rf As Decimal = 0
        Decimal.TryParse(txtStoreCredit.Text, store_cr)
        Decimal.TryParse(txtCash.Text, cash_rf)

        ' Waterfall assigns to credit first
        Dim remainingAfterCredit As Decimal = TotalRefundDue - ApplyToCredit

        Dim activeTextBox As TextBox = TryCast(sender, TextBox)
        If activeTextBox Is txtStoreCredit Then
            ' Auto-adjust cash
            If store_cr > remainingAfterCredit Then
                store_cr = remainingAfterCredit
                txtStoreCredit.Text = store_cr.ToString("N2")
            End If
            cash_rf = remainingAfterCredit - store_cr
            txtCash.Text = cash_rf.ToString("N2")
        ElseIf activeTextBox Is txtCash Then
            ' Auto-adjust store credit
            If cash_rf > remainingAfterCredit Then
                cash_rf = remainingAfterCredit
                txtCash.Text = cash_rf.ToString("N2")
            End If
            store_cr = remainingAfterCredit - cash_rf
            txtStoreCredit.Text = store_cr.ToString("N2")
        End If

        ApplyToSupplierCreditNote = store_cr
        ApplyToCash = cash_rf

        isCalculating = False
    End Sub

    Private Sub btnConfirm_Click(sender As Object, e As EventArgs) Handles btnConfirm.Click
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub
End Class
