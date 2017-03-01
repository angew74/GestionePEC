
using System;
using System.Text;

namespace Com.Delta.PrintManager.Engine.Pdf
{
	internal class PdfRoot : PdfObject
	{
		internal PdfRoot(PdfDocument pdfDocument)
		{
			this.PdfDocument = pdfDocument;
			this.id = this.PdfDocument.GetNextId;
		}


		private string GetKidsLine()
		{
			string s="";
			for (int x=1;x<this.PdfDocument._nextid;x++)
			{
				object o = this.PdfDocument.PdfObjects[x.ToString()+" 0 obj\n"];
				if (o!=null)
				{
					if (o.GetType()==typeof(PdfPage))
						s+=((PdfObject)o).HeadR;
				}
			}
			return "/Kids ["+s+"]\n";
			
		}

		internal override int StreamWrite(System.IO.Stream stream)
		{
			string s="";
			s+=this.HeadObj;
			s+="<<\n";
			s+="/Type /Pages\n/Count "+this.PdfDocument.PageCount+"\n"+this.GetKidsLine();
			s+=">>\n";
			s+="endobj\n";

			Byte[] b = ASCIIEncoding.ASCII.GetBytes(s);
			stream.Write(b, 0, b.Length);
			return b.Length;
		}

	}
}
