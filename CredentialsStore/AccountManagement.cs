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
            
            User user = new User(username, password);
            if (DBManager.Instance.AddUser(user))
            {
                return true;
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
            User user = DBManager.Instance.GetUserByUsername(username);
            if (DBManager.Instance.ResetPassword(user, newPassword))
                return true;
            else
                return false;
        }
    }
}
