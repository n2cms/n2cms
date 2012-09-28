using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.Serialization
{
	public class UnresolvedLink
	{
		public int ReferencedItemID { get; set; }
		public Action<ContentItem> Setter;

		public UnresolvedLink(int referencedItemID, Action<ContentItem> setter)
		{
			this.ReferencedItemID = referencedItemID;
			this.Setter = setter;
		}

		public bool IsChild { get; set; }
	}
}
