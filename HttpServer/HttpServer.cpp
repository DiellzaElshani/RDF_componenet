using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
	class Program
	{
		static async Task Main(string[] args)
		{
			string[] prefixes = { "http://localhost:5000/" };

			using (HttpListener listener = new HttpListener())
			{
				foreach(string prefix in prefixes)
				{
					listener.Prefixes.Add(prefix);
				}
				listener.Start();
				Console.WriteLine("Listening for connections on http://localhost:5000/");

				while (true)
				{
					HttpListenerContext context = await listener.GetContextAsync();
					HttpListenerRequest request = context.Request;
					HttpListenerResponse response = context.Response;

					if (request.HttpMethod == "POST")
					{
						using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
						{
							string postData = reader.ReadToEnd();
							Console.WriteLine("Received POST data: " + postData);

							string responseString = "POST request received";
							byte[] buffer = Encoding.UTF8.GetBytes(responseString);
							response.ContentLength64 = buffer.Length;
							using (Stream output = response.OutputStream)
							{
								await output.WriteAsync(buffer, 0, buffer.Length);
							}
						}
					}
					else
					{
						response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
						string responseString = "Only POST requests are supported.";
						byte[] buffer = Encoding.UTF8.GetBytes(responseString);
						response.ContentLength64 = buffer.Length;
						using (Stream output = response.OutputStream)
						{
							await output.WriteAsync(buffer, 0, buffer.Length);
						}
					}
				}
			}
		}
	}
}
