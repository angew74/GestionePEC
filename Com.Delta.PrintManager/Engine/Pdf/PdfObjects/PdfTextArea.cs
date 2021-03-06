
using System;
using System.Collections;
using System.Drawing;
using System.Text;

using Com.Delta.PrintManager.Engine.Pdf.Fonts;

namespace Com.Delta.PrintManager.Engine.Pdf
{

	public enum HorizontalAlignment {Left=0, Center, Right, Justified};
	public enum VerticalAlignment {Top=0, Middle, Bottom};

	/// <summary>
	/// an area to be use for Text writing
	/// </summary>
	public class PdfTextArea : PdfObject
	{
		internal ArrayList tl;
		internal string text;
		internal Font font;
		internal Color foregroundColor = Color.Black;
		internal HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left;
		internal VerticalAlignment verticalAlignment = VerticalAlignment.Top;
		internal double lineHeight;
		internal PdfFont pdfFont;
		private bool vertical = false;
		internal bool Underline = false;
		internal ArrayList UnderlineData = new ArrayList();
		private ArrayList lastLines = new ArrayList();
		internal PdfArea textArea;
		private double fontRatio = 1;

		internal ByteBuffer byteBuffer = new ByteBuffer();



		internal PdfDocument PdfDocument
		{
			get{return this.PdfArea.PdfDocument;}
		}

		
		public PdfArea PdfArea
		{
			get {return this.textArea;}
		}
		
		internal int maxlines
		{
			get
			{
				if (vertical)
					return (int)(this.textArea.Width/this.lineHeight);
				else
					return (int)(this.textArea.height/this.lineHeight);
			}
		}
		
		
		
		internal int DrawnLines
		{
			get
			{
				int l=this.RenderLines().Count;
				if (l>this.maxlines) return maxlines;
				return l;
			}

		}
		/// <summary>
		/// returns the Text that can't fit inside the estabilished area
		/// </summary>
		public string OverFlowText
		{
			get
			{
				ArrayList lines=this.RenderLines();
				string s="";
				for (int index=this.maxlines;index<lines.Count;index++)
				{
					s+=lines[index];
				}
				return s;
			}
		}
		

		public PdfTextArea(System.Drawing.Font Font,Color Color,PdfArea TextArea, HorizontalAlignment horAlignment, VerticalAlignment verAlignment, string Text):this(Font,Color,TextArea, horAlignment, verAlignment, Text, false)
		{
		}

		/// <summary>
		/// creates a new PdfTextArea
		/// </summary>
		/// <param name="Font">the font that will be used</param>
		/// <param name="Color">the color of the font that will be used</param>
		/// <param name="TextArea">the estabilished area for the Text</param>
		/// <param name="PdfTextAlign">the ContentAlignment for the Text inside the area</param>
		/// <param name="Text">the text that will be written inside the area</param>
		public PdfTextArea(System.Drawing.Font Font,Color Color, PdfArea TextArea, HorizontalAlignment horAlignment, VerticalAlignment verAlignment, string Text, bool vertical)
		{
			if (Text==null) throw new Exception("Text cannot be null.");
			// setting fonts
			this.font = new Font(Font.FontFamily, Font.Size*0.96f, Font.Style);			

			this.foregroundColor = Color;
			this.textArea = TextArea;
			this.text=Text;
			this.horizontalAlignment = horAlignment;
			this.verticalAlignment = verAlignment;

			//this.lineHeight = (double)(Font.Size);
			this.lineHeight = (double)(this.font.Size);			
								
			this.vertical = vertical;
			this.pdfFont = this.PdfDocument.AddFont(this.font);

			if (this.pdfFont.PdfFontType == PdfFont.FontType.TrueType)
			{
				((PdfTrueTypeFont)pdfFont).CalculateMetrics(this.text);
			}

		}



