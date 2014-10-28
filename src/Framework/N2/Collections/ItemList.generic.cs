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

using N2.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web.UI;

namespace N2.Collections
{
    /// <summary>
    /// A generic item list.
    /// </summary>
    /// <typeparam name="T">The type of item to list.</typeparam>
    public class ItemList<T> : ContentList<T>, IContentItemList<T>, IEnumerable<T> where T : ContentItem
    {
        #region Constructors

        /// <summary>Initializes an empty instance of the ItemList class.</summary>
        public ItemList()
        {
        }

        /// <summary>Initializes an instance of the ItemList class with the supplied items.</summary>
        public ItemList(IEnumerable<T> items)
            : base(items)
        {
        }

        /// <summary>Initializes an instance of the ItemList class with the supplied items.</summary>
        public ItemList(Func<IEnumerable<T>> itemsFactory)
            : base (itemsFactory)
        {
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
            List<T> copy = new List<T>(Inner);
            copy.Sort();
            Inner = copy;
        }

        /// <summary>Sorts the elements in the entire list using the specified comparer.</summary>
        /// <param name="comparer">The comparer to use.</param>
        public void Sort(IComparer<T> comparer)
        {
            List<T> copy = new List<T>(Inner);
            copy.Sort(comparer);
            Inner = copy;
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

		public ItemList<T> WhereNavigatable(IPrincipal byUser = null, ISecurityManager security = null)
		{
			return new ItemList<T>(this, new NavigationFilter(byUser, security));
		}

		public ItemList<T> WhereAccessible(IPrincipal byUser = null, ISecurityManager security = null)
		{
			return new ItemList<T>(this, new AccessFilter(byUser, security));
		}

		public ItemList<T> Where(params ItemFilter[] filters)
		{
			return new ItemList<T>(this, filters);
		}

		public ItemList<T> Skip(int skip)
		{
			return new ItemList<T>(this, new CountFilter(skip, int.MaxValue));
		}

		public ItemList<T> Take(int take)
		{
			return new ItemList<T>(this, new CountFilter(0, take));
		}
        #endregion

        #region IZonedList<T> Members

        public IEnumerable<T> FindParts(string zoneName)
        {
            return Inner.Where(i => i.ZoneName == zoneName);
        }

        public IEnumerable<T> FindNavigatablePages()
        {
            return FindPages().Where(p => new VisibleFilter().Match(p) && new PublishedFilter().Match(p));
        }

        public IEnumerable<T> FindPages()
        {
            return this.Where(i => i.ZoneName == null);
        }

        public IEnumerable<T> FindParts()
        {
            return this.Where(i => i.ZoneName != null);
        }

        public IEnumerable<string> FindZoneNames()
        {
            return this.Select(i => i.ZoneName).Distinct().ToList();
        }

        #endregion

        #region IContentItemList<T> Members
        public IEnumerable<T> Find(Persistence.IParameter parameters)
        {
            return this.Where(parameters.IsMatch);
        }
        public int FindCount(Persistence.IParameter parameters)
        {
            return this.Where(parameters.IsMatch).Count();
        }
        public IEnumerable<IDictionary<string, object>> Select(Persistence.IParameter parameters, params string[] properties)
        {
            return Find(parameters).Select(i => properties.ToDictionary(p => p, p => i[p]));
        }
        #endregion

		public static implicit operator ItemList(ItemList<T> items)
		{
			return new ItemList(items);
		}
    }
}
