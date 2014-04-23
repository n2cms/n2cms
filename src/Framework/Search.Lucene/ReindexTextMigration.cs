using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Edit.Installation;
using N2.Details;
using N2.Engine;

namespace N2.Persistence.Search
{
    [Service(typeof(AbstractMigration), Configuration = "Lucene")]
    public class ReindexTextMigration : AbstractMigration
    {
        IRepository<ContentItem> repository;
        IContentIndexer indexer;
        LuceneAccesor accessor;

        public ReindexTextMigration(IRepository<ContentItem> repository, IContentIndexer indexer, LuceneAccesor accessor)
        {
            this.repository = repository;
            this.indexer = indexer;
            this.accessor = accessor;

            Title = "Reindex all content using the lucene based search index";
            Description = "Will re-index all items using lucene search database format.";
        }

        public override bool IsApplicable(DatabaseStatus status)
        {
            return accessor.IndexExists() == false;
        }

        public override MigrationResult Migrate(DatabaseStatus preSchemaUpdateStatus)
        {
            indexer.Clear();

            int count = 0;
            foreach (var item in repository.Find("VersionOf.ID", null))
            {
                indexer.Update(item);
                count++;
            }

            return new MigrationResult(this) { UpdatedItems = count };
        }
    }
}
