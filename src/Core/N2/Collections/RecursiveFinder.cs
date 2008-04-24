using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Collections
{
	/// <summary>
	/// Enumerates child items recursively to find items of a certain type, or 
	/// implementing a certain interface.
	/// </summary>
	public class RecursiveFinder
	{
		public IEnumerable<T> Find<T>(ContentItem root, int recursionDepth)
			where T : class
		{
			List<T> items = new List<T>();
			AppendRecursive(root, items, recursionDepth);
			return items;
		}

		private void AppendRecursive<T>(ContentItem item, List<T> items, int depth)
			where T : class
		{
			if (item is T)
				items.Add(item as T);

			if (depth <= 0)
				return;

			foreach (ContentItem child in item.Children)
			{
				AppendRecursive(child, items, depth - 1);
			}
		}
	}
}
