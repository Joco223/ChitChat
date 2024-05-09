using ChitChat.Models;
using ChitChat.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitChat.ViewModels
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
		public async Task<bool> LeaveServer(int serverId)
		{
            return await serverService.LeaveServer(serverId);
        }

		// Get users in a server
		public async Task GetServerUsers(int serverId)
		{
			if (serverId != -1)
			{
				CurrentServerUsers = await userService.GetServerUsers(serverId);
			}
        }
	}
}
