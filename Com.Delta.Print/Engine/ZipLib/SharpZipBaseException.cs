using System;

namespace Com.Delta.Print.Engine.ZipLib
{
	/// <summary>
	/// SharpZipBaseException is the base exception class for the SharpZipLibrary.
	/// All library exceptions are derived from this.
	/// </summary>
	internal class SharpZipBaseException : ApplicationException
	{
		/// <summary>
		/// Initializes a new instance of the SharpZipLibraryException class.
		/// </summary>
		public SharpZipBaseException()
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the SharpZipLibraryException class with a specified error message.
		/// </summary>
		public SharpZipBaseException(string msg) : base(msg)
		{
		}

		/// <summary>
		/// Initializes a new instance of the SharpZipLibraryException class with a specified
		/// error message and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="message">Error message string</param>
		/// <param name="innerException">The inner exception</param>
		public SharpZipBaseException(string message, Exception innerException)	: base(message, innerException)
		{
		}
	}
}
