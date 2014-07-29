Imports Telerik.OpenAccess
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass> _
Public Class UnitTestsBase

    <TestInitialize> _
    Public Overridable Sub TestInitialize()
        LocalDbInstanceManager.CreateInstance()
        ContextOperations.ClearAllEntites()
        ContextOperations.AddEntities()
    End Sub

    <TestCleanup> _
    Public Overridable Sub TestCleanup()
        ContextOperations.ClearAllEntites()
    End Sub

    'Get the object state of a persistent object detached from the context.
    Protected Function GetObjectState(entity As Object) As ObjectState
        Return OpenAccessContext.PersistenceState.GetState(entity)
    End Function

End Class
