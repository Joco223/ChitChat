using ChitChatClient.Models;
using ChitChatClient.Services;
using ChitChatClient.ViewModels;
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

namespace ChitChatClient.Windows
{
	/// <summary>
	/// Interaction logic for ServerListWindow.xaml
	/// </summary>
	public partial class ServerListWindow : Window
	{
		private readonly ServerService serverService = ServerService.Instance;
		public ServerList serverList = new();

		public ServerListWindow()
		{
			InitializeComponent();
			DataContext = serverList;
		}

		private async Task refreshServers()
		{
            await serverList.GetServerOptions();
            serverListView.GetBindingExpression(ListView.ItemsSourceProperty).UpdateTarget();
            serverListView.SelectedIndex = 0;
        }

		// Filter servers
		private void serverSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			CollectionViewSource.GetDefaultView(serverListView.ItemsSource).Refresh();
        }

		private bool FilterServer(object obj)
		{
			if (string.IsNullOrEmpty(serverSearchTextBox.Text))
			{
				return true;
			}

			if (obj is ServerOption serverOption)
			{
				if (serverOption.Server.Name.ToLower().Contains(serverSearchTextBox.Text.ToLower()))
				{
                    return true;
                }
            }

            return false;
		}

		private async void Window_Loaded(object sender, RoutedEventArgs e)
		{
			await refreshServers();
            CollectionViewSource.GetDefaultView(serverListView.ItemsSource).Filter = FilterServer;
        }

		private void cancelButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
        }

		private async void joinButton_Click(object sender, RoutedEventArgs e)
		{
			ServerOption selectedServerOption = (ServerOption)serverListView.SelectedItem;
			Server selectedServer = selectedServerOption.Server;

			if (selectedServerOption.Joined == "Yes")
			{
				MessageBox.Show("You are already joined to this server", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
			{
                bool result = await serverService.JoinServer(selectedServer.Id);

                if (result)
				{
                    MessageBox.Show("You have joined the server", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                else
				{
                    MessageBox.Show("Failed to join the server", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
        }
    }
}
