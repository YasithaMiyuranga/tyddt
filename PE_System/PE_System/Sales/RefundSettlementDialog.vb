Imports System.Windows.Forms

Partial Public Class RefundSettlementDialog
    
    Public Property TotalRefundDue As Decimal = 0
    Public Property UnpaidCredit As Decimal = 0
    Public Property UnclearedChequeAmount As Decimal = 0
    
    Public Property ApplyToCredit As Decimal = 0
    Public Property ApplyToStoreCredit As Decimal = 0
    Public Property ApplyToCash As Decimal = 0

    Private Sub RefundSettlementDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        lblRefundDue.Text = TotalRefundDue.ToString("N2")
        
        ' Waterfall Calculation
        Dim remaining As Decimal = TotalRefundDue
        
        ' 1. Credit Deduction
        ApplyToCredit = Math.Min(remaining, UnpaidCredit)
        remaining -= ApplyToCredit
        
        ' 2. Uncleared Cheques -> Store Credit Note
        ApplyToStoreCredit = Math.Min(remaining, UnclearedChequeAmount)
        remaining -= ApplyToStoreCredit
        
        ' 3. Remainder -> Cash Refund
        ApplyToCash = remaining
        
        ' Display formatting
        txtCreditDeduction.Text = ApplyToCredit.ToString("N2")
        txtStoreCredit.Text = ApplyToStoreCredit.ToString("N2")
        txtCashRefund.Text = ApplyToCash.ToString("N2")
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
