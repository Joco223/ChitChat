using System;
using System.Threading.Tasks;
using ChitChatClient.DatabaseModels;
using ChitChatClient.Helpers;
using ChitChatClient.Services;
using Supabase.Gotrue;

namespace ChitChatClient;

public class AuthService
{
	private static readonly AuthService instance = new();

	public static AuthService Instance { get => instance; }

	private readonly SupabaseService SupabaseService = SupabaseService.Instance;

	private readonly UserRepository UserRepository = new();

	public AuthService() {}

	/// <summary>
	/// Register a new user and save record of user to database
	/// </summary>
	/// <param name="registerUser">User to register</param>
	/// <param name="password">User password to register with</param>
	/// <returns></returns>
	public async Task<Result<string>> RegisterUser(DatabaseModels.User registerUser, string password) {
		// Register user
		Session? session = await SupabaseService.Client.Auth.SignUp(registerUser.Email, password);

		// If any of the data is null or empty return fail
		if (session == null || session.User == null || session.User.Id == null) {
			return Result<string>.Fail($"Failed to register user {registerUser.GetDebugInfo()}");
		}

		// Insert record of user into database
		var userInsertResult = await UserRepository.CreateUser(registerUser, session);

		// Check if it was successfull
		if (userInsertResult.Failed)
			return Result<string>.Fail(userInsertResult.Error);

		return Result<string>.OK("User successfully registered");
	}

	/// <summary>
	/// Logs in user with given email and password
	/// </summary>
	/// <param name="email"></param>
	/// <param name="password"></param>
	/// <returns></returns>
	public async Task<Result<string>> LogInUser(string email, string password) {
		// Login user
		try {
			Session? session = await SupabaseService.Client.Auth.SignIn(email, password);

			// Check if session is null
			if (session == null)
				return Result<string>.Fail("Failed to login user");

			return Result<string>.OK("User logged in successfully");
		} catch (Exception ex) {
			return Result<string>.Fail($"Failed to login user {ex.Message}");
		}
	}

	/// <summary>
	/// Checks if user is logged in
	/// </summary>
	/// <returns>Returns user id if user is logged in</returns>
	public bool CheckLoggedInUser() {
		var user = SupabaseService.Client.Auth.CurrentUser;

		if (user == null || user.Id == null)
			return false;

		return true;
	}

	/// <summary>
	/// Gets the current user from supabase auth
	/// </summary>
	/// <returns></returns>
	public Result<Supabase.Gotrue.User> GetCurrentUser() {
		var loggedIn = CheckLoggedInUser();

		if (!loggedIn)
			return Result<Supabase.Gotrue.User>.Fail("User not logged in");

		return Result<Supabase.Gotrue.User>.OK(SupabaseService.Client.Auth.CurrentUser);
	}
}
