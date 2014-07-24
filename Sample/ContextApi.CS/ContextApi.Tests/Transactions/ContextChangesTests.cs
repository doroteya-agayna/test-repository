using System;
using System.Collections.Generic;
using System.Linq;
using ContextApi.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.OpenAccess;

namespace ContextApi.Tests.Transactions
{
    [TestClass]
    public class ContextChangesTests : UnitTestsBase
    {
        /// <summary>
        /// Scenario: Get all objects that are going to be inserted in the database    
        /// GetInserts: Returns a list of new objects managed by the context. The changes are not yet committed to the database.
        /// </summary>
        [TestMethod]
        public void GetInsertChanges()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Project project = dbContext.Projects.FirstOrDefault();
                Assert.IsNotNull(project, MessageHelper.NoRecordsInDatabase(typeof(Project)));

                dbContext.Add(new Bug() { ProjectId = project.ProjectId });
                dbContext.Add(new NewItem() { ProjectId = project.ProjectId, StartTime = DateTime.Now, ReadyFor = DateTime.Now });
                dbContext.Add(new Employee() { LastName = "Smith" });

                AssertExactNumberOfChangesInContext(dbContext, 3, 0, 0);
                
                ContextChanges contextChanges = dbContext.GetChanges();
                IList<object> allInserts = contextChanges.GetInserts<object>();                       
                Assert.AreEqual(3, allInserts.Count);

                foreach (object objectToBeInserted in allInserts)
                {
                    ObjectState state = dbContext.GetState(objectToBeInserted);
                    Assert.IsTrue(state == ObjectState.New);
                }

                Assert.AreEqual(1, allInserts.Count(insert => insert is Bug));
                Assert.AreEqual(1, allInserts.Count(insert => insert is NewItem));
                Assert.AreEqual(1, allInserts.Count(insert => insert is Employee));

                IList<Task> tasksToInsert = contextChanges.GetInserts<Task>();
                Assert.AreEqual(2, tasksToInsert.Count);
                Assert.AreEqual(1, tasksToInsert.Count(insert => insert is Bug));
                Assert.AreEqual(1, tasksToInsert.Count(insert => insert is NewItem));

                IList<Bug> bugsToInsert = contextChanges.GetInserts<Bug>();
                Assert.AreEqual(1, bugsToInsert.Count);
                Assert.AreEqual(1, bugsToInsert.Count(insert => insert is Bug));

                dbContext.SaveChanges();

