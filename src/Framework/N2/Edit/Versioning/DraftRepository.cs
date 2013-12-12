using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Web;
using N2.Persistence;
using N2.Plugin;
using N2.Engine;

namespace N2.Edit.Versioning
{
    public class DraftInfo
    {
        public int ItemID { get; set; }
        public DateTime Saved { get; set; }
        public string SavedBy { get; set; }

        public int VersionIndex { get; set; }
    }

    [Service]
    public class DraftRepository : IAutoStart
    {
        public ContentVersionRepository Versions { get; private set; }
        private CacheWrapper cache;

        public DraftRepository(ContentVersionRepository repository, CacheWrapper cache)
        {
            this.Versions = repository;
            this.cache = cache;
        }

        public bool HasDraft(ContentItem item)
        {
            if (item == null)
                return false;

            var drafts = GetPagesWithDrafts();
            return drafts.ContainsKey(item.ID) 
                && drafts[item.ID].Saved > item.Updated;
        }

        public DraftInfo GetDraftInfo(ContentItem item)
        {
            if (item.ID == 0)
                return null;

            var drafts = GetPagesWithDrafts();
            DraftInfo draft;
            if (drafts.TryGetValue(item.ID, out draft))
                if (draft.Saved > item.Updated)
                    return draft;

            return null;
        }

        public IEnumerable<ContentVersion> FindDrafts(int skip = 0, int take = 100)
        {
            return Versions.Repository.Find(new ParameterCollection(Parameter.Equal("State", ContentState.Draft)).Skip(skip).Take(take).OrderBy("Saved DESC"))
                .Select(v => Versions.Inject(v));
        }

        public IEnumerable<ContentVersion> FindDrafts(ContentItem newerThanMasterVersion)
        {
            return Versions.Repository.Find((Parameter.Equal("State", ContentState.Draft)
                & Parameter.Equal("Master.ID", newerThanMasterVersion.ID)
                & Parameter.GreaterThan("VersionIndex", newerThanMasterVersion.VersionIndex)).OrderBy("VersionIndex DESC"))
                .OrderByDescending(v => v.VersionIndex)
                .Select(v => Versions.Inject(v));
        }

        private IDictionary<int, DraftInfo> GetPagesWithDrafts()
        {
            return cache.GetOrCreate<IDictionary<int, DraftInfo>>("PagesWithDrafts", () => 
                {
                    var drafts = new Dictionary<int, DraftInfo>();
                    foreach (var draft in FindDrafts())
                    {
                        if (!draft.Master.ID.HasValue)
                            continue;
                        int itemID = draft.Master.ID.Value;
                        if (drafts.ContainsKey(itemID) && drafts[itemID].Saved >= draft.Saved)
                            continue;

                        drafts[draft.Master.ID.Value] = new DraftInfo 
                        {
                            ItemID = itemID, 
                            Saved = draft.Saved, 
                            SavedBy = draft.SavedBy,
                            VersionIndex = draft.VersionIndex
                        };
                    }
                    return drafts;
                });
        }

        public void Start()
        {
            Versions.VersionsChanged += Versions_VersionsChanged;
            Versions.VersionsDeleted += Versions_VersionsDeleted;
        }

        void Versions_VersionsDeleted(object sender, ItemEventArgs e)
        {
            cache.Remove("PagesWithDrafts");
        }

        void Versions_VersionsChanged(object sender, VersionsChangedEventArgs e)
        {
            cache.Remove("PagesWithDrafts");
        }

        public void Stop()
        {
            Versions.VersionsChanged -= Versions_VersionsChanged;
            Versions.VersionsDeleted -= Versions_VersionsDeleted;
        }
    }
}
