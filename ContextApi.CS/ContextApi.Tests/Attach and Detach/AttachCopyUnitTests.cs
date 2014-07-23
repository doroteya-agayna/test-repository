using System.Linq;
using ContextApi.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.OpenAccess;

namespace ContextApi.Tests.Attach_and_Detach
{
    [TestClass]
    public class AttachCopyUnitTests : UnitTestsBase
    {
        private EntitiesModel originalContext;
        private EntitiesModel anotherContext;

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();

            this.originalContext = new EntitiesModel();
            this.anotherContext = new EntitiesModel();
        }

        [TestCleanup]
        public override void TestCleanup()
        {
            base.TestCleanup();

            if (this.originalContext != null)
            {
                this.originalContext.Dispose();
            }

            if (this.anotherContext != null)
            {
                this.anotherContext.Dispose();
            }
        }

        /// <summary>
        /// Scenario: Create a new instance of a persistent object and attach it to a context.
        /// Outcome: The attached object will have ObjectState New and cause the context to which is attached to have changes.
        /// </summary>
        [TestMethod]
        public void AttachCopy_AttachNewInstance()
        {
            Employee newEmployee = new Employee()
            {
                EmployeeId = 1337,
                FirstName = "Mad",
                LastName = "Jack"
            };

            Employee attachedEmployee = this.anotherContext.AttachCopy<Employee>(newEmployee);
            bool hasChangesAfterAttach = this.anotherContext.HasChanges;
            ObjectState attachedEmployeeState = this.GetObjectState(attachedEmployee);

            this.anotherContext.SaveChanges();

            Assert.AreEqual(ObjectState.New, attachedEmployeeState);
            Assert.IsTrue(hasChangesAfterAttach);
        }

        /// <summary>
        /// Scenario: Detach an object from its context and attach it to another context.
        /// Outcome: The managing context of the attached object will be the one to which it was attached and different than the one of the
        /// original object.
        /// </summary>
        [TestMethod]
        public void AttachCopy_AttachToAnotherContext()
        {
            Employee retrievedEmployee = this.originalContext.Employees.FirstOrDefault(emp => emp.Title == "Developer");

            Employee detachedEmployee = this.originalContext.CreateDetachedCopy<Employee>(retrievedEmployee);

            Employee attachedEmployee = this.anotherContext.AttachCopy<Employee>(detachedEmployee);
            OpenAccessContextBase managingContext = OpenAccessContext.GetContext(attachedEmployee);

            Assert.AreEqual(this.anotherContext, managingContext);
            Assert.AreNotEqual(this.originalContext, managingContext);
        }

        /// <summary>
        /// Scenario: Detach an object from its context and attach it to the same context.
        /// Condition: There are no changes made to neither the original nor the detached objects.
        /// Outcome: The attached object is the same as the original one and different from the detached. 
        /// It will have ObjectState Clean and will not cause changes to the context to which is attached.
        /// </summary>
        [TestMethod]
        public void AttachCopy_DetachCleanObject_AttachToTheSameContext()
        {
            Employee retrievedEmployee = this.originalContext.Employees.FirstOrDefault(emp => emp.Title == "Developer");

            Employee detachedEmployee = this.originalContext.CreateDetachedCopy<Employee>(retrievedEmployee);

            Employee attachedEmployee = this.originalContext.AttachCopy<Employee>(detachedEmployee);
            bool hasChangesAfterAttach = this.originalContext.HasChanges;
            ObjectState attachedEmployeeState = this.GetObjectState(attachedEmployee);

            Assert.AreSame(retrievedEmployee, attachedEmployee);
            Assert.AreNotSame(detachedEmployee, attachedEmployee);
            
            Assert.AreEqual(ObjectState.Clean, attachedEmployeeState);
            Assert.IsFalse(hasChangesAfterAttach);
        }

