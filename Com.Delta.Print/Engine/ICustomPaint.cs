
using System;
using System.Drawing;
using System.Drawing.Printing;
using System.ComponentModel;
using System.Xml;


namespace Com.Delta.Print.Engine
{
	/// <summary>
	/// Abstract class that is used as a base class for all report elements.
	/// </summary>
	/// <remarks>
	/// ICustomPaint is the base for all document types and contains some properties and methods
	/// common to all document types.
	/// </remarks>
	public abstract class ICustomPaint : ICloneable
	{

		#region Declarations

		// the parent document of element
		// this reference is mandatory due to handling margins 
		/// <summary>
		/// The <see cref="Com.Delta.Print.Engine.ReportDocument">Com.Delta.Print.Engine.PrintDocument</see> object
		/// </summary>
		protected Section section = null;


		/// <summary>
		/// Layout type of this element
		/// </summary>
		protected LayoutTypes layoutType = LayoutTypes.EveryPage;

		/// <summary>
		/// If component is painted semi-transparent.
		/// </summary>
		protected bool paintTransparent = false;


		/// <summary>
		/// Gets the parent <see cref="Com.Delta.Print.Engine.Section">Section</see> object that this report element belongs to.
		/// </summary>
		[Browsable(false)]
		public Section Section
		{
			get {return section;}
			set {section = value;}
		}


		/// <summary>
		/// Possible layout methods of report element.
		/// </summary>
		public enum LayoutTypes
		{
			/// <summary>Element is displayed on every section page.</summary>
			EveryPage=0,
			/// <summary>Element is displayed on the first section page.</summary>
			FirstPage,
			/// <summary>Element is displayed on last section page.</summary>
			LastPage
		};

		/// <summary>
		/// Enumeration of possible horizontal alignments for the ReportDocument
		/// </summary>
		public enum HorizontalAlignmentTypes
		{
			/// <summary>No alignment. Manual placement.</summary>
			None=0,
			/// <summary>Left Alignment on the page.</summary>
			Left,
			/// <summary>Canter Alignment on the page.</summary>
			Center,
			/// <summary>Right Alignment on the page.</summary>
			Right
		};

		/// <summary>
		/// Enumeration of possible vertical alignments for the ReportDocument
		/// </summary>
		public enum VerticalAlignmentTypes
		{
			/// <summary>No alignment. Manual placement.</summary>
			None=0,
			/// <summary>Aligns to the top the page.</summary>
			Top,
			/// <summary>Aligns to the middle the page.</summary>
			Middle,
			/// <summary>Aligns to the bottom the page.</summary>
			Bottom
		};

		
		/// <summary>
		/// Horizontal alignment of the report element relative to page margins.
		/// </summary>
		protected HorizontalAlignmentTypes horizontalAlignment = HorizontalAlignmentTypes.None;

		/// <summary>
		/// Vertical alignment of the object report element relative to page margins.
		/// </summary>
		protected VerticalAlignmentTypes verticalAlignment = VerticalAlignmentTypes.None;
		
		/// <summary>
		/// Rectangle region of ICustomPaint.
		/// </summary>
		protected Rectangle theRegion = new Rectangle(0,0,0,0);

		/// <summary>
		/// Printer margins.
		/// </summary>
		protected Rectangle rBounds = new Rectangle(0,0,0,0);


		internal Rectangle Bounds = new Rectangle(0,0,0,0);

		private bool selectable = true;
		protected bool done = false;
		private bool displayed = false;
		private bool shifted = false;

		protected string name = String.Empty;

		#endregion


		internal bool Shifted
		{
			get {return shifted;}
			set {shifted=value;}
		}

		internal bool Done
		{
			get {return done;}
			set {done=value;}
		}

		internal bool Displayed
		{
			get {return displayed;}
			set {displayed=value;}
		}

		internal bool Ready
		{
			get {return this.Anchor == null ? true : (this.Anchor.Done && !this.Shifted);}
		}

		[Browsable(false)]
		internal bool Anchored
		{
			get {return this.section.Chain.Contains(this);}
		}

		[Browsable(false)]
		internal ICustomPaint Anchor
		{
			get 
			{
				if (this.section.Chain.Contains(this))
				{
					int index = this.section.Chain.IndexOf(this);
					if (index > 0)
						return this.section.Chain[index-1] as ICustomPaint;
					else
						return null;
				}
				else
				{
					return null;
				}				
			}
		}
		

		#region Public Properties

		/// <summary>
		/// Get or sets the layout type of report element.
		/// </summary>
		[Category("Layout"), DefaultValue(LayoutTypes.EveryPage), Description("Layout type for this report element.")]
		public virtual LayoutTypes Layout
		{
			get {return layoutType;}
			set {layoutType = value;} 		
		}

		/// <summary>
		/// Gets or sets whether the element is selectable in the Designer pane.
		/// </summary>
		[Category("Design mode"), DefaultValue(true), Description("Sets whether the object can be selected in the design pane.")]
		public bool Selectable
		{
			get {return selectable;}
			set {selectable = value;} 		
		}

