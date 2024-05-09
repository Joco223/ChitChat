using ChitChat.Helpers;
using ChitChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitChat.Services
{
	public class AppInfoService
	{
		private SupabaseHandler supabaseHandler = SupabaseHandler.Instance;

		private static AppInfoService instance = new();

		public static AppInfoService Instance { get => instance; }

		public AppInfoService()
		{
        }

		public async Task<string> GetAppInfo(string key)
		{
			try
			{
				var response = await supabaseHandler.Client.From<AppInfo>().Where(ap => ap.Key == key).Get();

                if (response != null && response.Model != null)
				{
                    return response.Model.Value;
                }
            }
            catch (Exception ex)
			{
                Console.WriteLine(ex.Message);
            }

            return string.Empty;
		}
	}
}
