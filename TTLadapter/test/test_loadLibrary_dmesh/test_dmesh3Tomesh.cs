using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using g3;

namespace TTLadapter.test.test_loadLibrary_dmesh
{
    public class test_dmesh3Tomesh : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the test_dmesh2mesh class.
        /// </summary>
        public test_dmesh3Tomesh()
          : base("test_dmesh3Tomesh", "DM32M",
			  "Converts a G3 DMesh3 to a GH mesh ",
			  "BHoM", "RDF_component")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
			pManager.AddGenericParameter("dmesh3", "dm3", "Converted dmesh3", GH_ParamAccess.item);
		}

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
			pManager.AddMeshParameter("mesh", "M", "GH Mesh", GH_ParamAccess.item);
		}

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DMesh3 dm = null;
            if (!DA.GetData(0, ref dm)) return;

            Mesh m = ConvertMesh(dm);

            DA.SetData(0, m);
        }

		protected Mesh ConvertMesh(DMesh3 dm)
		{
            // Create empty rhino mesh
            Mesh m = new Mesh();

			// Copy all vertices
			for (int i = 0; i < dm.VertexCount; i++)
			{
				if (dm.IsVertex(i))
				{
                    g3.Vector3d v = dm.GetVertex(i);
                    m.Vertices.Add(v.x, v.y, v.z);
				}
			}

			// Copy all the faces
            for (int i = 0; i < dm.TriangleCount; i++)
            {
                if (dm.IsTriangle(i))
                {
                    Index3i t = dm.GetTriangle(i);
                    m.Faces.AddFace(t.a, t.b, t.c);
                }
            }

            m.RebuildNormals();

            return m;
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
            get { return new Guid("0FB130C0-92EC-4673-BA15-FD381040E702"); }
        }
    }
}