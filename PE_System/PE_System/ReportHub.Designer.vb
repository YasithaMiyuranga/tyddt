Option Explicit On
Option Strict Off

Imports System
Imports System.Windows.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ReportHub
    Inherits System.Windows.Forms.Form

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

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.pnlHeader = New System.Windows.Forms.Panel()
        Me.lblSubtitle = New System.Windows.Forms.Label()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.flowMain = New System.Windows.Forms.FlowLayoutPanel()
        Me.pnlSalesCard = New System.Windows.Forms.Panel()
        Me.btnStockReturn = New System.Windows.Forms.Button()
        Me.btnMonthlyItemSales = New System.Windows.Forms.Button()
        Me.btnMonthlySales = New System.Windows.Forms.Button()
        Me.btnDailySales = New System.Windows.Forms.Button()
        Me.lblSalesTitle = New System.Windows.Forms.Label()
        Me.pnlStockCard = New System.Windows.Forms.Panel()
        Me.btnCurrentStock = New System.Windows.Forms.Button()
        Me.lblStockTitle = New System.Windows.Forms.Label()
        Me.pnlCustomerCard = New System.Windows.Forms.Panel()
        Me.btnFullCreditReport = New System.Windows.Forms.Button()
        Me.btnCustomerPaymentNote = New System.Windows.Forms.Button()
        Me.btnCustomerCheque = New System.Windows.Forms.Button()
        Me.btnCustomerCredit = New System.Windows.Forms.Button()
        Me.btnCustomerCreditByCity = New System.Windows.Forms.Button()
        Me.btnCustomerList = New System.Windows.Forms.Button()
        Me.btnFullChequeReport = New System.Windows.Forms.Button()
        Me.lblCustomerTitle = New System.Windows.Forms.Label()
        Me.pnlSupplierCard = New System.Windows.Forms.Panel()
        Me.btnPurchaseRequest = New System.Windows.Forms.Button()
        Me.btnPurchaseHistory = New System.Windows.Forms.Button()
        Me.btnSupplierPayment = New System.Windows.Forms.Button()
        Me.btnSupplierCheque = New System.Windows.Forms.Button()
        Me.btnSupplierDebit = New System.Windows.Forms.Button()
        Me.btnSupplierList = New System.Windows.Forms.Button()
        Me.btnFullDebitReport = New System.Windows.Forms.Button()
        Me.btnCashSalesCashOnly = New System.Windows.Forms.Button()
        Me.btnCashSalesOnline = New System.Windows.Forms.Button()
        Me.lblSupplierTitle = New System.Windows.Forms.Label()
        Me.pnlInvoiceCard = New System.Windows.Forms.Panel()
        Me.btnQuatePOS = New System.Windows.Forms.Button()
        Me.btnPurchaseReturnHistory = New System.Windows.Forms.Button()
        Me.btnQuotation = New System.Windows.Forms.Button()
        Me.btnPurchaseInvoice = New System.Windows.Forms.Button()
        Me.btnSaleReturnInv = New System.Windows.Forms.Button()
        Me.btnSaleInvPOS = New System.Windows.Forms.Button()
        Me.btnSaleInvA4 = New System.Windows.Forms.Button()
        Me.btnBillDetails = New System.Windows.Forms.Button()
        Me.lblInvoiceTitle = New System.Windows.Forms.Label()
        Me.pnlDailyCard = New System.Windows.Forms.Panel()
        Me.btnFullDaySummary = New System.Windows.Forms.Button()
        Me.btnQtBillsDaily = New System.Windows.Forms.Button()
        Me.btnVatBillsDaily = New System.Windows.Forms.Button()
        Me.btnElBillsDaily = New System.Windows.Forms.Button()
        Me.btnGrBillsDaily = New System.Windows.Forms.Button()
        Me.btnCreditBillsDaily = New System.Windows.Forms.Button()
        Me.btnCashBillsDaily = New System.Windows.Forms.Button()
        Me.btnCashOnlineBillsDaily = New System.Windows.Forms.Button()
        Me.lblDailyTitle = New System.Windows.Forms.Label()
        Me.pnlHeader.SuspendLayout()
        Me.flowMain.SuspendLayout()
        Me.pnlSalesCard.SuspendLayout()
        Me.pnlStockCard.SuspendLayout()
        Me.pnlCustomerCard.SuspendLayout()
        Me.pnlSupplierCard.SuspendLayout()
        Me.pnlInvoiceCard.SuspendLayout()
        Me.pnlDailyCard.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlHeader
        '
        Me.pnlHeader.BackColor = System.Drawing.Color.FromArgb(CType(CType(44, Byte), Integer), CType(CType(62, Byte), Integer), CType(CType(80, Byte), Integer))
        Me.pnlHeader.Controls.Add(Me.lblSubtitle)
        Me.pnlHeader.Controls.Add(Me.lblTitle)
        Me.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlHeader.Location = New System.Drawing.Point(0, 0)
        Me.pnlHeader.Name = "pnlHeader"
        Me.pnlHeader.Size = New System.Drawing.Size(1200, 120)
        Me.pnlHeader.TabIndex = 0
        '
        'lblSubtitle
        '
        Me.lblSubtitle.AutoSize = True
        Me.lblSubtitle.Font = New System.Drawing.Font("Segoe UI", 11.0!)
        Me.lblSubtitle.ForeColor = System.Drawing.Color.LightGray
        Me.lblSubtitle.Location = New System.Drawing.Point(30, 75)
        Me.lblSubtitle.Name = "lblSubtitle"
        Me.lblSubtitle.Size = New System.Drawing.Size(352, 25)
        Me.lblSubtitle.TabIndex = 1
        Me.lblSubtitle.Text = "Access all system reports from one place"
        '
        'lblTitle
        '
        Me.lblTitle.AutoSize = True
        Me.lblTitle.Font = New System.Drawing.Font("Segoe UI", 26.0!, System.Drawing.FontStyle.Bold)
        Me.lblTitle.ForeColor = System.Drawing.Color.White
        Me.lblTitle.Location = New System.Drawing.Point(20, 15)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(429, 60)
        Me.lblTitle.TabIndex = 0
        Me.lblTitle.Text = "Central Report Hub"
        '
        'flowMain
        '
        Me.flowMain.AutoScroll = True
        Me.flowMain.BackColor = System.Drawing.Color.FromArgb(CType(CType(236, Byte), Integer), CType(CType(240, Byte), Integer), CType(CType(241, Byte), Integer))
        Me.flowMain.Controls.Add(Me.pnlSalesCard)
        Me.flowMain.Controls.Add(Me.pnlStockCard)
        Me.flowMain.Controls.Add(Me.pnlCustomerCard)
        Me.flowMain.Controls.Add(Me.pnlSupplierCard)
        Me.flowMain.Controls.Add(Me.pnlInvoiceCard)
        Me.flowMain.Controls.Add(Me.pnlDailyCard)
        Me.flowMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.flowMain.Location = New System.Drawing.Point(0, 120)
        Me.flowMain.Name = "flowMain"
        Me.flowMain.Padding = New System.Windows.Forms.Padding(30)
        Me.flowMain.Size = New System.Drawing.Size(1200, 837)
        Me.flowMain.TabIndex = 1
        '
        'pnlSalesCard
        '
        Me.pnlSalesCard.BackColor = System.Drawing.Color.White
        Me.pnlSalesCard.Controls.Add(Me.btnStockReturn)
        Me.pnlSalesCard.Controls.Add(Me.btnMonthlyItemSales)
        Me.pnlSalesCard.Controls.Add(Me.btnMonthlySales)
        Me.pnlSalesCard.Controls.Add(Me.btnDailySales)
        Me.pnlSalesCard.Controls.Add(Me.lblSalesTitle)
        Me.pnlSalesCard.Location = New System.Drawing.Point(40, 40)
        Me.pnlSalesCard.Margin = New System.Windows.Forms.Padding(10)
        Me.pnlSalesCard.Name = "pnlSalesCard"
        Me.pnlSalesCard.Size = New System.Drawing.Size(360, 320)
        Me.pnlSalesCard.TabIndex = 0
        '
        'btnStockReturn
        '
        Me.btnStockReturn.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnStockReturn.FlatAppearance.BorderSize = 0
        Me.btnStockReturn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(233, Byte), Integer), CType(CType(236, Byte), Integer), CType(CType(239, Byte), Integer))
        Me.btnStockReturn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnStockReturn.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnStockReturn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnStockReturn.Location = New System.Drawing.Point(15, 255)
        Me.btnStockReturn.Name = "btnStockReturn"
        Me.btnStockReturn.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnStockReturn.Size = New System.Drawing.Size(330, 45)
        Me.btnStockReturn.TabIndex = 4
        Me.btnStockReturn.Text = "  🔄 Stock Return Report"
        Me.btnStockReturn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnStockReturn.UseVisualStyleBackColor = False
        Me.btnStockReturn.Visible = False
        '
        'btnMonthlyItemSales
        '
        Me.btnMonthlyItemSales.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnMonthlyItemSales.FlatAppearance.BorderSize = 0
        Me.btnMonthlyItemSales.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(233, Byte), Integer), CType(CType(236, Byte), Integer), CType(CType(239, Byte), Integer))
        Me.btnMonthlyItemSales.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnMonthlyItemSales.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnMonthlyItemSales.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnMonthlyItemSales.Location = New System.Drawing.Point(15, 195)
        Me.btnMonthlyItemSales.Name = "btnMonthlyItemSales"
        Me.btnMonthlyItemSales.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnMonthlyItemSales.Size = New System.Drawing.Size(330, 45)
        Me.btnMonthlyItemSales.TabIndex = 3
        Me.btnMonthlyItemSales.Text = "  📦 Monthly Item Sales"
        Me.btnMonthlyItemSales.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnMonthlyItemSales.UseVisualStyleBackColor = False
        '
        'btnMonthlySales
        '
        Me.btnMonthlySales.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnMonthlySales.FlatAppearance.BorderSize = 0
        Me.btnMonthlySales.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(233, Byte), Integer), CType(CType(236, Byte), Integer), CType(CType(239, Byte), Integer))
        Me.btnMonthlySales.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnMonthlySales.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnMonthlySales.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnMonthlySales.Location = New System.Drawing.Point(15, 135)
        Me.btnMonthlySales.Name = "btnMonthlySales"
        Me.btnMonthlySales.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnMonthlySales.Size = New System.Drawing.Size(330, 45)
        Me.btnMonthlySales.TabIndex = 2
        Me.btnMonthlySales.Text = "  📊 Monthly Sales Summary"
        Me.btnMonthlySales.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnMonthlySales.UseVisualStyleBackColor = False
        '
        'btnDailySales
        '
        Me.btnDailySales.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnDailySales.FlatAppearance.BorderSize = 0
        Me.btnDailySales.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(233, Byte), Integer), CType(CType(236, Byte), Integer), CType(CType(239, Byte), Integer))
        Me.btnDailySales.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnDailySales.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnDailySales.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnDailySales.Location = New System.Drawing.Point(15, 75)
        Me.btnDailySales.Name = "btnDailySales"
        Me.btnDailySales.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnDailySales.Size = New System.Drawing.Size(330, 45)
        Me.btnDailySales.TabIndex = 1
        Me.btnDailySales.Text = "  📅 Daily Sales Detailed"
        Me.btnDailySales.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnDailySales.UseVisualStyleBackColor = False
        '
        'lblSalesTitle
        '
        Me.lblSalesTitle.AutoSize = True
        Me.lblSalesTitle.Font = New System.Drawing.Font("Segoe UI", 14.0!, System.Drawing.FontStyle.Bold)
        Me.lblSalesTitle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(46, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(113, Byte), Integer))
        Me.lblSalesTitle.Location = New System.Drawing.Point(15, 20)
        Me.lblSalesTitle.Name = "lblSalesTitle"
        Me.lblSalesTitle.Size = New System.Drawing.Size(168, 32)
        Me.lblSalesTitle.TabIndex = 0
        Me.lblSalesTitle.Text = "Sales Reports"
        '
        'pnlStockCard
        '
        Me.pnlStockCard.BackColor = System.Drawing.Color.White
        Me.pnlStockCard.Controls.Add(Me.btnCurrentStock)
        Me.pnlStockCard.Controls.Add(Me.lblStockTitle)
        Me.pnlStockCard.Location = New System.Drawing.Point(420, 40)
        Me.pnlStockCard.Margin = New System.Windows.Forms.Padding(10)
        Me.pnlStockCard.Name = "pnlStockCard"
        Me.pnlStockCard.Size = New System.Drawing.Size(360, 320)
        Me.pnlStockCard.TabIndex = 1
        '
        'btnCurrentStock
        '
        Me.btnCurrentStock.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnCurrentStock.FlatAppearance.BorderSize = 0
        Me.btnCurrentStock.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(233, Byte), Integer), CType(CType(236, Byte), Integer), CType(CType(239, Byte), Integer))
        Me.btnCurrentStock.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCurrentStock.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnCurrentStock.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCurrentStock.Location = New System.Drawing.Point(15, 75)
        Me.btnCurrentStock.Name = "btnCurrentStock"
        Me.btnCurrentStock.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnCurrentStock.Size = New System.Drawing.Size(330, 45)
        Me.btnCurrentStock.TabIndex = 1
        Me.btnCurrentStock.Text = "  📦 Current Stock List"
        Me.btnCurrentStock.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCurrentStock.UseVisualStyleBackColor = False
        '
        'lblStockTitle
        '
        Me.lblStockTitle.AutoSize = True
        Me.lblStockTitle.Font = New System.Drawing.Font("Segoe UI", 14.0!, System.Drawing.FontStyle.Bold)
        Me.lblStockTitle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(231, Byte), Integer), CType(CType(76, Byte), Integer), CType(CType(60, Byte), Integer))
        Me.lblStockTitle.Location = New System.Drawing.Point(15, 20)
        Me.lblStockTitle.Name = "lblStockTitle"
        Me.lblStockTitle.Size = New System.Drawing.Size(172, 32)
        Me.lblStockTitle.TabIndex = 0
        Me.lblStockTitle.Text = "Stock Reports"
        '
        'pnlCustomerCard
        '
        Me.pnlCustomerCard.BackColor = System.Drawing.Color.White
        Me.pnlCustomerCard.Controls.Add(Me.btnFullCreditReport)
        Me.pnlCustomerCard.Controls.Add(Me.btnCustomerPaymentNote)
        Me.pnlCustomerCard.Controls.Add(Me.btnCustomerCheque)
        Me.pnlCustomerCard.Controls.Add(Me.btnCustomerCredit)
        Me.pnlCustomerCard.Controls.Add(Me.btnCustomerCreditByCity)
        Me.pnlCustomerCard.Controls.Add(Me.btnCustomerList)
        Me.pnlCustomerCard.Controls.Add(Me.btnFullChequeReport)
        Me.pnlCustomerCard.Controls.Add(Me.lblCustomerTitle)
        Me.pnlCustomerCard.Location = New System.Drawing.Point(40, 380)
        Me.pnlCustomerCard.Margin = New System.Windows.Forms.Padding(10)
        Me.pnlCustomerCard.Name = "pnlCustomerCard"
        Me.pnlCustomerCard.Size = New System.Drawing.Size(360, 440)
        Me.pnlCustomerCard.TabIndex = 0
        '
        'btnFullCreditReport
        '
        Me.btnFullCreditReport.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnFullCreditReport.FlatAppearance.BorderSize = 0
        Me.btnFullCreditReport.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(233, Byte), Integer), CType(CType(236, Byte), Integer), CType(CType(239, Byte), Integer))
        Me.btnFullCreditReport.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnFullCreditReport.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnFullCreditReport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnFullCreditReport.Location = New System.Drawing.Point(15, 315)
        Me.btnFullCreditReport.Name = "btnFullCreditReport"
        Me.btnFullCreditReport.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnFullCreditReport.Size = New System.Drawing.Size(330, 45)
        Me.btnFullCreditReport.TabIndex = 5
        Me.btnFullCreditReport.Text = "  💳 Full Credit Report"
        Me.btnFullCreditReport.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnFullCreditReport.UseVisualStyleBackColor = False
        '
        'btnCustomerPaymentNote
        '
        Me.btnCustomerPaymentNote.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnCustomerPaymentNote.FlatAppearance.BorderSize = 0
        Me.btnCustomerPaymentNote.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(233, Byte), Integer), CType(CType(236, Byte), Integer), CType(CType(239, Byte), Integer))
        Me.btnCustomerPaymentNote.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCustomerPaymentNote.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnCustomerPaymentNote.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCustomerPaymentNote.Location = New System.Drawing.Point(15, 255)
        Me.btnCustomerPaymentNote.Name = "btnCustomerPaymentNote"
        Me.btnCustomerPaymentNote.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnCustomerPaymentNote.Size = New System.Drawing.Size(330, 45)
        Me.btnCustomerPaymentNote.TabIndex = 4
        Me.btnCustomerPaymentNote.Text = "  📝 Customer Payment Note"
        Me.btnCustomerPaymentNote.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCustomerPaymentNote.UseVisualStyleBackColor = False
        '
        'btnCustomerCheque
        '
        Me.btnCustomerCheque.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnCustomerCheque.FlatAppearance.BorderSize = 0
        Me.btnCustomerCheque.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(233, Byte), Integer), CType(CType(236, Byte), Integer), CType(CType(239, Byte), Integer))
        Me.btnCustomerCheque.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCustomerCheque.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnCustomerCheque.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCustomerCheque.Location = New System.Drawing.Point(15, 195)
        Me.btnCustomerCheque.Name = "btnCustomerCheque"
        Me.btnCustomerCheque.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnCustomerCheque.Size = New System.Drawing.Size(330, 45)
        Me.btnCustomerCheque.TabIndex = 3
        Me.btnCustomerCheque.Text = "  🏦 Customer Cheque Log"
        Me.btnCustomerCheque.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCustomerCheque.UseVisualStyleBackColor = False
        '
        'btnCustomerCredit
        '
        Me.btnCustomerCredit.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnCustomerCredit.FlatAppearance.BorderSize = 0
        Me.btnCustomerCredit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(233, Byte), Integer), CType(CType(236, Byte), Integer), CType(CType(239, Byte), Integer))
        Me.btnCustomerCredit.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCustomerCredit.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnCustomerCredit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCustomerCredit.Location = New System.Drawing.Point(15, 135)
        Me.btnCustomerCredit.Name = "btnCustomerCredit"
        Me.btnCustomerCredit.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnCustomerCredit.Size = New System.Drawing.Size(330, 45)
        Me.btnCustomerCredit.TabIndex = 2
        Me.btnCustomerCredit.Text = "  💳 Customer Credit History"
        Me.btnCustomerCredit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCustomerCredit.UseVisualStyleBackColor = False
        '
        'btnCustomerCreditByCity
        '
        Me.btnCustomerCreditByCity.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnCustomerCreditByCity.FlatAppearance.BorderSize = 0
        Me.btnCustomerCreditByCity.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(233, Byte), Integer), CType(CType(236, Byte), Integer), CType(CType(239, Byte), Integer))
        Me.btnCustomerCreditByCity.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCustomerCreditByCity.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnCustomerCreditByCity.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCustomerCreditByCity.Location = New System.Drawing.Point(15, 185)
        Me.btnCustomerCreditByCity.Name = "btnCustomerCreditByCity"
        Me.btnCustomerCreditByCity.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnCustomerCreditByCity.Size = New System.Drawing.Size(330, 45)
        Me.btnCustomerCreditByCity.TabIndex = 6
        Me.btnCustomerCreditByCity.Text = "  🌆 Customer Credit By City"
        Me.btnCustomerCreditByCity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCustomerCreditByCity.UseVisualStyleBackColor = False
        '
        'btnCustomerList
        '
        Me.btnCustomerList.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnCustomerList.FlatAppearance.BorderSize = 0
        Me.btnCustomerList.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(233, Byte), Integer), CType(CType(236, Byte), Integer), CType(CType(239, Byte), Integer))
        Me.btnCustomerList.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCustomerList.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnCustomerList.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCustomerList.Location = New System.Drawing.Point(15, 75)
        Me.btnCustomerList.Name = "btnCustomerList"
        Me.btnCustomerList.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnCustomerList.Size = New System.Drawing.Size(330, 45)
        Me.btnCustomerList.TabIndex = 3
        Me.btnCustomerList.Text = "  👥 Customer Master List"
        Me.btnCustomerList.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCustomerList.UseVisualStyleBackColor = False
        '
        'btnFullChequeReport
        '
        Me.btnFullChequeReport.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnFullChequeReport.FlatAppearance.BorderSize = 0
        Me.btnFullChequeReport.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(233, Byte), Integer), CType(CType(236, Byte), Integer), CType(CType(239, Byte), Integer))
        Me.btnFullChequeReport.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnFullChequeReport.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnFullChequeReport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnFullChequeReport.Location = New System.Drawing.Point(15, 375)
        Me.btnFullChequeReport.Name = "btnFullChequeReport"
        Me.btnFullChequeReport.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnFullChequeReport.Size = New System.Drawing.Size(330, 45)
        Me.btnFullChequeReport.TabIndex = 6
        Me.btnFullChequeReport.Text = "  🏦 Full Cheque Report"
        Me.btnFullChequeReport.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnFullChequeReport.UseVisualStyleBackColor = False
        '
        'lblCustomerTitle
        '
        Me.lblCustomerTitle.AutoSize = True
        Me.lblCustomerTitle.Font = New System.Drawing.Font("Segoe UI", 14.0!, System.Drawing.FontStyle.Bold)
        Me.lblCustomerTitle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(52, Byte), Integer), CType(CType(152, Byte), Integer), CType(CType(219, Byte), Integer))
        Me.lblCustomerTitle.Location = New System.Drawing.Point(15, 20)
        Me.lblCustomerTitle.Name = "lblCustomerTitle"
        Me.lblCustomerTitle.Size = New System.Drawing.Size(221, 32)
        Me.lblCustomerTitle.TabIndex = 0
        Me.lblCustomerTitle.Text = "Customer Reports"
        '
        'pnlSupplierCard
        '
        Me.pnlSupplierCard.BackColor = System.Drawing.Color.White
        Me.pnlSupplierCard.Controls.Add(Me.btnPurchaseRequest)
        Me.pnlSupplierCard.Controls.Add(Me.btnPurchaseHistory)
        Me.pnlSupplierCard.Controls.Add(Me.btnSupplierPayment)
        Me.pnlSupplierCard.Controls.Add(Me.btnSupplierCheque)
        Me.pnlSupplierCard.Controls.Add(Me.btnSupplierDebit)
        Me.pnlSupplierCard.Controls.Add(Me.btnSupplierList)
        Me.pnlSupplierCard.Controls.Add(Me.btnFullDebitReport)
        Me.pnlSupplierCard.Controls.Add(Me.btnCashSalesCashOnly)
        Me.pnlSupplierCard.Controls.Add(Me.btnCashSalesOnline)
        Me.pnlSupplierCard.Controls.Add(Me.lblSupplierTitle)
        Me.pnlSupplierCard.Location = New System.Drawing.Point(420, 380)
        Me.pnlSupplierCard.Margin = New System.Windows.Forms.Padding(10)
        Me.pnlSupplierCard.Name = "pnlSupplierCard"
        Me.pnlSupplierCard.Size = New System.Drawing.Size(360, 490)
        Me.pnlSupplierCard.TabIndex = 3
        '
        'btnPurchaseRequest
        '
        Me.btnPurchaseRequest.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnPurchaseRequest.FlatAppearance.BorderSize = 0
        Me.btnPurchaseRequest.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(233, Byte), Integer), CType(CType(236, Byte), Integer), CType(CType(239, Byte), Integer))
        Me.btnPurchaseRequest.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnPurchaseRequest.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnPurchaseRequest.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnPurchaseRequest.Location = New System.Drawing.Point(15, 255)
        Me.btnPurchaseRequest.Name = "btnPurchaseRequest"
        Me.btnPurchaseRequest.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnPurchaseRequest.Size = New System.Drawing.Size(330, 45)
        Me.btnPurchaseRequest.TabIndex = 6
        Me.btnPurchaseRequest.Text = "  📝 Purchase Request Report"
        Me.btnPurchaseRequest.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnPurchaseRequest.UseVisualStyleBackColor = False
        '
        'btnPurchaseHistory
        '
        Me.btnPurchaseHistory.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnPurchaseHistory.FlatAppearance.BorderSize = 0
        Me.btnPurchaseHistory.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(233, Byte), Integer), CType(CType(236, Byte), Integer), CType(CType(239, Byte), Integer))
        Me.btnPurchaseHistory.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnPurchaseHistory.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnPurchaseHistory.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnPurchaseHistory.Location = New System.Drawing.Point(15, 255)
        Me.btnPurchaseHistory.Name = "btnPurchaseHistory"
        Me.btnPurchaseHistory.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnPurchaseHistory.Size = New System.Drawing.Size(330, 45)
        Me.btnPurchaseHistory.TabIndex = 4
        Me.btnPurchaseHistory.Text = "  📊 Purchase History Report"
        Me.btnPurchaseHistory.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnPurchaseHistory.UseVisualStyleBackColor = False
        Me.btnPurchaseHistory.Visible = False
        '
        'btnSupplierPayment
        '
        Me.btnSupplierPayment.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnSupplierPayment.FlatAppearance.BorderSize = 0
        Me.btnSupplierPayment.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(233, Byte), Integer), CType(CType(236, Byte), Integer), CType(CType(239, Byte), Integer))
        Me.btnSupplierPayment.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnSupplierPayment.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnSupplierPayment.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSupplierPayment.Location = New System.Drawing.Point(15, 255)
        Me.btnSupplierPayment.Name = "btnSupplierPayment"
        Me.btnSupplierPayment.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnSupplierPayment.Size = New System.Drawing.Size(330, 45)
        Me.btnSupplierPayment.TabIndex = 4
        Me.btnSupplierPayment.Text = "  📝 Supplier Payment Log"
        Me.btnSupplierPayment.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSupplierPayment.UseVisualStyleBackColor = False
        '
        'btnSupplierCheque
        '
        Me.btnSupplierCheque.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnSupplierCheque.FlatAppearance.BorderSize = 0
        Me.btnSupplierCheque.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(233, Byte), Integer), CType(CType(236, Byte), Integer), CType(CType(239, Byte), Integer))
        Me.btnSupplierCheque.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnSupplierCheque.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnSupplierCheque.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSupplierCheque.Location = New System.Drawing.Point(15, 195)
        Me.btnSupplierCheque.Name = "btnSupplierCheque"
        Me.btnSupplierCheque.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnSupplierCheque.Size = New System.Drawing.Size(330, 45)
        Me.btnSupplierCheque.TabIndex = 3
        Me.btnSupplierCheque.Text = "  🏦 Supplier Cheque Log"
        Me.btnSupplierCheque.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSupplierCheque.UseVisualStyleBackColor = False
        '
        'btnSupplierDebit
        '
        Me.btnSupplierDebit.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnSupplierDebit.FlatAppearance.BorderSize = 0
        Me.btnSupplierDebit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(233, Byte), Integer), CType(CType(236, Byte), Integer), CType(CType(239, Byte), Integer))
        Me.btnSupplierDebit.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnSupplierDebit.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnSupplierDebit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSupplierDebit.Location = New System.Drawing.Point(15, 135)
        Me.btnSupplierDebit.Name = "btnSupplierDebit"
        Me.btnSupplierDebit.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnSupplierDebit.Size = New System.Drawing.Size(330, 45)
        Me.btnSupplierDebit.TabIndex = 2
        Me.btnSupplierDebit.Text = "  💳 Supplier Debit History"
        Me.btnSupplierDebit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSupplierDebit.UseVisualStyleBackColor = False
        '
        'btnSupplierList
        '
        Me.btnSupplierList.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnSupplierList.FlatAppearance.BorderSize = 0
        Me.btnSupplierList.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(233, Byte), Integer), CType(CType(236, Byte), Integer), CType(CType(239, Byte), Integer))
        Me.btnSupplierList.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnSupplierList.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnSupplierList.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSupplierList.Location = New System.Drawing.Point(15, 75)
        Me.btnSupplierList.Name = "btnSupplierList"
        Me.btnSupplierList.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnSupplierList.Size = New System.Drawing.Size(330, 45)
        Me.btnSupplierList.TabIndex = 1
        Me.btnSupplierList.Text = "  👥 Supplier Master List"
        Me.btnSupplierList.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSupplierList.UseVisualStyleBackColor = False
        '
        'btnFullDebitReport
        '
        Me.btnFullDebitReport.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnFullDebitReport.FlatAppearance.BorderSize = 0
        Me.btnFullDebitReport.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(233, Byte), Integer), CType(CType(236, Byte), Integer), CType(CType(239, Byte), Integer))
        Me.btnFullDebitReport.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnFullDebitReport.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnFullDebitReport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnFullDebitReport.Location = New System.Drawing.Point(15, 315)
        Me.btnFullDebitReport.Name = "btnFullDebitReport"
        Me.btnFullDebitReport.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnFullDebitReport.Size = New System.Drawing.Size(330, 45)
        Me.btnFullDebitReport.TabIndex = 7
        Me.btnFullDebitReport.Text = "  💳 Full Debit Report"
        Me.btnFullDebitReport.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnFullDebitReport.UseVisualStyleBackColor = False
        '
        'btnCashSalesCashOnly
        '
        Me.btnCashSalesCashOnly.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnCashSalesCashOnly.FlatAppearance.BorderSize = 0
        Me.btnCashSalesCashOnly.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(233, Byte), Integer), CType(CType(236, Byte), Integer), CType(CType(239, Byte), Integer))
        Me.btnCashSalesCashOnly.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCashSalesCashOnly.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnCashSalesCashOnly.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCashSalesCashOnly.Location = New System.Drawing.Point(15, 375)
        Me.btnCashSalesCashOnly.Name = "btnCashSalesCashOnly"
        Me.btnCashSalesCashOnly.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnCashSalesCashOnly.Size = New System.Drawing.Size(330, 45)
        Me.btnCashSalesCashOnly.TabIndex = 8
        Me.btnCashSalesCashOnly.Text = "  💵 Purchase: Cash Bills Only"
        Me.btnCashSalesCashOnly.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCashSalesCashOnly.UseVisualStyleBackColor = False
        '
        'btnCashSalesOnline
        '
        Me.btnCashSalesOnline.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnCashSalesOnline.FlatAppearance.BorderSize = 0
        Me.btnCashSalesOnline.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(233, Byte), Integer), CType(CType(236, Byte), Integer), CType(CType(239, Byte), Integer))
        Me.btnCashSalesOnline.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCashSalesOnline.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnCashSalesOnline.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCashSalesOnline.Location = New System.Drawing.Point(15, 420)
        Me.btnCashSalesOnline.Name = "btnCashSalesOnline"
        Me.btnCashSalesOnline.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnCashSalesOnline.Size = New System.Drawing.Size(330, 45)
        Me.btnCashSalesOnline.TabIndex = 9
        Me.btnCashSalesOnline.Text = "  💳 Purchase: Cards/Online Cash Bills"
        Me.btnCashSalesOnline.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCashSalesOnline.UseVisualStyleBackColor = False
        '
        'lblSupplierTitle
        '
        Me.lblSupplierTitle.AutoSize = True
        Me.lblSupplierTitle.Font = New System.Drawing.Font("Segoe UI", 14.0!, System.Drawing.FontStyle.Bold)
        Me.lblSupplierTitle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(155, Byte), Integer), CType(CType(89, Byte), Integer), CType(CType(182, Byte), Integer))
        Me.lblSupplierTitle.Location = New System.Drawing.Point(15, 20)
        Me.lblSupplierTitle.Name = "lblSupplierTitle"
        Me.lblSupplierTitle.Size = New System.Drawing.Size(206, 32)
        Me.lblSupplierTitle.TabIndex = 0
        Me.lblSupplierTitle.Text = "Supplier Reports"
        '
        'pnlInvoiceCard
        '
        Me.pnlInvoiceCard.BackColor = System.Drawing.Color.White
        Me.pnlInvoiceCard.Controls.Add(Me.btnQuatePOS)
        Me.pnlInvoiceCard.Controls.Add(Me.btnPurchaseReturnHistory)
        Me.pnlInvoiceCard.Controls.Add(Me.btnQuotation)
        Me.pnlInvoiceCard.Controls.Add(Me.btnPurchaseInvoice)
        Me.pnlInvoiceCard.Controls.Add(Me.btnSaleReturnInv)
        Me.pnlInvoiceCard.Controls.Add(Me.btnSaleInvPOS)
        Me.pnlInvoiceCard.Controls.Add(Me.btnSaleInvA4)
        Me.pnlInvoiceCard.Controls.Add(Me.btnBillDetails)
        Me.pnlInvoiceCard.Controls.Add(Me.lblInvoiceTitle)
        Me.pnlInvoiceCard.Location = New System.Drawing.Point(40, 890)
        Me.pnlInvoiceCard.Margin = New System.Windows.Forms.Padding(10)
        Me.pnlInvoiceCard.Name = "pnlInvoiceCard"
        Me.pnlInvoiceCard.Size = New System.Drawing.Size(360, 440)
        Me.pnlInvoiceCard.TabIndex = 4
        '
        'btnQuatePOS
        '
        Me.btnQuatePOS.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnQuatePOS.FlatAppearance.BorderSize = 0
        Me.btnQuatePOS.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(233, Byte), Integer), CType(CType(236, Byte), Integer), CType(CType(239, Byte), Integer))
        Me.btnQuatePOS.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnQuatePOS.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnQuatePOS.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnQuatePOS.Location = New System.Drawing.Point(15, 375)
        Me.btnQuatePOS.Name = "btnQuatePOS"
        Me.btnQuatePOS.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnQuatePOS.Size = New System.Drawing.Size(330, 45)
        Me.btnQuatePOS.TabIndex = 6
        Me.btnQuatePOS.Text = "  🧾 Quotation (POS)"
        Me.btnQuatePOS.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnQuatePOS.UseVisualStyleBackColor = False
        '
        'btnPurchaseReturnHistory
        '
        Me.btnPurchaseReturnHistory.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnPurchaseReturnHistory.FlatAppearance.BorderSize = 0
        Me.btnPurchaseReturnHistory.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(233, Byte), Integer), CType(CType(236, Byte), Integer), CType(CType(239, Byte), Integer))
        Me.btnPurchaseReturnHistory.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnPurchaseReturnHistory.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnPurchaseReturnHistory.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnPurchaseReturnHistory.Location = New System.Drawing.Point(15, 315)
        Me.btnPurchaseReturnHistory.Name = "btnPurchaseReturnHistory"
        Me.btnPurchaseReturnHistory.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnPurchaseReturnHistory.Size = New System.Drawing.Size(330, 45)
        Me.btnPurchaseReturnHistory.TabIndex = 8
        Me.btnPurchaseReturnHistory.Text = "  🔄 Purchase Return History"
        Me.btnPurchaseReturnHistory.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnPurchaseReturnHistory.UseVisualStyleBackColor = False
        '
        'btnQuotation
        '
        Me.btnQuotation.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnQuotation.FlatAppearance.BorderSize = 0
        Me.btnQuotation.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(233, Byte), Integer), CType(CType(236, Byte), Integer), CType(CType(239, Byte), Integer))
        Me.btnQuotation.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnQuotation.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnQuotation.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnQuotation.Location = New System.Drawing.Point(15, 315)
        Me.btnQuotation.Name = "btnQuotation"
        Me.btnQuotation.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnQuotation.Size = New System.Drawing.Size(330, 45)
        Me.btnQuotation.TabIndex = 5
        Me.btnQuotation.Text = "  📝 Quotation Report"
        Me.btnQuotation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnQuotation.UseVisualStyleBackColor = False
        '
        'btnPurchaseInvoice
        '
        Me.btnPurchaseInvoice.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnPurchaseInvoice.FlatAppearance.BorderSize = 0
        Me.btnPurchaseInvoice.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(233, Byte), Integer), CType(CType(236, Byte), Integer), CType(CType(239, Byte), Integer))
        Me.btnPurchaseInvoice.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnPurchaseInvoice.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnPurchaseInvoice.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnPurchaseInvoice.Location = New System.Drawing.Point(15, 255)
        Me.btnPurchaseInvoice.Name = "btnPurchaseInvoice"
        Me.btnPurchaseInvoice.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnPurchaseInvoice.Size = New System.Drawing.Size(330, 45)
        Me.btnPurchaseInvoice.TabIndex = 4
        Me.btnPurchaseInvoice.Text = "  📥 Purchase Invoice Report"
        Me.btnPurchaseInvoice.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnPurchaseInvoice.UseVisualStyleBackColor = False
        '
        'btnSaleReturnInv
        '
        Me.btnSaleReturnInv.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnSaleReturnInv.FlatAppearance.BorderSize = 0
        Me.btnSaleReturnInv.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(233, Byte), Integer), CType(CType(236, Byte), Integer), CType(CType(239, Byte), Integer))
        Me.btnSaleReturnInv.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnSaleReturnInv.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnSaleReturnInv.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSaleReturnInv.Location = New System.Drawing.Point(15, 195)
        Me.btnSaleReturnInv.Name = "btnSaleReturnInv"
        Me.btnSaleReturnInv.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnSaleReturnInv.Size = New System.Drawing.Size(330, 45)
        Me.btnSaleReturnInv.TabIndex = 3
        Me.btnSaleReturnInv.Text = "  🔙 Sale Return Report"
        Me.btnSaleReturnInv.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSaleReturnInv.UseVisualStyleBackColor = False
        '
        'btnSaleInvPOS
        '
        Me.btnSaleInvPOS.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnSaleInvPOS.FlatAppearance.BorderSize = 0
        Me.btnSaleInvPOS.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(233, Byte), Integer), CType(CType(236, Byte), Integer), CType(CType(239, Byte), Integer))
        Me.btnSaleInvPOS.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnSaleInvPOS.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnSaleInvPOS.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSaleInvPOS.Location = New System.Drawing.Point(15, 135)
        Me.btnSaleInvPOS.Name = "btnSaleInvPOS"
        Me.btnSaleInvPOS.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnSaleInvPOS.Size = New System.Drawing.Size(330, 45)
        Me.btnSaleInvPOS.TabIndex = 2
        Me.btnSaleInvPOS.Text = "  🧾 Sale Invoice (POS)"
        Me.btnSaleInvPOS.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSaleInvPOS.UseVisualStyleBackColor = False
        '
        'btnSaleInvA4
        '
        Me.btnSaleInvA4.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnSaleInvA4.FlatAppearance.BorderSize = 0
        Me.btnSaleInvA4.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(233, Byte), Integer), CType(CType(236, Byte), Integer), CType(CType(239, Byte), Integer))
        Me.btnSaleInvA4.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnSaleInvA4.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnSaleInvA4.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSaleInvA4.Location = New System.Drawing.Point(15, 75)
        Me.btnSaleInvA4.Name = "btnSaleInvA4"
        Me.btnSaleInvA4.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnSaleInvA4.Size = New System.Drawing.Size(330, 45)
        Me.btnSaleInvA4.TabIndex = 1
        Me.btnSaleInvA4.Text = "  📄 Sale Invoice (A4)"
        Me.btnSaleInvA4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSaleInvA4.UseVisualStyleBackColor = False
        '
        'btnBillDetails
        '
        Me.btnBillDetails.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnBillDetails.FlatAppearance.BorderSize = 0
        Me.btnBillDetails.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(233, Byte), Integer), CType(CType(236, Byte), Integer), CType(CType(239, Byte), Integer))
        Me.btnBillDetails.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnBillDetails.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnBillDetails.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnBillDetails.Location = New System.Drawing.Point(15, 435)
        Me.btnBillDetails.Name = "btnBillDetails"
        Me.btnBillDetails.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnBillDetails.Size = New System.Drawing.Size(330, 45)
        Me.btnBillDetails.TabIndex = 7
        Me.btnBillDetails.Text = "  🧾 Bill Details Report"
        Me.btnBillDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnBillDetails.UseVisualStyleBackColor = False
        '
        'lblInvoiceTitle
        '
        Me.lblInvoiceTitle.AutoSize = True
        Me.lblInvoiceTitle.Font = New System.Drawing.Font("Segoe UI", 14.0!, System.Drawing.FontStyle.Bold)
        Me.lblInvoiceTitle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(160, Byte), Integer), CType(CType(133, Byte), Integer))
        Me.lblInvoiceTitle.Location = New System.Drawing.Point(15, 20)
        Me.lblInvoiceTitle.Name = "lblInvoiceTitle"
        Me.lblInvoiceTitle.Size = New System.Drawing.Size(244, 32)
        Me.lblInvoiceTitle.TabIndex = 0
        Me.lblInvoiceTitle.Text = "Invoice & Bill Reports"
        '
        'pnlDailyCard
        '
        Me.pnlDailyCard.BackColor = System.Drawing.Color.White
        Me.pnlDailyCard.Controls.Add(Me.btnFullDaySummary)
        Me.pnlDailyCard.Controls.Add(Me.btnQtBillsDaily)
        Me.pnlDailyCard.Controls.Add(Me.btnVatBillsDaily)
        Me.pnlDailyCard.Controls.Add(Me.btnElBillsDaily)
        Me.pnlDailyCard.Controls.Add(Me.btnGrBillsDaily)
        Me.pnlDailyCard.Controls.Add(Me.btnCreditBillsDaily)
        Me.pnlDailyCard.Controls.Add(Me.btnCashBillsDaily)
        Me.pnlDailyCard.Controls.Add(Me.btnCashOnlineBillsDaily)
        Me.pnlDailyCard.Controls.Add(Me.lblDailyTitle)
        Me.pnlDailyCard.Location = New System.Drawing.Point(420, 890)
        Me.pnlDailyCard.Margin = New System.Windows.Forms.Padding(10)
        Me.pnlDailyCard.Name = "pnlDailyCard"
        Me.pnlDailyCard.Size = New System.Drawing.Size(360, 500)
        Me.pnlDailyCard.TabIndex = 5
        '
        'btnFullDaySummary
        '
        Me.btnFullDaySummary.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnFullDaySummary.FlatAppearance.BorderSize = 0
        Me.btnFullDaySummary.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnFullDaySummary.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnFullDaySummary.Location = New System.Drawing.Point(15, 435)
        Me.btnFullDaySummary.Name = "btnFullDaySummary"
        Me.btnFullDaySummary.Size = New System.Drawing.Size(330, 45)
        Me.btnFullDaySummary.TabIndex = 7
        Me.btnFullDaySummary.Text = "  💰 Full Day Summary (Reconciliation)"
        Me.btnFullDaySummary.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnFullDaySummary.UseVisualStyleBackColor = False
        '
        'btnQtBillsDaily
        '
        Me.btnQtBillsDaily.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnQtBillsDaily.FlatAppearance.BorderSize = 0
        Me.btnQtBillsDaily.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnQtBillsDaily.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnQtBillsDaily.Location = New System.Drawing.Point(15, 375)
        Me.btnQtBillsDaily.Name = "btnQtBillsDaily"
        Me.btnQtBillsDaily.Size = New System.Drawing.Size(330, 45)
        Me.btnQtBillsDaily.TabIndex = 6
        Me.btnQtBillsDaily.Text = "  📝 Daily QT Bills"
        Me.btnQtBillsDaily.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnQtBillsDaily.UseVisualStyleBackColor = False
        '
        'btnVatBillsDaily
        '
        Me.btnVatBillsDaily.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnVatBillsDaily.FlatAppearance.BorderSize = 0
        Me.btnVatBillsDaily.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnVatBillsDaily.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnVatBillsDaily.Location = New System.Drawing.Point(15, 315)
        Me.btnVatBillsDaily.Name = "btnVatBillsDaily"
        Me.btnVatBillsDaily.Size = New System.Drawing.Size(330, 45)
        Me.btnVatBillsDaily.TabIndex = 5
        Me.btnVatBillsDaily.Text = "  📋 Daily VAT (VT) Bills"
        Me.btnVatBillsDaily.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnVatBillsDaily.UseVisualStyleBackColor = False
        '
        'btnElBillsDaily
        '
        Me.btnElBillsDaily.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnElBillsDaily.FlatAppearance.BorderSize = 0
        Me.btnElBillsDaily.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnElBillsDaily.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnElBillsDaily.Location = New System.Drawing.Point(15, 255)
        Me.btnElBillsDaily.Name = "btnElBillsDaily"
        Me.btnElBillsDaily.Size = New System.Drawing.Size(330, 45)
        Me.btnElBillsDaily.TabIndex = 4
        Me.btnElBillsDaily.Text = "  ⚡ Daily EL Bills"
        Me.btnElBillsDaily.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnElBillsDaily.UseVisualStyleBackColor = False
        '
        'btnGrBillsDaily
        '
        Me.btnGrBillsDaily.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnGrBillsDaily.FlatAppearance.BorderSize = 0
        Me.btnGrBillsDaily.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnGrBillsDaily.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnGrBillsDaily.Location = New System.Drawing.Point(15, 195)
        Me.btnGrBillsDaily.Name = "btnGrBillsDaily"
        Me.btnGrBillsDaily.Size = New System.Drawing.Size(330, 45)
        Me.btnGrBillsDaily.TabIndex = 3
        Me.btnGrBillsDaily.Text = "  📜 Daily GR Bills"
        Me.btnGrBillsDaily.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnGrBillsDaily.UseVisualStyleBackColor = False
        '
        'btnCreditBillsDaily
        '
        Me.btnCreditBillsDaily.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnCreditBillsDaily.FlatAppearance.BorderSize = 0
        Me.btnCreditBillsDaily.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCreditBillsDaily.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnCreditBillsDaily.Location = New System.Drawing.Point(15, 135)
        Me.btnCreditBillsDaily.Name = "btnCreditBillsDaily"
        Me.btnCreditBillsDaily.Size = New System.Drawing.Size(330, 45)
        Me.btnCreditBillsDaily.TabIndex = 2
        Me.btnCreditBillsDaily.Text = "  💳 Daily Credit Bills"
        Me.btnCreditBillsDaily.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCreditBillsDaily.UseVisualStyleBackColor = False
        '
        'btnCashBillsDaily
        '
        Me.btnCashBillsDaily.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnCashBillsDaily.FlatAppearance.BorderSize = 0
        Me.btnCashBillsDaily.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCashBillsDaily.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnCashBillsDaily.Location = New System.Drawing.Point(15, 75)
        Me.btnCashBillsDaily.Name = "btnCashBillsDaily"
        Me.btnCashBillsDaily.Size = New System.Drawing.Size(330, 45)
        Me.btnCashBillsDaily.TabIndex = 1
        Me.btnCashBillsDaily.Text = "  💵 Daily Cash (Cash Only)"
        Me.btnCashBillsDaily.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCashBillsDaily.UseVisualStyleBackColor = False
        '
        'btnCashOnlineBillsDaily
        '
        Me.btnCashOnlineBillsDaily.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.btnCashOnlineBillsDaily.FlatAppearance.BorderSize = 0
        Me.btnCashOnlineBillsDaily.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCashOnlineBillsDaily.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.btnCashOnlineBillsDaily.Location = New System.Drawing.Point(15, 120)
        Me.btnCashOnlineBillsDaily.Name = "btnCashOnlineBillsDaily"
        Me.btnCashOnlineBillsDaily.Size = New System.Drawing.Size(330, 45)
        Me.btnCashOnlineBillsDaily.TabIndex = 8
        Me.btnCashOnlineBillsDaily.Text = "  💳 Daily Cash (Cards/Online)"
        Me.btnCashOnlineBillsDaily.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCashOnlineBillsDaily.UseVisualStyleBackColor = False
        '
        'lblDailyTitle
        '
        Me.lblDailyTitle.AutoSize = True
        Me.lblDailyTitle.Font = New System.Drawing.Font("Segoe UI", 14.0!, System.Drawing.FontStyle.Bold)
        Me.lblDailyTitle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(243, Byte), Integer), CType(CType(156, Byte), Integer), CType(CType(18, Byte), Integer))
        Me.lblDailyTitle.Location = New System.Drawing.Point(15, 20)
        Me.lblDailyTitle.Name = "lblDailyTitle"
        Me.lblDailyTitle.Size = New System.Drawing.Size(223, 32)
        Me.lblDailyTitle.TabIndex = 0
        Me.lblDailyTitle.Text = "Daily Transactions"
        '
        'ReportHub
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1200, 957)
        Me.Controls.Add(Me.flowMain)
        Me.Controls.Add(Me.pnlHeader)
        Me.Name = "ReportHub"
        Me.Text = "Central Report Hub"
        Me.pnlHeader.ResumeLayout(False)
        Me.pnlHeader.PerformLayout()
        Me.flowMain.ResumeLayout(False)
        Me.pnlSalesCard.ResumeLayout(False)
        Me.pnlSalesCard.PerformLayout()
        Me.pnlStockCard.ResumeLayout(False)
        Me.pnlStockCard.PerformLayout()
        Me.pnlCustomerCard.ResumeLayout(False)
        Me.pnlCustomerCard.PerformLayout()
        Me.pnlSupplierCard.ResumeLayout(False)
        Me.pnlSupplierCard.PerformLayout()
        Me.pnlInvoiceCard.ResumeLayout(False)
        Me.pnlInvoiceCard.PerformLayout()
        Me.pnlDailyCard.ResumeLayout(False)
        Me.pnlDailyCard.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents pnlHeader As Panel
    Friend WithEvents lblTitle As Label
    Friend WithEvents lblSubtitle As Label
    Friend WithEvents flowMain As FlowLayoutPanel
    Friend WithEvents pnlSalesCard As Panel
    Friend WithEvents lblSalesTitle As Label
    Friend WithEvents btnDailySales As Button
    Friend WithEvents btnMonthlySales As Button
    Friend WithEvents btnMonthlyItemSales As Button
    Friend WithEvents btnStockReturn As Button
    Friend WithEvents pnlStockCard As Panel
    Friend WithEvents lblStockTitle As Label
    Friend WithEvents btnCurrentStock As Button
    Friend WithEvents pnlCustomerCard As Panel
    Friend WithEvents lblCustomerTitle As Label
    Friend WithEvents btnCustomerList As Button
    Friend WithEvents btnCustomerCredit As Button
    Friend WithEvents btnCustomerCreditByCity As Button
    Friend WithEvents btnCustomerCheque As Button
    Friend WithEvents btnCustomerPaymentNote As Button
    Friend WithEvents btnFullCreditReport As Button
    Friend WithEvents pnlSupplierCard As Panel
    Friend WithEvents lblSupplierTitle As Label
    Friend WithEvents btnSupplierList As Button
    Friend WithEvents btnSupplierDebit As Button
    Friend WithEvents btnSupplierPayment As Button
    Friend WithEvents btnSupplierCheque As Button
    Friend WithEvents btnFullDebitReport As Button
    Friend WithEvents btnPurchaseHistory As Button
    Friend WithEvents btnPurchaseReturnHistory As Button
    Friend WithEvents btnPurchaseRequest As Button
    Friend WithEvents pnlInvoiceCard As Panel
    Friend WithEvents lblInvoiceTitle As Label
    Friend WithEvents btnSaleInvA4 As Button
    Friend WithEvents btnBillDetails As Button
    Friend WithEvents btnSaleInvPOS As Button
    Friend WithEvents btnSaleReturnInv As Button
    Friend WithEvents btnPurchaseInvoice As Button
    Friend WithEvents btnQuotation As Button
    Friend WithEvents btnQuatePOS As Button
    Friend WithEvents pnlDailyCard As Panel
    Friend WithEvents lblDailyTitle As Label
    Friend WithEvents btnCashBillsDaily As Button
    Friend WithEvents btnCashOnlineBillsDaily As Button
    Friend WithEvents btnCreditBillsDaily As Button
    Friend WithEvents btnGrBillsDaily As Button
    Friend WithEvents btnElBillsDaily As Button
    Friend WithEvents btnVatBillsDaily As Button
    Friend WithEvents btnQtBillsDaily As Button
    Friend WithEvents btnFullDaySummary As Button
    Friend WithEvents btnFullChequeReport As Button
    Friend WithEvents btnCashSalesCashOnly As Button
    Friend WithEvents btnCashSalesOnline As Button
End Class

