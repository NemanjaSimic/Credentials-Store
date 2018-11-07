using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Datebase;

namespace AuthenticationService
{
	public class AuthenticationService : IAuthenticationService
	{
		public void Login(string username, int password)
		{
			User current = DBManager.Instance.GetUserByUsername(username);
			if (current != null)
			{
				if (!current.IsAuthenticated)
				{
					if (current.Password == password)
					{
						DBManager.Instance.DeleteUser(current);
						current.IsAuthenticated = true;
						DBManager.Instance.AddUser(current);
					}
					else
					{
						SecurityException ex = new SecurityException("Bad password!");
						throw ex;
					}
				}
				else
				{
					SecurityException ex = new SecurityException("User already logged in!");
					throw ex;
				}
			}
			else
			{
				SecurityException ex = new SecurityException("Username does not exist!");
				throw ex;
			}
		}

		public void Logout(string username)
		{
			User current = DBManager.Instance.GetUserByUsername(username);
			if (current != null)
			{
				if (current.IsAuthenticated)
				{
					DBManager.Instance.DeleteUser(current);
					current.IsAuthenticated = false;
					DBManager.Instance.AddUser(current);
				}
				else
				{
					SecurityException ex = new SecurityException("User already logged out!");
					throw ex;
				}
			}
			else
			{
				SecurityException ex = new SecurityException("Username does not exist!");
				throw ex;
			}
		}
	}
}
