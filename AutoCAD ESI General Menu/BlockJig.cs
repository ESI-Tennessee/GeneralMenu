using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;
using System.IO;
//using Autodesk.AutoCAD.ApplicationServices.Core;

namespace AutoCAD_ESI_General_Menu
{
    class AttInfo
    {
        private Point3d _aln;
        private bool _aligned;


        public AttInfo(Point3d pos, Point3d aln, bool aligned)
        {
            Position = pos;
            _aln = aln;
            _aligned = aligned;
        }
        
        public Point3d Position { set; get; }

        public Point3d Alignment
        {
            set { _aln = value; }
            get { return _aln; }
        }

        public bool IsAligned
        {
            set { _aligned = value; }
            get { return _aligned; }
        }
    }

    class BlockJig : EntityJig
    {
        private Point3d _pos;
        private Dictionary<ObjectId, AttInfo> _attInfo;
        private Transaction _tr;

        public BlockJig(Transaction tr, BlockReference br, Dictionary<ObjectId, AttInfo> attInfo)
            : base(br)
        {
            _pos = br.Position;
            _attInfo = attInfo;
            _tr = tr;
        }

        protected override bool Update()
        {
            BlockReference br = Entity as BlockReference;
            br.Position = _pos;

            if (br.AttributeCollection.Count != 0)
            {
                foreach (ObjectId id in br.AttributeCollection)
                {
                    DBObject obj = _tr.GetObject(id, OpenMode.ForRead);
                    AttributeReference ar = obj as AttributeReference;

                    // Apply block transform to att def position
                    if (ar != null)
                    {
                        ar.UpgradeOpen();
                        AttInfo ai = _attInfo[ar.ObjectId];
                        ar.Position = ai.Position.TransformBy(br.BlockTransform);

                        if (ai.IsAligned)
                        {
                            ar.AlignmentPoint = ai.Alignment.TransformBy(br.BlockTransform);
                        }

                        if (ar.IsMTextAttribute)
                        {

                            ar.UpdateMTextAttribute();
                        }
                    }
                }
            }
            return true;
        }

        protected override SamplerStatus Sampler(JigPrompts prompts)
        {

            JigPromptPointOptions opts = new JigPromptPointOptions("\nSelect insertion point:");

            opts.BasePoint = new Point3d(0, 0, 0);
            opts.UserInputControls = UserInputControls.NoZeroResponseAccepted;

            PromptPointResult ppr = prompts.AcquirePoint(opts);
            
            if (_pos == ppr.Value)
            {
                return SamplerStatus.NoChange;
            }

            _pos = ppr.Value;
            return SamplerStatus.OK;
        }

