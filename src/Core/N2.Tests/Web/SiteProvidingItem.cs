using System;
using System.Collections.Generic;
using System.Text;
using N2.Web;

namespace N2.Tests.Web
{
	public class SiteProvidingItem : ContentItem, ISitesSource
	{
		public IEnumerable<Site> GetSites()
		{
			int rootItemID = this.ID;
			for (ContentItem item = this; item != null; item = item.Parent)
				rootItemID = item.ID;
			return new Site[] { new Site(rootItemID, this.ID, "www." + this.Name + ".com") };
		}
	}
}
