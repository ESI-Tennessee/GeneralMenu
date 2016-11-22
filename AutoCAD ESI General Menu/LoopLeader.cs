using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.DatabaseServices;
//using Autodesk.AutoCAD.ApplicationServices.Core;

namespace AutoCAD_ESI_General_Menu
{
    class LoopLeader
    {
        // Maintain a list of vertices...
        // Not strictly necessary, as these will be stored in the
        // polyline, but will not adversely impact performance
        private static Point3dCollection m_pts;
        private static double angleA = 0;
        

        class PlineJig : EntityJig
        {
            // Use a separate variable for the most recent point...
            // Again, not strictly necessary, but easier to reference
            Point3d m_tempPoint;
            Plane m_plane;

            public PlineJig(Matrix3d ucs)
                : base(new Polyline())
            {

                // Create a point collection to store our vertices
                m_pts = new Point3dCollection();

                // Create a temporary plane, to help with calcs
                Point3d origin = new Point3d(0, 0, 0);

                Vector3d normal = new Vector3d(0, 0, 1);
                normal = normal.TransformBy(ucs);
                m_plane = new Plane(origin, normal);



                // Create polyline, set defaults, add dummy vertex
                Polyline pline = Entity as Polyline;

                pline.SetDatabaseDefaults();
                pline.Normal = normal;
                pline.Layer = "Dim";                
                pline.AddVertexAt(0, new Point2d(0, 0), 0, 0, 0);                

            }

            protected override SamplerStatus Sampler(JigPrompts prompts)
            {

                JigPromptPointOptions jigOpts = new JigPromptPointOptions();

                jigOpts.UserInputControls =
                  (UserInputControls.Accept3dCoordinates |
                  UserInputControls.NullResponseAccepted |
                  UserInputControls.NoNegativeResponseAccepted
                  );

                if (m_pts.Count == 0)
                {

                    // For the first vertex, just ask for the point
                    jigOpts.Message = "\nInsertion point of loop: ";
                    
                }

                else if (m_pts.Count > 0)
                {
                    // For subsequent vertices, use a base point
                    jigOpts.BasePoint = m_pts[m_pts.Count - 1];
                    jigOpts.UseBasePoint = true;
                    jigOpts.Message = "\nText Position: ";
                }
                else // should never happen
                    return SamplerStatus.Cancel;

                // Get the point itself
                PromptPointResult res = prompts.AcquirePoint(jigOpts);

                // Check if it has changed or not
                // (reduces flicker)
                if (m_tempPoint == res.Value)
                {
                    return SamplerStatus.NoChange;
                }

                else if (res.Status == PromptStatus.OK)
                {
                    m_tempPoint = res.Value;
                    return SamplerStatus.OK;
                }

                return SamplerStatus.Cancel;
            }

            protected override bool Update()
            {
                // Update the dummy vertex to be our
                // 3D point projected onto our plane
                Polyline pline = Entity as Polyline;

                pline.SetPointAt(pline.NumberOfVertices - 1, m_tempPoint.Convert2d(m_plane));

                return true;
            }

            public Entity GetEntity()
            {
                return Entity;
            }

            public void AddLatestVertex()
            {
                // Add the latest selected point to
                // our internal list...
                // This point will already be in the
                // most recently added pline vertex
                m_pts.Add(m_tempPoint);

                Polyline pline = Entity as Polyline;

                // Create a new dummy vertex...
                // can have any initial value
                pline.AddVertexAt(pline.NumberOfVertices, new Point2d(0, 0), 0, 0, 0);
                
            }

            public void ReplaceOrigin(Point3d pt)
            {
                Polyline pline = Entity as Polyline;
                //resets the first point to the end angle of the ellipse
                pline.SetPointAt(0,new Point2d(pt.X,pt.Y));

                //adds that line thing to the text 
                //if the angle is between 90 degrees and 270 degrees, the line goes to the left
                if (angleA > 1.570796327 && angleA < 4.71238898)
                {
                    pline.AddVertexAt(2, new Point2d(m_pts[1].X - .125, m_pts[1].Y), 0, 0, 0);
                }
                else   //else the line goes to the right             
                {
                    pline.AddVertexAt(2, new Point2d(m_pts[1].X + .125, m_pts[1].Y), 0, 0, 0);
                }
            }


