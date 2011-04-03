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
using System.Diagnostics;

namespace N2.Definitions
{
	/// <summary>
	/// Stores item definitions and constructs new items.
	/// </summary>
	[Service(typeof(IDefinitionManager))]
	public class DefinitionManager : IDefinitionManager, IAutoStart
	{
		private readonly IDefinitionProvider[] definitionProviders;
		private readonly ContentActivator activator;

		public DefinitionManager(IDefinitionProvider[] definitionProviders, ContentActivator activator)
		{
			this.definitionProviders = definitionProviders;
			this.activator = activator;
		}

		/// <summary>Creates an instance of a certain type of item. It's good practice to create new items through this method so the item's dependencies can be injected by the engine.</summary>
		/// <returns>A new instance of an item.</returns>
		[Obsolete]
		public T CreateInstance<T>(ContentItem parentItem) where T : ContentItem
		{
			return activator.CreateInstance<T>(parentItem);
		}

		/// <summary>Creates an instance of a certain type of item. It's good practice to create new items through this method so the item's dependencies can be injected by the engine.</summary>
		/// <returns>A new instance of an item.</returns>
		[Obsolete]
		public virtual ContentItem CreateInstance(Type itemType, ContentItem parentItem)
		{
			return activator.CreateInstance(itemType, parentItem);
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

		/// <summary>Gets the definition for a certain item.</summary>
		/// <param name="item">The item whose definition we want.</param>
		/// <returns>The definition matching a certain item.</returns>
		public virtual ItemDefinition GetDefinition(ContentItem item)
		{
			return GetDefinition(item.GetContentType());
		}

		/// <summary>Gets all item definitions.</summary>
		/// <returns>A collection of item definitoins.</returns>
		public virtual IEnumerable<ItemDefinition> GetDefinitions()
		{
			return definitionProviders.SelectMany(dp => dp.GetDefinitions());
		}

		/// <summary>Gets items allowed below this item in a certain zone.</summary>
		/// <param name="parentItem">The parent whose allowed children to get.</param>
		/// <param name="zoneName">The zone whose allowed child item types to get.</param>
		/// <param name="user">The user whose access to query.</param>
		/// <returns>A list of items allowed in the zone the user is authorized to create.</returns>
		public virtual IList<ItemDefinition> GetAllowedChildren(ContentItem parentItem, string zoneName, IPrincipal user)
		{
			List<ItemDefinition> allowedChildItems = new List<ItemDefinition>();
			var definition = GetDefinition(parentItem.GetContentType());
			foreach (ItemDefinition childDefinition in definition.GetAllowedChildren(this, parentItem))
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
		[Obsolete]
		public event EventHandler<ItemEventArgs> ItemCreated;

		#region IAutoStart Members

		public void Start()
		{
			Debug.WriteLine("DefinitionManager.Start");
			activator.Initialize(definitionProviders.SelectMany(dp => dp.GetDefinitions()).Select(d => d.ItemType));
			activator.ItemCreated += new EventHandler<ItemEventArgs>(activator_ItemCreated);
		}

		void activator_ItemCreated(object sender, ItemEventArgs e)
		{
			if (ItemCreated != null)
				ItemCreated(this, e);
		}

		public void Stop()
		{
			activator.ItemCreated -= new EventHandler<ItemEventArgs>(activator_ItemCreated);
		}

		#endregion
	}
}
