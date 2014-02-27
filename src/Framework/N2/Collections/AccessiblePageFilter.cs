using System.Security.Principal;
using N2.Security;

namespace N2.Collections
{
    /// <summary>
    /// Filters parts and unauthorized or unpublished items.
    /// </summary>
    public class AccessiblePageFilter : AllFilter
    {
        public AccessiblePageFilter()
            : base(new PageFilter(), new PublishedFilter(), new AccessFilter())
        {
        }

        public AccessiblePageFilter(IPrincipal user, ISecurityManager securityManager)
            : base(new PageFilter(), new PublishedFilter(), new AccessFilter(user, securityManager))
        {
        }

        public override string ToString()
        {
            return "AsccessiblePage";
        }
    }
}
