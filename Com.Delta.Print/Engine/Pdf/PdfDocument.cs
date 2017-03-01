

using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Drawing;

using Com.Delta.Print.Engine.Pdf.Fonts;

namespace Com.Delta.Print.Engine.Pdf
{
	/// <summary>
	/// The base Pdf Document.
	/// </summary>
	public class PdfDocument
	{
		// font variables
		private Hashtable fontList = new Hashtable();
		private static Hashtable trueTypeFonts = new Hashtable();

				
		private PdfDocumentFormat pdfDocumentFormat = PdfDocumentFormat.A4;
		internal static bool FlateCompression = true;
		internal Hashtable PdfObjects;
		internal int _nextid = 0;
		private System.IO.Stream ms;


		#region Internal Properties

		internal double PH
		{
			get {return this.pdfDocumentFormat.height;}
		}

		internal double PW
		{
			get {return this.pdfDocumentFormat.width;}
		}

		internal int GetNextId
		{
			get
			{
				_nextid++;
				return _nextid;
			}
		}
		
		internal PdfObject Header
		{
			get
			{
				foreach (PdfObject po in this.PdfObjects.Values)
					if (po.GetType()==typeof(PdfHeader)) return po;
				return null;
			}
			set
			{
				foreach (PdfObject po in this.PdfObjects.Values)
					if (po.GetType()==typeof(PdfHeader))
					{
						this.PdfObjects.Remove(po.HeadObj);
						break;
					}
				this.PdfObjects.Add(value.HeadObj,value);
			}
		}

		internal PdfObject Catalog
		{
			get
			{
				foreach (PdfObject po in this.PdfObjects.Values)
					if (po.GetType()==typeof(PdfCatalog)) return po;
				return null;
			}
		}

		internal PdfObject PdfRoot
		{
			get
			{
				foreach (PdfObject po in this.PdfObjects.Values)
					if (po.GetType()==typeof(PdfRoot)) return po;
				return null;
			}
		}

		public PdfObject GetObject(int id)
		{
			foreach (PdfObject po in this.PdfObjects.Values)
			{
				if (po.ID == id)
					return po;
			}
			return null;
		}


		internal Hashtable FontList
		{
			get {return fontList;}
		}


		#endregion

		#region Public Properties

		public int PageCount
		{
			get
			{
				int count=0;
				foreach (PdfObject po in this.PdfObjects.Values)
				{
					if (po.GetType()==typeof(PdfPage)) 
						count++;
				}
				return count;
			}
		}

		#endregion

		#region Public Methods

		public void SaveToStream(System.IO.Stream m)
		{
			//foreach(PdfFont f in fontList.Keys)
			//{
			//	f.AddDescriptors();
			//}

			this.AddPdfObject(new PdfCatalog(this));
			this.AddPdfObject(new PdfRoot(this));
			this.ms = m;

			try 
			{
				Send("%PDF-1.4\n");
			}
			catch {throw new Exception("Error writing to the output stream.");}
			

			StringBuilder xref = new StringBuilder("xref\n0 " + (this.PdfObjects.Count+1).ToString() + "\n0000000000 65535 f \n");
			long pos = 9;
			for (int x=1;x<=this._nextid;x++)
			{
				PdfObject o = this.PdfObjects[x.ToString()+" 0 obj\n"] as PdfObject;
				if (o!=null)
				{					
					xref.Append( Utility.xRefFormatting(pos) + " 00000 n \n");
					try	
					{
						pos += o.StreamWrite(ms);
					}
					catch (Exception ex) 
					{
						Console.WriteLine(ex.StackTrace);
						throw new Exception("Error generating the document.");
					}
				}
				else
				{
					xref.Append( Utility.xRefFormatting(pos) + " 00000 n \n");
				}
			}
			
			long startxref = pos;
			
			Send(xref.ToString());
			Send("trailer\n<<\n/Size " + (this.PdfObjects.Count+1).ToString() + "\n/Root " + this.Catalog.HeadR + "\n/Info " + this.Header.HeadR + "\n>>\n");						
			Send("startxref\n" + startxref + "\n");
			Send("%%EOF\n");
			
		}

		public void SaveToFile(string filename)
		{
			try
			{
				FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
				SaveToStream(fs);
				fs.Close();
	
			}
			catch
			{
				throw new Exception("Error writing file");
			}
		}
		
		public void SetHeaders(string subject, string title, string author)
		{
			if (subject==null) throw new Exception("string Subject cannot be null.");
			if (title==null) throw new Exception("string Title cannot be null.");
			this.Header = new PdfHeader(this,subject,title,author);
		}

		
		public void SetPageFormat(PdfDocumentFormat pdfDocumentFormat)
		{
			this.pdfDocumentFormat = pdfDocumentFormat;
		}


