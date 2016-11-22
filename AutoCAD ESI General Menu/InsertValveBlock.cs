using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Autodesk.AutoCAD.Internal;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
//using Acad = Autodesk.AutoCAD.ApplicationServices.Application;
using System.IO;
using Acad = Autodesk.AutoCAD.ApplicationServices.Core.Application;
using Autodesk.AutoCAD.ApplicationServices.Core;

namespace AutoCAD_ESI_General_Menu
{
    class InsertValveBlock
    {

        public static void dostuff(string strSourceBlockPath)
        {  

            // Get the current document and database
            Document acDoc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            Point2d ClosestPoint = new Point2d();
            
            LineSegment3d seg1 = new LineSegment3d();
            ObjectId LayerOfBreakLine;

            // Start a transaction
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Pick the line/polyline for the block to reside on
                PromptEntityResult acSSPrompt1 = acDoc.Editor.GetEntity("Pick the line");

                // If the prompt status is OK, objects were selected
                if (acSSPrompt1.Status == PromptStatus.OK)
                {
                    ObjectId obj1 = acSSPrompt1.ObjectId;

                    Entity ent1 = acTrans.GetObject(obj1, OpenMode.ForWrite) as Entity;
                    LayerOfBreakLine = ent1.LayerId;
                    switch (obj1.ObjectClass.DxfName)
                    {
                        case "LINE":
                            Line ln1 = new Line();
                            ln1 = (Line)ent1;
                            
                            //get a 2d point that is actually on the line, not just close to where the user picked
                            ClosestPoint = ln1.GetClosestPointTo(acSSPrompt1.PickedPoint, false).Add(ln1.StartPoint.GetAsVector()).Convert2d(ln1.GetPlane());

                            seg1 = new LineSegment3d(new Point3d(ln1.StartPoint.X, ln1.StartPoint.Y, ln1.StartPoint.Z), new Point3d(ln1.EndPoint.X, ln1.EndPoint.Y, ln1.EndPoint.Z));
                            
                            break;
                        case "LWPOLYLINE":
                            Polyline pln1 = new Polyline();
                            pln1 = (Polyline)ent1;

                            //loops through each segment of the polyline and finds which one our user selected point resides on
                            for (int x = 0; x < pln1.NumberOfVertices; x++)
                            {
                                //converts a 3d point to 2d point that would be on the 2d polyline
                                ClosestPoint = pln1.GetClosestPointTo(acSSPrompt1.PickedPoint, false).Add(pln1.StartPoint.GetAsVector()).Convert2d(pln1.GetPlane());
                                //then checks if that 2d point resides on the current segment being looked at
                                if (pln1.OnSegmentAt(x, ClosestPoint, 0.00))
                                {
                                    seg1 = pln1.GetLineSegmentAt(x);
                                    break;
                                }

                            }
                            break;
                    }
                    //get the angle of the two points
                    //figure out how to freaking do this with a 3d point instead of converting to 2d
                    //Line templine = new Line(seg1.StartPoint,seg1.EndPoint);
                    //gets the start and endpoint of the segment selected                            
                    Point2d pt1 = new Point2d(seg1.StartPoint.X, seg1.StartPoint.Y);
                    Point2d pt2 = new Point2d(seg1.EndPoint.X, seg1.EndPoint.Y);

                    //with those two points we can calculate the angle of the first segment
                    double angle1 = pt1.GetVectorTo(pt2).Angle;
                    double angle2 = pt2.GetVectorTo(pt1).Angle;


                    try
                    {

                        //inserts the block and returns the extents of the block BEFORE it is rotate                  
                        Extents3d ext = InsertBlockRef(new Point3d(ClosestPoint.X, ClosestPoint.Y, 0), strSourceBlockPath, angle1);
                       
                        //calculate the angle of the extent line
                        Point2d extPoint1 = new Point2d(ext.MinPoint.X, ext.MinPoint.Y);
                        Point2d extPoint2 = new Point2d(ext.MaxPoint.X, ext.MaxPoint.Y);
                        double AngleofExtents = extPoint1.GetVectorTo(extPoint2).Angle;

                        //double AngleofExtents = ext.MinPoint.Convert2d(templine.GetPlane()).GetVectorTo(ext.MaxPoint.Convert2d(templine.GetPlane())).Angle;

                        double A = getDistanceBetweenTwoPoints(ext.MinPoint, ext.MaxPoint) / 2;
                        //double width = A * Math.Cos(angle);
                        
                        //right angle thereom
                        double B = A * Math.Cos(AngleofExtents);

                        //using the centerpoint and the angle, calculate the vector of where to break the line segment
                        Point3d point1 = new Point3d(ClosestPoint.X + B * Math.Cos(angle1), ClosestPoint.Y + B * Math.Sin(angle1), 0);
                        Point3d point2 = new Point3d(ClosestPoint.X + B * Math.Cos(angle2), ClosestPoint.Y + B * Math.Sin(angle2), 0);

                        BlockTable acBlkTbl;
                        acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForWrite) as BlockTable;
                        BlockTableRecord acBlkTbleRec;
                        acBlkTbleRec = acTrans.GetObject(acCurDb.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;

                        //ready to replace the original line/polyline with two new entities
                        switch (obj1.ObjectClass.DxfName)
                        {
                            case "LINE":
                                ent1.Erase(true);

                                Line newline1 = new Line(new Point3d(seg1.StartPoint.X, seg1.StartPoint.Y, seg1.StartPoint.Z), point2);
                                Line newline2 = new Line(point1, new Point3d(seg1.EndPoint.X, seg1.EndPoint.Y, seg1.EndPoint.Z));

                                newline1.LayerId = LayerOfBreakLine;
                                newline2.LayerId = LayerOfBreakLine;

                                acBlkTbleRec.AppendEntity(newline1);
                                acBlkTbleRec.AppendEntity(newline2);
                                acTrans.AddNewlyCreatedDBObject(newline1, true);
                                acTrans.AddNewlyCreatedDBObject(newline2, true);


                                break;
                            case "LWPOLYLINE":
                                Polyline oldpl = new Polyline();
                                oldpl = (Polyline)ent1;

                                //create the first polyline on one side of the break
                                Polyline newpl1 = new Polyline();
                                newpl1.LayerId = LayerOfBreakLine;
                                for (int x = 0; seg1.EndPoint != oldpl.GetPoint3dAt(x); x++)
                                {
                                    newpl1.AddVertexAt(x, oldpl.GetPoint2dAt(x), 0, 0, 0);
                                }
                                //add end point
                                newpl1.AddVertexAt(newpl1.NumberOfVertices, new Point2d(point2.X, point2.Y), 0, 0, 0);



                                //create the second polyline at the other side of the break                                        
                                Polyline newpl2 = new Polyline();
                                newpl2.LayerId = LayerOfBreakLine;

                                int i = 0;  //for tracking the index of our new polyline
                                //start at the end of the old polyline and work our way backwards to the break point
                                for (int x = oldpl.NumberOfVertices - 1; seg1.StartPoint != oldpl.GetPoint3dAt(x); x--)
                                {
                                    newpl2.AddVertexAt(i, oldpl.GetPoint2dAt(x), 0, 0, 0);
                                    i++;
                                }
                                //add end point
                                newpl2.AddVertexAt(i, new Point2d(point1.X, point1.Y), 0, 0, 0);

                                //erases the old polyline and adds the new two polylines to the database
                                ent1.Erase(true);
                                acBlkTbleRec.AppendEntity(newpl1);
                                acBlkTbleRec.AppendEntity(newpl2);
                                acTrans.AddNewlyCreatedDBObject(newpl1, true);
                                acTrans.AddNewlyCreatedDBObject(newpl2, true);

                                break;

                            }
                        }
                        catch (Exception e)
                        {
                        }                    
                }
                acTrans.Commit(); 
            }
        }

      

