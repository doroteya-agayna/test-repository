Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports ContextApi.Model
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Telerik.OpenAccess
Imports ContextApi.Tests

<TestClass()> _
Public Class ContextChangesTests
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
    ''' Scenario: Get all objects that are going to be inserted in the database    
    ''' GetInserts: Returns a list of new objects managed by the context. The changes are not yet committed to the database.
    ''' </summary>
    <TestMethod()> _
    Public Sub GetInsertChanges()
        Using dbContext As New EntitiesModel()
            Dim _project As Project = dbContext.Projects.FirstOrDefault()
            Assert.IsNotNull(_project, MessageHelper.NoRecordsInDatabase(GetType(Project)))

            dbContext.Add(New Bug() With {.ProjectId = _project.ProjectId})
            dbContext.Add(New NewItem() With {.ProjectId = _project.ProjectId, .StartTime = Date.Now, .ReadyFor = Date.Now})
            dbContext.Add(New Employee() With {.LastName = "Smith"})

            AssertExactNumberOfChangesInContext(dbContext, 3, 0, 0)

            Dim _contextChanges As ContextChanges = dbContext.GetChanges()
            Dim allInserts As IList(Of Object) = _contextChanges.GetInserts(Of Object)()
            Assert.AreEqual(3, allInserts.Count)

            For Each objectToBeInserted As Object In allInserts
                Dim state As ObjectState = dbContext.GetState(objectToBeInserted)
                Assert.IsTrue(state = ObjectState.New)
            Next objectToBeInserted
            'System.Linq.Enumerable.c()



            Assert.AreEqual(1, allInserts.Where(Function(insert) TypeOf insert Is Bug).Count())
            Assert.AreEqual(1, allInserts.Where(Function(insert) TypeOf insert Is NewItem).Count())
            Assert.AreEqual(1, allInserts.Where(Function(insert) TypeOf insert Is Employee).Count())

            Dim tasksToInsert As IList(Of Task) = _contextChanges.GetInserts(Of Task)()
            Assert.AreEqual(2, tasksToInsert.Count)
            Assert.AreEqual(1, tasksToInsert.Where(Function(insert) TypeOf insert Is Bug).Count())
            Assert.AreEqual(1, tasksToInsert.Where(Function(insert) TypeOf insert Is NewItem).Count())

            Dim bugsToInsert As IList(Of Bug) = _contextChanges.GetInserts(Of Bug)()
            Assert.AreEqual(1, bugsToInsert.Count)
            Assert.AreEqual(1, bugsToInsert.Where(Function(insert) TypeOf insert Is Bug).Count())

            dbContext.SaveChanges()

            AssertNoPendingChangesInContext(dbContext)
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: Get all objects that are going to be updated in the database    
    ''' GetUpdates: Returns a list of dirty objects managed by the context. The changes are not yet committed to the database.
    ''' </summary>
    <TestMethod()> _
    Public Sub GetUpdateChanges()
        Using dbContext As New EntitiesModel()
            Dim _bug As Bug = dbContext.Bugs.FirstOrDefault()
            Dim _newItem As NewItem = dbContext.NewItems.FirstOrDefault()
            Dim _employee As Employee = dbContext.Employees.FirstOrDefault()

            Assert.IsNotNull(_bug, MessageHelper.NoRecordsInDatabase(GetType(Bug)))
            Assert.IsNotNull(_newItem, MessageHelper.NoRecordsInDatabase(GetType(NewItem)))
            Assert.IsNotNull(_employee, MessageHelper.NoRecordsInDatabase(GetType(Employee)))

            _bug.Impact = 1
            _newItem.Title = "New Title"
            _employee.FirstName = "New First Name"

            AssertExactNumberOfChangesInContext(dbContext, 0, 3, 0)

            Dim _contextChanges As ContextChanges = dbContext.GetChanges()
            Dim allUpdates As IList(Of Object) = _contextChanges.GetUpdates(Of Object)()
            Assert.AreEqual(3, allUpdates.Count)


            For Each objectToBeUpdated As Object In allUpdates
                Dim state As ObjectState = dbContext.GetState(objectToBeUpdated)
                Assert.IsTrue(state = ObjectState.Dirty)
            Next objectToBeUpdated

            Assert.AreEqual(1, allUpdates.Where(Function(update) TypeOf update Is Bug).Count())
            Assert.AreEqual(1, allUpdates.Where(Function(update) TypeOf update Is NewItem).Count())
            Assert.AreEqual(1, allUpdates.Where(Function(update) TypeOf update Is Employee).Count())

            Dim tasksToUpdate As IList(Of Task) = _contextChanges.GetUpdates(Of Task)()
            Assert.AreEqual(2, tasksToUpdate.Count)
            Assert.AreEqual(1, tasksToUpdate.Where(Function(update) TypeOf update Is Bug).Count())
            Assert.AreEqual(1, tasksToUpdate.Where(Function(update) TypeOf update Is NewItem).Count())

            Dim bugsToUpdate As IList(Of Bug) = _contextChanges.GetUpdates(Of Bug)()
            Assert.AreEqual(1, bugsToUpdate.Count)
            Assert.AreEqual(1, bugsToUpdate.Where(Function(insert) TypeOf insert Is Bug).Count())

            dbContext.SaveChanges()

            AssertNoPendingChangesInContext(dbContext)
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: Get all objects that are going to be deleted in the database    
    ''' GetDeletes: Returns a list of deleted objects managed by the context. The changes are not yet committed to the database.
    ''' </summary>
    <TestMethod()> _
    Public Sub GetDeleteChanges()
        Using dbContext As New EntitiesModel()
            Dim _bug As Bug = dbContext.Bugs.FirstOrDefault()
            Dim _newItem As NewItem = dbContext.NewItems.FirstOrDefault()
            Dim _document As Document = dbContext.Documents.FirstOrDefault()

            Assert.IsNotNull(_bug, MessageHelper.NoRecordsInDatabase(GetType(Bug)))
            Assert.IsNotNull(_newItem, MessageHelper.NoRecordsInDatabase(GetType(NewItem)))
            Assert.IsNotNull(_document, MessageHelper.NoRecordsInDatabase(GetType(Document)))

            dbContext.Delete(_bug)
            dbContext.Delete(_newItem)
            dbContext.Delete(_document)

            AssertExactNumberOfChangesInContext(dbContext, 0, 0, 3)

            Dim _contextChanges As ContextChanges = dbContext.GetChanges()
            Dim allDeletes As IList(Of Object) = _contextChanges.GetDeletes(Of Object)()
            Assert.AreEqual(3, allDeletes.Count)

            For Each objectToBeDeleted As Object In allDeletes
                Dim state As ObjectState = dbContext.GetState(objectToBeDeleted)
                Assert.IsTrue(state = ObjectState.Deleted)
            Next objectToBeDeleted

            Assert.AreEqual(1, allDeletes.Where(Function(delete) TypeOf delete Is Bug).Count())
            Assert.AreEqual(1, allDeletes.Where(Function(delete) TypeOf delete Is NewItem).Count())
            Assert.AreEqual(1, allDeletes.Where(Function(delete) TypeOf delete Is Document).Count())

            Dim tasksToDelete As IList(Of Task) = _contextChanges.GetDeletes(Of Task)()
            Assert.AreEqual(2, tasksToDelete.Count)
            Assert.AreEqual(1, tasksToDelete.Where(Function(delete) TypeOf delete Is Bug).Count())
            Assert.AreEqual(1, tasksToDelete.Where(Function(delete) TypeOf delete Is NewItem).Count())

            Dim bugsToDelete As IList(Of Bug) = _contextChanges.GetDeletes(Of Bug)()
            Assert.AreEqual(1, bugsToDelete.Count)
            Assert.AreEqual(1, bugsToDelete.Where(Function(delete) TypeOf delete Is Bug).Count())

            dbContext.SaveChanges()

            AssertNoPendingChangesInContext(dbContext)
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: Save all changes that have been made to the database.
    ''' SaveChanges: Saves the changes with the specified concurency mode
    ''' </summary>
    <TestMethod()> _
    Public Sub SaveChanges()
        Using dbContext As New EntitiesModel()
            Dim _bug As Bug = dbContext.Bugs.FirstOrDefault()
            Dim _employee As Employee = dbContext.Employees.FirstOrDefault()
            Dim _task As New Task() With {.ProjectId = dbContext.Projects.FirstOrDefault().ProjectId}

            Assert.IsNotNull(_bug, MessageHelper.NoRecordsInDatabase(GetType(Bug)))
            Assert.IsNotNull(_employee, MessageHelper.NoRecordsInDatabase(GetType(Employee)))
            Assert.IsNotNull(_task, MessageHelper.NoRecordsInDatabase(GetType(Task)))

            Dim bugId As Integer = _bug.TaskId

            _employee.FirstName = "New First Name"
            dbContext.Add(_task)
            dbContext.Delete(_bug)

            AssertExactNumberOfChangesInContext(dbContext, 1, 1, 1)

            dbContext.SaveChanges()

            Assert.IsFalse(dbContext.HasChanges)

            ' Check that changes are commited to the database
            Dim employeeFromDb As Employee = dbContext.Employees.FirstOrDefault()
            Assert.IsNotNull(employeeFromDb, MessageHelper.NoRecordsInDatabase(GetType(Employee)))
            Assert.AreEqual(_employee.FirstName, employeeFromDb.FirstName)

            Dim taskFromDb As Task = dbContext.Tasks.LastOrDefault()
            Assert.IsNotNull(taskFromDb, MessageHelper.NoRecordsInDatabase(GetType(Task)))
            Assert.AreEqual(_task, taskFromDb)

            Assert.IsFalse(dbContext.Bugs.Any(Function(b) b.TaskId = bugId))

            AssertNoPendingChangesInContext(dbContext)
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: Clear all changes that have been made.
    ''' ClearChanges: Rolls back all changes in the context.
    ''' </summary>
    <TestMethod()> _
    Public Sub ClearChanges()
        Using dbContext As New EntitiesModel()
            Dim _bug As Bug = dbContext.Bugs.FirstOrDefault()
            Dim _employee As Employee = dbContext.Employees.FirstOrDefault()
            Dim _task As New Task() With {.ProjectId = dbContext.Projects.FirstOrDefault().ProjectId}

            Assert.IsNotNull(_bug, MessageHelper.NoRecordsInDatabase(GetType(Bug)))
            Assert.IsNotNull(_employee, MessageHelper.NoRecordsInDatabase(GetType(Employee)))
            Assert.IsNotNull(_task, MessageHelper.NoRecordsInDatabase(GetType(Task)))

            Dim oldEmployeeFirstName As String = _employee.FirstName
            Dim numberOfTasksBeforeClearChanges As Integer = dbContext.Tasks.Count()
            Dim numberOfBugsBeforeClearChanges As Integer = dbContext.Bugs.Count()

            _employee.FirstName = "New First Name"
            dbContext.Add(_task)
            dbContext.Delete(_bug)

            AssertExactNumberOfChangesInContext(dbContext, 1, 1, 1)

            dbContext.ClearChanges()

            Assert.IsFalse(dbContext.HasChanges)

            AssertNoPendingChangesInContext(dbContext)

            dbContext.SaveChanges() ' doesn't matter if SaveChanges() method is called.

            Dim employeeFromDb As Employee = dbContext.Employees.FirstOrDefault()
            Assert.IsNotNull(employeeFromDb, MessageHelper.NoRecordsInDatabase(GetType(Employee)))
            Assert.AreEqual(oldEmployeeFirstName, employeeFromDb.FirstName)

            Assert.AreEqual(numberOfBugsBeforeClearChanges, dbContext.Bugs.Count())

            Assert.AreEqual(numberOfTasksBeforeClearChanges, dbContext.Tasks.Count())
        End Using
    End Sub

    ''' <summary>
    '''  Scenario: Temporary save all current changes to the database.
    '''  FlushChanges: Flushes all current changes to the database but keeps the transaction running.
    ''' </summary>
    <TestMethod()> _
    Public Sub FlushChanges()
        Using dbContext As New EntitiesModel()
            Dim _bug As Bug = dbContext.Bugs.FirstOrDefault()
            Dim _employee As Employee = dbContext.Employees.FirstOrDefault()
            Dim _task As New Task() With {.ProjectId = dbContext.Projects.FirstOrDefault().ProjectId}

            Assert.IsNotNull(_bug, MessageHelper.NoRecordsInDatabase(GetType(Bug)))
            Assert.IsNotNull(_employee, MessageHelper.NoRecordsInDatabase(GetType(Employee)))
            Assert.IsNotNull(_task, MessageHelper.NoRecordsInDatabase(GetType(Task)))

            _employee.FirstName = "New First Name"
            dbContext.Add(_task)
            dbContext.Delete(_bug)

            AssertExactNumberOfChangesInContext(dbContext, 1, 1, 1)

            dbContext.FlushChanges()

            AssertNoPendingChangesInContext(dbContext)

            Assert.IsTrue(dbContext.HasChanges)
        End Using
    End Sub

    Private Sub AssertExactNumberOfChangesInContext(ByVal dbContext As OpenAccessContext, ByVal expectedInserts As Integer, ByVal expectedUpdates As Integer, ByVal expectedDeletes As Integer)
        Dim _contextChanges As ContextChanges = dbContext.GetChanges()

        Dim allUpdates As IList(Of Object) = _contextChanges.GetUpdates(Of Object)()
        Dim allDeletes As IList(Of Object) = _contextChanges.GetDeletes(Of Object)()
        Dim allInserts As IList(Of Object) = _contextChanges.GetInserts(Of Object)()

        Assert.AreEqual(expectedInserts, allInserts.Count)
        Assert.AreEqual(expectedUpdates, allUpdates.Count)
        Assert.AreEqual(expectedDeletes, allDeletes.Count)
    End Sub

    Private Sub AssertNoPendingChangesInContext(ByVal dbContext As OpenAccessContext)
        AssertExactNumberOfChangesInContext(dbContext, 0, 0, 0)
    End Sub
End Class
