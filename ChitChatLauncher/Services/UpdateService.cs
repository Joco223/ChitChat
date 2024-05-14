using System.IO;
using System.Net.Http;

using ChitChatLauncher.Helpers;

namespace ChitChatLauncher.Services {
	public class UpdateService {
		private static readonly UpdateService instance = new();

		public static UpdateService Instance { get => instance; }

		public UpdateService() { }

		/// <summary>
		/// Updates the app if a new version is available
		/// </summary>
		/// <param name="progressCallback">Callback for progressbar</param>
		/// <param name="notificationUpdate">Callback for notification display</param>
		/// <returns></returns>
		public async Task UpdateApp(Action<float, string>? progressCallback = null, Action<string>? notificationUpdate = null) {
			string appName = "ChitChatClient.exe";
			string clientHashName = "ChitChatClientSHA256.txt";

			// Get app path
			string appPath = GetAppPath();

			// Get client hash path
			string clientHashPath = Path.Combine(GetAppDirectory(), clientHashName);

			// Ensure app folder exists
			EnsureFolder(appPath);

			// Check if app exists
			if (!File.Exists(appPath)) {
				// App doesn't exist, download new version
				notificationUpdate?.Invoke("Downloading new version - ");
				await DownloadAsset(appPath, appName, progressCallback);
			} else {
				// Download hash checksum from GitHub
				await DownloadAsset(clientHashPath, clientHashName, null);

				// Read new client version hash
				string clientHash = GeneratorSHA256.ReadSHA256FromFile(clientHashPath);

				// Calcilat ecurrent client hash
				string currentClientHash = GeneratorSHA256.GenerateSHA256FromFile(appPath);

				// Compare hashes
				if (clientHash != currentClientHash) {
					// Hashes don't match, download new version
					// This way we ensure there are no unofficial changes to the client
					File.Delete(appPath);
					notificationUpdate?.Invoke("Downloading new version...");
					await DownloadAsset(appPath, appName, progressCallback);
				}
			}
		}

		/// <summary>
		/// Get the path to the app
		/// </summary>
		/// <returns></returns>
		public string GetAppPath() {
			return Path.Combine(GetAppDirectory(), "ChitChatClient.exe");
		}

		/// <summary>
		/// Get the directory of the app in the roaming folder
		/// </summary>
		/// <returns></returns>
		private string GetAppDirectory() {
			var roamingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			return Path.Combine(roamingDirectory, "ChitChat\\");
		}

		/// <summary>
		/// Ensures a folder exists
		/// </summary>
		/// <param name="path"></param>
		void EnsureFolder(string path) {
			string? directoryName = Path.GetDirectoryName(path);

			if (directoryName?.Length > 0) {
				Directory.CreateDirectory(directoryName);
			}
		}

		/// <summary>
		/// Downloads an asset with the passed in name from latest GitHub release
		/// </summary>
		/// <param name="savePath">Path where to save the asset</param>
		/// <param name="fileName">Asset to download</param>
		/// <param name="progressCallback">Callback for progressbar update</param>
		/// <returns></returns>
		private async Task DownloadAsset(string savePath, string fileName, Action<float, string>? progressCallback = null) {
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
			while ((read = await contentStream.ReadAsync(buffer)) > 0) {
				await fileStream.WriteAsync(buffer.AsMemory(0, read));
				totalRead += read;

				if (totalBytes > 0) {
					float percentage = ((float)totalRead / totalBytes) * 100;

					// Update progress bar
					progressCallback?.Invoke(percentage, fileName[..fileName.IndexOf('.')]);
				}
			}
		}
	}
}
