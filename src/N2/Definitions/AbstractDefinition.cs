using System.Collections.Generic;
using N2.Web;
using N2.Definitions.Static;

namespace N2.Definitions
{
	/// <summary>
	/// Generalizes features shared by <see cref="PartDefinitionAttribute"/> and <see cref="PageDefinitionAttribute"/>.
	/// </summary>
	public abstract class AbstractDefinition : AbstractDefinitionRefiner, IDefinitionRefiner, IPathFinder, IDescriptionRefiner
	{
		/// <summary>The title used to present the item type when adding it.</summary>
		public string Title { get; set; }

		/// <summary>Path to the ASPX template used to present this page to visitors. Similar behaviour can be accomplished using <see cref="TemplateAttribute"/> or <see cref="ConventionTemplateAttribute"/></summary>
		public string TemplateUrl { get; set; }

		/// <summary>The order of this definition when creating items.</summary>
		public int SortOrder { get; set; }

		/// <summary>The name/discriminator used when storing and loading the item to the database. By default the name/discriminator is the class name.</summary>
		public string Name { get; set; }

		/// <summary>Gets or sets the tooltip used when presenting this item type to editors.</summary>
		public string ToolTip { get; set; }

		/// <summary>Gets or sets the description of this item.</summary>
		public string Description { get; set; }

		/// <summary>The icon url is presented to editors and is used distinguish item types when creating and managing content nodes.</summary>
		public string IconUrl { get; set; }
		
		/// <summary>Whether the defined item is a page or not. This affects whether the item is displayed in the edit tree and how it's url is calculated.</summary>
		public bool IsPage { get; protected set; }

		protected AbstractDefinition()
		{
			RefinementOrder = RefineOrder.First;
		}

		#region ISortableRefiner Members
		
		public override void Refine(ItemDefinition currentDefinition, IList<ItemDefinition> allDefinitions)
		{
			currentDefinition.Title = Title ?? currentDefinition.ItemType.Name;
			currentDefinition.ToolTip = ToolTip ?? "";
			currentDefinition.SortOrder = SortOrder;
			currentDefinition.Description = Description ?? "";
			currentDefinition.Discriminator = Name ?? currentDefinition.ItemType.Name;

			currentDefinition.IsDefined = true;
		}

		#endregion

		#region IPathFinder Members

		public PathData GetPath(ContentItem item, string remainingUrl)
		{
			if (string.IsNullOrEmpty(remainingUrl) && !string.IsNullOrEmpty(TemplateUrl))
			{
				return new PathData(item, TemplateUrl);
			}
			return null;
		}

		#endregion

		#region IDescriptionRefiner Members

		void IDescriptionRefiner.Describe(System.Type entityType, Description description)
		{
			description.IsPage = IsPage;
			description.IconUrl = IconUrl;
		}

		#endregion
	}
}