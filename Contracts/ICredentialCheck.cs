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
    public interface ICredentialCheck
    {
        [OperationContract]
		[FaultContract(typeof(SecurityException))]
        void ValidateCredential(string username, int password);
    }
}
