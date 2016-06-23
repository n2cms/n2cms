using System;
using System.Collections.Generic;

namespace N2.Edit.Versioning
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
        /// <param name="storeCurrentVersion">Create a copy of the currently published version before overwriting it.</param>
        /// <returns>The previously published version.</returns>
        ContentItem ReplaceVersion(ContentItem currentItem, ContentItem replacementItem, bool storeCurrentVersion = true);

        /// <summary>Creates a "previous" version of an item. This must be called before the item item is modified.</summary>
        /// <param name="item">The item to create a old version of.</param>
        /// <returns>The old version.</returns>
        ContentItem AddVersion(ContentItem item, bool asPreviousVersion = true);

        /// <summary>Updates a version.</summary>
        /// <param name="item">The item to update.</param>
        void UpdateVersion(ContentItem item);

        /// <summary>Retrieves the version with a given version index.</summary>
        /// <param name="publishedItem">The published item whose version to get.</param>
        /// <param name="versionIndex">The index of the version to get.</param>
        /// <returns>The version with the given index, or null if no item is found.</returns>
        ContentItem GetVersion(ContentItem publishedItem, int versionIndex);

        /// <summary>Retrieves all versions of an item including the master version.</summary>
        /// <param name="publishedItem">The item whose versions to get.</param>
        /// <param name="count">The number of versions to get.</param>
        /// <returns>A list of versions of the item.</returns>
		IEnumerable<VersionInfo> GetVersionsOf(ContentItem publishedItem, int skip = 0, int take = 1000, ContentState? stateFilter = null);

        /// <summary>Removes exessive versions.</summary>
        /// <param name="publishedItem">The item whose versions to trim.</param>
        /// <param name="maximumNumberOfVersions">The maximum number of versions to keep.</param>
        void TrimVersionCountTo(ContentItem publishedItem, int maximumNumberOfVersions);

        /// <summary>Checks whether an item  may have versions.</summary>
        /// <param name="item">The item to check.</param>
        /// <returns>True if the item is allowed to have versions.</returns>
        bool IsVersionable(ContentItem item);

        /// <summary>Deletes a version from the version history.</summary>
        /// <param name="version">The version to delete.</param>
        void DeleteVersion(ContentItem version);

		ContentItem GetOrCreateDraft(ContentItem item);
    }
}
