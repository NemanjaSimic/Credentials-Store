using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace Client
{
	public class ProxyUserAccountManagement : ChannelFactory<IUserAccountManagement>, IUserAccountManagement , IDisposable
	{
		IUserAccountManagement factory;
		public ProxyUserAccountManagement(NetTcpBinding binding,EndpointAddress address) : base (binding, address)
		{
			this.factory = CreateChannel();
		}
		public void ResetPassword(string username, string oldPassword, string newPassword)
		{
            try
            {
                factory.ResetPassword(username, oldPassword, newPassword);
				Console.WriteLine("User reset password successfully!");
            }
            catch(Exception ex)
            {
				Console.WriteLine("Error while trying to reset password.{0}", ex.Message);
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
