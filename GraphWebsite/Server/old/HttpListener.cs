using System;
using System.IO;
using System.Net;
using System.Text;

namespace GraphWebsite
{

	public class LocalWebServer
	{
		private readonly HttpListener _listener = new HttpListener();
		private readonly string _baseDirectory;

		public LocalWebServer(string baseDirectory)
		{
			_baseDirectory = baseDirectory;
			_listener.Prefixes.Add("http://localhost:8080/");
		}

		public void Start()
		{
			_listener.Start();
			Console.WriteLine($"Server is listening on http://localhost:8080/");
			Console.WriteLine($"Base directory: {_baseDirectory}");
			_listener.BeginGetContext(new AsyncCallback(HandleRequest), _listener);
		}

		private void HandleRequest(IAsyncResult result)
		{
			if (!_listener.IsListening) return;

			HttpListenerContext context = _listener.EndGetContext(result);
			HttpListenerRequest request = context.Request;
			HttpListenerResponse response = context.Response;

			try
			{
				if (request.HttpMethod == "GET")
				{
					HandleGetRequest(request, response);
				}
				else if (request.HttpMethod == "POST")
				{
					HandlePostRequest(request, response);
				}
				else
				{
					response.StatusCode = 405; // Method Not Allowed
					byte[] errorMessage = Encoding.UTF8.GetBytes("405 - Method Not Allowed");
					response.OutputStream.Write(errorMessage, 0, errorMessage.Length);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error handling request: {ex.Message}");
				response.StatusCode = 500; // Internal Server Error
				byte[] errorMessage = Encoding.UTF8.GetBytes("500 - Internal Server Error");
				response.OutputStream.Write(errorMessage, 0, errorMessage.Length);
			}
			finally
			{
				response.OutputStream.Close();
				if (_listener.IsListening)
				{
					_listener.BeginGetContext(new AsyncCallback(HandleRequest), _listener);
				}
			}
		}

		private void HandleGetRequest(HttpListenerRequest request, HttpListenerResponse response)
		{
			string requestedFile = Path.Combine(_baseDirectory, request.Url.LocalPath.TrimStart('/'));
			Console.WriteLine($"Requested file path: {requestedFile}");

			if (File.Exists(requestedFile))
			{
				byte[] fileBytes = File.ReadAllBytes(requestedFile);
				response.ContentType = GetContentType(requestedFile);
				response.ContentLength64 = fileBytes.Length;
				response.OutputStream.Write(fileBytes, 0, fileBytes.Length);
			}
			else
			{
				response.StatusCode = 404;
				byte[] errorMessage = Encoding.UTF8.GetBytes("404 - File Not Found");
				response.OutputStream.Write(errorMessage, 0, errorMessage.Length);
			}
		}

		private void HandlePostRequest(HttpListenerRequest request, HttpListenerResponse response)
		{
			using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
			{
				string postData = reader.ReadToEnd();
				Console.WriteLine($"Received POST data: {postData}");

				string responseString = $"POST data received: {postData}";
				byte[] buffer = Encoding.UTF8.GetBytes(responseString);
				response.ContentLength64 = buffer.Length;
				response.OutputStream.Write(buffer, 0, buffer.Length);
			}
		}

		public void Stop()
		{
			try
			{
				_listener.Stop();
				_listener.Close();
				Console.WriteLine("Server stopped.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error stopping the server: {ex.Message}");
			}
		}

		// Determine the content type based on file extension
		private string GetContentType(string filePath)
		{
			string extension = Path.GetExtension(filePath);

			switch (extension)
			{
				case ".html":
					return "text/html";
				case ".js":
					return "application/javascript";
				case ".css":
					return "text/css";
				case ".json":
					return "application/json";
				default:
					return "application/octet-stream";
			}
		}
	}
}



