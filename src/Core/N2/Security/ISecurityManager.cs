#region License
/* Copyright (C) 2006 Cristian Libardo
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.
 */
#endregion

using System;
using System.Security.Principal;
using System.Web;
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
		/// <param name="principal">The user whose permissions to check.</param>
		/// <returns>True if the user is authorized.</returns>
		bool IsAuthorized(ContentItem item, IPrincipal principal);

		/// <summary>Check whether an item is published, i.e. it's published and isn't expired.</summary>
		/// <param name="item">The item to check.</param>
		/// <returns>A boolean indicating whether the item is published.</returns>
		bool IsPublished(ContentItem item);

		/// <summary>Gets or sets wether this security manager is enabled.</summary>
		bool Enabled { get; set; }

		/// <summary>Gets or sets wether this security manager is enabled in the current scope. In a web application this is equivalent to a request.</summary>
		bool ScopeEnabled { get; set; }
	}
}
