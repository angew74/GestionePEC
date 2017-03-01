

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;


namespace Com.Delta.Print.Engine.Editors
{
	/// <summary>
	/// Summary description for FormatMaskEditorDialog.
	/// </summary>
	public class FormatMaskEditorDialog : System.Windows.Forms.Form
	{
		#region Declarations
		private string mFormatMask = "";
		private System.Windows.Forms.Label lblDataType;
		private System.Windows.Forms.ComboBox comboDataType;
		private System.Windows.Forms.ComboBox comboFormatMask;
		private System.Windows.Forms.Label lblFormatMask;
		private System.Windows.Forms.Label lblSampleData;
		private System.Windows.Forms.TextBox txtSampleData;
		private System.Windows.Forms.Label lblOutputValue;
		private System.Windows.Forms.Label lblOutputValueResult;
		private System.Windows.Forms.Button btnUseFormatMask;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnTest;
		private System.Windows.Forms.DateTimePicker DateTimePicker;
		private System.ComponentModel.Container components = null;
		#endregion

		#region Form and Object Event Handlers

		private void comboDataType_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			switch (this.comboDataType.Text)
			{
				case "Date":
					this.comboFormatMask.Text = "";
					this.comboFormatMask.Items.Clear();
					
					this.comboFormatMask.Items.Add("MM/dd/yyyy");
					this.comboFormatMask.Items.Add("MM/dd/yy");
					this.comboFormatMask.Items.Add("dd/MM/yyyy");
					this.comboFormatMask.Items.Add("dd/MM/yy");
					this.comboFormatMask.Items.Add("dd.MM.yyyy");					
					this.comboFormatMask.Items.Add("dd.MM.yy");
					this.comboFormatMask.Items.Add("dd-MM-yy");
					this.comboFormatMask.Items.Add("dd-MM-yyyy");
					this.comboFormatMask.Items.Add("dd MMM yyyy");
					this.comboFormatMask.Items.Add("dd MMMM yyyy");
					this.comboFormatMask.Items.Add("dd.MM.yyyy HH:mm");
					this.txtSampleData.SendToBack();
					this.DateTimePicker.BringToFront();
					break;
				case "Number":
					this.comboFormatMask.Text = "";
					this.comboFormatMask.Items.Clear();
					this.comboFormatMask.Items.Add("c");					
					this.comboFormatMask.Items.Add("e");
					this.comboFormatMask.Items.Add("f");
					this.comboFormatMask.Items.Add("g");
					this.comboFormatMask.Items.Add("p");
					this.comboFormatMask.Items.Add("###,#");
					this.comboFormatMask.Items.Add("###,#.00");
					this.comboFormatMask.Items.Add("###.##");
					this.comboFormatMask.Items.Add("#,###.##");
					this.txtSampleData.Text = "12345";
					this.txtSampleData.BringToFront();
					this.DateTimePicker.SendToBack();
					break;
				case "Custom":
					this.comboFormatMask.Text = "";
					this.comboFormatMask.Items.Clear();
					this.txtSampleData.Text = "";
					this.txtSampleData.BringToFront();
					this.DateTimePicker.SendToBack();
					break;
			}
		}


