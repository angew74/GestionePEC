using System;
using System.Text;
using System.Collections;

namespace Com.Delta.PrintManager.Engine.Pdf.Fonts
{
	/// <summary>
	/// Summary description for PdfFontDescriptor.
	/// </summary>
	internal class PdfFontDescriptor : PdfObject
	{

		internal enum Style {REGULAR = 0, UNICODE_MAP, TO_UNICODE, CID};

		private Style style = Style.REGULAR;
		private PdfTrueTypeFont ttf;


		public PdfFontDescriptor(PdfTrueTypeFont t, Style s)
		{
			ttf = t;
			style = s;
		}


		internal override int StreamWrite(System.IO.Stream stream)
		{
			switch (style)
			{
				case Style.CID:
					return Stream2(stream);
				case Style.UNICODE_MAP:
					return StreamUnicodeMap(stream);
				case Style.TO_UNICODE:
					return StreamToUnicode(stream);
				default : 
					return StreamRegular(stream);
			}			
		}


		private int StreamRegular(System.IO.Stream stream)
		{
			PdfTrueTypeFont.FontHeader head = ttf.head;
			PdfTrueTypeFont.WindowsMetrics os2 = ttf.os_2;

			StringBuilder sb = new StringBuilder();
			sb.Append(this.HeadObj);

			sb.Append(String.Format("<</FontName/{0}", ttf.Name));
			

			sb.Append("/StemV 80");
			sb.Append(String.Format("/Descent {0}", (int)os2.sTypoDescender * 1000 / head.unitsPerEm));
			sb.Append(String.Format("/Ascent {0}", (int)os2.sTypoAscender * 1000 / head.unitsPerEm));
			sb.Append("/Flags 32");
			sb.Append("/ItalicAngle 0");
			sb.Append(String.Format("/FontFile2 {0}", ttf.HeadR));
			sb.Append(String.Format("/CapHeight {0}", (int)os2.sCapHeight * 1000 / head.unitsPerEm));

			sb.Append(String.Format("/FontBBox[{0} {1} {2} {3}]", (int)head.xMin * 1000 / head.unitsPerEm, (int)head.yMin * 1000 / head.unitsPerEm,
				(int)head.xMax * 1000 / head.unitsPerEm,
				(int)head.yMax * 1000 / head.unitsPerEm));

			sb.Append("/Type/FontDescriptor");
			sb.Append(">>\n");						
			sb.Append("endobj\n");


			Byte[] b = ASCIIEncoding.ASCII.GetBytes(sb.ToString());
			stream.Write(b,0,b.Length);
			return b.Length;

		}

		private int StreamUnicodeMap(System.IO.Stream stream)
		{
			
			Hashtable longTag = ttf.glyphsUsed;
			ArrayList tmp = new ArrayList();
			foreach (object o in longTag.Values) 
			{
				if (o!=null)
					tmp.Add(o);
			}
			Object[] metrics = tmp.ToArray();

			if (metrics.Length == 0)
				return 0;

			StringBuilder buf = new StringBuilder();


			buf.Append(
				"/CIDInit /ProcSet findresource begin\n" +
				"12 dict begin\n" +
				"begincmap\n" +
				"/CIDSystemInfo\n" +
				"<< /Registry (Adobe)\n" +
				"/Ordering (UCS)\n" +
				"/Supplement 0\n" +
				">> def\n" +
				"/CMapName /Adobe-Identity-UCS def\n" +
				"/CMapType 2 def\n" +
				"1 begincodespacerange\n" +
				"<0000><FFFF>\n" +
				"endcodespacerange\n");
			int size = 0;
			for (int k = 0; k < metrics.Length; ++k) 
			{
				if (size == 0) 
				{
					if (k != 0) 
					{
						buf.Append("endbfrange\n");
					}
					size = Math.Min(100, metrics.Length - k);
					buf.Append(size).Append(" beginbfrange\n");
				}
				--size;
				int[] metric = (int[])metrics[k];
				string fromTo = ToHex(metric[0]);
				buf.Append(fromTo).Append(fromTo).Append(ToHex(metric[2])).Append("\n");
			}
			buf.Append(
				"endbfrange\n" +
				"endcmap\n" +
				"CMapName currentdict /CMap defineresource pop\n" +
				"end end");

			

			string s = buf.ToString();

			Byte[] part2 = Utility.Deflate(s);

			StringBuilder sb = new StringBuilder();
			sb.Append(this.HeadObj);
			sb.Append("<</Filter/FlateDecode/Length " + part2.Length + " >>\n");
			sb.Append("stream\n");

			Byte[] part1 = ASCIIEncoding.ASCII.GetBytes(sb.ToString());
			Byte[] part3 = ASCIIEncoding.ASCII.GetBytes("\nendstream\nendobj\n");




			
			stream.Write(part1,0,part1.Length);
			stream.Write(part2,0,part2.Length);
			stream.Write(part3,0,part3.Length);

			return part1.Length + part2.Length + part3.Length;


		}

