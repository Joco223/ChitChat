using ChitChat.Helpers;
using ChitChat.Services;
using ChitChat.ViewModels;
using ChitChat.Windows;
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

		private readonly UserService userService = UserService.Instance;
		private readonly SupabaseHandler supabaseHandler = SupabaseHandler.Instance;
		private readonly AppInfoService appInfoService = AppInfoService.Instance;

		public RegisterUser registerUser = new();


		public MainWindow()
		{
			InitializeComponent();
			DataContext = registerUser;

			// Delete old version of app if it exists
			appInfoService.DeleteOldVersion();

			versionLabel.Content = $"Version: {Properties.Settings.Default["AppVersion"]}";
		}

		private void clearInput()
		{
			email.Text = "";
            password.Password = "";
            confirmPassword.Password = "";
            registerUser.ClearData();
		}

		private void showLoginForm()
		{
            usernameLabel.Visibility = Visibility.Collapsed;
            username.Visibility = Visibility.Collapsed;
            confirmPasswordLabel.Visibility = Visibility.Collapsed;
            confirmPassword.Visibility = Visibility.Collapsed;
            cancelRegistrationButton.Visibility = Visibility.Collapsed;
            loginButton.Visibility = Visibility.Visible;
            registerMode = false;
			clearInput();
        }

		private void showRegistrationForm()
		{
            usernameLabel.Visibility = Visibility.Visible;
            username.Visibility = Visibility.Visible;
            confirmPasswordLabel.Visibility = Visibility.Visible;
            confirmPassword.Visibility = Visibility.Visible;
            cancelRegistrationButton.Visibility = Visibility.Visible;
            loginButton.Visibility = Visibility.Collapsed;
            registerMode = true;
            clearInput();
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

                // Check if email is valid
				// Implement regex check if email is valid
				//if (!registerUser.IsEmailValid())
				//{

				//}

                // Register user
				if (registerUser.PasswordsMatch())
				{
					bool result = await userService.RegisterUser(registerUser);

					if (result)
					{
                        MessageBox.Show("User registered successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Clear form
                        showLoginForm();
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
				showRegistrationForm();
            }
        }

		private void cancelRegistrationButton_Click(object sender, RoutedEventArgs e)
		{
			showLoginForm();
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
					// Implement redirect to Servers window
                    var chatWindow = new ChatWindow();
					chatWindow.Show();
					this.Close();
					
					//MessageBox.Show("User logged in successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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
			try
			{
				bool latest = await appInfoService.CheckForLatestVersion();

				if (!latest)
				{
                    MessageBox.Show("New version available. Click OK to update.", "Update", MessageBoxButton.OK, MessageBoxImage.Information);
					//await appInfoService.UpdateApp();

					downloadProgressLabel.Visibility = Visibility.Visible;
					downloadProgressBar.Visibility = Visibility.Visible;

					loginButton.IsEnabled = false;
					registerButton.IsEnabled = false;
					email.IsEnabled = false;
					password.IsEnabled = false;

					await appInfoService.UpdateApp(UpdateDownloadProgressBar);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				// Exit the app
				Application.Current.Shutdown();
			}
        }

		private void UpdateDownloadProgressBar(float progressPercentage)
		{
			downloadProgressLabel.Content = "Downloading: " + Math.Round(progressPercentage) + "%";
			Console.WriteLine(progressPercentage);
			downloadProgressBar.Value = progressPercentage;
		}
    }
}