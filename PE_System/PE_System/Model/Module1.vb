Imports MySql.Data.MySqlClient
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared

Module Module1
    Public Sub FormatReportDecimals(ByRef rpt As ReportDocument)
        If rpt Is Nothing Then Return
        Try
            ' Check if main report is a Sale Invoice or Quotation
            Dim isTwoDecimals As Boolean = TypeOf rpt Is SeleInvoice OrElse TypeOf rpt Is SaleInvoicePOS OrElse TypeOf rpt Is Quate OrElse TypeOf rpt Is QuatePOS
            Dim decimalPlaces As Integer = If(isTwoDecimals, 2, 3)
            Dim roundingFmt As RoundingFormat = If(isTwoDecimals, RoundingFormat.RoundToHundredth, RoundingFormat.RoundToThousandth)

            ' Helper to format a single report definition
            Dim formatObj = Sub(o As ReportObject)
                                If o.Kind = ReportObjectKind.FieldObject Then
                                    Dim field As FieldObject = DirectCast(o, FieldObject)
                                    Dim nameLower As String = field.Name.ToLower()
                                    ' Format prices, costs, amounts, totals, discounts, balances, and credits
                                    If nameLower.Contains("cost") OrElse
                                       nameLower.Contains("price") OrElse
                                       nameLower.Contains("prc") OrElse
                                       nameLower.Contains("rate") OrElse
                                       nameLower.Contains("amount") OrElse
                                       nameLower.Contains("amt") OrElse
                                       nameLower.Contains("rtn") OrElse
                                       nameLower.Contains("return") OrElse
                                       nameLower.Contains("total") OrElse
                                       nameLower.Contains("dis") OrElse
                                       nameLower.Contains("discount") OrElse
                                       nameLower.Contains("payment") OrElse
                                       nameLower.Contains("balance") OrElse
                                       nameLower.Contains("credit") OrElse
                                       nameLower.Contains("debit") OrElse
                                       nameLower.Contains("net") OrElse
                                       nameLower.Contains("gross") OrElse
                                       nameLower.Contains("val") OrElse
                                       nameLower.Contains("subtotal") Then
                                        Try
                                            Dim numFormat As NumericFieldFormat = field.FieldFormat.NumericFormat
                                            field.FieldFormat.CommonFormat.EnableUseSystemDefaults = False
                                            numFormat.DecimalPlaces = decimalPlaces
                                            numFormat.RoundingFormat = roundingFmt
                                        Catch : End Try
                                    End If
                                End If
                            End Sub

            ' Helper to format dynamic quantity formula
            Dim formatFormula = Sub(r As ReportDocument)
                                    Try
                                        Dim formulaField = r.DataDefinition.FormulaFields("QYT")
                                        If formulaField IsNot Nothing Then
                                            Dim formulaText As String = formulaField.Text.Trim()

                                            ' Check which database field is used in the formula and replace it conditionally
                                            If formulaText.Contains("{billing_item1.quantity}") Then
                                                If formulaText.Contains("{items1.measure}") Then
                                                    formulaField.Text = "If {billing_item1.quantity} = Int({billing_item1.quantity}) Then ToText({billing_item1.quantity}, 0) & ' ' & {items1.measure} Else ToText({billing_item1.quantity}, 2) & ' ' & {items1.measure}"
                                                Else
                                                    formulaField.Text = "If {billing_item1.quantity} = Int({billing_item1.quantity}) Then ToText({billing_item1.quantity}, 0) Else ToText({billing_item1.quantity}, 2)"
                                                End If
                                            ElseIf formulaText.Contains("{quotation_billing_item1.quantity}") Then
                                                If formulaText.Contains("{items1.measure}") Then
                                                    formulaField.Text = "If {quotation_billing_item1.quantity} = Int({quotation_billing_item1.quantity}) Then ToText({quotation_billing_item1.quantity}, 0) & ' ' & {items1.measure} Else ToText({quotation_billing_item1.quantity}, 2) & ' ' & {items1.measure}"
                                                Else
                                                    formulaField.Text = "If {quotation_billing_item1.quantity} = Int({quotation_billing_item1.quantity}) Then ToText({quotation_billing_item1.quantity}, 0) Else ToText({quotation_billing_item1.quantity}, 2)"
                                                End If
                                            ElseIf formulaText.Contains("{sales_return_items1.qty}") Then
                                                If formulaText.Contains("{items1.measure}") Then
                                                    formulaField.Text = "If {sales_return_items1.qty} = Int({sales_return_items1.qty}) Then ToText({sales_return_items1.qty}, 0) & ' ' & {items1.measure} Else ToText({sales_return_items1.qty}, 2) & ' ' & {items1.measure}"
                                                Else
                                                    formulaField.Text = "If {sales_return_items1.qty} = Int({sales_return_items1.qty}) Then ToText({sales_return_items1.qty}, 0) Else ToText({sales_return_items1.qty}, 2)"
                                                End If
                                            End If
                                        End If
                                    Catch : End Try
                                End Sub

            ' Apply to main report
            formatFormula(rpt)
            For Each obj As ReportObject In rpt.ReportDefinition.ReportObjects
                formatObj(obj)
            Next

            ' Apply to subreports
            For Each subRpt As ReportDocument In rpt.Subreports
                formatFormula(subRpt)
                For Each obj As ReportObject In subRpt.ReportDefinition.ReportObjects
                    formatObj(obj)
                Next
            Next
        Catch ex As Exception
            ' Silent fail
        End Try
    End Sub

    Public ConnStr As String = "server=localhost;userid=root;password=Yasitha@123;database=stock_management;Convert Zero Datetime=True"
    ' ConnStr As String = "server=localhost;userid=root;password=0616;database=stock_management;Convert Zero Datetime=True"
    ' Public ConnStr As String = "server=192.168.8.196;userid=root;password=SERVER@123;database=stock_management;Convert Zero Datetime=True"
    ' Public ConnStr As String = "server=192.168.1.27;userid=pe_admin;password=Password@123;database=stock_management;Convert Zero Datetime=True"
    ' Public ConnStr As String = "server=192.168.100.1;userid=root;password=DESKTOP-R3HLovi@#;database=stock_management;Convert Zero Datetime=True"
    Public MySqlConn As New MySqlConnection(ConnStr)


    Public conn As New MySqlConnection(ConnStr)
    Public UserRole As String = "" ' Global variable to store logged-in user role (System)
    Public FinancialRole As String = "" ' Global variable to store logged-in user billing role (Order Taker, etc.)

    Public UserName As String = "" ' Global variablE:\OFFICE\new project\Stock_management_ER\PE_Stock_Management\PE_System\PE_System\Model\Module1.vbe to store logged-in user name
    Public CurrentUserID As Integer = 0 ' Global variable to store current user ID
    Public IsDayOpened As Boolean = False ' Global track for day session
    Public IsRgrVisible As Boolean = True ' Global track for hidden RGR bills visibility (Default to Visible)
    Public IsFinanceMenuVisible As Boolean = True ' Global track for sensitive financial reports/menus
    Public IsRgrModeActive As Boolean = False ' Temporary track when saving an RGR bill


    ''' <summary>
    ''' Verifies whether the given password matches any user with role = 'Owner' in the database and is active.
    ''' Returns True on a match, False otherwise.
    ''' </summary>
    Public Function VerifyOwnerPassword(password As String) As Boolean
        Try
            Using localConn As New MySqlConnection(ConnStr)
                localConn.Open()
                Dim sql As String = "SELECT COUNT(*) FROM user u " &
                                   "JOIN user_role r ON u.role_id = r.id " &
                                   "WHERE r.role_name = 'Owner' AND u.password = @pwd AND (u.status IS NULL OR u.status = 'active')"
                Using cmd As New MySqlCommand(sql, localConn)
                    cmd.Parameters.AddWithValue("@pwd", password)
                    Return Convert.ToInt32(cmd.ExecuteScalar()) > 0
                End Using
            End Using
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Sub db_connection()
        Try
            If MySqlConn.State = ConnectionState.Closed Then
                MySqlConn.Open()
                Dim createTableSql As String = "CREATE TABLE IF NOT EXISTS system_delete_log (" &
                                               "id INT AUTO_INCREMENT PRIMARY KEY, " &
                                               "entity_type VARCHAR(50) NOT NULL, " &
                                               "entity_id VARCHAR(100) NOT NULL, " &
                                               "details TEXT, " &
                                               "deleted_by VARCHAR(100), " &
                                               "deleted_at DATETIME NOT NULL" &
                                               ")"
                Using cmd As New MySqlCommand(createTableSql, MySqlConn)
                    cmd.ExecuteNonQuery()
                End Using
                MySqlConn.Close()
            End If
        Catch ex As Exception
            MessageBox.Show("Connection failed: " & ex.Message, "Database Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public Function GetCurrentDrawerStock() As Dictionary(Of Integer, Integer)
        Dim stock As New Dictionary(Of Integer, Integer)
        Dim denoms() As Integer = {5000, 2000, 1000, 500, 100, 50, 20, 10, 5, 2, 1}
        For Each d In denoms : stock(d) = 0 : Next

        Try
            Using localConn As New MySqlConnection(ConnStr)
                localConn.Open()

                ' 1. Get Opening Counts for today
                Dim sqlOpening = "SELECT d5000, d2000, d1000, d500, d100, d50, d20, d10, d5, d2, d1 " &
                                 "FROM cash_drawer_history WHERE DATE(log_date) = CURDATE() AND log_type = 'OPENING' LIMIT 1"
                Using cmdO = New MySqlCommand(sqlOpening, localConn)
                    Using drO = cmdO.ExecuteReader()
                        If drO.Read() Then
                            For Each d In denoms
                                stock(d) = If(IsDBNull(drO("d" & d)), 0, Convert.ToInt32(drO("d" & d)))
                            Next
                        End If
                    End Using
                End Using

                ' 2. Subtract Change/OUT denominations given today
                ' We look for 'OUT' transactions in petty_cash and their linked denomination_records
                Dim sqlOut = "SELECT dr.d5000, dr.d2000, dr.d1000, dr.d500, dr.d100, dr.d50, dr.d20, dr.d10, dr.d5, dr.d2, dr.d1 " &
                             "FROM denomination_records dr " &
                             "JOIN petty_cash pc ON dr.ref_id = pc.id AND dr.ref_type = 'PETTY_CASH' " &
                             "WHERE DATE(pc.date) = CURDATE() AND pc.transaction_type = 'OUT'"
                Using cmdExp = New MySqlCommand(sqlOut, localConn)
                    Using drE = cmdExp.ExecuteReader()
                        While drE.Read()
                            For Each d In denoms
                                stock(d) -= If(IsDBNull(drE("d" & d)), 0, Convert.ToInt32(drE("d" & d)))
                            Next
                        End While
                    End Using
                End Using

                ' 3. Add any 'IN' denominations (if we track them - e.g. from Petty Cash IN)
                Dim sqlIn = "SELECT dr.d5000, dr.d2000, dr.d1000, dr.d500, dr.d100, dr.d50, dr.d20, dr.d10, dr.d5, dr.d2, dr.d1 " &
                            "FROM denomination_records dr " &
                            "JOIN petty_cash pc ON dr.ref_id = pc.id AND dr.ref_type = 'PETTY_CASH' " &
                            "WHERE DATE(pc.date) = CURDATE() AND pc.transaction_type = 'IN'"
                Using cmdIn = New MySqlCommand(sqlIn, localConn)
                    Using drI = cmdIn.ExecuteReader()
                        While drI.Read()
                            For Each d In denoms
                                stock(d) += If(IsDBNull(drI("d" & d)), 0, Convert.ToInt32(drI("d" & d)))
                            Next
                        End While
                    End Using
                End Using

            End Using
        Catch ex As Exception
            Console.WriteLine("GetCurrentDrawerStock Error: " & ex.Message)
        End Try
        Return stock
    End Function

    Public Function CheckDayOpening() As Boolean
        Try
            Using localConn As New MySqlConnection(ConnStr)
                localConn.Open()
                ' Logic: Latest log_type for today should be 'OPENING' to be considered 'Opened'
                ' If latest is 'CLOSING', then day is closed.
                Dim sql = "SELECT log_type FROM cash_drawer_history WHERE DATE(log_date) = CURDATE() ORDER BY id DESC LIMIT 1"
                Using cmd = New MySqlCommand(sql, localConn)
                    Dim result = cmd.ExecuteScalar()
                    IsDayOpened = (result IsNot Nothing AndAlso result.ToString() = "OPENING")
                End Using
            End Using
        Catch ex As Exception
            IsDayOpened = False
        End Try
        Return IsDayOpened
    End Function

    Public Sub RegisterCashTransaction(amount As Decimal, type As String, description As String, Optional refNo As String = "", Optional denominations As Dictionary(Of Integer, Integer) = Nothing, Optional customDate As String = "")
        Try
            If amount <= 0 Then Return
            Using localConn As New MySqlConnection(ConnStr)
                localConn.Open()

                ' 1. Insert into petty_cash
                Dim dateValue As String = If(String.IsNullOrEmpty(customDate), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), customDate)
                Dim sqlPetty = "INSERT INTO petty_cash (item_name, amount, transaction_type, item_type, date, user, receipt_no, source, machine) " &
                               "VALUES (@iname, @amt, @ttype, 'SYSTEM', @dt, @usr, @rno, 'Cash Box', @mch); SELECT LAST_INSERT_ID();"
                Dim pettyId As Integer = 0
                Using cmd = New MySqlCommand(sqlPetty, localConn)
                    cmd.Parameters.AddWithValue("@iname", description)
                    cmd.Parameters.AddWithValue("@amt", amount)
                    cmd.Parameters.AddWithValue("@ttype", type) ' IN or OUT
                    cmd.Parameters.AddWithValue("@dt", dateValue)
                    cmd.Parameters.AddWithValue("@usr", UserName)
                    cmd.Parameters.AddWithValue("@rno", refNo)
                    cmd.Parameters.AddWithValue("@mch", Environment.MachineName)
                    pettyId = Convert.ToDecimal(cmd.ExecuteScalar())
                End Using

                ' 2. Insert into denomination_records (if provided)
                If denominations IsNot Nothing AndAlso denominations.Count > 0 Then
                    Dim sqlDenom = "INSERT INTO denomination_records (ref_type, ref_id, d5000, d2000, d1000, d500, d100, d50, d20, d10, d5, d2, d1, total_amount) " &
                                  "VALUES ('PETTY_CASH', @pid, @d5000, @d2000, @d1000, @d500, @d100, @d50, @d20, @d10, @d5, @d2, @d1, @total)"
                    Using cmdD = New MySqlCommand(sqlDenom, localConn)
                        cmdD.Parameters.AddWithValue("@pid", pettyId)
                        Dim denoms() As Integer = {5000, 2000, 1000, 500, 100, 50, 20, 10, 5, 2, 1}
                        For Each d In denoms
                            Dim count = If(denominations.ContainsKey(d), denominations(d), 0)
                            cmdD.Parameters.AddWithValue("@d" & d, count)
                        Next
                        cmdD.Parameters.AddWithValue("@total", amount)
                        cmdD.ExecuteNonQuery()
                    End Using
                End If
            End Using
        Catch ex As Exception
            Console.WriteLine("RegisterCashTransaction Error: " & ex.Message)
        End Try
    End Sub
    Public Sub SyncItemMasterData(itemId As String)
        Using localConn As New MySqlConnection(ConnStr)
            Try
                localConn.Open()
                ' 1. Get Oldest Supplier ID
                Dim oldestSid As Object = DBNull.Value
                Dim sidSql = "SELECT supplier_id FROM items_stock WHERE item_id = @id AND st_qty > 0 ORDER BY date ASC LIMIT 1"
                Using cmdSid = New MySqlCommand(sidSql, localConn)
                    cmdSid.Parameters.AddWithValue("@id", itemId)
                    oldestSid = cmdSid.ExecuteScalar()
                End Using

                ' 2. Get Latest Costs and Prices
                Dim maxSql = "SELECT item_cost as m_cost, avg_cost as m_avg, selling_price as m_sell, whole_selling_price as m_whole, retail_selling_price as m_retail " &
                             "FROM items_stock WHERE item_id = @id ORDER BY date DESC, id DESC LIMIT 1"
                Using cmdMax = New MySqlCommand(maxSql, localConn)
                    cmdMax.Parameters.AddWithValue("@id", itemId)
                    Using dr = cmdMax.ExecuteReader()
                        If dr.Read() AndAlso Not IsDBNull(dr("m_cost")) Then
                            Dim mCost = dr("m_cost")
                            Dim mAvg = If(IsDBNull(dr("m_avg")), 0, dr("m_avg"))
                            Dim mSell = dr("m_sell")
                            Dim mWhole = dr("m_whole")
                            Dim mRetail = dr("m_retail")
                            dr.Close()

                            ' Update items table
                            Dim updateSql = "UPDATE items SET supplier_id = @sid, item_cost = @ic, avg_cost = @ac, selling_price = @sp, whole_selling_price = @wp, retail_selling_price = @rp WHERE id = @id"
                            Using cmdUp = New MySqlCommand(updateSql, localConn)
                                cmdUp.Parameters.AddWithValue("@sid", If(oldestSid Is Nothing, DBNull.Value, oldestSid))
                                cmdUp.Parameters.AddWithValue("@ic", mCost)
                                cmdUp.Parameters.AddWithValue("@ac", mAvg)
                                cmdUp.Parameters.AddWithValue("@sp", mSell)
                                cmdUp.Parameters.AddWithValue("@wp", mWhole)
                                cmdUp.Parameters.AddWithValue("@rp", mRetail)
                                cmdUp.Parameters.AddWithValue("@id", itemId)
                                cmdUp.ExecuteNonQuery()
                            End Using

                            ' Update prices in existing current stock batches
                            Dim updateStockSql = "UPDATE items_stock SET avg_cost = @ac, selling_price = @sp, whole_selling_price = @wp, retail_selling_price = @rp WHERE item_id = @id AND st_qty > 0"
                            Using cmdUpStock = New MySqlCommand(updateStockSql, localConn)
                                cmdUpStock.Parameters.AddWithValue("@ac", mAvg)
                                cmdUpStock.Parameters.AddWithValue("@sp", mSell)
                                cmdUpStock.Parameters.AddWithValue("@wp", mWhole)
                                cmdUpStock.Parameters.AddWithValue("@rp", mRetail)
                                cmdUpStock.Parameters.AddWithValue("@id", itemId)
                                cmdUpStock.ExecuteNonQuery()
                            End Using
                        End If
                    End Using
                End Using
            Catch ex As Exception
                Console.WriteLine("SyncItemMasterData Error: " & ex.Message)
            End Try
        End Using
    End Sub

    ''' <summary>
    ''' Enables double buffering for a DataGridView to reduce flickering/lag during scrolling.
    ''' </summary>
    Public Sub EnableDoubleBuffered(ByVal dgv As DataGridView)
        Try
            Dim dgvType As Type = dgv.GetType()
            Dim pi As System.Reflection.PropertyInfo = dgvType.GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.NonPublic)
            pi.SetValue(dgv, True, Nothing)
        Catch ex As Exception
            Console.WriteLine("EnableDoubleBuffered Error: " & ex.Message)
        End Try
    End Sub
    ''' <summary>
    ''' Converts a plain string to Code 128 encoded string compatible with professional Code 128 fonts.
    ''' </summary>
    Public Function GetCode128(ByVal input As String) As String
        If String.IsNullOrEmpty(input) Then Return ""

        Dim charList As New System.Collections.Generic.List(Of Integer)
        Dim checkSum As Integer = 104 ' Start code B
        charList.Add(checkSum)

        For i As Integer = 0 To input.Length - 1
            Dim charVal As Integer = Asc(input(i))
            Dim codeVal As Integer = 0
            If charVal >= 32 AndAlso charVal <= 126 Then
                codeVal = charVal - 32
            Else
                codeVal = 0
            End If
            checkSum += (codeVal * (i + 1))
            charList.Add(codeVal)
        Next

        checkSum = checkSum Mod 103
        charList.Add(checkSum)
        charList.Add(106) ' Stop code

        Dim result As String = ""
        For Each cVal In charList
            If cVal <= 94 Then
                result &= Chr(cVal + 32)
            ElseIf cVal <= 106 Then
                result &= Chr(cVal + 100)
            End If
        Next
        Return result
    End Function

    Public Sub LogDeletion(entityType As String, entityId As String, details As String)
        Try
            Using localConn As New MySqlConnection(ConnStr)
                localConn.Open()
                Dim sql As String = "INSERT INTO system_delete_log (entity_type, entity_id, details, deleted_by, deleted_at) " &
                                    "VALUES (@type, @id, @details, @user, NOW())"
                Using cmd As New MySqlCommand(sql, localConn)
                    cmd.Parameters.AddWithValue("@type", entityType)
                    cmd.Parameters.AddWithValue("@id", entityId)
                    cmd.Parameters.AddWithValue("@details", details)
                    cmd.Parameters.AddWithValue("@user", If(String.IsNullOrEmpty(UserName), "System", UserName))
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("LogDeletion Error: " & ex.Message)
        End Try
    End Sub

End Module
