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
	public partial class Document
	{
		private int _documentId;
		public virtual int DocumentId
		{
			get
			{
				return this._documentId;
			}
			set
			{
				this._documentId = value;
			}
		}
		
		private byte[] _data;
		public virtual byte[] Data
		{
			get
			{
				return this._data;
			}
			set
			{
				this._data = value;
			}
		}
		
		private int _checksum;
		public virtual int Checksum
		{
			get
			{
				return this._checksum;
			}
			set
			{
				this._checksum = value;
			}
		}
		
		private int _concurencyVersion;
		public virtual int ConcurencyVersion
		{
			get
			{
				return this._concurencyVersion;
			}
			set
			{
				this._concurencyVersion = value;
			}
		}
		
		private DocumentMetadata _documentMetadatum;
		public virtual DocumentMetadata DocumentMetadatum
		{
			get
			{
				return this._documentMetadatum;
			}
			set
			{
				this._documentMetadatum = value;
			}
		}
		
	}
}
#pragma warning restore 1591