		public PdfPage NewPage()
		{
			PdfPage page = new PdfPage();
			page.PdfDocument = this;
			return page;
		}

		
		public PdfImage NewImage(string file)
		{
			PdfImage pi;
			try 
			{
				pi=new PdfImage(this.GetNextId,file);
				this.AddPdfObject(pi);
				if (pi.Transparent)
				{
					pi.mask.ID = this.GetNextId;
					this.AddPdfObject(pi.mask);
				}
			}
			catch {throw new Exception("Error opening the Image File");}			
			return pi;
		}

		
		public PdfImage NewImage(string file, double width, double height)
		{
			PdfImage pi;
			try 
			{
				pi = new PdfImage(this.GetNextId, file, width, height);
				this.AddPdfObject(pi);
				if (pi.Transparent)
				{
					pi.mask.ID = this.GetNextId;
					this.AddPdfObject(pi.mask);
				}
			}
			catch {throw new Exception("Error opening the Image File");}
			
			return pi;
		}


		public PdfImage NewImage(Bitmap bitmap)
		{
			PdfImage pi;
			try 
			{
				pi = new PdfImage(this.GetNextId, bitmap);
				this.AddPdfObject(pi);
				if (pi.Transparent)
				{
					pi.mask.ID = this.GetNextId;
					this.AddPdfObject(pi.mask);
				}
			}
			catch {throw new Exception("Error getting Bitmap");}			
			return pi;	
		}

		
		public PdfTable NewTable(Font DefaultFont, int Rows, int Columns, double CellPadding)
		{
			if (Rows<=0) throw new Exception("Rows must be grater than zero.");
			if (Columns<=0) throw new Exception("Columns must be grater than zero.");
			if (CellPadding<0) throw new Exception("CellPadding must be non-negative.");

			PdfTable pt = new PdfTable(this, ContentAlignment.TopCenter, DefaultFont, Color.Black, Rows, Columns, CellPadding);
			pt.header = new PdfTable(this, ContentAlignment.MiddleCenter, DefaultFont, Color.Black, 1, Columns, CellPadding);
			
			return pt;
		}

		#endregion

		#region Internal Methods

		internal void AddPdfObject(PdfObject po)
		{
			po.PdfDocument=this;
			if (!this.PdfObjects.ContainsKey(po.HeadObj))
			{
				this.PdfObjects.Add(po.HeadObj,po);
			}
		}

		internal PdfFont AddFont(System.Drawing.Font f)
		{
			PdfFont pf = null;
			PdfFont.FontType fontType = PdfFont.ResolveFontType(f);

			string name = (fontType == PdfFont.FontType.Type1) ? PdfFont.FontToPdfType(f) : PdfDocument.GetFullFontName(f).Replace(" ","");
		

			
			if (this.ContainsFont(name)) 
			{
				pf = GetFont(name);	
			}
			else
			{

				if (fontType == PdfFont.FontType.Type1)
				{
					pf = new Type1Font(name, this);
				}
				else
				{
					pf = new PdfTrueTypeFont(f, this);
				}


				pf.ID=this.GetNextId;
				this.AddPdfObject(pf);
				pf.AddDescriptors();
				pf.Alias = String.Format("F{0}", fontList.Count);

				fontList[pf] = pf.Alias;
			}


			return pf;
		}

		#endregion

		#region Private Methods

		private void Send(string strMsg)
		{
			Byte[] buffer = null;
			buffer = ASCIIEncoding.ASCII.GetBytes(strMsg);
			ms.Write(buffer, 0, buffer.Length); 
		}

		private bool ContainsFont(string fontName)
		{
			bool r = false;
			foreach (PdfFont pf2 in this.FontList.Keys)
			{
				if (pf2.BaseName == fontName) 
				{
					return true;
				}
			}

			return false;
		}
		

		private PdfFont GetFont(string fontName)
		{	
			foreach (PdfFont pdfFont in FontList.Keys)
			{
				if (pdfFont.BaseName == fontName) return pdfFont;
			}
			return null;
		}

		#endregion

		#region Constructors

		static PdfDocument()
		{
			FontFileResolver fontFileResolver = new FontFileResolver();
			trueTypeFonts = fontFileResolver.GetFontList();
		}
		

		public PdfDocument()
		{
			PdfObjects = new Hashtable();
			this.AddPdfObject(new PdfHeader(this, "", "", ""));				
		}


		public PdfDocument(PdfDocumentFormat DocumentFormat) : this()
		{
			this.SetPageFormat(DocumentFormat);			
		}
	
		#endregion

		#region Static Mathods

		public static string FindTTFFile(Font font)
		{
			string fullFontName = GetFullFontName(font);
			if (trueTypeFonts[fullFontName] != null)
				return trueTypeFonts[fullFontName] as string;
			else
				return trueTypeFonts[font.Name] as string;
		}

		public static string GetFullFontName(Font font)
		{
			StringBuilder sb = new StringBuilder(font.Name);

			if (font.Bold)
				sb.Append(" Bold");
			if (font.Italic)
				sb.Append(" Italic");

			return sb.ToString();
		}
		
		#endregion
		
		
	}
}
