using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using N2.Collections;
using N2.Configuration;
using N2.Definitions;
using N2.Edit.Workflow;
using N2.Engine;
using N2.Persistence.Finder;
using N2.Edit.Versioning;
using N2.Web;
using N2.Definitions.Static;
using N2.Persistence;

namespace N2.Edit.Versioning
{
    /// <summary>
    /// Handles saving and restoring versions of items.
    /// </summary>
    [Service(typeof(IVersionManager))]
    public class VersionManager : IVersionManager
    {
        public ContentVersionRepository Repository { get; private set; }
        readonly IContentItemRepository itemRepository;
        readonly StateChanger stateChanger;
        int maximumVersionsPerItem = 100;

        public VersionManager(ContentVersionRepository versionRepository, IContentItemRepository itemRepository, StateChanger stateChanger, EditSection config)
        {
            this.Repository = versionRepository;
            this.itemRepository = itemRepository;
            this.stateChanger = stateChanger;
            maximumVersionsPerItem = config.Versions.MaximumPerItem;
        }

        #region Versioning Methods
        /// <summary>Creates a version of the item. This must be called before the item item is modified to save a version before modifications.</summary>
        /// <param name="item">The item to create a old version of.</param>
        /// <returns>The old version.</returns>
        public virtual ContentItem AddVersion(ContentItem item, bool asPreviousVersion = true)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            CancellableItemEventArgs args = new CancellableItemEventArgs(item);
            if (ItemSavingVersion != null)
                ItemSavingVersion.Invoke(this, args);

            if (args.Cancel)
                return null;

            item = args.AffectedItem;

            if (!item.IsPage)
            {
                var page = Find.ClosestPage(item);
                if (page == null)
                    throw new InvalidOperationException("Cannot create version of part which isn't on a page: " + item);

                var pageVersion = AddVersion(page, asPreviousVersion: asPreviousVersion);
                var partVersion = pageVersion.FindPartVersion(item);
                return partVersion;
            }

            ContentItem version = item.CloneForVersioningRecursive(stateChanger, asPreviousVersion);

            if (item.Parent != null)
                version["ParentID"] = item.Parent.ID;

			//ContentVersion savedVersion;
            if (asPreviousVersion)
            {
				/*savedVersion = */Repository.Save(version, asPreviousVersion);
                item.VersionIndex = Repository.GetGreatestVersionIndex(item) + 1;
                itemRepository.SaveOrUpdate(item);
            }
            else
            {
                version.VersionIndex = Repository.GetGreatestVersionIndex(item) + 1;
				/*savedVersion = */Repository.Save(version, asPreviousVersion);
            }

            if (ItemSavedVersion != null)
                ItemSavedVersion.Invoke(this, new ItemEventArgs(version));

            TrimVersionCountTo(item, maximumVersionsPerItem);

			return version;// Repository.DeserializeVersion(savedVersion);
        }

        /// <summary>Updates a version.</summary>
        /// <param name="item">The item to update.</param>
        public void UpdateVersion(ContentItem item)
        {
			if (!item.IsPage)
				item = Find.ClosestPage(item) ?? item;

            if (item.VersionOf.HasValue)
                Repository.Save(item, asPreviousVersion: item.State != ContentState.Draft);
            else
                itemRepository.SaveOrUpdate(item);
        }

