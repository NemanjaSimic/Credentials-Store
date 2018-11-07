using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Security.cs
{
	public class AuthorizationManagerAccountManagement : ServiceAuthorizationManager
	{
		protected override bool CheckAccessCore(OperationContext operationContext)
		{
			bool authorizated = false;
			CustomPrincipal principal = operationContext.ServiceSecurityContext.AuthorizationContext.Properties["Principal"] as CustomPrincipal;

			if (principal.IsInRole("Administrate"))
			{
				authorizated = true;
			}
			return authorizated;
		}
	}
}
