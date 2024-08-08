using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace TTLadapter.test
{
    public class test_Mass_Adition : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the test class.
        /// </summary>
        public test_Mass_Adition()
          : base("test_Mass Adition", "test_Mass Adition",
              "Adds a list of numbers",
              "BHoM", "RDF_component")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Numbers", "I", "Number to add", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("R", "Result", "Result", GH_ParamAccess.item);
            pManager.AddNumberParameter("Pr", "Partial", "Partial results", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<double> vals = new List<double>();

            if (!DA.GetDataList(0, vals)) return;

            // Algorithm
            double p = 0;
            List<double> partials = new List<double>();

            foreach (double v in vals)
            {
                p += v;
                partials.Add(p);
            }

            // Outputs
            DA.SetData(0, p);
            DA.SetDataList(1, partials);
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
            get { return new Guid("1464A3A3-702F-48D0-9B33-947C0C9E1CA9"); }
        }
    }
}