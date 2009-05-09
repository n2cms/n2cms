using N2.Collections;
using System.Security.Principal;
using N2.Security;

namespace N2.Extensions
{
	public static class FilterExtensions
	{
		/// <summary>Filters an item by access which returning null if the item is not accessible to the current user.</summary>
		/// <typeparam name="T">Generic argument to allow strongly typed returns.</typeparam>
		/// <param name="item">The item to filter.</param>
		/// <returns>The item if it's accessible to the current user.</returns>
		public static T FilterByAccess<T>(this T item) where T: ContentItem
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

		private static T FilterWith<T>(this T item, ItemFilter filter) where T: ContentItem
		{
			if (item == null)
				return null;

			if (filter.Match(item))
				return item;

			return null;
		}
	}
}
