using System.Collections.Generic;
using System.Linq;
using ContextApi.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ContextApi.Tests.CrudOperations
{
    [TestClass]
    public class DeleteTests : UnitTestsBase
    {      
        /// <summary>
        /// Scenario: Delete a MonthlyReport. This deletion will not change the MonthlyReports collection of an employee.
        /// Delete: Task.
        /// Relationship: 1 - * / Employee - MonthlyReport.
        /// Conditions: Navigation property MonthlyReports in Employee is not managed.
        /// Outcome: MonthlyReports collection is not changed, even after the deletion of a MonthlyReport.
        /// </summary>
        [TestMethod]
        public void One_To_Many_DeleteChild_IsManagedFalse()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                MonthlyReport monthlyReport = dbContext.MonthlyReports.FirstOrDefault();
                Assert.IsNotNull(monthlyReport, MessageHelper.NoRecordsInDatabase(typeof(MonthlyReport)));

                Employee employee = monthlyReport.Employee;

                int monthlyReportsCountBeforeDelete = employee.MonthlyReports.Count;

                dbContext.Delete(monthlyReport);

                int monthlyReportsCountAfterDelete = employee.MonthlyReports.Count;

                // Given that IsManaged is false, monthlyReportsCountBeforeDelete and monthlyReportsCountAfterDelete should be equal to each other
                Assert.AreEqual(monthlyReportsCountBeforeDelete, monthlyReportsCountAfterDelete);
                monthlyReport = dbContext.MonthlyReports.FirstOrDefault();
                Assert.IsNotNull(monthlyReport);

                dbContext.SaveChanges();
                int monthlyReportsCountAfterSave = employee.MonthlyReports.Count;

                Assert.AreEqual(monthlyReportsCountAfterDelete - 1, monthlyReportsCountAfterSave);

                monthlyReport = dbContext.MonthlyReports.FirstOrDefault();
                Assert.IsNull(monthlyReport);
            }
        }

        /// <summary>
        /// Scenario: Delete an Employee. This deletion will not change the Employee property of a MonthlyReport, even if that employee is deleted.
        /// Delete: Task.
        /// Relationship: 1 - * / Employee - MonthlyReport.
        /// Conditions: Navigation property MonthlyReports in Employee is not managed.
        /// Outcome: Employee property of an MonthlyReport is not changed.
        /// </summary>
        [TestMethod]
        public void One_To_Many_DeleteParent_IsManagedFalse()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                MonthlyReport monthlyReport = dbContext.MonthlyReports.FirstOrDefault();
                Assert.IsNotNull(monthlyReport, MessageHelper.NoRecordsInDatabase(typeof(MonthlyReport)));


                Employee employee = monthlyReport.Employee;

                dbContext.Delete(employee);

                // Given that IsManaged is false monthlyRep.Employee will be not null
                Assert.IsNotNull(monthlyReport.Employee);
                // Since the Employee is actually set for deletion we cannot access its properties
                AssertException.Throws<Telerik.OpenAccess.Exceptions.InvalidOperationException>(() => { monthlyReport.Employee.FirstName = ""; });
            }
        }

        /// <summary>
        /// Scenario: Delete a DailyReport. This deletion will be reflected in the DailyReports collection of the Employee.
        /// Delete: DailyReport.
        /// Relationship: 1 - * / Employee - DailyReport.
        /// Conditions: Navigation property DailyReports in Employee is managed.
        /// Outcome: DailyReports property of an Employee is changed.
        /// </summary>
        [TestMethod]
        public void One_To_Many_DeleteChild_IsManagedTrue()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                DailyReport dailyReport = dbContext.DailyReports.FirstOrDefault();
                Assert.IsNotNull(dailyReport, MessageHelper.NoRecordsInDatabase(typeof(DailyReport)));
                
                Employee employee = dailyReport.Employee;

                int dailyReportsBeforeDeleteCount = employee.DailyReports.Count();

                dbContext.Delete(dailyReport);

                int dailyReportsAfterDeleteCount = employee.DailyReports.Count();

                Assert.AreEqual(dailyReportsBeforeDeleteCount - 1, dailyReportsAfterDeleteCount);
            }
        }

        /// <summary>
        /// Scenario: Delete an Employee. This deletion will be reflected in the Employee property if a DailyReport.
        /// Delete: DailyReport.
        /// Relationship: 1 - * / Employee - DailyReport.
        /// Conditions: Navigation property DailyReports in Employee is managed.
        /// Outcome: DailyReports property of an Employee is changed.
        /// </summary>
        [TestMethod]
        public void One_To_Many_DeleteParent_IsManagedTrue()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                DailyReport dailyReport = dbContext.DailyReports.FirstOrDefault();
                Assert.IsNotNull(dailyReport, MessageHelper.NoRecordsInDatabase(typeof(DailyReport)));


                Employee employee = dailyReport.Employee;
                int empPrimaryKey = employee.EmployeeId;

                dbContext.Delete(employee);

                AssertException.Throws<Telerik.OpenAccess.Exceptions.InvalidOperationException>(() => dailyReport.Employee.FirstName = "");

                // Primary key of an object marked for deletion can be accessed without exception being thrown
                Assert.AreEqual(empPrimaryKey, employee.EmployeeId);
            }
        }

        /// <summary>
        /// Scenario: Delete a task and all task assignments that are bound to that task.
        /// Delete: Task.
        /// Relationship: 1 - * / Task - TaskAssignment.
        /// Conditions: Navigation property TaskAssignment in Task is dependant.
        /// Outcome: Task and TaskAssignments task are deleted.
        /// </summary>
        [TestMethod]
        public void One_To_Many_DeleteParent_IsDependantTrueInParent()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Task task = dbContext.Tasks.FirstOrDefault();
                Assert.IsNotNull(task, MessageHelper.NoRecordsInDatabase(typeof(Task)));
                
                TaskAssignment taskAssignment = task.TaskAssignments.FirstOrDefault();
                Assert.IsNotNull(taskAssignment, MessageHelper.NoRecordsInDatabase(typeof(TaskAssignment)));


                int allTaskCount = dbContext.Tasks.Count();
                int allTaskAssignmentCount = dbContext.TaskAssignments.Count();
                int taskAssigmentCount = task.TaskAssignments.Count();

                dbContext.Delete(task);

                AssertException.Throws<Telerik.OpenAccess.Exceptions.InvalidOperationException>(() => taskAssignment.WorkingHours = 1);

                dbContext.SaveChanges();

                Assert.AreEqual(allTaskCount - 1, dbContext.Tasks.Count());
                Assert.AreEqual(allTaskAssignmentCount - taskAssigmentCount, dbContext.TaskAssignments.Count());
            }
        }

        /// <summary>
        /// Scenario: Delete a project with tasks bound to it. All the tasks should be deleted first in order this operation to succeed.
        /// Delete: Project.
        /// Relationship: 1 - * / Project - Task.
        /// Conditions: Navigation property Tasks in Project is not dependant.
        /// Outcome: Project is not deleted because there are tasks bound to that project.
        /// </summary>
        [TestMethod]
        public void One_To_Many_DeleteParent_IsDependantFalseInParent()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Task task = dbContext.Tasks.FirstOrDefault();
                Assert.IsNotNull(task, MessageHelper.NoRecordsInDatabase(typeof(Task)));

                Project project = task.Project;

                dbContext.Delete(project);

                // Here no exception is thrown because actually neighter the project nor the project tasks will be deleted.
                task.PercentCompleted = 12;

                AssertException.Throws<Telerik.OpenAccess.Exceptions.DataStoreException>(() => dbContext.SaveChanges());

                dbContext.Delete(project.Tasks);
                dbContext.Delete(project);
                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Scenario: Delete a TaskAssignment and its parent Task.
        /// Delete: TaskAssignment.
        /// Relationship: 1 - * / Task - TaskAssignment.
        /// Conditions: Navigation property Task in TaskAssignment is dependant.
        /// Outcome: TaskAssignment and Task are deleted.
        /// </summary>
        [TestMethod]
        public void One_To_Many_DeleteChild_IsDependantTrueInChild()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Task task = dbContext.Tasks.FirstOrDefault();
                Assert.IsNotNull(task, MessageHelper.NoRecordsInDatabase(typeof(Task)));
                
                TaskAssignment taskAssignment = task.TaskAssignments.FirstOrDefault();
                Assert.IsNotNull(taskAssignment, MessageHelper.NoRecordsInDatabase(typeof(TaskAssignment)));
                
                int allTaskCount = dbContext.Tasks.Count();
                int allTaskAssignmentCount = dbContext.TaskAssignments.Count();
                int taskAssigmentCount = task.TaskAssignments.Count();

                dbContext.Delete(taskAssignment);

                AssertException.Throws<Telerik.OpenAccess.Exceptions.InvalidOperationException>(() => task.Priority = 1);

                dbContext.SaveChanges();

                Assert.AreEqual(allTaskCount - 1, dbContext.Tasks.Count());
                Assert.AreEqual(allTaskAssignmentCount - taskAssigmentCount, dbContext.TaskAssignments.Count());
            }
        }

        /// <summary>
        /// Scenario: Delete a task without its parent project being affected.
        /// Delete: Task.
        /// Relationship: 1 - * / Project - Tasks.
        /// Conditions: Navigation property Project in Task is not dependant.
        /// Outcome: Task is deleted, Project is not deleted.
        /// </summary>
        [TestMethod]
        public void One_To_Many_DeleteChild_IsDependantFalseInChild()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Task task = dbContext.Tasks.FirstOrDefault();
                Assert.IsNotNull(task, MessageHelper.NoRecordsInDatabase(typeof(Task)));
                
                Project project = task.Project;

                dbContext.Delete(task);

                // No exception is thrown here
                project.Budget = 12;

                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Scenario: Delete a project.
        /// Detele: Project.
        /// Relationship: * - * / Project - Employee.
        /// Conditions: Navigation property Employees of project is managed.
        /// Outcome: The deleted project is removed from the collection employee.Projects.
        /// </summary>
        [TestMethod]
        public void Many_To_Many_IsManagedTrue()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Project project = dbContext.Projects.FirstOrDefault();
                Assert.IsNotNull(project, MessageHelper.NoRecordsInDatabase(typeof(Project)));

                Employee employee = project.Employees.FirstOrDefault();
                Assert.IsNotNull(employee, MessageHelper.NoRecordsInDatabase(typeof(Employee)));
                
                int projectCountForEmployee = employee.Projects.Count;
                                                             
                dbContext.Delete(project);

                Assert.AreEqual(employee.Projects.Count, projectCountForEmployee - 1);
            }
        }

        /// <summary>
        /// Scenario: Delete a project.
        /// Delete: Manager.
        /// Relationship: * - * / Project - DocumentMetadatas.
        /// Conditions: Navigation property DocumentMetadata in Project is not managed.
        /// Outcome: The DocumentMetadatas of the project are not deleted but cannot be accessed.
        /// </summary>
        [TestMethod]
        public void Many_To_Many_IsManagedFalse()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Project project = dbContext.Projects.FirstOrDefault();
                Assert.IsNotNull(project, MessageHelper.NoRecordsInDatabase(typeof(Project)));

                int projectId = project.ProjectId;

                IList<DocumentMetadata> docs = project.DocumentMetadatum;

                dbContext.Delete(project);

                foreach (DocumentMetadata doc in docs)
                {
                    // Even after the project is marked for deletion it is not removed from the Projects collection of doc, because IsManaged is false
                    Project projectMarkedForDeletion = doc.Projects.FirstOrDefault(p => p.ProjectId == projectId);
                    Assert.IsNotNull(projectMarkedForDeletion);
                    Assert.AreSame(project, projectMarkedForDeletion);
                    AssertException.Throws<Telerik.OpenAccess.Exceptions.InvalidOperationException>(() => project.Title = "");
                }
            }
        }

        /// <summary>
        /// Scenario: Delete a manager and all employees that are managed by the manager.
        /// Delete: Manager.
        /// Relationship: 1 - * / Manager - Employee.
        /// Conditions: Navigation property Employees in Employee is managed.
        /// Outcome: The manager and all of its employees are deleted.
        /// </summary>
        [TestMethod]
        public void One_To_Many_SelfReference_DeleteParent_IsManagedTrue()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Manager manager = dbContext.Managers.FirstOrDefault();
                Assert.IsNotNull(manager, MessageHelper.NoRecordsInDatabase(typeof(Manager)));
                
                IList<Employee> employees = manager.Employees;

                dbContext.Delete(manager);

                // Theese employees are marked for deletion
                foreach (var employee in employees)
                {
                    AssertException.Throws<Telerik.OpenAccess.Exceptions.InvalidOperationException>(() => employee.Title = "");
                }
            }
        }

        /// <summary>
        /// Scenario: Delete an employee.
        /// Delete: Employee.
        /// Relationship: 1 - * / Manager - Employee.
        /// Conditions: Navigation property Employees in Employee is managed.
        /// Outcome: The property Employees of the manager will be changed and the deleted employee will be removed from that collection.
        /// </summary>
        [TestMethod]
        public void One_To_Many_SelfReference_DeleteChild_IsManagedTrue()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Manager manager = dbContext.Managers.FirstOrDefault();
                Assert.IsNotNull(manager, MessageHelper.NoRecordsInDatabase(typeof(Manager)));
                
                IList<Employee> employees = manager.Employees;

                int employeesCountBeforeDelete = manager.Employees.Count;

                Employee employeeToDelete = employees.FirstOrDefault();
                Assert.IsNotNull(employeeToDelete, MessageHelper.NoRecordsInDatabase(typeof(Employee)));

                dbContext.Delete(employeeToDelete);

                Assert.AreEqual(employeesCountBeforeDelete - 1, manager.Employees.Count);
            }
        }
    }
}