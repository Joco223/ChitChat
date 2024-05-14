using System.Windows;
using System.Windows.Controls;

namespace ChitChatClient.Controls {
	/// <summary>
	/// Extension of default TextBox control with placeholder functionality
	/// </summary>
	public class PlaceholderTextBox : TextBox {
		public string Placeholder { get; set; }

		public PlaceholderTextBox() : base() {
			Placeholder = string.Empty;
			GotFocus += PlaceholderTextBox_GotFocus;
			LostFocus += PlaceholderTextBox_LostFocus;
			Initialized += PlaceholderTextBox_Initialized;
		}

		/// <summary>
		/// Shows placeholder text when text box is initialized
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PlaceholderTextBox_Initialized(object? sender, EventArgs e) {
			if (string.IsNullOrEmpty(Text)) {
				ShowPlaceholder();
			}
		}

		/// <summary>
		/// On focus action to hide placeholder when user clicks the text box
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PlaceholderTextBox_GotFocus(object sender, RoutedEventArgs e) {
			HidePlaceholder();
		}

		/// <summary>
		/// Lost focus action to show placeholder if text is empty
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PlaceholderTextBox_LostFocus(object sender, RoutedEventArgs e) {
			if (string.IsNullOrEmpty(Text)) {
				ShowPlaceholder();
			}
		}

		/// <summary>
		/// Shows placeholder and sets foreground to gray
		/// </summary>
		public void ShowPlaceholder() {
			Text = Placeholder;
			Foreground = System.Windows.Media.Brushes.Gray;
		}

		/// <summary>
		/// Hides placeholder and sets foreground to black
		/// </summary>
		public void HidePlaceholder() {
			Text = string.Empty;
			Foreground = System.Windows.Media.Brushes.Black;
		}
	}
}
