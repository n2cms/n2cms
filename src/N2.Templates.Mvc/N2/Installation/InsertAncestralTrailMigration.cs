using N2.Edit.Installation;
using N2.Persistence;
using N2.Security;

namespace N2.Management.Installation
{
	[N2.Engine.Service(typeof(AbstractMigration))]
	public class InsertAncestralTrailMigration : AbstractMigration
	{
		InstallationManager installer;
		IRepository<int, ContentItem> repository;

		public InsertAncestralTrailMigration(IRepository<int, ContentItem> repository, InstallationManager installer)
		{
			this.repository = repository;
			this.installer = installer;

			Title = "Update ancestral trail information";
			Description = "This allows to query the database for all items in a hierarchy.";
		}

		public override bool IsApplicable(N2.Edit.Installation.DatabaseStatus status)
		{
			return status.DatabaseVersion < 3;
		}

		public override MigrationResult Migrate(DatabaseStatus preSchemaUpdateStatus)
		{
			int updatedItems = 0;
			using (var transaction = repository.BeginTransaction())
			{
				foreach (var item in installer.ExecuteQuery(InstallationManager.QueryItemsWithoutAncestralTrail))
				{
					item.AncestralTrail = Utility.GetTrail(item.Parent);
					repository.Update(item);
					updatedItems++;
				}

				transaction.Commit();
			}

			return new MigrationResult(this) { UpdatedItems = updatedItems };
		}
	}
}
