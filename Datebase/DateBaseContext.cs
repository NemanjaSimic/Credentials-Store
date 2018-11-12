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
        public DataBaseContext() : base("CredentialsStore")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataBaseContext, Config>());

        }

        public  DbSet<User> CredentialsStore { get; set; }
    }
}
