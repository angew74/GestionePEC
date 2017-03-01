using System;
using System.Text;

namespace Com.Delta.PrintManager.Engine.Pdf
{
	/// <summary>
	/// Summary description for PdfShading.
	/// </summary>
	internal class PdfShading : PdfObject
	{
		int shadingType = 2;

		public PdfShading(int shadingType)
		{
			this.shadingType = shadingType;
		}


		internal override int StreamWrite(System.IO.Stream stream)
		{
			int num=this.id;
	
			System.Text.StringBuilder sb=new StringBuilder();
			sb.Append(num.ToString()+" 0 obj\n");
			sb.Append("<< ShadingType " + shadingType.ToString() + "r\n");
			sb.Append("/ColorSpace /DeviceRGB\r\n");
			sb.Append(">>");

			sb.Append("endobj\n");
			Byte[] b = ASCIIEncoding.ASCII.GetBytes(sb.ToString());
			stream.Write(b,0,b.Length);
			return b.Length;
		}
	}
}
