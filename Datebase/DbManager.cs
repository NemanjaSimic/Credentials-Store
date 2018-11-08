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
			using (var dbContext = new DataBaseContext())
			{
				if (!dbContext.Credentials.Contains(newUser))
				{
					dbContext.Credentials.Add(newUser);
					retVal = true;
					dbContext.SaveChanges();
				}
			}
            return retVal;
        }

        public bool DeleteUser(User user)
        {
            bool retVal = false;
			using (var dbContext = new DataBaseContext())
			{
				if (dbContext.Credentials.Contains(user))
				{
					dbContext.Credentials.Remove(user);
					retVal = true;
					dbContext.SaveChanges();
				}
			}
            return retVal;
        }

        public User GetUserByUsername(string username)
        {
            User retUser = null;
			using (var dbContext = new DataBaseContext())
			{
				foreach (User user in dbContext.Credentials)
				{
					if (user.Username.Equals(username))
					{
						retUser = user;
					}
				}
			}
            return retUser;
        }

		public int GetNumberOfPassRepeat(string username, int pass)
		{
			int number = 0;
			using (var dbContext = new DataBaseContextHistory())
			{
				foreach (var item in dbContext.PasswordHistory)
				{
					if (item.Username.Equals(username) && item.Password == pass)
					{
						number++;
					}
				}
			}
			return number;
		}

    }
}
