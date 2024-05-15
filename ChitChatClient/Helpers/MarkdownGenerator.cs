using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using Serilog;

namespace ChitChatClient.Helpers {

	/// <summary>
	/// Enum for different types of markdown elements
	/// </summary>
	public enum MarkdownElementType {
		Header,
		BulletList,
		Normal,
		Nothing
	}

	/// <summary>
	/// Enum for different subtypes of markdown elements
	/// </summary>
	public enum MarkdownElementSubTypes {
		Normal,
		Header1,
		Header2,
		Header3,
		Header4,
		Header5,
		Header6
	}

	/// <summary>
	/// Helper class for MarkdownGenerator
	/// Which contains information about each line
	/// </summary>
	public class MarkdownElement {
		public MarkdownElementType Type { get; set; }
		public MarkdownElementSubTypes SubType { get; set; }
		public List<string> TextLines { get; set; }

		public MarkdownElement() {
			Type = MarkdownElementType.Nothing;
			SubType = MarkdownElementSubTypes.Normal;
			TextLines = [];
		}

		/// <summary>
		/// Clears data in object
		/// </summary>
		public void Clear() {
			TextLines.Clear();
			Type = MarkdownElementType.Nothing;
			SubType = MarkdownElementSubTypes.Normal;
		}

		public string GetDebugInfo() => $"Type: {Type}, SubType: {SubType}, TextLines: {TextLines.Count}";
	}

	/// <summary>
	/// Class to generate WPF UI components from markdown input text
	/// Currently doesn't support number lists and tables
	/// </summary>
	public class MarkdownGenerator {

		/// <summary>
		/// List of text sizes for different elements
		/// </summary>
		private List<int> TextSizes = new([12, 14, 16, 18, 20, 22, 24]);

		// These are default text sizes for different elements
		public int NormalTextSize { get => TextSizes[0]; set => TextSizes[0] = value; }

		public int Header1Size { get => TextSizes[1]; set => TextSizes[1] = value; }
		public int Header2Size { get => TextSizes[2]; set => TextSizes[2] = value; }
		public int Header3Size { get => TextSizes[3]; set => TextSizes[3] = value; }
		public int Header4Size { get => TextSizes[4]; set => TextSizes[4] = value; }
		public int Header5Size { get => TextSizes[5]; set => TextSizes[5] = value; }
		public int Header6Size { get => TextSizes[6]; set => TextSizes[6] = value; }

		// This is a global scaling factor by witch everything is multiplied
		public float GlobalScaling { get; set; } = 1.0f;

		/// <summary>
		/// Generate StackPanel component from markdown file
		/// </summary>
		/// <param name="filePath">File to load markdown from</param>
		/// <returns></returns>
		public Result<StackPanel> GenerateFromMarkdown(string filePath) {
			// Load file from path and split it into lines
			var lines = GetLinesFromFile(filePath);

			// Check if there was an error
			if (lines.Failed)
				return Result<StackPanel>.Fail(lines.Error);

			var linesData = lines.Data;

			StackPanel rootPanel = new();

			// Enumerate lines in text and generate MarkdownElement objects
			List<MarkdownElement> elements = EnumerateLines(ref linesData);

			// Process each element and add it to the root panel
			foreach (MarkdownElement element in elements) {
				var processedElement = ProcessElement(element);

				// Check if there was an error
				if (processedElement.Failed)
					return Result<StackPanel>.Fail(processedElement.Error);

				rootPanel.Children.Add(processedElement.Data);
			}

			return Result<StackPanel>.OK(rootPanel);
		}

		/// <summary>
		/// Gets all lines from a file and returns them in a list
		/// </summary>
		/// <param name="filePath">Path to load data from</param>
		/// <returns></returns>
		/// <exception cref="FileNotFoundException"></exception>
		private static Result<List<string>> GetLinesFromFile(string filePath) {
			// Check if file exists
			if (!File.Exists(filePath)) {
				Log.Error($"File not found at {filePath}");
				return Result<List<string>>.Fail($"File not found {filePath}");
			}

			// Read file and add each line to the list
			List<string> lines = File.ReadLines(filePath).ToList();

			return Result<List<string>>.OK(lines);
		}

