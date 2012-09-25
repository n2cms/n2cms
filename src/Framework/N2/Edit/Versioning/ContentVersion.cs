using System;
using N2.Persistence;

namespace N2.Edit.Versioning
{
    public class ContentVersion
    {
        public virtual int Id { get; set; }
        public virtual int VersionIndex { get; set; }
		public virtual string Title { get; set; }
		public virtual ContentRelation Master { get; set; }
		public virtual ContentState State { get; set; }
        public virtual DateTime? Published { get; set; }
		public virtual string PublishedBy { get; set; }
        public virtual DateTime Saved { get; set; }
        public virtual string SavedBy { get; set; }
		public virtual string VersionDataXml { get; set; }
    }
}