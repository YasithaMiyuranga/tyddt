Imports MySql.Data.MySqlClient

Public Class InvoiceUpdateWarning
    Public Property OriginalBilling As String = ""
    Public Property OriginalStatus As String = ""
    Public Property NewBilling As String = ""
    Public Property NewStatus As String = ""
    
    Public Property InvNo As String = ""
    Public Property HasReturns As Boolean = False
    Public Property OriginalCredit As Decimal = 0
    Public Property OriginalCheque As Decimal = 0

    Private Sub InvoiceUpdateWarning_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        lblOriginalBilling.Text = "Original Billing: " & OriginalBilling
        lblOriginalStatus.Text = "Original Status: " & OriginalStatus
        lblNewBilling.Text = "New Billing: " & NewBilling
        lblNewStatus.Text = "New Status: " & NewStatus
        
        lblOriginalCredit.Text = "Orig. Credit Due: " & OriginalCredit.ToString("N2")
        lblOriginalCheque.Text = "Orig. Cheque Due: " & OriginalCheque.ToString("N2")
        
        If HasReturns Then
            lblDescription.Text &= vbCrLf & "Refunds for returned items will be guided by the Waterfall Settlement Priority to prevent unsafe cash withdrawals."
        End If
        
        LoadPaymentDetails()
    End Sub

    Private Sub LoadPaymentDetails()
        Try
            Using conn As New MySqlConnection(Module1.ConnStr)
                conn.Open()
                
                ' 1. Check for Cheques (with Bank Join)
                Dim sqlCheck = "SELECT cr.check_number as 'Cheque No', cr.amount as 'Amount', b.bank_name as 'Bank', cr.status as 'Status', cr.check_release_date as 'Date' " &
                               "FROM check_received cr " &
                               "LEFT JOIN bank b ON cr.bank_id = b.id " &
                               "WHERE cr.inv_no = @inv"
                Using cmd = New MySqlCommand(sqlCheck, conn)
                    cmd.Parameters.AddWithValue("@inv", InvNo)
                    Dim dt As New DataTable()
                    Using adapter As New MySqlDataAdapter(cmd)
                        adapter.Fill(dt)
                    End Using
                    
                    ' Fallback to main billing table if no data in check_received
                    If dt.Rows.Count = 0 Then
                        Dim sqlMain = "SELECT cheque_no as 'Cheque No', cheque_balance_due as 'Amount', b.bank_name as 'Bank', IF(po_number='not', '', po_number) as 'Reference', status as 'Status', timestamps as 'Date' " &
                                     "FROM billing main " &
                                     "LEFT JOIN bank b ON main.bank_id = b.id " &
                                     "WHERE main.inv_no = @inv AND (main.cheque_no IS NOT NULL AND main.cheque_no <> '')"
                        Using cmdM = New MySqlCommand(sqlMain, conn)
                            cmdM.Parameters.AddWithValue("@inv", InvNo)
                            Using adapterM As New MySqlDataAdapter(cmdM)
                                adapterM.Fill(dt)
                            End Using
                        End Using
                    End If
                    
                    If dt.Rows.Count > 0 Then
                        dgvCheques.DataSource = dt
                        pnlCheques.Visible = True
                    Else
                        pnlCheques.Visible = False
                    End If
                End Using
                
                ' 2. Check for Payments
                Dim sqlPay = "SELECT PaymentType, Amount, Date FROM customer_payments WHERE inv_no = @inv"
                Using cmd = New MySqlCommand(sqlPay, conn)
                    cmd.Parameters.AddWithValue("@inv", InvNo)
                    Dim dt As New DataTable()
                    Using adapter As New MySqlDataAdapter(cmd)
                        adapter.Fill(dt)
                    End Using
                    
                    If dt.Rows.Count > 0 Then
                        dgvPayments.DataSource = dt
                        pnlPayments.Visible = True
                    Else
                        pnlPayments.Visible = False
                    End If
                End Using
                
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading payment details: " & ex.Message)
        End Try
    End Sub

    Private Sub btnYes_Click(sender As Object, e As EventArgs) Handles btnYes.Click
        Me.DialogResult = DialogResult.Yes
        Me.Close()
    End Sub

    Private Sub btnNo_Click(sender As Object, e As EventArgs) Handles btnNo.Click
        Me.DialogResult = DialogResult.No
        Me.Close()
    End Sub
End Class
