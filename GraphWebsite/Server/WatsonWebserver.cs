using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WatsonWebserver;
using WatsonWebserver.Core;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.RegularExpressions;
using VDS.RDF;
using Grasshopper.Kernel.Data;
using System.Linq.Expressions;
using AngleSharp.Io;



namespace GraphWebsite.Server
{ 
	public class WatsonWebserverProgramm
	{
		static WebserverSettings _Settings = null;
		static WebserverBase _Server = null;
        static string _Hostname = "localhost";
		private string _AUTH;
		private int _PORT;
        private string _COMPONENT_DIRECTORY;
		private string _GRAPHDB_URL;
		private StringHtml _StringHtml;


		public void StartServer(int port, string component_directory, string graph_db_url, string userAcc, string userPwd)
		{
			_PORT = port;
			_COMPONENT_DIRECTORY = component_directory;
			_GRAPHDB_URL = graph_db_url;
			_AUTH = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userAcc}:{userPwd}"));

			_Settings = new WebserverSettings
			{
				Hostname = _Hostname,
				Port = port
			};

			// Initialize server
			_Server = new WatsonWebserver.Webserver(_Settings, DefaultRoute);
			
			// Set default permit
			_Server.Settings.AccessControl.Mode = AccessControlMode.DefaultPermit;
			//_Server.Settings.AccessControl.DenyList.Add("1.1.1.1", "255.255.255.255

			// Define Sparql Query to route 
			_Server.Routes.PreAuthentication.Static.Add(WatsonWebserver.Core.HttpMethod.GET, "/sparql", HandleSparqlQuery);

			#region
			//	// Ensure the upload directory exists
			//	if (!Directory.Exists(_COMPONENT_DIRECTORY))
			//         {
			//	_Server.Routes.PreAuthentication.Dynamic.Add(WatsonWebserver.Core.HttpMethod.GET, new Regex("/"), (ctx) =>
			//	{
			//		ctx.Response.StatusCode = 404;
			//		return ctx.Response.Send("_COMPONENT_DIRECTORY not found");
			//	});
			//};
			#endregion

			// Start server
			_Server.Start();
		}

		public void StopServer()
		{
			if (_Server != null)
			{
				_Server.Dispose();
				_Server = null;
			}
		}


		// Default route
		private async Task DefaultRoute(HttpContextBase ctx)
		{
			FileStream fs = null;
			string filePath;


			// Get html string
			if (_StringHtml == null)
			{
				// Ensure _StringHtml is initialized before using it
				_StringHtml = new StringHtml(_PORT);
			}
			
			string htmlContent = _StringHtml.HtmlContent;
			ctx.Response.ContentType = "text/html; charset=utf-8";
			await ctx.Response.Send(htmlContent);

			//if (string.IsNullOrEmpty(htmlContent))
			//{
			//	ctx.Response.StatusCode = 500;
			//	await ctx.Response.Send("Internal Server Error: HTML content is not initialized.");
			//	return;
			//}

			//switch (ctx.Request.Method)
			//{
			//	case WatsonWebserver.Core.HttpMethod.GET:
			//		// Handle GET requests to serve files dynamically
			//		if (ctx.Request.Url.Elements != null && ctx.Request.Url.Elements.Length > 0)
			//		{
			//			// Sanitize the file name to prevent directory traversal attacks
			//			string fileName = Path.GetFileName(ctx.Request.Url.Elements[0]);
			//			filePath = Path.Combine(_COMPONENT_DIRECTORY, fileName);

			//			// Check if the file exists
			//			if (File.Exists(filePath))
			//			{
			//				long len = new FileInfo(filePath).Length;
			//				using (fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
			//				{
			//					ctx.Response.StatusCode = 200;
			//					await ctx.Response.Send(len, fs);  // Send the file content
			//					return;
			//				}
			//			}
			//			else
			//			{
			//				// File not found
			//				ctx.Response.StatusCode = 404;
			//				await ctx.Response.Send("File not found");
			//				return;
			//			}
			//		}
			//		else
			//		{
			//			// Bad request if no file name is provided
			//			ctx.Response.StatusCode = 400;
			//			await ctx.Response.Send("Bad request - Missing file name");
			//			return;
			//		}

			//	case WatsonWebserver.Core.HttpMethod.POST:
			//		// Handle POST requests to upload files
			//		if (ctx.Request.Url.Elements == null || ctx.Request.Url.Elements.Length != 1)
			//		{
			//			ctx.Response.StatusCode = 400;
			//			await ctx.Response.Send("Bad request - Invalid URL");
			//			return;
			//		}
			//		else if (ctx.Request.Data == null || !ctx.Request.Data.CanRead)
			//		{
			//			ctx.Response.StatusCode = 400;
			//			await ctx.Response.Send("Bad request - No data provided");
			//			return;
			//		}
			//		else
			//		{
			//			// Get the file name from the URL and sanitize it
			//			string fileName = Path.GetFileName(ctx.Request.Url.Elements[0]);
			//			filePath = Path.Combine(_COMPONENT_DIRECTORY, fileName);

			//			// Save the uploaded file
			//			using (fs = new FileStream(filePath, FileMode.Create))  // Overwrite if file exists
			//			{
			//				int bytesRead = 0;
			//				byte[] buffer = new byte[2048];
			//				while ((bytesRead = ctx.Request.Data.Read(buffer, 0, buffer.Length)) > 0)
			//				{
			//					fs.Write(buffer, 0, bytesRead);
			//				}
			//			}

			//			// Respond with '201 Created' status
			//			ctx.Response.StatusCode = 201;
			//			await ctx.Response.Send("File uploaded successfully");
			//			return;
			//		}

			//	default:
			//		// Respond with 400 Bad Request for unsupported methods
			//		ctx.Response.StatusCode = 400;
			//		await ctx.Response.Send("Bad request - Unsupported method");
			//		return;
			//}
		}

