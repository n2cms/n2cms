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
	/// Filters based on item type.
	/// </summary>
	public class TypeFilter : ItemFilter
	{
		#region Constructors
		/// <summary>Instantiates a type filter that will filter anythning but the supplied types (and subclasses).</summary>
		/// <param name="allowedTypes">Types that will match.</param>
		public TypeFilter(params Type[] allowedTypes)
		{
			this.allowedTypes = allowedTypes;
		}

		/// <summary>Instantiates a type filter that will filter anythning but the supplied types (and subclasses).</summary>
		/// <param name="allowedTypes">Types that will match.</param>
		public TypeFilter(IEnumerable<Type> allowedTypes)
		{
			this.allowedTypes = allowedTypes;
		}

		/// <summary>Instantiats a type filter that will filter based on the supplied types. The types will be kept or filtered out depending on the inverse parameter.</summary>
		/// <param name="inverse">True means that supplied types will be removed when filtering.</param>
		/// <param name="allowedTypes">The types that will match or the inverse based on the inverse parameter.</param>
		[Obsolete("Wrap i an'inverse filter instead.")]
		public TypeFilter(bool inverse, params Type[] allowedTypes)
			: this(allowedTypes)
		{
			this.inverse = inverse;
		} 
		#endregion

		#region Private Members
		private IEnumerable<Type> allowedTypes;
		private bool inverse = false; 
		#endregion

		#region Properties
		/// <summary>Gets or sets whether matching types should be removed instead beeing kept.</summary>
		public bool Inverse
		{
			get { return inverse; }
			set { inverse = value; }
		}

		/// <summary>Gets or sets types that should match and thus are kept when applying this filter. This behaviour can be inversed by setting the Inverse property to true.</summary>
		public IEnumerable<Type> AllowedTypes
		{
			get { return allowedTypes; }
			set { allowedTypes = value; }
		} 
		#endregion

		#region Methods
		public override bool Match(ContentItem item)
		{
			Type itemType = item.GetType();
			foreach (Type t in allowedTypes)
				if (t.IsAssignableFrom(itemType))
					return !inverse;
			return inverse;
		} 
		#endregion

		#region Static Methods
		public static void Filter(IList<ContentItem> items, params Type[] allowedTypes)
		{
			Filter(false, items, allowedTypes);
		}
		public static void Filter(bool inverse, IList<ContentItem> items, params Type[] allowedTypes)
		{
			ItemFilter.Filter(items, new TypeFilter(inverse, allowedTypes));
		} 
		#endregion
	}
}
