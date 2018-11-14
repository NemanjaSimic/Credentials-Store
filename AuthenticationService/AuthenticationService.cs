using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Datebase;
using System.ServiceModel;
using Security.cs;

namespace AuthenticationService
{
	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
	public class AuthenticationService : IAuthenticationService, IAuthenticationCheck
	{
		private static Dictionary<string, ILogoutNotification> loggedUsers = new Dictionary<string, ILogoutNotification>();
		private NetTcpBinding binding = new NetTcpBinding();
		private string address = "net.tcp://localhost:9997/CredentialCheck";

		public void Login(string username, int password)
		{
			CustomPrincipal principal = OperationContext.Current.ServiceSecurityContext.AuthorizationContext.Properties["Principal"] as CustomPrincipal;
			string name = principal.Identity.Name.Split('\\').Last();
			if (name.Equals(username))
			{
				if (principal.IsInRole("Login"))
				{
					if (!loggedUsers.ContainsKey(username))
					{
						using (ProxyCredentialsStore proxy = new ProxyCredentialsStore(binding, new EndpointAddress(new Uri(address))))
						{
							try
							{
								proxy.ValidateCredential(username, password);
								ILogoutNotification CallbackService = OperationContext.Current.GetCallbackChannel<ILogoutNotification>();
								loggedUsers.Add(username, CallbackService);
								Console.WriteLine("User {0} successfully logged in!");
							}
							catch (Exception e)
							{
								CredentialsException ex = new CredentialsException();
								ex.Reason = e.Message;
								throw new FaultException<CredentialsException>(ex, new FaultReason(e.Message));
							}
						}
					}
					else
					{
						CredentialsException ex = new CredentialsException();
						ex.Reason = "User is already logged in.";
						throw new FaultException<CredentialsException>(ex, new FaultReason("User is already logged in."));
					}
				}
				else
				{
					CredentialsException ex = new CredentialsException();
					ex.Reason = "You are not authorized for this action.";
					throw new FaultException<CredentialsException>(ex, new FaultReason("You are not authorized for this action."));
				}
			}
			else
			{
				CredentialsException ex = new CredentialsException();
				ex.Reason = "Your username does not match windows account username.";
				throw new FaultException<CredentialsException>(ex, new FaultReason("Your username does not match windows account username."));
			}
		}

		public void Logout(string username)
		{
			CustomPrincipal principal = OperationContext.Current.ServiceSecurityContext.AuthorizationContext.Properties["Principal"] as CustomPrincipal;
			if (principal.IsInRole("Logout"))
			{
				if (loggedUsers.ContainsKey(username))
				{
					loggedUsers.Remove(username);
					Console.WriteLine("User {0} successfully logged out!");
				}
				else
				{
					CredentialsException ex = new CredentialsException();
					ex.Reason = "User already logged out.";
					throw new FaultException<CredentialsException>(ex, new FaultReason("User already logged out."));
				}
			}
			else
			{
				CredentialsException ex = new CredentialsException();
				ex.Reason = "You are not authorized for this action.";
				throw new FaultException<CredentialsException>(ex, new FaultReason("You are not authorized for this action."));
			}
		}

		public bool IsAuthenticated(string username)
		{
			return loggedUsers.ContainsKey(username);
		}
        public void NotifyClientsAndLogOut(string username,string message)
        {
            if (loggedUsers.ContainsKey(username))
            {
                loggedUsers[username].NotifyClient(message);
                Logout(username);
            }
            else
            {
				CredentialsException ex = new CredentialsException();
				ex.Reason = "Client is already logged out.";
				throw new FaultException<CredentialsException>(ex, new FaultReason("Client is already logged out."));				
            }
        }

        public List<string> GetAllLoggedUsers()
        {
            return loggedUsers.Keys.ToList();
        }
    }
}
