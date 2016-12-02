using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;
//using Autodesk.AutoCAD.ApplicationServices.Core;


namespace AutoCAD_ESI_General_Menu
{

    class mleader
    {

        class MLeaderJig : EntityJig
        {

            Point3dCollection m_pts;
            Point3d m_tempPoint;
            string m_contents;
            int m_leaderIndex;
            int m_leaderLineIndex;

            public MLeaderJig(string contents)
                : base(new MLeader())
            {

                // Store the string passed in
                m_contents = contents;

                // Create a point collection to store our vertices
                m_pts = new Point3dCollection();

                // Create mleader and set defaults
                MLeader ml = Entity as MLeader;

                ml.SetDatabaseDefaults();
                ml.Layer = "DIM";
                
                // Set up the MText contents
                ml.ContentType = ContentType.MTextContent;
                
                MText mt = new MText();
                mt.SetDatabaseDefaults();
                mt.Contents = m_contents;
                
                mt.TextStyleId = GeneralMenu.GetTextstyleId("ESI-STD");

                ml.MText = mt;
                ml.TextColor = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByAci, 32);

                //ml.TextAlignmentType = TextAlignmentType.LeftAlignment;
                //ml.TextAttachmentType = TextAttachmentType.AttachmentTopOfTop;

                //// Set the frame and landing properties
                //ml.EnableDogleg = true;
                //ml.EnableFrameText = true;
                //ml.EnableLanding = true;

                ml.SetTextAttachmentType(TextAttachmentType.AttachmentMiddleOfTop, LeaderDirectionType.LeftLeader);
                ml.SetTextAttachmentType(TextAttachmentType.AttachmentMiddleOfBottom, LeaderDirectionType.RightLeader);
                
                
                // Reduce the standard landing gap
                ml.LandingGap = .125;

                // Add a leader, but not a leader line (for now)
                m_leaderIndex = ml.AddLeader();
                m_leaderLineIndex = -1;
                
            }

            protected override SamplerStatus Sampler(JigPrompts prompts)
            {
                JigPromptPointOptions opts = new JigPromptPointOptions();

                // Not all options accept null response
                opts.UserInputControls =
                  (UserInputControls.Accept3dCoordinates |
                  UserInputControls.NoNegativeResponseAccepted
                  );

                // Get the first point
                if (m_pts.Count == 0)
                {
                    opts.UserInputControls |= UserInputControls.NullResponseAccepted;
                    opts.Message = "\nStart point of multileader: ";
                    opts.UseBasePoint = false;
                }

                // And the second
                else if (m_pts.Count == 1)
                {

                    opts.BasePoint = m_pts[m_pts.Count - 1];
                    opts.UseBasePoint = true;
                    opts.Message = "\nSpecify multileader vertex: ";

                }

                // And subsequent points
                else if (m_pts.Count > 1)
                {
                    opts.UserInputControls = UserInputControls.NullResponseAccepted;
                    opts.BasePoint = m_pts[m_pts.Count - 1];
                    opts.UseBasePoint = true;
                    opts.SetMessageAndKeywords("\nSpecify multileader vertex or [End]: ","End");
                }

                else // Should never happen
                    return SamplerStatus.Cancel;

                PromptPointResult res = prompts.AcquirePoint(opts);

                if (res.Status == PromptStatus.Keyword)
                {
                    if (res.StringResult == "End")
                    {
                        return SamplerStatus.Cancel;
                    }
                }

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
                try
                {
                    if (m_pts.Count > 0)
                    {
                        // Set the last vertex to the new value
                        MLeader ml = Entity as MLeader;
                        ml.SetLastVertex(m_leaderLineIndex, m_tempPoint);
                        
                        // Adjust the text location
                        Vector3d dogvec = ml.GetDogleg(m_leaderIndex);
                        double doglen = ml.DoglegLength;
                        double landgap = ml.LandingGap;
                        ml.TextLocation = m_tempPoint + ((doglen + landgap) * dogvec);
                    }
                }

                catch (System.Exception ex)
                {
                    Document doc = Application.DocumentManager.MdiActiveDocument;
                    doc.Editor.WriteMessage("\nException: " + ex.Message);
                    return false;
                }
                return true;
            }

            public void AddVertex()
            {
                MLeader ml = Entity as MLeader;
                // For the first point...
                if (m_pts.Count == 0)
                {
                    // Add a leader line
                    m_leaderLineIndex = ml.AddLeaderLine(m_leaderIndex);

                    // And a start vertex
                    ml.AddFirstVertex(m_leaderLineIndex, m_tempPoint);

                    // Add a second vertex that will be set within the jig
                    ml.AddLastVertex(m_leaderLineIndex, new Point3d(0, 0, 0));
                }

                else
                {
                    // For subsequent points,
                    // just add a vertex
                    ml.AddLastVertex(m_leaderLineIndex, m_tempPoint);
                }

                // Reset the attachment point, otherwise
                // it seems to get forgotten
                ml.TextAttachmentType = TextAttachmentType.AttachmentMiddle;

                // Add the latest point to our history
                m_pts.Add(m_tempPoint);
            }

            public void RemoveLastVertex()
            {
                // We don't need to actually remove
                // the vertex, just reset it
                MLeader ml = Entity as MLeader;
                
                if (m_pts.Count >= 1)
                {
                    Vector3d dogvec = ml.GetDogleg(m_leaderIndex);
                    double doglen = ml.DoglegLength;
                    double landgap = ml.LandingGap;

                    ml.TextLocation = m_pts[m_pts.Count - 1] + ((doglen + landgap) * dogvec);
                }
            }

            public Entity GetEntity()
            {
                return Entity;
            }
        }        

        public void MyMLeaderJig()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;
            string text = null;
            bool escape = false;

            // Get the text outside of the jig
            PromptStringOptions pso = new PromptStringOptions("\nEnter text: ");
            pso.AllowSpaces = true;
            PromptResult pr = ed.GetString(pso);
            text = pr.StringResult;

            do
            {
                PromptStringOptions pso2 = new PromptStringOptions("\nEnter Next Line: ");
                PromptResult pr2 = ed.GetString(pso2);
                text = text + @"\P" + pr2.StringResult;
                if (pr2.StringResult == "")
                    escape = true;
            }
            while (!escape);

            if (pr.Status == PromptStatus.OK)
            {
                // Create MleaderJig
                MLeaderJig jig = new MLeaderJig(text);
                
                // Loop to set vertices
                bool bSuccess = true;

                for (int i = 0; i < 2; i++)
                {
                    PromptResult dragres = ed.Drag(jig);
                    bSuccess = (dragres.Status == PromptStatus.OK);

                    // A new point was added
                    if (bSuccess)
                        jig.AddVertex();
                }
                jig.RemoveLastVertex();
                
                // Append entity
                Transaction tr = db.TransactionManager.StartTransaction();
                using (tr)
                {
                    BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead, false);
                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace],OpenMode.ForWrite,false);
                    btr.AppendEntity(jig.GetEntity());
                    tr.AddNewlyCreatedDBObject(jig.GetEntity(), true);
                    tr.Commit();
                }

                
            }
        }
    }
}
