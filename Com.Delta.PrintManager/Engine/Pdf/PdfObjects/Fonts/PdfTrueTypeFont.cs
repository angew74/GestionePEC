using System;
using System.Drawing;
using System.Text;
using System.Collections;

namespace Com.Delta.PrintManager.Engine.Pdf.Fonts
{
	/// <summary>
	/// Summary description for PdfTrueTypeFont.
	/// </summary>
	internal class PdfTrueTypeFont : PdfFont
	{

		private PdfFontDescriptor mainDescriptor;
		private PdfFontDescriptor toUnicodeDescriptor;
		private PdfFontDescriptor cidDescriptor;
		private PdfFontDescriptor unicodeMapDescriptor;

		private RandomAccessBuffer rf;




		internal static string[] tableNamesSimple = {"cvt ", "fpgm", "glyf", "head",
														"hhea", "hmtx", "loca", "maxp", "prep"};
		internal static string[] tableNamesCmap = {"cmap", "cvt ", "fpgm", "glyf", "head",
													  "hhea", "hmtx", "loca", "maxp", "prep"};
		internal static string[] tableNamesExtra = {"OS/2", "cmap", "cvt ", "fpgm", "glyf", "head",
													   "hhea", "hmtx", "loca", "maxp", "name, prep"};
		internal static int[] entrySelectors = {0,0,1,1,2,2,2,2,3,3,3,3,3,3,3,3,4,4,4,4,4};
		internal static int TABLE_CHECKSUM = 0;
		internal static int TABLE_OFFSET = 1;
		internal static int TABLE_LENGTH = 2;
		internal static int HEAD_LOCA_FORMAT_OFFSET = 51;

		internal static int ARG_1_AND_2_ARE_WORDS = 1;
		internal static int WE_HAVE_A_SCALE = 8;
		internal static int MORE_COMPONENTS = 32;
		internal static int WE_HAVE_AN_X_AND_Y_SCALE = 64;
		internal static int WE_HAVE_A_TWO_BY_TWO = 128;


		protected Hashtable tableDirectory;

		bool fontSpecific = false;
		protected Hashtable cmap10;
		protected Hashtable cmap31;

		protected int[] GlyphWidths;
		protected int[][] bboxes;
		protected IntHashtable kerning = new IntHashtable();

		internal FontHeader head = new FontHeader();
		internal HorizontalHeader hhea = new HorizontalHeader();
		protected string style = "";
		protected double italicAngle;
		protected bool isFixedPitch = false;
		internal WindowsMetrics os_2 = new WindowsMetrics();
		protected bool subset = true;
		protected ArrayList subsetRanges;


		protected string fileName = @"c:\winnt\fonts\tahoma.ttf";

		protected bool includeCmap = false;
		protected bool includeExtras = false;
		protected bool locaShortTable;
		protected int[] locaTable;
		private Hashtable charactersUsed = new Hashtable();
		public Hashtable glyphsUsed = new Hashtable();
		protected ArrayList glyphsInList = new ArrayList();
		protected int tableGlyphOffset;
		protected int[] newLocaTable;
		protected byte[] newLocaTableOut;
		protected byte[] newGlyfTable;
		protected int glyfTableRealSize;
		protected int locaTableRealSize;
		protected byte[] outFont;
		protected int fontPtr;
		protected int directoryOffset = 0;


		protected Hashtable tables = new Hashtable();



		internal PdfTrueTypeFont(Font font, PdfDocument document) : base(font.Name, font.Name, document)
		{
			
			this.fontType = PdfFont.FontType.TrueType;

			this.fileName = PdfDocument.FindTTFFile(font);
			ReadFont();

			this.typename = PdfDocument.GetFullFontName(font).Replace(" ","");
			this.name = CreatePrefix() + PdfDocument.GetFullFontName(font).Replace(" ","");
		}

		private string CreatePrefix()
		{
			Random random = new Random();
			char[] s = new char[7];
			lock (random) 
			{
				for (int k = 0; k < 6; ++k)
					s[k] = (char)(random.Next('A', 'Z' + 1));
			}
			s[6] = '+';
			return new string(s);
		}


		internal override void AddDescriptors()
		{
			mainDescriptor = new PdfFontDescriptor(this, PdfFontDescriptor.Style.REGULAR);
			mainDescriptor.ID = this.document.GetNextId;
			this.document.AddPdfObject(mainDescriptor);

			unicodeMapDescriptor = new PdfFontDescriptor(this,PdfFontDescriptor.Style.UNICODE_MAP);
			unicodeMapDescriptor.ID = this.document.GetNextId;
			this.document.AddPdfObject(unicodeMapDescriptor);
			
			toUnicodeDescriptor = new PdfFontDescriptor(this, PdfFontDescriptor.Style.TO_UNICODE);
			toUnicodeDescriptor.ID = this.document.GetNextId;
			this.document.AddPdfObject(toUnicodeDescriptor);

			cidDescriptor = new PdfFontDescriptor(this, PdfFontDescriptor.Style.CID);
			cidDescriptor.ID = this.document.GetNextId;
			this.document.AddPdfObject(cidDescriptor); 
		}

		internal override string PageReference
		{
			get {return toUnicodeDescriptor.HeadR;}
		}

		internal PdfFontDescriptor GetMainDescriptor()
		{
			return mainDescriptor;
		}

		internal PdfFontDescriptor GetCidDescriptor()
		{
			return cidDescriptor;
		}

		internal PdfFontDescriptor GetUnicodeMapDescriptor()
		{
			return unicodeMapDescriptor;
		}




		internal override int StreamWrite(System.IO.Stream stream)
		{
			AddRangeUni(glyphsUsed, true, subset);

			byte[] b = Process();

			byte[] part2 = Utility.Deflate(b);

			StringBuilder sb = new StringBuilder(this.HeadObj);		
			sb.Append(String.Format("<</Filter/FlateDecode/Length {0}/Length1 {1}>>stream\n", part2.Length, b.Length));

			byte[] part1 = ASCIIEncoding.ASCII.GetBytes(sb.ToString());



			string text3="\nendstream\nendobj\n";
			Byte[] part3 = ASCIIEncoding.ASCII.GetBytes(text3);

			stream.Write(part1, 0, part1.Length);
			stream.Write(part2, 0, part2.Length);
			stream.Write(part3, 0, part3.Length);

			return part1.Length + part2.Length + part3.Length;
		}





