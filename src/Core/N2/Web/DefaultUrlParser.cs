using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using N2.Persistence;

namespace N2.Web
{
	/// <summary>
	/// Creates unique urls for items and finds the corresponding item from
	/// such an url.
	/// </summary>
	public class DefaultUrlParser : IUrlParser
	{
		private Persistence.IPersister persister;
		private Web.Site defaultSite;
		private readonly IWebContext webContext;
		private string defaultExtension = null;
		private readonly Regex pathAndQueryIntoGroup = new Regex(@"^\w+?://.*?(/.*)$");
        private string defaultContentPage = "/default.aspx";

        public event EventHandler<PageNotFoundEventArgs> PageNotFound;

		#region Constructor
		public DefaultUrlParser(Persistence.IPersister persister, IWebContext webContext, Persistence.IItemNotifier notifier, int rootItemID)
			: this(persister, webContext, notifier, new Site(rootItemID))
		{
		}

		public DefaultUrlParser(Persistence.IPersister persister, IWebContext webContext, Persistence.IItemNotifier notifier, Web.Site site)
		{
			this.persister = persister;
			this.webContext = webContext;
			this.defaultSite = site;
			notifier.ItemCreated += OnItemCreated;
		}
		#endregion

		#region Properties

		/// <summary>Gets the current site.</summary>
		public Web.Site DefaultSite
		{
			get { return defaultSite; }
			set { defaultSite = value; }
		}

		/// <summary>Gets the current site.</summary>
		public virtual Web.Site CurrentSite
		{
			get { return DefaultSite; }
		}

		/// <summary>Gets or sets the default file extension. The default extension should be something that is handled by the .net isapi such as '.aspx'.</summary>
		public string DefaultExtension
		{
			get { return defaultExtension ?? ContentItem.DefaultExtension; }
			set { defaultExtension = value; }
		}

		/// <summary>Gets the current start page.</summary>
		public virtual ContentItem StartPage
		{
			get { return Persister.Get(this.CurrentSite.StartPageID); }
		}

		protected Persistence.IPersister Persister
		{
			get { return persister; }
		}
		protected IWebContext WebContext
		{
			get { return webContext; }
		}

        /// <summary>Gets or sets the default content document name. This is usually "/default.aspx".</summary>
        public string DefaultContentPage
        {
            get { return defaultContentPage; }
            set { defaultContentPage = value; }
        }
		#endregion

		#region Methods

		/// <summary>Invoked when an item is created or loaded from persistence medium.</summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnItemCreated(object sender, N2.Persistence.ItemEventArgs e)
		{
			((IUrlParserDependency)e.AffectedItem).SetUrlParser(this);
		}

		/// <summary>Finds an item by traversing names from the start page.</summary>
		/// <param name="url">The url that should be traversed.</param>
		/// <returns>The content item matching the supplied url.</returns>
		public virtual ContentItem Parse(string url)
		{
			if (string.IsNullOrEmpty(url)) throw new ArgumentNullException("url");

			return TryLoadingFromQueryString(url) ?? Parse(StartPage, url);
		}

		#region Parse Helper Methods
		protected virtual ContentItem TryLoadingFromQueryString(string url)
		{
			int? itemID = FindQueryStringReference(url);
			if (itemID.HasValue)
				return Persister.Get(itemID.Value);
			return null;
		}

		protected virtual ContentItem Parse(ContentItem current, string url)
		{
			if (current == null) throw new ArgumentNullException("current");

			url = CleanUrl(url);

			if (url.Length == 0)
				return current;
			else
				return current.GetChild(url) ?? NotFoundPage(url);
		}

        /// <summary>Returns a page when  no page is found. This method will return the start page if the url matches the default content page property.</summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected virtual ContentItem NotFoundPage(string url)
        {
            string defaultDocument = CleanUrl(DefaultContentPage);
            if (url.Equals(defaultDocument, StringComparison.InvariantCultureIgnoreCase))
            {
                return StartPage;
            }

            PageNotFoundEventArgs args = new PageNotFoundEventArgs(null);
            if (PageNotFound != null)
                PageNotFound(this, args);
            return args.AffectedItem;
        }

