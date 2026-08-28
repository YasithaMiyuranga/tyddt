<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class StockTransfer
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
        Dim lblnumb As System.Windows.Forms.Label
        Dim lblname As System.Windows.Forms.Label
        Dim Label3 As System.Windows.Forms.Label
        Dim lbltel As System.Windows.Forms.Label
        Dim lbltrans As System.Windows.Forms.Label
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.trann = New System.Windows.Forms.TextBox()
        Me.LabelLocatio = New System.Windows.Forms.Label()
        Me.recetel = New System.Windows.Forms.TextBox()
        Me.TextBox2 = New System.Windows.Forms.TextBox()
        Me.ComboBoxLocation = New System.Windows.Forms.ComboBox()
        Me.ComboBox1loca = New System.Windows.Forms.ComboBox()
        Me.loca = New System.Windows.Forms.Label()
        Me.TextBox4 = New System.Windows.Forms.TextBox()
        Me.TextBox7 = New System.Windows.Forms.TextBox()
        Me.DataGridView2 = New System.Windows.Forms.DataGridView()
        Me.sdetailsbtn = New System.Windows.Forms.Button()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.btnNext = New System.Windows.Forms.Button()
        lblnumb = New System.Windows.Forms.Label()
        lblname = New System.Windows.Forms.Label()
        Label3 = New System.Windows.Forms.Label()
        lbltel = New System.Windows.Forms.Label()
        lbltrans = New System.Windows.Forms.Label()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        CType(Me.DataGridView2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblnumb
        '
        lblnumb.AutoSize = True
        lblnumb.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        lblnumb.ForeColor = System.Drawing.Color.White
        lblnumb.Location = New System.Drawing.Point(323, 169)
        lblnumb.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        lblnumb.Name = "lblnumb"
        lblnumb.Size = New System.Drawing.Size(106, 20)
        lblnumb.TabIndex = 349
        lblnumb.Text = "Sender Te No:"
        '
        'lblname
        '
        lblname.AutoSize = True
        lblname.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        lblname.ForeColor = System.Drawing.Color.White
        lblname.Location = New System.Drawing.Point(20, 169)
        lblname.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        lblname.Name = "lblname"
        lblname.Size = New System.Drawing.Size(107, 20)
        lblname.TabIndex = 347
        lblname.Text = "Sender Name:"
        '
        'Label3
        '
        Label3.AutoSize = True
        Label3.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Label3.ForeColor = System.Drawing.Color.White
        Label3.Location = New System.Drawing.Point(277, 29)
        Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Label3.Name = "Label3"
        Label3.Size = New System.Drawing.Size(118, 20)
        Label3.TabIndex = 347
        Label3.Text = "Receiver Name:"
        '
        'lbltel
        '
        lbltel.AutoSize = True
        lbltel.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        lbltel.ForeColor = System.Drawing.Color.White
        lbltel.Location = New System.Drawing.Point(591, 29)
        lbltel.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        lbltel.Name = "lbltel"
        lbltel.Size = New System.Drawing.Size(117, 20)
        lbltel.TabIndex = 352
        lbltel.Text = "Receiver Te No:"
        '
        'lbltrans
        '
        lbltrans.AutoSize = True
        lbltrans.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        lbltrans.ForeColor = System.Drawing.Color.White
        lbltrans.Location = New System.Drawing.Point(16, 29)
        lbltrans.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        lbltrans.Name = "lbltrans"
        lbltrans.Size = New System.Drawing.Size(91, 20)
        lbltrans.TabIndex = 355
        lbltrans.Text = "Transfer ID:"
        '
        'GroupBox1
        '
        Me.GroupBox1.BackColor = System.Drawing.Color.DarkSlateGray
        Me.GroupBox1.Controls.Add(Me.GroupBox3)
        Me.GroupBox1.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox1.ForeColor = System.Drawing.Color.White
        Me.GroupBox1.Location = New System.Drawing.Point(6, 11)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(2)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(2)
        Me.GroupBox1.Size = New System.Drawing.Size(938, 146)
        Me.GroupBox1.TabIndex = 38
        Me.GroupBox1.TabStop = False
        '
        'GroupBox3
        '
        Me.GroupBox3.BackColor = System.Drawing.Color.DarkSlateGray
        Me.GroupBox3.Controls.Add(Me.Button1)
        Me.GroupBox3.Controls.Add(Me.trann)
        Me.GroupBox3.Controls.Add(Me.LabelLocatio)
        Me.GroupBox3.Controls.Add(Me.recetel)
        Me.GroupBox3.Controls.Add(lbltel)
        Me.GroupBox3.Controls.Add(lbltrans)
        Me.GroupBox3.Controls.Add(Me.TextBox2)
        Me.GroupBox3.Controls.Add(Me.ComboBoxLocation)
        Me.GroupBox3.Controls.Add(Label3)
        Me.GroupBox3.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox3.ForeColor = System.Drawing.Color.White
        Me.GroupBox3.Location = New System.Drawing.Point(5, 24)
        Me.GroupBox3.Margin = New System.Windows.Forms.Padding(2)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Padding = New System.Windows.Forms.Padding(2)
        Me.GroupBox3.Size = New System.Drawing.Size(910, 105)
        Me.GroupBox3.TabIndex = 391
        Me.GroupBox3.TabStop = False
        '
        'Button1
        '
        Me.Button1.BackColor = System.Drawing.Color.White
        Me.Button1.Cursor = System.Windows.Forms.Cursors.Hand
        Me.Button1.FlatAppearance.BorderSize = 0
        Me.Button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Button1.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.Button1.ForeColor = System.Drawing.Color.Black
        Me.Button1.Location = New System.Drawing.Point(802, 59)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(89, 36)
        Me.Button1.TabIndex = 398
        Me.Button1.Text = "Add New"
        Me.Button1.UseVisualStyleBackColor = False
        '
        'trann
        '
        Me.trann.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.trann.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.trann.Location = New System.Drawing.Point(111, 26)
        Me.trann.Margin = New System.Windows.Forms.Padding(4)
        Me.trann.Name = "trann"
        Me.trann.Size = New System.Drawing.Size(156, 26)
        Me.trann.TabIndex = 356
        '
        'LabelLocatio
        '
        Me.LabelLocatio.AutoSize = True
        Me.LabelLocatio.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.LabelLocatio.ForeColor = System.Drawing.Color.White
        Me.LabelLocatio.Location = New System.Drawing.Point(16, 62)
        Me.LabelLocatio.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LabelLocatio.Name = "LabelLocatio"
        Me.LabelLocatio.Size = New System.Drawing.Size(73, 20)
        Me.LabelLocatio.TabIndex = 315
        Me.LabelLocatio.Text = "Location:"
        '
        'recetel
        '
        Me.recetel.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.recetel.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.recetel.Location = New System.Drawing.Point(711, 26)
        Me.recetel.Margin = New System.Windows.Forms.Padding(4)
        Me.recetel.Name = "recetel"
        Me.recetel.Size = New System.Drawing.Size(180, 26)
        Me.recetel.TabIndex = 353
        '
        'TextBox2
        '
        Me.TextBox2.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TextBox2.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox2.Location = New System.Drawing.Point(403, 26)
        Me.TextBox2.Margin = New System.Windows.Forms.Padding(4)
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.Size = New System.Drawing.Size(180, 26)
        Me.TextBox2.TabIndex = 348
        '
        'ComboBoxLocation
        '
        Me.ComboBoxLocation.FormattingEnabled = True
        Me.ComboBoxLocation.Location = New System.Drawing.Point(111, 59)
        Me.ComboBoxLocation.Name = "ComboBoxLocation"
        Me.ComboBoxLocation.Size = New System.Drawing.Size(156, 28)
        Me.ComboBoxLocation.TabIndex = 338
        '
        'ComboBox1loca
        '
        Me.ComboBox1loca.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.ComboBox1loca.FormattingEnabled = True
        Me.ComboBox1loca.Location = New System.Drawing.Point(1105, 168)
        Me.ComboBox1loca.Name = "ComboBox1loca"
        Me.ComboBox1loca.Size = New System.Drawing.Size(147, 28)
        Me.ComboBox1loca.TabIndex = 338
        '
        'loca
        '
        Me.loca.AutoSize = True
        Me.loca.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.loca.ForeColor = System.Drawing.Color.White
        Me.loca.Location = New System.Drawing.Point(966, 171)
        Me.loca.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.loca.Name = "loca"
        Me.loca.Size = New System.Drawing.Size(136, 20)
        Me.loca.TabIndex = 315
        Me.loca.Text = "Receiver Location:"
        '
        'TextBox4
        '
        Me.TextBox4.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TextBox4.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox4.Location = New System.Drawing.Point(437, 166)
        Me.TextBox4.Margin = New System.Windows.Forms.Padding(4)
        Me.TextBox4.Name = "TextBox4"
        Me.TextBox4.Size = New System.Drawing.Size(180, 26)
        Me.TextBox4.TabIndex = 350
        '
        'TextBox7
        '
        Me.TextBox7.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.TextBox7.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox7.Location = New System.Drawing.Point(135, 166)
        Me.TextBox7.Margin = New System.Windows.Forms.Padding(4)
        Me.TextBox7.Name = "TextBox7"
        Me.TextBox7.Size = New System.Drawing.Size(180, 26)
        Me.TextBox7.TabIndex = 348
        '
        'DataGridView2
        '
        Me.DataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView2.Location = New System.Drawing.Point(6, 202)
        Me.DataGridView2.Name = "DataGridView2"
        Me.DataGridView2.Size = New System.Drawing.Size(1244, 388)
        Me.DataGridView2.TabIndex = 354
        Me.DataGridView2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        '
        'sdetailsbtn
        '
        Me.sdetailsbtn.BackColor = System.Drawing.Color.DodgerBlue
        Me.sdetailsbtn.Cursor = System.Windows.Forms.Cursors.Hand
        Me.sdetailsbtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.sdetailsbtn.Location = New System.Drawing.Point(1024, 11)
        Me.sdetailsbtn.Margin = New System.Windows.Forms.Padding(4)
        Me.sdetailsbtn.Name = "sdetailsbtn"
        Me.sdetailsbtn.Size = New System.Drawing.Size(94, 47)
        Me.sdetailsbtn.TabIndex = 400
        Me.sdetailsbtn.Text = "Transfer" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "History"
        Me.sdetailsbtn.UseVisualStyleBackColor = False
        '
        'btnSave
        '
        Me.btnSave.BackColor = System.Drawing.Color.DeepSkyBlue
        Me.btnSave.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnSave.FlatAppearance.BorderSize = 0
        Me.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnSave.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.btnSave.ForeColor = System.Drawing.Color.Black
        Me.btnSave.Location = New System.Drawing.Point(1068, 79)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(92, 43)
        Me.btnSave.TabIndex = 398
        Me.btnSave.Text = "Send"
        Me.btnSave.UseVisualStyleBackColor = False
        '
        'btnNext
        '
        Me.btnNext.BackColor = System.Drawing.Color.DodgerBlue
        Me.btnNext.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnNext.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnNext.ForeColor = System.Drawing.Color.White
        Me.btnNext.Location = New System.Drawing.Point(970, 79)
        Me.btnNext.Name = "btnNext"
        Me.btnNext.Size = New System.Drawing.Size(92, 43)
        Me.btnNext.TabIndex = 399
        Me.btnNext.Text = "Next"
        Me.btnNext.UseVisualStyleBackColor = False
        '
        'StockTransfer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(44, Byte), Integer), CType(CType(62, Byte), Integer), CType(CType(80, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(1264, 600)
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.Controls.Add(Me.sdetailsbtn)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.btnNext)
        Me.Controls.Add(Me.DataGridView2)
        Me.Controls.Add(Me.TextBox4)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(lblnumb)
        Me.Controls.Add(Me.TextBox7)
        Me.Controls.Add(lblname)
        Me.Controls.Add(Me.loca)
        Me.Controls.Add(Me.ComboBox1loca)
        Me.Name = "StockTransfer"
        Me.Text = "StockTransfer"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        CType(Me.DataGridView2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents LabelLocatio As Label
    Friend WithEvents ComboBoxLocation As ComboBox
    Friend WithEvents TextBox4 As TextBox
    Friend WithEvents TextBox7 As TextBox
    Friend WithEvents TextBox2 As TextBox
    Friend WithEvents ComboBox1loca As ComboBox
    Friend WithEvents loca As Label
    Friend WithEvents recetel As TextBox
    Friend WithEvents trann As TextBox
    Friend WithEvents DataGridView2 As DataGridView
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents Button1 As Button
    Friend WithEvents sdetailsbtn As Button
    Friend WithEvents btnSave As Button
    Friend WithEvents btnNext As Button
End Class
