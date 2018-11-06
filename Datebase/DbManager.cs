using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace Datebase
{
    public class DBManager
    {
        private DBManager() { }
        private static DBManager instance;

        public static DBManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DBManager();
                }
                return instance;
            }
        }

        public bool AddUser(User newUser)
        {
            bool retVal = false;
            if (!DataBaseContext.Credentials.Contains(newUser))
            {
                DataBaseContext.Credentials.Add(newUser);
                retVal = true;
            }
            return retVal;
        }

        public bool DeleteUser(User user)
        {
            bool retVal = false;
            if (DataBaseContext.Credentials.Contains(user))
            {
                DataBaseContext.Credentials.Remove(user);
                retVal = true;
            }
            return retVal;
        }

        public bool ResetPassword(User user,int newPass)
        {
            bool retVal = false;
            if (DataBaseContext.Credentials.Contains(user))
            {
                DataBaseContext.Credentials.Remove(user);
                user.Password = newPass;
                DataBaseContext.Credentials.Add(user);
                retVal = true;
            }
            return retVal;
        }

        public User GetUserByUsername(string username)
        {
            User retUser = null;
            foreach (User user in DataBaseContext.Credentials)
            {
                if (user.Username.Equals(username))
                {
                    retUser = user;
                }
            }
            return retUser;
        }

    }
}
