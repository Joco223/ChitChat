using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

using ChitChatClient.Models;
using ChitChatClient.Services;
using ChitChatClient.ViewModels;

namespace ChitChatClient.Windows {
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
		private async Task RefreshServers() {
			try {
				// Get server options
				await serverList.GetServerOptions();

				// Update the server list view and reset the selected index
				serverListView.GetBindingExpression(ListView.ItemsSourceProperty).UpdateTarget();
				serverListView.SelectedIndex = -1;
			} catch (Exception ex) {
				// Show the error to the user
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void serverSearchTextBox_TextChanged(object sender, TextChangedEventArgs e) {
			// Refresh the server list view
			if (serverListView.ItemsSource != null) {
				CollectionViewSource.GetDefaultView(serverListView.ItemsSource).Refresh();
			}
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
			if (serverSearchTextBox.Text == serverSearchTextBox.Placeholder) {
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
			try {
				// Refresh the server list
				await RefreshServers();

				// Set the filter callback for the server list view
				CollectionViewSource.GetDefaultView(serverListView.ItemsSource).Filter = FilterServer;
			} catch (Exception ex) {
				// Show user the error
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void cancelButton_Click(object sender, RoutedEventArgs e) {
			Close();
		}

		/// <summary>
		/// Joins a server
		/// </summary>
		/// <returns></returns>
		private async Task JoinServer() {
			try {
				ServerOption selectedServerOption = (ServerOption)serverListView.SelectedItem;
				Server selectedServer = selectedServerOption.Server;

				// If the server is already joined, show an error
				if (selectedServerOption.Joined) {
					MessageBox.Show("You are already joined to this server", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				} else {
					await serverService.JoinServer(selectedServer);

					MessageBox.Show("You have joined the server", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
					Close();
				}
			} catch (Exception ex) {
				// Show the error to the user
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private async void joinButton_Click(object sender, RoutedEventArgs e) {
			await JoinServer();
		}

		private async void refreshButton_Click(object sender, RoutedEventArgs e) {
			await RefreshServers();
		}

		private async void serverListView_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
			// Get the mouse position
			Point mousePosition = e.GetPosition((IInputElement)sender);

			// Check if the mouse position is within the DataGridColumnHeader
			DataGridColumnHeader? header = FindVisualParent<DataGridColumnHeader>(e.OriginalSource as DependencyObject);

			// If the mouse position is not within the DataGridColumnHeader, join the server
			if (header == null) {
				await JoinServer();
			}
			e.Handled = true;
		}

		// Helper method to find the DataGridColumnHeader at a given position
		private T? FindVisualParent<T>(DependencyObject? child) where T : DependencyObject {
			// Get the parent object
			DependencyObject parentObject = VisualTreeHelper.GetParent(child);

			// If the parent object is null, return null
			if (parentObject == null)
				return null;

			//  If the parent object is of type T, return the parent object
			if (parentObject is T parent)
				return parent;

			// Recursively call the method with the parent object
			return FindVisualParent<T>(parentObject);
		}

		private void Expander_Expanded(object sender, RoutedEventArgs e) {
			// Toggle the visibility of the details
			for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual) {
				if (vis is DataGridRow row) {
					row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
					break;
				}
			}
		}

		private void Expander_Collapsed(object sender, RoutedEventArgs e) {
			// Toggle the visibility of the details
			for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual) {
				if (vis is DataGridRow row) {
					row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
					break;
				}
			}
		}

		private void serverListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			// If the selected index is -1, disable the join button
			joinButton.IsEnabled = serverListView.SelectedIndex == -1;
		}
	}
}
