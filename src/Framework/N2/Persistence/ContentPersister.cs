#region License

/* Copyright (C) 2007 Cristian Libardo
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 */

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using N2.Definitions;
using N2.Details;
using N2.Engine;
using N2.Persistence.Sources;

namespace N2.Persistence
{
	/// <summary>
	/// A wrapper for NHibernate persistence functionality.
	/// </summary>
	[Service(typeof(IPersister))]
	public class ContentPersister : IPersister
	{
		private readonly IContentItemRepository repository;
		private readonly ContentSource source;

		/// <summary>Creates a new instance of the DefaultPersistenceManager.</summary>
		public ContentPersister(ContentSource source, IContentItemRepository itemRepository)
		{
			this.source = source;
			this.repository = itemRepository;
		}

		#region Load, Save, & Delete Methods

		/// <summary>Gets an item by id</summary>
		/// <param name="id">The id of the item to load</param>
		/// <returns>The item if one with a matching id was found, otherwise null.</returns>
		public virtual ContentItem Get(int id)
		{
            ContentItem item = source.Get(id);
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
		public virtual void Save(ContentItem unsavedItem)
		{
			Utility.InvokeEvent(ItemSaving, unsavedItem, this, source.Save, ItemSaved);
		}

		/// <summary>Deletes an item an all sub-items</summary>
		/// <param name="itemNoMore">The item to delete</param>
		public void Delete(ContentItem itemNoMore)
		{
			Utility.InvokeEvent(ItemDeleting, itemNoMore, this, source.Delete, ItemDeleted);
		}

		#endregion

		#region Move & Copy Methods

		/// <summary>Move an item to a destination</summary>
		/// <param name="source">The item to move</param>
		/// <param name="destination">The destination below which to place the item</param>
		public virtual void Move(ContentItem source, ContentItem destination)
		{
			Utility.InvokeEvent(ItemMoving, this, source, destination, this.source.Move, ItemMoved);
		}

		/// <summary>Copies an item and all sub-items to a destination</summary>
		/// <param name="source">The item to copy</param>
		/// <param name="destination">The destination below which to place the copied item</param>
		/// <returns>The copied item</returns>
		public virtual ContentItem Copy(ContentItem source, ContentItem destination)
		{
			return Utility.InvokeEvent(ItemCopying, this, source, destination, this.source.Copy, ItemCopied);
		}

		/// <summary>Copies an item and all sub-items to a destination</summary>
		/// <param name="source">The item to copy</param>
		/// <param name="destination">The destination below which to place the copied item</param>
		/// <param name="includeChildren">Whether child items should be copied as well.</param>
		/// <returns>The copied item</returns>
		public virtual ContentItem Copy(ContentItem source, ContentItem destination, bool includeChildren)
		{
			if (includeChildren)
				return Copy(source, destination);
			else
				return Copy(source.Clone(false), destination);
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
			repository.Dispose();
		}

		#endregion

        /// <summary>Persists changes.</summary>
        public void Flush()
        {
            repository.Flush();
        }
		public IContentItemRepository Repository
        {
            get { return this.repository; }
        }
        protected virtual T Invoke<T>(EventHandler<T> handler, T args)
            where T : ItemEventArgs
        {
            if (handler != null && args.AffectedItem.VersionOf == null)
                handler.Invoke(this, args);
            return args;
        }

		private void TraceInformation(string logMessage)
		{
			Trace.TraceInformation(logMessage);
		}
    }
}
