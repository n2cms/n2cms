using System;
using System.Security.Principal;
using System.Web;
using N2.Security;
using N2.Web.UI;

namespace N2.Web
{
	/// <summary>
	/// The default controller for N2 content items. Controls behaviour such as 
	/// rewriting, authorization and more. This class can be used as base class 
	/// for customizing the behaviour (decorate the inherited class with the 
	/// [Controls] attribute).
	/// </summary>
	public class BaseController
	{
		/// <summary>The path associated with this controller instance.</summary>
		public PathData Path { get; set; }

		/// <summary>Rewrites a dynamic/computed url to an actual template url.</summary>
		public virtual void RewriteRequest(IWebContext webContext)
		{
			if(!Path.IsEmpty())
				webContext.RewritePath(Path.RewrittenUrl);
		}

		/// <summary>Inject the current page into the page handler.</summary>
		/// <param name="handler"></param>
		public virtual void InjectCurrentPage(IHttpHandler handler)
		{
			IContentTemplate template = handler as IContentTemplate;
			if (template != null)
			{
				template.CurrentItem = Path.CurrentItem;
			}
		}

		/// <summary>Authorize the user against the current content item. Throw an exception if not authorized.</summary>
		/// <param name="security"></param>
		public virtual void AuthorizeRequest(IPrincipal user, ISecurityEnforcer security)
		{
			security.AuthorizeRequest();
		}

		/// <summary>Is notified when an unhandled error occurs.</summary>
		/// <param name="ex">The thrown exception.</param>
		public virtual void HandleError(Exception ex)
		{
			// do nothing special here
		}
	}
}
