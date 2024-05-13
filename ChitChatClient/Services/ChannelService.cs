using ChitChatClient.Helpers;
using ChitChatClient.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitChatClient.Services
{
	public class ChannelService
	{
		private static readonly ChannelService instance = new();

		public static ChannelService Instance { get => instance; }

		private readonly SupabaseHandler supabaseHandler = SupabaseHandler.Instance;

		private ChannelService()
		{
		}

		public async Task<List<Channel>> GetChannels(Server server)
		{
			try
			{
				var response = await supabaseHandler.Client.From<Channel>().Where(c => c.ServerId == server.Id).Get();
				var channels = response.Models;

				if (response != null && response.Models != null)
				{
					return channels;
				}
				else
				{
					throw new Exception("Error getting channels.");
				}

			}
			catch (Exception ex)
			{
				throw new Exception("Error getting channels. " + ex.Message);
			}
		}

		public async Task<bool> CreateChannel(Channel channel)
		{
			try
			{
				var response = await supabaseHandler.Client.From<Channel>().Insert(channel);
				var newChannel = response.Model;

				if (response != null && newChannel != null)
				{
					return true;
				}
				else
				{
					throw new Exception("Error creating channel.");
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error creating channel. " + ex.Message);
			}
		}

		public async Task DeleteChannel(Channel channel)
		{
			try
			{
				await supabaseHandler.Client.From<Channel>().Where(c => c.Id == channel.Id).Delete();
			}
			catch (Exception ex)
			{
				throw new Exception("Error deleting channel. " + ex.Message);
			}
		}

		public async Task<bool> UpdateChannel(Channel channel)
		{
			try
			{
				var response = await supabaseHandler.Client.From<Channel>().Update(channel);
				var updatedChannel = response.Model;

				if (response != null && updatedChannel != null)
				{
					return true;
				}
				else
				{
					throw new Exception("Error updating channel.");
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error updating channel. " + ex.Message);
			}
		}
	}
}
