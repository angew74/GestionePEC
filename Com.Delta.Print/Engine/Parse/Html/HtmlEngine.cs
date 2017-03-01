using System;
using System.Collections;
using System.Drawing;



namespace Com.Delta.Print.Engine.Parse.Html
{
	/// <summary>
	/// Summary description for Engine.
	/// </summary>
	internal class HtmlEngine
	{
		private RichTextField box;
		private Stack stack = new Stack();
		private Context context = new Context("", new Font("Arial", 8), Color.Black, TextField.TextAlignmentType.Left);
		private ArrayList result = new ArrayList();
		private ArrayList line = new ArrayList();

		private Graphics g;
		private StringFormat stringFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
		private RectangleF formatRectangle = new RectangleF(0, 0, 100000 , 100000);
		private Rectangle bounds = new Rectangle();
		private bool finished = false;
		private float height = 0;



		int offset = 0;
		private TextLine previousLine = null;
		private string residualText = string.Empty;

		public HtmlEngine(RichTextField box, Graphics g, Rectangle bounds, Font font, Color color)
		{
			this.box = box;
			this.g = g;
			this.bounds = bounds;
			context = new Context("", font, color, TextField.TextAlignmentType.Left);
		}

		
		internal Context Context
		{
			get {return context;}
		}
		
		internal TextLine Residual
		{
			get {return previousLine;}
		}

		internal string ResidualText
		{
			get {return residualText;}
		}
		


		internal void InitState(HtmlEngine engine, bool designMode)
		{
			if (engine != null)
			{
				if (!designMode)
				{
					this.previousLine = engine.previousLine;
					this.context = engine.context;
					this.stack = engine.stack;
					this.residualText = engine.residualText;
				}
			}
		}


		
		internal void Reset()
		{
			residualText = string.Empty;
			previousLine = null;
		}
		

		internal void SetEngine(Graphics g, Rectangle bounds)
		{
			this.g = g;
			this.bounds = bounds;
		}

		public int Parse(string content, ArrayList lines)
		{

			lines.Clear();
			result = lines;
			HtmlParser parser = new HtmlParser();
			parser.SetChunkHashMode(true);
									
			parser.Init(content);
			parser.SetEncoding(System.Text.Encoding.UTF8);
			

			HTMLchunk chunk = null;
			Start();


			while(!finished && (chunk=parser.ParseNext()) != null)
			{
				switch(chunk.oType)
				{
					case HTMLchunkType.OpenTag:
						HandleTag(chunk);
						break;

					case HTMLchunkType.CloseTag:						
						HandleClosure(chunk.sTag);
						break;
 
					case HTMLchunkType.Text:						
						offset = chunk.iChunkOffset;
						string text = ClearExtraBlanks(chunk.oHTML);
						text = HTMLentities.DecodeEntities(text);
						HandleWords(text);
						break;
				}

			}


			NewLine(true);
			if (!finished)			
			{
				TextLine textLine = new TextLine();

                /* Raffaele Russo - 19/04/2011 - Start - Modificato l'argomento del costruttore TextSegment, il quarto parametro
                 * da "context.Font.Height" è stato modificato in "context.Font.GetHeight(96)" */
                textLine.SetSegments(new ArrayList(line), context.Font.GetHeight(96));
                // Raffaele Russo - 19/04/2011 - End

				textLine.Alignment = context.Alignment;
				if (height + textLine.Height <= bounds.Height)
				{
					result.Add(textLine);
				}
				previousLine = null;
			}


			byte[] b = new byte[parser.iCurPos];
			Array.Copy(parser.bHTML, 0, b, 0, parser.iCurPos);
			int a = System.Text.Encoding.UTF8.GetString(b).Length;

			return a;
		}

