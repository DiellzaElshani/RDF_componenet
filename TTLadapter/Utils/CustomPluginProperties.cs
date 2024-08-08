using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTLadapter.Utils
{
	public static class CustomPluginProperties
	{
		//public static string Version
		//{
		//	get
		//	{
		//		return "0.1.0";
		//	}
		//}

		/// <summary>
		/// Current plugin version
		/// </summary>

		//public static string Version => "0.1.0";

		public static string Version => MAJOR_VERSION + "." + MINOR_VERSION + "." + PATCH_VERSION;

		public static int MAJOR_VERSION = 0;
		public static int MINOR_VERSION = 1;
		public static int PATCH_VERSION = 0;
	}
}
