

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Printing;
using System.Data;
using System.Xml;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Reflection;


namespace Com.Delta.PrintManager.Engine
{
	#region Event Delegate Declarations

	/// <summary>
	/// Represents the method that will handle the MarginsChanged event.
	/// </summary>
	/// <param name="sender">The source of the event. </param>
	/// <remarks>
	/// When you create a MarginsChangedHandler delegate, you identify the method that will handle the event.
	/// To associate the event with your event handler, add an instance of the delegate to the event. The event
	/// handler is called whenever the event occurs, until you remove the delegate.
	/// Note: The declaration of your event handler must have the same parameters as the
	/// MarginsChangedHandler delegate declaration.
	/// </remarks>
	public delegate void MarginsChangedHandler(object sender);

	/// <summary>
	/// Represents the method that will handle the PageSizeChanged event.
	/// </summary>
	/// <param name="sender">The source of the event. </param>
	/// <remarks>
	/// When you create a PageSizeChangedHandler delegate, you identify the method that will handle the event.
	/// To associate the event with your event handler, add an instance of the delegate to the event. The event
	/// handler is called whenever the event occurs, until you remove the delegate.
	/// Note: The declaration of your event handler must have the same parameters as the
	/// PageSizeChangedHandler delegate declaration.
	/// </remarks>
	public delegate void PageSizeChangedHandler(object sender);

	/// <summary>
	/// Represents the method that will handle the PageLayoutChanged event.
	/// </summary>
	/// <param name="sender">The source of the event. </param>
	/// <remarks>
	/// When you create a PageLayoutChangedHandler delegate, you identify the method that will handle the event.
	/// To associate the event with your event handler, add an instance of the delegate to the event. The event
	/// handler is called whenever the event occurs, until you remove the delegate.
	/// Note: The declaration of your event handler must have the same parameters as the
	/// PageLayoutChangedHandler delegate declaration.
	/// </remarks>
	public delegate void PageLayoutChangedHandler(object sender);

	#endregion

	/// <summary>
	/// Class representing the printing document.
	/// </summary>
	/// <remarks>The ReportDocument parses the XML template files and produces the report. The designer
	/// application also uses it to parse all the document objects and place them on the designer pane</remarks>
	[DefaultProperty("Layout")]
	public sealed class ReportDocument : System.Drawing.Printing.PrintDocument
	{
		#region Declarations

		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Occurs when document margins have changed in design mode.
		/// </summary>
		public event MarginsChangedHandler OnMarginsChanged;
		
		/// <summary>
		/// Occurs when document page size has changed in design mode.
		/// </summary>
		public event PageSizeChangedHandler OnPageSizeChanged;
		
		/// <summary>
		/// Occurs when document page layout has changed in design mode.
		/// </summary>
		public event PageLayoutChangedHandler OnPageLayoutChanged;
		private bool designMode = false;

		private Paper.Type paperType = Paper.Type.A4;

		private double customWidth = 210;
		private double customHeight = 297;
		private Units customUnit = Units.Millimeters;
		
		private string mDocRoot = "";
		private XmlDocument xmlDoc;
		private bool isXmlOK = true;
		private string theErrorMessage;

		private ArrayList sections = new ArrayList();
		private Section printingSection;

		private ArrayList declaredParameters;
		private Hashtable parameterValues;
		private Hashtable defaultParameterValues;

		private Hashtable systemValues;
		private int[] sectionPageNumbers = new int[]{};
		private int sectionIndex = 0;
		
		private Hashtable textFields = new Hashtable();
		private Hashtable richTextFields = new Hashtable();
		private Hashtable pictures = new Hashtable();
		internal Hashtable tables = new Hashtable();
		private Hashtable tablesByName = new Hashtable();
		private Hashtable charts = new Hashtable();
		private Hashtable timelines = new Hashtable();
		private Hashtable barcodes = new Hashtable();
		private Hashtable maps = new Hashtable();
		private Hashtable scatters = new Hashtable();
		private Hashtable userPaints = new Hashtable();
		private ArrayList queries = new ArrayList();

		private IDbConnection databaseConnection = null;
		private StartInfo startInfo = new StartInfo();
		private ConnectionInfo connectionInfo = new ConnectionInfo();
        
		private Graphics graphics;
		private Margins documentMargins = new Margins(50,50,50,50);
		


		/// <summary>
		/// Enumeration of report layout styles.
		/// </summary>
		public enum LayoutType
		{
			/// <summary>Portrait layout.</summary>
			Portrait=0,
			/// <summary>Landscape layout.</summary>
			Landscape
		};


		/// <summary>
		/// Specifies the unit for custom report size.
		/// </summary>
		public enum Units
		{
			/// <summary>Report custom size in pixels.</summary>
			Pixels=0,
			/// <summary>Report custom size in inches.</summary>
			Inches,
			/// <summary>Report custom size in millimeters.</summary>
			Millimeters
		}
		
		#endregion

		#region Public Methods

		/// <summary>
		/// Clears all document table data set by ReportDocument.AddData() method.
		/// </summary>
		public void ClearTables()
		{
			foreach(Section section in sections)
			{
				section.ClearTables();
			}
		}

		/// <summary>
		/// Resets system variables before print
		/// </summary>
		internal void StartPrinting()
		{
			this.DefaultPageSettings.Margins = documentMargins;
			if (startInfo.PreloadData)
			{
				PreloadData();
			}

			foreach(Section section in sections)
			{
				section.Reset();
			}

			sectionIndex = 0;
			systemValues["pageNumber"] = 0 ;
			systemValues["sectionPageNumber"] = 0 ;
			systemValues["totalPages"] = calculateNumberOfPages(GetGraphics()) ;

			if (sectionIndex<sectionPageNumbers.Length)
				systemValues["sectionTotalPages"] = sectionPageNumbers[sectionIndex] ;

			

		}


		/// <summary>
		/// Updates pageNumber in the ReportDocument.
		/// </summary>
		internal void NewPage()
		{
			systemValues["pageNumber"] = (int)systemValues["pageNumber"] + 1 ;
			systemValues["sectionPageNumber"] = (int)systemValues["sectionPageNumber"] + 1 ;

			int secPgNo = (int)systemValues["sectionPageNumber"];
			
			if (printingSection != null)
				printingSection.Prepare(secPgNo==1);
		}


		/// <summary>
		/// Updates sectionPageNumber in the ReportDocument.
		/// </summary>
		internal void NewSection()
		{
			sectionIndex++;
			systemValues["sectionPageNumber"] = 0 ;

			if (sectionIndex<sectionPageNumbers.Length)
				systemValues["sectionTotalPages"] = sectionPageNumbers[sectionIndex];
		}

		/// <summary>
		/// Clones the named Section into n instances.
		/// </summary>
		/// <param name="sectionName">specifies Section to be cloned.</param>
		/// <param name="n">number of clones</param>
		public Section[] RepeatSection(string sectionName, int n)
		{
			for (int i=0;i<n;i++)
			{
				RepeatSection(sectionName);
			}

			return GetSections(sectionName);
		}

