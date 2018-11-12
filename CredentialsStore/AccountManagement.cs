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
				byte[] data = System.Text.Encoding.ASCII.GetBytes(password.ToString());
				data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
				string hash = System.Text.Encoding.ASCII.GetString(data);
				if (DBManager.Instance.AddUser(new User(username, hash)))
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
					byte[] data = System.Text.Encoding.ASCII.GetBytes(newPassword.ToString());
					data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
					String hash = System.Text.Encoding.ASCII.GetString(data);

					if (PasswordPolicy.CanResetPassword(username,hash))
                    {
                        //uvecati staru za 1
                        if (!DBManager.Instance.DeleteUser(user))
                        {
                            SecurityException ex = new SecurityException("There was a conflict. Someone else is changing informations about this user.Trying to make things right...");
                            throw ex;
                        }
                        user.Password = hash;
                        PasswordHistoryModel model = new PasswordHistoryModel();
                        model.Username = user.Username;
						model.Password = hash;
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
