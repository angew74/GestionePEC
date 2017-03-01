using System;
using System.Collections;
using System.Drawing;

namespace Com.Delta.Print.Engine
{
	/// <summary>
	/// Summary description for ImageSerializer.
	/// </summary>
	internal class ImageSerializer
	{
		public ImageSerializer()
		{
			//
			// TODO: Add constructor logic here
			//
		}


		public Image[] Serialize(ReportDocument document)
		{
			ArrayList images = new ArrayList();

			Size documentSize = document.PaperType == Paper.Type.Custom ? document.PixelSize : Paper.GetPaperSize(document.PaperType);
			if (document.Layout == Com.Delta.Print.Engine.ReportDocument.LayoutType.Landscape)
				documentSize = new Size(documentSize.Height, documentSize.Width);
			
			document.StartPrinting();

			for (int sectionCounter=0;sectionCounter<document.Sections.Count;sectionCounter++)
			{
				document.NewPage();
				Section section = (Section)document.Sections[sectionCounter];
				section.Prepare(true);
				
				Image image = new Bitmap(documentSize.Width, documentSize.Height);

								
				bool morePages = false;
				int sectionPage = 0;
				do
				{
					Graphics g = Graphics.FromImage(image);
					g.Clear(Color.White);
					sectionPage++;
					morePages = section.UpdateDynamicContent();
					for (int i=0;i<section.Objects.Length;i++)
					{
						
						if (section.Objects[i].Layout == ICustomPaint.LayoutTypes.EveryPage)
						{
							ProcessElement(g, section.Objects[i]);
						}
						else if (section.Objects[i].Layout == ICustomPaint.LayoutTypes.FirstPage)
						{
							if (sectionPage == 1)
							{								
								ProcessElement(g, section.Objects[i]);
							}
						}
						else if (section.Objects[i].Layout == ICustomPaint.LayoutTypes.LastPage)
						{
							if (!morePages)
							{								
								ProcessElement(g, section.Objects[i]);
							}
						}
				
					}

					if (morePages)
					{
						images.Add(image);
						image = new Bitmap(documentSize.Width, documentSize.Height);
						
						document.NewPage();
						section.Prepare(false);
					}

				}
				while(morePages);

				if (image!=null)
				{
					images.Add(image);
					image = new Bitmap(documentSize.Width, documentSize.Height);
				}

				document.NewSection();
			}

		

			

			return (Image[])images.ToArray(typeof(Image));
			
		}

		private void ProcessElement(Graphics g, Com.Delta.Print.Engine.ICustomPaint element)
		{
			if (element.Anchored)
			{
				if (element.Ready && !element.Displayed)
				{													
					element.Paint(g);
					if (element.Done)
						element.Displayed = true;
								
				}
			}
			else
			{
				element.Paint(g);
			}
		}
	}
}
