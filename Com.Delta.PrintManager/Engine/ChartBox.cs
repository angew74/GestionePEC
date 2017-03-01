

using System;
using System.Drawing;
using System.Data;
using System.ComponentModel;
using System.Collections;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Xml;


namespace Com.Delta.PrintManager.Engine
{
	/// <summary>
	/// Report element for displaying charted data.
	/// </summary>
	/// <remarks>The ChartBox is used in reports where chated graphical representation of data is used.</remarks>
	[DefaultProperty("XCategories")]
	public sealed class ChartBox : ICustomPaint 
	{

		#region Declarations

		/// <summary>
		/// Enumeration of possible chart types
		/// </summary>
		public enum ChartType
		{
			/// <summary>Bar chart.</summary>
			Bar = 1,
			/// <summary>Pie chart in 3D.</summary>
			Bar3D,
			/// <summary>Pie chart.</summary>
			Pie,
			/// <summary>Pie chart in 3D.</summary>
			Pie3D
			
		};

		private Rectangle mMapArea = new Rectangle(0,0,0,0);
		private Rectangle mLegendArea = new Rectangle(0,0,0,0);

		private string mTitle = "Chart Title";
		private string mXLabel = "Categories";

		private ChartType mChartType = ChartType.Bar;
		private string[] mXCategories = new string[0];
		private ArrayList mYSeries = new ArrayList();
		private string[] mSeriesNames = new string[0];
		private bool mShowLegend = false;

		private Color mBorderColor = Color.Black;
		private Color mMapAreaColor = Color.WhiteSmoke;
		private Color mBackgroundColor = Color.White;
		private int mBorderWidth = 1;
		private Font mTitleFont = new Font("Tahoma",8,FontStyle.Bold);
		private Font mLabelFont = new Font("Tahoma",8,FontStyle.Regular);
		private Color[] mSeriesColors = new Color[0];
		private Color[] mPieColors = new Color[0];

		private float mMaxY = 100f;
		private float mMinY = 0f;

		private double depth = 50;
		private double angle = 0.523;

		private Font titleFont = null;
		private Font labelFont = null;

		private int pdfImageQuality = 100;

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the background color for the ChartBox element.
		/// </summary>
		[Category("Appearance"), DefaultValue(typeof(System.Drawing.Color),"White")]
		public Color BackgroundColor
		{
			get {return mBackgroundColor;}
			set {mBackgroundColor = value;}
		}

		/// <summary>
		/// Gets or sets the border color for the ChartBox.
		/// </summary>
		/// <remarks>This property sets the border color of the ChartBox object. This can be any color
		/// from the System.Drawing.Color structure.
		/// </remarks>
		[Category("Appearance"), DefaultValue(typeof(System.Drawing.Color),"Black")]
		public Color BorderColor
		{
			get {return mBorderColor;}
			set {mBorderColor = value;}
		}


		/// <summary>
		/// Gets/Sets the border width for the ChartBox element.
		/// </summary>
		/// <remarks>
		/// BorderWidth of the ChartBox. If this is set to zero, then the border is invisible.
		/// </remarks>
		[Category("Appearance"), DefaultValue(1)]
		public int BorderWidth
		{
			get {return mBorderWidth;}
			set {mBorderWidth = value;}
		}


		/// <summary>
		/// Gets or sets the font used for any labels displayed on the ChartReport.
		/// </summary>
		[Category("Appearance"), DefaultValue(typeof(System.Drawing.Font),"Tahoma, 8pt")]
		public Font LabelFont
		{
			get {return mLabelFont;}
			set {mLabelFont = value;}
		}


		/// <summary>
		/// The background color displayed behind the chart image.
		/// </summary>
		/// <remarks>This property sets the background color displayed behind the chart image. This can be any color
		/// from the System.Drawing.Color structure
		/// </remarks>
		[Category("Appearance"),Description("The color for map area within the chart."), DefaultValue(typeof(System.Drawing.Color),"WhiteSmoke")]
		public Color MapAreaColor
		{
			get {return mMapAreaColor;}
			set {mMapAreaColor = value;}
		}


		/// <summary>
		/// Gets or sets the name of the chart element.
		/// </summary>
		/// <remarks>This property is used in code when setting other properties programmaticly.</remarks>
		[Category("Data"), Description("The name of the chart. This property is used in code when setting other properties programmaticly.")]
		public override string Name
		{
			get {return name;}
			set {name = value;}
		}


		/// <summary>
		/// Gets or sets whether to display the ChartBox's legend.
		/// </summary>
		[Category("Appearance"), Description("Show legend in chart."), DefaultValue(false)]
		public bool ShowLegend
		{
			get {return mShowLegend;}
			set {mShowLegend = value;}
		}


		/// <summary>
		/// Gets or sets the chart title; value used at the top of the chart.
		/// </summary>
		[Category("Title"), Editor(typeof(Com.Delta.PrintManager.Engine.Editors.PlainTextEditor), typeof(UITypeEditor)), Description("The Title of the chart."), DefaultValue("Chart Title")]
		public string Title
		{
			get {return mTitle;}
			set {mTitle = value;}
		}


		


		/// <summary>
		/// Gets or sets the font used for the title of the ChartBox element.
		/// </summary>
		[Category("Title"), Description("Title font"), DefaultValue(typeof(System.Drawing.Font),"Tahoma, 8pt, style=Bold")]
		public Font TitleFont
		{
			get {return mTitleFont;}
			set {mTitleFont = value;}
		}

		/// <summary>
		/// Gets or sets the PDF export quality for the element.
		/// </summary>
		/// <remarks>This property sets the PDF export quality of the object.
		/// </remarks>
		[Category("Pdf"), DefaultValue(100), Description("Display quality when exporting to PDF format (0-100).")]
		public int ExportQuality
		{
			get {return pdfImageQuality;}
			set {pdfImageQuality = Math.Min(100, Math.Max(0,value));}
		}


		/// <summary>
		/// Gets or sets the type of the ChartBox.
		/// </summary>
		[Category("Data"), Description("The type of data presentation."), DefaultValue(ChartType.Bar)]
		public ChartType Type
		{
			get {return mChartType;}
			set {mChartType = value;}
		}


		/// <summary>
		/// XCategories is a string array of the categories defined in the chart data.
		/// </summary>
		[Category("Data")]
		public string[] Categories
		{
			get {return mXCategories;}
			set 
			{

				if (mXCategories.Length <= value.Length)
				{
					mXCategories = value;
				}
				else
				{
					mXCategories = value;
					ArrayList seriesValues = new ArrayList();

					for (int i=0;i<mYSeries.Count;i++)
					{

						double[] oldValues = this.mYSeries[i] as double[];
						double[] newValues = new double[mXCategories.Length];
					
						for (int j=0;j<this.mXCategories.Length;j++)
						{
							try
							{
								newValues[j] = oldValues[j];
							}
							catch (Exception) {newValues[j]=0;}
						}

						seriesValues.Add(newValues);
					}

					mYSeries = seriesValues;

				}
			}
		}


