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
		[FaultContract(typeof(SecurityException))]
        void CreateAccount(string username, SecureString password);
        [OperationContract]
		[FaultContract(typeof(SecurityException))]
		void DeleteAccount(string username);
        [OperationContract]
		[FaultContract(typeof(SecurityException))]
		void ResetPassword(string username, SecureString password);
    }
}
