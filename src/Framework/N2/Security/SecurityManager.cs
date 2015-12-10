#region License
/* Copyright (C) 2007 Cristian Libardo
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 *
 * This software is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this software; if not, write to the Free
 * Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
 * 02110-1301 USA, or see the FSF site: http://www.fsf.org.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using N2.Engine;
using N2.Collections;

namespace N2.Security
{
    /// <summary>
    /// Manages security by subscribing to persister events and providing 
    /// methods to authorize request event.
    /// </summary>
    [Service(typeof(ISecurityManager))]
    public class SecurityManager : ISecurityManager
    {
        static string[] defaultAdministratorRoles = new[] { "Administrators" };
        static string[] defaultAdministratorUsers = new[] { "admin" };
        static string[] defaultEditorRoles = new[] { "Editors" };
        static string[] defaultWriterRoles = new[] { "Writers" };
        static string[] none = new string[0];

        private Web.IWebContext webContext;
        
        private bool enabled = true;

        public PermissionMap Administrators { get; set; }
        public PermissionMap Editors { get; set; }
        public PermissionMap Writers { get; set; }

        /// <summary>Creates a new instance of the security manager.</summary>
        public SecurityManager(Web.IWebContext webContext, Configuration.EditSection config)
        {
            this.webContext = webContext;

            Administrators = config.Administrators.ToPermissionMap(Permission.Full, defaultAdministratorRoles, defaultAdministratorUsers);
            Editors = config.Editors.ToPermissionMap(Permission.ReadWritePublish, defaultEditorRoles, none);
            Writers = config.Writers.ToPermissionMap(Permission.ReadWrite, defaultWriterRoles, none);
        }

        #region Properties
        /// <summary>
        /// Gets user names considered as editors.
        /// </summary>
        [Obsolete("About to be gone")]
        public string[] EditorNames
        {
            get { return Editors.Users; }
        }

        /// <summary>
        /// Gets roles considered as editors.
        /// </summary>
        [Obsolete("About to be gone")]
        public string[] EditorRoles
        {
            get { return Editors.Roles; }
        }

        /// <summary>
        /// Gets roles considered as writers.
        /// </summary>
        [Obsolete("About to be gone")]
        public string[] WriterRoles
        {
            get { return Writers.Roles; }
        }

        /// <summary>
        /// Gets user names considered as administrators.
        /// </summary>
        [Obsolete("About to be gone")]
        public string[] AdminNames
        {
            get { return Administrators.Users; }
        }

        /// <summary>
        /// Gets or sets roles considered as administrators.
        /// </summary>
        [Obsolete("About to be gone")]
        public string[] AdminRoles
        {
            get { return Administrators.Roles; }
        }

        /// <summary>Check whether an item is published, i.e. it's published and isn't expired.</summary>
        /// <param name="item">The item to check.</param>
        /// <returns>A boolean indicating whether the item is published.</returns>
        public virtual bool IsPublished(ContentItem item)
        {
            return item.IsPublished();
        }

        /// <summary>Gets or sets whether the security manager is enabled.</summary>
        public virtual bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        /// <summary>Gets or sets whether the security manager is enabled in the current scope. This can be used to override the security features in certain situations.</summary>
        public virtual bool ScopeEnabled
        {
            get
            {
                return !webContext.RequestItems.Contains("ItemSecurityManager.ScopeEnabled");
            }
            set
            {
                if (value && webContext.RequestItems.Contains("ItemSecurityManager.ScopeEnabled"))
                    webContext.RequestItems.Remove("ItemSecurityManager.ScopeEnabled");
                else if (!value)
                    webContext.RequestItems["ItemSecurityManager.ScopeEnabled"] = false;
            }
        }

        #endregion

        #region ISecurityManager Members
        /// <summary>Find out if a princpial has edit access.</summary>
        /// <param name="user">The princpial to check.</param>
        /// <returns>A boolean indicating whether the principal has edit access.</returns>
        public virtual bool IsEditor(IPrincipal user)
        {
            return IsAdmin(user) || Editors.Contains(user) || Writers.Contains(user);
        }

        /// <summary>Find out if a princpial has admin access.</summary>
        /// <param name="user">The princpial to check.</param>
        /// <returns>A boolean indicating whether the principal has admin access.</returns>
        public virtual bool IsAdmin(IPrincipal user)
        {
            return Administrators.Contains(user);
        }

        /// <summary>Find out if a principal is allowed to access an item.</summary>
        /// <param name="item">The item to check against.</param>
        /// <param name="user">The principal to check for allowance.</param>
        /// <returns>True if the item has public access or the principal is allowed to access it.</returns>
        public virtual bool IsAuthorized(ContentItem item, IPrincipal user)
        {
            if (!Enabled || !ScopeEnabled || IsAdmin(user))
            {
                // Disabled security manager or Editor means full access
                return true;
            }
            else if (!IsEditor(user) && !IsPublished(item))
            {
                // Non-editors cannot load unpublished items
                return false;
            }
            return item.IsAuthorized(user);
        }


        /// <summary>Find out if a principal has a certain permission by default.</summary>
        /// <param name="user">The principal to check for allowance.</param>
        /// <param name="permission">The type of permission to map against.</param>
        /// <returns>True if the system is configured to allow the user to the given permission.</returns>
        public virtual bool IsAuthorized(IPrincipal user, Permission permission)
        {
            return (Administrators.MapsTo(permission) && Administrators.Contains(user))
               || (Editors.MapsTo(permission) && Editors.Contains(user))
               || (Writers.MapsTo(permission) && Writers.Contains(user));
        }

        /// <summary>Find out if a principal has a certain permission for an item.</summary>
        /// <param name="item">The item to check against.</param>
        /// <param name="user">The principal to check for allowance.</param>
        /// <param name="permission">The type of permission to map against.</param>
        /// <returns>True if the item has public access or the principal is allowed to access it.</returns>
        public virtual bool IsAuthorized(IPrincipal user, ContentItem item, Permission permission)
        {
            if (permission == Permission.None)
                return true;
            if (item == null)
                return IsAuthorized(user, permission);
            if (permission == Permission.Read)
                return IsAuthorized(item, user);

            foreach (PermissionRemapAttribute remap in item.GetContentType().GetCustomAttributes(typeof(PermissionRemapAttribute), true))
                permission = remap.Remap(permission);

            return Administrators.Authorizes(user, item, permission)
                   || Editors.Authorizes(user, item, permission)
                   || Writers.Authorizes(user, item, permission);
        }

        [Obsolete("Use PermissionMap.IsInRoles")]
        public bool IsAuthorized(IPrincipal user, IEnumerable<string> roles)
        {
            foreach (string role in roles)
                if (user.IsInRole(role) || AuthorizedRole.Everyone.Equals(role, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            return false;
        }

        static Permission[] permissions = new[] { Permission.Read, Permission.Write, Permission.Publish, Permission.Administer };

        /// <summary>Copies permissions from the source to the destination.</summary>
        /// <param name="source">The item whose permissions to carry over.</param>
        /// <param name="destination">The item whose permissions will be modified.</param>
        public void CopyPermissions(ContentItem source, ContentItem destination)
        {
            foreach (Permission p in permissions)
            {
                var roles = DynamicPermissionMap.GetRoles(source, p);
                if (roles == null)
                    continue;
                DynamicPermissionMap.SetRoles(destination, p, roles.ToArray());
            }
        }

        /// <summary>Gets the permissions for a certain user towards an item.</summary>
        /// <param name="user">The user whose permissoins to get.</param>
        /// <param name="item">The item for which permissions should be retrieved.</param>
        /// <returns>A permission flag.</returns>
        public virtual Permission GetPermissions(IPrincipal user, ContentItem item)
        {
            return GetPermiossions(user, item, Administrators)
                | GetPermiossions(user, item, Editors)
                | GetPermiossions(user, item, Writers)
                | (item.IsAuthorized(user) ? Permission.Read : Permission.None);
        }

        protected Permission GetPermiossions(IPrincipal user, ContentItem item, PermissionMap map)
        {
            return map.Authorizes(user, item, map.Permissions) ? map.Permissions : Permission.None;
        }

        
        /// <summary>Gets a filter that filters for the given permission.</summary>
        /// <param name="permission">The permission required.</param>
        /// <returns>An item filter.</returns>
        public Collections.ItemFilter GetAuthorizationFilter(Permission permission)
        {
            if (permission == Permission.Read)
                return new AccessFilter(webContext.User, this);
            if (permission == Permission.None)
                return new NullFilter();

            return new DelegateFilter(ci => IsAuthorized(webContext.User, permission));
        }

        #endregion
    }
}
