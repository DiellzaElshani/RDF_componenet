using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTLadapter.Utils
{
	public static class PConsole
	{
		static List<string> lines = new List<string>();

		public static void WriteLine(string line)
		{
			lines.Add(line);
		}

		public static List<string> Read()
		{
			return lines;
		}

		public static void Clear()
		{
			lines.Clear();
		}
	}
}
