using System;
using System.Collections.Generic;
using System.Security.Principal;
using N2.Collections;

namespace N2.Security
{
    /// <summary>
    /// Classes implementing this interface are responsible of maintaining 
    /// security by monitoring things like page access and permissions to save.
    /// </summary>
    public interface ISecurityManager
    {
        /// <summary>Checks wether a user is an editor.</summary>
        /// <param name="principal">The user to check.</param>
        /// <returns>True if the user is an editor.</returns>
        bool IsEditor(IPrincipal principal);

        /// <summary>Checks wether a user is an administrator.</summary>
        /// <param name="principal">The user to check.</param>
        /// <returns>True if the user is an administrator.</returns>
        bool IsAdmin(IPrincipal principal);

        /// <summary>Checks wether a user is authorized to access a certain item.</summary>
        /// <param name="item">The item to check for access.</param>
        /// <param name="user">The user whose permissions to check.</param>
        /// <returns>True if the user is authorized.</returns>
        bool IsAuthorized(ContentItem item, IPrincipal user);

        /// <summary>Check whether an item is published, i.e. it's published and isn't expired.</summary>
        /// <param name="item">The item to check.</param>
        /// <returns>A boolean indicating whether the item is published.</returns>
        bool IsPublished(ContentItem item);

        /// <summary>Gets or sets wether this security manager is enabled.</summary>
        bool Enabled { get; set; }

        /// <summary>Gets or sets wether this security manager is enabled in the current scope. In a web application this is equivalent to a request.</summary>
        bool ScopeEnabled { get; set; }

        /// <summary>Find out if a principal has a certain permission by default.</summary>
        /// <param name="user">The principal to check for allowance.</param>
        /// <param name="permission">The type of permission to map against.</param>
        /// <returns>True if the system is configured to allow the user to the given permission.</returns>
        bool IsAuthorized(IPrincipal user, Permission permission);

        /// <summary>Find out if a principal has a certain permission for an item.</summary>
        /// <param name="item">The item to check against.</param>
        /// <param name="user">The principal to check for allowance.</param>
        /// <param name="permission">The type of permission to map against.</param>
        /// <returns>True if the item has public access or the principal is allowed to access it.</returns>
        bool IsAuthorized(IPrincipal user, ContentItem item, Permission permission);

        /// <summary>Copies permissions from the source to the destination.</summary>
        /// <param name="source">The item whose permissions to carry over.</param>
        /// <param name="destination">The item whose permissions will be modified.</param>
        void CopyPermissions(ContentItem source, ContentItem destination);

        /// <summary>Gets the permissions for a certain user towards an item.</summary>
        /// <param name="user">The user whose permissoins to get.</param>
        /// <param name="item">The item for which permissions should be retrieved.</param>
        /// <returns>A permission flag.</returns>
        Permission GetPermissions(IPrincipal user, ContentItem item);

        /// <summary>Gets a filter that filters for the given permission.</summary>
        /// <param name="permission">The permission required.</param>
        /// <returns>An item filter.</returns>
        ItemFilter GetAuthorizationFilter(Permission permission);
    }
}
