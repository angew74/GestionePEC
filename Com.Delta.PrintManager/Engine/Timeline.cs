using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.IO;
using System.Collections;
using System.Xml;

namespace Com.Delta.PrintManager.Engine
{
	/// <summary>
	/// Report element for displaying data in a chronological context. 
	/// </summary>
	[DefaultProperty("Events")]
	public sealed class Timeline : ICustomPaint
	{

		/// <summary>
		/// Types of markers supported. Markers are shown only if Timeline.UseDates is set to true. 
		/// </summary>
		public enum MarkerStyles 
		{
			/// <summary>
			/// No markers. 
			/// </summary>
			None = 0,
			/// <summary>
			/// Label markers. 
			/// </summary>
			Label,
			/// <summary>
			/// Grid markers. 
			/// </summary>
			Grid 
		};


		/// <summary>
		/// Types of marker periods supported. 
		/// </summary>
		public enum MarkerPeriods {Minute, Hour, Day, Week, Month, Year, Decade, Century };

		/// <summary>
		/// Types of stripe styles supported. 
		/// </summary>
		public enum StripeStyles {None = 0, Line, Arrow};

		private int borderWidth = 1;
		private bool useDates = true;
		private DateTime startDate = new DateTime(2006,1,1);
		private DateTime endDate = new DateTime(2006,12,31);
		private long startValue = 0;
		private long endValue = 100;
		private int stripeNumber = 1;
		private Stripe[] stripes;
		private TimelinePoint[] points = new TimelinePoint[0]{};
		private TimelinePeriod[] periods = new TimelinePeriod[0]{};
		private Color backgroundColor = Color.White; 
		private Color stripeColor = Color.LightBlue;
		private int stripeSize = 20;
		private string markerDateFormat = "MMM yyyy";
		private Size padding = new Size(10, 10);
		private MarkerPeriods markerPeriod = MarkerPeriods.Month;
		private MarkerStyles markerStyle = MarkerStyles.Grid;
		private StripeStyles stripeStyle = StripeStyles.Arrow;
		private Font markerFont = new Font("Tahoma", 7);
		private Color markerColor = Color.Silver;

		private ArrayList periodPositions = new ArrayList();
		private int pdfImageQuality = 100;


		/// <summary>
		/// Creates new instance of Timeline in the given section.
		/// </summary>
		public Timeline(int x,int y, int width, int height, Section parent)
		{
			section = parent;
			theRegion = new Rectangle(x,y,width,height);
			this.Bounds = theRegion;
		}

		internal Timeline(XmlNode node, Section parent)
		{
			section = parent;
			Init(node);			
		}

