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
		private IList<Site> sites = new List<Site>();
        private string scheme = "http";


		public MultipleSitesParser(IPersister persister, IWebContext webContext, IItemNotifier notifier, IHost host, ISitesProvider sitesProvider, HostSection config)
			: base(persister, webContext, notifier, host)
		{
			if (config == null) throw new ArgumentNullException("config");

            if (config.DynamicSites)
            {
                foreach (Site s in sitesProvider.GetSites())
                {
                    Sites.Add(s);
                }
            }
            foreach (Site s in host.Sites)
            {
                if (!Sites.Contains(s))
                {
                    Sites.Add(s);
                }
            }
		}


		#region Properties

        /// <summary>The default scheme to use when creating external url's. Default is http.</summary>
        private string Scheme
        {
            get { return scheme; }
            set { scheme = value; }
        }

		public IList<Site> Sites
		{
			get { return sites; }
			set { sites = value; }
		}

		public override Site CurrentSite
		{
			get { return GetSite(WebContext.Authority) ?? base.CurrentSite; }
		}

		#endregion

		#region Methods

		public override ContentItem Parse(string url)
		{
			if (url.StartsWith("/") || url.StartsWith("~/"))
				return base.Parse(url);
			else
			{
				string host = GetHost(url);
				Site site = GetSite(host);
				if (site != null)
					return TryLoadingFromQueryString(url) ?? Parse(Persister.Get(site.StartPageID), GetPathAndQuery(url));
				else
					return TryLoadingFromQueryString(url);
			}
		}

		public override ContentItem ParsePage(string url)
		{
			if (url.StartsWith("/") || url.StartsWith("~/"))
				return base.ParsePage(url);
			else
			{
				string host = GetHost(url);
				Site site = GetSite(host);
				if (site != null)
					return TryLoadingFromQueryString(url, "item", "page") ?? Parse(Persister.Get(site.StartPageID), GetPathAndQuery(url));
				else
					return TryLoadingFromQueryString(url, "page");
			}
		}

		public override string BuildUrl(ContentItem item)
		{
			if (item == null) throw new ArgumentNullException("item");

			ContentItem current = item;
			string url = string.Empty;

			// Walk the item's parent items to compute it's url
			do
			{
				if (IsRootOrStartPage(current))
					break;
				if (current.IsPage)
					url = "/" + current.Name + url;
				current = current.Parent;
			} while (current != null);
            
            if (current == null)
            {
                return item.RewrittenUrl;
            }
            else if (current.ID == CurrentSite.StartPageID)
            {
                return ToAbsolute(url, item);
            }
            else
            {
                foreach (Site site in Sites)
                    if (current.ID == site.StartPageID)
                        return GetHostedUrl(item, url, site);
                return GetHostedUrl(item, url, DefaultSite);
            }
		}

        private string GetHostedUrl(ContentItem item, string url, Site site)
        {
            if (string.IsNullOrEmpty(site.Authority))
                return item.RewrittenUrl;
            else
                return Scheme + "://" + site.Authority + ToAbsolute(url, item);
        }

		public override bool IsRootOrStartPage(ContentItem item)
		{
			foreach (Site site in Sites)
				if (item.ID == site.StartPageID)
					return true;
			return base.IsRootOrStartPage(item);
		}

		public virtual Site GetSite(string authority)
		{
			foreach (Site site in Sites)
				if (site.Is(authority))
					return site;
			return null;
		}

		public virtual string GetHost(string url)
		{
			Match m = Regex.Match(url, @"^\w+?://([^/]+)", RegexOptions.Compiled);
			if (m != null && m.Groups.Count > 1)
				return m.Groups[1].Value.ToLower();
			return null;
		}

		#endregion
	}
}