using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitChatClient.Models
{
    [Table("Users")]
	public class User : BaseModel
	{
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("created_at")]
        public DateTime createdAt { get; set; }

        [Column("username")]
		public string Username { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("uuid")]
        public string Uuid { get; set; }

        public User()
        {
            Id = 0;
            createdAt = DateTime.Now;
            Username = string.Empty;
            Email = string.Empty;
            Uuid = string.Empty;
        }

        public User(string username, string email)
        {
            Id = 0;
            createdAt = DateTime.Now;
            Username = username;
            Email = email;
            Uuid = string.Empty;
        }

        public User(string username, string email, string uuid)
        {
            Id = 0;
            createdAt = DateTime.Now;
            Username = username;
            Email = email;
            Uuid = uuid;
        }
	}
}
