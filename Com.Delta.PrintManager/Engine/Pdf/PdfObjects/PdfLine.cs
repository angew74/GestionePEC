
using System;
using System.Drawing;
using System.Text;

namespace Com.Delta.PrintManager.Engine.Pdf
{
	/// <summary>
	/// a generic Line for a PdfPage
	/// </summary>
	public class PdfLine : PdfObject
	{
		private PointF start;
		private PointF end;
		private Color color;
		private double strokeWidth = 1;

		
		public Color Color
		{
			set {color = value;}
			get {return color;}
		}
		
		public double StrokeWidth
		{
			set {strokeWidth = Math.Max(value, 0.1);}
			get {return strokeWidth;}
		}
		
		public PointF Start
		{
			get {return start;}
			set {start = value;}
		}
		
		public PointF End
		{
			get {return end;}
			set {end = value;}
		}
		
		public PdfLine(PdfDocument pdfDocument, PointF start, PointF end, Color color, double strokeWidth)
		{
			this.PdfDocument = pdfDocument;
			this.start = start;
			this.end = end;
			this.color = color;
			this.StrokeWidth = strokeWidth;
		}
		
		internal string ToLineStream()
		{				
			return String.Format("{0} {1} m {2} {3} l S\n", Utility.FormatDecimal(start.X), Utility.FormatDecimal(this.PdfDocument.PH-this.start.Y), Utility.FormatDecimal(end.X), Utility.FormatDecimal(this.PdfDocument.PH-this.end.Y));
		}

		internal string ToLineStreamWithColorAndWidth()
		{
			return String.Format("{0} {1} w\n{2}", Utility.ColorRGLine(this.color), Utility.FormatDecimal(strokeWidth), this.ToLineStream());
		}
		
		internal override int StreamWrite(System.IO.Stream stream)
		{
			PdfWriter wr = new PdfWriter(this.id);
			wr.AddStreamText(this.ToLineStreamWithColorAndWidth() + "\n");
			return wr.Write(stream);
		}

	}
}
