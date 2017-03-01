using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml;

namespace Com.Delta.Print.Engine
{
	/// <summary>
	/// Inheritable class that provides empty implementation of Com.Delta.Print.Engine.ICustomPaint. This class is used to create user-defined report widgets.
	/// </summary>
	public class UserPaint : ICustomPaint
	{
		private string name = String.Empty;
		private string type = String.Empty;

		public UserPaint()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		/// Gets or sets the name of the report element.
		/// </summary>
		public override String Name
		{
			get {return name;}
			set {name = value;}
		}

		/// <summary>
		/// Gets the string representation of report element.
		/// </summary>
		public override string ToString()
		{
			return "UserPaint [" + this.Name + "]" ;
		}

		/*
		public String Type
		{
			get {return type;}
			set {type = value;}
		}
		*/

		/// <summary>
		/// Paints the report element.
		/// </summary>
		public override void Paint(System.Drawing.Graphics g)
		{			
			Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;
			Paint(g, paintArea);			
		}

		public virtual void Paint(System.Drawing.Graphics g, Rectangle drawingArea)
		{
			if (section.Document.DesignMode)
			{
				g.FillRectangle(new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.Percent05, Color.Gray, Color.Transparent), drawingArea);

				StringFormat sf = new StringFormat();
				sf.Alignment = StringAlignment.Center;
				sf.LineAlignment = StringAlignment.Center;
				g.DrawString("UserPaint placeholder.", new Font("Tahoma", 8*g.PageScale), Brushes.Black, drawingArea, sf);
			}
		}


		/// <summary>
		/// Serializes UserPaint object to XML (for future use).
		/// </summary>
		public virtual XmlNode SerializeToXml()
		{
			return null;
		}

		/// <summary>
		/// Deserializes UserPaint object from XML (for future use).
		/// </summary>
		public virtual void DeserializeFromXml(XmlNode node)
		{
			
		}
		

		/// <summary>
		/// Clones report element.
		/// </summary>
		public override object Clone()
		{
			UserPaint tmp = new UserPaint(0,0,0,0,section);
			tmp.X = this.X;
			tmp.Y = this.Y;
			tmp.Width = this.Width;
			tmp.Height = this.Height;
			tmp.Layout = this.Layout;			
			tmp.Name = "userPaint" + tmp.GetHashCode().ToString();
			return tmp;
		}

		/// <summary>
		/// Map image for printing and exporting purpose
		/// </summary>
		[Browsable(false)]
		public Bitmap Image
		{
			get 
			{
				Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;

				Bitmap mapImage = new Bitmap(paintArea.Width + 1, paintArea.Height + 1);

				Graphics imageGraphics = Graphics.FromImage(mapImage);

				Matrix m = new Matrix();
				m.Translate(-paintArea.X, -paintArea.Y, MatrixOrder.Append);
				imageGraphics.Transform = m;
				

				this.Paint(imageGraphics);


				imageGraphics.Dispose();

				return mapImage;
			}
		}

		#region Contructors


		/// <summary>
		/// Initializes a new instance of the TextField class.
		/// </summary>
		/// <param name="originX">x-position of the new TextField</param>
		/// <param name="originY">y-position of the new TextField</param>
		/// <param name="width">Width of the new TextField</param>
		/// <param name="height">Height of the new TextField</param>
		/// <param name="parent">Parent of the new TextField</param>
		public UserPaint(int originX,int originY,int width,int height, Section parent)
		{
			section = parent;
			theRegion = new Rectangle(originX,originY,width,height);
			this.Bounds = theRegion;
		}


		internal UserPaint(XmlNode node, Section parent)
		{
			section = parent;
			Init(node);
		}

		
		#endregion


	}
}
