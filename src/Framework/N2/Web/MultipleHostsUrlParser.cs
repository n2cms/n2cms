using System;
using System.Diagnostics;
using N2.Configuration;
using N2.Persistence;
using N2.Plugin;
using N2.Edit.Versioning;

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
                    ?? Parse(persister.Get(site.StartPageID), url);

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
        public override Url BuildUrl(ContentItem item)
        {
            if (item == null) throw new ArgumentNullException("item");

            if (item.VersionOf.HasValue && item.VersionOf.Value != null)
            {
                return BuildUrl(item.VersionOf.Value)
                    .SetQueryParameter(PathData.VersionIndexQueryKey, item.VersionIndex);
            }
            else if (item.ID == 0)
            {
                var page = Find.ClosestPage(item);
                if (page != null && page != item)
                {
                    return BuildUrl(page)
                        .SetQueryParameter(PathData.VersionIndexQueryKey, page.VersionIndex)
                        .SetQueryParameter(PathData.VersionKeyQueryKey, item.GetVersionKey());
                }
            }

            // move up until first real page
            var current = Find.ClosestPage(item);

            // no page found, throw
            if (current == null) throw new N2Exception("Cannot build url to data item '{0}' with no containing page item.", item);

            Url url = BuildPageUrl(current, ref current);

            var startPage = FindStartPage(current);
            // no start page found, use rewritten url
            if (startPage == null)
                return item.FindPath(PathData.DefaultAction).GetRewrittenUrl();

            if (!item.IsPage)
                // the item was not a page, add this information as a query string
                url = url.AppendQuery(PathData.ItemQueryKey, item.ID);

            if (startPage.ID == host.CurrentSite.StartPageID)
            {
                // the start page belongs to the current site, use relative url
                return Url.ToAbsolute("~" + url.PathAndQuery);
            }

            var site = host.GetSite(startPage.ID) ?? host.DefaultSite;

            return GetHostedUrl(item, url, site);
        }

        private ContentItem FindStartPage(ContentItem current)
        {
            if (current == null)
                return null;
            if (IsRootOrStartPage(current))
                return current;
            return FindStartPage(current.Parent);
        }

        private string GetHostedUrl(ContentItem item, string url, Site site)
        {
            if (string.IsNullOrEmpty(site.Authority))
                return item.FindPath(PathData.DefaultAction).GetRewrittenUrl();
            
            return Url.Parse(url).SetAuthority(site.Authority);
        }
    }
}
