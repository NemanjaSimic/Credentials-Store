using Contracts;
using Datebase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Security
{
    public class PasswordPolicy
    {
		private static bool complexPass;
		private static int expireTimeSeconds;
		private static int numberOrRepeats;

		private static void LoadRules()
		{
			var path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory.ToString(), "passwordPolicy.txt");
			string text = null;
			string[] rules = null;

			try
			{
				text = File.ReadAllText(path);
				rules = text.Split(';');

				if (rules[0].Equals("yes"))
				{
					complexPass = true;
				}
				else
				{
					complexPass = false;
				}

				if(!Int32.TryParse(rules[1], out expireTimeSeconds))
					expireTimeSeconds = 300;

				if (!Int32.TryParse(rules[2], out numberOrRepeats))
					numberOrRepeats = 3;
			}
			catch (Exception e)
			{
				Console.WriteLine("Error while reading password policy rule from file.{0}", e.Message);
				Console.WriteLine("Deafult values will be aplied.");

				complexPass = true;
				expireTimeSeconds = 300;
				numberOrRepeats = 3;
			}
		}

        public static bool ValidatePassword(SecureString password)
        {
			LoadRules();
			bool retVal = true;

			if (complexPass)
			{
				if (!string.IsNullOrEmpty(password.ToString()))
				{
					if (password.ToString().Length < 7)
						retVal = false;
					if (!Regex.Match(password.ToString(), @"/\d+/", RegexOptions.ECMAScript).Success)
						retVal = false;
					if (!Regex.Match(password.ToString(), @"/[a-z]/", RegexOptions.ECMAScript).Success ||
					  !Regex.Match(password.ToString(), @"/[A-Z]/", RegexOptions.ECMAScript).Success)
						retVal = false;
					if (!Regex.Match(password.ToString(), @"/.[!,@,#,$,%,^,&,*,?,_,~,-,£,(,)]/", RegexOptions.ECMAScript).Success)
						retVal = false;
				}
				//string patternPassword = @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{4,8}$";
				//if (!string.IsNullOrEmpty(password.ToString()))
				//{
				//	if (!Regex.IsMatch(password.ToString(), patternPassword))
				//	{
				//		retVal = false;
				//	}
				//}
			}

            return retVal;
        }

        public static bool CanResetPassword(string username, string password)
        {
			LoadRules();
            bool retVal = false;

			if (DBManager.Instance.GetNumberOfPassRepeat(username,password) < numberOrRepeats)
			{
				retVal = true;
			}

            return retVal;
        }

        public static bool IsPasswordOut(User user)
        {
			LoadRules();
			bool retVal = false;

			var period = DateTime.Now - user.PasswordInitialized;
			int seconds = period.Days * 86400 + period.Hours * 3600 + period.Minutes * 60 + period.Seconds;
			if (seconds > expireTimeSeconds)
			{
				retVal = true;
			}			

			return retVal;
        }
    }
}
