using System;
using System.Collections.Generic;
using g3;
using Grasshopper.Kernel;
using Rhino.Geometry;

using TTLadapter.Utils;
using g3;
using System.Diagnostics;

namespace TTLadapter.test.test_loadLibrary_dmesh
{
    public class test_remesh : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the test_remesh class.
        /// </summary>
        public test_remesh()
          : base("test_remesh", "test_remesh",
              "Remeshes a DMesh3 object to a target edge length",
			  "BHoM", "RDF_component")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("dmesh3", "dm3", "Converted dmesh3", GH_ParamAccess.item);
            pManager.AddNumberParameter("targetEdgeLength", "L", "Approximate target length for mesh edges", GH_ParamAccess.item);
            pManager.AddIntegerParameter("passes", "P", "Number of computations passes", GH_ParamAccess.item);

		}

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
			pManager.AddGenericParameter("dmesh3", "dm3", "Converted dmesh3", GH_ParamAccess.item);
		}

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DMesh3 dm = null;
            double targetLength = 0;
            int passes = 1;

            if (!DA.GetData(0, ref dm)) return;
			if (!DA.GetData(1, ref targetLength)) return;
			if (!DA.GetData(2, ref passes)) return;

            Stopwatch timer = new Stopwatch();
            timer.Start();

            DMesh3 remeshed = RemeshToTargetEdgeLength(dm, targetLength, passes);

            timer.Stop();

            string msg = "Remeshed mesh to V:" + remeshed.VertexCount + ", F:" + remeshed.TriangleCount;
            msg += ", calculated took " + timer.ElapsedMilliseconds + " ms";

            PConsole.WriteLine(msg);

            DA.SetData(0, remeshed);
		}

        protected DMesh3 RemeshToTargetEdgeLength(DMesh3 dmesh, double targetLength, int passes)
        {
            // Copy (avoid changes to the original obj reference)
            DMesh3 dm = new DMesh3(dmesh, true, true, true, true);

            // Remesh it
            Remesher r = new Remesher(dm);
            r.PreventNormalFlips = true;
            r.SetTargetEdgeLength(targetLength);
            for (int i = 0; i < passes; i++)
            {
                r.BasicRemeshPass();
            }

            // For some reasons, the reduced mesh had invalid vertices.
            // This gets solved by re-creating a new mesh as a copy from the valid one...
            // Need to do more research as to why this is not working.
            return new DMesh3(dm, true, true, true, true);
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
            get { return new Guid("83C30317-2D76-415E-8005-A0831BC85705"); }
        }
    }
}