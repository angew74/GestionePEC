using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Xml;
using System.Data;
using System.Globalization;

using System.Drawing;

namespace Com.Delta.PrintManager.Engine
{
	/// <summary>
	/// Section is report segment that contains one or more printing pages.
	/// </summary>
	public sealed class Section
	{

		#region Declarations

		private ReportDocument document;
		private ICustomPaint[] objects = new ICustomPaint[0];

		private string name = string.Empty;

		private Hashtable parameterValues = new Hashtable();
		private Hashtable thePictures = new Hashtable();
		private Hashtable theTimelines = new Hashtable();
		private Hashtable theBarcodes = new Hashtable();
		private Hashtable theTables = new Hashtable();		
		private Hashtable theTablesByName = new Hashtable();
		private Hashtable theColumns = new Hashtable();
		private Hashtable theCharts = new Hashtable();
		private Hashtable theMaps = new Hashtable();
		private Hashtable theScatters = new Hashtable();
		private Hashtable theTextFields = new Hashtable();
		private Hashtable theRichTextFields = new Hashtable();
		private Hashtable theUserPaints = new Hashtable();

		private DateTimeFormatInfo usFormat = new CultureInfo("en-US", false).DateTimeFormat;


		/// <summary>
		/// List of report elements in flow layout mode
		/// </summary>
		public ArrayList Chain = new ArrayList();

		#endregion

		#region Constructors

		/// <summary>
		/// Creates new instance of section.
		/// </summary>
		public Section(ReportDocument document)
		{
			this.document = document;
		}

		/// <summary>
		/// Creates new instance of section.
		/// </summary>
		internal Section(ReportDocument document, XmlNode node):this(document)
		{

			if (node.Attributes["name"] != null)
				this.name = node.Attributes["name"].Value;

			if (!document.Sections.Contains(this))
			{
				document.Sections.Add(this);
			}

			for (int i=0;i<node.ChildNodes.Count;i++)
			{
				
				string nodeName = node.ChildNodes[i].Name;
				if (nodeName == "staticContent" || nodeName == "dynamicContent" || nodeName == "content")
					InitObjects(node.ChildNodes[i].ChildNodes);
				else if (nodeName == "flow")
					resolveFlow(node.ChildNodes[i]);

			}

			ArrayList t = new ArrayList();
			foreach(int index in Chain)
			{
				if (index >=0 && index<objects.Length)
					t.Add(objects[index]);
			}
			Chain = t;

		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the parent Report Document of the section.
		/// </summary>
		[Browsable(false)]
		public ReportDocument Document
		{
			get {return document;}
		}


		/// <summary>
		/// Gets or sets the name of the section.
		/// </summary>
		/// <remarks>
		/// This property is used when cloning section dynamicly.
		/// </remarks>
		[Description("Gets or sets the name of the section.")]
		public string Name
		{
			get {return name;}
			set {name = value;}
		}


		/// <summary>
		/// Gets a collection of <see cref="Com.Delta.PrintManager.Engine.ICustomPaint">Com.Delta.PrintManager.Engine.ICustomPaint</see> objects.
		/// </summary>
		[Browsable(false)]
		public ICustomPaint[] Objects
		{
			get {return objects;}
			set {objects = value;}
		}

		
		/// <summary>
		/// Gets a collection of <see cref="Com.Delta.PrintManager.Engine.ICustomPaint">Com.Delta.PrintManager.Engine.ICustomPaint</see> objects
		/// which display dynamic data.
		/// </summary>
		[Browsable(false)]
		public ICustomPaint[] DynamicObjects
		{
			get 
			{
				ArrayList tmp = new ArrayList();
				for (int i=0;i<this.objects.Length;i++)
				{
					if (objects[i].IsDynamic())
						tmp.Add(objects[i]);
				}
				return (ICustomPaint[])tmp.ToArray(typeof(ICustomPaint));
			}
		}
		

		/// <summary>
		/// Gets a collection of <see cref="Com.Delta.PrintManager.Engine.ICustomPaint">Com.Delta.PrintManager.Engine.ICustomPaint</see> objects
		/// which only display static data.
		/// </summary>
		[Browsable(false)]
		public ICustomPaint[] StaticObjects
		{
			get 
			{
				ArrayList tmp = new ArrayList();
				for (int i=0;i<this.objects.Length;i++)
				{
					if (!objects[i].IsDynamic())
						tmp.Add(objects[i]);
				}
				return (ICustomPaint[])tmp.ToArray(typeof(ICustomPaint));
			}
		}

		#endregion

		#region Private Methods

		private void CreateSubtotals(DataTable podaci, StyledTable styledTable)
		{
			DataRow subtotalRow = null;
			DataRow[] printRows = styledTable.DataRows;
			for (int i=printRows.Length-1;i>=0;i--)
			{
				if (printRows[i].RowError == "Subtotal")
				{
					subtotalRow = printRows[i];
					break;
				}
			}

			
			if (subtotalRow != null)
			{
				podaci.Rows.Remove(subtotalRow);
			}
			

			
			double[] sums = new double[styledTable.Columns.Length];

			for (int i=0;i<printRows.Length;i++)
			{
				for (int j=0;j<styledTable.Columns.Length;j++)
				{
					try
					{
						if (sums[j] != Double.NaN)
						{
							double value = Convert.ToDouble(printRows[i][styledTable.Columns[j].Name]);

							if (styledTable.Columns[j].TotalType == StyledTableColumn.SubtotalType.Sum)
							{
								sums[j] += value;
							}
							else if (styledTable.Columns[j].TotalType == StyledTableColumn.SubtotalType.Average)
							{
								if (printRows.Length == 0)
									sums[j] = 0;
								else
									sums[j] += value / printRows.Length;
							}
						}
					}
					catch(Exception e)
					{
						sums[j] = Double.NaN;
					}
					
				}
			}


			string[] subtotals = new string[styledTable.Columns.Length];

			for (int j=0;j<styledTable.Columns.Length;j++)
			{
				if (styledTable.Columns[j].TotalType != StyledTableColumn.SubtotalType.None)
				{
					if (Double.IsNaN(sums[j]))
						subtotals[j] = "###";	
					else
					{
						if (styledTable.Columns[j].FormatMask == null || styledTable.Columns[j].FormatMask == String.Empty)
							subtotals[j] = styledTable.Columns[j].Prefix + sums[j].ToString();
						else
							subtotals[j] = styledTable.Columns[j].Prefix + String.Format("{0:" + styledTable.Columns[j].FormatMask + "}", sums[j]);
					}
				}
				else
				{
					subtotals[j] = string.Empty;
				}
			}

			styledTable.Subtotals = subtotals;

			

			if (subtotalRow == null)
			{
				subtotalRow = podaci.NewRow();
				subtotalRow.RowError = "Subtotal";	

				ArrayList tmp = new ArrayList(printRows);
				tmp.Add(subtotalRow);
				styledTable.DataRows = (DataRow[])tmp.ToArray(typeof(DataRow));

			}
			else
			{
				subtotalRow = podaci.NewRow();
				subtotalRow.RowError = "Subtotal";
			}


		}

		/// <summary>
		/// Resolves color from given text.
		/// </summary>
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
				catch(Exception)
				{
					return Color.Red;
				}
			}
			else
			{
				return c;
			}
		}

		/// <summary>
		/// Function to load the Document objects from the XML template file
		/// </summary>
		private void InitObjects(XmlNodeList elements)
		{
			if (elements == null)
			{
				return;
			}

			int j = 0;

			for (int i=0;i<elements.Count;i++)
			{
				switch( elements[i].Name )
				{
					case "textField":
						TextField textField = resolveTextField(elements[i], j);
						AddElement(textField);
						if (textField.Name != String.Empty)
							document.RegisterTextField(textField.Name, this);
						j++;
						break;

					case "pictureBox":
						PictureBox pictureBox = resolvePictureBox(elements[i], j);
						AddElement(pictureBox);
						if (pictureBox.Name != String.Empty)
							document.RegisterPicture(pictureBox.Name, this);
						j++;
						break;

					case "chartBox":
						ChartBox chartBox = resolveChartBox(elements[i], j);
						AddElement(chartBox);
						document.RegisterChart(chartBox.Name, this);
						j++;
						break;

					case "table":						
						StyledTable styledTable = resolveTable(elements[i], j);
						AddElement(styledTable);
						if (styledTable.HasDataSource())
							document.RegisterTable(styledTable.DataSource, this);
						if (styledTable.Name != String.Empty)
							document.RegisterTableByName(styledTable.Name, this);
						j++;
						break;

					case "timeline":
						Timeline timeline = resolveTimeline(elements[i], j);
						AddElement(timeline);
						if (timeline.Name != String.Empty)
							document.RegisterTimeline(timeline.Name, this);
						j++;
						break;

					case "line":
						Line line = resolveLine(elements[i]);
						AddElement(line);
						j++;
						break;

					case "elipse":
						Elipse elipse = resolveElipse(elements[i]);
						AddElement(elipse);
						j++;
						break;

					case "box":
						Box box = resolveBox(elements[i]);
						AddElement(box);
						j++;
						break;

					case "barcode":
						Barcode barcode = resolveBarcode(elements[i], j);
						AddElement(barcode);
						if (barcode.Name != String.Empty)
							document.RegisterBarcode(barcode.Name, this);
						j++;
						break;


					case "map":
						Map map = resolveMap(elements[i], j);
						AddElement(map);
						if (map.Name != String.Empty)
							document.RegisterMap(map.Name, this);
						j++;
						break;

					case "scatter":
						Scatter scatter = resolveScatter(elements[i], j);
						AddElement(scatter);
						if (scatter.Name != String.Empty)
							document.RegisterScatter(scatter.Name, this);
						j++;
						break;

					case "userPaint":
						UserPaint userPaint = resolveUserPaint(elements[i], j);
						AddElement(userPaint);
						if (userPaint.Name != String.Empty)
							document.RegisterUserPaint(userPaint.Name, this);
						j++;
						break;

					case "richTextField":
						RichTextField richTextField = resolveRichTextField(elements[i], j);
						AddElement(richTextField);
						if (richTextField.Name != String.Empty)
							document.RegisterRichTextField(richTextField.Name, this);
						j++;
						break;

					default:
						break;
				}
			}

		}

		private void AddElement(ICustomPaint element)
		{
			ArrayList tmp = new ArrayList();
			tmp.AddRange(objects);
			tmp.Add(element);
			objects = (ICustomPaint[])tmp.ToArray(typeof(ICustomPaint));
		}
		