		/// <summary>
		/// This method duplicates the section identified by sectionName.
		/// </summary>
		/// <returns>
		/// The clone of the report Section identified by sectionName.
		/// </returns>
		public Section RepeatSection(string sectionName)
		{
			Section sourceSection = null;
			Section targetSection = null;
			int index = -1;
			for (int i=0;i<this.sections.Count;i++)
			{
				if (((Section)sections[i]).Name == sectionName && sourceSection==null)
				{
					sourceSection = sections[i] as Section;
				}
				else
				{
					if (sourceSection!=null)
					{
						index = i;
						break;
					}			
				}

			}

			if (index > -1 && sourceSection!=null)
			{
				targetSection = (Section)sourceSection.Clone();
				sections.Insert(index, targetSection);
			}
			else if (index == -1 && sourceSection!=null)
			{
				targetSection = (Section)sourceSection.Clone();
				sections.Add(targetSection);
			}
			

			return targetSection;
		}


		/// <summary>
		/// Returns the Graphics object for measurement purposes.
		/// </summary>
		internal Graphics GetGraphics()
		{
			return graphics;
		}


		/// <summary>
		/// Sets the database connection info used for data preloading.
		/// </summary>
		public void SetConnection(ConnectionInfo connectionInfo)
		{
			this.connectionInfo = connectionInfo;
		}

		/// <summary>
		/// Sets the database connection info used for data preloading.
		/// </summary>
		public void SetConnection(string assembly, string type, string connectionString)
		{
			this.connectionInfo = new ConnectionInfo(assembly, type, connectionString);
		}

		/// <summary>
		/// A serie is used to populate the ChartBox.
		/// </summary>
		/// <param name="chartName">The name of the chart to add the serie to</param>
		/// <param name="serieName">Name of the serie. Displayed in the Legend</param>
		/// <param name="Values">A array of Double values</param>
		/// <param name="serieColor">Color of the bar/pie</param>
		/// <remarks>
		/// This method is used to a serie to a specific chart in the current DaPrintDocument object.
		/// See <see cref="Com.Delta.PrintManager.Engine.ChartBox.AddSerie">Com.Delta.PrintManager.Engine.ChartBox.AddSerie</see> for an example
		/// of using the method</remarks>
		public void AddChartSerie(string chartName,string serieName,double[] values,Color serieColor)
		{	
			if ( charts.Contains(chartName) ) 
			{
				Section section = (Section)charts[chartName];
				section.AddChartSerie(chartName, serieName, values, serieColor);
			}		
		}


