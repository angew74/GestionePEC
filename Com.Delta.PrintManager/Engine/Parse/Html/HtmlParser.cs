using System;
using System.IO;
using System.Collections;
using System.Text;




namespace Com.Delta.PrintManager.Engine.Parse.Html

{
	/// <summary>
	/// HTML content parser
	/// 
	/// </summary>
	internal class HtmlParser : IDisposable
	{
		

		/// <summary>
		/// If true (default: false) then parsed tag chunks will contain raw HTML, otherwise only comments will have it set
		/// <p>
		/// Performance hint: keep it as false, you can always get to original HTML as each chunk contains
		/// offset from which parsing started and finished, thus allowing to set exact HTML that was parsed
		/// </p>
		/// </summary>
		public bool bKeepRawHTML=false;

		/// <summary>
		/// If true (default) then HTML for comments tags themselves AND between them will be set to oHTML variable, otherwise it will be empty
		/// but you can always set it later 
		/// </summary>
		public bool bAutoKeepComments=true;

		/// <summary>
		/// If true (default: false) then HTML for script tags themselves AND between them will be set to oHTML variable, otherwise it will be empty
		/// but you can always set it later
		/// </summary>
		internal bool bAutoKeepScripts=true;

		/// <summary>
		/// If true (and either bAutoKeepComments or bAutoKeepScripts is true), then oHTML will be set
		/// to data BETWEEN tags excluding those tags themselves, as otherwise FULL HTML will be set, ie:
		/// '<!-- comments -->' but if this is set to true then only ' comments ' will be returned
		/// </summary>
		public bool bAutoExtractBetweenTagsOnly=true;

		/// <summary>
		/// Long winded name... by default if tag is closed BUT it has got parameters then we will consider it
		/// open tag, this is not right for proper XML parsing
		/// </summary>
		public bool bAutoMarkClosedTagsWithParamsAsOpen=true;

		/// <summary>
		/// If true (default), then all whitespace before TAG starts will be compressed to single space char (32 or 0x20)
		/// this makes parser run a bit faster, if you need exact whitespace before tags then change this flag to FALSE
		/// </summary>
		public bool bCompressWhiteSpaceBeforeTag=true;

		

		/// <summary>
		/// Heuristics engine used by Tag Parser to quickly match known tags and attribute names, can be disabled
		/// or you can add more tags to it to fit your most likely cases, it is currently tuned for HTML
		/// </summary>
		private HTMLheuristics oHE = new HTMLheuristics();

		
		private DynaString sText = new DynaString("");

		/// <summary>
		/// This chunk will be returned when it was parsed
		/// </summary>
		HTMLchunk oChunk = new HTMLchunk(true);

		/// <summary>
		/// Tag parser object
		/// </summary>
		TagParser oTP=new TagParser();

		/// <summary>
		/// Encoding used to convert binary data into string
		/// </summary>
		public Encoding oEnc=null;

		/// <summary>
		/// Byte array with HTML will be kept here
		/// </summary>
		internal byte[] bHTML;

		/// <summary>
		/// Current position pointing to byte in bHTML
		/// </summary>
		internal int iCurPos=0;

		/// <summary>
		/// Length of bHTML -- it appears to be faster to use it than bHTML.Length
		/// </summary>
		int iDataLength=0;

		/// <summary>
		/// Whitespace lookup table - 0 is not whitespace, otherwise it is
		/// </summary>
		static byte[] bWhiteSpace=new byte[byte.MaxValue+1];

		/// <summary>
		/// Entities manager
		/// </summary>
		HTMLentities oE=new HTMLentities();

		static HtmlParser()
		{
			// set bytes that are whitespace
			bWhiteSpace[' ']=1;
			bWhiteSpace['\t']=1;
			bWhiteSpace[13]=1;
			bWhiteSpace[10]=1;
		}