		private TextField resolveTextField(XmlNode theNode, int theIndex)
		{
			TextField textField = new TextField(theNode, this);

			XmlNodeList childNodes = theNode.ChildNodes;

			for (int i=0;i<childNodes.Count;i++)
			{
				switch (childNodes[i].Name)
				{
					case "text":
						
						try
						{
							if (childNodes[i].Attributes["value"] != null)
								textField.SetText(childNodes[i].Attributes["value"].Value);
							else
								textField.SetText(childNodes[i].InnerText);
						}
						catch (Exception){}
						
						try
						{
							string alignmentType = childNodes[i].Attributes["horAlignment"]==null ? "Left" : childNodes[i].Attributes["horAlignment"].Value;							
							textField.TextAlignment = (TextField.TextAlignmentType)Enum.Parse(typeof(Com.Delta.PrintManager.Engine.TextField.TextAlignmentType), alignmentType, false); 
						}
						catch (Exception){}

						try
						{
							string alignmentType = childNodes[i].Attributes["verAlignment"]==null ? "Top" : childNodes[i].Attributes["verAlignment"].Value;							
							textField.TextVerticalAlignment = (TextField.TextVerticalAlignmentType)Enum.Parse(typeof(TextField.TextVerticalAlignmentType), alignmentType, false); 
						}
						catch (Exception){}
						
						break;

					case "font":
						textField.Font = resolveFont( childNodes[i] );
						break;

					case "foregroundColor":
						try
						{
							textField.ForegroundColor = ResolveColor(childNodes[i].Attributes["color"].Value);
						}
						catch (Exception){}
						break;

					case "backgroundColor":
						try
						{
							textField.BackgroundColor = ResolveColor(childNodes[i].Attributes["color"].Value);
						}
						catch (Exception){}
						break;

					case "border":
						try
						{
							textField.BorderColor = ResolveColor( childNodes[i].Attributes["color"].Value );
							textField.BorderWidth = childNodes[i].Attributes["width"]==null ? 0 : Convert.ToInt32( childNodes[i].Attributes["width"].Value );
						}
						catch (Exception){}
						break;

					case "textSpacing":
						try
						{
							if (childNodes[i].Attributes["value"] != null)
								textField.Spacing = Convert.ToInt32(childNodes[i].Attributes["value"].Value);
							else
								textField.Spacing = Convert.ToInt32(childNodes[i].InnerText);	
						}
						catch (Exception){}
						break;

					case "textPadding":
						try
						{
							if (childNodes[i].Attributes["value"] != null)
								textField.Padding = Convert.ToInt32(childNodes[i].Attributes["value"].Value);
							else
								textField.Padding = Convert.ToInt32(childNodes[i].InnerText);	
						}
						catch (Exception){}
						break;

					case "textOrientation":
						try
						{
							if (childNodes[i].Attributes["value"] != null)
								textField.TextOrientation = (TextField.Orientation)Enum.Parse(typeof(TextField.Orientation), childNodes[i].Attributes["value"].Value, false);
							else
								textField.TextOrientation = (TextField.Orientation)Enum.Parse(typeof(TextField.Orientation), childNodes[i].InnerText, false); 
						}
						catch (Exception){}
						break;

					case "overflowTextDisplay":
						try
						{
							if (childNodes[i].Attributes["value"] != null)
								textField.OverflowTextHandling = childNodes[i].Attributes["value"].Value == "True" ? TextField.OverflowStyle.Display : TextField.OverflowStyle.Ignore;	
							else
								textField.OverflowTextHandling = childNodes[i].InnerText == "true" ? TextField.OverflowStyle.Display : TextField.OverflowStyle.Ignore;	
						}
						catch (Exception){}
						break;

				}

				if (textField.Name != String.Empty && !theTextFields.Contains(textField.Name))
					theTextFields.Add(textField.Name, theIndex);
			}

			return textField;
		}


		private RichTextField resolveRichTextField(XmlNode theNode, int theIndex)
		{
			RichTextField richTextField = new RichTextField(theNode, this);

			XmlNodeList childNodes = theNode.ChildNodes;

			for (int i=0;i<childNodes.Count;i++)
			{
				switch (childNodes[i].Name)
				{
					case "data":
						
						try
						{
								richTextField.Text = childNodes[i].InnerText;
						}
						catch (Exception){}
						
						
						
						break;

					case "font":
						richTextField.Font = resolveFont( childNodes[i] );
						break;

					case "foregroundColor":
						try
						{
							richTextField.ForegroundColor = ResolveColor(childNodes[i].Attributes["color"].Value);
						}
						catch (Exception){}
						break;

					case "backgroundColor":
						try
						{
							richTextField.BackgroundColor = ResolveColor(childNodes[i].Attributes["color"].Value);
						}
						catch (Exception){}
						break;

					case "border":
						try
						{
							richTextField.BorderColor = ResolveColor( childNodes[i].Attributes["color"].Value );
							richTextField.BorderWidth = childNodes[i].Attributes["width"]==null ? 0 : Convert.ToInt32( childNodes[i].Attributes["width"].Value );
						}
						catch (Exception){}
						break;

					
					case "textPadding":
						try
						{
							if (childNodes[i].Attributes["value"] != null)
								richTextField.Padding = Convert.ToInt32(childNodes[i].Attributes["value"].Value);
							else
								richTextField.Padding = Convert.ToInt32(childNodes[i].InnerText);	
						}
						catch (Exception){}
						break;

					case "overflowTextDisplay":
						try
						{
							if (childNodes[i].Attributes["value"] != null)
								richTextField.OverflowTextHandling = childNodes[i].Attributes["value"].Value == "True" ? TextField.OverflowStyle.Display : TextField.OverflowStyle.Ignore;	
							else
								richTextField.OverflowTextHandling = childNodes[i].InnerText == "true" ? TextField.OverflowStyle.Display : TextField.OverflowStyle.Ignore;	
						}
						catch (Exception){}
						break;

				}

				if (richTextField.Name != String.Empty && !theRichTextFields.Contains(richTextField.Name))
					theRichTextFields.Add(richTextField.Name, theIndex);
			}

			return richTextField;
		}


		internal ICustomPaint.LayoutTypes resolveLayout(XmlNode theNode)
		{
			try
			{
				if (theNode.Attributes["Layout"] != null)
				   return (ICustomPaint.LayoutTypes)Enum.Parse(typeof(ICustomPaint.LayoutTypes), theNode.Attributes["Layout"].Value, false); 				
				else 
				   return ICustomPaint.LayoutTypes.EveryPage;
			}
			catch (Exception)
			{
				return ICustomPaint.LayoutTypes.EveryPage;
			}
		}


		private Font resolveFont(XmlNode theNode)
		{
			try
			{
				Font theFont;
				string fntName = theNode.Attributes["family"]==null ? "Arial" : theNode.Attributes["family"].Value;
				int fntSize = theNode.Attributes["size"]==null ? 10 : Convert.ToInt32( theNode.Attributes["size"].Value );
				string fntStyle = theNode.Attributes["style"]==null ? "Regular" : theNode.Attributes["style"].Value;

				bool fntUnderline = theNode.Attributes["underline"]==null ? false : Convert.ToBoolean(theNode.Attributes["underline"].Value) ;

				FontStyle fontStyle = FontStyle.Regular;

				switch (fntStyle)
				{
					case "Bold Italic":
						fontStyle = FontStyle.Bold | FontStyle.Italic;
						break;

					case "Bold":
						fontStyle = FontStyle.Bold;
						break;

					case "Italic":
						fontStyle = FontStyle.Italic;
						break;

					default :
						fontStyle = FontStyle.Regular;
						break;
				}

				if (fntUnderline)
					fontStyle = fontStyle | FontStyle.Underline;


				theFont = new Font(fntName, fntSize, fontStyle);

				return theFont;
										
			}
			catch (Exception)
			{
				return new Font("Arial", 8, FontStyle.Regular);
			}
		}


		private StyledTable resolveTable(XmlNode theNode, int theIndex)
		{
			StyledTable styledTable = new StyledTable(theNode, this);
			

			bool hasDataSource = false;
			if ( theNode.Attributes["dataSource"] != null && theNode.Attributes["dataSource"].Value.Trim() != String.Empty)
			{
				styledTable.DataSource = theNode.Attributes["dataSource"].Value;
				hasDataSource = true;
			}
			
			try
			{
				styledTable.DrawHeader =  theNode.Attributes["showHeader"]==null ? true : Convert.ToBoolean(theNode.Attributes["showHeader"].Value);				
			}
			catch (Exception){}

			try
			{
				styledTable.DrawEmptyRows =  theNode.Attributes["drawEmptyRows"]==null ? false : Convert.ToBoolean(theNode.Attributes["drawEmptyRows"].Value);				
			}
			catch (Exception){}

			try
			{
				styledTable.CellHeight =  theNode.Attributes["cellHeight"]==null ? 18 : Convert.ToInt32(theNode.Attributes["cellHeight"].Value);				
			}
			catch (Exception){}
			

			string[] columnLabels = new string[0];
			XmlNodeList childNodes = theNode.ChildNodes;

			for (int i=0;i<childNodes.Count;i++)
			{
				switch (childNodes[i].Name)
				{
					case "columns":
						styledTable.Columns = this.resolveColumns(childNodes[i]);						
						break;

					case "header":
						try
						{
							styledTable.HeaderBackgroundColor =  ResolveColor( childNodes[i].Attributes["headerColor"].Value);
						}
						catch (Exception){}

						try
						{
							styledTable.HeaderFontColor =  ResolveColor( childNodes[i].Attributes["headerFontColor"].Value);
						}
						catch (Exception){}

						XmlNodeList headerNodes = childNodes[i].ChildNodes;
						for (int j=0;j<headerNodes.Count;j++)
						{
							switch (headerNodes[j].Name)
							{
								case "font":
									styledTable.HeaderFont = resolveFont(headerNodes[j]);
									break;
							}
						}						
						break;


					case "border":
						try
						{
							styledTable.BorderColor =  ResolveColor(childNodes[i].Attributes["color"].Value);
						}
						catch (Exception){}
						break;

					case "dataRows":
						try
						{
							styledTable.DataFontColor =  ResolveColor(childNodes[i].Attributes["dataFontColor"].Value);
							styledTable.BackgroundColor =  ResolveColor(childNodes[i].Attributes["backgroundColor"].Value);
						}
						catch (Exception){}
						try
						{
							styledTable.AlterDataColor =  ResolveColor(childNodes[i].Attributes["alterDataColor"].Value);
						}
						catch (Exception){}

						if (childNodes[i].Attributes["alterDataBackColor"]!=null)
						{
							try
							{
								styledTable.AlterDataBackColor = ResolveColor(childNodes[i].Attributes["alterDataBackColor"].Value);
							}
							catch (Exception){}
						}

						
						try
						{
							styledTable.SubtotalsColor =  ResolveColor(childNodes[i].Attributes["subtotalsColor"].Value);
						}
						catch (Exception){}


						try
						{
							styledTable.AlterDataCondition =  childNodes[i].Attributes["alterDataCondition"].Value;
						}
						catch (Exception){}

						
						XmlNodeList dataNodes = childNodes[i].ChildNodes;
						for (int j=0;j<dataNodes.Count;j++)
						{
							switch (dataNodes[j].Name)
							{
								case "font":
									styledTable.DataFont = resolveFont(dataNodes[j]);
									break;

								case "alternateBackColor":
									styledTable.AlternateBackColor = Convert.ToBoolean(dataNodes[j].Attributes["value"].Value);
									break;

								case "alternatingBackColor":
									styledTable.AlternatingBackColor = ResolveColor(dataNodes[j].Attributes["color"].Value);
									break;

								case "filterExpression":
									styledTable.FilterExpression = dataNodes[j].Attributes["value"].Value;
									break;

								case "sortExpression":
									styledTable.SortExpression = dataNodes[j].Attributes["value"].Value;
									break;
							}
						}
												
						break;

					case "font":
						styledTable.DataFont = resolveFont(childNodes[i]);
						break;


					case "data":
						if (this.Document.DesignMode)
						{
							styledTable.Data = resolveStaticTableData(childNodes[i],styledTable.Columns);
						}
						else
						{
							if (!hasDataSource)
								styledTable.Data = resolveStaticTableData(childNodes[i],styledTable.Columns);
						}
						break;
				}
			}

			if (styledTable.Columns.Length == 0)
			{
				if (hasDataSource && theTables.Contains(styledTable.DataSource) )
					styledTable.Columns = createColumns ((DataTable)theTables[styledTable.DataSource]);
				else
				{
					StyledTableColumn[] kolone = new StyledTableColumn[1];
					kolone[0] = new StyledTableColumn();
					kolone[0].Label = "Wrong dataSource name";
					styledTable.Columns = kolone;
				}
			}

			if (styledTable.Name != string.Empty && !theTablesByName.Contains(styledTable.Name))
				theTablesByName.Add(styledTable.Name,theIndex);

			return styledTable;
		}


		private StyledTableColumn[] createColumns(DataTable masterTable)
		{

			StyledTableColumn[] cols = new StyledTableColumn[masterTable.Columns.Count];
			for (int i=0;i<masterTable.Columns.Count;i++)
			{
				cols[i] = new StyledTableColumn();
				cols[i].Name = masterTable.Columns[i].ColumnName;
				cols[i].Label = masterTable.Columns[i].ColumnName;
			}
			return cols;
		}

