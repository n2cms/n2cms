using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Edit.Versioning
{
    public class VersionInfo
    {
        public virtual int ID { get; set; }
        public virtual string Title { get; set; }
        public virtual ContentState State { get; set; }
        public virtual string IconUrl { get; set; }
        public virtual DateTime? Published { get; set; }
        public virtual DateTime? FuturePublish { get; set; }
        public virtual DateTime? Expires { get; set; }
		public virtual int VersionIndex { get; set; }
		public virtual int PartsCount { get; set; }
        public virtual string SavedBy { get; set; }

        public virtual ContentItem Content { get { return ContentFactory(); } }
        public virtual Func<ContentItem> ContentFactory { get; set; }
    }
}
