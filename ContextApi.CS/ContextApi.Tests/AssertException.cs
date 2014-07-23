using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ContextApi.Tests
{
    public static class AssertException
    {
        public static int Throws<TException>(Action action)
            where TException : Exception
        {
            if (action == null)
                throw new ArgumentNullException("action");

            try
            {
                action();
                string message =  "Should have thrown " + typeof(TException).Name;
                Assert.Fail(message);
            }
            catch (TException)
            {
                // OK
            }
            return 0;
        }
    }
}