
using System;
using System.Text;

namespace Com.Delta.Print.Engine.Pdf
{
	internal class PdfHeader : PdfObject
	{
		private string subject,title,author,creationdate;

		internal PdfHeader(PdfDocument PdfDocument,string subject,string title,string author)
		{
			this.PdfDocument=PdfDocument;
			this.id=this.PdfDocument.GetNextId;
			this.subject=subject;
			this.title=title;
			this.author=author;
			this.creationdate = DateTime.Today.ToShortDateString();
		}


		internal override int StreamWrite(System.IO.Stream stream)
		{
			string text = "/Subject(" + subject + ")/Title(" + title + ")/Creator (Stampa Reports System)/Producer(Stampa Reports System)/Author (Stampa Reports System)/CreationDate (" + creationdate + ")\n";

			PdfWriter wr = new PdfWriter(this.id);
			wr.AddHeader(text);
			return wr.Write(stream);
		}

	}
}