		/// <summary>
		/// Adds data serie to the set of series at the specified index.
		/// </summary>
		public void AddChartSerie(string chartName,string serieName,double[] values,Color serieColor, int index)
		{	
			if ( charts.Contains(chartName) ) 
			{
				Section section = (Section)charts[chartName];
				section.AddChartSerie(chartName, serieName, values, serieColor, index);
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
			if ( scatters.Contains(scatterName) ) 
			{
				Section section = (Section)scatters[scatterName];
				section.AddScatterSerie(scatterName, serieName, x, y, color);
			}		
		}


		/// <summary>
		/// Adds a DataTable to the collection
		/// </summary>
		/// <remarks>The DataTable.Name must match the dataSource property in the report definition for a
		/// dynamic table</remarks>
		/// <param name="newTable">System.Data.DataTable: source data for a dynamic table</param>
		public void AddData(DataTable newTable)
		{
			if (tables.Contains(newTable.TableName))
			{
				object obj = tables[newTable.TableName];
				if (obj is ArrayList)
				{
					ArrayList indexes = obj as ArrayList;
					foreach (int index in indexes)
					{
						Section section = sections[index] as Section;
						section.AddData(newTable);
					}
				}
				else
				{
					tables[newTable.TableName] = newTable;
				}
			}
			else
			{
				tables[newTable.TableName] = newTable;
			}

		}

		/// <summary>
		/// Associates DataTable with named static StyledTable element.
		/// </summary>
		/// <param name="tableName">The name of the static table to associate data with.</param>
		/// <param name="newTable">System.Data.DataTable: source data for a static table</param>
		public void AddStaticData(string tableName, DataTable newTable)
		{
			if (tablesByName.Contains(tableName))
			{
				object obj = tablesByName[tableName];
				if (obj is Section)
				{
					Section section = (Section)obj;
					section.AddStaticData(tableName, newTable);
				}
				else
				{
					tablesByName[tableName] = newTable;
				}
			}
			else
			{
				tablesByName[tableName] = newTable;
			}

		}

		/// <summary>
		/// This method is used to set the PictureBox image in a dynamic fashion.
		/// </summary>
		/// <param name="pictureName">The name of the picture to set image.</param>
		/// <param name="image">Image to be displayed.</param>
		public void AddPicture(string pictureName, Bitmap image)
		{
			AddPicture(pictureName, image, false);
		}

		/// <summary>
		/// This method is used to set the TextField content in a dynamic fashion.
		/// </summary>
		/// <param name="textFieldName">The name of the TextField to set content.</param>
		/// <param name="text">Text content to set.</param>
		public void AddText(string textFieldName, string text)
		{
			if (textFields.Contains(textFieldName))
			{
				object obj = textFields[textFieldName];
				if (obj is Section)
				{
					Section section = (Section)obj;
					section.AddText(textFieldName, text);
				}
				else
				{
					textFields[textFieldName] = text;
				}
			}
			else
			{
				textFields[textFieldName] = text;
			}
		}


		/// <summary>
		/// This method is used to set the RichText content in a dynamic fashion.
		/// </summary>
		/// <param name="textFieldName">The name of the RichText element to set content.</param>
		/// <param name="text">Text content to set.</param>
		public void AddRichText(string richTextFieldName, string text)
		{
			if (richTextFields.Contains(richTextFieldName))
			{
				object obj = richTextFields[richTextFieldName];
				if (obj is Section)
				{
					Section section = (Section)obj;
					section.AddRichText(richTextFieldName, text);
				}
				else
				{
					richTextFields[richTextFieldName] = text;
				}
			}
			else
			{
				richTextFields[richTextFieldName] = text;
			}
		}

		/// <summary>
		/// This method is used to set the PictureBox image in a dynamic fashion.
		/// </summary>
		/// <param name="pictureName">The name of the picture to set image.</param>
		/// <param name="image">Image to be displayed.</param>
		/// <param name="stretch">Stretch image to fit PictureBox.</param>
		public void AddPicture(string pictureName, Bitmap image, bool stretch)
		{
			if (pictures.Contains(pictureName))
			{
				object obj = pictures[pictureName];
				if (obj is Section)
				{
					Section section = (Section)obj;
					section.AddPicture(pictureName, image, stretch);
				}
				else
				{
					pictures[pictureName] = image;
				}
			}
			else
			{
				pictures[pictureName] = image;
			}
		}

		/// <summary>
		/// This method is used to set the Timeline element data in a dynamic fashion.
		/// </summary>
		public void AddTimelineData(string timelineName, TimelinePoint[] points, TimelinePeriod[] periods)
		{
			if ( timelines.Contains(timelineName) ) 
			{
				Section section = (Section)timelines[timelineName];
				section.AddTimelineData(timelineName, points, periods);
			}
		}


		/// <summary>
		/// This method is used to set the map points in a dynamic fashion.
		/// </summary>
		public void AddMapData(string mapName, Map.MapPoint[] points)
		{
			if ( maps.Contains(mapName) ) 
			{
				if (maps[mapName] is Section)
				{
					Section section = (Section)maps[mapName];
					section.AddMapData(mapName, points);
				}
				else
				{
					maps[mapName] = points;
				}
			}
			else
			{
				maps[mapName] = points;
			}
		}

		/// <summary>
		/// This method is used to set both map points and map type in a dynamic fashion.
		/// </summary>
		public void AddMapData(string mapName, Map.MapPoint[] points, Map.MapTypes type)
		{
			if ( maps.Contains(mapName) ) 
			{
				if (maps[mapName] is Section)
				{
					Section section = (Section)maps[mapName];
					section.AddMapData(mapName, points, type);
				}
				else
				{
					maps[mapName] = new Map.MapData(type, points);
				}
			}
			else
			{
				maps[mapName] = new Map.MapData(type, points);
			}
		}

		/// <summary>
		/// This method is used to set the barcode data for named barcode element in a dynamic fashion.
		/// </summary>
		/// <param name="barcodeName">The name of the barcode to set data.</param>
		/// <param name="barcodeData">Text data for barcode.</param>
		public void AddBarcode(string barcodeName, string barcodeData)
		{
			if (barcodes.Contains(barcodeName))
			{
				object obj = barcodes[barcodeName];
				if (obj is Section)
				{
					Section section = (Section)obj;
					section.AddBarcode(barcodeName, barcodeData);
				}
				else
				{
					barcodes[barcodeName] = barcodeData;
				}
			}
			else
			{
				barcodes[barcodeName] = barcodeData;
			}
		}

		/// <summary>
		/// This method is used to set the UserPaint object for named UserPaint element in a dynamic fashion.
		/// </summary>
		/// <param name="userPaintName">The name of the UserPaintElement to set data.</param>
		/// <param name="userPaint">UserPaint object to display.</param>
		/// <param name="overrideTemplateSize">override template size with new UserPaint size.</param>
		public void AddUserPaint(string userPaintName, Com.Delta.PrintManager.Engine.UserPaint userPaint, bool overrideTemplateSize)
		{
			if (userPaints.Contains(userPaintName))
			{
				object obj = userPaints[userPaintName];
				if (obj is Section)
				{
					Section section = (Section)obj;
					section.AddUserPaint(userPaintName, userPaint, overrideTemplateSize);
				}
				else
				{
					userPaints[userPaintName] = userPaint;
				}
			}
			else
			{
				userPaints[userPaintName] = userPaint;
			}
		}

		/// <summary>
		/// This method is used to set the UserPaint object for named UserPaint element in a dynamic fashion.
		/// </summary>
		/// <param name="userPaintName">The name of the UserPaintElement to set data.</param>
		/// <param name="userPaint">UserPaint object to display.</param>
		public void AddUserPaint(string userPaintName, Com.Delta.PrintManager.Engine.UserPaint userPaint)
		{
			AddUserPaint(userPaintName, userPaint, false);
		}

		/// <summary>
		/// This method is used to set both the barcode type and barcode display text in a dynamic fashion.
		/// </summary>
		/// <param name="barcodeName">The name of the barcode to set data.</param>
		/// <param name="barcodeData">Text data for barcode.</param>
		/// <param name="barcodeType">The barcode type (barcode standard) to use for displaying data.</param>
		public void AddBarcode(string barcodeName, string barcodeData, Barcode.BarcodeTypes barcodeType)
		{
			if (barcodes.Contains(barcodeName))
			{
				object obj = barcodes[barcodeName];
				if (obj is Section)
				{
					Section section = (Section)obj;
					section.AddBarcode(barcodeName, barcodeData, barcodeType);
				}
				else
				{
					barcodes[barcodeName] = new Barcode.BarcodeData(barcodeType, barcodeData);
				}
			}
			else
			{
				barcodes[barcodeName] = new Barcode.BarcodeData(barcodeType, barcodeData);
			}
		}

		/// <summary>
		/// Registering textField for correct data transfer to sections
		/// </summary>
		internal void RegisterTextField(string textFieldName, Section section)
		{
			if (textFields.Contains(textFieldName) && textFields[textFieldName] is string)
			{
				section.AddText(textFieldName, (string)textFields[textFieldName]);	
			}
			textFields[textFieldName] = section;			
		}


		/// <summary>
		/// Registering textField for correct data transfer to sections
		/// </summary>
		internal void RegisterRichTextField(string textFieldName, Section section)
		{
			if (richTextFields.Contains(textFieldName) && richTextFields[textFieldName] is string)
			{
				section.AddText(textFieldName, (string)richTextFields[textFieldName]);	
			}
			richTextFields[textFieldName] = section;			
		}

		/// <summary>
		/// Registering picture for correct data transfer to sections
		/// </summary>
		internal void RegisterPicture(string pictureName, Section section)
		{
			if (pictures.Contains(pictureName) && pictures[pictureName] is Bitmap)
			{
				section.AddPicture(pictureName, (Bitmap)pictures[pictureName]);	
			}
			pictures[pictureName] = section;			
		}

		internal void RegisterUserPaint(string userPaintName, Section section)
		{
			if (userPaints.Contains(userPaintName) && userPaints[userPaintName] is UserPaint)
			{
				section.AddUserPaint(userPaintName, (UserPaint)userPaints[userPaintName], false);	
			}
			userPaints[userPaintName] = section;			
		}

		/// <summary>
		/// Registering timeline for correct data transfer to sections
		/// </summary>
		internal void RegisterTimeline(string timelineName, Section section)
		{
			timelines[timelineName] = section;			
		}

		/// <summary>
		/// Registering barcode for correct data transfer to sections
		/// </summary>
		internal void RegisterBarcode(string barcodeName, Section section)
		{
			if (barcodes.Contains(barcodeName))
			{
				if (barcodes[barcodeName] is string)
				{
					section.AddBarcode(barcodeName, (string)barcodes[barcodeName]);	
				}
				else if (barcodes[barcodeName] is Barcode.BarcodeData)
				{
					Barcode.BarcodeData barcodeData = barcodes[barcodeName] as Barcode.BarcodeData;
					section.AddBarcode(barcodeName, barcodeData.BarcodeText, barcodeData.BarcodeType);	
				}
			}

			barcodes[barcodeName] = section;			
		}

		/// <summary>
		/// Registering map for correct data transfer to sections
		/// </summary>
		internal void RegisterMap(string mapName, Section section)
		{
			if (maps.Contains(mapName))
			{
				if (maps[mapName] is Map.MapPoint[])
				{
					section.AddMapData(mapName, (Map.MapPoint[])maps[mapName]);	
				}
				else if (maps[mapName] is Map.MapData)
				{
					Map.MapData mapData = maps[mapName] as Map.MapData;
					section.AddMapData(mapName, mapData.MapPoints, mapData.MapType);
				}
			}

			maps[mapName] = section;			
		}

		/// <summary>
		/// Registering tables for correct data transfer to sections
		/// </summary>
		internal void RegisterTable(string tableName, Section section)
		{
			ArrayList indexes = new ArrayList();
			if (tables.Contains(tableName) && tables[tableName] is ArrayList)
			{
				indexes = tables[tableName] as ArrayList;
			}

			if (tables.Contains(tableName) && tables[tableName] is DataTable)
			{
				section.AddData((DataTable)tables[tableName]);	
			}

			int index = sections.IndexOf(section);
			if (!indexes.Contains(index))
				indexes.Add(index);


			tables[tableName] = indexes;			
		}

		/// <summary>
		/// Registering tables for correct data transfer to sections
		/// </summary>
		internal void RegisterTableByName(string tableName, Section section)
		{
			if (tablesByName.Contains(tableName) && tablesByName[tableName] is DataTable)
			{
				section.AddStaticData(tableName, (DataTable)tablesByName[tableName]);	
			}
			tablesByName[tableName] = section;			
		}

		/// <summary>
		/// Registering charts for correct data transfer to sections
		/// </summary>
		internal void RegisterScatter(string scatterName, Section section)
		{
			scatters[scatterName] = section;			
		}

		/// <summary>
		/// Registering charts for correct data transfer to sections
		/// </summary>
		internal void RegisterChart(string chartName, Section section)
		{
			charts[chartName] = section;			
		}
		


		/// <summary>
		/// Public function to reapply alignments to all static and dynamic document objects.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <remarks>This function is called when margins or page layout has been changed, and the alignment
		/// of the objects needs to be reset.</remarks>
		internal void RepeatAlignments(object sender)
		{
			foreach (Section section in Sections)
			{
				for (int i=0;i<section.Objects.Length;i++)
				{
					section.Objects[i].HorizontalAlignment = section.Objects[i].HorizontalAlignment;
					section.Objects[i].VerticalAlignment = section.Objects[i].VerticalAlignment;
				}
			}
		}


		/// <summary>
		/// Function to load a Hashtable of parameter values.
		/// </summary>
		/// <param name="parameters">Hashtable of parameter values</param>
		/// <remarks>
		/// c#
		/// <code language="c#">
		/// // fill in with some parameters
		/// // (parameter names are case sensitive)
		/// Hashtable parameters = new Hashtable();
		/// parameters.Add("author","Dynamica");
		/// printDocument.SetParameters(parameters);
		/// </code>
		/// </remarks>
		public void SetParameters(Hashtable parameters)
		{
			if (parameters != null)
				parameterValues = parameters;
			else
				parameterValues = new Hashtable();
		}


		/// <summary>
		/// Loads the XML content from the XML template file.
		/// </summary>
		/// 
		/// <param name="filename">File location of XML template file.</param>
		/// <remarks>Use absolute paths for best performance. Relative paths are resolved with respect to the application startup folder.</remarks>
		/// <exception cref="System.ArgumentException">ArgumentException is thrown is non-existing filepath is given, or the given file is not in the valid XML format.</exception>
		public void setXML(string filename)
		{
			xmlDoc = new XmlDocument();
			

			if (!File.Exists(filename))
			{
				throw new ArgumentException("File does not exist.", filename);
			}

			try
			{				
				xmlDoc.Load(filename);
				DocRoot = (new FileInfo(filename)).DirectoryName;
				setXML();
			}
			catch (Exception e)
			{
				throw new ArgumentException("File is not in a valid format.", filename, e);
			}
		}


		/// <summary>
		/// Loads the XML content from the XmlDocument object.			
		/// </summary>
		/// <param name="xmlDocument">XmlDocument object to load content from.</param>
		/// <param name="documentRoot">Report Document Root (<see cref="Com.Delta.PrintManager.Engine.ReportDocument.DocRoot"/>) folder</param>
		/// <remarks>
		/// The documentRoot is used as a starting point for
		/// PictureBox image locations.
		/// </remarks>
		public void setXML(System.Xml.XmlDocument xmlDocument, string documentRoot)
		{
			this.xmlDoc = xmlDocument;
			DocRoot = documentRoot;
			setXML();
		}

		/// <summary>
		/// Loads the XML content from the given string.
		/// </summary>
		/// <param name="text">XML text content</param>
		/// <param name="documentRoot">XMLDocument Root</param>
		/// <remarks>
		/// The documentRoot is used as a starting point for
		/// PictureBox image locations.
		/// </remarks>
		public void setXML(string text, string documentRoot)
		{
			this.xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(text);
			DocRoot = documentRoot;
			setXML();
		}

		/// <summary>
		/// Loads the XML content from the given web location.
		/// </summary>
		/// <param name="webLocation">Absolute web address of the XML text content file</param>
		public void setXML(Uri webLocation)
		{
			setXML(webLocation, String.Empty);
		}

		/// <summary>
		/// Loads the XML content from the given web location.
		/// </summary>
		/// <param name="webLocation">Absolute web address of the XML text content file</param>
		/// <param name="documentRoot">XMLDocument Root</param>
		/// <remarks>
		/// The documentRoot is used as a starting point for
		/// PictureBox image locations.
		/// </remarks>
		public void setXML(Uri webLocation, string documentRoot)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(webLocation);
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();

			Stream bufferStream = response.GetResponseStream();
			StreamReader sr = new StreamReader(bufferStream, System.Text.Encoding.GetEncoding("utf-8"));

			string content = sr.ReadToEnd();

			response.Close();
			sr.Close();

			setXML(content, documentRoot);

		}


