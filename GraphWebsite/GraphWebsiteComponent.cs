using BH.Engine.Adapters.RDF;
using GraphWebsite.Server;
using GraphWebsite.WindowPopUp;
using Grasshopper.Kernel;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

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
		private WatsonWebserverProgramm _watsonWebserver;
		//private WindowsFroms.WindowsFramesCredentials _windowsForms;

		private string _HOSTSERVERADRESS;
		private string _USERACCOUNT;
		private string _USERPASSWORD;

		private bool _cachedValue;
		private bool _inputHasChanged = true;


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
			pManager.AddBooleanParameter("Credentials", "Cred", "If true, enter credentials for the connecting host server in Eto Forms. Username & Password is not obligatory.", GH_ParamAccess.item, false);
			// Run server of GraphWebsite
			pManager.AddBooleanParameter("RunGraphWebsite", "RunGW", "If true & existing credentials, start running the server of GraphWebsite on localhost at default web browser.", GH_ParamAccess.item, false);
		}

		protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
		{
			// Output status
			//pManager.AddTextParameter("Status", "Status", "Status of Watson Webserver communication.", GH_ParamAccess.item);
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
			if (!DA.GetData("Credentials", ref credentials)) return;

			// Assign runGraphWebsite
			if (!DA.GetData("RunGraphWebsite", ref runGraphWebsite)) return;


			// If the input has not changed, avoid recomputation
			if (credentials == _cachedValue)
			{
				_inputHasChanged = false;
			}
			else
			{
				_inputHasChanged = true;
				_cachedValue = credentials;
			}


			// Open Windows Form to get user credentials.
			if (credentials && _inputHasChanged)
			{
				var credentialsDialog = new CredentialsDialog();
				var result = credentialsDialog.ShowModal();

				if (!credentialsDialog.Content.IsNullOrEmpty())
				{
					// Access the user inputs after the dialog closes
					_HOSTSERVERADRESS = credentialsDialog.ServerAddress;
					_USERACCOUNT = credentialsDialog.Username;
					_USERPASSWORD = credentialsDialog.Password;

					// Exit if the server address is empty
					if (string.IsNullOrEmpty(_HOSTSERVERADRESS))
					{
						AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Server address is empty. This is mandatory!");
						return;
					}
				}
			}


			// Open the website in the browser
			// Start running the Watson Webserver
			if (runGraphWebsite)
			{
				if (!string.IsNullOrEmpty(_HOSTSERVERADRESS))
				{
					// Rhino 8 - opens urls
					// https://discourse.mcneel.com/t/floating-working-dir-in-rhino8-netcore-process-start/179287
					SystemExtensions.OpenUrl($"http://localhost:{port}");


					// Ensure server instance exists
					if (_watsonWebserver == null)
					{
						_watsonWebserver = new WatsonWebserverProgramm();

						try
						{
							_watsonWebserver.StartServer(port, _HOSTSERVERADRESS, _USERACCOUNT, _USERPASSWORD);
							//DA.SetData(0, $"Server started at http://localhost:{port}");
						}
						catch (Exception ex)
						{
							AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Error starting server: {ex.Message}");
							//DA.SetData(0, $"Error starting server: {ex.Message}");
						}
					}
				}
				else
				{
					AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Server adress is empty. Enter your credentials!");
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

						AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Server stopped.");
						//DA.SetData(0, "Server stopped.");
					}
					catch (Exception ex)
					{
						AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Error stopping server: {ex.Message}");
						//DA.SetData(0, $"Error stopping server: {ex.Message}");
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