		private void ReadFont()
		{
			rf = new RandomAccessBuffer(fileName);

			rf.Seek(0);
			int ttId = rf.ReadInt();

			if (ttId != 0x00010000 && ttId != 0x4F54544F)
				throw new Exception(fileName + " is not a valid TTF or OTF file.");


			int num_tables = rf.ReadUnsignedShort();
			rf.SkipBytes(6);
			for (int k = 0; k < num_tables; ++k) 
			{
				string tag = ReadStandardString(4);
				rf.SkipBytes(4);
				int[] table_location = new int[2];
				table_location[0] = rf.ReadInt();
				table_location[1] = rf.ReadInt();
				tables[tag] = table_location;
			}

			string[][] fullName = GetNames(4);
			string[][]  familyName = GetNames(1);
		

			FillTables();
			ReadGlyphWidths();
			ReadCMaps();
			ReadKerning();
			ReadBbox();
			GlyphWidths = null;
					
		}

		/** Reads a <CODE>string</CODE> from the font file as bytes using the Cp1252
		 *  encoding.
		 * @param length the length of bytes to read
		 * @return the <CODE>string</CODE> read
		 * @throws IOException the font file could not be read
		 */
		protected string ReadStandardString(int length) 
		{
			byte[] buf = new byte[length];
			rf.ReadFully(buf);
			return System.Text.Encoding.GetEncoding(1252).GetString(buf);
		}
    
		/** Reads a Unicode <CODE>string</CODE> from the font file. Each character is
		 *  represented by two bytes.
		 * @param length the length of bytes to read. The <CODE>string</CODE> will have <CODE>length</CODE>/2
		 * characters
		 * @return the <CODE>string</CODE> read
		 * @throws IOException the font file could not be read
		 */
		protected string ReadUnicodeString(int length) 
		{
			StringBuilder buf = new StringBuilder();
			length /= 2;
			for (int k = 0; k < length; ++k) 
			{
				buf.Append(rf.ReadChar());
			}
			return buf.ToString();
		}

		/** Extracts the names of the font in all the languages available.
		 * @param id the name id to retrieve
		 * @throws DocumentException on error
		 * @throws IOException on error
		 */    
		internal string[][] GetNames(int id) 
		{
			int[] table_location;
			table_location = (int[])tables["name"];
			if (table_location == null)
				throw new Exception("Table 'name' does not exist in " + fileName );
			rf.Seek(table_location[0] + 2);
			int numRecords = rf.ReadUnsignedShort();
			int startOfStorage = rf.ReadUnsignedShort();
			ArrayList names = new ArrayList();
			for (int k = 0; k < numRecords; ++k) 
			{
				int platformID = rf.ReadUnsignedShort();
				int platformEncodingID = rf.ReadUnsignedShort();
				int languageID = rf.ReadUnsignedShort();
				int nameID = rf.ReadUnsignedShort();
				int length = rf.ReadUnsignedShort();
				int offset = rf.ReadUnsignedShort();
				if (nameID == id) 
				{
					int pos = rf.FilePointer;
					rf.Seek(table_location[0] + startOfStorage + offset);
					string name;
					if (platformID == 0 || platformID == 3 || (platformID == 2 && platformEncodingID == 1))
					{
						name = ReadUnicodeString(length);
					}
					else 
					{
						name = ReadStandardString(length);
					}
					names.Add(new string[]{platformID.ToString(),
											  platformEncodingID.ToString(), languageID.ToString(), name});
					rf.Seek(pos);
				}
			}
			string[][] thisName = new string[names.Count][];
			for (int k = 0; k < names.Count; ++k)
				thisName[k] = (string[])names[k];
			return thisName;
		}

		protected byte[] GetFullFont() 
		{
			RandomAccessBuffer rf2 = null;
			try 
			{
				rf2 = new RandomAccessBuffer(rf);
				rf2.ReOpen();
				byte[] b = new byte[rf2.Length];
				rf2.ReadFully(b);
				return b;
			} 
			finally 
			{
				try {rf2.Close();} 
				catch {}
			}
		}

		internal byte[] Process() 
		{
			try 
			{
				rf.ReOpen();
				CreateTableDirectory();
				ReadLoca();
				FlatGlyphs();
				CreateNewGlyphTables();
				LocaTobytes();
				AssembleFont();
				return outFont;
			}
			finally 
			{
				try 
				{
					rf.Close();
				}
				catch  
				{
					// empty on purpose
				}
			}
		}
    
		protected void AssembleFont() 
		{
			int[] tableLocation;
			int fullFontSize = 0;
			string[] tableNames;
			if (includeExtras)
				tableNames = tableNamesExtra;
			else 
			{
				if (includeCmap)
					tableNames = tableNamesCmap;
				else
					tableNames = tableNamesSimple;
			}
			int tablesUsed = 2;
			int len = 0;
			for (int k = 0; k < tableNames.Length; ++k) 
			{
				string name = tableNames[k];
				if (name.Equals("glyf") || name.Equals("loca"))
					continue;
				tableLocation = (int[])tableDirectory[name];
				if (tableLocation == null)
					continue;
				++tablesUsed;
				fullFontSize += (tableLocation[TABLE_LENGTH] + 3) & (~3);
			}
			fullFontSize += newLocaTableOut.Length;
			fullFontSize += newGlyfTable.Length;
			int iref = 16 * tablesUsed + 12;
			fullFontSize += iref;
			outFont = new byte[fullFontSize];
			fontPtr = 0;
			WriteFontInt(0x00010000);
			WriteFontShort(tablesUsed);
			int selector = entrySelectors[tablesUsed];
			WriteFontShort((1 << selector) * 16);
			WriteFontShort(selector);
			WriteFontShort((tablesUsed - (1 << selector)) * 16);
			for (int k = 0; k < tableNames.Length; ++k) 
			{
				string name = tableNames[k];
				tableLocation = (int[])tableDirectory[name];
				if (tableLocation == null)
					continue;
				WriteFontString(name);
				if (name.Equals("glyf")) 
				{
					WriteFontInt(CalculateChecksum(newGlyfTable));
					len = glyfTableRealSize;
				}
				else if (name.Equals("loca")) 
				{
					WriteFontInt(CalculateChecksum(newLocaTableOut));
					len = locaTableRealSize;
				}
				else 
				{
					WriteFontInt(tableLocation[TABLE_CHECKSUM]);
					len = tableLocation[TABLE_LENGTH];
				}
				WriteFontInt(iref);
				WriteFontInt(len);
				iref += (len + 3) & (~3);
			}
			for (int k = 0; k < tableNames.Length; ++k) 
			{
				string name = tableNames[k];
				tableLocation = (int[])tableDirectory[name];
				if (tableLocation == null)
					continue;
				if (name.Equals("glyf")) 
				{
					Array.Copy(newGlyfTable, 0, outFont, fontPtr, newGlyfTable.Length);
					fontPtr += newGlyfTable.Length;
					newGlyfTable = null;
				}
				else if (name.Equals("loca")) 
				{
					Array.Copy(newLocaTableOut, 0, outFont, fontPtr, newLocaTableOut.Length);
					fontPtr += newLocaTableOut.Length;
					newLocaTableOut = null;
				}
				else 
				{
					rf.Seek(tableLocation[TABLE_OFFSET]);
					rf.ReadFully(outFont, fontPtr, tableLocation[TABLE_LENGTH]);
					fontPtr += (tableLocation[TABLE_LENGTH] + 3) & (~3);
				}
			}
		}
    