		/// <summary>
		/// Gets report element with given Name property; returs null if no such element is found
		/// </summary>
		public ICustomPaint FindElement(string elementName)
		{
			foreach(Section section in this.sections)
			{
				ICustomPaint obj = null;
				if ((obj = section.FindElement(elementName)) != null)
					return obj;
			}

			return null;
		}


		/// <summary>
		/// Sets the categories for the specified chart.
		/// </summary>
		/// <param name="chartName"><see cref="Com.Delta.PrintManager.Engine.ChartBox">Com.Delta.PrintManager.Engine.ChartBox</see> to set categories for.</param>
		/// <param name="categories">string array of categories (category names).</param>
		/// <remarks>
		/// C# Example
		/// <code language="c#">
		/// printDocument.SetChartCategories("chart0", new string[] {"New York","Shangai","Mexico City"});
		/// </code>
		/// </remarks>
		public void SetChartCategories(string chartName,string[] categories)
		{
			if ( charts.Contains(chartName) ) 
			{
				Section section = (Section)charts[chartName];
				section.SetChartCategories(chartName, categories);
			}	
		}

		/// <summary>
		/// This method is used to serialize your report to the HTML document in the specified folder.
		/// </summary>
		public void SerializeToHtml(string folderName)
		{
			HtmlSerializer htmlSerializer = new HtmlSerializer();
			htmlSerializer.Serialize(this, folderName);			
		}

		/// <summary>
		/// This method is used to serialize your report to the PDF byte array.
		/// </summary>
		public byte[] SerializeToPdfStream()
		{
			if (OnPdfSerializationStarted!=null)
				OnPdfSerializationStarted(this, EventArgs.Empty);

			byte[] content = ToPdfStream();

			if (OnPdfSerializationFinished!=null)
				OnPdfSerializationFinished(this, EventArgs.Empty);

			return content;
		}


		/// <summary>
		/// This method is used to serialize (save) your report to the set of Images. Each report page corresponds to a single image.
		/// </summary>
		/// <remarks>
		/// In order to save images to specific file format, use the following:
		/// <code>
		///	Image[] reportImages = ReportDocument.SerializeToImages();
		///	for(int i=0;i&lt;reportImages.Length;i++)
		///	{
		///		// use appropriate image format and file extension
		///		reportImages[i].Save(@"e:\image" + i.ToString() + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
		///	}
		/// </code>	
		/// </remarks>		
		public Image[] SerializeToImages()
		{
			ImageSerializer imageSerializer = new ImageSerializer();
			return imageSerializer.Serialize(this);
		}

