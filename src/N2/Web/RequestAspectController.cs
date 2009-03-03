using System;
using System.Security.Principal;
using System.Web;
using N2.Engine;
using N2.Security;
using N2.Web.UI;
using N2.Engine.Aspects;

namespace N2.Web
{
	/// <summary>
	/// The default controller for N2 content items. Controls behaviour such as 
	/// rewriting, authorization and more. This class can be used as base class 
	/// for customizing the behaviour (decorate the inherited class with the 
	/// [Controls] attribute).
	/// </summary>
	[Controls(typeof(ContentItem))]
	public class RequestAspectController : IAspectController
	{
		/// <summary>The path associated with this controller instance.</summary>
		public PathData Path { get; set; }

		/// <summary>The content engine requesting control. TODO: support dependency injection.</summary>
		public IEngine Engine { get; set; }

		/// <summary>Rewrites a dynamic/computed url to an actual template url.</summary>
		public virtual void RewriteRequest()
		{
			if(Path != null && !Path.IsEmpty())
				Engine.Resolve<IWebContext>().RewritePath(Path.RewrittenUrl);
		}

		/// <summary>Inject the current page into the page handler.</summary>
		/// <param name="handler"></param>
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
