Public Class ChequeEntryDialog
    Public Property ChequeNo As String = ""
    Public Property BankID As Integer = 0
    Public Property ChequeDate As DateTime = DateTime.Now
    Public Property ChequeAmount As Decimal = 0
    Public Property DefaultAmount As Decimal = 0
    Public Property LockAmount As Boolean = False
    Public Property InitialChequeNo As String = ""
    Public Property InitialBankID As Integer = 0
    Public Property InitialDate As DateTime = DateTime.MinValue
    Public Property InitialAmount As Decimal = 0

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        If String.IsNullOrWhiteSpace(txtChequeNo.Text) Then
            MessageBox.Show("Please enter Cheque No.", "Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtChequeNo.Focus()
            Return
        End If

        If cmbBank.SelectedIndex = -1 Then
            MessageBox.Show("Please select a Bank.", "Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            cmbBank.Focus()
            cmbBank.DroppedDown = True
            Return
        End If

        Dim amt As Decimal = 0
        If Not Decimal.TryParse(txtAmount.Text, amt) OrElse amt <= 0 Then
            MessageBox.Show("Please enter a valid Cheque Amount.", "Invalid Amount", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtAmount.Focus()
            Return
        End If

        If amt > DefaultAmount AndAlso DefaultAmount > 0 Then
            If Not LockAmount Then
                Dim res = MessageBox.Show("Cheque amount exceeds the remaining balance (" & DefaultAmount.ToString("N2") & "). Do you want to proceed anyway?", "Confirm Overpayment", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                If res <> DialogResult.Yes Then Return
            End If
            ' If LockAmount is true, we allow it without warning because it was set programmatically (likely a return)
        End If

        ChequeNo = txtChequeNo.Text.Trim()
        BankID = Convert.ToInt32(cmbBank.SelectedValue)
        ChequeDate = dtpDate.Value
        ChequeAmount = amt

        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub cmbBank_Enter(sender As Object, e As EventArgs) Handles cmbBank.Enter
        cmbBank.DroppedDown = True
    End Sub

    Private Sub ChequeEntryDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadBanks()
        If DefaultAmount > 0 Then
            txtAmount.Text = DefaultAmount.ToString("F2")
        End If
        If LockAmount Then
            txtAmount.Enabled = False
            txtAmount.BackColor = SystemColors.Control
        End If

        ' Auto-fill existing details if provided
        If Not String.IsNullOrEmpty(InitialChequeNo) Then
            txtChequeNo.Text = InitialChequeNo
        End If
        If InitialBankID > 0 Then
            cmbBank.SelectedValue = InitialBankID
        End If
        If InitialDate <> DateTime.MinValue Then
            dtpDate.Value = InitialDate
        End If
        If InitialAmount > 0 Then
            txtAmount.Text = InitialAmount.ToString("F2")
        End If
    End Sub

    Private Sub LoadBanks()
        Try
            ' Need to reference the global connection string
            Using conn As New MySql.Data.MySqlClient.MySqlConnection(Module1.ConnStr)
                conn.Open()
                Dim da As New MySql.Data.MySqlClient.MySqlDataAdapter("SELECT id, bank_name FROM bank ORDER BY bank_name", conn)
                Dim dt As New DataTable()
                da.Fill(dt)
                cmbBank.DataSource = dt
                cmbBank.DisplayMember = "bank_name"
                cmbBank.ValueMember = "id"
                cmbBank.SelectedIndex = -1
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading banks: " & ex.Message)
        End Try
    End Sub

    Private Sub ChequeEntryDialog_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            If Me.ActiveControl Is txtAmount OrElse Me.ActiveControl Is btnSave Then
                btnSave_Click(sender, e)
            Else
                Me.SelectNextControl(Me.ActiveControl, True, True, True, True)
            End If
        ElseIf e.KeyCode = Keys.Escape Then
            Me.DialogResult = DialogResult.Cancel
            Me.Close()
        End If
    End Sub
End Class
