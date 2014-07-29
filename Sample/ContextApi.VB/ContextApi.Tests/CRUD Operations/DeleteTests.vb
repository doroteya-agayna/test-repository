Imports System
Imports System.Linq
Imports ContextApi.Model
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System.Collections.Generic
Imports ContextApi.Tests
<TestClass()> _
Public Class DeleteTests
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
    ''' Scenario: Delete a MonthlyReport. This deletion will not change the MonthlyReports collection of an employee.
    ''' Delete: Task.
    ''' Relationship: 1 - * / Employee - MonthlyReport.
    ''' Conditions: Navigation property MonthlyReports in Employee is not managed.
    ''' Result: MonthlyReports collection is not changed, even after the deletion of a MonthlyReport.
    ''' </summary>
    <TestMethod()> _
    Public Sub One_To_Many_DeleteChild_IsManagedFalse()
        Using dbContext As New EntitiesModel()
            Dim _monthlyReport As MonthlyReport = dbContext.MonthlyReports.FirstOrDefault()
            Assert.IsNotNull(_monthlyReport, MessageHelper.NoRecordsInDatabase(GetType(MonthlyReport)))

            Dim _employee As Employee = _monthlyReport.Employee

            Dim monthlyReportsCountBeforeDelete As Integer = _employee.MonthlyReports.Count

            dbContext.Delete(_monthlyReport)

            Dim monthlyReportsCountAfterDelete As Integer = _employee.MonthlyReports.Count

            ' Given that IsManaged is false, monthlyReportsCountBeforeDelete and monthlyReportsCountAfterDelete should be equal to each other
            Assert.AreEqual(monthlyReportsCountBeforeDelete, monthlyReportsCountAfterDelete)
            _monthlyReport = dbContext.MonthlyReports.FirstOrDefault()
            Assert.IsNotNull(_monthlyReport)

            dbContext.SaveChanges()
            Dim monthlyReportsCountAfterSave As Integer = _employee.MonthlyReports.Count

            Assert.AreEqual(monthlyReportsCountAfterDelete - 1, monthlyReportsCountAfterSave)

            _monthlyReport = dbContext.MonthlyReports.FirstOrDefault()
            Assert.IsNull(_monthlyReport)
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: Delete an Employee. This deletion will not change the Employee property of a MonthlyReport, even if that employee is deleted.
    ''' Delete: Task.
    ''' Relationship: 1 - * / Employee - MonthlyReport.
    ''' Conditions: Navigation property MonthlyReports in Employee is not managed.
    ''' Result: Employee property of an MonthlyReport is not changed.
    ''' </summary>
    <TestMethod()> _
    Public Sub One_To_Many_DeleteParent_IsManagedFalse()
        Using dbContext As New EntitiesModel()
            Dim _monthlyReport As MonthlyReport = dbContext.MonthlyReports.FirstOrDefault()
            Assert.IsNotNull(_monthlyReport, MessageHelper.NoRecordsInDatabase(GetType(MonthlyReport)))


            Dim _employee As Employee = _monthlyReport.Employee

            dbContext.Delete(_employee)

            ' Given that IsManaged is false monthlyRep.Employee will be not null
            Assert.IsNotNull(_monthlyReport.Employee)

            Assert.IsTrue(dbContext.GetState(_monthlyReport.Employee) = Telerik.OpenAccess.ObjectState.Deleted)
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: Delete a DailyReport. This deletion will be reflected in the DailyReports collection of the Employee.
    ''' Delete: DailyReport.
    ''' Relationship: 1 - * / Employee - DailyReport.
    ''' Conditions: Navigation property DailyReports in Employee is managed.
    ''' Result: DailyReports property of an Employee is changed.
    ''' </summary>
    <TestMethod()> _
    Public Sub One_To_Many_DeleteChild_IsManagedTrue()
        Using dbContext As New EntitiesModel()
            Dim _dailyReport As DailyReport = dbContext.DailyReports.FirstOrDefault()
            Assert.IsNotNull(_dailyReport, MessageHelper.NoRecordsInDatabase(GetType(DailyReport)))

            Dim _employee As Employee = _dailyReport.Employee

            Dim dailyReportsBeforeDeleteCount As Integer = _employee.DailyReports.Count()

            dbContext.Delete(_dailyReport)

            Dim dailyReportsAfterDeleteCount As Integer = _employee.DailyReports.Count()

            Assert.AreEqual(dailyReportsBeforeDeleteCount - 1, dailyReportsAfterDeleteCount)
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: Delete an Employee. This deletion will be reflected in the Employee property if a DailyReport.
    ''' Delete: DailyReport.
    ''' Relationship: 1 - * / Employee - DailyReport.
    ''' Conditions: Navigation property DailyReports in Employee is managed.
    ''' Result: DailyReports property of an Employee is changed.
    ''' </summary>
    <TestMethod()> _
    Public Sub One_To_Many_DeleteParent_IsManagedTrue()
        Using dbContext As New EntitiesModel()
            Dim _dailyReport As DailyReport = dbContext.DailyReports.FirstOrDefault()
            Assert.IsNotNull(_dailyReport, MessageHelper.NoRecordsInDatabase(GetType(DailyReport)))


            Dim _employee As Employee = _dailyReport.Employee
            Dim empPrimaryKey As Integer = _employee.EmployeeId

            dbContext.Delete(_employee)

            Assert.IsTrue(dbContext.GetState(_dailyReport.Employee) = Telerik.OpenAccess.ObjectState.Deleted)

            ' Primary key of an object marked for deletion can be accessed without exception being thrown
            Assert.AreEqual(empPrimaryKey, _employee.EmployeeId)
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: Delete a task and all task assignments that are bound to that task.
    ''' Delete: Task.
    ''' Relationship: 1 - * / Task - TaskAssignment.
    ''' Conditions: Navigation property TaskAssignment in Task is dependant.
    ''' Result: Task and TaskAssignments task are deleted.
    ''' </summary>
    <TestMethod()> _
    Public Sub One_To_Many_DeleteParent_IsDependantTrueInParent()
        Using dbContext As New EntitiesModel()
            Dim _task As Task = dbContext.Tasks.FirstOrDefault()
            Assert.IsNotNull(_task, MessageHelper.NoRecordsInDatabase(GetType(Task)))

            Dim _taskAssignment As TaskAssignment = _task.TaskAssignments.FirstOrDefault()
            Assert.IsNotNull(_taskAssignment, MessageHelper.NoRecordsInDatabase(GetType(TaskAssignment)))


            Dim allTaskCount As Integer = dbContext.Tasks.Count()
            Dim allTaskAssignmentCount As Integer = dbContext.TaskAssignments.Count()
            Dim taskAssigmentCount As Integer = _task.TaskAssignments.Count()

            dbContext.Delete(_task)

            Assert.IsTrue(dbContext.GetState(_taskAssignment) = Telerik.OpenAccess.ObjectState.Deleted)

            dbContext.SaveChanges()

            Assert.AreEqual(allTaskCount - 1, dbContext.Tasks.Count())
            Assert.AreEqual(allTaskAssignmentCount - taskAssigmentCount, dbContext.TaskAssignments.Count())
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: Delete a project with tasks bound to it. All the tasks should be deleted first in order this operation to succeed.
    ''' Delete: Project.
    ''' Relationship: 1 - * / Project - Task.
    ''' Conditions: Navigation property Tasks in Project is not dependant.
    ''' Result: Project is not deleted because there are tasks bound to that project.
    ''' </summary>
    <TestMethod()> _
    Public Sub One_To_Many_DeleteParent_IsDependantFalseInParent()
        Using dbContext As New EntitiesModel()
            Dim _task As Task = dbContext.Tasks.FirstOrDefault()
            Assert.IsNotNull(_task, MessageHelper.NoRecordsInDatabase(GetType(Task)))

            Dim _project As Project = _task.Project

            dbContext.Delete(_project)

            ' Here no exception is thrown because actually neighter the project nor the project tasks will be deleted.
            _task.PercentCompleted = 12

            Dim target As Action = AddressOf dbContext.SaveChanges
            AssertException.Throws(Of Telerik.OpenAccess.Exceptions.DataStoreException)(target)

            dbContext.Delete(_project.Tasks)
            dbContext.Delete(_project)
            dbContext.SaveChanges()
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: Delete a TaskAssignment and its parent Task.
    ''' Delete: TaskAssignment.
    ''' Relationship: 1 - * / Task - TaskAssignment.
    ''' Conditions: Navigation property Task in TaskAssignment is dependant.
    ''' Result: TaskAssignment and Task are deleted.
    ''' </summary>
    <TestMethod()> _
    Public Sub One_To_Many_DeleteChild_IsDependantTrueInChild()
        Using dbContext As New EntitiesModel()
            Dim _task As Task = dbContext.Tasks.FirstOrDefault()
            Assert.IsNotNull(_task, MessageHelper.NoRecordsInDatabase(GetType(Task)))

            Dim _taskAssignment As TaskAssignment = _task.TaskAssignments.FirstOrDefault()
            Assert.IsNotNull(_taskAssignment, MessageHelper.NoRecordsInDatabase(GetType(TaskAssignment)))

            Dim allTaskCount As Integer = dbContext.Tasks.Count()
            Dim allTaskAssignmentCount As Integer = dbContext.TaskAssignments.Count()
            Dim taskAssigmentCount As Integer = _task.TaskAssignments.Count()

            dbContext.Delete(_taskAssignment)

            Assert.IsTrue(dbContext.GetState(_task) = Telerik.OpenAccess.ObjectState.Deleted)

            dbContext.SaveChanges()

            Assert.AreEqual(allTaskCount - 1, dbContext.Tasks.Count())
            Assert.AreEqual(allTaskAssignmentCount - taskAssigmentCount, dbContext.TaskAssignments.Count())
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: Delete a task without its parent project being affected.
    ''' Delete: Task.
    ''' Relationship: 1 - * / Project - Tasks.
    ''' Conditions: Navigation property Project in Task is not dependant.
    ''' Result: Task is deleted, Project is not deleted.
    ''' </summary>
    <TestMethod()> _
    Public Sub One_To_Many_DeleteChild_IsDependantFalseInChild()
        Using dbContext As New EntitiesModel()
            Dim _task As Task = dbContext.Tasks.FirstOrDefault()
            Assert.IsNotNull(_task, MessageHelper.NoRecordsInDatabase(GetType(Task)))

            Dim _project As Project = _task.Project

            dbContext.Delete(_task)

            ' No exception is thrown here
            _project.Budget = 12

            dbContext.SaveChanges()
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: Delete a project.
    ''' Detele: Project.
    ''' Relationship: * - * / Project - Employee.
    ''' Conditions: Navigation property Employees of project is managed.
    ''' Result: The deleted project is removed from the collection employee.Projects.
    ''' </summary>
    <TestMethod()> _
    Public Sub Many_To_Many_IsManagedTrue()
        Using dbContext As New EntitiesModel()
            Dim _project As Project = dbContext.Projects.FirstOrDefault()
            Assert.IsNotNull(_project, MessageHelper.NoRecordsInDatabase(GetType(Project)))

            Dim _employee As Employee = _project.Employees.FirstOrDefault()
            Assert.IsNotNull(_employee, MessageHelper.NoRecordsInDatabase(GetType(Employee)))

            Dim projectCountForEmployee As Integer = _employee.Projects.Count

            dbContext.Delete(_project)

            Assert.AreEqual(_employee.Projects.Count, projectCountForEmployee - 1)
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: Delete a project.
    ''' Delete: Manager.
    ''' Relationship: * - * / Project - DocumentMetadatas.
    ''' Conditions: Navigation property DocumentMetadata in Project is not managed.
    ''' Result: The DocumentMetadatas of the project are not deleted but cannot be accessed.
    ''' </summary>
    <TestMethod()> _
    Public Sub Many_To_Many_IsManagedFalse()
        Using dbContext As New EntitiesModel()
            Dim _project As Project = dbContext.Projects.FirstOrDefault()
            Assert.IsNotNull(_project, MessageHelper.NoRecordsInDatabase(GetType(Project)))

            Dim projectId As Integer = _project.ProjectId

            Dim docs As IList(Of DocumentMetadata) = _project.DocumentMetadatum

            dbContext.Delete(_project)

            For Each doc As DocumentMetadata In docs
                ' Even after the project is marked for deletion it is not removed from the Projects collection of doc, because IsManaged is false
                Dim projectMarkedForDeletion As Project = doc.Projects.FirstOrDefault(Function(p) p.ProjectId = projectId)
                Assert.IsNotNull(projectMarkedForDeletion)
                Assert.AreSame(_project, projectMarkedForDeletion)

                Assert.IsTrue(dbContext.GetState(_project) = Telerik.OpenAccess.ObjectState.Deleted)
            Next doc
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: Delete a manager and all employees that are managed by the manager.
    ''' Delete: Manager.
    ''' Relationship: 1 - * / Manager - Employee.
    ''' Conditions: Navigation property Employees in Employee is managed.
    ''' Result: The manager and all of its employees are deleted.
    ''' </summary>
    <TestMethod()> _
    Public Sub One_To_Many_SelfReference_DeleteParent_IsManagedTrue()
        Using dbContext As New EntitiesModel()
            Dim _manager As Manager = dbContext.Managers.FirstOrDefault()
            Assert.IsNotNull(_manager, MessageHelper.NoRecordsInDatabase(GetType(Manager)))

            Dim employees As IList(Of Employee) = _manager.Employees

            dbContext.Delete(_manager)

            ' Theese employees are marked for deletion
            For Each employee In employees
                Assert.IsTrue(dbContext.GetState(employee) = Telerik.OpenAccess.ObjectState.Deleted)
            Next employee
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: Delete an employee.
    ''' Delete: Employee.
    ''' Relationship: 1 - * / Manager - Employee.
    ''' Conditions: Navigation property Employees in Employee is managed.
    ''' Result: The property Employees of the manager will be changed and the deleted employee will be removed from that collection.
    ''' </summary>
    <TestMethod()> _
    Public Sub One_To_Many_SelfReference_DeleteChild_IsManagedTrue()
        Using dbContext As New EntitiesModel()
            Dim _manager As Manager = dbContext.Managers.FirstOrDefault()
            Assert.IsNotNull(_manager, MessageHelper.NoRecordsInDatabase(GetType(Manager)))

            Dim employees As IList(Of Employee) = _manager.Employees

            Dim employeesCountBeforeDelete As Integer = _manager.Employees.Count

            Dim employeeToDelete As Employee = employees.FirstOrDefault()
            Assert.IsNotNull(employeeToDelete, MessageHelper.NoRecordsInDatabase(GetType(Employee)))

            dbContext.Delete(employeeToDelete)

            Assert.AreEqual(employeesCountBeforeDelete - 1, _manager.Employees.Count)
        End Using
    End Sub
End Class
