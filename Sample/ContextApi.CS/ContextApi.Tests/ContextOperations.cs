using System;
using System.Linq;
using ContextApi.Model;

namespace ContextApi.Tests
{
    public static class ContextOperations
    {
        public static void ClearAllEntites()
        {
            using (EntitiesModel context = new EntitiesModel())
            {             
                var documents = context.Documents.ToList();
                context.Delete(documents);

                var newItems = context.NewItems.ToList();
                context.Delete(newItems);

                var bugs = context.Bugs.ToList();
                context.Delete(bugs);

                var dailyReports = context.DailyReports.ToList();
                context.Delete(dailyReports);

                var monthlyReports = context.MonthlyReports.ToList();
                context.Delete(monthlyReports);

                var taskAssignments = context.TaskAssignments.ToList();
                context.Delete(taskAssignments);

                var tasks = context.Tasks.ToList();
                context.Delete(tasks);

                var documentMetadatas = context.DocumentMetadatum.ToList();
                context.Delete(documentMetadatas);

                var projects = context.Projects.ToList();
                context.Delete(projects);

                var managers = context.Managers.ToList();
                foreach (Manager m in managers)
                {
                    context.Delete(m.Employees);
                    context.SaveChanges();
                }
                context.Delete(managers);
                context.SaveChanges();

                var employees = context.Employees.ToList();
                context.Delete(employees);

                context.SaveChanges();
            }
        }

        public static void AddEntities()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Project project = new Project()
                {
                    Title = "New Project",
                    StartDate = DateTime.Now,
                    Manager = "Adam Cane"
                };
                dbContext.Add(project);

                Task task = new Task()
                {
                    Title = "Start Project",
                    Status = "Not Done",
                    Priority = 1,
                    PercentCompleted = 0
                };
                task.Project = project;
                dbContext.Add(task);

                project.Tasks.Add(task);

                Task task2 = new Task()
                {
                    Title = "Plan Project",
                    Status = "Not Done",
                    Priority = 1,
                    PercentCompleted = 11
                };
                task2.Project = project;
                dbContext.Add(task2);

                project.Tasks.Add(task2);

                Task task3 = new Task()
                {
                    Title = "Celebrate Project",
                    Status = "Not Done",
                    Priority = 1,
                    PercentCompleted = 0
                };
                task3.Project = project;
                dbContext.Add(task3);

                project.Tasks.Add(task3);

                Manager manager = new Manager()
                {
                    FirstName = "Adam the manager",
                    LastName = "Johnson",
                    Title = "Manager",
                    Department = "CEOs"
                };

                Employee employee = new Employee()
                {
                    FirstName = "Nicolas",
                    LastName = "Crown",
                    Title = "Team Leader",
                    Department = "HR",
                };
                dbContext.Add(employee);
                manager.Employees.Add(employee);

                employee = new Employee()
                {
                    FirstName = "John",
                    LastName = "Smith",
                    Title = "Developer",
                    Department = "R&D",
                };
                dbContext.Add(employee);
                manager.Employees.Add(employee);

                dbContext.Add(manager);

                TaskAssignment taskAssignment = new TaskAssignment()
                {
                    Employee = employee,
                    Task = task,
                    WorkingHours = 12
                };
                dbContext.Add(taskAssignment);

                project.Employees.Add(employee);

                DocumentMetadata docMeta = new DocumentMetadata()
                {
                    Created = DateTime.Now,
                    Category = "Start Up",
                    Title = "First Steps",
                };
                dbContext.Add(docMeta);

                docMeta.Projects.Add(project);
                project.DocumentMetadatum.Add(docMeta);

                DailyReport dailyRep = new DailyReport()
                {
                    StartTime = DateTime.Now.AddHours(-5),
                    EndTime = DateTime.Now.AddHours(+3),
                    Title = "Today Report"
                };
                dbContext.Add(dailyRep);

                employee.DailyReports.Add(dailyRep);
                dailyRep.Employee = employee;

                MonthlyReport monthlyRep = new MonthlyReport()
                {
                    Month = 10,
                    Title = "November Report"
                };
                dbContext.Add(monthlyRep);

                employee.MonthlyReports.Add(monthlyRep);
                monthlyRep.Employee = employee;

                Bug bug = new Bug()
                {
                    Impact = 1,
                    PercentCompleted = 50,
                    Priority = 1,
                    Project = project,
                    Regression = true,
                    Status = "some status",
                    Title = "title"
                };

                dbContext.Add(bug);

                NewItem newItem = new NewItem()
                {
                    PercentCompleted = 20,
                    Priority = 3,
                    Project = project,
                    Status = "some newItem status",
                    Title = "newItem title",
                    StartTime = DateTime.Now,
                    ReadyFor = DateTime.Now.AddDays(1)
                };

                dbContext.Add(newItem);

                Document document = new Document()
                {
                    Data = new byte[1],
                    Checksum = 1,
                };

                DocumentMetadata metaData = new DocumentMetadata()
                {
                    DocumentMetadataId = document.DocumentId,
                    Title = "new title",
                };
                document.DocumentMetadatum = metaData;
                dbContext.Add(document);

                dbContext.SaveChanges();
            }
        }
    }
}