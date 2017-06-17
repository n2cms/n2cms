using System;
using System.Collections.Generic;
using N2.Collections;
using N2.Persistence;
using N2.Templates.Mvc.Models.Pages;

namespace N2.Templates.Mvc
{
	public sealed class Find : GenericFind<ContentItem, StartPage>
	{
		public static StartPage ClosestStartPage
		{
			get
			{
				foreach (var item in EnumerateParents(CurrentPage, null, true))
				{
					var sp = item as StartPage;
					if (sp != null)
						return sp;
				}
				return StartPage;
			}
		}


		public static LanguageRoot ClosestLanguageRoot
		{
			get
			{
				foreach (var item in EnumerateParents(CurrentPage, null, true))
				{
					var sp = item as LanguageRoot;
					if (sp != null)
						return sp;
				}
				return StartPage;
			}
		}

		/// <summary>
		///     Gets the item at the specified level.
		/// </summary>
		/// <param name="level">Level = 1 equals start page, level = 2 a child of the start page, and so on.</param>
		/// <returns>An ancestor at the specified level.</returns>
		public static ContentItem AncestorAtLevel(int level)
		{
			return AncestorAtLevel(level, Parents, CurrentPage);
		}

		/// <summary>
		///     Gets the item at the specified level.
		/// </summary>
		/// <param name="level">Level = 1 equals start page, level = 2 a child of the start page, and so on.</param>
		/// <returns>An ancestor at the specified level.</returns>
		public static ContentItem AncestorAtLevel(int level, IEnumerable<ContentItem> parents, ContentItem currentPage)
		{
			var items = new ItemList(parents);
			if (items.Count >= level)
				return items[items.Count - level];
			if (items.Count == level - 1)
				return currentPage;
			return null;
		}
	}
}