		/// <summary>
		/// Gets or sets the value used for labelling the X-axis of the ChartBox.
		/// </summary>
		[Category("Data"), Description("The X-axis label.")]
		public string XLabel
		{
			get {return mXLabel;}
			set {mXLabel = value;}
		}


		/// <summary>
		/// Gets the names of containing series.
		/// </summary>
		[Browsable(false)]
		public string[] SeriesNames
		{
			get {return this.mSeriesNames;}
		}

		/// <summary>
		/// Gets the colors of the series.
		/// </summary>
		[Browsable(false)]
		public Color[] SeriesColors
		{
			get {return this.mSeriesColors;}
		}

		/// <summary>
		/// Gets the series data.
		/// </summary>
		[Browsable(false)]
		public double[][] SeriesData
		{
			get 
			{
				double[][] d = new double[this.mYSeries.Count][];
				for (int i=0;i<mYSeries.Count;i++)
					d[i] = (double[])mYSeries[i];
				return d;
			}
		}


		/// <summary>
		/// Gets or sets display data for chart object.
		/// </summary>
		[Editor(typeof(Com.Delta.PrintManager.Engine.Editors.ChartDataEditor), typeof(UITypeEditor)), Category("Data"), Description("Chart data.")]		
		public object ChartData
		{
			get 
			{
				DataTable table = new DataTable();
				table.Columns.Add("Series", typeof(System.String));
				table.Columns.Add("Color", typeof(System.Drawing.Color));
				
				for (int i=0;i<this.mXCategories.Length;i++)
					table.Columns.Add(String.Format("Cat {0} : {1}", i+1 , mXCategories[i]), typeof(System.Double));

				

				for (int i=0;i<this.mSeriesNames.Length;i++)
				{
					object[] d = new object[mXCategories.Length+2];
					d[0] = mSeriesNames[i];
					d[1] = mSeriesColors[i];

					for (int j=2;j<2+this.mXCategories.Length;j++)
					{
						try
						{
							d[j] = ((double[])mYSeries[i])[j-2];
						}
						catch (Exception){d[j] = 0;}
					}

					
					table.Rows.Add(d);
				}

				return table;
			}

			set 
			{
				DataTable table = (DataTable)value;
				ArrayList series = new ArrayList();
				ArrayList seriesColors = new ArrayList();
				mYSeries.Clear();



				for (int i=0;i<table.Rows.Count;i++)
				{
					series.Add((string)table.Rows[i][0]);
					seriesColors.Add(table.Rows[i][1]);

					double[] r = new double[this.mXCategories.Length];
					
					for (int j=2;j<2+this.mXCategories.Length;j++)
					{
						try
						{
							object tmp = table.Rows[i][j];
							r[j-2] = tmp is DBNull ? 0 : (double)tmp;
						}
						catch (Exception) {r[j-2]=0;}
					}

					mYSeries.Add(r);
					

				}

				mSeriesNames = (string[])series.ToArray(typeof(string));
				mSeriesColors = (Color[])seriesColors.ToArray(typeof(System.Drawing.Color));
								
			}
		}


		#endregion

		#region Public Overrides

		/// <summary>
		/// Gets the string representation of report element.
		/// </summary>
		public override string ToString()
		{
			return "Chart [" + this.Name + "]" ;
		}

		/// <summary>
		/// Clones the structure of the ChartBox element, including all properties.
		/// </summary>
		/// <returns><see cref="Com.Delta.PrintManager.Engine.ChartBox">Com.Delta.PrintManager.Engine.ChartBox</see></returns>
		public override object Clone()
		{
			ChartBox tmp = new ChartBox(0,0,0,0,section);
			tmp.X = this.X;
			tmp.Y = this.Y;
			tmp.Width = this.Width;
			tmp.Height = this.Height;
			tmp.Layout = this.Layout;
			tmp.BorderWidth = this.BorderWidth;
			tmp.BorderColor = this.BorderColor;
			tmp.Type = this.Type;
			tmp.mTitleFont = this.mTitleFont;
			tmp.mTitle = this.mTitle;
			tmp.ShowLegend = this.ShowLegend;
			tmp.MapAreaColor = this.MapAreaColor;
			tmp.BackgroundColor = this.BackgroundColor;
			tmp.mLabelFont = this.mLabelFont;
			tmp.XLabel = this.XLabel;
			tmp.ExportQuality = this.ExportQuality;

			tmp.Categories = new string[this.Categories.Length];
			for (int i=0;i<this.Categories.Length;i++)
				tmp.Categories[i] = this.Categories[i];

			tmp.mSeriesNames = new string[this.mSeriesNames.Length];
			for (int i=0;i<this.mSeriesNames.Length;i++)
				tmp.mSeriesNames[i] = this.mSeriesNames[i];

			tmp.mSeriesColors = new Color[this.mSeriesColors.Length];
			for (int i=0;i<this.mSeriesColors.Length;i++)
				tmp.mSeriesColors[i] = this.mSeriesColors[i];
 

			tmp.mYSeries = new ArrayList();
			for (int i=0;i<this.mYSeries.Count;i++)
			{
				double[] t = this.mYSeries[i] as double[];
				
				double[] duplicate = new double[t.Length];
				for (int j=0;j<t.Length;j++)
					duplicate[j] = t[j];

				tmp.mYSeries.Add(duplicate);
			}

			tmp.Name = "chart" + tmp.GetHashCode().ToString();

			return tmp;
		}


