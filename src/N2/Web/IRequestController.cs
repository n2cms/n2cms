using System;
using System.Security.Principal;
using System.Web;
using N2.Security;

namespace N2.Web
{
	/// <summary>
	/// The interface for controllers dealing with a web request.
	/// </summary>
	public interface IRequestController : IContentController
	{
		/// <summary>Rewrites a dynamic/computed url to an actual template url.</summary>
		void RewriteRequest(IWebContext webContext);

		/// <summary>Inject the current page into the page handler.</summary>
		/// <param name="handler"></param>
		void InjectCurrentPage(IHttpHandler handler);

		/// <summary>Authorize the user against the current content item. Throw an exception if not authorized.</summary>
		/// <param name="security"></param>
		void AuthorizeRequest(IPrincipal user, ISecurityEnforcer security);

		/// <summary>Is notified when an unhandled error occurs.</summary>
		/// <param name="ex">The thrown exception.</param>
		void HandleError(Exception ex);
	}
}