using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Migrations;

namespace Datebase
{
     class Config : DbMigrationsConfiguration<DataBaseContext>
    {
        public Config()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "DatabaseContext";
        }
    }
}
