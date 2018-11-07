using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Datebase;
using System.ServiceModel;

namespace AuthenticationService
{
	public class AuthenticationService : ChannelFactory<ICredentialCheck>,IAuthenticationService
	{
        ICredentialCheck factory;
        Dictionary<string, ILogoutNotification> loggedUsers = new Dictionary<string, ILogoutNotification>();

        public AuthenticationService(NetTcpBinding binding, string address): base(binding, address)
        {
            factory = this.CreateChannel();
        }
		public void Login(string username, int password)
		{
            try
            {
                factory.ValidateCredential(username, password);
                ILogoutNotification CallbackService = OperationContext.Current.GetCallbackChannel<ILogoutNotification>();
                loggedUsers.Add(username,CallbackService);
            }
            catch (SecurityException ex)
            {
                throw ex;
            }
		}

		public void Logout(string username)
		{
            if (loggedUsers.ContainsKey(username))
            {
                loggedUsers.Remove(username);
            }
		}

	}
}
