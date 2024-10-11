using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WatsonWebserver.Core;



namespace GraphWebsite.Server
{
	public class WatsonWebserverProgramm
	{
		static WebserverSettings _Settings;
		static WebserverBase _Server;
		
		private StringHtml _StringHtml;

		private int _PORT;
		private string _HOSTSERVERADRESS;
		private string _AUTH;


		public void StartServer(int port, string serverAdress, string userAcc, string userPwd)
		{
			_PORT = port;
			_HOSTSERVERADRESS = serverAdress;
			_AUTH = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userAcc}:{userPwd}"));

			_Settings = new WebserverSettings
			{
				Hostname = "localhost",
				Port = port
			};

			// Initialize server
			_Server = new WatsonWebserver.Webserver(_Settings, DefaultRoute);
			
			// Set default permit
			_Server.Settings.AccessControl.Mode = AccessControlMode.DefaultPermit;
			//_Server.Settings.AccessControl.DenyList.Add("1.1.1.1", "255.255.255.255

			// Define Sparql Query to route 
			_Server.Routes.PreAuthentication.Static.Add(WatsonWebserver.Core.HttpMethod.GET, "/sparql", HandleSparqlQuery);

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
			// Get html string
			if (_StringHtml == null)
			{
				// Ensure _StringHtml is initialized before using it
				_StringHtml = new StringHtml(_PORT);
			}
			
			string htmlContent = _StringHtml.HtmlContent;
			ctx.Response.ContentType = "text/html; charset=utf-8";
			await ctx.Response.Send(htmlContent);
		}


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
				string fullUrl = $"{_HOSTSERVERADRESS}?query={Uri.EscapeDataString(sparqlQuery)}";
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
		#region
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
		#endregion
	}
}
