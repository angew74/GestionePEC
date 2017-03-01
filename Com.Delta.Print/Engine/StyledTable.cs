
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.ComponentModel;
using System.Collections;
using System.Drawing.Design;
using System.Xml;

namespace Com.Delta.Print.Engine
{
	/// <summary>
	/// Report element for displaying tabular data.
	/// </summary>
	[DefaultProperty("DataSource")]
	public sealed class StyledTable : ICustomPaint
	{

		#region Declarations

        // Raffaele Russo - 06/03/2012 - Start
        private int cellHeight = 20;
        //private int cellHeight = 18;
        // Raffaele Russo - 06/03/2012 - End

		private DataTable bufferedData;
		private DataTable theData;
		private string dataSource;
		private Color mBorderColor = Color.Black;
		
		private StyledTableColumn[] columns = new StyledTableColumn[0];

		private Font dataFont = new Font("Tahoma",8,FontStyle.Regular);
		private Color mDataFontColor = Color.Black;
		private Color mBackgroundColor = Color.White;
		private Color mAlterDataColor = Color.Red;
		private Color mAlterDataBackgroundColor = Color.AntiqueWhite;
		private Color mSubtotalsColor = Color.Black;
		private Color mAlternatingBackColor = Color.Gainsboro;
		private bool alternateBackColor = false;
		private bool doDrawEmptyRows = false;

		private bool doDrawHeader = true;
		private Font headerFont = new Font("Tahoma",8,FontStyle.Bold);
		private Color mHeaderFontColor = Color.Black;
		private Color mHeaderBackgroundColor = Color.White;
		private string alterDataCondition = String.Empty;


		internal ArrayList AlterRows = new ArrayList();
		internal int Printed = 0;
		internal DataRow[] DataRows = new DataRow[0]{};
		internal DataRow[] ConditionalRows = new DataRow[0]{};

		internal string[] Subtotals = null;

		private string filterExpression = String.Empty;
		private string sortExpression = String.Empty;

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the border color for the StyledTable.
		/// </summary>
		/// <remarks>This property sets the border color of the StyledTable object. This can be any color
		/// from the System.Drawing.Color structure.
		/// </remarks>
		[Category("Appearance"), DefaultValue(typeof(System.Drawing.Color),"Black")]
		public Color BorderColor
		{
			get { return this.mBorderColor; }
			set { this.mBorderColor=value; }
		}


		/// <summary>
		/// Gets or sets the data cell back color for the StyledTable.
		/// </summary>
		[Category("Appearance"), DefaultValue(typeof(System.Drawing.Color),"White")]
		public Color BackgroundColor
		{
			get { return this.mBackgroundColor; }
			set { this.mBackgroundColor=value; }
		}


		/// <summary>
		/// Returns relative data row height after checking if row will be multi-line
		/// </summary>
		/// <param name="CurrentRow">Current Data Row to calculate height</param>
		/// <param name="g">Current Graphics object</param>
		/// <returns>integer value of relative row height</returns>
		internal int CalculateRelativeDataRowHeight(DataRow CurrentRow, Graphics g)
		{
			int relativeDataRowHeight = 0;

			for (int i=0;i<Columns.Length;i++)
			{
				StyledTableColumn CurrentColumn = Columns[i];
				int drawnRowsCounter = 0;

				if (CurrentColumn.Width>0)
				{
					object theObject = CurrentRow[CurrentColumn.Name];
					if (CurrentColumn.FormatMask == "Image")
					{
						try
						{
							if (theObject != null && theObject != DBNull.Value)
							{
								byte[] rawData = (byte[])theObject;
								Image image = Bitmap.FromStream(new System.IO.MemoryStream(rawData));
								//drawnRowsCounter = (int)Math.Ceiling((double)(image.Height+2) / cellHeight);
								drawnRowsCounter = (int)Math.Ceiling((double)(image.Height) / cellHeight);
								image.Dispose();
							}
							else
							{
								drawnRowsCounter = 1;
							}


						}
						catch(Exception)
						{
							drawnRowsCounter = 1;
						}
					}
					else
					{

						string tmpValue = FormatValue(theObject,CurrentRow.Table.Columns[CurrentColumn.Name].DataType, CurrentColumn.FormatMask);
						string theLine = "";
					

						bool hasMore = false;
						do
						{
							hasMore = splitString(ref tmpValue,ref theLine,columns[i].Width,g,dataFont);					
							drawnRowsCounter++;
						}
						while ( hasMore );
					}

					if (drawnRowsCounter>relativeDataRowHeight)
						relativeDataRowHeight = drawnRowsCounter;
				}
			}

			return relativeDataRowHeight;
		}


		// calculates relative header height with respect to cell height
		/// <summary>
		/// Returns relative header row height after checking if headers will be multi-line
		/// </summary>
		/// <param name="g">Graphics Object</param>
		/// <returns>integer value of relative Header row height</returns>
		internal int CalculateRelativeHeaderHeight(Graphics g)
		{
			if (!doDrawHeader) return 0;

			int headerRelativeHeight = 0;

			ArrayList[] headerLabels = new ArrayList[columns.Length];
			for (int i=0;i<columns.Length;i++)
			{
				if (columns[i].Width>0)
				{
					string tmpValue = columns[i].Label;
					string theLine = "";
					headerLabels[i] = new ArrayList();
					bool WillTextWrap = false;
					do
					{
						WillTextWrap = splitString(ref tmpValue,ref theLine,columns[i].Width,g,headerFont);
						headerLabels[i].Add(theLine);
					}
					while(WillTextWrap);

					if (headerLabels[i].Count > headerRelativeHeight)
						headerRelativeHeight = headerLabels[i].Count;
				}
			}
			return headerRelativeHeight;
		}
		
		
		/// <summary>
		/// Gets or sets the height of every cell in the table.
		/// </summary>
		[Category("Appearance"), DefaultValue(18)]
		public int CellHeight
		{
			get {return cellHeight;}
			set 
			{
				int val = Math.Max(1,value);
				cellHeight = val;
			}
		}

