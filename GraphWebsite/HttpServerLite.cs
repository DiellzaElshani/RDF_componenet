using HttpServerLite;
using System;
using System.Threading.Tasks;

namespace GraphWebsite
{
	public class HttpServerLiteProgram
	{
		private Webserver _Server;

		public void StartServer(int port)
		{
			Webserver server = new Webserver("localhost", port, false, null, null, DefaultRoute);
			server.Settings.Headers.Host = $"https://localhost:{port}";
			server.Start();
		}

		public void StopServer()
		{
			if (_Server != null)
			{
				_Server.Dispose();  // Stop and dispose of the server
				_Server = null;
			}
		}

		private static async Task DefaultRoute(HttpContext ctx)
		{
			string resp = "Hello from HttpServerLite!";
			ctx.Response.StatusCode = 200;
			ctx.Response.ContentLength = resp.Length;
			ctx.Response.ContentType = "text/plain";
			await ctx.Response.SendAsync(resp);
		}
	}
}
