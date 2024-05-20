using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChitChatClient.Helpers;
using ChitChatClient.DatabaseModels;

using Serilog;

namespace ChitChatClient.Services {
	/// <summary>
	/// Service for handling channel related database operations
	/// </summary>
	public class ChannelService {
		private static readonly ChannelService instance = new();

		public static ChannelService Instance { get => instance; }

		private readonly SupabaseService SupabaseService = SupabaseService.Instance;

		private ChannelService() { }

		/// <summary>
		/// Gets all channels for the given server
		/// </summary>
		/// <param name="server">Server to get channels from</param>
		/// <returns>Returns list of channels</returns>
		/// <exception cref="Exception"></exception>
		public async Task<Result<List<Channel>>> GetChannels(Server server) {
			// Get all channels for the given server
			var response = await SupabaseService.Client.From<Channel>().Where(c => c.ServerId == server.Id).Get();
			var channels = response.Models;

			// Check if response is not null and there are channels in the response
			if (response == null || channels == null) {
				Log.Error("Error getting channels");
				return Result<List<Channel>>.Fail("Error getting channels.");
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
				Log.Error($"Error creating channel {channel}");
				return Result<string>.Fail("Error creating channel.");
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
				Log.Error($"Error deleting channel {channel} - {ex.Message}");
				return Result<string>.Fail("Error deleting channel. " + ex.Message);
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
				Log.Error($"Error updating channel {channel}");
				return Result<string>.Fail("Error updating channel.");
			}

			// If response and updatedChannel are not null, return success
			return Result<string>.OK("Channel updated successfully");
		}
	}
}
