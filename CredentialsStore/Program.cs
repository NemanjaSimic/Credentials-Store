using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Contracts;
using System.ServiceModel.Description;
using System.IdentityModel.Policy;
using Security.cs;
using System.Threading;
using Datebase;
using System.Security;
using Security;
using System.ServiceModel.Security;
using Manager;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Configuration;

namespace CredentialsStore
{
    class Program
    {

        static void Main(string[] args)
        {
            Thread thread1 = new Thread(PasswordAlarm);

            ServiceHost hostAccountManagement = new ServiceHost(typeof(AccountManagement));
			OpenAccountManagementHost(hostAccountManagement);

			ServiceHost hostCredentialCheck = new ServiceHost(typeof(CredentialCheck));
			OpenCredentialCheckHost(hostCredentialCheck);

			thread1.Start();

            Console.ReadLine();

			hostAccountManagement.Close();
        }

		static void OpenAccountManagementHost(ServiceHost host)
		{
			NetTcpBinding binding = new NetTcpBinding();

			binding.Security.Mode = SecurityMode.Transport;
			binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
			binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;

			string CredentialStoreIp = ConfigurationManager.AppSettings["CredentialStoreIp"];
			string address = $"net.tcp://{CredentialStoreIp}:5555/AccountManagement";
		
			host.AddServiceEndpoint(typeof(IAccountManagement), binding, address);
			host.AddServiceEndpoint(typeof(IUserAccountManagement), binding, address);
			host.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
			host.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });
			host.Authorization.ServiceAuthorizationManager = new AuthorizationManagerAccountManagement();

			List<IAuthorizationPolicy> policies = new List<IAuthorizationPolicy>
			{
				new CustomAuthorizationPolicy()
			};
			host.Authorization.ExternalAuthorizationPolicies = policies.AsReadOnly();
			host.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.Custom;
			host.Open();
			Console.WriteLine("CredentialsStore AccountManagement service opened...");
		}
	

		static void OpenCredentialCheckHost(ServiceHost host)
		{
			string srvCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
			string CredentialStoreIp = ConfigurationManager.AppSettings["CredentialStoreIp"];
			string address = $"net.tcp://{CredentialStoreIp}:9997/CredentialCheck";


			NetTcpBinding binding = new NetTcpBinding()
			{
				CloseTimeout = new TimeSpan(0, 60, 0),
				OpenTimeout = new TimeSpan(0, 60, 0),
				ReceiveTimeout = new TimeSpan(0, 60, 0),
				SendTimeout = new TimeSpan(0, 60, 0),
			};
			binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

			host.AddServiceEndpoint(typeof(ICredentialCheck), binding, address);
			host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
			host.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
			host.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);
			
			host.Open();
			Console.WriteLine("CredentialsStore CredetialCheck service opened...");

		}
        static void PasswordAlarm()
        {
			string srvCertCN = "authservice";
			NetTcpBinding binding = new NetTcpBinding()
			{
				CloseTimeout = new TimeSpan(0, 60, 0),
				OpenTimeout = new TimeSpan(0, 60, 0),
				ReceiveTimeout = new TimeSpan(0, 60, 0),
				SendTimeout = new TimeSpan(0, 60, 0),
			};
			binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
			X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);
			string AuthenticationServiceIp = ConfigurationManager.AppSettings["AuthenticationServiceIp"];
			EndpointAddress address = new EndpointAddress(new Uri($"net.tcp://{AuthenticationServiceIp}:9995/AuthenticationService"),
									  new X509CertificateEndpointIdentity(srvCert));

			using (ProxyAuthenticationCheck proxy = new ProxyAuthenticationCheck(binding, address))
			{
				while (true)
				{
					try
					{
						List<string> loggedUsers = proxy.GetAllLoggedUsers();
						foreach (string user in loggedUsers)
						{
							if (PasswordPolicy.IsPasswordOut(DBManager.Instance.GetUserByUsername(user)))
							{
								proxy.NotifyClientsAndLogOut(user, "Your password has been expired.Please conntact admin.You will be logged out...");
							}
						}
					}
					catch (Exception)
					{

					}
					Thread.Sleep(PasswordPolicy.GetPeriodForPasswordCheck());
				}
			}
            
        }
	}
}
