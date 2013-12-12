using System.Security.Principal;
using N2.Collections;
using N2.Edit.Trash;
using N2.Persistence.NH;

namespace N2
{
    public static class ContentExtensions
    {       /// <summary>Filters an item by access which returning null if the item is not accessible to the current user.</summary>
        /// <typeparam name="T">Generic argument to allow strongly typed returns.</typeparam>
        /// <param name="item">The item to filter.</param>
        /// <returns>The item if it's accessible to the current user.</returns>
        public static T FilterByAccess<T>(this T item) where T : ContentItem
        {
            return item.FilterWith(new AccessFilter());
        }

        /// <summary>Filters an item by access which returning null if the item is not accessible to the current user.</summary>
        /// <typeparam name="T">Generic argument to allow strongly typed returns.</typeparam>
        /// <param name="item">The item to filter.</param>
        /// <param name="user">The user to filter access for.</param>
        /// <returns>The item if it's accessible to the current user.</returns>
        public static T FilterByAccess<T>(this T item, IPrincipal user) where T : ContentItem
        {
            return item.FilterWith(new AccessFilter(user, AccessFilter.CurrentSecurityManager()));
        }

        /// <summary>Filters an item by it's potential visibility in the menu which returning null if the item is not accessible to the current user.</summary>
        /// <typeparam name="T">Generic argument to allow strongly typed returns.</typeparam>
        /// <param name="item">The item to filter.</param>
        /// <returns>The item if it's accessible to the current user.</returns>
        public static T FilterByNavigation<T>(this T item) where T : ContentItem
        {
            return item.FilterWith(new NavigationFilter());
        }

        private static T FilterWith<T>(this T item, ItemFilter filter) where T : ContentItem
        {
            if (item == null)
                return null;

            if (filter.Match(item))
                return item;

            return null;
        }



        /// <summary>
        /// Checks if the item contained within a Trash Can.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>True if the item is in the trash can.</returns>
        public static bool IsRecycled(this ContentItem item)
        {
            if (item.State == ContentState.Deleted)
                return true;

            foreach (ContentItem ancestor in Find.EnumerateParents(item))
            {
                if (ancestor is ITrashCan)
                    return true;
            }
            return false;
        }
    }
}