		/// <summary>
		/// Gets or sets the sort expression of the StyledTable element.
		/// </summary>
		/// <remarks>Gets or sets the SQL ORDER BY clause that will be performed on table's DataSource before displaying data.
		/// </remarks>
		[Category("Data"), Description("List of sorting columns for displaying data.")]
		public string SortExpression
		{
			get {return sortExpression;}
			set {sortExpression=value;}
		}

		/// <summary>
		/// Gets or sets the filter expression of the StyledTable element.
		/// </summary>
		/// <remarks>Gets or sets the SQL WHERE clause that will be performed on table's DataSource before displaying data.
		/// </remarks>
		[Editor(typeof(Com.Delta.Print.Engine.Editors.ConditionEditor), typeof(UITypeEditor)), Category("Data"), Description("Filter expression to apply when displaying table. Filtering is not applied for tables holding static data.")]
		public string FilterExpression
		{
			get {return filterExpression;}
			set {filterExpression=value;}
		}
		
		/// <summary>
		/// Collection of <see cref="Com.Delta.Print.Engine.StyledTableColumn">Com.Delta.Print.Engine.StyledTableColumn</see> columns contained in this StyledTable element.
		/// </summary>
		[Category("Data"), Description("Table columns.")]
		public StyledTableColumn[] Columns
		{
			get {return columns;}
			set 
			{
				StyledTableColumn[] tmp = value as StyledTableColumn[];
				ArrayList namesList = new ArrayList();
				for (int i=0;i<tmp.Length;i++)
				{
					if (namesList.Contains(tmp[i].Name))
						throw new Exception("Identical column names are not allowed in StyledTable." );
					else
						namesList.Add(tmp[i].Name);
				}

				columns = value;

				if (theData != null)
					UpdateTable();

				if (columns.Length>0)
				{
					int sumOfWidths = 0;
					for (int i=0;i<columns.Length;i++)
					{
						sumOfWidths += columns[i].Width;
					}
					theRegion.Width = sumOfWidths;
				}
			}
		}


		
		
		/// <summary>
		/// Gets or sets the current System.Data.DataTable which contains the raw data for the StyledTable object.
		/// </summary>
		[Browsable(false)]	
		public DataTable Data
		{
			get {return bufferedData;}
			set {bufferedData = value;}
		}


		/// <summary>
		/// Gets or sets the current System.Data.DataTable which contains the page data for the StyledTable object.
		/// </summary>
		[Browsable(false)]	
		internal DataTable DisplayData
		{
			get {return theData;}
			set {theData = value;}
		}


		/// <summary>
		/// Gets or sets the static data for the StyledTable object.
		/// </summary>
		[Category("Data")]
		[Editor(typeof(Com.Delta.Print.Engine.Editors.StaticTableEditor), typeof(UITypeEditor)), Description("Static table data.")]
		public string[][] StaticData
		{
			get
			{
				if (Data != null) 
				{

					int colNumber = Columns.Length;
					int rowNumber = Data.Rows.Count;

					string[][] tmpData = new string[rowNumber+1][];
					for (int i=0;i<rowNumber+1;i++)
					{
						tmpData[i] = new string[colNumber];
					}
				
					for (int j=0;j<colNumber;j++)
					{
						tmpData[0][j] = Columns[j].Name;
					}

					for (int i=0;i<rowNumber;i++)
					{
						for (int j=0;j<colNumber;j++)
						{
							if (Data.Columns.Contains( Columns[j].Name) )
							{
								tmpData[i+1][j] = Data.Rows[i][j].ToString();
							}
							else
							{
								tmpData[i+1][j] = "";
							}
						}

					}

					return tmpData;
				}
				else
				{
					int colNumber = Columns.Length;
					string[][] tmpData = new string[1][];
					tmpData[0] = new string[colNumber];
					for (int j=0;j<colNumber;j++)
					{
						tmpData[0][j] = Columns[j].Name;
					}
					return tmpData;
				}
			}

			set 
			{
				DataTable dataTable = new DataTable();

				for (int i=0;i<this.Columns.Length;i++)
				{
					dataTable.Columns.Add(new DataColumn(this.Columns[i].Name));
				}

				for (int i=0;i<value.Length;i++)
				{
					string[] theRow = new string[this.Columns.Length];
					for (int j=0;j<value[i].Length;j++)
					{
						theRow[j] = value[i][j];
					}

					dataTable.Rows.Add(theRow);
				}

				this.Data = dataTable;
			}
		}

		/// <summary>
		/// Gets or sets the data font for the StyledTable.
		/// </summary>
		/// <remarks>This property sets the font of the StyledTable data rows. This can be any font
		/// from the System.Drawing.Font structure.
		/// </remarks>
		[Category("DataRow Settings"), DefaultValue(typeof(System.Drawing.Font),"Tahoma, 8pt")]
		public Font DataFont
		{
			get {return dataFont;}
			set {dataFont = value;}
		}


		/// <summary>
		/// Gets or sets the data font color for the StyledTable.
		/// </summary>
		/// <remarks>This property sets the data row font color of the StyledTable object. This can be any color
		/// from the System.Drawing.Color structure.
		/// </remarks>
		[Category("DataRow Settings"), DefaultValue(typeof(System.Drawing.Color),"Black")]
		public Color DataFontColor
		{
			get { return this.mDataFontColor; }
			set { this.mDataFontColor=value; }
		}


		/// <summary>
		/// Gets or sets the data color for alternative display (rows that  fulfill the AlterDataCondition). 
		/// </summary>
		/// <remarks>This property sets the color of data rows that fulfill the AlterDataCondition. 
		/// </remarks>
		[Category("DataRow Settings"), Description("This property sets the color of data rows that fulfill the AlterDataCondition."), DefaultValue(typeof(System.Drawing.Color),"Red")]
		public Color AlterDataColor
		{
			get { return this.mAlterDataColor; }
			set { this.mAlterDataColor=value; }
		}

