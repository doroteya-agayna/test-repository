using System;
using System.Linq;
using ContextApi.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.OpenAccess;
using Telerik.OpenAccess.FetchOptimization;

namespace ContextApi.Tests.TrackingChanges
{
    /// <summary>
    /// This class tests Adding, Added, Changing, Changed, Removing, Removed, Refreshing, Refreshed and ObjectConstructed events of the context.
    /// </summary>
    /// <seealso cref="http://documentation.telerik.com/openaccess-orm/feature-reference/api/context-api/feature-ref-api-context-api-tracking-context-changes"/>
    [TestClass]
    public class TrackingChangesTests : UnitTestsBase
    {
        private object addingTagValue = null;
        private object changingTagValue = null;
        private object deletingTagValue = null;
        private object refreshingTagValue = null;

        private int addingEventFireCount;
        private int addedEventFireCount;
        private int changingEventFireCount;
        private int changedEventFireCount;
        private int removingEventFireCount;
        private int removedEventFireCount;
        private int refreshingEventFireCount;
        private int refreshedEventFireCount;
        private int objectConstructedEventFireCount; 
                
        [TestInitialize()]
        public override void TestInitialize()
        {
            base.TestInitialize();

            this.addingTagValue = new object();
            this.changingTagValue = new object();
            this.deletingTagValue = new object();
            this.refreshingTagValue = new object();

            addingEventFireCount = 0;
            addedEventFireCount = 0;
            changingEventFireCount = 0;
            changedEventFireCount = 0;
            removingEventFireCount = 0;
            removedEventFireCount = 0;
            refreshingEventFireCount = 0;
            refreshedEventFireCount = 0;
            objectConstructedEventFireCount = 0;
        }

        [TestCleanup]
        public override void TestCleanup()
        {
            this.addingTagValue = null;
            this.changingTagValue = null;
            this.deletingTagValue = null;
            this.refreshingTagValue = null;

            base.TestCleanup();
        }
     
        /// <summary>
        /// Scenario: Track when an object is added to a context.
        /// Adding event: Occurs before a persistent object is added to the object context.
        /// Added event: Occurs after an object was marked as to be persisted by the object context.
        /// </summary>
        [TestMethod]
        public void Object_Is_Being_Added()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                dbContext.Events.Adding += Events_Adding;
                dbContext.Events.Added += Events_Added;

