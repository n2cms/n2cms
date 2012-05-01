using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Definitions;

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

		// state

		public static CollectionState CalculateState(this IEnumerable<ContentItem> children)
		{
			CollectionState state = None;
			foreach (var child in children)
			{
				state |= child.GetCollectionState();

				if (state == All)
					return state;
			}

			if (state == None)
				return CollectionState.IsEmpty;

			return state;
		}

		internal static CollectionState GetCollectionState(this ContentItem child)
		{
			if (child.IsPage)
			{
				if (child.State == ContentState.Published && (child.AlteredPermissions & Security.Permission.Read) == Security.Permission.None)
				{
					if (child.Visible)
						return CollectionState.ContainsVisiblePublicPages;
					else
						return CollectionState.ContainsHiddenPublicPages;
				}
				else
				{
					if (child.Visible)
						return CollectionState.ContainsVisibleSecuredPages;
					else
						return CollectionState.ContainsHiddenSecuredPages;
				}
			}
			else
			{
				if (child.State == ContentState.Published && (child.AlteredPermissions & Security.Permission.Read) == Security.Permission.None)
					return CollectionState.ContainsPublicParts;
				else
					return CollectionState.ContainsSecuredParts;
			}
		}

		internal static CollectionState All = CollectionState.ContainsVisiblePublicPages | CollectionState.ContainsHiddenPublicPages | CollectionState.ContainsVisibleSecuredPages | CollectionState.ContainsHiddenSecuredPages | CollectionState.ContainsPublicParts | CollectionState.ContainsSecuredParts;
		internal static CollectionState None = (CollectionState)0;

		// adding & replacing

		public static ICollection<T> AddOrReplace<T>(this ICollection<T> collection, T item) 
			where T : IUniquelyNamed
		{
			return CollectionExtensions.AddOrReplace(collection, item, false);
		}
		public static ICollection<T> AddOrReplace<T>(this ICollection<T> collection, T item, bool replaceIfComparedBefore) 
			where T : IUniquelyNamed
		{
			var existing = collection.FirstOrDefault(i => i.Name == item.Name);
			if (existing != null)
				collection.Remove(existing);

			collection.Add(item);

			return collection;
		}
	}
}
