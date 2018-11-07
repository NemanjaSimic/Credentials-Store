using Contracts;
using Security.cs;
using System;
using System.Collections.Generic;
using System.IdentityModel.Policy;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
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

		static NetTcpBinding MakeBinding()
		{
			NetTcpBinding binding = new NetTcpBinding();
			binding.Security.Mode = SecurityMode.Transport;
			binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
			binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
			return binding;
		}

		static void OpenHostForClients(ServiceHost host)
		{
			NetTcpBinding binding = MakeBinding();
			string address = "net.tcp://localhost:9996/AuthenticationService";
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
			NetTcpBinding binding = MakeBinding();
			string address = "net.tcp://localhost:9995/AuthenticationService";
			host.AddServiceEndpoint(typeof(IAuthenticationCheck), binding, address);
			host.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
			host.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });
			host.Open();
			Console.WriteLine("Service host for credential store opened...");
		}
	}
}
