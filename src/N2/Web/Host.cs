using System;
using System.Collections.Generic;
using System.Text;
using N2.Configuration;

namespace N2.Web
{
	public class Host : IHost
	{
		private readonly IWebContext context;
		private Site defaultSite;
		private List<Site> sites = new List<Site>();

		public Host(IWebContext context, HostSection config)
		{
            this.context = context;
			defaultSite = new Site(config.RootID, config.StartPageID);
			foreach (SiteElement configElement in config.Sites)
			{
                Site s = new Site(configElement.RootID ?? config.RootID, configElement.ID, configElement.Name);
                s.Wildcards = configElement.Wildcards || config.Wildcards;
                foreach (string key in configElement.Settings.AllKeys)
                    s.Settings[key] = configElement.Settings[key].Value;
                Sites.Add(s);
			}
		}

		public Host(IWebContext context, int rootItemID, int startPageID)
			: this(context, new Site(rootItemID, startPageID))
		{
		}

		public Host(IWebContext context, Site defaultSite)
		{
			this.context = context;
			this.defaultSite = defaultSite;
			sites.Add(defaultSite);
		}

		public Site DefaultSite
		{
			get { return defaultSite; }
			set { defaultSite = value; }
		}

		public Site CurrentSite
		{
            get { return GetSite(context.HostUrl) ?? DefaultSite; }
        }

        public Site GetSite(Url host)
        {
            if (host == null)
                return null;
            foreach (Site site in Sites)
                if (site.Is(host.Authority))
                    return site;
            return null;
        }

		public IList<Site> Sites
		{
			get { return sites; }
		}

        public void AddSites(IEnumerable<Site> sitesToAdd)
        {
            foreach (Site s in sitesToAdd)
            {
                if (!sites.Contains(s))
                    sites.Add(s);
            }
        }
    }
}