		protected void CreateTableDirectory() 
		{
			tableDirectory = new Hashtable();
			rf.Seek(directoryOffset);
			int id = rf.ReadInt();
			if (id != 0x00010000)
				throw new Exception(fileName + " is not a true type file.");
			int num_tables = rf.ReadUnsignedShort();
			rf.SkipBytes(6);
			for (int k = 0; k < num_tables; ++k) 
			{
				string tag = ReadStandardString(4);
				int[] tableLocation = new int[3];
				tableLocation[TABLE_CHECKSUM] = rf.ReadInt();
				tableLocation[TABLE_OFFSET] = rf.ReadInt();
				tableLocation[TABLE_LENGTH] = rf.ReadInt();
				tableDirectory[tag] = tableLocation;
			}
		}
    
		protected void ReadLoca() 
		{
			int[] tableLocation;
			tableLocation = (int[])tableDirectory["head"];
			if (tableLocation == null)
				throw new Exception("Table 'head' does not exist in " + fileName);
			rf.Seek(tableLocation[TABLE_OFFSET] + HEAD_LOCA_FORMAT_OFFSET);
			locaShortTable = (rf.ReadUnsignedShort() == 0);
			tableLocation = (int[])tableDirectory["loca"];
			if (tableLocation == null)
				throw new Exception("Table 'loca' does not exist in " + fileName);
			rf.Seek(tableLocation[TABLE_OFFSET]);
			if (locaShortTable) 
			{
				int entries = tableLocation[TABLE_LENGTH] / 2;
				locaTable = new int[entries];
				for (int k = 0; k < entries; ++k)
					locaTable[k] = rf.ReadUnsignedShort() * 2;
			}
			else 
			{
				int entries = tableLocation[TABLE_LENGTH] / 4;
				locaTable = new int[entries];
				for (int k = 0; k < entries; ++k)
					locaTable[k] = rf.ReadInt();
			}
		}
    
		protected void CreateNewGlyphTables() 
		{
			newLocaTable = new int[locaTable.Length];
			int[] activeGlyphs = new int[glyphsInList.Count];
			for (int k = 0; k < activeGlyphs.Length; ++k)
				activeGlyphs[k] = (int)glyphsInList[k];
			Array.Sort(activeGlyphs);
			int glyfSize = 0;
			for (int k = 0; k < activeGlyphs.Length; ++k) 
			{
				int glyph = activeGlyphs[k];
				glyfSize += locaTable[glyph + 1] - locaTable[glyph];
			}
			glyfTableRealSize = glyfSize;
			glyfSize = (glyfSize + 3) & (~3);
			newGlyfTable = new byte[glyfSize];
			int glyfPtr = 0;
			int listGlyf = 0;
			for (int k = 0; k < newLocaTable.Length; ++k) 
			{
				newLocaTable[k] = glyfPtr;
				if (listGlyf < activeGlyphs.Length && activeGlyphs[listGlyf] == k) 
				{
					++listGlyf;
					newLocaTable[k] = glyfPtr;
					int start = locaTable[k];
					int len = locaTable[k + 1] - start;
					if (len > 0) 
					{
						rf.Seek(tableGlyphOffset + start);
						rf.ReadFully(newGlyfTable, glyfPtr, len);
						glyfPtr += len;
					}
				}
			}
		}
    
		protected void LocaTobytes() 
		{
			if (locaShortTable)
				locaTableRealSize = newLocaTable.Length * 2;
			else
				locaTableRealSize = newLocaTable.Length * 4;
			newLocaTableOut = new byte[(locaTableRealSize + 3) & (~3)];
			outFont = newLocaTableOut;
			fontPtr = 0;
			for (int k = 0; k < newLocaTable.Length; ++k) 
			{
				if (locaShortTable)
					WriteFontShort(newLocaTable[k] / 2);
				else
					WriteFontInt(newLocaTable[k]);
			}
        
		}
    
		protected void FlatGlyphs() 
		{
			int[] tableLocation;
			tableLocation = (int[])tableDirectory["glyf"];
			if (tableLocation == null)
				throw new Exception("Table 'glyf' does not exist in " + fileName);
			int glyph0 = 0;
			if (!glyphsUsed.ContainsKey(glyph0)) 
			{
				glyphsUsed[glyph0] = null;
				glyphsInList.Add(glyph0);
			}
			tableGlyphOffset = tableLocation[TABLE_OFFSET];
			for (int k = 0; k < glyphsInList.Count; ++k) 
			{
				int glyph = (int)glyphsInList[k];
				CheckGlyphComposite(glyph);
			}
		}

