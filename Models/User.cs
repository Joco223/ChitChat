using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitChat.Models
{
	public class User
	{
		public string? Username { get; set; }

        public string? Email { get; set; }

        public User()
        {
            Username = "";
            Email = "";
        }

        public User(string username, string email)
        {
            Username = username;
            Email = email;
        }
	}
}