		private string CleanUrl(string url)
		{
			if(url.StartsWith(webContext.ApplicationUrl))
				url = url.Substring(webContext.ApplicationUrl.Length);
			url = url.TrimStart('~', '/');
			int queryIndex = url.IndexOf('?');
			if (queryIndex >= 0)
				url = url.Substring(0, queryIndex);
			int hashIndex = url.IndexOf('#');
			if (hashIndex >= 0)
				url = url.Substring(0, hashIndex);
			if (url.EndsWith(DefaultExtension, StringComparison.InvariantCultureIgnoreCase))
				url = url.Substring(0, url.Length - DefaultExtension.Length);
			return url;
		}

		private int? FindQueryStringReference(string url)
		{
			int? pageID = null;
			string[] urlQueryPair = url.Split('?');
			if (urlQueryPair.Length > 1)
			{
				foreach (string query in urlQueryPair[1].Split('&'))
				{
					// item takes priority over page
					if (query.StartsWith("item=", StringComparison.InvariantCultureIgnoreCase))
						return int.Parse(query.Substring(5));
					else if(query.StartsWith("page=", StringComparison.InvariantCultureIgnoreCase))
						pageID = int.Parse(query.Substring(5));
				}
			}
			return pageID;
		}

		private static char[] segmentSplitChars = new char[] { '~', '/' };
		private string[] Segment(string rawUrl)
		{
			rawUrl = GetPathAndQuery(rawUrl);

			string relativeUrlWithoutQueryString = this.webContext.ToAppRelative(rawUrl.Split('?', '#')[0]);
			return relativeUrlWithoutQueryString.Split(segmentSplitChars, StringSplitOptions.RemoveEmptyEntries);
		}

		public virtual string GetPathAndQuery(string rawUrl)
		{
			if (!rawUrl.StartsWith("/"))
			{
				Match m = pathAndQueryIntoGroup.Match(rawUrl);
				if (m != null && m.Groups.Count > 1)
				{
					rawUrl = m.Groups[1].Value;
				}
			}
			return rawUrl;
		}
		#endregion

		/// <summary>Calculates an item url by walking it's parent path.</summary>
		/// <param name="item">The item whose url to compute.</param>
		/// <returns>A friendly url to the supplied item.</returns>
		public virtual string BuildUrl(ContentItem item)
		{
			ContentItem current = item;
			string url = string.Empty;

			// Walk the item's parent items to compute it's url
			do
			{
				if (IsStartPage(current))
					return ToAbsolute(url, item);
				if (current.IsPage)
					url = "/" + current.Name + url;
				current = current.Parent;
			} while (current != null);

			return item.RewrittenUrl;
		}

		/// <summary>Handles virtual directories and non-page items.</summary>
		/// <param name="url">The relative url.</param>
		/// <param name="item">The item whose url is supplied.</param>
		/// <returns>The absolute url to the item.</returns>
		protected virtual string ToAbsolute(string url, ContentItem item)
		{
			if (string.IsNullOrEmpty(url))
				url = this.webContext.ToAbsolute("~/");
			else
				url = this.webContext.ToAbsolute("~" + url + DefaultExtension);

			if (item.IsPage)
				return url;
			else
				return url + "?item=" + item.ID;
		}

		/// <summary>Checks if an item is startpage or root page</summary>
		/// <param name="item">The item to compare</param>
		/// <returns>True if the item is a startpage or a rootpage</returns>
		public virtual bool IsRootOrStartPage(ContentItem item)
		{
			return item.ID == CurrentSite.StartPageID || item.ID == CurrentSite.RootItemID;
		}

		/// <summary>Checks if an item is the startpage</summary>
		/// <param name="item">The item to compare</param>
		/// <returns>True if the item is a startpage</returns>
		public virtual bool IsStartPage(ContentItem item)
		{
			return item.ID == CurrentSite.StartPageID;
		}
		#endregion
	}
}
