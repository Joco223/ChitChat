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

			userRefreshTimer.Tick += new EventHandler(RefreshServerUsersTimed);
			userRefreshTimer.Interval = new TimeSpan(0, 0, 5);
			userRefreshTimer.Start();
		}

		private int SelectedServerId
		{
			get
			{
				Server selectedServer = (Server)serverListView.SelectedItem;
				if (selectedServer == null)
				{
					return -1;
				}
				else
				{
					return selectedServer.Id;
				}
			}
		}

		private async Task RefreshJoinedServers()
		{
            // Get all joined servers
            await chatInterface.GetServers();
            serverListView.GetBindingExpression(ListView.ItemsSourceProperty).UpdateTarget();
            serverListView.SelectedIndex = 0;
			if (chatInterface.Servers.Count == 0)
			{
                removeServerButton.IsEnabled = false;
            }
            else
			{
                removeServerButton.IsEnabled = true;
            }
        }

		private async Task RefreshServerUsers()
		{
			await chatInterface.GetServerUsers(SelectedServerId);
			userListView.GetBindingExpression(ListView.ItemsSourceProperty).UpdateTarget();

			Server selectedServer = (Server)serverListView.SelectedItem;
			int onlineUserCount = await serverService.GetOnlineUserCount(selectedServer.Id);

			onlineUserCountTB.Text = "Online " + onlineUserCount.ToString();
			totalUserCountTB.Text = "Total " + selectedServer.UserCount.ToString();
		}

		private async void RefreshServerUsersTimed(object? sender, EventArgs e)
		{
			await RefreshServerUsers();
		}

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await RefreshJoinedServers();
            await RefreshServerUsers();
        }

        private async void createServerButton_Click(object sender, RoutedEventArgs e)
		{
			// Create a server
			CreateServerWindow createServerWindow = new();
			createServerWindow.ShowDialog();
			await RefreshJoinedServers();
        }

		private async void removeServerButton_Click(object sender, RoutedEventArgs e)
		{
			// Remove a server
			if (serverListView.SelectedIndex != -1)
			{
				bool result = await chatInterface.LeaveServer(SelectedServerId);

				if (result)
				{
					MessageBox.Show("Server left successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    await RefreshJoinedServers();
				}
				else
				{
					MessageBox.Show("Failed to leave server", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}

				await RefreshJoinedServers();
                serverListView.SelectedIndex = 0;
			}
        }

		// Joins a server
		private async void addServerButton_Click(object sender, RoutedEventArgs e)
		{
			ServerListWindow serverListWindow = new();
			serverListWindow.ShowDialog();
			await RefreshJoinedServers();
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
			await chatInterface.GetServerUsers(SelectedServerId);
            userListView.GetBindingExpression(ListView.ItemsSourceProperty).UpdateTarget();
        }

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			userRefreshTimer?.Stop();
		}
	}
}
