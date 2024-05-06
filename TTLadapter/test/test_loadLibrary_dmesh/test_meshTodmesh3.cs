using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Grasshopper.Kernel;
using Rhino.Geometry;

using g3;

namespace TTLadapter.test.test_loadLibrary_dmesh
{
    public class test_meshTodmesh3 : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the test_dmesh class.
        /// </summary>
        public test_meshTodmesh3()
          : base("test_meshTodmesh3", "M2DM3",
              "Converts a GH mesh to a G3 DMesh3",
			  "BHoM", "RDF_component")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("mesh", "M", "GH Mesh", GH_ParamAccess.item);
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
            Mesh mesh = null;

            DA.GetData(0, ref mesh);

            DMesh3 dm3 = ConvertMesh(mesh);

            DA.SetData(0, dm3);

        }

        protected DMesh3 ConvertMesh(Mesh mesh)
        {
            // Create empty dmesh3
            DMesh3 dm = new DMesh3(true, true, true, true);

            // Copy all vertices
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                MeshFace mf = mesh.Faces.GetFace(i);
                if (mf.IsTriangle)
                {
                    dm.AppendTriangle(mf.A, mf.B, mf.C);
                }
            }

            return dm;
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
            get { return new Guid("11E835C2-4E2F-4B1D-B091-E85147A446F3"); }
        }
    }
}