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

            // non-pages don't build url's
			while (current != null && !current.IsPage)
            {
                current = current.Parent;
            }
            
            // build path until start page
            Url url = new Url("/");
            while (current != null && !IsStartPage(current))
            {
                url = url.PrependSegment(current.Name, null);
                current = current.Parent;
            }
            
            if (current == null)
            {
                // no start page found
                return item.RewrittenUrl;
            }
            else if (current.ID == host.CurrentSite.StartPageID)
            {
                // the start page belongs to the current site, use relative url
                return ToAbsolute(url, item);
            }
            else
            {
                // find the start page and use it's host name
                foreach (Site site in host.Sites)
                    if (current.ID == site.StartPageID)
                        return GetHostedUrl(item, url, site);
                // revert to default site
                return GetHostedUrl(item, url, host.DefaultSite);
            }
		}

        private string GetHostedUrl(ContentItem item, string url, Site site)
        {
            if (string.IsNullOrEmpty(site.Authority))
                return item.RewrittenUrl;
            else
                return Url.Parse(ToAbsolute(url, item)).SetAuthority(site.Authority);
        }

        public override bool IsStartPage(ContentItem item)
        {
            foreach (Site site in host.Sites)
                if (item.ID == site.StartPageID)
                    return true;
            return false;
        }

        //public virtual Site GetSite(string authority)
        //{
        //    foreach (Site site in host.Sites)
        //        if (site.Is(authority))
        //            return site;
        //    return null;
        //}

        //public virtual string GetHost(string url)
        //{
        //    Match m = Regex.Match(url, @"^\w+?://([^/]+)", RegexOptions.Compiled);
        //    if (m != null && m.Groups.Count > 1)
        //        return m.Groups[1].Value.ToLower();
        //    return null;
        //}

		#endregion
	}
}