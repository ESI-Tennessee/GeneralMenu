using System;
using System.IO;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.PlottingServices;
//using Acad = Autodesk.AutoCAD.ApplicationServices.Application;
using System.Drawing;
//using Autodesk.AutoCAD.ApplicationServices.Core;
using Acad = Autodesk.AutoCAD.ApplicationServices.Core.Application;



namespace AutoCAD_ESI_General_Menu
{
    //a repository of sorts for common methods to be used throughout the project
    public class GeneralMenu
    {


        /*<summary>
        ///Inserts a drawing as an external reference overlay
        ///</summary>
        ///<param name="DrawingPath">The path of the dwg file to be XReferenced</param>
        <param name="blockname">The file name</param>*/
        public static void AttachAsOverlay(Point3d refPoint, string drawingPath)
        {
            var blockname = Path.GetFileName(drawingPath);

            var db = Acad.DocumentManager.MdiActiveDocument.Database;

            //start the transaction
            using (var tr = db.TransactionManager.StartTransaction())
            {
                var bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                //open the paper space for write
                var btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.PaperSpace], OpenMode.ForWrite);

                try
                {
                    var xrefId = db.OverlayXref(drawingPath, blockname);

                    var br = new BlockReference(refPoint, xrefId);

                    //add the reference to the paper space
                    btr.AppendEntity(br);
                    //tell the transaction about the newly added block
                    tr.AddNewlyCreatedDBObject(br, true);
                    //all ok, commit it 
                    tr.Commit();
                }

