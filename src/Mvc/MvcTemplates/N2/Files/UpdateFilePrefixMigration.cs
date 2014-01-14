using N2.Configuration;
using N2.Edit.Installation;
using N2.Edit.LinkTracker;
using N2.Persistence;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Management.Files
{
    [N2.Engine.Service(typeof(AbstractMigration))]
    public class UpdateFilePrefixMigration : AbstractMigration
    {
        private EditSection config;
        private IContentItemRepository repository;
        private Tracker tracker;

        public UpdateFilePrefixMigration(EditSection config, IContentItemRepository repository, Tracker tracker)
        {
            this.config = config;
            this.repository = repository;
            this.tracker = tracker;
            Title = "Updates links to files with prefix";
            Description = "Updates links to files within an upload folder prepending the configured prefix (if any)";
        }

        public override bool IsApplicable(DatabaseStatus status)
        {
            return config.UploadFolders.AllElements.Any(uf => !string.IsNullOrEmpty(uf.UrlPrefix));
        }

        public override MigrationResult Migrate(DatabaseStatus preSchemaUpdateStatus)
        {
            int updatedItems = 0;
            using (var tx = repository.BeginTransaction())
            {
                var itemsWithUntrackedLinks = repository.Find(Parameter.Like(null, "%/upload/%").Detail() & Parameter.IsNull("TrackedLinks").Detail());
                foreach (var item in itemsWithUntrackedLinks)
                {
                    tracker.UpdateLinks(item);
                    repository.SaveOrUpdate(item);
                    updatedItems++;
                }
                tx.Commit();
            }

            var path = config.UploadFolders.AllElements.Where(uf => !string.IsNullOrEmpty(uf.UrlPrefix)).Select(uf => uf.Path).FirstOrDefault();
            path = Url.ToAbsolute(path);

            return new MigrationResult(this) 
            {
                UpdatedItems = updatedItems,
                RedirectTo = "{ManagementUrl}/Content/LinkTracker/UpdateReferences.aspx"
                    + "?selectedUrl=" + path 
                    + "&previousUrl=" +  path
                    + "&location=upgrade"
            };
        }
    }
}
