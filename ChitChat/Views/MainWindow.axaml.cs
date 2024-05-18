using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ChitChat.Services;
using ChitChat.ViewModels;
using Serilog;
using Supabase.Gotrue;

namespace ChitChat.Views;

public partial class MainWindow : Window
{
    private bool registerMode = false;

	private readonly UserService userService = UserService.Instance;
	private readonly SupabaseService supabaseHandler = SupabaseService.Instance;

	public MainWindow() {
		InitializeComponent();

		// Set version
		versionLabel.Content = $"Version: {UpdateService.GetCurrentVersion()}";
	}


	/// <summary>
	/// Clears input fields
	/// </summary>
	private void ClearInput() {
		email.Text = string.Empty;
		password.Text = string.Empty;
		confirmPassword.Text = string.Empty;
		(DataContext as MainWindowViewModel)?.ClearData();
	}

	/// <summary>
	/// Hides registration form and shows login form
	/// </summary>
	private void ShowLoginForm() {
		usernameLabel.IsVisible = false;
		username.IsVisible = false;
		confirmPasswordLabel.IsVisible = false;
		confirmPassword.IsVisible = false;
		cancelRegistrationButton.IsVisible = false;
		loginButton.IsVisible = true;
		registerMode = false;
		ClearInput();
	}

	/// <summary>
	/// Shows registration form and hides login form
	/// </summary>
	private void ShowRegistrationForm() {
		usernameLabel.IsVisible = true;
		username.IsVisible = true;
		confirmPasswordLabel.IsVisible = true;
		confirmPassword.IsVisible = true;
		cancelRegistrationButton.IsVisible = true;
		loginButton.IsVisible = false;
		registerMode = true;
		ClearInput();
	}

	async private void registerButton_Click(object sender, RoutedEventArgs e) {
		try {
			if (registerMode) {
				// Check if username, email, and password are not empty
				if (!(DataContext as MainWindowViewModel)?.IsRegisterValid() == true) {
					//MessageBox.Show("All fields are required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}

				// Register user
				if ((DataContext as MainWindowViewModel)?.IsLoginValid() == true) {
					await userService.RegisterUser((DataContext as MainWindowViewModel)?.GetUser(), password.Text);

					//MessageBox.Show("User registered successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

					// Clear form
					ShowLoginForm();

				} else {
					//MessageBox.Show("Passwords do not match", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			} else {
				// Show registration form
				ShowRegistrationForm();
			}
		} catch (Exception ex) {
			// Show the error to the user
			//MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}

	private void cancelRegistrationButton_Click(object sender, RoutedEventArgs e) {
		ShowLoginForm();
	}

	async private void loginButton_Click(object sender, RoutedEventArgs e) {
		// Check if email and password are not empty
		if (!(DataContext as MainWindowViewModel)?.IsLoginValid() == true) {
			//MessageBox.Show("Email and password are required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		// Login user
		try {
			Session? session = await supabaseHandler.Client.Auth.SignIn(email.Text, password.Text);

			if (session != null) {
				// Set user online
				await userService.SetUserOnline(true);

				//var chatWindow = new ChatWindow();
				//chatWindow.Show();
				Close();
			} else {
				Log.Error($"Error logging in the user {(DataContext as MainWindowViewModel)?.GetDebugInfo()}");
				//MessageBox.Show("Invalid email or password", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		} catch (Exception ex) {
			//MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}
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
			//MessageBox.Show("There is a new version available, run launcher to update", "Update", MessageBoxButton.OK, MessageBoxImage.Information);
		} else if (notify) {
			//MessageBox.Show("No updates available", "Update", MessageBoxButton.OK, MessageBoxImage.Information);
		}
	}

	private void releaseNotesButton_Click(object sender, RoutedEventArgs e) {
		//ReleaseNotesWindow releaseNotesWindow = new();
		//releaseNotesWindow.ShowDialog();
	}

	private async void Window_Opened(object sender, EventArgs e) {
		// Check for updates
		await CheckForUpdates();
	}
}