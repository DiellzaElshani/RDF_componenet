using System.Windows.Forms;

namespace GraphWebsite.WindowsFroms
{
	public class Login : Form
	{
		// Public properties to access the user input data from the component
		public string ServerAddress { get; private set; }
		public string Username { get; private set; }
		public string Password { get; private set; }

		// Controls for the form
		private TextBox usernameTextBox;
		private TextBox passwordTextBox;
		private TextBox serverAddressTextBox;


		public Login()
		{

			// Set up the form properties
			this.Text = "Host server login";
			this.Width = 400;
			this.Height = 200;

			// Create the controls for username and password input
			Label usernameLabel = new Label()
			{
				Text = "Username:",
				Left = 20,
				Top = 20,
				Width = 100
			};

			TextBox usernameTextBox = new TextBox()
			{
				Left = 120,
				Top = 20,
				Width = 200
			};

			Label passwordLabel = new Label()
			{
				Text = "Password:",
				Left = 20,
				Top = 50,
				Width = 100
			};

			TextBox passwordTextBox = new TextBox()
			{
				Left = 120,
				Top = 50,
				Width = 200,
				PasswordChar = '*' // Show asterisks for password input
			};

			Label serverAddressLabel = new Label()
			{
				Text = "Server address:",
				Left = 20,
				Top = 80,
				Width = 100
			};

			TextBox serverAddressTextBox = new TextBox()
			{
				Left = 120,
				Top = 80,
				Width = 200
			};

			Button button = new Button()
			{
				Text = "OK",
				Left = 120,
				Top = 110,
				Width = 100
			};

			// Add the controls to the form
			this.Controls.Add(usernameLabel);
			this.Controls.Add(usernameTextBox);
			this.Controls.Add(passwordLabel);
			this.Controls.Add(passwordTextBox);
			this.Controls.Add(serverAddressLabel);
			this.Controls.Add(serverAddressTextBox);
			this.Controls.Add(button);

			// Attach the button's click event handler
			button.Click += (sender, e) =>
			{
				// Access the user's input data here
				Username = usernameTextBox.Text;
				Password = passwordTextBox.Text;
				ServerAddress = serverAddressTextBox.Text;

				// Close the form with a result indicating the "OK" button was clicked
				this.DialogResult = DialogResult.OK;
				this.Close();
			};
		}
	}
}
