#region License
/* Copyright (C) 2006-2007 Cristian Libardo
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 */
#endregion

using System;
using Castle.Core;
using N2.Definitions;
using N2.Persistence;
using N2.Web;

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
		readonly IPersister persister;
		readonly IDefinitionManager definitions;
		readonly Site site;

		public TrashHandler(IPersister persister, IDefinitionManager definitions, Site site)
		{
			this.persister = persister;
			this.definitions = definitions;
			this.site = site;
		}

		ITrashCan ITrashHandler.TrashContainer
		{
			get { return TrashContainer as ITrashCan; }
		}

		public TrashContainerItem TrashContainer
		{
			get
			{
				ContentItem rootItem = persister.Get(site.RootItemID);
				TrashContainerItem trashContainer = rootItem.GetChild("Trash") as TrashContainerItem;
				if (trashContainer == null)
				{
					trashContainer = definitions.CreateInstance<TrashContainerItem>(rootItem);
					trashContainer.Name = "Trash";
					trashContainer.Title = "Trash";
					trashContainer.Visible = false;
					trashContainer.AuthorizedRoles.Add(new Security.AuthorizedRole(trashContainer, "admin"));
					trashContainer.AuthorizedRoles.Add(new Security.AuthorizedRole(trashContainer, "Editors"));
					trashContainer.AuthorizedRoles.Add(new Security.AuthorizedRole(trashContainer, "Administrators"));
					trashContainer.SortOrder = int.MaxValue - 1000000;
					persister.Save(trashContainer);
				}
				return trashContainer;
			}
		}

		public bool CanThrow(ContentItem affectedItem)
		{
			TrashContainerItem trash = TrashContainer;
			return trash != null 
				&& trash.Enabled
				&& trash != affectedItem 
				&& trash != affectedItem.Parent;
		}

		public virtual void Throw(ContentItem item)
		{
			ExpireTrashedItem(item);
			item.AddTo(TrashContainer);

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

			persister.Save(item);
		}

		/// <summary>Restores an item to the original location.</summary>
		/// <param name="item">The item to restore.</param>
		public virtual void Restore(ContentItem item)
		{
			ContentItem parent = (ContentItem)item["FormerParent"];
			RestoreValues(item);

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

			persister.Save(item);
		}

		public bool IsInTrash(ContentItem item)
		{
			return Find.IsDescendantOrSelf(item, TrashContainer);
		}
	}
}
