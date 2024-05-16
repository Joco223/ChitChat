using System.IO;
using System.Windows;

using ChitChatClient.Services;

using Serilog;

namespace ChitChatClient {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application {
		private readonly UserService UserService = UserService.Instance;

		protected override void OnExit(ExitEventArgs e) {
			try {

				var user = UserService.GetUser();

				// Only set offline status if user is logged in
				if (!user.Failed)
					Task.Run(() => UserService.SetUserOnline(false)).Wait();

				Log.CloseAndFlush();
			} catch (Exception) {
				Log.Error("Unable to log out user");
				MessageBox.Show("Unable to log out user!", "Error logging out", MessageBoxButton.OK, MessageBoxImage.Error);
			}

			base.OnExit(e);
		}

		/// <summary>
		/// Ensures a folder exists
		/// </summary>
		/// <param name="path"></param>
		private static void EnsureFolder(string path) {
			string? directoryName = Path.GetDirectoryName(path);

			if (directoryName?.Length > 0) {
				Directory.CreateDirectory(directoryName);
			}
		}

		private void Application_Startup(object sender, StartupEventArgs e) {
			// Find the AppData folder where the app is stored
			var roamingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			var appDirectory = Path.Combine(roamingDirectory, "ChitChat");
			var logDirectory = Path.Combine(appDirectory, "logs");

			EnsureFolder(logDirectory);

			// Initialize logger to save log files in the log folder inside of app install location
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.Enrich.FromLogContext()
				.WriteTo.File($"{logDirectory}\\log.txt", rollingInterval: RollingInterval.Day)
				.CreateLogger();
		}
	}

}
