using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

namespace Com.Delta.PrintManager.Engine.Editors
{
	/// <summary>
	/// Summary description for ImageFileEditorDialog.
	/// </summary>
	public class ImageFileEditorDialog : System.Windows.Forms.Form
	{
		private string docRoot = String.Empty;
		private string imageFile = String.Empty;

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox docRootField;
		private System.Windows.Forms.TextBox imageFileField;
		private System.Windows.Forms.Button browseButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Creates a new instance of ImageFileEditorDialog internaly.
		/// </summary>
		private ImageFileEditorDialog()
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
		/// Creates a new instance of ImageFileEditorDialog.
		/// </summary>
		public ImageFileEditorDialog(string docRoot, string imageFile):this()
		{
			this.docRoot = docRoot;
			this.imageFile = imageFile;

			
			imageFileField.Text = this.imageFile;

			if (this.docRoot==null || this.docRoot==String.Empty)
			{
				docRootField.Text = "DocRoot not set. You may want to set this property first.";				
			}
			else
			{
				docRootField.Text = this.docRoot;
			}

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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ImageFileEditorDialog));
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.docRootField = new System.Windows.Forms.TextBox();
			this.imageFileField = new System.Windows.Forms.TextBox();
			this.browseButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(88, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "Document root :";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(88, 23);
			this.label2.TabIndex = 1;
			this.label2.Text = "Image file :";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// docRootField
			// 
			this.docRootField.Location = new System.Drawing.Point(96, 20);
			this.docRootField.Name = "docRootField";
			this.docRootField.ReadOnly = true;
			this.docRootField.Size = new System.Drawing.Size(248, 21);
			this.docRootField.TabIndex = 2;
			this.docRootField.TabStop = false;
			this.docRootField.Text = "";
			// 
			// imageFileField
			// 
			this.imageFileField.Location = new System.Drawing.Point(96, 48);
			this.imageFileField.Name = "imageFileField";
			this.imageFileField.Size = new System.Drawing.Size(248, 21);
			this.imageFileField.TabIndex = 0;
			this.imageFileField.Text = "";
			// 
			// browseButton
			// 
			this.browseButton.Location = new System.Drawing.Point(352, 48);
			this.browseButton.Name = "browseButton";
			this.browseButton.TabIndex = 1;
			this.browseButton.Text = "Browse ...";
			this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
			// 
			// okButton
			// 
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(352, 100);
			this.okButton.Name = "okButton";
			this.okButton.TabIndex = 2;
			this.okButton.Text = "OK";
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(268, 100);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.TabIndex = 3;
			this.cancelButton.Text = "Cancel";
			// 
			// ImageFileEditorDialog
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(436, 129);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.browseButton);
			this.Controls.Add(this.imageFileField);
			this.Controls.Add(this.docRootField);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ImageFileEditorDialog";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Select image file";
			this.ResumeLayout(false);

		}
		#endregion

		private void browseButton_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "JPG images (*.jpg)|*.jpg|GIF images (*.gif)|*.gif|PNG images (*.png)|*.png|BMP images (*.bmp)|*.bmp|All files (*.*)|*.*";
			ofd.InitialDirectory = this.docRoot;
			if (ofd.ShowDialog(this) == DialogResult.OK)
			{
				string relativePath = GetRelativePath(new DirectoryInfo(docRoot), new DirectoryInfo(Path.GetDirectoryName(ofd.FileName)));
				imageFileField.Text = relativePath + (relativePath==""?"":Path.DirectorySeparatorChar.ToString()) + Path.GetFileName(ofd.FileName);
			}
		}

		private string GetRelativePath(DirectoryInfo dir1, DirectoryInfo dir2)
		{
			string relativePath = "";

			DirectoryInfo tmp = dir2;			
			do
			{
				if (tmp.FullName.Equals(dir1.FullName))
				{
					return relativePath;
				}
				else
				{
					relativePath = tmp.Name + (relativePath=="" ? "" : Path.DirectorySeparatorChar + relativePath );
				}
			}
			while ( (tmp=tmp.Parent)!= null);

			return relativePath;
			
		}

		/// <summary>
		/// Gets the selected image file relative path.
		/// </summary>
		public string ImageFile
		{
			get {return imageFileField.Text;}
		}
	}
}
