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
    class ColumnBubble
    {
        public static void InsertColumnBubble()
        {
            //gets the current layer in the drawing so it can change back to it when we are done.
            //ObjectId CurrentLayer = HostApplicationServices.WorkingDatabase.Clayer;

            //gets the current document
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
           
            //turns ortho mode on
            acDoc.Database.Orthomode = true;

            //prompts user for the initial insertion point of the block
            PromptPointOptions pto = new PromptPointOptions("Insertion Point");
            PromptPointResult pPtRes = acDoc.Editor.GetPoint(pto);

            //creates point3d from user input
            Point3d pt1 = new Point3d(pPtRes.Value.X, pPtRes.Value.Y, pPtRes.Value.Z);

            //gets second point from user that will determine where to place the block
            PromptPointOptions pto2 = new PromptPointOptions("Select Orientation");
            
            //we already have the base point from our first input, only need the second
            pto2.UseBasePoint = true;
            pto2.BasePoint = pt1;

            
            PromptPointResult pPtRes2 = acDoc.Editor.GetPoint(pto2);
            Point3d pt2 = new Point3d(pPtRes2.Value.X, pPtRes2.Value.Y, pPtRes2.Value.Z);
            
            //get the angle of the two points
            double angleA = pt1.GetVectorTo(pt2).AngleOnPlane(new Plane(new Point3d (0,0,0),new Vector3d(0,0,1)));

            //new point for our final insertion point
            Point3d Final = pt1;
            
            //depending on the angle displaces the final point accordingly
            if (angleA == 0)
                Final = new Point3d(pt1.X + .1875, pt1.Y, pt1.Z);
            
            if (angleA == 1.5707963267948966)
                Final = new Point3d(pt1.X, pt1.Y + .1875, 0);

            if (angleA == 3.1415926535897931)
                Final = new Point3d(pt1.X - .1875, pt1.Y, pt1.Z);

            if (angleA == 4.71238898038469 )
                Final = new Point3d(pt1.X,pt1.Y - .1875,0);
            
            
            //calls to insert block based on user input            
            GeneralMenu.InsertBlockRef(Final, MyPlugin.GetRoot() + @"\Blocks\General\col-bubb.dwg","col-bubb", 32, "Continuous");

            //turns ortho mode back off
            acDoc.Database.Orthomode = false;
            //acDoc.Database.Clayer = CurrentLayer;

        }

        

        

    }
}
