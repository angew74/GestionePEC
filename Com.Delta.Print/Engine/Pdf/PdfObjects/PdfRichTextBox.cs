using System;
using System.Collections;
using System.Drawing;
using System.Text;

using Com.Delta.Print.Engine.Pdf.Fonts;

namespace Com.Delta.Print.Engine.Pdf
{
	/// <summary>
	/// Summary description for PdfRichTextBox.
	/// </summary>
	public class PdfRichTextBox : PdfObject
	{
		private Rectangle bounds = new Rectangle(120,120, 50, 90);
		private System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");

		public PdfRichTextBox(PdfDocument document, int x, int y, int width, int height)
		{
			this.PdfDocument = document;
			bounds = new Rectangle(x, y, width, height);
		}

		private ArrayList lines = new ArrayList();


		internal void SetLines(ArrayList content)
		{
			this.lines = content;


			foreach(TextLine line in lines)
			{
				foreach(TextSegment segment in line.Segments)
				{
					PdfFont pdfFont = this.PdfDocument.AddFont(segment.Font);

					if (pdfFont.PdfFontType == PdfFont.FontType.TrueType)
					{
						((PdfTrueTypeFont)pdfFont).CalculateMetrics(segment.Text);
					}
				}
			}
		}


		private byte[] ToStream()
		{
			
			

			ByteBuffer sb = new ByteBuffer();
			sb.Append("BT\n");

			double xPosition = bounds.Left;
			double yPosition = this.PdfDocument.PH - bounds.Top;
			for (int i=0;i<lines.Count;i++)
			{
				xPosition = bounds.Left;
				TextLine textLine = lines[i] as TextLine;
				TextSegment[] segments = (TextSegment[])textLine.Segments.ToArray(typeof(TextSegment));
				

				double height = Convert(textLine.Height);

				if (i==0)
					yPosition -= height*0.72;
				else
					yPosition -= height;

				double lineWidth = 0;

				for(int j=0;j<segments.Length;j++)
				{					
					lineWidth += (float)segments[j].Width;
				}
				lineWidth = Convert(lineWidth);

				if (textLine.Alignment == TextField.TextAlignmentType.Justified)
				{
					this.ProcessJustified(sb, textLine, yPosition);
				}
				else
				{

					if (textLine.Alignment == TextField.TextAlignmentType.Center)
					{
						xPosition = bounds.Left + (bounds.Width - lineWidth) /2;
					}
					else if (textLine.Alignment == TextField.TextAlignmentType.Right)
					{
						xPosition = bounds.Right - lineWidth;
					}
					else
					{
						xPosition = bounds.Left;
					}
				

					for(int j=0;j<segments.Length;j++)
					{
						TextSegment segment = segments[j];

						Font font = new Font(segment.Font.FontFamily, segment.Font.Size*0.96f, segment.Font.Style);	
						PdfFont pdfFont = this.PdfDocument.AddFont(font);
						//if (pdfFont.PdfFontType == PdfFont.FontType.TrueType)
						//{
						//	((PdfTrueTypeFont)pdfFont).CalculateMetrics(segment.Text);
						//}

						double width = pdfFont is PdfTrueTypeFont ? ((PdfTrueTypeFont)pdfFont).MeasureText(segment.Text, font.Size) : Utility.Measure(font, segment.Text);

						string fontSizeString = String.Format(ci.NumberFormat, "{0}", font.Size);

					
						sb.Append(String.Format("/{0} {1} Tf\n", pdfFont.Alias, fontSizeString));
						sb.Append(Utility.ColorrgLine(segment.Color));
						sb.Append("1 0 0 1 " + Utility.FormatDecimal(xPosition) + " " + Utility.FormatDecimal(yPosition ) + " Tm");
						sb.Append("(");

						byte[] textBytes = CreateTextString(segment.Text, pdfFont);
						if (pdfFont.PdfFontType == PdfFont.FontType.TrueType)
							Utility.EscapeString(textBytes, sb);
						else
							Utility.EscapeStringWeak(textBytes, sb);
						

					

						sb.Append(") Tj\n");

						xPosition += Convert(segment.Width);
					}
				}

				
			}

			sb.Append("ET\n");

			return sb.ToByteArray();
		}

		private void ProcessJustified(ByteBuffer sb, TextLine textLine, double yPosition)
		{
			TextSegment[] segments = (TextSegment[])textLine.Segments.ToArray(typeof(TextSegment));

			ArrayList words = new ArrayList();
			double wordsWidth = 0;
			for(int i=0;i<segments.Length;i++)
			{					
				if (segments[i].Text != " ")
				{
					words.Add(segments[i]);
					wordsWidth += Convert(segments[i].Width);
				}

			}
	
			float theOffset = 0;
			if (words.Count > 1)
				theOffset = (float)(bounds.Width - wordsWidth) / (words.Count - 1);


			double xPosition = bounds.Left;
			for (int i=0;i<words.Count;i++)
			{
				TextSegment segment = words[i] as TextSegment;

				Font font = new Font(segment.Font.FontFamily, segment.Font.Size*0.96f, segment.Font.Style);	
				PdfFont pdfFont = this.PdfDocument.AddFont(font);
				if (pdfFont.PdfFontType == PdfFont.FontType.TrueType)
				{
					((PdfTrueTypeFont)pdfFont).CalculateMetrics(segment.Text);
				}

				double width = pdfFont is PdfTrueTypeFont ? ((PdfTrueTypeFont)pdfFont).MeasureText(segment.Text, font.Size) : Utility.Measure(font, segment.Text);

				string fontSizeString = String.Format(ci.NumberFormat, "{0}", font.Size);

					
				sb.Append(String.Format("/{0} {1} Tf\n", pdfFont.Alias, fontSizeString));
				sb.Append(Utility.ColorrgLine(segment.Color));
				sb.Append("1 0 0 1 " + Utility.FormatDecimal(xPosition) + " " + Utility.FormatDecimal(yPosition ) + " Tm");
				sb.Append("(");

				byte[] textBytes = CreateTextString(segment.Text, pdfFont);
				if (pdfFont.PdfFontType == PdfFont.FontType.TrueType)
					Utility.EscapeString(textBytes, sb);
				else
					Utility.EscapeStringWeak(textBytes, sb);
				

					

				sb.Append(") Tj\n");

				xPosition += Convert(segment.Width) + theOffset;
			}
		}

		private static byte[] CreateTextString(string text, PdfFont pdfFont)
		{
			if (pdfFont.PdfFontType == PdfFont.FontType.TrueType)
			{
				PdfTrueTypeFont ttf = pdfFont as PdfTrueTypeFont;
				
				int i = 0;
				char[] glyph = new char[text.Length];

				for (int k = 0; k < text.Length; ++k) 
				{
					int[] metrics = ttf.GetMetricsTT(text[k]);

					if (metrics!=null)
					{
						glyph[i] = (char)metrics[0];
						i++;
					}
				}

				return System.Text.Encoding.BigEndianUnicode.GetBytes(glyph);
			}
			else
			{
				string alter = Utility.TextEncode(text);
				return System.Text.Encoding.ASCII.GetBytes(alter);
			}
		}


		internal override int StreamWrite(System.IO.Stream stream)
		{
			byte[] data = ToStream();
			//byte[] compressedData = Utility.Deflate(data);


			PdfWriter wr = new PdfWriter(this.id);


			wr.AddStreamContent(data);
			//wr.AddHeader("/Filter/FlateDecode");
			return wr.Write(stream);
		}

		private double Convert(double size)
		{
			return size*0.72;
		}

	}
}
