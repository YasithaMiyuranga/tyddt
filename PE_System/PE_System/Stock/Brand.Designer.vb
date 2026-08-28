<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Brand
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
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.TextBoxBrand = New System.Windows.Forms.TextBox()
        Me.LabelBrand = New System.Windows.Forms.Label()
        Me.ButAdd = New System.Windows.Forms.Button()
        Me.ButUpdate = New System.Windows.Forms.Button()
        Me.ButDelete = New System.Windows.Forms.Button()
        Me.ButSuccess = New System.Windows.Forms.Button()
        Me.DataGridView2 = New System.Windows.Forms.DataGridView()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.DataGridView3 = New System.Windows.Forms.DataGridView()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.BLAdd = New System.Windows.Forms.Button()
        Me.BLUpdate = New System.Windows.Forms.Button()
        Me.BLDelete = New System.Windows.Forms.Button()
        Me.BLSuccess = New System.Windows.Forms.Button()
        Me.TextBoxMeasure = New System.Windows.Forms.TextBox()
        Me.LabelMeasure = New System.Windows.Forms.Label()
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.BSAdd = New System.Windows.Forms.Button()
        Me.BSUpdate = New System.Windows.Forms.Button()
        Me.BSDelete = New System.Windows.Forms.Button()
        Me.BSSuccess = New System.Windows.Forms.Button()
        Me.TextBoxSuMethod = New System.Windows.Forms.TextBox()
        Me.LabelSuMethod = New System.Windows.Forms.Label()
        Me.DataGridView4 = New System.Windows.Forms.DataGridView()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        CType(Me.DataGridView2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPage2.SuspendLayout()
        CType(Me.DataGridView3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox2.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        CType(Me.DataGridView4, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Controls.Add(Me.TabPage3)
        Me.TabControl1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TabControl1.Location = New System.Drawing.Point(4, 2)
        Me.TabControl1.Margin = New System.Windows.Forms.Padding(4)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(475, 595)
        Me.TabControl1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.TabControl1.TabIndex = 7
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.GroupBox1)
        Me.TabPage1.Controls.Add(Me.DataGridView2)
        Me.TabPage1.Location = New System.Drawing.Point(4, 29)
        Me.TabPage1.Margin = New System.Windows.Forms.Padding(4)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(4)
        Me.TabPage1.Size = New System.Drawing.Size(467, 627)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Brand"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.BackColor = System.Drawing.Color.FromArgb(CType(CType(44, Byte), Integer), CType(CType(62, Byte), Integer), CType(CType(80, Byte), Integer))
        Me.GroupBox1.Controls.Add(Me.TextBoxBrand)
        Me.GroupBox1.Controls.Add(Me.LabelBrand)
        Me.GroupBox1.Controls.Add(Me.ButAdd)
        Me.GroupBox1.Controls.Add(Me.ButUpdate)
        Me.GroupBox1.Controls.Add(Me.ButDelete)
        Me.GroupBox1.Controls.Add(Me.ButSuccess)
        Me.GroupBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox1.ForeColor = System.Drawing.Color.Yellow
        Me.GroupBox1.Location = New System.Drawing.Point(8, 497)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(4)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(4)
        Me.GroupBox1.Size = New System.Drawing.Size(448, 127)
        Me.GroupBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left)), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.TabIndex = 4
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Add Brand"
        '
        'TextBoxBrand
        '
        Me.TextBoxBrand.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TextBoxBrand.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxBrand.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxBrand.Location = New System.Drawing.Point(85, 27)
        Me.TextBoxBrand.Margin = New System.Windows.Forms.Padding(4)
        Me.TextBoxBrand.Name = "TextBoxBrand"
        Me.TextBoxBrand.Size = New System.Drawing.Size(350, 30)
        Me.TextBoxBrand.TabIndex = 41
        '
        'LabelBrand
        '
        Me.LabelBrand.AutoSize = True
        Me.LabelBrand.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelBrand.ForeColor = System.Drawing.Color.Yellow
        Me.LabelBrand.Location = New System.Drawing.Point(8, 27)
        Me.LabelBrand.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LabelBrand.Name = "LabelBrand"
        Me.LabelBrand.Size = New System.Drawing.Size(64, 25)
        Me.LabelBrand.TabIndex = 40
        Me.LabelBrand.Text = "Brand"
        '
        'ButAdd
        '
        Me.ButAdd.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.ButAdd.Cursor = System.Windows.Forms.Cursors.Hand
        Me.ButAdd.FlatAppearance.BorderColor = System.Drawing.Color.Silver
        Me.ButAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ButAdd.Font = New System.Drawing.Font("Segoe UI", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButAdd.ForeColor = System.Drawing.Color.Black
        Me.ButAdd.Location = New System.Drawing.Point(125, 69)
        Me.ButAdd.Margin = New System.Windows.Forms.Padding(4)
        Me.ButAdd.Name = "ButAdd"
        Me.ButAdd.Size = New System.Drawing.Size(116, 43)
        Me.ButAdd.TabIndex = 39
        Me.ButAdd.Text = "Add New"
        Me.ButAdd.UseVisualStyleBackColor = False
        '
        'ButUpdate
        '
        Me.ButUpdate.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.ButUpdate.Cursor = System.Windows.Forms.Cursors.Hand
        Me.ButUpdate.FlatAppearance.BorderSize = 0
        Me.ButUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ButUpdate.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButUpdate.ForeColor = System.Drawing.Color.Black
        Me.ButUpdate.Location = New System.Drawing.Point(249, 69)
        Me.ButUpdate.Margin = New System.Windows.Forms.Padding(4)
        Me.ButUpdate.Name = "ButUpdate"
        Me.ButUpdate.Size = New System.Drawing.Size(89, 43)
        Me.ButUpdate.TabIndex = 38
        Me.ButUpdate.Text = "Update"
        Me.ButUpdate.UseVisualStyleBackColor = False
        '
        'ButDelete
        '
        Me.ButDelete.BackColor = System.Drawing.Color.Crimson
        Me.ButDelete.Cursor = System.Windows.Forms.Cursors.Hand
        Me.ButDelete.FlatAppearance.BorderSize = 0
        Me.ButDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ButDelete.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButDelete.ForeColor = System.Drawing.Color.White
        Me.ButDelete.Location = New System.Drawing.Point(346, 69)
        Me.ButDelete.Margin = New System.Windows.Forms.Padding(4)
        Me.ButDelete.Name = "ButDelete"
        Me.ButDelete.Size = New System.Drawing.Size(89, 43)
        Me.ButDelete.TabIndex = 37
        Me.ButDelete.Text = "Delete"
        Me.ButDelete.UseVisualStyleBackColor = False
        '
        'ButSuccess
        '
        Me.ButSuccess.BackColor = System.Drawing.Color.DeepSkyBlue
        Me.ButSuccess.Cursor = System.Windows.Forms.Cursors.Hand
        Me.ButSuccess.FlatAppearance.BorderSize = 0
        Me.ButSuccess.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ButSuccess.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButSuccess.ForeColor = System.Drawing.Color.Black
        Me.ButSuccess.Location = New System.Drawing.Point(28, 69)
        Me.ButSuccess.Margin = New System.Windows.Forms.Padding(4)
        Me.ButSuccess.Name = "ButSuccess"
        Me.ButSuccess.Size = New System.Drawing.Size(89, 43)
        Me.ButSuccess.TabIndex = 36
        Me.ButSuccess.Text = "Save"
        Me.ButSuccess.UseVisualStyleBackColor = False
        '
        'DataGridView2
        '
        Me.DataGridView2.BackgroundColor = System.Drawing.SystemColors.ButtonFace
        Me.DataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView2.Location = New System.Drawing.Point(8, 7)
        Me.DataGridView2.Margin = New System.Windows.Forms.Padding(4)
        Me.DataGridView2.Name = "DataGridView2"
        Me.DataGridView2.RowHeadersVisible = False
        Me.DataGridView2.RowHeadersWidth = 51
        Me.DataGridView2.Size = New System.Drawing.Size(448, 350)
        Me.DataGridView2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.DataGridView2.TabIndex = 1
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.DataGridView3)
        Me.TabPage2.Controls.Add(Me.GroupBox2)
        Me.TabPage2.Location = New System.Drawing.Point(4, 29)
        Me.TabPage2.Margin = New System.Windows.Forms.Padding(4)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(4)
        Me.TabPage2.Size = New System.Drawing.Size(467, 627)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Measure"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'DataGridView3
        '
        Me.DataGridView3.BackgroundColor = System.Drawing.SystemColors.ButtonFace
        Me.DataGridView3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView3.Location = New System.Drawing.Point(8, 6)
        Me.DataGridView3.Margin = New System.Windows.Forms.Padding(4)
        Me.DataGridView3.Name = "DataGridView3"
        Me.DataGridView3.RowHeadersVisible = False
        Me.DataGridView3.RowHeadersWidth = 51
        Me.DataGridView3.Size = New System.Drawing.Size(448, 485)
        Me.DataGridView3.TabIndex = 9
        '
        'GroupBox2
        '
        Me.GroupBox2.BackColor = System.Drawing.Color.FromArgb(CType(CType(44, Byte), Integer), CType(CType(62, Byte), Integer), CType(CType(80, Byte), Integer))
        Me.GroupBox2.Controls.Add(Me.BLAdd)
        Me.GroupBox2.Controls.Add(Me.BLUpdate)
        Me.GroupBox2.Controls.Add(Me.BLDelete)
        Me.GroupBox2.Controls.Add(Me.BLSuccess)
        Me.GroupBox2.Controls.Add(Me.TextBoxMeasure)
        Me.GroupBox2.Controls.Add(Me.LabelMeasure)
        Me.GroupBox2.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox2.ForeColor = System.Drawing.Color.Yellow
        Me.GroupBox2.Location = New System.Drawing.Point(8, 500)
        Me.GroupBox2.Margin = New System.Windows.Forms.Padding(4)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Padding = New System.Windows.Forms.Padding(4)
        Me.GroupBox2.Size = New System.Drawing.Size(448, 127)
        Me.GroupBox2.TabIndex = 8
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Add Measure"
        '
        'BLAdd
        '
        Me.BLAdd.BackColor = System.Drawing.Color.Yellow
        Me.BLAdd.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BLAdd.FlatAppearance.BorderColor = System.Drawing.Color.Silver
        Me.BLAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BLAdd.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BLAdd.ForeColor = System.Drawing.Color.Black
        Me.BLAdd.Location = New System.Drawing.Point(32, 70)
        Me.BLAdd.Margin = New System.Windows.Forms.Padding(4)
        Me.BLAdd.Name = "BLAdd"
        Me.BLAdd.Size = New System.Drawing.Size(94, 43)
        Me.BLAdd.TabIndex = 45
        Me.BLAdd.Text = "Add New"
        Me.BLAdd.UseVisualStyleBackColor = True
        '
        'BLUpdate
        '
        Me.BLUpdate.BackColor = System.Drawing.Color.DodgerBlue
        Me.BLUpdate.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BLUpdate.FlatAppearance.BorderSize = 0
        Me.BLUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BLUpdate.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BLUpdate.ForeColor = System.Drawing.Color.White
        Me.BLUpdate.Location = New System.Drawing.Point(231, 70)
        Me.BLUpdate.Margin = New System.Windows.Forms.Padding(4)
        Me.BLUpdate.Name = "BLUpdate"
        Me.BLUpdate.Size = New System.Drawing.Size(89, 43)
        Me.BLUpdate.TabIndex = 44
        Me.BLUpdate.Text = "Update"
        Me.BLUpdate.UseVisualStyleBackColor = False
        '
        'BLDelete
        '
        Me.BLDelete.BackColor = System.Drawing.Color.Crimson
        Me.BLDelete.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BLDelete.FlatAppearance.BorderSize = 0
        Me.BLDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BLDelete.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BLDelete.ForeColor = System.Drawing.Color.White
        Me.BLDelete.Location = New System.Drawing.Point(328, 70)
        Me.BLDelete.Margin = New System.Windows.Forms.Padding(4)
        Me.BLDelete.Name = "BLDelete"
        Me.BLDelete.Size = New System.Drawing.Size(89, 43)
        Me.BLDelete.TabIndex = 43
        Me.BLDelete.Text = "Delete"
        Me.BLDelete.UseVisualStyleBackColor = False
        '
        'BLSuccess
        '
        Me.BLSuccess.BackColor = System.Drawing.Color.SeaGreen
        Me.BLSuccess.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BLSuccess.FlatAppearance.BorderSize = 0
        Me.BLSuccess.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BLSuccess.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BLSuccess.ForeColor = System.Drawing.Color.White
        Me.BLSuccess.Location = New System.Drawing.Point(134, 70)
        Me.BLSuccess.Margin = New System.Windows.Forms.Padding(4)
        Me.BLSuccess.Name = "BLSuccess"
        Me.BLSuccess.Size = New System.Drawing.Size(89, 43)
        Me.BLSuccess.TabIndex = 42
        Me.BLSuccess.Text = "Success"
        Me.BLSuccess.UseVisualStyleBackColor = False
        '
        'TextBoxMeasure
        '
        Me.TextBoxMeasure.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TextBoxMeasure.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxMeasure.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxMeasure.Location = New System.Drawing.Point(113, 27)
        Me.TextBoxMeasure.Margin = New System.Windows.Forms.Padding(4)
        Me.TextBoxMeasure.Name = "TextBoxMeasure"
        Me.TextBoxMeasure.Size = New System.Drawing.Size(322, 30)
        Me.TextBoxMeasure.TabIndex = 41
        '
        'LabelMeasure
        '
        Me.LabelMeasure.AutoSize = True
        Me.LabelMeasure.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelMeasure.ForeColor = System.Drawing.Color.Yellow
        Me.LabelMeasure.Location = New System.Drawing.Point(8, 27)
        Me.LabelMeasure.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LabelMeasure.Name = "LabelMeasure"
        Me.LabelMeasure.Size = New System.Drawing.Size(89, 25)
        Me.LabelMeasure.TabIndex = 40
        Me.LabelMeasure.Text = "Measure"
        '
        'TabPage3
        '
        Me.TabPage3.Controls.Add(Me.GroupBox3)
        Me.TabPage3.Controls.Add(Me.DataGridView4)
        Me.TabPage3.Location = New System.Drawing.Point(4, 29)
        Me.TabPage3.Margin = New System.Windows.Forms.Padding(4)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Padding = New System.Windows.Forms.Padding(4)
        Me.TabPage3.Size = New System.Drawing.Size(467, 627)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "Supplier Method"
        Me.TabPage3.UseVisualStyleBackColor = True
        '
        'GroupBox3
        '
        Me.GroupBox3.BackColor = System.Drawing.Color.FromArgb(CType(CType(44, Byte), Integer), CType(CType(62, Byte), Integer), CType(CType(80, Byte), Integer))
        Me.GroupBox3.Controls.Add(Me.BSAdd)
        Me.GroupBox3.Controls.Add(Me.BSUpdate)
        Me.GroupBox3.Controls.Add(Me.BSDelete)
        Me.GroupBox3.Controls.Add(Me.BSSuccess)
        Me.GroupBox3.Controls.Add(Me.TextBoxSuMethod)
        Me.GroupBox3.Controls.Add(Me.LabelSuMethod)
        Me.GroupBox3.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox3.ForeColor = System.Drawing.Color.Yellow
        Me.GroupBox3.Location = New System.Drawing.Point(8, 500)
        Me.GroupBox3.Margin = New System.Windows.Forms.Padding(4)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Padding = New System.Windows.Forms.Padding(4)
        Me.GroupBox3.Size = New System.Drawing.Size(448, 127)
        Me.GroupBox3.TabIndex = 9
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Add Supplier Method"
        '
        'BSAdd
        '
        Me.BSAdd.BackColor = System.Drawing.Color.Yellow
        Me.BSAdd.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BSAdd.FlatAppearance.BorderColor = System.Drawing.Color.Silver
        Me.BSAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BSAdd.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BSAdd.ForeColor = System.Drawing.Color.Black
        Me.BSAdd.Location = New System.Drawing.Point(32, 69)
        Me.BSAdd.Margin = New System.Windows.Forms.Padding(4)
        Me.BSAdd.Name = "BSAdd"
        Me.BSAdd.Size = New System.Drawing.Size(94, 43)
        Me.BSAdd.TabIndex = 45
        Me.BSAdd.Text = "Add New"
        Me.BSAdd.UseVisualStyleBackColor = True
        '
        'BSUpdate
        '
        Me.BSUpdate.BackColor = System.Drawing.Color.DodgerBlue
        Me.BSUpdate.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BSUpdate.FlatAppearance.BorderSize = 0
        Me.BSUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BSUpdate.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BSUpdate.ForeColor = System.Drawing.Color.White
        Me.BSUpdate.Location = New System.Drawing.Point(231, 69)
        Me.BSUpdate.Margin = New System.Windows.Forms.Padding(4)
        Me.BSUpdate.Name = "BSUpdate"
        Me.BSUpdate.Size = New System.Drawing.Size(89, 43)
        Me.BSUpdate.TabIndex = 44
        Me.BSUpdate.Text = "Update"
        Me.BSUpdate.UseVisualStyleBackColor = False
        '
        'BSDelete
        '
        Me.BSDelete.BackColor = System.Drawing.Color.Crimson
        Me.BSDelete.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BSDelete.FlatAppearance.BorderSize = 0
        Me.BSDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BSDelete.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BSDelete.ForeColor = System.Drawing.Color.White
        Me.BSDelete.Location = New System.Drawing.Point(328, 69)
        Me.BSDelete.Margin = New System.Windows.Forms.Padding(4)
        Me.BSDelete.Name = "BSDelete"
        Me.BSDelete.Size = New System.Drawing.Size(89, 43)
        Me.BSDelete.TabIndex = 43
        Me.BSDelete.Text = "Delete"
        Me.BSDelete.UseVisualStyleBackColor = False
        '
        'BSSuccess
        '
        Me.BSSuccess.BackColor = System.Drawing.Color.SeaGreen
        Me.BSSuccess.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BSSuccess.FlatAppearance.BorderSize = 0
        Me.BSSuccess.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BSSuccess.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BSSuccess.ForeColor = System.Drawing.Color.White
        Me.BSSuccess.Location = New System.Drawing.Point(134, 69)
        Me.BSSuccess.Margin = New System.Windows.Forms.Padding(4)
        Me.BSSuccess.Name = "BSSuccess"
        Me.BSSuccess.Size = New System.Drawing.Size(89, 43)
        Me.BSSuccess.TabIndex = 42
        Me.BSSuccess.Text = "Success"
        Me.BSSuccess.UseVisualStyleBackColor = False
        '
        'TextBoxSuMethod
        '
        Me.TextBoxSuMethod.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TextBoxSuMethod.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxSuMethod.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxSuMethod.Location = New System.Drawing.Point(113, 27)
        Me.TextBoxSuMethod.Margin = New System.Windows.Forms.Padding(4)
        Me.TextBoxSuMethod.Name = "TextBoxSuMethod"
        Me.TextBoxSuMethod.Size = New System.Drawing.Size(322, 30)
        Me.TextBoxSuMethod.TabIndex = 41
        '
        'LabelSuMethod
        '
        Me.LabelSuMethod.AutoSize = True
        Me.LabelSuMethod.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelSuMethod.ForeColor = System.Drawing.Color.Yellow
        Me.LabelSuMethod.Location = New System.Drawing.Point(8, 27)
        Me.LabelSuMethod.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LabelSuMethod.Name = "LabelSuMethod"
        Me.LabelSuMethod.Size = New System.Drawing.Size(97, 25)
        Me.LabelSuMethod.TabIndex = 40
        Me.LabelSuMethod.Text = "S.Method"
        '
        'DataGridView4
        '
        Me.DataGridView4.BackgroundColor = System.Drawing.SystemColors.ButtonFace
        Me.DataGridView4.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView4.Location = New System.Drawing.Point(8, 6)
        Me.DataGridView4.Margin = New System.Windows.Forms.Padding(4)
        Me.DataGridView4.Name = "DataGridView4"
        Me.DataGridView4.RowHeadersVisible = False
        Me.DataGridView4.RowHeadersWidth = 51
        Me.DataGridView4.Size = New System.Drawing.Size(448, 485)
        Me.DataGridView4.TabIndex = 8
        '
        'Brand
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(480, 600)
        Me.AutoScroll = False
        Me.Controls.Add(Me.TabControl1)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "Brand"
        Me.Text = "Brand"
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        CType(Me.DataGridView2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPage2.ResumeLayout(False)
        CType(Me.DataGridView3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.TabPage3.ResumeLayout(False)
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        CType(Me.DataGridView4, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents TabPage1 As TabPage
    Friend WithEvents TabPage2 As TabPage
    Friend WithEvents DataGridView3 As DataGridView
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents TextBoxMeasure As TextBox
    Friend WithEvents LabelMeasure As Label
    Friend WithEvents TabPage3 As TabPage
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents TextBoxSuMethod As TextBox
    Friend WithEvents LabelSuMethod As Label
    Friend WithEvents DataGridView4 As DataGridView
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents TextBoxBrand As TextBox
    Friend WithEvents LabelBrand As Label
    Friend WithEvents ButAdd As Button
    Friend WithEvents ButUpdate As Button
    Friend WithEvents ButDelete As Button
    Friend WithEvents ButSuccess As Button
    Friend WithEvents DataGridView2 As DataGridView
    Friend WithEvents BLAdd As Button
    Friend WithEvents BLUpdate As Button
    Friend WithEvents BLDelete As Button
    Friend WithEvents BLSuccess As Button
    Friend WithEvents BSAdd As Button
    Friend WithEvents BSUpdate As Button
    Friend WithEvents BSDelete As Button
    Friend WithEvents BSSuccess As Button
End Class
