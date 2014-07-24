using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using ContextApi.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.OpenAccess;
using Telerik.OpenAccess.FetchOptimization;

namespace ContextApi.Tests.Fetch_Strategies
{
    [TestClass]
    public class FetchStrategyTests : UnitTestsBase
    {
        private const String SQL_TO_LOAD_TASK_ASSIGNMENTS_WITH_FETCH_STRATEGY =
                "SELECT [TaskAssignmentId] AS COL1, [EmployeeId] AS COL2, [EmployeeId] AS COL3, " +
                "[TaskId] AS COL4, [TaskId] AS COL5, [WorkingHours] AS COL6 FROM [TaskAssignments] " +
                "WHERE [EmployeeId] = @p0";

        private const String SQL_TO_LOAD_TASK_ASSIGNMENTS_FROM_NAVIGATION_PROPERTY =
                "SELECT b.[TaskAssignmentId] AS COL1, b.[EmployeeId] AS COL2, b.[EmployeeId] " +
                "AS COL3, b.[TaskId] AS COL4, b.[TaskId] AS COL5, b.[WorkingHours] AS COL6 FROM " +
                "[Employees] a LEFT JOIN [TaskAssignments] AS b ON (a.[EmployeeId] = b.[EmployeeId]) WHERE a.[EmployeeId] = @p0";


        private EntitiesModel context;

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();

            this.context = new EntitiesModel();
            this.ReinitializeContextLog();
        }

        [TestCleanup]
        public override void TestCleanup()
        {
            base.TestCleanup();

            if (this.context != null)
            {
                this.context.Dispose();
            }
        }

        /// <summary>
        /// Scenario: Load an Employee and access its TaskAssignments.
        /// Condition: No fetch strategy is used.
        /// Outcome: The TaskAssignments will be loaded on demand when accessed - a SQL statement is generated.
        /// </summary>
        [TestMethod]
        public void NoFetchStrategy_TaskAssignmentsAreLazyLoaded()
        {
            Employee retrievedEmployee = context.Employees.FirstOrDefault(emp => emp.Title == "Developer");
            string sqlGeneratedOnRetrievingEmployee = this.GetGeneratedSql();

            TaskAssignment taskAssignment = retrievedEmployee.TaskAssignments.FirstOrDefault();
            string sqlGeneratedOnAccessingTaskAssignments = this.GetGeneratedSql();

            bool areTaskAssignmentsEagerlyLoaded = sqlGeneratedOnRetrievingEmployee.Contains(SQL_TO_LOAD_TASK_ASSIGNMENTS_WITH_FETCH_STRATEGY);
            bool areTaskAssignmentsLazyLoaded = sqlGeneratedOnAccessingTaskAssignments.Contains(SQL_TO_LOAD_TASK_ASSIGNMENTS_FROM_NAVIGATION_PROPERTY);

            Assert.IsFalse(areTaskAssignmentsEagerlyLoaded);
            Assert.IsTrue(areTaskAssignmentsLazyLoaded);
        }

        /// <summary>
        /// Scenario: Load an Employee together with its TaskAssignments. Access its TaskAssignments.
        /// Condition: A context level fetch strategy is used. The navigation properties which are to be eagerly loaded
        /// are specified with by the LoadWith method using expressions.
        /// Outcome: The TaskAssignments will be loaded at the same time when the employee is retrieved. 
        /// When accessing the TaskAssignments property no SQL statement will be generated.
        /// </summary>
        [TestMethod]
        public void FetchStrategy_PerContext_UseLoadWithGeneric()
        {
            FetchStrategy withTaskAssignments = new FetchStrategy();
            withTaskAssignments.LoadWith<Employee>(emp => emp.TaskAssignments);
            this.context.FetchStrategy = withTaskAssignments;

            Employee retrievedEmployee = context.Employees.FirstOrDefault(emp => emp.Title == "Developer");
            string sqlGeneratedOnRetrievingEmployee = this.GetGeneratedSql();

            TaskAssignment taskAssignment = retrievedEmployee.TaskAssignments.FirstOrDefault();
            string sqlGeneratedOnAccessingTaskAssignments = this.GetGeneratedSql();

            bool areTaskAssignmentsEagerlyLoaded = sqlGeneratedOnRetrievingEmployee.Contains(SQL_TO_LOAD_TASK_ASSIGNMENTS_WITH_FETCH_STRATEGY);

            Assert.IsTrue(areTaskAssignmentsEagerlyLoaded);
            Assert.AreEqual(string.Empty, sqlGeneratedOnAccessingTaskAssignments);
        }

