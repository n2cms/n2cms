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
	public class ItemList<T> : IList<T>, IHierarchicalEnumerable where T : ContentItem
	{
		IList<T> inner;

        #region Constructors

        /// <summary>Initializes an empty instance of the ItemList class.</summary>
        public ItemList()
        {
			inner = new List<T>();
		}

		/// <summary>Initializes an instance of the ItemList class with the supplied items.</summary>
		public ItemList(IList<T> items)
		{
			inner = items ?? new List<T>();
		}

        /// <summary>Initializes an instance of the ItemList class with the supplied items.</summary>
        public ItemList(IEnumerable<T> items)
        {
			inner = new List<T>(items);
        }

		/// <summary>Initializes an instance of the ItemList class adding the items matching the supplied filter.</summary>
		/// <param name="items">The full enumeration of items to initialize with.</param>
		/// <param name="filters">The filters that should be applied to the full collection.</param>
		public ItemList(IEnumerable items, params ItemFilter[] filters)
			: this()
		{
			AddRange(items, filters);
		}

		/// <summary>Initializes an instance of the ItemList class adding the items matching the supplied filter.</summary>
		/// <param name="items">The full enumeration of items to initialize with.</param>
		/// <param name="filters">The filters that should be applied to the full collection.</param>
		public ItemList(ICollection<T> items, ItemFilter filter)
			: this()
		{
			if (items.Count == 0)
				return;
			AddRange(items, filter);
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

		/// <summary>Sorts the elements in the entire list using the default comparer.</summary>
		public void Sort()
		{
			List<T> copy = new List<T>(inner);
			copy.Sort();
			inner = copy;
		}

		/// <summary>Sorts the elements in the entire list using the specified comparer.</summary>
		/// <param name="comparer">The comparer to use.</param>
		public void Sort(IComparer<T> comparer)
		{
			List<T> copy = new List<T>(inner);
			copy.Sort(comparer);
			inner = copy;
		}

		/// <summary>Sorts the elements in the entire list using the specified expression.</summary>
		/// <param name="sortExpression">A sort expression, e.g. Published DESC.</param>
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

		/// <summary>Converts a list to a strongly typed list filtering all items of another type.</summary>
		/// <typeparam name="OtherT">The type the items in resulting list should have.</typeparam>
		/// <returns>A list of items of the correct type.</returns>
		public ItemList<OtherT> Cast<OtherT>() where OtherT : ContentItem
		{
			return new ItemList<OtherT>(this, TypeFilter.Of<OtherT>());
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

		#region IList<T> Members

		public int IndexOf(T item)
		{
			return inner.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			inner.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			inner.RemoveAt(index);
		}

		public T this[int index]
		{
			get { return inner[index]; }
			set { inner[index] = value; }
		}

		#endregion

		#region ICollection<T> Members

		public void Add(T item)
		{
			inner.Add(item);
		}

		public void Clear()
		{
			inner.Clear();
		}

		public bool Contains(T item)
		{
			return inner.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			inner.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return inner.Count; }
		}

		public bool IsReadOnly
		{
			get { return inner.IsReadOnly; }
		}

		public bool Remove(T item)
		{
			return inner.Remove(item);
		}

		#endregion

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			return inner.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return inner.GetEnumerator();
		}

		#endregion
	}
}