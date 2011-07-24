using System.Linq;
using N2.Edit.Installation;
using N2.Persistence;

namespace N2.Management.Installation
{
	[N2.Engine.Service(typeof(AbstractMigration))]
	public class CleanZoneNameMigration : AbstractMigration
	{
		IRepository<int, ContentItem> repository;

		public CleanZoneNameMigration(IRepository<int, ContentItem> repository)
		{
			this.repository = repository;

			Title = "Updates zone names from empty string to null";
			Description = "Setting the zone name to null is a performance improvement and is needed by certain functions to correctly find pages.";
		}

		public override bool IsApplicable(DatabaseStatus status)
		{
			return status.DatabaseVersion < 4 || repository.Find("ZoneName", "").Any();
		}

		public override MigrationResult Migrate(DatabaseStatus preSchemaUpdateStatus)
		{
			int updatedItems = 0;
			using (var transaction = repository.BeginTransaction())
			{
				foreach (var item in repository.Find("ZoneName", ""))
				{
					if (item.IsPage)
					{
						item.ZoneName = null;
						repository.Update(item);
						updatedItems++;
					}
				}

				transaction.Commit();
			}

			return new MigrationResult(this) { UpdatedItems = updatedItems };
		}
	}
}
