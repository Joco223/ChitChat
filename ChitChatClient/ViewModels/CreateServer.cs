namespace ChitChatClient.ViewModels {
	/// <summary>
	/// Class to handle creating a server
	/// </summary>
	public class CreateServer {
		public string Name { get; set; }

		public string Description { get; set; }

		public CreateServer() {
			Name = string.Empty;
			Description = string.Empty;
		}
	}
}
