using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
	[ServiceContract(CallbackContract =typeof(ILogoutNotification))]
	public interface IAuthenticationService
	{
		[OperationContract]
		[FaultContract(typeof(SecurityException))]
		void Login(string username,string password);
		[OperationContract]
		[FaultContract(typeof(SecurityException))]
		void Logout(string username);

	}
}
