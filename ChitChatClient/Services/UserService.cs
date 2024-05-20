using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChitChatClient.Helpers;
using ChitChatClient.DatabaseModels;
using Serilog;

using Supabase.Gotrue;

namespace ChitChatClient.Services {
	/// <summary>
	/// Service to handle user related operations
	/// </summary>
	public class UserService {
		private static readonly UserService instance = new();

		public static UserService Instance { get => instance; }

		private readonly UserRepository UserRepository = new();

		private readonly UserServerJoinReposiotry UserServerJoinReposiotry = new();

		private readonly AuthService AuthService = AuthService.Instance;

		private UserService() { }

		/// <summary>
		/// Gets all users in a server
		/// </summary>
		/// <param name="server">Server from which to get users</param>
		/// <returns>Returns list of all users</returns>
		public async Task<Result<List<DatabaseModels.User>>> GetServerUsers(Server server) {
			// Get all UserServerJoin objects from the given server
			var usjResult = await UserServerJoinReposiotry.GetUSJByServerID(server);

			if (usjResult.Failed)
				return Result<List<DatabaseModels.User>>.Fail(usjResult.Error);

			// Get all user ids from the user-server joins
			var usersId = usjResult.Data.Select(usj => usj.UserId).ToList();

			// Get all users where id is in the list of user ids
			var usersRequest = await UserRepository.GetUsersByID(usersId);

			// Check if request is not null
			if (usersRequest.Failed)
				return Result<List<DatabaseModels.User>>.Fail(usersRequest.Error);

			return Result<List<DatabaseModels.User>>.OK(usersRequest.Data);
		}

		/// <summary>
		/// Gets id of currently logged in user
		/// </summary>
		/// <returns>Returns id of the currently logged in user</returns>
		public async Task<Result<int>> GetUserId() {
			// Get current user
			var getUserResult = AuthService.GetCurrentUser();

			// Check if user is not null
			if (getUserResult.Failed)
				return Result<int>.Fail(getUserResult.Error);

			// Get user data where uuid is the same as the current user id
			var getUUIDUserResult = await UserRepository.GetUserByUUID(getUserResult.Data.Id);

			// Check if request is not null
			if (getUUIDUserResult.Failed)
				return Result<int>.Fail(getUUIDUserResult.Error);

			return Result<int>.OK(getUUIDUserResult.Data.Id);
		}

		/// <summary>
		/// Sets the online status of the currently logged in user
		/// </summary>
		/// <param name="status">True or false for online status</param>
		/// <returns></returns>
		public async Task<Result<string>> SetUserOnlineStatus(bool status) {
			// Get current user
			var getUserResult = AuthService.GetCurrentUser();

			// Check if user is not null
			if (getUserResult.Failed)
				return Result<string>.Fail(getUserResult.Error);

			// Get user data where uuid is the same as the current user id
			var getUUIDUserResult = await UserRepository.GetUserByUUID(getUserResult.Data.Id);

			// Check if request is not null
			if (getUUIDUserResult.Failed)
				return Result<string>.Fail(getUUIDUserResult.Error);

			// Set online status
			getUUIDUserResult.Data.IsOnline = status;

			// Update user data
			var updateUserResult = await UserRepository.UpdateUser(getUUIDUserResult.Data);

			if (updateUserResult.Failed)
				return Result<string>.Fail(updateUserResult.Error);

			return Result<string>.OK("User online status updated successfully");
		}
	}
}
