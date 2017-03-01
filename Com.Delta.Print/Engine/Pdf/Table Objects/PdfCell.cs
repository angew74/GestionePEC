
using System;
using System.Drawing;

namespace Com.Delta.Print.Engine.Pdf
{
	/// <summary>
	/// Cell of a PdfTable
	/// </summary>
	public class PdfCell
	{
		
		// cell variables
		internal double cellPadding;
		internal int row;
		internal int column;
		internal int colSpan;
		internal int rowSpan;
		
		private double neededHeight = 0;

		// content variables
		internal object content;
		internal string stringFormat;
		internal PdfTable owner;
		
		
		internal PdfArea area;
		private PdfArea _Area;
		private PdfTextArea pcatemp;
		
		// display variables
		internal bool transparent;
		internal Color foregroundColor;
		internal Color backgroundColor;
		internal Font Font;

		// aligmnent variables
		internal HorizontalAlignment horizontalAlignment;
		internal VerticalAlignment verticalAlignment;
		

		internal PdfCell(PdfTable owner, int row, int column, HorizontalAlignment horAlignment, VerticalAlignment verAlignment, Color ForegroundColor, Font Font, double CellPadding)
		{
			this.colSpan=1;
			this.rowSpan=1;
			this.row=row;
			this.stringFormat="{0}";
			this.transparent=true;
			this.Font=Font;
			this.owner=owner;
			this.column=column;
			this.horizontalAlignment = horAlignment;
			this.verticalAlignment = verAlignment;
			this.foregroundColor=ForegroundColor;
			this.cellPadding=CellPadding;
		}

		#region Internal Properties

		internal double Height
		{
			get {return this.owner.Rows[row].Height;}
		}

		internal PdfDocument PdfDocument
		{
			get {return this.owner.PdfDocument;}
		}

		internal PdfTextArea PdfTextArea
		{
			get {return pcatemp;}
		}

		internal string Text
		{
			get 
			{
				//return this.content!=null ? String.Format(this.stringFormat, this.content) : String.Empty;
				return this.content!=null ? this.content.ToString() : String.Empty;
			}
		}

		internal double Width
		{
			get {return this.owner.Columns[column].CompensatedWidth;}
		}
		
 
		public object Content
		{
			get {return this.content;}
		}
		
		

		

		internal PdfArea Area
		{
			get
			{
				if (_Area==null)
				{
					_Area = this.area.Merge(this.owner.Cell(row+rowSpan-1,column+colSpan-1).area);
				}
				return this._Area;
			}
		}


		internal double NeededHeight
		{
			get
			{
				if (neededHeight==0)
				{
					
					if (this.Content is Byte[])
					{
						try
						{
							byte[] d = (byte[])content;
							Image image = Bitmap.FromStream(new System.IO.MemoryStream(d));
							double imageHeight = image.Height * 0.72 + 2;
							double defaultHeight = this.owner.Rows[row].DefaultHeight;

							image.Dispose();

							if (defaultHeight == 0)
								neededHeight = imageHeight;
							else
								neededHeight = Math.Ceiling(imageHeight /  defaultHeight) * defaultHeight;
						}
						catch (Exception)
						{
							neededHeight = 1;	
						}
						
					}
					else
					{
						double defaultHeight = this.owner.Rows[row].DefaultHeight;
						double textHeight = this.Font.Size * 0.96;
						double spacing = Math.Max(0, defaultHeight - textHeight);

						// Raffale Russo - 17/01/2013 - Start
                        //neededHeight = this.minimumLines * this.Font.Size * 0.96 + Math.Max(0, minimumLines - 1)* spacing + this.cellPadding * 2;
                        neededHeight = this.minimumLines * this.Font.Size * 0.96 + this.cellPadding * 2;
                        // Raffale Russo - 17/01/2013 - End
                    }
				}
				return neededHeight;
			}
		}

		


		internal int minimumLines
		{
			get
			{
				double W=Width;
				if (this.colSpan>1)
				{
					for (int c=column;c<column+colSpan-1;c++)
						W+=this.owner.Cell(row,c).Width;
				}

				PdfArea pa = new PdfArea(this.PdfDocument, 0 , 0, W, 1000);

                // Raffaele Russo - 06/03/2012 - Start
                this.pcatemp = new PdfTextArea(Font, foregroundColor, pa.InnerArea(this.cellPadding*6), horizontalAlignment, verticalAlignment, this.Text);
                //this.pcatemp = new PdfTextArea(Font, foregroundColor, pa.InnerArea(this.cellPadding*2), horizontalAlignment, verticalAlignment, this.Text);
                // Raffaele Russo - 06/03/2012 - End

                return this.pcatemp.RenderLines().Count;
			}
		}

		#endregion
		

		#region Public Methods

		public void SetFont(Font Font)
		{
			this.Font=Font;
		}
		
		public void SetBackgroundColor(Color BackgroundColor)
		{
			this.backgroundColor = BackgroundColor;
			this.transparent = false;
			
		}
		

		public void SetForegroundColor(Color ForegroundColor)
		{
			this.foregroundColor = ForegroundColor;
		}
		
		public void SetContentAlignment(HorizontalAlignment horAlignment, VerticalAlignment verAlignment)
		{
			this.horizontalAlignment = horAlignment;
			this.verticalAlignment = verAlignment;
		}
		
		public void SetColors(Color ForegroundColor, Color BackgroundColor)
		{
			this.SetForegroundColor(ForegroundColor);
			this.SetBackgroundColor(BackgroundColor);
		}
		
		public void SetTransparent()
		{
			this.transparent=true;
		}		

		public void SetCellPadding(double CellPadding)
		{
			if (CellPadding<0) throw new Exception("CellPadding must be non-negative.");
			this.cellPadding = CellPadding;
		}
		
		public void SetContent(object content)
		{
			if (content != null && content is string)
				this.content = content.ToString().Replace("\r", "");
			else
				this.content = content;
		}
		
		public void SetContentFormat(string format)
		{
			this.stringFormat = format;
		}

		#endregion

	}
}
