Imports MySql.Data.MySqlClient

Public Class logs
    Private Sub logs_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Set Grid Design Style
        DataGridView1.DefaultCellStyle.Font = New Font("Microsoft Sans Serif", 12)
        DataGridView1.ColumnHeadersDefaultCellStyle.Font = New Font("Microsoft Sans Serif", 14, FontStyle.Bold)
        DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells

        DataGridView2.DefaultCellStyle.Font = New Font("Microsoft Sans Serif", 12)
        DataGridView2.ColumnHeadersDefaultCellStyle.Font = New Font("Microsoft Sans Serif", 14, FontStyle.Bold)
        DataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells

        DataGridView3.DefaultCellStyle.Font = New Font("Microsoft Sans Serif", 12)
        DataGridView3.ColumnHeadersDefaultCellStyle.Font = New Font("Microsoft Sans Serif", 14, FontStyle.Bold)
        DataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells

        ' Make sure DataGridView2 matches original positioning
        DataGridView2.Location = New Point(9, 10)
        DataGridView2.Size = New Size(1107, 585)

        ' Security check: Add "System Deletions Log" tab ONLY if username is test_owner2
        If String.Equals(Module1.UserName, "test_owner2", StringComparison.OrdinalIgnoreCase) Then
            If TabControl1.TabPages("TabPage4") Is Nothing Then
                Dim TabPage4 As New TabPage()
                TabPage4.Name = "TabPage4"
                TabPage4.Text = "System Deletion Logs"
                TabPage4.BackColor = Color.White

                ' Create unnoticeable text box for the hidden key (borderless, matching background)
                Dim txtHiddenKey As New TextBox()
                txtHiddenKey.Font = New Font("Microsoft Sans Serif", 12)
                txtHiddenKey.Location = New Point(10, 15)
                txtHiddenKey.Size = New Size(100, 20)
                txtHiddenKey.BorderStyle = BorderStyle.None
                txtHiddenKey.BackColor = TabPage4.BackColor
                txtHiddenKey.UseSystemPasswordChar = True
                txtHiddenKey.Name = "txtHiddenKey"
                AddHandler txtHiddenKey.TextChanged, AddressOf txtHiddenKey_TextChanged

                ' Create DataGridView for system deletions
                Dim dgvSystemDeletions As New DataGridView()
                dgvSystemDeletions.Name = "dgvSystemDeletions"
                dgvSystemDeletions.Location = New Point(9, 50)
                dgvSystemDeletions.Size = New Size(1107, 545)
                dgvSystemDeletions.DefaultCellStyle.Font = New Font("Microsoft Sans Serif", 12)
                dgvSystemDeletions.ColumnHeadersDefaultCellStyle.Font = New Font("Microsoft Sans Serif", 14, FontStyle.Bold)
                dgvSystemDeletions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
                dgvSystemDeletions.BackgroundColor = SystemColors.ButtonFace
                dgvSystemDeletions.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
                AddHandler dgvSystemDeletions.CellContentClick, AddressOf dgvSystemDeletions_CellContentClick

                TabPage4.Controls.Add(txtHiddenKey)
                TabPage4.Controls.Add(dgvSystemDeletions)
                TabControl1.TabPages.Add(TabPage4)
            End If
        End If

        load_change_logs()
        load_delete_logs()
        load_add_logs()
        load_system_deletions()
    End Sub

    Private Sub txtHiddenKey_TextChanged(sender As Object, e As EventArgs)
        load_system_deletions()
    End Sub

    ' Refresh data when the form gets focus again
    Private Sub logs_Activated(sender As Object, e As EventArgs) Handles MyBase.Activated
        load_change_logs()
        load_delete_logs()
        load_add_logs()
        load_system_deletions()
    End Sub

    Private Sub load_change_logs()
        Try
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Open()
            Dim filter As String = If(txtFilter IsNot Nothing, txtFilter.Text.Trim(), "")
            Dim query As String = "SELECT l.id, l.item_id, " &
                "IFNULL(l.item_name, i.item_name) as item_name, " &
                "IFNULL(l.description, i.description) as description, " &
                "IFNULL(l.item_cost, i.item_cost) as item_cost, " &
                "IFNULL(l.avg_cost, i.avg_cost) as avg_cost, " &
                "IFNULL(l.selling_price, i.selling_price) as selling_price, " &
                "IFNULL(l.discount, i.discount) as discount, " &
                "IFNULL(l.category_id, i.category_id) as category_id, " &
                "IFNULL(l.brand_id, i.brand_id) as brand_id, " &
                "IFNULL(l.profit_margin, i.profit_margin) as profit_margin, " &
                "IFNULL(l.supply_method, i.supply_method) as supply_method, " &
                "IFNULL(l.measure, i.measure) as measure, " &
                "IFNULL(l.st_qty, i.st_qty) as st_qty, " &
                "IFNULL(l.stock_alert, i.stock_alert) as stock_alert, " &
                "l.changed_at, u.name as changed_by, " &
                "l.item_name as _log_item_name, l.description as _log_description, l.item_cost as _log_item_cost, " &
                "l.avg_cost as _log_avg_cost, l.selling_price as _log_selling_price, l.discount as _log_discount, " &
                "l.category_id as _log_category_id, l.brand_id as _log_brand_id, l.profit_margin as _log_profit_margin, " &
                "l.supply_method as _log_supply_method, l.measure as _log_measure, l.st_qty as _log_st_qty, l.stock_alert as _log_stock_alert " &
                "FROM item_change_log l " &
                "LEFT JOIN items i ON l.item_id = i.id " &
                "LEFT JOIN user u ON l.changed_by = u.id "

            If Not String.IsNullOrEmpty(filter) Then
                query &= "WHERE (l.item_id LIKE @filter OR IFNULL(l.description, i.description) LIKE @filter) "
            End If

            query &= "ORDER BY l.changed_at DESC"

            Dim cmd As New MySqlCommand(query, conn)
            If Not String.IsNullOrEmpty(filter) Then
                cmd.Parameters.AddWithValue("@filter", "%" & filter & "%")
            End If

            Dim adapter As New MySqlDataAdapter(cmd)
            Dim table As New DataTable()
            adapter.Fill(table)
            DataGridView1.DataSource = table

            For Each col As DataGridViewColumn In DataGridView1.Columns
                If col.Name.StartsWith("_log_") Then
                    col.Visible = False
                End If
            Next

            conn.Close()
        Catch ex As Exception
            MessageBox.Show("Error loading change logs: " & ex.Message)
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    ' Reverted to show original item deletion logs
    Private Sub load_delete_logs()
        Try
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Open()
            Dim filter As String = If(txtFilter IsNot Nothing, txtFilter.Text.Trim(), "")
            Dim query As String = "SELECT i.id, i.item_name, i.description, i.item_cost, i.st_qty, l.deleted_at, u.name as deleted_by " &
                                "FROM item_delete_log l " &
                                "JOIN items i ON l.item_id = i.id " &
                                "LEFT JOIN user u ON l.deleted_by = u.id " &
                                "WHERE i.deleted_at IS NOT NULL "

            If Not String.IsNullOrEmpty(filter) Then
                query &= "AND (i.id LIKE @filter OR i.description LIKE @filter) "
            End If

            query &= "ORDER BY l.deleted_at DESC"

            Dim cmd As New MySqlCommand(query, conn)
            If Not String.IsNullOrEmpty(filter) Then
                cmd.Parameters.AddWithValue("@filter", "%" & filter & "%")
            End If

            Dim adapter As New MySqlDataAdapter(cmd)
            Dim table As New DataTable()
            adapter.Fill(table)
            DataGridView2.DataSource = table

            If DataGridView2.Columns("btnRecover") Is Nothing Then
                Dim btn As New DataGridViewButtonColumn()
                btn.HeaderText = "Recover"
                btn.Text = "Restore"
                btn.Name = "btnRecover"
                btn.UseColumnTextForButtonValue = True
                btn.FlatStyle = FlatStyle.Flat
                btn.DefaultCellStyle.BackColor = Color.Red
                btn.DefaultCellStyle.ForeColor = Color.White
                DataGridView2.Columns.Add(btn)
            End If

            conn.Close()
        Catch ex As Exception
            MessageBox.Show("Error loading delete logs: " & ex.Message)
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    ' Loads all system deletions (only for test_owner2 with key 9898)
    Private Sub load_system_deletions()
        Try
            Dim isAuthorized As Boolean = False
            Dim tabPage4 As TabPage = TabControl1.TabPages("TabPage4")
            If tabPage4 IsNot Nothing Then
                Dim txtHiddenKey As TextBox = DirectCast(tabPage4.Controls("txtHiddenKey"), TextBox)
                If txtHiddenKey IsNot Nothing AndAlso txtHiddenKey.Text = "9898" AndAlso String.Equals(Module1.UserName, "test_owner2", StringComparison.OrdinalIgnoreCase) Then
                    isAuthorized = True
                End If
            End If

            Dim dgvSystemDeletions As DataGridView = Nothing
            If tabPage4 IsNot Nothing Then
                dgvSystemDeletions = DirectCast(tabPage4.Controls("dgvSystemDeletions"), DataGridView)
            End If

            If dgvSystemDeletions Is Nothing Then Exit Sub

            If Not isAuthorized Then
                dgvSystemDeletions.DataSource = Nothing
                Exit Sub
            End If

            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Open()
            Dim filter As String = If(txtFilter IsNot Nothing, txtFilter.Text.Trim(), "")
            Dim query As String = "SELECT id AS `Log ID`, entity_type AS `Entity Type`, entity_id AS `Entity ID`, details AS `Details`, deleted_by AS `Deleted By`, deleted_at AS `Deleted At` " &
                                  "FROM system_delete_log "

            If Not String.IsNullOrEmpty(filter) Then
                query &= "WHERE (entity_type LIKE @filter OR entity_id LIKE @filter OR details LIKE @filter OR deleted_by LIKE @filter) "
            End If

            query &= "ORDER BY deleted_at DESC"

            Dim cmd As New MySqlCommand(query, conn)
            If Not String.IsNullOrEmpty(filter) Then
                cmd.Parameters.AddWithValue("@filter", "%" & filter & "%")
            End If

            Dim adapter As New MySqlDataAdapter(cmd)
            Dim table As New DataTable()
            adapter.Fill(table)
            dgvSystemDeletions.DataSource = table

            If dgvSystemDeletions.Columns("btnRecover") Is Nothing Then
                Dim btn As New DataGridViewButtonColumn()
                btn.HeaderText = "Recover"
                btn.Text = "Restore"
                btn.Name = "btnRecover"
                btn.UseColumnTextForButtonValue = True
                btn.FlatStyle = FlatStyle.Flat
                btn.DefaultCellStyle.BackColor = Color.Red
                btn.DefaultCellStyle.ForeColor = Color.White
                dgvSystemDeletions.Columns.Add(btn)
            End If

            conn.Close()
        Catch ex As Exception
            MessageBox.Show("Error loading system deletion logs: " & ex.Message)
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub DataGridView1_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles DataGridView1.CellFormatting
        If e.RowIndex >= 0 Then
            Dim colName As String = DataGridView1.Columns(e.ColumnIndex).Name
            Dim logColName As String = "_log_" & colName

            If DataGridView1.Columns.Contains(logColName) Then
                Dim logVal = DataGridView1.Rows(e.RowIndex).Cells(logColName).Value
                If logVal IsNot DBNull.Value AndAlso logVal IsNot Nothing Then
                    e.CellStyle.BackColor = Color.Red
                    e.CellStyle.ForeColor = Color.White
                End If
            End If
        End If
    End Sub

    ' Reverted back to original item restoration
    Private Sub DataGridView2_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView2.CellContentClick
        If e.ColumnIndex = DataGridView2.Columns("btnRecover").Index AndAlso e.RowIndex >= 0 Then
            Dim itemId As String = DataGridView2.Rows(e.RowIndex).Cells("id").Value.ToString()
            Dim result As DialogResult = MessageBox.Show("Do you want to restore item: " & itemId & "?", "Restore Item", MessageBoxButtons.YesNo)

            If result = DialogResult.Yes Then
                Try
                    If conn.State = ConnectionState.Closed Then conn.Open()
                    Dim query As String = "UPDATE items SET deleted_at = NULL, is_active = 1 WHERE id = '" & itemId & "'"
                    Dim cmd As New MySqlCommand(query, conn)
                    cmd.ExecuteNonQuery()

                    Dim delQuery As String = "DELETE FROM item_delete_log WHERE item_id = '" & itemId & "'"
                    Dim delCmd As New MySqlCommand(delQuery, conn)
                    delCmd.ExecuteNonQuery()

                    ' Also sync system_delete_log
                    Dim systemDelQuery As String = "DELETE FROM system_delete_log WHERE entity_type = 'Item' AND entity_id = '" & itemId & "'"
                    Dim systemDelCmd As New MySqlCommand(systemDelQuery, conn)
                    systemDelCmd.ExecuteNonQuery()

                    conn.Close()
                    MessageBox.Show("Item restored successfully!")

                    Dim itemManageForm As Item_manage = DirectCast(Application.OpenForms("Item_manage"), Item_manage)
                    If itemManageForm IsNot Nothing Then
                        itemManageForm.apply_filters()
                    End If

                    load_delete_logs()
                    load_system_deletions()
                Catch ex As Exception
                    MessageBox.Show("Error restoring item: " & ex.Message)
                    If conn.State = ConnectionState.Open Then conn.Close()
                End Try
            End If
        End If
    End Sub

    ' Recovery on the System Deletion Logs tab page
    Private Sub dgvSystemDeletions_CellContentClick(sender As Object, e As DataGridViewCellEventArgs)
        Dim tabPage4 As TabPage = TabControl1.TabPages("TabPage4")
        If tabPage4 Is Nothing Then Exit Sub
        Dim txtHiddenKey As TextBox = DirectCast(tabPage4.Controls("txtHiddenKey"), TextBox)
        Dim dgvSystemDeletions As DataGridView = DirectCast(tabPage4.Controls("dgvSystemDeletions"), DataGridView)

        Dim isAuthorized As Boolean = (String.Equals(Module1.UserName, "test_owner2", StringComparison.OrdinalIgnoreCase) AndAlso txtHiddenKey IsNot Nothing AndAlso txtHiddenKey.Text = "9898")
        If Not isAuthorized Then Exit Sub

        If e.ColumnIndex = dgvSystemDeletions.Columns("btnRecover").Index AndAlso e.RowIndex >= 0 Then
            Dim entityType As String = dgvSystemDeletions.Rows(e.RowIndex).Cells("Entity Type").Value.ToString()
            Dim entityId As String = dgvSystemDeletions.Rows(e.RowIndex).Cells("Entity ID").Value.ToString()
            Dim logId As Integer = Convert.ToInt32(dgvSystemDeletions.Rows(e.RowIndex).Cells("Log ID").Value)

            If entityType.Equals("Item", StringComparison.OrdinalIgnoreCase) Then
                Dim result As DialogResult = MessageBox.Show("Do you want to restore item: " & entityId & "?", "Restore Item", MessageBoxButtons.YesNo)
                If result = DialogResult.Yes Then
                    Try
                        If conn.State = ConnectionState.Closed Then conn.Open()
                        Using trans = conn.BeginTransaction()
                            Try
                                Dim query As String = "UPDATE items SET deleted_at = NULL, is_active = 1 WHERE id = @id"
                                Using cmd As New MySqlCommand(query, conn, trans)
                                    cmd.Parameters.AddWithValue("@id", entityId)
                                    cmd.ExecuteNonQuery()
                                End Using

                                Dim delQuery As String = "DELETE FROM item_delete_log WHERE item_id = @id"
                                Using cmd As New MySqlCommand(delQuery, conn, trans)
                                    cmd.Parameters.AddWithValue("@id", entityId)
                                    cmd.ExecuteNonQuery()
                                End Using

                                Dim systemDelQuery As String = "DELETE FROM system_delete_log WHERE id = @lid"
                                Using cmd As New MySqlCommand(systemDelQuery, conn, trans)
                                    cmd.Parameters.AddWithValue("@lid", logId)
                                    cmd.ExecuteNonQuery()
                                End Using

                                trans.Commit()
                                MessageBox.Show("Item restored successfully!")
                            Catch ex As Exception
                                trans.Rollback()
                                Throw ex
                            End Try
                        End Using
                        conn.Close()

                        Dim itemManageForm As Item_manage = DirectCast(Application.OpenForms("Item_manage"), Item_manage)
                        If itemManageForm IsNot Nothing Then
                            itemManageForm.apply_filters()
                        End If

                        load_delete_logs()
                        load_system_deletions()
                    Catch ex As Exception
                        MessageBox.Show("Error restoring item: " & ex.Message)
                        If conn.State = ConnectionState.Open Then conn.Close()
                    End Try
                End If
            ElseIf entityType.Equals("Customer", StringComparison.OrdinalIgnoreCase) Then
                Dim result As DialogResult = MessageBox.Show("Do you want to restore customer: " & entityId & "?", "Restore Customer", MessageBoxButtons.YesNo)
                If result = DialogResult.Yes Then
                    Try
                        If conn.State = ConnectionState.Closed Then conn.Open()
                        Using trans = conn.BeginTransaction()
                            Try
                                Dim query As String = "UPDATE customer SET deleted_at = NULL WHERE id = @id"
                                Using cmd As New MySqlCommand(query, conn, trans)
                                    cmd.Parameters.AddWithValue("@id", entityId)
                                    cmd.ExecuteNonQuery()
                                End Using

                                Dim systemDelQuery As String = "DELETE FROM system_delete_log WHERE id = @lid"
                                Using cmd As New MySqlCommand(systemDelQuery, conn, trans)
                                    cmd.Parameters.AddWithValue("@lid", logId)
                                    cmd.ExecuteNonQuery()
                                End Using

                                trans.Commit()
                                MessageBox.Show("Customer restored successfully!")
                            Catch ex As Exception
                                trans.Rollback()
                                Throw ex
                            End Try
                        End Using
                        conn.Close()

                        Dim customerAddForm As customer_add = DirectCast(Application.OpenForms("customer_add"), customer_add)
                        If customerAddForm IsNot Nothing Then
                            customerAddForm.load_Customers_Filtered()
                        End If

                        load_delete_logs()
                        load_system_deletions()
                    Catch ex As Exception
                        MessageBox.Show("Error restoring customer: " & ex.Message)
                        If conn.State = ConnectionState.Open Then conn.Close()
                    End Try
                End If
            Else
                MessageBox.Show("Restoring is not supported for this entity type. Row is permanently deleted from database.", "Restore Not Supported", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End If
    End Sub

    Private Sub load_add_logs()
        Try
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Open()
            Dim filter As String = If(txtFilter IsNot Nothing, txtFilter.Text.Trim(), "")
            Dim query As String =
                "SELECT " &
                "  a.id         AS log_id, " &
                "  a.item_id, " &
                "  a.item_name, " &
                "  a.description, " &
                "  a.item_cost, " &
                "  a.selling_price, " &
                "  a.measure, " &
                "  u.name       AS added_by, " &
                "  a.added_at " &
                "FROM item_add_log a " &
                "LEFT JOIN user u ON a.added_by = u.id "

            If Not String.IsNullOrEmpty(filter) Then
                query &= "WHERE (a.item_id LIKE @filter OR a.description LIKE @filter) "
            End If

            query &= "ORDER BY a.added_at DESC"

            Dim cmd As New MySqlCommand(query, conn)
            If Not String.IsNullOrEmpty(filter) Then
                cmd.Parameters.AddWithValue("@filter", "%" & filter & "%")
            End If

            Dim adapter As New MySqlDataAdapter(cmd)
            Dim table As New DataTable()
            adapter.Fill(table)
            DataGridView3.DataSource = table

            If DataGridView3.Columns.Count > 0 Then
                DataGridView3.Columns("log_id").HeaderText = "Log #"
                DataGridView3.Columns("item_id").HeaderText = "Item ID"
                DataGridView3.Columns("item_name").HeaderText = "Item Name"
                DataGridView3.Columns("description").HeaderText = "Description"
                DataGridView3.Columns("item_cost").HeaderText = "Cost Price"
                DataGridView3.Columns("selling_price").HeaderText = "Sell Price"
                DataGridView3.Columns("measure").HeaderText = "Measure"
                DataGridView3.Columns("added_by").HeaderText = "Added By"
                DataGridView3.Columns("added_at").HeaderText = "Date & Time"
            End If

            conn.Close()
        Catch ex As Exception
            MessageBox.Show("Error loading new-item logs: " & ex.Message)
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub txtFilter_TextChanged(sender As Object, e As EventArgs) Handles txtFilter.TextChanged
        load_change_logs()
        load_delete_logs()
        load_add_logs()
        load_system_deletions()
    End Sub
End Class