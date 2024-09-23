using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Grasshopper.Kernel.Data;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Net.Mime;

namespace TTLAdapter
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
                                                         
                                                                                                         
	public class TTLAdapterComponent : GH_Component
	{

		public TTLAdapterComponent()
		  : base("TTLadapter", "TTLadapt",
			"Send RDF as TTL data from Grasshopper to a web application.",
			"BHoM", "RDF")
		{
		}

		protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
		{
			// url
			pManager.AddTextParameter("URL", "URL", "Set URL", GH_ParamAccess.item, "http://localhost:5000");
			// RDF / ttl
			pManager.AddTextParameter("TTL", "TTL", "Path to TTL file", GH_ParamAccess.item);
			//pManager.AddGenericParameter("RDF as TTL", "TTL", "RDF as TTL file", GH_ParamAccess.item);

			// timeout
			pManager.AddIntegerParameter("Timeout", "Timeout", "Timeout for HTTPS POST request", GH_ParamAccess.item, 60000);
			// run button 
			pManager.AddBooleanParameter("Send", "Send", "If true, it sends the TTL file to the URL", GH_ParamAccess.item, false);
		}

		protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
		{
			// Status
			pManager.AddTextParameter("Status", "Status", "Status of the POST request", GH_ParamAccess.item);
		}

		protected override void SolveInstance(IGH_DataAccess DA)
		{
			//GH_Structure<IGH_Goo> T = new GH_Structure<IGH_Goo>();
			//if (!DA.GetData(1, ref T)) return;

			// Define placeholder variables
			bool active = false;
			string url = "";
			int timeout = 0;
			string ttlPath = "";
			string status = "";

			//object ttl = null;	

			if (!DA.GetData("Send", ref active) || !active)
			{
				DA.SetData("Status", "Inactive");
				return;
			}

			// assign url
			if (!DA.GetData("URL", ref url)) return;
			// assign timeout
			if (!DA.GetData("Timeout", ref timeout)) return;
			// assign ttl data
			if (!DA.GetData("TTL", ref ttlPath)) return;

			// Validity checks
			if (url == null || url.Length == 0)
			{
				AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Empty URL");
				return;
			}

			if (!DA.GetData("TTL", ref ttlPath) || string.IsNullOrEmpty(ttlPath))
			{
				AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid or empty TTL file path.");
				DA.SetData("Status", "Invalid or empty TTL file path");
				return;
			}

			if (!File.Exists(ttlPath))
			{
				AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "TTL file not found at specified path.");
				DA.SetData("Status", "TTL file not found");
				return;
			}

			try
			{
				// Read the TTL file content
				string ttlContent = File.ReadAllText(ttlPath);
				byte[] byteArray = Encoding.UTF8.GetBytes(ttlContent);

				// Create the HttpWebRequest
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
				request.Method = "POST";
				request.ContentType = "text/turtle"; // "application /json";
				request.ContentLength = byteArray.Length;
				request.AuthenticationLevel  // const AUTH = 'Basic ' + Buffer.from('admin:admin').toString('base64');
				request.Timeout = timeout;
				//request.Credentials = CredentialCache.DefaultCredentials;

				// Write the content to the request stream
				using (Stream dataStream = request.GetRequestStream())
				{
					dataStream.Write(byteArray, 0, byteArray.Length);
				}

				// Get the response
				using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
				{
					if (response.StatusCode >= HttpStatusCode.OK && response.StatusCode < HttpStatusCode.Ambiguous)
					{
						status = "Success: " + response.StatusCode;
					}
					else
					{
						status = "Failed: " + response.StatusCode;
					}
				}
				//var res = request.GetResponse();
				//response = new StreamReader(res.GetResponseStream()).ReadToEnd();
			}

			catch (WebException ex)
			{
				if (ex.Response != null)
				{
					using (var errorResponse = (HttpWebResponse)ex.Response)
					{
						status = "Server Error: " + errorResponse.StatusCode;
					}
				}
				else
				{
					status = "Request Error: " + ex.Message;
				}
			}
			catch (Exception ex)
			{
				status = "Unexpected Error: " + ex.Message;
			}
			//{
			//	AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Something went wrong: " + ex.Message);
			//	return;
			//}

			// Output
			DA.SetData("Status", status);
		}


		public override GH_Exposure Exposure => GH_Exposure.primary;

		protected override System.Drawing.Bitmap Icon => Properties.Resources.TTLadapter_icon_1;

		public override Guid ComponentGuid => new Guid("d2410be1-9461-4097-9428-0f29c478bb41");
	}
}