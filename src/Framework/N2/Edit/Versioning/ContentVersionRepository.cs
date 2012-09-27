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
	    Exporter exporter;
		Importer importer;

        public ContentVersionRepository(IRepository<ContentVersion> repository, Exporter exporter, Importer importer)
        {
            this.Repository = repository;
	        this.exporter = exporter;
			this.importer = importer;
        }

        public ContentVersion GetVersion(ContentItem item, int versionIndex = -1)
        {
			if (versionIndex >= 0)
			{
				return Repository.Find(Parameter.Equal("Master.ID", GetMaster(item).ID) & Parameter.Equal("VersionIndex", versionIndex))
					.Select(v => Inject(v))
					.FirstOrDefault();
			}

			return GetVersions(item)
				.Select(v => Inject(v))
				.FirstOrDefault();
        }

		private ContentVersion Inject(ContentVersion v)
		{
			v.Serializer = Serialize;
			v.Deserializer = Deserialize;
			return v;
		}

		public string Serialize(ContentItem item)
		{
			return ContentVersion.Serialize(exporter, item);
		}

		public ContentItem Deserialize(string xml)
		{
			return ContentVersion.Deserialize(importer, xml);
		}


		public IEnumerable<ContentVersion> GetVersions(ContentItem item)
		{
			var versions = Repository.Find(Parameter.Equal("Master.ID", item.ID));
			return versions
				.Select(v => Inject(v))
				.OrderByDescending(v => v.VersionIndex);
		}

		public bool HasDraft(ContentItem item)
		{
			return GetDraft(item) != null;
		}

		public ContentVersion GetDraft(ContentItem item)
		{
			return Repository.Find(Parameter.Equal("Master.ID", GetMaster(item).ID) & Parameter.Equal("State", ContentState.Draft))
				.OrderByDescending(v => v.VersionIndex)
				.Select(v => Inject(v))
				.FirstOrDefault();
		}

        public ContentVersion Save(ContentItem item, string username)
        {
            var version = new ContentVersion(importer, exporter)
            {
				Master = GetMaster(item),
				Published = Utility.CurrentTime(),
                Saved = Utility.CurrentTime(),
				SavedBy = username,
				Version = item
            };

            Repository.SaveOrUpdate(version);

            return version;
        }

		private static ContentItem GetMaster(ContentItem item)
		{
			return item.VersionOf.Value ?? item;
		}

		public void Delete(ContentItem item)
		{
			Repository.Delete(Repository.Find(Parameter.Equal("Master.ID", GetMaster(item).ID) & Parameter.Equal("VersionIndex", item.VersionIndex)).ToArray());
		}

		public virtual ContentItem GetLatestVersion(ContentItem item)
		{
			var latestVersion = GetVersions(item).FirstOrDefault();
			return (latestVersion != null && latestVersion.VersionIndex > item.VersionIndex) ? latestVersion.Version : item;
		}

		public virtual int GetGreatestVersionIndex(ContentItem item)
		{
			return GetVersions(item).Select(v => v.VersionIndex).Concat(new[] { item.VersionIndex }).Max();
		}
	}
}