		/// <summary>
		/// Gets or sets the row background color for alternative display (rows that  fulfill the AlterDataCondition). 
		/// </summary>
		/// <remarks>This property sets the background color of data rows that fulfill the AlterDataCondition. 
		/// </remarks>
		[Category("DataRow Settings"), Description("This property sets the background color of data rows that fulfill the AlterDataCondition."), DefaultValue(typeof(System.Drawing.Color),"AntiqueWhite")]
		public Color AlterDataBackColor
		{
			get { return this.mAlterDataBackgroundColor; }
			set { this.mAlterDataBackgroundColor=value; }
		}

		/// <summary>
		/// Gets or sets the even-number row background color (only if AlternateBackColors is set to True). 
		/// </summary>
		[Category("DataRow Settings"), Description("Gets/Sets the even-number row background color (only if AlternateBackColors is set to True)."), DefaultValue(typeof(System.Drawing.Color),"Gainsboro")]
		public Color AlternatingBackColor
		{
			get { return this.mAlternatingBackColor; }
			set { this.mAlternatingBackColor=value; }
		}

		/// <summary>
		/// Gets or sets whether the table will be displayed in two alternating back colors.
		/// </summary>
		[Category("DataRow Settings"), Description("Gets/Sets whether the table will be displayed in two alternating back colors."), DefaultValue(false)]
		public bool AlternateBackColor
		{
			get { return this.alternateBackColor; }
			set { this.alternateBackColor=value; }
		}

		/// <summary>
		/// Gets or sets the data color for subtotals display. 
		/// </summary>
		[Category("DataRow Settings"), Description("This property sets the color of subtotals data row."), DefaultValue(typeof(System.Drawing.Color),"Black")]
		public Color SubtotalsColor
		{
			get { return this.mSubtotalsColor; }
			set { this.mSubtotalsColor=value; }
		}

		
		/// <summary>
		/// The name for DataTable which exports data to this styled table.
		/// </summary>
		/// <remarks>
		/// Relevant for dynamic contents table. The binding itself is done programmaticly with
		/// <see cref="Com.Delta.Print.Engine.ReportDocument.AddData">Com.Delta.Print.Engine.ReportDocument.AddData(DataTable)</see>
		/// method of the Com.Delta.Print.Engine.ReportDocument class.
		/// </remarks>
		[Category("Data"), Description("The name for DataTable which exports data to this styled table. Relevant for dynamic contents table. The binding itself is done programmaticly with AddData(DataTable) method of the DaPrintDocument class.")]
		public string DataSource
		{
			get {return dataSource;}
			set {dataSource = value;}
		}


		/// <summary>
		/// This property sets the query to be used to determine rows that will be shown with alternative color (AlterDataColor).
		/// </summary>
		/// <remarks>
		/// This property is using the same syntax as the Expression property of <see cref="Com.Delta.Print.Engine.ReportDocument.AddData">System.Data.DataColumn</see> property.
		/// </remarks>
		[Editor(typeof(Com.Delta.Print.Engine.Editors.ConditionEditor), typeof(UITypeEditor)), Category("Data"), Description("This property set the query to be used to determine rows that will be shown with alternative color (AlterDataColor).")]	
		public string AlterDataCondition
		{
			get {return alterDataCondition;}
			set {alterDataCondition = value;}
		}


		/// <summary>
		/// Gets oro sets whether empty rows be drawn if any space in table area remains unused.
		/// </summary>
		[Category("DataRow Settings"), Description("Should empty rows be drawn if any space in table area remains unused."), DefaultValue(false)]
		public bool DrawEmptyRows
		{
			get {return doDrawEmptyRows;}
			set {doDrawEmptyRows = value;}
		}


		/// <summary>
		/// Gets or sets whether to draw the table header row in the generated report.
		/// </summary>		
		[Category("Header Settings"), DefaultValue(true)]
		public bool DrawHeader
		{
			get {return doDrawHeader;}
			set {doDrawHeader = value;}
		}


		/// <summary>
		/// Returns possible number of rows for the current table region
		/// </summary>
		/// <returns>integer value of possible number of rows for current table</returns>
		internal int GetPossibleRowNumber()
		{
            // Raffaele Russo - 12/12/2011 - Start
            Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : new Rectangle(Bounds.X - rBounds.X, Bounds.Y - rBounds.Y, Bounds.Width, Bounds.Height);
            // Rectangle paintArea = new Rectangle(theRegion.X - rBounds.X, theRegion.Y - rBounds.Y, theRegion.Width, theRegion.Height);
            // Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;
            // Raffaele Russo - 12/12/2011 - End

			return paintArea.Height / cellHeight;
		}


		/// <summary>
		/// Gets or sets the header background color for the StyledTable element.
		/// </summary>
		/// <remarks>This property sets the header background color of the StyledTable object. This can be any color
		/// from the System.Drawing.Color structure.
		/// </remarks>
		[Category("Header Settings"), DefaultValue(typeof(System.Drawing.Color),"White")]
		public Color HeaderBackgroundColor
		{
			get {return mHeaderBackgroundColor;}
			set {mHeaderBackgroundColor = value;}
		}


		/// <summary>
		/// Gets or sets the header font for the StyledTable.
		/// </summary>
		/// <remarks>This property sets the header font of the StyledTable object. This can be any font
		/// from the System.Drawing.Font structure.
		/// </remarks>
		[Category("Header Settings"), DefaultValue(typeof(System.Drawing.Font),"Tahoma, 8pt, style=Bold")]
		public Font HeaderFont
		{
			get {return headerFont;}
			set {headerFont = value;}
		}


