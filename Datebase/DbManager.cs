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
				if (!UserExists(newUser))
				{
					
					dbContext.CredentialsStore.Add(newUser);
					retVal = true;
					dbContext.SaveChanges();
				}

			}
            return retVal;
        }

		public bool UserExists(User user)
		{
			bool retVal = false;
			using (var dbContext = new DataBaseContext())
			{			
				foreach (var item in dbContext.CredentialsStore)
				{
					if (item.Username.Equals(user.Username))
					{
						retVal = true;
					}
				}
				

			}
			return retVal;
		}

        public bool DeleteUser(User user)
        {
            bool retVal = false;
			using (var dbContext = new DataBaseContext())
			{
				if (UserExists(user))
				{
					var userDel = dbContext.CredentialsStore.Single(u => u.Username.Equals(user.Username));
					dbContext.CredentialsStore.Remove(userDel);
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
				foreach (User user in dbContext.CredentialsStore)
				{
					if (user.Username.Equals(username))
					{
						retUser = user;
					}
				}
			}
            return retUser;
        }
        public bool AddPasswordToHistory(PasswordHistoryModel passModel)
        {
            bool retVal = false;
            using(var dbContext = new DataBaseContextHistory())
            {
                    dbContext.PasswordHistory.Add(passModel);
                    retVal = true;
                    dbContext.SaveChanges();
            }
            return retVal;
        }

		public int GetNumberOfPassRepeat(string username, string pass)
		{
			int number = 0;
			using (var dbContext = new DataBaseContextHistory())
			{
				foreach (var item in dbContext.PasswordHistory)
				{
					if (item.Username.Equals(username) && item.Password.Equals(pass))
					{
						number++;
					}
				}
			}
			return number;
		}
		//public bool DoesPasswordExistInPasswordHistory(string username, int pass)
		//{
		//	bool retVal = false;
		//	using (var dbContext = new DataBaseContextHistory())
		//	{
		//		foreach (var item in dbContext.PasswordHistory)
		//		{
		//			if (item.Username.Equals(username) && item.Password.Equals(pass))
		//			{
		//				retVal = true;
		//				break;
		//			}
		//		}
		//	}
		//	return retVal;
		//}

	}
}
