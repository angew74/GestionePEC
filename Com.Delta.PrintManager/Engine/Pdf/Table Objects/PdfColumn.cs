
using System;
using System.Drawing;
using System.Collections;

namespace Com.Delta.PrintManager.Engine.Pdf
{
	/// <summary>
	/// a Column of a PdfTable
	/// </summary>
	public class PdfColumn : PdfCellRange
	{
		internal int index;
		private double width;
		private double compansatedWidth = -1;

		

		internal PdfColumn(PdfTable owner, int index)
		{
			this.owner=owner;
			this.index=index;
			this.startColumn=index;
			this.endColumn=index;
			this.startRow=0;
			this.endRow=this.owner.rows-1;
		}

		internal double Width
		{
			get {return width;}
			set {width = value;}
		}

		internal double CompensatedWidth
		{
			get
			{
				
				if (this.compansatedWidth==-1)
				{
					
					double sum = 0;
					foreach (PdfColumn pc in this.owner.Columns)
					{
						sum += pc.Width;
					}
					this.compansatedWidth = (this.owner.TableArea.width/sum)*width;
				}
				return this.compansatedWidth;
								
			}

			set {this.compansatedWidth = value;}
		}
		

		public void SetWidth(int relativeWidth)
		{
			if (relativeWidth<=0) throw new Exception("RelativeWidth must be grater than zero.");
			width = relativeWidth;

			if (this.owner.header!=null) 
				this.owner.header.Columns[index].SetWidth(relativeWidth);
		}
		
	}
}
