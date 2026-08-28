<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class customer_add
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
        Me.MySqlCommand1 = New MySql.Data.MySqlClient.MySqlCommand()
        Me.CustomerDataGridView = New System.Windows.Forms.DataGridView()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblH2 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.cus_name = New System.Windows.Forms.TextBox()
        Me.cus_email = New System.Windows.Forms.TextBox()
        Me.cus_tel = New System.Windows.Forms.TextBox()
        Me.cus_add = New System.Windows.Forms.Button()
        Me.cus_save = New System.Windows.Forms.Button()
        Me.cus_edit = New System.Windows.Forms.Button()
        Me.cus_delete = New System.Windows.Forms.Button()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.cus_tot = New System.Windows.Forms.TextBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.DateTimePicker3 = New System.Windows.Forms.DateTimePicker()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.cus_address = New System.Windows.Forms.TextBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.cus_city = New System.Windows.Forms.TextBox()
        Me.creditlimit = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.addvat = New System.Windows.Forms.Button()
        Me.ComboBox2 = New System.Windows.Forms.ComboBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.ComboBox1 = New System.Windows.Forms.ComboBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.secure_key = New System.Windows.Forms.TextBox()
        Me.ComboBox3 = New System.Windows.Forms.ComboBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.btn_reactivate = New System.Windows.Forms.Button()
        Me.cus_block = New System.Windows.Forms.Button()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.ser_name = New System.Windows.Forms.TextBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.ser_address = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.ser_tel = New System.Windows.Forms.TextBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.print = New System.Windows.Forms.Button()
        Me.GroupBox7 = New System.Windows.Forms.GroupBox()
        Me.FooterPanel = New System.Windows.Forms.Panel()
        CType(Me.CustomerDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.FooterPanel.SuspendLayout()
        Me.SuspendLayout()
        '
        'MySqlCommand1
        '
        Me.MySqlCommand1.CacheAge = 0
        Me.MySqlCommand1.Connection = Nothing
        Me.MySqlCommand1.EnableCaching = False
        Me.MySqlCommand1.Transaction = Nothing
        '
        'CustomerDataGridView
        '
        Me.CustomerDataGridView.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.CustomerDataGridView.BackgroundColor = System.Drawing.Color.White
        Me.CustomerDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.CustomerDataGridView.GridColor = System.Drawing.Color.Black
        Me.CustomerDataGridView.Location = New System.Drawing.Point(0, 138)
        Me.CustomerDataGridView.Margin = New System.Windows.Forms.Padding(2)
        Me.CustomerDataGridView.MultiSelect = False
        Me.CustomerDataGridView.Name = "CustomerDataGridView"
        Me.CustomerDataGridView.ReadOnly = True
        Me.CustomerDataGridView.RowHeadersWidth = 51
        Me.CustomerDataGridView.RowTemplate.Height = 24
        Me.CustomerDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.CustomerDataGridView.Size = New System.Drawing.Size(1264, 332)
        Me.CustomerDataGridView.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(20, 30)
        Me.Label1.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(0, 20)
        Me.Label1.TabIndex = 2
        '
        'lblH2
        '
        Me.lblH2.AutoSize = True
        Me.lblH2.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblH2.ForeColor = System.Drawing.Color.White
        Me.lblH2.Location = New System.Drawing.Point(15, 29)
        Me.lblH2.Name = "lblH2"
        Me.lblH2.Size = New System.Drawing.Size(60, 21)
        Me.lblH2.TabIndex = 3
        Me.lblH2.Text = "Name:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.White
        Me.Label2.Location = New System.Drawing.Point(15, 61)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(74, 21)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Address:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.White
        Me.Label3.Location = New System.Drawing.Point(925, 29)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(109, 21)
        Me.Label3.TabIndex = 5
        Me.Label3.Text = "Phone No(s):"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.Color.White
        Me.Label4.Location = New System.Drawing.Point(520, 29)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(57, 21)
        Me.Label4.TabIndex = 6
        Me.Label4.Text = "Email:"
        '
        'cus_name
        '
        Me.cus_name.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.cus_name.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.cus_name.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cus_name.Location = New System.Drawing.Point(110, 26)
        Me.cus_name.Name = "cus_name"
        Me.cus_name.Size = New System.Drawing.Size(400, 25)
        Me.cus_name.TabIndex = 8
        '
        'cus_email
        '
        Me.cus_email.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.cus_email.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.cus_email.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cus_email.Location = New System.Drawing.Point(580, 26)
        Me.cus_email.Name = "cus_email"
        Me.cus_email.Size = New System.Drawing.Size(335, 25)
        Me.cus_email.TabIndex = 9
        '
        'cus_tel
        '
        Me.cus_tel.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.cus_tel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.cus_tel.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cus_tel.Location = New System.Drawing.Point(1040, 26)
        Me.cus_tel.Name = "cus_tel"
        Me.cus_tel.Size = New System.Drawing.Size(200, 25)
        Me.cus_tel.TabIndex = 10
        '
        'cus_add
        '
        Me.cus_add.BackColor = System.Drawing.Color.White
        Me.cus_add.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cus_add.FlatAppearance.BorderSize = 0
        Me.cus_add.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cus_add.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cus_add.ForeColor = System.Drawing.Color.Black
        Me.cus_add.Location = New System.Drawing.Point(20, 70)
        Me.cus_add.Name = "cus_add"
        Me.cus_add.Size = New System.Drawing.Size(110, 40)
        Me.cus_add.TabIndex = 14
        Me.cus_add.Text = "Add New"
        Me.cus_add.UseVisualStyleBackColor = False
        '
        'cus_save
        '
        Me.cus_save.BackColor = System.Drawing.Color.DodgerBlue
        Me.cus_save.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cus_save.FlatAppearance.BorderSize = 0
        Me.cus_save.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cus_save.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cus_save.ForeColor = System.Drawing.Color.White
        Me.cus_save.Location = New System.Drawing.Point(20, 20)
        Me.cus_save.Name = "cus_save"
        Me.cus_save.Size = New System.Drawing.Size(110, 40)
        Me.cus_save.TabIndex = 15
        Me.cus_save.Text = "Save"
        Me.cus_save.UseVisualStyleBackColor = False
        '
        'cus_edit
        '
        Me.cus_edit.BackColor = System.Drawing.Color.White
        Me.cus_edit.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cus_edit.FlatAppearance.BorderColor = System.Drawing.Color.Silver
        Me.cus_edit.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cus_edit.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cus_edit.ForeColor = System.Drawing.Color.Black
        Me.cus_edit.Location = New System.Drawing.Point(145, 20)
        Me.cus_edit.Name = "cus_edit"
        Me.cus_edit.Size = New System.Drawing.Size(110, 40)
        Me.cus_edit.TabIndex = 16
        Me.cus_edit.Text = "Edit"
        Me.cus_edit.UseVisualStyleBackColor = False
        '
        'cus_delete
        '
        Me.cus_delete.BackColor = System.Drawing.Color.Crimson
        Me.cus_delete.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cus_delete.FlatAppearance.BorderSize = 0
        Me.cus_delete.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cus_delete.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cus_delete.ForeColor = System.Drawing.Color.White
        Me.cus_delete.Location = New System.Drawing.Point(270, 20)
        Me.cus_delete.Name = "cus_delete"
        Me.cus_delete.Size = New System.Drawing.Size(110, 40)
        Me.cus_delete.TabIndex = 17
        Me.cus_delete.Text = "Delete"
        Me.cus_delete.UseVisualStyleBackColor = False
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.ForeColor = System.Drawing.Color.Yellow
        Me.Label8.Location = New System.Drawing.Point(526, 12)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(136, 21)
        Me.Label8.TabIndex = 18
        Me.Label8.Text = "Total Customers:"
        '
        'cus_tot
        '
        Me.cus_tot.BackColor = System.Drawing.Color.White
        Me.cus_tot.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.cus_tot.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cus_tot.Location = New System.Drawing.Point(677, 10)
        Me.cus_tot.Name = "cus_tot"
        Me.cus_tot.Size = New System.Drawing.Size(61, 25)
        Me.cus_tot.TabIndex = 19
        '
        'GroupBox1
        '
        Me.GroupBox1.BackColor = System.Drawing.Color.FromArgb(CType(CType(44, Byte), Integer), CType(CType(62, Byte), Integer), CType(CType(80, Byte), Integer))
        Me.GroupBox1.Controls.Add(Me.DateTimePicker3)
        Me.GroupBox1.Controls.Add(Me.Label13)
        Me.GroupBox1.Controls.Add(Me.cus_address)
        Me.GroupBox1.Controls.Add(Me.Label14)
        Me.GroupBox1.Controls.Add(Me.cus_city)
        Me.GroupBox1.Controls.Add(Me.creditlimit)
        Me.GroupBox1.Controls.Add(Me.Label7)
        Me.GroupBox1.Controls.Add(Me.addvat)
        Me.GroupBox1.Controls.Add(Me.ComboBox2)
        Me.GroupBox1.Controls.Add(Me.Label6)
        Me.GroupBox1.Controls.Add(Me.ComboBox1)
        Me.GroupBox1.Controls.Add(Me.Label5)
        Me.GroupBox1.Controls.Add(Me.cus_email)
        Me.GroupBox1.Controls.Add(Me.Label4)
        Me.GroupBox1.Controls.Add(Me.cus_tel)
        Me.GroupBox1.Controls.Add(Me.cus_name)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.lblH2)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox1.ForeColor = System.Drawing.Color.Yellow
        Me.GroupBox1.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(2)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(2)
        Me.GroupBox1.Size = New System.Drawing.Size(1264, 128)
        Me.GroupBox1.TabIndex = 1
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Add New Customer"
        '
        'DateTimePicker3
        '
        Me.DateTimePicker3.CalendarMonthBackground = System.Drawing.SystemColors.InactiveBorder
        Me.DateTimePicker3.CustomFormat = "yyyy-MM-dd"
        Me.DateTimePicker3.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DateTimePicker3.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DateTimePicker3.Location = New System.Drawing.Point(1040, 89)
        Me.DateTimePicker3.Margin = New System.Windows.Forms.Padding(1)
        Me.DateTimePicker3.Name = "DateTimePicker3"
        Me.DateTimePicker3.Size = New System.Drawing.Size(200, 29)
        Me.DateTimePicker3.TabIndex = 258
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label13.ForeColor = System.Drawing.Color.White
        Me.Label13.Location = New System.Drawing.Point(934, 93)
        Me.Label13.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(104, 20)
        Me.Label13.TabIndex = 125
        Me.Label13.Text = "Credit Period:"
        '
        'cus_address
        '
        Me.cus_address.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.cus_address.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.cus_address.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cus_address.Location = New System.Drawing.Point(110, 59)
        Me.cus_address.Name = "cus_address"
        Me.cus_address.Size = New System.Drawing.Size(805, 25)
        Me.cus_address.TabIndex = 124
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label14.ForeColor = System.Drawing.Color.White
        Me.Label14.Location = New System.Drawing.Point(925, 61)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(44, 21)
        Me.Label14.TabIndex = 125
        Me.Label14.Text = "City:"
        '
        'cus_city
        '
        Me.cus_city.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.cus_city.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.cus_city.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cus_city.Location = New System.Drawing.Point(1040, 59)
        Me.cus_city.Name = "cus_city"
        Me.cus_city.Size = New System.Drawing.Size(200, 25)
        Me.cus_city.TabIndex = 126
        '
        'creditlimit
        '
        Me.creditlimit.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.creditlimit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.creditlimit.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.creditlimit.Location = New System.Drawing.Point(720, 90)
        Me.creditlimit.Name = "creditlimit"
        Me.creditlimit.Size = New System.Drawing.Size(195, 25)
        Me.creditlimit.TabIndex = 123
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.ForeColor = System.Drawing.Color.White
        Me.Label7.Location = New System.Drawing.Point(610, 93)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(103, 21)
        Me.Label7.TabIndex = 122
        Me.Label7.Text = "Credit Limit:"
        '
        'addvat
        '
        Me.addvat.BackColor = System.Drawing.Color.White
        Me.addvat.Cursor = System.Windows.Forms.Cursors.Hand
        Me.addvat.FlatAppearance.BorderSize = 0
        Me.addvat.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.addvat.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.addvat.ForeColor = System.Drawing.Color.Black
        Me.addvat.Location = New System.Drawing.Point(490, 90)
        Me.addvat.Name = "addvat"
        Me.addvat.Size = New System.Drawing.Size(100, 28)
        Me.addvat.TabIndex = 121
        Me.addvat.Text = "Add VAT"
        Me.addvat.UseVisualStyleBackColor = False
        '
        'ComboBox2
        '
        Me.ComboBox2.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.ComboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBox2.ForeColor = System.Drawing.Color.Black
        Me.ComboBox2.FormattingEnabled = True
        Me.ComboBox2.Location = New System.Drawing.Point(395, 90)
        Me.ComboBox2.Name = "ComboBox2"
        Me.ComboBox2.Size = New System.Drawing.Size(85, 28)
        Me.ComboBox2.TabIndex = 26
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.ForeColor = System.Drawing.Color.White
        Me.Label6.Location = New System.Drawing.Point(305, 93)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(88, 21)
        Me.Label6.TabIndex = 25
        Me.Label6.Text = "Select vat:"
        '
        'ComboBox1
        '
        Me.ComboBox1.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.ComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBox1.ForeColor = System.Drawing.Color.Black
        Me.ComboBox1.FormattingEnabled = True
        Me.ComboBox1.Location = New System.Drawing.Point(110, 90)
        Me.ComboBox1.Name = "ComboBox1"
        Me.ComboBox1.Size = New System.Drawing.Size(180, 28)
        Me.ComboBox1.TabIndex = 6
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.ForeColor = System.Drawing.Color.White
        Me.Label5.Location = New System.Drawing.Point(15, 93)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(50, 21)
        Me.Label5.TabIndex = 24
        Me.Label5.Text = "Type:"
        '
        'secure_key
        '
        Me.secure_key.BackColor = System.Drawing.Color.DarkSlateGray
        Me.secure_key.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.secure_key.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.secure_key.Location = New System.Drawing.Point(404, 70)
        Me.secure_key.Name = "secure_key"
        Me.secure_key.Size = New System.Drawing.Size(100, 18)
        Me.secure_key.TabIndex = 29
        Me.secure_key.UseSystemPasswordChar = True
        '
        'ComboBox3
        '
        Me.ComboBox3.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.ComboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBox3.ForeColor = System.Drawing.Color.Black
        Me.ComboBox3.FormattingEnabled = True
        Me.ComboBox3.Location = New System.Drawing.Point(404, 33)
        Me.ComboBox3.Name = "ComboBox3"
        Me.ComboBox3.Size = New System.Drawing.Size(100, 28)
        Me.ComboBox3.TabIndex = 28
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label12.ForeColor = System.Drawing.Color.Yellow
        Me.Label12.Location = New System.Drawing.Point(400, 10)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(99, 21)
        Me.Label12.TabIndex = 27
        Me.Label12.Text = "Select User:"
        '
        'btn_reactivate
        '
        Me.btn_reactivate.BackColor = System.Drawing.Color.YellowGreen
        Me.btn_reactivate.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btn_reactivate.FlatAppearance.BorderSize = 0
        Me.btn_reactivate.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btn_reactivate.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btn_reactivate.ForeColor = System.Drawing.Color.White
        Me.btn_reactivate.Location = New System.Drawing.Point(270, 70)
        Me.btn_reactivate.Name = "btn_reactivate"
        Me.btn_reactivate.Size = New System.Drawing.Size(110, 40)
        Me.btn_reactivate.TabIndex = 23
        Me.btn_reactivate.Text = "ReActive"
        Me.btn_reactivate.UseVisualStyleBackColor = False
        Me.btn_reactivate.Visible = False
        '
        'cus_block
        '
        Me.cus_block.BackColor = System.Drawing.Color.Red
        Me.cus_block.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cus_block.FlatAppearance.BorderSize = 0
        Me.cus_block.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cus_block.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cus_block.ForeColor = System.Drawing.Color.White
        Me.cus_block.Location = New System.Drawing.Point(270, 70)
        Me.cus_block.Name = "cus_block"
        Me.cus_block.Size = New System.Drawing.Size(110, 40)
        Me.cus_block.TabIndex = 22
        Me.cus_block.Text = "Block"
        Me.cus_block.UseVisualStyleBackColor = False
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.ForeColor = System.Drawing.Color.White
        Me.Label9.Location = New System.Drawing.Point(221, 35)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(60, 21)
        Me.Label9.TabIndex = 5
        Me.Label9.Text = "Name:"
        '
        'ser_name
        '
        Me.ser_name.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.ser_name.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.ser_name.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ser_name.Location = New System.Drawing.Point(225, 59)
        Me.ser_name.Name = "ser_name"
        Me.ser_name.Size = New System.Drawing.Size(299, 25)
        Me.ser_name.TabIndex = 7
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label10.ForeColor = System.Drawing.Color.White
        Me.Label10.Location = New System.Drawing.Point(9, 33)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(90, 21)
        Me.Label10.TabIndex = 8
        Me.Label10.Text = "Phone No:"
        '
        'ser_address
        '
        Me.ser_address.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.ser_address.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.ser_address.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ser_address.Location = New System.Drawing.Point(530, 59)
        Me.ser_address.Name = "ser_address"
        Me.ser_address.Size = New System.Drawing.Size(208, 25)
        Me.ser_address.TabIndex = 9
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label11.ForeColor = System.Drawing.Color.White
        Me.Label11.Location = New System.Drawing.Point(526, 33)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(57, 21)
        Me.Label11.TabIndex = 10
        Me.Label11.Text = "Address:"
        '
        'ser_tel
        '
        Me.ser_tel.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.ser_tel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.ser_tel.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ser_tel.Location = New System.Drawing.Point(11, 59)
        Me.ser_tel.Name = "ser_tel"
        Me.ser_tel.Size = New System.Drawing.Size(207, 25)
        Me.ser_tel.TabIndex = 11
        '
        'GroupBox2
        '
        Me.GroupBox2.BackColor = System.Drawing.Color.Chartreuse
        Me.GroupBox2.Location = New System.Drawing.Point(0, 128)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(1264, 10)
        Me.GroupBox2.TabIndex = 4
        Me.GroupBox2.TabStop = False
        '
        'GroupBox4
        '
        Me.GroupBox4.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.GroupBox4.BackColor = System.Drawing.Color.DarkSlateGray
        Me.GroupBox4.Controls.Add(Me.ser_address)
        Me.GroupBox4.Controls.Add(Me.Label11)
        Me.GroupBox4.Controls.Add(Me.ser_tel)
        Me.GroupBox4.Controls.Add(Me.Label9)
        Me.GroupBox4.Controls.Add(Me.ser_name)
        Me.GroupBox4.Controls.Add(Me.Label10)
        Me.GroupBox4.Controls.Add(Me.Label8)
        Me.GroupBox4.Controls.Add(Me.cus_tot)
        Me.GroupBox4.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox4.ForeColor = System.Drawing.Color.Yellow
        Me.GroupBox4.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(747, 120)
        Me.GroupBox4.TabIndex = 5
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Filters"
        '
        'GroupBox5
        '
        Me.GroupBox5.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.GroupBox5.BackColor = System.Drawing.Color.Chartreuse
        Me.GroupBox5.Location = New System.Drawing.Point(744, 0)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Size = New System.Drawing.Size(10, 120)
        Me.GroupBox5.TabIndex = 7
        Me.GroupBox5.TabStop = False
        '
        'GroupBox3
        '
        Me.GroupBox3.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom), System.Windows.Forms.AnchorStyles)
        Me.GroupBox3.BackColor = System.Drawing.Color.DarkSlateGray
        Me.GroupBox3.Controls.Add(Me.print)
        Me.GroupBox3.Controls.Add(Me.cus_save)
        Me.GroupBox3.Controls.Add(Me.secure_key)
        Me.GroupBox3.Controls.Add(Me.cus_add)
        Me.GroupBox3.Controls.Add(Me.ComboBox3)
        Me.GroupBox3.Controls.Add(Me.cus_edit)
        Me.GroupBox3.Controls.Add(Me.Label12)
        Me.GroupBox3.Controls.Add(Me.cus_delete)
        Me.GroupBox3.Controls.Add(Me.btn_reactivate)
        Me.GroupBox3.Controls.Add(Me.cus_block)
        Me.GroupBox3.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox3.ForeColor = System.Drawing.Color.Yellow
        Me.GroupBox3.Location = New System.Drawing.Point(756, 0)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(508, 120)
        Me.GroupBox3.TabIndex = 6
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Buttons"
        '
        'print
        '
        Me.print.BackColor = System.Drawing.Color.White
        Me.print.Cursor = System.Windows.Forms.Cursors.Hand
        Me.print.FlatAppearance.BorderColor = System.Drawing.Color.Silver
        Me.print.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.print.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.print.ForeColor = System.Drawing.Color.Black
        Me.print.Location = New System.Drawing.Point(145, 70)
        Me.print.Name = "print"
        Me.print.Size = New System.Drawing.Size(110, 40)
        Me.print.TabIndex = 122
        Me.print.Text = "Print"
        Me.print.UseVisualStyleBackColor = False
        '
        'GroupBox7
        '
        Me.GroupBox7.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.GroupBox7.BackColor = System.Drawing.Color.Chartreuse
        Me.GroupBox7.Location = New System.Drawing.Point(0, 470)
        Me.GroupBox7.Name = "GroupBox7"
        Me.GroupBox7.Size = New System.Drawing.Size(1262, 10)
        Me.GroupBox7.TabIndex = 8
        Me.GroupBox7.TabStop = False
        '
        'FooterPanel
        '
        Me.FooterPanel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.FooterPanel.BackColor = System.Drawing.Color.DarkSlateGray
        Me.FooterPanel.Controls.Add(Me.GroupBox5)
        Me.FooterPanel.Controls.Add(Me.GroupBox3)
        Me.FooterPanel.Controls.Add(Me.GroupBox4)
        Me.FooterPanel.Location = New System.Drawing.Point(0, 480)
        Me.FooterPanel.Name = "FooterPanel"
        Me.FooterPanel.Size = New System.Drawing.Size(1264, 120)
        Me.FooterPanel.TabIndex = 9
        '
        'customer_add
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1264, 600)
        Me.Controls.Add(Me.GroupBox7)
        Me.Controls.Add(Me.CustomerDataGridView)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.FooterPanel)
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.Name = "customer_add"
        Me.Text = "NewCustomer"
        CType(Me.CustomerDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.FooterPanel.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents MySqlCommand1 As MySql.Data.MySqlClient.MySqlCommand
    Friend WithEvents CustomerDataGridView As DataGridView
    Friend WithEvents Label1 As Label
    Friend WithEvents lblH2 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents cus_name As TextBox
    Friend WithEvents cus_email As TextBox
    Friend WithEvents cus_tel As TextBox
    Friend WithEvents cus_add As Button
    Friend WithEvents cus_save As Button
    Friend WithEvents cus_edit As Button
    Friend WithEvents cus_delete As Button
    Friend WithEvents Label8 As Label
    Friend WithEvents cus_tot As TextBox
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents Label9 As Label
    Friend WithEvents Label10 As Label
    Friend WithEvents ser_name As TextBox
    Friend WithEvents ser_tel As TextBox
    Friend WithEvents Label11 As Label
    Friend WithEvents ser_address As TextBox
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents GroupBox4 As GroupBox
    Friend WithEvents cus_block As Button
    Friend WithEvents btn_reactivate As Button
    Friend WithEvents Label5 As Label
    Friend WithEvents ComboBox1 As ComboBox
    Friend WithEvents ComboBox2 As ComboBox
    Friend WithEvents Label6 As Label
    Friend WithEvents secure_key As TextBox
    Friend WithEvents ComboBox3 As ComboBox
    Friend WithEvents Label12 As Label
    Friend WithEvents addvat As Button
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents GroupBox5 As GroupBox
    Friend WithEvents print As Button
    Friend WithEvents GroupBox7 As GroupBox
    Friend WithEvents FooterPanel As Panel
    Friend WithEvents creditlimit As TextBox
    Friend WithEvents Label7 As Label
    Friend WithEvents cus_address As TextBox
    Friend WithEvents Label14 As Label
    Friend WithEvents cus_city As TextBox
    Friend WithEvents Label13 As Label
    Friend WithEvents DateTimePicker3 As DateTimePicker
End Class
