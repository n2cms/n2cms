using System.Security.Principal;
using N2.Security;

namespace N2.Collections
{
    /// <summary>
    /// Filters items not suitable for navigation. Basically it's a composition
    /// of page, visible, published, and access filter.
    /// </summary>
    public class NavigationFilter : AllFilter
    {
        public NavigationFilter()
            : base(new PageFilter(), new VisibleFilter(), new PublishedFilter(), new AccessFilter())
        {
        }

        public NavigationFilter(IPrincipal user, ISecurityManager securityManager)
            : base(new PageFilter(), new VisibleFilter(), new PublishedFilter(), new AccessFilter(user, securityManager))
        {
        }

        public override string ToString()
        {
            return "Navigatable" + base.ToString();
        }
    }
}
