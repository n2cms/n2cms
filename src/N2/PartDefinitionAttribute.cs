using System;
using System.Collections.Generic;
using N2.Definitions;
using N2.Integrity;
using N2.Installation;

namespace N2
{
	/// <summary>
	/// Defines a part (ASCX) available to the CMS and provides a way to define 
	/// useful meta-data. Unlike <see cref="DefinitionAttribute"/> this attribute
	/// makes more assumptions about the item beeing defined and provides more
	/// meta-data options.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class PartDefinitionAttribute : AbstractDefinition
	{
		public AllowedZones AllowedIn { get; set; }
		public string[] AllowedZoneNames { get; set; }

		public PartDefinitionAttribute(string title)
			:this()
		{
			Title = title;
		}

		public PartDefinitionAttribute()
		{
			IsPage = false;
			IconUrl = "{ManagementUrl}/Resources/icons/page_white.png";
			AllowedIn = AllowedZones.AllNamed;
		}

		public override void Refine(ItemDefinition currentDefinition, IList<ItemDefinition> allDefinitions)
		{
			base.Refine(currentDefinition, allDefinitions);

			currentDefinition.AllowedIn = AllowedIn;
			if (AllowedZoneNames != null)
				foreach (string zoneName in AllowedZoneNames)
					currentDefinition.AddAllowedZone(zoneName);
			currentDefinition.Installer = InstallerHint.NeverRootOrStartPage;
		}
	}
}