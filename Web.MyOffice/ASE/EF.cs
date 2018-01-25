using ASE.MVC;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;

namespace ASE.EF
{
    public class AutoMigrationInit<TContext> : IDatabaseInitializer<TContext> where TContext : System.Data.Entity.DbContext
    {
        public AutoMigrationInit()
        {
        }

        public void InitializeDatabase(TContext context)
        {
            var configuration = new DbMigrationsConfiguration<TContext>();
            configuration.AutomaticMigrationDataLossAllowed = true;
            configuration.AutomaticMigrationsEnabled = true;
            if (!String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["TablePrefix"]))
            {
                configuration.ContextKey = System.Configuration.ConfigurationManager.AppSettings["TablePrefix"] + "_" + configuration.ContextKey;
            }
            var migrator = new DbMigrator(configuration);
            migrator.Update();
        }
    }
}
