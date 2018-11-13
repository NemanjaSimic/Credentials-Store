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
		[FaultContract(typeof(CredentialsException))]
		void Login(string username,int password);
		[OperationContract]
		[FaultContract(typeof(CredentialsException))]
		void Logout(string username);

	}
}