		/// <summary>
		/// Gets or sets the header font color for the StyledTable.
		/// </summary>
		/// <remarks>This property sets the header font color of the StyledTable object. This can be any color
		/// from the System.Drawing.Color structure.
		/// </remarks>
		[Category("Header Settings"), DefaultValue(typeof(System.Drawing.Color),"Black")]
		public Color HeaderFontColor
		{
			get { return this.mHeaderFontColor; }
			set { this.mHeaderFontColor=value; }
		}


		
		
		
		#endregion

		#region Public Overrides

		/// <summary>
		/// Gets the string representation of report element.
		/// </summary>
		public override string ToString()
		{
			return this.DataSource != null ? "Styled table [" + this.DataSource + "]" : "Styled table" ;
		}

		/// <summary>
		/// Gets or sets the name of the report element.
		/// </summary>
		[Category("Data"), Description("The name of StyledTable element.")]
		public override string Name
		{
			get {return name;}
			set {name = value;}
		}

		/// <summary>
		/// Gets or sets the layout of the element.
		/// </summary>
		[Browsable(false)]
		public override LayoutTypes Layout
		{
			get {return layoutType;}
			set {layoutType = ICustomPaint.LayoutTypes.EveryPage;} 		
		}


		/// <summary>
		///  Gets or sets the width of the StyledTable.
		/// </summary>
		[Category("Layout"), Description("The width of the element.")]
		public override int Width
		{
			get {return theRegion.Width;}
			set 
			{
				int val = Math.Max(0,value);
				if (this.HorizontalAlignment == ICustomPaint.HorizontalAlignmentTypes.None || this.HorizontalAlignment == ICustomPaint.HorizontalAlignmentTypes.Left)
					theRegion.Width = val;
				else if (this.HorizontalAlignment == ICustomPaint.HorizontalAlignmentTypes.Right)
				{
					theRegion.X = theRegion.X - val + theRegion.Width;
					theRegion.Width = val;									
				}
				else if (this.HorizontalAlignment == ICustomPaint.HorizontalAlignmentTypes.Center)
				{
					theRegion.X = theRegion.X - value/2 + theRegion.Width/2;
					theRegion.Width = val;									
				}
				

				if (columns.Length>0)
				{
					int sumOfWidths = 0;
					int index = -1;
					for (int i=0;i<columns.Length;i++)
					{
						sumOfWidths += columns[i].Width;
						if (columns[i].Width>0) 
							index=i;
					}

					if (sumOfWidths<theRegion.Width)
					{
						if (index>-1)
							columns[index].Width += theRegion.Width - sumOfWidths;
						else
							columns[columns.Length-1].Width += theRegion.Width - sumOfWidths;
					}
					else
					{
						if (index>-1)
						{
							if (sumOfWidths-columns[index].Width < theRegion.Width)
								columns[index].Width -= sumOfWidths - theRegion.Width;
							else
							{
								double ratio = theRegion.Width==0 ? 0 : ((double)theRegion.Width / sumOfWidths) ;
								for (int i=0;i<columns.Length;i++)
								{
									columns[i].Width = (int)(ratio*columns[i].Width);
								}

							}
						}
						else
						{
							if (sumOfWidths-columns[columns.Length-1].Width < theRegion.Width)
								columns[columns.Length-1].Width -= sumOfWidths - theRegion.Width;
							else
							{
								double ratio = theRegion.Width==0 ? 0 : ((double)theRegion.Width / sumOfWidths) ;
								for (int i=0;i<columns.Length;i++)
								{
									columns[i].Width = (int)(ratio*columns[i].Width);
								}

							}
						}
					}
				}
			}
		}


		

		#endregion

		#region Private Properties

		private int drawHeader(Graphics g, Rectangle TargetRegion, int DrawnSoFar)
		{
			int MaxLinesDrawn = 0;
			int leftBorder = TargetRegion.X;

			Font displayFont = new Font(headerFont.FontFamily, headerFont.SizeInPoints*g.PageScale, headerFont.Style);

			ArrayList[] headerLabels = new ArrayList[columns.Length];
			for (int i=0;i<columns.Length;i++)
			{
				if (columns[i].Width>0)
				{
					string tmpValue = columns[i].Label;
					string theLine = "";
					headerLabels[i] = new ArrayList();
					bool willTextWrap = false;
					do
					{
						willTextWrap = splitString(ref tmpValue, ref theLine, columns[i].Width, g, displayFont);
						headerLabels[i].Add(theLine);
					}
					while(willTextWrap);

					if (headerLabels[i].Count > MaxLinesDrawn)
						MaxLinesDrawn = headerLabels[i].Count;
				}
			}

			// draw header background
			if (paintTransparent)
				g.FillRectangle(new SolidBrush(ShiftColor(mHeaderBackgroundColor)), TargetRegion.X, TargetRegion.Y, TargetRegion.Width, cellHeight*MaxLinesDrawn);	
			else
				g.FillRectangle(new SolidBrush(mHeaderBackgroundColor), TargetRegion.X, TargetRegion.Y ,TargetRegion.Width, cellHeight*MaxLinesDrawn);	

			for (int i=0;i<columns.Length;i++)
			{
				if (Columns[i].Width>0)
				{
					ArrayList theLines = headerLabels[i];

					for (int j=0;j<theLines.Count;j++)
					{
						SizeF sf = g.MeasureString(theLines[j].ToString(), displayFont);				
						float yPos = leftBorder + 1;

						switch (columns[i].Alignment)
						{
							case StyledTableColumn.AlignmentType.Left:
								yPos = leftBorder + 1;
								break;

							case StyledTableColumn.AlignmentType.Center:
								yPos = leftBorder + ((columns[i].Width / 2) - (sf.Width / 2));
								break;

							case StyledTableColumn.AlignmentType.Right:
								yPos = leftBorder + columns[i].Width - sf.Width;
								break;

							default:
								yPos = leftBorder + 1;
								break;
						}

						g.DrawString(theLines[j].ToString(), displayFont, new SolidBrush(this.mHeaderFontColor), yPos , TargetRegion.Top+cellHeight*j+cellHeight/2-sf.Height/2);
					}
					leftBorder += columns[i].Width;
				}
				
			}

			g.DrawLine(new Pen(this.mBorderColor,1), TargetRegion.X, TargetRegion.Y, TargetRegion.X + TargetRegion.Width, TargetRegion.Y );

			return DrawnSoFar+MaxLinesDrawn;
		}


