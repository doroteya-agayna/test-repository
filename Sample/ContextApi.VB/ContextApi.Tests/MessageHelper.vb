Public NotInheritable Class MessageHelper
    Private Sub New()
    End Sub
    Public Shared Function NoRecordsInDatabase(ByVal typeOfEntity As Type) As String
        Dim message As String = String.Format("No records of type {0} in the database!", typeOfEntity.Name)
        Return message
    End Function
End Class