		/// <summary>
		/// Gets the string representation of report element.
		/// </summary>
		public override string ToString()
		{
			return "Timeline [" + this.Name + "]" ;
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
		/// Timeline start date. Used if Timeline.UseDates is set to true.
		/// </summary>
		[Category("Data"),Editor(typeof(Com.Delta.PrintManager.Engine.Editors.DateEditor), typeof(UITypeEditor)), Description("Timeline start date. Used if Timeline.UseDates is set to true."), DefaultValue(typeof(System.DateTime),"1/1/2006")]
		public DateTime StartDate
		{
			get {return startDate;}
			set {startDate = value;}
		}

		/// <summary>
		/// Timeline end date. Used if Timeline.UseDates is set to true.
		/// </summary>
		[Category("Data"),Editor(typeof(Com.Delta.PrintManager.Engine.Editors.DateEditor), typeof(UITypeEditor)), Description("Timeline end date. Used if Timeline.UseDates is set to true."), DefaultValue(typeof(System.DateTime),"12/31/2006")]
		public DateTime EndDate
		{
			get {return endDate;}
			set {endDate = value;}
		}

		/// <summary>
		/// Timeline start value. Used if Timeline.UseDates is set to false.
		/// </summary>
		[Category("Data"), Description("Timeline start value. Used if Timeline.UseDates is set to false."), DefaultValue(0l)]
		public long StartValue
		{
			get {return startValue;}
			set {startValue = value;}
		}

		/// <summary>
		/// Timeline end value. Used if Timeline.UseDates is set to false.
		/// </summary>
		[Category("Data"), Description("Timeline end value. Used if Timeline.UseDates is set to false."), DefaultValue(100l)]
		public long EndValue
		{
			get {return endValue;}
			set {endValue = value;}
		}

		/// <summary>
		/// Determines if timeline is using dates or values for presenting timepoints.
		/// </summary>
		[Category("Data"), Description("Determines if timeline is using dates or values for presenting timepoints."), DefaultValue(true)]
		public bool UseDates
		{
			get {return useDates;}
			set {useDates = value;}
		}

		/// <summary>
		/// Gets or sets timeline border width.
		/// </summary>
		[Category("Display"), Description("Gets or sets timeline border width."), DefaultValue(1)]
		public int BorderWidth
		{
			get {return borderWidth;}
			set {borderWidth = Math.Max(0,value);}
		}


		/// <summary>
		/// Gets or sets display style for date markers. Markers are shown only if Timeline.UseDates is set to true.
		/// </summary>
		[Category("Display"), Description("Gets or sets display style for date markers. Markers are shown only if Timeline.UseDates is set to true."), DefaultValue(MarkerStyles.Grid)]
		public MarkerStyles MarkerStyle
		{
			get {return markerStyle;}
			set {markerStyle = value;}
		}

		/// <summary>
		/// Gets or sets display style for timeline stripe
		/// </summary>
		[Category("Display"), Description("Gets or sets display style for timeline stripe."), DefaultValue(StripeStyles.Arrow)]
		public StripeStyles StripeStyle
		{
			get {return stripeStyle;}
			set {stripeStyle = value;}
		}

		/// <summary>
		/// Gets or sets marker period. This property is significant only if Timeline.UseDates is set to true.
		/// </summary>
		[Category("Data"), Description("Gets or sets marker period. This property is significant only if Timeline.UseDates is set to true."), DefaultValue(MarkerPeriods.Month)]
		public MarkerPeriods MarkerPeriod
		{
			get {return markerPeriod;}
			set 
			{
				markerPeriod = value;
				switch(markerPeriod)
				{
					case MarkerPeriods.Minute:
					case MarkerPeriods.Hour:
						this.markerDateFormat = "HH:mm";
						break;
					case MarkerPeriods.Day:
					case MarkerPeriods.Week:
						this.markerDateFormat = "dd/MM/yyyy";					
						break;
					case MarkerPeriods.Month:
						this.markerDateFormat = "MMM yyyy.";
						break;
					case MarkerPeriods.Year:
					case MarkerPeriods.Decade:
					case MarkerPeriods.Century:
						this.markerDateFormat = "yyyy.";
						break;
				}
			}
		}

		/// <summary>
		/// Gets or sets the inner padding of the timeline.
		/// </summary>
		[Category("Display"), Description("Gets or sets the inner padding of the timeline."), DefaultValue(typeof(System.Drawing.Size),"10, 10")]
		public Size Padding
		{
			get {return padding;}
			set {padding = value;}
		}

		/// <summary>
		/// The number of stripes in timeline.
		/// </summary>
		[Category("Data"), Description("The number of stripes in timeline."), DefaultValue(1)]
		public int StripeNumber
		{
			get {return stripeNumber;}
			set {stripeNumber = Math.Max(1,value);}
		}

		/// <summary>
		/// Gets or sets width of the stripes.
		/// </summary>
		[Category("Display"), Description("Gets or sets width of the stripes."), DefaultValue(20)]
		public int StripeSize
		{
			get {return stripeSize;}
			set 
			{
				stripeSize = Math.Max(2,value);
			}
		}


		/// <summary>
		/// Timeline background color.
		/// </summary>
		[Category("Display"), Description("Timeline background color."), DefaultValue(typeof(System.Drawing.Color),"White")]
		public Color BackgroundColor
		{
			get {return backgroundColor;}
			set {backgroundColor = value;}
		}

		/// <summary>
		/// Gets or sets color for stripes.
		/// </summary>
		[Category("Display"), Description("Gets or sets color for stripes."), DefaultValue(typeof(System.Drawing.Color),"LightBlue")]
		public Color StripeColor
		{
			get {return stripeColor;}
			set {stripeColor = value;}
		}

		/// <summary>
		/// Gets or sets color for markers.
		/// </summary>
		[Category("Display"), Description("Gets or sets color for markers."), DefaultValue(typeof(System.Drawing.Color),"Silver")]
		public Color MarkerColor
		{
			get {return markerColor;}
			set {markerColor = value;}
		}

		/// <summary>
		/// Gets or sets the timeline name.
		/// </summary>
		[Category("Data"), Description("Gets or sets the timeline name.")]
		public override string Name
		{
			get {return name;}
			set {name = value;}
		}

		/// <summary>
		/// Timeline image for printing and exporting purpose
		/// </summary>
		[Browsable(false)]
		public Bitmap Image
		{
			get 
			{
                // Raffaele Russo - 12/12/2011 - Start
                Rectangle paintArea = new Rectangle(Bounds.X - rBounds.X, Bounds.Y - rBounds.Y, Bounds.Width, Bounds.Height);
                // Rectangle paintArea = new Rectangle(theRegion.X - rBounds.X, theRegion.Y - rBounds.Y, theRegion.Width, theRegion.Height);
                // Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;
                // Raffaele Russo - 12/12/2011 - End

				Bitmap timelineImage = new Bitmap(paintArea.Width + (borderWidth==1? 1 : 0), paintArea.Height + (borderWidth==1? 1 : 0));

				Graphics imageGraphics = Graphics.FromImage(timelineImage);
				
				Matrix m = new Matrix();
				m.Translate(-paintArea.X, -paintArea.Y, MatrixOrder.Append);
				imageGraphics.Transform = m;

				this.Paint(imageGraphics);
				imageGraphics.Dispose();

				return timelineImage;
			}
		}


		/// <summary>
		/// Set of TimelinePeriods that contain display data.
		/// </summary>
		[Category("Data"), Description("Set of TimelinePeriods that contain display data.")]
		public TimelinePeriod[] Periods
		{
			get {return periods;}
			set {periods = value;}
		}

		/// <summary>
		/// Set of TimelinePoints that contain display data.
		/// </summary>
		[Category("Data"), Description("Set of TimelinePoints that contain display data.")]
		public TimelinePoint[] Events
		{
			get {return points;}
			set 
			{
				points = value;
				for (int i=0;i<points.Length;i++)
				{
					if (points[i].PictureFile != String.Empty)
					{
						try
						{
							string imageFile = section.Document.DocRoot;
							if (points[i].PictureFile.StartsWith(Path.DirectorySeparatorChar.ToString()))
								imageFile += points[i].PictureFile;
							else
								imageFile += Path.DirectorySeparatorChar.ToString() + points[i].PictureFile;

							points[i].Picture = new Bitmap(imageFile);
						}
						catch(Exception)
						{
							points[i].Picture = null;
						}
					}
				}
			}
		}

		private void CalculateStripes()
		{

            // Raffaele Russo - 12/12/2011 - Start
            Rectangle paintArea = new Rectangle(Bounds.X - rBounds.X, Bounds.Y - rBounds.Y, Bounds.Width, Bounds.Height);
            // Rectangle paintArea = new Rectangle(theRegion.X - rBounds.X, theRegion.Y - rBounds.Y, theRegion.Width, theRegion.Height);
            // Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;
            // Raffaele Russo - 12/12/2011 - End

			long range = useDates ? endDate.Ticks - startDate.Ticks : endValue - startValue;

			long tickStripeInternal = range / stripeNumber;
			int stripeHeight = (paintArea.Height - 2*padding.Height) / stripeNumber;

		
			stripes = new Stripe[stripeNumber];
			for (int i=0;i<stripeNumber;i++)
			{
				Stripe s = new Stripe(new Rectangle(paintArea.X + padding.Width, paintArea.Y + padding.Height + i*stripeHeight, paintArea.Width - 2*padding.Width , stripeHeight));

				if (useDates)
				{
					s.StartTicks = startDate.Ticks + i*tickStripeInternal;
					s.EndTicks = s.StartTicks + tickStripeInternal;
				}
				else
				{
					s.StartTicks = startValue + i*tickStripeInternal;
					s.EndTicks = s.StartTicks + tickStripeInternal;
				}
				s.StripeSize = stripeSize;
				stripes[i] = s;
			}			

		}

		#region Public Overrides


		/// <summary>
		/// Paints the Timeline
		/// </summary>
		/// <param name="g">The Graphics object to draw</param>
		/// <remarks>Causes the Timeline to be painted to the screen.</remarks>
		public override void Paint(Graphics g)
		{
			CalculateStripes();

			float sizeInPixels = this.PointsToPixels(markerFont.SizeInPoints);
			Font displayFont = new Font(markerFont.FontFamily, sizeInPixels*g.PageScale, markerFont.Style, GraphicsUnit.Pixel);

            // Raffaele Russo - 12/12/2011 - Start
            Rectangle paintArea = new Rectangle(Bounds.X - rBounds.X, Bounds.Y - rBounds.Y, Bounds.Width, Bounds.Height);
            // Rectangle paintArea = new Rectangle(theRegion.X - rBounds.X, theRegion.Y - rBounds.Y, theRegion.Width, theRegion.Height);
            // Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;
            // Raffaele Russo - 12/12/2011 - End

			Brush bgBrush = paintTransparent ? new SolidBrush(ShiftColor(backgroundColor, 192)) : new SolidBrush(backgroundColor);
			g.FillRectangle(bgBrush, paintArea);
			if (borderWidth>0)
			{
				Pen borderPen = new Pen(Color.Black, borderWidth);
				borderPen.Alignment = PenAlignment.Inset;
				g.DrawRectangle(borderPen, paintArea);
			}

			if (startDate.Ticks>endDate.Ticks)
			{
				StringFormat sf = new StringFormat();
				sf.Alignment = StringAlignment.Center;
				sf.LineAlignment = StringAlignment.Center;
				g.DrawString("StartDate can not be after EndDate.", displayFont, new SolidBrush(this.markerColor), paintArea, sf);
			}
			else
			{

				if (markerStyle == MarkerStyles.Grid && useDates)
					PaintGrid(g);
			
				PaintStripes(g);

				if (markerStyle == MarkerStyles.Label && useDates)
					PaintLabels(g);

				g.SetClip(new Region(paintArea), System.Drawing.Drawing2D.CombineMode.Intersect);

				PaintTimelinePointsConnectors(g);
				PaintTimelinePeriods(g);			
				PaintTimelinePoints(g);
				g.ResetClip();
			}
			
		}


		/// <summary>
		/// Clones the structure of the Timeline, including all properties
		/// </summary>
		/// <returns><see cref="Com.Delta.PrintManager.Engine.Timeline">Com.Delta.PrintManager.Engine.Timeline</see></returns>
		public override object Clone()
		{
			Timeline tmp = new Timeline(0,0,0,0,section);
			tmp.X = this.X;
			tmp.Y = this.Y;
			tmp.Width = this.Width;
			tmp.Height = this.Height;
			tmp.Name = this.Name;
			tmp.Layout = this.Layout;
			tmp.BackgroundColor = this.BackgroundColor;
			tmp.BorderWidth = this.BorderWidth;
			tmp.ExportQuality = this.ExportQuality;
			tmp.EndDate = this.EndDate;
			tmp.EndValue = this.EndValue;
			tmp.MarkerColor = this.MarkerColor;
			tmp.MarkerPeriod = this.MarkerPeriod;
			tmp.MarkerStyle = this.MarkerStyle;
			tmp.Padding = this.Padding;
			tmp.StartDate = this.StartDate;
			tmp.StartValue = this.StartValue;
			tmp.StripeStyle = this.StripeStyle;
			tmp.StripeColor = this.StripeColor;
			tmp.StripeNumber = this.StripeNumber;
			tmp.StripeSize = this.StripeSize;
			tmp.UseDates = this.UseDates;

			TimelinePoint[] tmpPoints = new TimelinePoint[points.Length];
			for (int i=0;i<points.Length;i++)
			{
				tmpPoints[i] = (TimelinePoint)points[i].Clone();
			}
			tmp.Events = tmpPoints;


			TimelinePeriod[] tmpPeriods = new TimelinePeriod[periods.Length];
			for (int i=0;i<periods.Length;i++)
			{
				tmpPeriods[i] = (TimelinePeriod)periods[i].Clone();
			}
			tmp.Periods = tmpPeriods;


			return tmp;
		}


		#endregion


		private long[] CalculateMarkers(Stripe stripe)
		{
			ArrayList markers = new ArrayList();
			DateTime s = new DateTime(stripe.StartTicks);

			if (markerPeriod == MarkerPeriods.Day)
			{
				DateTime s1 = new DateTime(s.Year, s.Month, s.Day);

				while(s1.Ticks<stripe.EndTicks)
				{
					if (s1.Ticks>=stripe.StartTicks) markers.Add(s1.Ticks);
					s1 = s1.AddDays(1);
				}
			}
			else if (markerPeriod == MarkerPeriods.Minute)
			{
				DateTime s1 = new DateTime(s.Year, s.Month, s.Day, s.Hour, s.Minute, 0);

				while(s1.Ticks<stripe.EndTicks)
				{
					if (s1.Ticks>=stripe.StartTicks) markers.Add(s1.Ticks);
					s1 = s1.AddMinutes(1);
				}
			}
			else if (markerPeriod == MarkerPeriods.Hour)
			{
				DateTime s1 = new DateTime(s.Year, s.Month, s.Day, s.Hour, 0, 0);

				while(s1.Ticks<stripe.EndTicks)
				{
					if (s1.Ticks>=stripe.StartTicks) markers.Add(s1.Ticks);
					s1 = s1.AddHours(1);
				}
			}
			else if (markerPeriod == MarkerPeriods.Week)
			{
				DateTime s1 = s.AddDays(-Convert.ToInt32(s.DayOfWeek)+1);
				while(s1.Ticks<stripe.EndTicks)
				{
					if (s1.Ticks>=stripe.StartTicks) markers.Add(s1.Ticks);
					s1 = s1.AddDays(7);
				}
			}
			else if (markerPeriod == MarkerPeriods.Month)
			{
				DateTime s1 = new DateTime(s.Year, s.Month, 1);

				while(s1.Ticks<stripe.EndTicks)
				{
					if (s1.Ticks>=stripe.StartTicks) markers.Add(s1.Ticks);
					s1 = s1.AddMonths(1);
				}
			}
			else if (markerPeriod == MarkerPeriods.Year)
			{
				DateTime s1 = new DateTime(s.Year, 1, 1);

				while(s1.Ticks <stripe.EndTicks)
				{
					if (s1.Ticks>=stripe.StartTicks) markers.Add(s1.Ticks);
					s1 = s1.AddYears(1);
				}
			}
			else if (markerPeriod == MarkerPeriods.Decade)
			{
				int year = 10*(s.Year/10);
				DateTime s1 = new DateTime(Math.Max(1,year), 1, 1);

				while(s1.Ticks <stripe.EndTicks)
				{
					if (s1.Ticks>=stripe.StartTicks) markers.Add(s1.Ticks);
					s1 = s1.AddYears(10);
				}
			}
			else if (markerPeriod == MarkerPeriods.Century)
			{
				int year = 100*(s.Year/100);
				DateTime s1 = new DateTime(Math.Max(1,year), 1, 1);

				while(s1.Ticks <stripe.EndTicks)
				{
					if (s1.Ticks>=stripe.StartTicks) markers.Add(s1.Ticks);
					s1 = s1.AddYears(100);
				}
			}

			return (long[])markers.ToArray(typeof(System.Int64));
		}


		private void PaintGrid(Graphics g)
		{
			//Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;

			float sizeInPixels = this.PointsToPixels(markerFont.SizeInPoints);
			Font displayFont = new Font(markerFont.FontFamily, sizeInPixels*g.PageScale, markerFont.Style, GraphicsUnit.Pixel);

			for (int i=0;i<stripes.Length;i++)
			{
				Stripe s = stripes[i];

				Rectangle gridArea = new Rectangle(s.Area.Left, s.Area.Top + 4, s.Area.Width, s.Area.Height - 8);
				g.DrawRectangle(new Pen(markerColor, 1), gridArea);

				long[] markers = CalculateMarkers(s);

				for (int j=0;j<markers.Length;j++)
				{
					DateTime markerDate = new DateTime(markers[j]);
					double ratio = (markers[j] - s.StartTicks) / (double)(s.EndTicks - s.StartTicks);
					int xPosition = s.Area.Left + (int)(s.Area.Width*ratio);

					g.DrawLine(new Pen(markerColor, 1), xPosition, gridArea.Top, xPosition, gridArea.Bottom);

					string markerString = markerDate.ToString(markerDateFormat);
					
					double availableWidth = 0;
					if (j<markers.Length-1)
					{
						availableWidth = gridArea.Width * ((markers[j+1]-markers[j]) / (double)(s.EndTicks - s.StartTicks));
					}
					else
					{
						availableWidth = gridArea.Right - xPosition;
					}

					float markerStringWidth = g.MeasureString(markerString, displayFont).Width; 

					if (markerStringWidth < availableWidth)
					{
						// draw horizontal
						g.DrawString(markerString, displayFont, new SolidBrush(markerColor), new RectangleF(xPosition + 1, gridArea.Top + 1, gridArea.Right- xPosition, 20));
					}
					else if (availableWidth > markerFont.Height)
					{
						// draw vertical
						StringFormat sf = new StringFormat();
						sf.FormatFlags = StringFormatFlags.DirectionVertical;
						g.DrawString(markerString, displayFont, new SolidBrush(markerColor), new RectangleF(xPosition + 1, gridArea.Top + 1,  20, gridArea.Height), sf);
					}


				}
			}
		}

		private void PaintLabels(Graphics g)
		{
			float sizeInPixels = this.PointsToPixels(markerFont.SizeInPoints);
			Font displayFont = new Font(markerFont.FontFamily, sizeInPixels*g.PageScale, markerFont.Style, GraphicsUnit.Pixel);

			for (int i=0;i<stripes.Length;i++)
			{
				Stripe s = stripes[i];

				long[] markers = CalculateMarkers(s);

				for (int j=0;j<markers.Length;j++)
				{
					DateTime markerDate = new DateTime(markers[j]);
					double ratio = (markers[j] - s.StartTicks) / (double)(s.EndTicks - s.StartTicks);
					int xPosition = s.Area.Left + (int)(s.Area.Width*ratio);
					
					string markerString = markerDate.ToString(markerDateFormat);
					
					double availableWidth = 0;
					if (j<markers.Length-1)
					{
						availableWidth = s.Area.Width * ((markers[j+1]-markers[j]) / (double)(s.EndTicks - s.StartTicks));
					}
					else
					{
						availableWidth = s.Area.Right - xPosition;
					}

					float markerStringWidth = g.MeasureString(markerString, displayFont).Width; 

					if (markerStringWidth < availableWidth)
					{
						// draw horizontal
						g.DrawLine(new Pen(markerColor, 1), xPosition, s.VerticalCenter - stripeSize/2 - 5, xPosition, s.VerticalCenter - stripeSize/2);
						g.DrawString(markerString, displayFont, new SolidBrush(markerColor), new RectangleF(xPosition + 1, s.VerticalCenter - stripeSize/2 - 1 - markerFont.Height, s.Area.Right- xPosition, 20));
					}
					else if (availableWidth > markerFont.Height)
					{
						g.DrawLine(new Pen(markerColor, 1), xPosition, s.VerticalCenter - stripeSize/2 - 5, xPosition, s.VerticalCenter - stripeSize/2);
						// draw vertical
						StringFormat sf = new StringFormat();
						sf.FormatFlags = StringFormatFlags.DirectionVertical;
						g.DrawString(markerString, displayFont, new SolidBrush(markerColor), new RectangleF(xPosition + 1,  s.VerticalCenter - stripeSize/2 - 1 - markerStringWidth,  20, s.Area.Height), sf);
					}


				}
			}
		}

		private void PaintStripes(Graphics g)
		{
			if (stripeStyle == StripeStyles.None)
				return;
			else
			{
				for (int i=0;i<stripes.Length;i++)
				{
					Stripe s = stripes[i];
			
					Point[] stripeCurve = stripes[i].GetStripeCurve(stripeStyle);
					Brush bgBrush = paintTransparent ? new SolidBrush(ShiftColor(stripeColor, 192)) : new SolidBrush(stripeColor); 

                    g.SetClip(s.Area);
					g.FillClosedCurve(bgBrush, stripeCurve, System.Drawing.Drawing2D.FillMode.Alternate, 0.0f);
					g.DrawClosedCurve(Pens.Black, stripeCurve, 0.0f, System.Drawing.Drawing2D.FillMode.Alternate);
					g.ResetClip();
				}
			}
		}

		private void PaintTimelinePeriodText(Graphics g)
		{
			for (int i=0;i<periodPositions.Count;i++)
			{
				Rectangle position = (Rectangle)periodPositions[i];
				TimelinePeriod period = periods[i];

				float sizeInPixels = this.PointsToPixels(period.Font.SizeInPoints);
				Font displayFont = new Font(period.Font.FontFamily, sizeInPixels*g.PageScale, period.Font.Style, GraphicsUnit.Pixel);

				SizeF textSize = g.MeasureString(period.Text, displayFont);

				if (position.X!=0 && position.Y!=0)
				{
					float x = 0;
					float y = 0;

					switch (period.TextPosition)
					{
						case TimelinePeriod.Positions.None:
							continue;

						case TimelinePeriod.Positions.Right:
							x = position.Width + 1;
							y = position.Height ;
							break;

						case TimelinePeriod.Positions.Left:
							x = position.X - textSize.Width - 1;							
							y = position.Y;
							break;

						case TimelinePeriod.Positions.Bottom:
							x = position.X;
							y = position.Y + period.MarkSize + 1;
							break;

						default:
							x = position.X;
							y = position.Y - period.Font.Height;
							break;

					}

				
					g.DrawString(period.Text, displayFont, Brushes.Black, x, y);
				}
			}
		}

		private void PaintTimelinePeriods(Graphics g)
		{
			periodPositions.Clear();
			for (int i=0;i<periods.Length;i++)
			{
				TimelinePeriod period = periods[i];

				if (useDates)
				{
					// using dates

					if (period.StartDate.Ticks>period.EndDate.Ticks)
					{
						periodPositions.Add(new Rectangle(0,0,0,0));
						continue;
					}

					if (period.StartDate.Ticks < startDate.Ticks && period.EndDate.Ticks > endDate.Ticks)
					{
						for (int j=0;j<stripes.Length;j++)
						{
							Rectangle markRectangle = new Rectangle(stripes[j].Area.Left, stripes[j].VerticalCenter + period.Offset, stripes[j].Area.Width, period.MarkSize);
							Brush bgBrush = paintTransparent ? new SolidBrush(ShiftColor(period.Color,192)) : new SolidBrush(period.Color);
							g.FillRectangle(bgBrush, markRectangle);
						}
						periodPositions.Add(new Rectangle(stripes[0].Area.Left, stripes[0].VerticalCenter + period.Offset, stripes[stripes.Length-1].Area.Left, stripes[stripes.Length-1].VerticalCenter + period.Offset));
						continue;
					}					

					int startStripe = this.FindDateStripe(period.StartDate);
					int endStripe = this.FindDateStripe(period.EndDate);

					if (startStripe==-1 && endStripe==-1)
					{
						periodPositions.Add(new Rectangle(0,0,0,0));
						continue;
					}
					else
					{
						if (startStripe==endStripe)
						{
							Stripe s = stripes[startStripe];
							double startRatio = (period.StartDate.Ticks - s.StartTicks) / (double)(s.EndTicks - s.StartTicks);
							double endRatio = (period.EndDate.Ticks - s.StartTicks) / (double)(s.EndTicks - s.StartTicks);
							int startPosition = s.Area.Left + (int)(s.Area.Width*startRatio);
							int endPosition = s.Area.Left + (int)(s.Area.Width*endRatio);

							Rectangle markRectangle = new Rectangle(startPosition, s.VerticalCenter + period.Offset, endPosition - startPosition, period.MarkSize);
							Brush bgBrush = paintTransparent ? new SolidBrush(ShiftColor(period.Color,192)) : new SolidBrush(period.Color);
							g.FillRectangle(bgBrush, markRectangle);

							periodPositions.Add(new Rectangle(startPosition, s.VerticalCenter + period.Offset, endPosition, s.VerticalCenter + period.Offset));
						}
						else if (startStripe==-1 && endStripe!=-1)
						{
							Rectangle positions = new Rectangle(stripes[0].Area.Left, stripes[0].VerticalCenter + period.Offset,0,0);
							for (int j=0;j<=endStripe;j++)
							{
								Stripe s = stripes[j];
								int startPosition = 0;
								int endPosition = 0;

								if (j==endStripe)
								{
									startPosition = s.Area.Left;									
									double endRatio = (period.EndDate.Ticks - s.StartTicks) / (double)(s.EndTicks - s.StartTicks);
									endPosition = s.Area.Left + (int)(s.Area.Width*endRatio);

									positions.Width = endPosition;
									positions.Height = stripes[j].VerticalCenter + period.Offset; 
								}
								else
								{
									startPosition = s.Area.Left;
									endPosition = s.Area.Right;
								}

								Rectangle markRectangle = new Rectangle(startPosition, s.VerticalCenter + period.Offset, endPosition - startPosition, period.MarkSize);
								Brush bgBrush = paintTransparent ? new SolidBrush(ShiftColor(period.Color,192)) : new SolidBrush(period.Color);
								g.FillRectangle(bgBrush, markRectangle);
							}
							periodPositions.Add(positions);
						}
						else if (startStripe!=-1 && endStripe==-1)
						{
							Rectangle positions = new Rectangle(0,0,stripes[stripes.Length-1].Area.Right, stripes[stripes.Length-1].VerticalCenter + period.Offset);
							for (int j=startStripe;j<stripes.Length;j++)
							{
								Stripe s = stripes[j];
								int startPosition = 0;
								int endPosition = 0;

								if (j==startStripe)
								{
									endPosition = s.Area.Right;									
									double startRatio = (period.StartDate.Ticks - s.StartTicks) / (double)(s.EndTicks - s.StartTicks);
									startPosition = s.Area.Left + (int)(s.Area.Width*startRatio);

									positions.X = startPosition;
									positions.Y = stripes[j].VerticalCenter + period.Offset;
								}
								else
								{
									startPosition = s.Area.Left;
									endPosition = s.Area.Right;
								}

								Rectangle markRectangle = new Rectangle(startPosition, s.VerticalCenter + period.Offset, endPosition - startPosition, period.MarkSize);
								Brush bgBrush = paintTransparent ? new SolidBrush(ShiftColor(period.Color,192)) : new SolidBrush(period.Color);
								g.FillRectangle(bgBrush, markRectangle);
							}
							periodPositions.Add(positions);
						}
						else if (startStripe!=-1 && endStripe!=-1)
						{
							Rectangle positions = new Rectangle(0,0,0,0);
							for (int j=startStripe;j<=endStripe;j++)
							{
								Stripe s = stripes[j];
								int startPosition = 0;
								int endPosition = 0;

								if (j==endStripe)
								{
									startPosition = s.Area.Left;									
									double endRatio = (period.EndDate.Ticks - s.StartTicks) / (double)(s.EndTicks - s.StartTicks);
									endPosition = s.Area.Left + (int)(s.Area.Width*endRatio);

									positions.Width = endPosition;
									positions.Height = stripes[j].VerticalCenter + period.Offset;
								}
								else if (j==startStripe)
								{
									endPosition = s.Area.Right;									
									double startRatio = (period.StartDate.Ticks - s.StartTicks) / (double)(s.EndTicks - s.StartTicks);
									startPosition = s.Area.Left + (int)(s.Area.Width*startRatio);

									positions.X = startPosition;
									positions.Y = stripes[j].VerticalCenter + period.Offset;
								}
								else
								{
									startPosition = s.Area.Left;
									endPosition = s.Area.Right;
								}

								Rectangle markRectangle = new Rectangle(startPosition, s.VerticalCenter + period.Offset, endPosition - startPosition, period.MarkSize);
								Brush bgBrush = paintTransparent ? new SolidBrush(ShiftColor(period.Color,192)) : new SolidBrush(period.Color);
								g.FillRectangle(bgBrush, markRectangle);
							}

							periodPositions.Add(positions);
						}
					}
				}
				else
				{
					// using values

					if (period.StartValue>period.EndValue)
					{
						periodPositions.Add(new Rectangle(0,0,0,0));
						continue;
					}

					if (period.StartValue < this.startValue && period.EndValue > this.endValue)
					{
						for (int j=0;j<stripes.Length;j++)
						{
							Rectangle markRectangle = new Rectangle(stripes[j].Area.Left, stripes[j].VerticalCenter + period.Offset, stripes[j].Area.Width, period.MarkSize);
							Brush bgBrush = paintTransparent ? new SolidBrush(ShiftColor(period.Color,192)) : new SolidBrush(period.Color);
							g.FillRectangle(bgBrush, markRectangle);
						}
						periodPositions.Add(new Rectangle(stripes[0].Area.Left, stripes[0].VerticalCenter + period.Offset, stripes[stripes.Length-1].Area.Left, stripes[stripes.Length-1].VerticalCenter + period.Offset));
						continue;
					}

					int startStripe = this.FindValueStripe(period.StartValue);
					int endStripe = this.FindValueStripe(period.EndValue);

					if (startStripe==-1 && endStripe==-1)
					{
						periodPositions.Add(new Rectangle(0,0,0,0));
						continue;
					}
					else
					{
						if (startStripe==endStripe)
						{
							Stripe s = stripes[startStripe];
							double startRatio = (period.StartValue - s.StartTicks) / (double)(s.EndTicks - s.StartTicks);
							double endRatio = (period.EndValue - s.StartTicks) / (double)(s.EndTicks - s.StartTicks);
							int startPosition = s.Area.Left + (int)(s.Area.Width*startRatio);
							int endPosition = s.Area.Left + (int)(s.Area.Width*endRatio);

							Rectangle markRectangle = new Rectangle(startPosition, s.VerticalCenter + period.Offset, endPosition - startPosition, period.MarkSize);
							Brush bgBrush = paintTransparent ? new SolidBrush(ShiftColor(period.Color,192)) : new SolidBrush(period.Color);
							g.FillRectangle(bgBrush, markRectangle);

							periodPositions.Add(new Rectangle(startPosition, s.VerticalCenter + period.Offset, endPosition, s.VerticalCenter + period.Offset));
						}
						else if (startStripe==-1 && endStripe!=-1)
						{
							Rectangle positions = new Rectangle(stripes[0].Area.Left, stripes[0].VerticalCenter + period.Offset,0,0);
							for (int j=0;j<=endStripe;j++)
							{
								Stripe s = stripes[j];
								int startPosition = 0;
								int endPosition = 0;

								if (j==endStripe)
								{
									startPosition = s.Area.Left;									
									double endRatio = (period.EndValue - s.StartTicks) / (double)(s.EndTicks - s.StartTicks);
									endPosition = s.Area.Left + (int)(s.Area.Width*endRatio);

									positions.Width = endPosition;
									positions.Height = stripes[j].VerticalCenter + period.Offset; 
								}
								else
								{
									startPosition = s.Area.Left;
									endPosition = s.Area.Right;
								}

								Rectangle markRectangle = new Rectangle(startPosition, s.VerticalCenter + period.Offset, endPosition - startPosition, period.MarkSize);
								Brush bgBrush = paintTransparent ? new SolidBrush(ShiftColor(period.Color,192)) : new SolidBrush(period.Color);
								g.FillRectangle(bgBrush, markRectangle);
							}
							periodPositions.Add(positions);
						}
						else if (startStripe!=-1 && endStripe==-1)
						{
							Rectangle positions = new Rectangle(0,0,stripes[stripes.Length-1].Area.Right, stripes[stripes.Length-1].VerticalCenter + period.Offset);
							for (int j=startStripe;j<stripes.Length;j++)
							{
								Stripe s = stripes[j];
								int startPosition = 0;
								int endPosition = 0;

								if (j==startStripe)
								{
									endPosition = s.Area.Right;									
									double startRatio = (period.StartValue - s.StartTicks) / (double)(s.EndTicks - s.StartTicks);
									startPosition = s.Area.Left + (int)(s.Area.Width*startRatio);

									positions.X = startPosition;
									positions.Y = stripes[j].VerticalCenter + period.Offset;
								}
								else
								{
									startPosition = s.Area.Left;
									endPosition = s.Area.Right;
								}

								Rectangle markRectangle = new Rectangle(startPosition, s.VerticalCenter + period.Offset, endPosition - startPosition, period.MarkSize);
								Brush bgBrush = paintTransparent ? new SolidBrush(ShiftColor(period.Color,192)) : new SolidBrush(period.Color);
								g.FillRectangle(bgBrush, markRectangle);
							}

							periodPositions.Add(positions);
						}
						else if (startStripe!=-1 && endStripe!=-1)
						{
							Rectangle positions = new Rectangle(0,0,0,0);
							for (int j=startStripe;j<=endStripe;j++)
							{
								Stripe s = stripes[j];
								int startPosition = 0;
								int endPosition = 0;

								if (j==endStripe)
								{
									startPosition = s.Area.Left;									
									double endRatio = (period.EndValue - s.StartTicks) / (double)(s.EndTicks - s.StartTicks);
									endPosition = s.Area.Left + (int)(s.Area.Width*endRatio);

									positions.Width = endPosition;
									positions.Height = stripes[j].VerticalCenter + period.Offset;
								}
								else if (j==startStripe)
								{
									endPosition = s.Area.Right;									
									double startRatio = (period.StartValue - s.StartTicks) / (double)(s.EndTicks - s.StartTicks);
									startPosition = s.Area.Left + (int)(s.Area.Width*startRatio);

									positions.X = startPosition;
									positions.Y = stripes[j].VerticalCenter + period.Offset;
								}
								else
								{
									startPosition = s.Area.Left;
									endPosition = s.Area.Right;
								}

								Rectangle markRectangle = new Rectangle(startPosition, s.VerticalCenter + period.Offset, endPosition - startPosition, period.MarkSize);
								Brush bgBrush = paintTransparent ? new SolidBrush(ShiftColor(period.Color,192)) : new SolidBrush(period.Color);
								g.FillRectangle(bgBrush, markRectangle);
							}

							periodPositions.Add(positions);
						}
					}
				}

			}

			PaintTimelinePeriodText(g);
		}

		private void PaintTimelinePointsConnectors(Graphics g)
		{
			for (int i=0;i<points.Length;i++)
			{
				TimelinePoint point = points[i];
				Stripe s = FindPointStripe(point);
				if (s != null)
				{
					int xPosition = 0;
					if (useDates)
					{
						double ratio = (point.Date.Ticks - s.StartTicks) / (double)(s.EndTicks - s.StartTicks);
						xPosition = s.Area.Left + (int)(s.Area.Width*ratio);
					}
					else
					{
						double ratio = (point.Value - s.StartTicks) / (double)(s.EndTicks - s.StartTicks);
						xPosition = s.Area.Left + (int)(s.Area.Width*ratio);
					}

					g.FillEllipse(Brushes.Black, xPosition-2, s.VerticalCenter-2, 4, 4);
					g.DrawLine(Pens.Black, xPosition, s.VerticalCenter, xPosition + point.BoxOffset.X , s.VerticalCenter + point.BoxOffset.Y); 

					if (point.ShowPicture && point.Picture != null)
					{
						if (point.PictureBorderWidth>0)
						{
							g.DrawLine(new Pen(point.PictureBorderColor,1), xPosition, s.VerticalCenter, xPosition + point.PictureOffset.X, s.VerticalCenter + point.PictureOffset.Y);
							//g.DrawRectangle(new Pen(point.PictureBorderColor,point.PictureBorderWidth), pictureRectangle);  
						}
					}
				}
				else
				{
					continue;
				}
			}	
		}

		private void PaintTimelinePoints(Graphics g)
		{
			for (int i=0;i<points.Length;i++)
			{
				TimelinePoint point = points[i];
				Stripe s = FindPointStripe(point);
				if (s != null)
				{
					int xPosition = 0;
					if (useDates)
					{
						double ratio = (point.Date.Ticks - s.StartTicks) / (double)(s.EndTicks - s.StartTicks);
						xPosition = s.Area.Left + (int)(s.Area.Width*ratio);
					}
					else
					{
						double ratio = (point.Value - s.StartTicks) / (double)(s.EndTicks - s.StartTicks);
						xPosition = s.Area.Left + (int)(s.Area.Width*ratio);
					}

					//g.FillEllipse(Brushes.Black, xPosition-2, s.VerticalCenter-2, 4, 4);
					//g.DrawLine(Pens.Black, xPosition, s.VerticalCenter, xPosition + point.BoxOffset.X , s.VerticalCenter + point.BoxOffset.Y); 

				
					Rectangle box = new Rectangle(xPosition + point.BoxOffset.X, s.VerticalCenter + point.BoxOffset.Y, point.BoxSize.Width, point.BoxSize.Height);


					if (point.BoxShadow)
					{
						g.ExcludeClip(box);
						Rectangle shadowRect = new Rectangle(box.Left + 4, box.Top + 4, box.Width, box.Height);
						g.FillRectangle(new SolidBrush(Color.Gray), shadowRect);
						g.ResetClip();					
					}

					Brush bgBrush = paintTransparent ? new SolidBrush(ShiftColor(point.BoxColor, 192)) : new SolidBrush(point.BoxColor); 
					g.FillRectangle(bgBrush, box);
					g.DrawRectangle(Pens.Black, box);

					float sizeInPixels = this.PointsToPixels(point.Font.SizeInPoints);
					Font displayFont = new Font(point.Font.FontFamily, sizeInPixels*g.PageScale, point.Font.Style, GraphicsUnit.Pixel);

					string boxString = (useDates && point.ShowDate) ? point.Date.ToString(point.DateFormat) + "\r\n" + point.Text : point.Text;
					Rectangle textBox = new Rectangle(box.Left+1, box.Top+1, box.Width-2, box.Height-2);
					g.DrawString(boxString, displayFont, new SolidBrush(point.BoxTextColor), textBox);
					
					if (point.ShowPicture && point.Picture != null)
					{
						
						Rectangle pictureRectangle = new Rectangle(xPosition + point.PictureOffset.X, s.VerticalCenter + point.PictureOffset.Y - point.Picture.Height, point.Picture.Width, point.Picture.Height);
						if (point.StretchPicture)
						{
							pictureRectangle = new Rectangle(xPosition + point.PictureOffset.X, s.VerticalCenter + point.PictureOffset.Y - point.PictureSize.Height, point.PictureSize.Width, point.PictureSize.Height);

							if (point.BoxShadow)
							{
								g.ExcludeClip(pictureRectangle);
								Rectangle shadowRect = new Rectangle(pictureRectangle.Left + 3, pictureRectangle.Top + 3, pictureRectangle.Width, pictureRectangle.Height);
								g.FillRectangle(new SolidBrush(Color.Gray), shadowRect);
								g.ResetClip();
							}
							g.DrawImage(point.Picture,pictureRectangle,0,0,point.Picture.Width,point.Picture.Height,GraphicsUnit.Pixel);
						}
						else
						{
							if (point.BoxShadow)
							{
								g.ExcludeClip(pictureRectangle);
								Rectangle shadowRect = new Rectangle(pictureRectangle.Left + 4, pictureRectangle.Top + 4, pictureRectangle.Width, pictureRectangle.Height);
								g.FillRectangle(new SolidBrush(Color.Gray), shadowRect);
								g.ResetClip();
								
							}

							g.DrawImage(point.Picture,pictureRectangle,0,0,pictureRectangle.Width,pictureRectangle.Height,GraphicsUnit.Pixel);
						}

	
						if (point.PictureBorderWidth>0)
						{
							//g.DrawLine(new Pen(point.PictureBorderColor,1), xPosition, s.VerticalCenter, xPosition + point.PictureOffset.X, s.VerticalCenter + point.PictureOffset.Y);
							g.DrawRectangle(new Pen(point.PictureBorderColor,point.PictureBorderWidth), pictureRectangle);  
						}
					}

				}
				else
				{
					continue;
				}
			}	
		}

		private Stripe FindPointStripe(TimelinePoint point)
		{
			if (useDates)
			{
				for (int i=0;i<stripes.Length;i++)
				{
					if (stripes[i].EndTicks > point.Date.Ticks && stripes[i].StartTicks <= point.Date.Ticks)
					{
						return stripes[i];
					}
				}
				return null;
			}
			else
			{
				for (int i=0;i<stripes.Length;i++)
				{
					if (stripes[i].EndTicks > point.Value && stripes[i].StartTicks <= point.Value)
					{
						return stripes[i];
					}
				}
				return null;
			}
		}

		private int FindDateStripe(DateTime date)
		{

			for (int i=0;i<stripes.Length;i++)
			{
				if (stripes[i].EndTicks > date.Ticks && stripes[i].StartTicks <= date.Ticks)
				{
					return i;
				}
			}
			return -1;		
		}

		private int FindValueStripe(long theValue)
		{

			for (int i=0;i<stripes.Length;i++)
			{
				if (stripes[i].EndTicks > theValue && stripes[i].StartTicks <= theValue)
				{
					return i;
				}
			}
			return -1;		
		}

	}

