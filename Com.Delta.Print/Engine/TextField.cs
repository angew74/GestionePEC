

using System;
using System.Drawing;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;
using System.Xml;



namespace Com.Delta.Print.Engine
{
	/// <summary>
	/// Report element for displaying textual data.
	/// </summary>
	[DefaultProperty("Text")]
	public sealed class TextField : ICustomPaint
	{
		
		#region Declarations
	
		private string theText = "Sample text";
		private Font theFont = new Font("Tahoma", 10);
		private Font paintingFont;
		private Font measureFont;
		private Color foregroundColor = Color.Black;
		private Color backgroundColor = Color.Transparent;
		private Color borderColor = Color.Black;
		private int borderWidth = 0;
		private int innerPadding = 2;
		private int lineSpacing = 3;
		private Orientation orientation = Orientation.Horizontal;
		private OverflowStyle overflowStyle = OverflowStyle.Ignore;

		private string currentText = string.Empty;
		private string residualText = string.Empty;
		private int startPosition = 0;

		/// <summary>
		/// Enumeration of possible horizontal alignments for the TextField text
		/// </summary>
		public enum TextAlignmentType
		{
			/// <summary>Text is aligned to the left of the TextField</summary>
			Left = 1,
			/// <summary>Text is aligned to the center of the TextField</summary>
			Center,
			/// <summary>Text is aligned to the right of the TextField</summary>
			Right,
			/// <summary>Text is aligned to both sides of the TextField</summary>
			Justified,
			/// <summary>The text is not aligned and will be displayed normally</summary>
			None
		};
		
		/// <summary>
		/// Enumeration of possible horizontal alignments for the TextField text
		/// </summary>
		public enum TextVerticalAlignmentType
		{
			/// <summary>Text is aligned to the top of the TextField</summary>
			Top = 1,
			/// <summary>Text is aligned to the middle of the TextField</summary>
			Middle,
			/// <summary>Text is aligned to the bottom of the TextField</summary>
			Bottom,
			/// <summary>The text is not aligned and will be displayed normally</summary>
			None
		};

		/// <summary>
		/// Enumeration of possible overflow text handling for the TextField text
		/// </summary>
		public enum OverflowStyle 
		{
			/// <summary>Ignore overflown text.</summary>
			Ignore=0, 
			/// <summary>Display overflown text.</summary>
			Display
		};

		

		/// <summary>
		/// Enumeration of possible text orientations for the TextField text
		/// </summary>
		public enum Orientation 
		{
			/// <summary>Text is drawn horizontally.</summary>
			Horizontal=0,
			/// <summary>Text is drawn vertically.</summary>
			Vertical
		};
		
		private TextAlignmentType textAlignment = TextAlignmentType.Left;
		private TextVerticalAlignmentType textVerticalAlignment = TextVerticalAlignmentType.Top;
		
		
		private ArrayList theLines;
		private ArrayList lastLines;

		#endregion

		#region Public Properties

		/// <summary>
		/// Justification of text lines.
		/// </summary>
		[Browsable(false)]
		internal bool[] Justification
		{
			get 
			{
				if (lastLines==null)
					return null;
				else
					return (bool[])lastLines.ToArray(typeof(bool));
			}
		}

		/// <summary>
		/// Text currently contained in theLines object
		/// </summary>
		[Browsable(false)]
		internal string CurrentText
		{
			get {return currentText;}
		}

		/// <summary>
		/// Gets/Sets the background color for the TextField
		/// </summary>
		/// <remarks>This property sets the background color of the TextField object. This can be any color
		/// from the System.Drawing.Color structure
		/// </remarks>
		[Category("Appearance"), DefaultValue(typeof(System.Drawing.Color),"Transparent")]
		public Color BackgroundColor
		{
			get {return backgroundColor;}
			set {backgroundColor = value;}
		}


		/// <summary>
		/// Gets/Sets the border color for the TextField
		/// </summary>
		/// <remarks>This property sets the border color of the TextField object. This can be any color
		/// from the System.Drawing.Color structure
		/// </remarks>
		[Category("Appearance"), DefaultValue(typeof(System.Drawing.Color),"Black")]
		public Color BorderColor
		{
			get {return borderColor;}
			set {borderColor = value;}
		}


