using ChitChatClient.Helpers;
using ChitChatClient.Services;
using ChitChatClient.ViewModels;
using ChitChatClient.Windows;
using Supabase.Gotrue;
using System.Reflection;
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

namespace ChitChatClient
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private bool registerMode = false;

		private readonly UserService userService = UserService.Instance;
		private readonly SupabaseHandler supabaseHandler = SupabaseHandler.Instance;
		private readonly UpdateService updateService = UpdateService.Instance;

		public RegisterUser registerUser = new();


		public MainWindow()
		{
			InitializeComponent();
			DataContext = registerUser;

			// Set version
			string? version = Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString();
			versionLabel.Content = $"Version: {version}";
		}


		/// <summary>
		/// Clears input fields
		/// </summary>
		private void ClearInput()
		{
			email.Text = "";
            password.Password = "";
            confirmPassword.Password = "";
            registerUser.ClearData();
		}

		/// <summary>
		/// Hides registration form and shows login form
		/// </summary>
		private void ShowLoginForm()
		{
            usernameLabel.Visibility = Visibility.Collapsed;
            username.Visibility = Visibility.Collapsed;
            confirmPasswordLabel.Visibility = Visibility.Collapsed;
            confirmPassword.Visibility = Visibility.Collapsed;
            cancelRegistrationButton.Visibility = Visibility.Collapsed;
            loginButton.Visibility = Visibility.Visible;
            registerMode = false;
			ClearInput();
        }

		/// <summary>
		/// Shows registration form and hides login form
		/// </summary>
		private void ShowRegistrationForm()
		{
            usernameLabel.Visibility = Visibility.Visible;
            username.Visibility = Visibility.Visible;
            confirmPasswordLabel.Visibility = Visibility.Visible;
            confirmPassword.Visibility = Visibility.Visible;
            cancelRegistrationButton.Visibility = Visibility.Visible;
            loginButton.Visibility = Visibility.Collapsed;
            registerMode = true;
            ClearInput();
        }

		async private void registerButton_Click(object sender, RoutedEventArgs e)
		{
			if (registerMode)
			{
				// Check if username, email, and password are not empty
				if (!registerUser.IsRegisterValid())
				{
					MessageBox.Show("All fields are required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Register user
				if (registerUser.PasswordsMatch())
				{
					bool result = await userService.RegisterUser(registerUser);

					if (result)
					{
                        MessageBox.Show("User registered successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Clear form
                        ShowLoginForm();
                    }
                    else
					{
                        MessageBox.Show("User registration failed", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
				ShowRegistrationForm();
            }
        }

		private void cancelRegistrationButton_Click(object sender, RoutedEventArgs e)
		{
			ShowLoginForm();
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
			try
			{
                Session? session = await supabaseHandler.Client.Auth.SignIn(email.Text, password.Password);

                if (session != null)
                {
					var loginResult = await userService.SetUserOnline(true);

					if (loginResult)
					{
						var chatWindow = new ChatWindow();
						chatWindow.Show();
						Close();
					}
					else
					{
						MessageBox.Show("Login failed", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					}
                }
            }
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
        }

		private void password_PasswordChanged(object sender, RoutedEventArgs e)
		{
			registerUser.Password = password.Password;
        }

		private void confirmPassword_PasswordChanged(object sender, RoutedEventArgs e)
		{
			registerUser.ConfirmPassword = confirmPassword.Password;
        }

		private async void Window_ContentRendered(object sender, EventArgs e)
		{
			// Check for updates
			await CheckForUpdates();
        }

		private async void checkForUpdatesButton_Click(object sender, RoutedEventArgs e)
		{
			await CheckForUpdates(true);
        }

		/// <summary>
		/// Checks for updates and notifies the user if there is an update available
		/// </summary>
		/// <param name="notify">Should there be a notification where no updates are available</param>
		/// <returns></returns>
		private async Task CheckForUpdates(bool notify = false)
		{
			if (await updateService.CheckForUpdates())
			{
				MessageBox.Show("There is a new version available, run launcher to update", "Update", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			else if (notify)
			{
				MessageBox.Show("No updates available", "Update", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}
    }
}
