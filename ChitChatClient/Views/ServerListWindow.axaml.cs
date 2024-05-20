using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ChitChatClient.Helpers;
using ChitChatClient.DatabaseModels;
using ChitChatClient.Services;
using ChitChatClient.ViewModels;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using ChitChatClient.Models;

namespace ChitChatClient.Views {
	/// <summary>
	/// Interaction logic for ServerListWindow.xaml
	/// </summary>
	public partial class ServerListWindow : Window {
		private readonly ServerService serverService = ServerService.Instance;
		public ServerList serverList = new();

		public ServerListWindow() {
			InitializeComponent();
			DataContext = serverList;
		}

		/// <summary>
		/// Refreshes the server list
		/// </summary>
		/// <returns></returns>
		private async Task<Result<string>> RefreshServers() {
			// Get server options
			var result = await serverList.GetServerOptions();

			if (result.Failed) {
				var errorNotification = MessageBoxManager.GetMessageBoxStandard("Error", "Error loading servers", ButtonEnum.Ok);
				await errorNotification.ShowWindowDialogAsync(this);
			}

			return Result<string>.OK("Success");

			// Update the server list view and reset the selected index
			//serverListView.GetBindingExpression(ListView.ItemsSourceProperty).UpdateTarget();
			//serverListView.SelectedIndex = -1;
		}

		private void serverSearchTextBox_TextChanged(object sender, TextChangedEventArgs e) {
			// Refresh the server list view
			// if (serverListView.ItemsSource != null) {
			// 	CollectionViewSource.GetDefaultView(serverListView.ItemsSource).Refresh();
			// }
		}

		/// <summary>
		/// Filters the server list based on the serverSearchTextBox text
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		private bool FilterServer(object obj) {
			// If the search text is empty, return true
			if (string.IsNullOrEmpty(serverSearchTextBox.Text)) {
				return true;
			}

			// If the search text is the placeholder, return true
			if (serverSearchTextBox.Text == serverSearchTextBox.Watermark) {
				return true;
			}

			// If the server name contains the search text, return true
			if (obj is ServerOption serverOption) {
				if (serverOption.Server.Name.ToLower().Contains(serverSearchTextBox.Text.ToLower())) {
					return true;
				}
			}

			return false;
		}

		private async void Window_Loaded(object sender, RoutedEventArgs e) {
			// Refresh the server list
			var result = await RefreshServers();

			if (result.Failed) {
				var errorNotification = MessageBoxManager.GetMessageBoxStandard("Error", "Error loading servers", ButtonEnum.Ok);
				await errorNotification.ShowWindowDialogAsync(this);
				return;
			}

			// Set the filter callback for the server list view
			//CollectionViewSource.GetDefaultView(serverListView.ItemsSource).Filter = FilterServer;
		}

		private void cancelButton_Click(object sender, RoutedEventArgs e) {
			Close();
		}

		/// <summary>
		/// Joins a server
		/// </summary>
		/// <returns></returns>
		private async Task JoinServer() {
			ServerOption selectedServerOption = (ServerOption)serverListView.SelectedItem;
			Server selectedServer = selectedServerOption.Server;

			// If the server is already joined, show an error
			if (selectedServerOption.Joined) {
				// Show an error message
				var errorNotification = MessageBoxManager.GetMessageBoxStandard("Error", "You have already joined this server", ButtonEnum.Ok);
				await errorNotification.ShowWindowDialogAsync(this);
			} else {
				var result = await serverService.JoinServer(selectedServer);

				if (result.Failed) {
					// Show an error message
					var errorNotification = MessageBoxManager.GetMessageBoxStandard("Error", "Error joining the server", ButtonEnum.Ok);
					await errorNotification.ShowWindowDialogAsync(this);
					return;
				}

				// Show a success message
				var successNotification = MessageBoxManager.GetMessageBoxStandard("Success", "Server joined successfully", ButtonEnum.Ok);
				await successNotification.ShowWindowDialogAsync(this);

				Close();
			}
		}

		private async void joinButton_Click(object sender, RoutedEventArgs e) {
			await JoinServer();
		}

		private async void refreshButton_Click(object sender, RoutedEventArgs e) {
			await RefreshServers();
			serverListView.SelectedIndex = -1;
			joinButton.IsEnabled = false;
		}

		// private async void serverListView_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
		// 	// Get the mouse position
		// 	Point mousePosition = e.GetPosition((IInputElement)sender);

		// 	// Check if the mouse position is within the DataGridColumnHeader
		// 	DataGridColumnHeader? header = FindVisualParent<DataGridColumnHeader>(e.OriginalSource as DependencyObject);

		// 	// If the mouse position is not within the DataGridColumnHeader, join the server
		// 	if (header == null) {
		// 		await JoinServer();
		// 	}
		// 	e.Handled = true;
		// }

		// Helper method to find the DataGridColumnHeader at a given position
		// private T? FindVisualParent<T>(DependencyObject? child) where T : DependencyObject {
		// 	// Get the parent object
		// 	DependencyObject parentObject = VisualTreeHelper.GetParent(child);

		// 	// If the parent object is null, return null
		// 	if (parentObject == null)
		// 		return null;

		// 	//  If the parent object is of type T, return the parent object
		// 	if (parentObject is T parent)
		// 		return parent;

		// 	// Recursively call the method with the parent object
		// 	return FindVisualParent<T>(parentObject);
		// }

		// private void Expander_Expanded(object sender, RoutedEventArgs e) {
		// 	// Toggle the visibility of the details
		// 	for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual) {
		// 		if (vis is DataGridRow row) {
		// 			row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
		// 			break;
		// 		}
		// 	}
		// }

		// private void Expander_Collapsed(object sender, RoutedEventArgs e) {
		// 	// Toggle the visibility of the details
		// 	for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual) {
		// 		if (vis is DataGridRow row) {
		// 			row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
		// 			break;
		// 		}
		// 	}
		// }

		private void serverListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			// If the selected index is -1, disable the join button
			joinButton.IsEnabled = serverListView.SelectedIndex != -1;
		}
	}
}