		/// <summary>
		/// Gets/Sets the border width for the TextField
		/// </summary>
		/// <remarks>
		/// BorderWidth of the TextField. If this is set to zero, then the border is invisible
		/// </remarks>
		[Category("Appearance"), DefaultValue(1)]
		public int BorderWidth
		{
			get {return borderWidth;}
			set {borderWidth = Math.Max(0,value);}
		}


		/// <summary>
		/// Gets/Sets the font used in the TextField object
		/// </summary>
		[Category("Appearance"), DefaultValue(typeof(System.Drawing.Font),"Tahoma, 9.75pt")]
		public Font Font
		{
			get {return theFont;}
			set 
			{
				theFont = value; 
				paintingFont = null;
			}
		}


		/// <summary>
		/// Gets/Sets the foreground color for the TextField
		/// </summary>
		/// <remarks>This property sets the foreground color of the TextField object. This can be any color
		/// from the System.Drawing.Color structure
		/// </remarks>
		[Category("Appearance"), DefaultValue(typeof(System.Drawing.Color),"Black")]
		public Color ForegroundColor
		{
			get {return foregroundColor;}
			set {foregroundColor = value;}
		}


		


		/// <summary>
		/// Gets/Sets text to be displayed in the TextField object.
		/// </summary>
		[Category("Data")]
		[Editor(typeof(Com.Delta.Print.Engine.Editors.PlainTextEditor), typeof(UITypeEditor)), Description("Text content of the Text Field.")]
		public string Text
		{
			get 
			{
				string displayText = theText.Length>25 ? theText.Substring(0,25)+"..." : theText ;				
				return displayText.Replace("\r\n","  ");
			}
			set {theText = value;}
		}

		/// <summary>
		/// Gets/Sets text to be displayed in the TextField object.
		/// </summary>
		[Category("Data")]
		[Description("Line spacing (in pixels)."), DefaultValue(3)]
		public int Spacing
		{
			get {return lineSpacing;}
			set {lineSpacing = Math.Max(0,value);}
		}

		/// <summary>
		/// Gets/Sets text to be displayed in the TextField object.
		/// </summary>
		[Category("Data")]
		[Description("Inner padding (in pixels) for TextField."), DefaultValue(2)]
		public int Padding
		{
			get {return innerPadding;}
			set {innerPadding = Math.Max(0,value);}
		}


		/// <summary>
		/// Gets/Sets text to be displayed in the TextField object.
		/// </summary>
		[Category("Data")]
		[DefaultValue(Orientation.Horizontal), Description("Inner padding (in pixels) for TextField.")]
		public Orientation TextOrientation
		{
			get {return orientation;}
			set {orientation = value;}
		}

		/// <summary>
		/// Gets/Sets overflow text handling for this object.
		/// </summary>
		[Category("Data")]
		[DefaultValue(OverflowStyle.Ignore), Description("How to handle oveflow text. If set to Display, the whole text will be spread over pages. Option Ignore will paint text always from the start.")]
		public OverflowStyle OverflowTextHandling
		{
			get {return overflowStyle;}
			set {overflowStyle = value;}
		}
		

		/// <summary>
		///  Gets or sets the horizontal alignment of text in the ChartBox
		/// </summary>
		[Category("Appearance"), DefaultValue(TextAlignmentType.Left), Description("Horizontal alignment of text in the TextField object, relative to borders.")]
		public TextAlignmentType TextAlignment
		{
			get {return textAlignment;}
			set {textAlignment = value;}
		}


		/// <summary>
		///  Gets or sets the vertical alignment of text in the ChartBox
		/// </summary>
		[Category("Appearance"), DefaultValue(TextVerticalAlignmentType.Top), Description("Horizontal vertical of text in the TextField object, relative to borders.")]
		public TextVerticalAlignmentType TextVerticalAlignment
		{
			get {return textVerticalAlignment;}
			set {textVerticalAlignment = value;}
		}



		#endregion
		
		#region Public Overrides

		/// <summary>
		/// Gets the string representation of report element.
		/// </summary>
		public override string ToString()
		{
			return "Text field [" + this.Text + "]" ;
		}

		/// <summary>
		/// Gets or sets the name of the TextField element.
		/// </summary>
		[Category("Data"), Description("The name of TextField element.")]
		public override string Name
		{
			get {return name;}
			set {name = value;}
		}
	
		
		#endregion

		#region Public Functions