		/// <summary>
		/// Paints the ChartBox element.
		/// </summary>
		/// <param name="g">The Graphics object to draw</param>
		/// <remarks>Causes the ChartBox to be painted to the screen.</remarks>
		public override void Paint(Graphics g)
		{			
			Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;

			float sizeInPixels = this.PointsToPixels(mLabelFont.SizeInPoints);
			labelFont = new Font(mLabelFont.FontFamily, sizeInPixels*g.PageScale, mLabelFont.Style, GraphicsUnit.Pixel);
			sizeInPixels = this.PointsToPixels(mTitleFont.SizeInPoints);
			titleFont = new Font(mTitleFont.FontFamily, sizeInPixels*g.PageScale, mTitleFont.Style, GraphicsUnit.Pixel);

			if ( mChartType == ChartType.Bar)
				calculateBarsMapArea(g);
			else if ( mChartType == ChartType.Pie)
				this.calculatePieMapArea(g);
			else if ( mChartType == ChartType.Pie3D)
				this.calculatePie3DMapArea(g);
			else if ( mChartType == ChartType.Bar3D)
				calculateBarsMapArea(g);

			if (mBackgroundColor != Color.Transparent)
			{
				Rectangle theMapArea = this.mMapArea;

				if (paintTransparent)
				{
					g.ExcludeClip(theMapArea);
					g.FillRectangle(new SolidBrush(ShiftColor(mBackgroundColor)), paintArea);
					g.ResetClip();
				}
				else
				{
					g.ExcludeClip(theMapArea);
					g.FillRectangle(new SolidBrush(ShiftColor(mBackgroundColor)), paintArea);
					g.ResetClip();
				}
			}

			if ( mChartType == ChartType.Bar)
				drawBarsDiagram(g);
			else if ( mChartType == ChartType.Pie)
				drawPieDiagram(g);
			else if ( mChartType == ChartType.Pie3D)
				drawPie3DDiagram(g);
			else if ( mChartType == ChartType.Bar3D)
				drawBars3DDiagram(g);
			
			if ( mBorderWidth > 0 )
			{							
				Pen borderPen = new Pen(mBorderColor, mBorderWidth);
				borderPen.Alignment = PenAlignment.Inset;
				g.DrawRectangle(borderPen, paintArea);
				borderPen.Dispose();
			}
			
		}
	

		/// <summary>
		/// Chart image for printing and exporting purpose
		/// </summary>
		[Browsable(false)]
		public Bitmap Image
		{
			get 
			{
				Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;

				Bitmap chartImage = new Bitmap(paintArea.Width + (mBorderWidth==1? 1 : 0), paintArea.Height + (mBorderWidth==1? 1 : 0));

				Graphics imageGraphics = Graphics.FromImage(chartImage);
				
				Matrix m = new Matrix();
				m.Translate(-paintArea.X, -paintArea.Y, MatrixOrder.Append);
				imageGraphics.Transform = m;

				this.Paint(imageGraphics);				
				imageGraphics.Dispose();

				return chartImage;
			}
		}
		
		#endregion

		#region Public Functions

		/// <summary>
		/// A serie is used to populate the ChartBox.
		/// </summary>
		/// <param name="serieName">Name of the serie. Displayed in the Legend</param>
		/// <param name="values">A array of Double values</param>
		/// <param name="serieColor">Color of the bar/pie</param>
		/// <remarks>
		/// You can add multiple serie to a chart. This would be useful for comparing values over certain years.
		/// Please see the "Chart Report" example, especially the City Population chart
		/// C# Sample
		/// <code language="c#">
		/// daPrintDocument.AddChartSerie("chart0","Year 1975.",new double[3]{15.9,11.4,11.2},Color.DarkGreen);
		/// </code>
		/// VB.Net Sample
		/// <code language="Visual Basic">
		/// daPrintDocument.AddChartSerie("chart0", "Year 1975.", New Double() {15.9, 11.4, 11.2}, Color.DarkGreen)
		/// </code>
		/// </remarks>
		public void AddSerie(string serieName, double[] values, Color serieColor)
		{
			mYSeries.Add(values);

			Color[] tmp = new Color[mSeriesColors.Length+1];
			Array.Copy(mSeriesColors,0,tmp,0,mSeriesColors.Length);
			tmp[mSeriesColors.Length] = serieColor;
			mSeriesColors = tmp;

			string[] tmp1 = new string[mSeriesNames.Length+1];
			Array.Copy(mSeriesNames,0,tmp1,0,mSeriesNames.Length);
			tmp1[mSeriesNames.Length] = serieName;
			mSeriesNames = tmp1;

		}

		/// <summary>
		/// Adds data serie to the set of series at the specified index
		/// </summary>
		public void AddSerie(string serieName, double[] values, Color serieColor, int index)
		{
			mYSeries.Insert(index, values);

			ArrayList tmp = new ArrayList(mSeriesColors);
			tmp.Insert(index, serieColor);
			mSeriesColors = (Color[])tmp.ToArray(typeof(System.Drawing.Color));

			tmp = new ArrayList(mSeriesNames);
			tmp.Insert(index, serieName);
			mSeriesNames = (string[])tmp.ToArray(typeof(System.String));
		}

		/// <summary>
		/// Clears all category and seri data from the chart.
		/// </summary>
		public void Clear()
		{
			mSeriesNames = new string[0];
			mSeriesColors = new Color[0];
			mPieColors = new Color[0];
			mYSeries.Clear();
			mXCategories = new string[0];
		}

		
		#endregion

		#region Private Functions



		
		private void calculateBarsMapArea(Graphics g)
		{			
			Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;
			SizeF sf = g.MeasureString(mTitle, titleFont);
			SizeF sfLabel = g.MeasureString("Text", labelFont);

			float bottomTextHeight = 0;

			if (mXLabel != string.Empty)
				bottomTextHeight += sfLabel.Height;

			if (this.mXCategories.Length>0)
				bottomTextHeight += sfLabel.Height;

			if (mShowLegend)
			{
				int originX = (int) (paintArea.X+paintArea.Width*0.08);
				int originY = (int) (paintArea.Y+sf.Height+10);
				int width = (int) (paintArea.Width*0.68);
				int height = (int) (paintArea.Height-sf.Height-10-bottomTextHeight-9);

				mMapArea = new Rectangle(originX,originY,width,height);
			
				originX = (int) (paintArea.X+paintArea.Width*0.78);
				originY = (int) (paintArea.Y+sf.Height+10);
				width = (int) (paintArea.Width*0.20);
				height = (int) (paintArea.Height-sf.Height- 10 - bottomTextHeight -9);

				mLegendArea = new Rectangle(originX,originY,width,height);
			}
			else
			{
				int originX = (int) (paintArea.X+theRegion.Width*0.1);
				int originY = (int) (paintArea.Y+sf.Height+10);
				int width = (int) (paintArea.Width*0.8);
				int height = (int) (paintArea.Height-sf.Height-10-bottomTextHeight-9);

				mMapArea = new Rectangle(originX,originY,width,height);
			
			}
		}

		
		private void calculatePieMapArea(Graphics g)
		{			
			Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;
			SizeF sf = g.MeasureString(mTitle,titleFont);
			SizeF sfLabel = g.MeasureString(mXLabel,labelFont);

			if (mShowLegend)
			{
				int originX = (int) (paintArea.X + theRegion.Width*0.02);
				int originY = (int) (paintArea.Y + sf.Height + 12);
				int width = (int) (paintArea.Width*0.72);
				int height = (int) (paintArea.Height-sf.Height-20);

				mMapArea = new Rectangle(originX,originY,width,height);
			
				originX = (int) (paintArea.X+theRegion.Width*0.76);
				originY = (int) (paintArea.Y+sf.Height+12);
				width = (int) (paintArea.Width*0.22);
				height = (int) (paintArea.Height-sf.Height-20);

				mLegendArea = new Rectangle(originX,originY,width,height);
			}
			else
			{
				int originX = (int) (paintArea.X+theRegion.Width*0.02);
				int originY = (int) (paintArea.Y+sf.Height+12);
				int width = (int) (paintArea.Width*0.96);
				int height = (int) (paintArea.Height-sf.Height-20);

				mMapArea = new Rectangle(originX,originY,width,height);
			
			}
		}

