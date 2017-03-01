using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections;
using System.Xml;
namespace Com.Delta.PrintManager.Engine
{
	/// <summary>
	/// Report element for displaying XY charts.
	/// </summary>
	public sealed class Scatter : ICustomPaint
	{

		private Rectangle mMapArea = new Rectangle(0,0,0,0);
		private Rectangle mLegendArea = new Rectangle(0,0,0,0);

		private string mTitle = "Scatter Diagram Title";
		private string mXLabel = "X Axis Title";

		private bool mShowLegend = false;
		private bool showMarkers = false;

		private Color mBorderColor = Color.Black;
		private Color mMapAreaColor = Color.WhiteSmoke;
		private Color mBackgroundColor = Color.White;
		private int mBorderWidth = 1;
		private Font mTitleFont = new Font("Tahoma", 8, FontStyle.Bold);
		private Font mLabelFont = new Font("Tahoma", 8, FontStyle.Regular);

		private Font titleFont = null;
		private Font labelFont = null;

		private float mMaxY = 100f;
		private float mMinY = 0f;

		private float mMaxX = 100f;
		private float mMinX = 0f;

		private ArrayList series = new ArrayList();

		private int pdfImageQuality = 100;

		#region Constructors

		/// <summary>
		/// Creates new instance of Scatter report element.
		/// </summary>
		public Scatter(int x,int y, int width, int height, Section parent)
		{
			section = parent;
			theRegion = new Rectangle(x,y,width,height);
			this.Bounds = theRegion;
		}

		internal Scatter(XmlNode node, Section parent)
		{
			section = parent;
			Init(node);			
		}

		#endregion

		#region Public Overrides

