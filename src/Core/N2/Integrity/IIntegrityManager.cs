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
using System.Web;

namespace N2.Integrity
{
	/// <summary>
	/// Classes implementing this interface are responsible of the integrity 
	/// between content items in a N2 cms systems. This probably involves 
	/// checking which items may be placed below which and ensuring that no 
	/// items are placed below themselves.
	/// </summary>
	public interface IIntegrityManager
	{
		/// <summary>Check wether a copy operation is allowed.</summary>
		/// <param name="source">The item to copy.</param>
		/// <param name="destination">The destination for the copied item.</param>
		/// <returns>True if the item can becopied.</returns>
		bool CanCopy(ContentItem source, ContentItem destination);

		/// <summary>Checks wether an item can be deleted</summary>
		/// <param name="item">The item to delete.</param>
		/// <returns>True if the item can be deleted.</returns>
		bool CanDelete(ContentItem item);

		/// <summary>Check wether a move operation is allowed.</summary>
		/// <param name="source">The item to move.</param>
		/// <param name="destination">The parent item where the item will be moved to.</param>
		/// <returns>True if the item can be moved.</returns>
		bool CanMove(ContentItem source, ContentItem destination);

		/// <summary>Checks wether an item can be saved.</summary>
		/// <param name="item">The item to save.</param>
		/// <returns>True if the item can be saved.</returns>
		bool CanSave(ContentItem item);

		/// <summary>Checks wether an item's name is locally unique, i.e. no other sibling has the same name.</summary>
		/// <param name="name">The name we're proposing for the item.</param>
		/// <param name="item">The item whose siblings to check.</param>
		/// <returns>True if the item would get a unique name.</returns>
		bool IsLocallyUnique(string name, ContentItem item);

		N2Exception GetMoveException(ContentItem source, ContentItem destination);
		N2Exception GetCopyException(ContentItem source, ContentItem destination);
		N2Exception GetDeleteException(ContentItem item);
		N2Exception GetSaveException(ContentItem item);
	}
}
