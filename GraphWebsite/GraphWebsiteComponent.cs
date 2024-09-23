using Amazon.AutoScaling.Model;
using Amazon.ElasticBeanstalk.Model;
using Amazon.ElasticLoadBalancing.Model;
using Amazon.Runtime.Internal;
using Grasshopper;
using Grasshopper.Kernel;
using HttpServerLite;
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
		private HttpServerLiteProgram _httpServerLite;

		/// <summary>
		/// Each implementation of GH_Component must provide a public 
		/// constructor without any arguments.
		/// Category represents the Tab in which the component will appear, 
		/// Subcategory the panel. If you use non-existing tab or panel names, 
		/// new tabs/panels will automatically be created.
		/// </summary>
		public GraphWebsiteComponent()
		  : base("GraphWebsite", "GraphWeb",
			"Convert Rhino Geometry into RDF and sends it to GraphDB. Runs a website on localhost for querying data and representing its graphs.",
			"BHoM", "RDF")
		{
		}

		protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
		{
			// Graph db repository
			pManager.AddTextParameter("GraphDB URL", "URL", "Define URL of the GraphDB repository.", GH_ParamAccess.item, "");
			// Account name
			pManager.AddTextParameter("Account", "Account", "Define account name to GraphDB.", GH_ParamAccess.item, "");
			// Password
			pManager.AddTextParameter("Password", "PWD", "Define your password to GraphDB.", GH_ParamAccess.item, "");
			// Open website in webbrowser
			pManager.AddBooleanParameter("Open", "Open", "Open website in default browser with defined localhost port number", GH_ParamAccess.item, false);
			// Run HttpServerLite
			pManager.AddBooleanParameter("HttpServerLite", "HttpServerLite", "If true, start running the server of graph website on localhost at default webbrowser.", GH_ParamAccess.item, false);
		}

		protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
		{
			// Output status
			pManager.AddTextParameter("Status", "Status", "Status of localhost server communication.", GH_ParamAccess.item);
		}

		/// <summary>
		/// This is the method that actually does the work.
		/// </summary>
		/// <param name="DA">The DA object can be used to retrieve data from input parameters and 
		/// to store data in output parameters.</param>
		protected override void SolveInstance(IGH_DataAccess DA)
		{
			// Initialize variables
			string graphDBurl = "";
			string accountName = "";
			string password = "";
			bool openBrowser = false;
			bool runHttpServerLite = false;
			int port = 9000;

			/// <summary>
			/// Access the input parameters individually. 
			/// When data cannot be extracted from a parameter, abort this method.
			/// <summary>
			// Assign graphDB url
			if (!DA.GetData("GraphDB URL", ref graphDBurl)) return;
			// Assign account name
			if (!DA.GetData("Account", ref accountName)) return;
			// Assign password
			if (!DA.GetData("Password", ref password)) return;
			// Assign open browser
			if (!DA.GetData("Open", ref openBrowser)) return;
			// Assign run localhost server
			if (!DA.GetData("HttpServerLite", ref runHttpServerLite)) return;


			// Open the website in the browser
			if (openBrowser)
			{
				try
				{
					Process.Start($"http://localhost:{port}");
				}
				catch (Exception e)
				{
					DA.SetData("Status", e.Message);
					return;
				}
			}

			// Start or stop the HttpServerLite server based on input
			if (runHttpServerLite)
			{
				// Ensure server instance exists
				if (_httpServerLite == null) _httpServerLite = new HttpServerLiteProgram();

				try
				{
					_httpServerLite.StartServer(port);
					DA.SetData(0, $"HttpServerLite started at http://localhost:{port}");
				}
				catch (Exception ex)
				{
					DA.SetData(0, $"Error starting server: {ex.Message}");
				}
			}
			else
			{
				if (_httpServerLite != null)
				{
					try
					{
						_httpServerLite.StopServer();
						DA.SetData(0, "HttpServerLite stopped.");
					}
					catch (Exception ex)
					{
						DA.SetData(0, $"Error stopping server: {ex.Message}");
					}
				}
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