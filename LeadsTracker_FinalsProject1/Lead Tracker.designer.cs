﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LeadsTracker_FinalsProject1
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="Lead Tracker")]
	public partial class DataClasses1DataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    #endregion
		
		public DataClasses1DataContext() : 
				base(global::LeadsTracker_FinalsProject1.Properties.Settings.Default.Lead_TrackerConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public DataClasses1DataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public DataClasses1DataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public DataClasses1DataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public DataClasses1DataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<Document> Documents
		{
			get
			{
				return this.GetTable<Document>();
			}
		}
		
		public System.Data.Linq.Table<Lead> Leads
		{
			get
			{
				return this.GetTable<Lead>();
			}
		}
		
		public System.Data.Linq.Table<Staff> Staffs
		{
			get
			{
				return this.GetTable<Staff>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Documents")]
	public partial class Document
	{
		
		private string _Documents_ID;
		
		private string _Picture;
		
		private string _Birth_Certificate;
		
		private string _Good_Moral;
		
		private string _TOR;
		
		private string _Medical_Clearance;
		
		private string _Report_Card;
		
		public Document()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Documents_ID", DbType="VarChar(60)")]
		public string Documents_ID
		{
			get
			{
				return this._Documents_ID;
			}
			set
			{
				if ((this._Documents_ID != value))
				{
					this._Documents_ID = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Picture", DbType="VarChar(255)")]
		public string Picture
		{
			get
			{
				return this._Picture;
			}
			set
			{
				if ((this._Picture != value))
				{
					this._Picture = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Birth_Certificate", DbType="VarChar(255)")]
		public string Birth_Certificate
		{
			get
			{
				return this._Birth_Certificate;
			}
			set
			{
				if ((this._Birth_Certificate != value))
				{
					this._Birth_Certificate = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Good_Moral", DbType="VarChar(255)")]
		public string Good_Moral
		{
			get
			{
				return this._Good_Moral;
			}
			set
			{
				if ((this._Good_Moral != value))
				{
					this._Good_Moral = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TOR", DbType="VarChar(255)")]
		public string TOR
		{
			get
			{
				return this._TOR;
			}
			set
			{
				if ((this._TOR != value))
				{
					this._TOR = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Medical_Clearance", DbType="VarChar(255)")]
		public string Medical_Clearance
		{
			get
			{
				return this._Medical_Clearance;
			}
			set
			{
				if ((this._Medical_Clearance != value))
				{
					this._Medical_Clearance = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Report_Card", DbType="VarChar(255)")]
		public string Report_Card
		{
			get
			{
				return this._Report_Card;
			}
			set
			{
				if ((this._Report_Card != value))
				{
					this._Report_Card = value;
				}
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Leads")]
	public partial class Lead
	{
		
		private string _Lead_ID;
		
		private System.Nullable<System.DateTime> _Date;
		
		private string _Lead_Source;
		
		private string _Lead_Name;
		
		private string _Lead_Email;
		
		private string _Phone_Number;
		
		private string _Notes;
		
		private string _Documents_ID;
		
		private string _Lead_Status;
		
		private System.Nullable<System.DateTime> _Interview_Date;
		
		public Lead()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Lead_ID", DbType="VarChar(60)")]
		public string Lead_ID
		{
			get
			{
				return this._Lead_ID;
			}
			set
			{
				if ((this._Lead_ID != value))
				{
					this._Lead_ID = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Date", DbType="DateTime")]
		public System.Nullable<System.DateTime> Date
		{
			get
			{
				return this._Date;
			}
			set
			{
				if ((this._Date != value))
				{
					this._Date = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Lead_Source", DbType="VarChar(60)")]
		public string Lead_Source
		{
			get
			{
				return this._Lead_Source;
			}
			set
			{
				if ((this._Lead_Source != value))
				{
					this._Lead_Source = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Lead_Name", DbType="VarChar(100)")]
		public string Lead_Name
		{
			get
			{
				return this._Lead_Name;
			}
			set
			{
				if ((this._Lead_Name != value))
				{
					this._Lead_Name = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Lead_Email", DbType="VarChar(60)")]
		public string Lead_Email
		{
			get
			{
				return this._Lead_Email;
			}
			set
			{
				if ((this._Lead_Email != value))
				{
					this._Lead_Email = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Phone_Number", DbType="VarChar(60)")]
		public string Phone_Number
		{
			get
			{
				return this._Phone_Number;
			}
			set
			{
				if ((this._Phone_Number != value))
				{
					this._Phone_Number = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Notes", DbType="VarChar(100)")]
		public string Notes
		{
			get
			{
				return this._Notes;
			}
			set
			{
				if ((this._Notes != value))
				{
					this._Notes = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Documents_ID", DbType="VarChar(60)")]
		public string Documents_ID
		{
			get
			{
				return this._Documents_ID;
			}
			set
			{
				if ((this._Documents_ID != value))
				{
					this._Documents_ID = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Lead_Status", DbType="VarChar(60)")]
		public string Lead_Status
		{
			get
			{
				return this._Lead_Status;
			}
			set
			{
				if ((this._Lead_Status != value))
				{
					this._Lead_Status = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Interview_Date", DbType="DateTime")]
		public System.Nullable<System.DateTime> Interview_Date
		{
			get
			{
				return this._Interview_Date;
			}
			set
			{
				if ((this._Interview_Date != value))
				{
					this._Interview_Date = value;
				}
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Staff")]
	public partial class Staff
	{
		
		private string _Staff_ID;
		
		private string _Staff_Name;
		
		private string _Staff_Username;
		
		private string _Staff_Password;
		
		private string _Staff_Role;
		
		public Staff()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Staff_ID", DbType="VarChar(60)")]
		public string Staff_ID
		{
			get
			{
				return this._Staff_ID;
			}
			set
			{
				if ((this._Staff_ID != value))
				{
					this._Staff_ID = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Staff_Name", DbType="VarChar(60)")]
		public string Staff_Name
		{
			get
			{
				return this._Staff_Name;
			}
			set
			{
				if ((this._Staff_Name != value))
				{
					this._Staff_Name = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Staff_Username", DbType="VarChar(60)")]
		public string Staff_Username
		{
			get
			{
				return this._Staff_Username;
			}
			set
			{
				if ((this._Staff_Username != value))
				{
					this._Staff_Username = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Staff_Password", DbType="VarChar(60)")]
		public string Staff_Password
		{
			get
			{
				return this._Staff_Password;
			}
			set
			{
				if ((this._Staff_Password != value))
				{
					this._Staff_Password = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Staff_Role", DbType="VarChar(60)")]
		public string Staff_Role
		{
			get
			{
				return this._Staff_Role;
			}
			set
			{
				if ((this._Staff_Role != value))
				{
					this._Staff_Role = value;
				}
			}
		}
	}
}
#pragma warning restore 1591
