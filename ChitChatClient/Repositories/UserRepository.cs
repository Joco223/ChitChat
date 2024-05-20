using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChitChatClient.DatabaseModels;
using ChitChatClient.Helpers;
using ChitChatClient.Services;
using Serilog;
using Supabase.Gotrue;

namespace ChitChatClient;

public class UserRepository
{
	private readonly SupabaseService SupabaseService = SupabaseService.Instance;

	public UserRepository() {}

	/// <summary>
	/// Create database record for user
	/// </summary>
	/// <param name="registerUser">Uset to save</param>
	/// <param name="session">Users session to store uuid</param>
	/// <returns></returns>
	public async Task<Result<string>> CreateUser(DatabaseModels.User registerUser, Session session) {
		// Generate user data
		var user = new DatabaseModels.User(registerUser.Username, registerUser.Email, session.User.Id);

		// Insert user data
		var response = await SupabaseService.Client.From<DatabaseModels.User>().Insert(user);

		// If response is null return fail
		if (response == null) {
			return Result<string>.Fail($"Failed to insert user in database {registerUser.GetDebugInfo()}");
		}

		return Result<string>.OK("User created successfully");
	}

	/// <summary>
	/// Gets user from Users table by UUID
	/// </summary>
	/// <param name="UUID">UUID to search by</param>
	/// <returns></returns>
	public async Task<Result<DatabaseModels.User>> GetUserByUUID(string UUID) {
		var userRequest = await SupabaseService.Client.From<DatabaseModels.User>().Where(u => u.Uuid == UUID).Get();

		if (userRequest == null)
			return Result<DatabaseModels.User>.Fail($"Failed getting user from database {UUID}");


		if (userRequest.Model == null)
			return Result<DatabaseModels.User>.Fail($"Failed to find user by UUID {UUID}");

		return Result<DatabaseModels.User>.OK(userRequest.Model);
	}

	/// <summary>
	/// Get users which have IDs in the provided parameter
	/// </summary>
	/// <param name="IDs">IDs to search by</param>
	/// <returns></returns>
	public async Task<Result<List<DatabaseModels.User>>> GetUsersByID(List<int> IDs) {
		var usersRequest = await SupabaseService.Client.From<DatabaseModels.User>().Filter(u => u.Id, Supabase.Postgrest.Constants.Operator.In, IDs).Get();
		var users = usersRequest.Models;

		if (usersRequest == null || users == null)
			return Result<List<DatabaseModels.User>>.Fail("Failed to get users by ID");

		return Result<List<DatabaseModels.User>>.OK(users);
	}

	/// <summary>
	/// Updates user record in database
	/// </summary>
	/// <param name="user">Record to update</param>
	/// <returns></returns>
	public async Task<Result<string>> UpdateUser(DatabaseModels.User user) {
		var userRequest = await SupabaseService.Client.From<DatabaseModels.User>().Update(user);

		if (userRequest == null)
			return Result<string>.Fail($"Failed to update user {user.GetDebugInfo()}");

		return Result<string>.OK("User succesfully updated");
	}
}
