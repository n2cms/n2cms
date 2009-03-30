using System;
using System.Security.Principal;
using System.Web;
using N2.Engine;
using N2.Security;
using N2.Web.UI;
using N2.Configuration;

namespace N2.Web
{
	/// <summary>
	/// The default controller for N2 content items. Controls behaviour such as 
	/// rewriting, authorization and more. This class can be used as base class 
	/// for customizing the behaviour (decorate the inherited class with the 
	/// [Controls] attribute).
	/// </summary>
	[Controls(typeof(ContentItem))]
	public class RequestAdapter : AbstractContentAdapter
	{
		/// <summary>Rewrites a dynamic/computed url to an actual template url.</summary>
		public virtual void RewriteRequest(RewriteMethod rewriteMethod)
		{
			if(Path == null || Path.IsEmpty())
				return;
			
			string templateUrl = GetHandlerPath();
			if(rewriteMethod == RewriteMethod.RewriteRequest)
				Engine.Resolve<IWebContext>().RewritePath(templateUrl);
			else if(rewriteMethod == RewriteMethod.TransferRequest)
				Engine.Resolve<IWebContext>().TransferRequest(templateUrl);
		}

		[Obsolete("Please use RewriteRequest(RewriteMethod rewriteMethod)")]
		public virtual void RewriteRequest()
		{
			RewriteRequest(RewriteMethod.RewriteRequest);
		}

        /// <summary>Gets the path to the handler (aspx template) to rewrite to.</summary>
        /// <returns></returns>
		protected virtual string GetHandlerPath()
		{
			return Path.RewrittenUrl;
		}

		/// <summary>Inject the current page into the page handler.</summary>
		/// <param name="handler">The handler executing the request.</param>
		public virtual void InjectCurrentPage(IHttpHandler handler)
		{
			IContentTemplate template = handler as IContentTemplate;
			if (template != null && Path != null)
			{
				template.CurrentItem = Path.CurrentItem;
			}
		}

		/// <summary>Authorize the user against the current content item. Throw an exception if not authorized.</summary>
		/// <param name="user">The user for which to authorize the request.</param>
		public virtual void AuthorizeRequest(IPrincipal user)
		{
			Engine.Resolve<ISecurityEnforcer>().AuthorizeRequest();
		}
	}
}
