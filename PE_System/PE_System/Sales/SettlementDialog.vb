Public Class SettlementDialog
    Public Property SelectedSettlement As String = ""

    Private Sub btnCredit_Click(sender As Object, e As EventArgs) Handles btnCredit.Click
        SelectedSettlement = "Credit"
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub btnCheque_Click(sender As Object, e As EventArgs) Handles btnCheque.Click
        SelectedSettlement = "Cheque"
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub SettlementDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Premium styling would go here or in Designer
    End Sub

    Private Sub SettlementDialog_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            btnCredit_Click(sender, e)
        ElseIf e.KeyCode = Keys.ShiftKey Then
            e.SuppressKeyPress = True
            btnCheque_Click(sender, e)
        End If
    End Sub
End Class
