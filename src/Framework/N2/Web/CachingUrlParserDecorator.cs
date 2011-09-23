using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Caching;
using N2.Persistence;
using N2.Web.UI;

namespace N2.Web
{
	public class CachingUrlParserDecorator : IUrlParser
	{
		readonly IUrlParser inner;
		readonly IPersister persister;
		TimeSpan slidingExpiration = TimeSpan.FromHours(1);

	    private static readonly HashSet<string> contentParameters = new HashSet<string>
	                                                                    {
	                                                                        PathData.ItemQueryKey,
	                                                                        PathData.PageQueryKey,
	                                                                        "action",
	                                                                        "arguments"
	                                                                    };

        private static readonly object classLock = new object();

				
		public CachingUrlParserDecorator(IUrlParser inner, IPersister persister)
		{
			this.inner = inner;
			this.persister = persister;
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
			get { return inner.CurrentPage; }
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

		/// <summary>Finds the content item and the template associated with an url.</summary>
		/// <param name="url">The url to the template to locate.</param>
		/// <returns>A TemplateData object. If no template was found the object will have empty properties.</returns>
        public PathData ResolvePath(Url url)
		{
		    if (url == null)
		        return PathData.Empty;

            // To minimise the amount of unique urls process and cache remove all the querystring parameters we don't care about
            foreach (var queryParameter in url.GetQueries())
            {
                if (!contentParameters.Contains(queryParameter.Key.ToLowerInvariant()))
                    url = url.RemoveQuery(queryParameter.Key);
            }

		    string key = url.ToString().ToLowerInvariant();

		    var contentKey = "N2." + key;
		    var noContentKey = "N2.NoContent." + key;

            PathData data;
            if ((data = HttpRuntime.Cache[contentKey] as PathData) == null)
		    {
                if (HttpRuntime.Cache[noContentKey] == null)
                {
                    lock (classLock)
                    {
                        if (data == null)
                        {
                            data = inner.ResolvePath(url);
                            if (data.IsCacheable)
                            {
                                if (data.IsEmpty())
                                {
                                    Debug.WriteLine("Adding " + url + " to no content cache");
                                    HttpRuntime.Cache.Add(noContentKey, true, new ContentCacheDependency(persister), Cache.NoAbsoluteExpiration, SlidingExpiration, CacheItemPriority.Normal, null);
                                }
                                else
                                {
                                    // We only global cache the path data if the current url has CMS Content
                                    Debug.WriteLine("Adding " + url + " to cache");
                                    HttpRuntime.Cache.Add(contentKey, data.Detach(), new ContentCacheDependency(persister), Cache.NoAbsoluteExpiration, SlidingExpiration, CacheItemPriority.Normal, null);
                                }
                            }
                        }
                    }
                }
		        data = PathData.Empty;
		    }
		    else
		    {
		        Debug.WriteLine("Retrieving " + url + " from cache");
		        data = data.Attach(persister);
		        data.UpdateParameters(Url.Parse(url).GetQueries());
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