		private int drawSubtotals(Graphics g, int drawnSoFar)
		{
            // Raffaele Russo - 12/12/2011 - Start
            Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : new Rectangle(Bounds.X - rBounds.X, Bounds.Y - rBounds.Y, Bounds.Width, Bounds.Height);
            // Rectangle paintArea = new Rectangle(theRegion.X - rBounds.X, theRegion.Y - rBounds.Y, theRegion.Width, theRegion.Height);
            // Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;
            // Raffaele Russo - 12/12/2011 - End

			Font displayFont = new Font(dataFont.FontFamily, dataFont.SizeInPoints*g.PageScale, dataFont.Style);
			int	topBorder = paintArea.Top + drawnSoFar*cellHeight;


			if (mBackgroundColor != Color.Transparent)
			{
				if (paintTransparent)
					g.FillRectangle(new SolidBrush(ShiftColor(mBackgroundColor)), paintArea.X, topBorder, paintArea.Width, cellHeight); 
				else
					g.FillRectangle(new SolidBrush(mBackgroundColor), paintArea.X, topBorder, paintArea.Width , cellHeight); 
			}

			g.DrawLine(new Pen(this.mBorderColor,1), paintArea.X, topBorder, paintArea.Right, topBorder);

			int maxDrawnRows = 0;
			int leftBorder = paintArea.X;

			if (this.Subtotals != null)
			{
				for (int i=0;i<this.Subtotals.Length;i++)
				{

					SizeF sf = g.MeasureString(this.Subtotals[i], displayFont);

					float yPos = leftBorder+1;

					if (Columns[i].Width>0)
					{

						switch (columns[i].Alignment)
						{
							case StyledTableColumn.AlignmentType.Left:
								yPos = leftBorder + 1;
								break;

							case StyledTableColumn.AlignmentType.Center:
								yPos = leftBorder + ((columns[i].Width / 2) - (sf.Width / 2));
								break;

							case StyledTableColumn.AlignmentType.Right:
								yPos = leftBorder + columns[i].Width - sf.Width;
								break;

							default:
								yPos = leftBorder + 1;
								break;
						}

						g.SetClip(new RectangleF(leftBorder, topBorder, columns[i].Width, this.cellHeight));
						g.DrawString(this.Subtotals[i], displayFont, new SolidBrush(this.mSubtotalsColor), yPos, topBorder + cellHeight/2 - sf.Height/2);		
						g.ResetClip();

						leftBorder += columns[i].Width;
					}
				}
			}

			return drawnSoFar + 1;
		}
		
		
		private int drawDataRow(Graphics g, DataRow printRow, int drawnSoFar, bool useAlterColor)
		{

			int numOfRows = CalculateRelativeDataRowHeight(printRow, g);

            // Raffaele Russo - 12/12/2011 - Start
            Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : new Rectangle(Bounds.X - rBounds.X, Bounds.Y - rBounds.Y, Bounds.Width, Bounds.Height);
            // Rectangle paintArea = new Rectangle(theRegion.X - rBounds.X, theRegion.Y - rBounds.Y, theRegion.Width, theRegion.Height);
            // Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;
            // Raffaele Russo - 12/12/2011 - End

			Font displayFont = new Font(dataFont.FontFamily, dataFont.SizeInPoints*g.PageScale, dataFont.Style);

			//if (theRow > theData.Rows.Count || theRow < 0 || drawnSoFar >= GetPossibleRowNumber() || drawnSoFar+numOfRows > GetPossibleRowNumber()) return drawnSoFar;

			if (drawnSoFar >= GetPossibleRowNumber() || drawnSoFar+numOfRows > GetPossibleRowNumber()) return drawnSoFar;


			if ( printRow.RowError == "Subtotal")
			{
				return drawSubtotals(g, drawnSoFar);				
			}

			int maxDrawnRows = 0;			
			int drawingCellSize = (int)(cellHeight * g.PageScale);
			int	topBorder = paintArea.Top + drawnSoFar*drawingCellSize;

			Rectangle rowRect = new Rectangle(paintArea.X, topBorder, paintArea.Width , numOfRows*drawingCellSize);

			if (alterDataCondition!=String.Empty && AlterRows.Contains(printRow))
			{
				if (mAlterDataBackgroundColor != Color.Transparent)
				{
					if (paintTransparent)
						g.FillRectangle(new SolidBrush(ShiftColor(mAlterDataBackgroundColor)), paintArea.X, topBorder, paintArea.Width , numOfRows*cellHeight); 
					else
						g.FillRectangle(new SolidBrush(mAlterDataBackgroundColor), paintArea.X, topBorder, paintArea.Width, numOfRows*cellHeight); 
				}
			}
			else
			{
				if (useAlterColor)
				{
					if (mAlternatingBackColor != Color.Transparent)
					{
						if (paintTransparent)
							g.FillRectangle(new SolidBrush(ShiftColor(mAlternatingBackColor)), rowRect); 
						else
							g.FillRectangle(new SolidBrush(mAlternatingBackColor), rowRect); 
					}
				}
				else
				{
					if (mBackgroundColor != Color.Transparent)
					{
						if (paintTransparent)
							g.FillRectangle(new SolidBrush(ShiftColor(mBackgroundColor)), rowRect); 
						else							
							g.FillRectangle(new SolidBrush(mBackgroundColor), rowRect); 
					}
				}
			}

			g.DrawLine(new Pen(this.mBorderColor,1), paintArea.X, topBorder, paintArea.Right, topBorder);

			int leftBorder = paintArea.X;
			for (int i=0;i<printRow.ItemArray.Length;i++)
			{
				if (Columns[i].Width>0)
				{
					object theObject = printRow.ItemArray.GetValue(i);
					int drawnRowsCounter = 0;

					if (Columns[i].FormatMask == "Image")
					{
						try
						{
							// draw image object
							byte[] imageData = (byte[])theObject;
							Image image = Bitmap.FromStream(new System.IO.MemoryStream(imageData));

							int imageHeight = image.Height;
						
							int xPos = leftBorder + 1;						
							int yPos = imageHeight <= cellHeight ? topBorder + cellHeight * drawnRowsCounter + cellHeight/2 - image.Height/2 : topBorder + cellHeight * drawnRowsCounter + 1;

							switch (Columns[i].Alignment)
							{
								case StyledTableColumn.AlignmentType.Left:
									xPos = leftBorder + 1;
									break;

								case StyledTableColumn.AlignmentType.Center:
									xPos = leftBorder + ((columns[i].Width / 2) - (image.Width / 2));
									break;

								case StyledTableColumn.AlignmentType.Right:
									xPos = leftBorder + columns[i].Width - image.Width - 1;
									break;

								default:
									xPos = leftBorder + 1;
									break;
							}

							g.DrawImage(image, xPos, yPos);
							image.Dispose();						
							drawnRowsCounter = cellHeight == 0 ? 0 : (int)Math.Ceiling((double)imageHeight / cellHeight);
						}
						catch(Exception)
						{
							drawnRowsCounter = 1;
						}
						
					}
					else
					{

						// format object as string
						string tmpValue = FormatValue(theObject, theData.Columns[i].DataType, Columns[i].FormatMask);

						string theLine = "";
					

						bool hasMore = false;
						do
						{						
							hasMore = splitString(ref tmpValue, ref theLine, columns[i].Width, g, displayFont);
							SizeF sf = g.MeasureString(theLine, displayFont);

							float yPos = leftBorder + 1;

							switch (columns[i].Alignment)
							{
								case StyledTableColumn.AlignmentType.Left:
									yPos = leftBorder + 1;
									break;

								case StyledTableColumn.AlignmentType.Center:
									yPos = leftBorder + ((columns[i].Width / 2) - (sf.Width / 2));
									break;

								case StyledTableColumn.AlignmentType.Right:
									yPos = leftBorder + columns[i].Width - sf.Width;
									break;

								default:
									yPos = leftBorder + 1;
									break;
							}

							if (alterDataCondition!=String.Empty && AlterRows.Contains(printRow))
							{
								g.DrawString(theLine, displayFont, new SolidBrush(this.mAlterDataColor), yPos, topBorder + cellHeight*drawnRowsCounter+cellHeight/2-sf.Height/2);	
							}
							else
							{
								g.DrawString(theLine, displayFont, new SolidBrush(this.mDataFontColor), yPos, topBorder + cellHeight*drawnRowsCounter+cellHeight/2-sf.Height/2);	
							}
							drawnRowsCounter++;
						}
						while ( hasMore );
					}

					leftBorder += columns[i].Width;

					if (drawnRowsCounter>maxDrawnRows)
						maxDrawnRows = drawnRowsCounter;
				}
			}

			return drawnSoFar+maxDrawnRows;
		}

		
		//   formating datatable objects
		//   TODO : improve this function 
		//   for instance : dates with respect to user input locale, etc.
		private string FormatValue(object theObject,Type tip, string FormatMask)
		{
			string theValue = "";
			if ( theObject == DBNull.Value )
			{
				theValue = "";
			}
			else
			{
				try
				{
					if (FormatMask != "")
					{
						theValue = string.Format("{0:"+FormatMask+"}", theObject);
					}
					else
					{
						if (tip == System.Type.GetType("System.DateTime") )
						{
							theValue = ((DateTime)theObject).ToString("dd.MM.yyyy.");
						}
						else if (tip == System.Type.GetType("System.Decimal") )
						{
							theValue = ((Decimal)theObject).ToString("f02"); 
						}
						else if (tip == System.Type.GetType("System.UInt32") || tip == System.Type.GetType("System.UInt16") || tip == System.Type.GetType("System.UInt64") || tip == System.Type.GetType("System.Int16") || tip == System.Type.GetType("System.Int32") || tip == System.Type.GetType("System.Int64"))
						{
							theValue = theObject.ToString();
						}
						else
						{
							theValue = theObject.ToString();
						}
					}
				}
				catch
				{
					if (tip == System.Type.GetType("System.DateTime") )
					{
						theValue = ((DateTime)theObject).ToString("dd.MM.yyyy.");
					}
					else if (tip == System.Type.GetType("System.Decimal") )
					{
						theValue = ((Decimal)theObject).ToString("f02");
					}
					else if (tip == System.Type.GetType("System.UInt32") || tip == System.Type.GetType("System.UInt16") || tip == System.Type.GetType("System.UInt64") || tip == System.Type.GetType("System.Int16") || tip == System.Type.GetType("System.Int32") || tip == System.Type.GetType("System.Int64"))
					{
						theValue = theObject.ToString();
					}
					else
					{
						theValue = theObject.ToString();
					}
				}
				
			}
			return theValue;
		}

		
		private bool splitString(ref string theSource,ref string theResult,int theWidth,Graphics g,Font theFont)
		{
			int lastBlank = -1;
			string tmpBuffer = "";
			bool hasMore = false;

			for (int i=0;i<theSource.Length;i++)
			{
				
				// handling new lines
				if (theSource[i] == '\r' && i<theSource.Length-1 && theSource[i+1] == '\n')
				{
					theResult = theSource.Substring(0,i);
					theSource = theSource.Substring(i+2);
					hasMore = theSource.Length > 0;
					return hasMore;
				}
				else if (theSource[i] == '\n')
				{
					theResult = theSource.Substring(0, i);
					theSource = theSource.Substring(i + 1);
					hasMore = theSource.Length > 0;
					return hasMore;
				}


				if ( theSource[i] == ' ')
					lastBlank = i;
				tmpBuffer += theSource[i];
				SizeF sf = g.MeasureString(tmpBuffer,theFont);
				
				if ( sf.Width > theWidth)
				{
					int breakPoint = lastBlank != -1? lastBlank+1 : (i==0?1:i);
					theResult = theSource.Substring(0,breakPoint);
					theSource = theSource.Substring(breakPoint);
					hasMore = true;
					return hasMore;
				}
			}
			
			theResult = theSource;
			theSource = "";
			return hasMore;
		}


