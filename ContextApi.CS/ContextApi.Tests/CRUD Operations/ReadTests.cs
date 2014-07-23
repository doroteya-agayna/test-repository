using System.IO;
using System.Linq;
using ContextApi.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.OpenAccess;

namespace ContextApi.Tests.CrudOperations
{
    [TestClass]
    public class ReadTests : UnitTestsBase
    {
        /// <summary>
        /// Scenario: Fetch only one object from the database.
        /// </summary>
        [TestMethod]
        public void ReadOne()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Employee singleEmployee = dbContext.Employees.FirstOrDefault();
                Assert.IsNotNull(singleEmployee, MessageHelper.NoRecordsInDatabase(typeof(Employee)));
            }
        }

        /// <summary>
        /// Scenario: Fetch exact number of objects from the database.
        /// </summary>
        [TestMethod]
        public void ReadMany()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                IQueryable<Employee> twoEmployees = dbContext.Employees.Take(2);
                Assert.IsNotNull(twoEmployees);
                Assert.AreEqual(2, twoEmployees.Count());
            }
        }

        /// <summary>
        /// Scenario: Fetching an object twice will result in two references pointing to the same object.
        /// </summary>
        [TestMethod]
        public void ReadObjectTwice()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            { 
                Employee firstEmployee = dbContext.Employees.FirstOrDefault();
                Assert.IsNotNull(firstEmployee, MessageHelper.NoRecordsInDatabase(typeof(Employee)));

                Employee secondEmployee = dbContext.Employees.FirstOrDefault();
                Assert.IsNotNull(secondEmployee, MessageHelper.NoRecordsInDatabase(typeof(Employee)));

                Assert.AreSame(firstEmployee, secondEmployee);
            }
        }

        /// <summary>
        /// Scenario: When object is fetched with GetObjectByKey() method that object is cached.
        /// </summary>
        [TestMethod]
        public void ReadObjectIsChached_GetObjectByKey()
        {
            int employeeId;
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                employeeId = dbContext.Employees.Select(e => e.EmployeeId).FirstOrDefault();
                Assert.AreNotEqual(0, employeeId, MessageHelper.NoRecordsInDatabase(typeof(Employee)));
            }
            using (EntitiesModel dbContext = new EntitiesModel())
            {            
                ObjectKey key = new ObjectKey(typeof(Employee).FullName, employeeId);

                Employee firstEmployee = dbContext.GetObjectByKey(key) as Employee;
                SetLog(dbContext);
                // No hit to the database here
                Employee secondEmployee = dbContext.GetObjectByKey(key) as Employee;

                Assert.AreSame(firstEmployee, secondEmployee);
                string logSqlString = GetLogString(dbContext);
                Assert.AreEqual(string.Empty, logSqlString);
            }
        }
                
        /// <summary>
        /// Scenario: Anonymous objects are never cached and will hit the database with every query.
        /// </summary>
        [TestMethod]
        public void ReadAnonymousType()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                SetLog(dbContext);
                var employees1 = dbContext.Employees.Select(s => new { s.EmployeeId, s.FirstName }).ToList();
                string query1 = GetLogString(dbContext);
                SetLog(dbContext);
                var employees2 = dbContext.Employees.Select(s => new { s.EmployeeId, s.FirstName }).ToList();
                string query2 = GetLogString(dbContext);

                Assert.IsNotNull(employees1);
                Assert.IsNotNull(employees2);

                Assert.AreEqual(query1, query2);
            }
        }
        
        private void SetLog(OpenAccessContext dbContext)
        {
            StringWriter writer = new StringWriter();
            dbContext.Log = writer;
        }

        private string GetLogString(OpenAccessContext dbContext)
        {
            StringWriter writer = dbContext.Log as StringWriter;
            if (writer != null)
            {
                string logString = writer.GetStringBuilder().ToString();
                return logString;
            }
            return string.Empty;
        }
    }
}