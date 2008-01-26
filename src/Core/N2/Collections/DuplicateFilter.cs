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

namespace N2.Collections
{
	/// <summary>
	/// A filter that removes duplicated by keeping track of already added 
	/// items and only matching new items.
	/// </summary>
	public class DuplicateFilter : ItemFilter
	{
		#region Private Fields
		private Dictionary<int, bool> existing = new Dictionary<int, bool>();	
		#endregion

		#region Public Methods
		public override bool Match(N2.ContentItem item)
		{
			if (existing.ContainsKey(item.ID))
				return false;

			existing.Add(item.ID, true);
			return true;
		}

		public void Clear()
		{
			existing.Clear();
		} 
		#endregion

		/// <summary>Removes duplicate items.</summary>
		/// <param name="items">The items whose duplicate items should be deleted.</param>
		public static void FilterDuplicates(IList<ContentItem> items)
		{
			ItemFilter.Filter(items, new DuplicateFilter());
		}
	}
}