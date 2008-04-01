using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using N2.Persistence;

namespace N2.Web
{
	/// <summary>
	/// Search crawler (and user) friendly url rewriter for N2 cms. The 
	/// rewriter walks the tree structure using the page's names to find the 
	/// appropriate name.
	/// </summary>
	public class UrlRewriter : IUrlRewriter
	{
		#region Private Fields

		private readonly IDictionary<string, string> rewritesCache = new Dictionary<string, string>();
		private readonly IUrlParser urlParser;
		private readonly IWebContext webContext;

		#endregion

		#region Constructor

		/// <summary>Creates a new instance of the UrlRewriter.</summary>
		public UrlRewriter(IPersister persister, IUrlParser urlParser, IWebContext webContext)
		{
			this.urlParser = urlParser;
			persister.ItemDeleted += ItemChangedEventHandler;
			persister.ItemMoved += ItemChangedEventHandler;
			persister.ItemSaved += ItemChangedEventHandler;
			this.webContext = webContext;
		}

		#endregion

		#region Properties

		/// <summary>The currently built rewrites cache.</summary>
		protected virtual IDictionary<string, string> RewritesCache
		{
			get { return rewritesCache; }
		}

		#endregion

		#region Rewrite Methods

		/// <summary>Rewrites a dynamic/computed url to an actual template url.</summary>
		/// <param name="context">The context to perform the rewriting on.</param>
		public virtual void RewriteRequest()
		{
			if (IsRewritable(webContext.Request))
			{
				string requestedUrl = webContext.RelativeUrl;
				string rewrittenUrl;
				string key = webContext.Request.Url.Authority + requestedUrl;
				if (RewritesCache.ContainsKey(key))
				{
					rewrittenUrl = RewritesCache[key];
				}
				else
				{
					try
					{
						rewritesCache[key] = rewrittenUrl = GetRewrittenUrl(webContext);
					}
					catch (InvalidPathException ex)
					{
						Trace.TraceWarning(ex.ToString());
						return;
					}
				}

				if (string.IsNullOrEmpty(webContext.QueryString))
					webContext.RewritePath(rewrittenUrl);
				else
					webContext.RewritePath(rewrittenUrl + "&" + webContext.QueryString);
			}
		}

		/// <summary>Clears rewrites cache.</summary>
		public virtual void ClearRewrites()
		{
			rewritesCache.Clear();
		}

		protected virtual string GetRewrittenUrl(IWebContext context)
		{
			ContentItem item = urlParser.Parse(context.RelativeUrl);

			if (item == null)
				throw new InvalidPathException(context.RelativeUrl);

			return item.RewrittenUrl;
		}

		protected virtual bool IsRewritable(HttpRequest request)
		{
			return request.Url.AbsolutePath.EndsWith(urlParser.DefaultExtension, StringComparison.InvariantCultureIgnoreCase)
			       && !File.Exists(request.PhysicalPath);
		}

		/// <summary>Is invoked when the site maps is changed. Clears rewrites.</summary>
		/// <param name="item">The item responsible of the change.</param>
		protected virtual void OnSiteMapChanged(ContentItem item)
		{
			ClearRewrites();
		}

		#endregion

		#region Event Handler Methods

		private void ItemChangedEventHandler(object sender, ItemEventArgs e)
		{
			OnSiteMapChanged(e.AffectedItem);
		}

		#endregion

		#region IUrlRewriter Members

		public void InjectContentPage()
		{
			IHttpHandler handler = webContext.CurrentHandler;
			if (handler is UI.IContentTemplate)
			{
				UI.IContentTemplate template = handler as UI.IContentTemplate;
				ContentItem item = webContext.RequestItems["N2.Factory.CurrentPage"] as ContentItem;
				if (item == null)
					item = urlParser.Parse(webContext.RelativeUrl);
				template.CurrentItem = item;
			}
		}

		#endregion
	}
}