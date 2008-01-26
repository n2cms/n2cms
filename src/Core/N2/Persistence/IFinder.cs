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
using System.Text;

namespace N2.Persistence
{
	/// <summary>Finds items based on property/detail values.</summary>
	public interface IFinder
	{
		/// <summary>Sets wether the search result may be cached. Defaults to true.</summary>
		/// <param name="enableCache">True if the query can be cached.</param>
		/// <returns>The same relation finder.</returns>
		IFinder SetCachable(bool enableCache);

		/// <summary>The first item in the list to return. This is used for paging. Defaults to 0.</summary>
		/// <param name="firstResult">The first result to return.</param>
		/// <returns>The same relation finder.</returns>
		IFinder SetFirstResult(int firstResult);

		/// <summary>The maximum number of items in the list to return. This is used for paging. Defaults to infinite.</summary>
		/// <param name="maxResult">The number of items to return.</param>
		/// <returns>The same relation finder.</returns>
		IFinder SetMaxResults(int maxResult);

		/// <summary>Sets the sort expression to use with the query. The expression is SQL-inspired, e.g. "Published DESC"</summary>
		/// <param name="sortExpression">The sort expression to use.</param>
		/// <returns>The same relation finder.</returns>
		IFinder SetSortExpression(string sortExpression);

		/// <summary>Search values with a given name.</summary>
		/// <param name="name">The name of the property or detail to search among.</param>
		/// <param name="comparison">The type of comparison to use. As you might expect the like comparison is only supported for string values and the greater than, etc., for values that are comparable that way.</param>
		/// <param name="value">The value to search for or compare to. When using the Like operator the % can be used for wildcard.</param>
		/// <returns>The same finder instance.</returns>
		IFinder SetExpression(string name, Comparison comparison, object value);

		/// <summary>Search values of the same type as the given value.</summary>
		/// <param name="comparison">The type of comparison to use. As you might expect the like comparison is only supported for string values and the greater than, etc., for values that are comparable that way.</param>
		/// <param name="value">The value to search for or compare to. When using the Like operator the % can be used for wildcard.</param>
		/// <returns>The same finder instance.</returns>
		IFinder SetNamelessDetailExpression(Comparison comparison, object value);

		/// <summary>Lists items matching the given expresson.</summary>
		/// <returns>A list of items matching the query.</returns>
		IList<ContentItem> List();
	}
}