		private PictureBox resolvePictureBox(XmlNode theNode, int theIndex)
		{
			PictureBox pictureBox = new PictureBox(theNode, this);			

			if (theNode.Attributes["stretch"] != null)
			{
				try
				{
					pictureBox.Stretch = Convert.ToBoolean(theNode.Attributes["stretch"].Value);
				}
				catch (Exception){}
			}

			for (int i=0;i<theNode.ChildNodes.Count;i++)
			{
				switch (theNode.ChildNodes[i].Name)
				{
					case "file":
						if (theNode.ChildNodes[i].Attributes["value"] != null)
						    pictureBox.ImageFile = theNode.ChildNodes[i].Attributes["value"].Value;
						else
							pictureBox.ImageFile = theNode.ChildNodes[i].InnerText;
						break;
							

					case "border":
						try
						{
							pictureBox.BorderColor = ResolveColor( theNode.ChildNodes[i].Attributes["color"].Value );
							pictureBox.BorderWidth = theNode.ChildNodes[i].Attributes["width"]==null ? 0 : Convert.ToInt32( theNode.ChildNodes[i].Attributes["width"].Value );
						}
						catch (Exception){}
						break;

					case "stretch":
						try
						{
							pictureBox.Stretch = Convert.ToBoolean(theNode.ChildNodes[i].Attributes["value"].Value );							
						}
						catch (Exception){}
						break;

					case "pdfExportQuality":
						try
						{
							pictureBox.ExportQuality = theNode.ChildNodes[i].Attributes["value"]==null ? 100 : Convert.ToInt32( theNode.ChildNodes[i].Attributes["value"].Value );
						}
						catch (Exception){}
						break;
				}
			}

			if (pictureBox.Name!=String.Empty && !thePictures.Contains(pictureBox.Name))
				thePictures.Add(pictureBox.Name, theIndex);

			return pictureBox;
		}


		private Line resolveLine(XmlNode theNode)
		{
			Line line = new Line(theNode, this);

			if (theNode.Attributes["color"] != null)
			{
				try
				{
					line.Color = ResolveColor(theNode.Attributes["color"].Value);
					line.LineWidth = Convert.ToInt32(theNode.Attributes["lineWidth"].Value);
				}
				catch (Exception){}
			}

			for (int i=0;i<theNode.ChildNodes.Count;i++)
			{
				if (theNode.ChildNodes[i].Name == "orientation")
				{
					try 
					{
						if (theNode.ChildNodes[i].Attributes["value"] != null)
							line.Orientation = (Line.Orientations)Enum.Parse(typeof(Line.Orientations), theNode.ChildNodes[i].Attributes["value"].Value, false);
						else
							line.Orientation = (Line.Orientations)Enum.Parse(typeof(Line.Orientations), theNode.ChildNodes[i].InnerText, false);						
					}
					catch (Exception){}
				}
				else if (theNode.ChildNodes[i].Name == "foregroundColor")
				{					
					try
					{
						line.Color = ResolveColor(theNode.ChildNodes[i].Attributes["color"].Value);							
					}
					catch (Exception){}						
				}
				else if (theNode.ChildNodes[i].Name == "lineWidth")
				{					
					try
					{
						line.LineWidth = Convert.ToInt32(theNode.ChildNodes[i].Attributes["value"].Value);							
					}
					catch (Exception){}						
				}
			}

			return line;
		}

		private void resolveFlow(XmlNode theNode)
		{
			Chain.Clear();
			string s = theNode.Attributes["value"].Value;
			string[] segments = s.Split(',');
			foreach(string indexWord in segments)
			{
				try
				{
					int index = Int32.Parse(indexWord);
					if (!Chain.Contains(index))
						Chain.Add(index);
				}
				catch(Exception){}
			}
		}

		private Elipse resolveElipse(XmlNode theNode)
		{
			Elipse elipse = new Elipse(theNode, this);

			if (theNode.Attributes["color"] != null)
				elipse.Color = ResolveColor(theNode.Attributes["color"].Value);

			XmlNodeList childNodes = theNode.ChildNodes;

			for (int i=0;i<childNodes.Count;i++)
			{
				switch (childNodes[i].Name)
				{
					case "fillStyle":
						try 
						{
							if (childNodes[i].Attributes["value"] != null)
								elipse.FillStyle = (Elipse.FillStyles)Enum.Parse(typeof(Elipse.FillStyles), childNodes[i].Attributes["value"].Value, false);								
							else
								elipse.FillStyle = (Elipse.FillStyles)Enum.Parse(typeof(Elipse.FillStyles), childNodes[i].InnerText, false);
						}
						catch (Exception){}
						break;

					case "border":
						try
						{
							elipse.BorderColor = ResolveColor( childNodes[i].Attributes["color"].Value );
							elipse.BorderWidth = childNodes[i].Attributes["width"]==null ? 1 : Convert.ToInt32( childNodes[i].Attributes["width"].Value );
						}
						catch (Exception){}
						break;

					case "gradientColor":
						try
						{
							if (childNodes[i].Attributes["color"] != null)
								elipse.GradientColor = ResolveColor(childNodes[i].Attributes["color"].Value);							
							else
								elipse.GradientColor = ResolveColor(childNodes[i].InnerText);
						}
						catch (Exception){}
						break;

					case "foregroundColor":
						try
						{
							elipse.Color = ResolveColor(childNodes[i].Attributes["color"].Value);							
						}
						catch (Exception){}
						break;

					case "gradientMode":						
						try 
						{
							if (childNodes[i].Attributes["value"] != null)
								elipse.GradientMode = (System.Drawing.Drawing2D.LinearGradientMode)Enum.Parse(typeof(System.Drawing.Drawing2D.LinearGradientMode), childNodes[i].Attributes["value"].Value, false);								
							else
								elipse.GradientMode = (System.Drawing.Drawing2D.LinearGradientMode)Enum.Parse(typeof(System.Drawing.Drawing2D.LinearGradientMode), childNodes[i].InnerText, false);								
						}
						catch (Exception){}
						break;

				}

			}

			return elipse;
		}


		private Box resolveBox(XmlNode theNode)
		{
			Box box = new Box(theNode, this);			

			if (theNode.Attributes["color"] != null)
				box.Color = ResolveColor(theNode.Attributes["color"].Value);

			
			XmlNodeList childNodes = theNode.ChildNodes;

			for (int i=0;i<childNodes.Count;i++)
			{
				switch (childNodes[i].Name)
				{
					case "fillStyle":						
						try 
						{
							if (childNodes[i].Attributes["value"] != null)
								box.FillStyle = (Box.FillStyles)Enum.Parse(typeof(Box.FillStyles), childNodes[i].Attributes["value"].Value, false);								
							else
								box.FillStyle = (Box.FillStyles)Enum.Parse(typeof(Box.FillStyles), childNodes[i].InnerText, false);
						}
						catch (Exception){}
						break;

					case "border":
						try
						{
							box.BorderColor = ResolveColor( childNodes[i].Attributes["color"].Value );
							box.BorderWidth = childNodes[i].Attributes["width"]==null ? 1 : Convert.ToInt32( childNodes[i].Attributes["width"].Value );
						}
						catch (Exception){}
						break;

					case "gradientColor":
						try
						{
							if (childNodes[i].Attributes["color"] != null)
								box.GradientColor = ResolveColor(childNodes[i].Attributes["color"].Value);							
							else
								box.GradientColor = ResolveColor(childNodes[i].InnerText);
						}
						catch (Exception){}
						break;

					case "foregroundColor":
						try
						{
							box.Color = ResolveColor(childNodes[i].Attributes["color"].Value);							
						}
						catch (Exception){}
						break;

					case "gradientMode":						
						try 
						{
							if (childNodes[i].Attributes["value"] != null)
								box.GradientMode = (System.Drawing.Drawing2D.LinearGradientMode)Enum.Parse(typeof(System.Drawing.Drawing2D.LinearGradientMode), childNodes[i].Attributes["value"].Value, false);								
							else
								box.GradientMode = (System.Drawing.Drawing2D.LinearGradientMode)Enum.Parse(typeof(System.Drawing.Drawing2D.LinearGradientMode), childNodes[i].InnerText, false);								
						}
						catch (Exception){}
						break;

				}

			}

			return box;
		}


		private Timeline resolveTimeline(XmlNode theNode, int theIndex)
		{
			Timeline timeline = new Timeline( theNode, this);

			
			try 
			{
				timeline.StripeNumber = Convert.ToInt32(theNode.Attributes["stripeNumber"].Value);
			}
			catch(Exception){}

			try 
			{
				timeline.StripeSize = Convert.ToInt32(theNode.Attributes["stripeSize"].Value);
			}
			catch(Exception){}

			
			try
			{
				string stripeStyle = theNode.Attributes["stripeStyle"].Value;
				timeline.StripeStyle = (Timeline.StripeStyles)Enum.Parse(typeof(Timeline.StripeStyles), stripeStyle, false);
			}
			catch (Exception){}

			try 
			{
				timeline.UseDates = Convert.ToBoolean(theNode.Attributes["useDates"].Value);
			}
			catch(Exception){}


			XmlNodeList childNodes = theNode.ChildNodes;
			for (int i=0;i<childNodes.Count;i++)
			{
				switch (childNodes[i].Name)
				{
					case "backgroundColor":						
						try
						{
							if (childNodes[i].Attributes["color"] != null)
								timeline.BackgroundColor = ResolveColor(childNodes[i].Attributes["color"].Value);							
							else
								timeline.BackgroundColor = ResolveColor(childNodes[i].InnerText);							
							}
						catch (Exception){}
						break;

					case "stripeColor":
						try
						{
							if (childNodes[i].Attributes["color"] != null)
								timeline.StripeColor = ResolveColor(childNodes[i].Attributes["color"].Value);							
							else
								timeline.StripeColor = ResolveColor(childNodes[i].InnerText);							
						}
						catch (Exception){}
						break;

					case "startDate":
						try
						{
							if (childNodes[i].Attributes["value"] != null)
								timeline.StartDate = Convert.ToDateTime(childNodes[i].Attributes["value"].Value, usFormat);							
							else
								timeline.StartDate = Convert.ToDateTime(childNodes[i].InnerText, usFormat);							
						}
						catch (Exception){}
						break;

					case "markerStyle":
						try
						{
							if (childNodes[i].Attributes["value"] != null)
								timeline.MarkerStyle = (Timeline.MarkerStyles)Enum.Parse(typeof(Timeline.MarkerStyles), childNodes[i].Attributes["value"].Value, false);
							else
								timeline.MarkerStyle = (Timeline.MarkerStyles)Enum.Parse(typeof(Timeline.MarkerStyles), childNodes[i].InnerText, false); 
						}
						catch (Exception){}
						break;

					case "markerPeriod":
						try
						{
							if (childNodes[i].Attributes["value"] != null)
								timeline.MarkerPeriod = (Timeline.MarkerPeriods)Enum.Parse(typeof(Timeline.MarkerPeriods), childNodes[i].Attributes["value"].Value, false);
							else
								timeline.MarkerPeriod = (Timeline.MarkerPeriods)Enum.Parse(typeof(Timeline.MarkerPeriods), childNodes[i].InnerText, false); 
						}
						catch (Exception){}
						break;

					case "markerColor":
						try
						{
							if (childNodes[i].Attributes["color"] != null)
								timeline.MarkerColor = ResolveColor(childNodes[i].Attributes["color"].Value);							
							else
								timeline.MarkerColor = ResolveColor(childNodes[i].InnerText);							
						}
						catch (Exception){}
						break;

					case "endDate":
						try
						{
							if (childNodes[i].Attributes["value"] != null)
								timeline.EndDate = Convert.ToDateTime(childNodes[i].Attributes["value"].Value, usFormat);							
							else
							    timeline.EndDate = Convert.ToDateTime(childNodes[i].InnerText, usFormat);							
						}
						catch (Exception){}
						break;

					case "startValue":
						try
						{
							if (childNodes[i].Attributes["value"] != null)
								timeline.StartValue = Convert.ToInt64(childNodes[i].Attributes["value"].Value);							
							else
								timeline.StartValue = Convert.ToInt64(childNodes[i].InnerText);							
						}
						catch (Exception){}
						break;

					case "endValue":
						try
						{
							if (childNodes[i].Attributes["value"] != null)
								timeline.EndValue = Convert.ToInt64(childNodes[i].Attributes["value"].Value);							
							else
							    timeline.EndValue = Convert.ToInt64(childNodes[i].InnerText);							
						}
						catch (Exception){}
						break;

					case "padding":						
						try
						{
							int w = Convert.ToInt32(childNodes[i].Attributes["width"].Value);
							int h = Convert.ToInt32(childNodes[i].Attributes["height"].Value);
							timeline.Padding = new Size(w, h);								
						}
						catch (Exception){}
						break;

					case "border":						
						try
						{
							timeline.BorderWidth = Convert.ToInt32( childNodes[i].Attributes["width"].Value );							
						}
						catch (Exception){}
						break;

					case "pdfExportQuality":
						try
						{
							timeline.ExportQuality = theNode.ChildNodes[i].Attributes["value"]==null ? 100 : Convert.ToInt32( theNode.ChildNodes[i].Attributes["value"].Value );
						}
						catch (Exception){}
						break;

					case "timepoints":
						timeline.Events = resolveTimelinePoints(childNodes[i]);
						break;

					case "timeperiods":
						timeline.Periods = resolveTimelinePeriods(childNodes[i]);
						break;

				}

			}

			if (timeline.Name!=String.Empty && !theTimelines.Contains(timeline.Name))
				theTimelines.Add(timeline.Name, theIndex);

			return timeline;
		}