        /// <summary>Update a page version with another, i.e. save a version of the current item and replace it with the replacement item. Returns a version of the previously published item.</summary>
        /// <param name="currentItem">The item that will be stored as a previous version.</param>
        /// <param name="replacementItem">The item that will take the place of the current item using it's ID. Any saved version of this item will not be modified.</param>
        /// <param name="storeCurrentVersion">Create a copy of the currently published version before overwriting it.</param>
        /// <returns>A version of the previously published item or the current item when storeCurrentVersion is false.</returns>
        public virtual ContentItem ReplaceVersion(ContentItem currentItem, ContentItem replacementItem, bool storeCurrentVersion = true)
        {
            if (currentItem == null)
                throw new ArgumentNullException("currentItem");
            if (replacementItem == null)
                throw new ArgumentNullException("replacementItem");

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
                        ContentItem versionOfCurrentItem = AddVersion(currentItem);

                        Replace(currentItem, replacementItem);

                        if ((replacementItem.State == ContentState.Draft || replacementItem.State == ContentState.Waiting) && replacementItem.VersionOf.Value == currentItem)
                        {
                            // drafts can be removed once they have been published
                            currentItem.VersionIndex = replacementItem.VersionIndex;
                            itemRepository.SaveOrUpdate(currentItem);

                            Repository.Delete(replacementItem);
                        }

                        transaction.Commit();
                        return versionOfCurrentItem;
                    }
                    else
                    {
                        Replace(currentItem, replacementItem);

                        if (replacementItem.State == ContentState.Draft && replacementItem.VersionOf.Value == currentItem)
                            // drafts can be removed once they have been published
                            //itemRepository.Delete(replacementItem);
                            Repository.Delete(replacementItem);

                        transaction.Commit();
                        return currentItem;
                    }
                }
            }
            return currentItem;
        }

		public ContentItem GetOrCreateDraft(ContentItem item)
		{
			var draft = GetVersionsOf(item, stateFilter: ContentState.Draft).Where(vi => vi.VersionIndex > item.VersionIndex).FirstOrDefault();
			if (draft != null)
				return GetVersion(item, draft.VersionIndex);
			return AddVersion(item, asPreviousVersion: false);
		}

        private void Replace(ContentItem currentItem, ContentItem replacementItem)
        {
            UpdateValues(currentItem, replacementItem);
            itemRepository.SaveOrUpdate(currentItem);

            foreach (var removedItem in RemoveRemovedPartsRecursive(currentItem, replacementItem))
                itemRepository.Delete(removedItem);

            foreach (var modifiedItem in UpdateModifiedPartsRecursive(currentItem, replacementItem))
                itemRepository.SaveOrUpdate(modifiedItem);

            foreach (var addedItem in AddAddedPartsRecursive(currentItem, replacementItem))
                itemRepository.SaveOrUpdate(addedItem);

            if (ItemReplacedVersion != null)
                ItemReplacedVersion.Invoke(this, new ItemEventArgs(replacementItem));

            itemRepository.Flush();
        }

        private void UpdateValues(ContentItem currentItem, ContentItem replacementItem)
        {
            ClearAllDetails(currentItem);

            ((IUpdatable<ContentItem>)currentItem).UpdateFrom(replacementItem);

            RelinkMasterVersion(currentItem);

            var latestVersion = Repository.GetLatestVersion(currentItem);
            if (currentItem != latestVersion)
                // do not increment when latest is current
                currentItem.VersionIndex = latestVersion.VersionIndex + 1;
            currentItem.Updated = Utility.CurrentTime();
        }

        private void RelinkMasterVersion(ContentItem currentItem)
        {
            foreach (var detail in currentItem.Details)
            {
                Relink(detail);
            }
            foreach (var dc in currentItem.DetailCollections)
            {
                foreach (var detail in dc.Details)
                {
                    Relink(detail);
                }
            }
        }

        private static void Relink(Details.ContentDetail detail)
        {
            if (detail.LinkedItem.ID.HasValue && detail.LinkedItem.Value != null && detail.LinkedItem.Value.VersionOf.HasValue)
            {
                detail.LinkedItem = detail.LinkedItem.Value.VersionOf;
            }
        }

        private IEnumerable<ContentItem> RemoveRemovedPartsRecursive(ContentItem currentItem, ContentItem replacementItem)
        {
            var versionedChildren = replacementItem != null
                ? replacementItem.Children.Where(c => c.VersionOf.HasValue).ToDictionary(c => c.VersionOf.ID.Value)
                : new Dictionary<int, ContentItem>();
            foreach (var existingChild in currentItem.Children.Where(c => !c.IsPage).ToList())
            {
                if (versionedChildren.ContainsKey(existingChild.ID))
                {
                    foreach (var removedChild in RemoveRemovedPartsRecursive(existingChild, versionedChildren[existingChild.ID]))
                        yield return removedChild;
                }
                else
                {
                    foreach (var removedChild in RemoveRemovedPartsRecursive(existingChild, null))
                        yield return removedChild;

                    existingChild.AddTo(null);
                    RelinkMasterVersion(existingChild); // transient links may cause trouble even for items being deleted
                    yield return existingChild;
                }
            }
        }

        private IEnumerable<ContentItem> AddAddedPartsRecursive(ContentItem currentItem, ContentItem replacementItem)
        {
            foreach (var replacingChild in replacementItem.Children)
            {
                var masterChild = replacingChild.VersionOf.Value;
                if (replacingChild.VersionOf.Value == null)
                {
                    var clone = replacingChild.Clone(false);
                    clone.State = ContentState.Published;
                    if (!clone.Published.HasValue)
                        clone.Published = Utility.CurrentTime();
                    clone.AddTo(currentItem);
					replacingChild.VersionOf = clone;
                    RelinkMasterVersion(clone);
                    yield return clone;
					masterChild = clone;
                }
                foreach (var addedPart in AddAddedPartsRecursive(masterChild, replacingChild))
                    yield return addedPart;
            }
        }

        private IEnumerable<ContentItem> UpdateModifiedPartsRecursive(ContentItem currentItem, ContentItem replacementItem)
        {
            var versionedChildren = replacementItem.Children.Where(c => c.VersionOf.HasValue).ToDictionary(c => c.VersionOf.ID);
            foreach (var existingChild in currentItem.Children.Where(c => !c.IsPage).ToList())
            {
                ContentItem versionedCounterpart;
                if (!versionedChildren.TryGetValue(existingChild.ID, out versionedCounterpart))
                    continue;

                UpdateValues(existingChild, versionedCounterpart);
                yield return existingChild;

                foreach (var modifiedItem in UpdateModifiedPartsRecursive(existingChild, versionedCounterpart))
                    yield return modifiedItem;
            }
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

        /// <summary>Retrieves the version with a given version index.</summary>
        /// <param name="publishedItem">The published item whose version to get.</param>
        /// <param name="versionIndex">The index of the version to get.</param>
        /// <returns>The version with the given index, or null if no item is found.</returns>
        public virtual ContentItem GetVersion(ContentItem publishedItem, int versionIndex)
        {
            if (versionIndex == publishedItem.VersionIndex)
                return publishedItem;
            if (publishedItem.IsPage)
            {
                var version = Repository.GetVersion(publishedItem, versionIndex);
                if (version == null)
                    return null;
                return Repository.DeserializeVersion(version);
            }
            else
            {
                var version = Repository.GetVersion(Find.ClosestPage(publishedItem), versionIndex);
                if (version == null)
                    return null;
                return Repository.DeserializeVersion(version).FindPartVersion(publishedItem);
            }
        }

        /// <summary>Retrieves all versions of an item including the master version.</summary>
        /// <param name="publishedItem">The item whose versions to get.</param>
        /// <param name="count">The number of versions to get.</param>
        /// <returns>A list of versions of the item.</returns>
        public virtual IEnumerable<VersionInfo> GetVersionsOf(ContentItem publishedItem, int skip = 0, int take = 1000, ContentState? stateFilter = null)
        {
            if (publishedItem.ID == 0)
                return new [] { publishedItem.GetVersionInfo() };

	        var versionQuery = Repository.GetVersions(publishedItem).Select(v => v.GetVersionInfo(Repository))
		        .Concat(new[] {publishedItem.GetVersionInfo()})
				.Where(vi => !stateFilter.HasValue || vi.State.IsFlagSet(stateFilter.Value))
		        .OrderByDescending(i => i.VersionIndex)
		        .Skip(skip).Take(take);
			
	        var versionList = new List<VersionInfo>();
	        foreach (var version in versionQuery)
	        {
		        try
		        {
			        versionList.Add(version);
		        }
		        catch (Exception ex)
		        {
			        versionList.Add(new InvalidVersionInfo() {InnerException = ex});
		        }
	        }

	        return versionList;
        }

        /// <summary>Removes exessive versions.</summary>
        /// <param name="publishedItem">The item whose versions to trim.</param>
        /// <param name="maximumNumberOfVersions">The maximum number of versions to keep.</param>
        public virtual void TrimVersionCountTo(ContentItem publishedItem, int maximumNumberOfVersions)
        {
            if (maximumNumberOfVersions < 0) throw new ArgumentOutOfRangeException("maximumNumberOfVersions");
            if (maximumNumberOfVersions == 0) return;


            using (ITransaction transaction = Repository.Repository.BeginTransaction())
            {
                Repository.Repository.Delete(Repository.GetVersions(publishedItem).Skip(maximumNumberOfVersions - 1).ToArray());
                Repository.Repository.Flush();
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

        /// <summary>Deletes a version from the version history.</summary>
        /// <param name="version">The version to delete.</param>
        public virtual void DeleteVersion(ContentItem version)
        {
            Repository.Delete(version);
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
