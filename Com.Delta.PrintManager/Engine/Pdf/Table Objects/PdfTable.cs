
using System;
using System.Data;
using System.Collections;
using System.Text;
using System.Drawing;
using System.IO;

namespace Com.Delta.PrintManager.Engine.Pdf
{
	/// <summary>
	/// the generic Pdf Table class.
	/// </summary>
	public class PdfTable : Com.Delta.PrintManager.Engine.Pdf.PdfCellRange
	{
		internal int columns;
		internal int rows;
		internal Hashtable cells;
		internal PdfArea TableArea;
		internal PdfTable header = null;
		internal PdfDocument PdfDocument;
		internal int renderingIndex;
		internal int renderingRows;
		internal bool visibleHeaders = true;


		internal ArrayList pdfRows;
		internal ArrayList pdfColumns;
		

		private double borderWidth = 1;
		private Color borderColor = Color.Black;
		private BorderType borderType = BorderType.CompleteGrid;

		private PdfRow[] _Rows;
		private PdfColumn[] _Columns;

		internal PdfPage pdfPage;
		internal Hashtable Images = new Hashtable();

		


		public bool VisibleHeaders
		{
			get {return visibleHeaders;}
			set {visibleHeaders = value;}
		}
		
		public PdfRow HeadersRow
		{
			get {return this.header.Rows[0];}
		}
		
		/// <summary>
		/// the Collection of the Rows of the Table.
		/// </summary>
		public PdfRow[] Rows
		{
			get
			{
				if (this._Rows == null) 
				{
					this._Rows = (PdfRow[])this.pdfRows.ToArray(typeof(PdfRow));
				}
				return this._Rows;
			}
		}
		
		/// <summary>
		/// the Collection of the Columns of the Table.
		/// </summary>
		public PdfColumn[] Columns
		{
			get
			{
				if (this._Columns==null) this._Columns=this.pdfColumns.ToArray(typeof(PdfColumn)) as PdfColumn[];
				return this._Columns;
			}
		}
	
	
		/// <summary>
		/// sets the borders style of the Table.
		/// </summary>
		/// <param name="BorderColor"></param>
		/// <param name="BorderWidth"></param>
		/// <param name="BorderType"></param>
		public void SetBorders(Color color, double width, BorderType type)
		{
			if (width<0) throw new Exception("BorderWidth must be grater than zero.");
			this.borderColor = color;
			this.borderType = type;
			this.borderWidth = width;
			if (this.header != null)
			{
				this.header.borderColor = color;
				this.header.borderType = type;
				this.header.borderWidth = width;
			}
		}
		/// <summary>
		/// sets the widths of the Columns.
		/// </summary>
		/// <param name="ColumnsWidthArray"></param>
		public void SetColumnsWidth(int[] ColumnsWidthArray)
		{
			if (ColumnsWidthArray.Length>this.columns) throw new Exception("Table has only "+this.columns+" columns.");
			for (int index=0;index<ColumnsWidthArray.Length;index++)
			{
				if (ColumnsWidthArray[index]<=0) throw new Exception("Column size must be greater than zero.");
				this.Columns[index].SetWidth(ColumnsWidthArray[index]);
			}
		}
		

		public PdfCell Cell(int Row,int Column)
		{
			object o = this.cells[Row+","+Column];
			if (o==null) throw new Exception("Cell ["+Row+","+Column+"] does not exist in the Table.");
			return o as PdfCell;
		}
		

		public PdfCellRange CellRange(int startRow, int startColumn, int endRow, int endColumn)
		{
			return new PdfCellRange(this, startRow, startColumn, endRow, endColumn);
		}

		internal PdfTable(PdfDocument PdfDocument,ContentAlignment DefaultContentAlignment,Font Font,Color DefaultForegroundColor,int Rows, int Columns,double CellPadding)
		{
			cells=new Hashtable();
			pdfRows=new ArrayList();
			this.owner=this;
			this.PdfDocument=PdfDocument;
			this.borderWidth=0;
			this.pdfColumns=new ArrayList();
			this.rows=Rows;
			this.startRow=0;
			this.startColumn=0;
			this.endColumn=Columns-1;
			this.endRow=Rows-1;
			this.columns=Columns;
			
			
			for (int c=0;c<columns;c++)
				for (int r=0;r<rows;r++)
				{
					this.cells[r+","+c]=new PdfCell(this,r,c,HorizontalAlignment.Left, VerticalAlignment.Top,DefaultForegroundColor, Font, CellPadding);
				}
			for (int r=0;r<rows;r++)
			{
				this.pdfRows.Add(new PdfRow(this,r));
			}

			for (int c=0;c<columns;c++)
			{
				PdfColumn pc = new PdfColumn(this,c);
				pc.Width = 100/this.columns;
				this.pdfColumns.Add(pc);
			}
		}
		

