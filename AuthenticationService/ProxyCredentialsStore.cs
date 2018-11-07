using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService
{
	public class ProxyCredentialsStore : ChannelFactory<ICredentialCheck>, ICredentialCheck
	{
		ICredentialCheck factory;
		public ProxyCredentialsStore(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
		{
			factory = this.CreateChannel();
		}

		public void ValidateCredential(string username, int password)
		{
			try
			{
				factory.ValidateCredential(username, password);
			}
			catch (SecurityAccessDeniedException e)
			{
				//Console.WriteLine("Error while trying to validate credential. {0}", e.Message);
				throw e;
			}
		}
	}
}
