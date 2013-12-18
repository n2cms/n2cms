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
                    .Select(v => Inject(v))
                    .FirstOrDefault();
            }

            return GetVersions(item)
                .Select(v => Inject(v))
                .FirstOrDefault();
        }

        internal ContentVersion Inject(ContentVersion v)
        {
            v.Serializer = Serialize;
            v.Deserializer = Deserialize;
            return v;
        }

        public string Serialize(ContentItem item)
        {
            foreach(var descendant in Find.EnumerateChildren(item, true, false))
                proxyFactory.OnSaving(descendant);
            return ContentVersion.Serialize(exporter, item);
        }

        public ContentItem Deserialize(string xml)
        {
            return ContentVersion.Deserialize(importer, parser, xml);
        }


        public IEnumerable<ContentVersion> GetVersions(ContentItem item)
        {
            var versions = Repository.Find(Parameter.Equal("Master.ID", item.ID));
            return versions
                .Select(v => Inject(v))
                .OrderByDescending(v => v.VersionIndex);
        }

        public ContentVersion Save(ContentItem item)
        {
            item = Find.ClosestPage(item);
            var version = GetVersion(GetMaster(item), item.VersionIndex)
                ?? Inject(new ContentVersion());

            ApplyCommonValuesRecursive(item);

            version.Master = GetMaster(item);
            version.Saved = Utility.CurrentTime();
            version.Version = item;
            version.ItemCount = N2.Find.EnumerateChildren(item, includeSelf: true, useMasterVersion: false).Count();


            using (var tx = Repository.BeginTransaction())
            {
                Repository.SaveOrUpdate(version);
                tx.Commit();
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
                    var versionedItem = version.Version.FindPartVersion(item);
                    if (versionedItem == null)
                        return;
                    versionedItem.AddTo(null);
                    Save(version.Version);
                }
                tx.Commit();
            }

            if (VersionsDeleted != null)
                VersionsDeleted(this, new ItemEventArgs(item));

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
    }
}