		private TimelinePeriod[] resolveTimelinePeriods(XmlNode theNode)
		{
			ArrayList tmp = new ArrayList();

			XmlNodeList childNodes = theNode.ChildNodes;
			for (int i=0;i<childNodes.Count;i++)
			{
				TimelinePeriod period = new TimelinePeriod();

				try 
				{
					period.Offset = Convert.ToInt32(childNodes[i].Attributes["offset"].Value);
				}
				catch(Exception){}

				try 
				{
					period.Color = ResolveColor(childNodes[i].Attributes["color"].Value);
				}
				catch(Exception){}

				XmlNodeList periodData = childNodes[i].ChildNodes;
				for (int j=0;j<periodData.Count;j++)
				{
					switch (periodData[j].Name)
					{
						case "text":						
							try
							{
								if (periodData[j].Attributes["value"] != null)
									period.Text = periodData[j].Attributes["value"].Value;
								else
									period.Text = periodData[j].InnerText;
							}
							catch (Exception){}
							break;

						case "textPosition":						
							try
							{
								if (periodData[j].Attributes["value"] != null)
									period.TextPosition = (TimelinePeriod.Positions)Enum.Parse(typeof(TimelinePeriod.Positions), periodData[j].Attributes["value"].Value, false);
								else
									period.TextPosition = (TimelinePeriod.Positions)Enum.Parse(typeof(TimelinePeriod.Positions), periodData[j].InnerText, false); 												
							}
							catch (Exception){}
							break;

						case "startDate":						
							try
							{
								if (periodData[j].Attributes["value"] != null)
									period.StartDate = Convert.ToDateTime(periodData[j].Attributes["value"].Value, usFormat);							
								else
									period.StartDate = Convert.ToDateTime(periodData[j].InnerText, usFormat);							
							}
							catch (Exception){}
							break;

						case "endDate":						
							try
							{
								if (periodData[j].Attributes["value"] != null)
									period.EndDate = Convert.ToDateTime(periodData[j].Attributes["value"].Value, usFormat);							
								else
									period.EndDate = Convert.ToDateTime(periodData[j].InnerText, usFormat);							
							}
							catch (Exception){}
							break;

						case "startValue":						
							try
							{
								if (periodData[j].Attributes["value"] != null)
									period.StartValue = Convert.ToInt64(periodData[j].Attributes["value"].Value);
								else
									period.StartValue = Convert.ToInt64(periodData[j].InnerText);							
							}
							catch (Exception){}
							break;

						case "endValue":						
							try
							{
								if (periodData[j].Attributes["value"] != null)
									period.EndValue = Convert.ToInt64(periodData[j].Attributes["value"].Value);
								else
									period.EndValue = Convert.ToInt64(periodData[j].InnerText);							
							}
							catch (Exception){}
							break;

						case "font":						
							period.Font = resolveFont(periodData[j]);
							break;

					}
				}

				tmp.Add(period);
			}

			return (TimelinePeriod[])tmp.ToArray(typeof(Com.Delta.PrintManager.Engine.TimelinePeriod));

		}


		private TimelinePoint[] resolveTimelinePoints(XmlNode theNode)
		{
			ArrayList tmp = new ArrayList();

			XmlNodeList childNodes = theNode.ChildNodes;
			for (int i=0;i<childNodes.Count;i++)
			{
				TimelinePoint point = new TimelinePoint();

				try 
				{
					point.ShowDate = Convert.ToBoolean(childNodes[i].Attributes["showDate"].Value);
				}
				catch(Exception){}

				XmlNodeList pointData = childNodes[i].ChildNodes;
				for (int j=0;j<pointData.Count;j++)
				{
					switch (pointData[j].Name)
					{
						case "text":						
							try
							{
								if (pointData[j].Attributes["value"] != null)
									point.Text = pointData[j].Attributes["value"].Value;							
								else
									point.Text = pointData[j].InnerText;
							}
							catch (Exception){}
							break;

						case "date":						
							try
							{
								if (pointData[j].Attributes["value"] != null)
									point.Date = Convert.ToDateTime(pointData[j].Attributes["value"].Value, usFormat);							
								else
									point.Date = Convert.ToDateTime(pointData[j].InnerText, usFormat);							
							}
							catch (Exception){}
							break;

						case "value":						
							try
							{
								if (pointData[j].Attributes["value"] != null)
									point.Value = Convert.ToInt64(pointData[j].Attributes["value"].Value);							
								else
									point.Value = Convert.ToInt64(pointData[j].InnerText);							
							}
							catch (Exception){}
							break;

						case "box":						
							try
							{
								int w = Convert.ToInt32(pointData[j].Attributes["width"].Value);
								int h = Convert.ToInt32(pointData[j].Attributes["height"].Value);
								point.BoxSize = new Size(w, h);						
							}
							catch (Exception){}
							try
							{
								point.BoxTextColor = ResolveColor( pointData[j].Attributes["textColor"].Value );							
							}
							catch (Exception){}
							try
							{
								point.BoxColor = ResolveColor( pointData[j].Attributes["color"].Value );							
							}
							catch (Exception){}

							try
							{
								point.BoxShadow = Convert.ToBoolean(pointData[j].Attributes["boxShadow"].Value );							
							}
							catch (Exception){}
							break;

						case "boxOffset":						
							try
							{
								int x = Convert.ToInt32(pointData[j].Attributes["x"].Value);
								int y = Convert.ToInt32(pointData[j].Attributes["y"].Value);
								point.BoxOffset = new Point(x, y);
							}
							catch (Exception){}
							break;

						case "pictureSize":						
							try
							{
								int w = Convert.ToInt32(pointData[j].Attributes["width"].Value);
								int h = Convert.ToInt32(pointData[j].Attributes["height"].Value);
								point.PictureSize = new Size(w, h);
							}
							catch (Exception){}
							break;

						case "pictureOffset":						
							try
							{
								int x = Convert.ToInt32(pointData[j].Attributes["x"].Value);
								int y = Convert.ToInt32(pointData[j].Attributes["y"].Value);
								point.PictureOffset = new Point(x, y);
							}
							catch (Exception){}
							break;

						case "dateFormat":						
							try
							{
								if (pointData[j].Attributes["value"] != null)
									point.DateFormat = pointData[j].Attributes["value"].Value;							
								else
									point.DateFormat = pointData[j].InnerText;							
							}
							catch (Exception){}
							break;

						case "pictureFile":						
							try
							{
								if (pointData[j].Attributes["value"] != null)
									point.PictureFile = pointData[j].Attributes["value"].Value;
								else
									point.PictureFile = pointData[j].InnerText;							
							}
							catch (Exception){}
							break;

						case "pictureBorder":						
							try
							{
								point.PictureBorderWidth = Convert.ToInt32( pointData[j].Attributes["width"].Value );							
							}
							catch (Exception){}
							try
							{
								point.PictureBorderColor = ResolveColor( pointData[j].Attributes["color"].Value );							
							}
							catch (Exception){}
							break;

						case "font":						
							point.Font = resolveFont(pointData[j]);
							break;
					}
				}

				tmp.Add(point);
			}

			return (TimelinePoint[])tmp.ToArray(typeof(Com.Delta.PrintManager.Engine.TimelinePoint));
		}

		private Com.Delta.PrintManager.Engine.Map.MapPoint[] resolveMapPoints(XmlNode theNode)
		{
			System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
			ArrayList tmp = new ArrayList();

			XmlNodeList childNodes = theNode.ChildNodes;
			for (int i=0;i<childNodes.Count;i++)
			{
				Com.Delta.PrintManager.Engine.Map.MapPoint point = new Com.Delta.PrintManager.Engine.Map.MapPoint();

				try 
				{
					point.X = Convert.ToSingle(childNodes[i].Attributes["x"].Value, ci.NumberFormat);
				}
				catch(Exception){}

				try 
				{
					point.Y = Convert.ToSingle(childNodes[i].Attributes["y"].Value, ci.NumberFormat);
				}
				catch(Exception){}

				try 
				{
					point.Text = childNodes[i].Attributes["text"].Value;
				}
				catch(Exception){}

				try 
				{
					point.Color = ResolveColor(childNodes[i].Attributes["color"].Value);
				}
				catch(Exception){}

				try 
				{
					point.ShowMarker = Convert.ToBoolean(childNodes[i].Attributes["showMarker"].Value);
				}
				catch(Exception){}

				XmlNodeList children = childNodes[i].ChildNodes;
				for (int j=0;j<children.Count;j++)
				{
					if (children[j].Name == "font")
					{
						point.Font = this.resolveFont(children[j]);
					}
				}

				

				tmp.Add(point);
			}

			return (Com.Delta.PrintManager.Engine.Map.MapPoint[])tmp.ToArray(typeof(Com.Delta.PrintManager.Engine.Map.MapPoint));
		}


		private void resolveChartData(XmlNode theNode, ChartBox chartBox)
		{
			XmlNodeList childNodes = theNode.ChildNodes;
			for (int i=0;i<childNodes.Count;i++)
			{
				if (childNodes[i].Name == "categories")
				{
					XmlNodeList categoryNodes = childNodes[i].ChildNodes;
					string[] catList = new string[categoryNodes.Count];
					for (int j=0;j<categoryNodes.Count;j++)
					{
						if (categoryNodes[j].Attributes["value"] != null)
							catList[j] = categoryNodes[j].Attributes["value"].Value;
						else
							catList[j] = categoryNodes[j].InnerText;
					}

					chartBox.Categories = catList;					
				}
				else if (childNodes[i].Name == "series")
				{
					XmlNodeList seriesNodes = childNodes[i].ChildNodes;

					string[] seriesNames = new string[seriesNodes.Count];
					Color[] seriesColor = new Color[seriesNodes.Count];

					System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
					for (int j=0;j<seriesNodes.Count;j++)
					{
						seriesNames[j] = seriesNodes[j].Attributes["name"].Value;
						
						try
						{
							seriesColor[j] = this.ResolveColor( seriesNodes[j].Attributes["color"].Value );
							
						}
						catch (Exception){ seriesColor[j] = Color.Red;}

						
						double[] values = new double[chartBox.Categories.Length];
						XmlNodeList valueNodes = seriesNodes[j].ChildNodes;
						for (int k=0;k<chartBox.Categories.Length;k++)
						{
							try
							{
								if (valueNodes[k].Attributes["value"] != null)
									values[k] = Convert.ToDouble(valueNodes[k].Attributes["value"].Value, ci.NumberFormat);
								else
									values[k] = Convert.ToDouble(valueNodes[k].InnerText, ci.NumberFormat);
							}
							catch (Exception){ values[k] = 0;}
						}

						chartBox.AddSerie(seriesNames[j], values, seriesColor[j]);
					}
				}

			}
		}

