using System;
using System.Collections.Generic;
using System.Linq;
using N2.Configuration;
using N2.Engine;
using System.Configuration;
using N2.Edit;

namespace N2.Web
{
    /// <summary>
    /// Reads the configuration to build and maintains knowledge of 
    /// <see cref="Site"/>s in the application
    /// </summary>
    [Service(typeof(IHost))]
    public class Host : IHost
    {
        private readonly IWebContext context;
        SiteTable sites = new SiteTable(null, Enumerable.Empty<Site>());

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
            this.sites = new SiteTable(defaultSite, Enumerable.Empty<Site>());
        }

        /// <summary>The default site if the current cannot be determined.</summary>
        public Site DefaultSite
        {
            get { return sites.DefaultSite; }
            set { sites = new SiteTable(value, sites.Sites); }
        }

        /// <summary>The current site based on the request's host header information. Fallbacks to defualt site.</summary>
        public Site CurrentSite
        {
            get { return GetSite(context.Url.HostUrl) ?? DefaultSite; }
        }

        /// <summary>All sites in the system.</summary>
        public IList<Site> Sites
        {
            get { return sites.Sites; }
        }

        /// <summary>Gets the site associated with an url.</summary>
        /// <param name="hostUrl">The url of the site.</param>
        /// <returns>The associated site or null if no matching site is found.</returns>
        public Site GetSite(Url hostUrl)
        {
            if (hostUrl == null)
                return null;
            if(hostUrl.Authority == null)
                return null;

            return sites[hostUrl.Authority];
        }

        /// <summary>Gets the site associated with an item.</summary>
        /// <param name="item">The item whose site to get.</param>
        /// <returns>The site this node belongs to.</returns>
        public Site GetSite(ContentItem item)
        {
            return sites.SiteOf(item);
        }

        /// <summary>Gets the site with the given start page ID.</summary>
        /// <param name="startPageId">The id of the site's start page.</param>
        /// <returns>The site or null if no start page with that id exists.</returns>
        public Site GetSite(int startPageID)
        {
            return sites[startPageID];
        }

        /// <summary>Adds sites to the available sites.</summary>
        /// <param name="additionalSites">Sites to add.</param>
        public void AddSites(IEnumerable<Site> additionalSites)
        {
            var previous = sites;
            sites = previous.Add(additionalSites);

            var args = new SitesChangedEventArgs();
            args.PreviousDefault = previous.DefaultSite;
            args.PreviousSites = previous.Sites;
            args.CurrentDefault = sites.DefaultSite;
            args.CurrentSites = sites.Sites;
            
            if (SitesChanged != null)
                SitesChanged.Invoke(this, args);
        }

        /// <summary>Replaces the site list with new sites.</summary>
        /// <param name="newDefaultSite">The default site to use.</param>
        /// <param name="newSites">The new site list.</param>
        public void ReplaceSites(Site newDefaultSite, IEnumerable<Site> newSites)
        {
            if(newSites == null) throw new ArgumentNullException("newSites");

            var previous = sites;
            sites = new SiteTable(newDefaultSite, newSites);

            var args = new SitesChangedEventArgs();
            args.PreviousDefault = previous.DefaultSite;
            args.PreviousSites = previous.Sites;
            args.CurrentDefault = sites.DefaultSite;
            args.CurrentSites = sites.Sites;

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

            return sites.ContainsKey(item.ID);
        }

        // static helpers

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
                
                foreach (FolderElement folder in configElement.UploadFolders.AllElements)
                {
                    if (string.IsNullOrEmpty(folder.Path))
                        throw new ConfigurationErrorsException("Upload path configured for site '" + configElement.Name + "' cannot be empty.");
                    s.UploadFolders.Add(new FileSystemRoot(folder, s));
                }
                foreach (string key in configElement.Settings.AllKeys)
                    s.Settings[key] = configElement.Settings[key].Value;
                sites.Add(s);
            }
            return sites;
        }

        /// <summary>Is triggered when the sites collection changes.</summary>
        public event EventHandler<SitesChangedEventArgs> SitesChanged;

        #region class SiteTable 

        class SiteTable
        {
            bool wildcards;
            Site defaultSite;
            Dictionary<string, Site> sitesByAuthority = new Dictionary<string, Site>(StringComparer.InvariantCultureIgnoreCase);
            Dictionary<int, Site> sitesById = new Dictionary<int, Site>();
            Site[] sites;



            public SiteTable(Site defaultSite, IEnumerable<Site> sites)
            {
                this.defaultSite = defaultSite;
                this.sites = new List<Site>(sites).ToArray();

                Add(defaultSite);
                foreach (var site in sites)
                    Add(site);
            }



            public Site DefaultSite
            {
                get { return defaultSite; }
            }

            public Site[] Sites
            {
                get { return sites; }
            }



            private void Add(Site site)
            {
                if (site == null)
                    return;

                if(site.Authority != null)
                    sitesByAuthority[site.Authority] = site;
                sitesById[site.StartPageID] = site;
                
                wildcards |= site.Wildcards;
            }

            public Site this[string authority]
            {
                get 
                {
                    Site site;
                    if (sitesByAuthority.TryGetValue(authority, out site))
                        return site;

                    if (wildcards)
                    {
                        int dotIndex = authority.IndexOf('.');
                        if(dotIndex >= 0)
                            return this[authority.Substring(dotIndex + 1)];
                    }

                    return null;
                }
            }

            public Site this[int siteId]
            {
                get
                {
                    Site site;
                    sitesById.TryGetValue(siteId, out site);
                    return site;
                }
            }

            public Site SiteOf(ContentItem item)
            {
                if(item == null)
                    return null;

                return this[item.ID] ?? SiteOf(item.Parent);
            }

            public bool ContainsKey(int startPageId)
            {
                return sitesById.ContainsKey(startPageId);
            }

            public SiteTable Add(IEnumerable<Site> additionalSites)
            {
                List<Site> sites = new List<Site>(this.sites);
                sites.AddRange(additionalSites);

                return new SiteTable(defaultSite, sites);
            }
        }
        #endregion
    }
}
