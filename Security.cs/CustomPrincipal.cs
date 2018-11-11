using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Security.cs
{
	public class CustomPrincipal : IPrincipal
	{
		IIdentity identity;
		HashSet<string> permissions;
		public IIdentity Identity => this.identity;
		public HashSet<string> Permissions => this.permissions;

		public CustomPrincipal(WindowsIdentity windowsIdentity)
		{
			this.identity = windowsIdentity;
			this.permissions = new HashSet<string>();
			foreach (var group in windowsIdentity.Groups)
			{
				SecurityIdentifier sid = (SecurityIdentifier)group.Translate(typeof(SecurityIdentifier));
				var name = sid.Translate(typeof(NTAccount));
				string groupName = name.Value.Split('\\').Last();
				List<string> perms = GroupsAndPermissions.Instance.GetPerms(groupName);
				if (perms != null)
				{
					foreach (var item in perms)
					{
						this.permissions.Add(item);
					}
				}
			}
		}

		public bool IsInRole(string role)
		{
			bool retVal = false;
			foreach (var item in Permissions)
			{
				if (item.Equals(role))
				{
					retVal = true;
				}
			}
			return retVal;
		}
	}
}
