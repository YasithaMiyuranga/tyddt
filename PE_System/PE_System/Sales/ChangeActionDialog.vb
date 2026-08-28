Imports System.Windows.Forms
Imports System.Drawing

Public Class ChangeActionDialog

    Public Property SelectedAction As ChangeAction
    Public Property ChangeAmount As Decimal

    Public Enum ChangeAction
        CashReturn
        AddToAdvance
        SettlePreviousCredit
        Cancel
    End Enum

    Public Sub New(amount As Decimal, hasPreviousCredit As Boolean)
        InitializeComponent()

        Me.ChangeAmount = amount
        Me.SelectedAction = ChangeAction.Cancel

        lblTitle.Text = "Overpayment / Change Amount: Rs. " & amount.ToString("N2")

        If Not hasPreviousCredit Then
            rbSettle.Enabled = False
            rbSettle.Text &= " (No Credit Available)"
        End If
    End Sub

    Private Sub btnOk_Click(sender As Object, e As EventArgs) Handles btnOk.Click
        If rbCash.Checked Then
            SelectedAction = ChangeAction.CashReturn
        ElseIf rbAdvance.Checked Then
            SelectedAction = ChangeAction.AddToAdvance
        ElseIf rbSettle.Checked Then
            SelectedAction = ChangeAction.SettlePreviousCredit
        End If
    End Sub


End Class
