using System.Collections.Generic;
using System.Security.Principal;
using System.Web;
using N2.Security;

namespace N2.Collections
{
	/// <summary>
	/// Filter based on user access and security.
	/// </summary>
	public class AccessFilter : ItemFilter
    {
        private IPrincipal user;
        private ISecurityManager securityManager;

		public AccessFilter()
			: this(HttpContext.Current != null ? HttpContext.Current.User : null, Context.SecurityManager)
		{
		}

		public AccessFilter(IPrincipal user, ISecurityManager securityManager)
		{
			this.user = user;
			this.securityManager = securityManager;
		}

		public IPrincipal User
		{
			get { return user; }
			set { user = value; }
		}

		public ISecurityManager SecurityManager
		{
			get { return securityManager; }
			set { securityManager = value; }
		}

		public override bool Match(ContentItem item)
		{
			return SecurityManager.IsAuthorized(item, User);
		}

		#region Static Methods

		public static void Filter(IList<ContentItem> items, IPrincipal user, ISecurityManager securityManager)
		{
			Filter(items, new AccessFilter(user, securityManager));
		}

		public static void DefaultFilter(IList<ContentItem> items)
		{
			Filter(items, new AccessFilter());
		}

		#endregion
	}
}