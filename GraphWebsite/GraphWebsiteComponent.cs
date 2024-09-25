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
using System.Reflection;
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
		//private HttpServerLiteProgram _httpServerLite;
		private WatsonWebserverProgramm _watsonWebserver;

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
			pManager.AddTextParameter("GraphDB URL", "URL", "Define URL of the GraphDB repository.", GH_ParamAccess.item, "http://localhost:7200/repositories/BotOntology");
			// Account name
			pManager.AddTextParameter("Account", "Account", "Define account name to GraphDB.", GH_ParamAccess.item, "admin");
			// Password
			pManager.AddTextParameter("Password", "PWD", "Define your password to GraphDB.", GH_ParamAccess.item, "admin");
			// Open website in webbrowser
			pManager.AddBooleanParameter("Open", "Open", "Open website in default browser with defined localhost port number", GH_ParamAccess.item, false);
			// Run HttpServerLite
			pManager.AddBooleanParameter("RunServer", "Run", "If true, start running the server of graph website on localhost at default webbrowser.", GH_ParamAccess.item, false);
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
			string graphDB_url = "";
			string userAccount = "";
			string userPassword = "";
			bool openBrowser = false;
			bool runServer = false;
			int port = 9000;

			/// <summary>
			/// Access the input parameters individually. 
			/// When data cannot be extracted from a parameter, abort this method.
			/// <summary>
			// Assign graphDB url
			if (!DA.GetData("GraphDB URL", ref graphDB_url)) return;
			// Assign account name
			if (!DA.GetData("Account", ref userAccount)) return;
			// Assign password
			if (!DA.GetData("Password", ref userPassword)) return;
			// Assign open browser
			if (!DA.GetData("Open", ref openBrowser)) return;
			// Assign run localhost server
			if (!DA.GetData("RunServer", ref runServer)) return;

			/// <summary>
			/// Access the current working directory.
			/// Printing file path in GH.
			/// <summary>
			// Get the current working directory
			string rhinoDirectory = Directory.GetCurrentDirectory();
			string App_Domain = AppDomain.CurrentDomain.BaseDirectory; // Same as Directory.GetCurrentDirectory()
			// Get the directory of the running component
			string componentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			//DA.SetData(0, $"Current Working Directory: {rhinoDirectory} \nRunning Component Directory: {componentDirectory} \nAppDomain Current Directory: {App_Domain}");
			#region
			//// Construct the path to the HTML file
			//string htmlFilePath = Path.Combine(componentDirectory, "Website", "webpage.html");

			//// Check if the HTML file exists
			//if (!System.IO.File.Exists(htmlFilePath))
			//{
			//	DA.SetData(0, $"HTML file not found at: {htmlFilePath}");
			//	return;
			//}
			//else
			//{
			//	DA.SetData(0, $"HTML file found at: {htmlFilePath}");
			//}
			#endregion

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
			if (runServer)
			{
				// Ensure server instance exists
				if (_watsonWebserver == null) _watsonWebserver = new WatsonWebserverProgramm();

				try
				{
					_watsonWebserver.StartServer(port, componentDirectory, graphDB_url, userAccount, userPassword);
					DA.SetData(0, $"Server started at http://localhost:{port}");
				}
				catch (Exception ex)
				{
					DA.SetData(0, $"Error starting server: {ex.Message}");
				}
			}
			else
			{
				if (_watsonWebserver != null)
				{
					try
					{
						_watsonWebserver.StopServer();
						_watsonWebserver = null;
						DA.SetData(0, "Server stopped.");
					}
					catch (Exception ex)
					{
						DA.SetData(0, $"Error stopping server: {ex.Message}");
					}
				}
			}

			//if (runServer)
			//{
			//	// Ensure server instance exists
			//	if (_httpServerLite == null) _httpServerLite = new HttpServerLiteProgram();

			//	try 
			//	{
			//		_httpServerLite.StartServer(port, graphDB_url, userAccount, userPassword);
			//		DA.SetData(0, $"HttpServerLite started at http://localhost:{port}");
			//	}
			//	catch (Exception ex)
			//	{
			//		DA.SetData(0, $"Error starting server: {ex.Message}");
			//	}
			//}
			//else
			//{
			//	if (_httpServerLite != null)
			//	{
			//		try
			//		{
			//			_httpServerLite.StopServer();
			//			_httpServerLite = null;
			//			DA.SetData(0, "HttpServerLite stopped.");
			//		}
			//		catch (Exception ex)
			//		{
			//			DA.SetData(0, $"Error stopping server: {ex.Message}");
			//		}
			//	}
			//}
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