		private void calculatePie3DMapArea(Graphics g)
		{			
			Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;
			SizeF sf = g.MeasureString(mTitle, titleFont);
			SizeF sfLabel = g.MeasureString(mXLabel, labelFont);

			if (mShowLegend)
			{
				int originX = (int) (paintArea.X + theRegion.Width*0.03);
				int originY = (int) (paintArea.Y + sf.Height + 16);
				int width = (int) (paintArea.Width*0.70);
				int height = (int) (paintArea.Height - sf.Height - 24);

				mMapArea = new Rectangle(originX,originY,width,height);
			
				originX = (int) (paintArea.X+theRegion.Width*0.76);
				originY = (int) (paintArea.Y+sf.Height+12);
				width = (int) (paintArea.Width*0.22);
				height = (int) (paintArea.Height-sf.Height-20);

				mLegendArea = new Rectangle(originX,originY,width,height);
			}
			else
			{
				int originX = (int) (paintArea.X + theRegion.Width*0.03);
				int originY = (int) (paintArea.Y + sf.Height + 16);
				int width = (int) (paintArea.Width*0.94);
				int height = (int) (paintArea.Height - sf.Height - 24);

				mMapArea = new Rectangle(originX, originY, width, height);
			
			}
		}

		
		private void drawBars(Graphics g)
		{
			float overlappingFactor = 0.2f;
			float fillSliceFactor = 0.9f;

			int theNumberOfCategories = mXCategories.Length;
			if (theNumberOfCategories==0) return;

			int theNumberOfSeries = mYSeries.Count;

			float sliceWidth = (float)mMapArea.Width/theNumberOfCategories;

			float recWidth = fillSliceFactor*sliceWidth/(theNumberOfSeries*(1-overlappingFactor)+overlappingFactor);
			float recOffset = recWidth*(1-overlappingFactor);
			
			
			float zeroPosition = calculateZeroPosition();


			for (int i=0;i<mYSeries.Count;i++)
			{
				double[] theValues = (double[]) mYSeries[i];
	
				for (int j=0;j<theNumberOfCategories;j++)
				{
					double theValue = 0;
					if (j<theValues.Length)
						theValue = theValues[j];
					
					
					float recHeight = calculateBarHeight(theValue);
					
					if (theValue>=0)
					{
						g.FillRectangle(new SolidBrush(mSeriesColors[i]),mMapArea.X+j*sliceWidth+sliceWidth*(1-fillSliceFactor)/2+i*recOffset, zeroPosition-recHeight, recWidth, recHeight);
						g.DrawRectangle(new Pen(Color.Black,1),mMapArea.X+j*sliceWidth+sliceWidth*(1-fillSliceFactor)/2+i*recOffset, zeroPosition-recHeight, recWidth, recHeight);
					}
					else
					{
						g.FillRectangle(new SolidBrush(mSeriesColors[i]), mMapArea.X+j*sliceWidth+sliceWidth*(1-fillSliceFactor)/2+i*recOffset, zeroPosition, recWidth, recHeight);
						g.DrawRectangle(new Pen(Color.Black,1), mMapArea.X+j*sliceWidth+sliceWidth*(1-fillSliceFactor)/2+i*recOffset, zeroPosition, recWidth, recHeight);
					}

				}
			}
		}