		#endregion

		#region Private Functions

		private void UpdateTable()
		{
			DataTable table = new DataTable();

			for (int i=0;i<columns.Length;i++)
				table.Columns.Add(columns[i].Name);
			

			for (int i=0;i<theData.Rows.Count;i++)
			{
				table.Rows.Add(table.NewRow());		
				for (int j=0;j<theData.Columns.Count;j++)
				{
					if (table.Columns.Contains(theData.Columns[j].ColumnName))
						table.Rows[i][theData.Columns[j].ColumnName] = theData.Rows[i][j];
				}
			}

			bufferedData = table;
		}


		private void drawEmptyRows(Graphics g, int drawnSoFar)
		{
			int topBorder;

            // Raffaele Russo - 12/12/2011 - Start
            Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : new Rectangle(Bounds.X - rBounds.X, Bounds.Y - rBounds.Y, Bounds.Width, Bounds.Height);
            // Rectangle paintArea = new Rectangle(theRegion.X - rBounds.X, theRegion.Y - rBounds.Y, theRegion.Width, theRegion.Height);
            // Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;
            // Raffaele Russo - 12/12/2011 - End
			
			topBorder = paintArea.Top + cellHeight * Math.Min(drawnSoFar,GetPossibleRowNumber() );
			
			for (int j=0;j< GetPossibleRowNumber() - Math.Min(drawnSoFar,GetPossibleRowNumber());j++)
			{
				int row = j + drawnSoFar + (this.DrawHeader ? 1 : 0);
				if (alternateBackColor && row%2==1)
				{
					if (this.mAlternatingBackColor != Color.Transparent)
					{
						if (paintTransparent)
							g.FillRectangle(new SolidBrush(ShiftColor(mAlternatingBackColor)), paintArea.X, topBorder, paintArea.Width, cellHeight); 
						else
							g.FillRectangle(new SolidBrush(mAlternatingBackColor), paintArea.X, topBorder, paintArea.Width, cellHeight); 
					}
				}
				else
				{
					if (mBackgroundColor != Color.Transparent)
					{
						if (paintTransparent)
							g.FillRectangle(new SolidBrush(ShiftColor(mBackgroundColor)), paintArea.X, topBorder, paintArea.Width, cellHeight); 
						else
							g.FillRectangle(new SolidBrush(mBackgroundColor), paintArea.X, topBorder, paintArea.Width, cellHeight); 
					}
				}
				g.DrawLine(new Pen(this.mBorderColor,1), paintArea.X, topBorder, paintArea.Right , topBorder);				

				topBorder += cellHeight;
			}
		}

		
		private void drawBorders(Graphics g, int drawnSoFar)
		{
            // Raffaele Russo - 12/12/2011 - Start
            Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : new Rectangle(Bounds.X - rBounds.X, Bounds.Y - rBounds.Y, Bounds.Width, Bounds.Height);
            // Rectangle paintArea = new Rectangle(theRegion.X - rBounds.X, theRegion.Y - rBounds.Y, theRegion.Width, theRegion.Height);
            // Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;
            // Raffaele Russo - 12/12/2011 - End			
            
            int rectHeight;

			if (doDrawEmptyRows)
				rectHeight = cellHeight * GetPossibleRowNumber() ;
			else
				rectHeight = cellHeight * ( Math.Min(drawnSoFar,GetPossibleRowNumber()) );

			g.DrawRectangle(new Pen(this.mBorderColor,1), paintArea.X, paintArea.Y , paintArea.Width, rectHeight);

			int leftBorder = paintArea.X;
			for (int i=0;i<columns.Length;i++)
			{
				if (columns[i].Width>0)
				{
					g.DrawLine(new Pen(this.mBorderColor,1), leftBorder, paintArea.Y ,leftBorder, paintArea.Y + rectHeight);
					leftBorder += columns[i].Width;
				}
			}
		}


