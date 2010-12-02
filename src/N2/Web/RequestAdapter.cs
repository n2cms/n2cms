using System;
using System.Security.Principal;
using System.Web;
using N2.Engine;
using N2.Security;
using N2.Web.UI;
using N2.Configuration;
using System.Web.Security;

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
			get { return securityEnforcer ?? engine.Resolve<ISecurityEnforcer>(); }
			set { securityEnforcer = value; }
		}

		public IWebContext WebContext
		{
			get { return webContext ?? engine.Resolve<IWebContext>(); }
			set { webContext = value; }
		}


		/// <summary>Rewrites a dynamic/computed url to an actual template url.</summary>
		public virtual void RewriteRequest(PathData path, RewriteMethod rewriteMethod)
		{
			if (path == null || path.IsEmpty() || !path.IsRewritable || path.Ignore)
				return;

			string templateUrl = GetHandlerPath(path);

			if(rewriteMethod == RewriteMethod.BeginRequest || rewriteMethod == RewriteMethod.SurroundMapRequestHandler)
				WebContext.RewritePath(templateUrl);
			else if(rewriteMethod == RewriteMethod.TransferRequest)
				WebContext.TransferRequest(templateUrl);
		}

        /// <summary>Gets the path to the handler (aspx template) to rewrite to.</summary>
        /// <returns></returns>
		protected virtual string GetHandlerPath(PathData path)
		{
			return path.RewrittenUrl;
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
