
using System;
using System.Drawing;
using System.Collections;
using System.Text;
using Com.Delta.PrintManager.Engine.ZipLib.Zip.Compression.Streams;

namespace Com.Delta.PrintManager.Engine.Pdf
{
	internal class Utility
	{
		internal static int[] TimesNewRomanRegular=new int[]{481,481,481,481,481,481,481,481,481,0,0,481,481,0,481,481,481,481,481,481,481,481,481,481,481,481,481,481,0,0,0,0,155,206,253,310,310,516,481,112,206,206,309,349,155,207,155,173,309,310,309,309,310,310,310,310,310,310,172,173,349,349,349,275,570,447,413,412,447,379,344,447,447,207,241,446,379,550,447,447,344,447,413,344,379,447,447,584,447,447,378,206,172,206,291,309,206,274,310,275,310,275,206,310,310,172,172,310,172,482,310,310,309,310,206,241,173,310,310,446,310,310,275,297,124,297,335,481,481,481,481,481,0,0,481,481,481,481,481,481,481,481,481,481,481,481,481,481,481,481,481,481,481,481,481,481,481,481,481,481,155,206,310,310,309,310,124,309,206,470,171,310,349,207,470,309,248,340,186,186,206,357,281,155,206,186,192,310,464,464,464,275,447,447,447,447,447,447,550,412,379,379,379,379,207,207,207,207,447,447,447,447,447,447,447,349,447,447,447,447,447,447,344,309,274,274,274,274,274,274,413,275,275,275,275,275,172,172,172,172,310,310,310,310,310,310,310,339,310,310,310,310,310,310};
		internal static int[] TimesNewRomanBold=new int[]{493,493,493,493,493,493,493,493,493,0,0,493,493,0,493,493,493,493,493,493,493,493,493,493,493,493,493,493,0,0,0,0,154,206,342,309,309,618,515,171,206,205,308,352,154,206,154,171,308,309,309,308,309,309,309,309,309,309,205,206,352,352,352,308,575,446,412,446,446,412,377,481,481,241,308,480,412,583,446,481,378,481,446,343,411,446,446,618,446,446,412,206,171,205,359,309,205,309,344,274,343,274,205,309,343,172,205,343,172,514,343,308,343,343,274,240,205,344,309,446,309,309,274,244,136,243,321,493,493,493,493,493,0,0,493,493,493,493,493,493,493,493,493,493,493,493,493,493,493,493,493,493,493,493,493,493,493,493,493,493,154,205,309,309,309,309,136,309,205,462,185,308,352,206,462,309,247,339,184,185,206,356,334,154,205,185,203,308,463,463,463,308,446,446,446,446,446,446,618,446,412,412,412,412,241,241,241,241,446,446,481,481,481,481,481,352,481,446,446,446,446,446,378,343,309,309,309,309,309,309,446,274,274,274,274,274,171,172,171,172,308,343,308,308,308,308,308,339,308,344,344,344,344,309};
		internal static int[] HelveticaRegular=new int[]{464,464,464,464,464,464,464,464,464,0,0,464,464,0,464,464,464,464,464,464,464,464,464,464,464,464,464,464,0,0,0,0,172,173,220,344,344,550,413,119,207,206,241,362,172,206,173,172,344,344,345,344,344,344,344,344,345,344,172,172,362,362,362,344,628,412,413,447,446,413,378,481,447,172,310,413,344,515,446,481,412,481,447,413,378,447,412,584,413,412,378,172,172,172,291,344,206,344,344,310,344,344,172,344,345,137,137,309,138,516,345,344,345,344,206,309,172,344,310,447,309,309,310,207,161,207,361,464,464,464,464,464,0,0,464,464,464,464,464,464,464,464,464,464,464,464,464,464,464,464,464,464,464,464,464,464,464,464,464,464,172,206,344,345,344,344,161,344,206,455,229,344,362,206,455,342,248,340,206,206,207,356,333,172,206,206,226,344,515,515,516,378,412,412,412,412,412,412,618,447,413,413,413,413,172,172,173,172,447,446,481,481,481,481,481,361,481,447,447,447,447,412,412,378,344,344,344,344,344,344,550,310,344,344,344,344,172,172,173,0,345,345,344,344,344,344,344,340,378,344,344,344,344,309};
		internal static int[] HelveticaBold=new int[]{476,476,476,476,476,476,476,476,476,0,0,476,476,0,476,476,476,476,476,476,476,476,476,476,476,476,476,476,0,0,0,0,172,207,294,344,345,550,447,148,206,207,241,361,172,206,172,173,344,344,344,344,344,345,344,344,345,344,206,206,361,361,361,378,604,447,447,447,447,413,378,481,447,172,344,447,378,516,447,481,413,481,447,413,378,447,413,584,413,413,379,206,173,206,362,345,206,345,379,344,378,344,206,378,378,172,172,344,172,550,379,379,378,379,242,345,207,378,344,482,344,344,309,241,174,241,361,476,476,476,476,476,0,0,476,476,476,476,476,476,476,476,476,476,476,476,476,476,476,476,476,476,476,476,476,476,476,476,476,476,172,207,344,344,344,344,174,344,206,456,229,344,361,206,456,342,248,340,206,206,206,357,344,172,206,207,226,345,516,516,516,379,447,447,447,447,447,447,618,447,413,413,413,413,172,173,172,172,447,447,481,481,481,481,481,362,481,447,447,447,447,413,413,378,345,345,345,345,345,345,550,344,344,344,344,344,172,173,172,172,379,379,379,379,379,379,379,340,378,378,378,378,378,344};
		
