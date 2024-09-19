using Amazon.ElasticBeanstalk.Model;
using Amazon.ElasticLoadBalancing.Model;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using static System.Net.WebRequestMethods;

namespace GraphWebsite
{

	// ██████╗ ██████╗  █████╗ ██████╗ ██╗  ██╗             
	//██╔════╝ ██╔══██╗██╔══██╗██╔══██╗██║  ██║             
	//██║  ███╗██████╔╝███████║██████╔╝███████║             
	//██║   ██║██╔══██╗██╔══██║██╔═══╝ ██╔══██║             
	//╚██████╔╝██║  ██║██║  ██║██║     ██║  ██║             
	// ╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝     ╚═╝  ╚═╝             

	//██╗    ██╗███████╗██████╗ ███████╗██╗████████╗███████╗
	//██║    ██║██╔════╝██╔══██╗██╔════╝██║╚══██╔══╝██╔════╝
	//██║ █╗ ██║█████╗  ██████╔╝███████╗██║   ██║   █████╗  
	//██║███╗██║██╔══╝  ██╔══██╗╚════██║██║   ██║   ██╔══╝  
	//╚███╔███╔╝███████╗██████╔╝███████║██║   ██║   ███████╗
	// ╚══╝╚══╝ ╚══════╝╚═════╝ ╚══════╝╚═╝   ╚═╝   ╚══════╝


	public class GraphWebsiteComponent : GH_Component
	{

		/// <summary>
		/// Each implementation of GH_Component must provide a public 
		/// constructor without any arguments.
		/// Category represents the Tab in which the component will appear, 
		/// Subcategory the panel. If you use non-existing tab or panel names, 
		/// new tabs/panels will automatically be created.
		/// </summary>
		public GraphWebsiteComponent()
		  : base("GraphWebsite", "GraphWeb",
			"Run graph website on localhost for representing custom RDF.",
			"BHoM", "RDF")
		{
		}

		protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
		{
			// Use the pManager object to register your input parameters.
			// You can often supply default values when creating parameters.
			// All parameters must have the correct access type. If you want 
			// to import lists or trees of values, modify the ParamAccess flag.

			// open website
			pManager.AddBooleanParameter("Open", "Open", "Open website in default browser with defined localhost port number", GH_ParamAccess.item, false);
			// run button
			pManager.AddBooleanParameter("Run", "Run", "If true, start running the server of graph website on localhost at default webbrowser.", GH_ParamAccess.item, false);
			;           // localhost port
						//pManager.AddIntegerParameter("Port", "Port", "Define port of http://localhost:", GH_ParamAccess.item, 8000);


			// If you want to change properties of certain parameters, 
			// you can use the pManager instance to access them by index:
			//pManager[0].Optional = true;
		}

		protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
		{
			// Use the pManager object to register your output parameters.
			// Output parameters do not have default values, but they too must have the correct access type.

			// output
			pManager.AddTextParameter("Status", "Status", "Status of localhost graph website", GH_ParamAccess.item);

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

			bool open = false;
			bool run = false;
			int port = 8000;
			HttpListener listener = null;


			// Then we need to access the input parameters individually. 
			// When data cannot be extracted from a parameter, we should abort this method.


			// assign open browser
			if (!DA.GetData("Open", ref open)) return;

			// assign run
			//if (!DA.GetData("Run", ref run) || !run)
			//{
			//	DA.SetData("Status", "Inactive");
			//	return;
			//}

			// assign port
			//if (!DA.GetData("Port", ref port)) return;

			// We should now validate the data and warn the user if invalid data is supplied.
			// Validity checks
			if (port > 10000 || port < 0)
			{
				AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid port number ");
				DA.SetData("Status", "Invalid port number");
				return;
			}


			// We're set to create the website now. To keep the size of the SolveInstance() method small, 
			// The actual functionality will be in a different method:

			if (open)
			{
				try
				{
					Process.Start($"http://localhost:{port}");
				}

				catch (Exception e)
				{
					DA.SetData("Status", e.Message.ToString());
				}
			}


			//RunScript(port, run, listener);
			//DA.SetData("Status", currentStatus);


			// Finally assign the spiral to the output parameter.
			//DA.SetData(0, spiral);
		}

		//public void RunScript(int port, bool run, HttpListener listener)
		//{
		//	// Get the current directory
		//	string currentDirectory = System.IO.Directory.GetCurrentDirectory();