		private byte[] ToPdfStream()
		{
			PdfSerializer pdfSerializer = new PdfSerializer();
			return pdfSerializer.Serialize(this);
		}

		/// <summary>
		/// This method is used to serialize (save) your report to the PDF file.
		/// </summary>
		/// <param name="filepath">The absolute path of the PDF file that this report will be saved to.</param>
		/// <remarks>If the file with this name already exists, it will be overwritten.</remarks>
		public void SerializeToPdfFile(string filepath)
		{
			if (OnPdfSerializationStarted!=null)
				OnPdfSerializationStarted(this, EventArgs.Empty);

			byte[] content = ToPdfStream();

			FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.Write);
			fs.Write(content, 0, content.Length);
			fs.Close();

			if (OnPdfSerializationFinished!=null)
				OnPdfSerializationFinished(this, EventArgs.Empty);
		}


		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the report document name.
		/// </summary>
		[Category("Document"), Description("The name of the Report Document.")]
		public new string DocumentName
		{
			get {return base.DocumentName;}
			set {base.DocumentName = value;}
		}

		/// <summary>
		/// Gets or sets the report startup parameters.
		/// </summary>
		[Category("Document"), Description("The report startup options.")]
		public StartInfo StartInfo
		{
			get {return this.startInfo;}
			set {this.startInfo = value;}
		}

		/// <summary>
		/// Gets or sets the report default database connection.
		/// </summary>
		[Category("Document"), Description("The report default database connection. This connection is used to prefill data if PreloadData option is set to True."), NotifyParentProperty(true)]
		public ConnectionInfo Connection
		{
			get {return this.connectionInfo;}
			set {this.connectionInfo = value;}
		}

		/// <summary>
		/// The bool property to show if in preview mode.
		/// </summary>
		/// <remarks></remarks>
		[Browsable(false)]
		public bool PreviewMode
		{
			get
			{
				return !(this.PrintController is StampaPrintController);
			}
		}


		/// <summary>
		/// Gets the value showing if document is in design mode.
		/// </summary>
		/// <remarks></remarks>
		[Browsable(false)]
		public new bool DesignMode
		{
			get {return designMode;}
		}



		/// <summary>
		/// Gets or sets the Document Root for the current XML document.
		/// </summary>
		/// <remarks>This is used to parse for locations of referenced images.</remarks>
		[Category("Document"), Description("The folder where document resides.")]
		public string DocRoot
		{
			get { return this.mDocRoot; }
			set 
			{
				this.mDocRoot = value;
				
				foreach(Section section in sections)
				{
					for (int i =0;i<section.Objects.Length;i++)
					{
						if ( section.Objects[i] is PictureBox )
							((PictureBox)section.Objects[i]).Reload();
					}
					
				}

			}
		}

		/// <summary>
		/// Gets or sets the margins for this report document.
		/// </summary>
		[Category("Document"), Description("Document margins (in pixels)."), DefaultValue(typeof(System.Drawing.Printing.Margins),"50, 50, 50, 50")]
		public Margins Margins
		{
			//get {return this.DefaultPageSettings.Margins;}
			get {return documentMargins;}
			set 
			{
				documentMargins = value;
				this.DefaultPageSettings.Margins = value;
				if(OnMarginsChanged!=null)
					OnMarginsChanged(this);
			}
		}

		
		
		/// <summary>
		/// Document custom width. Meaningfull only is PaperType is set to Custom.
		/// </summary>
		[Category("Document"), DefaultValue(210.0), Description("Document custom width. Meaningfull only is PaperType is set to Custom.")]
		public double CustomWidth
		{
			get {return customWidth;}
			set
			{
				this.customWidth = Math.Max(0,value);
				LimitCustomSize();

				if (paperType == Paper.Type.Custom)
				{
					this.DefaultPageSettings.PaperSize = new PaperSize("", PixelSize.Width, PixelSize.Height);

					if(OnPageSizeChanged!=null)
						OnPageSizeChanged(this);
				}
			}
		}

		/// <summary>
		/// Document custom width. Meaningfull only is PaperType is set to Custom.
		/// </summary>
		[Category("Document"), DefaultValue(297.0), Description("Document custom height. Meaningfull only is PaperType is set to Custom.")]
		public double CustomHeight
		{
			get {return customHeight;}
			set
			{
				this.customHeight = Math.Max(0,value);;
				LimitCustomSize();

				if (paperType == Paper.Type.Custom)
				{
					this.DefaultPageSettings.PaperSize = new PaperSize("", PixelSize.Width, PixelSize.Height);

					if(OnPageSizeChanged!=null)
						OnPageSizeChanged(this);
				}
			}
		}

		/// <summary>
		/// Document unit for custom sized documents. Meaningfull only is PaperType is set to Custom.
		/// </summary>
		[Category("Document"), DefaultValue(Units.Millimeters), RefreshProperties(RefreshProperties.All), Description("Document unit when PaperType is set to Custom.")]
		public Units CustomUnit
		{
			get {return this.customUnit;}
			set 
			{
				this.customUnit = value;
				LimitCustomSize();
				if (paperType == Paper.Type.Custom)
				{
					this.DefaultPageSettings.PaperSize = new PaperSize("", PixelSize.Width, PixelSize.Height);

					if(OnPageSizeChanged!=null)
						OnPageSizeChanged(this);
				}
			}
		}

		/// <summary>
		/// Gets the document size in pixels.
		/// </summary>
		[Browsable(false)]
		public Size PixelSize
		{
			get 
			{
				switch(customUnit)
				{
					case Units.Pixels:
						return new Size(Convert.ToInt32(customWidth), Convert.ToInt32(customHeight));

					case Units.Inches:
						return new Size(Convert.ToInt32(customWidth * 100), Convert.ToInt32(customHeight * 100));

					case Units.Millimeters:
						return new Size(Convert.ToInt32(customWidth * 3.938), Convert.ToInt32(customHeight * 3.938));

					default :
						return new Size(Convert.ToInt32(customWidth), Convert.ToInt32(customHeight));
				}				
			}
		}


		/// <summary>
		/// Gets or sets the 
		/// <see cref="Com.Delta.PrintManager.Engine.ReportDocument.LayoutType">Com.Delta.PrintManager.Engine.ReportDocument.LayoutType</see>
		/// for the ReportDocument.
		/// </summary>
		[Category("Document"), DefaultValue(LayoutType.Portrait), Description("Document layout.")]
		public LayoutType Layout
		{
			get
			{
				if ( this.DefaultPageSettings.Landscape )
					return LayoutType.Landscape;
				else
					return LayoutType.Portrait;
			}

			set
			{
				if ( value == LayoutType.Landscape )
					this.DefaultPageSettings.Landscape = true;
				else
					this.DefaultPageSettings.Landscape = false;

				if(OnPageLayoutChanged!=null)
					OnPageLayoutChanged(this);
			}
		}

