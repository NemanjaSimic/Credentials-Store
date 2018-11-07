using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace AuthenticationService
{
    class LogoutNotification : ILogoutNotification
    {
        public void NotifyClient()
        {
            Console.WriteLine("Password Timeout, LOGOUT");
        }
    }
}
