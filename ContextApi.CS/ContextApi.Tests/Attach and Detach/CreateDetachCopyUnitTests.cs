using System.Collections.Generic;
using System.Linq;
using ContextApi.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.OpenAccess;
using Telerik.OpenAccess.FetchOptimization;

namespace ContextApi.Tests.Attach_and_Detach
{
    [TestClass]
    public class CreateDetachCopyUnitTests : UnitTestsBase
    {
        private EntitiesModel context;

        /// <summary>
        /// Scenario: Detach a single object from its managing context.
        /// Condition: No navigation properties should be detached with the object.
        /// Outcome: The detached object is not the same instance as the original,its reference properties will be null
        /// and reference collections will be empty.
        /// </summary>
        [TestMethod]
        public void Detach_BasicDetach()
        {
            Employee retrievedEmployee;
            Employee detachedEmployee;

            using (this.context = new EntitiesModel())
            {                
                retrievedEmployee = context.Employees.FirstOrDefault(emp => emp.Title == "Developer");

                detachedEmployee = context.CreateDetachedCopy<Employee>(retrievedEmployee);
            }

            ObjectState detachedEmployeeState = this.GetObjectState(detachedEmployee);

            Assert.AreNotSame(detachedEmployee, retrievedEmployee);
            Assert.IsNull(detachedEmployee.Supervisor);
            Assert.AreEqual(0, detachedEmployee.TaskAssignments.Count);
        }

        /// <summary>
        /// Scenario: Detach a single object from its managing context together with some of its navigation properties.
        /// Condition: The navigation properties will be specified with strings representing their names.
        /// Outcome: The state of the detached object will be DetachedClean and the specified navigation properties will be detached as well.
        /// </summary>
        [TestMethod]
        public void Detach_DetachWithReferenecPropertiesPassedAsStrings()
        {
            Employee detachedEmployee;
            int expectedAssignmentsCount;

            using (this.context = new EntitiesModel())
            {
                Employee retrievedEmployee = context.Employees.FirstOrDefault(emp => emp.Title == "Developer");
                expectedAssignmentsCount = retrievedEmployee.TaskAssignments.Count;

                detachedEmployee = context.CreateDetachedCopy<Employee>(retrievedEmployee, "Supervisor", "TaskAssignments");
            }

            ObjectState detachedEmployeeState = this.GetObjectState(detachedEmployee);

            Assert.AreEqual(ObjectState.DetachedClean, detachedEmployeeState);
            Assert.IsNotNull(detachedEmployee.Supervisor);
            Assert.AreEqual(expectedAssignmentsCount, detachedEmployee.TaskAssignments.Count);
        }

        /// <summary>
        /// Scenario: Detach a single object from its managing context together with some of its navigation properties.
        /// Condition: The navigation properties will be specified with expressions.
        /// Outcome: The state of the detached object will be DetachedClean and the specified navigation properties will be detached as well.
        /// </summary>
        [TestMethod]
        public void Detach_DetachWithReferencePropertiesPassedAsExpression()
        {
            Employee detachedEmployee;
            int expectedAssignmentsCount;

            using (this.context = new EntitiesModel())
            {
                Employee retrievedEmployee = context.Employees.FirstOrDefault(emp => emp.Title == "Developer");
                expectedAssignmentsCount = retrievedEmployee.TaskAssignments.Count;

                detachedEmployee = context.CreateDetachedCopy<Employee>(retrievedEmployee, 
                    (Employee emp) => emp.Supervisor, 
                    (Employee emp) => emp.TaskAssignments);
            }

            ObjectState detachedEmployeeState = this.GetObjectState(detachedEmployee);

            Assert.AreEqual(ObjectState.DetachedClean, detachedEmployeeState);
            Assert.IsNotNull(detachedEmployee.Supervisor);
            Assert.AreEqual(expectedAssignmentsCount, detachedEmployee.TaskAssignments.Count);
        }