		/// <summary>
		/// Gets or sets the 
		/// <see cref="Com.Delta.PrintManager.Engine.Paper.Type">Com.Delta.PrintManager.Engine.Paper.Type</see>
		/// for the ReportDocument.
		/// </summary>
		[Category("Document"), DefaultValue(Paper.Type.A4), Description("Paper type.")]
		public Paper.Type PaperType
		{
			get{ return paperType;}
			set
			{
				paperType = value;
				if (paperType != Paper.Type.Custom)
				{
					Size size = Paper.GetPaperSize(paperType);
					this.DefaultPageSettings.PaperSize = new PaperSize("", size.Width, size.Height);
				}
				else
				{
					this.DefaultPageSettings.PaperSize = new PaperSize("", PixelSize.Width, PixelSize.Height);
				}

				if(OnPageSizeChanged!=null)
					OnPageSizeChanged(this);
			}

		}
		

		/// <summary>
		/// Gets or set the queries used by this report.
		/// </summary>
		[Category("Document"), Description("Report queries.")]
		public QueryData[] Queries
		{
			get {return (QueryData[])queries.ToArray(typeof(QueryData));}
			set {queries = new ArrayList(value);}
		}

		/// <summary>
		/// Gets or set the parameter collection used by this report.
		/// </summary>
		[Category("Document"), Description("Report parameters.")]
		public string[] Parameters
		{
			get
			{
				string[] tmp = new string[declaredParameters.Count];
				for (int i=0;i<declaredParameters.Count;i++)
				{
					tmp[i] = declaredParameters[i].ToString();
				}
				return tmp;
			}

			set
			{
				declaredParameters = new ArrayList(value);
				Hashtable tmp = new Hashtable();

				for (int i=0;i<declaredParameters.Count;i++)
				{
					if (!defaultParameterValues.Contains(declaredParameters[i]))
						tmp[declaredParameters[i]] = String.Empty;
					else
						tmp[declaredParameters[i]] = defaultParameterValues[declaredParameters[i]];

				}
				defaultParameterValues = tmp;
				

			}

		}
	
		/// <summary>
		/// Gets the collection of default parameter values for this document.
		/// </summary>
		[Browsable(false)]
		public Hashtable DefaultParameterValues
		{
			get {return defaultParameterValues;}
			set {defaultParameterValues = value;}
		}

		/// <summary>
		/// Gets an array of Sections contained in this document.
		/// </summary>
		[Browsable(false)]
		public ArrayList Sections
		{
			get {return sections;}
		}

		
		internal Section[] GetSections(string name)
		{
			ArrayList list = new ArrayList();
			foreach (Section section in this.sections)
			{
				if (section.Name == name)
					list.Add(section);
			}
			return (Section[])list.ToArray(typeof(Com.Delta.PrintManager.Engine.Section));
		}

		/// <summary>
		/// 
		/// </summary>
		[Browsable(false)]
		public new bool OriginAtMargins
		{
			get {return base.OriginAtMargins;}
		}


		#endregion

		#region Public Events
 
		/// <summary>
		/// Occurs before PDF serialization has started.
		/// </summary>
		public event EventHandler OnPdfSerializationStarted;

		/// <summary>
		/// Occurs when PDF serialization has finished.
		/// </summary>
		public event EventHandler OnPdfSerializationFinished;

		#endregion

		#region Private Event Handlers

		


		private void DaPrintDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
		{
			if (isXmlOK)
			{
								
				if (Convert.ToInt32(systemValues["totalPages"]) == 0)
				{
					systemValues["totalPages"] = calculateNumberOfPages(e.Graphics) ;
					if (sectionIndex<sectionPageNumbers.Length)
						systemValues["sectionTotalPages"] = sectionPageNumbers[sectionIndex] ;
					

					if (this.PrinterSettings.PrintRange==PrintRange.SomePages)
					{
						if (this.PrinterSettings.FromPage > this.PrinterSettings.ToPage)
						{
							e.Graphics.DrawString("Starting print page is greater than ending page index.",new Font("Tahoma",10),new SolidBrush(Color.Black), e.MarginBounds);
							e.HasMorePages = false;
							return;
						}
						else if (this.PrinterSettings.FromPage > (int)systemValues["totalPages"])
						{
							e.Graphics.DrawString("Starting print page is greater than total number of report pages.",new Font("Tahoma",10),new SolidBrush(Color.Black), e.MarginBounds);
							e.HasMorePages = false;
							return;
						}

						for (int i=1;i<Math.Max(this.PrinterSettings.FromPage,1);i++)
						{
							NewPage();
							if (!printingSection.UpdateDynamicContent(e.Graphics))
								SwitchToNextSection();
						}
					}
				}

				NewPage();	
				bool morePagesInSection = printingSection.UpdateDynamicContent(e.Graphics);
				

				foreach(ICustomPaint obj in printingSection.Objects)
				{
					
					if (obj.Anchored)
					{
						if (obj.Ready && !obj.Displayed)
						{													
							obj.Paint(e);
							if (obj.Done)
								obj.Displayed = true;
								
						}
					}
					else
					{

						if (obj.Layout == ICustomPaint.LayoutTypes.EveryPage)
						{
							obj.Paint(e);
						}
						else if (obj.Layout == ICustomPaint.LayoutTypes.FirstPage)
						{
							if ((int)systemValues["sectionPageNumber"] == 1)
							{
								obj.Paint(e);
							}
						}
						else if (obj.Layout == ICustomPaint.LayoutTypes.LastPage)
						{
							if (!morePagesInSection)
							{
								obj.Paint(e);
							}
						}
					}
				}

				e.HasMorePages = morePagesInSection ? true : SwitchToNextSection();

				if (this.PrinterSettings.PrintRange==PrintRange.SomePages)
				{
					int pageNumber = (int)systemValues["pageNumber"];
					if (pageNumber == this.PrinterSettings.ToPage)
					{
						e.HasMorePages = false;
					}
				}

			}
			else
			{
				string theMessage = "Xml file structure is incorrect !\r\n\r\n";

				e.Graphics.DrawString(theMessage+theErrorMessage,new Font("Tahoma",10),new SolidBrush(Color.Black), e.MarginBounds);
				e.HasMorePages = false;
			}
		}

		private bool SwitchToNextSection()
		{
			if (sectionIndex < sections.Count - 1)
			{
				NewSection();
				printingSection = (Section)sections[sectionIndex];
				return true;
			}
			else
			{
				return false;
			}
		}


		private void DaPrintDocument_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
		{
			this.DefaultPageSettings.Margins = documentMargins;
			if (startInfo.PreloadData)
			{
				PreloadData();
			}

			sectionIndex = 0;
			printingSection = sections[0] as Section;
			systemValues["pageNumber"] = 0 ;
			systemValues["sectionPageNumber"] = 0 ;
			systemValues["totalPages"] = 0 ;
			systemValues["sectionTotalPages"] = 0 ;

			foreach (Section section in this.Sections)
			{
				section.Reset();
			}
								
		}


		#endregion

		#region Private Functions