        /// <summary>
        /// Scenario: Load an Employee together with its TaskAssignments using context level fetch strategy. Access its TaskAssignments.
        /// Condition: Non generic LoadWith method is used. The properties which should be eagerly loaded are specified by expression.
        /// Outcome: The TaskAssignments will be loaded at the same time the employee is retrieved.
        /// When accessing the TaskAssignments property no SQL statement will be generated.
        /// </summary>
        [TestMethod]
        public void FetchStrategy_PerContext_UseLoadWith()
        {
            FetchStrategy withTaskAssignments = new FetchStrategy();
            withTaskAssignments.LoadWith((Employee emp) => emp.TaskAssignments);
            this.context.FetchStrategy = withTaskAssignments;

            Employee retrievedEmployee = context.Employees.FirstOrDefault(emp => emp.Title == "Developer");
            string sqlGeneratedOnRetrievingEmployee = this.GetGeneratedSql();

            TaskAssignment taskAssignment = retrievedEmployee.TaskAssignments.FirstOrDefault();
            string sqlGeneratedOnAccessingTaskAssignments = this.GetGeneratedSql();

            bool areTaskAssignmentsEagerlyLoaded = sqlGeneratedOnRetrievingEmployee.Contains(SQL_TO_LOAD_TASK_ASSIGNMENTS_WITH_FETCH_STRATEGY);

            Assert.IsTrue(areTaskAssignmentsEagerlyLoaded);
            Assert.AreEqual(string.Empty, sqlGeneratedOnAccessingTaskAssignments);
        }

        /// <summary>
        /// Scenario: Load an Employee together with its TaskAssignments using context level fetch strategy. Access its TaskAssignments property.
        /// Condition: Non generic LoadWith method is used where the properties which should be eagerly loaded are specified by a lambda expression.
        /// Outcome: The TaskAssignments will be loaded at the same time the employee is retrieved.
        /// When accessing the TaskAssignments property no SQL statement will be generated.
        /// </summary>
        [TestMethod]
        public void FetchStrategy_PerContext_UseLoadWith_LambdaExpressionParameter()
        {
            ParameterExpression param = Expression.Parameter(typeof(Employee), "param");
            LambdaExpression lexWithTaskAssignments = Expression.Lambda(Expression.MakeMemberAccess(param, param.Type.GetProperty("TaskAssignments")), param);

            FetchStrategy withTaskAssignments = new FetchStrategy();
            withTaskAssignments.LoadWith(lexWithTaskAssignments);
            this.context.FetchStrategy = withTaskAssignments;

            Employee retrievedEmployee = context.Employees.FirstOrDefault(emp => emp.Title == "Developer");
            string sqlGeneratedOnRetrievingEmployee = this.GetGeneratedSql();

            TaskAssignment taskAssignment = retrievedEmployee.TaskAssignments.FirstOrDefault();
            string sqlGeneratedOnAccessingTaskAssignments = this.GetGeneratedSql();

            bool areTaskAssignmentsEagerlyLoaded = sqlGeneratedOnRetrievingEmployee.Contains(SQL_TO_LOAD_TASK_ASSIGNMENTS_WITH_FETCH_STRATEGY);

            Assert.IsTrue(areTaskAssignmentsEagerlyLoaded);
            Assert.AreEqual(string.Empty, sqlGeneratedOnAccessingTaskAssignments);
        }

