using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using System.ServiceModel;
using System.Security;
using Datebase;
using Security;

namespace CredentialsStore
{
    class AccountManagement : IAccountManagement
    {
        public void CreateAccount(string username, SecureString password)
        {
            if (PasswordPolicy.ValidatePassword(password))
            {
                if (DBManager.Instance.AddUser(new User(username, password.GetHashCode())))
                {
					Console.WriteLine("User {0} successfully created!",username);
                }
                else
                {
					SecurityException ex = new SecurityException("User with this username already exists");
					throw ex;
				}
            }
            else
            {
				SecurityException ex = new SecurityException("Password must contains at least one upper case,one lower case, one number and one special char");
				throw ex;
			}
        }

        public void DeleteAccount(string username)
        {
            User user = DBManager.Instance.GetUserByUsername(username);

            if(user != null && DBManager.Instance.DeleteUser(user))
            {
					Console.WriteLine("User {0} successfully deleted!",username);
			}
			else
            {
				SecurityException ex = new SecurityException("User does not exist or is already deleted by other admin.");
				throw ex;
			}
        }

        public void ResetPassword(string username,SecureString newPassword)
        {
            User user = DBManager.Instance.GetUserByUsername(username);

			if (user != null)
			{
                if (PasswordPolicy.ValidatePassword(newPassword))
                {
                    int newPass = newPassword.GetHashCode();

                    if (PasswordPolicy.CanResetPassword(username, newPass))
                    {
                        //uvecati staru za 1
                        if (!DBManager.Instance.DeleteUser(user))
                        {
                            SecurityException ex = new SecurityException("There was a conflict. Someone else is changing informations about this user.Trying to make things right...");
                            throw ex;
                        }
                        user.Password = newPass;
                        PasswordHistoryModel model = new PasswordHistoryModel();
                        model.Username = user.Username;
                        model.Password = user.Password;
                        DBManager.Instance.AddPasswordToHistory(model);
                        if (!DBManager.Instance.AddUser(user))
                        {
                            SecurityException ex = new SecurityException("There was a conflict. Someone else is changing informations about this user.");
                            throw ex;
                        }
                    }
                    else
                    {
                        SecurityException ex = new SecurityException("These password used to many times");
                        throw ex;
                    }

                }
                else
                {
                    SecurityException ex = new SecurityException("Password must contains at least one upper case,one lower case, one number and one special char.");
                    throw ex;
                }
            }
			else
			{
				SecurityException ex = new SecurityException("User with this username does not exist.");
				throw ex;
			}
        }

    }
}
