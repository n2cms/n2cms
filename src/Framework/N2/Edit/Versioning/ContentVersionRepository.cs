using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using N2.Web;
using N2.Persistence;
using System.Security.Principal;

namespace N2.Edit.Versioning
{
    [Service]
    public class ContentVersionRepository
    {
        public IRepository<ContentVersion> Repository { get; private set; }

        public ContentVersionRepository(IRepository<ContentVersion> repository)
        {
            this.Repository = repository;
        }

        public bool HasDraft(ContentItem item)
        {
            return Repository.Find(Parameter.Equal("MasterVersion.ID", item.ID)).Any();
        }

        public ContentVersion GetDraft(ContentItem item)
        {
            if (item.VersionOf.HasValue)
                return Repository.Find(Parameter.Equal("AssociatedVersion.ID", item.ID)).FirstOrDefault();
            return Repository.Find(Parameter.Equal("MasterVersion.ID", item.ID) & Parameter.Equal("IsDraft", true)).FirstOrDefault();
        }

        public ContentVersion CreateDraft(ContentItem item, IPrincipal user)
        {
            var version = new ContentVersion
            {
                AssociatedVersion = item,
                IsPublishedVersion = !item.VersionOf.HasValue,
                IsDraft = true,
                MasterVersion = item.VersionOf.Value ?? item,
                Published = Utility.CurrentTime(),
                PublishedBy = item.IsPublished() ? item.SavedBy : null,
                Saved = Utility.CurrentTime(),
                SavedBy = user.Identity.Name,
                State = item.State,
                VersionIndex = item.VersionIndex
            };

            Repository.SaveOrUpdate(version);

            return version;
        }
    }
}
