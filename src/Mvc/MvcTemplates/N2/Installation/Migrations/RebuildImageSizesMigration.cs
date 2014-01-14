using N2.Configuration;
using N2.Edit.FileSystem;
using N2.Edit.Installation;
using N2.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Installation.Migrations
{
    [Migration]
    public class RebuildImageSizesMigration : AbstractMigration
    {
        private ImageSizesCollection configuredSizes;
        private Edit.UploadFolderSource uploads;
        private IPersister persister;

        public RebuildImageSizesMigration(IPersister persister, N2.Edit.UploadFolderSource uploads, EditSection config)
        {
            this.persister = persister;
            this.uploads = uploads;
            configuredSizes = config.Images.Sizes;

            Title = "Rebuild Image Sizes";
            Description = "Redirects to a page where images can re-generated using currently configured settings";
        }

        public override bool IsApplicable(DatabaseStatus status)
        {
            if (!status.IsInstalled)
                return false;

            var recordedSizeHash = string.Join(";", status.RecordedImageSizes.OrderBy(s => s.Name).Select(s => s.ToString()));
            var existingSizeHash = string.Join(";", configuredSizes.AllElements.OrderBy(s => s.Name).Select(s => s.ToString()));

            return recordedSizeHash != existingSizeHash;
        }

        public override MigrationResult Migrate(DatabaseStatus preSchemaUpdateStatus)
        {
            var recorded = preSchemaUpdateStatus.RecordedImageSizes.ToDictionary(s => s.Name, s => s.ToString());
            var existing = configuredSizes.AllElements.ToDictionary(s => s.Name, s => s.ToString());
            var toAdd = existing.Keys.Except(recorded.Keys).ToList();
            var toModify = existing.Where(kvp => recorded.ContainsKey(kvp.Key) && recorded[kvp.Key] != kvp.Value).Select(kvp => kvp.Key);
            var toRemove = recorded.Keys.Except(existing.Keys).ToList();

            var root = persister.Get(preSchemaUpdateStatus.RootItemID);
            root.RecordInstalledImageSizes(configuredSizes);
            persister.Save(root);

            return new MigrationResult(this)
            {
                UpdatedItems = 1,
                RedirectTo = "{ManagementUrl}/Files/Rebuild.aspx?location=upgrade&add=" + string.Join(",", toAdd)
                    + "&modify=" + string.Join(",", toModify)
                    + "&remove=" + string.Join(",", toRemove)
                    + "&selectedUrl=" + uploads.GetUploadFoldersForAllSites().Select(uf => uf.Path).FirstOrDefault()
            };
        }
    }
}
