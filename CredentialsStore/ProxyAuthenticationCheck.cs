using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Manager;

namespace CredentialsStore
{
	public class ProxyAuthenticationCheck : ChannelFactory<IAuthenticationCheck>, IAuthenticationCheck
	{
		IAuthenticationCheck factory;

		public ProxyAuthenticationCheck(NetTcpBinding binding, EndpointAddress address) : base(binding,address)
		{
			string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

			this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
			this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
			this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);

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

        public void NotifyClientsAndLogOut(string username,string message)
        {
            factory.NotifyClientsAndLogOut(username,message);
        }
    }
}
