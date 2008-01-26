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
	/// Filters based on start index and count.
	/// </summary>
	public class CountFilter : ItemFilter
	{
		#region Constructors
		/// <summary>Creates a new instance of the <see cref="CountFilter"/>.</summary>
		/// <param name="startIndex">The index of the first item to show.</param>
		/// <param name="maxCount">The maximum number of items to leave.</param>
		public CountFilter(int startIndex, int maxCount)
		{
			this.startIndex = startIndex;
			this.maxCount = maxCount > 0 ? maxCount : int.MaxValue;
		} 
		#endregion

		#region Private Members
		private int startIndex;
		private int maxCount;
		private int currentIndex; 
		#endregion

		#region Properties
		public int StartIndex
		{
			get { return startIndex; }
			set { startIndex = value; }
		}

		public int MaxCount
		{
			get { return maxCount; }
			set { maxCount = value; }
		}

		public int CurrentIndex
		{
			get { return currentIndex; }
			set { currentIndex = value; }
		} 
		#endregion

		#region Methods
		public override void Filter(IList<ContentItem> items)
		{
			while (items.Count > maxCount + startIndex)
				items.RemoveAt(items.Count - 1);
			while (items.Count > maxCount)
				items.RemoveAt(0);
		}

		/// <summary>This method doesn't consider the input item at all. Instead it increments and compares against this filter's CurrentIndex property.</summary>
		public override bool Match(ContentItem item)
		{
			return Match(CurrentIndex++);
		}

		public virtual bool Match(int itemIndex)
		{
			return itemIndex >= StartIndex 
				&& (MaxCount == int.MaxValue || itemIndex < (StartIndex + MaxCount));
		}

		public virtual void Reset()
		{
			CurrentIndex = 0;
		} 
		#endregion

		#region Static Methods
		public static void Filter(IList<ContentItem> items, int startIndex, int maxCount)
		{
			ItemFilter.Filter(items, new CountFilter(startIndex, maxCount));
		}
		#endregion
	}
}
