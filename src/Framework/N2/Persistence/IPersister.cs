using N2.Persistence.Sources;
using System;

namespace N2.Persistence
{
    /// <summary>
    /// Represents a class responsible for (database) persistence of content 
    /// items and details therof.
    /// </summary>
    public interface IPersister: IDisposable
    {
        /// <summary>Gets the repository implementation used by this persister. Please note that using the using the repository circumvent events exposed by the persister and is not run in a transaction. Don't expect the behaviour to be exactly the same.</summary>
        ContentSource Sources { get; }

        /// <summary>Gets the repository implementation used by this persister. Please note that using the using the repository circumvent events exposed by the persister and is not run in a transaction. Don't expect the behaviour to be exactly the same.</summary>
        IContentItemRepository Repository { get; }

        /// <summary>Loads the content item with the supplied id or returns null if no items matches that identifier.</summary>
        /// <param name="id">The identifier of the item to retrieve.</param>
        /// <returns>The matching item or null if no item was found.</returns>
        ContentItem Get(int id);

        /// <summary>Loads the content item with the supplied id or returns null if no items matches that identifier.</summary>
        /// <typeparam name="T">The type of item to load.</typeparam>
        /// <param name="id">The identifier of the item to retrieve.</param>
        /// <returns>The matching item or null if no item was found.</returns>
        T Get<T>(int id) where T : ContentItem;
        
        /// <summary>Saves an item to persistence medium.</summary>
        /// <param name="unsavedItem">The item to save.</param>
        /// <remarks>When using <see cref="N2.Persistence.NH.ContentPersister"/> changes on existing items are tracked and automatically persisted.</remarks>
        void Save(ContentItem unsavedItem);

        /// <summary>Deletes an item including child items, any associations towards it (<see cref="N2.Details.LinkDetail"/>) and previous versions.</summary>
        /// <param name="itemNoMore">The item to delete.</param>
        void Delete(ContentItem itemNoMore);

        /// <summary>Copies an item and it's child items to a place below the new destination.</summary>
        /// <param name="source">The item to copy.</param>
        /// <param name="destination">The item below which the copied item will be placed.</param>
        /// <returns>The newly created item.</returns>
        ContentItem Copy(ContentItem source, ContentItem destination);

        /// <summary>Copies an item and it's child items to a place below the new destination.</summary>
        /// <param name="source">The item to copy.</param>
        /// <param name="destination">The item below which the copied item will be placed.</param>
        /// <param name="includeChildren">Whether child items should be copied as well.</param>
        /// <returns>The newly created item.</returns>
        ContentItem Copy(ContentItem source, ContentItem destination, bool includeChildren);

        /// <summary>Moves an item and it's chlid items to a place below the new destination.</summary>
        /// <param name="source">The item to move.</param>
        /// <param name="destination">The below which the moved item will be placed.</param>
        void Move(ContentItem source, ContentItem destination);

        /// <summary>Persist changes.</summary>
        [Obsolete("Use Repository.Flush()")]
        void Flush();

        /// <summary>Occurs before an item is saved</summary>
        event EventHandler<CancellableItemEventArgs> ItemSaving;
        /// <summary>Occurs when an item has been saved</summary>
        event EventHandler<N2.ItemEventArgs> ItemSaved;
        /// <summary>Occurs before an item is deleted</summary>
        event EventHandler<CancellableItemEventArgs> ItemDeleting;
        /// <summary>Occurs when an item has been deleted</summary>
        event EventHandler<N2.ItemEventArgs> ItemDeleted;
        /// <summary>Occurs before an item is moved</summary>
        event EventHandler<CancellableDestinationEventArgs> ItemMoving;
        /// <summary>Occurs when an item has been moved</summary>
        event EventHandler<DestinationEventArgs> ItemMoved;
        /// <summary>Occurs before an item is copied</summary>
        event EventHandler<CancellableDestinationEventArgs> ItemCopying;
        /// <summary>Occurs when an item has been copied</summary>
        event EventHandler<DestinationEventArgs> ItemCopied;
        /// <summary>Occurs when an item is loaded</summary>
        event EventHandler<N2.ItemEventArgs> ItemLoaded;
    }
}
