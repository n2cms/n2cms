using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using N2.Persistence;
using N2.Configuration;
using System.Diagnostics;

namespace N2.Web
{
	/// <summary>
	/// Parses urls in a multiple host environment.
	/// </summary>
	public class MultipleSitesParser : UrlParser
	{
		public MultipleSitesParser(IPersister persister, IWebContext webContext, IItemNotifier notifier, IHost host, ISitesProvider sitesProvider, HostSection config)
			: base(persister, webContext, notifier, host)
		{
			if (config == null) throw new ArgumentNullException("config");

            if (config.DynamicSites)
            {
                host.AddSites(sitesProvider.GetSites());
                persister.ItemSaved += delegate(object sender, ItemEventArgs e)
                {
                    if(e.AffectedItem is ISitesSource)
                    {
                        IList<Site> sites = Host.ExtractSites(config);
                        sites = Host.Union(sites, sitesProvider.GetSites());
                        host.ReplaceSites(host.DefaultSite, sites);
                    }
                };
            }
		}

		#region Methods

		public override ContentItem Parse(string url)
		{
			if (url.StartsWith("/") || url.StartsWith("~/"))
				return base.Parse(url);
			else
			{
                Site site = host.GetSite(url);
				if (site != null)
                    return TryLoadingFromQueryString(url, "item", "page") ?? Parse(persister.Get(site.StartPageID), Url.Parse(url).PathAndQuery);
				else
                    return TryLoadingFromQueryString(url, "item", "page");
			}
		}

		public override ContentItem ParsePage(string url)
		{
			if (url.StartsWith("/") || url.StartsWith("~/"))
				return base.ParsePage(url);
			else
			{
				Site site = host.GetSite(url);
				if (site != null)
					return TryLoadingFromQueryString(url, "page") ?? Parse(persister.Get(site.StartPageID), Url.Parse(url).PathAndQuery);
				else
					return TryLoadingFromQueryString(url, "page");
			}
		}

		public override string BuildUrl(ContentItem item)
		{
			if (item == null) throw new ArgumentNullException("item");

			ContentItem current = item;

			if(item.VersionOf != null)
			{
				current = item.VersionOf;
			}
            
            // build path until start page
            Url url = new Url("/");
            while (current != null && !IsStartPage(current))
            {
				if(current.IsPage)
					url = url.PrependSegment(current.Name, current.Extension);
                current = current.Parent;
            }
            
            if (current == null)
                // no start page found
                return item.RewrittenUrl;

			if (item.IsPage && item.VersionOf != null)
				url = url.AppendQuery("page", item.ID);
			else if(!item.IsPage)
				url = url.AppendQuery("item", item.ID);

			if (current.ID == host.CurrentSite.StartPageID)
            {
                // the start page belongs to the current site, use relative url
                return url;
            }

			// find the start page and use it's host name
			foreach (Site site in host.Sites)
				if (current.ID == site.StartPageID)
					return GetHostedUrl(item, url, site);

			// revert to default site
			return GetHostedUrl(item, url, host.DefaultSite);
		}

        private string GetHostedUrl(ContentItem item, string url, Site site)
        {
            if (string.IsNullOrEmpty(site.Authority))
                return item.RewrittenUrl;
            else
                return Url.Parse(url).SetAuthority(site.Authority);
        }

        public override bool IsStartPage(ContentItem item)
        {
            foreach (Site site in host.Sites)
                if (item.ID == site.StartPageID)
                    return true;
            return base.IsStartPage(item);
        }

		#endregion
	}
}