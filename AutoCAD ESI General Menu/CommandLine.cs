using System;
using System.Security;
using System.Runtime.InteropServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using System.Collections;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
//using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace AutoCAD_ESI_General_Menu
{
   public static class CommandLine
   {
      const string ACAD_EXE = "acad.exe";

      const short RTSTR = 5005;
      const short RTNORM = 5100;
      const short RTNONE = 5000;
      const short RTREAL = 5001;
      const short RT3DPOINT = 5009;
      const short RTLONG = 5010;
      const short RTSHORT = 5003;
      const short RTENAME = 5006;
      const short RTPOINT = 5002;      /*2D point X and Y only */

      static Hashtable resTypes = new Hashtable();

      static CommandLine()
      {
         resTypes[typeof( string )] = RTSTR;
         resTypes[typeof( double )] = RTREAL;
         resTypes[typeof( Point3d )] = RT3DPOINT;
         resTypes[typeof( ObjectId )] = RTENAME;
         resTypes[typeof( Int32 )] = RTLONG;
         resTypes[typeof( Int16 )] = RTSHORT;
         resTypes[typeof( Point2d )] = RTPOINT;
      }

      static TypedValue TypedValueFromObject( Object val )
      {
         Type t = val.GetType();
         if( !resTypes.ContainsKey( t ) )
            throw new InvalidOperationException( "Unsupported type in Command() method" );
         return new TypedValue( (short) resTypes[t], val );
      }

      public static int Command( params object[] args )
      {
         if( AcadApp.DocumentManager.IsApplicationContext )
            return 0;
         int stat = 0;
         int cnt = 0;
         bool supressOSnap = ShouldSupressRunningOSnap();
         bool transform = ShouldTransformCoords();
         using( ResultBuffer buffer = new ResultBuffer() )
         {
            foreach( object o in args )
            {
               if( supressOSnap && ( o is Point3d || o is Point2d ) )
                  buffer.Add( new TypedValue( RTSTR, "_non" ) );
               if( transform && ( o is Point3d ) )
                  buffer.Add( new TypedValue( RT3DPOINT, WorldToCurrent( (Point3d) o ) ) );
               else
                  buffer.Add( TypedValueFromObject( o ) );
               ++cnt;
            }
            if( cnt > 0 )
            {
               string s = (string) AcadApp.GetSystemVariable( "USERS1" );
               bool debug = string.Compare( s, "DEBUG", true ) == 0;
               int val = debug ? 1 : 0;
               object cmdecho = AcadApp.GetSystemVariable( "CMDECHO" );
               Int16 c = (Int16) cmdecho;
               if( c != 0 || debug )
                  AcadApp.SetSystemVariable( "CMDECHO", val );
               stat = acedCmd( buffer.UnmanagedObject );
               if( c != 0 || debug )
                  AcadApp.SetSystemVariable( "CMDECHO", cmdecho );
            }
         }
         return stat;
      }

      /// <summary>
      ///
      /// If TransformToUcs is true and the current UCS is not the
      /// WORLD coordinate system, all Point3d arguments passed
      /// to Command() are assumed to be WCS coordinates, and will
      /// be transformed into current UCS coordinates internally,
      /// before being handed AutoCAD.
      ///
      /// Rationale:
      ///
      /// When obtaining coordinates from object data, they are usually
      /// WCS coordinates, and are not directly usable as command input
      /// without first transforming them to UCS coordinates.
      ///
      /// This property provides an easy way to automatically transform
      //  those coordinates to current UCS coordinates.
      ///
      /// One note of caution, is that when using the GetPoint() method
      /// of the editor, the coordinate returned is a UCS coordinate, so
      /// if you are passing coordinate data obtained from GetPoint(),
      /// the value of this property should always be false.
      ///
      /// </summary>

      public static bool TransformToUcs
      {
         get
         {
            return transformToUcs;
         }
         set
         {
            transformToUcs = value;
         }
      }

      /// <summary>
      /// If SupressRunningObjectSnap is true, and running object
      /// snap is effectively enabled, the string "_non" will be
      /// inserted into the command list, preceding each Point3D
      /// or Point2d argument passed to Command();
      ///
      /// Note that it is not wise to to rely on this when you
      /// are passing a large number of coordinates to the Command()
      /// method, and is preferable to just disable running object
      /// snap temporarily before calling Command() and restoring
      /// it afterward.
      ///
      /// </summary>

      public static bool SupressRunningObjectSnap
      {
         get
         {
            return supressRunningOSnap;
         }
         set
         {
            supressRunningOSnap = value;
         }
      }

      private static Point3d WorldToCurrent( Point3d worldPoint )
      {
         Point3d result = new Point3d();
         acdbWcs2Ucs( out worldPoint, out result, false );
         return result;
      }

      [SuppressUnmanagedCodeSecurity]
      [DllImport( ACAD_EXE, CallingConvention = CallingConvention.Cdecl )]
      extern static int acedCmd( IntPtr resbuf );

      [SuppressUnmanagedCodeSecurity]
      [DllImport( "acdb17.dll", CallingConvention = CallingConvention.Cdecl,
          EntryPoint = "?acdbWcs2Ucs@@YA_NQAN0_N@Z" )]
      extern static bool acdbWcs2Ucs( out Point3d inPt, out Point3d outPt, bool vec );

      private static bool ShouldSupressRunningOSnap()
      {
         if( ! supressRunningOSnap )
            return false;
         int num = (short) AcadApp.GetSystemVariable( "OSMODE" );
         return num != 0 && ( num & 16384 ) == 0;
      }

      private static bool ShouldTransformCoords()
      {
         return transformToUcs && ( 0 == (short) AcadApp.GetSystemVariable( "WORLDUCS" ) );
      }

      private static bool transformToUcs = false;
      private static bool supressRunningOSnap = false;
      private static bool debugMode = false;

      /// <summary>
      /// Not implemented yet
      /// </summary>
      public static bool DebugMode
      {
         get
         {
            return CommandLine.debugMode;
         }
         set
         {
            CommandLine.debugMode = value;
         }
      }

      // Sample member functions that use the Command() method.
      // Note that by default, coordinate parameters are assumed
      // to be UCS coordinates. If TransformToUcs is true and the
      // current UCS is not set to WORLD, then coordiantes are
      // assumed to be WCS coordinates, and will be transformed
      // to current UCS coordinates.

      public static int ZoomExtents()
      {
         return Command( "._ZOOM", "_E" );
      }

      public static int ZoomCenter( Point3d center, double height )
      {
         return Command( "._ZOOM", "_C", center, height );
      }

      public static int ZoomWindow( Point3d corner1, Point3d corner2 )
      {
         return Command( "._ZOOM", "_W", corner1, corner2 );
      }
   }

}


