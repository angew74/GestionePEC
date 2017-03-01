using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml;

namespace Com.Delta.Print.Engine
{
	/// <summary>
	/// Report element for displaying box frames.
	/// </summary>
	[DefaultProperty("Color")]
	public sealed class Box : ICustomPaint
	{

		/// <summary>
		/// Enumeration of fill styles for the Elipse object
		/// </summary>
		public enum FillStyles 
		{
			/// <summary>Box is filled using solid color.</summary>
			Solid = 0, 
			/// <summary>Box is filled using linear gradient.</summary>
			LinearGradient,
			/// <summary>Box is filled using radial gradient.</summary>
			RadialGradient
		};

		private FillStyles fillStyle = FillStyles.Solid;
		private Color backgroundColor = Color.Transparent;
		private Color gradientColor = Color.Blue;
		private LinearGradientMode linearGradientMode = LinearGradientMode.Vertical; 
		private int borderWidth = 1;
		private Color borderColor = Color.Black;


		/// <summary>
		/// Creates new instance of Box in the given section.
		/// </summary>
		public Box(int x,int y, int width, int height, Section parent)
		{
			section = parent;
			theRegion = new Rectangle(x,y,width,height);
			this.Bounds = theRegion;
		}

		internal Box(XmlNode node, Section parent)
		{
			section = parent;
			Init(node);			
		}


		/// <summary>
		/// Gets the string representation of report element.
		/// </summary>
		public override string ToString()
		{
			return "Box";
		}

		/// <summary>
		/// Gets or sets the fill style for box element.
		/// </summary>
		/// <remarks>This property sets the fill style of the Box object.
		/// </remarks>
		[Category("Appearance"), DefaultValue(FillStyles.Solid), Description("Gets or sets the fill style for elipse.")]
		public FillStyles FillStyle
		{
			get {return fillStyle;}
			set {fillStyle = value;}
		}

		/// <summary>
		/// Gets or sets the box element color.
		/// </summary>
		/// <remarks>This property sets the background color of the Box object. This can be any color
		/// from the System.Drawing.Color structure.
		/// </remarks>
		[Category("Appearance"), DefaultValue(typeof(System.Drawing.Color),"Transparent"), Description("Gets or sets the elipse color.")]
		public Color Color
		{
			get {return backgroundColor;}
			set {backgroundColor = value;}
		}

		/// <summary>
		/// Gets or sets the box gradient color. Meaningfull only if FillStyle is set to Gradient.
		/// </summary>
		/// <remarks>Gets or sets the box gradient color. Meaningfull only if FillStyle is set to Gradient. This can be any color
		/// from the System.Drawing.Color structure.
		/// </remarks>
		[Category("Appearance"), DefaultValue(typeof(System.Drawing.Color),"Blue"), Description("Gets or sets the elipse gradient color. Meaningfull only if FillStyle is set to Gradient.")]
		public Color GradientColor
		{
			get {return gradientColor;}
			set {gradientColor = value;}
		}

		/// <summary>
		/// Gets or sets the box linear gradient mode. Meaningfull only if FillStyle is set to Gradient.
		/// </summary>
		[Category("Appearance"), DefaultValue(LinearGradientMode.Vertical), Description("Gets or sets the elipse linear gradient mode. Meaningfull only if FillStyle is set to Gradient.")]
		public LinearGradientMode GradientMode
		{
			get {return linearGradientMode;}
			set {linearGradientMode = value;}
		}

		/// <summary>
		/// Gets or sets the box element border width.
		/// </summary>		
		[Category("Appearance"), DefaultValue(1), Description("Gets or sets the border width.")]
		public int BorderWidth
		{
			get {return borderWidth;}
			set {borderWidth = Math.Max(0,value);}
		}

		/// <summary>
		/// Gets or sets the box border color.
		/// </summary>
		[Category("Appearance"), DefaultValue(typeof(System.Drawing.Color),"Black"), Description("Gets or sets the elipse border color.")]
		public Color BorderColor
		{
			get {return borderColor;}
			set 
			{
				if (value == Color.Transparent)
					throw new Exception("Border color can not be set to Transparent. Use BorderWidth=0 if you want to hide border.");
				borderColor = value;
			}
		}
		