		private void drawBars3D(Graphics g)
		{

			int xOffset = (int)(depth * Math.Cos(angle));
			int yOffset = (int)(depth * Math.Sin(angle));

			float overlappingFactor = 0.0f;
			float fillSliceFactor = 0.75f;

			int theNumberOfCategories = mXCategories.Length;
			if (theNumberOfCategories==0) return;

			int theNumberOfSeries = mYSeries.Count;

			float sliceWidth = (float)(mMapArea.Width - xOffset)/theNumberOfCategories;

			float recWidth = fillSliceFactor*sliceWidth/(theNumberOfSeries);
			float recOffset = recWidth;
			
			
			float zeroPosition = mMapArea.Top + yOffset + (mMapArea.Height-yOffset)*mMaxY/(mMaxY-mMinY);

			for (int j=0;j<theNumberOfCategories;j++)
			{
				for (int i=0;i<mYSeries.Count;i++)
				{
					double[] theValues = (double[]) mYSeries[i];
	
				
					double theValue = 0;
					if (j<theValues.Length)
						theValue = theValues[j];
					
					
					float recHeight = calculateBarHeight(theValue);
					
					if (theValue>=0)
					{
						g.FillRectangle(new SolidBrush(mSeriesColors[i]),mMapArea.X+j*sliceWidth+sliceWidth*(1-fillSliceFactor)/2+i*recOffset, zeroPosition-recHeight, recWidth, recHeight);
						g.DrawRectangle(new Pen(DarkenColor(mSeriesColors[i], 0.6)), mMapArea.X+j*sliceWidth+sliceWidth*(1-fillSliceFactor)/2+i*recOffset, zeroPosition-recHeight, recWidth, recHeight);

						GraphicsPath path = new GraphicsPath();
						path.StartFigure();
						path.AddLine(mMapArea.X+j*sliceWidth+sliceWidth*(1-fillSliceFactor)/2+i*recOffset , zeroPosition-recHeight, mMapArea.X+j*sliceWidth+sliceWidth*(1-fillSliceFactor)/2+i*recOffset+xOffset/2, zeroPosition-recHeight-yOffset/2);
						path.AddLine(mMapArea.X+j*sliceWidth+sliceWidth*(1-fillSliceFactor)/2+i*recOffset+xOffset/2, zeroPosition-recHeight-yOffset/2, mMapArea.X+j*sliceWidth+sliceWidth*(1-fillSliceFactor)/2+i*recOffset+xOffset/2+recWidth, zeroPosition-recHeight-yOffset/2);
						path.AddLine(mMapArea.X+j*sliceWidth+sliceWidth*(1-fillSliceFactor)/2+i*recOffset+xOffset/2+recWidth, zeroPosition-recHeight-yOffset/2, mMapArea.X+j*sliceWidth+sliceWidth*(1-fillSliceFactor)/2+i*recOffset+recWidth, zeroPosition-recHeight);
						path.CloseFigure();

						g.FillPath(new SolidBrush(Color.FromArgb(mSeriesColors[i].R, mSeriesColors[i].G, mSeriesColors[i].B)), path);						
						g.DrawPath(new Pen(DarkenColor(mSeriesColors[i], 0.6)), path);


						path = new GraphicsPath();
						path.StartFigure();
						path.AddLine(mMapArea.X+j*sliceWidth+sliceWidth*(1-fillSliceFactor)/2+i*recOffset+xOffset/2+recWidth, zeroPosition-recHeight-yOffset/2, mMapArea.X+j*sliceWidth+sliceWidth*(1-fillSliceFactor)/2+i*recOffset+recWidth, zeroPosition-recHeight);
						path.AddLine(mMapArea.X+j*sliceWidth+sliceWidth*(1-fillSliceFactor)/2+i*recOffset+recWidth, zeroPosition-recHeight, mMapArea.X+j*sliceWidth+sliceWidth*(1-fillSliceFactor)/2+i*recOffset+recWidth, zeroPosition);
						path.AddLine(mMapArea.X+j*sliceWidth+sliceWidth*(1-fillSliceFactor)/2+i*recOffset+recWidth, zeroPosition, mMapArea.X+j*sliceWidth+sliceWidth*(1-fillSliceFactor)/2+i*recOffset+recWidth+xOffset/2, zeroPosition-yOffset/2);
						path.CloseFigure();

						g.FillPath(new SolidBrush(DarkenColor(mSeriesColors[i], 0.8)), path);						
						g.DrawPath(new Pen(DarkenColor(mSeriesColors[i], 0.6)), path);
					}
					else
					{
						g.FillRectangle(new SolidBrush(mSeriesColors[i]),mMapArea.X+j*sliceWidth+sliceWidth*(1-fillSliceFactor)/2+i*recOffset, zeroPosition, recWidth, recHeight);
						g.DrawRectangle(new Pen(DarkenColor(mSeriesColors[i], 0.6)), mMapArea.X+j*sliceWidth+sliceWidth*(1-fillSliceFactor)/2+i*recOffset, zeroPosition, recWidth, recHeight);

						GraphicsPath path = new GraphicsPath();
						path.StartFigure();
						path.AddLine(mMapArea.X+j*sliceWidth+sliceWidth*(1-fillSliceFactor)/2+i*recOffset , zeroPosition, mMapArea.X+j*sliceWidth+sliceWidth*(1-fillSliceFactor)/2+i*recOffset+xOffset/2, zeroPosition-yOffset/2);
						path.AddLine(mMapArea.X+j*sliceWidth+sliceWidth*(1-fillSliceFactor)/2+i*recOffset+xOffset/2, zeroPosition-yOffset/2, mMapArea.X+j*sliceWidth+sliceWidth*(1-fillSliceFactor)/2+i*recOffset+xOffset/2+recWidth, zeroPosition-yOffset/2);
						path.AddLine(mMapArea.X+j*sliceWidth+sliceWidth*(1-fillSliceFactor)/2+i*recOffset+xOffset/2+recWidth, zeroPosition-yOffset/2, mMapArea.X+j*sliceWidth+sliceWidth*(1-fillSliceFactor)/2+i*recOffset+recWidth, zeroPosition);
						path.CloseFigure();

						g.FillPath(new SolidBrush(Color.FromArgb(mSeriesColors[i].R, mSeriesColors[i].G, mSeriesColors[i].B)), path);						
						g.DrawPath(new Pen(DarkenColor(mSeriesColors[i], 0.6)), path);


						path = new GraphicsPath();
						path.StartFigure();
						path.AddLine(mMapArea.X+j*sliceWidth+sliceWidth*(1-fillSliceFactor)/2+i*recOffset+xOffset/2+recWidth, zeroPosition-yOffset/2, mMapArea.X+j*sliceWidth+sliceWidth*(1-fillSliceFactor)/2+i*recOffset+recWidth, zeroPosition);
						path.AddLine(mMapArea.X+j*sliceWidth+sliceWidth*(1-fillSliceFactor)/2+i*recOffset+recWidth, zeroPosition, mMapArea.X+j*sliceWidth+sliceWidth*(1-fillSliceFactor)/2+i*recOffset+recWidth, zeroPosition+recHeight);
						path.AddLine(mMapArea.X+j*sliceWidth+sliceWidth*(1-fillSliceFactor)/2+i*recOffset+recWidth, zeroPosition+recHeight, mMapArea.X+j*sliceWidth+sliceWidth*(1-fillSliceFactor)/2+i*recOffset+recWidth+xOffset/2, zeroPosition+recHeight-yOffset/2);
						path.CloseFigure();

						g.FillPath(new SolidBrush(DarkenColor(mSeriesColors[i], 0.8)), path);						
						g.DrawPath(new Pen(DarkenColor(mSeriesColors[i], 0.6)), path);
					}

				
				}
			}
		}


		private void drawBarsDiagram(Graphics g)
		{
			drawTitle(g);
			drawBarsMapArea(g);
			drawLabels(g);
			drawBars(g);
			if (mShowLegend)
				drawBarsLegend(g);
		}

