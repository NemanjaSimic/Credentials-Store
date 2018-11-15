using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using System.ServiceModel;
using Datebase;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using Manager;

namespace CredentialsStore
{
    class CredentialCheck : ICredentialCheck
    {
        public void ValidateCredential(string username, int password)
        {
			
			User user = DBManager.Instance.GetUserByUsername(username);
            if(user != null)
            {
                if(user.Password.Equals(password))
                {
                   // Console.WriteLine("User ulogovan");
                }
                else
                {
					CredentialsException ex = new CredentialsException();
					ex.Reason = "Wrong Password.";
					throw new FaultException<CredentialsException>(ex, new FaultReason("Wrong Password."));
                }
            }
            else
            {
				CredentialsException ex = new CredentialsException();
				ex.Reason = "Username does not exist.";
				throw new FaultException<CredentialsException>(ex, new FaultReason("Username does not exist."));
            }
        }
    }
}
