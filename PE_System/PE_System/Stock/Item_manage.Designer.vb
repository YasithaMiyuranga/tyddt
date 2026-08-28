<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Item_manage
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
        Me.LiveTimer = New System.Windows.Forms.Timer(Me.components)
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.Filters = New System.Windows.Forms.GroupBox()
        Me.TextBoxFDescription = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TextBoxFItemName = New System.Windows.Forms.TextBox()
        Me.cmbFilterType = New System.Windows.Forms.ComboBox()
        Me.LabelFCategory = New System.Windows.Forms.Label()
        Me.LabelFBrand = New System.Windows.Forms.Label()
        Me.ComboBoxFCategory = New System.Windows.Forms.ComboBox()
        Me.ComboBoxFBrand = New System.Windows.Forms.ComboBox()
        Me.LabelFItemName = New System.Windows.Forms.Label()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.TextBoxWPrice = New System.Windows.Forms.TextBox()
        Me.LabelWPrice = New System.Windows.Forms.Label()
        Me.TextBoxRPrice = New System.Windows.Forms.TextBox()
        Me.LabelRPrice = New System.Windows.Forms.Label()
        Me.LabelSupplier = New System.Windows.Forms.Label()
        Me.ComboBoxSupplier = New System.Windows.Forms.ComboBox()
        Me.btnBarcode = New System.Windows.Forms.Button()
        Me.btnAddToList = New System.Windows.Forms.Button()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.btnSuccess = New System.Windows.Forms.Button()
        Me.btnLogs = New System.Windows.Forms.Button()
        Me.btnUpdate = New System.Windows.Forms.Button()
        Me.ComboBoxBrand = New System.Windows.Forms.ComboBox()
        Me.ComboBoxCategory = New System.Windows.Forms.ComboBox()
        Me.TextBoxDis = New System.Windows.Forms.TextBox()
        Me.LabelDis = New System.Windows.Forms.Label()
        Me.cmbAuthUser = New System.Windows.Forms.ComboBox()
        Me.LabelAuthUser = New System.Windows.Forms.Label()
        Me.txtReadOnly = New System.Windows.Forms.TextBox()
        Me.LabelBrand = New System.Windows.Forms.Label()
        Me.TextBoxAvgCost = New System.Windows.Forms.TextBox()
        Me.LabelAvgCost = New System.Windows.Forms.Label()
        Me.LabelCategory = New System.Windows.Forms.Label()
        Me.btnAddNew = New System.Windows.Forms.Button()
        Me.ComboBoxSuMethod = New System.Windows.Forms.ComboBox()
        Me.ComboBoxMeasure = New System.Windows.Forms.ComboBox()
        Me.CheckBoxIsActive = New System.Windows.Forms.CheckBox()
        Me.LabelMeasure = New System.Windows.Forms.Label()
        Me.LabelSuppMethod = New System.Windows.Forms.Label()
        Me.TextBoxStockAlert = New System.Windows.Forms.TextBox()
        Me.LabelStockAlert = New System.Windows.Forms.Label()
        Me.TextBoxStockQyt = New System.Windows.Forms.TextBox()
        Me.LabelStockQyt = New System.Windows.Forms.Label()
        Me.LabelItemCost = New System.Windows.Forms.Label()
        Me.TextBoxItemCost = New System.Windows.Forms.TextBox()
        Me.TextBoxDes = New System.Windows.Forms.TextBox()
        Me.LabelDes = New System.Windows.Forms.Label()
        Me.TextBoxItemName = New System.Windows.Forms.TextBox()
        Me.TextBoxProMargin = New System.Windows.Forms.TextBox()
        Me.LabelSellPrice = New System.Windows.Forms.Label()
        Me.LabelName = New System.Windows.Forms.Label()
        Me.LabelProMargine = New System.Windows.Forms.Label()
        Me.TextBoxItemId = New System.Windows.Forms.TextBox()
        Me.TextBoxSellPrice = New System.Windows.Forms.TextBox()
        Me.labelItemId = New System.Windows.Forms.Label()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Filters.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'LiveTimer
        '
        Me.LiveTimer.Enabled = True
        Me.LiveTimer.Interval = 1000
        '
        'DataGridView1
        '
        Me.DataGridView1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.DataGridView1.BackgroundColor = System.Drawing.SystemColors.ButtonFace
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Location = New System.Drawing.Point(16, 307)
        Me.DataGridView1.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.RowHeadersVisible = False
        Me.DataGridView1.RowHeadersWidth = 51
        Me.DataGridView1.Size = New System.Drawing.Size(1653, 422)
        Me.DataGridView1.TabIndex = 50
        '
        'Filters
        '
        Me.Filters.BackColor = System.Drawing.Color.DarkSlateGray
        Me.Filters.Controls.Add(Me.TextBoxFDescription)
        Me.Filters.Controls.Add(Me.Label1)
        Me.Filters.Controls.Add(Me.TextBoxFItemName)
        Me.Filters.Controls.Add(Me.cmbFilterType)
        Me.Filters.Controls.Add(Me.LabelFCategory)
        Me.Filters.Controls.Add(Me.LabelFBrand)
        Me.Filters.Controls.Add(Me.ComboBoxFCategory)
        Me.Filters.Controls.Add(Me.ComboBoxFBrand)
        Me.Filters.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Filters.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.Filters.Location = New System.Drawing.Point(16, 234)
        Me.Filters.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.Filters.Name = "Filters"
        Me.Filters.Padding = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.Filters.Size = New System.Drawing.Size(1653, 65)
        Me.Filters.TabIndex = 2
        Me.Filters.TabStop = False
        Me.Filters.Text = "Filters"
        '
        'TextBoxFDescription
        '
        Me.TextBoxFDescription.Location = New System.Drawing.Point(565, 23)
        Me.TextBoxFDescription.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.TextBoxFDescription.Name = "TextBoxFDescription"
        Me.TextBoxFDescription.Size = New System.Drawing.Size(529, 30)
        Me.TextBoxFDescription.TabIndex = 41
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.Label1.Location = New System.Drawing.Point(439, 28)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(109, 25)
        Me.Label1.TabIndex = 46
        Me.Label1.Text = "Description"
        '
        'TextBoxFItemName
        '
        Me.TextBoxFItemName.Location = New System.Drawing.Point(220, 23)
        Me.TextBoxFItemName.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.TextBoxFItemName.Name = "TextBoxFItemName"
        Me.TextBoxFItemName.Size = New System.Drawing.Size(200, 30)
        Me.TextBoxFItemName.TabIndex = 40
        '
        'cmbFilterType
        '
        Me.cmbFilterType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbFilterType.FormattingEnabled = True
        Me.cmbFilterType.Items.AddRange(New Object() {"Item ID"})
        Me.cmbFilterType.Location = New System.Drawing.Point(13, 23)
        Me.cmbFilterType.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.cmbFilterType.Name = "cmbFilterType"
        Me.cmbFilterType.Size = New System.Drawing.Size(200, 33)
        Me.cmbFilterType.TabIndex = 39
        '
        'LabelFCategory
        '
        Me.LabelFCategory.AutoSize = True
        Me.LabelFCategory.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelFCategory.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.LabelFCategory.Location = New System.Drawing.Point(1352, 26)
        Me.LabelFCategory.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LabelFCategory.Name = "LabelFCategory"
        Me.LabelFCategory.Size = New System.Drawing.Size(92, 25)
        Me.LabelFCategory.TabIndex = 40
        Me.LabelFCategory.Text = "Category"
        '
        'LabelFBrand
        '
        Me.LabelFBrand.AutoSize = True
        Me.LabelFBrand.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelFBrand.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.LabelFBrand.Location = New System.Drawing.Point(1112, 28)
        Me.LabelFBrand.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LabelFBrand.Name = "LabelFBrand"
        Me.LabelFBrand.Size = New System.Drawing.Size(64, 25)
        Me.LabelFBrand.TabIndex = 42
        Me.LabelFBrand.Text = "Brand"
        '
        'ComboBoxFCategory
        '
        Me.ComboBoxFCategory.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.ComboBoxFCategory.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.ComboBoxFCategory.FormattingEnabled = True
        Me.ComboBoxFCategory.Items.AddRange(New Object() {"PCS", "BOX", "BAG", "Bar", "CARTON", "CARD", "DAYS", "DOZ PAIR", "EACH", "FEET", "GRAM", "INCH", "KG", "Length", "Meter", "Mile", "MLS", "NO", "SET", "SHEET", "YARD"})
        Me.ComboBoxFCategory.Location = New System.Drawing.Point(1455, 23)
        Me.ComboBoxFCategory.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.ComboBoxFCategory.Name = "ComboBoxFCategory"
        Me.ComboBoxFCategory.Size = New System.Drawing.Size(160, 33)
        Me.ComboBoxFCategory.TabIndex = 43
        '
        'ComboBoxFBrand
        '
        Me.ComboBoxFBrand.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.ComboBoxFBrand.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.ComboBoxFBrand.FormattingEnabled = True
        Me.ComboBoxFBrand.Items.AddRange(New Object() {"PCS", "BOX", "BAG", "Bar", "CARTON", "CARD", "DAYS", "DOZ PAIR", "EACH", "FEET", "GRAM", "INCH", "KG", "Length", "Meter", "Mile", "MLS", "NO", "SET", "SHEET", "YARD"})
        Me.ComboBoxFBrand.Location = New System.Drawing.Point(1184, 23)
        Me.ComboBoxFBrand.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.ComboBoxFBrand.Name = "ComboBoxFBrand"
        Me.ComboBoxFBrand.Size = New System.Drawing.Size(160, 33)
        Me.ComboBoxFBrand.TabIndex = 42
        '
        'LabelFItemName
        '
        Me.LabelFItemName.Location = New System.Drawing.Point(0, 0)
        Me.LabelFItemName.Name = "LabelFItemName"
        Me.LabelFItemName.Size = New System.Drawing.Size(100, 23)
        Me.LabelFItemName.TabIndex = 0
        Me.LabelFItemName.Visible = False
        '
        'GroupBox3
        '
        Me.GroupBox3.BackColor = System.Drawing.Color.FromArgb(CType(CType(44, Byte), Integer), CType(CType(62, Byte), Integer), CType(CType(80, Byte), Integer))
        Me.GroupBox3.Controls.Add(Me.GroupBox1)
        Me.GroupBox3.Location = New System.Drawing.Point(16, 9)
        Me.GroupBox3.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Padding = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.GroupBox3.Size = New System.Drawing.Size(1653, 226)
        Me.GroupBox3.TabIndex = 1
        Me.GroupBox3.TabStop = False
        '
        'GroupBox1
        '
        Me.GroupBox1.BackColor = System.Drawing.Color.Transparent
        Me.GroupBox1.Controls.Add(Me.TextBoxWPrice)
        Me.GroupBox1.Controls.Add(Me.LabelWPrice)
        Me.GroupBox1.Controls.Add(Me.TextBoxRPrice)
        Me.GroupBox1.Controls.Add(Me.LabelRPrice)
        Me.GroupBox1.Controls.Add(Me.LabelSupplier)
        Me.GroupBox1.Controls.Add(Me.ComboBoxSupplier)
        Me.GroupBox1.Controls.Add(Me.btnBarcode)
        Me.GroupBox1.Controls.Add(Me.btnAddToList)
        Me.GroupBox1.Controls.Add(Me.btnDelete)
        Me.GroupBox1.Controls.Add(Me.btnSuccess)
        Me.GroupBox1.Controls.Add(Me.btnLogs)
        Me.GroupBox1.Controls.Add(Me.btnUpdate)
        Me.GroupBox1.Controls.Add(Me.ComboBoxBrand)
        Me.GroupBox1.Controls.Add(Me.ComboBoxCategory)
        Me.GroupBox1.Controls.Add(Me.TextBoxDis)
        Me.GroupBox1.Controls.Add(Me.LabelDis)
        Me.GroupBox1.Controls.Add(Me.cmbAuthUser)
        Me.GroupBox1.Controls.Add(Me.LabelAuthUser)
        Me.GroupBox1.Controls.Add(Me.txtReadOnly)
        Me.GroupBox1.Controls.Add(Me.LabelBrand)
        Me.GroupBox1.Controls.Add(Me.TextBoxAvgCost)
        Me.GroupBox1.Controls.Add(Me.LabelAvgCost)
        Me.GroupBox1.Controls.Add(Me.LabelCategory)
        Me.GroupBox1.Controls.Add(Me.btnAddNew)
        Me.GroupBox1.Controls.Add(Me.ComboBoxSuMethod)
        Me.GroupBox1.Controls.Add(Me.ComboBoxMeasure)
        Me.GroupBox1.Controls.Add(Me.CheckBoxIsActive)
        Me.GroupBox1.Controls.Add(Me.LabelMeasure)
        Me.GroupBox1.Controls.Add(Me.LabelSuppMethod)
        Me.GroupBox1.Controls.Add(Me.TextBoxStockAlert)
        Me.GroupBox1.Controls.Add(Me.LabelStockAlert)
        Me.GroupBox1.Controls.Add(Me.TextBoxStockQyt)
        Me.GroupBox1.Controls.Add(Me.LabelStockQyt)
        Me.GroupBox1.Controls.Add(Me.LabelItemCost)
        Me.GroupBox1.Controls.Add(Me.TextBoxItemCost)
        Me.GroupBox1.Controls.Add(Me.TextBoxDes)
        Me.GroupBox1.Controls.Add(Me.LabelDes)
        Me.GroupBox1.Controls.Add(Me.TextBoxItemName)
        Me.GroupBox1.Controls.Add(Me.TextBoxProMargin)
        Me.GroupBox1.Controls.Add(Me.LabelSellPrice)
        Me.GroupBox1.Controls.Add(Me.LabelName)
        Me.GroupBox1.Controls.Add(Me.LabelProMargine)
        Me.GroupBox1.Controls.Add(Me.TextBoxItemId)
        Me.GroupBox1.Controls.Add(Me.TextBoxSellPrice)
        Me.GroupBox1.Controls.Add(Me.labelItemId)
        Me.GroupBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.GroupBox1.Location = New System.Drawing.Point(8, 9)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.GroupBox1.Size = New System.Drawing.Size(1636, 213)
        Me.GroupBox1.TabIndex = 1
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Item Actions"
        '
        'TextBoxWPrice
        '
        Me.TextBoxWPrice.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TextBoxWPrice.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxWPrice.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxWPrice.Location = New System.Drawing.Point(433, 107)
        Me.TextBoxWPrice.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.TextBoxWPrice.Name = "TextBoxWPrice"
        Me.TextBoxWPrice.Size = New System.Drawing.Size(213, 30)
        Me.TextBoxWPrice.TabIndex = 123
        '
        'LabelWPrice
        '
        Me.LabelWPrice.AutoSize = True
        Me.LabelWPrice.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelWPrice.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.LabelWPrice.Location = New System.Drawing.Point(349, 107)
        Me.LabelWPrice.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LabelWPrice.Name = "LabelWPrice"
        Me.LabelWPrice.Size = New System.Drawing.Size(76, 25)
        Me.LabelWPrice.TabIndex = 124
        Me.LabelWPrice.Text = "WPrice"
        '
        'TextBoxRPrice
        '
        Me.TextBoxRPrice.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TextBoxRPrice.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxRPrice.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxRPrice.Location = New System.Drawing.Point(736, 106)
        Me.TextBoxRPrice.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.TextBoxRPrice.Name = "TextBoxRPrice"
        Me.TextBoxRPrice.Size = New System.Drawing.Size(213, 30)
        Me.TextBoxRPrice.TabIndex = 121
        '
        'LabelRPrice
        '
        Me.LabelRPrice.AutoSize = True
        Me.LabelRPrice.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelRPrice.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.LabelRPrice.Location = New System.Drawing.Point(659, 107)
        Me.LabelRPrice.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LabelRPrice.Name = "LabelRPrice"
        Me.LabelRPrice.Size = New System.Drawing.Size(69, 25)
        Me.LabelRPrice.TabIndex = 122
        Me.LabelRPrice.Text = "RPrice"
        '
        'LabelSupplier
        '
        Me.LabelSupplier.AutoSize = True
        Me.LabelSupplier.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelSupplier.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.LabelSupplier.Location = New System.Drawing.Point(232, 177)
        Me.LabelSupplier.Name = "LabelSupplier"
        Me.LabelSupplier.Size = New System.Drawing.Size(84, 25)
        Me.LabelSupplier.TabIndex = 120
        Me.LabelSupplier.Text = "Supplier"
        '
        'ComboBoxSupplier
        '
        Me.ComboBoxSupplier.FormattingEnabled = True
        Me.ComboBoxSupplier.Location = New System.Drawing.Point(331, 172)
        Me.ComboBoxSupplier.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.ComboBoxSupplier.Name = "ComboBoxSupplier"
        Me.ComboBoxSupplier.Size = New System.Drawing.Size(164, 33)
        Me.ComboBoxSupplier.TabIndex = 27
        '
        'btnBarcode
        '
        Me.btnBarcode.BackColor = System.Drawing.Color.DodgerBlue
        Me.btnBarcode.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnBarcode.FlatAppearance.BorderSize = 0
        Me.btnBarcode.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnBarcode.Font = New System.Drawing.Font("Segoe UI", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnBarcode.ForeColor = System.Drawing.Color.White
        Me.btnBarcode.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnBarcode.Location = New System.Drawing.Point(968, 149)
        Me.btnBarcode.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.btnBarcode.Name = "btnBarcode"
        Me.btnBarcode.Size = New System.Drawing.Size(117, 50)
        Me.btnBarcode.TabIndex = 28
        Me.btnBarcode.Text = "Barcode"
        Me.btnBarcode.UseVisualStyleBackColor = False
        '
        'btnAddToList
        '
        Me.btnAddToList.BackColor = System.Drawing.Color.DarkSlateBlue
        Me.btnAddToList.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnAddToList.FlatAppearance.BorderSize = 0
        Me.btnAddToList.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnAddToList.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnAddToList.ForeColor = System.Drawing.Color.White
        Me.btnAddToList.Location = New System.Drawing.Point(827, 149)
        Me.btnAddToList.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.btnAddToList.Name = "btnAddToList"
        Me.btnAddToList.Size = New System.Drawing.Size(135, 50)
        Me.btnAddToList.TabIndex = 28
        Me.btnAddToList.Text = "Add to List"
        Me.btnAddToList.UseVisualStyleBackColor = False
        '
        'btnDelete
        '
        Me.btnDelete.BackColor = System.Drawing.Color.Crimson
        Me.btnDelete.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnDelete.FlatAppearance.BorderSize = 0
        Me.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnDelete.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnDelete.ForeColor = System.Drawing.Color.White
        Me.btnDelete.Location = New System.Drawing.Point(1440, 149)
        Me.btnDelete.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(87, 53)
        Me.btnDelete.TabIndex = 32
        Me.btnDelete.Text = "Delete"
        Me.btnDelete.UseVisualStyleBackColor = False
        '
        'btnSuccess
        '
        Me.btnSuccess.BackColor = System.Drawing.Color.DeepSkyBlue
        Me.btnSuccess.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnSuccess.FlatAppearance.BorderSize = 0
        Me.btnSuccess.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnSuccess.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSuccess.ForeColor = System.Drawing.Color.Black
        Me.btnSuccess.Location = New System.Drawing.Point(1093, 150)
        Me.btnSuccess.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.btnSuccess.Name = "btnSuccess"
        Me.btnSuccess.Size = New System.Drawing.Size(115, 53)
        Me.btnSuccess.TabIndex = 30
        Me.btnSuccess.Text = "Save"
        Me.btnSuccess.UseVisualStyleBackColor = False
        '
        'btnLogs
        '
        Me.btnLogs.BackColor = System.Drawing.Color.DimGray
        Me.btnLogs.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnLogs.FlatAppearance.BorderSize = 0
        Me.btnLogs.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnLogs.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnLogs.ForeColor = System.Drawing.Color.White
        Me.btnLogs.Location = New System.Drawing.Point(1535, 153)
        Me.btnLogs.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.btnLogs.Name = "btnLogs"
        Me.btnLogs.Size = New System.Drawing.Size(79, 53)
        Me.btnLogs.TabIndex = 33
        Me.btnLogs.Text = "Logs"
        Me.btnLogs.UseVisualStyleBackColor = False
        '
        'btnUpdate
        '
        Me.btnUpdate.BackColor = System.Drawing.Color.White
        Me.btnUpdate.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnUpdate.FlatAppearance.BorderSize = 0
        Me.btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnUpdate.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnUpdate.ForeColor = System.Drawing.Color.Black
        Me.btnUpdate.Location = New System.Drawing.Point(1340, 149)
        Me.btnUpdate.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.btnUpdate.Name = "btnUpdate"
        Me.btnUpdate.Size = New System.Drawing.Size(92, 53)
        Me.btnUpdate.TabIndex = 31
        Me.btnUpdate.Text = "Edit"
        Me.btnUpdate.UseVisualStyleBackColor = False
        '
        'ComboBoxBrand
        '
        Me.ComboBoxBrand.FormattingEnabled = True
        Me.ComboBoxBrand.Items.AddRange(New Object() {"Loca", "Import", "selfproduct"})
        Me.ComboBoxBrand.Location = New System.Drawing.Point(85, 139)
        Me.ComboBoxBrand.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.ComboBoxBrand.Name = "ComboBoxBrand"
        Me.ComboBoxBrand.Size = New System.Drawing.Size(131, 33)
        Me.ComboBoxBrand.TabIndex = 22
        '
        'ComboBoxCategory
        '
        Me.ComboBoxCategory.FormattingEnabled = True
        Me.ComboBoxCategory.Items.AddRange(New Object() {"Loca", "Import", "selfproduct"})
        Me.ComboBoxCategory.Location = New System.Drawing.Point(331, 139)
        Me.ComboBoxCategory.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.ComboBoxCategory.Name = "ComboBoxCategory"
        Me.ComboBoxCategory.Size = New System.Drawing.Size(164, 33)
        Me.ComboBoxCategory.TabIndex = 23
        '
        'TextBoxDis
        '
        Me.TextBoxDis.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TextBoxDis.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxDis.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxDis.Location = New System.Drawing.Point(1429, 107)
        Me.TextBoxDis.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.TextBoxDis.Name = "TextBoxDis"
        Me.TextBoxDis.Size = New System.Drawing.Size(186, 30)
        Me.TextBoxDis.TabIndex = 21
        '
        'LabelDis
        '
        Me.LabelDis.AutoSize = True
        Me.LabelDis.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelDis.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.LabelDis.Location = New System.Drawing.Point(1304, 107)
        Me.LabelDis.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LabelDis.Name = "LabelDis"
        Me.LabelDis.Size = New System.Drawing.Size(63, 25)
        Me.LabelDis.TabIndex = 38
        Me.LabelDis.Text = "Dis %"
        '
        'cmbAuthUser
        '
        Me.cmbAuthUser.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.cmbAuthUser.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cmbAuthUser.FormattingEnabled = True
        Me.cmbAuthUser.Location = New System.Drawing.Point(688, 10)
        Me.cmbAuthUser.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.cmbAuthUser.Name = "cmbAuthUser"
        Me.cmbAuthUser.Size = New System.Drawing.Size(212, 33)
        Me.cmbAuthUser.TabIndex = 10
        '
        'LabelAuthUser
        '
        Me.LabelAuthUser.AutoSize = True
        Me.LabelAuthUser.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelAuthUser.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.LabelAuthUser.Location = New System.Drawing.Point(555, 15)
        Me.LabelAuthUser.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LabelAuthUser.Name = "LabelAuthUser"
        Me.LabelAuthUser.Size = New System.Drawing.Size(108, 20)
        Me.LabelAuthUser.TabIndex = 101
        Me.LabelAuthUser.Text = "Select User"
        '
        'txtReadOnly
        '
        Me.txtReadOnly.BackColor = System.Drawing.Color.FromArgb(CType(CType(44, Byte), Integer), CType(CType(62, Byte), Integer), CType(CType(80, Byte), Integer))
        Me.txtReadOnly.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtReadOnly.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtReadOnly.Location = New System.Drawing.Point(919, 14)
        Me.txtReadOnly.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.txtReadOnly.Name = "txtReadOnly"
        Me.txtReadOnly.Size = New System.Drawing.Size(167, 23)
        Me.txtReadOnly.TabIndex = 11
        Me.txtReadOnly.UseSystemPasswordChar = True
        '
        'LabelBrand
        '
        Me.LabelBrand.AutoSize = True
        Me.LabelBrand.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelBrand.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.LabelBrand.Location = New System.Drawing.Point(19, 141)
        Me.LabelBrand.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LabelBrand.Name = "LabelBrand"
        Me.LabelBrand.Size = New System.Drawing.Size(64, 25)
        Me.LabelBrand.TabIndex = 8
        Me.LabelBrand.Text = "Brand"
        '
        'TextBoxAvgCost
        '
        Me.TextBoxAvgCost.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TextBoxAvgCost.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxAvgCost.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxAvgCost.Location = New System.Drawing.Point(1068, 107)
        Me.TextBoxAvgCost.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.TextBoxAvgCost.Name = "TextBoxAvgCost"
        Me.TextBoxAvgCost.Size = New System.Drawing.Size(229, 30)
        Me.TextBoxAvgCost.TabIndex = 20
        '
        'LabelAvgCost
        '
        Me.LabelAvgCost.AutoSize = True
        Me.LabelAvgCost.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelAvgCost.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.LabelAvgCost.Location = New System.Drawing.Point(963, 112)
        Me.LabelAvgCost.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LabelAvgCost.Name = "LabelAvgCost"
        Me.LabelAvgCost.Size = New System.Drawing.Size(98, 25)
        Me.LabelAvgCost.TabIndex = 20
        Me.LabelAvgCost.Text = "Avg. Cost"
        '
        'LabelCategory
        '
        Me.LabelCategory.AutoSize = True
        Me.LabelCategory.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelCategory.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.LabelCategory.Location = New System.Drawing.Point(229, 143)
        Me.LabelCategory.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LabelCategory.Name = "LabelCategory"
        Me.LabelCategory.Size = New System.Drawing.Size(92, 25)
        Me.LabelCategory.TabIndex = 10
        Me.LabelCategory.Text = "Category"
        '
        'btnAddNew
        '
        Me.btnAddNew.BackColor = System.Drawing.Color.White
        Me.btnAddNew.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnAddNew.FlatAppearance.BorderSize = 0
        Me.btnAddNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnAddNew.Font = New System.Drawing.Font("Segoe UI", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnAddNew.ForeColor = System.Drawing.Color.Black
        Me.btnAddNew.Location = New System.Drawing.Point(1217, 150)
        Me.btnAddNew.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.btnAddNew.Name = "btnAddNew"
        Me.btnAddNew.Size = New System.Drawing.Size(115, 52)
        Me.btnAddNew.TabIndex = 29
        Me.btnAddNew.Text = "Add New"
        Me.btnAddNew.UseVisualStyleBackColor = False
        '
        'ComboBoxSuMethod
        '
        Me.ComboBoxSuMethod.FormattingEnabled = True
        Me.ComboBoxSuMethod.Items.AddRange(New Object() {"Local", "Import", "selfproduct"})
        Me.ComboBoxSuMethod.Location = New System.Drawing.Point(620, 175)
        Me.ComboBoxSuMethod.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.ComboBoxSuMethod.Name = "ComboBoxSuMethod"
        Me.ComboBoxSuMethod.Size = New System.Drawing.Size(189, 33)
        Me.ComboBoxSuMethod.TabIndex = 25
        '
        'ComboBoxMeasure
        '
        Me.ComboBoxMeasure.FormattingEnabled = True
        Me.ComboBoxMeasure.Items.AddRange(New Object() {"PCS", "BOX", "BAG", "Bar", "CARTON", "CARD", "DAYS", "DOZ PAIR", "EACH", "FEET", "GRAM", "INCH", "KG", "Length", "Meter", "Mile", "MLS", "NO", "SET", "SHEET", "YARD"})
        Me.ComboBoxMeasure.Location = New System.Drawing.Point(620, 139)
        Me.ComboBoxMeasure.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.ComboBoxMeasure.Name = "ComboBoxMeasure"
        Me.ComboBoxMeasure.Size = New System.Drawing.Size(189, 33)
        Me.ComboBoxMeasure.TabIndex = 24
        '
        'CheckBoxIsActive
        '
        Me.CheckBoxIsActive.AutoSize = True
        Me.CheckBoxIsActive.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.CheckBoxIsActive.Location = New System.Drawing.Point(24, 175)
        Me.CheckBoxIsActive.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.CheckBoxIsActive.Name = "CheckBoxIsActive"
        Me.CheckBoxIsActive.Size = New System.Drawing.Size(111, 29)
        Me.CheckBoxIsActive.TabIndex = 26
        Me.CheckBoxIsActive.Text = "IsActive"
        Me.CheckBoxIsActive.UseVisualStyleBackColor = True
        '
        'LabelMeasure
        '
        Me.LabelMeasure.AutoSize = True
        Me.LabelMeasure.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelMeasure.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.LabelMeasure.Location = New System.Drawing.Point(523, 141)
        Me.LabelMeasure.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LabelMeasure.Name = "LabelMeasure"
        Me.LabelMeasure.Size = New System.Drawing.Size(88, 25)
        Me.LabelMeasure.TabIndex = 27
        Me.LabelMeasure.Text = "measure"
        '
        'LabelSuppMethod
        '
        Me.LabelSuppMethod.AutoSize = True
        Me.LabelSuppMethod.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelSuppMethod.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.LabelSuppMethod.Location = New System.Drawing.Point(503, 180)
        Me.LabelSuppMethod.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LabelSuppMethod.Name = "LabelSuppMethod"
        Me.LabelSuppMethod.Size = New System.Drawing.Size(108, 25)
        Me.LabelSuppMethod.TabIndex = 26
        Me.LabelSuppMethod.Text = "Su.Method"
        '
        'TextBoxStockAlert
        '
        Me.TextBoxStockAlert.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TextBoxStockAlert.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxStockAlert.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxStockAlert.Location = New System.Drawing.Point(1429, 45)
        Me.TextBoxStockAlert.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.TextBoxStockAlert.Name = "TextBoxStockAlert"
        Me.TextBoxStockAlert.Size = New System.Drawing.Size(186, 30)
        Me.TextBoxStockAlert.TabIndex = 15
        '
        'LabelStockAlert
        '
        Me.LabelStockAlert.AutoSize = True
        Me.LabelStockAlert.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelStockAlert.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.LabelStockAlert.Location = New System.Drawing.Point(1305, 45)
        Me.LabelStockAlert.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LabelStockAlert.Name = "LabelStockAlert"
        Me.LabelStockAlert.Size = New System.Drawing.Size(107, 25)
        Me.LabelStockAlert.TabIndex = 16
        Me.LabelStockAlert.Text = "Stock Alert"
        '
        'TextBoxStockQyt
        '
        Me.TextBoxStockQyt.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TextBoxStockQyt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxStockQyt.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxStockQyt.Location = New System.Drawing.Point(1068, 45)
        Me.TextBoxStockQyt.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.TextBoxStockQyt.Name = "TextBoxStockQyt"
        Me.TextBoxStockQyt.Size = New System.Drawing.Size(229, 30)
        Me.TextBoxStockQyt.TabIndex = 14
        '
        'LabelStockQyt
        '
        Me.LabelStockQyt.AutoSize = True
        Me.LabelStockQyt.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelStockQyt.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.LabelStockQyt.Location = New System.Drawing.Point(913, 45)
        Me.LabelStockQyt.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LabelStockQyt.Name = "LabelStockQyt"
        Me.LabelStockQyt.Size = New System.Drawing.Size(136, 25)
        Me.LabelStockQyt.TabIndex = 14
        Me.LabelStockQyt.Text = "stock Quantity"
        '
        'LabelItemCost
        '
        Me.LabelItemCost.AutoSize = True
        Me.LabelItemCost.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelItemCost.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.LabelItemCost.Location = New System.Drawing.Point(1305, 76)
        Me.LabelItemCost.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LabelItemCost.Name = "LabelItemCost"
        Me.LabelItemCost.Size = New System.Drawing.Size(95, 25)
        Me.LabelItemCost.TabIndex = 18
        Me.LabelItemCost.Text = "Item Cost"
        '
        'TextBoxItemCost
        '
        Me.TextBoxItemCost.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TextBoxItemCost.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxItemCost.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxItemCost.Location = New System.Drawing.Point(1429, 76)
        Me.TextBoxItemCost.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.TextBoxItemCost.Name = "TextBoxItemCost"
        Me.TextBoxItemCost.Size = New System.Drawing.Size(186, 30)
        Me.TextBoxItemCost.TabIndex = 17
        '
        'TextBoxDes
        '
        Me.TextBoxDes.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TextBoxDes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxDes.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxDes.Location = New System.Drawing.Point(139, 76)
        Me.TextBoxDes.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.TextBoxDes.Name = "TextBoxDes"
        Me.TextBoxDes.Size = New System.Drawing.Size(1158, 30)
        Me.TextBoxDes.TabIndex = 16
        '
        'LabelDes
        '
        Me.LabelDes.AutoSize = True
        Me.LabelDes.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelDes.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.LabelDes.Location = New System.Drawing.Point(19, 76)
        Me.LabelDes.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LabelDes.Name = "LabelDes"
        Me.LabelDes.Size = New System.Drawing.Size(109, 25)
        Me.LabelDes.TabIndex = 6
        Me.LabelDes.Text = "Description"
        '
        'TextBoxItemName
        '
        Me.TextBoxItemName.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TextBoxItemName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxItemName.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxItemName.Location = New System.Drawing.Point(431, 45)
        Me.TextBoxItemName.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.TextBoxItemName.Name = "TextBoxItemName"
        Me.TextBoxItemName.Size = New System.Drawing.Size(474, 30)
        Me.TextBoxItemName.TabIndex = 13
        '
        'TextBoxProMargin
        '
        Me.TextBoxProMargin.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TextBoxProMargin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxProMargin.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxProMargin.Location = New System.Drawing.Point(279, 14)
        Me.TextBoxProMargin.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.TextBoxProMargin.Name = "TextBoxProMargin"
        Me.TextBoxProMargin.Size = New System.Drawing.Size(217, 30)
        Me.TextBoxProMargin.TabIndex = 19
        '
        'LabelSellPrice
        '
        Me.LabelSellPrice.AutoSize = True
        Me.LabelSellPrice.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelSellPrice.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.LabelSellPrice.Location = New System.Drawing.Point(19, 107)
        Me.LabelSellPrice.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LabelSellPrice.Name = "LabelSellPrice"
        Me.LabelSellPrice.Size = New System.Drawing.Size(94, 25)
        Me.LabelSellPrice.TabIndex = 22
        Me.LabelSellPrice.Text = "Sell Price"
        '
        'LabelName
        '
        Me.LabelName.AutoSize = True
        Me.LabelName.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelName.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.LabelName.Location = New System.Drawing.Point(307, 45)
        Me.LabelName.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LabelName.Name = "LabelName"
        Me.LabelName.Size = New System.Drawing.Size(106, 25)
        Me.LabelName.TabIndex = 4
        Me.LabelName.Text = "Item Name"
        '
        'LabelProMargine
        '
        Me.LabelProMargine.AutoSize = True
        Me.LabelProMargine.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelProMargine.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.LabelProMargine.Location = New System.Drawing.Point(133, 18)
        Me.LabelProMargine.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LabelProMargine.Name = "LabelProMargine"
        Me.LabelProMargine.Size = New System.Drawing.Size(132, 25)
        Me.LabelProMargine.TabIndex = 24
        Me.LabelProMargine.Text = "Profit Margine"
        '
        'TextBoxItemId
        '
        Me.TextBoxItemId.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TextBoxItemId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxItemId.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxItemId.Location = New System.Drawing.Point(139, 45)
        Me.TextBoxItemId.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.TextBoxItemId.Name = "TextBoxItemId"
        Me.TextBoxItemId.Size = New System.Drawing.Size(159, 30)
        Me.TextBoxItemId.TabIndex = 12
        '
        'TextBoxSellPrice
        '
        Me.TextBoxSellPrice.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TextBoxSellPrice.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxSellPrice.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxSellPrice.Location = New System.Drawing.Point(139, 107)
        Me.TextBoxSellPrice.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.TextBoxSellPrice.Name = "TextBoxSellPrice"
        Me.TextBoxSellPrice.Size = New System.Drawing.Size(202, 30)
        Me.TextBoxSellPrice.TabIndex = 18
        '
        'labelItemId
        '
        Me.labelItemId.AutoSize = True
        Me.labelItemId.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.labelItemId.ForeColor = System.Drawing.Color.White
        Me.labelItemId.Location = New System.Drawing.Point(19, 45)
        Me.labelItemId.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.labelItemId.Name = "labelItemId"
        Me.labelItemId.Size = New System.Drawing.Size(73, 25)
        Me.labelItemId.TabIndex = 2
        Me.labelItemId.Text = "Item ID"
        '
        'Item_manage
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(1685, 738)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.Filters)
        Me.Controls.Add(Me.DataGridView1)
        Me.KeyPreview = True
        Me.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.Name = "Item_manage"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Item_manage"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Filters.ResumeLayout(False)
        Me.Filters.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents Filters As GroupBox
    Friend WithEvents ComboBoxFCategory As ComboBox
    Friend WithEvents ComboBoxFBrand As ComboBox
    Friend WithEvents LabelFCategory As Label
    Friend WithEvents LabelFBrand As Label
    Friend WithEvents LabelFItemName As Label
    Friend WithEvents TextBoxFItemName As TextBox
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents ComboBoxSupplier As ComboBox
    Friend WithEvents LabelSupplier As Label
    Friend WithEvents CheckBoxWholesale As CheckBox
    Friend WithEvents CheckBoxRetail As CheckBox
    Friend WithEvents TextBoxWholesalePrice As TextBox
    Friend WithEvents TextBoxRetailPrice As TextBox
    Friend WithEvents LabelWholesalePrice As Label
    Friend WithEvents LabelRetailPrice As Label
    Friend WithEvents TextBoxFDescription As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents LiveTimer As Timer
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents btnBarcode As System.Windows.Forms.Button
    Friend WithEvents btnAddToList As System.Windows.Forms.Button
    Friend WithEvents btnDelete As Button
    Friend WithEvents btnSuccess As Button
    Friend WithEvents btnLogs As Button
    Friend WithEvents btnUpdate As Button
    Friend WithEvents ComboBoxBrand As ComboBox
    Friend WithEvents ComboBoxCategory As ComboBox
    Friend WithEvents TextBoxDis As TextBox
    Friend WithEvents LabelDis As Label
    Friend WithEvents cmbAuthUser As ComboBox
    Friend WithEvents LabelAuthUser As Label
    Friend WithEvents txtReadOnly As TextBox
    Friend WithEvents LabelBrand As Label
    Friend WithEvents TextBoxAvgCost As TextBox
    Friend WithEvents LabelAvgCost As Label
    Friend WithEvents LabelCategory As Label
    Friend WithEvents btnAddNew As Button
    Friend WithEvents ComboBoxSuMethod As ComboBox
    Friend WithEvents ComboBoxMeasure As ComboBox
    Friend WithEvents CheckBoxIsActive As CheckBox
    Friend WithEvents LabelMeasure As Label
    Friend WithEvents LabelSuppMethod As Label
    Friend WithEvents TextBoxStockAlert As TextBox
    Friend WithEvents LabelStockAlert As Label
    Friend WithEvents TextBoxStockQyt As TextBox
    Friend WithEvents LabelStockQyt As Label
    Friend WithEvents LabelItemCost As Label
    Friend WithEvents TextBoxItemCost As TextBox
    Friend WithEvents TextBoxDes As TextBox
    Friend WithEvents LabelDes As Label
    Friend WithEvents TextBoxItemName As TextBox
    Friend WithEvents TextBoxProMargin As TextBox
    Friend WithEvents LabelSellPrice As Label
    Friend WithEvents LabelName As Label
    Friend WithEvents LabelProMargine As Label
    Friend WithEvents TextBoxItemId As TextBox
    Friend WithEvents TextBoxSellPrice As TextBox
    Friend WithEvents labelItemId As Label
    Friend WithEvents TextBoxWPrice As TextBox
    Friend WithEvents LabelWPrice As Label
    Friend WithEvents TextBoxRPrice As TextBox
    Friend WithEvents LabelRPrice As Label
    Friend WithEvents cmbFilterType As ComboBox
End Class