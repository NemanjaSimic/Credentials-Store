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
	public class ProxyAccountManagement : ChannelFactory<IAccountManagement>, IAccountManagement, IDisposable
	{
		IAccountManagement factory;

		public ProxyAccountManagement(NetTcpBinding binding,EndpointAddress address) : base (binding,address)
		{
			this.factory = CreateChannel();
		}
		public void CreateAccount(string username, string password)
		{
			try
			{
				factory.CreateAccount(username, password);
				Console.WriteLine("Account successfully created!");
			}
			catch (Exception e)
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
			catch (FaultException<CredentialsException> ex)
			{
				Console.WriteLine("Error while trying to delete account.{0}", ex.Message);
			}
			catch (Exception e)
			{
				Console.WriteLine("Error while trying to delete account. {0}", e.Message);
			}
		}

		public void ResetPassword(string username, string password)
		{
			try
			{
				factory.ResetPassword(username, password);
				Console.WriteLine("Password successfully reset!");
			}
			catch (FaultException<CredentialsException> ex)
			{
				Console.WriteLine("Error while trying to reset password. {0}", ex.Message);
			}
			catch (Exception e)
			{
				Console.WriteLine("Error while trying to reset password. {0}", e.Message);
			}
		}

		public void Dispose()
		{
			if (factory != null)
			{
				factory = null;
			}

			this.Close();
		}

	}
}
