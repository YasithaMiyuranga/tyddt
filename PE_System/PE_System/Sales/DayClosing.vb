Imports MySql.Data.MySqlClient

Public Class DayClosing
    Private conn As New MySqlConnection(Module1.ConnStr)
    Private denominations() As Integer = {5000, 2000, 1000, 500, 100, 50, 20, 10, 5, 2, 1}
    Private openingInputs As New Dictionary(Of Integer, TextBox)
    Private closingInputs As New Dictionary(Of Integer, TextBox)

    Private Sub DayClosing_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        InitializeDatabase()
        SetupDenominationPanels()
        CheckExistingSession()
        LoadDailySummary()
        LoadSessionHistory()
        btnClearOpening.BringToFront()
        btnClearClosing.BringToFront()

        ' Hide Petty Cash and Session History tabs
        TabControl1.TabPages.Remove(TabPagePettyCash)
        TabControl1.TabPages.Remove(TabPageHistory)
    End Sub

    Private Sub InitializeDatabase()
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()
            ' Create cash_drawer_history table as per user requirements
            Dim sqlHistory = "CREATE TABLE IF NOT EXISTS cash_drawer_history (" &
                           "id INT AUTO_INCREMENT PRIMARY KEY, " &
                           "log_date DATETIME DEFAULT CURRENT_TIMESTAMP, " &
                           "log_type ENUM('OPENING','CLOSING') NOT NULL, " &
                           "user_id INT, " &
                           "d5000 INT DEFAULT 0, d2000 INT DEFAULT 0, d1000 INT DEFAULT 0, " &
                           "d500 INT DEFAULT 0, d100 INT DEFAULT 0, d50 INT DEFAULT 0, " &
                           "d20 INT DEFAULT 0, d10 INT DEFAULT 0, d5 INT DEFAULT 0, " &
                           "d2 INT DEFAULT 0, d1 INT DEFAULT 0, " &
                           "total_amount DECIMAL(18,2) DEFAULT 0.00, " &
                           "system_expected_cash DECIMAL(18,2) DEFAULT 0.00, " &
                           "machine VARCHAR(255))"
            Dim cmd = New MySqlCommand(sqlHistory, conn)
            cmd.ExecuteNonQuery()
            conn.Close()
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub SetupDenominationPanels()
        SetupPanel(pnlOpeningDenom, openingInputs, AddressOf CalculateOpeningTotal)
        SetupPanel(pnlClosingDenom, closingInputs, AddressOf CalculateClosingTotal)
    End Sub

    Private Sub SetupPanel(panel As FlowLayoutPanel, dict As Dictionary(Of Integer, TextBox), calcHandler As EventHandler)
        panel.Controls.Clear()
        For Each d In denominations
            Dim container As New FlowLayoutPanel With {.Width = 220, .Height = 40, .FlowDirection = FlowDirection.LeftToRight}
            Dim lbl = New Label With {.Text = d.ToString("N0") & " x ", .Width = 80, .Font = New Font("Segoe UI", 12, FontStyle.Bold), .TextAlign = ContentAlignment.MiddleRight}
            Dim txt = New TextBox With {.Width = 100, .Font = New Font("Segoe UI", 12), .Tag = d}
            
            AddHandler txt.TextChanged, calcHandler
            AddHandler txt.KeyDown, AddressOf Denom_KeyDown
            
            container.Controls.Add(lbl)
            container.Controls.Add(txt)
            panel.Controls.Add(container)
            dict(d) = txt
        Next
    End Sub

    Private Sub Denom_KeyDown(sender As Object, e As KeyEventArgs)
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            Dim currentTxt = DirectCast(sender, TextBox)
            Dim parentContainer = currentTxt.Parent
            Dim mainPanel = parentContainer.Parent
            
            Dim currentIndex = mainPanel.Controls.IndexOf(parentContainer)
            If currentIndex < mainPanel.Controls.Count - 1 Then
                Dim nextContainer = mainPanel.Controls(currentIndex + 1)
                Dim nextTxt = nextContainer.Controls.OfType(Of TextBox).FirstOrDefault()
                If nextTxt IsNot Nothing Then
                    nextTxt.Focus()
                    nextTxt.SelectAll()
                End If
            Else
                ' Last box, focus the relevant button
                If mainPanel.Name = "pnlOpeningDenom" Then btnStartDay.Focus() Else btnFinalClose.Focus()
            End If
        End If
    End Sub

    Private Sub CheckExistingSession()
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()
            Dim sql = "SELECT * FROM cash_drawer_history WHERE DATE(log_date) = CURDATE() ORDER BY id ASC"
            Dim dt As New DataTable()
            Dim da As New MySqlDataAdapter(sql, conn)
            da.Fill(dt)
            
            Dim hasOpening As Boolean = False
            Dim hasClosing As Boolean = False
            
            ' Determine current state based on latest entry
            If dt.Rows.Count > 0 Then
                Dim lastRow = dt.Rows(dt.Rows.Count - 1)
                Dim lastType = lastRow("log_type").ToString()
 
                If lastType = "OPENING" Then
                    ' Currently Open
                    hasOpening = True
                    hasClosing = False
                    lblOpeningTotal.Text = Convert.ToDecimal(lastRow("total_amount")).ToString("N2")
                    
                    Dim isDirect As Boolean = (Convert.ToDecimal(lastRow("total_amount")) > 0)
                    If isDirect Then
                        For Each d In denominations
                            Dim dVal = If(lastRow("d" & d) Is DBNull.Value, 0, Convert.ToInt32(lastRow("d" & d)))
                            If dVal > 0 Then
                                isDirect = False
                                Exit For
                            End If
                        Next
                    End If
                    
                    chkDirectOpening.Checked = isDirect
                    If isDirect Then
                        txtDirectOpening.Text = Convert.ToDecimal(lastRow("total_amount")).ToString("F2")
                    Else
                        LoadDenomsToPanel(lastRow, openingInputs)
                    End If
                    
                    btnStartDay.Enabled = False
                    pnlOpeningDenom.Enabled = False
                    chkDirectOpening.Enabled = False
                    txtDirectOpening.Enabled = False
                    SetClearOpeningState(False)
                    Module1.IsDayOpened = True
                    TabControl1.SelectedIndex = 1 ' Move to Closing/Expenses tab
                Else
                    ' Currently Closed (Last was CLOSING)
                    hasOpening = False
                    hasClosing = True
                    Module1.IsDayOpened = False
                    
                    ' Enable Opening Panel for a new session
                    btnStartDay.Enabled = True
                    pnlOpeningDenom.Enabled = True
                    chkDirectOpening.Enabled = True
                    txtDirectOpening.Enabled = True
                    SetClearOpeningState(True)
                    TabControl1.SelectedIndex = 0
                End If
            Else
                ' No session today - standard new day
                hasOpening = False
                hasClosing = False
                Module1.IsDayOpened = False
                btnStartDay.Enabled = True
                pnlOpeningDenom.Enabled = True
                chkDirectOpening.Enabled = True
                txtDirectOpening.Enabled = True
                SetClearOpeningState(True)
                TabControl1.SelectedIndex = 0
            End If
            conn.Close()
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub LoadDenomsToPanel(row As DataRow, inputs As Dictionary(Of Integer, TextBox))
        For Each d In denominations
            Dim colName = "d" & d
            If inputs.ContainsKey(d) Then
                inputs(d).Text = row(colName).ToString()
            End If
        Next
    End Sub

    Private Sub LoadFromPreviousClosing()
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()
            Dim sql = "SELECT * FROM cash_drawer_history WHERE log_type = 'CLOSING' ORDER BY id DESC LIMIT 1"
            Using cmd As New MySqlCommand(sql, conn)
                Using dr = cmd.ExecuteReader()
                    If dr.Read() Then
                        Dim isDirect As Boolean = (Convert.ToDecimal(dr("total_amount")) > 0)
                        If isDirect Then
                            For Each d In denominations
                                Dim dVal = If(dr("d" & d) Is DBNull.Value, 0, Convert.ToInt32(dr("d" & d)))
                                If dVal > 0 Then
                                    isDirect = False
                                    Exit For
                                End If
                            Next
                        End If

                        chkDirectOpening.Checked = isDirect
                        If isDirect Then
                            txtDirectOpening.Text = Convert.ToDecimal(dr("total_amount")).ToString("F2")
                        Else
                            For Each d In denominations
                                Dim colName = "d" & d
                                If openingInputs.ContainsKey(d) Then
                                    openingInputs(d).Text = dr(colName).ToString()
                                End If
                            Next
                        End If
                    End If
                End Using
            End Using
            conn.Close()
            If Not chkDirectOpening.Checked Then
                CalculateOpeningTotal()
            Else
                Dim amt As Decimal = 0
                Decimal.TryParse(txtDirectOpening.Text, amt)
                lblOpeningTotal.Text = amt.ToString("N2")
                UpdateClosingExpected()
            End If
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub CalculateOpeningTotal()
        Dim total As Decimal = 0
        For Each kvp In openingInputs
            Dim count As Integer = 0
            Integer.TryParse(kvp.Value.Text, count)
            total += (kvp.Key * count)
        Next
        lblOpeningTotal.Text = total.ToString("N2")
        UpdateClosingExpected()
    End Sub

    Private Sub CalculateClosingTotal()
        Dim total As Decimal = 0
        For Each kvp In closingInputs
            Dim count As Integer = 0
            Integer.TryParse(kvp.Value.Text, count)
            total += (kvp.Key * count)
        Next
        lblActualPhysical.Text = total.ToString("N2")
        UpdateClosingExpected()
    End Sub

    Private Sub LoadPettyCashLog()
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()
            Dim sql = "SELECT id As ID, date As Date_Time, item_name As Description, amount As Amount, transaction_type As Type, user As User, receipt_no As Receipt_No FROM petty_cash WHERE DATE(date) = CURDATE() AND item_type != 'SYSTEM' ORDER BY id DESC"
            Dim dt As New DataTable()
            Using cmd As New MySqlCommand(sql, conn)
                Using da As New MySqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
            dgvPetty.DataSource = dt
            
            ' Format grid columns and header text
            If dgvPetty.Columns.Contains("ID") Then dgvPetty.Columns("ID").HeaderText = "ID"
            If dgvPetty.Columns.Contains("Date_Time") Then dgvPetty.Columns("Date_Time").HeaderText = "Date/Time"
            If dgvPetty.Columns.Contains("Description") Then dgvPetty.Columns("Description").HeaderText = "Description"
            If dgvPetty.Columns.Contains("Amount") Then
                dgvPetty.Columns("Amount").HeaderText = "Amount"
                dgvPetty.Columns("Amount").DefaultCellStyle.Format = "N2"
                dgvPetty.Columns("Amount").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            End If
            If dgvPetty.Columns.Contains("Type") Then dgvPetty.Columns("Type").HeaderText = "Type"
            If dgvPetty.Columns.Contains("User") Then dgvPetty.Columns("User").HeaderText = "User"
            If dgvPetty.Columns.Contains("Receipt_No") Then dgvPetty.Columns("Receipt_No").HeaderText = "Receipt No"
            
            conn.Close()
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub LoadDailySummary()
        LoadPettyCashLog()
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()
            
            ' 1. Net Cash Sales (Sales IN - Refund OUT)
            ' Sales are identified by 'Cash Sale%', Refunds by 'Cash Refund%'
            Dim totalSalesIn As Decimal = 0
            Dim totalRefundsOut As Decimal = 0
            
            Dim sqlSalesIn = "SELECT SUM(amount) FROM petty_cash WHERE DATE(date) = CURDATE() AND transaction_type = 'IN' AND item_name LIKE 'Cash Sale%'"
            Using cmdSales As New MySqlCommand(sqlSalesIn, conn)
                Dim res = cmdSales.ExecuteScalar()
                totalSalesIn = If(res Is DBNull.Value Or res Is Nothing, 0, Convert.ToDecimal(res))
            End Using

            Dim sqlRefunds = "SELECT SUM(amount) FROM petty_cash WHERE DATE(date) = CURDATE() AND transaction_type = 'OUT' AND item_name LIKE 'Cash Refund%'"
            Using cmdRef As New MySqlCommand(sqlRefunds, conn)
                Dim res = cmdRef.ExecuteScalar()
                totalRefundsOut = If(res Is DBNull.Value Or res Is Nothing, 0, Convert.ToDecimal(res))
            End Using

            lblNetSales.Text = (totalSalesIn - totalRefundsOut).ToString("N2")

            ' 2. Total Manual Petty Cash IN (Additions)
            ' Non-sales IN entries
            Dim sqlPettyIn = "SELECT SUM(amount) FROM petty_cash WHERE DATE(date) = CURDATE() AND transaction_type = 'IN' AND item_name NOT LIKE 'Cash Sale%'"
            Using cmdPettyIn As New MySqlCommand(sqlPettyIn, conn)
                Dim res = cmdPettyIn.ExecuteScalar()
                lblPettyIn.Text = If(res Is DBNull.Value Or res Is Nothing, "0.00", Convert.ToDecimal(res).ToString("N2"))
            End Using

            ' 3. Total Petty Cash Expenses (OUT)
            ' Non-refund OUT entries
            Dim sqlPettyOut = "SELECT SUM(amount) FROM petty_cash WHERE DATE(date) = CURDATE() AND transaction_type = 'OUT' AND item_name NOT LIKE 'Cash Refund%'"
            Using cmdPettyOut As New MySqlCommand(sqlPettyOut, conn)
                Dim res = cmdPettyOut.ExecuteScalar()
                lblTotalPetty.Text = If(res Is DBNull.Value Or res Is Nothing, "0.00", Convert.ToDecimal(res).ToString("N2"))
            End Using

            conn.Close()
            UpdateClosingExpected()
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub UpdateClosingExpected()
        Try
            Dim opening As Decimal = 0
            Decimal.TryParse(lblOpeningTotal.Text, opening)
            
            Dim pettyIn As Decimal = 0
            Dim pettyOut As Decimal = 0

            ' Unified calculation from petty_cash table for everything today
            Try
                If conn.State = ConnectionState.Closed Then conn.Open()
                
                Dim sqlIn = "SELECT SUM(amount) FROM petty_cash WHERE DATE(date) = CURDATE() AND transaction_type = 'IN'"
                Using cmdIn As New MySqlCommand(sqlIn, conn)
                    Dim res = cmdIn.ExecuteScalar()
                    pettyIn = If(res Is DBNull.Value Or res Is Nothing, 0, Convert.ToDecimal(res))
                End Using

                Dim sqlOut = "SELECT SUM(amount) FROM petty_cash WHERE DATE(date) = CURDATE() AND transaction_type = 'OUT'"
                Using cmdOut As New MySqlCommand(sqlOut, conn)
                    Dim res = cmdOut.ExecuteScalar()
                    pettyOut = If(res Is DBNull.Value Or res Is Nothing, 0, Convert.ToDecimal(res))
                End Using

                conn.Close()
            Catch : End Try

            Dim actual As Decimal = 0
            Decimal.TryParse(lblActualPhysical.Text, actual)
            
            ' Core Reconciliation Formula
            Dim expected = opening + pettyIn - pettyOut
            lblExpectedDrawer.Text = expected.ToString("N2")
            
            Dim variance = actual - expected
            lblVariance.Text = variance.ToString("N2")
            lblVariance.ForeColor = If(variance >= 0, Color.Lime, Color.Red)

            ' Owner-only visibility
            Dim isOwner = (Module1.FinancialRole.ToLower() = "owner")
            lblVariance.Visible = isOwner
            lblExpectedDrawer.Visible = isOwner
            lblVarianceHeader.Visible = isOwner
            lblExpectedHeader.Visible = isOwner
            
        Catch ex As Exception
        End Try
    End Sub

    Private Sub btnStartDay_Click(sender As Object, e As EventArgs) Handles btnStartDay.Click
        Try
            Dim openingAmt As Decimal = 0
            Decimal.TryParse(lblOpeningTotal.Text, openingAmt)

            If conn.State = ConnectionState.Closed Then conn.Open()
            Dim sql = "INSERT INTO cash_drawer_history (log_date, user_id, log_type, d5000, d2000, d1000, d500, d100, d50, d20, d10, d5, d2, d1, total_amount, machine) " &
                      "VALUES (NOW(), @uid, 'OPENING', @d5000, @d2000, @d1000, @d500, @d100, @d50, @d20, @d10, @d5, @d2, @d1, @total, @mch)"
            Using cmd As New MySqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@uid", Module1.CurrentUserID)
                For Each d In denominations
                    Dim count As Integer = 0
                    If Not chkDirectOpening.Checked AndAlso openingInputs.ContainsKey(d) Then
                        Integer.TryParse(openingInputs(d).Text, count)
                    End If
                    cmd.Parameters.AddWithValue("@d" & d, count)
                Next
                cmd.Parameters.AddWithValue("@total", openingAmt)
                cmd.Parameters.AddWithValue("@mch", Environment.MachineName)
                cmd.ExecuteNonQuery()
            End Using
            conn.Close()

            Module1.IsDayOpened = True
            Dim parentForm = TryCast(Me.MdiParent, Start)
            If parentForm IsNot Nothing Then parentForm.ApplyPermissions()

            MessageBox.Show("Day Session Started.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            btnStartDay.Enabled = False
            pnlOpeningDenom.Enabled = False
            chkDirectOpening.Enabled = False
            txtDirectOpening.Enabled = False
            SetClearOpeningState(False)
            LoadSessionHistory()
            TabControl1.SelectedIndex = 1 
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub btnFinalClose_Click(sender As Object, e As EventArgs) Handles btnFinalClose.Click
        Try
            Dim actualAmt As Decimal = 0
            Decimal.TryParse(lblActualPhysical.Text, actualAmt)
            Dim expectedAmt As Decimal = 0
            Decimal.TryParse(lblExpectedDrawer.Text, expectedAmt)

            If conn.State = ConnectionState.Closed Then conn.Open()
            Dim sql = "INSERT INTO cash_drawer_history (log_date, user_id, log_type, d5000, d2000, d1000, d500, d100, d50, d20, d10, d5, d2, d1, total_amount, system_expected_cash, machine) " &
                      "VALUES (NOW(), @uid, 'CLOSING', @d5000, @d2000, @d1000, @d500, @d100, @d50, @d20, @d10, @d5, @d2, @d1, @total, @exp, @mch)"
            Using cmd As New MySqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@uid", Module1.CurrentUserID)
                For Each d In denominations
                    Dim count As Integer = 0
                    If Not chkDirectClosing.Checked AndAlso closingInputs.ContainsKey(d) Then
                        Integer.TryParse(closingInputs(d).Text, count)
                    End If
                    cmd.Parameters.AddWithValue("@d" & d, count)
                Next
                cmd.Parameters.AddWithValue("@total", actualAmt)
                cmd.Parameters.AddWithValue("@exp", expectedAmt)
                cmd.Parameters.AddWithValue("@mch", Environment.MachineName)
                cmd.ExecuteNonQuery()
            End Using
            conn.Close()

            Module1.IsDayOpened = False
            Dim parentForm = TryCast(Me.MdiParent, Start)
            If parentForm IsNot Nothing Then parentForm.ApplyPermissions()

            MessageBox.Show("Day Session Closed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Me.Close()
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub btnAddPetty_Click(sender As Object, e As EventArgs) Handles btnAddPetty.Click
        PettyCashAdd.ShowDialog()
        LoadDailySummary()
    End Sub

    Private Sub btnRefreshPetty_Click(sender As Object, e As EventArgs) Handles btnRefreshPetty.Click
        LoadDailySummary()
    End Sub

    ''' <summary>Enables or disables the Clear Opening button with matching visual feedback.</summary>
    Private Sub SetClearOpeningState(enabled As Boolean)
        btnClearOpening.Enabled = enabled
        If enabled Then
            btnClearOpening.BackColor = System.Drawing.Color.Yellow
            btnClearOpening.ForeColor = System.Drawing.Color.Black
        Else
            btnClearOpening.BackColor = System.Drawing.Color.FromArgb(200, 200, 200) ' light gray
            btnClearOpening.ForeColor = System.Drawing.Color.FromArgb(120, 120, 120) ' dim gray text
        End If
    End Sub

    Private Sub btnClearOpening_Click(sender As Object, e As EventArgs) Handles btnClearOpening.Click
        For Each kvp In openingInputs
            kvp.Value.Text = "0"
        Next
        CalculateOpeningTotal()
    End Sub

    Private Sub btnClearClosing_Click(sender As Object, e As EventArgs) Handles btnClearClosing.Click
        For Each kvp In closingInputs
            kvp.Value.Text = "0"
        Next
        CalculateClosingTotal()
    End Sub

    Private Sub LoadSessionHistory()
        Try
            If conn.State = ConnectionState.Closed Then conn.Open()
            
            Dim sql = "SELECT " &
                      "DATE(o.log_date) AS 'Date', " &
                      "o.id AS 'Session ID', " &
                      "o.log_date AS 'Opening Time', " &
                      "c.log_date AS 'Closing Time', " &
                      "o.total_amount AS 'Opening Amt', " &
                      "CASE WHEN c.id IS NOT NULL THEN c.total_amount ELSE 0.00 END AS 'Closing Amt', " &
                      "((SELECT COALESCE(SUM(amount), 0) FROM petty_cash " &
                      "  WHERE transaction_type = 'IN' AND item_name LIKE 'Cash Sale%' " &
                      "    AND date >= o.log_date " &
                      "    AND date <= COALESCE(c.log_date, (SELECT MIN(log_date) FROM cash_drawer_history WHERE log_type = 'OPENING' AND id > o.id), NOW())) - " &
                      " (SELECT COALESCE(SUM(amount), 0) FROM petty_cash " &
                      "  WHERE transaction_type = 'OUT' AND item_name LIKE 'Cash Refund%' " &
                      "    AND date >= o.log_date " &
                      "    AND date <= COALESCE(c.log_date, (SELECT MIN(log_date) FROM cash_drawer_history WHERE log_type = 'OPENING' AND id > o.id), NOW()))) AS 'Cash Sales', " &
                      "CASE WHEN c.id IS NOT NULL THEN c.system_expected_cash ELSE 0.00 END AS 'Expected Amt', " &
                      "(SELECT name FROM user WHERE id = o.user_id) AS 'Opened By', " &
                      "(SELECT name FROM user WHERE id = c.user_id) AS 'Closed By' " &
                      "FROM cash_drawer_history o " &
                      "LEFT JOIN cash_drawer_history c ON c.log_type = 'CLOSING' " &
                      "  AND c.id = (SELECT MIN(id) FROM cash_drawer_history WHERE log_type = 'CLOSING' AND id > o.id) " &
                      "  AND NOT EXISTS ( " &
                      "    SELECT 1 FROM cash_drawer_history o2 " &
                      "    WHERE o2.log_type = 'OPENING' AND o2.id > o.id AND o2.id < c.id " &
                      "  ) " &
                      "WHERE o.log_type = 'OPENING' " &
                      "ORDER BY o.id DESC"
                      
            Dim dt As New DataTable()
            Using cmd As New MySqlCommand(sql, conn)
                Using da As New MySqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
            dgvHistory.DataSource = dt
            
            ' Format Grid Columns
            If dgvHistory.Columns.Contains("Date") Then
                dgvHistory.Columns("Date").DefaultCellStyle.Format = "yyyy-MM-dd"
            End If
            If dgvHistory.Columns.Contains("Opening Time") Then
                dgvHistory.Columns("Opening Time").DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss"
            End If
            If dgvHistory.Columns.Contains("Closing Time") Then
                dgvHistory.Columns("Closing Time").DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss"
            End If
            If dgvHistory.Columns.Contains("Opening Amt") Then
                dgvHistory.Columns("Opening Amt").DefaultCellStyle.Format = "N2"
                dgvHistory.Columns("Opening Amt").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            End If
            If dgvHistory.Columns.Contains("Closing Amt") Then
                dgvHistory.Columns("Closing Amt").DefaultCellStyle.Format = "N2"
                dgvHistory.Columns("Closing Amt").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            End If
            If dgvHistory.Columns.Contains("Cash Sales") Then
                dgvHistory.Columns("Cash Sales").DefaultCellStyle.Format = "N2"
                dgvHistory.Columns("Cash Sales").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            End If
            If dgvHistory.Columns.Contains("Expected Amt") Then
                dgvHistory.Columns("Expected Amt").DefaultCellStyle.Format = "N2"
                dgvHistory.Columns("Expected Amt").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            End If
            
            conn.Close()
        Catch ex As Exception
            If conn.State = ConnectionState.Open Then conn.Close()
            MessageBox.Show("Error loading history: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnRefreshHistory_Click(sender As Object, e As EventArgs) Handles btnRefreshHistory.Click
        LoadSessionHistory()
    End Sub

    Private Sub chkDirectOpening_CheckedChanged(sender As Object, e As EventArgs) Handles chkDirectOpening.CheckedChanged
        If chkDirectOpening.Checked Then
            pnlOpeningDenom.Visible = False
            btnClearOpening.Visible = False
            lblDirectOpeningPrompt.Visible = True
            txtDirectOpening.Visible = True
            txtDirectOpening.Focus()
            txtDirectOpening.SelectAll()
            
            ' Clear denominations
            For Each kvp In openingInputs
                kvp.Value.Text = ""
            Next
            
            lblOpeningTotal.Text = "0.00"
            txtDirectOpening.Text = ""
        Else
            pnlOpeningDenom.Visible = True
            btnClearOpening.Visible = True
            lblDirectOpeningPrompt.Visible = False
            txtDirectOpening.Visible = False
            CalculateOpeningTotal()
        End If
        UpdateClosingExpected()
    End Sub

    Private Sub chkDirectClosing_CheckedChanged(sender As Object, e As EventArgs) Handles chkDirectClosing.CheckedChanged
        If chkDirectClosing.Checked Then
            pnlClosingDenom.Visible = False
            btnClearClosing.Visible = False
            lblDirectClosingPrompt.Visible = True
            txtDirectClosing.Visible = True
            txtDirectClosing.Focus()
            txtDirectClosing.SelectAll()
            
            ' Clear denominations
            For Each kvp In closingInputs
                kvp.Value.Text = ""
            Next
            
            lblActualPhysical.Text = "0.00"
            txtDirectClosing.Text = ""
        Else
            pnlClosingDenom.Visible = True
            btnClearClosing.Visible = True
            lblDirectClosingPrompt.Visible = False
            txtDirectClosing.Visible = False
            CalculateClosingTotal()
        End If
        UpdateClosingExpected()
    End Sub

    Private Sub txtDirectOpening_TextChanged(sender As Object, e As EventArgs) Handles txtDirectOpening.TextChanged
        Dim amt As Decimal = 0
        If Decimal.TryParse(txtDirectOpening.Text, amt) Then
            lblOpeningTotal.Text = amt.ToString("N2")
        Else
            lblOpeningTotal.Text = "0.00"
        End If
        UpdateClosingExpected()
    End Sub

    Private Sub txtDirectClosing_TextChanged(sender As Object, e As EventArgs) Handles txtDirectClosing.TextChanged
        Dim amt As Decimal = 0
        If Decimal.TryParse(txtDirectClosing.Text, amt) Then
            lblActualPhysical.Text = amt.ToString("N2")
        Else
            lblActualPhysical.Text = "0.00"
        End If
        UpdateClosingExpected()
    End Sub

    Private Sub txtDirectOpening_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtDirectOpening.KeyPress
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) AndAlso (e.KeyChar <> "."c) Then
            e.Handled = True
        End If
        If (e.KeyChar = "."c) AndAlso (DirectCast(sender, TextBox).Text.IndexOf("."c) > -1) Then
            e.Handled = True
        End If
    End Sub

    Private Sub txtDirectClosing_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtDirectClosing.KeyPress
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) AndAlso (e.KeyChar <> "."c) Then
            e.Handled = True
        End If
        If (e.KeyChar = "."c) AndAlso (DirectCast(sender, TextBox).Text.IndexOf("."c) > -1) Then
            e.Handled = True
        End If
    End Sub
End Class