		internal static Hashtable wordWidthsCache = new Hashtable();

		//internal static Hashtable 

		private static System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
		
		internal static string BCR(Color Color)
		{
			return FormatDecimal(((double)Color.R)/255);
		}

		internal static string BCG(Color Color)
		{
			return FormatDecimal(((double)Color.G)/255);
		
		}

		internal static string BCB(Color Color)
		{
			return FormatDecimal(((double)Color.B)/255);
		}

		internal static string ColorRGLine(Color Color)
		{
			/*
			System.Text.StringBuilder sb=new StringBuilder();
			sb.Append(BCR(Color));
			sb.Append(" ");
			sb.Append(BCG(Color));
			sb.Append(" ");
			sb.Append(BCB(Color));
			sb.Append(" RG\n");
			return sb.ToString();
			*/

			return BCR(Color) + " " + BCG(Color) + " " + BCB(Color) + " RG\n";
		}
		internal static string ColorrgLine(Color Color)
		{
			return BCR(Color) + " " + BCG(Color) + " " + BCB(Color) + " rg\n";
		}

		internal static Byte[] Deflate(string text)
		{
			Byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(text);
			return Utility.Deflate(b);
		}

		internal static Byte[] Deflate(Byte[] b)
		{
			
			System.IO.MemoryStream ms = new System.IO.MemoryStream();

			
			DeflaterOutputStream outStream =new DeflaterOutputStream(ms);
			
			outStream.Write(b, 0, b.Length);
			outStream.Flush();
			outStream.Finish();
			
					
			
			Byte[] result=ms.ToArray();
			outStream.Close();
			ms.Close();


			return result;
		}

		internal static string xRefFormatting(long xValue)
		{
			string strMsg = xValue.ToString();
			return strMsg.PadLeft(10, '0');

		}

		internal static double Measure(Font Font,string text)
		{
			string key=text+Font.Name.ToString()+Font.Size.ToString()
				+Font.Bold.ToString()+Font.Italic.ToString();
			object o=wordWidthsCache[key];
			if (o==null)
			{
				double dbl=-Utility.Difference(Font,0,text);
				wordWidthsCache.Add(key,dbl);
				return dbl;
			}
			return (double)o;
		}

