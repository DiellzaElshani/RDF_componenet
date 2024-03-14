using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace TTLadapter
{

//████████╗████████╗██╗                                    
//╚══██╔══╝╚══██╔══╝██║                                    
//   ██║      ██║   ██║                                    
//   ██║      ██║   ██║                                    
//   ██║      ██║   ███████╗                               
//   ╚═╝      ╚═╝   ╚══════╝                               
                                                         
// █████╗ ██████╗  █████╗ ██████╗ ████████╗███████╗██████╗ 
//██╔══██╗██╔══██╗██╔══██╗██╔══██╗╚══██╔══╝██╔════╝██╔══██╗
//███████║██║  ██║███████║██████╔╝   ██║   █████╗  ██████╔╝
//██╔══██║██║  ██║██╔══██║██╔═══╝    ██║   ██╔══╝  ██╔══██╗
//██║  ██║██████╔╝██║  ██║██║        ██║   ███████╗██║  ██║
//╚═╝  ╚═╝╚═════╝ ╚═╝  ╚═╝╚═╝        ╚═╝   ╚══════╝╚═╝  ╚═╝
                                                         
                                                                                                         
	public class TTLadapterComponent : GH_Component
	{
		/// <summary>
		/// Each implementation of GH_Component must provide a public 
		/// constructor without any arguments.
		/// Category represents the Tab in which the component will appear, 
		/// Subcategory the panel. If you use non-existing tab or panel names, 
		/// new tabs/panels will automatically be created.
		/// </summary>
		public TTLadapterComponent()
		  : base("TTLadapter", "TTLadapt",
			"Send RDF data from grasshopper to the web application.",
			"BHoM", "TTLadapter")
		{
			// Adding custom code when the code is created.
		}

		/// <summary>
		/// Registers all the input parameters for this component.
		/// </summary>
		protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
		{
			// Use the pManager object to register your input parameters.
			// You can often supply default values when creating parameters.
			// All parameters must have the correct access type. If you want 
			// to import lists or trees of values, modify the ParamAccess flag.

			//pManager.AddPlaneParameter("Plane", "P", "Base plane for spiral", GH_ParamAccess.item, Plane.WorldXY);
			//pManager.AddNumberParameter("Inner Radius", "R0", "Inner radius for spiral", GH_ParamAccess.item, 1.0);
			//pManager.AddNumberParameter("Outer Radius", "R1", "Outer radius for spiral", GH_ParamAccess.item, 10.0);
			//pManager.AddIntegerParameter("Turns", "T", "Number of turns between radii", GH_ParamAccess.item, 10);



			// TODO: run button 
			pManager.AddBooleanParameter("Run TTL adapter", "Run", "Set to true to run the TTL adapter", GH_ParamAccess.item, false);

			// TODO: what is RDF input type? --> "Variable Object"?
			// TODO: what is the GH_ input type?
			pManager.AddScriptVariableParameter("RDF", "RDF", "RDF as Variable Object", GH_ParamAccess.list);
			

			// If you want to change properties of certain parameters, 
			// you can use the pManager instance to access them by index:
			//pManager[0].Optional = true;
		}

		/// <summary>
		/// Registers all the output parameters for this component.
		/// </summary>
		protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
		{
			// Use the pManager object to register your output parameters.
			// Output parameters do not have default values, but they too must have the correct access type.

			//pManager.AddCurveParameter("Spiral", "S", "Spiral curve", GH_ParamAccess.item);

			// TODO: implement TTL output here
			pManager.AddTextParameter("TTL", "TTL", "TTL as Variable Object", GH_ParamAccess.item);

			// Sometimes you want to hide a specific parameter from the Rhino preview.
			// You can use the HideParameter() method as a quick way:
			//pManager.HideParameter(0);
		}

		/// <summary>
		/// This is the method that actually does the work.
		/// </summary>
		/// <param name="DA">The DA object can be used to retrieve data from input parameters and 
		/// to store data in output parameters.</param>
		protected override void SolveInstance(IGH_DataAccess DA)
		{
			// First, we need to retrieve all data from the input parameters.
			// We'll start by declaring variables and assigning them starting values.

			//Plane plane = Plane.WorldXY;
			//double radius0 = 0.0;
			//double radius1 = 0.0;
			//int turns = 0;

			//// Then we need to access the input parameters individually. 
			//// When data cannot be extracted from a parameter, we should abort this method.
			//if (!DA.GetData(0, ref plane)) return;
			//if (!DA.GetData(1, ref radius0)) return;
			//if (!DA.GetData(2, ref radius1)) return;
			//if (!DA.GetData(3, ref turns)) return;

			//// We should now validate the data and warn the user if invalid data is supplied.
			//if (radius0 < 0.0)
			//{
			//	AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Inner radius must be bigger than or equal to zero");
			//	return;
			//}
			//if (radius1 <= radius0)
			//{
			//	AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Outer radius must be bigger than the inner radius");
			//	return;
			//}
			//if (turns <= 0)
			//{
			//	AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Spiral turn count must be bigger than or equal to one");
			//	return;
			//}

			//// We're set to create the spiral now. To keep the size of the SolveInstance() method small, 
			//// The actual functionality will be in a different method:
			//Curve spiral = CreateSpiral(plane, radius0, radius1, turns);

			//// Finally assign the spiral to the output parameter.
			//DA.SetData(0, spiral);

			// Define placeholder variables
			var run = false;
			var rdf = 0;
			var ttl = 0;

			// Load values from inputs into those variables
			// DA "DATA ACCESS" retrieving data from the input and pushs to output
			// index 0 == first value of RegisterInputParams
			//assign run
			if(!DA.GetData(0, ref run)) return;
			// assign rdf data
			if(!DA.GetData(1, ref rdf)) return;

			// check if "run" triggered
			if (run)  
			{

				// TODO: PROCESSING RDF DATA TO CONVERT INTO TTL FORMAT AND MAKE A HTTPS POST




				// Check if rdf is valid
				if (!DA.GetData(1, ref rdf)) // replace with other error statement
					{
					this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No valid RDF input!");
					DA.SetData(0, ttl);
					return;
				}


				// Output
				DA.SetData(0, ttl);

			}

		}


		/// <summary>
		/// The Exposure property controls where in the panel a component icon 
		/// will appear. There are seven possible locations (primary to septenary), 
		/// each of which can be combined with the GH_Exposure.obscure flag, which 
		/// ensures the component will only be visible on panel dropdowns.
		/// </summary>
		public override GH_Exposure Exposure => GH_Exposure.primary;

		/// <summary>
		/// Provides an Icon for every component that will be visible in the User Interface.
		/// Icons need to be 24x24 pixels.
		/// You can add image files to your project resources and access them like this:
		/// return Resources.IconForThisComponent;
		/// </summary>
		protected override System.Drawing.Bitmap Icon => Properties.Resources.TTLadapter_icon_1;


		/// <summary>
		/// Each component must have a unique Guid to identify it. 
		/// It is vital this Guid doesn't change otherwise old ghx files 
		/// that use the old ID will partially fail during loading.
		/// </summary>
		public override Guid ComponentGuid => new Guid("0619b454-86e6-4881-89b2-767a8893b14c");
	}
}