		protected void CheckGlyphComposite(int glyph) 
		{
			int start = locaTable[glyph];
			if (start == locaTable[glyph + 1]) 
				return;
			rf.Seek(tableGlyphOffset + start);
			int numContours = rf.ReadShort();
			if (numContours >= 0)
				return;
			rf.SkipBytes(8);
			for(;;) 
			{
				int flags = rf.ReadUnsignedShort();
				int cGlyph = rf.ReadUnsignedShort();
				if (!glyphsUsed.ContainsKey(cGlyph)) 
				{
					glyphsUsed[cGlyph] = null;
					glyphsInList.Add(cGlyph);
				}
				if ((flags & MORE_COMPONENTS) == 0)
					return;
				int skip;
				if ((flags & ARG_1_AND_2_ARE_WORDS) != 0)
					skip = 4;
				else
					skip = 2;
				if ((flags & WE_HAVE_A_SCALE) != 0)
					skip += 2;
				else if ((flags & WE_HAVE_AN_X_AND_Y_SCALE) != 0)
					skip += 4;
				if ((flags & WE_HAVE_A_TWO_BY_TWO) != 0)
					skip += 8;
				rf.SkipBytes(skip);
			}
		}
    

    
		protected void WriteFontShort(int n) 
		{
			outFont[fontPtr++] = (byte)(n >> 8);
			outFont[fontPtr++] = (byte)(n);
		}

		protected void WriteFontInt(int n) 
		{
			outFont[fontPtr++] = (byte)(n >> 24);
			outFont[fontPtr++] = (byte)(n >> 16);
			outFont[fontPtr++] = (byte)(n >> 8);
			outFont[fontPtr++] = (byte)(n);
		}

		protected void WriteFontString(string s) 
		{
			//byte[] b = PdfEncodings.ConvertToBytes(s, BaseFont.WINANSI);


			byte[] b = Encoding.ASCII.GetBytes(s);

			
			Array.Copy(b, 0, outFont, fontPtr, b.Length);
			fontPtr += b.Length;
		}
    
		protected int CalculateChecksum(byte[] b) 
		{
			int len = b.Length / 4;
			int v0 = 0;
			int v1 = 0;
			int v2 = 0;
			int v3 = 0;
			int ptr = 0;
			for (int k = 0; k < len; ++k) 
			{
				v3 += (int)b[ptr++] & 0xff;
				v2 += (int)b[ptr++] & 0xff;
				v1 += (int)b[ptr++] & 0xff;
				v0 += (int)b[ptr++] & 0xff;
			}
			return v0 + (v1 << 8) + (v2 << 16) + (v3 << 24);
		}

		internal void ReadCMaps() 
		{
			int[] table_location;
			table_location = (int[])tables["cmap"];
			if (table_location == null)
				throw new Exception("Table 'cmap' does not exist in " + fileName + style);
			rf.Seek(table_location[0]);
			rf.SkipBytes(2);
			int num_tables = rf.ReadUnsignedShort();
			fontSpecific = false;
			int map10 = 0;
			int map31 = 0;
			int map30 = 0;
			for (int k = 0; k < num_tables; ++k) 
			{
				int platId = rf.ReadUnsignedShort();
				int platSpecId = rf.ReadUnsignedShort();
				int offset = rf.ReadInt();
				if (platId == 3 && platSpecId == 0) 
				{
					fontSpecific = true;
					map30 = offset;
				}
				else if (platId == 3 && platSpecId == 1) 
				{
					map31 = offset;
				}
				if (platId == 1 && platSpecId == 0) 
				{
					map10 = offset;
				}
			}
			if (map10 > 0) 
			{
				rf.Seek(table_location[0] + map10);
				int format = rf.ReadUnsignedShort();
				switch (format) 
				{
					case 0:
						cmap10 = ReadFormat0();
						break;
					case 4:
						cmap10 = ReadFormat4();
						break;
					case 6:
						cmap10 = ReadFormat6();
						break;
				}
			}
			if (map31 > 0) 
			{
				rf.Seek(table_location[0] + map31);
				int format = rf.ReadUnsignedShort();
				if (format == 4) 
				{
					cmap31 = ReadFormat4();
				}
			}
			if (map30 > 0) 
			{
				rf.Seek(table_location[0] + map30);
				int format = rf.ReadUnsignedShort();
				if (format == 4) 
				{
					cmap10 = ReadFormat4();
				}
			}
		}


		internal Hashtable ReadFormat0() 
		{
			Hashtable h = new Hashtable();
			rf.SkipBytes(4);
			for (int k = 0; k < 256; ++k) 
			{
				int[] r = new int[2];
				r[0] = rf.ReadUnsignedByte();
				r[1] = GetGlyphWidth(r[0]);
				h[k] = r;
			}
			return h;
		}
    
		/** The information in the maps of the table 'cmap' is coded in several formats.
		 *  Format 4 is the Microsoft standard character to glyph index mapping table.
		 * @return a <CODE>Hashtable</CODE> representing this map
		 * @throws IOException the font file could not be read
		 */
		internal Hashtable ReadFormat4() 
		{
			Hashtable h = new Hashtable();
			int table_lenght = rf.ReadUnsignedShort();
			rf.SkipBytes(2);
			int segCount = rf.ReadUnsignedShort() / 2;
			rf.SkipBytes(6);
			int[] endCount = new int[segCount];
			for (int k = 0; k < segCount; ++k) 
			{
				endCount[k] = rf.ReadUnsignedShort();
			}
			rf.SkipBytes(2);
			int[] startCount = new int[segCount];
			for (int k = 0; k < segCount; ++k) 
			{
				startCount[k] = rf.ReadUnsignedShort();
			}
			int[] idDelta = new int[segCount];
			for (int k = 0; k < segCount; ++k) 
			{
				idDelta[k] = rf.ReadUnsignedShort();
			}
			int[] idRO = new int[segCount];
			for (int k = 0; k < segCount; ++k) 
			{
				idRO[k] = rf.ReadUnsignedShort();
			}
			int[] glyphId = new int[table_lenght / 2 - 8 - segCount * 4];
			for (int k = 0; k < glyphId.Length; ++k) 
			{
				glyphId[k] = rf.ReadUnsignedShort();
			}
			for (int k = 0; k < segCount; ++k) 
			{
				int glyph;
				for (int j = startCount[k]; j <= endCount[k] && j != 0xFFFF; ++j) 
				{
					if (idRO[k] == 0) 
					{
						glyph = (j + idDelta[k]) & 0xFFFF;
					}
					else 
					{
						int idx = k + idRO[k] / 2 - segCount + j - startCount[k];
						if (idx >= glyphId.Length)
							continue;
						glyph = (glyphId[idx] + idDelta[k]) & 0xFFFF;
					}
					int[] r = new int[2];
					r[0] = glyph;
					r[1] = GetGlyphWidth(r[0]);
					h[fontSpecific ? ((j & 0xff00) == 0xf000 ? j & 0xff : j) : j] = r;
				}
			}
			return h;
		}
    
