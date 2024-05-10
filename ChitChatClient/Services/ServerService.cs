using ChitChatClient.Helpers;
using ChitChatClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitChatClient.Services
{
	public class ServerService
	{
		private static readonly ServerService instance = new();

        public static ServerService Instance { get => instance; }

        private readonly SupabaseHandler supabaseHandler = SupabaseHandler.Instance;
        private readonly UserService userService = UserService.Instance;

		public ServerService() { }

        // Create a server
		public async Task<bool> CreateServer(string name)
		{
            try
			{
                var user = supabaseHandler.Client.Auth.CurrentUser;

                if (user != null && user.Id != null)
				{
                    var userId = await userService.GetUserId();

                    var server = new Server(name, userId);

                    var response = await supabaseHandler.Client.From<Server>().Insert(server);

                    server = response.Model;

                    if (response != null && server != null)
					{
                        // Join the server
                        var userServerJoin = await JoinServer(server.Id);

                        if (userServerJoin)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }

        // Get all Servers
        public async Task<List<Server>> GetServers()
        {
			List<Server> servers = [];

            try
            {
                var request = await supabaseHandler.Client.From<Server>().Get();
                servers = request.Models;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return servers;
        }

        // Get all servers with data if user is joined
        public async Task<List<ServerOption>> GetServerOptions()
        {
            List<ServerOption> serverOptions = [];

            try
            {
                var user = supabaseHandler.Client.Auth.CurrentUser;

                if (user != null && user.Id != null)
                {
                    var serversRequest = await supabaseHandler.Client.From<Server>().Get();
                    var servers = serversRequest.Models;
                    var userId = await userService.GetUserId();
                    
                    var userServerJoinRequest = await supabaseHandler.Client.From<UserServerJoin>().Where(usj => usj.UserId == userId).Get();
                    var userServerJoins = userServerJoinRequest.Models;

                    foreach (var server in servers)
                    {
                        var serverOption = new ServerOption(server, false, server.UserCount);

                        if (userServerJoins.Any(x => x.ServerId == server.Id))
                        {
                            serverOption.Joined = true;
                        }

                        serverOption.OnlineUserCount = await GetOnlineUserCount(server.Id);

                        serverOptions.Add(serverOption);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return serverOptions;
        }

        public async Task<int> GetOnlineUserCount(int serverId)
        {
            var userServerJoinRequest = await supabaseHandler.Client.From<UserServerJoin>().Where(usj => usj.ServerId == serverId).Get();
            var userServerJoins = userServerJoinRequest.Models;

            var serverUsersRequest = await supabaseHandler.Client.From<User>().Filter(x => x.Id, Supabase.Postgrest.Constants.Operator.In, userServerJoins.Select(x => x.UserId).ToList()).Get();
            var serverUsers = serverUsersRequest.Models;

            return serverUsers.Where(u => u.IsOnline).Count();
        }

        // Get joined servers
        public async Task<List<Server>> GetJoinedServers()
        {
            List<Server> servers = [];

            try
            {
                var user = supabaseHandler.Client.Auth.CurrentUser;

                if (user != null && user.Id != null)
                {
                    var userId = await userService.GetUserId();

                    var request = await supabaseHandler.Client.From<UserServerJoin>().Where(usj => usj.UserId == userId).Get();

                    if (request != null)
                    {
                        var serverIds = request.Models.Select(x => x.ServerId).ToList();

                        var serverRequest = await supabaseHandler.Client.From<Server>().Filter(x => x.Id, Supabase.Postgrest.Constants.Operator.In, serverIds).Get();

                        servers = serverRequest.Models;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return servers;
        }

        // Join a server
        public async Task<bool> JoinServer(int serverId)
        {
            try
            {
                var user = supabaseHandler.Client.Auth.CurrentUser;

                if (user != null && user.Id != null)
                {
                    var userId = await userService.GetUserId();

                    var userServerJoin = new UserServerJoin(userId, serverId);

                    var response = await supabaseHandler.Client.From<UserServerJoin>().Insert(userServerJoin);

                    if (response != null)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }

        // Leave a server
        public async Task<bool> LeaveServer(int serverId)
        {
            try
            {
                Supabase.Gotrue.User? user = supabaseHandler.Client.Auth.CurrentUser;

                if (user != null && user.Id != null)
                {
                    var userId = await userService.GetUserId();

                    await supabaseHandler.Client.From<UserServerJoin>().Where(usj => usj.UserId == userId && usj.ServerId == serverId).Delete();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }
	}
}
