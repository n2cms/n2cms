using System;
using System.Collections.Generic;

namespace N2.Persistence
{
	/// <summary>
	/// Classes implementing this interface can store and restore versions of 
	/// items.
	/// </summary>
	public interface IVersionManager
	{
		/// <summary>Update a page version with another, i.e. save a version of the current item and replace it with the replacement item and returns the previously published item.</summary>
		/// <param name="currentItem">The item that will be stored as a previous version.</param>
		/// <param name="replacementItem">The item that will take the place of the current item using it's ID. Any saved version of this item will not be modified.</param>
		/// <returns>The previously published version.</returns>
        [Obsolete("Use ReplaceVersion(ContentItem, ContentItem, bool)")]
        ContentItem ReplaceVersion(ContentItem currentItem, ContentItem replacementItem);

        /// <summary>Update a page version with another, i.e. save a version of the current item and replace it with the replacement item and returns the previously published item.</summary>
        /// <param name="currentItem">The item that will be stored as a previous version.</param>
        /// <param name="replacementItem">The item that will take the place of the current item using it's ID. Any saved version of this item will not be modified.</param>
        /// <param name="storeCurrentVersion">Create a copy of the currently published version before overwriting it.</param>
        /// <returns>The previously published version.</returns>
        ContentItem ReplaceVersion(ContentItem currentItem, ContentItem replacementItem, bool storeCurrentVersion);

		/// <summary>Creates a "previous" version of an item. This must be called before the item item is modified.</summary>
		/// <param name="item">The item to create a old version of.</param>
		/// <returns>The old version.</returns>
		ContentItem SaveVersion(ContentItem item);

		/// <summary>Retrieves all versions of an item including the master version.</summary>
		/// <param name="publishedItem">The item whose versions to get.</param>
		/// <returns>A list of versions of the item.</returns>
		IList<ContentItem> GetVersionsOf(ContentItem publishedItem);

		/// <summary>Retrieves all versions of an item including the master version.</summary>
		/// <param name="publishedItem">The item whose versions to get.</param>
		/// <param name="count">The number of versions to get.</param>
		/// <returns>A list of versions of the item.</returns>
		IList<ContentItem> GetVersionsOf(ContentItem publishedItem, int count);

		/// <summary>Removes exessive versions.</summary>
		/// <param name="publishedItem">The item whose versions to trim.</param>
		/// <param name="maximumNumberOfVersions">The maximum number of versions to keep.</param>
		void TrimVersionCountTo(ContentItem publishedItem, int maximumNumberOfVersions);

		/// <summary>Checks whether an item  may have versions.</summary>
		/// <param name="item">The item to check.</param>
		/// <returns>True if the item is allowed to have versions.</returns>
		bool IsVersionable(ContentItem item);
	}
}
