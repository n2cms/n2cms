using N2.Details;
using N2.Edit.Installation;
using N2.Persistence;

namespace N2.Management.Installation
{
    [N2.Engine.Service(typeof(AbstractMigration))]
    public class UpdateTranslationKeyMigration : AbstractMigration
    {
        InstallationManager installer;
        IRepository<ContentDetail> detailRepository;
        IRepository<ContentItem> itemRepository;

        public UpdateTranslationKeyMigration(IRepository<ContentDetail> repository, IRepository<ContentItem> itemRepository, InstallationManager installer)
        {
            this.detailRepository = repository;
            this.itemRepository = itemRepository;
            this.installer = installer;

            Title = "Update translation keys to v2.2 model";
            Description = "The information previously known as LanguageKey detail is now stored as a TranslationKey property";
        }

        public override bool IsApplicable(DatabaseStatus status)
        {
            return status.DatabaseVersion < 5;
        }

        public override MigrationResult Migrate(DatabaseStatus preSchemaUpdateStatus)
        {
            int updatedItems = 0;
            using (var transaction = itemRepository.BeginTransaction())
            {
                foreach (var detail in detailRepository.Find("Name", "LanguageKey"))
                {
                    detail.EnclosingItem.TranslationKey = detail.IntValue;
                    detail.EnclosingItem.SetDetail("LanguageKey", null, typeof(int));
                    itemRepository.SaveOrUpdate(detail.EnclosingItem);
                    updatedItems++;
                }
                itemRepository.Flush();
                transaction.Commit();
            }

            return new MigrationResult(this) { UpdatedItems = updatedItems };
        }
    }
}
