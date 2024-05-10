using ChitChatClient.Models;
using ChitChatClient.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ChitChatClient.ViewModels
{

	public class ServerList
	{
		private readonly ServerService serverService = ServerService.Instance;

		public List<ServerOption> Servers { get; set; }

		public ServerList()
		{
			Servers = [];
        }

        // Get all servers
        public async Task GetServerOptions()
		{
            Servers = await serverService.GetServerOptions();
        }
	}
}
