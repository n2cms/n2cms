using System;
using System.Collections.Generic;
using System.Linq;
using N2.Definitions;
using N2.Edit.Workflow;
using N2.Engine;
using N2.Persistence;
using N2.Persistence.Finder;
using N2.Security;
using System.Security.Principal;
using N2.Web;
using N2.Collections;

namespace N2.Edit.Trash
{
	/// <summary>
	/// Can throw and restore items. Thrown items are moved to a trash 
	/// container item.
	/// </summary>
	[Service(typeof(ITrashHandler))]
	public class TrashHandler : ITrashHandler
	{
		public const string TrashContainerName = "Trash";
		public const string FormerName = "FormerName";
		public const string FormerState = "FormerState";
		public const string FormerParent = "FormerParent";
		public const string DeletedDate = "DeletedDate";
		private readonly IPersister persister;
		private readonly IItemFinder finder;
		private readonly ISecurityManager security;
		private readonly ContainerRepository<TrashContainerItem> container;
		private readonly StateChanger stateChanger;
		private readonly IWebContext webContext;
		Logger<TrashHandler> logger;

		/// <summary>Instructs this class to navigate rather than query for items.</summary>
		public bool UseNavigationMode
		{
			get { return container.Navigate; }
			set { container.Navigate = value; }
		}

		public TrashHandler(IPersister persister, IItemFinder finder, ISecurityManager security, ContainerRepository<TrashContainerItem> container, StateChanger stateChanger, IWebContext webContext)
		{
			this.finder = finder;
			this.persister = persister;
			this.security = security;
			this.container = container;
			this.stateChanger = stateChanger;
			this.webContext = webContext;
		}

        /// <summary>The container of thrown items.</summary>
		ITrashCan ITrashHandler.TrashContainer
		{
			get { return GetTrashContainer(false) as ITrashCan; }
		}

		public TrashContainerItem GetTrashContainer(bool create)
		{
			if (!create)
				container.GetBelowRoot();

			return container.GetOrCreateBelowRoot((trashContainer) =>
			{
				trashContainer.Name = TrashContainerName;
				trashContainer.Title = TrashContainerName;
				trashContainer.Visible = false;
				trashContainer.AuthorizedRoles.Add(new AuthorizedRole(trashContainer, "admin"));
				trashContainer.AuthorizedRoles.Add(new AuthorizedRole(trashContainer, "Editors"));
				trashContainer.AuthorizedRoles.Add(new AuthorizedRole(trashContainer, "Administrators"));
				trashContainer.SortOrder = 1000000;
			});
		}

        /// <summary>Checks if the trash is enabled, the item is not already thrown and for the NotThrowable attribute.</summary>
        /// <param name="affectedItem">The item to check.</param>
        /// <returns>True if the item may be thrown.</returns>
		public bool CanThrow(ContentItem affectedItem)
		{
			TrashContainerItem trash = GetTrashContainer(false);
            bool enabled = trash == null || trash.Enabled;
            bool alreadyThrown = IsInTrash(affectedItem);

			var throwables = (ThrowableAttribute[])affectedItem.GetContentType().GetCustomAttributes(typeof(ThrowableAttribute), true);
			bool throwable = throwables.Length == 0 || throwables[0].Throwable == AllowInTrash.Yes;
			return enabled && !alreadyThrown && throwable;
		}

        /// <summary>Throws an item in a way that it later may be restored to it's original location at a later stage.</summary>
        /// <param name="item">The item to throw.</param>
		public virtual void Throw(ContentItem item)
		{
            CancellableItemEventArgs args = Invoke<CancellableItemEventArgs>(ItemThrowing, new CancellableItemEventArgs(item));
            if (!args.Cancel)
            {
                item = args.AffectedItem;

                ExpireTrashedItem(item);

                try
                {
					persister.Move(item, GetTrashContainer(true));
                }
                catch (PermissionDeniedException ex)
                {
                    throw new PermissionDeniedException("Permission denied while moving item to trash. Try disabling security checks using N2.Context.Security or preventing items from beeing moved to the trash with the [NonThrowable] attribute", ex);
                }

                Invoke<ItemEventArgs>(ItemThrowed, new ItemEventArgs(item));
            }
		}

        /// <summary>Expires an item that has been thrown so that it's not accessible to external users.</summary>
        /// <param name="item">The item to restore.</param>
		public virtual void ExpireTrashedItem(ContentItem item)
		{
			item[FormerName] = item.Name;
			item[FormerParent] = item.Parent;
			item[FormerState] = (int)item.State;
			item[DeletedDate] = DateTime.Now;
			item.Name = item.ID.ToString();
			
			ExpireTrashedItemsRecursive(item);
		}

