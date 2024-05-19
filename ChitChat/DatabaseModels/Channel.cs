using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace ChitChat.DatabaseModels {
	/// <summary>
	/// Represents a channel in the application
	/// </summary>
	[Table("Channels")]
	public class Channel : BaseModel {

		[PrimaryKey("id", false)]
		public int Id { get; set; }

		[Column("created_at")]
		public DateTime CreatedAt { get; set; }

		[Column("name")]
		public string Name { get; set; }

		[Column("description")]
		public string Description { get; set; }

		[Column("server_id")]
		public int ServerId { get; set; }

		public Channel() {
			Id = -1;
			CreatedAt = DateTime.Now;
			Name = string.Empty;
			Description = string.Empty;
			ServerId = -1;
		}

		public Channel(string name, string description, int serverId) {
			Name = name ?? throw new ArgumentNullException(nameof(name));
			Description = description ?? throw new ArgumentNullException(nameof(description));
			ServerId = serverId;
		}

		public string GetDebugInfo() => $"ID: {Id}, Name: {Name}, Description: {Description}, Server ID: {ServerId}";
	}
}