        public static BlockTableRecord GetBlock(string strSourceBlockName, string strSourceBlockPath)
        {
            ObjectId oid;
            BlockTableRecord btr = null;

            Document doc = Acad.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            BlockTable bt = (BlockTable)db.BlockTableId.GetObject(OpenMode.ForRead);

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                using (Database sourcedb = new Database(false, false))
                {
                    try
                    {
                        sourcedb.ReadDwgFile(strSourceBlockPath, FileShare.Read, true, "");
                        oid = db.Insert(strSourceBlockPath, sourcedb, true);
                        btr = (BlockTableRecord)trans.GetObject(oid, OpenMode.ForWrite, true, true);
                        btr.Name = strSourceBlockName;
                        
                        trans.Commit();
                    }
                    catch (System.Exception e)
                    {
                        //something didn't go right, displays the error message
                        Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog(e.Message);
                        return null;
                    }
                }
            }
            return btr;
        }

        //returns a blockreference 
        public static Extents3d InsertBlockRef(Point3d dblInsert, string strSourceBlockPath, double angle)
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
                
                //get the extents BEFORE the block is rotated
                Extents3d ext = br.GeometricExtents;
                
                //rotates the block
                br.Rotation = angle;                

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

                return ext;
            }
        }

        public static double getDistanceBetweenTwoPoints(Point3d point1, Point3d point2)
        {
            return Math.Abs(Math.Sqrt(Math.Pow(point2.X - point1.X, 2) + Math.Pow(point2.Y - point1.Y, 2)));
        }
    }
}
