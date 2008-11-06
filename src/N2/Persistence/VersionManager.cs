using System;
using System.Reflection;

namespace N2.Persistence
{
	/// <summary>
	/// Handles saving and restoring versions of items.
	/// </summary>
	public class VersionManager : IVersionManager
	{
		private IRepository<int, ContentItem> itemRepository;
		private IPersister persister;

		public VersionManager(IPersister persister, IRepository<int, ContentItem> itemRepository)
		{
			this.persister = persister;
			this.itemRepository = itemRepository;
		}

		#region Versioning Methods
		/// <summary>Creates an old version of an item. This must be called before the item item is modified.</summary>
		/// <param name="item">The item to create a old version of.</param>
		/// <returns>The old version.</returns>
		public virtual ContentItem SaveVersion(ContentItem item)
		{
			CancellableItemEventArgs args = new CancellableItemEventArgs(item);
			OnSavingVersion(args);
			if (!args.Cancel)
			{
				ContentItem oldVersion = item.Clone(false);
				oldVersion.Expires = DateTime.Now;
				oldVersion.Updated = DateTime.Now;
				oldVersion.Parent = null;
				oldVersion.VersionOf = item;
				if (item.Parent != null)
					oldVersion["ParentID"] = item.Parent.ID;
				persister.Save(oldVersion);

				OnSavedVersion(new ItemEventArgs(oldVersion));
				return oldVersion;
			}
			return null;
		}

		#region SaveVersion Helper Methods
		/// <summary>Invokes the ItemSavingVersion event.</summary>
		/// <param name="args">Event arguments that are checked for cancellation.</param>
		protected virtual void OnSavingVersion(CancellableItemEventArgs args)
		{
			if (ItemSavingVersion != null)
				ItemSavingVersion.Invoke(this, args);
		}

		/// <summary>Invokes the ItemSavedVersion event.</summary>
		protected virtual void OnSavedVersion(ItemEventArgs args)
		{
			if (ItemSavedVersion != null)
				ItemSavedVersion.Invoke(this, args);
		}
		#endregion

		/// <summary>Update a page version with another, i.e. save a version of the current item and replace it with the replacement item. Returns a version of the previously published item.</summary>
		/// <param name="currentItem">The item that will be stored as a previous version.</param>
		/// <param name="replacementItem">The item that will take the place of the current item using it's ID. Any saved version of this item will not be modified.</param>
		/// <returns>A version of the previously published item.</returns>
		public virtual ContentItem ReplaceVersion(ContentItem currentItem, ContentItem replacementItem)
		{
			CancellableDestinationEventArgs args = new CancellableDestinationEventArgs(currentItem, replacementItem);
			OnReplacingVersion(args);
			if (!args.Cancel)
			{
				ContentItem versionOfCurrentItem = SaveVersion(currentItem);

				using (ITransaction transaction = itemRepository.BeginTransaction())
				{
					ClearAllDetails(currentItem);

					UpdateCurrentItemData(currentItem, replacementItem);

					itemRepository.Update(currentItem);

					transaction.Commit();
				}
				OnReplacedVersion(new ItemEventArgs(replacementItem));
				return versionOfCurrentItem;
			}
			return currentItem;
		}

		#region ReplaceVersion Helper Methods
		/// <summary>Invokes the ItemReplacingVersion event.</summary>
		/// <param name="args">Arguments that are checked for cancellation.</param>
		protected virtual void OnReplacingVersion(CancellableDestinationEventArgs args)
		{
			if (ItemReplacingVersion != null)
				ItemReplacingVersion.Invoke(this, args);
		}

		/// <summary>Invokes the ItemReplacedVersion event.</summary>
		protected virtual void OnReplacedVersion(ItemEventArgs args)
		{
			if (ItemReplacedVersion != null)
				ItemReplacedVersion.Invoke(this, args);
		}

		private static void UpdateCurrentItemData(ContentItem currentItem, ContentItem replacementItem)
		{
			for (Type t = currentItem.GetType(); t.BaseType != null; t = t.BaseType)
			{
				foreach (FieldInfo fi in t.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
				{
					if (fi.GetCustomAttributes(typeof(DoNotCopyAttribute), true).Length == 0)
						if (fi.Name != "id" && fi.Name != "expires" && fi.Name != "published")
							fi.SetValue(currentItem, fi.GetValue(replacementItem));
					if(fi.Name == "url")
						fi.SetValue(currentItem, null);
				}
			}

			foreach (Details.ContentDetail detail in replacementItem.Details.Values)
			{
				currentItem[detail.Name] = detail.Value;
			}
			foreach (Details.DetailCollection collection in replacementItem.DetailCollections.Values)
			{
				Details.DetailCollection newCollection = currentItem.GetDetailCollection(collection.Name, true);
				foreach (Details.ContentDetail detail in collection.Details)
					newCollection.Add(detail.Value);
			}
		}

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
