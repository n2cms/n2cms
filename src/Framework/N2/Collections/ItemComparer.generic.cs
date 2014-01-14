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

using System.Collections.Generic;

namespace N2.Collections
{
    /// <summary>
    /// An item comparer. This class can compare classes given a expression.
    /// </summary>
    /// <typeparam name="T">
    /// The type of items to compare.
    /// </typeparam>
    public class ItemComparer<T> : IComparer<T> where T: ContentItem
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
        {
            string[] pair = sortExpression.Split(' ');
            this.detailToCompare = pair[0];
            if (pair.Length > 1 && string.Compare(pair[1], "DESC", true) == 0)
                inverse = true;
        }

        /// <summary>Creates a new instance of the ItemComparer that sorts using sort property and direction.</summary>
        /// <param name="detailToCompare">The name of the property to sort on.</param>
        /// <param name="inverse">Wether the comparison should be "inverse", i.e. make Z less than A.</param>
        public ItemComparer(string detailToCompare, bool inverse)
        {
            this.detailToCompare = detailToCompare;
            this.inverse = inverse;
        }
        #endregion

        #region Static Methods
        /// <summary>Compares two items with each other.</summary>
        /// <param name="x">The first item.</param>
        /// <param name="y">The second item</param>
        /// <param name="detailToCompare">The detail name to use for comparison.</param>
        /// <param name="inverse">Inverse the comparison.</param>
        /// <returns>The compared difference.</returns>
        public static int Compare(T x, T y, string detailToCompare, bool inverse)
        {
            object ox = x[detailToCompare];
            object oy = y[detailToCompare];
            if (inverse)
                return System.Collections.Comparer.Default.Compare(oy, ox);
            else
                return System.Collections.Comparer.Default.Compare(ox, oy);
        }
        #endregion

        #region IComparer & IComparer<T> Members

        /// <summary>Compares to items.</summary>
        /// <param name="x">The first item.</param>
        /// <param name="y">The second item.</param>
        /// <returns>The comparison result.</returns>
        public int Compare(object x, object y)
        {
            if (x is T && y is T)
                return Compare((T)x, (T)y);
            return 0;
        }

        /// <summary>Compares two items.</summary>
        /// <param name="x">The first item.</param>
        /// <param name="y">The second item.</param>
        /// <returns>The comparison result.</returns>
        public int Compare(T x, T y)
        {
            return Compare(x, y, DetailToCompare, Inverse);
        }

        private string detailToCompare = "SortOrder";
        /// <summary>Gets or sets the detail name to use for comparison.</summary>
        public string DetailToCompare
        {
            get { return detailToCompare; }
            set { detailToCompare = value; }
        }

        bool inverse = false;
        /// <summary>Gets or sets wether the comparison should be inversed.</summary>
        public bool Inverse
        {
            get { return inverse; }
            set { inverse = value; }
        }
        #endregion

    }
}