		private void drawBars3DDiagram(Graphics g)
		{
			drawTitle(g);

			drawBars3DMapArea(g);

			drawBar3DLabels(g);

			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			drawBars3D(g);
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;

			if (mShowLegend)
				drawBarsLegend(g);
		}

		
		private void drawBarsLegend(Graphics g)
		{						
			g.DrawRectangle(new Pen(Color.Black,1),new Rectangle(mLegendArea.X, mLegendArea.Y, mLegendArea.Width, mLegendArea.Height));
			
			for (int i=0;i<mYSeries.Count;i++)
			{
				SizeF sf = g.MeasureString(mSeriesNames[i], labelFont);
				int yPos = (int)(mLegendArea.Y + 8 + i*sf.Height);
				g.FillRectangle(new SolidBrush(mSeriesColors[i]), mLegendArea.X + 4, yPos-3 , 6, 6);
				g.DrawString(mSeriesNames[i], labelFont, new SolidBrush(Color.Black), mLegendArea.X + 11, yPos-sf.Height/2);	
			}
		}

		
		private void drawBarsMapArea(Graphics g)
		{
			Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;

			if (mMapAreaColor != Color.Transparent)
			{
				if (paintTransparent)
					g.FillRectangle(new SolidBrush(ShiftColor(mMapAreaColor,192)),new Rectangle(mMapArea.X, mMapArea.Y, mMapArea.Width, mMapArea.Height));
				else
					g.FillRectangle(new SolidBrush(mMapAreaColor),new Rectangle(mMapArea.X, mMapArea.Y, mMapArea.Width, mMapArea.Height));
			}

			g.DrawRectangle(new Pen(Color.Black,1),new Rectangle(mMapArea.X, mMapArea.Y, mMapArea.Width, mMapArea.Height));

			int theNumberOfCategories = mXCategories.Length;
			if (theNumberOfCategories==0) return;

			float sliceWidth = (float)mMapArea.Width/theNumberOfCategories;

			

			for (int i=0;i<theNumberOfCategories;i++)
			{
				SizeF sf = g.MeasureString(mXCategories[i], labelFont);

				float bottomTextHeight = sf.Height;

				if (mXLabel != string.Empty)
					bottomTextHeight += sf.Height;

				g.DrawString(mXCategories[i], labelFont, new SolidBrush(Color.Black), mMapArea.X + i*sliceWidth+sliceWidth/2-sf.Width/2, paintArea.Bottom - bottomTextHeight - 6);

				if (i!= theNumberOfCategories-1)
					g.DrawLine(new Pen(Color.Black,1), mMapArea.X + (i+1)*sliceWidth, mMapArea.Top, mMapArea.X+(i+1)*sliceWidth, mMapArea.Bottom);
			}
		}

		private void drawBars3DMapArea(Graphics g)
		{
			Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;

			int xOffset = (int)(depth * Math.Cos(angle));
			int yOffset = (int)(depth * Math.Sin(angle));

			
			GraphicsPath path = new GraphicsPath();
			path.StartFigure();
			path.AddLine(mMapArea.Left, mMapArea.Top, mMapArea.Left + xOffset, mMapArea.Top);
			path.AddLine(mMapArea.Left + xOffset, mMapArea.Top, mMapArea.Left, mMapArea.Top + yOffset);
			path.CloseFigure();

			if (this.BackgroundColor != Color.Transparent)
			{
				g.FillPath(new SolidBrush(this.BackgroundColor), path);
			}

			path.StartFigure();
			path.AddLine(mMapArea.Right - xOffset, mMapArea.Bottom, mMapArea.Right, mMapArea.Bottom);
			path.AddLine(mMapArea.Right, mMapArea.Bottom, mMapArea.Right, mMapArea.Bottom - yOffset);
			path.CloseFigure();

			if (this.BackgroundColor != Color.Transparent)
			{
				g.FillPath(new SolidBrush(this.BackgroundColor), path);
			}



			path = new GraphicsPath();
			path.StartFigure();
			path.AddLine(mMapArea.Left, mMapArea.Bottom, mMapArea.Right - xOffset, mMapArea.Bottom);
			path.AddLine(mMapArea.Right - xOffset, mMapArea.Bottom, mMapArea.Right, mMapArea.Bottom - yOffset);
			path.AddLine(mMapArea.Right, mMapArea.Bottom - yOffset, mMapArea.Left + xOffset, mMapArea.Bottom - yOffset);
			path.CloseFigure();

			
			if (mMapAreaColor != Color.Transparent)
			{
				if (paintTransparent)
					g.FillPath(new SolidBrush(ShiftColor(mMapAreaColor,192)), path);
				else
					g.FillPath(new SolidBrush(mMapAreaColor), path);
			}
			g.DrawPath(Pens.Black, path);

			path = new GraphicsPath();
			path.AddLine(mMapArea.Left, mMapArea.Bottom, mMapArea.Left, mMapArea.Top + yOffset);
			path.AddLine(mMapArea.Left, mMapArea.Top + yOffset, mMapArea.Left + xOffset, mMapArea.Top);
			path.AddLine(mMapArea.Left + xOffset, mMapArea.Top, mMapArea.Left + xOffset, mMapArea.Bottom - yOffset);
			path.CloseFigure();

			
			if (mMapAreaColor != Color.Transparent)
			{
				if (paintTransparent)
					g.FillPath(new SolidBrush(ShiftColor(mMapAreaColor,192)), path);
				else
					g.FillPath(new SolidBrush(mMapAreaColor), path);
			}
			g.DrawPath(Pens.Black, path);

			path = new GraphicsPath();
			path.AddLine(mMapArea.Left + xOffset, mMapArea.Bottom - yOffset, mMapArea.Right, mMapArea.Bottom - yOffset);
			path.AddLine(mMapArea.Right, mMapArea.Bottom - yOffset, mMapArea.Right, mMapArea.Top);
			path.AddLine(mMapArea.Right, mMapArea.Top, mMapArea.Left + xOffset, mMapArea.Top);
			path.CloseFigure();

			if (mMapAreaColor != Color.Transparent)
			{
				if (paintTransparent)
					g.FillPath(new SolidBrush(ShiftColor(mMapAreaColor,192)), path);
				else
					g.FillPath(new SolidBrush(mMapAreaColor), path);
			}
			g.DrawPath(Pens.Black, path);
			



			int theNumberOfCategories = mXCategories.Length;
			if (theNumberOfCategories==0) return;

			float sliceWidth = (float)(mMapArea.Width - xOffset)/theNumberOfCategories;

			

			for (int i=0;i<theNumberOfCategories;i++)
			{
				SizeF sf = g.MeasureString(mXCategories[i], labelFont);

				float bottomTextHeight = sf.Height;

				if (mXLabel != string.Empty)
					bottomTextHeight += sf.Height;

				g.DrawString(mXCategories[i], labelFont, new SolidBrush(Color.Black), mMapArea.X + i*sliceWidth+sliceWidth/2-sf.Width/2, paintArea.Bottom - bottomTextHeight - 6);

				if (i!= theNumberOfCategories-1)
				{
					g.DrawLine(Pens.DarkGray, mMapArea.X +(i+1)*sliceWidth, mMapArea.Bottom, mMapArea.X + xOffset +(i+1)*sliceWidth, mMapArea.Bottom - yOffset);
					g.DrawLine(Pens.DarkGray, mMapArea.X + xOffset +(i+1)*sliceWidth, mMapArea.Top, mMapArea.X + xOffset +(i+1)*sliceWidth, mMapArea.Bottom - yOffset);
				}
			}
		}