		/// <summary>
		/// Gets the TextField inner text.
		/// </summary>
		public string GetText()
		{
			return theText;
		}

		
		/// <summary>
		/// Sets the TextField inner text.
		/// </summary>
		public void SetText(string text)
		{
			theText = text;
		}

		#endregion

		#region Private Functions

		
		private void drawJustified(Graphics g,string text, float yPos, bool isLast,bool isIndented)
		{

            // Raffaele Russo - 12/12/2011 - Start
            Rectangle paintArea = new Rectangle(Bounds.X - rBounds.X, Bounds.Y - rBounds.Y, Bounds.Width, Bounds.Height);
            // Rectangle paintArea = new Rectangle(theRegion.X - rBounds.X, theRegion.Y - rBounds.Y, theRegion.Width, theRegion.Height);
            // Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;
            // Raffaele Russo - 12/12/2011 - End

			string txt = text.Replace("\t","     ");
			float indentSize = g.MeasureString("mmm",paintingFont).Width;

			float xPos = paintArea.X + innerPadding + (isIndented?indentSize:0);

			string[] theWords = txt.Split(new char[]{' '});

			float[] wordsWidths = new float[theWords.Length];

			float totalWordsWidth = 0;
			for (int i=0;i<theWords.Length;i++)
			{
				SizeF sf = g.MeasureString(theWords[i], paintingFont);
				wordsWidths[i] = sf.Width;
				totalWordsWidth += sf.Width;
			}

			float theOffset = 0;
			if (theWords.Length>1)
				theOffset = (paintArea.Width - 2*innerPadding - totalWordsWidth - (isIndented?indentSize:0) ) / (theWords.Length-1);

			if (isLast)
			{
				g.DrawString(txt, paintingFont,new SolidBrush(foregroundColor),xPos, yPos);	
			}
			else
			{
				for (int i=0;i<theWords.Length;i++)
				{

					if (i==0)
					{
						xPos = paintArea.X + innerPadding + (isIndented?indentSize:0);
					}
					else if(i==theWords.Length-1)
					{
						xPos = paintArea.Right - innerPadding - g.MeasureString(theWords[i], paintingFont).Width;
					}					
				
					g.DrawString(theWords[i], paintingFont,new SolidBrush(foregroundColor), xPos, yPos);							
					
					xPos += wordsWidths[i]+ theOffset;
				}
			}

			
		}

		private void drawJustifiedVertical(Graphics g,string text, float xPos,bool isLast,bool isIndented)
		{
            // Raffaele Russo - 12/12/2011 - Start
            Rectangle paintArea = new Rectangle(Bounds.X - rBounds.X, Bounds.Y - rBounds.Y, Bounds.Width, Bounds.Height);
            // Rectangle paintArea = new Rectangle(theRegion.X - rBounds.X, theRegion.Y - rBounds.Y, theRegion.Width, theRegion.Height);
            // Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;
            // Raffaele Russo - 12/12/2011 - End

			StringFormat sformat = new StringFormat(StringFormatFlags.DirectionVertical);
			
			string txt = text.Replace("\t","     ");
			float indentSize = g.MeasureString("mmm", paintingFont).Width;

			float yPos = paintArea.Top + innerPadding + (isIndented?indentSize:0);

			string[] theWords = txt.Split(new char[]{' '});

			float[] wordsWidths = new float[theWords.Length];

			float totalWordsWidth = 0;
			for (int i=0;i<theWords.Length;i++)
			{
				SizeF sf = g.MeasureString(theWords[i], paintingFont);
				wordsWidths[i] = sf.Width;
				totalWordsWidth += sf.Width;
			}

			float theOffset = 0;
			if (theWords.Length>1)
				theOffset = (paintArea.Height - 2*innerPadding - totalWordsWidth - (isIndented?indentSize:0) ) / (theWords.Length-1);

			if (isLast)
			{
				RectangleF r = new RectangleF(xPos, yPos, paintingFont.Height, paintArea.Height - 2*innerPadding);
				g.DrawString(txt, paintingFont, new SolidBrush(foregroundColor), r, sformat);	
			}
			else
			{
				for (int i=0;i<theWords.Length;i++)
				{

					if (i==0)
					{
						yPos = paintArea.Top + innerPadding + (isIndented?indentSize:0);
					}
					else if(i==theWords.Length-1)
					{
						yPos = paintArea.Bottom - innerPadding - g.MeasureString(theWords[i], paintingFont).Width;
					}					
				
					RectangleF r = new RectangleF(xPos, yPos, paintingFont.Height, paintArea.Height - 2*innerPadding);
					g.DrawString(theWords[i], paintingFont, new SolidBrush(foregroundColor), r, sformat);							
					
					yPos += wordsWidths[i]+ theOffset;
				}
			}

			
		}


