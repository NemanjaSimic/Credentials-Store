﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Security.cs
{
	public class CustomAuthorizationPolicy : IAuthorizationPolicy
	{
		private string id;
		private object locker = new object();
		public CustomAuthorizationPolicy()
		{
			this.id = Guid.NewGuid().ToString();
		}
		public string Id
		{
			get { return this.id; }
		}

		public ClaimSet Issuer
		{
			get { return ClaimSet.System; }
		}

		public bool Evaluate(EvaluationContext evaluationContext, ref object state)
		{
			if (!evaluationContext.Properties.TryGetValue("Identities", out object list))
			{
				return false;
			}

			IList<IIdentity> identities = list as IList<IIdentity>;
			if (list == null || identities.Count <= 0)
			{
				return false;
			}
			evaluationContext.Properties["Principal"] = GetPrincipal(identities[0]);
			return true;
		}

		protected virtual IPrincipal GetPrincipal(IIdentity identity)
		{
			lock (locker)
			{
				IPrincipal principal = null;
				if (identity is WindowsIdentity windowsIdentity)
				{
					principal = new CustomPrincipal(windowsIdentity);
				}
				return principal;
			}
		}
	}
}