		private void resolveScatterData(XmlNode theNode, Scatter scatter)
		{
			XmlNodeList childNodes = theNode.ChildNodes;
			for (int i=0;i<childNodes.Count;i++)
			{
                if (childNodes[i].Name == "series")
				{
					XmlNodeList seriesNodes = childNodes[i].ChildNodes;

					string[] seriesNames = new string[seriesNodes.Count];
					Color[] seriesColor = new Color[seriesNodes.Count];

					System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
					for (int j=0;j<seriesNodes.Count;j++)
					{
						ScatterSerie serie = new ScatterSerie();
						
						string serieName = seriesNodes[j].Attributes["name"].Value;
						Color serieColor = Color.Red;
						int pointNumber = 0;
						System.Drawing.Drawing2D.DashStyle style = System.Drawing.Drawing2D.DashStyle.Solid;
						
						try
						{
							serieColor = this.ResolveColor( seriesNodes[j].Attributes["color"].Value );							
						}
						catch (Exception){ serieColor = Color.Red;}

						try
						{
							pointNumber = Convert.ToInt32( seriesNodes[j].Attributes["pointCount"].Value );							
						}
						catch (Exception){ pointNumber = 0;}

						try
						{
							switch (seriesNodes[j].Attributes["dashStyle"].Value)
							{
								case "Dot":									
									style = System.Drawing.Drawing2D.DashStyle.Dot;
									break;
								case "DashDotDot":
									style = System.Drawing.Drawing2D.DashStyle.DashDotDot;
									break;
								case "DashDot":
									style = System.Drawing.Drawing2D.DashStyle.DashDot;
									break;
								case "Dash":
									style = System.Drawing.Drawing2D.DashStyle.Dash;
									break;
								case "Custom":
									style = System.Drawing.Drawing2D.DashStyle.Custom;
									break;
								default:
									style = System.Drawing.Drawing2D.DashStyle.Solid;
									break;

							}	
						}
						catch (Exception){}


						double[] x = new double[pointNumber];
						double[] y = new double[pointNumber];

						XmlNodeList valueNodes = seriesNodes[j].ChildNodes;
						for (int k=0;k<pointNumber;k++)
						{
							try
							{
								x[k] = Convert.ToDouble(valueNodes[k].Attributes["x"].Value, ci.NumberFormat);
								y[k] = Convert.ToDouble(valueNodes[k].Attributes["y"].Value, ci.NumberFormat);
							}
							catch (Exception)
							{
								x[k] = 0;
								y[k] = 0;
							}
						}

						scatter.AddSerie(serieName, x, y, serieColor, style);
					}
				}

			}
		}


		private DataTable resolveStaticTableData(XmlNode theNode,StyledTableColumn[] staticColumns)
		{
			DataTable dataTable = new DataTable();

			XmlNodeList recordNodes = theNode.ChildNodes;

			int recordCount = recordNodes.Count;
			if (recordCount > 0)
			{
				int columnCount = staticColumns.Length;
				for (int i=0;i<columnCount;i++)
					dataTable.Columns.Add(new DataColumn(staticColumns[i].Name));

				for (int i=0;i<recordNodes.Count;i++)
				{
					XmlNodeList fieldNodes = recordNodes[i].ChildNodes;
					string[] theRow = new string[columnCount];
					for (int j=0;j<columnCount;j++)
					{						
						try
						{
							if (fieldNodes[j].Attributes["value"] != null)
							    theRow[j] = fieldNodes[j].Attributes["value"].Value;
							else
								theRow[j] = fieldNodes[j].InnerText;
						}
						catch (Exception)
						{
							theRow[j] = "";
						}
					}

					dataTable.Rows.Add(theRow);
				}
			}
			return dataTable;
		}

		private UserPaint resolveUserPaint(XmlNode theNode,int theIndex)
		{
			UserPaint userPaint = new UserPaint(theNode, this);

			/*
			for (int i=0;i<theNode.ChildNodes.Count;i++)
			{
				if (theNode.ChildNodes[i].Name == "type")
				{
					try
					{
						
							userPaint.Type = theNode.ChildNodes[i].Attributes["value"].Value;
							
							if (userPaint.Type != string.Empty)
							{
								Type externalType = Type.GetType(userPaint.Type);
							    
								if (externalType != null)
								{
									UserPaint external = (UserPaint)Activator.CreateInstance(externalType);

									if (external != null)
									{
										external.Section = userPaint.Section;
										external.X = userPaint.X;
										external.Y = userPaint.Y;
										external.Width = userPaint.Width;
										external.Height = userPaint.Height;							
										external.Name = userPaint.Name;
										external.Layout = userPaint.Layout;
										external.Selectable = userPaint.Selectable;
										external.HorizontalAlignment = userPaint.HorizontalAlignment;
										external.VerticalAlignment = userPaint.VerticalAlignment;
										external.Type = userPaint.Type;

										userPaint = external;
									}
								}
							}							
							
					}
					catch (Exception){}
				}
				else if (theNode.ChildNodes[i].Name == "data")
				{
					if (theNode.ChildNodes[i].ChildNodes[0] != null)
					{
						XmlNode dataNode = theNode.ChildNodes[i].ChildNodes[0];
						try
						{
							userPaint.DeserializeFromXml(dataNode);
						}
						catch(Exception){}
					}
				}
			}
			*/
	
			if (userPaint.Name!=string.Empty && !theUserPaints.Contains(userPaint.Name))
				theUserPaints.Add(userPaint.Name, theIndex);

			return userPaint;
		}

		private Barcode resolveBarcode(XmlNode theNode,int theIndex)
		{
			Barcode barcode = new Barcode(theNode, this);
			if (barcode.Name == string.Empty)
				barcode.Name = "scatter0";		

			for (int i=0;i<theNode.ChildNodes.Count;i++)
			{
				switch (theNode.ChildNodes[i].Name)
				{
					
					case "text":
						try
						{
							if (theNode.ChildNodes[i].Attributes["value"] != null)
							    barcode.Text = theNode.ChildNodes[i].Attributes["value"].Value;
							else
								barcode.Text = theNode.ChildNodes[i].InnerText ;
						}
						catch (Exception){}
						break;

					case "labelFont":
						try
						{							
							barcode.LabelFont = resolveFont( theNode.ChildNodes[i] );
						}
						catch (Exception){}
						break;

					case "showLabel":
						try
						{
							if (theNode.ChildNodes[i].Attributes["value"] != null)
								barcode.ShowLabel = Convert.ToBoolean(theNode.ChildNodes[i].Attributes["value"].Value) ;
							else
								barcode.ShowLabel = Convert.ToBoolean(theNode.ChildNodes[i].InnerText) ;
						}
						catch (Exception){}
						break;

					case "barcodeType":
						try 
						{
							if (theNode.ChildNodes[i].Attributes["value"] != null)
								barcode.BarcodeType = (Barcode.BarcodeTypes)Enum.Parse(typeof(Barcode.BarcodeTypes), theNode.ChildNodes[i].Attributes["value"].Value, false);
							else
								barcode.BarcodeType = (Barcode.BarcodeTypes)Enum.Parse(typeof(Barcode.BarcodeTypes), theNode.ChildNodes[i].InnerText, false);
						}
						catch (Exception){}
						break;
				}
			}
			
			if (!theBarcodes.Contains(barcode.Name))
				theBarcodes.Add(barcode.Name, theIndex);

			return barcode;
		}

		private Map resolveMap(XmlNode theNode,int theIndex)
		{
			Map map = new Map(theNode, this);
			
			XmlNodeList childNodes = theNode.ChildNodes;

			for (int i=0;i<childNodes.Count;i++)
			{
				switch (childNodes[i].Name)
				{
				
					case "foregroundColor":
						try
						{
							if (childNodes[i].Attributes["color"] != null)
								map.ForegroundColor = ResolveColor(childNodes[i].Attributes["color"].Value);
							else
								map.ForegroundColor = ResolveColor(childNodes[i].InnerText);
						}
						catch (Exception){}
						break;

					case "backgroundColor":
						try
						{
							if (childNodes[i].Attributes["color"] != null)
								map.BackgroundColor = ResolveColor(childNodes[i].Attributes["color"].Value);
							else
								map.BackgroundColor = ResolveColor(childNodes[i].InnerText);
						}
						catch (Exception){}
						break;

					case "mapType":
						if (childNodes[i].Attributes["value"] != null)
						{
							map.MapType = Map.GetMapType(childNodes[i].Attributes["value"].Value);
						}
						else
						{
							map.MapType = Map.GetMapType(childNodes[i].InnerText);
						}
						break;
					
					case "border":
						try
						{
							map.BorderColor = ResolveColor( childNodes[i].Attributes["color"].Value );
							map.BorderWidth = childNodes[i].Attributes["width"]==null ? 0 : Convert.ToInt32( childNodes[i].Attributes["width"].Value );
						}
						catch (Exception){}
						break;

					case "pdfExportQuality":
						try
						{
							map.ExportQuality = theNode.ChildNodes[i].Attributes["value"]==null ? 100 : Convert.ToInt32( theNode.ChildNodes[i].Attributes["value"].Value );
						}
						catch (Exception){}
						break;
							
					case "mapPoints":
						map.Landmarks = resolveMapPoints(childNodes[i]);
						break;	
						
				}
			}
			

			if (map.Name!=String.Empty && !theMaps.Contains(map.Name))
				theMaps.Add(map.Name, theIndex);

			return map;
		}


		private ChartBox resolveChartBox(XmlNode theNode,int theIndex)
		{
			ChartBox chartBox = new ChartBox(theNode, this);

			if (chartBox.Name==string.Empty)
				chartBox.Name = "chart0";			
			
			try
			{
				string chartType = theNode.Attributes["type"]==null ? "Bars" : theNode.Attributes["type"].Value;							
				chartBox.Type = (ChartBox.ChartType)Enum.Parse(typeof(ChartBox.ChartType), chartType, false); 
			}
			catch (Exception){}


			XmlNodeList childNodes = theNode.ChildNodes;

			for (int i=0;i<childNodes.Count;i++)
			{
				switch (childNodes[i].Name)
				{
					
					case "title":
						try
						{
							if (childNodes[i].Attributes["value"] != null)
								chartBox.Title = childNodes[i].Attributes["value"].Value;
							else
								chartBox.Title = childNodes[i].InnerText;
						}
						catch (Exception){}
						break;

					case "xLabel":
						try
						{
							if (childNodes[i].Attributes["value"] != null)
								chartBox.XLabel = childNodes[i].Attributes["value"].Value;
							else
								chartBox.XLabel = childNodes[i].InnerText ;
						}
						catch (Exception){}
						break;

					case "labelFont":
						try
						{							
							chartBox.LabelFont = resolveFont( childNodes[i] );
						}
						catch (Exception){}
						break;

					case "titleFont":
						try
						{							
							chartBox.TitleFont = resolveFont( childNodes[i] );
						}
						catch (Exception){}
						break;

					case "mapAreaColor":
						try
						{
							if (childNodes[i].Attributes["color"] != null)
								chartBox.MapAreaColor = ResolveColor(childNodes[i].Attributes["color"].Value);
							else
								chartBox.MapAreaColor = ResolveColor(childNodes[i].InnerText) ;
						}
						catch (Exception){}
						break;

					case "backgroundColor":
						try
						{
							if (childNodes[i].Attributes["color"] != null)
								chartBox.BackgroundColor = ResolveColor(childNodes[i].Attributes["color"].Value);
							else
								chartBox.BackgroundColor = ResolveColor(childNodes[i].InnerText) ;
						}
						catch (Exception){}
						break;

					case "showLegend":
						try
						{
							if (childNodes[i].Attributes["value"] != null)
								chartBox.ShowLegend = Convert.ToBoolean(childNodes[i].Attributes["value"].Value);
							else
								chartBox.ShowLegend = Convert.ToBoolean(childNodes[i].InnerText) ;
						}
						catch (Exception){}
						break;

					case "border":
						try
						{
							chartBox.BorderColor = ResolveColor( childNodes[i].Attributes["color"].Value );
							chartBox.BorderWidth = childNodes[i].Attributes["width"]==null ? 0 : Convert.ToInt32( childNodes[i].Attributes["width"].Value );
						}
						catch (Exception){}
						break;

					case "pdfExportQuality":
						try
						{
							chartBox.ExportQuality = theNode.ChildNodes[i].Attributes["value"]==null ? 100 : Convert.ToInt32( theNode.ChildNodes[i].Attributes["value"].Value );
						}
						catch (Exception){}
						break;

					case "chartData":
						try
						{
							resolveChartData(childNodes[i], chartBox);
						}
						catch (Exception){}
						break;

				}

			}

			if (!theCharts.Contains(chartBox.Name))
				theCharts.Add(chartBox.Name,theIndex);
			
			return chartBox;	
		}


