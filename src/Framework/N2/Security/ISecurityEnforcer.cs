using System;
using System.Security.Principal;

namespace N2.Security
{
    /// <summary>
    /// Is responsible of securing the web site.
    /// </summary>
    public interface ISecurityEnforcer
    {
        /// <summary>
        /// Is invoked when a security violation is encountered. The security 
        /// exception can be cancelled by setting the cancel property on the event 
        /// arguments.
        /// </summary>
        event EventHandler<CancellableItemEventArgs> AuthorizationFailed;

        void AuthorizeRequest(IPrincipal user, ContentItem page, Permission requiredPermission);

        void Start();
    }
}