	internal class Stripe
	{
		private Rectangle area;
		private long startTicks = 0;
		private long endTicks = 0;

		private int arrowSemiWidth = 10;

		internal Stripe(Rectangle area)
		{
			this.area = new Rectangle(area.Location, area.Size);
		}

		internal Rectangle Area
		{
			get {return area;}
		}

		internal int VerticalCenter
		{
			get {return area.Top + area.Height/2;}
		}

		internal long StartTicks
		{
			get {return startTicks;}
			set {startTicks = value;}
		}

		internal long EndTicks
		{
			get {return endTicks;}
			set {endTicks = value;}
		}

		internal int StripeSize
		{
			set {arrowSemiWidth = value/2;}
		}


		internal Point[] GetStripeCurve(Timeline.StripeStyles style)
		{
			Point[] arrowPoints = null;
			switch (style)
			{
				case Timeline.StripeStyles.Arrow:

					int arrowSize = Math.Min(30, arrowSemiWidth*2);
					arrowPoints = new Point[7];
					arrowPoints[0] = new Point(area.Left, this.VerticalCenter - arrowSemiWidth);
					arrowPoints[1] = new Point(area.Right - arrowSize, this.VerticalCenter - arrowSemiWidth);
					arrowPoints[2] = new Point(area.Right - arrowSize, this.VerticalCenter - 2*arrowSemiWidth);
					arrowPoints[3] = new Point(area.Right, this.VerticalCenter);
					arrowPoints[4] = new Point(area.Right - arrowSize, this.VerticalCenter + 2*arrowSemiWidth);
					arrowPoints[5] = new Point(area.Right - arrowSize, this.VerticalCenter + arrowSemiWidth);
					arrowPoints[6] = new Point(area.Left, this.VerticalCenter + arrowSemiWidth);
					break;

				case Timeline.StripeStyles.Line:
					arrowPoints = new Point[4];
					arrowPoints[0] = new Point(area.Left, this.VerticalCenter - arrowSemiWidth);
					arrowPoints[1] = new Point(area.Right, this.VerticalCenter - arrowSemiWidth);
					arrowPoints[2] = new Point(area.Right, this.VerticalCenter + arrowSemiWidth);
					arrowPoints[3] = new Point(area.Left, this.VerticalCenter + arrowSemiWidth);
					break;

				default :

					arrowPoints = new Point[]{};
					break;
			}

			return arrowPoints;
		}
	}