		private void ExpireTrashedItemsRecursive(ContentItem item)
		{
			if (item.State == ContentState.Published)
				stateChanger.ChangeTo(item, ContentState.Deleted);
			foreach (ContentItem child in item.Children)
				ExpireTrashedItemsRecursive(child);
		}

		/// <summary>Restores an item to the original location.</summary>
		/// <param name="item">The item to restore.</param>
		public virtual void Restore(ContentItem item)
		{
			ContentItem parent = (ContentItem)item["FormerParent"];
			RestoreValues(item);
            persister.Save(item);
			persister.Move(item, parent);
		}

		/// <summary>Removes expiry date and metadata set during throwing.</summary>
		/// <param name="item">The item to reset.</param>
		public virtual void RestoreValues(ContentItem item)
		{
			if (item[FormerName] != null)
				item.Name = (string)item["FormerName"];
			if (item[FormerState] != null)
				stateChanger.ChangeTo(item, (ContentState)item[FormerState]);
			item[FormerName] = null;
			item[FormerParent] = null;
			item[FormerState] = null;
			item[DeletedDate] = null;

			RestoreValuesRecursive(item);
		}

		private void RestoreValuesRecursive(ContentItem item)
		{
			if (item.State == ContentState.Deleted)
				stateChanger.ChangeTo(item, ContentState.Published);
			foreach (ContentItem child in item.Children)
				RestoreValuesRecursive(child);
		}

        /// <summary>Determines wether an item has been thrown away.</summary>
        /// <param name="item">The item to check.</param>
        /// <returns>True if the item is in the scraps.</returns>
		public bool IsInTrash(ContentItem item)
		{
            TrashContainerItem trash = GetTrashContainer(false);
			return trash != null && Find.IsDescendantOrSelf(item, trash);
		}

        protected virtual T Invoke<T>(EventHandler<T> handler, T args)
            where T : ItemEventArgs
        {
			if (handler != null && !args.AffectedItem.VersionOf.HasValue)
                handler.Invoke(this, args);
            return args;
        }

		/// <summary>Delete items lying in trash for longer than the specified interval.</summary>
		public void PurgeOldItems()
		{
			TrashContainerItem trash = GetTrashContainer(false);
			if (trash == null) return;
			if (trash.PurgeInterval == TrashPurgeInterval.Never) return;

			DateTime tresholdDate = Utility.CurrentTime().AddDays(-(int)trash.PurgeInterval);
			IList<ContentItem> expiredItems = null;

			if (UseNavigationMode)
				expiredItems = trash.Children
					.Where(i => i[DeletedDate] != null)
					.Where(i => ((DateTime)i[DeletedDate]) < tresholdDate)
					.ToList();
			else
				expiredItems = finder.Where.Parent.Eq(trash)
					.And.Detail(TrashHandler.DeletedDate).Le(tresholdDate)
					.Select();
			
			try
			{
				security.ScopeEnabled = false;
				foreach (var item in expiredItems)
				{
					persister.Delete(item);
				}	
			}
			finally
			{
				security.ScopeEnabled = true;
			}
		}

		public void PurgeAll(Action<PurgingStatus> onProgress = null)
		{
			logger.InfoFormat("Purging all trash");
			var container = GetTrashContainer(create: false);
			if (container != null)
			{
				var children = container.GetChildren(new AccessFilter(webContext.User, security)).ToList()
					.Select(c => new { Item = c, DescendantCount = persister.Repository.CountDescendants(c) }).ToList();

				int deletedCount = 0;
				int total = children.Sum(c => c.DescendantCount);
				foreach (var child in children)
				{
					logger.InfoFormat("Purging {0} and {1} descendants", child, child.DescendantCount);
					if (onProgress != null)
						onProgress(new PurgingStatus { Deleted = deletedCount, Remaining = total - deletedCount });

					persister.Delete(child.Item);
					deletedCount += child.DescendantCount;

					if (onProgress != null)
						onProgress(new PurgingStatus { Deleted = deletedCount, Remaining = total - deletedCount });
				}
			}
		}

		public void Purge(ContentItem itemToDelete, Action<PurgingStatus> onProgress = null)
		{
			var count = persister.Repository.CountDescendants(itemToDelete);

			if (onProgress != null)
				onProgress(new PurgingStatus { Deleted = 0, Remaining = count });

			logger.InfoFormat("Purging {0} and {1} descendants", itemToDelete, count);

			persister.Delete(itemToDelete);
			if (onProgress != null)
				onProgress(new PurgingStatus { Deleted = count, Remaining = 0 });
		}

        /// <summary>Occurs before an item is thrown.</summary>
        public event EventHandler<CancellableItemEventArgs> ItemThrowing;
        /// <summary>Occurs after an item has been thrown.</summary>
        public event EventHandler<ItemEventArgs> ItemThrowed;
	}
}
