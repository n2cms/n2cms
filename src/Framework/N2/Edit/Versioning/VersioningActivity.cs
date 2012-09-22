using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Edit.Versioning
{
    public class ContentVersion
    {
        public virtual int ID { get; set; }
        public virtual int VersionIndex { get; set; }
        public virtual bool IsLatestVersion { get; set; }
        public virtual bool IsPublishedVersion { get; set; }
        public virtual DateTime? Published { get; set; }
        public virtual DateTime Saved { get; set; }
        public virtual ContentState State { get; set; }
        public virtual ContentItem MainItem { get; set; }
        public virtual string ComponentsJson { get; set; }
        public virtual string SavedBy { get; set; }
        public virtual string PublishedBy { get; set; }
        public virtual IEnumerable<ContentItem> GetComponents(Func<int, ContentItem> loader)
        {
            yield break;
        }
        public virtual string DeltaJson { get; set; }
        public virtual IEnumerable<Delta> GetChanges()
        {
            yield break;
        }
    }
}
