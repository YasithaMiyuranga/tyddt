Imports MySql.Data.MySqlClient

Public Class PettyCashAdd
    Private CurrentUpdateID As Integer = 0
    Private denominations() As Integer = {5000, 2000, 1000, 500, 100, 50, 20, 10, 5, 2, 1}
    Private denomInputs As New Dictionary(Of Integer, TextBox)

    Private Sub PettyCashAdd_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.KeyPreview = True
        LoadBanks()
        SetupDenominationControls()
        If CurrentUpdateID > 0 Then Return ' Prevent overwriting data set by LoadForUpdate
        
        cmbItemType.SelectedIndex = 0
        cmbTransactionType.SelectedIndex = 0 ' Default to Cash OUT
        cmbSource.SelectedIndex = 0
        dtpDate.Value = DateTime.Now
        txtItemName.Text = ""
        txtAmount.Text = ""
        txtReceiptNo.Text = ""
        txtBranch.Text = ""
    End Sub

    Private Sub LoadBanks()
        Try
            db_connection()
            If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()
            
            Dim dt As New DataTable()
            Dim cmd As New MySqlCommand("SELECT id, bank_name FROM bank ORDER BY bank_name ASC", MySqlConn)
            Dim da As New MySqlDataAdapter(cmd)
            da.Fill(dt)
            
            cmbBank.DataSource = dt
            cmbBank.DisplayMember = "bank_name"
            cmbBank.ValueMember = "id"
            cmbBank.SelectedIndex = -1 ' Start with no bank selected
            
        Catch ex As Exception
            ' Silent fail for banks or show minor warning
        Finally
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Sub

    Private Sub SetupDenominationControls()
        pnlDenominations.Controls.Clear()
        For Each d In denominations
            Dim lbl = New Label With {.Text = d & " x ", .Width = 80, .Font = New Font("Segoe UI", 12, FontStyle.Bold)}
            Dim txt = New TextBox With {.Width = 100, .Font = New Font("Segoe UI", 12), .Tag = d}
            AddHandler txt.TextChanged, AddressOf CalculateDenomTotal
            AddHandler txt.KeyDown, AddressOf Denom_KeyDown
            pnlDenominations.Controls.Add(lbl)
            pnlDenominations.Controls.Add(txt)
            pnlDenominations.SetFlowBreak(txt, True)
            denomInputs(d) = txt
        Next
    End Sub

    Private Sub CalculateDenomTotal()
        Dim total As Decimal = 0
        For Each kvp In denomInputs
            Dim count As Integer = 0
            Integer.TryParse(kvp.Value.Text, count)
            total += (kvp.Key * count)
        Next
        If total > 0 Then
            txtAmount.Text = total.ToString("F2")
        End If
    End Sub

    Private Sub Denom_KeyDown(sender As Object, e As KeyEventArgs)
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            Dim currentTxt = DirectCast(sender, TextBox)
            Dim parent = currentTxt.Parent
            Dim index = parent.Controls.IndexOf(currentTxt)
            For i As Integer = index + 1 To parent.Controls.Count - 1
                If TypeOf parent.Controls(i) Is TextBox Then
                    parent.Controls(i).Focus()
                    DirectCast(parent.Controls(i), TextBox).SelectAll()
                    Return
                End If
            Next
            btnSave.Focus()
        End If
    End Sub

    Private Sub cmbSource_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbSource.SelectedIndexChanged
        ' Enable Bank selection only if Source is Bank
        cmbBank.Enabled = (cmbSource.SelectedItem.ToString() = "Bank")
        If Not cmbBank.Enabled Then cmbBank.SelectedIndex = -1
    End Sub

    Private Sub ComboBoxes_Enter(sender As Object, e As EventArgs) Handles cmbItemType.Enter, cmbTransactionType.Enter, cmbSource.Enter, cmbBank.Enter
        DirectCast(sender, ComboBox).DroppedDown = True
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub

    Private Sub txtAmount_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtAmount.KeyPress
        ' Only allow numbers and one decimal point
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) AndAlso (e.KeyChar <> "."c) Then
            e.Handled = True
        End If
        If (e.KeyChar = "."c) AndAlso (CType(sender, TextBox).Text.IndexOf("."c) > -1) Then
            e.Handled = True
        End If
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        ' Validation
        If String.IsNullOrWhiteSpace(txtItemName.Text) Then
            MessageBox.Show("Please enter a description.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtItemName.Focus()
            Return
        End If

        If String.IsNullOrWhiteSpace(txtAmount.Text) OrElse Val(txtAmount.Text) <= 0 Then
            MessageBox.Show("Please enter a valid amount greater than 0.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtAmount.Focus()
            Return
        End If

        Try
            db_connection()
            If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()


            Dim transType As String = If(cmbTransactionType.SelectedIndex = 1, "IN", "OUT")
            Dim bankId As Object = DBNull.Value
            If cmbBank.Enabled AndAlso cmbBank.SelectedValue IsNot Nothing Then
                bankId = cmbBank.SelectedValue
            End If

            Dim sql As String = ""
            If CurrentUpdateID > 0 Then
                sql = "UPDATE petty_cash SET item_name=@iname, amount=@amt, transaction_type=@ttype, item_type=@itype, date=@dt, user=@usr, receipt_no=@rno, bank_id=@bid, source=@src, branch=@br, machine=@mch WHERE id=@id"
            Else
                sql = "INSERT INTO petty_cash (item_name, amount, transaction_type, item_type, date, user, receipt_no, bank_id, source, branch, machine) " &
                      "VALUES (@iname, @amt, @ttype, @itype, @dt, @usr, @rno, @bid, @src, @br, @mch)"
            End If
            
            Dim cmd As New MySqlCommand(sql, MySqlConn)
            cmd.Parameters.AddWithValue("@iname", txtItemName.Text.Trim())
            cmd.Parameters.AddWithValue("@amt", Convert.ToDecimal(txtAmount.Text))
            cmd.Parameters.AddWithValue("@ttype", transType)
            cmd.Parameters.AddWithValue("@itype", cmbItemType.SelectedItem.ToString())
            cmd.Parameters.AddWithValue("@dt", dtpDate.Value.ToString("yyyy-MM-dd HH:mm:ss"))
            cmd.Parameters.AddWithValue("@usr", UserName)
            cmd.Parameters.AddWithValue("@rno", txtReceiptNo.Text.Trim())
            cmd.Parameters.AddWithValue("@bid", bankId)
            cmd.Parameters.AddWithValue("@src", cmbSource.SelectedItem.ToString())
            cmd.Parameters.AddWithValue("@br", txtBranch.Text.Trim())
            cmd.Parameters.AddWithValue("@mch", Environment.MachineName)
            
            If CurrentUpdateID > 0 Then
                cmd.Parameters.AddWithValue("@id", CurrentUpdateID)
            End If

            cmd.ExecuteNonQuery()
            
            ' Determine the ID for denomination record
            Dim refId As Integer = If(CurrentUpdateID > 0, CurrentUpdateID, Convert.ToInt32(cmd.LastInsertedId))
            
            ' Save Denominations
            SaveDenominations(refId)
            
            Dim msg As String = If(CurrentUpdateID > 0, "Petty Cash entry updated successfully.", "Petty Cash entry saved successfully.")
            MessageBox.Show(msg, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            
            If CurrentUpdateID > 0 Then
                Me.DialogResult = DialogResult.OK
                Me.Close()
            Else
                ' Clear fields after save
                txtItemName.Text = ""
                txtAmount.Text = ""
                txtReceiptNo.Text = ""
                txtBranch.Text = ""
                cmbSource.SelectedIndex = 0
                cmbBank.SelectedIndex = -1
                ' Clear denominations
                For Each kvp In denomInputs
                    kvp.Value.Clear()
                Next
                txtItemName.Focus()
            End If
            
        Catch ex As Exception
            MessageBox.Show("Error saving petty cash: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Sub

    Private Sub SaveDenominations(refId As Integer)
        Try
            ' First, check if already exists (for updates)
            Dim exists As Boolean = False
            Using checkCmd As New MySqlCommand("SELECT COUNT(*) FROM denomination_records WHERE ref_type='PETTY_CASH' AND ref_id=@id", MySqlConn)
                checkCmd.Parameters.AddWithValue("@id", refId)
                exists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0
            End Using

            Dim sql As String = ""
            If exists Then
                sql = "UPDATE denomination_records SET d5000=@d5000, d2000=@d2000, d1000=@d1000, d500=@d500, d100=@d100, d50=@d50, d20=@d20, d10=@d10, d5=@d5, d2=@d2, d1=@d1, total_amount=@total " &
                      "WHERE ref_type='PETTY_CASH' AND ref_id=@id"
            Else
                sql = "INSERT INTO denomination_records (ref_type, ref_id, d5000, d2000, d1000, d500, d100, d50, d20, d10, d5, d2, d1, total_amount) " &
                      "VALUES ('PETTY_CASH', @id, @d5000, @d2000, @d1000, @d500, @d100, @d50, @d20, @d10, @d5, @d2, @d1, @total)"
            End If

            Using cmd As New MySqlCommand(sql, MySqlConn)
                cmd.Parameters.AddWithValue("@id", refId)
                For Each d In denominations
                    Dim count As Integer = 0
                    If denomInputs.ContainsKey(d) Then Integer.TryParse(denomInputs(d).Text, count)
                    cmd.Parameters.AddWithValue("@d" & d, count)
                Next
                cmd.Parameters.AddWithValue("@total", Val(txtAmount.Text))
                cmd.ExecuteNonQuery()
            End Using
        Catch ex As Exception
            MessageBox.Show("Error saving denominations: " & ex.Message)
        End Try
    End Sub

    Public Sub LoadForUpdate(id As Integer)
        CurrentUpdateID = id
        btnSave.Text = "Update"
        
        ' Ensure banks are loaded before we try to set SelectedValue
        LoadBanks()
        
        Try
            db_connection()
            If MySqlConn.State = ConnectionState.Closed Then MySqlConn.Open()
            
            Dim sql = "SELECT * FROM petty_cash WHERE id = @id"
            Using cmd As New MySqlCommand(sql, MySqlConn)
                cmd.Parameters.AddWithValue("@id", id)
                Using dr As MySqlDataReader = cmd.ExecuteReader()
                    If dr.Read() Then
                        txtItemName.Text = dr("item_name").ToString()
                        txtAmount.Text = Convert.ToDecimal(dr("amount")).ToString("F2")
                        cmbItemType.SelectedItem = dr("item_type").ToString()
                        cmbTransactionType.SelectedIndex = If(dr("transaction_type").ToString().ToUpper() = "IN", 1, 0)
                        dtpDate.Value = Convert.ToDateTime(dr("date"))
                        txtReceiptNo.Text = dr("receipt_no").ToString()
                        cmbSource.SelectedItem = dr("source").ToString()
                        txtBranch.Text = dr("branch").ToString()
                        
                         If Not IsDBNull(dr("bank_id")) Then
                            cmbBank.SelectedValue = dr("bank_id")
                        End If
                    End If
                End Using
            End Using

            ' Load denominations
            Dim sqlDenom = "SELECT * FROM denomination_records WHERE ref_type = 'PETTY_CASH' AND ref_id = @id"
            Using cmd As New MySqlCommand(sqlDenom, MySqlConn)
                cmd.Parameters.AddWithValue("@id", id)
                Using dr = cmd.ExecuteReader()
                    If dr.Read() Then
                        For Each d In denominations
                            Dim col = "d" & d
                            If denomInputs.ContainsKey(d) Then
                                denomInputs(d).Text = dr(col).ToString()
                            End If
                        Next
                    End If
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show("Error loading petty cash data: " & ex.Message)
        Finally
            If MySqlConn.State = ConnectionState.Open Then MySqlConn.Close()
        End Try
    End Sub

    Private Sub PettyCashAdd_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            If TypeOf Me.ActiveControl Is Button Then
                DirectCast(Me.ActiveControl, Button).PerformClick()
            Else
                Me.SelectNextControl(Me.ActiveControl, True, True, True, True)
            End If
        ElseIf e.KeyCode = Keys.Escape Then
            btnCancel.PerformClick()
        End If
    End Sub
End Class