        /// <summary>
        /// Scenario: Detach a single object from its managing context together with some of its navigation properties.
        /// Condition: The navigation properties which will be detached will be specified by a fetch strategy.
        /// Outcome: The state of the detached object will be DetachedClean and the specified navigation properties will be detached as well.
        /// </summary>
        [TestMethod]
        public void Detach_DetachWithReferencePropertiesUsingFetchStrategy()
        {
            Employee detachedEmployee;
            int expectedAssignmentsCount;
            
            using (this.context = new EntitiesModel())
            {
                Employee retrievedEmployee = context.Employees.FirstOrDefault(emp => emp.Title == "Developer");
                expectedAssignmentsCount = retrievedEmployee.TaskAssignments.Count;

                FetchStrategy withSupervisorAndTaskAssignments = new FetchStrategy();
                withSupervisorAndTaskAssignments.LoadWith<Employee>(emp => emp.Supervisor);
                withSupervisorAndTaskAssignments.LoadWith<Employee>(emp => emp.TaskAssignments);

                detachedEmployee = context.CreateDetachedCopy<Employee>(retrievedEmployee, withSupervisorAndTaskAssignments);
            }

            ObjectState detachedEmployeeState = this.GetObjectState(detachedEmployee);

            Assert.AreEqual(ObjectState.DetachedClean, detachedEmployeeState);
            Assert.IsNotNull(detachedEmployee.Supervisor);
            Assert.AreEqual(expectedAssignmentsCount, detachedEmployee.TaskAssignments.Count);
        }

        /// <summary>
        /// Scenario: Detach multiple objects from their managing context with some of their navigation properties.
        /// Condition: The navigation properties which will be detached will be specified by a fetch strategy.
        /// Outcome: All of the detached objects will have DetachedClean state and their specified navigation properties will be detached as well.
        /// </summary>
        [TestMethod]
        public void Detach_DetachMultipleEntitiesWithReferencePropertiesUsingFetchStrategy()
        {
            IList<Task> retrievedTasks;
            int retrievedTasksCount;
            IList<Task> detachedTasks;

            using (this.context = new EntitiesModel())
            {
                FetchStrategy withProjectAndAssignments = new FetchStrategy();
                withProjectAndAssignments.LoadWith<Task>(task => task.Project);
                withProjectAndAssignments.LoadWith<Task>(task => task.TaskAssignments);

                retrievedTasks = context.Tasks.LoadWith(withProjectAndAssignments).ToList();
                retrievedTasksCount = retrievedTasks.Count;

                detachedTasks = context.CreateDetachedCopy<Task>(retrievedTasks, withProjectAndAssignments).ToList();
            }

            for (int taskIndex = 0; taskIndex < retrievedTasksCount; taskIndex++)
            {
                Task detachedTask = detachedTasks[taskIndex];
                ObjectState detachedTaskState = this.GetObjectState(detachedTask);

                Task respectiveManagedTask = retrievedTasks[taskIndex];                

                Assert.AreEqual(ObjectState.DetachedClean, detachedTaskState);
                Assert.IsNotNull(detachedTask.Project);
                Assert.AreEqual(respectiveManagedTask.TaskAssignments.Count, detachedTask.TaskAssignments.Count);
            }
        }

        /// <summary>
        /// Scenario: Detach multiple objects from their managing object without loading any of their navigation properties.
        /// Condition: A null value will be passed as a FetchStrategy argument to the CreateDetachedCopy method.
        /// Outcome: All of the detached objects will have DetachedClean state, their reference properties will be null and their 
        /// reference collections will be empty because the default fetch strategy will be used.
        /// </summary>
        [TestMethod]
        public void Detach_DetachMultipleEntitiesWithoutReferenceProperties()
        {
            IList<Task> detachedTasks;

            using (this.context = new EntitiesModel())
            {
                IList<Task> retrievedTasks = context.Tasks.ToList();

                detachedTasks = context.CreateDetachedCopy<Task>(retrievedTasks, null).ToList();
            }

            foreach (Task detachedTask in detachedTasks)
            {
                ObjectState detachedTaskState = this.GetObjectState(detachedTask);

                Assert.AreEqual(ObjectState.DetachedClean, detachedTaskState);
                Assert.IsNull(detachedTask.Project);
                Assert.AreEqual(0, detachedTask.TaskAssignments.Count);
            }
        }

