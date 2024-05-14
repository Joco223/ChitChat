using ChitChatClient.Models;
using ChitChatClient.Services;

namespace ChitChatClient.ViewModels {

	/// <summary>
	/// Class to handle server list operations
	/// </summary>
	public class ServerList {
		private readonly ServerService serverService = ServerService.Instance;

		public List<ServerOption> Servers { get; set; }

		public ServerList() {
			Servers = [];
		}

		/// <summary>
		/// gets all servers user can join
		/// </summary>
		/// <returns></returns>
		public async Task GetServerOptions() {
			try {
				Servers = await serverService.GetServerOptions();
			} catch {
				throw;
			}
		}
	}
}
