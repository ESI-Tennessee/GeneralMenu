namespace AutoCAD_ESI_General_Menu
{
    partial class CoverSheet
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
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.CoverSheetPictureBox = new System.Windows.Forms.PictureBox();
            this.CoverSheetlistBox = new System.Windows.Forms.ListBox();
            this.CancelButton = new System.Windows.Forms.Button();
            this.OKButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.DRadioButton = new System.Windows.Forms.RadioButton();
            this.CRadioButton = new System.Windows.Forms.RadioButton();
            this.BRadioButton = new System.Windows.Forms.RadioButton();
            this.AERadioButton = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.CoverSheetPictureBox)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(121, 17);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 13);
            this.label6.TabIndex = 18;
            this.label6.Text = "CoverSheet";
            this.label6.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(315, 17);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(104, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "CoverSheet Preview";
            this.label5.Visible = false;
            // 
            // CoverSheetPictureBox
            // 
            this.CoverSheetPictureBox.Location = new System.Drawing.Point(315, 33);
            this.CoverSheetPictureBox.Name = "CoverSheetPictureBox";
            this.CoverSheetPictureBox.Size = new System.Drawing.Size(147, 158);
            this.CoverSheetPictureBox.TabIndex = 16;
            this.CoverSheetPictureBox.TabStop = false;
            // 
            // CoverSheetlistBox
            // 
            this.CoverSheetlistBox.FormattingEnabled = true;
            this.CoverSheetlistBox.Location = new System.Drawing.Point(124, 33);
            this.CoverSheetlistBox.Name = "CoverSheetlistBox";
            this.CoverSheetlistBox.Size = new System.Drawing.Size(171, 147);
            this.CoverSheetlistBox.TabIndex = 15;
            this.CoverSheetlistBox.SelectedIndexChanged += new System.EventHandler(this.CoverSheetlistBox_SelectedIndexChanged);
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(265, 208);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(80, 22);
            this.CancelButton.TabIndex = 20;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // OKButton
            // 
            this.OKButton.Location = new System.Drawing.Point(84, 208);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(80, 22);
            this.OKButton.TabIndex = 19;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.DRadioButton);
            this.panel1.Controls.Add(this.CRadioButton);
            this.panel1.Controls.Add(this.BRadioButton);
            this.panel1.Controls.Add(this.AERadioButton);
            this.panel1.Location = new System.Drawing.Point(13, 33);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(84, 153);
            this.panel1.TabIndex = 22;
            // 
            // DRadioButton
            // 
            this.DRadioButton.AutoSize = true;
            this.DRadioButton.Location = new System.Drawing.Point(14, 80);
            this.DRadioButton.Margin = new System.Windows.Forms.Padding(2);
            this.DRadioButton.Name = "DRadioButton";
            this.DRadioButton.Size = new System.Drawing.Size(33, 17);
            this.DRadioButton.TabIndex = 3;
            this.DRadioButton.TabStop = true;
            this.DRadioButton.Text = "D";
            this.DRadioButton.UseVisualStyleBackColor = true;
            this.DRadioButton.CheckedChanged += new System.EventHandler(this.DRadioButton_CheckedChanged);
            // 
            // CRadioButton
            // 
            this.CRadioButton.AutoSize = true;
            this.CRadioButton.Location = new System.Drawing.Point(14, 57);
            this.CRadioButton.Margin = new System.Windows.Forms.Padding(2);
            this.CRadioButton.Name = "CRadioButton";
            this.CRadioButton.Size = new System.Drawing.Size(32, 17);
            this.CRadioButton.TabIndex = 2;
            this.CRadioButton.TabStop = true;
            this.CRadioButton.Text = "C";
            this.CRadioButton.UseVisualStyleBackColor = true;
            this.CRadioButton.CheckedChanged += new System.EventHandler(this.CRadioButton_CheckedChanged);
            // 
            // BRadioButton
            // 
            this.BRadioButton.AutoSize = true;
            this.BRadioButton.Location = new System.Drawing.Point(14, 34);
            this.BRadioButton.Margin = new System.Windows.Forms.Padding(2);
            this.BRadioButton.Name = "BRadioButton";
            this.BRadioButton.Size = new System.Drawing.Size(32, 17);
            this.BRadioButton.TabIndex = 1;
            this.BRadioButton.TabStop = true;
            this.BRadioButton.Text = "B";
            this.BRadioButton.UseVisualStyleBackColor = true;
            this.BRadioButton.CheckedChanged += new System.EventHandler(this.BRadioButton_CheckedChanged);
            // 
            // AERadioButton
            // 
            this.AERadioButton.AutoSize = true;
            this.AERadioButton.Location = new System.Drawing.Point(14, 11);
            this.AERadioButton.Margin = new System.Windows.Forms.Padding(2);
            this.AERadioButton.Name = "AERadioButton";
            this.AERadioButton.Size = new System.Drawing.Size(39, 17);
            this.AERadioButton.TabIndex = 0;
            this.AERadioButton.TabStop = true;
            this.AERadioButton.Text = "AE";
            this.AERadioButton.UseVisualStyleBackColor = true;
            this.AERadioButton.CheckedChanged += new System.EventHandler(this.AERadioButton_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Logo Size";
            // 
            // CoverSheet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 242);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.CoverSheetPictureBox);
            this.Controls.Add(this.CoverSheetlistBox);
            this.Name = "CoverSheet";
            this.Text = "CoverSheet";
            ((System.ComponentModel.ISupportInitialize)(this.CoverSheetPictureBox)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.PictureBox CoverSheetPictureBox;
        private System.Windows.Forms.ListBox CoverSheetlistBox;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton DRadioButton;
        private System.Windows.Forms.RadioButton CRadioButton;
        private System.Windows.Forms.RadioButton BRadioButton;
        private System.Windows.Forms.RadioButton AERadioButton;
        private System.Windows.Forms.Label label1;
    }
}