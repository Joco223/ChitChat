using ChitChat.Helpers;
using Supabase.Gotrue;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChitChat
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private bool registerMode = false;
		private SupabaseHandler supabaseHandler = SupabaseHandler.GetHandler();

		public MainWindow()
		{
			InitializeComponent();
		}

		async private void registerButton_Click(object sender, RoutedEventArgs e)
		{
			if (registerMode)
			{
				// Check if username, email, and password are not empty
				if (string.IsNullOrEmpty(username.Text) || string.IsNullOrEmpty(email.Text) || string.IsNullOrEmpty(password.Password))
				{
					MessageBox.Show("Username, email, and password are required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Check if email is valid


                // Register user
				if (password.Password == confirmPassword.Password)
				{
					Dictionary<string, object> data = new()
					{
						{ "username", username.Text }
					};

					SignUpOptions options = new Supabase.Gotrue.SignUpOptions()
					{ 
						Data = data
					};

					try
					{
                        Session? session = await supabaseHandler.Client.Auth.SignUp(email.Text, password.Password, options);

                        if (session != null)
                        {
                            MessageBox.Show("User registered successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            usernameLabel.Visibility = Visibility.Collapsed;
                            username.Visibility = Visibility.Collapsed;
                            confirmPasswordLabel.Visibility = Visibility.Collapsed;
                            confirmPassword.Visibility = Visibility.Collapsed;
                            cancelRegistrationButton.Visibility = Visibility.Collapsed;
                            loginButton.Visibility = Visibility.Visible;
							email.Text = "";
							password.Password = "";
                            registerMode = false;
                        }
                        else
                        {
                            MessageBox.Show("Failed to register user", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
					catch (Exception ex)
					{
                        MessageBox.Show("Failed to register user, " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        throw;
					}
				}
				else
				{
					MessageBox.Show("Passwords do not match", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
            }
            else
			{
                // Show registration form
				usernameLabel.Visibility = Visibility.Visible;
				username.Visibility = Visibility.Visible;
				confirmPasswordLabel.Visibility = Visibility.Visible;
				confirmPassword.Visibility = Visibility.Visible;
				cancelRegistrationButton.Visibility = Visibility.Visible;
				loginButton.Visibility = Visibility.Collapsed;
				registerMode = true;
            }
        }

		private void cancelRegistrationButton_Click(object sender, RoutedEventArgs e)
		{
			usernameLabel.Visibility = Visibility.Collapsed;
            username.Visibility = Visibility.Collapsed;
			confirmPasswordLabel.Visibility = Visibility.Collapsed;
            confirmPassword.Visibility = Visibility.Collapsed;
            cancelRegistrationButton.Visibility = Visibility.Collapsed;
            loginButton.Visibility = Visibility.Visible;
			registerMode = false;
        }

		async private void loginButton_Click(object sender, RoutedEventArgs e)
		{
			// Check if email and password are not empty
			if (string.IsNullOrEmpty(email.Text) || string.IsNullOrEmpty(password.Password))
			{
				MessageBox.Show("Email and password are required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Login user
			Session? session = await supabaseHandler.Client.Auth.SignIn(email.Text, password.Password);

			if (session != null)
			{
				MessageBox.Show("User logged in successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}