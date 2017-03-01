using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace Com.Delta.PrintManager.Engine.Editors
{
	/// <summary>
	/// Summary description for StaticTableEditorDialog.
	/// </summary>
	public class StaticTableEditorDialog : System.Windows.Forms.Form
	{
		private DataTable theTable;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.DataGrid dataGrid;
		private System.Windows.Forms.Panel bottomPanel;
		private System.Windows.Forms.Button button1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Creates new instance of StaticTableEditorDialog.
		/// </summary>
		public StaticTableEditorDialog()
		{
			InitializeComponent();
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(StaticTableEditorDialog));
			this.okButton = new System.Windows.Forms.Button();
			this.dataGrid = new System.Windows.Forms.DataGrid();
			this.bottomPanel = new System.Windows.Forms.Panel();
			this.button1 = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.dataGrid)).BeginInit();
			this.bottomPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// okButton
			// 
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(448, 4);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(72, 23);
			this.okButton.TabIndex = 0;
			this.okButton.Text = "OK";
			// 
			// dataGrid
			// 
			this.dataGrid.CaptionVisible = false;
			this.dataGrid.DataMember = "";
			this.dataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGrid.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.dataGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dataGrid.Location = new System.Drawing.Point(3, 0);
			this.dataGrid.Name = "dataGrid";
			this.dataGrid.PreferredColumnWidth = 100;
			this.dataGrid.RowHeaderWidth = 20;
			this.dataGrid.Size = new System.Drawing.Size(522, 245);
			this.dataGrid.TabIndex = 1;
			// 
			// bottomPanel
			// 
			this.bottomPanel.Controls.Add(this.button1);
			this.bottomPanel.Controls.Add(this.okButton);
			this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.bottomPanel.Location = new System.Drawing.Point(3, 245);
			this.bottomPanel.Name = "bottomPanel";
			this.bottomPanel.Size = new System.Drawing.Size(522, 32);
			this.bottomPanel.TabIndex = 2;
			// 
			// button1
			// 
			this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button1.Location = new System.Drawing.Point(368, 4);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(72, 23);
			this.button1.TabIndex = 1;
			this.button1.Text = "Cancel";
			// 
			// StaticTableEditorDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(528, 277);
			this.Controls.Add(this.dataGrid);
			this.Controls.Add(this.bottomPanel);
			this.DockPadding.Left = 3;
			this.DockPadding.Right = 3;
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "StaticTableEditorDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Static Table Data Editor";
			((System.ComponentModel.ISupportInitialize)(this.dataGrid)).EndInit();
			this.bottomPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion


		/// <summary>
		/// Gets/sets two-dimensioanl string array for edit in this dialog.
		/// </summary>
		public string[][] Data
		{
			set
			{
				theTable = new DataTable();
				for (int i=0;i<value[0].Length;i++)
				{
					theTable.Columns.Add(new DataColumn(value[0][i]));
				}

				for (int i=1;i<value.Length;i++)
					theTable.Rows.Add(value[i]);

				dataGrid.DataSource = theTable;			
			}

			get
			{
				int colNumber = theTable.Columns.Count;
				int rowNumber = theTable.Rows.Count;

				string[][] tmpData = new string[rowNumber][];
				for (int i=0;i<rowNumber;i++)
				{
					tmpData[i] = new string[colNumber];
				}

				for (int i=0;i<rowNumber;i++)
				{
					for (int j=0;j<colNumber;j++)
					{
						tmpData[i][j] = theTable.Rows[i][j].ToString();
					}
				}

				return tmpData;
			}
		}
	}
}
