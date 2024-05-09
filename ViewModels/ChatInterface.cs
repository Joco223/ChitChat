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

		public List<Server> Servers { get; set; }
		public ChatInterface()
		{
			Servers = [];
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
	}
}
