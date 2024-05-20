using System.Collections.Generic;
using ChitChatClient.Models;

namespace ChitChatClient.Helpers {
	/// <summary>
	/// Comparer for sorting a list of users
	/// Online users go first and then offline users
	/// </summary>
	public class OnlineUserComparer : IComparer<ServerUser> {
		public int Compare(ServerUser? x, ServerUser? y) {
			if (x == null || y == null) {
				return 0;
			}

			if (x.User.IsOnline && !y.User.IsOnline) {
				return -1;
			} else if (!x.User.IsOnline && y.User.IsOnline) {
				return 1;
			} else {
				return 0;
			}
		}
	}
}