		public void SetRowHeight(double height)
		{
			if (height<=0) throw new Exception("Row Height must be grater than zero.");
			foreach (PdfRow pr in this.Rows)
			{
				pr.SetRowHeight(height);
			}
		}
		/// <summary>
		/// Imports text from a datatable.
		/// </summary>
		/// <param name="dt">The Source Datatable</param>
		/// <param name="PdfTableStartRow">the starting row of the Pdf Table that will import datas.</param>
		/// <param name="PdfTableStartColumn">the starting column of the Pdf Table that will import datas.</param>
		/// <param name="DataTableStartRow">the starting row of the DataTable that will export datas.</param>
		/// <param name="DataTableEndRow">the ending row of the DataTable that will export datas.</param>
		public void ImportDataTable(DataTable dt,int PdfTableStartRow,int PdfTableStartColumn, int DataTableStartRow, int DataTableEndRow)
		{
			for (int c=0;((c<dt.Columns.Count)&&(c+PdfTableStartColumn<this.columns));c++)
			{
				if (c+PdfTableStartColumn>=0 && c+PdfTableStartColumn<dt.Columns.Count) 
				{
					this.HeadersRow[c+PdfTableStartColumn].SetContent(dt.Columns[c].ColumnName);
				}
			}

			for (int r=DataTableStartRow;((r<dt.Rows.Count)&&(r<=DataTableEndRow)&& (r+PdfTableStartRow-DataTableStartRow<this.rows));r++)
			{
				for (int c=0;((c<dt.Columns.Count)&&(c+PdfTableStartColumn<this.columns));c++)
				{
					if (c+PdfTableStartColumn>=0&&c+PdfTableStartColumn<dt.Columns.Count) 
					{
						Cell(r+PdfTableStartRow-DataTableStartRow,c+PdfTableStartColumn).SetContent(dt.Rows[r][c]);	
					}
				}
			}

			
		}
		

		public void ImportDataTable(DataTable dt,int startRow,int startColumn)
		{
			ImportDataTable(dt, startRow, startColumn, 0, 999999);
		}
		
		public void ImportDataTable(DataTable dt)
		{
			ImportDataTable(dt, 0, 0, 0, 999999);
		}


		/// <summary>
		/// Creates the TablePage, the rasterized page of a Table.
		/// </summary>
		/// <param name="PageArea"></param>
		/// <returns></returns>
		public PdfTablePage CreateTablePage(PdfArea PageArea)
		{
			
			this.TableArea = PageArea.Clone();
			PdfTablePage ptp;
			if (!this.visibleHeaders)
			{
				ptp = this.createTablePage();
			}
			else			
			{
				this.header.TableArea = PageArea.Clone();
				double headerHeight = this.HeadersRow.Height;
				this.header.TableArea.height = headerHeight;
				this.TableArea.posy += headerHeight;
				this.TableArea.height -= headerHeight;

				this.TableArea.height = this.TableArea.height * 1.0000001;
				
				ptp = this.createTablePage();


				ByteBuffer byteBuffer = new ByteBuffer();
				byteBuffer.Append(ptp.byteStream);

				if (this.visibleHeaders)
					byteBuffer.Append(this.header.createTablePage().byteStream);

				ptp.byteStream = byteBuffer.ToByteArray();

				this.header.renderingIndex=0;
			}

				
			foreach (PdfColumn pc in this.Columns) pc.CompensatedWidth=-1;
			foreach (PdfColumn pc in this.header.Columns) pc.CompensatedWidth=-1;
			return ptp;
		}


		internal PdfTablePage createTablePage()
		{
		
			int index = this.renderingIndex;
			double h = this.Rows[index].Height;


			double oh=0;
			while ((h <= this.TableArea.height) && (index < this.rows))
			{
				index++;
				oh = h;
				if (index<this.rows) 
				{ 
					double rowHeight = this.Rows[index].Height;
					h += rowHeight;
				}
				
			}
			this.renderingRows=index-this.renderingIndex;
			this.TableArea.height = oh;

		
			PdfTablePage ptp = new PdfTablePage(renderingIndex, renderingIndex+renderingRows-1, columns);
			
			
			//caculates areas
			double y = this.TableArea.posy;
			for (int rowIndex=this.renderingIndex;(rowIndex<this.renderingIndex+this.renderingRows);rowIndex++)
			{
				double x = this.TableArea.posx;
				double rowHeight = this.Rows[rowIndex].Height;
				for (int columnIndex=0;columnIndex<this.columns;columnIndex++)
				{
					PdfCell pc = this.Cell(rowIndex,columnIndex);
					double width = pc.Width;
																				
					pc.area = new PdfArea(this.PdfDocument, x, y, width, rowHeight);																				
					x+=width;

				} 

				y += rowHeight;
			}	
			
			for (int rowIndex=this.renderingIndex;(rowIndex<this.renderingIndex+this.renderingRows);rowIndex++)
			{
				for (int columnIndex=0;columnIndex<this.columns;columnIndex++)
				{
					PdfCell pc = this.Cell(rowIndex,columnIndex);
	
						ptp.cellAreas.Add(pc.row + "," + pc.column, pc.Area);
				}
			}


			ptp.byteStream = this.ToByteStream();	
			ptp.SetArea();	
	
			this.renderingIndex=index;
			this.renderingRows=0;
			return ptp;
		}



