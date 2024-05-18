using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace ChitChat.Models {
	/// <summary>
	/// Represents a user in the database
	/// </summary>
	[Table("Users")]
	public class User : BaseModel {
		[PrimaryKey("id", false)]
		public int Id { get; set; }

		[Column("created_at")]
		public DateTime CreatedAt { get; set; }

		[Column("username")]
		public string Username { get; set; }

		[Column("email")]
		public string Email { get; set; }

		[Column("uuid")]
		public string Uuid { get; set; }

		[Column("is_online")]
		public bool IsOnline { get; set; }

		public User() {
			Id = 0;
			CreatedAt = DateTime.Now;
			Username = string.Empty;
			Email = string.Empty;
			Uuid = string.Empty;
		}

		public User(string username, string email, string uuid) {
			Username = username ?? throw new ArgumentNullException(nameof(username));
			Email = email ?? throw new ArgumentNullException(nameof(email));
			Uuid = uuid ?? throw new ArgumentNullException(nameof(uuid));
		}

		public User(string username, string email) {
			Username = username ?? throw new ArgumentNullException(nameof(username));
			Email = email ?? throw new ArgumentNullException(nameof(email));
			Uuid = string.Empty;
		}

		public string GetDebugInfo() => $"Id: {Id}, Username: {Username}, Email: {Email}, Uuid: {Uuid}, IsOnline: {IsOnline}";
	}
}
