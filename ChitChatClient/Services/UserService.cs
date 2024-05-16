using System.Diagnostics;

using ChitChatClient.Helpers;
using ChitChatClient.Models;
using ChitChatClient.ViewModels;

using Serilog;

using Supabase.Gotrue;

namespace ChitChatClient.Services {
	/// <summary>
	/// Service to handle user related operations
	/// </summary>
	public class UserService {
		private static readonly UserService instance = new();
		private readonly SupabaseHandler supabaseHandler = SupabaseHandler.Instance;

		public static UserService Instance { get => instance; }

		private UserService() { }


		/// <summary>
		/// Registers a new user
		/// </summary>
		/// <param name="registerUser">RegisterUser object with all necessary data</param>
		/// <returns>Returns true if registration is sucesfull, false if not</returns>
		public async Task RegisterUser(RegisterUser registerUser) {
			try {
				// Register user
				Session? session = await supabaseHandler.Client.Auth.SignUp(registerUser.Email, registerUser.Password);

				// If user is registered
				if (session != null && session.User != null && session.User.Id != null) {
					// Generate user data
					var user = new Models.User(registerUser.Username, registerUser.Email, session.User.Id);

					// Insert user data
					var response = await supabaseHandler.Client.From<Models.User>().Insert(user);

					if (response == null) {
						Log.Error("Failed to insert user in database");
						throw new Exception("Failed to insert user in database");
					}
				} else {
					Log.Error("Failed to register user");
					throw new Exception("Failed to register user");
				}
			} catch (Exception ex) {
				Log.Error($"Failed to register user {registerUser.GetDebugInfo()} - {ex.Message}");
				throw;
			}
		}

		/// <summary>
		/// Gets all users in a server
		/// </summary>
		/// <param name="server">Server from which to get users</param>
		/// <returns>Returns list of all users</returns>
		public async Task<List<Models.User>> GetServerUsers(Server server) {
			try {
				// Get all usesr-server joins where server id is the same as the passed in server id
				var usjRequest = await supabaseHandler.Client.From<UserServerJoin>().Where(usj => usj.ServerId == server.Id).Get();
				var usjList = usjRequest.Models;

				if (usjRequest != null) {
					// Gets all user ids from the user-server joins list
					var usersId = usjList.Select(usj => usj.UserId).ToList();

					var userRequest = await supabaseHandler.Client.From<Models.User>().Filter(u => u.Id, Supabase.Postgrest.Constants.Operator.In, usersId).Get();

					if (userRequest != null && userRequest.Models != null) {
						return userRequest.Models;
					} else {
						Log.Error($"Failed to get users on server {server.GetDebugInfo()}");
						throw new Exception("Failed to get users on server");
					}
				} else {
					Log.Error($"Failed to get user-server joins on server {server.GetDebugInfo()}");
					throw new Exception("Failed to get user-server joins on server");
				}

			} catch (Exception ex) {
				Log.Error($"Failed to get users on server {server.GetDebugInfo()} - {ex.Message}");
				throw;
			}
		}

		/// <summary>
		/// Gets id of currently logged in user
		/// </summary>
		/// <returns>Returns id of the currently logged in user</returns>
		public async Task<int> GetUserId() {
			try {
				// Get current user
				var user = supabaseHandler.Client.Auth.CurrentUser;

				if (user != null && user.Id != null) {
					// Get user data where uuid is the same as the current user id
					var userRequest = await supabaseHandler.Client.From<Models.User>().Where(u => u.Uuid == user.Id).Get();
					var userResponse = userRequest.Model;

					if (userResponse != null) {
						return userResponse.Id;
					} else {
						Log.Error("Failed to get user data");
						throw new Exception("Failed to get user data");
					}
				} else {
					Log.Error("Failed to get user id");
					throw new Exception("Failed to get user id");
				}
			} catch (Exception ex) {
				Log.Error($"Failed to get user id - {ex.Message}");
				throw;
			}
		}

		/// <summary>
		/// Sets the online status of the currently logged in user
		/// </summary>
		/// <param name="status">True or false for online status</param>
		/// <returns></returns>
		public async Task SetUserOnline(bool status) {
			try {
				var user = supabaseHandler.Client.Auth.CurrentUser;

				if (user != null && user.Id != null) {

					// Get user data where uuid is the same as the current user id
					var userRequest = await supabaseHandler.Client.From<Models.User>().Where(u => u.Uuid == user.Id).Get();
					var userResponse = userRequest.Model;

					if (userResponse != null) {
						// Set online status
						userResponse.IsOnline = status;

						// Update user data
						var response = await supabaseHandler.Client.From<Models.User>().Update(userResponse);

						if (response == null) {
							Log.Error($"Failed to update user {userResponse.GetDebugInfo()}");
							throw new Exception("Failed to update user");
						}
					} else {
						Log.Error("Failed to get user data");
						throw new Exception("Failed to get user data");
					}
				} else {
					Log.Error("Failed to set user online status");
					throw new Exception("Failed to set user online status");
				}
			} catch (Exception ex) {
				Debug.WriteLine(ex.Message);
				throw;
			}
		}

		/// <summary>
		/// Gets the currently logged in user
		/// </summary>
		/// <returns></returns>
		public Result<Supabase.Gotrue.User> GetUser() {
			var user = supabaseHandler.Client.Auth.CurrentUser;

			if (user == null)
				return Result<Supabase.Gotrue.User>.Fail("Failed to get user");

			return Result<Supabase.Gotrue.User>.OK(user);
		}
	}
}