	/// <summary>
	/// Class that holds the event displayed in Timeline element.
	/// </summary>
	public class TimelinePoint : ICloneable
	{
		private long val = 0;
		private DateTime date = new DateTime(2006, 1, 15);
		private Font font = new Font("Tahoma", 8);

		private bool showDate = true;
		private string dateFormat = "dd/MM/yyyy";
		private string text = "Timepoint Description";
		
		private Point boxOffset = new Point(0, 20);
		private Size boxSize = new Size(120, 60);		
		private Color boxColor = Color.White;
		private Color boxTextColor = Color.Black;
		private bool boxShadow = true;

		private Image image = null;
		private string imageFile = String.Empty;
		private bool showPicture = true;
		private int picBorderWidth = 1;
		private Color picBorderColor = Color.Black;
		private Size pictureSize = new Size(80, 100);
		private bool stretchPicture = true;
		private Point pictureOffset = new Point(20, -20);


		/// <summary>
		/// Creates new instance of TimlinePoint
		/// </summary>
		public TimelinePoint()
		{
			date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
		}

		/// <summary>
		/// Gets or sets timepoint label text.
		/// </summary>
		[Category("Display"),Editor(typeof(Com.Delta.PrintManager.Engine.Editors.PlainTextEditor), typeof(UITypeEditor)), Description("Gets or sets timepoint label text."), DefaultValue("Timepoint Description")]
		public string Text
		{
			get {return text;}
			set {text = value;}
		}

