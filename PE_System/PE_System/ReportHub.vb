Option Explicit On
Option Strict Off

Imports System
Imports System.Windows.Forms

Public Class ReportHub

    Private Sub ReportHub_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.WindowState = FormWindowState.Maximized

        ' Restrict Sales, Stock, and Daily Transaction reports to Owner only
        Dim role As String = If(Module1.UserRole IsNot Nothing, Module1.UserRole.ToLower(), "")
        If role <> "owner" Then
            pnlSalesCard.Visible = False
            pnlStockCard.Visible = False
            pnlDailyCard.Visible = (role = "admin")
            
            Dim finRole As String = If(Module1.FinancialRole IsNot Nothing, Module1.FinancialRole.ToLower(), "")
            If finRole = "seller" Then
                pnlCustomerCard.Visible = False
                pnlSupplierCard.Visible = False
            End If
        Else
            pnlSalesCard.Visible = True
            pnlStockCard.Visible = True
            pnlDailyCard.Visible = True
            pnlCustomerCard.Visible = True
            pnlSupplierCard.Visible = True
        End If

        ApplySecurityLock()
    End Sub

    Public Sub ApplySecurityLock()
        If Not Module1.IsRgrVisible Then
            ' Hide all panels except the first one to create a single list
            pnlStockCard.Visible = False
            pnlCustomerCard.Visible = False
            pnlSupplierCard.Visible = False
            pnlInvoiceCard.Visible = False
            pnlDailyCard.Visible = False
            pnlSalesCard.Visible = True
            
            lblSalesTitle.Visible = False

            ' Ensure all other buttons in pnlSalesCard are hidden
            btnMonthlySales.Visible = False
            btnMonthlyItemSales.Visible = False

            ' Hide all other sensitive buttons to be safe
            btnCustomerList.Visible = False
            btnCustomerCredit.Visible = False
            btnCustomerCreditByCity.Visible = False
            btnCustomerCheque.Visible = False
            btnCustomerPaymentNote.Visible = False
            btnFullCreditReport.Visible = False
            btnFullChequeReport.Visible = False
            btnSupplierDebit.Visible = False
            btnSupplierCheque.Visible = False
            btnSupplierPayment.Visible = False
            btnFullDebitReport.Visible = False
            btnPurchaseHistory.Visible = False
            btnPurchaseReturnHistory.Visible = False
            
            btnGrBillsDaily.Visible = False
            btnCashBillsDaily.Visible = False
            btnCashOnlineBillsDaily.Visible = False
            btnCreditBillsDaily.Visible = False
            btnElBillsDaily.Visible = False
            btnVatBillsDaily.Visible = False
            btnQtBillsDaily.Visible = False
            btnFullDaySummary.Visible = False

            ' List of buttons to show in one single list
            Dim visibleButtons = New List(Of Button) From {
                btnDailySales,
                btnCurrentStock,
                btnPurchaseRequest,
                btnSaleInvA4,
                btnSaleInvPOS,
                btnSaleReturnInv,
                btnPurchaseInvoice,
                btnQuotation,
                btnQuatePOS,
                btnBillDetails
            }

            Dim finRole As String = If(Module1.FinancialRole IsNot Nothing, Module1.FinancialRole.ToLower(), "")
            Dim role As String = If(Module1.UserRole IsNot Nothing, Module1.UserRole.ToLower(), "")

            If role <> "owner" Then
                visibleButtons.Remove(btnDailySales)
                btnDailySales.Visible = False
            End If

            If finRole = "seller" Then
                visibleButtons.Remove(btnCurrentStock)
                visibleButtons.Remove(btnPurchaseRequest)
            End If

            ' Expand card to hold two columns
            pnlSalesCard.Width = 700

            Dim currentTop As Integer = 20
            Dim currentLeft As Integer = 15
            Dim colCount As Integer = 0

            For Each btn In visibleButtons
                btn.Parent = pnlSalesCard
                btn.Visible = True
                btn.Left = currentLeft
                btn.Top = currentTop

                colCount += 1
                If colCount = 2 Then
                    colCount = 0
                    currentLeft = 15
                    currentTop += btn.Height + 10 ' Move to next row
                Else
                    currentLeft = 355 ' Move to second column
                End If
            Next

            If colCount = 1 Then
                currentTop += visibleButtons.First().Height + 10
            End If

            pnlSalesCard.Height = currentTop + 10
            CenterCard()
        Else
            ' Revert visibility and structure
            lblSalesTitle.Visible = True

            Dim role As String = If(Module1.UserRole IsNot Nothing, Module1.UserRole.ToLower(), "")
            Dim finRole As String = If(Module1.FinancialRole IsNot Nothing, Module1.FinancialRole.ToLower(), "")

            If role <> "owner" Then
                pnlSalesCard.Visible = False
                pnlStockCard.Visible = False
                pnlDailyCard.Visible = (role = "admin")
                pnlCustomerCard.Visible = True
                pnlSupplierCard.Visible = True
                pnlInvoiceCard.Visible = True

                If finRole = "seller" Then
                    pnlCustomerCard.Visible = False
                    pnlSupplierCard.Visible = False
                End If
            Else
                pnlSalesCard.Visible = True
                pnlStockCard.Visible = True
                pnlDailyCard.Visible = True
                pnlCustomerCard.Visible = True
                pnlSupplierCard.Visible = True
                pnlInvoiceCard.Visible = True
            End If

            ' Restore original parents
            btnDailySales.Parent = pnlSalesCard
            btnCurrentStock.Parent = pnlStockCard
            btnCustomerList.Parent = pnlCustomerCard
            btnSupplierList.Parent = pnlSupplierCard
            btnPurchaseRequest.Parent = pnlSupplierCard

            btnSaleInvA4.Parent = pnlInvoiceCard
            btnSaleInvPOS.Parent = pnlInvoiceCard
            btnSaleReturnInv.Parent = pnlInvoiceCard
            btnPurchaseInvoice.Parent = pnlInvoiceCard
            btnQuotation.Parent = pnlInvoiceCard
            btnQuatePOS.Parent = pnlInvoiceCard
            btnBillDetails.Parent = pnlInvoiceCard
            btnPurchaseReturnHistory.Parent = pnlInvoiceCard

            ' Revert visibility for all buttons
            If finRole = "seller" Then
                btnSupplierList.Visible = False
            Else
                btnSupplierList.Visible = True
            End If
            btnBillDetails.Visible = True
            btnDailySales.Visible = True
            btnMonthlySales.Visible = True
            btnMonthlyItemSales.Visible = True

            btnCustomerList.Visible = True
            btnCustomerCredit.Visible = True
            btnCustomerCreditByCity.Visible = True
            btnCustomerCheque.Visible = True
            btnCustomerPaymentNote.Visible = True
            btnFullCreditReport.Visible = True
            btnFullChequeReport.Visible = True

            btnSupplierDebit.Visible = True
            btnSupplierCheque.Visible = True
            btnSupplierPayment.Visible = True
            btnFullDebitReport.Visible = True
            btnPurchaseHistory.Visible = False
            btnPurchaseReturnHistory.Visible = True

            btnGrBillsDaily.Visible = True
            btnCashBillsDaily.Visible = True
            btnCashOnlineBillsDaily.Visible = True
            btnCreditBillsDaily.Visible = True
            btnElBillsDaily.Visible = True
            btnVatBillsDaily.Visible = True
            btnQtBillsDaily.Visible = True
            btnFullDaySummary.Visible = True

            ' Reset original panel heights and widths (approximate from Designer)
            pnlSalesCard.Width = 360
            pnlSalesCard.Height = 320
            pnlStockCard.Height = 320
            pnlCustomerCard.Height = 440
            pnlSupplierCard.Height = 440
            pnlInvoiceCard.Height = 500
            pnlDailyCard.Height = 500

            ' Force hide Daily Transactions card and specific buttons requested by the user
            pnlDailyCard.Visible = False

            btnCustomerCheque.Visible = False
            btnCustomerList.Visible = False
            btnCustomerCreditByCity.Visible = False
            btnFullChequeReport.Visible = False

            btnSupplierList.Visible = False
            btnSupplierCheque.Visible = False
            btnPurchaseRequest.Visible = False
            btnFullDebitReport.Visible = False

            ' Reposition inside each panel properly
            RepositionButtons(pnlSalesCard)
            RepositionButtons(pnlStockCard)
            RepositionButtons(pnlCustomerCard)
            RepositionButtons(pnlSupplierCard)
            RepositionButtons(pnlInvoiceCard)
            RepositionButtons(pnlDailyCard)

            pnlSalesCard.Visible = False
            pnlStockCard.Visible = False
            CenterCard()
        End If
    End Sub

    Private Sub CenterCard()
        If Not Module1.IsRgrVisible Then
            ' Position the card on the left side instead of perfectly centered
            pnlSalesCard.Margin = New Padding(10, 20, 0, 0)
        Else
            pnlSalesCard.Margin = New Padding(10)
        End If
    End Sub

    Private Sub flowMain_Resize(sender As Object, e As EventArgs) Handles flowMain.Resize
        CenterCard()
    End Sub

    Private Sub RepositionButtons(pnl As Panel)
        Dim currentTop As Integer = 50

        Dim titleLabel As Label = pnl.Controls.OfType(Of Label)().FirstOrDefault()
        If titleLabel IsNot Nothing AndAlso titleLabel.Visible Then
            currentTop = titleLabel.Bottom + 15
        Else
            currentTop = 15
        End If

        Dim buttons = pnl.Controls.OfType(Of Button)().Where(Function(b) b.Visible).OrderBy(Function(b) b.TabIndex).ToList()

        For Each btn In buttons
            btn.Left = 15 ' Reset back to single column left position
            btn.Top = currentTop
            currentTop += btn.Height + 5
        Next
    End Sub

    Private Sub OpenReport(reportIndex As Integer)
        ' Sales report indices: 0 (Daily Sales), 1 (Monthly Sales), 2 (Monthly Item Sales)
        ' Stock report index: 3 (Current Stock List)
        If Array.IndexOf({0, 1, 2, 3}, reportIndex) >= 0 Then
            Dim role As String = If(Module1.UserRole IsNot Nothing, Module1.UserRole.ToLower(), "")
            If role <> "owner" Then
                Dim reportName As String = If(reportIndex = 3, "Stock Reports", "Sales Reports")
                MessageBox.Show($"Access Denied: {reportName} are restricted to the Owner.", "Authorization Error", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                Return
            End If
        End If

        ' Restrict Customer and Supplier reports from sellers
        If Array.IndexOf({4, 5, 6, 20, 21, 25, 26, 7, 8, 9, 19, 23, 24}, reportIndex) >= 0 Then
            Dim finRole As String = If(Module1.FinancialRole IsNot Nothing, Module1.FinancialRole.ToLower(), "")
            If finRole = "seller" Then
                MessageBox.Show("Access Denied: Customer and Supplier Reports are restricted for your role.", "Authorization Error", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                Return
            End If
        End If

        Dim viewer As New SalesHistoryForm()
        ' Set report context with current date range as default
        viewer.SetReportContext(reportIndex, DateTime.Now, DateTime.Now)
        viewer.Show()
    End Sub

    ' Sales Reports
    Private Sub btnDailySales_Click(sender As Object, e As EventArgs) Handles btnDailySales.Click
        OpenReport(0)
    End Sub

    Private Sub btnMonthlySales_Click(sender As Object, e As EventArgs) Handles btnMonthlySales.Click
        OpenReport(1)
    End Sub

    Private Sub btnMonthlyItemSales_Click(sender As Object, e As EventArgs) Handles btnMonthlyItemSales.Click
        OpenReport(2)
    End Sub

    ' Stock Reports
    Private Sub btnCurrentStock_Click(sender As Object, e As EventArgs) Handles btnCurrentStock.Click
        OpenReport(3)
    End Sub

    ' --- Customer Reports ---
    Private Sub btnCustomerCredit_Click(sender As Object, e As EventArgs) Handles btnCustomerCredit.Click
        OpenReport(5)
    End Sub

    Private Sub btnCustomerCreditByCity_Click(sender As Object, e As EventArgs) Handles btnCustomerCreditByCity.Click
        OpenReport(26)
    End Sub

    Private Sub btnCustomerCheque_Click(sender As Object, e As EventArgs) Handles btnCustomerCheque.Click
        OpenReport(6)
    End Sub

    Private Sub btnCustomerList_Click(sender As Object, e As EventArgs) Handles btnCustomerList.Click
        OpenReport(4)
    End Sub

    Private Sub btnCustomerPaymentNote_Click(sender As Object, e As EventArgs) Handles btnCustomerPaymentNote.Click
        OpenReport(20)
    End Sub

    Private Sub btnFullCreditReport_Click(sender As Object, e As EventArgs) Handles btnFullCreditReport.Click
        OpenReport(21)
    End Sub

    Private Sub btnFullChequeReport_Click(sender As Object, e As EventArgs) Handles btnFullChequeReport.Click
        OpenReport(25)
    End Sub

    ' --- Supplier Reports ---
    Private Sub btnSupplierList_Click(sender As Object, e As EventArgs) Handles btnSupplierList.Click
        OpenReport(7)
    End Sub

    Private Sub btnSupplierDebit_Click(sender As Object, e As EventArgs) Handles btnSupplierDebit.Click
        OpenReport(8)
    End Sub

    Private Sub btnSupplierCheque_Click(sender As Object, e As EventArgs) Handles btnSupplierCheque.Click
        OpenReport(9)
    End Sub

    Private Sub btnSupplierPayment_Click(sender As Object, e As EventArgs) Handles btnSupplierPayment.Click
        OpenReport(23)
    End Sub

    Private Sub btnFullDebitReport_Click(sender As Object, e As EventArgs) Handles btnFullDebitReport.Click
        OpenReport(24)
    End Sub

    Private Sub btnPurchaseRequest_Click(sender As Object, e As EventArgs) Handles btnPurchaseRequest.Click
        OpenReport(19)
    End Sub

    Private Sub btnCashSalesCashOnly_Click(sender As Object, e As EventArgs) Handles btnCashSalesCashOnly.Click
        Dim role As String = If(Module1.UserRole IsNot Nothing, Module1.UserRole.ToLower(), "")
        If role <> "owner" AndAlso role <> "admin" Then
            MessageBox.Show("Access Denied: This report is restricted to Owner or Admin.", "Authorization Error", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            Return
        End If
        Dim viewer As New PurchaseCashViewer()
        viewer.ShowReport(0)  ' 0 = Cash Only (pur_type=Cash, p_method=Cash)
        viewer.Show()
    End Sub

    Private Sub btnCashSalesOnline_Click(sender As Object, e As EventArgs) Handles btnCashSalesOnline.Click
        Dim role As String = If(Module1.UserRole IsNot Nothing, Module1.UserRole.ToLower(), "")
        If role <> "owner" AndAlso role <> "admin" Then
            MessageBox.Show("Access Denied: This report is restricted to Owner or Admin.", "Authorization Error", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            Return
        End If
        Dim viewer As New PurchaseCashViewer()
        viewer.ShowReport(1)  ' 1 = Cards/Online (pur_type=Cash, p_method IN credit card/debit card/online transfer)
        viewer.Show()
    End Sub

    Private Sub btnPurchaseReturnHistory_Click(sender As Object, e As EventArgs) Handles btnPurchaseReturnHistory.Click
        OpenReport(18)
    End Sub

    ' --- Invoice & Bill Reports ---
    Private Sub btnSaleInvA4_Click(sender As Object, e As EventArgs) Handles btnSaleInvA4.Click
        OpenReport(11)
    End Sub

    Private Sub btnSaleInvPOS_Click(sender As Object, e As EventArgs) Handles btnSaleInvPOS.Click
        OpenReport(12)
    End Sub

    Private Sub btnSaleReturnInv_Click(sender As Object, e As EventArgs) Handles btnSaleReturnInv.Click
        OpenReport(13)
    End Sub

    Private Sub btnPurchaseInvoice_Click(sender As Object, e As EventArgs) Handles btnPurchaseInvoice.Click
        OpenReport(14)
    End Sub

    Private Sub btnQuotation_Click(sender As Object, e As EventArgs) Handles btnQuotation.Click
        OpenReport(15)
    End Sub

    Private Sub btnQuatePOS_Click(sender As Object, e As EventArgs) Handles btnQuatePOS.Click
        OpenReport(16)
    End Sub

    Private Sub btnBillDetails_Click(sender As Object, e As EventArgs) Handles btnBillDetails.Click
        OpenReport(22)
    End Sub

    ' --- Daily Transaction Reports ---
    Private Sub OpenDailyReport(reportIndex As Integer)
        Dim role As String = If(Module1.UserRole IsNot Nothing, Module1.UserRole.ToLower(), "")
        If role <> "owner" AndAlso role <> "admin" Then
            MessageBox.Show("Access Denied: Daily Transaction Reports are restricted to the Owner or Admin.", "Authorization Error", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            Return
        End If

        Dim viewer As New DailyTransactionViewer()
        viewer.ShowReport(reportIndex)
        viewer.Show()
    End Sub

    Private Sub btnCashBillsDaily_Click(sender As Object, e As EventArgs) Handles btnCashBillsDaily.Click
        OpenDailyReport(0)
    End Sub

    Private Sub btnCashOnlineBillsDaily_Click(sender As Object, e As EventArgs) Handles btnCashOnlineBillsDaily.Click
        OpenDailyReport(1)
    End Sub

    Private Sub btnCreditBillsDaily_Click(sender As Object, e As EventArgs) Handles btnCreditBillsDaily.Click
        OpenDailyReport(2)
    End Sub

    Private Sub btnGrBillsDaily_Click(sender As Object, e As EventArgs) Handles btnGrBillsDaily.Click
        OpenDailyReport(3)
    End Sub

    Private Sub btnElBillsDaily_Click(sender As Object, e As EventArgs) Handles btnElBillsDaily.Click
        OpenDailyReport(4)
    End Sub

    Private Sub btnVatBillsDaily_Click(sender As Object, e As EventArgs) Handles btnVatBillsDaily.Click
        OpenDailyReport(5)
    End Sub

    Private Sub btnQtBillsDaily_Click(sender As Object, e As EventArgs) Handles btnQtBillsDaily.Click
        OpenDailyReport(6)
    End Sub

    Private Sub btnFullDaySummary_Click(sender As Object, e As EventArgs) Handles btnFullDaySummary.Click
        OpenDailyReport(7)
    End Sub

End Class
