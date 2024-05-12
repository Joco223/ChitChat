using ChitChatClient.Helpers;
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
		public ServerSettingsWindow()
		{
			InitializeComponent();

			// Set placeholders
			PlaceholderProperty.SetPlaceholderText(filterChannelList);
			PlaceholderProperty.SetPlaceholderText(filterRoleList);
		}

		private void filterChannelList_GotFocus(object sender, RoutedEventArgs e)
		{
			PlaceholderProperty.ClearPlaceholderText(filterChannelList);
        }

		private void filterChannelList_LostFocus(object sender, RoutedEventArgs e)
		{
			PlaceholderProperty.SetPlaceholderText(filterChannelList);
        }

		private void filterRoleList_GotFocus(object sender, RoutedEventArgs e)
		{
			PlaceholderProperty.ClearPlaceholderText(filterRoleList);
        }

		private void filterRoleList_LostFocus(object sender, RoutedEventArgs e)
		{
			PlaceholderProperty.SetPlaceholderText(filterRoleList);
        }
    }
}
