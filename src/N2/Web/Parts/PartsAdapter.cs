using System.Collections.Generic;
using N2.Collections;
using N2.Engine;
using N2.Definitions;
using System.Security.Principal;
using N2.Web.UI;
using System.Web.UI;

namespace N2.Web.Parts
{
	/// <summary>
	/// Controls aspects related to zones, zone definitions, and items to display in a zone.
	/// </summary>
	[Controls(typeof(ContentItem))]
	public class PartsAdapter : AbstractContentAdapter
	{
		/// <summary>Retrieves content items added to a zone of the parnet item.</summary>
		/// <param name="parentItem">The item whose items to get.</param>
		/// <param name="zoneName">The zone in which the items should be contained.</param>
		/// <returns>A list of items in the zone.</returns>
		public virtual ItemList GetItemsInZone(ContentItem parentItem, string zoneName)
		{
			if(parentItem == null)
				return new ItemList();

			return parentItem.GetChildren(zoneName);
		}

		/// <summary>Retrieves allowed item definitions.</summary>
		/// <param name="parentItem">The parent item.</param>
		/// <param name="zoneName">The zone where children would be placed.</param>
		/// <param name="user">The user to restrict access for.</param>
		/// <returns>Item definitions allowed by zone, parent restrictions and security.</returns>
		public virtual IEnumerable<ItemDefinition> GetAllowedDefinitions(ContentItem parentItem, string zoneName, IPrincipal user)
		{
			ItemDefinition containerDefinition = Engine.Definitions.GetDefinition(parentItem.GetType());

			foreach (ItemDefinition childDefinition in containerDefinition.AllowedChildren)
			{
				if (childDefinition.IsAllowedInZone(zoneName) && childDefinition.Enabled && childDefinition.IsAuthorized(user))
				{
					yield return childDefinition;
				}
			}
		}

		/// <summary>Adds a content item part to a containing control hierarchy (typically a zone control).</summary>
		/// <param name="item">The item to add a part.</param>
		/// <param name="container">The container control to host the part user interface.</param>
		public virtual Control AddChildPart(ContentItem item, Control container)
		{
			IAddablePart addablePart = item as IAddablePart;
			if (addablePart != null)
			{
				return addablePart.AddTo(container);
			}

			string templateUrl = GetTemplateUrl(item);
			if(string.IsNullOrEmpty(templateUrl))
				return null;

			return ItemUtility.AddUserControl(templateUrl, container, item);
		}

		/// <summary>Gets the path to the given item's template. This is a way to override the default template provided by the content item.</summary>
		/// <param name="item">The item whose path is requested.</param>
		/// <returns>The virtual path of the template or null if the item is not supposed to be added.</returns>
		protected virtual string GetTemplateUrl(ContentItem item)
		{
			return item.FindPath(PathData.DefaultAction).TemplateUrl;
		}
	}
}