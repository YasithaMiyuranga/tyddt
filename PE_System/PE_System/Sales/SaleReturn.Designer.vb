<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class SaleReturn
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
        Me.components = New System.ComponentModel.Container()
        Dim Label34 As System.Windows.Forms.Label
        Dim Label35 As System.Windows.Forms.Label
        Dim Label14 As System.Windows.Forms.Label
        Dim Label16 As System.Windows.Forms.Label
        Dim Label20 As System.Windows.Forms.Label
        Dim Label22 As System.Windows.Forms.Label
        Dim Label23 As System.Windows.Forms.Label
        Dim Label24 As System.Windows.Forms.Label
        Dim Label36 As System.Windows.Forms.Label
        Dim Label31 As System.Windows.Forms.Label
        Dim Label25 As System.Windows.Forms.Label
        Dim Label26 As System.Windows.Forms.Label
        Dim Label27 As System.Windows.Forms.Label
        Dim Label29 As System.Windows.Forms.Label
        Dim Label30 As System.Windows.Forms.Label
        Dim Label3 As System.Windows.Forms.Label
        Dim Label2 As System.Windows.Forms.Label
        Dim Label4 As System.Windows.Forms.Label
        Dim Label10 As System.Windows.Forms.Label
        Dim Label11 As System.Windows.Forms.Label
        Dim Label6 As System.Windows.Forms.Label
        Dim Label8 As System.Windows.Forms.Label
        Dim Label7 As System.Windows.Forms.Label
        Dim Label1 As System.Windows.Forms.Label
        Me.btnReturnLog = New System.Windows.Forms.Button()
        Me.LiveTimer = New System.Windows.Forms.Timer(Me.components)
        Me.lblLiveTimeDisplay = New System.Windows.Forms.Label()
        Me.btnSaveReturn = New System.Windows.Forms.Button()
        Me.GroupBox7 = New System.Windows.Forms.GroupBox()
        Me.remain_stock = New System.Windows.Forms.Label()
        Me.txtCashierID = New System.Windows.Forms.TextBox()
        Me.cmbCashier = New System.Windows.Forms.ComboBox()
        Me.return_amt = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.return_debit = New System.Windows.Forms.Label()
        Me.return_cash = New System.Windows.Forms.TextBox()
        Me.btnchange = New System.Windows.Forms.Button()
        Me.btnprintinvoice = New System.Windows.Forms.Button()
        Me.ComboBox5 = New System.Windows.Forms.ComboBox()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.DateTimePicker1 = New System.Windows.Forms.DateTimePicker()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.discount = New System.Windows.Forms.Label()
        Me.invdis = New System.Windows.Forms.Label()
        Me.grandtotal = New System.Windows.Forms.Label()
        Me.totalamount = New System.Windows.Forms.Label()
        Me.TextBox5 = New System.Windows.Forms.TextBox()
        Me.TextBox6 = New System.Windows.Forms.TextBox()
        Me.GroupBox6 = New System.Windows.Forms.GroupBox()
        Me.trans_dis = New System.Windows.Forms.TextBox()
        Me.trans_itcode = New System.Windows.Forms.Label()
        Me.trans_unit = New System.Windows.Forms.Label()
        Me.trans_amount = New System.Windows.Forms.TextBox()
        Me.tran_cus = New System.Windows.Forms.TextBox()
        Me.trans_qyt = New System.Windows.Forms.Label()
        Me.trans_des = New System.Windows.Forms.TextBox()
        Me.trans_invno = New System.Windows.Forms.TextBox()
        Me.DataGridView2 = New System.Windows.Forms.DataGridView()
        Label34 = New System.Windows.Forms.Label()
        Label35 = New System.Windows.Forms.Label()
        Label14 = New System.Windows.Forms.Label()
        Label16 = New System.Windows.Forms.Label()
        Label20 = New System.Windows.Forms.Label()
        Label22 = New System.Windows.Forms.Label()
        Label23 = New System.Windows.Forms.Label()
        Label24 = New System.Windows.Forms.Label()
        Label36 = New System.Windows.Forms.Label()
        Label31 = New System.Windows.Forms.Label()
        Label25 = New System.Windows.Forms.Label()
        Label26 = New System.Windows.Forms.Label()
        Label27 = New System.Windows.Forms.Label()
        Label29 = New System.Windows.Forms.Label()
        Label30 = New System.Windows.Forms.Label()
        Label3 = New System.Windows.Forms.Label()
        Label2 = New System.Windows.Forms.Label()
        Label4 = New System.Windows.Forms.Label()
        Label10 = New System.Windows.Forms.Label()
        Label11 = New System.Windows.Forms.Label()
        Label6 = New System.Windows.Forms.Label()
        Label8 = New System.Windows.Forms.Label()
        Label7 = New System.Windows.Forms.Label()
        Label1 = New System.Windows.Forms.Label()
        Me.GroupBox7.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        Me.GroupBox6.SuspendLayout()
        CType(Me.DataGridView2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnReturnLog
        '
        Me.btnReturnLog.BackColor = System.Drawing.Color.White
        Me.btnReturnLog.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnReturnLog.FlatAppearance.BorderSize = 0
        Me.btnReturnLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnReturnLog.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnReturnLog.ForeColor = System.Drawing.Color.Black
        Me.btnReturnLog.Location = New System.Drawing.Point(1142, 61)
        Me.btnReturnLog.Name = "btnReturnLog"
        Me.btnReturnLog.Size = New System.Drawing.Size(105, 30)
        Me.btnReturnLog.TabIndex = 118
        Me.btnReturnLog.Text = "Return Log"
        Me.btnReturnLog.UseVisualStyleBackColor = False
        '
        'Label34
        '
        Label34.AutoSize = True
        Label34.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Label34.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Label34.Location = New System.Drawing.Point(5, 51)
        Label34.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Label34.Name = "Label34"
        Label34.Size = New System.Drawing.Size(128, 20)
        Label34.TabIndex = 12
        Label34.Text = "Cash Payment: "
        '
        'Label35
        '
        Label35.AutoSize = True
        Label35.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Label35.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Label35.Location = New System.Drawing.Point(5, 20)
        Label35.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Label35.Name = "Label35"
        Label35.Size = New System.Drawing.Size(125, 20)
        Label35.TabIndex = 3
        Label35.Text = "Debit Balance: "
        '
        'Label14
        '
        Label14.AutoSize = True
        Label14.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Label14.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Label14.Location = New System.Drawing.Point(19, 47)
        Label14.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Label14.Name = "Label14"
        Label14.Size = New System.Drawing.Size(114, 20)
        Label14.TabIndex = 24
        Label14.Text = "Our Discount:"
        '
        'Label16
        '
        Label16.AutoSize = True
        Label16.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Label16.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Label16.Location = New System.Drawing.Point(19, 80)
        Label16.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Label16.Name = "Label16"
        Label16.Size = New System.Drawing.Size(66, 20)
        Label16.TabIndex = 22
        Label16.Text = "Inv DIs:"
        '
        'Label20
        '
        Label20.AutoSize = True
        Label20.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Label20.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Label20.Location = New System.Drawing.Point(19, 113)
        Label20.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Label20.Name = "Label20"
        Label20.Size = New System.Drawing.Size(102, 20)
        Label20.TabIndex = 18
        Label20.Text = "Grand Total:"
        '
        'Label22
        '
        Label22.AutoSize = True
        Label22.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Label22.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Label22.Location = New System.Drawing.Point(19, 14)
        Label22.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Label22.Name = "Label22"
        Label22.Size = New System.Drawing.Size(113, 20)
        Label22.TabIndex = 11
        Label22.Text = "Total Amount:"
        '
        'Label23
        '
        Label23.AutoSize = True
        Label23.Location = New System.Drawing.Point(4, 207)
        Label23.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Label23.Name = "Label23"
        Label23.Size = New System.Drawing.Size(60, 20)
        Label23.TabIndex = 8
        Label23.Text = "Stock:"
        '
        'Label24
        '
        Label24.AutoSize = True
        Label24.Location = New System.Drawing.Point(175, 227)
        Label24.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Label24.Name = "Label24"
        Label24.Size = New System.Drawing.Size(60, 20)
        Label24.TabIndex = 6
        Label24.Text = "Name:"
        '
        'Label36
        '
        Label36.AutoSize = True
        Label36.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Label36.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Label36.Location = New System.Drawing.Point(439, 20)
        Label36.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Label36.Name = "Label36"
        Label36.Size = New System.Drawing.Size(83, 20)
        Label36.TabIndex = 45
        Label36.Text = "Unit Price"
        '
        'Label31
        '
        Label31.AutoSize = True
        Label31.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Label31.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Label31.Location = New System.Drawing.Point(615, 83)
        Label31.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Label31.Name = "Label31"
        Label31.Size = New System.Drawing.Size(66, 20)
        Label31.TabIndex = 39
        Label31.Text = "Amount"
        '
        'Label25
        '
        Label25.AutoSize = True
        Label25.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Label25.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Label25.Location = New System.Drawing.Point(218, 24)
        Label25.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Label25.Name = "Label25"
        Label25.Size = New System.Drawing.Size(131, 20)
        Label25.TabIndex = 12
        Label25.Text = "Customer Name"
        '
        'Label26
        '
        Label26.AutoSize = True
        Label26.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Label26.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Label26.Location = New System.Drawing.Point(8, 87)
        Label26.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Label26.Name = "Label26"
        Label26.Size = New System.Drawing.Size(67, 20)
        Label26.TabIndex = 9
        Label26.Text = "IT Code"
        '
        'Label27
        '
        Label27.AutoSize = True
        Label27.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Label27.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Label27.Location = New System.Drawing.Point(625, 20)
        Label27.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Label27.Name = "Label27"
        Label27.Size = New System.Drawing.Size(35, 20)
        Label27.TabIndex = 7
        Label27.Text = "Qyt"
        '
        'Label29
        '
        Label29.AutoSize = True
        Label29.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Label29.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Label29.Location = New System.Drawing.Point(117, 83)
        Label29.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Label29.Name = "Label29"
        Label29.Size = New System.Drawing.Size(95, 20)
        Label29.TabIndex = 5
        Label29.Text = "Description"
        '
        'Label30
        '
        Label30.AutoSize = True
        Label30.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Label30.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Label30.Location = New System.Drawing.Point(5, 20)
        Label30.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Label30.Name = "Label30"
        Label30.Size = New System.Drawing.Size(56, 20)
        Label30.TabIndex = 3
        Label30.Text = "Inv No"
        '
        'Label3
        '
        Label3.AutoSize = True
        Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Label3.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Label3.Location = New System.Drawing.Point(960, 4)
        Label3.Name = "Label3"
        Label3.Size = New System.Drawing.Size(83, 20)
        Label3.TabIndex = 352
        Label3.Text = "Sale Date"
        '
        'Label2
        '
        Label2.AutoSize = True
        Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Label2.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Label2.Location = New System.Drawing.Point(530, 83)
        Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Label2.Name = "Label2"
        Label2.Size = New System.Drawing.Size(35, 20)
        Label2.TabIndex = 48
        Label2.Text = "Dis"
        '
        'Label4
        '
        Label4.AutoSize = True
        Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Label4.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Label4.Location = New System.Drawing.Point(228, 51)
        Label4.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Label4.Name = "Label4"
        Label4.Size = New System.Drawing.Size(24, 20)
        Label4.TabIndex = 26
        Label4.Text = "%"
        '
        'Label10
        '
        Label10.AutoSize = True
        Label10.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Label10.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Label10.Location = New System.Drawing.Point(243, 51)
        Label10.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Label10.Name = "Label10"
        Label10.Size = New System.Drawing.Size(113, 20)
        Label10.TabIndex = 73
        Label10.Text = "Cash Return: "
        '
        'Label11
        '
        Label11.AutoSize = True
        Label11.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Label11.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Label11.Location = New System.Drawing.Point(243, 20)
        Label11.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Label11.Name = "Label11"
        Label11.Size = New System.Drawing.Size(104, 20)
        Label11.TabIndex = 72
        Label11.Text = "Return Amt: "
        '
        'Label6
        '
        Label6.AutoSize = True
        Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Label6.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Label6.Location = New System.Drawing.Point(862, 23)
        Label6.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Label6.Name = "Label6"
        Label6.Size = New System.Drawing.Size(99, 20)
        Label6.TabIndex = 76
        Label6.Text = "Cash Type: "
        '
        'Label8
        '
        Label8.AutoSize = True
        Label8.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Label8.ForeColor = System.Drawing.Color.White
        Label8.Location = New System.Drawing.Point(482, 19)
        Label8.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Label8.Name = "Label8"
        Label8.Size = New System.Drawing.Size(67, 20)
        Label8.TabIndex = 109
        Label8.Text = "Cashier"
        '
        'Label7
        '
        Label7.AutoSize = True
        Label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Label7.ForeColor = System.Drawing.Color.White
        Label7.Location = New System.Drawing.Point(649, 23)
        Label7.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Label7.Name = "Label7"
        Label7.Size = New System.Drawing.Size(89, 20)
        Label7.TabIndex = 112
        Label7.Text = "Cashier ID"
        '
        'Label1
        '
        Label1.AutoSize = True
        Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Label1.ForeColor = System.Drawing.Color.White
        Label1.Location = New System.Drawing.Point(493, 63)
        Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Label1.Name = "Label1"
        Label1.Size = New System.Drawing.Size(149, 20)
        Label1.TabIndex = 114
        Label1.Text = "Remaining Stocks:"
        '
        'LiveTimer
        '
        Me.LiveTimer.Enabled = True
        Me.LiveTimer.Interval = 1000
        '
        'lblLiveTimeDisplay
        '
        Me.lblLiveTimeDisplay.AutoSize = True
        Me.lblLiveTimeDisplay.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLiveTimeDisplay.ForeColor = System.Drawing.Color.White
        Me.lblLiveTimeDisplay.Location = New System.Drawing.Point(837, 4)
        Me.lblLiveTimeDisplay.Name = "lblLiveTimeDisplay"
        Me.lblLiveTimeDisplay.Size = New System.Drawing.Size(71, 20)
        Me.lblLiveTimeDisplay.TabIndex = 117
        Me.lblLiveTimeDisplay.Text = "00:00:00"
        '
        'btnSaveReturn
        '
        Me.btnSaveReturn.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.btnSaveReturn.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnSaveReturn.FlatAppearance.BorderSize = 0
        Me.btnSaveReturn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnSaveReturn.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSaveReturn.ForeColor = System.Drawing.Color.White
        Me.btnSaveReturn.Location = New System.Drawing.Point(755, 58)
        Me.btnSaveReturn.Name = "btnSaveReturn"
        Me.btnSaveReturn.Size = New System.Drawing.Size(139, 35)
        Me.btnSaveReturn.TabIndex = 37
        Me.btnSaveReturn.Text = "Save Return"
        Me.btnSaveReturn.UseVisualStyleBackColor = False
        '
        'GroupBox7
        '
        Me.GroupBox7.BackColor = System.Drawing.Color.DarkSlateGray
        Me.GroupBox7.Controls.Add(Me.remain_stock)
        Me.GroupBox7.Controls.Add(Label1)
        Me.GroupBox7.Controls.Add(Me.txtCashierID)
        Me.GroupBox7.Controls.Add(Label7)
        Me.GroupBox7.Controls.Add(Me.cmbCashier)
        Me.GroupBox7.Controls.Add(Label8)
        Me.GroupBox7.Controls.Add(Label6)
        Me.GroupBox7.Controls.Add(Me.return_amt)
        Me.GroupBox7.Controls.Add(Me.Label9)
        Me.GroupBox7.Controls.Add(Label10)
        Me.GroupBox7.Controls.Add(Label11)
        Me.GroupBox7.Controls.Add(Me.return_debit)
        Me.GroupBox7.Controls.Add(Me.return_cash)
        Me.GroupBox7.Controls.Add(Me.btnchange)
        Me.GroupBox7.Controls.Add(Me.btnprintinvoice)
        Me.GroupBox7.Controls.Add(Label34)
        Me.GroupBox7.Controls.Add(Label35)
        Me.GroupBox7.Controls.Add(Me.btnReturnLog)
        Me.GroupBox7.Controls.Add(Me.btnSaveReturn)
        Me.GroupBox7.Controls.Add(Me.ComboBox5)
        Me.GroupBox7.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox7.ForeColor = System.Drawing.Color.Yellow
        Me.GroupBox7.Location = New System.Drawing.Point(13, 206)
        Me.GroupBox7.Margin = New System.Windows.Forms.Padding(4)
        Me.GroupBox7.Name = "GroupBox7"
        Me.GroupBox7.Padding = New System.Windows.Forms.Padding(4)
        Me.GroupBox7.Size = New System.Drawing.Size(1254, 100)
        Me.GroupBox7.TabIndex = 70
        Me.GroupBox7.TabStop = False
        Me.GroupBox7.Text = "Return"
        '
        'remain_stock
        '
        Me.remain_stock.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.remain_stock.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.remain_stock.ForeColor = System.Drawing.Color.Black
        Me.remain_stock.Location = New System.Drawing.Point(638, 59)
        Me.remain_stock.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.remain_stock.Name = "remain_stock"
        Me.remain_stock.Size = New System.Drawing.Size(89, 28)
        Me.remain_stock.TabIndex = 115
        Me.remain_stock.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'txtCashierID
        '
        Me.txtCashierID.BackColor = System.Drawing.Color.White
        Me.txtCashierID.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtCashierID.Location = New System.Drawing.Point(736, 23)
        Me.txtCashierID.Name = "txtCashierID"
        Me.txtCashierID.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.txtCashierID.Size = New System.Drawing.Size(98, 19)
        Me.txtCashierID.TabIndex = 113
        Me.txtCashierID.UseSystemPasswordChar = True
        '
        'cmbCashier
        '
        Me.cmbCashier.FormattingEnabled = True
        Me.cmbCashier.Location = New System.Drawing.Point(556, 17)
        Me.cmbCashier.Name = "cmbCashier"
        Me.cmbCashier.Size = New System.Drawing.Size(86, 28)
        Me.cmbCashier.TabIndex = 110
        '
        'return_amt
        '
        Me.return_amt.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.return_amt.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.return_amt.ForeColor = System.Drawing.Color.Black
        Me.return_amt.Location = New System.Drawing.Point(376, 15)
        Me.return_amt.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.return_amt.Name = "return_amt"
        Me.return_amt.Size = New System.Drawing.Size(89, 28)
        Me.return_amt.TabIndex = 75
        Me.return_amt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label9
        '
        Me.Label9.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.Label9.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.ForeColor = System.Drawing.Color.Black
        Me.Label9.Location = New System.Drawing.Point(376, 51)
        Me.Label9.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(89, 28)
        Me.Label9.TabIndex = 74
        Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'return_debit
        '
        Me.return_debit.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.return_debit.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.return_debit.ForeColor = System.Drawing.Color.Black
        Me.return_debit.Location = New System.Drawing.Point(138, 15)
        Me.return_debit.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.return_debit.Name = "return_debit"
        Me.return_debit.Size = New System.Drawing.Size(89, 28)
        Me.return_debit.TabIndex = 71
        Me.return_debit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'return_cash
        '
        Me.return_cash.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.return_cash.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.return_cash.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.return_cash.Location = New System.Drawing.Point(138, 51)
        Me.return_cash.Margin = New System.Windows.Forms.Padding(4)
        Me.return_cash.Name = "return_cash"
        Me.return_cash.Size = New System.Drawing.Size(89, 20)
        Me.return_cash.TabIndex = 69
        Me.return_cash.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'btnchange
        '
        Me.btnchange.BackColor = System.Drawing.Color.DodgerBlue
        Me.btnchange.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnchange.FlatAppearance.BorderSize = 0
        Me.btnchange.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnchange.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnchange.ForeColor = System.Drawing.Color.White
        Me.btnchange.Location = New System.Drawing.Point(1042, 59)
        Me.btnchange.Name = "btnchange"
        Me.btnchange.Size = New System.Drawing.Size(93, 35)
        Me.btnchange.TabIndex = 36
        Me.btnchange.Text = "Change"
        Me.btnchange.UseVisualStyleBackColor = False
        '
        'btnprintinvoice
        '
        Me.btnprintinvoice.BackColor = System.Drawing.Color.SeaGreen
        Me.btnprintinvoice.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnprintinvoice.FlatAppearance.BorderSize = 0
        Me.btnprintinvoice.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnprintinvoice.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnprintinvoice.ForeColor = System.Drawing.Color.White
        Me.btnprintinvoice.Location = New System.Drawing.Point(900, 58)
        Me.btnprintinvoice.Name = "btnprintinvoice"
        Me.btnprintinvoice.Size = New System.Drawing.Size(136, 35)
        Me.btnprintinvoice.TabIndex = 35
        Me.btnprintinvoice.Text = "RePrint Invoice"
        Me.btnprintinvoice.UseVisualStyleBackColor = False
        '
        'ComboBox5
        '
        Me.ComboBox5.FormattingEnabled = True
        Me.ComboBox5.Location = New System.Drawing.Point(982, 17)
        Me.ComboBox5.Name = "ComboBox5"
        Me.ComboBox5.Size = New System.Drawing.Size(235, 28)
        Me.ComboBox5.TabIndex = 10
        '
        'GroupBox4
        '
        Me.GroupBox4.BackColor = System.Drawing.Color.FromArgb(CType(CType(44, Byte), Integer), CType(CType(62, Byte), Integer), CType(CType(80, Byte), Integer))
        Me.GroupBox4.Controls.Add(Me.lblLiveTimeDisplay)
        Me.GroupBox4.Controls.Add(Label3)
        Me.GroupBox4.Controls.Add(Me.DateTimePicker1)
        Me.GroupBox4.Controls.Add(Me.GroupBox5)
        Me.GroupBox4.Controls.Add(Me.GroupBox6)
        Me.GroupBox4.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox4.ForeColor = System.Drawing.Color.Yellow
        Me.GroupBox4.Location = New System.Drawing.Point(14, 12)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(1254, 187)
        Me.GroupBox4.TabIndex = 69
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Stock Transaction"
        '
        'DateTimePicker1
        '
        Me.DateTimePicker1.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DateTimePicker1.Location = New System.Drawing.Point(1051, 0)
        Me.DateTimePicker1.Margin = New System.Windows.Forms.Padding(2)
        Me.DateTimePicker1.Name = "DateTimePicker1"
        Me.DateTimePicker1.Size = New System.Drawing.Size(188, 27)
        Me.DateTimePicker1.TabIndex = 64
        '
        'GroupBox5
        '
        Me.GroupBox5.Controls.Add(Label4)
        Me.GroupBox5.Controls.Add(Me.discount)
        Me.GroupBox5.Controls.Add(Label14)
        Me.GroupBox5.Controls.Add(Me.invdis)
        Me.GroupBox5.Controls.Add(Label16)
        Me.GroupBox5.Controls.Add(Me.grandtotal)
        Me.GroupBox5.Controls.Add(Label20)
        Me.GroupBox5.Controls.Add(Me.totalamount)
        Me.GroupBox5.Controls.Add(Label22)
        Me.GroupBox5.Controls.Add(Label23)
        Me.GroupBox5.Controls.Add(Me.TextBox5)
        Me.GroupBox5.Controls.Add(Label24)
        Me.GroupBox5.Controls.Add(Me.TextBox6)
        Me.GroupBox5.Location = New System.Drawing.Point(786, 18)
        Me.GroupBox5.Margin = New System.Windows.Forms.Padding(4)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Padding = New System.Windows.Forms.Padding(4)
        Me.GroupBox5.Size = New System.Drawing.Size(448, 149)
        Me.GroupBox5.TabIndex = 63
        Me.GroupBox5.TabStop = False
        '
        'discount
        '
        Me.discount.BackColor = System.Drawing.Color.Yellow
        Me.discount.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.discount.Location = New System.Drawing.Point(153, 47)
        Me.discount.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.discount.Name = "discount"
        Me.discount.Size = New System.Drawing.Size(67, 28)
        Me.discount.TabIndex = 25
        Me.discount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'invdis
        '
        Me.invdis.BackColor = System.Drawing.Color.Yellow
        Me.invdis.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.invdis.Location = New System.Drawing.Point(153, 80)
        Me.invdis.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.invdis.Name = "invdis"
        Me.invdis.Size = New System.Drawing.Size(276, 28)
        Me.invdis.TabIndex = 23
        Me.invdis.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'grandtotal
        '
        Me.grandtotal.BackColor = System.Drawing.Color.Yellow
        Me.grandtotal.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grandtotal.Location = New System.Drawing.Point(153, 113)
        Me.grandtotal.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.grandtotal.Name = "grandtotal"
        Me.grandtotal.Size = New System.Drawing.Size(276, 28)
        Me.grandtotal.TabIndex = 19
        Me.grandtotal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'totalamount
        '
        Me.totalamount.BackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.totalamount.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.totalamount.ForeColor = System.Drawing.Color.Black
        Me.totalamount.Location = New System.Drawing.Point(153, 12)
        Me.totalamount.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.totalamount.Name = "totalamount"
        Me.totalamount.Size = New System.Drawing.Size(276, 28)
        Me.totalamount.TabIndex = 13
        Me.totalamount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TextBox5
        '
        Me.TextBox5.Location = New System.Drawing.Point(63, 204)
        Me.TextBox5.Margin = New System.Windows.Forms.Padding(4)
        Me.TextBox5.Name = "TextBox5"
        Me.TextBox5.Size = New System.Drawing.Size(132, 26)
        Me.TextBox5.TabIndex = 9
        '
        'TextBox6
        '
        Me.TextBox6.Location = New System.Drawing.Point(233, 223)
        Me.TextBox6.Margin = New System.Windows.Forms.Padding(4)
        Me.TextBox6.Name = "TextBox6"
        Me.TextBox6.Size = New System.Drawing.Size(132, 26)
        Me.TextBox6.TabIndex = 7
        '
        'GroupBox6
        '
        Me.GroupBox6.Controls.Add(Label2)
        Me.GroupBox6.Controls.Add(Me.trans_dis)
        Me.GroupBox6.Controls.Add(Me.trans_itcode)
        Me.GroupBox6.Controls.Add(Label36)
        Me.GroupBox6.Controls.Add(Me.trans_unit)
        Me.GroupBox6.Controls.Add(Label31)
        Me.GroupBox6.Controls.Add(Me.trans_amount)
        Me.GroupBox6.Controls.Add(Label25)
        Me.GroupBox6.Controls.Add(Me.tran_cus)
        Me.GroupBox6.Controls.Add(Label26)
        Me.GroupBox6.Controls.Add(Label27)
        Me.GroupBox6.Controls.Add(Me.trans_qyt)
        Me.GroupBox6.Controls.Add(Label29)
        Me.GroupBox6.Controls.Add(Label30)
        Me.GroupBox6.Controls.Add(Me.trans_des)
        Me.GroupBox6.Controls.Add(Me.trans_invno)
        Me.GroupBox6.Location = New System.Drawing.Point(12, 18)
        Me.GroupBox6.Margin = New System.Windows.Forms.Padding(4)
        Me.GroupBox6.Name = "GroupBox6"
        Me.GroupBox6.Padding = New System.Windows.Forms.Padding(4)
        Me.GroupBox6.Size = New System.Drawing.Size(766, 149)
        Me.GroupBox6.TabIndex = 62
        Me.GroupBox6.TabStop = False
        '
        'trans_dis
        '
        Me.trans_dis.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.trans_dis.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.trans_dis.Location = New System.Drawing.Point(534, 110)
        Me.trans_dis.Margin = New System.Windows.Forms.Padding(4)
        Me.trans_dis.Name = "trans_dis"
        Me.trans_dis.Size = New System.Drawing.Size(77, 27)
        Me.trans_dis.TabIndex = 47
        '
        'trans_itcode
        '
        Me.trans_itcode.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.trans_itcode.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.trans_itcode.ForeColor = System.Drawing.Color.Black
        Me.trans_itcode.Location = New System.Drawing.Point(8, 107)
        Me.trans_itcode.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.trans_itcode.Name = "trans_itcode"
        Me.trans_itcode.Size = New System.Drawing.Size(105, 28)
        Me.trans_itcode.TabIndex = 46
        Me.trans_itcode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'trans_unit
        '
        Me.trans_unit.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.trans_unit.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.trans_unit.ForeColor = System.Drawing.Color.Black
        Me.trans_unit.Location = New System.Drawing.Point(530, 20)
        Me.trans_unit.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.trans_unit.Name = "trans_unit"
        Me.trans_unit.Size = New System.Drawing.Size(89, 28)
        Me.trans_unit.TabIndex = 44
        Me.trans_unit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'trans_amount
        '
        Me.trans_amount.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.trans_amount.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.trans_amount.Location = New System.Drawing.Point(619, 110)
        Me.trans_amount.Margin = New System.Windows.Forms.Padding(4)
        Me.trans_amount.Name = "trans_amount"
        Me.trans_amount.Size = New System.Drawing.Size(139, 27)
        Me.trans_amount.TabIndex = 38
        '
        'tran_cus
        '
        Me.tran_cus.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.tran_cus.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tran_cus.Location = New System.Drawing.Point(220, 52)
        Me.tran_cus.Margin = New System.Windows.Forms.Padding(4)
        Me.tran_cus.Name = "tran_cus"
        Me.tran_cus.Size = New System.Drawing.Size(538, 27)
        Me.tran_cus.TabIndex = 11
        '
        'trans_qyt
        '
        Me.trans_qyt.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.trans_qyt.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.trans_qyt.ForeColor = System.Drawing.Color.Black
        Me.trans_qyt.Location = New System.Drawing.Point(668, 20)
        Me.trans_qyt.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.trans_qyt.Name = "trans_qyt"
        Me.trans_qyt.Size = New System.Drawing.Size(89, 28)
        Me.trans_qyt.TabIndex = 6
        Me.trans_qyt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'trans_des
        '
        Me.trans_des.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.trans_des.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.trans_des.Location = New System.Drawing.Point(121, 110)
        Me.trans_des.Margin = New System.Windows.Forms.Padding(4)
        Me.trans_des.Name = "trans_des"
        Me.trans_des.Size = New System.Drawing.Size(405, 27)
        Me.trans_des.TabIndex = 1
        '
        'trans_invno
        '
        Me.trans_invno.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.trans_invno.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.trans_invno.Location = New System.Drawing.Point(5, 52)
        Me.trans_invno.Margin = New System.Windows.Forms.Padding(4)
        Me.trans_invno.Name = "trans_invno"
        Me.trans_invno.Size = New System.Drawing.Size(207, 27)
        Me.trans_invno.TabIndex = 0
        '
        'DataGridView2
        '
        Me.DataGridView2.BackgroundColor = System.Drawing.SystemColors.ButtonFace
        Me.DataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView2.Location = New System.Drawing.Point(14, 312)
        Me.DataGridView2.Name = "DataGridView2"
        Me.DataGridView2.Size = New System.Drawing.Size(1268, 626)
        Me.DataGridView2.TabIndex = 68
        '
        'SaleReturn
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1280, 950)
        Me.AutoScroll = True
        Me.Controls.Add(Me.GroupBox7)
        Me.Controls.Add(Me.GroupBox4)
        Me.Controls.Add(Me.DataGridView2)
        Me.Name = "SaleReturn"
        Me.Text = "SaleReturn"
        Me.GroupBox7.ResumeLayout(False)
        Me.GroupBox7.PerformLayout()
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        Me.GroupBox5.ResumeLayout(False)
        Me.GroupBox5.PerformLayout()
        Me.GroupBox6.ResumeLayout(False)
        Me.GroupBox6.PerformLayout()
        CType(Me.DataGridView2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GroupBox7 As GroupBox
    Friend WithEvents btnchange As Button
    Friend WithEvents btnprintinvoice As Button
    Friend WithEvents ComboBox5 As ComboBox
    Friend WithEvents btnSaveReturn As Button
    Friend WithEvents GroupBox4 As GroupBox
    Friend WithEvents DateTimePicker1 As DateTimePicker
    Friend WithEvents GroupBox5 As GroupBox
    Friend WithEvents discount As Label
    Friend WithEvents invdis As Label
    Friend WithEvents grandtotal As Label
    Friend WithEvents totalamount As Label
    Friend WithEvents TextBox5 As TextBox
    Friend WithEvents TextBox6 As TextBox
    Friend WithEvents GroupBox6 As GroupBox
    Friend WithEvents trans_itcode As Label
    Friend WithEvents trans_unit As Label
    Friend WithEvents trans_amount As TextBox
    Friend WithEvents tran_cus As TextBox
    Friend WithEvents trans_qyt As Label
    Friend WithEvents trans_des As TextBox
    Friend WithEvents trans_invno As TextBox
    Friend WithEvents DataGridView2 As DataGridView
    Friend WithEvents return_amt As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents return_debit As Label
    Friend WithEvents return_cash As TextBox
    Friend WithEvents trans_dis As TextBox
    Friend WithEvents lblLiveTimeDisplay As Label
    Friend WithEvents LiveTimer As Timer
    Friend WithEvents cmbCashier As ComboBox
    Friend WithEvents txtCashierID As TextBox
    Friend WithEvents remain_stock As Label
    Friend WithEvents btnReturnLog As Button
End Class
