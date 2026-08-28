<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class bank_add
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
        Dim Label15 As System.Windows.Forms.Label
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.GroupBox6 = New System.Windows.Forms.GroupBox()
        Me.filter_name = New System.Windows.Forms.TextBox()
        Me.bankCount = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.bank_name = New System.Windows.Forms.TextBox()
        Me.LabelBank = New System.Windows.Forms.Label()
        Me.ButAdd = New System.Windows.Forms.Button()
        Me.ButUpdate = New System.Windows.Forms.Button()
        Me.ButDelete = New System.Windows.Forms.Button()
        Me.BtnSave = New System.Windows.Forms.Button()
        Me.BankDataGridView = New System.Windows.Forms.DataGridView()
        Label15 = New System.Windows.Forms.Label()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox6.SuspendLayout()
        CType(Me.BankDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label15
        '
        Label15.AutoSize = True
        Label15.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Label15.ForeColor = System.Drawing.Color.White
        Label15.Location = New System.Drawing.Point(6, 29)
        Label15.Name = "Label15"
        Label15.Size = New System.Drawing.Size(57, 20)
        Label15.TabIndex = 230
        Label15.Text = "Bank:"
        '
        'GroupBox1
        '
        Me.GroupBox1.BackColor = System.Drawing.Color.FromArgb(CType(CType(44, Byte), Integer), CType(CType(62, Byte), Integer), CType(CType(80, Byte), Integer))
        Me.GroupBox1.Controls.Add(Me.GroupBox6)
        Me.GroupBox1.Controls.Add(Me.bankCount)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.bank_name)
        Me.GroupBox1.Controls.Add(Me.LabelBank)
        Me.GroupBox1.Controls.Add(Me.ButAdd)
        Me.GroupBox1.Controls.Add(Me.ButUpdate)
        Me.GroupBox1.Controls.Add(Me.ButDelete)
        Me.GroupBox1.Controls.Add(Me.BtnSave)
        Me.GroupBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox1.ForeColor = System.Drawing.Color.Yellow
        Me.GroupBox1.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(4)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(4)
        Me.GroupBox1.Size = New System.Drawing.Size(594, 171)
        Me.GroupBox1.TabIndex = 5
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Add Bank"
        '
        'GroupBox6
        '
        Me.GroupBox6.Controls.Add(Label15)
        Me.GroupBox6.Controls.Add(Me.filter_name)
        Me.GroupBox6.ForeColor = System.Drawing.Color.Yellow
        Me.GroupBox6.Location = New System.Drawing.Point(0, 110)
        Me.GroupBox6.Name = "GroupBox6"
        Me.GroupBox6.Size = New System.Drawing.Size(209, 60)
        Me.GroupBox6.TabIndex = 263
        Me.GroupBox6.TabStop = False
        Me.GroupBox6.Text = "Filter Options"
        '
        'filter_name
        '
        Me.filter_name.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.filter_name.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.filter_name.Location = New System.Drawing.Point(75, 25)
        Me.filter_name.Name = "filter_name"
        Me.filter_name.Size = New System.Drawing.Size(128, 27)
        Me.filter_name.TabIndex = 229
        '
        'bankCount
        '
        Me.bankCount.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.bankCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.bankCount.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bankCount.Location = New System.Drawing.Point(373, 134)
        Me.bankCount.Margin = New System.Windows.Forms.Padding(4)
        Me.bankCount.Name = "bankCount"
        Me.bankCount.Size = New System.Drawing.Size(107, 25)
        Me.bankCount.TabIndex = 43
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(321, 139)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(44, 20)
        Me.Label1.TabIndex = 42
        Me.Label1.Text = "Total"
        '
        'bank_name
        '
        Me.bank_name.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.bank_name.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.bank_name.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bank_name.Location = New System.Drawing.Point(112, 27)
        Me.bank_name.Margin = New System.Windows.Forms.Padding(4)
        Me.bank_name.Name = "bank_name"
        Me.bank_name.Size = New System.Drawing.Size(323, 25)
        Me.bank_name.TabIndex = 41
        '
        'LabelBank
        '
        Me.LabelBank.AutoSize = True
        Me.LabelBank.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelBank.ForeColor = System.Drawing.Color.White
        Me.LabelBank.Location = New System.Drawing.Point(8, 27)
        Me.LabelBank.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LabelBank.Name = "LabelBank"
        Me.LabelBank.Size = New System.Drawing.Size(96, 20)
        Me.LabelBank.TabIndex = 40
        Me.LabelBank.Text = "Bank Name:"
        '
        'ButAdd
        '
        Me.ButAdd.BackColor = System.Drawing.Color.White
        Me.ButAdd.Cursor = System.Windows.Forms.Cursors.Hand
        Me.ButAdd.FlatAppearance.BorderColor = System.Drawing.Color.Silver
        Me.ButAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ButAdd.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButAdd.ForeColor = System.Drawing.Color.Black
        Me.ButAdd.Location = New System.Drawing.Point(12, 60)
        Me.ButAdd.Margin = New System.Windows.Forms.Padding(4)
        Me.ButAdd.Name = "ButAdd"
        Me.ButAdd.Size = New System.Drawing.Size(94, 43)
        Me.ButAdd.TabIndex = 39
        Me.ButAdd.Text = "Add New"
        Me.ButAdd.UseVisualStyleBackColor = False
        '
        'ButUpdate
        '
        Me.ButUpdate.BackColor = System.Drawing.Color.White
        Me.ButUpdate.Cursor = System.Windows.Forms.Cursors.Hand
        Me.ButUpdate.FlatAppearance.BorderSize = 0
        Me.ButUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ButUpdate.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButUpdate.ForeColor = System.Drawing.Color.Black
        Me.ButUpdate.Location = New System.Drawing.Point(211, 60)
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
        Me.ButDelete.Location = New System.Drawing.Point(308, 60)
        Me.ButDelete.Margin = New System.Windows.Forms.Padding(4)
        Me.ButDelete.Name = "ButDelete"
        Me.ButDelete.Size = New System.Drawing.Size(89, 43)
        Me.ButDelete.TabIndex = 37
        Me.ButDelete.Text = "Delete"
        Me.ButDelete.UseVisualStyleBackColor = False
        '
        'BtnSave
        '
        Me.BtnSave.BackColor = System.Drawing.Color.DodgerBlue
        Me.BtnSave.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BtnSave.FlatAppearance.BorderSize = 0
        Me.BtnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnSave.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnSave.ForeColor = System.Drawing.Color.White
        Me.BtnSave.Location = New System.Drawing.Point(114, 60)
        Me.BtnSave.Margin = New System.Windows.Forms.Padding(4)
        Me.BtnSave.Name = "BtnSave"
        Me.BtnSave.Size = New System.Drawing.Size(89, 43)
        Me.BtnSave.TabIndex = 36
        Me.BtnSave.Text = "Save"
        Me.BtnSave.UseVisualStyleBackColor = False
        '
        'BankDataGridView
        '
        Me.BankDataGridView.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.BankDataGridView.BackgroundColor = System.Drawing.Color.White
        Me.BankDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.BankDataGridView.Location = New System.Drawing.Point(0, 176)
        Me.BankDataGridView.Name = "BankDataGridView"
        Me.BankDataGridView.RowHeadersVisible = False
        Me.BankDataGridView.Size = New System.Drawing.Size(594, 420)
        Me.BankDataGridView.TabIndex = 6
        '
        'bank_add
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(596, 600)
        Me.Controls.Add(Me.BankDataGridView)
        Me.Controls.Add(Me.GroupBox1)
        Me.Name = "bank_add"
        Me.Text = "bank_add"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox6.ResumeLayout(False)
        Me.GroupBox6.PerformLayout()
        CType(Me.BankDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents bank_name As TextBox
    Friend WithEvents LabelBank As Label
    Friend WithEvents ButAdd As Button
    Friend WithEvents ButUpdate As Button
    Friend WithEvents ButDelete As Button
    Friend WithEvents BtnSave As Button
    Friend WithEvents BankDataGridView As DataGridView
    Friend WithEvents Label1 As Label
    Friend WithEvents bankCount As TextBox
    Friend WithEvents GroupBox6 As GroupBox
    Friend WithEvents filter_name As TextBox
End Class
