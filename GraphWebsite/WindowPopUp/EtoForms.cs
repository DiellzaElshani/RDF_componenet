using Eto.Drawing;
using Eto.Forms;

namespace GraphWebsite.WindowPopUp
{
	public class CredentialsDialog : Dialog<DialogResult>
	{
		// Declare properties for server address, username, and password
		public string ServerAddress { get; private set; }
		public string Username { get; private set; }
		public string Password { get; private set; }

		public CredentialsDialog()
		{
			// Textboxes for user input
			var serverAddressBox = new TextBox { PlaceholderText = "Enter server address", Width = 300 };
			var usernameBox = new TextBox { PlaceholderText = "Enter username", Width = 300 };
			var passwordBox = new PasswordBox { PasswordChar = '*', Width = 300 };

			// StackLayout for organizing the form elements
			var layout = new StackLayout
			{
				Orientation = Orientation.Vertical,
				Spacing = 10,
				Padding = new Padding(10),
				Items =
			{
				new Label { Text = "Server Address:" },
				serverAddressBox,
				new Label { Text = "Username:" },
				usernameBox,
				new Label { Text = "Password:" },
				passwordBox,
				new StackLayout
				{
					Orientation = Orientation.Horizontal,
					Spacing = 10,
					Items =
					{
						new Button
						{
							Text = "OK",
							Command = new Command((sender, e) => {
                                // Set properties based on input
                                ServerAddress = serverAddressBox.Text;
								Username = usernameBox.Text;
								Password = passwordBox.Text;
								Result = DialogResult.Ok;
								Close();
							})
						},
						new Button
						{
							Text = "Cancel",
							Command = new Command((sender, e) => {
								Result = DialogResult.Cancel;
								Close();
							})
						}
					}
				}
			}
			};

			// Set dialog content to the layout
			Content = layout;  
		}
	}
}