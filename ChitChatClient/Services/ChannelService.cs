using ChitChatClient.Helpers;
using ChitChatClient.Models;

using Serilog;

namespace ChitChatClient.Services {
	/// <summary>
	/// Service for handling channel related database operations
	/// </summary>
	public class ChannelService {
		private static readonly ChannelService instance = new();

		public static ChannelService Instance { get => instance; }

		private readonly SupabaseHandler supabaseHandler = SupabaseHandler.Instance;

		private ChannelService() { }

		/// <summary>
		/// Gets all channels for the given server
		/// </summary>
		/// <param name="server">Server to get channels from</param>
		/// <returns>Returns list of channels</returns>
		/// <exception cref="Exception"></exception>
		public async Task<List<Channel>> GetChannels(Server server) {
			try {
				// Get all channels for the given server
				var response = await supabaseHandler.Client.From<Channel>().Where(c => c.ServerId == server.Id).Get();
				var channels = response.Models;

				// If response is not null and there are channels in the response, return the channels
				if (response != null && response.Models != null) {
					return channels;
				} else {
					Log.Error("Error getting channels");
					throw new Exception("Error getting channels.");
				}

			} catch (Exception ex) {
				Log.Error($"Error getting channels {ex.Message}");
				throw new Exception("Error getting channels. " + ex.Message);
			}
		}

		/// <summary>
		/// Creates database record with the given channel
		/// </summary>
		/// <param name="channel">Channel to create the record for</param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public async Task CreateChannel(Channel channel) {
			try {
				// Insert the channel into the database
				var response = await supabaseHandler.Client.From<Channel>().Insert(channel);
				var newChannel = response.Model;

				// If response response or newChannel is null, throw error
				if (response == null || newChannel == null) {
					Log.Error($"Error creating channel {channel}");
					throw new Exception("Error creating channel.");
				}
			} catch (Exception ex) {
				Log.Error($"Error creating channel {channel} - {ex.Message}");
				throw new Exception("Error creating channel. " + ex.Message);
			}
		}

		/// <summary>
		/// Deletes database record for the given channe;
		/// </summary>
		/// <param name="channel">Channel to delete the database record for</param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public async Task DeleteChannel(Channel channel) {
			try {
				// Delete the channel from the database
				await supabaseHandler.Client.From<Channel>().Where(c => c.Id == channel.Id).Delete();
			} catch (Exception ex) {
				Log.Error($"Error deleting channel {channel} - {ex.Message}");
				throw new Exception("Error deleting channel. " + ex.Message);
			}
		}

		/// <summary>
		/// Updates database record for the given channel
		/// </summary>
		/// <param name="channel">Channel to update the record for</param>
		/// <returns>True if update is succesfull, false if not</returns>
		/// <exception cref="Exception"></exception>
		public async Task UpdateChannel(Channel channel) {
			try {
				// Update the channel in the database
				var response = await supabaseHandler.Client.From<Channel>().Update(channel);
				var updatedChannel = response.Model;

				// If response or updatedChannel is null, throw error
				if (response == null || updatedChannel == null) {
					Log.Error($"Error updating channel {channel}");
					throw new Exception("Error updating channel.");
				}
			} catch (Exception ex) {
				Log.Error($"Error updating channel {channel} - {ex.Message}");
				throw new Exception("Error updating channel. " + ex.Message);
			}
		}
	}
}
