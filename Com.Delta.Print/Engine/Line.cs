using System;
using System.Drawing;
using System.ComponentModel;
using System.Xml;

namespace Com.Delta.Print.Engine
{
	/// <summary>
	/// Report element that displays the line.
	/// </summary>
	public sealed class Line : ICustomPaint
	{

		/// <summary>
		/// Possible line orientations.
		/// </summary>
		public enum Orientations {WE = 0, NS, NWSE, NESW};

		private Orientations orientation = Orientations.WE;
		private Color color = Color.Black;
		private int lineWidth = 1;

       
		/// <summary>
		/// Creates new instance of Line in the given section.
		/// </summary>
		public Line(int x,int y, int width, int height, Section parent)
		{
			section = parent;
			theRegion = new Rectangle(x,y,width,height);
			this.Bounds = theRegion;
		}

		internal Line(XmlNode node, Section parent)
		{
			section = parent;
			Init(node);
		}

		/// <summary>
		/// Gets the string representation of report element.
		/// </summary>
		public override string ToString()
		{
			return "Line" ;
		}

		/// <summary>
		/// Gets or sets the line orientation.
		/// </summary>
		/// <remarks>This property sets the line orientation of the Line object.
		/// </remarks>
		[Category("Appearance"), DefaultValue(Orientations.WE), Description("Gets or sets the orientation of the line.")]
		public Orientations Orientation
		{
			get {return orientation;}
			set {orientation = value;}
		}

		/// <summary>
		/// Gets or sets the line color.
		/// </summary>
		/// <remarks>This property sets the line color of the Line object. This can be any color
		/// from the System.Drawing.Color structure
		/// </remarks>
		[Category("Appearance"), DefaultValue(typeof(System.Drawing.Color),"Black"), Description("Gets or sets the line color.")]
		public Color Color
		{
			get {return color;}
			set {color = value;}
		}

		/// <summary>
		/// Gets or sets the line display width.
		/// </summary>
		[Category("Appearance"), DefaultValue(1), Description("Gets or sets the line display width.")]
		public int LineWidth
		{
			get {return lineWidth;}
			set {lineWidth = Math.Max(1,value);}
		}


		/// <summary>
		/// Gets the line coordinates
		/// </summary>
		[Browsable(false)]
		internal Point[] Coordinates
		{
			get
			{
				Point[] coordinates = new Point[2];
				switch (orientation)
				{
					case Orientations.WE:
						coordinates[0] = new Point(0, this.Height/2);
						coordinates[1] = new Point(this.Width, this.Height/2);
						break;

					case Orientations.NS:
						coordinates[0] = new Point(this.Width/2, 0);
						coordinates[1] = new Point(this.Width/2, this.Height);
						break;

					case Orientations.NWSE:
						coordinates[0] = new Point(0, 0);
						coordinates[1] = new Point(this.Width, this.Height);
						break;

					case Orientations.NESW:
						coordinates[0] = new Point(this.Width, 0);
						coordinates[1] = new Point(0, this.Height);
						break;

					default:
						coordinates[0] = new Point(0, this.Height/2);
						coordinates[1] = new Point(this.Width, this.Height/2);
						break;

				}

				if (this.Section.Document.DesignMode)
				{
					coordinates[0].Offset(this.X, this.Y);
					coordinates[1].Offset(this.X, this.Y);
				}
				else
				{
					coordinates[0].Offset(Bounds.X, Bounds.Y);
					coordinates[1].Offset(Bounds.X, Bounds.Y);
				}

				return coordinates;
			}
		}

		#region Public Overrides


		/// <summary>
		/// Gets or sets the name of the line element.
		/// </summary>
        [Category("Data"), Description("The name of Line element.")]
		public override string Name
		{
			get {return name;}
			set {name = value;}
		}


		/// <summary>
		/// Paints the line element.
		/// </summary>
		/// <param name="g">The Graphics object to draw</param>
		/// <remarks>Causes the Line to be painted to the screen.</remarks>
		public override void Paint(Graphics g)
		{
			Point[] coordinates = this.Coordinates;

            // Raffaele Russo - 28/09/2011 - Start
            coordinates[0].X = coordinates[0].X - rBounds.X;
            coordinates[1].X = coordinates[1].X - rBounds.X;
            coordinates[0].Y = coordinates[0].Y - rBounds.Y;
            coordinates[1].Y = coordinates[1].Y - rBounds.Y;
            // Raffaele Russo - 28/09/2011 - End
	
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			g.DrawLine(new Pen(color, this.lineWidth), coordinates[0], coordinates[1]);
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
			
		}


		/// <summary>
		/// Clones the structure of the line element, including all properties.
		/// </summary>
		/// <returns><see cref="Com.Delta.Print.Engine.Line">Com.Delta.Print.Engine.Line</see></returns>
		public override object Clone()
		{
			
			Line tmp = new Line(this.X, this.Y, this.Width, this.Height, this.section);
			tmp.Layout = this.Layout;
			tmp.Orientation = this.Orientation;
			tmp.Color = this.Color;
			tmp.LineWidth = this.LineWidth;

			return tmp;
		}

		#endregion

	}
}
