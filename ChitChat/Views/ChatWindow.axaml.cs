using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using ChitChat.DatabaseModels;
using ChitChat.Services;
using ChitChat.ViewModels;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace ChitChat.Views {
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
			// Refresh the server list view
			var result = await chatInterface.RefreshJoinedServers(RefreshSeverListView);

			if (result.Failed) {
				var errorNotification = MessageBoxManager.GetMessageBoxStandard("Error", "Error loading servers", ButtonEnum.Ok);
				await errorNotification.ShowWindowDialogAsync(this);
				return;
			}

			// Refresh the user list view if there are servers
			if (chatInterface.HasServers()) {
				serverListView.SelectedIndex = 0;
				await chatInterface.RefreshServerUsers(RefreshUserListView, SelectedServer);
			}

			// Refresh the channel list view if there are servers
			await RefreshServerChannels();

			// Set the filter callbacks for the user and server list views
			//CollectionViewSource.GetDefaultView(userListView.ItemsSource).Filter = FilterUser;
			//CollectionViewSource.GetDefaultView(serverListView.ItemsSource).Filter = FilterServer;
		}

		/// <summary>
		/// Refreshes the server list view
		/// </summary>
		private void RefreshSeverListView() {
			// Update the server list view
			//serverListView.GetBindingExpression(ListView.ItemsSourceProperty).UpdateTarget();

			// Set the filter callback for the server list view and refresh
			//CollectionViewSource.GetDefaultView(serverListView.ItemsSource).Filter = FilterServer;
			//CollectionViewSource.GetDefaultView(serverListView.ItemsSource).Refresh();

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
			//userListView.GetBindingExpression(ListView.ItemsSourceProperty).UpdateTarget();

			// Set the filter callback for the user list view and refresh
			//CollectionViewSource.GetDefaultView(userListView.ItemsSource).Filter = FilterUser;
			//CollectionViewSource.GetDefaultView(userListView.ItemsSource).Refresh();

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
				var errorNotification = MessageBoxManager.GetMessageBoxStandard("Error", ex.Message, ButtonEnum.Ok);
				await errorNotification.ShowWindowDialogAsync(this);
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
			if (chatInterface.HasServers() && SelectedServer != null) {
				// Get the channels for the selected server
				var result = await chatInterface.GetServerChannels(SelectedServer);

				if (result.Failed) {
					// Show error to the user
					var errorNotification = MessageBoxManager.GetMessageBoxStandard("Error", result.Error, ButtonEnum.Ok);
					await errorNotification.ShowWindowDialogAsync(this);
					return;
				}

				// Update the channel label and description if there are channels
				if (chatInterface.HasChannels() && SelectedChannel != null) {
					currentChannelLabel.Text = SelectedChannel.Name;
					currentChannelDescriptionLabel.Text = SelectedChannel.Description;
				}
			}

		}

		private async void createServerButton_Click(object sender, RoutedEventArgs e) {
			// Open server creation window
			CreateServerWindow createServerWindow = new();
			await createServerWindow.ShowDialog(this);

			// Refresh the server list view
			var result = await chatInterface.RefreshJoinedServers(RefreshSeverListView);

			if (result.Failed) {
				// Show error to the user
				var errorNotification = MessageBoxManager.GetMessageBoxStandard("Error", result.Error, ButtonEnum.Ok);
				await errorNotification.ShowWindowDialogAsync(this);
			}
		}

		private async void removeServerButton_Click(object sender, RoutedEventArgs e) {
			if (chatInterface.HasServers() && SelectedServer != null) {
				// Leave the server
				var result = await chatInterface.LeaveServer(SelectedServer);

				if (result.Failed) {
					// Show error to the user
					var errorNotification = MessageBoxManager.GetMessageBoxStandard("Error", result.Error, ButtonEnum.Ok);
					await errorNotification.ShowWindowDialogAsync(this);
					return;
				}

				// Notify the user that the server was left
				var notification = MessageBoxManager.GetMessageBoxStandard("Success", "Server left successfully", ButtonEnum.Ok);
				await notification.ShowWindowDialogAsync(this);

				// Refresh the server list view
				var chatResult = await chatInterface.RefreshJoinedServers(RefreshSeverListView);

				if (chatResult.Failed) {
					// Show error to the user
					var errorNotification = MessageBoxManager.GetMessageBoxStandard("Error", chatResult.Error, ButtonEnum.Ok);
					await errorNotification.ShowWindowDialogAsync(this);
				}

				serverListView.SelectedIndex = 0;
			}
		}

		private async void addServerButton_Click(object sender, RoutedEventArgs e) {
			// Open server list window
			ServerListWindow serverListWindow = new();
			await serverListWindow.ShowDialog(this);

			var result = await chatInterface.RefreshJoinedServers(RefreshSeverListView);

			if (result.Failed) {
				// Show error to the user
				var errorNotification = MessageBoxManager.GetMessageBoxStandard("Error", result.Error, ButtonEnum.Ok);
				await errorNotification.ShowWindowDialogAsync(this);
			}
		}

		private void serverInfoButton_Click(object sender, RoutedEventArgs e) {

		}

		private async void serverSettingsButton_Click(object sender, RoutedEventArgs e) {
			if (chatInterface.HasServers() && SelectedServer != null) {
				// Check if current user is the server owner
				var result = await serverService.IsServerOwner(SelectedServer);

				// Check for error
				if (result.Failed) {
					var errorNotification = MessageBoxManager.GetMessageBoxStandard("Error", result.Error, ButtonEnum.Ok);
					await errorNotification.ShowWindowDialogAsync(this);
					return;
				}

				if (!result.Data) {
					var errorNotification = MessageBoxManager.GetMessageBoxStandard("Error", "You are not the server owner", ButtonEnum.Ok);
					await errorNotification.ShowWindowDialogAsync(this);
					return;
				}

				// Open server settings window
				ServerSettingsWindow serverSettingsWindow = new(SelectedServer);
				await serverSettingsWindow.ShowDialog(this);
			}
		}

		private void sendButton_Click(object sender, RoutedEventArgs e) {
		}

		private void attachButton_Click(object sender, RoutedEventArgs e) {
		}

		private void accountSettingsButton_Click(object sender, RoutedEventArgs e) {
		}

		private async void serverListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			if (chatInterface.HasServers() && SelectedServer != null) {
				// Refresh the user list view
				var result = await chatInterface.RefreshServerUsers(RefreshUserListView, SelectedServer);

				if (result.Failed) {
					// Show error to the user
					var errorNotification = MessageBoxManager.GetMessageBoxStandard("Error", result.Error, ButtonEnum.Ok);
					await errorNotification.ShowWindowDialogAsync(this);
					return;
				}

				// Refresh the channel list view
				await RefreshServerChannels();

				// Enable or disable the server settings button if the user is the server owner
				var ownerResult = await serverService.IsServerOwner(SelectedServer);

				if (ownerResult.Failed) {
					// Show error to the user
					var errorNotification = MessageBoxManager.GetMessageBoxStandard("Error", ownerResult.Error, ButtonEnum.Ok);
					await errorNotification.ShowWindowDialogAsync(this);
					return;
				}

				serverSettingsButton.IsEnabled = ownerResult.Data;
				serverInfoButton.IsEnabled = true;
			} else {
				serverSettingsButton.IsEnabled = false;
				serverInfoButton.IsEnabled = false;
			}
		}

		protected override void OnClosing(WindowClosingEventArgs e)
		{
			// Stop the user refresh timer on window close
			userRefreshTimer?.Stop();

			base.OnClosing(e);
		}

		private void userFilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			// Refresh the user list view
			chatInterface.FilterUsers();
		}

		private void filterServerTextBox_TextChanged(object sender, TextChangedEventArgs e) {
			//Refresh the server list view
			chatInterface.FilterServers();
		}
	}
}
