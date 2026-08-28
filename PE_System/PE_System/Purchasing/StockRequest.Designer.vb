<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class StockRequest
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
        Dim lblnumb As System.Windows.Forms.Label
        Dim lblname As System.Windows.Forms.Label
        Dim deslab As System.Windows.Forms.Label
        Dim stock As System.Windows.Forms.Label
        Dim lblitemid As System.Windows.Forms.Label
        Dim lbltrans As System.Windows.Forms.Label
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.DataGridView3 = New System.Windows.Forms.DataGridView()
        Me.ComboBox1 = New System.Windows.Forms.ComboBox()
        Me.LabelLocatio = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.deletebtn = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.IT_CodeTextBox = New System.Windows.Forms.TextBox()
        Me.DescriptionTextBox = New System.Windows.Forms.TextBox()
        Me.QutTextBox = New System.Windows.Forms.TextBox()
        Me.TextBox9 = New System.Windows.Forms.TextBox()
        Me.lblRtnQty = New System.Windows.Forms.Label()
        Me.reteln = New System.Windows.Forms.TextBox()
        Me.rename = New System.Windows.Forms.TextBox()
        Me.cleaBtn = New System.Windows.Forms.Button()
        Me.btnNext = New System.Windows.Forms.Button()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.trann = New System.Windows.Forms.TextBox()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.sentbtn = New System.Windows.Forms.Button()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.sdetailsbtn = New System.Windows.Forms.Button()
        Me.DataGridView2 = New System.Windows.Forms.DataGridView()
        Me.ComboBoxLocation = New System.Windows.Forms.ComboBox()
        Me.DataGridView4 = New System.Windows.Forms.DataGridView()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        lblnumb = New System.Windows.Forms.Label()
        lblname = New System.Windows.Forms.Label()
        deslab = New System.Windows.Forms.Label()
        stock = New System.Windows.Forms.Label()
        lblitemid = New System.Windows.Forms.Label()
        lbltrans = New System.Windows.Forms.Label()
        Me.GroupBox1.SuspendLayout()
        CType(Me.DataGridView3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        CType(Me.DataGridView2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DataGridView4, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox4.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblnumb
        '
        lblnumb.AutoSize = True
        lblnumb.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        lblnumb.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        lblnumb.Location = New System.Drawing.Point(611, 57)
        lblnumb.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        lblnumb.Name = "lblnumb"
        lblnumb.Size = New System.Drawing.Size(123, 21)
        lblnumb.TabIndex = 349
        lblnumb.Text = "Request Te No:"
        '
        'lblname
        '
        lblname.AutoSize = True
        lblname.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        lblname.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        lblname.Location = New System.Drawing.Point(296, 56)
        lblname.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        lblname.Name = "lblname"
        lblname.Size = New System.Drawing.Size(125, 21)
        lblname.TabIndex = 347
        lblname.Text = "Request Name:"
        '
        'deslab
        '
        deslab.AutoSize = True
        deslab.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        deslab.ForeColor = System.Drawing.Color.White
        deslab.Location = New System.Drawing.Point(175, 24)
        deslab.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        deslab.Name = "deslab"
        deslab.Size = New System.Drawing.Size(93, 20)
        deslab.TabIndex = 344
        deslab.Text = "Description:"
        '
        'stock
        '
        stock.AutoSize = True
        stock.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        stock.ForeColor = System.Drawing.Color.White
        stock.Location = New System.Drawing.Point(564, 24)
        stock.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        stock.Name = "stock"
        stock.Size = New System.Drawing.Size(52, 20)
        stock.TabIndex = 342
        stock.Text = "Stock:"
        '
        'lblitemid
        '
        lblitemid.AutoSize = True
        lblitemid.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        lblitemid.ForeColor = System.Drawing.Color.White
        lblitemid.Location = New System.Drawing.Point(6, 24)
        lblitemid.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        lblitemid.Name = "lblitemid"
        lblitemid.Size = New System.Drawing.Size(64, 20)
        lblitemid.TabIndex = 339
        lblitemid.Text = "Item Id:"
        '
        'lbltrans
        '
        lbltrans.AutoSize = True
        lbltrans.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        lbltrans.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        lbltrans.Location = New System.Drawing.Point(12, 57)
        lbltrans.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        lbltrans.Name = "lbltrans"
        lbltrans.Size = New System.Drawing.Size(96, 21)
        lbltrans.TabIndex = 353
        lbltrans.Text = "Transfer ID:"
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.BackColor = System.Drawing.Color.DarkSlateGray
        Me.GroupBox1.Controls.Add(Me.DataGridView3)
        Me.GroupBox1.Controls.Add(Me.ComboBox1)
        Me.GroupBox1.Controls.Add(Me.LabelLocatio)
        Me.GroupBox1.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox1.ForeColor = System.Drawing.Color.White
        Me.GroupBox1.Location = New System.Drawing.Point(5, 121)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(2)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(2)
        Me.GroupBox1.Size = New System.Drawing.Size(557, 505)
        Me.GroupBox1.TabIndex = 39
        Me.GroupBox1.TabStop = False
        '
        'DataGridView3
        '
        Me.DataGridView3.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.DataGridView3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView3.Location = New System.Drawing.Point(5, 69)
        Me.DataGridView3.Name = "DataGridView3"
        Me.DataGridView3.Size = New System.Drawing.Size(547, 431)
        Me.DataGridView3.TabIndex = 396
        '
        'ComboBox1
        '
        Me.ComboBox1.FormattingEnabled = True
        Me.ComboBox1.Location = New System.Drawing.Point(141, 25)
        Me.ComboBox1.Name = "ComboBox1"
        Me.ComboBox1.Size = New System.Drawing.Size(147, 28)
        Me.ComboBox1.TabIndex = 390
        '
        'LabelLocatio
        '
        Me.LabelLocatio.AutoSize = True
        Me.LabelLocatio.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.LabelLocatio.ForeColor = System.Drawing.Color.White
        Me.LabelLocatio.Location = New System.Drawing.Point(40, 28)
        Me.LabelLocatio.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LabelLocatio.Name = "LabelLocatio"
        Me.LabelLocatio.Size = New System.Drawing.Size(73, 20)
        Me.LabelLocatio.TabIndex = 315
        Me.LabelLocatio.Text = "Location:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.Label2.ForeColor = System.Drawing.Color.White
        Me.Label2.Location = New System.Drawing.Point(31, 22)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(103, 20)
        Me.Label2.TabIndex = 389
        Me.Label2.Text = "Our Location:"
        '
        'deletebtn
        '
        Me.deletebtn.BackColor = System.Drawing.Color.Crimson
        Me.deletebtn.Cursor = System.Windows.Forms.Cursors.Hand
        Me.deletebtn.FlatAppearance.BorderSize = 0
        Me.deletebtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.deletebtn.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.deletebtn.ForeColor = System.Drawing.Color.White
        Me.deletebtn.Location = New System.Drawing.Point(6, 170)
        Me.deletebtn.Margin = New System.Windows.Forms.Padding(2)
        Me.deletebtn.Name = "deletebtn"
        Me.deletebtn.Size = New System.Drawing.Size(93, 42)
        Me.deletebtn.TabIndex = 392
        Me.deletebtn.Text = "Delete"
        Me.deletebtn.UseVisualStyleBackColor = False
        '
        'Button1
        '
        Me.Button1.BackColor = System.Drawing.Color.White
        Me.Button1.Cursor = System.Windows.Forms.Cursors.Hand
        Me.Button1.FlatAppearance.BorderSize = 0
        Me.Button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Button1.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button1.ForeColor = System.Drawing.Color.Black
        Me.Button1.Location = New System.Drawing.Point(6, 122)
        Me.Button1.Margin = New System.Windows.Forms.Padding(4)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(93, 42)
        Me.Button1.TabIndex = 391
        Me.Button1.Text = "Edit"
        Me.Button1.UseVisualStyleBackColor = False
        '
        'GroupBox2
        '
        Me.GroupBox2.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.GroupBox2.Controls.Add(Me.IT_CodeTextBox)
        Me.GroupBox2.Controls.Add(lblitemid)
        Me.GroupBox2.Controls.Add(Me.DescriptionTextBox)
        Me.GroupBox2.Controls.Add(stock)
        Me.GroupBox2.Controls.Add(Me.QutTextBox)
        Me.GroupBox2.Controls.Add(deslab)
        Me.GroupBox2.Location = New System.Drawing.Point(6, 630)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(722, 65)
        Me.GroupBox2.TabIndex = 387
        Me.GroupBox2.TabStop = False
        '
        'IT_CodeTextBox
        '
        Me.IT_CodeTextBox.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.IT_CodeTextBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.IT_CodeTextBox.Location = New System.Drawing.Point(65, 21)
        Me.IT_CodeTextBox.Margin = New System.Windows.Forms.Padding(4)
        Me.IT_CodeTextBox.Name = "IT_CodeTextBox"
        Me.IT_CodeTextBox.Size = New System.Drawing.Size(103, 26)
        Me.IT_CodeTextBox.TabIndex = 340
        '
        'DescriptionTextBox
        '
        Me.DescriptionTextBox.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.DescriptionTextBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DescriptionTextBox.Location = New System.Drawing.Point(261, 21)
        Me.DescriptionTextBox.Margin = New System.Windows.Forms.Padding(4)
        Me.DescriptionTextBox.Name = "DescriptionTextBox"
        Me.DescriptionTextBox.Size = New System.Drawing.Size(295, 26)
        Me.DescriptionTextBox.TabIndex = 341
        '
        'QutTextBox
        '
        Me.QutTextBox.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.QutTextBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.QutTextBox.Location = New System.Drawing.Point(615, 21)
        Me.QutTextBox.Margin = New System.Windows.Forms.Padding(4)
        Me.QutTextBox.Name = "QutTextBox"
        Me.QutTextBox.Size = New System.Drawing.Size(100, 26)
        Me.QutTextBox.TabIndex = 343
        '
        'TextBox9
        '
        Me.TextBox9.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.TextBox9.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TextBox9.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox9.Location = New System.Drawing.Point(755, 654)
        Me.TextBox9.Margin = New System.Windows.Forms.Padding(4)
        Me.TextBox9.Name = "TextBox9"
        Me.TextBox9.Size = New System.Drawing.Size(109, 26)
        Me.TextBox9.TabIndex = 383
        '
        'lblRtnQty
        '
        Me.lblRtnQty.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblRtnQty.AutoSize = True
        Me.lblRtnQty.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.lblRtnQty.ForeColor = System.Drawing.Color.Yellow
        Me.lblRtnQty.Location = New System.Drawing.Point(751, 630)
        Me.lblRtnQty.Name = "lblRtnQty"
        Me.lblRtnQty.Size = New System.Drawing.Size(99, 20)
        Me.lblRtnQty.TabIndex = 382
        Me.lblRtnQty.Text = "Request Qty:"
        '
        'reteln
        '
        Me.reteln.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.reteln.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.reteln.Location = New System.Drawing.Point(733, 54)
        Me.reteln.Margin = New System.Windows.Forms.Padding(4)
        Me.reteln.Name = "reteln"
        Me.reteln.Size = New System.Drawing.Size(180, 26)
        Me.reteln.TabIndex = 350
        '
        'rename
        '
        Me.rename.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.rename.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rename.Location = New System.Drawing.Point(421, 54)
        Me.rename.Margin = New System.Windows.Forms.Padding(4)
        Me.rename.Name = "rename"
        Me.rename.Size = New System.Drawing.Size(180, 26)
        Me.rename.TabIndex = 348
        '
        'cleaBtn
        '
        Me.cleaBtn.BackColor = System.Drawing.Color.White
        Me.cleaBtn.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cleaBtn.FlatAppearance.BorderSize = 0
        Me.cleaBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cleaBtn.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.cleaBtn.ForeColor = System.Drawing.Color.Black
        Me.cleaBtn.Location = New System.Drawing.Point(938, 39)
        Me.cleaBtn.Name = "cleaBtn"
        Me.cleaBtn.Size = New System.Drawing.Size(89, 42)
        Me.cleaBtn.TabIndex = 385
        Me.cleaBtn.Text = "Add New"
        Me.cleaBtn.UseVisualStyleBackColor = False
        '
        'btnNext
        '
        Me.btnNext.BackColor = System.Drawing.Color.DodgerBlue
        Me.btnNext.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnNext.FlatAppearance.BorderSize = 0
        Me.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnNext.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.btnNext.ForeColor = System.Drawing.Color.White
        Me.btnNext.Location = New System.Drawing.Point(6, 19)
        Me.btnNext.Name = "btnNext"
        Me.btnNext.Size = New System.Drawing.Size(93, 45)
        Me.btnNext.TabIndex = 386
        Me.btnNext.Text = "Next"
        Me.btnNext.UseVisualStyleBackColor = False
        '
        'btnSave
        '
        Me.btnSave.BackColor = System.Drawing.Color.DeepSkyBlue
        Me.btnSave.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnSave.FlatAppearance.BorderSize = 0
        Me.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnSave.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.btnSave.ForeColor = System.Drawing.Color.Black
        Me.btnSave.Location = New System.Drawing.Point(6, 70)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(93, 45)
        Me.btnSave.TabIndex = 384
        Me.btnSave.Text = "Save"
        Me.btnSave.UseVisualStyleBackColor = False
        '
        'trann
        '
        Me.trann.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.trann.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.trann.Location = New System.Drawing.Point(112, 54)
        Me.trann.Margin = New System.Windows.Forms.Padding(4)
        Me.trann.Name = "trann"
        Me.trann.Size = New System.Drawing.Size(176, 26)
        Me.trann.TabIndex = 354
        '
        'GroupBox3
        '
        Me.GroupBox3.BackColor = System.Drawing.Color.DarkSlateGray
        Me.GroupBox3.Controls.Add(Me.Label10)
        Me.GroupBox3.Controls.Add(lbltrans)
        Me.GroupBox3.Controls.Add(Me.trann)
        Me.GroupBox3.Controls.Add(Me.cleaBtn)
        Me.GroupBox3.Controls.Add(Me.rename)
        Me.GroupBox3.Controls.Add(Me.reteln)
        Me.GroupBox3.Controls.Add(lblname)
        Me.GroupBox3.Controls.Add(lblnumb)
        Me.GroupBox3.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox3.ForeColor = System.Drawing.Color.White
        Me.GroupBox3.Location = New System.Drawing.Point(6, 11)
        Me.GroupBox3.Margin = New System.Windows.Forms.Padding(2)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Padding = New System.Windows.Forms.Padding(2)
        Me.GroupBox3.Size = New System.Drawing.Size(1062, 100)
        Me.GroupBox3.TabIndex = 354
        Me.GroupBox3.TabStop = False
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("Segoe UI", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label10.ForeColor = System.Drawing.Color.White
        Me.Label10.Location = New System.Drawing.Point(6, 16)
        Me.Label10.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(151, 30)
        Me.Label10.TabIndex = 351
        Me.Label10.Text = "Stock Request"
        '
        'sentbtn
        '
        Me.sentbtn.BackColor = System.Drawing.Color.Yellow
        Me.sentbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.sentbtn.Font = New System.Drawing.Font("Segoe UI", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.sentbtn.ForeColor = System.Drawing.Color.Black
        Me.sentbtn.Location = New System.Drawing.Point(586, 137)
        Me.sentbtn.Margin = New System.Windows.Forms.Padding(2)
        Me.sentbtn.Name = "sentbtn"
        Me.sentbtn.Size = New System.Drawing.Size(71, 33)
        Me.sentbtn.TabIndex = 388
        Me.sentbtn.Text = "> > >"
        Me.sentbtn.UseVisualStyleBackColor = False
        '
        'GroupBox5
        '
        Me.GroupBox5.BackColor = System.Drawing.Color.FromArgb(CType(CType(44, Byte), Integer), CType(CType(62, Byte), Integer), CType(CType(80, Byte), Integer))
        Me.GroupBox5.Controls.Add(Me.btnNext)
        Me.GroupBox5.Controls.Add(Me.btnSave)
        Me.GroupBox5.Controls.Add(Me.Button1)
        Me.GroupBox5.Controls.Add(Me.deletebtn)
        Me.GroupBox5.Location = New System.Drawing.Point(578, 236)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Size = New System.Drawing.Size(109, 224)
        Me.GroupBox5.TabIndex = 394
        Me.GroupBox5.TabStop = False
        '
        'Button2
        '
        Me.Button2.BackColor = System.Drawing.Color.Yellow
        Me.Button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Button2.Font = New System.Drawing.Font("Segoe UI", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button2.ForeColor = System.Drawing.Color.Black
        Me.Button2.Location = New System.Drawing.Point(586, 182)
        Me.Button2.Margin = New System.Windows.Forms.Padding(2)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(71, 33)
        Me.Button2.TabIndex = 393
        Me.Button2.Text = "< < <"
        Me.Button2.UseVisualStyleBackColor = False
        '
        'sdetailsbtn
        '
        Me.sdetailsbtn.BackColor = System.Drawing.Color.DodgerBlue
        Me.sdetailsbtn.Cursor = System.Windows.Forms.Cursors.Hand
        Me.sdetailsbtn.FlatAppearance.BorderSize = 0
        Me.sdetailsbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.sdetailsbtn.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.sdetailsbtn.ForeColor = System.Drawing.Color.White
        Me.sdetailsbtn.Location = New System.Drawing.Point(1127, 41)
        Me.sdetailsbtn.Margin = New System.Windows.Forms.Padding(4)
        Me.sdetailsbtn.Name = "sdetailsbtn"
        Me.sdetailsbtn.Size = New System.Drawing.Size(90, 51)
        Me.sdetailsbtn.TabIndex = 396
        Me.sdetailsbtn.Text = "Request" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "History"
        Me.sdetailsbtn.UseVisualStyleBackColor = False
        '
        'DataGridView2
        '
        Me.DataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView2.Location = New System.Drawing.Point(1108, 510)
        Me.DataGridView2.Name = "DataGridView2"
        Me.DataGridView2.Size = New System.Drawing.Size(100, 45)
        Me.DataGridView2.TabIndex = 397
        Me.DataGridView2.Visible = False
        '
        'ComboBoxLocation
        '
        Me.ComboBoxLocation.FormattingEnabled = True
        Me.ComboBoxLocation.Location = New System.Drawing.Point(141, 19)
        Me.ComboBoxLocation.Name = "ComboBoxLocation"
        Me.ComboBoxLocation.Size = New System.Drawing.Size(147, 28)
        Me.ComboBoxLocation.TabIndex = 338
        '
        'DataGridView4
        '
        Me.DataGridView4.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.DataGridView4.BackgroundColor = System.Drawing.Color.WhiteSmoke
        Me.DataGridView4.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView4.Location = New System.Drawing.Point(5, 61)
        Me.DataGridView4.Name = "DataGridView4"
        Me.DataGridView4.Size = New System.Drawing.Size(549, 439)
        Me.DataGridView4.TabIndex = 397
        '
        'GroupBox4
        '
        Me.GroupBox4.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.GroupBox4.BackColor = System.Drawing.Color.DarkSlateGray
        Me.GroupBox4.Controls.Add(Me.DataGridView4)
        Me.GroupBox4.Controls.Add(Me.ComboBoxLocation)
        Me.GroupBox4.Controls.Add(Me.Label2)
        Me.GroupBox4.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox4.ForeColor = System.Drawing.Color.White
        Me.GroupBox4.Location = New System.Drawing.Point(700, 121)
        Me.GroupBox4.Margin = New System.Windows.Forms.Padding(2)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Padding = New System.Windows.Forms.Padding(2)
        Me.GroupBox4.Size = New System.Drawing.Size(559, 505)
        Me.GroupBox4.TabIndex = 339
        Me.GroupBox4.TabStop = False
        '
        'DataGridView1
        '
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Location = New System.Drawing.Point(87, 143)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.RowHeadersWidth = 51
        Me.DataGridView1.Size = New System.Drawing.Size(470, 583)
        Me.DataGridView1.TabIndex = 395
        '
        'StockRequest
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(44, Byte), Integer), CType(CType(62, Byte), Integer), CType(CType(80, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(1264, 749)
        Me.Controls.Add(Me.DataGridView2)
        Me.Controls.Add(Me.sdetailsbtn)
        Me.Controls.Add(Me.DataGridView1)
        Me.Controls.Add(Me.GroupBox5)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.GroupBox4)
        Me.Controls.Add(Me.sentbtn)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.TextBox9)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.lblRtnQty)
        Me.Name = "StockRequest"
        Me.Text = "StockRequest"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        CType(Me.DataGridView3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.GroupBox5.ResumeLayout(False)
        CType(Me.DataGridView2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DataGridView4, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents trann As TextBox
    Friend WithEvents reteln As TextBox
    Friend WithEvents rename As TextBox
    Friend WithEvents QutTextBox As TextBox
    Friend WithEvents DescriptionTextBox As TextBox
    Friend WithEvents IT_CodeTextBox As TextBox
    Friend WithEvents lblRtnQty As Label
    Friend WithEvents TextBox9 As TextBox
    Friend WithEvents btnNext As Button
    Friend WithEvents btnSave As Button
    Friend WithEvents cleaBtn As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents ComboBox1 As ComboBox
    Friend WithEvents Button1 As Button
    Friend WithEvents deletebtn As Button
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents Label10 As Label
    Friend WithEvents sentbtn As Button
    Friend WithEvents Button2 As Button
    Friend WithEvents GroupBox5 As GroupBox
    Friend WithEvents DataGridView3 As DataGridView
    Friend WithEvents sdetailsbtn As Button
    Friend WithEvents DataGridView2 As DataGridView
    Friend WithEvents LabelLocatio As Label
    Friend WithEvents ComboBoxLocation As ComboBox
    Friend WithEvents DataGridView4 As DataGridView
    Friend WithEvents GroupBox4 As GroupBox
    Friend WithEvents DataGridView1 As DataGridView
End Class