        /// <summary>
        /// Scenario: Detach multiple objects.
        /// Condition: The detached objects have self referencing association between them.
        /// Outcome: The detached objects will have DetachedClean state.
        /// Their navigation properties referencing instances of the detached objects will be automatically resolved and set.
        /// </summary>
        [TestMethod]
        public void Detach_DetachMultipleEntitiesWithSelfReference()
        {
            IList<Employee> retrievedEmployees;
            int retrievedEmployeesCount;
            IList<Employee> detachedEmployees;

            using (this.context = new EntitiesModel())
            {
                retrievedEmployees = context.Employees.ToList();
                retrievedEmployeesCount = retrievedEmployees.Count;

                detachedEmployees = context.CreateDetachedCopy<Employee>(retrievedEmployees, null).ToList();
            }

            for (int employeeIndex = 0; employeeIndex < retrievedEmployeesCount; employeeIndex++)
            {
                Employee detachedEmployee = detachedEmployees[employeeIndex];
                ObjectState detachedEmployeeState = this.GetObjectState(detachedEmployee);

                Employee respectiveManagedEmployee = retrievedEmployees[employeeIndex];

                bool areSupervisorsTheSame = this.AreSupervisorsTheSame(detachedEmployee.Supervisor, respectiveManagedEmployee.Supervisor);

                Assert.AreEqual(ObjectState.DetachedClean, detachedEmployeeState);
                Assert.IsTrue(areSupervisorsTheSame);
                Assert.AreEqual(0, detachedEmployee.TaskAssignments.Count);
            }
        }

        /// <summary>
        /// Scenario: Detach an object.
        /// Condition: The object which will be detached has some of its navigation properties loaded with a fetch strategy.
        /// Outcome: The detached object will have DetachedClean state. Default fetch strategy will be used and 
        /// its navigation properties will not be detached with it.
        /// </summary>
        [TestMethod]
        public void Detach_ContextLevelFetchStrategyIsIgnored_DuringDetach_DefaultFetchStrategyIsApplied()
        {
            Employee detachedEmployee;

            using (this.context = new EntitiesModel())
            {
                FetchStrategy withSupervisorAndAssignments = new FetchStrategy();
                withSupervisorAndAssignments.LoadWith<Employee>(emp => emp.Supervisor);
                withSupervisorAndAssignments.LoadWith<Employee>(emp => emp.TaskAssignments);

                this.context.FetchStrategy = withSupervisorAndAssignments;

                Employee retrievedEmployee = context.Employees.FirstOrDefault(emp => emp.Title == "Developer");

                detachedEmployee = context.CreateDetachedCopy<Employee>(retrievedEmployee);
            }

            ObjectState detachedEmployeeState = this.GetObjectState(detachedEmployee);

            Assert.AreEqual(ObjectState.DetachedClean, detachedEmployeeState);
            Assert.IsNull(detachedEmployee.Supervisor);
            Assert.AreEqual(0, detachedEmployee.TaskAssignments.Count);
        }

        /// <summary>
        /// Scenario: Detach an object.
        /// Condition: The object which will be detached has some of its navigation properties loaded with a fetch strategy.
        /// Another fetch strategy is applied when detaching.
        /// Outcome: The detached object will have DetachedClean state. The context fetch strategy will be ignored during the detach
        /// and only the detach fetch strategy will be applies.
        /// </summary>
        [TestMethod]
        public void Detach_ContextLevelFetchStrategyIsIgnored_DuringDetach_DetachSpecificFetchStrategyIsApplied()
        {
            Employee detachedEmployee;
            int expectedTaskAssignmentsCount;

            using (this.context = new EntitiesModel())
            {
                FetchStrategy contextFetchStrategy = new FetchStrategy();
                contextFetchStrategy.LoadWith<Employee>(emp => emp.Supervisor);

                this.context.FetchStrategy = contextFetchStrategy;

                Employee retrievedEmployee = context.Employees.FirstOrDefault(emp => emp.Title == "Developer");
                expectedTaskAssignmentsCount = retrievedEmployee.TaskAssignments.Count;

                FetchStrategy detachFetchStrategy = new FetchStrategy();
                detachFetchStrategy.LoadWith<Employee>(emp => emp.TaskAssignments);

                detachedEmployee = context.CreateDetachedCopy<Employee>(retrievedEmployee, detachFetchStrategy);
            }

            ObjectState detachedEmployeeState = this.GetObjectState(detachedEmployee);

            Assert.AreEqual(ObjectState.DetachedClean, detachedEmployeeState);
            Assert.IsNull(detachedEmployee.Supervisor);
            Assert.AreEqual(expectedTaskAssignmentsCount, detachedEmployee.TaskAssignments.Count);
        }

