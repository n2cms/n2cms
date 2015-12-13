using System;
using N2.Web;
using N2.Security;

namespace N2.Definitions
{
    /// <summary>
    /// Generalizes features shared by <see cref="PartDefinitionAttribute"/> and <see cref="PageDefinitionAttribute"/>.
    /// </summary>
    public abstract class AbstractDefinition : Attribute, ISimpleDefinitionRefiner, IPathFinder
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

        /// <summary>A class used by a CSS sprite on the client to display an icon.</summary>
        public string IconClass { get; set; }

        /// <summary>Whether the defined item is a page or not. This affects whether the item is displayed in the edit tree and how it's url is calculated.</summary>
        public bool IsPage { get; protected set; }

        /// <summary>Roles allowed to create/edit/delete items of this type.</summary>
        public string[] AuthorizedRoles { get; set; }

        /// <summary>Permission required to create/edit/delete items of this type.</summary>
        public Permission RequiredPermission { get; set; }

		/// <summary>A helpful text available when editing the page.</summary>
		public string HelpText { get; set; }

		/// <summary>A text available displayed editing the page.</summary>
		public string EditingInstructions { get; set; }

		protected AbstractDefinition()
        {
            SortOrder = int.MinValue;
        }

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

        #region ISimpleDefinitionRefiner Members

        protected virtual string DefaultIconUrl
        {
            get { return "{ManagementUrl}/Resources/icons/page.png"; }
        }

        protected virtual string DefaultIconClass
        {
            get { return "fa fa-file"; }
        }

        public virtual void Refine(ItemDefinition currentDefinition)
        {
            currentDefinition.Title = Title ?? currentDefinition.Title ?? currentDefinition.ItemType.Name;
            if (!string.IsNullOrEmpty(ToolTip))
                currentDefinition.ToolTip = ToolTip ?? "";
            if (SortOrder > int.MinValue)
                currentDefinition.SortOrder = SortOrder;
            if (!string.IsNullOrEmpty(Description))
                currentDefinition.Description = Description;
            currentDefinition.Discriminator = Name ?? currentDefinition.ItemType.Name;
            currentDefinition.IconUrl = IconUrl ?? currentDefinition.IconUrl;
            currentDefinition.IconClass = IconClass ?? currentDefinition.IconClass ?? DefaultIconClass;
            currentDefinition.IsPage = IsPage;
			currentDefinition.HelpText = HelpText ?? currentDefinition.HelpText;
			currentDefinition.EditingInstructions = EditingInstructions ?? currentDefinition.EditingInstructions;

            currentDefinition.IsDefined = true;

            if (AuthorizedRoles != null)
                currentDefinition.AuthorizedRoles = AuthorizedRoles;
            if (RequiredPermission != Permission.None)
                currentDefinition.RequiredPermission = RequiredPermission;
        }

        #endregion
    }
}
