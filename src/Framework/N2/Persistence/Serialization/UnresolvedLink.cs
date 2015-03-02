using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.Serialization
{
    public class UnresolvedLink
    {
        public int ReferencedItemID { get; set; }
        private string VersionKey { get; set; }
        public Action<ContentItem> Setter { get; set; }
        public bool IsChild { get; set; }

        public UnresolvedLink(int referencedItemID, Action<ContentItem> setter)
        {
            this.ReferencedItemID = referencedItemID;
            this.Setter = setter;
        }

        public UnresolvedLink(string versionKey, Action<ContentItem> setter)
        {
            this.VersionKey = versionKey;
            this.Setter = setter;
        }

		public ContentItem ReferencingItem { get; set; }

		public string RelationType { get; set; }
	}
}
