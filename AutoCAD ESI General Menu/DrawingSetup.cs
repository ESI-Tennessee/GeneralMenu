using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Geometry;
using System.Data.OleDb;


namespace AutoCAD_ESI_General_Menu
{
    public partial class DrawingSetup : Form
    {
        public string SelectedBorderSize;
        public string TitleBlock;

        public DrawingSetup()
        {
            InitializeComponent();
            
        }

        //information for the drawing setup.
        //all of the double values are for the Viewport
        //a viewport in AutoCAD has a Point3d which is the center point, and a Height and Width
        public struct BorderStruct
        {
            public string BorderName;
            public string TitleBlock;
            public double CenterX;     
            public double CenterY;
            public double CenterZ;
            public double Height;
            public double Width;
            public string BorderSize;
        }

        public void PopulateBorderSheet()
        {
            string FileName;
            BorderSheetListBox.Items.Clear();

            string[] files = Directory.GetFiles( MyPlugin.GetRoot() + @"DWGSetup\Border\" + SelectedBorderSize);
            foreach (string file in files)
            {
                FileName = Path.GetFileName(file);

                //make sure only dwg files are added to the list
                if (Path.GetExtension(file) == ".dwg")
                {
                    BorderSheetListBox.Items.Add(FileName);
                }
            }
            BorderSheetListBox.SetSelected(0, true);
        }

        public void PopulateLogo()
        {
            string FileName;
            LogoListBox.Items.Clear();
            string[] files = Directory.GetFiles( MyPlugin.GetRoot() + @"Blocks\Logos\" + SelectedBorderSize);

            //create a "no logo" for the option of not inserting a logo into a drawing
            LogoListBox.Items.Add("No Logo");

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

        public void PopulateCoverSheet()
        {
            string FileName;
            CoverSheetPictureBox.Image = null;
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

        private void AERadioButtion_CheckedChanged(object sender, EventArgs e)
        {
            SelectedBorderSize = "AE";            
            PopulateBorderSheet();
            PopulateLogo();
            PopulateCoverSheet();
        }

        private void CRadioButtion_CheckedChanged(object sender, EventArgs e)
        {
            SelectedBorderSize = "C";            
            PopulateBorderSheet();
            PopulateLogo();
            PopulateCoverSheet();
        }

        private void DRadioButtion_CheckedChanged(object sender, EventArgs e)
        {
            SelectedBorderSize = "D";            
            PopulateBorderSheet();
            PopulateLogo();
            PopulateCoverSheet();
        }

        private void BRadioButtion_CheckedChanged(object sender, EventArgs e)
        {
            SelectedBorderSize = "B";            
            PopulateBorderSheet();
            PopulateLogo();
            PopulateCoverSheet();
        }

        private void OtherRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            SelectedBorderSize = "Other";
            PopulateBorderSheet();
            PopulateLogo();
            PopulateCoverSheet();
        }

        private void LogoListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((string)LogoListBox.SelectedItem != "No Logo")
            {
                string filepath = MyPlugin.GetRoot() + @"Blocks\Logos\" + SelectedBorderSize + @"\" + LogoListBox.SelectedItem;
                GeneralMenu.ACADIconPreview(filepath, LogoPictureBox);
            }
            else
            {
                LogoPictureBox.Image = null;
                LogoPictureBox.Invalidate();
            }
        }

        private void OKButton_Click(object sender, EventArgs e)
        { 
            BorderStruct myStruct;

            //need a special case if the 'Other' checkbox is selected
            if (SelectedBorderSize == "Other")
            {
                myStruct = OtherDataConnection(BorderSheetListBox.SelectedItem.ToString());
            }
            else
            {
                myStruct = StandardSelectedBorderSize(SelectedBorderSize);                
                
            }
            string LogoPath = MyPlugin.GetRoot() + @"Blocks\Logos\" + SelectedBorderSize + @"\" + LogoListBox.SelectedItem;
            string BorderPath = MyPlugin.GetRoot() + @"DWGSetup\Border\" + SelectedBorderSize + @"\" + myStruct.BorderName;
            string TitleBlockPath = MyPlugin.GetRoot() + @"DWGSetup\TitleBlock\" + myStruct.TitleBlock;
            string CoverSheetPath = MyPlugin.GetRoot() + @"Blocks\Cover Sheet Logos\" + SelectedBorderSize + @"\" + CoverSheetlistBox.SelectedItem;


            //switches to paperspace            
            Document acDoc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;

            Autodesk.AutoCAD.ApplicationServices.Core.Application.SetSystemVariable("TILEMODE", 0);
            acDoc.Editor.SwitchToPaperSpace();

            //inserts Border
            GeneralMenu.AttachAsOverlay(new Point3d(0, 0, 0), BorderPath);

            //inserts Title Block
            if(myStruct.TitleBlock != "null")
                GeneralMenu.InsertBlockRef(new Point3d(0, 0, 0), TitleBlockPath);

            //inserts Viewport
            Point3d VPCenter = new Point3d(myStruct.CenterX, myStruct.CenterY, myStruct.CenterZ);
            GeneralMenu.CreateViewport(VPCenter, myStruct.Height, myStruct.Width);

            //inserts Logo
            if ((string)LogoListBox.SelectedItem != "No Logo")
                GeneralMenu.AttachAsOverlay(new Point3d(0, 0, 0), LogoPath);

            //inserts CoverSheet
            if ((string)CoverSheetlistBox.SelectedItem != null)
                GeneralMenu.InsertBlockRef(new Point3d(0, 0, 0), CoverSheetPath);
            
            //change drawing setup
            GeneralMenu.ChangePlotSetting(myStruct.BorderSize);

            this.Close();

        }

        /// <summary>
        /// Sets parameters for the drawing based on what Border size is selected
        /// </summary>
        /// <param name="SelectedBorderSize"></param>
        /// <returns></returns>
        public BorderStruct StandardSelectedBorderSize(string SelectedBorderSize)
        {
            BorderStruct myStruct = new BorderStruct();

            myStruct.BorderName = BorderSheetListBox.SelectedItem.ToString();
            
            switch (SelectedBorderSize)
            {
                case "B":
                    myStruct.TitleBlock = @"Title-B.dwg";
                    myStruct.CenterX = 8.75;
                    myStruct.CenterY = 6.5;
                    myStruct.CenterZ = 0;                    
                    myStruct.Height = 7.5;
                    myStruct.Width = 15;
                    myStruct.BorderSize = "B";
                    break;
                case "C":
                    myStruct.TitleBlock = @"Title-C.dwg";
                    myStruct.CenterX = 12.1673;
                    myStruct.CenterY = 10;
                    myStruct.CenterZ = 0;
                    myStruct.Height = 14.8333;
                    myStruct.Width = 22.5012;
                    myStruct.BorderSize = "C";
                    break;
                case "D":
                    myStruct.TitleBlock = @"Title-D.dwg";
                    myStruct.CenterX = 18.25;
                    myStruct.CenterY = 13.5;
                    myStruct.CenterZ = 0;
                    myStruct.Height = 19.5;
                    myStruct.Width = 34; 
                    myStruct.BorderSize = "D";
                    break;
                case "E":
                    myStruct.TitleBlock = @"Title-E.dwg";
                    myStruct.CenterX = 24.25;
                    myStruct.CenterY = 19.5;
                    myStruct.CenterZ = 0;
                    myStruct.Height = 31.5;
                    myStruct.Width = 46;
                    myStruct.BorderSize = "E";
                    break;
                case "AE":
                    myStruct.TitleBlock = @"Title-AE.dwg";
                    myStruct.CenterX = 21.25;
                    myStruct.CenterY = 16.5;
                    myStruct.CenterZ = 0;
                    myStruct.Height = 25.5;
                    myStruct.Width = 40;
                    myStruct.BorderSize = "AE";
                    break;
                    
            }
            return myStruct;
        }

        /// <summary>
        /// These are 'special cases' where there is a border that is usually not part of ESI's standard
        /// and a different parameters are set.
        /// Grabs these atypical parameters from a text file on the network 
        /// </summary>
        /// <param name="BorderBlockName"></param>
        /// <returns></returns>
        public BorderStruct OtherDataConnection(string BorderBlockName)
        {
            BorderStruct myStruct = new BorderStruct();

            string textfile = MyPlugin.GetRoot() + @"DWGSetup\OtherBorders.txt";
            string line;

            using (StreamReader file = new StreamReader(textfile))
            {
                //goes through text file one line at a time to find a match
                while ((line = file.ReadLine()) != null)
                {
                    string[] strArray = line.Split(new string[] { "\t" }, StringSplitOptions.None);

                    if (strArray[0].ToString().ToUpper() == BorderBlockName.ToUpper())
                    {
                        //found our matching line.  Populate stuff
                        myStruct.BorderName = strArray[0].ToString();
                        myStruct.TitleBlock = strArray[1].ToString();
                        double.TryParse(strArray[2].ToString(), out myStruct.CenterX);
                        double.TryParse(strArray[3].ToString(), out myStruct.CenterY);
                        double.TryParse(strArray[4].ToString(), out myStruct.CenterZ);
                        double.TryParse(strArray[5].ToString(), out myStruct.Height);
                        double.TryParse(strArray[6].ToString(), out myStruct.Width);
                        myStruct.BorderSize = strArray[7].ToString();
                        break;

                    }
                    //if it gets here did not find a match

                }
                return myStruct;
            }
        }



        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DrawingSetup_Load(object sender, EventArgs e)
        {
            this.Height = 250;

        }

        private void CoverSheetCheckBox_CheckedChanged(object sender, EventArgs e)
        {            
            if (CoverSheetCheckBox.Checked)
            {
                this.Height = 450;
                CoverSheetlistBox.Visible = true;
                CoverSheetPictureBox.Visible = true;
                label5.Visible = true;
                label6.Visible = true;
                PopulateCoverSheet();                
            }
            else
            {                 
                this.Height = 250;               
                CoverSheetPictureBox.Image = null;
                CoverSheetPictureBox.Invalidate();
                CoverSheetlistBox.Visible = false;
                CoverSheetPictureBox.Visible = false;
                label5.Visible = false;
                label6.Visible = false;                
            }
            
        }

        private void CoverSheetlistBox_SelectedIndexChanged(object sender, EventArgs e)
        {


            if (CoverSheetlistBox.SelectedItem != null)
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
    }
}
