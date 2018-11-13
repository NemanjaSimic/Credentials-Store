﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using System.ServiceModel;
using System.Security;
using Datebase;
using Security;
using Security.cs;

namespace CredentialsStore
{
    class AccountManagement : IAccountManagement, IUserAccountManagement
    {
        public void CreateAccount(string username, string password)
        {
            if (PasswordPolicy.ValidatePassword(password))
            {
				int pass = password.GetHashCode();
				if (DBManager.Instance.AddUser(new User(username, pass)))
                {
					DBManager.Instance.AddPasswordToHistory(username, pass);
					Console.WriteLine("User {0} successfully created!",username);
                }
                else
                {
					CredentialsException ex = new CredentialsException();
					ex.Reason = "User with this username already exists";
					throw new FaultException<CredentialsException>(ex, new FaultReason("User with this username already exists"));
				}
            }
            else
            {
				CredentialsException ex = new CredentialsException();
				ex.Reason = "Password must contains at least one upper case,one lower case, one number and one special char";
				throw new FaultException<CredentialsException>(ex, new FaultReason("Password must contains at least one upper case,one lower case, one number and one special char"));
			}
        }

        public void DeleteAccount(string username)
        {
            User user = DBManager.Instance.GetUserByUsername(username);

            if(user != null && DBManager.Instance.DeleteUser(user))
            {
				//Console.WriteLine("User {0} successfully deleted!",username);
				DBManager.Instance.DeletePasswordToHistory(username);
				NetTcpBinding binding = new NetTcpBinding();
				string address = "net.tcp://localhost:9995/AuthenticationService";
				using (ProxyAuthenticationCheck proxy = new ProxyAuthenticationCheck(binding, new EndpointAddress(new Uri(address))))
				{
					proxy.NotifyClientsAndLogOut(username, "Your account has been deleted by admin.You are logged out!");
				}
				
			}
			else
            {
				CredentialsException ex = new CredentialsException();
				ex.Reason = "User does not exist or is already deleted by other admin.";
				throw new FaultException<CredentialsException>(ex, new FaultReason("User does not exist or is already deleted by other admin."));
			}
        }

        public void ResetPassword(string username,string newPassword)
        {
            User user = DBManager.Instance.GetUserByUsername(username);

			if (user != null)
			{
                if (PasswordPolicy.ValidatePassword(newPassword))
                {
					int password = newPassword.GetHashCode();

					if (PasswordPolicy.CanResetPassword(username,password))
                    {				
                        if (!DBManager.Instance.DeleteUser(user))
                        {
							CredentialsException ex = new CredentialsException();
							ex.Reason = "There was a conflict. Someone else is changing informations about this user.Trying to make things right...";
							throw new FaultException<CredentialsException>(ex, new FaultReason("There was a conflict. Someone else is changing informations about this user.Trying to make things right..."));
                        }
                        user.Password = password;
                        user.PasswordInitialized = DateTime.Now;
                        DBManager.Instance.AddPasswordToHistory(user.Username,password);
                        if (!DBManager.Instance.AddUser(user))
                        {
							CredentialsException ex = new CredentialsException();
							ex.Reason = "There was a conflict. Someone else is changing informations about this user.";
							throw new FaultException<CredentialsException>(ex, new FaultReason("There was a conflict. Someone else is changing informations about this user."));
                        }
                    }
                    else
                    {
						CredentialsException ex = new CredentialsException();
						ex.Reason = "This password has been used too many times.";
						throw new FaultException<CredentialsException>(ex, new FaultReason("This password has been used too many times."));
                    }

                }
                else
                {
					CredentialsException ex = new CredentialsException();
					ex.Reason = "Password must contains at least one upper case,one lower case, one number and one special char.";
					throw new FaultException<CredentialsException>(ex, new FaultReason("Password must contains at least one upper case,one lower case, one number and one special char."));				
                }
            }
			else
			{
				CredentialsException ex = new CredentialsException();
				ex.Reason = "User with this username does not exist.";
				throw new FaultException<CredentialsException>(ex, new FaultReason("User with this username does not exist."));
			}
        }
		public void ResetPassword(string username, string oldPassword, string newPassword)
		{
			NetTcpBinding binding = new NetTcpBinding();
			string address = "net.tcp://localhost:9995/AuthenticationService";
			bool isAuth = false;
			using (ProxyAuthenticationCheck proxy = new ProxyAuthenticationCheck(binding, new EndpointAddress(new Uri(address))))
			{

				try
				{
					if (proxy.IsAuthenticated(username))
					{
						isAuth = true;
					}
				}
				catch (Exception ex)
				{
					throw ex;
				}

			}

			if (isAuth)
			{
				User user = DBManager.Instance.GetUserByUsername(username);
				int oldPass = oldPassword.GetHashCode();
				if (user != null)
				{
					if (user.Password.Equals(oldPass))
					{
						if (PasswordPolicy.ValidatePassword(newPassword))
						{
							int newPass = newPassword.GetHashCode();
							if (PasswordPolicy.CanResetPassword(username, newPass))
							{
								if (!DBManager.Instance.DeleteUser(user))
								{
									CredentialsException ex = new CredentialsException();
									ex.Reason = "There was a conflict. Someone else is changing informations about this user.Trying to make things right...";
									throw new FaultException<CredentialsException>(ex, new FaultReason("There was a conflict. Someone else is changing informations about this user.Trying to make things right..."));
								}
								user.PasswordInitialized = DateTime.Now;
								user.Password = newPass;
								DBManager.Instance.AddPasswordToHistory(user.Username, newPass);
								if (!DBManager.Instance.AddUser(user))
								{
									CredentialsException ex = new CredentialsException();
									ex.Reason = "There was a conflict. Someone else is changing informations about this user.";
									throw new FaultException<CredentialsException>(ex, new FaultReason("There was a conflict. Someone else is changing informations about this user."));
								}
							}
							else
							{
								CredentialsException ex = new CredentialsException();
								ex.Reason = "This password has been used too many times.";
								throw new FaultException<CredentialsException>(ex, new FaultReason("This password has been used too many times."));
							}

						}
						else
						{
							CredentialsException ex = new CredentialsException();
							ex.Reason = "Password must contains at least one upper case,one lower case, one number and one special char.";
							throw new FaultException<CredentialsException>(ex, new FaultReason("There was a conflict. Someone else is changing informations about this user.Trying to make things right..."));
						}
					}
					else
					{
						CredentialsException ex = new CredentialsException();
						ex.Reason = "Invalid password.";
						throw new FaultException<CredentialsException>(ex, new FaultReason("Invalid password."));
					}
				}
				else
				{
					CredentialsException ex = new CredentialsException();
					ex.Reason = "User with this username does not exist.";
					throw new FaultException<CredentialsException>(ex, new FaultReason("User with this username does not exist."));
				}
			}
			else
			{
				CredentialsException ex = new CredentialsException();
				ex.Reason = "User is not logged in.";
				throw new FaultException<CredentialsException>(ex, new FaultReason("User is not logged in."));
			}
		}
	}
}
