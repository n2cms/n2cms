using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Collections
{
	public static class CollectionExtensions
	{
		public static IEnumerable<HierarchyNode<T>> DescendantsAndSelf<T>(this HierarchyNode<T> parent)
		{
			yield return parent;
			foreach (var descendant in parent.Children.SelectMany(c => c.DescendantsAndSelf()))
			{
				yield return descendant;
			}
		}
	}
}