        /// <summary>
        /// Scenario: Detach an object from its context and attach it to another context.
        /// Condition: There are no changes to neither the original nor the detached objects.
        /// Outcome: The attached object is different from the original and detached objects. 
        /// It will have ObjectState Clean and will not cause changes to the context to which is attached.
        /// </summary>
        [TestMethod]
        public void AttachCopy_DetachCleanObject_AttachToAnotherContext()
        {
            Employee retrievedEmployee = this.originalContext.Employees.FirstOrDefault(emp => emp.Title == "Developer");

            Employee detachedEmployee = this.originalContext.CreateDetachedCopy<Employee>(retrievedEmployee);

            Employee attachedEmployee = this.anotherContext.AttachCopy<Employee>(detachedEmployee);
            bool hasChangesAfterAttach = this.anotherContext.HasChanges;
            ObjectState attachedEmployeeState = this.GetObjectState(attachedEmployee);

            Assert.AreNotSame(retrievedEmployee, attachedEmployee);
            Assert.AreNotSame(detachedEmployee, attachedEmployee);

            Assert.AreEqual(ObjectState.Clean, attachedEmployeeState);
            Assert.IsFalse(hasChangesAfterAttach);
        }

        /// <summary>
        /// Scenario: Detach an object from its context and attach it to the same context.
        /// Condition: Modify the original object before detaching it.
        /// Outcome: The attached object will have ObjectState Dirty and cause changes to the context to which is attached.
        /// </summary>
        [TestMethod]
        public void AttachCopy_ModifyOriginalObjectDetach_AttachToTheSameContext()
        {
            Employee retrievedEmployee = this.originalContext.Employees.FirstOrDefault(emp => emp.Title == "Developer");
            string newName = "Gary";
            retrievedEmployee.FirstName = newName;

            Employee detachedEmployee = this.originalContext.CreateDetachedCopy<Employee>(retrievedEmployee);
            this.originalContext.ClearChanges();
            bool hasChangesBeforeAttach = this.originalContext.HasChanges;

            Employee attachedEmployee = this.originalContext.AttachCopy<Employee>(detachedEmployee);
            bool hasChangesAfterAttach = this.originalContext.HasChanges;
            ObjectState attachedEmployeeState = this.GetObjectState(attachedEmployee);

            Assert.AreEqual(newName, attachedEmployee.FirstName);
            Assert.AreEqual(ObjectState.Dirty, attachedEmployeeState);
            Assert.IsFalse(hasChangesBeforeAttach);
            Assert.IsTrue(hasChangesAfterAttach);
        }

        /// <summary>
        /// Scenario: Detach an object from its context and attach it to another context.
        /// Condition: Modify the original object before detaching it.
        /// Outcome: The attached object will have ObjectState Dirty and cause changes to the context to which is attached.
        /// </summary>
        [TestMethod]
        public void AttachCopy_ModifyOriginalObjectDetach_AndAttachToAnotherContext()
        {
            Employee retrievedEmployee = this.originalContext.Employees.FirstOrDefault(emp => emp.Title == "Developer");
            string newName = "Gary";
            retrievedEmployee.FirstName = newName;

            Employee detachedEmployee = this.originalContext.CreateDetachedCopy<Employee>(retrievedEmployee);

            Employee attachedEmployee = this.anotherContext.AttachCopy<Employee>(detachedEmployee);
            bool hasChangesAfterAttach = this.anotherContext.HasChanges;
            ObjectState attachedEmployeeState = this.GetObjectState(attachedEmployee);

            Assert.AreEqual(newName, attachedEmployee.FirstName);
            Assert.AreEqual(ObjectState.Dirty, attachedEmployeeState);
            Assert.IsTrue(hasChangesAfterAttach);
        }

