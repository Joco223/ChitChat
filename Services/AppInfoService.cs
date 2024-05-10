using ChitChat.Helpers;
using ChitChat.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace ChitChat.Services
{
	public class AppInfoService
	{
		private SupabaseHandler supabaseHandler = SupabaseHandler.Instance;

		private static AppInfoService instance = new();

		public static AppInfoService Instance { get => instance; }

		public AppInfoService()
		{
        }

		// Gets the value of the app info key
		public async Task<string> GetAppInfo(string key)
		{
			try
			{
				var response = await supabaseHandler.Client.From<AppInfo>().Where(ap => ap.Key == key).Get();

                if (response != null && response.Model != null)
				{
                    return response.Model.Value;
                }
            }
            catch (Exception ex)
			{
                Console.WriteLine(ex.Message);
            }

            return string.Empty;
		}

		// Checks for latest version of the app
		public async Task<bool> CheckForLatestVersion()
		{
            string latestAppVersion = await GetAppInfo("AppVersion");
            string? currentAppVersion = Properties.Settings.Default["AppVersion"].ToString();

            if (string.IsNullOrEmpty(currentAppVersion))
			{
                throw new Exception("App version not found");
            }

            if (latestAppVersion != currentAppVersion)
			{
                return false;
            }

            return true;
        }

		// Downloads the latest version of the app
		private async Task DownloadLatestVersion()
		{
            using (var client = new HttpClient())
            {
                string url = "https://www.example.com/file-to-download.txt";

                var response = await client.GetByteArrayAsync(url);
                File.WriteAllBytes("ChitChat.exe", response);
            }
        }

        // Updates the app
        public async Task UpdateApp()
        {
            File.Move("ChitChat.exe", "ChitChat_old.exe");
			File.SetAttributes("ChitChat_old.exe", File.GetAttributes("ChitChat_old.exe") | FileAttributes.Hidden);
			await DownloadLatestVersion();

            System.Diagnostics.Process.Start("ChitChat.exe");
            Application.Current.Shutdown();
        }

        // Deletes old version if it exists
        public void DeleteOldVersion()
        {
            if (File.Exists("ChitChat_old.exe"))
            {
                File.Delete("ChitChat_old.exe");
            }
        }
	}
}
