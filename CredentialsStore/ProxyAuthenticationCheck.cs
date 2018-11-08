using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace CredentialsStore
{
	public class ProxyAuthenticationCheck : ChannelFactory<IAuthenticationCheck>, IAuthenticationCheck
	{
		IAuthenticationCheck factory;

		public ProxyAuthenticationCheck(NetTcpBinding binding, EndpointAddress address) : base(binding,address)
		{
			this.factory = CreateChannel();
		}

        public List<string> GetAllLoggedUsers()
        {
            return factory.GetAllLoggedUsers();
        }

        public bool IsAuthenticated(string username)
		{
			return factory.IsAuthenticated(username);
		}

        public void NotifyClientsAndLogOut(string username)
        {
            factory.NotifyClientsAndLogOut(username);
        }
    }
}