		private void HandleTag(HTMLchunk tag)
		{
			if (tag.sTag == "b")
			{
				Font f = new Font(context.Font.FontFamily, context.Font.Size, context.Font.Style | FontStyle.Bold);
				Context c = new Context("b", f, context.Color, context.Alignment);
				stack.Push(context);
				context = c;
			}
			else if (tag.sTag == "i")
			{
				Font f = new Font(context.Font.FontFamily, context.Font.Size, context.Font.Style | FontStyle.Italic);
				Context c = new Context("i", f, context.Color, context.Alignment);
				stack.Push(context);
				context = c;
			}
			else if (tag.sTag == "div")
			{
				if (tag.oParams["align"] != null)
				{
					NewLine(false);

					if(tag.oParams["align"].ToString() == "center")
					{						
						Context c = new Context("div", context.Font, context.Color, TextField.TextAlignmentType.Center);
						stack.Push(context);
						context = c;						
					}
					else if(tag.oParams["align"].ToString() == "right")
					{
						Context c = new Context("div",context.Font, context.Color, TextField.TextAlignmentType.Right);
						stack.Push(context);
						context = c;						
					}
					else if(tag.oParams["align"].ToString() == "justify")
					{
						Context c = new Context("div",context.Font, context.Color, TextField.TextAlignmentType.Justified);
						stack.Push(context);
						context = c;						
					}
					else
					{
						Context c = new Context("div",context.Font, context.Color, TextField.TextAlignmentType.Left);
						stack.Push(context);
						context = c;						
					}
				}
								
			}
			else if (tag.sTag == "font")
			{
				float fontSize = context.Font.Size;
				string fontFamily = context.Font.FontFamily.Name;
				FontStyle fontStyle = context.Font.Style;
				Color color = context.Color;

				if(tag.oParams["size"] != null)
				{
					try
					{
						fontSize = ResolveFontSize(tag.oParams["size"].ToString().Trim());
					}
					catch(Exception){}
				}

				if(tag.oParams["face"] != null)
				{
					try
					{
						fontFamily = tag.oParams["face"].ToString();
					}
					catch(Exception){}
				}

				if(tag.oParams["color"] != null)
				{
					try
					{
						color = ResolveColor("ff" + tag.oParams["color"].ToString().TrimStart('#'));
					}
					catch(Exception){}
				}

				Font f = new Font(fontFamily, fontSize, fontStyle);
				Context c = new Context("font", f, color, context.Alignment);
				stack.Push(context);
				context = c;
			}
			else if (tag.sTag == "br")
			{
				NewLine(true);
			}
			else if (tag.sTag == "p")
			{
				NewLine(true);
			}

		}

		private void HandleClosure(string tag)
		{
			if (context.Tag == tag)
			{
				if (tag == "div")
				{
					NewLine(true);
				}

				if (stack.Count > 0)
				{
					context = (Context)stack.Pop();
				}
			}
				
			
		}


		private void HandleWords(string text)
		{
			string[] words = text.Split(' ');
			for (int i=0;i<words.Length;i++)
			{
				string word = words[i];

				if (word != "")
					HandleText(word);
				else
					HandleText(" ");


				if (i != words.Length - 1 && word!="")
					HandleText(" ");
			}
		}

		private void HandleText(string text)
		{
			if (!finished)
			{
                /* Raffaele Russo - 19/04/2011 - Start - Modificato l'argomento del costruttore TextSegment, il quarto parametro
                 * da "context.Font.Height" è stato modificato in "context.Font.GetHeight(96)" */
                TextSegment segment = new TextSegment(text, context.Color, context.Font, context.Font.GetHeight(96));
                // Raffaele Russo - 19/04/2011 - End                
                
                //Font font = box.PrepareFont(g, context.Font);
				//TextSegment segment = new TextSegment(text, context.Color, font, font.Height);

				float lineWidth = 0;
				for(int i=0;i<line.Count;i++)
				{
					TextSegment seg = line[i] as TextSegment;				
					lineWidth += (float)seg.Width;
				}


				Font measureFont = box.PrepareFont(g, segment.Font);

				if (segment.Text.Trim() == "")
				{
					segment.Width = g.MeasureString(segment.Text, measureFont).Width * 0.6f;
				}
				else
				{
					segment.Width = g.MeasureString(segment.Text, measureFont).Width;
				}


				if (lineWidth + segment.Width > bounds.Width)
				{
					if (lineWidth == 0)
					{
						SplitSegment(segment.Text);
					}
					else
					{
						NewLine(false);
						HandleText(text);
					}
				}
				else
				{
					line.Add(segment);					
				}
			}
			else
			{
				residualText += text;
			}
		}

		

		
		
