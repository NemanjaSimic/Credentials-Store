﻿using System;
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
	public class AuthenticationService : IAuthenticationService, IAuthenticationCheck
	{
		private Dictionary<string, ILogoutNotification> loggedUsers = new Dictionary<string, ILogoutNotification>();
		private NetTcpBinding binding = new NetTcpBinding();
		private string address = "net.tcp://localhost:9997/CredentialCheck";
		public void Login(string username, int password)
		{
			using (ProxyCredentialsStore proxy = new ProxyCredentialsStore(binding, new EndpointAddress(new Uri(address))))
			{
				try
				{
					proxy.ValidateCredential(username, password);
					ILogoutNotification CallbackService = OperationContext.Current.GetCallbackChannel<ILogoutNotification>();
					loggedUsers.Add(username, CallbackService);
				}
				catch (SecurityException ex)
				{
					throw ex;
				}
			}
            
		}

		public void Logout(string username)
		{
			if (loggedUsers.ContainsKey(username))
			{
				loggedUsers.Remove(username);
			}
			else
			{
				SecurityException ex = new SecurityException("User already logged out");
				throw ex;
			}
		}

		public bool IsAuthenticated(string username)
		{
			return loggedUsers.ContainsKey(username);
		}
	}
}