		public HtmlParser()
		{
			// init heuristics engine
			oHE.AddTag("a","href");
			oHE.AddTag("b","");
			oHE.AddTag("p","class");
			oHE.AddTag("i","");
			oHE.AddTag("s","");
			oHE.AddTag("u","");

			oHE.AddTag("td","align,valign,bgcolor,rowspan,colspan");
			oHE.AddTag("table","border,width,cellpadding");
			oHE.AddTag("span","");
			oHE.AddTag("option","");
			oHE.AddTag("select","");

			oHE.AddTag("tr","");
			oHE.AddTag("div","class,align");
			oHE.AddTag("img","src,width,height,title,alt");
			oHE.AddTag("input","");
			oHE.AddTag("br","");
			oHE.AddTag("li","");
			oHE.AddTag("ul","");
			oHE.AddTag("ol","");
			oHE.AddTag("hr","");
			oHE.AddTag("h1","");
			oHE.AddTag("h2","");
			oHE.AddTag("h3","");
			oHE.AddTag("h4","");
			oHE.AddTag("h5","");
			oHE.AddTag("h6","");
			oHE.AddTag("font","size,color");
			oHE.AddTag("meta","name,content,http-equiv");
			oHE.AddTag("base","href");
			
			// these are pretty rare
			oHE.AddTag("script","");
			oHE.AddTag("style","");
			oHE.AddTag("html","");
			oHE.AddTag("body","");
		}

		/// <summary>
		/// Sets chunk param hash mode
		/// </summary>
		/// <param name="bHashMode">If true then tag's params will be kept in Chunk's hashtable (slower), otherwise kept in arrays (sParams/sValues)</param>
		public void SetChunkHashMode(bool bHashMode)
		{
			oChunk.bHashMode = bHashMode;
		}

		bool bDisposed=false;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool bDisposing)
		{
			if(!bDisposed)
			{
				bDisposed=true;

				if(oChunk!=null)
				{
					oChunk.Dispose();
					oChunk=null;
				}

				if(sText!=null)
				{
					sText.Dispose();
					sText=null;
				}

				bHTML=null;

				if(oE!=null)
				{
					oE.Dispose();
					oE=null;
				}

				if(oTP!=null)
				{
					oTP.Dispose();
					oTP=null;
				}

			}

		}

		/// <summary>
		/// Sets oHTML variable in a chunk to the raw HTML that was parsed for that chunk.
		/// </summary>
		/// <param name="oChunk">Chunk returned by ParseNext function, it must belong to the same HTMLparser that
		/// was initiated with the same HTML data that this chunk belongs to</param>
		public void SetRawHTML(HTMLchunk oChunk)
		{
			oChunk.oHTML = oEnc.GetString(bHTML,oChunk.iChunkOffset,oChunk.iChunkLength);
		}

		/// <summary>
		/// Closes object and releases all allocated resources
		/// </summary>
		public void Close()
		{
			Dispose();
		}

		/// <summary>
		/// Sets encoding 
		/// </summary>
		/// <param name="encoding">Encoding object</param>
		public void SetEncoding(Encoding encoding)
		{
			oChunk.SetEncoding(encoding);
			sText.SetEncoding(encoding);
			
			if(encoding != null)
				oEnc= encoding;

		}

		


		public string ChangeToEntities(string sLine)
		{
			return ChangeToEntities(sLine,false);
		}

		/// <summary>
		/// Parses line and changes known entiry characters into proper HTML entiries
		/// </summary>
		/// <param name="sLine">Line of text</param>
		/// <returns>Line of text with proper HTML entities</returns>
		public string ChangeToEntities(string sLine,bool bChangeDangerousCharsOnly)
		{
			// PHP does not handle that well, fsckers
			//bChangeAllNonASCII=false;

			try
			{
				// scan string first and if 
				for(int i=0; i<sLine.Length; i++)
				{				
					int cChar=(int)sLine[i];

					// yeah I know its lame but its 3:30am and I had v.long debugging session :-/
					switch(cChar)
					{
						case 0:
						case 39:
						case 145:
						case 146:
						case 147:
						case 148:
							return oE.ChangeToEntities(sLine,i,bChangeDangerousCharsOnly);

						default:

							if(cChar<32) // || (bChangeAllNonASCII && cChar>=127))
								goto case 148;

							break;
					};

					if(cChar<HTMLentities.sEntityReverseLookup.Length && HTMLentities.sEntityReverseLookup[cChar]!=null)
						return oE.ChangeToEntities(sLine,i,bChangeDangerousCharsOnly);
				}

			}
			catch(Exception oEx)
			{
				Console.WriteLine("Entity exception: "+oEx.ToString());
			}

			// nothing need to be changed
			return sLine;
		}


		/// <summary>
		/// HtmlParser constructor
		/// </summary>
		/// <param name="text"></param>
		public HtmlParser(string text)
		{
			Init(text);
		}

		/// <summary>
		/// Initialises parses with HTML to be parsed from provided string
		/// </summary>
		/// <param name="text">Text in HTML format</param>
		public void Init(string text)
		{			
			byte[] s = Encoding.UTF8.GetBytes(text);		
			Init(s, s.Length);
		}
		

