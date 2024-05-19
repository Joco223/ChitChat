using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ChitChat.Helpers;
using ChitChat.DatabaseModels;
using ChitChat.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using ChitChat.Models;

namespace ChitChat.ViewModels {
	/// <summary>
	/// Class to handle chat interface operations
	/// </summary>
	public partial class ChatInterface : ObservableObject {
		private static readonly ServerService serverService = ServerService.Instance;
		private static readonly UserService userService = UserService.Instance;
		private static readonly ChannelService channelService = ChannelService.Instance;

		[ObservableProperty]
		private List<Server> servers;

		[ObservableProperty]
		public List<ServerUser> currentServerUsers;

		[ObservableProperty]
		public List<Channel> currentServerChannels;

		[ObservableProperty]
		public ObservableCollection<Server> serversFiltered;

		[ObservableProperty]
		public ObservableCollection<ServerUser> usersFiltered;

		public string ServerFilterText { get; set; } = string.Empty;

		public string UserFilterText { get; set; } = string.Empty;

		public ChatInterface() {
			servers = [];
			currentServerUsers = [];
			currentServerChannels = [];
			serversFiltered = [];
			usersFiltered = [];
		}

		/// <summary>
		/// Filters servers based on search text
		/// </summary>
		public void FilterServers() {
			ServersFiltered.Clear();

			// Check if SearchText is empty
			if (string.IsNullOrEmpty(ServerFilterText)) {
				ServersFiltered.AddRange(Servers);
				return;
			}

			ServersFiltered.AddRange(Servers.Where(server => server.Name.Contains(ServerFilterText, StringComparison.CurrentCultureIgnoreCase)));
		}

		/// <summary>
		/// Filters users based on search text
		/// </summary>
		public void FilterUsers() {
			UsersFiltered.Clear();

			// Check if SearchText is empty
			if (string.IsNullOrEmpty(UserFilterText)) {
				UsersFiltered.AddRange(CurrentServerUsers);
				return;
			}

			UsersFiltered.AddRange(CurrentServerUsers.Where(user => user.User.Username.Contains(UserFilterText, StringComparison.CurrentCultureIgnoreCase)));
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

			// Sort Servers by name
			Servers.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));

			// Update filtered servers
			FilterServers();

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

			CurrentServerUsers = ServerUser.ConvertToServerUserList(currentUsers.Data);

			// Sort users by online status and then username
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

			// Sort channels by name
			CurrentServerChannels.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));

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

			// Update filtered users
			FilterUsers();

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
