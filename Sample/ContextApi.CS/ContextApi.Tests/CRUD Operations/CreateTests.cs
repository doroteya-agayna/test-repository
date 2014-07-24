using System;
using System.Linq;
using ContextApi.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ContextApi.Tests.CRUD_Operations
{
    [TestClass]
    public class CreateTests : UnitTestsBase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            ContextOperations.ClearAllEntites();
        }

        /// <summary>
        /// Add: Employee.
        /// Relationship: None.
        /// Conditions: None.
        /// Outcome: Adding a single element in the database.
        /// </summary>
        [TestMethod]
        public void AddIndependentElement()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Employee employee = new Employee()
                {
                    FirstName = "Adam",
                    LastName = "Draven"
                };

                int employeesCountBeforeAdd = dbContext.Employees.Count();

                dbContext.Add(employee);
                dbContext.SaveChanges();

                int employeesCountAfterAdd = dbContext.Employees.Count();
                Assert.AreEqual(employeesCountBeforeAdd + 1, employeesCountAfterAdd);
                Employee employeeFromDb = dbContext.Employees.FirstOrDefault();
                Assert.IsNotNull(employeeFromDb, MessageHelper.NoRecordsInDatabase(typeof(Employee)));

                Assert.AreSame(employee, employeeFromDb);
            }
        }

        /// <summary>
        /// Add: Project.
        /// Relationship: 1 - * / Project - Tasks.
        /// Conditions: Navigation property Tasks in Project is not managed.
        /// Outcome: Project reference in Task is Null.
        /// </summary>
        [TestMethod]
        public void Add_OneToMany_IsManagedFalse_NoSynchronization()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Project project = new Project()
                {
                    StartDate = DateTime.Now,
                    Title = "New Project",
                    Manager = "Peter The Manager"
                };

                Task task = new Task()
                {
                    Status = "Not Done",
                    Title = "Project Overview"
                };

                dbContext.Add(project);
                project.Tasks.Add(task);

                Assert.IsNull(task.Project);
                AssertException.Throws<Telerik.OpenAccess.Exceptions.DataStoreException>(() => dbContext.SaveChanges());      
            }
        }

        /// <summary>
        /// Add: Project.
        /// Relationship: 1 - * / Project - Tasks.
        /// Conditions: Navigation property Tasks in Project is not managed.
        /// Conditions: Project reference should be added to task manually.
        /// Outcome: Adding Project and Task in the database.
        /// </summary>
        [TestMethod]
        public void Add_OneToMany_IsManagedFalse()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Project project = new Project()
                {
                    StartDate = DateTime.Now,
                    Title = "New Project",
                    Manager = "Peter The Manager"
                };

                Task task = new Task()
                {
                    Status = "Not Done",
                    Title = "Project Overview"
                };

                int projectsCountBeforeAdd = dbContext.Projects.Count();
                int tasksCountBeforeAdd = dbContext.Tasks.Count();

                dbContext.Add(project);

                project.Tasks.Add(task);
                task.Project = project;

                dbContext.SaveChanges();

                int projectsCountAfterAdd = dbContext.Projects.Count();
                int tasksCountAfterAdd = dbContext.Tasks.Count();

                Assert.AreEqual(projectsCountBeforeAdd + 1, projectsCountAfterAdd);
                Assert.AreEqual(tasksCountBeforeAdd + 1, tasksCountAfterAdd);

                Project projectFromDb = dbContext.Projects.FirstOrDefault();
                Assert.IsNotNull(projectFromDb, MessageHelper.NoRecordsInDatabase(typeof(Project)));

                Assert.AreSame(project, projectFromDb);

                Task taskFromDb = dbContext.Tasks.FirstOrDefault();
                Assert.AreSame(task, taskFromDb);
            }
        }
       
        /// <summary>
        /// Add: Employee
        /// Relationship: 1 - * / Employee - DailyReport
        /// Conditions: Navigation property DailyReport in Employee is managed
        /// Outcome: Adding a new Employee with DailyReprot will add Employee and DailyReport to the database
        /// </summary>
        [TestMethod]
        public void Add_OneToMany_IsManagedTrue()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Employee employee = new Employee()
                {
                    FirstName = "Andre",
                    LastName = "Cage",
                    Title = "Worker"
                };

                DailyReport dailyReport = new DailyReport()
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(4),
                    Title = "Work Report"
                };

                dbContext.Add(employee);
                employee.DailyReports.Add(dailyReport);

                int employeeCountBeforeAdd = dbContext.Employees.Count();
                int dailyReportCountBeforeAdd = dbContext.DailyReports.Count();

                dbContext.SaveChanges();

                int employeeCountAfterAdd = dbContext.Employees.Count();
                int dailyReportCountAfterAdd = dbContext.DailyReports.Count();

                Assert.AreEqual(employeeCountAfterAdd, employeeCountBeforeAdd + 1);
                Assert.AreEqual(dailyReportCountAfterAdd, dailyReportCountBeforeAdd + 1);
            }
        }

        /// <summary>
        /// Add: Employee
        /// Relationship: 1 - * / Employee - DailyReport
        /// Conditions: Navigation property DailyReport in Employee is managed
        /// Outcome: Adding a new Employee with DailyReprot will add Employee and DailyReport to the database
        /// </summary>
        [TestMethod]
        public void Add_OneToMany_IsManagedTrue_Synchronization()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Employee employee = new Employee()
                {
                    FirstName = "Andre",
                    LastName = "Cage",
                    Title = "Worker"
                };

                DailyReport dailyReport = new DailyReport()
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(4),
                    Title = "Work Report"
                };

                dbContext.Add(employee);
                employee.DailyReports.Add(dailyReport);
                Assert.IsNotNull(dailyReport.Employee);

                dbContext.SaveChanges(); // ?
            }
        }        

        /// <summary>
        /// Add: Employee.
        /// Relationship: 1 - * / Employee - DailyReport.
        /// Conditions: Navigation property DailyReport in Employee is managed.
        /// Outcome: Employee property in DailyReport is null even when IsManaged is true because the employee is not added to a context.
        /// </summary>
        [TestMethod]
        public void Add_OneToMany_IsManagedTrue_NoSynchronization_BeforeAddingToTheContext()
        {
            Employee employee = new Employee()
            {
                FirstName = "Andre",
                LastName = "Cage",
                Title = "Worker"
            };

            DailyReport dailyReport = new DailyReport()
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(4),
                Title = "Work Report"
            };

            employee.DailyReports.Add(dailyReport);
            Assert.IsNull(dailyReport.Employee);
        }

        /// <summary>
        /// Add: Project.
        /// Relationship: * - * / Project - Employee.
        /// Conditions: Navigation properties Employee in Project and Project in Employee are managed.
        /// Conditions: Add Project to Context before add Employee to Project.
        /// Conditions: Add project to a context before adding entities to the collection.
        /// Outcome: Add Project in Employee.Project collection.
        /// </summary>
        [TestMethod]
        public void Add_ManyToMany_IsManagedTrue_Synchronization()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Project project = new Project()
                {
                    StartDate = DateTime.Now,
                    Title = "New Project",
                    Manager = "Peter The Manager"
                };

                Employee employee = new Employee()
                {
                    FirstName = "Andre",
                    LastName = "Cage",
                    Title = "Worker"
                };

                dbContext.Add(project);
                project.Employees.Add(employee);

                Assert.AreEqual(1, employee.Projects.Count());
            }
        }

        /// <summary>
        /// Add: Employee.
        /// Relationship: * - * / Project - Employee.
        /// Conditions: Navigation property Employee in Project is managed.
        /// Outcome: The project collection of an amployee is empty event after that employee is added to a prject because the project is not added to a context.
        /// </summary>
        [TestMethod]
        public void Add_ManyToMany_IsManagedTrue_NoSynchronization()
        {
            Project project = new Project()
            {
                StartDate = DateTime.Now,
                Title = "New Project",
                Manager = "Peter The Manager"
            };

            Employee employee = new Employee()
            {
                FirstName = "Andre",
                LastName = "Cage",
                Title = "Worker"
            };

            project.Employees.Add(employee);
            Assert.AreEqual(0, employee.Projects.Count());
        }

        /// <summary>
        /// Add: Project.
        /// Relationship: * - * / Project - Employee.
        /// Conditions: Navigation property Employee in Project and Project in Employee are managed.
        /// Conditions: Add Project to Context before add Employee to Project.
        /// Outcome: Add Project in Employee.Project collection.
        /// </summary>
        [TestMethod]
        public void Add_ManyToMany_IsManagedTrue_AddBothElementsToDatabase()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Project project = new Project()
                {
                    StartDate = DateTime.Now,
                    Title = "New Project",
                    Manager = "Peter The Manager"
                };

                Employee employee = new Employee()
                {
                    FirstName = "Andre",
                    LastName = "Cage",
                    Title = "Worker"
                };

                dbContext.Add(project);
                project.Employees.Add(employee);

                int projectsCountBeforeAdd = dbContext.Projects.Count();
                int employeeCountBeforeAdd = dbContext.Employees.Count();

                dbContext.SaveChanges();

                int projectsCountAfterAdd = dbContext.Projects.Count();
                int employeeCountAfterAdd = dbContext.Employees.Count();

                Assert.AreEqual(projectsCountBeforeAdd + 1, projectsCountAfterAdd);
                Assert.AreEqual(employeeCountBeforeAdd + 1, employeeCountAfterAdd);
            }
        }

        /// <summary>
        /// Add: DocumentMetadata.
        /// Relationship: * - * / Project - DocumentMetadata.
        /// Conditions: Navigation property DocumentMetadata in Project is not managed.
        /// Outcome: Project and DocumentMetadata are saved in the database.
        /// </summary>
        [TestMethod]
        public void Add_ManyToMany_IsManagedFalse_AddProjectAndDocument()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Project project = new Project()
                {
                    StartDate = DateTime.Now,
                    Title = "New Project",
                    Manager = "Peter The Manager"
                };

                DocumentMetadata docMetadata = new DocumentMetadata()
                {
                    Author = "Andre The Author",
                    Title = "Start up "
                };

                dbContext.Add(docMetadata);
                docMetadata.Projects.Add(project);

                int projectsCountBeforeAdd = dbContext.Projects.Count();
                int docMetadataCountBeforeAdd = dbContext.DocumentMetadatum.Count();

                dbContext.SaveChanges();

                int projectsCountAfterAdd = dbContext.Projects.Count();
                int docMetadataCountAfterAdd = dbContext.DocumentMetadatum.Count();

                Assert.AreEqual(projectsCountBeforeAdd + 1, projectsCountAfterAdd);
                Assert.AreEqual(docMetadataCountBeforeAdd + 1, docMetadataCountAfterAdd);
            }
        }

        /// <summary>
        /// Add: Project.
        /// Relationship: * - * / Project - DocumentMetadata.
        /// Conditions: Navigation property DocumentMetadata in Project is not managed.
        /// Outcome: The collection projects in documentMetadata is empty even after adding a documentMetadata
        /// to a project because IsManaged property is false.
        /// </summary>
        [TestMethod]
        public void Add_ManyToMany_IsManagedFalse_NoSynchronization()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Project project = new Project()
                {
                    StartDate = DateTime.Now,
                    Title = "New Project",
                    Manager = "Peter The Manager"
                };

                DocumentMetadata docMetadata = new DocumentMetadata()
                {
                    Author = "Andre The Author",
                    Title = "Start up "
                };

                dbContext.Add(project);
                project.DocumentMetadatum.Add(docMetadata);

                Assert.AreEqual(0, docMetadata.Projects.Count);
            }
        }
            
        /// <summary>
        /// Add: DocumentMetadata.
        /// Relationship: * - * / Project - DocumentMetadata.
        /// Conditions: Navigation property Employee in Project is not managed.
        /// Outcome: Adding a new Project with Employee will add Project to the database.
        /// </summary>
        [TestMethod]
        public void Add_ManyToMany_IsManagedFalse_NoAssociationInTheDatabase()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Project project = new Project()
                {
                    StartDate = DateTime.Now,
                    Title = "New Project",
                    Manager = "Peter The Manager"
                };

                DocumentMetadata docMetadata = new DocumentMetadata()
                {
                    Author = "Andre The Author",
                    Title = "Start up "
                };

                dbContext.Add(docMetadata);
                dbContext.Add(project);
                docMetadata.Projects.Add(project);

                dbContext.SaveChanges();

                Project projectInDatabase = dbContext.Projects.FirstOrDefault();
                Assert.IsNotNull(projectInDatabase, MessageHelper.NoRecordsInDatabase(typeof(Project)));

                DocumentMetadata docMetaInDatabase = dbContext.DocumentMetadatum.FirstOrDefault();
                Assert.IsNotNull(docMetaInDatabase, MessageHelper.NoRecordsInDatabase(typeof(DocumentMetadata)));

                Assert.AreEqual(0, projectInDatabase.DocumentMetadatum.Count());
                Assert.AreEqual(0, docMetaInDatabase.Projects.Count());
            }
        }
      
        /// <summary>
        /// Add: Employee
        /// Relationship: * - * / Project - DocumentMetadata
        /// Conditions: Navigation property DocumentMetadata in Project is not managed
        /// Outcome: If the DocumentMetadata is added to a context it will be saved in the database.
        /// </summary>
        [TestMethod]
        public void Add_ManyToMany_IsManagedFalse_WithAssociationInTheDatabase()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Project project = new Project()
                {
                    StartDate = DateTime.Now,
                    Title = "New Project",
                    Manager = "Peter The Manager"
                };

                DocumentMetadata docMetadata = new DocumentMetadata()
                {
                    Author = "Andre The Author",
                    Title = "Start up "
                };

                dbContext.Add(docMetadata);
                dbContext.Add(project);
                docMetadata.Projects.Add(project);
                project.DocumentMetadatum.Add(docMetadata);

                dbContext.SaveChanges();

                Project projectInDatabase = dbContext.Projects.FirstOrDefault();
                Assert.IsNotNull(projectInDatabase, MessageHelper.NoRecordsInDatabase(typeof(Project)));

                DocumentMetadata docMetaInDatabase = dbContext.DocumentMetadatum.FirstOrDefault();
                Assert.IsNotNull(docMetaInDatabase, MessageHelper.NoRecordsInDatabase(typeof(DocumentMetadata)));

                Assert.AreEqual(1, projectInDatabase.DocumentMetadatum.Count());
                Assert.AreEqual(1, docMetaInDatabase.Projects.Count());
            }
        }

        /// <summary>
        /// Scenario: Set the relational property in slave to the master record.
        /// Outcome: The IsManaged property doesn't matter in this situation. The master and slave records are always synchronized.
        /// </summary>
        [TestMethod]
        public void Add_OneToOne_IsManagedFalse_SetRelationInSlave_AddToContextFirst()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Document doc = new Document()
                {
                    Checksum = 10
                };

                DocumentMetadata docMeta = new DocumentMetadata()
                {
                    Title = "New Document",
                    Created = DateTime.Now
                };

                dbContext.Add(docMeta);
                docMeta.Document = doc;

                Assert.IsNotNull(doc.DocumentMetadatum);
            }
        }

        /// <summary>
        /// Scenario: Set the relational property in master to the slave record.
        /// Outcome: The IsManaged property doesn't matter in this situation. The master and slave records are always synchronized.
        /// </summary>
        [TestMethod]
        public void Add_OneToOne_IsManagedFalse_SetRelationInMaster_AddToContextFirst()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Document doc = new Document()
                {
                    Checksum = 10
                };

                DocumentMetadata docMeta = new DocumentMetadata()
                {
                    Title = "New Document",
                    Created = DateTime.Now
                };

                dbContext.Add(doc);
                doc.DocumentMetadatum = docMeta;

                Assert.IsNotNull(docMeta.Document);
            }
        }
        
        /// <summary>
        /// Scenario: Set the relational property in slave to the master record.
        /// Outcome: Doesn't matter if the slave is added to a context before or after its master property is set.
        /// </summary>
        [TestMethod]
        public void Add_OneToOne_IsManagedFalse_SetRelationInSlave_AddToContextLast()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Document doc = new Document()
                {
                    Checksum = 10
                };

                DocumentMetadata docMeta = new DocumentMetadata()
                {
                    Title = "New Document",
                    Created = DateTime.Now
                };

                docMeta.Document = doc;
                dbContext.Add(docMeta);

                Assert.IsNotNull(doc.DocumentMetadatum);
            }
        }

        /// <summary>
        /// Scenario: Set the relational property in master to the slave record.
        /// Outcome: Doesn't matter if the master is added to a context before or after its slave property is set.
        /// </summary>
        [TestMethod]
        public void Add_OneToOne_IsManagedFalse_SetRelationInParent_AddToContextLast()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Document doc = new Document()
                {
                    Checksum = 10
                };

                DocumentMetadata docMeta = new DocumentMetadata()
                {
                    Title = "New Document",
                    Created = DateTime.Now
                };

                doc.DocumentMetadatum = docMeta;
                dbContext.Add(doc);

                Assert.IsNotNull(docMeta.Document);
            }
        }

        /// <summary>
        /// Scenario: Add two entities in relation one to one.
        /// Outcome: Two entities are added in the database.
        /// </summary>
        [TestMethod]
        public void Add_OneToOne_Test_SaveInDatabase()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Document doc = new Document()
                {
                    Checksum = 10
                };

                DocumentMetadata docMeta = new DocumentMetadata()
                {
                    Title = "New Document",
                    Created = DateTime.Now
                };

                dbContext.Add(docMeta);
                docMeta.Document = doc;

                int docMetaCountBeforeAdd = dbContext.DocumentMetadatum.Count();
                int docCountBeforeAdd = dbContext.Documents.Count();

                dbContext.SaveChanges();

                int docMetaCountAfterAdd = dbContext.DocumentMetadatum.Count();
                int docCountAfterAdd = dbContext.Documents.Count();

                Assert.AreEqual(docMetaCountBeforeAdd + 1, docMetaCountAfterAdd);
                Assert.AreEqual(docCountBeforeAdd + 1, docCountAfterAdd);
            }
        }

        /// <summary>
        /// Scenario: Add an entity and a parent entity. Add only the parent entity to a context.
        /// Outcome: The supervisor property of the employee is set appropriately.
        /// </summary>
        [TestMethod]
        public void Add_SelfRef_AddParentToContext()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Employee supervisor = new Employee()
                {
                    FirstName = "Adam",
                    LastName = "Cage",
                    Department = "Human Resources"
                };

                Employee employee = new Employee()
                {
                    FirstName = "Nicolas",
                    LastName = "Rooney",
                    Department = "Human Resources"
                };

                dbContext.Add(supervisor);
                supervisor.Employees.Add(employee);

                Assert.IsNotNull(employee.Supervisor);
                Assert.AreSame(supervisor, employee.Supervisor);
            }
        }

        /// <summary>
        /// Scenario: Add an entity and a parent entity. Add only the child entity to a context.
        /// Outcome: The Employees collection of the supervisor is populated appropriately.
        /// </summary>
        [TestMethod]
        public void Add_SelfRef_AddChildToContext()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Employee supervisor = new Employee()
                {
                    FirstName = "Adam",
                    LastName = "Cage",
                    Department = "Human Resources"
                };

                Employee employee = new Employee()
                {
                    FirstName = "Nicolas",
                    LastName = "Rooney",
                    Department = "Human Resources"
                };

                dbContext.Add(employee);
                employee.Supervisor = supervisor;

                Assert.AreEqual(1, supervisor.Employees.Count());

                Employee employeeOfSupervisor = supervisor.Employees.FirstOrDefault();
                Assert.IsNotNull(employeeOfSupervisor, MessageHelper.NoRecordsInDatabase(typeof(Employee)));

                Assert.AreSame(employee, employeeOfSupervisor);
            }
        }

    }
}