Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports ContextApi.Model
Imports System.IO
Imports Telerik.OpenAccess.FetchOptimization
Imports Telerik.OpenAccess
Imports System.Linq.Expressions

<TestClass()> _
Public Class FetchStrategyTests
    Inherits UnitTestsBase

    Private Const SQL_TO_LOAD_TASK_ASSIGNMENTS_WITH_FETCH_STRATEGY As String = _
                "SELECT [TaskAssignmentId] AS COL1, [EmployeeId] AS COL2, [EmployeeId] AS COL3, " + _
                "[TaskId] AS COL4, [TaskId] AS COL5, [WorkingHours] AS COL6 FROM [TaskAssignments] " + _
                "WHERE [EmployeeId] = @p0"

    Private Const SQL_TO_LOAD_TASK_ASSIGNMENTS_FROM_NAVIGATION_PROPERTY As String = _
                "SELECT b.[TaskAssignmentId] AS COL1, b.[EmployeeId] AS COL2, b.[EmployeeId] " + _
                "AS COL3, b.[TaskId] AS COL4, b.[TaskId] AS COL5, b.[WorkingHours] AS COL6 FROM " + _
                "[Employees] a LEFT JOIN [TaskAssignments] AS b ON (a.[EmployeeId] = b.[EmployeeId]) WHERE a.[EmployeeId] = @p0"

    Private context As EntitiesModel

    <TestInitialize> _
    Public Overrides Sub TestInitialize()
        MyBase.TestInitialize()

        Me.context = New EntitiesModel()
        Me.ReinitializeContextLog()
    End Sub

    <TestCleanup> _
    Public Overrides Sub TestCleanup()
        MyBase.TestCleanup()

        If Me.context IsNot Nothing Then
            Me.context.Dispose()
        End If
    End Sub

    ''' <summary>
    ''' Scenario: Load an Employee and access its TaskAssignments.
    ''' Condition: No fetch strategy is used.
    ''' Outcome: The TaskAssignments will be loaded on demand when accessed - a SQL statement is generated.
    ''' </summary>
    <TestMethod> _
    Public Sub NoFetchStrategy_TaskAssignmentsAreLazyLoaded()
        Dim retrievedEmployee As Employee = context.Employees.FirstOrDefault(Function(emp) emp.Title = "Developer")
        Dim sqlGeneratedOnRetrievingEmployee As String = Me.GetGeneratedSql()

        Dim taskAssignment As TaskAssignment = retrievedEmployee.TaskAssignments.FirstOrDefault()
        Dim sqlGeneratedOnAccessingTaskAssignments As String = Me.GetGeneratedSql()

        Dim areTaskAssignmentsEagerlyLoaded As Boolean = sqlGeneratedOnRetrievingEmployee.Contains(SQL_TO_LOAD_TASK_ASSIGNMENTS_WITH_FETCH_STRATEGY)
        Dim areTaskAssignmentsLazyLoaded As Boolean = sqlGeneratedOnAccessingTaskAssignments.Contains(SQL_TO_LOAD_TASK_ASSIGNMENTS_FROM_NAVIGATION_PROPERTY)

        Assert.IsFalse(areTaskAssignmentsEagerlyLoaded)
        Assert.IsTrue(areTaskAssignmentsLazyLoaded)
    End Sub

    ''' <summary>
    ''' Scenario: Load an Employee together with its TaskAssignments. Access its TaskAssignments.
    ''' Condition: A context level fetch strategy is used. The navigation properties which are to be eagerly loaded
    ''' are specified with by the LoadWith method using expressions.
    ''' Outcome: The TaskAssignments will be loaded at the same time when the employee is retrieved. 
    ''' When accessing the TaskAssignments property no SQL statement will be generated.
    ''' </summary>
    <TestMethod> _
    Public Sub FetchStrategy_PerContext_UseLoadWithGeneric()
        Dim withTaskAssignments As New FetchStrategy()
        withTaskAssignments.LoadWith(Of Employee)(Function(emp) emp.TaskAssignments)
        Me.context.FetchStrategy = withTaskAssignments

        Dim retrievedEmployee As Employee = context.Employees.FirstOrDefault(Function(emp) emp.Title = "Developer")
        Dim sqlGeneratedOnRetrievingEmployee As String = Me.GetGeneratedSql()

        Dim taskAssignment As TaskAssignment = retrievedEmployee.TaskAssignments.FirstOrDefault()
        Dim sqlGeneratedOnAccessingTaskAssignments As String = Me.GetGeneratedSql()

        Dim areTaskAssignmentsEagerlyLoaded As Boolean = sqlGeneratedOnRetrievingEmployee.Contains(SQL_TO_LOAD_TASK_ASSIGNMENTS_WITH_FETCH_STRATEGY)

        Assert.IsTrue(areTaskAssignmentsEagerlyLoaded)
        Assert.AreEqual(String.Empty, sqlGeneratedOnAccessingTaskAssignments)
    End Sub

    ''' <summary>
    ''' Scenario: Load an Employee together with its TaskAssignments using context level fetch strategy. Access its TaskAssignments.
    ''' Condition: Non generic LoadWith method is used. The properties which should be eagerly loaded are specified by expression.
    ''' Outcome: The TaskAssignments will be loaded at the same time the employee is retrieved.
    ''' When accessing the TaskAssignments property no SQL statement will be generated.
    ''' </summary>
    <TestMethod> _
    Public Sub FetchStrategy_PerContext_UseLoadWith()
        Dim withTaskAssignments As New FetchStrategy()
        withTaskAssignments.LoadWith(Function(emp As Employee) emp.TaskAssignments)
        Me.context.FetchStrategy = withTaskAssignments

        Dim retrievedEmployee As Employee = context.Employees.FirstOrDefault(Function(emp) emp.Title = "Developer")
        Dim sqlGeneratedOnRetrievingEmployee As String = Me.GetGeneratedSql()

        Dim taskAssignment As TaskAssignment = retrievedEmployee.TaskAssignments.FirstOrDefault()
        Dim sqlGeneratedOnAccessingTaskAssignments As String = Me.GetGeneratedSql()

        Dim areTaskAssignmentsEagerlyLoaded As Boolean = sqlGeneratedOnRetrievingEmployee.Contains(SQL_TO_LOAD_TASK_ASSIGNMENTS_WITH_FETCH_STRATEGY)

        Assert.IsTrue(areTaskAssignmentsEagerlyLoaded)
        Assert.AreEqual(String.Empty, sqlGeneratedOnAccessingTaskAssignments)
    End Sub

    ''' <summary>
    ''' Scenario: Load an Employee together with its TaskAssignments using context level fetch strategy. Access its TaskAssignments property.
    ''' Condition: Non generic LoadWith method is used where the properties which should be eagerly loaded are specified by a lambda expression.
    ''' Outcome: The TaskAssignments will be loaded at the same time the employee is retrieved.
    ''' When accessing the TaskAssignments property no SQL statement will be generated.
    ''' </summary>
    <TestMethod> _
    Public Sub FetchStrategy_PerContext_UseLoadWith_LambdaExpressionParameter()
        Dim param As ParameterExpression = Expression.Parameter(GetType(Employee), "param")
        Dim lexWithTaskAssignments As LambdaExpression = Expression.Lambda(Expression.MakeMemberAccess(param, param.Type.GetProperty("TaskAssignments")), param)

        Dim withTaskAssignments As New FetchStrategy()
        withTaskAssignments.LoadWith(lexWithTaskAssignments)
        Me.context.FetchStrategy = withTaskAssignments

        Dim retrievedEmployee As Employee = context.Employees.FirstOrDefault(Function(emp) emp.Title = "Developer")
        Dim sqlGeneratedOnRetrievingEmployee As String = Me.GetGeneratedSql()

        Dim taskAssignment As TaskAssignment = retrievedEmployee.TaskAssignments.FirstOrDefault()
        Dim sqlGeneratedOnAccessingTaskAssignments As String = Me.GetGeneratedSql()

        Dim areTaskAssignmentsEagerlyLoaded As Boolean = sqlGeneratedOnRetrievingEmployee.Contains(SQL_TO_LOAD_TASK_ASSIGNMENTS_WITH_FETCH_STRATEGY)

        Assert.IsTrue(areTaskAssignmentsEagerlyLoaded)
        Assert.AreEqual(String.Empty, sqlGeneratedOnAccessingTaskAssignments)
    End Sub

    ''' <summary>
    ''' Scenario: Load an Employee together with several of its related objects in depth. Access the related objects specified in the fetch strategy.
    ''' Condition: Context level fetch strategy is used. The properties which should be eagerly loaded are specified using the LoadWitn(Of T) method by an expression.
    ''' Outcome: The specified related objects will all be loaded at the same time the employee is retrieved, therefore no SQL is generated
    ''' when accessing them.
    ''' </summary>
    <TestMethod> _
    Public Sub FetchStrategy_PerContext_LoadingInDepth()
        Dim upToProject As New FetchStrategy()
        upToProject.LoadWith(Of Employee)(Function(emp) emp.TaskAssignments)
        upToProject.LoadWith(Of TaskAssignment)(Function(asignment) asignment.Task)
        upToProject.LoadWith(Of Task)(Function(task) task.Project)

        Me.context.FetchStrategy = upToProject

        Dim retrievedEmployee As Employee = context.Employees.FirstOrDefault(Function(emp) emp.Title = "Developer")

        Me.ReinitializeContextLog()

        Dim retrievedTaskAssgnment As TaskAssignment = retrievedEmployee.TaskAssignments.FirstOrDefault()
        Dim sqlGeneratedOnAccessingTaskAssignments As String = Me.GetGeneratedSql()

        Dim retrievedTask As Task = retrievedTaskAssgnment.Task
        Dim sqlGeneratedOnAccessingTask As String = Me.GetGeneratedSql()

        Dim retrievedProject As Project = retrievedTask.Project
        Dim sqlGeneratedOnAccessingProject As String = Me.GetGeneratedSql()

        Assert.AreEqual(String.Empty, sqlGeneratedOnAccessingTaskAssignments)
        Assert.AreEqual(String.Empty, sqlGeneratedOnAccessingTask)
        Assert.AreEqual(String.Empty, sqlGeneratedOnAccessingProject)
    End Sub

    ''' <summary>
    ''' Scenario: Load an Employee together with several of its related objects in depth. Access the objects specified in the fetch strategy.
    ''' Condition: Use a context level fetch strategy. Use the MaxFetchDepth property to set the maximum depth of the properties which are to be eagerly loaded.
    ''' Outcome: The specified related objects up to the MaxFetchDepth, will all be loaded at the same time the Employee is retrieved, therefore no SQL is generated
    ''' when accessing them. When a related object with higher depth is accessed, SQL statement(s) will be generated in order to load it.
    ''' </summary>
    <TestMethod> _
    Public Sub FetchStrategy_PerContext_LoadingInDepth_UsingMaxFetchDepth()
        Dim upToTask As New FetchStrategy()
        upToTask.LoadWith(Of Employee)(Function(emp) emp.TaskAssignments)
        upToTask.LoadWith(Of TaskAssignment)(Function(asignment) asignment.Task)
        upToTask.LoadWith(Of Task)(Function(task) task.Project)
        upToTask.MaxFetchDepth = 2

        Me.context.FetchStrategy = upToTask

        Dim retrievedEmployee As Employee = context.Employees.FirstOrDefault(Function(emp) emp.Title = "Developer")

        ReinitializeContextLog()

        Dim retrievedTaskAssgnment As TaskAssignment = retrievedEmployee.TaskAssignments.FirstOrDefault()
        Dim sqlGeneratedOnAccessingTaskAssignments As String = Me.GetGeneratedSql()

        Dim retrievedTask As Task = retrievedTaskAssgnment.Task
        Dim sqlGeneratedOnAccessingTask As String = Me.GetGeneratedSql()

        Dim retrievedProject As Project = retrievedTask.Project
        Dim sqlGeneratedOnAccessingProject As String = Me.GetGeneratedSql()

        Assert.AreEqual(String.Empty, sqlGeneratedOnAccessingTaskAssignments)
        Assert.AreEqual(String.Empty, sqlGeneratedOnAccessingTask)
        Assert.AreNotEqual(String.Empty, sqlGeneratedOnAccessingProject)
    End Sub

    ''' <summary>
    ''' Scenario: Load an Employee together with several directly related objects. Access the related objects
    ''' specified in the fetch strategy.
    ''' Condition: Use a context level fetch strategy.
    ''' Outcome: The related objects specified in the fetch strategy will be loaded at the same time the employee is retrieved. No SQL will be
    ''' generated when accessing them.
    ''' </summary>
    <TestMethod> _
    Public Sub FetchStrategy_PerContext_LoadingInWidth()
        Dim loadSupervisorAndAssignment As New FetchStrategy()
        loadSupervisorAndAssignment.LoadWith(Of Employee)(Function(emp) emp.Supervisor)
        loadSupervisorAndAssignment.LoadWith(Of Employee)(Function(emp) emp.TaskAssignments)

        Me.context.FetchStrategy = loadSupervisorAndAssignment

        Dim retrievedEmployee As Employee = Me.context.Employees.FirstOrDefault(Function(emp) emp.Title = "Developer")

        ReinitializeContextLog()

        Dim employeeSupervisor As Employee = retrievedEmployee.Supervisor
        Dim sqlGeneratedOnAccessingSupervisor As String = Me.GetGeneratedSql()

        Dim employeeAssignment As TaskAssignment = retrievedEmployee.TaskAssignments.FirstOrDefault()
        Dim sqlGeneratedOnAccessingTaskAssignments As String = Me.GetGeneratedSql()

        Assert.AreEqual(String.Empty, sqlGeneratedOnAccessingSupervisor)
        Assert.AreEqual(String.Empty, sqlGeneratedOnAccessingTaskAssignments)
    End Sub

    ''' <summary>
    ''' Scenario: Load an Employee together with its TaskAssignments.  Access its TaskAssignments.
    ''' Condition: Use a query level fetch strategy with the Include method. The properties which are to be 
    ''' eagerly loaded are specified with an expression.
    ''' Outcome: TaskAssignments will be loaded at the same time the employee is retrieved. No SQL will be generated when accessing them.
    ''' </summary>
    <TestMethod> _
    Public Sub FetchStrategy_PerQuery_UsingInclude()
        Dim retrievedEmployee As Employee = Me.context.Employees.Include(Function(emp) emp.TaskAssignments).FirstOrDefault(Function(emp) emp.Title = "Developer")
        Dim sqlGeneratedOnRetrievingEmployee As String = Me.GetGeneratedSql()

        Dim taskAssignment As TaskAssignment = retrievedEmployee.TaskAssignments.FirstOrDefault()
        Dim sqlGeneratedOnAccessingTaskAssignments As String = Me.GetGeneratedSql()

        Dim areTaskAssignmentsEagerlyLoaded As Boolean = sqlGeneratedOnRetrievingEmployee.Contains(SQL_TO_LOAD_TASK_ASSIGNMENTS_WITH_FETCH_STRATEGY)

        Assert.IsTrue(areTaskAssignmentsEagerlyLoaded)
        Assert.AreEqual(String.Empty, sqlGeneratedOnAccessingTaskAssignments)
    End Sub

    ''' <summary>
    ''' Scenario: Load an Employee together with several related objects in depth. Access the navigation properties specified in the fetch strategy.
    ''' Condition: Use a query level fetch strategy. Specify the properties which are to be eagerly loaded with the Include method using expressions.
    ''' Outcome: The related objects specified in the Include method will be loaded at the same time the Employee is retrieved. No SQL will be 
    ''' generated when accessing them.
    ''' </summary>
    <TestMethod> _
    Public Sub FetchStrategy_PerQuery_UsingInclude_LoadingInDepth()
        Dim retrievedEmployee As Employee = Me.context.Employees.Include(Function(emp) emp.TaskAssignments.[Select](Function(assignment) assignment.Task)).FirstOrDefault(Function(emp) emp.Title = "Developer")

        ReinitializeContextLog()

        Dim employeeAssignment As TaskAssignment = retrievedEmployee.TaskAssignments.FirstOrDefault()
        Dim sqlGeneratedOnAccessingTaskAssignments As String = Me.GetGeneratedSql()

        Dim employeeAssignmentTask As Task = employeeAssignment.Task
        Dim sqlGeneratedOnAccessingTask As String = Me.GetGeneratedSql()

        Assert.AreEqual(String.Empty, sqlGeneratedOnAccessingTaskAssignments)
        Assert.AreEqual(String.Empty, sqlGeneratedOnAccessingTask)
    End Sub

    ''' <summary>
    ''' Scenario: Load an Employee together with several directly related objects. Access the navigation properties specified in the fetch strategy.
    ''' Condition: Use a query level fetch strategy. Specify the properties which are to be eagerly loaded with the Include method using expressions.
    ''' Outcome The related objects specified in the Include method strategy will be loaded at the same time the employee is retrieved.
    ''' No SQL will be generated when accessing them.
    ''' </summary>
    <TestMethod> _
    Public Sub FetchStrategy_PerQuery_UsingInclude_LoadingInWidth()
        Dim retrievedEmployee As Employee = Me.context.Employees.Include(Function(emp) emp.TaskAssignments).Include(Function(emp) emp.Supervisor).FirstOrDefault(Function(emp) emp.Title = "Developer")

        ReinitializeContextLog()

        Dim employeeAssignment As TaskAssignment = retrievedEmployee.TaskAssignments.FirstOrDefault()
        Dim sqlGeneratedOnAccessingTaskAssignments As String = Me.GetGeneratedSql()

        Dim employeeSupervisor As Employee = retrievedEmployee.Supervisor
        Dim sqlGeneratedOnAccessingSupervisor As String = Me.GetGeneratedSql()

        Assert.AreEqual(String.Empty, sqlGeneratedOnAccessingTaskAssignments)
        Assert.AreEqual(String.Empty, sqlGeneratedOnAccessingSupervisor)
    End Sub

    ''' <summary>
    ''' Scenario: Load an Employee together with its TaskAssignments. Access its TaskAssignments property.
    ''' Condition: Use a query level fetch strategy. The navigation properties which are to be eagerly loaded are specified by passing a fetch strategy
    ''' to the LoadWith method.
    ''' Outcome: The TaskAssignments property will be loaded at the same time the employee is retrieved. No SQL will be generated when 
    ''' accessing it.
    ''' </summary>
    <TestMethod> _
    Public Sub FetchStrategy_PerQuery_UsingLoadWith()
        Dim withTaskAssignments As New FetchStrategy()
        withTaskAssignments.LoadWith(Of Employee)(Function(emp) emp.TaskAssignments)

        Dim retrievedEmployee As Employee = Me.context.Employees.LoadWith(withTaskAssignments).FirstOrDefault(Function(emp) emp.Title = "Developer")
        Dim sqlGeneratedOnRetrievingEmployee As String = Me.GetGeneratedSql()

        Dim employeeAssignment As TaskAssignment = retrievedEmployee.TaskAssignments.FirstOrDefault()
        Dim sqlGeneratedOnAccessingTaskAssignments As String = Me.GetGeneratedSql()

        Dim areTaskAssignmentsEagerlyLoaded As Boolean = sqlGeneratedOnRetrievingEmployee.Contains(SQL_TO_LOAD_TASK_ASSIGNMENTS_WITH_FETCH_STRATEGY)

        Assert.IsTrue(areTaskAssignmentsEagerlyLoaded)
        Assert.AreEqual(String.Empty, sqlGeneratedOnAccessingTaskAssignments)
    End Sub

    ''' <summary>
    ''' Scenario: Load an Employee together with several of its related objects in depth.
    ''' Condition: Query level fetch strategy is used. The navigation properties which are to be eagerly loaded are specified by passing a fetch strategy
    ''' to the LoadWith method.
    ''' Outcome: The related objects specified in the fetch strategy will be loaded at the same time the employee is retrieved.
    ''' No SQL will be generated when accessing them.
    ''' </summary>
    <TestMethod> _
    Public Sub FetchStrategy_PerQuery_UsingLoadWith_LoadingInDepth()
        Dim upToProject As New FetchStrategy()
        upToProject.LoadWith(Of Employee)(Function(emp) emp.TaskAssignments)
        upToProject.LoadWith(Of TaskAssignment)(Function(emp) emp.Task)
        upToProject.LoadWith(Of Task)(Function(task) task.Project)

        Dim retrievedEmployee As Employee = Me.context.Employees.LoadWith(upToProject).FirstOrDefault(Function(emp) emp.Title = "Developer")

        ReinitializeContextLog()

        Dim employeeAssignment As TaskAssignment = retrievedEmployee.TaskAssignments.FirstOrDefault()
        Dim sqlGeneratedOnAccessingTaskAssignments As String = Me.GetGeneratedSql()

        Dim assignedTask As Task = employeeAssignment.Task
        Dim sqlGeneratedOnAccessingTask As String = Me.GetGeneratedSql()

        Dim taskProject As Project = assignedTask.Project
        Dim sqlGeneratedOnAccessingProject As String = Me.GetGeneratedSql()

        Assert.AreEqual(String.Empty, sqlGeneratedOnAccessingTaskAssignments)
        Assert.AreEqual(String.Empty, sqlGeneratedOnAccessingTask)
        Assert.AreEqual(String.Empty, sqlGeneratedOnAccessingProject)
    End Sub

    ''' <summary>
    ''' Scenario: Load an Employee together with several of its related objects in depth.
    ''' Condition: Query level fetch strategy is used. The navigation properties which are to be eagerly loaded are specified by passing a fetch strategy to the LoadWith method.
    ''' The MaxFetchDepth property is used to limit the depth of the eagerly loaded navigation properties.
    ''' Outcome: the related objects up to the specified depth will be loaded at the same time the employee is retrieved. No SQL will be generated
    ''' when accessing them. When accessing related objects with higher depth, SQL statement(s) will be generated in order to retrieve them.
    ''' </summary>
    <TestMethod> _
    Public Sub FetchStrategy_PerQuery_UsingLoadWith_InDepth_MaxFetchDepth()
        Dim upToTask As New FetchStrategy()
        upToTask.LoadWith(Of Employee)(Function(emp) emp.TaskAssignments)
        upToTask.LoadWith(Of TaskAssignment)(Function(emp) emp.Task)
        upToTask.LoadWith(Of Task)(Function(task) task.Project)
        upToTask.MaxFetchDepth = 2

        Dim retrievedEmployee As Employee = Me.context.Employees.LoadWith(upToTask).FirstOrDefault(Function(emp) emp.Title = "Developer")

        ReinitializeContextLog()

        Dim employeeAssignment As TaskAssignment = retrievedEmployee.TaskAssignments.FirstOrDefault()
        Dim sqlGeneratedOnAccessingTaskAssignments As String = Me.GetGeneratedSql()

        Dim assignedTask As Task = employeeAssignment.Task
        Dim sqlGeneratedOnAccessingTask As String = Me.GetGeneratedSql()

        Dim taskProject As Project = assignedTask.Project
        Dim sqlGeneratedOnAccessingProject As String = Me.GetGeneratedSql()

        Assert.AreEqual(String.Empty, sqlGeneratedOnAccessingTaskAssignments)
        Assert.AreEqual(String.Empty, sqlGeneratedOnAccessingTask)
        Assert.AreNotEqual(String.Empty, sqlGeneratedOnAccessingProject)
    End Sub

    ''' <summary>
    ''' Scenario: Load an Employee together with several of its directly related object. 
    ''' Condition: Query level fetch strategy is used. The navigation properties which are to be eagerly loaded are specified by passing a fetch strategy to
    ''' the LoadWith method.
    ''' Outcome: the related objects will be loaded at the same time the Employee is retrieved. No SQL will be generated when accessing them
    ''' </summary>
    <TestMethod> _
    Public Sub FetchStrategy_PerQuery_UsingLoadWith_LoadingInWidth()
        Dim loadSupervisorAndAssignment As New FetchStrategy()
        loadSupervisorAndAssignment.LoadWith(Of Employee)(Function(emp) emp.Supervisor)
        loadSupervisorAndAssignment.LoadWith(Of Employee)(Function(emp) emp.TaskAssignments)

        Dim retrievedEmployee As Employee = Me.context.Employees.LoadWith(loadSupervisorAndAssignment).FirstOrDefault(Function(emp) emp.Title = "Developer")

        ReinitializeContextLog()

        Dim employeeAssignment As TaskAssignment = retrievedEmployee.TaskAssignments.FirstOrDefault()
        Dim sqlGeneratedOnAccessingTaskAssignments As String = Me.GetGeneratedSql()

        Dim employeeSupervisor As Employee = retrievedEmployee.Supervisor
        Dim sqlGeneratedOnAccessingSupervisor As String = Me.GetGeneratedSql()

        Assert.AreEqual(String.Empty, sqlGeneratedOnAccessingTaskAssignments)
        Assert.AreEqual(String.Empty, sqlGeneratedOnAccessingSupervisor)
    End Sub

    ''' <summary>
    ''' Scenario: Use a query level fetch strategy together with a context level fetch strategy. 
    ''' Outcome: The query level fetch strategy will override the context level fetch strategy. The objects specified in the context level fetch strategy
    ''' will not be eagerly loaded.
    ''' </summary>
    <TestMethod> _
    Public Sub FetchStrategy_PerQuery_OrverridesFetchStrategyPerContext()
        Dim upToProject As New FetchStrategy()
        upToProject.LoadWith(Of Employee)(Function(emp) emp.TaskAssignments)
        upToProject.LoadWith(Of TaskAssignment)(Function(emp) emp.Task)

        Me.context.FetchStrategy = upToProject

        Dim retrievedEmployee As Employee = Me.context.Employees.Include(Function(emp) emp.TaskAssignments).FirstOrDefault(Function(emp) emp.Title = "Developer")

        ReinitializeContextLog()

        Dim employeeAssignment As TaskAssignment = retrievedEmployee.TaskAssignments.FirstOrDefault()
        Dim sqlGeneratedOnAccessingTaskAssignments As String = Me.GetGeneratedSql()

        Dim assignedTask As Task = employeeAssignment.Task
        Dim sqlGeneratedOnAccessingTask As String = Me.GetGeneratedSql()

        Assert.AreEqual(String.Empty, sqlGeneratedOnAccessingTaskAssignments)
        Assert.AreNotEqual(String.Empty, sqlGeneratedOnAccessingTask)
    End Sub

    'Retrieves the SQL statements in the log of the context.
    Private Function GetGeneratedSql() As String
        If Me.context IsNot Nothing Then
            Dim generatedSql As String = context.Log.ToString()
            Me.ReinitializeContextLog()
            Return generatedSql
        Else
            Throw New NullReferenceException("The context object is null")
        End If
    End Function

    'Reinitializes the context log in order to remove left over SQL statements.
    Private Sub ReinitializeContextLog()
        If context IsNot Nothing Then
            Me.context.Log = Nothing
            Me.context.Log = New StringWriter()
        Else
            Throw New NullReferenceException("The context object is null")
        End If
    End Sub

End Class