		internal byte[] ToByteStream()
		{
			ByteBuffer byteBuffer = new ByteBuffer();
			

			System.Text.StringBuilder sb = new StringBuilder();
			for (int rowIndex=this.renderingIndex;(rowIndex<this.renderingIndex+this.renderingRows);rowIndex++)
			{
				for (int columnIndex=0;columnIndex<this.columns;columnIndex++)
				{
					PdfCell pc = this.Cell(rowIndex, columnIndex);					
					
					if (!pc.transparent)
					{							
						sb.Append(pc.Area.InnerArea(1).ToRectangle(pc.backgroundColor, pc.backgroundColor).ToLineStream());							
					}					
				}
			}

			byteBuffer.Append(sb.ToString());

			byteBuffer.Append("BT\n");


			
			Font actualFont = null;
			Color actualColor = Color.Black;
			byteBuffer.Append(Utility.ColorrgLine(Color.Black));

			sb = new StringBuilder();
			if (this.borderWidth > 0 && this.borderColor != Color.Transparent)
			{
				//sb.Append(new PdfRectangle(this.PdfDocument, new PdfArea(this.PdfDocument,0,0,1,1), this.borderColor, this.borderWidth).ToColorAndWidthStream());
				sb.Append(new PdfRectangle(this.PdfDocument, this.TableArea, this.borderColor, this.borderWidth).ToLineStream());
				//sb.Append(this.TableArea.ToRectangle(this.borderColor, this.borderWidth).ToRectangleStream());
			}			

			for (int rowIndex=this.renderingIndex;(rowIndex<this.renderingIndex+this.renderingRows);rowIndex++)
			{
				for (int columnIndex=0;columnIndex<this.columns;columnIndex++)
				{
					PdfCell pc = this.Cell(rowIndex, columnIndex);
					
					

					
					if (pc.Content is byte[])
					{
						byte[] d = (byte[])pc.Content;
					
						try
						{
							Bitmap image = (Bitmap)Bitmap.FromStream(new System.IO.MemoryStream(d));
							PdfImage pdfImage = this.PdfDocument.NewImage(image);

							pdfImage.Width = 0.72 * image.Width;
							pdfImage.Height = 0.72 * image.Height;

							double xPos = pc.Area.PosX + 1;
							double yPos = pc.Area.PosY + (pc.Area.Height - pdfImage.Height) / 2;
							if (pc.horizontalAlignment == HorizontalAlignment.Center)
							{
								xPos = pc.Area.PosX + (pc.Area.Width - pdfImage.Width) / 2;
							}
							else if (pc.horizontalAlignment == HorizontalAlignment.Right)
							{
								xPos = pc.Area.PosX + pc.Area.Width - pdfImage.Width - 1;
							}

							Images[pdfImage] = new RectangleF((float)xPos, (float)yPos, image.Width, image.Height);
						}
						catch(Exception)
						{

						}
						
						
					}
					else
					{
						pc.PdfTextArea.textArea = pc.Area.InnerArea(pc.cellPadding*2);					

										
					

						if (pc.Font!=actualFont)
						{
							byteBuffer.Append(String.Format("/{0} {1} Tf\n", pc.PdfTextArea.pdfFont.Alias, pc.Font.Size));
							actualFont = pc.Font;
						}

						if (pc.foregroundColor!=actualColor)
						{
							byteBuffer.Append(Utility.ColorrgLine(pc.foregroundColor));
							actualColor = pc.foregroundColor;
						}

						byteBuffer.Append(pc.PdfTextArea.ToByteStream());
					}

					
					if (borderWidth > 0 && this.borderColor != Color.Transparent)
					{
						if (rowIndex!=this.renderingIndex)
						{
							sb.Append(pc.Area.UpperBound(this.borderColor, this.borderWidth).ToLineStream());
						}
						if (columnIndex!=0)
						{
							sb.Append(pc.Area.LeftBound(this.borderColor, this.borderWidth).ToLineStream());
						}
					}
					
					
				}
				
			}

			byteBuffer.Append("ET\n");


			byteBuffer.Append(sb.ToString());
			return byteBuffer.ToByteArray();
		}



	}
}
