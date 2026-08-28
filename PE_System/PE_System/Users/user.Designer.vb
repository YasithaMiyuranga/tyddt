<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class user
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
        Dim lbluserid As System.Windows.Forms.Label
        Dim lblusername As System.Windows.Forms.Label
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(user))
        Me.lblFinancialRole = New System.Windows.Forms.Label()
        Me.lblRole = New System.Windows.Forms.Label()
        Me.lblSecureKey = New System.Windows.Forms.Label()
        Me.lblHiddenKey = New System.Windows.Forms.Label()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.txtUserId = New System.Windows.Forms.TextBox()
        Me.txtUserName = New System.Windows.Forms.TextBox()
        Me.btnAddUser = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.cmbRole = New System.Windows.Forms.ComboBox()
        Me.cmbFinancialRole = New System.Windows.Forms.ComboBox()
        Me.txtSecureKey = New System.Windows.Forms.TextBox()
        Me.txtHiddenKey = New System.Windows.Forms.TextBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        lbluserid = New System.Windows.Forms.Label()
        lblusername = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'lbluserid
        '
        lbluserid.AutoSize = True
        lbluserid.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        lbluserid.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        lbluserid.Location = New System.Drawing.Point(62, 510)
        lbluserid.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        lbluserid.Name = "lbluserid"
        lbluserid.Size = New System.Drawing.Size(97, 29)
        lbluserid.TabIndex = 7
        lbluserid.Text = "User Id"
        '
        'lblusername
        '
        lblusername.AutoSize = True
        lblusername.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        lblusername.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        lblusername.Location = New System.Drawing.Point(62, 580)
        lblusername.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        lblusername.Name = "lblusername"
        lblusername.Size = New System.Drawing.Size(144, 29)
        lblusername.TabIndex = 8
        lblusername.Text = "User Name"
        '
        'lblFinancialRole
        '
        Me.lblFinancialRole.AutoSize = True
        Me.lblFinancialRole.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblFinancialRole.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.lblFinancialRole.Location = New System.Drawing.Point(53, 727)
        Me.lblFinancialRole.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.lblFinancialRole.Name = "lblFinancialRole"
        Me.lblFinancialRole.Size = New System.Drawing.Size(149, 29)
        Me.lblFinancialRole.TabIndex = 12
        Me.lblFinancialRole.Text = "Billing Role"
        '
        'lblRole
        '
        Me.lblRole.AutoSize = True
        Me.lblRole.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblRole.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.lblRole.Location = New System.Drawing.Point(62, 652)
        Me.lblRole.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.lblRole.Name = "lblRole"
        Me.lblRole.Size = New System.Drawing.Size(68, 29)
        Me.lblRole.TabIndex = 11
        Me.lblRole.Text = "Role"
        '
        'lblSecureKey
        '
        Me.lblSecureKey.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!)
        Me.lblSecureKey.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.lblSecureKey.Location = New System.Drawing.Point(59, 796)
        Me.lblSecureKey.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.lblSecureKey.Name = "lblSecureKey"
        Me.lblSecureKey.Size = New System.Drawing.Size(125, 30)
        Me.lblSecureKey.TabIndex = 13
        Me.lblSecureKey.Text = "Password"
        '
        'lblHiddenKey
        '
        Me.lblHiddenKey.AutoSize = True
        Me.lblHiddenKey.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblHiddenKey.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.lblHiddenKey.Location = New System.Drawing.Point(55, 868)
        Me.lblHiddenKey.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.lblHiddenKey.Name = "lblHiddenKey"
        Me.lblHiddenKey.Size = New System.Drawing.Size(148, 29)
        Me.lblHiddenKey.TabIndex = 14
        Me.lblHiddenKey.Text = "Hidden Key"
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.Color.Transparent
        Me.lblTitle.Font = New System.Drawing.Font("Microsoft Uighur", 36.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Italic), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTitle.ForeColor = System.Drawing.Color.White
        Me.lblTitle.Location = New System.Drawing.Point(-1, 122)
        Me.lblTitle.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(750, 107)
        Me.lblTitle.TabIndex = 0
        Me.lblTitle.Text = "Create Users"
        Me.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'txtUserId
        '
        Me.txtUserId.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtUserId.Location = New System.Drawing.Point(218, 500)
        Me.txtUserId.Margin = New System.Windows.Forms.Padding(6)
        Me.txtUserId.Name = "txtUserId"
        Me.txtUserId.ReadOnly = True
        Me.txtUserId.Size = New System.Drawing.Size(442, 39)
        Me.txtUserId.TabIndex = 4
        '
        'txtUserName
        '
        Me.txtUserName.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtUserName.Location = New System.Drawing.Point(218, 572)
        Me.txtUserName.Margin = New System.Windows.Forms.Padding(6)
        Me.txtUserName.Name = "txtUserName"
        Me.txtUserName.Size = New System.Drawing.Size(442, 39)
        Me.txtUserName.TabIndex = 5
        '
        'btnAddUser
        '
        Me.btnAddUser.Location = New System.Drawing.Point(218, 960)
        Me.btnAddUser.Margin = New System.Windows.Forms.Padding(4)
        Me.btnAddUser.Name = "btnAddUser"
        Me.btnAddUser.Size = New System.Drawing.Size(446, 66)
        Me.btnAddUser.TabIndex = 11
        Me.btnAddUser.Text = "Add User"
        Me.btnAddUser.UseVisualStyleBackColor = False
        '
        'btnCancel
        '
        Me.btnCancel.BackColor = System.Drawing.Color.LightCoral
        Me.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnCancel.FlatAppearance.BorderSize = 0
        Me.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCancel.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
        Me.btnCancel.ForeColor = System.Drawing.Color.Black
        Me.btnCancel.Location = New System.Drawing.Point(218, 1043)
        Me.btnCancel.Margin = New System.Windows.Forms.Padding(4)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(446, 66)
        Me.btnCancel.TabIndex = 12
        Me.btnCancel.Text = "CANCEL"
        Me.btnCancel.UseVisualStyleBackColor = False
        '
        'cmbRole
        '
        Me.cmbRole.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbRole.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmbRole.FormattingEnabled = True
        Me.cmbRole.Location = New System.Drawing.Point(218, 644)
        Me.cmbRole.Margin = New System.Windows.Forms.Padding(4)
        Me.cmbRole.Name = "cmbRole"
        Me.cmbRole.Size = New System.Drawing.Size(442, 40)
        Me.cmbRole.TabIndex = 7
        '
        'cmbFinancialRole
        '
        Me.cmbFinancialRole.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbFinancialRole.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmbFinancialRole.FormattingEnabled = True
        Me.cmbFinancialRole.Location = New System.Drawing.Point(218, 716)
        Me.cmbFinancialRole.Margin = New System.Windows.Forms.Padding(4)
        Me.cmbFinancialRole.Name = "cmbFinancialRole"
        Me.cmbFinancialRole.Size = New System.Drawing.Size(442, 40)
        Me.cmbFinancialRole.TabIndex = 8
        '
        'txtSecureKey
        '
        Me.txtSecureKey.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtSecureKey.Location = New System.Drawing.Point(218, 788)
        Me.txtSecureKey.Margin = New System.Windows.Forms.Padding(6)
        Me.txtSecureKey.Name = "txtSecureKey"
        Me.txtSecureKey.Size = New System.Drawing.Size(442, 39)
        Me.txtSecureKey.TabIndex = 9
        Me.txtSecureKey.UseSystemPasswordChar = True
        '
        'txtHiddenKey
        '
        Me.txtHiddenKey.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtHiddenKey.Location = New System.Drawing.Point(218, 860)
        Me.txtHiddenKey.Margin = New System.Windows.Forms.Padding(6)
        Me.txtHiddenKey.Name = "txtHiddenKey"
        Me.txtHiddenKey.Size = New System.Drawing.Size(442, 39)
        Me.txtHiddenKey.TabIndex = 10
        '
        'GroupBox1
        '
        Me.GroupBox1.BackgroundImage = CType(resources.GetObject("GroupBox1.BackgroundImage"), System.Drawing.Image)
        Me.GroupBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.GroupBox1.Location = New System.Drawing.Point(269, 316)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.GroupBox1.Size = New System.Drawing.Size(261, 152)
        Me.GroupBox1.TabIndex = 92
        Me.GroupBox1.TabStop = False
        '
        'user
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(11.0!, 24.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.DarkSlateGray
        Me.ClientSize = New System.Drawing.Size(750, 1080)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnAddUser)
        Me.Controls.Add(Me.cmbRole)
        Me.Controls.Add(Me.cmbFinancialRole)
        Me.Controls.Add(Me.lblRole)
        Me.Controls.Add(Me.lblFinancialRole)
        Me.Controls.Add(Me.txtSecureKey)
        Me.Controls.Add(Me.lblSecureKey)
        Me.Controls.Add(Me.txtHiddenKey)
        Me.Controls.Add(Me.lblHiddenKey)
        Me.Controls.Add(lblusername)
        Me.Controls.Add(lbluserid)
        Me.Controls.Add(Me.txtUserName)
        Me.Controls.Add(Me.txtUserId)
        Me.Controls.Add(Me.lblTitle)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "user"
        Me.Text = "Create User"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents txtUserId As System.Windows.Forms.TextBox
    Friend WithEvents txtUserName As System.Windows.Forms.TextBox
    Friend WithEvents btnAddUser As System.Windows.Forms.Button
    Friend WithEvents cmbRole As System.Windows.Forms.ComboBox
    Friend WithEvents cmbFinancialRole As System.Windows.Forms.ComboBox
    Friend WithEvents txtSecureKey As System.Windows.Forms.TextBox
    Friend WithEvents txtHiddenKey As System.Windows.Forms.TextBox
    Friend WithEvents lblRole As Label
    Friend WithEvents lblFinancialRole As Label
    Friend WithEvents lblSecureKey As Label
    Friend WithEvents lblHiddenKey As Label
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As GroupBox
End Class
