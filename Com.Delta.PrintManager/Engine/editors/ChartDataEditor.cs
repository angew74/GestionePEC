using System;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Data;

namespace Com.Delta.PrintManager.Engine.Editors
{
	/// <summary>
	/// Summary description for StaticTableEditor.
	/// </summary>
	public class ChartDataEditor : UITypeEditor
	{
		/// <summary>
		/// Edits the value of the specified object using the editor style indicated by GetEditStyle.
		/// </summary>
		public ChartDataEditor()
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

			if (context.Instance is ChartBox)
			{
				ChartBox chartBox = context.Instance as ChartBox;
				if (chartBox.Categories.Length == 0)
				{
					MessageBox.Show("At least one category must be defined in order to set chart data.\nSet categories through Categories property.","Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return value;
				}
			}

			ChartDataEditorDialog f = new ChartDataEditorDialog();
			f.Data = value;
			if (edSvc.ShowDialog(f) == DialogResult.OK)
			{
				return (object)f.Data;
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

	}
}