		/// <summary>
		/// Gets or sets timepoint display font.
		/// </summary>
		[Category("Display"), Description("Gets or sets timepoint display font."), DefaultValue(typeof(System.Drawing.Font),"Tahoma, 8pt")]
		public Font Font
		{
			get {return font;}
			set {font = value;}
		}

		/// <summary>
		/// Gets or sets timepoint value. This value is used if Timeline.UseDates is set to false.
		/// </summary>
		[Category("Data"), Description("Gets or sets timepoint value. This value is used if Timeline.UseDates is set to false."), DefaultValue(0l)]
		public long Value
		{
			get {return val;}
			set {val = value;}
		}

		/// <summary>
		/// Gets or sets timepoint date. This value is used if Timeline.UseDates is set to true.
		/// </summary>
		[Category("Data"), Editor(typeof(Com.Delta.PrintManager.Engine.Editors.DateEditor), typeof(UITypeEditor)), Description("Gets or sets timepoint date. This value is used if Timeline.UseDates is set to true.")]
		public DateTime Date
		{
			get {return date;}
			set {date = value;}
		}

		/// <summary>
		/// Box offset relative to timeline.
		/// </summary>
		[Category("Display"), Description("Box offset relative to timeline."), DefaultValue(typeof(System.Drawing.Point),"0, 20")]
		public Point BoxOffset
		{
			get {return boxOffset;}
			set {boxOffset = value;}
		}

