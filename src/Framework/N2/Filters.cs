using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Collections;

namespace N2
{
	/// <summary>
	/// Provides access to common filters.
	/// </summary>
	public static class Filters
	{
		/// <summary>Filters by access</summary>
		/// <returns>A filter.</returns>
		public static ItemFilter Access()
		{
			return new AccessFilter();
		}

		/// <summary>Filters by all the provided filters.</summary>
		/// <param name="filters">The filters to aggregate.</param>
		/// <returns>A filter.</returns>
		public static ItemFilter Composition(params ItemFilter[] filters)
		{
			return new CompositeFilter(filters);
		}

		/// <summary>Filters by counting items..</summary>
		/// <param name="skip">Number of items to skip.</param>
		/// <param name="take">Number of items to take.</param>
		/// <returns>A filter.</returns>
		public static ItemFilter Count(int skip, int take)
		{
			return new CountFilter(skip, take);
		}

		/// <summary>Filters by the passed delegate.</summary>
		/// <param name="isMatch">A function that returns true if the item can be filtered.</param>
		/// <returns>A filter.</returns>
		public static ItemFilter Custom(Func<ContentItem, bool> isMatch)
		{
			return new DelegateFilter(isMatch);
		}

		/// <summary>Filters away duplicates.</summary>
		/// <returns>A filter.</returns>
		public static ItemFilter Duplicates()
		{
			return new DuplicateFilter();
		}

		/// <summary>Filters by items that can be shown in a navigation. This is a composition of page, access, visibility and published filter.</summary>
		/// <returns>A filter.</returns>
		public static ItemFilter Navigation()
		{
			return new NavigationFilter();
		}

		/// <summary>Doesn't filter.</summary>
		/// <returns>A filter.</returns>
		public static ItemFilter Nothing()
		{
			return new NullFilter();
		}

		/// <summary>Filters by pages.</summary>
		/// <returns>A filter.</returns>
		public static ItemFilter Pages()
		{
			return new PageFilter();
		}

		/// <summary>Filters items below an ancestor.</summary>
		/// <param name="ancestor">The ancestor of the items to pass.</param>
		/// <returns>A filter.</returns>
		public static ItemFilter Ancestor(ContentItem ancestor)
		{
			return new ParentFilter(ancestor);
		}

		/// <summary>Filters items below an ancestor or the ancestor itself.</summary>
		/// <param name="ancestor">The ancestor of the items to pass.</param>
		/// <returns>A filter.</returns>
		public static ItemFilter AncestorOrSelf(ContentItem ancestor)
		{
			return Custom(i => i == ancestor || i.AncestralTrail.StartsWith(Utility.GetTrail(ancestor)));
		}

		/// <summary>Filters by items that are published and not expired.</summary>
		/// <returns>A filter.</returns>
		public static ItemFilter Published()
		{
			return new PublishedFilter();
		}

		/// <summary>Filters by types of page.</summary>
		/// <param name="types">The types of pages to allow.</param>
		/// <returns>A filter.</returns>
		public static ItemFilter Type(params Type[] types)
		{
			return new TypeFilter(types);
		}

		/// <summary>Filters by type of page.</summary>
		/// <typeparam name="T">The type of page to allow.</typeparam>
		/// <returns>A filter.</returns>
		public static ItemFilter Type<T>()
		{
			return new TypeFilter(typeof(T));
		}

		/// <summary>Filters by items that are visible.</summary>
		/// <returns>A filter.</returns>
		public static ItemFilter Visible()
		{
			return new VisibleFilter();
		}

	}
}