		internal ArrayList RenderLines()
		{
			if (tl==null)
			{
				double areaWidth = this.textArea.width;
				if (vertical)
					areaWidth = this.textArea.height;

				tl = new ArrayList();
				
				string line = "";
				string oldline = "";
				double lineWidth=0, oldlineWidth=0;
				
				char[] textchars = text.Replace("\t","     ").ToCharArray();

				ArrayList words = new ArrayList();

				string aWord="";
				for (int index=0;index<textchars.Length;index++)
				{
					char c = textchars[index];

					switch (c)
					{
						case ' ':
						{
							if (aWord!="") 
								words.Add(aWord); 
							words.Add(" "); 
							aWord=""; 
							break;
						}

						case '\n': 
							words.Add(aWord); 
							words.Add("\n"); 
							aWord=""; 
							break;

						default: 
							aWord+=c; 
							break;
					}
				}

				if (aWord!="") 
					words.Add(aWord);

				for (int i=0;i<words.Count;i++)
				{
					string s = (string)words[i];
					oldline=line;
					line += s;

					double wordWidth = this.pdfFont is PdfTrueTypeFont ? ((PdfTrueTypeFont)pdfFont).MeasureText(s, font.Size) : Utility.Measure(font,s);

					oldlineWidth = lineWidth;
					lineWidth += wordWidth;
					
					if (s=="\n")
					{
						tl.Add(oldline);
						line="";
						lineWidth=0;
					}
					else
					{
						if (lineWidth > areaWidth)
						{
							if (oldline == string.Empty)
							{
								SplitWord(s, areaWidth, tl);
								line = string.Empty;
							}

							if (oldline!=" " && oldline!="")
							{
								tl.Add(oldline);
								if (s==" ")
								{ 
									line = string.Empty; 
									lineWidth = 0;
								}
								else
								{
									line = s; 
									lineWidth = wordWidth;
								}
							} 
						}

						if (i == words.Count-1)
						{
							if (line!=" " && line!="")
							{
								tl.Add(line);
							}
						}
										
					}
					
				}
			}

			return this.tl;
		}


		private void SplitWord(string word, double areaWidth, ArrayList lines)
		{
			if (word == null || word == string.Empty || areaWidth <= 0)
				return;


			string residual = word;
			
			bool hasMore = true;

			while(hasMore)
			{
				bool hasBreak = false;
				string buffer = String.Empty;
				for (int i=0;i<residual.Length;i++)
				{
					buffer += residual.Substring(i, 1);

					double wordWidth = this.pdfFont is PdfTrueTypeFont ? ((PdfTrueTypeFont)pdfFont).MeasureText(buffer, font.Size) : Utility.Measure(font, buffer);
					if (wordWidth > areaWidth)
					{
						if (i > 0)
						{
							int index = i ;
							string line = buffer.Substring(0, index);

							lines.Add(line);
							residual = residual.Substring(index);
						
							hasBreak = true;

							break;
						}
						else
						{
							// not a single letter can fit the area -> quit operation
							residual = string.Empty;
							break;
						}
					}
				}

				if (hasBreak)
				{
					hasMore = true;
				}
				else
				{
					hasMore = false;
					if (residual.Length > 0)
						lines.Add(residual);
				}
			}

		}