        /// <summary>
        /// Scenario: Detach an object from its context and attach it to the same context.
        /// Condition: Modify the detached object before attaching it.
        /// Outcome: The attached object will have ObjectState Dirty and cause changes to the context to which is attached.
        /// </summary>
        [TestMethod]
        public void AttachCopy_DetachModifyDetachedObject_AttachToTheSameContext()
        {
            Employee retrievedEmployee = this.originalContext.Employees.FirstOrDefault(emp => emp.Title == "Developer");

            Employee detachedEmployee = this.originalContext.CreateDetachedCopy<Employee>(retrievedEmployee);
            string newName = "Gary";
            detachedEmployee.FirstName = newName;

            Employee attachedEmployee = this.originalContext.AttachCopy<Employee>(detachedEmployee);
            bool hasChangesAfterAttach = this.originalContext.HasChanges;
            ObjectState attachedEmployeeState = this.GetObjectState(attachedEmployee);

            Assert.AreEqual(newName, attachedEmployee.FirstName);
            Assert.AreEqual(ObjectState.Dirty, attachedEmployeeState);
            Assert.IsTrue(hasChangesAfterAttach);
        }

        /// <summary>
        /// Scenario: Detach an object from its context and attach it to another context.
        /// Condition: Modify the detached object before attaching it.
        /// Outcome: The attached object will have ObjectState Dirty and cause changes to the context to which is attached.
        /// </summary>
        [TestMethod]
        public void AttachCopy_Detach_ModifyDetachedObject_AttachToAnotherContext()
        {
            Employee retrievedEmployee = this.originalContext.Employees.FirstOrDefault(emp => emp.Title == "Developer");

            Employee detachedEmployee = this.originalContext.CreateDetachedCopy<Employee>(retrievedEmployee);
            string newName = "Gary";
            detachedEmployee.FirstName = newName;

            Employee attachedEmployee = this.anotherContext.AttachCopy<Employee>(detachedEmployee);
            bool hasChangesAfterAttach = this.anotherContext.HasChanges;
            ObjectState attachedEmployeeState = this.GetObjectState(attachedEmployee);

            Assert.AreEqual(newName, attachedEmployee.FirstName);
            Assert.AreEqual(ObjectState.Dirty, attachedEmployeeState);
            Assert.IsTrue(hasChangesAfterAttach);
        }

        /// <summary>
        /// Scenario: Detach an object from its original context and attach it to the same context.
        /// Condition: Modify the original object after detaching it.
        /// Outcome: The attached object will have ObjectState Dirty and will have the same value of the modified property as the original object.
        /// </summary>
        [TestMethod]
        public void AttachCopy_Detach_ModifyOriginalObject_AttachToTheSameContext()
        {
            Employee retrievedEmployee = this.originalContext.Employees.FirstOrDefault(emp => emp.Title == "Developer");

            Employee detachedEmployee = this.originalContext.CreateDetachedCopy<Employee>(retrievedEmployee);
            
            string newName = "Gary";
            retrievedEmployee.FirstName = newName;

            Employee attachedEmployee = this.originalContext.AttachCopy<Employee>(detachedEmployee);
            ObjectState attachedEmployeeState = this.GetObjectState(attachedEmployee);
            OpenAccessContextBase managingContext = OpenAccessContext.GetContext(attachedEmployee);

            Assert.AreEqual(newName, attachedEmployee.FirstName);
            Assert.AreEqual(ObjectState.Dirty, attachedEmployeeState);
        }

        /// <summary>
        /// Scenario: Detach an object from its original context and attach it to another context.
        /// Condition: Modify the original object after detaching it.
        /// Outcome: The attached object will have ObjectState Clean and the value of its respective property which was modified in the
        /// original object will be unchanged. The context to which the object is attached will not have changes.
        /// </summary>
        [TestMethod]
        public void AttachCopy_Detach_ModifyOriginalObject_AttachToAnotherContext()
        {
            Employee retrievedEmployee = this.originalContext.Employees.FirstOrDefault(emp => emp.Title == "Developer");

            Employee detachedEmployee = this.originalContext.CreateDetachedCopy<Employee>(retrievedEmployee);

            string newName = "Gary";
            retrievedEmployee.FirstName = newName;

            Employee attachedEmployee = this.anotherContext.AttachCopy<Employee>(detachedEmployee);
            bool hasChangesAfterAttach = this.anotherContext.HasChanges;
            ObjectState attachedEmployeeState = this.GetObjectState(attachedEmployee);
            OpenAccessContextBase managingContext = OpenAccessContext.GetContext(attachedEmployee);

            Assert.AreNotEqual(newName, attachedEmployee.FirstName);
            Assert.AreEqual(ObjectState.Clean, attachedEmployeeState);
            Assert.IsFalse(hasChangesAfterAttach);
        }

