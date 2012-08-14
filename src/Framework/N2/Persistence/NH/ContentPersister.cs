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
using N2.Persistence.Finder;
using NHibernate.Criterion;

namespace N2.Persistence.NH
{
	/// <summary>
	/// A wrapper for NHibernate persistence functionality.
	/// </summary>
	[Service(typeof(IPersister))]
	public class ContentPersister : IPersister
	{
		private readonly IRepository<ContentItem> itemRepository;
		private readonly IRepository<ContentDetail> linkRepository;

		/// <summary>Creates a new instance of the DefaultPersistenceManager.</summary>
		public ContentPersister(IRepository<ContentItem> itemRepository, IRepository<ContentDetail> linkRepository)
		{
			this.itemRepository = itemRepository;
			this.linkRepository = linkRepository;
		}

		#region Load, Save, & Delete Methods

		/// <summary>Gets an item by id</summary>
		/// <param name="id">The id of the item to load</param>
		/// <returns>The item if one with a matching id was found, otherwise null.</returns>
		public virtual ContentItem Get(int id)
		{
			if (id == 0) return null;

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
		public virtual void Save(ContentItem unsavedItem)
		{
			using (ITransaction transaction = itemRepository.BeginTransaction())
			{
				// delay saved event until a previously initiated transcation is completed
				transaction.Committed += (s, a) => Invoke(ItemSaved, new ItemEventArgs(unsavedItem));

				Utility.InvokeEvent(ItemSaving, unsavedItem, this, SaveAction);

				transaction.Commit();
			}
		}

		private void SaveAction(ContentItem item)
		{
			if (item is IActiveContent)
			{
				(item as IActiveContent).Save();
			}
			else
			{
				if (!item.VersionOf.HasValue)
					item.Updated = Utility.CurrentTime();
				if (string.IsNullOrEmpty(item.Name))
					item.Name = null;

				itemRepository.SaveOrUpdate(item);
				EnsureName(item);
			}
		}

		/// <summary>Deletes an item an all sub-items</summary>
		/// <param name="itemNoMore">The item to delete</param>
		public void Delete(ContentItem itemNoMore)
		{
			Utility.InvokeEvent(ItemDeleting, itemNoMore, this, DeleteAction);
		}

		void DeleteAction(ContentItem itemNoMore)
		{
			if (itemNoMore is IActiveContent)
			{
				Engine.Logger.Info("ContentPersister.DeleteAction " + itemNoMore + " is IActiveContent");
				(itemNoMore as IActiveContent).Delete();
			}
			else
			{
				using (ITransaction transaction = itemRepository.BeginTransaction())
				{
					DeleteReferencesRecursive(itemNoMore);

					DeleteRecursive(itemNoMore, itemNoMore);

					transaction.Commit();
				}
			}
			Invoke(ItemDeleted, new ItemEventArgs(itemNoMore));
		}

		private void DeleteReferencesRecursive(ContentItem itemNoMore)
		{
			string itemTrail = Utility.GetTrail(itemNoMore);
			var inboundLinks = Find.EnumerateChildren(itemNoMore, true, false)
				.SelectMany(i => linkRepository.Find(new Parameter("LinkedItem", i), new Parameter("ValueTypeKey", ContentDetail.TypeKeys.LinkType)))
				.Where(l => !Utility.GetTrail(l.EnclosingItem).StartsWith(itemTrail))
				.ToList();

			Engine.Logger.Info("ContentPersister.DeleteReferencesRecursive " + inboundLinks.Count + " of " + itemNoMore);

			foreach (ContentDetail link in inboundLinks)
			{
				linkRepository.Delete(link);
				link.AddTo((DetailCollection)null);
			}
			linkRepository.Flush();
		}

		#region Delete Helper Methods

		private void DeleteRecursive(ContentItem topItem, ContentItem itemToDelete)
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

			Engine.Logger.Info("ContentPersister.DeleteRecursive " + itemToDelete);
			itemRepository.Delete(itemToDelete);
		}

		private void DeletePreviousVersions(ContentItem itemNoMore)
		{
			var previousVersions = itemRepository.Find("VersionOf.ID", itemNoMore.ID);

			int count = 0;
			foreach (ContentItem version in previousVersions)
			{
				itemRepository.Delete(version);
				count++;
			}

			Engine.Logger.Info("ContentPersister.DeletePreviousVersions " + count + " of " + itemNoMore);
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
		}

		private ContentItem MoveAction(ContentItem source, ContentItem destination)
		{
			if (source is IActiveContent)
			{
				Engine.Logger.Info("ContentPersister.MoveAction " + source + " (is IActiveContent) to " + destination);
				(source as IActiveContent).MoveTo(destination);
			}
			else
			{
				using (ITransaction transaction = itemRepository.BeginTransaction())
				{
					Engine.Logger.Info("ContentPersister.MoveAction " + source + " to " + destination);
					source.AddTo(destination);
					Save(source);
					transaction.Commit();
				}
			}
			Invoke(ItemMoved, new DestinationEventArgs(source, destination));
			return null;
		}

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
					Engine.Logger.Info("ContentPersister.Copy " + source + " (is IActiveContent) to " + destination);
					return (copiedItem as IActiveContent).CopyTo(destinationItem);
				}

				Engine.Logger.Info("ContentPersister.Copy " + source + " to " + destination);
				ContentItem cloned = copiedItem.Clone(includeChildren);
				if(cloned.Name == source.ID.ToString())
					cloned.Name = null;
				cloned.AddTo(destination);

				Repository.SaveOrUpdateRecursive(cloned);
				EnsureName(cloned);

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
        public IRepository<ContentItem> Repository
        {
            get { return this.itemRepository; }
        }
        protected virtual T Invoke<T>(EventHandler<T> handler, T args)
            where T : ItemEventArgs
        {
			if (handler != null && !args.AffectedItem.VersionOf.HasValue)
                handler.Invoke(this, args);
            return args;
        }

		private void EnsureName(ContentItem item)
		{
			if (string.IsNullOrEmpty(item.Name))
			{
				item.Name = item.ID.ToString();
				itemRepository.SaveOrUpdate(item);
			}
		}
    }
}
