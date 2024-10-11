using System;
using System.Windows.Forms;

namespace GraphWebsite.WindowsFroms
{
	public class WindowsFramesCredentials
	{
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
	}
}
