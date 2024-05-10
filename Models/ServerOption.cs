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
        public string Joined { get; set; }

        public ServerOption(Server server, string joined)
        {
            Server = server;
            Joined = joined;
        }

		public ServerOption()
		{
            Server = new();
            Joined = string.Empty;
		}

        public override string ToString()
        {
            return Server.Name + " - " + (Joined == "Yes" ? "Joined" : "Not Joined");
        }
	}
}
