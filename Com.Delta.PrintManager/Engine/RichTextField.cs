using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Text;
using System.Xml;
using System.Drawing.Design;

using Com.Delta.PrintManager.Engine.Parse.Html;

namespace Com.Delta.PrintManager.Engine
{
	/// <summary>
	/// Summary description for RichTextField.
	/// </summary>
	public sealed class RichTextField : ICustomPaint
	{

		private string content = String.Empty;
		private Font font = new Font("Tahoma", 8);
		private Color foregroundColor = Color.Black;
		private Color backgroundColor = Color.Transparent;
		private Color borderColor = Color.Black;
		private int borderWidth = 1;
		private int innerPadding = 2;
		private int renderingIndex = 0;
		private TextField.OverflowStyle overflowStyle = TextField.OverflowStyle.Ignore;

		internal ArrayList lines = new ArrayList();


		private HtmlEngine previousState = null;


		

		public RichTextField()
		{

		}



		#region Contructors


		/// <summary>
		/// Initializes a new instance of the RichTextField class.
		/// </summary>
		/// <param name="originX">x-position of the new TextField</param>
		/// <param name="originY">y-position of the new TextField</param>
		/// <param name="width">Width of the new TextField</param>
		/// <param name="height">Height of the new TextField</param>
		/// <param name="parent">Parent of the new TextField</param>
		public RichTextField(int originX,int originY,int width,int height, Section parent)
		{
			section = parent;
			theRegion = new Rectangle(originX,originY,width,height);
			lines = new ArrayList();
		}


		internal RichTextField(XmlNode node, Section parent)
		{
			section = parent;
			Init(node);
			lines = new ArrayList();
		}

		
		#endregion



		internal override bool IsDynamic()
		{
			return true;
		}

		/// <summary>
		/// Clones the structure of the RichTextField, including all properties
		/// </summary>
		/// <returns><see cref="Com.Delta.PrintManager.Engine.TextField">daReport.TextField</see></returns>
		public override object Clone()
		{
			RichTextField tmp = new RichTextField(this.X, this.Y, this.Width, this.Height, this.section);
			
			tmp.Layout = this.Layout;
			tmp.content = this.content;
			tmp.borderWidth = this.borderWidth;
			tmp.borderColor = this.borderColor;
			tmp.backgroundColor = this.backgroundColor;
			tmp.font = this.font;
			tmp.foregroundColor = this.foregroundColor;
			tmp.OverflowTextHandling = this.OverflowTextHandling;
			
			
			return tmp;
		}


		/// <summary>
		/// Paints the RichTextField
		/// </summary>
		/// <param name="g">The Graphics object to draw</param>
		/// <remarks>Causes the RichTextBox to be painted to the screen.</remarks>
		public override void Paint(System.Drawing.Graphics g)
		{

            // Raffaele Russo - 12/12/2011 - Start
            Rectangle paintArea = new Rectangle(Bounds.X - rBounds.X, Bounds.Y - rBounds.Y, Bounds.Width, Bounds.Height);
            // Rectangle paintArea = new Rectangle(theRegion.X - rBounds.X, theRegion.Y - rBounds.Y, theRegion.Width, theRegion.Height);
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

			if (this.Section.Document.DesignMode)
			{
				renderingIndex = 0;
				CreateLines(g, true, false);

			}

			PaintLines(g);
		}

