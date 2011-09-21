using System;
using System.Diagnostics;
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

			string key = url.ToString().ToLowerInvariant();

            var data = HttpContext.Current == null ? null : HttpContext.Current.Items[key] as PathData;
            if (data == null)
            {
                data = HttpRuntime.Cache[key] as PathData;
                if (data == null)
                {
                    data = inner.ResolvePath(url);
                    if (!data.IsEmpty() && data.IsCacheable)
                    {
                        // We only global cache the path data if the current url has CMS Content
                        Debug.WriteLine("Adding " + url + " to cache");
                        HttpRuntime.Cache.Add(key, data.Detach(), new ContentCacheDependency(persister), Cache.NoAbsoluteExpiration, SlidingExpiration, CacheItemPriority.Normal, null);
                    }
                    else
                    {
                        // If there is no content for the given url we add it to a first level request cache 
                        // so we don't have to do unnecessary lookups in the same request.
                        if (HttpContext.Current != null)
                            HttpContext.Current.Items.Add(key, data);
                    }
                }
                else
                {
                    Debug.WriteLine("Retrieving " + url + " from cache");
                    data = data.Attach(persister);
                    data.UpdateParameters(Url.Parse(url).GetQueries());
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