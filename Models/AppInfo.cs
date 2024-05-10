using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitChatClient.Models
{
	[Table("AppInfo")]
	public class AppInfo : BaseModel
	{
		[PrimaryKey("id", false)]
        public int Id { get; set; }

		[Column("created_at")]
        public DateTime CreatedAt { get; set; }

		[Column("key")]
        public string Key { get; set; }

		[Column("value")]
        public string Value { get; set; }

        public AppInfo()
		{
            Id = -1;
            Key = string.Empty;
            Value = string.Empty;
            CreatedAt = DateTime.Now;
        }

        public AppInfo(string appName, string appVersion)
        {
            Key = appName;
            Value = appVersion;
            CreatedAt = DateTime.Now;
        }

        public AppInfo(int id, string appName, string appVersion, DateTime createdAt)
        {
            Id = id;
            Key = appName;
            Value = appVersion;
            CreatedAt = createdAt;
        }

        public override string ToString() => Key;
	}
}
