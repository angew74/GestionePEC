using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;

namespace Com.Delta.PrintManager.Engine.Editors
{
	/// <summary>
	/// Summary description for DateEditorDialog.
	/// </summary>
	public class DateEditorDialog : System.Windows.Forms.Form
	{
		private DateTimeFormatInfo usFormat = new CultureInfo("en-US", false).DateTimeFormat;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.MonthCalendar datePicker;
		private System.Windows.Forms.TextBox dateField;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button cancelButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DateEditorDialog()
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(DateEditorDialog));
			this.okButton = new System.Windows.Forms.Button();
			this.datePicker = new System.Windows.Forms.MonthCalendar();
			this.dateField = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.cancelButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// okButton
			// 
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(140, 228);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(68, 23);
			this.okButton.TabIndex = 1;
			this.okButton.Text = "OK";
			// 
			// datePicker
			// 
			this.datePicker.Location = new System.Drawing.Point(8, 8);
			this.datePicker.MaxSelectionCount = 1;
			this.datePicker.Name = "datePicker";
			this.datePicker.ShowToday = false;
			this.datePicker.ShowTodayCircle = false;
			this.datePicker.TabIndex = 2;
			this.datePicker.DateChanged += new System.Windows.Forms.DateRangeEventHandler(this.datePicker_DateChanged);
			// 
			// dateField
			// 
			this.dateField.Location = new System.Drawing.Point(8, 196);
			this.dateField.Name = "dateField";
			this.dateField.Size = new System.Drawing.Size(200, 21);
			this.dateField.TabIndex = 3;
			this.dateField.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 180);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 16);
			this.label1.TabIndex = 4;
			this.label1.Text = "Selected date :";
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.cancelButton.Location = new System.Drawing.Point(68, 228);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(68, 23);
			this.cancelButton.TabIndex = 5;
			this.cancelButton.Text = "Cancel";
			// 
			// DateEditorDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(214, 259);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.dateField);
			this.Controls.Add(this.datePicker);
			this.Controls.Add(this.okButton);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DateEditorDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Date Editor";
			this.ResumeLayout(false);

		}
		#endregion

		private void datePicker_DateChanged(object sender, System.Windows.Forms.DateRangeEventArgs e)
		{
			dateField.Text = datePicker.SelectionStart.ToString(usFormat);
		}


		/// <summary>
		/// Gets/sets the date for editing in this dialog.
		/// </summary>
		public DateTime Date
		{
			get 
			{
				return Convert.ToDateTime(dateField.Text, usFormat);
			}
			set 
			{
				try
				{
					datePicker.SelectionStart = value;
				}
				catch (Exception)
				{
					dateField.Text = value.ToString(usFormat);
					datePicker.Enabled = false;
					datePicker.TitleBackColor = SystemColors.InactiveCaption;
				}
			}
		}
	}
}
