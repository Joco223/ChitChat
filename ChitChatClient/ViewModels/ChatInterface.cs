using ChitChatClient.Helpers;
using ChitChatClient.Models;
using ChitChatClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ChitChatClient.ViewModels
{
	public class ChatInterface
	{
		private static readonly ServerService serverService = ServerService.Instance;
		private static readonly UserService userService = UserService.Instance;

		public List<Server> Servers { get; set; }

		public List<User> CurrentServerUsers { get; set; }

		public ChatInterface()
		{
			Servers = [];
			CurrentServerUsers = [];
        }

		// Get joined servers
		public async Task GetServers()
		{
            Servers = await serverService.GetJoinedServers();
        }

		// Leave a server
		public async Task<bool> LeaveServer(Server? server)
		{
			if (server == null)
			{
				return false;
			}
            return await serverService.LeaveServer(server);
        }

		// Get users in a server
		public async Task GetServerUsers(Server? server)
		{
			if (server != null)
			{
				CurrentServerUsers = await userService.GetServerUsers(server);
				CurrentServerUsers.Sort(new OnlineUserComparer());
			}
        }

		public async Task RefreshJoinedServers(Action refreshServerListView)
		{
			// Get all joined servers
			await GetServers();

			refreshServerListView.Invoke();

		}

		public async Task RefreshServerUsers(Action<int?, int?> refreshUserListView, Server? selectedServer)
		{
			if (selectedServer == null)
			{
				return;
			}

			await GetServerUsers(selectedServer);
			selectedServer.OnlineUserCount = await serverService.GetOnlineUserCount(selectedServer);

			refreshUserListView.Invoke(selectedServer.OnlineUserCount, selectedServer.UserCount);
		}
	}
}
