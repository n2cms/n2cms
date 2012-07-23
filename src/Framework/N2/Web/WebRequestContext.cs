using System;
using System.Collections;
using System.Diagnostics;
using System.Security.Principal;
using System.Web;
using System.Web.Hosting;
using N2.Engine;

namespace N2.Web
{
    /// <summary>
    /// A request context class that interacts with HttpContext.Current.
    /// </summary>
    public class WebRequestContext : IWebContext, IDisposable
    {
		private readonly Engine.Logger<WebRequestContext> logger;
		IProvider<HttpContextBase> httpContextProvider;
		private const string CurrentPathKey = "N2.CurrentPath";

		public WebRequestContext(IProvider<HttpContextBase> httpContextProvider)
		{
			this.httpContextProvider = httpContextProvider;
		}

        /// <summary>Provides access to HttpContext.Current.</summary>
        protected virtual HttpContext CurrentHttpContext
        {
            get
            {
                if (System.Web.HttpContext.Current == null)
                    throw new N2Exception("Tried to retrieve HttpContext.Current but it's null. This may happen when working outside a request or when doing stuff after the context has been recycled.");
				return System.Web.HttpContext.Current;
            }
        }

		public virtual HttpContextBase HttpContext
		{
			get { return httpContextProvider.Get(); }
		}

        public bool IsWeb
        {
            get { return true; }
        }

        /// <summary>Gets a dictionary of request scoped items.</summary>
        public IDictionary RequestItems
        {
            get { return CurrentHttpContext.Items; }
        }

        /// <summary>A page instance stored in the request context.</summary>
        public ContentItem CurrentPage
        {
            get { return CurrentPath.CurrentPage; }
			set { CurrentPath.CurrentPage = value; }
		}

		/// <summary>The template used to serve this request.</summary>
		public PathData CurrentPath
		{
			get { return RequestItems[CurrentPathKey] as PathData ?? PathData.Empty; }
			set { RequestItems[CurrentPathKey] = value; }
		}

		/// <summary>Specifies whether the UrlAuthorizationModule should skip authorization for the current request.</summary>
		[Obsolete("Use HttpContext.SkipAuthorization")]
		public bool SkipAuthorization
		{
			get { return CurrentHttpContext.SkipAuthorization; }
			set { CurrentHttpContext.SkipAuthorization = value; }
		}

        /// <summary>The handler associated with the current request.</summary>
		[Obsolete("Use HttpContext.Handler")]
		public IHttpHandler Handler
        {
            get { return CurrentHttpContext.Handler; }
        }

        /// <summary>The current request object.</summary>
		[Obsolete("Use HttpContext.Request")]
		public HttpRequest Request
        {
            get { return CurrentHttpContext.Request; }
        }

        /// <summary>The physical path on disk to the requested resource.</summary>
		[Obsolete("Use HttpContext.Request.PhysicalPath")]
		public virtual string PhysicalPath
        {
            get { return CurrentHttpContext.Request.PhysicalPath; }
        }

		/// <summary>The host part of the requested url, e.g. http://n2cms.com/path/to/a/page.aspx?some=query.</summary>
        public Url Url
        {
			get { return new Url(HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority, HttpContext.Request.RawUrl); }
        }

        /// <summary>The current request object.</summary>
		[Obsolete("Use HttpContext.Response")]
		public HttpResponse Response
        {
            get { return CurrentHttpContext.Response; }
        }

        /// <summary>Gets the current user in the web execution context.</summary>
        public IPrincipal User
        {
            get { return CurrentHttpContext.User; }
        }

        /// <summary>Converts a virtual url to an absolute url.</summary>
        /// <param name="virtualPath">The virtual url to make absolute.</param>
        /// <returns>The absolute url.</returns>
		[Obsolete("Use N2.Web.Url.ToAbsolute(path)")]
		public virtual string ToAbsolute(string virtualPath)
        {
            return N2.Web.Url.ToAbsolute(virtualPath);
        }

        /// <summary>Converts an absolute url to an app relative url.</summary>
        /// <param name="virtualPath">The absolute url to convert.</param>
        /// <returns>An app relative url.</returns>
		[Obsolete("Use N2.Web.Url.ToRelative(path)")]
		public virtual string ToAppRelative(string virtualPath)
        {
			return N2.Web.Url.ToRelative(virtualPath);
        }

        /// <summary>Maps a virtual path to a physical disk path.</summary>
        /// <param name="path">The path to map. E.g. "~/bin"</param>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        public string MapPath(string path)
        {
			return HostingEnvironment.MapPath(path);
        }

		/// <summary>Assigns a rewrite path.</summary>
		/// <param name="path">The path to the template that will handle the request.</param>
		[Obsolete("Use HttpContext.RewritePath(path)")]
		public void RewritePath(string path)
		{
			logger.Debug("Rewriting '" + Url.LocalUrl + "' to '" + path + "'");
			CurrentHttpContext.RewritePath(path, false);
		}

		/// <summary>Assigns a rewrite path.</summary>
		/// <param name="path">The path to the template that will handle the request.</param>
		/// <param name="queryString">The query string to rewrite to.</param>
		[Obsolete("Use HttpContext.RewritePath(path, \"\", queryString)")]
		public void RewritePath(string path, string queryString)
		{
			logger.Debug("Rewriting '" + Url.LocalUrl + "' to '" + path + "'");
			CurrentHttpContext.RewritePath(path, "", queryString);
		}

		/// <summary>Calls into HttpContext.ClearError().</summary>
		[Obsolete("Use HttpContext.ClearError()")]
		public void ClearError()
		{
			CurrentHttpContext.ClearError();
		}

        /// <summary>Disposes request items that needs disposing. This method should be called at the end of each request.</summary>
		public virtual void Close()
        {
            object[] keys = new object[RequestItems.Keys.Count];
            RequestItems.Keys.CopyTo(keys, 0);

            foreach (object key in keys)
            {
                IClosable value = RequestItems[key] as IClosable;
                if (value != null)
                {
                    value.Dispose();
                }
            }
        }

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			Close();
		}

		#endregion
	}
}
