using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Datebase;

using Security;

namespace CredentialsStore
{
	public class UserAccountManagement : IUserAccountManagement
	{
		public void ResetPassword(string username, SecureString oldPassword, SecureString newPassword)
		{
            User user = DBManager.Instance.GetUserByUsername(username);
            int oldPass = oldPassword.GetHashCode();
            if (user != null)
            {
                if(user.Password.Equals(oldPass)){
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
                    SecurityException ex = new SecurityException("Bad old password");
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
