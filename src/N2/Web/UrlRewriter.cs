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
			ContentItem currentPage = webContext.CurrentPage;
			if (!File.Exists(webContext.PhysicalPath) && currentPage != null)
			{
				Url rewrittenUrl = currentPage.RewrittenUrl;
                webContext.RewritePath(rewrittenUrl.AppendQuery(webContext.LocalUrl.Query));
			}
		}

        //private bool HasContentExtension(string requestedUrl)
        //{
        //    if (requestedUrl.EndsWith(Url.DefaultExtension, StringComparison.InvariantCultureIgnoreCase))
        //    {
        //        return true;
        //    }
        //    return false;
        //}

		#endregion

		public void UpdateCurrentPage()
		{
			try
			{
				if (IsUpdatable(webContext.LocalUrl))
				{
					ContentItem page = urlParser.ParsePage(webContext.LocalUrl);

					Debug.WriteLine("CurrentPage <- " + page);

					webContext.CurrentPage = page;
				}
                else if (webContext.LocalUrl == "/")
					webContext.CurrentPage = urlParser.StartPage;
			}
			catch (Exception ex)
			{
				Trace.TraceWarning(ex.ToString());
			}
		}

        private bool IsUpdatable(Url url)
        {
            string extension = url.Extension;
            if((extension == null && string.IsNullOrEmpty(Url.DefaultExtension)) || extension.Equals(Url.DefaultExtension, StringComparison.InvariantCultureIgnoreCase))
                return true;
            else if(url.GetQuery("page") != null)
                return true;
            
            return false;
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