		#endregion

		#region ICustomPaint Members


		/// <summary>
		/// Paints the StyledTable element.
		/// </summary>
		/// <param name="g">The Graphics object to draw</param>
		/// <remarks>Causes the StyledTable element to be painted to the screen.</remarks>
		public override void Paint(System.Drawing.Graphics g)
		{
            // Raffaele Russo - 12/12/2011 - Start
            Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : new Rectangle(Bounds.X - rBounds.X, Bounds.Y - rBounds.Y, Bounds.Width, Bounds.Height);
            // Rectangle paintArea = new Rectangle(theRegion.X - rBounds.X, theRegion.Y - rBounds.Y, theRegion.Width, theRegion.Height);
            // Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;
            // Raffaele Russo - 12/12/2011 - End

			if (this.section.Document.DesignMode)
			{
				theData = bufferedData;
				Pen borderPen = new Pen(Color.LightGray, 1);
				borderPen.DashStyle = DashStyle.Dot;
				borderPen.DashPattern = new float[]{1,3};
				g.DrawRectangle(borderPen, this.X, this.Y , this.Width, this.Height);

				
				int colPos = this.X;
				for (int i=0;i<this.Columns.Length-1;i++)
				{
					StyledTableColumn column = this.Columns[i];
					if (column.Width>0)
					{
						colPos += column.Width;
						g.DrawLine(borderPen, colPos, this.Y, colPos, this.Y + this.Height);  
					}
				}

				borderPen.Dispose();
			}
			else if (HasDataSource())
			{
				theData = bufferedData;
			}


			int RowsDrawnSoFar = 0;
			int maxRows = this.GetPossibleRowNumber();

			if (doDrawHeader)
			{
				int relativeHeaderHeight = this.CalculateRelativeHeaderHeight(g);
				if (relativeHeaderHeight <= maxRows)
					RowsDrawnSoFar = drawHeader(g, paintArea, RowsDrawnSoFar);
			}


			if (theData != null)
			{
				for (int i=0;i<theData.Rows.Count;i++)
				{
					int bufferRowsDrawnSoFar = RowsDrawnSoFar;
					RowsDrawnSoFar = drawDataRow(g, theData.Rows[i] , RowsDrawnSoFar, (alternateBackColor && i%2==1));	
					if (RowsDrawnSoFar==bufferRowsDrawnSoFar)
						break;
				}					
			}

			if (doDrawEmptyRows)
				drawEmptyRows(g, RowsDrawnSoFar);

			drawBorders(g, RowsDrawnSoFar);
		}


