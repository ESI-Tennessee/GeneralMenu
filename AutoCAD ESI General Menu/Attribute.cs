using Autodesk.AutoCAD.ApplicationServices;
//using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
namespace AutoCAD_ESI_General_Menu
{

    public class Atrribute
    {        
        public static void UpdateAttribute()
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

                //the old 2008 lisp command put the title blocks without the .dwg at the end
                //so basically I have half the drawings out there without it and this is the only easy fix I can think of
                if (bt.Has("Title-AE"))
                    blockName = "TITLE-AE";
                if (bt.Has("Title-C"))
                    blockName = "TITLE-C";
                if (bt.Has("Title-D"))
                    blockName = "TITLE-D";
                if (bt.Has("Title-B"))
                    blockName = "TITLE-B";

                tr.Commit();

            }
                       
            PromptStringOptions pso = new PromptStringOptions("");            
            pso.AllowSpaces = true;

            //gets the attribute name to change from the menu macro
            PromptResult pr = ed.GetString(pso);
            if (pr.Status != PromptStatus.OK)
                return;
            string attbName = pr.StringResult.ToUpper();
            
            //get the attribute value from the menu macro
            pr = ed.GetString(pso);
            if (pr.Status != PromptStatus.OK)
                return;
            string attbValue = pr.StringResult;

            UpdateAttributesInDatabase(db,blockName,attbName,attbValue);
        }

        public static void UpdateAttributesInDatabase(Database db,string blockName,string attbName,string attbValue)
        {

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            // Get the IDs of the spaces we want to process
            // and simply call a function to process each

            ObjectId msId, psId;

            Transaction tr = db.TransactionManager.StartTransaction();

            using (tr)
            {
                BlockTable bt =(BlockTable)tr.GetObject(db.BlockTableId,OpenMode.ForRead);

                msId = bt[BlockTableRecord.ModelSpace];
                psId = bt[BlockTableRecord.PaperSpace];

                // Not needed, but quicker than aborting
                tr.Commit();
            }
            
            UpdateAttributesInBlock(psId,blockName,attbName,attbValue);

            ed.Regen();

        }

        private static void UpdateAttributesInBlock(ObjectId btrId,string blockName,string attbName,string attbValue)
        {            

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            Transaction tr = doc.TransactionManager.StartTransaction();

            using (tr)
            {                
                BlockTableRecord btr =(BlockTableRecord)tr.GetObject( btrId,OpenMode.ForRead);
                 
                // Test each entity in the container...
                foreach (ObjectId entId in btr)
                {
                    Entity ent = tr.GetObject(entId, OpenMode.ForRead) as Entity;

                    if (ent != null)
                    {
                        BlockReference br = ent as BlockReference;
                        if (br != null)
                        {
                            BlockTableRecord bd =(BlockTableRecord)tr.GetObject(br.BlockTableRecord,OpenMode.ForRead);

                            // ... to see whether it's a block with
                            // the name we're after
                            if (bd.Name.ToUpper() == blockName)
                            {
                                // Check each of the attributes...
                                foreach (ObjectId arId in br.AttributeCollection)
                                {
                                    DBObject obj =tr.GetObject(arId,OpenMode.ForRead);
                                    AttributeReference ar = obj as AttributeReference;

                                    if (ar != null)
                                    {
                                        // ... to see whether it has
                                        // the tag we're after
                                        if (ar.Tag.ToUpper() == attbName)
                                        {
                                            //toggles the value from blank to the desired value
                                            if (ar.TextString == attbValue)
                                            {
                                                ar.UpgradeOpen();
                                                ar.TextString = "";
                                                ar.DowngradeOpen();
                                            }
                                            else
                                            {
                                                ar.UpgradeOpen();
                                                ar.TextString = attbValue;
                                                ar.DowngradeOpen();
                                            }
                                        }
                                    }
                                }
                            }
                            // Recurse for nested blocks
                            //UpdateAttributesInBlock(br.BlockTableRecord,blockName,attbName,attbValue);
                        }
                    }
                }
                tr.Commit();
            }
            
        }
    }
}
