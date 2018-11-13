using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Security;

namespace Contracts
{
   [ServiceContract]
    public interface IAccountManagement
    {
        [OperationContract]
		[FaultContract(typeof(CredentialsException))]
        void CreateAccount(string username, string password);
        [OperationContract]
		[FaultContract(typeof(CredentialsException))]
		void DeleteAccount(string username);
        [OperationContract]
		[FaultContract(typeof(CredentialsException))]
		void ResetPassword(string username, string password);
    }
}
