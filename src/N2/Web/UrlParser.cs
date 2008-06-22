using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using N2.Persistence;
using System.Diagnostics;

namespace N2.Web
{
	/// <summary>
	/// Creates unique urls for items and finds the corresponding item from
	/// such an url.
	/// </summary>
	public class UrlParser : IUrlParser
	{
		private readonly Persistence.IPersister persister;
		private readonly IHost host;
		private readonly IWebContext webContext;
		private readonly Regex pathAndQueryIntoGroup = new Regex(@"^\w+?://.*?(/.*)$");
		
		private string defaultExtension = null;
		private string defaultContentPage = "/default.aspx";

        public event EventHandler<PageNotFoundEventArgs> PageNotFound;


		public UrlParser(Persistence.IPersister persister, IWebContext webContext, Persistence.IItemNotifier notifier, IHost host)
		{
			if (host == null) throw new ArgumentNullException("host");

			this.persister = persister;
			this.webContext = webContext;
			this.host = host;

			notifier.ItemCreated += OnItemCreated;
		}


		#region Properties

		/// <summary>Gets the current site.</summary>
		public Web.Site DefaultSite
		{
			get { return host.DefaultSite; }
		}

		/// <summary>Parses the current url to retrieve the current page.</summary>
		public ContentItem CurrentPage
		{
			get 
			{
				return webContext.CurrentPage 
					?? (webContext.CurrentPage = ParsePage(webContext.RawUrl));
			}
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
			get 
            {
                return Persister.Repository.Load(this.CurrentSite.StartPageID);
            }
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

            ContentItem startingPoint = StartPage;
			return TryLoadingFromQueryString(url, "item", "page") ?? Parse(startingPoint, url);
		}

		public virtual ContentItem ParsePage(string url)
		{
			if (string.IsNullOrEmpty(url)) throw new ArgumentNullException("url");

			return TryLoadingFromQueryString(url, "page") ?? Parse(StartPage, url);
		}

		#region Parse Helper Methods
		protected virtual ContentItem TryLoadingFromQueryString(string url, params string[] parameters)
		{
			int? itemID = FindQueryStringReference(url, parameters);
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

			Debug.WriteLine("Not found Url: " + url);

            PageNotFoundEventArgs args = new PageNotFoundEventArgs(null);
            if (PageNotFound != null)
                PageNotFound(this, args);
            return args.AffectedItem;
        }

		private string CleanUrl(string url)
		{
            url = Url.PathPart(url);
			url = webContext.ToAppRelative(url);
			url = url.TrimStart('~', '/');
			if (url.EndsWith(DefaultExtension, StringComparison.InvariantCultureIgnoreCase))
				url = url.Substring(0, url.Length - DefaultExtension.Length);
			return url;
		}

		private int? FindQueryStringReference(string url, params string[] parameters)
		{
            string queryString = Url.QueryPart(url);
			if (!string.IsNullOrEmpty(queryString))
			{
				string[] queries = queryString.Split('&');

				foreach (string parameter in parameters)
				{ 
					int parameterLength = parameter.Length + 1;
					foreach (string query in queries)
					{
						if (query.StartsWith(parameter + "=", StringComparison.InvariantCultureIgnoreCase))
						{
							int id;
							if (int.TryParse(query.Substring(parameterLength), out id))
							{
								return id;
							}
						}
					}
				}
			}
			return null;
		}

		private int? TryParse(string possibleNumber)
		{
			int id;
			if (int.TryParse(possibleNumber, out id))
				return id;
			return null;
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
