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
using N2.Persistence.Proxying;

namespace N2.Edit.Versioning
{
    [Service]
    public class ContentVersionRepository
    {
        public IRepository<ContentVersion> Repository { get; private set; }
        public event EventHandler<VersionsChangedEventArgs> VersionsChanged;
        public event EventHandler<ItemEventArgs> VersionsDeleted;

        Exporter exporter;
        Importer importer;
        private IUrlParser parser;
        private IProxyFactory proxyFactory;

        public ContentVersionRepository(IRepository<ContentVersion> repository, Exporter exporter, Importer importer, IUrlParser parser, IProxyFactory proxyFactory)
        {
            this.Repository = repository;
            this.exporter = exporter;
            this.importer = importer;
            this.parser = parser;
            this.proxyFactory = proxyFactory;
        }

        public ContentVersion GetVersion(ContentItem item, int versionIndex = -1)
        {
            if (versionIndex >= 0)
            {
                return Repository.Find(Parameter.Equal("Master.ID", GetMaster(item).ID) & Parameter.Equal("VersionIndex", versionIndex))
                    .FirstOrDefault();
            }

            return GetVersions(item)
                .FirstOrDefault();
        }

        public string Serialize(ContentItem item)
        {
            foreach(var descendant in Find.EnumerateChildren(item, true, false))
                proxyFactory.OnSaving(descendant);
            return ContentVersion.Serialize(exporter, item);
        }

        public ContentItem Deserialize(string xml)
        {
			var previousIgnoreMissingTypes = importer.Reader.IgnoreMissingTypes;
			try
			{
				importer.Reader.IgnoreMissingTypes = true;
				return ContentVersion.Deserialize(importer, parser, xml);
			}
			finally
			{
				importer.Reader.IgnoreMissingTypes = previousIgnoreMissingTypes;
			}
        }

        public IEnumerable<ContentVersion> GetVersions(ContentItem item)
        {
            var versions = Repository.Find(Parameter.Equal("Master.ID", item.ID) as IParameter);
            return versions
                .OrderByDescending(v => v.VersionIndex);
        }

        public ContentVersion Save(ContentItem item, bool asPreviousVersion = true)
        {
			if (item == null)
				throw new ArgumentNullException("item");

            item = Find.ClosestPage(item);
            var master = GetMaster(item);
            var version = GetVersion(master, item.VersionIndex)
                ?? new ContentVersion();

            ApplyCommonValuesRecursive(item);

            version.Master = GetMaster(item);
            version.Saved = Utility.CurrentTime();
			SerializeVersion(version, item);
            version.ItemCount = N2.Find.EnumerateChildren(item, includeSelf: true, useMasterVersion: false).Count();
            if (asPreviousVersion)
            {
	            try
	            {
		            version.Published = GetVersions(master)
			            .Where(v => v.VersionIndex < item.VersionIndex)
			            .OrderByDescending(v => v.VersionIndex)
			            .Select(v => v.Expired)
			            .FirstOrDefault()
		                                ?? item.Published;
	            }
	            catch (Exception ex)
	            {
		            Logger.Error("Failure in ContentVersionRepository::Save", ex);
		            version.Published = item.Published; // recover
	            }
	            version.Expired = Utility.CurrentTime();
            }
            else
                version.Published = null;

	        try
	        {
		        using (var tx = Repository.BeginTransaction())
		        {
			        Repository.SaveOrUpdate(version);
			        tx.Commit();
		        }
	        }
	        catch (Exception ex)
	        {
		        throw new N2Exception("Failed to commit version to repository", ex);
	        }

	        if (VersionsChanged != null)
                VersionsChanged(this, new VersionsChangedEventArgs { Version = version });

            return version;
        }

