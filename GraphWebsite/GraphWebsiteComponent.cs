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
		//private LocalWebServer _server;
		private Process _nodeProcess;
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
			// run nodejs
			pManager.AddBooleanParameter("NodeJs", "NodeJs", "If true, start running the server of graph website on localhost at default webbrowser.", GH_ParamAccess.item, false);
			// run HttpServerLite
			pManager.AddBooleanParameter("HttpServerLite", "HttpServerLite", "If true, start running the server of graph website on localhost at default webbrowser.", GH_ParamAccess.item, false);
			// localhost port
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

			bool openBrowser = false;
			bool runNodeJs = false;
			bool runHttpServerLite = false;
			int port = 9000;
			string nginxServer = "http://ac497e68-aff6-453b-a07a-4a321b3a31c5.ma.bw-cloud-instance.org/";

			// Get the Grasshopper libraries path
			string grasshopperLibrariesPath = PathHelper.GetGrasshopperLibrariesPath();

			// Construct paths relative to the Grasshopper libraries path
			string scriptPath = Path.Combine(grasshopperLibrariesPath, "server2.js");
			string installDependenciesPath = Path.Combine(grasshopperLibrariesPath, "setup-node.ps1");
			//string installDependenciesPath = Path.Combine(grasshopperLibrariesPath, "install-dependencies.ps1");


			// Then we need to access the input parameters individually. 
			// When data cannot be extracted from a parameter, we should abort this method.

			// assign open browser
			if (!DA.GetData("Open", ref openBrowser)) return;

			// assign run node js server
			if (!DA.GetData("NodeJs", ref runNodeJs)) return;

			// assign run localhost server
			if (!DA.GetData("HttpServerLite", ref runHttpServerLite)) return;


			// We're set to create the website now. To keep the size of the SolveInstance() method small, 
			// The actual functionality will be in a different method:

			// Open the website in the browser
			if (openBrowser)
			{
				try
				{
					Process.Start($"http://localhost:{port}");
					Process.Start(nginxServer);
					//DA.SetData("Status", $"powerShellPath: {powerShellPath}");
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

			// Start the web server if 'run' is true
			if (runNodeJs)
			{
				if (_nodeProcess == null || _nodeProcess.HasExited)
				{
					try
					{

						// First, install dependencies using the PowerShell script
						string installOutput = CommandLine.RunCommand($"powershell.exe -ExecutionPolicy Bypass -File \"{installDependenciesPath}\"", grasshopperLibrariesPath);
						DA.SetData("Status", "Dependencies installed:\n" + installOutput);

						//// Then, run the Node.js server script
						//string startServerOutput = CommandLine.RunCommand($"node \"{scriptPath}\"", grasshopperLibrariesPath);
						//DA.SetData("Status", "Server started:\n" + startServerOutput);

						//// Prepare the PowerShell process start info
						//var powerShellStartInfo = new System.Diagnostics.ProcessStartInfo
						//{
						//	FileName = "powershell.exe",
						//	Arguments = $"-ExecutionPolicy Bypass -File \"{powerShellPath}\"",
						//	RedirectStandardOutput = true,
						//	RedirectStandardError = true,
						//	UseShellExecute = false,
						//	CreateNoWindow = true
						//};

						//// Start the PowerShell process
						//using (var powerShellProcess = System.Diagnostics.Process.Start(powerShellStartInfo))
						//{
						//	// Read output and error streams
						//	string powerShellOutput = powerShellProcess.StandardOutput.ReadToEnd();
						//	string powerShellError = powerShellProcess.StandardError.ReadToEnd();

						//	powerShellProcess.WaitForExit();

						//	// Handle output and error messages
						//	if (powerShellProcess.ExitCode == 0)
						//	{
						//		DA.SetData("Status", "Script executed successfully.");
						//		// Optionally, log the output
						//		Console.WriteLine("Output: " + powerShellOutput);
						//	}
						//	else
						//	{
						//		DA.SetData("Status", "Script execution failed.");
						//		// Optionally, log the error
						//		Console.WriteLine("Error: " + powerShellError);
						//	}
						//}
					}

					catch (Exception e)
					{
						DA.SetData("Status", $"Failed to execute PowerShell script: {e.Message}");
						Console.WriteLine($"Exception: {e.Message}");
					}

					try
					{
						_nodeProcess = new Process
						{
							StartInfo = new ProcessStartInfo
							{
								FileName = "node",
								Arguments = $"\"{scriptPath}\"",
								WorkingDirectory = grasshopperLibrariesPath,
								UseShellExecute = false,
								RedirectStandardOutput = true,
								RedirectStandardError = true,
								CreateNoWindow = true
							}
						};

						// Start the process
						_nodeProcess.Start();

						// Read the output
						string output = _nodeProcess.StandardOutput.ReadToEnd();
						string error = _nodeProcess.StandardError.ReadToEnd();

						_nodeProcess.WaitForExit();

						// Check for errors
						if (_nodeProcess.ExitCode == 0)
						{
							DA.SetData("Status", "Node.js server started successfully.");
						}
						else
						{
							DA.SetData("Status", $"Error starting Node.js server: {error}");
						}
					}
					catch (Exception e)
					{
						DA.SetData("Status", $"Failed to start Node.js server: {e.Message}");
					}
				}
				else
				{
					DA.SetData("Status", "Node.js server is already running.");
				}
			}
			else
			{
				if (_nodeProcess != null && !_nodeProcess.HasExited)
				{
					try
					{
						_nodeProcess.Kill();
						_nodeProcess.WaitForExit();
						_nodeProcess = null;
						DA.SetData("Status", "Node.js server stopped.");
					}
					catch (Exception e)
					{
						DA.SetData("Status", $"Failed to stop Node.js server: {e.Message}");
					}
				}
			}
		}




			// Start the web server if 'run' is true
			//if (runNodeJs)
			//{

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

			//if (port > 10000 || port < 0)
			//{
			//	AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid port number ");
			//	DA.SetData("Status", "Invalid port number");
			//	return;
			//}


			//// Start the web server if 'run' is true
			//if (run)
			//{
			//	if (_server == null)  // Initialize _server only if it's not already running
			//	{
			//		string baseDirectory = PathHelper.GetGrasshopperLibrariesPath();
			//		LocalWebServer server = new LocalWebServer(baseDirectory);
			//		server.Start();

			//		DA.SetData("Status", "Web server is running.");

			//		DA.SetData("Status", $"Base directory: {baseDirectory}");

			//	}
			//	else
			//	{
			//		DA.SetData("Status", "Web server is already running.");
			//	}
			//}
			//else
			//{
			//	// Only try to stop the server if it's been started
			//	if (_server != null)
			//	{
			//		_server.Stop();
			//		_server = null;  // Set _server to null after stopping
			//		DA.SetData("Status", "Plugin stopped and web server shut down.");
			//	}
			//	else
			//	{
			//		DA.SetData("Status", "Web server is not running.");
			//	}


			//	//RunScript(port, run, listener);
			//	//DA.SetData("Status", currentStatus);


			//	// Finally assign the spiral to the output parameter.
			//	//DA.SetData(0, spiral);
			//}
			//}

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