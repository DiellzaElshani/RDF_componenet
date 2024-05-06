using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using TTLadapter.Utils;

namespace TTLadapter.Obselete
{
    public class PluginVersion_OBSOLETE : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Plugin_Version class.
        /// </summary>
        public PluginVersion_OBSOLETE()
          : base("Plugin Version", "Version",
              "Return RDF Component plugin version",
              "BHoM", "RDF component")
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

            DA.SetData(0, CustomPluginProperties.Version);

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
            get { return new Guid("6C576122-9367-4A56-97CC-7E962A98A442"); }
        }

        public override GH_Exposure Exposure => GH_Exposure.hidden;

    }
}