		private static double Difference(System.Drawing.Font font,double Width,string text)
		{
			text=System.Text.RegularExpressions.Regex.Replace(text,@"\\\d\d\d","A");
			text=text.Replace(@"\\",@"\");

			switch (font.Name)
			{
				case "Courier New":
					return Width-(text.Length*font.Size)/1.66666;
					
				case "Times New Roman":
					if (font.Bold) 
					{
						double dim=0;
						int l=text.Length;
						for (int index=0;index<l;index++)
						{
							char c=text[index];
							if (c<255)	dim+=Utility.TimesNewRomanBold[c];
							else
							{
								dim+=Utility.TimesNewRomanBold['A'];
							}
						}
						return Width-(dim*font.Size)/618;
					}
					else
					{
						double dim=0;
						int l=text.Length;
						for (int index=0;index<l;index++)
						{
							char c=text[index];
							if (c<255)	dim+=Utility.TimesNewRomanRegular[c];
							else
							{
								dim+=Utility.TimesNewRomanRegular['A'];
							}
						}
						return Width-(dim*font.Size)/618;
					}
				default:
					if (font.Bold) 
					{
						double dim=0;
						int l=text.Length;
						for (int index=0;index<l;index++)
						{
							char c=text[index];
							if (c<255)	dim+=Utility.HelveticaBold[c];
							else
							{
								dim+=Utility.HelveticaBold['A'];
							}
						}
						return Width-(dim*font.Size)/618;
					}
					else
					{
						double dim=0;
						int l=text.Length;
						for (int index=0;index<l;index++)
						{
							char c=text[index];
							if (c<255) dim+=Utility.HelveticaRegular[c];
							else
							{
								dim+=Utility.HelveticaBold['A']; 
							}
						}
						return Width-(dim*font.Size)/618;
					}
					
			}
		}

		
		internal static string FontToFontLine(Font Font)
		{
			return "/" + PdfFont.FontToPdfType(Font) + " " + Font.Size.ToString() + " Tf\n";
		}

		internal static string FormatDecimal(double number)
		{			
			return number.ToString("0.##", ci.NumberFormat); 
		}

		internal static string FormatDecimal(float number)
		{			
			return number.ToString("0.##", ci.NumberFormat); 
		}


		internal static byte[] Combine(string a, string end, byte[] s)
		{
			Byte[] part1=System.Text.ASCIIEncoding.ASCII.GetBytes(a);
			Byte[] part3=System.Text.ASCIIEncoding.ASCII.GetBytes(end);

			Byte[] res = new byte[part1.Length+s.Length+part3.Length];

			Array.Copy(part1,0,res,0,part1.Length);
			Array.Copy(s,0,res,part1.Length,s.Length);
			Array.Copy(part3,0,res,part1.Length+s.Length,part3.Length);

			return res;
		}

		internal static void EscapeString(byte[] b, ByteBuffer content) 
		{
			for (int k = 0; k < b.Length; ++k) 
			{
				byte c = b[k];
				switch ((int)c) 
				{
					case '\r':
						content.Append("\\r");
						break;
					case '\n':
						content.Append("\\n");
						break;
					case '\t':
						content.Append("\\t");
						break;
					case '\b':
						content.Append("\\b");
						break;
					case '\f':
						content.Append("\\f");
						break;
					case '(':
					case ')':
						content.Append_i('\\').Append_i(c);
						break;
					case '\\':
						content.Append_i('\\').Append_i(c);												
						break;
					default:
						content.Append_i(c);
						break;
				}
			}
		}

		internal static void EscapeStringWeak(byte[] b, ByteBuffer content) 
		{
			for (int k = 0; k < b.Length; ++k) 
			{
				byte c = b[k];
				switch ((int)c) 
				{
					case '\r':
						content.Append("\\r");
						break;
					case '\n':
						content.Append("\\n");
						break;
					case '\t':
						content.Append("\\t");
						break;
					case '\b':
						content.Append("\\b");
						break;
					case '\f':
						content.Append("\\f");
						break;
					default:
						content.Append_i(c);
						break;
				}
			}
		}

		internal static string TextEncode(string text)
		{
			text=text.Replace("\\","\\\\");
			text=text.Replace("(","\\050").Replace(")","\\051");

			text=text.Replace("À","\\300");
			text=text.Replace("Á","\\301");
			text=text.Replace("Â","\\302");
			text=text.Replace("Ã","\\303");
			text=text.Replace("Ä","\\304");
			text=text.Replace("Æ","\\306");
			text=text.Replace("Ç","\\307");

			text=text.Replace("È","\\310");
			text=text.Replace("É","\\311");
			text=text.Replace("Ê","\\312");
			text=text.Replace("Ë","\\313");

			text=text.Replace("Ì","\\314");
			text=text.Replace("Í","\\315");
			text=text.Replace("Î","\\316");
			text=text.Replace("Ï","\\317");

			text=text.Replace("Ñ","\\321");
			text=text.Replace("Œ","\\214");

			text=text.Replace("Ò","\\322");
			text=text.Replace("Ó","\\323");
			text=text.Replace("Ô","\\324");
			text=text.Replace("Õ","\\325");
			text=text.Replace("Ö","\\326");
			text=text.Replace("Ø","\\330");
			text=text.Replace("Œ","\\214");

			text=text.Replace("Š","\\212");

			text=text.Replace("Ù","\\331");
			text=text.Replace("Ú","\\332");
			text=text.Replace("Û","\\333");
			text=text.Replace("Ü","\\334");

			text=text.Replace("Ý","\\335");
			text=text.Replace("Ÿ","\\237");

			text=text.Replace("Ž","\\216");

			text=text.Replace("à","\\340");
			text=text.Replace("á","\\341");
			text=text.Replace("â","\\342");
			text=text.Replace("ä","\\344");
			text=text.Replace("å","\\345");
			text=text.Replace("æ","\\346");
			
			text=text.Replace("ç","\\347");

			

			text=text.Replace("è","\\350");
			text=text.Replace("è","\\351");
			text=text.Replace("ê","\\352");
			text=text.Replace("ë","\\353");

			text=text.Replace("ì","\\354");
			text=text.Replace("í","\\355");
			text=text.Replace("î","\\356");
			text=text.Replace("ï","\\357");

			text=text.Replace("ñ","\\361");

			text=text.Replace("ò","\\362");
			text=text.Replace("ó","\\363");
			text=text.Replace("ô","\\364");
			text=text.Replace("õ","\\365");
			text=text.Replace("ö","\\366");
			text=text.Replace("ø","\\370");
			text=text.Replace("œ","\\234");

			text=text.Replace("š","\\232");


			text=text.Replace("ù","\\371");
			text=text.Replace("ú","\\372");
			text=text.Replace("û","\\373");
			text=text.Replace("ü","\\374");

			text=text.Replace("ý","\\375");
			text=text.Replace("ÿ","\\377");

			text=text.Replace("ž","\\236");

			text=text.Replace("ß","\\337");

			
			text=text.Replace("ƒ","\\203").Replace("™","\\231").Replace("®","\\256").Replace("©","\\251");
			text=text.Replace("€","\\200").Replace("£","\\243").Replace("¢","\\242");
			text=text.Replace("°","\\272").Replace("§","\\247").Replace("¥","\\245");
			text=text.Replace("±","\\261").Replace("‰","\\211").Replace("µ","\\265");

			return text;
		}
	
	}
}
