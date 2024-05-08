using ChitChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitChat.ViewModels
{
	public class RegisterUser : User
	{
        public string? Password { get; set; }

        public string? ConfirmPassword { get; set; }

		public RegisterUser() : base()
		{
            Username = "";
            Email = "";
            Password = "";
            ConfirmPassword = "";
        }

        public RegisterUser(string username, string email, string password, string confirmPassword) : base(username, email)
        {
            Username = username;
            Email = email;
            Password = password;
            ConfirmPassword = confirmPassword;
        }

        // Check if email and password is not empty
        public bool IsLoginValid()
        {
            return !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password);
        }

        // Check if username, email, password, confirmPassword is not empty 
        public bool IsRegisterValid()
        {
            return !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(ConfirmPassword);
        }

        // TODO Regex check if email is valid


        // Check if password and confirm password match
        public bool PasswordsMatch()
        {
            return Password == ConfirmPassword;
        }

        // Clear data
        public void ClearData()
        {
            Username = "";
            Email = "";
            Password = "";
            ConfirmPassword = "";
        }

        public override string ToString()
        {
            return $"Username: {Username}, Email: {Email}, Password: {Password}, ConfirmPassword: {ConfirmPassword}";
        }
	}
}
