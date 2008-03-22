using System;
using System.Collections.Generic;
using N2.Definitions;

namespace N2
{
    /// <summary>
    /// Decoration for N2 content items. Provides information needed in edit 
    /// mode and for data integrity.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
	public class DefinitionAttribute : Attribute, IDefinitionRefiner
    {
		private string title;
		private string name;
		private int sortOrder = 0;
		private string toolTip = string.Empty;
		private string description = string.Empty;
    	private bool mayBeRoot = false;
    	private bool mayBeStartPage = false;
		
		public DefinitionAttribute()
		{
		}

		/// <summary>Initializes a new instance of ItemAttribute class.</summary>
		/// <param name="title">The title used when presenting this item type to editors.</param>
		public DefinitionAttribute(string title)
		{
			this.title = title;
		}

		/// <summary>Initializes a new instance of ItemAttribute class.</summary>
		/// <param name="title">The title used when presenting this item type to editors.</param>
		/// <param name="name">The name/discriminator needed to map the appropriate type with content data when retrieving from persistence. When this is null the type's full name is used.</param>
		public DefinitionAttribute(string title, string name)
		{
			this.title = title;
			this.name = name;
		}

		/// <summary>Initializes a new instance of ItemAttribute class.</summary>
		/// <param name="title">The title used when presenting this item type to editors.</param>
		/// <param name="name">The name/discriminator needed to map the appropriate type with content data when retrieving from persistence. When this is null the type's name is used.</param>
		/// <param name="description">The description of this item.</param>
		/// <param name="toolTip">The tool tip displayed when hovering over this item type.</param>
		/// <param name="sortOrder">The order of this type compared to other items types.</param>
		public DefinitionAttribute(string title, string name, string description, string toolTip, int sortOrder)
		{
			this.title = title;
			this.name = name;
			this.description = description;
			this.toolTip = toolTip;
			this.sortOrder = sortOrder;
		}

		/// <summary>Gets or sets the name used when presenting this item type to editors.</summary>
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

		/// <summary>Gets or sets the name/discriminator needed to map the appropriate type with content data when retrieving from persistence. When this is null the type's full name is used.</summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

		/// <summary>Gets or sets the description of this item.</summary>
		public string Description
		{
			get { return description; }
			set { description = value; }
		}

		/// <summary>Gets or sets the order of this item type when selecting new item in edit mode.</summary>
		public int SortOrder
		{
			get { return sortOrder; }
			set { sortOrder = value; }
		}

		/// <summary>Gets or sets the tooltip used when presenting this item type to editors.</summary>
		public string ToolTip
		{
			get { return toolTip; }
			set { toolTip = value; }
		}

		/// <summary>Gets or sets wether this items should be considered as start page by the installer.</summary>
		public bool MayBeStartPage
    	{
    		get { return mayBeStartPage; }
    		set { mayBeStartPage = value; }
    	}

		/// <summary>Gets or sets wether this items should be considered as root by the installer.</summary>
    	public bool MayBeRoot
    	{
    		get { return mayBeRoot; }
    		set { mayBeRoot = value; }
    	}

		/// <summary>Updates the item definition with the attribute.</summary>
		public void Refine(ItemDefinition definition, IList<ItemDefinition> allDefinitions)
    	{
			if (string.IsNullOrEmpty(Title))
				Title = definition.ItemType.Name;

			definition.DefinitionAttribute = this;
			definition.IsDefined = true;
    	}
    }
}
