using System.Reflection;

using ChitChatClient.Helpers;

using Octokit;

using Serilog;

namespace ChitChatClient.Services {
	/// <summary>
	/// Service to check for updates
	/// </summary>
	public class UpdateService {
		public UpdateService() { }

		/// <summary>
		/// Check for updates
		/// </summary>
		/// <returns>Returns true if there is a new update</returns>
		public static async Task<bool> CheckForUpdates() {
			Version currentVersion = GetCurrentVersion();
			Version latestVersion = await GetLatestVersion();

			return latestVersion > currentVersion;
		}

		/// <summary>
		/// Get the changelog for current version
		/// </summary>
		/// <returns></returns>
		public static async Task<Result<string>> GetCurrentChangelog() {
			GitHubClient client = new(new ProductHeaderValue("ChitChat"));

			var tmp = await client.Repository.Release.GetAll("Joco223", "ChitChat");

			if (tmp.Count == 0)
				return Result<string>.Fail("No releases found");

			Version currentVersion = GetCurrentVersion();

			var target = tmp.Where(r => r.TagName == currentVersion.ToString()).FirstOrDefault();

			if (target == null)
				return Result<string>.Fail("No release found for current version");

			return Result<string>.OK(target.Body);
		}

		/// <summary>
		/// Get the current version of the application
		/// </summary>
		/// <returns>Returns current version object</returns>
		public static Version GetCurrentVersion() => new(Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString() ?? "0.0.0.0");

		/// <summary>
		/// Get the latest version of the application from GitHub
		/// </summary>
		/// <returns>Returns latest version object</returns>
		private static async Task<Version> GetLatestVersion() {
			try {
				GitHubClient client = new(new ProductHeaderValue("ChitChat"));
				Release latestRelease = await client.Repository.Release.GetLatest("Joco223", "ChitChat");



				return new Version(latestRelease.TagName);
			} catch (Exception ex) {
				Log.Error($"Failed to get latest version from GitHub: {ex.Message}");
				throw;
			}
		}
	}
}
