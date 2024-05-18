using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChitChat.Helpers;
using ChitChat.Models;
using ChitChat.Services;

namespace ChitChat.ViewModels {
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
		public async Task<Result<string>> GetServers() {
			var servers = await serverService.GetJoinedServers();

			if (servers.Failed)
				return Result<string>.Fail("Error getting servers.");

			Servers = servers.Data;

			return Result<string>.OK("Servers fetched successfully");
		}

		/// <summary>
		/// Joins a server
		/// </summary>
		/// <param name="server">Server to leave</param>
		/// <returns></returns>
		public async Task<Result<string>> LeaveServer(Server server) {
			var res = await serverService.LeaveServer(server);

			if (res.Failed)
				return Result<string>.Fail("Error leaving server.");

			return Result<string>.OK("Server left successfully");
		}

		/// <summary>
		/// Gets all users in a server
		/// </summary>
		/// <param name="server">Server to get users from</param>
		/// <returns></returns>
		public async Task<Result<string>> GetServerUsers(Server server) {
			var currentUsers = await userService.GetServerUsers(server);

			if (currentUsers.Failed)
				return Result<string>.Fail("Error getting users.");

			CurrentServerUsers = currentUsers.Data;

			CurrentServerUsers.Sort(new OnlineUserComparer());

			return Result<string>.OK("Users fetched successfully");
		}

		/// <summary>
		/// Gets all channels in a server
		/// </summary>
		/// <param name="server">Server to get channels from</param>
		/// <returns></returns>
		public async Task<Result<string>> GetServerChannels(Server server) {
			var currentChannels = await channelService.GetChannels(server);

			if (currentChannels.Failed)
				return Result<string>.Fail("Error getting channels.");

			CurrentServerChannels = currentChannels.Data;

			return Result<string>.OK("Channels fetched successfully");
		}

		/// <summary>
		/// Refreshes the list of joined servers
		/// </summary>
		/// <param name="refreshServerListView">Callback when server list is updated</param>
		/// <returns></returns>
		public async Task<Result<string>> RefreshJoinedServers(Action refreshServerListView) {
			var res = await GetServers();

			if (res.Failed)
				return Result<string>.Fail("Error refreshing servers.");

			refreshServerListView.Invoke();

			return Result<string>.OK("Servers refreshed successfully");
		}

		/// <summary>
		/// Refreshes the list of users in a server
		/// </summary>
		/// <param name="refreshUserListView">Callback to call when user list is updated</param>
		/// <param name="selectedServer">Server to get users from</param>
		/// <returns></returns>
		public async Task<Result<string>> RefreshServerUsers(Action<int?, int?> refreshUserListView, Server selectedServer) {
			var users = await GetServerUsers(selectedServer);

			if (users.Failed)
				return Result<string>.Fail("Error refreshing users.");

			var userCount = await serverService.GetOnlineUserCount(selectedServer);

			if (userCount.Failed)
				return Result<string>.Fail("Error getting user count.");

			selectedServer.OnlineUserCount = userCount.Data;

			refreshUserListView.Invoke(selectedServer.OnlineUserCount, selectedServer.UserCount);

			return Result<string>.OK("Users refreshed successfully");
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
