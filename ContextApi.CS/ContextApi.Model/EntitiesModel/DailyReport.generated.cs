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
	public partial class DailyReport : Report
	{
		private int _dailyReportId;
		public virtual int DailyReportId
		{
			get
			{
				return this._dailyReportId;
			}
			set
			{
				this._dailyReportId = value;
			}
		}
		
		private DateTime? _startTime;
		public virtual DateTime? StartTime
		{
			get
			{
				return this._startTime;
			}
			set
			{
				this._startTime = value;
			}
		}
		
		private DateTime? _endTime;
		public virtual DateTime? EndTime
		{
			get
			{
				return this._endTime;
			}
			set
			{
				this._endTime = value;
			}
		}
		
		private Employee _employee;
		public virtual Employee Employee
		{
			get
			{
				return this._employee;
			}
			set
			{
				this._employee = value;
			}
		}
		
	}
}
#pragma warning restore 1591
