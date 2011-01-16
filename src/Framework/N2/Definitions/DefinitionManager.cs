using System;
using System.Linq;
using System.Collections.Generic;
using N2.Persistence;
using System.Security.Principal;
using N2.Security;
using N2.Edit.Workflow;
using N2.Engine;
using N2.Persistence.Proxying;
using N2.Plugin;

namespace N2.Definitions
{
	/// <summary>
	/// Stores item definitions and constructs new items.
	/// </summary>
	[Service(typeof(IDefinitionManager))]
	public class DefinitionManager : IDefinitionManager, IAutoStart
	{
		private readonly IDefinitionProvider[] definitionProviders;
		private readonly IItemNotifier notifier;
		private readonly StateChanger stateChanger;
		private readonly IProxyFactory interceptor;

		public DefinitionManager(IDefinitionProvider[] definitionProviders, StateChanger changer, IItemNotifier notifier, IProxyFactory interceptor)
		{
			this.definitionProviders = definitionProviders;
			this.stateChanger = changer;
			this.notifier = notifier;
			this.interceptor = interceptor;
		}

		/// <summary>Creates an instance of a certain type of item. It's good practice to create new items through this method so the item's dependencies can be injected by the engine.</summary>
		/// <returns>A new instance of an item.</returns>
		public T CreateInstance<T>(ContentItem parentItem) where T : ContentItem
		{
			return (T) CreateInstance(typeof(T), parentItem);
		}

		/// <summary>Creates an instance of a certain type of item. It's good practice to create new items through this method so the item's dependencies can be injected by the engine.</summary>
		/// <returns>A new instance of an item.</returns>
		public virtual ContentItem CreateInstance(Type itemType, ContentItem parentItem)
		{
			object intercepted = interceptor.Create(itemType.FullName);
			ContentItem item = (intercepted ?? Activator.CreateInstance(itemType, true))
				as ContentItem;
			stateChanger.ChangeTo(item, ContentState.New);
			OnItemCreating(item, parentItem);
			return item;
		}

		protected virtual void OnItemCreating(ContentItem item, ContentItem parentItem)
		{
			item.Parent = parentItem;
			notifier.NotifiyCreated(item);
			if (ItemCreated != null)
				ItemCreated.Invoke(this, new ItemEventArgs(item));
		}

		/// <summary>Gets the definition for a certain item type.</summary>
		/// <param name="itemType">The type of item whose definition we want.</param>
		/// <returns>The definition matching a certain item type.</returns>
		public virtual ItemDefinition GetDefinition(Type itemType)
		{
			if (itemType == null) throw new ArgumentNullException("itemType");

			return GetDefinitions().FirstOrDefault(d => d.ItemType == itemType);
		}

		/// <summary>Gets item definition for a certain discriminator.</summary>
		/// <param name="discriminator">The discriminator/name that uniquely identifies a certain type.</param>
		/// <returns>The definition matching the string.</returns>
		public virtual ItemDefinition GetDefinition(string discriminator)
		{
			if (discriminator == null) throw new ArgumentNullException("discriminator");

			foreach (ItemDefinition definition in GetDefinitions())
				if(definition.Discriminator == discriminator)
					return definition;
			return null;
		}

		/// <summary>Gets all item definitions.</summary>
		/// <returns>A collection of item definitoins.</returns>
		public virtual IEnumerable<ItemDefinition> GetDefinitions()
		{
			return definitionProviders.SelectMany(dp => dp.GetDefinitions());
		}

		/// <summary>Gets items allowed below this item in a certain zone.</summary>
		/// <param name="definition">The parent definition whose allowed children to get.</param>
		/// <param name="zoneName">The zone whose allowed child item types to get.</param>
		/// <param name="user">The user whose access to query.</param>
		/// <returns>A list of items allowed in the zone the user is authorized to create.</returns>
		public virtual IList<ItemDefinition> GetAllowedChildren(ItemDefinition definition, string zoneName, IPrincipal user)
		{
			List<ItemDefinition> allowedChildItems = new List<ItemDefinition>();
			foreach (ItemDefinition childDefinition in definition.AllowedChildren)
			{
				if (!childDefinition.IsDefined)
					continue;
				if (!childDefinition.Enabled)
					continue;
				if(!childDefinition.IsAllowedInZone(zoneName))
					continue;
				if (!childDefinition.IsAuthorized(user))
					continue;

				allowedChildItems.Add(childDefinition);
			}
			allowedChildItems.Sort();
			return allowedChildItems;
		}

		/// <summary>Notifies subscriber that an item was created through a <see cref="CreateInstance"/> method.</summary>
		public event EventHandler<ItemEventArgs> ItemCreated;

		#region IAutoStart Members

		public void Start()
		{
			interceptor.Initialize(definitionProviders.SelectMany(dp => dp.GetDefinitions()).Select(d => d.ItemType));
		}

		public void Stop()
		{
		}

		#endregion
	}
}