		private Scatter resolveScatter(XmlNode theNode,int theIndex)
		{			
			Scatter chartBox = new Scatter(theNode, this);
			if (chartBox.Name == string.Empty)
				chartBox.Name = "scatter0";

			XmlNodeList childNodes = theNode.ChildNodes;

			for (int i=0;i<childNodes.Count;i++)
			{
				switch (childNodes[i].Name)
				{
					
					case "title":
						try
						{
							if (childNodes[i].Attributes["value"] != null)
								chartBox.Title = childNodes[i].Attributes["value"].Value;
							else
								chartBox.Title = childNodes[i].InnerText;
						}
						catch (Exception){}
						break;

					case "xLabel":
						try
						{
							if (childNodes[i].Attributes["value"] != null)
								chartBox.XLabel = childNodes[i].Attributes["value"].Value;
							else
								chartBox.XLabel = childNodes[i].InnerText ;
						}
						catch (Exception){}
						break;

					case "labelFont":
						try
						{							
							chartBox.LabelFont = resolveFont( childNodes[i] );
						}
						catch (Exception){}
						break;

					case "titleFont":
						try
						{							
							chartBox.TitleFont = resolveFont( childNodes[i] );
						}
						catch (Exception){}
						break;

					case "mapAreaColor":
						try
						{
							if (childNodes[i].Attributes["color"] != null)
								chartBox.MapAreaColor = ResolveColor(childNodes[i].Attributes["color"].Value);
							else
								chartBox.MapAreaColor = ResolveColor(childNodes[i].InnerText) ;
						}
						catch (Exception){}
						break;

					case "backgroundColor":
						try
						{
							if (childNodes[i].Attributes["color"] != null)
								chartBox.BackgroundColor = ResolveColor(childNodes[i].Attributes["color"].Value);
							else
								chartBox.BackgroundColor = ResolveColor(childNodes[i].InnerText) ;
						}
						catch (Exception){}
						break;

					case "showLegend":
						try
						{
							if (childNodes[i].Attributes["value"] != null)
								chartBox.ShowLegend = Convert.ToBoolean(childNodes[i].Attributes["value"].Value);
							else
								chartBox.ShowLegend = Convert.ToBoolean(childNodes[i].InnerText) ;
						}
						catch (Exception){}
						break;

					case "showMarkers":
						try
						{
							if (childNodes[i].Attributes["value"] != null)
								chartBox.ShowMarkers = Convert.ToBoolean(childNodes[i].Attributes["value"].Value);
							else
								chartBox.ShowMarkers = Convert.ToBoolean(childNodes[i].InnerText) ;
						}
						catch (Exception){}
						break;

					case "border":
						try
						{
							chartBox.BorderColor = ResolveColor( childNodes[i].Attributes["color"].Value );
							chartBox.BorderWidth = childNodes[i].Attributes["width"]==null ? 0 : Convert.ToInt32( childNodes[i].Attributes["width"].Value );
						}
						catch (Exception){}
						break;

					case "pdfExportQuality":
						try
						{
							chartBox.ExportQuality = theNode.ChildNodes[i].Attributes["value"]==null ? 100 : Convert.ToInt32( theNode.ChildNodes[i].Attributes["value"].Value );
						}
						catch (Exception){}
						break;

					case "scatterData":
						try
						{
							resolveScatterData(childNodes[i], chartBox);
						}
						catch (Exception){}
						break;

				}

			}

			if (!theScatters.Contains(chartBox.Name))
				theScatters.Add(chartBox.Name,theIndex);
			
			return chartBox;	
		}


		private StyledTableColumn[] resolveColumns(XmlNode theNode)
		{
			XmlNodeList columnNodes = theNode.ChildNodes;

			StyledTableColumn[] result = new StyledTableColumn[columnNodes.Count];
		
			for (int i=0;i<columnNodes.Count;i++)
			{
				result[i] = new StyledTableColumn();
				
				try
				{
					result[i].Name =  columnNodes[i].Attributes["name"].Value ;
				}
				catch (Exception){}

				try
				{
					result[i].Label =  columnNodes[i].Attributes["label"].Value ;
				}
				catch (Exception){}

				try
				{
					result[i].FormatMask =  columnNodes[i].Attributes["FormatMask"].Value;
				}
				catch (Exception){}


				try
				{
					result[i].Prefix =  columnNodes[i].Attributes["prefix"].Value;
				}
				catch (Exception){}

				try
				{
					string content =  columnNodes[i].Attributes["subtotalType"].Value;
					result[i].TotalType = (StyledTableColumn.SubtotalType)Enum.Parse(typeof(StyledTableColumn.SubtotalType), content, false);
				}
				catch (Exception){}

				try
				{
					result[i].Width =  Convert.ToInt32(columnNodes[i].Attributes["width"].Value) ;
				}
				catch (Exception){}

				try 
				{
					string content = columnNodes[i].Attributes["align"].Value;
					result[i].Alignment = (StyledTableColumn.AlignmentType)Enum.Parse(typeof(StyledTableColumn.AlignmentType), content, false);
				}
				catch (Exception)
				{
					result[i].Alignment = StyledTableColumn.AlignmentType.Left;
				}
				
				
			}
			return result;
		}


		private void CreateSubtable(DataTable masterTable, StyledTable styledTable, int length)
		{
			DataTable currentData = new DataTable();
			ArrayList segmentAlterationList = new ArrayList();			
			ArrayList conditionalRows = new ArrayList(styledTable.ConditionalRows);
			int start = styledTable.Printed;


			if ( styledTable.Columns.Length > 0 )
			{				
				for (int i=0;i<styledTable.Columns.Length;i++)
				{
					if ( masterTable.Columns.Contains(styledTable.Columns[i].Name) )
					{
						int ord = masterTable.Columns.IndexOf(styledTable.Columns[i].Name);
						currentData.Columns.Add(new DataColumn(masterTable.Columns[ord].ColumnName,masterTable.Columns[ord].DataType));
					}
					else
						throw new Exception("No such column "+ styledTable.Columns[i].Name.ToString());
				}


				for (int i=start;i<start+length;i++)
				{
					object[] newRow = new object[currentData.Columns.Count];
					for (int j=0;j<currentData.Columns.Count;j++)
					{
						int ord = masterTable.Columns.IndexOf(currentData.Columns[j].ColumnName);
						newRow[j] = styledTable.DataRows[i].ItemArray[ord];
					}
					
					DataRow row = currentData.Rows.Add(newRow);
					row.RowError = styledTable.DataRows[i].RowError;
					
					if (conditionalRows.Contains(styledTable.DataRows[i]))
						segmentAlterationList.Add(currentData.Rows[i-start]);
								
				}
				
			}
			else
			{

				for (int i=0;i<masterTable.Columns.Count;i++)
				{
					currentData.Columns.Add(new DataColumn(masterTable.Columns[i].ColumnName,masterTable.Columns[i].DataType));
				}

				for (int i=start;i<start+length;i++)
				{
					currentData.Rows.Add(styledTable.DataRows[i].ItemArray);
					if (conditionalRows.Contains(styledTable.DataRows[i]))
						segmentAlterationList.Add(currentData.Rows[i-start]);
				}
			}

			styledTable.Data = currentData;
			styledTable.AlterRows = segmentAlterationList;

		}

		#endregion

		#region Internal Methods

		/// <summary>
		/// Resolves vertical alignment type from text.
		/// </summary>
		internal ICustomPaint.VerticalAlignmentTypes resolveVerticalAlignment(string theAlignment)
		{	
			try 
			{
				return (ICustomPaint.VerticalAlignmentTypes)Enum.Parse(typeof(ICustomPaint.VerticalAlignmentTypes), theAlignment, false);	
			}
			catch(Exception) 
			{
				return ICustomPaint.VerticalAlignmentTypes.None;
			}						
		}


		internal ICustomPaint.HorizontalAlignmentTypes resolveHorizontalAlignment(string theAlignment)
		{
			try 
			{
				return (ICustomPaint.HorizontalAlignmentTypes)Enum.Parse(typeof(ICustomPaint.HorizontalAlignmentTypes), theAlignment, false);	
			}
			catch(Exception) 
			{
				return ICustomPaint.HorizontalAlignmentTypes.None;
			}
		}


		internal void Prepare(bool startPage)
		{
			foreach(ICustomPaint obj in objects)
			{
				obj.Prepare(startPage);
			}
		}

		internal int GetAvailableHeight(int offset)
		{
			return Math.Max(0, this.Document.PixelSize.Height - offset - this.Document.Margins.Bottom);
		}

		private ICustomPaint[] ArrangeDynamicObjects()
		{

			ArrayList tmp = new ArrayList();
			

			foreach(ICustomPaint obj in this.Chain)
			{
				tmp.Add(obj);
			}

			for (int i=0;i<this.objects.Length;i++)
			{
				if(!Chain.Contains(objects[i]))
				{
					tmp.Add(objects[i]);
				}
			}


			
			return (ICustomPaint[])tmp.ToArray(typeof(ICustomPaint));
			
		}

		/// <summary>
		/// Updates content for dynamic elements on the page.
		/// </summary>
		internal bool UpdateDynamicContent()
		{
			return UpdateDynamicContent(Document.GetGraphics());
		}

		internal bool UpdateDynamicContent(Graphics g)
		{
			return UpdateDynamicContent(g, false);
		}


