<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SupplierManualPaymentDistributor
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
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.LabelTotal = New System.Windows.Forms.Label()
        Me.LabelRemaining = New System.Windows.Forms.Label()
        Me.BtnOk = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.LabelSupplier = New System.Windows.Forms.Label()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'DataGridView1
        '
        Me.DataGridView1.AllowUserToAddRows = False
        Me.DataGridView1.AllowUserToDeleteRows = False
        Me.DataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DataGridView1.BackgroundColor = System.Drawing.Color.White
        Me.DataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.DataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.EnableHeadersVisualStyles = False
        Me.DataGridView1.GridColor = System.Drawing.Color.SteelBlue
        Me.DataGridView1.Location = New System.Drawing.Point(15, 80)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.RowHeadersVisible = False
        Me.DataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DataGridView1.Size = New System.Drawing.Size(820, 440)
        Me.DataGridView1.TabIndex = 0
        '
        'LabelTotal
        '
        Me.LabelTotal.AutoSize = True
        Me.LabelTotal.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.LabelTotal.ForeColor = System.Drawing.Color.Aqua
        Me.LabelTotal.Location = New System.Drawing.Point(143, 540)
        Me.LabelTotal.Name = "LabelTotal"
        Me.LabelTotal.Size = New System.Drawing.Size(41, 21)
        Me.LabelTotal.TabIndex = 1
        Me.LabelTotal.Text = "0.00"
        '
        'LabelRemaining
        '
        Me.LabelRemaining.AutoSize = True
        Me.LabelRemaining.Font = New System.Drawing.Font("Segoe UI", 14.0!, System.Drawing.FontStyle.Bold)
        Me.LabelRemaining.ForeColor = System.Drawing.Color.Tomato
        Me.LabelRemaining.Location = New System.Drawing.Point(350, 538)
        Me.LabelRemaining.Name = "LabelRemaining"
        Me.LabelRemaining.Size = New System.Drawing.Size(50, 25)
        Me.LabelRemaining.TabIndex = 2
        Me.LabelRemaining.Text = "0.00"
        '
        'BtnOk
        '
        Me.BtnOk.BackColor = System.Drawing.Color.SeaGreen
        Me.BtnOk.FlatAppearance.BorderSize = 0
        Me.BtnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnOk.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.BtnOk.ForeColor = System.Drawing.Color.White
        Me.BtnOk.Location = New System.Drawing.Point(350, 580)
        Me.BtnOk.Name = "BtnOk"
        Me.BtnOk.Size = New System.Drawing.Size(180, 50)
        Me.BtnOk.TabIndex = 3
        Me.BtnOk.Text = "COMPLETE"
        Me.BtnOk.UseVisualStyleBackColor = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(12, 542)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(125, 19)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "Total Paid Amount:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
        Me.Label2.ForeColor = System.Drawing.Color.White
        Me.Label2.Location = New System.Drawing.Point(260, 543)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(92, 19)
        Me.Label2.TabIndex = 5
        Me.Label2.Text = "REMAINING:"
        '
        'LabelSupplier
        '
        Me.LabelSupplier.AutoSize = True
        Me.LabelSupplier.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.LabelSupplier.ForeColor = System.Drawing.Color.White
        Me.LabelSupplier.Location = New System.Drawing.Point(15, 20)
        Me.LabelSupplier.Name = "LabelSupplier"
        Me.LabelSupplier.Size = New System.Drawing.Size(114, 21)
        Me.LabelSupplier.TabIndex = 6
        Me.LabelSupplier.Text = "Supplier: N/A"
        '
        'LabelTotalDebit
        '
        Me.LabelTotalDebit = New System.Windows.Forms.Label()
        Me.LabelTotalDebit.AutoSize = True
        Me.LabelTotalDebit.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.LabelTotalDebit.ForeColor = System.Drawing.Color.Yellow
        Me.LabelTotalDebit.Location = New System.Drawing.Point(15, 45)
        Me.LabelTotalDebit.Name = "LabelTotalDebit"
        Me.LabelTotalDebit.Size = New System.Drawing.Size(180, 21)
        Me.LabelTotalDebit.TabIndex = 7
        Me.LabelTotalDebit.Text = "Total Outstanding: 0.00"
        '
        'SupplierManualPaymentDistributor
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(37, Byte), Integer), CType(CType(46, Byte), Integer), CType(CType(59, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(850, 650)
        Me.Controls.Add(Me.LabelTotalDebit)
        Me.Controls.Add(Me.LabelSupplier)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.BtnOk)
        Me.Controls.Add(Me.LabelRemaining)
        Me.Controls.Add(Me.LabelTotal)
        Me.Controls.Add(Me.DataGridView1)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.ForeColor = System.Drawing.Color.White
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "SupplierManualPaymentDistributor"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Manual Payment Distribution (Supplier)"
        Me.AutoScroll = False
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents DataGridView1 As System.Windows.Forms.DataGridView
    Friend WithEvents LabelTotal As System.Windows.Forms.Label
    Friend WithEvents LabelRemaining As System.Windows.Forms.Label
    Friend WithEvents BtnOk As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents LabelSupplier As System.Windows.Forms.Label
    Friend WithEvents LabelTotalDebit As System.Windows.Forms.Label
End Class
