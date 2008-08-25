using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Collections;
using System.Security.Principal;
using System.Diagnostics;

namespace N2.Web
{
    /// <summary>
    /// A request context class that interacts with HttpContext.Current.
    /// </summary>
    public class WebRequestContext : IWebContext
    {
        /// <summary>Provides access to HttpContext.Current.</summary>
        protected virtual HttpContext CurrentHttpContext
        {
            get
            {
                if (HttpContext.Current == null)
                    throw new N2Exception("Tried to retrieve HttpContext.Current but it's null. This may happen when working outside a request or when doing stuff after the context has been recycled.");
                return HttpContext.Current;
            }
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
            get { return RequestItems["CurrentPage"] as ContentItem; }
            set { RequestItems["CurrentPage"] = value; }
        }

        /// <summary>The handler associated with the current request.</summary>
        public IHttpHandler Handler
        {
            get { return CurrentHttpContext.Handler; }
        }

        /// <summary>The current request object.</summary>
        public HttpRequest Request
        {
            get { return CurrentHttpContext.Request; }
        }

        /// <summary>The physical path on disk to the requested resource.</summary>
        public virtual string PhysicalPath
        {
            get { return Request.PhysicalPath; }
        }

        /// <summary>The local part of the requested path, e.g. /path/to/a/page.aspx?some=query.</summary>
        public Url LocalUrl
        {
            get { return Url.Parse(Request.RawUrl); }
        }

        /// <summary>The host part of the requested url, e.g. http://n2cms.com/.</summary>
        public Url HostUrl
        {
            get { return new Url(Request.Url.Scheme, Request.Url.Authority, null, null, null); }
        }

        /// <summary>The current request object.</summary>
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
        public virtual string ToAbsolute(string virtualPath)
        {
            return N2.Web.Url.ToAbsolute(virtualPath);
        }

        /// <summary>Converts an absolute url to an app relative url.</summary>
        /// <param name="virtualPath">The absolute url to convert.</param>
        /// <returns>An app relative url.</returns>
        public virtual string ToAppRelative(string virtualPath)
        {
            if (virtualPath != null && virtualPath.StartsWith(Url.ApplicationPath, System.StringComparison.InvariantCultureIgnoreCase))
                return "~/" + virtualPath.Substring(Url.ApplicationPath.Length);
            return virtualPath;
        }

        /// <summary>Maps a virtual path to a physical disk path.</summary>
        /// <param name="path">The path to map. E.g. "~/bin"</param>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        public string MapPath(string path)
        {
            return CurrentHttpContext.Server.MapPath(path);
        }

        /// <summary>Assigns a rewrite path.</summary>
        /// <param name="path">The path to the template that will handle the request.</param>
        public void RewritePath(string path)
        {
            Debug.WriteLine("Rewriting '" + LocalUrl + "' to '" + path + "'");
            // the whitespace is a workaround for strange IIS7 rewrite behaviour
            CurrentHttpContext.RewritePath(path + "                                        ", false);
        }

        public void TransferRequest(string path)
        {
            string url = Url.Parse(path).AppendQuery("postback", LocalUrl);
            CurrentHttpContext.Server.TransferRequest(url, true);
        }

        /// <summary>Disposes request items that needs disposing. This method should be called at the end of each request.</summary>
        public virtual void Dispose()
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
    }
}
