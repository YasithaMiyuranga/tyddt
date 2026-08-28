<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmStock
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
        Dim LabelItemID As System.Windows.Forms.Label
        Dim LabelDes As System.Windows.Forms.Label
        Dim LabelTotalItems As System.Windows.Forms.Label
        Dim LabelBrand As System.Windows.Forms.Label
        Dim LabelTotlaCost As System.Windows.Forms.Label
        Dim LabelTotalRprice As System.Windows.Forms.Label
        Dim LabelTotalProfit As System.Windows.Forms.Label
        Dim LabelTotalWprice As System.Windows.Forms.Label
        Dim LabelTotalPrice As System.Windows.Forms.Label
        Dim Label2 As System.Windows.Forms.Label
        Dim Label4 As System.Windows.Forms.Label
        Dim Label6 As System.Windows.Forms.Label
        Dim LabelItemName As System.Windows.Forms.Label
        Me.LabelFLocation = New System.Windows.Forms.Label()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.Nprofit = New System.Windows.Forms.Label()
        Me.Wprofitlb = New System.Windows.Forms.Label()
        Me.Rprofitlb = New System.Windows.Forms.Label()
        Me.LTotalPrice = New System.Windows.Forms.Label()
        Me.WpriceLbl = New System.Windows.Forms.Label()
        Me.profitLbl = New System.Windows.Forms.Label()
        Me.RpriceLbl = New System.Windows.Forms.Label()
        Me.CostLbl = New System.Windows.Forms.Label()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.btnViewTotal = New System.Windows.Forms.Button()
        Me.TextBoxItemName = New System.Windows.Forms.TextBox()
        Me.btnViewBatch = New System.Windows.Forms.Button()
        Me.btnViewLocation = New System.Windows.Forms.Button()
        Me.ComboBoxFBrand = New System.Windows.Forms.ComboBox()
        Me.LabelTotalItem = New System.Windows.Forms.Label()
        Me.TextBoxFDescription = New System.Windows.Forms.TextBox()
        Me.ComboBoxFItemId = New System.Windows.Forms.ComboBox()
        Me.ComboBoxFLocation = New System.Windows.Forms.ComboBox()
        Me.ComboBoxPrinter = New System.Windows.Forms.ComboBox()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        LabelItemID = New System.Windows.Forms.Label()
        LabelDes = New System.Windows.Forms.Label()
        LabelTotalItems = New System.Windows.Forms.Label()
        LabelBrand = New System.Windows.Forms.Label()
        LabelTotlaCost = New System.Windows.Forms.Label()
        LabelTotalRprice = New System.Windows.Forms.Label()
        LabelTotalProfit = New System.Windows.Forms.Label()
        LabelTotalWprice = New System.Windows.Forms.Label()
        LabelTotalPrice = New System.Windows.Forms.Label()
        Label2 = New System.Windows.Forms.Label()
        Label4 = New System.Windows.Forms.Label()
        Label6 = New System.Windows.Forms.Label()
        LabelItemName = New System.Windows.Forms.Label()
        Me.TabPage1.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabControl1.SuspendLayout()
        Me.SuspendLayout()
        '
        'LabelItemID
        '
        LabelItemID.AutoSize = True
        LabelItemID.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        LabelItemID.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        LabelItemID.Location = New System.Drawing.Point(18, 13)
        LabelItemID.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        LabelItemID.Name = "LabelItemID"
        LabelItemID.Size = New System.Drawing.Size(83, 26)
        LabelItemID.TabIndex = 3
        LabelItemID.Text = "Item ID"
        '
        'LabelDes
        '
        LabelDes.AutoSize = True
        LabelDes.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        LabelDes.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        LabelDes.Location = New System.Drawing.Point(146, 72)
        LabelDes.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        LabelDes.Name = "LabelDes"
        LabelDes.Size = New System.Drawing.Size(121, 26)
        LabelDes.TabIndex = 5
        LabelDes.Text = "Description"
        '
        'LabelTotalItems
        '
        LabelTotalItems.AutoSize = True
        LabelTotalItems.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        LabelTotalItems.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        LabelTotalItems.Location = New System.Drawing.Point(21, 135)
        LabelTotalItems.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        LabelTotalItems.Name = "LabelTotalItems"
        LabelTotalItems.Size = New System.Drawing.Size(119, 26)
        LabelTotalItems.TabIndex = 7
        LabelTotalItems.Text = "Total Items"
        '
        'LabelBrand
        '
        LabelBrand.AutoSize = True
        LabelBrand.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        LabelBrand.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        LabelBrand.Location = New System.Drawing.Point(18, 74)
        LabelBrand.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        LabelBrand.Name = "LabelBrand"
        LabelBrand.Size = New System.Drawing.Size(70, 26)
        LabelBrand.TabIndex = 9
        LabelBrand.Text = "Brand"
        '
        'LabelTotlaCost
        '
        LabelTotlaCost.AutoSize = True
        LabelTotlaCost.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        LabelTotlaCost.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        LabelTotlaCost.Location = New System.Drawing.Point(156, 16)
        LabelTotlaCost.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        LabelTotlaCost.Name = "LabelTotlaCost"
        LabelTotlaCost.Size = New System.Drawing.Size(116, 26)
        LabelTotlaCost.TabIndex = 11
        LabelTotlaCost.Text = "Total Cost:"
        '
        'LabelTotalRprice
        '
        LabelTotalRprice.AutoSize = True
        LabelTotalRprice.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        LabelTotalRprice.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        LabelTotalRprice.Location = New System.Drawing.Point(2, 100)
        LabelTotalRprice.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        LabelTotalRprice.Name = "LabelTotalRprice"
        LabelTotalRprice.Size = New System.Drawing.Size(84, 26)
        LabelTotalRprice.TabIndex = 18
        LabelTotalRprice.Text = "RPrice:"
        '
        'LabelTotalProfit
        '
        LabelTotalProfit.AutoSize = True
        LabelTotalProfit.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        LabelTotalProfit.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        LabelTotalProfit.Location = New System.Drawing.Point(148, 131)
        LabelTotalProfit.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        LabelTotalProfit.Name = "LabelTotalProfit"
        LabelTotalProfit.Size = New System.Drawing.Size(122, 26)
        LabelTotalProfit.TabIndex = 20
        LabelTotalProfit.Text = "Total Profit:"
        '
        'LabelTotalWprice
        '
        LabelTotalWprice.AutoSize = True
        LabelTotalWprice.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        LabelTotalWprice.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        LabelTotalWprice.Location = New System.Drawing.Point(2, 72)
        LabelTotalWprice.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        LabelTotalWprice.Name = "LabelTotalWprice"
        LabelTotalWprice.Size = New System.Drawing.Size(89, 26)
        LabelTotalWprice.TabIndex = 22
        LabelTotalWprice.Text = "WPrice:"
        '
        'LabelTotalPrice
        '
        LabelTotalPrice.AutoSize = True
        LabelTotalPrice.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        LabelTotalPrice.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        LabelTotalPrice.Location = New System.Drawing.Point(4, 42)
        LabelTotalPrice.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        LabelTotalPrice.Name = "LabelTotalPrice"
        LabelTotalPrice.Size = New System.Drawing.Size(84, 26)
        LabelTotalPrice.TabIndex = 24
        LabelTotalPrice.Text = "NPrice:"
        '
        'Label2
        '
        Label2.AutoSize = True
        Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Label2.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Label2.Location = New System.Drawing.Point(381, 40)
        Label2.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Label2.Name = "Label2"
        Label2.Size = New System.Drawing.Size(91, 26)
        Label2.TabIndex = 30
        Label2.Text = "NProfit: "
        '
        'Label4
        '
        Label4.AutoSize = True
        Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Label4.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Label4.Location = New System.Drawing.Point(381, 69)
        Label4.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Label4.Name = "Label4"
        Label4.Size = New System.Drawing.Size(96, 26)
        Label4.TabIndex = 28
        Label4.Text = "WProfit: "
        '
        'Label6
        '
        Label6.AutoSize = True
        Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Label6.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Label6.Location = New System.Drawing.Point(387, 97)
        Label6.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Label6.Name = "Label6"
        Label6.Size = New System.Drawing.Size(85, 26)
        Label6.TabIndex = 26
        Label6.Text = "RProfit:"
        '
        'LabelItemName
        '
        LabelItemName.AutoSize = True
        LabelItemName.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        LabelItemName.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        LabelItemName.Location = New System.Drawing.Point(146, 13)
        LabelItemName.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        LabelItemName.Name = "LabelItemName"
        LabelItemName.Size = New System.Drawing.Size(120, 26)
        LabelItemName.TabIndex = 12
        LabelItemName.Text = "Item Name"
        '
        'LabelFLocation
        '
        Me.LabelFLocation.AutoSize = True
        Me.LabelFLocation.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelFLocation.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.LabelFLocation.Location = New System.Drawing.Point(146, 13)
        Me.LabelFLocation.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.LabelFLocation.Name = "LabelFLocation"
        Me.LabelFLocation.Size = New System.Drawing.Size(94, 26)
        Me.LabelFLocation.TabIndex = 67
        Me.LabelFLocation.Text = "Location"
        Me.LabelFLocation.Visible = False
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.GroupBox1)
        Me.TabPage1.Controls.Add(Me.DataGridView1)
        Me.TabPage1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TabPage1.Location = New System.Drawing.Point(4, 29)
        Me.TabPage1.Margin = New System.Windows.Forms.Padding(2)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(2)
        Me.TabPage1.Size = New System.Drawing.Size(1673, 422)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Current Stock"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.BackColor = System.Drawing.Color.FromArgb(CType(CType(44, Byte), Integer), CType(CType(62, Byte), Integer), CType(CType(80, Byte), Integer))
        Me.GroupBox1.Controls.Add(Me.GroupBox3)
        Me.GroupBox1.Controls.Add(Me.GroupBox2)
        Me.GroupBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox1.ForeColor = System.Drawing.Color.Yellow
        Me.GroupBox1.Location = New System.Drawing.Point(10, 4)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(2)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(2)
        Me.GroupBox1.Size = New System.Drawing.Size(1667, 192)
        Me.GroupBox1.TabIndex = 1
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Current Stock"
        '
        'GroupBox3
        '
        Me.GroupBox3.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.GroupBox3.Controls.Add(Me.Nprofit)
        Me.GroupBox3.Controls.Add(Label2)
        Me.GroupBox3.Controls.Add(Me.Wprofitlb)
        Me.GroupBox3.Controls.Add(Label4)
        Me.GroupBox3.Controls.Add(Me.Rprofitlb)
        Me.GroupBox3.Controls.Add(Label6)
        Me.GroupBox3.Controls.Add(Me.LTotalPrice)
        Me.GroupBox3.Controls.Add(LabelTotalPrice)
        Me.GroupBox3.Controls.Add(Me.WpriceLbl)
        Me.GroupBox3.Controls.Add(LabelTotalWprice)
        Me.GroupBox3.Controls.Add(Me.profitLbl)
        Me.GroupBox3.Controls.Add(LabelTotalProfit)
        Me.GroupBox3.Controls.Add(Me.RpriceLbl)
        Me.GroupBox3.Controls.Add(LabelTotalRprice)
        Me.GroupBox3.Controls.Add(Me.CostLbl)
        Me.GroupBox3.Controls.Add(LabelTotlaCost)
        Me.GroupBox3.Location = New System.Drawing.Point(957, 19)
        Me.GroupBox3.Margin = New System.Windows.Forms.Padding(2)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Padding = New System.Windows.Forms.Padding(2)
        Me.GroupBox3.Size = New System.Drawing.Size(685, 167)
        Me.GroupBox3.TabIndex = 63
        Me.GroupBox3.TabStop = False
        '
        'Nprofit
        '
        Me.Nprofit.BackColor = System.Drawing.Color.Yellow
        Me.Nprofit.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Nprofit.ForeColor = System.Drawing.Color.Black
        Me.Nprofit.Location = New System.Drawing.Point(492, 40)
        Me.Nprofit.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Nprofit.Name = "Nprofit"
        Me.Nprofit.Size = New System.Drawing.Size(146, 22)
        Me.Nprofit.TabIndex = 31
        Me.Nprofit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Wprofitlb
        '
        Me.Wprofitlb.BackColor = System.Drawing.Color.Yellow
        Me.Wprofitlb.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Wprofitlb.ForeColor = System.Drawing.Color.Black
        Me.Wprofitlb.Location = New System.Drawing.Point(492, 70)
        Me.Wprofitlb.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Wprofitlb.Name = "Wprofitlb"
        Me.Wprofitlb.Size = New System.Drawing.Size(146, 23)
        Me.Wprofitlb.TabIndex = 29
        Me.Wprofitlb.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Rprofitlb
        '
        Me.Rprofitlb.BackColor = System.Drawing.Color.Yellow
        Me.Rprofitlb.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Rprofitlb.ForeColor = System.Drawing.Color.Black
        Me.Rprofitlb.Location = New System.Drawing.Point(492, 101)
        Me.Rprofitlb.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Rprofitlb.Name = "Rprofitlb"
        Me.Rprofitlb.Size = New System.Drawing.Size(146, 22)
        Me.Rprofitlb.TabIndex = 27
        Me.Rprofitlb.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LTotalPrice
        '
        Me.LTotalPrice.BackColor = System.Drawing.Color.Yellow
        Me.LTotalPrice.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LTotalPrice.ForeColor = System.Drawing.Color.Black
        Me.LTotalPrice.Location = New System.Drawing.Point(90, 40)
        Me.LTotalPrice.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.LTotalPrice.Name = "LTotalPrice"
        Me.LTotalPrice.Size = New System.Drawing.Size(192, 24)
        Me.LTotalPrice.TabIndex = 25
        Me.LTotalPrice.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'WpriceLbl
        '
        Me.WpriceLbl.BackColor = System.Drawing.Color.Yellow
        Me.WpriceLbl.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.WpriceLbl.ForeColor = System.Drawing.Color.Black
        Me.WpriceLbl.Location = New System.Drawing.Point(90, 72)
        Me.WpriceLbl.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.WpriceLbl.Name = "WpriceLbl"
        Me.WpriceLbl.Size = New System.Drawing.Size(192, 21)
        Me.WpriceLbl.TabIndex = 23
        Me.WpriceLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'profitLbl
        '
        Me.profitLbl.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.profitLbl.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.profitLbl.ForeColor = System.Drawing.Color.Black
        Me.profitLbl.Location = New System.Drawing.Point(276, 131)
        Me.profitLbl.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.profitLbl.Name = "profitLbl"
        Me.profitLbl.Size = New System.Drawing.Size(246, 28)
        Me.profitLbl.TabIndex = 21
        Me.profitLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'RpriceLbl
        '
        Me.RpriceLbl.BackColor = System.Drawing.Color.Yellow
        Me.RpriceLbl.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RpriceLbl.ForeColor = System.Drawing.Color.Black
        Me.RpriceLbl.Location = New System.Drawing.Point(90, 100)
        Me.RpriceLbl.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.RpriceLbl.Name = "RpriceLbl"
        Me.RpriceLbl.Size = New System.Drawing.Size(192, 23)
        Me.RpriceLbl.TabIndex = 19
        Me.RpriceLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'CostLbl
        '
        Me.CostLbl.BackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.CostLbl.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CostLbl.ForeColor = System.Drawing.Color.Black
        Me.CostLbl.Location = New System.Drawing.Point(276, 16)
        Me.CostLbl.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.CostLbl.Name = "CostLbl"
        Me.CostLbl.Size = New System.Drawing.Size(140, 23)
        Me.CostLbl.TabIndex = 13
        Me.CostLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.btnPrint)
        Me.GroupBox2.Controls.Add(Me.btnViewTotal)
        Me.GroupBox2.Controls.Add(Me.TextBoxItemName)
        Me.GroupBox2.Controls.Add(Me.btnViewBatch)
        Me.GroupBox2.Controls.Add(LabelBrand)
        Me.GroupBox2.Controls.Add(Me.btnViewLocation)
        Me.GroupBox2.Controls.Add(Me.ComboBoxFBrand)
        Me.GroupBox2.Controls.Add(LabelTotalItems)
        Me.GroupBox2.Controls.Add(Me.LabelTotalItem)
        Me.GroupBox2.Controls.Add(LabelDes)
        Me.GroupBox2.Controls.Add(LabelItemID)
        Me.GroupBox2.Controls.Add(LabelItemName)
        Me.GroupBox2.Controls.Add(Me.TextBoxFDescription)
        Me.GroupBox2.Controls.Add(Me.ComboBoxFItemId)
        Me.GroupBox2.Controls.Add(Me.ComboBoxFLocation)
        Me.GroupBox2.Controls.Add(Me.LabelFLocation)
        Me.GroupBox2.Controls.Add(Me.ComboBoxPrinter)
        Me.GroupBox2.Location = New System.Drawing.Point(7, 25)
        Me.GroupBox2.Margin = New System.Windows.Forms.Padding(2)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Padding = New System.Windows.Forms.Padding(2)
        Me.GroupBox2.Size = New System.Drawing.Size(946, 167)
        Me.GroupBox2.TabIndex = 62
        Me.GroupBox2.TabStop = False
        '
        'btnPrint
        '
        Me.btnPrint.BackColor = System.Drawing.Color.SeaGreen
        Me.btnPrint.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnPrint.FlatAppearance.BorderSize = 0
        Me.btnPrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnPrint.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnPrint.ForeColor = System.Drawing.Color.White
        Me.btnPrint.Location = New System.Drawing.Point(589, 110)
        Me.btnPrint.Margin = New System.Windows.Forms.Padding(2)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(91, 38)
        Me.btnPrint.TabIndex = 35
        Me.btnPrint.Text = "Print"
        Me.btnPrint.UseVisualStyleBackColor = False
        '
        'btnViewTotal
        '
        Me.btnViewTotal.BackColor = System.Drawing.Color.DodgerBlue
        Me.btnViewTotal.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnViewTotal.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
        Me.btnViewTotal.ForeColor = System.Drawing.Color.White
        Me.btnViewTotal.Location = New System.Drawing.Point(499, 17)
        Me.btnViewTotal.Margin = New System.Windows.Forms.Padding(2)
        Me.btnViewTotal.Name = "btnViewTotal"
        Me.btnViewTotal.Size = New System.Drawing.Size(195, 39)
        Me.btnViewTotal.TabIndex = 64
        Me.btnViewTotal.Text = "TOTAL STOCK"
        Me.btnViewTotal.UseVisualStyleBackColor = False
        '
        'TextBoxItemName
        '
        Me.TextBoxItemName.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TextBoxItemName.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxItemName.Location = New System.Drawing.Point(150, 39)
        Me.TextBoxItemName.Margin = New System.Windows.Forms.Padding(2)
        Me.TextBoxItemName.Name = "TextBoxItemName"
        Me.TextBoxItemName.Size = New System.Drawing.Size(284, 32)
        Me.TextBoxItemName.TabIndex = 11
        '
        'btnViewBatch
        '
        Me.btnViewBatch.BackColor = System.Drawing.Color.DodgerBlue
        Me.btnViewBatch.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnViewBatch.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
        Me.btnViewBatch.ForeColor = System.Drawing.Color.White
        Me.btnViewBatch.Location = New System.Drawing.Point(714, 39)
        Me.btnViewBatch.Margin = New System.Windows.Forms.Padding(2)
        Me.btnViewBatch.Name = "btnViewBatch"
        Me.btnViewBatch.Size = New System.Drawing.Size(190, 40)
        Me.btnViewBatch.TabIndex = 65
        Me.btnViewBatch.Text = "BATCH VIEW"
        Me.btnViewBatch.UseVisualStyleBackColor = False
        '
        'btnViewLocation
        '
        Me.btnViewLocation.BackColor = System.Drawing.Color.DodgerBlue
        Me.btnViewLocation.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnViewLocation.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
        Me.btnViewLocation.ForeColor = System.Drawing.Color.White
        Me.btnViewLocation.Location = New System.Drawing.Point(499, 68)
        Me.btnViewLocation.Margin = New System.Windows.Forms.Padding(2)
        Me.btnViewLocation.Name = "btnViewLocation"
        Me.btnViewLocation.Size = New System.Drawing.Size(195, 38)
        Me.btnViewLocation.TabIndex = 66
        Me.btnViewLocation.Text = "BY LOCATION"
        Me.btnViewLocation.UseVisualStyleBackColor = False
        '
        'ComboBoxFBrand
        '
        Me.ComboBoxFBrand.BackColor = System.Drawing.Color.White
        Me.ComboBoxFBrand.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ComboBoxFBrand.FormattingEnabled = True
        Me.ComboBoxFBrand.Location = New System.Drawing.Point(22, 100)
        Me.ComboBoxFBrand.Margin = New System.Windows.Forms.Padding(2)
        Me.ComboBoxFBrand.Name = "ComboBoxFBrand"
        Me.ComboBoxFBrand.Size = New System.Drawing.Size(118, 33)
        Me.ComboBoxFBrand.TabIndex = 8
        '
        'LabelTotalItem
        '
        Me.LabelTotalItem.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.LabelTotalItem.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelTotalItem.ForeColor = System.Drawing.Color.Black
        Me.LabelTotalItem.Location = New System.Drawing.Point(150, 139)
        Me.LabelTotalItem.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.LabelTotalItem.Name = "LabelTotalItem"
        Me.LabelTotalItem.Size = New System.Drawing.Size(266, 22)
        Me.LabelTotalItem.TabIndex = 6
        '
        'TextBoxFDescription
        '
        Me.TextBoxFDescription.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TextBoxFDescription.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxFDescription.Location = New System.Drawing.Point(150, 101)
        Me.TextBoxFDescription.Margin = New System.Windows.Forms.Padding(2)
        Me.TextBoxFDescription.Name = "TextBoxFDescription"
        Me.TextBoxFDescription.Size = New System.Drawing.Size(284, 32)
        Me.TextBoxFDescription.TabIndex = 1
        '
        'ComboBoxFItemId
        '
        Me.ComboBoxFItemId.BackColor = System.Drawing.Color.White
        Me.ComboBoxFItemId.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ComboBoxFItemId.FormattingEnabled = True
        Me.ComboBoxFItemId.Location = New System.Drawing.Point(22, 39)
        Me.ComboBoxFItemId.Margin = New System.Windows.Forms.Padding(2)
        Me.ComboBoxFItemId.Name = "ComboBoxFItemId"
        Me.ComboBoxFItemId.Size = New System.Drawing.Size(118, 33)
        Me.ComboBoxFItemId.TabIndex = 0
        '
        'ComboBoxFLocation
        '
        Me.ComboBoxFLocation.BackColor = System.Drawing.Color.White
        Me.ComboBoxFLocation.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ComboBoxFLocation.FormattingEnabled = True
        Me.ComboBoxFLocation.Location = New System.Drawing.Point(150, 39)
        Me.ComboBoxFLocation.Margin = New System.Windows.Forms.Padding(2)
        Me.ComboBoxFLocation.Name = "ComboBoxFLocation"
        Me.ComboBoxFLocation.Size = New System.Drawing.Size(266, 33)
        Me.ComboBoxFLocation.TabIndex = 68
        Me.ComboBoxFLocation.Visible = False
        '
        'ComboBoxPrinter
        '
        Me.ComboBoxPrinter.FormattingEnabled = True
        Me.ComboBoxPrinter.Location = New System.Drawing.Point(438, 113)
        Me.ComboBoxPrinter.Margin = New System.Windows.Forms.Padding(2)
        Me.ComboBoxPrinter.Name = "ComboBoxPrinter"
        Me.ComboBoxPrinter.Size = New System.Drawing.Size(134, 33)
        Me.ComboBoxPrinter.TabIndex = 10
        '
        'DataGridView1
        '
        Me.DataGridView1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.DataGridView1.BackgroundColor = System.Drawing.SystemColors.ButtonFace
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Location = New System.Drawing.Point(10, 200)
        Me.DataGridView1.Margin = New System.Windows.Forms.Padding(2)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.RowHeadersWidth = 51
        Me.DataGridView1.Size = New System.Drawing.Size(1661, 220)
        Me.DataGridView1.TabIndex = 0
        '
        'TabControl1
        '
        Me.TabControl1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TabControl1.Location = New System.Drawing.Point(0, 0)
        Me.TabControl1.Margin = New System.Windows.Forms.Padding(2)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(1681, 455)
        Me.TabControl1.TabIndex = 0
        '
        'frmStock
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 17.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1681, 459)
        Me.Controls.Add(Me.TabControl1)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.Name = "frmStock"
        Me.Text = "Stock"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.TabPage1.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabControl1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TabPage1 As TabPage
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents LTotalPrice As Label
    Friend WithEvents WpriceLbl As Label
    Friend WithEvents profitLbl As Label
    Friend WithEvents RpriceLbl As Label
    Friend WithEvents CostLbl As Label
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents btnPrint As Button
    Friend WithEvents TextBoxItemName As TextBox
    Friend WithEvents ComboBoxFBrand As ComboBox
    Friend WithEvents LabelTotalItem As Label
    Friend WithEvents TextBoxFDescription As TextBox
    Friend WithEvents ComboBoxFItemId As ComboBox
    Friend WithEvents ComboBoxPrinter As ComboBox
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents Nprofit As Label
    Friend WithEvents Wprofitlb As Label
    Friend WithEvents Rprofitlb As Label
    Friend WithEvents btnViewTotal As Button
    Friend WithEvents btnViewBatch As Button
    Friend WithEvents btnViewLocation As Button
    Friend WithEvents ComboBoxFLocation As ComboBox
    Friend WithEvents LabelFLocation As Label
End Class