                AssertNoPendingChangesInContext(dbContext);
            }
        }

        /// <summary>
        /// Scenario: Get all objects that are going to be updated in the database    
        /// GetUpdates: Returns a list of dirty objects managed by the context. The changes are not yet committed to the database.
        /// </summary>
        [TestMethod]
        public void GetUpdateChanges()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Bug bug = dbContext.Bugs.FirstOrDefault();                
                NewItem newItem = dbContext.NewItems.FirstOrDefault();
                Employee employee = dbContext.Employees.FirstOrDefault();

                Assert.IsNotNull(bug, MessageHelper.NoRecordsInDatabase(typeof(Bug)));
                Assert.IsNotNull(newItem, MessageHelper.NoRecordsInDatabase(typeof(NewItem)));
                Assert.IsNotNull(employee, MessageHelper.NoRecordsInDatabase(typeof(Employee)));
                                
                bug.Impact = 1;
                newItem.Title = "New Title";
                employee.FirstName = "New First Name";

                AssertExactNumberOfChangesInContext(dbContext, 0, 3, 0);

                ContextChanges contextChanges = dbContext.GetChanges();
                IList<object> allUpdates = contextChanges.GetUpdates<object>();
                Assert.AreEqual(3, allUpdates.Count);

                foreach (object objectToBeUpdated in allUpdates)
                {
                    ObjectState state = dbContext.GetState(objectToBeUpdated);
                    Assert.IsTrue(state == ObjectState.Dirty);
                }

                Assert.AreEqual(1, allUpdates.Count(update => update is Bug));
                Assert.AreEqual(1, allUpdates.Count(update => update is NewItem));
                Assert.AreEqual(1, allUpdates.Count(update => update is Employee));

                IList<Task> tasksToUpdate = contextChanges.GetUpdates<Task>();
                Assert.AreEqual(2, tasksToUpdate.Count);
                Assert.AreEqual(1, tasksToUpdate.Count(update => update is Bug));
                Assert.AreEqual(1, tasksToUpdate.Count(update => update is NewItem));

                IList<Bug> bugsToUpdate = contextChanges.GetUpdates<Bug>();
                Assert.AreEqual(1, bugsToUpdate.Count);
                Assert.AreEqual(1, bugsToUpdate.Count(insert => insert is Bug));

                dbContext.SaveChanges();

                AssertNoPendingChangesInContext(dbContext);
            }
        }

        /// <summary>
        /// Scenario: Get all objects that are going to be deleted in the database    
        /// GetDeletes: Returns a list of deleted objects managed by the context. The changes are not yet committed to the database.
        /// </summary>
        [TestMethod]
        public void GetDeleteChanges()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Bug bug = dbContext.Bugs.FirstOrDefault();
                NewItem newItem = dbContext.NewItems.FirstOrDefault();
                Document document = dbContext.Documents.FirstOrDefault();

                Assert.IsNotNull(bug, MessageHelper.NoRecordsInDatabase(typeof(Bug)));
                Assert.IsNotNull(newItem, MessageHelper.NoRecordsInDatabase(typeof(NewItem)));
                Assert.IsNotNull(document, MessageHelper.NoRecordsInDatabase(typeof(Document)));

                dbContext.Delete(bug);
                dbContext.Delete(newItem);
                dbContext.Delete(document);

                AssertExactNumberOfChangesInContext(dbContext, 0, 0, 3);

                ContextChanges contextChanges = dbContext.GetChanges();
                IList<object> allDeletes = contextChanges.GetDeletes<object>();
                Assert.AreEqual(3, allDeletes.Count);

                foreach (object objectToBeDeleted in allDeletes)
                {
                    ObjectState state = dbContext.GetState(objectToBeDeleted);
                    Assert.IsTrue(state == ObjectState.Deleted);
                }

                Assert.AreEqual(1, allDeletes.Count(delete => delete is Bug));
                Assert.AreEqual(1, allDeletes.Count(delete => delete is NewItem));
                Assert.AreEqual(1, allDeletes.Count(delete => delete is Document));

                IList<Task> tasksToDelete = contextChanges.GetDeletes<Task>();
                Assert.AreEqual(2, tasksToDelete.Count);
                Assert.AreEqual(1, tasksToDelete.Count(delete => delete is Bug));
                Assert.AreEqual(1, tasksToDelete.Count(delete => delete is NewItem));

                IList<Bug> bugsToDelete = contextChanges.GetDeletes<Bug>();
                Assert.AreEqual(1, bugsToDelete.Count);
                Assert.AreEqual(1, bugsToDelete.Count(delete => delete is Bug));

                dbContext.SaveChanges();

                AssertNoPendingChangesInContext(dbContext);
            }
        }

        /// <summary>
        /// Scenario: Save all changes that have been made to the database.
        /// SaveChanges: Saves the changes with the specified concurency mode
        /// </summary>
        [TestMethod]
        public void SaveChanges()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Bug bug = dbContext.Bugs.FirstOrDefault();             
                Employee employee = dbContext.Employees.FirstOrDefault();
                Task task = new Task() { ProjectId = dbContext.Projects.FirstOrDefault().ProjectId };

                Assert.IsNotNull(bug, MessageHelper.NoRecordsInDatabase(typeof(Bug)));
                Assert.IsNotNull(employee, MessageHelper.NoRecordsInDatabase(typeof(Employee)));
                Assert.IsNotNull(task, MessageHelper.NoRecordsInDatabase(typeof(Task)));

                int bugId = bug.TaskId;

                employee.FirstName = "New First Name";
                dbContext.Add(task);
                dbContext.Delete(bug);

                AssertExactNumberOfChangesInContext(dbContext, 1, 1, 1);

                dbContext.SaveChanges();

                Assert.IsFalse(dbContext.HasChanges);

                // Check that changes are commited to the database
                Employee employeeFromDb = dbContext.Employees.FirstOrDefault();
                Assert.IsNotNull(employeeFromDb, MessageHelper.NoRecordsInDatabase(typeof(Employee)));
                Assert.AreEqual(employee.FirstName, employeeFromDb.FirstName);

                Task taskFromDb = dbContext.Tasks.LastOrDefault();
                Assert.IsNotNull(taskFromDb, MessageHelper.NoRecordsInDatabase(typeof(Task)));
                Assert.AreEqual(task, taskFromDb);

                Assert.IsFalse(dbContext.Bugs.Any(b => b.TaskId == bugId));

                AssertNoPendingChangesInContext(dbContext);
            }
        }

        /// <summary>
        /// Scenario: Clear all changes that have been made.
        /// ClearChanges: Rolls back all changes in the context.
        /// </summary>
        [TestMethod]
        public void ClearChanges()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Bug bug = dbContext.Bugs.FirstOrDefault();
                Employee employee = dbContext.Employees.FirstOrDefault();
                Task task = new Task() { ProjectId = dbContext.Projects.FirstOrDefault().ProjectId };

                Assert.IsNotNull(bug, MessageHelper.NoRecordsInDatabase(typeof(Bug)));
                Assert.IsNotNull(employee, MessageHelper.NoRecordsInDatabase(typeof(Employee)));
                Assert.IsNotNull(task, MessageHelper.NoRecordsInDatabase(typeof(Task)));
                
                string oldEmployeeFirstName = employee.FirstName;
                int numberOfTasksBeforeClearChanges = dbContext.Tasks.Count();
                int numberOfBugsBeforeClearChanges = dbContext.Bugs.Count();

                employee.FirstName = "New First Name";
                dbContext.Add(task);
                dbContext.Delete(bug);

                AssertExactNumberOfChangesInContext(dbContext, 1, 1, 1);

                dbContext.ClearChanges();

                Assert.IsFalse(dbContext.HasChanges);

                AssertNoPendingChangesInContext(dbContext);

                dbContext.SaveChanges(); // doesn't matter if SaveChanges() method is called.

                Employee employeeFromDb = dbContext.Employees.FirstOrDefault();
                Assert.IsNotNull(employeeFromDb, MessageHelper.NoRecordsInDatabase(typeof(Employee)));
                Assert.AreEqual(oldEmployeeFirstName, employeeFromDb.FirstName);

                Assert.AreEqual(numberOfBugsBeforeClearChanges, dbContext.Bugs.Count());

                Assert.AreEqual(numberOfTasksBeforeClearChanges, dbContext.Tasks.Count());
            }
        }

        /// <summary>
        ///  Scenario: Temporary save all current changes to the database.
        ///  FlushChanges: Flushes all current changes to the database but keeps the transaction running.
        /// </summary>
        [TestMethod]
        public void FlushChanges()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Bug bug = dbContext.Bugs.FirstOrDefault();
                Employee employee = dbContext.Employees.FirstOrDefault();
                Task task = new Task() { ProjectId = dbContext.Projects.FirstOrDefault().ProjectId };

                Assert.IsNotNull(bug, MessageHelper.NoRecordsInDatabase(typeof(Bug)));
                Assert.IsNotNull(employee, MessageHelper.NoRecordsInDatabase(typeof(Employee)));
                Assert.IsNotNull(task, MessageHelper.NoRecordsInDatabase(typeof(Task)));
                
                employee.FirstName = "New First Name";
                dbContext.Add(task);
                dbContext.Delete(bug);

                AssertExactNumberOfChangesInContext(dbContext, 1, 1, 1);

                dbContext.FlushChanges();

                AssertNoPendingChangesInContext(dbContext);

                Assert.IsTrue(dbContext.HasChanges);                
            }
        }

        private void AssertExactNumberOfChangesInContext(OpenAccessContext dbContext, int expectedInserts, int expectedUpdates, int expectedDeletes)
        {
            ContextChanges contextChanges = dbContext.GetChanges();

            IList<object> allUpdates = contextChanges.GetUpdates<object>();
            IList<object> allDeletes = contextChanges.GetDeletes<object>();
            IList<object> allInserts = contextChanges.GetInserts<object>();

            Assert.AreEqual(expectedInserts, allInserts.Count);         
            Assert.AreEqual(expectedUpdates, allUpdates.Count); 
            Assert.AreEqual(expectedDeletes, allDeletes.Count);
        }

        private void AssertNoPendingChangesInContext(OpenAccessContext dbContext)
        {
            AssertExactNumberOfChangesInContext(dbContext, 0, 0, 0);
        }
    }
}