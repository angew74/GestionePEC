
using System;
using System.Drawing;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Xml;


namespace Com.Delta.Print.Engine
{
	/// <summary>
	/// Report element for displaying images.
	/// </summary>
	/// <remarks>The PictureBox is used in reports where images need to be displayed.
	/// </remarks>
	[DefaultProperty("ImageFile")]
	public sealed class PictureBox : ICustomPaint
	{
		#region Declarations

		private Bitmap mImage;
		private bool mDoStretch = false;
		private Color mBorderColor = Color.Black;
		private int mBorderWidth = 1;
		private int pdfImageQuality = 80;
		private string mFilename;
		private ImageAttributes transparencyAttributes = new ImageAttributes();

		#endregion

		#region Public Functions

		/// <summary>
		/// Gets the string representation of report element.
		/// </summary>
		public override string ToString()
		{
			string theText = this.Name;
			theText += this.ImageFile==null ? "" : (theText==String.Empty ? this.ImageFile : " :: "+ this.ImageFile); 
			return "Picture [" + theText + "]" ;			
		}

		/// <summary>
		/// Used to reload image file.
		/// </summary>
		internal void Reload()
		{
			if (mFilename != null)
				Load(mFilename);	
		}

		internal void SetPicture(Bitmap image)
		{
			mImage = image;
		}
		
		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the PDF export quality for the element.
		/// </summary>
		/// <remarks>This property sets the PDF export quality of the object.
		/// </remarks>
		[Category("Pdf"), DefaultValue(80), Description("Display quality when exporting to PDF format (0-100).")]
		public int ExportQuality
		{
			get {return pdfImageQuality;}
			set {pdfImageQuality = Math.Min(100, Math.Max(0,value));}
		}

		/// <summary>
		/// Gets or sets the border color for the PictureBox element.
		/// </summary>
		/// <remarks>This property sets the border color of the PictureBox object. This can be any color
		/// from the System.Drawing.Color structure
		/// </remarks>
		[Category("Picture"), DefaultValue(typeof(System.Drawing.Color),"Black"), Description("Border color for picture frame.")]
		public Color BorderColor
		{
			get {return mBorderColor;}
			set {mBorderColor = value;}
		}


		/// <summary>
		/// Gets or sets the border width for the PictureBox element.
		/// </summary>
		/// <remarks>
		/// BorderWidth of the PictureBox. If this is set to zero, then the border is invisible
		/// </remarks>
		[Category("Picture"), DefaultValue(1), Description("Border thickness for picture frame.")]
		public int BorderWidth
		{
			get {return mBorderWidth;}
			set {mBorderWidth = Math.Max(0,value);}
		}


		/// <summary>
		/// Location of image file relative to xml template file.
		/// </summary>
		/// <remarks>For new documents defaults to working directory of Designer.</remarks>
		[Editor(typeof(Com.Delta.Print.Engine.Editors.ImageFileEditor), typeof(UITypeEditor)),Category("Picture"), Description("Image file location relative to .xml file. For new documents defaults to working directory of Designer.")]
		public string ImageFile
		{
			get {return mFilename;}
			set
			{
				Load(value);
			}
		}

		/// <summary>
		/// The PictureBox image.
		/// </summary>
		[Browsable(false)]
		internal Bitmap Image
		{
			get {return mImage;}
		}

		/// <summary>
		/// Actual physical size of the loaded picture.
		/// </summary>
		[Category("Picture"), Description("Actual physical size of the loaded picture.")]
		public Size ImageSize
		{
			get
			{
				if (mImage != null)
					return mImage.Size;
				else
					return new Size(0,0);
			}
		}
		

		/// <summary>
		/// Stretch the image to the container size.
		/// </summary>
		/// <remarks>
		/// If set to true, the image to the container size, otherwise will be displayed as it's physical size.
		/// </remarks>
		[Category("Picture"), DefaultValue(false), Description("Stretch the image to the container size.")]
		public bool Stretch
		{
			get {return mDoStretch;}
			set {mDoStretch = value;}
		}

		/// <summary>
		/// Gets or sets the name of the PictureBox element.
		/// </summary>
		/// <remarks>
		/// This property is used when setting image dynamicly.
		/// </remarks>
		[Category("Picture"), Description("Gets or sets the name of the PictureBox element.")]
		public override string Name
		{
			get {return name;}
			set {name = value;}
		}


		#endregion

		#region Private Functions

		private void Load(string filename)
		{
			try
			{
				string imageFile = section.Document.DocRoot;
				if (filename.StartsWith(Path.DirectorySeparatorChar.ToString()))
					imageFile += filename;
				else
					imageFile += Path.DirectorySeparatorChar.ToString() + filename;

				mImage = new Bitmap(imageFile);
				mFilename = filename;
				
			}
			catch(Exception)
			{
				mImage = null;
				mFilename = filename;
			}
		}

        // Raffaele Russo - 07/03/2012 - Start - Metodo aggiunto per poter mandare in input l'url dell'immagine proveniente dal pru
        private string ResolveParameterValues(string input)
        {
            string buffer = "";
            int pos = -1;
            int oldPos = 0;

            while ((pos = input.IndexOf("$P", oldPos)) != -1)
            {

                buffer += input.Substring(oldPos, pos - oldPos);
                if (input.Substring(pos + 2, 1).Equals("{") && input.IndexOf("}", pos + 2) != -1)
                {
                    string parameterName = input.Substring(pos + 3, input.IndexOf("}", pos + 2) - pos - 3).Trim();
                    int parPosition = buffer.Length;
                    buffer += this.section.GetParameterValue(parameterName);

                    oldPos = input.IndexOf("}", pos + 2) + 1;
                }
                else
                {
                    oldPos = pos + 2;
                }
            }

            buffer += input.Substring(oldPos);

            return buffer;
        }
        // Raffaele Russo - 07/03/2012 - End