        /// <summary>
        /// Scenario: Detach an object from its context.
        /// Conditions: Some of the navigation properties of the object should be loaded before the detach.
        /// Outcome: The detached object will have DetachedClean state. Its navigation properties will not be detached with it.
        /// </summary>
        [TestMethod]
        public void Detach_LoadedNavigationPropertiesAreIgnoredDuringDetach()
        {
            Employee detachedEmployee;

            using (this.context = new EntitiesModel())
            {
                Employee retrievedEmployee = context.Employees.FirstOrDefault(emp => emp.Title == "Developer");
                Employee supervisor = retrievedEmployee.Supervisor;
                IList<TaskAssignment> assignments = retrievedEmployee.TaskAssignments.ToList();

                detachedEmployee = context.CreateDetachedCopy<Employee>(retrievedEmployee);
            }

            ObjectState detachedEmployeeState = this.GetObjectState(detachedEmployee);

            Assert.AreEqual(ObjectState.DetachedClean, detachedEmployeeState);
            Assert.IsNull(detachedEmployee.Supervisor);
            Assert.AreEqual(0, detachedEmployee.TaskAssignments.Count);
        }

        /// <summary>
        /// Scenario: Detach a modified object from its context.
        /// Outcome: The state of the detached object will be DetachedDirty.
        /// </summary>
        [TestMethod]
        public void Detach_DetachModifiedObject()
        {
            Employee detachedEmployee;

            using (this.context = new EntitiesModel())
            {
                Employee retrievedEmployee = this.context.Employees.FirstOrDefault(emp => emp.Title == "Developer");
                retrievedEmployee.FirstName = "Mark";

                detachedEmployee = this.context.CreateDetachedCopy<Employee>(retrievedEmployee);
            }

            ObjectState detachedEmployeeState = this.GetObjectState(detachedEmployee);

            Assert.AreEqual(ObjectState.DetachedDirty, detachedEmployeeState);
        }

        /// <summary>
        /// Scenario: Detach an object from its context.
        /// Condition: modify the detached object.
        /// Outcome: The context from which the object was detached will not have any changes.
        /// </summary>
        [TestMethod]
        public void Detach_ModifyDetachedObject()
        {
            Employee detachedEmployee;
            bool contextHasChangesAfterModifyingDetachedObject;

            using (this.context = new EntitiesModel())
            {
                Employee retrievedEmployee = this.context.Employees.FirstOrDefault(emp => emp.Title == "Developer");

                detachedEmployee = this.context.CreateDetachedCopy<Employee>(retrievedEmployee);
                detachedEmployee.FirstName = "Mark";

                contextHasChangesAfterModifyingDetachedObject = this.context.HasChanges;
            }

            Assert.IsFalse(contextHasChangesAfterModifyingDetachedObject);
        }

        /// <summary>
        /// Scenario: Detach an object marked for deletion from its context.
        /// Condition: The object is not yet actually deleted from the database.
        /// Outcome: An InvalidOperationException will be thrown.
        /// </summary>
        [TestMethod]
        public void Detach_DetachObjectMarkedForDeletion()
        {
            using (this.context = new EntitiesModel())
            {
                Employee retrievedEmployee = this.context.Employees.FirstOrDefault(emp => emp.Title == "Developer");
                this.context.Delete(retrievedEmployee);

                AssertException.Throws<Telerik.OpenAccess.Exceptions.InvalidOperationException>(() => this.context.CreateDetachedCopy<Employee>(retrievedEmployee));
            }
        }

        /// <summary>
        /// Scenario: Detach a newly added object from its context.
        /// Condition: The object is not yet persisted in the database.
        /// Outcome: A InvalidOperationException will be thrown.
        /// </summary>
        [TestMethod]
        public void Detach_DetachNewObject()
        {
            Employee newEmployee = new Employee()
            {
                EmployeeId = 1337,
                FirstName = "Mad",
                LastName = "Jack"
            };

            using (this.context = new EntitiesModel())
            {
                this.context.Add(newEmployee);

                AssertException.Throws<Telerik.OpenAccess.Exceptions.InvalidOperationException>(() => this.context.CreateDetachedCopy<Employee>(newEmployee));
            }
        }

        //Check if the passed Employee instances can be considered the same.
        protected bool AreSupervisorsTheSame(Employee emp1, Employee emp2)
        {

            if (emp1 == null && emp2 == null)
            {
                return true;
            }
            else if (emp1 == null || emp2 == null)
            {
                return false;
            }
            else
            {
                return emp1.EmployeeId.Equals(emp2.EmployeeId);
            }
        }
    }
}
