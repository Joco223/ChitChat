using ChitChatClient.Services;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;

namespace ChitChatClient
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private readonly UserService userService = UserService.Instance;

		protected override void OnExit(ExitEventArgs e)
		{
			try
			{
				Task.Run(() => userService.SetUserOnline(false)).Wait();
			}
			catch (Exception)
			{
				Debug.WriteLine("Error setting user offline");
			}
			
			base.OnExit(e);
		}
	}

}
