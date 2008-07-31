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
    public class WebRequestContext : RequestContext
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

        /// <summary>Gets a dictionary of request scoped items.</summary>
        public override IDictionary RequestItems
        {
            get { return CurrentHttpContext.Items; }
        }

        /// <summary>The handler associated with this request.</summary>
        public override IHttpHandler Handler
        {
            get { return CurrentHttpContext.Handler; }
        }

        /// <summary>The current request object.</summary>
        public override HttpRequest Request
        {
            get { return CurrentHttpContext.Request; }
        }

        public override HttpCookieCollection Cookies
        {
            get { return Request.Cookies; }
        }

        /// <summary>The current request object.</summary>
        public override HttpResponse Response
        {
            get { return CurrentHttpContext.Response; }
        }

        /// <summary>Gets the current user in the web execution context.</summary>
        public override IPrincipal User
        {
            get { return CurrentHttpContext.User; }
        }

        public override string MapPath(string path)
        {
            return CurrentHttpContext.Server.MapPath(path);
        }

        public override void RewritePath(string path)
        {
            Debug.WriteLine("Rewriting '" + RawUrl + "' to '" + path + "'");
            CurrentHttpContext.RewritePath(path, false);
        }
    }
}
