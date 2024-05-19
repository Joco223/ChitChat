using Avalonia.Controls;
using Avalonia.Interactivity;
using ChitChat.DatabaseModels;
using ChitChat.Services;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace ChitChat.Views {
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
			var result = await serverService.UpdateServer(CurrentServer);

			if (result.Failed) {
				// Show error message
				var errorNotification = MessageBoxManager.GetMessageBoxStandard("Error", "Error updating the server", ButtonEnum.Ok);
				await errorNotification.ShowWindowDialogAsync(this);
				return;
			}
		}
	}
}