		internal byte[] ToByteStream()
		{
			UnderlineData.Clear();
			ByteBuffer sb = new ByteBuffer();

			double posx = 0;
			double posy = 0;
			ArrayList al = this.RenderLines();

			int l=al.Count;
			int ml=this.maxlines;
			for(int index=0;((index<l)&&(index<ml));index++)
			{
				
				string line=(al[index] as string);

				if (vertical)
				{
					posx = textArea.posx + textArea.Width;
					posy = textArea.posy;

					double xdiff=0,ydiff=0;
					if (horizontalAlignment!=HorizontalAlignment.Left)
					{
						double lineWidth = 0;
						if (this.pdfFont is PdfTrueTypeFont)
							lineWidth = ((PdfTrueTypeFont)pdfFont).MeasureText(line, font.Size);
						else
							lineWidth = Utility.Measure(font, line);

						ydiff = textArea.height - lineWidth;
						if (this.horizontalAlignment==HorizontalAlignment.Center)
							ydiff=ydiff/2;
					}

					if (this.verticalAlignment!=VerticalAlignment.Top)
					{

						xdiff = textArea.Width - (this.DrawnLines)*this.lineHeight;
						if (this.verticalAlignment == VerticalAlignment.Middle)
							xdiff = xdiff/2;				

					}

					posx -= xdiff;
					posx = posx - this.lineHeight*(index) - this.font.Size;
					posy = this.PdfDocument.PH - (textArea.posy + ydiff);
				}
				else
				{
					posx = textArea.posx;
					posy = textArea.posy;

					double xdiff=0,ydiff=0;

					if (horizontalAlignment!=HorizontalAlignment.Left)
					{
						double lineWidth = 0;
						if (this.pdfFont is PdfTrueTypeFont)
							lineWidth = ((PdfTrueTypeFont)pdfFont).MeasureText(line, font.Size);
						else
							lineWidth = Utility.Measure(font, line);

						xdiff = textArea.width - lineWidth - 1;
						if (this.horizontalAlignment==HorizontalAlignment.Center)
							xdiff=xdiff/2;
					}

					if (this.verticalAlignment!=VerticalAlignment.Top)
					{																		
						ydiff = textArea.Height - this.DrawnLines*this.lineHeight + 1;												
						if (this.verticalAlignment==VerticalAlignment.Middle)
							ydiff = ydiff/2;						
					}
					
					posy = this.PdfDocument.PH - posy - ydiff - this.lineHeight*(index) - lineHeight*0.72;
					posx = textArea.posx + xdiff;
					
				}
				
				if (this.horizontalAlignment==HorizontalAlignment.Justified)
				{
					if (lastLines!=null && index<lastLines.Count)
						ProcessJustified(sb, line, vertical, (vertical? posx : posy), (bool)lastLines[index]);
					else
						ProcessJustified(sb, line, vertical, (vertical? posx : posy), false);
				}
				else
				{

					if (vertical)
						sb.Append("0 -1 1 0 ");
					else
						sb.Append("1 0 0 1 ");



					sb.Append(Utility.FormatDecimal(posx) + " " + Utility.FormatDecimal(posy) + " Tm(");

					byte[] textBytes = this.CreateTextString(line);

					if (this.pdfFont.PdfFontType == PdfFont.FontType.TrueType)
						Utility.EscapeString(textBytes, sb);
					else
						Utility.EscapeStringWeak(textBytes, sb);

					sb.Append(") Tj\n");

					if (this.Underline)
					{
						double lineWidth = this.pdfFont is PdfTrueTypeFont ? ((PdfTrueTypeFont)pdfFont).MeasureText(line, font.Size) : Utility.Measure(font,line);
						double lineStroke = lineHeight / 16;
						

						float startX = (float)posx;
						float startY = (float)(this.PdfDocument.PH - posy + 1.5*lineStroke);

						float endX = startX + (float)lineWidth;
						float endY = startY;

						if (vertical)
						{
							startX = (float)(posx - 1.5*lineStroke) ;
							startY = (float)(this.PdfDocument.PH - posy );

							endX = startX ;
							endY = startY + (float)lineWidth;	
						}
						
						PdfLine pdfLine = new PdfLine(this.PdfDocument, new PointF(startX, startY), new PointF(endX, endY), this.foregroundColor, lineStroke);
						string lineText = pdfLine.ToLineStreamWithColorAndWidth();
						UnderlineData.Add(lineText);
					}

				}

			}
			//Console.WriteLine(sb.ToString());
			return sb.ToByteArray();
		}


