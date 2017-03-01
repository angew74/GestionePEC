using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;

namespace Com.Delta.PrintManager.Engine
{
	internal class PrinterBounds
	{
		[DllImport("gdi32.dll")] private static extern Int32 GetDeviceCaps(IntPtr hdc, System.Int32 capindex);

		private const int PHYSICALOFFSETX = 112;
		private const int PHYSICALOFFSETY = 113;

		public readonly Rectangle Bounds;
		public readonly int HardMarginLeft;
		public readonly int HardMarginTop;

		/// <summary>
	/// Creates a new instance of PrinterBounds.
		/// </summary>
		internal PrinterBounds(PrintPageEventArgs e)
		{
			try
			{
				IntPtr hDC = e.Graphics.GetHdc();

             
                HardMarginLeft = GetDeviceCaps(hDC , PHYSICALOFFSETX);
				HardMarginTop  = GetDeviceCaps(hDC , PHYSICALOFFSETY);

				e.Graphics.ReleaseHdc(hDC);

				HardMarginLeft = (int)(HardMarginLeft * 75.0 / e.Graphics.DpiX);
				HardMarginTop  = (int)(HardMarginTop  * 75.0 / e.Graphics.DpiY);

				Bounds = e.MarginBounds;
				Bounds.X = HardMarginLeft;
				Bounds.Y = HardMarginTop;
			}
			catch(Exception)
			{
				Bounds = e.MarginBounds;
				Bounds.X = 0;
				Bounds.Y = 0;
			}
		}
	}
}