		/// <summary>
		/// Gets or sets timepoint label size.
		/// </summary>
		[Category("Display"), Description("Gets or sets timepoint label size."), DefaultValue(typeof(System.Drawing.Size),"120, 60")]
		public Size BoxSize
		{
			get {return boxSize;}
			set 
			{
				boxSize.Width = Math.Max(0, value.Width);
				boxSize.Height = Math.Max(0, value.Height);
			}
		}

		/// <summary>
		/// Show textbox shadow.
		/// </summary>
		[Category("Display"), Description("Show textbox shadow"), DefaultValue(true)]
		public bool BoxShadow
		{
			get {return boxShadow;}
			set {boxShadow = value;} 
		}

		/// <summary>
		/// Gets or sets background color for timepoint display label.
		/// </summary>
		[Category("Display"), Description("Gets or sets background color for timepoint display label."), DefaultValue(typeof(System.Drawing.Color),"White")]
		public Color BoxColor
		{
			get {return boxColor;}
			set {boxColor = value;}
		}

		/// <summary>
		/// Gets or sets foreground color for timepoint display label.
		/// </summary>
		[Category("Display"), Description("Gets or sets foreground color for timepoint display label."), DefaultValue(typeof(System.Drawing.Color),"Black")]
		public Color BoxTextColor
		{
			get {return boxTextColor;}
			set {boxTextColor = value;}
		}

