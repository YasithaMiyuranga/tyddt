Imports System.IO
Imports System.Text
Imports System.Diagnostics
Imports MySql.Data.MySqlClient


Public Class BarcodePrint

    Private dtUsers As DataTable

    Private Sub LoadAuthUsers()
        Try
            dtUsers = New DataTable()
            Dim adapter As New MySqlDataAdapter("SELECT id, name, hiddenSecureKey, role_id FROM user WHERE role_id IN (2, 3) ORDER BY name", conn)
            adapter.Fill(dtUsers)
            cmbAuthUser.DataSource = dtUsers
            cmbAuthUser.DisplayMember = "name"
            cmbAuthUser.ValueMember = "id"
            cmbAuthUser.SelectedIndex = -1
        Catch ex As Exception
            MessageBox.Show("Error loading auth users: " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' Public method to add an item to the grid.
    ''' </summary>
    Public Sub AddToGrid(id As String, name As String, description As String, price As String, qty As String, Optional cost As String = "0")
        ' Check if item already exists to avoid duplicates
        For Each row As DataGridViewRow In DataGridViewBarcode.Rows
            If row.Cells("ItemID").Value IsNot Nothing AndAlso row.Cells("ItemID").Value.ToString() = id Then
                ' If exists, increment quantity
                Dim currentQty As Double = 0
                Double.TryParse(row.Cells("PrintQty").Value.ToString(), currentQty)
                Dim addQty As Double = 0
                Double.TryParse(qty, addQty)
                row.Cells("PrintQty").Value = (currentQty + addQty).ToString()
                Return
            End If
        Next

        ' Grid Columns in Designer: ItemID, ItemName, Description, PrintQty, Price
        DataGridViewBarcode.Rows.Add(id, name, description, qty, price)
    End Sub

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        If DataGridViewBarcode.Rows.Count > 0 Then
            If MessageBox.Show("Are you sure you want to clear the entire list?", "Confirm Clear", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                DataGridViewBarcode.Rows.Clear()
            End If
        End If
    End Sub

    Private Sub btnRemove_Click(sender As Object, e As EventArgs) Handles btnRemove.Click
        RemoveSelectedRows()
    End Sub

    Private Sub RemoveSelectedRows()
        If DataGridViewBarcode.SelectedRows.Count > 0 Then
            For Each row As DataGridViewRow In DataGridViewBarcode.SelectedRows
                If Not row.IsNewRow Then
                    DataGridViewBarcode.Rows.Remove(row)
                End If
            Next
        Else
            MessageBox.Show("Please select at least one row to remove.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub DataGridViewBarcode_KeyDown(sender As Object, e As KeyEventArgs) Handles DataGridViewBarcode.KeyDown
        If e.KeyCode = Keys.Delete Then
            RemoveSelectedRows()
        End If
    End Sub

    Private Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click
        ' 1. Check if there are items to print
        If DataGridViewBarcode.Rows.Count = 0 Then
            MessageBox.Show("Please add items to the list first.", "No Items", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            ' 2. Define the path for the CSV file
            Dim folderPath As String = "C:\barcode project"
            Dim filePath As String = Path.Combine(folderPath, "data.csv")

            ' Create folder if not exists
            If Not Directory.Exists(folderPath) Then
                Directory.CreateDirectory(folderPath)
            End If

            ' 3. Create CSV Content (Header must match BarTender Setup)
            Dim csvContent As New StringBuilder()
            csvContent.AppendLine("ItemID,Description,PrintQty,Price") ' Headers

            For Each row As DataGridViewRow In DataGridViewBarcode.Rows
                If Not row.IsNewRow Then
                    Dim id As String = row.Cells("ItemID").Value.ToString()
                    Dim desc As String = row.Cells("Description").Value.ToString()
                    Dim qty As String = row.Cells("PrintQty").Value.ToString()
                    Dim price As String = row.Cells("Price").Value.ToString()

                    ' Format price if needed, here we just take the string
                    csvContent.AppendLine($"{id},{desc},{qty},{price}")
                End If
            Next

            ' 4. Save CSV File
            File.WriteAllText(filePath, csvContent.ToString())

            ' 5. Run BarTender in Background and Print
            ' Note: /AF is your Template Path
            Dim bartendPath As String = "C:\Program Files (x86)\Seagull\BarTender Suite\bartend.exe"
            Dim templatePath As String = "D:\PE System\PE_Stock_Management\Document1.btw" 

            If File.Exists(bartendPath) Then
                Dim startInfo As New ProcessStartInfo()
                startInfo.FileName = bartendPath
                ' Passing the template path to BarTender
                startInfo.Arguments = $"/AF=""{templatePath}"" "

                Process.Start(startInfo)
            Else
                MessageBox.Show("BarTender software not found at: " & bartendPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Print Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub BarcodePrint_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Configure selection for easier deletion
        DataGridViewBarcode.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        DataGridViewBarcode.MultiSelect = True
        
        ' Ensure columns exist if not already in designer
        If DataGridViewBarcode.Columns.Count = 0 Then
            DataGridViewBarcode.Columns.Add("ItemID", "Item ID")
            DataGridViewBarcode.Columns.Add("ItemName", "Item Name")
            DataGridViewBarcode.Columns.Add("Description", "Description")
            DataGridViewBarcode.Columns.Add("PrintQty", "Print Qty")
            DataGridViewBarcode.Columns.Add("Price", "Price")
        End If

        ' Set Price column to read-only initially
        If DataGridViewBarcode.Columns("Price") IsNot Nothing Then
            DataGridViewBarcode.Columns("Price").ReadOnly = True
        End If

        ' Load authorized users
        LoadAuthUsers()

        ' Reset auth status and enable print button
        txtReadOnly.Clear()
        btnPrint.Enabled = True
        cmbAuthUser.Enabled = False
    End Sub

    Private Sub txtReadOnly_TextChanged(sender As Object, e As EventArgs) Handles txtReadOnly.TextChanged
        Dim inputText As String = txtReadOnly.Text.Trim()
        Dim matched As Boolean = False

        If Not String.IsNullOrEmpty(inputText) AndAlso dtUsers IsNot Nothing Then
            For Each row As DataRow In dtUsers.Rows
                Dim key As String = row("hiddenSecureKey").ToString()
                If Not String.IsNullOrEmpty(key) AndAlso key <> "0" AndAlso key = inputText Then
                    Dim roleId As String = row("role_id").ToString()
                    If roleId = "2" OrElse roleId = "3" Then
                        cmbAuthUser.SelectedValue = row("id")
                        
                        ' Make Price editable
                        If DataGridViewBarcode.Columns("Price") IsNot Nothing Then
                            DataGridViewBarcode.Columns("Price").ReadOnly = False
                        End If
                        
                        matched = True
                        Exit For
                    End If
                End If
            Next
        End If

        If Not matched Then
            cmbAuthUser.SelectedIndex = -1
            
            ' Make Price read-only
            If DataGridViewBarcode.Columns("Price") IsNot Nothing Then
                DataGridViewBarcode.Columns("Price").ReadOnly = True
            End If
        End If
    End Sub

End Class