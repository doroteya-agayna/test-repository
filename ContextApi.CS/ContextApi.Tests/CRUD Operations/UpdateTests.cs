using System.Collections.Generic;
using System.Linq;
using ContextApi.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ContextApi.Tests.CrudOperations
{
    [TestClass]
    public class UpdateTests : UnitTestsBase
    {
        /// <summary>
        /// Scenario: Set the property of an object to a new value and save it.
        /// SaveChanges: Saves all changes in the context.
        /// <see cref="http://documentation.telerik.com/openaccess-orm/developers-guide/crud-operations/developer-guide-crud-saving"/>
        /// </summary>
        [TestMethod]
        public void Update_SaveChanges()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Employee employee = dbContext.Employees.LastOrDefault();
                Assert.IsNotNull(employee, MessageHelper.NoRecordsInDatabase(typeof(Employee)));

                IList<TaskAssignment> tasks = employee.TaskAssignments;

                string newName = "Jane";

                employee.FirstName = newName;

                dbContext.SaveChanges();

                foreach (TaskAssignment taskAssignment in tasks)
                {
                    Assert.AreSame(taskAssignment.Employee, employee);
                }

                Employee employeeFromDb = dbContext.Employees.LastOrDefault();
                Assert.IsNotNull(employeeFromDb, MessageHelper.NoRecordsInDatabase(typeof(Employee)));

                Assert.AreEqual(newName, employeeFromDb.FirstName);
            }
        }
                      
        /// <summary>
        /// Scenario: Set property of an object to new values and discard the changes.
        /// ClearChanges: Rolls back all changes in the context.
        /// <see cref="http://documentation.telerik.com/openaccess-orm/feature-reference/api/context-api/feature-ref-api-context-api-handling-transactions#rollbacking_changes"/>
        /// </summary>
        [TestMethod]
        public void Update_ClearChanges()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Employee employee = dbContext.Employees.FirstOrDefault();
                Assert.IsNotNull(employee, MessageHelper.NoRecordsInDatabase(typeof(Employee)));

                string oldName = employee.FirstName;
                string newName = "Jane";

                employee.FirstName = newName;

                dbContext.ClearChanges();

                Employee employeeFromDb = dbContext.Employees.FirstOrDefault();
                Assert.IsNotNull(employeeFromDb, MessageHelper.NoRecordsInDatabase(typeof(Employee)));

                Assert.AreEqual(oldName, employeeFromDb.FirstName);
            }
        }

        /// <summary>
        /// Scenario: Set properties of an object to new values and save the changes but keep the transaction running. 
        /// If SaveChanges is not called those changes will be rolled back.
        /// FlushChanges: Flushes all current changes to the database but keeps the transaction running.
        /// <see cref="http://documentation.telerik.com/openaccess-orm/feature-reference/api/context-api/feature-ref-api-context-api-handling-transactions#flushing_changes"/>
        /// </summary>
        [TestMethod]
        public void Update_FlushChanges_WithoutSaveChanges()
        {
            string oldName = null;

            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Employee employee = dbContext.Employees.FirstOrDefault();
                Assert.IsNotNull(employee, MessageHelper.NoRecordsInDatabase(typeof(Employee)));

                oldName = employee.FirstName;
                string newName = "Jane";

                employee.FirstName = newName;
                // Flushes all current changes to the database but keeps the transaction running.
                dbContext.FlushChanges();

                employee = dbContext.Employees.FirstOrDefault();
                Assert.IsNotNull(employee, MessageHelper.NoRecordsInDatabase(typeof(Employee)));
                Assert.AreEqual(newName, employee.FirstName);
            }

            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Employee employee = dbContext.Employees.FirstOrDefault();
                Assert.IsNotNull(employee, MessageHelper.NoRecordsInDatabase(typeof(Employee)));

                Assert.AreEqual(oldName, employee.FirstName);
            }
        }

        /// <summary>
        /// Scenario: Set properties of an object to new values and save the changes but keep the transaction running. 
        /// If SaveChanges is not called those changes will be rolled back.
        /// </summary>
        [TestMethod]
        public void Update_FlushChanges_SaveChangesCalled()
        {
            string newName = "Jane";

            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Employee employee = dbContext.Employees.FirstOrDefault();
                Assert.IsNotNull(employee, MessageHelper.NoRecordsInDatabase(typeof(Employee)));
                            
                employee.FirstName = newName;

                dbContext.FlushChanges();

                employee = dbContext.Employees.FirstOrDefault();
                Assert.IsNotNull(employee, MessageHelper.NoRecordsInDatabase(typeof(Employee)));

                Assert.AreEqual(newName, employee.FirstName);

                dbContext.SaveChanges();
            }

            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Employee employee = dbContext.Employees.FirstOrDefault();
                Assert.IsNotNull(employee, MessageHelper.NoRecordsInDatabase(typeof(Employee)));

                Assert.AreEqual(newName, employee.FirstName);
            }
        }
    }
}