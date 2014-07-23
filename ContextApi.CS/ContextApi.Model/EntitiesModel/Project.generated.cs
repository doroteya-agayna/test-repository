#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the ClassGenerator.ttinclude code generation file.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Common;
using System.Collections.Generic;
using Telerik.OpenAccess;
using Telerik.OpenAccess.Metadata;
using Telerik.OpenAccess.Data.Common;
using Telerik.OpenAccess.Metadata.Fluent;
using Telerik.OpenAccess.Metadata.Fluent.Advanced;
using ContextApi.Model;

namespace ContextApi.Model	
{
	public partial class Project
	{
		private string _title;
		public virtual string Title
		{
			get
			{
				return this._title;
			}
			set
			{
				this._title = value;
			}
		}
		
		private int? _employeeId;
		public virtual int? EmployeeId
		{
			get
			{
				return this._employeeId;
			}
			set
			{
				this._employeeId = value;
			}
		}
		
		private int _projectId;
		public virtual int ProjectId
		{
			get
			{
				return this._projectId;
			}
			set
			{
				this._projectId = value;
			}
		}
		
		private string _manager;
		public virtual string Manager
		{
			get
			{
				return this._manager;
			}
			set
			{
				this._manager = value;
			}
		}
		
		private DateTime? _startDate;
		public virtual DateTime? StartDate
		{
			get
			{
				return this._startDate;
			}
			set
			{
				this._startDate = value;
			}
		}
		
		private DateTime? _endDate;
		public virtual DateTime? EndDate
		{
			get
			{
				return this._endDate;
			}
			set
			{
				this._endDate = value;
			}
		}
		
		private long? _budget;
		public virtual long? Budget
		{
			get
			{
				return this._budget;
			}
			set
			{
				this._budget = value;
			}
		}
		
		private int? _priority;
		public virtual int? Priority
		{
			get
			{
				return this._priority;
			}
			set
			{
				this._priority = value;
			}
		}
		
		private IList<DocumentMetadata> _documentMetadata = new List<DocumentMetadata>();
		public virtual IList<DocumentMetadata> DocumentMetadatum
		{
			get
			{
				return this._documentMetadata;
			}
		}
		
		private IList<Employee> _employees = new List<Employee>();
		public virtual IList<Employee> Employees
		{
			get
			{
				return this._employees;
			}
		}
		
		private IList<Task> _tasks = new List<Task>();
		public virtual IList<Task> Tasks
		{
			get
			{
				return this._tasks;
			}
		}
		
	}
}
#pragma warning restore 1591
