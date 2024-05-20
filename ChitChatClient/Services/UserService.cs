using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		private readonly SupabaseService SupabaseService = SupabaseService.Instance;

		public static UserService Instance { get => instance; }

		private UserService() { }


		/// <summary>
		/// Registers a new user
		/// </summary>
		/// <param name="registerUser">RegisterUser object with all necessary data</param>
		/// <param name="password">Password to reguster the user with</param>
		/// <returns>Returns true if registration is sucesfull, false if not</returns>
		public async Task<Result<string>> RegisterUser(DatabaseModels.User registerUser, string password) {
			// Register user
			Session? session = await SupabaseService.Client.Auth.SignUp(registerUser.Email, password);

			// If any of the data is null or empty return fail
			if (session == null || session.User == null || session.User.Id == null) {
				Log.Error("Failed to register user");
				return Result<string>.Fail("Failed to register user");
			}

			// Generate user data
			var user = new DatabaseModels.User(registerUser.Username, registerUser.Email, session.User.Id);

			// Insert user data
			var response = await SupabaseService.Client.From<DatabaseModels.User>().Insert(user);

			// If response is null return fail
			if (response == null) {
				Log.Error("Failed to insert user in database");
				return Result<string>.Fail("Failed to insert user in database");
			}

			return Result<string>.OK("User registered successfully");
		}

		public async Task<Result<string>> LogInUser(string email, string password) {
			// Login user
			try {
				Session? session = await SupabaseService.Client.Auth.SignIn(email, password);

				// Check if session is null
				if (session == null) {
					Log.Error("Failed to login user");
					return Result<string>.Fail("Failed to login user");
				}

				return Result<string>.OK("User logged in successfully");
			} catch (Exception e) {
				Log.Error(e, "Failed to login user");
				return Result<string>.Fail("Failed to login user");
			}
		}

		/// <summary>
		/// Gets all users in a server
		/// </summary>
		/// <param name="server">Server from which to get users</param>
		/// <returns>Returns list of all users</returns>
		public async Task<Result<List<DatabaseModels.User>>> GetServerUsers(Server server) {
			// Get all usesr-server joins where server id is the same as the passed in server id
			var usjRequest = await SupabaseService.Client.From<UserServerJoin>().Where(usj => usj.ServerId == server.Id).Get();

			// Cheeck if request is not null
			if (usjRequest == null) {
				Log.Error($"Failed to get user-server joins on server {server.GetDebugInfo()}");
				return Result<List<DatabaseModels.User>>.Fail("Failed to get user-server joins on server");
			}

			var usjList = usjRequest.Models;

			// Get all user ids from the user-server joins
			var usersId = usjList.Select(usj => usj.UserId).ToList();

			// Get all users where id is in the list of user ids
			var userRequest = await SupabaseService.Client.From<DatabaseModels.User>().Filter(u => u.Id, Supabase.Postgrest.Constants.Operator.In, usersId).Get();

			// Check if request is not null
			if (userRequest == null || userRequest.Model == null) {
				Log.Error($"Failed to get users on server {server.GetDebugInfo()}");
				return Result<List<DatabaseModels.User>>.Fail("Failed to get users on server");
			}

			return Result<List<DatabaseModels.User>>.OK(userRequest.Models);
		}

		/// <summary>
		/// Checks if user is logged in
		/// </summary>
		/// <returns>Returns user id if user is logged in</returns>
		public async Task<Result<int>> CheckLoggedInUser() {
			var user = SupabaseService.Client.Auth.CurrentUser;

			if (user == null || user.Id == null) {
				Log.Error("Failed to get user id");
				return Result<int>.Fail("Failed to get user id");
			}

			var userID = await GetUserId();

			if (userID.Failed) {
				Log.Error("Failed to get user id");
				return Result<int>.Fail("Failed to get user id");
			}

			return Result<int>.OK(userID.Data);
		}

		/// <summary>
		/// Gets id of currently logged in user
		/// </summary>
		/// <returns>Returns id of the currently logged in user</returns>
		public async Task<Result<int>> GetUserId() {
			// Get current user
			var user = SupabaseService.Client.Auth.CurrentUser;

			// Check if user is not null
			if (user == null || user.Id == null) {
				Log.Error("Failed to get user id");
				return Result<int>.Fail("Failed to get user id");
			}

			// Get user data where uuid is the same as the current user id
			var userRequest = await SupabaseService.Client.From<DatabaseModels.User>().Where(u => u.Uuid == user.Id).Get();
			var userResponse = userRequest.Model;

			// Check if request is not null
			if (userRequest == null || userResponse == null) {
				Log.Error("Failed to get user data");
				return Result<int>.Fail("Failed to get user data");
			}

			return Result<int>.OK(userResponse.Id);
		}

		/// <summary>
		/// Gets the uuid of the currently logged in user
		/// </summary>
		/// <returns></returns>
		public Result<string> GetUserUUID() {
			// Get current user
			var user = SupabaseService.Client.Auth.CurrentUser;

			// Check if user is not null
			if (user == null || user.Id == null) {
				Log.Error("Failed to get user id");
				return Result<string>.Fail("Failed to get user id");
			}

			return Result<string>.OK(user.Id);
		}

		/// <summary>
		/// Sets the online status of the currently logged in user
		/// </summary>
		/// <param name="status">True or false for online status</param>
		/// <returns></returns>
		public async Task<Result<string>> SetUserOnline(bool status) {
			var user = SupabaseService.Client.Auth.CurrentUser;

			// Check if user is not null
			if (user == null || user.Id == null) {
				Log.Error("Failed to set user online status");
				return Result<string>.Fail("Failed to set user online status");
			}

			// Get user data where uuid is the same as the current user id
			var userRequest = await SupabaseService.Client.From<DatabaseModels.User>().Where(u => u.Uuid == user.Id).Get();
			var userResponse = userRequest.Model;

			// Check if request is not null
			if (userRequest == null || userResponse == null) {
				Log.Error("Failed to get user data");
				return Result<string>.Fail("Failed to get user data");
			}

			// Set online status
			userResponse.IsOnline = status;

			// Update user data
			var response = await SupabaseService.Client.From<DatabaseModels.User>().Update(userResponse);

			if (response == null) {
				Log.Error($"Failed to update user {userResponse.GetDebugInfo()}");
				return Result<string>.Fail("Failed to update user");
			}

			return Result<string>.OK("User online status updated successfully");
		}

		/// <summary>
		/// Gets the currently logged in user
		/// </summary>
		/// <returns></returns>
		public Result<Supabase.Gotrue.User> GetUser() {
			var user = SupabaseService.Client.Auth.CurrentUser;

			if (user == null)
				return Result<Supabase.Gotrue.User>.Fail("Failed to get user");

			return Result<Supabase.Gotrue.User>.OK(user);
		}
	}
}
