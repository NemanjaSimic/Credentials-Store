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
        public void NotifyClient(string message)
        {
            Console.WriteLine(message);
        }
    }
}