		/// <summary>
		/// Gets the string representation of report element.
		/// </summary>
		public override string ToString()
		{
			return "Scatter [" + this.Name + "]" ;
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
		/// Gets or sets the background color for the scatter element.
		/// </summary>
		[Category("Appearance"), DefaultValue(typeof(System.Drawing.Color),"White")]
		public Color BackgroundColor
		{
			get {return mBackgroundColor;}
			set {mBackgroundColor = value;}
		}

		/// <summary>
		/// Gets or sets the border color for the scatter element.
		/// </summary>
		/// <remarks>This property sets the border color of the scatter element. This can be any color
		/// from the System.Drawing.Color structure.
		/// </remarks>
		[Category("Appearance"), DefaultValue(typeof(System.Drawing.Color),"Black")]
		public Color BorderColor
		{
			get {return mBorderColor;}
			set {mBorderColor = value;}
		}


		/// <summary>
		/// Gets or sets the border width for the scatter element.
		/// </summary>
		/// <remarks>
		/// BorderWidth of the scatter element. If this is set to zero, then the border is invisible.
		/// </remarks>
		[Category("Appearance"), DefaultValue(1)]
		public int BorderWidth
		{
			get {return mBorderWidth;}
			set {mBorderWidth = value;}
		}


		/// <summary>
		/// Gets/Sets the font used for any labels displayed on the scatter element.
		/// </summary>
		[Category("Appearance"), DefaultValue(typeof(System.Drawing.Font),"Tahoma, 8pt")]
		public Font LabelFont
		{
			get {return mLabelFont;}
			set {mLabelFont = value;}
		}


		/// <summary>
		/// The background color displayed behind the scatter image
		/// </summary>
		/// <remarks>This property sets the background color displayed behind the chart image. This can be any color
		/// from the System.Drawing.Color structure
		/// </remarks>
		[Category("Appearance"),Description("The color for map area within the scatter."), DefaultValue(typeof(System.Drawing.Color),"WhiteSmoke")]
		public Color MapAreaColor
		{
			get {return mMapAreaColor;}
			set {mMapAreaColor = value;}
		}

		/// <summary>
		/// Gets or sets whether to display the chart legend.
		/// </summary>
		[Category("Appearance"), Description("Show legend in scatter."), DefaultValue(false)]
		public bool ShowLegend
		{
			get {return mShowLegend;}
			set {mShowLegend = value;}
		}

		/// <summary>
		/// Gets or sets whether to display the dot marker in lines.
		/// </summary>
		[Category("Appearance"), Description("Show dot markers for data points."), DefaultValue(false)]
		public bool ShowMarkers
		{
			get {return showMarkers;}
			set {showMarkers = value;}
		}

		/// <summary>
		/// Gets or sets the title; value used at the top of the scatter element.
		/// </summary>
		[Category("Title"), Editor(typeof(Com.Delta.PrintManager.Engine.Editors.PlainTextEditor), typeof(UITypeEditor)), Description("The Title of the chart."), DefaultValue("Scatter Diagram Title")]
		public string Title
		{
			get {return mTitle;}
			set {mTitle = value;}
		}

		/// <summary>
		/// Gets or sets the font used for the title of the scatter element.
		/// </summary>
		[Category("Title"), Description("Title font"), DefaultValue(typeof(System.Drawing.Font),"Tahoma, 8pt, style=Bold")]
		public Font TitleFont
		{
			get {return mTitleFont;}
			set {mTitleFont = value;}
		}
		/// <summary>
		/// Gets or sets the name of the scatter element.
		/// </summary>
		[Category("Data"), Description("Gets or sets the name of the scatter element.")]
		public override string Name
		{
			get {return name;}
			set {name = value;}
		}

		/// <summary>
		/// Gets or sets the value used for labelling the X-axis of the scatter element.
		/// </summary>
		[Category("Data"), Description("The X-axis label.")]
		public string XLabel
		{
			get {return mXLabel;}
			set {mXLabel = value;}
		}

		[Category("Data"), Description("Gets or sets the data series for scatter element.")]
		public ScatterSerie[] Series
		{
			get {return (ScatterSerie[])series.ToArray(typeof(ScatterSerie));}
			set {series = new ArrayList(value);}
		}

		/// <summary>
		/// Chart image for printing and exporting purpose
		/// </summary>
		[Browsable(false)]
		internal Bitmap Image
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
		

		/// <summary>
		/// Paints the scatter element.
		/// </summary>
		/// <param name="g">The Graphics object to draw</param>
		/// <remarks>Causes the scatter element to be painted to the screen.</remarks>
		public override void Paint(Graphics g)
		{
			Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;

			float sizeInPixels = this.PointsToPixels(mLabelFont.SizeInPoints);
			labelFont = new Font(mLabelFont.FontFamily, sizeInPixels*g.PageScale, mLabelFont.Style, GraphicsUnit.Pixel);
			sizeInPixels = this.PointsToPixels(mTitleFont.SizeInPoints);
			titleFont = new Font(mTitleFont.FontFamily, sizeInPixels*g.PageScale, mTitleFont.Style, GraphicsUnit.Pixel);

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

			 
			g.SetClip(new Region(paintArea), System.Drawing.Drawing2D.CombineMode.Intersect);
			drawDiagram(g);
			g.ResetClip();

			if ( mBorderWidth > 0 )
			{
				Pen borderPen = new Pen(mBorderColor, mBorderWidth);
				borderPen.Alignment = PenAlignment.Inset;
				g.DrawRectangle(borderPen, paintArea);
			}
		}



		/// <summary>
		/// Clones the structure of the scatter element, including all properties.
		/// </summary>
		/// <returns><see cref="Com.Delta.PrintManager.Engine.Scatter">Com.Delta.PrintManager.Engine.Scatter</see></returns>
		public override object Clone()
		{
			
			Scatter tmp = new Scatter(this.X, this.Y, this.Width, this.Height, this.section);
			tmp.Layout = this.Layout;
			tmp.BorderWidth = this.BorderWidth;
			tmp.BorderColor = this.BorderColor;
			tmp.mTitleFont = this.mTitleFont;
			tmp.mTitle = this.mTitle;
			tmp.ShowLegend = this.ShowLegend;
			tmp.ShowMarkers = this.ShowMarkers;
			tmp.MapAreaColor = this.MapAreaColor;
			tmp.BackgroundColor = this.BackgroundColor;
			tmp.mLabelFont = this.mLabelFont;
			tmp.ExportQuality = this.ExportQuality;

			tmp.Name = "scatter" + tmp.GetHashCode().ToString();

			ArrayList tmpList = new ArrayList();
			foreach(ScatterSerie s in this.series)
			{
				tmpList.Add(s.Clone());
			}

			tmp.series = tmpList;

			return tmp;
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


			bottomTextHeight += sfLabel.Height;

			if (mShowLegend)
			{
				int originX = (int) (paintArea.X + paintArea.Width*0.08);
				int originY = (int) (paintArea.Y + sf.Height+10);
				int width = (int) (paintArea.Width*0.68);
				int height = (int) (paintArea.Height-sf.Height-10-bottomTextHeight-9);

				mMapArea = new Rectangle(originX, originY, width, height);
			
				originX = (int) (paintArea.X + paintArea.Width*0.78);
				originY = (int) (paintArea.Y + sf.Height+10);
				width = (int) (paintArea.Width*0.20);
				height = (int) (paintArea.Height - sf.Height - 10 - bottomTextHeight - 9);

				mLegendArea = new Rectangle(originX, originY, width, height);
			}
			else
			{
				int originX = (int) (paintArea.X + paintArea.Width*0.1);
				int originY = (int) (paintArea.Y + sf.Height + 10);
				int width = (int) (paintArea.Width*0.8);
				int height = (int) (paintArea.Height - sf.Height - 10 - bottomTextHeight-9);

				mMapArea = new Rectangle(originX, originY, width, height);
			
			}
		}

		private void drawDiagram(Graphics g)
		{
			drawTitle(g);
			drawScatterArea(g);
			drawLabels(g);
			drawLines(g);

			if (mShowLegend)
				drawLegend(g);
		}

		private void drawTitle(Graphics g)
		{
			Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;
			SizeF sf = g.MeasureString(mTitle, titleFont);
			g.DrawString(mTitle, titleFont,new SolidBrush(Color.Black), paintArea.X + paintArea.Width/2 - sf.Width/2, paintArea.Y + 5);
		}

		private void drawScatterArea(Graphics g)
		{

			if (mMapAreaColor != Color.Transparent)
			{
				if (paintTransparent)
					g.FillRectangle(new SolidBrush(ShiftColor(mMapAreaColor,192)), mMapArea);
				else
					g.FillRectangle(new SolidBrush(mMapAreaColor), mMapArea);
			}

			g.DrawRectangle(new Pen(Color.Black,1), mMapArea);

		}

		private void drawLabels(Graphics g)
		{
			Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;

			resolveRangeY();
			resolveRangeX();		

			SizeF sf = g.MeasureString(mXLabel,labelFont);
			g.DrawString(mXLabel, labelFont, new SolidBrush(Color.Black), mMapArea.X + mMapArea.Width/2 - sf.Width/2, paintArea.Bottom - sf.Height - 3);

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
				string numLabel = i.ToString("#,##0.##");
				sf = g.MeasureString(numLabel, labelFont);
				float theYPosition = calculateYPosition(i);
				g.DrawString(numLabel, labelFont,new SolidBrush(Color.Black), mMapArea.Left - 2 - sf.Width, theYPosition - sf.Height/2);
				if (i!=mMinY && i!=mMaxY)
					g.DrawLine(new Pen(Color.DarkGray,1), mMapArea.Left, theYPosition, mMapArea.Right, theYPosition);
			}

			step = Math.Min(Math.Abs(mMinX),Math.Abs(mMaxX))/2f;

			if (mMaxX!=0 && mMinX!=0)
				step = Math.Min(Math.Abs(mMinX),Math.Abs(mMaxX))/2f;
			else if (mMaxX!=0 && mMinX==0)
				step = Math.Abs(mMaxX)/4f;
			else if (mMaxX==0 && mMinX!=0)
				step = Math.Abs(mMinX)/4f;
			else
				return;

			
			for ( float i=mMinX;i<=mMaxX;i+=step)
			{
				string numLabel = i.ToString("#,##0.##");
				sf = g.MeasureString(numLabel, labelFont);
				float theXPosition = calculateXPosition(i);
				g.DrawString(numLabel, labelFont,new SolidBrush(Color.Black), theXPosition - sf.Width/2, mMapArea.Bottom + 2 );
				if (i!=mMinX && i!=mMaxX)
					g.DrawLine(new Pen(Color.DarkGray,1), theXPosition, mMapArea.Top, theXPosition, mMapArea.Bottom);
			}
			
			
		}