		/// <summary>
		/// Updates content for dynamic elements on the page.
		/// </summary>
		private bool UpdateDynamicContent(Graphics g, bool forCounting)
		{
			bool printMore = false;
			
			ICustomPaint[] dynamicObjects = this.ArrangeDynamicObjects();

			for (int i=0;i<dynamicObjects.Length;i++)
			{
				bool printMoreTextField = false;
				bool printMoreRichTextField = false;
				bool printMoreTable = false;
				bool printMoreShift = false;


				if (dynamicObjects[i].Anchored)
				{
					if (dynamicObjects[i].Ready && !dynamicObjects[i].Displayed)
					{
						ICustomPaint anchor = dynamicObjects[i].Anchor;
						if (anchor != null)
						{
							int xOffset = dynamicObjects[i].X - anchor.X;
							int yOffset = dynamicObjects[i].Y - anchor.Y - anchor.Height;
							if (anchor.Bounds.Height > 0)
							{
								dynamicObjects[i].Bounds.X = anchor.Bounds.Left + xOffset;
								dynamicObjects[i].Bounds.Y = anchor.Bounds.Bottom + yOffset;
							}

							int availableHeight = this.GetAvailableHeight(dynamicObjects[i].Bounds.Y);
							if (dynamicObjects[i].CanStretch())
							{
								dynamicObjects[i].Bounds.Height = availableHeight;
							}

						}
					}
					else
					{
						dynamicObjects[i].Bounds.Height = 0;
						continue;
					}
				}

				if ( dynamicObjects[i] != null )
				{
					if ( dynamicObjects[i] is TextField )
					{						
						TextField textField = dynamicObjects[i] as TextField;

						if (textField.Anchored)
						{
							if (!textField.CanStretch())
							{
								printMoreShift = CheckShifting(textField);		
							}

							if (textField.Ready)
							{
								if (textField.OverflowTextHandling == TextField.OverflowStyle.Display)
									printMoreTextField = textField.CreateSubText(g, false, false, true);
								else
									textField.CreateSubText(g, true, true, false);

								textField.Done = !printMoreTextField;
							}
						}
						else
						{

							if (textField.Layout == ICustomPaint.LayoutTypes.EveryPage)
							{
								if (textField.OverflowTextHandling == TextField.OverflowStyle.Display)
									printMoreTextField = textField.CreateSubText(g, false, false, false);
								else
									textField.CreateSubText(g, true, true, false);
							}
							else
							{
								textField.CreateSubText(g, true, true, false);
								textField.ResetText();
							}
						}

					}
					else if ( dynamicObjects[i] is RichTextField )
					{						
						RichTextField richTextField = dynamicObjects[i] as RichTextField;
						if (richTextField.Anchored)
						{
							if (!richTextField.CanStretch())
							{
								printMoreShift = CheckShifting(richTextField);		
							}

							if (richTextField.Ready)
							{
								if (richTextField.OverflowTextHandling == TextField.OverflowStyle.Display)
									printMoreRichTextField = richTextField.CreateLines(g, false, true);
								else
									richTextField.CreateLines(g, true, false);

								richTextField.Done = !printMoreRichTextField;
							}
						}
						else
						{

							if (richTextField.Layout == ICustomPaint.LayoutTypes.EveryPage)
							{
								if (richTextField.OverflowTextHandling == TextField.OverflowStyle.Display)
									printMoreRichTextField = richTextField.CreateLines(g, false, false);
								else
								{
									richTextField.CreateLines(g, true, false);
									richTextField.ResetText();
								}
							}
							else
							{
								richTextField.CreateLines(g, true, false);
								richTextField.ResetText();
							}
						}

					}
					else if ( dynamicObjects[i] is StyledTable)
					{
						StyledTable tempTable = (StyledTable)dynamicObjects[i];
						if (tempTable.HasDataSource())
						{

							if (theTables.Contains(tempTable.DataSource))
							{
								string theTableName = tempTable.DataSource;
								DataTable podaci = (DataTable)theTables[theTableName];

								try
								{
									// perform filtering on first pass 
									if (tempTable.Printed==0)
									{
										if (tempTable.AlterDataCondition != String.Empty)
										{
											try
											{
												tempTable.ConditionalRows = podaci.Select(tempTable.AlterDataCondition);
											}
											catch (Exception)
											{
												tempTable.ConditionalRows = new DataRow[0]{};
											}
										}
										else
										{
											tempTable.ConditionalRows = new DataRow[0]{};
										}
									}

									DataRow[] printRows = tempTable.DataRows;

									int relativeHeaderHeight = tempTable.CalculateRelativeHeaderHeight(g);
									int rowsForPrint = 0;
									int relativeHeight = 0;
									int relativeDataRowHeight = 0;
									int maxRows = tempTable.GetPossibleRowNumber();

									// find out how many succeeding rows will fit into table area
									//taking into account grouping because that will inject another header row
									do
									{
										relativeDataRowHeight = 0;
										if (printRows.Length <= tempTable.Printed + rowsForPrint )
											break;

										DataRow nextRow = printRows[tempTable.Printed + rowsForPrint];

										relativeDataRowHeight = tempTable.CalculateRelativeDataRowHeight(nextRow, g); 
										if (relativeHeaderHeight+relativeHeight+relativeDataRowHeight <= maxRows)
										{
											relativeHeight += relativeDataRowHeight;
											relativeDataRowHeight = 0;
											rowsForPrint++;
										}
									}
									while (relativeHeaderHeight+relativeHeight+relativeDataRowHeight <= maxRows);
					
									// create subtable for printing
									
									CreateSubtable(podaci, tempTable, rowsForPrint);									
									tempTable.Printed = rowsForPrint + tempTable.Printed;

									
									if (tempTable.Anchored && maxRows>0)
										//tempTable.Bounds.Height = tempTable.CellHeight * (relativeHeaderHeight+relativeHeight+relativeDataRowHeight);
										tempTable.Bounds.Height = tempTable.CellHeight * (relativeHeaderHeight+relativeHeight);


									// if there are more rows, go on with printing
									if (printRows.Length > 0 && rowsForPrint==0 && !tempTable.Anchored)
									{
										printMoreTable = false;
									}
									else
									{
										if (printRows.Length > tempTable.Printed)
											printMoreTable = true;
									}
								}
								catch (Exception e)
								{
									// print exception text in table header
									printMoreTable = false;
									StyledTableColumn[] kolone = new StyledTableColumn[1];
									kolone[0] = new StyledTableColumn();
									kolone[0].Label = e.Message;
									tempTable.Columns = kolone;
								}
							}
							else
							{
								try
								{
									int relativeHeaderHeight = tempTable.CalculateRelativeHeaderHeight(g);
									if (tempTable.Anchored)
										tempTable.Bounds.Height = tempTable.CellHeight * relativeHeaderHeight;
								}
								catch(Exception){}

							}

							tempTable.Done = !printMoreTable;
						}
						else
						{
							tempTable.CreateDisplayTable();

							if (tempTable.Anchored)
							{								
								printMoreShift = CheckShifting(tempTable);
							}

							tempTable.Done = !printMoreShift;
						}

						
					}
					else
					{
						if (dynamicObjects[i].Anchored)
						{
							int availableHeight = this.GetAvailableHeight(dynamicObjects[i].Bounds.Y);

							if (dynamicObjects[i].Bounds.Height > availableHeight)
							{
								dynamicObjects[i].Bounds.Height = 0;
								dynamicObjects[i].Shifted = true;
								printMoreShift = true;
							}
							else
							{

								if (dynamicObjects[i].Ready)
									dynamicObjects[i].Done = true;	
							}
						}
						else
						{
							if (dynamicObjects[i].Ready)
								dynamicObjects[i].Done = true;	
						}
					}

					if (forCounting)
					{
						if (dynamicObjects[i].Ready && !dynamicObjects[i].Displayed)
						{													
							if (dynamicObjects[i].Done)
								dynamicObjects[i].Displayed = true;
								
						}
					}



					printMore = printMoreTextField || printMoreRichTextField || printMoreTable || printMoreShift || printMore;
				}
			}
			return printMore;
		}

		internal int CalculateNumberOfPages(Graphics g)
		{
			int result = 0;
			
			bool more = true;
			while(more)
			{
				result++;
				Prepare(result==1);
				more = UpdateDynamicContent(g, true);
			}

			Reset();
			return result;
		}

		private bool CheckShifting(ICustomPaint element)
		{
			int availableHeight = this.GetAvailableHeight(element.Bounds.Y);
			int areaHeight = this.GetAvailableHeight(this.Document.Margins.Top);

			if (element.Anchor == null)
			{
				return false;
			}
			else
			{
				if (element.Bounds.Height > areaHeight)
				{
					if (element.Bounds.Y == this.Document.Margins.Top)
					{
						return false;
					}
					else
					{
						element.Bounds.Height = 0;
						element.Shifted = true;
						return true;
					}
				}
				else
				{
					if (element.Bounds.Height > availableHeight)
					{
						element.Bounds.Height = 0;
						element.Shifted = true;
						return true;
					}
					else
					{
						return false;	
					}
				}
			}
		}

		/// <summary>
		/// Meyhod calculates number of pages in this section
		/// </summary>
		 /*
		internal int CalculateNumberOfPages1(Graphics g)
		{
			int result = 1;
			ICustomPaint[] dynamicObjects = this.DynamicObjects;

			for (int i=0;i<dynamicObjects.Length;i++)
			{
				if ( dynamicObjects[i] != null && dynamicObjects[i] is StyledTable)
				{
					StyledTable tempTable = (StyledTable)dynamicObjects[i];
					if ( tempTable.DataSource != null && theTables.Contains(tempTable.DataSource) )
					{

						string theTableName = tempTable.DataSource;
						DataTable podaci = (DataTable)theTables[theTableName];

						
						DataRow[] printRows = tempTable.DataRows;

						bool hasMore = false;
						int counted = 0;
						int numPages = 0;

						do
						{
							this.Prepare(numPages == 0);

							int relativeHeaderHeight = tempTable.CalculateRelativeHeaderHeight(g);
							int rowsForPrint = 0;
							int relativeHeight = 0;
							int relativeDataRowHeight = 0;

							// find out how many succeeding rows will fit into table area
							do
							{
								if (printRows.Length <= counted + rowsForPrint )
									break;

								DataRow nextRow = printRows[counted + rowsForPrint];
								relativeDataRowHeight = tempTable.CalculateRelativeDataRowHeight(nextRow,g); 
								if (relativeHeaderHeight+relativeHeight+relativeDataRowHeight <= tempTable.GetPossibleRowNumber())
								{
									relativeHeight += relativeDataRowHeight;
									rowsForPrint++;
								}
							}
							while (relativeHeaderHeight+relativeHeight+relativeDataRowHeight<=tempTable.GetPossibleRowNumber());
																				
							counted += rowsForPrint;
							numPages++;

							if (printRows.Length > 0 && rowsForPrint==0)
							{
								hasMore = false;
							}
							else
							{
								hasMore = printRows.Length > counted;
							}
							
						}
						while (hasMore);
						
						result = Math.Max(result, numPages);
					}
				}
				else if ( dynamicObjects[i] != null && dynamicObjects[i] is TextField)
				{
					TextField textField = dynamicObjects[i] as TextField;

					int numPages = 1;

					if (textField.Layout == ICustomPaint.LayoutTypes.EveryPage)
					{
						bool more = true;
						while (more)
						{
							this.Prepare(numPages == 1);
							bool forceResize = textField.Anchored && textField.OverflowTextHandling == TextField.OverflowStyle.Display;
							more = textField.CreateSubText(g, false, true, forceResize);
							if (more)
								numPages++;
						}
					}

					textField.ResetText();
					result = Math.Max(result,numPages);

				}
			}
			return result;
		}
		*/

		/// <summary>
		/// Gets the value for given section parameter.
		/// </summary>
		internal string GetParameterValue(string parameterName)
		{
			if (parameterValues.ContainsKey(parameterName))
			{
				return parameterValues[parameterName] != null ? parameterValues[parameterName].ToString() : String.Empty;
			}
			else
			{
				return this.document.GetParameterValue(parameterName);	
			}			
		}

		/// <summary>
		/// Resolves parameter values during printing.
		/// </summary>
		internal string ResolveParameterValues(string input)
		{
			string buffer = "";
			int pos = -1;
			int oldPos = 0;

			while( (pos=input.IndexOf("$P",oldPos)) != -1 )
			{

				buffer += input.Substring(oldPos,pos-oldPos);
				if ( input.Substring(pos+2,1).Equals("{") && input.IndexOf("}",pos+2) != -1 )
				{
					string parameterName = input.Substring(pos+3,input.IndexOf("}",pos+2)-pos-3).Trim();
					buffer += GetParameterValue(parameterName);
					
					oldPos = input.IndexOf("}",pos+2) + 1;
				}
				else
				{				
					oldPos = pos+2;
				}
			}

			buffer += input.Substring(oldPos);

			return buffer;
		}

		#endregion

		#region Public Methods


		/// <summary>
		/// Gets report element with given Name property; returs null if no such element is found
		/// </summary>
		public ICustomPaint FindElement(string elementName)
		{
			foreach(ICustomPaint obj in this.objects)
			{
				if (obj.Name == elementName)
					return obj;
			}

			return null;
		}

		/// <summary>
		/// Gets the string representation of report element.
		/// </summary>
		public override string ToString()
		{
			return "Section " + (this.Name==string.Empty ? "" : "[" + this.Name + "]") ;			
		}

		/// <summary>
		/// Adds a DataTable to the collection
		/// </summary>
		/// <remarks>The DataTable.Name must match the dataSource property in the report definition for a
		/// dynamic table</remarks>
		/// <param name="newTable">System.Data.DataTable: source data for a dynamic table</param>
		public void AddData(DataTable newTable)
		{		
			try
			{
				if (theTables.Contains(newTable.TableName))
				{
					theTables.Remove(newTable.TableName);					
				}

				theTables.Add(newTable.TableName, newTable);				
			}
			catch (Exception){}

		}

		/// <summary>
		/// Adds a DataTable to the named StyledTableElement an a static fashion.
		/// </summary>
		public void AddStaticData(string tableName, DataTable newTable)
		{
			if ( theTablesByName.Contains(tableName) ) 
			{
				int theIndex = (int)theTablesByName[tableName];
				StyledTable styledTable = objects[theIndex] as StyledTable;
				styledTable.SetStaticData(newTable);
			}
		}

