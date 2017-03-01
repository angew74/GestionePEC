
using System;
using System.Drawing;
using System.Text;

namespace Com.Delta.Print.Engine.Pdf
{
	/// <summary>
	/// a circle to put inside a PdfPage
	/// </summary>
	internal class PdfCircle : PdfObject
	{
		private PdfArea axesArea;
		/// <summary>
		/// gets or sets the Area of defined by the horizontal and vertical axes
		/// </summary>
		public PdfArea AxesArea
		{
			get {return this.axesArea;}
			set {this.axesArea=value;}
		}
		internal double strokeWidth;		
		internal Color borderColor;
		internal Color backColor = Color.White;
		internal bool filled = false;
		internal int gradientType = 0;

		/// <summary>
		/// gets the center coordinates
		/// </summary>
		public System.Drawing.PointF Center
		{
			get
			{
				return new PointF((float)(this.axesArea.PosX+(this.axesArea.Width/2)), (float)(this.axesArea.PosY+(this.axesArea.Height/2)));
			}
		}


		internal PdfCircle()
		{
			this.strokeWidth = 1;
		}

		/// <summary>
		/// creates a new circle
		/// </summary>
		/// <param name="posx">center's x coordinate</param>
		/// <param name="posy">center's y coordinate</param>
		/// <param name="ray">ray measure</param>
		/// <param name="Color">circumference color</param>
		public PdfCircle(double posx, double posy, Color backColor, Color borderColor, double borderWidth)
		{

			this.axesArea = new PdfArea(this.PdfDocument, posx, posy, 200, 200);

			this.borderColor = borderColor;
			this.backColor = backColor;
			if (backColor != Color.Transparent)
				this.filled = true;

			this.strokeWidth = borderWidth;
		}




		/// <summary>
		/// sets the diameters lenght of the circle.
		/// </summary>
		/// <param name="XDiameter"></param>
		/// <param name="YDiameter"></param>
		public void SetDiameters(double XDiameter,double YDiameter)
		{
			if (XDiameter<=0) throw new Exception("XDiameter must be grater than zero.");
			if (YDiameter<=0) throw new Exception("YDiameter must be grater than zero.");

			this.axesArea = new PdfArea(this.PdfDocument, this.axesArea.CenterX-XDiameter/2, this.axesArea.CenterY-YDiameter/2, XDiameter, YDiameter);
		}



		internal string ToLineStream()
		{

			if (gradientType == 0)
			{
				string text="";
			
				text+= Utility.ColorRGLine(this.borderColor);

				if (filled) 
					text += Utility.ColorrgLine(this.backColor);
			
				text+=this.strokeWidth.ToString("0.##")+" ";
				text+="w\n";
			
				text+=this.Center.X.ToString("0.##")+" ";
				text+=(this.PdfDocument.PH-this.axesArea.PosY).ToString("0.##")+" m\n";

				text+=(this.axesArea.BottomRightCornerX+this.axesArea.Width/6).ToString("0.##")+" ";
				text+=(this.PdfDocument.PH-this.axesArea.PosY).ToString("0.##")+" ";
				text+=(this.axesArea.BottomRightCornerX+this.axesArea.Width/6).ToString("0.##")+" ";
				text+=(this.PdfDocument.PH-this.axesArea.BottomRightCornerY).ToString("0.##")+" ";
				text+=this.Center.X.ToString("0.##")+" ";
				text+=(this.PdfDocument.PH-this.axesArea.BottomRightCornerY).ToString("0.##")+" c \n";
			
				text+=(this.axesArea.PosX-this.axesArea.Width/6).ToString("0.##")+" ";
				text+=(this.PdfDocument.PH-this.axesArea.BottomRightCornerY).ToString("0.##")+" ";
				text+=(this.axesArea.PosX-this.axesArea.Width/6).ToString("0.##")+" ";
				text+=(this.PdfDocument.PH-this.axesArea.PosY).ToString("0.##")+" ";
				text+=this.Center.X.ToString("0.##")+" ";
				text+=(this.PdfDocument.PH-this.axesArea.PosY).ToString("0.##")+" c\n";

				if (strokeWidth == 0)
				{
					if (filled) 
						text+="f";
					else 
						text+="s";
				}
				else
				{
					if (filled)
						text+="B";
					else 
						text+="s";
				}


				text+="\n";
		
				text=text.Replace(",",".");
				return text;
			}
			else
			{
				StringBuilder sb = new StringBuilder();

				sb.Append(Utility.ColorRGLine(this.borderColor));
				sb.Append(this.strokeWidth.ToString("0.##") + " ");
				sb.Append("w\n");

				//sb.Append("/Pattern cs\n");
				//sb.Append("0.2 0.8 0.4 /P1 scn\n");

				sb.Append(this.Center.X.ToString("0.##")+" ");
				sb.Append((this.PdfDocument.PH-this.axesArea.PosY).ToString("0.##")+" m\n");

				sb.Append((this.axesArea.BottomRightCornerX+this.axesArea.Width/6).ToString("0.##")+" ");
				sb.Append((this.PdfDocument.PH-this.axesArea.PosY).ToString("0.##")+" ");
				sb.Append((this.axesArea.BottomRightCornerX+this.axesArea.Width/6).ToString("0.##")+" ");
				sb.Append((this.PdfDocument.PH-this.axesArea.BottomRightCornerY).ToString("0.##")+" ");
				sb.Append(this.Center.X.ToString("0.##")+" ");
				sb.Append((this.PdfDocument.PH-this.axesArea.BottomRightCornerY).ToString("0.##")+" c \n");
			
				sb.Append((this.axesArea.PosX-this.axesArea.Width/6).ToString("0.##")+" ");
				sb.Append((this.PdfDocument.PH-this.axesArea.BottomRightCornerY).ToString("0.##")+" ");
				sb.Append((this.axesArea.PosX-this.axesArea.Width/6).ToString("0.##")+" ");
				sb.Append((this.PdfDocument.PH-this.axesArea.PosY).ToString("0.##")+" ");
				sb.Append(this.Center.X.ToString("0.##")+" ");
				sb.Append((this.PdfDocument.PH-this.axesArea.PosY).ToString("0.##")+" c\n");


				if (strokeWidth == 0)
				{
					if (filled) 
						sb.Append("f");
					else 
						sb.Append("s");
				}
				else
				{
					if (filled)
						sb.Append("B");
					else 
						sb.Append("s");
				}

				//sb.Append("\n");
				return sb.ToString();
			}
		}


		internal override int StreamWrite(System.IO.Stream stream)
		{
			PdfWriter wr = new PdfWriter(this.id);
			wr.AddStreamText(this.ToLineStream());
			return wr.Write(stream);
		}

	}
}
