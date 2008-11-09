using System;
using System.Text.RegularExpressions;
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
        protected readonly IPersister persister;
		protected readonly IHost host;
        protected readonly IWebContext webContext;
        protected readonly Regex pathAndQueryIntoGroup = new Regex(@"^\w+?://.*?(/.*)$");
		
		private string defaultContentPage = "/default.aspx";

        public event EventHandler<PageNotFoundEventArgs> PageNotFound;

		public UrlParser(IPersister persister, IWebContext webContext, IItemNotifier notifier, IHost host)
		{
			if (host == null) throw new ArgumentNullException("host");

			this.persister = persister;
			this.webContext = webContext;
			this.host = host;

			notifier.ItemCreated += OnItemCreated;
        }


		#region Properties

		/// <summary>Parses the current url to retrieve the current page.</summary>
		public ContentItem CurrentPage
		{
			get 
			{
				return webContext.CurrentPage 
					?? (webContext.CurrentPage = ParsePage(webContext.LocalUrl));
			}
		}

		/// <summary>Gets the current start page.</summary>
		public virtual ContentItem StartPage
		{
			get 
            {
                return persister.Repository.Load(host.CurrentSite.StartPageID);
            }
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
		protected virtual void OnItemCreated(object sender, ItemEventArgs e)
		{
			((IUrlParserDependency)e.AffectedItem).SetUrlParser(this);
		}

		public TemplateData ResolveTemplate(Url url)
		{
			ContentItem item = TryLoadingFromQueryString(url, "page");
			if(item != null)
			{
				return new TemplateData(item, item.Path, item.TemplateUrl, url["action"], url["arguments"]).UpdateParameters(url.GetQueries());
			}

			return StartPage.FindTemplate(url.Path).UpdateParameters(url.GetQueries());
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
				return persister.Get(itemID.Value);
			return null;
		}

		protected virtual ContentItem Parse(ContentItem current, string url)
		{
			if (current == null) throw new ArgumentNullException("current");

			url = CleanUrl(url);

			if (url.Length == 0)
				return current;
			
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

            Debug.WriteLine("No content at: " + url);

            PageNotFoundEventArgs args = new PageNotFoundEventArgs(url);
            if (PageNotFound != null)
                PageNotFound(this, args);
            return args.AffectedItem;
        }

		private string CleanUrl(string url)
		{
            url = Url.PathPart(url);
			url = webContext.ToAppRelative(url);
			url = url.TrimStart('~', '/');
			if (url.EndsWith(Url.DefaultExtension, StringComparison.InvariantCultureIgnoreCase))
                url = url.Substring(0, url.Length - Url.DefaultExtension.Length);
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
		#endregion

		/// <summary>Calculates an item url by walking it's parent path.</summary>
		/// <param name="item">The item whose url to compute.</param>
		/// <returns>A friendly url to the supplied item.</returns>
		public virtual string BuildUrl(ContentItem item)
		{
			ContentItem current = item;
			Url url = "/";
			
			if(item.VersionOf != null)
			{
				current = item.VersionOf;
			}

			// Walk the item's parent items to compute it's url
			do
			{
				if (IsStartPage(current))
				{
					if (!item.IsPage)
						url = url.AppendQuery("item", item.ID);
					else if (item.VersionOf != null)
						url = url.AppendQuery("page", item.ID);

					// we've reached the start page so we're done here
					return Url.ToAbsolute("~" + url.PathAndQuery);
				}

				if (current.IsPage)
					url = url.PrependSegment(current.Name, current.Extension);

				current = current.Parent;
			} while (current != null);

			// we didn't find the startpage before reaching the root -> use rewrittenUrl
			return item.FindTemplate(TemplateData.DefaultAction).RewrittenUrl;
		}

		/// <summary>Handles virtual directories and non-page items.</summary>
		/// <param name="url">The relative url.</param>
		/// <param name="item">The item whose url is supplied.</param>
		/// <returns>The absolute url to the item.</returns>
		[Obsolete]
		protected virtual string ToAbsolute(string url, ContentItem item)
		{
			if (string.IsNullOrEmpty(url) || url == "/")
				url = this.webContext.ToAbsolute("~/");
			else
				url = this.webContext.ToAbsolute("~" + url + item.Extension);

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
            return item.ID == host.CurrentSite.RootItemID || IsStartPage(item);
		}

		/// <summary>Checks if an item is the startpage</summary>
		/// <param name="item">The item to compare</param>
		/// <returns>True if the item is a startpage</returns>
		public virtual bool IsStartPage(ContentItem item)
		{
            return item.ID == host.CurrentSite.StartPageID;
		}
		#endregion
	}
}
