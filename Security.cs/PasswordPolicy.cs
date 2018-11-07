using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Security
{
    public class PasswordPolicy
    {
        public static bool ValidatePassword(SecureString password)
        {
            string patternPassword = @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{4,8}$";
            if (!string.IsNullOrEmpty(password.ToString()))
            {
                if (!Regex.IsMatch(password.ToString(), patternPassword))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool CanResetPassword(User user, int password, int n)
        {
            bool retVal = false;
            foreach (var pass in user.PasswordHistory)
            {
                if (pass.Key == password && pass.Value < n)
                {
                    retVal = true;
                    break;
                }
            }

            return retVal;
        }

        public static bool IsPasswordOut(User user, int password, int time)
        {
            if (user.Password == password && DateTime.Now.Subtract(user.PasswordInitialized).Seconds > time)
                return false;
            else
                return true;
        }
    }
}
