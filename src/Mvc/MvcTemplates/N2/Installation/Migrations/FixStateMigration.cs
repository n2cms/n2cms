using System.Linq;
using N2.Edit.Installation;
using N2.Persistence;

namespace N2.Management.Installation
{
    [N2.Engine.Service(typeof(AbstractMigration))]
    public class FixStateMigration : AbstractMigration
    {
        IRepository<ContentItem> repository;

        public FixStateMigration(IRepository<ContentItem> repository)
        {
            this.repository = repository;

            Title = "Fix state on items with no or new state";
            Description = "Changes the stat to either Waiting, Published or Expired on items with the New state.";
        }

        public override bool IsApplicable(DatabaseStatus status)
        {
            return status.DatabaseVersion < DatabaseStatus.RequiredDatabaseVersion 
                || !status.HasSchema 
                || repository.Find("State", ContentState.None).Any()
                || repository.Find("State", ContentState.New).Any();
        }

        public override MigrationResult Migrate(DatabaseStatus preSchemaUpdateStatus)
        {
            int updatedItems = 0;
            using (var transaction = repository.BeginTransaction())
            {
                foreach (var item in repository.Find("State", ContentState.New))
                {
                    Fixit(ref updatedItems, item);
                }
                foreach (var item in repository.Find("State", ContentState.None))
                {
                    Fixit(ref updatedItems, item);
                }

                transaction.Commit();
            }

            return new MigrationResult(this) { UpdatedItems = updatedItems };
        }

        private void Fixit(ref int updatedItems, ContentItem item)
        {
            if (item.IsExpired())
                item.State = ContentState.Unpublished;
            else if (item.IsPublished())
                item.State = ContentState.Published;
            else
                item.State = ContentState.Waiting;

            repository.SaveOrUpdate(item);
            updatedItems++;
        }
    }
}
