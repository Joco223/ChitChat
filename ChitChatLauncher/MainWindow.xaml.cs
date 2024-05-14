using System.Windows;

using ChitChatLauncher.Services;

namespace ChitChatLauncher {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {

		private readonly UpdateService updateService = UpdateService.Instance;

		public MainWindow() {
			InitializeComponent();
		}

		private async void Window_Loaded(object sender, RoutedEventArgs e) {
			try {
				await updateService.UpdateApp(UpdateDownloadProgressBar, UpdateNotification);

				System.Diagnostics.Process.Start(updateService.GetAppPath());
				Application.Current.Shutdown();
			} catch (Exception ex) {
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void UpdateNotification(string notification) {
			downloadProgressLabel.Content = notification;
		}

		private void UpdateDownloadProgressBar(float progressPercentage, string fileName) {
			downloadProgressPercentLabel.Content = Math.Round(progressPercentage) + "%";
			downloadProgressBar.Value = progressPercentage;
			downloadProgressBar.Visibility = Visibility.Visible;
			downloadProgressPercentLabel.Visibility = Visibility.Visible;
		}
	}
}