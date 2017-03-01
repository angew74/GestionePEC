
using System;
using System.Collections;
using System.Text;
using System.Drawing;

namespace Com.Delta.PrintManager.Engine.Pdf
{
	/// <summary>
	/// a generic page for a PdfDocument
	/// </summary>
	public class PdfPage : PdfObject
	{
		private string FontsLine
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("Font <<");
				foreach (PdfFont pf in this.PdfDocument.FontList.Keys)
					sb.Append("/" + pf.Alias + " " + pf.PageReference);
				
				sb.Append(">>\n");
				
				return sb.ToString();
				
			}
		}
		private ArrayList Contents;
		internal ArrayList PagePdfObjects;
		
		internal string MediaBoxLine
		{
			get
			{
				return "/MediaBox [0 0 "+this.PdfDocument.PW.ToString("0.##").Replace(",",".")
					+" "+this.PdfDocument.PH.ToString("0.##").Replace(",",".")+"]\n";
			}
		}
		internal string ContentsLine
		{
			get
			{
				string s="/Contents [";
				foreach (PdfObject po in this.PagePdfObjects) 
				{
					if (po.GetType()!=typeof(PdfImage)) s+=po.HeadR;
				}
				s+="]\n";
				return s;
			}
		}
		internal string XObjectLine
		{
			get
			{
				string s="/XObject <<";
				foreach (PdfObject po in this.PagePdfObjects) 
				{
					if (po.GetType()==typeof(PdfImage))
					{
						PdfImage image = po as PdfImage;
						s+="/I"+po.ID+" "+po.HeadR;
						if (image.Transparent)
							s+="/I"+image.mask.ID+" "+image.mask.HeadR;
					}
				}
				s+=" >>\n";
				return s;
			}
		}
		
		internal PdfPage()
		{
			Contents=new ArrayList();
			PagePdfObjects=new ArrayList();
		}

		
		/// <summary>
		/// adds a Pdf Element into this PdfPage.
		/// </summary>
		/// <param name="PdfImage"></param>
		/// <param name="posx"></param>
		/// <param name="posy"></param>
		public void Add(PdfImage PdfImage,double posx,double posy, double width, double height)
		{
			this.PagePdfObjects.Add(PdfImage);
			this.PagePdfObjects.Add(new PdfImageContent(PdfDocument.GetNextId,"I"+PdfImage.ID, PdfImage.ID, PdfImage.Width, PdfImage.Height, posx, this.PdfDocument.PH-posy,width, height));
		}

		/// <summary>
		/// adds a Pdf Element into this PdfPage.
		/// </summary>
		/// <param name="PdfRectangle"></param>
		public void Add(PdfRectangle PdfRectangle)
		{
			PdfRectangle.ID=this.PdfDocument.GetNextId;
			this.PagePdfObjects.Add(PdfRectangle);
		}


		/// <summary>
		/// adds a Pdf Element into this PdfPage.
		/// </summary>
		/// <param name="PdfRectangle"></param>
		public void Add(PdfRichTextBox box)
		{
			box.ID = this.PdfDocument.GetNextId;
			this.PagePdfObjects.Add(box);
		}

		/// <summary>
		/// adds a Pdf Element into this PdfPage.
		/// </summary>
		/// <param name="PdfRectangle"></param>
		internal void Add(PdfCircle pdfCircle)
		{
			pdfCircle.ID = this.PdfDocument.GetNextId;
			this.PagePdfObjects.Add(pdfCircle);
		}


		/// <summary>
		/// adds a Pdf Element into this PdfPage.
		/// </summary>
		/// <param name="PdfLine"></param>
		public void Add(PdfLine PdfLine)
		{
			PdfLine.ID=this.PdfDocument.GetNextId;
			this.PagePdfObjects.Add(PdfLine);
		}


		/// <summary>
		/// adds a Pdf Element into this PdfPage.
		/// </summary>
		/// <param name="PdfTablePage"></param>
		public void Add(PdfTablePage PdfTablePage)
		{
			if (PdfTablePage!=null)
			{				
				PdfTablePage.ID=this.PdfDocument.GetNextId;
				this.PagePdfObjects.Add(PdfTablePage);
			}
		}
		/// <summary>
		/// adds a Pdf Element into this PdfPage.
		/// </summary>
		/// <param name="PdfTextArea"></param>
		public void Add(PdfTextArea PdfTextArea)
		{
			PdfTextArea.ID=this.PdfDocument.GetNextId;
			if (!this.PdfDocument.FontList.Contains(PdfTextArea.font.Name))
				this.PdfDocument.AddFont(PdfTextArea.font);
			this.PagePdfObjects.Add(PdfTextArea);
		}
				
		internal override int StreamWrite(System.IO.Stream stream)
		{
			
			StringBuilder sb = new StringBuilder();
			sb.Append(this.HeadObj);
			sb.Append("<< /Type /Page\n/");
			sb.Append("Parent "+PdfDocument.PdfRoot.HeadR+"\n");
			sb.Append(this.MediaBoxLine);
			sb.Append("/Resources\n");
			sb.Append("<<\n/");
			sb.Append(this.FontsLine);
			sb.Append("/ProcSet [/PDF/ImageC/ImageI/ImageB/Text]\n");
			sb.Append(this.XObjectLine);
			sb.Append(">>\n");
			sb.Append(this.ContentsLine);
			sb.Append(">>\n");
			sb.Append("endobj\n");
			Byte[] b=ASCIIEncoding.ASCII.GetBytes(sb.ToString());
			stream.Write(b,0,b.Length);
			return b.Length;
		}
		

		public PdfPage CreateCopy()
		{
			PdfPage clone = new PdfPage();
			clone.PdfDocument = this.PdfDocument;
			foreach (PdfObject o in this.PagePdfObjects) clone.PagePdfObjects.Add(o);
			return clone;
		}
		
		public void SaveToDocument ()
		{
			this.id = this.PdfDocument.GetNextId;
			this.PdfDocument.AddPdfObject(this);
			foreach (PdfObject o2 in this.PagePdfObjects) 
			{
				this.PdfDocument.AddPdfObject(o2);
			}
			
		}
	}
}

