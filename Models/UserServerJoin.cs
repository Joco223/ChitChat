﻿using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitChatClient.Models
{
    [Table("UserServerJoin")]
	public class UserServerJoin : BaseModel
	{
        [PrimaryKey("id", false)]
		public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("server_id")]
        public int ServerId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        public UserServerJoin()
        {
            Id = -1;
            UserId = -1;
            ServerId = -1;
            CreatedAt = DateTime.Now;
        }

        public UserServerJoin(int userId, int serverId)
        {
            UserId = userId;
            ServerId = serverId;
            CreatedAt = DateTime.Now;
        }

        public UserServerJoin(int id, int userId, int serverId)
        {
            Id = id;
            UserId = userId;
            ServerId = serverId;
            CreatedAt = DateTime.Now;
        }
    }
}
