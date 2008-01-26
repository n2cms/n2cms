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
using System.Collections;
using System.Collections.Generic;

namespace N2.Collections
{
	/// <summary>
	/// Compares two content items based on a detail. This class can compare 
	/// classes given a expression.
	/// </summary>
	public class ItemComparer : ItemComparer<ContentItem>, IComparer
	{
		#region Constructors
		/// <summary>Creates a new instance of the ItemComparer that sorts on the item's sort order.</summary>
		public ItemComparer() 
			: this("SortOrder")
		{
		}

		/// <summary>Creates a new instance of the ItemComparer that sorts using a custom sort expression.</summary>
		/// <param name="sortExpression">The name of the property to sort on. DESC can be appended to the string to reverse the sort order.</param>
		public ItemComparer(string sortExpression) 
			: base(sortExpression)
		{
		}

		/// <summary>Creates a new instance of the ItemComparer that sorts using sort property and direction.</summary>
		/// <param name="detailToCompare">The name of the property to sort on.</param>
		/// <param name="inverse">Wether the comparison should be "inverse", i.e. make Z less than A.</param>
		public ItemComparer(string detailToCompare, bool inverse)
			: base(detailToCompare, inverse)
		{
		}
		#endregion
	}
}
