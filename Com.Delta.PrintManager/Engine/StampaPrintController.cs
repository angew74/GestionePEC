using System;

namespace Com.Delta.PrintManager.Engine
{
	/// <summary>
	/// Summary description for DaReportPrintController.
	/// </summary>
	internal class StampaPrintController  : System.Windows.Forms.PrintControllerWithStatusDialog
	{

		/// <summary>
		/// Object encapsulating StandardPrintController
		/// </summary>		
		internal StampaPrintController() : base(new System.Drawing.Printing.StandardPrintController(), "Stampa Report System Printing")
		{
			
		}
	}
}
