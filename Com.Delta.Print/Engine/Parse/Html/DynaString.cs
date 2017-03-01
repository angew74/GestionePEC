using System;
using System.Text;
using System.Collections;

namespace Com.Delta.Print.Engine.Parse.Html
{
	/// <summary>
	/// Class for fast dynamic string building - it is faster than StringBuilder
	/// </summary>
	///<exclude/>
	internal class DynaString	: IDisposable
	{
		/// <summary>
		/// Finalised text will be available in this string
		/// </summary>
		public string sText;

		
		public static int TEXT_CAPACITY=1024*256-1;

		public byte[] bBuffer;
		public int iBufPos;
		private int iLength;

		Encoding oEnc=Encoding.Default;

		private bool bDisposed=false;

		/// <summary>
		/// Constructor 
		/// </summary>
		/// <param name="sString">Initial string</param>
		internal DynaString(string sString)
		{
			sText=sString;
			iBufPos=0;
			bBuffer=new byte[TEXT_CAPACITY+1];
			iLength=sString.Length;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool bDisposing)
		{
			if(!bDisposed)
			{
				bBuffer=null;
			}

			bDisposed=true;
		}

		/// <summary>
		/// Resets object to zero length string
		/// </summary>
		public void Clear()
		{
			sText="";
			iLength=0;
			iBufPos=0;
		}

		/// <summary>
		/// Sets encoding to be used for conversion of binary data into string
		/// </summary>
		/// <param name="p_oEnc">Encoding object</param>
		public void SetEncoding(Encoding p_oEnc)
		{
			oEnc=p_oEnc;
		}

		
		/// <summary>
		/// Appends proper char with smart handling of Unicode chars
		/// </summary>
		/// <param name="cChar">Char to append</param>
		public void Append(char cChar)
		{
			if(cChar<=127)
				bBuffer[iBufPos++]=(byte)cChar;
			else
			{
				// unicode character - this is really bad way of doing it, but 
				// it seems to be called almost never
				byte[] bBytes=oEnc.GetBytes(cChar.ToString());

				
				// the problem is that some unicode chars might not be mapped to bytes by specified encoding
				// in the HTML itself, this means we will get single byte ? - this will look like failed conversion
				// Not good situation that we need to deal with :(
				if(bBytes.Length==1 && bBytes[0]==(char)'?')
				{
					// TODO: 

					for(int i=0; i<bBytes.Length; i++)
						bBuffer[iBufPos++]=bBytes[i];
				}
				else
				{
					for(int i=0; i<bBytes.Length; i++)
						bBuffer[iBufPos++]=bBytes[i];
				}
			}
		}
		
		/// <summary>
		/// Creates string from buffer using set encoder
		/// </summary>
		internal string SetToString()
		{
			if(iBufPos>0)
			{
				if(sText.Length==0)
				{
					sText=oEnc.GetString(bBuffer,0,iBufPos);
				}
				else
				{
					sText+=oEnc.GetString(bBuffer,0,iBufPos);
				}

				iLength+=iBufPos;
				iBufPos=0;
			}

			return sText;
		}

		/// <summary>
		/// Creates string from buffer using default encoder
		/// </summary>
		internal string SetToStringASCII()
		{
			if(iBufPos>0)
			{
				if(sText.Length==0)
				{
					sText=Encoding.Default.GetString(bBuffer,0,iBufPos);
				}
				else
					sText+=Encoding.Default.GetString(bBuffer,0,iBufPos);

				iLength+=iBufPos;
				iBufPos=0;
			}

			return sText;
		}

	}
}