        /// <summary>
        /// Scenario: Detach an object from its original context and attach it to the same context.
        /// Condition: Modify the original after its detached copy has been attached.
        /// Outcome: The attached object will have ObjectState Dirty and will have the same value of the modified property as the original object.
        /// </summary>
        [TestMethod]
        public void AttachCopy_DetachObject_AttachToTheSameContext_ModifyOriginalObject()
        {
            Employee retrievedEmployee = this.originalContext.Employees.FirstOrDefault(emp => emp.Title == "Developer");

            Employee detachedEmployee = this.originalContext.CreateDetachedCopy<Employee>(retrievedEmployee);

            Employee attachedEmployee = this.originalContext.AttachCopy<Employee>(detachedEmployee);

            string newName = "Gary";
            retrievedEmployee.FirstName = newName;

            ObjectState attachedEmployeeState = this.GetObjectState(attachedEmployee);
            OpenAccessContextBase managingContext = OpenAccessContext.GetContext(attachedEmployee);

            Assert.AreEqual(newName, attachedEmployee.FirstName);
            Assert.AreEqual(ObjectState.Dirty, attachedEmployeeState);
        }

        /// <summary>
        /// Scenario: Detach an object from its original context and attach it to another context.
        /// Condition: Modify the original object after detaching it.
        /// Outcome: The attached object will have ObjectState Clean and the value of its respective property which was modified in the
        /// original object will be unchanged. The context to which the object is attached will not have changes.
        /// </summary>
        [TestMethod]
        public void AttachCopy_DetachObject_AttachToAnotherContext_ModifyOriginalObject()
        {
            Employee retrievedEmployee = this.originalContext.Employees.FirstOrDefault(emp => emp.Title == "Developer");

            Employee detachedEmployee = this.originalContext.CreateDetachedCopy<Employee>(retrievedEmployee);

            Employee attachedEmployee = this.anotherContext.AttachCopy<Employee>(detachedEmployee);

            string newName = "Gary";
            retrievedEmployee.FirstName = newName;

            bool hasChangesAfterAttach = this.anotherContext.HasChanges;
            ObjectState attachedEmployeeState = this.GetObjectState(attachedEmployee);
            OpenAccessContextBase managingContext = OpenAccessContext.GetContext(attachedEmployee);

            Assert.AreNotEqual(newName, attachedEmployee.FirstName);
            Assert.AreEqual(ObjectState.Clean, attachedEmployeeState);
            Assert.IsFalse(hasChangesAfterAttach);
        }

        /// <summary>
        /// Scenario: Detach an object from its original context and attach it to the same context.
        /// Condition: Modify the detached object after it has been attached to a context.
        /// Outcome: The attached object will have ObjectState Clean and the value of its respective property which has 
        /// been modified in the detached object will not be changed.
        /// </summary>
        [TestMethod]
        public void AttachCopy_DetachObject_AttachToTheSameContext_ModifyDetachedObject()
        {
            Employee retrievedEmployee = this.originalContext.Employees.FirstOrDefault(emp => emp.Title == "Developer");

            Employee detachedEmployee = this.originalContext.CreateDetachedCopy<Employee>(retrievedEmployee);

            Employee attachedEmployee = this.originalContext.AttachCopy<Employee>(detachedEmployee);

            string newName = "Gary";
            detachedEmployee.FirstName = newName;

            ObjectState attachedEmployeeState = this.GetObjectState(attachedEmployee);
            OpenAccessContextBase managingContext = OpenAccessContext.GetContext(attachedEmployee);

            Assert.AreNotEqual(newName, attachedEmployee.FirstName);
            Assert.AreEqual(ObjectState.Clean, attachedEmployeeState);
        }

