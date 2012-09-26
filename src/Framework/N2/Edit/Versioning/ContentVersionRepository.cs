using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using N2.Engine;
using N2.Persistence.Serialization;
using N2.Web;
using N2.Persistence;
using System.Security.Principal;

namespace N2.Edit.Versioning
{
    [Service]
    public class ContentVersionRepository
    {
        public IRepository<ContentVersion> Repository { get; private set; }
	    public Exporter Exporter { get; set; }

        public ContentVersionRepository(IRepository<ContentVersion> repository, Exporter exporter)
        {
            this.Repository = repository;
	        this.Exporter = exporter;
        }

        public bool HasDraft(ContentItem item)
        {
            return Repository.Find(Parameter.Equal("Master.ID", item.ID)).Any();
        }

        public ContentVersion GetDraft(ContentItem item, int versionIndex = 0)
        {
	        var versions = Repository.Find(Parameter.Equal("Master.ID", item.ID));

			if(versionIndex > 0)
			{
				return versions.FirstOrDefault(v => v.VersionIndex == versionIndex);
			}

	        return versions.OrderByDescending(v => v.Saved).FirstOrDefault();
        }

        public ContentVersion CreateDraft(ContentItem item, IPrincipal user)
        {
	        var sw = new StringWriter();
			this.Exporter.Export(item, ExportOptions.IncludePartsOnly, sw);
			
            var version = new ContentVersion
            {
                VersionIndex = item.VersionIndex,
				Title = item.Title,
				Master = item,
				State = item.State,
                Published = Utility.CurrentTime(),
                PublishedBy = item.IsPublished() ? item.SavedBy : null,
                Saved = Utility.CurrentTime(),
                SavedBy = user.Identity.Name,
				VersionDataXml = sw.ToString()
            };

            Repository.SaveOrUpdate(version);

            return version;
        }
    }
}
