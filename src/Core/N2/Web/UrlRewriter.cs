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
	/// Search crawler (and user) friendly url rewriter for N2 cms. The 
	/// rewriter walks the tree structure using the page's names to find the 
	/// appropriate name.
	/// </summary>
	public class UrlRewriter : IUrlRewriter
	{
		private readonly IUrlParser urlParser;
		private readonly IWebContext webContext;

		/// <summary>Creates a new instance of the UrlRewriter.</summary>
		public UrlRewriter(IUrlParser urlParser, IWebContext webContext)
		{
			this.urlParser = urlParser;
			this.webContext = webContext;
		}

		#region Rewrite Methods

		/// <summary>Rewrites a dynamic/computed url to an actual template url.</summary>
		/// <param name="context">The context to perform the rewriting on.</param>
		public virtual void RewriteRequest()
		{
			string requestedUrl = webContext.AbsolutePath;
			ContentItem currentPage = webContext.CurrentPage;
			if (IsRewritable(requestedUrl) && currentPage != null)
			{
				string rewrittenUrl = currentPage.RewrittenUrl;

				if (webContext.QueryString.Count == 0)
					webContext.RewritePath(rewrittenUrl);
				else
					webContext.RewritePath(rewrittenUrl + "&" + webContext.Query);
			}
		}

		protected virtual bool IsRewritable(string requestedUrl)
		{
			return requestedUrl.EndsWith(urlParser.DefaultExtension, StringComparison.InvariantCultureIgnoreCase)
			       && !File.Exists(webContext.PhysicalPath);
		}

		#endregion

		public void UpdateCurrentPage()
		{
			try
			{
				if (webContext.AbsolutePath.EndsWith(urlParser.DefaultExtension, StringComparison.InvariantCultureIgnoreCase)
					|| webContext.QueryString["page"] != null)
				{
					webContext.CurrentPage = urlParser.Parse(webContext.RawUrl);
				}
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