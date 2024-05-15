using ChitChatClient.Helpers;
using ChitChatClient.Models;
using ChitChatClient.Services;

namespace ChitChatClient.ViewModels {
	/// <summary>
	/// Class to handle chat interface operations
	/// </summary>
	public class ChatInterface {
		private static readonly ServerService serverService = ServerService.Instance;
		private static readonly UserService userService = UserService.Instance;
		private static readonly ChannelService channelService = ChannelService.Instance;

		public List<Server> Servers { get; set; }

		public List<User> CurrentServerUsers { get; set; }

		public List<Channel> CurrentServerChannels { get; set; }

		public ChatInterface() {
			Servers = [];
			CurrentServerUsers = [];
			CurrentServerChannels = [];
		}

		/// <summary>
		/// Gets all joined servers
		/// </summary>
		/// <returns></returns>
		public async Task GetServers() {
			try {
				Servers = await serverService.GetJoinedServers();
			} catch {
				throw;
			}
		}

		/// <summary>
		/// Joins a server
		/// </summary>
		/// <param name="server">Server to leave</param>
		/// <returns></returns>
		public async Task LeaveServer(Server server) {
			try {
				await serverService.LeaveServer(server);
				return;
			} catch {
				throw;
			}
		}

		/// <summary>
		/// Gets all users in a server
		/// </summary>
		/// <param name="server">Server to get users from</param>
		/// <returns></returns>
		public async Task GetServerUsers(Server server) {
			try {
				CurrentServerUsers = await userService.GetServerUsers(server);
				CurrentServerUsers.Sort(new OnlineUserComparer());
			} catch {
				throw;
			}
		}

		/// <summary>
		/// Gets all channels in a server
		/// </summary>
		/// <param name="server">Server to get channels from</param>
		/// <returns></returns>
		public async Task GetServerChannels(Server server) {
			try {
				CurrentServerChannels = await channelService.GetChannels(server);
			} catch {
				throw;
			}
		}

		/// <summary>
		/// Refreshes the list of joined servers
		/// </summary>
		/// <param name="refreshServerListView">Callback when server list is updated</param>
		/// <returns></returns>
		public async Task RefreshJoinedServers(Action refreshServerListView) {
			try {
				await GetServers();

				refreshServerListView.Invoke();
			} catch {
				throw;
			}
		}

		/// <summary>
		/// Refreshes the list of users in a server
		/// </summary>
		/// <param name="refreshUserListView">Callback to call when user list is updated</param>
		/// <param name="selectedServer">Server to get users from</param>
		/// <returns></returns>
		public async Task RefreshServerUsers(Action<int?, int?> refreshUserListView, Server selectedServer) {
			try {
				await GetServerUsers(selectedServer);

				selectedServer.OnlineUserCount = await serverService.GetOnlineUserCount(selectedServer);

				refreshUserListView.Invoke(selectedServer.OnlineUserCount, selectedServer.UserCount);
			} catch {
				throw;
			}
		}

		/// <summary>
		/// Checks if there are any servers
		/// </summary>
		/// <returns></returns>
		public bool HasServers() => Servers.Count > 0;

		/// <summary>
		/// Checks if there are any channels in the current server
		/// </summary>
		/// <returns></returns>
		public bool HasChannels() => CurrentServerChannels.Count > 0;
	}
}