        /// <summary>
        /// Scenario: Load an Employee together with several of its related objects in depth. Access the related objects specified in the fetch strategy.
        /// Condition: Context level fetch strategy is used. The properties which should be eagerly loaded are specified using the LoadWitn<T> method by an expression.
        /// Outcome: The specified related objects will all be loaded at the same time the employee is retrieved, therefore no SQL is generated
        /// when accessing them.
        /// </summary>
        [TestMethod]
        public void FetchStrategy_PerContext_LoadingInDepth()
        {
            FetchStrategy upToProject = new FetchStrategy();
            upToProject.LoadWith<Employee>(emp => emp.TaskAssignments);
            upToProject.LoadWith<TaskAssignment>(asignment => asignment.Task);
            upToProject.LoadWith<Task>(task => task.Project);

            this.context.FetchStrategy = upToProject;

            Employee retrievedEmployee = context.Employees.FirstOrDefault(emp => emp.Title == "Developer");

            this.ReinitializeContextLog();

            TaskAssignment retrievedTaskAssgnment = retrievedEmployee.TaskAssignments.FirstOrDefault();
            string sqlGeneratedOnAccessingTaskAssignments = this.GetGeneratedSql();

            Task retrievedTask = retrievedTaskAssgnment.Task;
            string sqlGeneratedOnAccessingTask = this.GetGeneratedSql();

            Project retrievedProject = retrievedTask.Project;
            string sqlGeneratedOnAccessingProject = this.GetGeneratedSql();

            Assert.AreEqual(string.Empty, sqlGeneratedOnAccessingTaskAssignments);
            Assert.AreEqual(string.Empty, sqlGeneratedOnAccessingTask);
            Assert.AreEqual(string.Empty, sqlGeneratedOnAccessingProject);
        }

        /// <summary>
        /// Scenario: Load an Employee together with several of its related objects in depth. Access the objects specified in the fetch strategy.
        /// Condition: Use a context level fetch strategy. Use the MaxFetchDepth property to set the maximum depth of the properties which are to be eagerly loaded.
        /// Outcome: The specified related objects up to the MaxFetchDepth, will all be loaded at the same time the Employee is retrieved, therefore no SQL is generated
        /// when accessing them. When a related object with higher depth is accessed, SQL statement(s) will be generated in order to load it.
        /// </summary>
        [TestMethod]
        public void FetchStrategy_PerContext_LoadingInDepth_UsingMaxFetchDepth()
        {
            FetchStrategy upToTask = new FetchStrategy();
            upToTask.LoadWith<Employee>(emp => emp.TaskAssignments);
            upToTask.LoadWith<TaskAssignment>(asignment => asignment.Task);
            upToTask.LoadWith<Task>(task => task.Project);
            upToTask.MaxFetchDepth = 2;

            this.context.FetchStrategy = upToTask;

            Employee retrievedEmployee = context.Employees.FirstOrDefault(emp => emp.Title == "Developer");

            ReinitializeContextLog();

            TaskAssignment retrievedTaskAssgnment = retrievedEmployee.TaskAssignments.FirstOrDefault();
            string sqlGeneratedOnAccessingTaskAssignments = this.GetGeneratedSql();

            Task retrievedTask = retrievedTaskAssgnment.Task;
            string sqlGeneratedOnAccessingTask = this.GetGeneratedSql();

            Project retrievedProject = retrievedTask.Project;
            string sqlGeneratedOnAccessingProject = this.GetGeneratedSql();

            Assert.AreEqual(string.Empty, sqlGeneratedOnAccessingTaskAssignments);
            Assert.AreEqual(string.Empty, sqlGeneratedOnAccessingTask);
            Assert.AreNotEqual(string.Empty, sqlGeneratedOnAccessingProject);
        }

        /// <summary>
        /// Scenario: Load an Employee together with several directly related objects. Access the related objects
        /// specified in the fetch strategy.
        /// Condition: Use a context level fetch strategy.
        /// Outcome: The related objects specified in the fetch strategy will be loaded at the same time the employee is retrieved. No SQL will be
        /// generated when accessing them.
        /// </summary>
        [TestMethod]
        public void FetchStrategy_PerContext_LoadingInWidth()
        {
            FetchStrategy loadSupervisorAndAssignment = new FetchStrategy();
            loadSupervisorAndAssignment.LoadWith<Employee>(emp => emp.Supervisor);
            loadSupervisorAndAssignment.LoadWith<Employee>(emp => emp.TaskAssignments);

            this.context.FetchStrategy = loadSupervisorAndAssignment;

            Employee retrievedEmployee = this.context.Employees.FirstOrDefault(emp => emp.Title == "Developer");

            ReinitializeContextLog();

            Employee employeeSupervisor = retrievedEmployee.Supervisor;
            string sqlGeneratedOnAccessingSupervisor = this.GetGeneratedSql();

            TaskAssignment employeeAssignment = retrievedEmployee.TaskAssignments.FirstOrDefault();
            string sqlGeneratedOnAccessingTaskAssignments = this.GetGeneratedSql();

            Assert.AreEqual(string.Empty, sqlGeneratedOnAccessingSupervisor);
            Assert.AreEqual(string.Empty, sqlGeneratedOnAccessingTaskAssignments);
        }

