using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Com.Delta.PrintManager.Engine.Editors
{
	/// <summary>
	/// Summary description for MapDataEditorDialog.
	/// </summary>
	public class MapDataEditorDialog : System.Windows.Forms.Form
	{
		private Map map = null;
		private System.Windows.Forms.Panel mapPanel;
		private System.Windows.Forms.ListView landmarksList;
		private System.Windows.Forms.PropertyGrid propertyGrid;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button addButton;
		private System.Windows.Forms.Button removeButton;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.ToolTip toolTip;
		private System.ComponentModel.IContainer components;

		private MapDataEditorDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		public MapDataEditorDialog(Map map):this()
		{
			this.map = map;

			FillList();
			
			if (landmarksList.Items.Count>0)
				landmarksList.Items[0].Selected = true;
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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MapDataEditorDialog));
			this.mapPanel = new System.Windows.Forms.Panel();
			this.landmarksList = new System.Windows.Forms.ListView();
			this.propertyGrid = new System.Windows.Forms.PropertyGrid();
			this.okButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.addButton = new System.Windows.Forms.Button();
			this.removeButton = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.cancelButton = new System.Windows.Forms.Button();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.SuspendLayout();
			// 
			// mapPanel
			// 
			this.mapPanel.BackColor = System.Drawing.Color.White;
			this.mapPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.mapPanel.Cursor = System.Windows.Forms.Cursors.Cross;
			this.mapPanel.Location = new System.Drawing.Point(8, 28);
			this.mapPanel.Name = "mapPanel";
			this.mapPanel.Size = new System.Drawing.Size(288, 288);
			this.mapPanel.TabIndex = 0;
			this.toolTip.SetToolTip(this.mapPanel, "Click in Preview area to add new landmark.");
			this.mapPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.mapPanel_Paint);
			this.mapPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mapPanel_MouseDown);
			// 
			// landmarksList
			// 
			this.landmarksList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.landmarksList.HideSelection = false;
			this.landmarksList.Location = new System.Drawing.Point(308, 28);
			this.landmarksList.MultiSelect = false;
			this.landmarksList.Name = "landmarksList";
			this.landmarksList.Size = new System.Drawing.Size(152, 252);
			this.landmarksList.TabIndex = 1;
			this.landmarksList.View = System.Windows.Forms.View.List;
			this.landmarksList.SelectedIndexChanged += new System.EventHandler(this.landmarksList_SelectedIndexChanged);
			// 
			// propertyGrid
			// 
			this.propertyGrid.CommandsVisibleIfAvailable = true;
			this.propertyGrid.LargeButtons = false;
			this.propertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.propertyGrid.Location = new System.Drawing.Point(476, 28);
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
			this.propertyGrid.Size = new System.Drawing.Size(180, 288);
			this.propertyGrid.TabIndex = 2;
			this.propertyGrid.Text = "propertyGrid1";
			this.propertyGrid.ToolbarVisible = false;
			this.propertyGrid.ViewBackColor = System.Drawing.SystemColors.Window;
			this.propertyGrid.ViewForeColor = System.Drawing.SystemColors.WindowText;
			this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
			// 
			// okButton
			// 
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(588, 328);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(64, 23);
			this.okButton.TabIndex = 3;
			this.okButton.Text = "OK";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(308, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 16);
			this.label1.TabIndex = 4;
			this.label1.Text = "Landmarks :";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(476, 8);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 16);
			this.label2.TabIndex = 5;
			this.label2.Text = "Properties :";
			// 
			// addButton
			// 
			this.addButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.addButton.Location = new System.Drawing.Point(308, 292);
			this.addButton.Name = "addButton";
			this.addButton.Size = new System.Drawing.Size(72, 24);
			this.addButton.TabIndex = 6;
			this.addButton.Text = "Add";
			this.addButton.Click += new System.EventHandler(this.button1_Click);
			// 
			// removeButton
			// 
			this.removeButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.removeButton.Location = new System.Drawing.Point(392, 292);
			this.removeButton.Name = "removeButton";
			this.removeButton.Size = new System.Drawing.Size(68, 24);
			this.removeButton.TabIndex = 7;
			this.removeButton.Text = "Remove";
			this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 8);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 16);
			this.label3.TabIndex = 8;
			this.label3.Text = "Preview :";
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(516, 328);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(64, 23);
			this.cancelButton.TabIndex = 9;
			this.cancelButton.Text = "Cancel";
			// 
			// MapDataEditorDialog
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(666, 357);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.removeButton);
			this.Controls.Add(this.addButton);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.propertyGrid);
			this.Controls.Add(this.landmarksList);
			this.Controls.Add(this.mapPanel);
			this.Controls.Add(this.label3);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MapDataEditorDialog";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Map Landmarks Editor";
			this.toolTip.SetToolTip(this, "Click in Preview area to add new landmark.");
			this.Load += new System.EventHandler(this.MapDataEditorDialog_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void FillList()
		{
			landmarksList.Clear();
			foreach(Map.MapPoint point in map.Landmarks)
			{
				ListViewItem lvi = new ListViewItem(point.Text);
				lvi.Tag = point;
				landmarksList.Items.Add(lvi);
			}
		}

		private void UpdateList(Com.Delta.PrintManager.Engine.Map.MapPoint point)
		{
			foreach(ListViewItem lvi in landmarksList.Items)
			{
				if (lvi.Tag == point)
				{
					lvi.Text = point.Text;
					break;
				}
			}
		}

		private void landmarksList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (landmarksList.SelectedIndices.Count > 0)
			{
				int index = landmarksList.SelectedIndices[0];
				propertyGrid.SelectedObject = landmarksList.Items[index].Tag;
			}
			else
			{
				propertyGrid.SelectedObject = null;
			}
		}

		private void mapPanel_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			if (map != null)
			{
				map.PaintForEdit(e.Graphics, mapPanel.Width);
			}

		}

		private void MapDataEditorDialog_Load(object sender, System.EventArgs e)
		{
			mapPanel.Refresh();
			
		}

		private void mapPanel_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			AddPoint(e.X, e.Y);
		}

		private void AddPoint(int x, int y)
		{
			float xCo = x*1000f/mapPanel.Width;
			float yCo = y*1000f/mapPanel.Height;

			int index = map.Landmarks.Length;

			Map.MapPoint point = new Com.Delta.PrintManager.Engine.Map.MapPoint(String.Format("Point{0}", index), xCo, yCo);

			ArrayList tmp = new ArrayList(map.Landmarks);
			tmp.Add(point);

			map.Landmarks = (Com.Delta.PrintManager.Engine.Map.MapPoint[])tmp.ToArray(typeof(Com.Delta.PrintManager.Engine.Map.MapPoint));

			FillList();
			landmarksList.Items[index].Selected = true;
			mapPanel.Refresh();
		}

		private void propertyGrid_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
		{
			mapPanel.Refresh();

			if (e.ChangedItem.PropertyDescriptor.Name == "Text")
			{
				UpdateList((Com.Delta.PrintManager.Engine.Map.MapPoint)propertyGrid.SelectedObject);
			}
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			AddPoint(mapPanel.Width/2, mapPanel.Height/2);
		}

		private void removeButton_Click(object sender, System.EventArgs e)
		{
			if (landmarksList.SelectedIndices.Count > 0)
			{
				Map.MapPoint dropPoint = landmarksList.SelectedItems[0].Tag as Map.MapPoint;
				
				
				ArrayList tmp = new ArrayList(map.Landmarks);
				int index = tmp.IndexOf(dropPoint);
				tmp.Remove(dropPoint);

				map.Landmarks = (Com.Delta.PrintManager.Engine.Map.MapPoint[])tmp.ToArray(typeof(Com.Delta.PrintManager.Engine.Map.MapPoint));

				FillList();
				if (landmarksList.Items.Count>0)
				{
					if (index>1)
						landmarksList.Items[index-1].Selected = true;
					else
						landmarksList.Items[0].Selected = true;
				}
				else
				{
					propertyGrid.SelectedObject = null;
				}
				mapPanel.Refresh();
			}
		}

		public Map.MapPoint[] Landmarks
		{
			get {return map.Landmarks;}
		}
	}
}
