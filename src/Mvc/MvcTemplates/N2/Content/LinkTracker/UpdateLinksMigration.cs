using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Engine;
using N2.Edit.Installation;
using N2.Persistence;
using N2.Details;
using N2.Edit.LinkTracker;

namespace N2.Management.Content.LinkTracker
{
    [Service(typeof(AbstractMigration))]
    public class UpdateLinksMigration : AbstractMigration
    {
        private IRepository<ContentDetail> repository;
        private Tracker linkTracker;
        private IContentItemRepository itemRepository;

        public UpdateLinksMigration(IRepository<ContentDetail> repository, IContentItemRepository itemRepository, Tracker linkTracker)
        {
            this.repository = repository;
            this.itemRepository = itemRepository;
            this.linkTracker = linkTracker;

            Title = "Update tracked links to v2.3 model";
            Description = "In order to support updating links on moves more information is stored about references on the site.";
        }

        public override bool IsApplicable(DatabaseStatus status)
        {
            return status.DatabaseVersion < 7 
				|| repository.Count(new Parameter("Name", Tracker.LinkDetailName).Detail()) > 0;
        }

        public override MigrationResult Migrate(DatabaseStatus preSchemaUpdateStatus)
        {
            var alreadyUpdated = new HashSet<int>();

            int updatedItems = 0;
            using (var transaction = repository.BeginTransaction())
            {
                foreach (var detail in repository.Find("Name", Tracker.LinkDetailName))
                {
                    if (alreadyUpdated.Contains(detail.EnclosingItem.ID))
                        continue;

                    alreadyUpdated.Add(detail.EnclosingItem.ID);
                    linkTracker.UpdateLinks(detail.EnclosingItem);
                    itemRepository.SaveOrUpdate(detail.EnclosingItem);
                    updatedItems++;
                }
                repository.Flush();
                transaction.Commit();
            }

            return new MigrationResult(this) { UpdatedItems = updatedItems };
        }
    }
}
