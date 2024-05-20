using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChitChatClient.Helpers;
using ChitChatClient.DatabaseModels;

using Serilog;
using ChitChatClient.Models;

namespace ChitChatClient.Services {
	/// <summary>
	/// Server service for handling server related operations
	/// </summary>
	public class ServerService {
		private static readonly ServerService instance = new();

		public static ServerService Instance { get => instance; }

		private readonly SupabaseService SupabaseService = SupabaseService.Instance;
		private readonly UserService UserService = UserService.Instance;

		public ServerService() { }

		/// <summary>
		/// Creates a new server and automatically joins the server
		/// </summary>
		/// <param name="name">Name of the new server</param>
		/// <param name="description">Description of the server</param>
		/// <returns></returns>
		public async Task<Result<string>> CreateServer(string name, string description) {
			// Get user id from Users table in database
			var userId = await UserService.CheckLoggedInUser();

			// Check if user id is null
			if (userId.Failed) {
				Log.Error("Error getting user ID when creating server");
				return Result<string>.Fail("Error getting user ID");
			}

			var server = new Server(name, description)
			{
				// Set the server owner
				CreatedBy = userId.Data
			};

			// Insert the server to database
			var response = await SupabaseService.Client.From<Server>().Insert(server);

			// Check if server is created
			if (response == null) {
				Log.Error("Error creating server");
				return Result<string>.Fail("Error creating server");
			}

			server = response.Model;

			// Check if server is not null
			if (server == null) {
				Log.Error("Error getting server from database when creating server");
				return Result<string>.Fail("Error getting server from database");
			}

			await JoinServer(server);


			return Result<string>.OK("Server created successfully");
		}

		/// <summary>
		/// Gets all servers in database
		/// </summary>
		/// <returns></returns>
		public async Task<Result<List<Server>>> GetServers() {
			List<Server> servers = [];

			var request = await SupabaseService.Client.From<Server>().Get();

			if (request == null || request.Models == null) {
				Log.Error("Error getting servers from database");
				return Result<List<Server>>.Fail("Error getting servers from database");
			}

			servers = request.Models;

			return Result<List<Server>>.OK(servers);
		}

		/// <summary>
		/// Gets all servers with additional information:
		/// Is user joined, server name, online user count, total user count
		/// </summary>
		/// <returns></returns>
		public async Task<Result<List<ServerOption>>> GetServerOptions() {

			// Get current user ID from Users table
			var userId = await UserService.CheckLoggedInUser();

			// Check if user ID is null
			if (userId.Failed) {
				Log.Error("Error getting user ID when constructing server options list");
				return Result<List<ServerOption>>.Fail("Error getting user ID");
			}

			// Get all servers
			var serversRequest = await SupabaseService.Client.From<Server>().Get();

			// Check if servers are null
			if (serversRequest == null || serversRequest.Models == null) {
				Log.Error("Error getting servers from database when constructing server options list");
				return Result<List<ServerOption>>.Fail("Error getting servers from database");
			}

			var servers = serversRequest.Models;

			// Get all user-server joins where user ID is current user ID
			var userServerJoinRequest = await SupabaseService.Client.From<UserServerJoin>().Where(usj => usj.UserId == userId.Data).Get();

			// Check if user-server joins are null
			if (userServerJoinRequest == null || userServerJoinRequest.Models == null) {
				Log.Error("Error getting user-server joins when constructing server options list");
				return Result<List<ServerOption>>.Fail("Error getting user-server joins");
			}

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
				var userCount = await GetOnlineUserCount(server);

				// Check if user count is null
				if (userCount.Failed) {
					Log.Error("Error getting online user count when constructing server options list");
					return Result<List<ServerOption>>.Fail("Error getting online user count");
				}

				serverOption.OnlineUserCount = userCount.Data;

				serverOptions.Add(serverOption);
			}

			return Result<List<ServerOption>>.OK(serverOptions);
		}

		/// <summary>
		/// Gets online user count in a server
		/// </summary>
		/// <param name="server"></param>
		/// <returns></returns>
		public async Task<Result<int>> GetOnlineUserCount(Server server) {
			// Get all user-server joins where server ID is server ID
			var userServerJoinRequest = await SupabaseService.Client.From<UserServerJoin>().Where(usj => usj.ServerId == server.Id).Get();

			// Check if user-server joins are null
			if (userServerJoinRequest == null || userServerJoinRequest.Models == null) {
				Log.Error("Error getting user-server joins when getting online user count");
				return Result<int>.Fail("Error getting user-server joins");
			}

			var userServerJoins = userServerJoinRequest.Models;

			// Get all users where user ID is in user-server joins
			var serverUsersRequest = await SupabaseService.Client.From<User>().Filter(x => x.Id, Supabase.Postgrest.Constants.Operator.In, userServerJoins.Select(x => x.UserId).ToList()).Get();

			// Check if users are null
			if (serverUsersRequest == null || serverUsersRequest.Models == null) {
				Log.Error("Error getting users from database when getting online user count");
				return Result<int>.Fail("Error getting users from database");
			}

			var serverUsers = serverUsersRequest.Models;

			// Return online user count
			return Result<int>.OK(serverUsers.Where(x => x.IsOnline).Count());
		}

