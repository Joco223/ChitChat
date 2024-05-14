using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

using ChitChatClient.Models;
using ChitChatClient.Services;
using ChitChatClient.ViewModels;

namespace ChitChatClient.Windows {
	/// <summary>
	/// Interaction logic for ChatWindow.xaml
	/// </summary>
	public partial class ChatWindow : Window {
		public ChatInterface chatInterface = new();

		private static DispatcherTimer? userRefreshTimer;

		private readonly ServerService serverService = ServerService.Instance;

		/// <summary>
		/// Returns the selected server from the server list view, null if none is selected
		/// </summary>
		private Server? SelectedServer {
			get {
				Server selectedServer = (Server)serverListView.SelectedItem;
				if (selectedServer == null) {
					return null;
				} else {
					return selectedServer;
				}
			}
		}

		/// <summary>
		/// Returns the selected channel from the channel list view, null if none is selected
		/// </summary>
		private Channel? SelectedChannel {
			get {
				Channel selectedChannel = (Channel)channelListView.SelectedItem;
				if (selectedChannel == null) {
					return null;
				} else {
					return selectedChannel;
				}
			}
		}

		public ChatWindow() {
			InitializeComponent();
			DataContext = chatInterface;

			// Initializing user refresh timer
			userRefreshTimer = new DispatcherTimer();

			userRefreshTimer.Tick += new EventHandler(RefreshServerUsersTask);
			userRefreshTimer.Interval = new TimeSpan(0, 0, 5);
			userRefreshTimer.Start();
		}


		private async void Window_Loaded(object sender, RoutedEventArgs e) {
			try {
				// Refresh the server list view
				await chatInterface.RefreshJoinedServers(RefreshSeverListView);

				// Refresh the user list view if there are servers
				if (chatInterface.HasServers() && SelectedServer != null) {
					await chatInterface.RefreshServerUsers(RefreshUserListView, SelectedServer);
				}

				// Refresh the channel list view if there are servers
				await RefreshServerChannels();
			} catch (Exception ex) {
				// Show the error to the user
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}

			// Set the filter callbacks for the user and server list views
			CollectionViewSource.GetDefaultView(userListView.ItemsSource).Filter = FilterUser;
			CollectionViewSource.GetDefaultView(serverListView.ItemsSource).Filter = FilterServer;
		}

		/// <summary>
		/// Refreshes the server list view
		/// </summary>
		private void RefreshSeverListView() {
			// Update the server list view
			serverListView.GetBindingExpression(ListView.ItemsSourceProperty).UpdateTarget();

			// Set the filter callback for the server list view and refresh
			CollectionViewSource.GetDefaultView(serverListView.ItemsSource).Filter = FilterServer;
			CollectionViewSource.GetDefaultView(serverListView.ItemsSource).Refresh();

			// Reser selected index
			serverListView.SelectedIndex = 0;

			// Enable or disable the remove server button if there are servers
			removeServerButton.IsEnabled = chatInterface.Servers.Count != 0;
		}

		/// <summary>
		/// Refreshes the user list view
		/// </summary>
		/// <param name="onlineUserCount">Server online user count</param>
		/// <param name="totalUserCount">Server total user count</param>
		private void RefreshUserListView(int? onlineUserCount, int? totalUserCount) {
			// Update the user list view
			userListView.GetBindingExpression(ListView.ItemsSourceProperty).UpdateTarget();

			// Set the filter callback for the user list view and refresh
			CollectionViewSource.GetDefaultView(userListView.ItemsSource).Filter = FilterUser;
			CollectionViewSource.GetDefaultView(userListView.ItemsSource).Refresh();

			// Reset selected index
			userListView.SelectedIndex = -1;

			// Update the online and total user count text blocks
			if (onlineUserCount != null && totalUserCount != null) {
				onlineUserCountTB.Text = $"Online {onlineUserCount}";
				totalUserCountTB.Text = $"Total {totalUserCount}";
			}
		}

