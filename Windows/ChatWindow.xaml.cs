using ChitChat.Models;
using ChitChat.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChitChat.Windows
{
	/// <summary>
	/// Interaction logic for ChatWindow.xaml
	/// </summary>
	public partial class ChatWindow : Window
	{
		public ChatInterface chatInterface = new();
		public ChatWindow()
		{
			InitializeComponent();
			DataContext = chatInterface;
		}

		private int getSelectedServerId()
		{
			Server selectedServer = (Server)serverListView.SelectedItem;
			return selectedServer.Id;
		}

		private async Task refreshJoinedServers()
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

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await refreshJoinedServers();
        }

        private async void createServerButton_Click(object sender, RoutedEventArgs e)
		{
			// Create a server
			CreateServerWindow createServerWindow = new();
			createServerWindow.ShowDialog();
			await refreshJoinedServers();
        }

		private async void removeServerButton_Click(object sender, RoutedEventArgs e)
		{
			// Remove a server
			if (serverListView.SelectedIndex != -1)
			{
				bool result = await chatInterface.LeaveServer(getSelectedServerId());

				if (result)
				{
					MessageBox.Show("Server left successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    await refreshJoinedServers();
				}
				else
				{
					MessageBox.Show("Failed to leave server", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}

				await refreshJoinedServers();
                serverListView.SelectedIndex = 0;
			}
        }

		private async void addServerButton_Click(object sender, RoutedEventArgs e)
		{
			ServerListWindow serverListWindow = new();
			serverListWindow.ShowDialog();
			await refreshJoinedServers();
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
    }
}
