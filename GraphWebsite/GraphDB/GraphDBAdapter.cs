using Amazon.AutoScaling.Model;
using BH.UI.Engine.GraphDB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphWebsite.GraphDB
{
	public class GraphDBAdapter
	{
		private string _GRAPHDBEXEPATH;
		private string _SERVERADRESS;
		private readonly string graphDBExecutableName = "GraphDB";
		private readonly string executablePath = @"C:\ProgramData\BHoM\Assemblies\GraphDB_WindowsForms.exe";

		/// <summary>
		/// Initializes the GraphDB process and login form internally.
		/// If activate, start GraphDBProcess
		/// If true
		/// </summary>
		public string ExecuteGraphDB(string userServerAdress, string userAccount, bool activate)
		{
			_SERVERADRESS = userServerAdress;

			// Checking existance
			if (string.IsNullOrEmpty(_GRAPHDBEXEPATH))
			{
				// Find GraphDB executable 
				_GRAPHDBEXEPATH = FindExecutable();
			}

			// Open Login Windows Form
			bool isRunningLogin = Process.GetProcessesByName("GraphDB_WindowsForms").Any();
			//bool isRunningLogin = Process.GetProcesses().Any(p => p.ProcessName.Contains("GraphDB_WindowsForms"));

			// Check 
			bool jsonWithUsername = File.Exists(LoginDataRetriever.ConstructJSONPath(_SERVERADRESS, userAccount));

			// Start default root GraphDB executable
			if (!isRunningLogin && !jsonWithUsername)
			{
				string executablePath = @"C:\ProgramData\BHoM\Assemblies\GraphDB_WindowsForms.exe";
				Process.Start(executablePath);
			}

			bool isRunning = Process.GetProcesses().Any(p => p.ProcessName.Contains("GraphDB"));
			if (activate && !isRunning)
			{
				StartGraphDBProcess(_GRAPHDBEXEPATH);
			}

			return _GRAPHDBEXEPATH;
		}

		/// <summary>
		/// Find GraphDB executable .exe on machine
		/// </summary>
		/// <param name="folderNameToSearch"></param>
		/// <returns></returns>
		public static string FindExecutable()
		{
			string folderNameToSearch = "GraphDB";
			string exeFileExtension = ".exe";

			// Get the path to the LocalAppData folder for the current user
			string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

			// Get all subdirectories in the LocalAppData folder
			string[] subdirectories = Directory.GetDirectories(localAppDataPath);

			// Search for the target folder
			string targetFolderPath = null;
			foreach (string subdirectory in subdirectories)
			{
				if (Path.GetFileName(subdirectory).IndexOf(folderNameToSearch, StringComparison.OrdinalIgnoreCase) >= 0)
				{
					Console.WriteLine($"Found the target folder: {subdirectory}");
					targetFolderPath = subdirectory;
					break;
				}
			}

			if (targetFolderPath != null)
			{
				// Get all files in the target folder
				string[] files = Directory.GetFiles(targetFolderPath);

				// Search for the exe file in the target folder
				foreach (string file in files)
				{
					if (Path.GetExtension(file).Equals(exeFileExtension, StringComparison.OrdinalIgnoreCase))
					{
						// Return the found executable path
						return file;
					}
				}

				// If no exe file is found
				return $"No exe files found in the folder '{folderNameToSearch}'.";
			}
			else
			{
				// If the target folder is not found
				return $"Folder with the name '{folderNameToSearch}' not found.";
			};
		}

		public static void StartGraphDBProcess(string graphDBexePath = "%APPDATA%\\Local\\GraphDB Free\\GraphDB Free.exe", bool run = false)
		{
			if (!System.IO.File.Exists(graphDBexePath))
			{
				Console.WriteLine($"Could not find the {nameof(graphDBexePath)}. Please make sure that the path to the GraphDB executable file is correct.", typeof(ArgumentException));
				return;
			}

			if (run == true)
			{
				Process.Start(graphDBexePath);
				//Task.Delay(500).Wait();
			}
		}
	}
}
