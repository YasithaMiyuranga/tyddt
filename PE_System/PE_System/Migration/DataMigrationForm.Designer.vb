<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DataMigrationForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.rtbLog = New System.Windows.Forms.RichTextBox()
        Me.pbProgress = New System.Windows.Forms.ProgressBar()
        Me.btnStartMigration = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.btnBrowseCredits = New System.Windows.Forms.Button()
        Me.txtCreditsPath = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.btnBrowseSuppliers = New System.Windows.Forms.Button()
        Me.txtSuppliersPath = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.btnBrowseCustomers = New System.Windows.Forms.Button()
        Me.txtCustomersPath = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.btnBrowseItems = New System.Windows.Forms.Button()
        Me.txtItemsPath = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.btnGenerateSQL = New System.Windows.Forms.Button()
        Me.btnDirectImport = New System.Windows.Forms.Button()
        Me.dgvMapping = New System.Windows.Forms.DataGridView()
        Me.btnAutoMap = New System.Windows.Forms.Button()
        Me.cmbTargetTable = New System.Windows.Forms.ComboBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.cmbSourceTable = New System.Windows.Forms.ComboBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.btnBrowseAdvanced = New System.Windows.Forms.Button()
        Me.txtAdvancedSourcePath = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.ColSource = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.ColTarget = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ColDefault = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        CType(Me.dgvMapping, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl1.Location = New System.Drawing.Point(0, 0)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(684, 561)
        Me.TabControl1.TabIndex = 0
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.lblStatus)
        Me.TabPage1.Controls.Add(Me.rtbLog)
        Me.TabPage1.Controls.Add(Me.pbProgress)
        Me.TabPage1.Controls.Add(Me.btnStartMigration)
        Me.TabPage1.Controls.Add(Me.GroupBox1)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(676, 535)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Standard Migration"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'lblStatus
        '
        Me.lblStatus.AutoSize = True
        Me.lblStatus.Location = New System.Drawing.Point(18, 206)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(38, 13)
        Me.lblStatus.TabIndex = 9
        Me.lblStatus.Text = "Ready"
        '
        'rtbLog
        '
        Me.rtbLog.BackColor = System.Drawing.Color.Black
        Me.rtbLog.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.rtbLog.ForeColor = System.Drawing.Color.Lime
        Me.rtbLog.Location = New System.Drawing.Point(18, 226)
        Me.rtbLog.Name = "rtbLog"
        Me.rtbLog.ReadOnly = True
        Me.rtbLog.Size = New System.Drawing.Size(640, 290)
        Me.rtbLog.TabIndex = 8
        Me.rtbLog.Text = ""
        '
        'pbProgress
        '
        Me.pbProgress.Location = New System.Drawing.Point(18, 176)
        Me.pbProgress.Name = "pbProgress"
        Me.pbProgress.Size = New System.Drawing.Size(490, 20)
        Me.pbProgress.TabIndex = 7
        '
        'btnStartMigration
        '
        Me.btnStartMigration.BackColor = System.Drawing.Color.SteelBlue
        Me.btnStartMigration.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnStartMigration.ForeColor = System.Drawing.Color.White
        Me.btnStartMigration.Location = New System.Drawing.Point(528, 166)
        Me.btnStartMigration.Name = "btnStartMigration"
        Me.btnStartMigration.Size = New System.Drawing.Size(130, 35)
        Me.btnStartMigration.TabIndex = 6
        Me.btnStartMigration.Text = "Start Migration"
        Me.btnStartMigration.UseVisualStyleBackColor = False
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.btnBrowseCredits)
        Me.GroupBox1.Controls.Add(Me.txtCreditsPath)
        Me.GroupBox1.Controls.Add(Me.Label4)
        Me.GroupBox1.Controls.Add(Me.btnBrowseSuppliers)
        Me.GroupBox1.Controls.Add(Me.txtSuppliersPath)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.btnBrowseCustomers)
        Me.GroupBox1.Controls.Add(Me.txtCustomersPath)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.btnBrowseItems)
        Me.GroupBox1.Controls.Add(Me.txtItemsPath)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Location = New System.Drawing.Point(18, 16)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(640, 144)
        Me.GroupBox1.TabIndex = 5
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Source SQL Files"
        '
        'btnBrowseCredits
        '
        Me.btnBrowseCredits.Location = New System.Drawing.Point(544, 110)
        Me.btnBrowseCredits.Name = "btnBrowseCredits"
        Me.btnBrowseCredits.Size = New System.Drawing.Size(75, 23)
        Me.btnBrowseCredits.TabIndex = 11
        Me.btnBrowseCredits.Text = "Browse..."
        Me.btnBrowseCredits.UseVisualStyleBackColor = True
        '
        'txtCreditsPath
        '
        Me.txtCreditsPath.Location = New System.Drawing.Point(120, 112)
        Me.txtCreditsPath.Name = "txtCreditsPath"
        Me.txtCreditsPath.Size = New System.Drawing.Size(418, 20)
        Me.txtCreditsPath.TabIndex = 10
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(15, 115)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(86, 13)
        Me.Label4.TabIndex = 9
        Me.Label4.Text = "Customer Credits:"
        '
        'btnBrowseSuppliers
        '
        Me.btnBrowseSuppliers.Location = New System.Drawing.Point(544, 80)
        Me.btnBrowseSuppliers.Name = "btnBrowseSuppliers"
        Me.btnBrowseSuppliers.Size = New System.Drawing.Size(75, 23)
        Me.btnBrowseSuppliers.TabIndex = 8
        Me.btnBrowseSuppliers.Text = "Browse..."
        Me.btnBrowseSuppliers.UseVisualStyleBackColor = True
        '
        'txtSuppliersPath
        '
        Me.txtSuppliersPath.Location = New System.Drawing.Point(120, 82)
        Me.txtSuppliersPath.Name = "txtSuppliersPath"
        Me.txtSuppliersPath.Size = New System.Drawing.Size(418, 20)
        Me.txtSuppliersPath.TabIndex = 7
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(15, 85)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(53, 13)
        Me.Label3.TabIndex = 6
        Me.Label3.Text = "Suppliers:"
        '
        'btnBrowseCustomers
        '
        Me.btnBrowseCustomers.Location = New System.Drawing.Point(544, 50)
        Me.btnBrowseCustomers.Name = "btnBrowseCustomers"
        Me.btnBrowseCustomers.Size = New System.Drawing.Size(75, 23)
        Me.btnBrowseCustomers.TabIndex = 5
        Me.btnBrowseCustomers.Text = "Browse..."
        Me.btnBrowseCustomers.UseVisualStyleBackColor = True
        '
        'txtCustomersPath
        '
        Me.txtCustomersPath.Location = New System.Drawing.Point(120, 52)
        Me.txtCustomersPath.Name = "txtCustomersPath"
        Me.txtCustomersPath.Size = New System.Drawing.Size(418, 20)
        Me.txtCustomersPath.TabIndex = 4
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(15, 55)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(59, 13)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "Customers:"
        '
        'btnBrowseItems
        '
        Me.btnBrowseItems.Location = New System.Drawing.Point(544, 20)
        Me.btnBrowseItems.Name = "btnBrowseItems"
        Me.btnBrowseItems.Size = New System.Drawing.Size(75, 23)
        Me.btnBrowseItems.TabIndex = 2
        Me.btnBrowseItems.Text = "Browse..."
        Me.btnBrowseItems.UseVisualStyleBackColor = True
        '
        'txtItemsPath
        '
        Me.txtItemsPath.Location = New System.Drawing.Point(120, 22)
        Me.txtItemsPath.Name = "txtItemsPath"
        Me.txtItemsPath.Size = New System.Drawing.Size(418, 20)
        Me.txtItemsPath.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(15, 25)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(35, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Items:"
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.btnGenerateSQL)
        Me.TabPage2.Controls.Add(Me.btnDirectImport)
        Me.TabPage2.Controls.Add(Me.dgvMapping)
        Me.TabPage2.Controls.Add(Me.btnAutoMap)
        Me.TabPage2.Controls.Add(Me.cmbTargetTable)
        Me.TabPage2.Controls.Add(Me.Label7)
        Me.TabPage2.Controls.Add(Me.cmbSourceTable)
        Me.TabPage2.Controls.Add(Me.Label6)
        Me.TabPage2.Controls.Add(Me.btnBrowseAdvanced)
        Me.TabPage2.Controls.Add(Me.txtAdvancedSourcePath)
        Me.TabPage2.Controls.Add(Me.Label5)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(676, 535)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Advanced Migration"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'btnGenerateSQL
        '
        Me.btnGenerateSQL.Location = New System.Drawing.Point(380, 485)
        Me.btnGenerateSQL.Name = "btnGenerateSQL"
        Me.btnGenerateSQL.Size = New System.Drawing.Size(130, 35)
        Me.btnGenerateSQL.TabIndex = 10
        Me.btnGenerateSQL.Text = "Generate SQL File"
        Me.btnGenerateSQL.UseVisualStyleBackColor = True
        '
        'btnDirectImport
        '
        Me.btnDirectImport.BackColor = System.Drawing.Color.ForestGreen
        Me.btnDirectImport.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnDirectImport.ForeColor = System.Drawing.Color.White
        Me.btnDirectImport.Location = New System.Drawing.Point(528, 485)
        Me.btnDirectImport.Name = "btnDirectImport"
        Me.btnDirectImport.Size = New System.Drawing.Size(130, 35)
        Me.btnDirectImport.TabIndex = 9
        Me.btnDirectImport.Text = "Direct Import"
        Me.btnDirectImport.UseVisualStyleBackColor = False
        '
        'dgvMapping
        '
        Me.dgvMapping.AllowUserToAddRows = False
        Me.dgvMapping.AllowUserToDeleteRows = False
        Me.dgvMapping.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvMapping.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ColTarget, Me.ColSource, Me.ColDefault})
        Me.dgvMapping.Location = New System.Drawing.Point(18, 120)
        Me.dgvMapping.Name = "dgvMapping"
        Me.dgvMapping.RowHeadersVisible = False
        Me.dgvMapping.Size = New System.Drawing.Size(640, 350)
        Me.dgvMapping.TabIndex = 8
        '
        'btnAutoMap
        '
        Me.btnAutoMap.Location = New System.Drawing.Point(544, 83)
        Me.btnAutoMap.Name = "btnAutoMap"
        Me.btnAutoMap.Size = New System.Drawing.Size(114, 23)
        Me.btnAutoMap.TabIndex = 7
        Me.btnAutoMap.Text = "Auto-Map Columns"
        Me.btnAutoMap.UseVisualStyleBackColor = True
        '
        'cmbTargetTable
        '
        Me.cmbTargetTable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbTargetTable.FormattingEnabled = True
        Me.cmbTargetTable.Location = New System.Drawing.Point(380, 85)
        Me.cmbTargetTable.Name = "cmbTargetTable"
        Me.cmbTargetTable.Size = New System.Drawing.Size(150, 21)
        Me.cmbTargetTable.TabIndex = 6
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(300, 88)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(71, 13)
        Me.Label7.TabIndex = 5
        Me.Label7.Text = "Target Table:"
        '
        'cmbSourceTable
        '
        Me.cmbSourceTable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbSourceTable.FormattingEnabled = True
        Me.cmbSourceTable.Location = New System.Drawing.Point(120, 85)
        Me.cmbSourceTable.Name = "cmbSourceTable"
        Me.cmbSourceTable.Size = New System.Drawing.Size(150, 21)
        Me.cmbSourceTable.TabIndex = 4
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(15, 88)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(74, 13)
        Me.Label6.TabIndex = 3
        Me.Label6.Text = "Source Table:"
        '
        'btnBrowseAdvanced
        '
        Me.btnBrowseAdvanced.Location = New System.Drawing.Point(583, 18)
        Me.btnBrowseAdvanced.Name = "btnBrowseAdvanced"
        Me.btnBrowseAdvanced.Size = New System.Drawing.Size(75, 23)
        Me.btnBrowseAdvanced.TabIndex = 2
        Me.btnBrowseAdvanced.Text = "Browse..."
        Me.btnBrowseAdvanced.UseVisualStyleBackColor = True
        '
        'txtAdvancedSourcePath
        '
        Me.txtAdvancedSourcePath.Location = New System.Drawing.Point(120, 20)
        Me.txtAdvancedSourcePath.Name = "txtAdvancedSourcePath"
        Me.txtAdvancedSourcePath.Size = New System.Drawing.Size(457, 20)
        Me.txtAdvancedSourcePath.TabIndex = 1
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(15, 23)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(95, 13)
        Me.Label5.TabIndex = 0
        Me.Label5.Text = "Source SQL File:"
        '
        'ColSource
        '
        Me.ColSource.HeaderText = "Source Column (from file)"
        Me.ColSource.Name = "ColSource"
        Me.ColSource.Width = 200
        '
        'ColTarget
        '
        Me.ColTarget.HeaderText = "Target Column (System)"
        Me.ColTarget.Name = "ColTarget"
        Me.ColTarget.ReadOnly = True
        Me.ColTarget.Width = 200
        '
        'ColDefault
        '
        Me.ColDefault.HeaderText = "Default Value (Optional)"
        Me.ColDefault.Name = "ColDefault"
        Me.ColDefault.Width = 200
        '
        'DataMigrationForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(684, 561)
        Me.Controls.Add(Me.TabControl1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.Name = "DataMigrationForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Advanced Database Migration Utility"
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage1.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.TabPage2.ResumeLayout(False)
        Me.TabPage2.PerformLayout()
        CType(Me.dgvMapping, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents TabPage1 As TabPage
    Friend WithEvents TabPage2 As TabPage
    Friend WithEvents lblStatus As Label
    Friend WithEvents rtbLog As RichTextBox
    Friend WithEvents pbProgress As ProgressBar
    Friend WithEvents btnStartMigration As Button
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents btnBrowseCredits As Button
    Friend WithEvents txtCreditsPath As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents btnBrowseSuppliers As Button
    Friend WithEvents txtSuppliersPath As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents btnBrowseCustomers As Button
    Friend WithEvents txtCustomersPath As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents btnBrowseItems As Button
    Friend WithEvents txtItemsPath As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents btnGenerateSQL As Button
    Friend WithEvents btnDirectImport As Button
    Friend WithEvents dgvMapping As DataGridView
    Friend WithEvents btnAutoMap As Button
    Friend WithEvents cmbTargetTable As ComboBox
    Friend WithEvents Label7 As Label
    Friend WithEvents cmbSourceTable As ComboBox
    Friend WithEvents Label6 As Label
    Friend WithEvents btnBrowseAdvanced As Button
    Friend WithEvents txtAdvancedSourcePath As TextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents ColTarget As DataGridViewTextBoxColumn
    Friend WithEvents ColSource As DataGridViewComboBoxColumn
    Friend WithEvents ColDefault As DataGridViewTextBoxColumn
End Class
