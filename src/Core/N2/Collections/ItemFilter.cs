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

namespace N2.Collections
{
	/// <summary>
	/// The abstract base class of item filters.
	/// </summary>
	public abstract class ItemFilter
	{
		/// <summary>Matches an item against the current filter.</summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public abstract bool Match(ContentItem item);
		
		/// <summary>Filters items by matching them against the current filter.</summary>
		/// <param name="items">The items to perform the filtering on.</param>
		public virtual void Filter(IList<ContentItem> items)
		{
			for (int i = items.Count - 1; i >= 0; i--)
				if (!Match(items[i]))
					items.RemoveAt(i);
		}

		/// <summary>Applies a filter on a list of items.</summary>
		/// <param name="items">The items to filter.</param>
		/// <param name="filter">The filter to apply.</param>
		public static void Filter(IList<ContentItem> items, ItemFilter filter)
		{
			filter.Filter(items);
		}
	}
}
