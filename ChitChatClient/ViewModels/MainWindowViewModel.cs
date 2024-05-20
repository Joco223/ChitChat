using ChitChatClient.DatabaseModels;

namespace ChitChatClient.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
	public string Username { get; set; }

	public string Email { get; set; }

	public string Password { get; set; }

	public string ConfirmPassword { get; set; }

	public MainWindowViewModel()
	{
		Username = string.Empty;
		Email = string.Empty;
		Password = string.Empty;
		ConfirmPassword = string.Empty;
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
	/// Get user data
	/// </summary>
	/// <returns></returns>
	public User GetUser() => new(Username, Email);

	/// <summary>
	/// Clears data
	/// </summary>
	public void ClearData() {
		Username = "";
		Email = "";
		Password = "";
		ConfirmPassword = "";
	}

	public string GetDebugInfo() => $"Username: {Username}, Email: {Email}, Password: {Password}, ConfirmPassword: {ConfirmPassword}";
}
