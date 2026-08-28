Module BarcodeModule
    ''' <summary>
    ''' Converts a plain string to Code 128 encoded string compatible with IDAutomation or standard Code 128 fonts.
    ''' </summary>
    Public Function GetCode128(ByVal input As String) As String
        If String.IsNullOrEmpty(input) Then Return ""

        Dim charList As New System.Collections.Generic.List(Of Integer)
        Dim checkSum As Integer = 104 ' Start code B
        charList.Add(checkSum)

        For i As Integer = 0 To input.Length - 1
            Dim charVal As Integer = Asc(input(i))
            Dim codeVal As Integer = 0

            If charVal >= 32 AndAlso charVal <= 126 Then
                codeVal = charVal - 32
            Else
                codeVal = 0 ' Default to space for unhandled chars
            End If

            checkSum += (codeVal * (i + 1))
            charList.Add(codeVal)
        Next

        checkSum = checkSum Mod 103
        charList.Add(checkSum)
        charList.Add(106) ' Stop code

        ' Convert back to font characters
        Dim result As String = ""
        For Each cVal In charList
            If cVal <= 94 Then
                result &= Chr(cVal + 32)
            ElseIf cVal <= 106 Then
                result &= Chr(cVal + 100) ' Special handling for characters above 94
            End If
        Next

        Return result
    End Function
End Module
