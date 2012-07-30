using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using N2.Edit.Workflow;
using N2.Engine;
using N2.Persistence;
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
		private readonly ITemplateProvider[] providers;
		private readonly ContentActivator activator;
		private readonly StateChanger stateChanger;

		public DefinitionManager(IDefinitionProvider[] definitionProviders, ITemplateProvider[] templateProviders, ContentActivator activator, StateChanger changer)
		{
			this.definitionProviders = definitionProviders.OrderBy(dp => dp.SortOrder).ToArray();
			this.providers = templateProviders.OrderBy(tp => tp.SortOrder).ToArray();
			this.activator = activator;
			this.stateChanger = changer;

			activator.Initialize(GetDefinitions());
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
			if (itemType == null) return null;

			return GetDefinitions().FirstOrDefault(d => d.ItemType == itemType);
		}

		/// <summary>Gets item definition for a certain discriminator.</summary>
		/// <param name="discriminator">The discriminator/name that uniquely identifies a certain type.</param>
		/// <returns>The definition matching the string.</returns>
		public virtual ItemDefinition GetDefinition(string discriminator)
		{
			if (discriminator == null) throw new ArgumentNullException("discriminator");

			var definitionTemplatePair = discriminator.Split('/');
			if (definitionTemplatePair.Length > 1)
			{
				foreach (ItemDefinition definition in GetDefinitions())
					if (definition.Discriminator == definitionTemplatePair[0] && definition.TemplateKey == definitionTemplatePair[1])
						return definition;
			}
			else
			{
				foreach (ItemDefinition definition in GetDefinitions())
					if (definition.Discriminator == discriminator)
						return definition;
			}

			return null;
		}

		/// <summary>Gets the definition for a certain item.</summary>
		/// <param name="item">The item whose definition we want.</param>
		/// <returns>The definition matching a certain item.</returns>
		public virtual ItemDefinition GetDefinition(ContentItem item)
		{
			var t = GetTemplate(item);
			if (t != null)
				return t.Definition;

			return GetDefinition(item.GetContentType());
		}

		/// <summary>Gets all item definitions.</summary>
		/// <returns>A collection of item definitoins.</returns>
		public virtual IEnumerable<ItemDefinition> GetDefinitions()
		{
			return definitionProviders.SelectMany(dp => dp.GetDefinitions());
		}

		/// <summary>Gets child types allowed below a certain item and zone.</summary>
		/// <param name="parentItem">The parent item whose allowed children to get.</param>
		/// <returns>A list of definitions allowed by the given criterias.</returns>
		public virtual IEnumerable<ItemDefinition> GetAllowedChildren(ContentItem parentItem)
		{
			var definition = GetDefinition(parentItem);
			var allowedChildItems = definition.GetAllowedChildren(this, parentItem)
				.ToList();
			allowedChildItems.Sort();
			return allowedChildItems;
		}

		/// <summary>Gets child types allowed below a certain item and zone.</summary>
		/// <param name="parentItem">The parent item whose allowed children to get.</param>
		/// <param name="zoneName">The zone name.</param>
		/// <returns>A list of definitions allowed by the given criterias.</returns>
		public virtual IEnumerable<ItemDefinition> GetAllowedChildren(ContentItem parentItem, string zoneName)
		{
			return GetAllowedChildren(parentItem)
				.Where(d => d.IsAllowedInZone(zoneName));
		}

		/// <summary>Gets items allowed below this item in a certain zone.</summary>
		/// <param name="parentItem">The parent whose allowed children to get.</param>
		/// <param name="zoneName">The zone whose allowed child item types to get.</param>
		/// <param name="user">The user whose access to query.</param>
		/// <returns>A list of items allowed in the zone the user is authorized to create.</returns>
		[Obsolete("Use GetAllowedChildren(parentItem, zoneName).Where(d => Security.IsAuthorized(d, ...))")]
		public virtual IList<ItemDefinition> GetAllowedChildren(ContentItem parentItem, string zoneName, IPrincipal user)
		{
			return GetAllowedChildren(parentItem, zoneName)
				.Where(d => d.IsAuthorized(user)).ToList();
		}

		/// <summary>Gets items allowed below this item in a certain zone.</summary>
		/// <param name="parentItem">The parent whose allowed children to get.</param>
		/// <param name="zoneName">The zone whose allowed child item types to get.</param>
		/// <param name="user">The user whose access to query.</param>
		/// <returns>A list of items allowed in the zone the user is authorized to create.</returns>
		[Obsolete("Use GetAllowedChildren(parentItem, ...)")]
		public virtual IList<ItemDefinition> GetAllowedChildren(ItemDefinition parentDefinition, string zoneName, IPrincipal user)
		{
			var allowedChildItems = parentDefinition.GetAllowedChildren(this, null).Where(d => d.IsAllowed(zoneName, user)).ToList();
			allowedChildItems.Sort();
			return allowedChildItems;
		}

		/// <summary>Notifies subscriber that an item was created through a <see cref="CreateInstance"/> method.</summary>
		[Obsolete]
		public event EventHandler<ItemEventArgs> ItemCreated;


		public virtual IEnumerable<TemplateDefinition> GetTemplates(Type contentType)
		{
			if (contentType == null) return new TemplateDefinition[0];

			var templates = providers.SelectMany(tp => tp.GetTemplates(contentType)).ToList();
			if (!templates.Any(t => t.ReplaceDefault))
				return templates;
			return templates.Where(t => t.Name != null).ToList();
		}

		public virtual TemplateDefinition GetTemplate(Type contentType, string templateKey)
		{
			if (contentType == null) return null;

			return providers
				.SelectMany(tp => tp.GetTemplates(contentType))
				.FirstOrDefault(td => string.Equals(td.Name ?? "", templateKey ?? ""));
		}

		public virtual TemplateDefinition GetTemplate(ContentItem item)
		{
			if (item == null) return null;

			return providers.Select(tp => tp.GetTemplate(item)).FirstOrDefault(t => t != null);
		}

		#region IAutoStart Members

		public void Start()
		{
			activator.ItemCreated += activator_ItemCreated;
			stateChanger.StateChanged += stateChanger_StateChanged;
		}

		public void Stop()
		{
			activator.ItemCreated -= activator_ItemCreated;
			stateChanger.StateChanged -= stateChanger_StateChanged;
		}

		void stateChanger_StateChanged(object sender, StateChangedEventArgs e)
		{
			foreach (var ct in GetDefinition(e.AffectedItem).ContentTransformers.Where(ct => (ct.ChangingTo & e.AffectedItem.State) == e.AffectedItem.State))
				ct.Transform(e.AffectedItem);
		}

		void activator_ItemCreated(object sender, ItemEventArgs e)
		{
			if (ItemCreated != null)
				ItemCreated(this, e);
		}

		#endregion
	}
}