		/// <summary>
		/// Clones the structure of the StyledTable, including all properties.
		/// </summary>
		/// <returns><see cref="Com.Delta.Print.Engine.ChartBox">Com.Delta.Print.Engine.ChartBox</see></returns>
		public override object Clone()
		{
			StyledTable tmp = new StyledTable(this.X, this.Y, this.Width, this.Height, this.section);

			

			StyledTableColumn[] cols = new StyledTableColumn[this.Columns.Length];
			for (int i=0;i<this.Columns.Length;i++)
				cols[i] = this.Columns[i].Clone() as StyledTableColumn;
			tmp.Columns = cols;

			tmp.Layout = this.Layout;
			tmp.Data = this.Data;
			tmp.DataSource = this.DataSource;
			tmp.CellHeight = this.CellHeight;
			tmp.DrawEmptyRows = this.DrawEmptyRows;
			tmp.DrawHeader = this.DrawHeader;
			tmp.HeaderBackgroundColor = this.HeaderBackgroundColor;
			tmp.HeaderFont = this.HeaderFont;
			tmp.HeaderFontColor = this.HeaderFontColor;
			tmp.DataFont = this.DataFont;
			tmp.DataFontColor = this.DataFontColor;
			tmp.BackgroundColor = this.BackgroundColor;
			tmp.BorderColor = this.BorderColor;
			tmp.SubtotalsColor = this.SubtotalsColor;

			tmp.AlterDataColor = this.AlterDataColor;
			tmp.AlterDataBackColor = this.AlterDataBackColor;
			tmp.AlterDataCondition = this.AlterDataCondition;
			tmp.AlternatingBackColor = this.AlternatingBackColor;
			tmp.AlternateBackColor = this.AlternateBackColor;
			tmp.FilterExpression = this.FilterExpression;
			tmp.SortExpression = this.SortExpression;
			return tmp;
		}

		internal override bool IsDynamic()
		{
			return true;
		}

		internal override bool CanStretch()
		{
//			Console.WriteLine(this.DataSource != string.Empty);
			return (this.DataSource != null && this.DataSource != string.Empty);
		}


		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the StyledTable class.
		/// </summary>
		internal StyledTable(XmlNode node, Section parent)
		{
			section = parent;
			Init(node);
		}


		/// <summary>
		/// Initializes a new instance of the StyledTable class.
		/// </summary>
		/// <param name="originX">x-position of the new StyledTable</param>
		/// <param name="originY">y-position of the new StyledTable</param>
		/// <param name="width">Width of the new StyledTable</param>
		/// <param name="height">Height of the new StyledTable</param>
		/// <param name="parent">Parent of the new StyledTable</param>
		public StyledTable(int originX,int originY,int width,int height, Section parent)
		{
			section = parent;
			theRegion = new Rectangle(originX,originY,width,height);
		}

		
		#endregion

		#region Internal Methods

		internal bool HasDataSource()
		{
			return (this.DataSource!=null && this.DataSource!=String.Empty);
		}

		internal bool HasSubtotals
		{
			get
			{
				foreach(StyledTableColumn column in columns)
				{
					if (column.TotalType != StyledTableColumn.SubtotalType.None)
					{
						return true;
					}
				}
				return false;
			}
		}

		internal void SetStaticData(DataTable table)
		{
			DataTable dataTable = new DataTable();

			for (int i=0;i<this.Columns.Length;i++)
			{
				dataTable.Columns.Add(new DataColumn(this.Columns[i].Name));
			}

			for (int i=0;i<table.Rows.Count;i++)
			{
				string[] theRow = new string[this.Columns.Length];
				for (int j=0;j<theRow.Length;j++)
				{
					if (table.Columns.Contains(this.Columns[j].Name))
					{
						theRow[j] = table.Rows[i][this.Columns[j].Name].ToString();
					}
					else
					{
						theRow[j] = "";
					}
				}

				dataTable.Rows.Add(theRow);
			}

			this.Data = dataTable;
		}

		internal void CreateDisplayTable()
		{
			if (bufferedData != null)
			{
				DataTable dataTable = bufferedData.Copy();
				for (int i=0;i<dataTable.Columns.Count;i++)
				{
					for (int j=0;j<dataTable.Rows.Count;j++)
					{
						string stringData = dataTable.Rows[j][i] as string;						
						dataTable.Rows[j][i] = this.section.ResolveParameterValues(stringData);
						dataTable.Rows[j].RowError = bufferedData.Rows[j].RowError;
					}
				}

				theData = dataTable;
			}
		}

		internal int GetVisibleColumnsCount()
		{
			int cnt = 0;
			foreach (StyledTableColumn col in this.Columns)
			{
				if (col.Width>0)
					cnt++;				
			}
			return cnt;
		}

		#endregion

	}
}

