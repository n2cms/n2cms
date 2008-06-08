using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Web
{
	public class Host : N2.Web.IHost
	{
		private readonly IWebContext context;
		private Site defaultSite;

		public Host(IWebContext context, Configuration.SiteSection config)
		{
			defaultSite = new Site(config.RootPageID, config.StartPageID);
		}

		public Host(IWebContext context, Configuration.SitesSection config)
		{
			defaultSite = new Site(config.RootID, config.StartPageID);
		}

		public Host(IWebContext context, int rootItemID, int startPageID)
		{
			this.context = context;
			this.defaultSite = new Site(rootItemID, startPageID);
		}

		public Host(IWebContext context, Site defaultSite)
		{
			this.context = context;
			this.defaultSite = defaultSite;
		}

		public Site DefaultSite
		{
			get { return defaultSite; }
			set { defaultSite = value; }
		}

		public Site CurrentSite
		{
			get { return defaultSite; }
		}

		public ICollection<Site> Sites
		{
			get { return null; }
		}
	}
}