		/** The information in the maps of the table 'cmap' is coded in several formats.
		 *  Format 6 is a trimmed table mapping. It is similar to format 0 but can have
		 *  less than 256 entries.
		 * @return a <CODE>Hashtable</CODE> representing this map
		 * @throws IOException the font file could not be read
		 */
		internal Hashtable ReadFormat6() 
		{
			Hashtable h = new Hashtable();
			rf.SkipBytes(4);
			int start_code = rf.ReadUnsignedShort();
			int code_count = rf.ReadUnsignedShort();
			for (int k = 0; k < code_count; ++k) 
			{
				int[] r = new int[2];
				r[0] = rf.ReadUnsignedShort();
				r[1] = GetGlyphWidth(r[0]);
				h[k + start_code] = r;
			}
			return h;
		}
    
		/** Reads the kerning information from the 'kern' table.
		 * @throws IOException the font file could not be read
		 */
		
		internal void ReadKerning() 
		{
			int[] table_location;
			table_location = (int[])tables["kern"];
			if (table_location == null)
				return;
			rf.Seek(table_location[0] + 2);
			int nTables = rf.ReadUnsignedShort();
			int checkpoint = table_location[0] + 4;
			int length = 0;
			for (int k = 0; k < nTables; ++k) 
			{
				checkpoint += length;
				rf.Seek(checkpoint);
				rf.SkipBytes(2);
				length = rf.ReadUnsignedShort();
				int coverage = rf.ReadUnsignedShort();
				if ((coverage & 0xfff7) == 0x0001) 
				{
					int nPairs = rf.ReadUnsignedShort();
					rf.SkipBytes(6);
					for (int j = 0; j < nPairs; ++j) 
					{
						int pair = rf.ReadInt();
						int value = ((int)rf.ReadShort() * 1000) / head.unitsPerEm;
						kerning[pair] = value;
					}
				}
			}
		}
		
		


		protected int GetGlyphWidth(int glyph) 
		{
			if (glyph >= GlyphWidths.Length)
				glyph = GlyphWidths.Length - 1;
			return GlyphWidths[glyph];
		}

		private void ReadBbox() 
		{
			int[] tableLocation;
			tableLocation = (int[])tables["head"];
			if (tableLocation == null)
				throw new Exception("Table 'head' does not exist in " + fileName + style);
			rf.Seek(tableLocation[0] + HEAD_LOCA_FORMAT_OFFSET);
			bool locaShortTable = (rf.ReadUnsignedShort() == 0);
			tableLocation = (int[])tables["loca"];
			if (tableLocation == null)
				return;
			rf.Seek(tableLocation[0]);
			int[] locaTable;
			if (locaShortTable) 
			{
				int entries = tableLocation[1] / 2;
				locaTable = new int[entries];
				for (int k = 0; k < entries; ++k)
					locaTable[k] = rf.ReadUnsignedShort() * 2;
			}
			else 
			{
				int entries = tableLocation[1] / 4;
				locaTable = new int[entries];
				for (int k = 0; k < entries; ++k)
					locaTable[k] = rf.ReadInt();
			}
			tableLocation = (int[])tables["glyf"];
			if (tableLocation == null)
				throw new Exception("Table 'glyf' does not exist in " + fileName + style);
			int tableGlyphOffset = tableLocation[0];
			bboxes = new int[locaTable.Length - 1][];
			for (int glyph = 0; glyph < locaTable.Length - 1; ++glyph) 
			{
				int start = locaTable[glyph];
				if (start != locaTable[glyph + 1]) 
				{
					rf.Seek(tableGlyphOffset + start + 2);
					bboxes[glyph] = new int[]{
												 (rf.ReadShort() * 1000) / head.unitsPerEm,
												 (rf.ReadShort() * 1000) / head.unitsPerEm,
												 (rf.ReadShort() * 1000) / head.unitsPerEm,
												 (rf.ReadShort() * 1000) / head.unitsPerEm};
				}
			}
		}


		protected void ReadGlyphWidths() 
		{
			int[] table_location;
			table_location = (int[])tables["hmtx"];
			if (table_location == null)
				throw new Exception("Table 'hmtx' does not exist in " + fileName + style);
			rf.Seek(table_location[0]);
			GlyphWidths = new int[hhea.numberOfHMetrics];
			for (int k = 0; k < hhea.numberOfHMetrics; ++k) 
			{
				GlyphWidths[k] = (rf.ReadUnsignedShort() * 1000) / head.unitsPerEm;
				rf.ReadUnsignedShort();
			}
		}


		internal class FontHeader 
		{
			/** A variable. */
			internal int flags;
			/** A variable. */
			internal int unitsPerEm;
			/** A variable. */
			internal short xMin;
			/** A variable. */
			internal short yMin;
			/** A variable. */
			internal short xMax;
			/** A variable. */
			internal short yMax;
			/** A variable. */
			internal int macStyle;
		}

		internal class HorizontalHeader 
		{
			/** A variable. */
			internal short Ascender;
			/** A variable. */
			internal short Descender;
			/** A variable. */
			internal short LineGap;
			/** A variable. */
			internal int advanceWidthMax;
			/** A variable. */
			internal short minLeftSideBearing;
			/** A variable. */
			internal short minRightSideBearing;
			/** A variable. */
			internal short xMaxExtent;
			/** A variable. */
			internal short caretSlopeRise;
			/** A variable. */
			internal short caretSlopeRun;
			/** A variable. */
			internal int numberOfHMetrics;
		}

