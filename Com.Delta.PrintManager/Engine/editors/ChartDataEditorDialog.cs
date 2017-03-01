using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace Com.Delta.PrintManager.Engine.Editors
{
	/// <summary>
	/// Summary description for ChartDataEditorDialog.
	/// </summary>
	public class ChartDataEditorDialog : System.Windows.Forms.Form
	{
		private DataTable theTable;
		private System.Windows.Forms.DataGrid dataGrid;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button addButton;
		private System.Windows.Forms.Button removeButton;
		private System.Windows.Forms.Button colorButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Creates new instance of this form.
		/// </summary>
		public ChartDataEditorDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ChartDataEditorDialog));
			this.dataGrid = new System.Windows.Forms.DataGrid();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.addButton = new System.Windows.Forms.Button();
			this.removeButton = new System.Windows.Forms.Button();
			this.colorButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.dataGrid)).BeginInit();
			this.SuspendLayout();
			// 
			// dataGrid
			// 
			this.dataGrid.AllowSorting = false;
			this.dataGrid.CaptionVisible = false;
			this.dataGrid.DataMember = "";
			this.dataGrid.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.dataGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dataGrid.Location = new System.Drawing.Point(8, 36);
			this.dataGrid.Name = "dataGrid";
			this.dataGrid.PreferredColumnWidth = 100;
			this.dataGrid.RowHeaderWidth = 20;
			this.dataGrid.Size = new System.Drawing.Size(524, 256);
			this.dataGrid.TabIndex = 0;
			// 
			// okButton
			// 
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(456, 300);
			this.okButton.Name = "okButton";
			this.okButton.TabIndex = 1;
			this.okButton.Text = "OK";
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(376, 300);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.TabIndex = 2;
			this.cancelButton.Text = "Cancel";
			// 
			// addButton
			// 
			this.addButton.Location = new System.Drawing.Point(8, 8);
			this.addButton.Name = "addButton";
			this.addButton.Size = new System.Drawing.Size(84, 23);
			this.addButton.TabIndex = 3;
			this.addButton.Text = "Add serie";
			this.addButton.Click += new System.EventHandler(this.addButton_Click);
			// 
			// removeButton
			// 
			this.removeButton.Location = new System.Drawing.Point(96, 8);
			this.removeButton.Name = "removeButton";
			this.removeButton.Size = new System.Drawing.Size(84, 23);
			this.removeButton.TabIndex = 4;
			this.removeButton.Text = "Remove serie";
			this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
			// 
			// colorButton
			// 
			this.colorButton.Location = new System.Drawing.Point(204, 8);
			this.colorButton.Name = "colorButton";
			this.colorButton.Size = new System.Drawing.Size(84, 23);
			this.colorButton.TabIndex = 5;
			this.colorButton.Text = "Serie color";
			this.colorButton.Click += new System.EventHandler(this.colorButton_Click);
			// 
			// ChartDataEditorDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(538, 329);
			this.Controls.Add(this.colorButton);
			this.Controls.Add(this.removeButton);
			this.Controls.Add(this.addButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.dataGrid);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ChartDataEditorDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Chart Data Editor";
			((System.ComponentModel.ISupportInitialize)(this.dataGrid)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void addButton_Click(object sender, System.EventArgs e)
		{
			if (theTable!=null)
			{
				object[] row = new object[theTable.Columns.Count];
				row[0] = String.Format("Serie {0}", theTable.Rows.Count+1);
				row[1] = Color.Red;

				for (int i=2;i<row.Length;i++)
					row[i] = 0;
				theTable.Rows.Add(row);
			}
		}

		private void removeButton_Click(object sender, System.EventArgs e)
		{
			if (theTable!=null)
			{
				try
				{
					theTable.Rows.RemoveAt(dataGrid.CurrentCell.RowNumber);
				}
				catch(Exception){}
			}
		}

		private void colorButton_Click(object sender, System.EventArgs e)
		{
			if (theTable!=null)
			{
				try
				{
					ColorDialog cd = new ColorDialog();
					object colorObject = theTable.Rows[dataGrid.CurrentCell.RowNumber][1];
					if (colorObject != null)
						cd.Color = (Color)colorObject;

					cd.SolidColorOnly = true;
					if (DialogResult.OK == cd.ShowDialog(this))
					{
						theTable.Rows[dataGrid.CurrentCell.RowNumber][1] = cd.Color;	
					}

				}
				catch(Exception){}
			}
		}

		/// <summary>
		/// Gets/Sets the table data for this edit dialog.
		/// </summary>
		public object Data
		{
			set
			{
				theTable = (DataTable)value;
				dataGrid.DataSource = theTable;			
			}

			get
			{
				return theTable.Copy();
			}
		}
	}
}
