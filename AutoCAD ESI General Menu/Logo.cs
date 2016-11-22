using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Autodesk.AutoCAD.Geometry;

namespace AutoCAD_ESI_General_Menu
{
    public partial class Logo : Form
    {
        public string SelectedBorderSize;
        
        public Logo()
        {
            InitializeComponent();
        }

        private void AERadioButtion_CheckedChanged(object sender, EventArgs e)
        {
            SelectedBorderSize = "AE";            
            PopulateLogo();
        }

        private void CRadioButtion_CheckedChanged(object sender, EventArgs e)
        {
            SelectedBorderSize = "C";
            PopulateLogo();
        }

        private void DRadioButtion_CheckedChanged(object sender, EventArgs e)
        {
            SelectedBorderSize = "D";
            PopulateLogo();
        }

        private void BRadioButtion_CheckedChanged(object sender, EventArgs e)
        {
            SelectedBorderSize = "B";
            PopulateLogo();
        }

        public void PopulateLogo()
        {
            string FileName;
            LogoListBox.Items.Clear();
            string[] files = Directory.GetFiles(MyPlugin.GetRoot() + @"Blocks\Logos\" + SelectedBorderSize);

            foreach (string file in files)
            {
                FileName = Path.GetFileName(file);

                //makes sure only dwg files are added to the list
                if (Path.GetExtension(file) == ".dwg")
                {
                    LogoListBox.Items.Add(FileName);
                }
            }
            LogoListBox.SetSelected(0, true);
        }

        private void LogoListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string filepath = MyPlugin.GetRoot() + @"Blocks\Logos\" + SelectedBorderSize + @"\" + LogoListBox.SelectedItem;
            GeneralMenu.ACADIconPreview(filepath, LogoPictureBox);
           
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            string LogoPath = MyPlugin.GetRoot() + @"Blocks\Logos\" + SelectedBorderSize + @"\" + LogoListBox.SelectedItem;
            
            GeneralMenu.AttachAsOverlay(new Point3d(0, 0, 0), LogoPath);

            this.Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}