		private void drawLines(Graphics g)
		{
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

			for (int i=0;i<series.Count;i++)
			{
				ScatterSerie data = series[i] as ScatterSerie;

				PointF[] points = new PointF[data.X.Length];

				for (int j=0;j<data.X.Length;j++)
				{
					points[j] = new PointF(this.calculateXPosition((float)data.X[j]), this.calculateYPosition((float)data.Y[j])); 
				}

				Array.Sort(points, new PointComparer());

				if (points.Length > 1)
				{
					Pen pen = new Pen(data.Color);
					if (data.DashStyle == DashStyle.Dash)
						pen.DashPattern = new float[]{4,4};
					else if (data.DashStyle == DashStyle.Dot)
						pen.DashPattern = new float[]{2,4};
					else if (data.DashStyle == DashStyle.DashDot)
						pen.DashPattern = new float[]{4,3,1,3};
					else if (data.DashStyle == DashStyle.DashDotDot)
						pen.DashPattern = new float[]{4,3,1,3,1,3};
					else
						pen.DashStyle = data.DashStyle;

					g.DrawLines(pen, points);
				}

				

				if (showMarkers)
				{
					Brush markerBrush = new SolidBrush(data.Color);
					for (int k=0;k<points.Length;k++)
					{
						g.FillEllipse(markerBrush, points[k].X - 2, points[k].Y - 2, 5, 5);
					}
					markerBrush.Dispose();
				}
			}

			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
		}

