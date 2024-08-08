using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using GrasshopperAsyncComponent;
using Rhino.Geometry;
using System.Windows.Forms;

namespace TTLadapter.test.test_AsynComps
{
    public class test_AsyncForLoop : GH_AsyncComponent
	{
        /// <summary>
        /// Initializes a new instance of the test_ForLoop class.
        /// </summary>
        public test_AsyncForLoop()
          : base("test_AsyncForLoop", "test_AsyncForLoop",
              "Asynch component from gh while computing a heayv for loop",
			  "BHoM", "RDF_component")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Iterations", "I", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Message", "M", "", GH_ParamAccess.item   );
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
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
            get { return new Guid("21D3C20A-F8A5-40FC-825C-51867D4AAF7C"); }
        }

		public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
		{
			base.AppendAdditionalMenuItems(menu);
            Menu_AppendItem(menu, "Cancel", (s, e) =>
            {
                RequestCancellation();
            });
		}

        public class ForLoopWorker : WorkerInstance

	}
}