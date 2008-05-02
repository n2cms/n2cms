using System;
using System.Collections.Generic;

namespace N2.Persistence
{
	/// <summary>
	/// Facade for finding and querying items.
	/// </summary>
	/// <typeparam name="TRoot">The type of root page in the system.</typeparam>
	/// <typeparam name="TStart">The type of start page in the system.</typeparam>
	public abstract class GenericFind<TRoot,TStart> 
		where TRoot:ContentItem 
		where TStart:ContentItem
	{
		/// <summary>Gets the current start page (this may vary depending on host url).</summary>
		public static TStart StartPage
		{
			get { return (TStart)Context.UrlParser.StartPage; }
		}

		/// <summary>Gets the site's root items.</summary>
		public static TRoot RootItem
		{
			get { return Context.Persister.Get<TRoot>(Context.UrlParser.CurrentSite.RootItemID); }
		}

		/// <summary>Gets the currently displayed page (based on the query string).</summary>
		public static ContentItem CurrentPage
		{
			get { return Context.CurrentPage; }
		}

		/// <summary>Gets an enumeration of pages leading to the current page.</summary>
		public static IEnumerable<ContentItem> Parents
		{
			get
			{
				ContentItem startPage = StartPage;
				ContentItem item = CurrentPage;
				return EnumerateParents(item, startPage);
			}
		}
		
		/// <summary>Enumerates parents of the initial item.</summary>
		/// <param name="initialItem">The page whose parents will be enumerated. The page itself will not appear in the enumeration.</param>
		/// <returns>An enumeration of the parents of the initial page.</returns>
		public static IEnumerable<ContentItem> EnumerateParents(ContentItem initialItem)
		{
			return EnumerateParents(initialItem, null);
		}

		/// <summary>Enumerates parents of the initial item.</summary>
		/// <param name="initialItem">The page whose parents will be enumerated. The page itself will not appear in the enumeration.</param>
		/// <param name="lastAncestor">The last page of the enumeration. The enumeration will contain this page.</param>
		/// <returns>An enumeration of the parents of the initial page. If the last page isn't a parent of the inital page all pages until there are no more parents are returned.</returns>
		public static IEnumerable<ContentItem> EnumerateParents(ContentItem initialItem, ContentItem lastAncestor)
		{
			return EnumerateParents(initialItem, lastAncestor, false);
		}

		/// <summary>Enumerates parents of the initial item.</summary>
		/// <param name="initialItem">The page whose parents will be enumerated. The page itself will appear in the enumeration if includeSelf is applied.</param>
		/// <param name="lastAncestor">The last page of the enumeration. The enumeration will contain this page.</param>
		/// <param name="includeSelf">Include the lanitialItem in the enumeration.</param>
		/// <returns>An enumeration of the parents of the initial page. If the last page isn't a parent of the inital page all pages until there are no more parents are returned.</returns>
		public static IEnumerable<ContentItem> EnumerateParents(ContentItem initialItem, ContentItem lastAncestor, bool includeSelf)
		{
			if (initialItem == null) throw new ArgumentNullException("initialItem");

			ContentItem item;
			if(includeSelf)
				item = initialItem;
			else if (initialItem != lastAncestor)
				item = initialItem.Parent;
			else
				yield break;

			while (item != null)
			{
				yield return item;
				if (item == lastAncestor)
					break;
				item = item.Parent;
			}
		}

		/// <summary>Enumerates the trail of items from the last ancestor to the deepest child.</summary>
		/// <param name="deepestChild">The page whose parents will be enumerated. The page itself will appear in the enumeration if includeSelf is applied.</param>
		/// <param name="lastAncestor">The first page of the enumeration.</param>
		/// <param name="includeSelf">Include the deepest child in the enumeration.</param>
		/// <returns>An enumeration of the from the ancestor uptil the deepest child.</returns>
		public static IEnumerable<ContentItem> EnumerateBetween(ContentItem lastAncestor, ContentItem deepestChild, bool includeDeepestChild)
		{
			IList<ContentItem> items = ListParents(deepestChild, lastAncestor, includeDeepestChild);
			for (int i = items.Count-1; i >= 0; --i)
			{
				yield return items[i];
			}
		}

		/// <summary>Creates a list of the parents of the initial item.</summary>
		/// <param name="initialItem">The page whose parents will be enumerated. The page itself will not appear in the enumeration.</param>
		/// <param name="lastAncestor">The last page of the enumeration. The enumeration will contain this page.</param>
		/// <returns>A list of the parents of the initial page. If the last page isn't a parent of the inital page all pages until there are no more parents are returned.</returns>
		public static IList<ContentItem> ListParents(ContentItem initialItem, ContentItem lastAncestor, bool includeInitialItem)
		{
			return new List<ContentItem>(EnumerateParents(initialItem, lastAncestor, includeInitialItem));
		}

		/// <summary>Creates a list of the parents of the initial item.</summary>
		/// <param name="initialItem">The page whose parents will be enumerated. The page itself will not appear in the enumeration.</param>
		/// <param name="lastAncestor">The last page of the enumeration. The enumeration will contain this page.</param>
		/// <returns>A list of the parents of the initial page. If the last page isn't a parent of the inital page all pages until there are no more parents are returned.</returns>
		public static IList<ContentItem> ListParents(ContentItem initialItem, ContentItem lastAncestor)
		{
			return new List<ContentItem>(EnumerateParents(initialItem, lastAncestor));
		}

		/// <summary>Creates a list of the parents of the initial item.</summary>
		/// <param name="initialItem">The page whose parents will be enumerated. The page itself will not appear in the enumeration.</param>
		/// <returns>A list of the parents of the initial page.</returns>
		public static IList<ContentItem> ListParents(ContentItem initialItem)
		{
			return new List<ContentItem>(EnumerateParents(initialItem));
		}

		/// <summary>Enumerates items in a tree hierarchy.</summary>
		/// <param name="rootItem">The parent item whose child items to enumerate. The item itself is not returned.</param>
		/// <returns>An enumeration of all children of an item.</returns>
		public static IEnumerable<ContentItem> EnumerateTree(ContentItem rootItem)
		{
			yield return rootItem;
			foreach (ContentItem item in EnumerateChildren(rootItem))
			{
				yield return item;
			}
		}

		/// <summary>Enumerates child items and their children, and so on.</summary>
		/// <param name="item">The parent item whose child items to enumerate. The item itself is not returned.</param>
		/// <returns>An enumeration of all children of an item.</returns>
		public static IEnumerable<ContentItem> EnumerateChildren(ContentItem item)
		{
			foreach (ContentItem child in item.Children)
			{
				yield return child;
				foreach (ContentItem childItem in EnumerateChildren(child))
					yield return childItem;
			}
		}

		/// <summary>Filters items not of the given type and return an enumeration.</summary>
		/// <typeparam name="T">The typeof item to return.</typeparam>
		/// <param name="items">The input enumeration to filter.</param>
		/// <returns>An enumeration items of the supplied type.</returns>
		public static IEnumerable<T> OfType<T>(IEnumerable<ContentItem> items)
			where T: class
		{
			foreach (ContentItem item in items)
			{
				if (item is T)
					yield return item as T;
			}
		}

		/// <summary>Search for items based on properties and details.</summary>
		public static Finder.IItemFinder Items
		{
			get { return Context.Current.Resolve<Finder.IItemFinder>(); }
		}
		
		/// <summary>Determines wether an item is below a certain ancestral item.</summary>
		/// <param name="item">The item to check for beeing a child or descendant.</param>
		/// <param name="ancestor">The item to check for beeing parent or ancestor.</param>
		/// <returns>True if the item is descendant the ancestor.</returns>
		public static bool IsDescendant(ContentItem item, ContentItem ancestor)
		{
			if (item == null) throw new ArgumentNullException("item");
			if (ancestor == null) throw new ArgumentNullException("ancestor");

			return In(ancestor, EnumerateParents(item));
		}

		/// <summary>Gets an item at a certain level from the last ancestor.</summary>
		/// <param name="initialItem">The item at the greatest depth.</param>
		/// <param name="lastAncestor">The root node for this operation.</param>
		/// <param name="depth">The depth counting from the last ancestor to retrieve an item from.</param>
		/// <returns>The item at the specified depth, or null if this is outside bounds.</returns>
		public static ContentItem AtLevel(ContentItem initialItem, ContentItem lastAncestor, int depth)
		{
			IList<ContentItem> path = ListParents(initialItem, lastAncestor, true);
			int index = path.Count - depth;
			if (index >= 0 && index < path.Count)
				return path[index];
			return null;
		}

		/// <summary>Determines wether an item is below a certain ancestral item or is the ancestral item.</summary>
		/// <param name="item">The item to check for beeing a child or descendant.</param>
		/// <param name="ancestor">The item to check for beeing parent or ancestor.</param>
		/// <returns>True if the item is descendant the ancestor.</returns>
		public static bool IsDescendantOrSelf(ContentItem item, ContentItem ancestor)
		{
			if (item == null) throw new ArgumentNullException("item");
			if (ancestor == null) throw new ArgumentNullException("ancestor");

			return item == ancestor || In(ancestor, EnumerateParents(item));
		}

		/// <summary>Determines wether an item is in a enumeration of items.</summary>
		/// <param name="wantedItem">The item to look for.</param>
		/// <param name="linedUpItems">The items to look among.</param>
		/// <returns>True if the item is in the enumeration of items.</returns>
		public static bool In(ContentItem wantedItem, IEnumerable<ContentItem> linedUpItems)
		{
			if (wantedItem == null) throw new ArgumentNullException("wantedItem");
			if (linedUpItems == null) throw new ArgumentNullException("linedUpItems");

			foreach (ContentItem enumeratedItem in linedUpItems)
			{
				if (enumeratedItem == wantedItem)
					return true;
			}
			return false;
		}
	}
}
