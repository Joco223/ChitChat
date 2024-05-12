using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ChitChatClient.Helpers
{
	public static class PlaceholderProperty
	{
		public static readonly DependencyProperty PlaceholderPropertyProperty = DependencyProperty.RegisterAttached("Placeholder",
			typeof(string), typeof(PlaceholderProperty), new FrameworkPropertyMetadata(null));

		public static string GetPlaceholder(UIElement element)
		{
			ArgumentNullException.ThrowIfNull(element);

			return (string)element.GetValue(PlaceholderPropertyProperty);
		}

		public static void SetPlaceholder(UIElement element, string value)
		{
			ArgumentNullException.ThrowIfNull(element);

			element.SetValue(PlaceholderPropertyProperty, value);
		}

		public static void ClearPlaceholderText(TextBox textbox)
		{
			// If the text is the placeholder, clear it
			if (textbox.Text == PlaceholderProperty.GetPlaceholder(textbox))
			{
				textbox.Text = string.Empty;
				textbox.Foreground = Brushes.Black;
			}
		}

		public static void SetPlaceholderText(TextBox textbox)
		{
			// If there is no text, set the placeholder as the text
			if (string.IsNullOrEmpty(textbox.Text))
			{
				textbox.Text = PlaceholderProperty.GetPlaceholder(textbox);
				textbox.Foreground = Brushes.Gray;
			}
		}
	}
}
