Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Printing

Partial Public Class PrintSelector

    Public Property InvoiceNo As String
    Public Property BillingType As String ' "Quote", "Retail", "Wholesale" etc.
    
    Private _isReturn As Boolean = False
    Public Property IsReturn As Boolean
        Get
            Return _isReturn
        End Get
        Set(value As Boolean)
            _isReturn = value
            ' When IsReturn is set, immediately update printer selection logic
            UpdatePrinterSelection()
            InitializeBillTypes()
        End Set
    End Property

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
        LoadPrinters()
    End Sub

    ' Map UI selection to SaleInv report index
    Private Structure BillTypeItem
        Public Property Name As String
        Public Property ReportIndex As Integer
        Public Overrides Function ToString() As String
            Return Name
        End Function
    End Structure

    Private Sub PrintSelector_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        InitializeBillTypes()
        UpdatePrinterSelection()
    End Sub

    Private Sub LoadPrinters()
        Try
            ' Clear both combos
            cmbPrinterReturn.Items.Clear()
            cmbPrinterSale.Items.Clear()

            ' Load all installed printers into both
            For Each printer As String In PrinterSettings.InstalledPrinters
                cmbPrinterReturn.Items.Add(printer)
                cmbPrinterSale.Items.Add(printer)
            Next

            If cmbPrinterSale.Items.Count > 0 Then
                Dim defaultPrinter As New PrinterSettings()
                cmbPrinterSale.SelectedItem = defaultPrinter.PrinterName
                cmbPrinterReturn.SelectedItem = defaultPrinter.PrinterName
            Else
                cmbPrinterReturn.Items.Add("No Printers Found")
                cmbPrinterSale.Items.Add("No Printers Found")
                cmbPrinterReturn.SelectedIndex = 0
                cmbPrinterSale.SelectedIndex = 0
            End If
        Catch ex As Exception
            MessageBox.Show("Error loading printers: " & ex.Message)
        End Try
    End Sub

    Private Sub UpdatePrinterSelection()
        ' Handle Return Note Printer forcing to PDF
        If IsReturn Then
            ' Find Microsoft Print to PDF specifically
            Dim pdfPrinter As String = ""
            For Each p As String In cmbPrinterReturn.Items
                If p.ToLower().Contains("microsoft print to pdf") Then
                    pdfPrinter = p
                    Exit For
                End If
            Next
            
            If Not String.IsNullOrEmpty(pdfPrinter) Then
                cmbPrinterReturn.SelectedItem = pdfPrinter
                cmbPrinterReturn.Enabled = True ' Unlocked to allow changing if needed
            End If
        Else
            ' Hide return printer parts if not a return
            lblPrinterReturn.Visible = False
            cmbPrinterReturn.Visible = False
        End If
    End Sub

    Private Sub InitializeBillTypes()
        cmbBillType.Items.Clear()
        
        ' Return එකක් නම් පමණක් skip option එක සහ Return Printer එක පෙන්වන්න
        chkNoSalePrint.Visible = IsReturn 
        lblPrinterReturn.Visible = IsReturn
        cmbPrinterReturn.Visible = IsReturn

        If IsReturn Then
            lblBillType.Text = "Also Print Updated Sale Bill?"
            ' Ensure Sale Printer is at its default position (below return printer)
            lblPrinterSale.Location = New Point(20, 55)
            cmbPrinterSale.Location = New Point(20, 70)
            lblBillType.Location = New Point(20, 100)
            cmbBillType.Location = New Point(20, 115)
            chkNoSalePrint.Location = New Point(20, 145)
            
            ' Adjust buttons position for bigger form
            btnPrint.Location = New Point(40, 185)
            btnCancel.Location = New Point(150, 185)
            Me.ClientSize = New Size(284, 240) 
        Else
            lblBillType.Text = "Select Bill Type:"
            ' Move Sale Bill controls UP to fill space if Return printer is hidden
            lblPrinterSale.Location = lblPrinterReturn.Location
            cmbPrinterSale.Location = cmbPrinterReturn.Location
            lblBillType.Location = New Point(lblBillType.Location.X, lblPrinterSale.Bottom + 10)
            cmbBillType.Location = New Point(cmbBillType.Location.X, lblBillType.Bottom + 2)
            chkNoSalePrint.Visible = False

            ' Adjust buttons position for shorter form
            btnPrint.Location = New Point(40, 140)
            btnCancel.Location = New Point(150, 140)
            Me.ClientSize = New Size(284, 190) 
        End If
        
        If BillingType = "Quote" Then
            ' Quotation options
            cmbBillType.Items.Add(New BillTypeItem() With {.Name = "Quotation POS", .ReportIndex = 5})
            cmbBillType.Items.Add(New BillTypeItem() With {.Name = "Quotation Standard", .ReportIndex = 4})
        Else
            ' Standard sale options
            cmbBillType.Items.Add(New BillTypeItem() With {.Name = "POS Invoice", .ReportIndex = 0})
            cmbBillType.Items.Add(New BillTypeItem() With {.Name = "Standard Invoice", .ReportIndex = 1})
        End If
        
        If cmbBillType.Items.Count > 0 Then cmbBillType.SelectedIndex = 0
    End Sub

    Private Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click
        If String.IsNullOrEmpty(InvoiceNo) Then
            MessageBox.Show("Invoice number is missing.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Try
            ' 1. If it's a Return, ALWAYS print the Return Note (Stock Return - Index 3) to the Return Printer
            If IsReturn Then
                Dim selectedReturnPrinter As String = cmbPrinterReturn.Text
                Dim rptReturn As New SaleInv()
                ' Index 3 is Stock Return. Passing True as isReturn parameter.
                rptReturn.ShowReport(InvoiceNo, 3, True, True) 
                rptReturn.DirectPrint(selectedReturnPrinter)
            End If

            ' 2. Print Sale Bill (if not skipped or if not a return)
            If Not (IsReturn AndAlso chkNoSalePrint.Checked) Then
                ' Get selected report index and sale printer
                Dim selectedSalePrinter As String = cmbPrinterSale.Text
                Dim selectedItem = DirectCast(cmbBillType.SelectedItem, BillTypeItem)
                Dim reportTypeIndex As Integer = selectedItem.ReportIndex

                ' Create report form
                Dim rptForm As New SaleInv()

                ' Load report in silent mode (no UI)
                ' Passing current IsReturn value (True if returning, False otherwise)
                rptForm.ShowReport(InvoiceNo, reportTypeIndex, True, IsReturn, "", 1, 0, True)
                
                ' Send to printer
                rptForm.DirectPrint(selectedSalePrinter)
            End If
            
            Me.DialogResult = DialogResult.OK
            Me.Close()
        Catch ex As Exception
            MessageBox.Show("Printing Failed: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub
End Class
