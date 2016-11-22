using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
//using Autodesk.AutoCAD.ApplicationServices.Core;

namespace AutoCAD_ESI_General_Menu
{
    class BreakLine
    {  
        public static void Break()
        {
            ObjectId LayerOfBreakLine;
            
            
            // Get the current document and database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            
            LineSegment3d seg1 = new LineSegment3d();
            LineSegment3d seg2 = new LineSegment3d();

            // Start a transaction
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Pick the line/polyline that will be broke
                PromptEntityResult acSSPrompt1 = acDoc.Editor.GetEntity("Pick the line to break");

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
                            
                            seg1 = new LineSegment3d(new Point3d(ln1.StartPoint.X, ln1.StartPoint.Y, ln1.StartPoint.Z), new Point3d(ln1.EndPoint.X, ln1.EndPoint.Y, ln1.EndPoint.Z));
                            
                            break;
                        case "LWPOLYLINE":
                            Polyline pln1 = new Polyline();
                            pln1 = (Polyline)ent1;

                            //loops through each segment of the polyline and finds which one our user selected point resides on
                            for (int x = 0; x < pln1.NumberOfVertices; x++)
                            {
                                //converts a 3d point to 2d point that would be on the 2d polyline
                                Point2d temppt = pln1.GetClosestPointTo(acSSPrompt1.PickedPoint, false).Add(pln1.StartPoint.GetAsVector()).Convert2d(pln1.GetPlane());
                                //then checks if that 2d point resides on the current segment being looked at
                                if (pln1.OnSegmentAt(x, temppt, 0.00))
                                {                                    
                                    seg1 = pln1.GetLineSegmentAt(x);                                    
                                    break;
                                }

                            }
                            break;
                    }
                    // Pick the line/polyline that intersects with the line to be broke
                    PromptEntityResult acSSPrompt2 = acDoc.Editor.GetEntity("Pick the intersecting line");

                    // If the prompt status is OK, objects were selected
                    if (acSSPrompt2.Status == PromptStatus.OK)
                    {
                        ObjectId obj2 = acSSPrompt2.ObjectId;

                        Entity ent2 = acTrans.GetObject(obj2, OpenMode.ForWrite) as Entity;
                        
                        switch (obj2.ObjectClass.DxfName)
                        {
                            case "LINE":
                                Line ln2 = new Line();
                                ln2 = (Line)ent2;
                                
                                seg2 = new LineSegment3d(new Point3d(ln2.StartPoint.X, ln2.StartPoint.Y, ln2.StartPoint.Z), new Point3d(ln2.EndPoint.X, ln2.EndPoint.Y, ln2.EndPoint.Z));
                                
                                break;
                            case "LWPOLYLINE":
                                Polyline pln2 = new Polyline();
                                pln2 = (Polyline)ent2;

                                for (int x = 0; x < pln2.NumberOfVertices; x++)
                                {                                    
                                    Point2d temppt = pln2.GetClosestPointTo(acSSPrompt2.PickedPoint, false).Add(pln2.StartPoint.GetAsVector()).Convert2d(pln2.GetPlane());
                                    if (pln2.OnSegmentAt(x, temppt, 0.00))
                                    {                                        
                                        seg2 = pln2.GetLineSegmentAt(x);
                                        break;
                                    }
                                   
                                }
                                break;


                        }
                        //checks if the line segments actually intersect
                        //also checks if the line segment are the same
                        //both cases would cause AutoCAD to crash if the code was allowed to run
                        if (seg1.IntersectWith(seg2) != null || (seg1.Equals(seg2)))
                        {
                            //gets the start and endpoint of the segment selected                            
                            Point2d pt1 = new Point2d(seg1.StartPoint.X, seg1.StartPoint.Y);
                            Point2d pt2 = new Point2d(seg1.EndPoint.X, seg1.EndPoint.Y);

                            //with those two points we can calculate the angle of the first segment
                            double angle1 = pt1.GetVectorTo(pt2).Angle;
                            double angle2 = pt2.GetVectorTo(pt1).Angle;

                            //get the point where the two segments intersect
                            Point3d[] intersectpoint = seg1.IntersectWith(seg2);
                            Point3d center = intersectpoint[0];

                            //using the centerpoint and the angle, calculate the vector of where to break the line segment
                            Point3d point1 = new Point3d(center.X + .125 * Math.Cos(angle1), center.Y + .125 * Math.Sin(angle1), 0);
                            Point3d point2 = new Point3d(center.X + .125 * Math.Cos(angle2), center.Y + .125 * Math.Sin(angle2), 0);
                            
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
                                    newpl1.AddVertexAt(newpl1.NumberOfVertices, new Point2d(point2.X,point2.Y), 0, 0, 0);

                                    
                                
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
                                    newpl2.AddVertexAt(i, new Point2d(point1.X,point1.Y), 0, 0, 0);

                                    //erases the old polyline and adds the new two polylines to the database
                                    ent1.Erase(true);
                                    acBlkTbleRec.AppendEntity(newpl1);
                                    acBlkTbleRec.AppendEntity(newpl2);
                                    acTrans.AddNewlyCreatedDBObject(newpl1, true);
                                    acTrans.AddNewlyCreatedDBObject(newpl2, true);

                                    break;

                            }
                            //commit everything
                            acTrans.Commit();

                        }
                    } 
                }
            }
        } 
    }
}
