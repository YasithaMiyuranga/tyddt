<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class vat_add
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.VatDataGridView = New System.Windows.Forms.DataGridView()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.ser_vattype = New System.Windows.Forms.ComboBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.ser_vatname = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.btn_active = New System.Windows.Forms.Button()
        Me.btn_inactive = New System.Windows.Forms.Button()
        Me.secure_key = New System.Windows.Forms.TextBox()
        Me.ComboBox3 = New System.Windows.Forms.ComboBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.vat_delete = New System.Windows.Forms.Button()
        Me.vat_edit = New System.Windows.Forms.Button()
        Me.add_vat = New System.Windows.Forms.Button()
        Me.vat_save = New System.Windows.Forms.Button()
        Me.tot_vat = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.vat_type = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.vat_percentage = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.vat_name = New System.Windows.Forms.TextBox()
        Me.lblH2 = New System.Windows.Forms.Label()
        CType(Me.VatDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox4.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox2
        '
        Me.GroupBox2.BackColor = System.Drawing.Color.Chartreuse
        Me.GroupBox2.Location = New System.Drawing.Point(0, 121)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(1264, 14)
        Me.GroupBox2.TabIndex = 1
        Me.GroupBox2.TabStop = False
        '
        'VatDataGridView
        '
        Me.VatDataGridView.AllowUserToAddRows = False
        Me.VatDataGridView.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.VatDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.VatDataGridView.BackgroundColor = System.Drawing.Color.White
        Me.VatDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.VatDataGridView.Location = New System.Drawing.Point(0, 141)
        Me.VatDataGridView.Name = "VatDataGridView"
        Me.VatDataGridView.Size = New System.Drawing.Size(1264, 457)
        Me.VatDataGridView.TabIndex = 3
        '
        'GroupBox4
        '
        Me.GroupBox4.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox4.BackColor = System.Drawing.Color.DarkSlateGray
        Me.GroupBox4.Controls.Add(Me.ser_vattype)
        Me.GroupBox4.Controls.Add(Me.Label5)
        Me.GroupBox4.Controls.Add(Me.ser_vatname)
        Me.GroupBox4.Controls.Add(Me.Label4)
        Me.GroupBox4.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox4.ForeColor = System.Drawing.Color.Yellow
        Me.GroupBox4.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(1264, 62)
        Me.GroupBox4.TabIndex = 2
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Filters"
        '
        'ser_vattype
        '
        Me.ser_vattype.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.ser_vattype.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ser_vattype.FormattingEnabled = True
        Me.ser_vattype.Location = New System.Drawing.Point(721, 19)
        Me.ser_vattype.Name = "ser_vattype"
        Me.ser_vattype.Size = New System.Drawing.Size(174, 28)
        Me.ser_vattype.TabIndex = 15
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.ForeColor = System.Drawing.Color.Yellow
        Me.Label5.Location = New System.Drawing.Point(634, 22)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(79, 21)
        Me.Label5.TabIndex = 14
        Me.Label5.Text = "Vat Type:"
        '
        'ser_vatname
        '
        Me.ser_vatname.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.ser_vatname.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.ser_vatname.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ser_vatname.Location = New System.Drawing.Point(125, 22)
        Me.ser_vatname.Name = "ser_vatname"
        Me.ser_vatname.Size = New System.Drawing.Size(445, 25)
        Me.ser_vatname.TabIndex = 11
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.Color.Yellow
        Me.Label4.Location = New System.Drawing.Point(30, 22)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(89, 21)
        Me.Label4.TabIndex = 6
        Me.Label4.Text = "Vat Name:"
        '
        'GroupBox3
        '
        Me.GroupBox3.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox3.BackColor = System.Drawing.Color.FromArgb(CType(CType(44, Byte), Integer), CType(CType(62, Byte), Integer), CType(CType(80, Byte), Integer))
        Me.GroupBox3.Controls.Add(Me.btn_active)
        Me.GroupBox3.Controls.Add(Me.btn_inactive)
        Me.GroupBox3.Controls.Add(Me.secure_key)
        Me.GroupBox3.Controls.Add(Me.ComboBox3)
        Me.GroupBox3.Controls.Add(Me.Label12)
        Me.GroupBox3.Controls.Add(Me.vat_delete)
        Me.GroupBox3.Controls.Add(Me.vat_edit)
        Me.GroupBox3.Controls.Add(Me.add_vat)
        Me.GroupBox3.Controls.Add(Me.vat_save)
        Me.GroupBox3.Controls.Add(Me.tot_vat)
        Me.GroupBox3.Controls.Add(Me.Label3)
        Me.GroupBox3.Controls.Add(Me.vat_type)
        Me.GroupBox3.Controls.Add(Me.Label2)
        Me.GroupBox3.Controls.Add(Me.Label6)
        Me.GroupBox3.Controls.Add(Me.vat_percentage)
        Me.GroupBox3.Controls.Add(Me.Label1)
        Me.GroupBox3.Controls.Add(Me.vat_name)
        Me.GroupBox3.Controls.Add(Me.lblH2)
        Me.GroupBox3.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(1264, 125)
        Me.GroupBox3.TabIndex = 0
        Me.GroupBox3.TabStop = False
        '
        'btn_active
        '
        Me.btn_active.BackColor = System.Drawing.Color.MediumSeaGreen
        Me.btn_active.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btn_active.FlatAppearance.BorderSize = 0
        Me.btn_active.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btn_active.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btn_active.ForeColor = System.Drawing.Color.White
        Me.btn_active.Location = New System.Drawing.Point(621, 81)
        Me.btn_active.Name = "btn_active"
        Me.btn_active.Size = New System.Drawing.Size(120, 35)
        Me.btn_active.TabIndex = 107
        Me.btn_active.Text = "Set Active"
        Me.btn_active.UseVisualStyleBackColor = False
        Me.btn_active.Visible = False
        '
        'btn_inactive
        '
        Me.btn_inactive.BackColor = System.Drawing.Color.Goldenrod
        Me.btn_inactive.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btn_inactive.FlatAppearance.BorderSize = 0
        Me.btn_inactive.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btn_inactive.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btn_inactive.ForeColor = System.Drawing.Color.White
        Me.btn_inactive.Location = New System.Drawing.Point(621, 81)
        Me.btn_inactive.Name = "btn_inactive"
        Me.btn_inactive.Size = New System.Drawing.Size(120, 35)
        Me.btn_inactive.TabIndex = 108
        Me.btn_inactive.Text = "Set Inactive"
        Me.btn_inactive.UseVisualStyleBackColor = False
        Me.btn_inactive.Visible = False
        '
        'secure_key
        '
        Me.secure_key.BackColor = System.Drawing.Color.FromArgb(CType(CType(44, Byte), Integer), CType(CType(62, Byte), Integer), CType(CType(80, Byte), Integer))
        Me.secure_key.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.secure_key.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.secure_key.Location = New System.Drawing.Point(721, 16)
        Me.secure_key.Name = "secure_key"
        Me.secure_key.Size = New System.Drawing.Size(101, 18)
        Me.secure_key.TabIndex = 106
        Me.secure_key.UseSystemPasswordChar = True
        '
        'ComboBox3
        '
        Me.ComboBox3.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.ComboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBox3.ForeColor = System.Drawing.Color.Black
        Me.ComboBox3.FormattingEnabled = True
        Me.ComboBox3.Location = New System.Drawing.Point(536, 12)
        Me.ComboBox3.Name = "ComboBox3"
        Me.ComboBox3.Size = New System.Drawing.Size(121, 21)
        Me.ComboBox3.TabIndex = 105
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label12.ForeColor = System.Drawing.Color.Yellow
        Me.Label12.Location = New System.Drawing.Point(431, 12)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(99, 21)
        Me.Label12.TabIndex = 104
        Me.Label12.Text = "Select User:"
        '
        'vat_delete
        '
        Me.vat_delete.BackColor = System.Drawing.Color.Crimson
        Me.vat_delete.Cursor = System.Windows.Forms.Cursors.Hand
        Me.vat_delete.FlatAppearance.BorderSize = 0
        Me.vat_delete.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.vat_delete.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.vat_delete.ForeColor = System.Drawing.Color.White
        Me.vat_delete.Location = New System.Drawing.Point(495, 81)
        Me.vat_delete.Name = "vat_delete"
        Me.vat_delete.Size = New System.Drawing.Size(120, 35)
        Me.vat_delete.TabIndex = 25
        Me.vat_delete.Text = "Delete"
        Me.vat_delete.UseVisualStyleBackColor = False
        '
        'vat_edit
        '
        Me.vat_edit.BackColor = System.Drawing.Color.White
        Me.vat_edit.Cursor = System.Windows.Forms.Cursors.Hand
        Me.vat_edit.FlatAppearance.BorderColor = System.Drawing.Color.Silver
        Me.vat_edit.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.vat_edit.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.vat_edit.ForeColor = System.Drawing.Color.Black
        Me.vat_edit.Location = New System.Drawing.Point(369, 81)
        Me.vat_edit.Name = "vat_edit"
        Me.vat_edit.Size = New System.Drawing.Size(120, 35)
        Me.vat_edit.TabIndex = 24
        Me.vat_edit.Text = "Edit"
        Me.vat_edit.UseVisualStyleBackColor = False
        '
        'add_vat
        '
        Me.add_vat.BackColor = System.Drawing.Color.White
        Me.add_vat.Cursor = System.Windows.Forms.Cursors.Hand
        Me.add_vat.FlatAppearance.BorderSize = 0
        Me.add_vat.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.add_vat.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.add_vat.ForeColor = System.Drawing.Color.Black
        Me.add_vat.Location = New System.Drawing.Point(243, 81)
        Me.add_vat.Name = "add_vat"
        Me.add_vat.Size = New System.Drawing.Size(120, 35)
        Me.add_vat.TabIndex = 23
        Me.add_vat.Text = "Add New"
        Me.add_vat.UseVisualStyleBackColor = False
        '
        'vat_save
        '
        Me.vat_save.BackColor = System.Drawing.Color.DodgerBlue
        Me.vat_save.Cursor = System.Windows.Forms.Cursors.Hand
        Me.vat_save.FlatAppearance.BorderSize = 0
        Me.vat_save.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.vat_save.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.vat_save.ForeColor = System.Drawing.Color.White
        Me.vat_save.Location = New System.Drawing.Point(117, 82)
        Me.vat_save.Name = "vat_save"
        Me.vat_save.Size = New System.Drawing.Size(120, 35)
        Me.vat_save.TabIndex = 22
        Me.vat_save.Text = "Save"
        Me.vat_save.UseVisualStyleBackColor = False
        '
        'tot_vat
        '
        Me.tot_vat.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.tot_vat.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.tot_vat.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tot_vat.Location = New System.Drawing.Point(1120, 88)
        Me.tot_vat.Name = "tot_vat"
        Me.tot_vat.Size = New System.Drawing.Size(123, 25)
        Me.tot_vat.TabIndex = 21
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.Yellow
        Me.Label3.Location = New System.Drawing.Point(1033, 88)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(81, 21)
        Me.Label3.TabIndex = 20
        Me.Label3.Text = "Total Vat:"
        '
        'vat_type
        '
        Me.vat_type.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.vat_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.vat_type.FormattingEnabled = True
        Me.vat_type.Location = New System.Drawing.Point(1069, 45)
        Me.vat_type.Name = "vat_type"
        Me.vat_type.Size = New System.Drawing.Size(174, 21)
        Me.vat_type.TabIndex = 19
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.Yellow
        Me.Label2.Location = New System.Drawing.Point(984, 44)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(79, 21)
        Me.Label2.TabIndex = 18
        Me.Label2.Text = "Vat Type:"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.ForeColor = System.Drawing.Color.Yellow
        Me.Label6.Location = New System.Drawing.Point(893, 46)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(24, 21)
        Me.Label6.TabIndex = 17
        Me.Label6.Text = "%"
        '
        'vat_percentage
        '
        Me.vat_percentage.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.vat_percentage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.vat_percentage.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.vat_percentage.Location = New System.Drawing.Point(808, 42)
        Me.vat_percentage.Name = "vat_percentage"
        Me.vat_percentage.Size = New System.Drawing.Size(79, 25)
        Me.vat_percentage.TabIndex = 12
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.Yellow
        Me.Label1.Location = New System.Drawing.Point(673, 45)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(129, 21)
        Me.Label1.TabIndex = 11
        Me.Label1.Text = "Vat Percentage:"
        '
        'vat_name
        '
        Me.vat_name.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.vat_name.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.vat_name.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.vat_name.Location = New System.Drawing.Point(117, 42)
        Me.vat_name.Name = "vat_name"
        Me.vat_name.Size = New System.Drawing.Size(503, 25)
        Me.vat_name.TabIndex = 10
        '
        'lblH2
        '
        Me.lblH2.AutoSize = True
        Me.lblH2.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblH2.ForeColor = System.Drawing.Color.Yellow
        Me.lblH2.Location = New System.Drawing.Point(22, 45)
        Me.lblH2.Name = "lblH2"
        Me.lblH2.Size = New System.Drawing.Size(89, 21)
        Me.lblH2.TabIndex = 5
        Me.lblH2.Text = "Vat Name:"
        '
        'vat_add
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1264, 600)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.GroupBox4)
        Me.Controls.Add(Me.VatDataGridView)
        Me.Controls.Add(Me.GroupBox2)
        Me.Name = "vat_add"
        Me.Text = "vat_add"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        CType(Me.VatDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents VatDataGridView As DataGridView
    Friend WithEvents GroupBox4 As GroupBox
    Friend WithEvents ser_vattype As ComboBox
    Friend WithEvents Label5 As Label
    Friend WithEvents ser_vatname As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents secure_key As TextBox
    Friend WithEvents btn_active As Button
    Friend WithEvents btn_inactive As Button
    Friend WithEvents ComboBox3 As ComboBox
    Friend WithEvents Label12 As Label
    Friend WithEvents vat_delete As Button
    Friend WithEvents vat_edit As Button
    Friend WithEvents add_vat As Button
    Friend WithEvents vat_save As Button
    Friend WithEvents tot_vat As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents vat_type As ComboBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents vat_percentage As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents vat_name As TextBox
    Friend WithEvents lblH2 As Label
End Class
