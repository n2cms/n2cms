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

		private static readonly object pathLock = new object();
		private static readonly object urlLock = new object();
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

		public event EventHandler<UrlEventArgs> BuiltUrl
		{
			add { inner.BuiltUrl += value; }
			remove { inner.BuiltUrl -= value; }
		}

		public event EventHandler<UrlEventArgs> BuildingUrl
		{
			add { inner.BuildingUrl += value; }
			remove { inner.BuildingUrl -= value; }
		}

		public ContentItem StartPage
		{
			get { return inner.StartPage; }
		}

		public ContentItem CurrentPage
		{
			get { return webContext.CurrentPage ?? (webContext.CurrentPage = FindPath(webContext.Url).CurrentPage); }
		}

		public TimeSpan SlidingExpiration
		{
			get { return slidingExpiration; }
			set { slidingExpiration = value; }
		}

		/// <summary>Calculates an item url by walking it's parent path.</summary>
		/// <param name="item">The item whose url to compute.</param>
		/// <returns>A friendly url to the supplied item.</returns>
		public Url BuildUrl(ContentItem item)
		{
			if (item.ID == 0)
				return inner.BuildUrl(item);


			var cacheKey = "N2.UrlCache" + webContext.Url.Authority.ToLower();
			Dictionary<int, string> itemToUrlCache = cache.Get<Dictionary<int, string>>(cacheKey);
			if (itemToUrlCache == null)
			{
				lock (urlLock)
				{
					itemToUrlCache = cache.Get<Dictionary<int, string>>(cacheKey);
					if (itemToUrlCache == null)
					{
						itemToUrlCache = new Dictionary<int, string>();
						cache.Add(cacheKey, itemToUrlCache, new CacheOptions { SlidingExpiration = SlidingExpiration });
					}
				}
			}

			bool exists;
			string url;
			lock (urlLock)
			{
				exists = itemToUrlCache.TryGetValue(item.ID, out url);
			}

			if (!exists)
			{
				url = inner.BuildUrl(item);
				lock (urlLock)
				{
					itemToUrlCache[item.ID] = url;
				}
			}

			return url;
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

		[Obsolete("Use FindPath")]
		/// <summary>Finds the path associated with an url.</summary>
		/// <param name="url">The url to the template to locate.</param>
		/// <param name="startNode">The node to start finding path from if none supplied will start from StartNode</param>
		/// <param name="remainingPath">The remaining path to search</param>
		/// <returns>A PathData object. If no template was found the object will have empty properties.</returns>
		public PathData ResolvePath(Url url, ContentItem startNode = null, string remainingPath = null)
		{
			return FindPath(url, startNode, remainingPath);
		}

		/// <summary>Finds the path associated with an url.</summary>
		/// <param name="url">The url to the template to locate.</param>
		/// <param name="startNode">The node to start finding path from if none supplied will start from StartNode</param>
		/// <param name="remainingPath">The remaining path to search</param>
		/// <returns>A PathData object. If no template was found the object will have empty properties.</returns>
		public PathData FindPath(Url url, ContentItem startNode = null, string remainingPath = null)
		{
			if (url == null)
				return PathData.Empty;

			url = url.GetNormalizedContentUrl();

			var urlKey = GetUrlLowerInvariantString(url);

			// Make sure the cached path data is initialized thread safely
			Dictionary<string, PathData> cachedPathData;
			if ((cachedPathData = cache.Get<Dictionary<string, PathData>>("N2.PathDataCache")) == null)
			{
				lock (pathLock)
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
				lock (pathLock)
				{
					if (!cachedPathData.TryGetValue(urlKey, out data))
					{
						remainingPath = remainingPath ?? Url.ToRelative(url.Path).TrimStart('~');

						string path = remainingPath;
						var pathData = GetStartNode(url, cachedPathData, ref path, 0);

						data = pathData.ID == 0
							? inner.FindPath(url)
							: inner.FindPath(url, persister.Get(pathData.ID), remainingPath.Substring(path.Length, remainingPath.Length - path.Length));

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