		private void PaintLines(Graphics g)
		{
            // Raffaele Russo - 12/12/2011 - Start
            Rectangle bounds = new Rectangle(Bounds.X - rBounds.X, Bounds.Y - rBounds.Y, Bounds.Width, Bounds.Height);
            // Rectangle bounds = new Rectangle(theRegion.X - rBounds.X, theRegion.Y - rBounds.Y, theRegion.Width, theRegion.Height);
            // Rectangle bounds = this.Section.Document.DesignMode ? theRegion : Bounds;
            // Raffaele Russo - 12/12/2011 - End

			bounds.Inflate(-innerPadding, -innerPadding);

			float yPosition = bounds.Top;
			float xPosition = bounds.Left;
			for (int i=0;i<lines.Count;i++)
			{
				TextLine textLine = lines[i] as TextLine;
				TextSegment[] segments = (TextSegment[])textLine.Segments.ToArray(typeof(TextSegment));

				float height = textLine.Height;

				float width = 0;
				for(int j=0;j<segments.Length;j++)
				{					
					width += (float)segments[j].Width;
				}


				if (textLine.Alignment == TextField.TextAlignmentType.Justified)
				{
					this.PaintJustified(g, textLine, bounds, yPosition);
				}
				else
				{

					if (textLine.Alignment == TextField.TextAlignmentType.Center)
					{
						xPosition = bounds.Left + (bounds.Width - width) /2;
					}
					else if (textLine.Alignment == TextField.TextAlignmentType.Right)
					{
						xPosition = bounds.Right - width;
					}
					else
					{
						xPosition = bounds.Left;
					}

				
					for (int j=0;j<segments.Length;j++)
					{
					
						TextSegment segment = segments[j];
						float y = yPosition + height - (float)segment.Height;
						Font paintFont = this.PrepareFont(g, segment.Font);
						g.DrawString(segment.Text, paintFont, new SolidBrush(segment.Color), new PointF(xPosition, y));

						xPosition += (float)segment.Width;

					}
				}

				yPosition += height;
			}
		}

		private void PaintJustified(Graphics g, TextLine textLine, Rectangle bounds, float yPosition)
		{
			TextSegment[] segments = (TextSegment[])textLine.Segments.ToArray(typeof(TextSegment));

			float height = textLine.Height;

			ArrayList words = new ArrayList();
			double wordsWidth = 0;
			for(int i=0;i<segments.Length;i++)
			{					
				if (segments[i].Text != " ")
				{
					words.Add(segments[i]);
					wordsWidth += segments[i].Width;
				}

			}
	
			float theOffset = 0;
			if (words.Count > 1)
				theOffset = (float)(bounds.Width - 2*innerPadding - wordsWidth) / (words.Count - 1);



			float xPos = bounds.Left;
			for (int i=0;i<words.Count;i++)
			{

				TextSegment segment = words[i] as TextSegment;

				if (i==0)
				{
					xPos = bounds.Left;
				}
				else if(i == words.Count - 1)
				{
					xPos = bounds.Right - innerPadding - (float)segment.Width;
				}					
				
				Font paintFont = this.PrepareFont(g, segment.Font);
				//g.DrawString(segment.Text, segment.Font, new SolidBrush(segment.Color), xPos, yPosition );							
				g.DrawString(segment.Text, paintFont, new SolidBrush(segment.Color), xPos, yPosition );							
					
				xPos += (float)segment.Width + theOffset;
			}
		}


		internal bool CreateLines(Graphics g, bool fromStart, bool forceResize)
		{
			int position = fromStart ? 0 : renderingIndex;

			ArrayList parameterPositions = new ArrayList();
			ArrayList parameterEnds = new ArrayList();
			
			string fullText = this.ResolveParameterValues(content, parameterPositions, parameterEnds);
			string text = section.Document.DesignMode ? content.Substring(position) : fullText.Substring(position);

			Rectangle bounds = this.Section.Document.DesignMode ? theRegion : Bounds;
			bounds.Inflate(-innerPadding, -innerPadding);


			Context previousContext = null;
			if (previousState != null && previousState.Context != null)
				previousContext = previousState.Context;

			Com.Delta.PrintManager.Engine.Parse.Html.HtmlEngine engine = section.Document.DesignMode ? new Com.Delta.PrintManager.Engine.Parse.Html.HtmlEngine(this, g, bounds, this.font, this.foregroundColor) : (previousContext == null ? new Com.Delta.PrintManager.Engine.Parse.Html.HtmlEngine(this, g, bounds, this.font, this.foregroundColor) : new Com.Delta.PrintManager.Engine.Parse.Html.HtmlEngine(this, g, bounds, previousContext.Font, previousContext.Color));

			engine.InitState(previousState, section.Document.DesignMode);
			int parsingLength = engine.Parse(text, lines);


			double minimalHeight = 0;
			foreach(TextLine line in lines)
			{
				minimalHeight += line.Height;
			}

			int netHeight = (int)minimalHeight + 2 * innerPadding + 2; 
			if (forceResize)
				Bounds.Height = netHeight;



			this.previousState = engine;

			renderingIndex += parsingLength;

			return renderingIndex < fullText.Length || engine.Residual != null || engine.ResidualText != string.Empty;

		}

