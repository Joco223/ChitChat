using ChitChat.Helpers;
using ChitChat.ViewModels;
using Supabase.Gotrue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitChat.Services
{
	public class UserService
	{
		private static readonly UserService instance = new();
		private readonly SupabaseHandler supabaseHandler = SupabaseHandler.Instance;

        public static UserService Instance { get => instance; }

		private UserService() { }

		public async Task<bool> RegisterUser(RegisterUser registerUser)
		{
            if (registerUser.PasswordsMatch())
			{

                SignUpOptions options = new()
				{
                    Data = registerUser.GetCustomData(),
                };

                try
				{
                    Session? session = await supabaseHandler.Client.Auth.SignUp(registerUser.Email, registerUser.Password, options);

                    if (session != null)
					{
                        return true;
                    }
                }
                catch (Exception ex)
				{
                    Console.WriteLine(ex.Message);
                }
            }

            return false;
        }
	}
}
