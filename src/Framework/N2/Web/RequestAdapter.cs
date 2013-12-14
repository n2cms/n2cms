using System.Security.Principal;
using System.Web;
using N2.Configuration;
using N2.Engine;
using N2.Security;
using N2.Web.UI;
using N2.Web.Targeting;
using System.Web.Hosting;

namespace N2.Web
{
    /// <summary>
    /// The default controller for N2 content items. Controls behaviour such as 
    /// rewriting, authorization and more. This class can be used as base class 
    /// for customizing the behaviour (decorate the inherited class with the 
    /// [Adapts] attribute).
    /// </summary>
    [Adapts(typeof(ContentItem))]
    public class RequestAdapter : AbstractContentAdapter
    {
        IWebContext webContext;
        ISecurityEnforcer securityEnforcer;

        public ISecurityEnforcer SecurityEnforcer
        {
            get { return securityEnforcer ?? Engine.Resolve<ISecurityEnforcer>(); }
            set { securityEnforcer = value; }
        }

        public IWebContext WebContext
        {
            get { return webContext ?? Engine.Resolve<IWebContext>(); }
            set { webContext = value; }
        }


        /// <summary>Rewrites a dynamic/computed url to an actual template url.</summary>
        public virtual void RewriteRequest(PathData path, RewriteMethod rewriteMethod)
        {
            if (path == null || path.IsEmpty() || !path.IsRewritable || path.Ignore || rewriteMethod == RewriteMethod.None)
                return;

            string templateUrl = GetHandlerPath(path);
            WebContext.HttpContext.RewritePath(templateUrl, false);
        }

        /// <summary>Gets the path to the handler (aspx template) to rewrite to.</summary>
        /// <returns></returns>
        protected virtual string GetHandlerPath(PathData path)
        {
            var templateUrl = path.GetRewrittenUrl();
            return ResolveTargetingUrl(templateUrl);
        }

        /// <summary>Retrieves the first targeting path which exists according to the virtual path provider or default.</summary>
        /// <param name="defaultUrl"></param>
        /// <returns></returns>
        public virtual string ResolveTargetingUrl(string defaultUrl)
        {
            var ctx = WebContext.HttpContext.GetTargetingContext(Engine);
            foreach (var alternativeUrl in ctx.GetTargetedPaths(defaultUrl))
                if (WebContext.Vpp.FileExists(Url.PathPart(alternativeUrl)))
                    return alternativeUrl;
            return defaultUrl;
        }

        /// <summary>Inject the current page into the page handler.</summary>
        /// <param name="handler">The handler executing the request.</param>
        public virtual void InjectCurrentPage(PathData path, IHttpHandler handler)
        {
            IContentTemplate template = handler as IContentTemplate;
            if (template != null && path != null)
            {
                template.CurrentItem = path.CurrentPage;
            }
        }

        /// <summary>Authorize the user against the current content item. Throw an exception if not authorized.</summary>
        /// <param name="user">The user for which to authorize the request.</param>
        public virtual void AuthorizeRequest(PathData path, IPrincipal user)
        {
            SecurityEnforcer.AuthorizeRequest(user, path.CurrentPage, Permission.Read);
        }
    }
}
