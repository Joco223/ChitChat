using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ChitChatClient.Services
{
	public class UpdateService
	{
		private static readonly UpdateService instance = new();

		public static UpdateService Instance { get => instance; }

		public UpdateService()
		{
		}

		public async Task<bool> CheckForUpdates()
		{
			Version currentVersion = GetCurrentVersion();
			Version latestVersion = await GetLatestVersion();

			return latestVersion > currentVersion;
		}

		private Version GetCurrentVersion()
		{
			return new Version(Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString() ?? "0.0.0.0");
		}

		private async Task<Version> GetLatestVersion()
		{
			try
			{
				GitHubClient client = new(new ProductHeaderValue("ChitChat"));
				Release latestRelease = await client.Repository.Release.GetLatest("Joco223", "ChitChat");

				return new Version(latestRelease.TagName);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return new Version(0, 0, 0, 0);
			}
		}
	}
}
