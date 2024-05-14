using ChitChatClient.Helpers;
using ChitChatClient.Models;

using Serilog;

namespace ChitChatClient.Services {
	/// <summary>
	/// Server service for handling server related operations
	/// </summary>
	public class ServerService {
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
		public async Task CreateServer(string name, string description) {
			var server = new Server(name, description);

			try {
				// Get current user
				var user = supabaseHandler.Client.Auth.CurrentUser;

				if (user != null && user.Id != null) {
					// Get user id from Users table in database
					var userId = await userService.GetUserId();

					// Set the server owner
					server.CreatedBy = userId;

					// Insert the server to database
					var response = await supabaseHandler.Client.From<Server>().Insert(server);
					server = response.Model;

					// Check if server is created
					if (response != null && server != null) {
						// Join the server
						await JoinServer(server);
					} else {
						Log.Error("Error creating server");
						throw new Exception("Error creating server");
					}
				} else {
					Log.Error($"Errog getting current user info when creating server {server?.GetDebugInfo()}");
					throw new Exception("Error getting current user info");
				}
			} catch (Exception ex) {
				Log.Error($"Error creating server {server?.GetDebugInfo()} - {ex.Message}");
				throw;
			}
		}

		/// <summary>
		/// Gets all servers in database
		/// </summary>
		/// <returns></returns>
		public async Task<List<Server>> GetServers() {
			List<Server> servers = [];

			try {
				// Get all servers from database
				var request = await supabaseHandler.Client.From<Server>().Get();
				servers = request.Models;
			} catch (Exception ex) {
				Log.Error($"Error getting all servesr {ex.Message}");
			}

			return servers;
		}

		/// <summary>
		/// Gets all servers with additional information:
		/// Is user joined, server name, online user count, total user count
		/// </summary>
		/// <returns></returns>
		public async Task<List<ServerOption>> GetServerOptions() {

			try {
				// Get current user
				var user = supabaseHandler.Client.Auth.CurrentUser;

				if (user != null && user.Id != null) {

					// Get all servers
					var serversRequest = await supabaseHandler.Client.From<Server>().Get();
					var servers = serversRequest.Models;

					// Get current user ID from Users table
					var userId = await userService.GetUserId();

					// Get all user-server joins where user ID is current user ID
					var userServerJoinRequest = await supabaseHandler.Client.From<UserServerJoin>().Where(usj => usj.UserId == userId).Get();
					var userServerJoins = userServerJoinRequest.Models;

					List<ServerOption> serverOptions = [];

					// Construct server options list
					foreach (var server in servers) {
						var serverOption = new ServerOption(server, false, server.UserCount);

						// Check if current user has joined the server
						if (userServerJoins.Any(x => x.ServerId == server.Id)) {
							serverOption.Joined = true;
						}

						// Get online user count in server
						serverOption.OnlineUserCount = await GetOnlineUserCount(server);

						serverOptions.Add(serverOption);
					}

					return serverOptions;
				} else {
					Log.Error($"Error getting current user info when constructing server options list");
					throw new Exception("Error getting current user info");
				}
			} catch (Exception ex) {
				Log.Error($"Error construction server options list {ex.Message}");
				throw;
			}
		}

		/// <summary>
		/// Gets online user count in a server
		/// </summary>
		/// <param name="server"></param>
		/// <returns></returns>
		public async Task<int> GetOnlineUserCount(Server server) {
			// Get all user-server joins where server ID is server ID
			var userServerJoinRequest = await supabaseHandler.Client.From<UserServerJoin>().Where(usj => usj.ServerId == server.Id).Get();
			var userServerJoins = userServerJoinRequest.Models;

			// Get all users where user ID is in user-server joins
			var serverUsersRequest = await supabaseHandler.Client.From<User>().Filter(x => x.Id, Supabase.Postgrest.Constants.Operator.In, userServerJoins.Select(x => x.UserId).ToList()).Get();
			var serverUsers = serverUsersRequest.Models;

			// Return online user count
			return serverUsers.Where(u => u.IsOnline).Count();
		}

		/// <summary>
		/// Gets all servers current user has joined
		/// </summary>
		/// <returns></returns>
		public async Task<List<Server>> GetJoinedServers() {
			try {
				// Get the current user
				var user = supabaseHandler.Client.Auth.CurrentUser;

				if (user != null && user.Id != null) {
					// Get the current user ID from Users table
					var userId = await userService.GetUserId();

					// Get all user-server joins where user ID is current user ID
					var request = await supabaseHandler.Client.From<UserServerJoin>().Where(usj => usj.UserId == userId).Get();

					if (request != null) {
						// Construct a list of server IDs from user-server joins
						var serverIds = request.Models.Select(x => x.ServerId).ToList();

						// Get all servers where server ID is in server IDs
						var serverRequest = await supabaseHandler.Client.From<Server>().Filter(x => x.Id, Supabase.Postgrest.Constants.Operator.In, serverIds).Get();

						return serverRequest.Models;
					} else {
						Log.Error("Error getting joined servers.");
						return [];
					}
				} else {
					Log.Error("Error getting current user info when getting joined servers.");
					throw new Exception("Error getting current user info");
				}
			} catch (Exception ex) {
				Log.Error($"Error getting joined servers {ex.Message}");
				throw;
			}
		}

