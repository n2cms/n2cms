using System.Security.Principal;
using System;

namespace N2.Tests.Fakes
{
    /// <summary>
    /// A wrapper for static web methods.
    /// </summary>
    public class FakeWebContextWrapper : N2.Web.ThreadContext
    {
        public FakeWebContextWrapper()
        {
        }
        public FakeWebContextWrapper(FakeHttpContext httpContext)
        {
            this.httpContext = httpContext;
        }
        public FakeWebContextWrapper(string currentUrl)
        {
            Url = currentUrl;
			httpContext = new FakeHttpContext(currentUrl);
        }

        public FakeHttpContext httpContext = new FakeHttpContext();
        public override System.Web.HttpContextBase HttpContext
        {
            get { return httpContext; }
        }

        public IPrincipal currentUser = SecurityUtilities.CreatePrincipal("admin");
        public override IPrincipal User
        {
            get { return currentUser; }
        }
        [Obsolete("Use N2.Web.Url.ToAbsolute(path)")]
        public override string ToAbsolute(string virtualPath)
        {
            return virtualPath.TrimStart('~');
        }
        [Obsolete("Use N2.Web.Url.ToRelative(path)")]
        public override string ToAppRelative(string virtualPath)
        {
            return virtualPath;
        }

        public string rewrittenPath { get { return httpContext.request.rawUrl; } }

        public bool isWeb;
        public override bool IsWeb
        {
            get { return isWeb; }
        }
    }
}
