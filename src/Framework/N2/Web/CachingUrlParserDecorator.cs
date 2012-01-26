using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Caching;
using N2.Persistence;
using N2.Web.UI;
using log4net;

namespace N2.Web
{
	public class CachingUrlParserDecorator : IUrlParser
	{
        private readonly ILog logger = LogManager.GetLogger(typeof(CachingUrlParserDecorator));

		readonly IUrlParser inner;
		readonly IPersister persister;
        readonly IWebContext webContext;
		TimeSpan slidingExpiration = TimeSpan.FromHours(1);

	    private static readonly HashSet<string> contentParameters = new HashSet<string>
	                                                                    {
	                                                                        PathData.ItemQueryKey,
	                                                                        PathData.PageQueryKey,
	                                                                        "action",
	                                                                        "arguments"
	                                                                    };
        private static readonly object classLock = new object();

				
		public CachingUrlParserDecorator(IUrlParser inner, IPersister persister, IWebContext webContext)
		{
			this.inner = inner;
			this.persister = persister;
		    this.webContext = webContext;
		}

		public event EventHandler<PageNotFoundEventArgs> PageNotFound
		{
			add { inner.PageNotFound += value; }
			remove { inner.PageNotFound -= value; }
		}

		public ContentItem StartPage
		{
			get { return inner.StartPage; }
		}

		public ContentItem CurrentPage
		{
			get { return webContext.CurrentPage ?? (webContext.CurrentPage = ResolvePath(webContext.Url).CurrentPage); }
		}

		public TimeSpan SlidingExpiration
		{
			get { return slidingExpiration; }
			set { slidingExpiration = value; }
		}

		/// <summary>Calculates an item url by walking it's parent path.</summary>
		/// <param name="item">The item whose url to compute.</param>
		/// <returns>A friendly url to the supplied item.</returns>
		public string BuildUrl(ContentItem item)
		{
			return inner.BuildUrl(item);
		}

		/// <summary>Checks if an item is start or root page</summary>
		/// <param name="item">The item to check</param>
		/// <returns>True if the item is a start page or a root page</returns>
		public bool IsRootOrStartPage(ContentItem item)
		{
			return inner.IsRootOrStartPage(item);
		}

        private string GetUrlKey(Url url)
        {
            return url.ToString().ToLowerInvariant();
        }

        private PathData GetStartNode(Url url, Dictionary<string, PathData> cachedPathData, ref string remainingPath)
        {
            if (string.IsNullOrEmpty(remainingPath))
                return PathData.Empty;

            var lastSlash = remainingPath.LastIndexOf('/');
            if (lastSlash == 0)
                return PathData.Empty;

            remainingPath = remainingPath.Substring(0, lastSlash);

            var urlKey = GetUrlKey(new Url(url.Scheme, url.Authority, remainingPath, url.Query, url.Fragment));

            PathData data;
            if (!cachedPathData.TryGetValue(urlKey, out data))
                return GetStartNode(url, cachedPathData, ref remainingPath);

            return data;
        }

	    /// <summary>Finds the content item and the template associated with an url.</summary>
		/// <param name="url">The url to the template to locate.</param>
        /// <param name="startNode">The node to start finding path from if none supplied will start from StartNode</param>
        /// <param name="remainingPath">The remaining path to search</param>
		/// <returns>A TemplateData object. If no template was found the object will have empty properties.</returns>
        public PathData ResolvePath(Url url, ContentItem startNode = null, string remainingPath = null)
	    {
	        if (url == null)
	            return PathData.Empty;

	        // To minimise the amount of unique urls process and cache remove all the querystring parameters we don't care about
	        foreach (var queryParameter in url.GetQueries())
	        {
	            if (!contentParameters.Contains(queryParameter.Key.ToLowerInvariant()))
	                url = url.RemoveQuery(queryParameter.Key);
	        }

	        var urlKey = GetUrlKey(url);
	        
	        Dictionary<string, PathData> cachedPathData;
	        if ((cachedPathData = HttpRuntime.Cache["N2.PathDataCache"] as Dictionary<string, PathData>) == null)
	        {
	            cachedPathData = new Dictionary<string, PathData>();
	            HttpRuntime.Cache.Add("N2.PathDataCache", cachedPathData, new ContentCacheDependency(persister), Cache.NoAbsoluteExpiration, SlidingExpiration, CacheItemPriority.Normal, null);
	        }

            PathData data;
	        if (cachedPathData.TryGetValue(urlKey, out data))
	        {
                if (data == null || data.ID == 0)
                {
                    // Cached path has to CMS content
                    return PathData.Empty;
                }

	            logger.Debug("Retrieving " + url + " from cache");
	            data = data.Attach(persister);
	            data.UpdateParameters(Url.Parse(url).GetQueries());
	        }
	        else
	        {
	            lock (classLock)
	            {
	                if (data == null)
	                {
	                    remainingPath = Url.ToRelative(url.Path).TrimStart('~');

	                    string path = remainingPath;
	                    var pathData = GetStartNode(url, cachedPathData, ref path);

	                    var contentItem = persister.Get(pathData.ID);

	                    data = pathData.ID == 0 ? inner.ResolvePath(url) : inner.ResolvePath(url, contentItem, remainingPath.Replace(path, ""));

	                    if (data.IsCacheable)
	                    {
	                        logger.Debug("Adding " + urlKey + " to cache");
	                        cachedPathData.Add(urlKey, data.Detach());
	                    }
	                }
	            }
	        }

	        return data;
	    }

	    /// <summary>Finds an item by traversing names from the starting point root.</summary>
		/// <param name="url">The url that should be traversed.</param>
		/// <returns>The content item matching the supplied url.</returns>
		public ContentItem Parse(string url)
		{
			return inner.Parse(url);
		}

		/// <summary>Removes a trailing Default.aspx from an URL.</summary>
		/// <param name="path">A URL path without query strings from which to remove any trailing Default.aspx.</param>
		/// <returns>The same path or one stripped of the remaining default document segment.</returns>
		public string StripDefaultDocument(string path)
		{
			return inner.StripDefaultDocument(path);
		}
	}
}