		/// <summary>
		///  Gets or sets the horizontal alignment of the report element.
		/// </summary>
		[Category("Layout"), DefaultValue(ICustomPaint.HorizontalAlignmentTypes.None), Description("Horizontal alignment in the page, relative to margins. This property overrides element coordinates.")]
		public ICustomPaint.HorizontalAlignmentTypes HorizontalAlignment
		{
			get {return horizontalAlignment;}
			set 
			{
				horizontalAlignment = value;
				Size displaySize = section.Document.GetDisplaySize();
				switch (horizontalAlignment)
				{
					case ICustomPaint.HorizontalAlignmentTypes.Center:												
						theRegion.X = (displaySize.Width - section.Document.Margins.Right + section.Document.Margins.Left)/2 - Width/2;
						break;

					case ICustomPaint.HorizontalAlignmentTypes.Right:												
						theRegion.X = displaySize.Width - section.Document.Margins.Right - Width;
						break;

					case ICustomPaint.HorizontalAlignmentTypes.Left:
						theRegion.X = section.Document.Margins.Left;
						break;
				}
			}
		}


		/// <summary>
		///  Gets or sets the vertical alignment of the report element.
		/// </summary>
		[Category("Layout"), DefaultValue(ICustomPaint.VerticalAlignmentTypes.None), Description("Vertical alignment in the page, relative to margins. This property overrides element coordinates.")]
		public ICustomPaint.VerticalAlignmentTypes VerticalAlignment
		{
			get {return verticalAlignment;}
			set 
			{
				verticalAlignment = value;
				Size displaySize = section.Document.GetDisplaySize();
				switch (verticalAlignment)
				{
					case ICustomPaint.VerticalAlignmentTypes.Middle:
						theRegion.Y = (displaySize.Height - section.Document.Margins.Bottom + section.Document.Margins.Top)/2 - Height/2;
						break;

					case ICustomPaint.VerticalAlignmentTypes.Bottom:
						theRegion.Y = displaySize.Height - section.Document.Margins.Bottom - Height;
						break;

					case ICustomPaint.VerticalAlignmentTypes.Top:
						theRegion.Y = section.Document.Margins.Top;
						break;
				}
			}
		}

		#endregion

		#region Public Abstract Declarations

		/// <summary>
		/// When implemented by a class, the object is painted to the screen.
		/// </summary>
		public abstract void Paint(Graphics g);

		

		/// <summary>
		/// The name of the report element.
		/// </summary>
		[Category("Layout")]
		public abstract string Name
		{
			get;
			set;
		}





		#endregion

		#region ICloneable Members

		// needed for duplicateXXX() functions in design mode
		// the clone object MUST BE deep copy of the original element
		/// <summary>
		/// When implemented by a class, creates a deep copy of the object.
		/// </summary>
		/// <returns></returns>
		public abstract object Clone();	
	
		

		#endregion

		#region Virtual Properties

