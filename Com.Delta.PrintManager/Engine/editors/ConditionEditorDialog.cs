using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Com.Delta.PrintManager.Engine.Editors
{
	/// <summary>
	/// Summary description for ConditionEditorDialog.
	/// </summary>
	public class ConditionEditorDialog : System.Windows.Forms.Form
	{
		

		private System.Windows.Forms.TextBox conditionField;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.GroupBox fieldsGroup;
		private System.Windows.Forms.ListBox fieldsBox;
		private System.Windows.Forms.Button addFieldButton;
		private System.Windows.Forms.GroupBox operatorsGroup;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.Button button5;
		private System.Windows.Forms.Button button6;
		private System.Windows.Forms.Button button7;
		private System.Windows.Forms.Button button8;
		private System.Windows.Forms.Button button9;
		private System.Windows.Forms.Button button10;
		private System.Windows.Forms.Button button11;
		private System.Windows.Forms.Button button12;
		private System.Windows.Forms.Button button13;
		private System.Windows.Forms.Button button14;
		private System.Windows.Forms.Button button15;
		private System.Windows.Forms.Button button16;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button button17;
		private System.Windows.Forms.Button button18;
		private System.Windows.Forms.Button button19;
		private System.Windows.Forms.Button button20;
		private System.Windows.Forms.Button button21;
		private System.Windows.Forms.Button button22;
		private System.Windows.Forms.Button button23;
		private System.Windows.Forms.Button button24;
		private System.Windows.Forms.Button button25;
		private System.Windows.Forms.Button button26;
		private System.Windows.Forms.Button button27;
		private System.Windows.Forms.Button button28;
		private System.Windows.Forms.Button button29;
		private System.Windows.Forms.GroupBox conversionsGroup;
		private System.Windows.Forms.Button button30;
		private System.Windows.Forms.Button button31;
		private System.Windows.Forms.Button button32;
		private System.Windows.Forms.Button button33;
		private System.Windows.Forms.Button button34;
		private System.Windows.Forms.ToolTip toolTip;
		private System.ComponentModel.IContainer components;

		public ConditionEditorDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		public ConditionEditorDialog(StyledTable table):this()
		{
			for (int i=0;i<table.Columns.Length;i++)
			{
				fieldsBox.Items.Add(table.Columns[i].Name);
			}

			addFieldButton.Enabled = fieldsBox.Items.Count>0 ;

			this.toolTip.SetToolTip(this.conditionField, "Tip on data formats:\r\nString format : 'text'  \r\nDate format : #MM/dd/yyyy#  \r\nColumn name format : [columnName]  ");


			
			
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ConditionEditorDialog));
			this.conditionField = new System.Windows.Forms.TextBox();
			this.okButton = new System.Windows.Forms.Button();
			this.fieldsGroup = new System.Windows.Forms.GroupBox();
			this.fieldsBox = new System.Windows.Forms.ListBox();
			this.addFieldButton = new System.Windows.Forms.Button();
			this.operatorsGroup = new System.Windows.Forms.GroupBox();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.button4 = new System.Windows.Forms.Button();
			this.button5 = new System.Windows.Forms.Button();
			this.button6 = new System.Windows.Forms.Button();
			this.button7 = new System.Windows.Forms.Button();
			this.button8 = new System.Windows.Forms.Button();
			this.button9 = new System.Windows.Forms.Button();
			this.button10 = new System.Windows.Forms.Button();
			this.button11 = new System.Windows.Forms.Button();
			this.button12 = new System.Windows.Forms.Button();
			this.button13 = new System.Windows.Forms.Button();
			this.button14 = new System.Windows.Forms.Button();
			this.button15 = new System.Windows.Forms.Button();
			this.button16 = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.button17 = new System.Windows.Forms.Button();
			this.button18 = new System.Windows.Forms.Button();
			this.button19 = new System.Windows.Forms.Button();
			this.button20 = new System.Windows.Forms.Button();
			this.button21 = new System.Windows.Forms.Button();
			this.button22 = new System.Windows.Forms.Button();
			this.button23 = new System.Windows.Forms.Button();
			this.button24 = new System.Windows.Forms.Button();
			this.button25 = new System.Windows.Forms.Button();
			this.button26 = new System.Windows.Forms.Button();
			this.button27 = new System.Windows.Forms.Button();
			this.button28 = new System.Windows.Forms.Button();
			this.button29 = new System.Windows.Forms.Button();
			this.conversionsGroup = new System.Windows.Forms.GroupBox();
			this.button30 = new System.Windows.Forms.Button();
			this.button31 = new System.Windows.Forms.Button();
			this.button32 = new System.Windows.Forms.Button();
			this.button33 = new System.Windows.Forms.Button();
			this.button34 = new System.Windows.Forms.Button();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.fieldsGroup.SuspendLayout();
			this.operatorsGroup.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.conversionsGroup.SuspendLayout();
			this.SuspendLayout();
			// 
			// conditionField
			// 
			this.conditionField.HideSelection = false;
			this.conditionField.Location = new System.Drawing.Point(8, 8);
			this.conditionField.Multiline = true;
			this.conditionField.Name = "conditionField";
			this.conditionField.Size = new System.Drawing.Size(544, 80);
			this.conditionField.TabIndex = 0;
			this.conditionField.Text = "";
			this.toolTip.SetToolTip(this.conditionField, "Tip : string format : \'string\'\\r\\ndate format : #MM/dd/yyyy#");
			// 
			// okButton
			// 
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(476, 320);
			this.okButton.Name = "okButton";
			this.okButton.TabIndex = 1;
			this.okButton.Text = "OK";
			// 
			// fieldsGroup
			// 
			this.fieldsGroup.Controls.Add(this.fieldsBox);
			this.fieldsGroup.Controls.Add(this.addFieldButton);
			this.fieldsGroup.Location = new System.Drawing.Point(8, 92);
			this.fieldsGroup.Name = "fieldsGroup";
			this.fieldsGroup.Size = new System.Drawing.Size(136, 220);
			this.fieldsGroup.TabIndex = 2;
			this.fieldsGroup.TabStop = false;
			this.fieldsGroup.Text = " Fields";
			// 
			// fieldsBox
			// 
			this.fieldsBox.Location = new System.Drawing.Point(8, 20);
			this.fieldsBox.Name = "fieldsBox";
			this.fieldsBox.Size = new System.Drawing.Size(120, 160);
			this.fieldsBox.TabIndex = 0;
			this.fieldsBox.DoubleClick += new System.EventHandler(this.fieldsBox_DoubleClick);
			// 
			// addFieldButton
			// 
			this.addFieldButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.addFieldButton.Location = new System.Drawing.Point(8, 188);
			this.addFieldButton.Name = "addFieldButton";
			this.addFieldButton.Size = new System.Drawing.Size(120, 23);
			this.addFieldButton.TabIndex = 1;
			this.addFieldButton.Text = "Insert";
			this.addFieldButton.Click += new System.EventHandler(this.addFieldButton_Click);
			// 
			// operatorsGroup
			// 
			this.operatorsGroup.Controls.Add(this.button29);
			this.operatorsGroup.Controls.Add(this.button16);
			this.operatorsGroup.Controls.Add(this.button15);
			this.operatorsGroup.Controls.Add(this.button14);
			this.operatorsGroup.Controls.Add(this.button13);
			this.operatorsGroup.Controls.Add(this.button12);
			this.operatorsGroup.Controls.Add(this.button11);
			this.operatorsGroup.Controls.Add(this.button10);
			this.operatorsGroup.Controls.Add(this.button9);
			this.operatorsGroup.Controls.Add(this.button8);
			this.operatorsGroup.Controls.Add(this.button7);
			this.operatorsGroup.Controls.Add(this.button6);
			this.operatorsGroup.Controls.Add(this.button5);
			this.operatorsGroup.Controls.Add(this.button4);
			this.operatorsGroup.Controls.Add(this.button3);
			this.operatorsGroup.Controls.Add(this.button2);
			this.operatorsGroup.Controls.Add(this.button1);
			this.operatorsGroup.Location = new System.Drawing.Point(152, 92);
			this.operatorsGroup.Name = "operatorsGroup";
			this.operatorsGroup.Size = new System.Drawing.Size(400, 80);
			this.operatorsGroup.TabIndex = 3;
			this.operatorsGroup.TabStop = false;
			this.operatorsGroup.Text = " Operators";
			// 
			// button1
			// 
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button1.Location = new System.Drawing.Point(204, 48);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(40, 23);
			this.button1.TabIndex = 15;
			this.button1.Tag = " LIKE (\'template%\')";
			this.button1.Text = "LIKE";
			this.button1.Click += new System.EventHandler(this.aggregateFunction_Click);
			// 
			// button2
			// 
			this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button2.Location = new System.Drawing.Point(248, 48);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(40, 23);
			this.button2.TabIndex = 16;
			this.button2.Tag = " IN (expresion, expression)";
			this.button2.Text = "IN";
			this.button2.Click += new System.EventHandler(this.analyticFunction_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(396, 320);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.TabIndex = 6;
			this.cancelButton.Text = "Cancel";
			// 
			// button3
			// 
			this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button3.Location = new System.Drawing.Point(8, 20);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(24, 24);
			this.button3.TabIndex = 0;
			this.button3.Tag = "+";
			this.button3.Text = "+";
			this.button3.Click += new System.EventHandler(this.operator_Click);
			// 
			// button4
			// 
			this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button4.Location = new System.Drawing.Point(36, 20);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(24, 24);
			this.button4.TabIndex = 1;
			this.button4.Tag = "-";
			this.button4.Text = "-";
			this.button4.Click += new System.EventHandler(this.operator_Click);
			// 
			// button5
			// 
			this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button5.Location = new System.Drawing.Point(64, 20);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(24, 24);
			this.button5.TabIndex = 2;
			this.button5.Tag = "*";
			this.button5.Text = "*";
			this.button5.Click += new System.EventHandler(this.operator_Click);
			// 
			// button6
			// 
			this.button6.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button6.Location = new System.Drawing.Point(92, 20);
			this.button6.Name = "button6";
			this.button6.Size = new System.Drawing.Size(24, 24);
			this.button6.TabIndex = 3;
			this.button6.Tag = "/";
			this.button6.Text = "/";
			this.button6.Click += new System.EventHandler(this.operator_Click);
			// 
			// button7
			// 
			this.button7.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button7.Location = new System.Drawing.Point(120, 20);
			this.button7.Name = "button7";
			this.button7.Size = new System.Drawing.Size(24, 24);
			this.button7.TabIndex = 4;
			this.button7.Tag = "^";
			this.button7.Text = "^";
			this.button7.Click += new System.EventHandler(this.operator_Click);
			// 
			// button8
			// 
			this.button8.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button8.Location = new System.Drawing.Point(8, 48);
			this.button8.Name = "button8";
			this.button8.Size = new System.Drawing.Size(36, 23);
			this.button8.TabIndex = 12;
			this.button8.Tag = " And ()";
			this.button8.Text = "And";
			this.button8.Click += new System.EventHandler(this.operator_Click);
			// 
			// button9
			// 
			this.button9.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button9.Location = new System.Drawing.Point(48, 48);
			this.button9.Name = "button9";
			this.button9.Size = new System.Drawing.Size(36, 23);
			this.button9.TabIndex = 13;
			this.button9.Tag = " Or ()";
			this.button9.Text = "Or";
			this.button9.Click += new System.EventHandler(this.operator_Click);
			// 
			// button10
			// 
			this.button10.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button10.Location = new System.Drawing.Point(88, 48);
			this.button10.Name = "button10";
			this.button10.Size = new System.Drawing.Size(36, 23);
			this.button10.TabIndex = 14;
			this.button10.Tag = " Not ()";
			this.button10.Text = "Not";
			this.button10.Click += new System.EventHandler(this.operator_Click);
			// 
			// button11
			// 
			this.button11.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button11.Location = new System.Drawing.Point(204, 20);
			this.button11.Name = "button11";
			this.button11.Size = new System.Drawing.Size(24, 24);
			this.button11.TabIndex = 6;
			this.button11.Tag = "=";
			this.button11.Text = "=";
			this.button11.Click += new System.EventHandler(this.operator_Click);
			// 
			// button12
			// 
			this.button12.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button12.Location = new System.Drawing.Point(232, 20);
			this.button12.Name = "button12";
			this.button12.Size = new System.Drawing.Size(24, 24);
			this.button12.TabIndex = 7;
			this.button12.Tag = "<";
			this.button12.Text = "<";
			this.button12.Click += new System.EventHandler(this.operator_Click);
			// 
			// button13
			// 
			this.button13.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button13.Location = new System.Drawing.Point(260, 20);
			this.button13.Name = "button13";
			this.button13.Size = new System.Drawing.Size(24, 24);
			this.button13.TabIndex = 8;
			this.button13.Tag = ">";
			this.button13.Text = ">";
			this.button13.Click += new System.EventHandler(this.operator_Click);
			// 
			// button14
			// 
			this.button14.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button14.Location = new System.Drawing.Point(288, 20);
			this.button14.Name = "button14";
			this.button14.Size = new System.Drawing.Size(32, 24);
			this.button14.TabIndex = 9;
			this.button14.Tag = ">=";
			this.button14.Text = ">=";
			this.button14.Click += new System.EventHandler(this.operator_Click);
			// 
			// button15
			// 
			this.button15.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button15.Location = new System.Drawing.Point(324, 20);
			this.button15.Name = "button15";
			this.button15.Size = new System.Drawing.Size(32, 24);
			this.button15.TabIndex = 10;
			this.button15.Tag = "<=";
			this.button15.Text = "<=";
			this.button15.Click += new System.EventHandler(this.operator_Click);
			// 
			// button16
			// 
			this.button16.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button16.Location = new System.Drawing.Point(360, 20);
			this.button16.Name = "button16";
			this.button16.Size = new System.Drawing.Size(32, 24);
			this.button16.TabIndex = 11;
			this.button16.Tag = "<>";
			this.button16.Text = "<>";
			this.button16.Click += new System.EventHandler(this.operator_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.button28);
			this.groupBox1.Controls.Add(this.button27);
			this.groupBox1.Controls.Add(this.button26);
			this.groupBox1.Controls.Add(this.button25);
			this.groupBox1.Controls.Add(this.button24);
			this.groupBox1.Controls.Add(this.button23);
			this.groupBox1.Controls.Add(this.button22);
			this.groupBox1.Controls.Add(this.button21);
			this.groupBox1.Controls.Add(this.button20);
			this.groupBox1.Controls.Add(this.button19);
			this.groupBox1.Controls.Add(this.button18);
			this.groupBox1.Controls.Add(this.button17);
			this.groupBox1.Location = new System.Drawing.Point(152, 176);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(400, 80);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = " Functions";
			// 
			// button17
			// 
			this.button17.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button17.Location = new System.Drawing.Point(112, 20);
			this.button17.Name = "button17";
			this.button17.Size = new System.Drawing.Size(40, 23);
			this.button17.TabIndex = 2;
			this.button17.Tag = " LEN (stringExpression)";
			this.button17.Text = "LEN";
			this.button17.Click += new System.EventHandler(this.aggregateFunction_Click);
			// 
			// button18
			// 
			this.button18.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button18.Location = new System.Drawing.Point(8, 20);
			this.button18.Name = "button18";
			this.button18.Size = new System.Drawing.Size(56, 23);
			this.button18.TabIndex = 0;
			this.button18.Tag = " ISNULL (expression, replacementValue)";
			this.button18.Text = "ISNULL";
			this.button18.Click += new System.EventHandler(this.analyticFunction_Click);
			// 
			// button19
			// 
			this.button19.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button19.Location = new System.Drawing.Point(68, 20);
			this.button19.Name = "button19";
			this.button19.Size = new System.Drawing.Size(40, 23);
			this.button19.TabIndex = 1;
			this.button19.Tag = " IIF (expression, trueValue, falseValue)";
			this.button19.Text = "IIF";
			this.button19.Click += new System.EventHandler(this.analyticFunction_Click);
			// 
			// button20
			// 
			this.button20.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button20.Location = new System.Drawing.Point(156, 20);
			this.button20.Name = "button20";
			this.button20.Size = new System.Drawing.Size(48, 23);
			this.button20.TabIndex = 3;
			this.button20.Tag = " TRIM (stringExpression)";
			this.button20.Text = "TRIM";
			this.button20.Click += new System.EventHandler(this.aggregateFunction_Click);
			// 
			// button21
			// 
			this.button21.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button21.Location = new System.Drawing.Point(208, 20);
			this.button21.Name = "button21";
			this.button21.Size = new System.Drawing.Size(76, 23);
			this.button21.TabIndex = 4;
			this.button21.Tag = " SUBSTRING (stringExpression, start, length)";
			this.button21.Text = "SUBSTRING";
			this.button21.Click += new System.EventHandler(this.analyticFunction_Click);
			// 
			// button22
			// 
			this.button22.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button22.Location = new System.Drawing.Point(8, 48);
			this.button22.Name = "button22";
			this.button22.Size = new System.Drawing.Size(40, 23);
			this.button22.TabIndex = 5;
			this.button22.Tag = " Sum (columnExpression)";
			this.button22.Text = "Sum";
			this.button22.Click += new System.EventHandler(this.aggregateFunction_Click);
			// 
			// button23
			// 
			this.button23.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button23.Location = new System.Drawing.Point(52, 48);
			this.button23.Name = "button23";
			this.button23.Size = new System.Drawing.Size(40, 23);
			this.button23.TabIndex = 6;
			this.button23.Tag = " Avg (columnExpression)";
			this.button23.Text = "Avg";
			this.button23.Click += new System.EventHandler(this.aggregateFunction_Click);
			// 
			// button24
			// 
			this.button24.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button24.Location = new System.Drawing.Point(96, 48);
			this.button24.Name = "button24";
			this.button24.Size = new System.Drawing.Size(40, 23);
			this.button24.TabIndex = 7;
			this.button24.Tag = " Max (columnExpression)";
			this.button24.Text = "Max";
			this.button24.Click += new System.EventHandler(this.aggregateFunction_Click);
			// 
			// button25
			// 
			this.button25.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button25.Location = new System.Drawing.Point(140, 48);
			this.button25.Name = "button25";
			this.button25.Size = new System.Drawing.Size(40, 23);
			this.button25.TabIndex = 8;
			this.button25.Tag = " Min (columnExpression)";
			this.button25.Text = "Min";
			this.button25.Click += new System.EventHandler(this.aggregateFunction_Click);
			// 
			// button26
			// 
			this.button26.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button26.Location = new System.Drawing.Point(184, 48);
			this.button26.Name = "button26";
			this.button26.Size = new System.Drawing.Size(48, 23);
			this.button26.TabIndex = 9;
			this.button26.Tag = " Count (columnName)";
			this.button26.Text = "Count";
			this.button26.Click += new System.EventHandler(this.aggregateFunction_Click);
			// 
			// button27
			// 
			this.button27.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button27.Location = new System.Drawing.Point(236, 48);
			this.button27.Name = "button27";
			this.button27.Size = new System.Drawing.Size(48, 23);
			this.button27.TabIndex = 10;
			this.button27.Tag = " StDev (columnExpression)";
			this.button27.Text = "StDev";
			this.button27.Click += new System.EventHandler(this.aggregateFunction_Click);
			// 
			// button28
			// 
			this.button28.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button28.Location = new System.Drawing.Point(288, 48);
			this.button28.Name = "button28";
			this.button28.Size = new System.Drawing.Size(48, 23);
			this.button28.TabIndex = 11;
			this.button28.Tag = " Var (columnExpression)";
			this.button28.Text = "Var";
			this.button28.Click += new System.EventHandler(this.aggregateFunction_Click);
			// 
			// button29
			// 
			this.button29.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button29.Location = new System.Drawing.Point(148, 20);
			this.button29.Name = "button29";
			this.button29.Size = new System.Drawing.Size(24, 24);
			this.button29.TabIndex = 5;
			this.button29.Tag = "%";
			this.button29.Text = "%";
			this.button29.Click += new System.EventHandler(this.operator_Click);
			// 
			// conversionsGroup
			// 
			this.conversionsGroup.Controls.Add(this.button34);
			this.conversionsGroup.Controls.Add(this.button33);
			this.conversionsGroup.Controls.Add(this.button32);
			this.conversionsGroup.Controls.Add(this.button31);
			this.conversionsGroup.Controls.Add(this.button30);
			this.conversionsGroup.Location = new System.Drawing.Point(152, 260);
			this.conversionsGroup.Name = "conversionsGroup";
			this.conversionsGroup.Size = new System.Drawing.Size(400, 52);
			this.conversionsGroup.TabIndex = 5;
			this.conversionsGroup.TabStop = false;
			this.conversionsGroup.Text = " Conversion";
			// 
			// button30
			// 
			this.button30.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button30.Location = new System.Drawing.Point(8, 20);
			this.button30.Name = "button30";
			this.button30.Size = new System.Drawing.Size(60, 23);
			this.button30.TabIndex = 0;
			this.button30.Tag = " CONVERT (expression, \'System.String\')";
			this.button30.Text = "ToString";
			this.button30.Click += new System.EventHandler(this.conversion_Click);
			// 
			// button31
			// 
			this.button31.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button31.Location = new System.Drawing.Point(72, 20);
			this.button31.Name = "button31";
			this.button31.Size = new System.Drawing.Size(60, 23);
			this.button31.TabIndex = 1;
			this.button31.Tag = " CONVERT (expression, \'System.DateTime\')";
			this.button31.Text = "ToDate";
			this.button31.Click += new System.EventHandler(this.conversion_Click);
			// 
			// button32
			// 
			this.button32.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button32.Location = new System.Drawing.Point(136, 20);
			this.button32.Name = "button32";
			this.button32.Size = new System.Drawing.Size(60, 23);
			this.button32.TabIndex = 2;
			this.button32.Tag = " CONVERT (expression, \'System.Int32\')";
			this.button32.Text = "ToInt32";
			this.button32.Click += new System.EventHandler(this.conversion_Click);
			// 
			// button33
			// 
			this.button33.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button33.Location = new System.Drawing.Point(200, 20);
			this.button33.Name = "button33";
			this.button33.Size = new System.Drawing.Size(60, 23);
			this.button33.TabIndex = 3;
			this.button33.Tag = " CONVERT (expression, \'System.Int64\')";
			this.button33.Text = "ToInt64";
			this.button33.Click += new System.EventHandler(this.conversion_Click);
			// 
			// button34
			// 
			this.button34.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button34.Location = new System.Drawing.Point(264, 20);
			this.button34.Name = "button34";
			this.button34.Size = new System.Drawing.Size(76, 23);
			this.button34.TabIndex = 4;
			this.button34.Tag = " CONVERT (expression, \'System.Boolean\')";
			this.button34.Text = "ToBoolean";
			this.button34.Click += new System.EventHandler(this.conversion_Click);
			// 
			// toolTip
			// 
			this.toolTip.AutoPopDelay = 10000;
			this.toolTip.InitialDelay = 400;
			this.toolTip.ReshowDelay = 100;
			// 
			// ConditionEditorDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(558, 347);
			this.Controls.Add(this.conversionsGroup);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.operatorsGroup);
			this.Controls.Add(this.fieldsGroup);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.conditionField);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ConditionEditorDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Condition Editor";
			this.fieldsGroup.ResumeLayout(false);
			this.operatorsGroup.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.conversionsGroup.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void addFieldButton_Click(object sender, System.EventArgs e)
		{
			AddText("[" + fieldsBox.SelectedItem.ToString() + "]");
		}

		private void AddText(string text)
		{
			conditionField.SelectedText = text;
			conditionField.Focus();
		}

		private void AddSelectedText(string text, string selectionEndMarker)
		{
			int startPosition = conditionField.SelectionStart;
			conditionField.SelectedText = text;

			int selStart = conditionField.Text.IndexOf("(", startPosition+1, text.Length-1) + 1; 
			int selEnd = conditionField.Text.IndexOf(selectionEndMarker, startPosition+1, text.Length-1); 

			conditionField.SelectionStart = selStart;
			conditionField.SelectionLength = selEnd - selStart;
			conditionField.Focus();
		}

		private void operator_Click(object sender, System.EventArgs e)
		{
			Button b = sender as Button;
			AddText(b.Tag.ToString());
		}

		private void conversion_Click(object sender, System.EventArgs e)
		{
			Button b = sender as Button;
			AddSelectedText(b.Tag.ToString(), ",");
		}

		private void analyticFunction_Click(object sender, System.EventArgs e)
		{
			Button b = sender as Button;
			AddSelectedText(b.Tag.ToString(), ",");
		}

		private void aggregateFunction_Click(object sender, System.EventArgs e)
		{
			Button b = sender as Button;
			AddSelectedText(b.Tag.ToString(), ")");
		}

		private void fieldsBox_DoubleClick(object sender, System.EventArgs e)
		{
			if (fieldsBox.SelectedItem != null)
			{
				AddText("[" + fieldsBox.SelectedItem.ToString() + "]");
			}
		}

		/// <summary>
		/// Gets or sets the where condition to edit.
		/// </summary>
		public string Condition
		{
			get {return conditionField.Text;}
			set {conditionField.Text = value;}
		}
	}
}
