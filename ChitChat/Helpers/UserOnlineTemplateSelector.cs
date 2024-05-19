using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;
using ChitChat.DatabaseModels;

namespace ChitChat.Helpers {

	public class UserOnlineTemplateSelector : IDataTemplate
	{
		public bool SupportsRecycling => false;

		[Content]
		public Dictionary<bool, IDataTemplate> Templates {get;} = [];

		public Control Build(object data)
		{
			return Templates[((User) data).IsOnline].Build(data);
		}

		public bool Match(object data)
		{
			return data is User user && Templates.ContainsKey(user.IsOnline);
		}
	}

}

