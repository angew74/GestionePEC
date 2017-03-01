
using System;
using System.Text;

namespace Com.Delta.Print.Engine.Pdf
{
	internal class PdfImageContent : PdfObject
	{
		private string name,imagename;
		private int imageid;
		private double width,height;
		private double displayWidth, displayHeight;


		double posx,posy;
		internal string Name
		{
			get
			{
				return this.name;
			}
		}
		internal PdfImageContent(int id,string imagename,int imageid, double width, double height,double posx,double posy, double dispWidth, double dispHeight)
		{
			this.name=name;
			this.imagename=imagename;
			this.imageid=imageid;
			this.id=id;
			this.height=height;
			this.width=width;
			this.posx=posx;
			this.posy = posy - this.height;
			
			this.displayWidth = dispWidth;
			this.displayHeight = dispHeight;
			
		}

		internal override int StreamWrite(System.IO.Stream stream)
		{
			
			string s="";
			s+="q\n";
			
			s += posx.ToString("0.##") + " " + (posy+height-displayHeight).ToString("0.##") + " " + displayWidth.ToString("0.##") + " " + displayHeight.ToString("0.##") + " re W n \n";

			s+=width.ToString()+" 0 0 ";			
			s+=height.ToString()+" "+posx.ToString("0.##")+" "+posy.ToString("0.##")+" cm\n";

			s+="/"+this.imagename+" Do\n";
			s+="Q\n";
				
			s=s.Replace(",",".");

			Byte[] b=ASCIIEncoding.ASCII.GetBytes(s);
				
			string r=this.HeadObj;
			r+="<< /Lenght "+b.Length.ToString()+">>\n";
			r+="stream\n";
			r+=s;

			r+="endstream\n";
			r+="endobj\n";
			Byte[] b2=ASCIIEncoding.ASCII.GetBytes(r);
			stream.Write(b2,0,b2.Length);
			return b2.Length;
		}

	}
	
}