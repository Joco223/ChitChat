using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ChitChatClient.Services;
using ChitChatClient.ViewModels;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace ChitChatClient.Views;

public partial class MainWindow : Window
{
	private bool registerMode = false;

	private readonly UserService userService = UserService.Instance;

	private MainWindowViewModel context = new();

	public MainWindow() {
		InitializeComponent();

		// Set version
		versionLabel.Content = $"Version: {UpdateService.GetCurrentVersion()}";
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
		context?.ClearData();
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
		context?.ClearData();
	}

	async private void registerButton_Click(object sender, RoutedEventArgs e) {
		if (registerMode) {
			// Check if username, email, and password are not empty
			if (!context.IsRegisterValid()) {
				var errorNotification = MessageBoxManager.GetMessageBoxStandard("Error", "Username, email, and password are required", ButtonEnum.Ok);
				await errorNotification.ShowWindowDialogAsync(this);
				return;
			}

			// Register user
			if (context.IsLoginValid()) {
				await userService.RegisterUser(context.GetUser(), context.Password);

				// Inform user registration was sucesfull
				var successNotification = MessageBoxManager.GetMessageBoxStandard("Success", "User registered successfully", ButtonEnum.Ok);
				await successNotification.ShowWindowDialogAsync(this);

				// Clear form
				ShowLoginForm();

			} else {
				// Inform user that passwords do not match
				var errorNotification = MessageBoxManager.GetMessageBoxStandard("Error", "Passwords do not match", ButtonEnum.Ok);
				await errorNotification.ShowWindowDialogAsync(this);
			}
		} else {
			// Show registration form
			ShowRegistrationForm();
		}
	}

	private void cancelRegistrationButton_Click(object sender, RoutedEventArgs e) {
		ShowLoginForm();
	}

	private async void loginButton_Click(object sender, RoutedEventArgs e) {
		// Check if email and password are not empty
		if (!context.IsLoginValid()) {
			var errorNotification = MessageBoxManager.GetMessageBoxStandard("Error", "Email and password are required", ButtonEnum.Ok);
			await errorNotification.ShowWindowDialogAsync(this);
			return;
		}

		// Login user
		var loginResult = await userService.LogInUser(context.Email, context.Password);

		// Check if session is null
		if (loginResult.Failed) {
			var errorNotification = MessageBoxManager.GetMessageBoxStandard("Error", "Invalid email or password", ButtonEnum.Ok);
			await errorNotification.ShowWindowDialogAsync(this);
			return;
		}

		// Set user online
		var setOnlineResult = await userService.SetUserOnlineStatus(true);

		// Check if user is online
		if (setOnlineResult.Failed) {
			var errorNotification = MessageBoxManager.GetMessageBoxStandard("Error", "Failed to set user online", ButtonEnum.Ok);
			await errorNotification.ShowWindowDialogAsync(this);
			return;
		}

		var chatWindow = new ChatWindow();
		chatWindow.Show();
		Close();
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
			var updateNotification = MessageBoxManager.GetMessageBoxStandard("Update", "There is a new version available, run launcher to update", ButtonEnum.Ok);
			await updateNotification.ShowWindowDialogAsync(this);
		} else if (notify) {
			var noUpdateNotification = MessageBoxManager.GetMessageBoxStandard("Update", "No updates available", ButtonEnum.Ok);
			await noUpdateNotification.ShowWindowDialogAsync(this);
		}
	}

	private async void releaseNotesButton_Click(object sender, RoutedEventArgs e) {
		ReleaseNotesWindow releaseNotesWindow = new();
		await releaseNotesWindow.ShowDialog(this);
	}

	private async void Window_Opened(object sender, EventArgs e) {
		// Set context
		context = (MainWindowViewModel)DataContext!;

		// Check for updates
		await CheckForUpdates();
	}
}