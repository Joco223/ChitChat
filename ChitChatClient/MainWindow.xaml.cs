using System.Windows;

using ChitChatClient.Helpers;
using ChitChatClient.Services;
using ChitChatClient.ViewModels;
using ChitChatClient.Windows;

using Serilog;

using Supabase.Gotrue;

namespace ChitChatClient {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		private bool registerMode = false;

		private readonly UserService userService = UserService.Instance;
		private readonly SupabaseHandler supabaseHandler = SupabaseHandler.Instance;

		public RegisterUser registerUser = new();

		public MainWindow() {
			InitializeComponent();
			DataContext = registerUser;

			// Set version
			versionLabel.Content = $"Version: {UpdateService.GetCurrentVersion()}";
		}


		/// <summary>
		/// Clears input fields
		/// </summary>
		private void ClearInput() {
			email.Text = string.Empty;
			password.Password = string.Empty;
			confirmPassword.Password = string.Empty;
			registerUser.ClearData();
		}

		/// <summary>
		/// Hides registration form and shows login form
		/// </summary>
		private void ShowLoginForm() {
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
		private void ShowRegistrationForm() {
			usernameLabel.Visibility = Visibility.Visible;
			username.Visibility = Visibility.Visible;
			confirmPasswordLabel.Visibility = Visibility.Visible;
			confirmPassword.Visibility = Visibility.Visible;
			cancelRegistrationButton.Visibility = Visibility.Visible;
			loginButton.Visibility = Visibility.Collapsed;
			registerMode = true;
			ClearInput();
		}

		async private void registerButton_Click(object sender, RoutedEventArgs e) {
			try {
				if (registerMode) {
					// Check if username, email, and password are not empty
					if (!registerUser.IsRegisterValid()) {
						MessageBox.Show("All fields are required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
						return;
					}

					// Register user
					if (registerUser.PasswordsMatch()) {
						await userService.RegisterUser(registerUser);

						MessageBox.Show("User registered successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

						// Clear form
						ShowLoginForm();

					} else {
						MessageBox.Show("Passwords do not match", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				} else {
					// Show registration form
					ShowRegistrationForm();
				}
			} catch (Exception ex) {
				// Show the error to the user
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void cancelRegistrationButton_Click(object sender, RoutedEventArgs e) {
			ShowLoginForm();
		}

		async private void loginButton_Click(object sender, RoutedEventArgs e) {
			// Check if email and password are not empty
			if (!registerUser.IsLoginValid()) {
				MessageBox.Show("Email and password are required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			// Login user
			try {
				Session? session = await supabaseHandler.Client.Auth.SignIn(email.Text, password.Password);

				if (session != null) {
					// Set user online
					await userService.SetUserOnline(true);

					var chatWindow = new ChatWindow();
					chatWindow.Show();
					Close();
				} else {
					Log.Error($"Error logging in the user {registerUser.GetDebugInfo()}");
					MessageBox.Show("Invalid email or password", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			} catch (Exception ex) {
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void password_PasswordChanged(object sender, RoutedEventArgs e) {
			registerUser.Password = password.Password;
		}

		private void confirmPassword_PasswordChanged(object sender, RoutedEventArgs e) {
			registerUser.ConfirmPassword = confirmPassword.Password;
		}

		private async void Window_ContentRendered(object sender, EventArgs e) {
			// Check for updates
			await CheckForUpdates();
		}

		private async void checkForUpdatesButton_Click(object sender, RoutedEventArgs e) {
			// Check for updates
			await CheckForUpdates(true);
		}

		/// <summary>
		/// Checks for updates and notifies the user if there is an update available
		/// </summary>
		/// <param name="notify">Should there be a notification where no updates are available</param>
		/// <returns></returns>
		private async Task CheckForUpdates(bool notify = false) {
			if (await UpdateService.CheckForUpdates()) {
				MessageBox.Show("There is a new version available, run launcher to update", "Update", MessageBoxButton.OK, MessageBoxImage.Information);
			} else if (notify) {
				MessageBox.Show("No updates available", "Update", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}
	}
}
