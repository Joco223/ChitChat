﻿using ChitChatClient.Helpers;
using ChitChatClient.Models;
using ChitChatClient.ViewModels;
using Supabase.Gotrue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitChatClient.Services
{
	public class UserService
	{
		private static readonly UserService instance = new();
		private readonly SupabaseHandler supabaseHandler = SupabaseHandler.Instance;

        public static UserService Instance { get => instance; }

		private UserService() { }


        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="registerUser">RegisterUser object with all necessary data</param>
        /// <returns></returns>
		public async Task<bool> RegisterUser(RegisterUser registerUser)
		{
            if (registerUser.PasswordsMatch())
			{
                try
				{
                    Session? session = await supabaseHandler.Client.Auth.SignUp(registerUser.Email, registerUser.Password);

                    if (session != null && session.User != null && session.User.Id != null)
					{
                        // Generate user data
                        var user = new Models.User(registerUser.Username, registerUser.Email, session.User.Id);

                        // Insert user data
                        var response = await supabaseHandler.Client.From<Models.User>().Insert(user);

                        if (response != null)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                catch (Exception ex)
				{
                    Console.WriteLine(ex.Message);
                }
            }

            return false;
        }

        /// <summary>
        /// Gets all users in a server
        /// </summary>
        /// <param name="server">Server from which to get users</param>
        /// <returns></returns>
        public async Task<List<Models.User>> GetServerUsers(Server? server)
        {
            List<Models.User> users = [];

            if (server == null)
            {
				return users;
			}

			try
			{
                var usjRequest = await supabaseHandler.Client.From<UserServerJoin>().Where(usj => usj.ServerId == server.Id).Get();
                var usjList = usjRequest.Models;
                var usersId = usjList.Select(usj => usj.UserId).ToList();

                if (usjRequest != null)
                {
                    var userRequest = await supabaseHandler.Client.From<Models.User>().Filter(u => u.Id, Supabase.Postgrest.Constants.Operator.In, usersId).Get();
                    users = userRequest.Models;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

            return users;
        }

        /// <summary>
        /// Gets id of currently logged in user
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetUserId()
        {
            try
            {
                var user = supabaseHandler.Client.Auth.CurrentUser;

                if (user != null && user.Id != null)
                {
                    var userRequest = await supabaseHandler.Client.From<Models.User>().Where(u => u.Uuid == user.Id).Get();
                    var userResponse = userRequest.Model;

                    if (userResponse != null)
                    {
                        return userResponse.Id;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return -1;
        }

        /// <summary>
        /// Sets the online status of the currently logged in user
        /// </summary>
        /// <param name="status">True or false for online status</param>
        /// <returns></returns>
        public async Task<bool> SetUserOnline(bool status)
        {
			try
            {
				var user = supabaseHandler.Client.Auth.CurrentUser;

				if (user != null && user.Id != null)
                {

					var userRequest = await supabaseHandler.Client.From<Models.User>().Where(u => u.Uuid == user.Id).Get();
					var userResponse = userRequest.Model;

					if (userResponse != null)
                    {
						userResponse.IsOnline = status;
						var response = await supabaseHandler.Client.From<Models.User>().Update(userResponse);

						if (response != null)
                        {
							return true;
						}
					}
				}
			}
			catch (Exception ex)
            {
				Console.WriteLine(ex.Message);
			}

			return false;
		}
    }
}
