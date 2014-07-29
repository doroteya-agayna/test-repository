Imports System
Imports System.Linq
Imports ContextApi.Model

Public NotInheritable Class ContextOperations
    Private Sub New()
    End Sub
    Public Shared Sub ClearAllEntites()
        Using context As New EntitiesModel()
            Dim documents = context.Documents.ToList()
            context.Delete(documents)

            Dim newItems = context.NewItems.ToList()
            context.Delete(newItems)

            Dim bugs = context.Bugs.ToList()
            context.Delete(bugs)

            Dim dailyReports = context.DailyReports.ToList()
            context.Delete(dailyReports)

            Dim monthlyReports = context.MonthlyReports.ToList()
            context.Delete(monthlyReports)

            Dim taskAssignments = context.TaskAssignments.ToList()
            context.Delete(taskAssignments)

            Dim tasks = context.Tasks.ToList()
            context.Delete(tasks)

            Dim documentMetadatas = context.DocumentMetadatum.ToList()
            context.Delete(documentMetadatas)

            Dim projects = context.Projects.ToList()
            context.Delete(projects)

            Dim managers = context.Managers.ToList()
            For Each m As Manager In managers
                context.Delete(m.Employees)
                context.SaveChanges()
            Next m
            context.Delete(managers)
            context.SaveChanges()

            Dim employees = context.Employees.ToList()
            context.Delete(employees)

            context.SaveChanges()
        End Using
    End Sub

    Public Shared Sub AddEntities()
        Using dbContext As New EntitiesModel()
            Dim _project As New Project() With {.Title = "New Project", .StartDate = Date.Now, .Manager = "Adam Cane"}
            dbContext.Add(_project)

            Dim _task As New Task() With {.Title = "Start Project", .Status = "Not Done", .Priority = 1, .PercentCompleted = 0}
            _task.Project = _project
            dbContext.Add(_task)

            _project.Tasks.Add(_task)

            Dim _manager As New Manager() With {.FirstName = "Adam the manager", .LastName = "Johnson", .Title = "Manager", .Department = "CEOs"}

            Dim _employee As New Employee() With {.FirstName = "Nicolas", .LastName = "Crown", .Title = "Team Leader", .Department = "HR"}
            dbContext.Add(_employee)
            _manager.Employees.Add(_employee)

            _employee = New Employee() With {.FirstName = "John", .LastName = "Smith", .Title = "Developer", .Department = "R&D"}
            dbContext.Add(_employee)
            _manager.Employees.Add(_employee)

            dbContext.Add(_manager)

            Dim _taskAssignment As New TaskAssignment() With {.Employee = _employee, .Task = _task, .WorkingHours = 12}
            dbContext.Add(_taskAssignment)

            _project.Employees.Add(_employee)

            Dim docMeta As New DocumentMetadata() With {.Created = Date.Now, .Category = "Start Up", .Title = "First Steps"}
            dbContext.Add(docMeta)

            docMeta.Projects.Add(_project)
            _project.DocumentMetadatum.Add(docMeta)

            Dim dailyRep As New DailyReport() With {.StartTime = Date.Now.AddHours(-5), .EndTime = Date.Now.AddHours(+3), .Title = "Today Report"}
            dbContext.Add(dailyRep)

            _employee.DailyReports.Add(dailyRep)
            dailyRep.Employee = _employee

            Dim monthlyRep As New MonthlyReport() With {.Month = 10, .Title = "November Report"}
            dbContext.Add(monthlyRep)

            _employee.MonthlyReports.Add(monthlyRep)
            monthlyRep.Employee = _employee

            Dim _bug As New Bug() With {.Impact = 1, .PercentCompleted = 50, .Priority = 1, .Project = _project, .Regression = True, .Status = "some status", .Title = "title"}

            dbContext.Add(_bug)

            Dim _newItem As New NewItem() With {.PercentCompleted = 20, .Priority = 3, .Project = _project, .Status = "some newItem status", .Title = "newItem title", .StartTime = Date.Now, .ReadyFor = Date.Now.AddDays(1)}

            dbContext.Add(_newItem)

            Dim _document As New Document() With {.Data = New Byte(0) {}, .Checksum = 1}

            Dim metaData As New DocumentMetadata() With {.DocumentMetadataId = _document.DocumentId, .Title = "new title"}
            _document.DocumentMetadatum = metaData
            dbContext.Add(_document)

            dbContext.SaveChanges()
        End Using
    End Sub
End Class


