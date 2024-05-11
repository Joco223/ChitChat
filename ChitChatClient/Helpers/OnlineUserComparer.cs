using ChitChatClient.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitChatClient.Helpers
{
	public class OnlineUserComparer : IComparer<User>
	{
		public int Compare(User? x, User? y)
		{
			if (x == null || y == null)
			{
				return 0;
			}

			if (x.IsOnline && !y.IsOnline)
			{
				return -1;
			}
			else if (!x.IsOnline && y.IsOnline)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}
	}
}
