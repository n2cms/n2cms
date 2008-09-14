using System.Collections.Generic;
using N2.Configuration;

namespace N2.Web
{
    /// <summary>
    /// Reads the configuration to build and maintains knowledge of 
    /// <see cref="Site"/>s in the application
    /// </summary>
	public class Host : IHost
	{
		private readonly IWebContext context;
		private Site defaultSite;
        private IList<Site> sites;

		public Host(IWebContext context, HostSection config)
		{
            this.context = context;
			defaultSite = new Site(config.RootID, config.StartPageID);

			sites = ExtractSites(config);
		}

        public static IList<Site> ExtractSites(HostSection config)
        {
            List<Site> sites = new List<Site>();
            foreach (SiteElement configElement in config.Sites)
            {
                Site s = new Site(configElement.RootID ?? config.RootID, configElement.ID, configElement.Name);
                s.Wildcards = configElement.Wildcards || config.Wildcards;
                foreach (FolderElement folder in configElement.UploadFolders)
                    s.UploadFolders.Add(folder.Path);
                foreach (string key in configElement.Settings.AllKeys)
                    s.Settings[key] = configElement.Settings[key].Value;
                sites.Add(s);
            }
            return sites;
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
            IList<Site> currentSites = Sites;
            foreach (Site site in currentSites)
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
            sites = Union(Sites, sitesToAdd);
        }

        public void ReplaceSites(IList<Site> newSites)
        {
            sites = newSites;
        }

        public static IList<Site> Union(IList<Site> sites, IEnumerable<Site> sitesToAdd)
        {
            List<Site> writableSites = new List<Site>(sites);
            foreach (Site s in sitesToAdd)
            {
                if (!writableSites.Contains(s))
                    writableSites.Add(s);
            }
            return writableSites;
        }
	}
}