		#endregion

		#region Creator and Destructor

		/// <summary>
		/// Initializes a new instance of the PictureBox class.
		/// </summary>
		/// <param name="x">x-position of the new PictureBox</param>
		/// <param name="y">y-position of the new PictureBox</param>
		/// <param name="width">Width of the new PictureBox</param>
		/// <param name="height">Height of the new PictureBox</param>
		/// <param name="parent">Parent of the new PictureBox</param>
		public PictureBox(int x,int y, int width, int height, Section parent)
		{
			section = parent;
			theRegion = new Rectangle(x,y,width,height);
            SetTransparencyAttributes();
		}

		internal PictureBox(XmlNode node, Section parent)
		{
			section = parent;
			Init(node);
			SetTransparencyAttributes();			
		}

		private void SetTransparencyAttributes()
		{
			float[][] matrixItems = { 
										new float[] {1, 0, 0, 0, 0},
										new float[] {0, 1, 0, 0, 0},
										new float[] {0, 0, 1, 0, 0},
										new float[] {0, 0, 0, 0.9f, 0}, 
										new float[] {0, 0, 0, 0, 1}}; 

			ColorMatrix colorMatrix = new ColorMatrix(matrixItems);
			transparencyAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
		}


		/// <summary>
		/// Releases all resources used by this PictureBox object.
		/// </summary>
		internal void Dispose()
		{
			if (mImage!=null)
				mImage.Dispose();
		}


		#endregion
        
        #region ICustomPaint Members

		/// <summary>
		/// Paints the PictureBox
		/// </summary>
		/// <param name="g">The Graphics object to draw</param>
		/// <remarks>Causes the PictureBox to be painted to the screen.</remarks>
		public override void Paint(Graphics g)
		{
            // Raffaele Russo - 12/12/2011 - Start
            Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : new Rectangle(Bounds.X - rBounds.X, Bounds.Y - rBounds.Y, Bounds.Width, Bounds.Height);
            // Rectangle paintArea = new Rectangle(theRegion.X - rBounds.X, theRegion.Y - rBounds.Y, theRegion.Width, theRegion.Height);
            // Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;
            // Raffaele Russo - 12/12/2011 - End

            // Raffaele Russo - 07/03/2012 - Start
            if (mImage == null)
            {
                string url = this.ResolveParameterValues(mFilename);
                
                int lio = url.LastIndexOf('/');
                string soloFilename;
                if (lio>0)
                    soloFilename = System.IO.Path.GetFileName(url.Substring(lio + 1));
                else
                    soloFilename = System.IO.Path.GetFileName(url);

                System.Net.WebClient client = new System.Net.WebClient();
                
                try
                {
                    string filename = section.Document.DocRoot + Path.DirectorySeparatorChar.ToString() + soloFilename;
                    if (!System.IO.File.Exists(filename))
                    {
                        client.DownloadFile(url,filename);
                    }

                    mImage = new Bitmap(filename);
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    client.Dispose();
                }
            }
            // Raffaele Russo - 07/03/2012 - End
            
            if (mImage != null)
			{
				
				if (mDoStretch)
				{
					if (paintTransparent)
						g.DrawImage(mImage,paintArea,0,0,mImage.Width,mImage.Height,GraphicsUnit.Pixel, transparencyAttributes);
					else
						g.DrawImage(mImage,paintArea,0,0,mImage.Width,mImage.Height,GraphicsUnit.Pixel);
				}
				else
				{
					if (paintTransparent)
						g.DrawImage(mImage,paintArea,0,0,paintArea.Width,paintArea.Height,GraphicsUnit.Pixel, transparencyAttributes);
					else
						g.DrawImage(mImage,paintArea,0,0,paintArea.Width,paintArea.Height,GraphicsUnit.Pixel);
				}

				if ( mBorderWidth > 0 )
				{
					g.DrawRectangle( new Pen(mBorderColor,mBorderWidth), paintArea);
				}
			}
            else
            {
                if (section.Document.DesignMode)
                {
                    g.FillRectangle(new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.Percent05, Color.Gray, Color.Transparent), paintArea);

                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    g.DrawString("Set your picture through ImageFile property.", new Font("Tahoma", 8 * g.PageScale), Brushes.Black, theRegion, sf);
                }
            }
		}

		


		/// <summary>
		/// Clones the structure of the PictureBox, including all properties
		/// </summary>
		/// <returns><see cref="Com.Delta.Print.Engine.PictureBox">Com.Delta.Print.Engine.PictureBox</see></returns>
		public override object Clone()
		{
			PictureBox tmp = new PictureBox(0,0,0,0,section);
			tmp.X = this.X;
			tmp.Y = this.Y;
			tmp.Width = this.Width;
			tmp.Height = this.Height;
			tmp.Layout = this.Layout;
			tmp.BorderWidth = this.BorderWidth;
			tmp.BorderColor = this.BorderColor;
			tmp.Stretch = this.Stretch;
			tmp.ImageFile = this.ImageFile;
			tmp.ExportQuality = this.ExportQuality;
			return tmp;
		}


		#endregion

	}
}
