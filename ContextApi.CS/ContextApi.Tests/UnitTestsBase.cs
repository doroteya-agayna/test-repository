using ContextApi.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.OpenAccess;

namespace ContextApi.Tests
{
    [TestClass]
    public abstract class UnitTestsBase
    {
        [TestInitialize]
        public virtual void TestInitialize()
        {
            LocalDbInstanceManager.CreateInstance();
            ContextOperations.ClearAllEntites();
            ContextOperations.AddEntities();
        }

        [TestCleanup]
        public virtual void TestCleanup()
        {
            ContextOperations.ClearAllEntites();
        }

        //Get the object state of a persistent object.
        protected ObjectState GetObjectState(object entity)
        {
            return OpenAccessContext.PersistenceState.GetState(entity);
        }
    }
}
