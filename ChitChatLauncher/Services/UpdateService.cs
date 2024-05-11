using ChitChatLauncher.Helpers;
using Octokit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ChitChatLauncher.Services
{
	public class UpdateService
	{
		private static readonly UpdateService instance = new();

		public static UpdateService Instance { get => instance; }

		public UpdateService()
		{
		}

		public async Task<bool> UpdateApp(Action<float, string>? progressCallback = null, Action<string>? notificationUpdate = null)
		{
			string appName = "ChitChatClient.exe";
			string clientHashName = "ChitChatClientSHA256.txt";

			string appPath = GetAppPath();
			string clientHashPath = Path.Combine(GetAppDirectory(), clientHashName);
			
			EnsureFolder(appPath);

			// Check if app exists
			if (!File.Exists(appPath))
			{
				notificationUpdate?.Invoke("Downloading new version - ");
				await DownloadAsset(appPath, appName, progressCallback);
				return true;
			}
			else
			{
				// Check app checksum sha256
				await DownloadAsset(clientHashPath, clientHashName, null);

				string clientHash = GeneratorSHA256.ReadSHA256FromFile(clientHashPath);
				string currentClientHash = GeneratorSHA256.GenerateSHA256FromFile(appPath);

				if (clientHash != currentClientHash)
				{
					File.Delete(appPath);
					notificationUpdate?.Invoke("Downloading new version...");
					await DownloadAsset(appPath, appName, progressCallback);
					return true;
				}
			}

			return false;

		}

		public string GetAppPath()
		{
			return Path.Combine(GetAppDirectory(), "ChitChatClient.exe");
		}

		private string GetAppDirectory()
		{
			var roamingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			return Path.Combine(roamingDirectory, "ChitChat\\");
		}

		void EnsureFolder(string path)
		{
			string? directoryName = Path.GetDirectoryName(path);

			if (directoryName?.Length > 0)
			{
				Directory.CreateDirectory(directoryName);
			}
		}

		// Downloads the latest version of the app
		private async Task DownloadAsset(string savePath, string fileName, Action<float, string>? progressCallback = null)
		{
			string urlBase = "https://github.com/Joco223/ChitChat/releases/latest/download/";
			string url = urlBase + fileName;

			using HttpClient client = new();

			using HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

			response.EnsureSuccessStatusCode();

			using Stream contentStream = await response.Content.ReadAsStreamAsync();

			long totalBytes = response.Content.Headers.ContentLength ?? -1;
			long totalRead = 0;
			byte[] buffer = new byte[5 * 1024 * 1204]; // 5 MB Buffer
			int read;

			using FileStream fileStream = new(savePath, System.IO.FileMode.Create, FileAccess.Write, FileShare.None);
			while ((read = await contentStream.ReadAsync(buffer)) > 0)
			{
				await fileStream.WriteAsync(buffer.AsMemory(0, read));
				totalRead += read;

				if (totalBytes > 0)
				{
					float percentage = ((float)totalRead / totalBytes) * 100;

					progressCallback?.Invoke(percentage, fileName[..fileName.IndexOf('.')]);
				}
			}
		}
	}
}