		/// <summary>
		/// Gets or sets whether data will be shown in label.
		/// </summary>
		[Category("Display"), Description("Gets or sets whether data will be shown in label."), DefaultValue(true)]
		public bool ShowDate
		{
			get {return showDate;}
			set {showDate = value;}
		}

		/// <summary>
		/// Gets or sets timepoint date display format.
		/// </summary>
		[Category("Display"), Description("Gets or sets timepoint date display format."), DefaultValue("dd/MM/yyyy")]
		public string DateFormat
		{
			get {return dateFormat;}
			set {dateFormat = value;}
		}


		/// <summary>
		/// Picture file (relative to document root) for loading timepoint picture.
		/// </summary>
		[Category("Picture"), Description("Picture file (relative to document root) for loading timepoint picture.")]
		public string PictureFile
		{
			get {return imageFile;}
			set {imageFile = value;}
		}

		internal Image Picture
		{
			get {return image;}
			set {image = value;}
		}

		/// <summary>
		/// Gets or sets whether timepoint picture will be shown.
		/// </summary>
		[Category("Picture"), Description("Gets or sets whether timepoint picture will be shown."), DefaultValue(true)]
		public bool ShowPicture
		{
			get {return showPicture;}
			set {showPicture = value;}
		}

		/// <summary>
		/// Border width for display picture.
		/// </summary>
		[Category("Picture"), Description("Border width for display picture."), DefaultValue(1)]
		public int PictureBorderWidth
		{
			get {return picBorderWidth;}
			set {picBorderWidth = Math.Max(0,value);}
		}


