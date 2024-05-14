using System.Windows;

using ChitChatClient.Services;
using ChitChatClient.ViewModels;

namespace ChitChatClient.Windows {
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
			try {
				// Create server
				await serverService.CreateServer(CreateServer.Name, CreateServer.Description);

				MessageBox.Show("Server created successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
				Close();
			} catch (Exception ex) {
				// Show user the error
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void cancelButton_Click(object sender, RoutedEventArgs e) {
			Close();
		}

		private void serverNameTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) {
			IsInputValid();
		}

		private void serverDescriptionTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) {
			IsInputValid();
		}
	}
}
