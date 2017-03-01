
using System;
using System.Text;
using System.Drawing;

namespace Com.Delta.PrintManager.Engine.Pdf
{
	/// <summary>
	/// a generic Rectangle for a PdfPage
	/// </summary>
	public class PdfRectangle : PdfObject
	{
		
		internal PdfArea rectangleArea;
		/// <summary>
		/// gets the area of the rectangle
		/// </summary>
		public PdfArea RectangleArea
		{
			get {return this.rectangleArea;}
		}

		private Color borderColor = Color.Black;
		private Color FillingColor = Color.Transparent;
		private bool filled;
		private bool stroked = true;
		private double strokeWidth;


		/// <summary>
		/// gets or sets the width of the stroke
		/// </summary>
		public double StrokeWidth
		{
			set {this.strokeWidth = Math.Max(0, value);}
			get {return this.strokeWidth;}
		}
		
		/// <summary>
		/// creates a new rectangle
		/// </summary>
		/// <param name="RectangleArea">the area which will contains the rectangle</param>
		/// <param name="BorderColor"></param>
		public PdfRectangle(PdfDocument PdfDocument, PdfArea RectangleArea, Color BorderColor)
		{
			this.PdfDocument=PdfDocument;
			this.rectangleArea=RectangleArea;
			this.borderColor=BorderColor;
			this.strokeWidth=1;
		}
		/// <summary>
		/// creates a new rectangle 
		/// </summary>
		/// <param name="RectangleArea"></param>
		/// <param name="BorderColor"></param>
		/// <param name="BorderWidth"></param>
		public PdfRectangle(PdfDocument PdfDocument,PdfArea RectangleArea,Color BorderColor,double BorderWidth):this(PdfDocument, RectangleArea, BorderColor, BorderWidth, Color.Transparent)
		{

		}

		/// <summary>
		/// creates a new rectangle
		/// </summary>
		/// <param name="RectangleArea"></param>
		/// <param name="BorderColor"></param>
		/// <param name="FillingColor"></param>
		public PdfRectangle(PdfDocument PdfDocument,PdfArea RectangleArea,Color BorderColor,Color FillingColor):this(PdfDocument, RectangleArea, BorderColor, 0, FillingColor)
		{

		}

		/// <summary>
		/// creates a new rectangle
		/// </summary>
		/// <param name="RectangleArea"></param>
		/// <param name="BorderColor"></param>
		/// <param name="BorderWidth"></param>
		/// <param name="FillingColor"></param>
		public PdfRectangle(PdfDocument PdfDocument, PdfArea RectangleArea, Color BorderColor, double BorderWidth, Color FillingColor)
		{
			this.PdfDocument=PdfDocument;
			this.rectangleArea=RectangleArea;
			this.borderColor=BorderColor;
			this.FillingColor=FillingColor;
			if (FillingColor != Color.Transparent)
				this.filled=true;

			this.strokeWidth = BorderWidth;
			this.stroked = this.borderColor != Color.Transparent && this.strokeWidth > 0;

		}

		/// <summary>
		/// fills the rectangle with a Color
		/// </summary>
		/// <param name="Color"></param>
		public void Fill(Color Color)
		{
			this.borderColor=Color;
			this.FillingColor=Color;
			if (FillingColor != Color.Transparent)
				this.filled=true;			
		}



		private string ToColorAndWidthStream()
		{
			System.Text.StringBuilder sb=new StringBuilder();
			
			sb.Append(Utility.ColorRGLine(this.borderColor));
			if (filled) 
				sb.Append(Utility.ColorrgLine(this.FillingColor));
			sb.Append(Utility.FormatDecimal(this.strokeWidth)+" ");
			sb.Append("w\n");
			return sb.ToString();
		}


		private string ToRectangleStream()
		{
			string text = String.Format("{0} {1} {2} {3} re ", Utility.FormatDecimal(this.RectangleArea.PosX), Utility.FormatDecimal(this.PdfDocument.PH-this.rectangleArea.PosY-this.RectangleArea.Height), Utility.FormatDecimal(this.RectangleArea.Width), Utility.FormatDecimal(this.RectangleArea.Height));
			text += filled ? (strokeWidth == 0 ? "f" : "B") : "s";
			text += "\n";
			return text;
		}

		internal string ToLineStream()
		{
			if (filled || stroked)
				return this.ToColorAndWidthStream() + this.ToRectangleStream();	
			else
				return String.Empty;
		}

		internal override int StreamWrite(System.IO.Stream stream)
		{
			string text = this.ToLineStream();

			PdfWriter wr = new PdfWriter(this.id);
			wr.AddStreamText(text);
			return wr.Write(stream);
		}

	}
}
