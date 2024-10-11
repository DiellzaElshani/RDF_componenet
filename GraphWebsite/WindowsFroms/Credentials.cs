using GraphDB_WindowsForms;
using GraphWebsite.WindowsFroms;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GraphWebsite.WindowsForms
{
	public class Credentials
	{
		#region
		//private string _GRAPHDBEXEPATH;
		//private string _SERVERADRESS = "localhost://7200";
		//private readonly string _USERACCOUNT = "";
		//private readonly string graphDBExecutableName = "GraphDB";
		//private readonly string executablePath = @"C:\ProgramData\BHoM\Assemblies\GraphDB_WindowsForms.exe";
		#endregion

		[STAThread] // Ensure single-threaded apartment
		public (string ServerAddress, string Username, string Password) ExecuteWindowsForms()
		{
			// Enable visual styles for the application
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			// Create and show the login form
			using (Login loginForm = new Login())
			{
				// Show the form as a dialog and check the result
				if (loginForm.ShowDialog() == DialogResult.OK)
				{
					// Access the user inputs after the form closes
					return (loginForm.ServerAddress, loginForm.Username, loginForm.Password);
				}
				// Return default values if canceled
				return (string.Empty, string.Empty, string.Empty);
			}
		}
			#region
		//	// Example data to save
		//	var testData = new
		//	{
		//		ServerAddress = "localhost",
		//		Username = "john.doe",
		//		Password = "password123"
		//	};

		//	// Serialize the test data to JSON format
		//	string json = JsonConvert.SerializeObject(testData, Formatting.Indented);

		//	// Get the path to the temp directory
		//	string tempPath = Path.GetTempPath();

		//	// Define the file name (e.g., "test_credentials.json")
		//	string fileName = "test_credentials.json";
		//	string filePath = Path.Combine(tempPath, fileName);

		//	// Write the JSON data to the file
		//	try
		//	{
		//		File.WriteAllText(filePath, json);
		//	}
		//	catch (Exception ex)
		//	{
		//		return ("Error saving JSON file: " + ex.Message);
		//	}


		//	return ($"Test JSON file saved successfully at: {filePath}");
		//}

		
		///// <sumdmary>
		///// Initializes the GraphDB process and login form internally.
		///// If activate, start GraphDBProcess
		///// If true
		///// </summary>
		//public string ExecuteGraphDB()
		//{
		//	// Checking existance
		//	if (string.IsNullOrEmpty(_GRAPHDBEXEPATH))
		//	{
		//		// Find GraphDB executable 
		//		_GRAPHDBEXEPATH = FindExecutable();
		//	}

		//	// Open Login Windows Form
		//	//bool isRunningLogin = Process.GetProcessesByName("GraphDB_WindowsForms").Any();
		//	bool isRunningLogin = Process.GetProcesses().Any(p => p.ProcessName.Contains("GraphDB_WindowsForms"));

		//	// Check 
		//	bool jsonWithUsername = File.Exists(ConstructJSONPath(_SERVERADRESS, _USERACCOUNT));

		//	// Start default root GraphDB executable
		//	if (!isRunningLogin && !jsonWithUsername)
		//	{
		//		string executablePath = @"C:\ProgramData\BHoM\Assemblies\GraphDB_WindowsForms.exe";
		//		Process.Start(executablePath);
		//	}

		//	bool isRunning = Process.GetProcesses().Any(p => p.ProcessName.Contains("GraphDB"));
		//	if (!isRunning)
		//	{
		//		return StartGraphDBProcess(_GRAPHDBEXEPATH);
		//	}

		//	return $"isRunningLogin: {isRunningLogin}\njsonWithUsername: {jsonWithUsername}\nexecutablePath: {executablePath}";
		//}

		///// <summary>
		///// Find GraphDB executable .exe on machine
		///// </summary>
		///// <param name="folderNameToSearch"></param>
		///// <returns></returns>
		//public static string FindExecutable()
		//{
		//	string folderNameToSearch = "GraphDB";
		//	string exeFileExtension = ".exe";

		//	// Get the path to the LocalAppData folder for the current user
		//	string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

		//	// Get all subdirectories in the LocalAppData folder
		//	string[] subdirectories = Directory.GetDirectories(localAppDataPath);

		//	// Search for the target folder
		//	string targetFolderPath = null;
		//	foreach (string subdirectory in subdirectories)
		//	{
		//		if (Path.GetFileName(subdirectory).IndexOf(folderNameToSearch, StringComparison.OrdinalIgnoreCase) >= 0)
		//		{
		//			Console.WriteLine($"Found the target folder: {subdirectory}");
		//			targetFolderPath = subdirectory;
		//			break;
		//		}
		//	}

		//	if (targetFolderPath != null)
		//	{
		//		// Get all files in the target folder
		//		string[] files = Directory.GetFiles(targetFolderPath);

		//		// Search for the exe file in the target folder
		//		foreach (string file in files)
		//		{
		//			if (Path.GetExtension(file).Equals(exeFileExtension, StringComparison.OrdinalIgnoreCase))
		//			{
		//				// Return the found executable path
		//				return file;
		//			}
		//		}

		//		// If no exe file is found
		//		return $"No exe files found in the folder '{folderNameToSearch}'.";
		//	}
		//	else
		//	{
		//		// If the target folder is not found
		//		return $"Folder with the name '{folderNameToSearch}' not found.";
		//	};
		//}

		//public string RetrievePassword(string serverAddress, string username)
		//{

		//	SecureStorage secureStorage = new SecureStorage();

		//	// Check if combination of server and username is already stored, if so return password for API interactions
		//	string jsonFilePath = ConstructJSONPath(serverAddress, username);
		//	if (File.Exists(jsonFilePath))
		//		return secureStorage.GetCredentials(jsonFilePath, username);

		//	// if not return null and login is called after 
		//	return null;
		//}

		//public static string MakeValidFileName(string name, string username)
		//{
		//	string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
		//	string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

		//	return System.Text.RegularExpressions.Regex.Replace(name + " " + username, invalidRegStr, "_");
		//}

		//public static string ConstructJSONPath(string serverAddress, string username)
		//{
		//	string potentialJsonFile = $"{MakeValidFileName(serverAddress, username)}.json";
		//	return Path.Combine(Path.GetTempPath(), potentialJsonFile); //C:\Users\User\AppData\Local\Temp + Filename
		//}

		//public static string StartGraphDBProcess(string graphDBexePath = "%APPDATA%\\Local\\GraphDB Free\\GraphDB Free.exe")
		//{
		//	if (!System.IO.File.Exists(graphDBexePath))
		//	{
		//		Console.WriteLine($"Could not find the {nameof(graphDBexePath)}. Please make sure that the path to the GraphDB executable file is correct.", typeof(ArgumentException));
		//		return $"Could not find the {nameof(graphDBexePath)}. Please make sure that the path to the GraphDB executable file is correct.";
		//	}

		//	else
		//	{
		//		Process.Start(graphDBexePath);
		//		//Task.Delay(500).Wait();
		//		return $"Process started at: {graphDBexePath}";
		//	}
		//}
		#endregion
	}
}
