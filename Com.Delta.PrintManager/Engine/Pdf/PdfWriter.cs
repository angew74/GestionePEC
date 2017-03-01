using System;
using System.IO;
using System.Text;

namespace Com.Delta.PrintManager.Engine.Pdf
{
	/// <summary>
	/// Summary description for PdfStream.
	/// </summary>
	public class PdfWriter
	{
		private string header = String.Empty;
		private int id = 0;
		private byte[] streamContent = new byte[]{};
		private string streamText = String.Empty;

		public PdfWriter(int id)
		{
			this.id = id;
		}

		public PdfWriter AddHeader(string text)
		{
			header += text;
			return this;
		}

		public PdfWriter AddHeader(string key, string item)
		{
			return AddHeader(key + item);
		}

		public string Header
		{
			get { return String.Format("{0} 0 obj\n<<{1}>>\n", id, header);}
		}

		public void AddStreamText(string text)
		{
			streamText = text;
			AddStreamContent(ASCIIEncoding.ASCII.GetBytes(text));
		}

		public void AddStreamContent(byte[] content)
		{			
			streamContent = content;
			AddHeader("/Length " + streamContent.Length);
		}

		public int Write(Stream stream)
		{
			int length = 0;
			byte[] headerContent = ASCIIEncoding.ASCII.GetBytes(this.Header);
			byte[] footerContent = ASCIIEncoding.ASCII.GetBytes("endobj\n\n");

			stream.Write(headerContent, 0, headerContent.Length);
			length += headerContent.Length;

			if (streamContent.Length > 0)
			{
				byte[] streamStart = ASCIIEncoding.ASCII.GetBytes("stream\n");
				byte[] streamEnd = ASCIIEncoding.ASCII.GetBytes("\nendstream\n");
				stream.Write(streamStart, 0, streamStart.Length);
				stream.Write(streamContent, 0, streamContent.Length);
				stream.Write(streamEnd, 0, streamEnd.Length);
				length += (streamStart.Length + streamContent.Length + streamEnd.Length);
			}
			
			stream.Write(footerContent, 0, footerContent.Length);
			length += footerContent.Length;

			return length;
		}

	}
}