		/// <summary>
		/// Image for printing and exporting purpose
		/// </summary>
		[Browsable(false)]
		public Bitmap Image
		{
			get 
			{
                // Raffaele Russo - 12/12/2011 - Start
                Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : new Rectangle(Bounds.X - rBounds.X, Bounds.Y - rBounds.Y, Bounds.Width, Bounds.Height);
                // Rectangle paintArea = new Rectangle(theRegion.X - rBounds.X, theRegion.Y - rBounds.Y, theRegion.Width, theRegion.Height);
                // Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;
                // Raffaele Russo - 12/12/2011 - End

				Bitmap mImage = new Bitmap(paintArea.Width + (borderWidth==1? 1 : 0), paintArea.Height + (borderWidth==1? 1 : 0));

				Graphics imageGraphics = Graphics.FromImage(mImage);

				Matrix m = new Matrix();
				m.Translate(-paintArea.X, -paintArea.Y, MatrixOrder.Append);
				imageGraphics.Transform = m;
				

				this.Paint(imageGraphics);


				imageGraphics.Dispose();

				return mImage;
			}
		}

		#region Public Overrides


		/// <summary>
		/// Gets or sets the name of the box element.
		/// </summary>
        [Category("Data"), Description("The name of Box element.")]
		public override string Name
		{
			get {return name;}
			set {name = value;}
		}		


		/// <summary>
		/// Paints the box element.
		/// </summary>
		/// <param name="g">The Graphics object to draw</param>
		/// <remarks>Causes the Line to be painted to the specified device.</remarks>
		public override void Paint(Graphics g)
		{

			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Raffaele Russo - 12/12/2011 - Start
            Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : new Rectangle(Bounds.X - rBounds.X, Bounds.Y - rBounds.Y, Bounds.Width, Bounds.Height);
            // Rectangle paintArea = new Rectangle(theRegion.X - rBounds.X, theRegion.Y - rBounds.Y, theRegion.Width, theRegion.Height);
            // Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;
            // Raffaele Russo - 12/12/2011 - End

			if (fillStyle == FillStyles.LinearGradient)
			{
				Color backColor = paintTransparent ? this.ShiftColor(backgroundColor) : backgroundColor;
				Color foreColor = paintTransparent ? this.ShiftColor(gradientColor) : gradientColor;
				
				LinearGradientBrush brush = new LinearGradientBrush(paintArea, backColor, foreColor, this.linearGradientMode);
				g.FillRectangle(brush, paintArea);
			}
			else if (fillStyle == FillStyles.RadialGradient)
			{
				GraphicsPath path = new GraphicsPath();
				path.StartFigure();
				path.AddRectangle(paintArea);
				
				path.CloseFigure();

				PathGradientBrush brush = new PathGradientBrush(path);
				brush.CenterColor = paintTransparent ? this.ShiftColor(backgroundColor) : backgroundColor;
				Color foreColor = paintTransparent ? this.ShiftColor(gradientColor) : gradientColor;
				brush.SurroundColors = new Color[]{foreColor};

				g.FillRectangle(brush, paintArea);
			}
			else
			{				
				Color backColor = paintTransparent ? this.ShiftColor(backgroundColor) : backgroundColor;
				g.FillRectangle(new SolidBrush(backColor), paintArea);
			}			

			if (this.borderWidth>0)
			{								
				Pen borderPen = new Pen(borderColor, borderWidth);
				borderPen.Alignment = PenAlignment.Inset;
				g.DrawRectangle(borderPen, paintArea);
				borderPen.Dispose();
			}
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
			
		}


		/// <summary>
		/// Clones the structure of the box element, including all properties.
		/// </summary>
		/// <returns><see cref="Com.Delta.Print.Engine.Box">Com.Delta.Print.Engine.Box</see></returns>
		public override object Clone()
		{			
			Box tmp = new Box(this.X, this.Y, this.Width, this.Height, this.section);
			tmp.Layout = this.Layout;
			tmp.Color = this.Color;
			tmp.BorderWidth = this.BorderWidth;
			tmp.BorderColor = this.borderColor;
			tmp.FillStyle = this.FillStyle;
			tmp.GradientColor = this.GradientColor;
			tmp.GradientMode = this.GradientMode;

			return tmp;
		}

		#endregion

	}
}
