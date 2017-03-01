
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace Com.Delta.Print.Engine.Pdf
{
	/// <summary>
	/// a 72dpi jpeg based Image for a PdfPage
	/// </summary>
	public class PdfImage : PdfObject
	{
		
		string file;
		internal Bitmap bmp;
		private double width = 0;
		private double height = 0;
		internal PdfImageMask mask = null;
		private bool interpolate = true;
		private long quality = 85;

		/// <summary>
		/// gets the height of the loaded picture
		/// </summary>
		public double Height
		{
			get {return height;}
			set {height = value;}
		}
		/// <summary>
		/// gets the width of the loaded picture
		/// </summary>
		public double Width
		{
			get {return width;}
			set {width = value;}
		}

		/// <summary>
		/// gets the PDF export quality
		/// </summary>
		public long Quality
		{
			get {return quality;}
			set {quality = value;}
		}

		public bool Transparent
		{
			get {return mask==null ? false : mask.Transparent;}
		}

		internal bool Interpolate
		{
			get {return interpolate;}
			set {interpolate = value;}
		}
		
		internal PdfImage(int id, string file)
		{
			this.id=id;
			this.file=file;

			try
			{
				this.bmp = new Bitmap(file);
				this.width = bmp.Width;
				this.height = bmp.Height;
				this.mask = new PdfImageMask(bmp);
			}
			catch
			{
				throw new Exception("Error Loading Image File");
			}
		}

		internal PdfImage(int id, string file, double width, double height) : this(id, file)
		{
			this.width = width;
			this.height = height;
		}

		internal PdfImage(int id, Bitmap bitmap):this(id, bitmap, true)
		{
			
		}

		internal PdfImage(int id, Bitmap bitmap, bool transparent)
		{
			this.id=id;
			this.bmp = bitmap;
			this.width = bmp.Width;
			this.height = bmp.Height;

			if (transparent)
				this.mask = new PdfImageMask(bmp);
		}
		



		internal override int StreamWrite(System.IO.Stream stream)
		{
			if (bmp!=null)
			{
				return this.StreamBitmap(stream, bmp);
			}
			else
			{
			
				FileStream fs;
				try
				{
					fs = File.OpenRead(this.file);
				}
				catch {throw new Exception("Can't open image file");}
				byte[] data = new byte[fs.Length];
				
				Bitmap bitmap = (Bitmap)Bitmap.FromFile(file);
				return this.StreamBitmap(stream, bitmap);
				

				
			}
		}


		private int StreamBitmap(System.IO.Stream stream, Bitmap bitmap)
		{
			MemoryStream imageStream = new MemoryStream();
			
			ImageCodecInfo codecInfo = this.GetEncoderInfo("image/jpeg");

			EncoderParameter ratio = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, this.quality);

			// Add the quality parameter to the list
			EncoderParameters codecParams = new EncoderParameters(1);
			codecParams.Param[0] = ratio;

 
			bitmap.Save(imageStream, codecInfo, codecParams); 

			byte[] data = imageStream.ToArray();


			PdfWriter wr = new PdfWriter(this.id);			
			wr.AddHeader("/Type/XObject/Subtype/Image/BitsPerComponent 8/ColorSpace/DeviceRGB");
			wr.AddHeader("/Name/I"+this.ID).AddHeader("/Filter[/DCTDecode]");
			wr.AddHeader("/Width " + bitmap.Width.ToString() + "/Height " + bitmap.Height.ToString());

			if (interpolate)
				wr.AddHeader("/Interpolate true");
			if (this.Transparent)
				wr.AddHeader("/Mask " + mask.HeadR);

			wr.AddStreamContent(data);
			return wr.Write(stream);



		}

		private ImageCodecInfo GetEncoderInfo(String mimeType)
		{
			ImageCodecInfo[] encoders;
			encoders = ImageCodecInfo.GetImageEncoders();
			for(int j = 0; j < encoders.Length; ++j)
			{
				if(encoders[j].MimeType == mimeType)
					return encoders[j];
			} 
			return null;
		}


	}
}
