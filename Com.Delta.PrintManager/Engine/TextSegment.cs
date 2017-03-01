using System;
using System.Drawing;

namespace Com.Delta.PrintManager.Engine
{
	/// <summary>
	/// Summary description for TextSegment.
	/// </summary>
	public class TextSegment
	{
		private string text = string.Empty;
		private Color color = Color.Red;
		private Font font = new Font("Tahoma", 8);
		private double height = 0;
		private double width = 0;

		public TextSegment(string text, Color color, Font font, double height)
		{
			this.text = text;
			this.color = color;
			this.font = font;
			this.height = height;

		}


		public string Text
		{
			get {return text;}
		}

		public Color Color
		{
			get {return color;}
		}

		public Font Font
		{
			get {return font;}
		}	

		public double Height
		{
			get {return height;}
		}

		public double Width
		{
			get {return width;}
			set {width = value;}
		}
	}
}
