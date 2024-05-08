using Supabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitChat.Helpers
{
	public class SupabaseHandler
	{
		private string? supabaseUrl = "";
		private string? supabaseKey = "";

		private Client localClient;

        public Client Client { get => localClient; set => localClient = value; }

        private SupabaseHandler()
		{
			supabaseUrl = Properties.Settings.Default["supabaseUrl"].ToString();
			supabaseKey = Properties.Settings.Default["supabaseKey"].ToString();

			if (string.IsNullOrEmpty(supabaseUrl) || string.IsNullOrEmpty(supabaseKey))
			{
                throw new Exception("Supabase URL and Key must be set in the application settings");
            }

			// Initialize Supabase client
			localClient = new Client(supabaseUrl, supabaseKey);
            localClient.InitializeAsync().Wait();
		}

        private static readonly SupabaseHandler handler = new();

		public static SupabaseHandler GetHandler()
        {
            return handler;
        }
    }
}