		private void btnUseFormatMask_Click(object sender, System.EventArgs e)
		{
			this.mFormatMask = this.comboFormatMask.Text;
			this.Close();
		}


		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}


		private void btnTest_Click(object sender, System.EventArgs e)
		{
			try
			{
				switch (this.comboDataType.Text)
				{
					case "Date":
						this.lblOutputValueResult.Text = string.Format("{0:" + this.comboFormatMask.Text + "}", this.DateTimePicker.Value);
						break;
					case "Number":
						this.lblOutputValueResult.Text = string.Format("{0:" + this.comboFormatMask.Text + "}", Convert.ToDecimal(this.txtSampleData.Text));
						break;
					default:
						this.lblOutputValueResult.Text = string.Format("{0:" + this.comboFormatMask.Text + "}", this.txtSampleData.Text);
						break;
				}
			}
			catch (Exception ex)
			{
				this.lblOutputValueResult.Text = ex.Message;
			}
		}


		#endregion

		#region Public Properties

		/// <summary>
		/// Get/Set for format mask value
		/// </summary>
		public string FormatMask
		{
			get { return this.mFormatMask; }
			set
			{
				this.mFormatMask = value;
				this.comboFormatMask.Text = value;
			}
		}


		#endregion

		#region Creator

		/// <summary>
		/// Initializes a new instance of the FormatMaskEditorDialog class.
		/// </summary>
		public FormatMaskEditorDialog()
		{
			InitializeComponent();
		}

		
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FormatMaskEditorDialog));
			this.lblDataType = new System.Windows.Forms.Label();
			this.comboDataType = new System.Windows.Forms.ComboBox();
			this.comboFormatMask = new System.Windows.Forms.ComboBox();
			this.lblFormatMask = new System.Windows.Forms.Label();
			this.lblSampleData = new System.Windows.Forms.Label();
			this.txtSampleData = new System.Windows.Forms.TextBox();
			this.lblOutputValue = new System.Windows.Forms.Label();
			this.lblOutputValueResult = new System.Windows.Forms.Label();
			this.btnUseFormatMask = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnTest = new System.Windows.Forms.Button();
			this.DateTimePicker = new System.Windows.Forms.DateTimePicker();
			this.SuspendLayout();
			// 
			// lblDataType
			// 
			this.lblDataType.Location = new System.Drawing.Point(12, 16);
			this.lblDataType.Name = "lblDataType";
			this.lblDataType.Size = new System.Drawing.Size(78, 23);
			this.lblDataType.TabIndex = 0;
			this.lblDataType.Text = "Data Type :";
			this.lblDataType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboDataType
			// 
			this.comboDataType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboDataType.Items.AddRange(new object[] {
															   "Date",
															   "Number",
															   "Custom"});
			this.comboDataType.Location = new System.Drawing.Point(98, 17);
			this.comboDataType.Name = "comboDataType";
			this.comboDataType.Size = new System.Drawing.Size(184, 21);
			this.comboDataType.TabIndex = 0;
			this.comboDataType.SelectedIndexChanged += new System.EventHandler(this.comboDataType_SelectedIndexChanged);
			// 
			// comboFormatMask
			// 
			this.comboFormatMask.Location = new System.Drawing.Point(98, 48);
			this.comboFormatMask.Name = "comboFormatMask";
			this.comboFormatMask.Size = new System.Drawing.Size(184, 21);
			this.comboFormatMask.TabIndex = 1;
			// 
			// lblFormatMask
			// 
			this.lblFormatMask.Location = new System.Drawing.Point(12, 48);
			this.lblFormatMask.Name = "lblFormatMask";
			this.lblFormatMask.Size = new System.Drawing.Size(78, 23);
			this.lblFormatMask.TabIndex = 2;
			this.lblFormatMask.Text = "Format Mask :";
			this.lblFormatMask.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblSampleData
			// 
			this.lblSampleData.Location = new System.Drawing.Point(12, 80);
			this.lblSampleData.Name = "lblSampleData";
			this.lblSampleData.Size = new System.Drawing.Size(78, 23);
			this.lblSampleData.TabIndex = 4;
			this.lblSampleData.Text = "Sample Data :";
			this.lblSampleData.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtSampleData
			// 
			this.txtSampleData.Location = new System.Drawing.Point(98, 80);
			this.txtSampleData.Name = "txtSampleData";
			this.txtSampleData.Size = new System.Drawing.Size(184, 21);
			this.txtSampleData.TabIndex = 2;
			this.txtSampleData.Text = "";
			// 
			// lblOutputValue
			// 
			this.lblOutputValue.Location = new System.Drawing.Point(12, 110);
			this.lblOutputValue.Name = "lblOutputValue";
			this.lblOutputValue.Size = new System.Drawing.Size(78, 23);
			this.lblOutputValue.TabIndex = 6;
			this.lblOutputValue.Text = "Output Value :";
			this.lblOutputValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblOutputValueResult
			// 
			this.lblOutputValueResult.BackColor = System.Drawing.SystemColors.Window;
			this.lblOutputValueResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblOutputValueResult.Location = new System.Drawing.Point(98, 112);
			this.lblOutputValueResult.Name = "lblOutputValueResult";
			this.lblOutputValueResult.Size = new System.Drawing.Size(184, 20);
			this.lblOutputValueResult.TabIndex = 7;
			this.lblOutputValueResult.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lblOutputValueResult.Click += new System.EventHandler(this.lblOutputValueResult_Click);
			// 
			// btnUseFormatMask
			// 
			this.btnUseFormatMask.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnUseFormatMask.Location = new System.Drawing.Point(84, 144);
			this.btnUseFormatMask.Name = "btnUseFormatMask";
			this.btnUseFormatMask.Size = new System.Drawing.Size(112, 23);
			this.btnUseFormatMask.TabIndex = 4;
			this.btnUseFormatMask.Text = "Use Format Mask";
			this.btnUseFormatMask.Click += new System.EventHandler(this.btnUseFormatMask_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(206, 144);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 5;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnTest
			// 
			this.btnTest.Location = new System.Drawing.Point(10, 144);
			this.btnTest.Name = "btnTest";
			this.btnTest.Size = new System.Drawing.Size(64, 23);
			this.btnTest.TabIndex = 3;
			this.btnTest.Text = "&Test";
			this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
			// 
			// DateTimePicker
			// 
			this.DateTimePicker.CustomFormat = "dd\'/\'MM\'/\'yyyy hh\':\'mm tt";
			this.DateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.DateTimePicker.Location = new System.Drawing.Point(98, 80);
			this.DateTimePicker.Name = "DateTimePicker";
			this.DateTimePicker.Size = new System.Drawing.Size(184, 21);
			this.DateTimePicker.TabIndex = 9;
			// 
			// FormatMaskEditorDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(290, 175);
			this.Controls.Add(this.btnTest);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnUseFormatMask);
			this.Controls.Add(this.lblOutputValueResult);
			this.Controls.Add(this.lblOutputValue);
			this.Controls.Add(this.lblSampleData);
			this.Controls.Add(this.comboFormatMask);
			this.Controls.Add(this.lblFormatMask);
			this.Controls.Add(this.comboDataType);
			this.Controls.Add(this.lblDataType);
			this.Controls.Add(this.txtSampleData);
			this.Controls.Add(this.DateTimePicker);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormatMaskEditorDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Format Mask Editor";
			this.ResumeLayout(false);

		}
		#endregion

		private void lblOutputValueResult_Click(object sender, System.EventArgs e)
		{
		
		}

		#endregion
	}
}