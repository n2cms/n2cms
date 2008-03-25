using System;
using System.Collections.Generic;
using System.Text;
using N2.Collections;

namespace N2.Templates
{
	public sealed class Find : N2.Persistence.GenericFind<Items.AbstractRootPage,Items.AbstractStartPage>
	{
		/// <summary>
		/// Gets the item at the specified level.
		/// </summary>
		/// <param name="level">Level = 1 equals start page, level = 2 a child of the start page, and so on.</param>
		/// <returns>An ancestor at the specified level.</returns>
		public static ContentItem FindAncestorAtLevel(int level)
		{
			ItemList items = new ItemList(Parents);
			if (items.Count >= level)
				return items[items.Count - level];
			else if (items.Count == level - 1)
				return CurrentPage;
			return null;
		}
	}
}
