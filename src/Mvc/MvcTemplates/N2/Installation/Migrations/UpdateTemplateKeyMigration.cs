using N2.Details;
using N2.Edit.Installation;
using N2.Persistence;

namespace N2.Management.Installation
{
    [N2.Engine.Service(typeof(AbstractMigration))]
    public class UpdateTemplateKeyMigration : AbstractMigration
    {
        InstallationManager installer;
        IRepository<ContentDetail> repository;

        public UpdateTemplateKeyMigration(IRepository<ContentDetail> repository, InstallationManager installer)
        {
            this.repository = repository;
            this.installer = installer;

            Title = "Update template keys to v2.2 model";
            Description = "The information previously known as TemplateName detail is now stored as a TemplateKey property";
        }

        public override bool IsApplicable(DatabaseStatus status)
        {
            return status.DatabaseVersion < 4;
        }

        public override MigrationResult Migrate(DatabaseStatus preSchemaUpdateStatus)
        {
            int updatedItems = 0;
            using (var transaction = repository.BeginTransaction())
            {
                foreach (var detail in repository.Find("Name", "TemplateName"))
                {
                    detail.EnclosingItem.TemplateKey = detail.StringValue;
                    detail.EnclosingItem.SetDetail("TemplateName", null, typeof(string));
                    updatedItems++;
                }
                repository.Flush();
                transaction.Commit();
            }

            return new MigrationResult(this) { UpdatedItems = updatedItems };
        }
    }
}