                catch (Exception e)
                {
                    //something didn't go right, displays the error message
                    Acad.ShowAlertDialog(e.Message);
                }
                //dispose of transaction when we are done
                tr.Dispose();

            }
        }

        public static BlockTableRecord GetBlock(string strSourceBlockName, string strSourceBlockPath)
        {
            BlockTableRecord btr;

            var doc = Acad.DocumentManager.MdiActiveDocument;
            var db = doc.Database;

            var bt = (BlockTable)db.BlockTableId.GetObject(OpenMode.ForRead);

            using (var trans = db.TransactionManager.StartTransaction())
            {
                using (var sourcedb = new Database(false, false))
                {
                    try
                    {
                        sourcedb.ReadDwgFile(strSourceBlockPath, FileShare.Read, true, "");
                        var oid = db.Insert(strSourceBlockPath, sourcedb, true);
                        btr = (BlockTableRecord)trans.GetObject(oid, OpenMode.ForWrite, true, true);
                        btr.Name = strSourceBlockName;
                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        //something didn't go right, displays the error message
                        Acad.ShowAlertDialog(e.Message);
                        return null;
                    }
                }
            }
            return btr;
        }

        //returns a blockreference 
        public static BlockReference InsertBlockRef(Point3d dblInsert, string strSourceBlockPath)
        {
            string strSourceBlockName = Path.GetFileName(strSourceBlockPath);

            BlockTable bt;
            BlockTableRecord btr;
            BlockReference br;
            //ObjectId id;           

            Document doc = Acad.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTableRecord btrSpace;

                //insert block
                bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForWrite, true, true);

                //if block already exists in drawing retrieve it, if not create it from external drawing
                if (bt.Has(strSourceBlockName))
                {
                    br = new BlockReference(dblInsert, bt[strSourceBlockName]);
                    ObjectId id = bt[strSourceBlockName];
                    btr = (BlockTableRecord)trans.GetObject(id, OpenMode.ForRead, true, true);

                }
                else
                {
                    BlockTableRecord btrSource = GetBlock(strSourceBlockName, strSourceBlockPath);
                    btr = (BlockTableRecord)trans.GetObject(btrSource.ObjectId, OpenMode.ForRead, true, true);

                }
                //Get the current space
                btrSpace = (BlockTableRecord)trans.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);

                //Get the Attributes from the block source and add references to them in the blockref's attribute collection
                AttributeCollection attColl;
                Entity ent;
                br = new BlockReference(dblInsert, btr.ObjectId);


                btrSpace.AppendEntity(br);
                trans.AddNewlyCreatedDBObject(br, true);
                attColl = br.AttributeCollection;

                foreach (ObjectId oid in btr)
                {
                    ent = (Entity)trans.GetObject(oid, OpenMode.ForRead, true, true);

                    if (ent.GetType() == typeof(AttributeDefinition))
                    {
                        AttributeDefinition attdef = (AttributeDefinition)ent;
                        AttributeReference attref = new AttributeReference();
                        attref.SetAttributeFromBlock(attdef, br.BlockTransform);
                        attref.TextString = attdef.TextString;
                        attColl.AppendAttribute(attref);
                        trans.AddNewlyCreatedDBObject(attref, true);
                    }
                }
                trans.Commit();

                return br;
            }
        }

        //returns a blockreference 
        public static BlockReference InsertBlockRef(Point3d dblInsert, string strSourceBlockPath, string layerName, short colorindex, string lineType)
        {
            string strSourceBlockName = Path.GetFileName(strSourceBlockPath);

            BlockTable bt;
            BlockTableRecord btr;
            BlockReference br;
            //ObjectId id;           

            Document doc = Acad.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTableRecord btrSpace;

                //insert block
                bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForWrite, true, true);

                //if block already exists in drawing retrieve it, if not create it from external drawing
                if (bt.Has(strSourceBlockName))
                {
                    br = new BlockReference(dblInsert, bt[strSourceBlockName]);
                    ObjectId id = bt[strSourceBlockName];
                    btr = (BlockTableRecord)trans.GetObject(id, OpenMode.ForRead, true, true);

                }
                else
                {
                    BlockTableRecord btrSource = GetBlock(strSourceBlockName, strSourceBlockPath);
                    btr = (BlockTableRecord)trans.GetObject(btrSource.ObjectId, OpenMode.ForRead, true, true);

                }
                //Get the current space
                btrSpace = (BlockTableRecord)trans.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);

                //Get the Attributes from the block source and add references to them in the blockref's attribute collection
                AttributeCollection attColl;
                Entity ent;
                br = new BlockReference(dblInsert, btr.ObjectId);
                CreateLayer(layerName, colorindex, lineType, false);
                br.Layer = layerName;

                btrSpace.AppendEntity(br);
                trans.AddNewlyCreatedDBObject(br, true);
                attColl = br.AttributeCollection;

                foreach (ObjectId oid in btr)
                {
                    ent = (Entity)trans.GetObject(oid, OpenMode.ForRead, true, true);

                    if (ent.GetType() == typeof(AttributeDefinition))
                    {
                        AttributeDefinition attdef = (AttributeDefinition)ent;
                        AttributeReference attref = new AttributeReference();
                        attref.SetAttributeFromBlock(attdef, br.BlockTransform);
                        attref.TextString = attdef.TextString;
                        attColl.AppendAttribute(attref);
                        trans.AddNewlyCreatedDBObject(attref, true);
                    }
                }
                trans.Commit();

                return br;
            }
        }

        public static void CreateViewport(Point3d centerPoint, double height, double width)
        {
            Database db = Acad.DocumentManager.MdiActiveDocument.Database;

            //start the transaction
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                //open the paper space for write
                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.PaperSpace], OpenMode.ForWrite);

                try
                {
                    CreateLayer("VPORT", 151, "Continuous", false);

                    Viewport vp = new Viewport
                    {
                        Layer = "VPORT",
                        CenterPoint = centerPoint,
                        Height = height,
                        Width = width
                    };

                    //add the reference to the paper space
                    btr.AppendEntity(vp);


                    //tell the transaction about the newly added block
                    tr.AddNewlyCreatedDBObject(vp, true);
                    vp.On = true;

                    //all ok, commit it
                    tr.Commit();
                }

                catch (Exception e)
                {
                    //something didn't go right, displays the error message
                    Acad.ShowAlertDialog(e.Message);
                }
                //dispose of transaction when we are done
                tr.Dispose();
            }
        }

        public static void SetAttribute(BlockReference bref, String tag, String textString)
        {
            Database db = Acad.DocumentManager.MdiActiveDocument.Database;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                tr.GetObject(bref.ObjectId, OpenMode.ForWrite);
                AttributeCollection attColl = bref.AttributeCollection;

                foreach (ObjectId attID in attColl)
                {
                    var att = (AttributeReference)attID.GetObject(OpenMode.ForWrite, true, true);
                    if (att.Tag == tag)
                        att.TextString = textString;
                }
                tr.Commit();
            }
        }

        /// <summary>
        /// Adds a layer in the current drawing
        /// </summary>
        /// <param name="layName"></param>Sets the Name of the layer
        /// <param name="colorindex"></param>Sets the color index of the layer
        /// <param name="lineType"></param>Sets the line type of the layer
        /// <param name="makeLayerCurrent"></param>Make the layer current or not
        public static void CreateLayer(string layName, short colorindex, string lineType, bool makeLayerCurrent)
        {

            Document doc = Acad.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Transaction tr = db.TransactionManager.StartTransaction();

            using (tr)
            {
                // Get the layer table from the drawing
                LayerTable lt = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);

                // Create our new layer table record...
                LayerTableRecord ltr = new LayerTableRecord();

                //checks if layer name already exists
                if (!(lt.Has(layName)))
                {
                    // ... and set its properties
                    ltr.Name = layName;

                    //assign color value to layer
                    ltr.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByAci, colorindex);

                    // Open the Layer table for read
                    LinetypeTable acLinTbl;
                    acLinTbl = tr.GetObject(db.LinetypeTableId, OpenMode.ForRead) as LinetypeTable;

                    if (acLinTbl != null && acLinTbl.Has(lineType))
                    {
                        // Upgrade the Layer Table Record for write
                        //ltr.UpgradeOpen();
                        // Set the linetype for the layer
                        ltr.LinetypeObjectId = acLinTbl[lineType];
                    }
                    else
                    {
                        //Do some exception handling IDK
                    }

                    // Add the new layer to the layer table
                    lt.UpgradeOpen();
                    lt.Add(ltr);
                    tr.AddNewlyCreatedDBObject(ltr, true);

                    // Commit the transaction
                    tr.Commit();
                }
                // Set the layer to be current for this drawing
                if (makeLayerCurrent == true)
                {
                    //lt.UpgradeOpen();
                    db.Clayer = lt[layName];
                }
            }

        }

        public static void CreateLayer(string layName, bool MakeLayerCurrent)
        {

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            Transaction tr = db.TransactionManager.StartTransaction();

            using (tr)
            {
                // Get the layer table from the drawing
                LayerTable lt = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);

                // Create our new layer table record...
                LayerTableRecord ltr = new LayerTableRecord();

                //checks if layer name already exists
                if (!(lt.Has(layName)))
                {
                    // ... and set its properties
                    ltr.Name = layName;

                    // Open the Layer table for read
                    LinetypeTable acLinTbl = tr.GetObject(db.LinetypeTableId, OpenMode.ForRead) as LinetypeTable;

                    // Add the new layer to the layer table
                    lt.UpgradeOpen();
                    ObjectId ltId = lt.Add(ltr);
                    tr.AddNewlyCreatedDBObject(ltr, true);

                    // Commit the transaction
                    tr.Commit();
                }
                // Set the layer to be current for this drawing
                if (MakeLayerCurrent == true)
                {
                    //lt.UpgradeOpen();
                    db.Clayer = lt[layName];
                }
            }

        }
        /// <summary>
        /// attempts to load the linetype from the acad.lin file. If the linetype already exists, or the file does not exist, then a message is displayed.
        /// </summary>
        /// <param name="sLineTypName"></param>The linetype name to be loaded
        public static void LoadLinetype(string sLineTypName)
        {
            // Get the current document and database
            Document acDoc = Acad.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            // Start a transaction
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Linetype table for read
                LinetypeTable acLineTypTbl;
                acLineTypTbl = acTrans.GetObject(acCurDb.LinetypeTableId,
                                                 OpenMode.ForRead) as LinetypeTable;

                if (acLineTypTbl.Has(sLineTypName) == false)
                {
                    // Load the Center Linetype
                    acCurDb.LoadLineTypeFile(sLineTypName, "acad.lin");
                }

                // Save the changes and dispose of the transaction
                acTrans.Commit();
            }
        }

        public static ObjectId GetTextstyleID(string name)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            ObjectId textstyleid = new ObjectId();

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {

                TextStyleTable tst = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;

                if (tst.Has(name))
                {
                    textstyleid = tst[name];
                }
                tr.Commit();
            }
            return textstyleid;
        }

        public static void ChangePlotSetting(string BorderSize)
        {
            string MediaName = "";
            CustomScale CmScale = new CustomScale();

            switch (BorderSize)
            {
                case "AE":
                    MediaName = "ARCH_E1_(30.00_x_42.00_Inches)";
                    CmScale = new CustomScale(1, 1.037);
                    break;
                case "B":
                    MediaName = "ANSI_expand_B_(11.00_x_17.00_Inches)";
                    CmScale = new CustomScale(1, 1.037);
                    break;
                case "C":
                    MediaName = "ARCH_expand_C_(18.00_x_24.00_Inches)";
                    CmScale = new CustomScale(1, 1.037);
                    break;
                case "D":
                    MediaName = "ARCH_expand_D_(24.00_x_36.00_Inches)";
                    CmScale = new CustomScale(1, 1);
                    break;
                case "E":
                    MediaName = "ARCH_E_(36.00_x_48.00_Inches)";
                    CmScale = new CustomScale(1, 1.032);
                    break;
                case "F":
                    MediaName = "ARCH_E1_(30.00_x_42.00_Inches)";
                    CmScale = new CustomScale(1, 1.037);
                    break;
                default:
                    MediaName = "ARCH_expand_D_(24.00_x_36.00_Inches)";
                    CmScale = new CustomScale(1, 1);
                    break;
            }

            // Get the current document and database, and start a transaction
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {

                // Reference the Layout Manager
                LayoutManager acLayoutMgr;
                acLayoutMgr = LayoutManager.Current;

                // Get the current layout and output its name in the Command Line window
                Layout acLayout;
                acLayout = acTrans.GetObject(acLayoutMgr.GetLayoutId(acLayoutMgr.CurrentLayout),
                OpenMode.ForRead) as Layout;

                // Get the PlotInfo from the layout
                PlotInfo acPlInfo = new PlotInfo();
                acPlInfo.Layout = acLayout.ObjectId;

                // Get a copy of the PlotSettings from the layout
                PlotSettings acPlSet = new PlotSettings(acLayout.ModelType);
                acPlSet.CopyFrom(acLayout);

                // Update the PlotConfigurationName property of the PlotSettings object
                PlotSettingsValidator acPlSetVdr = PlotSettingsValidator.Current;
                acPlSetVdr.SetPlotConfigurationName(acPlSet, "DWF6 ePlot.pc3", MediaName);
                acPlSetVdr.SetPlotType(acPlSet, Autodesk.AutoCAD.DatabaseServices.PlotType.Extents);

                acPlSetVdr.SetPlotCentered(acPlSet, true);
                acPlSetVdr.SetCustomPrintScale(acPlSet, CmScale);


                // Update the layout
                acLayout.UpgradeOpen();
                acLayout.CopyFrom(acPlSet);

                // Output the name of the new device assigned to the layout
                acDoc.Editor.WriteMessage("\nNew device name: " + acLayout.PlotConfigurationName);

                //regen the current view
                Application.DocumentManager.MdiActiveDocument.Editor.Regen();

                //zoom to the extents of the drawing
                acDoc.SendStringToExecute("._zoom _all ", true, false, false);

                // Save the new objects to the database
                acTrans.Commit();
            }
        }

        /// <summary>
        /// Extracts the thumbnail image from a dwg file
        /// </summary>
        /// <param name="strFile">Path and filename of dwg</param>
        /// <param name="picBox">Picture Box to display the thumbnail</param>
        public static void ACADIconPreview(string strFile, System.Windows.Forms.PictureBox picBox)
        {
            //int Num1 = 1;
            //Image imgVal1 = null;

            // Create the reader for data.
            FileStream fs = new FileStream(strFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            BinaryReader r = new BinaryReader(fs);

            // Get the image position in the DWG file
            r.BaseStream.Seek(0xd, SeekOrigin.Begin);
            Int32 imgPos = r.ReadInt32();
            r.BaseStream.Seek(imgPos, SeekOrigin.Begin);

            // Image sentinel to check if the image data

            // is not corrupted
            byte[] imgBSentinel = { 0x1f, 0x25, 0x6d, 0x7, 0xd4, 0x36, 0x28, 0x28, 0x9d, 0x57,
    0xca, 0x3f, 0x9d, 0x44, 0x10, 0x2b };

            // Read Image sentinel
            byte[] imgCSentinel = new byte[17];
            imgCSentinel = r.ReadBytes(16);

            // if image sentinel is correct
            if ((imgBSentinel.ToString() == imgCSentinel.ToString()))
            {
                // Get image size
                UInt32 imgSize = r.ReadUInt32();

                // Get number of images present
                byte imgPresent = r.ReadByte();

                // header
                Int32 imgHeaderStart = 0;
                Int32 imgHeaderSize = 0;

                // bmp data
                Int32 imgBmpStart = 0;
                Int32 imgBmpSize = 0;
                bool bmpDataPresent = false;

                // wmf data
                Int32 imgWmfStart = default(Int32);
                Int32 imgWmfSize = default(Int32);
                bool wmfDataPresent = false;

                // get each image present
                for (int I = 1; I <= imgPresent; I++)
                {
                    // Get image type

                    byte imgCode = r.ReadByte();
                    switch (imgCode)
                    {

                        case 1:
                            // Header data
                            imgHeaderStart = r.ReadInt32();
                            imgHeaderSize = r.ReadInt32();
                            break;
                        case 2:
                            // bmp data
                            imgBmpStart = r.ReadInt32();
                            imgBmpSize = r.ReadInt32();
                            bmpDataPresent = true;
                            break;
                        case 3:
                            // wmf data
                            imgWmfStart = r.ReadInt32();
                            imgWmfSize = r.ReadInt32();
                            wmfDataPresent = true;
                            break;

                    }
                }

                if ((bmpDataPresent))
                {

                    r.BaseStream.Seek(imgBmpStart, SeekOrigin.Begin);

                    byte[] tempPixelData = new byte[imgBmpSize + 15];

                    // indicate it is a bit map
                    tempPixelData[0] = 0x42;
                    tempPixelData[1] = 0x4d;

                    // offBits
                    tempPixelData[10] = 0x36;
                    tempPixelData[11] = 0x4;

                    byte[] tempBuffData = new byte[imgBmpSize + 1];

                    tempBuffData = r.ReadBytes(imgBmpSize);
                    tempBuffData.CopyTo(tempPixelData, 14);

                    MemoryStream memStream = new MemoryStream(tempPixelData);
                    Bitmap bmp = new Bitmap(memStream);
                    System.Drawing.Size szBmp = default(System.Drawing.Size);
                    szBmp.Width = picBox.Width;
                    szBmp.Height = picBox.Height;
                    Bitmap bmpResize = new Bitmap(bmp, szBmp);
                    //resize the bitmap for the picturebox


                    picBox.Image = bmpResize;
                }

                if ((wmfDataPresent))
                {
                }
                // read imgWmfSize wmf data


                byte[] imgESentinel = { 0xe0, 0xda, 0x92, 0xf8, 0x2b, 0xc9, 0xd7, 0xd7, 0x62, 0xa8,
      0x35, 0xc0, 0x62, 0xbb, 0xef, 0xd4 };
                imgCSentinel = r.ReadBytes(16);

                // if image sentinel is correct
                if ((imgESentinel.ToString() == imgCSentinel.ToString()))
                {
                }
                // Image data is not corrupted               

            }

            fs.Close();
        }

    }
}
