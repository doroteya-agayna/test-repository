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
	Public Partial Class MonthlyReport
		Inherits Report
		Private _monthlyReportId As Integer 
		Public Overridable Property MonthlyReportId As Integer
			Get
		        Return Me._monthlyReportId
			End Get
			Set(ByVal value As Integer)
		        Me._monthlyReportId = value
			End Set
		End Property
		
		Private _month As Integer 
		Public Overridable Property Month As Integer
			Get
		        Return Me._month
			End Get
			Set(ByVal value As Integer)
		        Me._month = value
			End Set
		End Property
		
		Private _employee As Employee 
		Public Overridable Property Employee As Employee
			Get
		        Return Me._employee
			End Get
			Set(ByVal value As Employee)
		        Me._employee = value
			End Set
		End Property
		
	End Class
End Namespace
