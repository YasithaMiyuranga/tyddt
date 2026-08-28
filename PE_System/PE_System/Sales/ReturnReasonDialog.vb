Imports System.Windows.Forms

Public Class ReturnReasonDialog

    Public Property ReturnReason As String
    Public Property AddToStock As Boolean

    Private Sub btnActionReturn_Click(sender As Object, e As EventArgs) Handles btnActionReturn.Click
        If String.IsNullOrWhiteSpace(txtReason.Text) Then
            MessageBox.Show("The return reason cannot be empty.", "Reason Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Me.DialogResult = DialogResult.None
            Return
        End If
        Me.ReturnReason = txtReason.Text.Trim()
        Me.AddToStock = chkStock.Checked
    End Sub

End Class
