Imports ContextApi.Tests
Imports ContextApi.Model
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System
Imports System.Collections.Generic
Imports System.Linq

<TestClass()> _
Public Class UpdateTests
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
    ''' Scenario: Set the property of an object to a new value and save it.
    ''' SaveChanges: Saves all changes in the context.
    ''' </summary>
    <TestMethod()> _
    Public Sub Update_SaveChanges()
        Using dbContext As New EntitiesModel()
            Dim _employee As Employee = dbContext.Employees.LastOrDefault()
            Assert.IsNotNull(_employee, MessageHelper.NoRecordsInDatabase(GetType(Employee)))

            Dim tasks As IList(Of TaskAssignment) = _employee.TaskAssignments

            Dim newName As String = "Jane"

            _employee.FirstName = newName

            dbContext.SaveChanges()

            For Each _taskAssignment As TaskAssignment In tasks
                Assert.AreSame(_taskAssignment.Employee, _employee)
            Next _taskAssignment

            Dim employeeFromDb As Employee = dbContext.Employees.LastOrDefault()
            Assert.IsNotNull(employeeFromDb, MessageHelper.NoRecordsInDatabase(GetType(Employee)))

            Assert.AreEqual(newName, employeeFromDb.FirstName)
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: Set property of an object to new values and discard the changes.
    ''' ClearChanges: Rolls back all changes in the context.
    ''' </summary>
    <TestMethod()> _
    Public Sub Update_ClearChanges()
        Using dbContext As New EntitiesModel()
            Dim _employee As Employee = dbContext.Employees.FirstOrDefault()
            Assert.IsNotNull(_employee, MessageHelper.NoRecordsInDatabase(GetType(Employee)))

            Dim oldName As String = _employee.FirstName
            Dim newName As String = "Jane"

            _employee.FirstName = newName

            dbContext.ClearChanges()

            Dim employeeFromDb As Employee = dbContext.Employees.FirstOrDefault()
            Assert.IsNotNull(employeeFromDb, MessageHelper.NoRecordsInDatabase(GetType(Employee)))

            Assert.AreEqual(oldName, employeeFromDb.FirstName)
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: Set properties of an object to new values and save the changes but keep the transaction running. 
    ''' If SaveChanges is not called those changes will be rolled back.
    ''' FlushChanges: Flushes all current changes to the database but keeps the transaction running.
    ''' </summary>
    <TestMethod()> _
    Public Sub Update_FlushChanges_WithoutSaveChanges()
        Dim oldName As String = Nothing
        Dim newName As String = Nothing

        Using dbContext As New EntitiesModel()
            Dim _employee As Employee = dbContext.Employees.FirstOrDefault()
            Assert.IsNotNull(_employee, MessageHelper.NoRecordsInDatabase(GetType(Employee)))

            oldName = _employee.FirstName
            newName = "Jane"

            _employee.FirstName = newName
            ' Flushes all current changes to the database but keeps the transaction running.
            dbContext.FlushChanges()

            _employee = dbContext.Employees.FirstOrDefault()
            Assert.IsNotNull(_employee, MessageHelper.NoRecordsInDatabase(GetType(Employee)))
            Assert.AreEqual(newName, _employee.FirstName)
        End Using

        Using dbContext As New EntitiesModel()
            Dim _employee As Employee = dbContext.Employees.FirstOrDefault()
            Assert.IsNotNull(_employee, MessageHelper.NoRecordsInDatabase(GetType(Employee)))

            Assert.AreEqual(oldName, _employee.FirstName)
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: Set properties of an object to new values and save the changes but keep the transaction running. 
    ''' If SaveChanges is not called those changes will be rolled back.
    ''' </summary>
    <TestMethod()> _
    Public Sub Update_FlushChanges_SaveChangesCalled()
        Dim newName As String = "Jane"

        Using dbContext As New EntitiesModel()
            Dim _employee As Employee = dbContext.Employees.FirstOrDefault()
            Assert.IsNotNull(_employee, MessageHelper.NoRecordsInDatabase(GetType(Employee)))

            _employee.FirstName = newName

            dbContext.FlushChanges()

            _employee = dbContext.Employees.FirstOrDefault()
            Assert.IsNotNull(_employee, MessageHelper.NoRecordsInDatabase(GetType(Employee)))

            Assert.AreEqual(newName, _employee.FirstName)

            dbContext.SaveChanges()
        End Using

        Using dbContext As New EntitiesModel()
            Dim _employee As Employee = dbContext.Employees.FirstOrDefault()
            Assert.IsNotNull(_employee, MessageHelper.NoRecordsInDatabase(GetType(Employee)))

            Assert.AreEqual(newName, _employee.FirstName)
        End Using
    End Sub
End Class
