using System;
using System.Linq;


namespace ContextApi.Tests
{
    public static class MessageHelper
    {
        public static string NoRecordsInDatabase(Type typeOfEntity)
        {
            string message = string.Format("No records of type {0} in the database!", typeOfEntity.Name);
            return message;
        }
    }
}
