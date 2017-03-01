

using System;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;


namespace Com.Delta.Print.Engine.Editors
{
	/// <summary>
	/// FormatMaskEditor: provides editing interface for the FormatMask property
	/// </summary>
	public class FormatMaskEditor : UITypeEditor
	{
		#region Creator

		/// <summary>
		/// Initializes a new instance of the FormatMaskEditor class.
		/// </summary>
		public FormatMaskEditor()
		{
		}


		#endregion

		#region Public Overrides

		/// <summary>
		/// Edits the value of the specified object using the editor style indicated by GetEditStyle.
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext that can be used to gain additional context information.</param>
		/// <param name="provider">An IServiceProvider that this editor can use to obtain services. </param>
		/// <param name="value">The object to edit. </param>
		/// <returns>The new value of the object.</returns>
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			//return base.EditValue (context, provider, value);
			// Attempts to obtain an IWindowsFormsEditorService.
			IWindowsFormsEditorService edSvc  = ((IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService)));
				//CType(provider.GetService(GetType(IWindowsFormsEditorService)), IWindowsFormsEditorService)
			if (edSvc == null)
				return null;

			//Displays a StringInputDialog Form to get a user-adjustable 
			//string value.
			FormatMaskEditorDialog f = new FormatMaskEditorDialog();
			f.FormatMask = value.ToString();
			if (edSvc.ShowDialog(f) == DialogResult.OK)
			{
				return f.FormatMask;
			}

			//If OK was not pressed, return the original value
			return value;
		}


		/// <summary>
		/// Gets the editor style used by the EditValue method.
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext that can be used to gain additional context information. </param>
		/// <returns>UITypeEditorEditStyle.Modal indicating the editor will be a modal form </returns>
		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			//return base.GetEditStyle (context);
			return UITypeEditorEditStyle.Modal;
		}


		#endregion
	}
}
