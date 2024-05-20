using System.Collections.Generic;
using System.Threading.Tasks;
using ChitChatClient.Helpers;
using ChitChatClient.Services;
using ChitChatClient.Models;

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
		public async Task<Result<string>> GetServerOptions() {
			var result = await serverService.GetServerOptions();

			if (result.Failed) {
				return Result<string>.Fail(result.Error);
			}

			Servers = result.Data;

			return Result<string>.OK("Success");
		}
	}
}
