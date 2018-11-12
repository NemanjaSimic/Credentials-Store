using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using System.Data.Entity;

namespace Datebase
{
	class DataBaseContextHistory : DbContext
	{
		public DataBaseContextHistory() : base("PasswordHistory")
		{
			Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataBaseContext, Config>());

		}

		public DbSet<PasswordHistoryModel> PasswordHistory { get; set; }
	}
}
