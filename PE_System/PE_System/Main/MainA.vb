Imports MySql.Data.MySqlClient

Public Class MainA


    Private Sub Main_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.KeyPreview = True
        HideControls()
    End Sub

    Private Sub HideControls()
        Label2.Visible = False
        PictureBox1.Visible = False
        Label3.Visible = False
        PictureBox2.Visible = False
        Label4.Visible = False
        PictureBox3.Visible = False
        Label9.Visible = False
        PictureBox7.Visible = False
        Label10.Visible = False
        PictureBox9.Visible = False
    End Sub

    Private Sub Main_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.Control AndAlso e.KeyCode = Keys.S Then
            Label2.Visible = True
            PictureBox1.Visible = True
            Label3.Visible = True
            PictureBox2.Visible = True
            Label4.Visible = True
            PictureBox3.Visible = True
            Label9.Visible = True
            PictureBox7.Visible = True
            Label10.Visible = True
            PictureBox9.Visible = True
        End If
    End Sub

    Private Sub PictureBox9_Click(sender As Object, e As EventArgs) Handles PictureBox9.Click
        OpenOwnerControl()
    End Sub

    Private Sub Label10_Click(sender As Object, e As EventArgs) Handles Label10.Click
        OpenOwnerControl()
    End Sub

    Private Sub OpenOwnerControl()
        Dim password As String = InputBox("Enter Owner Password:", "Authentication Required")
        If password = "123" Then
            Dim logForm As New logs()
            logForm.Show()
        ElseIf password <> "" Then
            MessageBox.Show("Invalid Password!", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub PictureBox7_Click(sender As Object, e As EventArgs) Handles PictureBox7.Click
        OpenSalesReport()
    End Sub

    Private Sub Label9_Click(sender As Object, e As EventArgs) Handles Label9.Click
        OpenSalesReport()
    End Sub

    Private Sub OpenSalesReport()
        Dim salesRepForm As New SalesHistoryForm()
        salesRepForm.Show()
    End Sub

    Private Sub lblExit_Click(sender As Object, e As EventArgs) Handles lblExit.Click
        Application.Exit()
    End Sub

    Private Sub lblExit_MouseEnter(sender As Object, e As EventArgs) Handles lblExit.MouseEnter
        lblExit.ForeColor = Color.Red
    End Sub

    Private Sub lblExit_MouseLeave(sender As Object, e As EventArgs) Handles lblExit.MouseLeave
        lblExit.ForeColor = Color.White
    End Sub

End Class
