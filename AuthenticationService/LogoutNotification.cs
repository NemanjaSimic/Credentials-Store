using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace AuthenticationService
{
    public class LogoutNotification : ILogoutNotification
    {
        public void NotifyClient()
        {
            Console.WriteLine("Your password has expired, you need to reset it. You will be logged out...");
        }
    }
}