		private void drawLabels(Graphics g)
		{
			Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;
			resolveRange();			

			SizeF sf = g.MeasureString(mXLabel, labelFont);
			g.DrawString(mXLabel, labelFont, new SolidBrush(Color.Black), mMapArea.X + mMapArea.Width/2-sf.Width/2, paintArea.Bottom - sf.Height - 3);

			float step = Math.Min(Math.Abs(mMinY),Math.Abs(mMaxY))/2f;

			if (mMaxY!=0 && mMinY!=0)
				step = Math.Min(Math.Abs(mMinY),Math.Abs(mMaxY))/2f;
			else if (mMaxY!=0 && mMinY==0)
				step = Math.Abs(mMaxY)/4f;
			else if (mMaxY==0 && mMinY!=0)
				step = Math.Abs(mMinY)/4f;
			else
				return;

			for ( float i=mMinY;i<=mMaxY;i+=step)
			{
				string numLabel = i.ToString("#,##0.#");
				sf = g.MeasureString(numLabel, labelFont);
				float theYPosition = calculatePosition(i);
				g.DrawString(numLabel, labelFont, new SolidBrush(Color.Black), mMapArea.Left - 2 - sf.Width, theYPosition - sf.Height/2);
				if (i!=mMinY && i!=mMaxY)
					g.DrawLine(new Pen(Color.DarkGray,1),mMapArea.Left, theYPosition, mMapArea.Right, theYPosition);
			}
			
		}

		private void drawBar3DLabels(Graphics g)
		{
			Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;
			resolveRange();

			int xOffset = (int)(depth * Math.Cos(angle));
			int yOffset = (int)(depth * Math.Sin(angle));

			SizeF sf = g.MeasureString(mXLabel, labelFont);
			g.DrawString(mXLabel, labelFont, new SolidBrush(Color.Black), mMapArea.X + mMapArea.Width/2-sf.Width/2, paintArea.Bottom - sf.Height - 3);

			float step = Math.Min(Math.Abs(mMinY),Math.Abs(mMaxY))/2f;

			if (mMaxY!=0 && mMinY!=0)
				step = Math.Min(Math.Abs(mMinY),Math.Abs(mMaxY))/2f;
			else if (mMaxY!=0 && mMinY==0)
				step = Math.Abs(mMaxY)/4f;
			else if (mMaxY==0 && mMinY!=0)
				step = Math.Abs(mMinY)/4f;
			else
				return;

			for ( float i=mMinY;i<=mMaxY;i+=step)
			{
				string numLabel = i.ToString("#,##0.#");
				sf = g.MeasureString(numLabel, labelFont);
				float theYPosition = mMapArea.Top + yOffset + (mMapArea.Height-yOffset)*mMaxY/(mMaxY-mMinY) - (mMapArea.Height-yOffset)*i/(mMaxY-mMinY);
				g.DrawString(numLabel, labelFont, new SolidBrush(Color.Black), mMapArea.Left - 2 - sf.Width, theYPosition - sf.Height/2);
				if (i!=mMinY && i!=mMaxY)
				{
					g.DrawLine(new Pen(Color.DarkGray,1), mMapArea.Left, theYPosition, mMapArea.Left + xOffset, theYPosition - yOffset);
					g.DrawLine(new Pen(Color.DarkGray,1), mMapArea.Left + xOffset, theYPosition - yOffset, mMapArea.Right, theYPosition - yOffset);
				}
			}
			
		}

		
		private void drawPie(Graphics g)
		{
			if (mYSeries.Count == 0) return;
			
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

			int theNumberOfCategories = mXCategories.Length;
			double[] theValues = mYSeries[0] as double[];

			double theSum = 0;
			for (int i=0;i<theNumberOfCategories;i++)
			{
				double x = 0;
				if (i<theValues.Length)
					x = Math.Abs(theValues[i]);

				theSum += x;			
			}

			if (theSum==0) return;

			int pieRecWidth = (int) (Math.Min(mMapArea.Width,mMapArea.Height)*0.82f);
			Rectangle pieRectangle = new Rectangle(mMapArea.X + (mMapArea.Width-pieRecWidth)/2,mMapArea.Y + (mMapArea.Height-pieRecWidth)/2, pieRecWidth, pieRecWidth);
			

			int colorRedStep = (int)((255-mSeriesColors[0].R)/theNumberOfCategories);
			int colorGreenStep = (int)((255-mSeriesColors[0].G)/theNumberOfCategories);
			int colorBlueStep = (int)((255-mSeriesColors[0].B)/theNumberOfCategories);
			mPieColors = new Color[theNumberOfCategories];

			float currentAngle = 0;
			for (int i=0;i<theNumberOfCategories;i++)
			{
				double x = 0;
				if (i<theValues.Length)
					x = Math.Abs(theValues[i]);

				mPieColors[i] = Color.FromArgb( mSeriesColors[0].R+i*colorRedStep,mSeriesColors[0].G+i*colorGreenStep,mSeriesColors[0].B+i*colorBlueStep);				
				float theAngle = (float) (360 * Math.Abs(x) / theSum) ;
				float percentage = (float) (Math.Abs(x) / theSum) ;

				
				g.FillPie(new SolidBrush(mPieColors[i]), new Rectangle(pieRectangle.X, pieRectangle.Y, pieRectangle.Width, pieRectangle.Height), currentAngle, theAngle);
				g.DrawPie(new Pen(Color.Black,1),new Rectangle(pieRectangle.X, pieRectangle.Y, pieRectangle.Width, pieRectangle.Height),currentAngle,theAngle);

				float labelAngle = currentAngle + theAngle/2;
				int labelRadius = pieRecWidth/2 + 1;
				
				string labelString = Categories[i] + " (" + percentage.ToString("#0.#%") + ")"; 

				float labelX = (float)(pieRectangle.X + pieRectangle.Width/2 + labelRadius*Math.Cos(6.28*labelAngle/360)) ;
				float labelY = (float)(pieRectangle.Y + pieRectangle.Height/2 + labelRadius*Math.Sin(6.28*labelAngle/360)) ;

				SizeF sf = g.MeasureString(labelString,labelFont);

				if (labelX < pieRectangle.X + pieRectangle.Width/2)
					labelX = labelX - sf.Width;
				if (labelY < pieRectangle.Y + pieRectangle.Height/2)
					labelY = labelY - sf.Height;
				
				if (percentage>0.01)
					g.DrawString(labelString, labelFont, new SolidBrush(Color.Black), labelX, labelY);

				currentAngle += theAngle;
			}

			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
		}

		
		private void drawPieDiagram(Graphics g)
		{
			drawTitle(g);
			drawPieMapArea(g);
			drawPie(g);
			if (mShowLegend)
				drawPieLegend(g);
		}