		private void drawSimpleString(Graphics g, string txt, float yPos, bool isLast)
		{
            // Raffaele Russo - 12/12/2011 - Start
            Rectangle paintArea = new Rectangle(Bounds.X - rBounds.X, Bounds.Y - rBounds.Y, Bounds.Width, Bounds.Height);
            // Rectangle paintArea = new Rectangle(theRegion.X - rBounds.X, theRegion.Y - rBounds.Y, theRegion.Width, theRegion.Height);
            // Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;
            // Raffaele Russo - 12/12/2011 - End

			float xPos = 0;
			
			RectangleF lineRectangle = new RectangleF(paintArea.Left+innerPadding, yPos, paintArea.Width - 2*innerPadding, paintingFont.Height + lineSpacing);
			StringFormat sf = new StringFormat();



			switch (TextAlignment)
			{
				case TextAlignmentType.Center:
					sf.Alignment = StringAlignment.Center;					
					g.DrawString(txt, paintingFont, new SolidBrush(foregroundColor), lineRectangle, sf);	
					break;

				case TextAlignmentType.Right:
					sf.Alignment = StringAlignment.Far;								
					g.DrawString(txt.Trim(), paintingFont, new SolidBrush(foregroundColor), lineRectangle, sf);						
					break;

				case TextAlignmentType.Justified:
					if (txt.StartsWith("\t"))
					{
						drawJustified(g, txt.Trim(), yPos, isLast, true);
					}
					else
					{
						drawJustified(g, txt.TrimEnd(new char[]{' '}), yPos, isLast, false);	
					}
					break;

				default:
					xPos = paintArea.X + innerPadding;
					g.DrawString(txt, paintingFont, new SolidBrush(foregroundColor), new PointF(xPos, yPos));											
					break;

			}			
		}

		private void drawVertical(Graphics g, string txt, float xPos, bool isLast)
		{
            // Raffaele Russo - 12/12/2011 - Start
            Rectangle paintArea = new Rectangle(Bounds.X - rBounds.X, Bounds.Y - rBounds.Y, Bounds.Width, Bounds.Height);
            // Rectangle paintArea = new Rectangle(theRegion.X - rBounds.X, theRegion.Y - rBounds.Y, theRegion.Width, theRegion.Height);
            // Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;
            // Raffaele Russo - 12/12/2011 - End


			if (TextAlignment == TextAlignmentType.Justified)
			{
				if (txt.StartsWith("\t"))
				{
					drawJustifiedVertical(g, txt.Trim(), xPos, isLast, true);
				}
				else
				{
					drawJustifiedVertical(g, txt.TrimEnd(new char[]{' '}), xPos, isLast, false);	
				}
			}
			else
			{
				StringFormat sf = new StringFormat(StringFormatFlags.DirectionVertical);

				switch (TextAlignment)
				{
					case TextAlignmentType.Center:
						sf.Alignment = StringAlignment.Center;
						break;

					case TextAlignmentType.Right:
						sf.Alignment = StringAlignment.Far;					
						break;

					default:
						sf.Alignment = StringAlignment.Near;					
						break;
				}	

				Rectangle r = new Rectangle((int)xPos, paintArea.Y + innerPadding, paintingFont.Height, paintArea.Height - 2*innerPadding);
				g.DrawString(txt, paintingFont, new SolidBrush(foregroundColor), r, sf);	
			}
		}

		
		private void drawText(Graphics g)
		{			
			if (paintingFont==null)
			{		
				prepareFonts(g);
			}

			if (this.IsDynamic() && this.Section.Document.DesignMode)
			{
				CreateSubText(g, true, false, false);
			}
			startPainting(g);
		}