		/// <summary>
		/// Border color of display picture.
		/// </summary>
		[Category("Picture"), Description("Border color of display picture."), DefaultValue(typeof(System.Drawing.Color),"Black")]
		public Color PictureBorderColor
		{
			get {return picBorderColor;}
			set {picBorderColor = value;}
		}

		/// <summary>
		/// Picture display size. Meaningfull only if PictureStretch is set to true.
		/// </summary>
		[Category("Picture"), Description("Picture display size. Meaningfull only if PictureStretch is set to true."), DefaultValue(typeof(System.Drawing.Size),"80, 100")]
		public Size PictureSize
		{
			get {return pictureSize;}
			set 
			{
				pictureSize.Width = Math.Max(0, value.Width);
				pictureSize.Height = Math.Max(0, value.Height);
			}
		}

		/// <summary>
		/// Picture drawing offset relative to timeline.
		/// </summary>
		[Category("Picture"), Description("Picture drawing offset relative to timeline."), DefaultValue(typeof(System.Drawing.Point),"20, -20")]
		public Point PictureOffset
		{
			get {return pictureOffset;}
			set {pictureOffset = value;}
		}


		/// <summary>
		/// If true, picture will be drawn within PictureSize rectangle ; otherwise, picture is drawn in it's full size.
		/// </summary>
		[Category("Picture"), Description("If true, picture will be drawn within PictureSize rectangle ; otherwise, picture is drawn in it's full size."), DefaultValue(true)]
		public bool StretchPicture
		{
			get {return stretchPicture;}
			set {stretchPicture = value;}
		}


		/// <summary>
		/// String representation of the TimelinePoint.
		/// </summary>
		public override string ToString()
		{			
			return "Timepoint (Value:" + val.ToString() + " Date:"+ date.ToString("dd.MM.yyyy") + ")";
		}

		#region ICloneable Members

		public object Clone()
		{
			TimelinePoint tmp = new TimelinePoint();
			tmp.BoxColor = this.BoxColor;
			tmp.BoxOffset = this.BoxOffset;
			tmp.BoxSize = this.BoxSize;
			tmp.BoxTextColor = this.BoxTextColor;
			tmp.Date = this.Date;
			tmp.DateFormat = this.DateFormat;
			tmp.Font =this.Font;
			tmp.PictureBorderColor = this.PictureBorderColor;
			tmp.PictureBorderWidth = this.PictureBorderWidth;
			tmp.PictureFile = this.PictureFile;
			tmp.PictureOffset = this.PictureOffset;
			tmp.PictureSize = this.PictureSize;
			tmp.ShowDate = this.ShowDate;
			tmp.ShowPicture = this.ShowPicture;
			tmp.StretchPicture = this.StretchPicture;
			tmp.Text = this.Text;
			tmp.Value = this.Value;
			
			return tmp;
		}

		#endregion
	}

	/// <summary>
	/// Class that holds the period displayed in Timeline element.
	/// </summary>
	public class TimelinePeriod : ICloneable
	{
		/// <summary>
		/// Enumeration of possible positions for displaying marker periods. 
		/// </summary>
		public enum Positions {None, Left, Right, Top, Bottom};

		private DateTime startDate = new DateTime(2006, 4, 1);
		private DateTime endDate = new DateTime(2006, 8, 1);
		private long startValue = 0;
		private long endValue = 10;
		private string text = "Period Description";
		private int offset = -30;
		private int markSize = 12;
		private Color color = Color.Navy;
		private Font font = new Font("Tahoma", 8);
		private Positions textPosition = Positions.Top;

		/// <summary>
		/// Period mark offset relative to stripe.
		/// </summary>
		[Category("Display"), Description("Period mark offset relative to stripe."), DefaultValue(-30)]
		public int Offset
		{
			get {return offset;}
			set {offset = value;}
		}

		/// <summary>
		/// Period mark size.
		/// </summary>
		[Category("Display"), Description("Period mark size."), DefaultValue(12)]
		public int MarkSize
		{
			get {return markSize;}
			set {markSize = Math.Max(0,value);}
		}

		/// <summary>
		/// Period text font
		/// </summary>
		[Category("Display"), Description("Period text font."), DefaultValue(typeof(System.Drawing.Font),"Tahoma, 8pt")]
		public Font Font
		{
			get {return font;}
			set {font = value;}
		}

		/// <summary>
		/// Period text position relative to period marker
		/// </summary>
		[Category("Display"), Description("Period text position relative to period marker."), DefaultValue(Positions.Top)]
		public Positions TextPosition
		{
			get {return textPosition;}
			set {textPosition = value;}
		}

		/// <summary>
		/// Period mark color.
		/// </summary>
		[Category("Display"), Description("Period mark color."), DefaultValue(typeof(System.Drawing.Color),"Navy")]
		public Color Color
		{
			get {return color;}
			set {color = value;}
		}

		/// <summary>
		/// Gets or sets period start date. This value is used if Timeline.UseDates is set to true.
		/// </summary>
		[Category("Data"), Editor(typeof(Com.Delta.PrintManager.Engine.Editors.DateEditor), typeof(UITypeEditor)), Description("Gets or sets period start date. This value is used if Timeline.UseDates is set to true."), DefaultValue(typeof(System.DateTime),"4/1/2006")]
		public DateTime StartDate
		{
			get {return startDate;}
			set {startDate = value;}
		}

		/// <summary>
		/// Gets or sets period end date. This value is used if Timeline.UseDates is set to true.
		/// </summary>
		[Category("Data"), Editor(typeof(Com.Delta.PrintManager.Engine.Editors.DateEditor), typeof(UITypeEditor)), Description("Gets or sets period end date. This value is used if Timeline.UseDates is set to true."), DefaultValue(typeof(System.DateTime),"8/1/2006")]
		public DateTime EndDate
		{
			get {return endDate;}
			set {endDate = value;}
		}

		/// <summary>
		/// Gets or sets period start value. This value is used if Timeline.UseDates is set to false.
		/// </summary>
		[Category("Data"), Description("Gets or sets period start value. This value is used if Timeline.UseDates is set to false."), DefaultValue(0l)]
		public long StartValue
		{
			get {return startValue;}
			set {startValue = value;}
		}

		/// <summary>
		/// Gets or sets period end value. This value is used if Timeline.UseDates is set to false.
		/// </summary>
		[Category("Data"), Description("Gets or sets period end value. This value is used if Timeline.UseDates is set to false."), DefaultValue(10l)]
		public long EndValue
		{
			get {return endValue;}
			set {endValue = value;}
		}

		/// <summary>
		/// Gets or sets period label text.
		/// </summary>
		[Category("Display"), Description("Gets or sets period label text."), DefaultValue("Period Description")]
		public string Text
		{
			get {return text;}
			set {text = value;}
		}

		#region ICloneable Members

		public object Clone()
		{
			TimelinePeriod tmp = new TimelinePeriod();

			tmp.text = this.Text;
			tmp.startDate = this.StartDate;
			tmp.endDate = this.EndDate;
			tmp.startValue = this.StartValue;
			tmp.endValue = this.EndValue;
			tmp.offset = this.Offset;
			tmp.color = this.Color;
			tmp.markSize = this.MarkSize;
			tmp.font = this.Font;
			tmp.textPosition = this.TextPosition;

			return tmp;
		}

		#endregion
	}


}
