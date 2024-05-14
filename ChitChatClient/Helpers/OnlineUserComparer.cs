using ChitChatClient.Models;

namespace ChitChatClient.Helpers {
	/// <summary>
	/// Comparer for sorting a list of users
	/// Online users go first and then offline users
	/// </summary>
	public class OnlineUserComparer : IComparer<User> {
		public int Compare(User? x, User? y) {
			if (x == null || y == null) {
				return 0;
			}

			if (x.IsOnline && !y.IsOnline) {
				return -1;
			} else if (!x.IsOnline && y.IsOnline) {
				return 1;
			} else {
				return 0;
			}
		}
	}
}
