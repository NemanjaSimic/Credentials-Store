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
	public class ProxyAccountManagement : ChannelFactory<IAccountManagement>, IAccountManagement
	{
		IAccountManagement factory;

		public ProxyAccountManagement(NetTcpBinding binding,EndpointAddress address) : base (binding,address)
		{
			this.factory = CreateChannel();
		}
		public bool CreateAccount(string username, int password)
		{
			return factory.CreateAccount(username, password);
		}

		public bool DeleteAccount(string username)
		{
			return factory.DeleteAccount(username);
		}

		public bool ResetPassword(string username, int password)
		{
			return factory.ResetPassword(username, password);
		}
	}
}
