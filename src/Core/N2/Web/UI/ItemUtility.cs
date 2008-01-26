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
using System.Web;
using System.Web.UI;
using System.Collections;
using System.Collections.Generic;
using N2.Definitions;

namespace N2.Web.UI
{
    public class ItemUtility
    {
		public static T FindInParents<T>(Control parentControl) where T:class
		{
			if (parentControl == null || parentControl is T)
				return parentControl as T;
			else
				return FindInParents<T>(parentControl.Parent);
		}

        public static ContentItem FindCurrentItem(Control startControl)
        {
            IItemContainer itemContainer = FindInParents<IItemContainer>(startControl);
            if (itemContainer != null)
                return itemContainer.CurrentItem;
            return null;
        }

		public static ContentItem WalkPath(ContentItem startItem, string path)
		{
			// find starting point
			if (path.StartsWith("/"))
				startItem = Context.UrlParser.StartPage;

			// walk path
			ContentItem item = startItem;
			if (path.StartsWith(Utility.ToAbsolute("~/")))
				path = path.Substring(Utility.ToAbsolute("~/").Length);
			string[] names = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string name in names)
			{
				if (item == null)
					break;
				else if (name == "..")
					item = item.Parent;
				else if (name != "." && !string.IsNullOrEmpty(name))
					item = item.GetChild(name);
			}
			return item;
		}

		public static Control AddUserControl(Control container, ContentItem item, string templateUrl)
		{
			using (new ItemStacker(item))
			{
				return ((IContainable)item).AddTo(container);
			}
		}

		private class ItemStacker : IDisposable
		{
			public ItemStacker(ContentItem currentItem)
			{
				ItemStack.Push(currentItem);
			}
			public void Dispose()
			{
				ItemStack.Pop();
			}
		}

		internal static ContentItem CurrentContentItem
		{
			get { return ItemStack.Peek(); }
		}

		internal static Stack<ContentItem> ItemStack
		{
			get
			{
				Stack<ContentItem> stack = HttpContextItems["ItemStack"] as Stack<ContentItem>;
				if (stack == null)
				{
					HttpContextItems["ItemStack"] = stack = new Stack<ContentItem>();
					stack.Push(Find.CurrentPage);
				}
				return stack;
			}
		}

		internal static IDictionary HttpContextItems
		{
			get
			{
				return HttpContext.Current.Items;
			}
		}

		public static IEnumerable<T> FindInChildren<T>(Control container)
			where T:class
		{
			foreach (Control child in container.Controls)
			{
				if (child is T)
					yield return child as T;
				foreach(T match in FindInChildren<T>(child))
					yield return match as T;
			}
		}
	}
}
