'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by the ClassGenerator.ttinclude code generation file.
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------
Imports System
Imports System.Data
Imports System.Linq
Imports System.Linq.Expressions
Imports System.Data.Common
Imports System.Collections.Generic
Imports Telerik.OpenAccess
Imports Telerik.OpenAccess.Metadata
Imports Telerik.OpenAccess.Data.Common
Imports Telerik.OpenAccess.Metadata.Fluent
Imports Telerik.OpenAccess.Metadata.Fluent.Advanced
Imports ContextApi.Model

Namespace ContextApi.Model	
	Public Partial Class Project
		Private _title As String 
		Public Overridable Property Title As String
			Get
		        Return Me._title
			End Get
			Set(ByVal value As String)
		        Me._title = value
			End Set
		End Property
		
		Private _employeeId As Integer? 
		Public Overridable Property EmployeeId As Integer?
			Get
		        Return Me._employeeId
			End Get
			Set(ByVal value As Integer?)
		        Me._employeeId = value
			End Set
		End Property
		
		Private _projectId As Integer 
		Public Overridable Property ProjectId As Integer
			Get
		        Return Me._projectId
			End Get
			Set(ByVal value As Integer)
		        Me._projectId = value
			End Set
		End Property
		
		Private _manager As String 
		Public Overridable Property Manager As String
			Get
		        Return Me._manager
			End Get
			Set(ByVal value As String)
		        Me._manager = value
			End Set
		End Property
		
		Private _startDate As Date? 
		Public Overridable Property StartDate As Date?
			Get
		        Return Me._startDate
			End Get
			Set(ByVal value As Date?)
		        Me._startDate = value
			End Set
		End Property
		
		Private _endDate As Date? 
		Public Overridable Property EndDate As Date?
			Get
		        Return Me._endDate
			End Get
			Set(ByVal value As Date?)
		        Me._endDate = value
			End Set
		End Property
		
		Private _budget As Long? 
		Public Overridable Property Budget As Long?
			Get
		        Return Me._budget
			End Get
			Set(ByVal value As Long?)
		        Me._budget = value
			End Set
		End Property
		
		Private _priority As Integer? 
		Public Overridable Property Priority As Integer?
			Get
		        Return Me._priority
			End Get
			Set(ByVal value As Integer?)
		        Me._priority = value
			End Set
		End Property
		
		Private _documentMetadata As IList(Of DocumentMetadata)  = new List(Of DocumentMetadata)
		Public Overridable ReadOnly Property DocumentMetadatum As IList(Of DocumentMetadata)
			Get
		        Return Me._documentMetadata
			End Get
		End Property
		
		Private _employees As IList(Of Employee)  = new List(Of Employee)
		Public Overridable ReadOnly Property Employees As IList(Of Employee)
			Get
		        Return Me._employees
			End Get
		End Property
		
		Private _tasks As IList(Of Task)  = new List(Of Task)
		Public Overridable ReadOnly Property Tasks As IList(Of Task)
			Get
		        Return Me._tasks
			End Get
		End Property
		
	End Class
End Namespace
