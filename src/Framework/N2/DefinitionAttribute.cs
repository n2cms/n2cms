using System;
using N2.Definitions;
using N2.Installation;

namespace N2
{
	/// <summary>
	/// Decoration for N2 content items. Provides information needed in edit 
	/// mode and for data integrity.
	/// </summary>
	/// <remarks>This attribute may be deprecated in the future. Use <see cref="PageDefinitionAttribute"/> or <see cref="PartDefinitionAttribute"/> instead.</remarks>
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public class DefinitionAttribute : AbstractDefinition, ISimpleDefinitionRefiner

	{
		public DefinitionAttribute()
		{
			Installer = InstallerHint.Default;
			IsPage = true;
			IconUrl = "{ManagementUrl}/Resources/icons/page.png";
		}

		/// <summary>Initializes a new instance of ItemAttribute class.</summary>
		/// <param name="title">The title used when presenting this item type to editors.</param>
		public DefinitionAttribute(string title)
			: this()
		{
			Title = title;
		}

		/// <summary>Initializes a new instance of ItemAttribute class.</summary>
		/// <param name="title">The title used when presenting this item type to editors.</param>
		/// <param name="name">The name/discriminator needed to map the appropriate type with content data when retrieving from persistence. When this is null the type's full name is used.</param>
		public DefinitionAttribute(string title, string name)
			: this(title)
		{
			Name = name;
		}

		/// <summary>Initializes a new instance of ItemAttribute class.</summary>
		/// <param name="title">The title used when presenting this item type to editors.</param>
		/// <param name="name">The name/discriminator needed to map the appropriate type with content data when retrieving from persistence. When this is null the type's name is used.</param>
		/// <param name="description">The description of this item.</param>
		/// <param name="toolTip">The tool tip displayed when hovering over this item type.</param>
		/// <param name="sortOrder">The order of this type compared to other items types.</param>
		public DefinitionAttribute(string title, string name, string description, string toolTip, int sortOrder)
			: this(title, name)
		{
			Description = description;
			ToolTip = toolTip;
			SortOrder = sortOrder;
		}

		/// <summary>
		/// Gets or sets how to treat this definition during installation.
		/// </summary>
		public InstallerHint Installer { get; set; }

		/// <summary>Updates the item definition with the attribute.</summary>
		public override void Refine(ItemDefinition definition)
		{
			base.Refine(definition);

			definition.Installer = Installer;
		}
	}
}