		/// <summary>
		/// Inits parsing
		/// </summary>
		/// <param name="p_bHTML">Data buffer</param>
		/// <param name="p_iHtmlLength">Length of data (buffer itself can be longer) - start offset assumed to be 0</param>
		public void Init(byte[] p_bHTML,int p_iHtmlLength)
		{
			// set default encoding
			oEnc=Encoding.Default;

			CleanUp();
		
			bHTML=p_bHTML;

			// check whether we have got data that is actually in Unicode format
			// normally this would mean we have got plenty of zeroes
			// this and related code was contributed by Martin Bächtold from TTN Tele.Translator.Network
			if(bHTML.Length > 2)
			{
				if(bHTML[0]==255 && bHTML[1]==254)
				{
					bHTML=Encoding.Default.GetBytes(Encoding.Unicode.GetString(bHTML,2,bHTML.Length-2));
				}
			}

			iDataLength=p_iHtmlLength;

			oTP.Init(this,oChunk,sText,bHTML,iDataLength,oE,oHE);
		}

		/// <summary>
		/// Cleans up parser in preparation for next parsing
		/// </summary>
		public void CleanUp()
		{			
			oTP.CleanUp();
			bHTML = null;

			iCurPos=0;
			iDataLength=0;
		}

		/// <summary>
		/// Resets current parsed data to start
		/// </summary>
		private void Reset()
		{
			iCurPos=0;
		}



		/// <summary>
		/// Parses next chunk and returns it with 
		/// </summary>
		/// <returns>HTMLchunk or null if end of data reached</returns>
		public HTMLchunk ParseNext()
		{
			if(iCurPos>=iDataLength)
				return null;

			oChunk.Clear();
			oChunk.iChunkOffset=iCurPos;

			byte cChar=bHTML[iCurPos++];

			// most likely what we have here is a normal char, 
			if(cChar==(byte)'<')
			{
				// tag parsing route - we know for sure that we have not had some text chars before 
				// that point to worry about
				return GetNextTag();
			}
			else
			{
				// check if it's whitespace - typically happens after tag end and before new tag starts
				// so it makes sense make it special case
				if(bCompressWhiteSpaceBeforeTag && cChar<=32 && bWhiteSpace[cChar]==1)
				{
					// ok first char is empty space, this can often lead to new tag
					// thus causing us to create essentially empty strings where as we could have
					// returned fixed single space string when it is necessary

					while(iCurPos<iDataLength)
					{
						cChar=bHTML[iCurPos++];			

						if(cChar<=32 && bWhiteSpace[cChar]==1)
							continue;

						// ok we got tag, but all we had before it was spaces, most likely end of lines
						// so we will return compact representation of that text data
						if(cChar==(byte)'<')
						{
							iCurPos--;

							oChunk.oType=HTMLchunkType.Text;
							oChunk.oHTML=" ";
							
							return oChunk;
						}

						break;
					}
				   
				}

				// ok normal text, we just scan it until tag or end of text
				// statistically this loop will have plenty of iterations
				// thus it makes sense to deal with pointers, we only do that if
				// we have got plenty of bytes to scan left
				int iQuadBytes=((iDataLength-iCurPos)>>2)-1;

				if(!oE.bDecodeEntities && !oE.bMiniEntities)
				{
					while(iCurPos<iDataLength)
					{
						// ok we got tag, but all we had before it was spaces, most likely end of lines
						// so we will return compact representation of that text data
						if(bHTML[iCurPos++]==(byte)'<')
						{
							iCurPos--;
							break;
						}
					}

				}
				else
				{
					// TODO: might help skipping data in quads but we need to perfect bitmap operations for that:
					// stop when at least one & or < is detected in quad
					/*
					fixed(byte* bpData=&bHTML[iCurPos])
					{
						uint* uiData=(uint*)bpData;

						for(int i=0; i<iQuadBytes; i++)
						{
							// use bitmask operation to quickly check if any of the 4 bytes
							// has got < in them - should be FAIRLY unlikely thus allowing us to skip
							// few bytes in one go
							if((~(*uiData &  0x3C3C3C3C)) )
							{
								iCurPos+=4;
								uiData++;
								continue;
							}

							break;
						}

					}
					 */

					// we might have entity here, which is first char of the text:
					if(cChar==(byte)'&')
					{
						int iLastCurPos=iCurPos-1;

						char cEntityChar=oE.CheckForEntity(bHTML,ref iCurPos,iDataLength);

						// restore current symbol
						if(cEntityChar!=0)
						{
							// ok, we have got entity on our hand, it means that we can't just copy
							// data from start of the buffer to end of text thereby avoiding having to
							// accumulate same data elsewhere
							sText.Clear();

							oChunk.bEntities=true;

							if(cEntityChar==(byte)'<')
								oChunk.bLtEntity=true;

							sText.Append(cEntityChar);

							return ParseTextWithEntities();
						}
					}

					while(iCurPos<iDataLength)
					{
						cChar=bHTML[iCurPos++];

						// ok we got tag, but all we had before it was spaces, most likely end of lines
						// so we will return compact representation of that text data
						if(cChar==(byte)'<')
						{
							iCurPos--;
							break;
						}

						// check if we got entity
						if(cChar==(byte)'&')
						{
							int iLastCurPos=iCurPos-1;

							char cEntityChar=oE.CheckForEntity(bHTML,ref iCurPos,iDataLength);

							// restore current symbol
							if(cEntityChar!=0)
							{
								// ok, we have got entity on our hand, it means that we can't just copy
								// data from start of the buffer to end of text thereby avoiding having to
								// accumulate same data elsewhere
								sText.Clear();

								int iLen=iLastCurPos-oChunk.iChunkOffset;

								if(iLen>0)
								{
									Array.Copy(bHTML,oChunk.iChunkOffset,sText.bBuffer,0,iLen);
									sText.iBufPos=iLen;
								}

								oChunk.bEntities=true;

								if(cEntityChar==(byte)'<')
									oChunk.bLtEntity=true;

								sText.Append(cEntityChar);

								return ParseTextWithEntities();
							}
						}
					}

				}

				oChunk.iChunkLength=iCurPos-oChunk.iChunkOffset;

				if(oChunk.iChunkLength==0)
					return null;

				oChunk.oType=HTMLchunkType.Text;
				oChunk.oHTML=oEnc.GetString(bHTML,oChunk.iChunkOffset,oChunk.iChunkLength);

				return oChunk;
			}

		}


