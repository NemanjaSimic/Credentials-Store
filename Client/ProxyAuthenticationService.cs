using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Security;
using Contracts;
using AuthenticationService;


namespace Client
{
	public class ProxyAuthenticationService : DuplexClientBase<IAuthenticationService>, IAuthenticationService
	{
		IAuthenticationService factory;
		public ProxyAuthenticationService(NetTcpBinding binding,EndpointAddress address) : base(new LogoutNotification(),binding,address)
		{
			this.factory = CreateChannel();
		}

		public void Login(string username, string password)
		{
			try
			{
				factory.Login(username, password);
				Console.WriteLine("User logged in successfully!");
			}
			catch (SecurityAccessDeniedException ex)
			{
				Console.WriteLine("Error while trying to login.{0}", ex.Message);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}

		public void Logout(string username)
		{
            try
            {
				factory.Logout(username);
				Console.WriteLine("User logged out successfully!");
            }
            catch(SecurityAccessDeniedException ex)
            {
				Console.WriteLine("Error while trying to logout.{0}", ex.Message);
			}
		}
	}
}
