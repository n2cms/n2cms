using N2.Details;
using N2.Edit.Installation;
using N2.Persistence;
using System.Linq;

namespace N2.Management.Installation
{
    [N2.Engine.Service(typeof(AbstractMigration))]
    public class UpgradeVersionsMigration : AbstractMigration
    {
        IRepository<ContentItem> itemRepository;

        public UpgradeVersionsMigration(IRepository<ContentItem> itemRepository)
        {
            this.itemRepository = itemRepository;

            Title = "Upgrade versions to 2.4 model";
            Description = "This migration will open a separate page on which the migration is performed.";
        }

        public override bool IsApplicable(DatabaseStatus status)
        {
	        return status.NeedsUpgrade ||
	               itemRepository.Count(new ParameterCollection(Parameter.IsNotNull("VersionOf.ID")).Take(1)) > 0;
        }

        public override MigrationResult Migrate(DatabaseStatus preSchemaUpdateStatus)
        {
            if (IsApplicable(preSchemaUpdateStatus))
                return new MigrationResult(this) { RedirectTo = "UpgradeVersions.aspx" };
            else
                return new MigrationResult(this);
        }
    }
}