		private HTMLchunk ParseTextWithEntities()
		{
			// okay, now that we got our first entity we will need to continue
			// parsing by copying data into temporary buffer and when finished
			// convert it to string
			while(iCurPos<iDataLength)
			{
				byte cChar=bHTML[iCurPos++];

				// ok we got tag, but all we had before it was spaces, most likely end of lines
				// so we will return compact representation of that text data
				if(cChar==(byte)'<')
				{
					iCurPos--;
					break;
				}

				// check if we got entity again
				if(cChar==(byte)'&')
				{
					char cNewEntityChar=oE.CheckForEntity(bHTML,ref iCurPos,iDataLength);

					// restore current symbol
					if(cNewEntityChar!=0)
					{
						if(cNewEntityChar==(byte)'<')
							oChunk.bLtEntity=true;

						sText.Append(cNewEntityChar);
					}
					continue;
				}

				sText.bBuffer[sText.iBufPos++]=cChar;
			}

			oChunk.iChunkLength=iCurPos-oChunk.iChunkOffset;

			oChunk.oType=HTMLchunkType.Text;
			oChunk.oHTML=sText.SetToString();

			return oChunk;
		}

		/// <summary>
		/// Internally parses tag and returns it from point when left angular bracket was found
		/// </summary>
		/// <returns>Chunk</returns>
		internal HTMLchunk GetNextTag()
		{

			oChunk=oTP.ParseTag(ref iCurPos);

			// for backwards compatibility mark closed tags with params as open
			if(oChunk.iParams>0 && bAutoMarkClosedTagsWithParamsAsOpen && oChunk.oType==HTMLchunkType.CloseTag)
				oChunk.oType=HTMLchunkType.OpenTag;

			//                    012345
			// check for start of script
			if(oChunk.sTag.Length==6 && oChunk.sTag[0]=='s' && oChunk.sTag=="script")
			{
				if(!oChunk.bClosure)
				{
					oChunk.oType=HTMLchunkType.Script;
					oChunk=oTP.ParseScript(ref iCurPos);
					return oChunk;
				}
			}

			oChunk.iChunkLength=iCurPos-oChunk.iChunkOffset;

			if(bKeepRawHTML)
				oChunk.oHTML=oEnc.GetString(bHTML,oChunk.iChunkOffset,oChunk.iChunkLength);

			return oChunk;

		}

		

		/// <summary>
		/// This function will decode any entities found in a string - not fast!
		/// </summary>
		internal static string DecodeEntities(string text)
		{
			return HTMLentities.DecodeEntities(text);
		}


	}

}

