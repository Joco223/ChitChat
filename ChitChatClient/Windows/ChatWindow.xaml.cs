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

		public ChatWindow()
		{
			InitializeComponent();
			DataContext = chatInterface;

			// Initializing user refresh timer
			userRefreshTimer = new DispatcherTimer();

			userRefreshTimer.Tick += new EventHandler(RefreshServerUsersTask);
			userRefreshTimer.Interval = new TimeSpan(0, 0, 5);
			userRefreshTimer.Start();
		}

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
			userFilterTextBox.Text = string.Empty;
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

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await chatInterface.RefreshJoinedServers(RefreshSeverListView);
            await chatInterface.RefreshServerUsers(RefreshUserListView, SelectedServer);
			CollectionViewSource.GetDefaultView(userListView.ItemsSource).Filter = FilterUser;
			CollectionViewSource.GetDefaultView(serverListView.ItemsSource).Filter = FilterServer;
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

        private void serverSettingsButton_Click(object sender, RoutedEventArgs e)
		{
            MessageBox.Show("Feature not implemented", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
			await chatInterface.RefreshServerUsers(RefreshUserListView, SelectedServer);
        }

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			userRefreshTimer?.Stop();
		}

		private void userFilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			CollectionViewSource.GetDefaultView(userListView.ItemsSource).Refresh();
		}

		private void filterServerTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			CollectionViewSource.GetDefaultView(serverListView.ItemsSource).Refresh();
		}

		private bool FilterUser(object obj)
		{
			if (string.IsNullOrEmpty(userFilterTextBox.Text))
			{
				return true;
			}

			if (obj is User user)
			{
				if (user.Username.ToLower().Contains(userFilterTextBox.Text.ToLower()))
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

			if (obj is Server server)
			{
				if (server.Name.ToLower().Contains(filterServerTextBox.Text.ToLower()))
				{
					return true;
				}
			}

			return false;
		}
	}
}
