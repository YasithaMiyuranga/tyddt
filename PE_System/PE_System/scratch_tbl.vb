Imports System
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared

Public Class TblCheck
    Public Shared Sub Main()
        Dim rpt As New ReportDocument()
        rpt.Load("d:\work\stockara\PE_Stock_Management\PE_System\PE_System\ReportDesign\Invoice\purchasereturn.rpt")
        Console.WriteLine("Tables:")
        For Each tbl As Table In rpt.Database.Tables
            Console.WriteLine(tbl.Name)
        Next
        Console.WriteLine("Links:")
        For Each link As TableLink In rpt.Database.Links
            Console.WriteLine(link.SourceTable.Name & "." & link.SourceFields(0).Name & " -> " & link.DestinationTable.Name & "." & link.DestinationFields(0).Name)
        Next
    End Sub
End Class
