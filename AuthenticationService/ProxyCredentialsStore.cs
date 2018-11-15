using Contracts;
using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
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
			
			string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

			this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
			this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
			this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);


			factory = this.CreateChannel();
		}

		public void ValidateCredential(string username, int password)
		{
			try
			{
				factory.ValidateCredential(username, password);
			}
			catch (Exception e)
			{
				//Console.WriteLine("Error while trying to validate credential. {0}", e.Message);
				CredentialsException ex = new CredentialsException();
				ex.Reason = e.Message;
				throw new FaultException<CredentialsException>(ex, new FaultReason(e.Message));
			}
		}
	}
}