        public PromptStatus Run()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            PromptResult promptResult = ed.Drag(this);
            return promptResult.Status;
        }
    }
    
    public class Commands
    {
        
        static public void BlockJigCmd(string strSourceBlockPath, bool displayAttEdit)
        {

            //same stuff
            Document doc =Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            BlockTableRecord btr;

            Transaction tr = doc.TransactionManager.StartTransaction();

            using (tr)
            {
                string strSourceBlockName = Path.GetFileName(strSourceBlockPath);
                
                BlockTable bt =(BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
               
                //if block already exists in drawing retrieve it, if not create if from external drawing
                if (bt.Has(strSourceBlockName))
                {
                    ObjectId id = bt[strSourceBlockName];
                    btr = (BlockTableRecord)tr.GetObject(id,OpenMode.ForRead,true,true);
                   
                }
                else
                {
                    BlockTableRecord btrSource = GeneralMenu.GetBlock(strSourceBlockName, strSourceBlockPath);
                    if (btrSource == null)
                        return;
                    btr = (BlockTableRecord)tr.GetObject(btrSource.ObjectId, OpenMode.ForRead, true, true);
                }

                BlockTableRecord space = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                
                // Block needs to be inserted to current space before
                // being able to append attribute to it
                BlockReference br = new BlockReference(new Point3d(), btr.ObjectId);
                
                space.AppendEntity(br);
                tr.AddNewlyCreatedDBObject(br, true);
                
                Dictionary<ObjectId, AttInfo> attInfo = new Dictionary<ObjectId, AttInfo>();

                if (btr.HasAttributeDefinitions)
                {
                    foreach (ObjectId id in btr)
                    {
                        DBObject obj = tr.GetObject(id, OpenMode.ForRead);
                        AttributeDefinition ad = obj as AttributeDefinition;

                        if (ad != null && !ad.Constant)
                        {
                            AttributeReference ar = new AttributeReference();
                            ar.SetAttributeFromBlock(ad, br.BlockTransform);
                            ar.Position = ad.Position.TransformBy(br.BlockTransform);

                            if (ad.Justify != AttachmentPoint.BaseLeft)
                            {
                                ar.AlignmentPoint = ad.AlignmentPoint.TransformBy(br.BlockTransform);
                            }

                            if (ar.IsMTextAttribute)
                            {
                                ar.UpdateMTextAttribute();
                            }

                            ar.TextString = ad.TextString;
                            ObjectId arId = br.AttributeCollection.AppendAttribute(ar);
                            tr.AddNewlyCreatedDBObject(ar, true);

                            // Initialize our dictionary with the ObjectId of
                            // the attribute reference + attribute definition info

                            attInfo.Add(arId, new AttInfo(ad.Position, ad.AlignmentPoint, ad.Justify != AttachmentPoint.BaseLeft));
                        }
                    }
                }
                
                // Run the jig
                BlockJig myJig = new BlockJig(tr, br, attInfo);
                
                if (myJig.Run() != PromptStatus.OK)
                    return;
                if (btr.HasAttributeDefinitions && displayAttEdit)
                {
                    doc.SendStringToExecute("_.EATTEDIT l ", true, false, true);

                    //CommandLine.Command("_.EATTEDIT", br.ObjectId);
                }
                // Commit changes if user accepted, otherwise discard
                tr.Commit();
            }
        }

        static public void BlockJigCmd(string strSourceBlockPath, string LayerName, bool displayAttEdit)
        {

            //same stuff
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            BlockTableRecord btr;

            Transaction tr = doc.TransactionManager.StartTransaction();

            using (tr)
            {
                string strSourceBlockName = Path.GetFileName(strSourceBlockPath);

                BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);

                //if block already exists in drawing retrieve it, if not create if from external drawing
                if (bt.Has(strSourceBlockName))
                {
                    ObjectId id = bt[strSourceBlockName];
                    btr = (BlockTableRecord)tr.GetObject(id, OpenMode.ForRead, true, true);

                }
                else
                {
                    BlockTableRecord btrSource = GeneralMenu.GetBlock(strSourceBlockName, strSourceBlockPath);
                    if (btrSource == null)
                        return;
                    btr = (BlockTableRecord)tr.GetObject(btrSource.ObjectId, OpenMode.ForRead, true, true);
                }

                BlockTableRecord space = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);

                // Block needs to be inserted to current space before
                // being able to append attribute to it
                BlockReference br = new BlockReference(new Point3d(), btr.ObjectId);
                
                GeneralMenu.CreateLayer(LayerName, false);
                br.Layer = LayerName;

                space.AppendEntity(br);
                tr.AddNewlyCreatedDBObject(br, true);

                Dictionary<ObjectId, AttInfo> attInfo = new Dictionary<ObjectId, AttInfo>();

                if (btr.HasAttributeDefinitions)
                {
                    foreach (ObjectId id in btr)
                    {
                        DBObject obj = tr.GetObject(id, OpenMode.ForRead);
                        AttributeDefinition ad = obj as AttributeDefinition;

                        if (ad != null && !ad.Constant)
                        {
                            AttributeReference ar = new AttributeReference();
                            ar.SetAttributeFromBlock(ad, br.BlockTransform);
                            ar.Position = ad.Position.TransformBy(br.BlockTransform);

                            if (ad.Justify != AttachmentPoint.BaseLeft)
                            {
                                ar.AlignmentPoint = ad.AlignmentPoint.TransformBy(br.BlockTransform);
                            }

                            if (ar.IsMTextAttribute)
                            {
                                ar.UpdateMTextAttribute();
                            }

                            ar.TextString = ad.TextString;
                            ObjectId arId = br.AttributeCollection.AppendAttribute(ar);
                            tr.AddNewlyCreatedDBObject(ar, true);

                            // Initialize our dictionary with the ObjectId of
                            // the attribute reference + attribute definition info

                            attInfo.Add(arId, new AttInfo(ad.Position, ad.AlignmentPoint, ad.Justify != AttachmentPoint.BaseLeft));
                        }
                    }
                }

                // Run the jig
                BlockJig myJig = new BlockJig(tr, br, attInfo);

                if (myJig.Run() != PromptStatus.OK)
                    return;
                if (btr.HasAttributeDefinitions && displayAttEdit)
                {
                    doc.SendStringToExecute("_.EATTEDIT l ", true, false, true);
                    //CommandLine.Command("_.EATTEDIT", br.ObjectId);
                }
                // Commit changes if user accepted, otherwise discard
                tr.Commit();
            }
        }
    }
}
