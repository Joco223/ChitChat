using Supabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitChatClient.Helpers
{
	public class SupabaseHandler
	{
		private readonly string supabaseUrl = "https://pblidipsxjrfmmousmwh.supabase.co";
		private readonly string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InBibGlkaXBzeGpyZm1tb3VzbXdoIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MTUyMDUxNTcsImV4cCI6MjAzMDc4MTE1N30.PkJ2W98C0Avn6sCdoPXhnVwUnOTLWyfCXXqaoYvliIk";

		private Client localClient;

        public Client Client { get => localClient; set => localClient = value; }
        
		private static readonly SupabaseHandler handler = new();

		public static SupabaseHandler Instance { get => handler; }

        private SupabaseHandler()
		{
			// Initialize Supabase client
			localClient = new Client(supabaseUrl, supabaseKey);
            localClient.InitializeAsync().Wait();
		}

    }
}
