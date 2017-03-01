using System;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Data;
using System.Drawing;

namespace Com.Delta.Print.Engine.Editors
{
	/// <summary>
	/// Summary description for StaticTableEditor.
	/// </summary>
	public class PlainTextEditor : UITypeEditor
	{
		/// <summary>
		/// Edits the value of the specified object using the editor style indicated by GetEditStyle.
		/// </summary>
		public PlainTextEditor()
		{
		}

		
		/// <summary>
		/// Edits the value of the specified object using the editor style indicated by GetEditStyle.
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext that can be used to gain additional context information.</param>
		/// <param name="provider">An IServiceProvider that this editor can use to obtain services. </param>
		/// <param name="value">The object to edit. </param>
		/// <returns>The new value of the object.</returns>
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{

			IWindowsFormsEditorService edSvc  = ((IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService)));
		
			if (edSvc == null)
				return null;
			

			PlainTextEditorDialog f = new PlainTextEditorDialog();

			string editText = value as string;

			if (context.Instance is TextField)
			{
				TextField textField = context.Instance as TextField;
				editText = textField.GetText();
				f.SetFont(textField.Font);
			}

			if (context.Instance is RichTextField)
			{
				f.WordWrap = true;
			}

			f.Data = editText;
			if (edSvc.ShowDialog(f) == DialogResult.OK)
			{
				return f.Data;
			}
						
			return value;
		}


		/// <summary>
		/// Gets the editor style used by the EditValue method.
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext that can be used to gain additional context information. </param>
		/// <returns>UITypeEditorEditStyle.Modal indicating the editor will be a modal form </returns>
		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;			
		}



	}
}
