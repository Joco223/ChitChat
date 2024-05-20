using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChitChatClient.DatabaseModels;
using ChitChatClient.Helpers;
using ChitChatClient.Services;

namespace ChitChatClient;

public class UserServerJoinReposiotry
{
	private readonly SupabaseService SupabaseService = SupabaseService.Instance;

	public UserServerJoinReposiotry() {}

	/// <summary>
	/// Get all UserServerJoin records that match server ID
	/// </summary>
	/// <param name="server">Server to match to</param>
	/// <returns></returns>
	public async Task<Result<List<UserServerJoin>>> GetUSJByServerID(Server server) {
		// Get all usesr-server joins where server id is the same as the passed in server id
		var usjRequest = await SupabaseService.Client.From<UserServerJoin>().Where(usj => usj.ServerId == server.Id).Get();
		var usjList = usjRequest.Models;

		// Cheeck if request is not null
		if (usjRequest == null || usjList == null) {
			return Result<List<UserServerJoin>>.Fail("Failed to get user-server joins on server");
		}

		return Result<List<UserServerJoin>>.OK(usjList);
	}
}