		public int[] GetMetricsTT(int c) 
		{
			if (!fontSpecific && cmap31 != null) 
				return (int[])cmap31[c];
			if (fontSpecific && cmap10 != null) 
				return (int[])cmap10[c];
			if (cmap31 != null) 
				return (int[])cmap31[c];
			if (cmap10 != null) 
				return (int[])cmap10[c];
			return null;
		}


		internal void CalculateMetrics(string text)
		{
			int len = text.Length;
			int[] metrics = null;
			char[] glyph = new char[len];
			int i = 0;

			for (int k = 0; k < len; ++k) 
			{
				char c = text[k];
				metrics = GetMetricsTT(c);
				if (metrics == null)
					continue;
				int m0 = metrics[0];
				int gl = m0;
				if (!glyphsUsed.ContainsKey(gl))
					glyphsUsed[gl] = new int[]{m0, metrics[1], c};

				if (!charactersUsed.Contains(c))
					charactersUsed[c] = metrics;

				glyph[i++] = (char)m0;
			}

			glyphsInList = new ArrayList(glyphsUsed.Keys);
		}

		internal void FillTables() 
		{
			int[] table_location;
			table_location = (int[])tables["head"];
			if (table_location == null)
				throw new Exception("Table 'head' does not exist in " + fileName + style);
			rf.Seek(table_location[0] + 16);
			head.flags = rf.ReadUnsignedShort();
			head.unitsPerEm = rf.ReadUnsignedShort();
			rf.SkipBytes(16);
			head.xMin = rf.ReadShort();
			head.yMin = rf.ReadShort();
			head.xMax = rf.ReadShort();
			head.yMax = rf.ReadShort();
			head.macStyle = rf.ReadUnsignedShort();
        
			table_location = (int[])tables["hhea"];
			if (table_location == null)
				throw new Exception("Table 'hhea' does not exist " + fileName + style);
			rf.Seek(table_location[0] + 4);
			hhea.Ascender = rf.ReadShort();
			hhea.Descender = rf.ReadShort();
			hhea.LineGap = rf.ReadShort();
			hhea.advanceWidthMax = rf.ReadUnsignedShort();
			hhea.minLeftSideBearing = rf.ReadShort();
			hhea.minRightSideBearing = rf.ReadShort();
			hhea.xMaxExtent = rf.ReadShort();
			hhea.caretSlopeRise = rf.ReadShort();
			hhea.caretSlopeRun = rf.ReadShort();
			rf.SkipBytes(12);
			hhea.numberOfHMetrics = rf.ReadUnsignedShort();
        
			table_location = (int[])tables["OS/2"];
			if (table_location == null)
				throw new Exception("Table 'OS/2' does not exist in " + fileName + style);
			rf.Seek(table_location[0]);
			int version = rf.ReadUnsignedShort();
			os_2.xAvgCharWidth = rf.ReadShort();
			os_2.usWeightClass = rf.ReadUnsignedShort();
			os_2.usWidthClass = rf.ReadUnsignedShort();
			os_2.fsType = rf.ReadShort();
			os_2.ySubscriptXSize = rf.ReadShort();
			os_2.ySubscriptYSize = rf.ReadShort();
			os_2.ySubscriptXOffset = rf.ReadShort();
			os_2.ySubscriptYOffset = rf.ReadShort();
			os_2.ySuperscriptXSize = rf.ReadShort();
			os_2.ySuperscriptYSize = rf.ReadShort();
			os_2.ySuperscriptXOffset = rf.ReadShort();
			os_2.ySuperscriptYOffset = rf.ReadShort();
			os_2.yStrikeoutSize = rf.ReadShort();
			os_2.yStrikeoutPosition = rf.ReadShort();
			os_2.sFamilyClass = rf.ReadShort();
			rf.ReadFully(os_2.panose);
			rf.SkipBytes(16);
			rf.ReadFully(os_2.achVendID);
			os_2.fsSelection = rf.ReadUnsignedShort();
			os_2.usFirstCharIndex = rf.ReadUnsignedShort();
			os_2.usLastCharIndex = rf.ReadUnsignedShort();
			os_2.sTypoAscender = rf.ReadShort();
			os_2.sTypoDescender = rf.ReadShort();
			if (os_2.sTypoDescender > 0)
				os_2.sTypoDescender = (short)(-os_2.sTypoDescender);
			os_2.sTypoLineGap = rf.ReadShort();
			os_2.usWinAscent = rf.ReadUnsignedShort();
			os_2.usWinDescent = rf.ReadUnsignedShort();
			os_2.ulCodePageRange1 = 0;
			os_2.ulCodePageRange2 = 0;
			if (version > 0) 
			{
				os_2.ulCodePageRange1 = rf.ReadInt();
				os_2.ulCodePageRange2 = rf.ReadInt();
			}
			if (version > 1) 
			{
				rf.SkipBytes(2);
				os_2.sCapHeight = rf.ReadShort();
			}
			else
				os_2.sCapHeight = (int)(0.7 * head.unitsPerEm);
        
			table_location = (int[])tables["post"];
			if (table_location == null) 
			{
				italicAngle = -Math.Atan2(hhea.caretSlopeRun, hhea.caretSlopeRise) * 180 / Math.PI;
				return;
			}
			rf.Seek(table_location[0] + 4);
			short mantissa = rf.ReadShort();
			int fraction = rf.ReadUnsignedShort();
			italicAngle = (double)mantissa + (double)fraction / 16384.0;
			rf.SkipBytes(4);
			isFixedPitch = rf.ReadInt() != 0;
		}