		private void drawLegend(Graphics g)
		{

			g.DrawRectangle(new Pen(Color.Black,1), mLegendArea);
						
			for (int i=0;i<series.Count;i++)
			{
				ScatterSerie data = series[i] as ScatterSerie;

				SizeF sf = g.MeasureString(data.Name, labelFont);
				int yPos = (int)(mLegendArea.Y + 8 + i*sf.Height);
				//g.FillRectangle(new SolidBrush(data.Color), mLegendArea.X-rBounds.X + 8, yPos-3 , 6, 6);

				Pen pen = new Pen(data.Color);
				if (data.DashStyle == DashStyle.Dash)
					pen.DashPattern = new float[]{4,2};
				else if (data.DashStyle == DashStyle.Dot)
					pen.DashPattern = new float[]{1,3};
				else if (data.DashStyle == DashStyle.DashDot)
					pen.DashPattern = new float[]{5,2,2,2};
				else if (data.DashStyle == DashStyle.DashDotDot)
					pen.DashPattern = new float[]{4,2,1,2,1,2};
				else
					pen.DashStyle = data.DashStyle;

				g.DrawLine(pen, new Point(mLegendArea.X + 4, yPos), new Point(mLegendArea.X + 20, yPos)); 
				g.DrawString(data.Name, labelFont, new SolidBrush(Color.Black), mLegendArea.X + 22, yPos-sf.Height/2);	
			}
		}

