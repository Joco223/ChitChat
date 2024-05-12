using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitChatClient.ViewModels
{
	public class CreateServer
	{
		public string Name { get; set; }

		public string Description { get; set; }

		public CreateServer()
		{
			Name = string.Empty;
			Description = string.Empty;
        }
	}
}