        /// <summary>
        /// Scenario: Detach an object from its original context and attach it to another context.
        /// Condition: Modify the detached object after it has been attached to a context.
        /// Outcome: The attached object will have ObjectState Clean and the value of its respective property which was modified in the
        /// original object will be unchanged. The context to which the object is attached will not have changes.
        /// </summary>
        [TestMethod]
        public void AttachCopy_DetachObject_AttachToAnotherContext_ModifyDetachedObject()
        {
            Employee retrievedEmployee = this.originalContext.Employees.FirstOrDefault(emp => emp.Title == "Developer");

            Employee detachedEmployee = this.originalContext.CreateDetachedCopy<Employee>(retrievedEmployee);

            Employee attachedEmployee = this.anotherContext.AttachCopy<Employee>(detachedEmployee);

            string newName = "Gary";
            detachedEmployee.FirstName = newName;

            bool hasChangesAfterAttach = this.anotherContext.HasChanges;
            ObjectState attachedEmployeeState = this.GetObjectState(attachedEmployee);
            OpenAccessContextBase managingContext = OpenAccessContext.GetContext(attachedEmployee);

            Assert.AreNotEqual(newName, attachedEmployee.FirstName);
            Assert.AreEqual(ObjectState.Clean, attachedEmployeeState);
            Assert.IsFalse(hasChangesAfterAttach);
        }

        /// <summary>
        /// Scenario: Attach a new object to a context.
        /// Condition: The new object has a reference to a related object.
        /// Outcome: The related object will also be attached to the context.
        /// </summary>
        [TestMethod]
        public void AttachCopy_AttachGraphOfRelatedObjects()
        {
            Employee newEmployee = new Employee()
            {
                EmployeeId = 1337,
                FirstName = "Mad",
                LastName = "Jack"
            };

            Employee newManager = new Employee()
            {
                EmployeeId = 1338,
                FirstName = "Sane",
                LastName = "Jack",
                Title = "Grand Master Manager"
            };
            newEmployee.Supervisor = newManager;

            Employee attachedEmployee = this.originalContext.AttachCopy<Employee>(newEmployee);
            bool hasChangesAfterAttach = this.originalContext.HasChanges;
            ObjectState attachedEmployeeState = this.GetObjectState(attachedEmployee);

            Assert.AreEqual(newManager.EmployeeId, attachedEmployee.Supervisor.EmployeeId);
        }

        /// <summary>
        /// Scenario: Detach an object from its context an attach it to another context.
        /// Condition: To the detached object add a reference to a related object.
        /// Outcome: The related object will also be attached to the context.
        /// </summary>
        [TestMethod]
        public void AttachCopy_DetachObject_AttachGraphOfRelatedObjects()
        {
            Employee retrievedEmployee = this.originalContext.Employees.FirstOrDefault(emp => emp.Title == "Developer");

            Employee detachedEmployee = this.originalContext.CreateDetachedCopy<Employee>(retrievedEmployee);

            Employee newManager = new Employee()
            {
                EmployeeId = 1338,
                FirstName = "Sane",
                LastName = "Jack",
                Title = "Grand Master Manager"
            };
            detachedEmployee.Supervisor = newManager;

            Employee attachedEmployee = this.anotherContext.AttachCopy<Employee>(detachedEmployee);
            bool hasChangesAfterAttach = this.anotherContext.HasChanges;
            ObjectState attachedEmployeeState = this.GetObjectState(attachedEmployee);

            Assert.AreEqual(newManager.EmployeeId, attachedEmployee.Supervisor.EmployeeId);
        }
    }
}
