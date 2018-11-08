using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace CredentialsStore
{
	public class UserAccountManagement : IUserAccountManagement
	{
		public bool ResetPassword(SecureString oldPassword, SecureString newPassword)
		{
			throw new NotImplementedException();
		}
	}
}
