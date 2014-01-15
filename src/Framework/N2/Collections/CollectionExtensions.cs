using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Definitions;
using NHibernate.Mapping;

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
                if (child.IsPublished() && (child.AlteredPermissions & Security.Permission.Read) == Security.Permission.None)
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
                if (child.IsPublished() && (child.AlteredPermissions & Security.Permission.Read) == Security.Permission.None)
                    return CollectionState.ContainsPublicParts;
                else
                    return CollectionState.ContainsSecuredParts;
            }
        }

        public static bool IsAny(this CollectionState state, CollectionState anyOf)
        {
            return (state & anyOf) > 0;
        }

        public static bool IsAll(this CollectionState state, CollectionState allOf)
        {
            return (state & allOf) == allOf;
        }

        internal static CollectionState All = CollectionState.ContainsVisiblePublicPages | CollectionState.ContainsHiddenPublicPages | CollectionState.ContainsVisibleSecuredPages | CollectionState.ContainsHiddenSecuredPages | CollectionState.ContainsPublicParts | CollectionState.ContainsSecuredParts;
        internal static CollectionState None = (CollectionState)0;

        // adding & replacing

        public static ICollection<T> AddOrReplace<T>(this ICollection<T> collection, T item) 
            where T : IUniquelyNamed
        {
            return AddOrReplace(collection, item, false);
        }
        public static ICollection<T> AddOrReplace<T>(this ICollection<T> collection, T item, bool replaceIfComparedBefore) 
            where T : IUniquelyNamed
        {
            if (collection is List<T>)
            {
                var rp = false;
                var c2 = collection as List<T>;
                lock (collection)
                {
                    for (int i = 0; i < c2.Count; ++i)
                        if (c2[i].Name == item.Name)
                        {
                            c2[i] = item;
                            rp = true;
                            break;
                        }
                }
                if (!rp)
                    c2.Add(item);
            }
            else
            {
                foreach (var x in collection.Where(i => i.Name == item.Name).ToList())
                    collection.Remove(x);
                collection.Add(item);
            }
            return collection;
        }

        public static IEnumerable<ContentItem> FindPartsRecursive(this IContentItemList<ContentItem> children)
        {
            foreach (var child in children.FindParts())
            {
                yield return child;
                if (child.ChildState.IsAny(CollectionState.Unknown | CollectionState.ContainsPublicParts | CollectionState.ContainsSecuredParts))
                    foreach (var descendant in child.Children.FindPartsRecursive())
                        yield return descendant;
            }
        }
    }
}
