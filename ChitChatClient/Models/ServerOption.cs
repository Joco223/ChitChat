using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitChatClient.Models
{
	public class ServerOption
	{
		public Server Server { get; set; }
		public bool Joined { get; set; }
		public int OnlineUserCount { get; set; }
		public int TotalUserCount { get; set; }

		public string JoinedStatus => Joined ? "Yes" : "No";
		public string UserCount => $"{OnlineUserCount} / {TotalUserCount}";
		public string Name => Server.Name;
		public string Description => Server.Description;

		public ServerOption(Server server, bool joined, int userCount)
		{
			Server = server;
			Joined = joined;
			TotalUserCount = userCount;
			OnlineUserCount = 0;
		}

		public ServerOption()
		{
			Server = new();
			Joined = false;
			TotalUserCount = 0;
			OnlineUserCount = 0;
		}
	}
}