		internal class WindowsMetrics 
		{
			/** A variable. */
			internal short xAvgCharWidth;
			/** A variable. */
			internal int usWeightClass;
			/** A variable. */
			internal int usWidthClass;
			/** A variable. */
			internal short fsType;
			/** A variable. */
			internal short ySubscriptXSize;
			/** A variable. */
			internal short ySubscriptYSize;
			/** A variable. */
			internal short ySubscriptXOffset;
			/** A variable. */
			internal short ySubscriptYOffset;
			/** A variable. */
			internal short ySuperscriptXSize;
			/** A variable. */
			internal short ySuperscriptYSize;
			/** A variable. */
			internal short ySuperscriptXOffset;
			/** A variable. */
			internal short ySuperscriptYOffset;
			/** A variable. */
			internal short yStrikeoutSize;
			/** A variable. */
			internal short yStrikeoutPosition;
			/** A variable. */
			internal short sFamilyClass;
			/** A variable. */
			internal byte[] panose = new byte[10];
			/** A variable. */
			internal byte[] achVendID = new byte[4];
			/** A variable. */
			internal int fsSelection;
			/** A variable. */
			internal int usFirstCharIndex;
			/** A variable. */
			internal int usLastCharIndex;
			/** A variable. */
			internal short sTypoAscender;
			/** A variable. */
			internal short sTypoDescender;
			/** A variable. */
			internal short sTypoLineGap;
			/** A variable. */
			internal int usWinAscent;
			/** A variable. */
			internal int usWinDescent;
			/** A variable. */
			internal int ulCodePageRange1;
			/** A variable. */
			internal int ulCodePageRange2;
			/** A variable. */
			internal int sCapHeight;
		}


		public double MeasureText(string text, float fontSize)
		{
			double length = 0;
			foreach (char c in text)
			{
				int[] metrics = (int[])charactersUsed[c];
				if (metrics != null)
				{
					length += fontSize * metrics[1] / 1000;
				}

			}
			return length;
		}

		


		protected void AddRangeUni(Hashtable longTag, bool includeMetrics, bool subsetp) 
		{
			if (!subsetp && (subsetRanges != null || directoryOffset > 0)) 
			{
				int[] rg = (subsetRanges == null && directoryOffset > 0) ? new int[]{0, 0xffff} : CompactRanges(subsetRanges);
				Hashtable usemap;
				if (!fontSpecific && cmap31 != null) 
					usemap = cmap31;
				else if (fontSpecific && cmap10 != null) 
					usemap = cmap10;
				else if (cmap31 != null) 
					usemap = cmap31;
				else 
					usemap = cmap10;
				foreach (DictionaryEntry e in usemap) 
				{
					int[] v = (int[])e.Value;
					int gi = (int)v[0];
					if (longTag.ContainsKey(gi))
						continue;
					int c = (int)e.Key;
					bool skip = true;
					for (int k = 0; k < rg.Length; k += 2) 
					{
						if (c >= rg[k] && c <= rg[k + 1]) 
						{
							skip = false;
							break;
						}
					}
					if (!skip)
						longTag[gi] = includeMetrics ? new int[]{v[0], v[1], c} : null;
				}
			}
		}


		protected static int[] CompactRanges(ArrayList ranges) 
		{
			ArrayList simp = new ArrayList();
			for (int k = 0; k < ranges.Count; ++k) 
			{
				int[] r = (int[])ranges[k];
				for (int j = 0; j < r.Length; j += 2) 
				{
					simp.Add(new int[]{Math.Max(0, Math.Min(r[j], r[j + 1])), Math.Min(0xffff, Math.Max(r[j], r[j + 1]))});
				}
			}
			for (int k1 = 0; k1 < simp.Count - 1; ++k1) 
			{
				for (int k2 = k1 + 1; k2 < simp.Count; ++k2) 
				{
					int[] r1 = (int[])simp[k1];
					int[] r2 = (int[])simp[k2];
					if ((r1[0] >= r2[0] && r1[0] <= r2[1]) || (r1[1] >= r2[0] && r1[0] <= r2[1])) 
					{
						r1[0] = Math.Min(r1[0], r2[0]);
						r1[1] = Math.Max(r1[1], r2[1]);
						simp.RemoveAt(k2);
						--k2;
					}
				}
			}
			int[] s = new int[simp.Count * 2];
			for (int k = 0; k < simp.Count; ++k) 
			{
				int[] r = (int[])simp[k];
				s[k * 2] = r[0];
				s[k * 2 + 1] = r[1];
			}
			return s;
		}
	
	}

	internal class IntHashtable 
	{
		/// The hash table data.
		private IntHashtableEntry[] table;
    
		/// The total number of entries in the hash table.
		private int count;
    
		/// Rehashes the table when count exceeds this threshold.
		private int threshold;
    
		/// The load factor for the hashtable.
		private float loadFactor;
    
		/// Constructs a new, empty hashtable with the specified initial
		// capacity and the specified load factor.
		// @param initialCapacity the initial number of buckets
		// @param loadFactor a number between 0.0 and 1.0, it defines
		//      the threshold for rehashing the hashtable into
		//      a bigger one.
		// @exception IllegalArgumentException If the initial capacity
		// is less than or equal to zero.
		// @exception IllegalArgumentException If the load factor is
		// less than or equal to zero.
		public IntHashtable( int initialCapacity, float loadFactor ) 
		{
			if ( initialCapacity <= 0 || loadFactor <= 0.0 )
				throw new ArgumentException();
			this.loadFactor = loadFactor;
			table = new IntHashtableEntry[initialCapacity];
			threshold = (int) ( initialCapacity * loadFactor );
		}
    
		/// Constructs a new, empty hashtable with the specified initial
		// capacity.
		// @param initialCapacity the initial number of buckets
		public IntHashtable( int initialCapacity ) : this( initialCapacity, 0.75f ) {}
    
		/// Constructs a new, empty hashtable. A default capacity and load factor
		// is used. Note that the hashtable will automatically grow when it gets
		// full.
		public IntHashtable() : this( 101, 0.75f ) {}
    
		/// Returns the number of elements contained in the hashtable.
		public int Size 
		{
			get 
			{
				return count;
			}
		}
    
		/// Returns true if the hashtable contains no elements.
		public bool IsEmpty() 
		{
			return count == 0;
		}
    
		/// Returns true if the specified object is an element of the hashtable.
		// This operation is more expensive than the ContainsKey() method.
		// @param value the value that we are looking for
		// @exception NullPointerException If the value being searched
		// for is equal to null.
		// @see IntHashtable#containsKey
		public bool Contains( int value ) 
		{
			IntHashtableEntry[] tab = table;
			for ( int i = tab.Length ; i-- > 0 ; ) 
			{
				for ( IntHashtableEntry e = tab[i] ; e != null ; e = e.next ) 
				{
					if ( e.value == value )
						return true;
				}
			}
			return false;
		}
    
