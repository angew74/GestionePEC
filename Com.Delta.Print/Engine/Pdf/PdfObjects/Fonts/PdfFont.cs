
using System;
using System.Text;
using System.Drawing;

namespace Com.Delta.Print.Engine.Pdf
{
	internal abstract class PdfFont : PdfObject
	{
		internal enum FontType {Type1=0, TrueType};

		protected PdfDocument document;
		protected string name, typename, alias;
		protected FontType fontType;

		public string Name
		{
			get{return this.name;}
		}

		public string BaseName
		{
			get{return this.typename;}
		}

		public string Alias
		{
			get{return this.alias;}
			set{alias = value;}
		}

		public FontType PdfFontType
		{
			get {return fontType;}
		}


		internal PdfFont(int id, string name, string typename, PdfDocument document)
		{
			this.name = name;
			this.typename = typename;
			this.id = id;
			this.document = document;
		}

		internal PdfFont(string name, string typename, PdfDocument document) 
		{
			this.name=name;
			this.typename=typename;
			this.document = document;
		}


		internal static string FontToPdfType(System.Drawing.Font f)
		{
			string name="";
			switch (f.Name)
			{
				case "Times New Roman":
					if (f.Bold) name="Times-Bold"; else name="Times-Roman";
					break;
				case "Courier New":
					if (f.Bold) name="Courier-Bold"; else name="Courier";
					break;
				//case "Tahoma":
				//	name="Tahoma";
				//	break;
				//case "Arial":
				//	name="Arial";
				//	break;
				default:
					if (f.Bold) name="Helvetica-Bold"; else name="Helvetica";
					break;
			}
			return name;
		}

		internal static FontType ResolveFontType(System.Drawing.Font f)
		{
			switch (f.Name)
			{
				case "Times New Roman":
				case "Courier New":
				case "Helvetica":
					return FontType.Type1;
				default:
					if (CheckExistance(f))
						return FontType.TrueType;
					else
						return FontType.Type1;
			}
		}

		private static bool CheckExistance(Font font)
		{
			if (PdfDocument.FindTTFFile(font) != null)
				return true;
			else
				return false;
		}

		internal virtual void AddDescriptors()
		{
		}

		internal virtual string PageReference
		{
			get {return this.HeadR;}
		}
		
		internal override int StreamWrite(System.IO.Stream stream)
		{
			return 0;
		}

	}
	
}
