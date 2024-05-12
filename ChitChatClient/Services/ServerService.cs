using ChitChatClient.Helpers;
using ChitChatClient.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        /// <summary>
        /// Creates a new server and automatically joins the server
        /// </summary>
        /// <param name="name">Name of the new server</param>
        /// <returns></returns>
		public async Task<bool> CreateServer(string name, string description)
		{
            try
			{
                var user = supabaseHandler.Client.Auth.CurrentUser;

                if (user != null && user.Id != null)
				{
                    var userId = await userService.GetUserId();

                    var server = new Server(name, userId, description);

                    var response = await supabaseHandler.Client.From<Server>().Insert(server);

                    server = response.Model;

                    if (response != null && server != null)
					{
                        // Join the server
                        var userServerJoin = await JoinServer(server);

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
                Debug.WriteLine(ex.Message);
            }

            return false;
        }

        /// <summary>
        /// Gets all servers in database
        /// </summary>
        /// <returns></returns>
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
                Debug.WriteLine(ex.Message);
            }

            return servers;
        }

        /// <summary>
        /// Gets all servers with additional information:
        /// Is user joined, server name, online user count, total user count
        /// </summary>
        /// <returns></returns>
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

                        serverOption.OnlineUserCount = await GetOnlineUserCount(server);

                        serverOptions.Add(serverOption);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return serverOptions;
        }

        /// <summary>
        /// Gets online user count in a server
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public async Task<int> GetOnlineUserCount(Server server)
        {
            var userServerJoinRequest = await supabaseHandler.Client.From<UserServerJoin>().Where(usj => usj.ServerId == server.Id).Get();
            var userServerJoins = userServerJoinRequest.Models;

            var serverUsersRequest = await supabaseHandler.Client.From<User>().Filter(x => x.Id, Supabase.Postgrest.Constants.Operator.In, userServerJoins.Select(x => x.UserId).ToList()).Get();
            var serverUsers = serverUsersRequest.Models;

           return serverUsers.Where(u => u.IsOnline).Count();
        }

        /// <summary>
        /// Gets all servers current user has joined
        /// </summary>
        /// <returns></returns>
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
                Debug.WriteLine(ex.Message);
            }

            return servers;
        }

        /// <summary>
        /// Joins the new server
        /// </summary>
        /// <param name="server">Server to join</param>
        /// <returns>True if sucess, false if not</returns>
        public async Task<bool> JoinServer(Server? server)
        {
            try
            {
                if (server == null)
                {
                    return false;
				}

                var user = supabaseHandler.Client.Auth.CurrentUser;

                if (user != null && user.Id != null)
                {
                    var userId = await userService.GetUserId();

                    var userServerJoin = new UserServerJoin(userId, server.Id);

                    var response = await supabaseHandler.Client.From<UserServerJoin>().Insert(userServerJoin);

                    if (response != null)
                    {
                        var serverRequest = await supabaseHandler.Client.From<Server>().Where(s => s.Id == server.Id).Get();
                        var joinedServer = serverRequest.Model;

                        if (joinedServer != null)
                        {
							joinedServer.UserCount += 1;

							await supabaseHandler.Client.From<Server>().Update(joinedServer);
                            
                            return true;
						}
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return false;
        }

        /// <summary>
        /// Remove current user from server
        /// </summary>
        /// <param name="server">Server to leave</param>
        /// <returns></returns>
        public async Task<bool> LeaveServer(Server? server)
        {
            try
            {
                if (server == null)
                {
					return false;
				}

                Supabase.Gotrue.User? user = supabaseHandler.Client.Auth.CurrentUser;

                if (user != null && user.Id != null)
                {
                    var userId = await userService.GetUserId();

                    await supabaseHandler.Client.From<UserServerJoin>().Where(usj => usj.UserId == userId && usj.ServerId == server.Id).Delete();

					var serverRequest = await supabaseHandler.Client.From<Server>().Where(s => s.Id == server.Id).Get();
					var joinedServer = serverRequest.Model;

					if (joinedServer != null)
					{
						joinedServer.UserCount -= 1;

						await supabaseHandler.Client.From<Server>().Update(joinedServer);

						return true;
					}
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return false;
        }

		/// <summary>
		/// Checks if the current user is the owner of the server
		/// </summary>
		/// <param name="server">Server to check for</param>
		/// <returns>True if logged in user is the owner, false if not</returns>
		public async Task<bool> IsServerOwner(Server server)
        {
			try
            {
				var user = supabaseHandler.Client.Auth.CurrentUser;

				if (user != null && user.Id != null)
                {
					var userId = await userService.GetUserId();

					if (userId == server.CreatedBy)
                    {
						return true;
					}
				}
			}
			catch (Exception ex)
            {
				Debug.WriteLine(ex.Message);
			}

			return false;
		}

        /// <summary>
        /// Update server information
        /// </summary>
        /// <param name="server">Server to update</param>
        /// <returns>True if update succesful, false if not</returns>
        public async Task<bool> UpdateServer(Server server)
        {
			try
            {
				var response = await supabaseHandler.Client.From<Server>().Update(server);

				if (response != null)
                {
					return true;
				}
			}
			catch (Exception ex)
            {
				Debug.WriteLine(ex.Message);
			}

			return false;
		}
	}
}
