using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Contracts;


namespace Client
{
	public class ProxyAuthenticationService : ChannelFactory<IAuthenticationService>, IAuthenticationService
	{
		IAuthenticationService factory;
		public ProxyAuthenticationService(NetTcpBinding binding,EndpointAddress address) : base(binding,address)
		{
			this.factory = CreateChannel();
		}

		public void Login(string username, int password)
		{
            try
            {
                factory.Login(username, password);
				Console.WriteLine("User logged in successfully!");
            }
            catch(SecurityException ex)
            {
				Console.WriteLine("Error while trying to login.{0}",ex.Message);
            }
		}

		public void Logout(string username)
		{
            try
            {
				factory.Logout(username);
				Console.WriteLine("User logged out successfully!");
            }
            catch(SecurityException ex)
            {
				Console.WriteLine("Error while trying to logout.{0}", ex.Message);
			}
		}
	}
}
