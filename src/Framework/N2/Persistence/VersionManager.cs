using System;
using System.Collections.Generic;
using N2.Collections;
using N2.Configuration;
using N2.Definitions;
using N2.Edit.Workflow;
using N2.Engine;
using N2.Persistence.Finder;

namespace N2.Persistence
{
	/// <summary>
	/// Handles saving and restoring versions of items.
	/// </summary>
	[Service(typeof(IVersionManager))]
	public class VersionManager : IVersionManager
	{
        readonly IRepository<ContentItem> itemRepository;
		readonly IItemFinder finder;
        readonly StateChanger stateChanger;
		int maximumVersionsPerItem = 100;

		public VersionManager(IRepository<ContentItem> itemRepository, IItemFinder finder, StateChanger stateChanger, EditSection config)
		{
			this.itemRepository = itemRepository;
			this.finder = finder;
            this.stateChanger = stateChanger;
			maximumVersionsPerItem = config.Versions.MaximumPerItem;
		}

		#region Versioning Methods
		/// <summary>Creates a version of the item. This must be called before the item item is modified to save a version before modifications.</summary>
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
                if(item.State == ContentState.Published)
                    stateChanger.ChangeTo(oldVersion, ContentState.Unpublished);
                else
                    stateChanger.ChangeTo(oldVersion, ContentState.Draft);
				oldVersion.Expires = Utility.CurrentTime().AddSeconds(-1);
				oldVersion.Updated = Utility.CurrentTime().AddSeconds(-1);
				oldVersion.Parent = null;
				oldVersion.VersionOf = item;
				if (item.Parent != null)
					oldVersion["ParentID"] = item.Parent.ID;
                itemRepository.SaveOrUpdate(oldVersion);
				itemRepository.Flush();

				if (ItemSavedVersion != null)
					ItemSavedVersion.Invoke(this, new ItemEventArgs(oldVersion));

				TrimVersionCountTo(item, maximumVersionsPerItem);

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
            return ReplaceVersion(currentItem, replacementItem, true);
        }

		/// <summary>Update a page version with another, i.e. save a version of the current item and replace it with the replacement item. Returns a version of the previously published item.</summary>
		/// <param name="currentItem">The item that will be stored as a previous version.</param>
		/// <param name="replacementItem">The item that will take the place of the current item using it's ID. Any saved version of this item will not be modified.</param>
        /// <param name="storeCurrentVersion">Create a copy of the currently published version before overwriting it.</param>
        /// <returns>A version of the previously published item or the current item when storeCurrentVersion is false.</returns>
		public virtual ContentItem ReplaceVersion(ContentItem currentItem, ContentItem replacementItem, bool storeCurrentVersion)
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
                    if (storeCurrentVersion)
                    {
                        ContentItem versionOfCurrentItem = SaveVersion(currentItem); //TODO: remove?

                        Replace(currentItem, replacementItem);

						if (replacementItem.State == ContentState.Draft && replacementItem.VersionOf.Value == currentItem)
							// drafts can be removed once they have been published
							itemRepository.Delete(replacementItem);

                        transaction.Commit();
                        return versionOfCurrentItem;
                    }
                    else
                    {
                        Replace(currentItem, replacementItem);

						if (replacementItem.State == ContentState.Draft && replacementItem.VersionOf.Value == currentItem)
							// drafts can be removed once they have been published
							itemRepository.Delete(replacementItem);

                        transaction.Commit();
                        return currentItem;
                    }
				}
			}
			return currentItem;
		}

        private void Replace(ContentItem currentItem, ContentItem replacementItem)
        {
            ClearAllDetails(currentItem);

            ((IUpdatable<ContentItem>)currentItem).UpdateFrom(replacementItem);

            currentItem.Updated = Utility.CurrentTime();
            itemRepository.SaveOrUpdate(currentItem);

            if (ItemReplacedVersion != null)
                ItemReplacedVersion.Invoke(this, new ItemEventArgs(replacementItem));

            itemRepository.Flush();
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
		public virtual IList<ContentItem> GetVersionsOf(ContentItem publishedItem)
		{
			if (publishedItem.ID == 0)
				return new ItemList { publishedItem };

			return GetVersionsQuery(publishedItem)
				.Select();
		}

		/// <summary>Retrieves all versions of an item including the master version.</summary>
		/// <param name="publishedItem">The item whose versions to get.</param>
		/// <param name="count">The number of versions to get.</param>
		/// <returns>A list of versions of the item.</returns>
		public virtual IList<ContentItem> GetVersionsOf(ContentItem publishedItem, int count)
		{
			if (publishedItem.ID == 0)
				return new ItemList { publishedItem };

			return GetVersionsQuery(publishedItem)
				.MaxResults(count)
				.Select();
		}

		private IQueryEnding GetVersionsQuery(ContentItem publishedItem)
		{
			return finder.Where.VersionOf.Eq(publishedItem)
				.Or.ID.Eq(publishedItem.ID)
				.OrderBy.VersionIndex.Desc;
		}

		/// <summary>Removes exessive versions.</summary>
		/// <param name="publishedItem">The item whose versions to trim.</param>
		/// <param name="maximumNumberOfVersions">The maximum number of versions to keep.</param>
		public virtual void TrimVersionCountTo(ContentItem publishedItem, int maximumNumberOfVersions)
		{
			if (maximumNumberOfVersions < 0) throw new ArgumentOutOfRangeException("maximumNumberOfVersions");
			if (maximumNumberOfVersions == 0) return;

			IList<ContentItem> versions = GetVersionsOf(publishedItem);
			versions.Remove(publishedItem);
			int max = maximumNumberOfVersions - 1;

			if (versions.Count <= max) return;

			using (ITransaction transaction = itemRepository.BeginTransaction())
			{
				for (int i = max; i < versions.Count; i++)
				{
					this.itemRepository.Delete(versions[i]);
				}
				itemRepository.Flush();
				transaction.Commit();
			}
		}

		/// <summary>Checks whether an item  may have versions.</summary>
		/// <param name="item">The item to check.</param>
		/// <returns>True if the item is allowed to have versions.</returns>
		public bool IsVersionable(ContentItem item)
		{
			var versionables = (VersionableAttribute[])item.GetContentType().GetCustomAttributes(typeof(VersionableAttribute), true);
			bool isVersionable = versionables.Length == 0 || versionables[0].Versionable == N2.Definitions.AllowVersions.Yes;
			
			return isVersionable;
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
