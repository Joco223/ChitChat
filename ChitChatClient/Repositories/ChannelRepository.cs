using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChitChatClient.DatabaseModels;
using ChitChatClient.Helpers;
using ChitChatClient.Services;
using Serilog;

namespace ChitChatClient.Repositories;

public class ChannelRepository
{
	private readonly SupabaseService SupabaseService = SupabaseService.Instance;

	public ChannelRepository() { }

	/// <summary>
	/// Gets all channels for the given server
	/// </summary>
	/// <param name="server">Server to get channels from</param>
	/// <returns></returns>
	public async Task<Result<List<Channel>>> GetChannelsAsync(Server server)
	{
		// Get all channels for the given server
		var response = await SupabaseService.Client.From<Channel>().Where(c => c.ServerId == server.Id).Get();
		var channels = response.Models;

		// Check if response is not null and there are channels in the response
		if (response == null || channels == null) {
			return Result<List<Channel>>.Fail($"Error getting channels {server.GetDebugInfo()}");
		}

		// If response is not null and there are channels in the response, return the channels
		return Result<List<Channel>>.OK(channels);
	}

	/// <summary>
	/// Creates database record with the given channel
	/// </summary>
	/// <param name="channel">Channel to create the record for</param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public async Task<Result<string>> CreateChannel(Channel channel) {
		// Insert the channel into the database
		var response = await SupabaseService.Client.From<Channel>().Insert(channel);
		var newChannel = response.Model;

		// If response response or newChannel is null, throw error
		if (response == null || newChannel == null) {
			return Result<string>.Fail($"Error creating channel {channel.GetDebugInfo()}");
		}

		// If response and newChannel are not null, return success
		return Result<string>.OK("Channel created successfully");
	}

	/// <summary>
	/// Deletes database record for the given channe;
	/// </summary>
	/// <param name="channel">Channel to delete the database record for</param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public async Task<Result<string>> DeleteChannel(Channel channel) {
		try {
			// Delete the channel from the database
			await SupabaseService.Client.From<Channel>().Where(c => c.Id == channel.Id).Delete();

			// Return success
			return Result<string>.OK("Channel deleted successfully");
		} catch (Exception ex) {
			return Result<string>.Fail($"Error deleting channel {channel.GetDebugInfo()} - {ex.Message}");
		}
	}

	/// <summary>
	/// Updates database record for the given channel
	/// </summary>
	/// <param name="channel">Channel to update the record for</param>
	/// <returns>True if update is succesfull, false if not</returns>
	/// <exception cref="Exception"></exception>
	public async Task<Result<string>> UpdateChannel(Channel channel) {
		// Update the channel in the database
		var response = await SupabaseService.Client.From<Channel>().Update(channel);
		var updatedChannel = response.Model;

		// If response or updatedChannel is null, throw error
		if (response == null || updatedChannel == null) {
			return Result<string>.Fail($"Error updating channel {channel.GetDebugInfo()}");
		}

		// If response and updatedChannel are not null, return success
		return Result<string>.OK("Channel updated successfully");
	}
}
