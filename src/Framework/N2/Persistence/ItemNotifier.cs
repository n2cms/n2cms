using System;
using N2.Engine;

namespace N2.Persistence
{
    /// <summary>
    /// Brokers items loaded from the database to whoever might want to know 
    /// they were loaded. This is somewhat different from items which are 
    /// caught through the <see cref="IPersister"/> load event since this will
    /// also be invoked when new items are created and when items are lazily
    /// loaded e.g. through the children collection.
    /// </summary>
    [Service(typeof(IItemNotifier))]
    public class ItemNotifier : IItemNotifier
    {
        #region IItemNotifier Members

        /// <summary>Notify subscribers that an item was loaded or created.</summary>
        /// <param name="newlyCreatedItem">The item that was loaded or created.</param>
        /// <returns>True if the item was modified.</returns>
        public bool NotifiyCreated(ContentItem newlyCreatedItem)
        {
            if (ItemCreated == null)
                return false;

            NotifiableItemEventArgs args = new NotifiableItemEventArgs(newlyCreatedItem);
            ItemCreated(this, args);
            return args.WasModified;
        }

        /// <summary>Notify subscribers that an item is to be saved.</summary>
        /// <param name="itemToBeSaved">The item that is to be saved.</param>
        /// <returns>True if the item was modified.</returns>
        public bool NotifySaving(ContentItem itemToBeSaved)
        {
            if (ItemSaving == null)
                return false;

            var args = new NotifiableItemEventArgs(itemToBeSaved);
            ItemSaving(this, args);
            return args.WasModified;
        }
        
        /// <summary>Notify subscribers that an item is to be deleted.</summary>
        /// <param name="itemToBeDeleted">The item that is to be deleted.</param>
        /// <returns>True if the item was modified.</returns>
        public void NotifyDeleting(ContentItem itemToBeDeleted)
        {
            if (ItemDeleting == null)
                return;

            var args = new ItemEventArgs(itemToBeDeleted);
            ItemDeleting(this, args);
        }

        /// <summary>Is triggered when an item was created or loaded from the database.</summary>
        public event EventHandler<NotifiableItemEventArgs> ItemCreated;

        /// <summary>Is triggered when an item is to be saved the database.</summary>
        public event EventHandler<NotifiableItemEventArgs> ItemSaving;
    
        /// <summary>Is triggered when an item is to be deleted from the database.</summary>
        public event EventHandler<ItemEventArgs> ItemDeleting;
        #endregion
    }
}
