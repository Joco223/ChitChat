using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChitChatClient.Helpers;
using ChitChatClient.DatabaseModels;

using Serilog;
using ChitChatClient.Repositories;

namespace ChitChatClient.Services {
	/// <summary>
	/// Service for handling channel related database operations
	/// </summary>
	public class ChannelService {
		private static readonly ChannelService instance = new();

		public static ChannelService Instance { get => instance; }

		private readonly ChannelRepository channelRepository = new();


		private ChannelService() { }

		/// <summary>
		/// Gets all channels for the given server
		/// </summary>
		/// <param name="server">Server to get channels from</param>
		/// <returns>Returns list of channels</returns>
		/// <exception cref="Exception"></exception>
		public async Task<Result<List<Channel>>> GetChannels(Server server) {
			var result = await channelRepository.GetChannelsAsync(server);

			if (result.Failed) {
				return Result<List<Channel>>.Fail(result.Error);
			} else {
				return Result<List<Channel>>.OK(result.Data);
			}
		}

		/// <summary>
		/// Creates database record with the given channel
		/// </summary>
		/// <param name="channel">Channel to create the record for</param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public async Task<Result<string>> CreateChannel(Channel channel) {
			var result = await channelRepository.CreateChannel(channel);

			if (result.Failed) {
				return Result<string>.Fail(result.Error);
			} else {
				return Result<string>.OK(result.Data);
			}
		}

		/// <summary>
		/// Deletes database record for the given channe;
		/// </summary>
		/// <param name="channel">Channel to delete the database record for</param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public async Task<Result<string>> DeleteChannel(Channel channel) {
			var result = await channelRepository.DeleteChannel(channel);

			if (result.Failed) {
				return Result<string>.Fail(result.Error);
			} else {
				return Result<string>.OK(result.Data);
			}
		}

		/// <summary>
		/// Updates database record for the given channel
		/// </summary>
		/// <param name="channel">Channel to update the record for</param>
		/// <returns>True if update is succesfull, false if not</returns>
		/// <exception cref="Exception"></exception>
		public async Task<Result<string>> UpdateChannel(Channel channel) {
			var result = await channelRepository.UpdateChannel(channel);

			if (result.Failed) {
				return Result<string>.Fail(result.Error);
			} else {
				return Result<string>.OK(result.Data);
			}
		}
	}
}
