using System;
using System.IO;
using System.Collections;
using System.Text;
using System.Drawing;
using System.Data;

using Com.Delta.Print.Engine;
using Com.Delta.Print.Engine.Pdf;

namespace Com.Delta.Print.Engine
{
	/// <summary>
	/// Summary description for PdfSerializer.
	/// </summary>
	internal class PdfSerializer
	{

		PdfDocument pdfDocument;
		Hashtable images = new Hashtable();

		public PdfSerializer() 
		{

		}
		
		public byte[] Serialize(ReportDocument document)
		{

			//long start = DateTime.Now.Ticks;
			Size documentSize = document.PaperType == Paper.Type.Custom ? document.PixelSize : Paper.GetPaperSize(document.PaperType);

			if (document.Layout == Com.Delta.Print.Engine.ReportDocument.LayoutType.Portrait)
				pdfDocument = new PdfDocument(PdfDocumentFormat.InInches(documentSize.Width*0.72/72, documentSize.Height*0.72/72));
			else
				pdfDocument = new PdfDocument(PdfDocumentFormat.InInches(documentSize.Height*0.72/72, documentSize.Width*0.72/72));
			document.StartPrinting();

			for (int sectionCounter=0;sectionCounter<document.Sections.Count;sectionCounter++)
			{
				document.NewPage();
				Section section = (Section)document.Sections[sectionCounter];
				section.Prepare(true);
				PdfPage page = pdfDocument.NewPage();
								
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
							ProcessElement(page, section.Objects[i]);
						}
						else if (section.Objects[i].Layout == ICustomPaint.LayoutTypes.FirstPage)
						{
							if (sectionPage == 1)
							{
								ProcessElement(page, section.Objects[i]);
							}
						}
						else if (section.Objects[i].Layout == ICustomPaint.LayoutTypes.LastPage)
						{
							if (!morePages)
							{
								ProcessElement(page, section.Objects[i]);
							}
						}
				
					}

					if (morePages)
					{
						page.SaveToDocument();
						page = pdfDocument.NewPage();
						document.NewPage();
						section.Prepare(false);
					}

				}
				while(morePages);

