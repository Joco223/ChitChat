// using System.Windows;
// using System.Windows.Controls;

// using ChitChatClient.Models;

// namespace ChitChatClient.Helpers {
// 	/// <summary>
// 	/// DTS for representing online and offline users
// 	/// Selects the appropriate template based on the users online status
// 	/// </summary>
// 	public class UserOnlineDTS : DataTemplateSelector {
// 		public DataTemplate? OnlineTemplate { get; set; }
// 		public DataTemplate? OfflineTemplate { get; set; }

// 		public override DataTemplate? SelectTemplate(object item, DependencyObject container) {
// 			// Add your condition here to select the appropriate template
// 			if (item is User yourObject) {
// 				if (yourObject.IsOnline) {
// 					return OnlineTemplate;
// 				} else {
// 					return OfflineTemplate;
// 				}
// 			}

// 			return base.SelectTemplate(item, container);
// 		}
// 	}
// }
