using System;
using Castle.Core;
using N2.Definitions;
using N2.Persistence;
using N2.Web;
using N2.Edit.Trash;
using N2.Security;

namespace N2.Edit.Trash
{
	/// <summary>
	/// Can throw and restore items. Thrown items are moved to a trash 
	/// container item.
	/// </summary>
	public class TrashHandler : ITrashHandler
	{
		public const string FormerName = "FormerName";
		public const string FormerParent = "FormerParent";
		public const string FormerExpires = "FormerExpires";
		public const string DeletedDate = "DeletedDate";
		private readonly IPersister persister;
		private readonly IDefinitionManager definitions;
		private readonly IHost host;

		public TrashHandler(IPersister persister, IDefinitionManager definitions, IHost host)
		{
			this.persister = persister;
			this.definitions = definitions;
			this.host = host;
		}

		ITrashCan ITrashHandler.TrashContainer
		{
			get { return GetTrashContainer(true) as ITrashCan; }
		}

		public TrashContainerItem GetTrashContainer(bool create)
		{
			ContentItem rootItem = persister.Get(host.DefaultSite.RootItemID);
			TrashContainerItem trashContainer = rootItem.GetChild("Trash") as TrashContainerItem;
			if (create && trashContainer == null)
			{
				trashContainer = definitions.CreateInstance<TrashContainerItem>(rootItem);
				trashContainer.Name = "Trash";
				trashContainer.Title = "Trash";
				trashContainer.Visible = false;
				trashContainer.AuthorizedRoles.Add(new AuthorizedRole(trashContainer, "admin"));
				trashContainer.AuthorizedRoles.Add(new AuthorizedRole(trashContainer, "Editors"));
				trashContainer.AuthorizedRoles.Add(new AuthorizedRole(trashContainer, "Administrators"));
				trashContainer.SortOrder = int.MaxValue - 1000000;
				persister.Save(trashContainer);
			}
			return trashContainer;
		}

		public bool CanThrow(ContentItem affectedItem)
		{
			TrashContainerItem trash = GetTrashContainer(false);
            bool enabled = trash == null || trash.Enabled;
            bool alreadyThrown = IsInTrash(affectedItem);
            bool throwable = affectedItem.GetType().GetCustomAttributes(typeof(NotThrowableAttribute), true).Length == 0;
			return enabled && !alreadyThrown && throwable;
		}

		public virtual void Throw(ContentItem item)
		{
			ExpireTrashedItem(item);
            item.AddTo(GetTrashContainer(true));

			persister.Save(item);
		}

		public virtual void ExpireTrashedItem(ContentItem item)
		{
			item[FormerName] = item.Name;
			item[FormerParent] = item.Parent;
			item[FormerExpires] = item.Expires;
			item[DeletedDate] = DateTime.Now;
			item.Expires = DateTime.Now;
			item.Name = item.ID.ToString();

            foreach (ContentItem child in item.Children)
                ExpireTrashedItem(child);
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
			item.Name = (string)item["FormerName"];
			item.Expires = (DateTime?)item["FormerExpires"];

			item["FormerName"] = null;
			item["FormerParent"] = null;
			item["FormerExpires"] = null;
			item["DeletedDate"] = null;

            foreach (ContentItem child in item.Children)
                RestoreValues(child);
		}

		public bool IsInTrash(ContentItem item)
		{
            TrashContainerItem trash = GetTrashContainer(false);
			return trash != null && Find.IsDescendantOrSelf(item, trash);
		}
	}
}
