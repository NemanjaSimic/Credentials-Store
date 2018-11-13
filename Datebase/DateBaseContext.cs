using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using System.Data.Entity;

namespace Datebase
{
    class DataBaseContext : DbContext
    {
        public DataBaseContext() : base("Credentials")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataBaseContext, Config>());

        }

        public  DbSet<User> Credentials { get; set; }
    }
}
