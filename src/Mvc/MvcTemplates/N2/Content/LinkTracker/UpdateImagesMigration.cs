using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Definitions;
using N2.Details;
using N2.Edit.FileSystem.Items;
using N2.Edit.Installation;
using N2.Edit.LinkTracker;
using N2.Engine;
using N2.Persistence;
using N2.Web.Drawing;

namespace Management.N2.Content.LinkTracker
{
    [Service(typeof(AbstractMigration))]
    public class UpdateImagesMigration : AbstractMigration
    {
        private IRepository<ContentDetail> repository;
        private Tracker linkTracker;
        private IContentItemRepository itemRepository;

        public UpdateImagesMigration(IRepository<ContentDetail> repository, IContentItemRepository itemRepository, Tracker linkTracker)
        {
            this.repository = repository;
            this.itemRepository = itemRepository;
            this.linkTracker = linkTracker;

            Title = "Update tracked images to v2.3 model";
            Description = "In order to support updating images src:s when moving and renaming more information is stored about references on the site.";
        }

        public override bool IsApplicable(DatabaseStatus status)
        {
            return status.DatabaseVersion < 7 
				|| repository.Count(new Parameter("Name", Tracker.LinkDetailName).Detail()) > 0;
        }

        public override MigrationResult Migrate(DatabaseStatus preSchemaUpdateStatus)
        {
            var alreadyUpdated = new HashSet<string>();

            int updatedItems = 0;
            using (var transaction = repository.BeginTransaction())
            {
                var detailsWithImages = repository.Find(
                    Parameter.Equal("ValueTypeKey", "String"),
                    Parameter.Like("StringValue", "%<img%")
                );

                foreach (var detail in detailsWithImages)
                {
                    if (alreadyUpdated.Contains(detail.EnclosingItem.Name))
                        continue;

                    alreadyUpdated.Add(detail.EnclosingItem.Name);
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
