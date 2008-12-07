using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using N2.Persistence;
using N2.Web.UI;
using System.Collections.Specialized;
using N2.Configuration;

namespace N2.Web
{
	/// <summary>
	/// Search crawler (and user) friendly url rewriter for N2 CMS. The 
	/// rewriter walks the tree structure using the page's names to find the 
	/// appropriate name.
	/// </summary>
	public class UrlRewriter : IUrlRewriter
	{
		readonly RewriteMethod rewrite = RewriteMethod.RewriteRequest;
		readonly bool rewriteEmptyExtension;
		readonly bool ignoreExistingFiles;
		readonly string[] observedExtensions = new[] { ".aspx" };
		readonly IUrlParser urlParser;
		readonly IWebContext webContext;
		readonly IErrorHandler errorHandler;

		/// <summary>Creates a new instance of the UrlRewriter.</summary>
		public UrlRewriter(IUrlParser urlParser, IWebContext webContext, IErrorHandler errorHandler)
		{
            if (urlParser == null) throw new ArgumentNullException("urlParser");
            if (webContext == null) throw new ArgumentNullException("webContext");
            
            this.urlParser = urlParser;
			this.webContext = webContext;
			this.errorHandler = errorHandler;
		}

        /// <summary>Creates a new instance of the UrlRewriter.</summary>
		public UrlRewriter(IUrlParser urlParser, IWebContext webContext, IErrorHandler errorHandler, HostSection config)
            : this(urlParser, webContext, errorHandler)
        {
            if (config == null) throw new ArgumentNullException("config");

            rewrite = config.Web.Rewrite;
            rewriteEmptyExtension = config.Web.ObserveEmptyExtension;
            StringCollection additionalExtensions = config.Web.ObservedExtensions;
            if (additionalExtensions != null && additionalExtensions.Count > 0)
            {
                observedExtensions = new string[additionalExtensions.Count + 1];
                additionalExtensions.CopyTo(observedExtensions, 1);
            }
            observedExtensions[0] = config.Web.Extension;
            ignoreExistingFiles = config.Web.IgnoreExistingFiles;
        }

		public void InitializeRequest()
		{
			try
			{
				Url url = webContext.Url.LocalUrl;
				if (IsUpdatable(url))
				{
					PathData data = urlParser.ResolvePath(url);

					Debug.WriteLine("CurrentPage <- " + data.CurrentItem);

					webContext.CurrentPage = data.CurrentItem;
					webContext.CurrentPath = data;
				}
				else if (url == "/")
					webContext.CurrentPage = urlParser.StartPage;
			}
			catch (Exception ex)
			{
				errorHandler.Notify(ex);
			}
		}

		/// <summary>Rewrites a dynamic/computed url to an actual template url.</summary>
		public virtual void RewriteRequest()
		{
            if (rewrite == RewriteMethod.None)
                return;

			PathData data = webContext.CurrentPath;
            if (data != null && data.CurrentItem != null&& PathIsRewritable())
			{
				Url requestedUrl = webContext.Url.LocalUrl;
				Url rewrittenUrl = data.RewrittenUrl;

				Trace.WriteLine(requestedUrl + " -> " + rewrittenUrl);

                if (rewrite == RewriteMethod.RewriteRequest)
                    webContext.RewritePath(rewrittenUrl);
                else
                    webContext.TransferRequest(rewrittenUrl);
			}
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

		private bool IsUpdatable(Url url)
		{
			string extension = url.Extension;
			if (rewriteEmptyExtension && string.IsNullOrEmpty(extension))
				return true;
			foreach (string observed in observedExtensions)
				if (string.Equals(observed, extension, StringComparison.InvariantCultureIgnoreCase))
					return true;
			if (url.GetQuery("page") != null)
				return true;

			return false;
		}

		bool PathIsRewritable()
		{
			return ignoreExistingFiles || (!File.Exists(webContext.PhysicalPath) && !Directory.Exists(webContext.PhysicalPath));
		}
	}
}
