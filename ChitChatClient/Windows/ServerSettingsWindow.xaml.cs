using System.Windows;

using ChitChatClient.Models;
using ChitChatClient.Services;

namespace ChitChatClient.Windows {
	/// <summary>
	/// Interaction logic for ServerSettingsWindow.xaml
	/// </summary>
	public partial class ServerSettingsWindow : Window {
		private readonly ServerService serverService = ServerService.Instance;
		private Server currentServer;

		public Server CurrentServer { get => currentServer; set => currentServer = value; }

		public ServerSettingsWindow() {
			InitializeComponent();
			currentServer = new();
		}

		public ServerSettingsWindow(Server server) : base() {
			InitializeComponent();
			DataContext = this;

			currentServer = server;
		}

		private async void saveServerButton_Click(object sender, RoutedEventArgs e) {
			try {
				await serverService.UpdateServer(CurrentServer);
			} catch (Exception ex) {
				// Show the error to the user
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}
