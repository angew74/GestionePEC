using System;
using System.Drawing;
using System.Text;
using System.Collections;

namespace Com.Delta.Print.Engine.Pdf
{
	/// <summary>
	/// Summary description for PdfImageMssk.
	/// </summary>
	internal class PdfImageMask : PdfObject
	{
		private bool transparent = false;
		private Bitmap bmp;
		private ArrayList bitMask = new ArrayList();


		public PdfImageMask(Bitmap b)
		{
			bmp = b;
			Process();
		}

		private void Process()
		{
			bitMask.Clear();

			for (int i=0;i<bmp.Height;i++)
			{
				for (int j=0;j<bmp.Width;j++)
				{
					
					Color c = bmp.GetPixel(j,i);
					if (c == Color.Transparent || c.A == 0)
					{
						bitMask.Add(0);
						transparent = true;
					}
					else
					{
						bitMask.Add(1);
					}

				}
			}
		}

		internal bool Transparent
		{
			get {return transparent;}
		}

		internal override int StreamWrite(System.IO.Stream stream)
		{
			int imageWidth = bmp.Width;
			int imageHeight = bmp.Height;

			int dataLength = imageHeight * imageHeight;

			int maskWidth = 8 * imageWidth;
			int maskHeight = 8 * imageHeight;



			int rawDataLength = dataLength*64;
			int[] t = new int[maskWidth*maskHeight];

			byte[] maskData = new byte[maskWidth*maskHeight/8];

			
			for (int i=0;i<maskHeight;i++)
			{
				for (int j=0;j<maskWidth;j++)
				{	
					t[i*maskWidth+j] = Convert.ToByte(bitMask[i/8 * imageWidth + j/8]);
				}
			}

			for (int i=0;i<t.Length;i+=8)
			{
				byte q = (byte) ((int)t[i]*128 + (int)t[i+1]*64 + (int)t[i+2]*32 + (int)t[i+3]*16 + (int)t[i+4]*8 + (int)t[i+5]*4 + (int)t[i+6]*2 + (int)t[i+7]);
				maskData[i/8] = q;
			}

			Byte[] part2 = CCITTG4Encoder.Compress(maskData, maskWidth, maskHeight);


			StringBuilder sb = new StringBuilder();
			sb.Append(this.HeadObj);
			sb.Append("<</Type/XObject\n");
			sb.Append("/Subtype/Image\n");
			sb.Append("/Width " + maskWidth.ToString() + "\n/Height " + maskHeight.ToString()+"\n");
			sb.Append("/BitsPerComponent 1\n");
			sb.Append("/ImageMask true\n");
			sb.Append("/Decode [1 0]\n");

			sb.Append("/Filter /CCITTFaxDecode\n");
			sb.Append("/Length " + part2.Length.ToString() + "\n");
			sb.Append("/DecodeParms <</K -1 /BlackIs1 true /Columns " + maskWidth + " /Rows " + maskHeight + ">>\n");	

			sb.Append(">>\nstream\n");



			string text3="";
			text3+="\nendstream\n";
			text3+="endobj\n";

			Byte[] part1 = ASCIIEncoding.ASCII.GetBytes(sb.ToString());
			Byte[] part3 = ASCIIEncoding.ASCII.GetBytes(text3);

			
			stream.Write(part1,0, part1.Length);
			stream.Write(part2,0, part2.Length);
			stream.Write(part3,0, part3.Length);

			return part1.Length + part2.Length + part3.Length;
		}



	}
}
