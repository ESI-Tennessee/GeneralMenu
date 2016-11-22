// (C) Copyright 2011 by Microsoft 
//
using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
//using Autodesk.AutoCAD.ApplicationServices.Core;

// This line is not mandatory, but improves loading performances
//[assembly: CommandClass(typeof(AutoCAD_ESI_General_Menu.MyCommands))]

namespace AutoCAD_ESI_General_Menu
{

    // This class is instantiated by AutoCAD for each document when
    // a command is called by the user the first time in the context
    // of a given document. In other words, non static data in this class
    // is implicitly per-document!
    public class MyCommands
    {
        
        //inserting CoverSheet
        [CommandMethod("InsertCoverSheet")]
        public void InsertCoverSheet()
        {
            CoverSheet myCoverSheet = new CoverSheet();                    
            myCoverSheet.ShowDialog();

        }
        
        //Insert just a block
        [CommandMethod("InsertLogo")]
        public void InsertLogo()
        {
            Logo myLogo = new Logo();                      
            myLogo.ShowDialog();

        }
        
        
        //inserting valves and breaking lines and shit
        [CommandMethod("VALVEBREAK")]
        public void VALVEBREAK()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            PromptStringOptions pStrOpts = new PromptStringOptions("");

            //getting string result from menu macro
            //this gets the relative file name and path from the root
            // replaced @ with \ for folder structure because you can't put a \ in an AutoCAD menu macro string
            PromptResult pStrRes = acDoc.Editor.GetString(pStrOpts); 
            string filepath = MyPlugin.GetRoot() + @"blocks\" + pStrRes.StringResult.Replace('@','\\');

            //another string result from the menu macro
            //this one will determine what layer the block will be inserted on
            pStrRes = acDoc.Editor.GetString(pStrOpts);
            string LayerName = pStrRes.StringResult;

            //checks to ensure the layer that the block is to be inserted on exists
            GeneralMenu.CreateLayer(LayerName, 7, "Continuous", false);

            InsertValveBlock.dostuff(filepath);
        }
               
        
        
        //For inserting a block defined from a menu macro
        [CommandMethod("INSBLK")]
        public void INSBLK()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            PromptStringOptions pStrOpts = new PromptStringOptions("");
            
            //getting string result from menu macro
            //this gets the relative file name and path from the root
            // replaced @ with \ for folder structure because you can't put a \ in an AutoCAD menu macro string
            PromptResult pStrRes = acDoc.Editor.GetString(pStrOpts);
            string filepath = MyPlugin.GetRoot() + @"blocks\" + pStrRes.StringResult.Replace('@','\\');

            //gets another string result from menu macro
            //this one tells what layer the block will be placed on
            pStrRes = acDoc.Editor.GetString(pStrOpts);
            string LayerName = pStrRes.StringResult;

            //runs the Jig so the user can insert the block
            Commands.BlockJigCmd(filepath, LayerName, true);
        }
        [CommandMethod("ESILeader")]
        public void ESILeader()
        {
            mleader ql = new mleader();
            ql.MyMLeaderJig();
        }

        [CommandMethod("ColumnBubble")]
        public void ColumnBubble()
        {
            AutoCAD_ESI_General_Menu.ColumnBubble.InsertColumnBubble();
        }


        [CommandMethod("ToggleTitleBlockAttribute")]
        public void ToggleTitleBlockAttribute()
        {
            Atrribute.UpdateAttribute();
        }

        [CommandMethod("insertlayer")]
        public void insertlayer()
        {
            InsertLayers.InsertLayer();
        }

        [CommandMethod("deleteLayer")]
        public void deleteLayer()
        {
            RevCloud.DeleteRevisionLayer();
        }


        [CommandMethod("bline")]
        public void bline()
        {
            BreakLine.Break();
        }


        [CommandMethod("rcloud")]
        public void rcloud()
        {
            RevCloud.RevisionCloudCommand();
            //General_Menu.RevisionCloud.CreateRevisionCloud();
        }

        [CommandMethod("lleader")]
        public void lleader()
        {
            LoopLeader ll = new LoopLeader();
            ll.MyPolyJig();

        }

        [CommandMethod("matchline")]
        public void matchline()
        {
            Application.DocumentManager.MdiActiveDocument.Database.Orthomode = true;

            MatchLine ml = new MatchLine();
            ml.CreateMatchLine();

            Application.DocumentManager.MdiActiveDocument.Database.Orthomode = false;

        }

        [CommandMethod("Drawing_Setup")]
        public void addform()
        {
            DrawingSetup dsetup = new DrawingSetup();
            //Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(dsetup);
            dsetup.ShowDialog();
        }

        //[CommandMethod("marks")]
        //public void addmarkform()
        //{
        //    //get which sub folder from the Root directory to get the subfolders from that directory and all dwg files
        //    //this sub folder is obtained from the autocad CUI file.
        //    //the macro contained in the command will pass on the sub folder
        //    Document acDoc = Application.DocumentManager.MdiActiveDocument;
        //    PromptStringOptions pStrOpts = new PromptStringOptions("");
        //    PromptResult pStrRes = acDoc.Editor.GetString(pStrOpts);



        //    Blocks bsetup = new Blocks(pStrRes.StringResult);
        //    Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(bsetup);
        //}

    }

}
