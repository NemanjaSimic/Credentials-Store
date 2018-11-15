using Contracts;
using Manager;
using Security.cs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Policy;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;

namespace CredentialsStore
{
    class Program
    {
        static void Main(string[] args)
        {
			
			ServiceHost hostClient = new ServiceHost(typeof(AuthenticationService.AuthenticationService));
			OpenHostForClients(hostClient);

			ServiceHost hostCredentialStore = new ServiceHost(typeof(AuthenticationService.AuthenticationService));
			OpenHostForCredentialStore(hostCredentialStore);



			Console.ReadLine();

			hostClient.Close();
			hostCredentialStore.Close();
		}

		static void OpenHostForClients(ServiceHost host)
		{
			NetTcpBinding binding = new NetTcpBinding()
			{
				CloseTimeout = new TimeSpan(0, 60, 0),
				OpenTimeout = new TimeSpan(0, 60, 0),
				ReceiveTimeout = new TimeSpan(0, 60, 0),
				SendTimeout = new TimeSpan(0, 60, 0),
			};
			binding.Security.Mode = SecurityMode.Transport;
			binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
			binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;

			string AuthServiceIP = ConfigurationManager.AppSettings["AuthenticationServiceIp"];

			string address = $"net.tcp://{AuthServiceIP}:9996/AuthenticationService";
			host.AddServiceEndpoint(typeof(IAuthenticationService), binding, address);
			host.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
			host.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });
			host.Authorization.ServiceAuthorizationManager = new AuthorizationManagerAuthenticationService();
			List<IAuthorizationPolicy> policies = new List<IAuthorizationPolicy>
			{
				new CustomAuthorizationPolicy()
			};
			host.Authorization.ExternalAuthorizationPolicies = policies.AsReadOnly();
			host.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.Custom;

			host.Open();
			Console.WriteLine("Service host for clients opened...");
		}

		static void OpenHostForCredentialStore(ServiceHost host)
		{
			string srvCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

			string AuthServiceIP = ConfigurationManager.AppSettings["AuthenticationServiceIp"];
			string address = $"net.tcp://{AuthServiceIP}:9995/AuthenticationService";
			NetTcpBinding binding = new NetTcpBinding();
			binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

			
			host.AddServiceEndpoint(typeof(IAuthenticationCheck), binding, address);
			host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
			host.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
			host.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);


			host.Open();
			Console.WriteLine("Service host for credential store opened...");
		}
	}
}
