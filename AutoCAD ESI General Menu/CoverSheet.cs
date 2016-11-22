using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AutoCAD_ESI_General_Menu
{
    public partial class CoverSheet : Form
    {
        public string SelectedBorderSize;

        public CoverSheet()
        {
            InitializeComponent();
        }

        private void AERadioButton_CheckedChanged(object sender, EventArgs e)
        {
            SelectedBorderSize = "AE";
            PopulateCoverSheet();
        }

        private void BRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            SelectedBorderSize = "B";
            PopulateCoverSheet();
        }

        private void CRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            SelectedBorderSize = "C";
            PopulateCoverSheet();
        }

        private void DRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            SelectedBorderSize = "D";
            PopulateCoverSheet();
        }

        public void PopulateCoverSheet()
        {
            string FileName;
            CoverSheetlistBox.Items.Clear();

            

            string[] files = Directory.GetFiles(MyPlugin.GetRoot() + @"Blocks\Cover Sheet Logos\" + SelectedBorderSize);

           
                foreach (string file in files)
                {
                    FileName = Path.GetFileName(file);

                    //makes sure only dwg files are added to the list
                    if (Path.GetExtension(file) == ".dwg")
                    {
                        CoverSheetlistBox.Items.Add(FileName);
                    }
                }
                //CoverSheetlistBox.SetSelected(0, true);
           

        }


        private void CoverSheetlistBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CoverSheetlistBox.SelectedIndex != -1)
            {
                string filepath = MyPlugin.GetRoot() + @"Blocks\Cover Sheet Logos\" + SelectedBorderSize + @"\" + CoverSheetlistBox.SelectedItem;
                GeneralMenu.ACADIconPreview(filepath, CoverSheetPictureBox);
            }
            else
            {
                CoverSheetPictureBox.Image = null;
                CoverSheetPictureBox.Invalidate();
            }
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            string CoverSheetPath = MyPlugin.GetRoot() + @"Blocks\Cover Sheet Logos\" + SelectedBorderSize + @"\" + CoverSheetlistBox.SelectedItem;

            GeneralMenu.InsertBlockRef(new Point3d(0, 0, 0), CoverSheetPath);

            this.Close();

        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
