namespace AutoCAD_ESI_General_Menu
{
    partial class Logo
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
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.AERadioButton = new System.Windows.Forms.RadioButton();
            this.BRadioButton = new System.Windows.Forms.RadioButton();
            this.CRadioButton = new System.Windows.Forms.RadioButton();
            this.DRadioButton = new System.Windows.Forms.RadioButton();
            this.LogoListBox = new System.Windows.Forms.ListBox();
            this.LogoPictureBox = new System.Windows.Forms.PictureBox();
            this.CancelButton = new System.Windows.Forms.Button();
            this.OKButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LogoPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Logo Size";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.DRadioButton);
            this.panel1.Controls.Add(this.CRadioButton);
            this.panel1.Controls.Add(this.BRadioButton);
            this.panel1.Controls.Add(this.AERadioButton);
            this.panel1.Location = new System.Drawing.Point(30, 65);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(112, 188);
            this.panel1.TabIndex = 1;
            // 
            // AERadioButton
            // 
            this.AERadioButton.AutoSize = true;
            this.AERadioButton.Location = new System.Drawing.Point(18, 14);
            this.AERadioButton.Name = "AERadioButton";
            this.AERadioButton.Size = new System.Drawing.Size(47, 21);
            this.AERadioButton.TabIndex = 0;
            this.AERadioButton.TabStop = true;
            this.AERadioButton.Text = "AE";
            this.AERadioButton.UseVisualStyleBackColor = true;
            this.AERadioButton.CheckedChanged += new System.EventHandler(this.AERadioButtion_CheckedChanged);
            // 
            // BRadioButton
            // 
            this.BRadioButton.AutoSize = true;
            this.BRadioButton.Location = new System.Drawing.Point(18, 42);
            this.BRadioButton.Name = "BRadioButton";
            this.BRadioButton.Size = new System.Drawing.Size(38, 21);
            this.BRadioButton.TabIndex = 1;
            this.BRadioButton.TabStop = true;
            this.BRadioButton.Text = "B";
            this.BRadioButton.UseVisualStyleBackColor = true;
            this.BRadioButton.CheckedChanged += new System.EventHandler(this.BRadioButtion_CheckedChanged);
            // 
            // CRadioButton
            // 
            this.CRadioButton.AutoSize = true;
            this.CRadioButton.Location = new System.Drawing.Point(18, 70);
            this.CRadioButton.Name = "CRadioButton";
            this.CRadioButton.Size = new System.Drawing.Size(38, 21);
            this.CRadioButton.TabIndex = 2;
            this.CRadioButton.TabStop = true;
            this.CRadioButton.Text = "C";
            this.CRadioButton.UseVisualStyleBackColor = true;
            this.CRadioButton.CheckedChanged += new System.EventHandler(this.CRadioButtion_CheckedChanged);
            // 
            // DRadioButton
            // 
            this.DRadioButton.AutoSize = true;
            this.DRadioButton.Location = new System.Drawing.Point(18, 98);
            this.DRadioButton.Name = "DRadioButton";
            this.DRadioButton.Size = new System.Drawing.Size(39, 21);
            this.DRadioButton.TabIndex = 3;
            this.DRadioButton.TabStop = true;
            this.DRadioButton.Text = "D";
            this.DRadioButton.UseVisualStyleBackColor = true;
            this.DRadioButton.CheckedChanged += new System.EventHandler(this.DRadioButtion_CheckedChanged);
            // 
            // LogoListBox
            // 
            this.LogoListBox.FormattingEnabled = true;
            this.LogoListBox.ItemHeight = 16;
            this.LogoListBox.Location = new System.Drawing.Point(174, 65);
            this.LogoListBox.Name = "LogoListBox";
            this.LogoListBox.Size = new System.Drawing.Size(162, 180);
            this.LogoListBox.TabIndex = 2;
            this.LogoListBox.SelectedIndexChanged += new System.EventHandler(this.LogoListBox_SelectedIndexChanged);
            // 
            // LogoPictureBox
            // 
            this.LogoPictureBox.Location = new System.Drawing.Point(369, 65);
            this.LogoPictureBox.Margin = new System.Windows.Forms.Padding(4);
            this.LogoPictureBox.Name = "LogoPictureBox";
            this.LogoPictureBox.Size = new System.Drawing.Size(184, 188);
            this.LogoPictureBox.TabIndex = 5;
            this.LogoPictureBox.TabStop = false;
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(342, 276);
            this.CancelButton.Margin = new System.Windows.Forms.Padding(4);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(100, 28);
            this.CancelButton.TabIndex = 11;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // OKButton
            // 
            this.OKButton.Location = new System.Drawing.Point(120, 276);
            this.OKButton.Margin = new System.Windows.Forms.Padding(4);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(100, 28);
            this.OKButton.TabIndex = 10;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(171, 36);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 17);
            this.label3.TabIndex = 12;
            this.label3.Text = "Logo";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(366, 36);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 17);
            this.label4.TabIndex = 13;
            this.label4.Text = "Logo Preview";
            // 
            // Logo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(583, 330);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.LogoPictureBox);
            this.Controls.Add(this.LogoListBox);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Name = "Logo";
            this.Text = "Logo";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LogoPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton DRadioButton;
        private System.Windows.Forms.RadioButton CRadioButton;
        private System.Windows.Forms.RadioButton BRadioButton;
        private System.Windows.Forms.RadioButton AERadioButton;
        private System.Windows.Forms.ListBox LogoListBox;
        private System.Windows.Forms.PictureBox LogoPictureBox;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}