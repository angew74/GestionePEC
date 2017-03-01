using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Com.Delta.Print.Engine.Editors
{
	/// <summary>
	/// Summary description for PlainTextEditorDialog.
	/// </summary>
	public class PlainTextEditorDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel bottomPanel;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.TextBox editBox;
		private System.Windows.Forms.Button cancelButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Creates a new instance of PlainTextEditorDialog.
		/// </summary>
		public PlainTextEditorDialog()
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

		/// <summary>
		/// Sets the font for this dialog.
		/// </summary>
		public void SetFont(Font f)
		{
			editBox.Font = f;
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(PlainTextEditorDialog));
			this.bottomPanel = new System.Windows.Forms.Panel();
			this.cancelButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.editBox = new System.Windows.Forms.TextBox();
			this.bottomPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// bottomPanel
			// 
			this.bottomPanel.Controls.Add(this.cancelButton);
			this.bottomPanel.Controls.Add(this.okButton);
			this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.bottomPanel.Location = new System.Drawing.Point(3, 241);
			this.bottomPanel.Name = "bottomPanel";
			this.bottomPanel.Size = new System.Drawing.Size(526, 32);
			this.bottomPanel.TabIndex = 1;
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(380, 4);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(68, 23);
			this.cancelButton.TabIndex = 1;
			this.cancelButton.Text = "Cancel";
			// 
			// okButton
			// 
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(456, 4);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(68, 23);
			this.okButton.TabIndex = 0;
			this.okButton.Text = "OK";
			// 
			// editBox
			// 
			this.editBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.editBox.Location = new System.Drawing.Point(3, 0);
			this.editBox.MaxLength = 0;
			this.editBox.Multiline = true;
			this.editBox.Name = "editBox";
			this.editBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.editBox.Size = new System.Drawing.Size(526, 241);
			this.editBox.TabIndex = 0;
			this.editBox.Text = "";
			this.editBox.WordWrap = false;
			// 
			// PlainTextEditorDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(532, 273);
			this.Controls.Add(this.editBox);
			this.Controls.Add(this.bottomPanel);
			this.DockPadding.Left = 3;
			this.DockPadding.Right = 3;
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PlainTextEditorDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Text Editor Dialog";
			this.bottomPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Gets/sets text for editing in this dialog.
		/// </summary>
		public string Data
		{
			get {return editBox.Text;}
			set {editBox.Text = value;}
		}

		/// <summary>
		/// Gets/sets word wrapping for editing in this dialog.
		/// </summary>
		public bool WordWrap
		{
			get {return editBox.WordWrap;}
			set {editBox.WordWrap = value;}	
		}
	}
}