		private void prepareFonts(Graphics g)
		{
			float sizeInPixels = this.PointsToPixels(theFont.SizeInPoints);
			paintingFont = this.Section.Document.DesignMode ? new Font(theFont.FontFamily, sizeInPixels*g.PageScale, theFont.Style, GraphicsUnit.Pixel) : new Font(theFont.FontFamily, sizeInPixels*0.96f*g.PageScale, theFont.Style, GraphicsUnit.Pixel);
			float ratio = 96f/ this.Section.Document.GetGraphics().DpiX;			
			measureFont = new Font(theFont.FontFamily, theFont.SizeInPoints*ratio, theFont.Style);
			
		}

		


		/// <summary>
		/// Creates the text for display on a single page.
		/// </summary>
		internal bool CreateSubText(Graphics g, bool fromStart, bool resetPosition, bool forceResize)
		{	
			try
			{
				if (measureFont==null)						
					prepareFonts(g);

				Font msFont = new Font(measureFont.FontFamily, measureFont.SizeInPoints, measureFont.Style);
				//Font msFont = new Font(theFont.FontFamily, theFont.SizeInPoints, theFont.Style);

				theLines.Clear();
				lastLines.Clear();

				if (fromStart)
				{
					startPosition = 0;
				}
		
								
				int current = 0;

				ArrayList parameterPositions = new ArrayList();
				ArrayList parameterEnds = new ArrayList();
				string fullText = section.Document.DesignMode ? theText : this.ResolveParameterValues(residualText + theText.Substring(startPosition), parameterPositions, parameterEnds);
		

				StringBuilder buffer = new StringBuilder(fullText);

				StringBuilder line = new StringBuilder();
				int lastBlank = -1;

                // Raffaele Russo - 12/12/2011 - Start
                Rectangle r = new Rectangle(0,0,Bounds.Width,Bounds.Height);
                // Rectangle r = new Rectangle(0, 0, theRegion.Width, theRegion.Height);
				// Rectangle r = this.Section.Document.DesignMode ? theRegion : Bounds;
                // Raffaele Russo - 12/12/2011 - End

				int yPosition = innerPadding;
				if (orientation == Orientation.Vertical)
				{
                    // Raffaele Russo - 12/12/2011 - Start
                    r = new Rectangle(0, 0, Bounds.Height, Bounds.Width);
                    // r = new Rectangle(0, 0, theRegion.Height, theRegion.Width);
					// r = this.Section.Document.DesignMode ? new Rectangle(0, 0, theRegion.Height, theRegion.Width) : new Rectangle(0, 0, Bounds.Height, Bounds.Width);
                    // Raffaele Russo - 12/12/2011 - End
                }

				if (r.Width == 0 || r.Height == 0)
				{
					return false;
				}

				SizeF sf = g.MeasureString(line.ToString(), msFont);
				
				
				while ( yPosition + sf.Height <= (r.Height - innerPadding) && buffer.Length > current )
				{
					string nextChar = buffer.ToString(current,1);
					line.Append(nextChar);

																			
					int correction = 0;
					string parameterName = null;
					
					
					sf = this.section.Document.GetGraphics().MeasureString(line.ToString(), msFont);

					
					if (nextChar == "\r" && current+1<buffer.Length && buffer.ToString(current+1,1) == "\n" )
					{
						line.Remove(line.Length-1, 1);
						current += 1;
						theLines.Add(line.ToString());						
						lastLines.Add(true);
						yPosition += (int)sf.Height + lineSpacing;
						line.Remove(0, line.Length);
						lastBlank = -1;

					}
					else if (sf.Width > r.Width - 2*innerPadding)
					{					
						if (lastBlank == -1)
						{
							line.Remove(line.Length-1, 1);
							current--;
						}
						else
						{
							int blankOffset = line.Length - lastBlank - 1;
							if (blankOffset>0)
							{
								line.Remove(lastBlank+1, blankOffset);
								current = current - blankOffset;
							}
						}
				
						theLines.Add(line.ToString().TrimEnd(' '));							
						lastLines.Add(false);
						yPosition += (int)sf.Height + lineSpacing;
						line.Remove(0, line.Length);					
						lastBlank = -1;
						
					}
					else if (current == buffer.Length-1)
					{
						theLines.Add(line.ToString());						
						lastLines.Add(true);
						line.Remove(0, line.Length);
						lastBlank = -1;
					}
					else
					{
						if (nextChar.Equals(" "))
							lastBlank = line.Length - 1;
					} 

					++current;
				}


				if (current < buffer.Length)
				{
					
					int newPosition = 0;
					bool parameterBreak = false;

					
					int maxParameter = 0;
					int maxParameterPosition = 0;

					for(int i=0;i<parameterPositions.Count;i++)
					{
						Point p = (Point)parameterPositions[i];
						if (current>p.X && current<p.Y)
						{
							parameterBreak = true;
							newPosition = (int)parameterEnds[i];
							
							this.residualText = buffer.ToString().Substring(current, p.Y - current);
						}

						maxParameter = (int)parameterEnds[i];
						maxParameterPosition = p.Y;
					}

					if (parameterBreak)
					{
						startPosition = newPosition;
					}
					else
					{						
						if (current > residualText.Length)
						{
							//startPosition += (current - residualText.Length);
							

							startPosition += maxParameter + (current - maxParameterPosition) ;
							residualText = string.Empty;
						}
						else
						{
							this.residualText = residualText.Substring(current);
						}

					}
				}
				else
				{
					if (this.OverflowTextHandling == TextField.OverflowStyle.Ignore)
						startPosition = 0;
					else
						startPosition = resetPosition ? 0 : theText.Length;

					residualText = string.Empty;
				}
				

				StringBuilder currText = new StringBuilder();
				for (int i=0;i<theLines.Count;i++)
				{
					currText.Append(theLines[i].ToString());
					if (i<theLines.Count-1)
						currText.Append("\r\n");
				}
				currentText = currText.ToString();


				int lineHeight = paintingFont == null ? theFont.Height : paintingFont.Height;
				int netHeight = lineHeight * theLines.Count + lineSpacing * Math.Max(0, theLines.Count - 1) + 2 * innerPadding + 2; 
				if (forceResize)
					Bounds.Height = netHeight;


				if (this.OverflowTextHandling == TextField.OverflowStyle.Ignore)
				{
					return false;
				}
				else
				{
					return current < buffer.Length;
				}
			}
			catch (Exception e)
			{
				return false;
			}
		}

