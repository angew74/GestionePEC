

using System;
using System.ComponentModel;
using System.Drawing.Design;


namespace Com.Delta.PrintManager.Engine
{
	/// <summary>
	/// Class representing the StyledTableColumn object.
	/// </summary>
	[DefaultProperty("Name")]
	public sealed class StyledTableColumn : ICloneable
	{

		#region Declarations

		/// <summary>
		/// Enumeration of possible horizontal alignments for the StyledTableColumn text
		/// </summary>
		public enum AlignmentType
		{
			/// <summary>Left Alignment in the coloumn</summary>
			Left = 1,
			/// <summary>Center Alignment in the coloumn</summary>
			Center,
			/// <summary>Right Alignment in the coloumn</summary>
			Right
		};

		/// <summary>
		/// Enumeration of possible horizontal alignments for the StyledTableColumn text
		/// </summary>
		public enum SubtotalType
		{
			/// <summary>Left Alignment in the coloumn</summary>
			None = 1,
			/// <summary>Center Alignment in the coloumn</summary>
			Sum,
			/// <summary>Right Alignment in the coloumn</summary>
			Average
		};

		private string mName;
		private string mLabel;
		private int mWidth;
		private string mFormatMask = "";
		private AlignmentType mAlignment;
		private SubtotalType mSubtotalType = SubtotalType.None;
		private string subtotalPrefix = "";

		#endregion

		#region Public Properties

		/// <summary>
		///  Gets or sets the horizontal alignment of text in the StyledTableColumn.
		/// </summary>
		[Description("Horizontal alignment in the column, relative to borders."), DefaultValue(AlignmentType.Left)]
		public AlignmentType Alignment
		{
			get {return mAlignment;}
			set {mAlignment = value;}
		}

		/// <summary>
		///  Gets or sets the subtotal type for this StyledTableColumn.
		/// </summary>
		[Description("Subtotal type for this column."), DefaultValue(SubtotalType.None)]
		public SubtotalType TotalType
		{
			get {return mSubtotalType;}
			set {mSubtotalType = value;}
		}

		/// <summary>
		/// Gets or sets the display text prefix in subtotal row.
		/// </summary>
		public string Prefix
		{
			get {return subtotalPrefix;}
			set {subtotalPrefix = value;}
		}


		/// <summary>
		/// Gets or sets the FormatMask used to format the data being placed into the column.
		/// </summary>
		/// <remarks>These can be any standard formatting of strings used in the string.Format method. This
		/// also depends on the data-type being passed in. For DateTime datatypes, you can use a FormatMask of
		/// "yyyy-MM-dd" for example. Or for currency use a "c" (without quotes for both of these examples)</remarks>
		[Editor(typeof(Com.Delta.PrintManager.Engine.Editors.FormatMaskEditor), typeof(UITypeEditor))]
		public string FormatMask
		{
			get { return this.mFormatMask; }
			set { this.mFormatMask=value; }
		}


		/// <summary>
		/// Gets or sets the label of the column displayed in the report.
		/// </summary>
		public string Label
		{
			get {return mLabel;}
			set {mLabel = value;}
		}


		/// <summary>
		/// Gets or sets the name of the column. This name must match the DataTable column name for user provided data.
		/// </summary>
		public string Name
		{
			get {return mName;}
			set {mName = value;}
		}



		/// <summary>
		///  Gets or sets the width of the StyledTableColumn. Zero=width columns are not displayed (hidden columns).
		/// </summary>
		/// <remarks>This will affect the width of the overall table.</remarks>
		[Description("The width of the element.")]
		public int Width
		{
			get {return mWidth;}
			set {mWidth = Math.Max(0,value);}
		}


		#endregion
		
		#region ICloneable Members

		/// <summary>
		/// Clones the structure of the StyledTableColumn, including all properties
		/// </summary>
		/// <returns><see cref="Com.Delta.PrintManager.Engine.StyledTableColumn">daReport.StyledTableColumn</see></returns>
		public object Clone()
		{
			StyledTableColumn tmp = new StyledTableColumn();
			tmp.Name = this.Name;
			tmp.Label = this.Label;
			tmp.FormatMask = this.FormatMask;
			tmp.Width = this.Width;
			tmp.Alignment = this.Alignment;
			tmp.TotalType = this.TotalType;
			tmp.Prefix = this.Prefix;
			return tmp;
		}

		
		#endregion

		#region Creator

		/// <summary>
		/// Initializes a new instance of the StyledTableColumn class.
		/// </summary>
		public StyledTableColumn()
		{
			Random random = new Random();
			int id = (int)(random.NextDouble()*1000000);
			mName = String.Format("columnName_{0}", id);
			mLabel = "columnLabel";
			mWidth = 80;
			mAlignment = AlignmentType.Left;
		}


		#endregion

	}
}