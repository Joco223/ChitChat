using System.Windows;
using System.Windows.Controls;

using ChitChatClient.Helpers;
using ChitChatClient.Services;

namespace ChitChatClient.Windows {
	/// <summary>
	/// Interaction logic for ReleaseNotesWindow.xaml
	/// </summary>
	public partial class ReleaseNotesWindow : Window {

		private readonly MarkdownGenerator markdownGenerator = MarkdownGenerator.Instance;

		public ReleaseNotesWindow() {
			InitializeComponent();
		}

		private async void Window_Loaded(object sender, RoutedEventArgs e) {
			var notes = await GetreleaseNotesMarkdown();

			if (notes.Failed) {
				MessageBox.Show(notes.Error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			// Add notes to scroll viewer
			notesScrollViewer.Content = notes.Data;

			// Set version to title
			Version version = UpdateService.GetCurrentVersion();

			Title = $"Release notes - {version}";
			titleLabel.Content = $"Release notes - {version}";
		}

		private async Task<Result<StackPanel>> GetreleaseNotesMarkdown() {
			// Get the current release notes
			var notes = await UpdateService.GetCurrentChangelog();

			if (notes.Failed)
				return Result<StackPanel>.Fail(notes.Error);

			// Generate the UI component
			var panel = markdownGenerator.GenerateFromText(notes.Data);

			// Check for errors
			if (panel.Failed)
				return Result<StackPanel>.Fail(panel.Error);

			return Result<StackPanel>.OK(panel.Data);
		}
	}
}