        /// <summary>
        /// Scenario: Load an Employee together with its TaskAssignments.  Access its TaskAssignments.
        /// Condition: Use a query level fetch strategy with the Include method. The properties which are to be 
        /// eagerly loaded are specified with an expression.
        /// Outcome: TaskAssignments will be loaded at the same time the employee is retrieved. No SQL will be generated when accessing them.
        /// </summary>
        [TestMethod]
        public void FetchStrategy_PerQuery_UsingInclude()
        {
            Employee retrievedEmployee = this.context.Employees.Include(emp => emp.TaskAssignments).FirstOrDefault(emp => emp.Title == "Developer");
            string sqlGeneratedOnRetrievingEmployee = this.GetGeneratedSql();

            TaskAssignment taskAssignment = retrievedEmployee.TaskAssignments.FirstOrDefault();
            string sqlGeneratedOnAccessingTaskAssignments = this.GetGeneratedSql();

            bool areTaskAssignmentsEagerlyLoaded = sqlGeneratedOnRetrievingEmployee.Contains(SQL_TO_LOAD_TASK_ASSIGNMENTS_WITH_FETCH_STRATEGY);

            Assert.IsTrue(areTaskAssignmentsEagerlyLoaded);
            Assert.AreEqual(string.Empty, sqlGeneratedOnAccessingTaskAssignments);
        }

        /// <summary>
        /// Scenario: Load an Employee together with several related objects in depth. Access the navigation properties specified in the fetch strategy.
        /// Condition: Use a query level fetch strategy. Specify the properties which are to be eagerly loaded with the Include method using expressions.
        /// Outcome: The related objects specified in the Include method will be loaded at the same time the Employee is retrieved. No SQL will be 
        /// generated when accessing them.
        /// </summary>
        [TestMethod]
        public void FetchStrategy_PerQuery_UsingInclude_LoadingInDepth()
        {
            Employee retrievedEmployee = this.context.Employees.Include(emp => emp.TaskAssignments.Select(assignment => assignment.Task)).FirstOrDefault(emp => emp.Title == "Developer");

            ReinitializeContextLog();

            TaskAssignment employeeAssignment = retrievedEmployee.TaskAssignments.FirstOrDefault();
            string sqlGeneratedOnAccessingTaskAssignments = this.GetGeneratedSql();

            Task employeeAssignmentTask = employeeAssignment.Task;
            string sqlGeneratedOnAccessingTask = this.GetGeneratedSql();

            Assert.AreEqual(string.Empty, sqlGeneratedOnAccessingTaskAssignments);
            Assert.AreEqual(string.Empty, sqlGeneratedOnAccessingTask);
        }

        /// <summary>
        /// Scenario: Load an Employee together with several directly related objects. Access the navigation properties specified in the fetch strategy.
        /// Condition: Use a query level fetch strategy. Specify the properties which are to be eagerly loaded with the Include method using expressions.
        /// Outcome The related objects specified in the Include method strategy will be loaded at the same time the employee is retrieved.
        /// No SQL will be generated when accessing them.
        /// </summary>
        [TestMethod]
        public void FetchStrategy_PerQuery_UsingInclude_LoadingInWidth()
        {
            Employee retrievedEmployee = this.context.Employees.Include(emp => emp.TaskAssignments).Include(emp => emp.Supervisor).FirstOrDefault(emp => emp.Title == "Developer");

            ReinitializeContextLog();

            TaskAssignment employeeAssignment = retrievedEmployee.TaskAssignments.FirstOrDefault();
            string sqlGeneratedOnAccessingTaskAssignments = this.GetGeneratedSql();

            Employee employeeSupervisor = retrievedEmployee.Supervisor;
            string sqlGeneratedOnAccessingSupervisor = this.GetGeneratedSql();

            Assert.AreEqual(string.Empty, sqlGeneratedOnAccessingTaskAssignments);
            Assert.AreEqual(string.Empty, sqlGeneratedOnAccessingSupervisor);
        }

