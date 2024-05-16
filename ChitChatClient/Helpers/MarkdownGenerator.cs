using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

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
		Header6,
		StarList,
		DashList
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

		// Singleton instance of markdown generator
		public static readonly MarkdownGenerator Instance = new();

		/// <summary>
		/// List of text sizes for different elements
		/// </summary>
		private List<int> TextSizes = new([12, 14, 16, 18, 20, 22, 24, 12, 12]);

		// These are default text sizes for different elements
		public int NormalTextSize { get => TextSizes[0]; set => TextSizes[0] = value; }

		public int Header1Size { get => TextSizes[1]; set => TextSizes[1] = value; }
		public int Header2Size { get => TextSizes[2]; set => TextSizes[2] = value; }
		public int Header3Size { get => TextSizes[3]; set => TextSizes[3] = value; }
		public int Header4Size { get => TextSizes[4]; set => TextSizes[4] = value; }
		public int Header5Size { get => TextSizes[5]; set => TextSizes[5] = value; }
		public int Header6Size { get => TextSizes[6]; set => TextSizes[6] = value; }

		public int StarBulletListSize { get => TextSizes[7]; set => TextSizes[7] = value; }
		public int DashBulletListSize { get => TextSizes[8]; set => TextSizes[8] = value; }

		public int LineSpacing { get; set; } = 0;
		public int ElementSpacing { get; set; } = 10;

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

			var result = GenerateMarkdown(linesData);

			if (result.Failed)
				return Result<StackPanel>.Fail(result.Error);

			return Result<StackPanel>.OK(result.Data);
		}

		/// <summary>
		/// Generate a StackPanel component from a text
		/// </summary>
		/// <param name="text">Text to parse</param>
		/// <returns></returns>
		public Result<StackPanel> GenerateFromText(string text) {
			var lines = text.Split("\n");

			if (lines.Length == 0)
				return Result<StackPanel>.Fail("No lines in text");

			var linesData = lines.ToList();

			var result = GenerateMarkdown(linesData);

			if (result.Failed)
				return Result<StackPanel>.Fail(result.Error);

			return Result<StackPanel>.OK(result.Data);
		}

		/// <summary>
		/// Generate a StackPanel component from a list of lines
		/// </summary>
		/// <param name="linesData"></param>
		/// <returns></returns>
		private Result<StackPanel> GenerateMarkdown(List<string> linesData) {
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

			// For gaps between elements
			MarkdownElement spaceElement = new() {
				Type = MarkdownElementType.Normal,
				SubType = MarkdownElementSubTypes.Normal
			};

			spaceElement.TextLines.Add("\n");

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
					} else if (line.StartsWith("* ")) { // Bullet list
						currentElement.Type = MarkdownElementType.BulletList;
						currentElement.SubType = MarkdownElementSubTypes.StarList;

						// Remove bullet point from line
						string removedStar = RemoveFirst(line, "* ");

						currentElement.TextLines.Add(removedStar);
					} else if (line.StartsWith("- ")) { // Bullet list
						currentElement.Type = MarkdownElementType.BulletList;
						currentElement.SubType = MarkdownElementSubTypes.DashList;

						// Remove bullet point from line
						string removedDash = RemoveFirst(line, "- ");

						currentElement.TextLines.Add(removedDash);
					} else {
						currentElement.Type = MarkdownElementType.Normal;

						// If line is empty, add new line character to it
						if (string.IsNullOrWhiteSpace(line)) {
							line += '\n';
							currentElement.TextLines.Add(line);
							currentElement = new();
						} else {
							currentElement.TextLines.Add(line);
						}
					}
				} else {
					if (string.IsNullOrWhiteSpace(line)) {
						markdownLines.Add(currentElement);

						// Add a normal element with just a newline to make space between lines
						//markdownLines.Add(spaceElement);

						currentElement = new();
					} else {
						currentElement.TextLines.Add(line);
					}
				}
			}

			// If element is not nothing, add it
			if (currentElement.Type != MarkdownElementType.Nothing)
				markdownLines.Add(currentElement);

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
		private Result<StackPanel> ProcessBulletList(MarkdownElement element) {
			// Calculate font size
			var fontSize = GetTextSize(element);

			if (fontSize.Failed)
				return Result<StackPanel>.Fail(fontSize.Error);

			// Create a stack panel for the bullet list
			StackPanel result = new() {
				Margin = new Thickness(0, 0, 0, ElementSpacing)
			};

			foreach (string line in element.TextLines) {
				// Split line into before and after the bullet point
				List<string> parts = [];

				// Check valididty of bullet point
				if (!line.Contains('*') && !line.Contains('-') && element.TextLines.IndexOf(line) != 0)
					return Result<StackPanel>.Fail("Invalid bullet list line (no bullet point found)");

				// Split into 3 (before bullet point, bullet point, after bullet point)
				// Depending on type of bullet point
				if (element.SubType == MarkdownElementSubTypes.StarList) {
					parts = [.. line.Split('*')];
				} else if (element.SubType == MarkdownElementSubTypes.DashList) {
					parts = [.. line.Split('-')];
				} else {
					return Result<StackPanel>.Fail("Invalid bullet list type");
				}

				// Current line content
				TextBlock content = new() {
					FontSize = fontSize.Data,
					TextWrapping = TextWrapping.WrapWithOverflow,
					Margin = new Thickness(0, 0, 0, LineSpacing)
				};

				// Add content before the bullet point
				if (element.TextLines.IndexOf(line) != 0)
					content.Inlines.Add(new Run(parts[0]));

				// Bullet list element
				BulletDecorator bulletDecorator = new() {
					Margin = new Thickness(5, 0, 5, 0),
					Bullet = new Ellipse() {
						Width = 5 * GlobalScaling,
						Height = 5 * GlobalScaling,
						Fill = Brushes.Black
					}
				};

				List<string> tokens = [];

				// Split line into tokens
				if (element.TextLines.IndexOf(line) == 0) {
					tokens = [.. line.Split(" ")];
				} else {
					// Set non filled ellipse if line is indented
					if (string.IsNullOrWhiteSpace(parts[0]) && parts[0] != string.Empty) {
						bulletDecorator.Bullet = new Ellipse() {
							Width = 5 * GlobalScaling,
							Height = 5 * GlobalScaling,
							Stroke = Brushes.Black,
							StrokeThickness = 1 * GlobalScaling
						};
					}

					// Split text into words
					tokens = [.. parts[1].Split(" ", StringSplitOptions.RemoveEmptyEntries)];
				}

				TextBlock bulletText = new() {
					FontSize = fontSize.Data,
					TextWrapping = TextWrapping.WrapWithOverflow,
				};

				// Add gap after bulletpoint
				bulletText.Inlines.Add(" ");

				// Process each token in the line
				foreach (string token in tokens) {
					var decoratedToken = ProcessToken(token);

					// Check if there was an error
					if (decoratedToken.Failed)
						return Result<StackPanel>.Fail(decoratedToken.Error);

					bulletText.Inlines.Add(decoratedToken.Data);

					// If it isn't the last token, add space
					if (tokens.IndexOf(token) < tokens.Count - 1) {
						bulletText.Inlines.Add(" ");
					}
				}

				bulletDecorator.Child = bulletText;
				content.Inlines.Add(bulletDecorator);

				// Add textblock to the stackpanel
				result.Children.Add(content);
			}

			return Result<StackPanel>.OK(result);
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
				TextWrapping = TextWrapping.WrapWithOverflow,
				Margin = new Thickness(0, 0, 0, ElementSpacing)
			};

			// If it is header 1 or 2 add underline text decoration
			if (element.SubType == MarkdownElementSubTypes.Header1 || element.SubType == MarkdownElementSubTypes.Header2) {
				result.TextDecorations = TextDecorations.Underline;
			}

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
			if (element.Type == MarkdownElementType.Nothing) {
				Log.Error($"Invalid markdown element passed to GetTextSize {element.GetDebugInfo()}");
				return Result<int>.Fail("Invalid markdown element passed to GetTextSize");
			}

			int originalSize = TextSizes[(int)element.SubType];
			int scaledSize = (int)Math.Round(originalSize * GlobalScaling);
			return Result<int>.OK(scaledSize);
		}
	}
}
