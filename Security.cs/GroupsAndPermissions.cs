using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.cs
{
	public class GroupsAndPermissions
	{
		private Dictionary<string, List<string>> groupsPermissions;

		private static GroupsAndPermissions instance;
		public static GroupsAndPermissions Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new GroupsAndPermissions();
				}
				return instance;
			}
		}

		private GroupsAndPermissions()
		{
			groupsPermissions = new Dictionary<string, List<string>>();
			var path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory.ToString(), "groupsAndPermissions.txt");
			string text = File.ReadAllText(path);
			string[] groupsPerms = text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
			foreach (var groupPerms in groupsPerms)
			{
				string group = groupPerms.Split('-').First().Trim();
				string[] perms = groupPerms.Split('-').Last().Split(',');
				List<string> permissions = new List<string>();
				foreach (var item in perms)
				{
					permissions.Add(item);
				}
				groupsPermissions.Add(group, permissions);
			}
		}

		public List<string> GetPerms(string group)
		{
			groupsPermissions.TryGetValue(group, out List<string> retPerms);	
			return retPerms;
		}

	}
}
