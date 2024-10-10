using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphWebsite
{
	public class PathHelper
	{
		public static string GetGrasshopperLibrariesPath()
		{
			string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			string librariesPath = Path.Combine(appDataPath, "Grasshopper", "Libraries", "GraphWebsite");
			return librariesPath;
		}
	}
}
