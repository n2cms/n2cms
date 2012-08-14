using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using System.Web.Caching;
using N2.Persistence;
using N2.Web.UI;

namespace N2.Web
{
	public class CachingUrlParserDecorator : IUrlParser
	{
		private readonly Engine.Logger<CachingUrlParserDecorator> logger;

		readonly IUrlParser inner;
		readonly IPersister persister;
		private readonly IWebContext webContext;
		TimeSpan slidingExpiration = TimeSpan.FromHours(1);

		private static readonly object classLock = new object();
		private CacheWrapper cache;

		public CachingUrlParserDecorator(IUrlParser inner, IPersister persister, IWebContext webContext, CacheWrapper cache)
		{
			this.inner = inner;
			this.persister = persister;
			this.webContext = webContext;
			this.cache = cache;
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

		/// <summary>
		/// Returns the string of the current url in invariant culture
		/// </summary>
		/// <param name="url">Url to get the string of</param>
		/// <returns>Lowered invariant string of the url passed in</returns>
		private string GetUrlLowerInvariantString(Url url)
		{
			return url.ToString().ToLowerInvariant();
		}

		/// <summary>
		/// Recursively traverse down the url and find the first cached PathData node.  
		/// </summary>
		/// <param name="url">The full url</param>
		/// <param name="cachedPathData">The cached path datas</param>
		/// <param name="remainingPath">The remaining path which is un-cached</param>
		/// <param name="depth"></param>
		/// <returns></returns>
		private PathData GetStartNode(Url url, Dictionary<string, PathData> cachedPathData, ref string remainingPath, int depth)
		{
			const int recursionDepth = 100;

			if (++depth > recursionDepth || string.IsNullOrEmpty(remainingPath))
				return PathData.Empty;

			var lastSlash = remainingPath.LastIndexOf('/');
			if (lastSlash == 0)
				return PathData.Empty;

			remainingPath = remainingPath.Substring(0, lastSlash);

			var urlKey = GetUrlLowerInvariantString(new Url(url.Scheme, url.Authority, remainingPath, url.Query, url.Fragment));

			PathData data;
			if (!cachedPathData.TryGetValue(urlKey, out data))
				return GetStartNode(url, cachedPathData, ref remainingPath, depth);

			return data;
		}

		public PathData ResolvePath(Url url, ContentItem startNode = null, string remainingPath = null)
		{
			if (url == null)
				return PathData.Empty;

			url = url.GetNormalizedContentUrl();

			var urlKey = GetUrlLowerInvariantString(url);

			// Make sure the cached path data is initialized thread safely
			Dictionary<string, PathData> cachedPathData;
			if ((cachedPathData = cache.Get<Dictionary<string, PathData>>("N2.PathDataCache")) == null)
			{
				lock (classLock)
				{
					if ((cachedPathData = cache.Get<Dictionary<string, PathData>>("N2.PathDataCache")) == null)
					{
						cachedPathData = new Dictionary<string, PathData>();
						cache.Add("N2.PathDataCache", cachedPathData, new CacheOptions { SlidingExpiration = SlidingExpiration });
					}
				}
			}

			PathData data;
			if (cachedPathData.TryGetValue(urlKey, out data))
			{
				data = data.Attach(persister);
				if (data == null || data.ID == 0)
				{
					// Cached path has to CMS content
					return data;
				}

				logger.Debug("Retrieving " + url + " from cache");
				if (!string.IsNullOrEmpty(url.Query))
					data.UpdateParameters(Url.Parse(url).GetQueries());
			}
			else
			{
				// The requested url doesn't exist in the cached path data
				lock (classLock)
				{
					if (!cachedPathData.TryGetValue(urlKey, out data))
					{
						remainingPath = remainingPath ?? Url.ToRelative(url.Path).TrimStart('~');

						string path = remainingPath;
						var pathData = GetStartNode(url, cachedPathData, ref path, 0);

						data = pathData.ID == 0
							? inner.ResolvePath(url)
							: inner.ResolvePath(url, persister.Get(pathData.ID), remainingPath.Substring(path.Length, remainingPath.Length - path.Length));

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