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
using System.Security.Principal;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using Castle.Core;
using System.Collections.Specialized;

namespace N2.Security
{
	/// <summary>
	/// Manages security by subscribing to persister events and providing 
	/// methods to authorize request event.
	/// </summary>
	public class SecurityManager : ISecurityManager
	{
		private Web.IWebContext webContext;
		
		private bool enabled = true;

        string[] editorNames = new string[0];
        string[] editorRoles = new string[] { "Editors" };
		string[] adminNames = new string[] { "admin" };
        string[] adminRoles = new string[] { "Administrators" };
		string[] writerNames = new string[] { "writer" };
		string[] writerRoles = new string[] { "Writers" };
		
		/// <summary>Creates a new instance of the security manager.</summary>
		[Obsolete("Don't use", true)]
		public SecurityManager(Web.IWebContext webContext)
		{
			this.webContext = webContext;
        }

        /// <summary>Creates a new instance of the security manager.</summary>
        public SecurityManager(Web.IWebContext webContext, Configuration.EditSection config)
        {
            this.webContext = webContext;

            if (config.Editors.Users != null)
                editorNames = ToArray(config.Editors.Users);
            if (config.Editors.Roles != null)
                editorRoles = ToArray(config.Editors.Roles);
			if (config.Writers.Users != null)
				writerNames = ToArray(config.Writers.Users);
			if (config.Writers.Roles != null)
				writerRoles = ToArray(config.Writers.Roles);
			if (config.Administrators.Users != null)
                adminNames = ToArray(config.Administrators.Users);
            if (config.Administrators.Roles != null)
                adminRoles = ToArray(config.Administrators.Roles);
        }

        private static string[] ToArray(StringCollection list)
        {
            string[] array = new string[list.Count];
            list.CopyTo(array, 0);
            return array;
        }

		#region Properties
		/// <summary>
		/// Gets or sets user names considered as editors.
		/// </summary>
		public string[] EditorNames
		{
			get { return editorNames; }
			set { editorNames = value; }
		}

		/// <summary>
		/// Gets or sets roles considered as editors.
		/// </summary>
        public string[] EditorRoles
		{
			get { return editorRoles; }
			set { editorRoles = value; }
		}

		/// <summary>
		/// Gets or sets roles considered as writers.
		/// </summary>
		public string[] WriterRoles
		{
			get { return writerRoles; }
			set { writerRoles = value; }
		}

		/// <summary>
		/// Gets or sets user names considered as administrators.
		/// </summary>
        public string[] AdminNames
		{
			get { return adminNames; }
			set { adminNames = value; }
		}

		/// <summary>
		/// Gets or sets roles considered as administrators.
		/// </summary>
        public string[] AdminRoles
		{
			get { return adminRoles; }
			set { adminRoles = value; }
		}

		/// <summary>Check whether an item is published, i.e. it's published and isn't expired.</summary>
		/// <param name="item">The item to check.</param>
		/// <returns>A boolean indicating whether the item is published.</returns>
		public virtual bool IsPublished(ContentItem item)
		{
			return (item.Published.HasValue && DateTime.Now >= item.Published)
				&& (!item.Expires.HasValue || DateTime.Now < item.Expires.Value);
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

		#region Methods
		/// <summary>Find out if a princpial has edit access.</summary>
		/// <param name="user">The princpial to check.</param>
		/// <returns>A boolean indicating whether the principal has edit access.</returns>
		public virtual bool IsEditor(IPrincipal user)
		{
			if (user == null)
				return false;
			else
				return IsAdmin(user) || HasName(user, this.EditorNames) || IsInRole(user, this.EditorRoles) || IsInRole(user, this.WriterRoles);
		}

		/// <summary>Find out if a princpial has admin access.</summary>
		/// <param name="user">The princpial to check.</param>
		/// <returns>A boolean indicating whether the principal has admin access.</returns>
		public virtual bool IsAdmin(IPrincipal user)
		{
			if (user == null)
				return false;
			else
				return HasName(user, this.AdminNames) || IsInRole(user, this.AdminRoles);
		}

		/// <summary>Find out if a principal is allowed to access an item.</summary>
		/// <param name="item">The item to check against.</param>
		/// <param name="principal">The principal to check for allowance.</param>
		/// <returns>True if the item has public access or the principal is allowed to access it.</returns>
		public virtual bool IsAuthorized(ContentItem item, IPrincipal principal)
		{
			if (!Enabled || !ScopeEnabled || IsAdmin(principal))
			{
				// Disabled security manager or Editor means full access
				return true;
			}
			else if (!IsEditor(principal) && !IsPublished(item))
			{
				// Non-editors cannot load unpublished items
				return false;
			}
			return item.IsAuthorized(principal);
		}

		private bool HasName(IPrincipal user, string[] names)
		{
			foreach(string name in names)
				if(user.Identity.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
					return true;
			return false;
		}

        private bool IsInRole(IPrincipal user, string[] roles)
		{
			foreach(string role in roles)
				if(user.IsInRole(role))
					return true;
			return false;
		}

        public bool IsAuthorized(IPrincipal user, IEnumerable<string> roles)
        {
            foreach (string role in roles)
                if (user.IsInRole(role) || AuthorizedRole.Everyone.Equals(role, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            return false;
        }

		#endregion

    }
}
