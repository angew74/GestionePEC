
using System;

namespace Com.Delta.Print.Engine.Pdf
{
	/// <summary>
	/// the abstract pdf object class
	/// </summary>
	public abstract class PdfObject
	{
		private Byte[] buffer;
		protected string type;
		internal PdfDocument PdfDocument;
		protected int id;

		internal PdfObject()
		{
			
		}
		internal PdfObject(Byte[] buffer, int id)
		{
			this.buffer = buffer;
			this.id = id;
		}

		
		#region Properties

		internal int ID
		{
			get {return id;}
			set {id = value;}
		}

		public int PublicID
		{
			get {return this.id;}
		}
		
		internal string Type
		{
			get {return type;}
		}

		internal string HeadR
		{
			get {return this.id.ToString()+" 0 R ";}
		}

		internal string HeadObj
		{
			get {return this.id.ToString()+" 0 obj\n";}
		}
		
		#endregion

		internal virtual int StreamWrite(System.IO.Stream stream)
		{
			return 0;
		}

		internal PdfObject Clone()
		{
			return this.MemberwiseClone() as PdfObject;
		}
	}
}
