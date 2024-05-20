using System;
using System.Collections.Generic;
using ChitChatClient.DatabaseModels;

namespace ChitChatClient.Models;

public class ServerUser
{
	public User User { get; set; }

	public string StatusColor => User.IsOnline ? "#00FF00" : "#FF0000";

	public string Username => User.Username;

	public ServerUser(User user)
	{
		User = user ?? throw new ArgumentNullException(nameof(user));
	}

	public static List<ServerUser> ConvertToServerUserList(List<User> users)
	{
		List<ServerUser> serverUsers = new();
		foreach (User user in users)
		{
			serverUsers.Add(new ServerUser(user));
		}
		return serverUsers;
	}

	public string GetDebugInfo() => $"User: {User.GetDebugInfo()}";
}
