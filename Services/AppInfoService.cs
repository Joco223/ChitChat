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
		private async Task DownloadLatestVersion(Action<float>? progressCallback = null)
		{
            string url = "https://github.com/Joco223/ChitChat/releases/latest/download/ChitChat.exe";
            string savePath = "ChitChat_new.exe";

			using HttpClient client = new();

			using HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

			response.EnsureSuccessStatusCode();

			using Stream contentStream = await response.Content.ReadAsStreamAsync();

			long totalBytes = response.Content.Headers.ContentLength ?? -1;
			long totalRead = 0;
			byte[] buffer = new byte[8192];
			int read;

			using FileStream fileStream = new(savePath, FileMode.Create, FileAccess.Write, FileShare.None);
			while ((read = await contentStream.ReadAsync(buffer)) > 0)
			{
				await fileStream.WriteAsync(buffer.AsMemory(0, read));
				totalRead += read;

				if (totalBytes > 0)
				{
					float percentage = ((float)totalRead / totalBytes) * 100;

					progressCallback?.Invoke(percentage);
				}
			}
		}

        // Updates the app
        public async Task UpdateApp(Action<float>? progressCallback = null)
        {
            File.Move("ChitChat.exe", "ChitChat_old.exe");
			File.SetAttributes("ChitChat_old.exe", File.GetAttributes("ChitChat_old.exe") | FileAttributes.Hidden);
			await DownloadLatestVersion(progressCallback);

            System.Diagnostics.Process.Start("ChitChat_new.exe");
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

		// Rename new copy to ChitChat.exe
		public void RenameNewVersion()
		{
            if (File.Exists("ChitChat_new.exe"))
			{
                File.Move("ChitChat_new.exe", "ChitChat.exe");
            }
        }
	}
}