		/// <summary>
		/// Gets or sets the X coordinate of the the report element.
		/// </summary>
		[Category("Layout"), Description("The X coordinate of the left-upper corner of the element.")]
		public int X
		{
			get {return theRegion.X;}
			set 
			{
				if (this.HorizontalAlignment == ICustomPaint.HorizontalAlignmentTypes.None)
					theRegion.X = value;
				else if (this.HorizontalAlignment == ICustomPaint.HorizontalAlignmentTypes.Right)
				{
					theRegion.Width = theRegion.Width + theRegion.X - value;
					theRegion.X = value;					
				}
				else if (this.HorizontalAlignment == ICustomPaint.HorizontalAlignmentTypes.Center)
				{
					theRegion.Width = theRegion.Width + 2*(theRegion.X - value);
					theRegion.X = value;					
				}

			}
		}

		
		/// <summary>
		/// Gets or sets the Y coordinate of the report element.
		/// </summary>
		[Category("Layout"), Description("The Y coordinate of the left-upper corner of the element.")]
		public int Y
		{
			get {return theRegion.Y;}
			set 
			{
				if (this.VerticalAlignment == ICustomPaint.VerticalAlignmentTypes.None)
					theRegion.Y = value;
				else if (this.VerticalAlignment == ICustomPaint.VerticalAlignmentTypes.Bottom)
				{
					theRegion.Height = theRegion.Height + theRegion.Y - value;
					theRegion.Y = value;					
				}
				else if (this.VerticalAlignment == ICustomPaint.VerticalAlignmentTypes.Middle)
				{
					theRegion.Height = theRegion.Height + 2*(theRegion.Y - value);
					theRegion.Y = value;					
				}
			}
		}

		
		/// <summary>
		///  Gets or sets the width of the report element.
		/// </summary>
		[Category("Layout"), Description("The width of the element.")]
		public virtual int Width
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
					theRegion.X = theRegion.X - val/2 + theRegion.Width/2;
					theRegion.Width = val;									
				}
			}
		}


		/// <summary>
		///  Gets or sets the height of the report element.
		/// </summary>
		[Category("Layout"), Description("The height of the element.")]
		public virtual int Height
		{
			get {return theRegion.Height;}
			set
			{
				int val = Math.Max(0,value);
				if (this.VerticalAlignment == ICustomPaint.VerticalAlignmentTypes.None || this.VerticalAlignment == ICustomPaint.VerticalAlignmentTypes.Top)
					theRegion.Height = val;
				else if (this.VerticalAlignment == ICustomPaint.VerticalAlignmentTypes.Bottom)
				{
					theRegion.Y = theRegion.Y - val + theRegion.Height;
					theRegion.Height = val;									
				}
				else if (this.VerticalAlignment == ICustomPaint.VerticalAlignmentTypes.Middle)
				{
					theRegion.Y = theRegion.Y - val/2 + theRegion.Height/2;
					theRegion.Height = val;									
				}
			}
		}

		#endregion

		#region Virtual methods

		internal virtual bool IsDynamic()
		{
			return false;
		}

		internal virtual bool CanStretch()
		{
			return false;
		}

		/// <summary>
		/// Paints this object semi-transparent.
		/// </summary>
		public virtual void PaintTransparent(Graphics g)
		{
			paintTransparent = true;
			Paint(g);
			paintTransparent = false;
		}

		/// <summary>
		/// When implemented by a class, the object is painted to the screen.
		/// </summary>
		internal void Paint(PrintPageEventArgs e)
		{
			
			if( section.Document.PreviewMode )
			{
				rBounds = new Rectangle(0,0,0,0);
			}
			else
			{
				PrinterBounds objBounds = new PrinterBounds(e);
				rBounds = objBounds.Bounds;				
			}

			Paint(e.Graphics);
		}

		
		/// <summary>
		/// Gets the drawing region of the report element.
		/// </summary>
		/// <returns>System.Drawing.Rectangle</returns>
		public Rectangle GetRegion()
		{
			return theRegion;
		}



		#endregion

		#region Protected Methods

		/// <summary>
		/// Get the semi-transparent color for given opaque color.
		/// </summary>
		protected Color ShiftColor(Color color)
		{
			if (color==Color.Transparent)
				return color;
			else
				return Color.FromArgb(240, color);
		}

		/// <summary>
		/// Get the semi-transparent color for given opaque color.
		/// </summary>
		protected Color ShiftColor(Color color, int amount)
		{
			if (color==Color.Transparent)
				return color;
			else
			{
				int a = Math.Min(255, Math.Max(amount, 0));
				return Color.FromArgb(a, color);
			}
		}


		protected float PointsToPixels(float sizeInPoints) 
		{ 
			return (sizeInPoints/72.0f)*96.0f; 
		}


		protected void Init(XmlNode theNode)
		{
			int x = Convert.ToInt32( theNode.Attributes["x"].Value );
			int y = Convert.ToInt32( theNode.Attributes["y"].Value );
			int width = Convert.ToInt32( theNode.Attributes["width"].Value );
			int height = Convert.ToInt32( theNode.Attributes["height"].Value );

			this.theRegion = new Rectangle(x, y, width, height);
			
			

			if (theNode.Attributes["name"] != null)			
				this.Name = theNode.Attributes["name"].Value;

			if (theNode.Attributes["horAlignment"] != null)
				this.HorizontalAlignment = section.resolveHorizontalAlignment(theNode.Attributes["horAlignment"].Value);

			if (theNode.Attributes["verAlignment"] != null)
				this.VerticalAlignment = section.resolveVerticalAlignment(theNode.Attributes["verAlignment"].Value);

			if (theNode.Attributes["Selectable"] != null)
				this.Selectable = Convert.ToBoolean(theNode.Attributes["Selectable"].Value);
			
			this.Layout = section.resolveLayout(theNode);
		}


		internal void Prepare(bool startPage)
		{
			
			if (Anchored)
			{
				
				Bounds.X = theRegion.X - rBounds.X;
				Bounds.Width = theRegion.Width;

				if (this.Shifted)
				{
					Bounds.Y = this.section.Document.Margins.Top;
					Bounds.Height = theRegion.Height;
				}
				else
				{

					if (this.IsDynamic())
					{
						if (startPage && this.Anchor==null)
						{
							Bounds.Y = theRegion.Y - rBounds.Y;				
							Bounds.Height = this.CanStretch() ? this.section.GetAvailableHeight(Bounds.Y) : theRegion.Height;
						}
						else
						{
							Bounds.Y = this.section.Document.Margins.Top;				
							Bounds.Height = this.CanStretch() ? this.section.Document.PixelSize.Height - this.section.Document.Margins.Top - this.section.Document.Margins.Bottom : theRegion.Height;
						}
					}
					else
					{
						Bounds.Y = theRegion.Y - rBounds.Y;				
						Bounds.Height = theRegion.Height;
					}
				}
			}
			else
			{
				Bounds = new Rectangle(theRegion.X - rBounds.X, theRegion.Y - rBounds.Y, theRegion.Width, theRegion.Height);
			}


			this.Shifted = false;
		}

		#endregion

	}
}