		private void PreloadData()
		{
			try
			{
				if (queries.Count > 0)
				{
					InitConnection();
					databaseConnection.Open();

					foreach (QueryData queryData in queries)
					{
						string queryCommand = resolveParameterValues(queryData.SelectCommand);
						DataTable data = FillDataTable(queryCommand);
						data.TableName = queryData.DataSource;
						AddData(data);
					}
				}

			}
			catch (Exception e)
			{
				if (!this.StartInfo.IgnorePreloadExceptions)
					throw e;
			}
			finally
			{
				if (databaseConnection != null)
				{
					try
					{
						databaseConnection.Close();
					}
					catch (Exception){}
				}
			}
		}

		private void InitConnection()
		{			
			Assembly a = Assembly.LoadWithPartialName(connectionInfo.Assembly);
			Type connectionType = a.GetType(connectionInfo.Type);

			databaseConnection = (IDbConnection)Activator.CreateInstance(connectionType);		
			databaseConnection.ConnectionString = connectionInfo.ConnectionString;
		}

		private DataTable FillDataTable(string query)
		{

			IDbCommand command = databaseConnection.CreateCommand();
			command.CommandText = query;

			IDataReader reader = command.ExecuteReader();
			IDataRecord record = reader as IDataRecord;

			DataTable metadata = reader.GetSchemaTable();

			DataTable data = new DataTable("printTable");

			for (int i=0;i<metadata.Rows.Count;i++)
			{
				string columnName = metadata.Rows[i]["ColumnName"] as String;
				Type columnType = metadata.Rows[i]["DataType"] as Type;
				data.Columns.Add(columnName, columnType);
			}

			while (reader.Read())
			{
				DataRow dataRow = data.NewRow();
				for (int i=0;i<data.Columns.Count;i++)
				{
					dataRow[i] = record.GetValue(i);					
				}

				data.Rows.Add(dataRow);
			}

			reader.Close();

			return data;
		}

		private void LimitCustomSize()
		{
			switch(customUnit)
			{
				case Units.Inches:
					customWidth = Math.Min(customWidth, 45);
					customHeight = Math.Min(customHeight, 45);
					break;

				case Units.Millimeters:
					customWidth = Math.Min(customWidth, 1100);
					customHeight = Math.Min(customHeight, 1100);
					break;

				case Units.Pixels:
					customWidth = Math.Min(customWidth, 4500);
					customHeight = Math.Min(customHeight, 4500);
					break;

			}
		}

		private void getChartNames()
		{
			/*
			if (xmlStaticElements==null) return;

			for (int i=0;i<xmlStaticElements.Count;i++)
			{
				if (xmlStaticElements[i].Name == "chartBox")
				{
					if (xmlStaticElements[i].Attributes["name"] != null)
					{
						theCharts.Add(xmlStaticElements[i].Attributes["name"].Value,i);
					}
				}
			}*/
		}


		private void initSystemParameters()
		{
			systemValues = new Hashtable();
			systemValues.Add("pageNumber",0);
			systemValues.Add("totalPages",0);
			systemValues.Add("systemDate",DateTime.Now.ToString("dd.MM.yyyy"));
			systemValues.Add("systemTime",DateTime.Now.ToString("HH:mm:ss"));
		}


			


		private void resolveContents(XmlNode theNode)
		{
			// for backward compatibility - not supported since version 1.5
			sections.Clear();
			sections.Add(new Section(this, theNode));
		}
		
		
		private void resolveLayout(XmlNode theNode)
		{
			
			if (theNode.Attributes["size"] != null)
			{
				string theSize = theNode.Attributes["size"].Value;
			}			
			
			if (theNode.Attributes["layout"] != null)
			{
				string theOrientation = theNode.Attributes["layout"].Value;

				if ( theOrientation == "Landscape" )
					this.DefaultPageSettings.Landscape = true;
				else
					this.DefaultPageSettings.Landscape = false;
			}
		}
		

		private void resolveMargins(XmlNode theNode)
		{
			try
			{
				int top = Convert.ToInt32(theNode.Attributes["top"].Value);
				int left = Convert.ToInt32 (theNode.Attributes["left"].Value);
				int bottom = Convert.ToInt32 (theNode.Attributes["bottom"].Value);
				int right = Convert.ToInt32 (theNode.Attributes["right"].Value);
				this.Margins = new Margins(left, right, top, bottom);
			}
			catch(Exception){}
		}

		
		private void resolvePaperSize(XmlNode theNode)
		{			
			if (theNode.Attributes["papersize"] != null)
			{
				this.PaperType = Paper.GetType(theNode.Attributes["papersize"].Value);
			}					
		}


		private void resolveQueries(XmlNode theNode)
		{
			XmlNodeList childNodes = theNode.ChildNodes;

			queries.Clear();

			for (int i=0;i<childNodes.Count;i++)
			{
				try
				{
					QueryData queryData = new QueryData();
					queryData.DataSource = childNodes[i].Attributes["dataSource"].Value;
					queryData.SelectCommand = childNodes[i].Attributes["selectCommand"].Value;
					
					queries.Add(queryData);
				}
				catch(Exception){}
			}			

		}

		private void resolveStartInfo(XmlNode theNode)
		{
			XmlNodeList childNodes = theNode.ChildNodes;

			for (int i=0;i<childNodes.Count;i++)
			{
				switch (childNodes[i].Name)
				{
					case "preloadData" :
						try
						{
							this.StartInfo.PreloadData = Convert.ToBoolean(childNodes[i].Attributes["value"].Value);
						}
						catch (Exception){}
						break;	

					case "ignorePreloadExceptions" :
						try
						{
							this.StartInfo.IgnorePreloadExceptions = Convert.ToBoolean(childNodes[i].Attributes["value"].Value);
						}
						catch (Exception){}
						break;
				}
			}
		}


		private void resolveConnection(XmlNode theNode)
		{
			XmlNodeList childNodes = theNode.ChildNodes;

			for (int i=0;i<childNodes.Count;i++)
			{
				switch (childNodes[i].Name)
				{
					case "name" :
						try
						{
							this.Connection.Name = childNodes[i].Attributes["value"].Value;
						}
						catch (Exception){}
						break;	

					case "assembly" :
						try
						{
							this.Connection.Assembly = childNodes[i].Attributes["value"].Value;
						}
						catch (Exception){}
						break;	

					case "type" :
						try
						{
							this.Connection.Type = childNodes[i].Attributes["value"].Value;
						}
						catch (Exception){}
						break;

					case "connectionString" :
						try
						{
							this.Connection.ConnectionString = childNodes[i].Attributes["value"].Value;
						}
						catch (Exception){}
						break;
				}
			}
		}

		private void resolveParameterDeclaration(XmlNode theNode)
		{
			XmlNodeList childNodes = theNode.ChildNodes;

			for (int i=0;i<childNodes.Count;i++)
			{
				try
				{
					string parameterName = childNodes[i].Attributes["name"].Value;

					string parameterDefaultValue = String.Empty;
					try
					{
						parameterDefaultValue = childNodes[i].Attributes["defaultValue"].Value;
					}
					catch(Exception){parameterDefaultValue=String.Empty;}

					if (!declaredParameters.Contains(parameterName))
					{
						declaredParameters.Add(parameterName);
						defaultParameterValues[parameterName] = parameterDefaultValue;
					}
				}
				catch(Exception){}
			}
		}


		private void resolveSections(XmlNode theNode)
		{
			sections.Clear();
			XmlNodeList childNodes = theNode.ChildNodes;

			for (int i=0;i<childNodes.Count;i++)
			{
				new Section(this, childNodes[i]);
			}
		}