        private void ApplyCommonValuesRecursive(ContentItem parent)
        {
            if (string.IsNullOrEmpty(parent.GetVersionKey()))
                parent.SetVersionKey(Guid.NewGuid().ToString());



            foreach (var child in parent.Children)
            {
                child.State = parent.State;
                child.VersionIndex = parent.VersionIndex;
                ApplyCommonValuesRecursive(child);
            }
        }

        private static ContentItem GetMaster(ContentItem item)
        {
            return item.VersionOf.Value ?? item;
        }

        public void Delete(ContentItem item)
        {
			if (item == null)
				throw new ArgumentNullException("item");

            using (var tx = Repository.BeginTransaction())
            {
                if (item.IsPage)
                {
                    Repository.Delete(Repository.Find(Parameter.Equal("Master.ID", GetMaster(item).ID) & Parameter.Equal("VersionIndex", item.VersionIndex)).ToArray());
                    tx.Commit();
                }
                else
                {
                    var page = Find.ClosestPage(item);
                    if (page == null)
                        return;
                    var version = GetVersion(page, page.VersionIndex);
                    if (version == null)
                        return;

                    var versionedPage = DeserializeVersion(version);
	                if (versionedPage == null)
		                return;
					var versionedItem = versionedPage.FindPartVersion(item);
                    if (versionedItem == null)
                        return;
                    versionedItem.AddTo(null);
					Save(versionedItem);
                }
                tx.Commit();
            }

            if (VersionsDeleted != null)
                VersionsDeleted(this, new ItemEventArgs(item));

        }

        public virtual ContentItem GetLatestVersion(ContentItem item)
        {
            var latestVersion = GetVersions(item).FirstOrDefault();
            return (latestVersion != null && latestVersion.VersionIndex > item.VersionIndex) ? DeserializeVersion(latestVersion) : item;
        }

        public virtual int GetGreatestVersionIndex(ContentItem item)
        {
            return GetVersions(item).Select(v => v.VersionIndex).Concat(new[] { item.VersionIndex }).Max();
        }

        public virtual void DeleteVersionsOf(ContentItem item)
        {
            Repository.Delete(GetVersions(item).ToArray());

            if (VersionsDeleted != null)
                VersionsDeleted(this, new ItemEventArgs(item));
        }

        public virtual IEnumerable<ContentVersion> GetVersionsScheduledForPublish(DateTime publishVersionsScheduledBefore)
        {
            return Repository.Find(
                Parameter.Equal("State", ContentState.Waiting)
                & Parameter.LessOrEqual("FuturePublish", publishVersionsScheduledBefore));
        }

		public virtual ContentItem DeserializeVersion(ContentVersion version)
		{
			var initialIgnoreMissingTypes = importer.Reader.IgnoreMissingTypes;
			importer.Reader.IgnoreMissingTypes = true;
			try
			{
				var item = ContentVersion.Deserialize(importer, parser, version.VersionDataXml);
				if (version.FuturePublish.HasValue)
					item["FuturePublishDate"] = version.FuturePublish;
				item.Updated = version.Saved;
				return item;
			}
			finally
			{
				importer.Reader.IgnoreMissingTypes = initialIgnoreMissingTypes;
			}
		}

		public virtual void SerializeVersion(ContentVersion version, ContentItem item)
		{
			if (item == null)
			{
				version.Published = null;
				version.FuturePublish = null;
				version.Expired = null;
				version.VersionDataXml = null;
				version.VersionIndex = 0;
				version.Title = null;
				version.State = ContentState.None;
				version.ItemCount = 0;
				version.VersionDataXml = null;
			}
			else
			{
				version.VersionIndex = item.VersionIndex;
				version.Published = item.Published;
				version.FuturePublish = item["FuturePublishDate"] as DateTime?;
				if (version.FuturePublish.HasValue)
					item["FuturePublishDate"] = null;
				version.Expired = item.Expires;
				version.SavedBy = item.SavedBy;
				version.Title = item.Title;
				version.State = item.State;
				version.VersionDataXml = Serialize(item);
			}
		}
    }
}
