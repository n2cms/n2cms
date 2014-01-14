using N2.Edit.Installation;
using N2.Persistence;
using N2.Web;

namespace N2.Management.Installation
{
    [N2.Engine.Service(typeof(AbstractMigration))]
    public class UpdateAppPathMigration : AbstractMigration
    {
        IRepository<ContentItem> repository;
        private readonly IWebContext webContext;

        public UpdateAppPathMigration(IRepository<ContentItem> repository, IWebContext webContext)
        {
            this.repository = repository;
            this.webContext = webContext;

            Title = "Store the application path in content";
            Description = "Allows the site to change application virtual directory in the future.";
        }

        public override bool IsApplicable(DatabaseStatus status)
        {
            return status.RootItem != null && status.RootItem[InstallationManager.InstallationAppPath] == null;
        }

        public override MigrationResult Migrate(DatabaseStatus preSchemaUpdateStatus)
        {
            var root = repository.Get(preSchemaUpdateStatus.RootItemID);
            root[InstallationManager.InstallationAppPath] = N2.Web.Url.ToAbsolute("~/");
            repository.SaveOrUpdate(root);
            repository.Flush();

            return new MigrationResult(this) { UpdatedItems = 1 };
        }
    }
}