		internal void ResetText()
		{
			renderingIndex = 0;
			if (previousState != null)
			{
				previousState.Reset();
			}
		}

		internal override bool CanStretch()
		{
			return this.OverflowTextHandling == TextField.OverflowStyle.Display;
		}

		internal Font PrepareMeasurementFont(Font theFont)
		{			
			float ratio = 96f / this.Section.Document.GetGraphics().DpiX;			
			Font measureFont = new Font(theFont.FontFamily, theFont.SizeInPoints * ratio, theFont.Style);
			return measureFont;
		}

		internal Font PrepareFont(Graphics g, Font theFont)
		{
			float sizeInPixels = this.PointsToPixels(theFont.SizeInPoints);
			//Font paintingFont = this.Section.Document.DesignMode ? new Font(theFont.FontFamily, sizeInPixels * g.PageScale, theFont.Style, GraphicsUnit.Pixel) : new Font(theFont.FontFamily, sizeInPixels * 0.96f * g.PageScale, theFont.Style, GraphicsUnit.Pixel);			
			Font paintingFont = this.Section.Document.DesignMode ? new Font(theFont.FontFamily, sizeInPixels * g.PageScale, theFont.Style, GraphicsUnit.Pixel) : new Font(theFont.FontFamily, sizeInPixels * g.PageScale, theFont.Style, GraphicsUnit.Pixel);			
			return paintingFont;
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


		/// <summary>
		/// Gets or sets the name of the RichTextField element.
		/// </summary>
		[Category("Data"), Description("The name of TextField element.")]
		public override string Name
		{
			get {return name;}
			set {name = value;}
		}

		/// <summary>
		/// Gets the string representation of report element.
		/// </summary>
		public override string ToString()
		{
			string displayName = content.Substring(0, Math.Min(20, content.Length));
			return "Rich Text [" + displayName + "]" ;
		}

		/// <summary>
		/// Gets/Sets text to be displayed in the TextField object.
		/// </summary>
		[Category("Data")]
		[Editor(typeof(Com.Delta.PrintManager.Engine.Editors.PlainTextEditor), typeof(UITypeEditor)), Description("Content of the RichTextField.")]
		public string Text
		{
			get 
			{
				//string displayText = content.Length>25 ? content.Substring(0,25)+"..." : content ;				
				//return displayText.Replace("\r\n","  ");
				return content;
			}
			set {content = value;}
		}


		/// <summary>
		/// Gets/Sets the foreground color for the RichTextField
		/// </summary>
		/// <remarks>This property sets the foreground color of the RichTextField object. This can be any color
		/// from the System.Drawing.Color structure
		/// </remarks>
		[Category("Appearance"), DefaultValue(typeof(System.Drawing.Color),"Black")]
		public Color ForegroundColor
		{
			get {return foregroundColor;}
			set {foregroundColor = value;}
		}


		/// <summary>
		/// Gets/Sets the background color for the RichTextField
		/// </summary>
		/// <remarks>This property sets the background color of the RichTextField object. This can be any color
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
			get {return font;}
			set 
			{
				font = value; 
				//paintingFont = null;
			}
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
		/// Gets/Sets overflow text handling for this object.
		/// </summary>
		[Category("Data")]
		[DefaultValue(TextField.OverflowStyle.Ignore), Description("How to handle oveflow text. If set to Display, the whole text will be spread over pages. Option Ignore will paint text always from the start.")]
		public TextField.OverflowStyle OverflowTextHandling
		{
			get {return overflowStyle;}
			set {overflowStyle = value;}
		}

	
	}
}
