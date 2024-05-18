
using Avalonia.Controls;
using Avalonia.Interactivity;
using ChitChat.Services;
using ChitChat.ViewModels;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace ChitChat.Views {
	/// <summary>
	/// Interaction logic for CreateServerWindow.xaml
	/// </summary>
	public partial class CreateServerWindow : Window {
		private CreateServer CreateServer = new();

		private readonly ServerService serverService = ServerService.Instance;

		public CreateServerWindow() {
			InitializeComponent();
			DataContext = CreateServer;
		}

		/// <summary>
		/// Checks if the input is valid and enables the create server button
		/// </summary>
		private void IsInputValid() => createServerButton.IsEnabled = !string.IsNullOrEmpty(CreateServer.Name) && !string.IsNullOrEmpty(CreateServer.Description);

		async private void createServerButton_Click(object sender, RoutedEventArgs e) {
			// Create server
			var result = await serverService.CreateServer(CreateServer.Name, CreateServer.Description);

			if (result.Failed) {
				var errorNotification = MessageBoxManager.GetMessageBoxStandard("Error", "Error creating the server", ButtonEnum.Ok);
				await errorNotification.ShowWindowDialogAsync(this);
			}

			var successNotification = MessageBoxManager.GetMessageBoxStandard("Success", "Server created successfully", ButtonEnum.Ok);
			await successNotification.ShowWindowDialogAsync(this);

			Close();
		}

		private void cancelButton_Click(object sender, RoutedEventArgs e) {
			Close();
		}

		private void serverNameTextBox_TextChanged(object sender, TextChangedEventArgs e) {
			IsInputValid();
		}

		private void serverDescriptionTextBox_TextChanged(object sender, TextChangedEventArgs e) {
			IsInputValid();
		}
	}
}
