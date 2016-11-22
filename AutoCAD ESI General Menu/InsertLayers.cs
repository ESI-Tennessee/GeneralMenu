using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.ApplicationServices;
//using Autodesk.AutoCAD.ApplicationServices.Core;




namespace AutoCAD_ESI_General_Menu
{
    /// <summary>
    /// This class is for inserting multiple layers at a time via a text file
    /// The text file is tab delinated and has the parameters for the Layer
    /// Such as layercolor, linetype
    /// For inserting a single layer into a drawing passing the above parameters
    /// look to the "CreateLayer" method in the "GeneralMenu" Class
    /// </summary>
    class InsertLayers
    {
                
        public static void InsertLayer()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            PromptStringOptions pStrOpts = new PromptStringOptions("");
            
            //from the menu macro command gets the filename of the text file to use
            PromptResult pStrRes = acDoc.Editor.GetString(pStrOpts);

            //concatenates the full file path
            string FileName = MyPlugin.GetRoot() + @"CSV\" + pStrRes.StringResult;
            
            //gets each line in the text file and adds each line to an element in a string array
            string[] fileLines = SplitFileByLine(FileName);

            //parses each element in individual line to get the variables we need
            for (int index = 0; index < fileLines.Length; index++)
            {
                string[] items = fileLines[index].Trim().Split(',');
                if (items.Length > 1)
                {
                    string Name = items[0];
                    short Color = short.Parse(items[1]);
                    string LineType = items[2];

                    //makes sure the linetype exists 
                    GeneralMenu.LoadLinetype(LineType);

                    //creates the layer
                    GeneralMenu.CreateLayer(Name, Color, LineType, false);
                    acDoc.Editor.WriteMessage("\n " + Name + " Layer created");
                }

            }
            
        }

        //there is an easier way to do this
        public static string[] SplitFileByLine(string FileName)
        {
            StreamReader fileReader = null;
            try
            {
                // Read the contents of the entire file
                fileReader = new StreamReader(FileName);
                StringBuilder fileContents = new StringBuilder();
                char[] buffer = new char[32768];
                while (fileReader.ReadBlock(buffer, 0, buffer.Length)
                       > 0)
                {
                    fileContents.Append(buffer);
                    buffer = new char[32768];
                }
                
                // Separate the contents of the file into lines
                fileContents = fileContents.Replace("\r\n", "\n");
                return fileContents.ToString().Split('\n');
            }
            catch (Exception ex)
            {
                throw (new IOException("Unable to read " +
                                      FileName, ex));
            }
            finally
            {
                if (fileReader != null)
                {
                    fileReader.Close();
                }
            }
        }

    }
}
