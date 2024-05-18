
using System;

namespace ChitChat.Models {
	/// <summary>
	/// Represents a server option for the user to choose from
	/// when joining a new server
	/// </summary>
	public class ServerOption {
		public Server Server { get; set; }
		public bool Joined { get; set; }
		public int OnlineUserCount { get; set; }
		public int TotalUserCount { get; set; }

		public string JoinedStatus => Joined ? "Yes" : "No";
		public string UserCount => $"{OnlineUserCount} / {TotalUserCount}";
		public string Name => Server.Name;
		public string Description => Server.Description;

		public ServerOption(Server server, bool joined, int totalUserCount) {
			Server = server ?? throw new ArgumentNullException(nameof(server));
			Joined = joined;
			TotalUserCount = totalUserCount;
			OnlineUserCount = 0;
		}

		public ServerOption() {
			Server = new();
			Joined = false;
			TotalUserCount = 0;
			OnlineUserCount = 0;
		}

		public string GetDebugInfo() => $"Server: {Server.Name}, Joined: {Joined}, Online Users: {OnlineUserCount}, Total Users: {TotalUserCount}";
	}
}
