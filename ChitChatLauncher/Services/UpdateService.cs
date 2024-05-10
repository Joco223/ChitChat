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

		public async Task<bool> UpdateApp(Action<float>? progressCallback = null)
		{
			string appPath = GetAppPath();
			EnsureFolder(appPath);

			// Check if app exists
			if (!File.Exists(appPath))
			{
				await DownloadLatestVersion(appPath, progressCallback);
				return true;
			}
			else
			{
				Version currentVersion = GetCurrentVersion();
				Version latestVersion = await GetLatestVersion();

				if (latestVersion > currentVersion)
				{
					File.Delete(appPath);
					await DownloadLatestVersion(appPath, progressCallback);
					return true;
				}
			}

			return false;

		}

		public string GetAppPath()
		{
			return Path.Combine(GetAppDirectory(), "ChitChatClient.exe");
		}

		public Version GetCurrentVersion()
		{
			try
			{
				// Assuming the path to the other application's executable
				string otherAppPath = GetAppPath();

				var versionInfo = FileVersionInfo.GetVersionInfo(otherAppPath);

				// Get the version information
				string version = versionInfo.FileVersion ?? "0.0.0.0";

				return new Version(version);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return new Version("0.0.0.0");
			}
		}

		public async Task<Version> GetLatestVersion()
		{
			var client = new GitHubClient(new ProductHeaderValue("ChitChat"));

			var releases = await client.Repository.Release.GetAll("Joco223", "ChitChat");

			if (releases.Count > 0)
			{
				var latestRelease = releases[0];
				string tagName = latestRelease.TagName;

				return new Version(tagName);
			}
			else
			{
				return new Version("0.0.0.0");
			}
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
		private async Task DownloadLatestVersion(string savePath, Action<float>? progressCallback = null)
		{
			string url = "https://github.com/Joco223/ChitChat/releases/latest/download/ChitChatClient.exe";

			using HttpClient client = new();

			using HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

			response.EnsureSuccessStatusCode();

			using Stream contentStream = await response.Content.ReadAsStreamAsync();

			long totalBytes = response.Content.Headers.ContentLength ?? -1;
			long totalRead = 0;
			byte[] buffer = new byte[8192];
			int read;

			using FileStream fileStream = new(savePath, System.IO.FileMode.Create, FileAccess.Write, FileShare.None);
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
	}
}
