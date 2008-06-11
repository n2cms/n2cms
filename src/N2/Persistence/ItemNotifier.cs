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
	/// <summary>Intercepts content items loads and injects dependencies such as definition manager and url rewriter.</summary>
	public class ItemNotifier: IItemNotifier
	{
		/// <summary>Creates a new instance of the NHItemDependencyHandler.</summary>
		public ItemNotifier()
		{
			count++;
		}
		public static int count = 0;

		#region IItemCreatedNotifier Members

		public bool Notifiy(ContentItem newlyCreatedItem)
		{
			if (newlyCreatedItem != null && ItemCreated != null)
			{
				ItemCreated.Invoke(this, new ItemEventArgs(newlyCreatedItem));
				return true;
			}
			return false;
		}

		public event EventHandler<ItemEventArgs> ItemCreated;

		#endregion
	}
}
