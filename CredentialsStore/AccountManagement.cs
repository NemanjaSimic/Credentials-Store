using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using System.ServiceModel;
using System.Security;
using Datebase;

namespace CredentialsStore
{
    class AccountManagement : IAccountManagement
    {
        public bool CreateAccount(string username, SecureString password)
        {
            //proveriti sifru
            if (Security.PasswordPolicy.ValidatePassword(password))
            {
                int pass = password.GetHashCode();
                User user = new User(username, pass);
                if (DBManager.Instance.AddUser(user))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool DeleteAccount(string username)
        {
            User user = DBManager.Instance.GetUserByUsername(username);
            if(user != null && DBManager.Instance.DeleteUser(user))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ResetPassword(string username,SecureString newPassword)
        {
            //proveri sifru
            User user = DBManager.Instance.GetUserByUsername(username);
            if (Security.PasswordPolicy.ValidatePassword(newPassword))
            {
                int newPass = newPassword.GetHashCode();

                //provera da li sifra nalazi vec u korisnikovim siframa
                if (user.PasswordHistory.ContainsKey(newPass))
                {
                    DBManager.Instance.DeleteUser(user);
                    user.Password = newPass;
                    user.PasswordHistory[newPass]++;
                }
                else
                {
                    DBManager.Instance.DeleteUser(user);
                    user.PasswordHistory.Add(newPass, 1);
                    user.Password = newPass;
                }
                if (DBManager.Instance.AddUser(user))
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }
    }
}
