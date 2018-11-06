using System;
using System.Security;
using System.Security.Principal;
using System.ServiceModel;

namespace Client
{
	class Program
    {
        static void Main(string[] args)
        {				
			if (IsAdmin())
			{
				NetTcpBinding binding = MakeBinding();
				string address = "net.tcp://localhost:9999/AccountManagement";
				using (ProxyAccountManagement proxy = new ProxyAccountManagement(binding,new EndpointAddress(new Uri(address))))
				{
					bool exit = false;
					while (!exit)
					{
						switch (AdminMenu())
						{
							case '1':
								//proxy.CreateAccount();
								break;
							case '2':
								//proxy.DeleteAccount();
								break;
							case '3':
								//proxy.ResetPassword();
								break;
							default:
								exit = true;
								break;
						}
					}
				}
			}
			else
			{
				NetTcpBinding bindingForAuthenticationService = MakeBinding();
				string addressForAuthenticationService = "net.tcp://localhost:9999/AuthenticationService";
				using (ProxyAuthenticationService proxyAuthSrvc = new ProxyAuthenticationService(bindingForAuthenticationService, new EndpointAddress(new Uri(addressForAuthenticationService))))
				{
					NetTcpBinding bindingForAccountManagement = MakeBinding();
					string addressForUserAccountManagement = "net.tcp://localhost:9999/CredentialsStore";
					using (ProxyUserAccountManagement proxyAccMngmnt = new ProxyUserAccountManagement(bindingForAccountManagement, new EndpointAddress(new Uri(addressForUserAccountManagement))))
					{
						bool exit = false;
						string username;
						SecureString password;
						while (!exit)
						{
							switch (RegularUserMenu())
							{
								case '1':
									//proxyAuthSrvc.Login();
									break;
								case '2':
									//proxyAuthSrvc.Logout();
									break;
								case '3':
									//proxyAccMngmnt.ResetPassword();
									break;
								default:
									exit = true;
									break;
							}
						}
					}
				}
			}

		}
		static bool IsAdmin()
		{
			bool retVal = false;
			foreach (var group in WindowsIdentity.GetCurrent().Groups)
			{
				SecurityIdentifier sid = (SecurityIdentifier)group.Translate(typeof(SecurityIdentifier));
				var name = sid.Translate(typeof(NTAccount));
				if (name.Value.Contains("Admin"))
				{
					retVal = true;
				}
			}
			return retVal;
		}

		static char AdminMenu()
		{
			char answ;
			do
			{
				Console.WriteLine("Meni:");
				Console.WriteLine("1.Create Account");
				Console.WriteLine("2.Delete Account");
				Console.WriteLine("3.Reset password");
				Console.WriteLine("4.Exit");
				answ = (char)Console.Read();
			} while (answ != '1' && answ != '2' && answ != '3' && answ != '4');
			return answ;
		}

		static char RegularUserMenu()
		{
			char answ;
			do
			{
				Console.WriteLine("Meni:");
				Console.WriteLine("1.Log in");
				Console.WriteLine("2.Log out");
				Console.WriteLine("3.Reset password");
				Console.WriteLine("4.Exit");
				answ = (char)Console.Read();
			} while (answ != '1' && answ != '2' && answ != '3' && answ != '4');
			return answ;
		}

		static NetTcpBinding MakeBinding()
		{
			NetTcpBinding binding = new NetTcpBinding();
			binding.Security.Mode = SecurityMode.Transport;
			binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
			binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
			return binding;
		}
	}
}
