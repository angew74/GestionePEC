using System;
using System.Text;

namespace Com.Delta.Print.Engine.Pdf.Fonts
{
	/// <summary>
	/// Summary description for Type1Font.
	/// </summary>
	internal class Type1Font : PdfFont
	{
		public Type1Font(int id, string name, PdfDocument document):base(id, name, name, document)
		{
			this.fontType = FontType.Type1;
		}

		public Type1Font(string name, PdfDocument document): base(name, name, document)
		{
			this.fontType = FontType.Type1;
		}

		internal override int StreamWrite(System.IO.Stream stream)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(this.HeadObj);
			sb.Append(String.Format("<</Type/Font/Subtype/Type1/Name/{0}/BaseFont/{1}/Encoding/WinAnsiEncoding>>\n", this.Name, typename));
			sb.Append("endobj\n");

			Byte[] b = ASCIIEncoding.ASCII.GetBytes(sb.ToString());
			stream.Write(b, 0, b.Length);
			return b.Length;
			
		}
	}
}
