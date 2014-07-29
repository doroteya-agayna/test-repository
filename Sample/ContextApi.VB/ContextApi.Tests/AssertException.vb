Imports System
Imports System.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting

Public NotInheritable Class AssertException
    Private Sub New()
    End Sub

    Public Shared Sub Throws(Of TException As Exception)(ByVal _action As Action)
        If _action Is Nothing Then
            Throw New ArgumentNullException("action")
        End If

        Try
            _action()
            Dim message As String = "Should have thrown " & GetType(TException).Name
            Assert.Fail(message)
        Catch e1 As TException
            ' OK
        End Try
    End Sub
End Class
