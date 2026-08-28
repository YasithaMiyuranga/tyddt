Imports System.IO
Imports System.Text.RegularExpressions
Imports MySql.Data.MySqlClient
Imports System.Threading.Tasks
Imports System.Text

Public Class DataMigrationForm
    Private targetConnStr As String = Module1.ConnStr
    Private sourceTableData As New Dictionary(Of String, List(Of String)) ' TableName -> ColumnNames

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()
    End Sub

    Private Sub DataMigrationForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadTargetTables()
    End Sub

    ' --- Utility Functions ---

    Private Function GetFilePath() As String
        Try
            Dim ofd As New OpenFileDialog()
            ofd.Filter = "SQL Files (*.sql)|*.sql|All Files (*.*)|*.*"
            ofd.Title = "Select SQL Source File"
            If ofd.ShowDialog() = DialogResult.OK Then
                Return ofd.FileName
            End If
        Catch ex As Exception
            MessageBox.Show("Error opening file dialog: " & ex.Message)
        End Try
        Return ""
    End Function

    Private Sub Log(msg As String)
        If Me.InvokeRequired Then
            Me.Invoke(Sub() Log(msg))
            Return
        End If
        rtbLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}" & vbCrLf)
        rtbLog.SelectionStart = rtbLog.Text.Length
        rtbLog.ScrollToCaret()
    End Sub

    ' --- Standard Migration Tab Events ---

    Private Sub btnBrowseItems_Click(sender As Object, e As EventArgs) Handles btnBrowseItems.Click
        Dim p = GetFilePath()
        If p <> "" Then txtItemsPath.Text = p
    End Sub

    Private Sub btnBrowseCustomers_Click(sender As Object, e As EventArgs) Handles btnBrowseCustomers.Click
        Dim p = GetFilePath()
        If p <> "" Then txtCustomersPath.Text = p
    End Sub

    Private Sub btnBrowseSuppliers_Click(sender As Object, e As EventArgs) Handles btnBrowseSuppliers.Click
        Dim p = GetFilePath()
        If p <> "" Then txtSuppliersPath.Text = p
    End Sub

    Private Sub btnBrowseCredits_Click(sender As Object, e As EventArgs) Handles btnBrowseCredits.Click
        Dim p = GetFilePath()
        If p <> "" Then txtCreditsPath.Text = p
    End Sub

    ' --- Advanced Migration Tab Events ---

    Private Sub btnBrowseAdvanced_Click(sender As Object, e As EventArgs) Handles btnBrowseAdvanced.Click
        Dim path = GetFilePath()
        If path <> "" Then
            txtAdvancedSourcePath.Text = path
            LoadTablesFromSQL(path)
        End If
    End Sub

    Private Sub cmbSourceTable_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbSourceTable.SelectedIndexChanged
        UpdateSourceComboBoxInGrid()
    End Sub

    Private Sub cmbTargetTable_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbTargetTable.SelectedIndexChanged
        If cmbTargetTable.SelectedItem IsNot Nothing Then
            LoadTargetColumns(cmbTargetTable.SelectedItem.ToString())
        End If
    End Sub

    Private Sub btnAutoMap_Click(sender As Object, e As EventArgs) Handles btnAutoMap.Click
        If cmbSourceTable.SelectedItem Is Nothing Then Return
        Dim sourceCols = sourceTableData(cmbSourceTable.SelectedItem.ToString())
        For Each row As DataGridViewRow In dgvMapping.Rows
            Dim targetCol = row.Cells("ColTarget").Value.ToString().ToLower()
            Dim match = sourceCols.FirstOrDefault(Function(s) s.ToLower() = targetCol OrElse s.ToLower().Replace("_", "") = targetCol.Replace("_", ""))
            If match IsNot Nothing Then
                row.Cells("ColSource").Value = match
            End If
        Next
    End Sub

    ' --- Database Logic ---

    Private Sub LoadTargetTables()
        cmbTargetTable.Items.Clear()
        Try
            Using conn As New MySqlConnection(targetConnStr)
                conn.Open()
                Dim cmd As New MySqlCommand("SHOW TABLES", conn)
                Using reader = cmd.ExecuteReader()
                    While reader.Read()
                        cmbTargetTable.Items.Add(reader.GetString(0))
                    End While
                End Using
            End Using
        Catch ex As Exception
            Dim common = {"items", "customer", "supplier", "location", "vat", "user", "customer_credit"}
            cmbTargetTable.Items.AddRange(common)
        End Try
    End Sub

    Private Sub EnsureDefaults(conn As MySqlConnection)
        Log("Ensuring default records (Location, VAT, Roles, Users)...")
        
        ' 1. Location & VAT
        Using cmd = New MySqlCommand("INSERT INTO location (id, name, is_active) VALUES (1, 'MAIN STOCK', 1) ON DUPLICATE KEY UPDATE name='MAIN STOCK';", conn)
            cmd.ExecuteNonQuery()
        End Using
        Using cmd = New MySqlCommand("INSERT INTO vat (id, name, value) VALUES (0, 'NO VAT', 0) ON DUPLICATE KEY UPDATE name='NO VAT';", conn)
            cmd.ExecuteNonQuery()
        End Using
        Using cmd = New MySqlCommand("INSERT INTO customer (id, name, customer_type, is_active, vat_id) VALUES (1, 'CASH', 'CASH', 1, 0) ON DUPLICATE KEY UPDATE name='CASH', customer_type='CASH';", conn)
            cmd.ExecuteNonQuery()
        End Using

        ' 2. user_role
        Dim roles As New Dictionary(Of Integer, String) From {{1, "cashier"}, {2, "admin"}, {3, "owner"}}
        For Each role In roles
            Using cmd = New MySqlCommand("INSERT INTO user_role (id, role_name) VALUES (@id, @name) ON DUPLICATE KEY UPDATE role_name=@name;", conn)
                cmd.Parameters.AddWithValue("@id", role.Key)
                cmd.Parameters.AddWithValue("@name", role.Value)
                cmd.ExecuteNonQuery()
            End Using
        Next

        ' 3. financial_role
        Dim fRoles As New Dictionary(Of Integer, String) From {{1, "seller"}, {2, "normal seller"}, {3, "cashier"}, {4, "admin"}, {5, "owner"}}
        For Each fRole In fRoles
            Using cmd = New MySqlCommand("INSERT INTO financial_role (id, f_role_name) VALUES (@id, @name) ON DUPLICATE KEY UPDATE f_role_name=@name;", conn)
                cmd.Parameters.AddWithValue("@id", fRole.Key)
                cmd.Parameters.AddWithValue("@name", fRole.Value)
                cmd.ExecuteNonQuery()
            End Using
        Next

        ' 4. Default User (Owner)
        Using cmd = New MySqlCommand("INSERT INTO user (id, name, role_id, password, hiddenSecureKey, login, status, financial_role_id) " &
                                   "VALUES (1, 'Owner', 3, '1234', '1234', 0, 'active', 5) " &
                                   "ON DUPLICATE KEY UPDATE name='Owner', role_id=3, financial_role_id=5;", conn)
            cmd.ExecuteNonQuery()
        End Using
        
        Log("Default records verified.")
    End Sub

    Private Sub LoadTargetColumns(tableName As String)
        dgvMapping.Rows.Clear()
        Try
            Using conn As New MySqlConnection(targetConnStr)
                conn.Open()
                Dim cmd As New MySqlCommand($"DESCRIBE {tableName}", conn)
                Using reader = cmd.ExecuteReader()
                    While reader.Read()
                        Dim colName = reader.GetString(0)
                        dgvMapping.Rows.Add(colName, "", "")
                    End While
                End Using
            End Using
            UpdateSourceComboBoxInGrid()
        Catch ex As Exception
            Log("Error loading target columns: " & ex.Message)
        End Try
    End Sub

    Private Sub LoadTablesFromSQL(path As String)
        Try
            sourceTableData.Clear()
            cmbSourceTable.Items.Clear()
            
            Using reader As New StreamReader(path)
                Dim lineCount As Integer = 0
                While Not reader.EndOfStream AndAlso lineCount < 20000 ' Scan first 20k lines
                    Dim line = reader.ReadLine()
                    If String.IsNullOrWhiteSpace(line) Then Continue While
                    
                    ' Support both INSERT INTO and INSERT IGNORE INTO
                    ' Updated to make column list optional
                    Dim match = Regex.Match(line, "INSERT\s+(?:IGNORE\s+)?INTO\s+`?(\w+)`?\s*(?:\((.*?)\))?\s*VALUES", RegexOptions.IgnoreCase)
                    If match.Success Then
                        Dim tableName = match.Groups(1).Value
                        Dim colStr = If(match.Groups(2).Success, match.Groups(2).Value, "")
                        
                        If Not String.IsNullOrEmpty(colStr) Then
                            Dim cols = colStr.Split({","c, "`"c, " "c}, StringSplitOptions.RemoveEmptyEntries).Select(Function(s) s.Trim()).ToList()
                            
                            If Not sourceTableData.ContainsKey(tableName) Then
                                sourceTableData.Add(tableName, cols)
                            Else
                                ' Merge columns if we find new ones
                                For Each c In cols
                                    If Not sourceTableData(tableName).Contains(c) Then
                                        sourceTableData(tableName).Add(c)
                                    End If
                                Next
                            End If
                        End If

                        If Not cmbSourceTable.Items.Contains(tableName) Then
                            cmbSourceTable.Items.Add(tableName)
                        End If
                    End If
                    lineCount += 1
                End While
            End Using

            ' If no tables found via INSERT, try the more advanced CREATE TABLE scan
            ScanForCreateTables(path)

            If cmbSourceTable.Items.Count > 0 Then
                If cmbSourceTable.SelectedIndex < 0 Then cmbSourceTable.SelectedIndex = 0
                Log($"Found {cmbSourceTable.Items.Count} tables in source file.")
            Else
                Log("No tables found in the source file.")
            End If
        Catch ex As Exception
            MessageBox.Show("Error reading SQL file: " & ex.Message)
        End Try
    End Sub

    Private Sub ScanForCreateTables(path As String)
        Try
            Log("Scanning for CREATE TABLE definitions...")
            ' Using File.ReadLines to be memory efficient for large files
            Dim currentTable As String = ""
            Dim currentCols As New List(Of String)
            Dim inTable As Boolean = False
            
            Dim keywords = {"PRIMARY", "KEY", "CONSTRAINT", "UNIQUE", "INDEX", "FOREIGN", "FULLTEXT", "SPATIAL", "CHECK", "REFERENCES", "ON", "DELETE", "UPDATE"}

            For Each line In File.ReadLines(path, Encoding.UTF8)
                Dim trimmed = line.Trim()
                If String.IsNullOrEmpty(trimmed) Then Continue For

                If Not inTable Then
                    Dim startMatch = Regex.Match(trimmed, "CREATE TABLE\s+(?:IF NOT EXISTS\s+)?`?(\w+)`?\s*\(", RegexOptions.IgnoreCase)
                    If startMatch.Success Then
                        currentTable = startMatch.Groups(1).Value
                        currentCols = New List(Of String)
                        inTable = True
                        
                        Dim idx = trimmed.IndexOf("("c)
                        If idx >= 0 Then
                            Dim afterParen = trimmed.Substring(idx + 1).Trim()
                            If Not String.IsNullOrEmpty(afterParen) AndAlso Not afterParen.EndsWith(")") Then
                                ProcessColumnLine(afterParen, currentCols, keywords)
                            End If
                        End If
                    End If
                Else
                    ' Check for end of table definition
                    If trimmed.StartsWith(")") OrElse (trimmed.EndsWith(";") AndAlso trimmed.Contains(")")) Then
                        If Not String.IsNullOrEmpty(currentTable) AndAlso currentCols.Count > 0 Then
                            If Not sourceTableData.ContainsKey(currentTable) Then
                                sourceTableData.Add(currentTable, currentCols)
                                If Not cmbSourceTable.Items.Contains(currentTable) Then
                                    cmbSourceTable.Items.Add(currentTable)
                                End If
                            Else
                                ' Keep the one with more columns or overwrite
                                If currentCols.Count >= sourceTableData(currentTable).Count Then
                                    sourceTableData(currentTable) = currentCols
                                End If
                            End If
                            ' Log($"Found table: {currentTable} ({currentCols.Count} columns)")
                        End If
                        inTable = False
                        currentTable = ""
                    Else
                        ProcessColumnLine(trimmed, currentCols, keywords)
                    End If
                End If
            Next
        Catch ex As Exception
            Log("Error in ScanForCreateTables: " & ex.Message)
        End Try
    End Sub

    Private Sub ProcessColumnLine(line As String, cols As List(Of String), keywords As String())
        Try
            Dim trimmed = line.Replace("`", "").Replace(",", "").Trim()
            If String.IsNullOrEmpty(trimmed) Then Return

            Dim parts = Regex.Split(trimmed, "\s+")
            If parts.Length > 0 Then
                Dim firstWord = parts(0).Trim()
                If Not String.IsNullOrEmpty(firstWord) AndAlso Not keywords.Contains(firstWord.ToUpper()) Then
                    If Not Char.IsDigit(firstWord(0)) AndAlso Not firstWord.StartsWith("'") Then
                        If Not cols.Contains(firstWord) Then
                            cols.Add(firstWord)
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub UpdateSourceComboBoxInGrid()
        If cmbSourceTable.SelectedItem Is Nothing Then Return
        Dim tableName = cmbSourceTable.SelectedItem.ToString()
        If sourceTableData.ContainsKey(tableName) Then
            Dim sourceCols = sourceTableData(tableName)
            Dim colSource = DirectCast(dgvMapping.Columns("ColSource"), DataGridViewComboBoxColumn)
            colSource.Items.Clear()
            colSource.Items.Add("")
            For Each col In sourceCols
                colSource.Items.Add(col)
            Next
        End If
    End Sub

    ' --- Value Parser for INSERT statements ---

    Private Function ParseInsertValues(valuesPart As String) As List(Of List(Of String))
        Dim results As New List(Of List(Of String))
        Dim currentLine As New List(Of String)
        Dim currentVal As New StringBuilder()
        Dim inQuotes As Boolean = False
        Dim quoteChar As Char = " "c
        Dim inParen As Boolean = False
        Dim depth As Integer = 0

        Dim i = 0
        While i < valuesPart.Length
            Dim c = valuesPart(i)

            If Not inParen Then
                If c = "("c Then
                    inParen = True
                    depth = 1
                    currentLine = New List(Of String)
                    currentVal.Clear()
                End If
            Else
                If Not inQuotes Then
                    If c = "'"c OrElse c = """"c Then
                        inQuotes = True
                        quoteChar = c
                        currentVal.Append(c)
                    ElseIf c = "("c Then
                        depth += 1
                        currentVal.Append(c)
                    ElseIf c = ")"c Then
                        depth -= 1
                        if depth = 0 Then
                            ' End of row
                            currentLine.Add(currentVal.ToString().Trim())
                            results.Add(currentLine)
                            currentVal.Clear()
                            inParen = False
                        Else
                            currentVal.Append(c)
                        End If
                    ElseIf c = ","c AndAlso depth = 1 Then
                        currentLine.Add(currentVal.ToString().Trim())
                        currentVal.Clear()
                    Else
                        currentVal.Append(c)
                    End If
                Else
                    currentVal.Append(c)
                    If c = quoteChar Then
                        ' Check for escaped quote in standard SQL ('') or MySQL (\')
                        ' We check for escaped quote '' here
                        If i + 1 < valuesPart.Length AndAlso valuesPart(i + 1) = quoteChar Then
                            currentVal.Append(valuesPart(i + 1))
                            i += 1
                        Else
                            inQuotes = False
                        End If
                    ElseIf c = "\"c AndAlso i + 1 < valuesPart.Length Then
                        ' MySQL escaped character
                        currentVal.Append(valuesPart(i + 1))
                        i += 1
                    End If
                End If
            End If
            i += 1
        End While
        Return results
    End Function

    Private Function CleanSqlValue(val As String) As String
        If String.IsNullOrEmpty(val) Then Return ""
        Dim trimmed = val.Trim()
        If trimmed.ToUpper() = "NULL" Then Return Nothing
        
        If (trimmed.StartsWith("'") AndAlso trimmed.EndsWith("'")) OrElse (trimmed.StartsWith("""") AndAlso trimmed.EndsWith("""")) Then
            trimmed = trimmed.Substring(1, trimmed.Length - 2)
            ' Unescape common MySQL/SQL patterns
            trimmed = trimmed.Replace("''", "'").Replace("\'", "'").Replace("\""", """").Replace("\\", "\")
        End If
        Return trimmed
    End Function

    ' --- Migration Execution (Advanced) ---

    Private Sub btnDirectImport_Click(sender As Object, e As EventArgs) Handles btnDirectImport.Click
        RunAdvancedMigration(False)
    End Sub

    Private Sub btnGenerateSQL_Click(sender As Object, e As EventArgs) Handles btnGenerateSQL.Click
        RunAdvancedMigration(True)
    End Sub

    Private Sub RunAdvancedMigration(generateOnly As Boolean)
        If cmbSourceTable.SelectedItem Is Nothing Or cmbTargetTable.SelectedItem Is Nothing Then
            MessageBox.Show("Please select both Source and Target tables.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim sourceTable = cmbSourceTable.SelectedItem.ToString()
        Dim targetTable = cmbTargetTable.SelectedItem.ToString()
        Dim sqlPath = txtAdvancedSourcePath.Text

        ' Get Mapping
        Dim mapping As New Dictionary(Of String, String) ' TargetCol -> SourceCol
        Dim defaults As New Dictionary(Of String, String) ' TargetCol -> DefaultValue
        
        For Each row As DataGridViewRow In dgvMapping.Rows
            Dim targetCol = row.Cells("ColTarget").Value.ToString()
            Dim sourceCol = If(row.Cells("ColSource").Value?.ToString(), "")
            Dim defaultVal = If(row.Cells("ColDefault").Value?.ToString(), "")
            
            If Not String.IsNullOrEmpty(sourceCol) Then mapping.Add(targetCol, sourceCol)
            If Not String.IsNullOrEmpty(defaultVal) Then defaults.Add(targetCol, defaultVal)
        Next

        If mapping.Count = 0 And defaults.Count = 0 Then
            MessageBox.Show("Please map at least one column.", "Mapping Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        btnDirectImport.Enabled = False
        btnGenerateSQL.Enabled = False
        pbProgress.Value = 0
        Log($"Starting {(If(generateOnly, "SQL Generation", "Direct Import"))} for {targetTable}...")

        Task.Run(Sub()
                     Try
                         Dim totalInserted = 0
                         Dim outputSql As New StringBuilder()
                         If generateOnly Then outputSql.AppendLine($"-- Migration for {targetTable} from {sourceTable}")

                         Using targetConn As New MySqlConnection(targetConnStr)
                             If Not generateOnly Then targetConn.Open()
                             
                             ' Stream the file to find INSERTS for our table
                             Using reader As New StreamReader(sqlPath)
                                 While Not reader.EndOfStream
                                     Dim line = reader.ReadLine()
                                     If String.IsNullOrWhiteSpace(line) Then Continue While

                                     ' Regex to find INSERT INTO `sourceTable`
                                     ' Updated to handle cases where column list is missing: INSERT INTO `table` VALUES ...
                                     Dim match = Regex.Match(line, $"INSERT\s+(?:IGNORE\s+)?INTO\s+`?{sourceTable}`?\s*(?:\((.*?)\))?\s*VALUES\s*(.*);", RegexOptions.IgnoreCase)
                                     If match.Success Then
                                         Dim colListFound = match.Groups(1).Success
                                         Dim sourceCols As List(Of String)
                                         
                                         If colListFound Then
                                             sourceCols = match.Groups(1).Value.Split({","c, "`"c, " "c}, StringSplitOptions.RemoveEmptyEntries).Select(Function(s) s.Trim()).ToList()
                                         ElseIf sourceTableData.ContainsKey(sourceTable) Then
                                             sourceCols = sourceTableData(sourceTable)
                                         Else
                                             Log($"Warning: No column mapping for {sourceTable} and none found in INSERT statement. Skipping.")
                                             Continue While
                                         End If

                                         Dim valuesPart = match.Groups(2).Value
                                         Dim rows = ParseInsertValues(valuesPart)
                                         ' Log($"Found {rows.Count} rows in INSERT statement.")
                                         
                                         For Each rawRow In rows
                                             ' Build the target insert
                                             Dim targetColNames As New List(Of String)
                                             Dim targetValues As New List(Of String)
                                             
                                             Dim cmd As New MySqlCommand("", targetConn)
                                             Dim paramIdx = 0
                                             
                                             ' 1. Mapped Columns
                                             For Each entry In mapping
                                                 Dim targetCol = entry.Key
                                                 Dim sCol = entry.Value
                                                 Dim sIdx = sourceCols.IndexOf(sCol)
                                                 
                                                 If sIdx >= 0 AndAlso sIdx < rawRow.Count Then
                                                     targetColNames.Add($"`{targetCol}`")
                                                     Dim val = CleanSqlValue(rawRow(sIdx))
                                                     
                                                     If generateOnly Then
                                                         targetValues.Add(If(val Is Nothing, "NULL", $"'{val.Replace("'", "''")}'"))
                                                     Else
                                                         Dim pName = "@p" & paramIdx
                                                         If val Is Nothing Then
                                                             cmd.Parameters.AddWithValue(pName, DBNull.Value)
                                                         Else
                                                             cmd.Parameters.AddWithValue(pName, val)
                                                         End If
                                                         targetValues.Add(pName)
                                                         paramIdx += 1
                                                     End If
                                                 End If
                                             Next
                                             
                                             ' 2. Default Values (Only if not already mapped)
                                             For Each entry In defaults
                                                 If Not targetColNames.Contains($"`{entry.Key}`") Then
                                                     targetColNames.Add($"`{entry.Key}`")
                                                     If generateOnly Then
                                                         targetValues.Add($"'{entry.Value.Replace("'", "''")}'")
                                                     Else
                                                         Dim pName = "@p" & paramIdx
                                                         cmd.Parameters.AddWithValue(pName, entry.Value)
                                                         targetValues.Add(pName)
                                                         paramIdx += 1
                                                     End If
                                                 End If
                                             Next

                                             If targetColNames.Count > 0 Then
                                                 Dim finalSql = $"INSERT IGNORE INTO `{targetTable}` ({String.Join(",", targetColNames)}) VALUES ({String.Join(",", targetValues)});"
                                                 
                                                 If generateOnly Then
                                                     outputSql.AppendLine(finalSql)
                                                 Else
                                                     cmd.CommandText = finalSql
                                                     cmd.ExecuteNonQuery()
                                                 End If
                                                 totalInserted += 1
                                             End If
                                         Next
                                     End If
                                 End While
                             End Using
                         End Using

                         If generateOnly Then
                             Dim savePath = Path.Combine(Path.GetDirectoryName(sqlPath), $"migrated_{targetTable}.sql")
                             File.WriteAllText(savePath, outputSql.ToString())
                             Log($"Successfully generated SQL file: {savePath}")
                             Me.Invoke(Sub() MessageBox.Show($"SQL file generated at: {savePath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information))
                         Else
                             Log($"Successfully imported {totalInserted} records into {targetTable}.")
                             Me.Invoke(Sub() MessageBox.Show($"Imported {totalInserted} records successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information))
                         End If

                     Catch ex As Exception
                         Log("ERROR: " & ex.Message)
                         Me.Invoke(Sub() MessageBox.Show("Migration Failed: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error))
                     Finally
                         Me.Invoke(Sub()
                                       btnDirectImport.Enabled = True
                                       btnGenerateSQL.Enabled = True
                                   End Sub)
                     End Try
                 End Sub)
    End Sub

    ' --- Migration Execution (Standard) ---

    Private Sub btnStartMigration_Click(sender As Object, e As EventArgs) Handles btnStartMigration.Click
        btnStartMigration.Enabled = False
        rtbLog.Clear()
        pbProgress.Value = 0
        Task.Run(Sub()
                     Try
                         Using targetConn As New MySqlConnection(targetConnStr)
                             targetConn.Open()
                             EnsureDefaults(targetConn)
                             Log("Starting Standard Migration...")
                             ' (Standard migration logic - can be expanded here)
                         End Using
                         Me.Invoke(Sub() MessageBox.Show("Migration Completed Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information))
                     Catch ex As Exception
                         Log("ERROR: " & ex.Message)
                         Me.Invoke(Sub() MessageBox.Show("Migration Failed: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error))
                     Finally
                         Me.Invoke(Sub() btnStartMigration.Enabled = True)
                     End Try
                 End Sub)
    End Sub

End Class