				page.SaveToDocument();
				document.NewSection();
			}

			MemoryStream ms = new MemoryStream();

			pdfDocument.SaveToStream(ms);

			
			byte[] content = new byte[ms.Length];
			//Console.WriteLine((DateTime.Now.Ticks-start)/10000);
			ms.Position = 0;
			ms.Read(content, 0, content.Length);
			ms.Close();

			return content;
		}


		private void ProcessElement(PdfPage page, Com.Delta.Print.Engine.ICustomPaint element)
		{
			if (element.Anchored)
			{
				if (element.Ready && !element.Displayed)
				{													
					Process(page, element);
					if (element.Done)
						element.Displayed = true;
								
				}
			}
			else
			{
				Process(page, element);
			}
		}

		private void Process(PdfPage page, Com.Delta.Print.Engine.ICustomPaint element)
		{
			if (element is TextField)
				ProcessTextField(page, (TextField)element);
			else if (element is PictureBox)
				ProcessPicture(page, (PictureBox)element);
			else if (element is StyledTable)
				ProcessStyledTable(page, (StyledTable)element);
			else if (element is ChartBox)
				ProcessChartBox(page, (ChartBox)element);
			else if (element is Timeline)
				ProcessTimeline(page, (Timeline)element);
			else if (element is Com.Delta.Print.Engine.Line)
				ProcessLine(page, (Com.Delta.Print.Engine.Line)element);
			else if (element is Com.Delta.Print.Engine.Elipse)
				ProcessElipse(page, (Com.Delta.Print.Engine.Elipse)element);
			else if (element is Com.Delta.Print.Engine.Box)
				ProcessBox(page, (Com.Delta.Print.Engine.Box)element);
			else if (element is Com.Delta.Print.Engine.Barcode)
				ProcessBarcode(page, (Com.Delta.Print.Engine.Barcode)element);
			else if (element is Com.Delta.Print.Engine.Map)
				ProcessMap(page, (Com.Delta.Print.Engine.Map)element);
			else if (element is Com.Delta.Print.Engine.Scatter)
				ProcessScatter(page, (Com.Delta.Print.Engine.Scatter)element);
			else if (element is Com.Delta.Print.Engine.UserPaint)
				ProcessUserPaint(page, (Com.Delta.Print.Engine.UserPaint)element);
			else if (element is Com.Delta.Print.Engine.RichTextField)
				ProcessRichTextField(page, (Com.Delta.Print.Engine.RichTextField)element);
		}

		private void ProcessTextField(PdfPage page, Com.Delta.Print.Engine.TextField textField)
		{
			try
			{
				//PdfArea area = new PdfArea(pdfDocument, Convert(textField.X), Convert(textField.Y), Convert(textField.Width), Convert(textField.Height));
				PdfArea area = new PdfArea(pdfDocument, Convert(textField.Bounds.X), Convert(textField.Bounds.Y), Convert(textField.Bounds.Width), Convert(textField.Bounds.Height));

				if ((textField.BorderWidth > 0 && textField.BorderColor != Color.Transparent) || textField.BackgroundColor != Color.Transparent)
				{
					PdfRectangle border = new PdfRectangle(pdfDocument, area, textField.BorderColor, textField.BorderWidth, textField.BackgroundColor);
					page.Add(border);
				}

				string content = textField.CurrentText.Replace("\r","");

				if (content != String.Empty)
				{
					//PdfArea textArea = new PdfArea(pdfDocument, Convert(textField.X + textField.Padding), Convert(textField.Y + textField.Padding), Convert(textField.Width - 2*textField.Padding), Convert(textField.Height - 2*textField.Padding));
					PdfArea textArea = new PdfArea(pdfDocument, Convert(textField.Bounds.X + textField.Padding), Convert(textField.Bounds.Y + textField.Padding), Convert(textField.Bounds.Width - 2*textField.Padding), Convert(textField.Bounds.Height - 2*textField.Padding));
				
					float ratio = 96f/ textField.Section.Document.GetGraphics().DpiX;			
				
					PdfTextArea pdfTextArea = new PdfTextArea(textField.Font, textField.ForegroundColor, textArea, GetTextHorizontalAlignment(textField.TextAlignment), GetTextVerticalAlignment(textField.TextVerticalAlignment), content, textField.TextOrientation==TextField.Orientation.Vertical);
				
					pdfTextArea.Underline = textField.Font.Underline;
					pdfTextArea.SetFontRatio(ratio);
					pdfTextArea.SetLineSpacing(Convert(textField.Spacing));
					if (textField.TextAlignment==TextField.TextAlignmentType.Justified)
						pdfTextArea.SetJustification(textField.Justification);
			
					page.Add(pdfTextArea);
				}
			}
			catch(Exception e){}
		}

		private void ProcessPicture(PdfPage page, Com.Delta.Print.Engine.PictureBox picture)
		{
			try
			{
				if (picture.Image==null && (picture.Image==null || picture.ImageFile==string.Empty))
					return;

				string imageFile = picture.Section.Document.DocRoot + Path.DirectorySeparatorChar + picture.ImageFile;
				

				PdfImage image = null;

				if (picture.Image != null)
				{
					int imageHash = picture.Image.GetHashCode();

					if (images.Contains(imageHash))
					{
						image = pdfDocument.GetObject((int)images[imageHash]) as PdfImage;
					}
					else
					{

						if (picture.Image.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
						{
							image = new PdfImage(pdfDocument.GetNextId, picture.Image, false);
						}
						else
						{
							image = pdfDocument.NewImage(picture.Image);
						}

						images[imageHash] = image.PublicID;

					}

					if (picture.Stretch)
					{
						image.Width = Convert(picture.Width);
						image.Height = Convert(picture.Height);
					}
					else
					{
						image.Width = Convert(picture.ImageSize.Width);
						image.Height = Convert(picture.ImageSize.Height);	
					}

				}
				else
				{
					if (images.Contains(imageFile))
					{
						image = pdfDocument.GetObject((int)images[imageFile]) as PdfImage;
						if (picture.Stretch)
						{
							image.Width = Convert(picture.Width);
							image.Height = Convert(picture.Height);
						}
						else
						{
							image.Width = Convert(picture.ImageSize.Width);
							image.Height = Convert(picture.ImageSize.Height);	
						}
					}
					else
					{
						double imageWidth = 0;
						double imageHeight = 0;
					
						if (picture.Stretch)
						{
							imageWidth = Convert(picture.Width);
							imageHeight = Convert(picture.Height);
						}
						else
						{
							imageWidth = Convert(picture.ImageSize.Width);
							imageHeight = Convert(picture.ImageSize.Height);	
						}

						image = pdfDocument.NewImage(picture.Section.Document.DocRoot + Path.DirectorySeparatorChar + picture.ImageFile, imageWidth, imageHeight);									
						images[imageFile] = image.PublicID;
					}
				}
				
				if (image!=null)
				{
					image.Quality = System.Convert.ToInt64(picture.ExportQuality);
					page.Add(image, Convert(picture.Bounds.X), Convert(picture.Bounds.Y), Convert(picture.Bounds.Width), Convert(picture.Bounds.Height));
				}
							
				if (picture.BorderWidth > 0)
				{
					PdfArea area = new PdfArea(pdfDocument, Convert(picture.Bounds.X), Convert(picture.Bounds.Y), Convert(picture.Bounds.Width), Convert(picture.Bounds.Height));
					PdfRectangle border = new PdfRectangle(pdfDocument, area, picture.BorderColor, picture.BorderWidth, Color.Transparent);
					page.Add(border);
				}
				
			}
			catch(Exception e){}
		}


		private void ProcessChartBox(PdfPage page, Com.Delta.Print.Engine.ChartBox chart)
		{
			try
			{
				if (chart.BackgroundColor != Color.Transparent && chart.MapAreaColor != Color.Transparent)
				{
					PdfImage image = new PdfImage(pdfDocument.GetNextId, chart.Image, false);

					image.Width = Convert(chart.Width);
					image.Height = Convert(chart.Height);				
					image.Quality = System.Convert.ToInt64(chart.ExportQuality);

					page.Add(image, Convert(chart.Bounds.X), Convert(chart.Bounds.Y), Convert(chart.Bounds.Width), Convert(chart.Bounds.Height));							
				}
				else
				{
					PdfImage image = pdfDocument.NewImage(chart.Image);

					image.Width = Convert(chart.Width);
					image.Height = Convert(chart.Height);
					image.Quality = System.Convert.ToInt64(chart.ExportQuality);

					page.Add(image, Convert(chart.Bounds.X), Convert(chart.Bounds.Y), Convert(chart.Bounds.Width), Convert(chart.Bounds.Height));							
				}
			}
			catch(Exception e){}
		}

		private void ProcessScatter(PdfPage page, Com.Delta.Print.Engine.Scatter chart)
		{
			try
			{
				if (chart.BackgroundColor != Color.Transparent && chart.MapAreaColor != Color.Transparent)
				{
					PdfImage image = new PdfImage(pdfDocument.GetNextId, chart.Image, false);

					image.Width = Convert(chart.Width);
					image.Height = Convert(chart.Height);
					image.Quality = System.Convert.ToInt64(chart.ExportQuality);

					page.Add(image, Convert(chart.Bounds.X), Convert(chart.Bounds.Y), Convert(chart.Bounds.Width), Convert(chart.Bounds.Height));							
				}
				else
				{
					PdfImage image = pdfDocument.NewImage(chart.Image);

					image.Width = Convert(chart.Width);
					image.Height = Convert(chart.Height);
					image.Quality = System.Convert.ToInt64(chart.ExportQuality);

					page.Add(image, Convert(chart.Bounds.X), Convert(chart.Bounds.Y), Convert(chart.Bounds.Width), Convert(chart.Bounds.Height));							
				}
			}
			catch(Exception e){}
		}

		private void ProcessUserPaint(PdfPage page, Com.Delta.Print.Engine.UserPaint userPaint)
		{
			try
			{
				
				PdfImage image = pdfDocument.NewImage(userPaint.Image);

				image.Width = Convert(userPaint.Width);
				image.Height = Convert(userPaint.Height);				

				page.Add(image, Convert(userPaint.Bounds.X), Convert(userPaint.Bounds.Y), Convert(userPaint.Bounds.Width), Convert(userPaint.Bounds.Height));							
								
			}
			catch(Exception e)
			{
				Console.WriteLine(e.StackTrace);
			}
		}

		private void ProcessRichTextField(PdfPage page, Com.Delta.Print.Engine.RichTextField richTextField)
		{
			try
			{
				PdfArea area = new PdfArea(pdfDocument, Convert(richTextField.Bounds.X), Convert(richTextField.Bounds.Y), Convert(richTextField.Bounds.Width), Convert(richTextField.Bounds.Height));

				if ((richTextField.BorderWidth > 0 && richTextField.BorderColor != Color.Transparent) || richTextField.BackgroundColor != Color.Transparent)
				{
					PdfRectangle border = new PdfRectangle(pdfDocument, area, richTextField.BorderColor, Convert(richTextField.BorderWidth), richTextField.BackgroundColor);
					page.Add(border);
				}

				PdfRichTextBox p = new PdfRichTextBox(pdfDocument, (int)Convert(richTextField.Bounds.X + richTextField.Padding), (int)Convert(richTextField.Bounds.Y + richTextField.Padding), (int)Convert(richTextField.Bounds.Width - 2*richTextField.Padding), (int)Convert(richTextField.Bounds.Height - 2*richTextField.Padding));
				p.SetLines(new ArrayList(richTextField.lines));

				page.Add(p);
			}
			catch(Exception e)
			{
				Console.WriteLine(e.StackTrace);
			}
		}


		private void ProcessTimeline(PdfPage page, Com.Delta.Print.Engine.Timeline timeline)
		{
			try
			{
				if (timeline.BackgroundColor != Color.Transparent)
				{
					PdfImage image = new PdfImage(pdfDocument.GetNextId, timeline.Image, false);

					image.Width = Convert(timeline.Width);
					image.Height = Convert(timeline.Height);
					image.Quality = System.Convert.ToInt64(timeline.ExportQuality);

					page.Add(image, Convert(timeline.Bounds.X), Convert(timeline.Bounds.Y), Convert(timeline.Bounds.Width), Convert(timeline.Bounds.Height));
				}
				else
				{
					PdfImage image = pdfDocument.NewImage(timeline.Image);

					image.Width = Convert(timeline.Width);
					image.Height = Convert(timeline.Height);
					image.Quality = System.Convert.ToInt64(timeline.ExportQuality);

					page.Add(image, Convert(timeline.Bounds.X), Convert(timeline.Bounds.Y), Convert(timeline.Bounds.Width), Convert(timeline.Bounds.Height));							
				}
			}
			catch(Exception e){}
		}

		private void ProcessLine(PdfPage page, Com.Delta.Print.Engine.Line line)
		{
			try
			{
				PdfLine pdfLine = new PdfLine(pdfDocument, new PointF((float)Convert(line.Coordinates[0].X), (float)Convert(line.Coordinates[0].Y)), new PointF((float)Convert(line.Coordinates[1].X), (float)Convert(line.Coordinates[1].Y)), line.Color, Convert(line.LineWidth));	
				page.Add(pdfLine);				
			}
			catch(Exception){}
		}


		private void ProcessElipse(PdfPage page, Com.Delta.Print.Engine.Elipse elipse)
		{
			try
			{
				if (elipse.FillStyle == Elipse.FillStyles.Solid)
				{
					
					if (elipse.BorderWidth > 0 || elipse.Color != Color.Transparent)
					{
						
						PdfCircle pdfCircle = new PdfCircle(Convert(elipse.Bounds.X), Convert(elipse.Bounds.Y), elipse.Color, elipse.BorderColor, elipse.BorderWidth);
						pdfCircle.AxesArea = new PdfArea(pdfDocument, Convert(elipse.Bounds.X), Convert(elipse.Bounds.Y), Convert(elipse.Bounds.Width), Convert(elipse.Bounds.Height));
						
						page.Add(pdfCircle);
					}					
				}
				else
				{
					int elipseHash = elipse.GetHashCode();
					PdfImage image = null;
					if (images.Contains(elipseHash))
					{
						image = pdfDocument.GetObject((int)images[elipseHash]) as PdfImage;
					}
					else
					{
						image = pdfDocument.NewImage(elipse.Image);
						images[elipseHash] = image.PublicID;
					}

					image.Width = Convert(elipse.Bounds.Width);
					image.Height = Convert(elipse.Bounds.Height);				

					page.Add(image, Convert(elipse.Bounds.X), Convert(elipse.Bounds.Y), Convert(elipse.Bounds.Width), Convert(elipse.Bounds.Height));	
				}
				
			}
			catch(Exception e){}
		}

		private void ProcessBox(PdfPage page, Com.Delta.Print.Engine.Box box)
		{
			try
			{
				if (box.FillStyle == Box.FillStyles.Solid)
				{
					PdfArea area = new PdfArea(pdfDocument, Convert(box.Bounds.X), Convert(box.Bounds.Y), Convert(box.Bounds.Width), Convert(box.Bounds.Height));

					if (box.BorderWidth > 0 || box.Color != Color.Transparent)
					{
						PdfRectangle pdfBox = new PdfRectangle(pdfDocument, area, box.BorderColor, box.BorderWidth, box.Color);
						page.Add(pdfBox);
					}
					
				}
				else
				{
					int boxHash = box.GetHashCode();
					PdfImage image = null;
					if (images.Contains(boxHash))
					{
						image = pdfDocument.GetObject((int)images[boxHash]) as PdfImage;
					}
					else
					{
						if (box.Color != Color.Transparent && box.GradientColor != Color.Transparent)
						{
							image = new PdfImage(pdfDocument.GetNextId, box.Image, false);		
						}
						else
						{
							image = pdfDocument.NewImage(box.Image);	
						}
						images[boxHash] = image.PublicID;
					}

					image.Width = Convert(box.Width);
					image.Height = Convert(box.Height);	

					page.Add(image, Convert(box.Bounds.X), Convert(box.Bounds.Y), Convert(box.Bounds.Width), Convert(box.Bounds.Height));
				}
				
			}
			catch(Exception e){}
		}

		private void ProcessBarcode(PdfPage page, Com.Delta.Print.Engine.Barcode barcode)
		{
			try
			{
				int barcodeHash = barcode.GetHashCode();

				PdfImage image = null;
				if (images.Contains(barcodeHash))
				{
					image = pdfDocument.GetObject((int)images[barcodeHash]) as PdfImage;
				}
				else
				{
					image = new PdfImage(pdfDocument.GetNextId, barcode.Image, false);
					image.Interpolate = false;
					images[barcodeHash] = image.PublicID;

				}

				image.Width = Convert(barcode.Bounds.Width);
				image.Height = Convert(barcode.Bounds.Height);				

				page.Add(image, Convert(barcode.Bounds.X), Convert(barcode.Bounds.Y), Convert(barcode.Bounds.Width), Convert(barcode.Bounds.Height));							
							
			}
			catch(Exception e){}
		}

		private void ProcessMap(PdfPage page, Com.Delta.Print.Engine.Map map)
		{
			try
			{
				int mapHash = map.GetHashCode();
				PdfImage image = null;
				if (images.Contains(mapHash))
				{
					image = pdfDocument.GetObject((int)images[mapHash]) as PdfImage;
				}
				else
				{
					if (map.ForegroundColor != Color.Transparent && map.BackgroundColor != Color.Transparent)
					{
						image = new PdfImage(pdfDocument.GetNextId, map.Image, false);
					}
					else
					{
						image = pdfDocument.NewImage(map.Image);							
					}
					images[mapHash] = image.PublicID;
				}

				image.Width = Convert(map.Bounds.Width);
				image.Height = Convert(map.Bounds.Height);
				image.Quality = System.Convert.ToInt64(map.ExportQuality);

				page.Add(image, Convert(map.Bounds.X), Convert(map.Bounds.Y), Convert(map.Bounds.Width), Convert(map.Bounds.Height));
							
			}
			catch(Exception e){}
		}




		private void ProcessStyledTable(PdfPage page, Com.Delta.Print.Engine.StyledTable table)
		{
			
			try
			{

				DataTable data = table.DataSource == null ? table.DisplayData : table.Data;

				int visibleColumnCount = table.GetVisibleColumnsCount();

				if (data==null)
				{
					data = new DataTable();
					for (int i=0;i<visibleColumnCount;i++)
						data.Columns.Add();
				}


				if (data.Rows.Count==0)
				{
					if (table.DrawEmptyRows)
					{
						int maxRows = table.Bounds.Height/table.CellHeight;
						PdfTable pdfTable = pdfDocument.NewTable(table.DataFont, maxRows, visibleColumnCount, 2 );

						System.Data.DataTable dt = new System.Data.DataTable();
						for (int i=0;i<visibleColumnCount;i++)
						{
							dt.Columns.Add("");
						}

						for (int i=0;i<table.Height/table.CellHeight;i++)
						{
							dt.Rows.Add(dt.NewRow());
							if (dt.Columns.Count>0)
								dt.Rows[i][0] = " " ;
						}

						pdfTable.ImportDataTable(dt);
			
						pdfTable.SetRowHeight(Convert(table.CellHeight));

						pdfTable.HeadersRow.SetRowHeight(Convert(table.CellHeight));
						pdfTable.HeadersRow.SetColors(table.HeaderFontColor, table.HeaderBackgroundColor);
						pdfTable.HeadersRow.SetFont(table.HeaderFont);

						int count = 0;
						for (int i=0;i<table.Columns.Length;i++)
						{
							if (table.Columns[i].Width>0)
							{
								string columnName = table.Columns[i].Label == String.Empty || table.Columns[i].Label == "" ? " " : table.Columns[i].Label; 
								pdfTable.HeadersRow[count].SetContent(columnName);
								count++;
							}
						}

						pdfTable.SetColors(table.DataFontColor, table.BackgroundColor);
						pdfTable.SetBorders(table.BorderColor, 1, BorderType.CompleteGrid);

						if (table.AlternateBackColor)
						{
							for (int i=0;i<dt.Rows.Count;i++)
							{
								if (i%2==1)
									pdfTable.Rows[i].SetBackgroundColor(table.AlternatingBackColor);
							}
						}

			
						int[] columnWidths = new int[visibleColumnCount];
						int tableWidth = 0;
						count = 0;
						for (int i=0;i<table.Columns.Length;i++)
						{
							if (table.Columns[i].Width>0)
							{
								columnWidths[count] = (int)Convert(table.Columns[i].Width);
								tableWidth += columnWidths[count];
								count++;
							}
						}
						pdfTable.SetColumnsWidth(columnWidths);

						count = 0;
						for (int i=0;i<table.Columns.Length;i++)
						{
							if (table.Columns[i].Width>0)
							{
								Com.Delta.Print.Engine.Pdf.HorizontalAlignment columnAlignment = GetColumnContentAlignment(table.Columns[i].Alignment);
								pdfTable.HeadersRow[count].SetContentAlignment(columnAlignment, VerticalAlignment.Middle);
								pdfTable.Columns[count].SetContentAlignment(columnAlignment, VerticalAlignment.Middle);
								
								count++;
							}
						}

						pdfTable.VisibleHeaders = table.DrawHeader;								
						PdfTablePage tablePage = pdfTable.CreateTablePage(new PdfArea(pdfDocument, Convert(table.Bounds.X), Convert(table.Bounds.Y), tableWidth, Convert(table.Bounds.Height)));				
						page.Add(tablePage);
					}
					else if (table.DrawHeader && !table.DrawEmptyRows)
					{
						int position = table.Bounds.X;

						int headerRelativeHeight = 1;
						for (int i=0;i<table.Columns.Length;i++)
						{
							if (table.Columns[i].Width > 0)
							{
								PdfArea area = new PdfArea(pdfDocument, Convert(position+2), Convert(table.Bounds.Y+2), Convert(table.Columns[i].Width-4), Convert(table.CellHeight-4));
								
								PdfTextArea pdfTextArea = new PdfTextArea(table.HeaderFont, table.HeaderFontColor, area, GetColumnContentAlignment(table.Columns[i].Alignment), VerticalAlignment.Middle, table.Columns[i].Label, false);			
								int minimumLines = pdfTextArea.RenderLines().Count;
								headerRelativeHeight = Math.Max(headerRelativeHeight, minimumLines);								
							}
						}	

						int headerHeight = table.CellHeight * headerRelativeHeight;

						for (int i=0;i<table.Columns.Length;i++)
						{
							if (table.Columns[i].Width>0)
							{
								PdfArea area = new PdfArea(pdfDocument, Convert(position+2), Convert(table.Bounds.Y+2), Convert(table.Columns[i].Width-4), Convert(headerHeight-4));
								PdfArea borderArea = new PdfArea(pdfDocument, Convert(position), Convert(table.Bounds.Y), Convert(table.Columns[i].Width), Convert(headerHeight));

								PdfRectangle border = new PdfRectangle(pdfDocument, borderArea, table.BorderColor, 1, table.HeaderBackgroundColor);
								page.Add(border);

								PdfTextArea pdfTextArea = new PdfTextArea(table.HeaderFont, table.HeaderFontColor, area, GetColumnContentAlignment(table.Columns[i].Alignment), VerticalAlignment.Middle, table.Columns[i].Label, false);			

								// arbitrary line spacing
								pdfTextArea.SetLineSpacing(Convert(table.CellHeight / 8));
								page.Add(pdfTextArea);

								position += table.Columns[i].Width;
							}
						}	
					}
					else
					{
						return;
					}
				}
				else
				{					

					int maxRows = table.Height/table.CellHeight;					
					int rowCount = data.Rows.Count;

					PdfTable pdfTable ;

					double cellPadding = Math.Max(0, Math.Min(2, Math.Floor(0.5 * (table.CellHeight - table.DataFont.GetHeight()))));   

					if (table.DrawEmptyRows)
						pdfTable = pdfDocument.NewTable(table.DataFont, rowCount + maxRows, visibleColumnCount, cellPadding );
					else
						pdfTable = pdfDocument.NewTable(table.DataFont, rowCount, visibleColumnCount, cellPadding );

					pdfTable.pdfPage = page;
			
					System.Data.DataTable dt = new System.Data.DataTable();
					for (int i=0;i<table.Columns.Length;i++)
					{
						if (table.Columns[i].Width>0)
						{
							if (table.Columns[i].FormatMask == "Image")
								dt.Columns.Add("", typeof(byte[]));
							else
								dt.Columns.Add("", typeof(System.String));
						}
					}
			
					for (int i=0;i<data.Rows.Count;i++)
					{
						if (data.Rows[i].RowError == "Subtotal")
						{
							object[] rowData = new object[visibleColumnCount];
							int cnt = 0;
							for (int j=0;j<table.Columns.Length;j++)
							{
								if (table.Columns[j].Width>0)
								{
									rowData[cnt] = table.Subtotals[j];
									cnt++;
								}								
							}

							dt.Rows.Add(rowData);
						}
						else
						{
							object[] rowData = new object[visibleColumnCount];
							int cnt = 0;
							for (int j=0;j<table.Columns.Length;j++)
							{
								if (table.Columns[j].Width>0)
								{
									if (table.Columns[j].FormatMask == null || table.Columns[j].FormatMask == String.Empty)
										rowData[cnt] = String.Format("{0}", data.Rows[i][j]);
									else if (table.Columns[j].FormatMask == "Image")
										rowData[cnt] = data.Rows[i][j] is byte[] ? data.Rows[i][j] : null;
									else
										rowData[cnt] = String.Format("{0:" + table.Columns[j].FormatMask + "}", data.Rows[i][j]); 
									cnt++;
								}								
							}
							dt.Rows.Add(rowData);
						}
					}

					if (table.DrawEmptyRows)
					{
						for (int i=0;i<table.Height/table.CellHeight;i++)
						{
							dt.Rows.Add(dt.NewRow());
						}
						
					}

					pdfTable.ImportDataTable(dt);

			
					pdfTable.SetRowHeight(Convert(table.CellHeight));

					pdfTable.VisibleHeaders = table.DrawHeader;
					pdfTable.HeadersRow.SetRowHeight(Convert(table.CellHeight));
					pdfTable.HeadersRow.SetColors(table.HeaderFontColor, table.HeaderBackgroundColor);
					pdfTable.HeadersRow.SetFont(table.HeaderFont);

					pdfTable.SetColors(table.DataFontColor, table.BackgroundColor);
					pdfTable.SetBorders(table.BorderColor, 1, BorderType.CompleteGrid);
					//pdfTable.SetBorders(table.BorderColor, (table.BorderColor == Color.Transparent ? 0 : 1), BorderType.CompleteGrid);

			
					int[] columnWidths = new int[visibleColumnCount];
					int tableWidth = 0;
					int count = 0;
					for (int i=0;i<table.Columns.Length;i++)
					{
						if (table.Columns[i].Width>0)
						{
							columnWidths[count] = (int)Convert(table.Columns[i].Width);
							tableWidth += columnWidths[count];

							string columnName = table.Columns[i].Label == String.Empty || table.Columns[i].Label == "" ? " " : table.Columns[i].Label; 
							pdfTable.HeadersRow[count].SetContent(columnName);

							Com.Delta.Print.Engine.Pdf.HorizontalAlignment columnAlignment = GetColumnContentAlignment(table.Columns[i].Alignment);
							pdfTable.HeadersRow[count].SetContentAlignment(columnAlignment, VerticalAlignment.Middle);
							pdfTable.Columns[count].SetContentAlignment(columnAlignment, VerticalAlignment.Middle);

							count++;
						}
					}
					pdfTable.SetColumnsWidth(columnWidths);


					ArrayList alterRows = table.AlterRows;
					for (int i=0;i<data.Rows.Count;i++)
					{
						if (table.AlternateBackColor && i%2==1)
						{
							pdfTable.Rows[i].SetBackgroundColor(table.AlternatingBackColor);
						}

						if (alterRows != null && alterRows.Count>0)
						{
							if (alterRows.Contains(data.Rows[i]) && i<pdfTable.Rows.Length)
							{
								pdfTable.Rows[i].SetForegroundColor(table.AlterDataColor);
								pdfTable.Rows[i].SetBackgroundColor(table.AlterDataBackColor);
							}
						}
					}

					
					if (table.DrawEmptyRows && table.AlternateBackColor)
					{
						for (int i=data.Rows.Count;i<table.Height/table.CellHeight;i++)
						{
							if (i % 2 == 1)
							{
								pdfTable.Rows[i].SetBackgroundColor(table.AlternatingBackColor);
							}
						}
					}
					
					

					if (table.HasSubtotals)
					{
						for (int i=0;i<data.Rows.Count;i++)
						{
							if (data.Rows[i].RowError == "Subtotal" && i<pdfTable.Rows.Length)
								pdfTable.Rows[i].SetForegroundColor(table.SubtotalsColor);
						}
					}
										
					PdfTablePage tablePage = pdfTable.CreateTablePage(new PdfArea(pdfDocument, Convert(table.Bounds.X), Convert(table.Bounds.Y), tableWidth, Convert(table.Bounds.Height)));
				
					page.Add(tablePage);

					foreach(PdfImage image in pdfTable.Images.Keys)
					{
						RectangleF area = (RectangleF)pdfTable.Images[image];
						page.Add(image, area.X, area.Y, 0.72*area.Width, 0.72*area.Height);
					}

					
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.StackTrace);
			}
			
		}

		private Com.Delta.Print.Engine.Pdf.HorizontalAlignment GetColumnContentAlignment(Com.Delta.Print.Engine.StyledTableColumn.AlignmentType alignment)
		{
			switch (alignment)
			{
				case StyledTableColumn.AlignmentType.Left : return Com.Delta.Print.Engine.Pdf.HorizontalAlignment.Left;
				case StyledTableColumn.AlignmentType.Center : return Com.Delta.Print.Engine.Pdf.HorizontalAlignment.Center;
				case StyledTableColumn.AlignmentType.Right : return Com.Delta.Print.Engine.Pdf.HorizontalAlignment.Right;
				default : return Com.Delta.Print.Engine.Pdf.HorizontalAlignment.Left;
			}
		}

		private Com.Delta.Print.Engine.Pdf.HorizontalAlignment GetTextHorizontalAlignment(TextField.TextAlignmentType horAlignment)
		{
			if (horAlignment == TextField.TextAlignmentType.Right)
			{
				return Com.Delta.Print.Engine.Pdf.HorizontalAlignment.Right;
			}
			else if (horAlignment == TextField.TextAlignmentType.Center)
			{
				return Com.Delta.Print.Engine.Pdf.HorizontalAlignment.Center;
			}
			else if (horAlignment == TextField.TextAlignmentType.Justified)
			{
				return Com.Delta.Print.Engine.Pdf.HorizontalAlignment.Justified;
			}
			else
			{
				return Com.Delta.Print.Engine.Pdf.HorizontalAlignment.Left;
			}
		}

		private Com.Delta.Print.Engine.Pdf.VerticalAlignment GetTextVerticalAlignment(TextField.TextVerticalAlignmentType verAlignment)
		{
			if (verAlignment == TextField.TextVerticalAlignmentType.Bottom)
			{
				return Com.Delta.Print.Engine.Pdf.VerticalAlignment.Bottom;
			}
			else if (verAlignment == TextField.TextVerticalAlignmentType.Middle)
			{
				return Com.Delta.Print.Engine.Pdf.VerticalAlignment.Middle;
			}
			else
			{
				return Com.Delta.Print.Engine.Pdf.VerticalAlignment.Top;
			}
		}

		private double Convert(int size)
		{
			return size*0.72;
		}


		

	}
}