		private XmlNode FindMainNode(XmlDocument xmlDocument)
		{
			foreach(XmlNode node in xmlDocument.ChildNodes)
			{
				if (node is XmlElement && (node.Name == "stampa" || node.Name == "daReport"))
					return node;
			}
			return null;
		}

		private void setXML()
		{
			try
			{
				System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");

				XmlNode root = FindMainNode(xmlDoc);
				if (root == null)
					throw new Exception("File is not in a valid format.");

				XmlNodeList temp = root.ChildNodes;

				
				if (root.Attributes["customUnit"] != null)
				{
					string unit = root.Attributes["customUnit"].Value;
					if (unit.Equals("Inches"))
						this.customUnit = Units.Inches;
					else if (unit.Equals("Millimeters"))
						this.customUnit = Units.Millimeters;
					else
						this.customUnit = Units.Pixels;

				}

				try
				{
					this.DocumentName = root.Attributes["name"].Value;										
				}
				catch(Exception){}

				try
				{
					this.customWidth = Math.Max(0, Convert.ToDouble(root.Attributes["customWidth"].Value, ci.NumberFormat));					
					this.customHeight = Math.Max(0, Convert.ToDouble(root.Attributes["customHeight"].Value, ci.NumberFormat));
				}
				catch(Exception){}
				
				LimitCustomSize();

				resolvePaperSize(root);
				resolveLayout(root);


				for (int i=0;i<temp.Count;i++)
				{
					switch (temp[i].Name)
					{
						case "margins" :
							resolveMargins(temp[i]);
							break;

						case "startInfo" :
							resolveStartInfo(temp[i]);
							break;

						case "connection" :
							resolveConnection(temp[i]);
							break;

						case "parameters" :
							resolveParameterDeclaration(temp[i]);
							break;

						case "queries" :
							resolveQueries(temp[i]);
							break;

						case "sections" :							
							resolveSections(temp[i]);
							break;

						case "content" :
							// for backward compatibility
							//resolveContents(temp[i]);
							throw new Exception("Since version 1.5 Stampa Reports System does not support daReport document format.");
							break;
					}
				}

				
				

				this.OnMarginsChanged += new MarginsChangedHandler(RepeatAlignments);
				this.OnPageLayoutChanged += new PageLayoutChangedHandler(RepeatAlignments);
			}
			catch(FileNotFoundException)
			{
				//MessageBox.Show("No such file "+filename);
				
			}
			catch(XmlException e)
			{
				isXmlOK = false;
				Exception tmp = e;
				theErrorMessage = "";
				while (tmp != null)
				{
					theErrorMessage += tmp.Message + "\r\n";
					tmp = tmp.InnerException;
				}
			}
			catch(Exception e)
			{
				isXmlOK = false;
				Exception tmp = e;
				theErrorMessage = "";
				while (tmp != null)
				{
					theErrorMessage += tmp.Message + "\r\n";
					tmp = tmp.InnerException;
				}
			}

		}
		
		
		#endregion

		#region Private Properties


		private int calculateNumberOfPages(Graphics g)
		{
			int result = 0;
			this.sectionPageNumbers = new int[this.Sections.Count];
			int cnt = 0;
			foreach (Section section in Sections)
			{
				int pgNumber = section.CalculateNumberOfPages(g);
				result += pgNumber;
				sectionPageNumbers[cnt] = pgNumber;
				cnt++;
			}

			return result;			
		}


		/// <summary>
		/// Resolves parameter values during printing.
		/// </summary>
		internal string resolveParameterValues(string input)
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


					if ( systemValues.ContainsKey(parameterName))
					{
						buffer += systemValues[parameterName].ToString();
					}
					else if ( declaredParameters.Contains(parameterName) && parameterValues.ContainsKey(parameterName))
					{
						buffer += parameterValues[parameterName].ToString();
					}
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

		/// <summary>
		/// Finds the first occurence of parametar in given text and returns it's name (or null, if none found).
		/// </summary>
		internal string FindParameterInText(string input)
		{
			string parameterName = null;
			int pos = -1;
			int oldPos = 0;

			while( (pos=input.IndexOf("$P",oldPos)) != -1 )
			{

				if (input.IndexOf("}",pos+2) != -1 && input.Substring(pos+2,1).Equals("{"))
				{
					parameterName = input.Substring(pos+3,input.IndexOf("}",pos+2)-pos-3);

					oldPos = input.IndexOf("}",pos+2) + 1;
				}
				else
				{				
					oldPos = pos+2;
				}
			}

			return parameterName;
		}

		/// <summary>
		/// Gets the value for given parameter.
		/// </summary>
		internal string GetParameterValue(string parameterName)
		{
			if ( systemValues.ContainsKey(parameterName))
			{
				return systemValues[parameterName].ToString();
			}
			else if ( declaredParameters.Contains(parameterName) && parameterValues.ContainsKey(parameterName))
			{
				return parameterValues[parameterName] != null ? parameterValues[parameterName].ToString() : String.Empty;
			}
			else
			{
				return String.Empty;
			}
		}



		#endregion

		#region Creators and Destructor

		/// <summary>
		/// Initializes a new instance of the ReportDocument class.
		/// </summary>
		public ReportDocument()
		{

			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			graphics = Graphics.FromImage(new Bitmap(2500,1000));

			sections.Add(new Section(this));
			
			Size size = Paper.GetPaperSize(paperType);
			DefaultPageSettings.PaperSize = new PaperSize("",size.Width,size.Height);
			//DefaultPageSettings.Margins = new Margins(50,50,50,50);
			DefaultPageSettings.Margins = documentMargins;
			DefaultPageSettings.Landscape = false;
			declaredParameters = new ArrayList();
			initSystemParameters();
			parameterValues = new Hashtable();
			defaultParameterValues = new Hashtable();
			
			this.DocumentName = String.Empty;
			this.PrintController = new StampaPrintController();

		}	


		/// <summary>
		/// Initializes a new instance of the ReportDocument class.
		/// </summary>
		public ReportDocument(bool theMode):this() 
		{			
			designMode = theMode;
		}


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{

				foreach(Section section in sections)
				{
					for (int i =0;i<section.Objects.Length;i++)
					{
						if ( section.Objects[i] is PictureBox )
							((PictureBox)section.Objects[i]).Dispose();
					}
					
				}

				if( components != null )
					components.Dispose();
			}
			base.Dispose( disposing );
		}

		
		/// <summary>
		/// Gives StampaDocument the opportunity to finalize any child resources
		/// </summary>
		~ReportDocument()
		{
			Dispose();
		}

		
		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// DaPrintDocument
			// 
			this.BeginPrint += new System.Drawing.Printing.PrintEventHandler(this.DaPrintDocument_BeginPrint);
			this.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.DaPrintDocument_PrintPage);

		}
		#endregion

		#endregion


		




		internal Size GetDisplaySize()
		{
			Size s = this.PaperType == Paper.Type.Custom ? this.PixelSize : Paper.GetPaperSize(this.PaperType);

			if (this.Layout == LayoutType.Portrait)
			{
				return new Size(s.Width, s.Height);
			}
			else
			{
				return new Size(s.Height, s.Width);
			}
		}

	}
}