                try
                {
                    Bug bug = new Bug();
                    // If the object is not attached, the events Adding and Added will not be fired for that object
                    dbContext.Add(bug);

                    Assert.AreEqual(addingEventFireCount, 1);
                    Assert.AreEqual(addedEventFireCount, 1);
                }
                finally
                {
                    dbContext.Events.Adding -= Events_Adding;
                    dbContext.Events.Added -= Events_Added;
                }
            }
        }

        /// <summary>
        /// Scenario: Track when an object which is attached to a context is being changed.
        /// Changing event: Occurs before a field of a persistent object is changed by the application.
        /// Changed event: Occurs after a field of a persistent object has been changed by the application.
        /// </summary>
        [TestMethod]
        public void Object_Is_Being_Updated()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                dbContext.Events.Changing += Events_Changing;
                dbContext.Events.Changed += Events_Changed;

                try
                {
                    Bug bug = dbContext.Bugs.FirstOrDefault();
                    Assert.IsNotNull(bug, MessageHelper.NoRecordsInDatabase(typeof(Bug)));

                    // Touch the impact to load it to the memory so in the event Changing the value of the
                    // e.OldValue will be the old value of the property - otherwise null
                    // int? impact =  bug.Impact
                    
                    // Setting the impact will not load the old impact to the memory.
                    bug.Impact = 1;

                    Assert.AreEqual(1 ,changingEventFireCount);
                    Assert.AreEqual(1, changedEventFireCount);
                }
                finally
                {
                    dbContext.Events.Changing -= Events_Changing;
                    dbContext.Events.Changed -= Events_Changed;
                }
            }
        }

        /// <summary>
        /// Scenario: Track when an object which is attached to a context is being deleted.
        /// Removing: Occurs before a persistent object is removed from the object context.
        /// Removed:  Occurs after a persistent object was marked as to be removed from the object context.
        /// </summary>
        [TestMethod]
        public void Object_Is_Being_Deleted()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                dbContext.Events.Removing += Events_Removing;
                dbContext.Events.Removed += Events_Removed;

                try
                {
                    Bug bug = dbContext.Bugs.FirstOrDefault();
                    Assert.IsNotNull(bug, MessageHelper.NoRecordsInDatabase(typeof(Bug)));
                    dbContext.Delete(bug);

                    Assert.AreEqual(1, removingEventFireCount);
                    Assert.AreEqual(1, removedEventFireCount);
                }
                finally
                {
                    dbContext.Events.Removing -= Events_Removing;
                    dbContext.Events.Removed -= Events_Removed;
                }
            }
        }

        /// <summary>
        /// Scenario: Track when an object is constructed by a context.
        /// ObjectConstructed event: Occurs after an object has been constructed by Telerik Data Access' runtime.
        /// </summary>
        [TestMethod]
        public void Object_Is_Being_Constructed()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                dbContext.Events.ObjectConstructed += Events_ObjectConstructed;

                try
                {
                    // Here object gets constructed and ObjectConstructed event is fired
                    Bug bug = dbContext.Bugs.FirstOrDefault();
                    Assert.IsNotNull(bug, MessageHelper.NoRecordsInDatabase(typeof(Bug)));

                    Assert.AreEqual(1, objectConstructedEventFireCount);
                    Assert.IsNotNull(bug);
                }
                finally
                {
                    dbContext.Events.ObjectConstructed -= Events_ObjectConstructed;
                }
            }
        }

        /// <summary>
        /// Scenario: Track when an object is constructed by a context.
        /// ObjectConstructed event: Occurs after an object has been constructed by Telerik Data Access' runtime.
        /// </summary>
        [TestMethod]
        public void Object_Is_Being_Constructed_CreateDetachedCopy()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Project project = dbContext.Projects.FirstOrDefault();
                Assert.IsNotNull(project, MessageHelper.NoRecordsInDatabase(typeof(Project)));

                FetchStrategy strategy = new FetchStrategy();
                strategy.LoadWith<Project>(p => p.Tasks);

                try
                {
                    dbContext.Events.ObjectConstructed += TaskConstructed;

                    // Here five tasks get constructed and ObjectConstructed event is fired five times.
                    Project detachedProject = dbContext.CreateDetachedCopy(project, strategy);

                    Assert.AreEqual(5, objectConstructedEventFireCount);
                    Assert.IsNotNull(detachedProject);
                }
                finally
                {
                    dbContext.Events.ObjectConstructed -= TaskConstructed;
                }
            }
        }

        /// <summary>
        /// Scenario: Track when an object is constructed by a context.
        /// ObjectConstructed event: Occurs after an object has been constructed by Telerik Data Access runtime.
        /// </summary>
        [TestMethod]
        public void Object_Is_Being_Constructed_GetObjectByKey()
        { 
            // get bugId here from different context
            int bugId = 0;
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Bug bug = dbContext.Bugs.FirstOrDefault();
                Assert.IsNotNull(bug, MessageHelper.NoRecordsInDatabase(typeof(Bug)));

                bugId = bug.TaskId;
            }

            using (EntitiesModel dbContext = new EntitiesModel())
            { 
                dbContext.Events.ObjectConstructed += Events_ObjectConstructed;

                try
                {
                    // Here object gets constructed and ObjectConstructed event is fired
                    Bug bugObject = dbContext.GetObjectByKey(new ObjectKey(typeof(Bug).FullName, bugId)) as Bug;

                    Assert.AreEqual(1, objectConstructedEventFireCount);
                    Assert.IsNotNull(bugObject);
                }
                finally
                {
                    dbContext.Events.ObjectConstructed -= Events_ObjectConstructed;
                }
            }
        }

        /// <summary>
        /// Scenario: Track when an object is constructed by a context.
        /// ObjectConstructed event is not fired if the object that is to be constructed is already cached.
        /// </summary>
        [TestMethod]
        public void Object_Is_Being_Constructed_EventIsNotFired()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                Bug bug = dbContext.Bugs.FirstOrDefault();
                Assert.IsNotNull(bug, MessageHelper.NoRecordsInDatabase(typeof(Bug)));

                dbContext.Events.ObjectConstructed += ObjectConstructedIsNotFired;

                try
                {
                    // ObjectConstructed event is not fired because the bug is cached.
                    Bug bugObject = dbContext.GetObjectByKey(new ObjectKey(typeof(Bug).FullName, bug.TaskId)) as Bug;
                    bugObject = dbContext.CreateDetachedCopy(bugObject);
                    bugObject = dbContext.Bugs.FirstOrDefault();
                    Assert.IsNotNull(bugObject, MessageHelper.NoRecordsInDatabase(typeof(Bug)));

                    Assert.AreEqual(0, objectConstructedEventFireCount);
                }
                finally
                {
                    dbContext.Events.ObjectConstructed -= ObjectConstructedIsNotFired;
                }
            }
        }

        /// <summary>
        /// Scenario: Track when an object is refreshed by a context.
        /// Refreshing event: Occurs before a persistent object is refreshed from the database.
        /// Refreshed event:  Occurs after a persistent object was refreshed from the database.
        /// </summary>
        [TestMethod]
        public void Object_Is_Being_Refreshed()
        {
            using (EntitiesModel dbContext = new EntitiesModel())
            {
                dbContext.Events.Refreshing += Events_Refreshing;
                dbContext.Events.Refreshed += Events_Refreshed;

                try
                {
                    Bug bug = dbContext.Bugs.FirstOrDefault();
                    Assert.IsNotNull(bug, MessageHelper.NoRecordsInDatabase(typeof(Bug)));

                    dbContext.Refresh(RefreshMode.OverwriteChangesFromStore, bug);

                    Assert.AreEqual(1, refreshingEventFireCount);
                    Assert.AreEqual(1, refreshedEventFireCount);
                }
                finally
                {
                    dbContext.Events.Refreshed -= Events_Refreshed;
                    dbContext.Events.Refreshing -= Events_Refreshing;
                }
            }
        }

        private void ObjectConstructedIsNotFired(object sender, ObjectConstructedEventArgs e)
        {
            throw new ApplicationException("ObjectConstructed event should not be fired!");
        }
        
        private void TaskConstructed(object sender, ObjectConstructedEventArgs e)
        {
            Assert.IsTrue(e.PersistentObject is Task);
            objectConstructedEventFireCount++;
        }

        void Events_Refreshed(object sender, RefreshEventArgs e)
        {
            Assert.AreSame(refreshingTagValue, e.Tag);
            refreshedEventFireCount++;
        }

        void Events_Refreshing(object sender, RefreshEventArgs e)
        {
            e.Tag = refreshingTagValue;
            refreshingEventFireCount++;
        }

        void Events_ObjectConstructed(object sender, ObjectConstructedEventArgs e)
        {
            // The constructed object can be accessed via PersistentObject  property
            Assert.IsTrue(e.PersistentObject is Bug);
            objectConstructedEventFireCount++;
        }

        void Events_Removed(object sender, RemoveEventArgs e)
        {
            Assert.AreSame(deletingTagValue, e.Tag);
            removedEventFireCount++;
        }

        void Events_Removing(object sender, RemoveEventArgs e)
        {
            e.Tag = deletingTagValue;
            removingEventFireCount++;
        }

        void Events_Changed(object sender, ChangeEventArgs e)
        {
            Assert.AreSame(this.changingTagValue, e.Tag);
            changedEventFireCount++;
        }

        void Events_Changing(object sender, ChangeEventArgs e)
        {
            Assert.IsFalse(e.Cancel);
            Assert.AreEqual("Impact", e.PropertyName);
            Assert.IsFalse(e.Finished);
            Assert.AreEqual(e.NewValue, 1);
            Assert.IsNull(e.OldValue);
            Assert.IsNotNull(e.PersistentObject);
            Assert.IsNull(e.Tag);
            Assert.IsFalse(e.WasDirty);
            Assert.IsFalse(e.WasLoaded);

            e.Tag = this.changingTagValue;

            changingEventFireCount++;
        }

        void Events_Added(object sender, AddEventArgs e)
        {
            Assert.IsNotNull(e.Tag);
            Assert.IsFalse(e.Cancel);
            Assert.IsTrue(e.Finished);
            Assert.IsNotNull(e.PersistentObject);
            Assert.AreSame(this.addingTagValue, e.Tag);

            addedEventFireCount++;
        }

        void Events_Adding(object sender, AddEventArgs e)
        {
            Assert.IsNull(e.Tag);
            Assert.IsFalse(e.Cancel);
            Assert.IsFalse(e.Finished);
            Assert.IsNotNull(e.PersistentObject);

            // The tag property can be set to an arbitrary object. This object can be accessed in the Added event handler.
            e.Tag = this.addingTagValue;
            // If we set the Cancel property to true, the object will be not added to the context.
            //e.Cancel = true;

            addingEventFireCount++;
        }
    }
}