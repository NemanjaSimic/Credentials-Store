using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace Client
{
	public class ProxyAccountManagement : ChannelFactory<IAccountManagement>, IAccountManagement
	{
		IAccountManagement factory;

		public ProxyAccountManagement(NetTcpBinding binding,EndpointAddress address) : base (binding,address)
		{
			this.factory = CreateChannel();
		}
		public void CreateAccount(string username, SecureString password)
		{
			try
			{
				factory.CreateAccount(username, password);
				Console.WriteLine("Account successfully created!");
			}
			catch (SecurityAccessDeniedException e)
			{
				Console.WriteLine("Error while trying to create account. {0}", e.Message);
			}
		}

		public void DeleteAccount(string username)
		{
			try
			{
				factory.DeleteAccount(username);
				Console.WriteLine("Account successfully deleted!");
			}
			catch (SecurityAccessDeniedException e)
			{
				Console.WriteLine("Error while trying to delete account. {0}", e.Message);
			}
		}

		public void ResetPassword(string username, SecureString password)
		{
			try
			{
				factory.ResetPassword(username, password);
				Console.WriteLine("Password successfully reset!");
			}
			catch (SecurityAccessDeniedException e)
			{
				Console.WriteLine("Error while trying to reset password. {0}", e.Message);
			}
		}
	}
}