		/// <summary>
		/// Refreshes the server users list view
		/// </summary>
		/// <returns></returns>
		private async Task RefreshServerUsersWrapper() {
			try {
				// Refresh the user list view if there are servers
				if (chatInterface.HasServers() && SelectedServer != null) {
					await chatInterface.RefreshServerUsers(RefreshUserListView, SelectedServer);
				}
			} catch (Exception ex) {
				// Show error to the user
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		/// <summary>
		/// Function for the user refresh timer
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void RefreshServerUsersTask(object? sender, EventArgs e) {
			await RefreshServerUsersWrapper();
		}

		/// <summary>
		/// Refreshes the server channels list view
		/// </summary>
		/// <returns></returns>
		private async Task RefreshServerChannels() {
			try {
				if (chatInterface.HasServers() && SelectedServer != null) {
					// Get the channels for the selected server
					await chatInterface.GetServerChannels(SelectedServer);

					// Update the channel label and description if there are channels
					if (chatInterface.HasChannels() && SelectedChannel != null) {
						currentChannelLabel.Text = SelectedChannel.Name;
						currentChannelDescriptionLabel.Text = SelectedChannel.Description;
					}
				}
			} catch (Exception ex) {
				// Show error to the user
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}

		}

		private async void createServerButton_Click(object sender, RoutedEventArgs e) {
			// Open server creation window
			CreateServerWindow createServerWindow = new();
			createServerWindow.ShowDialog();

			try {
				// Refresh the server list view
				await chatInterface.RefreshJoinedServers(RefreshSeverListView);
			} catch (Exception ex) {
				// Show error to the user
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private async void removeServerButton_Click(object sender, RoutedEventArgs e) {
			try {
				if (chatInterface.HasServers() && SelectedServer != null) {
					// Leave the server
					await chatInterface.LeaveServer(SelectedServer);

					MessageBox.Show("Server left successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

					// Refresh the server list view
					await chatInterface.RefreshJoinedServers(RefreshSeverListView);
					serverListView.SelectedIndex = 0;
				}
			} catch (Exception ex) {
				// Show error to the user
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private async void addServerButton_Click(object sender, RoutedEventArgs e) {
			// Open server list window
			ServerListWindow serverListWindow = new();
			serverListWindow.ShowDialog();

			try {
				await chatInterface.RefreshJoinedServers(RefreshSeverListView);
			} catch (Exception ex) {
				// Show error to user
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void serverInfoButton_Click(object sender, RoutedEventArgs e) {
			MessageBox.Show("Feature not implemented", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

		}

		private async void serverSettingsButton_Click(object sender, RoutedEventArgs e) {
			if (chatInterface.HasServers() && SelectedServer != null) {
				// Check if current user is the server owner
				if (!await serverService.IsServerOwner(SelectedServer)) {
					MessageBox.Show("You are not the server owner", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}

				// Open server settings window
				ServerSettingsWindow serverSettingsWindow = new(SelectedServer);
				serverSettingsWindow.ShowDialog();
			}
		}

		private void sendButton_Click(object sender, RoutedEventArgs e) {
			MessageBox.Show("Feature not implemented", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		private void attachButton_Click(object sender, RoutedEventArgs e) {
			MessageBox.Show("Feature not implemented", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		private void accountSettingsButton_Click(object sender, RoutedEventArgs e) {
			MessageBox.Show("Feature not implemented", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		private async void serverListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			try {
				if (chatInterface.HasServers() && SelectedServer != null) {
					// Refresh the user list view
					await chatInterface.RefreshServerUsers(RefreshUserListView, SelectedServer);

					// Refresh the channel list view
					await RefreshServerChannels();

					// Enable or disable the server settings button if the user is the server owner
					serverSettingsButton.IsEnabled = await serverService.IsServerOwner(SelectedServer);
				}
			} catch (Exception ex) {
				// Show error to the user
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}

		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			// Stop the user refresh timer on window close
			userRefreshTimer?.Stop();
		}

		private void userFilterTextBox_TextChanged(object sender, TextChangedEventArgs e) {
			// Refresh the user list view
			if (userListView.ItemsSource != null) {
				CollectionViewSource.GetDefaultView(userListView.ItemsSource).Refresh();
			}
		}

		private void filterServerTextBox_TextChanged(object sender, TextChangedEventArgs e) {
			// Refresh the server list view
			if (serverListView.ItemsSource != null) {
				CollectionViewSource.GetDefaultView(serverListView.ItemsSource).Refresh();
			}
		}

		/// <summary>
		/// Filters server list depending on userFilterTextBox input
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		private bool FilterUser(object obj) {
			// If filter text is empty, return true on all servers
			if (string.IsNullOrEmpty(userFilterTextBox.Text)) {
				return true;
			}

			// If the text is the placeholder, return true
			if (userFilterTextBox.Text == userFilterTextBox.Placeholder) {
				return true;
			}

			// If the object is a user and the username contains the filter text, return true
			if (obj is User user) {
				if (user.Username.Contains(userFilterTextBox.Text, StringComparison.CurrentCultureIgnoreCase)) {
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Filters server list depending on filterServerTextBox input
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		private bool FilterServer(object obj) {
			// If filter text is empty, return true on all servers
			if (string.IsNullOrEmpty(filterServerTextBox.Text)) {
				return true;
			}

			// If the text is the placeholder, return true
			if (filterServerTextBox.Text == filterServerTextBox.Placeholder) {
				return true;
			}

			// If the object is a server and the server name contains the filter text, return true
			if (obj is Server server) {
				if (server.Name.Contains(filterServerTextBox.Text, StringComparison.CurrentCultureIgnoreCase)) {
					return true;
				}
			}

			return false;
		}
	}
}