		/// <summary>
		/// Joins the new server
		/// </summary>
		/// <param name="server">Server to join</param>
		/// <returns>True if sucess, false if not</returns>
		public async Task JoinServer(Server server) {
			try {
				// Get the current user
				var user = supabaseHandler.Client.Auth.CurrentUser;

				if (user != null && user.Id != null) {
					// Get the current user id from Users table
					var userId = await userService.GetUserId();

					var userServerJoin = new UserServerJoin(userId, server.Id);

					// Insert the user-server join to database
					var response = await supabaseHandler.Client.From<UserServerJoin>().Insert(userServerJoin);

					if (response != null) {
						// Get the server from database
						var serverRequest = await supabaseHandler.Client.From<Server>().Where(s => s.Id == server.Id).Get();
						var joinedServer = serverRequest.Model;

						if (joinedServer != null) {
							// Increment the user count in server
							joinedServer.UserCount += 1;

							// Update the server in database
							await supabaseHandler.Client.From<Server>().Update(joinedServer);
						} else {
							Log.Error($"Error upading user count in server {server.GetDebugInfo()}");
							throw new Exception("Error updating user count in server");
						}
					} else {
						Log.Error($"Error getting server from database when updaing user count {server.GetDebugInfo()}");
						throw new Exception("Error getting server from database when updaing user count");
					}
				} else {
					Log.Error($"Error getting current user info when joining server {server.GetDebugInfo()}");
					throw new Exception("Error getting current user info");
				}
			} catch (Exception ex) {
				Log.Error($"Error joining server {server.GetDebugInfo()} - {ex.Message}");
				throw;
			}
		}

		/// <summary>
		/// Remove current user from server
		/// </summary>
		/// <param name="server">Server to leave</param>
		/// <returns></returns>
		public async Task LeaveServer(Server server) {
			try {
				// Get current user
				var user = supabaseHandler.Client.Auth.CurrentUser;

				if (user != null && user.Id != null) {
					// Get current user if from Users table
					var userId = await userService.GetUserId();

					// Delete user-server join from database where user ID is current user ID and server ID is passed in server ID
					await supabaseHandler.Client.From<UserServerJoin>().Where(usj => usj.UserId == userId && usj.ServerId == server.Id).Delete();

					// Get the server from database where server ID is passed inserver ID
					var serverRequest = await supabaseHandler.Client.From<Server>().Where(s => s.Id == server.Id).Get();
					var joinedServer = serverRequest.Model;

					if (joinedServer != null) {
						// Decrement the user count in server
						joinedServer.UserCount -= 1;

						// Update the server in database
						await supabaseHandler.Client.From<Server>().Update(joinedServer);
					} else {
						Log.Error("Error getting server from database when leaving server");
						throw new Exception("Error getting server from database when leaving server");
					}
				} else {
					Log.Error($"Error getting current user info when leaving server {server.GetDebugInfo()}");
					throw new Exception("Error getting current user info");
				}
			} catch (Exception ex) {
				Log.Error($"Error leaving server {server.GetDebugInfo()} - {ex.Message}");
				throw;
			}
		}

		/// <summary>
		/// Checks if the current user is the owner of the server
		/// </summary>
		/// <param name="server">Server to check for</param>
		/// <returns>True if logged in user is the owner, false if not</returns>
		public async Task<bool> IsServerOwner(Server server) {
			try {
				// get current user
				var user = supabaseHandler.Client.Auth.CurrentUser;

				if (user != null && user.Id != null) {
					// Get current user if from Users table
					var userId = await userService.GetUserId();

					return userId == server.CreatedBy;
				} else {
					Log.Error("Error getting current user info when checking server owner");
					throw new Exception("Error getting current user info");
				}
			} catch (Exception ex) {
				Log.Error($"Error checking server owner {server.GetDebugInfo()} - {ex.Message}");
				throw;
			}
		}

		/// <summary>
		/// Update server information
		/// </summary>
		/// <param name="server">Server to update</param>
		/// <returns>True if update succesful, false if not</returns>
		public async Task UpdateServer(Server server) {
			try {
				// Update the server in database
				var response = await supabaseHandler.Client.From<Server>().Update(server);

				if (response == null) {
					Log.Error($"Error updating server in database {server.GetDebugInfo()}");
					throw new Exception("Error updating server in database");
				}
			} catch (Exception ex) {
				Log.Error($"Error updating server {server.GetDebugInfo()} - {ex.Message}");
				throw;
			}
		}
	}
}
