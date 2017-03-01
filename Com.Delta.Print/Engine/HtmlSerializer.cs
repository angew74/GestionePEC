using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Globalization;
using System.Runtime.InteropServices;


namespace Com.Delta.Print.Engine
{
	/// <summary>
	/// Summary description for HtmlSerializer.
	/// </summary>
	internal class HtmlSerializer
	{
		private Hashtable images = new Hashtable();
		private int counter = 1;
		private string externalQuery = string.Empty;


		public HtmlSerializer()
		{

		}

		public Hashtable Images
		{
			get {return images;}
		}

        internal byte[] SerializeToHTML(ReportDocument document)
        {
            string text = Serialize(document, true);
            byte[] bytes = new byte[text.Length * sizeof(char)];
            System.Buffer.BlockCopy(text.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        internal string SerializeToHTMLString(ReportDocument document)
        {
            string text = Serialize(document, true);
            return text;

        }

		internal void Serialize(ReportDocument document, string folderName)
		{
			DirectoryInfo di = Directory.CreateDirectory(folderName);
			string text = Serialize(document, true);

			string filename = di.FullName + Path.DirectorySeparatorChar + "index.html";
			StreamWriter sw = new StreamWriter(filename, false, System.Text.Encoding.UTF8);
			sw.Write(text);
			sw.Close();

			foreach(string key in this.images.Keys)
			{
				byte[] content = images[key] as byte[];
				string filepath = di.FullName + Path.DirectorySeparatorChar + key + ".jpg";
				FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.Write);
				fs.Write(content, 0, content.Length);
				fs.Close();
			}

		}

		internal string Serialize(ReportDocument document, bool hardcodeLink)
		{
			StringBuilder buffer = new StringBuilder();

			Size documentSize = new Size(200,200);
			if (document.PaperType == Paper.Type.Custom)
				documentSize = document.PixelSize;
			else
				documentSize = Paper.GetPaperSize(document.PaperType);

			MemoryStream ms = new MemoryStream();

			StringWriter sw = new StringWriter(buffer);
			sw.WriteLine("<html>");
			sw.WriteLine("<head>");
			sw.WriteLine("</head>");
			
			
			int pageNum = 1;
			int pageGap = 10;
			int pageOffset = 10;
			int pageHeight = document.Layout == Com.Delta.Print.Engine.ReportDocument.LayoutType.Portrait ? documentSize.Height : documentSize.Width;

			sw.WriteLine("<body bgcolor=" + FormatColor(SystemColors.AppWorkspace) + " style='margin-top:10'>");

			string style = "";
			if (document.Layout == Com.Delta.Print.Engine.ReportDocument.LayoutType.Portrait)
			{
				style += Format("width", String.Format("{0}px", documentSize.Width)); 
				style += Format("height", String.Format("{0}px", documentSize.Height));
				
			}
			else
			{
				style += Format("width", String.Format("{0}px", documentSize.Height)); 
				style += Format("height", String.Format("{0}px", documentSize.Width));
			}

			style += Format("border", "1px solid black");
			style += Format("background-color", "white");
			style += Format("margin-bottom", String.Format("{0}px", pageGap));

			
			document.StartPrinting();
			for (int sectionCounter=0;sectionCounter<document.Sections.Count;sectionCounter++)
			{
				document.NewPage();
				Section section = (Section)document.Sections[sectionCounter];
				section.Prepare(true);
				sw.WriteLine("<a name=page1>");
				sw.WriteLine("<div name=page " + FormatStyle(style) + ">");
				
				bool morePages = false;
				int sectionPage = 0;
				do
				{
					sectionPage++;
					morePages = section.UpdateDynamicContent();
					for (int i=0;i<section.Objects.Length;i++)
					{
						
						if (section.Objects[i].Layout == ICustomPaint.LayoutTypes.EveryPage)
						{
							ProcessElement(sw, section.Objects[i], i, pageOffset, hardcodeLink);
						}
						else if (section.Objects[i].Layout == ICustomPaint.LayoutTypes.FirstPage)
						{
							if (sectionPage == 1)
							{
								ProcessElement(sw, section.Objects[i], i, pageOffset, hardcodeLink);
							}
						}
						else if (section.Objects[i].Layout == ICustomPaint.LayoutTypes.LastPage)
						{
							if (!morePages)
							{
								ProcessElement(sw, section.Objects[i], i, pageOffset, hardcodeLink);
							}
						}
				
					}

					if (morePages)
					{
						document.NewPage();
						section.Prepare(false);
						pageNum++;
						sw.WriteLine("</div>");
						sw.WriteLine("<a name=page" + pageNum.ToString()+">");
						sw.WriteLine("<div name=page " + FormatStyle(style) + ">");
						pageOffset += pageHeight + pageGap;
					}

				}
				while(morePages);

				document.NewSection();
				sw.WriteLine("</div>");
				pageOffset += pageHeight + pageGap;
			}

			sw.WriteLine("</body>");
			sw.WriteLine("</html>");

			return buffer.ToString();
			
		}


		private void ProcessElement(StringWriter writer, Com.Delta.Print.Engine.ICustomPaint element, int order, int pageOffset, bool hardcodeLink)
		{
			if (element.Anchored)
			{
				if (element.Ready && !element.Displayed)
				{													
					Process(writer, element, order, pageOffset, hardcodeLink);
					if (element.Done)
						element.Displayed = true;
								
				}
			}
			else
			{
				Process(writer, element, order, pageOffset, hardcodeLink);
			}
		}

		private void Process(StringWriter writer, Com.Delta.Print.Engine.ICustomPaint element, int order, int pageOffset, bool hardcodeLink)
		{
			if (element is TextField)
				ProcessTextField(writer, (TextField)element, order, pageOffset);
			else if (element is PictureBox)
				ProcessPictureBox(writer, (PictureBox)element, order, pageOffset, hardcodeLink);
			else if (element is Map)
				ProcessMap(writer, (Map)element, order, pageOffset, hardcodeLink);
			else if (element is ChartBox)
				ProcessChartBox(writer, (ChartBox)element, order, pageOffset, hardcodeLink);
			else if (element is Barcode)
				ProcessBarcode(writer, (Barcode)element, order, pageOffset, hardcodeLink);
			else if (element is Timeline)
				ProcessTimeline(writer, (Timeline)element, order, pageOffset, hardcodeLink);
			else if (element is UserPaint)
				ProcessUserPaint(writer, (UserPaint)element, order, pageOffset, hardcodeLink);
			else if (element is Scatter)
				ProcessScatter(writer, (Scatter)element, order, pageOffset, hardcodeLink);
			else if (element is Box)
				ProcessBox(writer, (Box)element, order, pageOffset, hardcodeLink);
			else if (element is StyledTable)
				ProcessStyledTable(writer, (StyledTable)element, order, pageOffset);
			else if (element is RichTextField)
				ProcessRichTextField(writer, (RichTextField)element, order, pageOffset);
			else if (element is Line)
				ProcessLine(writer, (Line)element, order, pageOffset);
		}

		private void ProcessTextField(StringWriter writer, Com.Delta.Print.Engine.TextField textField, int order, int pageOffset)
		{
			bool justify = false;
			string style = "";
			string cellStyle = "";
	
		//	style += Format("position", "absolute");
			style += Format("z-index", String.Format("{0}", order));
			style += Format("top", String.Format("{0}px", textField.Bounds.Y + pageOffset));
	     	style += Format("left", String.Format("{0}px", textField.Bounds.X));
			style += Format("width", String.Format("{0}px", textField.Bounds.Width));
			style += Format("height", String.Format("{0}px", textField.Bounds.Height));
			style += Format("border", String.Format("{0}px solid {1}", textField.BorderWidth, textField.BorderColor.Name));
			style += Format("color", textField.ForegroundColor.Name);			
			cellStyle += Format("padding", String.Format("{0}px", textField.Padding));			
			cellStyle += Format("overflow", "hidden");


			switch(textField.TextAlignment)
			{
				case TextField.TextAlignmentType.Center:
					style += Format("text-align", "center");
					break;
				case TextField.TextAlignmentType.Justified:
					style += Format("text-align", "justify");
					justify = true;
					break;
				case TextField.TextAlignmentType.Right:
					style += Format("text-align", "right");
					break;
				default:
					style += Format("text-align", "left");
					break;

			}

			if (textField.BackgroundColor != Color.Transparent)
				style += Format("background-color", textField.BackgroundColor.Name);

			style += Format("font-family", String.Format("{0}", textField.Font.FontFamily.Name));
			style += Format("font-size", String.Format("{0}pt", textField.Font.SizeInPoints));
			style += Format("font-weight", textField.Font.Bold ? "bold" : "regular");
			if (textField.Font.Underline)
				style += Format("text-decoration", "underline");

			//style += Format("line-height", String.Format("{0}px", textField.Spacing + textField.Font.Height));
			cellStyle += Format("line-height", String.Format("{0}px", textField.Spacing + textField.Font.Height));
			

			writer.WriteLine("<table " + FormatStyle(style) + "><tr><td " + this.FormatVerticalAlignment(textField.TextVerticalAlignment) +  " " + FormatStyle(cellStyle) + ">");


			if (textField.TextAlignment == TextField.TextAlignmentType.Justified)
			{
				writer.WriteLine(textField.CurrentText);
			}
			else
			{
				writer.WriteLine(FormatText(textField.CurrentText));
			}

			writer.WriteLine("</table>");
			writer.WriteLine("");
		}


		private void ProcessRichTextField(StringWriter writer, Com.Delta.Print.Engine.RichTextField richTextField, int order, int pageOffset)
		{
			bool justify = false;
			string style = "";
			string cellStyle = "";
	
		    style += Format("position", "absolute");
			style += Format("z-index", String.Format("{0}", order));
			style += Format("top", String.Format("{0}px", richTextField.Bounds.Y + pageOffset));
			style += Format("left", String.Format("{0}px", richTextField.Bounds.X));
			style += Format("width", String.Format("{0}px", richTextField.Bounds.Width));
			style += Format("height", String.Format("{0}px", richTextField.Bounds.Height));
			style += Format("border", String.Format("{0}px solid {1}", richTextField.BorderWidth, richTextField.BorderColor.Name));
			style += Format("color", richTextField.ForegroundColor.Name);			
			cellStyle += Format("padding", String.Format("{0}px", richTextField.Padding));			
			cellStyle += Format("overflow", "hidden");



			if (richTextField.BackgroundColor != Color.Transparent)
				style += Format("background-color", FormatColor(richTextField.BackgroundColor));

			style += Format("font-family", String.Format("{0}", richTextField.Font.FontFamily.Name));
			style += Format("font-size", String.Format("{0}pt", richTextField.Font.SizeInPoints));
			style += Format("font-weight", richTextField.Font.Bold ? "bold" : "regular");
			if (richTextField.Font.Underline)
				style += Format("text-decoration", "underline");

						

			writer.WriteLine("<table " + FormatStyle(style) + "><tr><td valign=top " + FormatStyle(cellStyle) + ">");


			foreach(TextLine line in richTextField.lines)
			{
				
				string lineStyle = "";
				switch(line.Alignment)
				{
					case TextField.TextAlignmentType.Center:
						lineStyle += Format("text-align", "center");
						break;
					case TextField.TextAlignmentType.Justified:
						lineStyle += Format("text-align", "justify");
						break;
					case TextField.TextAlignmentType.Right:
						lineStyle += Format("text-align", "right");
						break;
					default:
						lineStyle += Format("text-align", "left");
						break;

				}

				writer.WriteLine("<div " + FormatStyle(lineStyle) + ">");

				foreach(TextSegment segment in line.Segments)
				{
				
					string segmentStyle = String.Empty;

					segmentStyle += Format("color", FormatColor(segment.Color));
					segmentStyle += Format("font-family", String.Format("{0}", segment.Font.FontFamily.Name));
					segmentStyle += Format("font-size", String.Format("{0}pt", segment.Font.SizeInPoints));
					segmentStyle += Format("font-weight", segment.Font.Bold ? "bold" : "regular");
					if (richTextField.Font.Underline)
						segmentStyle += Format("text-decoration", "underline");

					writer.WriteLine("<font " + FormatStyle(segmentStyle) + ">"+ segment.Text + "</font>");
				}

				writer.WriteLine("</div>");
			}



			writer.WriteLine("</td></tr></table>");
			writer.WriteLine("");
		}

		private void ProcessStyledTable(StringWriter writer, Com.Delta.Print.Engine.StyledTable table, int order, int pageOffset)
		{
			string style = "";
	
			style += Format("position", "absolute");
			style += Format("z-index", String.Format("{0}", order));
			style += Format("top", String.Format("{0}px", table.Bounds.Y + pageOffset));
			style += Format("left", String.Format("{0}px", table.Bounds.X));
			style += Format("width", String.Format("{0}px", table.Bounds.Width));
			style += Format("border-collapse", "collapse");
			style += Format("background-color", FormatColor(table.BackgroundColor));

			style += Format("font-family", String.Format("{0}", table.DataFont.FontFamily.Name));
			style += Format("font-size", String.Format("{0}pt", table.DataFont.SizeInPoints));
			style += Format("font-weight", table.DataFont.Bold ? "bold" : "regular");
			if (table.DataFont.Underline)
				style += Format("text-decoration", "underline");

			DataTable data = table.DataSource == null ? table.DisplayData : table.Data;
			if (data==null)
			{
				if (table.DrawHeader)
				{
					writer.WriteLine("<table " + FormatStyle(style) +">");
					writer.WriteLine("<tr>");
					for (int i=0;i<table.Columns.Length;i++)
					{
						string headerStyle = "";
						headerStyle += Format("height", String.Format("{0}px", table.CellHeight));
						headerStyle += Format("font-family", String.Format("{0}", table.HeaderFont.FontFamily.Name));
						headerStyle += Format("font-size", String.Format("{0}pt", table.HeaderFont.SizeInPoints));
						headerStyle += Format("font-weight", table.HeaderFont.Bold ? "bold" : "regular");
						headerStyle += Format("border", String.Format("1px solid {0}", FormatColor(table.BorderColor)));
						headerStyle += Format("background-color", FormatColor(table.HeaderBackgroundColor));
						headerStyle += Format("color", FormatColor(table.HeaderFontColor));

						if (table.HeaderFont.Underline)
							headerStyle += Format("text-decoration", "underline");

						if (table.Columns[i].Width > 0)
						{
							headerStyle += Format("text-align", GetColumnContentAlignment(table.Columns[i].Alignment));
							writer.WriteLine("<td " + FormatStyle(headerStyle) + ">" + table.Columns[i].Label + "</td>");	
						}
					}
					writer.WriteLine("<tr>");

					if (table.DrawEmptyRows)
					{
						for (int i=(table.DrawHeader? 1 : 0);i<table.Height/table.CellHeight;i++)
						{
							Color bgColor = (table.AlternateBackColor && i%2==1) ? table.AlternatingBackColor : table.BackgroundColor;
							writer.WriteLine("<tr>");
							for (int j=0;j<table.Columns.Length;j++)
							{
								if (table.Columns[j].Width > 0)
								{
									string cellStyle = "";
									cellStyle += Format("height", String.Format("{0}px", table.CellHeight));
									cellStyle += Format("border", String.Format("1px solid {0}", FormatColor(table.BorderColor)));
									cellStyle += Format("background-color", FormatColor(bgColor));
									writer.WriteLine("<td " + FormatStyle(cellStyle) + ">" + "" + "</td>");

								}
							}
							writer.WriteLine("</tr>");
						}
					}

					writer.WriteLine("</table>");
					writer.WriteLine("");
				}
				else
				{
					if (table.DrawEmptyRows)
					{
						writer.WriteLine("<table " + FormatStyle(style) +">");
						
						for (int i=0;i<table.Height/table.CellHeight;i++)
						{
							Color bgColor = (table.AlternateBackColor && i%2==1) ? table.AlternatingBackColor : table.BackgroundColor;
							writer.WriteLine("<tr>");
							for (int j=0;j<table.Columns.Length;j++)
							{
								if (table.Columns[j].Width > 0)
								{
									string cellStyle = "";
									cellStyle += Format("height", String.Format("{0}px", table.CellHeight));
									cellStyle += Format("border", String.Format("1px solid {0}", FormatColor(table.BorderColor)));
									cellStyle += Format("background-color", FormatColor(bgColor));
									writer.WriteLine("<td " + FormatStyle(cellStyle) + "></td>");

								}
							}
							writer.WriteLine("</tr>");
						}
						
						writer.WriteLine("</table>");
						writer.WriteLine("");
					}
				}
			}
			else
			{

				writer.WriteLine("<table " + FormatStyle(style) +">");

				if (table.DrawHeader)
				{
					for (int i=0;i<table.Columns.Length;i++)
					{
						string headerStyle = "";
						headerStyle += Format("height", String.Format("{0}px", table.CellHeight));
						headerStyle += Format("font-family", String.Format("{0}", table.HeaderFont.FontFamily.Name));
						headerStyle += Format("font-size", String.Format("{0}pt", table.HeaderFont.SizeInPoints));
						headerStyle += Format("font-weight", table.HeaderFont.Bold ? "bold" : "regular");
						headerStyle += Format("border", String.Format("1px solid {0}", FormatColor(table.BorderColor)));
						headerStyle += Format("background-color", FormatColor(table.HeaderBackgroundColor));
						headerStyle += Format("color", FormatColor(table.HeaderFontColor));

						if (table.HeaderFont.Underline)
							headerStyle += Format("text-decoration", "underline");

						if (table.Columns[i].Width > 0)
						{
							headerStyle += Format("text-align", GetColumnContentAlignment(table.Columns[i].Alignment));
							writer.WriteLine("<td " + FormatStyle(headerStyle) + ">" + table.Columns[i].Label + "</td>");	
						}
					}
				}

				int rowsNum = Math.Min(table.Height/table.CellHeight, data.Rows.Count) ;

				for (int j=0;j<rowsNum;j++)
				{
					writer.WriteLine("<tr>");
					for (int i=0;i<table.Columns.Length;i++)
					{

						if (table.Columns[i].Width > 0)
						{
							string cellStyle = "";
							if (j==0)
								cellStyle += Format("width", String.Format("{0}px", table.Columns[i].Width));
						
							cellStyle += Format("height", String.Format("{0}px", table.CellHeight));
							cellStyle += Format("border", String.Format("1px solid {0}", FormatColor(table.BorderColor)));
							cellStyle += Format("text-align", GetColumnContentAlignment(table.Columns[i].Alignment));

							if (data.Rows[j].RowError == "Subtotal")
							{
								string content = table.Subtotals[i];
								cellStyle += Format("color", FormatColor(table.SubtotalsColor));
								writer.WriteLine("<td " + FormatStyle(cellStyle) + ">" + content + "</td>");
							}
							else
							{
								if (table.AlterRows!=null && table.AlterRows.Count>0)
								{
									if (table.AlterRows.Contains(data.Rows[j]))
									{
										cellStyle += Format("background-color", FormatColor(table.AlterDataBackColor));
										cellStyle += Format("color", FormatColor(table.AlterDataColor));					
									}
									else
									{
										if (table.AlternateBackColor && j%2==1)
											cellStyle += Format("background-color", FormatColor(table.AlternatingBackColor));
									}							
								}
								else
								{
									if (table.AlternateBackColor && j%2==1)
										cellStyle += Format("background-color", FormatColor(table.AlternatingBackColor));
								}

								string content = "";
								if (table.Columns[i].FormatMask == null || table.Columns[i].FormatMask == String.Empty)
									content = String.Format("{0}", data.Rows[j][i]);
								else
									content = String.Format("{0:" + table.Columns[i].FormatMask + "}", data.Rows[j][i]);

								writer.WriteLine("<td " + FormatStyle(cellStyle) + ">" + content + "</td>");
							}
						}
					}
					writer.WriteLine("</tr>");
				}

				if (table.DrawEmptyRows)
				{
					for (int i=(table.DrawHeader? data.Rows.Count+1 : data.Rows.Count);i<table.Height/table.CellHeight;i++)
					{
						Color bgColor = (table.AlternateBackColor && i%2==0) ? table.AlternatingBackColor : table.BackgroundColor;
						writer.WriteLine("<tr>");
						for (int j=0;j<table.Columns.Length;j++)
						{
							if (table.Columns[j].Width > 0)
							{
								string cellStyle = "";
								cellStyle += Format("height", String.Format("{0}px", table.CellHeight));
								cellStyle += Format("border", String.Format("1px solid {0}", FormatColor(table.BorderColor)));
								cellStyle += Format("background-color", FormatColor(bgColor));
								writer.WriteLine("<td " + FormatStyle(cellStyle) + ">" + "" + "</td>");

							}
						}
						writer.WriteLine("</tr>");
					}
				}

				writer.WriteLine("</table>");
				writer.WriteLine("");
			}

		}

		private void ProcessPictureBox(StringWriter writer, Com.Delta.Print.Engine.PictureBox pictureBox, int order, int pageOffset, bool hardcodeLink)
		{
			string style = "";
	
			style += Format("position", "absolute");
			style += Format("z-index", String.Format("{0}", order));
			style += Format("top", String.Format("{0}px", pictureBox.Bounds.Y + pageOffset));
			style += Format("left", String.Format("{0}px", pictureBox.Bounds.X));
			style += Format("width", String.Format("{0}px", pictureBox.Bounds.Width));
			style += Format("height", String.Format("{0}px", pictureBox.Bounds.Height));
			style += Format("border", String.Format("{0}px solid {1}", pictureBox.BorderWidth, pictureBox.BorderColor.Name));

			
			if (pictureBox.Image!=null)
			{
				string pictureName = "image" + GetCounter();
				FormatImage(pictureBox.Image, pictureName);
				
				if (hardcodeLink)
					writer.WriteLine("<img " + FormatStyle(style) + " src='" + pictureName + ".jpg'>");
				else
					writer.WriteLine("<img " + FormatStyle(style) + " src='?stampaImage=" + pictureName + externalQuery + "'>");
				writer.WriteLine("");
			}
		}

		private void ProcessMap(StringWriter writer, Com.Delta.Print.Engine.Map element, int order, int pageOffset, bool hardcodeLink)
		{
			string style = "";
	
			style += Format("position", "absolute");
			style += Format("z-index", String.Format("{0}", order));
			style += Format("top", String.Format("{0}px", element.Bounds.Y + pageOffset));
			style += Format("left", String.Format("{0}px", element.Bounds.X));
			style += Format("width", String.Format("{0}px", element.Bounds.Width+1));
			style += Format("height", String.Format("{0}px", element.Bounds.Height+1));

			string elementName = "image" + GetCounter();
			FormatImage(element.Image, elementName);
			
			if (hardcodeLink)
				writer.WriteLine("<img " + FormatStyle(style) + " src='" + elementName + ".jpg'>");
			else
				writer.WriteLine("<img " + FormatStyle(style) + " src='?stampaImage=" + elementName + externalQuery + "'>");

			writer.WriteLine("");
		}

		private void ProcessChartBox(StringWriter writer, Com.Delta.Print.Engine.ChartBox element, int order, int pageOffset, bool hardcodeLink)
		{
			string style = "";
	
			style += Format("position", "absolute");
			style += Format("z-index", String.Format("{0}", order));
			style += Format("top", String.Format("{0}px", element.Bounds.Y + pageOffset));
			style += Format("left", String.Format("{0}px", element.Bounds.X));
			style += Format("width", String.Format("{0}px", element.Bounds.Width+1));
			style += Format("height", String.Format("{0}px", element.Bounds.Height+1));

			string elementName = "image" + GetCounter();
			FormatImage(element.Image, elementName);
			
			if (hardcodeLink)
				writer.WriteLine("<img " + FormatStyle(style) + " src='" + elementName + ".jpg'>");
			else
				writer.WriteLine("<img " + FormatStyle(style) + " src='?stampaImage=" + elementName + externalQuery + "'>");
			writer.WriteLine("");
		}

		private void ProcessScatter(StringWriter writer, Com.Delta.Print.Engine.Scatter element, int order, int pageOffset, bool hardcodeLink)
		{
			string style = "";
	
			style += Format("position", "absolute");
			style += Format("z-index", String.Format("{0}", order));
			style += Format("top", String.Format("{0}px", element.Bounds.Y + pageOffset));
			style += Format("left", String.Format("{0}px", element.Bounds.X));
			style += Format("width", String.Format("{0}px", element.Bounds.Width+1));
			style += Format("height", String.Format("{0}px", element.Bounds.Height+1));


			string elementName = "image" + GetCounter();
			FormatImage(element.Image, elementName);
			
			if (hardcodeLink)
				writer.WriteLine("<img " + FormatStyle(style) + " src='" + elementName + ".jpg'>");
			else
				writer.WriteLine("<img " + FormatStyle(style) + " src='?stampaImage=" + elementName + externalQuery + "'>");

			writer.WriteLine("");
		}

		private void ProcessTimeline(StringWriter writer, Com.Delta.Print.Engine.Timeline element, int order, int pageOffset, bool hardcodeLink)
		{
			string style = "";
	
			style += Format("position", "absolute");
			style += Format("z-index", String.Format("{0}", order));
			style += Format("top", String.Format("{0}px", element.Bounds.Y + pageOffset));
			style += Format("left", String.Format("{0}px", element.Bounds.X));
			style += Format("width", String.Format("{0}px", element.Bounds.Width+1));
			style += Format("height", String.Format("{0}px", element.Bounds.Height+1));

			string elementName = "image" + GetCounter();
			FormatImage(element.Image, elementName);
			
			if (hardcodeLink)
				writer.WriteLine("<img " + FormatStyle(style) + " src='" + elementName + ".jpg'>");
			else
				writer.WriteLine("<img " + FormatStyle(style) + " src='?stampaImage=" + elementName + externalQuery + "'>");
			writer.WriteLine("");
		}

		private void ProcessBarcode(StringWriter writer, Com.Delta.Print.Engine.Barcode element, int order, int pageOffset, bool hardcodeLink)
		{
			string style = "";
	
			style += Format("position", "absolute");
			style += Format("z-index", String.Format("{0}", order));
			style += Format("top", String.Format("{0}px", element.Bounds.Y + pageOffset));
			style += Format("left", String.Format("{0}px", element.Bounds.X));
			style += Format("width", String.Format("{0}px", element.Bounds.Width));
			style += Format("height", String.Format("{0}px", element.Bounds.Height));			

			string elementName = "image" + GetCounter();
			FormatImage(element.Image, elementName);
			
			if (hardcodeLink)
				writer.WriteLine("<img " + FormatStyle(style) + " src='" + elementName + ".jpg'>");
			else
				writer.WriteLine("<img " + FormatStyle(style) + " src='?stampaImage=" + elementName + externalQuery + "'>");
			writer.WriteLine("");
		}

		private void ProcessUserPaint(StringWriter writer, Com.Delta.Print.Engine.UserPaint element, int order, int pageOffset, bool hardcodeLink)
		{
			string style = "";
	
			style += Format("position", "absolute");
			style += Format("z-index", String.Format("{0}", order));
			style += Format("top", String.Format("{0}px", element.Bounds.Y + pageOffset));
			style += Format("left", String.Format("{0}px", element.Bounds.X));
			style += Format("width", String.Format("{0}px", element.Bounds.Width+1));
			style += Format("height", String.Format("{0}px", element.Bounds.Height+1));				

			string elementName = "image" + GetCounter();
			FormatImage(element.Image, elementName);
			
			if (hardcodeLink)
				writer.WriteLine("<img " + FormatStyle(style) + " src='" + elementName + ".jpg'>");
			else
				writer.WriteLine("<img " + FormatStyle(style) + " src='?stampaImage=" + elementName + externalQuery + "'>");
			writer.WriteLine("");
		}

		private void ProcessBox(StringWriter writer, Com.Delta.Print.Engine.Box element, int order, int pageOffset, bool hardcodeLink)
		{
			string style = "";
	
			style += Format("position", "absolute");
			style += Format("z-index", String.Format("{0}", order));
			style += Format("top", String.Format("{0}px", element.Bounds.Y + pageOffset));
			style += Format("left", String.Format("{0}px", element.Bounds.X));
			style += Format("width", String.Format("{0}px", element.Bounds.Width));
			style += Format("height", String.Format("{0}px", element.Bounds.Height));
			
			if (element.FillStyle == Box.FillStyles.Solid)
			{
				if (element.Color != Color.Transparent)
					style += Format("background-color", FormatColor(element.Color));
				style += Format("border", String.Format("{0}px solid {1}", element.BorderWidth, FormatColor(element.BorderColor)));
				writer.WriteLine("<div " + FormatStyle(style) + "></div>");	
			}
			else
			{
				string elementName = "image" + GetCounter();
				FormatImage(element.Image, elementName);
			
				if (hardcodeLink)
					writer.WriteLine("<img " + FormatStyle(style) + " src='" + elementName + ".jpg'>");
				else
					writer.WriteLine("<img " + FormatStyle(style) + " src='?stampaImage=" + elementName + externalQuery + "'>");
			}
			writer.WriteLine("");
		}

		private void ProcessLine(StringWriter writer, Com.Delta.Print.Engine.Line element, int order, int pageOffset)
		{
			string style = "";
	
			style += Format("position", "absolute");
			style += Format("z-index", String.Format("{0}", order));
			//style += Format("top", String.Format("{0}px", element.Y + pageOffset));
			//style += Format("left", String.Format("{0}px", element.X));
			//style += Format("width", String.Format("{0}px", element.Width));
			//style += Format("height", String.Format("{0}px", element.Height));
			
			if (element.Orientation == Line.Orientations.WE)
			{
				style += Format("top", String.Format("{0}px", element.Coordinates[1].Y + pageOffset));
				style += Format("left", String.Format("{0}px", element.Bounds.X));
				style += Format("width", String.Format("{0}px", element.Bounds.Width));
				style += Format("height", String.Format("{0}px", element.LineWidth));
				style += Format("max-height", String.Format("{0}px", element.LineWidth));
				style += Format("font", "0px");
				style += Format("background-color", FormatColor(element.Color));
				writer.WriteLine("<div " + FormatStyle(style) + "></div>");
			}
			else if (element.Orientation == Line.Orientations.NS)
			{
				style += Format("top", String.Format("{0}px", element.Bounds.Y + pageOffset));
				style += Format("left", String.Format("{0}px", element.Coordinates[1].X));
				style += Format("width", String.Format("{0}px", element.LineWidth));
				style += Format("height", String.Format("{0}px", element.Bounds.Height));
				style += Format("font", "0px");
				style += Format("background-color", FormatColor(element.Color));
				writer.WriteLine("<div " + FormatStyle(style) + "></div>");
			}

		
			writer.WriteLine("");
		}

		private string Format(string name, string value)
		{
			return String.Format("{0}:{1};", name, value);
		}

		private string FormatStyle(string style)
		{
			return String.Format("style='{0}'", style);
		}

		private string FormatVerticalAlignment(Com.Delta.Print.Engine.TextField.TextVerticalAlignmentType alignment)
		{
			if (alignment==Com.Delta.Print.Engine.TextField.TextVerticalAlignmentType.Bottom)
				return "valign=bottom";
			else if (alignment==Com.Delta.Print.Engine.TextField.TextVerticalAlignmentType.Middle)
				return "valign=middle";
			else
				return "valign=top";
		}

		private void FormatImage(Bitmap image, string pictureName)
		{
			MemoryStream ImageStream = new MemoryStream();			
			image.Save(ImageStream, ImageFormat.Jpeg);

			// alternativa
			//Bitmap b = MakeTransparent(image);
			//b.Save(ImageStream, ImageFormat.Gif);

			byte[] data = ImageStream.ToArray();

			images[pictureName] = data;			
		}


		private string GetColumnContentAlignment(Com.Delta.Print.Engine.StyledTableColumn.AlignmentType alignment)
		{
			switch (alignment)
			{
				case StyledTableColumn.AlignmentType.Left : return "left";
				case StyledTableColumn.AlignmentType.Center : return "center";
				case StyledTableColumn.AlignmentType.Right : return "right";
				default : return "left";
			}
		}

		private string FormatColor(Color color)
		{
			string red = String.Format("{0:X}", color.R);
			string green = String.Format("{0:X}", color.G);
			string blue = String.Format("{0:X}", color.B);
  
			if (red.Length<2) red = "0" + red;
			if (green.Length<2) green = "0" + green;
			if (blue.Length<2) blue = "0" + blue;
			return String.Format("#{0}{1}{2}", red, green, blue).ToLower();
		}

		private string FormatText(string text)
		{

			string buffer = text.Replace("\r", "");
			string[] lines = buffer.Split('\n');
			StringBuilder sb = new StringBuilder();
			foreach(string line in lines)
				sb.Append(line + "<br>");


			return sb.ToString();
		}

		private string GetCounter()
		{
			return (counter++).ToString();
		}

		internal void SetExternalQuery(String query)
		{
			externalQuery = query;
		}

		private Bitmap MakeTransparent(Bitmap original)
		{
			Color transparentColor = Color.FromArgb(0, 204, 153);
			int transparentArgb = transparentColor.ToArgb();

			Bitmap buffer = new Bitmap(original.Width, original.Width, original.PixelFormat);
			Graphics g = Graphics.FromImage(buffer);
			g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

			g.FillRectangle(new SolidBrush(transparentColor), 0, 0, original.Width, original.Height);
			g.DrawImage(original, 0, 0);

			MemoryStream imgStream = new MemoryStream();
			buffer.Save(imgStream, ImageFormat.Gif);

			Bitmap clone = new Bitmap(imgStream);

			Bitmap newBitmap = new Bitmap(clone.Width, clone.Height, PixelFormat.Format8bppIndexed);

			ColorPalette origPalette = clone.Palette;
			ColorPalette newPalette = newBitmap.Palette;

			int index = 0;
			int transparentIndex = -1;

			foreach (Color origColor in origPalette.Entries) 
			{
				newPalette.Entries[index] = Color.FromArgb(255, origColor);                
				if (origColor.ToArgb() == transparentArgb)
					transparentIndex = index;
                
				index += 1;
			}

			if (transparentIndex == -1)
				return original;

			newPalette.Entries[transparentIndex] = Color.FromArgb(0, transparentColor);
			newBitmap.Palette = newPalette;


       
			Rectangle rect = new Rectangle(0, 0, clone.Width, clone.Height);
           
			BitmapData origBitmapData = clone.LockBits(rect, ImageLockMode.ReadOnly, clone.PixelFormat);
			BitmapData newBitmapData = newBitmap.LockBits(rect, ImageLockMode.WriteOnly, newBitmap.PixelFormat);    
     
			for (int y = 0; y < clone.Height; y++) 
			{
				for (int x = 0; x < clone.Width; x++) 
				{
					byte origBitmapByte = Marshal.ReadByte(origBitmapData.Scan0, origBitmapData.Stride * y + x);
					Marshal.WriteByte(newBitmapData.Scan0, newBitmapData.Stride * y + x, origBitmapByte);
				}    
			}
     
			newBitmap.UnlockBits(newBitmapData);
			clone.UnlockBits(origBitmapData);

			return newBitmap;
		}
	}
}
