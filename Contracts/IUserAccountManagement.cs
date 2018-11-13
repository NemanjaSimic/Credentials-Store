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
    public interface IUserAccountManagement
    {
        [OperationContract]
		[FaultContract(typeof(CredentialsException))]
		void ResetPassword(string username,string oldPassword,string newPassword);
    }
}
