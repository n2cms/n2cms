using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Collections;
using System.Collections.Specialized;

namespace N2.Web
{
    /// <summary>
    /// A web context wrapper that maps to the web request context for calls in 
    /// web application scope and thread context when no request has been made 
    /// (e.g. when executing code in scheduled action).
    /// </summary>
    public class AdaptiveContext : IWebContext
    {
        IWebContext thread = new ThreadContext();
        IWebContext web = new WebRequestContext();

        public AdaptiveContext()
        {
        }

        public bool IsWeb
        {
            get { return HttpContext.Current != null; }
        }

        protected IWebContext CurrentContext 
        {
            get { return IsWeb ? web : thread; }
        }

        public IDictionary RequestItems
        {
            get { return CurrentContext.RequestItems; }
        }

        public System.Security.Principal.IPrincipal User
        {
            get { return CurrentContext.User; }
        }

        public System.Web.HttpRequest Request
        {
            get { return CurrentContext.Request; }
        }

        public System.Web.HttpResponse Response
        {
            get { return CurrentContext.Response; }
        }

        public System.Web.IHttpHandler Handler
        {
            get { return CurrentContext.Handler; }
        }

        public ContentItem CurrentPage
        {
            get { return CurrentContext.CurrentPage; }
            set { CurrentContext.CurrentPage = value; }
        }

        public string Authority
        {
            get { return CurrentContext.Authority; }
        }

        public string RawUrl
        {
            get { return CurrentContext.RawUrl; }
        }

        public string AbsolutePath
        {
            get { return CurrentContext.AbsolutePath; }
        }

        public System.Web.HttpCookieCollection Cookies
        {
            get { return CurrentContext.Cookies; }
        }

        public string ApplicationUrl
        {
            get { return CurrentContext.ApplicationUrl; }
        }

        public string Query
        {
            get { return CurrentContext.Query; }
        }

        public NameValueCollection QueryString
        {
            get { return CurrentContext.QueryString; }
        }

        public string PhysicalPath
        {
            get { return CurrentContext.PhysicalPath; }
        }

        public string ToAbsolute(string virtualPath)
        {
            return CurrentContext.ToAbsolute(virtualPath);
        }

        public string ToAppRelative(string virtualPath)
        {
            return CurrentContext.ToAppRelative(virtualPath);
        }

        public string MapPath(string path)
        {
            return CurrentContext.MapPath(path);
        }

        public void RewritePath(string path)
        {
            CurrentContext.RewritePath(path);
        }

        public void Dispose()
        {
            CurrentContext.Dispose();
        }
    }
}
