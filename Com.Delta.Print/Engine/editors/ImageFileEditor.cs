using System;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Com.Delta.Print.Engine.Editors
{
	/// <summary>
	/// Summary description for ImageFileEditor.
	/// </summary>
	public class ImageFileEditor : UITypeEditor
	{
		/// <summary>
		/// Editor for ImageFile property.
		/// </summary>
		public ImageFileEditor()
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
			//CType(provider.GetService(GetType(IWindowsFormsEditorService)), IWindowsFormsEditorService)
			if (edSvc == null)
				return null;

			//Displays a StringInputDialog Form to get a user-adjustable 
			//string value.
			
			PictureBox pictureBox = context.Instance as PictureBox;
			ImageFileEditorDialog f = new ImageFileEditorDialog(pictureBox.Section.Document.DocRoot, (string)value);
			if (edSvc.ShowDialog(f) == DialogResult.OK)
			{
				return f.ImageFile;
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
