using System;

namespace N2.Persistence
{
    /// <summary>
    /// Brokers items loaded from the database to whoever might want to know 
    /// they were loaded. This is somewhat different from items which are 
    /// caught through the <see cref="IPersister"/> load event since this will
    /// also be invoked when new items are created and when items are lazily
    /// loaded e.g. through the children collection.
    /// </summary>
    public interface IItemNotifier
    {
        /// <summary>Notify subscribers that an item was loaded or created.</summary>
        /// <param name="newlyCreatedItem">The item that was loaded or created.</param>
        /// <returns>True if the item was modified.</returns>
        bool NotifiyCreated(ContentItem newlyCreatedItem);

        /// <summary>Notify subscribers that an item is to be saved.</summary>
        /// <param name="itemToBeSaved">The item that is to be saved.</param>
        /// <returns>True if the item was modified.</returns>
        bool NotifySaving(ContentItem itemToBeSaved);

        /// <summary>Notify subscribers that an item is to be deleted.</summary>
        /// <param name="itemToBeDeleted">The item that is to be deleted.</param>
        void NotifyDeleting(ContentItem itemToBeDeleted);

        /// <summary>Is triggered when an item was created or loaded from the database.</summary>
        event EventHandler<NotifiableItemEventArgs> ItemCreated;

        /// <summary>Is triggered when an item is to be saved the database.</summary>
        event EventHandler<NotifiableItemEventArgs> ItemSaving;

        /// <summary>Is triggered when an item is to be deleted from the database.</summary>
        event EventHandler<ItemEventArgs> ItemDeleting;
    }
}
