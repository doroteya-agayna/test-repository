Imports System
Imports System.Linq
Imports ContextApi.Model
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Telerik.OpenAccess
Imports System.IO
Imports ContextApi.Tests

<TestClass()> _
Public Class ReadTests
    Inherits UnitTestsBase

    <TestInitialize()> _
    Public Overrides Sub TestInitialize()
        MyBase.TestInitialize()
    End Sub

    <TestCleanup> _
    Public Overrides Sub TestCleanup()
        MyBase.TestCleanup()
    End Sub

    ''' <summary>
    ''' Scenario: Fetch only one object from the database.
    ''' </summary>
    <TestMethod()> _
    Public Sub ReadOne()
        Using dbContext As New EntitiesModel()
            Dim singleEmployee As Employee = dbContext.Employees.FirstOrDefault()
            Assert.IsNotNull(singleEmployee, MessageHelper.NoRecordsInDatabase(GetType(Employee)))
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: Fetch exact number of objects from the database.
    ''' </summary>
    <TestMethod()> _
    Public Sub ReadMany()
        Using dbContext As New EntitiesModel()
            Dim twoEmployees As IQueryable(Of Employee) = dbContext.Employees.Take(2)
            Assert.IsNotNull(twoEmployees)
            Assert.AreEqual(2, twoEmployees.Count())
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: Fetching an object twice will result in two references pointing to the same object.
    ''' </summary>
    <TestMethod()> _
    Public Sub ReadObjectTwice()
        Using dbContext As New EntitiesModel()
            Dim firstEmployee As Employee = dbContext.Employees.FirstOrDefault()
            Assert.IsNotNull(firstEmployee, MessageHelper.NoRecordsInDatabase(GetType(Employee)))

            Dim secondEmployee As Employee = dbContext.Employees.FirstOrDefault()
            Assert.IsNotNull(secondEmployee, MessageHelper.NoRecordsInDatabase(GetType(Employee)))

            Assert.AreSame(firstEmployee, secondEmployee)
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: When object is fetched with GetObjectByKey() method that object is cached.
    ''' </summary>
    <TestMethod()> _
    Public Sub ReadObjectIsChached_GetObjectByKey()
        Dim employeeId As Integer
        Using dbContext As New EntitiesModel()
            employeeId = dbContext.Employees.Select(Function(e) e.EmployeeId).FirstOrDefault()
            Assert.AreNotEqual(0, employeeId, MessageHelper.NoRecordsInDatabase(GetType(Employee)))
        End Using
        Using dbContext As New EntitiesModel()
            Dim key As New ObjectKey(GetType(Employee).FullName, employeeId)

            Dim firstEmployee As Employee = TryCast(dbContext.GetObjectByKey(key), Employee)
            SetLog(dbContext)
            ' No hit to the database here
            Dim secondEmployee As Employee = TryCast(dbContext.GetObjectByKey(key), Employee)

            Assert.AreSame(firstEmployee, secondEmployee)
            Dim logSqlString As String = GetLogString(dbContext)
            Assert.AreEqual(String.Empty, logSqlString)
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: Anonymous objects are never cached and will hit the database with every query.
    ''' </summary>
    <TestMethod()> _
    Public Sub ReadAnonymousType()
        Using dbContext As New EntitiesModel()
            SetLog(dbContext)
            Dim employees1 = dbContext.Employees.Select(Function(s) New With {Key s.EmployeeId, Key s.FirstName}).ToList()
            Dim query1 As String = GetLogString(dbContext)
            SetLog(dbContext)
            Dim employees2 = dbContext.Employees.Select(Function(s) New With {Key s.EmployeeId, Key s.FirstName}).ToList()
            Dim query2 As String = GetLogString(dbContext)

            Assert.IsNotNull(employees1)
            Assert.IsNotNull(employees2)

            Assert.AreEqual(query1, query2)
        End Using
    End Sub

    Private Sub SetLog(ByVal dbContext As OpenAccessContext)
        Dim writer As New StringWriter()
        dbContext.Log = writer
    End Sub

    Private Function GetLogString(ByVal dbContext As OpenAccessContext) As String
        Dim writer As StringWriter = TryCast(dbContext.Log, StringWriter)
        If writer IsNot Nothing Then
            Dim logString As String = writer.GetStringBuilder().ToString()
            Return logString
        End If
        Return String.Empty
    End Function
End Class
