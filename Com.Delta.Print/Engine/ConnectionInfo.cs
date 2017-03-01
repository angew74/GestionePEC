using System;
using System.ComponentModel;
using System.Drawing.Design;


namespace Com.Delta.Print.Engine
{
	/// <summary>
	/// Summary description for ConnectionInfo.
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter)), DefaultProperty("ConnectionString")]
	public sealed class ConnectionInfo
	{
		private string name = "dbConnection";
		private string connectionString = "";
		private string connectionType = "System.Data.Odbc.OdbcConnection";
		private string connectionAssembly = "System.Data";

		public ConnectionInfo()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		/// Creates new instance of ConnectionInfo with given parameters.
		/// </summary>
		public ConnectionInfo(string assembly, string type, string connectionString)
		{
			this.connectionAssembly = assembly;
			this.connectionType = type;
			this.connectionString = connectionString;
		}

		/// <summary>
		/// Connection string used to access database.
		/// </summary>
		[Category("General"), Description("Connection string used to access database."), Editor(typeof(Com.Delta.Print.Engine.Editors.PlainTextEditor), typeof(UITypeEditor))]
		public string ConnectionString
		{
			get {return connectionString;}
			set {connectionString = value;}
		}

		/// <summary>
		/// Full name of the .NET type that encapsulates database connection. This type implements System.Data.IDbConnection interface.
		/// </summary>
		[Category("General"), Description("Full name of the .NET type that encapsulates database connection. This type implements System.Data.IDbConnection interface."), DefaultValue("System.Data.Odbc.OdbcConnection")]
		public string Type
		{
			get {return connectionType;}
			set {connectionType = value;}
		}

		/// <summary>
		/// Gets or sets the assembly name that contains the connection type referenced by ConnectionInfo.Type property.
		/// </summary>
		[Category("General"), Description("Assembly name that contains the connection type referenced by Type property."), DefaultValue("System.Data")]
		public string Assembly
		{
			get {return connectionAssembly;}
			set {connectionAssembly = value;}
		}

		/// <summary>
		/// Gets or sets the connection name.
		/// </summary>
		[Category("General"), Description("Connection display name."), ParenthesizePropertyNameAttribute(true), DefaultValue("dbConnection")]
		public string Name
		{
			get {return name;}
			set {name = value;}
		}

		public override string ToString()
		{
			return String.Empty;
		}

	}
}
