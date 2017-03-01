
using System;
using System.Text;

namespace Com.Delta.Print.Engine.Pdf
{
	internal class PdfCatalog : PdfObject
	{
		internal PdfCatalog(PdfDocument pdfDocument)
		{
			this.PdfDocument = pdfDocument;
			this.id = this.PdfDocument.GetNextId;
		}

		private string FirstKid
		{
			get
			{
				for (int x=1;x<this.PdfDocument._nextid;x++)
				{
					object o=this.PdfDocument.PdfObjects[x.ToString()+" 0 obj\n"];
					if (o!=null)
						if (o.GetType()==typeof(PdfPage)) return((PdfObject)o).HeadR;
				}
				return null;
			}
		}
		internal override int StreamWrite(System.IO.Stream stream)
		{
			string s="";
			s+=this.HeadObj;
			s+="<<\n";
			s+="/Type /Catalog\n";
			s+="/Pages "+this.PdfDocument.PdfRoot.HeadR+"\n";
			s+="/OpenAction["+this.FirstKid+"/Fit]\n";
			s+=">>\n";
			s+="endobj\n";
			Byte[] b=ASCIIEncoding.ASCII.GetBytes(s);
			stream.Write(b,0,b.Length);
			return b.Length;
		}

	}
}
