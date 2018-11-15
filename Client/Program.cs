using Contracts;
using Security.cs;
using System;
using System.Configuration;
using System.Security;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Security;

namespace Client
{
	class Program
    {
        static void Main(string[] args)
        {
			while (true)
			{
				char op;
				do
				{
					//ClearConsoleBuffer();
					Console.WriteLine("Connect to service like:");
					Console.WriteLine("1.Admin");
					Console.WriteLine("2.Regular user");
					op = (char)Console.Read();
				} while (op != '1' && op != '2');

				if (op == '1')
				{
					NetTcpBinding binding = MakeBinding();
					string CredentialStoreIp = ConfigurationManager.AppSettings["CredentialStoreIp"];
				
					string address = $"net.tcp://{CredentialStoreIp}:5555/AccountManagement";
					using (ProxyAccountManagement proxy = new ProxyAccountManagement(binding, new EndpointAddress(new Uri(address))))
					{
						bool exit = false;
						while (!exit)
						{
							switch (AdminMenu())
							{
								case '1':
									ClearConsoleBuffer();
									Console.WriteLine("Enter username:");
									string username = Console.ReadLine();
									string password = EnterPasswordString();
									proxy.CreateAccount(username,password);							
									break;
								case '2':
									ClearConsoleBuffer();
									Console.WriteLine("Enter username:");
									string usernameForDelete = Console.ReadLine();
									proxy.DeleteAccount(usernameForDelete);
									break;
								case '3':
									ClearConsoleBuffer();
									Console.WriteLine("Enter username:");
									string usernameForReset = Console.ReadLine();
									string passwordForReset = EnterPasswordString();
									proxy.ResetPassword(usernameForReset,passwordForReset);
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
					ClearConsoleBuffer();
					NetTcpBinding bindingForAuthenticationService = MakeBinding();
					string AuthenticationServiceIp = ConfigurationManager.AppSettings["AuthenticationServiceIp"];
					
					string addressForAuthenticationService = $"net.tcp://{AuthenticationServiceIp}:9996/AuthenticationService";
					using (ProxyAuthenticationService proxyAuthSrvc = new ProxyAuthenticationService(bindingForAuthenticationService, new EndpointAddress(new Uri(addressForAuthenticationService))))
					{
						NetTcpBinding bindingForAccountManagement = MakeBinding();
						string CredentialStoreIp = ConfigurationManager.AppSettings["CredentialStoreIp"];

						string addressForUserAccountManagement = $"net.tcp://{CredentialStoreIp}:5555/AccountManagement";					
						using (ProxyUserAccountManagement proxyAccMngmnt = new ProxyUserAccountManagement(bindingForAccountManagement, new EndpointAddress(new Uri(addressForUserAccountManagement))))
						{
							bool exit = false;						
							while (!exit)
							{
								switch (RegularUserMenu())
								{
									case '1':
										ClearConsoleBuffer();
										Console.WriteLine("Enter your username:");
										string username = Console.ReadLine();

										SecureString password = EnterPassword();
										int pass = SecureConverter.Encrypt(password);
										
										proxyAuthSrvc.Login(username,pass);
										break;
									case '2':
										ClearConsoleBuffer();

										Console.WriteLine("Enter your username:");
										string usernameForLogout = Console.ReadLine();

										proxyAuthSrvc.Logout(usernameForLogout);
										break;
									case '3':
										ClearConsoleBuffer();

										Console.WriteLine("Enter your username:");
										string usernameForReset = Console.ReadLine();

										Console.WriteLine("First we need your current password");
										string oldPassword = EnterPasswordString();

										Console.WriteLine("Now you can pick new password");
										string newPassword = EnterPasswordString();

										proxyAccMngmnt.ResetPassword(usernameForReset,oldPassword,newPassword);
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

		}
		static char AdminMenu()
		{
			char answ;
			do
			{
				//Console.Clear();
				ClearConsoleBuffer();
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
				ClearConsoleBuffer();
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
			return binding;
		}

		static SecureString EnterPassword()
		{
			SecureString securePwd = new SecureString();
			ClearConsoleBuffer();
			Console.Write("Enter password: ");

			ConsoleKeyInfo key = Console.ReadKey(true);
			securePwd.AppendChar(key.KeyChar);
			int hash = securePwd.GetHashCode();
			Console.Write("*");
			while (key.Key != ConsoleKey.Enter)
			{
				key = Console.ReadKey(true);

				// Append the character to the password.
				securePwd.AppendChar(key.KeyChar);
				hash = securePwd.GetHashCode();
				if (key.Key != ConsoleKey.Enter)
				{
					Console.Write("*");
				}
				// Exit if Enter key is pressed.
			}
			securePwd.RemoveAt(securePwd.Length - 1);
			return securePwd;
		}
		static string EnterPasswordString()
		{
			Console.WriteLine("Enter password:");
			return Console.ReadLine();
		}

		static void EnterCredentials(string username, SecureString password)
		{
			Console.WriteLine("Enter your username:");
			username = Console.ReadLine();
			password = EnterPassword();
		}

		static void ClearConsoleBuffer()
		{
			while (Console.In.Peek() != -1)
				Console.In.Read();
		}
	}
}
