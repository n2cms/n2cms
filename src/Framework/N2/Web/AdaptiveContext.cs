using System;
using System.Collections;
using System.Security.Principal;
using System.Web;
using N2.Engine;
using N2.Engine.Providers;
using System.Web.Caching;
using System.Web.Hosting;

namespace N2.Web
{
    /// <summary>
    /// A web context wrapper that maps to the web request context for calls in 
    /// web application scope and thread context when no request has been made 
    /// (e.g. when executing code in scheduled action).
    /// </summary>
    public class AdaptiveContext : IWebContext, IDisposable
    {
        readonly IWebContext thread;
        readonly IWebContext web;

        public AdaptiveContext()
        {
            thread = new ThreadContext();
            web = new WebRequestContext(new HttpContextProvider());
        }

        /// <summary>Gets wether there is a web context availabe.</summary>
        public bool IsWeb
        {
            get 
            {
                try
                {
                    return System.Web.HttpContext.Current != null 
                        && System.Web.HttpContext.Current.Request != null;
                }
                catch (HttpException)
                {
                    return false;
                }
            }
        }

        /// <summary>Returns either the web or the thread context depending on <see cref="IsWeb"/>.</summary>
        protected IWebContext CurrentContext
        {
            get { return IsWeb ? web : thread; }
        }

        public HttpContextBase HttpContext
        {
            get { return CurrentContext.HttpContext; }
        }

        /// <summary>Gets a dictionary of request scoped items.</summary>
        public IDictionary RequestItems
        {
            get { return CurrentContext.RequestItems; }
        }

        /// <summary>Gets the current user principal (may be null).</summary>
        public IPrincipal User
        {
            get { return CurrentContext.User; }
        }

        /// <summary>The current request object.</summary>
        [Obsolete("Use HttpContext.Request")]
        public HttpRequest Request
        {
            get { return CurrentContext.Request; }
        }

        /// <summary>The current response object.</summary>
        [Obsolete("Use HttpContext.Response")]
        public HttpResponse Response
        {
            get { return CurrentContext.Response; }
        }

        /// <summary>The handler associated with this request.</summary>
        [Obsolete("Use HttpContext.Handler")]
        public IHttpHandler Handler
        {
            get { return CurrentContext.Handler; }
        }

        /// <summary>A page instance stored in the request context.</summary>
        public ContentItem CurrentPage
        {
            get { return CurrentContext.CurrentPage; }
            set { CurrentContext.CurrentPage = value; }
        }

        /// <summary>The template used to serve this request.</summary>
        public PathData CurrentPath
        {
            get { return CurrentContext.CurrentPath; }
            set { CurrentContext.CurrentPath = value;}
        }

        /// <summary>Specifies whether the UrlAuthorizationModule should skip authorization for the current request.</summary>
        [Obsolete("Use HttpContext.SkipAuthorization")]
        public bool SkipAuthorization
        {
            get { return CurrentContext.SkipAuthorization; }
        }

        /// <summary>The physical path on disk to the requested page.</summary>
        [Obsolete("Use HttpContext.Request.PhysicalPath")]
        public string PhysicalPath
        {
            get { return CurrentContext.PhysicalPath; }
        }

        /// <summary>The host part of the requested url, e.g. http://n2cms.com/path/to/a/page.aspx?some=query.</summary>
        public Url Url
        {
            get { return CurrentContext.Url; }
        }

        /// <summary>Converts a virtual path to an an absolute path. E.g. ~/hello.aspx -> /MyVirtualDirectory/hello.aspx.</summary>
        /// <param name="virtualPath">The virtual url to make absolute.</param>
        /// <returns>The absolute url.</returns>
        [Obsolete("Use N2.Web.Url.ToAbsolute(path)")]
        public string ToAbsolute(string virtualPath)
        {
            return CurrentContext.ToAbsolute(virtualPath);
        }

        /// <summary>Converts an absolute url to an app relative path. E.g. /MyVirtualDirectory/hello.aspx -> ~/hello.aspx.</summary>
        /// <param name="virtualPath">The absolute url to convert.</param>
        /// <returns>An app relative url.</returns>
        [Obsolete("Use N2.Web.Url.ToRelative(path)")]
        public string ToAppRelative(string virtualPath)
        {
            return CurrentContext.ToAppRelative(virtualPath);
        }

        /// <summary>Maps a virtual path to a physical disk path.</summary>
        /// <param name="path">The path to map. E.g. "~/bin"</param>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        public string MapPath(string path)
        {
            return CurrentContext.MapPath(path);
        }

        /// <summary>Rewrites the request to the given path.</summary>
        /// <param name="path">The path to the template that will handle the request.</param>
        [Obsolete("Use HttpContext.RewritePath(path)")]
        public void RewritePath(string path)
        {
            CurrentContext.RewritePath(path);
        }

        /// <summary>Rewrites the request to the given path.</summary>
        /// <param name="path">The path to the template that will handle the request.</param>
        /// <param name="query">The query string to rewrite to.</param>
        [Obsolete("Use HttpContext.RewritePath(path, \"\", queryString)")]
        public void RewritePath(string path, string query)
        {
            CurrentContext.RewritePath(path, query);
        }

        /// <summary>Calls into HttpContext.ClearError().</summary>
        [Obsolete("Use HttpContext.ClearError()")]
        public void ClearError()
        {
            CurrentContext.ClearError();
        }

        /// <summary>Disposes request items that needs disposing. This method should be called at the end of each request.</summary>
        public void Close()
        {
            CurrentContext.Close();
        }

        /// <summary>Retrieves the http context cache.</summary>
        public Cache Cache
        {
            get { return CurrentContext.Cache; }
        }

        public VirtualPathProvider Vpp
        {
            get { return HostingEnvironment.VirtualPathProvider; }
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            Close();
        }

        #endregion
    }
}
