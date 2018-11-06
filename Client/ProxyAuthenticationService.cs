using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Contracts;


namespace Client
{
	public class ProxyAuthenticationService : ChannelFactory<IAuthenticationService>, IAuthenticationService
	{
		IAuthenticationService factory;
		public ProxyAuthenticationService(NetTcpBinding binding,EndpointAddress address) : base(binding,address)
		{
			this.factory = CreateChannel();
		}

		public void Login(string username, int password)
		{
			throw new NotImplementedException();
		}

		public void Logout(string username)
		{
			throw new NotImplementedException();
		}
	}
}
