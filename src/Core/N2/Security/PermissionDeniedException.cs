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
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;

namespace N2.Security
{
	/// <summary>
	/// An exeption thrown when a user tries to access an unauthorized item.
	/// </summary>
	public class PermissionDeniedException : N2Exception
	{
		public PermissionDeniedException(ContentItem item, IPrincipal user)
			: base("Permission denied")
		{
			this.user = user;
			this.item = item;
		}

		#region Private Members
		private ContentItem item;
		private IPrincipal user; 
		#endregion

		#region Properties
		/// <summary>Gets the user which caused the exception.</summary>
		public IPrincipal User
		{
			get { return user; }
		}

		/// <summary>Gets the item that caused the exception.</summary>
		public ContentItem Item
		{
			get { return item; }
		} 
		#endregion
	}
}