        /// <summary>
        /// Scenario: Load an Employee together with its TaskAssignments. Access its TaskAssignments property.
        /// Condition: Use a query level fetch strategy. The navigation properties which are to be eagerly loaded are specified by passing a fetch strategy
        /// to the LoadWith method.
        /// Outcome: The TaskAssignments property will be loaded at the same time the employee is retrieved. No SQL will be generated when 
        /// accessing it.
        /// </summary>
        [TestMethod]
        public void FetchStrategy_PerQuery_UsingLoadWith()
        {
            FetchStrategy withTaskAssignments = new FetchStrategy();
            withTaskAssignments.LoadWith<Employee>(emp => emp.TaskAssignments);

            Employee retrievedEmployee = this.context.Employees.LoadWith(withTaskAssignments).FirstOrDefault(emp => emp.Title == "Developer");
            string sqlGeneratedOnRetrievingEmployee = this.GetGeneratedSql();

            TaskAssignment employeeAssignment = retrievedEmployee.TaskAssignments.FirstOrDefault();
            string sqlGeneratedOnAccessingTaskAssignments = this.GetGeneratedSql();

            bool areTaskAssignmentsEagerlyLoaded = sqlGeneratedOnRetrievingEmployee.Contains(SQL_TO_LOAD_TASK_ASSIGNMENTS_WITH_FETCH_STRATEGY);

            Assert.IsTrue(areTaskAssignmentsEagerlyLoaded);
            Assert.AreEqual(string.Empty, sqlGeneratedOnAccessingTaskAssignments);
        }

        /// <summary>
        /// Scenario: Load an Employee together with several of its related objects in depth.
        /// Condition: Query level fetch strategy is used. The navigation properties which are to be eagerly loaded are specified by passing a fetch strategy
        /// to the LoadWith method.
        /// Outcome: The related objects specified in the fetch strategy will be loaded at the same time the employee is retrieved.
        /// No SQL will be generated when accessing them.
        /// </summary>
        [TestMethod]
        public void FetchStrategy_PerQuery_UsingLoadWith_LoadingInDepth()
        {
            FetchStrategy upToProject = new FetchStrategy();
            upToProject.LoadWith<Employee>(emp => emp.TaskAssignments);
            upToProject.LoadWith<TaskAssignment>(emp => emp.Task);
            upToProject.LoadWith<Task>(task => task.Project);

            Employee retrievedEmployee = this.context.Employees.LoadWith(upToProject).FirstOrDefault(emp => emp.Title == "Developer");

            ReinitializeContextLog();

            TaskAssignment employeeAssignment = retrievedEmployee.TaskAssignments.FirstOrDefault();
            string sqlGeneratedOnAccessingTaskAssignments = this.GetGeneratedSql();

            Task assignedTask = employeeAssignment.Task;
            string sqlGeneratedOnAccessingTask = this.GetGeneratedSql();

            Project taskProject = assignedTask.Project;
            string sqlGeneratedOnAccessingProject = this.GetGeneratedSql();

            Assert.AreEqual(string.Empty, sqlGeneratedOnAccessingTaskAssignments);
            Assert.AreEqual(string.Empty, sqlGeneratedOnAccessingTask);
            Assert.AreEqual(string.Empty, sqlGeneratedOnAccessingProject);
        }

        /// <summary>
        /// Scenario: Load an Employee together with several of its related objects in depth.
        /// Condition: Query level fetch strategy is used. The navigation properties which are to be eagerly loaded are specified by passing a fetch strategy to the LoadWith method.
        /// The MaxFetchDepth property is used to limit the depth of the eagerly loaded navigation properties.
        /// Outcome: the related objects up to the specified depth will be loaded at the same time the employee is retrieved. No SQL will be generated
        /// when accessing them. When accessing related objects with higher depth, SQL statement(s) will be generated in order to retrieve them.
        /// </summary>
        [TestMethod]
        public void FetchStrategy_PerQuery_UsingLoadWith_InDepth_MaxFetchDepth()
        {
            FetchStrategy upToTask = new FetchStrategy();
            upToTask.LoadWith<Employee>(emp => emp.TaskAssignments);
            upToTask.LoadWith<TaskAssignment>(emp => emp.Task);
            upToTask.LoadWith<Task>(task => task.Project);
            upToTask.MaxFetchDepth = 2;

            Employee retrievedEmployee = this.context.Employees.LoadWith(upToTask).FirstOrDefault(emp => emp.Title == "Developer");

            ReinitializeContextLog();

            TaskAssignment employeeAssignment = retrievedEmployee.TaskAssignments.FirstOrDefault();
            string sqlGeneratedOnAccessingTaskAssignments = this.GetGeneratedSql();

            Task assignedTask = employeeAssignment.Task;
            string sqlGeneratedOnAccessingTask = this.GetGeneratedSql();

            Project taskProject = assignedTask.Project;
            string sqlGeneratedOnAccessingProject = this.GetGeneratedSql();

            Assert.AreEqual(string.Empty, sqlGeneratedOnAccessingTaskAssignments);
            Assert.AreEqual(string.Empty, sqlGeneratedOnAccessingTask);
            Assert.AreNotEqual(string.Empty, sqlGeneratedOnAccessingProject);
        }

