using ChitChatClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ChitChatClient.Helpers
{
	public class UserOnlineDTS : DataTemplateSelector
	{

		public DataTemplate? OnlineTemplate { get; set; }
		public DataTemplate? OfflineTemplate { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			// Add your condition here to select the appropriate template
			if (item is User yourObject)
			{
				if (yourObject.IsOnline)
				{
					return OnlineTemplate;
				}
				else
				{
					return OfflineTemplate;
				}
			}

			return base.SelectTemplate(item, container);
		}
	}
}
