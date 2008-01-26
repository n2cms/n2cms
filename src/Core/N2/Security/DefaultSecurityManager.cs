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

namespace N2.Security
{
	/// <summary>
	/// Manages security by subscribing to persister events and providing 
	/// methods to authorize request event.
	/// </summary>
	public class DefaultSecurityManager : ISecurityManager
	{
		#region Private Fields
		private Web.IWebContext webContext;
		
		private bool enabled = true;

		IList editorNames = new List<string>();
		IList editorRoles = new List<string>();
		IList adminNames = new List<string>();
		IList adminRoles = new List<string>();
		#endregion

		#region Constructor & Initialize
		/// <summary>Creates a new instance of the security manager.</summary>
		public DefaultSecurityManager(Web.IWebContext webContext)
		{
			this.webContext = webContext;

			this.AdminNames = new string[] { "admin" };
			this.EditorNames = this.AdminNames;
			this.AdminRoles = new string[] { "Administrators" };
			this.EditorRoles = new string[] { "Administrators", "Editors" };
		}

		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets user names considered as editors.
		/// </summary>
		public IList EditorNames
		{
			get { return editorNames; }
			set { editorNames = value; }
		}

		/// <summary>
		/// Gets or sets roles considered as editors.
		/// </summary>
		public IList EditorRoles
		{
			get { return editorRoles; }
			set { editorRoles = value; }
		}

		/// <summary>
		/// Gets or sets user names considered as administrators.
		/// </summary>
		public IList AdminNames
		{
			get { return adminNames; }
			set { adminNames = value; }
		}

		/// <summary>
		/// Gets or sets roles considered as administrators.
		/// </summary>
		public IList AdminRoles
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
				return IsAdmin(user) || HasName(user, this.EditorNames) || IsInRole(user, this.EditorRoles);
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

		private bool HasName(IPrincipal user, IEnumerable names)
		{
			foreach(string name in names)
				if(user.Identity.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
					return true;
			return false;
		}

		private bool IsInRole(IPrincipal user, IEnumerable roles)
		{
			foreach(string role in roles)
				if(user.IsInRole(role))
					return true;
			return false;
		}
		#endregion

	}
}