		/// <summary>
		/// Resolves parameter values during printing.
		/// </summary>
		private string ResolveParameterValues(string input, ArrayList parameterPoints, ArrayList parameterOriginalEnds)
		{
			string buffer = "";
			int pos = -1;
			int oldPos = 0;

			while( (pos=input.IndexOf("$P",oldPos)) != -1 )
			{

				buffer += input.Substring(oldPos,pos-oldPos);
				if ( input.Substring(pos+2,1).Equals("{") && input.IndexOf("}",pos+2) != -1 )
				{
					string parameterName = input.Substring(pos+3,input.IndexOf("}",pos+2)-pos-3).Trim();
					int parPosition = buffer.Length;
					buffer += this.section.GetParameterValue(parameterName);

					parameterPoints.Add(new Point(parPosition, buffer.Length));
					
					oldPos = input.IndexOf("}",pos+2) + 1;
					parameterOriginalEnds.Add(oldPos);
				}
				else
				{				
					oldPos = pos+2;
				}
			}

			buffer += input.Substring(oldPos);

			return buffer;
		}

		
		private void startPainting(Graphics g)
		{
            // Raffaele Russo - 12/12/2011 - Start
            Rectangle paintArea = new Rectangle(Bounds.X - rBounds.X, Bounds.Y - rBounds.Y, Bounds.Width, Bounds.Height);
            // Rectangle paintArea = new Rectangle(theRegion.X - rBounds.X, theRegion.Y - rBounds.Y, theRegion.Width, theRegion.Height);
            // Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;
            // Raffaele Russo - 12/12/2011 - End

            int lineHeight = paintingFont.Height;


						
			if (orientation == Orientation.Horizontal)
			{
				int yPos = 0;

				switch (textVerticalAlignment)
				{
					case TextVerticalAlignmentType.Bottom:
						yPos = paintArea.Bottom - innerPadding - theLines.Count*lineHeight - (theLines.Count-1)*lineSpacing; 
						break;

					case TextVerticalAlignmentType.Middle:
						yPos = paintArea.Top + paintArea.Height/2 - ( theLines.Count*lineHeight + (theLines.Count-1)*lineSpacing )/2; 
						break;

					default:
						yPos = paintArea.Top + innerPadding;
						break;
				}

				for (int i=0;i<theLines.Count;i++)
				{					
					drawSimpleString(g, theLines[i].ToString(), yPos, (bool)lastLines[i] );
					yPos = yPos + lineHeight + lineSpacing;
				}
			}
			else
			{
				int xPos = paintArea.Right - lineHeight - innerPadding;

				switch (textVerticalAlignment)
				{
					case TextVerticalAlignmentType.Bottom:
						xPos = paintArea.Left + innerPadding + theLines.Count*lineHeight + (theLines.Count-1)*lineSpacing - lineHeight; 
						break;

					case TextVerticalAlignmentType.Middle:
						xPos = paintArea.Right - theRegion.Width/2 + ( theLines.Count*lineHeight + (theLines.Count-1)*lineSpacing )/2 - lineHeight; 
						break;

					default:
						xPos = paintArea.Right - lineHeight - innerPadding;
						break;
				}

				for (int i=0;i<theLines.Count;i++)
				{				
					drawVertical(g,theLines[i].ToString(), xPos, (bool)lastLines[i]);
					xPos = xPos - lineHeight - lineSpacing;
				}
			}
		}

		
		#endregion