		private bool NewLine(bool forceCorrection)
		{
			TextLine textLine = new TextLine();

            /* Raffaele Russo - 19/04/2011 - Start - Modificato l'argomento del costruttore TextSegment, il quarto parametro
            * da "context.Font.Height" è stato modificato in "context.Font.GetHeight(96)" */
            textLine.SetSegments(new ArrayList(line), context.Font.GetHeight(96));
            // Raffaele Russo - 19/04/2011 - End
             
			textLine.Alignment = context.Alignment;

			if (height + textLine.Height > bounds.Height)
			{
				if (!finished)
					previousLine = textLine;

				finished = true;				
			}
			else
			{
				height += textLine.Height;

				if (textLine.Alignment == TextField.TextAlignmentType.Justified && forceCorrection)
				{
					textLine.Alignment = TextField.TextAlignmentType.Left;
				}

				result.Add(textLine);
				line.Clear();
			}

			return !finished;
		}

		private void Start()
		{
			if (previousLine != null)
			{
				height += previousLine.Height;
				result.Add(previousLine);
			}

			if (residualText != string.Empty)
			{
				string leftover = residualText;
				residualText = String.Empty;
				HandleWords(leftover);
				
			}
		}


		private void SplitSegment(string text)
		{
			if (!finished)
			{
				float width = 0;
				int index = -1;
				for (int i=0;i<text.Length;i++)
				{
					index = i;
					width = g.MeasureString(text.Substring(0, i), context.Font).Width;
					if (width > bounds.Width)
					{
						break;
					}
				}

				if (index > 0)
				{
					HandleText(text.Substring(0,index - 1));
					NewLine(false);
					HandleText(text.Substring(Math.Max(0, index - 1)));
				}
			}
		}

		private Color ResolveColor(string text)
		{
			Color c = Color.FromName(text);

			if (!c.IsKnownColor)
			{
				try
				{
					int argbValue = Convert.ToInt32(text, 16);
					return Color.FromArgb(argbValue);
				}
				catch (Exception)
				{
					return Color.Red;
				}
			}
			else
			{
				return c;
			}
		}

		private string ClearExtraBlanks(string text)
		{
			int position = 0;
			char previous = (char)0;
			string buffer = string.Empty;
			text = text.Replace("\r"," ").Replace("\t"," ").Replace("\n"," ");

			for (int i=0;i<text.Length;i++)
			{
				if (text[i] != 32 || previous != 32)
				{
					buffer += text[i];
				}
				previous = text[i];
			}

			return buffer;
		}

		private float CalculateStringWidth(Graphics g, string text, Font font)
		{

			if (text == null || text.Length==0) return 0;
			
			float measuredWidth = 0;		
			int textLength = text.Length;
		
			lock(g)
			{					
				CharacterRange[] characterRanges = {new CharacterRange(0, textLength)};
				stringFormat.SetMeasurableCharacterRanges(characterRanges);
			
				Region[] reg = new Region[1];
				reg = g.MeasureCharacterRanges(text, font, formatRectangle, stringFormat);
				measuredWidth = reg[0].GetBounds(g).Right;				
			}

			return measuredWidth;
			
		}


		private float ResolveFontSize(string text)
		{
			string s = string.Empty;
			int type = 0;
			if (text.EndsWith("pt"))
			{
				s = text.Substring(0, text.Length - 2);
				type = 1;
			}
			else if (text.EndsWith("px"))
			{
				s = text.Substring(0, text.Length - 2);
				type = 2;
			}
			else
			{
				s = text;
			}

			try
			{
				float f = Single.Parse(s);

				if (type == 1)
					return f;
				else if (type == 2)
					return f * 72f / 96.0f;
				else
				{
					int w = (int)f;
					switch(w)
					{
						case 1:
							return 3;
						case 2:
							return 6;
						case 3:
							return 8;
						case 4:
							return 12;
						case 5:
							return 18;
						case 6:
							return 24;
						case 7:
							return 32;
						default:
							return 8;

					}
				}
			}
			catch(Exception) {return 8;}

		}

	}

	internal class Context
	{
		private Font font;
		private Color color = Color.Black;
		private TextField.TextAlignmentType alignment = Com.Delta.Print.Engine.TextField.TextAlignmentType.Left;
		private string tag = string.Empty;


		public Context(string tag, Font font, Color color, TextField.TextAlignmentType alignment)
		{
			this.tag = tag;
			this.font = font;
			this.color = color;
			this.alignment = alignment;
		}

		public Color Color
		{
			get {return color;}
		}

		public Font Font 
		{
			get {return font;}
		}

		public TextField.TextAlignmentType Alignment 
		{
			get {return alignment;}
		}

		public string Tag
		{
			get {return tag;}
		}
	}
}
