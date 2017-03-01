using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace Com.Delta.PrintManager.Engine
{
	/// <summary>
	/// Class holding query data.
	/// </summary>
	public class QueryData
	{
		private string dataSource = "printTable";
		private string query = "select column1,column2 from tablename";

		/// <summary>
		/// Creates new instance of QueryData class.
		/// </summary>
		public QueryData()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public override string ToString()
		{
			return dataSource + " [" + query + "]";

		}


		/// <summary>
		/// Gets or sets the DataSource name that is populated through query command.
		/// </summary>
		[Category("General"), Description("DataSource name that is populated through query command.")]
		public string DataSource
		{
			get {return dataSource;}
			set {dataSource = value;}
		}

		/// <summary>
		/// Gets or sets the query select command.
		/// </summary>
		[Category("General"), Description("Query select command."), Editor(typeof(Com.Delta.PrintManager.Engine.Editors.PlainTextEditor), typeof(UITypeEditor))]
		public string SelectCommand
		{
			get {return query;}
			set {query = value;}
		}
	}
}
