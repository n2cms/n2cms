using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Engine;
using N2.Edit.Versioning;
using N2.Persistence;
using N2.Edit.Workflow;

namespace N2.Management.Installation
{
    [Service]
    public class UpgradeVersionWorker
    {
        private ContentVersionRepository versionRepository;
        private IContentItemRepository itemRepository;

        public UpgradeVersionWorker(ContentVersionRepository versionRepository, IContentItemRepository itemRepository)
        {
            this.versionRepository = versionRepository;
            this.itemRepository = itemRepository;
        }

        public ContentVersion UpgradeVersion(ContentItem version)
        {
            using (var tx = itemRepository.BeginTransaction())
            {
                var clone = version.CloneForVersioningRecursive();
                clone.VersionOf = version.VersionOf.Value;

                foreach (var child in version.VersionOf.Children.FindParts())
                {
                    child.CloneForVersioningRecursive().AddTo(clone);
                }
                
                var newVersion = versionRepository.Save(clone);
                itemRepository.Delete(version);

                tx.Commit();

                return newVersion;
            }
        }
    }
}
