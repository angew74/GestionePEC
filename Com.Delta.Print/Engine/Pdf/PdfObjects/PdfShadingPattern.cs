using System;
using System.Text;
using System.IO;

namespace Com.Delta.Print.Engine.Pdf
{
	/// <summary>
	/// Summary description for PdfShadingPattern.
	/// </summary>
	internal class PdfShadingPattern : PdfObject
	{
		private PdfShading pdfShading = null;


		public PdfShadingPattern(PdfShading pdfShading)
		{
			this.pdfShading = pdfShading;
		}

		internal override int StreamWrite(System.IO.Stream stream)
		{
			int num=this.id;
	
			System.Text.StringBuilder sb=new StringBuilder();
			sb.Append(num.ToString()+" 0 obj\n");
			sb.Append("<< Type /Pattern\n");
			sb.Append("PatternType 2\n");
			sb.Append("Shading " + pdfShading.HeadR + "\n");
			sb.Append(">>");

			sb.Append("endobj\n");
			Byte[] b = ASCIIEncoding.ASCII.GetBytes(sb.ToString());
			stream.Write(b,0,b.Length);
			return b.Length;
		}

	}
}