		private int StreamToUnicode(System.IO.Stream stream)
		{

			Hashtable longTag = ttf.glyphsUsed;
			ArrayList tmp = new ArrayList();
			foreach (object o in longTag.Values) 
			{
				if (o!=null)
					tmp.Add(o);
			}
			Object[] metrics = tmp.ToArray();

			StringBuilder buf = new StringBuilder();
			buf.Append(this.HeadObj);

			buf.Append(String.Format("<</ToUnicode {0}", ttf.GetUnicodeMapDescriptor().HeadR));
			buf.Append(String.Format("/DescendantFonts [{0}]", ttf.GetCidDescriptor().HeadR));
			buf.Append("/Subtype/Type0/Encoding/Identity-H");
			buf.Append(String.Format("/BaseFont/{0}", ttf.Name));

			buf.Append("/Type/Font>>\nendobj\n");						


			Byte[] b = ASCIIEncoding.ASCII.GetBytes(buf.ToString());
			stream.Write(b,0,b.Length);
			return b.Length;
		}


		private int Stream2(System.IO.Stream stream)
		{

			Hashtable longTag = ttf.glyphsUsed;
			ArrayList tmp = new ArrayList();
			foreach (object o in longTag.Values) 
			{
				if (o!=null)
					tmp.Add(o);
			}
			Object[] metrics = tmp.ToArray();

			StringBuilder buf = new StringBuilder();
			buf.Append(this.HeadObj);

			buf.Append(String.Format("<</BaseFont/{0}", ttf.Name));

			buf.Append("/CIDSystemInfo<</Ordering(Identity)/Registry(Adobe)/Supplement 0>>/CIDToGIDMap/Identity/FontDescriptor "+ttf.GetMainDescriptor().HeadR+"/Subtype/CIDFontType2/DW 1000/W ");


			StringBuilder sb = new StringBuilder("[");
			int lastNumber = -10;
			bool firstTime = true;
			for (int k = 0; k < metrics.Length; ++k) 
			{
				int[] metric = (int[])metrics[k];
				if (metric[1] == 1000)
					continue;
				int m = metric[0];
				if (m == lastNumber + 1) 
				{
					sb.Append(" ").Append(metric[1]);
				}
				else 
				{
					if (!firstTime) 
					{
						sb.Append("]");
					}
					firstTime = false;
					sb.Append(m).Append("[").Append(metric[1]);
				}
				lastNumber = m;
			}

			if (sb.Length == 1) 
			{
				sb.Append("]");
			}
			else if (sb.Length > 1) 
			{
				sb.Append("]]");
			}


			buf.Append(sb.ToString());
			buf.Append("/Type/Font>>");
			buf.Append("\nendobj\n");						


			Byte[] b = ASCIIEncoding.ASCII.GetBytes(buf.ToString());
			stream.Write(b,0,b.Length);
			return b.Length;
		}

		internal static string ToHex(int n) 
		{
			string s = System.Convert.ToString(n, 16);
			return "<0000".Substring(0, 5 - s.Length) + s + ">";
		}

	}
}
