using ChitChatClient.Helpers;
using ChitChatClient.Models;
using ChitChatClient.Services;
using ChitChatClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ChitChatClient.Windows
{
	/// <summary>
	/// Interaction logic for ChatWindow.xaml
	/// </summary>
	public partial class ChatWindow : Window
	{
		public ChatInterface chatInterface = new();

		private static DispatcherTimer? userRefreshTimer;

		private readonly ServerService serverService = ServerService.Instance;

		private Server? SelectedServer
		{
			get
			{
				Server selectedServer = (Server)serverListView.SelectedItem;
				if (selectedServer == null)
				{
					return null;
				}
				else
				{
					return selectedServer;
				}
			}
		}

		private Channel? SelectedChannel
		{
			get
			{
				Channel selectedChannel = (Channel)channelListView.SelectedItem;
				if (selectedChannel == null)
				{
					return null;
				}
				else
				{
					return selectedChannel;
				}
			}
		}

		public ChatWindow()
		{
			InitializeComponent();
			DataContext = chatInterface;

			// Initializing user refresh timer
			userRefreshTimer = new DispatcherTimer();

			userRefreshTimer.Tick += new EventHandler(RefreshServerUsersTask);
			userRefreshTimer.Interval = new TimeSpan(0, 0, 5);
			userRefreshTimer.Start();

			// Set placeholders
			PlaceholderProperty.SetPlaceholderText(userFilterTextBox);
			PlaceholderProperty.SetPlaceholderText(filterServerTextBox);
			PlaceholderProperty.SetPlaceholderText(messageTextBox);
		}

		private async void Window_Loaded(object sender, RoutedEventArgs e)
		{
			await chatInterface.RefreshJoinedServers(RefreshSeverListView);
			await chatInterface.RefreshServerUsers(RefreshUserListView, SelectedServer);
			await RefreshServerChannels();
			CollectionViewSource.GetDefaultView(userListView.ItemsSource).Filter = FilterUser;
			CollectionViewSource.GetDefaultView(serverListView.ItemsSource).Filter = FilterServer;
		}

		private void RefreshSeverListView()
		{
			serverListView.GetBindingExpression(ListView.ItemsSourceProperty).UpdateTarget();
			CollectionViewSource.GetDefaultView(serverListView.ItemsSource).Filter = FilterServer;
			CollectionViewSource.GetDefaultView(serverListView.ItemsSource).Refresh();
			serverListView.SelectedIndex = 0;

			removeServerButton.IsEnabled = chatInterface.Servers.Count != 0;
		}

		private void RefreshUserListView(int? onlineUserCount, int? totalUserCount)
		{
			userListView.GetBindingExpression(ListView.ItemsSourceProperty).UpdateTarget();
			CollectionViewSource.GetDefaultView(userListView.ItemsSource).Filter = FilterUser;
			CollectionViewSource.GetDefaultView(userListView.ItemsSource).Refresh();
			userListView.SelectedIndex = -1;

			if (onlineUserCount != null && totalUserCount != null)
			{
				onlineUserCountTB.Text = "Online " + onlineUserCount.ToString();
				totalUserCountTB.Text = "Total " + totalUserCount.ToString();
			}
		}

		private async Task RefreshServerUsersWrapper()
		{
			await chatInterface.RefreshServerUsers(RefreshUserListView, SelectedServer);
		}

		private async void RefreshServerUsersTask(object? sender, EventArgs e)
		{
			await RefreshServerUsersWrapper();
		}

		private async Task RefreshServerChannels()
		{
			await chatInterface.GetServerChannels(SelectedServer);

			if (SelectedChannel != null)
			{
				currentChannelLabel.Text = SelectedChannel.Name;
				currentChannelDescriptionLabel.Text = SelectedChannel.Description;
			}

		}

		private async void createServerButton_Click(object sender, RoutedEventArgs e)
		{
			// Create a server
			CreateServerWindow createServerWindow = new();
			createServerWindow.ShowDialog();
			await chatInterface.RefreshJoinedServers(RefreshSeverListView);
		}

		private async void removeServerButton_Click(object sender, RoutedEventArgs e)
		{
			// Leave a server
			if (serverListView.SelectedIndex != -1)
			{
				bool result = await chatInterface.LeaveServer(SelectedServer);

				if (result)
				{
					MessageBox.Show("Server left successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
				}
				else
				{
					MessageBox.Show("Failed to leave server", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}

				await chatInterface.RefreshJoinedServers(RefreshSeverListView);
                serverListView.SelectedIndex = 0;
			}
        }

		// Joins a server
		private async void addServerButton_Click(object sender, RoutedEventArgs e)
		{
			ServerListWindow serverListWindow = new();
			serverListWindow.ShowDialog();
			await chatInterface.RefreshJoinedServers(RefreshSeverListView);
        }

		private void serverInfoButton_Click(object sender, RoutedEventArgs e)
		{
            MessageBox.Show("Feature not implemented", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

        }

        private async void serverSettingsButton_Click(object sender, RoutedEventArgs e)
		{
			if (SelectedServer == null)
			{
				MessageBox.Show("No server selected", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			// Check if current user is the server owner
			if (!await serverService.IsServerOwner(SelectedServer))
			{
				MessageBox.Show("You are not the server owner", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			ServerSettingsWindow serverSettingsWindow = new(SelectedServer);
			serverSettingsWindow.ShowDialog();
        }

		private void sendButton_Click(object sender, RoutedEventArgs e)
		{
            MessageBox.Show("Feature not implemented", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

		private void attachButton_Click(object sender, RoutedEventArgs e)
		{
            MessageBox.Show("Feature not implemented", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

		private void accountSettingsButton_Click(object sender, RoutedEventArgs e)
		{
            MessageBox.Show("Feature not implemented", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

		private async void serverListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (SelectedServer == null)
			{
				return;
			}

			await chatInterface.RefreshServerUsers(RefreshUserListView, SelectedServer);
			await RefreshServerChannels();

			if (await serverService.IsServerOwner(SelectedServer))
			{
				serverSettingsButton.IsEnabled = true;
			}
			else
			{
				serverSettingsButton.IsEnabled = false;
			}
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			userRefreshTimer?.Stop();
		}

		private void userFilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (userListView.ItemsSource != null)
			{
				CollectionViewSource.GetDefaultView(userListView.ItemsSource).Refresh();
			}
		}

		private void filterServerTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (serverListView.ItemsSource != null)
			{
				CollectionViewSource.GetDefaultView(serverListView.ItemsSource).Refresh();
			}
		}

		private bool FilterUser(object obj)
		{
			if (string.IsNullOrEmpty(userFilterTextBox.Text))
			{
				return true;
			}

			// If the text is the placeholder, return true
			if (userFilterTextBox.Text == PlaceholderProperty.GetPlaceholder(userFilterTextBox))
			{
				return true;
			}

			if (obj is User user)
			{
				if (user.Username.Contains(userFilterTextBox.Text, StringComparison.CurrentCultureIgnoreCase))
				{
					return true;
				}
			}

			return false;
		}

		private bool FilterServer(object obj)
		{
			if (string.IsNullOrEmpty(filterServerTextBox.Text))
			{
				return true;
			}

			// If the text is the placeholder, return true
			if (filterServerTextBox.Text == PlaceholderProperty.GetPlaceholder(filterServerTextBox))
			{
				return true;
			}

			if (obj is Server server)
			{
				if (server.Name.Contains(filterServerTextBox.Text, StringComparison.CurrentCultureIgnoreCase))
				{
					return true;
				}
			}

			return false;
		}

		private void userFilterTextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			PlaceholderProperty.ClearPlaceholderText(userFilterTextBox);
		}

		private void userFilterTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			PlaceholderProperty.SetPlaceholderText(userFilterTextBox);
		}

		private void filterServerTextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			PlaceholderProperty.ClearPlaceholderText(filterServerTextBox);
		}

		private void filterServerTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			PlaceholderProperty.SetPlaceholderText(filterServerTextBox);
		}

		private void messageTextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			PlaceholderProperty.ClearPlaceholderText(messageTextBox);
		}

		private void messageTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			PlaceholderProperty.SetPlaceholderText(messageTextBox);
		}
	}
}
