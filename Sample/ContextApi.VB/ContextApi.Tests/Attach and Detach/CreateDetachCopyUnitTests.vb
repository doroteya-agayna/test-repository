Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports ContextApi.Model
Imports Telerik.OpenAccess
Imports System.Collections.Generic
Imports Telerik.OpenAccess.FetchOptimization

<TestClass()> _
Public Class CreateDetachCopyUnitTests
    Inherits UnitTestsBase


    Private context As EntitiesModel

    ''' <summary>
    ''' Scenario: Detach a single object from its managing context.
    ''' Condition: No navigation properties should be detached with the object.
    ''' Outcome: The detached object is not the same instance as the original,its reference properties will be null
    ''' and reference collections will be empty.
    ''' </summary>
    <TestMethod> _
    Public Sub Detach_BasicDetach()
        Dim retrievedEmployee As Employee
        Dim detachedEmployee As Employee

        Using Me.context
            Me.context = New EntitiesModel()

            retrievedEmployee = context.Employees.FirstOrDefault(Function(emp) emp.Title = "Developer")

            detachedEmployee = context.CreateDetachedCopy(Of Employee)(retrievedEmployee)
        End Using

        Dim detachedEmployeeState As ObjectState = Me.GetObjectState(detachedEmployee)

        Assert.AreNotSame(detachedEmployee, retrievedEmployee)
        Assert.IsNull(detachedEmployee.Supervisor)
        Assert.AreEqual(0, detachedEmployee.TaskAssignments.Count)
    End Sub

    ''' <summary>
    ''' Scenario: Detach a single object from its managing context together with some of its navigation properties.
    ''' Condition: The navigation properties will be specified with strings representing their names.
    ''' Outcome: The state of the detached object will be DetachedClean and the specified navigation properties will be detached as well.
    ''' </summary>
    <TestMethod> _
    Public Sub Detach_DetachWithReferenecPropertiesPassedAsStrings()
        Dim detachedEmployee As Employee
        Dim expectedAssignmentsCount As Integer

        Using Me.context
            Me.context = New EntitiesModel()

            Dim retrievedEmployee As Employee = context.Employees.FirstOrDefault(Function(emp) emp.Title = "Developer")
            expectedAssignmentsCount = retrievedEmployee.TaskAssignments.Count

            detachedEmployee = context.CreateDetachedCopy(Of Employee)(retrievedEmployee, "Supervisor", "TaskAssignments")
        End Using

        Dim detachedEmployeeState As ObjectState = Me.GetObjectState(detachedEmployee)

        Assert.AreEqual(ObjectState.DetachedClean, detachedEmployeeState)
        Assert.IsNotNull(detachedEmployee.Supervisor)
        Assert.AreEqual(expectedAssignmentsCount, detachedEmployee.TaskAssignments.Count)
    End Sub

    ''' <summary>
    ''' Scenario: Detach a single object from its managing context together with some of its navigation properties.
    ''' Condition: The navigation properties will be specified with expressions.
    ''' Outcome: The state of the detached object will be DetachedClean and the specified navigation properties will be detached as well.
    ''' </summary>
    <TestMethod> _
    Public Sub Detach_DetachWithReferencePropertiesPassedAsExpression()
        Dim detachedEmployee As Employee
        Dim expectedAssignmentsCount As Integer

        Using Me.context
            Me.context = New EntitiesModel()

            Dim retrievedEmployee As Employee = context.Employees.FirstOrDefault(Function(emp) emp.Title = "Developer")
            expectedAssignmentsCount = retrievedEmployee.TaskAssignments.Count

            detachedEmployee = context.CreateDetachedCopy(Of Employee)(retrievedEmployee, Function(emp As Employee) emp.Supervisor, Function(emp As Employee) emp.TaskAssignments)
        End Using

        Dim detachedEmployeeState As ObjectState = Me.GetObjectState(detachedEmployee)

        Assert.AreEqual(ObjectState.DetachedClean, detachedEmployeeState)
        Assert.IsNotNull(detachedEmployee.Supervisor)
        Assert.AreEqual(expectedAssignmentsCount, detachedEmployee.TaskAssignments.Count)
    End Sub

    ''' <summary>
    ''' Scenario: Detach a single object from its managing context together with some of its navigation properties.
    ''' Condition: The navigation properties which will be detached will be specified by a fetch strategy.
    ''' Outcome: The state of the detached object will be DetachedClean and the specified navigation properties will be detached as well.
    ''' </summary>
    <TestMethod> _
    Public Sub Detach_DetachWithReferencePropertiesUsingFetchStrategy()
        Dim detachedEmployee As Employee
        Dim expectedAssignmentsCount As Integer

        Using Me.context
            Me.context = New EntitiesModel()

            Dim retrievedEmployee As Employee = context.Employees.FirstOrDefault(Function(emp) emp.Title = "Developer")
            expectedAssignmentsCount = retrievedEmployee.TaskAssignments.Count

            Dim withSupervisorAndTaskAssignments As New FetchStrategy()
            withSupervisorAndTaskAssignments.LoadWith(Of Employee)(Function(emp) emp.Supervisor)
            withSupervisorAndTaskAssignments.LoadWith(Of Employee)(Function(emp) emp.TaskAssignments)

            detachedEmployee = context.CreateDetachedCopy(Of Employee)(retrievedEmployee, withSupervisorAndTaskAssignments)
        End Using

        Dim detachedEmployeeState As ObjectState = Me.GetObjectState(detachedEmployee)

        Assert.AreEqual(ObjectState.DetachedClean, detachedEmployeeState)
        Assert.IsNotNull(detachedEmployee.Supervisor)
        Assert.AreEqual(expectedAssignmentsCount, detachedEmployee.TaskAssignments.Count)
    End Sub

    ''' <summary>
    ''' Scenario: Detach multiple objects from their managing context with some of their navigation properties.
    ''' Condition: The navigation properties which will be detached will be specified by a fetch strategy.
    ''' Outcome: All of the detached objects will have DetachedClean state and their specified navigation properties will be detached as well.
    ''' </summary>
    <TestMethod> _
    Public Sub Detach_DetachMultipleEntitiesWithReferencePropertiesUsingFetchStrategy()
        Dim retrievedTasks As IList(Of Task)
        Dim retrievedTasksCount As Integer
        Dim detachedTasks As IList(Of Task)

        Using Me.context
            Me.context = New EntitiesModel()

            Dim withProjectAndAssignments As New FetchStrategy()
            withProjectAndAssignments.LoadWith(Of Task)(Function(task) task.Project)
            withProjectAndAssignments.LoadWith(Of Task)(Function(task) task.TaskAssignments)

            retrievedTasks = context.Tasks.LoadWith(withProjectAndAssignments).ToList()
            retrievedTasksCount = retrievedTasks.Count

            detachedTasks = context.CreateDetachedCopy(Of Task)(retrievedTasks, withProjectAndAssignments).ToList()
        End Using

        For taskIndex As Integer = 0 To retrievedTasksCount - 1
            Dim detachedTask As Task = detachedTasks(taskIndex)
            Dim detachedTaskState As ObjectState = Me.GetObjectState(detachedTask)

            Dim respectiveManagedTask As Task = retrievedTasks(taskIndex)

            Assert.AreEqual(ObjectState.DetachedClean, detachedTaskState)
            Assert.IsNotNull(detachedTask.Project)
            Assert.AreEqual(respectiveManagedTask.TaskAssignments.Count, detachedTask.TaskAssignments.Count)
        Next
    End Sub

    ''' <summary>
    ''' Scenario: Detach multiple objects from their managing object without loading any of their navigation properties.
    ''' Condition: A null value will be passed as a FetchStrategy argument to the CreateDetachedCopy method.
    ''' Outcome: All of the detached objects will have DetachedClean state, their reference properties will be null and their 
    ''' reference collections will be empty because the default fetch strategy will be used.
    ''' </summary>
    <TestMethod> _
    Public Sub Detach_DetachMultipleEntitiesWithoutReferenceProperties()
        Dim detachedTasks As IList(Of Task)

        Using Me.context
            Me.context = New EntitiesModel()

            Dim retrievedTasks As IList(Of Task) = context.Tasks.ToList()

            detachedTasks = context.CreateDetachedCopy(Of Task)(retrievedTasks, Nothing).ToList()
        End Using

        For Each detachedTask As Task In detachedTasks
            Dim detachedTaskState As ObjectState = Me.GetObjectState(detachedTask)

            Assert.AreEqual(ObjectState.DetachedClean, detachedTaskState)
            Assert.IsNull(detachedTask.Project)
            Assert.AreEqual(0, detachedTask.TaskAssignments.Count)
        Next
    End Sub

    ''' <summary>
    ''' Scenario: Detach multiple objects.
    ''' Condition: The detached objects have self referencing association between them.
    ''' Outcome: The detached objects will have DetachedClean state.
    ''' Their navigation properties referencing instances of the detached objects will be automatically resolved and set.
    ''' </summary>
    <TestMethod> _
    Public Sub Detach_DetachMultipleEntitiesWithSelfReference()
        Dim retrievedEmployees As IList(Of Employee)
        Dim retrievedEmployeesCount As Integer
        Dim detachedEmployees As IList(Of Employee)

        Using Me.context
            Me.context = New EntitiesModel()

            retrievedEmployees = context.Employees.ToList()
            retrievedEmployeesCount = retrievedEmployees.Count

            detachedEmployees = context.CreateDetachedCopy(Of Employee)(retrievedEmployees, Nothing).ToList()
        End Using

        For employeeIndex As Integer = 0 To retrievedEmployeesCount - 1
            Dim detachedEmployee As Employee = detachedEmployees(employeeIndex)
            Dim detachedEmployeeState As ObjectState = Me.GetObjectState(detachedEmployee)

            Dim respectiveManagedEmployee As Employee = retrievedEmployees(employeeIndex)

            Dim areSupervisorsTheSame As Boolean = Me.AreSupervisorsTheSame(detachedEmployee.Supervisor, respectiveManagedEmployee.Supervisor)

            Assert.AreEqual(ObjectState.DetachedClean, detachedEmployeeState)
            Assert.IsTrue(areSupervisorsTheSame)
            Assert.AreEqual(0, detachedEmployee.TaskAssignments.Count)
        Next
    End Sub

    ''' <summary>
    ''' Scenario: Detach an object.
    ''' Condition: The object which will be detached has some of its navigation properties loaded with a fetch strategy.
    ''' Outcome: The detached object will have DetachedClean state. Default fetch strategy will be used and 
    ''' its navigation properties will not be detached with it.
    ''' </summary>
    <TestMethod> _
    Public Sub Detach_ContextLevelFetchStrategyIsIgnored_DuringDetach_DefaultFetchStrategyIsApplied()
        Dim detachedEmployee As Employee

        Using Me.context
            Me.context = New EntitiesModel()

            Dim withSupervisorAndAssignments As New FetchStrategy()
            withSupervisorAndAssignments.LoadWith(Of Employee)(Function(emp) emp.Supervisor)
            withSupervisorAndAssignments.LoadWith(Of Employee)(Function(emp) emp.TaskAssignments)

            Me.context.FetchStrategy = withSupervisorAndAssignments

            Dim retrievedEmployee As Employee = context.Employees.FirstOrDefault(Function(emp) emp.Title = "Developer")

            detachedEmployee = context.CreateDetachedCopy(Of Employee)(retrievedEmployee)
        End Using

        Dim detachedEmployeeState As ObjectState = Me.GetObjectState(detachedEmployee)

        Assert.AreEqual(ObjectState.DetachedClean, detachedEmployeeState)
        Assert.IsNull(detachedEmployee.Supervisor)
        Assert.AreEqual(0, detachedEmployee.TaskAssignments.Count)
    End Sub

    ''' <summary>
    ''' Scenario: Detach an object.
    ''' Condition: The object which will be detached has some of its navigation properties loaded with a fetch strategy.
    ''' Another fetch strategy is applied when detaching.
    ''' Outcome: The detached object will have DetachedClean state. The context fetch strategy will be ignored during the detach
    ''' and only the detach fetch strategy will be applies.
    ''' </summary>
    <TestMethod> _
    Public Sub Detach_ContextLevelFetchStrategyIsIgnored_DuringDetach_DetachSpecificFetchStrategyIsApplied()
        Dim detachedEmployee As Employee
        Dim expectedTaskAssignmentsCount As Integer

        Using Me.context
            Me.context = New EntitiesModel()

            Dim contextFetchStrategy As New FetchStrategy()
            contextFetchStrategy.LoadWith(Of Employee)(Function(emp) emp.Supervisor)

            Me.context.FetchStrategy = contextFetchStrategy

            Dim retrievedEmployee As Employee = context.Employees.FirstOrDefault(Function(emp) emp.Title = "Developer")
            expectedTaskAssignmentsCount = retrievedEmployee.TaskAssignments.Count

            Dim detachFetchStrategy As New FetchStrategy()
            detachFetchStrategy.LoadWith(Of Employee)(Function(emp) emp.TaskAssignments)

            detachedEmployee = context.CreateDetachedCopy(Of Employee)(retrievedEmployee, detachFetchStrategy)
        End Using

        Dim detachedEmployeeState As ObjectState = Me.GetObjectState(detachedEmployee)

        Assert.AreEqual(ObjectState.DetachedClean, detachedEmployeeState)
        Assert.IsNull(detachedEmployee.Supervisor)
        Assert.AreEqual(expectedTaskAssignmentsCount, detachedEmployee.TaskAssignments.Count)
    End Sub

    ''' <summary>
    ''' Scenario: Detach an object from its context.
    ''' Conditions: Some of the navigation properties of the object should be loaded before the detach.
    ''' Outcome: The detached object will have DetachedClean state. Its navigation properties will not be detached with it.
    ''' </summary>
    <TestMethod> _
    Public Sub Detach_LoadedNavigationPropertiesAreIgnoredDuringDetach()
        Dim detachedEmployee As Employee

        Using Me.context
            Me.context = New EntitiesModel()

            Dim retrievedEmployee As Employee = context.Employees.FirstOrDefault(Function(emp) emp.Title = "Developer")
            Dim supervisor As Employee = retrievedEmployee.Supervisor
            Dim assignments As IList(Of TaskAssignment) = retrievedEmployee.TaskAssignments.ToList()

            detachedEmployee = context.CreateDetachedCopy(Of Employee)(retrievedEmployee)
        End Using

        Dim detachedEmployeeState As ObjectState = Me.GetObjectState(detachedEmployee)

        Assert.AreEqual(ObjectState.DetachedClean, detachedEmployeeState)
        Assert.IsNull(detachedEmployee.Supervisor)
        Assert.AreEqual(0, detachedEmployee.TaskAssignments.Count)
    End Sub

    ''' <summary>
    ''' Scenario: Detach a modified object from its context.
    ''' Outcome: The state of the detached object will be DetachedDirty.
    ''' </summary>
    <TestMethod> _
    Public Sub Detach_DetachModifiedObject()
        Dim detachedEmployee As Employee

        Using Me.context
            Me.context = New EntitiesModel()

            Dim retrievedEmployee As Employee = Me.context.Employees.FirstOrDefault(Function(emp) emp.Title = "Developer")
            retrievedEmployee.FirstName = "Mark"

            detachedEmployee = Me.context.CreateDetachedCopy(Of Employee)(retrievedEmployee)
        End Using

        Dim detachedEmployeeState As ObjectState = Me.GetObjectState(detachedEmployee)

        Assert.AreEqual(ObjectState.DetachedDirty, detachedEmployeeState)
    End Sub

    ''' <summary>
    ''' Scenario: Detach an object from its context.
    ''' Condition: modify the detached object.
    ''' Outcome: The context from which the object was detached will not have any changes.
    ''' </summary>
    <TestMethod> _
    Public Sub Detach_ModifyDetachedObject()
        Dim detachedEmployee As Employee
        Dim contextHasChangesAfterModifyingDetachedObject As Boolean

        Using Me.context
            Me.context = New EntitiesModel()

            Dim retrievedEmployee As Employee = Me.context.Employees.FirstOrDefault(Function(emp) emp.Title = "Developer")

            detachedEmployee = Me.context.CreateDetachedCopy(Of Employee)(retrievedEmployee)
            detachedEmployee.FirstName = "Mark"

            contextHasChangesAfterModifyingDetachedObject = Me.context.HasChanges
        End Using

        Assert.IsFalse(contextHasChangesAfterModifyingDetachedObject)
    End Sub

    ''' <summary>
    ''' Scenario: Detach an object marked for deletion from its context.
    ''' Condition: The object is not yet actually deleted from the database.
    ''' Outcome: An InvalidOperationException will be thrown.
    ''' </summary>
    <TestMethod> _
    Public Sub Detach_DetachObjectMarkedForDeletion()
        Using Me.context
            Me.context = New EntitiesModel()

            Dim retrievedEmployee As Employee = Me.context.Employees.FirstOrDefault(Function(emp) emp.Title = "Developer")
            Me.context.Delete(retrievedEmployee)

            AssertException.Throws(Of Telerik.OpenAccess.Exceptions.InvalidOperationException)(Function() Me.context.CreateDetachedCopy(Of Employee)(retrievedEmployee))
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: Detach a newly added object from its context.
    ''' Condition: The object is not yet persisted in the database.
    ''' Outcome: A InvalidOperationException will be thrown.
    ''' </summary>
    <TestMethod> _
    Public Sub Detach_DetachNewObject()
        Dim newEmployee As New Employee() With _
        { _
            .EmployeeId = 1337, _
            .FirstName = "Mad", _
            .LastName = "Jack" _
        }

        Using Me.context
            Me.context = New EntitiesModel()

            Me.context.Add(newEmployee)

            AssertException.Throws(Of Telerik.OpenAccess.Exceptions.InvalidOperationException)(Function() Me.context.CreateDetachedCopy(Of Employee)(newEmployee))
        End Using
    End Sub

    'Check if the passed Employee instances can be considered the same.
    Protected Function AreSupervisorsTheSame(emp1 As Employee, emp2 As Employee) As Boolean

        If emp1 Is Nothing AndAlso emp2 Is Nothing Then
            Return True
        ElseIf emp1 Is Nothing OrElse emp2 Is Nothing Then
            Return False
        Else
            Return emp1.EmployeeId.Equals(emp2.EmployeeId)
        End If
    End Function

End Class