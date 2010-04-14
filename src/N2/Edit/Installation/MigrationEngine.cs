using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;

namespace N2.Edit.Installation
{
	[Service]
	public class MigrationEngine
	{
		IServiceContainer container;
		InstallationManager installer;

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
			foreach (var service in GetMigrations(initialStatus))
			{
				var result = service.Migrate(initialStatus);
				results.Add(result);
			}
			return results;
		}

		public virtual IEnumerable<AbstractMigration> GetMigrations(DatabaseStatus initialStatus)
		{
			return container.ResolveAll<AbstractMigration>()
				.Where(m => m.IsApplicable(initialStatus));
		}
	}
}
