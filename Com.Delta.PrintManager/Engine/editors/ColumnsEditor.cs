using System;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;

namespace Com.Delta.PrintManager.Engine.Editors
{
	/// <summary>
	/// Summary description for StaticTableEditor.
	/// </summary>
	public class ColumnsEditor : ArrayEditor
	{
		private StyledTable table = null;
		private StyledTableColumn[] columns = null;

		public ColumnsEditor(Type type):base(type)
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
			

			if (context.Instance is StyledTable)
			{				
				StyledTable styledTable = context.Instance as StyledTable;
				table = styledTable;
				columns = styledTable.Columns;
				//this.SetItems(styledTable.Columns[0], styledTable.Columns);

				CollectionForm collectionForm = this.CreateCollectionForm();
				collectionForm.EditValue = styledTable.Columns;
				collectionForm.Text = "Columns Editor";

				//if (edSvc.ShowDialog(collectionForm) == DialogResult.OK)
				//{
					//return f..Data;
					
				//	return collectionForm.EditValue;
				//}

				//Console.WriteLine(edSvc.ShowDialog(collectionForm));
				base.EditValue(context, provider, value);

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

		protected override object CreateInstance(Type itemType)
		{
			StyledTableColumn column = new StyledTableColumn();
			if (table!=null)
			{				
				column.Name = String.Format("Column{0}", table.Columns.Length+1);
				column.Label = column.Name;
			}
			return column;						
		}

		protected override Type CreateCollectionItemType()
		{
			return typeof(Com.Delta.PrintManager.Engine.StyledTableColumn);
		}

		protected override object[] GetItems(object editValue)
		{
			return columns;
		}
		
		protected override object SetItems(object editValue, object[] value)
		{	
			columns = (StyledTableColumn[])value;

			return editValue;
		}

		protected override bool CanSelectMultipleInstances()
		{
			return false;
		}


	}
}
