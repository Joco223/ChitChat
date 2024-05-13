using ChitChatClient.Helpers;
using ChitChatClient.Models;
using ChitChatClient.Services;
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
	/// Interaction logic for ServerSettingsWindow.xaml
	/// </summary>
	public partial class ServerSettingsWindow : Window
	{
		private readonly ServerService serverService = ServerService.Instance;
		private Server currentServer;

		public Server CurrentServer { get => currentServer; set => currentServer = value; }

		public ServerSettingsWindow()
		{
			InitializeComponent();
			currentServer = new();

		}

		public ServerSettingsWindow(Server server) : base()
		{
			InitializeComponent();
			DataContext = this;

			currentServer = server;
		}

		private async void saveServerButton_Click(object sender, RoutedEventArgs e)
		{
			await serverService.UpdateServer(CurrentServer);
		}
	}
}
