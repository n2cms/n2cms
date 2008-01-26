#region License

/* Copyright (C) 2007 Cristian Libardo
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 */

#endregion

using System.Collections.Generic;

namespace N2.Collections
{
	/// <summary>
	/// A list of content items. Provides easier access to filtering and sorting.
	/// </summary>
	public class ItemList : ItemList<ContentItem>
	{
		#region Constructors

		/// <summary>Initializes an empty instance of the ItemList class.</summary>
		public ItemList()
		{
		}

		/// <summary>Initializes an instance of the ItemList class adding the items matching the supplied filter.</summary>
		/// <param name="items">The gross enumeration of items to initialize with.</param>
		/// <param name="filters">The filters that should be applied to the gross collection.</param>
		public ItemList(IEnumerable<ContentItem> items, params ItemFilter[] filters) : base(items, filters)
		{
		}

		/// <summary>Initializes an instance of the ItemList class adding the items matching the supplied filter.</summary>
		/// <param name="items">The gross enumeration of items to initialize with.</param>
		/// <param name="filters">The filters that should be applied to the gross collection.</param>
		public ItemList(IEnumerable<ContentItem> items, IEnumerable<ItemFilter> filters) : base(items, filters)
		{
		}

		#endregion
	}
}