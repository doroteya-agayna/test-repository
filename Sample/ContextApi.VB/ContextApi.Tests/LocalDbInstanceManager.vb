Imports System
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Diagnostics
Imports System.IO
Imports System.Linq
Imports System.Reflection
Imports System.Text

Public NotInheritable Class LocalDbInstanceManager
    Private Sub New()
    End Sub
    Public Shared Sub BuildConnectionString()
        Dim location As String = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
        Dim fullDbFileName As String = Directory.GetFiles(location, "*.mdf").FirstOrDefault()

        If String.IsNullOrEmpty(fullDbFileName) Then
            Return
        End If

        Dim attachdbfilename As String = GetConnectionStringDetails("ProjectManagementConnection", "attachdbfilename")

    End Sub

    Private Shared Function GetConnectionStringDetails(ByVal connectionName As String, ByVal detailName As String) As String
        Dim connectionString As String = ConfigurationManager.ConnectionStrings(connectionName).ConnectionString.ToLower()
        Return connectionString.Split(";"c).First(Function(s) s.Contains(detailName))
    End Function
    'TODO: Check if LocalDB any version is available. 
    'If not stop the testrun
    'Creates and starts the TelerikDataAccess instance of LoclaDB
    Public Shared Sub CreateInstance()
        ExecuteCommand("create")
        ExecuteCommand("start")
    End Sub

    'Executes the LocalDB commands through the Command console
    Private Shared Sub ExecuteCommand(ByVal command As String)
        Dim dataSource As String = GetConnectionStringDetails("ProjectManagementConnection", "data source")

        Dim index As Integer = dataSource.IndexOf("\") + 1
        Dim instanceName As String = dataSource.Substring(index)

        command = "sqllocaldb " + command + " " + instanceName
        Dim info As New ProcessStartInfo("cmd", "/b /c " + command)
        Dim myProcess As New Process()

        myProcess.StartInfo = info
        myProcess.StartInfo.UseShellExecute = False
        myProcess.StartInfo.RedirectStandardOutput = True
        myProcess.StartInfo.RedirectStandardError = True
        Try
            myProcess.Start()
        Catch ex As Exception
            Dim exMessage As String = ex.Message
            'TODO: Display the message in the Output window
            'TODO: Stop the testrun
        Finally
            myProcess.Close()
        End Try
    End Sub

    'Just in case, although currently unused
    Public Shared Sub DeleteInstance()
        ExecuteCommand("stop")
        ExecuteCommand("delete")
    End Sub
End Class