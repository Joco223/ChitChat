using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace ChitChat.DatabaseModels {
	/// <summary>
	/// Represents a server in the application
	/// </summary>
	[Table("Servers")]
	public class Server : BaseModel {
		[PrimaryKey("id", false)]
		public int Id { get; set; }

		[Column("name")]
		public string Name { get; set; }

		[Column("description")]
		public string Description { get; set; }

		[Column("created_by")]
		public int CreatedBy { get; set; }

		[Column("created_at")]
		public DateTime CreatedAt { get; set; }

		[Column("user_count")]
		public int UserCount { get; set; }

		[Column("online_user_count")]
		public int OnlineUserCount { get; set; }

		public Server() {
			Id = -1;
			Name = string.Empty;
			Description = string.Empty;
			CreatedBy = 0;
			CreatedAt = DateTime.Now;
			UserCount = 0;
		}

		public Server(string name, string description) {
			Name = name ?? throw new ArgumentNullException(nameof(name));
			Description = description ?? throw new ArgumentNullException(nameof(description));
		}

		public override string ToString() => Name;

		public string GetDebugInfo() => $"ID: {Id}, Name: {Name}, Description: {Description}, Created By: {CreatedBy}, Created At: {CreatedAt}";
	}
}
