Imports MySql.Data.MySqlClient
Imports System.Text

Public Class TempSales
    Public Property SlotID As Integer = 0
    Dim isElBill As Boolean = True
    Dim dtBill As New DataTable()
    Dim selectedIndex As Integer = -1 ' To track which row is being edited
    Dim syncTimer As New Timer()
    Dim selectedCustomerId As String = "" ' Background tracking for customer ID
    Dim currentSearchMode As String = "" ' To track if dgvSearch is showing Items or Customers
    Private isFormLoading As Boolean = True
    Private isUpdatingSlotCombo As Boolean = False
    Private isWalletApplied As Boolean = False ' Track if wallet balance is being used

    ' Historical Editing Flags
    Private isEditingHistory As Boolean = False
    Private loadedHistoryInvNo As String = ""
    Private loadedHistoryDate As DateTime = DateTime.MinValue
    Private isProcessingLoad As Boolean = False
    Private isRestoringReason As Boolean = False

    ' Fields to track original invoice state for change warnings
    Private originalBillingType As String = ""
    Private originalPaymentMethod As String = ""
    Private originalStatusValue As String = ""
    Private originalChequeNo As String = ""
    Private originalBankId As String = ""
    Private originalChequeAmt As Decimal = 0
    Private originalChequeDate As DateTime = DateTime.MinValue

    'Printer configuration - Change these names to match your printer names in Windows
    'Private POSPrinterName As String = "POS-80"
    'Private POSPrinterName As String = "\\pc2\BIXOLON SRP-E302"
    'Private StandardPrinterName As String = "\\pc2\EPSON L3210 Series"
    'Private POSPrinterName As String = "BIXOLON SRP-E302"
    'Private StandardPrinterName As String = "EPSON L3210 Series"


    'Main
    Private POSPrinterName As String = "\\192.168.100.3\POS-80"
    Private StandardPrinterName As String = "EPSON LQ-310 ESC/P2 (Copy 1)"

    'second Main
    'Private POSPrinterName As String = "\\192.168.100.3\POS-80"
    'Private StandardPrinterName As String = "\\192.168.100.1\epson lq-310 escp2"

    'PC1 
    'Private POSPrinterName As String = "POS-80"
    'Private StandardPrinterName As String = "\\192.168.100.1\epson lq-310 escp2"


    Private Shared AllSlots As New Dictionary(Of Integer, SlotData)
    Private MyReservedSlots As New List(Of Integer)

    Private Class SlotData
        Public Items As DataTable
        Public CustomerPhone As String
        Public CustomerAddress As String
        Public SalesRep As String
        Public BillingTypeIndex As Integer
        Public SaleQuoteIndex As Integer
        Public PaymentMethod As String
        Public IsVat As Boolean
        Public IsWholesale As Boolean
        Public IsRetail As Boolean
        Public TotalAmount As String
        Public GrandTotal As String
        Public OurDiscount As String
        Public InvDiscount As String
        Public CashAmount As String
        Public ChangeAmount As String
        Public Balance As String
        Public CreditBalance As String
        Public ProjectedInvNo As String
        Public IsElBill As Boolean
        Public SelectedCustomerId As String
        Public SelectedBankId As String
        Public ChequeNo As String
        Public TotalVatIndex As Integer
        Public PONumber As String
        ' New Invoice View State Fields
        Public ShowDetailsPanel As Boolean
        Public IsEditingHistory As Boolean
        Public LoadedHistoryInvNo As String
        Public LoadedHistoryDate As DateTime
        Public OriginalStatus As String
        Public OriginalBillingType As String
    End Class

    Private Sub ResetStuckSessionsForThisPC()
        Dim openedHere As Boolean = False
        Try
            If conn.State = ConnectionState.Closed Then
                conn.Open()
                openedHere = True
            End If
            ' Reset any sessions that were marked as used by THIS PC
            ' This clears "ghost" sessions if the app crashed.
            ' [BUG FIX]: Only release sessions that do NOT contain draft items in temp_bill_items.
            ' This prevents active slots with drafts (e.g. Slot 3) from being marked as free, which caused them to be purged on switch.
            Dim sql = "UPDATE window_sessions w " &
                      "LEFT JOIN (SELECT DISTINCT slot_id FROM temp_bill_items) t ON w.slot_id = t.slot_id " &
                      "SET w.is_used = 0, w.user_name = NULL, w.pc_name = NULL, w.current_type = 'NONE' " &
                      "WHERE w.pc_name = @pc AND t.slot_id IS NULL"
            Using cmd As New MySqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@pc", Environment.MachineName)
                cmd.ExecuteNonQuery()
            End Using
            If openedHere Then conn.Close()
        Catch ex As Exception
            If openedHere AndAlso conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    ''' <summary>Releases active screen lock for a slot. Called on slot switch or form close.</summary>
    Private Sub ResetMySessions(id As Integer)
        If id <= 0 Then Return
        Dim openedHere As Boolean = False
        Try
            If conn.State = ConnectionState.Closed Then
                conn.Open()
                openedHere = True
            End If

            ' Release active screen lock in window_sessions so other machines can access this slot
            Dim sql = "UPDATE window_sessions SET is_used = 0, user_name = NULL, pc_name = NULL, current_type = 'NONE' " &
                      "WHERE slot_id = @slot AND (user_name = @uname OR user_name IS NULL)"
            Using cmd As New MySqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@uname", Module1.UserName)
                cmd.Parameters.AddWithValue("@slot", id)
                cmd.ExecuteNonQuery()
            End Using

            ' Remove from tracking list if successfully released
            If MyReservedSlots.Contains(id) Then MyReservedSlots.Remove(id)

            If openedHere Then conn.Close()
        Catch ex As Exception
            If openedHere AndAlso conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub EnsureSlotsExist()
        Dim openedHere As Boolean = False
        Try
            If conn.State = ConnectionState.Closed Then
                conn.Open()
                openedHere = True
            End If
            Dim checkCountSql = "SELECT COUNT(*) FROM window_sessions"
            Dim currentCount As Integer = 0
            Using cmdCount As New MySqlCommand(checkCountSql, conn)
                currentCount = Convert.ToInt32(cmdCount.ExecuteScalar())
            End Using

            If currentCount < 30 Then
                ' Create missing slots
                For i As Integer = 1 To 30
                    Dim insertSql = "INSERT IGNORE INTO window_sessions (slot_id, is_used, pc_name, user_name, current_type) VALUES (@id, 0, NULL, NULL, 'NONE')"
                    Using cmdIns As New MySqlCommand(insertSql, conn)
                        cmdIns.Parameters.AddWithValue("@id", i)
                        cmdIns.ExecuteNonQuery()
                    End Using
                Next
            End If
            If openedHere Then conn.Close()
        Catch ex As Exception
            If openedHere AndAlso conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub ResetAllSessionsCompletely()
        EnsureSlotsExist() ' Ensure slots exist before resetting

        Dim openedHere As Boolean = False
        Try
            If conn.State = ConnectionState.Closed Then
                conn.Open()
                openedHere = True
            End If

            ' Reset all to unused
            Dim sql = "UPDATE window_sessions SET is_used = 0, pc_name = NULL, user_name = NULL, current_type = 'NONE'"
            Using cmd As New MySqlCommand(sql, conn)
                cmd.ExecuteNonQuery()
            End Using

            If openedHere Then conn.Close()
            MessageBox.Show("All 30 Sales slots have been initialized and reset successfully.", "Reset Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show("Error resetting sessions: " & ex.Message)
            If openedHere AndAlso conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub DataGridView2_KeyDown(sender As Object, e As KeyEventArgs) Handles DataGridView2.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            If dtBill.Rows.Count > 0 AndAlso DataGridView2.CurrentRow IsNot Nothing Then
                ' Grid has items — edit the selected row
                btnUpdate.PerformClick()
            Else
                ' Grid is empty (app just started) — move focus to Customer Name entry
                txtSalesRep.Focus()
            End If
        End If
    End Sub


    Private Function SerializeSlotData(data As SlotData) As String
        Try
            Dim sw As New System.IO.StringWriter()
            data.Items.TableName = "BillItems"
            data.Items.WriteXml(sw)
            Dim itemsXml = sw.ToString()

            ' Simple pipe-separated metadata + XML for items
            Dim meta = String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}|{15}|{16}|{17}|{18}|{19}|{20}|{21}|{22}|{23}|{24}|{25}|{26}|{27}|{28}|{29}",
                data.CustomerPhone, data.CustomerAddress, data.SalesRep,
                data.BillingTypeIndex, data.SaleQuoteIndex, data.PaymentMethod,
                data.IsVat, data.IsWholesale, data.IsRetail,
                data.TotalAmount, data.GrandTotal, data.OurDiscount,
                data.InvDiscount, data.CashAmount, data.ChangeAmount,
                data.Balance, data.CreditBalance, data.IsElBill, data.SelectedCustomerId,
                data.SelectedBankId, data.ProjectedInvNo, data.TotalVatIndex, data.ChequeNo, data.PONumber,
                data.ShowDetailsPanel, data.IsEditingHistory, data.LoadedHistoryInvNo,
                data.LoadedHistoryDate.ToString("yyyy-MM-dd HH:mm:ss"), data.OriginalStatus,
                data.OriginalBillingType)

            Return meta & "[ITEMS]" & itemsXml
        Catch
            Return ""
        End Try
    End Function

    Private Function DeserializeSlotData(serialized As String) As SlotData
        Try
            If String.IsNullOrEmpty(serialized) OrElse Not serialized.Contains("[ITEMS]") Then Return Nothing
            Dim sep() As String = {"[ITEMS]"}
            Dim parts = serialized.Split(sep, StringSplitOptions.None)
            Dim metaParts = parts(0).Split("|"c)
            Dim itemsXml = parts(1)

            Dim data As New SlotData()
            data.CustomerPhone = metaParts(0)
            data.CustomerAddress = metaParts(1)
            data.SalesRep = metaParts(2)
            data.BillingTypeIndex = Integer.Parse(metaParts(3))
            data.SaleQuoteIndex = Integer.Parse(metaParts(4))
            data.PaymentMethod = metaParts(5)
            data.IsVat = Boolean.Parse(metaParts(6))
            data.IsWholesale = Boolean.Parse(metaParts(7))
            data.IsRetail = Boolean.Parse(metaParts(8))
            data.TotalAmount = metaParts(9)
            data.GrandTotal = metaParts(10)
            data.OurDiscount = metaParts(11)
            data.InvDiscount = metaParts(12)
            data.CashAmount = metaParts(13)
            data.ChangeAmount = metaParts(14)
            data.Balance = metaParts(15)
            data.CreditBalance = metaParts(16)
            data.IsElBill = Boolean.Parse(metaParts(17))
            data.SelectedCustomerId = metaParts(18)
            data.SelectedBankId = metaParts(19)
            data.ProjectedInvNo = metaParts(20)
            If metaParts.Length > 21 Then data.TotalVatIndex = Integer.Parse(metaParts(21))
            If metaParts.Length > 22 Then data.ChequeNo = metaParts(22)
            If metaParts.Length > 23 Then data.PONumber = metaParts(23)
            If metaParts.Length > 24 Then data.ShowDetailsPanel = Boolean.Parse(metaParts(24))
            If metaParts.Length > 25 Then data.IsEditingHistory = Boolean.Parse(metaParts(25))
            If metaParts.Length > 26 Then data.LoadedHistoryInvNo = metaParts(26)
            If metaParts.Length > 27 Then DateTime.TryParse(metaParts(27), data.LoadedHistoryDate)
            If metaParts.Length > 28 Then data.OriginalStatus = metaParts(28)
            If metaParts.Length > 29 Then data.OriginalBillingType = metaParts(29)

            data.Items = New DataTable("BillItems")
            Using sr As New System.IO.StringReader(itemsXml)
                data.Items.ReadXml(sr)
            End Using

            Return data
        Catch
            Return Nothing
        End Try
    End Function

    Private Sub UpdateSessionTypeInDB()
        If SlotID <= 0 Then Return
        Dim openedHere As Boolean = False
        Try
            If conn.State = ConnectionState.Closed Then
                conn.Open()
                openedHere = True
            End If
            Dim typeVal As String = "NONE"
            If ComboBox1.Text = "Sale" Then
                If CheckBoxIsVat.Checked Then
                    typeVal = "VT"
                Else
                    typeVal = If(isElBill, "EL", "GR")
                End If
            ElseIf ComboBox1.Text = "Quote" Then
                typeVal = "QT"
            End If

            Dim localSql = "UPDATE window_sessions SET current_type = @type WHERE slot_id = @slot"
            Using localCmd As New MySqlCommand(localSql, conn)
                localCmd.Parameters.AddWithValue("@type", typeVal)
                localCmd.Parameters.AddWithValue("@slot", SlotID)
                localCmd.ExecuteNonQuery()
            End Using
            If openedHere Then conn.Close()
            ' Trigger immediate refresh
            UpdateLiveInvoiceProjection()
        Catch ex As Exception
            If openedHere AndAlso conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        ' Allow changing type even in history mode (to support conversion from Quote to Sale)
        If ComboBox1.Text = "Quote" Then
            ' Quotations shouldn't be forced into Wholesale or Retail usually, 
            ' or as per user request, they should not be selectable.
            CheckBoxWholesale.Checked = False
            CheckBoxRetail.Checked = False
            CheckBoxWholesale.Enabled = False
            CheckBoxRetail.Enabled = False
        Else
            CheckBoxWholesale.Enabled = True
            CheckBoxRetail.Enabled = True
        End If
        UpdateSessionTypeInDB()
    End Sub

    Private Sub LoadVatRates()
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()
            Dim sqlVat = "SELECT id, vat_name, vat_value FROM vat WHERE is_active = 1"
            Dim adapter As New MySqlDataAdapter(sqlVat, conn)
            Dim dtVat As New DataTable()
            adapter.Fill(dtVat)

            ' Add a manual "None" row with ID 1 if it doesn't exist, 
            ' or ensure ID 1 is the default. Usually, vat_id 1 is No VAT 0%.

            Dim dtVatItem = dtVat.Copy()
            Dim dtVatTotal = dtVat.Copy()

            ' Create Display Member Column
            dtVatItem.Columns.Add("display_name", GetType(String), "vat_name + ' (' + vat_value + '%)'")
            dtVatTotal.Columns.Add("display_name", GetType(String), "vat_name + ' (' + vat_value + '%)'")

            ' Populate Item-level VAT ComboBox
            ComboBoxVat.DataSource = dtVatItem
            ComboBoxVat.DisplayMember = "display_name"
            ComboBoxVat.ValueMember = "id"

            ' Populate Bill-level VAT ComboBox
            ComboBoxTotalVat.DataSource = dtVatTotal
            ComboBoxTotalVat.DisplayMember = "display_name"
            ComboBoxTotalVat.ValueMember = "id"

            ' Set default to VAT ID 1 (requested)
            ComboBoxVat.SelectedValue = 1
            ComboBoxTotalVat.SelectedValue = 1

            conn.Close()
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Public Sub ApplySecurityLock()
        If Not Module1.IsRgrVisible Then
            ' Hide full credit and limits
            lblAccountOutstanding.Visible = False
            LabelAccountOutstandingHeader.Visible = False
            lblCreditLimit.Visible = False
            txtCreditLimit.Visible = False
            lblCreditPeriod.Visible = False
            dtpCreditPeriod.Visible = False

            ' Hide the Customer list by default, but allow manual re-enable
            chkShowCustomer.Checked = False

            ' Ensure the inputs remain accessible
            txtSalesRep.Enabled = True
            txtCustomerPhone.Enabled = True
            txtCustomerAddress.Enabled = True
            btnAddCustomer.Enabled = True
        Else
            ' Show full credit and limits
            lblAccountOutstanding.Visible = True
            LabelAccountOutstandingHeader.Visible = True
            lblCreditLimit.Visible = True
            txtCreditLimit.Visible = True
            lblCreditPeriod.Visible = True
            dtpCreditPeriod.Visible = True

            If btnInvoiceDetails IsNot Nothing Then btnInvoiceDetails.Visible = True

            ' Show the Customer list by default
            chkShowCustomer.Checked = True

            ' Re-enable
            txtSalesRep.Enabled = True
            txtCustomerPhone.Enabled = True
            txtCustomerAddress.Enabled = True
            btnAddCustomer.Enabled = True
        End If
    End Sub

    Private Sub TempSales_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            ' Ensure draft_user column exists for user-specific recovery
            Dim sqlDraftUser As String = "ALTER TABLE temp_bill_items ADD COLUMN draft_user VARCHAR(100) NULL"
            Using cmdDraft As New MySqlCommand(sqlDraftUser, conn)
                Try
                    If conn.State = ConnectionState.Closed Then conn.Open()
                    cmdDraft.ExecuteNonQuery()
                Catch : End Try
            End Using

            ' Ensure adv_pay column exists in temp_bill_items for draft recovery
            Dim sqlTempAdvPay As String = "ALTER TABLE temp_bill_items ADD COLUMN adv_pay DECIMAL(15,2) DEFAULT 0"
            Using cmdTempAdv As New MySqlCommand(sqlTempAdvPay, conn)
                Try
                    If conn.State = ConnectionState.Closed Then conn.Open()
                    cmdTempAdv.ExecuteNonQuery()
                Catch : End Try
            End Using

            Dim sqlCreate As String = "CREATE TABLE IF NOT EXISTS `sales_adjustments` (" &
                                      "`id` INT AUTO_INCREMENT PRIMARY KEY, " &
                                      "`inv_no` VARCHAR(50) NOT NULL, " &
                                      "`adjustment_date` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP, " &
                                      "`difference_amount` DECIMAL(15,2) NOT NULL, " &
                                      "`cashier_id` VARCHAR(50), " &
                                      "`reason` VARCHAR(255))"
            Using cmdCreate As New MySqlCommand(sqlCreate, conn)
                If conn.State = ConnectionState.Closed Then conn.Open()
                cmdCreate.ExecuteNonQuery()
            End Using


            Dim sqlAlter As String = "ALTER TABLE billing_history ADD COLUMN timestamps DATETIME NULL"
            Using cmdAlter As New MySqlCommand(sqlAlter, conn)
                Try
                    cmdAlter.ExecuteNonQuery()
                Catch
                End Try
            End Using

            Dim sqlCreditNotes As String = "CREATE TABLE IF NOT EXISTS `customer_credit_notes` (" &
                                      "`id` INT AUTO_INCREMENT PRIMARY KEY, " &
                                      "`customer_id` VARCHAR(50) NOT NULL, " &
                                      "`inv_no` VARCHAR(50) NOT NULL, " &
                                      "`credit_amount` DECIMAL(15,2) NOT NULL DEFAULT 0, " &
                                      "`issue_date` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP, " &
                                      "`cheque_reference` VARCHAR(50), " &
                                      "`status` VARCHAR(20) DEFAULT 'active', " &
                                      "`used_amount` DECIMAL(15,2) NOT NULL DEFAULT 0)"
            Using cmdCredit As New MySqlCommand(sqlCreditNotes, conn)
                Try
                    cmdCredit.ExecuteNonQuery()
                Catch ex1 As Exception
                End Try
            End Using

            Dim sqlPendingReturn As String = "CREATE TABLE IF NOT EXISTS `pending_returns_data` (" &
                                             "`billing_id` INT PRIMARY KEY, " &
                                             "`original_billing_type` VARCHAR(50), " &
                                             "`original_payment_type` VARCHAR(50), " &
                                             "`original_paid_amount` DECIMAL(15,2), " &
                                             "`updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP)"
            Using cmdPending As New MySqlCommand(sqlPendingReturn, conn)
                Try
                    cmdPending.ExecuteNonQuery()
                Catch ex2 As Exception
                End Try
            End Using

            ' Fix: If table was previously created with a bad 'id' column that has no default,
            ' silently drop it so INSERT IGNORE works correctly.
            Dim sqlFixPending As String = "ALTER TABLE `pending_returns_data` DROP COLUMN IF EXISTS `id`"
            Using cmdFix As New MySqlCommand(sqlFixPending, conn)
                Try
                    cmdFix.ExecuteNonQuery()
                Catch ex2 As Exception
                    ' Silently ignore if column doesn't exist or DB version doesn't support IF EXISTS
                End Try
            End Using

            Dim sqlChangeAction As String = "ALTER TABLE billing ADD COLUMN change_action VARCHAR(50) DEFAULT 'CashReturn'"
            Using cmdChangeAction As New MySqlCommand(sqlChangeAction, conn)
                Try
                    cmdChangeAction.ExecuteNonQuery()
                Catch ex As Exception
                End Try
            End Using

            Dim sql1 As String = "ALTER TABLE billing_item ADD COLUMN location VARCHAR(150) AFTER discount"
            Using cmd1 As New MySqlCommand(sql1, conn)
                Try
                    cmd1.ExecuteNonQuery()
                Catch ex1 As Exception
                End Try
            End Using

            Dim sql2 As String = "ALTER TABLE quotation_billing_item ADD COLUMN location VARCHAR(150) AFTER discount"
            Using cmd2 As New MySqlCommand(sql2, conn)
                Try
                    cmd2.ExecuteNonQuery()
                Catch ex2 As Exception
                End Try
            End Using

            Dim sqlTempCusVat As String = "ALTER TABLE billing ADD COLUMN cus_vat_id VARCHAR(100) NULL"
            Using cmdCusVat As New MySqlCommand(sqlTempCusVat, conn)
                Try
                    cmdCusVat.ExecuteNonQuery()
                Catch : End Try
            End Using

            Dim sqlTempCusVatHist As String = "ALTER TABLE billing_history ADD COLUMN cus_vat_id VARCHAR(100) NULL"
            Using cmdCusVatHist As New MySqlCommand(sqlTempCusVatHist, conn)
                Try
                    cmdCusVatHist.ExecuteNonQuery()
                Catch : End Try
            End Using

            Dim sqlTempCusVatQuote As String = "ALTER TABLE quotation_billing ADD COLUMN cus_vat_id VARCHAR(100) NULL"
            Using cmdCusVatQuote As New MySqlCommand(sqlTempCusVatQuote, conn)
                Try
                    cmdCusVatQuote.ExecuteNonQuery()
                Catch : End Try
            End Using

            ' Migrations for print_as_retail flag
            Dim sqlPrintRetail As String = "ALTER TABLE billing ADD COLUMN print_as_retail TINYINT(1) DEFAULT 0"
            Using cmdPrint As New MySqlCommand(sqlPrintRetail, conn)
                Try
                    cmdPrint.ExecuteNonQuery()
                Catch : End Try
            End Using

            Dim sqlPrintRetailQuote As String = "ALTER TABLE quotation_billing ADD COLUMN print_as_retail TINYINT(1) DEFAULT 0"
            Using cmdPrintQuote As New MySqlCommand(sqlPrintRetailQuote, conn)
                Try
                    cmdPrintQuote.ExecuteNonQuery()
                Catch : End Try
            End Using

            Dim sqlPrintRetailPrice As String = "ALTER TABLE billing_item ADD COLUMN print_retail_price DECIMAL(10,2) NULL"
            Using cmdPrintRetailPrice As New MySqlCommand(sqlPrintRetailPrice, conn)
                Try
                    cmdPrintRetailPrice.ExecuteNonQuery()
                Catch : End Try
            End Using

            Dim sqlPrintRetailPriceQuote As String = "ALTER TABLE quotation_billing_item ADD COLUMN print_retail_price DECIMAL(10,2) NULL"
            Using cmdPrintRetailPriceQuote As New MySqlCommand(sqlPrintRetailPriceQuote, conn)
                Try
                    cmdPrintRetailPriceQuote.ExecuteNonQuery()
                Catch : End Try
            End Using

            Dim sqlTempPrintRetailPrice As String = "ALTER TABLE temp_bill_items ADD COLUMN print_retail_price DECIMAL(10,2) DEFAULT NULL"
            Using cmdTempPrintRetailPrice As New MySqlCommand(sqlTempPrintRetailPrice, conn)
                Try
                    cmdTempPrintRetailPrice.ExecuteNonQuery()
                Catch : End Try
            End Using

            If conn.State = ConnectionState.Open Then conn.Close()
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try

        ' Initial cleanup for this PC (clears stuck sessions from previous crashes, but keeps drafts)
        ResetStuckSessionsForThisPC()

        ' Load VAT Rates
        LoadVatRates()


        ' Reserve a slot for this window if not already reserved
        ' Reserve a slot for this window if not already reserved
        If SlotID = 0 Then
            ' 1. Check if this user has a draft slot saved in temp_bill_items
            Dim heldSlot As Integer = 0
            Try
                If conn.State = ConnectionState.Closed Then conn.Open()
                Dim holdSql = "SELECT slot_id FROM temp_bill_items WHERE draft_user = @uname ORDER BY id DESC LIMIT 1"
                Using holdCmd As New MySqlCommand(holdSql, conn)
                    holdCmd.Parameters.AddWithValue("@uname", Module1.UserName)
                    Dim holdRes = holdCmd.ExecuteScalar()
                    If holdRes IsNot Nothing Then
                        heldSlot = Convert.ToInt32(holdRes)
                    End If
                End Using
                conn.Close()
            Catch : End Try

            If heldSlot > 0 Then
                ' Reserve the user's held draft slot
                ReserveSlot(heldSlot, True)
            Else
                ' 2. Find the first truly blank free slot (not in active use AND has no draft items)
                Dim targetBlankSlot As Integer = 0
                Try
                    If conn.State = ConnectionState.Closed Then conn.Open()
                    Dim blankSql = "SELECT w.slot_id FROM window_sessions w " &
                                   "LEFT JOIN (SELECT DISTINCT slot_id FROM temp_bill_items) t ON w.slot_id = t.slot_id " &
                                   "WHERE w.is_used = 0 AND t.slot_id IS NULL ORDER BY w.slot_id LIMIT 1"
                    Using cmdBlank As New MySqlCommand(blankSql, conn)
                        Dim bRes = cmdBlank.ExecuteScalar()
                        If bRes IsNot Nothing Then targetBlankSlot = Convert.ToInt32(bRes)
                    End Using
                    conn.Close()
                Catch : End Try

                If targetBlankSlot > 0 Then
                    ReserveSlot(targetBlankSlot, True)
                Else
                    ' Fallback to first available unused slot
                    If Not ReserveSlot() Then
                        Me.Close()
                        Return
                    End If
                End If
            End If
        End If

        ComboBox1.Items.Add("Sale")
        ComboBox1.Items.Add("Quote")
        ComboBox1.SelectedIndex = 0 ' Default to Sale


        LoadInvoiceNumber()
        LoadCashiers()
        ' Redundant LoadVatRates call removed
        InitializeBillTable() ' Must be initialized before recovery so dtBill schema is correct

        ' Initialize ComboBoxes
        cmbBillingType.Items.Add("Cash")
        cmbBillingType.Items.Add("Credit")
        cmbBillingType.Items.Add("Cheque")
        cmbBillingType.SelectedIndex = -1
        cmbInvSearchType.SelectedIndex = 0 ' Default to Inv No

        ' Initialize Payment Method TextBox with default
        txtPaymentMethod.Text = ""

        ' Initialize Payment Method DataGridView font
        dgvPaymentMethod.DefaultCellStyle.Font = New Font("Microsoft Sans Serif", 9)

        ' Default settings
        CheckBoxIsVat.Checked = False
        CheckBoxRetail.Checked = False
        CheckBoxWholesale.Checked = False
        chkKeepStoreCredit.Visible = False ' Hidden per user request
        btnEl.Visible = True ' Always visible by default
        btnEl.Text = "EL" ' Initialize text to EL since isElBill is True
        ' Set Default Cashier to currently logged in user
        If Not String.IsNullOrEmpty(Module1.UserName) Then
            cmbCashier.Text = Module1.UserName
        End If

        ' User requested: Initially cursor point to item ID
        txtItemID.Focus()
        Me.BeginInvoke(Sub() txtItemID.Focus()) ' Ensure focus sticks after all events finish

        ' Do not auto-show "Cash" customer details; start blank
        txtSalesRep.Text = ""
        txtCustomerPhone.Text = ""
        txtCustomerAddress.Text = ""

        ' Remove empty row at bottom of Grid
        DataGridView2.AllowUserToAddRows = False

        ' Increase iTruenternal text size of the GridView
        DataGridView2.DefaultCellStyle.Font = New Font("Microsoft Sans Serif", 10)
        DataGridView2.ColumnHeadersDefaultCellStyle.Font = New Font("Microsoft Sans Serif", 12, FontStyle.Bold)

        ' Increase iTruenternal text size of the GridView
        dgvInvoices.DefaultCellStyle.Font = New Font("Microsoft Sans Serif", 12)
        dgvInvoices.ColumnHeadersDefaultCellStyle.Font = New Font("Microsoft Sans Serif", 14, FontStyle.Bold)

        ' Increase internal text size of the GridView
        dgvSearch.DefaultCellStyle.Font = New Font("Microsoft Sans Serif", 14)
        dgvSearch.ColumnHeadersDefaultCellStyle.Font = New Font("Microsoft Sans Serif", 16, FontStyle.Bold)

        ' Enable KeyPreview for shortcuts
        Me.KeyPreview = True
        ' DateTimePicker1.Value = DateTime.Now (removed)

        ' Enable Double Buffering to fix flickering/shaking
        EnableDoubleBuffered(DataGridView2)
        EnableDoubleBuffered(dgvInvoices)
        EnableDoubleBuffered(dgvSearch)

        ' Setup Sync Timer for live numbering
        syncTimer.Interval = 1000 ' 1 second
        AddHandler syncTimer.Tick, AddressOf SyncTimer_Tick
        syncTimer.Start()

        ' Sync initial session type
        UpdateSessionTypeInDB()

        ' Configure Navigation Buttons visibility
        UpdateNavigationButtons()

        ' Designer UI buttons used directly instead of generating dynamically
        btnCompleteInv.Visible = False
        btnCancelView.Visible = False
        btnSaveRGR.Visible = False ' Default to Normal bill (light blue Save button) on startup

        ' Load Locations
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()
            Dim dtLoc As New DataTable()
            ' Union with Main Stock to ensure at least one option
            Dim adpLoc As New MySqlDataAdapter("SELECT id, location_name FROM location ORDER BY location_name", conn)
            adpLoc.Fill(dtLoc)
            ComboBoxLocation.DataSource = dtLoc
            ComboBoxLocation.DisplayMember = "location_name"
            ComboBoxLocation.ValueMember = "id"

            ' Default to MAIN STOCK (Case sensitive matching with DB)
            Dim defaultLoc As String = "MAIN STOCK"
            Dim foundIndex As Integer = -1
            For i As Integer = 0 To ComboBoxLocation.Items.Count - 1
                Dim rowVal As String = DirectCast(ComboBoxLocation.Items(i), DataRowView)("location_name").ToString()
                If String.Equals(rowVal, defaultLoc, StringComparison.OrdinalIgnoreCase) Then
                    foundIndex = i
                    Exit For
                End If
            Next

            If foundIndex <> -1 Then
                ComboBoxLocation.SelectedIndex = foundIndex
            Else
                ComboBoxLocation.SelectedIndex = 0
            End If
            conn.Close()
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try

        ' CHECK FOR RECOVERY DATA: Prompt user to restore unsaved draft from previous session
        ' We do this at the VERY END so that default UI initialization doesn't overwrite restored data.
        Using localConn As New MySqlConnection(ConnStr)
            Try
                localConn.Open()
                ' Check if there are any items in temp_bill_items for this slot
                ' Check if there are any items in temp_bill_items for this slot AND this user
                Dim checkSql = "SELECT COUNT(*) FROM temp_bill_items WHERE slot_id = @slot"
                Using cmdCheck As New MySqlCommand(checkSql, localConn)
                    cmdCheck.Parameters.AddWithValue("@slot", SlotID)
                    Dim rowCount = Convert.ToInt32(cmdCheck.ExecuteScalar())

                    If rowCount > 0 Then
                        Dim ans = MessageBox.Show(
                            "You have unsaved bill data in Slot " & SlotID & " from your previous session." & vbCrLf &
                            "Would you like to restore it?" & vbCrLf & vbCrLf &
                            "Yes  →  Load Slot " & SlotID & " draft bill" & vbCrLf &
                            "No   →  Keep draft saved and start a new bill",
                            "Unsaved Bill Data Found",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question)

                        If ans = DialogResult.Yes Then
                            LoadSlotState(SlotID)
                        Else
                            ' User chose No - release current slot lock and switch to a truly blank free slot so draft is not lost
                            ResetMySessions(SlotID)
                            Dim blankSlot As Integer = 0
                            Try
                                Using connBlank As New MySqlConnection(ConnStr)
                                    connBlank.Open()
                                    Dim bSql = "SELECT w.slot_id FROM window_sessions w " &
                                               "LEFT JOIN (SELECT DISTINCT slot_id FROM temp_bill_items) t ON w.slot_id = t.slot_id " &
                                               "WHERE w.is_used = 0 AND t.slot_id IS NULL ORDER BY w.slot_id LIMIT 1"
                                    Using cmdB As New MySqlCommand(bSql, connBlank)
                                        Dim resB = cmdB.ExecuteScalar()
                                        If resB IsNot Nothing Then blankSlot = Convert.ToInt32(resB)
                                    End Using
                                End Using
                            Catch : End Try

                            If blankSlot > 0 Then
                                ReserveSlot(blankSlot, True)
                            End If
                        End If
                    End If
                End Using
            Catch ex As Exception
                ' Silent fail on recovery check
            End Try
        End Using

        ApplySecurityLock()
        
        ' Initial calculation to sync UI (hides VAT labels if not enabled)
        isFormLoading = False ' Mark loading as complete BEFORE final sync/save
        CalculateGrandTotal()

        ' Ensure UI is consistent with role on startup
        ApplyRoleBasedUI()

        ' Ensure focus is placed on the Item ID field at the very end of load (after any recovery dialogs)
        txtItemID.Focus()
        Me.BeginInvoke(Sub() txtItemID.Focus())
    End Sub

    Private Sub ApplyRoleBasedUI()
        Dim fRole As String = If(Module1.FinancialRole IsNot Nothing, Module1.FinancialRole.ToLower(), "")

        ' "seller" is the new name for order taker (Take Order role)
        If fRole = "seller" OrElse fRole.Contains("order") Then
            ' Sellers/Order Takers can now browse history for returns
            ' Seller cannot finalize (Paid status)
            ' Show Payment related controls per user request
            txtCashAmount.Visible = True
            txtChangeAmount.Visible = True
            lblCashAmount.Visible = True
            lblChange.Visible = True
            chkKeepStoreCredit.Visible = False

            ' Show Invoice Details Button (Always visible per user request)
            If btnInvoiceDetails IsNot Nothing Then
                btnInvoiceDetails.Visible = True
            End If

            ' Ensure PO Number is not forced
            TextBoxPO.Enabled = True
        Else
            ' Normal Seller, Main Cashier, Admin or Owner - ensure controls are visible
            txtCashAmount.Visible = True
            txtChangeAmount.Visible = True
            lblCashAmount.Visible = True
            lblChange.Visible = True
        End If

        ' Allow all roles to choose Billing Type and Payment Method
        txtPaymentMethod.Enabled = True

        Dim currentSelection As String = cmbBillingType.Text
        cmbBillingType.Items.Clear()

        If isElBill Then
            cmbBillingType.Items.Add("Cash")
            cmbBillingType.Text = "Cash"
            cmbBillingType.Enabled = True

            ' Default payment method to Cash for EL bills if empty/invalid
            If String.IsNullOrWhiteSpace(txtPaymentMethod.Text) OrElse
               txtPaymentMethod.Text = "Credit" OrElse
               txtPaymentMethod.Text = "Cheque" OrElse
               txtPaymentMethod.Text = "PENDING" Then
                txtPaymentMethod.Text = "Cash"
            End If
        Else
            cmbBillingType.Items.Add("Cash")
            cmbBillingType.Items.Add("Credit")
            cmbBillingType.Items.Add("Cheque")
            cmbBillingType.Enabled = True

            If cmbBillingType.Items.Contains(currentSelection) Then
                cmbBillingType.Text = currentSelection
            Else
                cmbBillingType.SelectedIndex = -1
            End If
        End If

        ' Invoice History Editing Constraints
        If isEditingHistory Then
            If btnClear IsNot Nothing Then btnClear.Visible = False
            If btnEl IsNot Nothing Then btnEl.Visible = False
        Else
            If btnClear IsNot Nothing Then btnClear.Visible = True
            If btnEl IsNot Nothing Then btnEl.Visible = True
        End If

        ' Store credit logic removed per user request
        If chkKeepStoreCredit IsNot Nothing Then chkKeepStoreCredit.Visible = False
    End Sub

    Private Sub UpdateNavigationButtons()
        ' Dynamic visibility: show/hide based on user-accessible slots in each direction
        ButtonBefore.BringToFront()
        btnNext.BringToFront()
        ' Refresh the slot combobox to reflect current state
        LoadComboBoxSlots()
    End Sub


    Private Function ReserveSlot(Optional forcedSlotID As Integer = 0, Optional silent As Boolean = False) As Boolean
        ' Auto-initialize slots if they don't exist
        EnsureSlotsExist()

        Try
            If conn.State = ConnectionState.Closed Then conn.Open()

            If forcedSlotID > 0 Then
                ' Try to reserve specific slot - Allow if free OR already held by this USER
                Dim localSql = "SELECT slot_id FROM window_sessions WHERE slot_id = @forced AND (is_used = 0 OR user_name = @uname)"
                Using localCmd As New MySqlCommand(localSql, conn)
                    localCmd.Parameters.AddWithValue("@forced", forcedSlotID)
                    localCmd.Parameters.AddWithValue("@uname", Module1.UserName)
                    Dim res = localCmd.ExecuteScalar()

                    If res IsNot Nothing Then
                        SlotID = forcedSlotID
                    Else
                        If Not silent Then
                            MessageBox.Show("Slot " & forcedSlotID & " is already in use by another user.", "Slot Busy", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        End If
                        conn.Close()
                        Return False
                    End If
                End Using
            Else
                ' 1. First, check if there's a draft slot saved by THIS USER
                Dim holdSql = "SELECT slot_id FROM temp_bill_items WHERE draft_user = @uname ORDER BY slot_id LIMIT 1"
                Using holdCmd As New MySqlCommand(holdSql, conn)
                    holdCmd.Parameters.AddWithValue("@uname", Module1.UserName)
                    Dim holdRes = holdCmd.ExecuteScalar()

                    If holdRes IsNot Nothing Then
                        SlotID = Convert.ToInt32(holdRes)
                    Else
                        ' 2. If no held draft, find the first TRULY BLANK free slot (not in active use AND has no draft items)
                        Dim freeSql = "SELECT w.slot_id FROM window_sessions w " &
                                      "LEFT JOIN (SELECT DISTINCT slot_id FROM temp_bill_items) t ON w.slot_id = t.slot_id " &
                                      "WHERE w.is_used = 0 AND t.slot_id IS NULL ORDER BY w.slot_id LIMIT 1"
                        Using freeCmd As New MySqlCommand(freeSql, conn)
                            Dim freeResult = freeCmd.ExecuteScalar()

                            If freeResult IsNot Nothing Then
                                SlotID = Convert.ToInt32(freeResult)
                            Else
                                ' Fallback to any unused slot
                                Dim fallbackSql = "SELECT slot_id FROM window_sessions WHERE is_used = 0 ORDER BY slot_id LIMIT 1"
                                Using fbCmd As New MySqlCommand(fallbackSql, conn)
                                    Dim fbResult = fbCmd.ExecuteScalar()
                                    If fbResult IsNot Nothing Then
                                        SlotID = Convert.ToInt32(fbResult)
                                    Else
                                        ' 3. All 30 are truly used by others.
                                        Dim ans = MessageBox.Show("Maximum 30 Sales slots are already in use across the network." & vbCrLf & vbCrLf &
                                                                "Yes -> Reset ALL 30 slots (Use if no one else is billing)" & vbCrLf &
                                                                "No -> Reset only Slot 1" & vbCrLf &
                                                                "Cancel -> Exit",
                                                                "Limit Reached", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning)

                                        If ans = DialogResult.Yes Then
                                            ResetAllSessionsCompletely()
                                            conn.Close()
                                            Return ReserveSlot(forcedSlotID, silent)
                                        ElseIf ans = DialogResult.No Then
                                            Dim sqlReset1 = "UPDATE window_sessions SET is_used = 0, pc_name = NULL, user_name = NULL, current_type = 'NONE' WHERE slot_id = 1"
                                            Using cmdReset1 As New MySqlCommand(sqlReset1, conn)
                                                cmdReset1.ExecuteNonQuery()
                                            End Using
                                            conn.Close()
                                            Return ReserveSlot(forcedSlotID, silent)
                                        Else
                                            conn.Close()
                                            Return False
                                        End If
                                    End If
                                End Using
                            End If
                        End Using
                    End If
                End Using
            End If

            ' Mark as used by current user
            Dim updateSql = "UPDATE window_sessions SET is_used = 1, pc_name = @pc, user_name = @uname, current_type = 'NONE' WHERE slot_id = @slot"
            Using localCmd As New MySqlCommand(updateSql, conn)
                localCmd.Parameters.AddWithValue("@pc", Environment.MachineName)
                localCmd.Parameters.AddWithValue("@uname", Module1.UserName)
                localCmd.Parameters.AddWithValue("@slot", SlotID)
                localCmd.ExecuteNonQuery()
            End Using

            ' Track this slot for release on close
            If Not MyReservedSlots.Contains(SlotID) Then MyReservedSlots.Add(SlotID)

            conn.Close()
            Return True
        Catch ex As Exception
            MessageBox.Show("Error initializing session: " & ex.Message)
            If conn.State = ConnectionState.Open Then conn.Close()
            Return False
        End Try
    End Function

    Private Sub SaveSlotState(id As Integer)
        If id <= 0 OrElse isFormLoading OrElse isProcessingLoad Then Return
        Dim data As New SlotData()
        data.Items = dtBill.Copy()
        data.CustomerPhone = txtCustomerPhone.Text
        data.CustomerAddress = txtCustomerAddress.Text
        data.SalesRep = txtSalesRep.Text
        data.BillingTypeIndex = cmbBillingType.SelectedIndex
        data.SaleQuoteIndex = ComboBox1.SelectedIndex
        data.PaymentMethod = txtPaymentMethod.Text
        data.IsVat = CheckBoxIsVat.Checked
        data.IsWholesale = CheckBoxWholesale.Checked
        data.IsRetail = CheckBoxRetail.Checked
        data.TotalAmount = lblTotalAmount.Text
        data.GrandTotal = lblGrandTotal.Text
        data.OurDiscount = txtOurDiscount.Text
        data.InvDiscount = txtInvDiscount.Text
        data.CashAmount = txtCashAmount.Text
        data.ChangeAmount = txtChangeAmount.Text
        data.Balance = lblBalance.Text
        data.CreditBalance = lblCreditBalance.Text
        data.ProjectedInvNo = lblInvoiceNumber.Text
        data.IsElBill = isElBill
        data.SelectedCustomerId = selectedCustomerId
        data.TotalVatIndex = ComboBoxTotalVat.SelectedIndex
        data.PONumber = TextBoxPO.Text
        ' New Invoice View State
        data.ShowDetailsPanel = InvDetailsPanel.Visible
        data.IsEditingHistory = isEditingHistory
        data.LoadedHistoryInvNo = loadedHistoryInvNo
        data.LoadedHistoryDate = loadedHistoryDate
        data.OriginalStatus = originalStatusValue
        data.OriginalBillingType = originalBillingType

        AllSlots(id) = data
        ' Also sync DB so other slots know what this slot is intending to be
        UpdateSessionTypeInDB()

        ' PERSIST SESSION DATA TO DB (Table-Based Full State)
        Using localConn As New MySqlConnection(ConnStr)
            Try
                localConn.Open()
                ' 1. Logic for clearing:
                ' ONLY clear the draft if we actually have new items or UI state to save, 
                ' OR if the user manually cleared a bill (isFormLoading = False).
                If dtBill.Rows.Count > 0 OrElse InvDetailsPanel.Visible OrElse isEditingHistory Then
                    Dim delSql = "DELETE FROM temp_bill_items WHERE slot_id = @slot"
                    Using delCmd As New MySqlCommand(delSql, localConn)
                        delCmd.Parameters.AddWithValue("@slot", id)
                        delCmd.ExecuteNonQuery()
                    End Using
                Else
                    ' If bill is empty and UI is clean, only delete the draft if we're NOT in loading state
                    ' (meaning the user deliberately cleared the bill)
                    If Not isFormLoading Then
                        Dim delSql = "DELETE FROM temp_bill_items WHERE slot_id = @slot"
                        Using delCmd As New MySqlCommand(delSql, localConn)
                            delCmd.Parameters.AddWithValue("@slot", id)
                            delCmd.ExecuteNonQuery()
                        End Using
                    End If
                    Return ' Don't proceed to insert if empty and no UI state
                End If

                Dim insSql = "INSERT INTO temp_bill_items (" &
                             "slot_id, item_id, description, qty, unit_price, discount, location, amount, vat_label, item_cost, " &
                             "pc_name, customer_name, customer_phone, customer_address, billing_type, payment_method, " &
                             "sale_quote_index, is_vat, is_wholesale, is_retail, total_amount_label, grand_total_label, " &
                             "our_discount, inv_discount, cash_amount, change_amount, balance_label, credit_balance_label, " &
                             "projected_inv_no, is_el_bill, selected_customer_id, total_vat_index, po_number, " &
                             "is_editing_history, loaded_history_inv_no, loaded_history_date, " &
                             "original_status, original_billing_type, return_reason_text, reason, is_original, draft_user, adv_pay, print_retail_price" &
                             ") VALUES (" &
                             "@slot, @item, @desc, @qty, @price, @disc, @loc, @amt, @vat, @cost, " &
                             "@pc, @cus, @phone, @addr, @btype, @pmethod, " &
                             "@sqi, @ivat, @iwhole, @iretail, @tamt, @gtamt, " &
                             "@od, @idisc, @cash, @change, @bal, @cbal, " &
                             "@pinv, @iel, @cid, @tvat, @po, @ieh, @linv, @ldate, @ostatus, @obtype, @rreason, @reason_val, @isoriginal, @duser, @adv_pay, @print_retail)"

                ' Prepare shared metadata values
                Dim ourDisc As Decimal = 0
                Dim invDisc As Decimal = 0
                Dim cashAmt As Decimal = 0
                Dim changeAmt As Decimal = 0
                Dim advPayVal As Decimal = 0
                Decimal.TryParse(txtOurDiscount.Text, ourDisc)
                Decimal.TryParse(txtInvDiscount.Text, invDisc)
                Decimal.TryParse(txtCashAmount.Text, cashAmt)
                Decimal.TryParse(txtChangeAmount.Text, changeAmt)
                Decimal.TryParse(txtAdvPay.Text, advPayVal)

                If dtBill.Rows.Count > 0 Then
                    For Each row As DataRow In dtBill.Rows
                        Using insCmd As New MySqlCommand(insSql, localConn)
                            insCmd.Parameters.AddWithValue("@slot", id)
                            insCmd.Parameters.AddWithValue("@item", row("Item ID"))
                            insCmd.Parameters.AddWithValue("@desc", row("Description"))
                            insCmd.Parameters.AddWithValue("@qty", row("Qty"))
                            insCmd.Parameters.AddWithValue("@price", row("Selling Price"))
                            insCmd.Parameters.AddWithValue("@disc", row("Dis"))
                            insCmd.Parameters.AddWithValue("@loc", row("Location"))
                            insCmd.Parameters.AddWithValue("@amt", row("Total/Amount"))
                            insCmd.Parameters.AddWithValue("@vat", row("VAT"))
                            insCmd.Parameters.AddWithValue("@cost", row("ItemCost"))
                            insCmd.Parameters.AddWithValue("@print_retail", If(dtBill.Columns.Contains("PrintRetailPrice") AndAlso Not IsDBNull(row("PrintRetailPrice")), row("PrintRetailPrice"), 0D))

                            insCmd.Parameters.AddWithValue("@pc", Environment.MachineName) ' Keep for audit
                            insCmd.Parameters.AddWithValue("@cus", txtSalesRep.Text)
                            insCmd.Parameters.AddWithValue("@phone", txtCustomerPhone.Text)
                            insCmd.Parameters.AddWithValue("@addr", txtCustomerAddress.Text)
                            insCmd.Parameters.AddWithValue("@btype", cmbBillingType.Text)
                            insCmd.Parameters.AddWithValue("@pmethod", txtPaymentMethod.Text)

                            insCmd.Parameters.AddWithValue("@sqi", ComboBox1.SelectedIndex)
                            insCmd.Parameters.AddWithValue("@ivat", If(CheckBoxIsVat.Checked, 1, 0))
                            insCmd.Parameters.AddWithValue("@iwhole", If(CheckBoxWholesale.Checked, 1, 0))
                            insCmd.Parameters.AddWithValue("@iretail", If(CheckBoxRetail.Checked, 1, 0))
                            insCmd.Parameters.AddWithValue("@tamt", lblTotalAmount.Text)
                            insCmd.Parameters.AddWithValue("@gtamt", lblGrandTotal.Text)

                            insCmd.Parameters.AddWithValue("@od", ourDisc)
                            insCmd.Parameters.AddWithValue("@idisc", invDisc)
                            insCmd.Parameters.AddWithValue("@cash", cashAmt)
                            insCmd.Parameters.AddWithValue("@change", changeAmt)
                            insCmd.Parameters.AddWithValue("@bal", lblBalance.Text)
                            insCmd.Parameters.AddWithValue("@cbal", lblCreditBalance.Text)

                            insCmd.Parameters.AddWithValue("@pinv", lblInvoiceNumber.Text)
                            insCmd.Parameters.AddWithValue("@iel", If(isElBill, 1, 0))
                            insCmd.Parameters.AddWithValue("@cid", selectedCustomerId)
                            insCmd.Parameters.AddWithValue("@tvat", ComboBoxTotalVat.SelectedIndex)
                            insCmd.Parameters.AddWithValue("@po", TextBoxPO.Text)

                            ' New fields
                            insCmd.Parameters.AddWithValue("@ieh", If(isEditingHistory, 1, 0))
                            insCmd.Parameters.AddWithValue("@linv", loadedHistoryInvNo)
                            insCmd.Parameters.AddWithValue("@ldate", loadedHistoryDate)
                            insCmd.Parameters.AddWithValue("@ostatus", originalStatusValue)
                            insCmd.Parameters.AddWithValue("@obtype", originalBillingType)
                            insCmd.Parameters.AddWithValue("@rreason", cmbReturnReason.Text)
                            insCmd.Parameters.AddWithValue("@reason_val", If(row("Reason") Is DBNull.Value, "", row("Reason").ToString()))
                            Dim isOrigVal As Boolean = True
                            If dtBill.Columns.Contains("IsOriginal") AndAlso row("IsOriginal") IsNot DBNull.Value Then
                                isOrigVal = Convert.ToBoolean(row("IsOriginal"))
                            End If
                            insCmd.Parameters.AddWithValue("@isoriginal", If(isOrigVal, 1, 0))
                            insCmd.Parameters.AddWithValue("@duser", Module1.UserName)
                            insCmd.Parameters.AddWithValue("@adv_pay", advPayVal)

                            insCmd.ExecuteNonQuery()
                        End Using
                    Next
                Else
                    ' Save metadata shell only
                    Using insCmd As New MySqlCommand(insSql, localConn)
                        insCmd.Parameters.AddWithValue("@slot", id)
                        insCmd.Parameters.AddWithValue("@item", DBNull.Value)
                        insCmd.Parameters.AddWithValue("@desc", "EMPTY_SHELL")
                        insCmd.Parameters.AddWithValue("@qty", 0)
                        insCmd.Parameters.AddWithValue("@price", 0)
                        insCmd.Parameters.AddWithValue("@disc", 0)
                        insCmd.Parameters.AddWithValue("@loc", "")
                        insCmd.Parameters.AddWithValue("@amt", 0)
                        insCmd.Parameters.AddWithValue("@vat", "")
                        insCmd.Parameters.AddWithValue("@cost", 0)

                        insCmd.Parameters.AddWithValue("@pc", Environment.MachineName)
                        insCmd.Parameters.AddWithValue("@cus", txtSalesRep.Text)
                        insCmd.Parameters.AddWithValue("@phone", txtCustomerPhone.Text)
                        insCmd.Parameters.AddWithValue("@addr", txtCustomerAddress.Text)
                        insCmd.Parameters.AddWithValue("@btype", cmbBillingType.Text)
                        insCmd.Parameters.AddWithValue("@pmethod", txtPaymentMethod.Text)

                        insCmd.Parameters.AddWithValue("@sqi", ComboBox1.SelectedIndex)
                        insCmd.Parameters.AddWithValue("@ivat", If(CheckBoxIsVat.Checked, 1, 0))
                        insCmd.Parameters.AddWithValue("@iwhole", If(CheckBoxWholesale.Checked, 1, 0))
                        insCmd.Parameters.AddWithValue("@iretail", If(CheckBoxRetail.Checked, 1, 0))
                        insCmd.Parameters.AddWithValue("@tamt", lblTotalAmount.Text)
                        insCmd.Parameters.AddWithValue("@gtamt", lblGrandTotal.Text)

                        insCmd.Parameters.AddWithValue("@od", ourDisc)
                        insCmd.Parameters.AddWithValue("@idisc", invDisc)
                        insCmd.Parameters.AddWithValue("@cash", cashAmt)
                        insCmd.Parameters.AddWithValue("@change", changeAmt)
                        insCmd.Parameters.AddWithValue("@bal", lblBalance.Text)
                        insCmd.Parameters.AddWithValue("@cbal", lblCreditBalance.Text)

                        insCmd.Parameters.AddWithValue("@pinv", lblInvoiceNumber.Text)
                        insCmd.Parameters.AddWithValue("@iel", If(isElBill, 1, 0))
                        insCmd.Parameters.AddWithValue("@cid", selectedCustomerId)
                        insCmd.Parameters.AddWithValue("@tvat", ComboBoxTotalVat.SelectedIndex)
                        insCmd.Parameters.AddWithValue("@po", TextBoxPO.Text)

                        insCmd.Parameters.AddWithValue("@ieh", If(isEditingHistory, 1, 0))
                        insCmd.Parameters.AddWithValue("@linv", loadedHistoryInvNo)
                        insCmd.Parameters.AddWithValue("@ldate", loadedHistoryDate)
                        insCmd.Parameters.AddWithValue("@ostatus", originalStatusValue)
                        insCmd.Parameters.AddWithValue("@obtype", originalBillingType)
                        insCmd.Parameters.AddWithValue("@rreason", cmbReturnReason.Text)
                        insCmd.Parameters.AddWithValue("@reason_val", "")
                        insCmd.Parameters.AddWithValue("@isoriginal", 1)
                        insCmd.Parameters.AddWithValue("@duser", Module1.UserName)
                        insCmd.Parameters.AddWithValue("@adv_pay", advPayVal)

                        insCmd.ExecuteNonQuery()
                    End Using
                End If
            Catch ex As Exception
                ' Silent fail but log to console
                Console.WriteLine("Error saving draft table: " & ex.Message)
            End Try
        End Using
    End Sub

    Private Sub LoadSlotState(id As Integer)
        isProcessingLoad = True
        Try
            ' Clear current state
            ClearFormKeepSlot()

            ' Restore panel state from in-memory session cache (not stale DB).
            ' If AllSlots has an entry for this slot (visited this session), use it.
            ' If this is the very first visit this session, default to False to avoid stale DB data.
            If AllSlots.ContainsKey(id) Then
                InvDetailsPanel.Visible = AllSlots(id).ShowDetailsPanel
                isEditingHistory = AllSlots(id).IsEditingHistory
                loadedHistoryInvNo = AllSlots(id).LoadedHistoryInvNo
                originalStatusValue = AllSlots(id).OriginalStatus
            Else
                InvDetailsPanel.Visible = False
                isEditingHistory = False
                loadedHistoryInvNo = ""
                originalStatusValue = ""
            End If

            ' Apply initial UI visibility based on the restored state (above)
            ' This ensures empty or fresh slots are also correctly updated.
            If InvDetailsPanel.Visible AndAlso Not isEditingHistory Then
                btnCompleteInv.Text = "Back to Sales"
                btnCompleteInv.Visible = True
            ElseIf isEditingHistory Then
                btnCancelView.Visible = True
                btnCompleteInv.Visible = True
                cmbReturnReason.Visible = True
                Label5.Visible = True

                If String.Equals(originalStatusValue, "completed", StringComparison.OrdinalIgnoreCase) Then
                    btnUpdate.Enabled = False
                    btnSave.Enabled = False
                    btnDelete.Enabled = False
                    btnAddNew.Enabled = False
                Else
                    btnUpdate.Enabled = True
                    btnSave.Enabled = True
                    btnDelete.Enabled = True
                    btnAddNew.Enabled = True
                End If

                CheckBoxWholesale.Enabled = True
                CheckBoxRetail.Enabled = True
                CheckBoxIsVat.Enabled = True
            End If

            Using localConn As New MySqlConnection(ConnStr)
                Try
                    localConn.Open()
                    Dim loadSql = "SELECT * FROM temp_bill_items WHERE slot_id = @slot ORDER BY id ASC"
                    Using cmd As New MySqlCommand(loadSql, localConn)
                        cmd.Parameters.AddWithValue("@slot", id)
                        Using dr = cmd.ExecuteReader()
                            Dim isFirstRow As Boolean = True
                            While dr.Read()
                                ' Restore Header Info (from first row)
                                If isFirstRow Then
                                    isElBill = If(dr("is_el_bill") Is DBNull.Value, False, Convert.ToBoolean(dr("is_el_bill")))
                                    selectedCustomerId = If(dr("selected_customer_id") Is DBNull.Value, "", dr("selected_customer_id").ToString())

                                    txtSalesRep.Text = If(dr("customer_name") Is DBNull.Value, "", dr("customer_name").ToString())
                                    txtCustomerPhone.Text = If(dr("customer_phone") Is DBNull.Value, "", dr("customer_phone").ToString())
                                    txtCustomerAddress.Text = If(dr("customer_address") Is DBNull.Value, "", dr("customer_address").ToString())
                                    cmbBillingType.Text = If(dr("billing_type") Is DBNull.Value, If(isElBill, "Cash", ""), dr("billing_type").ToString())
                                    txtPaymentMethod.Text = If(dr("payment_method") Is DBNull.Value, "", dr("payment_method").ToString())

                                    ComboBox1.SelectedIndex = If(dr("sale_quote_index") Is DBNull.Value, 0, Convert.ToInt32(dr("sale_quote_index")))
                                    CheckBoxIsVat.Checked = If(dr("is_vat") Is DBNull.Value, False, Convert.ToBoolean(dr("is_vat")))
                                    CheckBoxWholesale.Checked = If(dr("is_wholesale") Is DBNull.Value, False, Convert.ToBoolean(dr("is_wholesale")))
                                    CheckBoxRetail.Checked = If(dr("is_retail") Is DBNull.Value, False, Convert.ToBoolean(dr("is_retail")))

                                    lblTotalAmount.Text = If(dr("total_amount_label") Is DBNull.Value, "0.00", dr("total_amount_label").ToString())
                                    lblGrandTotal.Text = If(dr("grand_total_label") Is DBNull.Value, "0.00", dr("grand_total_label").ToString())
                                    txtOurDiscount.Text = If(dr("our_discount") Is DBNull.Value, "0.00", Convert.ToDecimal(dr("our_discount")).ToString("N2"))
                                    txtInvDiscount.Text = If(dr("inv_discount") Is DBNull.Value, "0.00", Convert.ToDecimal(dr("inv_discount")).ToString("N2"))
                                    txtCashAmount.Text = If(dr("cash_amount") Is DBNull.Value, "", Convert.ToDecimal(dr("cash_amount")).ToString())
                                    txtChangeAmount.Text = If(dr("change_amount") Is DBNull.Value, "0.00", Convert.ToDecimal(dr("change_amount")).ToString("N2"))
                                    Try
                                        txtAdvPay.Text = If(dr("adv_pay") Is DBNull.Value, "0.00", Convert.ToDecimal(dr("adv_pay")).ToString("N2"))
                                    Catch
                                        txtAdvPay.Text = "0.00"
                                    End Try
                                    lblBalance.Text = If(dr("balance_label") Is DBNull.Value, "0.00", dr("balance_label").ToString())
                                    lblCreditBalance.Text = If(dr("credit_balance_label") Is DBNull.Value, "0.00", dr("credit_balance_label").ToString())

                                    lblInvoiceNumber.Text = If(dr("projected_inv_no") Is DBNull.Value, "", dr("projected_inv_no").ToString())
                                    ComboBoxTotalVat.SelectedIndex = If(dr("total_vat_index") Is DBNull.Value, 0, Convert.ToInt32(dr("total_vat_index")))
                                    TextBoxPO.Text = If(dr("po_number") Is DBNull.Value, "", dr("po_number").ToString())

                                    ' Update button text if it was EL/GR
                                    If btnEl.Visible Then
                                        btnEl.Text = If(isElBill, "EL", "GR")
                                    End If

                                    ' New Invoice View State
                                    isEditingHistory = If(dr("is_editing_history") Is DBNull.Value, False, Convert.ToBoolean(dr("is_editing_history")))
                                    loadedHistoryInvNo = If(dr("loaded_history_inv_no") Is DBNull.Value, "", dr("loaded_history_inv_no").ToString())
                                    If dr("loaded_history_date") IsNot DBNull.Value Then loadedHistoryDate = Convert.ToDateTime(dr("loaded_history_date"))

                                    originalStatusValue = If(dr("original_status") Is DBNull.Value, "", dr("original_status").ToString())
                                    originalBillingType = If(dr("original_billing_type") Is DBNull.Value, "", dr("original_billing_type").ToString())
                                    isRestoringReason = True
                                    cmbReturnReason.Text = If(dr("return_reason_text") Is DBNull.Value, "", dr("return_reason_text").ToString())
                                    isRestoringReason = False

                                    ' Sync UI visibility again JUST IN CASE the DB row has different info than memory 
                                    ' (e.g. after a crash or manual DB edit), but memory usually wins at the start of this sub.
                                    ' Ask for confirmation if updating existing bill (Skip for Order Taker)
                                    If isEditingHistory AndAlso Module1.FinancialRole.ToLower() <> "order taker" Then
                                        Dim confirmMsg = "Warning: Re-saving an existing invoice will overwrite previous history." & vbCrLf & "Are you sure?"
                                        If MessageBox.Show(confirmMsg, "Update Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.No Then
                                            Return
                                        End If
                                    End If
                                    If isEditingHistory Then
                                        btnCancelView.Visible = True
                                        btnCompleteInv.Visible = True
                                        cmbReturnReason.Visible = True
                                        Label5.Visible = True

                                        If String.Equals(originalStatusValue, "completed", StringComparison.OrdinalIgnoreCase) Then
                                            btnUpdate.Enabled = False
                                            btnSave.Enabled = False
                                            btnDelete.Enabled = False
                                            btnAddNew.Enabled = False
                                        Else
                                            btnUpdate.Enabled = True
                                            btnSave.Enabled = True
                                            btnDelete.Enabled = True
                                            btnAddNew.Enabled = True
                                        End If

                                        CheckBoxWholesale.Enabled = True
                                        CheckBoxRetail.Enabled = True
                                        CheckBoxIsVat.Enabled = True
                                    End If

                                    isFirstRow = False
                                End If

                                Dim descVal = If(dr("description") Is DBNull.Value, "", dr("description").ToString())
                                If descVal = "EMPTY_SHELL" Then Continue While

                                ' Restore Item Row
                                Dim newRow = dtBill.NewRow()
                                newRow("Item ID") = If(dr("item_id") Is DBNull.Value, "", dr("item_id").ToString())
                                newRow("Description") = descVal
                                newRow("Qty") = If(dr("qty") Is DBNull.Value, 0D, Convert.ToDecimal(dr("qty")))
                                newRow("Selling Price") = If(dr("unit_price") Is DBNull.Value, 0D, Convert.ToDecimal(dr("unit_price")))
                                newRow("Dis") = If(dr("discount") Is DBNull.Value, 0D, Convert.ToDecimal(dr("discount")))
                                newRow("Location") = If(dr("location") Is DBNull.Value, "MAIN STOCK", dr("location").ToString())
                                newRow("Total/Amount") = If(dr("amount") Is DBNull.Value, 0D, Convert.ToDecimal(dr("amount")))
                                newRow("VAT") = If(dr("vat_label") Is DBNull.Value, "", dr("vat_label").ToString())
                                newRow("ItemCost") = If(dr("item_cost") Is DBNull.Value, 0D, Convert.ToDecimal(dr("item_cost")))
                                newRow("Reason") = If(dr("reason") Is DBNull.Value, "", dr("reason").ToString())
                                newRow("IsOriginal") = If(dr("is_original") Is DBNull.Value, True, Convert.ToBoolean(dr("is_original")))
                                newRow("PrintRetailPrice") = If(dr("print_retail_price") Is DBNull.Value, 0D, Convert.ToDecimal(dr("print_retail_price")))
                                dtBill.Rows.Add(newRow)
                            End While
                        End Using
                    End Using
                Catch ex As Exception
                    MessageBox.Show("Error loading draft: " & ex.Message & vbCrLf & "Stack: " & ex.StackTrace)
                End Try
            End Using

            DataGridView2.DataSource = dtBill
            CalculateGrandTotal()
            UpdateNavigationButtons()
            ' Sync type since current_type might be different for loaded slot
            UpdateSessionTypeInDB()

            ' Restore Customer Credit Info (Limit and Period) when switching slots
            If Not String.IsNullOrEmpty(selectedCustomerId) Then
                Try
                    Using cusConn As New MySqlConnection(ConnStr)
                        cusConn.Open()
                        Dim cSql As String = "SELECT credit_limit, credit_period FROM customer WHERE id = @cid"
                        Using cmdCus As New MySqlCommand(cSql, cusConn)
                            cmdCus.Parameters.AddWithValue("@cid", selectedCustomerId)
                            Using drCus = cmdCus.ExecuteReader()
                                If drCus.Read() Then
                                    txtCreditLimit.Text = If(IsDBNull(drCus("credit_limit")), "0.00", Convert.ToDecimal(drCus("credit_limit")).ToString("N2"))
                                    Dim cpVal As String = If(IsDBNull(drCus("credit_period")), "", drCus("credit_period").ToString())
                                    If Not String.IsNullOrEmpty(cpVal) AndAlso DateTime.TryParse(cpVal, Nothing) Then
                                        dtpCreditPeriod.Value = Convert.ToDateTime(cpVal)
                                    Else
                                        dtpCreditPeriod.Value = DateTime.Now
                                    End If
                                End If
                            End Using
                        End Using
                    End Using
                Catch ex As Exception
                    ' Silent fail for credit info restoration
                End Try
            End If

            ' Apply role restrictions after ALL drafting data is loaded or cleared
            ApplyRoleBasedUI()

            ' Restore Full Credit (Outstanding Balance) display
            CalculateTotalCredit()

            ' Ensure focus is placed on the Item ID field
            txtItemID.Focus()
        Finally
            isProcessingLoad = False
        End Try
    End Sub

    ' Helper to clear form without calling UpdateNavigationButtons (to avoid recursion/mess)
    Private Sub ClearFormKeepSlot()
        dtBill.Rows.Clear()
        lblTotalAmount.Text = "0.00"
        lblGrandTotal.Text = "0.00"
        txtCashAmount.Text = ""
        txtAdvPay.Text = ""
        txtChangeAmount.Text = ""
        txtOurDiscount.Text = "0.00"
        txtInvDiscount.Text = "0.00"
        lblBalance.Text = "0.00"
        lblVatBalance.Text = "0.00"
        lblCreditBalance.Text = "0.00"
        CheckBoxIsVat.Checked = False
        CheckBoxIsVat.Enabled = True
        ComboBoxTotalVat.SelectedValue = 1
        CheckBoxWholesale.Checked = False
        CheckBoxWholesale.Enabled = True
        CheckBoxRetail.Checked = False
        CheckBoxRetail.Enabled = True
        cmbBillingType.SelectedIndex = -1
        ComboBox1.SelectedIndex = 0
        txtSalesRep.Text = ""
        txtCustomerPhone.Text = ""
        txtCustomerAddress.Text = ""
        txtPaymentMethod.Text = ""
        TextBoxPO.Text = ""
        txtCusVatId.Text = ""
        dtpCreditPeriod.Value = DateTime.Now
        selectedCustomerId = ""
        isElBill = True
        btnEl.Text = "EL"
        ClearEntryFields()

        isEditingHistory = False
        loadedHistoryInvNo = ""
        InvDetailsPanel.Visible = False
        If btnSaveRGR IsNot Nothing Then
            btnSaveRGR.Visible = False
        End If
        btnCancelView.Visible = False
        btnCompleteInv.Visible = False
        cmbReturnReason.Visible = False
        Label5.Visible = False

        LoadInvoiceNumber()
        lblInvoiceNumber.Modified = False
        CalculateTotalCredit() ' Reset credit display if customer is cleared

        ' Clear cashier password when switching or clearing slots
        txtCashierID.Text = ""
    End Sub

    Private Sub SyncTimer_Tick(sender As Object, e As EventArgs)
        UpdateLiveInvoiceProjection()
    End Sub

    Private Sub UpdateLiveInvoiceProjection()
        If isProcessingLoad Then Return ' Prevent interference during data loading
        If lblInvoiceNumber.Focused Then Return ' Don't overwrite if user is actively typing
        ' Determine what the current prefix SHOULD be based on UI settings
        Dim currentPrefix As String = ""
        If ComboBox1.Text = "Quote" Then
            currentPrefix = "QT"
        ElseIf Module1.IsRgrModeActive Then
            currentPrefix = "RGR"
        ElseIf CheckBoxIsVat.Checked Then
            currentPrefix = "VT"
            lblInvoiceNumber.ReadOnly = False
        Else
            currentPrefix = If(isElBill, "EL", "GR")
            lblInvoiceNumber.ReadOnly = True
        End If

        If isEditingHistory Then
            ' If the current UI settings match the type of the loaded history item, 
            ' we keep the original invoice number.
            If loadedHistoryInvNo.StartsWith(currentPrefix) Then
                If lblInvoiceNumber.Text <> loadedHistoryInvNo Then
                    lblInvoiceNumber.Text = loadedHistoryInvNo
                End If
                Return
            End If
            ' If the user has changed settings (e.g., unchecked VAT or changed Quote to Sale),
            ' we allow the projection logic below to show the NEW projected number.
        End If

        Dim openedHere As Boolean = False
        Dim typeStr As String
        Dim prefix As String
        Dim tableName As String

        typeStr = currentPrefix
        prefix = currentPrefix
        If currentPrefix = "QT" Then
            tableName = "inv_no_qt1"
        ElseIf currentPrefix = "RGR" Then
            tableName = "inv_no_RGR1"
        ElseIf currentPrefix = "VT" Then
            tableName = "inv_no_VT1"
        Else
            tableName = If(isElBill, "inv_no_el1", "inv_no_gr1")
        End If

        Try
            If conn.State = ConnectionState.Closed Then
                conn.Open()
                openedHere = True
            End If

            ' 1. Get Max ID from sequence tables
            Dim localSql = "SELECT MAX(id) FROM " & tableName
            Using localCmd As New MySqlCommand(localSql, conn)
                Dim maxRes = localCmd.ExecuteScalar()
                Dim lastId As Integer = If(maxRes Is DBNull.Value OrElse maxRes Is Nothing, 0, Convert.ToInt32(maxRes))

                ' 2. Count active windows of same type with HIGHER priority (LOWER SlotID)
                Dim countSql = "SELECT COUNT(*) FROM window_sessions WHERE current_type = @type AND is_used = 1 AND slot_id < @slot"
                Using cmdCount As New MySqlCommand(countSql, conn)
                    cmdCount.Parameters.AddWithValue("@type", typeStr)
                    cmdCount.Parameters.AddWithValue("@slot", SlotID)
                    Dim countPreceding = Convert.ToInt32(cmdCount.ExecuteScalar())

                    ' 3. Calculate Projected No
                    Dim projectedId As Integer = lastId + countPreceding + 1

                    ' 4. Update UI if changed
                    Dim newText As String = prefix & projectedId.ToString("D5")
                    If lblInvoiceNumber.Text <> newText Then
                        Dim shouldUpdate As Boolean = True
                        If CheckBoxIsVat.Checked AndAlso lblInvoiceNumber.Modified AndAlso lblInvoiceNumber.Text.Trim() <> "" Then
                            shouldUpdate = False
                        End If

                        If shouldUpdate Then
                            lblInvoiceNumber.Text = newText
                            ' Visual feedback: Color flash
                            lblInvoiceNumber.ForeColor = Color.Red
                            Dim t As New Timer() With {.Interval = 1000} ' Slightly longer flash
                            AddHandler t.Tick, Sub(senderObj, eventArgs)
                                                   lblInvoiceNumber.ForeColor = Color.Black
                                                   Dim timerRef = DirectCast(senderObj, Timer)
                                                   timerRef.Stop()
                                                   timerRef.Dispose()
                                               End Sub
                            t.Start()
                        End If
                    End If
                End Using
            End Using

        Catch ex As Exception
            ' Log or ignore
        Finally
            If openedHere AndAlso conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub InitializeBillTable()
        dtBill = New DataTable()
        dtBill.Columns.Add("No") ' Row Number column
        dtBill.Columns.Add("Item ID")
        dtBill.Columns.Add("Description")
        dtBill.Columns.Add("Qty", GetType(Decimal))
        dtBill.Columns.Add("Selling Price", GetType(Decimal))
        dtBill.Columns.Add("Dis", GetType(Decimal))
        dtBill.Columns.Add("Location", GetType(String))
        dtBill.Columns.Add("Total/Amount", GetType(Decimal))
        dtBill.Columns.Add("VAT")
        dtBill.Columns.Add("ItemCost") ' Hidden column for batch cost (Accounting)
        dtBill.Columns.Add("AvgCost") ' Hidden column for average cost (Validation)
        dtBill.Columns.Add("LocationID") ' Hidden column for direct stock query
        dtBill.Columns.Add("Reason") ' Per-item return reason
        dtBill.Columns.Add("IsOriginal", GetType(Boolean)) ' Track if item is from original invoice
        dtBill.Columns.Add("PrintRetailPrice", GetType(Decimal))

        DataGridView2.DataSource = dtBill
        DataGridView2.RowHeadersVisible = False

        ' Positioning: Align DataGridView2 with the entry line boxes (starts at X=16)
        DataGridView2.Location = New Point(16, DataGridView2.Location.Y)

        FormatBillGrid()
    End Sub

    Private Sub FormatBillGrid()
        ' Enhancement: Set Header Height, Column Widths, and Colors
        DataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridView2.ColumnHeadersHeight = 40
        DataGridView2.EnableHeadersVisualStyles = False
        DataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black
        DataGridView2.ColumnHeadersVisible = True

        ' Parallel Alignment: Synchronize column widths with entry boxes above
        If DataGridView2.Columns("No") IsNot Nothing Then
            DataGridView2.Columns("No").Width = 40
            DataGridView2.Columns("No").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        End If
        If DataGridView2.Columns("Item ID") IsNot Nothing Then
            DataGridView2.Columns("Item ID").Width = txtItemID.Width + (txtDescription.Left - txtItemID.Right) + 5
        End If
        If DataGridView2.Columns("Description") IsNot Nothing Then
            DataGridView2.Columns("Description").Width = txtDescription.Width + (txtQuantity.Left - txtDescription.Right) + 2
        End If
        If DataGridView2.Columns("Qty") IsNot Nothing Then
            DataGridView2.Columns("Qty").Width = txtQuantity.Width + (txtSellingPrice1.Left - txtQuantity.Right) + 2
        End If
        If DataGridView2.Columns("Selling Price") IsNot Nothing Then
            DataGridView2.Columns("Selling Price").Width = txtSellingPrice1.Width + (txtDiscount.Left - txtSellingPrice1.Right) + 2
            DataGridView2.Columns("Selling Price").DefaultCellStyle.Format = "N2"
        End If
        If DataGridView2.Columns("Dis") IsNot Nothing Then
            DataGridView2.Columns("Dis").Width = txtDiscount.Width + 2
        End If
        If DataGridView2.Columns("Location") IsNot Nothing Then
            DataGridView2.Columns("Location").Width = (txtItemDiscountVal.Left - txtDiscount.Right) + 150 ' Give it some readable space
        End If
        If DataGridView2.Columns("Total/Amount") IsNot Nothing Then
            DataGridView2.Columns("Total/Amount").Width = txtItemDiscountVal.Width + (ComboBoxVat.Left - txtItemDiscountVal.Right) - 150
            DataGridView2.Columns("Total/Amount").DefaultCellStyle.Format = "N2"
        End If
        If DataGridView2.Columns("VAT") IsNot Nothing Then
            DataGridView2.Columns("VAT").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        End If
        If DataGridView2.Columns("ItemCost") IsNot Nothing Then DataGridView2.Columns("ItemCost").Visible = False
        If DataGridView2.Columns("PrintRetailPrice") IsNot Nothing Then DataGridView2.Columns("PrintRetailPrice").Visible = False
        If DataGridView2.Columns("AvgCost") IsNot Nothing Then DataGridView2.Columns("AvgCost").Visible = False
        If DataGridView2.Columns("LocationID") IsNot Nothing Then DataGridView2.Columns("LocationID").Visible = False
        If DataGridView2.Columns("Reason") IsNot Nothing Then DataGridView2.Columns("Reason").Visible = False
        If DataGridView2.Columns("IsOriginal") IsNot Nothing Then DataGridView2.Columns("IsOriginal").Visible = False

        ' Disable sorting on all columns to maintain natural sequential order and correct 'No' indexing
        For Each col As DataGridViewColumn In DataGridView2.Columns
            col.SortMode = DataGridViewColumnSortMode.NotSortable
        Next
    End Sub

    Private Function FormatQuantity(qtyVal As Object) As String
        If qtyVal Is Nothing OrElse IsDBNull(qtyVal) Then Return ""
        Dim qty As Decimal
        If Decimal.TryParse(qtyVal.ToString(), qty) Then
            If qty = Math.Truncate(qty) Then
                Return qty.ToString("0")
            Else
                Return qty.ToString("0.00")
            End If
        End If
        Return qtyVal.ToString()
    End Function

    Private Sub FormatQtyInput()
        Dim qtyVal As Decimal
        If Decimal.TryParse(txtQuantity.Text, qtyVal) Then
            If qtyVal = Math.Truncate(qtyVal) Then
                txtQuantity.Text = qtyVal.ToString("0")
            Else
                txtQuantity.Text = qtyVal.ToString("0.00")
            End If
        End If
    End Sub

    Private Sub DataGridView2_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles DataGridView2.CellFormatting
        If DataGridView2.Columns(e.ColumnIndex).Name = "Qty" AndAlso e.Value IsNot Nothing AndAlso Not IsDBNull(e.Value) Then
            Dim val As Decimal
            If Decimal.TryParse(e.Value.ToString(), val) Then
                If val = Math.Truncate(val) Then
                    e.Value = val.ToString("0")
                Else
                    e.Value = val.ToString("N2")
                End If
                e.FormattingApplied = True
            End If
        End If
    End Sub

    Private Sub LoadCashiers()
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()
            ' Load ALL users so anyone can process a bill by entering their password
            Dim localSql = "SELECT u.id, u.name, u.password, u.status, r.role_name, fr.f_role_name FROM user u " &
                           "LEFT JOIN user_role r ON u.role_id = r.id " &
                           "LEFT JOIN financial_role fr ON u.financial_role_id = fr.id " &
                           "WHERE (u.status IS NULL OR u.status = 'active') " &
                           "ORDER BY u.name"
            Using localCmd As New MySqlCommand(localSql, conn)
                Dim da As New MySqlDataAdapter(localCmd)
                Dim dt As New DataTable()
                da.Fill(dt)

                cmbCashier.DataSource = dt
                cmbCashier.DisplayMember = "name"
                cmbCashier.ValueMember = "id"

                ' Default to logged-in user
                If Not String.IsNullOrEmpty(Module1.UserName) Then
                    cmbCashier.Text = Module1.UserName
                End If

                ' Unlock the combo box so it can change based on the password entered
                cmbCashier.Enabled = True
            End Using
            conn.Close()
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    ''' <summary>
    ''' Automatically selects the cashier in cmbCashier by matching the password in txtCashierID.
    ''' </summary>
    Private Function IdentifyCashierByPassword() As Boolean
        Dim pass As String = txtCashierID.Text.Trim()
        If String.IsNullOrEmpty(pass) Then Return False

        Try
            Dim dt As DataTable = DirectCast(cmbCashier.DataSource, DataTable)
            If dt IsNot Nothing Then
                For Each row As DataRow In dt.Rows
                    If row("password").ToString() = pass Then
                        ' Check if the user is deleted or blocked
                        Dim userStatus As String = If(row("status") Is DBNull.Value, "", row("status").ToString().ToLower())
                        If userStatus = "deleted" OrElse userStatus = "blocked" Then
                            MessageBox.Show("Your account has been " & userStatus & ". You cannot perform any sales transactions.", "Account Restricted", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                            Return False
                        End If

                        cmbCashier.SelectedValue = row("id")

                        ' Update role access for the newly entered cashier
                        If dt.Columns.Contains("f_role_name") AndAlso dt.Columns.Contains("role_name") Then
                            Dim newRole As String = If(row("role_name") Is DBNull.Value, "Cashier", row("role_name").ToString())
                            Dim newFRole As String = If(row("f_role_name") Is DBNull.Value, "Standard Cashier", row("f_role_name").ToString())

                            ' Update global context so that any actions requiring role checks respond properly
                            Module1.UserRole = newRole
                            Module1.FinancialRole = newFRole
                            Module1.UserName = row("name").ToString()
                            Module1.CurrentUserID = Convert.ToInt32(row("id"))

                            ' Update ownership of the active slot session in DB to this new user
                            If SlotID > 0 Then
                                Try
                                    Using localConn As New MySqlConnection(ConnStr)
                                        localConn.Open()
                                        Dim updateSql = "UPDATE window_sessions SET user_name = @uname WHERE slot_id = @slot"
                                        Using cmd As New MySqlCommand(updateSql, localConn)
                                            cmd.Parameters.AddWithValue("@uname", Module1.UserName)
                                            cmd.Parameters.AddWithValue("@slot", SlotID)
                                            cmd.ExecuteNonQuery()
                                        End Using
                                    End Using
                                Catch ex As Exception
                                    ' Silent fail
                                End Try
                                LoadComboBoxSlots()
                            End If

                            ' Apply the new role's UI settings to this window immediately
                            ApplyRoleBasedUI()

                            ' Force the main Start menu to update and restrict access (e.g. hide Daily Sales)
                            If Application.OpenForms.OfType(Of Start)().Any() Then
                                Application.OpenForms.OfType(Of Start)().First().ApplyPermissions()
                            End If
                        End If

                        Return True
                    End If
                Next
            End If
        Catch
        End Try
        Return False
    End Function

    Private Sub cmbCashier_KeyDown(sender As Object, e As KeyEventArgs) Handles cmbCashier.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                ComboBoxLocation.Focus()
            Else
                txtCashierID.Focus()
            End If
            e.SuppressKeyPress = True ' Prevent ding sound
        End If
    End Sub

    Private Sub txtCashierID_KeyDown(sender As Object, e As KeyEventArgs) Handles txtCashierID.KeyDown
        If e.KeyCode = Keys.Enter Then
            ' Attempt to identify the cashier immediately when Enter is pressed
            IdentifyCashierByPassword()

            If e.Shift Then
                cmbCashier.Focus()
            Else
                ' User requested: cursor move to item id after entering password
                txtItemID.Focus()
            End If
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub txtCashierID_Leave(sender As Object, e As EventArgs) Handles txtCashierID.Leave
        ' Also attempt identification when the user leaves the password field
        IdentifyCashierByPassword()
    End Sub


    Private Sub txtCustomerAddress_KeyDown(sender As Object, e As KeyEventArgs) Handles txtCustomerAddress.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                txtCustomerPhone.Focus()
            Else
                txtItemID.Focus()
            End If
            e.SuppressKeyPress = True
        End If
    End Sub


    Private Sub btnAddCustomer_Click(sender As Object, e As EventArgs) Handles btnAddCustomer.Click
        Dim cusForm As New customer_add()
        cusForm.ShowDialog()
    End Sub

    Private Sub LoadInvoiceNumber()
        UpdateLiveInvoiceProjection()
    End Sub

    Private Sub ToggleBillType()
        ' Toggle Logic
        isElBill = Not isElBill

        If isElBill Then
            btnEl.Text = "EL" ' Show EL when in EL mode
        Else
            btnEl.Text = "GR" ' Show GR when in GR mode
        End If

        UpdateSessionTypeInDB()

        ' If transitioning to GR, clear the billing type, payment method, and default Cash customer fields
        If Not isElBill Then
            cmbBillingType.SelectedIndex = -1
            txtPaymentMethod.Text = ""

            If String.Equals(txtSalesRep.Text.Trim(), "Cash", StringComparison.OrdinalIgnoreCase) Then
                txtSalesRep.Text = ""
            End If
            If String.Equals(txtCustomerPhone.Text.Trim(), "Cash", StringComparison.OrdinalIgnoreCase) Then
                txtCustomerPhone.Text = ""
            End If
            If String.Equals(txtCustomerAddress.Text.Trim(), "Cash", StringComparison.OrdinalIgnoreCase) Then
                txtCustomerAddress.Text = ""
            End If
        End If

        ApplyRoleBasedUI()
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        ' WHOLESALE STOCK ALERT VALIDATION (5 QTY LIMIT)
        If ComboBox1.Text <> "Quote" AndAlso CheckBoxWholesale.Checked AndAlso dtBill.Rows.Count > 0 Then
            ' Group items by Item ID and Location
            Dim itemTotals As New Dictionary(Of String, Decimal)()
            Dim itemLocations As New Dictionary(Of String, String)()
            
            For Each row As DataRow In dtBill.Rows
                Dim itemId As String = row("Item ID").ToString().Trim()
                Dim loc As String = row("Location").ToString().Trim()
                If String.IsNullOrEmpty(loc) Then loc = "MAIN STOCK"
                
                Dim qtyVal As Decimal = 0
                Decimal.TryParse(row("Qty").ToString(), qtyVal)
                
                ' We only validate positive sale quantities
                If qtyVal > 0 Then
                    Dim key As String = itemId & "|" & loc
                    If itemTotals.ContainsKey(key) Then
                        itemTotals(key) += qtyVal
                    Else
                        itemTotals(key) = qtyVal
                        itemLocations(key) = loc
                    End If
                End If
            Next
            
            ' Perform stock alert validation for each group
            For Each kvp In itemTotals
                Dim parts = kvp.Key.Split("|"c)
                Dim itemId = parts(0)
                Dim loc = itemLocations(kvp.Key)
                Dim totalQty = kvp.Value
                
                Dim currentStock As Decimal = GetCurrentStock(itemId, loc)
                If currentStock > 0 Then
                    If currentStock < 5 Then
                        Dim result As DialogResult = MessageBox.Show("ප්‍රමාණවත් ප්‍රමාණයක් නොමැත. එසේ වුවද බිල save කිරීමට අවශ්‍යද? (අයිතමය: " & itemId & ")", "තොග ප්‍රමාණවත් නොවේ", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                        If result = DialogResult.No Then
                            For i As Integer = 0 To DataGridView2.Rows.Count - 1
                                Dim gridItemId As String = DataGridView2.Rows(i).Cells("Item ID").Value.ToString().Trim()
                                If String.Equals(gridItemId, itemId, StringComparison.OrdinalIgnoreCase) Then
                                    DataGridView2.ClearSelection()
                                    DataGridView2.Rows(i).Selected = True
                                    DataGridView2.CurrentCell = DataGridView2.Rows(i).Cells("Item ID")

                                    Dim rowView As DataRowView = DirectCast(DataGridView2.Rows(i).DataBoundItem, DataRowView)
                                    selectedIndex = dtBill.Rows.IndexOf(rowView.Row)

                                    txtItemID.Text = DataGridView2.Rows(i).Cells("Item ID").Value.ToString()
                                    txtDescription.Text = DataGridView2.Rows(i).Cells("Description").Value.ToString()
                                    txtAmount.Text = DataGridView2.Rows(i).Cells("Selling Price").Value.ToString()
                                    txtDiscount.Text = DataGridView2.Rows(i).Cells("Dis").Value.ToString()
                                    txtSellingPrice1.Tag = DataGridView2.Rows(i).Cells("ItemCost").Value.ToString()

                                    FetchItemByID(txtItemID.Text.Trim())
                                    btnUpdate.Text = "Edit"
                                    btnUpdate.BackColor = Color.White

                                    txtSellingPrice1.Text = DataGridView2.Rows(i).Cells("Selling Price").Value.ToString()
                                    txtDiscount.Text = DataGridView2.Rows(i).Cells("Dis").Value.ToString()
                                    txtQuantity.Text = FormatQuantity(DataGridView2.Rows(i).Cells("Qty").Value)

                                    txtQuantity.Focus()
                                    txtQuantity.SelectAll()
                                    Exit For
                                End If
                            Next
                            Return
                        End If
                    Else
                        Dim maxAllowed As Decimal = currentStock - 5
                        If totalQty > maxAllowed Then
                            Dim result As DialogResult = MessageBox.Show("මෙම අයිතමයෙන් ඔබට ලබාගත හැක්කේ " & maxAllowed.ToString("G") & " ක් පමණි. එසේ වුවද බිල save කිරීමට අවශ්‍යද? (අයිතමය: " & itemId & ")", "සීමාව ඉක්මවා ඇත", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                            If result = DialogResult.No Then
                                For i As Integer = 0 To DataGridView2.Rows.Count - 1
                                    Dim gridItemId As String = DataGridView2.Rows(i).Cells("Item ID").Value.ToString().Trim()
                                    If String.Equals(gridItemId, itemId, StringComparison.OrdinalIgnoreCase) Then
                                        DataGridView2.ClearSelection()
                                        DataGridView2.Rows(i).Selected = True
                                        DataGridView2.CurrentCell = DataGridView2.Rows(i).Cells("Item ID")

                                        Dim rowView As DataRowView = DirectCast(DataGridView2.Rows(i).DataBoundItem, DataRowView)
                                        selectedIndex = dtBill.Rows.IndexOf(rowView.Row)

                                        txtItemID.Text = DataGridView2.Rows(i).Cells("Item ID").Value.ToString()
                                        txtDescription.Text = DataGridView2.Rows(i).Cells("Description").Value.ToString()
                                        txtAmount.Text = DataGridView2.Rows(i).Cells("Selling Price").Value.ToString()
                                        txtDiscount.Text = DataGridView2.Rows(i).Cells("Dis").Value.ToString()
                                        txtSellingPrice1.Tag = DataGridView2.Rows(i).Cells("ItemCost").Value.ToString()

                                        FetchItemByID(txtItemID.Text.Trim())
                                        btnUpdate.Text = "Edit"
                                        btnUpdate.BackColor = Color.White

                                        txtSellingPrice1.Text = DataGridView2.Rows(i).Cells("Selling Price").Value.ToString()
                                        txtDiscount.Text = DataGridView2.Rows(i).Cells("Dis").Value.ToString()
                                        txtQuantity.Text = FormatQuantity(DataGridView2.Rows(i).Cells("Qty").Value)

                                        txtQuantity.Focus()
                                        txtQuantity.SelectAll()
                                        Exit For
                                    End If
                                Next
                                Return
                            End If
                        End If
                    End If
                End If
            Next
        End If

        ' Try to identify by password first
        IdentifyCashierByPassword()

        ' Check if the active cashier is admin1/2/3/4
        Dim currentCashier As String = cmbCashier.Text.Trim().ToLower()
        If currentCashier = "admin1" OrElse currentCashier = "admin2" OrElse currentCashier = "admin3" OrElse currentCashier = "admin4" Then
            MessageBox.Show("Please enter a valid Cashier ID", "Invalid Cashier", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtCashierID.Focus()
            txtCashierID.SelectAll()
            Return
        End If

        ' Validate Print R is only checked when Wholesale is checked
        If CheckBoxPrintAsRetail.Checked AndAlso Not CheckBoxWholesale.Checked Then
            MessageBox.Show("Please select Wholesale check box first to Print as Retail.", "Wholesale Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            CheckBoxWholesale.Focus()
            Return
        End If

        If dtBill.Rows.Count = 0 Then
            MessageBox.Show("Please add at least one item to the bill before saving.", "Empty Bill", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtItemID.Focus()
            txtItemID.SelectAll()
        Else
            ' Automatically switch EL to GR if total bill amount is > 5000 and it's not history editing, quote, vat, or RGR mode
            Dim tempGrandTotal As Decimal = 0
            Decimal.TryParse(lblGrandTotal.Text.Replace(",", ""), tempGrandTotal)
            If Not isEditingHistory AndAlso ComboBox1.Text <> "Quote" AndAlso Not CheckBoxIsVat.Checked AndAlso Not Module1.IsRgrModeActive AndAlso isElBill AndAlso tempGrandTotal > 5000 Then
                isElBill = False
                btnEl.Text = "GR"
                UpdateSessionTypeInDB()
                UpdateLiveInvoiceProjection()
                ApplyRoleBasedUI()
            End If

            If Not lblInvoiceNumber.Text.Trim().StartsWith("EL", StringComparison.OrdinalIgnoreCase) Then
                If MessageBox.Show("Do you want to save the bill?", "Confirm Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                    Return
                End If
            End If

            ' 1. Validate Cashier and Password
            If Not IdentifyCashierByPassword() Then
                MessageBox.Show("Please enter a valid Cashier ID", "Authorization Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtCashierID.Focus()
                txtCashierID.SelectAll()
                Return
            End If

            ' Final confirmation check (redundant but safe)
            If cmbCashier.SelectedIndex = -1 Then
                MessageBox.Show("Please enter a valid Cashier ID", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            ' Rest of saving logic...
            Dim isQuote As Boolean = (ComboBox1.Text = "Quote")

            ' 1. Validate Billing Type selection
            If Not isQuote AndAlso String.IsNullOrWhiteSpace(cmbBillingType.Text) Then
                MessageBox.Show("Please select a Billing Type before saving.", "Billing Type Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                cmbBillingType.Focus()
                Return
            End If

            ' 2. Validate Payment Method selection
            ' SKIP validation for PENDING billing type
            Dim currentBT As String = cmbBillingType.Text.Trim().ToUpper()
            Dim isPendingBT As Boolean = (currentBT = "PENDING")

            If Not isQuote AndAlso Not isPendingBT AndAlso String.IsNullOrWhiteSpace(txtPaymentMethod.Text) Then
                If isElBill Then
                    txtPaymentMethod.Text = "Cash"
                    UpdatePaymentAndBalance()
                Else
                    MessageBox.Show("Please select a Payment Method before saving.", "Payment Method Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtPaymentMethod.Focus()
                    Return
                End If
            End If

            ' Validate Customer Selection (Mandatory only for Credit)
            Dim s_billingType As String = cmbBillingType.Text.Trim()
            If Not isQuote AndAlso String.Equals(s_billingType, "Credit", StringComparison.OrdinalIgnoreCase) AndAlso (String.IsNullOrEmpty(txtSalesRep.Text.Trim()) OrElse txtSalesRep.Text = "") Then
                MessageBox.Show("Please select a real Customer for Credit bills.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSalesRep.Focus()
                txtSalesRep.SelectAll()
            Else
                ' Final check before password validation
                ' Only CREDIT bills strictly require a customer ID.
                ' Cash bills save with NULL customer_id if no "Cash" record exists in DB.
                ' Quotation bills also don't require one.
                If Not isQuote AndAlso String.IsNullOrEmpty(selectedCustomerId) AndAlso String.Equals(s_billingType, "Credit", StringComparison.OrdinalIgnoreCase) Then
                    MessageBox.Show("Unable to identify Customer ID. Please re-select the customer from the search list.", "ID Missing", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    txtSalesRep.Focus()
                    Return
                End If

                ' Final Blocked Customer Check
                If Not String.IsNullOrEmpty(selectedCustomerId) Then
                    Try
                        Using blockCheckConn As New MySqlConnection(ConnStr)
                            blockCheckConn.Open()
                            Using cmdBlock = New MySqlCommand("SELECT is_block FROM customer WHERE id = @id", blockCheckConn)
                                cmdBlock.Parameters.AddWithValue("@id", selectedCustomerId)
                                Dim isBlockedVal = Convert.ToInt32(If(cmdBlock.ExecuteScalar(), 0))
                                If isBlockedVal = 1 Then
                                    MessageBox.Show("This customer is blocked and cannot receive any more bills." & vbCrLf & "Please resolve their account status in the Customer module first.", "Customer Blocked", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                                    Return
                                End If
                            End Using
                        End Using
                    Catch ex As Exception
                        ' If DB check fails, we proceed but log/warn? For now, just allow or log.
                        ' Given the strict request, we could also abort if DB check fails.
                    End Try
                End If

                ' Cheque validation rules
                Dim paymentMethodText As String = If(txtPaymentMethod.Text Is Nothing, "", txtPaymentMethod.Text.Trim())
                If Not isQuote AndAlso String.Equals(s_billingType, "Cheque", StringComparison.OrdinalIgnoreCase) Then
                    If Not String.Equals(paymentMethodText, "Cheque", StringComparison.OrdinalIgnoreCase) Then
                        MessageBox.Show("When Billing Type is Cheque, Payment Method must also be Cheque.", "Invalid Combination", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtPaymentMethod.Focus()
                        txtPaymentMethod.SelectAll()
                        Return
                    End If
                    ' Legacy Cheque.Text validation has been removed because ChequeEntryDialog now handles this exclusively.
                End If

                ' Customer validation based on Billing Type
                Dim isCashCustomer As Boolean = (txtSalesRep.Text.Trim().ToLower() = "cash") OrElse String.IsNullOrEmpty(selectedCustomerId)

                ' Rule: Only Credit and Cheque bills strictly require a real (non-Cash) customer.
                ' PENDING and Cash bills are allowed to use the 'Cash' customer.
                Dim isRealDebtType As Boolean = String.Equals(s_billingType, "Credit", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(s_billingType, "Cheque", StringComparison.OrdinalIgnoreCase)

                If Not isQuote AndAlso isRealDebtType Then
                    ' Rule: Non-Cash Billing (Credit/Cheque) -> NO Cash Customer
                    If isCashCustomer Then
                        MessageBox.Show("Credit or Cheque bills require a real customer. 'Cash' customer or empty selection is not allowed for credit transactions.", "Customer Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtSalesRep.Focus()
                        Return
                    End If
                End If

                ' Rule: Mandatory PO for Credit
                If Not isQuote AndAlso String.Equals(s_billingType, "Credit", StringComparison.OrdinalIgnoreCase) Then
                    ' P/O Number validation disabled as per user request
                    ' If String.IsNullOrWhiteSpace(TextBoxPO.Text) Then
                    '     MessageBox.Show("P/O Number is mandatory for Credit bills. Please enter a valid P/O number before saving.", "PO Number Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    '     TextBoxPO.Focus()
                    '     Return
                    ' End If

                    ' Credit Limit Validation
                    Dim limitVal As Decimal = 0
                    Decimal.TryParse(txtCreditLimit.Text, limitVal)
                    If limitVal > 0 Then
                        Dim currentOutstanding As Decimal = 0
                        Dim newCreditAmount As Decimal = 0
                        Decimal.TryParse(lblAccountOutstanding.Text, currentOutstanding)
                        Decimal.TryParse(lblCreditBalance.Text, newCreditAmount)

                        If (currentOutstanding + newCreditAmount) > limitVal Then
                            MessageBox.Show("This customer has exceeded their credit limit." & vbCrLf &
                                            "Credit Limit: " & limitVal.ToString("N2") & vbCrLf &
                                            "Current Debt: " & currentOutstanding.ToString("N2") & vbCrLf &
                                            "New Bill Credit: " & newCreditAmount.ToString("N2") & vbCrLf &
                                            "Total Project Debt: " & (currentOutstanding + newCreditAmount).ToString("N2"),
                                            "Credit Limit Exceeded", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                            Return
                        End If
                    End If

                    ' Credit Period Validation (Expiry Check)
                    If dtpCreditPeriod.Value.Date < DateTime.Today Then
                        MessageBox.Show("This customer's credit period has expired (Date: " & dtpCreditPeriod.Value.ToString("yyyy-MM-dd") & ")." & vbCrLf &
                                        "Please update the credit period or settle outstanding debts before proceeding with a new credit bill.",
                                        "Credit Period Expired", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                        Return
                    End If
                End If

                ' Cash validation rules (Skip for Order Taker, as they can't close)
                If Module1.FinancialRole.ToLower() <> "order taker" AndAlso Not isQuote AndAlso String.Equals(s_billingType, "Cash", StringComparison.OrdinalIgnoreCase) Then
                    Dim cashAmtVal As Decimal = 0
                    Decimal.TryParse(txtCashAmount.Text, cashAmtVal)
                    Dim advPayVal As Decimal = 0
                    Decimal.TryParse(txtAdvPay.Text, advPayVal)

                    Dim currentGrandTotalVal As Decimal = 0
                    Decimal.TryParse(lblGrandTotal.Text.Replace(",", ""), currentGrandTotalVal)

                    If (cashAmtVal + advPayVal) <= 0 Then
                        MessageBox.Show("Payment cannot do please enter paid amount", "Payment Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtCashAmount.Focus()
                        txtCashAmount.SelectAll()
                        Return
                    ElseIf String.Equals(paymentMethodText, "Cash", StringComparison.OrdinalIgnoreCase) AndAlso (cashAmtVal + advPayVal) < currentGrandTotalVal Then
                        MessageBox.Show("Paid amount cannot be less than the Grand Total (" & currentGrandTotalVal.ToString("N2") & ") for Cash payments.", "Insufficient Payment", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtCashAmount.Focus()
                        txtCashAmount.SelectAll()
                        Return
                    Else
                        If String.IsNullOrWhiteSpace(paymentMethodText) Then
                            MessageBox.Show("Please select a Payment Method (Cash, Card, Online, etc.) for Cash bills.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            txtPaymentMethod.Focus()
                            txtPaymentMethod.SelectAll()
                            Return
                        End If
                    End If
                End If


                ' Validate Cashier and Password
                If cmbCashier.SelectedIndex = -1 OrElse String.IsNullOrEmpty(cmbCashier.Text) Then
                    MessageBox.Show("Please enter a valid Cashier ID", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    cmbCashier.Focus()
                    cmbCashier.Select()
                Else
                    If String.IsNullOrEmpty(txtCashierID.Text.Trim()) Then
                        MessageBox.Show("Please enter a valid Cashier ID", "Password Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtCashierID.Focus()
                        txtCashierID.SelectAll()
                    Else
                        ' Verify Password against DB
                        Dim rowCashier As DataRowView = DirectCast(cmbCashier.SelectedItem, DataRowView)
                        If rowCashier("password").ToString() <> txtCashierID.Text.Trim() Then
                            MessageBox.Show("Please enter a valid Cashier ID", "Unauthorized", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            txtCashierID.Focus()
                            txtCashierID.SelectAll()
                        Else
                            ' --- Bill Level Profit Validation (Using Avg Cost) ---
                            Dim totalBillAvgCost As Decimal = 0
                            For Each rowProfit As DataRow In dtBill.Rows
                                Dim qVal As Decimal = 0
                                Dim aVal As Decimal = 0
                                If rowProfit("Qty") IsNot DBNull.Value Then Decimal.TryParse(rowProfit("Qty").ToString(), qVal)
                                If dtBill.Columns.Contains("AvgCost") Then
                                    If rowProfit("AvgCost") IsNot DBNull.Value Then Decimal.TryParse(rowProfit("AvgCost").ToString(), aVal)
                                Else
                                    If rowProfit("ItemCost") IsNot DBNull.Value Then Decimal.TryParse(rowProfit("ItemCost").ToString(), aVal)
                                End If
                                totalBillAvgCost += (qVal * aVal)
                            Next

                            Dim currentGrandTotal As Decimal = 0
                            Decimal.TryParse(lblGrandTotal.Text.Replace(",", ""), currentGrandTotal)

                            ' Standard Profit Validation (Skipped for Returns/Refunds)
                            If currentGrandTotal > 0 AndAlso currentGrandTotal <= totalBillAvgCost Then
                                MessageBox.Show(
                                    "Total Bill Amount (" & currentGrandTotal.ToString("N2") & ") is not higher than total average cost (" & totalBillAvgCost.ToString("N2") & ")." & vbCrLf &
                                    "Saving this bill would result in a loss or zero profit.",
                                    "Bill Level Profit Check Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                Return
                            End If

                            ' 1. Insert into Main Billing Table
                            Dim billType As String = "Normal"
                            Dim mainTable As String = "billing"
                            Dim itemTable As String = "billing_item"
                            Dim invNoTable As String = If(isElBill, "inv_no_el1", "inv_no_gr1")
                            Dim specificInvType As String = "Normal"

                            ' --- Role Based Bypass for Update Dialogs ---
                            Dim financialRoleLower As String = Module1.FinancialRole.ToLower()
                            Dim isOrderTaker As Boolean = (financialRoleLower = "seller" OrElse financialRoleLower = "order taker") AndAlso Not (Module1.UserRole IsNot Nothing AndAlso Module1.UserRole.ToLower() = "cashier")

                            ' Determine Bill Type and Tables
                            If ComboBox1.Text = "Quote" Then
                                billType = "Quote"
                                specificInvType = "Quote"
                                mainTable = "quotation_billing"
                                itemTable = "quotation_billing_item"
                            ElseIf CheckBoxWholesale.Checked Then
                                billType = "Wholesale"
                                specificInvType = "Wholesale"
                                mainTable = "billing"
                                itemTable = "billing_item"
                            ElseIf CheckBoxRetail.Checked Then
                                billType = "Retail"
                                specificInvType = "Retail"
                                mainTable = "billing"
                                itemTable = "billing_item"
                            Else
                                billType = "Normal"
                                specificInvType = "Normal"
                                mainTable = "billing"
                                itemTable = "billing_item"
                            End If

                            Dim dbTrans As MySqlTransaction = Nothing
                            Dim originalSelectedCustomerId As String = selectedCustomerId
                            Dim originalSalesRepText As String = txtSalesRep.Text
                            Try
                                If conn.State = ConnectionState.Closed Then conn.Open()
                                dbTrans = conn.BeginTransaction()

                                ' --- QUICK AUTO-CUSTOMER LOGIC (Enhanced for Name and Phone) ---
                                Dim enteredName As String = txtSalesRep.Text.Trim()
                                Dim enteredPhone As String = txtCustomerPhone.Text.Trim()

                                ' Trigger this logic if no customer is selected (or it's generic ID 1) and something is entered in Name or Phone
                                If (String.IsNullOrEmpty(selectedCustomerId) OrElse selectedCustomerId = "1") AndAlso
                                    (Not String.IsNullOrWhiteSpace(enteredName) AndAlso Not enteredName.Equals("CASH", StringComparison.OrdinalIgnoreCase) OrElse Not String.IsNullOrWhiteSpace(enteredPhone)) Then

                                    Using localConn As New MySqlConnection(ConnStr)
                                        localConn.Open()

                                        ' 1. Try to find by Name (if provided) to avoid duplicates
                                        If Not String.IsNullOrWhiteSpace(enteredName) AndAlso Not enteredName.Equals("CASH", StringComparison.OrdinalIgnoreCase) Then
                                            Dim checkNameSql = "SELECT id FROM customer WHERE name = @name AND id <> 1 LIMIT 1"
                                            Using cmdCheckName As New MySqlCommand(checkNameSql, conn, dbTrans)
                                                cmdCheckName.Parameters.AddWithValue("@name", enteredName)
                                                Dim resId = cmdCheckName.ExecuteScalar()
                                                If resId IsNot Nothing Then
                                                    selectedCustomerId = resId.ToString()
                                                End If
                                            End Using
                                        End If

                                        ' 2. If not found by Name, try to find by Phone (if provided)
                                        If String.IsNullOrEmpty(selectedCustomerId) AndAlso Not String.IsNullOrWhiteSpace(enteredPhone) Then
                                            Dim checkPhoneSql = "SELECT id, name FROM customer WHERE tel_no = @tel AND id <> 1 LIMIT 1"
                                            Using cmdCheckPhone As New MySqlCommand(checkPhoneSql, conn, dbTrans)
                                                cmdCheckPhone.Parameters.AddWithValue("@tel", enteredPhone)
                                                Using drCus = cmdCheckPhone.ExecuteReader()
                                                    If drCus.Read() Then
                                                        selectedCustomerId = drCus("id").ToString()
                                                        txtSalesRep.Text = drCus("name").ToString() ' Sync UI
                                                    End If
                                                End Using
                                            End Using
                                        End If

                                        ' 3. If STILL not found, create a NEW one (Only for Cash Bills or Quote as requested)
                                        Dim currentBT_Check As String = If(String.IsNullOrWhiteSpace(cmbBillingType.Text), "Cash", cmbBillingType.Text).Trim()
                                        Dim isAutoCreateType As Boolean = String.Equals(currentBT_Check, "Cash", StringComparison.OrdinalIgnoreCase) OrElse (ComboBox1.Text = "Quote")

                                        If String.IsNullOrEmpty(selectedCustomerId) AndAlso isAutoCreateType Then
                                            Dim newName As String = enteredName
                                            ' If name is empty, fall back to CASH-XXXXX format using phone
                                            If String.IsNullOrWhiteSpace(newName) OrElse newName.Equals("CASH", StringComparison.OrdinalIgnoreCase) Then
                                                Dim last5Digits As String = If(enteredPhone.Length >= 5, enteredPhone.Substring(enteredPhone.Length - 5), enteredPhone)
                                                newName = "CASH-" & last5Digits
                                            End If

                                            Dim insCusSql = "INSERT INTO customer (name, tel_no, address, customer_type, is_block, vat_id, created_at, timestamps) " &
                                                            "VALUES (@nm, @tel, @addr, 'CASH', 0, 1, @now, @now)"
                                            Using cmdInsCus As New MySqlCommand(insCusSql, conn, dbTrans)
                                                cmdInsCus.Parameters.AddWithValue("@nm", newName)
                                                cmdInsCus.Parameters.AddWithValue("@tel", enteredPhone)
                                                cmdInsCus.Parameters.AddWithValue("@addr", txtCustomerAddress.Text.Trim())
                                                cmdInsCus.Parameters.AddWithValue("@now", DateTime.Now)
                                                cmdInsCus.ExecuteNonQuery()

                                                Dim newId = cmdInsCus.LastInsertedId
                                                If newId > 0 Then
                                                    selectedCustomerId = newId.ToString()
                                                End If
                                                txtSalesRep.Text = newName ' Sync UI
                                            End Using
                                        End If
                                    End Using
                                End If
                                ' ------------------------------------------

                                ' Default to generic CASH (ID 1) if still no customer identified
                                If String.IsNullOrEmpty(selectedCustomerId) Then
                                    selectedCustomerId = "1"
                                End If

                                ' Calculate logic for DB
                                Dim gTotal As Decimal = 0
                                Dim cashAmt As Decimal = 0
                                Decimal.TryParse(lblGrandTotal.Text.Replace(",", ""), gTotal)
                                If Not isQuote Then
                                    Decimal.TryParse(txtCashAmount.Text.Replace(",", ""), cashAmt)
                                End If

                                ' --- DETERMINING IF NEW OR UPDATE (Moved up for Waterfall logic) ---
                                Dim isUpdate As Boolean = False
                                Dim billingId As Long = 0
                                Dim existingInvNo As String = lblInvoiceNumber.Text.Trim()
                                Dim oldGrandTotal As Decimal = 0
                                Dim oldCreditBalance As Decimal = 0
                                Dim oldChequeBalance As Decimal = 0
                                Dim oldCashAmount As Decimal = 0

                                Dim checkSql = "SELECT id, grand_total, credit_balance_due, cheque_balance_due, paid_amount FROM " & mainTable & " WHERE inv_no = @inv LIMIT 1"
                                Using checkCmdInv As New MySqlCommand(checkSql, conn, dbTrans)
                                    checkCmdInv.Parameters.AddWithValue("@inv", existingInvNo)
                                    Using reader As MySqlDataReader = checkCmdInv.ExecuteReader()
                                        If reader.Read() Then
                                            isUpdate = True
                                            billingId = Convert.ToInt64(reader("id"))
                                            oldGrandTotal = Convert.ToDecimal(reader("grand_total"))
                                            oldCreditBalance = If(reader("credit_balance_due") Is DBNull.Value, 0D, Convert.ToDecimal(reader("credit_balance_due")))
                                            oldChequeBalance = If(reader("cheque_balance_due") Is DBNull.Value, 0D, Convert.ToDecimal(reader("cheque_balance_due")))
                                            oldCashAmount = If(reader("paid_amount") Is DBNull.Value, 0D, Convert.ToDecimal(reader("paid_amount")))
                                        End If
                                    End Using
                                End Using

                                ' --- WARNING AND WATERFALL SETTLEMENT (Moved up to intercept standard logic) ---
                                Dim applyToCredit As Decimal = 0
                                Dim applyToStoreCredit As Decimal = 0
                                Dim applyToCash As Decimal = 0

                                If (isEditingHistory OrElse isUpdate) AndAlso Not (isOrderTaker AndAlso isEditingHistory) Then
                                    ' Preliminary status for warning form (will be refined below)
                                    Dim warningForm As New InvoiceUpdateWarning()
                                    warningForm.OriginalBilling = originalBillingType
                                    warningForm.OriginalStatus = originalStatusValue
                                    warningForm.NewBilling = cmbBillingType.Text ' Preliminary
                                    warningForm.NewStatus = "Updating..."
                                    warningForm.OriginalCredit = oldCreditBalance
                                    warningForm.OriginalCheque = oldChequeBalance
                                    warningForm.InvNo = existingInvNo
                                    warningForm.HasReturns = (gTotal < oldGrandTotal)

                                    If warningForm.ShowDialog() = DialogResult.No Then
                                        Return
                                    End If

                                    ' Waterfall Calculation for Returns
                                    If gTotal < oldGrandTotal Then
                                        Dim refundDue As Decimal = oldGrandTotal - gTotal
                                        Dim unclearedChequeTotal As Decimal = oldChequeBalance

                                        Dim settleDlg As New RefundSettlementDialog()
                                        settleDlg.TotalRefundDue = refundDue
                                        settleDlg.UnpaidCredit = If(oldCreditBalance > 0, oldCreditBalance, 0)
                                        settleDlg.UnclearedChequeAmount = unclearedChequeTotal

                                        If settleDlg.ShowDialog() = DialogResult.OK Then
                                            applyToCredit = settleDlg.ApplyToCredit
                                            applyToStoreCredit = settleDlg.ApplyToStoreCredit
                                            applyToCash = settleDlg.ApplyToCash
                                        Else
                                            Return ' Cancelled save due to settlement abort
                                        End If
                                    End If
                                End If

                                ' --- PRESERVE ORIGINAL CASH FOR AUDIT ---
                                Dim originalReceived As Decimal = cashAmt
                                ' ----------------------------------------

                                ' Adjust the tracked cash amount on this bill.
                                ' Any cash given back to the customer (or converted to a voucher) means we hold less cash against this invoice.
                                If isUpdate Then
                                    ' Protect against the user manually clearing the cash textbox to 0 during a return.
                                    ' If the UI cashAmt is less than the old cash amount, we assume they cleared it by mistake.
                                    cashAmt = Math.Max(cashAmt, oldCashAmount) - applyToCash - applyToStoreCredit
                                    cashAmt = Math.Max(0, cashAmt) ' Ensure it doesn't go below 0
                                End If

                                ' --- USER REFINEMENT: Settlement, Status & Triple Split ---
                                Dim bType As String = If(String.IsNullOrWhiteSpace(cmbBillingType.Text), "Cash", cmbBillingType.Text).Trim()
                                Dim pMethod As String = If(String.IsNullOrWhiteSpace(txtPaymentMethod.Text), "Cash", txtPaymentMethod.Text).Trim()

                                ' Auto-switch to Cash if the cash amount covers the entire bill
                                ' (BUT: Preserve PENDING if specifically chosen or forced by return logic)
                                If cashAmt >= gTotal AndAlso gTotal > 0 AndAlso Not String.Equals(bType, "PENDING", StringComparison.OrdinalIgnoreCase) Then
                                    bType = "Cash"
                                End If

                                ' USER APPROVED RULE: If return, FORCE Billing Type and Payment Method to PENDING
                                ' ONLY if the user is a SELLER/ORDER TAKER. 
                                ' (Admins/Others can direct collect return bills if they choose a non-PENDING billing type)
                                If isUpdate AndAlso Not isQuote AndAlso gTotal < oldGrandTotal Then
                                    If isOrderTaker Then
                                        bType = "PENDING"
                                        pMethod = "PENDING"
                                    End If
                                End If
                                Dim statusValue As String = "Paid" ' Default for fully paid
                                Dim chequeNo As String = If(isUpdate, originalChequeNo, "")
                                Dim parsedBankId As Integer = 1
                                If isUpdate AndAlso Not String.IsNullOrEmpty(originalBankId) Then
                                    Integer.TryParse(originalBankId, parsedBankId)
                                End If
                                Dim bankIdValue As Object = parsedBankId
                                Dim finalChequeAmount As Decimal = If(isUpdate, originalChequeAmt, 0)
                                Dim chequeDateValue As DateTime = If(isUpdate AndAlso originalChequeDate <> DateTime.MinValue, originalChequeDate, DateTime.Now)

                                Dim advPay As Decimal = 0
                                Decimal.TryParse(txtAdvPay.Text, advPay)
                                Dim remainingBalance As Decimal = gTotal - (cashAmt + advPay)

                                ' Triple Split Balance Columns
                                Dim chequeBalanceDue As Decimal = 0
                                Dim creditBalanceDue As Decimal = 0
                                Dim partialCash As Decimal = 0

                                If String.Equals(bType, "PENDING", StringComparison.OrdinalIgnoreCase) Then
                                    ' For PENDING billing types, status is always Pending
                                    statusValue = "Pending"
                                    partialCash = cashAmt
                                    creditBalanceDue = Math.Max(0, gTotal - (cashAmt + advPay))
                                ElseIf String.Equals(bType, "Cash", StringComparison.OrdinalIgnoreCase) Then
                                    If Not isQuote AndAlso (cashAmt + advPay) < gTotal Then
                                        ' For Cash bills, if payment is not full, it's Pending
                                        statusValue = "Pending"
                                    Else
                                        statusValue = "Paid"
                                    End If
                                ElseIf String.Equals(bType, "Credit", StringComparison.OrdinalIgnoreCase) Then
                                    If remainingBalance > 0 Then
                                        ' Skip SettlementDialog for Return updates on Credit bills.
                                        ' The payment method is already known - it remains Credit.
                                        ' A new SettlementDialog would be confusing & wrong on a return.
                                        If isUpdate AndAlso gTotal < oldGrandTotal Then
                                            Dim originalPayMethod As String = If(String.IsNullOrWhiteSpace(txtPaymentMethod.Text), "Cash", txtPaymentMethod.Text).Trim()

                                            If String.Equals(originalPayMethod, "Cheque", StringComparison.OrdinalIgnoreCase) Then
                                                Dim isClearedCheque As Boolean = (oldChequeBalance = 0 AndAlso Not String.IsNullOrEmpty(originalChequeNo))

                                                If isClearedCheque Then
                                                    pMethod = "Cheque"
                                                    chequeNo = originalChequeNo
                                                    If Integer.TryParse(originalBankId, Nothing) Then
                                                        bankIdValue = Convert.ToInt32(originalBankId)
                                                    End If
                                                    finalChequeAmount = 0
                                                    statusValue = "Cleared_Cheque"
                                                Else
                                                    Dim chequeDlg As New ChequeEntryDialog()
                                                    chequeDlg.DefaultAmount = remainingBalance

                                                    ' Pass original cheque details
                                                    If Not String.IsNullOrEmpty(originalChequeNo) Then
                                                        chequeDlg.InitialChequeNo = originalChequeNo
                                                        If Integer.TryParse(originalBankId, Nothing) Then
                                                            chequeDlg.InitialBankID = Convert.ToInt32(originalBankId)
                                                        End If
                                                        chequeDlg.InitialDate = originalChequeDate
                                                        If remainingBalance < originalChequeAmt Then
                                                            MessageBox.Show("Cheque Amount Changed")
                                                        End If
                                                        chequeDlg.InitialAmount = Math.Min(originalChequeAmt, remainingBalance)
                                                    End If

                                                    If chequeDlg.ShowDialog() = DialogResult.OK Then
                                                        pMethod = "Cheque"
                                                        chequeNo = chequeDlg.ChequeNo
                                                        bankIdValue = chequeDlg.BankID
                                                        finalChequeAmount = chequeDlg.ChequeAmount
                                                        chequeDateValue = chequeDlg.ChequeDate
                                                        If finalChequeAmount < remainingBalance Then
                                                            statusValue = If(cashAmt > 0, "Mixed_Payment", "Credit_Cheque")
                                                        Else
                                                            statusValue = If(cashAmt > 0, "cash_Cheque", "Cheque")
                                                        End If
                                                    Else
                                                        Return ' User cancelled
                                                    End If
                                                End If
                                            Else
                                                ' It was purely credit before
                                                pMethod = "Credit"
                                                statusValue = If(cashAmt > 0, "cash_Credit", "Credit")
                                            End If
                                        Else
                                            Dim settlementDlg As New SettlementDialog()
                                            If settlementDlg.ShowDialog() = DialogResult.OK Then
                                                If settlementDlg.SelectedSettlement = "Cheque" Then
                                                    Dim chequeDlg As New ChequeEntryDialog()
                                                    chequeDlg.DefaultAmount = remainingBalance

                                                    ' Pass original cheque details if we are in update mode
                                                    If isUpdate AndAlso Not String.IsNullOrEmpty(originalChequeNo) Then
                                                        chequeDlg.InitialChequeNo = originalChequeNo
                                                        If Integer.TryParse(originalBankId, Nothing) Then
                                                            chequeDlg.InitialBankID = Convert.ToInt32(originalBankId)
                                                        End If
                                                        chequeDlg.InitialDate = originalChequeDate
                                                        If remainingBalance < originalChequeAmt Then
                                                            MessageBox.Show("cheque mount change")
                                                        End If
                                                        chequeDlg.InitialAmount = Math.Min(originalChequeAmt, remainingBalance)
                                                    End If

                                                    If chequeDlg.ShowDialog() = DialogResult.OK Then
                                                        pMethod = "Cheque"
                                                        chequeNo = chequeDlg.ChequeNo
                                                        bankIdValue = chequeDlg.BankID
                                                        finalChequeAmount = chequeDlg.ChequeAmount
                                                        chequeDateValue = chequeDlg.ChequeDate
                                                        If finalChequeAmount < remainingBalance Then
                                                            ' Mixed or Credit_Cheque depending on cash amount
                                                            statusValue = If(cashAmt > 0, "Mixed_Payment", "Credit_Cheque")
                                                        Else
                                                            ' Full settlement (Cash + Cheque OR Cheque only)
                                                            statusValue = If(cashAmt > 0, "cash_Cheque", "Cheque")
                                                        End If
                                                    Else
                                                        Return
                                                    End If
                                                Else
                                                    pMethod = "Credit"
                                                    statusValue = If(cashAmt > 0, "cash_Credit", "Credit")
                                                End If
                                            Else
                                                Return
                                            End If
                                        End If
                                    Else
                                        statusValue = "Paid"
                                    End If
                                ElseIf String.Equals(bType, "Cheque", StringComparison.OrdinalIgnoreCase) Then
                                    If remainingBalance <= 0 Then
                                        MessageBox.Show("Full amount is covered by Cash. Cannot proceed with Cheque billing.", "Invalid Option", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                        Return
                                    End If
                                    Dim isClearedCheque As Boolean = (isUpdate AndAlso oldChequeBalance = 0 AndAlso Not String.IsNullOrEmpty(originalChequeNo))

                                    If isClearedCheque Then
                                        pMethod = "Cheque"
                                        chequeNo = originalChequeNo
                                        If Integer.TryParse(originalBankId, Nothing) Then
                                            bankIdValue = Convert.ToInt32(originalBankId)
                                        End If
                                        finalChequeAmount = 0
                                        statusValue = "Cleared_Cheque"
                                    Else
                                        Dim chequeDlg As New ChequeEntryDialog()
                                        chequeDlg.DefaultAmount = remainingBalance
                                        chequeDlg.LockAmount = True

                                        ' Pass original cheque details if we are in update mode
                                        If isUpdate AndAlso Not String.IsNullOrEmpty(originalChequeNo) Then
                                            chequeDlg.InitialChequeNo = originalChequeNo
                                            If Integer.TryParse(originalBankId, Nothing) Then
                                                chequeDlg.InitialBankID = Convert.ToInt32(originalBankId)
                                            End If
                                            chequeDlg.InitialDate = originalChequeDate
                                            If remainingBalance < originalChequeAmt Then
                                                MessageBox.Show("cheque Amount change")
                                            End If
                                            chequeDlg.InitialAmount = Math.Min(originalChequeAmt, remainingBalance)
                                        End If

                                        If chequeDlg.ShowDialog() = DialogResult.OK Then
                                            pMethod = "Cheque"
                                            chequeNo = chequeDlg.ChequeNo
                                            bankIdValue = chequeDlg.BankID
                                            finalChequeAmount = chequeDlg.ChequeAmount
                                            chequeDateValue = chequeDlg.ChequeDate
                                            statusValue = If(cashAmt > 0, "cash_Cheque", "Cheque")
                                        Else
                                            Return
                                        End If
                                    End If
                                End If

                                ' FINAL CALCULATION FOR TRIPLE SPLIT COLUMNS (Unified & Robust)
                                If statusValue = "Paid" Then
                                    partialCash = gTotal
                                    chequeBalanceDue = 0
                                    creditBalanceDue = 0
                                ElseIf statusValue = "Cleared_Cheque" Then
                                    partialCash = cashAmt
                                    chequeBalanceDue = 0
                                    creditBalanceDue = Math.Max(0, gTotal - cashAmt - advPay)
                                    statusValue = If(creditBalanceDue > 0, "Cash_Credit", "success") ' Rewrite status based on remaining credit
                                ElseIf statusValue = "Cheque" Then
                                    partialCash = 0
                                    chequeBalanceDue = gTotal
                                    creditBalanceDue = 0
                                ElseIf statusValue = "Credit" Then
                                    partialCash = 0
                                    chequeBalanceDue = 0
                                    creditBalanceDue = Math.Max(0, gTotal - advPay)
                                Else
                                    ' Standard Mixed/Partial Settlement (Handles cash_Credit, cash_Cheque, Mixed_Payment, Credit_Cheque, etc.)
                                    partialCash = cashAmt
                                    chequeBalanceDue = finalChequeAmount
                                    creditBalanceDue = Math.Max(0, gTotal - (cashAmt + advPay) - finalChequeAmount)
                                End If

                                ' Note: We NO LONGER explicitly deduct applyToCredit here because mathematically
                                ' the new gTotal already excludes returned items, and (gTotal - cashAmt) correctly 
                                ' equates to the exact new credit balance due.

                                ' RE-ASSIGN BILLING TYPE BASED ON STATUS (USER REQUESTED RULES)
                                If statusValue = "Paid" Then
                                    bType = "Cash"
                                ElseIf statusValue = "Credit" OrElse statusValue = "cash_Credit" OrElse statusValue = "Mixed_Payment" OrElse statusValue = "Credit_Cheque" Then
                                    bType = "Credit"
                                ElseIf statusValue = "Cheque" OrElse statusValue = "cash_Cheque" Then
                                    bType = "Cheque"
                                End If

                                ' Force OR to Cash
                                If specificInvType = "OR" Then bType = "Cash"

                                ' FINAL VALIDATION: Enforce PO and Cheque numbers for Mixed/Credit/Cheque settlements
                                ' SKIP PO Validation for Pending status or Order Takers
                                Dim isPendingType As Boolean = String.Equals(bType, "PENDING", StringComparison.OrdinalIgnoreCase)
                                Dim isCreditBillType As Boolean = String.Equals(bType, "Credit", StringComparison.OrdinalIgnoreCase)
                                ' P/O Number validation disabled as per user request
                                ' If Module1.FinancialRole.ToLower() <> "order taker" AndAlso Not isPendingType AndAlso Not isQuote AndAlso isCreditBillType AndAlso creditBalanceDue > 0 Then
                                '     If String.IsNullOrWhiteSpace(TextBoxPO.Text) OrElse TextBoxPO.Text.Trim().ToLower() = "not" Then
                                '         MessageBox.Show("P/O Number is mandatory for Credit bills. Please enter a valid P/O number before saving.", "PO Number Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                '         TextBoxPO.Focus()
                                '         Return
                                '     End If
                                ' End If

                                If Not isQuote AndAlso chequeBalanceDue > 0 AndAlso String.IsNullOrWhiteSpace(chequeNo) Then
                                    MessageBox.Show("Cheque details are mandatory for settlements involving a cheque.", "Cheque Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                    Return
                                End If

                                ' CREDIT LIMIT VALIDATION
                                If Not isQuote AndAlso Not String.IsNullOrEmpty(selectedCustomerId) Then
                                    Dim lim As Decimal = 0
                                    Dim accBal As Decimal = 0
                                    Decimal.TryParse(txtCreditLimit.Text.Replace(",", ""), lim)
                                    Decimal.TryParse(lblAccountOutstanding.Text.Replace(",", ""), accBal)

                                    ' When updating an existing invoice, the account outstanding figure ALREADY includes
                                    ' the current invoice's old credit balance. Subtract it to avoid double-counting.
                                    Dim effectiveAccBal As Decimal = accBal
                                    If isUpdate Then
                                        effectiveAccBal = Math.Max(0, accBal - oldCreditBalance)
                                    End If

                                    Dim currentBillBalance As Decimal = gTotal - cashAmt
                                    Dim projectTotalDue As Decimal = effectiveAccBal + currentBillBalance

                                    If lim > 0 AndAlso projectTotalDue > lim Then
                                        MessageBox.Show("Credit Limit Exceeded! " & vbCrLf &
                                                      "Current Account Balance: " & effectiveAccBal.ToString("N2") & vbCrLf &
                                                      "This Bill Balance: " & currentBillBalance.ToString("N2") & vbCrLf &
                                                      "Total: " & projectTotalDue.ToString("N2") & vbCrLf &
                                                      "Credit Limit: " & lim.ToString("N2"), "Limit Exceeded", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                        Return
                                    End If
                                End If

                                ' Derive figures for DB
                                Dim totalPaid As Decimal = cashAmt + advPay
                                Dim advancePayment As Decimal = totalPaid
                                Dim balanceDue As Decimal = gTotal - totalPaid
                                ' Calculate change: Normal change (totalPaid > gTotal) + any Refund given back (applyToCash)
                                Dim changeAmt As Decimal = If(totalPaid > gTotal, totalPaid - gTotal, 0)
                                If isUpdate AndAlso applyToCash > 0 Then
                                    changeAmt = applyToCash
                                End If

                                ' --- CHANGE ACTION DIALOG LOGIC ---
                                Dim selectedChangeAction As ChangeActionDialog.ChangeAction = ChangeActionDialog.ChangeAction.CashReturn
                                Dim isGenericCash As Boolean = String.Equals(txtSalesRep.Text.Trim(), "CASH", StringComparison.OrdinalIgnoreCase) AndAlso
                                                               String.Equals(txtCustomerPhone.Text.Trim(), "CASH", StringComparison.OrdinalIgnoreCase) AndAlso
                                                               String.Equals(txtCustomerAddress.Text.Trim(), "CASH", StringComparison.OrdinalIgnoreCase)

                                If changeAmt > 0 AndAlso String.Equals(pMethod, "Cash", StringComparison.OrdinalIgnoreCase) AndAlso Not isGenericCash Then
                                    Dim hasCredit As Boolean = False
                                    If Not String.IsNullOrEmpty(selectedCustomerId) Then
                                        Try
                                            Using connCheck As New MySqlConnection(Module1.ConnStr)
                                                connCheck.Open()
                                                Using cmdCheck1 As New MySqlCommand("SELECT SUM(credit_balance_due) FROM billing WHERE customer_id=@cid AND status IN ('Credit','Cash_Credit','Mixed_Payment','Credit_Cheque') AND credit_balance_due > 0 AND inv_no != @inv", connCheck)
                                                    cmdCheck1.Parameters.AddWithValue("@cid", selectedCustomerId)
                                                    cmdCheck1.Parameters.AddWithValue("@inv", If(String.IsNullOrEmpty(existingInvNo), "", existingInvNo))
                                                    Dim credRes1 = cmdCheck1.ExecuteScalar()
                                                    If credRes1 IsNot DBNull.Value AndAlso Convert.ToDecimal(credRes1) > 0 Then
                                                        hasCredit = True
                                                    End If
                                                End Using
                                                Using cmdCheck2 As New MySqlCommand("SELECT SUM(amount) FROM customer_credit WHERE customer_id=@cid AND amount > 0 AND is_active=1", conn, dbTrans)
                                                    cmdCheck2.Parameters.AddWithValue("@cid", selectedCustomerId)
                                                    Dim credRes2 = cmdCheck2.ExecuteScalar()
                                                    If credRes2 IsNot DBNull.Value AndAlso Convert.ToDecimal(credRes2) > 0 Then
                                                        hasCredit = True
                                                    End If
                                                End Using
                                            End Using
                                        Catch
                                        End Try
                                    End If

                                    Dim dlg As New ChangeActionDialog(changeAmt, hasCredit)
                                    If Not String.IsNullOrEmpty(selectedCustomerId) Then
                                        If dlg.ShowDialog() = DialogResult.OK Then
                                            selectedChangeAction = dlg.SelectedAction
                                        Else
                                            Return ' User cancelled
                                        End If
                                    End If
                                End If
                                ' ----------------------------------

                                ' If customer paid more than bill, change is always given back. Balance is settled to 0.
                                If totalPaid > gTotal Then
                                    balanceDue = 0
                                    advancePayment = gTotal
                                End If


                                Dim tableNameForInv As String = ""
                                Dim prefix As String = ""
                                Dim printedInvNo As String = ""
                                If ComboBox1.Text = "Quote" Then
                                    tableNameForInv = "inv_no_qt1"
                                    prefix = "QT"
                                ElseIf Module1.IsRgrModeActive Then
                                    tableNameForInv = "inv_no_RGR1"
                                    prefix = "RGR"
                                ElseIf CheckBoxIsVat.Checked Then
                                    tableNameForInv = "inv_no_VT1"
                                    prefix = "VT"
                                Else
                                    tableNameForInv = If(isElBill, "inv_no_el1", "inv_no_gr1")
                                    prefix = If(isElBill, "EL", "GR")
                                End If

                                ' Ensure RGR table exists (one-time check)
                                If Module1.IsRgrModeActive Then
                                    Try
                                        Using cmdInit As New MySqlCommand("CREATE TABLE IF NOT EXISTS inv_no_RGR1 (id INT PRIMARY KEY)", conn, dbTrans)
                                            cmdInit.ExecuteNonQuery()
                                        End Using
                                    Catch : End Try
                                End If

                                Dim saveRetrySuccess As Boolean = False
                                Dim saveAttempts As Integer = 0
                                
                                While Not saveRetrySuccess AndAlso saveAttempts < 10
                                    saveAttempts += 1
                                    Try
                                        If Not isUpdate Then
                                            ' 1. Fetch Internal ID
                                            Dim fetchSql = "SELECT MAX(id) FROM " & tableNameForInv
                                            Using cmdFetch As New MySqlCommand(fetchSql, conn, dbTrans)
                                                Dim lastIdVal = cmdFetch.ExecuteScalar()
                                                Dim nextIdFinal As Integer = If(lastIdVal Is DBNull.Value OrElse lastIdVal Is Nothing, 0, Convert.ToInt32(lastIdVal)) + 1
                                                ' Use UI text if VT (allows manual edit), otherwise use projected sequence
                                                existingInvNo = If(CheckBoxIsVat.Checked, lblInvoiceNumber.Text.Trim(), prefix & nextIdFinal.ToString("D5"))
                                                
                                                ' PRE-RESERVE ID TO PREVENT DUPLICATES
                                                If Not CheckBoxIsVat.Checked Then
                                                    Using cmdReserve = New MySqlCommand("INSERT INTO " & tableNameForInv & " (id) VALUES (@id)", conn, dbTrans)
                                                        cmdReserve.Parameters.AddWithValue("@id", nextIdFinal)
                                                        cmdReserve.ExecuteNonQuery()
                                                    End Using
                                                End If
                                            End Using

                                            ' 2. Handle Masked Numbering for RGR
                                            If Module1.IsRgrModeActive AndAlso Not isQuote Then
                                                ' Fetch the next standard 'gr' number (but don't consume yet, will consume at the end)
                                                Dim maskedTable As String = "inv_no_gr1"
                                                Dim fetchMaskSql = "SELECT MAX(id) FROM " & maskedTable
                                                Using cmdMask As New MySqlCommand(fetchMaskSql, conn, dbTrans)
                                                    Dim lastMaskVal = cmdMask.ExecuteScalar()
                                                    Dim nextMaskId As Integer = If(lastMaskVal Is DBNull.Value OrElse lastMaskVal Is Nothing, 0, Convert.ToInt32(lastMaskVal)) + 1
                                                    printedInvNo = "gr" & nextMaskId.ToString("D5") ' Lowercase 'gr' as requested
                                                    
                                                    Using cmdReserveGR = New MySqlCommand("INSERT INTO " & maskedTable & " (id) VALUES (@id)", conn, dbTrans)
                                                        cmdReserveGR.Parameters.AddWithValue("@id", nextMaskId)
                                                        cmdReserveGR.ExecuteNonQuery()
                                                    End Using
                                                End Using
                                            Else
                                                ' For standard bills (GR, EL, VT, QT), printed_inv_no is the same as inv_no
                                                printedInvNo = existingInvNo
                                            End If
                                            ' If it's an update, use the existing invoice number already set at the start of the method
                                            ' EXCEPT if it's a VT bill, allow the UI text to override (in case they edited it)
                                            If CheckBoxIsVat.Checked Then
                                                existingInvNo = lblInvoiceNumber.Text.Trim()
                                            End If

                                            ' Also ensure printedInvNo is set (usually same as existingInvNo for standard bills)
                                            If String.IsNullOrEmpty(printedInvNo) Then
                                                printedInvNo = existingInvNo
                                            End If
                                        End If

                                ' --- BACKUP BILLING HEADER TO HISTORY TABLE ---
                                If isUpdate Then
                                    Dim backupMainSql As String = "INSERT INTO billing_history (billing_id, inv_no, customer_id, user_id, payment_type, inv_type, billing_type, status, subtotal, grand_total, vat_id, our_discount, inv_discount, advance_payment, balance_due, paid_amount, cheque_no, bank_id, po_number, cheque_balance_due, credit_balance_due, partial_cash, revision_date, revised_by, timestamps, cus_vat_id) " &
                                                                  "SELECT id, inv_no, customer_id, user_id, payment_type, inv_type, billing_type, status, subtotal, grand_total, vat_id, our_discount, inv_discount, advance_payment, balance_due, paid_amount, cheque_no, bank_id, po_number, cheque_balance_due, credit_balance_due, partial_cash, @now_local, @revised_by, timestamps, cus_vat_id " &
                                                                  "FROM " & mainTable & " WHERE id = @bid"
                                    Try
                                        Using cmdBackup As New MySqlCommand(backupMainSql, conn, dbTrans)
                                            cmdBackup.Parameters.AddWithValue("@bid", billingId)
                                            cmdBackup.Parameters.AddWithValue("@revised_by", If(cmbCashier.SelectedValue Is Nothing, DBNull.Value, cmbCashier.SelectedValue))
                                            cmdBackup.Parameters.AddWithValue("@now_local", DateTime.Now)
                                            cmdBackup.ExecuteNonQuery()
                                        End Using
                                    Catch ex As Exception
                                        ' Ignore error if table hasn't been created yet by user
                                    End Try
                                End If
                                ' ----------------------------------------------

                                ' 2. MAIN BILLING UPDATE/INSERT
                                Dim mainSql As String
                                If isUpdate Then
                                    If isQuote Then
                                        mainSql = "UPDATE " & mainTable & " SET payment_type=@p_type, inv_type=@i_type, billing_type=@b_type, status=@status, subtotal=@subtotal, grand_total=@grand_total, vat_id=@v_id, our_discount=@our_discount, inv_discount=@inv_discount, advance_payment=@advance, balance_due=@balance, paid_amount=@paid, cash_received=@received, change_amount=@change, user_id=@u_id, customer_id=@c_id, updated_at=@now_local, cus_vat_id=@cus_vat_id, print_as_retail=@print_retail WHERE id=@id"
                                    Else
                                        mainSql = "UPDATE " & mainTable & " SET payment_type=@p_type, inv_type=@i_type, billing_type=@b_type, status=@status, subtotal=@subtotal, grand_total=@grand_total, vat_id=@v_id, our_discount=@our_discount, inv_discount=@inv_discount, advance_payment=@advance, balance_due=@balance, paid_amount=@paid, cash_received=@received, change_amount=@change, user_id=@u_id, customer_id=@c_id, cheque_no=@cheque_no, bank_id=@bank_id, po_number=@po, cheque_balance_due=@chq_bal, credit_balance_due=@crd_bal, partial_cash=@p_cash, updated_at=@now_local, cash_status=@c_status, order_user_id=@o_uid, collector_user_id=@coll_uid, change_action=@bal_action, adv_pay_amount=@adv_pay_amount, cus_vat_id=@cus_vat_id, print_as_retail=@print_retail WHERE id=@id"
                                    End If
                                Else
                                    If isQuote Then
                                        mainSql = "INSERT INTO " & mainTable & " (inv_no, payment_type, inv_type, billing_type, status, subtotal, grand_total, customer_id, timestamps, vat_id, our_discount, inv_discount, advance_payment, balance_due, paid_amount, cash_received, change_amount, user_id, is_rgr, printed_inv_no, cus_vat_id, print_as_retail) VALUES (@inv_no, @p_type, @i_type, @b_type, @status, @subtotal, @grand_total, @c_id, @now_local, @v_id, @our_discount, @inv_discount, @advance, @balance, @paid, @received, @change, @u_id, @is_rgr, @printed, @cus_vat_id, @print_retail)"
                                    Else
                                        mainSql = "INSERT INTO " & mainTable & " (inv_no, payment_type, inv_type, billing_type, status, subtotal, grand_total, customer_id, timestamps, vat_id, our_discount, inv_discount, advance_payment, balance_due, paid_amount, cash_received, change_amount, user_id, cheque_no, bank_id, po_number, cheque_balance_due, credit_balance_due, partial_cash, cash_status, order_user_id, collector_user_id, balance_action, balance_amount, wallet_used, wallet_balance_after, is_rgr, printed_inv_no, adv_pay_amount, cus_vat_id, print_as_retail) VALUES (@inv_no, @p_type, @i_type, @b_type, @status, @subtotal, @grand_total, @c_id, @now_local, @v_id, @our_discount, @inv_discount, @advance, @balance, @paid, @received, @change, @u_id, @cheque_no, @bank_id, @po, @chq_bal, @crd_bal, @p_cash, @c_status, @o_uid, @coll_uid, @bal_action, @bal_amt, @w_used, @w_bal_after, @is_rgr, @printed, @adv_pay_amount, @cus_vat_id, @print_retail)"
                                    End If
                                End If

                                Using cmdMain As New MySqlCommand(mainSql, conn, dbTrans)
                                    cmdMain.Parameters.AddWithValue("@now_local", DateTime.Now)
                                    If isUpdate Then
                                        cmdMain.Parameters.AddWithValue("@id", billingId)
                                        cmdMain.Parameters.AddWithValue("@c_id", If(String.IsNullOrEmpty(selectedCustomerId), DBNull.Value, selectedCustomerId))
                                    Else
                                        cmdMain.Parameters.AddWithValue("@inv_no", existingInvNo)
                                        cmdMain.Parameters.AddWithValue("@c_id", If(String.IsNullOrEmpty(selectedCustomerId), DBNull.Value, selectedCustomerId))
                                        cmdMain.Parameters.AddWithValue("@printed", If(String.IsNullOrEmpty(printedInvNo), DBNull.Value, printedInvNo))
                                    End If

                                    cmdMain.Parameters.AddWithValue("@p_type", pMethod)
                                    cmdMain.Parameters.AddWithValue("@i_type", specificInvType)
                                    cmdMain.Parameters.AddWithValue("@b_type", bType)
                                    cmdMain.Parameters.AddWithValue("@cus_vat_id", If(String.IsNullOrWhiteSpace(txtCusVatId.Text), DBNull.Value, txtCusVatId.Text.Trim()))
                                    cmdMain.Parameters.AddWithValue("@print_retail", If(CheckBoxPrintAsRetail.Checked, 1, 0))

                                    ' UPDATE CUSTOMER CREDIT INFO IF CHANGED
                                    If Not String.IsNullOrEmpty(selectedCustomerId) Then
                                        Dim safeClim As Decimal = 0
                                        Decimal.TryParse(txtCreditLimit.Text.Replace(",", ""), safeClim)

                                        Dim updateCusSql = "UPDATE customer SET credit_limit = @clim, credit_period = @cper WHERE id = @cid"
                                        Using cmdCus As New MySqlCommand(updateCusSql, conn, dbTrans)
                                            cmdCus.Parameters.AddWithValue("@clim", safeClim)
                                            cmdCus.Parameters.AddWithValue("@cper", dtpCreditPeriod.Value.ToString("yyyy-MM-dd"))
                                            cmdCus.Parameters.AddWithValue("@cid", selectedCustomerId)
                                            cmdCus.ExecuteNonQuery()
                                        End Using
                                    End If
                                    cmdMain.Parameters.AddWithValue("@status", statusValue)
                                    Dim safeSubtotal As Decimal = 0
                                    If lblTotalAmount.Tag IsNot Nothing Then
                                        Decimal.TryParse(lblTotalAmount.Tag.ToString().Replace(",", ""), safeSubtotal)
                                    End If
                                    cmdMain.Parameters.AddWithValue("@subtotal", safeSubtotal)
                                    cmdMain.Parameters.AddWithValue("@grand_total", gTotal)
                                    cmdMain.Parameters.AddWithValue("@v_id", If(CheckBoxIsVat.Checked, ComboBoxTotalVat.SelectedValue, 1))

                                    Dim oDisc As Decimal = 0
                                    Dim iDisc As Decimal = 0
                                    Decimal.TryParse(txtOurDiscount.Text.Replace(",", ""), oDisc)
                                    Decimal.TryParse(txtInvDiscount.Text.Replace(",", ""), iDisc)
                                    cmdMain.Parameters.AddWithValue("@our_discount", oDisc)
                                    cmdMain.Parameters.AddWithValue("@inv_discount", iDisc)
                                    cmdMain.Parameters.AddWithValue("@advance", advancePayment)
                                    cmdMain.Parameters.AddWithValue("@balance", balanceDue)
                                    cmdMain.Parameters.AddWithValue("@paid", advancePayment)
                                    cmdMain.Parameters.AddWithValue("@received", originalReceived)
                                    Dim dbChangeAmt As Decimal = changeAmt
                                    If Not isQuote Then
                                        ' 1. Logic for +/- signs based on action
                                        If selectedChangeAction = ChangeActionDialog.ChangeAction.AddToAdvance OrElse selectedChangeAction = ChangeActionDialog.ChangeAction.SettlePreviousCredit Then
                                            dbChangeAmt = -Math.Abs(changeAmt)
                                        Else
                                            dbChangeAmt = Math.Abs(changeAmt)
                                        End If

                                        ' 2. If Wallet was applied, store remaining balance as negative
                                        Dim wUsed As Decimal = 0
                                        If isWalletApplied Then
                                            If txtAdvPay.Text <> "" Then
                                                Decimal.TryParse(txtAdvPay.Text, wUsed)
                                            ElseIf txtCashAmount.Text <> "" Then
                                                Decimal.TryParse(txtCashAmount.Text, wUsed)
                                            End If

                                            If wUsed > 0 AndAlso changeAmt = 0 Then
                                                ' Fetch current total wallet balance for this customer
                                                Dim totalWallet As Decimal = 0
                                                Using cmdWallet As New MySqlCommand("SELECT SUM(credit_amount) FROM customer_credit_notes WHERE customer_id = @cid AND status = 'active'", conn, dbTrans)
                                                    cmdWallet.Parameters.AddWithValue("@cid", selectedCustomerId)
                                                    Dim val = cmdWallet.ExecuteScalar()
                                                    If val IsNot DBNull.Value AndAlso val IsNot Nothing Then totalWallet = Convert.ToDecimal(val)
                                                End Using
                                                ' The totalWallet here is BEFORE this bill's usage because we haven't updated notes yet
                                                ' So remaining = totalWallet - wUsed
                                                dbChangeAmt = -(totalWallet - wUsed)
                                            End If
                                        End If
                                    End If

                                    cmdMain.Parameters.AddWithValue("@change", dbChangeAmt)
                                    cmdMain.Parameters.AddWithValue("@u_id", If(cmbCashier.SelectedValue Is Nothing, DBNull.Value, cmbCashier.SelectedValue))

                                    If Not isQuote Then
                                        cmdMain.Parameters.AddWithValue("@bal_action", selectedChangeAction.ToString())
                                        cmdMain.Parameters.AddWithValue("@bal_amt", Math.Abs(changeAmt))

                                        Dim wUsedParam As Decimal = 0
                                        If isWalletApplied Then
                                            If txtAdvPay.Text <> "" Then
                                                Decimal.TryParse(txtAdvPay.Text, wUsedParam)
                                            ElseIf txtCashAmount.Text <> "" Then
                                                Decimal.TryParse(txtCashAmount.Text, wUsedParam)
                                            End If
                                        End If
                                        cmdMain.Parameters.AddWithValue("@w_used", wUsedParam)

                                        ' Final wallet balance after this transaction
                                        cmdMain.Parameters.AddWithValue("@w_bal_after", Math.Abs(dbChangeAmt))
                                    End If
                                    cmdMain.Parameters.AddWithValue("@is_rgr", If(Module1.IsRgrModeActive, 1, 0))

                                    ' NEW: Role-based Cash Tracking and Auditing
                                    Dim currentCashStatus As String = "COLLECTED"
                                    Dim currentCashierID As Object = If(cmbCashier.SelectedValue Is Nothing, Module1.CurrentUserID, cmbCashier.SelectedValue)
                                    Dim orderUID As Object = currentCashierID
                                    Dim collectorUID As Object = currentCashierID

                                    ' PENDING only if billing_type is PENDING or status resolved to Pending
                                    If String.Equals(bType, "PENDING", StringComparison.OrdinalIgnoreCase) OrElse
                                       String.Equals(statusValue, "Pending", StringComparison.OrdinalIgnoreCase) Then
                                        currentCashStatus = "PENDING"
                                        collectorUID = DBNull.Value
                                    End If


                                    If currentCashStatus = "PENDING" AndAlso isUpdate AndAlso Not isQuote Then
                                        ' Log original states to the dedicated table if it's an update.
                                        ' Using INSERT IGNORE ensures that if a seller updates it multiple times, 
                                        ' we preserve the VERY FIRST original state, not a 'PENDING' state from the 2nd edit.
                                        ' NOTE: Wrapped in its own Try-Catch so a table schema issue never blocks the main save.
                                        Try
                                            Dim archiveSql = "INSERT IGNORE INTO pending_returns_data (billing_id, original_billing_type, original_payment_type, original_paid_amount) " &
                                                             "VALUES (@bid, @obt, @opt, @opa)"
                                            Using cmdArch As New MySqlCommand(archiveSql, conn, dbTrans)
                                                cmdArch.Parameters.AddWithValue("@bid", billingId)
                                                cmdArch.Parameters.AddWithValue("@obt", If(String.IsNullOrWhiteSpace(originalBillingType) OrElse originalBillingType.ToUpper() = "PENDING", "Cash", originalBillingType))
                                                cmdArch.Parameters.AddWithValue("@opt", If(String.IsNullOrWhiteSpace(originalPaymentMethod) OrElse originalPaymentMethod.ToUpper() = "PENDING", "Cash", originalPaymentMethod))
                                                ' Use the DB paid amount (before return), not the post-return textbox value
                                                cmdArch.Parameters.AddWithValue("@opa", oldCashAmount)
                                                cmdArch.ExecuteNonQuery()
                                            End Using
                                        Catch exArch As Exception
                                            ' Archive failed (e.g. table schema issue) — continue with main save anyway
                                        End Try
                                    End If

                                    ' Payment tracking columns (Not present in quotation_billing)
                                    If Not isQuote Then
                                        cmdMain.Parameters.AddWithValue("@c_status", currentCashStatus)
                                        cmdMain.Parameters.AddWithValue("@o_uid", orderUID)
                                        cmdMain.Parameters.AddWithValue("@coll_uid", collectorUID)
                                        cmdMain.Parameters.AddWithValue("@cheque_no", If(Not String.IsNullOrWhiteSpace(chequeNo), chequeNo, DBNull.Value))
                                        cmdMain.Parameters.AddWithValue("@bank_id", bankIdValue)
                                        Dim poVal As String = If(String.IsNullOrWhiteSpace(TextBoxPO.Text), "not", TextBoxPO.Text.Trim())
                                        cmdMain.Parameters.AddWithValue("@po", poVal)

                                        cmdMain.Parameters.AddWithValue("@chq_bal", chequeBalanceDue)
                                        cmdMain.Parameters.AddWithValue("@crd_bal", creditBalanceDue)
                                        cmdMain.Parameters.AddWithValue("@p_cash", partialCash)
                                        cmdMain.Parameters.AddWithValue("@adv_pay_amount", advPay)
                                    End If

                                    cmdMain.ExecuteNonQuery()
                                    If Not isUpdate Then billingId = cmdMain.LastInsertedId
                                End Using

                                saveRetrySuccess = True
                            Catch ex As MySqlException
                                If ex.Number = 1062 AndAlso Not isUpdate AndAlso Not CheckBoxIsVat.Checked Then
                                    If saveAttempts >= 10 Then Throw ex
                                Else
                                    Throw ex
                                End If
                            End Try
                        End While

                        ' --- AUTO SYNC TO CUSTOMER CREDIT TABLE ---
                                If Not isQuote AndAlso Not String.IsNullOrEmpty(selectedCustomerId) AndAlso selectedCustomerId <> "1" Then
                                    Try
                                        Dim syncSql As String = ""
                                        Dim existingCreditId As Integer = 0

                                        ' Use printedInvNo for RGR mode if available, otherwise use existingInvNo
                                        Dim creditInvNo As String = existingInvNo
                                        If Module1.IsRgrModeActive AndAlso Not String.IsNullOrEmpty(printedInvNo) Then
                                            creditInvNo = printedInvNo
                                        End If

                                        ' Check if a credit record already exists for this invoice
                                        Dim checkCreditSql As String = "SELECT id FROM customer_credit WHERE inv_no = @inv AND customer_id = @cid LIMIT 1"
                                        Using cmdCheck As New MySqlCommand(checkCreditSql, conn, dbTrans)
                                            cmdCheck.Parameters.AddWithValue("@inv", creditInvNo)
                                            cmdCheck.Parameters.AddWithValue("@cid", selectedCustomerId)
                                            Dim res = cmdCheck.ExecuteScalar()
                                            If res IsNot Nothing AndAlso res IsNot DBNull.Value Then
                                                existingCreditId = Convert.ToInt32(res)
                                            End If
                                        End Using

                                        If creditBalanceDue > 0 Then
                                            If existingCreditId > 0 Then
                                                ' Update existing record — preserve original timestamps to prevent re-dating on duplicate print
                                                If isUpdate Then
                                                    syncSql = "UPDATE customer_credit SET amount = @amt, is_active = 1 WHERE id = @id"
                                                Else
                                                    syncSql = "UPDATE customer_credit SET amount = @amt, timestamps = @now, is_active = 1 WHERE id = @id"
                                                End If
                                            Else
                                                ' Insert new record — only for truly new invoices
                                                If Not isUpdate Then
                                                    syncSql = "INSERT INTO customer_credit (amount, customer_id, inv_no, timestamps, is_active, is_rgr) VALUES (@amt, @cid, @inv, @now, 1, @is_rgr)"
                                                End If
                                            End If
                                        Else
                                            ' If balance is now 0 but a record exists, mark it as inactive/settled
                                            If existingCreditId > 0 Then
                                                syncSql = "UPDATE customer_credit SET amount = 0, is_active = 0 WHERE id = @id"
                                            End If
                                        End If

                                        If Not String.IsNullOrEmpty(syncSql) Then
                                            Using cmdSync As New MySqlCommand(syncSql, conn, dbTrans)
                                                cmdSync.Parameters.AddWithValue("@amt", creditBalanceDue)
                                                cmdSync.Parameters.AddWithValue("@cid", selectedCustomerId)
                                                cmdSync.Parameters.AddWithValue("@inv", creditInvNo)
                                                cmdSync.Parameters.AddWithValue("@now", DateTime.Now)
                                                cmdSync.Parameters.AddWithValue("@id", existingCreditId)
                                                cmdSync.Parameters.AddWithValue("@is_rgr", If(Module1.IsRgrModeActive, 1, 0))
                                                cmdSync.ExecuteNonQuery()
                                            End Using
                                        End If
                                    Catch ex As Exception
                                        ' Silent fail to ensure main transaction completion isn't blocked
                                    End Try
                                End If

                                ' --- APPLY CHANGE ACTION (Wallet or Settle Credit) ---
                                If Not isQuote AndAlso changeAmt > 0 AndAlso String.Equals(pMethod, "Cash", StringComparison.OrdinalIgnoreCase) Then
                                    If selectedChangeAction = ChangeActionDialog.ChangeAction.AddToAdvance Then
                                        Try
                                            Using connAction As New MySqlConnection(Module1.ConnStr)
                                                connAction.Open()
                                                Dim sqlNote As String = "INSERT INTO customer_credit_notes (customer_id, inv_no, credit_amount, issue_date, status, is_rgr) VALUES (@cid, @inv, @amt, @now, 'active', @is_rgr)"
                                                Using cmdNote As New MySqlCommand(sqlNote, conn, dbTrans)
                                                    cmdNote.Parameters.AddWithValue("@cid", selectedCustomerId)
                                                    cmdNote.Parameters.AddWithValue("@inv", existingInvNo)
                                                    cmdNote.Parameters.AddWithValue("@amt", changeAmt)
                                                    cmdNote.Parameters.AddWithValue("@now", DateTime.Now)
                                                    cmdNote.Parameters.AddWithValue("@is_rgr", If(Module1.IsRgrModeActive, 1, 0))
                                                    cmdNote.ExecuteNonQuery()
                                                End Using
                                            End Using
                                        Catch ex As Exception
                                            MessageBox.Show("Error adding to wallet: " & ex.Message)
                                        End Try
                                    ElseIf selectedChangeAction = ChangeActionDialog.ChangeAction.SettlePreviousCredit Then
                                        Try
                                            Using connAction As New MySqlConnection(Module1.ConnStr)
                                                connAction.Open()
                                                Dim sqlPay As String = "INSERT INTO customer_payments (CusID, Customer, PaymentType, Amount, Date, inv_no, is_rgr) VALUES (@cid, @cname, 'Cash', @amt, @now, @inv, @is_rgr)"
                                                Using cmdPay As New MySqlCommand(sqlPay, conn, dbTrans)
                                                    cmdPay.Parameters.AddWithValue("@cid", selectedCustomerId)
                                                    cmdPay.Parameters.AddWithValue("@cname", txtSalesRep.Text)
                                                    cmdPay.Parameters.AddWithValue("@amt", changeAmt)
                                                    cmdPay.Parameters.AddWithValue("@now", DateTime.Now)
                                                    cmdPay.Parameters.AddWithValue("@inv", "Settled by " & existingInvNo)
                                                    cmdPay.Parameters.AddWithValue("@is_rgr", If(Module1.IsRgrModeActive, 1, 0))
                                                    cmdPay.ExecuteNonQuery()
                                                End Using

                                                Dim remainingSettle As Decimal = changeAmt

                                                Dim sqlOldCred As String = "SELECT id, amount FROM customer_credit WHERE customer_id=@cid AND is_active=1 AND amount > 0 ORDER BY timestamps ASC"
                                                Dim dtOldCred As New DataTable()
                                                Using cmdOldCred As New MySqlCommand(sqlOldCred, conn, dbTrans)
                                                    cmdOldCred.Parameters.AddWithValue("@cid", selectedCustomerId)
                                                    Using da As New MySqlDataAdapter(cmdOldCred)
                                                        da.Fill(dtOldCred)
                                                    End Using
                                                End Using
                                                For Each row As DataRow In dtOldCred.Rows
                                                    If remainingSettle <= 0 Then Exit For
                                                    Dim oldId As Integer = Convert.ToInt32(row("id"))
                                                    Dim oldBal As Decimal = Convert.ToDecimal(row("amount"))
                                                    Dim amountToDeduct As Decimal = Math.Min(oldBal, remainingSettle)
                                                    Dim sqlUpd As String = "UPDATE customer_credit SET amount = amount - @deduct WHERE id=@id"
                                                    Using cmdUpd As New MySqlCommand(sqlUpd, conn, dbTrans)
                                                        cmdUpd.Parameters.AddWithValue("@deduct", amountToDeduct)
                                                        cmdUpd.Parameters.AddWithValue("@id", oldId)
                                                        cmdUpd.ExecuteNonQuery()
                                                    End Using
                                                    remainingSettle -= amountToDeduct
                                                Next

                                                If remainingSettle > 0 Then
                                                    Dim sqlOldBill As String = "SELECT id, credit_balance_due FROM billing WHERE customer_id=@cid AND status IN ('Credit','Cash_Credit','Mixed_Payment','Credit_Cheque') AND credit_balance_due > 0 AND inv_no != @inv ORDER BY timestamps ASC"
                                                    Dim dtOldBill As New DataTable()
                                                    Using cmdOldBill As New MySqlCommand(sqlOldBill, conn, dbTrans)
                                                        cmdOldBill.Parameters.AddWithValue("@cid", selectedCustomerId)
                                                        cmdOldBill.Parameters.AddWithValue("@inv", existingInvNo)
                                                        Using da As New MySqlDataAdapter(cmdOldBill)
                                                            da.Fill(dtOldBill)
                                                        End Using
                                                    End Using
                                                    For Each row As DataRow In dtOldBill.Rows
                                                        If remainingSettle <= 0 Then Exit For
                                                        Dim oldId As Integer = Convert.ToInt32(row("id"))
                                                        Dim oldBal As Decimal = Convert.ToDecimal(row("credit_balance_due"))
                                                        Dim amountToDeduct As Decimal = Math.Min(oldBal, remainingSettle)
                                                        Dim sqlUpd As String = "UPDATE billing SET credit_balance_due = credit_balance_due - @deduct WHERE id=@id"
                                                        Using cmdUpd As New MySqlCommand(sqlUpd, conn, dbTrans)
                                                            cmdUpd.Parameters.AddWithValue("@deduct", amountToDeduct)
                                                            cmdUpd.Parameters.AddWithValue("@id", oldId)
                                                            cmdUpd.ExecuteNonQuery()
                                                        End Using
                                                        remainingSettle -= amountToDeduct
                                                    Next
                                                End If
                                            End Using
                                        Catch ex As Exception
                                            MessageBox.Show("Error settling previous credit: " & ex.Message)
                                        End Try
                                    End If
                                End If
                                ' -----------------------------------------------------

                                ' --- CONSUME WALLET BALANCE IF APPLIED ---
                                If isWalletApplied Then
                                    Try
                                        Dim remainingToDeduct As Decimal = 0
                                        ' In this context, parse the wallet amount from whichever textbox holds it
                                        If txtAdvPay.Text <> "" Then
                                            Decimal.TryParse(txtAdvPay.Text, remainingToDeduct)
                                        ElseIf txtCashAmount.Text <> "" Then
                                            Decimal.TryParse(txtCashAmount.Text, remainingToDeduct)
                                        End If

                                        Dim sqlFetchNotes = "SELECT id, credit_amount FROM customer_credit_notes WHERE customer_id = @cid AND status = 'active' ORDER BY issue_date ASC"
                                        Dim dtNotes As New DataTable()
                                        Using cmdFetchNotes As New MySqlCommand(sqlFetchNotes, conn, dbTrans)
                                            cmdFetchNotes.Parameters.AddWithValue("@cid", selectedCustomerId)
                                            Using daNotes As New MySqlDataAdapter(cmdFetchNotes)
                                                daNotes.Fill(dtNotes)
                                            End Using
                                        End Using

                                        For Each rowNote As DataRow In dtNotes.Rows
                                            If remainingToDeduct <= 0 Then Exit For
                                            Dim noteId As Integer = Convert.ToInt32(rowNote("id"))
                                            Dim noteAmt As Decimal = Convert.ToDecimal(rowNote("credit_amount"))

                                            If noteAmt <= remainingToDeduct Then
                                                ' Mark note as fully used
                                                Dim sqlUseNote = "UPDATE customer_credit_notes SET status = 'used' WHERE id = @id"
                                                Using cmdUseNote As New MySqlCommand(sqlUseNote, conn, dbTrans)
                                                    cmdUseNote.Parameters.AddWithValue("@id", noteId)
                                                    cmdUseNote.ExecuteNonQuery()
                                                End Using
                                                remainingToDeduct -= noteAmt
                                            Else
                                                ' Partially use note
                                                Dim sqlPartNote = "UPDATE customer_credit_notes SET credit_amount = credit_amount - @deduct WHERE id = @id"
                                                Using cmdPartNote As New MySqlCommand(sqlPartNote, conn, dbTrans)
                                                    cmdPartNote.Parameters.AddWithValue("@deduct", remainingToDeduct)
                                                    cmdPartNote.Parameters.AddWithValue("@id", noteId)
                                                    cmdPartNote.ExecuteNonQuery()
                                                End Using
                                                remainingToDeduct = 0
                                            End If
                                        Next
                                        isWalletApplied = False
                                    Catch exWallet As Exception
                                        MessageBox.Show("Error processing wallet usage: " & exWallet.Message)
                                    End Try
                                End If

                                ' 2.5 INSERT DAILY ADJUSTMENT IF CHANGED
                                If isUpdate AndAlso Not isQuote Then
                                    Dim diffAmount As Decimal = gTotal - oldGrandTotal
                                    If diffAmount <> 0 Then
                                        Dim authId As String = If(cmbCashier.SelectedValue IsNot Nothing, cmbCashier.SelectedValue.ToString(), "")
                                        Dim adjSql As String = "INSERT INTO sales_adjustments (inv_no, adjustment_date, difference_amount, cashier_id, reason, is_rgr) VALUES (@inv_no, @now_local, @diff, @cid, 'Invoice Update', @is_rgr)"
                                        Using cmdAdj As New MySqlCommand(adjSql, conn, dbTrans)
                                            cmdAdj.Parameters.AddWithValue("@inv_no", existingInvNo)
                                            cmdAdj.Parameters.AddWithValue("@diff", diffAmount)
                                            cmdAdj.Parameters.AddWithValue("@cid", authId)
                                            cmdAdj.Parameters.AddWithValue("@now_local", DateTime.Now)
                                            cmdAdj.Parameters.AddWithValue("@is_rgr", If(Module1.IsRgrModeActive, 1, 0))
                                            cmdAdj.ExecuteNonQuery()
                                        End Using
                                    End If

                                    ' Store Credit Note Insertion if user elected Store Credit in Waterfall
                                    If applyToStoreCredit > 0 Then
                                        Dim cidVal As String = If(String.IsNullOrEmpty(selectedCustomerId), "CASH_CUSTOMER", selectedCustomerId)
                                        Dim snSql As String = "INSERT INTO customer_credit_notes (customer_id, inv_no, credit_amount, is_rgr) VALUES (@cid, @inv, @amt, @is_rgr)"
                                        Using cmdSn As New MySqlCommand(snSql, conn, dbTrans)
                                            cmdSn.Parameters.AddWithValue("@cid", cidVal)
                                            cmdSn.Parameters.AddWithValue("@inv", existingInvNo)
                                            cmdSn.Parameters.AddWithValue("@amt", applyToStoreCredit)
                                            cmdSn.Parameters.AddWithValue("@is_rgr", If(Module1.IsRgrModeActive, 1, 0))
                                            cmdSn.ExecuteNonQuery()
                                        End Using
                                    End If
                                End If

                                ' UPDATE CUSTOMER CREDIT TABLE if this invoice is tracked there
                                ' Only updates the amount if a record for this inv_no already exists in customer_credit
                                If isUpdate AndAlso Not isQuote AndAlso creditBalanceDue >= 0 Then
                                    Dim creditInvNo As String = existingInvNo
                                    If Module1.IsRgrModeActive AndAlso Not String.IsNullOrEmpty(printedInvNo) Then
                                        creditInvNo = printedInvNo
                                    End If
                                    Dim updateCreditSql As String =
                                        "UPDATE customer_credit SET amount = @newAmt WHERE inv_no = @inv"
                                    Using cmdUpdateCredit As New MySqlCommand(updateCreditSql, conn, dbTrans)
                                        cmdUpdateCredit.Parameters.AddWithValue("@newAmt", creditBalanceDue)
                                        cmdUpdateCredit.Parameters.AddWithValue("@inv", creditInvNo)
                                        cmdUpdateCredit.ExecuteNonQuery()
                                    End Using
                                End If

                                ' 3. ITEM PROCESSING
                                Dim oldItemsTable As New DataTable()
                                Dim salesReturnId As Integer = 0
                                If isUpdate Then
                                    Dim cmdOld = New MySqlCommand("SELECT item_id, description, quantity, unit_price, discount, item_cost FROM " & itemTable & " WHERE billing_id = @bid", conn, dbTrans)
                                    cmdOld.Parameters.AddWithValue("@bid", billingId)
                                    Using drOld = cmdOld.ExecuteReader()
                                        oldItemsTable.Load(drOld)
                                    End Using
                                    ' --- BACKUP OLD ITEMS TO HISTORY TABLE ---
                                    Dim backupSql As String = "INSERT INTO billing_item_history (billing_id, inv_no, item_id, description, quantity, unit_price, discount, item_cost, revision_date, revised_by) " &
                                                              "SELECT billing_id, @inv_no, item_id, description, quantity, unit_price, discount, item_cost, @now_local, @user_id " &
                                                              "FROM " & itemTable & " WHERE billing_id = @bid"

                                    Using cmdBackup As New MySqlCommand(backupSql, conn, dbTrans)
                                        cmdBackup.Parameters.AddWithValue("@bid", billingId)
                                        cmdBackup.Parameters.AddWithValue("@inv_no", existingInvNo)
                                        cmdBackup.Parameters.AddWithValue("@user_id", If(cmbCashier.SelectedValue Is Nothing, DBNull.Value, cmbCashier.SelectedValue))
                                        cmdBackup.Parameters.AddWithValue("@now_local", DateTime.Now)
                                        cmdBackup.ExecuteNonQuery()
                                    End Using
                                    ' -----------------------------------------

                                    ' 1. Delete items and DUPLICATE PROFIT RECORDS for this bill before re-inserting
                                    Dim cmdDelDaily = New MySqlCommand("DELETE FROM daily_sale WHERE billing_item_id IN (SELECT id FROM " & itemTable & " WHERE billing_id = @bid)", conn, dbTrans)
                                    cmdDelDaily.Parameters.AddWithValue("@bid", billingId)
                                    cmdDelDaily.ExecuteNonQuery()

                                    Dim cmdDel = New MySqlCommand("DELETE FROM " & itemTable & " WHERE billing_id = @bid", conn, dbTrans)
                                    cmdDel.Parameters.AddWithValue("@bid", billingId)
                                    cmdDel.ExecuteNonQuery()
                                End If

                                Dim LogReturn = Sub(r_itemId As String, r_desc As String, r_qtyDiff As Decimal, r_price As Decimal, r_disc As Decimal, r_cost As Decimal, r_reason As String)
                                                    If salesReturnId = 0 Then
                                                        Dim headSql = "INSERT INTO sales_return (inv_no, customer_id, return_date, cash_type, user_id) " &
                                                                      "VALUES (@inv, @cus, @now_local, @ctype, @uid); SELECT LAST_INSERT_ID();"
                                                        Using cmdHead = New MySqlCommand(headSql, conn, dbTrans)
                                                            cmdHead.Parameters.AddWithValue("@inv", existingInvNo)
                                                            cmdHead.Parameters.AddWithValue("@cus", If(String.IsNullOrEmpty(selectedCustomerId), DBNull.Value, selectedCustomerId))
                                                            cmdHead.Parameters.AddWithValue("@ctype", bType) ' Use dynamic billing type
                                                            cmdHead.Parameters.AddWithValue("@uid", If(cmbCashier.SelectedValue Is Nothing, DBNull.Value, cmbCashier.SelectedValue))
                                                            cmdHead.Parameters.AddWithValue("@now_local", DateTime.Now)
                                                            salesReturnId = Convert.ToInt32(cmdHead.ExecuteScalar())
                                                        End Using
                                                    End If

                                                    Dim netPrice = r_price - (r_price * r_disc / 100)
                                                    Dim retQty = Math.Abs(r_qtyDiff)
                                                    Dim retAmt = retQty * netPrice
                                                    Dim retProfit = (netPrice - r_cost) * retQty

                                                    Dim itemSql = "INSERT INTO sales_return_items (return_id, item_id, description, qty, unit_price, discount, return_amount, reason, cost_price, return_profit) " &
                                                                  "VALUES (@rid, @it, @des, @qty, @price, @dis, @ramt, @reason, @cost, @prof)"
                                                    Using cmdItem = New MySqlCommand(itemSql, conn, dbTrans)
                                                        cmdItem.Parameters.AddWithValue("@rid", salesReturnId)
                                                        cmdItem.Parameters.AddWithValue("@it", r_itemId)
                                                        cmdItem.Parameters.AddWithValue("@des", r_desc)
                                                        cmdItem.Parameters.AddWithValue("@qty", retQty)
                                                        cmdItem.Parameters.AddWithValue("@price", r_price)
                                                        cmdItem.Parameters.AddWithValue("@dis", r_disc)
                                                        cmdItem.Parameters.AddWithValue("@ramt", retAmt)
                                                        cmdItem.Parameters.AddWithValue("@reason", r_reason) ' Use per-item reason
                                                        cmdItem.Parameters.AddWithValue("@cost", r_cost)
                                                        cmdItem.Parameters.AddWithValue("@prof", retProfit)
                                                        cmdItem.ExecuteNonQuery()
                                                    End Using

                                                    ' Update refund amount and total profit loss in header
                                                    Dim updSql = "UPDATE sales_return SET refund_amount = refund_amount + @ramt, total_return_profit = total_return_profit + @prof WHERE id = @rid"
                                                    Using cmdUpd = New MySqlCommand(updSql, conn, dbTrans)
                                                        cmdUpd.Parameters.AddWithValue("@ramt", retAmt)
                                                        cmdUpd.Parameters.AddWithValue("@prof", retProfit)
                                                        cmdUpd.Parameters.AddWithValue("@rid", salesReturnId)
                                                        cmdUpd.ExecuteNonQuery()
                                                    End Using
                                                End Sub

                                Dim currentBillingItemId As Long = 0
                                For Each row As DataRow In dtBill.Rows
                                    Dim itemId As String = row("Item ID").ToString()
                                    Dim desc As String = row("Description").ToString()
                                    Dim qty As Decimal = 0
                                    Decimal.TryParse(row("Qty").ToString(), qty)
                                    Dim uPrice As Decimal = 0
                                    Decimal.TryParse(row("Selling Price").ToString(), uPrice)
                                    Dim disc As Decimal = 0
                                    Decimal.TryParse(row("Dis").ToString(), disc)
                                    Dim locationName As String = row("Location").ToString()
                                    Dim locationId As Long = 1
                                    If dtBill.Columns.Contains("LocationID") Then Long.TryParse(row("LocationID").ToString(), locationId)
                                    Dim itemCost As Decimal = 0
                                    If dtBill.Columns.Contains("ItemCost") AndAlso Not IsDBNull(row("ItemCost")) Then
                                        Decimal.TryParse(row("ItemCost").ToString(), itemCost)
                                    End If
                                    ' Fallback: if itemCost is still 0, pull it from the DB to avoid avg_cost NULL errors
                                    If itemCost = 0 AndAlso Not String.IsNullOrEmpty(itemId) Then
                                        Try
                                            Using cmdCost As New MySqlCommand(
                                                "SELECT IFNULL(avg_cost, IFNULL(item_cost, 0)) FROM items WHERE id = @iid LIMIT 1", conn, dbTrans)
                                                cmdCost.Parameters.AddWithValue("@iid", itemId)
                                                Dim costRes = cmdCost.ExecuteScalar()
                                                If costRes IsNot Nothing AndAlso costRes IsNot DBNull.Value Then
                                                    Decimal.TryParse(costRes.ToString(), itemCost)
                                                End If
                                            End Using
                                        Catch : End Try
                                    End If

                                    Dim qtyDiff As Decimal = qty
                                    If isUpdate Then
                                        Dim foundRows = oldItemsTable.Select("item_id = '" & itemId.Replace("'", "''") & "'")
                                        If foundRows.Length > 0 Then
                                            Dim oldQty = Convert.ToDecimal(foundRows(0)("quantity"))
                                            qtyDiff = qty - oldQty
                                            If qtyDiff < 0 AndAlso billType <> "Quote" Then
                                                Dim itemReason = row("Reason").ToString()
                                                Dim cleanReason = itemReason.Replace("[AddStock] ", "").Replace("[NoStock] ", "")
                                                LogReturn(itemId, desc, qtyDiff, uPrice, disc, itemCost, cleanReason)
                                            End If
                                            foundRows(0).Delete()
                                        End If
                                    End If

                                    If billType <> "Quote" Then
                                        Dim adjustStock As Boolean = True
                                        If qtyDiff < 0 Then
                                            Dim itemReason = row("Reason").ToString()
                                            If itemReason = "Damaged" OrElse itemReason = "Expired" Then
                                                adjustStock = False
                                            ElseIf itemReason.Contains("[NoStock]") Then
                                                adjustStock = False
                                            ElseIf itemReason.Contains("[AddStock]") Then
                                                adjustStock = True
                                            End If
                                        End If
                                        If adjustStock Then
                                            Dim adjustSql = "UPDATE items SET st_qty = st_qty - @diff WHERE id = @id"
                                            Using cmdAdjust As New MySqlCommand(adjustSql, conn, dbTrans)
                                                cmdAdjust.Parameters.AddWithValue("@diff", qtyDiff)
                                                cmdAdjust.Parameters.AddWithValue("@id", itemId)
                                                cmdAdjust.ExecuteNonQuery()
                                            End Using
                                            If qtyDiff > 0 Then
                                                Dim remainingToDeduct = qtyDiff
                                                Dim fetchBatchesSql = "SELECT id, st_qty FROM items_stock WHERE item_id = @id AND location_id = @locid AND st_qty > 0 ORDER BY date ASC"
                                                Dim batchesTable As New DataTable()
                                                Using cmdFetchBatches As New MySqlCommand(fetchBatchesSql, conn, dbTrans)
                                                    cmdFetchBatches.Parameters.AddWithValue("@id", itemId)
                                                    cmdFetchBatches.Parameters.AddWithValue("@locid", locationId)
                                                    Using drBatches = cmdFetchBatches.ExecuteReader()
                                                        batchesTable.Load(drBatches)
                                                    End Using
                                                End Using
                                                For Each batchRow As DataRow In batchesTable.Rows
                                                    If remainingToDeduct <= 0 Then Exit For
                                                    Dim deductNow = Math.Min(remainingToDeduct, Convert.ToDecimal(batchRow("st_qty")))
                                                    Using cmdUpdateBatch As New MySqlCommand("UPDATE items_stock SET st_qty = st_qty - @deduct WHERE id = @bid", conn, dbTrans)
                                                        cmdUpdateBatch.Parameters.AddWithValue("@deduct", deductNow)
                                                        cmdUpdateBatch.Parameters.AddWithValue("@bid", batchRow("id"))
                                                        cmdUpdateBatch.ExecuteNonQuery()
                                                    End Using
                                                    remainingToDeduct -= deductNow
                                                Next
                                                If remainingToDeduct > 0 Then
                                                    Using cmdBatchAdjust As New MySqlCommand("UPDATE items_stock SET st_qty = st_qty - @rem WHERE item_id = @id AND location_id = @locid ORDER BY date DESC LIMIT 1", conn, dbTrans)
                                                        cmdBatchAdjust.Parameters.AddWithValue("@rem", remainingToDeduct)
                                                        cmdBatchAdjust.Parameters.AddWithValue("@id", itemId)
                                                        cmdBatchAdjust.Parameters.AddWithValue("@locid", locationId)
                                                        If cmdBatchAdjust.ExecuteNonQuery() = 0 Then
                                                            Using cmdIns As New MySqlCommand("INSERT INTO items_stock (id, item_id, item_cost, st_qty, date, location_id, supplier_id, avg_cost, selling_price, whole_selling_price, retail_selling_price) " &
                                                                "VALUES (@batch_id, @id, @cost, -@rem, @now_local, @locid, 1, @avg, @sell, @sell, @sell)", conn, dbTrans)
                                                                cmdIns.Parameters.AddWithValue("@batch_id", existingInvNo & "_neg_" & Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10))
                                                                cmdIns.Parameters.AddWithValue("@id", itemId)
                                                                cmdIns.Parameters.AddWithValue("@cost", itemCost)
                                                                cmdIns.Parameters.AddWithValue("@avg", If(itemCost > 0, itemCost, uPrice))
                                                                cmdIns.Parameters.AddWithValue("@sell", uPrice)
                                                                cmdIns.Parameters.AddWithValue("@rem", remainingToDeduct)
                                                                cmdIns.Parameters.AddWithValue("@now_local", DateTime.Now)
                                                                cmdIns.Parameters.AddWithValue("@locid", locationId)
                                                                cmdIns.ExecuteNonQuery()
                                                            End Using
                                                        End If
                                                    End Using
                                                End If
                                            ElseIf qtyDiff < 0 Then
                                                Using cmdBatchAdjust As New MySqlCommand("UPDATE items_stock SET st_qty = st_qty - @diff WHERE item_id = @id AND location_id = (SELECT id FROM location WHERE location_name='MAIN STOCK' LIMIT 1) ORDER BY date DESC LIMIT 1", conn, dbTrans)
                                                    cmdBatchAdjust.Parameters.AddWithValue("@diff", qtyDiff)
                                                    cmdBatchAdjust.Parameters.AddWithValue("@id", itemId)
                                                    If cmdBatchAdjust.ExecuteNonQuery() = 0 Then
                                                        Using cmdIns As New MySqlCommand("INSERT INTO items_stock (id, item_id, item_cost, st_qty, date, location_id, supplier_id, avg_cost, selling_price, whole_selling_price, retail_selling_price) " &
                                                            "VALUES (@batch_id, @id, @cost, @ret_qty, @now_local, (SELECT id FROM location WHERE location_name='MAIN STOCK' LIMIT 1), 1, @avg, @sell, @wsell, @rsell)", conn, dbTrans)
                                                            cmdIns.Parameters.AddWithValue("@batch_id", Guid.NewGuid().ToString())
                                                            cmdIns.Parameters.AddWithValue("@id", itemId)
                                                            cmdIns.Parameters.AddWithValue("@cost", itemCost)
                                                            cmdIns.Parameters.AddWithValue("@avg", If(itemCost > 0, itemCost, uPrice))
                                                            cmdIns.Parameters.AddWithValue("@sell", uPrice)
                                                            cmdIns.Parameters.AddWithValue("@wsell", uPrice)
                                                            cmdIns.Parameters.AddWithValue("@rsell", uPrice)
                                                            cmdIns.Parameters.AddWithValue("@ret_qty", Math.Abs(qtyDiff))
                                                            cmdIns.Parameters.AddWithValue("@now_local", DateTime.Now)
                                                            cmdIns.ExecuteNonQuery()
                                                        End Using
                                                    End If
                                                End Using
                                            End If
                                        End If
                                        InsertIntoAllItem(itemId, desc, qtyDiff, itemCost, uPrice, disc, billType, bType, statusValue, CheckBoxIsVat.Checked)
                                    End If

                                    Dim insertItemSql = "INSERT INTO " & itemTable & " (billing_id, item_id, description, quantity, unit_price, item_amount, discount, stock_id, item_cost, location, print_retail_price) VALUES (@billing_id, @item_id, @description, @quantity, @unit_price, @item_amount, @discount, 1, @item_cost, @location, @print_retail)"
                                    Using cmdInsert As New MySqlCommand(insertItemSql, conn, dbTrans)
                                        Dim rowTotal As Decimal = 0
                                        If row("Total/Amount") IsNot DBNull.Value Then Decimal.TryParse(row("Total/Amount").ToString(), rowTotal)
                                        cmdInsert.Parameters.AddWithValue("@billing_id", billingId)
                                        cmdInsert.Parameters.AddWithValue("@item_id", itemId)
                                        cmdInsert.Parameters.AddWithValue("@description", desc)
                                        cmdInsert.Parameters.AddWithValue("@quantity", qty)
                                        cmdInsert.Parameters.AddWithValue("@unit_price", uPrice)
                                        cmdInsert.Parameters.AddWithValue("@item_amount", rowTotal)
                                        cmdInsert.Parameters.AddWithValue("@discount", disc)
                                        Dim printRetailVal As Decimal = If(dtBill.Columns.Contains("PrintRetailPrice") AndAlso Not IsDBNull(row("PrintRetailPrice")), Convert.ToDecimal(row("PrintRetailPrice")), 0D)
                                        cmdInsert.Parameters.AddWithValue("@item_cost", itemCost)
                                        cmdInsert.Parameters.AddWithValue("@print_retail", printRetailVal)
                                        cmdInsert.Parameters.AddWithValue("@location", locationName)
                                        cmdInsert.ExecuteNonQuery()
                                        currentBillingItemId = cmdInsert.LastInsertedId
                                    End Using

                                    If billType <> "Quote" Then
                                        Dim rowTotalVal As Decimal = 0
                                        If row("Total/Amount") IsNot DBNull.Value Then Decimal.TryParse(row("Total/Amount").ToString(), rowTotalVal)
                                        Using cmdDaily As New MySqlCommand("INSERT INTO daily_sale (item_id, billing_item_id, amount, profit, sale_time, is_rgr) VALUES (@it, @bit, @amt, @prof, @now_local, @is_rgr)", conn, dbTrans)
                                            cmdDaily.Parameters.AddWithValue("@it", itemId)
                                            cmdDaily.Parameters.AddWithValue("@bit", currentBillingItemId)
                                            cmdDaily.Parameters.AddWithValue("@amt", rowTotalVal)
                                            cmdDaily.Parameters.AddWithValue("@prof", rowTotalVal - (itemCost * qty))
                                            cmdDaily.Parameters.AddWithValue("@now_local", DateTime.Now)
                                            cmdDaily.Parameters.AddWithValue("@is_rgr", If(Module1.IsRgrModeActive, 1, 0))
                                            cmdDaily.ExecuteNonQuery()
                                        End Using
                                    End If
                                Next

                                If isUpdate Then
                                    oldItemsTable.AcceptChanges()
                                    For Each oldRow As DataRow In oldItemsTable.Rows
                                        Dim itId = oldRow("item_id").ToString()
                                        Dim itQty = Convert.ToDecimal(oldRow("quantity"))
                                        If billType <> "Quote" AndAlso cmbReturnReason.Text <> "Damaged" AndAlso cmbReturnReason.Text <> "Expired" Then
                                            Using cmdBackMaster = New MySqlCommand("UPDATE items SET st_qty = st_qty + @qty WHERE id = @id", conn, dbTrans)
                                                cmdBackMaster.Parameters.AddWithValue("@qty", itQty)
                                                cmdBackMaster.Parameters.AddWithValue("@id", itId)
                                                cmdBackMaster.ExecuteNonQuery()
                                            End Using
                                            Using cmdBackBatch = New MySqlCommand("UPDATE items_stock SET st_qty = st_qty + @qty WHERE item_id = @id AND location_id = (SELECT id FROM location WHERE location_name='MAIN STOCK' LIMIT 1) ORDER BY date DESC LIMIT 1", conn, dbTrans)
                                                cmdBackBatch.Parameters.AddWithValue("@qty", itQty)
                                                cmdBackBatch.Parameters.AddWithValue("@id", itId)
                                                If cmdBackBatch.ExecuteNonQuery() = 0 Then
                                                    Using cmdInsB = New MySqlCommand("INSERT INTO items_stock (id, item_id, item_cost, st_qty, date, location_id, supplier_id, avg_cost, selling_price, whole_selling_price, retail_selling_price) " &
                                                                    "VALUES (@batch_id, @id, 0, @qty, @now_local, (SELECT id FROM location WHERE location_name='MAIN STOCK' LIMIT 1), 1, 0, 0, 0, 0)", conn, dbTrans)
                                                        cmdInsB.Parameters.AddWithValue("@batch_id", Guid.NewGuid().ToString())
                                                        cmdInsB.Parameters.AddWithValue("@id", itId)
                                                        cmdInsB.Parameters.AddWithValue("@qty", itQty)
                                                        cmdInsB.Parameters.AddWithValue("@now_local", DateTime.Now)
                                                        cmdInsB.ExecuteNonQuery()
                                                    End Using
                                                End If
                                            End Using
                                            LogReturn(itId, oldRow("description").ToString(), -itQty, Convert.ToDecimal(oldRow("unit_price")), Convert.ToDecimal(oldRow("discount")), Convert.ToDecimal(oldRow("item_cost")), cmbReturnReason.Text)
                                            InsertIntoAllItem(itId, "Removed from " & existingInvNo, -itQty, 0, 0, 0, billType, bType, statusValue, CheckBoxIsVat.Checked)
                                        End If
                                    Next
                                End If

                                If CheckBoxIsVat.Checked Then
                                    ' 1. Consume primary sequence (RGR or INV)
                                    ' For Updates, we ONLY do this if it's a VT bill (to track manual number changes)
                                    Dim tableNameForFinal As String
                                    If ComboBox1.Text = "Quote" Then
                                        tableNameForFinal = "inv_no_qt1"
                                    ElseIf Module1.IsRgrModeActive Then
                                        tableNameForFinal = "inv_no_RGR1"
                                    ElseIf CheckBoxIsVat.Checked Then
                                        tableNameForFinal = "inv_no_VT1"
                                    Else
                                        tableNameForFinal = If(isElBill, "inv_no_el1", "inv_no_gr1")
                                    End If

                                    Dim finalNumericPart As Integer = 0
                                    Using cmdFetchM = New MySqlCommand("SELECT MAX(id) FROM " & tableNameForFinal, conn, dbTrans)
                                        Dim finalMax = cmdFetchM.ExecuteScalar()
                                        ' If VT, extract numeric part from potentially manually edited existingInvNo
                                        If CheckBoxIsVat.Checked Then
                                            Dim manualNum As Integer = 0
                                            Dim match = System.Text.RegularExpressions.Regex.Match(existingInvNo, "\d+$")
                                            If match.Success AndAlso Integer.TryParse(match.Value, manualNum) Then
                                                finalNumericPart = manualNum
                                            Else
                                                finalNumericPart = If(finalMax Is DBNull.Value OrElse finalMax Is Nothing, 0, Convert.ToInt32(finalMax)) + 1
                                            End If
                                        Else
                                            finalNumericPart = If(finalMax Is DBNull.Value OrElse finalMax Is Nothing, 0, Convert.ToInt32(finalMax)) + 1
                                        End If
                                    End Using

                                    ' Use INSERT IGNORE or check before insert to avoid primary key error during updates
                                    Dim checkExistsSql = "SELECT COUNT(*) FROM " & tableNameForFinal & " WHERE id = @id"
                                    Dim existsCount As Integer = 0
                                    Using cmdCheckE = New MySqlCommand(checkExistsSql, conn, dbTrans)
                                        cmdCheckE.Parameters.AddWithValue("@id", finalNumericPart)
                                        existsCount = Convert.ToInt32(cmdCheckE.ExecuteScalar())
                                    End Using

                                    If existsCount = 0 Then
                                        Using cmdFinal = New MySqlCommand("INSERT INTO " & tableNameForFinal & " (id) VALUES (@id)", conn, dbTrans)
                                            cmdFinal.Parameters.AddWithValue("@id", finalNumericPart)
                                            cmdFinal.ExecuteNonQuery()
                                        End Using
                                    End If

                                    ' 2. ALSO consume standard 'gr' sequence if this was a Masked RGR
                                    If Module1.IsRgrModeActive AndAlso Not isQuote Then
                                        Dim grTable As String = "inv_no_gr1"
                                        Dim grNumericPart As Integer = 0
                                        Using cmdFetchGR = New MySqlCommand("SELECT MAX(id) FROM " & grTable, conn, dbTrans)
                                            Dim grMax = cmdFetchGR.ExecuteScalar()
                                            grNumericPart = If(grMax Is DBNull.Value OrElse grMax Is Nothing, 0, Convert.ToInt32(grMax)) + 1
                                        End Using
                                        Using cmdFinalGR = New MySqlCommand("INSERT INTO " & grTable & " (id) VALUES (@id)", conn, dbTrans)
                                            cmdFinalGR.Parameters.AddWithValue("@id", grNumericPart)
                                            cmdFinalGR.ExecuteNonQuery()
                                        End Using
                                    End If
                                End If

                                If billType <> "Quote" Then
                                    Dim totalBillProfit As Decimal = 0
                                    For Each rowProfit As DataRow In dtBill.Rows
                                        Dim itId = rowProfit("Item ID").ToString()
                                        Dim q As Decimal = 0
                                        If rowProfit("Qty") IsNot DBNull.Value Then Decimal.TryParse(rowProfit("Qty").ToString(), q)
                                        Dim amt As Decimal = 0
                                        If rowProfit("Total/Amount") IsNot DBNull.Value Then Decimal.TryParse(rowProfit("Total/Amount").ToString(), amt)
                                        Dim itTotalCost As Decimal = 0
                                        Dim remQ = q
                                        Try
                                            Using localProfConn2 = New MySqlConnection(ConnStr)
                                                localProfConn2.Open()
                                                Using cmdBatch = New MySqlCommand("SELECT st_qty, item_cost FROM items_stock WHERE item_id = @id AND st_qty > 0 ORDER BY date ASC", localProfConn2)
                                                    cmdBatch.Parameters.AddWithValue("@id", itId)
                                                    Using drBatch = cmdBatch.ExecuteReader()
                                                        While drBatch.Read() AndAlso remQ > 0
                                                            Dim taken = Math.Min(remQ, Convert.ToDecimal(drBatch("st_qty")))
                                                            itTotalCost += (taken * Convert.ToDecimal(drBatch("item_cost")))
                                                            remQ -= taken
                                                        End While
                                                    End Using
                                                End Using
                                            End Using
                                        Catch : End Try
                                        Dim rowProfitAvgCost As Decimal = 0
                                        If dtBill.Columns.Contains("AvgCost") AndAlso rowProfit("AvgCost") IsNot DBNull.Value Then
                                            Decimal.TryParse(rowProfit("AvgCost").ToString(), rowProfitAvgCost)
                                        End If
                                        If remQ > 0 Then itTotalCost += (remQ * rowProfitAvgCost)
                                        totalBillProfit += (amt - itTotalCost)
                                    Next

                                    Dim monthlySql = If(isUpdate, "UPDATE monthly_income SET total_amount=@amt, total_profit=@prof WHERE billing_id=@bid", "INSERT INTO monthly_income (billing_id, total_amount, total_profit, month_year, is_rgr) VALUES (@bid, @amt, @prof, @my, @is_rgr)")
                                    Using cmdMonth = New MySqlCommand(monthlySql, conn, dbTrans)
                                        cmdMonth.Parameters.AddWithValue("@bid", billingId)
                                        cmdMonth.Parameters.AddWithValue("@amt", gTotal)
                                        cmdMonth.Parameters.AddWithValue("@prof", totalBillProfit)
                                        If Not isUpdate Then
                                            cmdMonth.Parameters.AddWithValue("@my", DateTime.Now.ToString("MM-yyyy"))
                                            cmdMonth.Parameters.AddWithValue("@is_rgr", If(Module1.IsRgrModeActive, 1, 0))
                                        End If
                                        cmdMonth.ExecuteNonQuery()
                                    End Using

                                    ' --- AUTO SAVE/UPDATE CHEQUE TO CUSTOMER CHEQUES TABLE ---
                                    If Not isQuote Then
                                        Try
                                            Dim chequeInvNo As String = existingInvNo
                                            If Module1.IsRgrModeActive AndAlso Not String.IsNullOrEmpty(printedInvNo) Then
                                                chequeInvNo = printedInvNo
                                            End If

                                            If chequeBalanceDue > 0 AndAlso Not String.IsNullOrWhiteSpace(chequeNo) Then
                                                Dim checkChqExistSql As String = "SELECT status FROM check_received WHERE inv_no = @inv LIMIT 1"
                                                Dim existsCount As Integer = 0
                                                Dim existingStatus As String = ""
                                                Using cmdCheckChq As New MySqlCommand(checkChqExistSql, conn, dbTrans)
                                                    cmdCheckChq.Parameters.AddWithValue("@inv", chequeInvNo)
                                                    Using drChq As MySqlDataReader = cmdCheckChq.ExecuteReader()
                                                        If drChq.Read() Then
                                                            existsCount = 1
                                                            existingStatus = If(drChq("status") Is DBNull.Value, "", drChq("status").ToString())
                                                        End If
                                                    End Using
                                                End Using

                                                Dim finalBankId As Integer = 1
                                                If bankIdValue IsNot Nothing AndAlso Not IsDBNull(bankIdValue) Then
                                                    Integer.TryParse(bankIdValue.ToString(), finalBankId)
                                                End If

                                                If existsCount > 0 Then
                                                    ' If it exists, update details if status is Pending
                                                    If String.Equals(existingStatus, "Pending", StringComparison.OrdinalIgnoreCase) Then
                                                        Dim updateChqSql As String = "UPDATE check_received SET check_number = @chq, check_name = @name, bank_id = @bank, amount = @amt, check_release_date = @release, issue_date = @issue WHERE inv_no = @inv"
                                                        Using cmdUpdateChq As New MySqlCommand(updateChqSql, conn, dbTrans)
                                                            cmdUpdateChq.Parameters.AddWithValue("@chq", chequeNo.Trim())
                                                            cmdUpdateChq.Parameters.AddWithValue("@name", txtSalesRep.Text.Trim())
                                                            cmdUpdateChq.Parameters.AddWithValue("@bank", finalBankId)
                                                            cmdUpdateChq.Parameters.AddWithValue("@amt", chequeBalanceDue)
                                                            cmdUpdateChq.Parameters.AddWithValue("@release", chequeDateValue.ToString("yyyy-MM-dd"))
                                                            cmdUpdateChq.Parameters.AddWithValue("@issue", DateTime.Now.ToString("yyyy-MM-dd"))
                                                            cmdUpdateChq.Parameters.AddWithValue("@inv", chequeInvNo)
                                                            cmdUpdateChq.ExecuteNonQuery()
                                                        End Using
                                                    End If
                                                Else
                                                    ' Insert new cheque
                                                    Dim insertChqSql As String = "INSERT INTO check_received (check_number, check_name, bank_id, amount, status, issue_date, check_release_date, inv_no) " &
                                                                                 "VALUES (@chq, @name, @bank, @amt, 'Pending', @issue, @release, @inv)"
                                                    Using cmdInsertChq As New MySqlCommand(insertChqSql, conn, dbTrans)
                                                        cmdInsertChq.Parameters.AddWithValue("@chq", chequeNo.Trim())
                                                        cmdInsertChq.Parameters.AddWithValue("@name", txtSalesRep.Text.Trim())
                                                        cmdInsertChq.Parameters.AddWithValue("@bank", finalBankId)
                                                        cmdInsertChq.Parameters.AddWithValue("@amt", chequeBalanceDue)
                                                        cmdInsertChq.Parameters.AddWithValue("@issue", DateTime.Now.ToString("yyyy-MM-dd"))
                                                        cmdInsertChq.Parameters.AddWithValue("@release", chequeDateValue.ToString("yyyy-MM-dd"))
                                                        cmdInsertChq.Parameters.AddWithValue("@inv", chequeInvNo)
                                                        cmdInsertChq.ExecuteNonQuery()
                                                    End Using
                                                End If
                                            Else
                                                ' If no cheque is associated with the bill now, but it's an update, delete any existing pending cheques for this invoice
                                                If isUpdate Then
                                                    Dim deleteChqSql As String = "DELETE FROM check_received WHERE inv_no = @inv AND status = 'Pending'"
                                                    Using cmdDeleteChq As New MySqlCommand(deleteChqSql, conn, dbTrans)
                                                        cmdDeleteChq.Parameters.AddWithValue("@inv", chequeInvNo)
                                                        cmdDeleteChq.ExecuteNonQuery()
                                                    End Using
                                                End If
                                            End If
                                        Catch ex As Exception
                                            MessageBox.Show("Error saving cheque to Customer Cheques: " & ex.Message, "Cheque Sync Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                        End Try
                                    End If
                                End If

                                dbTrans.Commit()

                                Dim msg = If(isUpdate, "Invoice Updated Successfully!", "Bill Saved Successfully!")

                                ' -----------------------------------------------------
                                ' DYNAMIC PRINTING LOGIC (Normal vs Return)
                                ' -----------------------------------------------------
                                ' Check if this is a "True Return" (Negative qty OR has a Return Reason)
                                Dim isTrueReturn As Boolean = False
                                For Each rowCheck As DataRow In dtBill.Rows
                                    Dim checkQ As Decimal = 0
                                    If rowCheck("Qty") IsNot DBNull.Value Then Decimal.TryParse(rowCheck("Qty").ToString(), checkQ)
                                    Dim reasonText As String = If(rowCheck("Reason") Is DBNull.Value, "", rowCheck("Reason").ToString().Trim())

                                    ' Detection: Negative quantity OR a non-empty Return Reason (that isn't "None")
                                    If checkQ < 0 OrElse (Not String.IsNullOrEmpty(reasonText) AndAlso Not String.Equals(reasonText, "None", StringComparison.OrdinalIgnoreCase)) Then
                                        isTrueReturn = True
                                        Exit For
                                    End If
                                Next

                                ' -----------------------------------------------------
                                ' DYNAMIC PRINTING LOGIC WITH PREVIEW
                                ' -----------------------------------------------------
                                Try
                                    ' Determine which number to print: Use masked 'gr' number for RGR bills
                                    Dim printNo As String = existingInvNo
                                    If Module1.IsRgrModeActive AndAlso Not String.IsNullOrEmpty(printedInvNo) Then
                                        printNo = printedInvNo
                                    End If
                                    If lblInvoiceNumber.Text.Trim().StartsWith("EL", StringComparison.OrdinalIgnoreCase) Then
                                        If MessageBox.Show("Do you want to print the bill?", "Print EL Bill", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                                            Dim rptForm As New SaleInv()
                                            rptForm.PrintAsRetail = CheckBoxPrintAsRetail.Checked
                                            ' Load POS report silently
                                            rptForm.ShowReport(printNo, 0, True, isTrueReturn, "", 1, 0, isUpdate)
                                            ' Direct print to POS printer
                                            rptForm.DirectPrint(POSPrinterName)
                                            ' Close/dispose the silent form to prevent resource leak
                                            rptForm.Close()
                                        End If
                                    Else
                                        ' 1. Show Preview form first (Standard A4 layout by default)
                                        Dim rptForm As New SaleInv()
                                        rptForm.PrintAsRetail = CheckBoxPrintAsRetail.Checked
                                        Dim previewIndex As Integer = If(ComboBox1.Text = "Quote", 4, 1)
                                        rptForm.ShowReport(printNo, previewIndex, False, isTrueReturn, "", 1, 0, isUpdate)

                                        ' Ensure the form paints before the modal MessageBox blocks the thread
                                        rptForm.Refresh()
                                        Application.DoEvents()

                                        ' 2. Ask user for printer type while preview is visible
                                        Dim printChoice As DialogResult
                                        If ComboBox1.Text = "Quote" Then
                                            printChoice = MessageBox.Show(msg & vbCrLf & vbCrLf & "Do you want to Save as PDF?" & vbCrLf & "(Yes = Microsoft PDF, No = Print Standard, Cancel = Close without Print)", "Quotation Print Selection", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)
                                        Else
                                            printChoice = MessageBox.Show(msg & vbCrLf & vbCrLf & "Do you want to print a POS Bill?" & vbCrLf & "(Yes = POS, No = Standard A4, Cancel = Close without Print)", "Print Selection", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)
                                        End If

                                        If printChoice <> DialogResult.Cancel Then
                                            Dim rptIndex As Integer = 0
                                            Dim printerToUse As String = ""

                                            If ComboBox1.Text = "Quote" Then
                                                ' Quotation Logic
                                                rptIndex = 4 ' Standard Quotation
                                                If printChoice = DialogResult.Yes Then
                                                    printerToUse = "Microsoft Print to PDF"
                                                Else
                                                    printerToUse = StandardPrinterName
                                                End If
                                                ' (Already loaded as Quotation Standard in step 1)
                                            Else
                                                ' Sale Logic
                                                If printChoice = DialogResult.Yes Then
                                                    ' POS Print Selection
                                                    rptIndex = 0
                                                    printerToUse = POSPrinterName
                                                    ' Update report to POS silently before direct printing
                                                    rptForm.ShowReport(printNo, rptIndex, True, isTrueReturn, "", 1, 0, isUpdate)
                                                Else
                                                    ' Standard A4 Print Selection
                                                    rptIndex = 1
                                                    printerToUse = StandardPrinterName
                                                    ' (Already loaded as A4 in step 1)
                                                End If
                                            End If

                                            ' 3. Direct Print
                                            rptForm.DirectPrint(printerToUse)
                                        End If
                                    End If
                                Catch exRpt As Exception
                                    MessageBox.Show("Printing Failed: " & exRpt.Message, "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                End Try

                                ' -----------------------------------------------------
                                ' LOG CASH TRANSACTION (Physical Cash Only / Cash_Credit / Cash_Cheque / Mixed_Payment)
                                Dim isPhysicalCashPayment As Boolean =
                                    String.Equals(pMethod, "Cash", StringComparison.OrdinalIgnoreCase)
                                Dim isPartCashPayment As Boolean =
                                    String.Equals(statusValue, "cash_Credit", StringComparison.OrdinalIgnoreCase) OrElse
                                    String.Equals(statusValue, "Cash_Credit", StringComparison.OrdinalIgnoreCase) OrElse
                                    String.Equals(statusValue, "cash_Cheque", StringComparison.OrdinalIgnoreCase) OrElse
                                    String.Equals(statusValue, "Cash_Cheque", StringComparison.OrdinalIgnoreCase) OrElse
                                    String.Equals(statusValue, "Mixed_Payment", StringComparison.OrdinalIgnoreCase) OrElse
                                    String.Equals(pMethod, "cash_credit", StringComparison.OrdinalIgnoreCase) OrElse
                                    String.Equals(pMethod, "Cash_Credit", StringComparison.OrdinalIgnoreCase) OrElse
                                    String.Equals(pMethod, "cash_cheque", StringComparison.OrdinalIgnoreCase) OrElse
                                    String.Equals(pMethod, "Cash_Cheque", StringComparison.OrdinalIgnoreCase) OrElse
                                    String.Equals(pMethod, "Mixed_Payment", StringComparison.OrdinalIgnoreCase)

                                If ComboBox1.Text <> "Quote" AndAlso (isPhysicalCashPayment OrElse isPartCashPayment) Then

                                    Dim transAmt As Decimal = 0
                                    Dim transType As String = "IN"
                                    Dim transMsg As String = ""

                                    Dim cName As String = txtSalesRep.Text.Trim()
                                    Dim isRealCustomer As Boolean = Not String.IsNullOrEmpty(cName) AndAlso
                                                                    Not String.Equals(cName, "No Customer", StringComparison.OrdinalIgnoreCase) AndAlso
                                                                    Not String.Equals(cName, "Cash", StringComparison.OrdinalIgnoreCase) AndAlso
                                                                    Not String.Equals(cName, "CASH", StringComparison.OrdinalIgnoreCase) AndAlso
                                                                    Not String.Equals(cName, "Cash Customer", StringComparison.OrdinalIgnoreCase)

                                    If isPhysicalCashPayment Then
                                        If isUpdate Then
                                            'Only physical-cash payments affect the cash drawer.
                                            If String.Equals(bType, "Cash", StringComparison.OrdinalIgnoreCase) Then
                                                Dim cashDiff As Decimal = gTotal - oldGrandTotal

                                                If cashDiff > 0 Then
                                                    transAmt = cashDiff
                                                    transType = "IN"
                                                    transMsg = If(isRealCustomer, "Cash Sale Update: " & cName & " (Inv: " & existingInvNo & ")", "Cash Sale Update (Inv: " & existingInvNo & ")")

                                                ElseIf cashDiff < 0 Then
                                                    transAmt = Math.Abs(cashDiff)
                                                    transType = "OUT"
                                                    transMsg = If(isRealCustomer, "Cash Refund: " & cName & " (Inv: " & existingInvNo & ")", "Cash Refund (Inv: " & existingInvNo & ")")
                                                End If
                                            End If
                                        Else
                                            'New bill: add only physical cash received.
                                            transAmt = advancePayment
                                            transType = "IN"
                                            transMsg = If(isRealCustomer, "Cash Sale: " & cName & " (Inv: " & existingInvNo & ")", "Cash Sale: " & existingInvNo)
                                        End If
                                    ElseIf isPartCashPayment Then
                                        Dim paymentLabel As String = "Cash_Credit"
                                        If String.Equals(statusValue, "cash_Cheque", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(pMethod, "cash_cheque", StringComparison.OrdinalIgnoreCase) Then
                                            paymentLabel = "Cash_Cheque"
                                        ElseIf String.Equals(statusValue, "Mixed_Payment", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(pMethod, "Mixed_Payment", StringComparison.OrdinalIgnoreCase) Then
                                            paymentLabel = "Mixed_Payment"
                                        End If

                                        If isUpdate Then
                                            Dim cashDiff As Decimal = cashAmt - oldCashAmount

                                            If cashDiff > 0 Then
                                                transAmt = cashDiff
                                                transType = "IN"
                                                transMsg = If(isRealCustomer, paymentLabel & " Update: " & cName & " (Inv: " & existingInvNo & ")", paymentLabel & " Update (Inv: " & existingInvNo & ")")
                                            ElseIf cashDiff < 0 Then
                                                transAmt = Math.Abs(cashDiff)
                                                transType = "OUT"
                                                transMsg = If(isRealCustomer, paymentLabel & " Refund: " & cName & " (Inv: " & existingInvNo & ")", paymentLabel & " Refund (Inv: " & existingInvNo & ")")
                                            End If
                                        Else
                                            'New bill: add the given cash amount.
                                            transAmt = cashAmt
                                            transType = "IN"
                                            transMsg = If(isRealCustomer, paymentLabel & " Sale: " & cName & " (Inv: " & existingInvNo & ")", paymentLabel & " Sale: " & existingInvNo)
                                        End If
                                    End If

                                    If transAmt > 0 Then
                                        Module1.RegisterCashTransaction(transAmt, transType, transMsg, existingInvNo)
                                    End If
                                End If

                                CalculateTotalCredit()
                                ClearForm()
                                LoadInvoiceNumber()


                            Catch ex As Exception
                                If dbTrans IsNot Nothing Then dbTrans.Rollback()
                                selectedCustomerId = originalSelectedCustomerId
                                txtSalesRep.Text = originalSalesRepText
                                Dim errDetail As String = "Error Saving Bill: " & ex.Message & vbCrLf & vbCrLf & ex.ToString()
                                Try
                                    Dim logPath As String = System.IO.Path.Combine(Application.StartupPath, "error_log.txt")
                                    System.IO.File.WriteAllText(logPath, errDetail)
                                Catch : End Try
                                MessageBox.Show(errDetail)
                            Finally
                                If conn.State = ConnectionState.Open Then conn.Close()
                            End Try
                        End If
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub InsertIntoAllItem(itemId As String, desc As String, qty As Decimal, buyingCost As Decimal, sellingPrice As Decimal, discount As Decimal, invType As String, billType As String, status As String, isVat As Boolean)
        ' Use a DEDICATED connection here  the shared `conn` is already in use by the outer save loop.
        Using localConn As New MySqlConnection(ConnStr)
            Try
                localConn.Open()

                ' 1. Fetch Brand and Category names from items table
                Dim brandName As String = ""
                Dim catName As String = ""

                Dim fetchSql As String = "SELECT b.name as brand, c.name as category FROM items i " &
                                       "LEFT JOIN brand b ON i.brand_id = b.id " &
                                       "LEFT JOIN category c ON i.category_id = c.id " &
                                       "WHERE i.id = @id"
                Dim cmdFetch As New MySqlCommand(fetchSql, localConn)
                cmdFetch.Parameters.AddWithValue("@id", itemId)

                Using dr As MySqlDataReader = cmdFetch.ExecuteReader()
                    If dr.Read() Then
                        If Not IsDBNull(dr("brand")) Then brandName = dr("brand").ToString()
                        If Not IsDBNull(dr("category")) Then catName = dr("category").ToString()
                    End If
                End Using

                ' 2. Determine Pricing columns based on Bill Type (invType)
                Dim normalPrice As Decimal = 0
                Dim wholesalePrice As Decimal = 0
                Dim retailPrice As Decimal = 0

                If invType = "Wholesale" Then
                    wholesalePrice = sellingPrice
                ElseIf invType = "Retail" Then
                    retailPrice = sellingPrice
                Else
                    normalPrice = sellingPrice
                End If

                ' 3. Insert into all_item
                Dim sqlInsert As String = "INSERT INTO all_item (item_id, description, brand, category, item_cost, billing_price, discount, normal, wholesale, retail, quantity, inv_type, bill_type, status, is_vat, bill_time, is_rgr) " &
                                        "VALUES (@item_id, @description, @brand, @category, @item_cost, @billing_price, @discount, @normal, @wholesale, @retail, @quantity, @inv_type, @bill_type, @status, @isVat, @now_local, @is_rgr)"
                Dim cmdInsert As New MySqlCommand(sqlInsert, localConn)
                cmdInsert.Parameters.AddWithValue("@is_rgr", If(Module1.IsRgrModeActive, 1, 0))
                cmdInsert.Parameters.AddWithValue("@item_id", itemId)
                cmdInsert.Parameters.AddWithValue("@description", desc)
                cmdInsert.Parameters.AddWithValue("@brand", brandName)
                cmdInsert.Parameters.AddWithValue("@category", catName)
                cmdInsert.Parameters.AddWithValue("@item_cost", buyingCost)
                cmdInsert.Parameters.AddWithValue("@billing_price", sellingPrice)
                cmdInsert.Parameters.AddWithValue("@discount", discount)
                cmdInsert.Parameters.AddWithValue("@normal", normalPrice)
                cmdInsert.Parameters.AddWithValue("@wholesale", wholesalePrice)
                cmdInsert.Parameters.AddWithValue("@retail", retailPrice)
                cmdInsert.Parameters.AddWithValue("@quantity", qty)
                cmdInsert.Parameters.AddWithValue("@inv_type", invType)
                cmdInsert.Parameters.AddWithValue("@bill_type", billType)
                cmdInsert.Parameters.AddWithValue("@status", status)
                cmdInsert.Parameters.AddWithValue("@isVat", If(isVat, 1, 0))
                cmdInsert.Parameters.AddWithValue("@now_local", DateTime.Now)
                cmdInsert.ExecuteNonQuery()

            Catch ex As Exception
                MessageBox.Show("AllItem Sync Error: " & ex.Message)
            End Try
        End Using
    End Sub

    Private Sub ClearForm()
        dtBill.Rows.Clear()
        lblTotalAmount.Text = "0.00"
        lblGrandTotal.Text = "0.00"
        txtCashAmount.Text = ""
        txtAdvPay.Text = ""
        txtChangeAmount.Text = ""
        txtOurDiscount.Text = "0.00"
        txtInvDiscount.Text = "0.00"
        lblBalance.Text = "0.00"
        lblVatBalance.Text = "0.00"
        lblCreditBalance.Text = "0.00"
        lblAccountOutstanding.Text = "0.00"
        CheckBoxIsVat.Checked = False
        CheckBoxIsVat.Enabled = True
        ComboBoxTotalVat.SelectedValue = 1
        CheckBoxWholesale.Checked = False
        CheckBoxWholesale.Enabled = (cmbBillingType.Text <> "Quote")
        CheckBoxRetail.Checked = False
        CheckBoxRetail.Enabled = (cmbBillingType.Text <> "Quote")
        CheckBoxPrintAsRetail.Checked = False

        ' Reset billing type to unselected state
        cmbBillingType.SelectedIndex = -1
        txtSalesRep.Text = ""
        txtCustomerPhone.Text = ""
        txtCustomerAddress.Text = ""
        txtPaymentMethod.Text = ""
        ComboBox1.SelectedIndex = 0 ' Default to Sale
        TextBoxPO.Text = ""
        txtCusVatId.Text = ""
        txtCreditLimit.Text = "0.00"
        dtpCreditPeriod.Value = DateTime.Now
        selectedCustomerId = ""
        isWalletApplied = False
        lblWalletValue.Text = "0.00"

        ' Hide return/history specific fields
        lblSupplierInfo.Visible = False
        lblSupplierInfo.Text = "Supplier:"
        cmbReturnReason.Visible = False
        Label5.Visible = False
        If cmbReturnReason.Items.Count > 0 Then
            isRestoringReason = True
            cmbReturnReason.SelectedIndex = 0
            isRestoringReason = False
        End If
        btnCancelView.Visible = False
        btnCompleteInv.Text = "Inv Complete"
        btnCompleteInv.Visible = False

        ' Reset editing state and refresh new invoice number
        isEditingHistory = False
        loadedHistoryInvNo = ""
        InvDetailsPanel.Visible = False
        If btnSaveRGR IsNot Nothing Then
            btnSaveRGR.Visible = False
        End If

        ' Reset to EL bill by default on fresh form states
        isElBill = True
        btnEl.Text = "EL"
        UpdateSessionTypeInDB()
        ApplyRoleBasedUI()

        ' Ensure buttons are enabled for new sales
        btnUpdate.Enabled = True
        btnSave.Enabled = True
        btnDelete.Enabled = True
        btnAddNew.Enabled = True

        ' Clear stored state for CURRENT slot since it's now completed
        If AllSlots.ContainsKey(SlotID) Then AllSlots.Remove(SlotID)

        ' Clear the DB persistence for this slot (Table-Based)
        Using localConn As New MySqlConnection(ConnStr)
            Try
                localConn.Open()
                Dim delSql = "DELETE FROM temp_bill_items WHERE slot_id = @slot"
                Using cmdDel As New MySqlCommand(delSql, localConn)
                    cmdDel.Parameters.AddWithValue("@slot", SlotID)
                    cmdDel.ExecuteNonQuery()
                End Using
            Catch ex As Exception
                ' Silent fail
            End Try
        End Using

        ClearEntryFields()
        CalculateGrandTotal()

        ' Clear Cashier ID ONLY after whole bill is saved/cleared
        txtCashierID.Text = ""

        ' After bill is done, go back to Item ID for next entry
        txtItemID.Focus()
        UpdateNavigationButtons()

        ' Re-apply role restrictions to ensure Order Taker defaults (PENDING) are restored
        ApplyRoleBasedUI()

        ' Reset Full Credit display
        CalculateTotalCredit()
    End Sub

    Private Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
        Dim currentCashier As String = cmbCashier.Text.Trim().ToLower()
        If currentCashier = "admin1" OrElse currentCashier = "admin2" OrElse currentCashier = "admin3" OrElse currentCashier = "admin4" Then
            MessageBox.Show("Please enter a valid Cashier ID to switch to the next slot.", "Invalid Cashier", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtCashierID.Focus()
            txtCashierID.SelectAll()
            Return
        End If

        ' Find the next accessible slot: my slots (any #) or free slots, with slot_id > current
        Dim nextSlot As Integer = 0
        Try
            Using localConn As New MySqlConnection(ConnStr)
                localConn.Open()
                Dim nextSql = "SELECT slot_id FROM window_sessions WHERE slot_id > @cur AND (is_used = 0 OR user_name = @uname) ORDER BY slot_id LIMIT 1"
                Using cmd As New MySqlCommand(nextSql, localConn)
                    cmd.Parameters.AddWithValue("@cur", SlotID)
                    cmd.Parameters.AddWithValue("@uname", Module1.UserName)
                    Dim res = cmd.ExecuteScalar()
                    If res IsNot Nothing Then nextSlot = Convert.ToInt32(res)
                End Using
            End Using
        Catch ex As Exception
        End Try

        If nextSlot > 0 Then
            SwitchToSlot(nextSlot)
        Else
            MessageBox.Show("No more accessible slots forward.", "End of Slots", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub ButtonBefore_Click(sender As Object, e As EventArgs) Handles ButtonBefore.Click
        ' Find the previous accessible slot: my slots (any #) or free slots, with slot_id < current
        Dim prevSlot As Integer = 0
        Try
            Using localConn As New MySqlConnection(ConnStr)
                localConn.Open()
                Dim prevSql = "SELECT slot_id FROM window_sessions WHERE slot_id < @cur AND (is_used = 0 OR user_name = @uname) ORDER BY slot_id DESC LIMIT 1"
                Using cmd As New MySqlCommand(prevSql, localConn)
                    cmd.Parameters.AddWithValue("@cur", SlotID)
                    cmd.Parameters.AddWithValue("@uname", Module1.UserName)
                    Dim res = cmd.ExecuteScalar()
                    If res IsNot Nothing Then prevSlot = Convert.ToInt32(res)
                End Using
            End Using
        Catch ex As Exception
        End Try

        If prevSlot > 0 Then
            SwitchToSlot(prevSlot)
        Else
            MessageBox.Show("No more accessible slots backward.", "Start of Slots", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    ' ── Slot Navigation Helpers ──────────────────────────────────────

    Private Sub SwitchToSlot(targetSlot As Integer)
        ' Save current state first
        Dim oldSlotID As Integer = SlotID
        SaveSlotState(oldSlotID)

        ' Release active screen lock on old slot
        If oldSlotID > 0 Then ResetMySessions(oldSlotID)

        ' Try to reserve the target slot (will fail if currently active on another machine)
        If ReserveSlot(targetSlot, silent:=False) Then
            LoadSlotState(SlotID)
            LoadComboBoxSlots()
        Else
            ' Re-reserve old slot
            If oldSlotID > 0 Then
                ReserveSlot(oldSlotID, silent:=True)
            End If
            LoadComboBoxSlots()
        End If
    End Sub

    ''' <summary>Populate the cmbSlot ComboBox with slots accessible to the current user.
    ''' Shows: active slots, draft slots (with owner), and free slots.
    ''' </summary>
    Private Sub LoadComboBoxSlots()
        If cmbSlot Is Nothing Then Return
        isUpdatingSlotCombo = True
        Try
            Dim savedText As String = cmbSlot.Text
            cmbSlot.Items.Clear()
            Using localConn As New MySqlConnection(ConnStr)
                localConn.Open()
                ' Fetch slots and check draft status from temp_bill_items
                Dim slotSql = "SELECT w.slot_id, w.is_used, w.user_name, w.current_type, " &
                              "(SELECT COUNT(*) FROM temp_bill_items t WHERE t.slot_id = w.slot_id) AS item_count, " &
                              "(SELECT draft_user FROM temp_bill_items t WHERE t.slot_id = w.slot_id LIMIT 1) AS draft_user " &
                              "FROM window_sessions w " &
                              "WHERE w.is_used = 0 OR w.user_name = @uname " &
                              "ORDER BY w.slot_id"
                Using cmd As New MySqlCommand(slotSql, localConn)
                    cmd.Parameters.AddWithValue("@uname", Module1.UserName)
                    Using dr = cmd.ExecuteReader()
                        While dr.Read()
                            Dim sid = Convert.ToInt32(dr("slot_id"))
                            Dim uname = If(dr("user_name") Is DBNull.Value, "", dr("user_name").ToString())
                            Dim isUsed = Convert.ToInt32(dr("is_used")) = 1
                            Dim itemCount = Convert.ToInt32(dr("item_count"))
                            Dim draftUser = If(dr("draft_user") Is DBNull.Value, "", dr("draft_user").ToString())

                            Dim slotDesc As String
                            If isUsed Then
                                If String.Equals(uname, Module1.UserName, StringComparison.OrdinalIgnoreCase) Then
                                    slotDesc = "Slot " & sid & " — Active (Me)"
                                Else
                                    slotDesc = "Slot " & sid & " — Active (" & uname & ")"
                                End If
                            Else
                                If itemCount > 0 Then
                                    If String.IsNullOrEmpty(draftUser) Then
                                        slotDesc = "Slot " & sid & " — Draft"
                                    Else
                                        slotDesc = "Slot " & sid & " — Draft (" & draftUser & ")"
                                    End If
                                Else
                                    slotDesc = "Slot " & sid & " — Free"
                                End If
                            End If
                            cmbSlot.Items.Add(slotDesc)
                        End While
                    End Using
                End Using
            End Using

            ' Re-select the current slot in the combobox
            Dim currentLabel = "Slot " & SlotID
            For i As Integer = 0 To cmbSlot.Items.Count - 1
                If cmbSlot.Items(i).ToString().StartsWith(currentLabel) Then
                    cmbSlot.SelectedIndex = i
                    Exit For
                End If
            Next
        Catch ex As Exception
            ' Silent fail
        End Try
        isUpdatingSlotCombo = False
    End Sub

    ' Handle slot combobox selection with Enter key
    Private Sub cmbSlot_KeyDown(sender As Object, e As KeyEventArgs) Handles cmbSlot.KeyDown
        If e.KeyCode = Keys.Enter Then
            Dim selectedLabel As String = cmbSlot.Text
            If Not String.IsNullOrWhiteSpace(selectedLabel) AndAlso selectedLabel.StartsWith("Slot ") Then
                Dim parts = selectedLabel.Split(" "c)
                Dim targetId As Integer = 0
                If parts.Length >= 2 AndAlso Integer.TryParse(parts(1), targetId) Then
                    If targetId <> SlotID Then
                        SwitchToSlot(targetId)
                    End If
                End If
            End If
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub cmbSlot_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbSlot.SelectedIndexChanged
        If isUpdatingSlotCombo Then Return
        If cmbSlot.SelectedIndex = -1 Then Return

        Dim selectedLabel As String = cmbSlot.SelectedItem.ToString()
        If Not String.IsNullOrWhiteSpace(selectedLabel) AndAlso selectedLabel.StartsWith("Slot ") Then
            Dim parts = selectedLabel.Split(" "c)
            Dim targetId As Integer = 0
            If parts.Length >= 2 AndAlso Integer.TryParse(parts(1), targetId) Then
                If targetId <> SlotID Then
                    SwitchToSlot(targetId)
                End If
            End If
        End If
    End Sub

    ' --- Wholesale / Retail Mutual Exclusion & Dynamic Pricing ---
    Private isSyncingToggles As Boolean = False
    Private Sub CheckBoxWholesale_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxWholesale.CheckedChanged
        If isSyncingToggles Then Return
        isSyncingToggles = True
        If CheckBoxWholesale.Checked Then
            CheckBoxRetail.Checked = False
        Else
            CheckBoxPrintAsRetail.Checked = False
        End If
        HandlePricingModeChange()
        If Not String.IsNullOrWhiteSpace(txtItemID.Text) Then
            FetchItemByID(txtItemID.Text.Trim())
        End If
        isSyncingToggles = False
    End Sub

    Private Sub CheckBoxRetail_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxRetail.CheckedChanged
        If isSyncingToggles Then Return
        isSyncingToggles = True
        If CheckBoxRetail.Checked Then CheckBoxWholesale.Checked = False
        HandlePricingModeChange()
        If Not String.IsNullOrWhiteSpace(txtItemID.Text) Then
            FetchItemByID(txtItemID.Text.Trim())
        End If
        isSyncingToggles = False
    End Sub

    Private Sub HandlePricingModeChange()
        If dtBill.Rows.Count > 0 Then
            Dim modeName As String = "Normal"
            If CheckBoxWholesale.Checked Then modeName = "Wholesale"
            If CheckBoxRetail.Checked Then modeName = "Retail"

            Dim msg As String = "You have items in the bill. Do you want to update all existing items to " & modeName & " pricing?"
            If MessageBox.Show(msg, "Confirm Price Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                SyncGridPricesWithMode()
            End If
        End If
    End Sub

    Private Sub SyncGridPricesWithMode()
        If dtBill.Rows.Count = 0 Then Return

        Dim openedHere As Boolean = False
        Try
            If conn.State = ConnectionState.Closed Then
                conn.Open()
                openedHere = True
            End If

            For Each row As DataRow In dtBill.Rows
                Dim itemId As String = row("Item ID").ToString()
                Dim qty As Decimal = 0
                If row("Qty") IsNot DBNull.Value Then Decimal.TryParse(row("Qty").ToString(), qty)
                Dim discPercent As Decimal = 0
                If row("Dis") IsNot DBNull.Value Then Decimal.TryParse(row("Dis").ToString(), discPercent)

                ' Extract VAT info from the grid label (e.g., "VAT (15.00%)" -> 15)
                Dim vatLabel As String = If(row("VAT") Is DBNull.Value, "", row("VAT").ToString())
                Dim vatPercent As Decimal = 0
                If vatLabel.Contains("(") AndAlso vatLabel.Contains("%") Then
                    Try
                        Dim startIdx = vatLabel.IndexOf("(") + 1
                        Dim endIdx = vatLabel.IndexOf("%")
                        Dim valStr = vatLabel.Substring(startIdx, endIdx - startIdx)
                        Decimal.TryParse(valStr, vatPercent)
                    Catch : End Try
                End If

                ' Fetch prices from items table
                Dim sql As String = "SELECT selling_price, whole_selling_price, retail_selling_price FROM items WHERE id = @id"
                Using cmd As New MySqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@id", itemId)
                    Using dr As MySqlDataReader = cmd.ExecuteReader()
                        If dr.Read() Then
                            Dim newPrice As Decimal = 0
                            If Not IsDBNull(dr("selling_price")) Then newPrice = Convert.ToDecimal(dr("selling_price"))

                            If CheckBoxWholesale.Checked AndAlso Not IsDBNull(dr("whole_selling_price")) Then
                                newPrice = Convert.ToDecimal(dr("whole_selling_price"))
                            ElseIf CheckBoxRetail.Checked AndAlso Not IsDBNull(dr("retail_selling_price")) Then
                                newPrice = Convert.ToDecimal(dr("retail_selling_price"))
                            End If

                            ' Calculate Line Total based on VAT state
                            Dim lineTotal As Decimal = 0
                            If CheckBoxIsVat.Checked AndAlso vatPercent > 0 Then
                                ' VAT Bill: amount = newPrice / (1 + vat/100) * (1 - dis/100) * qty
                                Dim netUnitPrice = newPrice / (1 + vatPercent / 100)
                                lineTotal = (netUnitPrice * (1 - discPercent / 100)) * qty
                            Else
                                ' Normal Bill: amount = newPrice * (1-dis/100) * qty
                                lineTotal = (newPrice * (1 - discPercent / 100)) * qty
                            End If

                            ' Update the row
                            If CheckBoxWholesale.Checked AndAlso CheckBoxPrintAsRetail.Checked Then
                                Dim currentPrintRetail As Decimal = 0
                                If row("PrintRetailPrice") IsNot DBNull.Value Then Decimal.TryParse(row("PrintRetailPrice").ToString(), currentPrintRetail)
                                
                                If currentPrintRetail <= 0 Then
                                    Dim currentPrice As Decimal = 0
                                    If row("Selling Price") IsNot DBNull.Value Then Decimal.TryParse(row("Selling Price").ToString(), currentPrice)
                                    
                                    If currentPrice <> newPrice AndAlso currentPrice > 0 Then
                                        row("PrintRetailPrice") = currentPrice
                                    Else
                                        Dim rpVal As Decimal = 0
                                        If Not IsDBNull(dr("retail_selling_price")) Then
                                            rpVal = Convert.ToDecimal(dr("retail_selling_price"))
                                        Else
                                            rpVal = Convert.ToDecimal(dr("selling_price"))
                                        End If
                                        row("PrintRetailPrice") = rpVal
                                    End If
                                End If
                            Else
                                row("PrintRetailPrice") = 0
                            End If

                            row("Selling Price") = newPrice
                            row("Total/Amount") = lineTotal
                        End If
                    End Using
                End Using
            Next
            If openedHere Then conn.Close()
            CalculateGrandTotal()
        Catch ex As Exception
            If openedHere AndAlso conn.State = ConnectionState.Open Then conn.Close()
            MessageBox.Show("Error updating prices: " & ex.Message)
        End Try
    End Sub

    Private Sub chkShowCustomer_CheckedChanged(sender As Object, e As EventArgs) Handles chkShowCustomer.CheckedChanged
        If Not chkShowCustomer.Checked AndAlso dgvSearch.Visible AndAlso currentSearchMode = "CUSTOMER" Then
            dgvSearch.Visible = False
        ElseIf chkShowCustomer.Checked AndAlso currentSearchMode = "CUSTOMER" AndAlso Not String.IsNullOrWhiteSpace(txtSalesRep.Text) Then
            SearchCustomers()
        End If
    End Sub

    Private Sub txtSalesRep_TextChanged(sender As Object, e As EventArgs) Handles txtSalesRep.TextChanged
        If String.Equals(txtSalesRep.Text.Trim(), "", StringComparison.OrdinalIgnoreCase) Then
            selectedCustomerId = ""
            txtCreditLimit.Text = "0.00"
            dtpCreditPeriod.Value = DateTime.Now
            lblAccountOutstanding.Text = "0.00"
        End If

        If txtSalesRep.Focused Then
            ' Clear any previously selected customer ID so that manual typing 
            ' triggers a fresh search or creation logic on Save.
            selectedCustomerId = ""
            currentSearchMode = "CUSTOMER"
            SearchCustomers()
        End If
    End Sub


    Private Sub txtSalesRep_KeyDown(sender As Object, e As KeyEventArgs) Handles txtSalesRep.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                txtCashierID.Focus()
                e.SuppressKeyPress = True
            Else
                ' User requested: if customer added (name exists) enter -> item id
                ' If no customer, enter -> phone -> address -> item id
                If Not String.IsNullOrWhiteSpace(txtSalesRep.Text) AndAlso Not String.IsNullOrEmpty(selectedCustomerId) Then
                    txtItemID.Focus()
                Else
                    txtCustomerPhone.Focus()
                End If
                e.SuppressKeyPress = True
            End If
        ElseIf e.KeyCode = Keys.Down Then
            ' Arrow Down → go into the grid to pick a customer with arrow keys
            If dgvSearch.Visible AndAlso currentSearchMode = "CUSTOMER" AndAlso dgvSearch.Rows.Count > 0 Then
                dgvSearch.Focus()
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub txtCustomerPhone_TextChanged(sender As Object, e As EventArgs) Handles txtCustomerPhone.TextChanged
        If txtCustomerPhone.Focused Then
            ' Clear any previously selected customer ID so that the Save logic 
            ' knows to either find or create a new 'Quick Customer' based on the new phone number.
            selectedCustomerId = ""
            currentSearchMode = "CUSTOMER"
            SearchCustomers()
        End If
    End Sub


    Private Sub txtCustomerPhone_KeyDown(sender As Object, e As KeyEventArgs) Handles txtCustomerPhone.KeyDown
        If e.KeyCode = Keys.Enter Then
            If dgvSearch.Visible AndAlso currentSearchMode = "CUSTOMER" AndAlso dgvSearch.Rows.Count > 0 Then
                SelectCustomerFromGrid(0)
                e.Handled = True
            Else
                Dim phone As String = txtCustomerPhone.Text.Trim()
                If phone <> "" Then
                    FetchCustomerData("tel_no", phone)
                End If
                ' User requested: Phone -> Address
                txtCustomerAddress.Focus()
            End If
        ElseIf e.KeyCode = Keys.Down Then
            If dgvSearch.Visible AndAlso currentSearchMode = "CUSTOMER" AndAlso dgvSearch.Rows.Count > 0 Then
                dgvSearch.Focus()
                e.Handled = True
            End If
        End If
    End Sub


    Private Sub SearchCustomers()
        Dim openedHere As Boolean = False
        Try
            Dim nameKey As String = ""
            Dim phoneKey As String = ""

            ' Use only the focused field for searching to prevent Name and Phone filters from conflicting
            If txtSalesRep.Focused Then
                nameKey = txtSalesRep.Text.Trim()
            ElseIf txtCustomerPhone.Focused Then
                phoneKey = txtCustomerPhone.Text.Trim()
            Else
                ' If called programmatically without focus, check both fields
                nameKey = txtSalesRep.Text.Trim()
                phoneKey = txtCustomerPhone.Text.Trim()
            End If

            ' If both search keys are empty, hide the search grid and exit
            If String.IsNullOrEmpty(nameKey) AndAlso String.IsNullOrEmpty(phoneKey) Then
                dgvSearch.Visible = False
                Return
            End If

            If conn.State = ConnectionState.Closed Then
                conn.Open()
                openedHere = True
            End If

            ' Base query
            Dim localSql As String = "SELECT id as 'ID', name as 'Customer Name', tel_no as 'Phone', address as 'Address', customer_type, is_block, credit_limit, credit_period FROM customer WHERE 1=1"

            If nameKey.Length > 0 Then
                localSql &= " AND name LIKE @name"
            End If

            If phoneKey.Length > 0 Then
                If phoneKey.StartsWith("0") Then
                    localSql &= " AND (tel_no LIKE @phone_start OR tel_no LIKE @phone_slash OR tel_no LIKE @phone_comma OR tel_no LIKE @phone_space OR tel_no LIKE @phone_hyphen)"
                Else
                    localSql &= " AND (tel_no LIKE @phoneA_start OR tel_no LIKE @phoneA_slash OR tel_no LIKE @phoneA_comma OR tel_no LIKE @phoneA_space OR tel_no LIKE @phoneA_hyphen OR " &
                                "tel_no LIKE @phoneB_start OR tel_no LIKE @phoneB_slash OR tel_no LIKE @phoneB_comma OR tel_no LIKE @phoneB_space OR tel_no LIKE @phoneB_hyphen)"
                End If
            End If

            ' No LIMIT - show all matching customers

            Dim dt As New DataTable()
            Using localCmd As New MySqlCommand(localSql, conn)
                If nameKey.Length > 0 Then
                    localCmd.Parameters.AddWithValue("@name", nameKey & "%")
                End If

                If phoneKey.Length > 0 Then
                    If phoneKey.StartsWith("0") Then
                        localCmd.Parameters.AddWithValue("@phone_start", phoneKey & "%")
                        localCmd.Parameters.AddWithValue("@phone_slash", "%/" & phoneKey & "%")
                        localCmd.Parameters.AddWithValue("@phone_comma", "%," & phoneKey & "%")
                        localCmd.Parameters.AddWithValue("@phone_space", "% " & phoneKey & "%")
                        localCmd.Parameters.AddWithValue("@phone_hyphen", "%-" & phoneKey & "%")
                    Else
                        Dim termA As String = phoneKey
                        Dim termB As String = "0" & phoneKey

                        localCmd.Parameters.AddWithValue("@phoneA_start", termA & "%")
                        localCmd.Parameters.AddWithValue("@phoneA_slash", "%/" & termA & "%")
                        localCmd.Parameters.AddWithValue("@phoneA_comma", "%," & termA & "%")
                        localCmd.Parameters.AddWithValue("@phoneA_space", "% " & termA & "%")
                        localCmd.Parameters.AddWithValue("@phoneA_hyphen", "%-" & termA & "%")

                        localCmd.Parameters.AddWithValue("@phoneB_start", termB & "%")
                        localCmd.Parameters.AddWithValue("@phoneB_slash", "%/" & termB & "%")
                        localCmd.Parameters.AddWithValue("@phoneB_comma", "%," & termB & "%")
                        localCmd.Parameters.AddWithValue("@phoneB_space", "% " & termB & "%")
                        localCmd.Parameters.AddWithValue("@phoneB_hyphen", "%-" & termB & "%")
                    End If
                End If

                Dim da As New MySqlDataAdapter(localCmd)
                da.Fill(dt)
            End Using

            If dt.Rows.Count > 0 Then
                dgvSearch.DataSource = dt
                ' Hide technical columns from user view
                If dgvSearch.Columns("ID") IsNot Nothing Then
                    dgvSearch.Columns("ID").Visible = False
                End If
                If dgvSearch.Columns("customer_type") IsNot Nothing Then
                    dgvSearch.Columns("customer_type").Visible = False
                End If
                If dgvSearch.Columns("is_block") IsNot Nothing Then
                    dgvSearch.Columns("is_block").Visible = False
                End If
                If dgvSearch.Columns("credit_limit") IsNot Nothing Then
                    dgvSearch.Columns("credit_limit").Visible = False
                End If
                If dgvSearch.Columns("credit_period") IsNot Nothing Then
                    dgvSearch.Columns("credit_period").Visible = False
                End If
                If chkShowCustomer.Checked Then
                    dgvSearch.Visible = True
                    dgvSearch.BringToFront()
                Else
                    dgvSearch.Visible = False
                End If
                dgvSearch.RowHeadersVisible = False

                ' Fixed Position: Always align with txtSalesRep for consistency
                Dim screenPosPrimary As Point = txtSalesRep.PointToScreen(New Point(0, txtSalesRep.Height))
                Dim formPosPrimary As Point = Me.PointToClient(screenPosPrimary)
                dgvSearch.Location = New Point(formPosPrimary.X, formPosPrimary.Y + 5)

                ' Adjusted width to be more compact
                dgvSearch.Width = 1100
                dgvSearch.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None

                ' Adjust widths to specifically match the visual borders
                If dgvSearch.Columns("Customer Name") IsNot Nothing Then
                    dgvSearch.Columns("Customer Name").Width = txtSalesRep.Width + (txtCustomerPhone.Left - txtSalesRep.Right) - 20
                End If
                If dgvSearch.Columns("Phone") IsNot Nothing Then
                    dgvSearch.Columns("Phone").Width = txtCustomerPhone.Width + (txtCustomerAddress.Left - txtCustomerPhone.Right) - 20
                End If
                If dgvSearch.Columns("Address") IsNot Nothing Then
                    dgvSearch.Columns("Address").Width = 600
                End If
            Else
                dgvSearch.Visible = False
            End If
            If openedHere Then conn.Close()
        Catch ex As Exception
            If openedHere AndAlso conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    ' --- Customer Selection Logic ---
    Private Sub SelectCustomerFromGrid(rowIndex As Integer)
        ' Guard: only proceed if we are actually in customer search mode
        If currentSearchMode <> "CUSTOMER" Then Return
        If rowIndex < 0 OrElse rowIndex >= dgvSearch.Rows.Count Then Return

        Try
            Dim row As DataGridViewRow = dgvSearch.Rows(rowIndex)

            ' Check if Customer is Blocked
            Dim is_block As Integer = If(IsDBNull(row.Cells("is_block").Value), 0, Convert.ToInt32(row.Cells("is_block").Value))
            If is_block = 1 Then
                MessageBox.Show("This customer is blocked and cannot be selected for new bills.", "Customer Blocked", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            selectedCustomerId = row.Cells("ID").Value.ToString()
            txtSalesRep.Text = row.Cells("Customer Name").Value.ToString()
            txtCustomerPhone.Text = row.Cells("Phone").Value.ToString()
            txtCustomerAddress.Text = If(row.Cells("Address").Value Is DBNull.Value, "", row.Cells("Address").Value.ToString())
            txtCreditLimit.Text = If(row.Cells("credit_limit").Value Is DBNull.Value, "0.00", Convert.ToDecimal(row.Cells("credit_limit").Value).ToString("N2"))
            Dim cpVal_g As String = If(row.Cells("credit_period").Value Is DBNull.Value, "", row.Cells("credit_period").Value.ToString())
            If Not String.IsNullOrEmpty(cpVal_g) AndAlso DateTime.TryParse(cpVal_g, Nothing) Then
                dtpCreditPeriod.Value = Convert.ToDateTime(cpVal_g)
            Else
                dtpCreditPeriod.Value = DateTime.Now
            End If

            ' Auto-update Billing Type (Only if no items in bill)
            If dtBill.Rows.Count = 0 Then
                Dim custType As String = If(row.Cells("customer_type").Value, "").ToString()
                CheckBoxWholesale.Checked = String.Equals(custType, "Wholesale", StringComparison.OrdinalIgnoreCase)
                CheckBoxRetail.Checked = String.Equals(custType, "Retail", StringComparison.OrdinalIgnoreCase)
            End If

            dgvSearch.Visible = False
            CalculateTotalCredit()
            txtItemID.Focus()
        Catch ex As Exception
            ' Grid columns don't match customer schema — ignore silently
            dgvSearch.Visible = False
        End Try
    End Sub




    Private Sub FetchCustomerData(field As String, value As String)
        Dim openedHere As Boolean = False
        Try
            If conn.State = ConnectionState.Closed Then
                conn.Open()
                openedHere = True
            End If

            Dim sql As String
            Dim cmd As New MySqlCommand()
            cmd.Connection = conn

            If String.Equals(field, "tel_no", StringComparison.OrdinalIgnoreCase) Then
                Dim phoneKey As String = value.Trim()
                If phoneKey.StartsWith("0") Then
                    sql = "SELECT id, name, address, tel_no, customer_type, is_block, credit_limit, credit_period FROM customer WHERE " &
                          "(tel_no = @val_start OR tel_no LIKE @val_slash OR tel_no LIKE @val_comma OR tel_no LIKE @val_space OR tel_no LIKE @val_hyphen)"
                    cmd.Parameters.AddWithValue("@val_start", phoneKey)
                    cmd.Parameters.AddWithValue("@val_slash", "%/" & phoneKey & "%")
                    cmd.Parameters.AddWithValue("@val_comma", "%," & phoneKey & "%")
                    cmd.Parameters.AddWithValue("@val_space", "% " & phoneKey & "%")
                    cmd.Parameters.AddWithValue("@val_hyphen", "%-" & phoneKey & "%")
                Else
                    Dim termA As String = phoneKey
                    Dim termB As String = "0" & phoneKey
                    sql = "SELECT id, name, address, tel_no, customer_type, is_block, credit_limit, credit_period FROM customer WHERE " &
                          "(tel_no = @valA_start OR tel_no LIKE @valA_slash OR tel_no LIKE @valA_comma OR tel_no LIKE @valA_space OR tel_no LIKE @valA_hyphen OR " &
                          "tel_no = @valB_start OR tel_no LIKE @valB_slash OR tel_no LIKE @valB_comma OR tel_no LIKE @valB_space OR tel_no LIKE @valB_hyphen)"
                    cmd.Parameters.AddWithValue("@valA_start", termA)
                    cmd.Parameters.AddWithValue("@valA_slash", "%/" & termA & "%")
                    cmd.Parameters.AddWithValue("@valA_comma", "%," & termA & "%")
                    cmd.Parameters.AddWithValue("@valA_space", "% " & termA & "%")
                    cmd.Parameters.AddWithValue("@valA_hyphen", "%-" & termA & "%")

                    cmd.Parameters.AddWithValue("@valB_start", termB)
                    cmd.Parameters.AddWithValue("@valB_slash", "%/" & termB & "%")
                    cmd.Parameters.AddWithValue("@valB_comma", "%," & termB & "%")
                    cmd.Parameters.AddWithValue("@valB_space", "% " & termB & "%")
                    cmd.Parameters.AddWithValue("@valB_hyphen", "%-" & termB & "%")
                End If
            Else
                sql = "SELECT id, name, address, tel_no, customer_type, is_block, credit_limit, credit_period FROM customer WHERE " & field & " = @val"
                cmd.Parameters.AddWithValue("@val", value)
            End If

            cmd.CommandText = sql
            Dim dr As MySqlDataReader = cmd.ExecuteReader()
            If dr.Read() Then
                ' Check if Customer is Blocked
                Dim is_block As Integer = If(IsDBNull(dr("is_block")), 0, Convert.ToInt32(dr("is_block")))
                If is_block = 1 Then
                    MessageBox.Show("This customer is blocked and cannot be selected for new bills.", "Customer Blocked", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    dr.Close()
                    If openedHere Then conn.Close()
                    Return
                End If

                selectedCustomerId = dr("id").ToString() ' Store ID in background
                txtSalesRep.Text = dr("name").ToString() ' Name field
                txtCustomerAddress.Text = dr("address").ToString()
                txtCustomerPhone.Text = dr("tel_no").ToString()
                txtCreditLimit.Text = If(IsDBNull(dr("credit_limit")), "0.00", Convert.ToDecimal(dr("credit_limit")).ToString("N2"))
                Dim cpVal_f As String = If(IsDBNull(dr("credit_period")), "", dr("credit_period").ToString())
                If Not String.IsNullOrEmpty(cpVal_f) AndAlso DateTime.TryParse(cpVal_f, Nothing) Then
                    dtpCreditPeriod.Value = Convert.ToDateTime(cpVal_f)
                Else
                    dtpCreditPeriod.Value = DateTime.Now
                End If

                ' Auto-update Billing Type (Only if no items in bill)
                If dtBill.Rows.Count = 0 Then
                    Dim custType As String = If(dr("customer_type"), "").ToString()
                    CheckBoxWholesale.Checked = String.Equals(custType, "Wholesale", StringComparison.OrdinalIgnoreCase)
                    CheckBoxRetail.Checked = String.Equals(custType, "Retail", StringComparison.OrdinalIgnoreCase)
                End If
            Else
                ' Customer not found - Clear old ID so Quick Customer logic can trigger on Save
                selectedCustomerId = ""
                MessageBox.Show("Customer Not Found")
            End If
            dr.Close()
            If openedHere Then conn.Close()
            CalculateTotalCredit() ' Show outstanding balance immediately
        Catch ex As Exception
            MessageBox.Show("Error fetching customer: " & ex.Message)
            If openedHere AndAlso conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    ' --- Unified Search Logic ---
    Private Sub txtItemID_TextChanged(sender As Object, e As EventArgs) Handles txtItemID.TextChanged
        If txtItemID.Focused Then
            currentSearchMode = "ITEM"
            SearchItems()
        End If
    End Sub


    Private Sub txtDescription_TextChanged(sender As Object, e As EventArgs) Handles txtDescription.TextChanged
        If txtDescription.Focused Then
            currentSearchMode = "ITEM"
            SearchItems()
        End If
    End Sub




    Public Sub UpdateCostColumnVisibility()
        Try
            Dim showCost As Boolean = False
            Dim startForm = Application.OpenForms.OfType(Of Start)().FirstOrDefault()
            If startForm IsNot Nothing AndAlso startForm.txtRgrPass IsNot Nothing AndAlso startForm.txtRgrPass.Text = "2233" Then
                showCost = True
            End If

            If dgvSearch.Columns("Cost") IsNot Nothing Then
                dgvSearch.Columns("Cost").Visible = showCost
                If showCost Then
                    dgvSearch.Columns("Cost").Width = 90
                    dgvSearch.Columns("Cost").DefaultCellStyle.Format = "N2"
                End If
            End If
        Catch
        End Try
    End Sub

    Private Sub SearchItems()
        Dim codeKey As String = txtItemID.Text.Trim()
        Dim descKey As String = txtDescription.Text.Trim()

        If String.IsNullOrWhiteSpace(codeKey) AndAlso String.IsNullOrWhiteSpace(descKey) Then
            dgvSearch.Visible = False
            Return
        End If

        Dim openedHere As Boolean = False
        Try
            If conn.State = ConnectionState.Closed Then
                conn.Open()
                openedHere = True
            End If

            Dim conditions As New List(Of String)()
            Dim sqlParams As New List(Of MySqlParameter)()

            ' 1. Filter by Item Code prefix if provided
            If Not String.IsNullOrWhiteSpace(codeKey) Then
                conditions.Add("(LOWER(i.id) LIKE @codeKey OR LOWER(i.barcode) LIKE @codeKey OR LOWER(REPLACE(i.id, '-', '')) LIKE @codeKeyWithoutHyphen OR LOWER(REPLACE(i.barcode, '-', '')) LIKE @codeKeyWithoutHyphen)")
                sqlParams.Add(New MySqlParameter("@codeKey", codeKey.ToLower() & "%"))
                sqlParams.Add(New MySqlParameter("@codeKeyWithoutHyphen", codeKey.Replace("-", "").ToLower() & "%"))
            End If

            ' 2. Filter by Description keywords anywhere in any order
            If Not String.IsNullOrWhiteSpace(descKey) Then
                Dim words = descKey.Split(New Char() {" "c}, StringSplitOptions.RemoveEmptyEntries)
                For i As Integer = 0 To words.Length - 1
                    Dim wordParamName = "@descWord" & i
                    conditions.Add("i.description LIKE " & wordParamName)
                    sqlParams.Add(New MySqlParameter(wordParamName, "%" & words(i) & "%"))
                Next
            End If

            Dim whereClause As String = String.Join(" AND ", conditions)

            Dim localSql = "SELECT i.id as 'Item ID', i.item_name as 'Item Name', i.description as 'Description', " &
                "i.selling_price as 'Price', i.item_cost as 'Cost', i.avg_cost as 'AvgCost', i.discount as 'Dis', " &
                "IFNULL((SELECT SUM(st_qty) FROM items_stock WHERE item_id = i.id AND location_id = (SELECT id FROM location WHERE location_name = @loc)), 0) as 'Stock' " &
                "FROM items i " &
                "WHERE " & whereClause & " ORDER BY i.id ASC"

            Using localCmd As New MySqlCommand(localSql, conn)
                For Each param In sqlParams
                    localCmd.Parameters.Add(param)
                Next
                Dim searchLoc As String = If(String.IsNullOrWhiteSpace(ComboBoxLocation.Text), "MAIN STOCK", ComboBoxLocation.Text)
                localCmd.Parameters.AddWithValue("@loc", searchLoc)

                Dim da As New MySqlDataAdapter(localCmd)
                Dim dt As New DataTable()
                da.Fill(dt)

                If dt.Rows.Count > 0 Then
                    dgvSearch.DataSource = dt

                    ' Show only Item ID, Description, Item Name columns
                    If dgvSearch.Columns("Item Name") IsNot Nothing Then dgvSearch.Columns("Item Name").Visible = False
                    UpdateCostColumnVisibility()
                    If dgvSearch.Columns("AvgCost") IsNot Nothing Then dgvSearch.Columns("AvgCost").Visible = False
                    ' Ensure Stock is visible
                    If dgvSearch.Columns("Stock") IsNot Nothing Then dgvSearch.Columns("Stock").Visible = True

                    dgvSearch.Visible = True
                    dgvSearch.BringToFront()
                    dgvSearch.RowHeadersVisible = False

                    ' Fixed Position: Align with txtItemID for parallel item entry
                    Dim screenPosItemLine As Point = txtItemID.PointToScreen(New Point(0, txtItemID.Height))
                    Dim formPosItemLine As Point = Me.PointToClient(screenPosItemLine)
                    dgvSearch.Location = New Point(formPosItemLine.X, formPosItemLine.Y + 5)

                    ' Adjusted width to be wider to prevent description truncation
                    dgvSearch.Width = 1250
                    dgvSearch.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None

                    ' Alignment logic: Ends of cols match Start of next box
                    If dgvSearch.Columns("Item ID") IsNot Nothing Then
                        dgvSearch.Columns("Item ID").Width = txtItemID.Width + (txtDescription.Left - txtItemID.Right) - 5
                    End If

                    If dgvSearch.Columns("Description") IsNot Nothing Then
                        ' Description spans over Description box and Qty box
                        ' Distance from start of Description to start of Selling Price, plus extra width for full item descriptions
                        dgvSearch.Columns("Description").Width = txtQuantity.Right - txtDescription.Left + (txtSellingPrice1.Left - txtQuantity.Right) - 5 + 250
                    End If

                    If dgvSearch.Columns("Price") IsNot Nothing Then
                        ' Price spans Selling Price box
                        dgvSearch.Columns("Price").Width = txtSellingPrice1.Width + (txtDiscount.Left - txtSellingPrice1.Right) - 5
                    End If

                    If dgvSearch.Columns("Dis") IsNot Nothing Then
                        dgvSearch.Columns("Dis").Width = txtDiscount.Width + (txtItemDiscountVal.Left - txtDiscount.Right) - 5
                    End If

                    If dgvSearch.Columns("Stock") IsNot Nothing Then
                        dgvSearch.Columns("Stock").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                    End If
                Else
                    dgvSearch.Visible = False
                End If
            End Using
            If openedHere Then conn.Close()
        Catch ex As Exception
            If openedHere AndAlso conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    ' --- Unified Selection Logic ---
    Private Sub dgvSearch_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvSearch.CellClick
        If e.RowIndex >= 0 Then
            If currentSearchMode = "CUSTOMER" Then
                SelectCustomerFromGrid(e.RowIndex)
            Else
                SelectItemFromGrid(e.RowIndex)
            End If
        End If
    End Sub

    Private Sub SelectItemFromGrid(rowIndex As Integer)
        Dim row As DataGridViewRow = dgvSearch.Rows(rowIndex)
        txtItemID.Text = row.Cells("Item ID").Value.ToString()
        txtSellingPrice1.Text = row.Cells("Price").Value.ToString()
        txtDescription.Text = row.Cells("Description").Value.ToString()
        txtSellingPrice1.Tag = row.Cells("Cost").Value.ToString()
        txtDiscount.Text = row.Cells("Dis").Value.ToString()

        dgvSearch.Visible = False
        FetchItemByID(txtItemID.Text.Trim()) ' Fetch full details including stock and measure
        txtQuantity.Text = "1" ' Default qty to 1 for quick entry
        txtQuantity.Focus() ' Point to Qty as requested
        txtQuantity.SelectAll()
    End Sub

    Private Sub dgvSearch_KeyDown(sender As Object, e As KeyEventArgs) Handles dgvSearch.KeyDown
        If e.KeyCode = Keys.Enter Then
            If dgvSearch.SelectedRows.Count > 0 Then
                If currentSearchMode = "CUSTOMER" Then
                    SelectCustomerFromGrid(dgvSearch.SelectedRows(0).Index)
                Else
                    SelectItemFromGrid(dgvSearch.SelectedRows(0).Index)
                End If
                e.Handled = True
            End If
        ElseIf e.KeyCode = Keys.Escape Then
            dgvSearch.Visible = False
            e.Handled = True
        End If
    End Sub

    ' Hide search grid if user clicks away or focuses elsewhere
    Private Sub dgvSearch_Leave(sender As Object, e As EventArgs) Handles dgvSearch.Leave
        dgvSearch.Visible = False
    End Sub

    ' --- Key Navigation and Focus Flow ---
    Private Sub txtItemID_KeyDown(sender As Object, e As KeyEventArgs) Handles txtItemID.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                txtCustomerAddress.Focus()
            Else
                Dim exactId As String = txtItemID.Text.Trim()
                Dim foundExact As Boolean = False
                
                If String.IsNullOrEmpty(exactId) Then
                    FetchItemByID("")
                    txtDescription.Focus()
                    txtDescription.SelectAll()
                Else
                    Dim openedHere As Boolean = False
                    Try
                        If conn.State = ConnectionState.Closed Then
                            conn.Open()
                            openedHere = True
                        End If
                        Using cmdCheck As New MySqlCommand("SELECT id FROM items WHERE (LOWER(id) = LOWER(@id) OR LOWER(barcode) = LOWER(@id) OR LOWER(REPLACE(id, '-', '')) = LOWER(REPLACE(@id, '-', '')) OR LOWER(REPLACE(barcode, '-', '')) = LOWER(REPLACE(@id, '-', ''))) AND deleted_at IS NULL LIMIT 1", conn)
                            cmdCheck.Parameters.AddWithValue("@id", exactId)
                            Dim dbId = cmdCheck.ExecuteScalar()
                            If dbId IsNot Nothing Then
                                exactId = dbId.ToString()
                                foundExact = True
                            End If
                        End Using
                    Catch
                    Finally
                        If openedHere AndAlso conn.State = ConnectionState.Open Then conn.Close()
                    End Try

                    If foundExact Then
                        dgvSearch.Visible = False
                        FetchItemByID(exactId)
                        txtItemID.Text = exactId
                        txtQuantity.Text = "1" ' Default qty to 1 for quick entry
                        txtQuantity.Focus()
                        txtQuantity.SelectAll()
                    Else
                        MessageBox.Show("The Item ID '" & exactId & "' does not exist in the database. Please select a valid item.", "Item Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtItemID.Focus()
                        txtItemID.SelectAll()
                    End If
                End If
            End If
            e.Handled = True
            e.SuppressKeyPress = True
        ElseIf e.KeyCode = Keys.Down Then
            If dgvSearch.Visible AndAlso dgvSearch.Rows.Count > 0 Then
                dgvSearch.Focus()
            End If
        End If
    End Sub

    Private Sub txtDescription_KeyDown(sender As Object, e As KeyEventArgs) Handles txtDescription.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                txtItemID.Focus()
            Else
                txtQuantity.Focus()
                txtQuantity.SelectAll()
            End If
            e.Handled = True
            e.SuppressKeyPress = True
        ElseIf e.KeyCode = Keys.Down Then
            If dgvSearch.Visible AndAlso dgvSearch.Rows.Count > 0 Then
                dgvSearch.Focus()
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub txtQuantity_KeyDown(sender As Object, e As KeyEventArgs) Handles txtQuantity.KeyDown
        If e.KeyCode = Keys.Enter Then
            FormatQtyInput()
            If e.Shift Then
                txtDescription.Focus()
            Else
                txtSellingPrice1.Focus()
                txtSellingPrice1.SelectAll()
            End If
            e.Handled = True
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub txtQuantity_Leave(sender As Object, e As EventArgs) Handles txtQuantity.Leave
        FormatQtyInput()
    End Sub

    Private Sub txtSellingPrice1_KeyDown(sender As Object, e As KeyEventArgs) Handles txtSellingPrice1.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                txtQuantity.Focus()
            Else
                txtDiscount.Focus()
                txtDiscount.SelectAll()
            End If
            e.Handled = True
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub txtDiscount_KeyDown(sender As Object, e As KeyEventArgs) Handles txtDiscount.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                txtSellingPrice1.Focus()
            Else
                txtAmount.Focus()
                txtAmount.SelectAll()
            End If
            e.Handled = True
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub txtAmount_KeyDown(sender As Object, e As KeyEventArgs) Handles txtAmount.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                txtDiscount.Focus()
            Else
                ComboBoxVat.Focus()
            End If
            e.Handled = True
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub ComboBoxVat_KeyDown(sender As Object, e As KeyEventArgs) Handles ComboBoxVat.KeyDown
        If e.KeyCode = Keys.Enter Then
            If e.Shift Then
                txtAmount.Focus()
            Else
                AddItemToBill()
            End If
            e.Handled = True
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Function ItemExistsInDatabase(id As String) As Boolean
        If String.IsNullOrWhiteSpace(id) Then Return False
        Dim exists As Boolean = False
        Dim openedHere As Boolean = False
        Try
            If conn.State = ConnectionState.Closed Then
                conn.Open()
                openedHere = True
            End If
            Using cmdCheck As New MySqlCommand("SELECT COUNT(*) FROM items WHERE (LOWER(id) = LOWER(@id) OR LOWER(barcode) = LOWER(@id) OR LOWER(REPLACE(id, '-', '')) = LOWER(REPLACE(@id, '-', '')) OR LOWER(REPLACE(barcode, '-', '')) = LOWER(REPLACE(@id, '-', ''))) AND deleted_at IS NULL", conn)
                cmdCheck.Parameters.AddWithValue("@id", id.Trim())
                Dim cnt As Long = Convert.ToInt64(cmdCheck.ExecuteScalar())
                If cnt > 0 Then
                    exists = True
                End If
            End Using
        Catch
        Finally
            If openedHere AndAlso conn.State = ConnectionState.Open Then conn.Close()
        End Try
        Return exists
    End Function

    Private Function ResolveItemID(id As String) As String
        Dim actualId As String = id
        Dim openedHere As Boolean = False
        Try
            If conn.State = ConnectionState.Closed Then
                conn.Open()
                openedHere = True
            End If
            Using cmdResolve As New MySqlCommand("SELECT id FROM items WHERE (LOWER(id) = LOWER(@id) OR LOWER(barcode) = LOWER(@id) OR LOWER(REPLACE(id, '-', '')) = LOWER(REPLACE(@id, '-', '')) OR LOWER(REPLACE(barcode, '-', '')) = LOWER(REPLACE(@id, '-', ''))) AND deleted_at IS NULL LIMIT 1", conn)
                cmdResolve.Parameters.AddWithValue("@id", id)
                Dim res = cmdResolve.ExecuteScalar()
                If res IsNot Nothing Then
                    actualId = res.ToString()
                End If
            End Using
        Catch
        Finally
            If openedHere AndAlso conn.State = ConnectionState.Open Then conn.Close()
        End Try
        Return actualId
    End Function

    Private Function GetCurrentStock(itemId As String, locationName As String) As Decimal
        Dim stock As Decimal = 0
        Dim actualId As String = ResolveItemID(itemId)
        Dim openedHere As Boolean = False
        Try
            If conn.State = ConnectionState.Closed Then
                conn.Open()
                openedHere = True
            End If
            Dim sql As String = "SELECT IFNULL(SUM(st_qty), 0) FROM items_stock WHERE item_id = @id AND location_id = (SELECT id FROM location WHERE location_name = @loc)"
            Using cmd As New MySqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@id", actualId)
                cmd.Parameters.AddWithValue("@loc", If(String.IsNullOrWhiteSpace(locationName), "MAIN STOCK", locationName))
                Dim res = cmd.ExecuteScalar()
                If res IsNot Nothing Then
                    Decimal.TryParse(res.ToString(), stock)
                End If
            End Using
        Catch
        Finally
            If openedHere AndAlso conn.State = ConnectionState.Open Then conn.Close()
        End Try
        Return stock
    End Function

    Private Function GetTotalQtyInBill(itemId As String, excludeIndex As Integer) As Decimal
        Dim totalQty As Decimal = 0
        Dim actualId As String = ResolveItemID(itemId)
        If dtBill IsNot Nothing Then
            For i As Integer = 0 To dtBill.Rows.Count - 1
                If i = excludeIndex Then Continue For
                Dim row As DataRow = dtBill.Rows(i)
                Dim rowItemId As String = ResolveItemID(row("Item ID").ToString())
                If String.Equals(rowItemId, actualId, StringComparison.OrdinalIgnoreCase) Then
                    Dim q As Decimal = 0
                    Decimal.TryParse(row("Qty").ToString(), q)
                    totalQty += q
                End If
            Next
        End If
        Return totalQty
    End Function

    ' --- Item Details Fetching and Calculations ---
    Private Sub FetchItemByID(id As String)
        Dim actualItemId As String = id
        Dim openedHereForResolve As Boolean = False
        Try
            If conn.State = ConnectionState.Closed Then
                conn.Open()
                openedHereForResolve = True
            End If
            Using cmdResolve As New MySqlCommand("SELECT id FROM items WHERE LOWER(id) = LOWER(@id) OR LOWER(barcode) = LOWER(@id) OR LOWER(REPLACE(id, '-', '')) = LOWER(REPLACE(@id, '-', '')) OR LOWER(REPLACE(barcode, '-', '')) = LOWER(REPLACE(@id, '-', '')) LIMIT 1", conn)
                cmdResolve.Parameters.AddWithValue("@id", id)
                Dim res = cmdResolve.ExecuteScalar()
                If res IsNot Nothing Then
                    actualItemId = res.ToString()
                End If
            End Using
        Catch
        Finally
            If openedHereForResolve AndAlso conn.State = ConnectionState.Open Then conn.Close()
        End Try

        Dim openedHere As Boolean = False
        Dim infoFetched As Boolean = False
        Try
            If conn.State = ConnectionState.Closed Then
                conn.Open()
                openedHere = True
            End If

            ' FIFO & Location Priority Logic:
            ' 1. Check selected location (oldest available)
            ' 2. If none, check 'MAIN STOCK' (oldest available)
            ' 3. If none, check ANY other location (oldest available)
            ' 4. If absolutely NO stock, fetch the most recent batch record just for supplier/price display.

            Dim selectedLoc As String = If(String.IsNullOrWhiteSpace(ComboBoxLocation.Text), "MAIN STOCK", ComboBoxLocation.Text)

            Dim batchSql As String = "SELECT s.st_qty, i.selling_price, i.avg_cost, s.item_cost, s.supplier_id, sup.name as supplier_name, i.description, i.measure, i.whole_selling_price, i.retail_selling_price, l.location_name " &
                                     "FROM items_stock s " &
                                     "JOIN items i ON s.item_id = i.id " &
                                     "LEFT JOIN supplier sup ON s.supplier_id = sup.id " &
                                     "JOIN location l ON s.location_id = l.id " &
                                     "WHERE s.item_id = @id AND s.st_qty > 0 " &
                                     "ORDER BY (CASE WHEN l.location_name = @selLoc THEN 1 " &
                                     "               WHEN l.location_name = 'MAIN STOCK' THEN 2 " &
                                     "               ELSE 3 END) ASC, s.date ASC LIMIT 1"

            Using cmd As New MySqlCommand(batchSql, conn)
                cmd.Parameters.AddWithValue("@id", actualItemId)
                cmd.Parameters.AddWithValue("@selLoc", selectedLoc)

                Using dr As MySqlDataReader = cmd.ExecuteReader()
                    If dr.Read() Then
                        ' Found available stock using priority - Store values first
                        Dim desc As String = dr("description").ToString()
                        Dim iCost As String = dr("avg_cost").ToString()
                        Dim sName As String = dr("supplier_name").ToString()
                        Dim fLoc As String = dr("location_name").ToString()
                        Dim mUnit As String = If(dr("measure") Is DBNull.Value, "Qyt", dr("measure").ToString())
                        Dim currentSid As Object = dr("supplier_id")

                        ' Determine Selling Price based on CheckBoxes
                        Dim sPrice As String = dr("selling_price").ToString()
                        If CheckBoxWholesale.Checked AndAlso Not IsDBNull(dr("whole_selling_price")) Then
                            sPrice = dr("whole_selling_price").ToString()
                        ElseIf CheckBoxRetail.Checked AndAlso Not IsDBNull(dr("retail_selling_price")) Then
                            sPrice = dr("retail_selling_price").ToString()
                        End If

                        dr.Close() ' Now safe to close

                        ' Update UI
                        txtDescription.Text = desc
                        txtSellingPrice1.Text = sPrice
                        txtSellingPrice1.Tag = iCost ' Batch Cost (Accounting)
                        ' Fetch Average Cost for validation
                        Using cmdAvg As New MySqlCommand("SELECT avg_cost FROM items WHERE id = @id", conn)
                            cmdAvg.Parameters.AddWithValue("@id", actualItemId)
                            txtItemID.Tag = cmdAvg.ExecuteScalar() ' Average Cost (Validation)
                        End Using
                        lblSupplierInfo.Text = "Supplier: " & sName
                        lblSupplierInfo.Visible = True

                        ' Auto-update location if priority moved it
                        If selectedLoc <> fLoc Then
                            For i As Integer = 0 To ComboBoxLocation.Items.Count - 1
                                If DirectCast(ComboBoxLocation.Items(i), DataRowView)("location_name").ToString() = fLoc Then
                                    ComboBoxLocation.SelectedIndex = i
                                    Exit For
                                End If
                            Next
                        End If

                        ' Update Qty Label
                        For Each ctrl As Control In GroupBox1.Controls
                            If ctrl.Name = "Label27" Then
                                ctrl.Text = "Qyt (" & mUnit & ")"
                                Exit For
                            End If
                        Next

                        ' --- SYNC MASTER SUPPLIER & PRICES ---
                        ' Sync removed to prevent price overwrite during sales
                        ' SyncItemMasterData(id)
                        infoFetched = True
                    Else
                        ' No stock available anywhere - Fallback to most recent batch for info
                        dr.Close()
                        Dim fallbackSql As String = "SELECT i.selling_price, i.avg_cost, s.item_cost, sup.name as supplier_name, i.description, i.measure, i.whole_selling_price, i.retail_selling_price " &
                                                   "FROM items_stock s " &
                                                   "JOIN items i ON s.item_id = i.id " &
                                                   "LEFT JOIN supplier sup ON s.supplier_id = sup.id " &
                                                   "WHERE s.item_id = @id " &
                                                   "ORDER BY s.date DESC LIMIT 1"
                        Using cmdFallback As New MySqlCommand(fallbackSql, conn)
                            cmdFallback.Parameters.AddWithValue("@id", actualItemId)
                            Using drFb = cmdFallback.ExecuteReader()
                                If drFb.Read() Then
                                    txtDescription.Text = drFb("description").ToString()

                                    Dim sPriceFb As String = drFb("selling_price").ToString()
                                    If CheckBoxWholesale.Checked AndAlso Not IsDBNull(drFb("whole_selling_price")) Then
                                        sPriceFb = drFb("whole_selling_price").ToString()
                                    ElseIf CheckBoxRetail.Checked AndAlso Not IsDBNull(drFb("retail_selling_price")) Then
                                        sPriceFb = drFb("retail_selling_price").ToString()
                                    End If

                                    txtSellingPrice1.Text = sPriceFb
                                    txtSellingPrice1.Tag = drFb("item_cost").ToString() ' Batch Cost
                                    txtItemID.Tag = drFb("avg_cost").ToString() ' Avg Cost
                                    lblSupplierInfo.Text = "Supplier: " & drFb("supplier_name").ToString()
                                    lblSupplierInfo.Visible = True
                                    infoFetched = True
                                End If
                            End Using
                        End Using
                    End If

                    ' If still empty (meaning no rows in items_stock at all), fall back directly to items table
                    If Not infoFetched Then
                        Using cmdItemsTable As New MySqlCommand("SELECT description, selling_price, whole_selling_price, retail_selling_price, avg_cost, item_cost FROM items WHERE id = @id", conn)
                            cmdItemsTable.Parameters.AddWithValue("@id", actualItemId)
                            Using drItems = cmdItemsTable.ExecuteReader()
                                If drItems.Read() Then
                                    txtDescription.Text = drItems("description").ToString()
                                    Dim sPriceItems As String = drItems("selling_price").ToString()
                                    If CheckBoxWholesale.Checked AndAlso Not IsDBNull(drItems("whole_selling_price")) Then
                                        sPriceItems = drItems("whole_selling_price").ToString()
                                    ElseIf CheckBoxRetail.Checked AndAlso Not IsDBNull(drItems("retail_selling_price")) Then
                                        sPriceItems = drItems("retail_selling_price").ToString()
                                    End If
                                    txtSellingPrice1.Text = sPriceItems
                                    txtSellingPrice1.Tag = If(IsDBNull(drItems("item_cost")), "0.00", drItems("item_cost").ToString())
                                    txtItemID.Tag = If(IsDBNull(drItems("avg_cost")), "0.00", drItems("avg_cost").ToString())
                                    lblSupplierInfo.Text = "Supplier: None"
                                    lblSupplierInfo.Visible = True
                                End If
                            End Using
                        End Using
                    End If
                End Using
            End Using

            ' Finally, get TOTAL stock for the CURRENTLY SELECTED location to show in label
            Dim stockLoc As String = If(String.IsNullOrWhiteSpace(ComboBoxLocation.Text), "MAIN STOCK", ComboBoxLocation.Text)
            Dim stockSql As String = "SELECT IFNULL(SUM(st_qty), 0) FROM items_stock WHERE item_id = @id AND location_id = (SELECT id FROM location WHERE location_name = @loc)"
            Using cmdStock As New MySqlCommand(stockSql, conn)
                cmdStock.Parameters.AddWithValue("@id", actualItemId)
                cmdStock.Parameters.AddWithValue("@loc", stockLoc)
                lblCurrentStock.Text = cmdStock.ExecuteScalar().ToString()
            End Using

            If openedHere Then conn.Close()
            UpdateItemCalculations()
        Catch ex As Exception
            If openedHere AndAlso conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub


    Private Sub UpdateItemCalculations()
        If isFormLoading Then Return ' Guard against calculations during form load
        Try
            Dim qty As Decimal = 0
            Dim price As Decimal = 0
            Dim discPercent As Decimal = 0
            Dim vatPercent As Decimal = 0

            Decimal.TryParse(txtQuantity.Text, qty)
            Decimal.TryParse(txtSellingPrice1.Text, price)
            Decimal.TryParse(txtDiscount.Text, discPercent)

            If CheckBoxWholesale.Checked AndAlso CheckBoxPrintAsRetail.Checked Then
                Dim wholesalePrice As Decimal = GetItemWholesalePrice(txtItemID.Text.Trim())
                If wholesalePrice > 0 Then
                    price = wholesalePrice
                End If
            End If

            ' Get VAT percentage from ComboBoxVat
            If ComboBoxVat.SelectedItem IsNot Nothing Then
                Dim rowVat As DataRowView = DirectCast(ComboBoxVat.SelectedItem, DataRowView)
                Decimal.TryParse(rowVat("vat_value").ToString(), vatPercent)
            End If

            Dim finalAmount As Decimal = 0
            Dim discAmount As Decimal = 0

            If CheckBoxIsVat.Checked AndAlso vatPercent > 0 Then
                ' VAT Bill Formula: Amount = (SellingPrice / (1 + VAT/100)) * (1 - Dis/100) * Qty
                Dim netPricePerUnit = price / (1 + (vatPercent / 100))
                discAmount = (netPricePerUnit * (discPercent / 100)) * qty
                finalAmount = (netPricePerUnit * (1 - (discPercent / 100))) * qty
            Else
                ' Normal Bill Formula: Amount = SellingPrice * (1 - Dis/100) * Qty
                discAmount = (price * (discPercent / 100)) * qty
                finalAmount = (price * (1 - (discPercent / 100))) * qty
            End If

            txtAmount.Text = finalAmount.ToString("N2")
            txtItemDiscountVal.Text = discAmount.ToString("N2")

            ' --- Exchange Mode UI Toggle ---
            If qty < 0 Then
                ' If quantity is negative, we are in "Exchange/Return" mode on a new line
                cmbReturnReason.Visible = True
                Label5.Visible = True
                lblSupplierInfo.Visible = True
                If cmbReturnReason.SelectedIndex <= 0 Then cmbReturnReason.SelectedIndex = 1 ' Default to first reason (Ex: "Exchange")
            ElseIf Not isEditingHistory Then
                ' Only hide if we aren't editing history (where it MUST stay visible)
                cmbReturnReason.Visible = False
                Label5.Visible = False
            End If

        Catch ex As Exception
            ' Silent fail for real-time calc
        End Try
    End Sub

    ' --- Real-time Change Handlers ---
    Private Sub ItemInput_TextChanged(sender As Object, e As EventArgs) Handles txtQuantity.TextChanged, txtSellingPrice1.TextChanged, txtDiscount.TextChanged
        UpdateItemCalculations()
    End Sub

    Private Sub ComboBoxVat_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxVat.SelectedIndexChanged
        If ComboBoxVat.SelectedValue IsNot Nothing Then
            Dim currentVatId As Integer = 1
            Integer.TryParse(ComboBoxVat.SelectedValue.ToString(), currentVatId)

            If currentVatId <> 1 Then
                ' Item logic: If VAT is selected for an item, the global VAT checkbox must be true
                If Not CheckBoxIsVat.Checked Then CheckBoxIsVat.Checked = True
            Else
                ' If No VAT is selected, we do NOT auto-check the global VAT checkbox
                ' CheckBoxIsVat.Checked = False ' Optional: Don't force false if it was already true? 
                ' User said "remain False unless explicitly set to True by the user or by specific item VAT selections"
            End If
        End If
        UpdateItemCalculations()
    End Sub

    Private Sub AddItemToBill()
        ' Prevent adding a new row while an existing row is selected for editing
        If selectedIndex <> -1 Then
            MessageBox.Show("You have selected an existing row. Please use Edit or Delete for that row, or click Add New to start a new line.", "Row Selected", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        ' Validate Cashier/Password ONLY if adding a return (Negative Quantity)
        Dim qtyInput As Decimal = 0
        Decimal.TryParse(txtQuantity.Text, qtyInput)

        If qtyInput < 0 Then
            ' For returns, identification is mandatory immediately
            If Not IdentifyCashierByPassword() Then
                MessageBox.Show("Please enter a valid Cashier ID to process a Return item.", "Authorization Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtCashierID.Focus()
                Return
            End If

            ' Check if the active cashier is admin1/2/3/4
            Dim currentCashier As String = cmbCashier.Text.Trim().ToLower()
            If currentCashier = "admin1" OrElse currentCashier = "admin2" OrElse currentCashier = "admin3" OrElse currentCashier = "admin4" Then
                MessageBox.Show("Please enter a valid Cashier ID", "Invalid Cashier", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtCashierID.Focus()
                txtCashierID.SelectAll()
                Return
            End If
        End If

        ' Basic Input Check
        Dim rawItemId As String = txtItemID.Text.Trim()
        If String.IsNullOrEmpty(rawItemId) OrElse txtQuantity.Text = "" OrElse Not IsNumeric(txtQuantity.Text) Then
            MessageBox.Show("Please enter valid Item and Quantity", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtItemID.Focus()
            txtItemID.SelectAll()
        ElseIf Not ItemExistsInDatabase(rawItemId) Then
            MessageBox.Show("The Item ID '" & rawItemId & "' does not exist in the database. Please select a valid item.", "Item Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtItemID.Focus()
            txtItemID.SelectAll()
        Else
            ' Resolve item ID to exact database ID
            Dim actualItemId As String = ResolveItemID(rawItemId)
            txtItemID.Text = actualItemId

            ' Ensure item details are fetched if description/price is blank
            If String.IsNullOrWhiteSpace(txtDescription.Text) OrElse String.IsNullOrWhiteSpace(txtSellingPrice1.Text) Then
                FetchItemByID(actualItemId)
            End If
            ' VAT VALIDATION
            Dim vatLabel As String = ComboBoxVat.Text
            Dim vatPercent As Decimal = 0
            Dim currentVatId As Integer = If(ComboBoxVat.SelectedValue IsNot Nothing, Convert.ToInt32(ComboBoxVat.SelectedValue), 1)

            ' SINGLE VAT RATE PER BILL ENFORCEMENT
            If dtBill.Rows.Count > 0 Then
                Dim existingVatLabel As String = If(dtBill.Rows(0)("VAT") Is DBNull.Value, "", dtBill.Rows(0)("VAT").ToString().Trim())
                Dim newItemVatLabel As String = If(currentVatId <> 1, vatLabel.Trim(), "")

                ' Normalize "No VAT" strings for comparison
                Dim normalizedExisting As String = If(existingVatLabel = "" OrElse existingVatLabel.ToUpper().Contains("NO VAT"), "NONE", existingVatLabel.ToUpper())
                Dim normalizedNew As String = If(newItemVatLabel = "" OrElse newItemVatLabel.ToUpper().Contains("NO VAT"), "NONE", newItemVatLabel.ToUpper())

                If normalizedExisting <> normalizedNew Then
                    Dim displayExVat As String = If(existingVatLabel = "", "No VAT", existingVatLabel)
                    Dim displayNewVat As String = If(newItemVatLabel = "", "No VAT", newItemVatLabel)

                    MessageBox.Show("This invoice is already restricted to '" & displayExVat & "'. " & vbCrLf &
                                    "You cannot add an item with '" & displayNewVat & "' to the same bill." & vbCrLf &
                                    "Please use a separate slot or clear the current bill.",
                                    "VAT Rate Mismatch", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
            End If

            If currentVatId <> 1 Then ' Not No VAT
                If ComboBoxVat.SelectedItem IsNot Nothing Then
                    Dim rowVat As DataRowView = DirectCast(ComboBoxVat.SelectedItem, DataRowView)
                    Decimal.TryParse(rowVat("vat_value").ToString(), vatPercent)
                End If

                ' SYNC BILL LEVEL VAT FOR THE FIRST ITEM
                If dtBill.Rows.Count = 0 Then
                    ComboBoxTotalVat.SelectedValue = currentVatId
                End If

                ' Rule: vat bii, vat check box ek false thiygn bill dnn ba.
                If Not CheckBoxIsVat.Checked Then
                    CheckBoxIsVat.Checked = True ' Auto-enable instead of blocking
                End If
            ElseIf CheckBoxIsVat.Checked Then
                ' If global VAT is ON but item is "No VAT", we might still need a percentage for the bill calculation if it's a global VAT bill
                ' But usually, ID 1 is 0%.
                vatPercent = 0
            End If

            Dim qty As Decimal = 0
            Decimal.TryParse(txtQuantity.Text, qty)
            Dim unitPrice As Decimal = 0
            Decimal.TryParse(txtSellingPrice1.Text, unitPrice)
            Dim disc As Decimal = 0
            Decimal.TryParse(txtDiscount.Text, disc)

            Dim printRetailVal As Decimal = unitPrice
            If CheckBoxWholesale.Checked AndAlso CheckBoxPrintAsRetail.Checked Then
                Dim wholesalePrice As Decimal = GetItemWholesalePrice(txtItemID.Text.Trim())
                If wholesalePrice > 0 Then
                    unitPrice = wholesalePrice
                End If
                If printRetailVal = wholesalePrice Then
                    Dim retailPrice As Decimal = GetItemRetailPrice(txtItemID.Text.Trim())
                    If retailPrice > 0 Then
                        printRetailVal = retailPrice
                    End If
                End If
            End If

            ' WHOLESALE STOCK ALERT VALIDATION (5 QTY LIMIT)
            If qty > 0 AndAlso CheckBoxWholesale.Checked Then
                Dim wholesaleLoc As String = If(ComboBoxLocation.Text IsNot Nothing, ComboBoxLocation.Text.Trim(), "MAIN STOCK")
                If String.IsNullOrEmpty(wholesaleLoc) Then wholesaleLoc = "MAIN STOCK"
                
                Dim currentStock As Decimal = GetCurrentStock(txtItemID.Text.Trim(), wholesaleLoc)
                If currentStock > 0 Then
                    If currentStock < 5 Then
                        Dim result As DialogResult = MessageBox.Show("ප්‍රමාණවත් ප්‍රමාණයක් නොමැත. එසේ වුවද save කිරීමට අවශ්‍යද? (අයිතමය: " & txtItemID.Text.Trim() & ")", "තොග ප්‍රමාණවත් නොවේ", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                        If result = DialogResult.No Then
                            txtQuantity.Focus()
                            txtQuantity.SelectAll()
                            Return
                        End If
                    Else
                        Dim alreadyInBill As Decimal = GetTotalQtyInBill(txtItemID.Text.Trim(), -1)
                        Dim maxAllowed As Decimal = currentStock - 5
                        Dim maxAllowedForThisLine As Decimal = maxAllowed - alreadyInBill

                        If maxAllowedForThisLine < 0 Then
                            Dim result As DialogResult = MessageBox.Show("මෙම අයිතමයෙන් ඔබට ලබාගත හැක්කේ 0 ක් පමණි. එසේ වුවද save කිරීමට අවශ්‍යද? (අයිතමය: " & txtItemID.Text.Trim() & ")", "සීමාව ඉක්මවා ඇත", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                            If result = DialogResult.No Then
                                txtQuantity.Text = "0"
                                txtQuantity.Focus()
                                txtQuantity.SelectAll()
                                Return
                            End If
                        ElseIf qty > maxAllowedForThisLine Then
                            Dim result As DialogResult = MessageBox.Show("මෙම අයිතමයෙන් ඔබට ලබාගත හැක්කේ " & maxAllowedForThisLine.ToString("G") & " ක් පමණි. එසේ වුවද save කිරීමට අවශ්‍යද? (අයිතමය: " & txtItemID.Text.Trim() & ")", "සීමාව ඉක්මවා ඇත", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                            If result = DialogResult.No Then
                                txtQuantity.Text = maxAllowedForThisLine.ToString("G")
                                txtQuantity.Focus()
                                txtQuantity.SelectAll()
                                Return
                            End If
                        End If
                    End If
                End If
            End If

            Dim netUnitPrice As Decimal = unitPrice
            Dim lineTotal As Decimal = 0

            If CheckBoxIsVat.Checked AndAlso vatPercent > 0 Then
                ' VAT Bill: amount = unitPrice / (1 + vat/100) * (1 - dis/100) * qty
                netUnitPrice = unitPrice / (1 + vatPercent / 100)
                lineTotal = (netUnitPrice * (1 - disc / 100)) * qty
            Else
                ' Normal Bill: amount = unitPrice * (1-dis/100) * qty
                lineTotal = (unitPrice * (1 - disc / 100)) * qty
            End If

            ' Mandatory Return Reason for Negative Quantities (Exchange/Return)
            If qty < 0 Then
                Dim reasonText As String = If(cmbReturnReason.Text, "").Trim()
                If cmbReturnReason.SelectedIndex <= 0 OrElse String.Equals(reasonText, "None", StringComparison.OrdinalIgnoreCase) Then
                    MessageBox.Show("Please select a Return Reason for the item being returned.", "Reason Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    cmbReturnReason.Focus()
                    cmbReturnReason.DroppedDown = True
                    Return
                End If
            End If

            ' Profit Validation (Compare against AVG COST)
            Dim avgCostVal As Decimal = 0
            Decimal.TryParse(If(txtItemID.Tag IsNot Nothing, txtItemID.Tag.ToString(), "0"), avgCostVal)

            ' Compare discounted price per unit to AVG cost
            Dim finalUnitPricePerUnit = Math.Abs(lineTotal / qty)
            If avgCostVal > 0 AndAlso finalUnitPricePerUnit <= avgCostVal Then
                MessageBox.Show(
                    "Total amount is low. Sale price (" & finalUnitPricePerUnit.ToString("N2") & ") must be higher than average item cost (" & avgCostVal.ToString("N2") & ") to ensure profit.",
                    "Low Profit Margin", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSellingPrice1.Focus()
                txtSellingPrice1.SelectAll()
                Return
            End If

            ' Add to Grid (New structure: Item ID, Description, Qty, Price, Dis, Location, Total, VAT, ItemCost)
            ' Price in grid should be the unit price used for calc (requested)
            Dim finalVatDisplay As String = If(currentVatId <> 1, vatLabel, "")
            Dim selectedLoc As String = If(ComboBoxLocation.Text IsNot Nothing, ComboBoxLocation.Text.Trim(), "MAIN STOCK")
            If String.IsNullOrEmpty(selectedLoc) Then selectedLoc = "MAIN STOCK"

            Dim selectedLocId As Object = If(ComboBoxLocation.SelectedValue IsNot Nothing, ComboBoxLocation.SelectedValue, 1)

            Dim itemCost As String = If(txtSellingPrice1.Tag IsNot Nothing, txtSellingPrice1.Tag.ToString(), "0")

            ' User requested: clear credit balance display as soon as the first item is added
            If dtBill.Rows.Count = 0 Then
                lblCreditBalance.Text = ""
            End If

            ' Check if item already exists in the bill list to overwrite it
            Dim actualId As String = ResolveItemID(txtItemID.Text.Trim())
            Dim existingRowIndex As Integer = -1
            For i As Integer = 0 To dtBill.Rows.Count - 1
                Dim rowItemId As String = ResolveItemID(dtBill.Rows(i)("Item ID").ToString())
                If String.Equals(rowItemId, actualId, StringComparison.OrdinalIgnoreCase) Then
                    existingRowIndex = i
                    Exit For
                End If
            Next

            If existingRowIndex <> -1 Then
                ' Overwrite the existing row directly
                Dim existingRow As DataRow = dtBill.Rows(existingRowIndex)
                existingRow("Item ID") = txtItemID.Text
                existingRow("Description") = txtDescription.Text
                existingRow("Qty") = qty
                existingRow("Selling Price") = unitPrice
                existingRow("Dis") = disc
                existingRow("Location") = selectedLoc
                existingRow("Total/Amount") = lineTotal
                existingRow("VAT") = finalVatDisplay
                existingRow("ItemCost") = itemCost
                existingRow("AvgCost") = avgCostVal
                existingRow("LocationID") = selectedLocId
                existingRow("Reason") = ""
                existingRow("IsOriginal") = False
                existingRow("PrintRetailPrice") = printRetailVal
                If DataGridView2.Columns("PrintRetailPrice") IsNot Nothing Then
                    DataGridView2.Columns("PrintRetailPrice").Visible = False
                End If

                ' Ensure grid is showing the bill
                DataGridView2.DataSource = dtBill
                If DataGridView2.Columns("ItemCost") IsNot Nothing Then
                    DataGridView2.Columns("ItemCost").Visible = False
                End If
                If DataGridView2.Columns("AvgCost") IsNot Nothing Then
                    DataGridView2.Columns("AvgCost").Visible = False
                End If
                If DataGridView2.Columns("LocationID") IsNot Nothing Then
                    DataGridView2.Columns("LocationID").Visible = False
                End If
                FormatBillGrid()

                CalculateGrandTotal()
                ' LIVE AUTO-SAVE
                If SlotID > 0 Then SaveSlotState(SlotID)
                ClearEntryFields()

                ' Reset Return UI for new additions
                cmbReturnReason.Visible = False
                Label5.Visible = False
                lblSupplierInfo.Visible = False
                Return
            End If

            dtBill.Rows.Add("", txtItemID.Text, txtDescription.Text, qty, unitPrice, disc, selectedLoc, lineTotal, finalVatDisplay, itemCost, avgCostVal, selectedLocId, "", False, printRetailVal)
            If DataGridView2.Columns("PrintRetailPrice") IsNot Nothing Then
                DataGridView2.Columns("PrintRetailPrice").Visible = False
            End If

            ' Ensure grid is showing the bill
            DataGridView2.DataSource = dtBill
            If DataGridView2.Columns("ItemCost") IsNot Nothing Then
                DataGridView2.Columns("ItemCost").Visible = False
            End If
            If DataGridView2.Columns("AvgCost") IsNot Nothing Then
                DataGridView2.Columns("AvgCost").Visible = False
            End If
            If DataGridView2.Columns("LocationID") IsNot Nothing Then
                DataGridView2.Columns("LocationID").Visible = False
            End If
            FormatBillGrid()

            CalculateGrandTotal()
            ' LIVE AUTO-SAVE
            If SlotID > 0 Then SaveSlotState(SlotID)
            ClearEntryFields()

            ' Reset Return UI for new additions
            cmbReturnReason.Visible = False
            Label5.Visible = False
            lblSupplierInfo.Visible = False
        End If
    End Sub


    Private Sub DataGridView2_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView2.CellClick
        If DataGridView2.CurrentRow Is Nothing Then Exit Sub

        Dim rowView As DataRowView = DirectCast(DataGridView2.CurrentRow.DataBoundItem, DataRowView)
        selectedIndex = dtBill.Rows.IndexOf(rowView.Row)
        If selectedIndex < 0 OrElse selectedIndex >= dtBill.Rows.Count Then Exit Sub

        Dim row As DataGridViewRow = DataGridView2.CurrentRow

        ' If we are in historical view mode (editing an invoice), allow loading items back to fields
        If DataGridView2.Columns.Contains("Item ID") Then
            txtItemID.Text = row.Cells("Item ID").Value.ToString()
            txtDescription.Text = row.Cells("Description").Value.ToString()
            txtAmount.Text = row.Cells("Selling Price").Value.ToString()
            txtDiscount.Text = row.Cells("Dis").Value.ToString()
            txtSellingPrice1.Tag = row.Cells("ItemCost").Value.ToString()
            ' Fetch Supplier for Returns and set button visuals
            If isEditingHistory Then
                FetchItemByID(txtItemID.Text.Trim())

                Dim isOriginal As Boolean = If(row.Cells("IsOriginal") IsNot Nothing AndAlso row.Cells("IsOriginal").Value IsNot DBNull.Value, Convert.ToBoolean(row.Cells("IsOriginal").Value), False)
                If isOriginal Then
                    btnUpdate.Text = "Return"
                    btnUpdate.BackColor = Color.LightGreen
                Else
                    btnUpdate.Text = "Edit"
                    btnUpdate.BackColor = Color.White
                End If
            Else
                FetchItemByID(txtItemID.Text.Trim())
                btnUpdate.Text = "Edit"
                btnUpdate.BackColor = Color.White
            End If

            ' Restore the exact values from the grid so that FetchItemByID does not overwrite custom edits
            txtSellingPrice1.Text = row.Cells("Selling Price").Value.ToString()
            txtDiscount.Text = row.Cells("Dis").Value.ToString()
            If DataGridView2.Columns.Contains("Qty") Then
                txtQuantity.Text = FormatQuantity(row.Cells("Qty").Value)
            End If

            ' Restore Per-item Return Reason
            isRestoringReason = True
            If row.Cells("Reason").Value IsNot Nothing Then
                Dim rowReason = row.Cells("Reason").Value.ToString()
                If rowReason.StartsWith("Other- ") Then
                    cmbReturnReason.Text = "Other"
                    txtOtherReason.Text = rowReason.Substring(7)
                    txtOtherReason.Visible = True
                ElseIf Not String.IsNullOrEmpty(rowReason) Then
                    cmbReturnReason.Text = rowReason
                    txtOtherReason.Visible = (cmbReturnReason.Text = "Other")
                    If Not txtOtherReason.Visible Then txtOtherReason.Text = ""
                Else
                    cmbReturnReason.SelectedIndex = 0
                    txtOtherReason.Text = ""
                    txtOtherReason.Visible = False
                End If
            End If
            isRestoringReason = False

            txtQuantity.Focus()
            txtQuantity.SelectAll()
        End If
    End Sub

    Private Sub btnInvoiceDetails_Click(sender As Object, e As EventArgs) Handles btnInvoiceDetails.Click
        ' Try to identify by password first
        IdentifyCashierByPassword()

        ' Check if the active cashier is admin1/2/3/4
        Dim currentCashier As String = cmbCashier.Text.Trim().ToLower()
        If currentCashier = "admin1" OrElse currentCashier = "admin2" OrElse currentCashier = "admin3" OrElse currentCashier = "admin4" Then
            MessageBox.Show("Please enter a valid Cashier ID", "Invalid Cashier", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtCashierID.Focus()
            txtCashierID.SelectAll()
            Return
        End If

        If dtBill.Rows.Count > 0 Then
            Dim confirm = MessageBox.Show("Billing is in progress. It is recommended to switch to another window to view invoices to avoid cluttering your current bill. Switch now?", "Progress Detected", MessageBoxButtons.YesNo, MessageBoxIcon.Information)
            If confirm = DialogResult.Yes Then
                ' Save current slot state BEFORE looping (since ReserveSlot changes SlotID)
                Dim currentSlotBeforeSwitch As Integer = SlotID
                SaveSlotState(currentSlotBeforeSwitch)

                ' Find next available slot
                Dim targetFound As Boolean = False
                For i As Integer = 1 To 30
                    If i <> currentSlotBeforeSwitch Then
                        If ReserveSlot(i, True) Then
                            targetFound = True
                            Exit For
                        End If
                    End If
                Next

                If targetFound Then
                    ' ReserveSlot already updated SlotID to the new one
                    LoadSlotState(SlotID)
                Else
                    MessageBox.Show("All windows (1-30) are currently in use. Please save or close another window first.", "No Slots Available", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
            Else
                ' User said No - don't load in current active billing window
                Return
            End If
        End If

        LoadCustomerInvoices()
        ' User requested: "invoice details button eke touch krhm credit balance text box ek fill wen ek nvththn"
        ' CalculateTotalCredit()

        ' Show InvDetailsPanel instead of pnlInvoiceHistory
        InvDetailsPanel.Visible = True
        InvDetailsPanel.BringToFront()

        ' Show Mark as Complete button alongside Inv Details view
        btnCompleteInv.Visible = True
        btnCompleteInv.Text = "Back to Sales"
        btnCancelView.Visible = False

        ' Crucially, update the current slot's memory state so if we switch away and back, it stays open 
        ' IF AND ONLY IF we are still in this session.
        If AllSlots.ContainsKey(SlotID) Then
            AllSlots(SlotID).ShowDetailsPanel = True
        End If

        txtInvSearch.Focus()
        txtInvSearch.SelectAll()
    End Sub

    Private Sub txtInvSearch_TextChanged(sender As Object, e As EventArgs) Handles txtInvSearch.TextChanged
        LoadCustomerInvoices()
    End Sub

    Private Sub TextBoxStatus_TextChanged(sender As Object, e As EventArgs) Handles TextBoxStatus.TextChanged
        LoadCustomerInvoices()
    End Sub

    Private Sub dtpInvDateSearch_ValueChanged(sender As Object, e As EventArgs) Handles dtpInvDateSearch.ValueChanged
        LoadCustomerInvoices()
    End Sub

    Private Sub txtInvSearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtInvSearch.KeyDown
        If e.KeyCode = Keys.Enter OrElse e.KeyCode = Keys.Down Then
            TextBoxStatus.Focus()
            TextBoxStatus.SelectAll()
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub TextBoxStatus_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBoxStatus.KeyDown
        If e.KeyCode = Keys.Enter OrElse e.KeyCode = Keys.Down Then
            dtpInvDateSearch.Focus()
            e.SuppressKeyPress = True
        ElseIf e.KeyCode = Keys.Up Then
            txtInvSearch.Focus()
            txtInvSearch.SelectAll()
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub dtpInvDateSearch_KeyDown(sender As Object, e As KeyEventArgs) Handles dtpInvDateSearch.KeyDown
        If e.KeyCode = Keys.Enter OrElse e.KeyCode = Keys.Down Then
            LoadCustomerInvoices()
            dgvInvoices.Focus()
            e.SuppressKeyPress = True
        ElseIf e.KeyCode = Keys.Up Then
            TextBoxStatus.Focus()
            TextBoxStatus.SelectAll()
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub LoadCustomerInvoices()
        Dim openedHere As Boolean = False
        Try
            If conn.State = ConnectionState.Closed Then
                conn.Open()
                openedHere = True
            End If

            Dim filterNo As String = txtInvSearch.Text.Trim()
            Dim filterStatus As String = TextBoxStatus.Text.Trim()
            Dim useDate As Boolean = dtpInvDateSearch.Checked
            Dim filterDate As String = dtpInvDateSearch.Value.ToString("yyyy-MM-dd")

            Dim filterSql As String = ""
            If Not String.IsNullOrEmpty(filterNo) Then
                filterSql &= " AND (t.inv_no LIKE @filterNo OR t.printed_inv_no LIKE @filterNo)"
            End If
            If Not String.IsNullOrEmpty(filterStatus) Then
                filterSql &= " AND t.status LIKE @filterStatus"
            End If
            If useDate Then
                filterSql &= " AND DATE(t.timestamps) = @filterDate"
            End If

            Dim custFilter As String = ""
            If Not String.IsNullOrEmpty(selectedCustomerId) AndAlso 
               Not String.IsNullOrWhiteSpace(txtSalesRep.Text) AndAlso 
               Not String.Equals(txtSalesRep.Text.Trim(), "CASH", StringComparison.OrdinalIgnoreCase) Then
                custFilter = " AND t.customer_id = @c_id"
            End If

            Dim rgrFilter As String = ""
            Dim finRole As String = If(Module1.FinancialRole IsNot Nothing, Module1.FinancialRole.ToLower(), "")
            Dim canAccessGR As Boolean = (Module1.UserRole.ToLower() = "cashier" AndAlso finRole = "seller") AndAlso Module1.IsRgrVisible
            If Not Module1.IsRgrVisible OrElse finRole = "seller" Then
                If canAccessGR Then
                    rgrFilter = " AND t.is_rgr = 0 AND t.inv_no NOT LIKE 'RGR%'"
                Else
                    rgrFilter = " AND t.is_rgr = 0 AND t.inv_no NOT LIKE 'GR%' AND t.inv_no NOT LIKE 'gr%' AND t.inv_no NOT LIKE 'RGR%'"
                End If
            End If

            Dim selectCols As String = "SELECT t.id, t.inv_no as 'Invoice No', t.printed_inv_no as 'Printed No', t.billing_type as 'B.Type', t.inv_type as 'Inv Type', t.payment_type as 'Payment', t.status as 'Status', t.grand_total as 'Total', t.advance_payment as 'Advance', t.paid_amount as 'Paid', t.balance_due as 'Balance', t.timestamps as 'Date'"

            Dim innerCols As String = "SELECT id, inv_no, printed_inv_no, billing_type, inv_type, payment_type, status, grand_total, advance_payment, paid_amount, balance_due, timestamps, customer_id, is_rgr"
            Dim combinedTable As String = "( " & innerCols & " FROM billing UNION ALL " & innerCols & " FROM quotation_billing ) as t"

            Dim localSql = selectCols & " FROM " & combinedTable & " LEFT JOIN customer c ON t.customer_id = c.id WHERE 1=1" & filterSql & custFilter & rgrFilter & " ORDER BY t.timestamps DESC"

            Using localCmd As New MySqlCommand(localSql, conn)
                If Not String.IsNullOrEmpty(filterNo) Then
                    localCmd.Parameters.AddWithValue("@filterNo", "%" & filterNo & "%")
                End If
                If Not String.IsNullOrEmpty(filterStatus) Then
                    localCmd.Parameters.AddWithValue("@filterStatus", "%" & filterStatus & "%")
                End If
                If useDate Then
                    localCmd.Parameters.AddWithValue("@filterDate", filterDate)
                End If
                If Not String.IsNullOrEmpty(selectedCustomerId) AndAlso 
                   Not String.IsNullOrWhiteSpace(txtSalesRep.Text) AndAlso 
                   Not String.Equals(txtSalesRep.Text.Trim(), "CASH", StringComparison.OrdinalIgnoreCase) Then
                    localCmd.Parameters.AddWithValue("@c_id", selectedCustomerId)
                End If

                Dim da As New MySqlDataAdapter(localCmd)
                Dim dt As New DataTable()
                da.Fill(dt)
                dgvInvoices.DataSource = dt
                dgvInvoices.RowHeadersVisible = False

                 If dgvInvoices.Columns.Contains("id") Then
                    dgvInvoices.Columns("id").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                    dgvInvoices.Columns("id").Width = 80
                End If

                If dgvInvoices.Columns.Contains("Invoice No") Then
                    dgvInvoices.Columns("Invoice No").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                    dgvInvoices.Columns("Invoice No").Width = 100
                End If

                If dgvInvoices.Columns.Contains("Printed No") Then
                    dgvInvoices.Columns("Printed No").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                    dgvInvoices.Columns("Printed No").Width = 100
                End If

                If dgvInvoices.Columns.Contains("Customer Name") Then
                    dgvInvoices.Columns("Customer Name").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                    dgvInvoices.Columns("Customer Name").Width = 150
                End If

                If dgvInvoices.Columns.Contains("B.Type") Then
                    dgvInvoices.Columns("B.Type").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                    dgvInvoices.Columns("B.Type").Width = 90
                End If

                If dgvInvoices.Columns.Contains("Inv Type") Then
                    dgvInvoices.Columns("Inv Type").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                    dgvInvoices.Columns("Inv Type").Width = 90
                End If

                If dgvInvoices.Columns.Contains("Payment") Then
                    dgvInvoices.Columns("Payment").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                    dgvInvoices.Columns("Payment").Width = 90
                End If

                If dgvInvoices.Columns.Contains("Status") Then
                    dgvInvoices.Columns("Status").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                    dgvInvoices.Columns("Status").Width = 130
                End If

                If dgvInvoices.Columns.Contains("Total") Then
                    dgvInvoices.Columns("Total").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                    dgvInvoices.Columns("Total").Width = 100
                End If

                If dgvInvoices.Columns.Contains("Advance") Then
                    dgvInvoices.Columns("Advance").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                    dgvInvoices.Columns("Advance").Width = 100
                End If

                If dgvInvoices.Columns.Contains("Paid") Then
                    dgvInvoices.Columns("Paid").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                    dgvInvoices.Columns("Paid").Width = 100
                End If

                If dgvInvoices.Columns.Contains("Balance") Then
                    dgvInvoices.Columns("Balance").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                    dgvInvoices.Columns("Balance").Width = 100
                End If

                If dgvInvoices.Columns.Contains("Date") Then
                    dgvInvoices.Columns("Date").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                    dgvInvoices.Columns("Date").Width = 160
                End If

            End Using

            If openedHere Then conn.Close()
        Catch ex As Exception
            If openedHere AndAlso conn.State = ConnectionState.Open Then conn.Close()
            MessageBox.Show("Error loading invoices: " & ex.Message)
        End Try
    End Sub

    Private Sub cmbInvSearchType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbInvSearchType.SelectedIndexChanged
        LoadCustomerInvoices()
    End Sub

    Private Sub CalculateTotalCredit()
        If String.IsNullOrEmpty(selectedCustomerId) OrElse String.IsNullOrWhiteSpace(txtSalesRep.Text) Then
            lblAccountOutstanding.Text = "0.00"
            lblWalletValue.Text = "0.00"
            Return
        End If
        Dim openedHere As Boolean = False
        Try
            If conn.State = ConnectionState.Closed Then
                conn.Open()
                openedHere = True
            End If

            ' 1. Load Credit Balance (Summing up active customer credits)
            Dim localSql = "SELECT SUM(amount) FROM customer_credit WHERE customer_id = @c_id AND is_active = 1 " & (If(Module1.IsRgrVisible, "", " AND is_rgr = 0 AND (inv_no IS NULL OR (inv_no NOT LIKE 'GR%' AND inv_no NOT LIKE 'RGR%')) "))
            Using localCmd As New MySqlCommand(localSql, conn)
                localCmd.Parameters.AddWithValue("@c_id", selectedCustomerId)
                Dim total = localCmd.ExecuteScalar()
                lblAccountOutstanding.Text = If(total Is DBNull.Value, "0.00", Convert.ToDecimal(total).ToString("N2"))
            End Using

            ' 2. Load Wallet Balance (Credit Notes)
            Dim walletSql = "SELECT SUM(credit_amount) FROM customer_credit_notes WHERE customer_id = @c_id AND status = 'active' " & (If(Module1.IsRgrVisible, "", " AND is_rgr = 0 AND inv_no NOT LIKE 'GR%' AND inv_no NOT LIKE 'RGR%' "))
            Using walletCmd As New MySqlCommand(walletSql, conn)
                walletCmd.Parameters.AddWithValue("@c_id", selectedCustomerId)
                Dim wTotal = walletCmd.ExecuteScalar()
                lblWalletValue.Text = If(wTotal Is DBNull.Value, "0.00", Convert.ToDecimal(wTotal).ToString("N2"))
            End Using

            If openedHere Then conn.Close()
        Catch ex As Exception
            If openedHere AndAlso conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub btnApplyWallet_Click(sender As Object, e As EventArgs) Handles btnApplyWallet.Click
        Dim walletAmt As Decimal = 0
        Decimal.TryParse(lblWalletValue.Text, walletAmt)

        If walletAmt <= 0 Then
            MessageBox.Show("No wallet balance available for this customer.")
            Return
        End If

        Dim gTotal As Decimal = 0
        Decimal.TryParse(lblGrandTotal.Text, gTotal)

        If gTotal <= 0 Then
            MessageBox.Show("Grand Total must be greater than zero to apply wallet.")
            Return
        End If

        ' Use the smaller of walletAmt or gTotal
        Dim applyAmt As Decimal = Math.Min(walletAmt, gTotal)

        If walletAmt >= gTotal Then
            ' Full payment from wallet -> Goes to Cash textbox
            txtCashAmount.Text = applyAmt.ToString("F2")
            txtAdvPay.Text = ""
            isWalletApplied = True

            MessageBox.Show(applyAmt.ToString("N2") & " from wallet has been applied to this bill.")

            ' Trigger change amount calculation
            txtCashAmount_TextChanged(Nothing, Nothing)
        Else
            ' Partial payment from wallet -> Goes to Adv Pay textbox
            txtAdvPay.Text = applyAmt.ToString("F2")
            txtCashAmount.Text = ""
            isWalletApplied = True

            MessageBox.Show(applyAmt.ToString("N2") & " from wallet has been applied to this bill.")

            ' Trigger change amount calculation
            txtAdvPay_TextChanged(Nothing, Nothing)
        End If
    End Sub

    Private Sub dgvInvoices_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvInvoices.CellClick
        If e.RowIndex >= 0 Then
            Dim row As DataGridViewRow = dgvInvoices.Rows(e.RowIndex)
            Dim bilId As String = row.Cells("id").Value.ToString()
            Dim bType As String = row.Cells("B.Type").Value.ToString()
            Dim bBalance As String = row.Cells("Balance").Value.ToString()
            Dim invTypeCheck As String = row.Cells("Inv Type").Value.ToString()

            Dim srcTable As String = "billing"
            If invTypeCheck = "Quote" OrElse bType = "Quote" OrElse row.Cells("Payment").Value.ToString() = "Quote" Then
                srcTable = "quotation_billing"
            End If

            ' Load the invoice items directly from the consolidated billing tables or quotation
            LoadInvoiceItemsIntoGrid(bilId, row.Cells("Invoice No").Value.ToString(), srcTable)
            InvDetailsPanel.Visible = False

            ' Check eligibility for Mark as Complete
            Dim invDate As DateTime
            Dim isDateValid As Boolean = DateTime.TryParse(row.Cells("Date").Value?.ToString(), invDate)
            Dim invStatus As String = row.Cells("Status").Value?.ToString()

            Dim isEligibleStatus As Boolean = invStatus.Equals("Paid", StringComparison.OrdinalIgnoreCase) OrElse invStatus.Equals("Success", StringComparison.OrdinalIgnoreCase)

            If isDateValid AndAlso isEligibleStatus AndAlso (DateTime.Now - invDate).TotalDays >= 10 Then
                btnCompleteInv.Text = "Mark as Completed"
                btnCompleteInv.Visible = True
            Else
                btnCompleteInv.Visible = False
            End If
        End If
    End Sub

    ' --- Mark Invoice as Completed ---
    Private Sub BtnCompleteInv_Click(sender As Object, e As EventArgs) Handles btnCompleteInv.Click
        ' If in List mode, just go back
        If btnCompleteInv.Text = "Back to Sales" Then
            InvDetailsPanel.Visible = False
            btnCompleteInv.Visible = False
            btnCancelView.Visible = False
            ClearForm()
            Return
        End If

        If dgvInvoices.CurrentRow Is Nothing OrElse dgvInvoices.CurrentRow.Index < 0 Then
            MessageBox.Show("Please select an invoice first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim invId As String = dgvInvoices.CurrentRow.Cells("id").Value.ToString()
        Dim invNo As String = dgvInvoices.CurrentRow.Cells("Invoice No").Value.ToString()
        Dim currentStatus As String = dgvInvoices.CurrentRow.Cells("Status").Value.ToString()

        If String.Equals(currentStatus, "completed", StringComparison.OrdinalIgnoreCase) Then
            MessageBox.Show("Invoice " & invNo & " is already completed.", "Already Completed", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim confirm = MessageBox.Show("Mark Invoice " & invNo & " as Completed?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If confirm = DialogResult.Yes Then
            Dim openedHere As Boolean = False
            Try
                If conn.State = ConnectionState.Closed Then
                    conn.Open()
                    openedHere = True
                End If
                Dim sqlUpdate As String = "UPDATE billing SET status='completed' WHERE id=@id"
                Dim cmdUpdate As New MySqlCommand(sqlUpdate, conn)
                cmdUpdate.Parameters.AddWithValue("@id", invId)
                cmdUpdate.ExecuteNonQuery()
                If openedHere Then conn.Close()
                MessageBox.Show("Invoice " & invNo & " marked as Completed!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                ' Return DIRECTLY to Main Sales Page
                InvDetailsPanel.Visible = False
                btnCompleteInv.Visible = False
                btnCancelView.Visible = False
                isEditingHistory = False
                loadedHistoryInvNo = ""

                ' Clear the entire form and return to fresh state
                ClearForm()
            Catch ex As Exception
                If openedHere AndAlso conn.State = ConnectionState.Open Then conn.Close()
                MessageBox.Show("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Private Sub BtnCancelView_Click(sender As Object, e As EventArgs) Handles btnCancelView.Click
        ' Return to Invoice History List (Instead of clearing everything)
        LoadCustomerInvoices()
        InvDetailsPanel.Visible = True
        InvDetailsPanel.BringToFront()

        ' Hide details specific buttons
        btnCancelView.Visible = False
        btnCompleteInv.Text = "Back to Sales"

        ' Clear the temporary invoice load from main grid
        dtBill.Rows.Clear()
        DataGridView2.DataSource = dtBill
        isEditingHistory = False
        loadedHistoryInvNo = ""

        ' Reset labels
        ClearEntryFields()
        CalculateGrandTotal()

        ' Reset billing totals, cash, and VAT state after cancelling invoice view
        lblTotalAmount.Text = "0.00"
        lblGrandTotal.Text = "0.00"
        txtCashAmount.Text = ""
        txtAdvPay.Text = ""
        txtChangeAmount.Text = ""
        txtOurDiscount.Text = "0.00"
        txtInvDiscount.Text = "0.00"
        lblBalance.Text = "0.00"
        lblVatBalance.Text = "0.00"
        lblCreditBalance.Text = "0.00"
        CheckBoxIsVat.Checked = False
        CheckBoxIsVat.Enabled = True
        CheckBoxWholesale.Enabled = True
        CheckBoxRetail.Enabled = True
        btnCancelView.Visible = False

        ComboBoxTotalVat.SelectedIndex = 0
        txtSalesRep.Text = ""
        txtCustomerAddress.Text = ""

        ' Ensure everything else is reset for a fresh state
        ClearForm()
        txtCustomerPhone.Text = ""

        ' Reload fresh blank invoice number
        LoadInvoiceNumber()

        ' Hide self and complete button since we are exiting Inv Details flow
        btnCompleteInv.Visible = False
        InvDetailsPanel.Visible = False
    End Sub

    ''' <summary>
    ''' Public method to load an existing invoice for editing from other forms (e.g., Sales history).
    ''' </summary>
    Public Sub LoadInvoiceForEditing(billingId As String, invNo As String)
        Try
            ' 1. Clear current form state
            btnCancelView.PerformClick()

            ' 2. Load the items
            LoadInvoiceItemsIntoGrid(billingId, invNo)

            ' 3. Additional UI Prep for Editing
            isEditingHistory = True ' Explicitly ensure this is set (LoadInvoiceItemsIntoGrid usually sets it)

            ' 4. Hide unnecessary panels that might interfere
            InvDetailsPanel.Visible = False
            btnCompleteInv.Visible = False

            ' 5. Focus the form if it was in background
            Me.BringToFront()

        Catch ex As Exception
            MessageBox.Show("Error loading invoice for editing: " & ex.Message)
        End Try
    End Sub

    Public Sub LoadInvoiceItemsIntoGrid(billingId As String, invNo As String, Optional sourceTable As String = "billing")
        isProcessingLoad = True ' Start protection
        Dim openedHere As Boolean = False
        Try
            If conn.State = ConnectionState.Closed Then
                conn.Open()
                openedHere = True
            End If

            ' Consolidated: always query the main billing and billing_item tables
            Dim mainTable As String = sourceTable
            Dim itemTable As String = If(sourceTable = "quotation_billing", "quotation_billing_item", "billing_item")

            ' 1. Fetch Master Bill Data
            Dim bType As String = "Cash"
            Dim c_id As String = ""
            Dim u_id As String = ""
            Dim p_amt As Decimal = 0
            Dim restored_vat_id As Integer = 0
            Dim restored_vat_label As String = "None"

            Dim masterSql As String = "SELECT * FROM " & mainTable & " WHERE id = @id"
            Using masterCmd As New MySqlCommand(masterSql, conn)
                masterCmd.Parameters.AddWithValue("@id", billingId)
                Using mdr = masterCmd.ExecuteReader()
                    If mdr.Read() Then
                        isEditingHistory = True
                        loadedHistoryInvNo = mdr("inv_no").ToString()
                        loadedHistoryDate = If(mdr("timestamps") Is DBNull.Value, DateTime.Now, Convert.ToDateTime(mdr("timestamps")))
                        lblInvoiceNumber.Text = loadedHistoryInvNo

                        bType = mdr("billing_type").ToString()
                        cmbBillingType.Text = bType
                        txtPaymentMethod.Text = mdr("payment_type").ToString()
                        Dim rawPaid As Decimal = If(mdr("paid_amount") Is DBNull.Value, 0, Convert.ToDecimal(mdr("paid_amount")))
                        Dim rawReceived As Decimal = If(mdr("cash_received") Is DBNull.Value, 0, Convert.ToDecimal(mdr("cash_received")))
                        Dim rawAdv As Decimal = If(mdr("advance_payment") Is DBNull.Value, 0, Convert.ToDecimal(mdr("advance_payment")))
                        Dim balanceDue As Decimal = If(mdr("balance_due") Is DBNull.Value, 0, Convert.ToDecimal(mdr("balance_due")))
                        Dim gTotalLoaded As Decimal = If(mdr("grand_total") Is DBNull.Value, 0, Convert.ToDecimal(mdr("grand_total")))

                        ' If change was given (balance_due < 0), the effective amount we kept is grand_total.
                        ' This ensures that returns calculate change based on the kept amount, not the physical cash handed over.
                        p_amt = If(balanceDue < 0, gTotalLoaded, rawPaid)

                        txtCashAmount.Text = rawReceived.ToString("N2")
                        txtOurDiscount.Text = If(mdr("our_discount") Is DBNull.Value, "0.00", Convert.ToDecimal(mdr("our_discount")).ToString("N2"))
                        txtInvDiscount.Text = If(mdr("inv_discount") Is DBNull.Value, "0.00", Convert.ToDecimal(mdr("inv_discount")).ToString("N2"))

                        Dim rawAdvPayAmt As Decimal = 0
                        Try
                            If mdr("adv_pay_amount") IsNot DBNull.Value Then
                                rawAdvPayAmt = Convert.ToDecimal(mdr("adv_pay_amount"))
                            End If
                        Catch
                        End Try

                        If rawAdvPayAmt > 0 Then
                            txtAdvPay.Text = rawAdvPayAmt.ToString("N2")
                        Else
                            txtAdvPay.Text = ""
                        End If

                        c_id = mdr("customer_id").ToString()
                        u_id = mdr("user_id").ToString()

                        ' Capture Original State for Warnings
                        originalBillingType = bType
                        originalPaymentMethod = mdr("payment_type").ToString()
                        originalStatusValue = mdr("status").ToString()
                        Try
                            originalChequeNo = If(mdr("cheque_no") Is DBNull.Value, "", mdr("cheque_no").ToString())
                            originalBankId = If(mdr("bank_id") Is DBNull.Value, "", mdr("bank_id").ToString())
                            originalChequeAmt = If(mdr("cheque_balance_due") Is DBNull.Value, 0D, Convert.ToDecimal(mdr("cheque_balance_due")))
                        Catch ex As Exception
                            originalChequeNo = ""
                            originalBankId = ""
                            originalChequeAmt = 0D
                        End Try
                        ' Handle cheque date if available (assuming it might be in timestamps or a dedicated column)
                        ' Mapping to timestamps as a reasonable fallback for the issued date
                        originalChequeDate = If(mdr("timestamps") Is DBNull.Value, DateTime.MinValue, Convert.ToDateTime(mdr("timestamps")))

                        ' Restore Wholesale / Retail flags
                        Dim iType As String = If(mdr("inv_type") Is DBNull.Value, "Normal", mdr("inv_type").ToString())
                        Dim finalType As String = If(iType <> "Normal", iType, "Normal")

                        CheckBoxWholesale.Checked = String.Equals(finalType, "Wholesale", StringComparison.OrdinalIgnoreCase)
                        CheckBoxRetail.Checked = String.Equals(finalType, "Retail", StringComparison.OrdinalIgnoreCase)


                        ' Restore VAT flag
                        Dim v_id = mdr("vat_id")
                        If v_id IsNot DBNull.Value AndAlso v_id IsNot Nothing Then
                            restored_vat_id = Convert.ToInt32(v_id)
                            CheckBoxIsVat.Checked = (restored_vat_id <> 1)
                        Else
                            restored_vat_id = 1
                            CheckBoxIsVat.Checked = False
                            restored_vat_label = "None"
                        End If

                        ' Restore PO Number
                        Try
                            Dim ord = mdr.GetOrdinal("po_number")
                            If Not mdr.IsDBNull(ord) Then
                                TextBoxPO.Text = mdr(ord).ToString()
                            End If
                        Catch
                            ' Column may not exist if migration hasn't been run
                        End Try

                        ' Restore Customer VAT ID
                        Try
                            Dim ordVat = mdr.GetOrdinal("cus_vat_id")
                            If Not mdr.IsDBNull(ordVat) Then
                                txtCusVatId.Text = mdr(ordVat).ToString()
                            Else
                                txtCusVatId.Text = ""
                            End If
                        Catch
                            txtCusVatId.Text = ""
                        End Try

                        ' Restore Print as Retail flag
                        Try
                            Dim ordPrintRetail = mdr.GetOrdinal("print_as_retail")
                            If Not mdr.IsDBNull(ordPrintRetail) Then
                                CheckBoxPrintAsRetail.Checked = Convert.ToBoolean(mdr(ordPrintRetail))
                            Else
                                CheckBoxPrintAsRetail.Checked = False
                            End If
                        Catch
                            CheckBoxPrintAsRetail.Checked = False
                        End Try

                        ' Restore RGR (Hidden Bill) state
                        Dim loadedIsRgr As Boolean = False
                        Try
                            Dim ordRgr = mdr.GetOrdinal("is_rgr")
                            If Not mdr.IsDBNull(ordRgr) Then
                                loadedIsRgr = Convert.ToBoolean(mdr(ordRgr))
                            End If
                        Catch ex As Exception
                        End Try

                        If btnSaveRGR IsNot Nothing Then
                            btnSaveRGR.Visible = loadedIsRgr
                        End If

                        ' Disable editing mode toggles during history/return mode for actual bills
                        If sourceTable = "quotation_billing" Then
                            CheckBoxWholesale.Enabled = True
                            CheckBoxRetail.Enabled = True
                            CheckBoxIsVat.Enabled = True
                        Else
                            CheckBoxWholesale.Enabled = True
                            CheckBoxRetail.Enabled = True
                            CheckBoxIsVat.Enabled = True
                        End If
                    End If
                End Using
            End Using

            ' Fetch and set the VAT ComboBox based on restored_vat_id
            If restored_vat_id > 0 Then
                Try
                    Using v_cmd As New MySqlCommand("SELECT vat_name, vat_value FROM vat WHERE id=@vid", conn)
                        v_cmd.Parameters.AddWithValue("@vid", restored_vat_id)
                        Using v_dr = v_cmd.ExecuteReader()
                            If v_dr.Read() Then
                                restored_vat_label = v_dr("vat_name").ToString() & " (" & v_dr("vat_value").ToString() & "%)"
                                ComboBoxTotalVat.SelectedValue = restored_vat_id
                            End If
                        End Using
                    End Using
                Catch ex As Exception
                End Try
            Else
                ComboBoxTotalVat.SelectedIndex = 0
            End If

            ' Cashier is intentionally kept as the current logged-in user for returns
            ' (Requested by user: cmbCashier should always be the logged-in user)
            If Module1.CurrentUserID > 0 Then
                cmbCashier.SelectedValue = Module1.CurrentUserID
            End If

            ' Restore Customer info (Bypass for sellers to keep current session customer, but allow cashiers to see it)
            Dim fRole As String = If(Module1.FinancialRole IsNot Nothing, Module1.FinancialRole.ToLower(), "")
            Dim isSellerRestrict As Boolean = (fRole = "seller" OrElse fRole.Contains("order")) AndAlso Not (Module1.UserRole IsNot Nothing AndAlso Module1.UserRole.ToLower() = "cashier")

            If Not isSellerRestrict AndAlso Not String.IsNullOrEmpty(c_id) Then
                FetchCustomerData("id", c_id)
            End If

            ' Re-open connection if closed by FetchCustomerData
            If conn.State = ConnectionState.Closed Then conn.Open()

            ' 2. Fetch Items
            Dim itemsSql = "SELECT item_id as 'Item ID', description as 'Description', quantity as 'Qty', unit_price as 'Price', item_amount, discount as 'Dis', item_cost as 'ItemCost', location as 'Location', 1 as 'LocationID', IFNULL(print_retail_price, 0) as 'PrintRetailPrice' " &
                          "FROM " & itemTable & " WHERE billing_id = @id"
            Using itemCmd = New MySqlCommand(itemsSql, conn)
                itemCmd.Parameters.AddWithValue("@id", billingId)
                Using dr = itemCmd.ExecuteReader()
                    dtBill.Rows.Clear()
                    While dr.Read()
                        Dim qtyVal As Decimal = If(dr("Qty") Is DBNull.Value, 0, Convert.ToDecimal(dr("Qty")))
                        Dim priceVal As Decimal = If(dr("Price") Is DBNull.Value, 0, Convert.ToDecimal(dr("Price")))
                        Dim discVal As Decimal = If(dr("Dis") Is DBNull.Value, 0, Convert.ToDecimal(dr("Dis")))
                        Dim locationVal As String = If(dr("Location") Is DBNull.Value, "MAIN STOCK", dr("Location").ToString())

                        ' Use database value if exists, otherwise fallback to calculation
                        Dim lineTotal As Decimal = If(dr("item_amount") Is DBNull.Value, (priceVal - (priceVal * discVal / 100)) * qtyVal, Convert.ToDecimal(dr("item_amount")))

                        ' Add to dtBill matching the schema (Item ID, Description, Qty, Selling Price, Dis, Location, Total/Amount, VAT, ItemCost, AvgCost, LocationID, Reason, IsOriginal)
                        Dim itAvgCost As Decimal = 0
                        Dim locId As Long = If(dr("LocationID") Is DBNull.Value, 1, Convert.ToInt64(dr("LocationID")))
                        Dim printRetailVal As Decimal = If(dr("PrintRetailPrice") Is DBNull.Value, 0, Convert.ToDecimal(dr("PrintRetailPrice")))
                        dtBill.Rows.Add("", dr("Item ID"), dr("Description"), qtyVal, priceVal, discVal, locationVal, lineTotal, restored_vat_label, dr("ItemCost"), itAvgCost, locId, "", True, printRetailVal)
                    End While
                End Using
            End Using
            If openedHere Then conn.Close()

            DataGridView2.DataSource = dtBill
            CalculateGrandTotal()

            ' Sync UI for Credit/Cash balance display
            UpdatePaymentAndBalance()

            ' Always load account total if customer is present
            CalculateTotalCredit()
            ' lblCreditBalance will be handled by UpdatePaymentAndBalance() called above

            lblInvoiceNumber.Text = invNo

            ' Now that readers are closed, sync UI states that trigger events
            isElBill = loadedHistoryInvNo.StartsWith("EL")
            If sourceTable = "quotation_billing" Then
                ComboBox1.SelectedIndex = 1
            Else
                ComboBox1.SelectedIndex = 0
            End If
            isProcessingLoad = False ' End protection

            ' Show Cancel View button when invoice is loaded
            btnCancelView.Visible = True
            btnCancelView.BringToFront()

            ' Show Return controls when loading a historical invoice
            cmbReturnReason.Visible = True
            Label5.Visible = True
            lblSupplierInfo.Visible = True
            If cmbReturnReason.Items.Count > 0 Then
                isRestoringReason = True
                cmbReturnReason.SelectedIndex = 0 ' "None" or first option
                isRestoringReason = False
            End If

            ' Restrict Editing for Completed Invoices
            If String.Equals(originalStatusValue, "completed", StringComparison.OrdinalIgnoreCase) Then
                btnUpdate.Enabled = False
                btnSave.Enabled = False
                btnDelete.Enabled = False
                btnAddNew.Enabled = False
                MessageBox.Show("This invoice is marked as Completed. You can view the details but cannot make further edits or returns.", "View Only", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                btnUpdate.Enabled = True
                btnSave.Enabled = True
                btnDelete.Enabled = True
                btnAddNew.Enabled = True
            End If

            ' Re-apply Role Based UI to ensure PENDING restriction for sellers after loading history
            ApplyRoleBasedUI()

        Catch ex As Exception
            If openedHere AndAlso conn.State = ConnectionState.Open Then conn.Close()
            isProcessingLoad = False ' Ensure flag is reset on error
            MessageBox.Show("Error loading invoice items: " & ex.Message)
        End Try
    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        ' Try to identify by password first
        IdentifyCashierByPassword()

        ' Check if the active cashier is admin1/2/3/4
        Dim currentCashier As String = cmbCashier.Text.Trim().ToLower()
        If currentCashier = "admin1" OrElse currentCashier = "admin2" OrElse currentCashier = "admin3" OrElse currentCashier = "admin4" Then
            MessageBox.Show("Please enter a valid Cashier ID", "Invalid Cashier", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtCashierID.Focus()
            txtCashierID.SelectAll()
            Return
        End If

        ' Prefer current grid selection; fall back to stored index
        If DataGridView2.CurrentRow IsNot Nothing Then
            Dim rowView As DataRowView = DirectCast(DataGridView2.CurrentRow.DataBoundItem, DataRowView)
            selectedIndex = dtBill.Rows.IndexOf(rowView.Row)
        End If

        If selectedIndex < 0 OrElse selectedIndex >= dtBill.Rows.Count Then
            MessageBox.Show("Please select a row in the grid to edit.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        ' Prevent updating if fields have been cleared (e.g. clicking Edit twice)
        If String.IsNullOrWhiteSpace(txtItemID.Text) Then
            MessageBox.Show("Please select an item to edit.", "No Item Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If Not ItemExistsInDatabase(txtItemID.Text.Trim()) Then
            MessageBox.Show("The Item ID '" & txtItemID.Text.Trim() & "' does not exist in the database. Please select a valid item.", "Item Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtItemID.Focus()
            txtItemID.SelectAll()
            Return
        End If
        ' In history/return mode, a reason is mandatory ONLY if we are returning an original item (btnUpdate.Text = "Return")
        If btnUpdate.Text = "Return" AndAlso cmbReturnReason.Visible Then
            Dim currentQtyVal As Decimal = 0
            Decimal.TryParse(txtQuantity.Text, currentQtyVal)

            Dim isReturnAction As Boolean = False
            If currentQtyVal <> 0 Then
                isReturnAction = True
            End If

            If isReturnAction Then
                Dim reasonText As String = If(cmbReturnReason.Text, "").Trim()
                If cmbReturnReason.SelectedIndex <= 0 OrElse String.Equals(reasonText, "None", StringComparison.OrdinalIgnoreCase) Then
                    MessageBox.Show("Please select a Return Reason before updating a return item in this invoice.", "Return Reason Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    cmbReturnReason.Focus()
                    cmbReturnReason.DroppedDown = True
                    Return
                End If


            End If
        End If

        ' Nested Validation for Cashier
        If cmbCashier.SelectedIndex = -1 OrElse String.IsNullOrEmpty(cmbCashier.Text) Then
            MessageBox.Show("Please enter a valid Cashier ID", "Cashier Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            cmbCashier.Focus()
        Else
If String.IsNullOrEmpty(txtCashierID.Text.Trim()) Then
                MessageBox.Show("Please enter a valid Cashier ID", "Password Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtCashierID.Focus()
            Else
                ' Verify Password
                Dim rowCashier As DataRowView = DirectCast(cmbCashier.SelectedItem, DataRowView)
                If rowCashier("password").ToString() <> txtCashierID.Text.Trim() Then
                    MessageBox.Show("Please enter a valid Cashier ID", "Unauthorized", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    txtCashierID.Focus()
                    txtCashierID.SelectAll()
                Else
                    Dim qty As Decimal = 0
                    Decimal.TryParse(txtQuantity.Text, qty)
                    Dim disc As Decimal = 0
                    Decimal.TryParse(txtDiscount.Text, disc)

                    ' WHOLESALE STOCK ALERT VALIDATION (5 QTY LIMIT)
                    If qty > 0 AndAlso CheckBoxWholesale.Checked Then
                        Dim wholesaleLoc As String = If(ComboBoxLocation.Text IsNot Nothing, ComboBoxLocation.Text.Trim(), "MAIN STOCK")
                        If String.IsNullOrEmpty(wholesaleLoc) Then wholesaleLoc = "MAIN STOCK"
                        
                        Dim currentStock As Decimal = GetCurrentStock(txtItemID.Text.Trim(), wholesaleLoc)
                        If currentStock > 0 Then
                            If currentStock < 5 Then
                                Dim result As DialogResult = MessageBox.Show("ප්‍රමාණවත් ප්‍රමාණයක් නොමැත. එසේ වුවද save කිරීමට අවශ්‍යද? (අයිතමය: " & txtItemID.Text.Trim() & ")", "තොග ප්‍රමාණවත් නොවේ", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                                If result = DialogResult.No Then
                                    txtQuantity.Focus()
                                    txtQuantity.SelectAll()
                                    Return
                                End If
                            Else
                                Dim alreadyInBill As Decimal = GetTotalQtyInBill(txtItemID.Text.Trim(), selectedIndex)
                                Dim maxAllowed As Decimal = currentStock - 5
                                Dim maxAllowedForThisLine As Decimal = maxAllowed - alreadyInBill

                                If maxAllowedForThisLine < 0 Then
                                    Dim result As DialogResult = MessageBox.Show("මෙම අයිතමයෙන් ඔබට ලබාගත හැක්කේ 0 ක් පමණි. එසේ වුවද save කිරීමට අවශ්‍යද? (අයිතමය: " & txtItemID.Text.Trim() & ")", "සීමාව ඉක්මවා ඇත", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                                    If result = DialogResult.No Then
                                        txtQuantity.Text = "0"
                                        txtQuantity.Focus()
                                        txtQuantity.SelectAll()
                                        Return
                                    End If
                                ElseIf qty > maxAllowedForThisLine Then
                                    Dim result As DialogResult = MessageBox.Show("මෙම අයිතමයෙන් ඔබට ලබාගත හැක්කේ " & maxAllowedForThisLine.ToString("G") & " ක් පමණි. එසේ වුවද save කිරීමට අවශ්‍යද? (අයිතමය: " & txtItemID.Text.Trim() & ")", "සීමාව ඉක්මවා ඇත", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                                    If result = DialogResult.No Then
                                        txtQuantity.Text = maxAllowedForThisLine.ToString("G")
                                        txtQuantity.Focus()
                                        txtQuantity.SelectAll()
                                        Return
                                    End If
                                End If
                            End If
                        End If
                    End If

                    ' Mandatory Return Reason logic here was removed as it's handled above and we only want it for "Return" items.
                    ' Keep qty<0 validation if you still want it globally for negative lines, but usually the "Return" button logic covers it.
                    If qty <> 0 AndAlso btnUpdate.Text = "Return" Then
                        Dim reasonText As String = If(cmbReturnReason.Text, "").Trim()
                        If cmbReturnReason.SelectedIndex <= 0 OrElse String.Equals(reasonText, "None", StringComparison.OrdinalIgnoreCase) Then
                            MessageBox.Show("Please select a Return Reason for the item being returned.", "Reason Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            cmbReturnReason.Focus()
                            cmbReturnReason.DroppedDown = True
                            Return
                        End If
                    End If

                    ' --- 7 DAY RETURN POLICY VALIDATION ---
                    ' If isEditingHistory AndAlso loadedHistoryDate <> DateTime.MinValue Then
                    ' Check if we are reducing quantity (Return/Exchange)
                    ' Dim rowCheck As DataRow = dtBill.Rows(selectedIndex)
                    ' Dim oldQtyCheck As Decimal = 0
                    ' Decimal.TryParse(rowCheck("Qty").ToString(), oldQtyCheck)

                    ' If qty > 0 AndAlso (DateTime.Now - loadedHistoryDate).TotalDays > 7 Then
                    ' When editing history, 'qty' is the amount to SUBTRACT from original. 
                    ' If user enters a value to subtract, it means they are returning.
                    ' MessageBox.Show("This invoice was issued on " & loadedHistoryDate.ToString("yyyy-MM-dd") & "." & vbCrLf &
                    ' "Returns or exchanges are only allowed within 7 days of the invoice date.",
                    ' "Return Policy Restriction", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    ' Return
                    ' End If
                    ' End If
                    Dim unitPrice As Decimal = 0
                    Decimal.TryParse(txtSellingPrice1.Text, unitPrice)

                    Dim printRetailVal As Decimal = unitPrice
                    If CheckBoxWholesale.Checked AndAlso CheckBoxPrintAsRetail.Checked Then
                        Dim wholesalePrice As Decimal = GetItemWholesalePrice(txtItemID.Text.Trim())
                        If wholesalePrice > 0 Then
                            unitPrice = wholesalePrice
                        End If
                        If printRetailVal = wholesalePrice Then
                            Dim retailPrice As Decimal = GetItemRetailPrice(txtItemID.Text.Trim())
                            If retailPrice > 0 Then
                                printRetailVal = retailPrice
                            End If
                        End If
                    End If

                    ' VAT EXTRACTION LOGIC
                    Dim vatPercent As Decimal = 0
                    Dim vatLabel As String = ComboBoxVat.Text
                    Dim currentVatId As Integer = If(ComboBoxVat.SelectedValue IsNot Nothing, Convert.ToInt32(ComboBoxVat.SelectedValue), 1)

                    If currentVatId <> 1 Then
                        If ComboBoxVat.SelectedItem IsNot Nothing Then
                            Dim rowVat As DataRowView = DirectCast(ComboBoxVat.SelectedItem, DataRowView)
                            Decimal.TryParse(rowVat("vat_value").ToString(), vatPercent)
                        End If
                    End If

                    ' Base unit price calculation (Extract VAT if necessary)
                    Dim baseItemPrice As Decimal = unitPrice
                    If CheckBoxIsVat.Checked AndAlso vatPercent > 0 Then
                        baseItemPrice = unitPrice / (1 + vatPercent / 100)
                    End If

                    Dim discountedPricePerUnit As Decimal = baseItemPrice * (1 - disc / 100)
                    Dim lineTotal As Decimal = discountedPricePerUnit * qty

                    Dim row As DataRow = dtBill.Rows(selectedIndex)
                    Dim itemCost As String = If(txtSellingPrice1.Tag IsNot Nothing, txtSellingPrice1.Tag.ToString(), "0")
                    Dim itemCostVal As Decimal = 0
                    Decimal.TryParse(itemCost, itemCostVal)

                    ' Validate profit margin (Compare against AvgCost)
                    Dim avgCostVal As Decimal = 0
                    If dtBill.Columns.Contains("AvgCost") Then
                        Decimal.TryParse(row("AvgCost").ToString(), avgCostVal)
                    End If

                    If avgCostVal > 0 AndAlso discountedPricePerUnit <= avgCostVal Then
                        MessageBox.Show(
                                "Total amount is low. Sale price (" & discountedPricePerUnit.ToString("N2") & ") must be higher than average item cost (" & avgCostVal.ToString("N2") & ") to ensure profit." & vbCrLf &
                                "Please reduce the discount or increase the price.",
                                "Low Profit Margin", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtSellingPrice1.Focus()
                        txtSellingPrice1.SelectAll()
                        Return
                    End If
                    Dim finalQty As Decimal = 0

                    If isEditingHistory Then
                        Dim currentQty As Decimal = 0
                        Decimal.TryParse(row("Qty").ToString(), currentQty)
                        finalQty = currentQty - qty
                    Else
                        finalQty = qty
                    End If

                    ' Handle Pricing and Discount
                    ' We use 'baseItemPrice' as the base unit price (selling price)
                    ' and 'discountedPricePerUnit' as the final price after discount
                    Dim rowTotal As Decimal = discountedPricePerUnit * finalQty

                    row("Qty") = finalQty
                    row("Selling Price") = unitPrice ' Original Selling Price entered
                    row("Dis") = disc
                    row("Location") = If(ComboBoxLocation.Text IsNot Nothing, ComboBoxLocation.Text.Trim(), "MAIN STOCK")
                    row("PrintRetailPrice") = printRetailVal
                    If String.IsNullOrEmpty(row("Location").ToString()) Then row("Location") = "MAIN STOCK"
                    row("Total/Amount") = rowTotal
                    row("VAT") = If(vatPercent > 0, vatLabel, "")
                    row("ItemCost") = itemCost

                    ' Save Reason per item
                    If isEditingHistory Then
                        Dim finalReason As String = cmbReturnReason.Text
                        If String.Equals(finalReason, "Other", StringComparison.OrdinalIgnoreCase) Then
                            If Not String.IsNullOrEmpty(txtOtherReason.Text) Then
                                finalReason = "Other- " & txtOtherReason.Text.Trim()
                            Else
                                Using dlg As New ReturnReasonDialog()
                                    If dlg.ShowDialog() = DialogResult.OK Then
                                        Dim stockTag As String = If(dlg.AddToStock, "[AddStock]", "[NoStock]")
                                        finalReason = "Other- " & stockTag & " " & dlg.ReturnReason
                                    Else
                                        Return ' Cancel the update
                                    End If
                                End Using
                            End If
                        End If
                        row("Reason") = finalReason
                    Else
                        row("Reason") = ""
                    End If

                    CalculateGrandTotal()
                    ClearEntryFields()
                End If
            End If
        End If
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        ' Prefer current grid selection; fall back to stored index
        If DataGridView2.CurrentRow IsNot Nothing Then
            Dim rowView As DataRowView = DirectCast(DataGridView2.CurrentRow.DataBoundItem, DataRowView)
            selectedIndex = dtBill.Rows.IndexOf(rowView.Row)
        End If

        If selectedIndex = -1 Then
            MessageBox.Show("Please select a row in the grid to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            ' Confirm delete directly without cashier validation
            If MessageBox.Show("Are you sure you want to remove this item from the bill?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                dtBill.Rows.RemoveAt(selectedIndex)
                CalculateGrandTotal()
                ' LIVE AUTO-SAVE
                If SlotID > 0 Then SaveSlotState(SlotID)
                ClearEntryFields()
            End If
        End If
    End Sub

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        If MessageBox.Show("Are you sure you want to clear the entire bill draft?", "Confirm Clear", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            ClearFormKeepSlot()
            CalculateGrandTotal()
            ApplyRoleBasedUI()
            If SlotID > 0 Then SaveSlotState(SlotID)
            MessageBox.Show("Bill draft cleared successfully.", "Cleared", MessageBoxButtons.OK, MessageBoxIcon.Information)
            txtItemID.Focus()
        End If
    End Sub

    Private Sub btnAddNew_Click(sender As Object, e As EventArgs) Handles btnAddNew.Click
        ' Start a fresh line: clear fields and deselect any current row
        ClearEntryFields()
        txtItemID.Focus()
    End Sub

    Private Sub ClearEntryFields()
        Dim startForm = Application.OpenForms.OfType(Of Start)().FirstOrDefault()
        If startForm IsNot Nothing AndAlso startForm.txtRgrPass IsNot Nothing Then
            If startForm.txtRgrPass.Text = "2233" Then
                startForm.txtRgrPass.Text = ""
            End If
        End If

        txtItemID.Text = ""
        txtSellingPrice1.Text = ""
        txtDescription.Text = ""
        txtAmount.Text = ""
        txtQuantity.Text = ""
        txtSellingPrice1.Tag = "0.00"
        txtDiscount.Text = "0"
        txtItemDiscountVal.Text = "0.00"
        lblCurrentStock.Text = "0"
        selectedIndex = -1
        DataGridView2.ClearSelection()
        btnUpdate.Text = "Edit"
        btnUpdate.BackColor = Color.White

        ComboBoxVat.SelectedValue = 1

        ' Reset Qty Label
        For Each ctrl As Control In GroupBox1.Controls
            If ctrl.Name = "Label27" Then
                ctrl.Text = "Qyt"
                Exit For
            End If
        Next

        ' Reset Return Reason controls
        isRestoringReason = True
        cmbReturnReason.SelectedIndex = 0
        isRestoringReason = False
        txtOtherReason.Text = ""
        txtOtherReason.Visible = False

        dgvSearch.Visible = False
        txtItemID.Focus()
    End Sub
    ' Grid editing is disabled (ReadOnly = True). Row click fetches data to entry fields for Update/Delete.

    Private Sub RenumberRows()
        If dtBill Is Nothing OrElse Not dtBill.Columns.Contains("No") Then Return
        For i As Integer = 0 To dtBill.Rows.Count - 1
            dtBill.Rows(i)("No") = (i + 1).ToString()
        Next
    End Sub

    Private Sub CalculateGrandTotal()
        If isFormLoading Then Return ' Guard against calculations during form load
        RenumberRows() ' Auto-update row numbers
        Dim subTotal As Decimal = 0
        For Each row As DataRow In dtBill.Rows
            Dim lineTotal As Decimal = 0
            Dim colName As String = If(dtBill.Columns.Contains("Total/Amount"), "Total/Amount", "Total")
            Decimal.TryParse(row(colName).ToString(), lineTotal)
            subTotal += lineTotal
        Next

        ' Store raw subtotal for DB persistence
        lblTotalAmount.Tag = subTotal

        ' Balance label (Green) - Internal Item Sum
        lblBalance.Text = subTotal.ToString("N2")

        ' VAT Calculation (Dynamic from ComboBoxTotalVat)
        Dim vatAmount As Decimal = 0
        Dim isAnyVatItem As Boolean = False
        For Each row As DataRow In dtBill.Rows
            If row("VAT") IsNot Nothing AndAlso Not String.IsNullOrEmpty(row("VAT").ToString()) Then
                isAnyVatItem = True
                Exit For
            End If
        Next

        ' Show VAT controls if bill is VAT type or has VAT items
        Dim showVat = CheckBoxIsVat.Checked Or isAnyVatItem
        ' If showVat And Not CheckBoxIsVat.Checked Then CheckBoxIsVat.Checked = True

        Dim currentVatRate As Decimal = 0
        Dim currentVatId As Integer = 1

        If CheckBoxIsVat.Checked Then
            Dim selectedVatText As String = ""
            If ComboBoxTotalVat.SelectedItem IsNot Nothing Then
                selectedVatText = ComboBoxTotalVat.Text
                Dim rowTotalVat As DataRowView = DirectCast(ComboBoxTotalVat.SelectedItem, DataRowView)
                Decimal.TryParse(rowTotalVat("vat_value").ToString(), currentVatRate)
                Integer.TryParse(rowTotalVat("id").ToString(), currentVatId)
            End If

            ' If ID is 1 (No VAT), we don't force uncheck anymore to allow manual override
            If currentVatId = 1 Then
                ' CheckBoxIsVat.Checked = False
                vatAmount = 0
            Else
                ' Calculate VAT only for items that have the matching VAT label
                Dim vatableSubTotal As Decimal = 0
                For Each row As DataRow In dtBill.Rows
                    If row("VAT") IsNot Nothing AndAlso row("VAT").ToString() = selectedVatText Then
                        Dim lineTotal As Decimal = 0
                        Dim colName As String = If(dtBill.Columns.Contains("Total/Amount"), "Total/Amount", "Total")
                        Decimal.TryParse(row(colName).ToString(), lineTotal)
                        vatableSubTotal += lineTotal
                    End If
                Next

                vatAmount = vatableSubTotal * currentVatRate / 100
            End If
        End If

        ' VAT Balance = SubTotal * (1 + VAT/100)
        Dim calculatedVatBalance As Decimal = subTotal + vatAmount
        lblVatBalance.Text = vatAmount.ToString("N2")

        ' Total Amount (Cyan box)
        If CheckBoxIsVat.Checked Then
            lblTotalAmount.Text = calculatedVatBalance.ToString("N2")
        Else
            lblTotalAmount.Text = subTotal.ToString("N2")
        End If

        ' Apply Global Discounts to Total Amount
        Dim ourDisc As Decimal = 0
        Dim invDisc As Decimal = 0
        Decimal.TryParse(txtOurDiscount.Text, ourDisc)
        Decimal.TryParse(txtInvDiscount.Text, invDisc)

        Dim totalForDiscount As Decimal = 0
        Decimal.TryParse(lblTotalAmount.Text, totalForDiscount)
        Dim gTotal As Decimal = totalForDiscount - (totalForDiscount * ourDisc / 100) - (totalForDiscount * invDisc / 100)
        lblGrandTotal.Text = gTotal.ToString("N2")

        ' Show/Hide VAT Balance labels and combo
        lblVatBalance.Visible = CheckBoxIsVat.Checked
        LabelVatBalance.Visible = CheckBoxIsVat.Checked
        ComboBoxTotalVat.Visible = CheckBoxIsVat.Checked ' Keep combo visible if checkbox is checked
        LabelTotalVat.Visible = CheckBoxIsVat.Checked

        ' Update Payment and Balance logic
        UpdatePaymentAndBalance()

        ' LIVE AUTO-SAVE (Guard against saving during form load or slot state loading)
        If Not isFormLoading AndAlso Not isProcessingLoad AndAlso SlotID > 0 Then SaveSlotState(SlotID)
    End Sub

    Private Sub txtOurDiscount_TextChanged(sender As Object, e As EventArgs) Handles txtOurDiscount.TextChanged
        CalculateGrandTotal()
    End Sub

    Private Sub txtInvDiscount_TextChanged(sender As Object, e As EventArgs) Handles txtInvDiscount.TextChanged
        CalculateGrandTotal()
    End Sub

    Private Sub ComboBoxTotalVat_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxTotalVat.SelectedIndexChanged
        ' Validate: If "No VAT" (ID 1) is selected while VAT checkbox is True, uncheck it.
        If ComboBoxTotalVat.SelectedValue IsNot Nothing Then
            Dim currentVatId As Integer = 1
            Integer.TryParse(ComboBoxTotalVat.SelectedValue.ToString(), currentVatId)
            If currentVatId = 1 AndAlso CheckBoxIsVat.Checked Then
                CheckBoxIsVat.Checked = False
            End If
        End If

        ' Refresh all grid items whenever the VAT rate changes
        If Not isFormLoading AndAlso dtBill.Rows.Count > 0 Then
            ConvertBillVatState()
        End If

        CalculateGrandTotal()
    End Sub



    Private Sub ConvertBillVatState()
        If dtBill.Rows.Count = 0 Then Return

        ' Determine the current VAT rate selected for the overall bill
        Dim currentVatRate As Decimal = 0
        If CheckBoxIsVat.Checked AndAlso ComboBoxTotalVat.SelectedItem IsNot Nothing Then
            Dim rowVat As DataRowView = DirectCast(ComboBoxTotalVat.SelectedItem, DataRowView)
            Decimal.TryParse(rowVat("vat_value").ToString(), currentVatRate)
        End If

        For Each row As DataRow In dtBill.Rows
            Dim unitPrice As Decimal = If(row("Selling Price") Is DBNull.Value, 0, Convert.ToDecimal(row("Selling Price")))
            Dim qtyVal As Decimal = If(row("Qty") Is DBNull.Value, 0, Convert.ToDecimal(row("Qty")))
            Dim discVal As Decimal = If(row("Dis") Is DBNull.Value, 0, Convert.ToDecimal(row("Dis")))

            Dim netUnitPrice As Decimal = unitPrice
            Dim lineTotal As Decimal = 0

            If CheckBoxIsVat.Checked AndAlso currentVatRate > 0 Then
                ' Re-convert items to follow the NEW selected VAT rate
                netUnitPrice = unitPrice / (1 + currentVatRate / 100)
                lineTotal = (netUnitPrice * (1 - discVal / 100)) * qtyVal
                row("VAT") = ComboBoxTotalVat.Text
            Else
                ' Conversion to NO VAT
                lineTotal = (unitPrice * (1 - discVal / 100)) * qtyVal
                row("VAT") = ""
            End If
            row("Total/Amount") = lineTotal
        Next
        CalculateGrandTotal()
    End Sub

    Private Sub CheckBoxIsVat_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxIsVat.CheckedChanged
        lblCusVatId.Visible = CheckBoxIsVat.Checked
        txtCusVatId.Visible = CheckBoxIsVat.Checked
        ' Automatically convert all existing items when VAT mode is toggled
        If Not isFormLoading AndAlso dtBill.Rows.Count > 0 Then
            ConvertBillVatState()
        End If

        ' Update Session Type in DB to reflect VT if checked
        If Not isFormLoading Then
            UpdateSessionTypeInDB()
            UpdateLiveInvoiceProjection() ' Refresh invoice number immediately
        End If

        ' Validate: If VAT is checked but ComboBox is on "No VAT" (ID 1), force a choice or prevent.
        If CheckBoxIsVat.Checked Then
            Dim currentVatId As Integer = 1
            If ComboBoxTotalVat.SelectedValue IsNot Nothing Then
                Integer.TryParse(ComboBoxTotalVat.SelectedValue.ToString(), currentVatId)
            End If
        End If

        CalculateGrandTotal()
        UpdateItemCalculations() ' Refresh current item price display

        ' Only refresh customer search if user is actively typing (not when loading invoice programmatically)
        If (txtSalesRep.Focused OrElse txtCustomerPhone.Focused) AndAlso
           (txtSalesRep.Text.Trim() <> "" OrElse txtCustomerPhone.Text.Trim() <> "") Then
            SearchCustomers()
        End If
    End Sub

    Private Sub txtCashAmount_KeyDown(sender As Object, e As KeyEventArgs) Handles txtCashAmount.KeyDown
        If e.KeyCode = Keys.Enter Then
            Dim cashAmt As Decimal = 0
            Decimal.TryParse(txtCashAmount.Text, cashAmt)
            If cashAmt > 0 Then
                btnSave.PerformClick()
                e.Handled = True
                e.SuppressKeyPress = True
            End If
        End If
    End Sub

    Private Sub txtCashAmount_TextChanged(sender As Object, e As EventArgs) Handles txtCashAmount.TextChanged
        UpdatePaymentAndBalance()

        ' If the bill is fully or over paid (Change >= 0) and no customer is selected,
        ' auto-fill the generic Cash customer details.
        Dim changeVal As Decimal = 0
        Decimal.TryParse(txtChangeAmount.Text, changeVal)
        If changeVal >= 0 Then
            ' If name, phone and address are all empty, auto-fill "Cash"
            If String.IsNullOrWhiteSpace(txtSalesRep.Text) AndAlso
               String.IsNullOrWhiteSpace(txtCustomerPhone.Text) AndAlso
               String.IsNullOrWhiteSpace(txtCustomerAddress.Text) Then

                txtSalesRep.Text = "CASH"
                txtCustomerPhone.Text = "CASH"
                txtCustomerAddress.Text = "CASH"
                FetchCashCustomerId() ' Ensure background ID is set
            End If
        End If
    End Sub

    Private Sub UpdatePaymentAndBalance()
        If isFormLoading Then Return
        Dim gTotal As Decimal = 0
        Dim cashAmt As Decimal = 0
        Dim advPay As Decimal = 0
        Decimal.TryParse(lblGrandTotal.Text, gTotal)
        Decimal.TryParse(txtCashAmount.Text, cashAmt)
        Decimal.TryParse(txtAdvPay.Text, advPay)

        ' 1. Handle Billing Type (Cash/Credit)
        Dim billingType As String = If(cmbBillingType.Text, "").Trim()
        Dim paymentMethodText As String = If(txtPaymentMethod.Text, "").Trim()

        ' Card/Online Payment Auto-fill Logic (fills remaining amount after advance payment)
        Dim isAutoFillPayment As Boolean = String.Equals(paymentMethodText, "Debit Card", StringComparison.OrdinalIgnoreCase) OrElse
                                          String.Equals(paymentMethodText, "Credit Card", StringComparison.OrdinalIgnoreCase) OrElse
                                          String.Equals(paymentMethodText, "Online Transfer", StringComparison.OrdinalIgnoreCase)

        If isAutoFillPayment Then
            Dim remainingPay As Decimal = Math.Max(0D, gTotal - advPay)
            txtCashAmount.Text = remainingPay.ToString("N2")
            cashAmt = remainingPay ' Update local variable for subsequent calculation
        End If

        Dim isChequeContext As Boolean =
            String.Equals(billingType, "Cheque", StringComparison.OrdinalIgnoreCase) OrElse
            String.Equals(paymentMethodText, "Cheque", StringComparison.OrdinalIgnoreCase)

        ' Change (Pink) = Grand Total - (Cash + Adv Pay)
        Dim changeAmt As Decimal = gTotal - (cashAmt + advPay)
        txtChangeAmount.Text = changeAmt.ToString("N2")

        If String.Equals(billingType, "Credit", StringComparison.OrdinalIgnoreCase) Then
            ' Credit Billing Rule: Payment Method defaults to Credit but can be changed
            If String.IsNullOrWhiteSpace(txtPaymentMethod.Text) Then
                txtPaymentMethod.Text = "Credit"
            End If

            txtCashAmount.Enabled = True ' Enabled for advance payments
            txtPaymentMethod.Enabled = True
            lblCreditBalance.Text = (gTotal - (cashAmt + advPay)).ToString("N2") ' Dynamic balance
        ElseIf String.Equals(billingType, "Cheque", StringComparison.OrdinalIgnoreCase) Then
            ' Cheque Billing Rule: Payment Method MUST be Cheque
            If Not String.Equals(txtPaymentMethod.Text.Trim(), "Cheque", StringComparison.OrdinalIgnoreCase) Then
                txtPaymentMethod.Text = "Cheque"
            End If

            txtCashAmount.Enabled = True ' Enabled for advance payments
            txtPaymentMethod.Enabled = True
            lblCreditBalance.Text = (gTotal - (cashAmt + advPay)).ToString("N2") ' Dynamic balance
        Else
            ' Cash Billing Rule: Payment Method CANNOT be Cheque or Credit
            Dim currMethod As String = txtPaymentMethod.Text.Trim()
            Dim isInvalidForCash As Boolean = String.Equals(currMethod, "Cheque", StringComparison.OrdinalIgnoreCase) OrElse
                                             String.Equals(currMethod, "Credit", StringComparison.OrdinalIgnoreCase)

            If isInvalidForCash Then
                ' Only revert if it's explicitly one of the forbidden ones
                txtPaymentMethod.Text = "Cash"
            End If

            txtCashAmount.Enabled = True
            txtPaymentMethod.Enabled = True
            lblCreditBalance.Text = "0.00"
        End If

        ' Toggle PO Visibility - Always visible so it is optionally accessible for non-Credit billing types
        Dim showPO As Boolean = True

        If LabelPO IsNot Nothing Then LabelPO.Visible = showPO
        If TextBoxPO IsNot Nothing Then TextBoxPO.Visible = showPO
    End Sub

    Private Sub txtAdvPay_TextChanged(sender As Object, e As EventArgs) Handles txtAdvPay.TextChanged
        UpdatePaymentAndBalance()
    End Sub

    Private Sub txtAdvPay_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtAdvPay.KeyPress
        Dim tb As TextBox = DirectCast(sender, TextBox)
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) AndAlso (e.KeyChar <> "."c) AndAlso (e.KeyChar <> "-"c) Then
            e.Handled = True
        End If

        ' Only allow one decimal point
        If (e.KeyChar = "."c) AndAlso tb.Text.Contains(".") Then
            e.Handled = True
        End If

        ' Only allow one minus sign, and it must be at the very start (index 0)
        If (e.KeyChar = "-"c) Then
            If tb.Text.Contains("-") OrElse tb.SelectionStart > 0 Then
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub txtAdvPay_KeyDown(sender As Object, e As KeyEventArgs) Handles txtAdvPay.KeyDown
        If e.KeyCode = Keys.Enter Then
            UpdatePaymentAndBalance()
            e.Handled = True
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub cmbBillingType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbBillingType.SelectedIndexChanged
        If cmbBillingType.Text = "PENDING" Then
    txtPaymentMethod.Text = "PENDING"

ElseIf cmbBillingType.Text = "Credit" Then
    txtPaymentMethod.Text = "Credit"

ElseIf cmbBillingType.Text = "Cheque" Then
    txtPaymentMethod.Text = "Cheque"

ElseIf cmbBillingType.Text = "Cash" Then
    'Do not overwrite valid payment methods for a Cash bill.
    'Cash bills may be paid by Cash, Debit Card, Credit Card, or Online Transfer.
    Dim currentPaymentMethod As String = txtPaymentMethod.Text.Trim()

    If String.IsNullOrWhiteSpace(currentPaymentMethod) OrElse
       String.Equals(currentPaymentMethod, "Cheque", StringComparison.OrdinalIgnoreCase) OrElse
       String.Equals(currentPaymentMethod, "Credit", StringComparison.OrdinalIgnoreCase) Then

        txtPaymentMethod.Text = "Cash"
    End If
End If

        UpdatePaymentAndBalance()

        Dim billingType As String = If(cmbBillingType.Text, "Cash").Trim()
        If String.Equals(billingType, "Cash", StringComparison.OrdinalIgnoreCase) Then
            ' Auto-fill "Cash" for Cash Bills ONLY if name and phone are both empty or already Cash
            Dim isNameEmptyOrCash As Boolean = String.IsNullOrWhiteSpace(txtSalesRep.Text) OrElse String.Equals(txtSalesRep.Text.Trim(), "Cash", StringComparison.OrdinalIgnoreCase)
            Dim isPhoneEmptyOrCash As Boolean = String.IsNullOrWhiteSpace(txtCustomerPhone.Text) OrElse String.Equals(txtCustomerPhone.Text.Trim(), "Cash", StringComparison.OrdinalIgnoreCase)

            If isNameEmptyOrCash AndAlso isPhoneEmptyOrCash Then
                txtSalesRep.Text = "CASH"
                txtCustomerPhone.Text = "CASH"
                txtCustomerAddress.Text = "CASH"
                FetchCashCustomerId()
            End If
        ElseIf Not (String.Equals(billingType, "PENDING", StringComparison.OrdinalIgnoreCase)) Then
            ' Prompt for real customer for Credit/Cheque (Skip for PENDING types)
            If String.Equals(txtSalesRep.Text.Trim(), "Cash", StringComparison.OrdinalIgnoreCase) OrElse String.IsNullOrWhiteSpace(txtSalesRep.Text) Then
                txtSalesRep.Text = ""
                selectedCustomerId = ""
            End If
            If String.Equals(txtCustomerPhone.Text.Trim(), "Cash", StringComparison.OrdinalIgnoreCase) Then
                txtCustomerPhone.Text = ""
            End If
            If String.Equals(txtCustomerAddress.Text.Trim(), "Cash", StringComparison.OrdinalIgnoreCase) Then
                txtCustomerAddress.Text = ""
            End If
        End If

        ' Update current stock display without overwriting manual price/qty edits or historical grid values
        If Not String.IsNullOrEmpty(txtItemID.Text) Then
            Dim tempPrice As String = txtSellingPrice1.Text
            Dim tempQty As String = txtQuantity.Text
            Dim tempDisc As String = txtDiscount.Text

            FetchItemByID(txtItemID.Text.Trim())

            If isEditingHistory AndAlso selectedIndex >= 0 AndAlso selectedIndex < DataGridView2.Rows.Count Then
                Dim row As DataGridViewRow = DataGridView2.Rows(selectedIndex)
                txtSellingPrice1.Text = row.Cells("Selling Price").Value.ToString()
                txtDiscount.Text = row.Cells("Dis").Value.ToString()
                txtQuantity.Text = tempQty ' Keep the quantity user typed (e.g. return quantity "1")
            Else
                txtSellingPrice1.Text = tempPrice
                txtQuantity.Text = tempQty
                txtDiscount.Text = tempDisc
            End If
        End If

        ' Store credit logic removed per user request
    End Sub

    Private Sub cmbBillingType_GotFocus(sender As Object, e As EventArgs) Handles cmbBillingType.GotFocus
        cmbBillingType.DroppedDown = True
    End Sub

    Private Sub cmbBillingType_KeyDown(sender As Object, e As KeyEventArgs) Handles cmbBillingType.KeyDown
        If e.KeyCode = Keys.Down Then
            ' Only move focus if the dropdown is NOT currently open
            If Not cmbBillingType.DroppedDown Then
                txtPaymentMethod.Focus()
                e.Handled = True
            End If
        ElseIf e.KeyCode = Keys.Enter Then
            ' Enter always moves to the next field
            txtPaymentMethod.Focus()
            e.Handled = True
            e.SuppressKeyPress = True
        End If
    End Sub


    Private Sub FetchCashCustomerId()
        Dim openedHere As Boolean = False
        Try
            If conn.State = ConnectionState.Closed Then
                conn.Open()
                openedHere = True
            End If
            ' Try multiple variations to be safe
            Dim sql_cash As String = "SELECT id FROM customer WHERE LOWER(TRIM(name)) = 'cash' OR customer_type = 'Cash' LIMIT 1"
            Using cmd_cash As New MySqlCommand(sql_cash, conn)
                Dim result As Object = cmd_cash.ExecuteScalar()

                If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                    selectedCustomerId = result.ToString()
                Else
                    ' Fallback: Try name LIKE 'Cash%'
                    cmd_cash.CommandText = "SELECT id FROM customer WHERE name LIKE 'Cash%' LIMIT 1"
                    result = cmd_cash.ExecuteScalar()
                    If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                        selectedCustomerId = result.ToString()
                    End If
                End If
            End Using
            If openedHere Then conn.Close()
        Catch ex As Exception
            If openedHere AndAlso conn.State = ConnectionState.Open Then conn.Close()
            ' Optional: Log error or notify user via status bar if available
        End Try
    End Sub

    ' --- Payment Method Search Grid Logic ---
    Private ReadOnly paymentMethods As String() = {"Cash", "Debit Card", "Credit Card", "Online Transfer", "Cheque", "Credit"}

    Private Sub ShowPaymentMethodDropdown()
        Dim dt As New DataTable()
        dt.Columns.Add("Payment Method")

        Dim searchText As String = txtPaymentMethod.Text.Trim().ToLower()
        Dim billingType As String = If(cmbBillingType.Text, "Cash").Trim()
        Dim isCashBill As Boolean = String.Equals(billingType, "Cash", StringComparison.OrdinalIgnoreCase)
        Dim isCreditBill As Boolean = String.Equals(billingType, "Credit", StringComparison.OrdinalIgnoreCase)
        Dim isChequeBill As Boolean = String.Equals(billingType, "Cheque", StringComparison.OrdinalIgnoreCase)

        For Each pm As String In paymentMethods
            ' Strict Filtering based on Billing Type
            If isCreditBill Then
                ' Credit Bill: Only allow "Credit"
                If Not String.Equals(pm, "Credit", StringComparison.OrdinalIgnoreCase) Then Continue For
            ElseIf isChequeBill Then
                ' Cheque Bill: Only allow "Cheque"
                If Not String.Equals(pm, "Cheque", StringComparison.OrdinalIgnoreCase) Then Continue For
            ElseIf isCashBill Then
                ' Cash Bill: Allow all EXCEPT "Credit" and "Cheque" (per user rules)
                If String.Equals(pm, "Credit", StringComparison.OrdinalIgnoreCase) OrElse
                   String.Equals(pm, "Cheque", StringComparison.OrdinalIgnoreCase) Then Continue For
            End If

            If searchText = "" OrElse pm.ToLower().Contains(searchText) Then
                dt.Rows.Add(pm)
            End If
        Next

        If dt.Rows.Count > 0 Then
            dgvPaymentMethod.DataSource = dt
            dgvPaymentMethod.Visible = True
            dgvPaymentMethod.BringToFront()

            ' Position relative to GroupBox7 (the parent container of dgvPaymentMethod)
            Dim screenPos As Point = txtPaymentMethod.PointToScreen(New Point(0, txtPaymentMethod.Height))
            Dim groupPos As Point = GroupBox7.PointToClient(screenPos)
            ' Move slightly right (+60) and up (-15) based on user feedback
            dgvPaymentMethod.Location = New Point(groupPos.X + 110, groupPos.Y - 55)
        Else
            dgvPaymentMethod.Visible = False
        End If
    End Sub

    Private Sub txtPaymentMethod_TextChanged(sender As Object, e As EventArgs) Handles txtPaymentMethod.TextChanged
        If txtPaymentMethod.Focused Then
            ShowPaymentMethodDropdown()
        End If
    End Sub

    Private Sub txtPaymentMethod_KeyDown(sender As Object, e As KeyEventArgs) Handles txtPaymentMethod.KeyDown
        If e.KeyCode = Keys.Enter Then
            If dgvPaymentMethod.Visible AndAlso dgvPaymentMethod.Rows.Count > 0 Then
                ' Select the first item from the dropdown
                txtPaymentMethod.Text = dgvPaymentMethod.Rows(0).Cells("Payment Method").Value.ToString()
                dgvPaymentMethod.Visible = False
                UpdatePaymentAndBalance()
                txtCashAmount.Focus()
            Else
                ' If dropdown is not visible or empty, try to save
                btnSave.PerformClick()
            End If
            e.Handled = True
            e.SuppressKeyPress = True
        ElseIf e.KeyCode = Keys.Down Then
            If dgvPaymentMethod.Visible Then
                dgvPaymentMethod.Focus()
                e.Handled = True
            End If
        ElseIf e.KeyCode = Keys.Escape Then
            dgvPaymentMethod.Visible = False
            e.Handled = True
        End If
    End Sub

    Private Sub txtPaymentMethod_Enter(sender As Object, e As EventArgs) Handles txtPaymentMethod.Enter
        ShowPaymentMethodDropdown()
    End Sub

    Private Sub txtPaymentMethod_Leave(sender As Object, e As EventArgs) Handles txtPaymentMethod.Leave
        ' Delay the check to see if focus moved to the grid
        Me.BeginInvoke(New MethodInvoker(Sub()
                                             If Not dgvPaymentMethod.Focused Then
                                                 dgvPaymentMethod.Visible = False
                                             End If
                                         End Sub))
    End Sub

    Private Sub dgvPaymentMethod_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvPaymentMethod.CellClick
        If e.RowIndex >= 0 Then
            txtPaymentMethod.Text = dgvPaymentMethod.Rows(e.RowIndex).Cells("Payment Method").Value.ToString()
            dgvPaymentMethod.Visible = False
            UpdatePaymentAndBalance()
            txtCashAmount.Focus()
        End If
    End Sub

    Private Sub dgvPaymentMethod_KeyDown(sender As Object, e As KeyEventArgs) Handles dgvPaymentMethod.KeyDown
        If e.KeyCode = Keys.Enter Then
            If dgvPaymentMethod.SelectedRows.Count > 0 Then
                txtPaymentMethod.Text = dgvPaymentMethod.SelectedRows(0).Cells("Payment Method").Value.ToString()
                dgvPaymentMethod.Visible = False
                UpdatePaymentAndBalance()
                txtCashAmount.Focus()
                e.Handled = True
            End If
        ElseIf e.KeyCode = Keys.Escape Then
            dgvPaymentMethod.Visible = False
            txtPaymentMethod.Focus()
            e.Handled = True
        End If
    End Sub

    Private Sub dgvPaymentMethod_Leave(sender As Object, e As EventArgs) Handles dgvPaymentMethod.Leave
        dgvPaymentMethod.Visible = False
    End Sub

    Private Sub NumericInputOnly(sender As Object, e As KeyPressEventArgs) Handles txtCashAmount.KeyPress
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) AndAlso (e.KeyChar <> "."c) Then
            e.Handled = True
        End If

        ' Only allow one decimal point
        If (e.KeyChar = "."c) AndAlso (DirectCast(sender, TextBox).Text.IndexOf("."c) > -1) Then
            e.Handled = True
        End If
    End Sub
    Private Sub TempSales_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        ' Ctrl + S to toggle between EL and GR bill types directly
        If e.Control AndAlso e.KeyCode = Keys.S Then
            ToggleBillType()
            e.SuppressKeyPress = True
        ElseIf e.Control AndAlso e.KeyCode = Keys.Right Then
            btnNext.PerformClick()
            e.SuppressKeyPress = True
        ElseIf e.Control AndAlso e.KeyCode = Keys.Left Then
            ButtonBefore.PerformClick()
            e.SuppressKeyPress = True
        ElseIf e.KeyCode = Keys.F2 Then
            btnAddNew.PerformClick()
        ElseIf e.KeyCode = Keys.F3 Then
            If btnUpdate.Text = "Edit" Then
                btnUpdate.PerformClick()
                e.SuppressKeyPress = True
            End If
        ElseIf e.KeyCode = Keys.F4 Then
            If btnUpdate.Text = "Return" Then
                btnUpdate.PerformClick()
                e.SuppressKeyPress = True
            End If
        ElseIf e.KeyCode = Keys.F12 Then
            If btnSaveRGR.Visible AndAlso btnSaveRGR.Enabled Then
                btnSaveRGR.PerformClick()
            Else
                btnSave.PerformClick()
            End If
        ElseIf e.KeyCode = Keys.Delete Then
            btnDelete.PerformClick()
            ' ElseIf e.Control AndAlso e.KeyCode = Keys.A AndAlso Module1.IsRgrVisible Then
            '    ' Ctrl + A to toggle RGR Save button visibility (Only works if RGR is globally visible)
            '    If btnSaveRGR IsNot Nothing Then
            '        btnSaveRGR.Visible = Not btnSaveRGR.Visible
            '        If btnSaveRGR.Visible Then btnSaveRGR.Focus()
            '    End If
            '    e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub cmbReturnReason_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbReturnReason.SelectedIndexChanged
        If isRestoringReason OrElse isProcessingLoad Then Exit Sub

        If String.Equals(cmbReturnReason.Text, "Other", StringComparison.OrdinalIgnoreCase) Then
            Using dlg As New ReturnReasonDialog()
                ' Pre-fill with existing reason if there is one
                If Not String.IsNullOrEmpty(txtOtherReason.Text) Then
                    Dim reasonText As String = txtOtherReason.Text.Trim()
                    Dim isAddStock As Boolean = True ' default
                    If reasonText.StartsWith("[AddStock]") Then
                        isAddStock = True
                        reasonText = reasonText.Substring(10).Trim()
                    ElseIf reasonText.StartsWith("[NoStock]") Then
                        isAddStock = False
                        reasonText = reasonText.Substring(9).Trim()
                    End If
                    dlg.txtReason.Text = reasonText
                    dlg.chkStock.Checked = isAddStock
                End If

                If dlg.ShowDialog() = DialogResult.OK Then
                    Dim stockTag As String = If(dlg.AddToStock, "[AddStock]", "[NoStock]")
                    txtOtherReason.Text = stockTag & " " & dlg.ReturnReason
                    txtOtherReason.Visible = True
                Else
                    ' Revert selection to None (index 0) if cancelled
                    isRestoringReason = True
                    cmbReturnReason.SelectedIndex = 0
                    txtOtherReason.Text = ""
                    txtOtherReason.Visible = False
                    isRestoringReason = False
                End If
            End Using
        Else
            txtOtherReason.Text = ""
            txtOtherReason.Visible = False
        End If
    End Sub

    ' Hide search grid when clicking outside to other containers
    Private Sub HideSearchGrid(sender As Object, e As EventArgs) Handles Me.Click, GroupBox4.Click, GroupBox3.Click, GroupBox1.Click, DataGridView2.Click, GroupBox7.Click, GroupBox6.Click, GroupBox5.Click, InvDetailsPanel.Click
        If dgvSearch.Visible Then
            dgvSearch.Visible = False
        End If
        If dgvPaymentMethod.Visible Then
            dgvPaymentMethod.Visible = False
        End If
    End Sub

    Private Sub TempSales_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        ' Save current slot state before closing (Only if we finished loading to avoid wiping draft)
        If Not isFormLoading AndAlso SlotID > 0 Then SaveSlotState(SlotID)

        ' Release ALL session slots held/visited by this window instance
        ' Use ToArray() to avoid "Collection was modified" exception since ResetMySessions removes from the list
        For Each sid In MyReservedSlots.ToArray()
            ResetMySessions(sid)
        Next
    End Sub

    ' Dummy handler for build compatibility
    Private Sub Label13_Click(sender As Object, e As EventArgs)
    End Sub

    Private Sub btnSaveRGR_Click(sender As Object, e As EventArgs) Handles btnSaveRGR.Click
        Module1.IsRgrModeActive = True
        Try
            btnSave.PerformClick()
        Finally
            Module1.IsRgrModeActive = False
        End Try
    End Sub

    Private Function GetItemWholesalePrice(itemId As String) As Decimal
        Dim wPrice As Decimal = 0
        Try
            Using localConn As New MySqlConnection(ConnStr)
                localConn.Open()
                Dim sql As String = "SELECT IFNULL(whole_selling_price, selling_price) FROM items WHERE id = @id"
                Using cmd As New MySqlCommand(sql, localConn)
                    cmd.Parameters.AddWithValue("@id", itemId)
                    Dim res = cmd.ExecuteScalar()
                    If res IsNot Nothing AndAlso res IsNot DBNull.Value Then
                        Decimal.TryParse(res.ToString(), wPrice)
                    End If
                End Using
            End Using
        Catch ex As Exception
            ' Fail silently
        End Try
        Return wPrice
    End Function

    Private Function GetItemRetailPrice(itemId As String) As Decimal
        Dim rPrice As Decimal = 0
        Try
            Using localConn As New MySqlConnection(ConnStr)
                localConn.Open()
                Dim sql As String = "SELECT IFNULL(retail_selling_price, selling_price) FROM items WHERE id = @id"
                Using cmd As New MySqlCommand(sql, localConn)
                    cmd.Parameters.AddWithValue("@id", itemId)
                    Dim res = cmd.ExecuteScalar()
                    If res IsNot Nothing AndAlso res IsNot DBNull.Value Then
                        Decimal.TryParse(res.ToString(), rPrice)
                    End If
                End Using
            End Using
        Catch ex As Exception
            ' Fail silently
        End Try
        Return rPrice
    End Function

    Private Sub CheckBoxPrintAsRetail_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxPrintAsRetail.CheckedChanged
        If CheckBoxWholesale.Checked Then
            SyncGridPricesWithMode()
        End If
    End Sub

End Class

