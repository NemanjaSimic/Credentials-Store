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
        bool CreateAccount(string username, int password);
        [OperationContract]
        bool DeleteAccount(string username);
        [OperationContract]
        bool ResetPassword(string username, int password);
    }
}
