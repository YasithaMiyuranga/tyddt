<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Suplier
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
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.DateTimePicker3 = New System.Windows.Forms.DateTimePicker()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.DebitLimitTxt = New System.Windows.Forms.TextBox()
        Me.LabelDebitLimit = New System.Windows.Forms.Label()
        Me.EmailTxt = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.TelNoTxt = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.AddressTxt = New System.Windows.Forms.TextBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.NameTxt = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.BottomPanel = New System.Windows.Forms.Panel()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.Addbtn = New System.Windows.Forms.Button()
        Me.Print = New System.Windows.Forms.Button()
        Me.Editbtn = New System.Windows.Forms.Button()
        Me.deletebtn = New System.Windows.Forms.Button()
        Me.savebtn = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.ComboBox3 = New System.Windows.Forms.ComboBox()
        Me.secure_key = New System.Windows.Forms.TextBox()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.Nameserch = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TelSearch = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.Supplier_DataGridView = New System.Windows.Forms.DataGridView()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.Cachedreceiving_stock_report1 = New PE_System.Cachedreceiving_stock_report()
        Me.GroupBox7 = New System.Windows.Forms.GroupBox()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.Panel3.SuspendLayout()
        Me.BottomPanel.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Supplier_DataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Panel3
        '
        Me.Panel3.BackColor = System.Drawing.Color.FromArgb(CType(CType(44, Byte), Integer), CType(CType(62, Byte), Integer), CType(CType(80, Byte), Integer))
        Me.Panel3.Controls.Add(Me.DateTimePicker3)
        Me.Panel3.Controls.Add(Me.Label2)
        Me.Panel3.Controls.Add(Me.DebitLimitTxt)
        Me.Panel3.Controls.Add(Me.LabelDebitLimit)
        Me.Panel3.Controls.Add(Me.EmailTxt)
        Me.Panel3.Controls.Add(Me.Label8)
        Me.Panel3.Controls.Add(Me.TelNoTxt)
        Me.Panel3.Controls.Add(Me.Label9)
        Me.Panel3.Controls.Add(Me.AddressTxt)
        Me.Panel3.Controls.Add(Me.Label10)
        Me.Panel3.Controls.Add(Me.NameTxt)
        Me.Panel3.Controls.Add(Me.Label11)
        Me.Panel3.Location = New System.Drawing.Point(0, 0)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(1264, 84)
        Me.Panel3.TabIndex = 2
        '
        'DateTimePicker3
        '
        Me.DateTimePicker3.CalendarMonthBackground = System.Drawing.SystemColors.InactiveBorder
        Me.DateTimePicker3.CustomFormat = "yyyy-MM-dd"
        Me.DateTimePicker3.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DateTimePicker3.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DateTimePicker3.Location = New System.Drawing.Point(1054, 42)
        Me.DateTimePicker3.Margin = New System.Windows.Forms.Padding(1)
        Me.DateTimePicker3.Name = "DateTimePicker3"
        Me.DateTimePicker3.Size = New System.Drawing.Size(166, 29)
        Me.DateTimePicker3.TabIndex = 257
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.White
        Me.Label2.Location = New System.Drawing.Point(951, 47)
        Me.Label2.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(100, 20)
        Me.Label2.TabIndex = 40
        Me.Label2.Text = "Debit Period:"
        '
        'DebitLimitTxt
        '
        Me.DebitLimitTxt.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.DebitLimitTxt.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DebitLimitTxt.Location = New System.Drawing.Point(767, 44)
        Me.DebitLimitTxt.Margin = New System.Windows.Forms.Padding(2)
        Me.DebitLimitTxt.Name = "DebitLimitTxt"
        Me.DebitLimitTxt.Size = New System.Drawing.Size(169, 27)
        Me.DebitLimitTxt.TabIndex = 39
        '
        'LabelDebitLimit
        '
        Me.LabelDebitLimit.AutoSize = True
        Me.LabelDebitLimit.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelDebitLimit.ForeColor = System.Drawing.Color.White
        Me.LabelDebitLimit.Location = New System.Drawing.Point(672, 47)
        Me.LabelDebitLimit.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.LabelDebitLimit.Name = "LabelDebitLimit"
        Me.LabelDebitLimit.Size = New System.Drawing.Size(91, 20)
        Me.LabelDebitLimit.TabIndex = 22
        Me.LabelDebitLimit.Text = "Debit Limit:"
        '
        'EmailTxt
        '
        Me.EmailTxt.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.EmailTxt.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.EmailTxt.Location = New System.Drawing.Point(360, 44)
        Me.EmailTxt.Margin = New System.Windows.Forms.Padding(2)
        Me.EmailTxt.Name = "EmailTxt"
        Me.EmailTxt.Size = New System.Drawing.Size(308, 27)
        Me.EmailTxt.TabIndex = 21
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.ForeColor = System.Drawing.Color.White
        Me.Label8.Location = New System.Drawing.Point(286, 44)
        Me.Label8.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(51, 20)
        Me.Label8.TabIndex = 20
        Me.Label8.Text = "Email:"
        '
        'TelNoTxt
        '
        Me.TelNoTxt.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TelNoTxt.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TelNoTxt.Location = New System.Drawing.Point(62, 44)
        Me.TelNoTxt.Margin = New System.Windows.Forms.Padding(2)
        Me.TelNoTxt.Name = "TelNoTxt"
        Me.TelNoTxt.Size = New System.Drawing.Size(209, 27)
        Me.TelNoTxt.TabIndex = 19
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.ForeColor = System.Drawing.Color.White
        Me.Label9.Location = New System.Drawing.Point(7, 47)
        Me.Label9.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(58, 20)
        Me.Label9.TabIndex = 18
        Me.Label9.Text = "Tel No:"
        '
        'AddressTxt
        '
        Me.AddressTxt.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.AddressTxt.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.AddressTxt.Location = New System.Drawing.Point(360, 9)
        Me.AddressTxt.Margin = New System.Windows.Forms.Padding(2)
        Me.AddressTxt.Name = "AddressTxt"
        Me.AddressTxt.Size = New System.Drawing.Size(860, 27)
        Me.AddressTxt.TabIndex = 17
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label10.ForeColor = System.Drawing.Color.White
        Me.Label10.Location = New System.Drawing.Point(286, 9)
        Me.Label10.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(70, 20)
        Me.Label10.TabIndex = 16
        Me.Label10.Text = "Address:"
        '
        'NameTxt
        '
        Me.NameTxt.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.NameTxt.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.NameTxt.Location = New System.Drawing.Point(62, 6)
        Me.NameTxt.Margin = New System.Windows.Forms.Padding(2)
        Me.NameTxt.Name = "NameTxt"
        Me.NameTxt.Size = New System.Drawing.Size(209, 27)
        Me.NameTxt.TabIndex = 15
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label11.ForeColor = System.Drawing.Color.White
        Me.Label11.Location = New System.Drawing.Point(7, 5)
        Me.Label11.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(55, 20)
        Me.Label11.TabIndex = 14
        Me.Label11.Text = "Name:"
        '
        'BottomPanel
        '
        Me.BottomPanel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.BottomPanel.Controls.Add(Me.GroupBox1)
        Me.BottomPanel.Controls.Add(Me.GroupBox3)
        Me.BottomPanel.Controls.Add(Me.GroupBox5)
        Me.BottomPanel.Location = New System.Drawing.Point(0, 404)
        Me.BottomPanel.Name = "BottomPanel"
        Me.BottomPanel.Size = New System.Drawing.Size(1264, 120)
        Me.BottomPanel.TabIndex = 41
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.BackColor = System.Drawing.Color.DarkSlateGray
        Me.GroupBox1.Controls.Add(Me.Addbtn)
        Me.GroupBox1.Controls.Add(Me.Print)
        Me.GroupBox1.Controls.Add(Me.Editbtn)
        Me.GroupBox1.Controls.Add(Me.deletebtn)
        Me.GroupBox1.Controls.Add(Me.savebtn)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.ComboBox3)
        Me.GroupBox1.Controls.Add(Me.secure_key)
        Me.GroupBox1.Location = New System.Drawing.Point(721, 4)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(543, 116)
        Me.GroupBox1.TabIndex = 38
        Me.GroupBox1.TabStop = False
        '
        'Addbtn
        '
        Me.Addbtn.BackColor = System.Drawing.Color.White
        Me.Addbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Addbtn.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Addbtn.ForeColor = System.Drawing.Color.Black
        Me.Addbtn.Location = New System.Drawing.Point(20, 70)
        Me.Addbtn.Margin = New System.Windows.Forms.Padding(2)
        Me.Addbtn.Name = "Addbtn"
        Me.Addbtn.Size = New System.Drawing.Size(110, 40)
        Me.Addbtn.TabIndex = 5
        Me.Addbtn.Text = "Add New"
        Me.Addbtn.UseVisualStyleBackColor = False
        '
        'Print
        '
        Me.Print.BackColor = System.Drawing.Color.White
        Me.Print.Cursor = System.Windows.Forms.Cursors.Hand
        Me.Print.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Print.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Print.ForeColor = System.Drawing.Color.Black
        Me.Print.Location = New System.Drawing.Point(145, 70)
        Me.Print.Margin = New System.Windows.Forms.Padding(2)
        Me.Print.Name = "Print"
        Me.Print.Size = New System.Drawing.Size(110, 40)
        Me.Print.TabIndex = 9
        Me.Print.Text = "Print"
        Me.Print.UseVisualStyleBackColor = False
        '
        'Editbtn
        '
        Me.Editbtn.BackColor = System.Drawing.Color.White
        Me.Editbtn.Cursor = System.Windows.Forms.Cursors.Hand
        Me.Editbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Editbtn.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Editbtn.ForeColor = System.Drawing.Color.Black
        Me.Editbtn.Location = New System.Drawing.Point(145, 20)
        Me.Editbtn.Margin = New System.Windows.Forms.Padding(2)
        Me.Editbtn.Name = "Editbtn"
        Me.Editbtn.Size = New System.Drawing.Size(110, 40)
        Me.Editbtn.TabIndex = 7
        Me.Editbtn.Text = "Edit"
        Me.Editbtn.UseVisualStyleBackColor = False
        '
        'deletebtn
        '
        Me.deletebtn.BackColor = System.Drawing.Color.Crimson
        Me.deletebtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.deletebtn.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.deletebtn.ForeColor = System.Drawing.Color.White
        Me.deletebtn.Location = New System.Drawing.Point(270, 20)
        Me.deletebtn.Margin = New System.Windows.Forms.Padding(2)
        Me.deletebtn.Name = "deletebtn"
        Me.deletebtn.Size = New System.Drawing.Size(110, 40)
        Me.deletebtn.TabIndex = 8
        Me.deletebtn.Text = "Delete"
        Me.deletebtn.UseVisualStyleBackColor = False
        '
        'savebtn
        '
        Me.savebtn.BackColor = System.Drawing.Color.DeepSkyBlue
        Me.savebtn.Cursor = System.Windows.Forms.Cursors.Hand
        Me.savebtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.savebtn.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.savebtn.ForeColor = System.Drawing.Color.Black
        Me.savebtn.Location = New System.Drawing.Point(20, 20)
        Me.savebtn.Margin = New System.Windows.Forms.Padding(2)
        Me.savebtn.Name = "savebtn"
        Me.savebtn.Size = New System.Drawing.Size(110, 40)
        Me.savebtn.TabIndex = 6
        Me.savebtn.Text = "Save"
        Me.savebtn.UseVisualStyleBackColor = False
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.Yellow
        Me.Label3.Location = New System.Drawing.Point(401, 10)
        Me.Label3.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(91, 20)
        Me.Label3.TabIndex = 33
        Me.Label3.Text = "Select User:"
        '
        'ComboBox3
        '
        Me.ComboBox3.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.ComboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBox3.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ComboBox3.ForeColor = System.Drawing.Color.Black
        Me.ComboBox3.FormattingEnabled = True
        Me.ComboBox3.Location = New System.Drawing.Point(405, 33)
        Me.ComboBox3.Name = "ComboBox3"
        Me.ComboBox3.Size = New System.Drawing.Size(100, 29)
        Me.ComboBox3.TabIndex = 31
        '
        'secure_key
        '
        Me.secure_key.BackColor = System.Drawing.Color.DarkSlateGray
        Me.secure_key.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.secure_key.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.secure_key.Location = New System.Drawing.Point(405, 70)
        Me.secure_key.Name = "secure_key"
        Me.secure_key.Size = New System.Drawing.Size(100, 18)
        Me.secure_key.TabIndex = 32
        '
        'GroupBox3
        '
        Me.GroupBox3.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.GroupBox3.BackColor = System.Drawing.Color.DarkSlateGray
        Me.GroupBox3.Controls.Add(Me.Nameserch)
        Me.GroupBox3.Controls.Add(Me.Label4)
        Me.GroupBox3.Controls.Add(Me.TelSearch)
        Me.GroupBox3.Controls.Add(Me.Label1)
        Me.GroupBox3.Location = New System.Drawing.Point(0, 6)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(694, 114)
        Me.GroupBox3.TabIndex = 42
        Me.GroupBox3.TabStop = False
        '
        'Nameserch
        '
        Me.Nameserch.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.Nameserch.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Nameserch.Location = New System.Drawing.Point(310, 68)
        Me.Nameserch.Margin = New System.Windows.Forms.Padding(2)
        Me.Nameserch.Name = "Nameserch"
        Me.Nameserch.Size = New System.Drawing.Size(358, 27)
        Me.Nameserch.TabIndex = 24
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.DarkSlateGray
        Me.Label4.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.Color.White
        Me.Label4.Location = New System.Drawing.Point(24, 43)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(90, 21)
        Me.Label4.TabIndex = 36
        Me.Label4.Text = "Phone No:"
        '
        'TelSearch
        '
        Me.TelSearch.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.TelSearch.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TelSearch.Location = New System.Drawing.Point(28, 68)
        Me.TelSearch.Margin = New System.Windows.Forms.Padding(2)
        Me.TelSearch.Name = "TelSearch"
        Me.TelSearch.Size = New System.Drawing.Size(260, 27)
        Me.TelSearch.TabIndex = 25
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.DarkSlateGray
        Me.Label1.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(306, 37)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(60, 21)
        Me.Label1.TabIndex = 35
        Me.Label1.Text = "Name:"
        '
        'DataGridView1
        '
        Me.DataGridView1.BackgroundColor = System.Drawing.Color.DarkSlateGray
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Location = New System.Drawing.Point(109, 600)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.Size = New System.Drawing.Size(10, 10)
        Me.DataGridView1.TabIndex = 34
        Me.DataGridView1.Visible = False
        '
        'Supplier_DataGridView
        '
        Me.Supplier_DataGridView.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Supplier_DataGridView.BackgroundColor = System.Drawing.Color.White
        Me.Supplier_DataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.Supplier_DataGridView.Location = New System.Drawing.Point(0, 100)
        Me.Supplier_DataGridView.Name = "Supplier_DataGridView"
        Me.Supplier_DataGridView.Size = New System.Drawing.Size(1264, 288)
        Me.Supplier_DataGridView.TabIndex = 11
        '
        'GroupBox2
        '
        Me.GroupBox2.BackColor = System.Drawing.Color.Chartreuse
        Me.GroupBox2.Location = New System.Drawing.Point(0, 84)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(1264, 10)
        Me.GroupBox2.TabIndex = 40
        Me.GroupBox2.TabStop = False
        '
        'GroupBox7
        '
        Me.GroupBox7.BackColor = System.Drawing.Color.Chartreuse
        Me.GroupBox7.Location = New System.Drawing.Point(0, 394)
        Me.GroupBox7.Name = "GroupBox7"
        Me.GroupBox7.Size = New System.Drawing.Size(1264, 10)
        Me.GroupBox7.TabIndex = 39
        Me.GroupBox7.TabStop = False
        Me.GroupBox7.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)

        'GroupBox5
        '
        Me.GroupBox5.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.GroupBox5.BackColor = System.Drawing.Color.Chartreuse
        Me.GroupBox5.Location = New System.Drawing.Point(700, 0)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Size = New System.Drawing.Size(15, 120)
        Me.GroupBox5.TabIndex = 37
        Me.GroupBox5.TabStop = False
        '
        'Suplier
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(44, Byte), Integer), CType(CType(62, Byte), Integer), CType(CType(80, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(1264, 530)
        Me.Controls.Add(Me.GroupBox7)
        Me.Controls.Add(Me.Supplier_DataGridView)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.Panel3)
        Me.Controls.Add(Me.BottomPanel)
        Me.Controls.Add(Me.DataGridView1)
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.Name = "Suplier"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Supplier Details"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
        Me.BottomPanel.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Supplier_DataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Panel3 As Panel
    Friend WithEvents EmailTxt As TextBox
    Friend WithEvents Label8 As Label
    Friend WithEvents TelNoTxt As TextBox
    Friend WithEvents Label9 As Label
    Friend WithEvents AddressTxt As TextBox
    Friend WithEvents Label10 As Label
    Friend WithEvents NameTxt As TextBox
    Friend WithEvents Label11 As Label
    Friend WithEvents deletebtn As Button
    Friend WithEvents Editbtn As Button
    Friend WithEvents savebtn As Button
    Friend WithEvents Addbtn As Button
    Friend WithEvents Supplier_DataGridView As DataGridView
    Friend WithEvents Nameserch As TextBox
    Friend WithEvents TelSearch As TextBox
    Friend WithEvents secure_key As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents Print As Button
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents Label1 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents DebitLimitTxt As TextBox
    Friend WithEvents LabelDebitLimit As Label
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents BottomPanel As Panel
    Friend WithEvents ComboBox3 As ComboBox
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents Cachedreceiving_stock_report1 As Cachedreceiving_stock_report
    Friend WithEvents Label2 As Label
    Friend WithEvents DateTimePicker3 As DateTimePicker
    Friend WithEvents GroupBox5 As GroupBox
    Friend WithEvents GroupBox7 As GroupBox
End Class
