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
			//Webserver server = new Webserver("localhost", port, false, null, null, DefaultRoute);
			//server.Settings.Headers.Host = $"https://localhost:{port}";
			//server.Start();

			Webserver server = new Webserver("localhost", port, false, null, null, MyDynamicRoute);
			server.Start();
		}

		public void StopServer()
		{
			if (_Server != null)
			{
				// Stop and dispose of the server
				_Server.Dispose();  
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

		[StaticRoute(HttpMethod.GET, "/static")]
		public static async Task MyStaticRoute(HttpContext ctx)
		{
			string resp = "Hello from the static route";
			ctx.Response.StatusCode = 200;
			ctx.Response.ContentType = "text/plain";
			ctx.Response.ContentLength = resp.Length;
			await ctx.Response.SendAsync(resp);
			return;
		}

		[ParameterRoute(HttpMethod.GET, "/{version}/api/{id}")]
		public static async Task MyParameterRoute(HttpContext ctx)
		{
			string resp = "Hello from parameter route version " + ctx.Request.Url.Parameters["version"] + " for ID " + ctx.Request.Url.Parameters["id"];
			ctx.Response.StatusCode = 200;
			ctx.Response.ContentType = "text/plain";
			ctx.Response.ContentLength = resp.Length;
			await ctx.Response.SendAsync(resp);
			return;
		}

		[DynamicRoute(HttpMethod.GET, "^/dynamic/\\d+$")]
		public static async Task MyDynamicRoute(HttpContext ctx)
		{
			string resp = "Hello from the dynamic route";
			ctx.Response.StatusCode = 200;
			ctx.Response.ContentType = "text/plain";
			ctx.Response.ContentLength = resp.Length;
			await ctx.Response.SendAsync(resp);
			return;
		}
	}
}