		private void drawPie3DDiagram(Graphics g)
		{
			drawTitle(g);
			drawPieMapArea(g);
			drawPie3D(g);			
			if (mShowLegend)
				drawPieLegend(g);
		}

		private void drawPie3D(Graphics g)
		{
			if (mYSeries.Count == 0) return;

			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

			int theNumberOfCategories = mXCategories.Length;

			double[] doubleValues = mYSeries[0] as double[];
			decimal[] decimalValues = new decimal[doubleValues.Length] ;
	
			for (int i=0;i<decimalValues.Length;i++)
			{
				decimalValues[i] = (decimal) Math.Abs(doubleValues[i]);
			}

			int colorRedStep = (int)((255-mSeriesColors[0].R)/theNumberOfCategories);
			int colorGreenStep = (int)((255-mSeriesColors[0].G)/theNumberOfCategories);
			int colorBlueStep = (int)((255-mSeriesColors[0].B)/theNumberOfCategories);
			mPieColors = new Color[theNumberOfCategories];

			for (int i=0;i<theNumberOfCategories;i++)
			{				
				mPieColors[i] = Color.FromArgb( mSeriesColors[0].R+i*colorRedStep, mSeriesColors[0].G+i*colorGreenStep, mSeriesColors[0].B+i*colorBlueStep);				
			}
			
			
			PieChart3D chart3D = new PieChart3D(new Rectangle(mMapArea.X, mMapArea.Y, mMapArea.Width, mMapArea.Height), decimalValues, 0.15f);
			chart3D.Texts = this.mXCategories;
			chart3D.SliceRelativeDisplacement = 0.1f;
			chart3D.Font = labelFont;
			chart3D.Colors = mPieColors;
			chart3D.InitialAngle = -30f;
			chart3D.ShadowStyle = ShadowStyle.GradualShadow;
			chart3D.EdgeColorType = EdgeColorType.DarkerThanSurface;
			
			chart3D.Draw(g);
			chart3D.PlaceTexts(g);

			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
		}

		private void drawPieLegend(Graphics g)
		{
			g.DrawRectangle(new Pen(Color.Black,1),new Rectangle(mLegendArea.X, mLegendArea.Y, mLegendArea.Width, mLegendArea.Height));
		
			if (Categories.Length==0 || mYSeries.Count == 0) return;
			
			double[] theValues = mYSeries[0] as double[];
			for (int i=0;i<Categories.Length;i++)
			{
				double x = 0;
				if (i<theValues.Length)
					x = Math.Abs(theValues[i]);

				string legendText = Categories[i] + " (" + x.ToString("#0.##") + ")";

				SizeF sf = g.MeasureString(legendText, labelFont);
				int yPos = (int)(mLegendArea.Y + 8 + i*sf.Height);
				g.FillRectangle(new SolidBrush(mPieColors[i]),mLegendArea.X + 4, yPos - 3, 6, 6);				
				g.DrawString(legendText, labelFont, new SolidBrush(Color.Black), mLegendArea.X+11, yPos-sf.Height/2);	
			}
		}

		
		private void drawPieMapArea(Graphics g)
		{
			if (paintTransparent)
				g.FillRectangle(new SolidBrush(ShiftColor(this.mMapAreaColor)),new Rectangle(mMapArea.X, mMapArea.Y, mMapArea.Width, mMapArea.Height));
			else 
				g.FillRectangle(new SolidBrush(this.mMapAreaColor),new Rectangle(mMapArea.X, mMapArea.Y, mMapArea.Width, mMapArea.Height));
		}

		
		private void drawTitle(Graphics g)
		{
			Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;
			SizeF sf = g.MeasureString(mTitle, titleFont);
			g.DrawString(mTitle, titleFont,new SolidBrush(Color.Black), paintArea.X + paintArea.Width/2-sf.Width/2, paintArea.Y + 5);
		}

		private Color DarkenColor(Color c, double ammount)
		{
			return Color.FromArgb((int)(c.R*ammount), (int)(c.G*ammount), (int)(c.B*ammount) );
		}

		
		private void resolveRange()
		{
			double maxValue = 0;
			double minValue = 0;

			for (int i=0;i<mYSeries.Count;i++)
			{
				double[] theValues = (double[])mYSeries[i];

				double[] tmp = new double[theValues.Length];
				Array.Copy(theValues,0,tmp,0,theValues.Length);

				Array.Sort(tmp);
				maxValue = Math.Max(maxValue,tmp[tmp.Length-1]);
				minValue = Math.Min(minValue,tmp[0]);
			}

			if (maxValue==0 && minValue==0)
			{
				maxValue = 100;
				minValue = 0;
			}
			else if (maxValue!=0 && minValue==0)
			{
				int theBase = (int)Math.Ceiling(Math.Log10(maxValue));
				mMaxY = (float)Math.Pow(10,theBase);

				if (maxValue<=mMaxY/2) mMaxY=mMaxY/2;
			}
			else
			{
				int theBase = (int)Math.Max( Math.Ceiling(Math.Log10(Math.Abs(maxValue))),Math.Ceiling(Math.Log10(Math.Abs(minValue))) );
				mMaxY = (float)Math.Pow(10,theBase);
				mMinY = (float)Math.Pow(10,theBase)*Math.Sign(minValue);

				if (maxValue<=mMaxY/2) mMaxY=mMaxY/2;
				if (minValue>=mMinY/2) mMinY=mMinY/2;

			}

		}

		
		#endregion

		#region Private Properties

		private float calculateBarHeight(double theValue)
		{
			double ratio = (double)mMapArea.Height/(mMaxY-mMinY);
			return (float) Math.Abs(theValue*ratio);
		}

		
		private float calculatePosition(float x)
		{
			return calculateZeroPosition() - mMapArea.Height*x/(mMaxY-mMinY);
		}


		private float calculateZeroPosition()
		{
			return (float)(mMapArea.Top+mMapArea.Height*mMaxY/(mMaxY-mMinY));
		}

		
		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the ChartBox class.
		/// </summary>
		/// <param name="x">x-position of the new ChartBox</param>
		/// <param name="y">y-position of the new ChartBox</param>
		/// <param name="width">Width of the new ChartBox</param>
		/// <param name="height">Height of the new ChartBox</param>
		/// <param name="parent">Parent of the new ChartBox</param>
		public ChartBox(int x,int y, int width, int height, Section parent)
		{
			section = parent;
			theRegion = new Rectangle(x,y,width,height);
			this.Bounds = theRegion;
			name = "chart0";
			this.mXCategories = new string[]{"Category1", "Category2"};
			
		}

		internal ChartBox(XmlNode node, Section parent)
		{
			section = parent;
			Init(node);			
		}
		

		#endregion
	}
}
