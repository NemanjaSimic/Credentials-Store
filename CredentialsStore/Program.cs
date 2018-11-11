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

namespace CredentialsStore
{
    class Program
    {

        static void Main(string[] args)
        {
            Thread thread1 = new Thread(PasswordAlarm);

            ServiceHost hostAccountManagement = new ServiceHost(typeof(AccountManagement));
			OpenAccountManagementHost(hostAccountManagement);

			ServiceHost hostUserAccountManagement = new ServiceHost(typeof(UserAccountManagement));
			OpenUserAccountManagementHost(hostUserAccountManagement);

			ServiceHost hostCredentialCheck = new ServiceHost(typeof(CredentialCheck));
			OpenCredentialCheckHost(hostCredentialCheck);

           // thread1.Start();

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
			string address = "net.tcp://localhost:5555/AccountManagement";
		
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
			host.Open();
			Console.WriteLine("CredentialsStore AccountManagement service opened...");
		}

		static void OpenUserAccountManagementHost(ServiceHost host)
		{
			NetTcpBinding binding = MakeBinding();
			string address = "net.tcp://localhost:9998/UserAccountManagement";

			host.AddServiceEndpoint(typeof(IUserAccountManagement), binding, address);
			host.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
			host.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });
			host.Open();
			Console.WriteLine("CredentialsStore UserAccountManagement service opened...");
		}

		static void OpenCredentialCheckHost(ServiceHost host)
		{
			NetTcpBinding binding = MakeBinding();
			string address = "net.tcp://localhost:9997/CredentialCheck";
			host.AddServiceEndpoint(typeof(ICredentialCheck), binding, address);
			host.Open();
			Console.WriteLine("CredentialsStore CredetialCheck service opened...");
		}
        static void PasswordAlarm()
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:9995/AuthenticationService";
            
                using (ProxyAuthenticationCheck proxy = new ProxyAuthenticationCheck(binding, new EndpointAddress(new Uri(address))))
                {
				while (true)
				{
					try
                    {
                        List<string> loggedUsers = proxy.GetAllLoggedUsers();
                        foreach(string user in loggedUsers)
                        {
                            if (PasswordPolicy.IsPasswordOut(DBManager.Instance.GetUserByUsername(user)))
                            {
                                proxy.NotifyClientsAndLogOut(user);
                            }
                        }
                    }
                    catch (SecurityException ex)
                    {
                        throw ex;
                    }
					Thread.Sleep(2250);
				}
			}
            
        }
	}
}
