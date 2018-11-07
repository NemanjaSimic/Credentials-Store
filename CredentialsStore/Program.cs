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

namespace CredentialsStore
{
    class Program
    {
        static void Main(string[] args)
        {
			ServiceHost hostAccountManagement = new ServiceHost(typeof(AccountManagement));
			OpenAccountManagementHost(hostAccountManagement);

			ServiceHost hostUserAccountManagement = new ServiceHost(typeof(UserAccountManagement));
			OpenUserAccountManagementHost(hostUserAccountManagement);

			ServiceHost hostCredentialCheck = new ServiceHost(typeof(CredentialCheck));
			OpenCredentialCheckHost(hostCredentialCheck);


			Console.ReadLine();

			hostAccountManagement.Close();
        }

		static NetTcpBinding MakeBinding()
		{
			NetTcpBinding binding = new NetTcpBinding();
			binding.Security.Mode = SecurityMode.Transport;
			binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
			binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
			return binding;
		}
		static void OpenAccountManagementHost(ServiceHost host)
		{
			NetTcpBinding binding = MakeBinding();
			string address = "net.tcp://localhost:9999/AccountManagement";
		
			host.AddServiceEndpoint(typeof(IAccountManagement), binding, address);
			host.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
			host.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });
			host.Authorization.ServiceAuthorizationManager = new AuthorizationManagerAccountManagement();

			List<IAuthorizationPolicy> policies = new List<IAuthorizationPolicy>
			{
				new CustomAuthorizationPolicy()
			};
			host.Authorization.ExternalAuthorizationPolicies = policies.AsReadOnly();
			host.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.Custom;
			Console.WriteLine("CredentialsStore AccountManagement service opened...");
		}

		static void OpenUserAccountManagementHost(ServiceHost host)
		{
			NetTcpBinding binding = MakeBinding();
			string address = "net.tcp://localhost:9998/UserAccountManagement";

			host.AddServiceEndpoint(typeof(IUserAccountManagement), binding, address);
			host.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
			host.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });
			Console.WriteLine("CredentialsStore UserAccountManagement service opened...");
		}

		static void OpenCredentialCheckHost(ServiceHost host)
		{
			NetTcpBinding binding = MakeBinding();
			string address = "net.tcp://localhost:9997/CredentialCheck";
			host.AddServiceEndpoint(typeof(ICredentialCheck), binding, address);		
			Console.WriteLine("CredentialsStore CredetialCheck service opened...");
		}
	}
}