		/// Returns true if the collection contains an element for the key.
		// @param key the key that we are looking for
		// @see IntHashtable#contains
		public bool ContainsKey( int key ) 
		{
			IntHashtableEntry[] tab = table;
			int hash = key;
			int index = ( hash & 0x7FFFFFFF ) % tab.Length;
			for ( IntHashtableEntry e = tab[index] ; e != null ; e = e.next ) 
			{
				if ( e.hash == hash && e.key == key )
					return true;
			}
			return false;
		}
    
		/// Gets the object associated with the specified key in the
		// hashtable.
		// @param key the specified key
		// @returns the element for the key or null if the key
		//      is not defined in the hash table.
		// @see IntHashtable#put
		public int this[int key] 
		{
			get 
			{
				IntHashtableEntry[] tab = table;
				int hash = key;
				int index = ( hash & 0x7FFFFFFF ) % tab.Length;
				for ( IntHashtableEntry e = tab[index] ; e != null ; e = e.next ) 
				{
					if ( e.hash == hash && e.key == key )
						return e.value;
				}
				return 0;
			}

			set 
			{
				// Makes sure the key is not already in the hashtable.
				IntHashtableEntry[] tab = table;
				int hash = key;
				int index = ( hash & 0x7FFFFFFF ) % tab.Length;
				for ( IntHashtableEntry e = tab[index] ; e != null ; e = e.next ) 
				{
					if ( e.hash == hash && e.key == key ) 
					{
						e.value = value;
						return;
					}
				}
        
				if ( count >= threshold ) 
				{
					// Rehash the table if the threshold is exceeded.
					Rehash();
					this[key] = value;
					return;
				}
        
				// Creates the new entry.
				IntHashtableEntry en = new IntHashtableEntry();
				en.hash = hash;
				en.key = key;
				en.value = value;
				en.next = tab[index];
				tab[index] = en;
				++count;
			}
		}
    
		/// Rehashes the content of the table into a bigger table.
		// This method is called automatically when the hashtable's
		// size exceeds the threshold.
		protected void Rehash() 
		{
			int oldCapacity = table.Length;
			IntHashtableEntry[] oldTable = table;
        
			int newCapacity = oldCapacity * 2 + 1;
			IntHashtableEntry[] newTable = new IntHashtableEntry[newCapacity];
        
			threshold = (int) ( newCapacity * loadFactor );
			table = newTable;
        
			for ( int i = oldCapacity ; i-- > 0 ; ) 
			{
				for ( IntHashtableEntry old = oldTable[i] ; old != null ; ) 
				{
					IntHashtableEntry e = old;
					old = old.next;
                
					int index = ( e.hash & 0x7FFFFFFF ) % newCapacity;
					e.next = newTable[index];
					newTable[index] = e;
				}
			}
		}
    
		/// Removes the element corresponding to the key. Does nothing if the
		// key is not present.
		// @param key the key that needs to be removed
		// @return the value of key, or null if the key was not found.
		public int Remove( int key ) 
		{
			IntHashtableEntry[] tab = table;
			int hash = key;
			int index = ( hash & 0x7FFFFFFF ) % tab.Length;
			for ( IntHashtableEntry e = tab[index], prev = null ; e != null ; prev = e, e = e.next ) 
			{
				if ( e.hash == hash && e.key == key ) 
				{
					if ( prev != null )
						prev.next = e.next;
					else
						tab[index] = e.next;
					--count;
					return e.value;
				}
			}
			return 0;
		}
    
		/// Clears the hash table so that it has no more elements in it.
		public void Clear() 
		{
			IntHashtableEntry[] tab = table;
			for ( int index = tab.Length; --index >= 0; )
				tab[index] = null;
			count = 0;
		}
    
		public IntHashtable Clone() 
		{
			IntHashtable t = new IntHashtable();
			t.count = count;
			t.loadFactor = loadFactor;
			t.threshold = threshold;
			t.table = new IntHashtableEntry[table.Length];
			for (int i = table.Length ; i-- > 0 ; ) 
			{
				t.table[i] = (table[i] != null)
					? (IntHashtableEntry)table[i].Clone() : null;
			}
			return t;
		}

		public int[] ToOrderedKeys() 
		{
			int[] res = GetKeys();
			Array.Sort(res);
			return res;
		}
        
		public int[] GetKeys() 
		{
			int[] res = new int[count];
			int ptr = 0;
			int index = table.Length;
			IntHashtableEntry entry = null;
			while (true) 
			{
				if (entry == null)
					while ((index-- > 0) && ((entry = table[index]) == null));
				if (entry == null)
					break;
				IntHashtableEntry e = entry;
				entry = e.next;
				res[ptr++] = e.key;
			}
			return res;
		}
    
		public class IntHashtableEntry 
		{
			internal int hash;
			internal int key;
			internal int value;
			internal IntHashtableEntry next;
            
			public int Key 
			{
				get 
				{
					return key;
				}
			}
            
			public int Value 
			{
				get 
				{
					return value;
				}
			}
            
			protected internal IntHashtableEntry Clone() 
			{
				IntHashtableEntry entry = new IntHashtableEntry();
				entry.hash = hash;
				entry.key = key;
				entry.value = value;
				entry.next = (next != null) ? next.Clone() : null;
				return entry;
			}
		}    

		public IntHashtableIterator GetEntryIterator() 
		{
			return new IntHashtableIterator(table);
		}
        
		public class IntHashtableIterator 
		{
			//    boolean keys;
			int index;
			IntHashtableEntry[] table;
			IntHashtableEntry entry;
            
			internal IntHashtableIterator(IntHashtableEntry[] table) 
			{
				this.table = table;
				this.index = table.Length;
			}
            
			public bool HasNext() 
			{
				if (entry != null) 
				{
					return true;
				}
				while (index-- > 0) 
				{
					if ((entry = table[index]) != null) 
					{
						return true;
					}
				}
				return false;
			}
            
			public IntHashtableEntry Next() 
			{
				if (entry == null) 
				{
					while ((index-- > 0) && ((entry = table[index]) == null));
				}
				if (entry != null) 
				{
					IntHashtableEntry e = entry;
					entry = e.next;
					return e;
				}
				throw new InvalidOperationException("IntHashtableIterator");
			}
		} 
       

		
	}



}
