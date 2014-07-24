using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ContextApi.Tests
{
    public static class LocalDbInstanceManager
    {
        //TODO: Check if LocalDB any version is available. 
        //If not stop the testrun
        //Creates and starts the TelerikDataAccess instance of LoclaDB
        public static void CreateInstance()
        {
            ExecuteCommand("create");
            ExecuteCommand("start");
        }

        //Executes the LocalDB commands through the Command console
        private static void ExecuteCommand(string command)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ProjectManagementConnection"]
                                                          .ConnectionString
                                                          .ToLower();
            string dataSource = connectionString.Split(';').First(s => s.Contains("data source"));
            
            int index = dataSource.IndexOf(@"\") + 1;
            string instanceName = dataSource.Substring(index);

            command = "sqllocaldb " + command + " " + instanceName;
            ProcessStartInfo info = new ProcessStartInfo("cmd", "/c " + command);
            Process process = new Process();

            process.StartInfo = info;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            try
            {
                process.Start();
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                //TODO: Display the message in the Output window
                //TODO: Stop the testrun
            }
            finally
            {
                process.Close();
            }
        }

        //Just in case, although currently unused
        public static void DeleteInstance()
        {
            ExecuteCommand("stop");
            ExecuteCommand("delete");
        }
    }
}
