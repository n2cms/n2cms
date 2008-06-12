using System;
using System.Collections.Generic;
using N2.Persistence;
using System.Security.Principal;
using N2.Security;

namespace N2.Definitions
{
	/// <summary>
	/// Stores item definitions and constructs new items.
	/// </summary>
	public class DefinitionManager : IDefinitionManager
	{
		private readonly IDictionary<Type, ItemDefinition> definitions;
		private readonly IItemNotifier notifier;

		public DefinitionManager(DefinitionBuilder builder, IItemNotifier notifier)
		{
			definitions = builder.GetDefinitions();
			this.notifier = notifier;
		}

		/// <summary>Creates an instance of a certain type of item. It's good practice to create new items through this method so the item's dependencies can be injected by the engine.</summary>
		/// <returns>A new instance of an item.</returns>
		public T CreateInstance<T>(ContentItem parentItem) where T : ContentItem
		{
			T item = Activator.CreateInstance<T>();
			OnItemCreating(item, parentItem);
			return item;
		}

		/// <summary>Creates an instance of a certain type of item. It's good practice to create new items through this method so the item's dependencies can be injected by the engine.</summary>
		/// <returns>A new instance of an item.</returns>
		public ContentItem CreateInstance(Type itemType, ContentItem parentItem)
		{
			ContentItem item = Activator.CreateInstance(itemType) as ContentItem;
			OnItemCreating(item, parentItem);
			return item;
		}

		protected virtual void OnItemCreating(ContentItem item, ContentItem parentItem)
		{
            if (parentItem != null)
			{
				ItemDefinition parentDefinition = GetDefinition(parentItem.GetType());
				ItemDefinition itemDefinition = GetDefinition(item.GetType());

				if(!parentDefinition.IsChildAllowed(itemDefinition))
					throw new NotAllowedParentException(itemDefinition, parentItem.GetType());

				item.Parent = parentItem;
				foreach (AuthorizedRole role in parentItem.AuthorizedRoles)
					item.AuthorizedRoles.Add(new AuthorizedRole(item, role.Role));
			}
			notifier.Notifiy(item);
			if (ItemCreated != null)
				ItemCreated.Invoke(this, new ItemEventArgs(item));
		}

		/// <summary>Gets the definition for a certain item type.</summary>
		/// <param name="itemType">The type of item whose definition we want.</param>
		/// <returns>The definition matching a certain item type.</returns>
		public virtual ItemDefinition GetDefinition(Type itemType)
		{
			if (itemType == null) throw new ArgumentNullException("itemType");

			if(definitions.ContainsKey(itemType))
				return definitions[itemType];
			else
				return null;
		}

		/// <summary>Gets item definition for a certain discriminator.</summary>
		/// <param name="discriminator">The discriminator/name that uniquely identifies a certain type.</param>
		/// <returns>The definition matching the string.</returns>
		public ItemDefinition GetDefinition(string discriminator)
		{
			if (discriminator == null) throw new ArgumentNullException("discriminator");

			foreach(ItemDefinition definition in definitions.Values)
				if(definition.Discriminator == discriminator)
					return definition;
			return null;
		}

		/// <summary>Gets all item definitions.</summary>
		/// <returns>A collection of item definitoins.</returns>
		public virtual ICollection<ItemDefinition> GetDefinitions()
		{
			return definitions.Values;
		}

		/// <summary>Gets items allowed below this item in a certain zone.</summary>
		/// <param name="zone">The zone whose allowed child item types to get.</param>
		/// <param name="user">The user whose access to query.</param>
		/// <returns>A list of items allowed in the zone the user is authorized to create.</returns>
		public IList<ItemDefinition> GetAllowedChildren(ItemDefinition definition, string zone, IPrincipal user)
		{
			if (!definition.HasZone(zone))
				throw new N2Exception("The definition '{0}' does not allow a zone named '{1}'.", definition.Title, zone);

			List<ItemDefinition> allowedChildItems = new List<ItemDefinition>();
			foreach (ItemDefinition childItem in definition.AllowedChildren)
			{
				if (!childItem.IsDefined)
					continue;
				if (!childItem.Enabled)
					continue;
				if (!definition.IsAllowedInZone(zone, childItem.AllowedZoneNames))
					continue;
				if (!childItem.IsAuthorized(user))
					continue;

				allowedChildItems.Add(childItem);
			}
			allowedChildItems.Sort();
			return allowedChildItems;
		}

		/// <summary>Notifies subscriber that an item was created through a <see cref="CreateInstance"/> method.</summary>
		public event EventHandler<ItemEventArgs> ItemCreated;
	}
}
