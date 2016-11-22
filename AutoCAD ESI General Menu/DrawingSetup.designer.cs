namespace AutoCAD_ESI_General_Menu
{
    partial class DrawingSetup
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.OtherRadioButton = new System.Windows.Forms.RadioButton();
            this.DRadioButton = new System.Windows.Forms.RadioButton();
            this.CRadioButton = new System.Windows.Forms.RadioButton();
            this.BRadioButton = new System.Windows.Forms.RadioButton();
            this.AERadioButton = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.BorderSheetListBox = new System.Windows.Forms.ListBox();
            this.LogoListBox = new System.Windows.Forms.ListBox();
            this.LogoPictureBox = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.OKButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.CoverSheetlistBox = new System.Windows.Forms.ListBox();
            this.CoverSheetCheckBox = new System.Windows.Forms.CheckBox();
            this.CoverSheetPictureBox = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LogoPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CoverSheetPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.OtherRadioButton);
            this.panel1.Controls.Add(this.DRadioButton);
            this.panel1.Controls.Add(this.CRadioButton);
            this.panel1.Controls.Add(this.BRadioButton);
            this.panel1.Controls.Add(this.AERadioButton);
            this.panel1.Location = new System.Drawing.Point(26, 42);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(94, 145);
            this.panel1.TabIndex = 0;
            // 
            // OtherRadioButton
            // 
            this.OtherRadioButton.AutoSize = true;
            this.OtherRadioButton.Location = new System.Drawing.Point(20, 102);
            this.OtherRadioButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.OtherRadioButton.Name = "OtherRadioButton";
            this.OtherRadioButton.Size = new System.Drawing.Size(51, 17);
            this.OtherRadioButton.TabIndex = 4;
            this.OtherRadioButton.TabStop = true;
            this.OtherRadioButton.Text = "Other";
            this.OtherRadioButton.UseVisualStyleBackColor = true;
            this.OtherRadioButton.CheckedChanged += new System.EventHandler(this.OtherRadioButton_CheckedChanged);
            // 
            // DRadioButton
            // 
            this.DRadioButton.AutoSize = true;
            this.DRadioButton.Location = new System.Drawing.Point(20, 80);
            this.DRadioButton.Name = "DRadioButton";
            this.DRadioButton.Size = new System.Drawing.Size(33, 17);
            this.DRadioButton.TabIndex = 3;
            this.DRadioButton.TabStop = true;
            this.DRadioButton.Text = "D";
            this.DRadioButton.UseVisualStyleBackColor = true;
            this.DRadioButton.CheckedChanged += new System.EventHandler(this.DRadioButtion_CheckedChanged);
            // 
            // CRadioButton
            // 
            this.CRadioButton.AutoSize = true;
            this.CRadioButton.Location = new System.Drawing.Point(20, 57);
            this.CRadioButton.Name = "CRadioButton";
            this.CRadioButton.Size = new System.Drawing.Size(32, 17);
            this.CRadioButton.TabIndex = 2;
            this.CRadioButton.TabStop = true;
            this.CRadioButton.Text = "C";
            this.CRadioButton.UseVisualStyleBackColor = true;
            this.CRadioButton.CheckedChanged += new System.EventHandler(this.CRadioButtion_CheckedChanged);
            // 
            // BRadioButton
            // 
            this.BRadioButton.AutoSize = true;
            this.BRadioButton.Location = new System.Drawing.Point(20, 34);
            this.BRadioButton.Name = "BRadioButton";
            this.BRadioButton.Size = new System.Drawing.Size(32, 17);
            this.BRadioButton.TabIndex = 1;
            this.BRadioButton.TabStop = true;
            this.BRadioButton.Text = "B";
            this.BRadioButton.UseVisualStyleBackColor = true;
            this.BRadioButton.CheckedChanged += new System.EventHandler(this.BRadioButtion_CheckedChanged);
            // 
            // AERadioButton
            // 
            this.AERadioButton.AutoSize = true;
            this.AERadioButton.Location = new System.Drawing.Point(20, 11);
            this.AERadioButton.Name = "AERadioButton";
            this.AERadioButton.Size = new System.Drawing.Size(39, 17);
            this.AERadioButton.TabIndex = 0;
            this.AERadioButton.TabStop = true;
            this.AERadioButton.Text = "AE";
            this.AERadioButton.UseVisualStyleBackColor = true;
            this.AERadioButton.CheckedChanged += new System.EventHandler(this.AERadioButtion_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Border Size";
            // 
            // BorderSheetListBox
            // 
            this.BorderSheetListBox.FormattingEnabled = true;
            this.BorderSheetListBox.Location = new System.Drawing.Point(141, 42);
            this.BorderSheetListBox.Name = "BorderSheetListBox";
            this.BorderSheetListBox.Size = new System.Drawing.Size(186, 134);
            this.BorderSheetListBox.TabIndex = 2;
            // 
            // LogoListBox
            // 
            this.LogoListBox.FormattingEnabled = true;
            this.LogoListBox.Location = new System.Drawing.Point(334, 42);
            this.LogoListBox.Name = "LogoListBox";
            this.LogoListBox.Size = new System.Drawing.Size(171, 134);
            this.LogoListBox.TabIndex = 3;
            this.LogoListBox.SelectedIndexChanged += new System.EventHandler(this.LogoListBox_SelectedIndexChanged);
            // 
            // LogoPictureBox
            // 
            this.LogoPictureBox.Location = new System.Drawing.Point(525, 42);
            this.LogoPictureBox.Name = "LogoPictureBox";
            this.LogoPictureBox.Size = new System.Drawing.Size(147, 145);
            this.LogoPictureBox.TabIndex = 4;
            this.LogoPictureBox.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(138, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Border Sheet";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(331, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Logo";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(525, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Logo Preview";
            // 
            // OKButton
            // 
            this.OKButton.Location = new System.Drawing.Point(704, 42);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(80, 22);
            this.OKButton.TabIndex = 8;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(704, 75);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(80, 22);
            this.CancelButton.TabIndex = 9;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // CoverSheetlistBox
            // 
            this.CoverSheetlistBox.FormattingEnabled = true;
            this.CoverSheetlistBox.Location = new System.Drawing.Point(334, 218);
            this.CoverSheetlistBox.Name = "CoverSheetlistBox";
            this.CoverSheetlistBox.Size = new System.Drawing.Size(171, 147);
            this.CoverSheetlistBox.TabIndex = 10;
            this.CoverSheetlistBox.Visible = false;
            this.CoverSheetlistBox.SelectedIndexChanged += new System.EventHandler(this.CoverSheetlistBox_SelectedIndexChanged);
            // 
            // CoverSheetCheckBox
            // 
            this.CoverSheetCheckBox.AutoSize = true;
            this.CoverSheetCheckBox.Location = new System.Drawing.Point(678, 170);
            this.CoverSheetCheckBox.Name = "CoverSheetCheckBox";
            this.CoverSheetCheckBox.Size = new System.Drawing.Size(123, 17);
            this.CoverSheetCheckBox.TabIndex = 11;
            this.CoverSheetCheckBox.Text = "Include Cover Sheet";
            this.CoverSheetCheckBox.UseVisualStyleBackColor = true;
            this.CoverSheetCheckBox.CheckedChanged += new System.EventHandler(this.CoverSheetCheckBox_CheckedChanged);
            // 
            // CoverSheetPictureBox
            // 
            this.CoverSheetPictureBox.Location = new System.Drawing.Point(525, 218);
            this.CoverSheetPictureBox.Name = "CoverSheetPictureBox";
            this.CoverSheetPictureBox.Size = new System.Drawing.Size(147, 158);
            this.CoverSheetPictureBox.TabIndex = 12;
            this.CoverSheetPictureBox.TabStop = false;
            this.CoverSheetPictureBox.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(525, 202);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(104, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "CoverSheet Preview";
            this.label5.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(331, 202);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "CoverSheet";
            this.label6.Visible = false;
            // 
            // DrawingSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(836, 422);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.CoverSheetPictureBox);
            this.Controls.Add(this.CoverSheetCheckBox);
            this.Controls.Add(this.CoverSheetlistBox);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.LogoPictureBox);
            this.Controls.Add(this.LogoListBox);
            this.Controls.Add(this.BorderSheetListBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Name = "DrawingSetup";
            this.Text = "DrawingSetup";
            this.Load += new System.EventHandler(this.DrawingSetup_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LogoPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CoverSheetPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton CRadioButton;
        private System.Windows.Forms.RadioButton BRadioButton;
        private System.Windows.Forms.RadioButton AERadioButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton DRadioButton;
        private System.Windows.Forms.ListBox BorderSheetListBox;
        private System.Windows.Forms.ListBox LogoListBox;
        private System.Windows.Forms.PictureBox LogoPictureBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.RadioButton OtherRadioButton;
        private System.Windows.Forms.ListBox CoverSheetlistBox;
        private System.Windows.Forms.CheckBox CoverSheetCheckBox;
        private System.Windows.Forms.PictureBox CoverSheetPictureBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;

    }
}