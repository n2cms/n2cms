using System;
using System.Diagnostics;
using N2.Configuration;
using N2.Persistence;
using N2.Plugin;

namespace N2.Web
{
	/// <summary>
	/// Parses managementUrls in a multiple host environment.
	/// </summary>
	public class MultipleSitesParser : UrlParser
	{
		public MultipleSitesParser(IPersister persister, IWebContext webContext, IHost host, N2.Plugin.ConnectionMonitor connections, HostSection config)
			: base(persister, webContext, host, connections, config)
		{
		}

		/// <summary>Finds an item by traversing names from the start page.</summary>
		/// <param name="url">The url that should be traversed.</param>
		/// <returns>The content item matching the supplied url.</returns>
		public override ContentItem Parse(string url)
		{
			if (string.IsNullOrEmpty(url)) return null;

			if (url.StartsWith("/") || url.StartsWith("~/"))
				return base.Parse(url);
			
			Site site = host.GetSite(url);
			if (site != null)
				return TryLoadingFromQueryString(url, PathData.ItemQueryKey, PathData.PageQueryKey) 
					?? Parse(persister.Get(site.StartPageID), Url.Parse(url).PathAndQuery);

			return TryLoadingFromQueryString(url, PathData.ItemQueryKey, PathData.PageQueryKey);
		}

		/// <summary>May be overridden to provide custom start page depending on authority.</summary>
		/// <param name="url">The host name and path information.</param>
		/// <returns>The configured start page.</returns>
		protected override ContentItem GetStartPage(Url url)
		{
			if (!IsOnline) return null;

			if (!url.IsAbsolute)
				return StartPage;
			Site site = host.GetSite(url) ?? host.CurrentSite;
			return persister.Get(site.StartPageID);
		}

		/// <summary>Calculates an item url by walking it's parent path.</summary>
		/// <param name="item">The item whose url to compute.</param>
		/// <returns>A friendly url to the supplied item.</returns>
		public override string BuildUrl(ContentItem item)
		{
			if (item == null) throw new ArgumentNullException("item");

			ContentItem current = item;

			if (item.VersionOf.HasValue)
			{
				return BuildUrl(item.VersionOf).ToUrl()
					.SetQueryParameter(PathData.VersionQueryKey, item.VersionIndex);
			}

			// move up until first real page
			while(current != null && !current.IsPage)
			{
				current = current.Parent;
			}

			// no start page found, use rewritten url
			if (current == null) return item.FindPath(PathData.DefaultAction).GetRewrittenUrl();

			Url url;
			if (host.IsStartPage(current))
			{
				// we move right up to the start page
				url = "/";
			}
			else
			{
				// at least one node before the start page
				url = new Url("/" + current.Name + current.Extension);
				current = current.Parent;
				// build path until a start page
				while (current != null && !host.IsStartPage(current))
				{
					url = url.PrependSegment(current.Name);
					current = current.Parent;
				}
			}

			// no start page found, use rewritten url
			if (current == null) return item.FindPath(PathData.DefaultAction).GetRewrittenUrl();

			if (item.IsPage && item.VersionOf.HasValue)
				// the item was a version, add this information as a query string
				url = url.AppendQuery(PathData.PageQueryKey, item.ID);
			else if(!item.IsPage)
				// the item was not a page, add this information as a query string
				url = url.AppendQuery(PathData.ItemQueryKey, item.ID);

			if (current.ID == host.CurrentSite.StartPageID)
            {
                // the start page belongs to the current site, use relative url
            	return Url.ToAbsolute("~" + url.PathAndQuery);
            }

            var site = host.GetSite(current.ID) ?? host.DefaultSite;

            return GetHostedUrl(item, url, site);
        }

        private string GetHostedUrl(ContentItem item, string url, Site site)
        {
        	if (string.IsNullOrEmpty(site.Authority))
				return item.FindPath(PathData.DefaultAction).GetRewrittenUrl();
        	
			return Url.Parse(url).SetAuthority(site.Authority);
        }
	}
}