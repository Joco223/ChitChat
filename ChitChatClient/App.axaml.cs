using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ChitChatClient.Services;
using ChitChatClient.ViewModels;
using ChitChatClient.Views;
using Serilog;

namespace ChitChatClient;

public partial class App : Application
{
	private readonly UserService UserService = UserService.Instance;

	public override void Initialize()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public override void OnFrameworkInitializationCompleted()
	{
		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			desktop.Exit += OnExit;
			desktop.MainWindow = new MainWindow
			{
				DataContext = new MainWindowViewModel(),
			};
		}

		base.OnFrameworkInitializationCompleted();
	}

	private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
	{
		try {
				// Only set offline status if user is logged in
				Task.Run(() => UserService.SetUserOnlineStatus(false)).Wait();

				Log.CloseAndFlush();
			} catch (Exception) {
				Log.Error("Unable to log out user");
				//MessageBox.Show("Unable to log out user!", "Error logging out", MessageBoxButton.OK, MessageBoxImage.Error);
			}
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

	private void OnStartup(object s, ControlledApplicationLifetimeStartupEventArgs e)
	{
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