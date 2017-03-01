using System;
using System.ComponentModel;

namespace Com.Delta.PrintManager.Engine
{
	/// <summary>
	/// Class containing data neccessarry for report generation. 
	/// </summary>
	
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class StartInfo
	{
		private bool preloadData = false;
		private bool ignorePreloadExceptions = false;


		[DefaultValue(false), Description("If set to True, prior to the report generation, the report engine will try to contact database using report connection and load data by executing report queries.")]
		public bool PreloadData
		{
			get {return preloadData;}
			set {preloadData = value;}
		}

		[DefaultValue(false), Description("Gets or sets whether exceptions generated during data preloading will be ignored. Meaningfull only if PreloadData is set to True.")]
		public bool IgnorePreloadExceptions
		{
			get {return ignorePreloadExceptions;}
			set {ignorePreloadExceptions = value;}
		}

		public override string ToString()
		{
			return String.Empty;
		}


	}
}
