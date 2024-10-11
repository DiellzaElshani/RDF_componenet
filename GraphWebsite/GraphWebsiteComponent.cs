using Amazon.AutoScaling.Model;
using Amazon.ElasticBeanstalk.Model;
using Amazon.ElasticLoadBalancing.Model;
using Amazon.Runtime.Internal;
using BH.Adapter.GraphDB;
using BH.Engine.Adapters.RDF;
using GraphDB_WindowsForms;
using GraphWebsite.Server;
using Grasshopper;
using Grasshopper.Kernel;
using HttpServerLite;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

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
		private WindowsForms.Credentials _windowsForms;

		private string _HOSTSERVERADRESS;
		private string _USERACCOUNT;
		private string _USERPASSWORD;


		/// <summary>
		/// Each implementation of GH_Component must provide a public 
		/// constructor without any arguments.
		/// Category represents the Tab in which the component will appear, 
		/// Subcategory the panel. If you use non-existing tab or panel names, 
		/// new tabs/panels will automatically be created.
		/// </summary>
		public GraphWebsiteComponent()
		  : base("GraphWebsite", "GraphWeb",
			"Convert Rhino Geometry into RDF (and send it to GraphDB). Run a website on localhost for querying data of defined GraphDB repository and representing its graphs.",
			"BHoM", "RDF")
		{
		}

		protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
		{
			// Credentials
			pManager.AddBooleanParameter("Credentials", "Cred", "If true, enter credentials for the connecting host server in Windows Forms.Username & Password is not obligatory.", GH_ParamAccess.item, false);
			// Run server of GraphWebsite
			pManager.AddBooleanParameter("RunGraphWebsite", "RunGW", "If true & existing credentials, start running the server of GraphWebsite on localhost at default web browser.", GH_ParamAccess.item, false);

			#region
			//// Bhom graph setting
			//int inputGraphSettins = pManager.AddGenericParameter("GraphSettings", "GS", "Bhom graph settings", GH_ParamAccess.item);
			//// Convert to RDF
			//int inputConvert2RDF = pManager.AddBooleanParameter("Convert2RDF", "C2RDF", "If true, convert Bhom graph settings to RDF as .ttl file.", GH_ParamAccess.item);

			//// Optional
			//pManager[inputGraphSettins].Optional = true;
			//pManager[inputConvert2RDF].Optional = true;


			//// Window GraphDB log-in and repository root
			//pManager.AddBooleanParameter("Log-in GraphDB", "Log-in", "Open window", GH_ParamAccess.item, false);
			//// GraphDB repository url
			//pManager.AddTextParameter("GraphDB URL", "URL", "Define URL of the GraphDB repository.", GH_ParamAccess.item, "http://localhost:7200/repositories/BotOntology");
			//// User Account
			//pManager.AddTextParameter("Account", "Account", "Define account name to GraphDB.", GH_ParamAccess.item, "admin");
			//// User Password
			//pManager.AddTextParameter("Password", "PWD", "Define your password to GraphDB.", GH_ParamAccess.item, "admin");
			//// Open website in webbrowser
			//pManager.AddBooleanParameter("Open", "Open", "Open website in default browser with defined localhost port number", GH_ParamAccess.item, false);
			// Run Server on localhost
			//pManager.AddBooleanParameter("RunServer", "Run", "If true, start running the server of GraphWebsite on localhost at default webbrowser with custom GraphDB log-in.", GH_ParamAccess.item, false);
			#endregion
		}

		protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
		{
			// Output status
			pManager.AddTextParameter("Status", "Status", "Status of Watson Webserver communication.", GH_ParamAccess.item);

			#region
			//// Output converted RDF as .ttl
			//pManager.AddGenericParameter("RDF", "RDF", "Converted Resource Description Framework (RDF).", GH_ParamAccess.item);
			#endregion
		}

		/// <summary>
		/// This is the method that actually does the work.
		/// </summary>
		/// <param name="DA">The DA object can be used to retrieve data from input parameters and 
		/// to store data in output parameters.</param>
		protected override void SolveInstance(IGH_DataAccess DA)
		{
			// Initialize variables
			bool credentials = false;
			bool runGraphWebsite = false;
			int port = 9000;

		
			// Assign runGraphWebsite
			if (!DA.GetData("Run", ref credentials)) return;

			// Assign runGraphWebsite
			if (!DA.GetData("Run", ref runGraphWebsite)) return;




			// Open Windows Form to get user credentials.
			if (credentials)
			{
				_windowsForms = new WindowsForms.Credentials();

				var (serverAddress, username, password) = credentials.ExecuteWindowsForms();

				_HOSTSERVERADRESS = serverAddress;
				_USERACCOUNT = username;
				_USERPASSWORD = password;

				while (string.IsNullOrEmpty(serverAddress))
				{
					AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Server adress is empty.");
				}

				DA.SetData(0, $"serverAddress: {serverAddress}, username: {username}, password: {password}");
			}


			
			// Open the website in the browser
			// Start running the Watson Webserver
			if (runGraphWebsite && _HOSTSERVERADRESS.IsNullOrEmpty())
			{
				// Rhino 8 - opens urls
				// https://discourse.mcneel.com/t/floating-working-dir-in-rhino8-netcore-process-start/179287
				SystemExtensions.OpenUrl($"http://localhost:{port}");


				// Ensure server instance exists
				if (_watsonWebserver == null || string.IsNullOrEmpty(_watsonWebserver.ToString()))
				{
					_watsonWebserver = new WatsonWebserverProgramm();

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
			}

			// Stop Server
			else
			{
				if (_watsonWebserver != null)
				{
					try
					{
						_watsonWebserver.StopServer();
						DA.SetData(0, "Server stopped.");
					}
					catch (Exception ex)
					{
						DA.SetData(0, $"Error stopping server: {ex.Message}");
					}
				}
			}

				#region
				// bhom.graph-settings
				//bool logInGraphDB = false;
				//bool openBrowser = false;
				//bool runServer = false;
				//string graphDB_url = "";
				//string userPassword = "";

				//string userServerAdress = "";
				//string userAccount = "";

				/// <summary>
				/// Access the input parameters individually. 
				/// When data cannot be extracted from a parameter, abort this method.
				/// <summary>
				//// Assign log-in graphDB
				//if (!DA.GetData("Log-in GraphDB", ref logInGraphDB)) return;
				//// Assign bhom graph settings
				//if (!DA.GetData("GraphSettings", ref bhomGraphSettings)) return;
				//// Assign graphDB url
				//if (!DA.GetData("GraphDB URL", ref graphDB_url)) return;
				//// Assign account name
				//if (!DA.GetData("Account", ref userAccount)) return;
				//// Assign password
				//if (!DA.GetData("Password", ref userPassword)) return;
				// Assign open browser
				//if (!DA.GetData("Open", ref openBrowser)) return;
				//// Assign run localhost server
				//if (!DA.GetData("RunServer", ref runServer)) return;



				///// <summary>
				///// Access the current working directory.
				///// Printing file path in GH.
				///// <summary>
				//// Get the current working directory
				//string rhinoDirectory = Directory.GetCurrentDirectory();
				//string App_Domain = AppDomain.CurrentDomain.BaseDirectory; // Same as Directory.GetCurrentDirectory()
				//// Get the directory of the running component
				//string componentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				////DA.SetData(0, $"Current Working Directory: {rhinoDirectory} \nRunning Component Directory: {componentDirectory} \nAppDomain Current Directory: {App_Domain}");


				// Construct the path to the HTML file
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

				//// Open the website in the browser
				//if (openBrowser)

				//	// Rhino 7 - only
				//	//try
				//	//{
				//	//	Process.Start($"http://localhost:{port}");
				//	//}
				//	//catch (Exception e)
				//	//{
				//	//	DA.SetData("Status", e.Message);
				//	//	return;
				//	//}
				//}

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
				#endregion

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