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
	public class ProxyUserAccountManagement : ChannelFactory<IUserAccountManagement>, IUserAccountManagement
	{
		IUserAccountManagement factory;
		public ProxyUserAccountManagement(NetTcpBinding binding,EndpointAddress address) : base (binding, address)
		{
			this.factory = CreateChannel();
		}
		public void ResetPassword(string username, SecureString oldPassword, SecureString newPassword)
		{
            try
            {
                factory.ResetPassword(username, oldPassword, newPassword);
            }
            catch(SecurityException ex)
            {
                throw ex;
            }
		}
	}
}
