using System.Security.Principal;
using N2.Definitions;
using N2.Plugin;
using System.Collections;
using System;
using N2.Engine.Globalization;
using System.Diagnostics;
using N2.Engine;

namespace N2.Security
{
    public static class SecurableExtensions
    {
        /// <summary>Checks whether the user is authorized to use a editable and has sufficient permissions.</summary>
        /// <param name="security">The security manager to query permissions on.</param>
        /// <param name="permittableOrSecurable">The object containing security information.</param>
        /// <param name="user">The user to check permissions for.</param>
        /// <param name="item">The item to check permissions for.</param>
        /// <returns>True if the user is authorized for the given editable.</returns>
        public static bool IsAuthorized(this ISecurityManager security, ISecurableBase permittableOrSecurable, IPrincipal user, ContentItem item)
        {
            return user != null && IsAuthorized(permittableOrSecurable, user, item) && IsPermitted(security, permittableOrSecurable, user, item);
        }

        /// <summary>Disables security checks until the returned object is disposed or the request ends.</summary>
        /// <param name="security">The security manager to disable.</param>
        /// <param name="isDisabled">True disables the security manager.</param>
        /// <returns>An object that resets the enabled state when disposed.</returns>
        public static IDisposable Disable(this ISecurityManager security, bool isDisabled = true)
        {
            bool previous = security.ScopeEnabled;
            security.ScopeEnabled = !isDisabled;
            Logger.Debug("Disabling security " + isDisabled + " [");
            Logger.Indent();
            return new Scope(() => 
            {
                security.ScopeEnabled = previous;
                Logger.Unindent();
                Logger.Debug("] Reenabling security " + previous);
            });
        }

        private static bool IsPermitted(ISecurityManager security, object possiblyPermittable, IPrincipal user, ContentItem item)
        {
            var permittable = possiblyPermittable as IPermittable;
            if (permittable != null && permittable.RequiredPermission > Permission.Read && !security.IsAuthorized(user, item, permittable.RequiredPermission))
                return false;
            return true;
        }

        private static bool IsAuthorized(object possiblySecurable, IPrincipal user, ContentItem item)
        {
            var securable = possiblySecurable as ISecurable;
            if (securable != null && securable.AuthorizedRoles != null && !PermissionMap.IsInRoles(user, securable.AuthorizedRoles))
                return false;
            return true;
        }
    }
}