		//	// Directory of the graph website
		//	string folderPath = Path.Combine(currentDirectory, "Website");

		//	if (!Directory.Exists(folderPath))
		//	{
		//		string currentStatus = ($"Folder does not exist: {folderPath}");
		//		return;
		//	}
		//}

		//	if (!run)
		//	{
		//		run = true;

		//		// Run the server in a new thread
		//		Thread serverThread = new Thread(() => StartHttpServer(port, folderPath));
		//		serverThread.IsBackground = true;
		//		serverThread.Start();

		//		// Open the default web browser and navigate to localhost
		//		Process.Start(new ProcessStartInfo("http://localhost:" + port)
		//		{
		//			UseShellExecute = true
		//		});
		//	}
		//	else
		//	{
		//		string currentStatus = ($"Server already running at: http://localhost:{port}");
		//	}
		//}

		//void StartHttpServer(int port, string folderPath, HttpListener listener)
		//{
		//	listener = new HttpListener();
		//	listener.Prefixes.Add($"http://localhost:{port}/");
		//	listener.Start();
		//	//Print($"Listening at: http://localhost:{port}/");


		//	while (serverRunning)
		//	{
		//		try
		//		{
		//			// Wait for an incoming request
		//			HttpListenerContext context = listener.GetContext();
		//			HttpListenerRequest request = context.Request;
		//			HttpListenerResponse response = context.Response;

		//			// Serve files based on the request URL
		//			string urlPath = request.Url.LocalPath.TrimStart('/');
		//			string filePath = Path.Combine(folderPath, urlPath);

		//			if (string.IsNullOrEmpty(urlPath)) // If no specific file requested, serve index.html
		//			{
		//				filePath = Path.Combine(folderPath, "index.html");
		//			}

		//			if (File.Exists(filePath))
		//			{
		//				string mimeType = GetMimeType(filePath);
		//				byte[] buffer = File.ReadAllBytes(filePath);
		//				response.ContentType = mimeType;
		//				response.ContentLength64 = buffer.Length;
		//				response.OutputStream.Write(buffer, 0, buffer.Length);
		//			}
		//			else
		//			{
		//				// File not found, return 404
		//				response.StatusCode = (int)HttpStatusCode.NotFound;
		//				byte[] buffer = Encoding.UTF8.GetBytes("404 - File not found");
		//				response.ContentLength64 = buffer.Length;
		//				response.OutputStream.Write(buffer, 0, buffer.Length);
		//			}

		//			response.OutputStream.Close();
		//		}
		//		catch (Exception ex)
		//		{
		//			("Error: " + ex.Message);
		//		}
		//	}
		//}

		//// Function to map file extensions to MIME types
		//string GetMimeType(string filePath)
		//{
		//	string extension = Path.GetExtension(filePath).ToLower();

		//	switch (extension)
		//	{
		//		case ".html":
		//		case ".htm":
		//			return "text/html";
		//		case ".js":
		//			return "application/javascript";
		//		case ".css":
		//			return "text/css";
		//		case ".png":
		//			return "image/png";
		//		case ".jpg":
		//		case ".jpeg":
		//			return "image/jpeg";
		//		case ".gif":
		//			return "image/gif";
		//		case ".svg":
		//			return "image/svg+xml";
		//		default:
		//			return "application/octet-stream"; // Default binary type for unknown extensions
		//	}
		//}

		//void StopHttpServer()
		//{
		//	if (listener != null && listener.IsListening)
		//	{
		//		listener.Stop();
		//		listener.Close();

		//		string currentStatus = ("Server stopped.");
		//	}
		//}


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
		//protected override System.Drawing.Bitmap Icon => null;
		//protected override System.Drawing.Bitmap Icon => Properties.Resources.GraphWebsite_icon_small;
		protected override Bitmap Icon
		{
			get
			{
				var iBytes = GraphWebsite.Properties.Resources.GraphWebsite_icon_small;
				using (MemoryStream memS = new MemoryStream(iBytes))
				{
					System.Drawing.Bitmap image = new System.Drawing.Bitmap(memS);
					return image;
				}
			}
		}


		/// <summary>
		/// Each component must have a unique Guid to identify it. 
		/// It is vital this Guid doesn't change otherwise old ghx files 
		/// that use the old ID will partially fail during loading.
		/// </summary>
		public override Guid ComponentGuid => new Guid("36545202-6c22-4737-a08f-6ebf896f66d3");
	}
}