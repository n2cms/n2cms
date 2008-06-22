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

using System.Collections;
using System.Collections.Generic;
using System.Web.UI;

namespace N2.Collections
{
	/// <summary>
	/// A generic item list.
	/// </summary>
	/// <typeparam name="T">The type of item to list.</typeparam>
	public class ItemList<T> : List<T>, IHierarchicalEnumerable where T : ContentItem
	{
		#region Constructors

		/// <summary>Initializes an empty instance of the ItemList class.</summary>
		public ItemList()
		{
		}

		/// <summary>Initializes an instance of the ItemList class adding the items matching the supplied filter.</summary>
		/// <param name="items">The gross enumeration of items to initialize with.</param>
		/// <param name="filters">The filters that should be applied to the gross collection.</param>
		public ItemList(IEnumerable<T> items, params ItemFilter[] filters)
		{
			AddRange(items, filters);
		}

		/// <summary>Initializes an instance of the ItemList class adding the items matching the supplied filter.</summary>
		/// <param name="items">The full enumeration of items to initialize with.</param>
		/// <param name="filters">The filters that should be applied to the full collection.</param>
		public ItemList(IEnumerable items, params ItemFilter[] filters)
		{
			AddRange(items, filters);
		}

		/// <summary>Initializes an instance of the ItemList class adding the items matching the supplied filter.</summary>
		/// <param name="items">The full enumeration of items to initialize with.</param>
		/// <param name="filters">The filters that should be applied to the full collection.</param>
		public ItemList(IEnumerable<T> items, IEnumerable<ItemFilter> filters)
		{
			AddRange(items, filters);
		}

		#endregion

		#region Methods

		public void AddRange(IEnumerable<T> items, params ItemFilter[] filters)
		{
			foreach (T item in items)
				if (AllMatch(item, filters))
					Add(item);
		}

		public void AddRange(IEnumerable items, params ItemFilter[] filters)
		{
			foreach (ContentItem item in items)
				if (AllMatch(item, filters))
					Add((T) item);
		}

		public void AddRange(IEnumerable<T> items, IEnumerable<ItemFilter> filters)
		{
			foreach (T item in items)
				if (AllMatch(item, filters))
					Add(item);
		}

		private bool AllMatch(ContentItem item, IEnumerable<ItemFilter> filters)
		{
			foreach (ItemFilter filter in filters)
				if (!filter.Match(item))
					return false;
			return true;
		}

		public void Sort(string sortExpression)
		{
			Sort(new ItemComparer<T>(sortExpression));
		}

        public bool ContainsAny(IEnumerable<T> items)
        {
            foreach (T item in items)
                if (Contains(item))
                    return true;
            return false;
        }
		#endregion

		#region IHierarchicalEnumerable Members

		public IHierarchyData GetHierarchyData(object enumeratedItem)
		{
			return new ItemHierarchyData((ContentItem) enumeratedItem);
		}

		#endregion

		#region Nested type: ItemHierarchyData

		private class ItemHierarchyData : IHierarchyData
		{
			private ContentItem item;

			public ItemHierarchyData(ContentItem item)
			{
				this.item = item;
			}

			#region IHierarchyData Members

			IHierarchicalEnumerable IHierarchyData.GetChildren()
			{
				return item.GetChildren();
			}

			IHierarchyData IHierarchyData.GetParent()
			{
				return (item.Parent != null)
				       	? new ItemHierarchyData(item.Parent)
				       	: null;
			}

			bool IHierarchyData.HasChildren
			{
				get { return item.GetChildren().Count > 0; }
			}

			object IHierarchyData.Item
			{
				get { return item; }
			}

			string IHierarchyData.Path
			{
				get { return item.Url; }
			}

			string IHierarchyData.Type
			{
				get { return item.GetType().Name; }
			}

			#endregion
		}

		#endregion
	}
}