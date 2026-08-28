<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Home
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

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Home))
        Dim ChartArea1 As System.Windows.Forms.DataVisualization.Charting.ChartArea = New System.Windows.Forms.DataVisualization.Charting.ChartArea()
        Dim Series1 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Dim Title1 As System.Windows.Forms.DataVisualization.Charting.Title = New System.Windows.Forms.DataVisualization.Charting.Title()
        Dim ChartArea2 As System.Windows.Forms.DataVisualization.Charting.ChartArea = New System.Windows.Forms.DataVisualization.Charting.ChartArea()
        Dim Legend1 As System.Windows.Forms.DataVisualization.Charting.Legend = New System.Windows.Forms.DataVisualization.Charting.Legend()
        Dim Series2 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Dim Title2 As System.Windows.Forms.DataVisualization.Charting.Title = New System.Windows.Forms.DataVisualization.Charting.Title()
        Me.pnlHeader = New System.Windows.Forms.Panel()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.lblUserDisplay = New System.Windows.Forms.Label()
        Me.lblDashTitle = New System.Windows.Forms.Label()
        Me.flowCards = New System.Windows.Forms.FlowLayoutPanel()
        Me.pnlSalesCard = New System.Windows.Forms.Panel()
        Me.lblTodaySales = New System.Windows.Forms.Label()
        Me.lblTodaySalesTitle = New System.Windows.Forms.Label()
        Me.pnlPaidCard = New System.Windows.Forms.Panel()
        Me.lblTodayPaid = New System.Windows.Forms.Label()
        Me.lblTodayPaidTitle = New System.Windows.Forms.Label()
        Me.pnlStockCard = New System.Windows.Forms.Panel()
        Me.lblLowStockCount = New System.Windows.Forms.Label()
        Me.lblLowStockTitle = New System.Windows.Forms.Label()
        Me.flowCharts = New System.Windows.Forms.FlowLayoutPanel()
        Me.chartSales = New System.Windows.Forms.DataVisualization.Charting.Chart()
        Me.chartStock = New System.Windows.Forms.DataVisualization.Charting.Chart()
        Me.pnlCreditDetails = New System.Windows.Forms.Panel()
        Me.tabsManagement = New System.Windows.Forms.TabControl()
        Me.tabPageCustomerCredits = New System.Windows.Forms.TabPage()
        Me.dgvCustomerCredits = New System.Windows.Forms.DataGridView()
        Me.tabPageBlockedCustomers = New System.Windows.Forms.TabPage()
        Me.dgvBlockedCustomers = New System.Windows.Forms.DataGridView()
        Me.tabPageSupplierCredits = New System.Windows.Forms.TabPage()
        Me.dgvSupplierCredits = New System.Windows.Forms.DataGridView()
        Me.tabPageSupplierAlerts = New System.Windows.Forms.TabPage()
        Me.dgvSupplierAlerts = New System.Windows.Forms.DataGridView()
        Me.tabPageCustomerReturns = New System.Windows.Forms.TabPage()
        Me.dgvCustomerReturnCheques = New System.Windows.Forms.DataGridView()
        Me.tabPageSupplierReturns = New System.Windows.Forms.TabPage()
        Me.dgvSupplierReturnCheques = New System.Windows.Forms.DataGridView()
        Me.headerPanel = New System.Windows.Forms.Panel()
        Me.lblCreditTitle = New System.Windows.Forms.Label()
        Me.pnlHeader.SuspendLayout()
        Me.flowCards.SuspendLayout()
        Me.pnlSalesCard.SuspendLayout()
        Me.pnlPaidCard.SuspendLayout()
        Me.pnlStockCard.SuspendLayout()
        Me.flowCharts.SuspendLayout()
        CType(Me.chartSales, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.chartStock, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlCreditDetails.SuspendLayout()
        Me.tabsManagement.SuspendLayout()
        Me.tabPageCustomerCredits.SuspendLayout()
        CType(Me.dgvCustomerCredits, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabPageBlockedCustomers.SuspendLayout()
        CType(Me.dgvBlockedCustomers, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabPageSupplierCredits.SuspendLayout()
        CType(Me.dgvSupplierCredits, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabPageSupplierAlerts.SuspendLayout()
        CType(Me.dgvSupplierAlerts, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabPageCustomerReturns.SuspendLayout()
        CType(Me.dgvCustomerReturnCheques, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabPageSupplierReturns.SuspendLayout()
        CType(Me.dgvSupplierReturnCheques, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.headerPanel.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlHeader
        '
        Me.pnlHeader.BackColor = System.Drawing.Color.FromArgb(CType(CType(44, Byte), Integer), CType(CType(62, Byte), Integer), CType(CType(80, Byte), Integer))
        Me.pnlHeader.Controls.Add(Me.GroupBox1)
        Me.pnlHeader.Controls.Add(Me.lblUserDisplay)
        Me.pnlHeader.Controls.Add(Me.lblDashTitle)
        Me.pnlHeader.Controls.Add(Me.flowCards)
        Me.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlHeader.Location = New System.Drawing.Point(0, 0)
        Me.pnlHeader.Margin = New System.Windows.Forms.Padding(6, 3, 6, 3)
        Me.pnlHeader.Name = "pnlHeader"
        Me.pnlHeader.Size = New System.Drawing.Size(2244, 333)
        Me.pnlHeader.TabIndex = 0
        '
        'GroupBox1
        '
        Me.GroupBox1.BackgroundImage = CType(resources.GetObject("GroupBox1.BackgroundImage"), System.Drawing.Image)
        Me.GroupBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.GroupBox1.Location = New System.Drawing.Point(13, 165)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.GroupBox1.Size = New System.Drawing.Size(218, 124)
        Me.GroupBox1.TabIndex = 91
        Me.GroupBox1.TabStop = False
        '
        'lblUserDisplay
        '
        Me.lblUserDisplay.AutoSize = True
        Me.lblUserDisplay.Font = New System.Drawing.Font("Segoe UI", 11.0!)
        Me.lblUserDisplay.ForeColor = System.Drawing.Color.LightGray
        Me.lblUserDisplay.Location = New System.Drawing.Point(1302, 38)
        Me.lblUserDisplay.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.lblUserDisplay.Name = "lblUserDisplay"
        Me.lblUserDisplay.Size = New System.Drawing.Size(150, 36)
        Me.lblUserDisplay.TabIndex = 3
        Me.lblUserDisplay.Text = "Login User: "
        '
        'lblDashTitle
        '
        Me.lblDashTitle.AutoSize = True
        Me.lblDashTitle.Font = New System.Drawing.Font("Segoe UI", 24.0!, System.Drawing.FontStyle.Bold)
        Me.lblDashTitle.ForeColor = System.Drawing.Color.White
        Me.lblDashTitle.Location = New System.Drawing.Point(248, 38)
        Me.lblDashTitle.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.lblDashTitle.Name = "lblDashTitle"
        Me.lblDashTitle.Size = New System.Drawing.Size(818, 74)
        Me.lblDashTitle.TabIndex = 0
        Me.lblDashTitle.Text = "Business Overview Dashboard"
        '
        'flowCards
        '
        Me.flowCards.Controls.Add(Me.pnlSalesCard)
        Me.flowCards.Controls.Add(Me.pnlPaidCard)
        Me.flowCards.Controls.Add(Me.pnlStockCard)
        Me.flowCards.Location = New System.Drawing.Point(248, 138)
        Me.flowCards.Margin = New System.Windows.Forms.Padding(6, 3, 6, 3)
        Me.flowCards.Name = "flowCards"
        Me.flowCards.Size = New System.Drawing.Size(2078, 184)
        Me.flowCards.TabIndex = 1
        '
        'pnlSalesCard
        '
        Me.pnlSalesCard.BackColor = System.Drawing.Color.White
        Me.pnlSalesCard.Controls.Add(Me.lblTodaySales)
        Me.pnlSalesCard.Controls.Add(Me.lblTodaySalesTitle)
        Me.pnlSalesCard.Location = New System.Drawing.Point(10, 9)
        Me.pnlSalesCard.Margin = New System.Windows.Forms.Padding(10, 9, 28, 9)
        Me.pnlSalesCard.Name = "pnlSalesCard"
        Me.pnlSalesCard.Padding = New System.Windows.Forms.Padding(28, 27, 28, 27)
        Me.pnlSalesCard.Size = New System.Drawing.Size(660, 166)
        Me.pnlSalesCard.TabIndex = 0
        '
        'lblTodaySales
        '
        Me.lblTodaySales.AutoSize = True
        Me.lblTodaySales.Font = New System.Drawing.Font("Segoe UI", 22.0!, System.Drawing.FontStyle.Bold)
        Me.lblTodaySales.ForeColor = System.Drawing.Color.FromArgb(CType(CType(46, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(113, Byte), Integer))
        Me.lblTodaySales.Location = New System.Drawing.Point(-7, 90)
        Me.lblTodaySales.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.lblTodaySales.Name = "lblTodaySales"
        Me.lblTodaySales.Size = New System.Drawing.Size(219, 70)
        Me.lblTodaySales.TabIndex = 1
        Me.lblTodaySales.Text = "Rs. 0.00"
        '
        'lblTodaySalesTitle
        '
        Me.lblTodaySalesTitle.AutoSize = True
        Me.lblTodaySalesTitle.Font = New System.Drawing.Font("Segoe UI", 12.0!)
        Me.lblTodaySalesTitle.ForeColor = System.Drawing.Color.Gray
        Me.lblTodaySalesTitle.Location = New System.Drawing.Point(28, 27)
        Me.lblTodaySalesTitle.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.lblTodaySalesTitle.Name = "lblTodaySalesTitle"
        Me.lblTodaySalesTitle.Size = New System.Drawing.Size(248, 38)
        Me.lblTodaySalesTitle.TabIndex = 0
        Me.lblTodaySalesTitle.Text = "Today's Sales (LKR)"
        '
        'pnlPaidCard
        '
        Me.pnlPaidCard.BackColor = System.Drawing.Color.White
        Me.pnlPaidCard.Controls.Add(Me.lblTodayPaid)
        Me.pnlPaidCard.Controls.Add(Me.lblTodayPaidTitle)
        Me.pnlPaidCard.Location = New System.Drawing.Point(708, 9)
        Me.pnlPaidCard.Margin = New System.Windows.Forms.Padding(10, 9, 28, 9)
        Me.pnlPaidCard.Name = "pnlPaidCard"
        Me.pnlPaidCard.Padding = New System.Windows.Forms.Padding(28, 27, 28, 27)
        Me.pnlPaidCard.Size = New System.Drawing.Size(667, 166)
        Me.pnlPaidCard.TabIndex = 2
        '
        'lblTodayPaid
        '
        Me.lblTodayPaid.AutoSize = True
        Me.lblTodayPaid.Font = New System.Drawing.Font("Segoe UI", 22.0!, System.Drawing.FontStyle.Bold)
        Me.lblTodayPaid.ForeColor = System.Drawing.Color.FromArgb(CType(CType(52, Byte), Integer), CType(CType(152, Byte), Integer), CType(CType(219, Byte), Integer))
        Me.lblTodayPaid.Location = New System.Drawing.Point(-6, 90)
        Me.lblTodayPaid.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.lblTodayPaid.Name = "lblTodayPaid"
        Me.lblTodayPaid.Size = New System.Drawing.Size(219, 70)
        Me.lblTodayPaid.TabIndex = 1
        Me.lblTodayPaid.Text = "Rs. 0.00"
        '
        'lblTodayPaidTitle
        '
        Me.lblTodayPaidTitle.AutoSize = True
        Me.lblTodayPaidTitle.Font = New System.Drawing.Font("Segoe UI", 12.0!)
        Me.lblTodayPaidTitle.ForeColor = System.Drawing.Color.Gray
        Me.lblTodayPaidTitle.Location = New System.Drawing.Point(28, 27)
        Me.lblTodayPaidTitle.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.lblTodayPaidTitle.Name = "lblTodayPaidTitle"
        Me.lblTodayPaidTitle.Size = New System.Drawing.Size(308, 38)
        Me.lblTodayPaidTitle.TabIndex = 0
        Me.lblTodayPaidTitle.Text = "Today's Collection (LKR)"
        '
        'pnlStockCard
        '
        Me.pnlStockCard.BackColor = System.Drawing.Color.White
        Me.pnlStockCard.Controls.Add(Me.lblLowStockCount)
        Me.pnlStockCard.Controls.Add(Me.lblLowStockTitle)
        Me.pnlStockCard.Location = New System.Drawing.Point(1413, 9)
        Me.pnlStockCard.Margin = New System.Windows.Forms.Padding(10, 9, 10, 9)
        Me.pnlStockCard.Name = "pnlStockCard"
        Me.pnlStockCard.Padding = New System.Windows.Forms.Padding(28, 27, 28, 27)
        Me.pnlStockCard.Size = New System.Drawing.Size(638, 166)
        Me.pnlStockCard.TabIndex = 1
        '
        'lblLowStockCount
        '
        Me.lblLowStockCount.AutoSize = True
        Me.lblLowStockCount.Font = New System.Drawing.Font("Segoe UI", 22.0!, System.Drawing.FontStyle.Bold)
        Me.lblLowStockCount.ForeColor = System.Drawing.Color.FromArgb(CType(CType(231, Byte), Integer), CType(CType(76, Byte), Integer), CType(CType(60, Byte), Integer))
        Me.lblLowStockCount.Location = New System.Drawing.Point(28, 102)
        Me.lblLowStockCount.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.lblLowStockCount.Name = "lblLowStockCount"
        Me.lblLowStockCount.Size = New System.Drawing.Size(60, 70)
        Me.lblLowStockCount.TabIndex = 1
        Me.lblLowStockCount.Text = "0"
        '
        'lblLowStockTitle
        '
        Me.lblLowStockTitle.AutoSize = True
        Me.lblLowStockTitle.Font = New System.Drawing.Font("Segoe UI", 12.0!)
        Me.lblLowStockTitle.ForeColor = System.Drawing.Color.Gray
        Me.lblLowStockTitle.Location = New System.Drawing.Point(28, 27)
        Me.lblLowStockTitle.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.lblLowStockTitle.Name = "lblLowStockTitle"
        Me.lblLowStockTitle.Size = New System.Drawing.Size(215, 38)
        Me.lblLowStockTitle.TabIndex = 0
        Me.lblLowStockTitle.Text = "Low Stock Items"
        '
        'flowCharts
        '
        Me.flowCharts.Controls.Add(Me.chartSales)
        Me.flowCharts.Controls.Add(Me.chartStock)
        Me.flowCharts.Dock = System.Windows.Forms.DockStyle.Fill
        Me.flowCharts.Location = New System.Drawing.Point(0, 333)
        Me.flowCharts.Margin = New System.Windows.Forms.Padding(6)
        Me.flowCharts.Name = "flowCharts"
        Me.flowCharts.Size = New System.Drawing.Size(2244, 441)
        Me.flowCharts.TabIndex = 1
        '
        'chartSales
        '
        ChartArea1.AxisX.LabelStyle.Font = New System.Drawing.Font("Segoe UI", 8.0!)
        ChartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.LightGray
        ChartArea1.AxisY.LabelStyle.Font = New System.Drawing.Font("Segoe UI", 8.0!)
        ChartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.LightGray
        ChartArea1.Name = "MainArea"
        Me.chartSales.ChartAreas.Add(ChartArea1)
        Me.chartSales.Location = New System.Drawing.Point(6, 3)
        Me.chartSales.Margin = New System.Windows.Forms.Padding(6, 3, 6, 3)
        Me.chartSales.Name = "chartSales"
        Series1.ChartArea = "MainArea"
        Series1.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Series1.Name = "Data"
        Me.chartSales.Series.Add(Series1)
        Me.chartSales.Size = New System.Drawing.Size(1393, 424)
        Me.chartSales.TabIndex = 0
        Me.chartSales.Text = "chartSales"
        Title1.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Title1.Name = "Title1"
        Title1.Text = "Sales by Category (LKR)"
        Me.chartSales.Titles.Add(Title1)
        '
        'chartStock
        '
        ChartArea2.AxisX.LabelStyle.Font = New System.Drawing.Font("Segoe UI", 8.0!)
        ChartArea2.AxisX.MajorGrid.LineColor = System.Drawing.Color.LightGray
        ChartArea2.AxisY.LabelStyle.Font = New System.Drawing.Font("Segoe UI", 8.0!)
        ChartArea2.AxisY.MajorGrid.LineColor = System.Drawing.Color.LightGray
        ChartArea2.Name = "MainArea"
        Me.chartStock.ChartAreas.Add(ChartArea2)
        Legend1.Font = New System.Drawing.Font("Segoe UI", 8.0!)
        Legend1.IsTextAutoFit = False
        Legend1.Name = "Legend1"
        Me.chartStock.Legends.Add(Legend1)
        Me.chartStock.Location = New System.Drawing.Point(23, 433)
        Me.chartStock.Margin = New System.Windows.Forms.Padding(23, 3, 6, 3)
        Me.chartStock.Name = "chartStock"
        Series2.ChartArea = "MainArea"
        Series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie
        Series2.Font = New System.Drawing.Font("Segoe UI", 8.0!, System.Drawing.FontStyle.Bold)
        Series2.IsValueShownAsLabel = True
        Series2.Label = "#PERCENT{P0}"
        Series2.Legend = "Legend1"
        Series2.LegendText = "#VALX (#VALY)"
        Series2.Name = "Data"
        Me.chartStock.Series.Add(Series2)
        Me.chartStock.Size = New System.Drawing.Size(868, 424)
        Me.chartStock.TabIndex = 1
        Me.chartStock.Text = "chartStock"
        Title2.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Title2.Name = "Title2"
        Title2.Text = "Stock Distribution by Category (Qty)"
        Me.chartStock.Titles.Add(Title2)
        '
        'pnlCreditDetails
        '
        Me.pnlCreditDetails.BackColor = System.Drawing.Color.White
        Me.pnlCreditDetails.Controls.Add(Me.tabsManagement)
        Me.pnlCreditDetails.Controls.Add(Me.headerPanel)
        Me.pnlCreditDetails.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlCreditDetails.Location = New System.Drawing.Point(0, 774)
        Me.pnlCreditDetails.Margin = New System.Windows.Forms.Padding(6, 3, 6, 3)
        Me.pnlCreditDetails.Name = "pnlCreditDetails"
        Me.pnlCreditDetails.Size = New System.Drawing.Size(2244, 333)
        Me.pnlCreditDetails.TabIndex = 2
        '
        'tabsManagement
        '
        Me.tabsManagement.Controls.Add(Me.tabPageCustomerCredits)
        Me.tabsManagement.Controls.Add(Me.tabPageBlockedCustomers)
        Me.tabsManagement.Controls.Add(Me.tabPageSupplierCredits)
        Me.tabsManagement.Controls.Add(Me.tabPageSupplierAlerts)
        Me.tabsManagement.Controls.Add(Me.tabPageCustomerReturns)
        Me.tabsManagement.Controls.Add(Me.tabPageSupplierReturns)
        Me.tabsManagement.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tabsManagement.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
        Me.tabsManagement.Location = New System.Drawing.Point(0, 78)
        Me.tabsManagement.Margin = New System.Windows.Forms.Padding(6, 3, 6, 3)
        Me.tabsManagement.Name = "tabsManagement"
        Me.tabsManagement.SelectedIndex = 0
        Me.tabsManagement.Size = New System.Drawing.Size(2244, 255)
        Me.tabsManagement.TabIndex = 1
        '
        'tabPageCustomerCredits
        '
        Me.tabPageCustomerCredits.Controls.Add(Me.dgvCustomerCredits)
        Me.tabPageCustomerCredits.Location = New System.Drawing.Point(4, 40)
        Me.tabPageCustomerCredits.Margin = New System.Windows.Forms.Padding(6, 3, 6, 3)
        Me.tabPageCustomerCredits.Name = "tabPageCustomerCredits"
        Me.tabPageCustomerCredits.Padding = New System.Windows.Forms.Padding(6, 3, 6, 3)
        Me.tabPageCustomerCredits.Size = New System.Drawing.Size(2236, 211)
        Me.tabPageCustomerCredits.TabIndex = 0
        Me.tabPageCustomerCredits.Text = "Customer Credits (2M+)"
        Me.tabPageCustomerCredits.UseVisualStyleBackColor = True
        '
        'dgvCustomerCredits
        '
        Me.dgvCustomerCredits.AllowUserToAddRows = False
        Me.dgvCustomerCredits.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvCustomerCredits.BackgroundColor = System.Drawing.Color.White
        Me.dgvCustomerCredits.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvCustomerCredits.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvCustomerCredits.Location = New System.Drawing.Point(6, 3)
        Me.dgvCustomerCredits.Margin = New System.Windows.Forms.Padding(6, 3, 6, 3)
        Me.dgvCustomerCredits.Name = "dgvCustomerCredits"
        Me.dgvCustomerCredits.ReadOnly = True
        Me.dgvCustomerCredits.RowHeadersVisible = False
        Me.dgvCustomerCredits.RowHeadersWidth = 51
        Me.dgvCustomerCredits.Size = New System.Drawing.Size(2224, 205)
        Me.dgvCustomerCredits.TabIndex = 0
        '
        'tabPageBlockedCustomers
        '
        Me.tabPageBlockedCustomers.Controls.Add(Me.dgvBlockedCustomers)
        Me.tabPageBlockedCustomers.Location = New System.Drawing.Point(4, 40)
        Me.tabPageBlockedCustomers.Margin = New System.Windows.Forms.Padding(6, 3, 6, 3)
        Me.tabPageBlockedCustomers.Name = "tabPageBlockedCustomers"
        Me.tabPageBlockedCustomers.Padding = New System.Windows.Forms.Padding(6, 3, 6, 3)
        Me.tabPageBlockedCustomers.Size = New System.Drawing.Size(2309, 211)
        Me.tabPageBlockedCustomers.TabIndex = 1
        Me.tabPageBlockedCustomers.Text = "Blocked Customers"
        Me.tabPageBlockedCustomers.UseVisualStyleBackColor = True
        '
        'dgvBlockedCustomers
        '
        Me.dgvBlockedCustomers.AllowUserToAddRows = False
        Me.dgvBlockedCustomers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvBlockedCustomers.BackgroundColor = System.Drawing.Color.White
        Me.dgvBlockedCustomers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvBlockedCustomers.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvBlockedCustomers.Location = New System.Drawing.Point(6, 3)
        Me.dgvBlockedCustomers.Margin = New System.Windows.Forms.Padding(6, 3, 6, 3)
        Me.dgvBlockedCustomers.Name = "dgvBlockedCustomers"
        Me.dgvBlockedCustomers.ReadOnly = True
        Me.dgvBlockedCustomers.RowHeadersVisible = False
        Me.dgvBlockedCustomers.RowHeadersWidth = 51
        Me.dgvBlockedCustomers.Size = New System.Drawing.Size(2297, 205)
        Me.dgvBlockedCustomers.TabIndex = 0
        '
        'tabPageSupplierCredits
        '
        Me.tabPageSupplierCredits.Controls.Add(Me.dgvSupplierCredits)
        Me.tabPageSupplierCredits.Location = New System.Drawing.Point(4, 40)
        Me.tabPageSupplierCredits.Margin = New System.Windows.Forms.Padding(6, 3, 6, 3)
        Me.tabPageSupplierCredits.Name = "tabPageSupplierCredits"
        Me.tabPageSupplierCredits.Padding = New System.Windows.Forms.Padding(6, 3, 6, 3)
        Me.tabPageSupplierCredits.Size = New System.Drawing.Size(2309, 211)
        Me.tabPageSupplierCredits.TabIndex = 2
        Me.tabPageSupplierCredits.Text = "Supplier Credits"
        Me.tabPageSupplierCredits.UseVisualStyleBackColor = True
        '
        'dgvSupplierCredits
        '
        Me.dgvSupplierCredits.AllowUserToAddRows = False
        Me.dgvSupplierCredits.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvSupplierCredits.BackgroundColor = System.Drawing.Color.White
        Me.dgvSupplierCredits.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvSupplierCredits.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvSupplierCredits.Location = New System.Drawing.Point(6, 3)
        Me.dgvSupplierCredits.Margin = New System.Windows.Forms.Padding(6, 3, 6, 3)
        Me.dgvSupplierCredits.Name = "dgvSupplierCredits"
        Me.dgvSupplierCredits.ReadOnly = True
        Me.dgvSupplierCredits.RowHeadersVisible = False
        Me.dgvSupplierCredits.RowHeadersWidth = 51
        Me.dgvSupplierCredits.Size = New System.Drawing.Size(2297, 205)
        Me.dgvSupplierCredits.TabIndex = 0
        '
        'tabPageSupplierAlerts
        '
        Me.tabPageSupplierAlerts.Controls.Add(Me.dgvSupplierAlerts)
        Me.tabPageSupplierAlerts.Location = New System.Drawing.Point(4, 40)
        Me.tabPageSupplierAlerts.Margin = New System.Windows.Forms.Padding(6, 3, 6, 3)
        Me.tabPageSupplierAlerts.Name = "tabPageSupplierAlerts"
        Me.tabPageSupplierAlerts.Padding = New System.Windows.Forms.Padding(6, 3, 6, 3)
        Me.tabPageSupplierAlerts.Size = New System.Drawing.Size(2309, 211)
        Me.tabPageSupplierAlerts.TabIndex = 3
        Me.tabPageSupplierAlerts.Text = "Supplier Alerts (10D)"
        Me.tabPageSupplierAlerts.UseVisualStyleBackColor = True
        '
        'dgvSupplierAlerts
        '
        Me.dgvSupplierAlerts.AllowUserToAddRows = False
        Me.dgvSupplierAlerts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvSupplierAlerts.BackgroundColor = System.Drawing.Color.White
        Me.dgvSupplierAlerts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvSupplierAlerts.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvSupplierAlerts.Location = New System.Drawing.Point(6, 3)
        Me.dgvSupplierAlerts.Margin = New System.Windows.Forms.Padding(6, 3, 6, 3)
        Me.dgvSupplierAlerts.Name = "dgvSupplierAlerts"
        Me.dgvSupplierAlerts.ReadOnly = True
        Me.dgvSupplierAlerts.RowHeadersVisible = False
        Me.dgvSupplierAlerts.RowHeadersWidth = 51
        Me.dgvSupplierAlerts.Size = New System.Drawing.Size(2297, 205)
        Me.dgvSupplierAlerts.TabIndex = 0
        '
        'tabPageCustomerReturns
        '
        Me.tabPageCustomerReturns.Controls.Add(Me.dgvCustomerReturnCheques)
        Me.tabPageCustomerReturns.Location = New System.Drawing.Point(4, 40)
        Me.tabPageCustomerReturns.Margin = New System.Windows.Forms.Padding(6, 3, 6, 3)
        Me.tabPageCustomerReturns.Name = "tabPageCustomerReturns"
        Me.tabPageCustomerReturns.Padding = New System.Windows.Forms.Padding(6, 3, 6, 3)
        Me.tabPageCustomerReturns.Size = New System.Drawing.Size(2309, 211)
        Me.tabPageCustomerReturns.TabIndex = 4
        Me.tabPageCustomerReturns.Text = "Customer Cheque Return"
        Me.tabPageCustomerReturns.UseVisualStyleBackColor = True
        '
        'dgvCustomerReturnCheques
        '
        Me.dgvCustomerReturnCheques.AllowUserToAddRows = False
        Me.dgvCustomerReturnCheques.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvCustomerReturnCheques.BackgroundColor = System.Drawing.Color.White
        Me.dgvCustomerReturnCheques.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvCustomerReturnCheques.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvCustomerReturnCheques.Location = New System.Drawing.Point(6, 3)
        Me.dgvCustomerReturnCheques.Margin = New System.Windows.Forms.Padding(6, 3, 6, 3)
        Me.dgvCustomerReturnCheques.Name = "dgvCustomerReturnCheques"
        Me.dgvCustomerReturnCheques.ReadOnly = True
        Me.dgvCustomerReturnCheques.RowHeadersVisible = False
        Me.dgvCustomerReturnCheques.RowHeadersWidth = 51
        Me.dgvCustomerReturnCheques.Size = New System.Drawing.Size(2297, 205)
        Me.dgvCustomerReturnCheques.TabIndex = 0
        '
        'tabPageSupplierReturns
        '
        Me.tabPageSupplierReturns.Controls.Add(Me.dgvSupplierReturnCheques)
        Me.tabPageSupplierReturns.Location = New System.Drawing.Point(4, 40)
        Me.tabPageSupplierReturns.Margin = New System.Windows.Forms.Padding(6, 3, 6, 3)
        Me.tabPageSupplierReturns.Name = "tabPageSupplierReturns"
        Me.tabPageSupplierReturns.Padding = New System.Windows.Forms.Padding(6, 3, 6, 3)
        Me.tabPageSupplierReturns.Size = New System.Drawing.Size(2309, 211)
        Me.tabPageSupplierReturns.TabIndex = 5
        Me.tabPageSupplierReturns.Text = "Supplier cheque return"
        Me.tabPageSupplierReturns.UseVisualStyleBackColor = True
        '
        'dgvSupplierReturnCheques
        '
        Me.dgvSupplierReturnCheques.AllowUserToAddRows = False
        Me.dgvSupplierReturnCheques.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvSupplierReturnCheques.BackgroundColor = System.Drawing.Color.White
        Me.dgvSupplierReturnCheques.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvSupplierReturnCheques.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvSupplierReturnCheques.Location = New System.Drawing.Point(6, 3)
        Me.dgvSupplierReturnCheques.Margin = New System.Windows.Forms.Padding(6, 3, 6, 3)
        Me.dgvSupplierReturnCheques.Name = "dgvSupplierReturnCheques"
        Me.dgvSupplierReturnCheques.ReadOnly = True
        Me.dgvSupplierReturnCheques.RowHeadersVisible = False
        Me.dgvSupplierReturnCheques.RowHeadersWidth = 51
        Me.dgvSupplierReturnCheques.Size = New System.Drawing.Size(2297, 205)
        Me.dgvSupplierReturnCheques.TabIndex = 0
        '
        'headerPanel
        '
        Me.headerPanel.BackColor = System.Drawing.Color.FromArgb(CType(CType(231, Byte), Integer), CType(CType(76, Byte), Integer), CType(CType(60, Byte), Integer))
        Me.headerPanel.Controls.Add(Me.lblCreditTitle)
        Me.headerPanel.Dock = System.Windows.Forms.DockStyle.Top
        Me.headerPanel.Location = New System.Drawing.Point(0, 0)
        Me.headerPanel.Margin = New System.Windows.Forms.Padding(6, 3, 6, 3)
        Me.headerPanel.Name = "headerPanel"
        Me.headerPanel.Size = New System.Drawing.Size(2244, 78)
        Me.headerPanel.TabIndex = 0
        '
        'lblCreditTitle
        '
        Me.lblCreditTitle.AutoSize = True
        Me.lblCreditTitle.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblCreditTitle.ForeColor = System.Drawing.Color.White
        Me.lblCreditTitle.Location = New System.Drawing.Point(21, 18)
        Me.lblCreditTitle.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.lblCreditTitle.Name = "lblCreditTitle"
        Me.lblCreditTitle.Size = New System.Drawing.Size(603, 38)
        Me.lblCreditTitle.TabIndex = 0
        Me.lblCreditTitle.Text = "MANAGEMENT & FINANCIAL NOTIFICATIONS"
        '
        'Home
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(11.0!, 24.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(44, Byte), Integer), CType(CType(62, Byte), Integer), CType(CType(80, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(2244, 1107)
        Me.Controls.Add(Me.flowCharts)
        Me.Controls.Add(Me.pnlHeader)
        Me.Controls.Add(Me.pnlCreditDetails)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(6, 3, 6, 3)
        Me.Name = "Home"
        Me.Text = "Business Overview Dashboard"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.pnlHeader.ResumeLayout(False)
        Me.pnlHeader.PerformLayout()
        Me.flowCards.ResumeLayout(False)
        Me.pnlSalesCard.ResumeLayout(False)
        Me.pnlSalesCard.PerformLayout()
        Me.pnlPaidCard.ResumeLayout(False)
        Me.pnlPaidCard.PerformLayout()
        Me.pnlStockCard.ResumeLayout(False)
        Me.pnlStockCard.PerformLayout()
        Me.flowCharts.ResumeLayout(False)
        CType(Me.chartSales, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.chartStock, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlCreditDetails.ResumeLayout(False)
        Me.tabsManagement.ResumeLayout(False)
        Me.tabPageCustomerCredits.ResumeLayout(False)
        CType(Me.dgvCustomerCredits, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabPageBlockedCustomers.ResumeLayout(False)
        CType(Me.dgvBlockedCustomers, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabPageSupplierCredits.ResumeLayout(False)
        CType(Me.dgvSupplierCredits, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabPageSupplierAlerts.ResumeLayout(False)
        CType(Me.dgvSupplierAlerts, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabPageCustomerReturns.ResumeLayout(False)
        CType(Me.dgvCustomerReturnCheques, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabPageSupplierReturns.ResumeLayout(False)
        CType(Me.dgvSupplierReturnCheques, System.ComponentModel.ISupportInitialize).EndInit()
        Me.headerPanel.ResumeLayout(False)
        Me.headerPanel.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents pnlHeader As Panel
    Friend WithEvents lblUserDisplay As Label
    Friend WithEvents lblDashTitle As Label
    Friend WithEvents flowCards As FlowLayoutPanel
    Friend WithEvents pnlSalesCard As Panel
    Friend WithEvents lblTodaySales As Label
    Friend WithEvents lblTodaySalesTitle As Label
    Friend WithEvents pnlPaidCard As Panel
    Friend WithEvents lblTodayPaid As Label
    Friend WithEvents lblTodayPaidTitle As Label
    Friend WithEvents pnlStockCard As Panel
    Friend WithEvents lblLowStockCount As Label
    Friend WithEvents lblLowStockTitle As Label
    Friend WithEvents flowCharts As FlowLayoutPanel
    Friend WithEvents chartSales As System.Windows.Forms.DataVisualization.Charting.Chart
    Friend WithEvents chartStock As System.Windows.Forms.DataVisualization.Charting.Chart
    Friend WithEvents pnlCreditDetails As Panel
    Friend WithEvents headerPanel As Panel
    Friend WithEvents lblCreditTitle As Label
    Friend WithEvents tabsManagement As TabControl
    Friend WithEvents tabPageCustomerCredits As TabPage
    Friend WithEvents dgvCustomerCredits As DataGridView
    Friend WithEvents tabPageBlockedCustomers As TabPage
    Friend WithEvents dgvBlockedCustomers As DataGridView
    Friend WithEvents tabPageSupplierCredits As TabPage
    Friend WithEvents dgvSupplierCredits As DataGridView
    Friend WithEvents tabPageSupplierAlerts As TabPage
    Friend WithEvents dgvSupplierAlerts As DataGridView
    Friend WithEvents tabPageCustomerReturns As TabPage
    Friend WithEvents dgvCustomerReturnCheques As DataGridView
    Friend WithEvents tabPageSupplierReturns As TabPage
    Friend WithEvents dgvSupplierReturnCheques As DataGridView
    Friend WithEvents GroupBox1 As GroupBox
End Class
