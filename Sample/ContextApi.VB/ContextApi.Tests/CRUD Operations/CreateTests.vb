Imports System
Imports System.Linq
Imports ContextApi.Model
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports ContextApi.Tests
<TestClass> _
Public Class CreateTests
    Inherits UnitTestsBase


    <TestInitialize> _
    Public Overrides Sub TestInitialize()
        ContextOperations.ClearAllEntites()
    End Sub

    <TestCleanup> _
    Public Overrides Sub TestCleanup()
        MyBase.TestCleanup()
    End Sub

    ''' <summary>
    ''' Add: Employee.
    ''' Relationship: None.
    ''' Conditions: None.
    ''' Result: Adding a single element in the database.
    ''' </summary>
    <TestMethod> _
    Public Sub AddIndependentElement()
        Using dbContext As New EntitiesModel()
            Dim _employee As New Employee() With {.FirstName = "Adam", .LastName = "Draven"}

            Dim employeesCountBeforeAdd As Integer = dbContext.Employees.Count()

            dbContext.Add(_employee)
            dbContext.SaveChanges()

            Dim employeesCountAfterAdd As Integer = dbContext.Employees.Count()
            Assert.AreEqual(employeesCountBeforeAdd + 1, employeesCountAfterAdd)
            Dim employeeFromDb As Employee = dbContext.Employees.FirstOrDefault()
            Assert.IsNotNull(employeeFromDb, MessageHelper.NoRecordsInDatabase(GetType(Employee)))

            Assert.AreSame(_employee, employeeFromDb)
        End Using
    End Sub

    ''' <summary>
    ''' Add: Project.
    ''' Relationship: 1 - * / Project - Tasks.
    ''' Conditions: Navigation property Tasks in Project is not managed.
    ''' Result: Project reference in Task is Null.
    ''' </summary>

    <TestMethod> _
    Public Sub Add_OneToMany_IsManagedFalse_NoSynchronization()
        Using dbContext As New EntitiesModel()
            Dim _project As New Project() With {.StartDate = Date.Now, .Title = "New Project", .Manager = "Peter The Manager"}

            Dim _task As New Task() With {.Status = "Not Done", .Title = "Project Overview"}

            dbContext.Add(_project)
            _project.Tasks.Add(_task)

            Assert.IsNull(_task.Project)

            Dim target As Action = AddressOf dbContext.SaveChanges
            AssertException.Throws(Of Telerik.OpenAccess.Exceptions.DataStoreException)(target)
        End Using
    End Sub

    Public Sub Test()


    End Sub

    ''' <summary>
    ''' Add: Project.
    ''' Relationship: 1 - * / Project - Tasks.
    ''' Conditions: Navigation property Tasks in Project is not managed.
    ''' Conditions: Project reference should be added to task manually.
    ''' Result: Adding Project and Task in the database.
    ''' </summary>
    <TestMethod> _
    Public Sub Add_OneToMany_IsManagedFalse()
        Using dbContext As New EntitiesModel()
            Dim _project As New Project() With {.StartDate = Date.Now, .Title = "New Project", .Manager = "Peter The Manager"}

            Dim _task As New Task() With {.Status = "Not Done", .Title = "Project Overview"}

            Dim projectsCountBeforeAdd As Integer = dbContext.Projects.Count()
            Dim tasksCountBeforeAdd As Integer = dbContext.Tasks.Count()

            dbContext.Add(_project)

            _project.Tasks.Add(_task)
            _task.Project = _project

            dbContext.SaveChanges()

            Dim projectsCountAfterAdd As Integer = dbContext.Projects.Count()
            Dim tasksCountAfterAdd As Integer = dbContext.Tasks.Count()

            Assert.AreEqual(projectsCountBeforeAdd + 1, projectsCountAfterAdd)
            Assert.AreEqual(tasksCountBeforeAdd + 1, tasksCountAfterAdd)

            Dim projectFromDb As Project = dbContext.Projects.FirstOrDefault()
            Assert.IsNotNull(projectFromDb, MessageHelper.NoRecordsInDatabase(GetType(Project)))

            Assert.AreSame(_project, projectFromDb)

            Dim taskFromDb As Task = dbContext.Tasks.FirstOrDefault()
            Assert.AreSame(_task, taskFromDb)
        End Using
    End Sub

    ''' <summary>
    ''' Add: Employee
    ''' Relationship: 1 - * / Employee - DailyReport
    ''' Conditions: Navigation property DailyReport in Employee is managed
    ''' Result: Adding a new Employee with DailyReprot will add Employee and DailyReport to the database
    ''' </summary>
    <TestMethod> _
    Public Sub Add_OneToMany_IsManagedTrue()
        Using dbContext As New EntitiesModel()
            Dim _employee As New Employee() With {.FirstName = "Andre", .LastName = "Cage", .Title = "Worker"}

            Dim _dailyReport As New DailyReport() With {.StartTime = Date.Now, .EndTime = Date.Now.AddHours(4), .Title = "Work Report"}

            dbContext.Add(_employee)
            _employee.DailyReports.Add(_dailyReport)

            Dim employeeCountBeforeAdd As Integer = dbContext.Employees.Count()
            Dim dailyReportCountBeforeAdd As Integer = dbContext.DailyReports.Count()

            dbContext.SaveChanges()

            Dim employeeCountAfterAdd As Integer = dbContext.Employees.Count()
            Dim dailyReportCountAfterAdd As Integer = dbContext.DailyReports.Count()

            Assert.AreEqual(employeeCountAfterAdd, employeeCountBeforeAdd + 1)
            Assert.AreEqual(dailyReportCountAfterAdd, dailyReportCountBeforeAdd + 1)
        End Using
    End Sub

    ''' <summary>
    ''' Add: Employee
    ''' Relationship: 1 - * / Employee - DailyReport
    ''' Conditions: Navigation property DailyReport in Employee is managed
    ''' Result: Adding a new Employee with DailyReprot will add Employee and DailyReport to the database
    ''' </summary>
    <TestMethod> _
    Public Sub Add_OneToMany_IsManagedTrue_Synchronization()
        Using dbContext As New EntitiesModel()
            Dim _employee As New Employee() With {.FirstName = "Andre", .LastName = "Cage", .Title = "Worker"}

            Dim _dailyReport As New DailyReport() With {.StartTime = Date.Now, .EndTime = Date.Now.AddHours(4), .Title = "Work Report"}

            dbContext.Add(_employee)
            _employee.DailyReports.Add(_dailyReport)
            Assert.IsNotNull(_dailyReport.Employee)

            dbContext.SaveChanges() ' ?
        End Using
    End Sub

    ''' <summary>
    ''' Add: Employee.
    ''' Relationship: 1 - * / Employee - DailyReport.
    ''' Conditions: Navigation property DailyReport in Employee is managed.
    ''' Result: Employee property in DailyReport is null even when IsManaged is true because the employee is not added to a context.
    ''' </summary>
    <TestMethod> _
    Public Sub Add_OneToMany_IsManagedTrue_NoSynchronization_BeforeAddingToTheContext()
        Dim _employee As New Employee() With {.FirstName = "Andre", .LastName = "Cage", .Title = "Worker"}

        Dim _dailyReport As New DailyReport() With {.StartTime = Date.Now, .EndTime = Date.Now.AddHours(4), .Title = "Work Report"}

        _employee.DailyReports.Add(_dailyReport)
        Assert.IsNull(_dailyReport.Employee)
    End Sub

    ''' <summary>
    ''' Add: Project.
    ''' Relationship: * - * / Project - Employee.
    ''' Conditions: Navigation properties Employee in Project and Project in Employee are managed.
    ''' Conditions: Add Project to Context before add Employee to Project.
    ''' Conditions: Add project to a context before adding entities to the collection.
    ''' Result: Add Project in Employee.Project collection.
    ''' </summary>
    <TestMethod> _
    Public Sub Add_ManyToMany_IsManagedTrue_Synchronization()
        Using dbContext As New EntitiesModel()
            Dim _project As New Project() With {.StartDate = Date.Now, .Title = "New Project", .Manager = "Peter The Manager"}

            Dim _employee As New Employee() With {.FirstName = "Andre", .LastName = "Cage", .Title = "Worker"}

            dbContext.Add(_project)
            _project.Employees.Add(_employee)

            Assert.AreEqual(1, _employee.Projects.Count())
        End Using
    End Sub

    ''' <summary>
    ''' Add: Employee.
    ''' Relationship: * - * / Project - Employee.
    ''' Conditions: Navigation property Employee in Project is managed.
    ''' Result: The project collection of an amployee is empty event after that employee is added to a prject because the project is not added to a context.
    ''' </summary>
    <TestMethod> _
    Public Sub Add_ManyToMany_IsManagedTrue_NoSynchronization()
        Dim _project As New Project() With {.StartDate = Date.Now, .Title = "New Project", .Manager = "Peter The Manager"}

        Dim _employee As New Employee() With {.FirstName = "Andre", .LastName = "Cage", .Title = "Worker"}

        _project.Employees.Add(_employee)
        Assert.AreEqual(0, _employee.Projects.Count())
    End Sub

    ''' <summary>
    ''' Add: Project.
    ''' Relationship: * - * / Project - Employee.
    ''' Conditions: Navigation property Employee in Project and Project in Employee are managed.
    ''' Conditions: Add Project to Context before add Employee to Project.
    ''' Result: Add Project in Employee.Project collection.
    ''' </summary>
    <TestMethod> _
    Public Sub Add_ManyToMany_IsManagedTrue_AddBothElementsToDatabase()
        Using dbContext As New EntitiesModel()
            Dim _project As New Project() With {.StartDate = Date.Now, .Title = "New Project", .Manager = "Peter The Manager"}

            Dim _employee As New Employee() With {.FirstName = "Andre", .LastName = "Cage", .Title = "Worker"}

            dbContext.Add(_project)
            _project.Employees.Add(_employee)

            Dim projectsCountBeforeAdd As Integer = dbContext.Projects.Count()
            Dim employeeCountBeforeAdd As Integer = dbContext.Employees.Count()

            dbContext.SaveChanges()

            Dim projectsCountAfterAdd As Integer = dbContext.Projects.Count()
            Dim employeeCountAfterAdd As Integer = dbContext.Employees.Count()

            Assert.AreEqual(projectsCountBeforeAdd + 1, projectsCountAfterAdd)
            Assert.AreEqual(employeeCountBeforeAdd + 1, employeeCountAfterAdd)
        End Using
    End Sub

    ''' <summary>
    ''' Add: DocumentMetadata.
    ''' Relationship: * - * / Project - DocumentMetadata.
    ''' Conditions: Navigation property DocumentMetadata in Project is not managed.
    ''' Result: Project and DocumentMetadata are saved in the database.
    ''' </summary>
    <TestMethod> _
    Public Sub Add_ManyToMany_IsManagedFalse_AddProjectAndDocument()
        Using dbContext As New EntitiesModel()
            Dim _project As New Project() With {.StartDate = Date.Now, .Title = "New Project", .Manager = "Peter The Manager"}

            Dim docMetadata As New DocumentMetadata() With {.Author = "Andre The Author", .Title = "Start up "}

            dbContext.Add(docMetadata)
            docMetadata.Projects.Add(_project)

            Dim projectsCountBeforeAdd As Integer = dbContext.Projects.Count()
            Dim docMetadataCountBeforeAdd As Integer = dbContext.DocumentMetadatum.Count()

            dbContext.SaveChanges()

            Dim projectsCountAfterAdd As Integer = dbContext.Projects.Count()
            Dim docMetadataCountAfterAdd As Integer = dbContext.DocumentMetadatum.Count()

            Assert.AreEqual(projectsCountBeforeAdd + 1, projectsCountAfterAdd)
            Assert.AreEqual(docMetadataCountBeforeAdd + 1, docMetadataCountAfterAdd)
        End Using
    End Sub

    ''' <summary>
    ''' Add: Project.
    ''' Relationship: * - * / Project - DocumentMetadata.
    ''' Conditions: Navigation property DocumentMetadata in Project is not managed.
    ''' Result: The collection projects in documentMetadata is empty even after adding a documentMetadata
    ''' to a project because IsManaged property is false.
    ''' </summary>
    <TestMethod> _
    Public Sub Add_ManyToMany_IsManagedFalse_NoSynchronization()
        Using dbContext As New EntitiesModel()
            Dim _project As New Project() With {.StartDate = Date.Now, .Title = "New Project", .Manager = "Peter The Manager"}

            Dim docMetadata As New DocumentMetadata() With {.Author = "Andre The Author", .Title = "Start up "}

            dbContext.Add(_project)
            _project.DocumentMetadatum.Add(docMetadata)

            Assert.AreEqual(0, docMetadata.Projects.Count)
        End Using
    End Sub

    ''' <summary>
    ''' Add: DocumentMetadata.
    ''' Relationship: * - * / Project - DocumentMetadata.
    ''' Conditions: Navigation property Employee in Project is not managed.
    ''' Result: Adding a new Project with Employee will add Project to the database.
    ''' </summary>
    <TestMethod> _
    Public Sub Add_ManyToMany_IsManagedFalse_NoAssociationInTheDatabase()
        Using dbContext As New EntitiesModel()
            Dim _project As New Project() With {.StartDate = Date.Now, .Title = "New Project", .Manager = "Peter The Manager"}

            Dim docMetadata As New DocumentMetadata() With {.Author = "Andre The Author", .Title = "Start up "}

            dbContext.Add(docMetadata)
            dbContext.Add(_project)
            docMetadata.Projects.Add(_project)

            dbContext.SaveChanges()

            Dim projectInDatabase As Project = dbContext.Projects.FirstOrDefault()
            Assert.IsNotNull(projectInDatabase, MessageHelper.NoRecordsInDatabase(GetType(Project)))

            Dim docMetaInDatabase As DocumentMetadata = dbContext.DocumentMetadatum.FirstOrDefault()
            Assert.IsNotNull(docMetaInDatabase, MessageHelper.NoRecordsInDatabase(GetType(DocumentMetadata)))

            Assert.AreEqual(0, projectInDatabase.DocumentMetadatum.Count())
            Assert.AreEqual(0, docMetaInDatabase.Projects.Count())
        End Using
    End Sub

    ''' <summary>
    ''' Add: Employee
    ''' Relationship: * - * / Project - DocumentMetadata
    ''' Conditions: Navigation property DocumentMetadata in Project is not managed
    ''' Result: If the DocumentMetadata is added to a context it will be saved in the database.
    ''' </summary>
    <TestMethod> _
    Public Sub Add_ManyToMany_IsManagedFalse_WithAssociationInTheDatabase()
        Using dbContext As New EntitiesModel()
            Dim _project As New Project() With {.StartDate = Date.Now, .Title = "New Project", .Manager = "Peter The Manager"}

            Dim docMetadata As New DocumentMetadata() With {.Author = "Andre The Author", .Title = "Start up "}

            dbContext.Add(docMetadata)
            dbContext.Add(_project)
            docMetadata.Projects.Add(_project)
            _project.DocumentMetadatum.Add(docMetadata)

            dbContext.SaveChanges()

            Dim projectInDatabase As Project = dbContext.Projects.FirstOrDefault()
            Assert.IsNotNull(projectInDatabase, MessageHelper.NoRecordsInDatabase(GetType(Project)))

            Dim docMetaInDatabase As DocumentMetadata = dbContext.DocumentMetadatum.FirstOrDefault()
            Assert.IsNotNull(docMetaInDatabase, MessageHelper.NoRecordsInDatabase(GetType(DocumentMetadata)))

            Assert.AreEqual(1, projectInDatabase.DocumentMetadatum.Count())
            Assert.AreEqual(1, docMetaInDatabase.Projects.Count())
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: Set the relational property in slave to the master record.
    ''' Result: The IsManaged property doesn't matter in this situation. The master and slave records are always synchronized.
    ''' </summary>
    <TestMethod> _
    Public Sub Add_OneToOne_IsManagedFalse_SetRelationInSlave_AddToContextFirst()
        Using dbContext As New EntitiesModel()
            Dim doc As New Document() With {.Checksum = 10}

            Dim docMeta As New DocumentMetadata() With {.Title = "New Document", .Created = Date.Now}

            dbContext.Add(docMeta)
            docMeta.Document = doc

            Assert.IsNotNull(doc.DocumentMetadatum)
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: Set the relational property in master to the slave record.
    ''' Result: The IsManaged property doesn't matter in this situation. The master and slave records are always synchronized.
    ''' </summary>
    <TestMethod> _
    Public Sub Add_OneToOne_IsManagedFalse_SetRelationInMaster_AddToContextFirst()
        Using dbContext As New EntitiesModel()
            Dim doc As New Document() With {.Checksum = 10}

            Dim docMeta As New DocumentMetadata() With {.Title = "New Document", .Created = Date.Now}

            dbContext.Add(doc)
            doc.DocumentMetadatum = docMeta

            Assert.IsNotNull(docMeta.Document)
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: Set the relational property in slave to the master record.
    ''' Result: Doesn't matter if the slave is added to a context before or after its master property is set.
    ''' </summary>
    <TestMethod> _
    Public Sub Add_OneToOne_IsManagedFalse_SetRelationInSlave_AddToContextLast()
        Using dbContext As New EntitiesModel()
            Dim doc As New Document() With {.Checksum = 10}

            Dim docMeta As New DocumentMetadata() With {.Title = "New Document", .Created = Date.Now}

            docMeta.Document = doc
            dbContext.Add(docMeta)

            Assert.IsNotNull(doc.DocumentMetadatum)
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: Set the relational property in master to the slave record.
    ''' Result: Doesn't matter if the master is added to a context before or after its slave property is set.
    ''' </summary>
    <TestMethod> _
    Public Sub Add_OneToOne_IsManagedFalse_SetRelationInParent_AddToContextLast()
        Using dbContext As New EntitiesModel()
            Dim doc As New Document() With {.Checksum = 10}

            Dim docMeta As New DocumentMetadata() With {.Title = "New Document", .Created = Date.Now}

            doc.DocumentMetadatum = docMeta
            dbContext.Add(doc)

            Assert.IsNotNull(docMeta.Document)
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: Add two entities in relation one to one.
    ''' Result: Two entities are added in the database.
    ''' </summary>
    <TestMethod> _
    Public Sub Add_OneToOne_Test_SaveInDatabase()
        Using dbContext As New EntitiesModel()
            Dim doc As New Document() With {.Checksum = 10}

            Dim docMeta As New DocumentMetadata() With {.Title = "New Document", .Created = Date.Now}

            dbContext.Add(docMeta)
            docMeta.Document = doc

            Dim docMetaCountBeforeAdd As Integer = dbContext.DocumentMetadatum.Count()
            Dim docCountBeforeAdd As Integer = dbContext.Documents.Count()

            dbContext.SaveChanges()

            Dim docMetaCountAfterAdd As Integer = dbContext.DocumentMetadatum.Count()
            Dim docCountAfterAdd As Integer = dbContext.Documents.Count()

            Assert.AreEqual(docMetaCountBeforeAdd + 1, docMetaCountAfterAdd)
            Assert.AreEqual(docCountBeforeAdd + 1, docCountAfterAdd)
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: Add an entity and a parent entity. Add only the parent entity to a context.
    ''' Result: The supervisor property of the employee is set appropriately.
    ''' </summary>
    <TestMethod> _
    Public Sub Add_SelfRef_AddParentToContext()
        Using dbContext As New EntitiesModel()
            Dim supervisor As New Employee() With {.FirstName = "Adam", .LastName = "Cage", .Department = "Human Resources"}

            Dim _employee As New Employee() With {.FirstName = "Nicolas", .LastName = "Rooney", .Department = "Human Resources"}

            dbContext.Add(supervisor)
            supervisor.Employees.Add(_employee)

            Assert.IsNotNull(_employee.Supervisor)
            Assert.AreSame(supervisor, _employee.Supervisor)
        End Using
    End Sub

    ''' <summary>
    ''' Scenario: Add an entity and a parent entity. Add only the child entity to a context.
    ''' Result: The Employees collection of the supervisor is populated appropriately.
    ''' </summary>
    <TestMethod> _
    Public Sub Add_SelfRef_AddChildToContext()
        Using dbContext As New EntitiesModel()
            Dim supervisor As New Employee() With {.FirstName = "Adam", .LastName = "Cage", .Department = "Human Resources"}

            Dim _employee As New Employee() With {.FirstName = "Nicolas", .LastName = "Rooney", .Department = "Human Resources"}

            dbContext.Add(_employee)
            _employee.Supervisor = supervisor

            Assert.AreEqual(1, supervisor.Employees.Count())

            Dim employeeOfSupervisor As Employee = supervisor.Employees.FirstOrDefault()
            Assert.IsNotNull(employeeOfSupervisor, MessageHelper.NoRecordsInDatabase(GetType(Employee)))

            Assert.AreSame(_employee, employeeOfSupervisor)
        End Using
    End Sub

End Class