        /// <summary>
        /// Scenario: Load an Employee together with several of its directly related object. 
        /// Condition: Query level fetch strategy is used. The navigation properties which are to be eagerly loaded are specified by passing a fetch strategy to
        /// the LoadWith method.
        /// Outcome: the related objects will be loaded at the same time the Employee is retrieved. No SQL will be generated when accessing them
        /// </summary>
        [TestMethod]
        public void FetchStrategy_PerQuery_UsingLoadWith_LoadingInWidth()
        {
            FetchStrategy loadSupervisorAndAssignment = new FetchStrategy();
            loadSupervisorAndAssignment.LoadWith<Employee>(emp => emp.Supervisor);
            loadSupervisorAndAssignment.LoadWith<Employee>(emp => emp.TaskAssignments);

            Employee retrievedEmployee = this.context.Employees.LoadWith(loadSupervisorAndAssignment).FirstOrDefault(emp => emp.Title == "Developer");

            ReinitializeContextLog();

            TaskAssignment employeeAssignment = retrievedEmployee.TaskAssignments.FirstOrDefault();
            string sqlGeneratedOnAccessingTaskAssignments = this.GetGeneratedSql();

            Employee employeeSupervisor = retrievedEmployee.Supervisor;
            string sqlGeneratedOnAccessingSupervisor = this.GetGeneratedSql();

            Assert.AreEqual(string.Empty, sqlGeneratedOnAccessingTaskAssignments);
            Assert.AreEqual(string.Empty, sqlGeneratedOnAccessingSupervisor);
        }

        /// <summary>
        /// Scenario: Use a query level fetch strategy together with a context level fetch strategy. 
        /// Outcome: The query level fetch strategy will override the context level fetch strategy. The objects specified in the context level fetch strategy
        /// will not be eagerly loaded.
        /// </summary>
        [TestMethod]
        public void FetchStrategy_PerQuery_OrverridesFetchStrategyPerContext()
        {
            FetchStrategy upToProject = new FetchStrategy();
            upToProject.LoadWith<Employee>(emp => emp.TaskAssignments);
            upToProject.LoadWith<TaskAssignment>(emp => emp.Task);

            this.context.FetchStrategy = upToProject;

            Employee retrievedEmployee = this.context.Employees.Include(emp => emp.TaskAssignments).FirstOrDefault(emp => emp.Title == "Developer");

            ReinitializeContextLog();

            TaskAssignment employeeAssignment = retrievedEmployee.TaskAssignments.FirstOrDefault();
            string sqlGeneratedOnAccessingTaskAssignments = this.GetGeneratedSql();

            Task assignedTask = employeeAssignment.Task;
            string sqlGeneratedOnAccessingTask = this.GetGeneratedSql();

            Assert.AreEqual(string.Empty, sqlGeneratedOnAccessingTaskAssignments);
            Assert.AreNotEqual(string.Empty, sqlGeneratedOnAccessingTask);
        }

        //Retrieves the SQL statements in the log of the context.
        private string GetGeneratedSql()
        {
            if (this.context != null)
            {
                string generatedSql = context.Log.ToString();
                this.ReinitializeContextLog();
                return generatedSql;
            }
            else
            {
                throw new NullReferenceException("The context object is null");
            }
        }

        //Reinitializes the context log in order to remove left over SQL statements.
        private void ReinitializeContextLog()
        {
            if (context != null)
            {
                this.context.Log = null;
                this.context.Log = new StringWriter();
            }
            else
            {
                throw new NullReferenceException("The context object is null");
            }
        }

    }
}
