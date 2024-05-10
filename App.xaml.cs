using ChitChat.Services;
using System.Configuration;
using System.Data;
using System.Windows;

namespace ChitChat
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private readonly AppInfoService appInfoService = AppInfoService.Instance;

		private void Application_Exit(object sender, ExitEventArgs e)
		{
			appInfoService.RenameNewVersion();
        }
    }

}
