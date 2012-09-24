using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Persistence;
using System.Web.Script.Serialization;

namespace N2.Edit.Versioning
{
    public class Change
    {
        public virtual int SortOrder { get; set; }
        public virtual string ZoneName { get; set; }
        public virtual int MasterVersion { get; set; }
        public virtual int AssociatedVersion { get; set; }
        public Delta[] Changes { get; set; }
    }

    public class ContentVersion
    {
        public ContentVersion()
        {
            MasterVersion = new ContentRelation();
            AssociatedVersion = new ContentRelation();
        }

        public virtual int ID { get; set; }
        public virtual int VersionIndex { get; set; }
        public virtual bool IsDraft { get; set; }
        public virtual bool IsPublishedVersion { get; set; }
        public virtual DateTime? Published { get; set; }
        public virtual DateTime Saved { get; set; }
        public virtual ContentState State { get; set; }
        public virtual ContentRelation MasterVersion { get; set; }
        public virtual ContentRelation AssociatedVersion { get; set; }
        public virtual string SavedBy { get; set; }
        public virtual string PublishedBy { get; set; }
        public virtual string ChangesJson { get; set; }
        public virtual IEnumerable<Change> Changes
        {
            get { return string.IsNullOrEmpty(ChangesJson) ? new Change[0] :  new JavaScriptSerializer().Deserialize<Change[]>(ChangesJson); }
            set { ChangesJson = new JavaScriptSerializer().Serialize(value); }
        }
    }
}
