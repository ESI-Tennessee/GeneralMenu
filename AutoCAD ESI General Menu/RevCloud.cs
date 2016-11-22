using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.DatabaseServices;
using System.Runtime.InteropServices;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Core.Application;
//using Autodesk.AutoCAD.ApplicationServices.Core;


namespace AutoCAD_ESI_General_Menu
{


    public class RevCloud
    {
        public static string revtri = MyPlugin.GetRoot() + @"\Blocks\General\revtri.dwg";
        
    
        public static void RevisionCloudCommand()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            //ensures the REV layer exists in the drawing, if not adds it
            GeneralMenu.CreateLayer("REV", 91, "Continuous", true);

            using (Transaction acTrans = db.TransactionManager.StartTransaction())
            {
                //finds and makes the REV layer the current layer for the drawing
                LayerTable acLyrTbl = acTrans.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                db.Clayer = acLyrTbl["REV"];

                acTrans.Commit();
            }

            //user inserts the triangle thingy
            Commands.BlockJigCmd(revtri, true);

            //get the current revision number from the title block
            string value = GetRevLetter();

            //updates the attribute for ALL triangle block references in the drawing
            if (value != null)
            {
                UpdateAttributesInRevBlock("REVTRI.DWG", "LEVEL", value);
            }
            //user inserts the revision cloud
            doc.SendStringToExecute("REVCLOUD ", true, false, true);
            

        }

        //same as the UpdateAttributesinBlock function I have in the Attribute class, but I had to copy it over here for slight changes
        //here it gets the Rev number in the title block and adds that to the attribute in the rev triangle block
        private static void UpdateAttributesInRevBlock(string blockName, string attbName, string attbValue)
        {

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            Transaction tr = doc.TransactionManager.StartTransaction();

            using (tr)
            {
                //get the current space
                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForRead);
                // Test each entity in the container...
                foreach (ObjectId entId in btr)
                {
                    Entity ent = tr.GetObject(entId, OpenMode.ForRead) as Entity;

                    if (ent != null)
                    {
                        BlockReference br = ent as BlockReference;
                        if (br != null)
                        {
                            BlockTableRecord bd = (BlockTableRecord)tr.GetObject(br.BlockTableRecord, OpenMode.ForRead);

                            // ... to see whether it's a block with
                            // the name we're after
                            if (bd.Name.ToUpper() == blockName)
                            {
                                // Check each of the attributes...
                                foreach (ObjectId arId in br.AttributeCollection)
                                {
                                    DBObject obj = tr.GetObject(arId, OpenMode.ForRead);
                                    AttributeReference ar = obj as AttributeReference;

                                    if (ar != null)
                                    {
                                        // ... to see whether it has
                                        // the tag we're after
                                        if (ar.Tag.ToUpper() == attbName)
                                        {
                                            ar.UpgradeOpen();
                                            ar.TextString = attbValue;
                                            ar.DowngradeOpen();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                tr.Commit();
            }
        }
        //draws our revision cloud
        

        //gets the revision layer from the title block attribute
        public static string GetRevLetter()
        {
            string blockName = "";

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            Transaction tr = doc.TransactionManager.StartTransaction();
            using (tr)
            {
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

                //check to see wich Title block is in the drawing
                if (bt.Has("Title-AE.dwg"))
                    blockName = "TITLE-AE.DWG";
                if (bt.Has("Title-C.dwg"))
                    blockName = "TITLE-C.DWG";
                if (bt.Has("Title-D.dwg"))
                    blockName = "TITLE-D.DWG";
                if (bt.Has("Title-B.dwg"))
                    blockName = "TITLE-B.DWG";

                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForRead);

                // Test each entity in the container...
                foreach (ObjectId entId in btr)
                {
                    Entity ent = tr.GetObject(entId, OpenMode.ForRead) as Entity;

                    if (ent != null)
                    {
                        BlockReference br = ent as BlockReference;
                        if (br != null)
                        {
                            BlockTableRecord bd = (BlockTableRecord)tr.GetObject(br.BlockTableRecord, OpenMode.ForRead);

                            // ... to see whether it's a block with
                            // the name we're after
                            if (bd.Name.ToUpper() == blockName)
                            {
                                // Check each of the attributes...
                                foreach (ObjectId arId in br.AttributeCollection)
                                {
                                    DBObject obj = tr.GetObject(arId, OpenMode.ForRead);
                                    AttributeReference ar = obj as AttributeReference;

                                    if (ar != null)
                                    {
                                        // ... to see whether it has
                                        // the tag we're after
                                        if (ar.Tag.ToUpper() == "L")
                                        {
                                            return ar.TextString;
                                        }
                                    }
                                }


                            }
                        }
                    }
                }
                tr.Commit();
            }
            return null;
        }

        //deletes objects on the paperspace that reside on the revision layer
        public static void DeleteRevisionLayer()
        {
            //we want to make sure the user really wants to delete all the revision clouds in the drawing
            System.Windows.Forms.DialogResult result1 = System.Windows.Forms.MessageBox.Show("Are you sure you want to delete all Rev Clouds?",
                "Important Question",
                System.Windows.Forms.MessageBoxButtons.YesNo);

            //if yes, proceed with the removal
            if (result1 == System.Windows.Forms.DialogResult.Yes)
            {

                // Get the current document and database, and start a transaction
                Document acDoc = Application.DocumentManager.MdiActiveDocument;
                Database acCurDb = acDoc.Database;

                using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    // Open the Block table record for read
                    BlockTable acBlkTbl;
                    acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                    // Open the Block table record paper space for write
                    BlockTableRecord acBlkTblRec;
                    acBlkTblRec = (BlockTableRecord)acTrans.GetObject(acBlkTbl[BlockTableRecord.PaperSpace], OpenMode.ForWrite);
                    int nCnt = 0;
                    acDoc.Editor.WriteMessage("\nobjects: ");

                    // Step through each object in the current space 
                    foreach (ObjectId acObjId in acBlkTblRec)
                    {
                        //get the entity of the object ID so we can access it's functions
                        Entity ent = (Entity)acTrans.GetObject(acObjId, OpenMode.ForWrite, true, true);
                        //only looking for entities on the REV layer to delete
                        if (ent.Layer == "REV")
                        {
                            //If it is a Light Weight polyline, which is what a rev cloud is made of
                            if (acObjId.ObjectClass.DxfName == "LWPOLYLINE")
                            {
                                //delete the entity
                                ent.Erase();
                            }
                            //If it is an object, which is what that triangle thing is
                            if (acObjId.ObjectClass.DxfName == "INSERT")
                            {
                                ent.Erase();
                            }
                        }
                        nCnt = nCnt + 1;
                    }

                    // If no objects are found then display a message
                    if (nCnt == 0)
                    {
                        acDoc.Editor.WriteMessage("\n No objects found");
                    }
                    acTrans.Commit();
                }
            }

        }
    }
}

