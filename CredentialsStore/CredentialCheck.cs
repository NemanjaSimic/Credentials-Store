using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using System.ServiceModel;
using Datebase;
using System.Security;

namespace CredentialsStore
{
    class CredentialCheck : ICredentialCheck
    {
        public void ValidateCredential(string username, string password)
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
                    SecurityException ex = new SecurityException("Wrong Password");
                    throw ex;
                }
            }
            else
            {
                SecurityException ex = new SecurityException("Username does not exist");
                throw ex;
            }
        }
    }
}