		/// <summary>
		/// Sets the categories for the specified chart.
		/// </summary>
		/// <param name="chartName"><see cref="Com.Delta.PrintManager.Engine.ChartBox">Com.Delta.PrintManager.Engine.ChartBox</see> to set categories for.</param>
		/// <param name="categories">string array of categories (category names).</param>
		public void SetChartCategories(string chartName, string[] categories)
		{			
			if ( theCharts.Contains(chartName) ) 
			{
				int theIndex = (int)theCharts[chartName];
				ChartBox chartBox = objects[theIndex] as ChartBox;
				chartBox.Categories = categories;	
			}
			
		}

		/// <summary>
		/// Adds data serie to the set of series of the named chart.
		/// </summary>
		/// <param name="chartName">The name of the chart to add the serie to</param>
		/// <param name="serieName">Name of the serie. Displayed in the Legend</param>
		/// <param name="values">A array of Double values</param>
		/// <param name="color">Color of the bar/pie</param>
		public void AddChartSerie(string chartName,string serieName,double[] values,Color color)
		{
			
			if ( theCharts.Contains(chartName) ) 
			{
				int theIndex = (int)theCharts[chartName];
				ChartBox chartBox = objects[theIndex] as ChartBox;
				chartBox.AddSerie(serieName, values, color);
			}
			
		}

		/// <summary>
		/// Adds data serie to the set of series of the named chart at the specified index.
		/// </summary>
		public void AddChartSerie(string chartName, string serieName, double[] values, Color serieColor, int index)
		{
			
			if ( theCharts.Contains(chartName) ) 
			{
				int theIndex = (int)theCharts[chartName];
				ChartBox chartBox = objects[theIndex] as ChartBox;
				chartBox.AddSerie(serieName, values, serieColor, index);
			}
			
		}


		/// <summary>
		/// This method is used add data serie to the Scatter element in a dynamic fashion.
		/// </summary>
		/// <param name="scatterName">The name of the scatter diagram to add the serie to</param>
		/// <param name="serieName">Descriptive name of this data serie (displayed in chart legend)</param>
		/// <param name="x">Data X values</param>
		/// <param name="y">Data Y values</param>
		/// <param name="color">color used to display data serie</param>
		public void AddScatterSerie(string scatterName, string serieName, double[] x, double[] y, Color color)
		{			
			if ( theScatters.Contains(scatterName) ) 
			{
				int index = (int)theScatters[scatterName];
				Scatter scatter = objects[index] as Scatter;
				scatter.AddSerie(serieName, x, y, color);
			}
			
		}


		/// <summary>
		/// This method is used for setting PictureBox image content in a dynamic fashion.
		/// </summary>
		/// <param name="pictureName">The name of the pictureBox to attach bitmap to.</param>
		/// <param name="image">Bitmap to be set</param>
		public void AddPicture(string pictureName, Bitmap image)
		{			
			AddPicture(pictureName, image, false);			
		}

		/// <summary>
		/// This method is used for setting TextField content in a dynamic fashion.
		/// </summary>
		public void AddText(string textFieldName, string text)
		{			
			if ( theTextFields.Contains(textFieldName) ) 
			{
				int theIndex = (int)theTextFields[textFieldName];
				TextField textField = objects[theIndex] as TextField;
				textField.Text = text;
			}			
		}


		/// <summary>
		/// This method is used for setting RichText content in a dynamic fashion.
		/// </summary>
		public void AddRichText(string richTextFieldName, string text)
		{			
			if ( theRichTextFields.Contains(richTextFieldName) ) 
			{
				int theIndex = (int)theRichTextFields[richTextFieldName];
				RichTextField richTextField = objects[theIndex] as RichTextField;
				richTextField.Text = text;
			}			
		}

		/// <summary>
		/// This method is used for setting PictureBox image content in a dynamic fashion.
		/// </summary>
		/// <param name="pictureName">The name of the pictureBox to attach bitmap to.</param>
		/// <param name="image">Bitmap to be set.</param>
		/// <param name="stretch">stretch image to fit PictureBox area.</param>
		public void AddPicture(string pictureName, Bitmap image, bool stretch)
		{			
			if ( thePictures.Contains(pictureName) ) 
			{
				int theIndex = (int)thePictures[pictureName];
				PictureBox pictureBox = objects[theIndex] as PictureBox;
				pictureBox.SetPicture(image);
				pictureBox.Stretch = stretch; 
			}			
		}

		/// <summary>
		/// This method is used for setting map points in a dynamic fashion.
		/// </summary>
		public void AddMapData(string mapName, Map.MapPoint[] points)
		{
			if ( theMaps.Contains(mapName) ) 
			{
				int theIndex = (int)theMaps[mapName];
				Map map = objects[theIndex] as Map;
				map.Landmarks = points;
			}
		}

		/// <summary>
		/// This method is used for setting map data in a dynamic fashion (both map type and map points).
		/// </summary>
		public void AddMapData(string mapName, Map.MapPoint[] points, Map.MapTypes type)
		{
			if ( theMaps.Contains(mapName) ) 
			{
				int theIndex = (int)theMaps[mapName];
				Map map = objects[theIndex] as Map;
				map.MapType = type;
				map.Landmarks = points;
			}
		}

		/// <summary>
		/// This method is used to set the Timeline element data in a dynamic fashion.
		/// </summary>
		public void AddTimelineData(string timelineName, TimelinePoint[] points, TimelinePeriod[] periods)
		{			
			if ( theTimelines.Contains(timelineName) ) 
			{
				int theIndex = (int)theTimelines[timelineName];
				Timeline timeline = objects[theIndex] as Timeline;
				timeline.Events = points;
				timeline.Periods = periods;
			}			
		}

		/// <summary>
		/// This method is used for setting barcode data in a dynamic fashion (text content only).
		/// </summary>
		public void AddBarcode(string barcodeName, string barcodeData)
		{
			if ( theBarcodes.Contains(barcodeName) ) 
			{
				int theIndex = (int)theBarcodes[barcodeName];
				Barcode barcode = objects[theIndex] as Barcode;
				barcode.Text = barcodeData;
			}
		}

		/// <summary>
		/// This method is used for setting barcode data in a dynamic fashion (both text content and barcode type).
		/// </summary>
		public void AddBarcode(string barcodeName, string barcodeData, Barcode.BarcodeTypes barcodeType)
		{
			if ( theBarcodes.Contains(barcodeName) ) 
			{
				int theIndex = (int)theBarcodes[barcodeName];
				Barcode barcode = objects[theIndex] as Barcode;
				barcode.BarcodeType = barcodeType;
				barcode.Text = barcodeData;
			}
		}


		/// <summary>
		/// This method is used for setting barcode data in a dynamic fashion (text content only).
		/// </summary>
		public void AddUserPaint(string userPaintName, UserPaint userPaint, bool overrideTemplateSize)
		{
			if ( theUserPaints.Contains(userPaintName) ) 
			{
				int theIndex = (int)theUserPaints[userPaintName];
				UserPaint placeHolder = objects[theIndex] as UserPaint;

				userPaint.Section = placeHolder.Section;
				userPaint.X = placeHolder.X;
				userPaint.Y = placeHolder.Y;
				if (!overrideTemplateSize)
				{
					userPaint.Width = placeHolder.Width;
					userPaint.Height = placeHolder.Height;
				}
				userPaint.Name = placeHolder.Name;
				userPaint.Layout = placeHolder.Layout;
				userPaint.Selectable = placeHolder.Selectable;

				userPaint.HorizontalAlignment = placeHolder.HorizontalAlignment;
				userPaint.VerticalAlignment = placeHolder.VerticalAlignment;

				objects[theIndex] = userPaint;

			}
		}

		/// <summary>
		/// Resets all page counters before printing.
		/// </summary>
		internal void Reset()
		{
			
			foreach(ICustomPaint obj in this.DynamicObjects)
			{
				if (obj is StyledTable)
				{
					StyledTable styledTable = obj as StyledTable;
					styledTable.Printed = 0;
					if (styledTable.HasDataSource())
					{
						if (theTables.Contains(styledTable.DataSource) && theTables[styledTable.DataSource] is DataTable)
						{

							DataTable podaci = theTables[styledTable.DataSource] as DataTable;
							if (styledTable.FilterExpression != String.Empty || styledTable.SortExpression != String.Empty)
							{
								try
								{
									string filterExpression = this.Document.resolveParameterValues(styledTable.FilterExpression);
									string sortExpression = this.Document.resolveParameterValues(styledTable.SortExpression);
									styledTable.DataRows = podaci.Select(filterExpression, sortExpression);
								}
								catch(Exception)
								{
									DataRow[] allRows = new DataRow[podaci.Rows.Count];
									podaci.Rows.CopyTo(allRows, 0);
									styledTable.DataRows = allRows;
								}
							}
							else
							{
								DataRow[] allRows = new DataRow[podaci.Rows.Count];
								podaci.Rows.CopyTo(allRows, 0);
								styledTable.DataRows = allRows;
							}

							if (styledTable.HasSubtotals)
							{								
								this.CreateSubtotals((DataTable)theTables[styledTable.DataSource], styledTable);
							}
						}

					}
				}
				else if (obj is TextField)
				{
					TextField textField = obj as TextField;
					textField.FullReset();
				}
				else if (obj is RichTextField)
				{
					RichTextField richTextField = obj as RichTextField;
					richTextField.ResetText();
				}
			}

			for (int i=0;i<objects.Length;i++)
			{
				objects[i].Shifted = false;
				objects[i].Done = false;
				objects[i].Displayed = false;
			}

		}


		internal object Clone()
		{
			Section tmp = new Section(this.document);
			tmp.Name = this.Name;

			ICustomPaint[] objectClones = new ICustomPaint[this.objects.Length];
			for (int i=0;i<this.objects.Length;i++)
			{
				ICustomPaint c  = this.objects[i];
				objectClones[i] = (ICustomPaint)c.Clone();
				objectClones[i].Name = c.Name;
				objectClones[i].Section = tmp;
				
				if (objectClones[i] is PictureBox)
				{
					tmp.thePictures[objectClones[i].Name] = i;
				}
				else if (objectClones[i] is Timeline)
				{
					tmp.theTimelines[objectClones[i].Name] = i;
				}
				else if (objectClones[i] is Barcode)
				{
					tmp.theBarcodes[objectClones[i].Name] = i;
				}
				else if (objectClones[i] is ChartBox)
				{
					tmp.theCharts[objectClones[i].Name] = i;
				}
				else if (objectClones[i] is Scatter)
				{
					tmp.theScatters[objectClones[i].Name] = i;
				}
				else if (objectClones[i] is TextField)
				{
					tmp.theTextFields[objectClones[i].Name] = i;
				}
				else if (objectClones[i] is Map)
				{
					tmp.theMaps[objectClones[i].Name] = i;
				}
				else if (objectClones[i] is StyledTable)
				{
					tmp.theTablesByName[objectClones[i].Name] = i;
				}
				else if (objectClones[i] is RichTextField)
				{
					tmp.theRichTextFields[objectClones[i].Name] = i;
				}
				else if (objectClones[i] is UserPaint)
				{
					tmp.theUserPaints[objectClones[i].Name] = i;
				}
			}

			tmp.objects = objectClones;

			return tmp;
		}

		/// <summary>
		/// Sets report parameters on section level.
		/// </summary>
		public void SetParameters(Hashtable parameters)
		{
			if (parameters != null)
				parameterValues = parameters;
			else
				parameterValues = new Hashtable();
		}

		/// <summary>
		/// Clears all table data (set through AddData() method) from this section. 
		/// </summary>
		public void ClearTables()
		{
			ArrayList tabs = new ArrayList(theTables.Keys);
			foreach (string tableName in tabs)
			{
				if (theTables[tableName] is DataTable)
				{					
					theTables[tableName] = tableName;
				}

			}
		}

		/// <summary>
		/// Clears all chart data (both categories and series) from the named chart.
		/// </summary>
		public void ClearChart(string chartName)
		{
			if (theCharts.Contains(chartName))
			{
				int index = (int)theCharts[chartName];
				ChartBox chartBox = objects[index] as ChartBox;
				chartBox.Clear();
			}
		}

		#endregion


	}
}
