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

        public static bool ValidatePassword(string password)
        {
			LoadRules();
			bool retVal = false;

			if (complexPass)
			{
                //if (!string.IsNullOrEmpty(password.ToString()))
                //{
                //	if (password.Length < 7)
                //		retVal = false;
                //                if (!Regex.Match(password, @"/\d+/", RegexOptions.ECMAScript).Success)
                //                    retVal = false;
                //                if (!Regex.Match(password, @"/[a-z]/", RegexOptions.ECMAScript).Success &&
                //                  !Regex.Match(password, @"/[A-Z]/", RegexOptions.ECMAScript).Success)
                //                    retVal = false;
                //                if (!Regex.Match(password, @"/.[!,@,#,$,%,^,&,*,?,_,~,-,£,(,)]/", RegexOptions.ECMAScript).Success)
                //		retVal = false;
                //}
                //string patternPassword = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$";
                //if (!string.IsNullOrEmpty(password))
                //{
                //    if (Regex.IsMatch(password, patternPassword))
                //    {
                //        retVal = false;
                //    }
                //}
                var input = password;


                if (string.IsNullOrWhiteSpace(input))
                {
                    return false;
                }

                var hasNumber = new Regex(@"[0-9]+");
                var hasUpperChar = new Regex(@"[A-Z]+");
                var hasMiniMaxChars = new Regex(@".{8,15}");
                var hasLowerChar = new Regex(@"[a-z]+");
                var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

                if (!hasLowerChar.IsMatch(input))
                {
                    return false;
                }
                else if (!hasUpperChar.IsMatch(input))
                {
                    return false;
                }
                else if (!hasMiniMaxChars.IsMatch(input))
                {
                    return false;
                }
                else if (!hasNumber.IsMatch(input))
                {
                    return false;
                }

                else if (!hasSymbols.IsMatch(input))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return retVal;
        }

        public static bool CanResetPassword(string username, int password)
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