		/// <summary>
		/// Gets all servers current user has joined
		/// </summary>
		/// <returns></returns>
		public async Task<Result<List<Server>>> GetJoinedServers() {
			// Get the current user ID from Users table
			var userId = await UserService.CheckLoggedInUser();

			// Check if user ID is null
			if (userId.Failed) {
				Log.Error("Error getting user ID when getting joined servers");
				return Result<List<Server>>.Fail("Error getting user ID");
			}

			// Get all user-server joins where user ID is current user ID
			var request = await SupabaseService.Client.From<UserServerJoin>().Where(usj => usj.UserId == userId.Data).Get();

			// Check if user-server joins are null
			if (request == null) {
				Log.Error("Error getting joined servers");
				return Result<List<Server>>.Fail("Error getting joined servers");
			}

			// Construct a list of server IDs from user-server joins
			var serverIds = request.Models.Select(x => x.ServerId).ToList();

			// Get all servers where server ID is in server IDs
			var serverRequest = await SupabaseService.Client.From<Server>().Filter(x => x.Id, Supabase.Postgrest.Constants.Operator.In, serverIds).Get();

			// Check if servers are null
			if (serverRequest == null || serverRequest.Models == null) {
				Log.Error("Error getting servers from database when getting joined servers");
				return Result<List<Server>>.Fail("Error getting servers from database");
			}

			return Result<List<Server>>.OK(serverRequest.Models);
		}

		/// <summary>
		/// Joins the new server
		/// </summary>
		/// <param name="server">Server to join</param>
		/// <returns>True if sucess, false if not</returns>
		public async Task<Result<string>> JoinServer(Server server) {
			// Get the current user id from Users table
			var userId = await UserService.CheckLoggedInUser();

			// Check if user ID is null
			if (userId.Failed) {
				Log.Error("Error getting user ID when joining server");
				return Result<string>.Fail("Error getting user ID");
			}

			var userServerJoin = new UserServerJoin(userId.Data, server.Id);

			// Insert the user-server join to database
			var response = await SupabaseService.Client.From<UserServerJoin>().Insert(userServerJoin);

			// Check if user-server join is created
			if (response == null) {
				Log.Error($"Error joining server {server.GetDebugInfo()}");
				return Result<string>.Fail("Error joining server");
			}

			// Get the server from database
			var serverRequest = await SupabaseService.Client.From<Server>().Where(s => s.Id == server.Id).Get();
			var joinedServer = serverRequest.Model;

			// Check if server is null
			if (serverRequest == null || joinedServer == null) {
				Log.Error($"Error getting server from database when joining server {server.GetDebugInfo()}");
				return Result<string>.Fail("Error getting server from database");
			}

			// Increment the user count in server
			joinedServer.UserCount += 1;

			// Update the server in database
			await SupabaseService.Client.From<Server>().Update(joinedServer);

			return Result<string>.OK("Server joined successfully");
		}

		/// <summary>
		/// Remove current user from server
		/// </summary>
		/// <param name="server">Server to leave</param>
		/// <returns></returns>
		public async Task<Result<string>> LeaveServer(Server server) {
			// Get current user if from Users table
			var userId = await UserService.CheckLoggedInUser();

			// Check if user ID is null
			if (userId.Failed) {
				Log.Error("Error getting user ID when leaving server");
				return Result<string>.Fail("Error getting user ID");
			}

			// Delete user-server join from database where user ID is current user ID and server ID is passed in server ID
			await SupabaseService.Client.From<UserServerJoin>().Where(usj => usj.UserId == userId.Data && usj.ServerId == server.Id).Delete();

			// Get the server from database where server ID is passed inserver ID
			var serverRequest = await SupabaseService.Client.From<Server>().Where(s => s.Id == server.Id).Get();
			var joinedServer = serverRequest.Model;

			// Check if server is null
			if (serverRequest == null) {
				Log.Error("Error getting server from database when leaving server");
				return Result<string>.Fail("Error getting server from database");
			}

			// Decrement the user count in server
			joinedServer.UserCount -= 1;

			// Update the server in database
			await SupabaseService.Client.From<Server>().Update(joinedServer);

			return Result<string>.OK("Server left successfully");
		}

		/// <summary>
		/// Checks if the current user is the owner of the server
		/// </summary>
		/// <param name="server">Server to check for</param>
		/// <returns>True if logged in user is the owner, false if not</returns>
		public async Task<Result<bool>> IsServerOwner(Server server) {
			// Get current user if from Users table
			var userId = await UserService.CheckLoggedInUser();

			// Check if user ID is null
			if (userId.Failed) {
				Log.Error("Error getting user ID when checking server owner");
				return Result<bool>.Fail("Error getting user ID");
			}

			// Check if server owner is the same as the current user ID
			return Result<bool>.OK(server.CreatedBy == userId.Data);
		}

		/// <summary>
		/// Update server information
		/// </summary>
		/// <param name="server">Server to update</param>
		/// <returns>True if update succesful, false if not</returns>
		public async Task<Result<string>> UpdateServer(Server server) {
			// Update the server in database
			var response = await SupabaseService.Client.From<Server>().Update(server);

			if (response == null) {
				Log.Error($"Error updating server in database {server.GetDebugInfo()}");
				return Result<string>.Fail("Error updating server in database");
			}

			return Result<string>.OK("Server updated successfully");
		}
	}
}
