using System;
using System.Collections.Generic;
using System.Linq;
using N2.Engine;

namespace N2.Edit.Installation
{
    [Service]
    public class MigrationEngine
    {
        IServiceContainer container;
        InstallationManager installer;

	    public static void PostProgress(string progress)
	    {
			if (System.Web.HttpContext.Current != null)
			    System.Web.HttpContext.Current.Session["UpgradeEngineProgress"] = progress;
	    }

        public MigrationEngine(IServiceContainer container, InstallationManager installer)
        {
            this.container = container;
            this.installer = installer;
        }

        public virtual IEnumerable<MigrationResult> UpgradeAndMigrate()
        {
            DatabaseStatus initialStatus = installer.GetStatus();
            
            installer.Upgrade();

            return MigrateOnly(initialStatus);
        }

        public virtual IEnumerable<MigrationResult> MigrateOnly(DatabaseStatus initialStatus)
        {
            List<MigrationResult> results = new List<MigrationResult>();
	        var migs = GetMigrations(initialStatus).ToList();
            for (int i = 0; i < migs.Count; ++i)
            {
	            var service = migs[i];
	            PostProgress(String.Format("Migrating service {0} of {1} ('{2}')", i, migs.Count, service.Title));
                var result = service.Migrate(initialStatus);
                results.Add(result);
            }
            return results;
        }

        public virtual IEnumerable<AbstractMigration> GetAllMigrations()
        {
            return container.ResolveAll<AbstractMigration>();
        }

        public virtual IEnumerable<AbstractMigration> GetMigrations(DatabaseStatus initialStatus)
        {
            return GetAllMigrations().Where(m => m.TryApplicable(initialStatus) ?? true);
        }
    }
}
