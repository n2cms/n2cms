using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Edit.Versioning
{
    public class VersionInfo
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public ContentState State { get; set; }
        public string IconUrl { get; set; }
        public DateTime? Published { get; set; }
        public DateTime? FuturePublish { get; set; }
        public DateTime? Expires { get; set; }
        public int VersionIndex { get; set; }
        public string SavedBy { get; set; }

        public ContentItem Content { get { return ContentFactory(); } }
        public Func<ContentItem> ContentFactory { get; set; }
    }
}