		/// <summary>
		/// Generate a list of MarkdownElement objects which are
		/// enumerated lines that are passed in
		/// </summary>
		/// <param name="lines">Lines to enumerate</param>
		/// <returns></returns>
		private static List<MarkdownElement> EnumerateLines(ref List<string> lines) {
			List<MarkdownElement> markdownLines = [];

			MarkdownElement currentElement = new();

			// This for loop goes trough every input line and generates a MarkdownElement object
			// If MarkdownElement doesn't have a type, determine the type by the current line
			// If MarkdownElement does have a type and current line is empty, add the element to the list and reset it
			// If MarkdownElement does have a type and current line is not empty, add the line to the element
			foreach (string unprocessedLine in lines) {

				string line = unprocessedLine.TrimEnd();

				if (currentElement.Type == MarkdownElementType.Nothing) {
					if (line.StartsWith("# ")) {            // Header 1
						currentElement.Type = MarkdownElementType.Header;
						currentElement.SubType = MarkdownElementSubTypes.Header1;
						currentElement.TextLines.Add(RemoveFirst(line, "# "));
					} else if (line.StartsWith("## ")) {     // Header 2
						currentElement.Type = MarkdownElementType.Header;
						currentElement.SubType = MarkdownElementSubTypes.Header2;
						currentElement.TextLines.Add(RemoveFirst(line, "## "));
					} else if (line.StartsWith("### ")) {    // Header 3
						currentElement.Type = MarkdownElementType.Header;
						currentElement.SubType = MarkdownElementSubTypes.Header3;
						currentElement.TextLines.Add(RemoveFirst(line, "### "));
					} else if (line.StartsWith("#### ")) {   // Header 4
						currentElement.Type = MarkdownElementType.Header;
						currentElement.SubType = MarkdownElementSubTypes.Header4;
						currentElement.TextLines.Add(RemoveFirst(line, "#### "));
					} else if (line.StartsWith("##### ")) {  // Header 5
						currentElement.Type = MarkdownElementType.Header;
						currentElement.SubType = MarkdownElementSubTypes.Header5;
						currentElement.TextLines.Add(RemoveFirst(line, "##### "));
					} else if (line.StartsWith("###### ")) { // Header 6
						currentElement.Type = MarkdownElementType.Header;
						currentElement.SubType = MarkdownElementSubTypes.Header6;
						currentElement.TextLines.Add(RemoveFirst(line, "###### "));
					} else if (line.StartsWith("* ") || line.StartsWith("- ")) { // Bullet list
						currentElement.Type = MarkdownElementType.BulletList;

						// Remove bullet point from line
						string removedStar = RemoveFirst(line, "* ");
						string removedDash = RemoveFirst(removedStar, "- ");

						currentElement.TextLines.Add(removedDash);
					} else {
						currentElement.Type = MarkdownElementType.Normal;
						currentElement.TextLines.Add(line);
					}
				} else {
					if (string.IsNullOrWhiteSpace(line)) {
						markdownLines.Add(currentElement);
						currentElement.Clear();
					} else {
						currentElement.TextLines.Add(line);
					}
				}
			}

			return markdownLines;
		}

		/// <summary>
		/// Removes the first occurence of a string from another string
		/// </summary>
		/// <param name="source">String to remove substring from</param>
		/// <param name="remove">Substring to remove</param>
		/// <returns></returns>
		private static string RemoveFirst(string source, string remove) {
			int index = source.IndexOf(remove);
			return (index < 0) ? source : source.Remove(index, remove.Length);
		}

		/// <summary>
		/// Processes a MarkdownElement object and returns a StackPanel UI component
		/// </summary>
		/// <param name="element">Element to process</param>
		/// <returns></returns>
		private Result<StackPanel> ProcessElement(MarkdownElement element) {
			StackPanel result = new();

			switch (element.Type) {
				// If element is a header or normal text, process it as text
				case MarkdownElementType.Normal:
				case MarkdownElementType.Header:
					var header = ProcessText(element);

					// Check if there was an error
					if (header.Failed)
						return Result<StackPanel>.Fail(header.Error);

					result.Children.Add(header.Data);
					break;
				case MarkdownElementType.BulletList:
					var list = ProcessBulletList(element);

					// Check if there was an error
					if (list.Failed)
						return Result<StackPanel>.Fail(list.Error);

					result.Children.Add(list.Data);
					break;
				default:
					Log.Error("Invalid markdown element passed to ProcessElement");
					return Result<StackPanel>.Fail("Invalid markdown element passed to ProcessElement");
			}

			return Result<StackPanel>.OK(result);
		}