		private void resolveRangeY()
		{
			double maxValue = 0;
			double minValue = 0;
		
			for (int i=0;i<series.Count;i++)
			{
				ScatterSerie data = series[i] as ScatterSerie;

				double[] tmp = new double[data.Y.Length];
				Array.Copy(data.Y,0,tmp,0,data.Y.Length);

				if (tmp.Length>0)
				{
					Array.Sort(tmp);
					maxValue = Math.Max(maxValue,tmp[tmp.Length-1]);
					minValue = Math.Min(minValue,tmp[0]);
				}
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

		private void resolveRangeX()
		{
			double maxValue = 0;
			double minValue = 0;

			for (int i=0;i<series.Count;i++)
			{
				ScatterSerie data = series[i] as ScatterSerie;

				double[] tmp = new double[data.X.Length];
				Array.Copy(data.X,0,tmp,0,data.X.Length);

				if (tmp.Length>0)
				{
					Array.Sort(tmp);
					maxValue = Math.Max(maxValue,tmp[tmp.Length-1]);
					minValue = Math.Min(minValue,tmp[0]);
				}
			}


			if (maxValue==0 && minValue==0)
			{
				maxValue = 100;
				minValue = 0;
			}
			else if (maxValue!=0 && minValue==0)
			{
				int theBase = (int)Math.Ceiling(Math.Log10(maxValue));
				mMaxX = (float)Math.Pow(10,theBase);

				if (maxValue<=mMaxX/2) mMaxX=mMaxX/2;
			}
			else
			{
				int theBase = (int)Math.Max( Math.Ceiling(Math.Log10(Math.Abs(maxValue))),Math.Ceiling(Math.Log10(Math.Abs(minValue))) );
				mMaxX = (float)Math.Pow(10,theBase);
				mMinX = (float)Math.Pow(10,theBase)*Math.Sign(minValue);

				if (maxValue<=mMaxX/2) mMaxX=mMaxX/2;
				if (minValue>=mMinX/2) mMinX=mMinX/2;

			}

		}

		private float calculateYPosition(float x)
		{
			return calculateYZeroPosition() - mMapArea.Height*x/(mMaxY-mMinY);
		}


		private float calculateYZeroPosition()
		{
			return (float)(mMapArea.Top+mMapArea.Height*mMaxY/(mMaxY-mMinY));
		}

		private float calculateXPosition(float x)
		{
			return  calculateXZeroPosition() + mMapArea.Width*x/(mMaxX-mMinX);
		}

		private float calculateXZeroPosition()
		{
			return (float)(mMapArea.Right - mMapArea.Width*mMaxX/(mMaxX-mMinX));
		}

		#endregion

		#region Public Functions

		/// <summary>
		/// Adds data serie to the scatter element.
		/// </summary>
		public void AddSerie(string serieName, double[] x, double[] y, Color serieColor)
		{
			series.Add(new ScatterSerie(serieName, x, y, serieColor));
		}

		/// <summary>
		/// Adds data serie to the scatter element.
		/// </summary>
		public void AddSerie(string serieName, double[] x, double[] y, Color serieColor, DashStyle style)
		{
			series.Add(new ScatterSerie(serieName, x, y, serieColor, style));
		}

		#endregion

	}

	/// <summary>
	/// Class that holds data to be displayed in Scatter element.
	/// </summary>
	public class ScatterSerie : ICloneable
	{

		private string name = "Test Serie";
		private double[] xData = new double[]{};
		private double[] yData = new double[]{};
		private Color color = Color.Red;
		private DashStyle dashStyle = DashStyle.Solid;

		public ScatterSerie()
		{

		}

		internal ScatterSerie(string name, double[] x, double[] y)
		{
			this.name = name;
			SetData(x, y);
		}

		internal ScatterSerie(string name, double[] x, double[] y, Color color):this(name, x, y)
		{
			this.color = color;
		}

		internal ScatterSerie(string name, double[] x, double[] y, Color color, DashStyle style):this(name, x, y, color)
		{
			this.dashStyle = style;
		}

		private void SetData(double[] x, double[] y)
		{
			if (x.Length >= xData.Length)
			{
				int count = Math.Max(x.Length, y.Length);

				double[] tXData = new double[count];
				double[] tYData = new double[count];

				for (int i=0;i<x.Length;i++)
					tXData[i] = x[i];

				for (int i=0;i<y.Length;i++)
					tYData[i] = y[i];

				xData = tXData;
				yData = tYData;
			}
			else
			{
				int count = x.Length;

				double[] tXData = new double[count];
				double[] tYData = new double[count];

				for (int i=0;i<count;i++)
					tXData[i] = x[i];

				for (int i=0;i<count;i++)
					tYData[i] = y[i];

				xData = tXData;
				yData = tYData;
			}
		}

		/// <summary>
		/// Gets or sets the serie name.
		/// </summary>
		public string Name
		{
			get {return name;}
			set {name = value;}
		}

		public double[] Y
		{
			get {return yData;}
			set {SetData(xData, value);}
		}

		public double[] X
		{
			get {return xData;}
			set { SetData(value, yData);}
		}

		/// <summary>
		/// Gets or sets the line display color.
		/// </summary>
		[DefaultValue(typeof(System.Drawing.Color),"Red")]
		public Color Color
		{
			get {return color;}
			set {color = value;}
		}

		/// <summary>
		/// Gets or sets the line style.
		/// </summary>
		[DefaultValue(DashStyle.Solid)]
		public DashStyle DashStyle
		{
			get {return dashStyle;}
			set {dashStyle = value;}
		}

		#region ICloneable Members

		public object Clone()
		{
			return new ScatterSerie(name, xData, yData, color);			
		}

		#endregion
	}

	internal class PointComparer : IComparer
	{
		#region IComparer Members

		public int Compare(object x, object y)
		{
			if ( x is PointF && y is PointF)
			{
				if ( ((PointF)x).X > ((PointF)y).X )
					return 1;
				else if ( ((PointF)x).X < ((PointF)y).X)
					return -1;
				else
					return 0;
			}
			else
			{			
				return 0;
			}
		}

		#endregion
	}
}
