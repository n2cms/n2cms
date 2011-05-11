using System;
using System.Collections.Generic;
using System.Diagnostics;
using N2.Definitions;
using N2.Engine;
using N2.Persistence.Finder;

namespace N2.Persistence
{
    /// <summary>
    /// A wrapper for persistence functionality.
    /// </summary>
    public abstract class ContentPersisterBase : IPersister
    {
        protected readonly IRepository<int, ContentItem> itemRepository;
        protected readonly IItemFinder finder;

        /// <summary>Creates a new instance of the PersistenceManager.</summary>
        public ContentPersisterBase(IRepository<int, ContentItem> itemRepository, IItemFinder finder)
        {
            this.itemRepository = itemRepository;
            this.finder = finder;
        }

        #region Load, Save, & Delete Methods

        /// <summary>Gets an item by id</summary>
        /// <param name="id">The id of the item to load</param>
        /// <returns>The item if one with a matching id was found, otherwise null.</returns>
        public virtual ContentItem Get(int id)
        {
            ContentItem item = itemRepository.Get(id);
            if (ItemLoaded != null)
            {
                return Invoke(ItemLoaded, new ItemEventArgs(item)).AffectedItem;
            }
            return item;
        }

        /// <summary>Gets an item by id</summary>
        /// <typeparam name="T">The type of object that is expected</typeparam>
        /// <param name="id">The id of the item to load</param>
        /// <returns>The item if one with a matching id was found, otherwise null.</returns>
        public virtual T Get<T>(int id) where T : ContentItem
        {
            return (T)Get(id);
        }

        /// <summary>Saves or updates an item storing it in database</summary>
        /// <param name="unsavedItem">Item to save</param>
        public void Save(ContentItem item)
        {
            Utility.InvokeEvent(ItemSaving, item, this, SaveAction);

            Invoke(ItemSaved, new ItemEventArgs(item));
        }

        protected abstract void SaveAction(ContentItem item);

        /// <summary>Deletes an item an all sub-items</summary>
        /// <param name="itemNoMore">The item to delete</param>
        public void Delete(ContentItem itemNoMore)
        {
            Utility.InvokeEvent(ItemDeleting, itemNoMore, this, DeleteAction);

            Invoke(ItemDeleted, new ItemEventArgs(itemNoMore));
        }

        protected abstract void DeleteAction(ContentItem itemNoMore);

        #region Delete Helper Methods

        protected void DeleteRecursive(ContentItem topItem, ContentItem itemToDelete)
        {
            DeletePreviousVersions(itemToDelete);

            try
            {
                Trace.Indent();
                List<ContentItem> children = new List<ContentItem>(itemToDelete.Children);
                foreach (ContentItem child in children)
                    DeleteRecursive(topItem, child);
            }
            finally
            {
                Trace.Unindent();
            }

            itemToDelete.AddTo(null);

            TraceInformation("ContentPersister.DeleteRecursive " + itemToDelete);
            itemRepository.Delete(itemToDelete);
        }

        protected void DeletePreviousVersions(ContentItem itemNoMore)
        {
            var previousVersions = finder.Where.VersionOf.Eq(itemNoMore).Select();
            if (previousVersions.Count == 0)
                return;

            TraceInformation("ContentPersister.DeletePreviousVersions " + previousVersions.Count + " of " + itemNoMore);

            foreach (ContentItem previousVersion in previousVersions)
            {
                itemRepository.Delete(previousVersion);
            }
        }

        #endregion

        #endregion

        #region Move & Copy Methods

        /// <summary>Move an item to a destination</summary>
        /// <param name="source">The item to move</param>
        /// <param name="destination">The destination below which to place the item</param>
        public virtual void Move(ContentItem source, ContentItem destination)
        {
            Utility.InvokeEvent(ItemMoving, this, source, destination, MoveAction);

            Invoke(ItemMoved, new DestinationEventArgs(source, destination));
        }

        protected abstract ContentItem MoveAction(ContentItem source, ContentItem destination);

        /// <summary>Copies an item and all sub-items to a destination</summary>
        /// <param name="source">The item to copy</param>
        /// <param name="destination">The destination below which to place the copied item</param>
        /// <returns>The copied item</returns>
        public virtual ContentItem Copy(ContentItem source, ContentItem destination)
        {
            return Copy(source, destination, true);
        }

        /// <summary>Copies an item and all sub-items to a destination</summary>
        /// <param name="source">The item to copy</param>
        /// <param name="destination">The destination below which to place the copied item</param>
        /// <param name="includeChildren">Whether child items should be copied as well.</param>
        /// <returns>The copied item</returns>
        public virtual ContentItem Copy(ContentItem source, ContentItem destination, bool includeChildren)
        {
            return Utility.InvokeEvent(ItemCopying, this, source, destination, delegate(ContentItem copiedItem, ContentItem destinationItem)
            {
                if (copiedItem is IActiveContent)
                {
                    TraceInformation("ContentPersister.Copy " + source + " (is IActiveContent) to " + destination);
                    return (copiedItem as IActiveContent).CopyTo(destinationItem);
                }

                TraceInformation("ContentPersister.Copy " + source + " to " + destination);
                ContentItem cloned = copiedItem.Clone(includeChildren);
                if (cloned.Name == source.ID.ToString())
                    cloned.Name = null;
                cloned.Parent = destinationItem;

                Save(cloned);

                Invoke(ItemCopied, new DestinationEventArgs(cloned, destinationItem));

                return cloned;
            });
        }

        #endregion

        #region IPersistenceEventSource

        /// <summary>Occurs before an item is saved</summary>
        public event EventHandler<CancellableItemEventArgs> ItemSaving;

        /// <summary>Occurs when an item has been saved</summary>
        public event EventHandler<ItemEventArgs> ItemSaved;

        /// <summary>Occurs before an item is deleted</summary>
        public event EventHandler<CancellableItemEventArgs> ItemDeleting;

        /// <summary>Occurs when an item has been deleted</summary>
        public event EventHandler<ItemEventArgs> ItemDeleted;

        /// <summary>Occurs before an item is moved</summary>
        public event EventHandler<CancellableDestinationEventArgs> ItemMoving;

        /// <summary>Occurs when an item has been moved</summary>
        public event EventHandler<DestinationEventArgs> ItemMoved;

        /// <summary>Occurs before an item is copied</summary>
        public event EventHandler<CancellableDestinationEventArgs> ItemCopying;

        /// <summary>Occurs when an item has been copied</summary>
        public event EventHandler<DestinationEventArgs> ItemCopied;

        /// <summary>Occurs when an item is loaded</summary>
        public event EventHandler<ItemEventArgs> ItemLoaded;

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            itemRepository.Dispose();
        }

        #endregion

        /// <summary>Persists changes.</summary>
        public void Flush()
        {
            itemRepository.Flush();
        }

        public IRepository<int, ContentItem> Repository
        {
            get { return this.itemRepository; }
        }

        protected virtual T Invoke<T>(EventHandler<T> handler, T args)
            where T : ItemEventArgs
        {
            if (handler != null && args.AffectedItem.VersionOf == null)
                handler.Invoke(this, args);
            return args;
        }

        protected void TraceInformation(string logMessage)
        {
            Trace.TraceInformation(logMessage);
        }
    }
}