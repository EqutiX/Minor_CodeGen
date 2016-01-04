namespace CodeGenStudio
{
	partial class CodeGenStudio
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CodeGenStudio));
			this.NamespaceBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.PublishButton = new System.Windows.Forms.Button();
			this.NamespaceCreateButton = new System.Windows.Forms.Button();
			this.ClassesPanel = new System.Windows.Forms.Panel();
			this.CreateClassButton = new System.Windows.Forms.Button();
			this.ClassNameBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.ClassPanel = new System.Windows.Forms.Panel();
			this.label8 = new System.Windows.Forms.Label();
			this.MethodAccessBox = new System.Windows.Forms.ComboBox();
			this.PropertyAccessBox = new System.Windows.Forms.ComboBox();
			this.FieldAccessBox = new System.Windows.Forms.ComboBox();
			this.MethodAddButton = new System.Windows.Forms.Button();
			this.PropertyAddButton = new System.Windows.Forms.Button();
			this.FieldAddButton = new System.Windows.Forms.Button();
			this.MethodNameBox = new System.Windows.Forms.TextBox();
			this.PropertyNameBox = new System.Windows.Forms.TextBox();
			this.FieldNameBox = new System.Windows.Forms.TextBox();
			this.MethodTypeBox = new System.Windows.Forms.ComboBox();
			this.PropertyTypeBox = new System.Windows.Forms.ComboBox();
			this.FieldTypeBox = new System.Windows.Forms.ComboBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.MembersBox = new System.Windows.Forms.ListBox();
			this.SaveClassButton = new System.Windows.Forms.Button();
			this.ClassesBox = new System.Windows.Forms.ListBox();
			this.label2 = new System.Windows.Forms.Label();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.GenerateCodeBox = new System.Windows.Forms.ComboBox();
			this.ClassesPanel.SuspendLayout();
			this.ClassPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// NamespaceBox
			// 
			resources.ApplyResources(this.NamespaceBox, "NamespaceBox");
			this.NamespaceBox.Name = "NamespaceBox";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// PublishButton
			// 
			resources.ApplyResources(this.PublishButton, "PublishButton");
			this.PublishButton.Name = "PublishButton";
			this.PublishButton.UseVisualStyleBackColor = true;
			this.PublishButton.Click += new System.EventHandler(this.button1_Click);
			// 
			// NamespaceCreateButton
			// 
			resources.ApplyResources(this.NamespaceCreateButton, "NamespaceCreateButton");
			this.NamespaceCreateButton.Name = "NamespaceCreateButton";
			this.NamespaceCreateButton.UseVisualStyleBackColor = true;
			this.NamespaceCreateButton.Click += new System.EventHandler(this.NamespaceCreateButton_Click);
			// 
			// ClassesPanel
			// 
			this.ClassesPanel.Controls.Add(this.CreateClassButton);
			this.ClassesPanel.Controls.Add(this.ClassNameBox);
			this.ClassesPanel.Controls.Add(this.label3);
			this.ClassesPanel.Controls.Add(this.ClassPanel);
			this.ClassesPanel.Controls.Add(this.ClassesBox);
			this.ClassesPanel.Controls.Add(this.label2);
			resources.ApplyResources(this.ClassesPanel, "ClassesPanel");
			this.ClassesPanel.Name = "ClassesPanel";
			// 
			// CreateClassButton
			// 
			resources.ApplyResources(this.CreateClassButton, "CreateClassButton");
			this.CreateClassButton.Name = "CreateClassButton";
			this.CreateClassButton.UseVisualStyleBackColor = true;
			this.CreateClassButton.Click += new System.EventHandler(this.CreateClassButton_Click);
			// 
			// ClassNameBox
			// 
			resources.ApplyResources(this.ClassNameBox, "ClassNameBox");
			this.ClassNameBox.Name = "ClassNameBox";
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// ClassPanel
			// 
			this.ClassPanel.Controls.Add(this.label8);
			this.ClassPanel.Controls.Add(this.MethodAccessBox);
			this.ClassPanel.Controls.Add(this.PropertyAccessBox);
			this.ClassPanel.Controls.Add(this.FieldAccessBox);
			this.ClassPanel.Controls.Add(this.MethodAddButton);
			this.ClassPanel.Controls.Add(this.PropertyAddButton);
			this.ClassPanel.Controls.Add(this.FieldAddButton);
			this.ClassPanel.Controls.Add(this.MethodNameBox);
			this.ClassPanel.Controls.Add(this.PropertyNameBox);
			this.ClassPanel.Controls.Add(this.FieldNameBox);
			this.ClassPanel.Controls.Add(this.MethodTypeBox);
			this.ClassPanel.Controls.Add(this.PropertyTypeBox);
			this.ClassPanel.Controls.Add(this.FieldTypeBox);
			this.ClassPanel.Controls.Add(this.label7);
			this.ClassPanel.Controls.Add(this.label6);
			this.ClassPanel.Controls.Add(this.label5);
			this.ClassPanel.Controls.Add(this.label4);
			this.ClassPanel.Controls.Add(this.MembersBox);
			this.ClassPanel.Controls.Add(this.SaveClassButton);
			resources.ApplyResources(this.ClassPanel, "ClassPanel");
			this.ClassPanel.Name = "ClassPanel";
			// 
			// label8
			// 
			resources.ApplyResources(this.label8, "label8");
			this.label8.Name = "label8";
			// 
			// MethodAccessBox
			// 
			this.MethodAccessBox.FormattingEnabled = true;
			this.MethodAccessBox.Items.AddRange(new object[] {
            resources.GetString("MethodAccessBox.Items"),
            resources.GetString("MethodAccessBox.Items1"),
            resources.GetString("MethodAccessBox.Items2")});
			resources.ApplyResources(this.MethodAccessBox, "MethodAccessBox");
			this.MethodAccessBox.Name = "MethodAccessBox";
			// 
			// PropertyAccessBox
			// 
			this.PropertyAccessBox.FormattingEnabled = true;
			this.PropertyAccessBox.Items.AddRange(new object[] {
            resources.GetString("PropertyAccessBox.Items"),
            resources.GetString("PropertyAccessBox.Items1"),
            resources.GetString("PropertyAccessBox.Items2")});
			resources.ApplyResources(this.PropertyAccessBox, "PropertyAccessBox");
			this.PropertyAccessBox.Name = "PropertyAccessBox";
			// 
			// FieldAccessBox
			// 
			this.FieldAccessBox.FormattingEnabled = true;
			this.FieldAccessBox.Items.AddRange(new object[] {
            resources.GetString("FieldAccessBox.Items"),
            resources.GetString("FieldAccessBox.Items1"),
            resources.GetString("FieldAccessBox.Items2")});
			resources.ApplyResources(this.FieldAccessBox, "FieldAccessBox");
			this.FieldAccessBox.Name = "FieldAccessBox";
			// 
			// MethodAddButton
			// 
			resources.ApplyResources(this.MethodAddButton, "MethodAddButton");
			this.MethodAddButton.Name = "MethodAddButton";
			this.MethodAddButton.UseVisualStyleBackColor = true;
			this.MethodAddButton.Click += new System.EventHandler(this.MethodAddButton_Click);
			// 
			// PropertyAddButton
			// 
			resources.ApplyResources(this.PropertyAddButton, "PropertyAddButton");
			this.PropertyAddButton.Name = "PropertyAddButton";
			this.PropertyAddButton.UseVisualStyleBackColor = true;
			this.PropertyAddButton.Click += new System.EventHandler(this.PropertyAddButton_Click);
			// 
			// FieldAddButton
			// 
			resources.ApplyResources(this.FieldAddButton, "FieldAddButton");
			this.FieldAddButton.Name = "FieldAddButton";
			this.FieldAddButton.UseVisualStyleBackColor = true;
			this.FieldAddButton.Click += new System.EventHandler(this.FieldAddButton_Click);
			// 
			// MethodNameBox
			// 
			resources.ApplyResources(this.MethodNameBox, "MethodNameBox");
			this.MethodNameBox.Name = "MethodNameBox";
			// 
			// PropertyNameBox
			// 
			resources.ApplyResources(this.PropertyNameBox, "PropertyNameBox");
			this.PropertyNameBox.Name = "PropertyNameBox";
			// 
			// FieldNameBox
			// 
			resources.ApplyResources(this.FieldNameBox, "FieldNameBox");
			this.FieldNameBox.Name = "FieldNameBox";
			// 
			// MethodTypeBox
			// 
			this.MethodTypeBox.FormattingEnabled = true;
			this.MethodTypeBox.Items.AddRange(new object[] {
            resources.GetString("MethodTypeBox.Items"),
            resources.GetString("MethodTypeBox.Items1"),
            resources.GetString("MethodTypeBox.Items2"),
            resources.GetString("MethodTypeBox.Items3")});
			resources.ApplyResources(this.MethodTypeBox, "MethodTypeBox");
			this.MethodTypeBox.Name = "MethodTypeBox";
			// 
			// PropertyTypeBox
			// 
			this.PropertyTypeBox.FormattingEnabled = true;
			this.PropertyTypeBox.Items.AddRange(new object[] {
            resources.GetString("PropertyTypeBox.Items"),
            resources.GetString("PropertyTypeBox.Items1"),
            resources.GetString("PropertyTypeBox.Items2")});
			resources.ApplyResources(this.PropertyTypeBox, "PropertyTypeBox");
			this.PropertyTypeBox.Name = "PropertyTypeBox";
			// 
			// FieldTypeBox
			// 
			this.FieldTypeBox.FormattingEnabled = true;
			this.FieldTypeBox.Items.AddRange(new object[] {
            resources.GetString("FieldTypeBox.Items"),
            resources.GetString("FieldTypeBox.Items1"),
            resources.GetString("FieldTypeBox.Items2")});
			resources.ApplyResources(this.FieldTypeBox, "FieldTypeBox");
			this.FieldTypeBox.Name = "FieldTypeBox";
			// 
			// label7
			// 
			resources.ApplyResources(this.label7, "label7");
			this.label7.Name = "label7";
			// 
			// label6
			// 
			resources.ApplyResources(this.label6, "label6");
			this.label6.Name = "label6";
			// 
			// label5
			// 
			resources.ApplyResources(this.label5, "label5");
			this.label5.Name = "label5";
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			// 
			// MembersBox
			// 
			this.MembersBox.FormattingEnabled = true;
			resources.ApplyResources(this.MembersBox, "MembersBox");
			this.MembersBox.Name = "MembersBox";
			// 
			// SaveClassButton
			// 
			resources.ApplyResources(this.SaveClassButton, "SaveClassButton");
			this.SaveClassButton.Name = "SaveClassButton";
			this.SaveClassButton.UseVisualStyleBackColor = true;
			this.SaveClassButton.Click += new System.EventHandler(this.SaveClassButton_Click);
			// 
			// ClassesBox
			// 
			this.ClassesBox.FormattingEnabled = true;
			resources.ApplyResources(this.ClassesBox, "ClassesBox");
			this.ClassesBox.Name = "ClassesBox";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// GenerateCodeBox
			// 
			this.GenerateCodeBox.FormattingEnabled = true;
			this.GenerateCodeBox.Items.AddRange(new object[] {
            resources.GetString("GenerateCodeBox.Items"),
            resources.GetString("GenerateCodeBox.Items1"),
            resources.GetString("GenerateCodeBox.Items2"),
            resources.GetString("GenerateCodeBox.Items3")});
			resources.ApplyResources(this.GenerateCodeBox, "GenerateCodeBox");
			this.GenerateCodeBox.Name = "GenerateCodeBox";
			// 
			// CodeGenStudio
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.GenerateCodeBox);
			this.Controls.Add(this.ClassesPanel);
			this.Controls.Add(this.NamespaceCreateButton);
			this.Controls.Add(this.PublishButton);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.NamespaceBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CodeGenStudio";
			this.ClassesPanel.ResumeLayout(false);
			this.ClassesPanel.PerformLayout();
			this.ClassPanel.ResumeLayout(false);
			this.ClassPanel.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox NamespaceBox;
		private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button PublishButton;
        private System.Windows.Forms.Button NamespaceCreateButton;
        private System.Windows.Forms.Panel ClassesPanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button CreateClassButton;
        private System.Windows.Forms.TextBox ClassNameBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel ClassPanel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox MembersBox;
        private System.Windows.Forms.Button SaveClassButton;
        private System.Windows.Forms.ListBox ClassesBox;
        private System.Windows.Forms.Button MethodAddButton;
        private System.Windows.Forms.Button PropertyAddButton;
        private System.Windows.Forms.Button FieldAddButton;
        private System.Windows.Forms.TextBox MethodNameBox;
        private System.Windows.Forms.TextBox PropertyNameBox;
        private System.Windows.Forms.TextBox FieldNameBox;
        private System.Windows.Forms.ComboBox MethodTypeBox;
        private System.Windows.Forms.ComboBox PropertyTypeBox;
        private System.Windows.Forms.ComboBox FieldTypeBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.ComboBox MethodAccessBox;
		private System.Windows.Forms.ComboBox PropertyAccessBox;
		private System.Windows.Forms.ComboBox FieldAccessBox;
		private System.Windows.Forms.ComboBox GenerateCodeBox;
	}
}

