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
	/// Interaction logic for CreateServerWindow.xaml
	/// </summary>
	public partial class CreateServerWindow : Window
	{
		private CreateServer CreateServer = new();

		private readonly ServerService serverService = ServerService.Instance;

		public CreateServerWindow()
		{
			InitializeComponent();
			DataContext = CreateServer;
		}

		async private void createServerButton_Click(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrEmpty(CreateServer.Name))
			{
				bool result = await serverService.CreateServer(CreateServer.Name);

				if (result)
				{
					MessageBox.Show("Server created successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
				}
				else
				{
					MessageBox.Show("Failed to create server", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
        }

		private void cancelButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
        }
    }
}