            public void RemoveLastVertex()
            {
                
                // Let's remove our dummy vertex
                Polyline pline = Entity as Polyline;
                pline.RemoveVertexAt(m_pts.Count);
            }
        }

        public MText addText()
        {
            MText tx = new MText();
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            PromptStringOptions pStrOpts = new PromptStringOptions("\nEnter Text: ");
            pStrOpts.AllowSpaces = true;
            PromptResult pStrRes = acDoc.Editor.GetString(pStrOpts);
            tx.Contents = pStrRes.StringResult;
            tx.Layer = "DIM";

            //if the angle is between 90 degrees and 270 degrees
            if (angleA > 1.570796327 && angleA < 4.71238898)
            {
                tx.Location = new Point3d(m_pts[1].X - .25, m_pts[1].Y, 0);
                tx.Attachment = AttachmentPoint.MiddleRight;
            }
            else
            {
                tx.Location = new Point3d(m_pts[1].X + .25, m_pts[1].Y, 0);
                tx.Attachment = AttachmentPoint.MiddleLeft;
            }

            return tx;
        }

        public Ellipse InsertElipse()
        {
            
            Point2d pt1 = new Point2d(m_pts[0].X,m_pts[0].Y);
            Point2d pt2 = new Point2d(m_pts[1].X,m_pts[1].Y);

            //get the angle of the two points
            angleA = pt1.GetVectorTo(pt2).Angle;
            
            //creates our ellipse
            Ellipse el = new Ellipse(m_pts[0], Vector3d.ZAxis, new Vector3d(.0625, 0, 0), .71875, 0, 4.71238898);
            el.Layer = "DIM";
            //rotate the ellipse to the same degree of our angleA
            el.TransformBy(Matrix3d.Rotation(angleA,Vector3d.ZAxis,m_pts[0]));
            
            
            return el;
        }

        public double getDistanceBetweenTwoPoints(Point3d point1, Point3d point2)
        {
            return Math.Abs(Math.Sqrt(Math.Pow(point2.X - point1.X, 2) + Math.Pow(point2.Y - point1.Y, 2)));
        }

        
        public void MyPolyJig()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;

            Editor ed = doc.Editor;
            // Get the current UCS, to pass to the Jig
            Matrix3d ucs = ed.CurrentUserCoordinateSystem;

            //ensures that the layer the object will be created on exists, if it doesn't creates it
            GeneralMenu.CreateLayer("DIM", 1, "Continuous", true);

            // Create our Jig object
            PlineJig jig = new PlineJig(ucs);

            // Loop to set the vertices directly on the polyline
            bool bSuccess = true;
            for (int i = 0; i < 2; i++)
            {
                PromptResult res = ed.Drag(jig);
                bSuccess = (res.Status == PromptStatus.OK);

                // A new point was added
                if (bSuccess)
                    jig.AddLatestVertex();
            }            
            jig.RemoveLastVertex();
            
            
            Ellipse el = InsertElipse();

            //formula for calculating a polar point based on a base point, an angle, and the distance
            Point3d newpoint = new Point3d(el.Center.X + 0.04492187 * Math.Cos(angleA + 4.71238898), el.Center.Y + 0.04492187 * Math.Sin(angleA + 4.71238898), 0);
            jig.ReplaceOrigin(newpoint);

            MText tx = addText();


            // If the jig completed successfully, add the polyline 
            // Append entity
            Database db = doc.Database;

            Transaction tr = db.TransactionManager.StartTransaction();



            using (tr)
            {
                BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead, false);
                BlockTableRecord btr = ((BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite));
                btr.AppendEntity(jig.GetEntity());
                btr.AppendEntity(el);
                btr.AppendEntity(tx);
                tr.AddNewlyCreatedDBObject(jig.GetEntity(), true);
                tr.AddNewlyCreatedDBObject(el, true);
                tr.AddNewlyCreatedDBObject(tx, true);
                tr.Commit();
            }

        }

        

    }
}
