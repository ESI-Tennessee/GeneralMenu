using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.ApplicationServices;
//using Autodesk.AutoCAD.ApplicationServices.Core;


namespace AutoCAD_ESI_General_Menu
{
    public class MatchLine
    {
        // Maintain a list of vertices...
        // Not strictly necessary, as these will be stored in the
        // polyline, but will not adversely impact performance
        private static Point3dCollection m_pts;
        
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
                
                //makes sure the layer exists then switches over to the layer               
                GeneralMenu.CreateLayer("Match", 134,"ByLayer", false);             
                pline.Layer = "Match";

                //makes sure the linetype exists then assigns the linetype
                GeneralMenu.LoadLinetype("PHANTOM");
                pline.Linetype = "PHANTOM";                
                
                pline.AddVertexAt(0, new Point2d(0, 0), 0, .0625, .0625);

                //Application.DocumentManager.MdiActiveDocument.Database.Orthomode = true;
                
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
                    jigOpts.Message = "\nStart point of polyline: ";

                }

                else if (m_pts.Count > 0)
                {
                    // For subsequent vertices, use a base point
                    jigOpts.BasePoint = m_pts[m_pts.Count - 1];

                    jigOpts.UseBasePoint = true;

                    jigOpts.Message = "\nPolyline vertex: ";
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

            public void RemoveLastVertex()
            {
                // Let's remove our dummy vertex
                Polyline pline = Entity as Polyline;
                pline.RemoveVertexAt(m_pts.Count);
            }
        }

        public void CreateMatchLine()
        {
            MyPolyJig();
            CreateText();

        }
        public void MyPolyJig()
        { 
            Document doc = Application.DocumentManager.MdiActiveDocument;

            Editor ed = doc.Editor;
            // Get the current UCS, to pass to the Jig
            Matrix3d ucs = ed.CurrentUserCoordinateSystem;

            // Create our Jig object
            PlineJig jig = new PlineJig(ucs);

            // Loop to set the vertices directly on the polyline
            bool bSuccess = true;
            for (int i = 0; i < 2 ; i++)
            {
                PromptResult res = ed.Drag(jig);
                bSuccess = (res.Status == PromptStatus.OK);

                // A new point was added
                if (bSuccess)
                    jig.AddLatestVertex();
            }                       
            jig.RemoveLastVertex();

            

            // If the jig completed successfully, add the polyline 
            // Append entity
            Database db = doc.Database;

            Transaction tr = db.TransactionManager.StartTransaction();

            using (tr)
            {
                BlockTable bt =(BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead, false);
                BlockTableRecord btr = ((BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite));
                btr.AppendEntity(jig.GetEntity());
                tr.AddNewlyCreatedDBObject(jig.GetEntity(), true);
                tr.Commit();
            }

        }

        public void CreateText()
        {
            Point2d side;
            MText text = new MText();
            double offset = .25;
            
            //temporary points for calculation where the text will be placed
            double xpoint = 0;
            double ypoint = 0;
            Point3d temp;

            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = acDoc.Editor;

            //gets user input for which side they want the text to appear in relation to the matchline
            PromptPointResult pPtRes;
            PromptPointOptions pPtOpts = new PromptPointOptions("");
            pPtOpts.Message = "\nSpecify which side: ";
            pPtRes = acDoc.Editor.GetPoint(pPtOpts);

            //stores the result in a point
            side = new Point2d(pPtRes.Value.X, pPtRes.Value.Y);

            //gets the alignment from the user
            PromptKeywordOptions pKeyOpts = new PromptKeywordOptions("");
            pKeyOpts.Message = "\nAlignment: ";
            pKeyOpts.Keywords.Add("Center");
            pKeyOpts.Keywords.Add("Left");
            pKeyOpts.Keywords.Add("Right");
            pKeyOpts.AllowNone = false;
            PromptResult pKeyRes = acDoc.Editor.GetKeywords(pKeyOpts);

            //Get drawing number from user to add to the text content
            PromptStringOptions pStOpts = new PromptStringOptions("\nEnter drawing number: ");
            PromptResult pSTRes = ed.GetString(pStOpts);

            
            //defines attributes for the Mtext
            text.Contents = "MATCH LINE:  SEE DRAWING " + pSTRes.StringResult;
            text.TextHeight = .1;
            //text.Height = .1;
            text.Layer = "Match";
            
            //if the match line is going parallel to the Y axis (I.E. up and down)
            //this determines the placement of the texted based on that
            if (m_pts[0].X == m_pts[1].X)
            {
                //the below math assumes pointcollection 0 to 1 is going down to up
                //if we started up to down as opposed to down to up
                //this swaps the positions so the math is the same either way
                if (m_pts[0].Y > m_pts[1].Y)
                {
                    temp = m_pts[0];
                    m_pts[0] = m_pts[1];
                    m_pts[1] = temp;
                }
                
                text.Rotation = 1.570796327;    //this equals 90 degrees in radians
                xpoint = m_pts[0].X;

                //offsets the text based on which side of the matchline the user picked
                if (side.X > m_pts[0].X)
                    xpoint = xpoint + offset;
                if (side.X < m_pts[0].X)
                    xpoint = xpoint - offset;

                //positions the texted based on what alignment the user inputed
                if (pKeyRes.StringResult == "Center")
                {
                    text.Attachment = AttachmentPoint.MiddleCenter;
                    ypoint = ((m_pts[1].Y - m_pts[0].Y)/2)+ m_pts[0].Y;
                }
                if (pKeyRes.StringResult == "Left")
                {
                    text.Attachment = AttachmentPoint.MiddleLeft;
                    ypoint = m_pts[0].Y;
                }
                if (pKeyRes.StringResult == "Right")
                {
                    text.Attachment = AttachmentPoint.MiddleRight;
                    ypoint = m_pts[1].Y;
                }

            }

            //if the match line is parralel to the X axis (I.E. going left to right)
            //this determines the placement of the texted based on that
            if (m_pts[0].Y == m_pts[1].Y)
            {
                //the below math assumes pointcollection 0 to 1 is going left to right
                //if we started right to left 
                //this swaps the positions so the math is the same either way
                if (m_pts[0].X > m_pts[1].X)
                {
                    temp = m_pts[0];
                    m_pts[0] = m_pts[1];
                    m_pts[1] = temp;
                }

                ypoint = m_pts[0].Y;

                //offsets the text based on which side of the matchline the user picked
                if (side.Y > m_pts[0].Y)
                    ypoint = ypoint + offset;
                if (side.Y < m_pts[0].Y)
                    ypoint = ypoint - offset;

                //positions the texted based on what alignment the user inputed
                if (pKeyRes.StringResult == "Center")
                {
                    text.Attachment = AttachmentPoint.MiddleCenter;
                    xpoint = ((m_pts[1].X - m_pts[0].X) / 2) + m_pts[0].X;
                }
                if (pKeyRes.StringResult == "Left")
                {
                    text.Attachment = AttachmentPoint.MiddleLeft;
                    xpoint = m_pts[0].X;
                }
                if (pKeyRes.StringResult == "Right")
                {
                    text.Attachment = AttachmentPoint.MiddleRight;
                    xpoint = m_pts[1].X;
                } 
            }

            text.Location = new Point3d(xpoint, ypoint, 0);

            Database db = acDoc.Database;

            Transaction tr = db.TransactionManager.StartTransaction();
            using (tr)
            {
                BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead, false);
                BlockTableRecord btr = ((BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite));
                btr.AppendEntity(text);
                tr.AddNewlyCreatedDBObject(text, true);
                tr.Commit();
            }
        }
    }
}



