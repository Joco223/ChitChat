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

		[Column("created_by")]
		public int CreatedBy { get; set; }

		[Column("created_at")]
		public DateTime CreatedAt { get; set; }

		public Server()
		{
			Id = -1;
			Name = string.Empty;
			CreatedBy = 0;
			CreatedAt = DateTime.Now;
		}

		public Server(string name, int createdBy)
		{
			Name = name;
			CreatedBy = createdBy;
			CreatedAt = DateTime.Now;
		}

		public Server(int id, string name, int createdBy, DateTime createdAt)
		{
            Id = id;
            Name = name;
            CreatedBy = createdBy;
            CreatedAt = createdAt;
        }

		public override string ToString() => Name;
	}
}
