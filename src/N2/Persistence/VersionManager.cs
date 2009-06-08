using System;
using System.Collections.Generic;
using System.Reflection;
using N2.Persistence.Finder;

namespace N2.Persistence
{
	/// <summary>
	/// Handles saving and restoring versions of items.
	/// </summary>
	public class VersionManager : IVersionManager
	{
		private IRepository<int, ContentItem> itemRepository;
		readonly IItemFinder finder;

		public VersionManager(IRepository<int, ContentItem> itemRepository, IItemFinder finder)
		{
			this.itemRepository = itemRepository;
			this.finder = finder;
		}

		#region Versioning Methods
		/// <summary>Creates an old version of an item. This must be called before the item item is modified.</summary>
		/// <param name="item">The item to create a old version of.</param>
		/// <returns>The old version.</returns>
		public virtual ContentItem SaveVersion(ContentItem item)
		{
			CancellableItemEventArgs args = new CancellableItemEventArgs(item);
			if (ItemSavingVersion != null)
				ItemSavingVersion.Invoke(this, args);
			if (!args.Cancel)
			{
				item = args.AffectedItem;

				ContentItem oldVersion = item.Clone(false);
				oldVersion.Expires = Utility.CurrentTime().AddSeconds(-1);
				oldVersion.Updated = Utility.CurrentTime().AddSeconds(-1);
				oldVersion.Parent = null;
				oldVersion.VersionOf = item;
				if (item.Parent != null)
					oldVersion["ParentID"] = item.Parent.ID;
				itemRepository.SaveOrUpdate(oldVersion);

				if (ItemSavedVersion != null)
					ItemSavedVersion.Invoke(this, new ItemEventArgs(oldVersion));

				return oldVersion;
			}
			return null;
		}

		/// <summary>Update a page version with another, i.e. save a version of the current item and replace it with the replacement item. Returns a version of the previously published item.</summary>
		/// <param name="currentItem">The item that will be stored as a previous version.</param>
		/// <param name="replacementItem">The item that will take the place of the current item using it's ID. Any saved version of this item will not be modified.</param>
		/// <returns>A version of the previously published item.</returns>
		public virtual ContentItem ReplaceVersion(ContentItem currentItem, ContentItem replacementItem)
		{
			CancellableDestinationEventArgs args = new CancellableDestinationEventArgs(currentItem, replacementItem);
			if (ItemReplacingVersion != null)
				ItemReplacingVersion.Invoke(this, args);
			if (!args.Cancel)
			{
				currentItem = args.AffectedItem;
				replacementItem = args.Destination;

				using (ITransaction transaction = itemRepository.BeginTransaction())
				{
					ContentItem versionOfCurrentItem = SaveVersion(currentItem);
					ClearAllDetails(currentItem);

					((IUpdatable<ContentItem>)currentItem).UpdateFrom(replacementItem);

					currentItem.Updated = Utility.CurrentTime();
					itemRepository.Update(currentItem);

					if (ItemReplacedVersion != null)
						ItemReplacedVersion.Invoke(this, new ItemEventArgs(replacementItem));

					itemRepository.Flush(); 
					transaction.Commit();

					return versionOfCurrentItem;
				}
			}
			return currentItem;
		}

		#region ReplaceVersion Helper Methods

		private void ClearAllDetails(ContentItem item)
		{
			item.Details.Clear();

			foreach (Details.DetailCollection collection in item.DetailCollections.Values)
			{
				collection.Details.Clear();
			}
			item.DetailCollections.Clear();
		}
		#endregion

		/// <summary>Retrieves all versions of an item including the master version.</summary>
		/// <param name="publishedItem">The item whose versions to get.</param>
		/// <returns>A list of versions of the item.</returns>
		public IList<ContentItem> GetVersionsOf(ContentItem publishedItem)
		{
			return GetVersionsQuery(publishedItem)
				.Select();
		}

		/// <summary>Retrieves all versions of an item including the master version.</summary>
		/// <param name="publishedItem">The item whose versions to get.</param>
		/// <param name="count">The number of versions to get.</param>
		/// <returns>A list of versions of the item.</returns>
		public IList<ContentItem> GetVersionsOf(ContentItem publishedItem, int count)
		{
			return GetVersionsQuery(publishedItem)
				.MaxResults(count)
				.Select();
		}

		private IQueryEnding GetVersionsQuery(ContentItem publishedItem)
		{
			return finder.Where.VersionOf.Eq(publishedItem)
				.Or.ID.Eq(publishedItem.ID)
				.OrderBy.Updated.Desc;
		}
		#endregion


		/// <summary>Occurs before an item is saved</summary>
		public event EventHandler<CancellableItemEventArgs> ItemSavingVersion;
		/// <summary>Occurs before an item is saved</summary>
		public event EventHandler<ItemEventArgs> ItemSavedVersion;
		/// <summary>Occurs before an item is saved</summary>
		public event EventHandler<CancellableDestinationEventArgs> ItemReplacingVersion;
		/// <summary>Occurs before an item is saved</summary>
		public event EventHandler<ItemEventArgs> ItemReplacedVersion;
	}
}