		// Testing
		// Handle Sparql Query
		private async Task HandleSparqlQuery(HttpContextBase ctx)
		{
			try
			{
				// Read the SPARQL query from the request URL
				string sparqlQuery = ctx.Request.Query.Elements["query"];
				string includeInferred = ctx.Request.Query.Elements["reasoning"];

				if (string.IsNullOrEmpty(sparqlQuery))
				{
					ctx.Response.StatusCode = 400;
					await ctx.Response.Send("No SPARQL query provided.");
					return;
				}

				// URL-decode the query
				sparqlQuery = Uri.UnescapeDataString(sparqlQuery);

				Console.WriteLine($"Executing SPARQL query: {sparqlQuery}");

				// Adjust the URL based on the reasoning parameter
				string fullUrl = $"{_GRAPHDB_URL}?query={Uri.EscapeDataString(sparqlQuery)}";
				fullUrl += includeInferred == "true" ? "&infer=true" : "&infer=false";

				// Use 'application/rdf+xml' for all queries
				string acceptHeader = "application/rdf+xml";

				using (HttpClient client = new HttpClient())
				{
					client.DefaultRequestHeaders.Add("Authorization", _AUTH); // Add authorization
					client.DefaultRequestHeaders.Add("Accept", acceptHeader);  // Set accept header for RDF/XML response

					// Send the request to GraphDB
					HttpResponseMessage graphDbResponse = await client.GetAsync(fullUrl);

					Console.WriteLine($"Received response from GraphDB: {graphDbResponse.StatusCode} - {graphDbResponse.ReasonPhrase}");

					if (graphDbResponse.IsSuccessStatusCode)
					{
						// Get the response as RDF/XML text
						string data = await graphDbResponse.Content.ReadAsStringAsync();

						// Return the RDF/XML content with proper headers
						ctx.Response.StatusCode = 200;
						ctx.Response.ContentType = $"{acceptHeader};charset=utf-8";
						await ctx.Response.Send(data);
					}
					else
					{
						// Handle error response from GraphDB
						string errorText = await graphDbResponse.Content.ReadAsStringAsync();
						ctx.Response.StatusCode = (int)graphDbResponse.StatusCode;
						await ctx.Response.Send($"SPARQL query failed: {graphDbResponse.ReasonPhrase} - {errorText}");
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error during SPARQL query execution: {ex.Message}");
				ctx.Response.StatusCode = 500;
				await ctx.Response.Send($"Error during SPARQL query execution: {ex.Message}");
			}
		}

		//// Route to handle RDF file upload and GraphDB interaction
		//static async Task UploadRoute(HttpContext ctx)
		//{
		//	if (!ctx.Request.Data.Equals("rdfFile"))
		//	{    
		//		ctx.Response.StatusCode = 400;
		//		await ctx.Response.Send("No file uploaded.");
		//		return;
		//	}

		//	var uploadedFile = ctx.Request.Data["rdfFile"];
		//          ctx.Request.Url.Elements.Equals("rdfFile");
		//	string contentType = "text/turtle";  // Assuming Turtle format (change if needed)

		//	try
		//	{
		//		Console.WriteLine("Uploading file to GraphDB...");

		//		// Read the file content
		//		string fileContent;
		//		using (var reader = new StreamReader(uploadedFile.Data))
		//		{
		//			fileContent = await reader.ReadToEndAsync();
		//		}

		//		Console.WriteLine("File content preview: " + fileContent.Substring(0, Math.Min(fileContent.Length, 500)));

		//		// Send file content to GraphDB
		//		var result = await PostToGraphDB(fileContent, contentType);
		//		Console.WriteLine("Received response from GraphDB: " + result.StatusCode);

		//		if (result.IsSuccessStatusCode)
		//		{
		//			await ctx.Response.Send("File uploaded successfully to GraphDB.");
		//		}
		//		else
		//		{
		//			string responseBody = await result.Content.ReadAsStringAsync();
		//			Console.WriteLine("Failed to upload: " + responseBody);
		//			await ctx.Response.Send($"Failed to upload file to GraphDB. Server said: {responseBody}", (int)result.StatusCode);
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		Console.WriteLine("Error during file upload: " + ex.Message);
		//		await ctx.Response.Send("Server error during file upload.", 500);
		//	}
		//}

		//// Function to send RDF file to GraphDB
		//static async Task<HttpResponseMessage> PostToGraphDB(string content, string contentType)
		//{
		//	using (var client = new HttpClient())
		//	{
		//		client.DefaultRequestHeaders.Add("Authorization", "Basic " + AUTH);
		//		HttpContent httpContent = new StringContent(content, Encoding.UTF8, contentType);

		//		return await client.PostAsync($"{GRAPHDB_URL}/statements", httpContent);
		//	}
		//}


	}
}
