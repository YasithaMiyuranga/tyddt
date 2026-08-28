<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class BarcodePrint
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
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.DataGridViewBarcode = New System.Windows.Forms.DataGridView()
        Me.ItemID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ItemName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Description = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PrintQty = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Price = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ItemCost = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Qty = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.LabelTitle = New System.Windows.Forms.Label()
        Me.PanelHeader = New System.Windows.Forms.Panel()
        Me.btnClear = New System.Windows.Forms.Button()
        Me.btnRemove = New System.Windows.Forms.Button()
        Me.PrintDocument1 = New System.Drawing.Printing.PrintDocument()
        Me.PrintDialog1 = New System.Windows.Forms.PrintDialog()
        Me.lblInvoiceNo = New System.Windows.Forms.Label()
        Me.txtInvoiceNo = New System.Windows.Forms.TextBox()
        Me.cmbAuthUser = New System.Windows.Forms.ComboBox()
        Me.txtReadOnly = New System.Windows.Forms.TextBox()
        Me.LabelPriceAuth = New System.Windows.Forms.Label()
        CType(Me.DataGridViewBarcode, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelHeader.SuspendLayout()
        Me.SuspendLayout()
        '
        'DataGridViewBarcode
        '
        Me.DataGridViewBarcode.AllowUserToAddRows = False
        Me.DataGridViewBarcode.AllowUserToDeleteRows = False
        Me.DataGridViewBarcode.BackgroundColor = System.Drawing.Color.FromArgb(CType(CType(44, Byte), Integer), CType(CType(62, Byte), Integer), CType(CType(80, Byte), Integer))
        Me.DataGridViewBarcode.BorderStyle = System.Windows.Forms.BorderStyle.None
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.Color.DarkSlateGray
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.Color.Yellow
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DataGridViewBarcode.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle3
        Me.DataGridViewBarcode.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridViewBarcode.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ItemID, Me.ItemName, Me.Description, Me.PrintQty, Me.Price, Me.ItemCost})
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!)
        DataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.RoyalBlue
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.DataGridViewBarcode.DefaultCellStyle = DataGridViewCellStyle4
        Me.DataGridViewBarcode.EnableHeadersVisualStyles = False
        Me.DataGridViewBarcode.Location = New System.Drawing.Point(12, 100)
        Me.DataGridViewBarcode.Name = "DataGridViewBarcode"
        Me.DataGridViewBarcode.RowHeadersWidth = 51
        Me.DataGridViewBarcode.RowTemplate.Height = 35
        Me.DataGridViewBarcode.Size = New System.Drawing.Size(960, 360)
        Me.DataGridViewBarcode.TabIndex = 6
        '
        'ItemID
        '
        Me.ItemID.HeaderText = "Item ID"
        Me.ItemID.MinimumWidth = 6
        Me.ItemID.Name = "ItemID"
        Me.ItemID.ReadOnly = True
        Me.ItemID.Width = 120
        '
        'ItemName
        '
        Me.ItemName.HeaderText = "Item Name"
        Me.ItemName.MinimumWidth = 6
        Me.ItemName.Name = "ItemName"
        Me.ItemName.Visible = False
        Me.ItemName.Width = 125
        '
        'Description
        '
        Me.Description.HeaderText = "Description"
        Me.Description.MinimumWidth = 6
        Me.Description.Name = "Description"
        Me.Description.ReadOnly = True
        Me.Description.Width = 350
        '
        'PrintQty
        '
        Me.PrintQty.HeaderText = "Print Qty"
        Me.PrintQty.MinimumWidth = 6
        Me.PrintQty.Name = "PrintQty"
        Me.PrintQty.Width = 125
        '
        'Price
        '
        Me.Price.HeaderText = "Price"
        Me.Price.MinimumWidth = 6
        Me.Price.Name = "Price"
        Me.Price.ReadOnly = True
        Me.Price.Width = 120
        '
        'ItemCost
        '
        Me.ItemCost.HeaderText = "Item Cost"
        Me.ItemCost.MinimumWidth = 6
        Me.ItemCost.Name = "ItemCost"
        Me.ItemCost.Visible = False
        Me.ItemCost.Width = 125
        '
        'Qty
        '
        Me.Qty.MinimumWidth = 6
        Me.Qty.Name = "Qty"
        Me.Qty.Width = 125
        '
        'btnPrint
        '
        Me.btnPrint.BackColor = System.Drawing.Color.DeepSkyBlue
        Me.btnPrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnPrint.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnPrint.ForeColor = System.Drawing.Color.Black
        Me.btnPrint.Location = New System.Drawing.Point(792, 470)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(180, 50)
        Me.btnPrint.TabIndex = 1
        Me.btnPrint.Text = "Print Barcode"
        Me.btnPrint.UseVisualStyleBackColor = False
        '
        'LabelTitle
        '
        Me.LabelTitle.AutoSize = True
        Me.LabelTitle.Font = New System.Drawing.Font("Microsoft Sans Serif", 16.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelTitle.ForeColor = System.Drawing.Color.Yellow
        Me.LabelTitle.Location = New System.Drawing.Point(14, 9)
        Me.LabelTitle.Name = "LabelTitle"
        Me.LabelTitle.Size = New System.Drawing.Size(30, 31)
        Me.LabelTitle.TabIndex = 2
        Me.LabelTitle.Text = "  "
        '
        'PanelHeader
        '
        Me.PanelHeader.BackColor = System.Drawing.Color.FromArgb(CType(CType(44, Byte), Integer), CType(CType(62, Byte), Integer), CType(CType(80, Byte), Integer))
        Me.PanelHeader.Controls.Add(Me.LabelTitle)
        Me.PanelHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelHeader.Location = New System.Drawing.Point(0, 0)
        Me.PanelHeader.Name = "PanelHeader"
        Me.PanelHeader.Size = New System.Drawing.Size(980, 50)
        Me.PanelHeader.TabIndex = 3
        '
        'btnClear
        '
        Me.btnClear.BackColor = System.Drawing.Color.Crimson
        Me.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnClear.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnClear.ForeColor = System.Drawing.Color.White
        Me.btnClear.Location = New System.Drawing.Point(12, 470)
        Me.btnClear.Name = "btnClear"
        Me.btnClear.Size = New System.Drawing.Size(120, 50)
        Me.btnClear.TabIndex = 4
        Me.btnClear.Text = "Clear List"
        Me.btnClear.UseVisualStyleBackColor = False
        '
        'btnRemove
        '
        Me.btnRemove.BackColor = System.Drawing.Color.OrangeRed
        Me.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnRemove.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnRemove.ForeColor = System.Drawing.Color.White
        Me.btnRemove.Location = New System.Drawing.Point(140, 470)
        Me.btnRemove.Name = "btnRemove"
        Me.btnRemove.Size = New System.Drawing.Size(200, 50)
        Me.btnRemove.TabIndex = 5
        Me.btnRemove.Text = "Remove Selected"
        Me.btnRemove.UseVisualStyleBackColor = False
        '
        'PrintDialog1
        '
        Me.PrintDialog1.Document = Me.PrintDocument1
        Me.PrintDialog1.UseEXDialog = True
        '
        'lblInvoiceNo
        '
        Me.lblInvoiceNo.AutoSize = True
        Me.lblInvoiceNo.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblInvoiceNo.Location = New System.Drawing.Point(12, 65)
        Me.lblInvoiceNo.Name = "lblInvoiceNo"
        Me.lblInvoiceNo.Size = New System.Drawing.Size(207, 25)
        Me.lblInvoiceNo.TabIndex = 7
        Me.lblInvoiceNo.Text = "Supplier Invoice No:"
        '
        'txtInvoiceNo
        '
        Me.txtInvoiceNo.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!)
        Me.txtInvoiceNo.Location = New System.Drawing.Point(235, 62)
        Me.txtInvoiceNo.Name = "txtInvoiceNo"
        Me.txtInvoiceNo.Size = New System.Drawing.Size(150, 30)
        Me.txtInvoiceNo.TabIndex = 0
        '
        'cmbAuthUser
        '
        Me.cmbAuthUser.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbAuthUser.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!)
        Me.cmbAuthUser.FormattingEnabled = True
        Me.cmbAuthUser.Location = New System.Drawing.Point(620, 62)
        Me.cmbAuthUser.Name = "cmbAuthUser"
        Me.cmbAuthUser.Size = New System.Drawing.Size(150, 30)
        Me.cmbAuthUser.TabIndex = 8
        '
        'txtReadOnly
        '
        Me.txtReadOnly.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!)
        Me.txtReadOnly.Location = New System.Drawing.Point(776, 62)
        Me.txtReadOnly.Name = "txtReadOnly"
        Me.txtReadOnly.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.txtReadOnly.Size = New System.Drawing.Size(100, 28)
        Me.txtReadOnly.TabIndex = 9
        '
        'LabelPriceAuth
        '
        Me.LabelPriceAuth.AutoSize = True
        Me.LabelPriceAuth.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!, System.Drawing.FontStyle.Bold)
        Me.LabelPriceAuth.Location = New System.Drawing.Point(500, 65)
        Me.LabelPriceAuth.Name = "LabelPriceAuth"
        Me.LabelPriceAuth.Size = New System.Drawing.Size(113, 24)
        Me.LabelPriceAuth.TabIndex = 10
        Me.LabelPriceAuth.Text = "Price Auth:"
        '
        'BarcodePrint
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(980, 532)
        Me.AutoScroll = True
        Me.Controls.Add(Me.btnRemove)
        Me.Controls.Add(Me.btnClear)
        Me.Controls.Add(Me.PanelHeader)
        Me.Controls.Add(Me.btnPrint)
        Me.Controls.Add(Me.DataGridViewBarcode)
        Me.Controls.Add(Me.LabelPriceAuth)
        Me.Controls.Add(Me.txtReadOnly)
        Me.Controls.Add(Me.cmbAuthUser)
        Me.Controls.Add(Me.lblInvoiceNo)
        Me.Controls.Add(Me.txtInvoiceNo)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.Name = "BarcodePrint"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Barcode Print List"
        CType(Me.DataGridViewBarcode, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelHeader.ResumeLayout(False)
        Me.PanelHeader.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents DataGridViewBarcode As System.Windows.Forms.DataGridView
    Friend WithEvents btnPrint As System.Windows.Forms.Button
    Friend WithEvents LabelTitle As System.Windows.Forms.Label
    Friend WithEvents PanelHeader As System.Windows.Forms.Panel
    Friend WithEvents btnClear As System.Windows.Forms.Button
    Friend WithEvents btnRemove As System.Windows.Forms.Button
    Friend WithEvents Qty As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents PrintDocument1 As System.Drawing.Printing.PrintDocument
    Friend WithEvents PrintDialog1 As System.Windows.Forms.PrintDialog
    Friend WithEvents ItemID As DataGridViewTextBoxColumn
    Friend WithEvents Description As DataGridViewTextBoxColumn
    Friend WithEvents PrintQty As DataGridViewTextBoxColumn
    Friend WithEvents Price As DataGridViewTextBoxColumn
    Friend WithEvents ItemName As DataGridViewTextBoxColumn
    Friend WithEvents lblInvoiceNo As System.Windows.Forms.Label
    Friend WithEvents txtInvoiceNo As System.Windows.Forms.TextBox
    Friend WithEvents ItemCost As DataGridViewTextBoxColumn
    Friend WithEvents cmbAuthUser As System.Windows.Forms.ComboBox
    Friend WithEvents txtReadOnly As System.Windows.Forms.TextBox
    Friend WithEvents LabelPriceAuth As System.Windows.Forms.Label
End Class