		/// <summary>
		/// Process a MarkdownElement object and return a StackPanel UI component
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		private Result<FlowDocumentReader> ProcessBulletList(MarkdownElement element) {
			// Calculate font size
			var fontSize = GetTextSize(element);

			if (fontSize.Failed)
				return Result<FlowDocumentReader>.Fail(fontSize.Error);

			// Create a stack panel for the bullet list
			FlowDocument result = new();

			// Create list object to add it to the flow document
			List list = new() {
				MarkerOffset = 5,
				MarkerStyle = TextMarkerStyle.Circle
			};

			foreach (string line in element.TextLines) {
				List<string> tokens = [.. line.Split(" ")];

				// Create a list item for each line
				ListItem listItem = new();
				Paragraph itemText = new();

				// Process each token in the line
				foreach (string token in tokens) {
					var decoratedToken = ProcessToken(token);

					// Check if there was an error
					if (decoratedToken.Failed)
						return Result<FlowDocumentReader>.Fail(decoratedToken.Error);

					itemText.Inlines.Add(decoratedToken.Data);

					// If it isn't the last token, add space
					if (tokens.IndexOf(token) < tokens.Count - 1) {
						itemText.Inlines.Add(" ");
					}
				}

				// Add textblock to the list item
				listItem.Blocks.Add(itemText);

				// Add list item to the list
				list.ListItems.Add(listItem);
			}

			// Add list with elements to the flow document
			result.Blocks.Add(list);

			// Convert result to flow document reader
			// so it can be added to the final stack panel
			FlowDocumentReader flowDocumentReader = new() {
				Document = result
			};

			return Result<FlowDocumentReader>.OK(flowDocumentReader);
		}

		/// <summary>
		/// Processes a MarkdownElement object and returns a TextBlock UI component
		/// </summary>
		/// <param name="element">Element to process</param>
		/// <returns></returns>
		private Result<TextBlock> ProcessText(MarkdownElement element) {
			// Calculates font size
			var fontSize = GetTextSize(element);

			// Check if there was an error
			if (fontSize.Failed)
				return Result<TextBlock>.Fail(fontSize.Error);

			// Sets font weight to bold, calculates size and sets overflow
			TextBlock result = new() {
				FontWeight = FontWeights.Bold,
				FontSize = fontSize.Data,
				TextWrapping = TextWrapping.WrapWithOverflow
			};

			// Joins all lines into one line
			string text = string.Join("", element.TextLines);

			// Tokenizes text by splitting it on spaces
			List<string> tokens = [.. text.Split(" ")];

			// Checks each token for text decorations
			foreach (string token in tokens) {
				var decoratedToken = ProcessToken(token);

				if (decoratedToken.Failed)
					return Result<TextBlock>.Fail(decoratedToken.Error);

				// Add token to the textblock
				result.Inlines.Add(decoratedToken.Data);

				// If it isn't the last token, add space
				if (tokens.IndexOf(token) < tokens.Count - 1) {
					result.Inlines.Add(" ");
				}
			}

			return Result<TextBlock>.OK(result);
		}

		/// <summary>
		/// Processes a token and returns a Run object with decorations
		/// </summary>
		/// <param name="token">String to be processed</param>
		/// <returns></returns>
		private static Result<Run> ProcessToken(string token) {
			if (string.IsNullOrEmpty(token)) {
				Log.Error("Empty token passed to ProcessToken");
				return Result<Run>.Fail("Empty token passed to ProcessToken");
			}

			Run decoratedToken = new() {
				Text = token // Text of the decoration is the token
			};

			if (token.StartsWith('*') && token.EndsWith('*') && token.Length > 1) {         // Bold text
				decoratedToken.FontWeight = FontWeights.Bold;
			} else if (token.StartsWith("__") && token.EndsWith("__") && token.Length > 2) { // Underline text
				decoratedToken.TextDecorations = TextDecorations.Underline;
			} else if (token.StartsWith('_') && token.EndsWith('_') && token.Length > 1) {   // Italic text
				decoratedToken.FontStyle = FontStyles.Italic;
			} else if (token.StartsWith('~') && token.EndsWith('~') && token.Length > 1) {   // Striketrough text
				decoratedToken.TextDecorations = TextDecorations.Strikethrough;
			}

			return Result<Run>.OK(decoratedToken);
		}

		/// <summary>
		/// Gets font size for given header or text element
		/// </summary>
		/// <param name="element">Element to get text size from</param>
		/// <returns></returns>
		private Result<int> GetTextSize(MarkdownElement element) {
			if (element.Type != MarkdownElementType.Header && element.Type != MarkdownElementType.Normal) {
				Log.Error($"Invalid markdown element passed to GetTextSize {element.GetDebugInfo()}");
				return Result<int>.Fail("Invalid markdown element passed to GetTextSize");
			}

			int originalSize = TextSizes[(int)element.SubType];
			int scaledSize = (int)Math.Round(originalSize * GlobalScaling);
			return Result<int>.OK(scaledSize);
		}
	}
}
