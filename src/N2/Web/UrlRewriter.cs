using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using N2.Persistence;
using N2.Web.UI;

namespace N2.Web
{
	/// <summary>
	/// Search crawler (and user) friendly url rewriter for N2 CMS. The 
	/// rewriter walks the tree structure using the page's names to find the 
	/// appropriate name.
	/// </summary>
	public class UrlRewriter : IUrlRewriter
	{
		private readonly string defaultExtension;
		private readonly IUrlParser urlParser;
		private readonly IWebContext webContext;

		/// <summary>Creates a new instance of the UrlRewriter.</summary>
		public UrlRewriter(IUrlParser urlParser, IWebContext webContext)
		{
			this.urlParser = urlParser;
			this.defaultExtension = urlParser.Extension;
			this.webContext = webContext;
		}

		#region Rewrite Methods

		/// <summary>Rewrites a dynamic/computed url to an actual template url.</summary>
		/// <param name="context">The context to perform the rewriting on.</param>
		public virtual void RewriteRequest()
		{
			ContentItem currentPage = webContext.CurrentPage;
			if (!File.Exists(webContext.PhysicalPath) && currentPage != null)
			{
				Url rewrittenUrl = currentPage.RewrittenUrl;
                webContext.RewritePath(rewrittenUrl.AppendQuery(webContext.Query));
			}
		}

		private bool HasContentExtension(string requestedUrl)
		{
			if (requestedUrl.EndsWith(urlParser.Extension, StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
			return false;
		}

		#endregion

		public void UpdateCurrentPage()
		{
			try
			{
				if (HasContentExtension(webContext.AbsolutePath) || webContext.QueryString["page"] != null)
				{
					ContentItem page = urlParser.ParsePage(webContext.RawUrl);

					if (webContext.CurrentPage == null)
						Debug.WriteLine("Setting CurrentPage to '" + page + "'");
					else
						Debug.WriteLine("Changing CurrentPage from '" + webContext.CurrentPage + "' to '" + page + "'");

					webContext.CurrentPage = page;
				}
				else if (webContext.AbsolutePath == "/")
					webContext.CurrentPage = urlParser.StartPage;
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.ToString());
			}
		}

		public void InjectContentPage()
		{
			IContentTemplate template = webContext.Handler as IContentTemplate;
			if (template != null)
			{
				template.CurrentItem = webContext.CurrentPage;
			}
		}
	}
}