using N2.Edit.Installation;
using N2.Persistence;

namespace N2.Management.Installation
{
    [N2.Engine.Service(typeof(AbstractMigration))]
    public class InsertAncestralTrailMigration : AbstractMigration
    {
        InstallationManager installer;
        IRepository<ContentItem> repository;

        public InsertAncestralTrailMigration(IRepository<ContentItem> repository, InstallationManager installer)
        {
            this.repository = repository;
            this.installer = installer;

            Title = "Update ancestral trail information";
            Description = "This allows to query the database for all items in a hierarchy.";
        }

        public override bool IsApplicable(DatabaseStatus status)
        {
            return status.DatabaseVersion < 3;
        }

        public override MigrationResult Migrate(DatabaseStatus preSchemaUpdateStatus)
        {
            int updatedItems = 0;
            using (var transaction = repository.BeginTransaction())
            {
                foreach (var item in installer.ExecuteQuery(NHInstallationManager.QueryItemsWithoutAncestralTrail))
                {
                    item.AncestralTrail = Utility.GetTrail(item.Parent);
                    repository.SaveOrUpdate(item);
                    updatedItems++;
                }

                transaction.Commit();
            }

            return new MigrationResult(this) { UpdatedItems = updatedItems };
        }
    }
}
