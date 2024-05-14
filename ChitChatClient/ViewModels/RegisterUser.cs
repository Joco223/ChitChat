using ChitChatClient.Models;

namespace ChitChatClient.ViewModels {
	/// <summary>
	/// Class to handle registering a user
	/// </summary>
	public class RegisterUser : User {
		public string Password { get; set; }

		public string ConfirmPassword { get; set; }

		public RegisterUser() : base() {
			Username = "";
			Email = "";
			Password = "";
			ConfirmPassword = "";
		}

		/// <summary>
		/// Check if login data is valid
		/// </summary>
		/// <returns></returns>
		public bool IsLoginValid() => !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password);

		/// <summary>
		/// Check if register data is valid
		/// </summary>
		/// <returns></returns>
		public bool IsRegisterValid() => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(ConfirmPassword);

		// TODO Regex check if email is valid

		/// <summary>
		/// Check if passwords match
		/// </summary>
		/// <returns></returns>
		public bool PasswordsMatch() => Password == ConfirmPassword;

		/// <summary>
		/// Clears data
		/// </summary>
		public void ClearData() {
			Username = "";
			Email = "";
			Password = "";
			ConfirmPassword = "";
		}

		public new string GetDebugInfo() => $"Username: {Username}, Email: {Email}, Password: {Password}, ConfirmPassword: {ConfirmPassword}";
	}
}
