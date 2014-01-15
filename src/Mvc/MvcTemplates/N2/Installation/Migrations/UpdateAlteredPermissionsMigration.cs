using N2.Edit.Installation;
using N2.Persistence;
using N2.Security;

namespace N2.Management.Installation
{
    [N2.Engine.Service(typeof(AbstractMigration))]
    public class UpdateAlteredPermissionsMigration : AbstractMigration
    {
        InstallationManager installer;
        IRepository<ContentItem> repository;

        public UpdateAlteredPermissionsMigration(IRepository<ContentItem> repository, InstallationManager installer)
        {
            this.repository = repository;
            this.installer = installer;

            Title = "Update altered permissions information";
            Description = "In order to increase performance some information is now stored in the n2item table. Not running this migration on existing content will cause security protected content to become visible to anyone until it is re-saved.";
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
                foreach (var item in installer.ExecuteQuery(NHInstallationManager.QueryItemsWithAuthorizedRoles))
                {
                    item.AlteredPermissions |= Permission.Read;
                    repository.SaveOrUpdate(item);
                    updatedItems++;
                }

                transaction.Commit();
            }

            return new MigrationResult(this) { UpdatedItems = updatedItems };
        }
    }
}
