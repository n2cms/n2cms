using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web.UI;
using N2.Collections;
using N2.Definitions;
using N2.Engine;
using N2.Persistence;
using N2.Web.UI;
using N2.Security;
using N2.Edit;

namespace N2.Web.Parts
{
	/// <summary>
	/// Adapts operations related to zones, zone definitions, and items to display in a zone.
	/// </summary>
	[Adapts(typeof(ContentItem))]
	public class PartsAdapter : AbstractContentAdapter
	{
		IContentAdapterProvider adapters;
		IWebContext webContext;
		IPersister persister;
		IDefinitionManager definitions;
		IEnumerable<ITemplateProvider> templates;
		ISecurityManager security;

		public IPersister Persister
		{
			get { return persister ?? engine.Resolve<IPersister>(); }
			set { persister = value; }
		}

		public IWebContext WebContext
		{
			get { return webContext ?? engine.Resolve<IWebContext>(); }
			set { webContext = value; }
		}

		public IContentAdapterProvider Adapters
		{
			get { return adapters ?? engine.Resolve<IContentAdapterProvider>(); }
			set { adapters = value; }
		}

		public IDefinitionManager Definitions
		{
			get { return definitions ?? engine.Definitions; }
			set { definitions = value; }
		}

		public IEnumerable<ITemplateProvider> Templates
		{
			get { return templates ?? engine.Container.ResolveAll<ITemplateProvider>(); }
			set { templates = value; }
		}

		public ISecurityManager Security
		{
			get { return security ?? engine.Container.Resolve<ISecurityManager>(); }
			set { security = value; }
		}

        /// <summary>Retrieves content items added to a zone of the parnet item.</summary>
		/// <param name="parentItem">The item whose items to get.</param>
		/// <param name="zoneName">The zone in which the items should be contained.</param>
		/// <returns>A list of items in the zone.</returns>
        [Obsolete("Use overload with interface parameter")]
        public virtual ItemList GetItemsInZone(ContentItem parentItem, string zoneName)
        {
            return new ItemList(GetParts(parentItem, zoneName, Interfaces.Viewing));
        }

		/// <summary>Retrieves content items added to a zone of the parnet item.</summary>
		/// <param name="belowParentItem">The item whose items to get.</param>
		/// <param name="inZoneNamed">The zone in which the items should be contained.</param>
        /// <param name="filteredForInterface">Interface where the parts are displayed.</param>
		/// <returns>A list of items in the zone.</returns>
		public virtual IEnumerable<ContentItem> GetParts(ContentItem belowParentItem, string inZoneNamed, string filteredForInterface)
		{
			if(belowParentItem == null)
				return new ItemList();

			var children = !belowParentItem.VersionOf.HasValue ? belowParentItem.Children : belowParentItem.VersionOf.Children;
            var items = children.FindParts(inZoneNamed)
                .Where(new AccessFilter(WebContext.User, Security));

            if(filteredForInterface == Interfaces.Viewing)
                items = items.Where(new PublishedFilter());

            return items;
		}

		/// <summary>Retrieves allowed item definitions.</summary>
		/// <param name="parentItem">The parent item.</param>
		/// <param name="zoneName">The zone where children would be placed.</param>
		/// <param name="user">The user to restrict access for.</param>
		/// <returns>Item definitions allowed by zone, parent restrictions and security.</returns>
		public virtual IEnumerable<ItemDefinition> GetAllowedDefinitions(ContentItem parentItem, string zoneName, IPrincipal user)
		{
			return Definitions.GetAllowedChildren(parentItem, zoneName)
				.WhereAuthorized(Security, user, parentItem);
		}

		/// <summary>Retrieves allowed item definitions.</summary>
		/// <param name="parentItem">The parent item.</param>
		/// <param name="user">The user to restrict access for.</param>
		/// <returns>Item definitions allowed by zone, parent restrictions and security.</returns>
		public virtual IEnumerable<ItemDefinition> GetAllowedDefinitions(ContentItem parentItem, IPrincipal user)
		{
			return Definitions.GetAllowedChildren(parentItem)
                .Where(d => d.Enabled && d.AllowedIn != Integrity.AllowedZones.None && d.Enabled)
				.WhereAuthorized(Security, user, parentItem);
		}

		/// <summary>Adds a content item part to a containing control hierarchy (typically a zone control). Override this method to adapt how a parent gets it's children added.</summary>
		/// <param name="item">The item to add a part.</param>
		/// <param name="container">The container control to host the part user interface.</param>
		public virtual Control AddChildPart(ContentItem item, Control container)
		{
			var adapter = Adapters.ResolveAdapter<PartsAdapter>(item);
			return adapter.AddTo(item, container);
		}

		/// <summary>Adds a content part to a containing control. Override this method to adapt how a part is added to a parent.</summary>
		/// <param name="item"></param>
		/// <param name="container"></param>
		/// <returns></returns>
		public virtual Control AddTo(ContentItem item, Control container)
		{
			IAddablePart addablePart = item as IAddablePart;
			if (addablePart != null)
			{
				return addablePart.AddTo(container);
			}

			string templateUrl = GetTemplateUrl(item);
			if (string.IsNullOrEmpty(templateUrl))
				return null;

			return ItemUtility.AddUserControl(Url.ResolveTokens(templateUrl), container, item);
		}

		/// <summary>Gets the path to the given item's template. This is a way to override the default template provided by the content item.</summary>
		/// <param name="item">The item whose path is requested.</param>
		/// <returns>The virtual path of the template or null if the item is not supposed to be added.</returns>
		protected virtual string GetTemplateUrl(ContentItem item)
		{
			return item.FindPath(PathData.DefaultAction).TemplateUrl;
		}

		public virtual IEnumerable<TemplateDefinition> GetTemplates(ContentItem item, ItemDefinition definition)
		{
			return Definitions.GetTemplates(definition.ItemType);
		}
	}
}