		#region Contructors


		/// <summary>
		/// Initializes a new instance of the TextField class.
		/// </summary>
		/// <param name="originX">x-position of the new TextField</param>
		/// <param name="originY">y-position of the new TextField</param>
		/// <param name="width">Width of the new TextField</param>
		/// <param name="height">Height of the new TextField</param>
		/// <param name="parent">Parent of the new TextField</param>
		public TextField(int originX,int originY,int width,int height, Section parent)
		{
			section = parent;
			theText = "";
			theRegion = new Rectangle(originX,originY,width,height);
			theLines = new ArrayList();
			lastLines = new ArrayList();
		}


		internal TextField(XmlNode node, Section parent)
	    {
			section = parent;
			theText = "";
			Init(node);
			theLines = new ArrayList();
			lastLines = new ArrayList();
	    }

		
		#endregion

		#region ICustomPaint Members

		/// <summary>
		/// Clones the structure of the TextField, including all properties
		/// </summary>
		/// <returns><see cref="Com.Delta.Print.Engine.TextField">daReport.TextField</see></returns>
		public override object Clone()
		{
			TextField tmp = new TextField(this.X, this.Y, this.Width, this.Height, this.section);
			tmp.Layout = this.Layout;
			tmp.theText = this.theText;
			tmp.BorderWidth = this.BorderWidth;
			tmp.BorderColor = this.BorderColor;
			tmp.BackgroundColor = this.BackgroundColor;
			tmp.Font = this.Font;
			tmp.TextAlignment = this.TextAlignment;
			tmp.TextVerticalAlignment = this.TextVerticalAlignment;
			tmp.ForegroundColor = this.ForegroundColor;
			tmp.OverflowTextHandling = this.OverflowTextHandling;
			tmp.TextOrientation = this.TextOrientation;

			return tmp;
		}


		/// <summary>
		/// Paints the TextField
		/// </summary>
		/// <param name="g">The Graphics object to draw</param>
		/// <remarks>Causes the TextBox to be painted to the screen.</remarks>
		public override void Paint(System.Drawing.Graphics g)
		{

            // Raffaele Russo - 12/12/2011 - Start
            Rectangle paintArea = new Rectangle(Bounds.X - rBounds.X, Bounds.Y - rBounds.Y, Bounds.Width, Bounds.Height);
            // Rectangle paintArea = new Rectangle(theRegion.X - rBounds.X,theRegion.Y - rBounds.Y,theRegion.Width,theRegion.Height);
			// Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;
            // Raffaele Russo - 12/12/2011 - End

			if ( backgroundColor != Color.Transparent )
			{
				g.FillRectangle(new SolidBrush(backgroundColor), paintArea);
			}

			if ( this.borderWidth > 0 )
			{
                g.DrawRectangle(new Pen(this.borderColor,borderWidth), paintArea);
			}

			drawText(g);
		}


		internal override bool IsDynamic()
		{
			return true;
		}

		internal override bool CanStretch()
		{
			return this.OverflowTextHandling == OverflowStyle.Display;
		}

		internal void ResetText()
		{
			residualText = String.Empty;
		}

		internal void FullReset()
		{
			currentText = string.Empty;
			residualText = String.Empty;
			startPosition = 0;
		}
		

		#endregion
			

	}
}
