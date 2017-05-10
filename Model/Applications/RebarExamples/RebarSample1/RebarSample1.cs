using System;
using System.Collections;
using Tekla.Structures;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using TSMUI = Tekla.Structures.Model.UI;

namespace RebarSample1
{
    /// <summary>
    /// This is a sample of reinforcement creation with Tekla.Net API. In this sample #1 we have a static application 
    /// which creates longitudinal bars at four boundary corners and stirrups around them for selected parts. 
    /// The implementation is based on basic Tekla.Net reinforcement classes without any higher level intelligence.
    /// </summary>
    class Class1
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                Model myModel = new Model();
                ModelObjectEnumerator myEnum = new TSMUI.ModelObjectSelector().GetSelectedObjects();
                
                while(myEnum.MoveNext())
                {
                    Beam myPart = myEnum.Current as Beam;
                    if(myPart != null)
                    {
                        // first store current work plane 
                        TransformationPlane currentPlane = myModel.GetWorkPlaneHandler().GetCurrentTransformationPlane();

                        // set new work plane same as part's local coordsys
                        TransformationPlane localPlane = new TransformationPlane(myPart.GetCoordinateSystem());
                        myModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(localPlane);

                        // get solid of part to be used for rebar point calculations
                        Solid solid = myPart.GetSolid() as Solid;

                        // initialize the single rebar object to be used in longitudinal bar creation
                        SingleRebar bar = new SingleRebar();
                        bar.Father = myPart;
                        bar.Size = "20";
                        bar.Grade = "A500HW";
                        bar.OnPlaneOffsets.Add(0.0);  // please note the data type has to be 'double'
                        bar.FromPlaneOffset = 0.0;
                        bar.Name = "Longitudinal";
                        bar.Class = 7;
                        bar.EndPointOffsetType = Reinforcement.RebarOffsetTypeEnum.OFFSET_TYPE_COVER_THICKNESS;
                        bar.EndPointOffsetValue = 25.0;
                        bar.StartPointOffsetType = Reinforcement.RebarOffsetTypeEnum.OFFSET_TYPE_COVER_THICKNESS;
                        bar.StartPointOffsetValue = 25.0;

                        // create longitudinal bars at four boundary corners of the solid
                        // bar #1 at "lower left"
                        bar.Polygon.Points.Add(new Point(solid.MinimumPoint.X, solid.MinimumPoint.Y + 40, solid.MinimumPoint.Z + 40));
                        bar.Polygon.Points.Add(new Point(solid.MaximumPoint.X, solid.MinimumPoint.Y + 40, solid.MinimumPoint.Z + 40));
                        bar.Insert();
                        
                        // bar #2 at "lower right"
                        bar.Polygon.Points.Clear();
                        bar.Polygon.Points.Add(new Point(solid.MinimumPoint.X, solid.MinimumPoint.Y + 40, solid.MaximumPoint.Z - 40));
                        bar.Polygon.Points.Add(new Point(solid.MaximumPoint.X, solid.MinimumPoint.Y + 40, solid.MaximumPoint.Z - 40));
                        bar.Insert();
                        
                        // bar #3 at "upper right"
                        bar.Polygon.Points.Clear();
                        bar.Polygon.Points.Add(new Point(solid.MinimumPoint.X, solid.MaximumPoint.Y - 40, solid.MaximumPoint.Z - 40));
                        bar.Polygon.Points.Add(new Point(solid.MaximumPoint.X, solid.MaximumPoint.Y - 40, solid.MaximumPoint.Z - 40));
                        bar.Insert();
                        
                        // bar #4 at "upper left"
                        bar.Polygon.Points.Clear();
                        bar.Polygon.Points.Add(new Point(solid.MinimumPoint.X, solid.MaximumPoint.Y - 40, solid.MinimumPoint.Z + 40));
                        bar.Polygon.Points.Add(new Point(solid.MaximumPoint.X, solid.MaximumPoint.Y - 40, solid.MinimumPoint.Z + 40));
                        bar.Insert();
                        
                        // initialize the rebar group object for stirrup creation
                        RebarGroup stirrup = new RebarGroup();
                        stirrup.Father = myPart;
                        stirrup.Size = "8";
                        stirrup.RadiusValues.Add(16.0);
                        stirrup.Grade = "A500HW";
                        stirrup.OnPlaneOffsets.Add(20.0);  // please note the data type has to be 'double'
                        stirrup.FromPlaneOffset = 50;
                        stirrup.Name = "Stirrup";
                        stirrup.Class = 4;
                        stirrup.EndPointOffsetType = Reinforcement.RebarOffsetTypeEnum.OFFSET_TYPE_COVER_THICKNESS;
                        stirrup.EndPointOffsetValue = 20.0;
                        stirrup.StartPointOffsetType = Reinforcement.RebarOffsetTypeEnum.OFFSET_TYPE_COVER_THICKNESS;
                        stirrup.StartPointOffsetValue = 20.0;
                        stirrup.StartHook.Angle = 135;
                        stirrup.StartHook.Length  = 80;
                        stirrup.StartHook.Radius = 16;
                        stirrup.StartHook.Shape = RebarHookData.RebarHookShapeEnum.HOOK_90_DEGREES;
                        stirrup.EndHook.Angle = 135;
                        stirrup.EndHook.Length = 80;
                        stirrup.EndHook.Radius = 16;
                        stirrup.EndHook.Shape = RebarHookData.RebarHookShapeEnum.HOOK_90_DEGREES;
                        
                        // set group spacing
                        stirrup.Spacings.Add(250.0);
                        stirrup.SpacingType = RebarGroup.RebarGroupSpacingTypeEnum.SPACING_TYPE_TARGET_SPACE;
                        
                        // set the polygon and insert stirrup into model
                        Polygon polygon1 = new Polygon();
                        polygon1.Points.Add(new Point(solid.MinimumPoint.X, solid.MaximumPoint.Y, solid.MinimumPoint.Z));
                        polygon1.Points.Add(new Point(solid.MinimumPoint.X, solid.MaximumPoint.Y, solid.MaximumPoint.Z));
                        polygon1.Points.Add(new Point(solid.MinimumPoint.X, solid.MinimumPoint.Y, solid.MaximumPoint.Z));
                        polygon1.Points.Add(new Point(solid.MinimumPoint.X, solid.MinimumPoint.Y, solid.MinimumPoint.Z));
                        polygon1.Points.Add(new Point(solid.MinimumPoint.X, solid.MaximumPoint.Y, solid.MinimumPoint.Z));
                        Polygon polygon2 = new Polygon();
                        polygon2.Points.Add(new Point(solid.MaximumPoint.X, solid.MaximumPoint.Y, solid.MinimumPoint.Z));
                        polygon2.Points.Add(new Point(solid.MaximumPoint.X, solid.MaximumPoint.Y, solid.MaximumPoint.Z));
                        polygon2.Points.Add(new Point(solid.MaximumPoint.X, solid.MinimumPoint.Y, solid.MaximumPoint.Z));
                        polygon2.Points.Add(new Point(solid.MaximumPoint.X, solid.MinimumPoint.Y, solid.MinimumPoint.Z));
                        polygon2.Points.Add(new Point(solid.MaximumPoint.X, solid.MaximumPoint.Y, solid.MinimumPoint.Z));
                        stirrup.Polygons.Add(polygon1);
                        stirrup.Polygons.Add(polygon2);
                        stirrup.Insert();
                    
                        // remember to restore current work plane
                        myModel.GetWorkPlaneHandler().SetCurrentTransformationPlane(currentPlane);

                    }
                }
                myModel.CommitChanges();  
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.ToString());
            }

            finally
            {
            }
        }
    }
}
