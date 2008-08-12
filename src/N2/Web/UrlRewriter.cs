using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using N2.Persistence;
using N2.Web.UI;
using System.Collections.Specialized;

namespace N2.Web
{
	/// <summary>
	/// Search crawler (and user) friendly url rewriter for N2 CMS. The 
	/// rewriter walks the tree structure using the page's names to find the 
	/// appropriate name.
	/// </summary>
	public class UrlRewriter : IUrlRewriter
	{
        private bool rewriteEmptyExtension = false;
        string[] observedExtensions = new string[] { ".aspx" };
		private readonly IUrlParser urlParser;
		private readonly IWebContext webContext;

		/// <summary>Creates a new instance of the UrlRewriter.</summary>
		public UrlRewriter(IUrlParser urlParser, IWebContext webContext)
		{
            if (urlParser == null) throw new ArgumentNullException("urlParser");
            if (webContext == null) throw new ArgumentNullException("webContext");
            
            this.urlParser = urlParser;
			this.webContext = webContext;
		}

        /// <summary>Creates a new instance of the UrlRewriter.</summary>
        public UrlRewriter(IUrlParser urlParser, IWebContext webContext, Configuration.HostSection config)
            : this(urlParser, webContext)
        {
            if (config == null) throw new ArgumentNullException("config");

            rewriteEmptyExtension = config.Web.ObserveEmptyExtension;
            StringCollection additionalExtensions = config.Web.ObservedExtensions;
            if (additionalExtensions != null && additionalExtensions.Count > 0)
            {
                observedExtensions = new string[additionalExtensions.Count + 1];
                additionalExtensions.CopyTo(observedExtensions, 1);
            }
            observedExtensions[0] = config.Web.Extension;
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
            if (rewriteEmptyExtension && string.IsNullOrEmpty(extension))
                return true;
            foreach (string observed in observedExtensions)
                if (string.Equals(observed, extension, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            if(url.GetQuery("page") != null)
                return true;

            return false;
        }

        /// <summary>Infuses the http handler (usually an aspx page) with the content page associated with the url if it implements the <see cref="IContentTemplate"/> interface.</summary>
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
