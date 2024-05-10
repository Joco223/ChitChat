using ChitChatLauncher.Services;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChitChatLauncher
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

		private readonly UpdateService updateService = UpdateService.Instance;

		public MainWindow()
		{
			InitializeComponent();
		}

		private async void Window_Loaded(object sender, RoutedEventArgs e)
		{
			bool updated = await updateService.UpdateApp(UpdateDownloadProgressBar);

			System.Diagnostics.Process.Start(updateService.GetAppPath());
			Application.Current.Shutdown();
		}

		private void UpdateDownloadProgressBar(float progressPercentage)
		{
			downloadProgressLabel.Content = "Downloading: " + Math.Round(progressPercentage) + "%";
			downloadProgressBar.Value = progressPercentage;
		}
	}
}