using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitChatClient.Models
{
	[Table("Servers")]
	public class Server : BaseModel
	{
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

		public Server()
		{
			Id = -1;
			Name = string.Empty;
			Description = string.Empty;
			CreatedBy = 0;
			CreatedAt = DateTime.Now;
			UserCount = 0;
		}

		public Server(string name, int createdBy, string description)
		{
			Name = name;
			Description = description;
			CreatedBy = createdBy;
			CreatedAt = DateTime.Now;
			UserCount = 0;
		}

		public Server(int id, string name, int createdBy, DateTime createdAt, string description)
		{
            Id = id;
            Name = name;
			Description = description;
            CreatedBy = createdBy;
            CreatedAt = createdAt;
        }

		public override string ToString() => Name;
	}
}
