using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GraphWebsite.Server
{
	public class SystemExtensions
	{
		/// <summary>
		/// Open default webbrowser on all OS with URL as input
		/// </summary>
		/// <param name="url"></param>
		public static void OpenUrl(string url)
		{
			if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute)) return;

			// see: https://github.com/dotnet/corefx/issues/10361
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				url = url.Replace("&", "^&");
				Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				Process.Start("xdg-open", url);
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				Process.Start("open", url);
			}
		}
	}
}
