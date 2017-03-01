using System;
using System.Collections;
using System.IO;
using System.Text;
using Microsoft.Win32;

namespace Com.Delta.PrintManager.Engine.Pdf.Fonts
{
	/// <summary>
	/// Summary description for FontFileResolver.
	/// </summary>
	internal class FontFileResolver
	{
		RandomAccessBuffer rf;
		protected Hashtable tables = new Hashtable();

		public FontFileResolver()
		{
		}


		public Hashtable GetFontList()
		{
			Hashtable ttFonts = new Hashtable();

			try
			{
				DirectoryInfo di = new DirectoryInfo(GetFontDirectory());
	
				if (di.Exists)
				{
				
					FileInfo[] fontFiles = di.GetFiles();
			
					for (int i=0;i<fontFiles.Length;i++)
					{
						string filename = fontFiles[i].FullName;
						rf = new RandomAccessBuffer(filename);

						rf.Seek(0);
						int ttId = rf.ReadInt();

						if (ttId != 0x00010000 && ttId != 0x4F54544F)
							continue;


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


						ttFonts[fullName[0][3]] = filename;

						
						
				
					}
				}


				//Console.WriteLine("Before : " + ttFonts.Count);
				
				ReadRegistry(ttFonts);

				//Console.WriteLine("After : " + ttFonts.Count);
			}
			catch (Exception)
			{

			}

			return ttFonts;
			
		}


		private void ReadFontDirectory(Hashtable fonts)
		{

		}


		private void ReadRegistry(Hashtable fonts)
		{
			try
			{
				string fontFolder = GetFontDirectory();

				DirectoryInfo di = new DirectoryInfo(System.Environment.SystemDirectory);
				int index = di.Parent.FullName.LastIndexOf("\\");
				string windowsFolder = di.Parent.FullName.Substring(index).ToUpper();

				string regKey = String.Empty;
				if (windowsFolder == @"\WINNT")
					regKey = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts";
				else
					regKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Fonts";


				RegistryKey fontRegistry = Registry.LocalMachine.OpenSubKey(regKey);
				foreach(string s in fontRegistry.GetValueNames())
				{
					
					string fontName = s;
					string fontFile = fontFolder + Path.DirectorySeparatorChar + fontRegistry.GetValue(s).ToString();

					int ttfIndex = s.IndexOf("(TrueType)");
					if (ttfIndex > -1)
					{
						fontName = s.Substring(0, ttfIndex).Trim();
					}

					string extension = Path.GetExtension(fontFile).ToUpper();

					if (!fonts.ContainsKey(fontName) && (extension==".TTF" ||extension==".OTF"))
						fonts[fontName] =  fontFile;

				}
			}
			catch (Exception){}
		}

		private string GetFontDirectory()
		{
			DirectoryInfo di = new DirectoryInfo(System.Environment.SystemDirectory);
			DirectoryInfo fontFolder = new DirectoryInfo( di.Parent.FullName + Path.DirectorySeparatorChar + "fonts");
			return fontFolder.FullName;
		}

		protected string ReadStandardString(int length) 
		{
			byte[] buf = new byte[length];
			rf.ReadFully(buf);
			return System.Text.Encoding.GetEncoding(1252).GetString(buf);
		}

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

		internal string[][] GetNames(int id) 
		{
			int[] table_location;
			table_location = (int[])tables["name"];
			if (table_location == null)
				return null;

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

					names.Add(new string[]{platformID.ToString(), platformEncodingID.ToString(), languageID.ToString(), name});
					rf.Seek(pos);

				}				
			}
			string[][] thisName = new string[names.Count][];
			for (int k = 0; k < names.Count; ++k)
				thisName[k] = (string[])names[k];
			return thisName;
		}
	}
}
