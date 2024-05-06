using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace TTLadapter.Utils
{
    public class PluginVersion2 : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Plugin_Version class.
        /// </summary>
        public PluginVersion2()
          : base("Plugin Version 2", "Version",
              "Return RDF Component plugin version (improved)",
			  "BHoM", "RDF_component")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            // Nothing here!
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Version", "V", "Plugin version (semnatic).", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Major", "Ma", "", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Minor", "Mi", "", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Patch", "Pa", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
			// Outputs
			//var info = new TTLadapterInfo();
			//DA.SetData(0, info.Version);

			DA.SetData(0, TTLadapter.Utils.CustomPluginProperties.Version);
            DA.SetData(1, TTLadapter.Utils.CustomPluginProperties.MAJOR_VERSION);
            DA.SetData(2, TTLadapter.Utils.CustomPluginProperties.MINOR_VERSION);
            DA.SetData(3, TTLadapter.Utils.CustomPluginProperties.PATCH_VERSION);

		}

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("9bd9f7d4-98a1-4827-a5e1-21806ceb3b55"); }
        }
    }
}