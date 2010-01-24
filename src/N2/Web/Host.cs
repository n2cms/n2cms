using System;
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
		private Site[] sites = new Site[0];

		public Host(IWebContext context, HostSection config)
		{
            this.context = context;

		    ReplaceSites(new Site(config.RootID, config.StartPageID), ExtractSites(config));
		}

        public Host(IWebContext context, int rootItemID, int startPageID)
			: this(context, new Site(rootItemID, startPageID))
		{
		}

		public Host(IWebContext context, Site defaultSite)
		{
			this.context = context;
			this.defaultSite = defaultSite;
		}

    	/// <summary>The default site if the current cannot be determined.</summary>
    	public Site DefaultSite
		{
			get { return defaultSite; }
			set { defaultSite = value; }
		}

    	/// <summary>The current site based on the request's host header information. Fallbacks to defualt site.</summary>
    	public Site CurrentSite
		{
            get { return GetSite(context.Url.HostUrl) ?? DefaultSite; }
        }

    	/// <summary>All sites in the system.</summary>
    	public IList<Site> Sites
        {
            get { return sites; }
        }

    	/// <summary>Gets the site associated with an url.</summary>
    	/// <param name="hostUrl">The url of the site.</param>
    	/// <returns>The associated site or null if no matching site is found.</returns>
    	public Site GetSite(Url hostUrl)
        {
            if (hostUrl == null)
                return null;
            foreach (Site site in sites)
                if (site.Is(hostUrl.Authority))
                    return site;
            return null;
        }

    	/// <summary>Adds sites to the available sites.</summary>
    	/// <param name="additionalSites">Sites to add.</param>
		public void AddSites(IEnumerable<Site> additionalSites)
        {
			var args = new SitesChangedEventArgs { PreviousSites = sites, PreviousDefault = defaultSite };

			lock (this)
			{
				sites = Union(Sites, additionalSites);

				args.CurrentSites = sites;
				args.CurrentDefault = defaultSite;
			}

			if (SitesChanged != null)
				SitesChanged.Invoke(this, args);
		}

    	/// <summary>Replaces the site list with new sites.</summary>
    	/// <param name="defaultSite">The default site to use.</param>
    	/// <param name="newSites">The new site list.</param>
    	public void ReplaceSites(Site newDefaultSite, IEnumerable<Site> newSites)
        {
            if(newSites == null) throw new ArgumentNullException("newSites");

			var args = new SitesChangedEventArgs { PreviousSites = sites, PreviousDefault = defaultSite };

			lock (this)
			{
				defaultSite = newDefaultSite;
				sites = new List<Site>(newSites).ToArray();

				args.CurrentSites = sites;
				args.CurrentDefault = defaultSite;
			}

			if (SitesChanged != null)
			{
				SitesChanged.Invoke(this, args);
			}
		}

		/// <summary>Checks if an item is the startpage</summary>
		/// <param name="item">The item to compare</param>
		/// <returns>True if the item is a startpage</returns>
		public bool IsStartPage(ContentItem item)
		{
			if (item == null) throw new ArgumentNullException("item");

			if (item.ID == DefaultSite.StartPageID)
				return true;

			foreach (Site site in Sites)
				if (item.ID == site.StartPageID)
					return true;

			return false;
		}

		public static Site[] Union(IEnumerable<Site> sites, IEnumerable<Site> sitesToAdd)
        {
            List<Site> writableSites = new List<Site>(sites);
            foreach (Site s in sitesToAdd)
            {
                if (!writableSites.Contains(s))
                    writableSites.Add(s);
            }
            return writableSites.ToArray();
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

		#region IHost Members

		/// <summary>Is triggered when the sites collection changes.</summary>
		public event EventHandler<SitesChangedEventArgs> SitesChanged;

		#endregion
	}
}
