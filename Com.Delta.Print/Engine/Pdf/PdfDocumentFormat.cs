

using System;

namespace Com.Delta.Print.Engine.Pdf
{
	/// <summary>
	/// Represents the Paper Size of each Page of the Document.
	/// </summary>
	public class PdfDocumentFormat
	{
		internal double height;
		internal double width;

		internal PdfDocumentFormat(double width, double height)
		{
			this.height = height;
			this.width = width;
		}
		
		public static PdfDocumentFormat A4
		{
			get {return PdfDocumentFormat.InCentimeters(21, 29.7);}
		}
		
		public static PdfDocumentFormat A4_Horizontal
		{
			get {return PdfDocumentFormat.InCentimeters(29.7, 21);}
		}
		
		public static PdfDocumentFormat InInches(double width, double height)
		{
			if (width<=0) throw new Exception("Width must be grater than zero.");
			if (height<=0) throw new Exception("Height must be grater than zero.");

			return new PdfDocumentFormat(width*72, height*72);
		}
		
		public static PdfDocumentFormat InCentimeters(double width, double height)
		{
			if (width<=0) throw new Exception("Width must be grater than zero.");
			if (height<=0) throw new Exception("Height must be grater than zero.");
			return new PdfDocumentFormat(width*72/2.54,height*72/2.54);
		}
	}
}