		private void ProcessJustified(ByteBuffer sb, string line, bool vertical, double position, bool isLast)
		{
			
			string[] theWords = line.Split(new char[]{' '});
			double[] wordsWidths = new double[theWords.Length];

			double totalWordsWidth = 0;
			for (int i=0;i<theWords.Length;i++)
			{
				double wordWidth = Utility.Measure(font, theWords[i]);

				if (this.pdfFont is PdfTrueTypeFont)
					wordWidth = ((PdfTrueTypeFont)pdfFont).MeasureText( theWords[i], font.Size);

				wordsWidths[i] = wordWidth;
				totalWordsWidth += wordWidth;
			}


			if (vertical)
			{
				double theOffset = 0;
				if (theWords.Length>1)
					theOffset = (textArea.height - totalWordsWidth) / (theWords.Length-1);

				double yPos = this.PdfDocument.PH - textArea.posy;


				if (isLast)
				{
					sb.Append("0 -1 1 0 ");			

					sb.Append(Utility.FormatDecimal(position) + " " + Utility.FormatDecimal(yPos) + " Tm(");

					byte[] textBytes = this.CreateTextString(line);
					if (this.pdfFont.PdfFontType == PdfFont.FontType.TrueType)
						Utility.EscapeString(textBytes, sb);
					else
						Utility.EscapeStringWeak(textBytes, sb);
					

					sb.Append(") Tj\n");

					if (this.Underline)
					{
						double lineWidth = Utility.Measure(font,line);

						if (this.pdfFont is PdfTrueTypeFont)
							lineWidth = ((PdfTrueTypeFont)pdfFont).MeasureText(line, font.Size);

						double lineStroke = lineHeight / 16;
						

						float startX = (float)(position - 1.5*lineStroke) ;
						float startY = (float)(textArea.posy + 0.01);
						float endX = startX;
						float endY = startY + (float)lineWidth;
						
						PdfLine pdfLine = new PdfLine(this.PdfDocument, new PointF(startX, startY), new PointF(endX, endY), this.foregroundColor, lineStroke);
						string lineText = pdfLine.ToLineStreamWithColorAndWidth();
						UnderlineData.Add(lineText);
					}
				}
				else
				{
					for (int i=0;i<theWords.Length;i++)
					{

						if (i==0)
						{
							yPos = this.PdfDocument.PH - textArea.posy;
						}
						else if(i==theWords.Length-1)
						{
							yPos = this.PdfDocument.PH - textArea.posy - textArea.height + wordsWidths[i];
						}					
				
						sb.Append("0 -1 1 0 ");				
						sb.Append(Utility.FormatDecimal(position) + " " + Utility.FormatDecimal(yPos) + " Tm(");


						byte[] textBytes = this.CreateTextString(theWords[i]);
						if (this.pdfFont.PdfFontType == PdfFont.FontType.TrueType)
							Utility.EscapeString(textBytes, sb);
						else
							Utility.EscapeStringWeak(textBytes, sb);						

						sb.Append(") Tj\n");
	

					
						yPos -= wordsWidths[i] + theOffset;
					}

					if (this.Underline)
					{
						double lineStroke = lineHeight / 16;
						
						float startX = (float)(position - 1.5*lineStroke) ;
						float startY = (float)(textArea.posy + 0.01);

						float endX = startX ;
						float endY = (float)(textArea.posy + textArea.height - 0.01);	
						
						PdfLine pdfLine = new PdfLine(this.PdfDocument, new PointF(startX, startY), new PointF(endX, endY), this.foregroundColor, lineStroke);
						string lineText = pdfLine.ToLineStreamWithColorAndWidth();
						UnderlineData.Add(lineText);
					}
				}
			}
			else
			{
				
				double theOffset = 0;
				if (theWords.Length>1)
					theOffset = (textArea.width - totalWordsWidth) / (theWords.Length-1);

				double xPos = textArea.posx;


				if (isLast)
				{
					sb.Append("1 0 0 1 ");				

					sb.Append((textArea.posx+0.01).ToString("0.##").Replace(",","."));
					sb.Append(" ");				
					sb.Append((position).ToString("0.##").Replace(",","."));
					sb.Append(" Tm (");

					byte[] textBytes = this.CreateTextString(line);
					if (this.pdfFont.PdfFontType == PdfFont.FontType.TrueType)
						Utility.EscapeString(textBytes, sb);
					else
						Utility.EscapeStringWeak(textBytes, sb);
					

					sb.Append(") Tj\n");

					if (this.Underline)
					{
						double lineWidth = Utility.Measure(font,line);

						if (this.pdfFont is PdfTrueTypeFont)
							lineWidth = ((PdfTrueTypeFont)pdfFont).MeasureText(line, font.Size);

						double lineStroke = lineHeight / 16;
						

						float startX = (float)(textArea.posx+0.01);
						float startY = (float)(this.PdfDocument.PH - position + 1.5*lineStroke);
						float endX = startX + (float)lineWidth;
						float endY = startY;
						
						PdfLine pdfLine = new PdfLine(this.PdfDocument, new PointF(startX, startY), new PointF(endX, endY), this.foregroundColor, lineStroke);
						string lineText = pdfLine.ToLineStreamWithColorAndWidth();
						UnderlineData.Add(lineText);
					}
				}
				else
				{
					for (int i=0;i<theWords.Length;i++)
					{

						if (i==0)
						{
							xPos = textArea.posx;
						}
						else if(i==theWords.Length-1)
						{
							xPos = textArea.posx + textArea.width - wordsWidths[i];
						}					
				
						sb.Append("1 0 0 1 ");				
						sb.Append(Utility.FormatDecimal(xPos) + " " + Utility.FormatDecimal(position) + " Tm(");


						byte[] textBytes = this.CreateTextString(theWords[i]);
						if (this.pdfFont.PdfFontType == PdfFont.FontType.TrueType)
							Utility.EscapeString(textBytes, sb);
						else
							Utility.EscapeStringWeak(textBytes, sb);
						

						sb.Append(") Tj\n");
	
						
					
						xPos += wordsWidths[i] + theOffset;
					}

					if (this.Underline)
					{
						double lineStroke = lineHeight / 16;
						
						float startX = (float)(textArea.posx + 0.01);
						float startY = (float)(this.PdfDocument.PH - position + 1.5*lineStroke);
						float endX = (float)(textArea.posx + textArea.width - 0.01);
						float endY = startY;
						
						PdfLine pdfLine = new PdfLine(this.PdfDocument, new PointF(startX, startY), new PointF(endX, endY), this.foregroundColor, lineStroke);
						string lineText = pdfLine.ToLineStreamWithColorAndWidth();
						UnderlineData.Add(lineText);
					}
				}

			}
		}



		
		internal override int StreamWrite(System.IO.Stream stream)
		{
			Byte[] part2;
			
			byteBuffer = new ByteBuffer();

			System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
			string fontSizeString = String.Format(ci.NumberFormat, "{0}", this.font.Size);

			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("BT\n");
			sb.Append(String.Format("/{0} {1} Tf\n", pdfFont.Alias, fontSizeString));
			sb.Append(Utility.ColorrgLine(this.foregroundColor));

			byteBuffer.Append(sb.ToString());

			byteBuffer.Append(this.ToByteStream());
			byteBuffer.Append("ET\n");

			

			for (int i=0;i<UnderlineData.Count;i++)
			{
				byteBuffer.Append(UnderlineData[i].ToString());
			}

			//if (PdfDocument.FlateCompression) 
			//	part2=Utility.Deflate(byteBuffer.ToByteArray());
			//else
				part2 = byteBuffer.ToByteArray();

			//Console.WriteLine(byteBuffer.ToString());
			

			string s1="";
			s1+=this.id.ToString()+" 0 obj\n";
			s1+="<</Length "+part2.Length;
			//if (PdfDocument.FlateCompression) s1+=" /Filter/FlateDecode";
			s1+=">>stream\n";
			
			string s3 = "\nendstream\nendobj\n";
				
			Byte[] part1 = System.Text.ASCIIEncoding.ASCII.GetBytes(s1);
			Byte[] part3 = System.Text.ASCIIEncoding.ASCII.GetBytes(s3);
				
				
			stream.Write(part1, 0, part1.Length);
			stream.Write(part2, 0, part2.Length);
			stream.Write(part3, 0, part3.Length);

			return part1.Length+part2.Length+part3.Length;
		}

		public void SetFontRatio(double ratio)
		{
			fontRatio = ratio;
		}

		public void SetLineSpacing(double spacing)
		{						
			//this.lineHeight = (font.Height + spacing)*0.72;
			//this.lineHeight = font.Size*96/72 + spacing*0.72;						

			if (this.RenderLines().Count>1)									
				this.lineHeight = font.Height*fontRatio*0.72 + spacing*0.72;						
			else
				this.lineHeight = font.Height*fontRatio*0.72;

		}

		public void SetJustification(bool[] justificationArray)
		{
			lastLines = new ArrayList(justificationArray);
		}


		private byte[] CreateTextString(string text)
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



		

	}
}
